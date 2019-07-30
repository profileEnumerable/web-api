// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.FilterOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class FilterOpRules
  {
    internal static readonly PatternMatchRule Rule_FilterOverFilter = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverFilter));
    internal static readonly PatternMatchRule Rule_FilterOverProject = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverProject));
    internal static readonly PatternMatchRule Rule_FilterOverUnionAll = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) UnionAllOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverSetOp));
    internal static readonly PatternMatchRule Rule_FilterOverIntersect = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) IntersectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverSetOp));
    internal static readonly PatternMatchRule Rule_FilterOverExcept = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ExceptOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverSetOp));
    internal static readonly PatternMatchRule Rule_FilterOverDistinct = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) DistinctOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverDistinct));
    internal static readonly PatternMatchRule Rule_FilterOverGroupBy = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) GroupByOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverGroupBy));
    internal static readonly PatternMatchRule Rule_FilterOverCrossJoin = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverJoin));
    internal static readonly PatternMatchRule Rule_FilterOverInnerJoin = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InnerJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverJoin));
    internal static readonly PatternMatchRule Rule_FilterOverLeftOuterJoin = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeftOuterJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverJoin));
    internal static readonly PatternMatchRule Rule_FilterOverOuterApply = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) OuterApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverOuterApply));
    internal static readonly PatternMatchRule Rule_FilterWithConstantPredicate = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConstantPredicateOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterWithConstantPredicate));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[12]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterWithConstantPredicate,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverCrossJoin,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverDistinct,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverExcept,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverFilter,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverGroupBy,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverInnerJoin,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverIntersect,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverLeftOuterJoin,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverProject,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverUnionAll,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverOuterApply
    };

    private static System.Data.Entity.Core.Query.InternalTrees.Node GetPushdownPredicate(
      Command command,
      System.Data.Entity.Core.Query.InternalTrees.Node filterNode,
      VarVec columns,
      out System.Data.Entity.Core.Query.InternalTrees.Node nonPushdownPredicateNode)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = filterNode.Child1;
      nonPushdownPredicateNode = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(filterNode);
      if (columns == null && extendedNodeInfo.ExternalReferences.IsEmpty)
        return child1;
      if (columns == null)
        columns = command.GetExtendedNodeInfo(filterNode.Child0).Definitions;
      Predicate otherPredicates;
      System.Data.Entity.Core.Query.InternalTrees.Node node = new Predicate(command, child1).GetSingleTablePredicates(columns, out otherPredicates).BuildAndTree();
      nonPushdownPredicateNode = otherPredicates.BuildAndTree();
      return node;
    }

    private static bool ProcessFilterOverFilter(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node filterNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node = context.Command.CreateNode((Op) context.Command.CreateConditionalOp(OpType.And), filterNode.Child0.Child1, filterNode.Child1);
      newNode = context.Command.CreateNode((Op) context.Command.CreateFilterOp(), filterNode.Child0.Child0, node);
      return true;
    }

    private static bool ProcessFilterOverProject(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node filterNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = filterNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = filterNode.Child1;
      if (child1.Op.OpType == OpType.ConstantPredicate)
        return false;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Dictionary<Var, int> varRefMap = new Dictionary<Var, int>();
      if (!transformationRulesContext.IsScalarOpTree(child1, varRefMap))
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = filterNode.Child0;
      Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varMap = transformationRulesContext.GetVarMap(child0.Child1, varRefMap);
      if (varMap == null)
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = transformationRulesContext.ReMap(child1, varMap);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = transformationRulesContext.Command.CreateNode((Op) transformationRulesContext.Command.CreateFilterOp(), child0.Child0, node1);
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = transformationRulesContext.Command.CreateNode(child0.Op, node2, child0.Child1);
      newNode = node3;
      return true;
    }

    private static bool ProcessFilterOverSetOp(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node filterNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = filterNode;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      System.Data.Entity.Core.Query.InternalTrees.Node nonPushdownPredicateNode;
      System.Data.Entity.Core.Query.InternalTrees.Node pushdownPredicate = FilterOpRules.GetPushdownPredicate(transformationRulesContext.Command, filterNode, (VarVec) null, out nonPushdownPredicateNode);
      if (pushdownPredicate == null || !transformationRulesContext.IsScalarOpTree(pushdownPredicate))
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = filterNode.Child0;
      SetOp op = (SetOp) child0.Op;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      int index = 0;
      foreach (VarMap var in op.VarMap)
      {
        if (op.OpType == OpType.Except && index == 1)
        {
          args.Add(child0.Child1);
          break;
        }
        Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varMap = new Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node>();
        foreach (KeyValuePair<Var, Var> keyValuePair in (Dictionary<Var, Var>) var)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node node = transformationRulesContext.Command.CreateNode((Op) transformationRulesContext.Command.CreateVarRefOp(keyValuePair.Value));
          varMap.Add(keyValuePair.Key, node);
        }
        System.Data.Entity.Core.Query.InternalTrees.Node node1 = pushdownPredicate;
        if (index == 0 && filterNode.Op.OpType != OpType.Except)
          node1 = transformationRulesContext.Copy(node1);
        System.Data.Entity.Core.Query.InternalTrees.Node n = transformationRulesContext.ReMap(node1, varMap);
        transformationRulesContext.Command.RecomputeNodeInfo(n);
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = transformationRulesContext.Command.CreateNode((Op) transformationRulesContext.Command.CreateFilterOp(), child0.Children[index], n);
        args.Add(node2);
        ++index;
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = transformationRulesContext.Command.CreateNode(child0.Op, args);
      newNode = nonPushdownPredicateNode == null ? node3 : transformationRulesContext.Command.CreateNode((Op) transformationRulesContext.Command.CreateFilterOp(), node3, nonPushdownPredicateNode);
      return true;
    }

    private static bool ProcessFilterOverDistinct(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node filterNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = filterNode;
      System.Data.Entity.Core.Query.InternalTrees.Node nonPushdownPredicateNode;
      System.Data.Entity.Core.Query.InternalTrees.Node pushdownPredicate = FilterOpRules.GetPushdownPredicate(context.Command, filterNode, (VarVec) null, out nonPushdownPredicateNode);
      if (pushdownPredicate == null)
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = filterNode.Child0;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = context.Command.CreateNode((Op) context.Command.CreateFilterOp(), child0.Child0, pushdownPredicate);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = context.Command.CreateNode(child0.Op, node1);
      newNode = nonPushdownPredicateNode == null ? node2 : context.Command.CreateNode((Op) context.Command.CreateFilterOp(), node2, nonPushdownPredicateNode);
      return true;
    }

    private static bool ProcessFilterOverGroupBy(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node filterNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = filterNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = filterNode.Child0;
      GroupByOp op = (GroupByOp) child0.Op;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Dictionary<Var, int> varRefMap = new Dictionary<Var, int>();
      if (!transformationRulesContext.IsScalarOpTree(filterNode.Child1, varRefMap))
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node nonPushdownPredicateNode;
      System.Data.Entity.Core.Query.InternalTrees.Node pushdownPredicate = FilterOpRules.GetPushdownPredicate(context.Command, filterNode, op.Keys, out nonPushdownPredicateNode);
      if (pushdownPredicate == null)
        return false;
      Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varMap = transformationRulesContext.GetVarMap(child0.Child1, varRefMap);
      if (varMap == null)
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = transformationRulesContext.ReMap(pushdownPredicate, varMap);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = transformationRulesContext.Command.CreateNode((Op) transformationRulesContext.Command.CreateFilterOp(), child0.Child0, node1);
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = transformationRulesContext.Command.CreateNode(child0.Op, node2, child0.Child1, child0.Child2);
      newNode = nonPushdownPredicateNode != null ? transformationRulesContext.Command.CreateNode((Op) transformationRulesContext.Command.CreateFilterOp(), node3, nonPushdownPredicateNode) : node3;
      return true;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-InnerJoin")]
    private static bool ProcessFilterOverJoin(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node filterNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = filterNode;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      if (transformationRulesContext.IsFilterPushdownSuppressed(filterNode))
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = filterNode.Child0;
      Op op = child0.Op;
      System.Data.Entity.Core.Query.InternalTrees.Node n1 = child0.Child0;
      System.Data.Entity.Core.Query.InternalTrees.Node n2 = child0.Child1;
      Command command = transformationRulesContext.Command;
      bool flag = false;
      ExtendedNodeInfo extendedNodeInfo1 = command.GetExtendedNodeInfo(n2);
      Predicate otherPredicates = new Predicate(command, filterNode.Child1);
      if (op.OpType == OpType.LeftOuterJoin && !otherPredicates.PreservesNulls(extendedNodeInfo1.Definitions, true))
      {
        if (transformationRulesContext.PlanCompiler.IsAfterPhase(PlanCompilerPhase.NullSemantics) && transformationRulesContext.PlanCompiler.IsAfterPhase(PlanCompilerPhase.JoinElimination))
        {
          op = (Op) command.CreateInnerJoinOp();
          flag = true;
        }
        else
          transformationRulesContext.PlanCompiler.TransformationsDeferred = true;
      }
      ExtendedNodeInfo extendedNodeInfo2 = command.GetExtendedNodeInfo(n1);
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      if (n1.Op.OpType != OpType.ScanTable)
        node1 = otherPredicates.GetSingleTablePredicates(extendedNodeInfo2.Definitions, out otherPredicates).BuildAndTree();
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      if (n2.Op.OpType != OpType.ScanTable && op.OpType != OpType.LeftOuterJoin)
        node2 = otherPredicates.GetSingleTablePredicates(extendedNodeInfo1.Definitions, out otherPredicates).BuildAndTree();
      System.Data.Entity.Core.Query.InternalTrees.Node predicate2 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      if (op.OpType == OpType.CrossJoin || op.OpType == OpType.InnerJoin)
        predicate2 = otherPredicates.GetJoinPredicates(extendedNodeInfo2.Definitions, extendedNodeInfo1.Definitions, out otherPredicates).BuildAndTree();
      if (node1 != null)
      {
        n1 = command.CreateNode((Op) command.CreateFilterOp(), n1, node1);
        flag = true;
      }
      if (node2 != null)
      {
        n2 = command.CreateNode((Op) command.CreateFilterOp(), n2, node2);
        flag = true;
      }
      if (predicate2 != null)
      {
        flag = true;
        if (op.OpType == OpType.CrossJoin)
        {
          op = (Op) command.CreateInnerJoinOp();
        }
        else
        {
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.OpType == OpType.InnerJoin, "unexpected non-InnerJoin?");
          predicate2 = PlanCompilerUtil.CombinePredicates(child0.Child2, predicate2, command);
        }
      }
      else
        predicate2 = op.OpType == OpType.CrossJoin ? (System.Data.Entity.Core.Query.InternalTrees.Node) null : child0.Child2;
      if (!flag)
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = op.OpType != OpType.CrossJoin ? command.CreateNode(op, n1, n2, predicate2) : command.CreateNode(op, n1, n2);
      System.Data.Entity.Core.Query.InternalTrees.Node node4 = otherPredicates.BuildAndTree();
      newNode = node4 != null ? command.CreateNode((Op) command.CreateFilterOp(), node3, node4) : node3;
      return true;
    }

    private static bool ProcessFilterOverOuterApply(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node filterNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = filterNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = filterNode.Child0;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = child0.Child1;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Command command = transformationRulesContext.Command;
      ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(child1);
      if (!new Predicate(command, filterNode.Child1).PreservesNulls(extendedNodeInfo.Definitions, true))
      {
        if (transformationRulesContext.PlanCompiler.IsAfterPhase(PlanCompilerPhase.NullSemantics) && transformationRulesContext.PlanCompiler.IsAfterPhase(PlanCompilerPhase.JoinElimination))
        {
          System.Data.Entity.Core.Query.InternalTrees.Node node1 = command.CreateNode((Op) command.CreateCrossApplyOp(), child0.Child0, child1);
          System.Data.Entity.Core.Query.InternalTrees.Node node2 = command.CreateNode((Op) command.CreateFilterOp(), node1, filterNode.Child1);
          newNode = node2;
          return true;
        }
        transformationRulesContext.PlanCompiler.TransformationsDeferred = true;
      }
      return false;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static bool ProcessFilterWithConstantPredicate(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = n;
      ConstantPredicateOp op = (ConstantPredicateOp) n.Child1.Op;
      if (op.IsTrue)
      {
        newNode = n.Child0;
        return true;
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.IsFalse, "unexpected non-false predicate?");
      if (n.Child0.Op.OpType == OpType.SingleRowTable || n.Child0.Op.OpType == OpType.Project && n.Child0.Child0.Op.OpType == OpType.SingleRowTable)
        return false;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      ExtendedNodeInfo extendedNodeInfo = transformationRulesContext.Command.GetExtendedNodeInfo(n.Child0);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      VarVec varVec = transformationRulesContext.Command.CreateVarVec();
      foreach (Var definition in extendedNodeInfo.Definitions)
      {
        NullOp nullOp = transformationRulesContext.Command.CreateNullOp(definition.Type);
        System.Data.Entity.Core.Query.InternalTrees.Node node = transformationRulesContext.Command.CreateNode((Op) nullOp);
        Var computedVar;
        System.Data.Entity.Core.Query.InternalTrees.Node varDefNode = transformationRulesContext.Command.CreateVarDefNode(node, out computedVar);
        transformationRulesContext.AddVarMapping(definition, computedVar);
        varVec.Set(computedVar);
        args.Add(varDefNode);
      }
      if (varVec.IsEmpty)
      {
        NullOp nullOp = transformationRulesContext.Command.CreateNullOp(transformationRulesContext.Command.BooleanType);
        System.Data.Entity.Core.Query.InternalTrees.Node node = transformationRulesContext.Command.CreateNode((Op) nullOp);
        Var computedVar;
        System.Data.Entity.Core.Query.InternalTrees.Node varDefNode = transformationRulesContext.Command.CreateVarDefNode(node, out computedVar);
        varVec.Set(computedVar);
        args.Add(varDefNode);
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = transformationRulesContext.Command.CreateNode((Op) transformationRulesContext.Command.CreateSingleRowTableOp());
      n.Child0 = node1;
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = transformationRulesContext.Command.CreateNode((Op) transformationRulesContext.Command.CreateVarDefListOp(), args);
      ProjectOp projectOp = transformationRulesContext.Command.CreateProjectOp(varVec);
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = transformationRulesContext.Command.CreateNode((Op) projectOp, n, node2);
      node3.Child0 = n;
      newNode = node3;
      return true;
    }
  }
}
