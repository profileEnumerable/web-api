// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.Shaper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal abstract class Shaper
  {
    private IList<IEntityWrapper> _materializedEntities;
    public readonly DbDataReader Reader;
    public readonly object[] State;
    public readonly ObjectContext Context;
    public readonly MetadataWorkspace Workspace;
    public readonly MergeOption MergeOption;
    protected readonly bool Streaming;
    private readonly Lazy<DbSpatialDataReader> _spatialReader;

    internal Shaper(
      DbDataReader reader,
      ObjectContext context,
      MetadataWorkspace workspace,
      MergeOption mergeOption,
      int stateCount,
      bool streaming)
    {
      this.Reader = reader;
      this.MergeOption = mergeOption;
      this.State = new object[stateCount];
      this.Context = context;
      this.Workspace = workspace;
      this._spatialReader = new Lazy<DbSpatialDataReader>(new Func<DbSpatialDataReader>(this.CreateSpatialDataReader));
      this.Streaming = streaming;
    }

    public TElement Discriminate<TElement>(
      object[] discriminatorValues,
      Func<object[], EntityType> discriminate,
      KeyValuePair<EntityType, Func<Shaper, TElement>>[] elementDelegates)
    {
      EntityType entityType = discriminate(discriminatorValues);
      Func<Shaper, TElement> func = (Func<Shaper, TElement>) null;
      foreach (KeyValuePair<EntityType, Func<Shaper, TElement>> elementDelegate in elementDelegates)
      {
        if (elementDelegate.Key == entityType)
          func = elementDelegate.Value;
      }
      return func(this);
    }

    public IEntityWrapper HandleEntityNoTracking<TEntity>(IEntityWrapper wrappedEntity)
    {
      this.RegisterMaterializedEntityForEvent(wrappedEntity);
      return wrappedEntity;
    }

    public IEntityWrapper HandleEntity<TEntity>(
      IEntityWrapper wrappedEntity,
      EntityKey entityKey,
      EntitySet entitySet)
    {
      IEntityWrapper wrappedEntity1 = wrappedEntity;
      if ((object) entityKey != null)
      {
        EntityEntry entityEntry = this.Context.ObjectStateManager.FindEntityEntry(entityKey);
        if (entityEntry != null && !entityEntry.IsKeyEntry)
        {
          this.UpdateEntry<TEntity>(wrappedEntity, entityEntry);
          wrappedEntity1 = entityEntry.WrappedEntity;
        }
        else
        {
          this.RegisterMaterializedEntityForEvent(wrappedEntity1);
          if (entityEntry == null)
            this.Context.ObjectStateManager.AddEntry(wrappedEntity, entityKey, entitySet, nameof (HandleEntity), false);
          else
            this.Context.ObjectStateManager.PromoteKeyEntry(entityEntry, wrappedEntity, false, true, false);
        }
      }
      return wrappedEntity1;
    }

    public IEntityWrapper HandleEntityAppendOnly<TEntity>(
      Func<Shaper, IEntityWrapper> constructEntityDelegate,
      EntityKey entityKey,
      EntitySet entitySet)
    {
      IEntityWrapper entityWrapper;
      if ((object) entityKey == null)
      {
        entityWrapper = constructEntityDelegate(this);
        this.RegisterMaterializedEntityForEvent(entityWrapper);
      }
      else
      {
        EntityEntry entityEntry = this.Context.ObjectStateManager.FindEntityEntry(entityKey);
        if (entityEntry != null && !entityEntry.IsKeyEntry)
        {
          if (typeof (TEntity) != entityEntry.WrappedEntity.IdentityType)
          {
            EntityKey entityKey1 = entityEntry.EntityKey;
            throw new NotSupportedException(Strings.Materializer_RecyclingEntity((object) TypeHelpers.GetFullName(entityKey1.EntityContainerName, entityKey1.EntitySetName), (object) typeof (TEntity).FullName, (object) entityEntry.WrappedEntity.IdentityType.FullName, (object) entityKey1.ConcatKeyValue()));
          }
          if (EntityState.Added == entityEntry.State)
            throw new InvalidOperationException(Strings.Materializer_AddedEntityAlreadyExists((object) entityEntry.EntityKey.ConcatKeyValue()));
          entityWrapper = entityEntry.WrappedEntity;
        }
        else
        {
          entityWrapper = constructEntityDelegate(this);
          this.RegisterMaterializedEntityForEvent(entityWrapper);
          if (entityEntry == null)
            this.Context.ObjectStateManager.AddEntry(entityWrapper, entityKey, entitySet, "HandleEntity", false);
          else
            this.Context.ObjectStateManager.PromoteKeyEntry(entityEntry, entityWrapper, false, true, false);
        }
      }
      return entityWrapper;
    }

    public IEntityWrapper HandleFullSpanCollection<TTargetEntity>(
      IEntityWrapper wrappedEntity,
      Coordinator<TTargetEntity> coordinator,
      AssociationEndMember targetMember)
    {
      if (wrappedEntity.Entity != null)
        coordinator.RegisterCloseHandler((Action<Shaper, List<IEntityWrapper>>) ((state, spannedEntities) => this.FullSpanAction<IEntityWrapper>(wrappedEntity, (IList<IEntityWrapper>) spannedEntities, targetMember)));
      return wrappedEntity;
    }

    public IEntityWrapper HandleFullSpanElement(
      IEntityWrapper wrappedSource,
      IEntityWrapper wrappedSpannedEntity,
      AssociationEndMember targetMember)
    {
      if (wrappedSource.Entity == null)
        return wrappedSource;
      List<IEntityWrapper> entityWrapperList = (List<IEntityWrapper>) null;
      if (wrappedSpannedEntity.Entity != null)
      {
        entityWrapperList = new List<IEntityWrapper>(1);
        entityWrapperList.Add(wrappedSpannedEntity);
      }
      else
      {
        EntityKey entityKey = wrappedSource.EntityKey;
        this.CheckClearedEntryOnSpan((object) null, wrappedSource, entityKey, targetMember);
      }
      this.FullSpanAction<IEntityWrapper>(wrappedSource, (IList<IEntityWrapper>) entityWrapperList, targetMember);
      return wrappedSource;
    }

    public IEntityWrapper HandleRelationshipSpan(
      IEntityWrapper wrappedEntity,
      EntityKey targetKey,
      AssociationEndMember targetMember)
    {
      if (wrappedEntity.Entity == null)
        return wrappedEntity;
      EntityKey entityKey = wrappedEntity.EntityKey;
      AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(targetMember);
      this.CheckClearedEntryOnSpan((object) targetKey, wrappedEntity, entityKey, targetMember);
      if ((object) targetKey != null)
      {
        EntitySet endEntitySet;
        AssociationSet cspaceAssociationSet = this.Context.MetadataWorkspace.MetadataOptimization.FindCSpaceAssociationSet((AssociationType) targetMember.DeclaringType, targetMember.Name, targetKey.EntitySetName, targetKey.EntityContainerName, out endEntitySet);
        ObjectStateManager objectStateManager = this.Context.ObjectStateManager;
        EntityState newEntryState;
        if (!ObjectStateManager.TryUpdateExistingRelationships(this.Context, this.MergeOption, cspaceAssociationSet, otherAssociationEnd, entityKey, wrappedEntity, targetMember, targetKey, true, out newEntryState))
        {
          EntityEntry entityEntry = objectStateManager.GetOrAddKeyEntry(targetKey, endEntitySet);
          bool flag = true;
          switch (otherAssociationEnd.RelationshipMultiplicity)
          {
            case RelationshipMultiplicity.ZeroOrOne:
            case RelationshipMultiplicity.One:
              flag = !ObjectStateManager.TryUpdateExistingRelationships(this.Context, this.MergeOption, cspaceAssociationSet, targetMember, targetKey, entityEntry.WrappedEntity, otherAssociationEnd, entityKey, true, out newEntryState);
              if (entityEntry.State == EntityState.Detached)
              {
                entityEntry = objectStateManager.AddKeyEntry(targetKey, endEntitySet);
                break;
              }
              break;
          }
          if (flag)
          {
            if (entityEntry.IsKeyEntry || newEntryState == EntityState.Deleted)
            {
              RelationshipWrapper wrapper = new RelationshipWrapper(cspaceAssociationSet, otherAssociationEnd.Name, entityKey, targetMember.Name, targetKey);
              objectStateManager.AddNewRelation(wrapper, newEntryState);
            }
            else if (entityEntry.State != EntityState.Deleted)
            {
              ObjectStateManager.AddEntityToCollectionOrReference(this.MergeOption, wrappedEntity, otherAssociationEnd, entityEntry.WrappedEntity, targetMember, true, false, false);
            }
            else
            {
              RelationshipWrapper wrapper = new RelationshipWrapper(cspaceAssociationSet, otherAssociationEnd.Name, entityKey, targetMember.Name, targetKey);
              objectStateManager.AddNewRelation(wrapper, EntityState.Deleted);
            }
          }
        }
      }
      else
      {
        RelatedEnd relatedEnd;
        if (this.TryGetRelatedEnd(wrappedEntity, (AssociationType) targetMember.DeclaringType, otherAssociationEnd.Name, targetMember.Name, out relatedEnd))
          this.SetIsLoadedForSpan(relatedEnd, false);
      }
      return wrappedEntity;
    }

    private bool TryGetRelatedEnd(
      IEntityWrapper wrappedEntity,
      AssociationType associationType,
      string sourceEndName,
      string targetEndName,
      out RelatedEnd relatedEnd)
    {
      AssociationType ospaceAssociationType = this.Workspace.MetadataOptimization.GetOSpaceAssociationType(associationType, (Func<AssociationType>) (() => this.Workspace.GetItemCollection(DataSpace.OSpace).GetItem<AssociationType>(associationType.FullName)));
      AssociationEndMember sourceMember = (AssociationEndMember) null;
      AssociationEndMember targetMember = (AssociationEndMember) null;
      foreach (AssociationEndMember associationEndMember in ospaceAssociationType.AssociationEndMembers)
      {
        if (associationEndMember.Name == sourceEndName)
          sourceMember = associationEndMember;
        else if (associationEndMember.Name == targetEndName)
          targetMember = associationEndMember;
      }
      if (sourceMember != null && targetMember != null)
      {
        bool flag = false;
        if (wrappedEntity.EntityKey == (EntityKey) null)
        {
          flag = true;
        }
        else
        {
          EntitySet endEntitySet;
          if (this.Workspace.MetadataOptimization.FindCSpaceAssociationSet(associationType, sourceEndName, wrappedEntity.EntityKey.EntitySetName, wrappedEntity.EntityKey.EntityContainerName, out endEntitySet) != null)
            flag = true;
        }
        if (flag)
        {
          relatedEnd = DelegateFactory.GetRelatedEnd(wrappedEntity.RelationshipManager, sourceMember, targetMember, (RelatedEnd) null);
          return true;
        }
      }
      relatedEnd = (RelatedEnd) null;
      return false;
    }

    private void SetIsLoadedForSpan(RelatedEnd relatedEnd, bool forceToTrue)
    {
      if (!forceToTrue)
      {
        forceToTrue = relatedEnd.IsEmpty();
        EntityReference entityReference = relatedEnd as EntityReference;
        if (entityReference != null)
          forceToTrue &= entityReference.EntityKey == (EntityKey) null;
      }
      if (!forceToTrue && this.MergeOption != MergeOption.OverwriteChanges)
        return;
      relatedEnd.IsLoaded = true;
    }

    public IEntityWrapper HandleIEntityWithKey<TEntity>(
      IEntityWrapper wrappedEntity,
      EntitySet entitySet)
    {
      return this.HandleEntity<TEntity>(wrappedEntity, wrappedEntity.EntityKey, entitySet);
    }

    public bool SetColumnValue(int recordStateSlotNumber, int ordinal, object value)
    {
      ((RecordState) this.State[recordStateSlotNumber]).SetColumnValue(ordinal, value);
      return true;
    }

    public bool SetEntityRecordInfo(
      int recordStateSlotNumber,
      EntityKey entityKey,
      EntitySet entitySet)
    {
      ((RecordState) this.State[recordStateSlotNumber]).SetEntityRecordInfo(entityKey, entitySet);
      return true;
    }

    public bool SetState<T>(int ordinal, T value)
    {
      this.State[ordinal] = (object) value;
      return true;
    }

    public T SetStatePassthrough<T>(int ordinal, T value)
    {
      this.State[ordinal] = (object) value;
      return value;
    }

    public TProperty GetPropertyValueWithErrorHandling<TProperty>(
      int ordinal,
      string propertyName,
      string typeName)
    {
      return new Shaper.PropertyErrorHandlingValueReader<TProperty>(propertyName, typeName).GetValue(this.Reader, ordinal);
    }

    public TColumn GetColumnValueWithErrorHandling<TColumn>(int ordinal)
    {
      return new Shaper.ColumnErrorHandlingValueReader<TColumn>().GetValue(this.Reader, ordinal);
    }

    protected virtual DbSpatialDataReader CreateSpatialDataReader()
    {
      return SpatialHelpers.CreateSpatialDataReader(this.Workspace, this.Reader);
    }

    public DbGeography GetGeographyColumnValue(int ordinal)
    {
      if (this.Streaming)
        return this._spatialReader.Value.GetGeography(ordinal);
      return (DbGeography) this.Reader.GetValue(ordinal);
    }

    public DbGeometry GetGeometryColumnValue(int ordinal)
    {
      if (this.Streaming)
        return this._spatialReader.Value.GetGeometry(ordinal);
      return (DbGeometry) this.Reader.GetValue(ordinal);
    }

    public TColumn GetSpatialColumnValueWithErrorHandling<TColumn>(
      int ordinal,
      PrimitiveTypeKind spatialTypeKind)
    {
      return spatialTypeKind != PrimitiveTypeKind.Geography ? (!this.Streaming ? new Shaper.ColumnErrorHandlingValueReader<TColumn>((Func<DbDataReader, int, TColumn>) ((reader, column) => (TColumn) this.Reader.GetValue(column)), (Func<DbDataReader, int, object>) ((reader, column) => this.Reader.GetValue(column))).GetValue(this.Reader, ordinal) : new Shaper.ColumnErrorHandlingValueReader<TColumn>((Func<DbDataReader, int, TColumn>) ((reader, column) => (TColumn) this._spatialReader.Value.GetGeometry(column)), (Func<DbDataReader, int, object>) ((reader, column) => (object) this._spatialReader.Value.GetGeometry(column))).GetValue(this.Reader, ordinal)) : (!this.Streaming ? new Shaper.ColumnErrorHandlingValueReader<TColumn>((Func<DbDataReader, int, TColumn>) ((reader, column) => (TColumn) this.Reader.GetValue(column)), (Func<DbDataReader, int, object>) ((reader, column) => this.Reader.GetValue(column))).GetValue(this.Reader, ordinal) : new Shaper.ColumnErrorHandlingValueReader<TColumn>((Func<DbDataReader, int, TColumn>) ((reader, column) => (TColumn) this._spatialReader.Value.GetGeography(column)), (Func<DbDataReader, int, object>) ((reader, column) => (object) this._spatialReader.Value.GetGeography(column))).GetValue(this.Reader, ordinal));
    }

    public TProperty GetSpatialPropertyValueWithErrorHandling<TProperty>(
      int ordinal,
      string propertyName,
      string typeName,
      PrimitiveTypeKind spatialTypeKind)
    {
      return !Helper.IsGeographicTypeKind(spatialTypeKind) ? (!this.Streaming ? new Shaper.PropertyErrorHandlingValueReader<TProperty>(propertyName, typeName, (Func<DbDataReader, int, TProperty>) ((reader, column) => (TProperty) this.Reader.GetValue(column)), (Func<DbDataReader, int, object>) ((reader, column) => this.Reader.GetValue(column))).GetValue(this.Reader, ordinal) : new Shaper.PropertyErrorHandlingValueReader<TProperty>(propertyName, typeName, (Func<DbDataReader, int, TProperty>) ((reader, column) => (TProperty) this._spatialReader.Value.GetGeometry(column)), (Func<DbDataReader, int, object>) ((reader, column) => (object) this._spatialReader.Value.GetGeometry(column))).GetValue(this.Reader, ordinal)) : (!this.Streaming ? new Shaper.PropertyErrorHandlingValueReader<TProperty>(propertyName, typeName, (Func<DbDataReader, int, TProperty>) ((reader, column) => (TProperty) this.Reader.GetValue(column)), (Func<DbDataReader, int, object>) ((reader, column) => this.Reader.GetValue(column))).GetValue(this.Reader, ordinal) : new Shaper.PropertyErrorHandlingValueReader<TProperty>(propertyName, typeName, (Func<DbDataReader, int, TProperty>) ((reader, column) => (TProperty) this._spatialReader.Value.GetGeography(column)), (Func<DbDataReader, int, object>) ((reader, column) => (object) this._spatialReader.Value.GetGeography(column))).GetValue(this.Reader, ordinal));
    }

    private void CheckClearedEntryOnSpan(
      object targetValue,
      IEntityWrapper wrappedSource,
      EntityKey sourceKey,
      AssociationEndMember targetMember)
    {
      if ((object) sourceKey == null || targetValue != null || this.MergeOption != MergeOption.PreserveChanges && this.MergeOption != MergeOption.OverwriteChanges)
        return;
      EdmType elementType = (EdmType) ((RefType) MetadataHelper.GetOtherAssociationEnd(targetMember).TypeUsage.EdmType).ElementType;
      TypeUsage outTypeUsage;
      if (this.Context.Perspective.TryGetType(wrappedSource.IdentityType, out outTypeUsage) && !outTypeUsage.EdmType.EdmEquals((MetadataItem) elementType) && !TypeSemantics.IsSubTypeOf(outTypeUsage.EdmType, elementType))
        return;
      this.CheckClearedEntryOnSpan(sourceKey, targetMember);
    }

    private void CheckClearedEntryOnSpan(EntityKey sourceKey, AssociationEndMember targetMember)
    {
      AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(targetMember);
      EntitySet endEntitySet;
      AssociationSet cspaceAssociationSet = this.Context.MetadataWorkspace.MetadataOptimization.FindCSpaceAssociationSet((AssociationType) otherAssociationEnd.DeclaringType, otherAssociationEnd.Name, sourceKey.EntitySetName, sourceKey.EntityContainerName, out endEntitySet);
      if (cspaceAssociationSet == null)
        return;
      this.Context.ObjectStateManager.RemoveRelationships(this.MergeOption, cspaceAssociationSet, sourceKey, otherAssociationEnd);
    }

    private void FullSpanAction<TTargetEntity>(
      IEntityWrapper wrappedSource,
      IList<TTargetEntity> spannedEntities,
      AssociationEndMember targetMember)
    {
      if (wrappedSource.Entity == null)
        return;
      AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(targetMember);
      RelatedEnd relatedEnd;
      if (!this.TryGetRelatedEnd(wrappedSource, (AssociationType) targetMember.DeclaringType, otherAssociationEnd.Name, targetMember.Name, out relatedEnd))
        return;
      int num = this.Context.ObjectStateManager.UpdateRelationships(this.Context, this.MergeOption, (AssociationSet) relatedEnd.RelationshipSet, otherAssociationEnd, wrappedSource, targetMember, (IList) spannedEntities, true);
      this.SetIsLoadedForSpan(relatedEnd, num > 0);
    }

    private void UpdateEntry<TEntity>(IEntityWrapper wrappedEntity, EntityEntry existingEntry)
    {
      Type type = typeof (TEntity);
      if (type != existingEntry.WrappedEntity.IdentityType)
      {
        EntityKey entityKey = existingEntry.EntityKey;
        throw new NotSupportedException(Strings.Materializer_RecyclingEntity((object) TypeHelpers.GetFullName(entityKey.EntityContainerName, entityKey.EntitySetName), (object) type.FullName, (object) existingEntry.WrappedEntity.IdentityType.FullName, (object) entityKey.ConcatKeyValue()));
      }
      if (EntityState.Added == existingEntry.State)
        throw new InvalidOperationException(Strings.Materializer_AddedEntityAlreadyExists((object) existingEntry.EntityKey.ConcatKeyValue()));
      if (this.MergeOption == MergeOption.AppendOnly)
        return;
      if (MergeOption.OverwriteChanges == this.MergeOption)
      {
        if (EntityState.Deleted == existingEntry.State)
          existingEntry.RevertDelete();
        existingEntry.UpdateCurrentValueRecord(wrappedEntity.Entity);
        this.Context.ObjectStateManager.ForgetEntryWithConceptualNull(existingEntry, true);
        existingEntry.AcceptChanges();
        this.Context.ObjectStateManager.FixupReferencesByForeignKeys(existingEntry, true);
      }
      else if (EntityState.Unchanged == existingEntry.State)
      {
        existingEntry.UpdateCurrentValueRecord(wrappedEntity.Entity);
        this.Context.ObjectStateManager.ForgetEntryWithConceptualNull(existingEntry, true);
        existingEntry.AcceptChanges();
        this.Context.ObjectStateManager.FixupReferencesByForeignKeys(existingEntry, true);
      }
      else if (this.Context.ContextOptions.UseLegacyPreserveChangesBehavior)
        existingEntry.UpdateRecordWithoutSetModified(wrappedEntity.Entity, (DbUpdatableDataRecord) existingEntry.EditableOriginalValues);
      else
        existingEntry.UpdateRecordWithSetModified(wrappedEntity.Entity, (DbUpdatableDataRecord) existingEntry.EditableOriginalValues);
    }

    public void RaiseMaterializedEvents()
    {
      if (this._materializedEntities == null)
        return;
      foreach (IEntityWrapper materializedEntity in (IEnumerable<IEntityWrapper>) this._materializedEntities)
        this.Context.OnObjectMaterialized(materializedEntity.Entity);
      this._materializedEntities.Clear();
    }

    public void InitializeForOnMaterialize()
    {
      if (this.Context.OnMaterializedHasHandlers)
      {
        if (this._materializedEntities != null)
          return;
        this._materializedEntities = (IList<IEntityWrapper>) new List<IEntityWrapper>();
      }
      else
      {
        if (this._materializedEntities == null)
          return;
        this._materializedEntities = (IList<IEntityWrapper>) null;
      }
    }

    protected void RegisterMaterializedEntityForEvent(IEntityWrapper wrappedEntity)
    {
      if (this._materializedEntities == null)
        return;
      this._materializedEntities.Add(wrappedEntity);
    }

    internal abstract class ErrorHandlingValueReader<T>
    {
      private readonly Func<DbDataReader, int, T> getTypedValue;
      private readonly Func<DbDataReader, int, object> getUntypedValue;

      protected ErrorHandlingValueReader(
        Func<DbDataReader, int, T> typedValueAccessor,
        Func<DbDataReader, int, object> untypedValueAccessor)
      {
        this.getTypedValue = typedValueAccessor;
        this.getUntypedValue = untypedValueAccessor;
      }

      protected ErrorHandlingValueReader()
        : this(new Func<DbDataReader, int, T>(Shaper.ErrorHandlingValueReader<T>.GetTypedValueDefault), new Func<DbDataReader, int, object>(Shaper.ErrorHandlingValueReader<T>.GetUntypedValueDefault))
      {
      }

      private static T GetTypedValueDefault(DbDataReader reader, int ordinal)
      {
        Type underlyingType = Nullable.GetUnderlyingType(typeof (T));
        if (underlyingType != (Type) null && underlyingType.IsEnum())
          return (T) Shaper.ErrorHandlingValueReader<T>.GetGenericTypedValueDefaultMethod(underlyingType).Invoke((object) null, new object[2]
          {
            (object) reader,
            (object) ordinal
          });
        bool isNullable;
        return (T) CodeGenEmitter.GetReaderMethod(typeof (T), out isNullable).Invoke((object) reader, new object[1]
        {
          (object) ordinal
        });
      }

      public static MethodInfo GetGenericTypedValueDefaultMethod(Type underlyingType)
      {
        return typeof (Shaper.ErrorHandlingValueReader<>).MakeGenericType(underlyingType).GetOnlyDeclaredMethod("GetTypedValueDefault");
      }

      private static object GetUntypedValueDefault(DbDataReader reader, int ordinal)
      {
        return reader.GetValue(ordinal);
      }

      internal T GetValue(DbDataReader reader, int ordinal)
      {
        if (reader.IsDBNull(ordinal))
        {
          try
          {
            return (T) null;
          }
          catch (NullReferenceException ex)
          {
            throw this.CreateNullValueException();
          }
        }
        else
        {
          try
          {
            return this.getTypedValue(reader, ordinal);
          }
          catch (Exception ex)
          {
            if (ex.IsCatchableExceptionType())
            {
              object obj = this.getUntypedValue(reader, ordinal);
              Type type = obj == null ? (Type) null : obj.GetType();
              if (!typeof (T).IsAssignableFrom(type))
                throw this.CreateWrongTypeException(type);
            }
            throw;
          }
        }
      }

      protected abstract Exception CreateNullValueException();

      protected abstract Exception CreateWrongTypeException(Type resultType);
    }

    private class ColumnErrorHandlingValueReader<TColumn> : Shaper.ErrorHandlingValueReader<TColumn>
    {
      internal ColumnErrorHandlingValueReader()
      {
      }

      internal ColumnErrorHandlingValueReader(
        Func<DbDataReader, int, TColumn> typedAccessor,
        Func<DbDataReader, int, object> untypedAccessor)
        : base(typedAccessor, untypedAccessor)
      {
      }

      protected override Exception CreateNullValueException()
      {
        return (Exception) new InvalidOperationException(Strings.Materializer_NullReferenceCast((object) typeof (TColumn)));
      }

      protected override Exception CreateWrongTypeException(Type resultType)
      {
        return (Exception) EntityUtil.ValueInvalidCast(resultType, typeof (TColumn));
      }
    }

    private class PropertyErrorHandlingValueReader<TProperty> : Shaper.ErrorHandlingValueReader<TProperty>
    {
      private readonly string _propertyName;
      private readonly string _typeName;

      internal PropertyErrorHandlingValueReader(string propertyName, string typeName)
      {
        this._propertyName = propertyName;
        this._typeName = typeName;
      }

      internal PropertyErrorHandlingValueReader(
        string propertyName,
        string typeName,
        Func<DbDataReader, int, TProperty> typedAccessor,
        Func<DbDataReader, int, object> untypedAccessor)
        : base(typedAccessor, untypedAccessor)
      {
        this._propertyName = propertyName;
        this._typeName = typeName;
      }

      protected override Exception CreateNullValueException()
      {
        Type type = Nullable.GetUnderlyingType(typeof (TProperty));
        if ((object) type == null)
          type = typeof (TProperty);
        return (Exception) new ConstraintException(Strings.Materializer_SetInvalidValue((object) type, (object) this._typeName, (object) this._propertyName, (object) "null"));
      }

      protected override Exception CreateWrongTypeException(Type resultType)
      {
        Type type = Nullable.GetUnderlyingType(typeof (TProperty));
        if ((object) type == null)
          type = typeof (TProperty);
        return (Exception) new InvalidOperationException(Strings.Materializer_SetInvalidValue((object) type, (object) this._typeName, (object) this._propertyName, (object) resultType));
      }
    }
  }
}
