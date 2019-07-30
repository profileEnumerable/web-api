// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.NestPullup
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
  internal class NestPullup : BasicOpVisitorOfNode
  {
    private readonly Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> m_definingNodeMap = new Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node>();
    private readonly Dictionary<Var, Var> m_varRefMap = new Dictionary<Var, Var>();
    private readonly System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler m_compilerState;
    private readonly VarRemapper m_varRemapper;
    private bool m_foundSortUnderUnnest;

    private NestPullup(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState)
    {
      this.m_compilerState = compilerState;
      this.m_varRemapper = new VarRemapper(compilerState.Command);
    }

    internal static void Process(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState)
    {
      new NestPullup(compilerState).Process();
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "physicalProject")]
    private void Process()
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this.Command.Root.Op.OpType == OpType.PhysicalProject, "root node is not physicalProject?");
      this.Command.Root = this.VisitNode(this.Command.Root);
      if (!this.m_foundSortUnderUnnest)
        return;
      SortRemover.Process(this.Command);
    }

    private Command Command
    {
      get
      {
        return this.m_compilerState.Command;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "singleStreamNest")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static bool IsNestOpNode(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Op.OpType != OpType.SingleStreamNest, "illegal singleStreamNest?");
      if (n.Op.OpType != OpType.SingleStreamNest)
        return n.Op.OpType == OpType.MultiStreamNest;
      return true;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node NestingNotSupported(Op op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      this.m_varRemapper.RemapNode(n);
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (NestPullup.IsNestOpNode(child))
          throw new NotSupportedException(Strings.ADP_NestingNotSupported((object) op.OpType.ToString(), (object) child.Op.OpType.ToString()));
      }
      return n;
    }

    private Var ResolveVarReference(Var refVar)
    {
      Var key = refVar;
      while (this.m_varRefMap.TryGetValue(key, out key))
        refVar = key;
      return refVar;
    }

    private void UpdateReplacementVarMap(IEnumerable<Var> fromVars, IEnumerable<Var> toVars)
    {
      IEnumerator<Var> enumerator = toVars.GetEnumerator();
      foreach (Var fromVar in fromVars)
      {
        if (!enumerator.MoveNext())
          throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 2, (object) null);
        this.m_varRemapper.AddMapping(fromVar, enumerator.Current);
      }
      if (enumerator.MoveNext())
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 3, (object) null);
    }

    private static void RemapSortKeys(List<SortKey> sortKeys, Dictionary<Var, Var> varMap)
    {
      if (sortKeys == null)
        return;
      foreach (SortKey sortKey in sortKeys)
      {
        Var var;
        if (varMap.TryGetValue(sortKey.Var, out var))
          sortKey.Var = var;
      }
    }

    private static IEnumerable<Var> RemapVars(
      IEnumerable<Var> vars,
      Dictionary<Var, Var> varMap)
    {
      foreach (Var var in vars)
      {
        Var mappedVar;
        if (varMap.TryGetValue(var, out mappedVar))
          yield return mappedVar;
        else
          yield return var;
      }
    }

    private static VarList RemapVarList(VarList varList, Dictionary<Var, Var> varMap)
    {
      return Command.CreateVarList(NestPullup.RemapVars((IEnumerable<Var>) varList, varMap));
    }

    private VarVec RemapVarVec(VarVec varVec, Dictionary<Var, Var> varMap)
    {
      return this.Command.CreateVarVec(NestPullup.RemapVars((IEnumerable<Var>) varVec, varMap));
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(VarDefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      this.m_varRemapper.RemapNode(n);
      if (n.Child0.Op.OpType == OpType.VarRef)
        this.m_varRefMap.Add(op.Var, ((VarRefOp) n.Child0.Op).Var);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(VarRefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      this.m_varRemapper.RemapNode(n);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(CaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (child.Op.OpType == OpType.Collect)
          throw new NotSupportedException(Strings.ADP_NestingNotSupported((object) op.OpType.ToString(), (object) child.Op.OpType.ToString()));
        if (child.Op.OpType == OpType.VarRef && this.m_definingNodeMap.ContainsKey(((VarRefOp) child.Op).Var))
          throw new NotSupportedException(Strings.ADP_NestingNotSupported((object) op.OpType.ToString(), (object) child.Op.OpType.ToString()));
      }
      return this.VisitDefault(n);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NestPull")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ExistsOp")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(ExistsOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      Var first = ((ProjectOp) n.Child0.Op).Outputs.First;
      this.VisitChildren(n);
      VarVec outputs = ((ProjectOp) n.Child0.Op).Outputs;
      if (outputs.Count > 1)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(outputs.IsSet(first), "The constant var is not present after NestPull up over the input of ExistsOp.");
        outputs.Clear();
        outputs.Set(first);
      }
      return n;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitRelOpDefault(
      RelOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.NestingNotSupported((Op) op, n);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node ApplyOpJoinOp(Op op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      int num = 0;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (child.Op is NestBaseOp)
        {
          ++num;
          if (OpType.SingleStreamNest == child.Op.OpType)
            throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1012));
        }
      }
      if (num == 0)
        return n;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (op.OpType != OpType.MultiStreamNest && child.Op.IsRelOp)
        {
          KeyVec keyVec = this.Command.PullupKeys(child);
          if (keyVec == null || keyVec.NoKeys)
            throw new NotSupportedException(Strings.ADP_KeysRequiredForJoinOverNest((object) op.OpType.ToString()));
        }
      }
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args1 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args2 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (child.Op.OpType == OpType.MultiStreamNest)
        {
          collectionInfoList.AddRange((IEnumerable<CollectionInfo>) ((NestBaseOp) child.Op).CollectionInfo);
          if (op.OpType == OpType.FullOuterJoin || (op.OpType == OpType.LeftOuterJoin || op.OpType == OpType.OuterApply) && n.Child1.Op.OpType == OpType.MultiStreamNest)
          {
            Var constantVar = (Var) null;
            args2.Add(this.AugmentNodeWithConstant(child.Child0, (Func<ConstantBaseOp>) (() => (ConstantBaseOp) this.Command.CreateNullSentinelOp()), out constantVar));
            foreach (CollectionInfo collectionInfo in ((NestBaseOp) child.Op).CollectionInfo)
              this.m_definingNodeMap[collectionInfo.CollectionVar].Child0 = this.ApplyIsNotNullFilter(this.m_definingNodeMap[collectionInfo.CollectionVar].Child0, constantVar);
            for (int index = 1; index < child.Children.Count; ++index)
            {
              System.Data.Entity.Core.Query.InternalTrees.Node node = this.ApplyIsNotNullFilter(child.Children[index], constantVar);
              args1.Add(node);
            }
          }
          else
          {
            args2.Add(child.Child0);
            for (int index = 1; index < child.Children.Count; ++index)
              args1.Add(child.Children[index]);
          }
        }
        else
          args2.Add(child);
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.Command.CreateNode(op, args2);
      args1.Insert(0, node1);
      VarVec varVec = this.Command.CreateVarVec(node1.GetExtendedNodeInfo(this.Command).Definitions);
      foreach (CollectionInfo collectionInfo in collectionInfoList)
        varVec.Set(collectionInfo.CollectionVar);
      return this.Command.CreateNode((Op) this.Command.CreateMultiStreamNestOp(new List<SortKey>(), varVec, collectionInfoList), args1);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node ApplyIsNotNullFilter(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      Var sentinelVar)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node input = node;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      for (; input.Op.OpType == OpType.MultiStreamNest; input = input.Child0)
        node1 = input;
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.CapWithIsNotNullFilter(input, sentinelVar);
      System.Data.Entity.Core.Query.InternalTrees.Node node3;
      if (node1 != null)
      {
        node1.Child0 = node2;
        node3 = node;
      }
      else
        node3 = node2;
      return node3;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CapWithIsNotNullFilter(
      System.Data.Entity.Core.Query.InternalTrees.Node input,
      Var var)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.Command.CreateNode((Op) this.Command.CreateConditionalOp(OpType.Not), this.Command.CreateNode((Op) this.Command.CreateConditionalOp(OpType.IsNull), this.Command.CreateNode((Op) this.Command.CreateVarRefOp(var))));
      return this.Command.CreateNode((Op) this.Command.CreateFilterOp(), input, node);
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitApplyOp(
      ApplyBaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.ApplyOpJoinOp((Op) op, n);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DistinctOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.NestingNotSupported((Op) op, n);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(FilterOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      if (!(n.Child0.Op is NestBaseOp))
        return n;
      System.Data.Entity.Core.Query.InternalTrees.Node child0_1 = n.Child0;
      System.Data.Entity.Core.Query.InternalTrees.Node child0_2 = child0_1.Child0;
      n.Child0 = child0_2;
      child0_1.Child0 = n;
      this.Command.RecomputeNodeInfo(n);
      this.Command.RecomputeNodeInfo(child0_1);
      return child0_1;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(GroupByOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.NestingNotSupported((Op) op, n);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GroupByIntoOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      GroupByIntoOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.HasChild3 && n.Child3.Children.Count > 0, "GroupByIntoOp with no group aggregates?");
      System.Data.Entity.Core.Query.InternalTrees.Node child3 = n.Child3;
      VarVec varVec = this.Command.CreateVarVec(op.Outputs);
      VarVec outputs = op.Outputs;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in child3.Children)
      {
        VarDefOp op1 = child.Op as VarDefOp;
        outputs.Clear(op1.Var);
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.Command.CreateNode((Op) this.Command.CreateGroupByOp(op.Keys, outputs), n.Child0, n.Child1, n.Child2);
      return this.VisitNode(this.Command.CreateNode((Op) this.Command.CreateProjectOp(varVec), node, child3));
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitJoinOp(
      JoinBaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.ApplyOpJoinOp((Op) op, n);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(ProjectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      this.m_varRemapper.RemapNode(n);
      System.Data.Entity.Core.Query.InternalTrees.Node node;
      if (n.Child0.Op.OpType == OpType.Sort)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child0 = n.Child0;
        foreach (SortKey key in ((SortBaseOp) child0.Op).Keys)
        {
          if (!this.Command.GetExtendedNodeInfo(child0).ExternalReferences.IsSet(key.Var))
            op.Outputs.Set(key.Var);
        }
        n.Child0 = child0.Child0;
        this.Command.RecomputeNodeInfo(n);
        child0.Child0 = this.HandleProjectNode(n);
        this.Command.RecomputeNodeInfo(child0);
        node = child0;
      }
      else
        node = this.HandleProjectNode(n);
      return node;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node HandleProjectNode(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.ProjectOpCase1(n);
      if (node.Op.OpType == OpType.Project && NestPullup.IsNestOpNode(node.Child0))
        node = this.ProjectOpCase2(node);
      return this.MergeNestedNestOps(node);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Vars")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "collectionVar")]
    private System.Data.Entity.Core.Query.InternalTrees.Node MergeNestedNestOps(System.Data.Entity.Core.Query.InternalTrees.Node nestNode)
    {
      if (!NestPullup.IsNestOpNode(nestNode) || !NestPullup.IsNestOpNode(nestNode.Child0))
        return nestNode;
      NestBaseOp op1 = (NestBaseOp) nestNode.Op;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = nestNode.Child0;
      NestBaseOp op2 = (NestBaseOp) child0.Op;
      VarVec varVec1 = this.Command.CreateVarVec();
      foreach (CollectionInfo collectionInfo in op1.CollectionInfo)
        varVec1.Set(collectionInfo.CollectionVar);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
      VarVec varVec2 = this.Command.CreateVarVec(op1.Outputs);
      args.Add(child0.Child0);
      for (int index = 1; index < child0.Children.Count; ++index)
      {
        CollectionInfo collectionInfo = op2.CollectionInfo[index - 1];
        if (varVec1.IsSet(collectionInfo.CollectionVar) || varVec2.IsSet(collectionInfo.CollectionVar))
        {
          collectionInfoList.Add(collectionInfo);
          args.Add(child0.Children[index]);
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(varVec2.IsSet(collectionInfo.CollectionVar), "collectionVar not in output Vars?");
        }
      }
      for (int index = 1; index < nestNode.Children.Count; ++index)
      {
        CollectionInfo collectionInfo = op1.CollectionInfo[index - 1];
        collectionInfoList.Add(collectionInfo);
        args.Add(nestNode.Children[index]);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(varVec2.IsSet(collectionInfo.CollectionVar), "collectionVar not in output Vars?");
      }
      List<SortKey> prefixSortKeys = this.ConsolidateSortKeys(op1.PrefixSortKeys, op2.PrefixSortKeys);
      foreach (SortKey sortKey in prefixSortKeys)
        varVec2.Set(sortKey.Var);
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.Command.CreateNode((Op) this.Command.CreateMultiStreamNestOp(prefixSortKeys, varVec2, collectionInfoList), args);
      this.Command.RecomputeNodeInfo(node);
      return node;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "physicalProject")]
    private System.Data.Entity.Core.Query.InternalTrees.Node ProjectOpCase1(System.Data.Entity.Core.Query.InternalTrees.Node projectNode)
    {
      ProjectOp op1 = (ProjectOp) projectNode.Op;
      List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args1 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> nodeList1 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      VarVec varVec1 = this.Command.CreateVarVec();
      VarVec varVec2 = this.Command.CreateVarVec();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> nodeList2 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> nodeList3 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in projectNode.Child1.Children)
      {
        VarDefOp op2 = (VarDefOp) child.Op;
        System.Data.Entity.Core.Query.InternalTrees.Node child0_1 = child.Child0;
        if (OpType.Collect == child0_1.Op.OpType)
        {
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(child0_1.HasChild0, "collect without input?");
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(OpType.PhysicalProject == child0_1.Child0.Op.OpType, "collect without physicalProject?");
          System.Data.Entity.Core.Query.InternalTrees.Node child0_2 = child0_1.Child0;
          this.m_definingNodeMap.Add(op2.Var, child0_2);
          this.ConvertToNestOpInput(child0_2, op2.Var, collectionInfoList, nodeList1, varVec1, varVec2);
        }
        else if (OpType.VarRef == child0_1.Op.OpType)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node node;
          if (this.m_definingNodeMap.TryGetValue(((VarRefOp) child0_1.Op).Var, out node))
          {
            node = this.CopyCollectionVarDefinition(node);
            this.m_definingNodeMap.Add(op2.Var, node);
            this.ConvertToNestOpInput(node, op2.Var, collectionInfoList, nodeList1, varVec1, varVec2);
          }
          else
          {
            nodeList3.Add(child);
            args1.Add(child);
          }
        }
        else
        {
          nodeList2.Add(child);
          args1.Add(child);
        }
      }
      if (nodeList1.Count == 0)
        return projectNode;
      VarVec varVec3 = this.Command.CreateVarVec(op1.Outputs);
      VarVec varVec4 = this.Command.CreateVarVec(op1.Outputs);
      varVec4.Minus(varVec2);
      varVec4.Or(varVec1);
      if (!varVec4.IsEmpty)
      {
        if (NestPullup.IsNestOpNode(projectNode.Child0))
        {
          if (nodeList2.Count == 0 && nodeList3.Count == 0)
          {
            projectNode = projectNode.Child0;
            this.EnsureReferencedVarsAreRemoved(nodeList3, varVec3);
          }
          else
          {
            NestBaseOp op2 = (NestBaseOp) projectNode.Child0.Op;
            List<System.Data.Entity.Core.Query.InternalTrees.Node> args2 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
            args2.Add(projectNode.Child0.Child0);
            nodeList3.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) nodeList2);
            args2.Add(this.Command.CreateNode((Op) this.Command.CreateVarDefListOp(), nodeList3));
            VarVec varVec5 = this.Command.CreateVarVec(op2.Outputs);
            foreach (CollectionInfo collectionInfo in op2.CollectionInfo)
              varVec5.Clear(collectionInfo.CollectionVar);
            foreach (System.Data.Entity.Core.Query.InternalTrees.Node node in nodeList3)
              varVec5.Set(((VarDefOp) node.Op).Var);
            System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.Command.CreateNode((Op) this.Command.CreateProjectOp(varVec5), args2);
            VarVec varVec6 = this.Command.CreateVarVec(varVec5);
            varVec6.Or(op2.Outputs);
            MultiStreamNestOp multiStreamNestOp = this.Command.CreateMultiStreamNestOp(op2.PrefixSortKeys, varVec6, op2.CollectionInfo);
            List<System.Data.Entity.Core.Query.InternalTrees.Node> args3 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
            args3.Add(node1);
            for (int index = 1; index < projectNode.Child0.Children.Count; ++index)
              args3.Add(projectNode.Child0.Children[index]);
            projectNode = this.Command.CreateNode((Op) multiStreamNestOp, args3);
          }
        }
        else
        {
          ProjectOp projectOp = this.Command.CreateProjectOp(varVec4);
          projectNode.Child1 = this.Command.CreateNode(projectNode.Child1.Op, args1);
          projectNode.Op = (Op) projectOp;
          this.EnsureReferencedVarsAreRemapped(nodeList3);
        }
      }
      else
      {
        projectNode = projectNode.Child0;
        this.EnsureReferencedVarsAreRemoved(nodeList3, varVec3);
      }
      varVec1.And(projectNode.GetExtendedNodeInfo(this.Command).Definitions);
      varVec3.Or(varVec1);
      MultiStreamNestOp multiStreamNestOp1 = this.Command.CreateMultiStreamNestOp(new List<SortKey>(), varVec3, collectionInfoList);
      nodeList1.Insert(0, projectNode);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.Command.CreateNode((Op) multiStreamNestOp1, nodeList1);
      this.Command.RecomputeNodeInfo(projectNode);
      this.Command.RecomputeNodeInfo(node2);
      return node2;
    }

    private void EnsureReferencedVarsAreRemoved(List<System.Data.Entity.Core.Query.InternalTrees.Node> referencedVars, VarVec outputVars)
    {
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node referencedVar in referencedVars)
      {
        Var var1 = ((VarDefOp) referencedVar.Op).Var;
        Var var2 = this.ResolveVarReference(var1);
        this.m_varRemapper.AddMapping(var1, var2);
        outputVars.Clear(var1);
        outputVars.Set(var2);
      }
    }

    private void EnsureReferencedVarsAreRemapped(List<System.Data.Entity.Core.Query.InternalTrees.Node> referencedVars)
    {
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node referencedVar in referencedVars)
      {
        Var var = ((VarDefOp) referencedVar.Op).Var;
        this.m_varRemapper.AddMapping(this.ResolveVarReference(var), var);
      }
    }

    private void ConvertToNestOpInput(
      System.Data.Entity.Core.Query.InternalTrees.Node physicalProjectNode,
      Var collectionVar,
      List<CollectionInfo> collectionInfoList,
      List<System.Data.Entity.Core.Query.InternalTrees.Node> collectionNodes,
      VarVec externalReferences,
      VarVec collectionReferences)
    {
      externalReferences.Or(this.Command.GetNodeInfo(physicalProjectNode).ExternalReferences);
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = physicalProjectNode.Child0;
      PhysicalProjectOp op = (PhysicalProjectOp) physicalProjectNode.Op;
      VarList varList = Command.CreateVarList((IEnumerable<Var>) op.Outputs);
      VarVec varVec1 = this.Command.CreateVarVec((IEnumerable<Var>) varList);
      List<SortKey> sortKeys;
      if (OpType.Sort == child0.Op.OpType)
      {
        sortKeys = OpCopier.Copy(this.Command, ((SortBaseOp) child0.Op).Keys);
        foreach (SortKey sortKey in sortKeys)
        {
          if (!varVec1.IsSet(sortKey.Var))
          {
            varList.Add(sortKey.Var);
            varVec1.Set(sortKey.Var);
          }
        }
      }
      else
        sortKeys = new List<SortKey>();
      VarVec keyVars = this.Command.GetExtendedNodeInfo(child0).Keys.KeyVars;
      VarVec varVec2 = keyVars.Clone();
      varVec2.Minus(varVec1);
      VarVec keys = varVec2.IsEmpty ? keyVars.Clone() : this.Command.CreateVarVec();
      CollectionInfo collectionInfo = Command.CreateCollectionInfo(collectionVar, op.ColumnMap.Element, varList, keys, sortKeys, (object) null);
      collectionInfoList.Add(collectionInfo);
      collectionNodes.Add(child0);
      collectionReferences.Set(collectionVar);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node ProjectOpCase2(System.Data.Entity.Core.Query.InternalTrees.Node projectNode)
    {
      ProjectOp op1 = (ProjectOp) projectNode.Op;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = projectNode.Child0;
      NestBaseOp op2 = child0.Op as NestBaseOp;
      VarVec varVec1 = this.Command.CreateVarVec();
      foreach (CollectionInfo collectionInfo in op2.CollectionInfo)
        varVec1.Set(collectionInfo.CollectionVar);
      VarVec varVec2 = this.Command.CreateVarVec(op2.Outputs);
      varVec2.Minus(varVec1);
      VarVec varVec3 = this.Command.CreateVarVec(op1.Outputs);
      varVec3.Minus(varVec1);
      VarVec varVec4 = this.Command.CreateVarVec(op1.Outputs);
      varVec4.Minus(varVec3);
      VarVec varVec5 = this.Command.CreateVarVec(varVec1);
      varVec5.Minus(varVec4);
      List<CollectionInfo> collectionInfoList;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args1;
      if (varVec5.IsEmpty)
      {
        collectionInfoList = op2.CollectionInfo;
        args1 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) child0.Children);
      }
      else
      {
        collectionInfoList = new List<CollectionInfo>();
        args1 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
        args1.Add(child0.Child0);
        int index = 1;
        foreach (CollectionInfo collectionInfo in op2.CollectionInfo)
        {
          if (!varVec5.IsSet(collectionInfo.CollectionVar))
          {
            collectionInfoList.Add(collectionInfo);
            args1.Add(child0.Children[index]);
          }
          ++index;
        }
      }
      VarVec varVec6 = this.Command.CreateVarVec();
      for (int index = 1; index < child0.Children.Count; ++index)
        varVec6.Or(child0.Children[index].GetExtendedNodeInfo(this.Command).ExternalReferences);
      varVec6.And(child0.Child0.GetExtendedNodeInfo(this.Command).Definitions);
      VarVec varVec7 = this.Command.CreateVarVec(varVec3);
      varVec7.Or(varVec2);
      varVec7.Or(varVec6);
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args2 = new List<System.Data.Entity.Core.Query.InternalTrees.Node>(projectNode.Child1.Children.Count);
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in projectNode.Child1.Children)
      {
        VarDefOp op3 = (VarDefOp) child.Op;
        if (varVec7.IsSet(op3.Var))
          args2.Add(child);
      }
      if (collectionInfoList.Count != 0 && varVec7.IsEmpty)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(args2.Count == 0, "outputs is empty with non-zero count of children?");
        Var computedVar;
        System.Data.Entity.Core.Query.InternalTrees.Node varDefNode = this.Command.CreateVarDefNode(this.Command.CreateNode((Op) this.Command.CreateNullOp(this.Command.StringType)), out computedVar);
        args2.Add(varDefNode);
        varVec7.Set(computedVar);
      }
      projectNode.Op = (Op) this.Command.CreateProjectOp(this.Command.CreateVarVec(varVec7));
      projectNode.Child1 = this.Command.CreateNode(projectNode.Child1.Op, args2);
      System.Data.Entity.Core.Query.InternalTrees.Node n;
      if (collectionInfoList.Count == 0)
      {
        projectNode.Child0 = child0.Child0;
        n = projectNode;
      }
      else
      {
        VarVec varVec8 = this.Command.CreateVarVec(op1.Outputs);
        for (int index = 1; index < args1.Count; ++index)
          varVec8.Or(args1[index].GetNodeInfo(this.Command).ExternalReferences);
        foreach (SortKey prefixSortKey in op2.PrefixSortKeys)
          varVec8.Set(prefixSortKey.Var);
        child0.Op = (Op) this.Command.CreateMultiStreamNestOp(op2.PrefixSortKeys, varVec8, collectionInfoList);
        n = this.Command.CreateNode(child0.Op, args1);
        projectNode.Child0 = n.Child0;
        n.Child0 = projectNode;
        this.Command.RecomputeNodeInfo(projectNode);
      }
      this.Command.RecomputeNodeInfo(n);
      return n;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitSetOp(
      SetOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.NestingNotSupported((Op) op, n);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      SingleRowOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      if (NestPullup.IsNestOpNode(n.Child0))
      {
        n = n.Child0;
        System.Data.Entity.Core.Query.InternalTrees.Node node = this.Command.CreateNode((Op) op, n.Child0);
        n.Child0 = node;
        this.Command.RecomputeNodeInfo(n);
      }
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(SortOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      this.m_varRemapper.RemapNode(n);
      NestBaseOp op1 = n.Child0.Op as NestBaseOp;
      if (op1 == null)
        return n;
      n.Child0.Op = (Op) this.GetNestOpWithConsolidatedSortKeys(op1, op.Keys);
      return n.Child0;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      ConstrainedSortOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      NestBaseOp op1 = n.Child0.Op as NestBaseOp;
      if (op1 != null)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child0 = n.Child0;
        n.Child0 = child0.Child0;
        child0.Child0 = n;
        child0.Op = (Op) this.GetNestOpWithConsolidatedSortKeys(op1, op.Keys);
        n = child0;
      }
      return n;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SingleStreamNestOp")]
    private NestBaseOp GetNestOpWithConsolidatedSortKeys(
      NestBaseOp inputNestOp,
      List<SortKey> sortKeys)
    {
      NestBaseOp nestBaseOp;
      if (inputNestOp.PrefixSortKeys.Count == 0)
      {
        foreach (SortKey sortKey in sortKeys)
          inputNestOp.PrefixSortKeys.Add(Command.CreateSortKey(sortKey.Var, sortKey.AscendingSort, sortKey.Collation));
        nestBaseOp = inputNestOp;
      }
      else
      {
        this.Command.CreateVarVec();
        List<SortKey> prefixSortKeys = this.ConsolidateSortKeys(sortKeys, inputNestOp.PrefixSortKeys);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(inputNestOp is MultiStreamNestOp, "Unexpected SingleStreamNestOp?");
        nestBaseOp = (NestBaseOp) this.Command.CreateMultiStreamNestOp(prefixSortKeys, inputNestOp.Outputs, inputNestOp.CollectionInfo);
      }
      return nestBaseOp;
    }

    private List<SortKey> ConsolidateSortKeys(
      List<SortKey> sortKeyList1,
      List<SortKey> sortKeyList2)
    {
      VarVec varVec = this.Command.CreateVarVec();
      List<SortKey> sortKeyList = new List<SortKey>();
      foreach (SortKey sortKey in sortKeyList1)
      {
        if (!varVec.IsSet(sortKey.Var))
        {
          varVec.Set(sortKey.Var);
          sortKeyList.Add(Command.CreateSortKey(sortKey.Var, sortKey.AscendingSort, sortKey.Collation));
        }
      }
      foreach (SortKey sortKey in sortKeyList2)
      {
        if (!varVec.IsSet(sortKey.Var))
        {
          varVec.Set(sortKey.Var);
          sortKeyList.Add(Command.CreateSortKey(sortKey.Var, sortKey.AscendingSort, sortKey.Collation));
        }
      }
      return sortKeyList;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "physicalProject")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDef")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(UnnestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      TypeHelpers.GetEdmType<CollectionType>(op.Var.Type);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Child0.Op.OpType == OpType.VarDef, "Un-nest without VarDef input?");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(((VarDefOp) n.Child0.Op).Var == op.Var, "Un-nest var not found?");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Child0.HasChild0, "VarDef without input?");
      System.Data.Entity.Core.Query.InternalTrees.Node child0_1 = n.Child0.Child0;
      if (OpType.Function == child0_1.Op.OpType)
        return n;
      System.Data.Entity.Core.Query.InternalTrees.Node node;
      if (OpType.Collect == child0_1.Op.OpType)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(child0_1.HasChild0, "collect without input?");
        node = child0_1.Child0;
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node.Op.OpType == OpType.PhysicalProject, "collect without physicalProject?");
        this.m_definingNodeMap.Add(op.Var, node);
      }
      else
      {
        if (OpType.VarRef != child0_1.Op.OpType)
          throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.InvalidInternalTree, 2, (object) child0_1.Op.OpType);
        System.Data.Entity.Core.Query.InternalTrees.Node refVarDefiningNode;
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this.m_definingNodeMap.TryGetValue(((VarRefOp) child0_1.Op).Var, out refVarDefiningNode), "Could not find a definition for a referenced collection var");
        node = this.CopyCollectionVarDefinition(refVarDefiningNode);
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node.Op.OpType == OpType.PhysicalProject, "driving node is not physicalProject?");
      }
      IEnumerable<Var> outputs = (IEnumerable<Var>) ((PhysicalProjectOp) node.Op).Outputs;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node.HasChild0, "physicalProject without input?");
      System.Data.Entity.Core.Query.InternalTrees.Node child0_2 = node.Child0;
      if (child0_2.Op.OpType == OpType.Sort)
        this.m_foundSortUnderUnnest = true;
      this.UpdateReplacementVarMap((IEnumerable<Var>) op.Table.Columns, outputs);
      return child0_2;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CopyCollectionVarDefinition(
      System.Data.Entity.Core.Query.InternalTrees.Node refVarDefiningNode)
    {
      VarMap varMap;
      Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> newCollectionVarDefinitions;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = OpCopierTrackingCollectionVars.Copy(this.Command, refVarDefiningNode, out varMap, out newCollectionVarDefinitions);
      if (newCollectionVarDefinitions.Count != 0)
      {
        VarMap reverseMap = varMap.GetReverseMap();
        foreach (KeyValuePair<Var, System.Data.Entity.Core.Query.InternalTrees.Node> keyValuePair in newCollectionVarDefinitions)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node node2;
          if (this.m_definingNodeMap.TryGetValue(reverseMap[keyValuePair.Key], out node2))
          {
            PhysicalProjectOp op = (PhysicalProjectOp) node2.Op;
            System.Data.Entity.Core.Query.InternalTrees.Node node3 = this.Command.CreateNode((Op) this.Command.CreatePhysicalProjectOp(VarRemapper.RemapVarList(this.Command, (Dictionary<Var, Var>) varMap, op.Outputs), (SimpleCollectionColumnMap) ColumnMapCopier.Copy((ColumnMap) op.ColumnMap, varMap)), keyValuePair.Value);
            this.m_definingNodeMap.Add(keyValuePair.Key, node3);
          }
        }
      }
      return node1;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitNestOp(
      NestBaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (NestPullup.IsNestOpNode(child))
          throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1002));
      }
      return n;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "physicalProject")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      PhysicalProjectOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(n.Children.Count == 1, "multiple inputs to physicalProject?");
      this.VisitChildren(n);
      this.m_varRemapper.RemapNode(n);
      if (n != this.Command.Root || !NestPullup.IsNestOpNode(n.Child0))
        return n;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = n.Child0;
      Dictionary<Var, ColumnMap> dictionary = new Dictionary<Var, ColumnMap>();
      VarList varList = Command.CreateVarList(op.Outputs.Where<Var>((Func<Var, bool>) (v => v.VarType == VarType.Parameter)));
      SimpleColumnMap[] parentKeyColumnMaps;
      System.Data.Entity.Core.Query.InternalTrees.Node singleStreamNest = this.ConvertToSingleStreamNest(child0, dictionary, varList, out parentKeyColumnMaps);
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.BuildSortForNestElimination((SingleStreamNestOp) singleStreamNest.Op, singleStreamNest);
      SimpleCollectionColumnMap collectionColumnMap = (SimpleCollectionColumnMap) ColumnMapTranslator.Translate((ColumnMap) ((PhysicalProjectOp) n.Op).ColumnMap, dictionary);
      SimpleCollectionColumnMap columnMap = new SimpleCollectionColumnMap(collectionColumnMap.Type, collectionColumnMap.Name, collectionColumnMap.Element, parentKeyColumnMaps, (SimpleColumnMap[]) null);
      n.Op = (Op) this.Command.CreatePhysicalProjectOp(varList, columnMap);
      n.Child0 = node;
      return n;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node BuildSortForNestElimination(
      SingleStreamNestOp ssnOp,
      System.Data.Entity.Core.Query.InternalTrees.Node nestNode)
    {
      List<SortKey> sortKeys = this.BuildSortKeyList(ssnOp);
      return sortKeys.Count <= 0 ? nestNode.Child0 : this.Command.CreateNode((Op) this.Command.CreateSortOp(sortKeys), nestNode.Child0);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private List<SortKey> BuildSortKeyList(SingleStreamNestOp ssnOp)
    {
      VarVec varVec = this.Command.CreateVarVec();
      List<SortKey> sortKeyList = new List<SortKey>();
      foreach (SortKey prefixSortKey in ssnOp.PrefixSortKeys)
      {
        if (!varVec.IsSet(prefixSortKey.Var))
        {
          varVec.Set(prefixSortKey.Var);
          sortKeyList.Add(prefixSortKey);
        }
      }
      foreach (Var key in ssnOp.Keys)
      {
        if (!varVec.IsSet(key))
        {
          varVec.Set(key);
          SortKey sortKey = Command.CreateSortKey(key);
          sortKeyList.Add(sortKey);
        }
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!varVec.IsSet(ssnOp.Discriminator), "prefix sort on discriminator?");
      sortKeyList.Add(Command.CreateSortKey(ssnOp.Discriminator));
      foreach (SortKey postfixSortKey in ssnOp.PostfixSortKeys)
      {
        if (!varVec.IsSet(postfixSortKey.Var))
        {
          varVec.Set(postfixSortKey.Var);
          sortKeyList.Add(postfixSortKey);
        }
      }
      return sortKeyList;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node ConvertToSingleStreamNest(
      System.Data.Entity.Core.Query.InternalTrees.Node nestNode,
      Dictionary<Var, ColumnMap> varRefReplacementMap,
      VarList flattenedOutputVarList,
      out SimpleColumnMap[] parentKeyColumnMaps)
    {
      MultiStreamNestOp op = (MultiStreamNestOp) nestNode.Op;
      for (int index = 1; index < nestNode.Children.Count; ++index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child = nestNode.Children[index];
        if (child.Op.OpType == OpType.MultiStreamNest)
        {
          CollectionInfo collectionInfo = op.CollectionInfo[index - 1];
          VarList varList = Command.CreateVarList();
          SimpleColumnMap[] parentKeyColumnMaps1;
          nestNode.Children[index] = this.ConvertToSingleStreamNest(child, varRefReplacementMap, varList, out parentKeyColumnMaps1);
          ColumnMap columnMap = ColumnMapTranslator.Translate(collectionInfo.ColumnMap, varRefReplacementMap);
          VarVec varVec = this.Command.CreateVarVec(((SingleStreamNestOp) nestNode.Children[index].Op).Keys);
          op.CollectionInfo[index - 1] = Command.CreateCollectionInfo(collectionInfo.CollectionVar, columnMap, varList, varVec, collectionInfo.SortKeys, (object) null);
        }
      }
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = nestNode.Child0;
      KeyVec keyVec = this.Command.PullupKeys(child0);
      if (keyVec.NoKeys)
        throw new NotSupportedException(Strings.ADP_KeysRequiredForNesting);
      VarList varList1 = Command.CreateVarList((IEnumerable<Var>) this.Command.GetExtendedNodeInfo(child0).Definitions);
      VarList discriminatorVarList;
      List<List<SortKey>> sortKeys1;
      this.NormalizeNestOpInputs((NestBaseOp) op, nestNode, out discriminatorVarList, out sortKeys1);
      Var discriminatorVar;
      List<Dictionary<Var, Var>> varMapList;
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.BuildUnionAllSubqueryForNestOp((NestBaseOp) op, nestNode, varList1, discriminatorVarList, out discriminatorVar, out varMapList);
      Dictionary<Var, Var> varMap = varMapList[0];
      flattenedOutputVarList.AddRange(NestPullup.RemapVars((IEnumerable<Var>) varList1, varMap));
      VarVec varVec1 = this.Command.CreateVarVec((IEnumerable<Var>) flattenedOutputVarList);
      VarVec varVec2 = this.Command.CreateVarVec(varVec1);
      foreach (KeyValuePair<Var, Var> keyValuePair in varMap)
      {
        if (keyValuePair.Key != keyValuePair.Value)
          varRefReplacementMap[keyValuePair.Key] = (ColumnMap) new VarRefColumnMap(keyValuePair.Value);
      }
      NestPullup.RemapSortKeys(op.PrefixSortKeys, varMap);
      List<SortKey> postfixSortKeys = new List<SortKey>();
      List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
      VarRefColumnMap varRefColumnMap = new VarRefColumnMap(discriminatorVar);
      varVec2.Set(discriminatorVar);
      if (!varVec1.IsSet(discriminatorVar))
      {
        flattenedOutputVarList.Add(discriminatorVar);
        varVec1.Set(discriminatorVar);
      }
      VarVec keys1 = this.RemapVarVec(keyVec.KeyVars, varMap);
      parentKeyColumnMaps = new SimpleColumnMap[keys1.Count];
      int index1 = 0;
      foreach (Var v in keys1)
      {
        parentKeyColumnMaps[index1] = (SimpleColumnMap) new VarRefColumnMap(v);
        ++index1;
        if (!varVec1.IsSet(v))
        {
          flattenedOutputVarList.Add(v);
          varVec1.Set(v);
        }
      }
      for (int index2 = 1; index2 < nestNode.Children.Count; ++index2)
      {
        CollectionInfo collectionInfo1 = op.CollectionInfo[index2 - 1];
        List<SortKey> sortKeys2 = sortKeys1[index2];
        NestPullup.RemapSortKeys(sortKeys2, varMapList[index2]);
        postfixSortKeys.AddRange((IEnumerable<SortKey>) sortKeys2);
        ColumnMap columnMap = ColumnMapTranslator.Translate(collectionInfo1.ColumnMap, varMapList[index2]);
        VarList flattenedElementVars = NestPullup.RemapVarList(collectionInfo1.FlattenedElementVars, varMapList[index2]);
        VarVec keys2 = this.RemapVarVec(collectionInfo1.Keys, varMapList[index2]);
        NestPullup.RemapSortKeys(collectionInfo1.SortKeys, varMapList[index2]);
        CollectionInfo collectionInfo2 = Command.CreateCollectionInfo(collectionInfo1.CollectionVar, columnMap, flattenedElementVars, keys2, collectionInfo1.SortKeys, (object) index2);
        collectionInfoList.Add(collectionInfo2);
        foreach (Var v in (List<Var>) flattenedElementVars)
        {
          if (!varVec1.IsSet(v))
          {
            flattenedOutputVarList.Add(v);
            varVec1.Set(v);
          }
        }
        varVec2.Set(collectionInfo1.CollectionVar);
        int index3 = 0;
        SimpleColumnMap[] keys3 = new SimpleColumnMap[collectionInfo2.Keys.Count];
        foreach (Var key in collectionInfo2.Keys)
        {
          keys3[index3] = (SimpleColumnMap) new VarRefColumnMap(key);
          ++index3;
        }
        DiscriminatedCollectionColumnMap collectionColumnMap = new DiscriminatedCollectionColumnMap(TypeUtils.CreateCollectionType(collectionInfo2.ColumnMap.Type), collectionInfo2.ColumnMap.Name, collectionInfo2.ColumnMap, keys3, parentKeyColumnMaps, (SimpleColumnMap) varRefColumnMap, collectionInfo2.DiscriminatorValue);
        varRefReplacementMap[collectionInfo1.CollectionVar] = (ColumnMap) collectionColumnMap;
      }
      return this.Command.CreateNode((Op) this.Command.CreateSingleStreamNestOp(keys1, op.PrefixSortKeys, postfixSortKeys, varVec2, collectionInfoList, discriminatorVar), node);
    }

    private void NormalizeNestOpInputs(
      NestBaseOp nestOp,
      System.Data.Entity.Core.Query.InternalTrees.Node nestNode,
      out VarList discriminatorVarList,
      out List<List<SortKey>> sortKeys)
    {
      discriminatorVarList = Command.CreateVarList();
      discriminatorVarList.Add((Var) null);
      sortKeys = new List<List<SortKey>>();
      sortKeys.Add(nestOp.PrefixSortKeys);
      for (int index = 1; index < nestNode.Children.Count; ++index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node input = nestNode.Children[index];
        SingleStreamNestOp op1 = input.Op as SingleStreamNestOp;
        if (op1 != null)
        {
          List<SortKey> sortKeyList = this.BuildSortKeyList(op1);
          sortKeys.Add(sortKeyList);
          input = input.Child0;
        }
        else
        {
          SortOp op2 = input.Op as SortOp;
          if (op2 != null)
          {
            input = input.Child0;
            sortKeys.Add(op2.Keys);
          }
          else
            sortKeys.Add(new List<SortKey>());
        }
        VarList flattenedElementVars = nestOp.CollectionInfo[index - 1].FlattenedElementVars;
        foreach (SortKey sortKey in sortKeys[index])
        {
          if (!flattenedElementVars.Contains(sortKey.Var))
            flattenedElementVars.Add(sortKey.Var);
        }
        Var internalConstantVar;
        System.Data.Entity.Core.Query.InternalTrees.Node node = this.AugmentNodeWithInternalIntegerConstant(input, index, out internalConstantVar);
        nestNode.Children[index] = node;
        discriminatorVarList.Add(internalConstantVar);
      }
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node AugmentNodeWithInternalIntegerConstant(
      System.Data.Entity.Core.Query.InternalTrees.Node input,
      int value,
      out Var internalConstantVar)
    {
      return this.AugmentNodeWithConstant(input, (Func<ConstantBaseOp>) (() => (ConstantBaseOp) this.Command.CreateInternalConstantOp(this.Command.IntegerType, (object) value)), out internalConstantVar);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node AugmentNodeWithConstant(
      System.Data.Entity.Core.Query.InternalTrees.Node input,
      Func<ConstantBaseOp> createOp,
      out Var constantVar)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = this.Command.CreateVarDefListNode(this.Command.CreateNode((Op) createOp()), out constantVar);
      VarVec varVec = this.Command.CreateVarVec(this.Command.GetExtendedNodeInfo(input).Definitions);
      varVec.Set(constantVar);
      return this.Command.CreateNode((Op) this.Command.CreateProjectOp(varVec), input, varDefListNode);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node BuildUnionAllSubqueryForNestOp(
      NestBaseOp nestOp,
      System.Data.Entity.Core.Query.InternalTrees.Node nestNode,
      VarList drivingNodeVars,
      VarList discriminatorVarList,
      out Var discriminatorVar,
      out List<Dictionary<Var, Var>> varMapList)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = nestNode.Child0;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      VarList leftVars = (VarList) null;
      for (int index1 = 1; index1 < nestNode.Children.Count; ++index1)
      {
        VarList newVarList;
        System.Data.Entity.Core.Query.InternalTrees.Node node2;
        VarList varList1;
        Op op;
        if (index1 > 1)
        {
          node2 = OpCopier.Copy(this.Command, child0, drivingNodeVars, out newVarList);
          VarRemapper varRemapper = new VarRemapper(this.Command);
          for (int index2 = 0; index2 < drivingNodeVars.Count; ++index2)
            varRemapper.AddMapping(drivingNodeVars[index2], newVarList[index2]);
          varRemapper.RemapSubtree(nestNode.Children[index1]);
          varList1 = varRemapper.RemapVarList(nestOp.CollectionInfo[index1 - 1].FlattenedElementVars);
          op = (Op) this.Command.CreateCrossApplyOp();
        }
        else
        {
          node2 = child0;
          newVarList = drivingNodeVars;
          varList1 = nestOp.CollectionInfo[index1 - 1].FlattenedElementVars;
          op = (Op) this.Command.CreateOuterApplyOp();
        }
        System.Data.Entity.Core.Query.InternalTrees.Node node3 = this.Command.CreateNode(op, node2, nestNode.Children[index1]);
        List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
        VarList varList2 = Command.CreateVarList();
        varList2.Add(discriminatorVarList[index1]);
        varList2.AddRange((IEnumerable<Var>) newVarList);
        for (int index2 = 1; index2 < nestNode.Children.Count; ++index2)
        {
          CollectionInfo collectionInfo = nestOp.CollectionInfo[index2 - 1];
          if (index1 == index2)
          {
            varList2.AddRange((IEnumerable<Var>) varList1);
          }
          else
          {
            foreach (Var flattenedElementVar in (List<Var>) collectionInfo.FlattenedElementVars)
            {
              Var computedVar;
              System.Data.Entity.Core.Query.InternalTrees.Node varDefNode = this.Command.CreateVarDefNode(this.Command.CreateNode((Op) this.Command.CreateNullOp(flattenedElementVar.Type)), out computedVar);
              args.Add(varDefNode);
              varList2.Add(computedVar);
            }
          }
        }
        System.Data.Entity.Core.Query.InternalTrees.Node node4 = this.Command.CreateNode((Op) this.Command.CreateVarDefListOp(), args);
        System.Data.Entity.Core.Query.InternalTrees.Node node5 = this.Command.CreateNode((Op) this.Command.CreateProjectOp(this.Command.CreateVarVec((IEnumerable<Var>) varList2)), node3, node4);
        if (node1 == null)
        {
          node1 = node5;
          leftVars = varList2;
        }
        else
        {
          VarMap leftMap = new VarMap();
          VarMap rightMap = new VarMap();
          for (int index2 = 0; index2 < leftVars.Count; ++index2)
          {
            Var setOpVar = (Var) this.Command.CreateSetOpVar(leftVars[index2].Type);
            leftMap.Add(setOpVar, leftVars[index2]);
            rightMap.Add(setOpVar, varList2[index2]);
          }
          UnionAllOp unionAllOp = this.Command.CreateUnionAllOp(leftMap, rightMap);
          node1 = this.Command.CreateNode((Op) unionAllOp, node1, node5);
          leftVars = NestPullup.GetUnionOutputs(unionAllOp, leftVars);
        }
      }
      varMapList = new List<Dictionary<Var, Var>>();
      IEnumerator<Var> enumerator = (IEnumerator<Var>) leftVars.GetEnumerator();
      if (!enumerator.MoveNext())
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 4, (object) null);
      discriminatorVar = enumerator.Current;
      for (int index1 = 0; index1 < nestNode.Children.Count; ++index1)
      {
        Dictionary<Var, Var> dictionary = new Dictionary<Var, Var>();
        foreach (Var index2 in index1 == 0 ? (List<Var>) drivingNodeVars : (List<Var>) nestOp.CollectionInfo[index1 - 1].FlattenedElementVars)
        {
          if (!enumerator.MoveNext())
            throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 5, (object) null);
          dictionary[index2] = enumerator.Current;
        }
        varMapList.Add(dictionary);
      }
      if (enumerator.MoveNext())
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 6, (object) null);
      return node1;
    }

    private static VarList GetUnionOutputs(UnionAllOp unionOp, VarList leftVars)
    {
      Dictionary<Var, Var> reverseMap = (Dictionary<Var, Var>) unionOp.VarMap[0].GetReverseMap();
      VarList varList = Command.CreateVarList();
      foreach (Var leftVar in (List<Var>) leftVars)
      {
        Var var = reverseMap[leftVar];
        varList.Add(var);
      }
      return varList;
    }
  }
}
