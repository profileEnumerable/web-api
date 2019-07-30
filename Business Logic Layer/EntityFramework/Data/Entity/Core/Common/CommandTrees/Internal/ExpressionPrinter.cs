// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.Internal.ExpressionPrinter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Common.CommandTrees.Internal
{
  internal class ExpressionPrinter : TreePrinter
  {
    private readonly ExpressionPrinter.PrinterVisitor _visitor = new ExpressionPrinter.PrinterVisitor();

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbDeleteCommandTree")]
    internal string Print(DbDeleteCommandTree tree)
    {
      TreeNode treeNode1 = tree.Target == null ? new TreeNode("Target", new TreeNode[0]) : this._visitor.VisitBinding("Target", tree.Target);
      TreeNode treeNode2 = tree.Predicate == null ? new TreeNode("Predicate", new TreeNode[0]) : this._visitor.VisitExpression("Predicate", tree.Predicate);
      return this.Print(new TreeNode("DbDeleteCommandTree", new TreeNode[3]
      {
        ExpressionPrinter.CreateParametersNode((DbCommandTree) tree),
        treeNode1,
        treeNode2
      }));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ResultType")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbFunctionCommandTree")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EdmFunction")]
    internal string Print(DbFunctionCommandTree tree)
    {
      TreeNode treeNode = new TreeNode("EdmFunction", new TreeNode[0]);
      if (tree.EdmFunction != null)
        treeNode.Children.Add(this._visitor.VisitFunction(tree.EdmFunction, (IList<DbExpression>) null));
      TreeNode node = new TreeNode("ResultType", new TreeNode[0]);
      if (tree.ResultType != null)
        ExpressionPrinter.PrinterVisitor.AppendTypeSpecifier(node, tree.ResultType);
      return this.Print(new TreeNode("DbFunctionCommandTree", new TreeNode[3]
      {
        ExpressionPrinter.CreateParametersNode((DbCommandTree) tree),
        treeNode,
        node
      }));
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbInsertCommandTree")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SetClauses")]
    internal string Print(DbInsertCommandTree tree)
    {
      TreeNode treeNode1 = tree.Target == null ? new TreeNode("Target", new TreeNode[0]) : this._visitor.VisitBinding("Target", tree.Target);
      TreeNode treeNode2 = new TreeNode("SetClauses", new TreeNode[0]);
      foreach (DbModificationClause setClause in (IEnumerable<DbModificationClause>) tree.SetClauses)
      {
        if (setClause != null)
          treeNode2.Children.Add(setClause.Print((DbExpressionVisitor<TreeNode>) this._visitor));
      }
      TreeNode treeNode3;
      if (tree.Returning != null)
        treeNode3 = new TreeNode("Returning", new TreeNode[1]
        {
          this._visitor.VisitExpression(tree.Returning)
        });
      else
        treeNode3 = new TreeNode("Returning", new TreeNode[0]);
      return this.Print(new TreeNode("DbInsertCommandTree", new TreeNode[4]
      {
        ExpressionPrinter.CreateParametersNode((DbCommandTree) tree),
        treeNode1,
        treeNode2,
        treeNode3
      }));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbUpdateCommandTree")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SetClauses")]
    internal string Print(DbUpdateCommandTree tree)
    {
      TreeNode treeNode1 = tree.Target == null ? new TreeNode("Target", new TreeNode[0]) : this._visitor.VisitBinding("Target", tree.Target);
      TreeNode treeNode2 = new TreeNode("SetClauses", new TreeNode[0]);
      foreach (DbModificationClause setClause in (IEnumerable<DbModificationClause>) tree.SetClauses)
      {
        if (setClause != null)
          treeNode2.Children.Add(setClause.Print((DbExpressionVisitor<TreeNode>) this._visitor));
      }
      TreeNode treeNode3;
      if (tree.Predicate != null)
        treeNode3 = new TreeNode("Predicate", new TreeNode[1]
        {
          this._visitor.VisitExpression(tree.Predicate)
        });
      else
        treeNode3 = new TreeNode("Predicate", new TreeNode[0]);
      TreeNode treeNode4;
      if (tree.Returning != null)
        treeNode4 = new TreeNode("Returning", new TreeNode[1]
        {
          this._visitor.VisitExpression(tree.Returning)
        });
      else
        treeNode4 = new TreeNode("Returning", new TreeNode[0]);
      return this.Print(new TreeNode("DbUpdateCommandTree", new TreeNode[5]
      {
        ExpressionPrinter.CreateParametersNode((DbCommandTree) tree),
        treeNode1,
        treeNode2,
        treeNode3,
        treeNode4
      }));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbQueryCommandTree")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
    internal string Print(DbQueryCommandTree tree)
    {
      TreeNode node = new TreeNode("Query", new TreeNode[0]);
      if (tree.Query != null)
      {
        ExpressionPrinter.PrinterVisitor.AppendTypeSpecifier(node, tree.Query.ResultType);
        node.Children.Add(this._visitor.VisitExpression(tree.Query));
      }
      return this.Print(new TreeNode("DbQueryCommandTree", new TreeNode[2]
      {
        ExpressionPrinter.CreateParametersNode((DbCommandTree) tree),
        node
      }));
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
    private static TreeNode CreateParametersNode(DbCommandTree tree)
    {
      TreeNode treeNode = new TreeNode("Parameters", new TreeNode[0]);
      foreach (KeyValuePair<string, TypeUsage> parameter in tree.Parameters)
      {
        TreeNode node = new TreeNode(parameter.Key, new TreeNode[0]);
        ExpressionPrinter.PrinterVisitor.AppendTypeSpecifier(node, parameter.Value);
        treeNode.Children.Add(node);
      }
      return treeNode;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private class PrinterVisitor : DbExpressionVisitor<TreeNode>
    {
      private static readonly Dictionary<DbExpressionKind, string> _opMap = ExpressionPrinter.PrinterVisitor.InitializeOpMap();
      private int _maxStringLength = 80;
      private bool _infix = true;

      private static Dictionary<DbExpressionKind, string> InitializeOpMap()
      {
        return new Dictionary<DbExpressionKind, string>(12)
        {
          [DbExpressionKind.Divide] = "/",
          [DbExpressionKind.Modulo] = "%",
          [DbExpressionKind.Multiply] = "*",
          [DbExpressionKind.Plus] = "+",
          [DbExpressionKind.Minus] = "-",
          [DbExpressionKind.UnaryMinus] = "-",
          [DbExpressionKind.Equals] = "=",
          [DbExpressionKind.LessThan] = "<",
          [DbExpressionKind.LessThanOrEquals] = "<=",
          [DbExpressionKind.GreaterThan] = ">",
          [DbExpressionKind.GreaterThanOrEquals] = ">=",
          [DbExpressionKind.NotEquals] = "<>"
        };
      }

      internal TreeNode VisitExpression(DbExpression expr)
      {
        return expr.Accept<TreeNode>((DbExpressionVisitor<TreeNode>) this);
      }

      internal TreeNode VisitExpression(string name, DbExpression expr)
      {
        return new TreeNode(name, new TreeNode[1]
        {
          expr.Accept<TreeNode>((DbExpressionVisitor<TreeNode>) this)
        });
      }

      internal TreeNode VisitBinding(string propName, DbExpressionBinding binding)
      {
        return this.VisitWithLabel(propName, binding.VariableName, binding.Expression);
      }

      internal TreeNode VisitFunction(EdmFunction func, IList<DbExpression> args)
      {
        TreeNode node = new TreeNode();
        ExpressionPrinter.PrinterVisitor.AppendFullName(node.Text, (EdmType) func);
        ExpressionPrinter.PrinterVisitor.AppendParameters(node, func.Parameters.Select<FunctionParameter, KeyValuePair<string, TypeUsage>>((Func<FunctionParameter, KeyValuePair<string, TypeUsage>>) (fp => new KeyValuePair<string, TypeUsage>(fp.Name, fp.TypeUsage))));
        if (args != null)
          this.AppendArguments(node, (IList<string>) func.Parameters.Select<FunctionParameter, string>((Func<FunctionParameter, string>) (fp => fp.Name)).ToArray<string>(), args);
        return node;
      }

      private static TreeNode NodeFromExpression(DbExpression expr)
      {
        return new TreeNode(Enum.GetName(typeof (DbExpressionKind), (object) expr.ExpressionKind), new TreeNode[0]);
      }

      private static void AppendParameters(
        TreeNode node,
        IEnumerable<KeyValuePair<string, TypeUsage>> paramInfos)
      {
        node.Text.Append("(");
        int num = 0;
        foreach (KeyValuePair<string, TypeUsage> paramInfo in paramInfos)
        {
          if (num > 0)
            node.Text.Append(", ");
          ExpressionPrinter.PrinterVisitor.AppendType(node, paramInfo.Value);
          node.Text.Append(" ");
          node.Text.Append(paramInfo.Key);
          ++num;
        }
        node.Text.Append(")");
      }

      internal static void AppendTypeSpecifier(TreeNode node, TypeUsage type)
      {
        node.Text.Append(" : ");
        ExpressionPrinter.PrinterVisitor.AppendType(node, type);
      }

      internal static void AppendType(TreeNode node, TypeUsage type)
      {
        ExpressionPrinter.PrinterVisitor.BuildTypeName(node.Text, type);
      }

      private static void BuildTypeName(StringBuilder text, TypeUsage type)
      {
        RowType edmType1 = type.EdmType as RowType;
        CollectionType edmType2 = type.EdmType as CollectionType;
        RefType edmType3 = type.EdmType as RefType;
        if (TypeSemantics.IsPrimitiveType(type))
          text.Append((object) type);
        else if (edmType2 != null)
        {
          text.Append("Collection{");
          ExpressionPrinter.PrinterVisitor.BuildTypeName(text, edmType2.TypeUsage);
          text.Append("}");
        }
        else if (edmType3 != null)
        {
          text.Append("Ref<");
          ExpressionPrinter.PrinterVisitor.AppendFullName(text, (EdmType) edmType3.ElementType);
          text.Append(">");
        }
        else if (edmType1 != null)
        {
          text.Append("Record[");
          int num = 0;
          foreach (EdmProperty property in edmType1.Properties)
          {
            text.Append("'");
            text.Append(property.Name);
            text.Append("'");
            text.Append("=");
            ExpressionPrinter.PrinterVisitor.BuildTypeName(text, property.TypeUsage);
            ++num;
            if (num < edmType1.Properties.Count)
              text.Append(", ");
          }
          text.Append("]");
        }
        else
        {
          if (!string.IsNullOrEmpty(type.EdmType.NamespaceName))
          {
            text.Append(type.EdmType.NamespaceName);
            text.Append(".");
          }
          text.Append(type.EdmType.Name);
        }
      }

      private static void AppendFullName(StringBuilder text, EdmType type)
      {
        if (BuiltInTypeKind.RowType != type.BuiltInTypeKind && !string.IsNullOrEmpty(type.NamespaceName))
        {
          text.Append(type.NamespaceName);
          text.Append(".");
        }
        text.Append(type.Name);
      }

      private List<TreeNode> VisitParams(
        IList<string> paramInfo,
        IList<DbExpression> args)
      {
        List<TreeNode> treeNodeList = new List<TreeNode>();
        for (int index = 0; index < paramInfo.Count; ++index)
        {
          TreeNode treeNode = new TreeNode(paramInfo[index], new TreeNode[0]);
          treeNode.Children.Add(this.VisitExpression(args[index]));
          treeNodeList.Add(treeNode);
        }
        return treeNodeList;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Collections.Generic.List<System.Data.Entity.Core.Common.Utils.TreeNode>)")]
      private void AppendArguments(
        TreeNode node,
        IList<string> paramNames,
        IList<DbExpression> args)
      {
        if (paramNames.Count <= 0)
          return;
        node.Children.Add(new TreeNode("Arguments", this.VisitParams(paramNames, args)));
      }

      private TreeNode VisitWithLabel(string label, string name, DbExpression def)
      {
        TreeNode treeNode = new TreeNode(label, new TreeNode[0]);
        treeNode.Text.Append(" : '");
        treeNode.Text.Append(name);
        treeNode.Text.Append("'");
        treeNode.Children.Add(this.VisitExpression(def));
        return treeNode;
      }

      private TreeNode VisitBindingList(
        string propName,
        IList<DbExpressionBinding> bindings)
      {
        List<TreeNode> children = new List<TreeNode>();
        for (int index = 0; index < bindings.Count; ++index)
          children.Add(this.VisitBinding(StringUtil.FormatIndex(propName, index), bindings[index]));
        return new TreeNode(propName, children);
      }

      private TreeNode VisitGroupBinding(DbGroupExpressionBinding groupBinding)
      {
        TreeNode treeNode1 = this.VisitExpression(groupBinding.Expression);
        TreeNode treeNode2 = new TreeNode();
        treeNode2.Children.Add(treeNode1);
        treeNode2.Text.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Input : '{0}', '{1}'", (object) groupBinding.VariableName, (object) groupBinding.GroupVariableName);
        return treeNode2;
      }

      private TreeNode Visit(string name, params DbExpression[] exprs)
      {
        TreeNode treeNode = new TreeNode(name, new TreeNode[0]);
        foreach (DbExpression expr in exprs)
          treeNode.Children.Add(this.VisitExpression(expr));
        return treeNode;
      }

      private TreeNode VisitInfix(DbExpression left, string name, DbExpression right)
      {
        if (this._infix)
        {
          TreeNode treeNode = new TreeNode("", new TreeNode[0]);
          treeNode.Children.Add(this.VisitExpression(left));
          treeNode.Children.Add(new TreeNode(name, new TreeNode[0]));
          treeNode.Children.Add(this.VisitExpression(right));
          return treeNode;
        }
        return this.Visit(name, left, right);
      }

      private TreeNode VisitUnary(DbUnaryExpression expr)
      {
        return this.VisitUnary(expr, false);
      }

      private TreeNode VisitUnary(DbUnaryExpression expr, bool appendType)
      {
        TreeNode node = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) expr);
        if (appendType)
          ExpressionPrinter.PrinterVisitor.AppendTypeSpecifier(node, expr.ResultType);
        node.Children.Add(this.VisitExpression(expr.Argument));
        return node;
      }

      private TreeNode VisitBinary(DbBinaryExpression expr)
      {
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) expr);
        treeNode.Children.Add(this.VisitExpression(expr.Left));
        treeNode.Children.Add(this.VisitExpression(expr.Right));
        return treeNode;
      }

      public override TreeNode Visit(DbExpression e)
      {
        Check.NotNull<DbExpression>(e, nameof (e));
        throw new NotSupportedException(Strings.Cqt_General_UnsupportedExpression((object) e.GetType().FullName));
      }

      public override TreeNode Visit(DbConstantExpression e)
      {
        Check.NotNull<DbConstantExpression>(e, nameof (e));
        TreeNode treeNode = new TreeNode();
        string str1 = e.Value as string;
        if (str1 != null)
        {
          string str2 = str1.Replace("\r\n", "\\r\\n");
          int count = str2.Length;
          if (this._maxStringLength > 0)
            count = Math.Min(str2.Length, this._maxStringLength);
          treeNode.Text.Append("'");
          treeNode.Text.Append(str2, 0, count);
          if (str2.Length > count)
            treeNode.Text.Append("...");
          treeNode.Text.Append("'");
        }
        else
          treeNode.Text.Append(e.Value);
        return treeNode;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      public override TreeNode Visit(DbNullExpression e)
      {
        Check.NotNull<DbNullExpression>(e, nameof (e));
        return new TreeNode("null", new TreeNode[0]);
      }

      public override TreeNode Visit(DbVariableReferenceExpression e)
      {
        Check.NotNull<DbVariableReferenceExpression>(e, nameof (e));
        TreeNode treeNode = new TreeNode();
        treeNode.Text.AppendFormat("Var({0})", (object) e.VariableName);
        return treeNode;
      }

      public override TreeNode Visit(DbParameterReferenceExpression e)
      {
        Check.NotNull<DbParameterReferenceExpression>(e, nameof (e));
        TreeNode treeNode = new TreeNode();
        treeNode.Text.AppendFormat("@{0}", (object) e.ParameterName);
        return treeNode;
      }

      public override TreeNode Visit(DbFunctionExpression e)
      {
        Check.NotNull<DbFunctionExpression>(e, nameof (e));
        return this.VisitFunction(e.Function, e.Arguments);
      }

      public override TreeNode Visit(DbLambdaExpression expression)
      {
        Check.NotNull<DbLambdaExpression>(expression, nameof (expression));
        TreeNode node = new TreeNode();
        node.Text.Append("Lambda");
        ExpressionPrinter.PrinterVisitor.AppendParameters(node, expression.Lambda.Variables.Select<DbVariableReferenceExpression, KeyValuePair<string, TypeUsage>>((Func<DbVariableReferenceExpression, KeyValuePair<string, TypeUsage>>) (v => new KeyValuePair<string, TypeUsage>(v.VariableName, v.ResultType))));
        this.AppendArguments(node, (IList<string>) expression.Lambda.Variables.Select<DbVariableReferenceExpression, string>((Func<DbVariableReferenceExpression, string>) (v => v.VariableName)).ToArray<string>(), expression.Arguments);
        node.Children.Add(this.Visit("Body", expression.Lambda.Body));
        return node;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      public override TreeNode Visit(DbPropertyExpression e)
      {
        Check.NotNull<DbPropertyExpression>(e, nameof (e));
        TreeNode treeNode1 = (TreeNode) null;
        if (e.Instance != null)
        {
          treeNode1 = this.VisitExpression(e.Instance);
          if (e.Instance.ExpressionKind == DbExpressionKind.VariableReference || e.Instance.ExpressionKind == DbExpressionKind.Property && treeNode1.Children.Count == 0)
          {
            treeNode1.Text.Append(".");
            treeNode1.Text.Append(e.Property.Name);
            return treeNode1;
          }
        }
        TreeNode treeNode2 = new TreeNode(".", new TreeNode[0]);
        EdmProperty property = e.Property as EdmProperty;
        if (property != null && !(property.DeclaringType is RowType))
        {
          ExpressionPrinter.PrinterVisitor.AppendFullName(treeNode2.Text, (EdmType) property.DeclaringType);
          treeNode2.Text.Append(".");
        }
        treeNode2.Text.Append(e.Property.Name);
        if (treeNode1 != null)
          treeNode2.Children.Add(new TreeNode("Instance", new TreeNode[1]
          {
            treeNode1
          }));
        return treeNode2;
      }

      public override TreeNode Visit(DbComparisonExpression e)
      {
        Check.NotNull<DbComparisonExpression>(e, nameof (e));
        return this.VisitInfix(e.Left, ExpressionPrinter.PrinterVisitor._opMap[e.ExpressionKind], e.Right);
      }

      public override TreeNode Visit(DbLikeExpression e)
      {
        Check.NotNull<DbLikeExpression>(e, nameof (e));
        return this.Visit("Like", e.Argument, e.Pattern, e.Escape);
      }

      public override TreeNode Visit(DbLimitExpression e)
      {
        Check.NotNull<DbLimitExpression>(e, nameof (e));
        return this.Visit(e.WithTies ? "LimitWithTies" : "Limit", e.Argument, e.Limit);
      }

      public override TreeNode Visit(DbIsNullExpression e)
      {
        Check.NotNull<DbIsNullExpression>(e, nameof (e));
        return this.VisitUnary((DbUnaryExpression) e);
      }

      public override TreeNode Visit(DbArithmeticExpression e)
      {
        Check.NotNull<DbArithmeticExpression>(e, nameof (e));
        if (DbExpressionKind.UnaryMinus != e.ExpressionKind)
          return this.VisitInfix(e.Arguments[0], ExpressionPrinter.PrinterVisitor._opMap[e.ExpressionKind], e.Arguments[1]);
        return this.Visit(ExpressionPrinter.PrinterVisitor._opMap[e.ExpressionKind], e.Arguments[0]);
      }

      public override TreeNode Visit(DbAndExpression e)
      {
        Check.NotNull<DbAndExpression>(e, nameof (e));
        return this.VisitInfix(e.Left, "And", e.Right);
      }

      public override TreeNode Visit(DbOrExpression e)
      {
        Check.NotNull<DbOrExpression>(e, nameof (e));
        return this.VisitInfix(e.Left, "Or", e.Right);
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      public override TreeNode Visit(DbInExpression e)
      {
        Check.NotNull<DbInExpression>(e, nameof (e));
        TreeNode treeNode;
        if (this._infix)
        {
          treeNode = new TreeNode(string.Empty, new TreeNode[0]);
          treeNode.Children.Add(this.VisitExpression(e.Item));
          treeNode.Children.Add(new TreeNode("In", new TreeNode[0]));
        }
        else
        {
          treeNode = new TreeNode("In", new TreeNode[0]);
          treeNode.Children.Add(this.VisitExpression(e.Item));
        }
        foreach (DbExpression expr in (IEnumerable<DbExpression>) e.List)
          treeNode.Children.Add(this.VisitExpression(expr));
        return treeNode;
      }

      public override TreeNode Visit(DbNotExpression e)
      {
        Check.NotNull<DbNotExpression>(e, nameof (e));
        return this.VisitUnary((DbUnaryExpression) e);
      }

      public override TreeNode Visit(DbDistinctExpression e)
      {
        Check.NotNull<DbDistinctExpression>(e, nameof (e));
        return this.VisitUnary((DbUnaryExpression) e);
      }

      public override TreeNode Visit(DbElementExpression e)
      {
        Check.NotNull<DbElementExpression>(e, nameof (e));
        return this.VisitUnary((DbUnaryExpression) e, true);
      }

      public override TreeNode Visit(DbIsEmptyExpression e)
      {
        Check.NotNull<DbIsEmptyExpression>(e, nameof (e));
        return this.VisitUnary((DbUnaryExpression) e);
      }

      public override TreeNode Visit(DbUnionAllExpression e)
      {
        Check.NotNull<DbUnionAllExpression>(e, nameof (e));
        return this.VisitBinary((DbBinaryExpression) e);
      }

      public override TreeNode Visit(DbIntersectExpression e)
      {
        Check.NotNull<DbIntersectExpression>(e, nameof (e));
        return this.VisitBinary((DbBinaryExpression) e);
      }

      public override TreeNode Visit(DbExceptExpression e)
      {
        Check.NotNull<DbExceptExpression>(e, nameof (e));
        return this.VisitBinary((DbBinaryExpression) e);
      }

      private TreeNode VisitCastOrTreat(string op, DbUnaryExpression e)
      {
        TreeNode node1 = this.VisitExpression(e.Argument);
        TreeNode node2;
        if (node1.Children.Count == 0)
        {
          node1.Text.Insert(0, op);
          node1.Text.Insert(op.Length, '(');
          node1.Text.Append(" As ");
          ExpressionPrinter.PrinterVisitor.AppendType(node1, e.ResultType);
          node1.Text.Append(")");
          node2 = node1;
        }
        else
        {
          node2 = new TreeNode(op, new TreeNode[0]);
          ExpressionPrinter.PrinterVisitor.AppendTypeSpecifier(node2, e.ResultType);
          node2.Children.Add(node1);
        }
        return node2;
      }

      public override TreeNode Visit(DbTreatExpression e)
      {
        Check.NotNull<DbTreatExpression>(e, nameof (e));
        return this.VisitCastOrTreat("Treat", (DbUnaryExpression) e);
      }

      public override TreeNode Visit(DbCastExpression e)
      {
        Check.NotNull<DbCastExpression>(e, nameof (e));
        return this.VisitCastOrTreat("Cast", (DbUnaryExpression) e);
      }

      public override TreeNode Visit(DbIsOfExpression e)
      {
        Check.NotNull<DbIsOfExpression>(e, nameof (e));
        TreeNode node = new TreeNode();
        if (DbExpressionKind.IsOfOnly == e.ExpressionKind)
          node.Text.Append("IsOfOnly");
        else
          node.Text.Append("IsOf");
        ExpressionPrinter.PrinterVisitor.AppendTypeSpecifier(node, e.OfType);
        node.Children.Add(this.VisitExpression(e.Argument));
        return node;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OfTypeOnly")]
      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OfType")]
      public override TreeNode Visit(DbOfTypeExpression e)
      {
        Check.NotNull<DbOfTypeExpression>(e, nameof (e));
        TreeNode node = new TreeNode(e.ExpressionKind == DbExpressionKind.OfTypeOnly ? "OfTypeOnly" : "OfType", new TreeNode[0]);
        ExpressionPrinter.PrinterVisitor.AppendTypeSpecifier(node, e.OfType);
        node.Children.Add(this.VisitExpression(e.Argument));
        return node;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      public override TreeNode Visit(DbCaseExpression e)
      {
        Check.NotNull<DbCaseExpression>(e, nameof (e));
        TreeNode treeNode = new TreeNode("Case", new TreeNode[0]);
        for (int index = 0; index < e.When.Count; ++index)
        {
          treeNode.Children.Add(this.Visit("When", e.When[index]));
          treeNode.Children.Add(this.Visit("Then", e.Then[index]));
        }
        treeNode.Children.Add(this.Visit("Else", e.Else));
        return treeNode;
      }

      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RelatedEntityReferences")]
      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      public override TreeNode Visit(DbNewInstanceExpression e)
      {
        Check.NotNull<DbNewInstanceExpression>(e, nameof (e));
        TreeNode node = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        ExpressionPrinter.PrinterVisitor.AppendTypeSpecifier(node, e.ResultType);
        if (BuiltInTypeKind.CollectionType == e.ResultType.EdmType.BuiltInTypeKind)
        {
          foreach (DbExpression expr in (IEnumerable<DbExpression>) e.Arguments)
            node.Children.Add(this.VisitExpression(expr));
        }
        else
        {
          string label = BuiltInTypeKind.RowType == e.ResultType.EdmType.BuiltInTypeKind ? "Column" : "Property";
          IList<EdmProperty> properties = (IList<EdmProperty>) TypeHelpers.GetProperties(e.ResultType);
          for (int index = 0; index < properties.Count; ++index)
            node.Children.Add(this.VisitWithLabel(label, properties[index].Name, e.Arguments[index]));
          if (BuiltInTypeKind.EntityType == e.ResultType.EdmType.BuiltInTypeKind && e.HasRelatedEntityReferences)
          {
            TreeNode treeNode = new TreeNode("RelatedEntityReferences", new TreeNode[0]);
            foreach (DbRelatedEntityRef relatedEntityReference in e.RelatedEntityReferences)
            {
              TreeNode navigationNode = ExpressionPrinter.PrinterVisitor.CreateNavigationNode(relatedEntityReference.SourceEnd, relatedEntityReference.TargetEnd);
              navigationNode.Children.Add(ExpressionPrinter.PrinterVisitor.CreateRelationshipNode((RelationshipType) relatedEntityReference.SourceEnd.DeclaringType));
              navigationNode.Children.Add(this.VisitExpression(relatedEntityReference.TargetEntityReference));
              treeNode.Children.Add(navigationNode);
            }
            node.Children.Add(treeNode);
          }
        }
        return node;
      }

      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EntitySet")]
      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      public override TreeNode Visit(DbRefExpression e)
      {
        Check.NotNull<DbRefExpression>(e, nameof (e));
        TreeNode treeNode1 = new TreeNode("Ref", new TreeNode[0]);
        treeNode1.Text.Append("<");
        ExpressionPrinter.PrinterVisitor.AppendFullName(treeNode1.Text, (EdmType) TypeHelpers.GetEdmType<RefType>(e.ResultType).ElementType);
        treeNode1.Text.Append(">");
        TreeNode treeNode2 = new TreeNode("EntitySet : ", new TreeNode[0]);
        treeNode2.Text.Append(e.EntitySet.EntityContainer.Name);
        treeNode2.Text.Append(".");
        treeNode2.Text.Append(e.EntitySet.Name);
        treeNode1.Children.Add(treeNode2);
        treeNode1.Children.Add(this.Visit("Keys", e.Argument));
        return treeNode1;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      private static TreeNode CreateRelationshipNode(RelationshipType relType)
      {
        TreeNode treeNode = new TreeNode("Relationship", new TreeNode[0]);
        treeNode.Text.Append(" : ");
        ExpressionPrinter.PrinterVisitor.AppendFullName(treeNode.Text, (EdmType) relType);
        return treeNode;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      private static TreeNode CreateNavigationNode(
        RelationshipEndMember fromEnd,
        RelationshipEndMember toEnd)
      {
        TreeNode treeNode = new TreeNode();
        treeNode.Text.Append("Navigation : ");
        treeNode.Text.Append(fromEnd.Name);
        treeNode.Text.Append(" -> ");
        treeNode.Text.Append(toEnd.Name);
        return treeNode;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      public override TreeNode Visit(DbRelationshipNavigationExpression e)
      {
        Check.NotNull<DbRelationshipNavigationExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(ExpressionPrinter.PrinterVisitor.CreateRelationshipNode(e.Relationship));
        treeNode.Children.Add(ExpressionPrinter.PrinterVisitor.CreateNavigationNode(e.NavigateFrom, e.NavigateTo));
        treeNode.Children.Add(this.Visit("Source", e.NavigationSource));
        return treeNode;
      }

      public override TreeNode Visit(DbDerefExpression e)
      {
        Check.NotNull<DbDerefExpression>(e, nameof (e));
        return this.VisitUnary((DbUnaryExpression) e);
      }

      public override TreeNode Visit(DbRefKeyExpression e)
      {
        Check.NotNull<DbRefKeyExpression>(e, nameof (e));
        return this.VisitUnary((DbUnaryExpression) e, true);
      }

      public override TreeNode Visit(DbEntityRefExpression e)
      {
        Check.NotNull<DbEntityRefExpression>(e, nameof (e));
        return this.VisitUnary((DbUnaryExpression) e, true);
      }

      public override TreeNode Visit(DbScanExpression e)
      {
        Check.NotNull<DbScanExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Text.Append(" : ");
        treeNode.Text.Append(e.Target.EntityContainer.Name);
        treeNode.Text.Append(".");
        treeNode.Text.Append(e.Target.Name);
        return treeNode;
      }

      public override TreeNode Visit(DbFilterExpression e)
      {
        Check.NotNull<DbFilterExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(this.VisitBinding("Input", e.Input));
        treeNode.Children.Add(this.Visit("Predicate", e.Predicate));
        return treeNode;
      }

      public override TreeNode Visit(DbProjectExpression e)
      {
        Check.NotNull<DbProjectExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(this.VisitBinding("Input", e.Input));
        treeNode.Children.Add(this.Visit("Projection", e.Projection));
        return treeNode;
      }

      public override TreeNode Visit(DbCrossJoinExpression e)
      {
        Check.NotNull<DbCrossJoinExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(this.VisitBindingList("Inputs", e.Inputs));
        return treeNode;
      }

      public override TreeNode Visit(DbJoinExpression e)
      {
        Check.NotNull<DbJoinExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(this.VisitBinding("Left", e.Left));
        treeNode.Children.Add(this.VisitBinding("Right", e.Right));
        treeNode.Children.Add(this.Visit("JoinCondition", e.JoinCondition));
        return treeNode;
      }

      public override TreeNode Visit(DbApplyExpression e)
      {
        Check.NotNull<DbApplyExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(this.VisitBinding("Input", e.Input));
        treeNode.Children.Add(this.VisitBinding("Apply", e.Apply));
        return treeNode;
      }

      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Collections.Generic.List<System.Data.Entity.Core.Common.Utils.TreeNode>)")]
      public override TreeNode Visit(DbGroupByExpression e)
      {
        Check.NotNull<DbGroupByExpression>(e, nameof (e));
        List<TreeNode> children1 = new List<TreeNode>();
        List<TreeNode> children2 = new List<TreeNode>();
        RowType edmType = TypeHelpers.GetEdmType<RowType>(TypeHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage);
        int index1 = 0;
        for (int index2 = 0; index2 < e.Keys.Count; ++index2)
        {
          children1.Add(this.VisitWithLabel("Key", edmType.Properties[index2].Name, e.Keys[index1]));
          ++index1;
        }
        int index3 = 0;
        for (int count = e.Keys.Count; count < edmType.Properties.Count; ++count)
        {
          TreeNode treeNode1 = new TreeNode("Aggregate : '", new TreeNode[0]);
          treeNode1.Text.Append(edmType.Properties[count].Name);
          treeNode1.Text.Append("'");
          DbFunctionAggregate aggregate1 = e.Aggregates[index3] as DbFunctionAggregate;
          if (aggregate1 != null)
          {
            TreeNode treeNode2 = this.VisitFunction(aggregate1.Function, aggregate1.Arguments);
            if (aggregate1.Distinct)
              treeNode2 = new TreeNode("Distinct", new TreeNode[1]
              {
                treeNode2
              });
            treeNode1.Children.Add(treeNode2);
          }
          else
          {
            DbGroupAggregate aggregate2 = e.Aggregates[index3] as DbGroupAggregate;
            treeNode1.Children.Add(this.Visit("GroupAggregate", aggregate2.Arguments[0]));
          }
          children2.Add(treeNode1);
          ++index3;
        }
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(this.VisitGroupBinding(e.Input));
        if (children1.Count > 0)
          treeNode.Children.Add(new TreeNode("Keys", children1));
        if (children2.Count > 0)
          treeNode.Children.Add(new TreeNode("Aggregates", children2));
        return treeNode;
      }

      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SortOrder")]
      [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
      private TreeNode VisitSortOrder(IList<DbSortClause> sortOrder)
      {
        TreeNode treeNode1 = new TreeNode("SortOrder", new TreeNode[0]);
        foreach (DbSortClause dbSortClause in (IEnumerable<DbSortClause>) sortOrder)
        {
          TreeNode treeNode2 = this.Visit(dbSortClause.Ascending ? "Asc" : "Desc", dbSortClause.Expression);
          if (!string.IsNullOrEmpty(dbSortClause.Collation))
          {
            treeNode2.Text.Append(" : ");
            treeNode2.Text.Append(dbSortClause.Collation);
          }
          treeNode1.Children.Add(treeNode2);
        }
        return treeNode1;
      }

      public override TreeNode Visit(DbSkipExpression e)
      {
        Check.NotNull<DbSkipExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(this.VisitBinding("Input", e.Input));
        treeNode.Children.Add(this.VisitSortOrder(e.SortOrder));
        treeNode.Children.Add(this.Visit("Count", e.Count));
        return treeNode;
      }

      public override TreeNode Visit(DbSortExpression e)
      {
        Check.NotNull<DbSortExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(this.VisitBinding("Input", e.Input));
        treeNode.Children.Add(this.VisitSortOrder(e.SortOrder));
        return treeNode;
      }

      public override TreeNode Visit(DbQuantifierExpression e)
      {
        Check.NotNull<DbQuantifierExpression>(e, nameof (e));
        TreeNode treeNode = ExpressionPrinter.PrinterVisitor.NodeFromExpression((DbExpression) e);
        treeNode.Children.Add(this.VisitBinding("Input", e.Input));
        treeNode.Children.Add(this.Visit("Predicate", e.Predicate));
        return treeNode;
      }
    }
  }
}
