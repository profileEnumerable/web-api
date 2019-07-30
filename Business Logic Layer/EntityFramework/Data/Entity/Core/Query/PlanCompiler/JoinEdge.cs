// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.JoinEdge
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class JoinEdge
  {
    private readonly AugmentedTableNode m_left;
    private readonly AugmentedTableNode m_right;
    private readonly AugmentedJoinNode m_joinNode;
    private readonly List<ColumnVar> m_leftVars;
    private readonly List<ColumnVar> m_rightVars;

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private JoinEdge(
      AugmentedTableNode left,
      AugmentedTableNode right,
      AugmentedJoinNode joinNode,
      JoinKind joinKind,
      List<ColumnVar> leftVars,
      List<ColumnVar> rightVars)
    {
      this.m_left = left;
      this.m_right = right;
      this.JoinKind = joinKind;
      this.m_joinNode = joinNode;
      this.m_leftVars = leftVars;
      this.m_rightVars = rightVars;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((this.m_leftVars.Count == this.m_rightVars.Count ? 1 : 0) != 0, "Count mismatch: " + (object) this.m_leftVars.Count + "," + (object) this.m_rightVars.Count);
    }

    internal AugmentedTableNode Left
    {
      get
      {
        return this.m_left;
      }
    }

    internal AugmentedTableNode Right
    {
      get
      {
        return this.m_right;
      }
    }

    internal AugmentedJoinNode JoinNode
    {
      get
      {
        return this.m_joinNode;
      }
    }

    internal JoinKind JoinKind { get; set; }

    internal List<ColumnVar> LeftVars
    {
      get
      {
        return this.m_leftVars;
      }
    }

    internal List<ColumnVar> RightVars
    {
      get
      {
        return this.m_rightVars;
      }
    }

    internal bool IsEliminated
    {
      get
      {
        if (!this.Left.IsEliminated)
          return this.Right.IsEliminated;
        return true;
      }
    }

    internal bool RestrictedElimination
    {
      get
      {
        if (this.m_joinNode == null)
          return false;
        if (this.m_joinNode.OtherPredicate == null && this.m_left.LastVisibleId >= this.m_joinNode.Id)
          return this.m_right.LastVisibleId < this.m_joinNode.Id;
        return true;
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal static JoinEdge CreateJoinEdge(
      AugmentedTableNode left,
      AugmentedTableNode right,
      AugmentedJoinNode joinNode,
      ColumnVar leftVar,
      ColumnVar rightVar)
    {
      List<ColumnVar> leftVars = new List<ColumnVar>();
      List<ColumnVar> rightVars = new List<ColumnVar>();
      leftVars.Add(leftVar);
      rightVars.Add(rightVar);
      OpType opType = joinNode.Node.Op.OpType;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(opType == OpType.LeftOuterJoin || opType == OpType.InnerJoin, "Unexpected join type for join edge: " + (object) opType);
      JoinKind joinKind = opType == OpType.LeftOuterJoin ? JoinKind.LeftOuter : JoinKind.Inner;
      return new JoinEdge(left, right, joinNode, joinKind, leftVars, rightVars);
    }

    internal static JoinEdge CreateTransitiveJoinEdge(
      AugmentedTableNode left,
      AugmentedTableNode right,
      JoinKind joinKind,
      List<ColumnVar> leftVars,
      List<ColumnVar> rightVars)
    {
      return new JoinEdge(left, right, (AugmentedJoinNode) null, joinKind, leftVars, rightVars);
    }

    internal bool AddCondition(AugmentedJoinNode joinNode, ColumnVar leftVar, ColumnVar rightVar)
    {
      if (joinNode != this.m_joinNode)
        return false;
      this.m_leftVars.Add(leftVar);
      this.m_rightVars.Add(rightVar);
      return true;
    }
  }
}
