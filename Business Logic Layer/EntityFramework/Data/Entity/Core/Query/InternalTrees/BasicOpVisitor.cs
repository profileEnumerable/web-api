// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.BasicOpVisitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class BasicOpVisitor
  {
    protected virtual void VisitChildren(Node n)
    {
      foreach (Node child in n.Children)
        this.VisitNode(child);
    }

    protected virtual void VisitChildrenReverse(Node n)
    {
      for (int index = n.Children.Count - 1; index >= 0; --index)
        this.VisitNode(n.Children[index]);
    }

    internal virtual void VisitNode(Node n)
    {
      n.Op.Accept(this, n);
    }

    protected virtual void VisitDefault(Node n)
    {
      this.VisitChildren(n);
    }

    protected virtual void VisitConstantOp(ConstantBaseOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    protected virtual void VisitTableOp(ScanTableBaseOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual void VisitJoinOp(JoinBaseOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual void VisitApplyOp(ApplyBaseOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual void VisitSetOp(SetOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual void VisitSortOp(SortBaseOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual void VisitGroupByOp(GroupByBaseOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual void Visit(Op op, Node n)
    {
      throw new NotSupportedException(Strings.Iqt_General_UnsupportedOp((object) op.GetType().FullName));
    }

    protected virtual void VisitScalarOpDefault(ScalarOp op, Node n)
    {
      this.VisitDefault(n);
    }

    public virtual void Visit(ConstantOp op, Node n)
    {
      this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual void Visit(NullOp op, Node n)
    {
      this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual void Visit(NullSentinelOp op, Node n)
    {
      this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual void Visit(InternalConstantOp op, Node n)
    {
      this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual void Visit(ConstantPredicateOp op, Node n)
    {
      this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual void Visit(FunctionOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(PropertyOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(RelPropertyOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(CaseOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(ComparisonOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(LikeOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(AggregateOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(NewInstanceOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(NewEntityOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(DiscriminatedNewEntityOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(NewMultisetOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(NewRecordOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(RefOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(VarRefOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(ConditionalOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(ArithmeticOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(TreatOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(CastOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(SoftCastOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(IsOfOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(ExistsOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(ElementOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(GetEntityRefOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(GetRefKeyOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(CollectOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(DerefOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual void Visit(NavigateOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    protected virtual void VisitAncillaryOpDefault(AncillaryOp op, Node n)
    {
      this.VisitDefault(n);
    }

    public virtual void Visit(VarDefOp op, Node n)
    {
      this.VisitAncillaryOpDefault((AncillaryOp) op, n);
    }

    public virtual void Visit(VarDefListOp op, Node n)
    {
      this.VisitAncillaryOpDefault((AncillaryOp) op, n);
    }

    protected virtual void VisitRelOpDefault(RelOp op, Node n)
    {
      this.VisitDefault(n);
    }

    public virtual void Visit(ScanTableOp op, Node n)
    {
      this.VisitTableOp((ScanTableBaseOp) op, n);
    }

    public virtual void Visit(ScanViewOp op, Node n)
    {
      this.VisitTableOp((ScanTableBaseOp) op, n);
    }

    public virtual void Visit(UnnestOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual void Visit(ProjectOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual void Visit(FilterOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual void Visit(SortOp op, Node n)
    {
      this.VisitSortOp((SortBaseOp) op, n);
    }

    public virtual void Visit(ConstrainedSortOp op, Node n)
    {
      this.VisitSortOp((SortBaseOp) op, n);
    }

    public virtual void Visit(GroupByOp op, Node n)
    {
      this.VisitGroupByOp((GroupByBaseOp) op, n);
    }

    public virtual void Visit(GroupByIntoOp op, Node n)
    {
      this.VisitGroupByOp((GroupByBaseOp) op, n);
    }

    public virtual void Visit(CrossJoinOp op, Node n)
    {
      this.VisitJoinOp((JoinBaseOp) op, n);
    }

    public virtual void Visit(InnerJoinOp op, Node n)
    {
      this.VisitJoinOp((JoinBaseOp) op, n);
    }

    public virtual void Visit(LeftOuterJoinOp op, Node n)
    {
      this.VisitJoinOp((JoinBaseOp) op, n);
    }

    public virtual void Visit(FullOuterJoinOp op, Node n)
    {
      this.VisitJoinOp((JoinBaseOp) op, n);
    }

    public virtual void Visit(CrossApplyOp op, Node n)
    {
      this.VisitApplyOp((ApplyBaseOp) op, n);
    }

    public virtual void Visit(OuterApplyOp op, Node n)
    {
      this.VisitApplyOp((ApplyBaseOp) op, n);
    }

    public virtual void Visit(UnionAllOp op, Node n)
    {
      this.VisitSetOp((SetOp) op, n);
    }

    public virtual void Visit(IntersectOp op, Node n)
    {
      this.VisitSetOp((SetOp) op, n);
    }

    public virtual void Visit(ExceptOp op, Node n)
    {
      this.VisitSetOp((SetOp) op, n);
    }

    public virtual void Visit(DistinctOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual void Visit(SingleRowOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual void Visit(SingleRowTableOp op, Node n)
    {
      this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual void VisitPhysicalOpDefault(PhysicalOp op, Node n)
    {
      this.VisitDefault(n);
    }

    public virtual void Visit(PhysicalProjectOp op, Node n)
    {
      this.VisitPhysicalOpDefault((PhysicalOp) op, n);
    }

    protected virtual void VisitNestOp(NestBaseOp op, Node n)
    {
      this.VisitPhysicalOpDefault((PhysicalOp) op, n);
    }

    public virtual void Visit(SingleStreamNestOp op, Node n)
    {
      this.VisitNestOp((NestBaseOp) op, n);
    }

    public virtual void Visit(MultiStreamNestOp op, Node n)
    {
      this.VisitNestOp((NestBaseOp) op, n);
    }
  }
}
