// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.NullEntityWrapper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class NullEntityWrapper : IEntityWrapper
  {
    private static readonly IEntityWrapper _nullWrapper = (IEntityWrapper) new NullEntityWrapper();

    private NullEntityWrapper()
    {
    }

    internal static IEntityWrapper NullWrapper
    {
      get
      {
        return NullEntityWrapper._nullWrapper;
      }
    }

    public RelationshipManager RelationshipManager
    {
      get
      {
        return (RelationshipManager) null;
      }
    }

    public bool OwnsRelationshipManager
    {
      get
      {
        return false;
      }
    }

    public object Entity
    {
      get
      {
        return (object) null;
      }
    }

    public EntityEntry ObjectStateEntry
    {
      get
      {
        return (EntityEntry) null;
      }
      set
      {
      }
    }

    public void CollectionAdd(RelatedEnd relatedEnd, object value)
    {
    }

    public bool CollectionRemove(RelatedEnd relatedEnd, object value)
    {
      return false;
    }

    public EntityKey EntityKey
    {
      get
      {
        return (EntityKey) null;
      }
      set
      {
      }
    }

    public EntityKey GetEntityKeyFromEntity()
    {
      return (EntityKey) null;
    }

    public ObjectContext Context
    {
      get
      {
        return (ObjectContext) null;
      }
      set
      {
      }
    }

    public MergeOption MergeOption
    {
      get
      {
        return MergeOption.NoTracking;
      }
    }

    public void AttachContext(ObjectContext context, EntitySet entitySet, MergeOption mergeOption)
    {
    }

    public void ResetContext(ObjectContext context, EntitySet entitySet, MergeOption mergeOption)
    {
    }

    public void DetachContext()
    {
    }

    public void SetChangeTracker(IEntityChangeTracker changeTracker)
    {
    }

    public void TakeSnapshot(EntityEntry entry)
    {
    }

    public void TakeSnapshotOfRelationships(EntityEntry entry)
    {
    }

    public Type IdentityType
    {
      get
      {
        return (Type) null;
      }
    }

    public void EnsureCollectionNotNull(RelatedEnd relatedEnd)
    {
    }

    public object GetNavigationPropertyValue(RelatedEnd relatedEnd)
    {
      return (object) null;
    }

    public void SetNavigationPropertyValue(RelatedEnd relatedEnd, object value)
    {
    }

    public void RemoveNavigationPropertyValue(RelatedEnd relatedEnd, object value)
    {
    }

    public void SetCurrentValue(
      EntityEntry entry,
      StateManagerMemberMetadata member,
      int ordinal,
      object target,
      object value)
    {
    }

    public bool InitializingProxyRelatedEnds
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    public void UpdateCurrentValueRecord(object value, EntityEntry entry)
    {
    }

    public bool RequiresRelationshipChangeTracking
    {
      get
      {
        return false;
      }
    }

    public bool OverridesEqualsOrGetHashCode
    {
      get
      {
        return false;
      }
    }
  }
}
