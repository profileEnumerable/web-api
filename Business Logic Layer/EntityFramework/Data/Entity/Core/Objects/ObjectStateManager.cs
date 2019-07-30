// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectStateManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// Maintains object state and identity management for entity type instances and relationship instances.
  /// </summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  public class ObjectStateManager : IEntityStateManager
  {
    private readonly Dictionary<EntityKey, HashSet<Tuple<EntityReference, EntityEntry>>> _danglingForeignKeys = new Dictionary<EntityKey, HashSet<Tuple<EntityReference, EntityEntry>>>();
    private const int InitialListSize = 16;
    private Dictionary<EntityKey, EntityEntry> _addedEntityStore;
    private Dictionary<EntityKey, EntityEntry> _modifiedEntityStore;
    private Dictionary<EntityKey, EntityEntry> _deletedEntityStore;
    private Dictionary<EntityKey, EntityEntry> _unchangedEntityStore;
    private Dictionary<object, EntityEntry> _keylessEntityStore;
    private Dictionary<RelationshipWrapper, RelationshipEntry> _addedRelationshipStore;
    private Dictionary<RelationshipWrapper, RelationshipEntry> _deletedRelationshipStore;
    private Dictionary<RelationshipWrapper, RelationshipEntry> _unchangedRelationshipStore;
    private readonly Dictionary<EdmType, StateManagerTypeMetadata> _metadataStore;
    private readonly Dictionary<EntitySetQualifiedType, StateManagerTypeMetadata> _metadataMapping;
    private readonly MetadataWorkspace _metadataWorkspace;
    private CollectionChangeEventHandler onObjectStateManagerChangedDelegate;
    private CollectionChangeEventHandler onEntityDeletedDelegate;
    private bool _inRelationshipFixup;
    private bool _isDisposed;
    private ComplexTypeMaterializer _complexTypeMaterializer;
    private HashSet<EntityEntry> _entriesWithConceptualNulls;
    private readonly EntityWrapperFactory _entityWrapperFactory;
    private bool _detectChangesNeeded;

    internal ObjectStateManager()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" /> class.
    /// </summary>
    /// <param name="metadataWorkspace">
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" />, which supplies mapping and metadata information.
    /// </param>
    public ObjectStateManager(MetadataWorkspace metadataWorkspace)
    {
      Check.NotNull<MetadataWorkspace>(metadataWorkspace, nameof (metadataWorkspace));
      this._metadataWorkspace = metadataWorkspace;
      this._metadataStore = new Dictionary<EdmType, StateManagerTypeMetadata>();
      this._metadataMapping = new Dictionary<EntitySetQualifiedType, StateManagerTypeMetadata>(EntitySetQualifiedType.EqualityComparer);
      this._isDisposed = false;
      this._entityWrapperFactory = new EntityWrapperFactory();
      this.TransactionManager = new TransactionManager();
    }

    internal virtual object ChangingObject { get; set; }

    internal virtual string ChangingEntityMember { get; set; }

    internal virtual string ChangingMember { get; set; }

    internal virtual EntityState ChangingState { get; set; }

    internal virtual bool SaveOriginalValues { get; set; }

    internal virtual object ChangingOldValue { get; set; }

    internal virtual bool InRelationshipFixup
    {
      get
      {
        return this._inRelationshipFixup;
      }
    }

    internal virtual ComplexTypeMaterializer ComplexTypeMaterializer
    {
      get
      {
        if (this._complexTypeMaterializer == null)
          this._complexTypeMaterializer = new ComplexTypeMaterializer(this.MetadataWorkspace);
        return this._complexTypeMaterializer;
      }
    }

    internal virtual TransactionManager TransactionManager { get; private set; }

    internal virtual EntityWrapperFactory EntityWrapperFactory
    {
      get
      {
        return this._entityWrapperFactory;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> associated with this state manager.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> associated with this
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" />
    /// .
    /// </returns>
    public virtual MetadataWorkspace MetadataWorkspace
    {
      get
      {
        return this._metadataWorkspace;
      }
    }

    /// <summary>Occurs when entities are added to or removed from the state manager.</summary>
    public event CollectionChangeEventHandler ObjectStateManagerChanged
    {
      add
      {
        this.onObjectStateManagerChangedDelegate += value;
      }
      remove
      {
        this.onObjectStateManagerChangedDelegate -= value;
      }
    }

    internal event CollectionChangeEventHandler EntityDeleted
    {
      add
      {
        this.onEntityDeletedDelegate += value;
      }
      remove
      {
        this.onEntityDeletedDelegate -= value;
      }
    }

    internal virtual void OnObjectStateManagerChanged(CollectionChangeAction action, object entity)
    {
      if (this.onObjectStateManagerChangedDelegate == null)
        return;
      this.onObjectStateManagerChangedDelegate((object) this, new CollectionChangeEventArgs(action, entity));
    }

    private void OnEntityDeleted(CollectionChangeAction action, object entity)
    {
      if (this.onEntityDeletedDelegate == null)
        return;
      this.onEntityDeletedDelegate((object) this, new CollectionChangeEventArgs(action, entity));
    }

    internal virtual EntityEntry AddKeyEntry(EntityKey entityKey, EntitySet entitySet)
    {
      if (this.FindEntityEntry(entityKey) != null)
        throw new InvalidOperationException(Strings.ObjectStateManager_ObjectStateManagerContainsThisEntityKey((object) entitySet.ElementType.Name));
      return this.InternalAddEntityEntry(entityKey, entitySet);
    }

    internal EntityEntry GetOrAddKeyEntry(EntityKey entityKey, EntitySet entitySet)
    {
      EntityEntry entry;
      if (this.TryGetEntityEntry(entityKey, out entry))
        return entry;
      return this.InternalAddEntityEntry(entityKey, entitySet);
    }

    private EntityEntry InternalAddEntityEntry(EntityKey entityKey, EntitySet entitySet)
    {
      StateManagerTypeMetadata managerTypeMetadata = this.GetOrAddStateManagerTypeMetadata((EdmType) entitySet.ElementType);
      EntityEntry entry = new EntityEntry(entityKey, entitySet, this, managerTypeMetadata);
      this.AddEntityEntryToDictionary(entry, entry.State);
      return entry;
    }

    private void ValidateProxyType(IEntityWrapper wrappedEntity)
    {
      Type identityType = wrappedEntity.IdentityType;
      Type type = wrappedEntity.Entity.GetType();
      if (!(identityType != type))
        return;
      EntityProxyTypeInfo proxyType = EntityProxyFactory.GetProxyType(this.MetadataWorkspace.GetItem<ClrEntityType>(identityType.FullNameWithNesting(), DataSpace.OSpace), this.MetadataWorkspace);
      if (proxyType == null || proxyType.ProxyType != type)
        throw new InvalidOperationException(Strings.EntityProxyTypeInfo_DuplicateOSpaceType((object) identityType.FullName));
    }

    internal virtual EntityEntry AddEntry(
      IEntityWrapper wrappedObject,
      EntityKey passedKey,
      EntitySet entitySet,
      string argumentName,
      bool isAdded)
    {
      EntityKey entityKey = passedKey;
      StateManagerTypeMetadata managerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(wrappedObject.IdentityType, entitySet);
      this.ValidateProxyType(wrappedObject);
      EdmType edmType = managerTypeMetadata.CdmMetadata.EdmType;
      if (isAdded && !entitySet.ElementType.IsAssignableFrom(edmType))
        throw new ArgumentException(Strings.ObjectStateManager_EntityTypeDoesnotMatchtoEntitySetType((object) wrappedObject.Entity.GetType().Name, (object) TypeHelpers.GetFullName(entitySet.EntityContainer.Name, entitySet.Name)), argumentName);
      EntityKey key = !isAdded ? wrappedObject.EntityKey : wrappedObject.GetEntityKeyFromEntity();
      if ((object) key != null)
      {
        entityKey = key;
        if ((object) entityKey == null)
          throw new InvalidOperationException(Strings.EntityKey_UnexpectedNull);
        if (wrappedObject.EntityKey != entityKey)
          throw new InvalidOperationException(Strings.EntityKey_DoesntMatchKeyOnEntity((object) wrappedObject.Entity.GetType().FullName));
      }
      if ((object) entityKey != null && !entityKey.IsTemporary && !isAdded)
        this.CheckKeyMatchesEntity(wrappedObject, entityKey, entitySet, false);
      EntityEntry entityEntry1;
      if (isAdded && (key == (EntityKey) null && (entityEntry1 = this.FindEntityEntry(wrappedObject.Entity)) != null || key != (EntityKey) null && (entityEntry1 = this.FindEntityEntry(key)) != null))
      {
        if (entityEntry1.Entity != wrappedObject.Entity)
          throw new InvalidOperationException(Strings.ObjectStateManager_ObjectStateManagerContainsThisEntityKey((object) wrappedObject.IdentityType.FullName));
        if (entityEntry1.State != EntityState.Added)
          throw new InvalidOperationException(Strings.ObjectStateManager_DoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity((object) entityEntry1.State));
        return (EntityEntry) null;
      }
      if ((object) entityKey == null || isAdded && !entityKey.IsTemporary)
      {
        entityKey = new EntityKey((EntitySetBase) entitySet);
        wrappedObject.EntityKey = entityKey;
      }
      if (!wrappedObject.OwnsRelationshipManager)
        wrappedObject.RelationshipManager.ClearRelatedEndWrappers();
      EntityEntry entityEntry2 = new EntityEntry(wrappedObject, entityKey, entitySet, this, managerTypeMetadata, isAdded ? EntityState.Added : EntityState.Unchanged);
      entityEntry2.AttachObjectStateManagerToEntity();
      this.AddEntityEntryToDictionary(entityEntry2, entityEntry2.State);
      this.OnObjectStateManagerChanged(CollectionChangeAction.Add, entityEntry2.Entity);
      if (!isAdded)
        this.FixupReferencesByForeignKeys(entityEntry2, false);
      return entityEntry2;
    }

    internal virtual void FixupReferencesByForeignKeys(EntityEntry newEntry, bool replaceAddedRefs = false)
    {
      if (!((EntitySet) newEntry.EntitySet).HasForeignKeyRelationships)
        return;
      newEntry.FixupReferencesByForeignKeys(replaceAddedRefs, (EntitySetBase) null);
      foreach (EntityEntry entityEntry in this.GetNonFixedupEntriesContainingForeignKey(newEntry.EntityKey))
        entityEntry.FixupReferencesByForeignKeys(false, newEntry.EntitySet);
      this.RemoveForeignKeyFromIndex(newEntry.EntityKey);
    }

    internal virtual void AddEntryContainingForeignKeyToIndex(
      EntityReference relatedEnd,
      EntityKey foreignKey,
      EntityEntry entry)
    {
      HashSet<Tuple<EntityReference, EntityEntry>> tupleSet;
      if (!this._danglingForeignKeys.TryGetValue(foreignKey, out tupleSet))
      {
        tupleSet = new HashSet<Tuple<EntityReference, EntityEntry>>();
        this._danglingForeignKeys.Add(foreignKey, tupleSet);
      }
      tupleSet.Add(Tuple.Create<EntityReference, EntityEntry>(relatedEnd, entry));
    }

    [Conditional("DEBUG")]
    internal virtual void AssertEntryDoesNotExistInForeignKeyIndex(EntityEntry entry)
    {
      foreach (Tuple<EntityReference, EntityEntry> tuple in this._danglingForeignKeys.SelectMany<KeyValuePair<EntityKey, HashSet<Tuple<EntityReference, EntityEntry>>>, Tuple<EntityReference, EntityEntry>>((Func<KeyValuePair<EntityKey, HashSet<Tuple<EntityReference, EntityEntry>>>, IEnumerable<Tuple<EntityReference, EntityEntry>>>) (kv => (IEnumerable<Tuple<EntityReference, EntityEntry>>) kv.Value)))
      {
        if (tuple.Item2.State != EntityState.Detached)
        {
          int state = (int) entry.State;
        }
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "This method is compiled only when the compilation symbol DEBUG is defined")]
    [Conditional("DEBUG")]
    internal virtual void AssertAllForeignKeyIndexEntriesAreValid()
    {
      if (this.GetMaxEntityEntriesForDetectChanges() > 100)
        return;
      HashSet<ObjectStateEntry> objectStateEntrySet = new HashSet<ObjectStateEntry>(this.GetObjectStateEntriesInternal(~EntityState.Detached));
      foreach (Tuple<EntityReference, EntityEntry> tuple in this._danglingForeignKeys.SelectMany<KeyValuePair<EntityKey, HashSet<Tuple<EntityReference, EntityEntry>>>, Tuple<EntityReference, EntityEntry>>((Func<KeyValuePair<EntityKey, HashSet<Tuple<EntityReference, EntityEntry>>>, IEnumerable<Tuple<EntityReference, EntityEntry>>>) (kv => (IEnumerable<Tuple<EntityReference, EntityEntry>>) kv.Value)))
        ;
    }

    internal virtual void RemoveEntryFromForeignKeyIndex(
      EntityReference relatedEnd,
      EntityKey foreignKey,
      EntityEntry entry)
    {
      HashSet<Tuple<EntityReference, EntityEntry>> tupleSet;
      if (!this._danglingForeignKeys.TryGetValue(foreignKey, out tupleSet))
        return;
      tupleSet.Remove(Tuple.Create<EntityReference, EntityEntry>(relatedEnd, entry));
    }

    internal virtual void RemoveForeignKeyFromIndex(EntityKey foreignKey)
    {
      this._danglingForeignKeys.Remove(foreignKey);
    }

    internal virtual IEnumerable<EntityEntry> GetNonFixedupEntriesContainingForeignKey(
      EntityKey foreignKey)
    {
      HashSet<Tuple<EntityReference, EntityEntry>> source;
      if (this._danglingForeignKeys.TryGetValue(foreignKey, out source))
        return (IEnumerable<EntityEntry>) source.Select<Tuple<EntityReference, EntityEntry>, EntityEntry>((Func<Tuple<EntityReference, EntityEntry>, EntityEntry>) (e => e.Item2)).ToList<EntityEntry>();
      return Enumerable.Empty<EntityEntry>();
    }

    internal virtual void RememberEntryWithConceptualNull(EntityEntry entry)
    {
      if (this._entriesWithConceptualNulls == null)
        this._entriesWithConceptualNulls = new HashSet<EntityEntry>();
      this._entriesWithConceptualNulls.Add(entry);
    }

    internal virtual bool SomeEntryWithConceptualNullExists()
    {
      if (this._entriesWithConceptualNulls != null)
        return this._entriesWithConceptualNulls.Count != 0;
      return false;
    }

    internal virtual bool EntryHasConceptualNull(EntityEntry entry)
    {
      if (this._entriesWithConceptualNulls != null)
        return this._entriesWithConceptualNulls.Contains(entry);
      return false;
    }

    internal virtual void ForgetEntryWithConceptualNull(EntityEntry entry, bool resetAllKeys)
    {
      if (entry.IsKeyEntry || this._entriesWithConceptualNulls == null || (!this._entriesWithConceptualNulls.Remove(entry) || !entry.RelationshipManager.HasRelationships))
        return;
      foreach (RelatedEnd relationship in entry.RelationshipManager.Relationships)
      {
        EntityReference entityReference = relationship as EntityReference;
        if (entityReference != null && ForeignKeyFactory.IsConceptualNullKey(entityReference.CachedForeignKey))
        {
          if (resetAllKeys)
          {
            entityReference.SetCachedForeignKey((EntityKey) null, (EntityEntry) null);
          }
          else
          {
            this._entriesWithConceptualNulls.Add(entry);
            break;
          }
        }
      }
    }

    internal virtual void PromoteKeyEntryInitialization(
      ObjectContext contextToAttach,
      EntityEntry keyEntry,
      IEntityWrapper wrappedEntity,
      bool replacingEntry)
    {
      StateManagerTypeMetadata managerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(wrappedEntity.IdentityType, (EntitySet) keyEntry.EntitySet);
      this.ValidateProxyType(wrappedEntity);
      keyEntry.PromoteKeyEntry(wrappedEntity, managerTypeMetadata);
      this.AddEntryToKeylessStore(keyEntry);
      if (replacingEntry)
        wrappedEntity.SetChangeTracker((IEntityChangeTracker) null);
      wrappedEntity.SetChangeTracker((IEntityChangeTracker) keyEntry);
      if (contextToAttach != null)
        wrappedEntity.AttachContext(contextToAttach, (EntitySet) keyEntry.EntitySet, MergeOption.AppendOnly);
      wrappedEntity.TakeSnapshot(keyEntry);
      this.OnObjectStateManagerChanged(CollectionChangeAction.Add, keyEntry.Entity);
    }

    internal virtual void PromoteKeyEntry(
      EntityEntry keyEntry,
      IEntityWrapper wrappedEntity,
      bool replacingEntry,
      bool setIsLoaded,
      bool keyEntryInitialized)
    {
      if (!keyEntryInitialized)
        this.PromoteKeyEntryInitialization((ObjectContext) null, keyEntry, wrappedEntity, replacingEntry);
      bool flag = true;
      try
      {
        foreach (RelationshipEntry relationshipEntry in this.CopyOfRelationshipsByKey(keyEntry.EntityKey))
        {
          if (relationshipEntry.State != EntityState.Deleted)
          {
            AssociationEndMember associationEndMember = keyEntry.GetAssociationEndMember(relationshipEntry);
            AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(associationEndMember);
            EntityEntry endOfRelationship = keyEntry.GetOtherEndOfRelationship(relationshipEntry);
            ObjectStateManager.AddEntityToCollectionOrReference(MergeOption.AppendOnly, wrappedEntity, associationEndMember, endOfRelationship.WrappedEntity, otherAssociationEnd, setIsLoaded, true, true);
          }
        }
        this.FixupReferencesByForeignKeys(keyEntry, false);
        flag = false;
      }
      finally
      {
        if (flag)
        {
          keyEntry.DetachObjectStateManagerFromEntity();
          this.RemoveEntryFromKeylessStore(wrappedEntity);
          keyEntry.DegradeEntry();
        }
      }
      if (!this.TransactionManager.IsAttachTracking)
        return;
      this.TransactionManager.PromotedKeyEntries.Add(wrappedEntity.Entity, keyEntry);
    }

    internal virtual void TrackPromotedRelationship(
      RelatedEnd relatedEnd,
      IEntityWrapper wrappedEntity)
    {
      IList<IEntityWrapper> entityWrapperList;
      if (!this.TransactionManager.PromotedRelationships.TryGetValue(relatedEnd, out entityWrapperList))
      {
        entityWrapperList = (IList<IEntityWrapper>) new List<IEntityWrapper>();
        this.TransactionManager.PromotedRelationships.Add(relatedEnd, entityWrapperList);
      }
      entityWrapperList.Add(wrappedEntity);
    }

    internal virtual void DegradePromotedRelationships()
    {
      foreach (KeyValuePair<RelatedEnd, IList<IEntityWrapper>> promotedRelationship in this.TransactionManager.PromotedRelationships)
      {
        foreach (IEntityWrapper wrappedEntity in (IEnumerable<IEntityWrapper>) promotedRelationship.Value)
        {
          if (promotedRelationship.Key.RemoveFromCache(wrappedEntity, false, false))
            promotedRelationship.Key.OnAssociationChanged(CollectionChangeAction.Remove, wrappedEntity.Entity);
        }
      }
    }

    internal static void AddEntityToCollectionOrReference(
      MergeOption mergeOption,
      IEntityWrapper wrappedSource,
      AssociationEndMember sourceMember,
      IEntityWrapper wrappedTarget,
      AssociationEndMember targetMember,
      bool setIsLoaded,
      bool relationshipAlreadyExists,
      bool inKeyEntryPromotion)
    {
      RelatedEnd relatedEndInternal = wrappedSource.RelationshipManager.GetRelatedEndInternal(sourceMember.DeclaringType.FullName, targetMember.Name);
      if (targetMember.RelationshipMultiplicity != RelationshipMultiplicity.Many)
      {
        EntityReference entityReference = (EntityReference) relatedEndInternal;
        switch (mergeOption)
        {
          case MergeOption.AppendOnly:
            if (inKeyEntryPromotion && !entityReference.IsEmpty() && !object.ReferenceEquals(entityReference.ReferenceValue.Entity, wrappedTarget.Entity))
              throw new InvalidOperationException(Strings.ObjectStateManager_EntityConflictsWithKeyEntry);
            break;
          case MergeOption.OverwriteChanges:
          case MergeOption.PreserveChanges:
            IEntityWrapper referenceValue = entityReference.ReferenceValue;
            if (referenceValue != null && referenceValue.Entity != null && referenceValue != wrappedTarget)
            {
              RelationshipEntry objectStateManager = relatedEndInternal.FindRelationshipEntryInObjectStateManager(referenceValue);
              relatedEndInternal.RemoveAll();
              if (objectStateManager != null && objectStateManager.State == EntityState.Deleted)
              {
                objectStateManager.AcceptChanges();
                break;
              }
              break;
            }
            break;
        }
      }
      RelatedEnd relatedEnd = (RelatedEnd) null;
      if (mergeOption == MergeOption.NoTracking)
      {
        relatedEnd = relatedEndInternal.GetOtherEndOfRelationship(wrappedTarget);
        if (relatedEnd.IsLoaded)
          throw new InvalidOperationException(Strings.Collections_CannotFillTryDifferentMergeOption((object) relatedEnd.SourceRoleName, (object) relatedEnd.RelationshipName));
      }
      if (relatedEnd == null)
        relatedEnd = relatedEndInternal.GetOtherEndOfRelationship(wrappedTarget);
      relatedEndInternal.Add(wrappedTarget, true, true, relationshipAlreadyExists, true, true);
      ObjectStateManager.UpdateRelatedEnd(relatedEndInternal, wrappedTarget, setIsLoaded, mergeOption);
      ObjectStateManager.UpdateRelatedEnd(relatedEnd, wrappedSource, setIsLoaded, mergeOption);
      if (!inKeyEntryPromotion || !wrappedSource.Context.ObjectStateManager.TransactionManager.IsAttachTracking)
        return;
      wrappedSource.Context.ObjectStateManager.TrackPromotedRelationship(relatedEndInternal, wrappedTarget);
      wrappedSource.Context.ObjectStateManager.TrackPromotedRelationship(relatedEnd, wrappedSource);
    }

    private static void UpdateRelatedEnd(
      RelatedEnd relatedEnd,
      IEntityWrapper wrappedRelatedEntity,
      bool setIsLoaded,
      MergeOption mergeOption)
    {
      AssociationEndMember toEndMember = (AssociationEndMember) relatedEnd.ToEndMember;
      if (toEndMember.RelationshipMultiplicity != RelationshipMultiplicity.One && toEndMember.RelationshipMultiplicity != RelationshipMultiplicity.ZeroOrOne)
        return;
      if (setIsLoaded)
        relatedEnd.IsLoaded = true;
      if (mergeOption != MergeOption.NoTracking)
        return;
      EntityKey entityKey = wrappedRelatedEntity.EntityKey;
      if ((object) entityKey == null)
        throw new InvalidOperationException(Strings.EntityKey_UnexpectedNull);
      ((EntityReference) relatedEnd).DetachedEntityKey = entityKey;
    }

    internal virtual int UpdateRelationships(
      ObjectContext context,
      MergeOption mergeOption,
      AssociationSet associationSet,
      AssociationEndMember sourceMember,
      IEntityWrapper wrappedSource,
      AssociationEndMember targetMember,
      IList targets,
      bool setIsLoaded)
    {
      int num = 0;
      EntityKey entityKey1 = wrappedSource.EntityKey;
      context.ObjectStateManager.TransactionManager.BeginGraphUpdate();
      try
      {
        if (targets != null)
        {
          if (mergeOption == MergeOption.NoTracking)
          {
            RelatedEnd relatedEndInternal = wrappedSource.RelationshipManager.GetRelatedEndInternal(sourceMember.DeclaringType.FullName, targetMember.Name);
            if (!relatedEndInternal.IsEmpty())
              throw new InvalidOperationException(Strings.Collections_CannotFillTryDifferentMergeOption((object) relatedEndInternal.SourceRoleName, (object) relatedEndInternal.RelationshipName));
          }
          foreach (object target in (IEnumerable) targets)
          {
            IEntityWrapper entityWrapper = target as IEntityWrapper ?? this.EntityWrapperFactory.WrapEntityUsingContext(target, context);
            ++num;
            if (mergeOption == MergeOption.NoTracking)
            {
              ObjectStateManager.AddEntityToCollectionOrReference(MergeOption.NoTracking, wrappedSource, sourceMember, entityWrapper, targetMember, setIsLoaded, true, false);
            }
            else
            {
              ObjectStateManager objectStateManager = context.ObjectStateManager;
              EntityKey entityKey2 = entityWrapper.EntityKey;
              EntityState newEntryState;
              if (!ObjectStateManager.TryUpdateExistingRelationships(context, mergeOption, associationSet, sourceMember, entityKey1, wrappedSource, targetMember, entityKey2, setIsLoaded, out newEntryState))
              {
                bool flag = true;
                switch (sourceMember.RelationshipMultiplicity)
                {
                  case RelationshipMultiplicity.ZeroOrOne:
                  case RelationshipMultiplicity.One:
                    flag = !ObjectStateManager.TryUpdateExistingRelationships(context, mergeOption, associationSet, targetMember, entityKey2, entityWrapper, sourceMember, entityKey1, setIsLoaded, out newEntryState);
                    break;
                }
                if (flag)
                {
                  if (newEntryState != EntityState.Deleted)
                  {
                    ObjectStateManager.AddEntityToCollectionOrReference(mergeOption, wrappedSource, sourceMember, entityWrapper, targetMember, setIsLoaded, false, false);
                  }
                  else
                  {
                    RelationshipWrapper wrapper = new RelationshipWrapper(associationSet, sourceMember.Name, entityKey1, targetMember.Name, entityKey2);
                    objectStateManager.AddNewRelation(wrapper, EntityState.Deleted);
                  }
                }
              }
            }
          }
        }
        if (num == 0)
          ObjectStateManager.EnsureCollectionNotNull(sourceMember, wrappedSource, targetMember);
      }
      finally
      {
        context.ObjectStateManager.TransactionManager.EndGraphUpdate();
      }
      return num;
    }

    private static void EnsureCollectionNotNull(
      AssociationEndMember sourceMember,
      IEntityWrapper wrappedSource,
      AssociationEndMember targetMember)
    {
      RelatedEnd relatedEndInternal = wrappedSource.RelationshipManager.GetRelatedEndInternal(sourceMember.DeclaringType.FullName, targetMember.Name);
      AssociationEndMember toEndMember = (AssociationEndMember) relatedEndInternal.ToEndMember;
      if (toEndMember == null || toEndMember.RelationshipMultiplicity != RelationshipMultiplicity.Many || !relatedEndInternal.TargetAccessor.HasProperty)
        return;
      wrappedSource.EnsureCollectionNotNull(relatedEndInternal);
    }

    internal virtual void RemoveRelationships(
      MergeOption mergeOption,
      AssociationSet associationSet,
      EntityKey sourceKey,
      AssociationEndMember sourceMember)
    {
      List<RelationshipEntry> relationshipEntryList = new List<RelationshipEntry>(16);
      switch (mergeOption)
      {
        case MergeOption.OverwriteChanges:
          using (EntityEntry.RelationshipEndEnumerator enumerator = this.FindRelationshipsByKey(sourceKey).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              RelationshipEntry current = enumerator.Current;
              if (current.IsSameAssociationSetAndRole(associationSet, sourceMember, sourceKey))
                relationshipEntryList.Add(current);
            }
            break;
          }
        case MergeOption.PreserveChanges:
          using (EntityEntry.RelationshipEndEnumerator enumerator = this.FindRelationshipsByKey(sourceKey).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              RelationshipEntry current = enumerator.Current;
              if (current.IsSameAssociationSetAndRole(associationSet, sourceMember, sourceKey) && current.State != EntityState.Added)
                relationshipEntryList.Add(current);
            }
            break;
          }
      }
      foreach (RelationshipEntry relationshipToRemove in relationshipEntryList)
        ObjectStateManager.RemoveRelatedEndsAndDetachRelationship(relationshipToRemove, true);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal static bool TryUpdateExistingRelationships(
      ObjectContext context,
      MergeOption mergeOption,
      AssociationSet associationSet,
      AssociationEndMember sourceMember,
      EntityKey sourceKey,
      IEntityWrapper wrappedSource,
      AssociationEndMember targetMember,
      EntityKey targetKey,
      bool setIsLoaded,
      out EntityState newEntryState)
    {
      newEntryState = EntityState.Unchanged;
      if (associationSet.ElementType.IsForeignKey)
        return true;
      bool flag = true;
      ObjectStateManager objectStateManager = context.ObjectStateManager;
      List<RelationshipEntry> relationshipEntryList1 = (List<RelationshipEntry>) null;
      List<RelationshipEntry> relationshipEntryList2 = (List<RelationshipEntry>) null;
      foreach (RelationshipEntry relationshipEntry in objectStateManager.FindRelationshipsByKey(sourceKey))
      {
        if (relationshipEntry.IsSameAssociationSetAndRole(associationSet, sourceMember, sourceKey))
        {
          if (targetKey == relationshipEntry.RelationshipWrapper.GetOtherEntityKey(sourceKey))
          {
            if (relationshipEntryList2 == null)
              relationshipEntryList2 = new List<RelationshipEntry>(16);
            relationshipEntryList2.Add(relationshipEntry);
          }
          else
          {
            switch (targetMember.RelationshipMultiplicity)
            {
              case RelationshipMultiplicity.ZeroOrOne:
              case RelationshipMultiplicity.One:
                switch (mergeOption)
                {
                  case MergeOption.AppendOnly:
                    if (relationshipEntry.State != EntityState.Deleted)
                    {
                      flag = false;
                      continue;
                    }
                    continue;
                  case MergeOption.OverwriteChanges:
                    if (relationshipEntryList1 == null)
                      relationshipEntryList1 = new List<RelationshipEntry>(16);
                    relationshipEntryList1.Add(relationshipEntry);
                    continue;
                  case MergeOption.PreserveChanges:
                    switch (relationshipEntry.State)
                    {
                      case EntityState.Unchanged:
                        if (relationshipEntryList1 == null)
                          relationshipEntryList1 = new List<RelationshipEntry>(16);
                        relationshipEntryList1.Add(relationshipEntry);
                        continue;
                      case EntityState.Added:
                        newEntryState = EntityState.Deleted;
                        continue;
                      case EntityState.Deleted:
                        newEntryState = EntityState.Deleted;
                        if (relationshipEntryList1 == null)
                          relationshipEntryList1 = new List<RelationshipEntry>(16);
                        relationshipEntryList1.Add(relationshipEntry);
                        continue;
                      default:
                        continue;
                    }
                  default:
                    continue;
                }
              default:
                continue;
            }
          }
        }
      }
      if (relationshipEntryList1 != null)
      {
        foreach (RelationshipEntry relationshipToRemove in relationshipEntryList1)
        {
          if (relationshipToRemove.State != EntityState.Detached)
            ObjectStateManager.RemoveRelatedEndsAndDetachRelationship(relationshipToRemove, setIsLoaded);
        }
      }
      if (relationshipEntryList2 != null)
      {
        foreach (RelationshipEntry relationshipEntry in relationshipEntryList2)
        {
          flag = false;
          switch (mergeOption)
          {
            case MergeOption.OverwriteChanges:
              if (relationshipEntry.State == EntityState.Added)
              {
                relationshipEntry.AcceptChanges();
                continue;
              }
              if (relationshipEntry.State == EntityState.Deleted)
              {
                EntityEntry entityEntry = objectStateManager.GetEntityEntry(targetKey);
                if (entityEntry.State != EntityState.Deleted)
                {
                  if (!entityEntry.IsKeyEntry)
                    ObjectStateManager.AddEntityToCollectionOrReference(mergeOption, wrappedSource, sourceMember, entityEntry.WrappedEntity, targetMember, setIsLoaded, true, false);
                  relationshipEntry.RevertDelete();
                  continue;
                }
                continue;
              }
              continue;
            case MergeOption.PreserveChanges:
              if (relationshipEntry.State == EntityState.Added)
              {
                relationshipEntry.AcceptChanges();
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      return !flag;
    }

    internal static void RemoveRelatedEndsAndDetachRelationship(
      RelationshipEntry relationshipToRemove,
      bool setIsLoaded)
    {
      if (setIsLoaded)
        ObjectStateManager.UnloadReferenceRelatedEnds(relationshipToRemove);
      if (relationshipToRemove.State != EntityState.Deleted)
        relationshipToRemove.Delete();
      if (relationshipToRemove.State == EntityState.Detached)
        return;
      relationshipToRemove.AcceptChanges();
    }

    private static void UnloadReferenceRelatedEnds(RelationshipEntry relationshipEntry)
    {
      ObjectStateManager objectStateManager = relationshipEntry.ObjectStateManager;
      ReadOnlyMetadataCollection<AssociationEndMember> associationEndMembers = relationshipEntry.RelationshipWrapper.AssociationEndMembers;
      ObjectStateManager.UnloadReferenceRelatedEnds(objectStateManager, relationshipEntry, relationshipEntry.RelationshipWrapper.GetEntityKey(0), associationEndMembers[1].Name);
      ObjectStateManager.UnloadReferenceRelatedEnds(objectStateManager, relationshipEntry, relationshipEntry.RelationshipWrapper.GetEntityKey(1), associationEndMembers[0].Name);
    }

    private static void UnloadReferenceRelatedEnds(
      ObjectStateManager cache,
      RelationshipEntry relationshipEntry,
      EntityKey sourceEntityKey,
      string targetRoleName)
    {
      EntityEntry entityEntry = cache.GetEntityEntry(sourceEntityKey);
      if (entityEntry.WrappedEntity.Entity == null)
        return;
      EntityReference relatedEndInternal = entityEntry.WrappedEntity.RelationshipManager.GetRelatedEndInternal(((AssociationSet) relationshipEntry.EntitySet).ElementType.FullName, targetRoleName) as EntityReference;
      if (relatedEndInternal == null)
        return;
      relatedEndInternal.IsLoaded = false;
    }

    internal virtual EntityEntry AttachEntry(
      EntityKey entityKey,
      IEntityWrapper wrappedObject,
      EntitySet entitySet)
    {
      StateManagerTypeMetadata managerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(wrappedObject.IdentityType, entitySet);
      this.ValidateProxyType(wrappedObject);
      this.CheckKeyMatchesEntity(wrappedObject, entityKey, entitySet, true);
      if (!wrappedObject.OwnsRelationshipManager)
        wrappedObject.RelationshipManager.ClearRelatedEndWrappers();
      EntityEntry entry = new EntityEntry(wrappedObject, entityKey, entitySet, this, managerTypeMetadata, EntityState.Unchanged);
      entry.AttachObjectStateManagerToEntity();
      this.AddEntityEntryToDictionary(entry, entry.State);
      this.OnObjectStateManagerChanged(CollectionChangeAction.Add, entry.Entity);
      return entry;
    }

    private void CheckKeyMatchesEntity(
      IEntityWrapper wrappedEntity,
      EntityKey entityKey,
      EntitySet entitySetForType,
      bool forAttach)
    {
      EntitySet entitySet = entityKey.GetEntitySet(this.MetadataWorkspace);
      if (entitySet == null)
        throw new InvalidOperationException(Strings.ObjectStateManager_InvalidKey);
      entityKey.ValidateEntityKey(this._metadataWorkspace, entitySet);
      StateManagerTypeMetadata managerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(wrappedEntity.IdentityType, entitySetForType);
      for (int index = 0; index < entitySet.ElementType.KeyMembers.Count; ++index)
      {
        EdmMember keyMember = entitySet.ElementType.KeyMembers[index];
        int clayerMemberName = managerTypeMetadata.GetOrdinalforCLayerMemberName(keyMember.Name);
        if (clayerMemberName < 0)
          throw new InvalidOperationException(Strings.ObjectStateManager_InvalidKey);
        object x = managerTypeMetadata.Member(clayerMemberName).GetValue(wrappedEntity.Entity);
        object valueByName = entityKey.FindValueByName(keyMember.Name);
        if (!ByValueEqualityComparer.Default.Equals(x, valueByName))
          throw new InvalidOperationException(forAttach ? Strings.ObjectStateManager_KeyPropertyDoesntMatchValueInKeyForAttach : Strings.ObjectStateManager_KeyPropertyDoesntMatchValueInKey);
      }
    }

    internal virtual RelationshipEntry AddNewRelation(
      RelationshipWrapper wrapper,
      EntityState desiredState)
    {
      RelationshipEntry relationshipEntry = new RelationshipEntry(this, desiredState, wrapper);
      this.AddRelationshipEntryToDictionary(relationshipEntry, desiredState);
      this.AddRelationshipToLookup(relationshipEntry);
      return relationshipEntry;
    }

    internal virtual RelationshipEntry AddRelation(
      RelationshipWrapper wrapper,
      EntityState desiredState)
    {
      RelationshipEntry relationshipEntry = this.FindRelationship(wrapper);
      if (relationshipEntry == null)
        relationshipEntry = this.AddNewRelation(wrapper, desiredState);
      else if (EntityState.Deleted != relationshipEntry.State)
      {
        if (EntityState.Unchanged == desiredState)
          relationshipEntry.AcceptChanges();
        else if (EntityState.Deleted == desiredState)
        {
          relationshipEntry.AcceptChanges();
          relationshipEntry.Delete(false);
        }
      }
      else if (EntityState.Deleted != desiredState)
        relationshipEntry.RevertDelete();
      return relationshipEntry;
    }

    private void AddRelationshipToLookup(RelationshipEntry relationship)
    {
      this.AddRelationshipEndToLookup(relationship.RelationshipWrapper.Key0, relationship);
      if (relationship.RelationshipWrapper.Key0.Equals(relationship.RelationshipWrapper.Key1))
        return;
      this.AddRelationshipEndToLookup(relationship.RelationshipWrapper.Key1, relationship);
    }

    private void AddRelationshipEndToLookup(EntityKey key, RelationshipEntry relationship)
    {
      this.GetEntityEntry(key).AddRelationshipEnd(relationship);
    }

    private void DeleteRelationshipFromLookup(RelationshipEntry relationship)
    {
      this.DeleteRelationshipEndFromLookup(relationship.RelationshipWrapper.Key0, relationship);
      if (relationship.RelationshipWrapper.Key0.Equals(relationship.RelationshipWrapper.Key1))
        return;
      this.DeleteRelationshipEndFromLookup(relationship.RelationshipWrapper.Key1, relationship);
    }

    private void DeleteRelationshipEndFromLookup(EntityKey key, RelationshipEntry relationship)
    {
      this.GetEntityEntry(key).RemoveRelationshipEnd(relationship);
    }

    internal virtual RelationshipEntry FindRelationship(
      RelationshipSet relationshipSet,
      KeyValuePair<string, EntityKey> roleAndKey1,
      KeyValuePair<string, EntityKey> roleAndKey2)
    {
      if ((object) roleAndKey1.Value == null || (object) roleAndKey2.Value == null)
        return (RelationshipEntry) null;
      return this.FindRelationship(new RelationshipWrapper((AssociationSet) relationshipSet, roleAndKey1, roleAndKey2));
    }

    internal virtual RelationshipEntry FindRelationship(
      RelationshipWrapper relationshipWrapper)
    {
      RelationshipEntry relationshipEntry = (RelationshipEntry) null;
      if ((this._unchangedRelationshipStore == null || !this._unchangedRelationshipStore.TryGetValue(relationshipWrapper, out relationshipEntry)) && (this._deletedRelationshipStore == null || !this._deletedRelationshipStore.TryGetValue(relationshipWrapper, out relationshipEntry)) && this._addedRelationshipStore != null)
        this._addedRelationshipStore.TryGetValue(relationshipWrapper, out relationshipEntry);
      return relationshipEntry;
    }

    internal virtual RelationshipEntry DeleteRelationship(
      RelationshipSet relationshipSet,
      KeyValuePair<string, EntityKey> roleAndKey1,
      KeyValuePair<string, EntityKey> roleAndKey2)
    {
      RelationshipEntry relationship = this.FindRelationship(relationshipSet, roleAndKey1, roleAndKey2);
      relationship?.Delete(false);
      return relationship;
    }

    internal virtual void DeleteKeyEntry(EntityEntry keyEntry)
    {
      if (keyEntry == null || !keyEntry.IsKeyEntry)
        return;
      this.ChangeState(keyEntry, keyEntry.State, EntityState.Detached);
    }

    internal virtual RelationshipEntry[] CopyOfRelationshipsByKey(EntityKey key)
    {
      return this.FindRelationshipsByKey(key).ToArray();
    }

    internal virtual EntityEntry.RelationshipEndEnumerable FindRelationshipsByKey(
      EntityKey key)
    {
      return new EntityEntry.RelationshipEndEnumerable(this.FindEntityEntry(key));
    }

    IEnumerable<IEntityStateEntry> IEntityStateManager.FindRelationshipsByKey(
      EntityKey key)
    {
      return (IEnumerable<IEntityStateEntry>) this.FindRelationshipsByKey(key);
    }

    [Conditional("DEBUG")]
    private void ValidateKeylessEntityStore()
    {
      Dictionary<EntityKey, EntityEntry>[] dictionaryArray = new Dictionary<EntityKey, EntityEntry>[4]
      {
        this._unchangedEntityStore,
        this._modifiedEntityStore,
        this._addedEntityStore,
        this._deletedEntityStore
      };
      if (this._keylessEntityStore != null && this._keylessEntityStore.Count == ((IEnumerable<Dictionary<EntityKey, EntityEntry>>) dictionaryArray).Sum<Dictionary<EntityKey, EntityEntry>>((Func<Dictionary<EntityKey, EntityEntry>, int>) (s =>
      {
        if (s != null)
          return s.Count;
        return 0;
      })))
        return;
      if (this._keylessEntityStore != null)
      {
        foreach (EntityEntry entityEntry1 in this._keylessEntityStore.Values)
        {
          bool flag1 = false;
          EntityEntry entityEntry2;
          if (this._addedEntityStore != null)
            flag1 = this._addedEntityStore.TryGetValue(entityEntry1.EntityKey, out entityEntry2);
          if (this._modifiedEntityStore != null)
            flag1 |= this._modifiedEntityStore.TryGetValue(entityEntry1.EntityKey, out entityEntry2);
          if (this._deletedEntityStore != null)
            flag1 |= this._deletedEntityStore.TryGetValue(entityEntry1.EntityKey, out entityEntry2);
          if (this._unchangedEntityStore != null)
          {
            bool flag2 = flag1 | this._unchangedEntityStore.TryGetValue(entityEntry1.EntityKey, out entityEntry2);
          }
        }
      }
      foreach (Dictionary<EntityKey, EntityEntry> dictionary in dictionaryArray)
      {
        if (dictionary != null)
        {
          foreach (EntityEntry entityEntry1 in dictionary.Values)
          {
            if (entityEntry1.Entity != null && !(entityEntry1.Entity is IEntityWithKey))
            {
              EntityEntry entityEntry2;
              this._keylessEntityStore.TryGetValue(entityEntry1.Entity, out entityEntry2);
            }
          }
        }
      }
    }

    private bool TryGetEntryFromKeylessStore(object entity, out EntityEntry entryRef)
    {
      entryRef = (EntityEntry) null;
      if (entity == null)
        return false;
      if (this._keylessEntityStore != null && this._keylessEntityStore.TryGetValue(entity, out entryRef))
        return true;
      entryRef = (EntityEntry) null;
      return false;
    }

    /// <summary>
    /// Returns a collection of <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> objects for objects or relationships with the given state.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> objects in the given
    /// <see cref="T:System.Data.Entity.EntityState" />
    /// .
    /// </returns>
    /// <param name="state">
    /// An <see cref="T:System.Data.Entity.EntityState" /> used to filter the returned
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// objects.
    /// </param>
    /// <exception cref="T:System.ArgumentException">
    /// When  state  is <see cref="F:System.Data.Entity.EntityState.Detached" />.
    /// </exception>
    public virtual IEnumerable<ObjectStateEntry> GetObjectStateEntries(
      EntityState state)
    {
      if ((EntityState.Detached & state) != (EntityState) 0)
        throw new ArgumentException(Strings.ObjectStateManager_DetachedObjectStateEntriesDoesNotExistInObjectStateManager);
      return this.GetObjectStateEntriesInternal(state);
    }

    IEnumerable<IEntityStateEntry> IEntityStateManager.GetEntityStateEntries(
      EntityState state)
    {
      foreach (ObjectStateEntry objectStateEntry in this.GetObjectStateEntriesInternal(state))
        yield return (IEntityStateEntry) objectStateEntry;
    }

    internal virtual bool HasChanges()
    {
      if (this._addedRelationshipStore != null && this._addedRelationshipStore.Count > 0 || this._addedEntityStore != null && this._addedEntityStore.Count > 0 || (this._modifiedEntityStore != null && this._modifiedEntityStore.Count > 0 || this._deletedRelationshipStore != null && this._deletedRelationshipStore.Count > 0))
        return true;
      if (this._deletedEntityStore != null)
        return this._deletedEntityStore.Count > 0;
      return false;
    }

    internal virtual int GetObjectStateEntriesCount(EntityState state)
    {
      int num = 0;
      if ((EntityState.Added & state) != (EntityState) 0)
        num = num + (this._addedRelationshipStore != null ? this._addedRelationshipStore.Count : 0) + (this._addedEntityStore != null ? this._addedEntityStore.Count : 0);
      if ((EntityState.Modified & state) != (EntityState) 0)
        num += this._modifiedEntityStore != null ? this._modifiedEntityStore.Count : 0;
      if ((EntityState.Deleted & state) != (EntityState) 0)
        num = num + (this._deletedRelationshipStore != null ? this._deletedRelationshipStore.Count : 0) + (this._deletedEntityStore != null ? this._deletedEntityStore.Count : 0);
      if ((EntityState.Unchanged & state) != (EntityState) 0)
        num = num + (this._unchangedRelationshipStore != null ? this._unchangedRelationshipStore.Count : 0) + (this._unchangedEntityStore != null ? this._unchangedEntityStore.Count : 0);
      return num;
    }

    private int GetMaxEntityEntriesForDetectChanges()
    {
      int num = 0;
      if (this._addedEntityStore != null)
        num += this._addedEntityStore.Count;
      if (this._modifiedEntityStore != null)
        num += this._modifiedEntityStore.Count;
      if (this._deletedEntityStore != null)
        num += this._deletedEntityStore.Count;
      if (this._unchangedEntityStore != null)
        num += this._unchangedEntityStore.Count;
      return num;
    }

    internal virtual IEnumerable<ObjectStateEntry> GetObjectStateEntriesInternal(
      EntityState state)
    {
      ObjectStateEntry[] objectStateEntryArray = new ObjectStateEntry[this.GetObjectStateEntriesCount(state)];
      int num = 0;
      if ((EntityState.Added & state) != (EntityState) 0 && this._addedRelationshipStore != null)
      {
        foreach (KeyValuePair<RelationshipWrapper, RelationshipEntry> keyValuePair in this._addedRelationshipStore)
          objectStateEntryArray[num++] = (ObjectStateEntry) keyValuePair.Value;
      }
      if ((EntityState.Deleted & state) != (EntityState) 0 && this._deletedRelationshipStore != null)
      {
        foreach (KeyValuePair<RelationshipWrapper, RelationshipEntry> keyValuePair in this._deletedRelationshipStore)
          objectStateEntryArray[num++] = (ObjectStateEntry) keyValuePair.Value;
      }
      if ((EntityState.Unchanged & state) != (EntityState) 0 && this._unchangedRelationshipStore != null)
      {
        foreach (KeyValuePair<RelationshipWrapper, RelationshipEntry> keyValuePair in this._unchangedRelationshipStore)
          objectStateEntryArray[num++] = (ObjectStateEntry) keyValuePair.Value;
      }
      if ((EntityState.Added & state) != (EntityState) 0 && this._addedEntityStore != null)
      {
        foreach (KeyValuePair<EntityKey, EntityEntry> keyValuePair in this._addedEntityStore)
          objectStateEntryArray[num++] = (ObjectStateEntry) keyValuePair.Value;
      }
      if ((EntityState.Modified & state) != (EntityState) 0 && this._modifiedEntityStore != null)
      {
        foreach (KeyValuePair<EntityKey, EntityEntry> keyValuePair in this._modifiedEntityStore)
          objectStateEntryArray[num++] = (ObjectStateEntry) keyValuePair.Value;
      }
      if ((EntityState.Deleted & state) != (EntityState) 0 && this._deletedEntityStore != null)
      {
        foreach (KeyValuePair<EntityKey, EntityEntry> keyValuePair in this._deletedEntityStore)
          objectStateEntryArray[num++] = (ObjectStateEntry) keyValuePair.Value;
      }
      if ((EntityState.Unchanged & state) != (EntityState) 0 && this._unchangedEntityStore != null)
      {
        foreach (KeyValuePair<EntityKey, EntityEntry> keyValuePair in this._unchangedEntityStore)
          objectStateEntryArray[num++] = (ObjectStateEntry) keyValuePair.Value;
      }
      return (IEnumerable<ObjectStateEntry>) objectStateEntryArray;
    }

    private IList<EntityEntry> GetEntityEntriesForDetectChanges()
    {
      if (!this._detectChangesNeeded)
        return (IList<EntityEntry>) null;
      List<EntityEntry> entries = (List<EntityEntry>) null;
      this.GetEntityEntriesForDetectChanges(this._addedEntityStore, ref entries);
      this.GetEntityEntriesForDetectChanges(this._modifiedEntityStore, ref entries);
      this.GetEntityEntriesForDetectChanges(this._deletedEntityStore, ref entries);
      this.GetEntityEntriesForDetectChanges(this._unchangedEntityStore, ref entries);
      if (entries == null)
        this._detectChangesNeeded = false;
      return (IList<EntityEntry>) entries;
    }

    private void GetEntityEntriesForDetectChanges(
      Dictionary<EntityKey, EntityEntry> entityStore,
      ref List<EntityEntry> entries)
    {
      if (entityStore == null)
        return;
      foreach (EntityEntry entityEntry in entityStore.Values)
      {
        if (entityEntry.RequiresAnyChangeTracking)
        {
          if (entries == null)
            entries = new List<EntityEntry>(this.GetMaxEntityEntriesForDetectChanges());
          entries.Add(entityEntry);
        }
      }
    }

    internal virtual void FixupKey(EntityEntry entry)
    {
      EntityKey entityKey = entry.EntityKey;
      EntitySet entitySet = (EntitySet) entry.EntitySet;
      bool keyRelationships = entitySet.HasForeignKeyRelationships;
      bool independentRelationships = entitySet.HasIndependentRelationships;
      if (keyRelationships)
        entry.FixupForeignKeysByReference();
      EntityKey key;
      try
      {
        key = new EntityKey((EntitySet) entry.EntitySet, (IExtendedDataRecord) entry.CurrentValues);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException(Strings.ObjectStateManager_ChangeStateFromAddedWithNullKeyIsInvalid, (Exception) ex);
      }
      EntityEntry entityEntry = this.FindEntityEntry(key);
      if (entityEntry != null)
      {
        if (!entityEntry.IsKeyEntry)
          throw new InvalidOperationException(Strings.ObjectStateManager_CannotFixUpKeyToExistingValues((object) entry.WrappedEntity.IdentityType.FullName));
        key = entityEntry.EntityKey;
      }
      RelationshipEntry[] relationshipEntryArray = (RelationshipEntry[]) null;
      if (independentRelationships)
      {
        relationshipEntryArray = entry.GetRelationshipEnds().ToArray();
        foreach (RelationshipEntry entry1 in relationshipEntryArray)
          this.RemoveObjectStateEntryFromDictionary(entry1, entry1.State);
      }
      this.RemoveObjectStateEntryFromDictionary(entry, EntityState.Added);
      this.ResetEntityKey(entry, key);
      if (independentRelationships)
      {
        entry.UpdateRelationshipEnds(entityKey, entityEntry);
        foreach (RelationshipEntry entry1 in relationshipEntryArray)
          this.AddRelationshipEntryToDictionary(entry1, entry1.State);
      }
      if (entityEntry != null)
      {
        this.PromoteKeyEntry(entityEntry, entry.WrappedEntity, true, false, false);
        entry = entityEntry;
      }
      else
        this.AddEntityEntryToDictionary(entry, EntityState.Unchanged);
      if (!keyRelationships)
        return;
      this.FixupReferencesByForeignKeys(entry, false);
    }

    internal virtual void ReplaceKeyWithTemporaryKey(EntityEntry entry)
    {
      EntityKey entityKey1 = entry.EntityKey;
      EntityKey entityKey2 = new EntityKey(entry.EntitySet);
      RelationshipEntry[] array = entry.GetRelationshipEnds().ToArray();
      foreach (RelationshipEntry entry1 in array)
        this.RemoveObjectStateEntryFromDictionary(entry1, entry1.State);
      this.RemoveObjectStateEntryFromDictionary(entry, entry.State);
      this.ResetEntityKey(entry, entityKey2);
      entry.UpdateRelationshipEnds(entityKey1, (EntityEntry) null);
      foreach (RelationshipEntry entry1 in array)
        this.AddRelationshipEntryToDictionary(entry1, entry1.State);
      this.AddEntityEntryToDictionary(entry, EntityState.Added);
    }

    private void ResetEntityKey(EntityEntry entry, EntityKey value)
    {
      EntityKey entityKey = entry.WrappedEntity.EntityKey;
      if (!(entityKey == (EntityKey) null))
      {
        if (!value.Equals(entityKey))
        {
          try
          {
            this._inRelationshipFixup = true;
            entry.WrappedEntity.EntityKey = value;
            IEntityWrapper wrappedEntity = entry.WrappedEntity;
            if (wrappedEntity.EntityKey != value)
              throw new InvalidOperationException(Strings.EntityKey_DoesntMatchKeyOnEntity((object) wrappedEntity.Entity.GetType().FullName));
          }
          finally
          {
            this._inRelationshipFixup = false;
          }
          entry.EntityKey = value;
          return;
        }
      }
      throw new InvalidOperationException(Strings.ObjectStateManager_AcceptChangesEntityKeyIsNotValid);
    }

    /// <summary>
    /// Changes state of the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for a specific object to the specified  entityState .
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the supplied  entity .
    /// </returns>
    /// <param name="entity">The object for which the state must be changed.</param>
    /// <param name="entityState">The new state of the object.</param>
    /// <exception cref="T:System.ArgumentNullException">When  entity  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the object is not detached and does not have an entry in the state manager
    /// or when you try to change the state to <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// from any other <see cref="T:System.Data.Entity.EntityState." />
    /// or when  state  is not a valid <see cref="T:System.Data.Entity.EntityState" /> value.
    /// </exception>
    public virtual ObjectStateEntry ChangeObjectState(
      object entity,
      EntityState entityState)
    {
      Check.NotNull<object>(entity, nameof (entity));
      EntityUtil.CheckValidStateForChangeEntityState(entityState);
      EntityEntry entityEntry = (EntityEntry) null;
      this.TransactionManager.BeginLocalPublicAPI();
      try
      {
        EntityKey key = entity as EntityKey;
        entityEntry = key != (EntityKey) null ? this.FindEntityEntry(key) : this.FindEntityEntry(entity);
        if (entityEntry == null)
        {
          if (entityState == EntityState.Detached)
            return (ObjectStateEntry) null;
          throw new InvalidOperationException(Strings.ObjectStateManager_NoEntryExistsForObject((object) entity.GetType().FullName));
        }
        entityEntry.ChangeObjectState(entityState);
      }
      finally
      {
        this.TransactionManager.EndLocalPublicAPI();
      }
      return (ObjectStateEntry) entityEntry;
    }

    /// <summary>Changes the state of the relationship between two entity objects that is specified based on the two related objects and the name of the navigation property.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the relationship that was changed.
    /// </returns>
    /// <param name="sourceEntity">
    /// The object instance or <see cref="T:System.Data.Entity.Core.EntityKey" /> of the source entity at one end of the relationship.
    /// </param>
    /// <param name="targetEntity">
    /// The object instance or <see cref="T:System.Data.Entity.Core.EntityKey" /> of the target entity at the other end of the relationship.
    /// </param>
    /// <param name="navigationProperty">The name of the navigation property on  source  that returns the specified  target .</param>
    /// <param name="relationshipState">
    /// The requested <see cref="T:System.Data.Entity.EntityState" /> of the specified relationship.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">When  source  or  target  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// When trying to change the state of the relationship to a state other than
    ///     <see cref="F:System.Data.Entity.EntityState.Deleted" /> or <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// when either  source  or  target  is in a <see cref="F:System.Data.Entity.EntityState.Deleted" /> state
    /// or when you try to change the state of the relationship to a state other than
    /// <see cref="F:System.Data.Entity.EntityState.Added" /> or <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// when either  source  or  target  is in an <see ref="F:System.Data.Entity.EntityState.Added" /> state
    /// or when  state  is not a valid <see cref="T:System.Data.Entity.EntityState" /> value
    /// </exception>
    public virtual ObjectStateEntry ChangeRelationshipState(
      object sourceEntity,
      object targetEntity,
      string navigationProperty,
      EntityState relationshipState)
    {
      EntityEntry sourceEntry;
      EntityEntry targetEntry;
      this.VerifyParametersForChangeRelationshipState(sourceEntity, targetEntity, out sourceEntry, out targetEntry);
      Check.NotEmpty(navigationProperty, nameof (navigationProperty));
      RelatedEnd relatedEnd = sourceEntry.WrappedEntity.RelationshipManager.GetRelatedEnd(navigationProperty, false);
      return this.ChangeRelationshipState(sourceEntry, targetEntry, relatedEnd, relationshipState);
    }

    /// <summary>Changes the state of the relationship between two entity objects that is specified based on the two related objects and a LINQ expression that defines the navigation property.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the relationship that was changed.
    /// </returns>
    /// <param name="sourceEntity">
    /// The object instance or <see cref="T:System.Data.Entity.Core.EntityKey" /> of the source entity at one end of the relationship.
    /// </param>
    /// <param name="targetEntity">
    /// The object instance or <see cref="T:System.Data.Entity.Core.EntityKey" /> of the target entity at the other end of the relationship.
    /// </param>
    /// <param name="navigationPropertySelector">A LINQ expression that selects the navigation property on  source  that returns the specified  target .</param>
    /// <param name="relationshipState">
    /// The requested <see cref="T:System.Data.Entity.EntityState" /> of the specified relationship.
    /// </param>
    /// <typeparam name="TEntity">The entity type of the  source  object.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">When  source ,  target , or  selector  is null.</exception>
    /// <exception cref="T:System.ArgumentException"> selector  is malformed or cannot return a navigation property.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// When you try to change the state of the relationship to a state other than
    /// <see cref="F:System.Data.Entity.EntityState.Deleted" />  or <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// when either  source  or  target  is in a
    /// <see cref="F:System.Data.Entity.EntityState.Deleted" /> state
    /// or when you try to change the state of the relationship to a state other than
    /// <see cref="F:System.Data.Entity.EntityState.Added" />  or <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// when either  source  or  target  is in an <see cref="F:System.Data.Entity.EntityState.Added" /> state
    /// or when  state  is not a valid <see cref="T:System.Data.Entity.EntityState" /> value.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual ObjectStateEntry ChangeRelationshipState<TEntity>(
      TEntity sourceEntity,
      object targetEntity,
      Expression<Func<TEntity, object>> navigationPropertySelector,
      EntityState relationshipState)
      where TEntity : class
    {
      EntityEntry sourceEntry;
      EntityEntry targetEntry;
      this.VerifyParametersForChangeRelationshipState((object) sourceEntity, targetEntity, out sourceEntry, out targetEntry);
      bool removedConvert;
      string selectorExpression = ObjectContext.ParsePropertySelectorExpression<TEntity>(navigationPropertySelector, out removedConvert);
      RelatedEnd relatedEnd = sourceEntry.WrappedEntity.RelationshipManager.GetRelatedEnd(selectorExpression, removedConvert);
      return this.ChangeRelationshipState(sourceEntry, targetEntry, relatedEnd, relationshipState);
    }

    /// <summary>Changes the state of the relationship between two entity objects that is specified based on the two related objects and the properties of the relationship.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the relationship that was changed.
    /// </returns>
    /// <param name="sourceEntity">
    /// The object instance or <see cref="T:System.Data.Entity.Core.EntityKey" /> of the source entity at one end of the relationship.
    /// </param>
    /// <param name="targetEntity">
    /// The object instance or <see cref="T:System.Data.Entity.Core.EntityKey" /> of the target entity at the other end of the relationship.
    /// </param>
    /// <param name="relationshipName">The name of the relationship.</param>
    /// <param name="targetRoleName">The role name at the  target  end of the relationship.</param>
    /// <param name="relationshipState">
    /// The requested <see cref="T:System.Data.Entity.EntityState" /> of the specified relationship.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">When  source  or  target  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// When you try to change the state of the relationship to a state other than
    /// <see cref="F:System.Data.Entity.EntityState.Deleted" /> or <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// when either  source  or  target  is in a <see cref="F:System.Data.Entity.EntityState.Deleted" /> state
    /// or when you try to change the state of the relationship to a state other than
    /// <see cref="F:System.Data.Entity.EntityState.Added" /> or <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// when either  source  or  target  is in an
    /// <see cref="F:System.Data.Entity.EntityState.Added" /> state
    /// or when  state  is not a valid  <see cref="T:System.Data.Entity.EntityState" /> value.
    /// </exception>
    public virtual ObjectStateEntry ChangeRelationshipState(
      object sourceEntity,
      object targetEntity,
      string relationshipName,
      string targetRoleName,
      EntityState relationshipState)
    {
      EntityEntry sourceEntry;
      EntityEntry targetEntry;
      this.VerifyParametersForChangeRelationshipState(sourceEntity, targetEntity, out sourceEntry, out targetEntry);
      RelatedEnd relatedEndInternal = sourceEntry.WrappedEntity.RelationshipManager.GetRelatedEndInternal(relationshipName, targetRoleName);
      return this.ChangeRelationshipState(sourceEntry, targetEntry, relatedEndInternal, relationshipState);
    }

    private ObjectStateEntry ChangeRelationshipState(
      EntityEntry sourceEntry,
      EntityEntry targetEntry,
      RelatedEnd relatedEnd,
      EntityState relationshipState)
    {
      ObjectStateManager.VerifyInitialStateForChangeRelationshipState(sourceEntry, targetEntry, relatedEnd, relationshipState);
      RelationshipWrapper relationshipWrapper = new RelationshipWrapper((AssociationSet) relatedEnd.RelationshipSet, new KeyValuePair<string, EntityKey>(relatedEnd.SourceRoleName, sourceEntry.EntityKey), new KeyValuePair<string, EntityKey>(relatedEnd.TargetRoleName, targetEntry.EntityKey));
      RelationshipEntry relationship = this.FindRelationship(relationshipWrapper);
      if (relationship == null && relationshipState == EntityState.Detached)
        return (ObjectStateEntry) null;
      this.TransactionManager.BeginLocalPublicAPI();
      try
      {
        if (relationship != null)
          relationship.ChangeRelationshipState(targetEntry, relatedEnd, relationshipState);
        else
          relationship = this.CreateRelationship(targetEntry, relatedEnd, relationshipWrapper, relationshipState);
      }
      finally
      {
        this.TransactionManager.EndLocalPublicAPI();
      }
      if (relationshipState != EntityState.Detached)
        return (ObjectStateEntry) relationship;
      return (ObjectStateEntry) null;
    }

    private void VerifyParametersForChangeRelationshipState(
      object sourceEntity,
      object targetEntity,
      out EntityEntry sourceEntry,
      out EntityEntry targetEntry)
    {
      sourceEntry = this.GetEntityEntryByObjectOrEntityKey(sourceEntity);
      targetEntry = this.GetEntityEntryByObjectOrEntityKey(targetEntity);
    }

    private static void VerifyInitialStateForChangeRelationshipState(
      EntityEntry sourceEntry,
      EntityEntry targetEntry,
      RelatedEnd relatedEnd,
      EntityState relationshipState)
    {
      relatedEnd.VerifyType(targetEntry.WrappedEntity);
      if (relatedEnd.IsForeignKey)
        throw new NotSupportedException(Strings.ObjectStateManager_ChangeRelationshipStateNotSupportedForForeignKeyAssociations);
      EntityUtil.CheckValidStateForChangeRelationshipState(relationshipState, nameof (relationshipState));
      if ((sourceEntry.State == EntityState.Deleted || targetEntry.State == EntityState.Deleted) && (relationshipState != EntityState.Deleted && relationshipState != EntityState.Detached))
        throw new InvalidOperationException(Strings.ObjectStateManager_CannotChangeRelationshipStateEntityDeleted);
      if ((sourceEntry.State == EntityState.Added || targetEntry.State == EntityState.Added) && (relationshipState != EntityState.Added && relationshipState != EntityState.Detached))
        throw new InvalidOperationException(Strings.ObjectStateManager_CannotChangeRelationshipStateEntityAdded);
    }

    private RelationshipEntry CreateRelationship(
      EntityEntry targetEntry,
      RelatedEnd relatedEnd,
      RelationshipWrapper relationshipWrapper,
      EntityState requestedState)
    {
      RelationshipEntry relationshipEntry = (RelationshipEntry) null;
      switch (requestedState)
      {
        case EntityState.Unchanged:
          relatedEnd.Add(targetEntry.WrappedEntity, true, false, false, false, true);
          relationshipEntry = this.FindRelationship(relationshipWrapper);
          relationshipEntry.AcceptChanges();
          break;
        case EntityState.Added:
          relatedEnd.Add(targetEntry.WrappedEntity, true, false, false, false, true);
          relationshipEntry = this.FindRelationship(relationshipWrapper);
          break;
        case EntityState.Deleted:
          relationshipEntry = this.AddNewRelation(relationshipWrapper, EntityState.Deleted);
          break;
      }
      return relationshipEntry;
    }

    private EntityEntry GetEntityEntryByObjectOrEntityKey(object o)
    {
      EntityKey key = o as EntityKey;
      EntityEntry entityEntry = key != (EntityKey) null ? this.FindEntityEntry(key) : this.FindEntityEntry(o);
      if (entityEntry == null)
        throw new InvalidOperationException(Strings.ObjectStateManager_NoEntryExistsForObject((object) o.GetType().FullName));
      if (entityEntry.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateManager_CannotChangeRelationshipStateKeyEntry);
      return entityEntry;
    }

    IEntityStateEntry IEntityStateManager.GetEntityStateEntry(
      EntityKey key)
    {
      return (IEntityStateEntry) this.GetEntityEntry(key);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the object or relationship entry with the specified key.
    /// </summary>
    /// <returns>
    /// The corresponding <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the given
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// .
    /// </returns>
    /// <param name="key">
    /// The <see cref="T:System.Data.Entity.Core.EntityKey" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">When  key  is null.</exception>
    /// <exception cref="T:System.ArgumentException">When the specified  key  cannot be found in the state manager.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// No entity with the specified <see cref="T:System.Data.Entity.Core.EntityKey" /> exists in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" />
    /// .
    /// </exception>
    public virtual ObjectStateEntry GetObjectStateEntry(EntityKey key)
    {
      ObjectStateEntry entry;
      if (!this.TryGetObjectStateEntry(key, out entry))
        throw new InvalidOperationException(Strings.ObjectStateManager_NoEntryExistForEntityKey);
      return entry;
    }

    internal virtual EntityEntry GetEntityEntry(EntityKey key)
    {
      EntityEntry entry;
      if (!this.TryGetEntityEntry(key, out entry))
        throw new InvalidOperationException(Strings.ObjectStateManager_NoEntryExistForEntityKey);
      return entry;
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the specified object.
    /// </summary>
    /// <returns>
    /// The corresponding <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the given
    /// <see cref="T:System.Object" />
    /// .
    /// </returns>
    /// <param name="entity">
    /// The <see cref="T:System.Object" /> to which the retrieved
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// belongs.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// No entity for the specified <see cref="T:System.Object" /> exists in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" />
    /// .
    /// </exception>
    public virtual ObjectStateEntry GetObjectStateEntry(object entity)
    {
      ObjectStateEntry entry;
      if (!this.TryGetObjectStateEntry(entity, out entry))
        throw new InvalidOperationException(Strings.ObjectStateManager_NoEntryExistsForObject((object) entity.GetType().FullName));
      return entry;
    }

    internal virtual EntityEntry GetEntityEntry(object entity)
    {
      EntityEntry entityEntry = this.FindEntityEntry(entity);
      if (entityEntry == null)
        throw new InvalidOperationException(Strings.ObjectStateManager_NoEntryExistsForObject((object) entity.GetType().FullName));
      return entityEntry;
    }

    /// <summary>
    /// Tries to retrieve the corresponding <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the specified
    /// <see cref="T:System.Object" />
    /// .
    /// </summary>
    /// <returns>
    /// A Boolean value that is true if there is a corresponding
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// for the given object; otherwise, false.
    /// </returns>
    /// <param name="entity">
    /// The <see cref="T:System.Object" /> to which the retrieved
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// belongs.
    /// </param>
    /// <param name="entry">
    /// When this method returns, contains the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the given
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// This parameter is passed uninitialized.
    /// </param>
    public virtual bool TryGetObjectStateEntry(object entity, out ObjectStateEntry entry)
    {
      Check.NotNull<object>(entity, nameof (entity));
      entry = (ObjectStateEntry) null;
      EntityKey key = entity as EntityKey;
      if (key != (EntityKey) null)
        return this.TryGetObjectStateEntry(key, out entry);
      entry = (ObjectStateEntry) this.FindEntityEntry(entity);
      return entry != null;
    }

    bool IEntityStateManager.TryGetEntityStateEntry(
      EntityKey key,
      out IEntityStateEntry entry)
    {
      ObjectStateEntry entry1;
      bool objectStateEntry = this.TryGetObjectStateEntry(key, out entry1);
      entry = (IEntityStateEntry) entry1;
      return objectStateEntry;
    }

    bool IEntityStateManager.TryGetReferenceKey(
      EntityKey dependentKey,
      AssociationEndMember principalRole,
      out EntityKey principalKey)
    {
      EntityEntry entry;
      if (this.TryGetEntityEntry(dependentKey, out entry))
        return entry.TryGetReferenceKey(principalRole, out principalKey);
      principalKey = (EntityKey) null;
      return false;
    }

    /// <summary>
    /// Tries to retrieve the corresponding <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the object or relationship with the specified
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// .
    /// </summary>
    /// <returns>
    /// A Boolean value that is true if there is a corresponding
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// for the given
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// ; otherwise, false.
    /// </returns>
    /// <param name="key">
    /// The given <see cref="T:System.Data.Entity.Core.EntityKey" />.
    /// </param>
    /// <param name="entry">
    /// When this method returns, contains an <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> for the given
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// This parameter is passed uninitialized.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">A null (Nothing in Visual Basic) value is provided for  key .</exception>
    public virtual bool TryGetObjectStateEntry(EntityKey key, out ObjectStateEntry entry)
    {
      EntityEntry entry1;
      bool entityEntry = this.TryGetEntityEntry(key, out entry1);
      entry = (ObjectStateEntry) entry1;
      return entityEntry;
    }

    internal virtual bool TryGetEntityEntry(EntityKey key, out EntityEntry entry)
    {
      entry = (EntityEntry) null;
      return !key.IsTemporary ? this._unchangedEntityStore != null && this._unchangedEntityStore.TryGetValue(key, out entry) || this._modifiedEntityStore != null && this._modifiedEntityStore.TryGetValue(key, out entry) || this._deletedEntityStore != null && this._deletedEntityStore.TryGetValue(key, out entry) : this._addedEntityStore != null && this._addedEntityStore.TryGetValue(key, out entry);
    }

    internal virtual EntityEntry FindEntityEntry(EntityKey key)
    {
      EntityEntry entry = (EntityEntry) null;
      if ((object) key != null)
        this.TryGetEntityEntry(key, out entry);
      return entry;
    }

    internal virtual EntityEntry FindEntityEntry(object entity)
    {
      EntityEntry entityEntry = (EntityEntry) null;
      IEntityWithKey entityWithKey = entity as IEntityWithKey;
      if (entityWithKey != null)
      {
        EntityKey entityKey = entityWithKey.EntityKey;
        if ((object) entityKey != null)
          this.TryGetEntityEntry(entityKey, out entityEntry);
      }
      else
        this.TryGetEntryFromKeylessStore(entity, out entityEntry);
      if (entityEntry != null && !object.ReferenceEquals(entity, entityEntry.Entity))
        entityEntry = (EntityEntry) null;
      return entityEntry;
    }

    /// <summary>
    /// Returns the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" /> that is used by the specified object.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" /> for the specified object.
    /// </returns>
    /// <param name="entity">
    /// The object for which to return the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" />.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// The entity does not implement IEntityWithRelationships and is not tracked by this ObjectStateManager
    /// </exception>
    public virtual RelationshipManager GetRelationshipManager(object entity)
    {
      RelationshipManager relationshipManager;
      if (!this.TryGetRelationshipManager(entity, out relationshipManager))
        throw new InvalidOperationException(Strings.ObjectStateManager_CannotGetRelationshipManagerForDetachedPocoEntity);
      return relationshipManager;
    }

    /// <summary>
    /// Returns the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" /> that is used by the specified object.
    /// </summary>
    /// <returns>
    /// true if a <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" /> instance was returned for the supplied  entity ; otherwise false.
    /// </returns>
    /// <param name="entity">
    /// The object for which to return the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" />.
    /// </param>
    /// <param name="relationshipManager">
    /// When this method returns, contains the
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" />
    /// for the  entity .
    /// </param>
    public virtual bool TryGetRelationshipManager(
      object entity,
      out RelationshipManager relationshipManager)
    {
      Check.NotNull<object>(entity, nameof (entity));
      IEntityWithRelationships withRelationships = entity as IEntityWithRelationships;
      if (withRelationships != null)
      {
        relationshipManager = withRelationships.RelationshipManager;
        if (relationshipManager == null)
          throw new InvalidOperationException(Strings.RelationshipManager_UnexpectedNull);
        if (relationshipManager.WrappedOwner.Entity != entity)
          throw new InvalidOperationException(Strings.RelationshipManager_InvalidRelationshipManagerOwner);
      }
      else
      {
        IEntityWrapper entityWrapper = this.EntityWrapperFactory.WrapEntityUsingStateManager(entity, this);
        if (entityWrapper.Context == null)
        {
          relationshipManager = (RelationshipManager) null;
          return false;
        }
        relationshipManager = entityWrapper.RelationshipManager;
      }
      return true;
    }

    internal virtual void ChangeState(
      RelationshipEntry entry,
      EntityState oldState,
      EntityState newState)
    {
      if (newState == EntityState.Detached)
      {
        this.DeleteRelationshipFromLookup(entry);
        this.RemoveObjectStateEntryFromDictionary(entry, oldState);
        entry.Reset();
      }
      else
      {
        this.RemoveObjectStateEntryFromDictionary(entry, oldState);
        this.AddRelationshipEntryToDictionary(entry, newState);
      }
    }

    internal virtual void ChangeState(
      EntityEntry entry,
      EntityState oldState,
      EntityState newState)
    {
      bool flag = !entry.IsKeyEntry;
      if (newState == EntityState.Detached)
      {
        foreach (RelationshipEntry entry1 in this.CopyOfRelationshipsByKey(entry.EntityKey))
          this.ChangeState(entry1, entry1.State, EntityState.Detached);
        this.RemoveObjectStateEntryFromDictionary(entry, oldState);
        IEntityWrapper wrappedEntity = entry.WrappedEntity;
        entry.Reset();
        if (flag && wrappedEntity.Entity != null && !this.TransactionManager.IsAttachTracking)
        {
          this.OnEntityDeleted(CollectionChangeAction.Remove, wrappedEntity.Entity);
          this.OnObjectStateManagerChanged(CollectionChangeAction.Remove, wrappedEntity.Entity);
        }
      }
      else
      {
        this.RemoveObjectStateEntryFromDictionary(entry, oldState);
        this.AddEntityEntryToDictionary(entry, newState);
      }
      if (newState != EntityState.Deleted)
        return;
      entry.RemoveFromForeignKeyIndex();
      this.ForgetEntryWithConceptualNull(entry, true);
      if (!flag)
        return;
      this.OnEntityDeleted(CollectionChangeAction.Remove, entry.Entity);
      this.OnObjectStateManagerChanged(CollectionChangeAction.Remove, entry.Entity);
    }

    private void AddRelationshipEntryToDictionary(RelationshipEntry entry, EntityState state)
    {
      Dictionary<RelationshipWrapper, RelationshipEntry> dictionary = (Dictionary<RelationshipWrapper, RelationshipEntry>) null;
      switch (state)
      {
        case EntityState.Unchanged:
          if (this._unchangedRelationshipStore == null)
            this._unchangedRelationshipStore = new Dictionary<RelationshipWrapper, RelationshipEntry>();
          dictionary = this._unchangedRelationshipStore;
          break;
        case EntityState.Added:
          if (this._addedRelationshipStore == null)
            this._addedRelationshipStore = new Dictionary<RelationshipWrapper, RelationshipEntry>();
          dictionary = this._addedRelationshipStore;
          break;
        case EntityState.Deleted:
          if (this._deletedRelationshipStore == null)
            this._deletedRelationshipStore = new Dictionary<RelationshipWrapper, RelationshipEntry>();
          dictionary = this._deletedRelationshipStore;
          break;
      }
      dictionary.Add(entry.RelationshipWrapper, entry);
    }

    private void AddEntityEntryToDictionary(EntityEntry entry, EntityState state)
    {
      if (entry.RequiresAnyChangeTracking)
        this._detectChangesNeeded = true;
      Dictionary<EntityKey, EntityEntry> dictionary = (Dictionary<EntityKey, EntityEntry>) null;
      switch (state)
      {
        case EntityState.Unchanged:
          if (this._unchangedEntityStore == null)
            this._unchangedEntityStore = new Dictionary<EntityKey, EntityEntry>();
          dictionary = this._unchangedEntityStore;
          break;
        case EntityState.Added:
          if (this._addedEntityStore == null)
            this._addedEntityStore = new Dictionary<EntityKey, EntityEntry>();
          dictionary = this._addedEntityStore;
          break;
        case EntityState.Deleted:
          if (this._deletedEntityStore == null)
            this._deletedEntityStore = new Dictionary<EntityKey, EntityEntry>();
          dictionary = this._deletedEntityStore;
          break;
        case EntityState.Modified:
          if (this._modifiedEntityStore == null)
            this._modifiedEntityStore = new Dictionary<EntityKey, EntityEntry>();
          dictionary = this._modifiedEntityStore;
          break;
      }
      dictionary.Add(entry.EntityKey, entry);
      this.AddEntryToKeylessStore(entry);
    }

    private void AddEntryToKeylessStore(EntityEntry entry)
    {
      if (entry.Entity == null || entry.Entity is IEntityWithKey)
        return;
      if (this._keylessEntityStore == null)
        this._keylessEntityStore = new Dictionary<object, EntityEntry>((IEqualityComparer<object>) ObjectReferenceEqualityComparer.Default);
      if (this._keylessEntityStore.ContainsKey(entry.Entity))
        return;
      this._keylessEntityStore.Add(entry.Entity, entry);
    }

    private void RemoveObjectStateEntryFromDictionary(RelationshipEntry entry, EntityState state)
    {
      Dictionary<RelationshipWrapper, RelationshipEntry> dictionary = (Dictionary<RelationshipWrapper, RelationshipEntry>) null;
      switch (state)
      {
        case EntityState.Unchanged:
          dictionary = this._unchangedRelationshipStore;
          break;
        case EntityState.Added:
          dictionary = this._addedRelationshipStore;
          break;
        case EntityState.Deleted:
          dictionary = this._deletedRelationshipStore;
          break;
      }
      dictionary.Remove(entry.RelationshipWrapper);
      if (dictionary.Count != 0)
        return;
      switch (state)
      {
        case EntityState.Unchanged:
          this._unchangedRelationshipStore = (Dictionary<RelationshipWrapper, RelationshipEntry>) null;
          break;
        case EntityState.Added:
          this._addedRelationshipStore = (Dictionary<RelationshipWrapper, RelationshipEntry>) null;
          break;
        case EntityState.Deleted:
          this._deletedRelationshipStore = (Dictionary<RelationshipWrapper, RelationshipEntry>) null;
          break;
      }
    }

    private void RemoveObjectStateEntryFromDictionary(EntityEntry entry, EntityState state)
    {
      Dictionary<EntityKey, EntityEntry> dictionary = (Dictionary<EntityKey, EntityEntry>) null;
      switch (state)
      {
        case EntityState.Unchanged:
          dictionary = this._unchangedEntityStore;
          break;
        case EntityState.Added:
          dictionary = this._addedEntityStore;
          break;
        case EntityState.Deleted:
          dictionary = this._deletedEntityStore;
          break;
        case EntityState.Modified:
          dictionary = this._modifiedEntityStore;
          break;
      }
      dictionary.Remove(entry.EntityKey);
      this.RemoveEntryFromKeylessStore(entry.WrappedEntity);
      if (dictionary.Count != 0)
        return;
      switch (state)
      {
        case EntityState.Unchanged:
          this._unchangedEntityStore = (Dictionary<EntityKey, EntityEntry>) null;
          break;
        case EntityState.Added:
          this._addedEntityStore = (Dictionary<EntityKey, EntityEntry>) null;
          break;
        case EntityState.Deleted:
          this._deletedEntityStore = (Dictionary<EntityKey, EntityEntry>) null;
          break;
        case EntityState.Modified:
          this._modifiedEntityStore = (Dictionary<EntityKey, EntityEntry>) null;
          break;
      }
    }

    internal virtual void RemoveEntryFromKeylessStore(IEntityWrapper wrappedEntity)
    {
      if (wrappedEntity == null || wrappedEntity.Entity == null || wrappedEntity.Entity is IEntityWithKey)
        return;
      this._keylessEntityStore.Remove(wrappedEntity.Entity);
    }

    internal virtual StateManagerTypeMetadata GetOrAddStateManagerTypeMetadata(
      Type entityType,
      EntitySet entitySet)
    {
      StateManagerTypeMetadata managerTypeMetadata;
      if (!this._metadataMapping.TryGetValue(new EntitySetQualifiedType(entityType, entitySet), out managerTypeMetadata))
        managerTypeMetadata = this.AddStateManagerTypeMetadata(entitySet, (ObjectTypeMapping) this.MetadataWorkspace.GetMap(entityType.FullNameWithNesting(), DataSpace.OSpace, DataSpace.OCSpace));
      return managerTypeMetadata;
    }

    internal virtual StateManagerTypeMetadata GetOrAddStateManagerTypeMetadata(
      EdmType edmType)
    {
      StateManagerTypeMetadata managerTypeMetadata;
      if (!this._metadataStore.TryGetValue(edmType, out managerTypeMetadata))
        managerTypeMetadata = this.AddStateManagerTypeMetadata(edmType, (ObjectTypeMapping) this.MetadataWorkspace.GetMap((GlobalItem) edmType, DataSpace.OCSpace));
      return managerTypeMetadata;
    }

    private StateManagerTypeMetadata AddStateManagerTypeMetadata(
      EntitySet entitySet,
      ObjectTypeMapping mapping)
    {
      EdmType edmType = mapping.EdmType;
      StateManagerTypeMetadata managerTypeMetadata;
      if (!this._metadataStore.TryGetValue(edmType, out managerTypeMetadata))
      {
        managerTypeMetadata = new StateManagerTypeMetadata(edmType, mapping);
        this._metadataStore.Add(edmType, managerTypeMetadata);
      }
      EntitySetQualifiedType key = new EntitySetQualifiedType(mapping.ClrType.ClrType, entitySet);
      if (this._metadataMapping.ContainsKey(key))
        throw new InvalidOperationException(Strings.Mapping_CannotMapCLRTypeMultipleTimes((object) managerTypeMetadata.CdmMetadata.EdmType.FullName));
      this._metadataMapping.Add(key, managerTypeMetadata);
      return managerTypeMetadata;
    }

    private StateManagerTypeMetadata AddStateManagerTypeMetadata(
      EdmType edmType,
      ObjectTypeMapping mapping)
    {
      StateManagerTypeMetadata managerTypeMetadata = new StateManagerTypeMetadata(edmType, mapping);
      this._metadataStore.Add(edmType, managerTypeMetadata);
      return managerTypeMetadata;
    }

    internal virtual void Dispose()
    {
      this._isDisposed = true;
    }

    internal virtual bool IsDisposed
    {
      get
      {
        return this._isDisposed;
      }
    }

    internal virtual void DetectChanges()
    {
      IList<EntityEntry> forDetectChanges = this.GetEntityEntriesForDetectChanges();
      if (forDetectChanges == null)
        return;
      if (!this.TransactionManager.BeginDetectChanges())
        return;
      try
      {
        ObjectStateManager.DetectChangesInNavigationProperties(forDetectChanges);
        ObjectStateManager.DetectChangesInScalarAndComplexProperties(forDetectChanges);
        ObjectStateManager.DetectChangesInForeignKeys(forDetectChanges);
        this.DetectConflicts(forDetectChanges);
        this.TransactionManager.BeginAlignChanges();
        this.AlignChangesInRelationships(forDetectChanges);
      }
      finally
      {
        this.TransactionManager.EndAlignChanges();
        this.TransactionManager.EndDetectChanges();
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void DetectConflicts(IList<EntityEntry> entries)
    {
      TransactionManager transactionManager = this.TransactionManager;
      foreach (EntityEntry entry in (IEnumerable<EntityEntry>) entries)
      {
        Dictionary<RelatedEnd, HashSet<IEntityWrapper>> dictionary1;
        transactionManager.AddedRelationshipsByGraph.TryGetValue(entry.WrappedEntity, out dictionary1);
        Dictionary<RelatedEnd, HashSet<EntityKey>> dictionary2;
        transactionManager.AddedRelationshipsByForeignKey.TryGetValue(entry.WrappedEntity, out dictionary2);
        if (dictionary1 != null && dictionary1.Count > 0 && entry.State == EntityState.Deleted)
          throw new InvalidOperationException(Strings.RelatedEnd_UnableToAddRelationshipWithDeletedEntity);
        if (dictionary2 != null)
        {
          foreach (KeyValuePair<RelatedEnd, HashSet<EntityKey>> keyValuePair in dictionary2)
          {
            if ((entry.State == EntityState.Unchanged || entry.State == EntityState.Modified) && (keyValuePair.Key.IsDependentEndOfReferentialConstraint(true) && keyValuePair.Value.Count > 0))
              throw new InvalidOperationException(Strings.EntityReference_CannotChangeReferentialConstraintProperty);
            if (keyValuePair.Key is EntityReference && keyValuePair.Value.Count > 1)
              throw new InvalidOperationException(Strings.ObjectStateManager_ConflictingChangesOfRelationshipDetected((object) keyValuePair.Key.RelationshipNavigation.To, (object) keyValuePair.Key.RelationshipNavigation.RelationshipName));
          }
        }
        if (dictionary1 != null)
        {
          Dictionary<string, KeyValuePair<object, IntBox>> properties1 = new Dictionary<string, KeyValuePair<object, IntBox>>();
          foreach (KeyValuePair<RelatedEnd, HashSet<IEntityWrapper>> keyValuePair in dictionary1)
          {
            if (keyValuePair.Key.IsForeignKey && (entry.State == EntityState.Unchanged || entry.State == EntityState.Modified) && (keyValuePair.Key.IsDependentEndOfReferentialConstraint(true) && keyValuePair.Value.Count > 0))
              throw new InvalidOperationException(Strings.EntityReference_CannotChangeReferentialConstraintProperty);
            EntityReference key = keyValuePair.Key as EntityReference;
            if (key != null)
            {
              if (keyValuePair.Value.Count > 1)
                throw new InvalidOperationException(Strings.ObjectStateManager_ConflictingChangesOfRelationshipDetected((object) keyValuePair.Key.RelationshipNavigation.To, (object) keyValuePair.Key.RelationshipNavigation.RelationshipName));
              if (keyValuePair.Value.Count == 1)
              {
                IEntityWrapper entityTo = keyValuePair.Value.First<IEntityWrapper>();
                HashSet<EntityKey> source = (HashSet<EntityKey>) null;
                if (dictionary2 != null)
                {
                  dictionary2.TryGetValue(keyValuePair.Key, out source);
                }
                else
                {
                  Dictionary<RelatedEnd, HashSet<EntityKey>> dictionary3;
                  if (transactionManager.AddedRelationshipsByPrincipalKey.TryGetValue(entry.WrappedEntity, out dictionary3))
                    dictionary3.TryGetValue(keyValuePair.Key, out source);
                }
                if (source != null && source.Count > 0)
                {
                  if (this.GetPermanentKey(entry.WrappedEntity, (RelatedEnd) key, entityTo) != source.First<EntityKey>())
                    throw new InvalidOperationException(Strings.ObjectStateManager_ConflictingChangesOfRelationshipDetected((object) key.RelationshipNavigation.To, (object) key.RelationshipNavigation.RelationshipName));
                }
                else
                {
                  Dictionary<RelatedEnd, HashSet<EntityKey>> dictionary3;
                  HashSet<EntityKey> entityKeySet;
                  if (transactionManager.DeletedRelationshipsByForeignKey.TryGetValue(entry.WrappedEntity, out dictionary3) && dictionary3.TryGetValue(keyValuePair.Key, out entityKeySet) && entityKeySet.Count > 0)
                    throw new InvalidOperationException(Strings.ObjectStateManager_ConflictingChangesOfRelationshipDetected((object) key.RelationshipNavigation.To, (object) key.RelationshipNavigation.RelationshipName));
                }
                EntityEntry entityEntry = this.FindEntityEntry(entityTo.Entity);
                if (entityEntry != null && (entityEntry.State == EntityState.Unchanged || entityEntry.State == EntityState.Modified))
                {
                  Dictionary<string, KeyValuePair<object, IntBox>> properties2 = new Dictionary<string, KeyValuePair<object, IntBox>>();
                  entityEntry.GetOtherKeyProperties(properties2);
                  foreach (ReferentialConstraint referentialConstraint in ((AssociationType) key.RelationMetadata).ReferentialConstraints)
                  {
                    if (referentialConstraint.ToRole == key.FromEndMember)
                    {
                      for (int index = 0; index < referentialConstraint.FromProperties.Count; ++index)
                        EntityEntry.AddOrIncreaseCounter(referentialConstraint, properties1, referentialConstraint.ToProperties[index].Name, properties2[referentialConstraint.FromProperties[index].Name].Key);
                      break;
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    internal virtual EntityKey GetPermanentKey(
      IEntityWrapper entityFrom,
      RelatedEnd relatedEndFrom,
      IEntityWrapper entityTo)
    {
      EntityKey entityKey = (EntityKey) null;
      if (entityTo.ObjectStateEntry != null)
        entityKey = entityTo.ObjectStateEntry.EntityKey;
      if (entityKey == (EntityKey) null || entityKey.IsTemporary)
        entityKey = this.CreateEntityKey(ObjectStateManager.GetEntitySetOfOtherEnd(entityFrom, relatedEndFrom), entityTo.Entity);
      return entityKey;
    }

    private static EntitySet GetEntitySetOfOtherEnd(
      IEntityWrapper entity,
      RelatedEnd relatedEnd)
    {
      AssociationSet relationshipSet = (AssociationSet) relatedEnd.RelationshipSet;
      EntitySet entitySet = relationshipSet.AssociationSetEnds[0].EntitySet;
      if (entitySet.Name != entity.EntityKey.EntitySetName)
        return entitySet;
      return relationshipSet.AssociationSetEnds[1].EntitySet;
    }

    private static void DetectChangesInForeignKeys(IList<EntityEntry> entries)
    {
      foreach (EntityEntry entry in (IEnumerable<EntityEntry>) entries)
      {
        if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
          entry.DetectChangesInForeignKeys();
      }
    }

    private void AlignChangesInRelationships(IList<EntityEntry> entries)
    {
      this.PerformDelete(entries);
      this.PerformAdd(entries);
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    private void PerformAdd(IList<EntityEntry> entries)
    {
      TransactionManager transactionManager = this.TransactionManager;
      foreach (EntityEntry entry1 in (IEnumerable<EntityEntry>) entries)
      {
        if (entry1.State != EntityState.Detached && !entry1.IsKeyEntry)
        {
          foreach (RelatedEnd relationship in entry1.WrappedEntity.RelationshipManager.Relationships)
          {
            HashSet<EntityKey> entityKeySet = (HashSet<EntityKey>) null;
            Dictionary<RelatedEnd, HashSet<EntityKey>> dictionary1;
            if (relationship is EntityReference && transactionManager.AddedRelationshipsByForeignKey.TryGetValue(entry1.WrappedEntity, out dictionary1))
              dictionary1.TryGetValue(relationship, out entityKeySet);
            HashSet<IEntityWrapper> entityWrapperSet = (HashSet<IEntityWrapper>) null;
            Dictionary<RelatedEnd, HashSet<IEntityWrapper>> dictionary2;
            if (transactionManager.AddedRelationshipsByGraph.TryGetValue(entry1.WrappedEntity, out dictionary2))
              dictionary2.TryGetValue(relationship, out entityWrapperSet);
            if (entityKeySet != null)
            {
              foreach (EntityKey key in entityKeySet)
              {
                EntityEntry entry2;
                if (this.TryGetEntityEntry(key, out entry2) && entry2.WrappedEntity.Entity != null)
                {
                  entityWrapperSet = entityWrapperSet ?? new HashSet<IEntityWrapper>();
                  if (entry2.State != EntityState.Deleted)
                  {
                    entityWrapperSet.Remove(entry2.WrappedEntity);
                    this.PerformAdd(entry1.WrappedEntity, relationship, entry2.WrappedEntity, true);
                  }
                }
                else
                {
                  EntityReference reference = relationship as EntityReference;
                  entry1.FixupEntityReferenceByForeignKey(reference);
                }
              }
            }
            if (entityWrapperSet != null)
            {
              foreach (IEntityWrapper entityToAdd in entityWrapperSet)
                this.PerformAdd(entry1.WrappedEntity, relationship, entityToAdd, false);
            }
          }
        }
      }
    }

    private void PerformAdd(
      IEntityWrapper wrappedOwner,
      RelatedEnd relatedEnd,
      IEntityWrapper entityToAdd,
      bool isForeignKeyChange)
    {
      relatedEnd.ValidateStateForAdd(relatedEnd.WrappedOwner);
      relatedEnd.ValidateStateForAdd(entityToAdd);
      if (relatedEnd.IsPrincipalEndOfReferentialConstraint())
      {
        EntityReference endOfRelationship = relatedEnd.GetOtherEndOfRelationship(entityToAdd) as EntityReference;
        if (endOfRelationship != null)
        {
          if (this.IsReparentingReference(entityToAdd, endOfRelationship))
            this.TransactionManager.EntityBeingReparented = endOfRelationship.GetDependentEndOfReferentialConstraint(endOfRelationship.ReferenceValue.Entity);
        }
      }
      else if (relatedEnd.IsDependentEndOfReferentialConstraint(false))
      {
        EntityReference reference = relatedEnd as EntityReference;
        if (reference != null)
        {
          if (this.IsReparentingReference(wrappedOwner, reference))
            this.TransactionManager.EntityBeingReparented = reference.GetDependentEndOfReferentialConstraint(reference.ReferenceValue.Entity);
        }
      }
      try
      {
        relatedEnd.Add(entityToAdd, false, false, false, true, !isForeignKeyChange);
      }
      finally
      {
        this.TransactionManager.EntityBeingReparented = (object) null;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void PerformDelete(IList<EntityEntry> entries)
    {
      TransactionManager transactionManager = this.TransactionManager;
      foreach (EntityEntry entry1 in (IEnumerable<EntityEntry>) entries)
      {
        if (entry1.State != EntityState.Detached && entry1.State != EntityState.Deleted && !entry1.IsKeyEntry)
        {
          foreach (RelatedEnd relationship in entry1.WrappedEntity.RelationshipManager.Relationships)
          {
            HashSet<EntityKey> entityKeySet = (HashSet<EntityKey>) null;
            EntityReference reference = relationship as EntityReference;
            Dictionary<RelatedEnd, HashSet<EntityKey>> dictionary1;
            if (reference != null && transactionManager.DeletedRelationshipsByForeignKey.TryGetValue(entry1.WrappedEntity, out dictionary1))
              dictionary1.TryGetValue((RelatedEnd) reference, out entityKeySet);
            HashSet<IEntityWrapper> entitiesToDelete = (HashSet<IEntityWrapper>) null;
            Dictionary<RelatedEnd, HashSet<IEntityWrapper>> dictionary2;
            if (transactionManager.DeletedRelationshipsByGraph.TryGetValue(entry1.WrappedEntity, out dictionary2))
              dictionary2.TryGetValue(relationship, out entitiesToDelete);
            if (entityKeySet != null)
            {
              foreach (EntityKey key in entityKeySet)
              {
                IEntityWrapper entityWrapper = (IEntityWrapper) null;
                EntityEntry entry2;
                if (this.TryGetEntityEntry(key, out entry2) && entry2.WrappedEntity.Entity != null)
                  entityWrapper = entry2.WrappedEntity;
                else if (reference != null && reference.ReferenceValue != NullEntityWrapper.NullWrapper && (reference.ReferenceValue.EntityKey.IsTemporary && this.TryGetEntityEntry(reference.ReferenceValue.EntityKey, out entry2)) && entry2.WrappedEntity.Entity != null)
                {
                  EntityKey entityKey = new EntityKey((EntitySet) entry2.EntitySet, (IExtendedDataRecord) entry2.CurrentValues);
                  if (key == entityKey)
                    entityWrapper = entry2.WrappedEntity;
                }
                if (entityWrapper != null)
                {
                  entitiesToDelete = entitiesToDelete ?? new HashSet<IEntityWrapper>();
                  bool preserveForeignKey = this.ShouldPreserveForeignKeyForDependent(entry1.WrappedEntity, relationship, entityWrapper, entitiesToDelete);
                  entitiesToDelete.Remove(entityWrapper);
                  if (reference != null)
                  {
                    if (this.IsReparentingReference(entry1.WrappedEntity, reference))
                      this.TransactionManager.EntityBeingReparented = reference.GetDependentEndOfReferentialConstraint(reference.ReferenceValue.Entity);
                  }
                  try
                  {
                    relationship.Remove(entityWrapper, preserveForeignKey);
                  }
                  finally
                  {
                    this.TransactionManager.EntityBeingReparented = (object) null;
                  }
                  if (entry1.State != EntityState.Detached)
                  {
                    if (entry1.State != EntityState.Deleted)
                    {
                      if (entry1.IsKeyEntry)
                        break;
                    }
                    else
                      break;
                  }
                  else
                    break;
                }
                if (reference != null && reference.IsForeignKey && reference.IsDependentEndOfReferentialConstraint(false))
                  reference.SetCachedForeignKey(ForeignKeyFactory.CreateKeyFromForeignKeyValues(entry1, (RelatedEnd) reference), entry1);
              }
            }
            if (entitiesToDelete != null)
            {
              foreach (IEntityWrapper entityWrapper in entitiesToDelete)
              {
                bool preserveForeignKey = this.ShouldPreserveForeignKeyForPrincipal(entry1.WrappedEntity, relationship, entityWrapper, entitiesToDelete);
                if (reference != null)
                {
                  if (this.IsReparentingReference(entry1.WrappedEntity, reference))
                    this.TransactionManager.EntityBeingReparented = reference.GetDependentEndOfReferentialConstraint(reference.ReferenceValue.Entity);
                }
                try
                {
                  relationship.Remove(entityWrapper, preserveForeignKey);
                }
                finally
                {
                  this.TransactionManager.EntityBeingReparented = (object) null;
                }
                if (entry1.State != EntityState.Detached)
                {
                  if (entry1.State != EntityState.Deleted)
                  {
                    if (entry1.IsKeyEntry)
                      break;
                  }
                  else
                    break;
                }
                else
                  break;
              }
            }
            if (entry1.State != EntityState.Detached)
            {
              if (entry1.State != EntityState.Deleted)
              {
                if (entry1.IsKeyEntry)
                  break;
              }
              else
                break;
            }
            else
              break;
          }
        }
      }
    }

    private bool ShouldPreserveForeignKeyForPrincipal(
      IEntityWrapper entity,
      RelatedEnd relatedEnd,
      IEntityWrapper relatedEntity,
      HashSet<IEntityWrapper> entitiesToDelete)
    {
      bool flag = false;
      if (relatedEnd.IsForeignKey)
      {
        RelatedEnd endOfRelationship = relatedEnd.GetOtherEndOfRelationship(relatedEntity);
        if (endOfRelationship.IsDependentEndOfReferentialConstraint(false))
        {
          HashSet<EntityKey> entityKeySet = (HashSet<EntityKey>) null;
          Dictionary<RelatedEnd, HashSet<EntityKey>> dictionary1;
          Dictionary<RelatedEnd, HashSet<IEntityWrapper>> dictionary2;
          if (this.TransactionManager.DeletedRelationshipsByForeignKey.TryGetValue(relatedEntity, out dictionary1) && dictionary1.TryGetValue(endOfRelationship, out entityKeySet) && (entityKeySet.Count > 0 && this.TransactionManager.DeletedRelationshipsByGraph.TryGetValue(relatedEntity, out dictionary2)) && dictionary2.TryGetValue(endOfRelationship, out entitiesToDelete))
            flag = this.ShouldPreserveForeignKeyForDependent(relatedEntity, endOfRelationship, entity, entitiesToDelete);
        }
      }
      return flag;
    }

    private bool ShouldPreserveForeignKeyForDependent(
      IEntityWrapper entity,
      RelatedEnd relatedEnd,
      IEntityWrapper relatedEntity,
      HashSet<IEntityWrapper> entitiesToDelete)
    {
      bool flag = entitiesToDelete.Contains(relatedEntity);
      if (!flag)
        return true;
      if (flag)
        return !this.HasAddedReference(entity, relatedEnd as EntityReference);
      return false;
    }

    private bool HasAddedReference(IEntityWrapper wrappedOwner, EntityReference reference)
    {
      HashSet<IEntityWrapper> entityWrapperSet = (HashSet<IEntityWrapper>) null;
      Dictionary<RelatedEnd, HashSet<IEntityWrapper>> dictionary;
      return reference != null && this.TransactionManager.AddedRelationshipsByGraph.TryGetValue(wrappedOwner, out dictionary) && (dictionary.TryGetValue((RelatedEnd) reference, out entityWrapperSet) && entityWrapperSet.Count > 0);
    }

    private bool IsReparentingReference(IEntityWrapper wrappedEntity, EntityReference reference)
    {
      TransactionManager transactionManager = this.TransactionManager;
      if (reference.IsPrincipalEndOfReferentialConstraint())
      {
        wrappedEntity = reference.ReferenceValue;
        reference = wrappedEntity.Entity == null ? (EntityReference) null : reference.GetOtherEndOfRelationship(wrappedEntity) as EntityReference;
      }
      if (wrappedEntity.Entity != null && reference != null)
      {
        HashSet<EntityKey> entityKeySet = (HashSet<EntityKey>) null;
        Dictionary<RelatedEnd, HashSet<EntityKey>> dictionary1;
        if (transactionManager.AddedRelationshipsByForeignKey.TryGetValue(wrappedEntity, out dictionary1) && dictionary1.TryGetValue((RelatedEnd) reference, out entityKeySet) && entityKeySet.Count > 0)
          return true;
        HashSet<IEntityWrapper> entityWrapperSet = (HashSet<IEntityWrapper>) null;
        Dictionary<RelatedEnd, HashSet<IEntityWrapper>> dictionary2;
        if (transactionManager.AddedRelationshipsByGraph.TryGetValue(wrappedEntity, out dictionary2) && dictionary2.TryGetValue((RelatedEnd) reference, out entityWrapperSet) && entityWrapperSet.Count > 0)
          return true;
      }
      return false;
    }

    private static void DetectChangesInNavigationProperties(IList<EntityEntry> entries)
    {
      foreach (EntityEntry entry in (IEnumerable<EntityEntry>) entries)
      {
        if (entry.WrappedEntity.RequiresRelationshipChangeTracking)
          entry.DetectChangesInRelationshipsOfSingleEntity();
      }
    }

    private static void DetectChangesInScalarAndComplexProperties(IList<EntityEntry> entries)
    {
      foreach (EntityEntry entry in (IEnumerable<EntityEntry>) entries)
      {
        if (entry.State != EntityState.Added && (entry.RequiresScalarChangeTracking || entry.RequiresComplexChangeTracking))
          entry.DetectChangesInProperties(!entry.RequiresScalarChangeTracking);
      }
    }

    internal virtual EntityKey CreateEntityKey(EntitySet entitySet, object entity)
    {
      ReadOnlyMetadataCollection<EdmMember> keyMembers = entitySet.ElementType.KeyMembers;
      StateManagerTypeMetadata managerTypeMetadata = this.GetOrAddStateManagerTypeMetadata(EntityUtil.GetEntityIdentityType(entity.GetType()), entitySet);
      object[] compositeKeyValues = new object[keyMembers.Count];
      for (int index = 0; index < keyMembers.Count; ++index)
      {
        string name = keyMembers[index].Name;
        int clayerMemberName = managerTypeMetadata.GetOrdinalforCLayerMemberName(name);
        if (clayerMemberName < 0)
          throw new ArgumentException(Strings.ObjectStateManager_EntityTypeDoesnotMatchtoEntitySetType((object) entity.GetType().FullName, (object) entitySet.Name), nameof (entity));
        compositeKeyValues[index] = managerTypeMetadata.Member(clayerMemberName).GetValue(entity);
        if (compositeKeyValues[index] == null)
          throw new InvalidOperationException(Strings.EntityKey_NullKeyValue((object) name, (object) entitySet.ElementType.Name));
      }
      if (compositeKeyValues.Length == 1)
        return new EntityKey((EntitySetBase) entitySet, compositeKeyValues[0]);
      return new EntityKey((EntitySetBase) entitySet, compositeKeyValues);
    }

    internal virtual object EntityInvokingFKSetter { get; set; }
  }
}
