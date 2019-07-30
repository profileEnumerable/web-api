// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.EntityWrapperWithoutRelationships`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class EntityWrapperWithoutRelationships<TEntity> : EntityWrapper<TEntity>
    where TEntity : class
  {
    internal EntityWrapperWithoutRelationships(
      TEntity entity,
      EntityKey key,
      EntitySet entitySet,
      ObjectContext context,
      MergeOption mergeOption,
      Type identityType,
      Func<object, IPropertyAccessorStrategy> propertyStrategy,
      Func<object, IChangeTrackingStrategy> changeTrackingStrategy,
      Func<object, IEntityKeyStrategy> keyStrategy,
      bool overridesEquals)
      : base(entity, RelationshipManager.Create(), key, entitySet, context, mergeOption, identityType, propertyStrategy, changeTrackingStrategy, keyStrategy, overridesEquals)
    {
    }

    internal EntityWrapperWithoutRelationships(
      TEntity entity,
      Func<object, IPropertyAccessorStrategy> propertyStrategy,
      Func<object, IChangeTrackingStrategy> changeTrackingStrategy,
      Func<object, IEntityKeyStrategy> keyStrategy,
      bool overridesEquals)
      : base(entity, RelationshipManager.Create(), propertyStrategy, changeTrackingStrategy, keyStrategy, overridesEquals)
    {
    }

    public override bool OwnsRelationshipManager
    {
      get
      {
        return false;
      }
    }

    public override void TakeSnapshotOfRelationships(EntityEntry entry)
    {
      entry.TakeSnapshotOfRelationships();
    }

    public override bool RequiresRelationshipChangeTracking
    {
      get
      {
        return true;
      }
    }
  }
}
