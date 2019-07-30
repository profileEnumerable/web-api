// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ColumnMapVisitorWithResults`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class ColumnMapVisitorWithResults<TResultType, TArgType>
  {
    protected EntityIdentity VisitEntityIdentity(
      EntityIdentity entityIdentity,
      TArgType arg)
    {
      DiscriminatedEntityIdentity entityIdentity1 = entityIdentity as DiscriminatedEntityIdentity;
      if (entityIdentity1 != null)
        return this.VisitEntityIdentity(entityIdentity1, arg);
      return this.VisitEntityIdentity((SimpleEntityIdentity) entityIdentity, arg);
    }

    protected virtual EntityIdentity VisitEntityIdentity(
      DiscriminatedEntityIdentity entityIdentity,
      TArgType arg)
    {
      return (EntityIdentity) entityIdentity;
    }

    protected virtual EntityIdentity VisitEntityIdentity(
      SimpleEntityIdentity entityIdentity,
      TArgType arg)
    {
      return (EntityIdentity) entityIdentity;
    }

    internal abstract TResultType Visit(ComplexTypeColumnMap columnMap, TArgType arg);

    internal abstract TResultType Visit(DiscriminatedCollectionColumnMap columnMap, TArgType arg);

    internal abstract TResultType Visit(EntityColumnMap columnMap, TArgType arg);

    internal abstract TResultType Visit(SimplePolymorphicColumnMap columnMap, TArgType arg);

    internal abstract TResultType Visit(RecordColumnMap columnMap, TArgType arg);

    internal abstract TResultType Visit(RefColumnMap columnMap, TArgType arg);

    internal abstract TResultType Visit(ScalarColumnMap columnMap, TArgType arg);

    internal abstract TResultType Visit(SimpleCollectionColumnMap columnMap, TArgType arg);

    internal abstract TResultType Visit(VarRefColumnMap columnMap, TArgType arg);

    internal abstract TResultType Visit(
      MultipleDiscriminatorPolymorphicColumnMap columnMap,
      TArgType arg);
  }
}
