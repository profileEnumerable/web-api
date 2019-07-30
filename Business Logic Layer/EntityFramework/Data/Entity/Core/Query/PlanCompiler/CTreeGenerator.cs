// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.CTreeGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class CTreeGenerator : BasicOpVisitorOfT<DbExpression>
  {
    private readonly Dictionary<ParameterVar, DbParameterReferenceExpression> _addedParams = new Dictionary<ParameterVar, DbParameterReferenceExpression>();
    private readonly Stack<CTreeGenerator.IqtVarScope> _bindingScopes = new Stack<CTreeGenerator.IqtVarScope>();
    private readonly Stack<CTreeGenerator.VarDefScope> _varScopes = new Stack<CTreeGenerator.VarDefScope>();
    private readonly Dictionary<DbExpression, CTreeGenerator.RelOpInfo> _relOpState = new Dictionary<DbExpression, CTreeGenerator.RelOpInfo>();
    private readonly AliasGenerator _applyAliases = new AliasGenerator("Apply");
    private readonly AliasGenerator _distinctAliases = new AliasGenerator("Distinct");
    private readonly AliasGenerator _exceptAliases = new AliasGenerator("Except");
    private readonly AliasGenerator _extentAliases = new AliasGenerator("Extent");
    private readonly AliasGenerator _filterAliases = new AliasGenerator("Filter");
    private readonly AliasGenerator _groupByAliases = new AliasGenerator("GroupBy");
    private readonly AliasGenerator _intersectAliases = new AliasGenerator("Intersect");
    private readonly AliasGenerator _joinAliases = new AliasGenerator("Join");
    private readonly AliasGenerator _projectAliases = new AliasGenerator("Project");
    private readonly AliasGenerator _sortAliases = new AliasGenerator("Sort");
    private readonly AliasGenerator _unionAllAliases = new AliasGenerator("UnionAll");
    private readonly AliasGenerator _elementAliases = new AliasGenerator("Element");
    private readonly AliasGenerator _singleRowTableAliases = new AliasGenerator("SingleRowTable");
    private readonly AliasGenerator _limitAliases = new AliasGenerator("Limit");
    private readonly AliasGenerator _skipAliases = new AliasGenerator("Skip");
    private readonly Command _iqtCommand;
    private readonly DbQueryCommandTree _queryTree;
    private DbProviderManifest _providerManifest;

    internal static DbCommandTree Generate(Command itree, System.Data.Entity.Core.Query.InternalTrees.Node toConvert)
    {
      return (DbCommandTree) new CTreeGenerator(itree, toConvert)._queryTree;
    }

    private CTreeGenerator(Command itree, System.Data.Entity.Core.Query.InternalTrees.Node toConvert)
    {
      this._iqtCommand = itree;
      DbExpression query = this.VisitNode(toConvert);
      this._queryTree = DbQueryCommandTree.FromValidExpression(itree.MetadataWorkspace, DataSpace.SSpace, query, true);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "relOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void AssertRelOp(DbExpression expr)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this._relOpState.ContainsKey(expr), "not a relOp expression?");
    }

    private CTreeGenerator.RelOpInfo PublishRelOp(
      string name,
      DbExpression expr,
      CTreeGenerator.VarInfoList publishedVars)
    {
      CTreeGenerator.RelOpInfo relOpInfo = new CTreeGenerator.RelOpInfo(name, expr, (IEnumerable<CTreeGenerator.VarInfo>) publishedVars);
      this._relOpState.Add(expr, relOpInfo);
      return relOpInfo;
    }

    private CTreeGenerator.RelOpInfo ConsumeRelOp(DbExpression expr)
    {
      this.AssertRelOp(expr);
      CTreeGenerator.RelOpInfo relOpInfo = this._relOpState[expr];
      this._relOpState.Remove(expr);
      return relOpInfo;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Non-RelOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbExpressionBinding")]
    private CTreeGenerator.RelOpInfo VisitAsRelOp(System.Data.Entity.Core.Query.InternalTrees.Node inputNode)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(inputNode.Op is RelOp, "Non-RelOp used as DbExpressionBinding Input");
      return this.ConsumeRelOp(this.VisitNode(inputNode));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbExpressionBinding")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RelOpInfo")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void PushExpressionBindingScope(CTreeGenerator.RelOpInfo inputState)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(inputState != null && inputState.PublisherName != null && inputState.PublishedVars != null, "Invalid RelOpInfo produced by DbExpressionBinding Input");
      this._bindingScopes.Push((CTreeGenerator.IqtVarScope) inputState);
    }

    private CTreeGenerator.RelOpInfo EnterExpressionBindingScope(
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode,
      bool pushScope)
    {
      CTreeGenerator.RelOpInfo inputState = this.VisitAsRelOp(inputNode);
      if (pushScope)
        this.PushExpressionBindingScope(inputState);
      return inputState;
    }

    private CTreeGenerator.RelOpInfo EnterExpressionBindingScope(System.Data.Entity.Core.Query.InternalTrees.Node inputNode)
    {
      return this.EnterExpressionBindingScope(inputNode, true);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExpressionBindingScope")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExitExpressionBindingScope")]
    private void ExitExpressionBindingScope(CTreeGenerator.RelOpInfo scope, bool wasPushed)
    {
      if (!wasPushed)
        return;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this._bindingScopes.Count > 0, "ExitExpressionBindingScope called on empty ExpressionBindingScope stack");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((CTreeGenerator.RelOpInfo) this._bindingScopes.Pop() == scope, "ExitExpressionBindingScope called on incorrect expression");
    }

    private void ExitExpressionBindingScope(CTreeGenerator.RelOpInfo scope)
    {
      this.ExitExpressionBindingScope(scope, true);
    }

    private CTreeGenerator.GroupByScope EnterGroupByScope(System.Data.Entity.Core.Query.InternalTrees.Node inputNode)
    {
      CTreeGenerator.RelOpInfo relOpInfo = this.VisitAsRelOp(inputNode);
      string publisherName = relOpInfo.PublisherName;
      string groupVarName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}Group", (object) publisherName);
      CTreeGenerator.GroupByScope groupByScope = new CTreeGenerator.GroupByScope(relOpInfo.CreateBinding().Expression.GroupBindAs(publisherName, groupVarName), (IEnumerable<CTreeGenerator.VarInfo>) relOpInfo.PublishedVars);
      this._bindingScopes.Push((CTreeGenerator.IqtVarScope) groupByScope);
      return groupByScope;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExpressionBindingScope")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExitGroupByScope")]
    private void ExitGroupByScope(CTreeGenerator.GroupByScope scope)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this._bindingScopes.Count > 0, "ExitGroupByScope called on empty ExpressionBindingScope stack");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((CTreeGenerator.GroupByScope) this._bindingScopes.Pop() == scope, "ExitGroupByScope called on incorrect expression");
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-VarDefOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDefOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDefListOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void EnterVarDefScope(List<System.Data.Entity.Core.Query.InternalTrees.Node> varDefNodes)
    {
      Dictionary<Var, DbExpression> definedVars = new Dictionary<Var, DbExpression>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node varDefNode in varDefNodes)
      {
        VarDefOp op = varDefNode.Op as VarDefOp;
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op != null, "VarDefListOp contained non-VarDefOp child node");
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.Var is ComputedVar, "VarDefOp defined non-Computed Var");
        definedVars.Add(op.Var, this.VisitNode(varDefNode.Child0));
      }
      this._varScopes.Push(new CTreeGenerator.VarDefScope(definedVars));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EnterVarDefListScope")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-VarDefListOp")]
    private void EnterVarDefListScope(System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(varDefListNode.Op is VarDefListOp, "EnterVarDefListScope called with non-VarDefListOp");
      this.EnterVarDefScope(varDefListNode.Children);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExitVarDefScope")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDefScope")]
    private void ExitVarDefScope()
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this._varScopes.Count > 0, "ExitVarDefScope called on empty VarDefScope stack");
      this._varScopes.Pop();
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Unresolvable")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarType")]
    private DbExpression ResolveVar(Var referencedVar)
    {
      DbExpression resultExpr1 = (DbExpression) null;
      ParameterVar key = referencedVar as ParameterVar;
      if (key != null)
      {
        DbParameterReferenceExpression referenceExpression;
        if (!this._addedParams.TryGetValue(key, out referenceExpression))
        {
          referenceExpression = key.Type.Parameter(key.ParameterName);
          this._addedParams[key] = referenceExpression;
        }
        resultExpr1 = (DbExpression) referenceExpression;
      }
      else
      {
        ComputedVar computedVar = referencedVar as ComputedVar;
        if (computedVar != null && this._varScopes.Count > 0 && !this._varScopes.Peek().TryResolveVar((Var) computedVar, out resultExpr1))
          resultExpr1 = (DbExpression) null;
        if (resultExpr1 == null)
        {
          DbExpression resultExpr2 = (DbExpression) null;
          foreach (CTreeGenerator.IqtVarScope bindingScope in this._bindingScopes)
          {
            if (bindingScope.TryResolveVar(referencedVar, out resultExpr2))
            {
              resultExpr1 = resultExpr2;
              break;
            }
          }
        }
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((resultExpr1 != null ? 1 : 0) != 0, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unresolvable Var used in Command: VarType={0}, Id={1}", (object) Enum.GetName(typeof (VarType), (object) referencedVar.VarType), (object) referencedVar.Id));
      return resultExpr1;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static void AssertBinary(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((2 == n.Children.Count ? 1 : 0) != 0, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Non-Binary {0} encountered", (object) n.Op.GetType().Name));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VisitChild")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private DbExpression VisitChild(System.Data.Entity.Core.Query.InternalTrees.Node n, int index)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Children.Count > index, "VisitChild called with invalid index");
      return this.VisitNode(n.Children[index]);
    }

    private List<DbExpression> VisitChildren(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      List<DbExpression> dbExpressionList = new List<DbExpression>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
        dbExpressionList.Add(this.VisitNode(child));
      return dbExpressionList;
    }

    protected override DbExpression VisitConstantOp(ConstantBaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (DbExpression) op.Type.Constant(op.Value);
    }

    public override DbExpression Visit(ConstantOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public override DbExpression Visit(InternalConstantOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public override DbExpression Visit(NullOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (DbExpression) op.Type.Null();
    }

    public override DbExpression Visit(NullSentinelOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public override DbExpression Visit(ConstantPredicateOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (DbExpression) DbExpressionBuilder.True.Equal(op.IsTrue ? (DbExpression) DbExpressionBuilder.True : (DbExpression) DbExpressionBuilder.False);
    }

    public override DbExpression Visit(FunctionOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (DbExpression) op.Function.Invoke((IEnumerable<DbExpression>) this.VisitChildren(n));
    }

    public override DbExpression Visit(PropertyOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(RelPropertyOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ArithmeticOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OpType")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override DbExpression Visit(ArithmeticOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      DbExpression dbExpression;
      if (OpType.UnaryMinus == op.OpType)
      {
        dbExpression = (DbExpression) this.VisitChild(n, 0).UnaryMinus();
      }
      else
      {
        DbExpression left = this.VisitChild(n, 0);
        DbExpression right = this.VisitChild(n, 1);
        switch (op.OpType)
        {
          case OpType.Plus:
            dbExpression = (DbExpression) left.Plus(right);
            break;
          case OpType.Minus:
            dbExpression = (DbExpression) left.Minus(right);
            break;
          case OpType.Multiply:
            dbExpression = (DbExpression) left.Multiply(right);
            break;
          case OpType.Divide:
            dbExpression = (DbExpression) left.Divide(right);
            break;
          case OpType.Modulo:
            dbExpression = (DbExpression) left.Modulo(right);
            break;
          default:
            dbExpression = (DbExpression) null;
            break;
        }
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((dbExpression != null ? 1 : 0) != 0, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ArithmeticOp OpType not recognized: {0}", (object) Enum.GetName(typeof (OpType), (object) op.OpType)));
      return dbExpression;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CaseOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override DbExpression Visit(CaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      int count = n.Children.Count;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(count > 1, "Invalid CaseOp: At least 2 child Nodes (1 When/Then pair) must be present");
      List<DbExpression> dbExpressionList1 = new List<DbExpression>();
      List<DbExpression> dbExpressionList2 = new List<DbExpression>();
      DbExpression elseExpression;
      if (n.Children.Count % 2 == 0)
      {
        elseExpression = (DbExpression) op.Type.Null();
      }
      else
      {
        --count;
        elseExpression = this.VisitChild(n, n.Children.Count - 1);
      }
      for (int index = 0; index < count; index += 2)
      {
        dbExpressionList1.Add(this.VisitChild(n, index));
        dbExpressionList2.Add(this.VisitChild(n, index + 1));
      }
      return (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) dbExpressionList1, (IEnumerable<DbExpression>) dbExpressionList2, elseExpression);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ComparisonOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OpType")]
    public override DbExpression Visit(ComparisonOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      CTreeGenerator.AssertBinary(n);
      DbExpression left = this.VisitChild(n, 0);
      DbExpression right = this.VisitChild(n, 1);
      DbExpression dbExpression;
      switch (op.OpType)
      {
        case OpType.GT:
          dbExpression = (DbExpression) left.GreaterThan(right);
          break;
        case OpType.GE:
          dbExpression = (DbExpression) left.GreaterThanOrEqual(right);
          break;
        case OpType.LE:
          dbExpression = (DbExpression) left.LessThanOrEqual(right);
          break;
        case OpType.LT:
          dbExpression = (DbExpression) left.LessThan(right);
          break;
        case OpType.EQ:
          dbExpression = (DbExpression) left.Equal(right);
          break;
        case OpType.NE:
          dbExpression = (DbExpression) left.NotEqual(right);
          break;
        default:
          dbExpression = (DbExpression) null;
          break;
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((dbExpression != null ? 1 : 0) != 0, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ComparisonOp OpType not recognized: {0}", (object) Enum.GetName(typeof (OpType), (object) op.OpType)));
      return dbExpression;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ConditionalOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OpType")]
    public override DbExpression Visit(ConditionalOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      DbExpression left = this.VisitChild(n, 0);
      DbExpression dbExpression;
      switch (op.OpType)
      {
        case OpType.And:
          dbExpression = (DbExpression) left.And(this.VisitChild(n, 1));
          break;
        case OpType.Or:
          dbExpression = (DbExpression) left.Or(this.VisitChild(n, 1));
          break;
        case OpType.In:
          int count = n.Children.Count;
          List<DbExpression> dbExpressionList = new List<DbExpression>(count - 1);
          for (int index = 1; index < count; ++index)
            dbExpressionList.Add(this.VisitChild(n, index));
          dbExpression = (DbExpression) DbExpressionBuilder.CreateInExpression(left, (IList<DbExpression>) dbExpressionList);
          break;
        case OpType.Not:
          DbNotExpression dbNotExpression = left as DbNotExpression;
          dbExpression = dbNotExpression == null ? (DbExpression) left.Not() : dbNotExpression.Argument;
          break;
        case OpType.IsNull:
          dbExpression = (DbExpression) left.IsNull();
          break;
        default:
          dbExpression = (DbExpression) null;
          break;
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((dbExpression != null ? 1 : 0) != 0, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ConditionalOp OpType not recognized: {0}", (object) Enum.GetName(typeof (OpType), (object) op.OpType)));
      return dbExpression;
    }

    public override DbExpression Visit(LikeOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (DbExpression) this.VisitChild(n, 0).Like(this.VisitChild(n, 1), this.VisitChild(n, 2));
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GroupByOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AggregateOp")]
    public override DbExpression Visit(AggregateOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "AggregateOp encountered outside of GroupByOp");
      throw new NotSupportedException(Strings.Iqt_CTGen_UnexpectedAggregate);
    }

    public override DbExpression Visit(NavigateOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(NewEntityOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(NewInstanceOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(DiscriminatedNewEntityOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(NewMultisetOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(NewRecordOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(RefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(VarRefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.ResolveVar(op.Var);
    }

    public override DbExpression Visit(TreatOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(CastOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (DbExpression) this.VisitChild(n, 0).CastTo(op.Type);
    }

    public override DbExpression Visit(SoftCastOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitChild(n, 0);
    }

    public override DbExpression Visit(IsOfOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (op.IsOfOnly)
        return (DbExpression) this.VisitChild(n, 0).IsOfOnly(op.IsOfType);
      return (DbExpression) this.VisitChild(n, 0).IsOf(op.IsOfType);
    }

    public override DbExpression Visit(ExistsOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      DbExpression expr = this.VisitNode(n.Child0);
      this.ConsumeRelOp(expr);
      return (DbExpression) expr.IsEmpty().Not();
    }

    public override DbExpression Visit(ElementOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      DbExpression expr = this.VisitNode(n.Child0);
      this.AssertRelOp(expr);
      this.ConsumeRelOp(expr);
      return (DbExpression) DbExpressionBuilder.CreateElementExpressionUnwrapSingleProperty(expr);
    }

    public override DbExpression Visit(GetRefKeyOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(GetEntityRefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(CollectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    private static string GenerateNameForVar(
      Var projectedVar,
      Dictionary<string, AliasGenerator> aliasMap,
      AliasGenerator defaultAliasGenerator,
      Dictionary<string, string> alreadyUsedNames)
    {
      string name;
      AliasGenerator aliasGenerator;
      if (projectedVar.TryGetName(out name))
      {
        if (!aliasMap.TryGetValue(name, out aliasGenerator))
        {
          aliasGenerator = new AliasGenerator(name);
          aliasMap[name] = aliasGenerator;
        }
        else
          name = aliasGenerator.Next();
      }
      else
      {
        aliasGenerator = defaultAliasGenerator;
        name = aliasGenerator.Next();
      }
      while (alreadyUsedNames.ContainsKey(name))
        name = aliasGenerator.Next();
      alreadyUsedNames[name] = name;
      return name;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.Collections.Generic.Dictionary`2<System.String,System.String>.#ctor(System.Collections.Generic.IEqualityComparer`1<System.String>)")]
    [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.Collections.Generic.Dictionary`2<System.String,System.Data.Entity.Core.Common.Utils.AliasGenerator>.#ctor(System.Collections.Generic.IEqualityComparer`1<System.String>)")]
    private DbExpression CreateProject(
      CTreeGenerator.RelOpInfo sourceInfo,
      IEnumerable<Var> outputVars)
    {
      CTreeGenerator.VarInfoList publishedVars = new CTreeGenerator.VarInfoList();
      List<KeyValuePair<string, DbExpression>> keyValuePairList = new List<KeyValuePair<string, DbExpression>>();
      AliasGenerator defaultAliasGenerator = new AliasGenerator("C");
      Dictionary<string, AliasGenerator> aliasMap = new Dictionary<string, AliasGenerator>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      Dictionary<string, string> alreadyUsedNames = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      foreach (Var outputVar in outputVars)
      {
        string nameForVar = CTreeGenerator.GenerateNameForVar(outputVar, aliasMap, defaultAliasGenerator, alreadyUsedNames);
        DbExpression dbExpression = this.ResolveVar(outputVar);
        keyValuePairList.Add(new KeyValuePair<string, DbExpression>(nameForVar, dbExpression));
        CTreeGenerator.VarInfo varInfo = new CTreeGenerator.VarInfo(outputVar);
        varInfo.PrependProperty(nameForVar);
        publishedVars.Add(varInfo);
      }
      DbExpression expr = (DbExpression) sourceInfo.CreateBinding().Project((DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList));
      this.PublishRelOp(this._projectAliases.Next(), expr, publishedVars);
      return expr;
    }

    private static CTreeGenerator.VarInfoList GetTableVars(Table targetTable)
    {
      CTreeGenerator.VarInfoList varInfoList = new CTreeGenerator.VarInfoList();
      if (targetTable.TableMetadata.Flattened)
      {
        for (int index = 0; index < targetTable.Columns.Count; ++index)
        {
          CTreeGenerator.VarInfo varInfo = new CTreeGenerator.VarInfo(targetTable.Columns[index]);
          varInfo.PrependProperty(targetTable.TableMetadata.Columns[index].Name);
          varInfoList.Add(varInfo);
        }
      }
      else
        varInfoList.Add(new CTreeGenerator.VarInfo(targetTable.Columns[0]));
      return varInfoList;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ScanTableOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TableMetadata")]
    public override DbExpression Visit(ScanTableOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.Table.TableMetadata.Extent != null, "Invalid TableMetadata used in ScanTableOp - no Extent specified");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!n.HasChild0, "views are not expected here");
      CTreeGenerator.VarInfoList tableVars = CTreeGenerator.GetTableVars(op.Table);
      DbExpression expr = (DbExpression) op.Table.TableMetadata.Extent.Scan();
      this.PublishRelOp(this._extentAliases.Next(), expr, tableVars);
      return expr;
    }

    public override DbExpression Visit(ScanViewOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDef")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override DbExpression Visit(UnnestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Child0.Op.OpType == OpType.VarDef, "an un-nest's child must be a VarDef");
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = n.Child0.Child0;
      DbExpression expr = child0.Op.Accept<DbExpression>((BasicOpVisitorOfT<DbExpression>) this, child0);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(expr.ResultType.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType, "the input to un-nest must yield a collection after plan compilation");
      CTreeGenerator.VarInfoList tableVars = CTreeGenerator.GetTableVars(op.Table);
      this.PublishRelOp(this._extentAliases.Next(), expr, tableVars);
      return expr;
    }

    private CTreeGenerator.RelOpInfo BuildEmptyProjection(System.Data.Entity.Core.Query.InternalTrees.Node relOpNode)
    {
      if (relOpNode.Op.OpType == OpType.Project)
        relOpNode = relOpNode.Child0;
      CTreeGenerator.RelOpInfo scope = this.EnterExpressionBindingScope(relOpNode);
      DbExpression dbExpression = (DbExpression) DbExpressionBuilder.Constant((object) 1);
      DbExpression expr = (DbExpression) scope.CreateBinding().Project((DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) new List<KeyValuePair<string, DbExpression>>()
      {
        new KeyValuePair<string, DbExpression>("C0", dbExpression)
      }));
      this.PublishRelOp(this._projectAliases.Next(), expr, new CTreeGenerator.VarInfoList());
      this.ExitExpressionBindingScope(scope);
      return this.ConsumeRelOp(expr);
    }

    private CTreeGenerator.RelOpInfo BuildProjection(
      System.Data.Entity.Core.Query.InternalTrees.Node relOpNode,
      IEnumerable<Var> projectionVars)
    {
      DbExpression expr;
      if (relOpNode.Op is ProjectOp)
      {
        expr = this.VisitProject(relOpNode, projectionVars);
      }
      else
      {
        CTreeGenerator.RelOpInfo relOpInfo = this.EnterExpressionBindingScope(relOpNode);
        expr = this.CreateProject(relOpInfo, projectionVars);
        this.ExitExpressionBindingScope(relOpInfo);
      }
      return this.ConsumeRelOp(expr);
    }

    private DbExpression VisitProject(System.Data.Entity.Core.Query.InternalTrees.Node n, IEnumerable<Var> varList)
    {
      CTreeGenerator.RelOpInfo relOpInfo = this.EnterExpressionBindingScope(n.Child0);
      if (n.Children.Count > 1)
        this.EnterVarDefListScope(n.Child1);
      DbExpression project = this.CreateProject(relOpInfo, varList);
      if (n.Children.Count > 1)
        this.ExitVarDefScope();
      this.ExitExpressionBindingScope(relOpInfo);
      return project;
    }

    public override DbExpression Visit(ProjectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitProject(n, (IEnumerable<Var>) op.Outputs);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "FilterOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-ScalarOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override DbExpression Visit(FilterOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      CTreeGenerator.RelOpInfo scope = this.EnterExpressionBindingScope(n.Child0);
      DbExpression predicate = this.VisitNode(n.Child1);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsPrimitiveType(predicate.ResultType, PrimitiveTypeKind.Boolean), "Invalid FilterOp Predicate (non-ScalarOp or non-Boolean result)");
      DbExpression expr = (DbExpression) scope.CreateBinding().Filter(predicate);
      this.ExitExpressionBindingScope(scope);
      this.PublishRelOp(this._filterAliases.Next(), expr, scope.PublishedVars);
      return expr;
    }

    private List<DbSortClause> VisitSortKeys(IList<System.Data.Entity.Core.Query.InternalTrees.SortKey> sortKeys)
    {
      VarVec varVec = this._iqtCommand.CreateVarVec();
      List<DbSortClause> dbSortClauseList = new List<DbSortClause>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.SortKey sortKey in (IEnumerable<System.Data.Entity.Core.Query.InternalTrees.SortKey>) sortKeys)
      {
        if (!varVec.IsSet(sortKey.Var))
        {
          varVec.Set(sortKey.Var);
          DbExpression key = this.ResolveVar(sortKey.Var);
          DbSortClause dbSortClause = string.IsNullOrEmpty(sortKey.Collation) ? (sortKey.AscendingSort ? key.ToSortClause() : key.ToSortClauseDescending()) : (sortKey.AscendingSort ? key.ToSortClause(sortKey.Collation) : key.ToSortClauseDescending(sortKey.Collation));
          dbSortClauseList.Add(dbSortClause);
        }
      }
      return dbSortClauseList;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SortOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override DbExpression Visit(SortOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      CTreeGenerator.RelOpInfo scope = this.EnterExpressionBindingScope(n.Child0);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!n.HasChild1, "SortOp can have only one child");
      DbExpression expr = (DbExpression) scope.CreateBinding().Sort((IEnumerable<DbSortClause>) this.VisitSortKeys((IList<System.Data.Entity.Core.Query.InternalTrees.SortKey>) op.Keys));
      this.ExitExpressionBindingScope(scope);
      this.PublishRelOp(this._sortAliases.Next(), expr, scope.PublishedVars);
      return expr;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static DbExpression CreateLimitExpression(
      DbExpression argument,
      DbExpression limit,
      bool withTies)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!withTies, "Limit with Ties is not currently supported");
      return (DbExpression) argument.Limit(limit);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ConstrainedSortOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SortKeys")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override DbExpression Visit(ConstrainedSortOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      DbExpression expr1 = (DbExpression) null;
      string name = (string) null;
      bool condition = OpType.Null == n.Child1.Op.OpType;
      bool flag = OpType.Null == n.Child2.Op.OpType;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!condition || !flag, "ConstrainedSortOp with no Skip Count and no Limit?");
      CTreeGenerator.RelOpInfo scope;
      if (op.Keys.Count == 0)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(condition, "ConstrainedSortOp without SortKeys cannot have Skip Count");
        DbExpression expr2 = this.VisitNode(n.Child0);
        scope = this.ConsumeRelOp(expr2);
        expr1 = CTreeGenerator.CreateLimitExpression(expr2, this.VisitNode(n.Child2), op.WithTies);
        name = this._limitAliases.Next();
      }
      else
      {
        scope = this.EnterExpressionBindingScope(n.Child0);
        List<DbSortClause> dbSortClauseList = this.VisitSortKeys((IList<System.Data.Entity.Core.Query.InternalTrees.SortKey>) op.Keys);
        this.ExitExpressionBindingScope(scope);
        if (!condition && !flag)
        {
          expr1 = CTreeGenerator.CreateLimitExpression((DbExpression) scope.CreateBinding().Skip((IEnumerable<DbSortClause>) dbSortClauseList, this.VisitChild(n, 1)), this.VisitChild(n, 2), op.WithTies);
          name = this._limitAliases.Next();
        }
        else if (!condition && flag)
        {
          expr1 = (DbExpression) scope.CreateBinding().Skip((IEnumerable<DbSortClause>) dbSortClauseList, this.VisitChild(n, 1));
          name = this._skipAliases.Next();
        }
        else if (condition && !flag)
        {
          expr1 = CTreeGenerator.CreateLimitExpression((DbExpression) scope.CreateBinding().Sort((IEnumerable<DbSortClause>) dbSortClauseList), this.VisitChild(n, 2), op.WithTies);
          name = this._limitAliases.Next();
        }
      }
      this.PublishRelOp(name, expr1, scope.PublishedVars);
      return expr1;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDefListOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Vars")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDefOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Non-ComputedVar")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Non-VarDefOp")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GroupByOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override DbExpression Visit(GroupByOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      CTreeGenerator.VarInfoList publishedVars = new CTreeGenerator.VarInfoList();
      CTreeGenerator.GroupByScope scope = this.EnterGroupByScope(n.Child0);
      this.EnterVarDefListScope(n.Child1);
      AliasGenerator aliasGenerator1 = new AliasGenerator("K");
      List<KeyValuePair<string, DbExpression>> keyValuePairList1 = new List<KeyValuePair<string, DbExpression>>();
      List<Var> varList = new List<Var>((IEnumerable<Var>) op.Outputs);
      foreach (Var key in op.Keys)
      {
        string str = aliasGenerator1.Next();
        keyValuePairList1.Add(new KeyValuePair<string, DbExpression>(str, this.ResolveVar(key)));
        CTreeGenerator.VarInfo varInfo = new CTreeGenerator.VarInfo(key);
        varInfo.PrependProperty(str);
        publishedVars.Add(varInfo);
        varList.Remove(key);
      }
      this.ExitVarDefScope();
      scope.SwitchToGroupReference();
      Dictionary<Var, DbAggregate> dictionary = new Dictionary<Var, DbAggregate>();
      System.Data.Entity.Core.Query.InternalTrees.Node child2 = n.Child2;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(child2.Op is VarDefListOp, "Invalid Aggregates VarDefListOp Node encountered in GroupByOp");
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in child2.Children)
      {
        VarDefOp op1 = child.Op as VarDefOp;
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op1 != null, "Non-VarDefOp Node encountered as child of Aggregates VarDefListOp Node");
        Var var = op1.Var;
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(var is ComputedVar, "Non-ComputedVar encountered in Aggregate VarDefOp");
        System.Data.Entity.Core.Query.InternalTrees.Node child0 = child.Child0;
        DbExpression dbExpression = this.VisitNode(child0.Child0);
        AggregateOp op2 = child0.Op as AggregateOp;
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op2 != null, "Non-Aggregate Node encountered as child of Aggregate VarDefOp Node");
        DbFunctionAggregate functionAggregate = !op2.IsDistinctAggregate ? op2.AggFunc.Aggregate(dbExpression) : op2.AggFunc.AggregateDistinct(dbExpression);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(varList.Contains(var), "Defined aggregate Var not in Output Aggregate Vars list?");
        dictionary.Add(var, (DbAggregate) functionAggregate);
      }
      this.ExitGroupByScope(scope);
      AliasGenerator aliasGenerator2 = new AliasGenerator("A");
      List<KeyValuePair<string, DbAggregate>> keyValuePairList2 = new List<KeyValuePair<string, DbAggregate>>();
      foreach (Var target in varList)
      {
        string str = aliasGenerator2.Next();
        keyValuePairList2.Add(new KeyValuePair<string, DbAggregate>(str, dictionary[target]));
        CTreeGenerator.VarInfo varInfo = new CTreeGenerator.VarInfo(target);
        varInfo.PrependProperty(str);
        publishedVars.Add(varInfo);
      }
      DbExpression expr = (DbExpression) scope.Binding.GroupBy((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList1, (IEnumerable<KeyValuePair<string, DbAggregate>>) keyValuePairList2);
      this.PublishRelOp(this._groupByAliases.Next(), expr, publishedVars);
      return expr;
    }

    public override DbExpression Visit(GroupByIntoOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    private CTreeGenerator.RelOpInfo VisitJoinInput(System.Data.Entity.Core.Query.InternalTrees.Node joinInputNode)
    {
      CTreeGenerator.RelOpInfo relOpInfo;
      if (joinInputNode.Op.OpType == OpType.Filter && joinInputNode.Child0.Op.OpType == OpType.ScanTable)
      {
        ScanTableOp op = (ScanTableOp) joinInputNode.Child0.Op;
        relOpInfo = !op.Table.ReferencedColumns.IsEmpty ? this.BuildProjection(joinInputNode, (IEnumerable<Var>) op.Table.ReferencedColumns) : this.BuildEmptyProjection(joinInputNode);
      }
      else
        relOpInfo = this.EnterExpressionBindingScope(joinInputNode, false);
      return relOpInfo;
    }

    private DbExpression VisitBinaryJoin(System.Data.Entity.Core.Query.InternalTrees.Node joinNode, DbExpressionKind joinKind)
    {
      CTreeGenerator.RelOpInfo relOpInfo1 = this.VisitJoinInput(joinNode.Child0);
      CTreeGenerator.RelOpInfo relOpInfo2 = this.VisitJoinInput(joinNode.Child1);
      bool wasPushed = false;
      DbExpression joinCondition;
      if (joinNode.Children.Count > 2)
      {
        wasPushed = true;
        this.PushExpressionBindingScope(relOpInfo1);
        this.PushExpressionBindingScope(relOpInfo2);
        joinCondition = this.VisitNode(joinNode.Child2);
      }
      else
        joinCondition = (DbExpression) DbExpressionBuilder.True;
      DbExpression expressionByKind = DbExpressionBuilder.CreateJoinExpressionByKind(joinKind, joinCondition, relOpInfo1.CreateBinding(), relOpInfo2.CreateBinding());
      CTreeGenerator.VarInfoList publishedVars = new CTreeGenerator.VarInfoList();
      this.ExitExpressionBindingScope(relOpInfo2, wasPushed);
      relOpInfo2.PublishedVars.PrependProperty(relOpInfo2.PublisherName);
      publishedVars.AddRange((IEnumerable<CTreeGenerator.VarInfo>) relOpInfo2.PublishedVars);
      this.ExitExpressionBindingScope(relOpInfo1, wasPushed);
      relOpInfo1.PublishedVars.PrependProperty(relOpInfo1.PublisherName);
      publishedVars.AddRange((IEnumerable<CTreeGenerator.VarInfo>) relOpInfo1.PublishedVars);
      this.PublishRelOp(this._joinAliases.Next(), expressionByKind, publishedVars);
      return expressionByKind;
    }

    public override DbExpression Visit(CrossJoinOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      List<DbExpressionBinding> expressionBindingList = new List<DbExpressionBinding>();
      CTreeGenerator.VarInfoList publishedVars = new CTreeGenerator.VarInfoList();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        CTreeGenerator.RelOpInfo scope = this.VisitJoinInput(child);
        expressionBindingList.Add(scope.CreateBinding());
        this.ExitExpressionBindingScope(scope, false);
        scope.PublishedVars.PrependProperty(scope.PublisherName);
        publishedVars.AddRange((IEnumerable<CTreeGenerator.VarInfo>) scope.PublishedVars);
      }
      DbExpression expr = (DbExpression) DbExpressionBuilder.CrossJoin((IEnumerable<DbExpressionBinding>) expressionBindingList);
      this.PublishRelOp(this._joinAliases.Next(), expr, publishedVars);
      return expr;
    }

    public override DbExpression Visit(InnerJoinOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitBinaryJoin(n, DbExpressionKind.InnerJoin);
    }

    public override DbExpression Visit(LeftOuterJoinOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitBinaryJoin(n, DbExpressionKind.LeftOuterJoin);
    }

    public override DbExpression Visit(FullOuterJoinOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitBinaryJoin(n, DbExpressionKind.FullOuterJoin);
    }

    private DbExpression VisitApply(System.Data.Entity.Core.Query.InternalTrees.Node applyNode, DbExpressionKind applyKind)
    {
      CTreeGenerator.RelOpInfo scope1 = this.EnterExpressionBindingScope(applyNode.Child0);
      CTreeGenerator.RelOpInfo scope2 = this.EnterExpressionBindingScope(applyNode.Child1, false);
      DbExpression expressionByKind = (DbExpression) DbExpressionBuilder.CreateApplyExpressionByKind(applyKind, scope1.CreateBinding(), scope2.CreateBinding());
      this.ExitExpressionBindingScope(scope2, false);
      this.ExitExpressionBindingScope(scope1);
      scope1.PublishedVars.PrependProperty(scope1.PublisherName);
      scope2.PublishedVars.PrependProperty(scope2.PublisherName);
      CTreeGenerator.VarInfoList publishedVars = new CTreeGenerator.VarInfoList();
      publishedVars.AddRange((IEnumerable<CTreeGenerator.VarInfo>) scope1.PublishedVars);
      publishedVars.AddRange((IEnumerable<CTreeGenerator.VarInfo>) scope2.PublishedVars);
      this.PublishRelOp(this._applyAliases.Next(), expressionByKind, publishedVars);
      return expressionByKind;
    }

    public override DbExpression Visit(CrossApplyOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitApply(n, DbExpressionKind.CrossApply);
    }

    public override DbExpression Visit(OuterApplyOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitApply(n, DbExpressionKind.OuterApply);
    }

    private DbExpression VisitSetOpArgument(
      System.Data.Entity.Core.Query.InternalTrees.Node argNode,
      VarVec outputVars,
      VarMap argVars)
    {
      List<Var> varList = new List<Var>();
      CTreeGenerator.RelOpInfo relOpInfo;
      if (outputVars.IsEmpty)
      {
        relOpInfo = this.BuildEmptyProjection(argNode);
      }
      else
      {
        foreach (Var outputVar in outputVars)
          varList.Add(argVars[outputVar]);
        relOpInfo = this.BuildProjection(argNode, (IEnumerable<Var>) varList);
      }
      return relOpInfo.Publisher;
    }

    private DbProviderManifest ProviderManifest
    {
      get
      {
        return this._providerManifest ?? (this._providerManifest = ((StoreItemCollection) this._iqtCommand.MetadataWorkspace.GetItemCollection(DataSpace.SSpace)).ProviderManifest);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "vars")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private DbExpression VisitSetOp(
      SetOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      AliasGenerator alias,
      Func<DbExpression, DbExpression, DbExpression> setOpExpressionBuilder)
    {
      CTreeGenerator.AssertBinary(n);
      bool flag = (op.OpType == OpType.UnionAll || op.OpType == OpType.Intersect) && this.ProviderManifest.SupportsIntersectAndUnionAllFlattening();
      DbExpression dbExpression1 = !flag || n.Child0.Op.OpType != op.OpType ? this.VisitSetOpArgument(n.Child0, op.Outputs, op.VarMap[0]) : this.VisitSetOp((SetOp) n.Child0.Op, n.Child0, alias, setOpExpressionBuilder);
      DbExpression dbExpression2 = !flag || n.Child1.Op.OpType != op.OpType ? this.VisitSetOpArgument(n.Child1, op.Outputs, op.VarMap[1]) : this.VisitSetOp((SetOp) n.Child1.Op, n.Child1, alias, setOpExpressionBuilder);
      CollectionType edmType = TypeHelpers.GetEdmType<CollectionType>(TypeHelpers.GetCommonTypeUsage(dbExpression1.ResultType, dbExpression2.ResultType));
      IEnumerator<EdmProperty> enumerator = (IEnumerator<EdmProperty>) null;
      RowType type = (RowType) null;
      if (TypeHelpers.TryGetEdmType<RowType>(edmType.TypeUsage, out type))
        enumerator = (IEnumerator<EdmProperty>) type.Properties.GetEnumerator();
      CTreeGenerator.VarInfoList publishedVars = new CTreeGenerator.VarInfoList();
      foreach (Var output in op.Outputs)
      {
        CTreeGenerator.VarInfo varInfo = new CTreeGenerator.VarInfo(output);
        if (type != null)
        {
          if (!enumerator.MoveNext())
            System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "Record columns don't match output vars");
          varInfo.PrependProperty(enumerator.Current.Name);
        }
        publishedVars.Add(varInfo);
      }
      DbExpression expr = setOpExpressionBuilder(dbExpression1, dbExpression2);
      this.PublishRelOp(alias.Next(), expr, publishedVars);
      return expr;
    }

    public override DbExpression Visit(UnionAllOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitSetOp((SetOp) op, n, this._unionAllAliases, new Func<DbExpression, DbExpression, DbExpression>(DbExpressionBuilder.UnionAll));
    }

    public override DbExpression Visit(IntersectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitSetOp((SetOp) op, n, this._intersectAliases, new Func<DbExpression, DbExpression, DbExpression>(DbExpressionBuilder.Intersect));
    }

    public override DbExpression Visit(ExceptOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitSetOp((SetOp) op, n, this._exceptAliases, new Func<DbExpression, DbExpression, DbExpression>(DbExpressionBuilder.Except));
    }

    public override DbExpression Visit(DerefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(DistinctOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      CTreeGenerator.RelOpInfo relOpInfo = this.BuildProjection(n.Child0, (IEnumerable<Var>) op.Keys);
      DbExpression expr = (DbExpression) relOpInfo.Publisher.Distinct();
      this.PublishRelOp(this._distinctAliases.Next(), expr, relOpInfo.PublishedVars);
      return expr;
    }

    public override DbExpression Visit(SingleRowOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      CTreeGenerator.RelOpInfo relOpInfo;
      DbExpression expr;
      if (n.Child0.Op.OpType != OpType.Project)
      {
        ExtendedNodeInfo extendedNodeInfo = this._iqtCommand.GetExtendedNodeInfo(n.Child0);
        relOpInfo = !extendedNodeInfo.Definitions.IsEmpty ? this.BuildProjection(n.Child0, (IEnumerable<Var>) extendedNodeInfo.Definitions) : this.BuildEmptyProjection(n.Child0);
        expr = relOpInfo.Publisher;
      }
      else
      {
        expr = this.VisitNode(n.Child0);
        this.AssertRelOp(expr);
        relOpInfo = this.ConsumeRelOp(expr);
      }
      DbNewInstanceExpression instanceExpression = DbExpressionBuilder.NewCollection((IEnumerable<DbExpression>) new List<DbExpression>()
      {
        (DbExpression) expr.Element()
      });
      this.PublishRelOp(this._elementAliases.Next(), (DbExpression) instanceExpression, relOpInfo.PublishedVars);
      return (DbExpression) instanceExpression;
    }

    public override DbExpression Visit(SingleRowTableOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      DbNewInstanceExpression instanceExpression = DbExpressionBuilder.NewCollection((DbExpression[]) new DbConstantExpression[1]
      {
        DbExpressionBuilder.Constant((object) 1)
      });
      this.PublishRelOp(this._singleRowTableAliases.Next(), (DbExpression) instanceExpression, new CTreeGenerator.VarInfoList());
      return (DbExpression) instanceExpression;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDefOp")]
    public override DbExpression Visit(VarDefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "Unexpected VarDefOp");
      throw new NotSupportedException(Strings.Iqt_CTGen_UnexpectedVarDef);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDefListOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override DbExpression Visit(VarDefListOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "Unexpected VarDefListOp");
      throw new NotSupportedException(Strings.Iqt_CTGen_UnexpectedVarDefList);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "physicalProjectOp")]
    public override DbExpression Visit(PhysicalProjectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Children.Count == 1, "more than one input to physicalProjectOp?");
      VarList varList = new VarList();
      foreach (Var output in (List<Var>) op.Outputs)
      {
        if (!varList.Contains(output))
          varList.Add(output);
      }
      op.Outputs.Clear();
      op.Outputs.AddRange((IEnumerable<Var>) varList);
      return this.BuildProjection(n.Child0, (IEnumerable<Var>) op.Outputs).Publisher;
    }

    public override DbExpression Visit(SingleStreamNestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    public override DbExpression Visit(MultiStreamNestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      throw new NotSupportedException();
    }

    private class VarInfo
    {
      private readonly List<string> _propertyChain = new List<string>();
      private readonly Var _var;

      internal Var Var
      {
        get
        {
          return this._var;
        }
      }

      internal List<string> PropertyPath
      {
        get
        {
          return this._propertyChain;
        }
      }

      internal VarInfo(Var target)
      {
        this._var = target;
      }

      internal void PrependProperty(string propName)
      {
        this._propertyChain.Insert(0, propName);
      }
    }

    private class VarInfoList : List<CTreeGenerator.VarInfo>
    {
      internal VarInfoList()
      {
      }

      internal VarInfoList(IEnumerable<CTreeGenerator.VarInfo> elements)
        : base(elements)
      {
      }

      internal void PrependProperty(string propName)
      {
        foreach (CTreeGenerator.VarInfo varInfo in (List<CTreeGenerator.VarInfo>) this)
          varInfo.PropertyPath.Insert(0, propName);
      }

      internal bool TryGetInfo(Var targetVar, out CTreeGenerator.VarInfo varInfo)
      {
        varInfo = (CTreeGenerator.VarInfo) null;
        foreach (CTreeGenerator.VarInfo varInfo1 in (List<CTreeGenerator.VarInfo>) this)
        {
          if (varInfo1.Var == targetVar)
          {
            varInfo = varInfo1;
            return true;
          }
        }
        return false;
      }
    }

    private abstract class IqtVarScope
    {
      internal abstract bool TryResolveVar(Var targetVar, out DbExpression resultExpr);
    }

    private abstract class BindingScope : CTreeGenerator.IqtVarScope
    {
      private readonly CTreeGenerator.VarInfoList _definedVars;

      internal BindingScope(IEnumerable<CTreeGenerator.VarInfo> boundVars)
      {
        this._definedVars = new CTreeGenerator.VarInfoList(boundVars);
      }

      internal CTreeGenerator.VarInfoList PublishedVars
      {
        get
        {
          return this._definedVars;
        }
      }

      internal override bool TryResolveVar(Var targetVar, out DbExpression resultExpr)
      {
        resultExpr = (DbExpression) null;
        CTreeGenerator.VarInfo varInfo = (CTreeGenerator.VarInfo) null;
        if (!this._definedVars.TryGetInfo(targetVar, out varInfo))
          return false;
        resultExpr = (DbExpression) this.BindingReference;
        foreach (string propertyName in varInfo.PropertyPath)
          resultExpr = (DbExpression) resultExpr.Property(propertyName);
        return true;
      }

      protected abstract DbVariableReferenceExpression BindingReference { get; }
    }

    private class RelOpInfo : CTreeGenerator.BindingScope
    {
      private readonly DbExpressionBinding _binding;

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RelOpInfo")]
      internal RelOpInfo(
        string bindingName,
        DbExpression publisher,
        IEnumerable<CTreeGenerator.VarInfo> publishedVars)
        : base(publishedVars)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(publisher.ResultType), "non-collection type used as RelOpInfo publisher");
        this._binding = publisher.BindAs(bindingName);
      }

      internal string PublisherName
      {
        get
        {
          return this._binding.VariableName;
        }
      }

      internal DbExpression Publisher
      {
        get
        {
          return this._binding.Expression;
        }
      }

      internal DbExpressionBinding CreateBinding()
      {
        return this._binding;
      }

      protected override DbVariableReferenceExpression BindingReference
      {
        get
        {
          return this._binding.Variable;
        }
      }
    }

    private class GroupByScope : CTreeGenerator.BindingScope
    {
      private readonly DbGroupExpressionBinding _binding;
      private bool _referenceGroup;

      internal GroupByScope(
        DbGroupExpressionBinding binding,
        IEnumerable<CTreeGenerator.VarInfo> publishedVars)
        : base(publishedVars)
      {
        this._binding = binding;
      }

      internal DbGroupExpressionBinding Binding
      {
        get
        {
          return this._binding;
        }
      }

      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SwitchToGroupReference")]
      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GroupByScope")]
      internal void SwitchToGroupReference()
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!this._referenceGroup, "SwitchToGroupReference called more than once on the same GroupByScope?");
        this._referenceGroup = true;
      }

      protected override DbVariableReferenceExpression BindingReference
      {
        get
        {
          if (!this._referenceGroup)
            return this._binding.Variable;
          return this._binding.GroupVariable;
        }
      }
    }

    private class VarDefScope : CTreeGenerator.IqtVarScope
    {
      private readonly Dictionary<Var, DbExpression> _definedVars;

      internal VarDefScope(Dictionary<Var, DbExpression> definedVars)
      {
        this._definedVars = definedVars;
      }

      internal override bool TryResolveVar(Var targetVar, out DbExpression resultExpr)
      {
        resultExpr = (DbExpression) null;
        DbExpression dbExpression = (DbExpression) null;
        if (!this._definedVars.TryGetValue(targetVar, out dbExpression))
          return false;
        resultExpr = dbExpression;
        return true;
      }
    }
  }
}
