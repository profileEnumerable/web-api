// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.IEntityWrapper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal interface IEntityWrapper
  {
    RelationshipManager RelationshipManager { get; }

    bool OwnsRelationshipManager { get; }

    object Entity { get; }

    EntityEntry ObjectStateEntry { get; set; }

    void EnsureCollectionNotNull(RelatedEnd relatedEnd);

    EntityKey EntityKey { get; set; }

    EntityKey GetEntityKeyFromEntity();

    ObjectContext Context { get; set; }

    MergeOption MergeOption { get; }

    void AttachContext(ObjectContext context, EntitySet entitySet, MergeOption mergeOption);

    void ResetContext(ObjectContext context, EntitySet entitySet, MergeOption mergeOption);

    void DetachContext();

    void SetChangeTracker(IEntityChangeTracker changeTracker);

    void TakeSnapshot(EntityEntry entry);

    void TakeSnapshotOfRelationships(EntityEntry entry);

    Type IdentityType { get; }

    void CollectionAdd(RelatedEnd relatedEnd, object value);

    bool CollectionRemove(RelatedEnd relatedEnd, object value);

    object GetNavigationPropertyValue(RelatedEnd relatedEnd);

    void SetNavigationPropertyValue(RelatedEnd relatedEnd, object value);

    void RemoveNavigationPropertyValue(RelatedEnd relatedEnd, object value);

    void SetCurrentValue(
      EntityEntry entry,
      StateManagerMemberMetadata member,
      int ordinal,
      object target,
      object value);

    bool InitializingProxyRelatedEnds { get; set; }

    void UpdateCurrentValueRecord(object value, EntityEntry entry);

    bool RequiresRelationshipChangeTracking { get; }

    bool OverridesEqualsOrGetHashCode { get; }
  }
}
