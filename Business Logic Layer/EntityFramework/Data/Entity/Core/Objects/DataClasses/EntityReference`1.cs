// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.EntityReference`1
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
using System.Xml.Serialization;

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>Models a relationship end with multiplicity 1.</summary>
  /// <typeparam name="TEntity">The type of the entity being referenced.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [DataContract]
  [Serializable]
  public class EntityReference<TEntity> : EntityReference where TEntity : class
  {
    private TEntity _cachedValue;
    [NonSerialized]
    private IEntityWrapper _wrappedCachedValue;

    /// <summary>
    /// Creates a new instance of <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" />.
    /// </summary>
    /// <remarks>
    /// The default constructor is required for some serialization scenarios. It should not be used to
    /// create new EntityReferences. Use the GetRelatedReference or GetRelatedEnd methods on the RelationshipManager
    /// class instead.
    /// </remarks>
    public EntityReference()
    {
      this._wrappedCachedValue = NullEntityWrapper.NullWrapper;
    }

    internal EntityReference(
      IEntityWrapper wrappedOwner,
      RelationshipNavigation navigation,
      IRelationshipFixer relationshipFixer)
      : base(wrappedOwner, navigation, relationshipFixer)
    {
      this._wrappedCachedValue = NullEntityWrapper.NullWrapper;
    }

    /// <summary>
    /// Gets or sets the related object returned by this
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" />
    /// .
    /// </summary>
    /// <returns>
    /// The object returned by this <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" />.
    /// </returns>
    [SoapIgnore]
    [XmlIgnore]
    public TEntity Value
    {
      get
      {
        this.DeferredLoad();
        return (TEntity) this.ReferenceValue.Entity;
      }
      set
      {
        this.ReferenceValue = this.EntityWrapperFactory.WrapEntityUsingContext((object) value, this.ObjectContext);
      }
    }

    internal override IEntityWrapper CachedValue
    {
      get
      {
        return this._wrappedCachedValue;
      }
    }

    internal override IEntityWrapper ReferenceValue
    {
      get
      {
        this.CheckOwnerNull();
        return this._wrappedCachedValue;
      }
      set
      {
        this.CheckOwnerNull();
        if (value.Entity != null && value.Entity == this._wrappedCachedValue.Entity)
          return;
        if (value.Entity != null)
        {
          this.ValidateOwnerWithRIConstraints(value, value == NullEntityWrapper.NullWrapper ? (EntityKey) null : value.EntityKey, true);
          ObjectContext objectContext = this.ObjectContext ?? value.Context;
          if (objectContext != null)
            objectContext.ObjectStateManager.TransactionManager.EntityBeingReparented = this.GetDependentEndOfReferentialConstraint(value.Entity);
          try
          {
            this.Add(value, false);
          }
          finally
          {
            if (objectContext != null)
              objectContext.ObjectStateManager.TransactionManager.EntityBeingReparented = (object) null;
          }
        }
        else
        {
          if (this.UsingNoTracking)
          {
            if (this._wrappedCachedValue.Entity != null)
              this.GetOtherEndOfRelationship(this._wrappedCachedValue).OnRelatedEndClear();
            this._isLoaded = false;
          }
          else if (this.ObjectContext != null && this.ObjectContext.ContextOptions.UseConsistentNullReferenceBehavior)
            this.AttemptToNullFKsOnRefOrKeySetToNull();
          this.ClearCollectionOrRef((IEntityWrapper) null, (RelationshipNavigation) null, false);
        }
      }
    }

    /// <summary>
    /// Loads the related object for this <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" /> with the specified merge option.
    /// </summary>
    /// <param name="mergeOption">
    /// Specifies how the object should be returned if it already exists in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// The source of the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" /> is null
    /// or a query returned more than one related end
    /// or a query returned zero related ends, and one related end was expected.
    /// </exception>
    public override void Load(MergeOption mergeOption)
    {
      this.CheckOwnerNull();
      bool hasResults;
      ObjectQuery<TEntity> objectQuery = this.ValidateLoad<TEntity>(mergeOption, nameof (EntityReference<TEntity>), out hasResults);
      this._suppressEvents = true;
      try
      {
        IList<TEntity> refreshedValue = (IList<TEntity>) null;
        if (hasResults)
          refreshedValue = (IList<TEntity>) objectQuery.Execute(objectQuery.MergeOption).ToList<TEntity>();
        this.HandleRefreshedValue(mergeOption, refreshedValue);
      }
      finally
      {
        this._suppressEvents = false;
      }
      this.OnAssociationChanged(CollectionChangeAction.Refresh, (object) null);
    }

    /// <inheritdoc />
    public override async Task LoadAsync(
      MergeOption mergeOption,
      CancellationToken cancellationToken)
    {
      this.CheckOwnerNull();
      cancellationToken.ThrowIfCancellationRequested();
      bool hasResults;
      ObjectQuery<TEntity> sourceQuery = this.ValidateLoad<TEntity>(mergeOption, nameof (EntityReference<TEntity>), out hasResults);
      this._suppressEvents = true;
      try
      {
        IList<TEntity> refreshedValue = (IList<TEntity>) null;
        if (hasResults)
        {
          ObjectResult<TEntity> objectResult = await sourceQuery.ExecuteAsync(sourceQuery.MergeOption, cancellationToken).WithCurrentCulture<ObjectResult<TEntity>>();
          refreshedValue = (IList<TEntity>) await objectResult.ToListAsync<TEntity>(cancellationToken).WithCurrentCulture<List<TEntity>>();
        }
        this.HandleRefreshedValue(mergeOption, refreshedValue);
      }
      finally
      {
        this._suppressEvents = false;
      }
      this.OnAssociationChanged(CollectionChangeAction.Refresh, (object) null);
    }

    private void HandleRefreshedValue(MergeOption mergeOption, IList<TEntity> refreshedValue)
    {
      if (refreshedValue == null || !refreshedValue.Any<TEntity>())
      {
        if (!((AssociationType) this.RelationMetadata).IsForeignKey && this.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)
          throw Error.EntityReference_LessThanExpectedRelatedEntitiesFound();
        if (mergeOption == MergeOption.OverwriteChanges || mergeOption == MergeOption.PreserveChanges)
        {
          EntityKey entityKey = this.WrappedOwner.EntityKey;
          if ((object) entityKey == null)
            throw Error.EntityKey_UnexpectedNull();
          this.ObjectContext.ObjectStateManager.RemoveRelationships(mergeOption, (AssociationSet) this.RelationshipSet, entityKey, (AssociationEndMember) this.FromEndMember);
        }
        this._isLoaded = true;
      }
      else
      {
        if (refreshedValue.Count<TEntity>() != 1)
          throw Error.EntityReference_MoreThanExpectedRelatedEntitiesFound();
        this.Merge<TEntity>((IEnumerable<TEntity>) refreshedValue, mergeOption, true);
      }
    }

    internal override IEnumerable GetInternalEnumerable()
    {
      this.CheckOwnerNull();
      if (this.ReferenceValue.Entity == null)
        return (IEnumerable) Enumerable.Empty<object>();
      return (IEnumerable) new object[1]
      {
        this.ReferenceValue.Entity
      };
    }

    internal override IEnumerable<IEntityWrapper> GetWrappedEntities()
    {
      if (this._wrappedCachedValue.Entity == null)
        return (IEnumerable<IEntityWrapper>) new IEntityWrapper[0];
      return (IEnumerable<IEntityWrapper>) new IEntityWrapper[1]
      {
        this._wrappedCachedValue
      };
    }

    /// <summary>Creates a many-to-one or one-to-one relationship between two objects in the object context.</summary>
    /// <param name="entity">The object being attached.</param>
    /// <exception cref="T:System.ArgumentNullException">When the  entity  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">When the  entity  cannot be related to the current related end. This can occur when the association in the conceptual schema does not support a relationship between the two types.</exception>
    public void Attach(TEntity entity)
    {
      Check.NotNull<TEntity>(entity, nameof (entity));
      this.CheckOwnerNull();
      this.Attach((IEnumerable<IEntityWrapper>) new IEntityWrapper[1]
      {
        this.EntityWrapperFactory.WrapEntityUsingContext((object) entity, this.ObjectContext)
      }, false);
    }

    internal override void Include(bool addRelationshipAsUnchanged, bool doAttach)
    {
      if (this._wrappedCachedValue.Entity != null)
      {
        IEntityWrapper entityWrapper = this.EntityWrapperFactory.WrapEntityUsingContext(this._wrappedCachedValue.Entity, this.WrappedOwner.Context);
        if (entityWrapper != this._wrappedCachedValue)
          this._wrappedCachedValue = entityWrapper;
        this.IncludeEntity(this._wrappedCachedValue, addRelationshipAsUnchanged, doAttach);
      }
      else
      {
        if (!(this.DetachedEntityKey != (EntityKey) null))
          return;
        this.IncludeEntityKey(doAttach);
      }
    }

    private void IncludeEntityKey(bool doAttach)
    {
      ObjectStateManager objectStateManager = this.ObjectContext.ObjectStateManager;
      bool flag1 = false;
      bool flag2 = false;
      EntityEntry entityEntry = objectStateManager.FindEntityEntry(this.DetachedEntityKey);
      if (entityEntry == null)
      {
        flag2 = true;
        flag1 = true;
      }
      else if (entityEntry.IsKeyEntry)
      {
        if (this.FromEndMember.RelationshipMultiplicity != RelationshipMultiplicity.Many)
        {
          foreach (RelationshipEntry relationshipEntry in this.ObjectContext.ObjectStateManager.FindRelationshipsByKey(this.DetachedEntityKey))
          {
            if (relationshipEntry.IsSameAssociationSetAndRole((AssociationSet) this.RelationshipSet, (AssociationEndMember) this.ToEndMember, this.DetachedEntityKey) && relationshipEntry.State != EntityState.Deleted)
              throw new InvalidOperationException(Strings.ObjectStateManager_EntityConflictsWithKeyEntry);
          }
        }
        flag1 = true;
      }
      else
      {
        IEntityWrapper wrappedEntity = entityEntry.WrappedEntity;
        if (entityEntry.State == EntityState.Deleted)
          throw new InvalidOperationException(Strings.RelatedEnd_UnableToAddRelationshipWithDeletedEntity);
        RelatedEnd relatedEndInternal = wrappedEntity.RelationshipManager.GetRelatedEndInternal(this.RelationshipName, this.RelationshipNavigation.From);
        if (this.FromEndMember.RelationshipMultiplicity != RelationshipMultiplicity.Many && !relatedEndInternal.IsEmpty())
          throw new InvalidOperationException(Strings.ObjectStateManager_EntityConflictsWithKeyEntry);
        this.Add(wrappedEntity, true, doAttach, false, true, true);
        objectStateManager.TransactionManager.PopulatedEntityReferences.Add((EntityReference) this);
      }
      if (!flag1 || this.IsForeignKey)
        return;
      if (flag2)
      {
        EntitySet entitySet = this.DetachedEntityKey.GetEntitySet(this.ObjectContext.MetadataWorkspace);
        objectStateManager.AddKeyEntry(this.DetachedEntityKey, entitySet);
      }
      EntityKey entityKey = this.WrappedOwner.EntityKey;
      if ((object) entityKey == null)
        throw Error.EntityKey_UnexpectedNull();
      RelationshipWrapper wrapper = new RelationshipWrapper((AssociationSet) this.RelationshipSet, this.RelationshipNavigation.From, entityKey, this.RelationshipNavigation.To, this.DetachedEntityKey);
      objectStateManager.AddNewRelation(wrapper, doAttach ? EntityState.Unchanged : EntityState.Added);
    }

    internal override void Exclude()
    {
      if (this._wrappedCachedValue.Entity != null)
      {
        TransactionManager transactionManager = this.ObjectContext.ObjectStateManager.TransactionManager;
        bool doFixup = transactionManager.PopulatedEntityReferences.Contains((EntityReference) this);
        bool flag = transactionManager.AlignedEntityReferences.Contains((EntityReference) this);
        if ((transactionManager.ProcessedEntities == null || !transactionManager.ProcessedEntities.Contains(this._wrappedCachedValue)) && (doFixup || flag))
        {
          RelationshipEntry relationshipEntry = this.IsForeignKey ? (RelationshipEntry) null : this.FindRelationshipEntryInObjectStateManager(this._wrappedCachedValue);
          this.Remove(this._wrappedCachedValue, doFixup, false, false, false, true);
          if (relationshipEntry != null && relationshipEntry.State != EntityState.Detached)
            relationshipEntry.AcceptChanges();
          if (doFixup)
            transactionManager.PopulatedEntityReferences.Remove((EntityReference) this);
          else
            transactionManager.AlignedEntityReferences.Remove((EntityReference) this);
        }
        else
          this.ExcludeEntity(this._wrappedCachedValue);
      }
      else
      {
        if (!(this.DetachedEntityKey != (EntityKey) null))
          return;
        this.ExcludeEntityKey();
      }
    }

    private void ExcludeEntityKey()
    {
      RelationshipEntry relationship = this.ObjectContext.ObjectStateManager.FindRelationship(this.RelationshipSet, new KeyValuePair<string, EntityKey>(this.RelationshipNavigation.From, this.WrappedOwner.EntityKey), new KeyValuePair<string, EntityKey>(this.RelationshipNavigation.To, this.DetachedEntityKey));
      if (relationship == null)
        return;
      relationship.Delete(false);
      if (relationship.State == EntityState.Detached)
        return;
      relationship.AcceptChanges();
    }

    internal override void ClearCollectionOrRef(
      IEntityWrapper wrappedEntity,
      RelationshipNavigation navigation,
      bool doCascadeDelete)
    {
      if (wrappedEntity == null)
        wrappedEntity = NullEntityWrapper.NullWrapper;
      if (this._wrappedCachedValue.Entity != null)
      {
        if (wrappedEntity.Entity == this._wrappedCachedValue.Entity && navigation.Equals((object) this.RelationshipNavigation))
          this.Remove(this._wrappedCachedValue, false, false, false, false, false);
        else
          this.Remove(this._wrappedCachedValue, true, doCascadeDelete, false, true, false);
      }
      else if (this.WrappedOwner.Entity != null && this.WrappedOwner.Context != null && !this.UsingNoTracking)
        this.WrappedOwner.Context.ObjectStateManager.GetEntityEntry(this.WrappedOwner.Entity).DeleteRelationshipsThatReferenceKeys(this.RelationshipSet, this.ToEndMember);
      if (this.WrappedOwner.Entity == null)
        return;
      this.DetachedEntityKey = (EntityKey) null;
    }

    internal override void ClearWrappedValues()
    {
      this._cachedValue = default (TEntity);
      this._wrappedCachedValue = NullEntityWrapper.NullWrapper;
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
        throw new InvalidOperationException(Strings.RelatedEnd_InvalidContainedType_Reference((object) wrappedEntity.Entity.GetType().FullName, (object) typeof (TEntity).FullName));
    }

    internal override void DisconnectedAdd(IEntityWrapper wrappedEntity)
    {
      this.CheckOwnerNull();
    }

    internal override bool DisconnectedRemove(IEntityWrapper wrappedEntity)
    {
      this.CheckOwnerNull();
      return false;
    }

    internal override bool RemoveFromLocalCache(
      IEntityWrapper wrappedEntity,
      bool resetIsLoaded,
      bool preserveForeignKey)
    {
      this._wrappedCachedValue = NullEntityWrapper.NullWrapper;
      this._cachedValue = default (TEntity);
      if (resetIsLoaded)
        this._isLoaded = false;
      if (this.ObjectContext != null && this.IsForeignKey && !preserveForeignKey)
        this.NullAllForeignKeys();
      return true;
    }

    internal override bool RemoveFromObjectCache(IEntityWrapper wrappedEntity)
    {
      if (this.TargetAccessor.HasProperty)
        this.WrappedOwner.RemoveNavigationPropertyValue((RelatedEnd) this, wrappedEntity.Entity);
      return true;
    }

    internal override void RetrieveReferentialConstraintProperties(
      Dictionary<string, KeyValuePair<object, IntBox>> properties,
      HashSet<object> visited)
    {
      if (this._wrappedCachedValue.Entity == null)
        return;
      foreach (ReferentialConstraint referentialConstraint in ((AssociationType) this.RelationMetadata).ReferentialConstraints)
      {
        if (referentialConstraint.ToRole == this.FromEndMember)
        {
          if (visited.Contains((object) this._wrappedCachedValue))
            throw new InvalidOperationException(Strings.RelationshipManager_CircularRelationshipsWithReferentialConstraints);
          visited.Add((object) this._wrappedCachedValue);
          Dictionary<string, KeyValuePair<object, IntBox>> properties1;
          this._wrappedCachedValue.RelationshipManager.RetrieveReferentialConstraintProperties(out properties1, visited, true);
          for (int index = 0; index < referentialConstraint.FromProperties.Count; ++index)
            EntityEntry.AddOrIncreaseCounter(referentialConstraint, properties, referentialConstraint.ToProperties[index].Name, properties1[referentialConstraint.FromProperties[index].Name].Key);
        }
      }
    }

    internal override bool IsEmpty()
    {
      return this._wrappedCachedValue.Entity == null;
    }

    internal override void VerifyMultiplicityConstraintsForAdd(bool applyConstraints)
    {
      if (applyConstraints && !this.IsEmpty())
        throw new InvalidOperationException(Strings.EntityReference_CannotAddMoreThanOneEntityToEntityReference((object) this.RelationshipNavigation.To, (object) this.RelationshipNavigation.RelationshipName));
    }

    internal override void OnRelatedEndClear()
    {
      this._isLoaded = false;
    }

    internal override bool ContainsEntity(IEntityWrapper wrappedEntity)
    {
      if (this._wrappedCachedValue.Entity != null)
        return this._wrappedCachedValue.Entity == wrappedEntity.Entity;
      return false;
    }

    /// <summary>Creates an equivalent object query that returns the related object.</summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> that returns the related object.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the object is in an <see cref="F:System.Data.Entity.EntityState.Added" /> state
    /// or when the object is in a <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// state with a <see cref="P:System.Data.Entity.Core.Objects.ObjectQuery.MergeOption" />
    /// other than <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />.
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

    internal void InitializeWithValue(RelatedEnd relatedEnd)
    {
      EntityReference<TEntity> entityReference = relatedEnd as EntityReference<TEntity>;
      if (entityReference == null || entityReference._wrappedCachedValue.Entity == null)
        return;
      this._wrappedCachedValue = entityReference._wrappedCachedValue;
      this._cachedValue = (TEntity) this._wrappedCachedValue.Entity;
    }

    internal override bool CheckIfNavigationPropertyContainsEntity(IEntityWrapper wrapper)
    {
      if (!this.TargetAccessor.HasProperty)
        return false;
      return object.ReferenceEquals(this.WrappedOwner.GetNavigationPropertyValue((RelatedEnd) this), wrapper.Entity);
    }

    internal override void VerifyNavigationPropertyForAdd(IEntityWrapper wrapper)
    {
      if (!this.TargetAccessor.HasProperty)
        return;
      object navigationPropertyValue = this.WrappedOwner.GetNavigationPropertyValue((RelatedEnd) this);
      if (!object.ReferenceEquals((object) null, navigationPropertyValue) && !object.ReferenceEquals(navigationPropertyValue, wrapper.Entity))
        throw new InvalidOperationException(Strings.EntityReference_CannotAddMoreThanOneEntityToEntityReference((object) this.RelationshipNavigation.To, (object) this.RelationshipNavigation.RelationshipName));
    }

    /// <summary>This method is used internally to serialize related entity objects.</summary>
    /// <param name="context">The serialized stream.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OnDeserialized]
    [Browsable(false)]
    [SuppressMessage("Microsoft.Usage", "CA2238:ImplementSerializationMethodsCorrectly")]
    public void OnRefDeserialized(StreamingContext context)
    {
      this._wrappedCachedValue = this.EntityWrapperFactory.WrapEntityUsingContext((object) this._cachedValue, this.ObjectContext);
    }

    /// <summary>This method is used internally to serialize related entity objects.</summary>
    /// <param name="context">The serialized stream.</param>
    [OnSerializing]
    [SuppressMessage("Microsoft.Usage", "CA2238:ImplementSerializationMethodsCorrectly")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnSerializing(StreamingContext context)
    {
      if (!(this.WrappedOwner.Entity is IEntityWithRelationships))
        throw new InvalidOperationException(Strings.RelatedEnd_CannotSerialize((object) nameof (EntityReference<TEntity>)));
    }

    internal override void AddToLocalCache(IEntityWrapper wrappedEntity, bool applyConstraints)
    {
      if (wrappedEntity == this._wrappedCachedValue)
        return;
      TransactionManager transactionManager = this.ObjectContext != null ? this.ObjectContext.ObjectStateManager.TransactionManager : (TransactionManager) null;
      if (applyConstraints && this._wrappedCachedValue.Entity != null && (transactionManager == null || transactionManager.ProcessedEntities == null || transactionManager.ProcessedEntities.Contains(this._wrappedCachedValue)))
        throw new InvalidOperationException(Strings.EntityReference_CannotAddMoreThanOneEntityToEntityReference((object) this.RelationshipNavigation.To, (object) this.RelationshipNavigation.RelationshipName));
      if (transactionManager != null)
      {
        if (wrappedEntity.Entity != null)
          transactionManager.BeginRelatedEndAdd();
      }
      try
      {
        this.ClearCollectionOrRef((IEntityWrapper) null, (RelationshipNavigation) null, false);
        this._wrappedCachedValue = wrappedEntity;
        this._cachedValue = (TEntity) wrappedEntity.Entity;
      }
      finally
      {
        if (transactionManager != null && transactionManager.IsRelatedEndAdd)
          transactionManager.EndRelatedEndAdd();
      }
    }

    internal override void AddToObjectCache(IEntityWrapper wrappedEntity)
    {
      if (!this.TargetAccessor.HasProperty)
        return;
      this.WrappedOwner.SetNavigationPropertyValue((RelatedEnd) this, wrappedEntity.Entity);
    }
  }
}
