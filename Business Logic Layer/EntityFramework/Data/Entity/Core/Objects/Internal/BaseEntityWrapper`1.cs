// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.BaseEntityWrapper`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal abstract class BaseEntityWrapper<TEntity> : IEntityWrapper where TEntity : class
  {
    private readonly RelationshipManager _relationshipManager;
    private Type _identityType;
    private BaseEntityWrapper<TEntity>.WrapperFlags _flags;

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "entity")]
    protected BaseEntityWrapper(
      TEntity entity,
      RelationshipManager relationshipManager,
      bool overridesEquals)
    {
      if (relationshipManager == null)
        throw new InvalidOperationException(Strings.RelationshipManager_UnexpectedNull);
      this._relationshipManager = relationshipManager;
      if (!overridesEquals)
        return;
      this._flags = BaseEntityWrapper<>.WrapperFlags.OverridesEquals;
    }

    protected BaseEntityWrapper(
      TEntity entity,
      RelationshipManager relationshipManager,
      EntitySet entitySet,
      ObjectContext context,
      MergeOption mergeOption,
      Type identityType,
      bool overridesEquals)
    {
      if (relationshipManager == null)
        throw new InvalidOperationException(Strings.RelationshipManager_UnexpectedNull);
      this._identityType = identityType;
      this._relationshipManager = relationshipManager;
      if (overridesEquals)
        this._flags = BaseEntityWrapper<>.WrapperFlags.OverridesEquals;
      this.RelationshipManager.SetWrappedOwner((IEntityWrapper) this, (object) entity);
      if (entitySet == null)
        return;
      this.Context = context;
      this.MergeOption = mergeOption;
      this.RelationshipManager.AttachContextToRelatedEnds(context, entitySet, mergeOption);
    }

    public RelationshipManager RelationshipManager
    {
      get
      {
        return this._relationshipManager;
      }
    }

    public ObjectContext Context { get; set; }

    public MergeOption MergeOption
    {
      get
      {
        return (this._flags & BaseEntityWrapper<>.WrapperFlags.NoTracking) == BaseEntityWrapper<>.WrapperFlags.None ? MergeOption.AppendOnly : MergeOption.NoTracking;
      }
      private set
      {
        if (value == MergeOption.NoTracking)
          this._flags |= BaseEntityWrapper<>.WrapperFlags.NoTracking;
        else
          this._flags &= ~BaseEntityWrapper<>.WrapperFlags.NoTracking;
      }
    }

    public bool InitializingProxyRelatedEnds
    {
      get
      {
        return (this._flags & BaseEntityWrapper<>.WrapperFlags.InitializingRelatedEnds) != BaseEntityWrapper<>.WrapperFlags.None;
      }
      set
      {
        if (value)
          this._flags |= BaseEntityWrapper<>.WrapperFlags.InitializingRelatedEnds;
        else
          this._flags &= ~BaseEntityWrapper<>.WrapperFlags.InitializingRelatedEnds;
      }
    }

    public void AttachContext(ObjectContext context, EntitySet entitySet, MergeOption mergeOption)
    {
      this.Context = context;
      this.MergeOption = mergeOption;
      if (entitySet == null)
        return;
      this.RelationshipManager.AttachContextToRelatedEnds(context, entitySet, mergeOption);
    }

    public void ResetContext(ObjectContext context, EntitySet entitySet, MergeOption mergeOption)
    {
      if (object.ReferenceEquals((object) this.Context, (object) context))
        return;
      this.Context = context;
      this.MergeOption = mergeOption;
      this.RelationshipManager.ResetContextOnRelatedEnds(context, entitySet, mergeOption);
    }

    public void DetachContext()
    {
      if (this.Context != null && this.Context.ObjectStateManager.TransactionManager.IsAttachTracking)
      {
        MergeOption? originalMergeOption = this.Context.ObjectStateManager.TransactionManager.OriginalMergeOption;
        if ((originalMergeOption.GetValueOrDefault() != MergeOption.NoTracking ? 0 : (originalMergeOption.HasValue ? 1 : 0)) != 0)
        {
          this.MergeOption = MergeOption.NoTracking;
          goto label_4;
        }
      }
      this.Context = (ObjectContext) null;
label_4:
      this.RelationshipManager.DetachContextFromRelatedEnds();
    }

    public EntityEntry ObjectStateEntry { get; set; }

    public Type IdentityType
    {
      get
      {
        if (this._identityType == (Type) null)
          this._identityType = EntityUtil.GetEntityIdentityType(typeof (TEntity));
        return this._identityType;
      }
    }

    public bool OverridesEqualsOrGetHashCode
    {
      get
      {
        return (this._flags & BaseEntityWrapper<>.WrapperFlags.OverridesEquals) != BaseEntityWrapper<>.WrapperFlags.None;
      }
    }

    public abstract void EnsureCollectionNotNull(RelatedEnd relatedEnd);

    public abstract EntityKey EntityKey { get; set; }

    public abstract bool OwnsRelationshipManager { get; }

    public abstract EntityKey GetEntityKeyFromEntity();

    public abstract void SetChangeTracker(IEntityChangeTracker changeTracker);

    public abstract void TakeSnapshot(EntityEntry entry);

    public abstract void TakeSnapshotOfRelationships(EntityEntry entry);

    public abstract object GetNavigationPropertyValue(RelatedEnd relatedEnd);

    public abstract void SetNavigationPropertyValue(RelatedEnd relatedEnd, object value);

    public abstract void RemoveNavigationPropertyValue(RelatedEnd relatedEnd, object value);

    public abstract void CollectionAdd(RelatedEnd relatedEnd, object value);

    public abstract bool CollectionRemove(RelatedEnd relatedEnd, object value);

    public abstract object Entity { get; }

    public abstract TEntity TypedEntity { get; }

    public abstract void SetCurrentValue(
      EntityEntry entry,
      StateManagerMemberMetadata member,
      int ordinal,
      object target,
      object value);

    public abstract void UpdateCurrentValueRecord(object value, EntityEntry entry);

    public abstract bool RequiresRelationshipChangeTracking { get; }

    [Flags]
    private enum WrapperFlags
    {
      None = 0,
      NoTracking = 1,
      InitializingRelatedEnds = 2,
      OverridesEquals = 4,
    }
  }
}
