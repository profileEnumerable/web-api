// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>
  /// Collection of entities modeling a particular EDM construct
  /// which can either be all entities of a particular type or
  /// entities participating in a particular relationship.
  /// </summary>
  /// <typeparam name="TEntity">The type of entities in this collection.</typeparam>
  [Serializable]
  public class EntityCollection<TEntity> : RelatedEnd, ICollection<TEntity>, IEnumerable<TEntity>, IEnumerable, IListSource
    where TEntity : class
  {
    private HashSet<TEntity> _relatedEntities;
    [NonSerialized]
    private CollectionChangeEventHandler _onAssociationChangedforObjectView;
    [NonSerialized]
    private Dictionary<TEntity, IEntityWrapper> _wrappedRelatedEntities;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" /> class.
    /// </summary>
    public EntityCollection()
    {
    }

    internal EntityCollection(
      IEntityWrapper wrappedOwner,
      RelationshipNavigation navigation,
      IRelationshipFixer relationshipFixer)
      : base(wrappedOwner, navigation, relationshipFixer)
    {
    }

    internal override event CollectionChangeEventHandler AssociationChangedForObjectView
    {
      add
      {
        this._onAssociationChangedforObjectView += value;
      }
      remove
      {
        this._onAssociationChangedforObjectView -= value;
      }
    }

    private Dictionary<TEntity, IEntityWrapper> WrappedRelatedEntities
    {
      get
      {
        if (this._wrappedRelatedEntities == null)
          this._wrappedRelatedEntities = new Dictionary<TEntity, IEntityWrapper>((IEqualityComparer<TEntity>) ObjectReferenceEqualityComparer.Default);
        return this._wrappedRelatedEntities;
      }
    }

    /// <summary>Gets the number of objects that are contained in the collection.</summary>
    /// <returns>
    /// The number of elements that are contained in the
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />
    /// .
    /// </returns>
    public int Count
    {
      get
      {
        this.DeferredLoad();
        return this.CountInternal;
      }
    }

    internal int CountInternal
    {
      get
      {
        if (this._wrappedRelatedEntities == null)
          return 0;
        return this._wrappedRelatedEntities.Count;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />
    /// is read-only.
    /// </summary>
    /// <returns>Always returns false.</returns>
    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    bool IListSource.ContainsListCollection
    {
      get
      {
        return false;
      }
    }

    internal override void OnAssociationChanged(
      CollectionChangeAction collectionChangeAction,
      object entity)
    {
      if (this._suppressEvents)
        return;
      if (this._onAssociationChangedforObjectView != null)
        this._onAssociationChangedforObjectView((object) this, new CollectionChangeEventArgs(collectionChangeAction, entity));
      if (this._onAssociationChanged == null)
        return;
      this._onAssociationChanged((object) this, new CollectionChangeEventArgs(collectionChangeAction, entity));
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IList IListSource.GetList()
    {
      EntityType entityType = (EntityType) null;
      if (this.WrappedOwner.Entity != null)
      {
        if (this.RelationshipSet != null)
        {
          EntitySet entitySet = ((AssociationSet) this.RelationshipSet).AssociationSetEnds[this.ToEndMember.Name].EntitySet;
          EntityType elementType1 = (EntityType) ((RefType) this.ToEndMember.TypeUsage.EdmType).ElementType;
          EntityType elementType2 = entitySet.ElementType;
          entityType = !elementType1.IsAssignableFrom((EdmType) elementType2) ? elementType1 : elementType2;
        }
      }
      return (IList) ObjectViewFactory.CreateViewForEntityCollection<TEntity>(entityType, this);
    }

    /// <summary>Loads related objects into the collection, using the specified merge option.</summary>
    /// <param name="mergeOption">
    /// Specifies how the objects in this collection should be merged with the objects that might have been returned from previous queries against the same
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </param>
    public override void Load(MergeOption mergeOption)
    {
      this.CheckOwnerNull();
      this.Load((List<IEntityWrapper>) null, mergeOption);
    }

    /// <inheritdoc />
    public override Task LoadAsync(MergeOption mergeOption, CancellationToken cancellationToken)
    {
      this.CheckOwnerNull();
      cancellationToken.ThrowIfCancellationRequested();
      return this.LoadAsync((List<IEntityWrapper>) null, mergeOption, cancellationToken);
    }

    /// <summary>Defines relationships between an object and a collection of related objects in an object context.</summary>
    /// <remarks>
    /// Loads related entities into the local collection. If the collection is already filled
    /// or partially filled, merges existing entities with the given entities. The given
    /// entities are not assumed to be the complete set of related entities.
    /// Owner and all entities passed in must be in Unchanged or Modified state. We allow
    /// deleted elements only when the state manager is already tracking the relationship
    /// instance.
    /// </remarks>
    /// <param name="entities">Collection of objects in the object context that are related to the source object.</param>
    /// <exception cref="T:System.ArgumentNullException"> entities  collection is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// The source object or an object in the  entities  collection is null or is not in an
    /// <see cref="F:System.Data.Entity.EntityState.Unchanged" />
    /// or <see cref="F:System.Data.Entity.EntityState.Modified" /> state.-or-The relationship cannot be defined based on the EDM metadata. This can occur when the association in the conceptual schema does not support a relationship between the two types.
    /// </exception>
    public void Attach(IEnumerable<TEntity> entities)
    {
      Check.NotNull<IEnumerable<TEntity>>(entities, nameof (entities));
      this.CheckOwnerNull();
      IList<IEntityWrapper> entityWrapperList = (IList<IEntityWrapper>) new List<IEntityWrapper>();
      foreach (TEntity entity in entities)
        entityWrapperList.Add(this.EntityWrapperFactory.WrapEntityUsingContext((object) entity, this.ObjectContext));
      this.Attach((IEnumerable<IEntityWrapper>) entityWrapperList, true);
    }

    /// <summary>Defines a relationship between two attached objects in an object context.</summary>
    /// <param name="entity">The object being attached.</param>
    /// <exception cref="T:System.ArgumentNullException">When the  entity  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the  entity  cannot be related to the source object. This can occur when the association in the conceptual schema does not support a relationship between the two types.-or-When either object is null or is not in an
    /// <see cref="F:System.Data.Entity.EntityState.Unchanged" />
    /// or <see cref="F:System.Data.Entity.EntityState.Modified" /> state.
    /// </exception>
    public void Attach(TEntity entity)
    {
      Check.NotNull<TEntity>(entity, nameof (entity));
      this.Attach((IEnumerable<IEntityWrapper>) new IEntityWrapper[1]
      {
        this.EntityWrapperFactory.WrapEntityUsingContext((object) entity, this.ObjectContext)
      }, false);
    }

    internal virtual void Load(List<IEntityWrapper> collection, MergeOption mergeOption)
    {
      bool hasResults;
      ObjectQuery<TEntity> objectQuery = this.ValidateLoad<TEntity>(mergeOption, nameof (EntityCollection<TEntity>), out hasResults);
      this._suppressEvents = true;
      try
      {
        if (collection == null)
          this.Merge<TEntity>(!hasResults ? Enumerable.Empty<TEntity>() : (IEnumerable<TEntity>) objectQuery.Execute(objectQuery.MergeOption), mergeOption, true);
        else
          this.Merge<TEntity>(collection, mergeOption, true);
      }
      finally
      {
        this._suppressEvents = false;
      }
      this.OnAssociationChanged(CollectionChangeAction.Refresh, (object) null);
    }

    internal virtual async Task LoadAsync(
      List<IEntityWrapper> collection,
      MergeOption mergeOption,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      bool hasResults;
      ObjectQuery<TEntity> sourceQuery = this.ValidateLoad<TEntity>(mergeOption, nameof (EntityCollection<TEntity>), out hasResults);
      this._suppressEvents = true;
      try
      {
        if (collection == null)
        {
          IEnumerable<TEntity> refreshedValues;
          if (hasResults)
          {
            ObjectResult<TEntity> queryResult = await sourceQuery.ExecuteAsync(sourceQuery.MergeOption, cancellationToken).WithCurrentCulture<ObjectResult<TEntity>>();
            refreshedValues = (IEnumerable<TEntity>) await queryResult.ToListAsync<TEntity>(cancellationToken).WithCurrentCulture<List<TEntity>>();
          }
          else
            refreshedValues = Enumerable.Empty<TEntity>();
          this.Merge<TEntity>(refreshedValues, mergeOption, true);
        }
        else
          this.Merge<TEntity>(collection, mergeOption, true);
      }
      finally
      {
        this._suppressEvents = false;
      }
      this.OnAssociationChanged(CollectionChangeAction.Refresh, (object) null);
    }

    /// <summary>Adds an object to the collection.</summary>
    /// <param name="item">
    /// An object to add to the collection.  entity  must implement
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.IEntityWithRelationships" />
    /// .
    /// </param>
    /// <exception cref="T:System.ArgumentNullException"> entity  is null.</exception>
    public void Add(TEntity item)
    {
      Check.NotNull<TEntity>(item, nameof (item));
      this.Add(this.EntityWrapperFactory.WrapEntityUsingContext((object) item, this.ObjectContext));
    }

    internal override void DisconnectedAdd(IEntityWrapper wrappedEntity)
    {
      if (wrappedEntity.Context != null && wrappedEntity.MergeOption != MergeOption.NoTracking)
        throw new InvalidOperationException(Strings.RelatedEnd_UnableToAddEntity);
      this.VerifyType(wrappedEntity);
      this.AddToCache(wrappedEntity, false);
      this.OnAssociationChanged(CollectionChangeAction.Add, wrappedEntity.Entity);
    }

    internal override bool DisconnectedRemove(IEntityWrapper wrappedEntity)
    {
      if (wrappedEntity.Context != null && wrappedEntity.MergeOption != MergeOption.NoTracking)
        throw new InvalidOperationException(Strings.RelatedEnd_UnableToRemoveEntity);
      bool flag = this.RemoveFromCache(wrappedEntity, false, false);
      this.OnAssociationChanged(CollectionChangeAction.Remove, wrappedEntity.Entity);
      return flag;
    }

    /// <summary>Removes an object from the collection and marks the relationship for deletion.</summary>
    /// <returns>true if item was successfully removed; otherwise, false. </returns>
    /// <param name="item">The object to remove from the collection.</param>
    /// <exception cref="T:System.ArgumentNullException"> entity  object is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">The  entity  object is not attached to the same object context.-or-The  entity  object does not have a valid relationship manager.</exception>
    public bool Remove(TEntity item)
    {
      Check.NotNull<TEntity>(item, nameof (item));
      this.DeferredLoad();
      return this.RemoveInternal(item);
    }

    internal bool RemoveInternal(TEntity entity)
    {
      return this.Remove(this.EntityWrapperFactory.WrapEntityUsingContext((object) entity, this.ObjectContext), false);
    }

    internal override void Include(bool addRelationshipAsUnchanged, bool doAttach)
    {
      if (this._wrappedRelatedEntities == null || this.ObjectContext == null)
        return;
      foreach (IEntityWrapper entityWrapper in new List<IEntityWrapper>((IEnumerable<IEntityWrapper>) this._wrappedRelatedEntities.Values))
      {
        IEntityWrapper wrappedEntity = this.EntityWrapperFactory.WrapEntityUsingContext(entityWrapper.Entity, this.WrappedOwner.Context);
        if (wrappedEntity != entityWrapper)
          this._wrappedRelatedEntities[(TEntity) wrappedEntity.Entity] = wrappedEntity;
        this.IncludeEntity(wrappedEntity, addRelationshipAsUnchanged, doAttach);
      }
    }

    internal override void Exclude()
    {
      if (this._wrappedRelatedEntities == null || this.ObjectContext == null)
        return;
      if (!this.IsForeignKey)
      {
        foreach (IEntityWrapper wrappedEntity in this._wrappedRelatedEntities.Values)
          this.ExcludeEntity(wrappedEntity);
      }
      else
      {
        TransactionManager transactionManager = this.ObjectContext.ObjectStateManager.TransactionManager;
        foreach (IEntityWrapper wrappedEntity in new List<IEntityWrapper>((IEnumerable<IEntityWrapper>) this._wrappedRelatedEntities.Values))
        {
          EntityReference endOfRelationship = this.GetOtherEndOfRelationship(wrappedEntity) as EntityReference;
          bool doFixup = transactionManager.PopulatedEntityReferences.Contains(endOfRelationship);
          bool flag = transactionManager.AlignedEntityReferences.Contains(endOfRelationship);
          if (doFixup || flag)
          {
            endOfRelationship.Remove(endOfRelationship.CachedValue, doFixup, false, false, false, true);
            if (doFixup)
              transactionManager.PopulatedEntityReferences.Remove(endOfRelationship);
            else
              transactionManager.AlignedEntityReferences.Remove(endOfRelationship);
          }
          else
            this.ExcludeEntity(wrappedEntity);
        }
      }
    }

    internal override void ClearCollectionOrRef(
      IEntityWrapper wrappedEntity,
      RelationshipNavigation navigation,
      bool doCascadeDelete)
    {
      if (this._wrappedRelatedEntities == null)
        return;
      foreach (IEntityWrapper wrappedEntity1 in new List<IEntityWrapper>((IEnumerable<IEntityWrapper>) this._wrappedRelatedEntities.Values))
      {
        if (wrappedEntity.Entity == wrappedEntity1.Entity && navigation.Equals((object) this.RelationshipNavigation))
          this.Remove(wrappedEntity1, false, false, false, false, false);
        else
          this.Remove(wrappedEntity1, true, doCascadeDelete, false, false, false);
      }
    }

    internal override void ClearWrappedValues()
    {
      if (this._wrappedRelatedEntities != null)
        this._wrappedRelatedEntities.Clear();
      if (this._relatedEntities == null)
        return;
      this._relatedEntities.Clear();
    }

    internal override bool VerifyEntityForAdd(
      IEntityWrapper wrappedEntity,
      bool relationshipAlreadyExists)
    {
      if (!relationshipAlreadyExists && this.ContainsEntity(wrappedEntity))
        return false;
      this.VerifyType(wrappedEntity);
      return true;
    }

    internal override bool CanSetEntityType(IEntityWrapper wrappedEntity)
    {
      return wrappedEntity.Entity is TEntity;
    }

    internal override void VerifyType(IEntityWrapper wrappedEntity)
    {
      if (!this.CanSetEntityType(wrappedEntity))
        throw new InvalidOperationException(Strings.RelatedEnd_InvalidContainedType_Collection((object) wrappedEntity.Entity.GetType().FullName, (object) typeof (TEntity).FullName));
    }

    internal override bool RemoveFromLocalCache(
      IEntityWrapper wrappedEntity,
      bool resetIsLoaded,
      bool preserveForeignKey)
    {
      if (this._wrappedRelatedEntities == null || !this._wrappedRelatedEntities.Remove((TEntity) wrappedEntity.Entity))
        return false;
      if (resetIsLoaded)
        this._isLoaded = false;
      return true;
    }

    internal override bool RemoveFromObjectCache(IEntityWrapper wrappedEntity)
    {
      if (this.TargetAccessor.HasProperty)
        return this.WrappedOwner.CollectionRemove((RelatedEnd) this, wrappedEntity.Entity);
      return false;
    }

    internal override void RetrieveReferentialConstraintProperties(
      Dictionary<string, KeyValuePair<object, IntBox>> properties,
      HashSet<object> visited)
    {
    }

    internal override bool IsEmpty()
    {
      if (this._wrappedRelatedEntities != null)
        return this._wrappedRelatedEntities.Count == 0;
      return true;
    }

    internal override void VerifyMultiplicityConstraintsForAdd(bool applyConstraints)
    {
    }

    internal override void OnRelatedEndClear()
    {
      this._isLoaded = false;
    }

    internal override bool ContainsEntity(IEntityWrapper wrappedEntity)
    {
      TEntity entity = wrappedEntity.Entity as TEntity;
      if (this._wrappedRelatedEntities != null)
        return this._wrappedRelatedEntities.ContainsKey(entity);
      return false;
    }

    /// <summary>Returns an enumerator that is used to iterate through the objects in the collection.</summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> that iterates through the set of values cached by
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />
    /// .
    /// </returns>
    public IEnumerator<TEntity> GetEnumerator()
    {
      this.DeferredLoad();
      return (IEnumerator<TEntity>) this.WrappedRelatedEntities.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      this.DeferredLoad();
      return (IEnumerator) this.WrappedRelatedEntities.Keys.GetEnumerator();
    }

    internal override IEnumerable GetInternalEnumerable()
    {
      return (IEnumerable) this.WrappedRelatedEntities.Keys;
    }

    internal override IEnumerable<IEntityWrapper> GetWrappedEntities()
    {
      return (IEnumerable<IEntityWrapper>) this.WrappedRelatedEntities.Values;
    }

    /// <summary>Removes all entities from the collection. </summary>
    public void Clear()
    {
      this.DeferredLoad();
      if (this.WrappedOwner.Entity != null)
      {
        bool flag = this.CountInternal > 0;
        if (this._wrappedRelatedEntities != null)
        {
          List<IEntityWrapper> entityWrapperList = new List<IEntityWrapper>((IEnumerable<IEntityWrapper>) this._wrappedRelatedEntities.Values);
          try
          {
            this._suppressEvents = true;
            foreach (IEntityWrapper wrappedEntity in entityWrapperList)
            {
              this.Remove(wrappedEntity, false);
              if (this.UsingNoTracking)
                this.GetOtherEndOfRelationship(wrappedEntity).OnRelatedEndClear();
            }
          }
          finally
          {
            this._suppressEvents = false;
          }
          if (this.UsingNoTracking)
            this._isLoaded = false;
        }
        if (!flag)
          return;
        this.OnAssociationChanged(CollectionChangeAction.Refresh, (object) null);
      }
      else
      {
        if (this._wrappedRelatedEntities == null)
          return;
        this._wrappedRelatedEntities.Clear();
      }
    }

    /// <summary>Determines whether a specific object exists in the collection.</summary>
    /// <returns>
    /// true if the object is found in the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />; otherwise, false.
    /// </returns>
    /// <param name="item">
    /// The object to locate in the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />.
    /// </param>
    public bool Contains(TEntity item)
    {
      this.DeferredLoad();
      if (this._wrappedRelatedEntities != null)
        return this._wrappedRelatedEntities.ContainsKey(item);
      return false;
    }

    /// <summary>Copies all the contents of the collection to an array, starting at the specified index of the target array.</summary>
    /// <param name="array">The array to copy to.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
    public void CopyTo(TEntity[] array, int arrayIndex)
    {
      this.DeferredLoad();
      this.WrappedRelatedEntities.Keys.CopyTo(array, arrayIndex);
    }

    internal virtual void BulkDeleteAll(List<object> list)
    {
      if (list.Count <= 0)
        return;
      this._suppressEvents = true;
      try
      {
        foreach (object obj in list)
          this.RemoveInternal(obj as TEntity);
      }
      finally
      {
        this._suppressEvents = false;
      }
      this.OnAssociationChanged(CollectionChangeAction.Refresh, (object) null);
    }

    internal override bool CheckIfNavigationPropertyContainsEntity(IEntityWrapper wrapper)
    {
      if (!this.TargetAccessor.HasProperty)
        return false;
      bool state = this.DisableLazyLoading();
      try
      {
        object navigationPropertyValue = this.WrappedOwner.GetNavigationPropertyValue((RelatedEnd) this);
        if (navigationPropertyValue != null)
        {
          IEnumerable<TEntity> source = navigationPropertyValue as IEnumerable<TEntity>;
          if (source == null)
            throw new EntityException(Strings.ObjectStateEntry_UnableToEnumerateCollection((object) this.TargetAccessor.PropertyName, (object) this.WrappedOwner.Entity.GetType().FullName));
          HashSet<TEntity> entitySet = navigationPropertyValue as HashSet<TEntity>;
          if (!wrapper.OverridesEqualsOrGetHashCode || entitySet != null && entitySet.Comparer is ObjectReferenceEqualityComparer)
            return source.Contains<TEntity>((TEntity) wrapper.Entity);
          return source.Any<TEntity>((Func<TEntity, bool>) (o => object.ReferenceEquals((object) o, wrapper.Entity)));
        }
      }
      finally
      {
        this.ResetLazyLoading(state);
      }
      return false;
    }

    internal override void VerifyNavigationPropertyForAdd(IEntityWrapper wrapper)
    {
    }

    /// <summary>Used internally to serialize entity objects.</summary>
    /// <param name="context">The streaming context.</param>
    [SuppressMessage("Microsoft.Usage", "CA2238:ImplementSerializationMethodsCorrectly")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OnSerializing]
    [Browsable(false)]
    public void OnSerializing(StreamingContext context)
    {
      if (!(this.WrappedOwner.Entity is IEntityWithRelationships))
        throw new InvalidOperationException(Strings.RelatedEnd_CannotSerialize((object) nameof (EntityCollection<TEntity>)));
      this._relatedEntities = this._wrappedRelatedEntities == null ? (HashSet<TEntity>) null : new HashSet<TEntity>((IEnumerable<TEntity>) this._wrappedRelatedEntities.Keys, (IEqualityComparer<TEntity>) ObjectReferenceEqualityComparer.Default);
    }

    /// <summary>Used internally to deserialize entity objects.</summary>
    /// <param name="context">The streaming context.</param>
    [Browsable(false)]
    [OnDeserialized]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Usage", "CA2238:ImplementSerializationMethodsCorrectly")]
    public void OnCollectionDeserialized(StreamingContext context)
    {
      if (this._relatedEntities == null)
        return;
      this._relatedEntities.OnDeserialization((object) null);
      this._wrappedRelatedEntities = new Dictionary<TEntity, IEntityWrapper>((IEqualityComparer<TEntity>) ObjectReferenceEqualityComparer.Default);
      foreach (TEntity relatedEntity in this._relatedEntities)
        this._wrappedRelatedEntities.Add(relatedEntity, this.EntityWrapperFactory.WrapEntityUsingContext((object) relatedEntity, this.ObjectContext));
    }

    /// <summary>Returns an object query that, when it is executed, returns the same set of objects that exists in the current collection. </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> that represents the entity collection.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the object is in an <see cref="F:System.Data.Entity.EntityState.Added" /> state
    /// or when the object is in a
    /// <see cref="F:System.Data.Entity.EntityState.Detached" /> state with a
    /// <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> other than
    /// <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />.
    /// </exception>
    public ObjectQuery<TEntity> CreateSourceQuery()
    {
      this.CheckOwnerNull();
      bool hasResults;
      return this.CreateSourceQuery<TEntity>(this.DefaultMergeOption, out hasResults);
    }

    internal override IEnumerable CreateSourceQueryInternal()
    {
      return (IEnumerable) this.CreateSourceQuery();
    }

    internal override void AddToLocalCache(IEntityWrapper wrappedEntity, bool applyConstraints)
    {
      this.WrappedRelatedEntities[(TEntity) wrappedEntity.Entity] = wrappedEntity;
    }

    internal override void AddToObjectCache(IEntityWrapper wrappedEntity)
    {
      if (!this.TargetAccessor.HasProperty)
        return;
      this.WrappedOwner.CollectionAdd((RelatedEnd) this, wrappedEntity.Entity);
    }
  }
}
