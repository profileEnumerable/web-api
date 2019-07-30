// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.SingleRowOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class SingleRowOpRules
  {
    internal static readonly PatternMatchRule Rule_SingleRowOpOverAnything = new PatternMatchRule(new Node((Op) SingleRowOp.Pattern, new Node[1]
    {
      new Node((Op) LeafOp.Pattern, new Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(SingleRowOpRules.ProcessSingleRowOpOverAnything));
    internal static readonly PatternMatchRule Rule_SingleRowOpOverProject = new PatternMatchRule(new Node((Op) SingleRowOp.Pattern, new Node[1]
    {
      new Node((Op) ProjectOp.Pattern, new Node[2]
      {
        new Node((Op) LeafOp.Pattern, new Node[0]),
        new Node((Op) LeafOp.Pattern, new Node[0])
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(SingleRowOpRules.ProcessSingleRowOpOverProject));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[2]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) SingleRowOpRules.Rule_SingleRowOpOverAnything,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) SingleRowOpRules.Rule_SingleRowOpOverProject
    };

    private static bool ProcessSingleRowOpOverAnything(
      RuleProcessingContext context,
      Node singleRowNode,
      out Node newNode)
    {
      newNode = singleRowNode;
      ExtendedNodeInfo extendedNodeInfo = context.Command.GetExtendedNodeInfo(singleRowNode.Child0);
      if (extendedNodeInfo.MaxRows <= RowCount.One)
      {
        newNode = singleRowNode.Child0;
        return true;
      }
      if (singleRowNode.Child0.Op.OpType != OpType.Filter || !new Predicate(context.Command, singleRowNode.Child0.Child1).SatisfiesKey(extendedNodeInfo.Keys.KeyVars, extendedNodeInfo.Definitions))
        return false;
      extendedNodeInfo.MaxRows = RowCount.One;
      newNode = singleRowNode.Child0;
      return true;
    }

    private static bool ProcessSingleRowOpOverProject(
      RuleProcessingContext context,
      Node singleRowNode,
      out Node newNode)
    {
      newNode = singleRowNode;
      Node child0_1 = singleRowNode.Child0;
      Node child0_2 = child0_1.Child0;
      singleRowNode.Child0 = child0_2;
      context.Command.RecomputeNodeInfo(singleRowNode);
      child0_1.Child0 = singleRowNode;
      newNode = child0_1;
      return true;
    }
  }
}
