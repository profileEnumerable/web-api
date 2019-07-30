// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.GroupAggregateRefComputingVisitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class GroupAggregateRefComputingVisitor : BasicOpVisitor
  {
    private readonly GroupAggregateVarInfoManager _groupAggregateVarInfoManager = new GroupAggregateVarInfoManager();
    private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node> _childToParent = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node>();
    private readonly Command _command;

    internal static IEnumerable<GroupAggregateVarInfo> Process(
      Command itree,
      out TryGetValue tryGetParent)
    {
      GroupAggregateRefComputingVisitor computingVisitor = new GroupAggregateRefComputingVisitor(itree);
      computingVisitor.VisitNode(itree.Root);
      tryGetParent = new TryGetValue(computingVisitor._childToParent.TryGetValue);
      return computingVisitor._groupAggregateVarInfoManager.GroupAggregateVarInfos;
    }

    private GroupAggregateRefComputingVisitor(Command itree)
    {
      this._command = itree;
    }

    public override void Visit(VarDefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitDefault(n);
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = n.Child0;
      Op op1 = child0.Op;
      GroupAggregateVarInfo groupAggregateVarInfo;
      System.Data.Entity.Core.Query.InternalTrees.Node templateNode;
      bool isUnnested;
      if (GroupAggregateVarComputationTranslator.TryTranslateOverGroupAggregateVar(child0, true, this._command, this._groupAggregateVarInfoManager, out groupAggregateVarInfo, out templateNode, out isUnnested))
      {
        this._groupAggregateVarInfoManager.Add(op.Var, groupAggregateVarInfo, templateNode, isUnnested);
      }
      else
      {
        if (op1.OpType != OpType.NewRecord)
          return;
        NewRecordOp newRecordOp = (NewRecordOp) op1;
        for (int index = 0; index < child0.Children.Count; ++index)
        {
          if (GroupAggregateVarComputationTranslator.TryTranslateOverGroupAggregateVar(child0.Children[index], true, this._command, this._groupAggregateVarInfoManager, out groupAggregateVarInfo, out templateNode, out isUnnested))
            this._groupAggregateVarInfoManager.Add(op.Var, groupAggregateVarInfo, templateNode, isUnnested, (EdmMember) newRecordOp.Properties[index]);
        }
      }
    }

    public override void Visit(GroupByIntoOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitGroupByOp((GroupByBaseOp) op, n);
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Child3.Children)
      {
        Var var = ((VarDefOp) child.Op).Var;
        GroupAggregateVarRefInfo groupAggregateVarRefInfo;
        if (!this._groupAggregateVarInfoManager.TryGetReferencedGroupAggregateVarInfo(var, out groupAggregateVarRefInfo))
          this._groupAggregateVarInfoManager.Add(var, new GroupAggregateVarInfo(n, var), this._command.CreateNode((Op) this._command.CreateVarRefOp(var)), false);
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override void Visit(UnnestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitDefault(n);
      GroupAggregateVarRefInfo groupAggregateVarRefInfo;
      if (!this._groupAggregateVarInfoManager.TryGetReferencedGroupAggregateVarInfo(op.Var, out groupAggregateVarRefInfo))
        return;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.Table.Columns.Count == 1, "Expected one column before NTE");
      this._groupAggregateVarInfoManager.Add(op.Table.Columns[0], groupAggregateVarRefInfo.GroupAggregateVarInfo, groupAggregateVarRefInfo.Computation, true);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override void Visit(FunctionOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitDefault(n);
      if (!PlanCompilerUtil.IsCollectionAggregateFunction(op, n))
        return;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Children.Count == 1, "Aggregate Function must have one argument");
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = n.Child0;
      GroupAggregateVarInfo groupAggregateVarInfo;
      System.Data.Entity.Core.Query.InternalTrees.Node templateNode;
      bool isUnnested;
      if (!GroupAggregateVarComputationTranslator.TryTranslateOverGroupAggregateVar(n.Child0, false, this._command, this._groupAggregateVarInfoManager, out groupAggregateVarInfo, out templateNode, out isUnnested) || !isUnnested && !AggregatePushdownUtil.IsVarRefOverGivenVar(templateNode, groupAggregateVarInfo.GroupAggregateVar))
        return;
      groupAggregateVarInfo.CandidateAggregateNodes.Add(new KeyValuePair<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node>(n, templateNode));
    }

    protected override void VisitDefault(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (child.Op.Arity != 0)
          this._childToParent.Add(child, n);
      }
    }
  }
}
