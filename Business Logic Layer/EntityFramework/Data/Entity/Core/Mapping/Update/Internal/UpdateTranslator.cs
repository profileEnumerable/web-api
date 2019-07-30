// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.UpdateTranslator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.EntityClient.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class UpdateTranslator
  {
    private readonly EntityAdapter _adapter;
    private readonly Dictionary<EntitySetBase, ChangeNode> _changes;
    private readonly Dictionary<EntitySetBase, List<ExtractedStateEntry>> _functionChanges;
    private readonly List<System.Data.Entity.Core.IEntityStateEntry> _stateEntries;
    private readonly Set<EntityKey> _knownEntityKeys;
    private readonly Dictionary<EntityKey, AssociationSet> _requiredEntities;
    private readonly Set<EntityKey> _optionalEntities;
    private readonly Set<EntityKey> _includedValueEntities;
    private readonly IEntityStateManager _stateManager;
    private readonly DbInterceptionContext _interceptionContext;
    private readonly RecordConverter _recordConverter;
    private readonly UpdateTranslator.RelationshipConstraintValidator _constraintValidator;
    private readonly DbProviderServices _providerServices;
    private Dictionary<ModificationFunctionMapping, DbCommandDefinition> _modificationFunctionCommandDefinitions;
    private readonly Dictionary<Tuple<EntitySetBase, StructuralType>, ExtractorMetadata> _extractorMetadata;
    internal readonly IEqualityComparer<CompositeKey> KeyComparer;

    public UpdateTranslator(EntityAdapter adapter)
      : this()
    {
      this._stateManager = (IEntityStateManager) adapter.Context.ObjectStateManager;
      this._interceptionContext = adapter.Context.InterceptionContext;
      this._adapter = adapter;
      this._providerServices = adapter.Connection.StoreProviderFactory.GetProviderServices();
    }

    protected UpdateTranslator()
    {
      this._changes = new Dictionary<EntitySetBase, ChangeNode>();
      this._functionChanges = new Dictionary<EntitySetBase, List<ExtractedStateEntry>>();
      this._stateEntries = new List<System.Data.Entity.Core.IEntityStateEntry>();
      this._knownEntityKeys = new Set<EntityKey>();
      this._requiredEntities = new Dictionary<EntityKey, AssociationSet>();
      this._optionalEntities = new Set<EntityKey>();
      this._includedValueEntities = new Set<EntityKey>();
      this._interceptionContext = new DbInterceptionContext();
      this._recordConverter = new RecordConverter(this);
      this._constraintValidator = new UpdateTranslator.RelationshipConstraintValidator();
      this._extractorMetadata = new Dictionary<Tuple<EntitySetBase, StructuralType>, ExtractorMetadata>();
      KeyManager keyManager = new KeyManager();
      this.KeyManager = keyManager;
      this.KeyComparer = CompositeKey.CreateComparer(keyManager);
    }

    internal MetadataWorkspace MetadataWorkspace
    {
      get
      {
        return this.Connection.GetMetadataWorkspace();
      }
    }

    internal virtual KeyManager KeyManager { get; private set; }

    internal ViewLoader ViewLoader
    {
      get
      {
        return this.MetadataWorkspace.GetUpdateViewLoader();
      }
    }

    internal RecordConverter RecordConverter
    {
      get
      {
        return this._recordConverter;
      }
    }

    internal virtual EntityConnection Connection
    {
      get
      {
        return this._adapter.Connection;
      }
    }

    internal virtual int? CommandTimeout
    {
      get
      {
        return this._adapter.CommandTimeout;
      }
    }

    public virtual DbInterceptionContext InterceptionContext
    {
      get
      {
        return this._interceptionContext;
      }
    }

    internal void RegisterReferentialConstraints(System.Data.Entity.Core.IEntityStateEntry stateEntry)
    {
      if (stateEntry.IsRelationship)
      {
        AssociationSet entitySet = (AssociationSet) stateEntry.EntitySet;
        if (0 >= entitySet.ElementType.ReferentialConstraints.Count)
          return;
        DbDataRecord dbDataRecord = stateEntry.State == EntityState.Added ? (DbDataRecord) stateEntry.CurrentValues : stateEntry.OriginalValues;
        using (ReadOnlyMetadataCollection<ReferentialConstraint>.Enumerator enumerator1 = entitySet.ElementType.ReferentialConstraints.GetEnumerator())
        {
label_14:
          while (enumerator1.MoveNext())
          {
            ReferentialConstraint current = enumerator1.Current;
            EntityKey entityKey1 = (EntityKey) dbDataRecord[current.FromRole.Name];
            EntityKey entityKey2 = (EntityKey) dbDataRecord[current.ToRole.Name];
            using (ReadOnlyMetadataCollection<EdmProperty>.Enumerator enumerator2 = current.FromProperties.GetEnumerator())
            {
              using (ReadOnlyMetadataCollection<EdmProperty>.Enumerator enumerator3 = current.ToProperties.GetEnumerator())
              {
                while (true)
                {
                  if (enumerator2.MoveNext())
                  {
                    if (enumerator3.MoveNext())
                    {
                      int keyMemberCount1;
                      int keyMemberOffset1 = UpdateTranslator.GetKeyMemberOffset(current.FromRole, enumerator2.Current, out keyMemberCount1);
                      int keyMemberCount2;
                      int keyMemberOffset2 = UpdateTranslator.GetKeyMemberOffset(current.ToRole, enumerator3.Current, out keyMemberCount2);
                      int identifierForMemberOffset1 = this.KeyManager.GetKeyIdentifierForMemberOffset(entityKey1, keyMemberOffset1, keyMemberCount1);
                      int identifierForMemberOffset2 = this.KeyManager.GetKeyIdentifierForMemberOffset(entityKey2, keyMemberOffset2, keyMemberCount2);
                      this.KeyManager.AddReferentialConstraint(stateEntry, identifierForMemberOffset2, identifierForMemberOffset1);
                    }
                    else
                      goto label_14;
                  }
                  else
                    goto label_14;
                }
              }
            }
          }
        }
      }
      else
      {
        if (stateEntry.IsKeyEntry)
          return;
        if (stateEntry.State == EntityState.Added || stateEntry.State == EntityState.Modified)
          this.RegisterEntityReferentialConstraints(stateEntry, true);
        if (stateEntry.State != EntityState.Deleted && stateEntry.State != EntityState.Modified)
          return;
        this.RegisterEntityReferentialConstraints(stateEntry, false);
      }
    }

    private void RegisterEntityReferentialConstraints(
      System.Data.Entity.Core.IEntityStateEntry stateEntry,
      bool currentValues)
    {
      IExtendedDataRecord extendedDataRecord = currentValues ? (IExtendedDataRecord) stateEntry.CurrentValues : (IExtendedDataRecord) stateEntry.OriginalValues;
      EntitySet entitySet1 = (EntitySet) stateEntry.EntitySet;
      EntityKey entityKey = stateEntry.EntityKey;
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyDependent in entitySet1.ForeignKeyDependents)
      {
        AssociationSet associationSet = foreignKeyDependent.Item1;
        ReferentialConstraint referentialConstraint = foreignKeyDependent.Item2;
        if (MetadataHelper.GetEntityTypeForEnd((AssociationEndMember) referentialConstraint.ToRole).IsAssignableFrom(extendedDataRecord.DataRecordInfo.RecordType.EdmType))
        {
          EntityKey principalKey = (EntityKey) null;
          if (!currentValues || !this._stateManager.TryGetReferenceKey(entityKey, (AssociationEndMember) referentialConstraint.FromRole, out principalKey))
          {
            EntityType entityTypeForEnd = MetadataHelper.GetEntityTypeForEnd((AssociationEndMember) referentialConstraint.FromRole);
            bool flag = false;
            object[] compositeKeyValues = new object[entityTypeForEnd.KeyMembers.Count];
            int index1 = 0;
            for (int length = compositeKeyValues.Length; index1 < length; ++index1)
            {
              EdmProperty keyMember = (EdmProperty) entityTypeForEnd.KeyMembers[index1];
              int index2 = referentialConstraint.FromProperties.IndexOf(keyMember);
              int ordinal = extendedDataRecord.GetOrdinal(referentialConstraint.ToProperties[index2].Name);
              if (extendedDataRecord.IsDBNull(ordinal))
              {
                flag = true;
                break;
              }
              compositeKeyValues[index1] = extendedDataRecord.GetValue(ordinal);
            }
            if (!flag)
            {
              EntitySet entitySet2 = associationSet.AssociationSetEnds[referentialConstraint.FromRole.Name].EntitySet;
              principalKey = 1 != compositeKeyValues.Length ? new EntityKey((EntitySetBase) entitySet2, compositeKeyValues) : new EntityKey((EntitySetBase) entitySet2, compositeKeyValues[0]);
            }
          }
          if ((EntityKey) null != principalKey)
          {
            System.Data.Entity.Core.IEntityStateEntry stateEntry1;
            EntityKey tempKey;
            if (!this._stateManager.TryGetEntityStateEntry(principalKey, out stateEntry1) && currentValues && this.KeyManager.TryGetTempKey(principalKey, out tempKey))
            {
              if ((EntityKey) null == tempKey)
                throw EntityUtil.Update(Strings.Update_AmbiguousForeignKey((object) referentialConstraint.ToRole.DeclaringType.FullName), (Exception) null, stateEntry);
              principalKey = tempKey;
            }
            this.AddValidAncillaryKey(principalKey, this._optionalEntities);
            int index = 0;
            for (int count = referentialConstraint.FromProperties.Count; index < count; ++index)
            {
              EdmProperty fromProperty = referentialConstraint.FromProperties[index];
              EdmProperty toProperty = referentialConstraint.ToProperties[index];
              int keyMemberCount1;
              int keyMemberOffset1 = UpdateTranslator.GetKeyMemberOffset(referentialConstraint.FromRole, fromProperty, out keyMemberCount1);
              int identifierForMemberOffset = this.KeyManager.GetKeyIdentifierForMemberOffset(principalKey, keyMemberOffset1, keyMemberCount1);
              int dependentIdentifier;
              if (entitySet1.ElementType.KeyMembers.Contains((EdmMember) toProperty))
              {
                int keyMemberCount2;
                int keyMemberOffset2 = UpdateTranslator.GetKeyMemberOffset(referentialConstraint.ToRole, toProperty, out keyMemberCount2);
                dependentIdentifier = this.KeyManager.GetKeyIdentifierForMemberOffset(entityKey, keyMemberOffset2, keyMemberCount2);
              }
              else
                dependentIdentifier = this.KeyManager.GetKeyIdentifierForMember(entityKey, toProperty.Name, currentValues);
              if (currentValues && stateEntry1 != null && stateEntry1.State == EntityState.Deleted && (stateEntry.State == EntityState.Added || stateEntry.State == EntityState.Modified))
                throw EntityUtil.Update(Strings.Update_InsertingOrUpdatingReferenceToDeletedEntity((object) associationSet.ElementType.FullName), (Exception) null, stateEntry, stateEntry1);
              this.KeyManager.AddReferentialConstraint(stateEntry, dependentIdentifier, identifierForMemberOffset);
            }
          }
        }
      }
    }

    private static int GetKeyMemberOffset(
      RelationshipEndMember role,
      EdmProperty property,
      out int keyMemberCount)
    {
      EntityType elementType = (EntityType) ((RefType) role.TypeUsage.EdmType).ElementType;
      keyMemberCount = elementType.KeyMembers.Count;
      return elementType.KeyMembers.IndexOf((EdmMember) property);
    }

    internal IEnumerable<System.Data.Entity.Core.IEntityStateEntry> GetRelationships(
      EntityKey entityKey)
    {
      return this._stateManager.FindRelationshipsByKey(entityKey);
    }

    internal virtual int Update()
    {
      Dictionary<int, object> identifierValues = new Dictionary<int, object>();
      List<KeyValuePair<PropagatorResult, object>> generatedValues = new List<KeyValuePair<PropagatorResult, object>>();
      IEnumerable<UpdateCommand> updateCommands = this.ProduceCommands();
      UpdateCommand source = (UpdateCommand) null;
      try
      {
        foreach (UpdateCommand updateCommand in updateCommands)
        {
          source = updateCommand;
          this.ValidateRowsAffected(updateCommand.Execute(identifierValues, generatedValues), source);
        }
      }
      catch (Exception ex)
      {
        if (ex.RequiresContext())
          throw new UpdateException(Strings.Update_GeneralExecutionException, ex, this.DetermineStateEntriesFromSource(source).Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
        throw;
      }
      this.BackPropagateServerGen(generatedValues);
      return this.AcceptChanges();
    }

    internal virtual async Task<int> UpdateAsync(CancellationToken cancellationToken)
    {
      Dictionary<int, object> identifierValues = new Dictionary<int, object>();
      List<KeyValuePair<PropagatorResult, object>> generatedValues = new List<KeyValuePair<PropagatorResult, object>>();
      IEnumerable<UpdateCommand> orderedCommands = this.ProduceCommands();
      UpdateCommand source = (UpdateCommand) null;
      try
      {
        foreach (UpdateCommand updateCommand in orderedCommands)
        {
          source = updateCommand;
          long rowsAffected = await updateCommand.ExecuteAsync(identifierValues, generatedValues, cancellationToken).WithCurrentCulture<long>();
          this.ValidateRowsAffected(rowsAffected, source);
        }
      }
      catch (Exception ex)
      {
        if (ex.RequiresContext())
          throw new UpdateException(Strings.Update_GeneralExecutionException, ex, this.DetermineStateEntriesFromSource(source).Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
        throw;
      }
      this.BackPropagateServerGen(generatedValues);
      return this.AcceptChanges();
    }

    protected virtual IEnumerable<UpdateCommand> ProduceCommands()
    {
      this.PullModifiedEntriesFromStateManager();
      this.PullUnchangedEntriesFromStateManager();
      this._constraintValidator.ValidateConstraints();
      this.KeyManager.ValidateReferentialIntegrityGraphAcyclic();
      IEnumerable<UpdateCommand> orderedVertices;
      IEnumerable<UpdateCommand> remainder;
      if (!new UpdateCommandOrderer(this.ProduceDynamicCommands().Concat<UpdateCommand>(this.ProduceFunctionCommands()), this).TryTopologicalSort(out orderedVertices, out remainder))
        throw this.DependencyOrderingError(remainder);
      return orderedVertices;
    }

    private void ValidateRowsAffected(long rowsAffected, UpdateCommand source)
    {
      if (0L == rowsAffected)
      {
        IEnumerable<System.Data.Entity.Core.IEntityStateEntry> entriesFromSource = this.DetermineStateEntriesFromSource(source);
        throw new OptimisticConcurrencyException(Strings.Update_ConcurrencyError((object) rowsAffected), (Exception) null, entriesFromSource.Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
      }
    }

    private IEnumerable<System.Data.Entity.Core.IEntityStateEntry> DetermineStateEntriesFromSource(
      UpdateCommand source)
    {
      if (source == null)
        return Enumerable.Empty<System.Data.Entity.Core.IEntityStateEntry>();
      return (IEnumerable<System.Data.Entity.Core.IEntityStateEntry>) source.GetStateEntries(this);
    }

    private void BackPropagateServerGen(
      List<KeyValuePair<PropagatorResult, object>> generatedValues)
    {
      foreach (KeyValuePair<PropagatorResult, object> generatedValue in generatedValues)
      {
        PropagatorResult owner;
        if (-1 == generatedValue.Key.Identifier || !this.KeyManager.TryGetIdentifierOwner(generatedValue.Key.Identifier, out owner))
          owner = generatedValue.Key;
        object obj = generatedValue.Value;
        if (owner.Identifier == -1)
        {
          owner.SetServerGenValue(obj);
        }
        else
        {
          foreach (int dependent in this.KeyManager.GetDependents(owner.Identifier))
          {
            if (this.KeyManager.TryGetIdentifierOwner(dependent, out owner))
              owner.SetServerGenValue(obj);
          }
        }
      }
    }

    private int AcceptChanges()
    {
      int num = 0;
      foreach (System.Data.Entity.Core.IEntityStateEntry stateEntry in this._stateEntries)
      {
        if (EntityState.Unchanged != stateEntry.State)
        {
          if (this._adapter.AcceptChangesDuringUpdate)
            stateEntry.AcceptChanges();
          ++num;
        }
      }
      return num;
    }

    private IEnumerable<EntitySetBase> GetDynamicModifiedExtents()
    {
      return (IEnumerable<EntitySetBase>) this._changes.Keys;
    }

    private IEnumerable<EntitySetBase> GetFunctionModifiedExtents()
    {
      return (IEnumerable<EntitySetBase>) this._functionChanges.Keys;
    }

    private IEnumerable<UpdateCommand> ProduceDynamicCommands()
    {
      UpdateCompiler updateCompiler = new UpdateCompiler(this);
      Set<EntitySet> tables = new Set<EntitySet>();
      foreach (EntitySetBase dynamicModifiedExtent in this.GetDynamicModifiedExtents())
      {
        Set<EntitySet> affectedTables = this.ViewLoader.GetAffectedTables(dynamicModifiedExtent, this.MetadataWorkspace);
        if (affectedTables.Count == 0)
          throw EntityUtil.Update(Strings.Update_MappingNotFound((object) dynamicModifiedExtent.Name), (Exception) null);
        foreach (EntitySet element in affectedTables)
          tables.Add(element);
      }
      foreach (EntitySet table in tables)
      {
        DbQueryCommandTree umView = this.Connection.GetMetadataWorkspace().GetCqtView((EntitySetBase) table);
        ChangeNode changeNode = Propagator.Propagate(this, table, umView);
        TableChangeProcessor change = new TableChangeProcessor(table);
        foreach (UpdateCommand compileCommand in change.CompileCommands(changeNode, updateCompiler))
          yield return compileCommand;
      }
    }

    internal DbCommandDefinition GenerateCommandDefinition(
      ModificationFunctionMapping functionMapping)
    {
      if (this._modificationFunctionCommandDefinitions == null)
        this._modificationFunctionCommandDefinitions = new Dictionary<ModificationFunctionMapping, DbCommandDefinition>();
      DbCommandDefinition commandDefinition;
      if (!this._modificationFunctionCommandDefinitions.TryGetValue(functionMapping, out commandDefinition))
      {
        TypeUsage resultType = (TypeUsage) null;
        if (functionMapping.ResultBindings != null && functionMapping.ResultBindings.Count > 0)
        {
          List<EdmProperty> edmPropertyList = new List<EdmProperty>(functionMapping.ResultBindings.Count);
          foreach (ModificationFunctionResultBinding resultBinding in functionMapping.ResultBindings)
            edmPropertyList.Add(new EdmProperty(resultBinding.ColumnName, resultBinding.Property.TypeUsage));
          resultType = TypeUsage.Create((EdmType) new CollectionType((EdmType) new RowType((IEnumerable<EdmProperty>) edmPropertyList)));
        }
        IEnumerable<KeyValuePair<string, TypeUsage>> parameters = functionMapping.Function.Parameters.Select<FunctionParameter, KeyValuePair<string, TypeUsage>>((Func<FunctionParameter, KeyValuePair<string, TypeUsage>>) (paramInfo => new KeyValuePair<string, TypeUsage>(paramInfo.Name, paramInfo.TypeUsage)));
        commandDefinition = this._providerServices.CreateCommandDefinition((DbCommandTree) new DbFunctionCommandTree(this.MetadataWorkspace, DataSpace.SSpace, functionMapping.Function, resultType, parameters), this._interceptionContext);
      }
      return commandDefinition;
    }

    private IEnumerable<UpdateCommand> ProduceFunctionCommands()
    {
      foreach (EntitySetBase functionModifiedExtent in this.GetFunctionModifiedExtents())
      {
        ModificationFunctionMappingTranslator translator = this.ViewLoader.GetFunctionMappingTranslator(functionModifiedExtent, this.MetadataWorkspace);
        if (translator != null)
        {
          foreach (ExtractedStateEntry functionModification in this.GetExtentFunctionModifications(functionModifiedExtent))
          {
            FunctionUpdateCommand command = translator.Translate(this, functionModification);
            if (command != null)
              yield return (UpdateCommand) command;
          }
        }
      }
    }

    internal ExtractorMetadata GetExtractorMetadata(
      EntitySetBase entitySetBase,
      StructuralType type)
    {
      Tuple<EntitySetBase, StructuralType> key = Tuple.Create<EntitySetBase, StructuralType>(entitySetBase, type);
      ExtractorMetadata extractorMetadata;
      if (!this._extractorMetadata.TryGetValue(key, out extractorMetadata))
      {
        extractorMetadata = new ExtractorMetadata(entitySetBase, type, this);
        this._extractorMetadata.Add(key, extractorMetadata);
      }
      return extractorMetadata;
    }

    private UpdateException DependencyOrderingError(
      IEnumerable<UpdateCommand> remainder)
    {
      HashSet<System.Data.Entity.Core.IEntityStateEntry> source = new HashSet<System.Data.Entity.Core.IEntityStateEntry>();
      foreach (UpdateCommand updateCommand in remainder)
        source.UnionWith((IEnumerable<System.Data.Entity.Core.IEntityStateEntry>) updateCommand.GetStateEntries(this));
      throw new UpdateException(Strings.Update_ConstraintCycle, (Exception) null, source.Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
    }

    internal DbCommand CreateCommand(DbModificationCommandTree commandTree)
    {
      try
      {
        return (DbCommand) new InterceptableDbCommand(this._providerServices.CreateCommand((DbCommandTree) commandTree, this._interceptionContext), this._interceptionContext, (DbDispatchers) null);
      }
      catch (Exception ex)
      {
        if (ex.RequiresContext())
          throw new EntityCommandCompilationException(Strings.EntityClient_CommandDefinitionPreparationFailed, ex);
        throw;
      }
    }

    internal void SetParameterValue(DbParameter parameter, TypeUsage typeUsage, object value)
    {
      this._providerServices.SetParameterValue(parameter, typeUsage, value);
    }

    private void PullModifiedEntriesFromStateManager()
    {
      foreach (System.Data.Entity.Core.IEntityStateEntry entityStateEntry in this._stateManager.GetEntityStateEntries(EntityState.Added))
      {
        if (!entityStateEntry.IsRelationship && !entityStateEntry.IsKeyEntry)
          this.KeyManager.RegisterKeyValueForAddedEntity(entityStateEntry);
      }
      foreach (System.Data.Entity.Core.IEntityStateEntry entityStateEntry in this._stateManager.GetEntityStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified))
        this.RegisterReferentialConstraints(entityStateEntry);
      foreach (System.Data.Entity.Core.IEntityStateEntry entityStateEntry in this._stateManager.GetEntityStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified))
        this.LoadStateEntry(entityStateEntry);
    }

    private void PullUnchangedEntriesFromStateManager()
    {
      foreach (KeyValuePair<EntityKey, AssociationSet> requiredEntity in this._requiredEntities)
      {
        EntityKey key = requiredEntity.Key;
        if (!this._knownEntityKeys.Contains(key))
        {
          System.Data.Entity.Core.IEntityStateEntry stateEntry;
          if (!this._stateManager.TryGetEntityStateEntry(key, out stateEntry) || stateEntry.IsKeyEntry)
            throw EntityUtil.Update(Strings.Update_MissingEntity((object) requiredEntity.Value.Name, (object) TypeHelpers.GetFullName(key.EntityContainerName, key.EntitySetName)), (Exception) null);
          this.LoadStateEntry(stateEntry);
        }
      }
      foreach (EntityKey optionalEntity in this._optionalEntities)
      {
        System.Data.Entity.Core.IEntityStateEntry stateEntry;
        if (!this._knownEntityKeys.Contains(optionalEntity) && this._stateManager.TryGetEntityStateEntry(optionalEntity, out stateEntry) && !stateEntry.IsKeyEntry)
          this.LoadStateEntry(stateEntry);
      }
      foreach (EntityKey includedValueEntity in this._includedValueEntities)
      {
        System.Data.Entity.Core.IEntityStateEntry stateEntry;
        if (!this._knownEntityKeys.Contains(includedValueEntity) && this._stateManager.TryGetEntityStateEntry(includedValueEntity, out stateEntry))
          this._recordConverter.ConvertCurrentValuesToPropagatorResult(stateEntry, ModifiedPropertiesBehavior.NoneModified);
      }
    }

    private void ValidateAndRegisterStateEntry(System.Data.Entity.Core.IEntityStateEntry stateEntry)
    {
      EntitySetBase entitySet = stateEntry.EntitySet;
      if (entitySet == null)
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.InvalidStateEntry, 1, (object) null);
      EntityKey entityKey = stateEntry.EntityKey;
      IExtendedDataRecord record = (IExtendedDataRecord) null;
      if (((EntityState.Unchanged | EntityState.Added | EntityState.Modified) & stateEntry.State) != (EntityState) 0)
      {
        record = (IExtendedDataRecord) stateEntry.CurrentValues;
        this.ValidateRecord(entitySet, record);
      }
      if (((EntityState.Unchanged | EntityState.Deleted | EntityState.Modified) & stateEntry.State) != (EntityState) 0)
      {
        record = (IExtendedDataRecord) stateEntry.OriginalValues;
        this.ValidateRecord(entitySet, record);
      }
      AssociationSet associationSet = entitySet as AssociationSet;
      if (associationSet != null)
      {
        AssociationSetMetadata associationSetMetadata = this.ViewLoader.GetAssociationSetMetadata(associationSet, this.MetadataWorkspace);
        if (associationSetMetadata.HasEnds)
        {
          foreach (FieldMetadata fieldMetadata in record.DataRecordInfo.FieldMetadata)
          {
            EntityKey key = (EntityKey) record.GetValue(fieldMetadata.Ordinal);
            AssociationEndMember fieldType = (AssociationEndMember) fieldMetadata.FieldType;
            if (associationSetMetadata.RequiredEnds.Contains(fieldType))
            {
              if (!this._requiredEntities.ContainsKey(key))
                this._requiredEntities.Add(key, associationSet);
            }
            else if (associationSetMetadata.OptionalEnds.Contains(fieldType))
              this.AddValidAncillaryKey(key, this._optionalEntities);
            else if (associationSetMetadata.IncludedValueEnds.Contains(fieldType))
              this.AddValidAncillaryKey(key, this._includedValueEntities);
          }
        }
        this._constraintValidator.RegisterAssociation(associationSet, record, stateEntry);
      }
      else
        this._constraintValidator.RegisterEntity(stateEntry);
      this._stateEntries.Add(stateEntry);
      if ((object) entityKey == null)
        return;
      this._knownEntityKeys.Add(entityKey);
    }

    private void AddValidAncillaryKey(EntityKey key, Set<EntityKey> keySet)
    {
      System.Data.Entity.Core.IEntityStateEntry stateEntry;
      if (!this._stateManager.TryGetEntityStateEntry(key, out stateEntry) || stateEntry.IsKeyEntry || stateEntry.State != EntityState.Unchanged)
        return;
      keySet.Add(key);
    }

    private void ValidateRecord(EntitySetBase extent, IExtendedDataRecord record)
    {
      DataRecordInfo dataRecordInfo;
      if (record == null || (dataRecordInfo = record.DataRecordInfo) == null || dataRecordInfo.RecordType == null)
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.InvalidStateEntry, 2, (object) null);
      UpdateTranslator.VerifyExtent(this.MetadataWorkspace, extent);
    }

    private static void VerifyExtent(MetadataWorkspace workspace, EntitySetBase extent)
    {
      EntityContainer entityContainer1 = extent.EntityContainer;
      EntityContainer entityContainer2 = (EntityContainer) null;
      if (entityContainer1 != null)
        workspace.TryGetEntityContainer(entityContainer1.Name, entityContainer1.DataSpace, out entityContainer2);
      if (entityContainer1 == null || entityContainer2 == null || !object.ReferenceEquals((object) entityContainer1, (object) entityContainer2))
        throw EntityUtil.Update(Strings.Update_WorkspaceMismatch, (Exception) null);
    }

    private void LoadStateEntry(System.Data.Entity.Core.IEntityStateEntry stateEntry)
    {
      this.ValidateAndRegisterStateEntry(stateEntry);
      ExtractedStateEntry extractedStateEntry = new ExtractedStateEntry(this, stateEntry);
      EntitySetBase entitySet = stateEntry.EntitySet;
      if (this.ViewLoader.GetFunctionMappingTranslator(entitySet, this.MetadataWorkspace) == null)
      {
        ChangeNode extentModifications = this.GetExtentModifications(entitySet);
        if (extractedStateEntry.Original != null)
          extentModifications.Deleted.Add(extractedStateEntry.Original);
        if (extractedStateEntry.Current == null)
          return;
        extentModifications.Inserted.Add(extractedStateEntry.Current);
      }
      else
        this.GetExtentFunctionModifications(entitySet).Add(extractedStateEntry);
    }

    internal ChangeNode GetExtentModifications(EntitySetBase extent)
    {
      ChangeNode changeNode;
      if (!this._changes.TryGetValue(extent, out changeNode))
      {
        changeNode = new ChangeNode(TypeUsage.Create((EdmType) extent.ElementType));
        this._changes.Add(extent, changeNode);
      }
      return changeNode;
    }

    internal List<ExtractedStateEntry> GetExtentFunctionModifications(
      EntitySetBase extent)
    {
      List<ExtractedStateEntry> extractedStateEntryList;
      if (!this._functionChanges.TryGetValue(extent, out extractedStateEntryList))
      {
        extractedStateEntryList = new List<ExtractedStateEntry>();
        this._functionChanges.Add(extent, extractedStateEntryList);
      }
      return extractedStateEntryList;
    }

    private class RelationshipConstraintValidator
    {
      private readonly Dictionary<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship, UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship> m_existingRelationships;
      private readonly Dictionary<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship, System.Data.Entity.Core.IEntityStateEntry> m_impliedRelationships;
      private readonly Dictionary<EntitySet, List<AssociationSet>> m_referencingRelationshipSets;

      internal RelationshipConstraintValidator()
      {
        this.m_existingRelationships = new Dictionary<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship, UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship>((IEqualityComparer<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship>) EqualityComparer<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship>.Default);
        this.m_impliedRelationships = new Dictionary<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship, System.Data.Entity.Core.IEntityStateEntry>((IEqualityComparer<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship>) EqualityComparer<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship>.Default);
        this.m_referencingRelationshipSets = new Dictionary<EntitySet, List<AssociationSet>>((IEqualityComparer<EntitySet>) EqualityComparer<EntitySet>.Default);
      }

      internal void RegisterEntity(System.Data.Entity.Core.IEntityStateEntry stateEntry)
      {
        if (EntityState.Added != stateEntry.State && EntityState.Deleted != stateEntry.State)
          return;
        EntityKey entityKey = stateEntry.EntityKey;
        EntitySet entitySet = (EntitySet) stateEntry.EntitySet;
        EntityType entityType = EntityState.Added == stateEntry.State ? UpdateTranslator.RelationshipConstraintValidator.GetEntityType((DbDataRecord) stateEntry.CurrentValues) : UpdateTranslator.RelationshipConstraintValidator.GetEntityType(stateEntry.OriginalValues);
        foreach (AssociationSet referencingAssocationSet in this.GetReferencingAssocationSets(entitySet))
        {
          ReadOnlyMetadataCollection<AssociationSetEnd> associationSetEnds = referencingAssocationSet.AssociationSetEnds;
          foreach (AssociationSetEnd associationSetEnd1 in associationSetEnds)
          {
            foreach (AssociationSetEnd associationSetEnd2 in associationSetEnds)
            {
              if (!object.ReferenceEquals((object) associationSetEnd2.CorrespondingAssociationEndMember, (object) associationSetEnd1.CorrespondingAssociationEndMember) && associationSetEnd2.EntitySet.EdmEquals((MetadataItem) entitySet) && (MetadataHelper.GetLowerBoundOfMultiplicity(associationSetEnd1.CorrespondingAssociationEndMember.RelationshipMultiplicity) != 0 && MetadataHelper.GetEntityTypeForEnd(associationSetEnd2.CorrespondingAssociationEndMember).IsAssignableFrom((EdmType) entityType)))
                this.m_impliedRelationships.Add(new UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship(entityKey, associationSetEnd1.CorrespondingAssociationEndMember, associationSetEnd2.CorrespondingAssociationEndMember, referencingAssocationSet, stateEntry), stateEntry);
            }
          }
        }
      }

      private static EntityType GetEntityType(DbDataRecord dbDataRecord)
      {
        return (EntityType) (dbDataRecord as IExtendedDataRecord).DataRecordInfo.RecordType.EdmType;
      }

      internal void RegisterAssociation(
        AssociationSet associationSet,
        IExtendedDataRecord record,
        System.Data.Entity.Core.IEntityStateEntry stateEntry)
      {
        Dictionary<string, EntityKey> dictionary = new Dictionary<string, EntityKey>((IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (FieldMetadata fieldMetadata in record.DataRecordInfo.FieldMetadata)
        {
          string name = fieldMetadata.FieldType.Name;
          EntityKey entityKey = (EntityKey) record.GetValue(fieldMetadata.Ordinal);
          dictionary.Add(name, entityKey);
        }
        ReadOnlyMetadataCollection<AssociationSetEnd> associationSetEnds = associationSet.AssociationSetEnds;
        foreach (AssociationSetEnd associationSetEnd1 in associationSetEnds)
        {
          foreach (AssociationSetEnd associationSetEnd2 in associationSetEnds)
          {
            if (!object.ReferenceEquals((object) associationSetEnd2.CorrespondingAssociationEndMember, (object) associationSetEnd1.CorrespondingAssociationEndMember))
              this.AddExistingRelationship(new UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship(dictionary[associationSetEnd2.CorrespondingAssociationEndMember.Name], associationSetEnd1.CorrespondingAssociationEndMember, associationSetEnd2.CorrespondingAssociationEndMember, associationSet, stateEntry));
          }
        }
      }

      internal void ValidateConstraints()
      {
        foreach (KeyValuePair<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship, System.Data.Entity.Core.IEntityStateEntry> impliedRelationship in this.m_impliedRelationships)
        {
          UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship key = impliedRelationship.Key;
          System.Data.Entity.Core.IEntityStateEntry stateEntry = impliedRelationship.Value;
          int actualCount = this.GetDirectionalRelationshipCountDelta(key);
          if (EntityState.Deleted == stateEntry.State)
            actualCount = -actualCount;
          int boundOfMultiplicity1 = MetadataHelper.GetLowerBoundOfMultiplicity(key.FromEnd.RelationshipMultiplicity);
          int? boundOfMultiplicity2 = MetadataHelper.GetUpperBoundOfMultiplicity(key.FromEnd.RelationshipMultiplicity);
          int num = boundOfMultiplicity2.HasValue ? boundOfMultiplicity2.Value : actualCount;
          if (actualCount < boundOfMultiplicity1 || actualCount > num)
            throw EntityUtil.UpdateRelationshipCardinalityConstraintViolation(key.AssociationSet.Name, boundOfMultiplicity1, boundOfMultiplicity2, TypeHelpers.GetFullName(key.ToEntityKey.EntityContainerName, key.ToEntityKey.EntitySetName), actualCount, key.FromEnd.Name, stateEntry);
        }
        foreach (UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship key in this.m_existingRelationships.Keys)
        {
          int addedCount;
          int deletedCount;
          key.GetCountsInEquivalenceSet(out addedCount, out deletedCount);
          int num = Math.Abs(addedCount - deletedCount);
          int boundOfMultiplicity1 = MetadataHelper.GetLowerBoundOfMultiplicity(key.FromEnd.RelationshipMultiplicity);
          int? boundOfMultiplicity2 = MetadataHelper.GetUpperBoundOfMultiplicity(key.FromEnd.RelationshipMultiplicity);
          if (boundOfMultiplicity2.HasValue)
          {
            EntityState? nullable1 = new EntityState?();
            int? nullable2 = new int?();
            if (addedCount > boundOfMultiplicity2.Value)
            {
              nullable1 = new EntityState?(EntityState.Added);
              nullable2 = new int?(addedCount);
            }
            else if (deletedCount > boundOfMultiplicity2.Value)
            {
              nullable1 = new EntityState?(EntityState.Deleted);
              nullable2 = new int?(deletedCount);
            }
            if (nullable1.HasValue)
              throw new UpdateException(Strings.Update_RelationshipCardinalityViolation((object) boundOfMultiplicity2.Value, (object) nullable1.Value, (object) key.AssociationSet.ElementType.FullName, (object) key.FromEnd.Name, (object) key.ToEnd.Name, (object) nullable2.Value), (Exception) null, key.GetEquivalenceSet().Select<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship, System.Data.Entity.Core.IEntityStateEntry>((Func<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship, System.Data.Entity.Core.IEntityStateEntry>) (reln => reln.StateEntry)).Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
          }
          if (1 == num && 1 == boundOfMultiplicity1)
          {
            int? nullable = boundOfMultiplicity2;
            if ((1 != nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
            {
              bool flag = addedCount > deletedCount;
              System.Data.Entity.Core.IEntityStateEntry entityStateEntry;
              if (!this.m_impliedRelationships.TryGetValue(key, out entityStateEntry) || flag && EntityState.Added != entityStateEntry.State || !flag && EntityState.Deleted != entityStateEntry.State)
                throw EntityUtil.Update(Strings.Update_MissingRequiredEntity((object) key.AssociationSet.Name, (object) key.StateEntry.State, (object) key.ToEnd.Name), (Exception) null, key.StateEntry);
            }
          }
        }
      }

      private int GetDirectionalRelationshipCountDelta(
        UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship expectedRelationship)
      {
        UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship directionalRelationship;
        if (!this.m_existingRelationships.TryGetValue(expectedRelationship, out directionalRelationship))
          return 0;
        int addedCount;
        int deletedCount;
        directionalRelationship.GetCountsInEquivalenceSet(out addedCount, out deletedCount);
        return addedCount - deletedCount;
      }

      private void AddExistingRelationship(
        UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship relationship)
      {
        UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship directionalRelationship;
        if (this.m_existingRelationships.TryGetValue(relationship, out directionalRelationship))
          directionalRelationship.AddToEquivalenceSet(relationship);
        else
          this.m_existingRelationships.Add(relationship, relationship);
      }

      private IEnumerable<AssociationSet> GetReferencingAssocationSets(
        EntitySet entitySet)
      {
        List<AssociationSet> associationSetList;
        if (!this.m_referencingRelationshipSets.TryGetValue(entitySet, out associationSetList))
        {
          associationSetList = new List<AssociationSet>();
          foreach (EntitySetBase baseEntitySet in entitySet.EntityContainer.BaseEntitySets)
          {
            AssociationSet associationSet = baseEntitySet as AssociationSet;
            if (associationSet != null && !associationSet.ElementType.IsForeignKey)
            {
              foreach (AssociationSetEnd associationSetEnd in associationSet.AssociationSetEnds)
              {
                if (associationSetEnd.EntitySet.Equals((object) entitySet))
                {
                  associationSetList.Add(associationSet);
                  break;
                }
              }
            }
          }
          this.m_referencingRelationshipSets.Add(entitySet, associationSetList);
        }
        return (IEnumerable<AssociationSet>) associationSetList;
      }

      private class DirectionalRelationship : IEquatable<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship>
      {
        internal readonly EntityKey ToEntityKey;
        internal readonly AssociationEndMember FromEnd;
        internal readonly AssociationEndMember ToEnd;
        internal readonly System.Data.Entity.Core.IEntityStateEntry StateEntry;
        internal readonly AssociationSet AssociationSet;
        private UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship _equivalenceSetLinkedListNext;
        private readonly int _hashCode;

        internal DirectionalRelationship(
          EntityKey toEntityKey,
          AssociationEndMember fromEnd,
          AssociationEndMember toEnd,
          AssociationSet associationSet,
          System.Data.Entity.Core.IEntityStateEntry stateEntry)
        {
          this.ToEntityKey = toEntityKey;
          this.FromEnd = fromEnd;
          this.ToEnd = toEnd;
          this.AssociationSet = associationSet;
          this.StateEntry = stateEntry;
          this._equivalenceSetLinkedListNext = this;
          this._hashCode = toEntityKey.GetHashCode() ^ fromEnd.GetHashCode() ^ toEnd.GetHashCode() ^ associationSet.GetHashCode();
        }

        internal void AddToEquivalenceSet(
          UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship other)
        {
          UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship setLinkedListNext = this._equivalenceSetLinkedListNext;
          this._equivalenceSetLinkedListNext = other;
          other._equivalenceSetLinkedListNext = setLinkedListNext;
        }

        internal IEnumerable<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship> GetEquivalenceSet()
        {
          UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship current = this;
          do
          {
            yield return current;
            current = current._equivalenceSetLinkedListNext;
          }
          while (!object.ReferenceEquals((object) current, (object) this));
        }

        internal void GetCountsInEquivalenceSet(out int addedCount, out int deletedCount)
        {
          addedCount = 0;
          deletedCount = 0;
          UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship directionalRelationship = this;
          do
          {
            if (directionalRelationship.StateEntry.State == EntityState.Added)
              ++addedCount;
            else if (directionalRelationship.StateEntry.State == EntityState.Deleted)
              ++deletedCount;
            directionalRelationship = directionalRelationship._equivalenceSetLinkedListNext;
          }
          while (!object.ReferenceEquals((object) directionalRelationship, (object) this));
        }

        public override int GetHashCode()
        {
          return this._hashCode;
        }

        public bool Equals(
          UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship other)
        {
          return object.ReferenceEquals((object) this, (object) other) || other != null && !(this.ToEntityKey != other.ToEntityKey) && (this.AssociationSet == other.AssociationSet && this.ToEnd == other.ToEnd) && this.FromEnd == other.FromEnd;
        }

        public override bool Equals(object obj)
        {
          return this.Equals(obj as UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship);
        }

        public override string ToString()
        {
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}-->{2}: {3}", (object) this.AssociationSet.Name, (object) this.FromEnd.Name, (object) this.ToEnd.Name, (object) StringUtil.BuildDelimitedList<EntityKeyMember>((IEnumerable<EntityKeyMember>) this.ToEntityKey.EntityKeyValues, (StringUtil.ToStringConverter<EntityKeyMember>) null, (string) null));
        }
      }
    }
  }
}
