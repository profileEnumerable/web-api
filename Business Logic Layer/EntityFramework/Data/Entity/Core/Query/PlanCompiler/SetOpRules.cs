// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.SetOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class SetOpRules
  {
    internal static readonly SimpleRule Rule_UnionAllOverEmptySet = new SimpleRule(OpType.UnionAll, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(SetOpRules.ProcessSetOpOverEmptySet));
    internal static readonly SimpleRule Rule_IntersectOverEmptySet = new SimpleRule(OpType.Intersect, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(SetOpRules.ProcessSetOpOverEmptySet));
    internal static readonly SimpleRule Rule_ExceptOverEmptySet = new SimpleRule(OpType.Except, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(SetOpRules.ProcessSetOpOverEmptySet));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[3]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) SetOpRules.Rule_UnionAllOverEmptySet,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) SetOpRules.Rule_IntersectOverEmptySet,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) SetOpRules.Rule_ExceptOverEmptySet
    };

    private static bool ProcessSetOpOverEmptySet(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node setOpNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      bool flag1 = context.Command.GetExtendedNodeInfo(setOpNode.Child0).MaxRows == RowCount.Zero;
      bool flag2 = context.Command.GetExtendedNodeInfo(setOpNode.Child1).MaxRows == RowCount.Zero;
      if (!flag1 && !flag2)
      {
        newNode = setOpNode;
        return false;
      }
      SetOp op = (SetOp) setOpNode.Op;
      int index = !flag2 && op.OpType == OpType.UnionAll || !flag1 && op.OpType == OpType.Intersect ? 1 : 0;
      newNode = setOpNode.Children[index];
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      foreach (KeyValuePair<Var, Var> keyValuePair in (Dictionary<Var, Var>) op.VarMap[index])
        transformationRulesContext.AddVarMapping(keyValuePair.Key, keyValuePair.Value);
      return true;
    }
  }
}
