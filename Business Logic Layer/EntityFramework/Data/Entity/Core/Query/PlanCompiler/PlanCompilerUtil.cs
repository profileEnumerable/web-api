// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.PlanCompilerUtil
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class PlanCompilerUtil
  {
    internal static bool IsRowTypeCaseOpWithNullability(
      CaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out bool thenClauseIsNull)
    {
      thenClauseIsNull = false;
      if (!TypeSemantics.IsRowType(op.Type) || n.Children.Count != 3 || (!n.Child1.Op.Type.EdmEquals((MetadataItem) op.Type) || !n.Child2.Op.Type.EdmEquals((MetadataItem) op.Type)))
        return false;
      if (n.Child1.Op.OpType == OpType.Null)
      {
        thenClauseIsNull = true;
        return true;
      }
      return n.Child2.Op.OpType == OpType.Null;
    }

    internal static bool IsCollectionAggregateFunction(FunctionOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (n.Children.Count == 1 && TypeSemantics.IsCollectionType(n.Child0.Op.Type))
        return TypeSemantics.IsAggregateFunction(op.Function);
      return false;
    }

    internal static bool IsConstantBaseOp(OpType opType)
    {
      if (opType != OpType.Constant && opType != OpType.InternalConstant && opType != OpType.Null)
        return opType == OpType.NullSentinel;
      return true;
    }

    internal static System.Data.Entity.Core.Query.InternalTrees.Node CombinePredicates(
      System.Data.Entity.Core.Query.InternalTrees.Node predicate1,
      System.Data.Entity.Core.Query.InternalTrees.Node predicate2,
      Command command)
    {
      IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node> nodes1 = PlanCompilerUtil.BreakIntoAndParts(predicate1);
      IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node> nodes2 = PlanCompilerUtil.BreakIntoAndParts(predicate2);
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = predicate1;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node other in nodes2)
      {
        bool flag = false;
        foreach (System.Data.Entity.Core.Query.InternalTrees.Node node2 in nodes1)
        {
          if (node2.IsEquivalent(other))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          node1 = command.CreateNode((Op) command.CreateConditionalOp(OpType.And), node1, other);
      }
      return node1;
    }

    private static IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node> BreakIntoAndParts(
      System.Data.Entity.Core.Query.InternalTrees.Node predicate)
    {
      return Helpers.GetLeafNodes<System.Data.Entity.Core.Query.InternalTrees.Node>(predicate, (Func<System.Data.Entity.Core.Query.InternalTrees.Node, bool>) (node => node.Op.OpType != OpType.And), (Func<System.Data.Entity.Core.Query.InternalTrees.Node, IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>>) (node => (IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        node.Child0,
        node.Child1
      }));
    }
  }
}
