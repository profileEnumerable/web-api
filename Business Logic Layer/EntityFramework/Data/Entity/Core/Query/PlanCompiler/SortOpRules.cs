// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.SortOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class SortOpRules
  {
    internal static readonly SimpleRule Rule_SortOpOverAtMostOneRow = new SimpleRule(OpType.Sort, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(SortOpRules.ProcessSortOpOverAtMostOneRow));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[1]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) SortOpRules.Rule_SortOpOverAtMostOneRow
    };

    private static bool ProcessSortOpOverAtMostOneRow(
      RuleProcessingContext context,
      Node n,
      out Node newNode)
    {
      ExtendedNodeInfo extendedNodeInfo = context.Command.GetExtendedNodeInfo(n.Child0);
      if (extendedNodeInfo.MaxRows == RowCount.Zero || extendedNodeInfo.MaxRows == RowCount.One)
      {
        newNode = n.Child0;
        return true;
      }
      newNode = n;
      return false;
    }
  }
}
