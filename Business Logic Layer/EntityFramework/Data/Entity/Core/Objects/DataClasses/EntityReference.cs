// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.EntityReference
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Resources;
using System.Linq;
using System.Runtime.Serialization;

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>Models a relationship end with multiplicity 1.</summary>
  [DataContract]
  [Serializable]
  public abstract class EntityReference : RelatedEnd
  {
    private EntityKey _detachedEntityKey;
    [NonSerialized]
    private EntityKey _cachedForeignKey;

    internal EntityReference()
    {
    }

    internal EntityReference(
      IEntityWrapper wrappedOwner,
      RelationshipNavigation navigation,
      IRelationshipFixer relationshipFixer)
      : base(wrappedOwner, navigation, relationshipFixer)
    {
    }

    /// <summary>Returns the key for the related object. </summary>
    /// <remarks>
    /// Returns the EntityKey of the target entity associated with this EntityReference.
    /// Is non-null in the following scenarios:
    /// (a) Entities are tracked by a context and an Unchanged or Added client-side relationships exists for this EntityReference's owner with the
    /// same RelationshipName and source role. This relationship could have been created explicitly by the user (e.g. by setting
    /// the EntityReference.Value, setting this property directly, or by calling EntityCollection.Add) or automatically through span queries.
    /// (b) If the EntityKey was non-null before detaching an entity from the context, it will still be non-null after detaching, until any operation
    /// occurs that would set it to null, as described below.
    /// (c) Entities are detached and the EntityKey is explicitly set to non-null by the user.
    /// (d) Entity graph was created using a NoTracking query with full span
    /// Is null in the following scenarios:
    /// (a) Entities are tracked by a context but there is no Unchanged or Added client-side relationship for this EntityReference's owner with the
    /// same RelationshipName and source role.
    /// (b) Entities are tracked by a context and a relationship exists, but the target entity has a temporary key (i.e. it is Added) or the key
    /// is one of the special keys
    /// (c) Entities are detached and the relationship was explicitly created by the user.
    /// </remarks>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.EntityKey" /> that is the key of the related object.
    /// </returns>
    [DataMember]
    public EntityKey EntityKey
    {
      get
      {
        if (this.ObjectContext == null || this.UsingNoTracking)
          return this.DetachedEntityKey;
        EntityKey entityKey1 = (EntityKey) null;
        if (this.CachedValue.Entity != null)
        {
          entityKey1 = this.CachedValue.EntityKey;
          if (entityKey1 != (EntityKey) null && !RelatedEnd.IsValidEntityKeyType(entityKey1))
            entityKey1 = (EntityKey) null;
        }
        else if (this.IsForeignKey)
        {
          if (this.IsDependentEndOfReferentialConstraint(false) && this._cachedForeignKey != (EntityKey) null)
          {
            if (!ForeignKeyFactory.IsConceptualNullKey(this._cachedForeignKey))
              entityKey1 = this._cachedForeignKey;
          }
          else
            entityKey1 = this.DetachedEntityKey;
        }
        else
        {
          EntityKey entityKey2 = this.WrappedOwner.EntityKey;
          foreach (RelationshipEntry relationshipEntry in this.ObjectContext.ObjectStateManager.FindRelationshipsByKey(entityKey2))
          {
            if (relationshipEntry.State != EntityState.Deleted && relationshipEntry.IsSameAssociationSetAndRole((AssociationSet) this.RelationshipSet, (AssociationEndMember) this.FromEndMember, entityKey2))
              entityKey1 = relationshipEntry.RelationshipWrapper.GetOtherEntityKey(entityKey2);
          }
        }
        return entityKey1;
      }
      set
      {
        this.SetEntityKey(value, false);
      }
    }

    internal void SetEntityKey(EntityKey value, bool forceFixup)
    {
      if (value != (EntityKey) null && value == this.EntityKey && (this.ReferenceValue.Entity != null || this.ReferenceValue.Entity == null && !forceFixup))
        return;
      if (this.ObjectContext != null && !this.UsingNoTracking)
      {
        if (value != (EntityKey) null && !RelatedEnd.IsValidEntityKeyType(value))
          throw new ArgumentException(Strings.EntityReference_CannotSetSpecialKeys, nameof (value));
        if (value == (EntityKey) null)
        {
          if (this.AttemptToNullFKsOnRefOrKeySetToNull())
            this.DetachedEntityKey = (EntityKey) null;
          else
            this.ReferenceValue = NullEntityWrapper.NullWrapper;
        }
        else
        {
          EntitySet entitySet = value.GetEntitySet(this.ObjectContext.MetadataWorkspace);
          this.CheckRelationEntitySet(entitySet);
          value.ValidateEntityKey(this.ObjectContext.MetadataWorkspace, entitySet, true, nameof (value));
          ObjectStateManager objectStateManager = this.ObjectContext.ObjectStateManager;
          bool flag1 = false;
          bool flag2 = false;
          EntityEntry entityEntry = objectStateManager.FindEntityEntry(value);
          if (entityEntry != null)
          {
            if (!entityEntry.IsKeyEntry)
              this.ReferenceValue = entityEntry.WrappedEntity;
            else
              flag1 = true;
          }
          else
          {
            flag2 = !this.IsForeignKey;
            flag1 = true;
          }
          if (!flag1)
            return;
          EntityKey key0 = this.ValidateOwnerWithRIConstraints(entityEntry == null ? (IEntityWrapper) null : entityEntry.WrappedEntity, value, true);
          this.ValidateStateForAdd(this.WrappedOwner);
          if (flag2)
            objectStateManager.AddKeyEntry(value, entitySet);
          objectStateManager.TransactionManager.EntityBeingReparented = this.WrappedOwner.Entity;
          try
          {
            this.ClearCollectionOrRef((IEntityWrapper) null, (RelationshipNavigation) null, false);
          }
          finally
          {
            objectStateManager.TransactionManager.EntityBeingReparented = (object) null;
          }
          if (this.IsForeignKey)
          {
            this.DetachedEntityKey = value;
            if (!this.IsDependentEndOfReferentialConstraint(false))
              return;
            this.UpdateForeignKeyValues(this.WrappedOwner, value);
          }
          else
          {
            RelationshipWrapper wrapper = new RelationshipWrapper((AssociationSet) this.RelationshipSet, this.RelationshipNavigation.From, key0, this.RelationshipNavigation.To, value);
            EntityState desiredState = EntityState.Added;
            if (!key0.IsTemporary && this.IsDependentEndOfReferentialConstraint(false))
              desiredState = EntityState.Unchanged;
            objectStateManager.AddNewRelation(wrapper, desiredState);
          }
        }
      }
      else
        this.DetachedEntityKey = value;
    }

    internal bool AttemptToNullFKsOnRefOrKeySetToNull()
    {
      if (this.ReferenceValue.Entity != null || this.WrappedOwner.Entity == null || (this.WrappedOwner.Context == null || this.UsingNoTracking) || !this.IsForeignKey)
        return false;
      if (this.WrappedOwner.ObjectStateEntry.State != EntityState.Added && this.IsDependentEndOfReferentialConstraint(true))
        throw new InvalidOperationException(Strings.EntityReference_CannotChangeReferentialConstraintProperty);
      this.RemoveFromLocalCache(NullEntityWrapper.NullWrapper, true, false);
      return true;
    }

    internal EntityKey AttachedEntityKey
    {
      get
      {
        return this.EntityKey;
      }
    }

    internal EntityKey DetachedEntityKey
    {
      get
      {
        return this._detachedEntityKey;
      }
      set
      {
        this._detachedEntityKey = value;
      }
    }

    internal EntityKey CachedForeignKey
    {
      get
      {
        EntityKey entityKey = this.EntityKey;
        if ((object) entityKey != null)
          return entityKey;
        return this._cachedForeignKey;
      }
    }

    internal void SetCachedForeignKey(EntityKey newForeignKey, EntityEntry source)
    {
      if (this.ObjectContext != null && this.ObjectContext.ObjectStateManager != null && (source != null && this._cachedForeignKey != (EntityKey) null) && (!ForeignKeyFactory.IsConceptualNullKey(this._cachedForeignKey) && this._cachedForeignKey != newForeignKey))
        this.ObjectContext.ObjectStateManager.RemoveEntryFromForeignKeyIndex(this, this._cachedForeignKey, source);
      this._cachedForeignKey = newForeignKey;
    }

    internal IEnumerable<EntityKey> GetAllKeyValues()
    {
      if (this.EntityKey != (EntityKey) null)
        yield return this.EntityKey;
      if (this._cachedForeignKey != (EntityKey) null)
        yield return this._cachedForeignKey;
      if (this._detachedEntityKey != (EntityKey) null)
        yield return this._detachedEntityKey;
    }

    internal abstract IEntityWrapper CachedValue { get; }

    internal abstract IEntityWrapper ReferenceValue { get; set; }

    internal EntityKey ValidateOwnerWithRIConstraints(
      IEntityWrapper targetEntity,
      EntityKey targetEntityKey,
      bool checkBothEnds)
    {
      EntityKey entityKey = this.WrappedOwner.EntityKey;
      if ((object) entityKey != null && !entityKey.IsTemporary && this.IsDependentEndOfReferentialConstraint(true))
        this.ValidateSettingRIConstraints(targetEntity, targetEntityKey == (EntityKey) null, this.CachedForeignKey != (EntityKey) null && this.CachedForeignKey != targetEntityKey);
      else if (checkBothEnds && targetEntity != null && targetEntity.Entity != null)
        (this.GetOtherEndOfRelationship(targetEntity) as EntityReference)?.ValidateOwnerWithRIConstraints(this.WrappedOwner, entityKey, false);
      return entityKey;
    }

    internal void ValidateSettingRIConstraints(
      IEntityWrapper targetEntity,
      bool settingToNull,
      bool changingForeignKeyValue)
    {
      bool flag = targetEntity != null && targetEntity.MergeOption == MergeOption.NoTracking;
      if (settingToNull || changingForeignKeyValue || targetEntity != null && !flag && (targetEntity.ObjectStateEntry == null || this.EntityKey == (EntityKey) null && targetEntity.ObjectStateEntry.State == EntityState.Deleted || this.CachedForeignKey == (EntityKey) null && targetEntity.ObjectStateEntry.State == EntityState.Added))
        throw new InvalidOperationException(Strings.EntityReference_CannotChangeReferentialConstraintProperty);
    }

    internal override bool CanDeferredLoad
    {
      get
      {
        return this.IsEmpty();
      }
    }

    internal void UpdateForeignKeyValues(
      IEntityWrapper dependentEntity,
      IEntityWrapper principalEntity,
      Dictionary<int, object> changedFKs,
      bool forceChange)
    {
      ReferentialConstraint referentialConstraint = ((AssociationType) this.RelationMetadata).ReferentialConstraints[0];
      bool flag = (object) this.WrappedOwner.EntityKey != null && !this.WrappedOwner.EntityKey.IsTemporary && this.IsDependentEndOfReferentialConstraint(true);
      ObjectStateManager objectStateManager = this.ObjectContext.ObjectStateManager;
      objectStateManager.TransactionManager.BeginForeignKeyUpdate(this);
      try
      {
        EntitySet entitySet1 = ((AssociationSet) this.RelationshipSet).AssociationSetEnds[this.ToEndMember.Name].EntitySet;
        StateManagerTypeMetadata managerTypeMetadata1 = objectStateManager.GetOrAddStateManagerTypeMetadata(principalEntity.IdentityType, entitySet1);
        EntitySet entitySet2 = ((AssociationSet) this.RelationshipSet).AssociationSetEnds[this.FromEndMember.Name].EntitySet;
        StateManagerTypeMetadata managerTypeMetadata2 = objectStateManager.GetOrAddStateManagerTypeMetadata(dependentEntity.IdentityType, entitySet2);
        ReadOnlyMetadataCollection<EdmProperty> fromProperties = referentialConstraint.FromProperties;
        int count = fromProperties.Count;
        string[] array = (string[]) null;
        object[] compositeKeyValues = (object[]) null;
        if (count > 1)
        {
          array = entitySet1.ElementType.KeyMemberNames;
          compositeKeyValues = new object[count];
        }
        for (int index1 = 0; index1 < count; ++index1)
        {
          int olayerMemberName1 = managerTypeMetadata1.GetOrdinalforOLayerMemberName(fromProperties[index1].Name);
          object obj = managerTypeMetadata1.Member(olayerMemberName1).GetValue(principalEntity.Entity);
          int olayerMemberName2 = managerTypeMetadata2.GetOrdinalforOLayerMemberName(referentialConstraint.ToProperties[index1].Name);
          bool changingForeignKeyValue = !ByValueEqualityComparer.Default.Equals(managerTypeMetadata2.Member(olayerMemberName2).GetValue(dependentEntity.Entity), obj);
          if (forceChange || changingForeignKeyValue)
          {
            if (flag)
              this.ValidateSettingRIConstraints(principalEntity, obj == null, changingForeignKeyValue);
            if (changedFKs != null)
            {
              object x;
              if (changedFKs.TryGetValue(olayerMemberName2, out x))
              {
                if (!ByValueEqualityComparer.Default.Equals(x, obj))
                  throw new InvalidOperationException(Strings.Update_ReferentialConstraintIntegrityViolation);
              }
              else
                changedFKs[olayerMemberName2] = obj;
            }
            dependentEntity.SetCurrentValue(dependentEntity.ObjectStateEntry, managerTypeMetadata2.Member(olayerMemberName2), -1, dependentEntity.Entity, obj);
          }
          if (count > 1)
          {
            int index2 = Array.IndexOf<string>(array, fromProperties[index1].Name);
            compositeKeyValues[index2] = obj;
          }
          else
            this.SetCachedForeignKey(obj == null ? (EntityKey) null : new EntityKey((EntitySetBase) entitySet1, obj), dependentEntity.ObjectStateEntry);
        }
        if (count > 1)
          this.SetCachedForeignKey(((IEnumerable<object>) compositeKeyValues).Any<object>((Func<object, bool>) (v => v == null)) ? (EntityKey) null : new EntityKey((EntitySetBase) entitySet1, compositeKeyValues), dependentEntity.ObjectStateEntry);
        if (this.WrappedOwner.ObjectStateEntry == null)
          return;
        objectStateManager.ForgetEntryWithConceptualNull(this.WrappedOwner.ObjectStateEntry, false);
      }
      finally
      {
        objectStateManager.TransactionManager.EndForeignKeyUpdate();
      }
    }

    internal void UpdateForeignKeyValues(IEntityWrapper dependentEntity, EntityKey principalKey)
    {
      ReferentialConstraint referentialConstraint = ((AssociationType) this.RelationMetadata).ReferentialConstraints[0];
      ObjectStateManager objectStateManager = this.ObjectContext.ObjectStateManager;
      objectStateManager.TransactionManager.BeginForeignKeyUpdate(this);
      try
      {
        EntitySet entitySet = ((AssociationSet) this.RelationshipSet).AssociationSetEnds[this.FromEndMember.Name].EntitySet;
        StateManagerTypeMetadata managerTypeMetadata = objectStateManager.GetOrAddStateManagerTypeMetadata(dependentEntity.IdentityType, entitySet);
        for (int index = 0; index < referentialConstraint.FromProperties.Count; ++index)
        {
          object valueByName = principalKey.FindValueByName(referentialConstraint.FromProperties[index].Name);
          int olayerMemberName = managerTypeMetadata.GetOrdinalforOLayerMemberName(referentialConstraint.ToProperties[index].Name);
          object x = managerTypeMetadata.Member(olayerMemberName).GetValue(dependentEntity.Entity);
          if (!ByValueEqualityComparer.Default.Equals(x, valueByName))
            dependentEntity.SetCurrentValue(dependentEntity.ObjectStateEntry, managerTypeMetadata.Member(olayerMemberName), -1, dependentEntity.Entity, valueByName);
        }
        this.SetCachedForeignKey(principalKey, dependentEntity.ObjectStateEntry);
        if (this.WrappedOwner.ObjectStateEntry == null)
          return;
        objectStateManager.ForgetEntryWithConceptualNull(this.WrappedOwner.ObjectStateEntry, false);
      }
      finally
      {
        objectStateManager.TransactionManager.EndForeignKeyUpdate();
      }
    }

    internal object GetDependentEndOfReferentialConstraint(object relatedValue)
    {
      if (!this.IsDependentEndOfReferentialConstraint(false))
        return relatedValue;
      return this.WrappedOwner.Entity;
    }

    internal bool NavigationPropertyIsNullOrMissing()
    {
      if (this.TargetAccessor.HasProperty)
        return this.WrappedOwner.GetNavigationPropertyValue((RelatedEnd) this) == null;
      return true;
    }

    internal override void AddEntityToObjectStateManager(
      IEntityWrapper wrappedEntity,
      bool doAttach)
    {
      base.AddEntityToObjectStateManager(wrappedEntity, doAttach);
      if (this.DetachedEntityKey != (EntityKey) null && this.DetachedEntityKey != wrappedEntity.EntityKey)
        throw new InvalidOperationException(Strings.EntityReference_EntityKeyValueMismatch);
    }

    internal override void AddToNavigationPropertyIfCompatible(RelatedEnd otherRelatedEnd)
    {
      if (this.NavigationPropertyIsNullOrMissing())
      {
        this.AddToNavigationProperty(otherRelatedEnd.WrappedOwner);
        if (otherRelatedEnd.ObjectContext.ObjectStateManager.FindEntityEntry(otherRelatedEnd.WrappedOwner.Entity) == null || !otherRelatedEnd.ObjectContext.ObjectStateManager.TransactionManager.IsAddTracking || (!otherRelatedEnd.IsForeignKey || !this.IsDependentEndOfReferentialConstraint(false)))
          return;
        this.MarkForeignKeyPropertiesModified();
      }
      else if (!this.CheckIfNavigationPropertyContainsEntity(otherRelatedEnd.WrappedOwner))
        throw Error.ObjectStateManager_ConflictingChangesOfRelationshipDetected((object) this.RelationshipNavigation.To, (object) this.RelationshipNavigation.RelationshipName);
    }

    internal override bool CachedForeignKeyIsConceptualNull()
    {
      return ForeignKeyFactory.IsConceptualNullKey(this.CachedForeignKey);
    }

    internal override bool UpdateDependentEndForeignKey(
      RelatedEnd targetRelatedEnd,
      bool forceForeignKeyChanges)
    {
      if (!this.IsDependentEndOfReferentialConstraint(false))
        return false;
      this.UpdateForeignKeyValues(this.WrappedOwner, targetRelatedEnd.WrappedOwner, (Dictionary<int, object>) null, forceForeignKeyChanges);
      return true;
    }

    internal override void ValidateDetachedEntityKey()
    {
      if (!this.IsEmpty() || !(this.DetachedEntityKey != (EntityKey) null))
        return;
      EntityKey detachedEntityKey = this.DetachedEntityKey;
      if (!RelatedEnd.IsValidEntityKeyType(detachedEntityKey))
        throw Error.EntityReference_CannotSetSpecialKeys();
      EntitySet entitySet = detachedEntityKey.GetEntitySet(this.ObjectContext.MetadataWorkspace);
      this.CheckRelationEntitySet(entitySet);
      detachedEntityKey.ValidateEntityKey(this.ObjectContext.MetadataWorkspace, entitySet);
    }

    internal override void VerifyDetachedKeyMatches(EntityKey entityKey)
    {
      if (!(this.DetachedEntityKey != (EntityKey) null))
        return;
      EntityKey entityKey1 = entityKey;
      if (!(this.DetachedEntityKey != entityKey1))
        return;
      if (entityKey1.IsTemporary)
        throw Error.RelatedEnd_CannotCreateRelationshipBetweenTrackedAndNoTrackedEntities((object) this.RelationshipNavigation.To);
      throw new InvalidOperationException(Strings.EntityReference_EntityKeyValueMismatch);
    }

    internal override void DetachAll(EntityState ownerEntityState)
    {
      this.DetachedEntityKey = this.AttachedEntityKey;
      base.DetachAll(ownerEntityState);
      if (!this.IsForeignKey)
        return;
      this.DetachedEntityKey = (EntityKey) null;
    }

    internal override bool CheckReferentialConstraintPrincipalProperty(
      EntityEntry ownerEntry,
      ReferentialConstraint constraint)
    {
      EntityKey principalKey;
      if (!this.IsEmpty())
      {
        IEntityWrapper referenceValue = this.ReferenceValue;
        if (referenceValue.ObjectStateEntry != null && referenceValue.ObjectStateEntry.State == EntityState.Added)
          return true;
        principalKey = this.ExtractPrincipalKey(referenceValue);
      }
      else
      {
        if (this.ToEndMember.RelationshipMultiplicity != RelationshipMultiplicity.ZeroOrOne && this.ToEndMember.RelationshipMultiplicity != RelationshipMultiplicity.One || !(this.DetachedEntityKey != (EntityKey) null))
          return true;
        principalKey = !this.IsForeignKey || this.ObjectContext.ObjectStateManager.TransactionManager.IsAddTracking || this.ObjectContext.ObjectStateManager.TransactionManager.IsAttachTracking ? this.DetachedEntityKey : this.EntityKey;
      }
      return RelatedEnd.VerifyRIConstraintsWithRelatedEntry(constraint, new Func<string, object>(ownerEntry.GetCurrentEntityValue), principalKey);
    }

    internal override bool CheckReferentialConstraintDependentProperty(
      EntityEntry ownerEntry,
      ReferentialConstraint constraint)
    {
      if (!this.IsEmpty())
        return base.CheckReferentialConstraintDependentProperty(ownerEntry, constraint);
      if ((this.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne || this.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One) && this.DetachedEntityKey != (EntityKey) null)
      {
        EntityKey detachedEntityKey = this.DetachedEntityKey;
        if (!RelatedEnd.VerifyRIConstraintsWithRelatedEntry(constraint, new Func<string, object>(detachedEntityKey.FindValueByName), ownerEntry.EntityKey))
          return false;
      }
      return true;
    }

    private EntityKey ExtractPrincipalKey(IEntityWrapper wrappedRelatedEntity)
    {
      EntitySet fromRelationshipSet = this.GetTargetEntitySetFromRelationshipSet();
      EntityKey entityKey = wrappedRelatedEntity.EntityKey;
      if ((object) entityKey != null && !entityKey.IsTemporary)
      {
        EntityUtil.ValidateEntitySetInKey(entityKey, fromRelationshipSet);
        entityKey.ValidateEntityKey(this.ObjectContext.MetadataWorkspace, fromRelationshipSet);
      }
      else
        entityKey = this.ObjectContext.ObjectStateManager.CreateEntityKey(fromRelationshipSet, wrappedRelatedEntity.Entity);
      return entityKey;
    }

    internal void NullAllForeignKeys()
    {
      ObjectStateManager objectStateManager = this.ObjectContext.ObjectStateManager;
      EntityEntry objectStateEntry = this.WrappedOwner.ObjectStateEntry;
      TransactionManager transactionManager = objectStateManager.TransactionManager;
      if (transactionManager.IsGraphUpdate || transactionManager.IsAttachTracking || transactionManager.IsRelatedEndAdd)
        return;
      ReferentialConstraint referentialConstraint = ((AssociationType) this.RelationMetadata).ReferentialConstraints.Single<ReferentialConstraint>();
      if (!(this.TargetRoleName == referentialConstraint.FromRole.Name))
        return;
      if (transactionManager.IsDetaching)
      {
        EntityKey foreignKeyValues = ForeignKeyFactory.CreateKeyFromForeignKeyValues(objectStateEntry, (RelatedEnd) this);
        if (!(foreignKeyValues != (EntityKey) null))
          return;
        objectStateManager.AddEntryContainingForeignKeyToIndex(this, foreignKeyValues, objectStateEntry);
      }
      else
      {
        if (object.ReferenceEquals(objectStateManager.EntityInvokingFKSetter, this.WrappedOwner.Entity) || transactionManager.IsForeignKeyUpdate)
          return;
        transactionManager.BeginForeignKeyUpdate(this);
        try
        {
          bool flag1 = true;
          bool flag2 = objectStateEntry != null && (objectStateEntry.State == EntityState.Modified || objectStateEntry.State == EntityState.Unchanged);
          EntitySet entitySet = ((AssociationSet) this.RelationshipSet).AssociationSetEnds[this.FromEndMember.Name].EntitySet;
          StateManagerTypeMetadata managerTypeMetadata = objectStateManager.GetOrAddStateManagerTypeMetadata(this.WrappedOwner.IdentityType, entitySet);
          for (int index = 0; index < referentialConstraint.FromProperties.Count; ++index)
          {
            string name = referentialConstraint.ToProperties[index].Name;
            int olayerMemberName = managerTypeMetadata.GetOrdinalforOLayerMemberName(name);
            StateManagerMemberMetadata managerMemberMetadata = managerTypeMetadata.Member(olayerMemberName);
            if (managerMemberMetadata.ClrMetadata.Nullable)
            {
              if (managerMemberMetadata.GetValue(this.WrappedOwner.Entity) != null)
                this.WrappedOwner.SetCurrentValue(this.WrappedOwner.ObjectStateEntry, managerTypeMetadata.Member(olayerMemberName), -1, this.WrappedOwner.Entity, (object) null);
              else if (flag2 && this.WrappedOwner.ObjectStateEntry.OriginalValues.GetValue(olayerMemberName) != null)
                objectStateEntry.SetModifiedProperty(name);
              flag1 = false;
            }
            else if (flag2)
              objectStateEntry.SetModifiedProperty(name);
          }
          if (flag1)
          {
            if (objectStateEntry == null)
              return;
            EntityKey originalKey = this.CachedForeignKey;
            if (originalKey == (EntityKey) null)
              originalKey = ForeignKeyFactory.CreateKeyFromForeignKeyValues(objectStateEntry, (RelatedEnd) this);
            if (!(originalKey != (EntityKey) null))
              return;
            this.SetCachedForeignKey(ForeignKeyFactory.CreateConceptualNullKey(originalKey), objectStateEntry);
            objectStateManager.RememberEntryWithConceptualNull(objectStateEntry);
          }
          else
            this.SetCachedForeignKey((EntityKey) null, objectStateEntry);
        }
        finally
        {
          transactionManager.EndForeignKeyUpdate();
        }
      }
    }
  }
}
