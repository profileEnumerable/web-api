// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.AggregatePushdown
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class AggregatePushdown
  {
    private readonly Command m_command;
    private TryGetValue m_tryGetParent;

    private AggregatePushdown(Command command)
    {
      this.m_command = command;
    }

    internal static void Process(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler planCompilerState)
    {
      new AggregatePushdown(planCompilerState.Command).Process();
    }

    private void Process()
    {
      foreach (GroupAggregateVarInfo groupAggregateVarInfo in GroupAggregateRefComputingVisitor.Process(this.m_command, out this.m_tryGetParent))
      {
        if (groupAggregateVarInfo.HasCandidateAggregateNodes)
        {
          foreach (KeyValuePair<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node> candidateAggregateNode in groupAggregateVarInfo.CandidateAggregateNodes)
            this.TryProcessCandidate(candidateAggregateNode, groupAggregateVarInfo);
        }
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GroupByInto")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void TryProcessCandidate(
      KeyValuePair<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node> candidate,
      GroupAggregateVarInfo groupAggregateVarInfo)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node definingGroupNode = groupAggregateVarInfo.DefiningGroupNode;
      IList<System.Data.Entity.Core.Query.InternalTrees.Node> ancestors1;
      IList<System.Data.Entity.Core.Query.InternalTrees.Node> ancestors2;
      this.FindPathsToLeastCommonAncestor(candidate.Key, definingGroupNode, out ancestors1, out ancestors2);
      if (!AggregatePushdown.AreAllNodesSupportedForPropagation(ancestors2))
        return;
      GroupByIntoOp op1 = (GroupByIntoOp) definingGroupNode.Op;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op1.Inputs.Count == 1, "There should be one input var to GroupByInto at this stage");
      Var first = op1.Inputs.First;
      FunctionOp op2 = (FunctionOp) candidate.Key.Op;
      System.Data.Entity.Core.Query.InternalTrees.Node subTree = OpCopier.Copy(this.m_command, candidate.Value);
      new VarRemapper(this.m_command, new Dictionary<Var, Var>(1)
      {
        {
          groupAggregateVarInfo.GroupAggregateVar,
          first
        }
      }).RemapSubtree(subTree);
      Var computedVar;
      System.Data.Entity.Core.Query.InternalTrees.Node varDefNode = this.m_command.CreateVarDefNode(this.m_command.CreateNode((Op) this.m_command.CreateAggregateOp(op2.Function, false), subTree), out computedVar);
      definingGroupNode.Child2.Children.Add(varDefNode);
      ((GroupByBaseOp) definingGroupNode.Op).Outputs.Set(computedVar);
      for (int index = 0; index < ancestors2.Count; ++index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node = ancestors2[index];
        if (node.Op.OpType == OpType.Project)
          ((ProjectOp) node.Op).Outputs.Set(computedVar);
      }
      candidate.Key.Op = (Op) this.m_command.CreateVarRefOp(computedVar);
      candidate.Key.Children.Clear();
    }

    private static bool AreAllNodesSupportedForPropagation(IList<System.Data.Entity.Core.Query.InternalTrees.Node> nodes)
    {
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node node in (IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) nodes)
      {
        if (node.Op.OpType != OpType.Project && node.Op.OpType != OpType.Filter && node.Op.OpType != OpType.ConstrainedSort)
          return false;
      }
      return true;
    }

    private void FindPathsToLeastCommonAncestor(
      System.Data.Entity.Core.Query.InternalTrees.Node node1,
      System.Data.Entity.Core.Query.InternalTrees.Node node2,
      out IList<System.Data.Entity.Core.Query.InternalTrees.Node> ancestors1,
      out IList<System.Data.Entity.Core.Query.InternalTrees.Node> ancestors2)
    {
      ancestors1 = this.FindAncestors(node1);
      ancestors2 = this.FindAncestors(node2);
      int index1 = ancestors1.Count - 1;
      int index2;
      for (index2 = ancestors2.Count - 1; ancestors1[index1] == ancestors2[index2]; --index2)
        --index1;
      for (int index3 = ancestors1.Count - 1; index3 > index1; --index3)
        ancestors1.RemoveAt(index3);
      for (int index3 = ancestors2.Count - 1; index3 > index2; --index3)
        ancestors2.RemoveAt(index3);
    }

    private IList<System.Data.Entity.Core.Query.InternalTrees.Node> FindAncestors(System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      List<System.Data.Entity.Core.Query.InternalTrees.Node> nodeList = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      System.Data.Entity.Core.Query.InternalTrees.Node node1;
      for (System.Data.Entity.Core.Query.InternalTrees.Node key = node; this.m_tryGetParent(key, out node1); key = node1)
        nodeList.Add(node1);
      return (IList<System.Data.Entity.Core.Query.InternalTrees.Node>) nodeList;
    }
  }
}
