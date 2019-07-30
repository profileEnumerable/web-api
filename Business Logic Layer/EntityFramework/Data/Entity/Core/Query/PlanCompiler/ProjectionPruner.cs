// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ProjectionPruner
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class ProjectionPruner : BasicOpVisitorOfNode
  {
    private readonly System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler m_compilerState;
    private readonly VarVec m_referencedVars;

    private Command m_command
    {
      get
      {
        return this.m_compilerState.Command;
      }
    }

    private ProjectionPruner(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState)
    {
      this.m_compilerState = compilerState;
      this.m_referencedVars = compilerState.Command.CreateVarVec();
    }

    internal static void Process(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState)
    {
      compilerState.Command.Root = ProjectionPruner.Process(compilerState, compilerState.Command.Root);
    }

    internal static System.Data.Entity.Core.Query.InternalTrees.Node Process(
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState,
      System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      return new ProjectionPruner(compilerState).Process(node);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node Process(System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      return this.VisitNode(node);
    }

    private void AddReference(Var v)
    {
      this.m_referencedVars.Set(v);
    }

    private void AddReference(IEnumerable<Var> varSet)
    {
      foreach (Var var in varSet)
        this.AddReference(var);
    }

    private bool IsReferenced(Var v)
    {
      return this.m_referencedVars.IsSet(v);
    }

    private bool IsUnreferenced(Var v)
    {
      return !this.IsReferenced(v);
    }

    private void PruneVarMap(VarMap varMap)
    {
      List<Var> varList = new List<Var>();
      foreach (Var key in varMap.Keys)
      {
        if (!this.IsReferenced(key))
          varList.Add(key);
        else
          this.AddReference(varMap[key]);
      }
      foreach (Var key in varList)
        varMap.Remove(key);
    }

    private void PruneVarSet(VarVec varSet)
    {
      varSet.And(this.m_referencedVars);
    }

    protected override void VisitChildren(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      base.VisitChildren(n);
      this.m_command.RecomputeNodeInfo(n);
    }

    protected override void VisitChildrenReverse(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      base.VisitChildrenReverse(n);
      this.m_command.RecomputeNodeInfo(n);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      VarDefListOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (this.IsReferenced((child.Op as VarDefOp).Var))
          args.Add(this.VisitNode(child));
      }
      return this.m_command.CreateNode((Op) op, args);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      PhysicalProjectOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (n == this.m_command.Root)
      {
        ProjectionPruner.ColumnMapVarTracker.FindVars((ColumnMap) op.ColumnMap, this.m_referencedVars);
        op.Outputs.RemoveAll(new Predicate<Var>(this.IsUnreferenced));
      }
      else
        this.AddReference((IEnumerable<Var>) op.Outputs);
      this.VisitChildren(n);
      return n;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitNestOp(
      NestBaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.AddReference((IEnumerable<Var>) op.Outputs);
      this.VisitChildren(n);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      SingleStreamNestOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.AddReference(op.Discriminator);
      return this.VisitNestOp((NestBaseOp) op, n);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      MultiStreamNestOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.VisitNestOp((NestBaseOp) op, n);
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitApplyOp(
      ApplyBaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildrenReverse(n);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      DistinctOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (op.Keys.Count > 1 && n.Child0.Op.OpType == OpType.Project)
        this.RemoveRedundantConstantKeys(op.Keys, ((ProjectOp) n.Child0.Op).Outputs, n.Child0.Child1);
      this.AddReference((IEnumerable<Var>) op.Keys);
      this.VisitChildren(n);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(ElementOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.AddReference((IEnumerable<Var>) this.m_command.GetExtendedNodeInfo(n.Child0).Definitions);
      n.Child0 = this.VisitNode(n.Child0);
      this.m_command.RecomputeNodeInfo(n);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(FilterOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildrenReverse(n);
      return n;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitGroupByOp(
      GroupByBaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      for (int index = n.Children.Count - 1; index >= 2; --index)
        n.Children[index] = this.VisitNode(n.Children[index]);
      if (op.Keys.Count > 1)
        this.RemoveRedundantConstantKeys(op.Keys, op.Outputs, n.Child1);
      this.AddReference((IEnumerable<Var>) op.Keys);
      n.Children[1] = this.VisitNode(n.Children[1]);
      n.Children[0] = this.VisitNode(n.Children[0]);
      this.PruneVarSet(op.Outputs);
      if (op.Keys.Count == 0 && op.Outputs.Count == 0)
        return this.m_command.CreateNode((Op) this.m_command.CreateSingleRowTableOp());
      this.m_command.RecomputeNodeInfo(n);
      return n;
    }

    private void RemoveRedundantConstantKeys(VarVec keyVec, VarVec outputVec, System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode)
    {
      List<System.Data.Entity.Core.Query.InternalTrees.Node> constantKeys = varDefListNode.Children.Where<System.Data.Entity.Core.Query.InternalTrees.Node>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, bool>) (d =>
      {
        if (d.Op.OpType == OpType.VarDef)
          return PlanCompilerUtil.IsConstantBaseOp(d.Child0.Op.OpType);
        return false;
      })).ToList<System.Data.Entity.Core.Query.InternalTrees.Node>();
      VarVec constantKeyVars = this.m_command.CreateVarVec(constantKeys.Select<System.Data.Entity.Core.Query.InternalTrees.Node, Var>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, Var>) (d => ((VarDefOp) d.Op).Var)));
      constantKeyVars.Minus(this.m_referencedVars);
      keyVec.Minus(constantKeyVars);
      outputVec.Minus(constantKeyVars);
      varDefListNode.Children.RemoveAll((Predicate<System.Data.Entity.Core.Query.InternalTrees.Node>) (c =>
      {
        if (constantKeys.Contains(c))
          return constantKeyVars.IsSet(((VarDefOp) c.Op).Var);
        return false;
      }));
      if (keyVec.Count != 0)
        return;
      System.Data.Entity.Core.Query.InternalTrees.Node node = constantKeys.First<System.Data.Entity.Core.Query.InternalTrees.Node>();
      Var var = ((VarDefOp) node.Op).Var;
      keyVec.Set(var);
      outputVec.Set(var);
      varDefListNode.Children.Add(node);
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      GroupByIntoOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.VisitGroupByOp((GroupByBaseOp) op, n);
      if (node.Op.OpType == OpType.GroupByInto && n.Child3.Children.Count == 0)
      {
        GroupByIntoOp op1 = (GroupByIntoOp) node.Op;
        node = this.m_command.CreateNode((Op) this.m_command.CreateGroupByOp(op1.Keys, op1.Outputs), node.Child0, node.Child1, node.Child2);
      }
      return node;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitJoinOp(
      JoinBaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (n.Op.OpType == OpType.CrossJoin)
      {
        this.VisitChildren(n);
        return n;
      }
      n.Child2 = this.VisitNode(n.Child2);
      n.Child0 = this.VisitNode(n.Child0);
      n.Child1 = this.VisitNode(n.Child1);
      this.m_command.RecomputeNodeInfo(n);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(ProjectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.PruneVarSet(op.Outputs);
      this.VisitChildrenReverse(n);
      if (!op.Outputs.IsEmpty)
        return n;
      return n.Child0;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "scanTable")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      ScanTableOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!n.HasChild0, "scanTable with an input?");
      op.Table.ReferencedColumns.And(this.m_referencedVars);
      this.m_command.RecomputeNodeInfo(n);
      return n;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitSetOp(
      SetOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (OpType.Intersect == op.OpType || OpType.Except == op.OpType)
        this.AddReference((IEnumerable<Var>) op.Outputs);
      this.PruneVarSet(op.Outputs);
      foreach (VarMap var in op.VarMap)
        this.PruneVarMap(var);
      this.VisitChildren(n);
      return n;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitSortOp(
      SortBaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      foreach (SortKey key in op.Keys)
        this.AddReference(key.Var);
      if (n.HasChild1)
        n.Child1 = this.VisitNode(n.Child1);
      n.Child0 = this.VisitNode(n.Child0);
      this.m_command.RecomputeNodeInfo(n);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(UnnestOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.AddReference(op.Var);
      this.VisitChildren(n);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(VarRefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.AddReference(op.Var);
      return n;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(ExistsOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.AddReference(((ProjectOp) n.Child0.Op).Outputs.First);
      this.VisitChildren(n);
      return n;
    }

    private class ColumnMapVarTracker : ColumnMapVisitor<VarVec>
    {
      internal static void FindVars(ColumnMap columnMap, VarVec vec)
      {
        ProjectionPruner.ColumnMapVarTracker columnMapVarTracker = new ProjectionPruner.ColumnMapVarTracker();
        columnMap.Accept<VarVec>((ColumnMapVisitor<VarVec>) columnMapVarTracker, vec);
      }

      private ColumnMapVarTracker()
      {
      }

      internal override void Visit(VarRefColumnMap columnMap, VarVec arg)
      {
        arg.Set(columnMap.Var);
        base.Visit(columnMap, arg);
      }
    }
  }
}
