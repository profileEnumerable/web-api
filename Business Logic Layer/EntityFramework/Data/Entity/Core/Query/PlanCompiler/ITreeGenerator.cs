// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ITreeGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class ITreeGenerator : DbExpressionVisitor<System.Data.Entity.Core.Query.InternalTrees.Node>
  {
    private static readonly Dictionary<DbExpressionKind, OpType> _opMap = ITreeGenerator.InitializeExpressionKindToOpTypeMap();
    private readonly Stack<ITreeGenerator.CqtVariableScope> _varScopes = new Stack<ITreeGenerator.CqtVariableScope>();
    private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, Var> _varMap = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, Var>();
    private readonly Stack<EdmFunction> _functionExpansions = new Stack<EdmFunction>();
    private readonly Dictionary<DbExpression, bool> _functionsIsPredicateFlag = new Dictionary<DbExpression, bool>();
    private readonly HashSet<DbFilterExpression> _processedIsOfFilters = new HashSet<DbFilterExpression>();
    private readonly HashSet<DbTreatExpression> _fakeTreats = new HashSet<DbTreatExpression>();
    private readonly bool _useDatabaseNullSemantics;
    private readonly Command _iqtCommand;
    private readonly DiscriminatorMap _discriminatorMap;
    private readonly DbProjectExpression _discriminatedViewTopProject;

    private static Dictionary<DbExpressionKind, OpType> InitializeExpressionKindToOpTypeMap()
    {
      return new Dictionary<DbExpressionKind, OpType>(12)
      {
        [DbExpressionKind.Plus] = OpType.Plus,
        [DbExpressionKind.Minus] = OpType.Minus,
        [DbExpressionKind.Multiply] = OpType.Multiply,
        [DbExpressionKind.Divide] = OpType.Divide,
        [DbExpressionKind.Modulo] = OpType.Modulo,
        [DbExpressionKind.UnaryMinus] = OpType.UnaryMinus,
        [DbExpressionKind.Equals] = OpType.EQ,
        [DbExpressionKind.NotEquals] = OpType.NE,
        [DbExpressionKind.LessThan] = OpType.LT,
        [DbExpressionKind.GreaterThan] = OpType.GT,
        [DbExpressionKind.LessThanOrEquals] = OpType.LE,
        [DbExpressionKind.GreaterThanOrEquals] = OpType.GE
      };
    }

    internal Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, Var> VarMap
    {
      get
      {
        return this._varMap;
      }
    }

    public static Command Generate(DbQueryCommandTree ctree)
    {
      return ITreeGenerator.Generate(ctree, (DiscriminatorMap) null);
    }

    internal static Command Generate(
      DbQueryCommandTree ctree,
      DiscriminatorMap discriminatorMap)
    {
      return new ITreeGenerator(ctree, discriminatorMap)._iqtCommand;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private ITreeGenerator(DbQueryCommandTree ctree, DiscriminatorMap discriminatorMap)
    {
      this._useDatabaseNullSemantics = ctree.UseDatabaseNullSemantics;
      this._iqtCommand = new Command(ctree.MetadataWorkspace);
      if (discriminatorMap != null)
      {
        this._discriminatorMap = discriminatorMap;
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(ctree.Query.ExpressionKind == DbExpressionKind.Project, "top level QMV expression must be project to match discriminator pattern");
        this._discriminatedViewTopProject = (DbProjectExpression) ctree.Query;
      }
      foreach (KeyValuePair<string, TypeUsage> parameter in ctree.Parameters)
      {
        if (!ITreeGenerator.ValidateParameterType(parameter.Value))
          throw new NotSupportedException(Strings.ParameterTypeNotSupported((object) parameter.Key, (object) parameter.Value.ToString()));
        this._iqtCommand.CreateParameterVar(parameter.Key, parameter.Value);
      }
      this._iqtCommand.Root = this.VisitExpr(ctree.Query);
      if (!this._iqtCommand.Root.Op.IsRelOp)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node scalarOpTree = this.ConvertToScalarOpTree(this._iqtCommand.Root, ctree.Query);
        System.Data.Entity.Core.Query.InternalTrees.Node node1 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateSingleRowTableOp());
        Var computedVar;
        System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = this._iqtCommand.CreateVarDefListNode(scalarOpTree, out computedVar);
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateProjectOp(computedVar), node1, varDefListNode);
        if (TypeSemantics.IsCollectionType(this._iqtCommand.Root.Op.Type))
        {
          UnnestOp unnestOp = this._iqtCommand.CreateUnnestOp(computedVar);
          node2 = this._iqtCommand.CreateNode((Op) unnestOp, varDefListNode.Child0);
          computedVar = unnestOp.Table.Columns[0];
        }
        this._iqtCommand.Root = node2;
        this._varMap[this._iqtCommand.Root] = computedVar;
      }
      this._iqtCommand.Root = this.CapWithPhysicalProject(this._iqtCommand.Root);
    }

    private static bool ValidateParameterType(TypeUsage paramType)
    {
      if (paramType == null || paramType.EdmType == null)
        return false;
      if (!TypeSemantics.IsPrimitiveType(paramType))
        return paramType.EdmType is EnumType;
      return true;
    }

    private static RowType ExtractElementRowType(TypeUsage typeUsage)
    {
      return TypeHelpers.GetEdmType<RowType>(TypeHelpers.GetEdmType<CollectionType>(typeUsage).TypeUsage);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IsPredicate")]
    private bool IsPredicate(DbExpression expr)
    {
      if (!TypeSemantics.IsPrimitiveType(expr.ResultType, PrimitiveTypeKind.Boolean))
        return false;
      switch (expr.ExpressionKind)
      {
        case DbExpressionKind.All:
        case DbExpressionKind.And:
        case DbExpressionKind.Any:
        case DbExpressionKind.Equals:
        case DbExpressionKind.GreaterThan:
        case DbExpressionKind.GreaterThanOrEquals:
        case DbExpressionKind.IsEmpty:
        case DbExpressionKind.IsNull:
        case DbExpressionKind.IsOf:
        case DbExpressionKind.IsOfOnly:
        case DbExpressionKind.LessThan:
        case DbExpressionKind.LessThanOrEquals:
        case DbExpressionKind.Like:
        case DbExpressionKind.Not:
        case DbExpressionKind.NotEquals:
        case DbExpressionKind.Or:
        case DbExpressionKind.In:
          return true;
        case DbExpressionKind.Function:
          if (!((DbFunctionExpression) expr).Function.HasUserDefinedBody)
            return false;
          bool flag1;
          if (this._functionsIsPredicateFlag.TryGetValue(expr, out flag1))
            return flag1;
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "IsPredicate must be called on a visited function expression");
          return false;
        case DbExpressionKind.VariableReference:
          DbVariableReferenceExpression e = (DbVariableReferenceExpression) expr;
          return this.ResolveScope(e).IsPredicate(e.VariableName);
        case DbExpressionKind.Lambda:
          bool flag2;
          if (this._functionsIsPredicateFlag.TryGetValue(expr, out flag2))
            return flag2;
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "IsPredicate must be called on a visited lambda expression");
          return false;
        default:
          return false;
      }
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node VisitExpr(DbExpression e)
    {
      return e?.Accept<System.Data.Entity.Core.Query.InternalTrees.Node>((DbExpressionVisitor<System.Data.Entity.Core.Query.InternalTrees.Node>) this);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node VisitExprAsScalar(DbExpression expr)
    {
      if (expr == null)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      return this.ConvertToScalarOpTree(this.VisitExpr(expr), expr);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node ConvertToScalarOpTree(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      DbExpression expr)
    {
      if (node.Op.IsRelOp)
        node = this.ConvertRelOpToScalarOpTree(node, expr.ResultType);
      else if (this.IsPredicate(expr))
        node = this.ConvertPredicateToScalarOpTree(node, expr);
      return node;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RelOp")]
    private System.Data.Entity.Core.Query.InternalTrees.Node ConvertRelOpToScalarOpTree(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      TypeUsage resultType)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(resultType), "RelOp with non-Collection result type");
      node = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateCollectOp(resultType), this.CapWithPhysicalProject(node));
      return node;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node ConvertPredicateToScalarOpTree(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      DbExpression expr)
    {
      CaseOp caseOp = this._iqtCommand.CreateCaseOp(this._iqtCommand.BooleanType);
      bool flag = this.IsNullable(expr);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>(flag ? 5 : 3);
      args.Add(node);
      args.Add(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateInternalConstantOp(this._iqtCommand.BooleanType, (object) true)));
      if (flag)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.VisitExpr(expr);
        args.Add(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.Not), node1));
      }
      args.Add(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateInternalConstantOp(this._iqtCommand.BooleanType, (object) false)));
      if (flag)
        args.Add(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateNullOp(this._iqtCommand.BooleanType)));
      node = this._iqtCommand.CreateNode((Op) caseOp, args);
      return node;
    }

    private bool IsNullable(DbExpression expression)
    {
      switch (expression.ExpressionKind)
      {
        case DbExpressionKind.All:
        case DbExpressionKind.Any:
        case DbExpressionKind.IsEmpty:
        case DbExpressionKind.IsNull:
          return false;
        case DbExpressionKind.And:
        case DbExpressionKind.Or:
          DbBinaryExpression binaryExpression = (DbBinaryExpression) expression;
          if (!this.IsNullable(binaryExpression.Left))
            return this.IsNullable(binaryExpression.Right);
          return true;
        case DbExpressionKind.Not:
          return this.IsNullable(((DbUnaryExpression) expression).Argument);
        default:
          return true;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "relOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node VisitExprAsPredicate(
      DbExpression expr)
    {
      if (expr == null)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.VisitExpr(expr);
      if (!this.IsPredicate(expr))
      {
        ComparisonOp comparisonOp = this._iqtCommand.CreateComparisonOp(OpType.EQ, false);
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateInternalConstantOp(this._iqtCommand.BooleanType, (object) true));
        node1 = this._iqtCommand.CreateNode((Op) comparisonOp, node1, node2);
      }
      else
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!node1.Op.IsRelOp, "unexpected relOp as predicate?");
      return node1;
    }

    private static IList<System.Data.Entity.Core.Query.InternalTrees.Node> VisitExpr(
      IList<DbExpression> exprs,
      ITreeGenerator.VisitExprDelegate exprDelegate)
    {
      List<System.Data.Entity.Core.Query.InternalTrees.Node> nodeList = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      for (int index = 0; index < exprs.Count; ++index)
        nodeList.Add(exprDelegate(exprs[index]));
      return (IList<System.Data.Entity.Core.Query.InternalTrees.Node>) nodeList;
    }

    private IList<System.Data.Entity.Core.Query.InternalTrees.Node> VisitExprAsScalar(
      IList<DbExpression> exprs)
    {
      return ITreeGenerator.VisitExpr(exprs, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsScalar));
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node VisitUnary(
      DbUnaryExpression e,
      Op op,
      ITreeGenerator.VisitExprDelegate exprDelegate)
    {
      return this._iqtCommand.CreateNode(op, exprDelegate(e.Argument));
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node VisitBinary(
      DbBinaryExpression e,
      Op op,
      ITreeGenerator.VisitExprDelegate exprDelegate)
    {
      return this._iqtCommand.CreateNode(op, exprDelegate(e.Left), exprDelegate(e.Right));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CollectOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ScalarOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PhysicalProjectOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RelOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-ScalarOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-RelOp")]
    private System.Data.Entity.Core.Query.InternalTrees.Node EnsureRelOp(System.Data.Entity.Core.Query.InternalTrees.Node inputNode)
    {
      Op op = inputNode.Op;
      if (op.IsRelOp)
        return inputNode;
      ScalarOp scalarOp = op as ScalarOp;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(scalarOp != null, "An expression in a CQT produced a non-ScalarOp and non-RelOp output Op");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(scalarOp.Type), "An expression used as a RelOp argument was neither a RelOp or a collection");
      if (op is CollectOp)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(inputNode.HasChild0, "CollectOp without argument");
        if (inputNode.Child0.Op is PhysicalProjectOp)
        {
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(inputNode.Child0.HasChild0, "PhysicalProjectOp without argument");
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(inputNode.Child0.Child0.Op.IsRelOp, "PhysicalProjectOp applied to non-RelOp input");
          return inputNode.Child0.Child0;
        }
      }
      Var computedVar1;
      System.Data.Entity.Core.Query.InternalTrees.Node varDefNode = this._iqtCommand.CreateVarDefNode(inputNode, out computedVar1);
      UnnestOp unnestOp = this._iqtCommand.CreateUnnestOp(computedVar1);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(unnestOp.Table.Columns.Count == 1, "Un-nest of collection ScalarOp produced unexpected number of columns (1 expected)");
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this._iqtCommand.CreateNode((Op) unnestOp, varDefNode);
      this._varMap[node1] = unnestOp.Table.Columns[0];
      Var computedVar2;
      System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = this._iqtCommand.CreateVarDefListNode(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarRefOp(unnestOp.Table.Columns[0])), out computedVar2);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateProjectOp(computedVar2), node1, varDefListNode);
      this._varMap[node2] = computedVar2;
      return node2;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-RelOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node CapWithProject(System.Data.Entity.Core.Query.InternalTrees.Node input)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(input.Op.IsRelOp, "unexpected non-RelOp?");
      if (input.Op.OpType == OpType.Project)
        return input;
      Var var = this._varMap[input];
      System.Data.Entity.Core.Query.InternalTrees.Node node = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateProjectOp(var), input, this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarDefListOp()));
      this._varMap[node] = var;
      return node;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-RelOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node CapWithPhysicalProject(System.Data.Entity.Core.Query.InternalTrees.Node input)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(input.Op.IsRelOp, "unexpected non-RelOp?");
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreatePhysicalProjectOp(this._varMap[input]), input);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node EnterExpressionBinding(
      DbExpressionBinding binding)
    {
      return this.VisitBoundExpressionPushBindingScope(binding.Expression, binding.VariableName);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node EnterGroupExpressionBinding(
      DbGroupExpressionBinding binding)
    {
      return this.VisitBoundExpressionPushBindingScope(binding.Expression, binding.VariableName);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node VisitBoundExpressionPushBindingScope(
      DbExpression boundExpression,
      string bindingName)
    {
      Var boundVar;
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.VisitBoundExpression(boundExpression, out boundVar);
      this.PushBindingScope(boundVar, bindingName);
      return node;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbExpressionBinding")]
    private System.Data.Entity.Core.Query.InternalTrees.Node VisitBoundExpression(
      DbExpression boundExpression,
      out Var boundVar)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode = this.VisitExpr(boundExpression);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(inputNode != null, "DbExpressionBinding.Expression produced null conversion");
      System.Data.Entity.Core.Query.InternalTrees.Node index = this.EnsureRelOp(inputNode);
      boundVar = this._varMap[index];
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(boundVar != null, "No Var found for Input Op");
      return index;
    }

    private void PushBindingScope(Var boundVar, string bindingName)
    {
      this._varScopes.Push((ITreeGenerator.CqtVariableScope) new ITreeGenerator.ExpressionBindingScope(this._iqtCommand, bindingName, boundVar));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExpressionBindingScope")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExitExpressionBinding")]
    private ITreeGenerator.ExpressionBindingScope ExitExpressionBinding()
    {
      ITreeGenerator.ExpressionBindingScope expressionBindingScope = this._varScopes.Pop() as ITreeGenerator.ExpressionBindingScope;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(expressionBindingScope != null, "ExitExpressionBinding called without ExpressionBindingScope on top of scope stack");
      return expressionBindingScope;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExitGroupExpressionBinding")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExpressionBindingScope")]
    private void ExitGroupExpressionBinding()
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this._varScopes.Pop() is ITreeGenerator.ExpressionBindingScope, "ExitGroupExpressionBinding called without ExpressionBindingScope on top of scope stack");
    }

    private void EnterLambdaFunction(
      DbLambda lambda,
      List<Tuple<System.Data.Entity.Core.Query.InternalTrees.Node, bool>> argumentValues,
      EdmFunction expandingEdmFunction)
    {
      IList<DbVariableReferenceExpression> variables = lambda.Variables;
      Dictionary<string, Tuple<System.Data.Entity.Core.Query.InternalTrees.Node, bool>> args = new Dictionary<string, Tuple<System.Data.Entity.Core.Query.InternalTrees.Node, bool>>();
      int index = 0;
      foreach (Tuple<System.Data.Entity.Core.Query.InternalTrees.Node, bool> argumentValue in argumentValues)
      {
        args.Add(variables[index].VariableName, argumentValue);
        ++index;
      }
      if (expandingEdmFunction != null)
      {
        if (this._functionExpansions.Contains(expandingEdmFunction))
          throw new EntityCommandCompilationException(Strings.Cqt_UDF_FunctionDefinitionWithCircularReference((object) expandingEdmFunction.FullName), (Exception) null);
        this._functionExpansions.Push(expandingEdmFunction);
      }
      this._varScopes.Push((ITreeGenerator.CqtVariableScope) new ITreeGenerator.LambdaScope(this, this._iqtCommand, args));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExitLambdaFunction")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LambdaScope")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private ITreeGenerator.LambdaScope ExitLambdaFunction(
      EdmFunction expandingEdmFunction)
    {
      ITreeGenerator.LambdaScope lambdaScope = this._varScopes.Pop() as ITreeGenerator.LambdaScope;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(lambdaScope != null, "ExitLambdaFunction called without LambdaScope on top of scope stack");
      if (expandingEdmFunction != null)
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this._functionExpansions.Pop() == expandingEdmFunction, "Function expansion stack corruption: unexpected function at the top of the stack");
      return lambdaScope;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node ProjectNewRecord(
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode,
      RowType recType,
      IEnumerable<Var> colVars)
    {
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (Var colVar in colVars)
        args.Add(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarRefOp(colVar)));
      Var computedVar;
      System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = this._iqtCommand.CreateVarDefListNode(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateNewRecordOp(recType), args), out computedVar);
      System.Data.Entity.Core.Query.InternalTrees.Node node = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateProjectOp(computedVar), inputNode, varDefListNode);
      this._varMap[node] = computedVar;
      return node;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbExpression e)
    {
      Check.NotNull<DbExpression>(e, nameof (e));
      throw new NotSupportedException(Strings.Cqt_General_UnsupportedExpression((object) e.GetType().FullName));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbConstantExpression e)
    {
      Check.NotNull<DbConstantExpression>(e, nameof (e));
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConstantOp(e.ResultType, e.GetValue()));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbNullExpression e)
    {
      Check.NotNull<DbNullExpression>(e, nameof (e));
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateNullOp(e.ResultType));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbVariableReferenceExpression e)
    {
      Check.NotNull<DbVariableReferenceExpression>(e, nameof (e));
      return this.ResolveScope(e)[e.VariableName];
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarRef")]
    private ITreeGenerator.CqtVariableScope ResolveScope(
      DbVariableReferenceExpression e)
    {
      foreach (ITreeGenerator.CqtVariableScope varScope in this._varScopes)
      {
        if (varScope.Contains(e.VariableName))
          return varScope;
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "CQT VarRef could not be resolved in the variable scope stack");
      return (ITreeGenerator.CqtVariableScope) null;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbParameterReferenceExpression e)
    {
      Check.NotNull<DbParameterReferenceExpression>(e, nameof (e));
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarRefOp((Var) this._iqtCommand.GetParameter(e.ParameterName)));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbFunctionExpression e)
    {
      Check.NotNull<DbFunctionExpression>(e, nameof (e));
      System.Data.Entity.Core.Query.InternalTrees.Node node;
      if (e.Function.IsModelDefinedFunction)
      {
        DbLambda functionDefinition;
        try
        {
          functionDefinition = this._iqtCommand.MetadataWorkspace.GetGeneratedFunctionDefinition(e.Function);
        }
        catch (Exception ex)
        {
          if (ex.IsCatchableExceptionType())
            throw new EntityCommandCompilationException(Strings.Cqt_UDF_FunctionDefinitionGenerationFailed((object) e.Function.FullName), ex);
          throw;
        }
        node = this.VisitLambdaExpression(functionDefinition, e.Arguments, (DbExpression) e, e.Function);
      }
      else
      {
        List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>(e.Arguments.Count);
        for (int index = 0; index < e.Arguments.Count; ++index)
          args.Add(this.BuildSoftCast(this.VisitExprAsScalar(e.Arguments[index]), e.Function.Parameters[index].TypeUsage));
        node = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateFunctionOp(e.Function), args);
      }
      return node;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbLambdaExpression e)
    {
      Check.NotNull<DbLambdaExpression>(e, nameof (e));
      return this.VisitLambdaExpression(e.Lambda, e.Arguments, (DbExpression) e, (EdmFunction) null);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node VisitLambdaExpression(
      DbLambda lambda,
      IList<DbExpression> arguments,
      DbExpression applicationExpr,
      EdmFunction expandingEdmFunction)
    {
      List<Tuple<System.Data.Entity.Core.Query.InternalTrees.Node, bool>> argumentValues = new List<Tuple<System.Data.Entity.Core.Query.InternalTrees.Node, bool>>(arguments.Count);
      foreach (DbExpression dbExpression in (IEnumerable<DbExpression>) arguments)
        argumentValues.Add(Tuple.Create<System.Data.Entity.Core.Query.InternalTrees.Node, bool>(this.VisitExpr(dbExpression), this.IsPredicate(dbExpression)));
      this.EnterLambdaFunction(lambda, argumentValues, expandingEdmFunction);
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.VisitExpr(lambda.Body);
      this._functionsIsPredicateFlag[applicationExpr] = this.IsPredicate(lambda.Body);
      this.ExitLambdaFunction(expandingEdmFunction);
      return node;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node BuildSoftCast(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      TypeUsage targetType)
    {
      if (node.Op.IsRelOp)
      {
        targetType = TypeHelpers.GetEdmType<CollectionType>(targetType).TypeUsage;
        Var var = this._varMap[node];
        if (Command.EqualTypes(targetType, var.Type))
          return node;
        System.Data.Entity.Core.Query.InternalTrees.Node node1 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarRefOp(var));
        Var computedVar;
        System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = this._iqtCommand.CreateVarDefListNode(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateSoftCastOp(targetType), node1), out computedVar);
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateProjectOp(computedVar), node, varDefListNode);
        this._varMap[node2] = computedVar;
        return node2;
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node.Op.IsScalarOp, "I want a scalar op");
      if (Command.EqualTypes(node.Op.Type, targetType))
        return node;
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateSoftCastOp(targetType), node);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node BuildSoftCast(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      EdmType targetType)
    {
      return this.BuildSoftCast(node, TypeUsage.Create(targetType));
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node BuildEntityRef(
      System.Data.Entity.Core.Query.InternalTrees.Node arg,
      TypeUsage entityType)
    {
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateGetEntityRefOp(TypeHelpers.CreateReferenceTypeUsage((EntityType) entityType.EdmType)), arg);
    }

    private static bool TryRewriteKeyPropertyAccess(
      DbPropertyExpression propertyExpression,
      out DbExpression rewritten)
    {
      if (propertyExpression.Instance.ExpressionKind == DbExpressionKind.Property && Helper.IsEntityType(propertyExpression.Instance.ResultType.EdmType))
      {
        EntityType edmType = (EntityType) propertyExpression.Instance.ResultType.EdmType;
        DbPropertyExpression instance = (DbPropertyExpression) propertyExpression.Instance;
        if (Helper.IsNavigationProperty(instance.Property) && edmType.KeyMembers.Contains(propertyExpression.Property))
        {
          NavigationProperty property = (NavigationProperty) instance.Property;
          DbExpression dbExpression = (DbExpression) instance.Instance.GetEntityRef().Navigate(property.FromEndMember, property.ToEndMember);
          rewritten = (DbExpression) dbExpression.GetRefKey();
          rewritten = (DbExpression) rewritten.Property(propertyExpression.Property.Name);
          return true;
        }
      }
      rewritten = (DbExpression) null;
      return false;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbPropertyExpression e)
    {
      Check.NotNull<DbPropertyExpression>(e, nameof (e));
      if (BuiltInTypeKind.EdmProperty != e.Property.BuiltInTypeKind && e.Property.BuiltInTypeKind != BuiltInTypeKind.AssociationEndMember && BuiltInTypeKind.NavigationProperty != e.Property.BuiltInTypeKind)
        throw new NotSupportedException();
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(e.Instance != null, "Static properties are not supported");
      DbExpression rewritten;
      System.Data.Entity.Core.Query.InternalTrees.Node node1;
      if (ITreeGenerator.TryRewriteKeyPropertyAccess(e, out rewritten))
      {
        node1 = this.VisitExpr(rewritten);
      }
      else
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.VisitExpr(e.Instance);
        if (e.Instance.ExpressionKind == DbExpressionKind.NewInstance && Helper.IsStructuralType(e.Instance.ResultType.EdmType))
        {
          IList structuralMembers = Helper.GetAllStructuralMembers(e.Instance.ResultType.EdmType);
          int index1 = -1;
          for (int index2 = 0; index2 < structuralMembers.Count; ++index2)
          {
            if (string.Equals(e.Property.Name, ((EdmMember) structuralMembers[index2]).Name, StringComparison.Ordinal))
            {
              index1 = index2;
              break;
            }
          }
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(index1 > -1, "The specified property was not found");
          node1 = this.BuildSoftCast(node2.Children[index1], e.ResultType);
        }
        else
          node1 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreatePropertyOp(e.Property), this.BuildSoftCast(node2, (EdmType) e.Property.DeclaringType));
      }
      return node1;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbComparisonExpression e)
    {
      Check.NotNull<DbComparisonExpression>(e, nameof (e));
      Op comparisonOp = (Op) this._iqtCommand.CreateComparisonOp(ITreeGenerator._opMap[e.ExpressionKind], false);
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.VisitExprAsScalar(e.Left);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.VisitExprAsScalar(e.Right);
      TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(e.Left.ResultType, e.Right.ResultType);
      if (!Command.EqualTypes(e.Left.ResultType, e.Right.ResultType))
      {
        node1 = this.BuildSoftCast(node1, commonTypeUsage);
        node2 = this.BuildSoftCast(node2, commonTypeUsage);
      }
      if (TypeSemantics.IsEntityType(commonTypeUsage) && (e.ExpressionKind == DbExpressionKind.Equals || e.ExpressionKind == DbExpressionKind.NotEquals))
      {
        node1 = this.BuildEntityRef(node1, commonTypeUsage);
        node2 = this.BuildEntityRef(node2, commonTypeUsage);
      }
      return this._iqtCommand.CreateNode(comparisonOp, node1, node2);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbLikeExpression e)
    {
      Check.NotNull<DbLikeExpression>(e, nameof (e));
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateLikeOp(), this.VisitExpr(e.Argument), this.VisitExpr(e.Pattern), this.VisitExpr(e.Escape));
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CreateLimitNode(
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode,
      System.Data.Entity.Core.Query.InternalTrees.Node limitNode,
      bool withTies)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node;
      if (OpType.ConstrainedSort == inputNode.Op.OpType && OpType.Null == inputNode.Child2.Op.OpType)
      {
        inputNode.Child2 = limitNode;
        if (withTies)
          ((ConstrainedSortOp) inputNode.Op).WithTies = true;
        node = inputNode;
      }
      else
        node = OpType.Sort != inputNode.Op.OpType ? this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConstrainedSortOp(new List<SortKey>(), withTies), inputNode, this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateNullOp(this._iqtCommand.IntegerType)), limitNode) : this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConstrainedSortOp(((SortBaseOp) inputNode.Op).Keys, withTies), inputNode.Child0, this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateNullOp(this._iqtCommand.IntegerType)), limitNode);
      return node;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbLimitExpression expression)
    {
      Check.NotNull<DbLimitExpression>(expression, nameof (expression));
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode = this.EnsureRelOp(this.VisitExpr(expression.Argument));
      Var var = this._varMap[inputNode];
      System.Data.Entity.Core.Query.InternalTrees.Node limitNode = this.VisitExprAsScalar(expression.Limit);
      System.Data.Entity.Core.Query.InternalTrees.Node index;
      if (OpType.Project == inputNode.Op.OpType && (inputNode.Child0.Op.OpType == OpType.Sort || inputNode.Child0.Op.OpType == OpType.ConstrainedSort))
      {
        inputNode.Child0 = this.CreateLimitNode(inputNode.Child0, limitNode, expression.WithTies);
        index = inputNode;
      }
      else
        index = this.CreateLimitNode(inputNode, limitNode, expression.WithTies);
      if (!object.ReferenceEquals((object) index, (object) inputNode))
        this._varMap[index] = var;
      return index;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbIsNullExpression e)
    {
      Check.NotNull<DbIsNullExpression>(e, nameof (e));
      bool flag = false;
      if (e.Argument.ExpressionKind == DbExpressionKind.IsNull)
        flag = true;
      else if (e.Argument.ExpressionKind == DbExpressionKind.Not && ((DbUnaryExpression) e.Argument).Argument.ExpressionKind == DbExpressionKind.IsNull)
        flag = true;
      Op conditionalOp = (Op) this._iqtCommand.CreateConditionalOp(OpType.IsNull);
      if (flag)
        return this._iqtCommand.CreateNode(conditionalOp, this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateInternalConstantOp(this._iqtCommand.BooleanType, (object) true)));
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.VisitExprAsScalar(e.Argument);
      if (TypeSemantics.IsEntityType(e.Argument.ResultType))
        node = this.BuildEntityRef(node, e.Argument.ResultType);
      return this._iqtCommand.CreateNode(conditionalOp, node);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbArithmeticExpression e)
    {
      Check.NotNull<DbArithmeticExpression>(e, nameof (e));
      Op arithmeticOp = (Op) this._iqtCommand.CreateArithmeticOp(ITreeGenerator._opMap[e.ExpressionKind], e.ResultType);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (DbExpression expr in (IEnumerable<DbExpression>) e.Arguments)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node = this.VisitExprAsScalar(expr);
        args.Add(this.BuildSoftCast(node, e.ResultType));
      }
      return this._iqtCommand.CreateNode(arithmeticOp, args);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbAndExpression e)
    {
      Check.NotNull<DbAndExpression>(e, nameof (e));
      Op conditionalOp = (Op) this._iqtCommand.CreateConditionalOp(OpType.And);
      return this.VisitBinary((DbBinaryExpression) e, conditionalOp, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsPredicate));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbOrExpression e)
    {
      Check.NotNull<DbOrExpression>(e, nameof (e));
      Op conditionalOp = (Op) this._iqtCommand.CreateConditionalOp(OpType.Or);
      return this.VisitBinary((DbBinaryExpression) e, conditionalOp, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsPredicate));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbInExpression e)
    {
      Check.NotNull<DbInExpression>(e, nameof (e));
      Op conditionalOp = (Op) this._iqtCommand.CreateConditionalOp(OpType.In);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>(1 + e.List.Count)
      {
        this.VisitExpr(e.Item)
      };
      args.AddRange(e.List.Select<DbExpression, System.Data.Entity.Core.Query.InternalTrees.Node>(new Func<DbExpression, System.Data.Entity.Core.Query.InternalTrees.Node>(this.VisitExpr)));
      return this._iqtCommand.CreateNode(conditionalOp, args);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbNotExpression e)
    {
      Check.NotNull<DbNotExpression>(e, nameof (e));
      Op conditionalOp = (Op) this._iqtCommand.CreateConditionalOp(OpType.Not);
      return this.VisitUnary((DbUnaryExpression) e, conditionalOp, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsPredicate));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbDistinctExpression e)
    {
      Check.NotNull<DbDistinctExpression>(e, nameof (e));
      System.Data.Entity.Core.Query.InternalTrees.Node index = this.EnsureRelOp(this.VisitExpr(e.Argument));
      Var var = this._varMap[index];
      System.Data.Entity.Core.Query.InternalTrees.Node node = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateDistinctOp(var), index);
      this._varMap[node] = var;
      return node;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbElementExpression e)
    {
      Check.NotNull<DbElementExpression>(e, nameof (e));
      Op elementOp = (Op) this._iqtCommand.CreateElementOp(e.ResultType);
      System.Data.Entity.Core.Query.InternalTrees.Node index = this.BuildSoftCast(this.EnsureRelOp(this.VisitExpr(e.Argument)), TypeHelpers.CreateCollectionTypeUsage(e.ResultType));
      Var var = this._varMap[index];
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateSingleRowOp(), index);
      this._varMap[node1] = var;
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.CapWithProject(node1);
      return this._iqtCommand.CreateNode(elementOp, node2);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbIsEmptyExpression e)
    {
      Check.NotNull<DbIsEmptyExpression>(e, nameof (e));
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.Not), this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateExistsOp(), this.EnsureRelOp(this.VisitExpr(e.Argument))));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbExpression")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SetOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VisitSetOpExpression")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Non-SetOp")]
    private System.Data.Entity.Core.Query.InternalTrees.Node VisitSetOpExpression(
      DbBinaryExpression expression)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(DbExpressionKind.Except == expression.ExpressionKind || DbExpressionKind.Intersect == expression.ExpressionKind || DbExpressionKind.UnionAll == expression.ExpressionKind, "Non-SetOp DbExpression used as argument to VisitSetOpExpression");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(expression.ResultType), "SetOp DbExpression does not have collection result type?");
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.EnsureRelOp(this.VisitExpr(expression.Left));
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.EnsureRelOp(this.VisitExpr(expression.Right));
      System.Data.Entity.Core.Query.InternalTrees.Node index1 = this.BuildSoftCast(node1, expression.ResultType);
      System.Data.Entity.Core.Query.InternalTrees.Node index2 = this.BuildSoftCast(node2, expression.ResultType);
      Var setOpVar = (Var) this._iqtCommand.CreateSetOpVar(TypeHelpers.GetEdmType<CollectionType>(expression.ResultType).TypeUsage);
      System.Data.Entity.Core.Query.InternalTrees.VarMap leftMap = new System.Data.Entity.Core.Query.InternalTrees.VarMap();
      leftMap.Add(setOpVar, this._varMap[index1]);
      System.Data.Entity.Core.Query.InternalTrees.VarMap rightMap = new System.Data.Entity.Core.Query.InternalTrees.VarMap();
      rightMap.Add(setOpVar, this._varMap[index2]);
      Op op = (Op) null;
      switch (expression.ExpressionKind)
      {
        case DbExpressionKind.Except:
          op = (Op) this._iqtCommand.CreateExceptOp(leftMap, rightMap);
          break;
        case DbExpressionKind.Intersect:
          op = (Op) this._iqtCommand.CreateIntersectOp(leftMap, rightMap);
          break;
        case DbExpressionKind.UnionAll:
          op = (Op) this._iqtCommand.CreateUnionAllOp(leftMap, rightMap);
          break;
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = this._iqtCommand.CreateNode(op, index1, index2);
      this._varMap[node3] = setOpVar;
      return node3;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbUnionAllExpression e)
    {
      Check.NotNull<DbUnionAllExpression>(e, nameof (e));
      return this.VisitSetOpExpression((DbBinaryExpression) e);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbIntersectExpression e)
    {
      Check.NotNull<DbIntersectExpression>(e, nameof (e));
      return this.VisitSetOpExpression((DbBinaryExpression) e);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbExceptExpression e)
    {
      Check.NotNull<DbExceptExpression>(e, nameof (e));
      return this.VisitSetOpExpression((DbBinaryExpression) e);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbTreatExpression e)
    {
      Check.NotNull<DbTreatExpression>(e, nameof (e));
      Op op = !this._fakeTreats.Contains(e) ? (Op) this._iqtCommand.CreateTreatOp(e.ResultType) : (Op) this._iqtCommand.CreateFakeTreatOp(e.ResultType);
      return this.VisitUnary((DbUnaryExpression) e, op, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsScalar));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbIsOfExpression e)
    {
      Check.NotNull<DbIsOfExpression>(e, nameof (e));
      Op op = DbExpressionKind.IsOfOnly != e.ExpressionKind ? (Op) this._iqtCommand.CreateIsOfOp(e.OfType) : (Op) this._iqtCommand.CreateIsOfOnlyOp(e.OfType);
      return this.VisitUnary((DbUnaryExpression) e, op, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsScalar));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbCastExpression e)
    {
      Check.NotNull<DbCastExpression>(e, nameof (e));
      Op castOp = (Op) this._iqtCommand.CreateCastOp(e.ResultType);
      return this.VisitUnary((DbUnaryExpression) e, castOp, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsScalar));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbCaseExpression e)
    {
      Check.NotNull<DbCaseExpression>(e, nameof (e));
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      for (int index = 0; index < e.When.Count; ++index)
      {
        args.Add(this.VisitExprAsPredicate(e.When[index]));
        args.Add(this.BuildSoftCast(this.VisitExprAsScalar(e.Then[index]), e.ResultType));
      }
      args.Add(this.BuildSoftCast(this.VisitExprAsScalar(e.Else), e.ResultType));
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateCaseOp(e.ResultType), args);
    }

    private DbFilterExpression CreateIsOfFilterExpression(
      DbExpression input,
      ITreeGenerator.IsOfFilter typeFilter)
    {
      DbExpressionBinding resultBinding = input.Bind();
      DbExpression predicate = Helpers.BuildBalancedTreeInPlace<DbExpression>((IList<DbExpression>) new List<DbExpression>((IEnumerable<DbExpression>) typeFilter.ToEnumerable().Select<KeyValuePair<TypeUsage, bool>, DbIsOfExpression>((Func<KeyValuePair<TypeUsage, bool>, DbIsOfExpression>) (tf =>
      {
        if (!tf.Value)
          return resultBinding.Variable.IsOf(tf.Key);
        return resultBinding.Variable.IsOfOnly(tf.Key);
      })).ToList<DbIsOfExpression>()), (Func<DbExpression, DbExpression, DbExpression>) ((left, right) => (DbExpression) left.And(right)));
      DbFilterExpression filterExpression = resultBinding.Filter(predicate);
      this._processedIsOfFilters.Add(filterExpression);
      return filterExpression;
    }

    private static bool IsIsOfFilter(DbFilterExpression filter)
    {
      if (filter.Predicate.ExpressionKind != DbExpressionKind.IsOf && filter.Predicate.ExpressionKind != DbExpressionKind.IsOfOnly)
        return false;
      DbExpression dbExpression = ((DbUnaryExpression) filter.Predicate).Argument;
      if (dbExpression.ExpressionKind == DbExpressionKind.VariableReference)
        return ((DbVariableReferenceExpression) dbExpression).VariableName == filter.Input.VariableName;
      return false;
    }

    private DbExpression ApplyIsOfFilter(
      DbExpression current,
      ITreeGenerator.IsOfFilter typeFilter)
    {
      DbExpression dbExpression;
      switch (current.ExpressionKind)
      {
        case DbExpressionKind.Distinct:
          dbExpression = (DbExpression) this.ApplyIsOfFilter(((DbUnaryExpression) current).Argument, typeFilter).Distinct();
          break;
        case DbExpressionKind.Filter:
          DbFilterExpression filter = (DbFilterExpression) current;
          if (ITreeGenerator.IsIsOfFilter(filter))
          {
            DbIsOfExpression predicate = (DbIsOfExpression) filter.Predicate;
            typeFilter = typeFilter.Merge(predicate);
            dbExpression = this.ApplyIsOfFilter(filter.Input.Expression, typeFilter);
            break;
          }
          dbExpression = (DbExpression) this.ApplyIsOfFilter(filter.Input.Expression, typeFilter).BindAs(filter.Input.VariableName).Filter(filter.Predicate);
          break;
        case DbExpressionKind.OfType:
        case DbExpressionKind.OfTypeOnly:
          DbOfTypeExpression other = (DbOfTypeExpression) current;
          typeFilter = typeFilter.Merge(other);
          DbExpressionBinding input = this.ApplyIsOfFilter(other.Argument, typeFilter).Bind();
          DbTreatExpression dbTreatExpression = input.Variable.TreatAs(other.OfType);
          this._fakeTreats.Add(dbTreatExpression);
          dbExpression = (DbExpression) input.Project((DbExpression) dbTreatExpression);
          break;
        case DbExpressionKind.Project:
          DbProjectExpression projectExpression = (DbProjectExpression) current;
          dbExpression = projectExpression.Projection.ExpressionKind != DbExpressionKind.VariableReference || !(((DbVariableReferenceExpression) projectExpression.Projection).VariableName == projectExpression.Input.VariableName) ? (DbExpression) this.CreateIsOfFilterExpression(current, typeFilter) : this.ApplyIsOfFilter(projectExpression.Input.Expression, typeFilter);
          break;
        case DbExpressionKind.Sort:
          DbSortExpression dbSortExpression = (DbSortExpression) current;
          dbExpression = (DbExpression) this.ApplyIsOfFilter(dbSortExpression.Input.Expression, typeFilter).BindAs(dbSortExpression.Input.VariableName).Sort((IEnumerable<DbSortClause>) dbSortExpression.SortOrder);
          break;
        default:
          dbExpression = (DbExpression) this.CreateIsOfFilterExpression(current, typeFilter);
          break;
      }
      return dbExpression;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbOfTypeExpression")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbOfTypeExpression e)
    {
      Check.NotNull<DbOfTypeExpression>(e, nameof (e));
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(e.Argument.ResultType), "Non-Collection Type Argument in DbOfTypeExpression");
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode = this.EnsureRelOp(this.VisitExpr(this.ApplyIsOfFilter(e.Argument, new ITreeGenerator.IsOfFilter(e))));
      Var var = this._varMap[inputNode];
      Var resultVar;
      System.Data.Entity.Core.Query.InternalTrees.Node index = this._iqtCommand.BuildFakeTreatProject(inputNode, var, e.OfType, out resultVar);
      this._varMap[index] = resultVar;
      return index;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbNewInstanceExpression e)
    {
      Check.NotNull<DbNewInstanceExpression>(e, nameof (e));
      List<System.Data.Entity.Core.Query.InternalTrees.Node> nodeList = (List<System.Data.Entity.Core.Query.InternalTrees.Node>) null;
      Op op;
      if (TypeSemantics.IsCollectionType(e.ResultType))
        op = (Op) this._iqtCommand.CreateNewMultisetOp(e.ResultType);
      else if (TypeSemantics.IsRowType(e.ResultType))
        op = (Op) this._iqtCommand.CreateNewRecordOp(e.ResultType);
      else if (TypeSemantics.IsEntityType(e.ResultType))
      {
        List<RelProperty> relProperties = new List<RelProperty>();
        nodeList = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
        if (e.HasRelatedEntityReferences)
        {
          foreach (DbRelatedEntityRef relatedEntityReference in e.RelatedEntityReferences)
          {
            RelProperty relProperty = new RelProperty((RelationshipType) relatedEntityReference.TargetEnd.DeclaringType, relatedEntityReference.SourceEnd, relatedEntityReference.TargetEnd);
            relProperties.Add(relProperty);
            System.Data.Entity.Core.Query.InternalTrees.Node node = this.VisitExprAsScalar(relatedEntityReference.TargetEntityReference);
            nodeList.Add(node);
          }
        }
        op = (Op) this._iqtCommand.CreateNewEntityOp(e.ResultType, relProperties);
      }
      else
        op = (Op) this._iqtCommand.CreateNewInstanceOp(e.ResultType);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      if (TypeSemantics.IsStructuralType(e.ResultType))
      {
        StructuralType edmType = TypeHelpers.GetEdmType<StructuralType>(e.ResultType);
        int index = 0;
        foreach (EdmMember structuralMember in (IEnumerable) TypeHelpers.GetAllStructuralMembers((EdmType) edmType))
        {
          System.Data.Entity.Core.Query.InternalTrees.Node node = this.BuildSoftCast(this.VisitExprAsScalar(e.Arguments[index]), Helper.GetModelTypeUsage(structuralMember));
          args.Add(node);
          ++index;
        }
      }
      else
      {
        TypeUsage typeUsage = TypeHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage;
        foreach (DbExpression expr in (IEnumerable<DbExpression>) e.Arguments)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node node = this.BuildSoftCast(this.VisitExprAsScalar(expr), typeUsage);
          args.Add(node);
        }
      }
      if (nodeList != null)
        args.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) nodeList);
      return this._iqtCommand.CreateNode(op, args);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbRefExpression e)
    {
      Check.NotNull<DbRefExpression>(e, nameof (e));
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateRefOp(e.EntitySet, e.ResultType), this.BuildSoftCast(this.VisitExprAsScalar(e.Argument), (EdmType) TypeHelpers.CreateKeyRowType((EntityTypeBase) e.EntitySet.ElementType)));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbRelationshipNavigationExpression e)
    {
      Check.NotNull<DbRelationshipNavigationExpression>(e, nameof (e));
      RelProperty relProperty = new RelProperty(e.Relationship, e.NavigateFrom, e.NavigateTo);
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateNavigateOp(e.ResultType, relProperty), this.VisitExprAsScalar(e.NavigationSource));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbDerefExpression e)
    {
      Check.NotNull<DbDerefExpression>(e, nameof (e));
      Op derefOp = (Op) this._iqtCommand.CreateDerefOp(e.ResultType);
      return this.VisitUnary((DbUnaryExpression) e, derefOp, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsScalar));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbRefKeyExpression e)
    {
      Check.NotNull<DbRefKeyExpression>(e, nameof (e));
      Op getRefKeyOp = (Op) this._iqtCommand.CreateGetRefKeyOp(e.ResultType);
      return this.VisitUnary((DbUnaryExpression) e, getRefKeyOp, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsScalar));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbEntityRefExpression e)
    {
      Check.NotNull<DbEntityRefExpression>(e, nameof (e));
      Op getEntityRefOp = (Op) this._iqtCommand.CreateGetEntityRefOp(e.ResultType);
      return this.VisitUnary((DbUnaryExpression) e, getEntityRefOp, new ITreeGenerator.VisitExprDelegate(this.VisitExprAsScalar));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbScanExpression e)
    {
      Check.NotNull<DbScanExpression>(e, nameof (e));
      ScanTableOp scanTableOp = this._iqtCommand.CreateScanTableOp(Command.CreateTableDefinition(e.Target));
      System.Data.Entity.Core.Query.InternalTrees.Node node = this._iqtCommand.CreateNode((Op) scanTableOp);
      Var column = scanTableOp.Table.Columns[0];
      this._varMap[node] = column;
      return node;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbFilterExpression e)
    {
      Check.NotNull<DbFilterExpression>(e, nameof (e));
      if (!ITreeGenerator.IsIsOfFilter(e) || this._processedIsOfFilters.Contains(e))
      {
        System.Data.Entity.Core.Query.InternalTrees.Node index = this.EnterExpressionBinding(e.Input);
        System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.VisitExprAsPredicate(e.Predicate);
        this.ExitExpressionBinding();
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateFilterOp(), index, node1);
        this._varMap[node2] = this._varMap[index];
        return node2;
      }
      DbIsOfExpression predicate = (DbIsOfExpression) e.Predicate;
      return this.VisitExpr(this.ApplyIsOfFilter(e.Input.Expression, new ITreeGenerator.IsOfFilter(predicate)));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbProjectExpression e)
    {
      Check.NotNull<DbProjectExpression>(e, nameof (e));
      if (e == this._discriminatedViewTopProject)
        return this.GenerateDiscriminatedProject(e);
      return this.GenerateStandardProject(e);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node GenerateDiscriminatedProject(
      DbProjectExpression e)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(null != this._discriminatedViewTopProject, "if a project matches the pattern, there must be a corresponding discriminator map");
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.EnterExpressionBinding(e.Input);
      List<RelProperty> relProperties = new List<RelProperty>();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> nodeList = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (KeyValuePair<RelProperty, DbExpression> relProperty in this._discriminatorMap.RelPropertyMap)
      {
        relProperties.Add(relProperty.Key);
        nodeList.Add(this.VisitExprAsScalar(relProperty.Value));
      }
      DiscriminatedNewEntityOp discriminatedNewEntityOp = this._iqtCommand.CreateDiscriminatedNewEntityOp(e.Projection.ResultType, new ExplicitDiscriminatorMap(this._discriminatorMap), this._discriminatorMap.EntitySet, relProperties);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>(this._discriminatorMap.PropertyMap.Count + 1);
      args.Add(this.CreateNewInstanceArgument(this._discriminatorMap.Discriminator.Property, (DbExpression) this._discriminatorMap.Discriminator));
      foreach (KeyValuePair<EdmProperty, DbExpression> property in this._discriminatorMap.PropertyMap)
      {
        DbExpression dbExpression = property.Value;
        System.Data.Entity.Core.Query.InternalTrees.Node instanceArgument = this.CreateNewInstanceArgument((EdmMember) property.Key, dbExpression);
        args.Add(instanceArgument);
      }
      args.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) nodeList);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) discriminatedNewEntityOp, args);
      this.ExitExpressionBinding();
      Var computedVar;
      System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = this._iqtCommand.CreateVarDefListNode(node2, out computedVar);
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateProjectOp(computedVar), node1, varDefListNode);
      this._varMap[node3] = computedVar;
      return node3;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CreateNewInstanceArgument(
      EdmMember property,
      DbExpression value)
    {
      return this.BuildSoftCast(this.VisitExprAsScalar(value), Helper.GetModelTypeUsage(property));
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node GenerateStandardProject(
      DbProjectExpression e)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.EnterExpressionBinding(e.Input);
      System.Data.Entity.Core.Query.InternalTrees.Node definingExpr = this.VisitExprAsScalar(e.Projection);
      this.ExitExpressionBinding();
      Var computedVar;
      System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = this._iqtCommand.CreateVarDefListNode(definingExpr, out computedVar);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateProjectOp(computedVar), node1, varDefListNode);
      this._varMap[node2] = computedVar;
      return node2;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbCrossJoinExpression e)
    {
      Check.NotNull<DbCrossJoinExpression>(e, nameof (e));
      return this.VisitJoin((DbExpression) e, e.Inputs, (DbExpression) null);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbJoinExpression e)
    {
      Check.NotNull<DbJoinExpression>(e, nameof (e));
      return this.VisitJoin((DbExpression) e, (IList<DbExpressionBinding>) new List<DbExpressionBinding>()
      {
        e.Left,
        e.Right
      }, e.JoinCondition);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbJoinExpression")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CrossJoinOps")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "JoinType")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "JoinOp")]
    private System.Data.Entity.Core.Query.InternalTrees.Node VisitJoin(
      DbExpression e,
      IList<DbExpressionBinding> inputs,
      DbExpression joinCond)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(DbExpressionKind.CrossJoin == e.ExpressionKind || DbExpressionKind.InnerJoin == e.ExpressionKind || DbExpressionKind.LeftOuterJoin == e.ExpressionKind || DbExpressionKind.FullOuterJoin == e.ExpressionKind, "Unrecognized JoinType specified in DbJoinExpression");
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      List<Var> varList = new List<Var>();
      for (int index = 0; index < inputs.Count; ++index)
      {
        Var boundVar;
        System.Data.Entity.Core.Query.InternalTrees.Node node = this.VisitBoundExpression(inputs[index].Expression, out boundVar);
        args.Add(node);
        varList.Add(boundVar);
      }
      for (int index = 0; index < args.Count; ++index)
        this.PushBindingScope(varList[index], inputs[index].VariableName);
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.VisitExprAsPredicate(joinCond);
      for (int index = 0; index < args.Count; ++index)
        this.ExitExpressionBinding();
      JoinBaseOp joinBaseOp = (JoinBaseOp) null;
      switch (e.ExpressionKind)
      {
        case DbExpressionKind.CrossJoin:
          joinBaseOp = (JoinBaseOp) this._iqtCommand.CreateCrossJoinOp();
          break;
        case DbExpressionKind.FullOuterJoin:
          joinBaseOp = (JoinBaseOp) this._iqtCommand.CreateFullOuterJoinOp();
          break;
        case DbExpressionKind.InnerJoin:
          joinBaseOp = (JoinBaseOp) this._iqtCommand.CreateInnerJoinOp();
          break;
        case DbExpressionKind.LeftOuterJoin:
          joinBaseOp = (JoinBaseOp) this._iqtCommand.CreateLeftOuterJoinOp();
          break;
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(joinBaseOp != null, "Unrecognized JoinOp specified in DbJoinExpression, no JoinOp was produced");
      if (e.ExpressionKind != DbExpressionKind.CrossJoin)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node1 != null, "Non CrossJoinOps must specify a join condition");
        args.Add(node1);
      }
      return this.ProjectNewRecord(this._iqtCommand.CreateNode((Op) joinBaseOp, args), ITreeGenerator.ExtractElementRowType(e.ResultType), (IEnumerable<Var>) varList);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbExpressionKind")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbApplyExpression")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbApplyExpression e)
    {
      Check.NotNull<DbApplyExpression>(e, nameof (e));
      System.Data.Entity.Core.Query.InternalTrees.Node index1 = this.EnterExpressionBinding(e.Input);
      System.Data.Entity.Core.Query.InternalTrees.Node index2 = this.EnterExpressionBinding(e.Apply);
      this.ExitExpressionBinding();
      this.ExitExpressionBinding();
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(DbExpressionKind.CrossApply == e.ExpressionKind || DbExpressionKind.OuterApply == e.ExpressionKind, "Unrecognized DbExpressionKind specified in DbApplyExpression");
      return this.ProjectNewRecord(this._iqtCommand.CreateNode(DbExpressionKind.CrossApply != e.ExpressionKind ? (Op) this._iqtCommand.CreateOuterApplyOp() : (Op) this._iqtCommand.CreateCrossApplyOp(), index1, index2), ITreeGenerator.ExtractElementRowType(e.ResultType), (IEnumerable<Var>) new Var[2]
      {
        this._varMap[index1],
        this._varMap[index2]
      });
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbGroupByExpression")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbAggregate")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbGroupByExpression e)
    {
      Check.NotNull<DbGroupByExpression>(e, nameof (e));
      VarVec varVec1 = this._iqtCommand.CreateVarVec();
      VarVec varVec2 = this._iqtCommand.CreateVarVec();
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode1;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> keyVarDefNodes1;
      ITreeGenerator.ExpressionBindingScope scope1;
      this.ExtractKeys(e, varVec1, varVec2, out inputNode1, out keyVarDefNodes1, out scope1);
      int num = -1;
      for (int index = 0; index < e.Aggregates.Count; ++index)
      {
        if (e.Aggregates[index].GetType() == typeof (DbGroupAggregate))
        {
          num = index;
          break;
        }
      }
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode2 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> keyVarDefNodes2 = (List<System.Data.Entity.Core.Query.InternalTrees.Node>) null;
      VarVec varVec3 = this._iqtCommand.CreateVarVec();
      VarVec varVec4 = this._iqtCommand.CreateVarVec();
      if (num >= 0)
      {
        ITreeGenerator.ExpressionBindingScope scope2;
        this.ExtractKeys(e, varVec4, varVec3, out inputNode2, out keyVarDefNodes2, out scope2);
      }
      this._varScopes.Push((ITreeGenerator.CqtVariableScope) new ITreeGenerator.ExpressionBindingScope(this._iqtCommand, e.Input.GroupVariableName, scope1.ScopeVar));
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args1 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      System.Data.Entity.Core.Query.InternalTrees.Node node = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      for (int index = 0; index < e.Aggregates.Count; ++index)
      {
        DbAggregate aggregate = e.Aggregates[index];
        IList<System.Data.Entity.Core.Query.InternalTrees.Node> argNodes = this.VisitExprAsScalar(aggregate.Arguments);
        Var v;
        if (index != num)
        {
          DbFunctionAggregate funcAgg = aggregate as DbFunctionAggregate;
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(funcAgg != null, "Unrecognized DbAggregate used in DbGroupByExpression");
          args1.Add(this.ProcessFunctionAggregate(funcAgg, argNodes, out v));
        }
        else
          node = this.ProcessGroupAggregate(keyVarDefNodes1, inputNode2, keyVarDefNodes2, varVec4, e.Input.Expression.ResultType, out v);
        varVec2.Set(v);
      }
      this.ExitGroupExpressionBinding();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args2 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      args2.Add(inputNode1);
      args2.Add(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarDefListOp(), keyVarDefNodes1));
      args2.Add(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarDefListOp(), args1));
      GroupByBaseOp groupByBaseOp;
      if (num >= 0)
      {
        args2.Add(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarDefListOp(), node));
        groupByBaseOp = (GroupByBaseOp) this._iqtCommand.CreateGroupByIntoOp(varVec1, this._iqtCommand.CreateVarVec(this._varMap[inputNode1]), varVec2);
      }
      else
        groupByBaseOp = (GroupByBaseOp) this._iqtCommand.CreateGroupByOp(varVec1, varVec2);
      return this.ProjectNewRecord(this._iqtCommand.CreateNode((Op) groupByBaseOp, args2), ITreeGenerator.ExtractElementRowType(e.ResultType), (IEnumerable<Var>) varVec2);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ScalarOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GroupBy")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void ExtractKeys(
      DbGroupByExpression e,
      VarVec keyVarSet,
      VarVec outputVarSet,
      out System.Data.Entity.Core.Query.InternalTrees.Node inputNode,
      out List<System.Data.Entity.Core.Query.InternalTrees.Node> keyVarDefNodes,
      out ITreeGenerator.ExpressionBindingScope scope)
    {
      inputNode = this.EnterGroupExpressionBinding(e.Input);
      keyVarDefNodes = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      for (int index = 0; index < e.Keys.Count; ++index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node definingExpr = this.VisitExprAsScalar(e.Keys[index]);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(definingExpr.Op is ScalarOp, "GroupBy Key is not a ScalarOp");
        Var computedVar;
        keyVarDefNodes.Add(this._iqtCommand.CreateVarDefNode(definingExpr, out computedVar));
        outputVarSet.Set(computedVar);
        keyVarSet.Set(computedVar);
      }
      scope = this.ExitExpressionBinding();
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node ProcessFunctionAggregate(
      DbFunctionAggregate funcAgg,
      IList<System.Data.Entity.Core.Query.InternalTrees.Node> argNodes,
      out Var aggVar)
    {
      return this._iqtCommand.CreateVarDefNode(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateAggregateOp(funcAgg.Function, funcAgg.Distinct), argNodes), out aggVar);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node ProcessGroupAggregate(
      List<System.Data.Entity.Core.Query.InternalTrees.Node> keyVarDefNodes,
      System.Data.Entity.Core.Query.InternalTrees.Node copyOfInput,
      List<System.Data.Entity.Core.Query.InternalTrees.Node> copyOfkeyVarDefNodes,
      VarVec copyKeyVarSet,
      TypeUsage inputResultType,
      out Var groupAggVar)
    {
      Var var1 = this._varMap[copyOfInput];
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = copyOfInput;
      if (keyVarDefNodes.Count > 0)
      {
        VarVec varVec = this._iqtCommand.CreateVarVec();
        varVec.Set(var1);
        varVec.Or(copyKeyVarSet);
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateProjectOp(varVec), node1, this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarDefListOp(), copyOfkeyVarDefNodes));
        List<System.Data.Entity.Core.Query.InternalTrees.Node> nodeList1 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
        List<System.Data.Entity.Core.Query.InternalTrees.Node> nodeList2 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
        for (int index = 0; index < keyVarDefNodes.Count; ++index)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node keyVarDefNode = keyVarDefNodes[index];
          System.Data.Entity.Core.Query.InternalTrees.Node copyOfkeyVarDefNode = copyOfkeyVarDefNodes[index];
          Var var2 = ((VarDefOp) keyVarDefNode.Op).Var;
          Var var3 = ((VarDefOp) copyOfkeyVarDefNode.Op).Var;
          this.FlattenProperties(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarRefOp(var2)), (IList<System.Data.Entity.Core.Query.InternalTrees.Node>) nodeList1);
          this.FlattenProperties(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarRefOp(var3)), (IList<System.Data.Entity.Core.Query.InternalTrees.Node>) nodeList2);
        }
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(nodeList1.Count == nodeList2.Count, "The flattened keys lists should have the same number of elements");
        System.Data.Entity.Core.Query.InternalTrees.Node node3 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
        for (int index = 0; index < nodeList1.Count; ++index)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node n1 = nodeList1[index];
          System.Data.Entity.Core.Query.InternalTrees.Node n2 = nodeList2[index];
          System.Data.Entity.Core.Query.InternalTrees.Node node4 = !this._useDatabaseNullSemantics ? this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateComparisonOp(OpType.EQ, false), n1, n2) : this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.Or), this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateComparisonOp(OpType.EQ, false), n1, n2), this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.And), this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.IsNull), OpCopier.Copy(this._iqtCommand, n1)), this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.IsNull), OpCopier.Copy(this._iqtCommand, n2))));
          node3 = node3 != null ? this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.And), node3, node4) : node4;
        }
        node1 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateFilterOp(), node2, node3);
      }
      this._varMap[node1] = var1;
      return this._iqtCommand.CreateVarDefNode(this.ConvertRelOpToScalarOpTree(node1, inputResultType), out groupAggVar);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RowType")]
    private void FlattenProperties(System.Data.Entity.Core.Query.InternalTrees.Node input, IList<System.Data.Entity.Core.Query.InternalTrees.Node> flattenedProperties)
    {
      if (input.Op.Type.EdmType.BuiltInTypeKind == BuiltInTypeKind.RowType)
      {
        IList<EdmProperty> properties = (IList<EdmProperty>) TypeHelpers.GetProperties(input.Op.Type);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(properties.Count != 0, "No nested properties for RowType");
        for (int index = 0; index < properties.Count; ++index)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node node = index == 0 ? input : OpCopier.Copy(this._iqtCommand, input);
          this.FlattenProperties(this._iqtCommand.CreateNode((Op) this._iqtCommand.CreatePropertyOp((EdmMember) properties[index]), node), flattenedProperties);
        }
      }
      else
        flattenedProperties.Add(input);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbSortClause")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SortKey")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SortClauses")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-ScalarOp")]
    private System.Data.Entity.Core.Query.InternalTrees.Node VisitSortArguments(
      DbExpressionBinding input,
      IList<DbSortClause> sortOrder,
      List<SortKey> sortKeys,
      out Var inputVar)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node index1 = this.EnterExpressionBinding(input);
      inputVar = this._varMap[index1];
      VarVec varVec = this._iqtCommand.CreateVarVec();
      varVec.Set(inputVar);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(sortKeys.Count == 0, "Non-empty SortKey list before adding converted SortClauses");
      for (int index2 = 0; index2 < sortOrder.Count; ++index2)
      {
        DbSortClause dbSortClause = sortOrder[index2];
        System.Data.Entity.Core.Query.InternalTrees.Node definingExpr = this.VisitExprAsScalar(dbSortClause.Expression);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(definingExpr.Op is ScalarOp, "DbSortClause Expression converted to non-ScalarOp");
        Var computedVar;
        args.Add(this._iqtCommand.CreateVarDefNode(definingExpr, out computedVar));
        varVec.Set(computedVar);
        SortKey sortKey = !string.IsNullOrEmpty(dbSortClause.Collation) ? Command.CreateSortKey(computedVar, dbSortClause.Ascending, dbSortClause.Collation) : Command.CreateSortKey(computedVar, dbSortClause.Ascending);
        sortKeys.Add(sortKey);
      }
      this.ExitExpressionBinding();
      return this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateProjectOp(varVec), index1, this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateVarDefListOp(), args));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbSkipExpression expression)
    {
      Check.NotNull<DbSkipExpression>(expression, nameof (expression));
      List<SortKey> sortKeys = new List<SortKey>();
      Var inputVar;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.VisitSortArguments(expression.Input, expression.SortOrder, sortKeys, out inputVar);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.VisitExprAsScalar(expression.Count);
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConstrainedSortOp(sortKeys), node1, node2, this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateNullOp(this._iqtCommand.IntegerType)));
      this._varMap[node3] = inputVar;
      return node3;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(DbSortExpression e)
    {
      Check.NotNull<DbSortExpression>(e, nameof (e));
      List<SortKey> sortKeys = new List<SortKey>();
      Var inputVar;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.VisitSortArguments(e.Input, e.SortOrder, sortKeys, out inputVar);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateSortOp(sortKeys), node1);
      this._varMap[node2] = inputVar;
      return node2;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbExpressionKind")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbQuantifierExpression")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DbQuantifierExpression e)
    {
      Check.NotNull<DbQuantifierExpression>(e, nameof (e));
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(DbExpressionKind.Any == e.ExpressionKind || DbExpressionKind.All == e.ExpressionKind, "Invalid DbExpressionKind in DbQuantifierExpression");
      System.Data.Entity.Core.Query.InternalTrees.Node index = this.EnterExpressionBinding(e.Input);
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.VisitExprAsPredicate(e.Predicate);
      if (e.ExpressionKind == DbExpressionKind.All)
        node1 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.Or), this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.Not), node1), this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.IsNull), this.VisitExprAsScalar(e.Predicate)));
      this.ExitExpressionBinding();
      Var var = this._varMap[index];
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateFilterOp(), index, node1);
      this._varMap[node2] = var;
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateExistsOp(), node2);
      if (e.ExpressionKind == DbExpressionKind.All)
        node3 = this._iqtCommand.CreateNode((Op) this._iqtCommand.CreateConditionalOp(OpType.Not), node3);
      return node3;
    }

    private abstract class CqtVariableScope
    {
      internal abstract bool Contains(string varName);

      internal abstract System.Data.Entity.Core.Query.InternalTrees.Node this[string varName] { get; }

      internal abstract bool IsPredicate(string varName);
    }

    private class ExpressionBindingScope : ITreeGenerator.CqtVariableScope
    {
      private readonly Command _tree;
      private readonly string _varName;
      private readonly Var _var;

      internal ExpressionBindingScope(Command iqtTree, string name, Var iqtVar)
      {
        this._tree = iqtTree;
        this._varName = name;
        this._var = iqtVar;
      }

      internal override bool Contains(string name)
      {
        return this._varName == name;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
      internal override System.Data.Entity.Core.Query.InternalTrees.Node this[string name]
      {
        get
        {
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(name == this._varName, "huh?");
          return this._tree.CreateNode((Op) this._tree.CreateVarRefOp(this._var));
        }
      }

      internal override bool IsPredicate(string varName)
      {
        return false;
      }

      internal Var ScopeVar
      {
        get
        {
          return this._var;
        }
      }
    }

    private sealed class LambdaScope : ITreeGenerator.CqtVariableScope
    {
      private readonly ITreeGenerator _treeGen;
      private readonly Command _command;
      private readonly Dictionary<string, Tuple<System.Data.Entity.Core.Query.InternalTrees.Node, bool>> _arguments;
      private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, bool> _referencedArgs;

      internal LambdaScope(
        ITreeGenerator treeGen,
        Command command,
        Dictionary<string, Tuple<System.Data.Entity.Core.Query.InternalTrees.Node, bool>> args)
      {
        this._treeGen = treeGen;
        this._command = command;
        this._arguments = args;
        this._referencedArgs = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, bool>(this._arguments.Count);
      }

      internal override bool Contains(string name)
      {
        return this._arguments.ContainsKey(name);
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LambdaScope")]
      internal override System.Data.Entity.Core.Query.InternalTrees.Node this[string name]
      {
        get
        {
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this._arguments.ContainsKey(name), "LambdaScope indexer called for invalid Var");
          System.Data.Entity.Core.Query.InternalTrees.Node index = this._arguments[name].Item1;
          if (this._referencedArgs.ContainsKey(index))
          {
            System.Data.Entity.Core.Query.InternalTrees.VarMap varMap = (System.Data.Entity.Core.Query.InternalTrees.VarMap) null;
            System.Data.Entity.Core.Query.InternalTrees.Node node = OpCopier.Copy(this._command, index, out varMap);
            if (varMap.Count > 0)
              this.MapCopiedNodeVars((IList<System.Data.Entity.Core.Query.InternalTrees.Node>) new List<System.Data.Entity.Core.Query.InternalTrees.Node>(1)
              {
                index
              }, (IList<System.Data.Entity.Core.Query.InternalTrees.Node>) new List<System.Data.Entity.Core.Query.InternalTrees.Node>(1)
              {
                node
              }, (Dictionary<Var, Var>) varMap);
            index = node;
          }
          else
            this._referencedArgs[index] = true;
          return index;
        }
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LambdaScope")]
      internal override bool IsPredicate(string name)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this._arguments.ContainsKey(name), "LambdaScope indexer called for invalid Var");
        return this._arguments[name].Item2;
      }

      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OpCopier")]
      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
      private void MapCopiedNodeVars(
        IList<System.Data.Entity.Core.Query.InternalTrees.Node> sources,
        IList<System.Data.Entity.Core.Query.InternalTrees.Node> copies,
        Dictionary<Var, Var> varMappings)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(sources.Count == copies.Count, "Source/Copy Node count mismatch");
        for (int index = 0; index < sources.Count; ++index)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node source = sources[index];
          System.Data.Entity.Core.Query.InternalTrees.Node copy = copies[index];
          if (source.Children.Count > 0)
            this.MapCopiedNodeVars((IList<System.Data.Entity.Core.Query.InternalTrees.Node>) source.Children, (IList<System.Data.Entity.Core.Query.InternalTrees.Node>) copy.Children, varMappings);
          Var key = (Var) null;
          if (this._treeGen.VarMap.TryGetValue(source, out key))
          {
            System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(varMappings.ContainsKey(key), "No mapping found for Var in Var to Var map from OpCopier");
            this._treeGen.VarMap[copy] = varMappings[key];
          }
        }
      }
    }

    private delegate System.Data.Entity.Core.Query.InternalTrees.Node VisitExprDelegate(
      DbExpression e);

    private class IsOfFilter
    {
      private readonly TypeUsage requiredType;
      private readonly bool isExact;
      private ITreeGenerator.IsOfFilter next;

      internal IsOfFilter(DbIsOfExpression template)
      {
        this.requiredType = template.OfType;
        this.isExact = template.ExpressionKind == DbExpressionKind.IsOfOnly;
      }

      internal IsOfFilter(DbOfTypeExpression template)
      {
        this.requiredType = template.OfType;
        this.isExact = template.ExpressionKind == DbExpressionKind.OfTypeOnly;
      }

      private IsOfFilter(TypeUsage required, bool exact)
      {
        this.requiredType = required;
        this.isExact = exact;
      }

      private ITreeGenerator.IsOfFilter Merge(
        TypeUsage otherRequiredType,
        bool otherIsExact)
      {
        bool flag = this.requiredType.EdmEquals((MetadataItem) otherRequiredType);
        ITreeGenerator.IsOfFilter isOfFilter;
        if (flag && this.isExact == otherIsExact)
          isOfFilter = this;
        else if (this.isExact && otherIsExact)
        {
          isOfFilter = new ITreeGenerator.IsOfFilter(otherRequiredType, otherIsExact);
          isOfFilter.next = this;
        }
        else if (!this.isExact && !otherIsExact)
        {
          if (otherRequiredType.IsSubtypeOf(this.requiredType))
          {
            isOfFilter = new ITreeGenerator.IsOfFilter(otherRequiredType, false);
            isOfFilter.next = this.next;
          }
          else if (this.requiredType.IsSubtypeOf(otherRequiredType))
          {
            isOfFilter = this;
          }
          else
          {
            isOfFilter = new ITreeGenerator.IsOfFilter(otherRequiredType, otherIsExact);
            isOfFilter.next = this;
          }
        }
        else if (flag)
        {
          isOfFilter = new ITreeGenerator.IsOfFilter(otherRequiredType, true);
          isOfFilter.next = this.next;
        }
        else
        {
          TypeUsage required = this.isExact ? this.requiredType : otherRequiredType;
          TypeUsage typeUsage = this.isExact ? otherRequiredType : this.requiredType;
          if (required.IsSubtypeOf(typeUsage))
          {
            if (object.ReferenceEquals((object) required, (object) this.requiredType) && this.isExact)
            {
              isOfFilter = this;
            }
            else
            {
              isOfFilter = new ITreeGenerator.IsOfFilter(required, true);
              isOfFilter.next = this.next;
            }
          }
          else
          {
            isOfFilter = new ITreeGenerator.IsOfFilter(otherRequiredType, otherIsExact);
            isOfFilter.next = this;
          }
        }
        return isOfFilter;
      }

      internal ITreeGenerator.IsOfFilter Merge(DbIsOfExpression other)
      {
        return this.Merge(other.OfType, other.ExpressionKind == DbExpressionKind.IsOfOnly);
      }

      internal ITreeGenerator.IsOfFilter Merge(DbOfTypeExpression other)
      {
        return this.Merge(other.OfType, other.ExpressionKind == DbExpressionKind.OfTypeOnly);
      }

      internal IEnumerable<KeyValuePair<TypeUsage, bool>> ToEnumerable()
      {
        for (ITreeGenerator.IsOfFilter currentFilter = this; currentFilter != null; currentFilter = currentFilter.next)
          yield return new KeyValuePair<TypeUsage, bool>(currentFilter.requiredType, currentFilter.isExact);
      }
    }
  }
}
