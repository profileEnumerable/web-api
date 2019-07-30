// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ConstrainedSortOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class ConstrainedSortOpRules
  {
    internal static readonly SimpleRule Rule_ConstrainedSortOpOverEmptySet = new SimpleRule(OpType.ConstrainedSort, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ConstrainedSortOpRules.ProcessConstrainedSortOpOverEmptySet));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[1]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ConstrainedSortOpRules.Rule_ConstrainedSortOpOverEmptySet
    };

    private static bool ProcessConstrainedSortOpOverEmptySet(
      RuleProcessingContext context,
      Node n,
      out Node newNode)
    {
      if (context.Command.GetExtendedNodeInfo(n.Child0).MaxRows == RowCount.Zero)
      {
        newNode = n.Child0;
        return true;
      }
      newNode = n;
      return false;
    }
  }
}
