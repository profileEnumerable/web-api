// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ScalarOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class ScalarOpRules
  {
    internal static readonly SimpleRule Rule_SimplifyCase = new SimpleRule(OpType.Case, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessSimplifyCase));
    internal static readonly SimpleRule Rule_FlattenCase = new SimpleRule(OpType.Case, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessFlattenCase));
    internal static readonly PatternMatchRule Rule_IsNullOverCase = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternIsNull, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CaseOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessIsNullOverCase));
    internal static readonly PatternMatchRule Rule_EqualsOverConstant = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ComparisonOp.PatternEq, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InternalConstantOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InternalConstantOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessComparisonsOverConstant));
    internal static readonly PatternMatchRule Rule_LikeOverConstants = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LikeOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InternalConstantOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InternalConstantOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) NullOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessLikeOverConstant));
    internal static readonly PatternMatchRule Rule_AndOverConstantPred1 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternAnd, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConstantPredicateOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessAndOverConstantPredicate1));
    internal static readonly PatternMatchRule Rule_AndOverConstantPred2 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternAnd, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConstantPredicateOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessAndOverConstantPredicate2));
    internal static readonly PatternMatchRule Rule_OrOverConstantPred1 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternOr, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConstantPredicateOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessOrOverConstantPredicate1));
    internal static readonly PatternMatchRule Rule_OrOverConstantPred2 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternOr, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConstantPredicateOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessOrOverConstantPredicate2));
    internal static readonly PatternMatchRule Rule_NotOverConstantPred = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternNot, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConstantPredicateOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessNotOverConstantPredicate));
    internal static readonly PatternMatchRule Rule_IsNullOverConstant = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternIsNull, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InternalConstantOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessIsNullOverConstant));
    internal static readonly PatternMatchRule Rule_IsNullOverNullSentinel = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternIsNull, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) NullSentinelOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessIsNullOverConstant));
    internal static readonly PatternMatchRule Rule_IsNullOverNull = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternIsNull, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) NullOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessIsNullOverNull));
    internal static readonly PatternMatchRule Rule_NullCast = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CastOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) NullOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessNullCast));
    internal static readonly PatternMatchRule Rule_IsNullOverVarRef = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternIsNull, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) VarRefOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessIsNullOverVarRef));
    internal static readonly PatternMatchRule Rule_IsNullOverAnything = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConditionalOp.PatternIsNull, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ScalarOpRules.ProcessIsNullOverAnything));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[15]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_IsNullOverCase,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_SimplifyCase,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_FlattenCase,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_LikeOverConstants,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_EqualsOverConstant,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_AndOverConstantPred1,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_AndOverConstantPred2,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_OrOverConstantPred1,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_OrOverConstantPred2,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_NotOverConstantPred,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_IsNullOverConstant,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_IsNullOverNullSentinel,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_IsNullOverNull,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_NullCast,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_IsNullOverVarRef
    };

    private static bool ProcessSimplifyCase(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node caseOpNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      CaseOp op = (CaseOp) caseOpNode.Op;
      newNode = caseOpNode;
      return ScalarOpRules.ProcessSimplifyCase_Collapse(caseOpNode, out newNode) || ScalarOpRules.ProcessSimplifyCase_EliminateWhenClauses(context, op, caseOpNode, out newNode);
    }

    private static bool ProcessSimplifyCase_Collapse(System.Data.Entity.Core.Query.InternalTrees.Node caseOpNode, out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = caseOpNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = caseOpNode.Child1;
      System.Data.Entity.Core.Query.InternalTrees.Node child = caseOpNode.Children[caseOpNode.Children.Count - 1];
      if (!child1.IsEquivalent(child))
        return false;
      for (int index = 3; index < caseOpNode.Children.Count - 1; index += 2)
      {
        if (!caseOpNode.Children[index].IsEquivalent(child1))
          return false;
      }
      newNode = child1;
      return true;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static bool ProcessSimplifyCase_EliminateWhenClauses(
      RuleProcessingContext context,
      CaseOp caseOp,
      System.Data.Entity.Core.Query.InternalTrees.Node caseOpNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = (List<System.Data.Entity.Core.Query.InternalTrees.Node>) null;
      newNode = caseOpNode;
      int index1 = 0;
      while (index1 < caseOpNode.Children.Count)
      {
        if (index1 == caseOpNode.Children.Count - 1)
        {
          if (OpType.SoftCast == caseOpNode.Children[index1].Op.OpType)
            return false;
          if (args != null)
          {
            args.Add(caseOpNode.Children[index1]);
            break;
          }
          break;
        }
        if (OpType.SoftCast == caseOpNode.Children[index1 + 1].Op.OpType)
          return false;
        if (caseOpNode.Children[index1].Op.OpType != OpType.ConstantPredicate)
        {
          if (args != null)
          {
            args.Add(caseOpNode.Children[index1]);
            args.Add(caseOpNode.Children[index1 + 1]);
          }
          index1 += 2;
        }
        else
        {
          ConstantPredicateOp op = (ConstantPredicateOp) caseOpNode.Children[index1].Op;
          if (args == null)
          {
            args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
            for (int index2 = 0; index2 < index1; ++index2)
              args.Add(caseOpNode.Children[index2]);
          }
          if (op.IsTrue)
          {
            args.Add(caseOpNode.Children[index1 + 1]);
            break;
          }
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.IsFalse, "constant predicate must be either true or false");
          index1 += 2;
        }
      }
      if (args == null)
        return false;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(args.Count > 0, "new args list must not be empty");
      newNode = args.Count != 1 ? context.Command.CreateNode((Op) caseOp, args) : args[0];
      return true;
    }

    private static bool ProcessFlattenCase(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node caseOpNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = caseOpNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child = caseOpNode.Children[caseOpNode.Children.Count - 1];
      if (child.Op.OpType != OpType.Case)
        return false;
      caseOpNode.Children.RemoveAt(caseOpNode.Children.Count - 1);
      caseOpNode.Children.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) child.Children);
      return true;
    }

    private static bool ProcessIsNullOverCase(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node isNullOpNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node child0_1 = isNullOpNode.Child0;
      if (child0_1.Children.Count != 3)
      {
        newNode = isNullOpNode;
        return false;
      }
      System.Data.Entity.Core.Query.InternalTrees.Node child0_2 = child0_1.Child0;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = child0_1.Child1;
      System.Data.Entity.Core.Query.InternalTrees.Node child2 = child0_1.Child2;
      switch (child1.Op.OpType)
      {
        case OpType.Constant:
        case OpType.InternalConstant:
        case OpType.NullSentinel:
          if (child2.Op.OpType == OpType.Null)
          {
            newNode = context.Command.CreateNode((Op) context.Command.CreateConditionalOp(OpType.Not), child0_2);
            return true;
          }
          break;
        case OpType.Null:
          switch (child2.Op.OpType)
          {
            case OpType.Constant:
            case OpType.InternalConstant:
            case OpType.NullSentinel:
              newNode = child0_2;
              return true;
          }
      }
      newNode = isNullOpNode;
      return false;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static bool ProcessComparisonsOverConstant(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = node;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node.Op.OpType == OpType.EQ || node.Op.OpType == OpType.NE, "unexpected comparison op type?");
      bool? nullable = new bool?(node.Child0.Op.IsEquivalent(node.Child1.Op));
      if (!nullable.HasValue)
        return false;
      bool flag = node.Op.OpType == OpType.EQ ? nullable.Value : !nullable.Value;
      ConstantPredicateOp constantPredicateOp = context.Command.CreateConstantPredicateOp(flag);
      newNode = context.Command.CreateNode((Op) constantPredicateOp);
      return true;
    }

    private static bool? MatchesPattern(string str, string pattern)
    {
      int num = pattern.IndexOf('%');
      if (num == -1 || num != pattern.Length - 1 || pattern.Length > str.Length + 1)
        return new bool?();
      bool flag = true;
      for (int index = 0; index < str.Length && index < pattern.Length - 1; ++index)
      {
        if ((int) pattern[index] != (int) str[index])
        {
          flag = false;
          break;
        }
      }
      return new bool?(flag);
    }

    private static bool ProcessLikeOverConstant(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = n;
      InternalConstantOp op1 = (InternalConstantOp) n.Child1.Op;
      InternalConstantOp op2 = (InternalConstantOp) n.Child0.Op;
      string str1 = (string) op2.Value;
      string str2 = (string) op1.Value;
      bool? nullable = ScalarOpRules.MatchesPattern((string) op2.Value, (string) op1.Value);
      if (!nullable.HasValue)
        return false;
      ConstantPredicateOp constantPredicateOp = context.Command.CreateConstantPredicateOp(nullable.Value);
      newNode = context.Command.CreateNode((Op) constantPredicateOp);
      return true;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OpType")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "constantPredicateOp")]
    private static bool ProcessLogOpOverConstant(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      System.Data.Entity.Core.Query.InternalTrees.Node constantPredicateNode,
      System.Data.Entity.Core.Query.InternalTrees.Node otherNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(constantPredicateNode != null, "null constantPredicateOp?");
      ConstantPredicateOp op = (ConstantPredicateOp) constantPredicateNode.Op;
      switch (node.Op.OpType)
      {
        case OpType.And:
          newNode = op.IsTrue ? otherNode : constantPredicateNode;
          break;
        case OpType.Or:
          newNode = op.IsTrue ? constantPredicateNode : otherNode;
          break;
        case OpType.Not:
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(otherNode == null, "Not Op with more than 1 child. Gasp!");
          newNode = context.Command.CreateNode((Op) context.Command.CreateConstantPredicateOp(!op.Value));
          break;
        default:
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "Unexpected OpType - " + (object) node.Op.OpType);
          newNode = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
          break;
      }
      return true;
    }

    private static bool ProcessAndOverConstantPredicate1(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      return ScalarOpRules.ProcessLogOpOverConstant(context, node, node.Child1, node.Child0, out newNode);
    }

    private static bool ProcessAndOverConstantPredicate2(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      return ScalarOpRules.ProcessLogOpOverConstant(context, node, node.Child0, node.Child1, out newNode);
    }

    private static bool ProcessOrOverConstantPredicate1(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      return ScalarOpRules.ProcessLogOpOverConstant(context, node, node.Child1, node.Child0, out newNode);
    }

    private static bool ProcessOrOverConstantPredicate2(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      return ScalarOpRules.ProcessLogOpOverConstant(context, node, node.Child0, node.Child1, out newNode);
    }

    private static bool ProcessNotOverConstantPredicate(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      return ScalarOpRules.ProcessLogOpOverConstant(context, node, node.Child0, (System.Data.Entity.Core.Query.InternalTrees.Node) null, out newNode);
    }

    private static bool ProcessIsNullOverConstant(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node isNullNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = context.Command.CreateNode((Op) context.Command.CreateFalseOp());
      return true;
    }

    private static bool ProcessIsNullOverNull(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node isNullNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = context.Command.CreateNode((Op) context.Command.CreateTrueOp());
      return true;
    }

    private static bool ProcessNullCast(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node castNullOp,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = context.Command.CreateNode((Op) context.Command.CreateNullOp(castNullOp.Op.Type));
      return true;
    }

    private static bool ProcessIsNullOverVarRef(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node isNullNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      Command command = context.Command;
      if (((TransformationRulesContext) context).IsNonNullable(((VarRefOp) isNullNode.Child0.Op).Var))
      {
        newNode = command.CreateNode((Op) context.Command.CreateFalseOp());
        return true;
      }
      newNode = isNullNode;
      return false;
    }

    private static bool ProcessIsNullOverAnything(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node isNullNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      Command command = context.Command;
      switch (isNullNode.Child0.Op.OpType)
      {
        case OpType.Cast:
          newNode = command.CreateNode((Op) command.CreateConditionalOp(OpType.IsNull), isNullNode.Child0.Child0);
          break;
        case OpType.Function:
          EdmFunction function = ((FunctionOp) isNullNode.Child0.Op).Function;
          newNode = ScalarOpRules.PreservesNulls(function) ? command.CreateNode((Op) command.CreateConditionalOp(OpType.IsNull), isNullNode.Child0.Child0) : isNullNode;
          break;
        default:
          newNode = isNullNode;
          break;
      }
      switch (isNullNode.Child0.Op.OpType)
      {
        case OpType.Constant:
        case OpType.InternalConstant:
        case OpType.NullSentinel:
          return ScalarOpRules.ProcessIsNullOverConstant(context, newNode, out newNode);
        case OpType.Null:
          return ScalarOpRules.ProcessIsNullOverNull(context, newNode, out newNode);
        case OpType.VarRef:
          return ScalarOpRules.ProcessIsNullOverVarRef(context, newNode, out newNode);
        default:
          return !object.ReferenceEquals((object) isNullNode, (object) newNode);
      }
    }

    private static bool PreservesNulls(EdmFunction function)
    {
      return function.FullName == "Edm.Length";
    }
  }
}
