// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.GroupAggregateVarComputationTranslator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class GroupAggregateVarComputationTranslator : BasicOpVisitorOfNode
  {
    private GroupAggregateVarInfo _targetGroupAggregateVarInfo;
    private bool _isUnnested;
    private readonly Command _command;
    private readonly GroupAggregateVarInfoManager _groupAggregateVarInfoManager;

    private GroupAggregateVarComputationTranslator(
      Command command,
      GroupAggregateVarInfoManager groupAggregateVarInfoManager)
    {
      this._command = command;
      this._groupAggregateVarInfoManager = groupAggregateVarInfoManager;
    }

    public static bool TryTranslateOverGroupAggregateVar(
      System.Data.Entity.Core.Query.InternalTrees.Node subtree,
      bool isVarDefinition,
      Command command,
      GroupAggregateVarInfoManager groupAggregateVarInfoManager,
      out GroupAggregateVarInfo groupAggregateVarInfo,
      out System.Data.Entity.Core.Query.InternalTrees.Node templateNode,
      out bool isUnnested)
    {
      GroupAggregateVarComputationTranslator computationTranslator = new GroupAggregateVarComputationTranslator(command, groupAggregateVarInfoManager);
      System.Data.Entity.Core.Query.InternalTrees.Node n = subtree;
      SoftCastOp softCastOp1 = (SoftCastOp) null;
      if (n.Op.OpType == OpType.SoftCast)
      {
        softCastOp1 = (SoftCastOp) n.Op;
        n = n.Child0;
      }
      bool flag;
      if (n.Op.OpType == OpType.Collect)
      {
        templateNode = computationTranslator.VisitCollect(n);
        flag = true;
      }
      else
      {
        templateNode = computationTranslator.VisitNode(n);
        flag = false;
      }
      groupAggregateVarInfo = computationTranslator._targetGroupAggregateVarInfo;
      isUnnested = computationTranslator._isUnnested;
      if (computationTranslator._targetGroupAggregateVarInfo == null || templateNode == null)
        return false;
      if (softCastOp1 != null)
      {
        SoftCastOp softCastOp2 = flag || !isVarDefinition && AggregatePushdownUtil.IsVarRefOverGivenVar(templateNode, computationTranslator._targetGroupAggregateVarInfo.GroupAggregateVar) ? command.CreateSoftCastOp(TypeHelpers.GetEdmType<CollectionType>(softCastOp1.Type).TypeUsage) : softCastOp1;
        templateNode = command.CreateNode((Op) softCastOp2, templateNode);
      }
      return true;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(VarRefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.TranslateOverGroupAggregateVar(op.Var, (EdmMember) null);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      PropertyOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (n.Child0.Op.OpType != OpType.VarRef)
        return base.Visit(op, n);
      return this.TranslateOverGroupAggregateVar(((VarRefOp) n.Child0.Op).Var, op.PropertyInfo);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node VisitCollect(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = n.Child0;
      Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> dictionary = new Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node>();
      while (child0.Child0.Op.OpType == OpType.Project)
      {
        child0 = child0.Child0;
        if (this.VisitDefault(child0.Child1) == null)
          return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
        foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in child0.Child1.Children)
        {
          if (GroupAggregateVarComputationTranslator.IsConstant(child.Child0))
            dictionary.Add(((VarDefOp) child.Op).Var, child.Child0);
        }
      }
      if (child0.Child0.Op.OpType != OpType.Unnest)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      GroupAggregateVarRefInfo groupAggregateVarRefInfo;
      if (!this._groupAggregateVarInfoManager.TryGetReferencedGroupAggregateVarInfo(((UnnestOp) child0.Child0.Op).Var, out groupAggregateVarRefInfo))
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      if (this._targetGroupAggregateVarInfo == null)
        this._targetGroupAggregateVarInfo = groupAggregateVarRefInfo.GroupAggregateVarInfo;
      else if (this._targetGroupAggregateVarInfo != groupAggregateVarRefInfo.GroupAggregateVarInfo)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      if (!this._isUnnested)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      PhysicalProjectOp op = (PhysicalProjectOp) n.Child0.Op;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op.Outputs.Count == 1, "Physical project should only have one output at this stage");
      Var output = op.Outputs[0];
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.TranslateOverGroupAggregateVar(output, (EdmMember) null);
      if (node1 != null)
      {
        this._isUnnested = true;
        return node1;
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node2;
      if (!dictionary.TryGetValue(output, out node2))
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      this._isUnnested = true;
      return node2;
    }

    private static bool IsConstant(System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = node;
      while (node1.Op.OpType == OpType.Cast)
        node1 = node1.Child0;
      return PlanCompilerUtil.IsConstantBaseOp(node1.Op.OpType);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node TranslateOverGroupAggregateVar(
      Var var,
      EdmMember property)
    {
      GroupAggregateVarRefInfo groupAggregateVarRefInfo;
      EdmMember prop;
      if (this._groupAggregateVarInfoManager.TryGetReferencedGroupAggregateVarInfo(var, out groupAggregateVarRefInfo))
      {
        prop = property;
      }
      else
      {
        if (!this._groupAggregateVarInfoManager.TryGetReferencedGroupAggregateVarInfo(var, property, out groupAggregateVarRefInfo))
          return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
        prop = (EdmMember) null;
      }
      if (this._targetGroupAggregateVarInfo == null)
      {
        this._targetGroupAggregateVarInfo = groupAggregateVarRefInfo.GroupAggregateVarInfo;
        this._isUnnested = groupAggregateVarRefInfo.IsUnnested;
      }
      else if (this._targetGroupAggregateVarInfo != groupAggregateVarRefInfo.GroupAggregateVarInfo || this._isUnnested != groupAggregateVarRefInfo.IsUnnested)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      System.Data.Entity.Core.Query.InternalTrees.Node node = groupAggregateVarRefInfo.Computation;
      if (prop != null)
        node = this._command.CreateNode((Op) this._command.CreatePropertyOp(prop), node);
      return node;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitDefault(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>(n.Children.Count);
      bool flag = false;
      for (int index = 0; index < n.Children.Count; ++index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node = this.VisitNode(n.Children[index]);
        if (node == null)
          return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
        if (!flag && !object.ReferenceEquals((object) n.Children[index], (object) node))
          flag = true;
        args.Add(node);
      }
      if (!flag)
        return n;
      return this._command.CreateNode(n.Op, args);
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitRelOpDefault(
      RelOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      AggregateOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(CollectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(ElementOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
    }
  }
}
