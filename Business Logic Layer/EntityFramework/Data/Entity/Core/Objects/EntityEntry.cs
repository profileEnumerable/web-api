// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.EntityEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Objects
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal sealed class EntityEntry : ObjectStateEntry
  {
    internal const int s_EntityRoot = -1;
    private StateManagerTypeMetadata _cacheTypeMetadata;
    private EntityKey _entityKey;
    private IEntityWrapper _wrappedEntity;
    private BitArray _modifiedFields;
    private List<StateManagerValue> _originalValues;
    private Dictionary<object, Dictionary<int, object>> _originalComplexObjects;
    private bool _requiresComplexChangeTracking;
    private bool _requiresScalarChangeTracking;
    private bool _requiresAnyChangeTracking;
    private RelationshipEntry _headRelationshipEnds;
    private int _countRelationshipEnds;

    internal EntityEntry()
      : base(new ObjectStateManager(), (EntitySet) null, EntityState.Unchanged)
    {
    }

    internal EntityEntry(ObjectStateManager stateManager)
      : base(stateManager, (EntitySet) null, EntityState.Unchanged)
    {
    }

    internal EntityEntry(
      IEntityWrapper wrappedEntity,
      EntityKey entityKey,
      EntitySet entitySet,
      ObjectStateManager cache,
      StateManagerTypeMetadata typeMetadata,
      EntityState state)
      : base(cache, entitySet, state)
    {
      this._wrappedEntity = wrappedEntity;
      this._cacheTypeMetadata = typeMetadata;
      this._entityKey = entityKey;
      wrappedEntity.ObjectStateEntry = this;
      this.SetChangeTrackingFlags();
    }

    private void SetChangeTrackingFlags()
    {
      this._requiresScalarChangeTracking = this.Entity != null && !(this.Entity is IEntityWithChangeTracker);
      this._requiresComplexChangeTracking = this.Entity != null && (this._requiresScalarChangeTracking || this.WrappedEntity.IdentityType != this.Entity.GetType() && this._cacheTypeMetadata.Members.Any<StateManagerMemberMetadata>((Func<StateManagerMemberMetadata, bool>) (m => m.IsComplex)));
      this._requiresAnyChangeTracking = this.Entity != null && (!(this.Entity is IEntityWithRelationships) || this._requiresComplexChangeTracking || this._requiresScalarChangeTracking);
    }

    internal EntityEntry(
      EntityKey entityKey,
      EntitySet entitySet,
      ObjectStateManager cache,
      StateManagerTypeMetadata typeMetadata)
      : base(cache, entitySet, EntityState.Unchanged)
    {
      this._wrappedEntity = NullEntityWrapper.NullWrapper;
      this._entityKey = entityKey;
      this._cacheTypeMetadata = typeMetadata;
      this.SetChangeTrackingFlags();
    }

    public override bool IsRelationship
    {
      get
      {
        this.ValidateState();
        return false;
      }
    }

    public override object Entity
    {
      get
      {
        this.ValidateState();
        return this._wrappedEntity.Entity;
      }
    }

    public override EntityKey EntityKey
    {
      get
      {
        this.ValidateState();
        return this._entityKey;
      }
      internal set
      {
        this._entityKey = value;
      }
    }

    internal IEnumerable<Tuple<AssociationSet, ReferentialConstraint>> ForeignKeyDependents
    {
      get
      {
        foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyDependent in ((EntitySet) this.EntitySet).ForeignKeyDependents)
        {
          ReferentialConstraint constraint = foreignKeyDependent.Item2;
          EntityType dependentType = MetadataHelper.GetEntityTypeForEnd((AssociationEndMember) constraint.ToRole);
          if (dependentType.IsAssignableFrom(this._cacheTypeMetadata.DataRecordInfo.RecordType.EdmType))
            yield return foreignKeyDependent;
        }
      }
    }

    internal IEnumerable<Tuple<AssociationSet, ReferentialConstraint>> ForeignKeyPrincipals
    {
      get
      {
        foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyPrincipal in ((EntitySet) this.EntitySet).ForeignKeyPrincipals)
        {
          ReferentialConstraint constraint = foreignKeyPrincipal.Item2;
          EntityType dependentType = MetadataHelper.GetEntityTypeForEnd((AssociationEndMember) constraint.FromRole);
          if (dependentType.IsAssignableFrom(this._cacheTypeMetadata.DataRecordInfo.RecordType.EdmType))
            yield return foreignKeyPrincipal;
        }
      }
    }

    public override IEnumerable<string> GetModifiedProperties()
    {
      this.ValidateState();
      if (EntityState.Modified == this.State && this._modifiedFields != null)
      {
        for (int i = 0; i < this._modifiedFields.Length; ++i)
        {
          if (this._modifiedFields[i])
            yield return this.GetCLayerName(i, this._cacheTypeMetadata);
        }
      }
    }

    public override void SetModifiedProperty(string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      int ordinalForProperty = this.ValidateAndGetOrdinalForProperty(propertyName, nameof (SetModifiedProperty));
      if (EntityState.Unchanged == this.State)
      {
        this.State = EntityState.Modified;
        this._cache.ChangeState(this, EntityState.Unchanged, this.State);
      }
      this.SetModifiedPropertyInternal(ordinalForProperty);
    }

    internal void SetModifiedPropertyInternal(int ordinal)
    {
      if (this._modifiedFields == null)
        this._modifiedFields = new BitArray(this.GetFieldCount(this._cacheTypeMetadata));
      this._modifiedFields[ordinal] = true;
    }

    private int ValidateAndGetOrdinalForProperty(string propertyName, string methodName)
    {
      this.ValidateState();
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotModifyKeyEntryState);
      int olayerMemberName = this._cacheTypeMetadata.GetOrdinalforOLayerMemberName(propertyName);
      if (olayerMemberName == -1)
        throw new ArgumentException(Strings.ObjectStateEntry_SetModifiedOnInvalidProperty((object) propertyName));
      if (this.State == EntityState.Added || this.State == EntityState.Deleted)
        throw new InvalidOperationException(Strings.ObjectStateEntry_SetModifiedStates((object) methodName));
      return olayerMemberName;
    }

    public override void RejectPropertyChanges(string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      int ordinalForProperty = this.ValidateAndGetOrdinalForProperty(propertyName, nameof (RejectPropertyChanges));
      if (this.State == EntityState.Unchanged || this._modifiedFields == null || !this._modifiedFields[ordinalForProperty])
        return;
      this.DetectChangesInComplexProperties();
      object originalEntityValue = this.GetOriginalEntityValue(this._cacheTypeMetadata, ordinalForProperty, this._wrappedEntity.Entity, ObjectStateValueRecord.OriginalReadonly);
      this.SetCurrentEntityValue(this._cacheTypeMetadata, ordinalForProperty, this._wrappedEntity.Entity, originalEntityValue);
      this._modifiedFields[ordinalForProperty] = false;
      for (int index = 0; index < this._modifiedFields.Length; ++index)
      {
        if (this._modifiedFields[index])
          return;
      }
      this.ChangeObjectState(EntityState.Unchanged);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public override DbDataRecord OriginalValues
    {
      get
      {
        return this.InternalGetOriginalValues(true);
      }
    }

    public override OriginalValueRecord GetUpdatableOriginalValues()
    {
      return (OriginalValueRecord) this.InternalGetOriginalValues(false);
    }

    private DbDataRecord InternalGetOriginalValues(bool readOnly)
    {
      this.ValidateState();
      if (this.State == EntityState.Added)
        throw new InvalidOperationException(Strings.ObjectStateEntry_OriginalValuesDoesNotExist);
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotAccessKeyEntryValues);
      this.DetectChangesInComplexProperties();
      if (readOnly)
        return (DbDataRecord) new ObjectStateEntryDbDataRecord(this, this._cacheTypeMetadata, this._wrappedEntity.Entity);
      return (DbDataRecord) new ObjectStateEntryOriginalDbUpdatableDataRecord_Public(this, this._cacheTypeMetadata, this._wrappedEntity.Entity, -1);
    }

    private void DetectChangesInComplexProperties()
    {
      if (!this.RequiresScalarChangeTracking)
        return;
      this.ObjectStateManager.TransactionManager.BeginOriginalValuesGetter();
      try
      {
        this.DetectChangesInProperties(true);
      }
      finally
      {
        this.ObjectStateManager.TransactionManager.EndOriginalValuesGetter();
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public override CurrentValueRecord CurrentValues
    {
      get
      {
        this.ValidateState();
        if (this.State == EntityState.Deleted)
          throw new InvalidOperationException(Strings.ObjectStateEntry_CurrentValuesDoesNotExist);
        if (this.IsKeyEntry)
          throw new InvalidOperationException(Strings.ObjectStateEntry_CannotAccessKeyEntryValues);
        return (CurrentValueRecord) new ObjectStateEntryDbUpdatableDataRecord(this, this._cacheTypeMetadata, this._wrappedEntity.Entity);
      }
    }

    public override void Delete()
    {
      this.Delete(true);
    }

    public override void AcceptChanges()
    {
      this.ValidateState();
      if (this.ObjectStateManager.EntryHasConceptualNull(this))
        throw new InvalidOperationException(Strings.ObjectContext_CommitWithConceptualNull);
      switch (this.State)
      {
        case EntityState.Added:
          bool flag = this.RetrieveAndCheckReferentialConstraintValuesInAcceptChanges();
          this._cache.FixupKey(this);
          this._modifiedFields = (BitArray) null;
          this._originalValues = (List<StateManagerValue>) null;
          this._originalComplexObjects = (Dictionary<object, Dictionary<int, object>>) null;
          this.State = EntityState.Unchanged;
          if (flag)
            this.RelationshipManager.CheckReferentialConstraintProperties(this);
          this._wrappedEntity.TakeSnapshot(this);
          break;
        case EntityState.Deleted:
          this.CascadeAcceptChanges();
          if (this._cache == null)
            break;
          this._cache.ChangeState(this, EntityState.Deleted, EntityState.Detached);
          break;
        case EntityState.Modified:
          this._cache.ChangeState(this, EntityState.Modified, EntityState.Unchanged);
          this._modifiedFields = (BitArray) null;
          this._originalValues = (List<StateManagerValue>) null;
          this._originalComplexObjects = (Dictionary<object, Dictionary<int, object>>) null;
          this.State = EntityState.Unchanged;
          this._cache.FixupReferencesByForeignKeys(this, false);
          this.RelationshipManager.CheckReferentialConstraintProperties(this);
          this._wrappedEntity.TakeSnapshot(this);
          break;
      }
    }

    public override void SetModified()
    {
      this.ValidateState();
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotModifyKeyEntryState);
      if (EntityState.Unchanged == this.State)
      {
        this.State = EntityState.Modified;
        this._cache.ChangeState(this, EntityState.Unchanged, this.State);
      }
      else if (EntityState.Modified != this.State)
        throw new InvalidOperationException(Strings.ObjectStateEntry_SetModifiedStates((object) nameof (SetModified)));
    }

    public override RelationshipManager RelationshipManager
    {
      get
      {
        this.ValidateState();
        if (this.IsKeyEntry)
          throw new InvalidOperationException(Strings.ObjectStateEntry_RelationshipAndKeyEntriesDoNotHaveRelationshipManagers);
        if (this.WrappedEntity.Entity == null)
          throw new InvalidOperationException(Strings.ObjectStateManager_CannotGetRelationshipManagerForDetachedPocoEntity);
        return this.WrappedEntity.RelationshipManager;
      }
    }

    internal override BitArray ModifiedProperties
    {
      get
      {
        return this._modifiedFields;
      }
    }

    public override void ChangeState(EntityState state)
    {
      EntityUtil.CheckValidStateForChangeEntityState(state);
      if (this.State == EntityState.Detached && state == EntityState.Detached)
        return;
      this.ValidateState();
      ObjectStateManager objectStateManager = this.ObjectStateManager;
      objectStateManager.TransactionManager.BeginLocalPublicAPI();
      try
      {
        this.ChangeObjectState(state);
      }
      finally
      {
        objectStateManager.TransactionManager.EndLocalPublicAPI();
      }
    }

    public override void ApplyCurrentValues(object currentEntity)
    {
      Check.NotNull<object>(currentEntity, nameof (currentEntity));
      this.ValidateState();
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotAccessKeyEntryValues);
      this.ApplyCurrentValuesInternal(this.ObjectStateManager.EntityWrapperFactory.WrapEntityUsingStateManager(currentEntity, this.ObjectStateManager));
    }

    public override void ApplyOriginalValues(object originalEntity)
    {
      Check.NotNull<object>(originalEntity, nameof (originalEntity));
      this.ValidateState();
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotAccessKeyEntryValues);
      this.ApplyOriginalValuesInternal(this.ObjectStateManager.EntityWrapperFactory.WrapEntityUsingStateManager(originalEntity, this.ObjectStateManager));
    }

    internal void AddRelationshipEnd(RelationshipEntry item)
    {
      item.SetNextRelationshipEnd(this.EntityKey, this._headRelationshipEnds);
      this._headRelationshipEnds = item;
      ++this._countRelationshipEnds;
    }

    internal bool ContainsRelationshipEnd(RelationshipEntry item)
    {
      for (RelationshipEntry relationshipEntry = this._headRelationshipEnds; relationshipEntry != null; relationshipEntry = relationshipEntry.GetNextRelationshipEnd(this.EntityKey))
      {
        if (object.ReferenceEquals((object) relationshipEntry, (object) item))
          return true;
      }
      return false;
    }

    internal void RemoveRelationshipEnd(RelationshipEntry item)
    {
      RelationshipEntry relationshipEntry1 = this._headRelationshipEnds;
      RelationshipEntry relationshipEntry2 = (RelationshipEntry) null;
      bool flag1 = false;
      while (relationshipEntry1 != null)
      {
        bool flag2 = object.ReferenceEquals((object) this.EntityKey, (object) relationshipEntry1.Key0) || !object.ReferenceEquals((object) this.EntityKey, (object) relationshipEntry1.Key1) && this.EntityKey.Equals(relationshipEntry1.Key0);
        if (object.ReferenceEquals((object) item, (object) relationshipEntry1))
        {
          RelationshipEntry relationshipEntry3;
          if (flag2)
          {
            relationshipEntry3 = relationshipEntry1.NextKey0;
            relationshipEntry1.NextKey0 = (RelationshipEntry) null;
          }
          else
          {
            relationshipEntry3 = relationshipEntry1.NextKey1;
            relationshipEntry1.NextKey1 = (RelationshipEntry) null;
          }
          if (relationshipEntry2 == null)
            this._headRelationshipEnds = relationshipEntry3;
          else if (flag1)
            relationshipEntry2.NextKey0 = relationshipEntry3;
          else
            relationshipEntry2.NextKey1 = relationshipEntry3;
          --this._countRelationshipEnds;
          break;
        }
        relationshipEntry2 = relationshipEntry1;
        relationshipEntry1 = flag2 ? relationshipEntry1.NextKey0 : relationshipEntry1.NextKey1;
        flag1 = flag2;
      }
    }

    internal void UpdateRelationshipEnds(EntityKey oldKey, EntityEntry promotedEntry)
    {
      int num = 0;
      RelationshipEntry relationshipEntry1 = this._headRelationshipEnds;
      while (relationshipEntry1 != null)
      {
        RelationshipEntry relationshipEntry2 = relationshipEntry1;
        relationshipEntry1 = relationshipEntry1.GetNextRelationshipEnd(oldKey);
        relationshipEntry2.ChangeRelatedEnd(oldKey, this.EntityKey);
        if (promotedEntry != null && !promotedEntry.ContainsRelationshipEnd(relationshipEntry2))
          promotedEntry.AddRelationshipEnd(relationshipEntry2);
        ++num;
      }
      if (promotedEntry == null)
        return;
      this._headRelationshipEnds = (RelationshipEntry) null;
      this._countRelationshipEnds = 0;
    }

    internal EntityEntry.RelationshipEndEnumerable GetRelationshipEnds()
    {
      return new EntityEntry.RelationshipEndEnumerable(this);
    }

    internal override bool IsKeyEntry
    {
      get
      {
        return null == this._wrappedEntity.Entity;
      }
    }

    internal override DataRecordInfo GetDataRecordInfo(
      StateManagerTypeMetadata metadata,
      object userObject)
    {
      if (Helper.IsEntityType(metadata.CdmMetadata.EdmType) && (object) this._entityKey != null)
        return (DataRecordInfo) new EntityRecordInfo(metadata.DataRecordInfo, this._entityKey, (EntitySet) this.EntitySet);
      return metadata.DataRecordInfo;
    }

    internal override void Reset()
    {
      this.RemoveFromForeignKeyIndex();
      this._cache.ForgetEntryWithConceptualNull(this, true);
      this.DetachObjectStateManagerFromEntity();
      this._wrappedEntity = NullEntityWrapper.NullWrapper;
      this._entityKey = (EntityKey) null;
      this._modifiedFields = (BitArray) null;
      this._originalValues = (List<StateManagerValue>) null;
      this._originalComplexObjects = (Dictionary<object, Dictionary<int, object>>) null;
      this.SetChangeTrackingFlags();
      base.Reset();
    }

    internal override Type GetFieldType(int ordinal, StateManagerTypeMetadata metadata)
    {
      return metadata.GetFieldType(ordinal);
    }

    internal override string GetCLayerName(int ordinal, StateManagerTypeMetadata metadata)
    {
      return metadata.CLayerMemberName(ordinal);
    }

    internal override int GetOrdinalforCLayerName(string name, StateManagerTypeMetadata metadata)
    {
      return metadata.GetOrdinalforCLayerMemberName(name);
    }

    internal override void RevertDelete()
    {
      this.State = this._modifiedFields == null ? EntityState.Unchanged : EntityState.Modified;
      this._cache.ChangeState(this, EntityState.Deleted, this.State);
    }

    internal override int GetFieldCount(StateManagerTypeMetadata metadata)
    {
      return metadata.FieldCount;
    }

    private void CascadeAcceptChanges()
    {
      foreach (ObjectStateEntry objectStateEntry in this._cache.CopyOfRelationshipsByKey(this.EntityKey))
        objectStateEntry.AcceptChanges();
    }

    internal override void SetModifiedAll()
    {
      this.ValidateState();
      if (this._modifiedFields == null)
        this._modifiedFields = new BitArray(this.GetFieldCount(this._cacheTypeMetadata));
      this._modifiedFields.SetAll(true);
    }

    internal override void EntityMemberChanging(string entityMemberName)
    {
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotAccessKeyEntryValues);
      this.EntityMemberChanging(entityMemberName, (object) null, (string) null);
    }

    internal override void EntityMemberChanged(string entityMemberName)
    {
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotAccessKeyEntryValues);
      this.EntityMemberChanged(entityMemberName, (object) null, (string) null);
    }

    internal override void EntityComplexMemberChanging(
      string entityMemberName,
      object complexObject,
      string complexObjectMemberName)
    {
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotAccessKeyEntryValues);
      this.EntityMemberChanging(entityMemberName, complexObject, complexObjectMemberName);
    }

    internal override void EntityComplexMemberChanged(
      string entityMemberName,
      object complexObject,
      string complexObjectMemberName)
    {
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotAccessKeyEntryValues);
      this.EntityMemberChanged(entityMemberName, complexObject, complexObjectMemberName);
    }

    internal IEntityWrapper WrappedEntity
    {
      get
      {
        return this._wrappedEntity;
      }
    }

    private void EntityMemberChanged(
      string entityMemberName,
      object complexObject,
      string complexObjectMemberName)
    {
      try
      {
        StateManagerTypeMetadata typeMetadata;
        string changingMemberName;
        object changingObject;
        int changeMemberInfo = this.GetAndValidateChangeMemberInfo(entityMemberName, complexObject, complexObjectMemberName, out typeMetadata, out changingMemberName, out changingObject);
        if (changeMemberInfo == -2)
          return;
        if (changingObject != this._cache.ChangingObject || changingMemberName != this._cache.ChangingMember || entityMemberName != this._cache.ChangingEntityMember)
          throw new InvalidOperationException(Strings.ObjectStateEntry_EntityMemberChangedWithoutEntityMemberChanging);
        if (this.State != this._cache.ChangingState)
          throw new InvalidOperationException(Strings.ObjectStateEntry_ChangedInDifferentStateFromChanging((object) this._cache.ChangingState, (object) this.State));
        object changingOldValue = this._cache.ChangingOldValue;
        object obj = (object) null;
        StateManagerMemberMetadata memberMetadata = (StateManagerMemberMetadata) null;
        if (this._cache.SaveOriginalValues)
        {
          memberMetadata = typeMetadata.Member(changeMemberInfo);
          if (memberMetadata.IsComplex && changingOldValue != null)
          {
            obj = memberMetadata.GetValue(changingObject);
            this.ExpandComplexTypeAndAddValues(memberMetadata, changingOldValue, obj, false);
          }
          else
            this.AddOriginalValueAt(-1, memberMetadata, changingObject, changingOldValue);
        }
        TransactionManager transactionManager = this.ObjectStateManager.TransactionManager;
        List<Pair<string, string>> relationships;
        if (complexObject == null && (transactionManager.IsAlignChanges || !transactionManager.IsDetectChanges) && this.IsPropertyAForeignKey(entityMemberName, out relationships))
        {
          foreach (Pair<string, string> pair in relationships)
          {
            EntityReference relatedEndInternal = this.WrappedEntity.RelationshipManager.GetRelatedEndInternal(pair.First, pair.Second) as EntityReference;
            if (!transactionManager.IsFixupByReference)
            {
              if (memberMetadata == null)
                memberMetadata = typeMetadata.Member(changeMemberInfo);
              if (obj == null)
                obj = memberMetadata.GetValue(changingObject);
              bool flag = ForeignKeyFactory.IsConceptualNullKey(relatedEndInternal.CachedForeignKey);
              if (!ByValueEqualityComparer.Default.Equals(changingOldValue, obj) || flag)
                this.FixupEntityReferenceByForeignKey(relatedEndInternal);
            }
          }
        }
        if (this._cache == null || this._cache.TransactionManager.IsOriginalValuesGetter)
          return;
        EntityState state = this.State;
        if (this.State != EntityState.Added)
          this.State = EntityState.Modified;
        if (this.State == EntityState.Modified)
          this.SetModifiedProperty(entityMemberName);
        if (state == this.State)
          return;
        this._cache.ChangeState(this, state, this.State);
      }
      finally
      {
        this.SetCachedChangingValues((string) null, (object) null, (string) null, EntityState.Detached, (object) null);
      }
    }

    internal void SetCurrentEntityValue(string memberName, object newValue)
    {
      this.SetCurrentEntityValue(this._cacheTypeMetadata, this._cacheTypeMetadata.GetOrdinalforOLayerMemberName(memberName), this._wrappedEntity.Entity, newValue);
    }

    internal void SetOriginalEntityValue(
      StateManagerTypeMetadata metadata,
      int ordinal,
      object userObject,
      object newValue)
    {
      this.ValidateState();
      if (this.State == EntityState.Added)
        throw new InvalidOperationException(Strings.ObjectStateEntry_OriginalValuesDoesNotExist);
      EntityState state = this.State;
      StateManagerMemberMetadata managerMemberMetadata = metadata.Member(ordinal);
      int originalValueIndex = this.FindOriginalValueIndex(managerMemberMetadata, userObject);
      if (managerMemberMetadata.IsComplex)
      {
        if (originalValueIndex >= 0)
          this._originalValues.RemoveAt(originalValueIndex);
        object oldComplexObject = managerMemberMetadata.GetValue(userObject);
        if (oldComplexObject == null)
          throw new InvalidOperationException(Strings.ComplexObject_NullableComplexTypesNotSupported((object) managerMemberMetadata.CLayerName));
        IExtendedDataRecord record = newValue as IExtendedDataRecord;
        if (record != null)
          newValue = this._cache.ComplexTypeMaterializer.CreateComplex(record, record.DataRecordInfo, (object) null);
        this.ExpandComplexTypeAndAddValues(managerMemberMetadata, oldComplexObject, newValue, true);
      }
      else
        this.AddOriginalValueAt(originalValueIndex, managerMemberMetadata, userObject, newValue);
      if (state != EntityState.Unchanged)
        return;
      this.State = EntityState.Modified;
    }

    private void EntityMemberChanging(
      string entityMemberName,
      object complexObject,
      string complexObjectMemberName)
    {
      StateManagerTypeMetadata typeMetadata;
      string changingMemberName;
      object changingObject;
      int changeMemberInfo = this.GetAndValidateChangeMemberInfo(entityMemberName, complexObject, complexObjectMemberName, out typeMetadata, out changingMemberName, out changingObject);
      if (changeMemberInfo == -2)
        return;
      StateManagerMemberMetadata metadata = typeMetadata.Member(changeMemberInfo);
      this._cache.SaveOriginalValues = (this.State == EntityState.Unchanged || this.State == EntityState.Modified) && this.FindOriginalValueIndex(metadata, changingObject) == -1;
      object oldValue = metadata.GetValue(changingObject);
      this.SetCachedChangingValues(entityMemberName, changingObject, changingMemberName, this.State, oldValue);
    }

    internal object GetOriginalEntityValue(string memberName)
    {
      return this.GetOriginalEntityValue(this._cacheTypeMetadata, this._cacheTypeMetadata.GetOrdinalforOLayerMemberName(memberName), this._wrappedEntity.Entity, ObjectStateValueRecord.OriginalReadonly);
    }

    internal object GetOriginalEntityValue(
      StateManagerTypeMetadata metadata,
      int ordinal,
      object userObject,
      ObjectStateValueRecord updatableRecord)
    {
      return this.GetOriginalEntityValue(metadata, ordinal, userObject, updatableRecord, -1);
    }

    internal object GetOriginalEntityValue(
      StateManagerTypeMetadata metadata,
      int ordinal,
      object userObject,
      ObjectStateValueRecord updatableRecord,
      int parentEntityPropertyIndex)
    {
      this.ValidateState();
      return this.GetOriginalEntityValue(metadata, metadata.Member(ordinal), ordinal, userObject, updatableRecord, parentEntityPropertyIndex);
    }

    internal object GetOriginalEntityValue(
      StateManagerTypeMetadata metadata,
      StateManagerMemberMetadata memberMetadata,
      int ordinal,
      object userObject,
      ObjectStateValueRecord updatableRecord,
      int parentEntityPropertyIndex)
    {
      int originalValueIndex = this.FindOriginalValueIndex(memberMetadata, userObject);
      if (originalValueIndex >= 0)
        return this._originalValues[originalValueIndex].OriginalValue ?? (object) DBNull.Value;
      return this.GetCurrentEntityValue(metadata, ordinal, userObject, updatableRecord, parentEntityPropertyIndex);
    }

    internal object GetCurrentEntityValue(
      StateManagerTypeMetadata metadata,
      int ordinal,
      object userObject,
      ObjectStateValueRecord updatableRecord)
    {
      return this.GetCurrentEntityValue(metadata, ordinal, userObject, updatableRecord, -1);
    }

    internal object GetCurrentEntityValue(
      StateManagerTypeMetadata metadata,
      int ordinal,
      object userObject,
      ObjectStateValueRecord updatableRecord,
      int parentEntityPropertyIndex)
    {
      this.ValidateState();
      StateManagerMemberMetadata managerMemberMetadata = metadata.Member(ordinal);
      object userObject1 = managerMemberMetadata.GetValue(userObject);
      if (managerMemberMetadata.IsComplex && userObject1 != null)
      {
        switch (updatableRecord)
        {
          case ObjectStateValueRecord.OriginalReadonly:
            userObject1 = (object) new ObjectStateEntryDbDataRecord(this, this._cache.GetOrAddStateManagerTypeMetadata(managerMemberMetadata.CdmMetadata.TypeUsage.EdmType), userObject1);
            break;
          case ObjectStateValueRecord.CurrentUpdatable:
            userObject1 = (object) new ObjectStateEntryDbUpdatableDataRecord(this, this._cache.GetOrAddStateManagerTypeMetadata(managerMemberMetadata.CdmMetadata.TypeUsage.EdmType), userObject1);
            break;
          case ObjectStateValueRecord.OriginalUpdatableInternal:
            userObject1 = (object) new ObjectStateEntryOriginalDbUpdatableDataRecord_Internal(this, this._cache.GetOrAddStateManagerTypeMetadata(managerMemberMetadata.CdmMetadata.TypeUsage.EdmType), userObject1);
            break;
          case ObjectStateValueRecord.OriginalUpdatablePublic:
            userObject1 = (object) new ObjectStateEntryOriginalDbUpdatableDataRecord_Public(this, this._cache.GetOrAddStateManagerTypeMetadata(managerMemberMetadata.CdmMetadata.TypeUsage.EdmType), userObject1, parentEntityPropertyIndex);
            break;
        }
      }
      return userObject1 ?? (object) DBNull.Value;
    }

    internal int FindOriginalValueIndex(StateManagerMemberMetadata metadata, object instance)
    {
      if (this._originalValues != null)
      {
        for (int index = 0; index < this._originalValues.Count; ++index)
        {
          if (object.ReferenceEquals(this._originalValues[index].UserObject, instance) && object.ReferenceEquals((object) this._originalValues[index].MemberMetadata, (object) metadata))
            return index;
        }
      }
      return -1;
    }

    internal AssociationEndMember GetAssociationEndMember(
      RelationshipEntry relationshipEntry)
    {
      this.ValidateState();
      return relationshipEntry.RelationshipWrapper.GetAssociationEndMember(this.EntityKey);
    }

    internal EntityEntry GetOtherEndOfRelationship(RelationshipEntry relationshipEntry)
    {
      return this._cache.GetEntityEntry(relationshipEntry.RelationshipWrapper.GetOtherEntityKey(this.EntityKey));
    }

    internal void ExpandComplexTypeAndAddValues(
      StateManagerMemberMetadata memberMetadata,
      object oldComplexObject,
      object newComplexObject,
      bool useOldComplexObject)
    {
      if (newComplexObject == null)
        throw new InvalidOperationException(Strings.ComplexObject_NullableComplexTypesNotSupported((object) memberMetadata.CLayerName));
      StateManagerTypeMetadata managerTypeMetadata = this._cache.GetOrAddStateManagerTypeMetadata(memberMetadata.CdmMetadata.TypeUsage.EdmType);
      for (int ordinal = 0; ordinal < managerTypeMetadata.FieldCount; ++ordinal)
      {
        StateManagerMemberMetadata managerMemberMetadata = managerTypeMetadata.Member(ordinal);
        if (managerMemberMetadata.IsComplex)
        {
          object oldComplexObject1 = (object) null;
          if (oldComplexObject != null)
          {
            oldComplexObject1 = managerMemberMetadata.GetValue(oldComplexObject);
            if (oldComplexObject1 == null)
            {
              int originalValueIndex = this.FindOriginalValueIndex(managerMemberMetadata, oldComplexObject);
              if (originalValueIndex >= 0)
                this._originalValues.RemoveAt(originalValueIndex);
            }
          }
          this.ExpandComplexTypeAndAddValues(managerMemberMetadata, oldComplexObject1, managerMemberMetadata.GetValue(newComplexObject), useOldComplexObject);
        }
        else
        {
          object userObject = newComplexObject;
          int index = -1;
          object originalValue;
          if (useOldComplexObject)
          {
            originalValue = managerMemberMetadata.GetValue(newComplexObject);
            userObject = oldComplexObject;
          }
          else if (oldComplexObject != null)
          {
            originalValue = managerMemberMetadata.GetValue(oldComplexObject);
            index = this.FindOriginalValueIndex(managerMemberMetadata, oldComplexObject);
            if (index >= 0)
              originalValue = this._originalValues[index].OriginalValue;
          }
          else
            originalValue = managerMemberMetadata.GetValue(newComplexObject);
          this.AddOriginalValueAt(index, managerMemberMetadata, userObject, originalValue);
        }
      }
    }

    internal int GetAndValidateChangeMemberInfo(
      string entityMemberName,
      object complexObject,
      string complexObjectMemberName,
      out StateManagerTypeMetadata typeMetadata,
      out string changingMemberName,
      out object changingObject)
    {
      Check.NotNull<string>(entityMemberName, nameof (entityMemberName));
      typeMetadata = (StateManagerTypeMetadata) null;
      changingMemberName = (string) null;
      changingObject = (object) null;
      this.ValidateState();
      int olayerMemberName = this._cacheTypeMetadata.GetOrdinalforOLayerMemberName(entityMemberName);
      if (olayerMemberName == -1)
      {
        if (!(entityMemberName == "-EntityKey-"))
          throw new ArgumentException(Strings.ObjectStateEntry_ChangeOnUnmappedProperty((object) entityMemberName));
        if (!this._cache.InRelationshipFixup)
          throw new InvalidOperationException(Strings.ObjectStateEntry_CantSetEntityKey);
        this.SetCachedChangingValues((string) null, (object) null, (string) null, this.State, (object) null);
        return -2;
      }
      StateManagerTypeMetadata typeMetadata1;
      string memberName;
      object obj;
      if (complexObject != null)
      {
        if (!this._cacheTypeMetadata.Member(olayerMemberName).IsComplex)
          throw new ArgumentException(Strings.ComplexObject_ComplexChangeRequestedOnScalarProperty((object) entityMemberName));
        typeMetadata1 = this._cache.GetOrAddStateManagerTypeMetadata(complexObject.GetType(), (EntitySet) this.EntitySet);
        olayerMemberName = typeMetadata1.GetOrdinalforOLayerMemberName(complexObjectMemberName);
        if (olayerMemberName == -1)
          throw new ArgumentException(Strings.ObjectStateEntry_ChangeOnUnmappedComplexProperty((object) complexObjectMemberName));
        memberName = complexObjectMemberName;
        obj = complexObject;
      }
      else
      {
        typeMetadata1 = this._cacheTypeMetadata;
        memberName = entityMemberName;
        obj = this.Entity;
        if (this.WrappedEntity.IdentityType != this.Entity.GetType() && this.Entity is IEntityWithChangeTracker && this.IsPropertyAForeignKey(entityMemberName))
          this._cache.EntityInvokingFKSetter = this.WrappedEntity.Entity;
      }
      this.VerifyEntityValueIsEditable(typeMetadata1, olayerMemberName, memberName);
      typeMetadata = typeMetadata1;
      changingMemberName = memberName;
      changingObject = obj;
      return olayerMemberName;
    }

    private void SetCachedChangingValues(
      string entityMemberName,
      object changingObject,
      string changingMember,
      EntityState changingState,
      object oldValue)
    {
      this._cache.ChangingEntityMember = entityMemberName;
      this._cache.ChangingObject = changingObject;
      this._cache.ChangingMember = changingMember;
      this._cache.ChangingState = changingState;
      this._cache.ChangingOldValue = oldValue;
      if (changingState != EntityState.Detached)
        return;
      this._cache.SaveOriginalValues = false;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal OriginalValueRecord EditableOriginalValues
    {
      get
      {
        return (OriginalValueRecord) new ObjectStateEntryOriginalDbUpdatableDataRecord_Internal(this, this._cacheTypeMetadata, this._wrappedEntity.Entity);
      }
    }

    internal void DetachObjectStateManagerFromEntity()
    {
      if (this.IsKeyEntry)
        return;
      this._wrappedEntity.SetChangeTracker((IEntityChangeTracker) null);
      this._wrappedEntity.DetachContext();
      if (this._cache.TransactionManager.IsAttachTracking)
      {
        MergeOption? originalMergeOption = this._cache.TransactionManager.OriginalMergeOption;
        if ((originalMergeOption.GetValueOrDefault() != MergeOption.NoTracking ? 1 : (!originalMergeOption.HasValue ? 1 : 0)) == 0)
          return;
      }
      this._wrappedEntity.EntityKey = (EntityKey) null;
    }

    internal void TakeSnapshot(bool onlySnapshotComplexProperties)
    {
      if (this.State != EntityState.Added)
      {
        StateManagerTypeMetadata cacheTypeMetadata = this._cacheTypeMetadata;
        int fieldCount = this.GetFieldCount(cacheTypeMetadata);
        for (int ordinal = 0; ordinal < fieldCount; ++ordinal)
        {
          StateManagerMemberMetadata managerMemberMetadata = cacheTypeMetadata.Member(ordinal);
          if (managerMemberMetadata.IsComplex)
          {
            object obj = managerMemberMetadata.GetValue(this._wrappedEntity.Entity);
            this.AddComplexObjectSnapshot(this.Entity, ordinal, obj);
            this.TakeSnapshotOfComplexType(managerMemberMetadata, obj);
          }
          else if (!onlySnapshotComplexProperties)
          {
            object obj = managerMemberMetadata.GetValue(this._wrappedEntity.Entity);
            this.AddOriginalValueAt(-1, managerMemberMetadata, this._wrappedEntity.Entity, obj);
          }
        }
      }
      this.TakeSnapshotOfForeignKeys();
    }

    internal void TakeSnapshotOfForeignKeys()
    {
      Dictionary<RelatedEnd, HashSet<EntityKey>> relatedEntities;
      this.FindRelatedEntityKeysByForeignKeys(out relatedEntities, false);
      if (relatedEntities == null)
        return;
      foreach (KeyValuePair<RelatedEnd, HashSet<EntityKey>> keyValuePair in relatedEntities)
      {
        EntityReference key = keyValuePair.Key as EntityReference;
        if (!ForeignKeyFactory.IsConceptualNullKey(key.CachedForeignKey))
          key.SetCachedForeignKey(keyValuePair.Value.First<EntityKey>(), this);
      }
    }

    private void TakeSnapshotOfComplexType(StateManagerMemberMetadata member, object complexValue)
    {
      if (complexValue == null)
        return;
      StateManagerTypeMetadata managerTypeMetadata = this._cache.GetOrAddStateManagerTypeMetadata(member.CdmMetadata.TypeUsage.EdmType);
      for (int ordinal = 0; ordinal < managerTypeMetadata.FieldCount; ++ordinal)
      {
        StateManagerMemberMetadata managerMemberMetadata = managerTypeMetadata.Member(ordinal);
        object obj = managerMemberMetadata.GetValue(complexValue);
        if (managerMemberMetadata.IsComplex)
        {
          this.AddComplexObjectSnapshot(complexValue, ordinal, obj);
          this.TakeSnapshotOfComplexType(managerMemberMetadata, obj);
        }
        else if (this.FindOriginalValueIndex(managerMemberMetadata, complexValue) == -1)
          this.AddOriginalValueAt(-1, managerMemberMetadata, complexValue, obj);
      }
    }

    private void AddComplexObjectSnapshot(object userObject, int ordinal, object complexObject)
    {
      if (complexObject == null)
        return;
      this.CheckForDuplicateComplexObjects(complexObject);
      if (this._originalComplexObjects == null)
        this._originalComplexObjects = new Dictionary<object, Dictionary<int, object>>((IEqualityComparer<object>) ObjectReferenceEqualityComparer.Default);
      Dictionary<int, object> dictionary;
      if (!this._originalComplexObjects.TryGetValue(userObject, out dictionary))
      {
        dictionary = new Dictionary<int, object>();
        this._originalComplexObjects.Add(userObject, dictionary);
      }
      dictionary.Add(ordinal, complexObject);
    }

    private void CheckForDuplicateComplexObjects(object complexObject)
    {
      if (this._originalComplexObjects == null || complexObject == null)
        return;
      foreach (Dictionary<int, object> dictionary in this._originalComplexObjects.Values)
      {
        foreach (object objB in dictionary.Values)
        {
          if (object.ReferenceEquals(complexObject, objB))
            throw new InvalidOperationException(Strings.ObjectStateEntry_ComplexObjectUsedMultipleTimes((object) this.Entity.GetType().FullName, (object) complexObject.GetType().FullName));
        }
      }
    }

    public override bool IsPropertyChanged(string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      return this.DetectChangesInProperty(this.ValidateAndGetOrdinalForProperty(propertyName, nameof (IsPropertyChanged)), false, true);
    }

    [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Used in the debug build", MessageId = "originalValueFound")]
    private bool DetectChangesInProperty(
      int ordinal,
      bool detectOnlyComplexProperties,
      bool detectOnly)
    {
      bool changeDetected = false;
      StateManagerMemberMetadata managerMemberMetadata = this._cacheTypeMetadata.Member(ordinal);
      object obj = managerMemberMetadata.GetValue(this._wrappedEntity.Entity);
      if (managerMemberMetadata.IsComplex)
      {
        if (this.State != EntityState.Deleted)
        {
          object complexObjectSnapshot = this.GetComplexObjectSnapshot(this.Entity, ordinal);
          if (this.DetectChangesInComplexType(managerMemberMetadata, managerMemberMetadata, obj, complexObjectSnapshot, ref changeDetected, detectOnly))
          {
            this.CheckForDuplicateComplexObjects(obj);
            if (!detectOnly)
            {
              ((IEntityChangeTracker) this).EntityMemberChanging(managerMemberMetadata.CLayerName);
              this._cache.ChangingOldValue = complexObjectSnapshot;
              ((IEntityChangeTracker) this).EntityMemberChanged(managerMemberMetadata.CLayerName);
            }
            this.UpdateComplexObjectSnapshot(managerMemberMetadata, this.Entity, ordinal, obj);
            if (!changeDetected)
              this.DetectChangesInComplexType(managerMemberMetadata, managerMemberMetadata, obj, complexObjectSnapshot, ref changeDetected, detectOnly);
          }
        }
      }
      else if (!detectOnlyComplexProperties)
      {
        int originalValueIndex = this.FindOriginalValueIndex(managerMemberMetadata, this._wrappedEntity.Entity);
        if (originalValueIndex < 0)
          return this.GetModifiedProperties().Contains<string>(managerMemberMetadata.CLayerName);
        object originalValue = this._originalValues[originalValueIndex].OriginalValue;
        if (!object.Equals(obj, originalValue))
        {
          changeDetected = true;
          if (managerMemberMetadata.IsPartOfKey)
          {
            if (!ByValueEqualityComparer.Default.Equals(obj, originalValue))
              throw new InvalidOperationException(Strings.ObjectStateEntry_CannotModifyKeyProperty((object) managerMemberMetadata.CLayerName));
          }
          else if (this.State != EntityState.Deleted && !detectOnly)
          {
            ((IEntityChangeTracker) this).EntityMemberChanging(managerMemberMetadata.CLayerName);
            ((IEntityChangeTracker) this).EntityMemberChanged(managerMemberMetadata.CLayerName);
          }
        }
      }
      return changeDetected;
    }

    internal void DetectChangesInProperties(bool detectOnlyComplexProperties)
    {
      int fieldCount = this.GetFieldCount(this._cacheTypeMetadata);
      for (int ordinal = 0; ordinal < fieldCount; ++ordinal)
        this.DetectChangesInProperty(ordinal, detectOnlyComplexProperties, false);
    }

    private bool DetectChangesInComplexType(
      StateManagerMemberMetadata topLevelMember,
      StateManagerMemberMetadata complexMember,
      object complexValue,
      object oldComplexValue,
      ref bool changeDetected,
      bool detectOnly)
    {
      if (complexValue == null)
      {
        if (oldComplexValue == null)
          return false;
        throw new InvalidOperationException(Strings.ComplexObject_NullableComplexTypesNotSupported((object) complexMember.CLayerName));
      }
      if (!object.ReferenceEquals(oldComplexValue, complexValue))
        return true;
      StateManagerTypeMetadata managerTypeMetadata = this._cache.GetOrAddStateManagerTypeMetadata(complexMember.CdmMetadata.TypeUsage.EdmType);
      for (int index = 0; index < this.GetFieldCount(managerTypeMetadata); ++index)
      {
        StateManagerMemberMetadata managerMemberMetadata = managerTypeMetadata.Member(index);
        object obj = managerMemberMetadata.GetValue(complexValue);
        if (managerMemberMetadata.IsComplex)
        {
          if (this.State != EntityState.Deleted)
          {
            object complexObjectSnapshot = this.GetComplexObjectSnapshot(complexValue, index);
            if (this.DetectChangesInComplexType(topLevelMember, managerMemberMetadata, obj, complexObjectSnapshot, ref changeDetected, detectOnly))
            {
              this.CheckForDuplicateComplexObjects(obj);
              if (!detectOnly)
              {
                ((IEntityChangeTracker) this).EntityComplexMemberChanging(topLevelMember.CLayerName, complexValue, managerMemberMetadata.CLayerName);
                this._cache.ChangingOldValue = complexObjectSnapshot;
                ((IEntityChangeTracker) this).EntityComplexMemberChanged(topLevelMember.CLayerName, complexValue, managerMemberMetadata.CLayerName);
              }
              this.UpdateComplexObjectSnapshot(managerMemberMetadata, complexValue, index, obj);
              if (!changeDetected)
                this.DetectChangesInComplexType(topLevelMember, managerMemberMetadata, obj, complexObjectSnapshot, ref changeDetected, detectOnly);
            }
          }
        }
        else
        {
          int originalValueIndex = this.FindOriginalValueIndex(managerMemberMetadata, complexValue);
          object objB = originalValueIndex == -1 ? (object) null : this._originalValues[originalValueIndex].OriginalValue;
          if (!object.Equals(obj, objB))
          {
            changeDetected = true;
            if (!detectOnly)
            {
              ((IEntityChangeTracker) this).EntityComplexMemberChanging(topLevelMember.CLayerName, complexValue, managerMemberMetadata.CLayerName);
              ((IEntityChangeTracker) this).EntityComplexMemberChanged(topLevelMember.CLayerName, complexValue, managerMemberMetadata.CLayerName);
            }
          }
        }
      }
      return false;
    }

    private object GetComplexObjectSnapshot(object parentObject, int parentOrdinal)
    {
      object obj = (object) null;
      Dictionary<int, object> dictionary;
      if (this._originalComplexObjects != null && this._originalComplexObjects.TryGetValue(parentObject, out dictionary))
        dictionary.TryGetValue(parentOrdinal, out obj);
      return obj;
    }

    internal void UpdateComplexObjectSnapshot(
      StateManagerMemberMetadata member,
      object userObject,
      int ordinal,
      object currentValue)
    {
      bool flag = true;
      Dictionary<int, object> dictionary;
      if (this._originalComplexObjects != null && this._originalComplexObjects.TryGetValue(userObject, out dictionary))
      {
        object key;
        dictionary.TryGetValue(ordinal, out key);
        dictionary[ordinal] = currentValue;
        if (key != null && this._originalComplexObjects.TryGetValue(key, out dictionary))
        {
          this._originalComplexObjects.Remove(key);
          this._originalComplexObjects.Add(currentValue, dictionary);
          StateManagerTypeMetadata managerTypeMetadata = this._cache.GetOrAddStateManagerTypeMetadata(member.CdmMetadata.TypeUsage.EdmType);
          for (int ordinal1 = 0; ordinal1 < managerTypeMetadata.FieldCount; ++ordinal1)
          {
            StateManagerMemberMetadata member1 = managerTypeMetadata.Member(ordinal1);
            if (member1.IsComplex)
            {
              object currentValue1 = member1.GetValue(currentValue);
              this.UpdateComplexObjectSnapshot(member1, currentValue, ordinal1, currentValue1);
            }
          }
        }
        flag = false;
      }
      if (!flag)
        return;
      this.AddComplexObjectSnapshot(userObject, ordinal, currentValue);
    }

    internal void FixupFKValuesFromNonAddedReferences()
    {
      if (!((EntitySet) this.EntitySet).HasForeignKeyRelationships)
        return;
      Dictionary<int, object> changedFKs = new Dictionary<int, object>();
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyDependent in this.ForeignKeyDependents)
      {
        EntityReference relatedEndInternal = this.RelationshipManager.GetRelatedEndInternal(foreignKeyDependent.Item1.ElementType.FullName, foreignKeyDependent.Item2.FromRole.Name) as EntityReference;
        if (relatedEndInternal.TargetAccessor.HasProperty)
        {
          object navigationPropertyValue = this.WrappedEntity.GetNavigationPropertyValue((RelatedEnd) relatedEndInternal);
          ObjectStateEntry entry;
          if (navigationPropertyValue != null && this._cache.TryGetObjectStateEntry(navigationPropertyValue, out entry) && (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged))
            relatedEndInternal.UpdateForeignKeyValues(this.WrappedEntity, ((EntityEntry) entry).WrappedEntity, changedFKs, false);
        }
      }
    }

    internal void TakeSnapshotOfRelationships()
    {
      RelationshipManager relationshipManager = this._wrappedEntity.RelationshipManager;
      foreach (NavigationProperty navigationProperty in (this._cacheTypeMetadata.CdmMetadata.EdmType as EntityType).NavigationProperties)
      {
        RelatedEnd relatedEndInternal = relationshipManager.GetRelatedEndInternal(navigationProperty.RelationshipType.FullName, navigationProperty.ToEndMember.Name);
        object navigationPropertyValue = this.WrappedEntity.GetNavigationPropertyValue(relatedEndInternal);
        if (navigationPropertyValue != null)
        {
          if (navigationProperty.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
          {
            IEnumerable enumerable = navigationPropertyValue as IEnumerable;
            if (enumerable == null)
              throw new EntityException(Strings.ObjectStateEntry_UnableToEnumerateCollection((object) navigationProperty.Name, (object) this.Entity.GetType().FullName));
            foreach (object o in enumerable)
            {
              if (o != null)
                this.TakeSnapshotOfSingleRelationship(relatedEndInternal, navigationProperty, o);
            }
          }
          else
            this.TakeSnapshotOfSingleRelationship(relatedEndInternal, navigationProperty, navigationPropertyValue);
        }
      }
    }

    private void TakeSnapshotOfSingleRelationship(
      RelatedEnd relatedEnd,
      NavigationProperty n,
      object o)
    {
      EntityEntry entityEntry1 = this.ObjectStateManager.FindEntityEntry(o);
      IEntityWrapper wrappedEntity;
      if (entityEntry1 != null)
      {
        wrappedEntity = entityEntry1._wrappedEntity;
        RelatedEnd relatedEndInternal = wrappedEntity.RelationshipManager.GetRelatedEndInternal(n.RelationshipType.FullName, n.FromEndMember.Name);
        if (!relatedEndInternal.ContainsEntity(this._wrappedEntity))
        {
          if (wrappedEntity.ObjectStateEntry.State == EntityState.Deleted)
            throw Error.RelatedEnd_UnableToAddRelationshipWithDeletedEntity();
          if (this.ObjectStateManager.TransactionManager.IsAttachTracking && (this.State & (EntityState.Unchanged | EntityState.Modified)) != (EntityState) 0 && (wrappedEntity.ObjectStateEntry.State & (EntityState.Unchanged | EntityState.Modified)) != (EntityState) 0)
          {
            EntityEntry entityEntry2 = (EntityEntry) null;
            EntityEntry entityEntry3 = (EntityEntry) null;
            if (relatedEnd.IsDependentEndOfReferentialConstraint(false))
            {
              entityEntry2 = wrappedEntity.ObjectStateEntry;
              entityEntry3 = this;
            }
            else if (relatedEndInternal.IsDependentEndOfReferentialConstraint(false))
            {
              entityEntry2 = this;
              entityEntry3 = wrappedEntity.ObjectStateEntry;
            }
            if (entityEntry2 != null)
            {
              ReferentialConstraint referentialConstraint = ((AssociationType) relatedEnd.RelationMetadata).ReferentialConstraints[0];
              if (!RelatedEnd.VerifyRIConstraintsWithRelatedEntry(referentialConstraint, new Func<string, object>(entityEntry3.GetCurrentEntityValue), entityEntry2.EntityKey))
                throw new InvalidOperationException(referentialConstraint.BuildConstraintExceptionMessage());
            }
          }
          EntityReference entityReference = relatedEndInternal as EntityReference;
          if (entityReference != null && entityReference.NavigationPropertyIsNullOrMissing())
            this.ObjectStateManager.TransactionManager.AlignedEntityReferences.Add(entityReference);
          relatedEndInternal.AddToLocalCache(this._wrappedEntity, true);
          relatedEndInternal.OnAssociationChanged(CollectionChangeAction.Add, this._wrappedEntity.Entity);
        }
      }
      else if (!this.ObjectStateManager.TransactionManager.WrappedEntities.TryGetValue(o, out wrappedEntity))
        wrappedEntity = this.ObjectStateManager.EntityWrapperFactory.WrapEntityUsingStateManager(o, this.ObjectStateManager);
      if (relatedEnd.ContainsEntity(wrappedEntity))
        return;
      relatedEnd.AddToLocalCache(wrappedEntity, true);
      relatedEnd.OnAssociationChanged(CollectionChangeAction.Add, wrappedEntity.Entity);
    }

    internal void DetectChangesInRelationshipsOfSingleEntity()
    {
      foreach (NavigationProperty navigationProperty in (this._cacheTypeMetadata.CdmMetadata.EdmType as EntityType).NavigationProperties)
      {
        RelatedEnd relatedEndInternal = this.WrappedEntity.RelationshipManager.GetRelatedEndInternal(navigationProperty.RelationshipType.FullName, navigationProperty.ToEndMember.Name);
        object navigationPropertyValue = this.WrappedEntity.GetNavigationPropertyValue(relatedEndInternal);
        HashSet<object> objectSet = new HashSet<object>((IEqualityComparer<object>) ObjectReferenceEqualityComparer.Default);
        if (navigationPropertyValue != null)
        {
          if (navigationProperty.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
          {
            IEnumerable enumerable = navigationPropertyValue as IEnumerable;
            if (enumerable == null)
              throw new EntityException(Strings.ObjectStateEntry_UnableToEnumerateCollection((object) navigationProperty.Name, (object) this.Entity.GetType().FullName));
            foreach (object obj in enumerable)
            {
              if (obj != null)
                objectSet.Add(obj);
            }
          }
          else
            objectSet.Add(navigationPropertyValue);
        }
        foreach (object relatedObject in relatedEndInternal.GetInternalEnumerable())
        {
          if (!objectSet.Contains(relatedObject))
            this.AddRelationshipDetectedByGraph(this.ObjectStateManager.TransactionManager.DeletedRelationshipsByGraph, relatedObject, relatedEndInternal, false);
          else
            objectSet.Remove(relatedObject);
        }
        foreach (object relatedObject in objectSet)
          this.AddRelationshipDetectedByGraph(this.ObjectStateManager.TransactionManager.AddedRelationshipsByGraph, relatedObject, relatedEndInternal, true);
      }
    }

    private void AddRelationshipDetectedByGraph(
      Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<IEntityWrapper>>> relationships,
      object relatedObject,
      RelatedEnd relatedEndFrom,
      bool verifyForAdd)
    {
      IEntityWrapper entityWrapper = this.ObjectStateManager.EntityWrapperFactory.WrapEntityUsingStateManager(relatedObject, this.ObjectStateManager);
      EntityEntry.AddDetectedRelationship<IEntityWrapper>(relationships, entityWrapper, relatedEndFrom);
      RelatedEnd endOfRelationship = relatedEndFrom.GetOtherEndOfRelationship(entityWrapper);
      if (verifyForAdd && endOfRelationship is EntityReference && this.ObjectStateManager.FindEntityEntry(relatedObject) == null)
        endOfRelationship.VerifyNavigationPropertyForAdd(this._wrappedEntity);
      EntityEntry.AddDetectedRelationship<IEntityWrapper>(relationships, this._wrappedEntity, endOfRelationship);
    }

    private void AddRelationshipDetectedByForeignKey(
      Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>> relationships,
      Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>> principalRelationships,
      EntityKey relatedKey,
      EntityEntry relatedEntry,
      RelatedEnd relatedEndFrom)
    {
      EntityEntry.AddDetectedRelationship<EntityKey>(relationships, relatedKey, relatedEndFrom);
      if (relatedEntry == null)
        return;
      IEntityWrapper wrappedEntity = relatedEntry.WrappedEntity;
      RelatedEnd endOfRelationship = relatedEndFrom.GetOtherEndOfRelationship(wrappedEntity);
      EntityKey permanentKey = this.ObjectStateManager.GetPermanentKey(relatedEntry.WrappedEntity, endOfRelationship, this.WrappedEntity);
      EntityEntry.AddDetectedRelationship<EntityKey>(principalRelationships, permanentKey, endOfRelationship);
    }

    private static void AddDetectedRelationship<T>(
      Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<T>>> relationships,
      T relatedObject,
      RelatedEnd relatedEnd)
    {
      Dictionary<RelatedEnd, HashSet<T>> dictionary;
      if (!relationships.TryGetValue(relatedEnd.WrappedOwner, out dictionary))
      {
        dictionary = new Dictionary<RelatedEnd, HashSet<T>>();
        relationships.Add(relatedEnd.WrappedOwner, dictionary);
      }
      HashSet<T> source;
      if (!dictionary.TryGetValue(relatedEnd, out source))
      {
        source = new HashSet<T>();
        dictionary.Add(relatedEnd, source);
      }
      else if (relatedEnd is EntityReference && !object.Equals((object) source.First<T>(), (object) relatedObject))
        throw new InvalidOperationException(Strings.EntityReference_CannotAddMoreThanOneEntityToEntityReference((object) relatedEnd.RelationshipNavigation.To, (object) relatedEnd.RelationshipNavigation.RelationshipName));
      source.Add(relatedObject);
    }

    internal void Detach()
    {
      this.ValidateState();
      RelationshipManager relationshipManager = this._wrappedEntity.RelationshipManager;
      bool flag = this.State != EntityState.Added && this.IsOneEndOfSomeRelationship();
      this._cache.TransactionManager.BeginDetaching();
      try
      {
        relationshipManager.DetachEntityFromRelationships(this.State);
      }
      finally
      {
        this._cache.TransactionManager.EndDetaching();
      }
      this.DetachRelationshipsEntries(relationshipManager);
      IEntityWrapper wrappedEntity = this._wrappedEntity;
      EntityKey entityKey = this._entityKey;
      EntityState state = this.State;
      if (flag)
      {
        this.DegradeEntry();
      }
      else
      {
        this._wrappedEntity.ObjectStateEntry = (EntityEntry) null;
        this._cache.ChangeState(this, this.State, EntityState.Detached);
      }
      if (state == EntityState.Added)
        return;
      wrappedEntity.EntityKey = entityKey;
    }

    internal void Delete(bool doFixup)
    {
      this.ValidateState();
      if (this.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotDeleteOnKeyEntry);
      if (doFixup && this.State != EntityState.Deleted)
      {
        this.RelationshipManager.NullAllFKsInDependentsForWhichThisIsThePrincipal();
        this.NullAllForeignKeys();
        this.FixupRelationships();
      }
      switch (this.State)
      {
        case EntityState.Unchanged:
          if (!doFixup)
            this.DeleteRelationshipsThatReferenceKeys((RelationshipSet) null, (RelationshipEndMember) null);
          this._cache.ChangeState(this, EntityState.Unchanged, EntityState.Deleted);
          this.State = EntityState.Deleted;
          break;
        case EntityState.Added:
          this._cache.ChangeState(this, EntityState.Added, EntityState.Detached);
          break;
        case EntityState.Modified:
          if (!doFixup)
            this.DeleteRelationshipsThatReferenceKeys((RelationshipSet) null, (RelationshipEndMember) null);
          this._cache.ChangeState(this, EntityState.Modified, EntityState.Deleted);
          this.State = EntityState.Deleted;
          break;
      }
    }

    private void NullAllForeignKeys()
    {
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyDependent in this.ForeignKeyDependents)
        (this.WrappedEntity.RelationshipManager.GetRelatedEndInternal(foreignKeyDependent.Item1.ElementType.FullName, foreignKeyDependent.Item2.FromRole.Name) as EntityReference).NullAllForeignKeys();
    }

    private bool IsOneEndOfSomeRelationship()
    {
      foreach (RelationshipEntry relationshipEntry in this._cache.FindRelationshipsByKey(this.EntityKey))
      {
        switch (this.GetAssociationEndMember(relationshipEntry).RelationshipMultiplicity)
        {
          case RelationshipMultiplicity.ZeroOrOne:
          case RelationshipMultiplicity.One:
            if (!this._cache.GetEntityEntry(relationshipEntry.RelationshipWrapper.GetOtherEntityKey(this.EntityKey)).IsKeyEntry)
              return true;
            continue;
          default:
            continue;
        }
      }
      return false;
    }

    private void DetachRelationshipsEntries(RelationshipManager relationshipManager)
    {
      foreach (RelationshipEntry relationshipEntry in this._cache.CopyOfRelationshipsByKey(this.EntityKey))
      {
        EntityKey otherEntityKey = relationshipEntry.RelationshipWrapper.GetOtherEntityKey(this.EntityKey);
        if (this._cache.GetEntityEntry(otherEntityKey).IsKeyEntry)
        {
          if (relationshipEntry.State != EntityState.Deleted)
          {
            AssociationEndMember associationEndMember = relationshipEntry.RelationshipWrapper.GetAssociationEndMember(otherEntityKey);
            ((EntityReference) relationshipManager.GetRelatedEndInternal(associationEndMember.DeclaringType.FullName, associationEndMember.Name)).DetachedEntityKey = otherEntityKey;
          }
          relationshipEntry.DeleteUnnecessaryKeyEntries();
          relationshipEntry.DetachRelationshipEntry();
        }
        else if (relationshipEntry.State == EntityState.Deleted && this.GetAssociationEndMember(relationshipEntry).RelationshipMultiplicity == RelationshipMultiplicity.Many)
          relationshipEntry.DetachRelationshipEntry();
      }
    }

    private void FixupRelationships()
    {
      this._wrappedEntity.RelationshipManager.RemoveEntityFromRelationships();
      this.DeleteRelationshipsThatReferenceKeys((RelationshipSet) null, (RelationshipEndMember) null);
    }

    internal void DeleteRelationshipsThatReferenceKeys(
      RelationshipSet relationshipSet,
      RelationshipEndMember endMember)
    {
      if (this.State == EntityState.Detached)
        return;
      foreach (RelationshipEntry relationshipEntry in this._cache.CopyOfRelationshipsByKey(this.EntityKey))
      {
        if (relationshipEntry.State != EntityState.Deleted && (relationshipSet == null || relationshipSet == relationshipEntry.EntitySet))
        {
          EntityEntry endOfRelationship = this.GetOtherEndOfRelationship(relationshipEntry);
          if (endMember == null || endMember == endOfRelationship.GetAssociationEndMember(relationshipEntry))
          {
            for (int ordinal = 0; ordinal < 2; ++ordinal)
            {
              EntityKey currentRelationValue = relationshipEntry.GetCurrentRelationValue(ordinal) as EntityKey;
              if ((object) currentRelationValue != null && this._cache.GetEntityEntry(currentRelationValue).IsKeyEntry)
              {
                relationshipEntry.Delete(false);
                break;
              }
            }
          }
        }
      }
    }

    private bool RetrieveAndCheckReferentialConstraintValuesInAcceptChanges()
    {
      RelationshipManager relationshipManager = this._wrappedEntity.RelationshipManager;
      List<string> propertiesToRetrieve;
      bool propertiesToPropagateExist;
      bool constraintProperties = relationshipManager.FindNamesOfReferentialConstraintProperties(out propertiesToRetrieve, out propertiesToPropagateExist, true);
      if (propertiesToRetrieve != null)
      {
        HashSet<object> visited = new HashSet<object>();
        Dictionary<string, KeyValuePair<object, IntBox>> properties;
        relationshipManager.RetrieveReferentialConstraintProperties(out properties, visited, false);
        foreach (KeyValuePair<string, KeyValuePair<object, IntBox>> keyValuePair in properties)
          this.SetCurrentEntityValue(keyValuePair.Key, keyValuePair.Value.Key);
      }
      if (propertiesToPropagateExist)
        this.CheckReferentialConstraintPropertiesInDependents();
      return constraintProperties;
    }

    internal void RetrieveReferentialConstraintPropertiesFromKeyEntries(
      Dictionary<string, KeyValuePair<object, IntBox>> properties)
    {
      foreach (RelationshipEntry relationshipEntry in this._cache.FindRelationshipsByKey(this.EntityKey))
      {
        EntityEntry endOfRelationship = this.GetOtherEndOfRelationship(relationshipEntry);
        if (endOfRelationship.IsKeyEntry)
        {
          foreach (ReferentialConstraint referentialConstraint in ((AssociationSet) relationshipEntry.EntitySet).ElementType.ReferentialConstraints)
          {
            string name = this.GetAssociationEndMember(relationshipEntry).Name;
            if (referentialConstraint.ToRole.Name == name)
            {
              foreach (EntityKeyMember entityKeyValue in (IEnumerable<EntityKeyMember>) endOfRelationship.EntityKey.EntityKeyValues)
              {
                for (int index = 0; index < referentialConstraint.FromProperties.Count; ++index)
                {
                  if (referentialConstraint.FromProperties[index].Name == entityKeyValue.Key)
                    EntityEntry.AddOrIncreaseCounter(referentialConstraint, properties, referentialConstraint.ToProperties[index].Name, entityKeyValue.Value);
                }
              }
            }
          }
        }
      }
    }

    internal static void AddOrIncreaseCounter(
      ReferentialConstraint constraint,
      Dictionary<string, KeyValuePair<object, IntBox>> properties,
      string propertyName,
      object propertyValue)
    {
      if (properties.ContainsKey(propertyName))
      {
        KeyValuePair<object, IntBox> property = properties[propertyName];
        if (!ByValueEqualityComparer.Default.Equals(property.Key, propertyValue))
          throw new InvalidOperationException(constraint.BuildConstraintExceptionMessage());
        ++property.Value.Value;
      }
      else
        properties[propertyName] = new KeyValuePair<object, IntBox>(propertyValue, new IntBox(1));
    }

    private void CheckReferentialConstraintPropertiesInDependents()
    {
      foreach (RelationshipEntry relationshipEntry in this._cache.FindRelationshipsByKey(this.EntityKey))
      {
        EntityEntry endOfRelationship = this.GetOtherEndOfRelationship(relationshipEntry);
        if (endOfRelationship.State == EntityState.Unchanged || endOfRelationship.State == EntityState.Modified)
        {
          foreach (ReferentialConstraint referentialConstraint in ((AssociationSet) relationshipEntry.EntitySet).ElementType.ReferentialConstraints)
          {
            string name = this.GetAssociationEndMember(relationshipEntry).Name;
            if (referentialConstraint.FromRole.Name == name)
            {
              foreach (EntityKeyMember entityKeyValue in (IEnumerable<EntityKeyMember>) endOfRelationship.EntityKey.EntityKeyValues)
              {
                for (int index = 0; index < referentialConstraint.FromProperties.Count; ++index)
                {
                  if (referentialConstraint.ToProperties[index].Name == entityKeyValue.Key && !ByValueEqualityComparer.Default.Equals(this.GetCurrentEntityValue(referentialConstraint.FromProperties[index].Name), entityKeyValue.Value))
                    throw new InvalidOperationException(referentialConstraint.BuildConstraintExceptionMessage());
                }
              }
            }
          }
        }
      }
    }

    internal void PromoteKeyEntry(
      IEntityWrapper wrappedEntity,
      StateManagerTypeMetadata typeMetadata)
    {
      this._wrappedEntity = wrappedEntity;
      this._wrappedEntity.ObjectStateEntry = this;
      this._cacheTypeMetadata = typeMetadata;
      this.SetChangeTrackingFlags();
    }

    internal void DegradeEntry()
    {
      this._entityKey = this.EntityKey;
      this.RemoveFromForeignKeyIndex();
      this._wrappedEntity.SetChangeTracker((IEntityChangeTracker) null);
      this._modifiedFields = (BitArray) null;
      this._originalValues = (List<StateManagerValue>) null;
      this._originalComplexObjects = (Dictionary<object, Dictionary<int, object>>) null;
      if (this.State == EntityState.Added)
      {
        this._wrappedEntity.EntityKey = (EntityKey) null;
        this._entityKey = (EntityKey) null;
      }
      if (this.State != EntityState.Unchanged)
      {
        this._cache.ChangeState(this, this.State, EntityState.Unchanged);
        this.State = EntityState.Unchanged;
      }
      this._cache.RemoveEntryFromKeylessStore(this._wrappedEntity);
      this._wrappedEntity.DetachContext();
      this._wrappedEntity.ObjectStateEntry = (EntityEntry) null;
      object entity = this._wrappedEntity.Entity;
      this._wrappedEntity = NullEntityWrapper.NullWrapper;
      this.SetChangeTrackingFlags();
      this._cache.OnObjectStateManagerChanged(CollectionChangeAction.Remove, entity);
    }

    internal void AttachObjectStateManagerToEntity()
    {
      this._wrappedEntity.SetChangeTracker((IEntityChangeTracker) this);
      this._wrappedEntity.TakeSnapshot(this);
    }

    internal void GetOtherKeyProperties(
      Dictionary<string, KeyValuePair<object, IntBox>> properties)
    {
      foreach (EdmMember keyMember in (this._cacheTypeMetadata.DataRecordInfo.RecordType.EdmType as EntityType).KeyMembers)
      {
        if (!properties.ContainsKey(keyMember.Name))
          properties[keyMember.Name] = new KeyValuePair<object, IntBox>(this.GetCurrentEntityValue(keyMember.Name), new IntBox(1));
      }
    }

    internal void AddOriginalValueAt(
      int index,
      StateManagerMemberMetadata memberMetadata,
      object userObject,
      object value)
    {
      StateManagerValue stateManagerValue = new StateManagerValue(memberMetadata, userObject, value);
      if (index >= 0)
      {
        this._originalValues[index] = stateManagerValue;
      }
      else
      {
        if (this._originalValues == null)
          this._originalValues = new List<StateManagerValue>();
        this._originalValues.Add(stateManagerValue);
      }
    }

    internal void CompareKeyProperties(object changed)
    {
      StateManagerTypeMetadata cacheTypeMetadata = this._cacheTypeMetadata;
      int fieldCount = this.GetFieldCount(cacheTypeMetadata);
      for (int ordinal = 0; ordinal < fieldCount; ++ordinal)
      {
        StateManagerMemberMetadata managerMemberMetadata = cacheTypeMetadata.Member(ordinal);
        if (managerMemberMetadata.IsPartOfKey)
        {
          object x = managerMemberMetadata.GetValue(changed);
          object y = managerMemberMetadata.GetValue(this._wrappedEntity.Entity);
          if (!ByValueEqualityComparer.Default.Equals(x, y))
            throw new InvalidOperationException(Strings.ObjectStateEntry_CannotModifyKeyProperty((object) managerMemberMetadata.CLayerName));
        }
      }
    }

    internal object GetCurrentEntityValue(string memberName)
    {
      return this.GetCurrentEntityValue(this._cacheTypeMetadata, this._cacheTypeMetadata.GetOrdinalforOLayerMemberName(memberName), this._wrappedEntity.Entity, ObjectStateValueRecord.CurrentUpdatable);
    }

    internal void VerifyEntityValueIsEditable(
      StateManagerTypeMetadata typeMetadata,
      int ordinal,
      string memberName)
    {
      if (this.State == EntityState.Deleted)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CantModifyDetachedDeletedEntries);
      if (typeMetadata.Member(ordinal).IsPartOfKey && this.State != EntityState.Added)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CannotModifyKeyProperty((object) memberName));
    }

    internal void SetCurrentEntityValue(
      StateManagerTypeMetadata metadata,
      int ordinal,
      object userObject,
      object newValue)
    {
      this.ValidateState();
      StateManagerMemberMetadata member = metadata.Member(ordinal);
      if (member.IsComplex)
      {
        if (newValue == null || newValue == DBNull.Value)
          throw new InvalidOperationException(Strings.ComplexObject_NullableComplexTypesNotSupported((object) member.CLayerName));
        IExtendedDataRecord record = newValue as IExtendedDataRecord;
        if (record == null)
          throw new ArgumentException(Strings.ObjectStateEntry_InvalidTypeForComplexTypeProperty, nameof (newValue));
        newValue = this._cache.ComplexTypeMaterializer.CreateComplex(record, record.DataRecordInfo, (object) null);
      }
      this._wrappedEntity.SetCurrentValue(this, member, ordinal, userObject, newValue);
    }

    private void TransitionRelationshipsForAdd()
    {
      foreach (RelationshipEntry entry in this._cache.CopyOfRelationshipsByKey(this.EntityKey))
      {
        if (entry.State == EntityState.Unchanged)
        {
          this.ObjectStateManager.ChangeState(entry, EntityState.Unchanged, EntityState.Added);
          entry.State = EntityState.Added;
        }
        else if (entry.State == EntityState.Deleted)
        {
          entry.DeleteUnnecessaryKeyEntries();
          entry.DetachRelationshipEntry();
        }
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [Conditional("DEBUG")]
    private void VerifyIsNotRelated()
    {
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal void ChangeObjectState(EntityState requestedState)
    {
      if (this.IsKeyEntry)
      {
        if (requestedState != EntityState.Unchanged)
          throw new InvalidOperationException(Strings.ObjectStateEntry_CannotModifyKeyEntryState);
      }
      else
      {
        switch (this.State)
        {
          case EntityState.Unchanged:
            switch (requestedState)
            {
              case EntityState.Detached:
                this.Detach();
                return;
              case EntityState.Unchanged:
                return;
              case EntityState.Added:
                this.ObjectStateManager.ReplaceKeyWithTemporaryKey(this);
                this._modifiedFields = (BitArray) null;
                this._originalValues = (List<StateManagerValue>) null;
                this._originalComplexObjects = (Dictionary<object, Dictionary<int, object>>) null;
                this.State = EntityState.Added;
                this.TransitionRelationshipsForAdd();
                return;
              case EntityState.Deleted:
                this.Delete(true);
                return;
              case EntityState.Modified:
                this.SetModified();
                this.SetModifiedAll();
                return;
              default:
                throw new ArgumentException(Strings.ObjectContext_InvalidEntityState, nameof (requestedState));
            }
          case EntityState.Added:
            switch (requestedState)
            {
              case EntityState.Detached:
                this.Detach();
                return;
              case EntityState.Unchanged:
                this.AcceptChanges();
                return;
              case EntityState.Added:
                this.TransitionRelationshipsForAdd();
                return;
              case EntityState.Deleted:
                this._cache.ForgetEntryWithConceptualNull(this, true);
                this.AcceptChanges();
                this.Delete(true);
                return;
              case EntityState.Modified:
                this.AcceptChanges();
                this.SetModified();
                this.SetModifiedAll();
                return;
              default:
                throw new ArgumentException(Strings.ObjectContext_InvalidEntityState, nameof (requestedState));
            }
          case EntityState.Deleted:
            switch (requestedState)
            {
              case EntityState.Detached:
                this.Detach();
                return;
              case EntityState.Unchanged:
                this._modifiedFields = (BitArray) null;
                this._originalValues = (List<StateManagerValue>) null;
                this._originalComplexObjects = (Dictionary<object, Dictionary<int, object>>) null;
                this.ObjectStateManager.ChangeState(this, EntityState.Deleted, EntityState.Unchanged);
                this.State = EntityState.Unchanged;
                this._wrappedEntity.TakeSnapshot(this);
                this._cache.FixupReferencesByForeignKeys(this, false);
                this._cache.OnObjectStateManagerChanged(CollectionChangeAction.Add, this.Entity);
                return;
              case EntityState.Added:
                this.TransitionRelationshipsForAdd();
                this.ObjectStateManager.ReplaceKeyWithTemporaryKey(this);
                this._modifiedFields = (BitArray) null;
                this._originalValues = (List<StateManagerValue>) null;
                this._originalComplexObjects = (Dictionary<object, Dictionary<int, object>>) null;
                this.State = EntityState.Added;
                this._cache.FixupReferencesByForeignKeys(this, false);
                this._cache.OnObjectStateManagerChanged(CollectionChangeAction.Add, this.Entity);
                return;
              case EntityState.Deleted:
                return;
              case EntityState.Modified:
                this.ObjectStateManager.ChangeState(this, EntityState.Deleted, EntityState.Modified);
                this.State = EntityState.Modified;
                this.SetModifiedAll();
                this._cache.FixupReferencesByForeignKeys(this, false);
                this._cache.OnObjectStateManagerChanged(CollectionChangeAction.Add, this.Entity);
                return;
              default:
                throw new ArgumentException(Strings.ObjectContext_InvalidEntityState, nameof (requestedState));
            }
          case EntityState.Modified:
            switch (requestedState)
            {
              case EntityState.Detached:
                this.Detach();
                return;
              case EntityState.Unchanged:
                this.AcceptChanges();
                return;
              case EntityState.Added:
                this.ObjectStateManager.ReplaceKeyWithTemporaryKey(this);
                this._modifiedFields = (BitArray) null;
                this._originalValues = (List<StateManagerValue>) null;
                this._originalComplexObjects = (Dictionary<object, Dictionary<int, object>>) null;
                this.State = EntityState.Added;
                this.TransitionRelationshipsForAdd();
                return;
              case EntityState.Deleted:
                this.Delete(true);
                return;
              case EntityState.Modified:
                this.SetModified();
                this.SetModifiedAll();
                return;
              default:
                throw new ArgumentException(Strings.ObjectContext_InvalidEntityState, nameof (requestedState));
            }
        }
      }
    }

    internal void UpdateOriginalValues(object entity)
    {
      EntityState state = this.State;
      this.UpdateRecordWithSetModified(entity, (DbUpdatableDataRecord) this.EditableOriginalValues);
      if (state != EntityState.Unchanged || this.State != EntityState.Modified)
        return;
      this.ObjectStateManager.ChangeState(this, state, EntityState.Modified);
    }

    internal void UpdateRecordWithoutSetModified(object value, DbUpdatableDataRecord current)
    {
      this.UpdateRecord(value, current, EntityEntry.UpdateRecordBehavior.WithoutSetModified, -1);
    }

    internal void UpdateRecordWithSetModified(object value, DbUpdatableDataRecord current)
    {
      this.UpdateRecord(value, current, EntityEntry.UpdateRecordBehavior.WithSetModified, -1);
    }

    private void UpdateRecord(
      object value,
      DbUpdatableDataRecord current,
      EntityEntry.UpdateRecordBehavior behavior,
      int propertyIndex)
    {
      StateManagerTypeMetadata metadata = current._metadata;
      foreach (FieldMetadata fieldMetadata in metadata.DataRecordInfo.FieldMetadata)
      {
        int ordinal = fieldMetadata.Ordinal;
        StateManagerMemberMetadata managerMemberMetadata = metadata.Member(ordinal);
        object newFieldValue = managerMemberMetadata.GetValue(value) ?? (object) DBNull.Value;
        if (Helper.IsComplexType(fieldMetadata.FieldType.TypeUsage.EdmType))
        {
          object obj = current.GetValue(ordinal);
          if (obj == DBNull.Value)
            throw new InvalidOperationException(Strings.ComplexObject_NullableComplexTypesNotSupported((object) fieldMetadata.FieldType.Name));
          if (newFieldValue != DBNull.Value)
            this.UpdateRecord(newFieldValue, (DbUpdatableDataRecord) obj, behavior, propertyIndex == -1 ? ordinal : propertyIndex);
        }
        else if (this.HasRecordValueChanged((DbDataRecord) current, ordinal, newFieldValue) && !managerMemberMetadata.IsPartOfKey)
        {
          current.SetValue(ordinal, newFieldValue);
          if (behavior == EntityEntry.UpdateRecordBehavior.WithSetModified)
            this.SetModifiedPropertyInternal(propertyIndex == -1 ? ordinal : propertyIndex);
        }
      }
    }

    internal bool HasRecordValueChanged(
      DbDataRecord record,
      int propertyIndex,
      object newFieldValue)
    {
      object x = record.GetValue(propertyIndex);
      if (x != newFieldValue && (DBNull.Value == newFieldValue || DBNull.Value == x || !ByValueEqualityComparer.Default.Equals(x, newFieldValue)))
        return true;
      if (this._cache.EntryHasConceptualNull(this) && this._modifiedFields != null)
        return this._modifiedFields[propertyIndex];
      return false;
    }

    internal void ApplyCurrentValuesInternal(IEntityWrapper wrappedCurrentEntity)
    {
      if (this.State != EntityState.Modified && this.State != EntityState.Unchanged)
        throw new InvalidOperationException(Strings.ObjectContext_EntityMustBeUnchangedOrModified((object) this.State.ToString()));
      if (this.WrappedEntity.IdentityType != wrappedCurrentEntity.IdentityType)
        throw new ArgumentException(Strings.ObjectContext_EntitiesHaveDifferentType((object) this.Entity.GetType().FullName, (object) wrappedCurrentEntity.Entity.GetType().FullName));
      this.CompareKeyProperties(wrappedCurrentEntity.Entity);
      this.UpdateCurrentValueRecord(wrappedCurrentEntity.Entity);
    }

    internal void UpdateCurrentValueRecord(object value)
    {
      this._wrappedEntity.UpdateCurrentValueRecord(value, this);
    }

    internal void ApplyOriginalValuesInternal(IEntityWrapper wrappedOriginalEntity)
    {
      if (this.State != EntityState.Modified && this.State != EntityState.Unchanged && this.State != EntityState.Deleted)
        throw new InvalidOperationException(Strings.ObjectContext_EntityMustBeUnchangedOrModifiedOrDeleted((object) this.State.ToString()));
      if (this.WrappedEntity.IdentityType != wrappedOriginalEntity.IdentityType)
        throw new ArgumentException(Strings.ObjectContext_EntitiesHaveDifferentType((object) this.Entity.GetType().FullName, (object) wrappedOriginalEntity.Entity.GetType().FullName));
      this.CompareKeyProperties(wrappedOriginalEntity.Entity);
      this.UpdateOriginalValues(wrappedOriginalEntity.Entity);
    }

    internal void RemoveFromForeignKeyIndex()
    {
      if (this.IsKeyEntry)
        return;
      foreach (EntityReference fkRelatedEnd in this.FindFKRelatedEnds())
      {
        foreach (EntityKey allKeyValue in fkRelatedEnd.GetAllKeyValues())
          this._cache.RemoveEntryFromForeignKeyIndex(fkRelatedEnd, allKeyValue, this);
      }
    }

    internal void FixupReferencesByForeignKeys(bool replaceAddedRefs, EntitySetBase restrictTo = null)
    {
      this._cache.TransactionManager.BeginGraphUpdate();
      bool setIsLoaded = !this._cache.TransactionManager.IsAttachTracking && !this._cache.TransactionManager.IsAddTracking;
      try
      {
        foreach (Tuple<AssociationSet, ReferentialConstraint> tuple in this.ForeignKeyDependents.Where<Tuple<AssociationSet, ReferentialConstraint>>((Func<Tuple<AssociationSet, ReferentialConstraint>, bool>) (t =>
        {
          if (restrictTo != null && !(t.Item1.SourceSet.Identity == restrictTo.Identity))
            return t.Item1.TargetSet.Identity == restrictTo.Identity;
          return true;
        })))
        {
          EntityReference relatedEndInternal = this.WrappedEntity.RelationshipManager.GetRelatedEndInternal(tuple.Item1.ElementType, (AssociationEndMember) tuple.Item2.FromRole) as EntityReference;
          if (!ForeignKeyFactory.IsConceptualNullKey(relatedEndInternal.CachedForeignKey))
            this.FixupEntityReferenceToPrincipal(relatedEndInternal, (EntityKey) null, setIsLoaded, replaceAddedRefs);
        }
      }
      finally
      {
        this._cache.TransactionManager.EndGraphUpdate();
      }
    }

    internal void FixupEntityReferenceByForeignKey(EntityReference reference)
    {
      reference.IsLoaded = false;
      if (ForeignKeyFactory.IsConceptualNullKey(reference.CachedForeignKey))
        this.ObjectStateManager.ForgetEntryWithConceptualNull(this, false);
      IEntityWrapper referenceValue = reference.ReferenceValue;
      EntityKey foreignKeyValues = ForeignKeyFactory.CreateKeyFromForeignKeyValues(this, (RelatedEnd) reference);
      bool flag;
      if ((object) foreignKeyValues == null || referenceValue.Entity == null)
      {
        flag = true;
      }
      else
      {
        EntityKey other = referenceValue.EntityKey;
        EntityEntry objectStateEntry = referenceValue.ObjectStateEntry;
        if ((other == (EntityKey) null || other.IsTemporary) && objectStateEntry != null)
          other = new EntityKey((EntitySet) objectStateEntry.EntitySet, (IExtendedDataRecord) objectStateEntry.CurrentValues);
        flag = !foreignKeyValues.Equals(other);
      }
      if (this._cache.TransactionManager.RelationshipBeingUpdated != reference)
      {
        if (!flag)
          return;
        this._cache.TransactionManager.BeginGraphUpdate();
        if ((object) foreignKeyValues != null)
          this._cache.TransactionManager.EntityBeingReparented = this.Entity;
        try
        {
          this.FixupEntityReferenceToPrincipal(reference, foreignKeyValues, false, true);
        }
        finally
        {
          this._cache.TransactionManager.EntityBeingReparented = (object) null;
          this._cache.TransactionManager.EndGraphUpdate();
        }
      }
      else
        this.FixupEntityReferenceToPrincipal(reference, foreignKeyValues, false, false);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal void FixupEntityReferenceToPrincipal(
      EntityReference relatedEnd,
      EntityKey foreignKey,
      bool setIsLoaded,
      bool replaceExistingRef)
    {
      if (foreignKey == (EntityKey) null)
        foreignKey = ForeignKeyFactory.CreateKeyFromForeignKeyValues(this, (RelatedEnd) relatedEnd);
      bool flag = this._cache.TransactionManager.RelationshipBeingUpdated != relatedEnd && (!this._cache.TransactionManager.IsForeignKeyUpdate || relatedEnd.ReferenceValue.ObjectStateEntry == null || relatedEnd.ReferenceValue.ObjectStateEntry.State != EntityState.Added);
      relatedEnd.SetCachedForeignKey(foreignKey, this);
      this.ObjectStateManager.ForgetEntryWithConceptualNull(this, false);
      if (foreignKey != (EntityKey) null)
      {
        EntityEntry entry;
        if (this._cache.TryGetEntityEntry(foreignKey, out entry) && !entry.IsKeyEntry && entry.State != EntityState.Deleted && ((replaceExistingRef || EntityEntry.WillNotRefSteal(relatedEnd, entry.WrappedEntity)) && relatedEnd.CanSetEntityType(entry.WrappedEntity)))
        {
          if (flag)
          {
            if (this._cache.TransactionManager.PopulatedEntityReferences != null)
              this._cache.TransactionManager.PopulatedEntityReferences.Add(relatedEnd);
            relatedEnd.SetEntityKey(foreignKey, true);
            if (this._cache.TransactionManager.PopulatedEntityReferences != null)
            {
              EntityReference endOfRelationship = relatedEnd.GetOtherEndOfRelationship(entry.WrappedEntity) as EntityReference;
              if (endOfRelationship != null)
                this._cache.TransactionManager.PopulatedEntityReferences.Add(endOfRelationship);
            }
          }
          if (!setIsLoaded || entry.State == EntityState.Added)
            return;
          relatedEnd.IsLoaded = true;
        }
        else
        {
          this._cache.AddEntryContainingForeignKeyToIndex(relatedEnd, foreignKey, this);
          if (!flag || !replaceExistingRef || relatedEnd.ReferenceValue.Entity == null)
            return;
          relatedEnd.ReferenceValue = NullEntityWrapper.NullWrapper;
        }
      }
      else
      {
        if (!flag)
          return;
        if (replaceExistingRef && (relatedEnd.ReferenceValue.Entity != null || relatedEnd.EntityKey != (EntityKey) null))
          relatedEnd.ReferenceValue = NullEntityWrapper.NullWrapper;
        if (!setIsLoaded)
          return;
        relatedEnd.IsLoaded = true;
      }
    }

    private static bool WillNotRefSteal(
      EntityReference refToPrincipal,
      IEntityWrapper wrappedPrincipal)
    {
      EntityReference endOfRelationship = refToPrincipal.GetOtherEndOfRelationship(wrappedPrincipal) as EntityReference;
      if (refToPrincipal.ReferenceValue.Entity == null && refToPrincipal.NavigationPropertyIsNullOrMissing() && (endOfRelationship == null || endOfRelationship.ReferenceValue.Entity == null && endOfRelationship.NavigationPropertyIsNullOrMissing()) || endOfRelationship != null && (object.ReferenceEquals(endOfRelationship.ReferenceValue.Entity, refToPrincipal.WrappedOwner.Entity) || endOfRelationship.CheckIfNavigationPropertyContainsEntity(refToPrincipal.WrappedOwner)))
        return true;
      if (endOfRelationship == null || object.ReferenceEquals(refToPrincipal.ReferenceValue.Entity, wrappedPrincipal.Entity) || refToPrincipal.CheckIfNavigationPropertyContainsEntity(wrappedPrincipal))
        return false;
      throw new InvalidOperationException(Strings.EntityReference_CannotAddMoreThanOneEntityToEntityReference((object) endOfRelationship.RelationshipNavigation.To, (object) endOfRelationship.RelationshipNavigation.RelationshipName));
    }

    internal bool TryGetReferenceKey(AssociationEndMember principalRole, out EntityKey principalKey)
    {
      EntityReference relatedEnd = this.RelationshipManager.GetRelatedEnd(principalRole.DeclaringType.FullName, principalRole.Name) as EntityReference;
      if (relatedEnd.CachedValue.Entity == null || relatedEnd.CachedValue.ObjectStateEntry == null)
      {
        principalKey = (EntityKey) null;
        return false;
      }
      ref EntityKey local = ref principalKey;
      EntityKey entityKey = relatedEnd.EntityKey;
      if ((object) entityKey == null)
        entityKey = relatedEnd.CachedValue.ObjectStateEntry.EntityKey;
      local = entityKey;
      return principalKey != (EntityKey) null;
    }

    internal void FixupForeignKeysByReference()
    {
      this._cache.TransactionManager.BeginFixupKeysByReference();
      try
      {
        this.FixupForeignKeysByReference((List<EntityEntry>) null);
      }
      finally
      {
        this._cache.TransactionManager.EndFixupKeysByReference();
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void FixupForeignKeysByReference(List<EntityEntry> visited)
    {
      if (!(this.EntitySet as EntitySet).HasForeignKeyRelationships)
        return;
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyDependent in this.ForeignKeyDependents)
      {
        EntityReference relatedEndInternal = this.RelationshipManager.GetRelatedEndInternal(foreignKeyDependent.Item1.ElementType.FullName, foreignKeyDependent.Item2.FromRole.Name) as EntityReference;
        IEntityWrapper referenceValue = relatedEndInternal.ReferenceValue;
        if (referenceValue.Entity != null)
        {
          EntityEntry objectStateEntry = referenceValue.ObjectStateEntry;
          bool? nullable = new bool?();
          if (objectStateEntry != null && objectStateEntry.State == EntityState.Added)
          {
            if (objectStateEntry == this)
            {
              nullable = new bool?(relatedEndInternal.GetOtherEndOfRelationship(referenceValue) is EntityReference);
              if (!nullable.Value)
                goto label_14;
            }
            visited = visited ?? new List<EntityEntry>();
            if (visited.Contains(this))
            {
              if (!nullable.HasValue)
                nullable = new bool?(relatedEndInternal.GetOtherEndOfRelationship(referenceValue) is EntityReference);
              if (nullable.Value)
                throw new InvalidOperationException(Strings.RelationshipManager_CircularRelationshipsWithReferentialConstraints);
            }
            else
            {
              visited.Add(this);
              objectStateEntry.FixupForeignKeysByReference(visited);
              visited.Remove(this);
            }
          }
label_14:
          relatedEndInternal.UpdateForeignKeyValues(this.WrappedEntity, referenceValue, (Dictionary<int, object>) null, false);
        }
        else
        {
          EntityKey entityKey = relatedEndInternal.EntityKey;
          if (entityKey != (EntityKey) null && !entityKey.IsTemporary)
            relatedEndInternal.UpdateForeignKeyValues(this.WrappedEntity, entityKey);
        }
      }
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyPrincipal in this.ForeignKeyPrincipals)
      {
        bool flag1 = false;
        bool flag2 = false;
        RelatedEnd relatedEndInternal = this.RelationshipManager.GetRelatedEndInternal(foreignKeyPrincipal.Item1.ElementType.FullName, foreignKeyPrincipal.Item2.ToRole.Name);
        foreach (IEntityWrapper wrappedEntity in relatedEndInternal.GetWrappedEntities())
        {
          EntityEntry objectStateEntry = wrappedEntity.ObjectStateEntry;
          if (objectStateEntry.State != EntityState.Added && !flag2)
          {
            flag2 = true;
            foreach (EdmProperty toProperty in foreignKeyPrincipal.Item2.ToProperties)
            {
              int olayerMemberName = objectStateEntry._cacheTypeMetadata.GetOrdinalforOLayerMemberName(toProperty.Name);
              if (objectStateEntry._cacheTypeMetadata.Member(olayerMemberName).IsPartOfKey)
              {
                flag1 = true;
                break;
              }
            }
          }
          if (objectStateEntry.State == EntityState.Added || objectStateEntry.State == EntityState.Modified && !flag1)
            (relatedEndInternal.GetOtherEndOfRelationship(wrappedEntity) as EntityReference).UpdateForeignKeyValues(wrappedEntity, this.WrappedEntity, (Dictionary<int, object>) null, false);
        }
      }
    }

    private bool IsPropertyAForeignKey(string propertyName)
    {
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyDependent in this.ForeignKeyDependents)
      {
        foreach (EdmMember toProperty in foreignKeyDependent.Item2.ToProperties)
        {
          if (toProperty.Name == propertyName)
            return true;
        }
      }
      return false;
    }

    private bool IsPropertyAForeignKey(
      string propertyName,
      out List<Pair<string, string>> relationships)
    {
      relationships = (List<Pair<string, string>>) null;
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyDependent in this.ForeignKeyDependents)
      {
        foreach (EdmMember toProperty in foreignKeyDependent.Item2.ToProperties)
        {
          if (toProperty.Name == propertyName)
          {
            if (relationships == null)
              relationships = new List<Pair<string, string>>();
            relationships.Add(new Pair<string, string>(foreignKeyDependent.Item1.ElementType.FullName, foreignKeyDependent.Item2.FromRole.Name));
            break;
          }
        }
      }
      return relationships != null;
    }

    internal void FindRelatedEntityKeysByForeignKeys(
      out Dictionary<RelatedEnd, HashSet<EntityKey>> relatedEntities,
      bool useOriginalValues)
    {
      relatedEntities = (Dictionary<RelatedEnd, HashSet<EntityKey>>) null;
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyDependent in this.ForeignKeyDependents)
      {
        AssociationSet associationSet = foreignKeyDependent.Item1;
        ReferentialConstraint constraint = foreignKeyDependent.Item2;
        string identity = constraint.ToRole.Identity;
        ReadOnlyMetadataCollection<AssociationSetEnd> associationSetEnds = associationSet.AssociationSetEnds;
        AssociationEndMember endMember = !(associationSetEnds[0].CorrespondingAssociationEndMember.Identity == identity) ? associationSetEnds[0].CorrespondingAssociationEndMember : associationSetEnds[1].CorrespondingAssociationEndMember;
        EntitySet entitySetAtEnd = MetadataHelper.GetEntitySetAtEnd(associationSet, endMember);
        EntityKey foreignKeyValues = ForeignKeyFactory.CreateKeyFromForeignKeyValues(this, constraint, entitySetAtEnd, useOriginalValues);
        if (foreignKeyValues != (EntityKey) null)
        {
          EntityReference relatedEndInternal = this.RelationshipManager.GetRelatedEndInternal(associationSet.ElementType, (AssociationEndMember) constraint.FromRole) as EntityReference;
          relatedEntities = relatedEntities != null ? relatedEntities : new Dictionary<RelatedEnd, HashSet<EntityKey>>();
          HashSet<EntityKey> entityKeySet;
          if (!relatedEntities.TryGetValue((RelatedEnd) relatedEndInternal, out entityKeySet))
          {
            entityKeySet = new HashSet<EntityKey>();
            relatedEntities.Add((RelatedEnd) relatedEndInternal, entityKeySet);
          }
          entityKeySet.Add(foreignKeyValues);
        }
      }
    }

    internal IEnumerable<EntityReference> FindFKRelatedEnds()
    {
      HashSet<EntityReference> entityReferenceSet = new HashSet<EntityReference>();
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyDependent in this.ForeignKeyDependents)
      {
        EntityReference relatedEndInternal = this.RelationshipManager.GetRelatedEndInternal(foreignKeyDependent.Item1.ElementType.FullName, foreignKeyDependent.Item2.FromRole.Name) as EntityReference;
        entityReferenceSet.Add(relatedEndInternal);
      }
      return (IEnumerable<EntityReference>) entityReferenceSet;
    }

    internal void DetectChangesInForeignKeys()
    {
      TransactionManager transactionManager = this.ObjectStateManager.TransactionManager;
      foreach (EntityReference fkRelatedEnd in this.FindFKRelatedEnds())
      {
        EntityKey foreignKeyValues = ForeignKeyFactory.CreateKeyFromForeignKeyValues(this, (RelatedEnd) fkRelatedEnd);
        EntityKey cachedForeignKey = fkRelatedEnd.CachedForeignKey;
        bool flag = ForeignKeyFactory.IsConceptualNullKey(cachedForeignKey);
        if (cachedForeignKey != (EntityKey) null || foreignKeyValues != (EntityKey) null)
        {
          if (cachedForeignKey == (EntityKey) null)
          {
            EntityEntry entry;
            this.ObjectStateManager.TryGetEntityEntry(foreignKeyValues, out entry);
            this.AddRelationshipDetectedByForeignKey(transactionManager.AddedRelationshipsByForeignKey, transactionManager.AddedRelationshipsByPrincipalKey, foreignKeyValues, entry, (RelatedEnd) fkRelatedEnd);
          }
          else if (foreignKeyValues == (EntityKey) null)
            EntityEntry.AddDetectedRelationship<EntityKey>(transactionManager.DeletedRelationshipsByForeignKey, cachedForeignKey, (RelatedEnd) fkRelatedEnd);
          else if (!foreignKeyValues.Equals(cachedForeignKey) && (!flag || ForeignKeyFactory.IsConceptualNullKeyChanged(cachedForeignKey, foreignKeyValues)))
          {
            EntityEntry entry;
            this.ObjectStateManager.TryGetEntityEntry(foreignKeyValues, out entry);
            this.AddRelationshipDetectedByForeignKey(transactionManager.AddedRelationshipsByForeignKey, transactionManager.AddedRelationshipsByPrincipalKey, foreignKeyValues, entry, (RelatedEnd) fkRelatedEnd);
            if (!flag)
              EntityEntry.AddDetectedRelationship<EntityKey>(transactionManager.DeletedRelationshipsByForeignKey, cachedForeignKey, (RelatedEnd) fkRelatedEnd);
          }
        }
      }
    }

    internal bool RequiresComplexChangeTracking
    {
      get
      {
        return this._requiresComplexChangeTracking;
      }
    }

    internal bool RequiresScalarChangeTracking
    {
      get
      {
        return this._requiresScalarChangeTracking;
      }
    }

    internal bool RequiresAnyChangeTracking
    {
      get
      {
        return this._requiresAnyChangeTracking;
      }
    }

    internal struct RelationshipEndEnumerable : IEnumerable<RelationshipEntry>, IEnumerable<IEntityStateEntry>, IEnumerable
    {
      internal static readonly RelationshipEntry[] EmptyRelationshipEntryArray = new RelationshipEntry[0];
      private readonly EntityEntry _entityEntry;

      internal RelationshipEndEnumerable(EntityEntry entityEntry)
      {
        this._entityEntry = entityEntry;
      }

      public EntityEntry.RelationshipEndEnumerator GetEnumerator()
      {
        return new EntityEntry.RelationshipEndEnumerator(this._entityEntry);
      }

      IEnumerator<IEntityStateEntry> IEnumerable<IEntityStateEntry>.GetEnumerator()
      {
        return (IEnumerator<IEntityStateEntry>) this.GetEnumerator();
      }

      IEnumerator<RelationshipEntry> IEnumerable<RelationshipEntry>.GetEnumerator()
      {
        return (IEnumerator<RelationshipEntry>) this.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      internal RelationshipEntry[] ToArray()
      {
        RelationshipEntry[] relationshipEntryArray = (RelationshipEntry[]) null;
        if (this._entityEntry != null && 0 < this._entityEntry._countRelationshipEnds)
        {
          RelationshipEntry relationshipEntry = this._entityEntry._headRelationshipEnds;
          relationshipEntryArray = new RelationshipEntry[this._entityEntry._countRelationshipEnds];
          for (int index = 0; index < relationshipEntryArray.Length; ++index)
          {
            relationshipEntryArray[index] = relationshipEntry;
            relationshipEntry = relationshipEntry.GetNextRelationshipEnd(this._entityEntry.EntityKey);
          }
        }
        return relationshipEntryArray ?? EntityEntry.RelationshipEndEnumerable.EmptyRelationshipEntryArray;
      }
    }

    internal struct RelationshipEndEnumerator : IEnumerator<RelationshipEntry>, IEnumerator<IEntityStateEntry>, IDisposable, IEnumerator
    {
      private readonly EntityEntry _entityEntry;
      private RelationshipEntry _current;

      internal RelationshipEndEnumerator(EntityEntry entityEntry)
      {
        this._entityEntry = entityEntry;
        this._current = (RelationshipEntry) null;
      }

      public RelationshipEntry Current
      {
        get
        {
          return this._current;
        }
      }

      IEntityStateEntry IEnumerator<IEntityStateEntry>.Current
      {
        get
        {
          return (IEntityStateEntry) this._current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this._current;
        }
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        if (this._entityEntry != null)
          this._current = this._current != null ? this._current.GetNextRelationshipEnd(this._entityEntry.EntityKey) : this._entityEntry._headRelationshipEnds;
        return null != this._current;
      }

      public void Reset()
      {
      }
    }

    private enum UpdateRecordBehavior
    {
      WithoutSetModified,
      WithSetModified,
    }
  }
}
