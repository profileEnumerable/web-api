// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.BasicOpVisitorOfT`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class BasicOpVisitorOfT<TResultType>
  {
    protected virtual void VisitChildren(Node n)
    {
      for (int index = 0; index < n.Children.Count; ++index)
        this.VisitNode(n.Children[index]);
    }

    protected virtual void VisitChildrenReverse(Node n)
    {
      for (int index = n.Children.Count - 1; index >= 0; --index)
        this.VisitNode(n.Children[index]);
    }

    internal TResultType VisitNode(Node n)
    {
      return n.Op.Accept<TResultType>(this, n);
    }

    protected virtual TResultType VisitDefault(Node n)
    {
      this.VisitChildren(n);
      return default (TResultType);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal virtual TResultType Unimplemented(Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "Not implemented op type");
      return default (TResultType);
    }

    public virtual TResultType Visit(Op op, Node n)
    {
      return this.Unimplemented(n);
    }

    protected virtual TResultType VisitAncillaryOpDefault(AncillaryOp op, Node n)
    {
      return this.VisitDefault(n);
    }

    public virtual TResultType Visit(VarDefOp op, Node n)
    {
      return this.VisitAncillaryOpDefault((AncillaryOp) op, n);
    }

    public virtual TResultType Visit(VarDefListOp op, Node n)
    {
      return this.VisitAncillaryOpDefault((AncillaryOp) op, n);
    }

    protected virtual TResultType VisitPhysicalOpDefault(PhysicalOp op, Node n)
    {
      return this.VisitDefault(n);
    }

    public virtual TResultType Visit(PhysicalProjectOp op, Node n)
    {
      return this.VisitPhysicalOpDefault((PhysicalOp) op, n);
    }

    protected virtual TResultType VisitNestOp(NestBaseOp op, Node n)
    {
      return this.VisitPhysicalOpDefault((PhysicalOp) op, n);
    }

    public virtual TResultType Visit(SingleStreamNestOp op, Node n)
    {
      return this.VisitNestOp((NestBaseOp) op, n);
    }

    public virtual TResultType Visit(MultiStreamNestOp op, Node n)
    {
      return this.VisitNestOp((NestBaseOp) op, n);
    }

    protected virtual TResultType VisitRelOpDefault(RelOp op, Node n)
    {
      return this.VisitDefault(n);
    }

    protected virtual TResultType VisitApplyOp(ApplyBaseOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual TResultType Visit(CrossApplyOp op, Node n)
    {
      return this.VisitApplyOp((ApplyBaseOp) op, n);
    }

    public virtual TResultType Visit(OuterApplyOp op, Node n)
    {
      return this.VisitApplyOp((ApplyBaseOp) op, n);
    }

    protected virtual TResultType VisitJoinOp(JoinBaseOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual TResultType Visit(CrossJoinOp op, Node n)
    {
      return this.VisitJoinOp((JoinBaseOp) op, n);
    }

    public virtual TResultType Visit(FullOuterJoinOp op, Node n)
    {
      return this.VisitJoinOp((JoinBaseOp) op, n);
    }

    public virtual TResultType Visit(LeftOuterJoinOp op, Node n)
    {
      return this.VisitJoinOp((JoinBaseOp) op, n);
    }

    public virtual TResultType Visit(InnerJoinOp op, Node n)
    {
      return this.VisitJoinOp((JoinBaseOp) op, n);
    }

    protected virtual TResultType VisitSetOp(SetOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual TResultType Visit(ExceptOp op, Node n)
    {
      return this.VisitSetOp((SetOp) op, n);
    }

    public virtual TResultType Visit(IntersectOp op, Node n)
    {
      return this.VisitSetOp((SetOp) op, n);
    }

    public virtual TResultType Visit(UnionAllOp op, Node n)
    {
      return this.VisitSetOp((SetOp) op, n);
    }

    public virtual TResultType Visit(DistinctOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual TResultType Visit(FilterOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual TResultType VisitGroupByOp(GroupByBaseOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual TResultType Visit(GroupByOp op, Node n)
    {
      return this.VisitGroupByOp((GroupByBaseOp) op, n);
    }

    public virtual TResultType Visit(GroupByIntoOp op, Node n)
    {
      return this.VisitGroupByOp((GroupByBaseOp) op, n);
    }

    public virtual TResultType Visit(ProjectOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual TResultType VisitTableOp(ScanTableBaseOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual TResultType Visit(ScanTableOp op, Node n)
    {
      return this.VisitTableOp((ScanTableBaseOp) op, n);
    }

    public virtual TResultType Visit(ScanViewOp op, Node n)
    {
      return this.VisitTableOp((ScanTableBaseOp) op, n);
    }

    public virtual TResultType Visit(SingleRowOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual TResultType Visit(SingleRowTableOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual TResultType VisitSortOp(SortBaseOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    public virtual TResultType Visit(SortOp op, Node n)
    {
      return this.VisitSortOp((SortBaseOp) op, n);
    }

    public virtual TResultType Visit(ConstrainedSortOp op, Node n)
    {
      return this.VisitSortOp((SortBaseOp) op, n);
    }

    public virtual TResultType Visit(UnnestOp op, Node n)
    {
      return this.VisitRelOpDefault((RelOp) op, n);
    }

    protected virtual TResultType VisitScalarOpDefault(ScalarOp op, Node n)
    {
      return this.VisitDefault(n);
    }

    protected virtual TResultType VisitConstantOp(ConstantBaseOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(AggregateOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(ArithmeticOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(CaseOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(CastOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(SoftCastOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(CollectOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(ComparisonOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(ConditionalOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(ConstantOp op, Node n)
    {
      return this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual TResultType Visit(ConstantPredicateOp op, Node n)
    {
      return this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual TResultType Visit(ElementOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(ExistsOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(FunctionOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(GetEntityRefOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(GetRefKeyOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(InternalConstantOp op, Node n)
    {
      return this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual TResultType Visit(IsOfOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(LikeOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(NewEntityOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(NewInstanceOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(DiscriminatedNewEntityOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(NewMultisetOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(NewRecordOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(NullOp op, Node n)
    {
      return this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual TResultType Visit(NullSentinelOp op, Node n)
    {
      return this.VisitConstantOp((ConstantBaseOp) op, n);
    }

    public virtual TResultType Visit(PropertyOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(RelPropertyOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(RefOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(TreatOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(VarRefOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(DerefOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }

    public virtual TResultType Visit(NavigateOp op, Node n)
    {
      return this.VisitScalarOpDefault((ScalarOp) op, n);
    }
  }
}
