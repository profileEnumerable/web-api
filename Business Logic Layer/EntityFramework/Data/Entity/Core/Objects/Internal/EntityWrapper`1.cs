// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.EntityWrapper`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal abstract class EntityWrapper<TEntity> : BaseEntityWrapper<TEntity> where TEntity : class
  {
    private readonly TEntity _entity;
    private readonly IPropertyAccessorStrategy _propertyStrategy;
    private readonly IChangeTrackingStrategy _changeTrackingStrategy;
    private readonly IEntityKeyStrategy _keyStrategy;

    protected EntityWrapper(
      TEntity entity,
      RelationshipManager relationshipManager,
      Func<object, IPropertyAccessorStrategy> propertyStrategy,
      Func<object, IChangeTrackingStrategy> changeTrackingStrategy,
      Func<object, IEntityKeyStrategy> keyStrategy,
      bool overridesEquals)
      : base(entity, relationshipManager, overridesEquals)
    {
      if (relationshipManager == null)
        throw new InvalidOperationException(Strings.RelationshipManager_UnexpectedNull);
      this._entity = entity;
      this._propertyStrategy = propertyStrategy((object) entity);
      this._changeTrackingStrategy = changeTrackingStrategy((object) entity);
      this._keyStrategy = keyStrategy((object) entity);
    }

    protected EntityWrapper(
      TEntity entity,
      RelationshipManager relationshipManager,
      EntityKey key,
      EntitySet set,
      ObjectContext context,
      MergeOption mergeOption,
      Type identityType,
      Func<object, IPropertyAccessorStrategy> propertyStrategy,
      Func<object, IChangeTrackingStrategy> changeTrackingStrategy,
      Func<object, IEntityKeyStrategy> keyStrategy,
      bool overridesEquals)
      : base(entity, relationshipManager, set, context, mergeOption, identityType, overridesEquals)
    {
      if (relationshipManager == null)
        throw new InvalidOperationException(Strings.RelationshipManager_UnexpectedNull);
      this._entity = entity;
      this._propertyStrategy = propertyStrategy((object) entity);
      this._changeTrackingStrategy = changeTrackingStrategy((object) entity);
      this._keyStrategy = keyStrategy((object) entity);
      this._keyStrategy.SetEntityKey(key);
    }

    public override void SetChangeTracker(IEntityChangeTracker changeTracker)
    {
      this._changeTrackingStrategy.SetChangeTracker(changeTracker);
    }

    public override void TakeSnapshot(EntityEntry entry)
    {
      this._changeTrackingStrategy.TakeSnapshot(entry);
    }

    public override EntityKey EntityKey
    {
      get
      {
        return this._keyStrategy.GetEntityKey();
      }
      set
      {
        this._keyStrategy.SetEntityKey(value);
      }
    }

    public override EntityKey GetEntityKeyFromEntity()
    {
      return this._keyStrategy.GetEntityKeyFromEntity();
    }

    public override void CollectionAdd(RelatedEnd relatedEnd, object value)
    {
      if (this._propertyStrategy == null)
        return;
      this._propertyStrategy.CollectionAdd(relatedEnd, value);
    }

    public override bool CollectionRemove(RelatedEnd relatedEnd, object value)
    {
      if (this._propertyStrategy == null)
        return false;
      return this._propertyStrategy.CollectionRemove(relatedEnd, value);
    }

    public override void EnsureCollectionNotNull(RelatedEnd relatedEnd)
    {
      if (this._propertyStrategy == null || this._propertyStrategy.GetNavigationPropertyValue(relatedEnd) != null)
        return;
      object obj = this._propertyStrategy.CollectionCreate(relatedEnd);
      this._propertyStrategy.SetNavigationPropertyValue(relatedEnd, obj);
    }

    public override object GetNavigationPropertyValue(RelatedEnd relatedEnd)
    {
      if (this._propertyStrategy == null)
        return (object) null;
      return this._propertyStrategy.GetNavigationPropertyValue(relatedEnd);
    }

    public override void SetNavigationPropertyValue(RelatedEnd relatedEnd, object value)
    {
      if (this._propertyStrategy == null)
        return;
      this._propertyStrategy.SetNavigationPropertyValue(relatedEnd, value);
    }

    public override void RemoveNavigationPropertyValue(RelatedEnd relatedEnd, object value)
    {
      if (this._propertyStrategy == null || !object.ReferenceEquals(this._propertyStrategy.GetNavigationPropertyValue(relatedEnd), value))
        return;
      this._propertyStrategy.SetNavigationPropertyValue(relatedEnd, (object) null);
    }

    public override object Entity
    {
      get
      {
        return (object) this._entity;
      }
    }

    public override TEntity TypedEntity
    {
      get
      {
        return this._entity;
      }
    }

    public override void SetCurrentValue(
      EntityEntry entry,
      StateManagerMemberMetadata member,
      int ordinal,
      object target,
      object value)
    {
      this._changeTrackingStrategy.SetCurrentValue(entry, member, ordinal, target, value);
    }

    public override void UpdateCurrentValueRecord(object value, EntityEntry entry)
    {
      this._changeTrackingStrategy.UpdateCurrentValueRecord(value, entry);
    }
  }
}
