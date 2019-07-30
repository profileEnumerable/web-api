// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.EdmModelDiffer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace System.Data.Entity.Migrations.Infrastructure
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class EdmModelDiffer
  {
    private static readonly PrimitiveTypeKind[] _validIdentityTypes = new PrimitiveTypeKind[6]
    {
      PrimitiveTypeKind.Byte,
      PrimitiveTypeKind.Decimal,
      PrimitiveTypeKind.Guid,
      PrimitiveTypeKind.Int16,
      PrimitiveTypeKind.Int32,
      PrimitiveTypeKind.Int64
    };
    private static readonly DynamicEqualityComparer<ForeignKeyOperation> _foreignKeyEqualityComparer = new DynamicEqualityComparer<ForeignKeyOperation>((Func<ForeignKeyOperation, ForeignKeyOperation, bool>) ((fk1, fk2) => fk1.Name.EqualsOrdinal(fk2.Name)));
    private static readonly DynamicEqualityComparer<IndexOperation> _indexEqualityComparer = new DynamicEqualityComparer<IndexOperation>((Func<IndexOperation, IndexOperation, bool>) ((i1, i2) =>
    {
      if (i1.Name.EqualsOrdinal(i2.Name))
        return i1.Table.EqualsOrdinal(i2.Table);
      return false;
    }));
    private EdmModelDiffer.ModelMetadata _source;
    private EdmModelDiffer.ModelMetadata _target;

    public ICollection<MigrationOperation> Diff(
      XDocument sourceModel,
      XDocument targetModel,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator = null,
      MigrationSqlGenerator migrationSqlGenerator = null,
      string sourceModelVersion = null,
      string targetModelVersion = null)
    {
      if (sourceModel == targetModel || XNode.DeepEquals((XNode) sourceModel, (XNode) targetModel))
        return (ICollection<MigrationOperation>) new MigrationOperation[0];
      DbProviderInfo providerInfo;
      StorageMappingItemCollection mappingItemCollection1 = sourceModel.GetStorageMappingItemCollection(out providerInfo);
      EdmModelDiffer.ModelMetadata source = new EdmModelDiffer.ModelMetadata()
      {
        EdmItemCollection = mappingItemCollection1.EdmItemCollection,
        StoreItemCollection = mappingItemCollection1.StoreItemCollection,
        StoreEntityContainer = mappingItemCollection1.StoreItemCollection.GetItems<EntityContainer>().Single<EntityContainer>(),
        EntityContainerMapping = mappingItemCollection1.GetItems<EntityContainerMapping>().Single<EntityContainerMapping>(),
        ProviderManifest = EdmModelDiffer.GetProviderManifest(providerInfo),
        ProviderInfo = providerInfo
      };
      StorageMappingItemCollection mappingItemCollection2 = targetModel.GetStorageMappingItemCollection(out providerInfo);
      EdmModelDiffer.ModelMetadata target = new EdmModelDiffer.ModelMetadata()
      {
        EdmItemCollection = mappingItemCollection2.EdmItemCollection,
        StoreItemCollection = mappingItemCollection2.StoreItemCollection,
        StoreEntityContainer = mappingItemCollection2.StoreItemCollection.GetItems<EntityContainer>().Single<EntityContainer>(),
        EntityContainerMapping = mappingItemCollection2.GetItems<EntityContainerMapping>().Single<EntityContainerMapping>(),
        ProviderManifest = EdmModelDiffer.GetProviderManifest(providerInfo),
        ProviderInfo = providerInfo
      };
      return this.Diff(source, target, modificationCommandTreeGenerator, migrationSqlGenerator, sourceModelVersion, targetModelVersion);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private ICollection<MigrationOperation> Diff(
      EdmModelDiffer.ModelMetadata source,
      EdmModelDiffer.ModelMetadata target,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator,
      string sourceModelVersion = null,
      string targetModelVersion = null)
    {
      this._source = source;
      this._target = target;
      List<Tuple<EntityType, EntityType>> list1 = this.FindEntityTypePairs().ToList<Tuple<EntityType, EntityType>>();
      List<Tuple<MappingFragment, MappingFragment>> list2 = this.FindMappingFragmentPairs((ICollection<Tuple<EntityType, EntityType>>) list1).ToList<Tuple<MappingFragment, MappingFragment>>();
      List<Tuple<AssociationType, AssociationType>> list3 = this.FindAssociationTypePairs((ICollection<Tuple<EntityType, EntityType>>) list1).ToList<Tuple<AssociationType, AssociationType>>();
      List<Tuple<EntitySet, EntitySet>> list4 = this.FindTablePairs((ICollection<Tuple<MappingFragment, MappingFragment>>) list2, (ICollection<Tuple<AssociationType, AssociationType>>) list3).ToList<Tuple<EntitySet, EntitySet>>();
      list3.AddRange(this.FindStoreOnlyAssociationTypePairs((ICollection<Tuple<AssociationType, AssociationType>>) list3, (ICollection<Tuple<EntitySet, EntitySet>>) list4));
      List<RenameTableOperation> list5 = EdmModelDiffer.FindRenamedTables((ICollection<Tuple<EntitySet, EntitySet>>) list4).ToList<RenameTableOperation>();
      List<RenameColumnOperation> list6 = this.FindRenamedColumns((ICollection<Tuple<MappingFragment, MappingFragment>>) list2, (ICollection<Tuple<AssociationType, AssociationType>>) list3).ToList<RenameColumnOperation>();
      List<AddColumnOperation> list7 = this.FindAddedColumns((ICollection<Tuple<EntitySet, EntitySet>>) list4, (ICollection<RenameColumnOperation>) list6).ToList<AddColumnOperation>();
      List<DropColumnOperation> list8 = this.FindDroppedColumns((ICollection<Tuple<EntitySet, EntitySet>>) list4, (ICollection<RenameColumnOperation>) list6).ToList<DropColumnOperation>();
      List<AlterColumnOperation> list9 = this.FindAlteredColumns((ICollection<Tuple<EntitySet, EntitySet>>) list4, (ICollection<RenameColumnOperation>) list6).ToList<AlterColumnOperation>();
      List<DropColumnOperation> list10 = this.FindOrphanedColumns((ICollection<Tuple<EntitySet, EntitySet>>) list4, (ICollection<RenameColumnOperation>) list6).ToList<DropColumnOperation>();
      List<MoveTableOperation> list11 = this.FindMovedTables((ICollection<Tuple<EntitySet, EntitySet>>) list4).ToList<MoveTableOperation>();
      List<CreateTableOperation> list12 = this.FindAddedTables((ICollection<Tuple<EntitySet, EntitySet>>) list4).ToList<CreateTableOperation>();
      List<DropTableOperation> list13 = this.FindDroppedTables((ICollection<Tuple<EntitySet, EntitySet>>) list4).ToList<DropTableOperation>();
      List<AlterTableOperation> list14 = this.FindAlteredTables((ICollection<Tuple<EntitySet, EntitySet>>) list4).ToList<AlterTableOperation>();
      List<MigrationOperation> list15 = this.FindAlteredPrimaryKeys((ICollection<Tuple<EntitySet, EntitySet>>) list4, (ICollection<RenameColumnOperation>) list6, (ICollection<AlterColumnOperation>) list9).ToList<MigrationOperation>();
      List<AddForeignKeyOperation> list16 = this.FindAddedForeignKeys((ICollection<Tuple<AssociationType, AssociationType>>) list3, (ICollection<RenameColumnOperation>) list6).Concat<AddForeignKeyOperation>(list15.OfType<AddForeignKeyOperation>()).ToList<AddForeignKeyOperation>();
      List<DropForeignKeyOperation> list17 = this.FindDroppedForeignKeys((ICollection<Tuple<AssociationType, AssociationType>>) list3, (ICollection<RenameColumnOperation>) list6).Concat<DropForeignKeyOperation>(list15.OfType<DropForeignKeyOperation>()).ToList<DropForeignKeyOperation>();
      List<CreateProcedureOperation> list18 = this.FindAddedModificationFunctions(modificationCommandTreeGenerator, migrationSqlGenerator).ToList<CreateProcedureOperation>();
      List<AlterProcedureOperation> list19 = this.FindAlteredModificationFunctions(modificationCommandTreeGenerator, migrationSqlGenerator).ToList<AlterProcedureOperation>();
      List<DropProcedureOperation> list20 = this.FindDroppedModificationFunctions().ToList<DropProcedureOperation>();
      List<RenameProcedureOperation> list21 = this.FindRenamedModificationFunctions().ToList<RenameProcedureOperation>();
      List<MoveProcedureOperation> list22 = this.FindMovedModificationFunctions().ToList<MoveProcedureOperation>();
      List<ConsolidatedIndex> list23 = (string.IsNullOrWhiteSpace(sourceModelVersion) || string.Compare(sourceModelVersion.Substring(0, 3), "6.1", StringComparison.Ordinal) >= 0 ? this.FindSourceIndexes((ICollection<Tuple<EntitySet, EntitySet>>) list4) : EdmModelDiffer.BuildLegacyIndexes(source)).ToList<ConsolidatedIndex>();
      List<ConsolidatedIndex> list24 = (string.IsNullOrWhiteSpace(targetModelVersion) || string.Compare(targetModelVersion.Substring(0, 3), "6.1", StringComparison.Ordinal) >= 0 ? this.FindTargetIndexes() : EdmModelDiffer.BuildLegacyIndexes(target)).ToList<ConsolidatedIndex>();
      List<CreateIndexOperation> list25 = EdmModelDiffer.FindAddedIndexes((ICollection<ConsolidatedIndex>) list23, (ICollection<ConsolidatedIndex>) list24, (ICollection<AlterColumnOperation>) list9, (ICollection<RenameColumnOperation>) list6).ToList<CreateIndexOperation>();
      List<DropIndexOperation> list26 = EdmModelDiffer.FindDroppedIndexes((ICollection<ConsolidatedIndex>) list23, (ICollection<ConsolidatedIndex>) list24, (ICollection<AlterColumnOperation>) list9, (ICollection<RenameColumnOperation>) list6).ToList<DropIndexOperation>();
      List<RenameIndexOperation> list27 = EdmModelDiffer.FindRenamedIndexes((ICollection<CreateIndexOperation>) list25, (ICollection<DropIndexOperation>) list26, (ICollection<AlterColumnOperation>) list9, (ICollection<RenameColumnOperation>) list6).ToList<RenameIndexOperation>();
      return (ICollection<MigrationOperation>) ((IEnumerable<MigrationOperation>) EdmModelDiffer.HandleTransitiveRenameDependencies((IList<RenameTableOperation>) list5)).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list11).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) ((IEnumerable<ForeignKeyOperation>) list17).Distinct<ForeignKeyOperation>((IEqualityComparer<ForeignKeyOperation>) EdmModelDiffer._foreignKeyEqualityComparer)).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) ((IEnumerable<IndexOperation>) list26).Distinct<IndexOperation>((IEqualityComparer<IndexOperation>) EdmModelDiffer._indexEqualityComparer)).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list10).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) EdmModelDiffer.HandleTransitiveRenameDependencies((IList<RenameColumnOperation>) list6)).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) EdmModelDiffer.HandleTransitiveRenameDependencies((IList<RenameIndexOperation>) list27)).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list15.OfType<DropPrimaryKeyOperation>()).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list12).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list14).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list7).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list9).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list15.OfType<AddPrimaryKeyOperation>()).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) ((IEnumerable<IndexOperation>) list25).Distinct<IndexOperation>((IEqualityComparer<IndexOperation>) EdmModelDiffer._indexEqualityComparer)).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) ((IEnumerable<ForeignKeyOperation>) list16).Distinct<ForeignKeyOperation>((IEqualityComparer<ForeignKeyOperation>) EdmModelDiffer._foreignKeyEqualityComparer)).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list8).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list13).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list18).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list22).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list21).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list19).Concat<MigrationOperation>((IEnumerable<MigrationOperation>) list20).ToList<MigrationOperation>();
    }

    private static IEnumerable<ConsolidatedIndex> BuildLegacyIndexes(
      EdmModelDiffer.ModelMetadata modelMetadata)
    {
      foreach (AssociationType associationType1 in modelMetadata.StoreItemCollection.GetItems<AssociationType>())
      {
        AssociationType associationType = associationType1;
        IEnumerable<string> dependentColumnNames = associationType.Constraint.ToProperties.Select<EdmProperty, string>((Func<EdmProperty, string>) (p => p.Name));
        string indexName = IndexOperation.BuildDefaultName(dependentColumnNames);
        string tableName = EdmModelDiffer.GetSchemaQualifiedName(modelMetadata.StoreEntityContainer.EntitySets.Single<EntitySet>((Func<EntitySet, bool>) (es => es.ElementType == associationType.Constraint.DependentEnd.GetEntityType())));
        ReadOnlyMetadataCollection<EdmProperty> dependentColumns = associationType.Constraint.ToProperties;
        ConsolidatedIndex consolidatedIndex;
        if (dependentColumns.Count > 0)
        {
          consolidatedIndex = new ConsolidatedIndex(tableName, dependentColumns[0].Name, new IndexAttribute(indexName, 0));
          for (int order = 1; order < dependentColumns.Count; ++order)
            consolidatedIndex.Add(dependentColumns[order].Name, new IndexAttribute(indexName, order));
        }
        else
          consolidatedIndex = new ConsolidatedIndex(tableName, new IndexAttribute(indexName));
        yield return consolidatedIndex;
      }
    }

    private IEnumerable<Tuple<EntityType, EntityType>> FindEntityTypePairs()
    {
      List<Tuple<EntityType, EntityType>> list1 = this._source.EdmItemCollection.GetItems<EntityType>().SelectMany((Func<EntityType, IEnumerable<EntityType>>) (et1 => (IEnumerable<EntityType>) this._target.EdmItemCollection.GetItems<EntityType>()), (et1, et2) => new
      {
        et1 = et1,
        et2 = et2
      }).Where(_param0 => _param0.et1.Name.EqualsOrdinal(_param0.et2.Name)).Select(_param0 => Tuple.Create<EntityType, EntityType>(_param0.et1, _param0.et2)).ToList<Tuple<EntityType, EntityType>>();
      List<EntityType> list2 = this._source.EdmItemCollection.GetItems<EntityType>().Except<EntityType>((IEnumerable<EntityType>) list1.Select<Tuple<EntityType, EntityType>, EntityType>((Func<Tuple<EntityType, EntityType>, EntityType>) (t => t.Item1)).ToList<EntityType>()).ToList<EntityType>();
      List<EntityType> targetRemainingEntities = this._target.EdmItemCollection.GetItems<EntityType>().Except<EntityType>((IEnumerable<EntityType>) list1.Select<Tuple<EntityType, EntityType>, EntityType>((Func<Tuple<EntityType, EntityType>, EntityType>) (t => t.Item2)).ToList<EntityType>()).ToList<EntityType>();
      return list1.Concat<Tuple<EntityType, EntityType>>(list2.SelectMany((Func<EntityType, IEnumerable<EntityType>>) (et1 => (IEnumerable<EntityType>) targetRemainingEntities), (et1, et2) => new
      {
        et1 = et1,
        et2 = et2
      }).Where(_param0 => EdmModelDiffer.FuzzyMatchEntities(_param0.et1, _param0.et2)).Select(_param0 => Tuple.Create<EntityType, EntityType>(_param0.et1, _param0.et2)));
    }

    private static bool FuzzyMatchEntities(EntityType entityType1, EntityType entityType2)
    {
      if (!entityType1.KeyMembers.SequenceEqual<EdmMember>((IEnumerable<EdmMember>) entityType2.KeyMembers, (IEqualityComparer<EdmMember>) new DynamicEqualityComparer<EdmMember>((Func<EdmMember, EdmMember, bool>) ((m1, m2) => m1.EdmEquals((MetadataItem) m2)))) || entityType1.BaseType != null && entityType2.BaseType == null || entityType1.BaseType == null && entityType2.BaseType != null)
        return false;
      return (double) entityType1.DeclaredMembers.SelectMany((Func<EdmMember, IEnumerable<EdmMember>>) (m1 => (IEnumerable<EdmMember>) entityType2.DeclaredMembers), (m1, m2) => new
      {
        m1 = m1,
        m2 = m2
      }).Where(_param0 => _param0.m1.EdmEquals((MetadataItem) _param0.m2)).Select(_param0 => 1).Count<int>() * 2.0 / (double) (entityType1.DeclaredMembers.Count + entityType2.DeclaredMembers.Count) > 0.8;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private IEnumerable<Tuple<MappingFragment, MappingFragment>> FindMappingFragmentPairs(
      ICollection<Tuple<EntityType, EntityType>> entityTypePairs)
    {
      List<EntityTypeMapping> targetEntityTypeMappings = this._target.EntityContainerMapping.EntitySetMappings.SelectMany<EntitySetMapping, EntityTypeMapping>((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (esm => (IEnumerable<EntityTypeMapping>) esm.EntityTypeMappings)).ToList<EntityTypeMapping>();
      foreach (EntityTypeMapping entityTypeMapping1 in this._source.EntityContainerMapping.EntitySetMappings.SelectMany<EntitySetMapping, EntityTypeMapping>((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (esm => (IEnumerable<EntityTypeMapping>) esm.EntityTypeMappings)))
      {
        EntityTypeMapping etm1 = entityTypeMapping1;
        foreach (EntityTypeMapping entityTypeMapping2 in targetEntityTypeMappings)
        {
          EntityTypeMapping etm2 = entityTypeMapping2;
          if (entityTypePairs.Any<Tuple<EntityType, EntityType>>((Func<Tuple<EntityType, EntityType>, bool>) (t =>
          {
            if (etm1.EntityType != null && etm2.EntityType != null && (t.Item1 == etm1.EntityType && t.Item2 == etm2.EntityType))
              return true;
            if (etm1.EntityType != null || etm2.EntityType != null || (!etm1.IsOfTypes.Contains((EntityTypeBase) t.Item1) || !etm2.IsOfTypes.Contains((EntityTypeBase) t.Item2)))
              return false;
            return etm1.IsOfTypes.Except<EntityTypeBase>((IEnumerable<EntityTypeBase>) new EntityType[1]
            {
              t.Item1
            }).Select<EntityTypeBase, string>((Func<EntityTypeBase, string>) (et => et.Name)).SequenceEqual<string>(etm2.IsOfTypes.Except<EntityTypeBase>((IEnumerable<EntityTypeBase>) new EntityType[1]
            {
              t.Item2
            }).Select<EntityTypeBase, string>((Func<EntityTypeBase, string>) (et => et.Name)));
          })))
          {
            using (IEnumerator<Tuple<MappingFragment, MappingFragment>> enumerator = etm1.MappingFragments.Zip<MappingFragment, MappingFragment, Tuple<MappingFragment, MappingFragment>>((IEnumerable<MappingFragment>) etm2.MappingFragments, new Func<MappingFragment, MappingFragment, Tuple<MappingFragment, MappingFragment>>(Tuple.Create<MappingFragment, MappingFragment>)).GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                Tuple<MappingFragment, MappingFragment> t = enumerator.Current;
                yield return t;
              }
              break;
            }
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private IEnumerable<Tuple<AssociationType, AssociationType>> FindAssociationTypePairs(
      ICollection<Tuple<EntityType, EntityType>> entityTypePairs)
    {
      List<Tuple<AssociationType, AssociationType>> list1 = entityTypePairs.SelectMany((Func<Tuple<EntityType, EntityType>, IEnumerable<NavigationProperty>>) (ets => (IEnumerable<NavigationProperty>) ets.Item1.NavigationProperties), (ets, np1) => new
      {
        ets = ets,
        np1 = np1
      }).SelectMany(_param0 => (IEnumerable<NavigationProperty>) _param0.ets.Item2.NavigationProperties, (_param0, np2) => new
      {
        \u003C\u003Eh__TransparentIdentifier4d = _param0,
        np2 = np2
      }).Where(_param0 => _param0.\u003C\u003Eh__TransparentIdentifier4d.np1.Name.EqualsIgnoreCase(_param0.np2.Name)).SelectMany(_param1 => this.GetStoreAssociationTypePairs(_param1.\u003C\u003Eh__TransparentIdentifier4d.np1.Association, _param1.np2.Association, entityTypePairs), (_param0, t) => t).Distinct<Tuple<AssociationType, AssociationType>>().ToList<Tuple<AssociationType, AssociationType>>();
      List<AssociationType> list2 = this._source.StoreItemCollection.GetItems<AssociationType>().Except<AssociationType>(list1.Select<Tuple<AssociationType, AssociationType>, AssociationType>((Func<Tuple<AssociationType, AssociationType>, AssociationType>) (t => t.Item1))).ToList<AssociationType>();
      List<AssociationType> targetRemainingAssociationTypes = this._target.StoreItemCollection.GetItems<AssociationType>().Except<AssociationType>(list1.Select<Tuple<AssociationType, AssociationType>, AssociationType>((Func<Tuple<AssociationType, AssociationType>, AssociationType>) (t => t.Item2))).ToList<AssociationType>();
      return list1.Concat<Tuple<AssociationType, AssociationType>>(list2.SelectMany((Func<AssociationType, IEnumerable<AssociationType>>) (at1 => (IEnumerable<AssociationType>) targetRemainingAssociationTypes), (at1, at2) => new
      {
        at1 = at1,
        at2 = at2
      }).Where(_param0 =>
      {
        if (_param0.at1.Name.EqualsIgnoreCase(_param0.at2.Name))
          return true;
        if (_param0.at1.Constraint != null && _param0.at2.Constraint != null && (_param0.at1.Constraint.PrincipalEnd.GetEntityType().EdmEquals((MetadataItem) _param0.at2.Constraint.PrincipalEnd.GetEntityType()) && _param0.at1.Constraint.DependentEnd.GetEntityType().EdmEquals((MetadataItem) _param0.at2.Constraint.DependentEnd.GetEntityType())))
          return ((IEnumerable<EdmMember>) _param0.at1.Constraint.ToProperties).SequenceEqual<EdmMember>((IEnumerable<EdmMember>) _param0.at2.Constraint.ToProperties, (IEqualityComparer<EdmMember>) new DynamicEqualityComparer<EdmMember>((Func<EdmMember, EdmMember, bool>) ((p1, p2) => p1.EdmEquals((MetadataItem) p2))));
        return false;
      }).Select(_param0 => Tuple.Create<AssociationType, AssociationType>(_param0.at1, _param0.at2)));
    }

    private IEnumerable<Tuple<AssociationType, AssociationType>> GetStoreAssociationTypePairs(
      AssociationType conceptualAssociationType1,
      AssociationType conceptualAssociationType2,
      ICollection<Tuple<EntityType, EntityType>> entityTypePairs)
    {
      AssociationType associationType1;
      AssociationType associationType2;
      if (this._source.StoreItemCollection.TryGetItem<AssociationType>(EdmModelDiffer.GetStoreAssociationIdentity(conceptualAssociationType1.Name), out associationType1) && this._target.StoreItemCollection.TryGetItem<AssociationType>(EdmModelDiffer.GetStoreAssociationIdentity(conceptualAssociationType2.Name), out associationType2))
      {
        yield return Tuple.Create<AssociationType, AssociationType>(associationType1, associationType2);
      }
      else
      {
        AssociationEndMember sourceEnd1 = conceptualAssociationType1.SourceEnd;
        Tuple<EntityType, EntityType> sourceEndEntityTypePair = entityTypePairs.Single<Tuple<EntityType, EntityType>>((Func<Tuple<EntityType, EntityType>, bool>) (t => t.Item1 == sourceEnd1.GetEntityType()));
        AssociationEndMember sourceEnd2 = conceptualAssociationType2.SourceEnd.GetEntityType() == sourceEndEntityTypePair.Item2 ? conceptualAssociationType2.SourceEnd : conceptualAssociationType2.TargetEnd;
        if (this._source.StoreItemCollection.TryGetItem<AssociationType>(EdmModelDiffer.GetStoreAssociationIdentity(sourceEnd1.Name), out associationType1) && this._target.StoreItemCollection.TryGetItem<AssociationType>(EdmModelDiffer.GetStoreAssociationIdentity(sourceEnd2.Name), out associationType2))
          yield return Tuple.Create<AssociationType, AssociationType>(associationType1, associationType2);
        AssociationEndMember targetEnd1 = conceptualAssociationType1.GetOtherEnd(sourceEnd1);
        AssociationEndMember targetEnd2 = conceptualAssociationType2.GetOtherEnd(sourceEnd2);
        if (this._source.StoreItemCollection.TryGetItem<AssociationType>(EdmModelDiffer.GetStoreAssociationIdentity(targetEnd1.Name), out associationType1) && this._target.StoreItemCollection.TryGetItem<AssociationType>(EdmModelDiffer.GetStoreAssociationIdentity(targetEnd2.Name), out associationType2))
          yield return Tuple.Create<AssociationType, AssociationType>(associationType1, associationType2);
      }
    }

    private IEnumerable<Tuple<AssociationType, AssociationType>> FindStoreOnlyAssociationTypePairs(
      ICollection<Tuple<AssociationType, AssociationType>> associationTypePairs,
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs)
    {
      List<AssociationType> list1 = this._source.StoreItemCollection.GetItems<AssociationType>().Except<AssociationType>(associationTypePairs.Select<Tuple<AssociationType, AssociationType>, AssociationType>((Func<Tuple<AssociationType, AssociationType>, AssociationType>) (t => t.Item1))).ToList<AssociationType>();
      List<AssociationType> list2 = this._target.StoreItemCollection.GetItems<AssociationType>().Except<AssociationType>(associationTypePairs.Select<Tuple<AssociationType, AssociationType>, AssociationType>((Func<Tuple<AssociationType, AssociationType>, AssociationType>) (t => t.Item2))).ToList<AssociationType>();
      List<Tuple<AssociationType, AssociationType>> tupleList = new List<Tuple<AssociationType, AssociationType>>();
      while (list1.Any<AssociationType>())
      {
        AssociationType associationType1 = list1[0];
        for (int index = 0; index < list2.Count; ++index)
        {
          AssociationType associationType2 = list2[index];
          if (tablePairs.Any<Tuple<EntitySet, EntitySet>>((Func<Tuple<EntitySet, EntitySet>, bool>) (t =>
          {
            if (t.Item1.ElementType == associationType1.Constraint.PrincipalEnd.GetEntityType())
              return t.Item2.ElementType == associationType2.Constraint.PrincipalEnd.GetEntityType();
            return false;
          })) && tablePairs.Any<Tuple<EntitySet, EntitySet>>((Func<Tuple<EntitySet, EntitySet>, bool>) (t =>
          {
            if (t.Item1.ElementType == associationType1.Constraint.DependentEnd.GetEntityType())
              return t.Item2.ElementType == associationType2.Constraint.DependentEnd.GetEntityType();
            return false;
          })))
          {
            tupleList.Add(Tuple.Create<AssociationType, AssociationType>(associationType1, associationType2));
            list2.RemoveAt(index);
            break;
          }
        }
        list1.RemoveAt(0);
      }
      return (IEnumerable<Tuple<AssociationType, AssociationType>>) tupleList;
    }

    private static string GetStoreAssociationIdentity(string associationName)
    {
      return "CodeFirstDatabaseSchema." + associationName;
    }

    private IEnumerable<Tuple<EntitySet, EntitySet>> FindTablePairs(
      ICollection<Tuple<MappingFragment, MappingFragment>> mappingFragmentPairs,
      ICollection<Tuple<AssociationType, AssociationType>> associationTypePairs)
    {
      HashSet<EntitySet> sourceTables = new HashSet<EntitySet>();
      HashSet<EntitySet> targetTables = new HashSet<EntitySet>();
      foreach (Tuple<MappingFragment, MappingFragment> mappingFragmentPair in (IEnumerable<Tuple<MappingFragment, MappingFragment>>) mappingFragmentPairs)
      {
        EntitySet sourceTable = mappingFragmentPair.Item1.TableSet;
        EntitySet targetTable = mappingFragmentPair.Item2.TableSet;
        if (!sourceTables.Contains(sourceTable) && !targetTables.Contains(targetTable))
        {
          sourceTables.Add(sourceTable);
          targetTables.Add(targetTable);
          yield return Tuple.Create<EntitySet, EntitySet>(sourceTable, targetTable);
        }
      }
      foreach (Tuple<AssociationType, AssociationType> associationTypePair1 in (IEnumerable<Tuple<AssociationType, AssociationType>>) associationTypePairs)
      {
        Tuple<AssociationType, AssociationType> associationTypePair = associationTypePair1;
        EntitySet sourceTable = this._source.StoreEntityContainer.EntitySets.Single<EntitySet>((Func<EntitySet, bool>) (es => es.ElementType == associationTypePair.Item1.Constraint.DependentEnd.GetEntityType()));
        EntitySet targetTable = this._target.StoreEntityContainer.EntitySets.Single<EntitySet>((Func<EntitySet, bool>) (es => es.ElementType == associationTypePair.Item2.Constraint.DependentEnd.GetEntityType()));
        if (!sourceTables.Contains(sourceTable) && !targetTables.Contains(targetTable))
        {
          sourceTables.Add(sourceTable);
          targetTables.Add(targetTable);
          yield return Tuple.Create<EntitySet, EntitySet>(sourceTable, targetTable);
        }
      }
    }

    private static IEnumerable<RenameTableOperation> HandleTransitiveRenameDependencies(
      IList<RenameTableOperation> renameTableOperations)
    {
      return EdmModelDiffer.HandleTransitiveRenameDependencies<RenameTableOperation>(renameTableOperations, (Func<RenameTableOperation, RenameTableOperation, bool>) ((rt1, rt2) =>
      {
        DatabaseName databaseName1 = DatabaseName.Parse(rt1.Name);
        DatabaseName databaseName2 = DatabaseName.Parse(rt2.Name);
        if (databaseName1.Name.EqualsIgnoreCase(rt2.NewName))
          return databaseName1.Schema.EqualsIgnoreCase(databaseName2.Schema);
        return false;
      }), (Func<string, RenameTableOperation, RenameTableOperation>) ((t, rt) => new RenameTableOperation(t, rt.NewName, (object) null)), (Action<RenameTableOperation, string>) ((rt, t) => rt.NewName = t));
    }

    private static IEnumerable<RenameColumnOperation> HandleTransitiveRenameDependencies(
      IList<RenameColumnOperation> renameColumnOperations)
    {
      return EdmModelDiffer.HandleTransitiveRenameDependencies<RenameColumnOperation>(renameColumnOperations, (Func<RenameColumnOperation, RenameColumnOperation, bool>) ((rc1, rc2) =>
      {
        if (rc1.Table.EqualsIgnoreCase(rc2.Table))
          return rc1.Name.EqualsIgnoreCase(rc2.NewName);
        return false;
      }), (Func<string, RenameColumnOperation, RenameColumnOperation>) ((c, rc) => new RenameColumnOperation(rc.Table, c, rc.NewName, (object) null)), (Action<RenameColumnOperation, string>) ((rc, c) => rc.NewName = c));
    }

    private static IEnumerable<RenameIndexOperation> HandleTransitiveRenameDependencies(
      IList<RenameIndexOperation> renameIndexOperations)
    {
      return EdmModelDiffer.HandleTransitiveRenameDependencies<RenameIndexOperation>(renameIndexOperations, (Func<RenameIndexOperation, RenameIndexOperation, bool>) ((ri1, ri2) =>
      {
        if (ri1.Table.EqualsIgnoreCase(ri2.Table))
          return ri1.Name.EqualsIgnoreCase(ri2.NewName);
        return false;
      }), (Func<string, RenameIndexOperation, RenameIndexOperation>) ((i, rc) => new RenameIndexOperation(rc.Table, i, rc.NewName, (object) null)), (Action<RenameIndexOperation, string>) ((rc, i) => rc.NewName = i));
    }

    private static IEnumerable<T> HandleTransitiveRenameDependencies<T>(
      IList<T> renameOperations,
      Func<T, T, bool> dependencyFinder,
      Func<string, T, T> renameCreator,
      Action<T, string> setNewName)
      where T : class
    {
      int tempCounter = 0;
      List<T> tempRenames = new List<T>();
      for (int i = 0; i < ((ICollection<T>) renameOperations).Count; ++i)
      {
        T renameOperation = renameOperations[i];
        T dependentRename = ((IEnumerable<T>) renameOperations).Skip<T>(i + 1).SingleOrDefault<T>((Func<T, bool>) (rt => dependencyFinder(renameOperation, rt)));
        if ((object) dependentRename != null)
        {
          string str = "__mig_tmp__" + (object) tempCounter++;
          tempRenames.Add(renameCreator(str, renameOperation));
          setNewName(renameOperation, str);
        }
        yield return renameOperation;
      }
      foreach (T obj in tempRenames)
        yield return obj;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private IEnumerable<MoveProcedureOperation> FindMovedModificationFunctions()
    {
      return this._source.EntityContainerMapping.EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm1 => (IEnumerable<EntityTypeModificationFunctionMapping>) esm1.ModificationFunctionMappings), (esm1, mfm1) => new
      {
        esm1 = esm1,
        mfm1 = mfm1
      }).SelectMany(_param1 => this._target.EntityContainerMapping.EntitySetMappings, (_param0, esm2) => new
      {
        \u003C\u003Eh__TransparentIdentifierb8 = _param0,
        esm2 = esm2
      }).SelectMany(_param0 => (IEnumerable<EntityTypeModificationFunctionMapping>) _param0.esm2.ModificationFunctionMappings, (_param0, mfm2) => new
      {
        \u003C\u003Eh__TransparentIdentifierb9 = _param0,
        mfm2 = mfm2
      }).Where(_param0 => _param0.\u003C\u003Eh__TransparentIdentifierb9.\u003C\u003Eh__TransparentIdentifierb8.mfm1.EntityType.Identity == _param0.mfm2.EntityType.Identity).SelectMany(_param0 => EdmModelDiffer.DiffModificationFunctionSchemas(_param0.\u003C\u003Eh__TransparentIdentifierb9.\u003C\u003Eh__TransparentIdentifierb8.mfm1, _param0.mfm2), (_param0, o) => o).Concat<MoveProcedureOperation>(this._source.EntityContainerMapping.AssociationSetMappings.Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm1 => asm1.ModificationFunctionMapping != null)).SelectMany((Func<AssociationSetMapping, IEnumerable<AssociationSetMapping>>) (asm1 => this._target.EntityContainerMapping.AssociationSetMappings), (asm1, asm2) => new
      {
        asm1 = asm1,
        asm2 = asm2
      }).Where(_param0 =>
      {
        if (_param0.asm2.ModificationFunctionMapping != null)
          return _param0.asm1.ModificationFunctionMapping.AssociationSet.Identity == _param0.asm2.ModificationFunctionMapping.AssociationSet.Identity;
        return false;
      }).SelectMany(_param0 => EdmModelDiffer.DiffModificationFunctionSchemas(_param0.asm1.ModificationFunctionMapping, _param0.asm2.ModificationFunctionMapping), (_param0, o) => o));
    }

    private static IEnumerable<MoveProcedureOperation> DiffModificationFunctionSchemas(
      EntityTypeModificationFunctionMapping sourceModificationFunctionMapping,
      EntityTypeModificationFunctionMapping targetModificationFunctionMapping)
    {
      if (!sourceModificationFunctionMapping.InsertFunctionMapping.Function.Schema.EqualsOrdinal(targetModificationFunctionMapping.InsertFunctionMapping.Function.Schema))
        yield return new MoveProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.InsertFunctionMapping.Function), targetModificationFunctionMapping.InsertFunctionMapping.Function.Schema, (object) null);
      if (!sourceModificationFunctionMapping.UpdateFunctionMapping.Function.Schema.EqualsOrdinal(targetModificationFunctionMapping.UpdateFunctionMapping.Function.Schema))
        yield return new MoveProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.UpdateFunctionMapping.Function), targetModificationFunctionMapping.UpdateFunctionMapping.Function.Schema, (object) null);
      if (!sourceModificationFunctionMapping.DeleteFunctionMapping.Function.Schema.EqualsOrdinal(targetModificationFunctionMapping.DeleteFunctionMapping.Function.Schema))
        yield return new MoveProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.DeleteFunctionMapping.Function), targetModificationFunctionMapping.DeleteFunctionMapping.Function.Schema, (object) null);
    }

    private static IEnumerable<MoveProcedureOperation> DiffModificationFunctionSchemas(
      AssociationSetModificationFunctionMapping sourceModificationFunctionMapping,
      AssociationSetModificationFunctionMapping targetModificationFunctionMapping)
    {
      if (!sourceModificationFunctionMapping.InsertFunctionMapping.Function.Schema.EqualsOrdinal(targetModificationFunctionMapping.InsertFunctionMapping.Function.Schema))
        yield return new MoveProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.InsertFunctionMapping.Function), targetModificationFunctionMapping.InsertFunctionMapping.Function.Schema, (object) null);
      if (!sourceModificationFunctionMapping.DeleteFunctionMapping.Function.Schema.EqualsOrdinal(targetModificationFunctionMapping.DeleteFunctionMapping.Function.Schema))
        yield return new MoveProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.DeleteFunctionMapping.Function), targetModificationFunctionMapping.DeleteFunctionMapping.Function.Schema, (object) null);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "<>h__TransparentIdentifier0")]
    private IEnumerable<CreateProcedureOperation> FindAddedModificationFunctions(
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      return this._target.EntityContainerMapping.EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm1 => (IEnumerable<EntityTypeModificationFunctionMapping>) esm1.ModificationFunctionMappings), (esm1, mfm1) => new
      {
        esm1 = esm1,
        mfm1 = mfm1
      }).Where(_param1 => !this._source.EntityContainerMapping.EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm2 => (IEnumerable<EntityTypeModificationFunctionMapping>) esm2.ModificationFunctionMappings), (esm2, mfm2) => new
      {
        esm2 = esm2,
        mfm2 = mfm2
      }).Where(_param1 => _param1.mfm1.EntityType.Identity == _param1.mfm2.EntityType.Identity).Select(_param0 => _param0.mfm2).Any<EntityTypeModificationFunctionMapping>()).SelectMany(_param1 => this.BuildCreateProcedureOperations(_param1.mfm1, modificationCommandTreeGenerator, migrationSqlGenerator), (_param0, o) => o).Concat<CreateProcedureOperation>(this._target.EntityContainerMapping.AssociationSetMappings.Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm1 => asm1.ModificationFunctionMapping != null)).Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm1 => !this._source.EntityContainerMapping.AssociationSetMappings.Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm2 =>
      {
        if (asm2.ModificationFunctionMapping != null)
          return asm1.ModificationFunctionMapping.AssociationSet.Identity == asm2.ModificationFunctionMapping.AssociationSet.Identity;
        return false;
      })).Select<AssociationSetMapping, AssociationSetModificationFunctionMapping>((Func<AssociationSetMapping, AssociationSetModificationFunctionMapping>) (asm2 => asm2.ModificationFunctionMapping)).Any<AssociationSetModificationFunctionMapping>())).SelectMany<AssociationSetMapping, CreateProcedureOperation, CreateProcedureOperation>((Func<AssociationSetMapping, IEnumerable<CreateProcedureOperation>>) (asm1 => this.BuildCreateProcedureOperations(asm1.ModificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator)), (Func<AssociationSetMapping, CreateProcedureOperation, CreateProcedureOperation>) ((asm1, o) => o)));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private IEnumerable<RenameProcedureOperation> FindRenamedModificationFunctions()
    {
      return this._source.EntityContainerMapping.EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm1 => (IEnumerable<EntityTypeModificationFunctionMapping>) esm1.ModificationFunctionMappings), (esm1, mfm1) => new
      {
        esm1 = esm1,
        mfm1 = mfm1
      }).SelectMany(_param1 => this._target.EntityContainerMapping.EntitySetMappings, (_param0, esm2) => new
      {
        \u003C\u003Eh__TransparentIdentifier103 = _param0,
        esm2 = esm2
      }).SelectMany(_param0 => (IEnumerable<EntityTypeModificationFunctionMapping>) _param0.esm2.ModificationFunctionMappings, (_param0, mfm2) => new
      {
        \u003C\u003Eh__TransparentIdentifier104 = _param0,
        mfm2 = mfm2
      }).Where(_param0 => _param0.\u003C\u003Eh__TransparentIdentifier104.\u003C\u003Eh__TransparentIdentifier103.mfm1.EntityType.Identity == _param0.mfm2.EntityType.Identity).SelectMany(_param0 => EdmModelDiffer.DiffModificationFunctionNames(_param0.\u003C\u003Eh__TransparentIdentifier104.\u003C\u003Eh__TransparentIdentifier103.mfm1, _param0.mfm2), (_param0, o) => o).Concat<RenameProcedureOperation>(this._source.EntityContainerMapping.AssociationSetMappings.Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm1 => asm1.ModificationFunctionMapping != null)).SelectMany((Func<AssociationSetMapping, IEnumerable<AssociationSetMapping>>) (asm1 => this._target.EntityContainerMapping.AssociationSetMappings), (asm1, asm2) => new
      {
        asm1 = asm1,
        asm2 = asm2
      }).Where(_param0 =>
      {
        if (_param0.asm2.ModificationFunctionMapping != null)
          return _param0.asm1.ModificationFunctionMapping.AssociationSet.Identity == _param0.asm2.ModificationFunctionMapping.AssociationSet.Identity;
        return false;
      }).SelectMany(_param0 => EdmModelDiffer.DiffModificationFunctionNames(_param0.asm1.ModificationFunctionMapping, _param0.asm2.ModificationFunctionMapping), (_param0, o) => o));
    }

    private static IEnumerable<RenameProcedureOperation> DiffModificationFunctionNames(
      AssociationSetModificationFunctionMapping sourceModificationFunctionMapping,
      AssociationSetModificationFunctionMapping targetModificationFunctionMapping)
    {
      if (!sourceModificationFunctionMapping.InsertFunctionMapping.Function.FunctionName.EqualsOrdinal(targetModificationFunctionMapping.InsertFunctionMapping.Function.FunctionName))
        yield return new RenameProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.InsertFunctionMapping.Function.FunctionName, targetModificationFunctionMapping.InsertFunctionMapping.Function.Schema), targetModificationFunctionMapping.InsertFunctionMapping.Function.FunctionName, (object) null);
      if (!sourceModificationFunctionMapping.DeleteFunctionMapping.Function.FunctionName.EqualsOrdinal(targetModificationFunctionMapping.DeleteFunctionMapping.Function.FunctionName))
        yield return new RenameProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.DeleteFunctionMapping.Function.FunctionName, targetModificationFunctionMapping.DeleteFunctionMapping.Function.Schema), targetModificationFunctionMapping.DeleteFunctionMapping.Function.FunctionName, (object) null);
    }

    private static IEnumerable<RenameProcedureOperation> DiffModificationFunctionNames(
      EntityTypeModificationFunctionMapping sourceModificationFunctionMapping,
      EntityTypeModificationFunctionMapping targetModificationFunctionMapping)
    {
      if (!sourceModificationFunctionMapping.InsertFunctionMapping.Function.FunctionName.EqualsOrdinal(targetModificationFunctionMapping.InsertFunctionMapping.Function.FunctionName))
        yield return new RenameProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.InsertFunctionMapping.Function.FunctionName, targetModificationFunctionMapping.InsertFunctionMapping.Function.Schema), targetModificationFunctionMapping.InsertFunctionMapping.Function.FunctionName, (object) null);
      if (!sourceModificationFunctionMapping.UpdateFunctionMapping.Function.FunctionName.EqualsOrdinal(targetModificationFunctionMapping.UpdateFunctionMapping.Function.FunctionName))
        yield return new RenameProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.UpdateFunctionMapping.Function.FunctionName, targetModificationFunctionMapping.UpdateFunctionMapping.Function.Schema), targetModificationFunctionMapping.UpdateFunctionMapping.Function.FunctionName, (object) null);
      if (!sourceModificationFunctionMapping.DeleteFunctionMapping.Function.FunctionName.EqualsOrdinal(targetModificationFunctionMapping.DeleteFunctionMapping.Function.FunctionName))
        yield return new RenameProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(sourceModificationFunctionMapping.DeleteFunctionMapping.Function.FunctionName, targetModificationFunctionMapping.DeleteFunctionMapping.Function.Schema), targetModificationFunctionMapping.DeleteFunctionMapping.Function.FunctionName, (object) null);
    }

    private static string GetSchemaQualifiedName(string table, string schema)
    {
      return new DatabaseName(table, schema).ToString();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private IEnumerable<AlterProcedureOperation> FindAlteredModificationFunctions(
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      return this._source.EntityContainerMapping.EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm1 => (IEnumerable<EntityTypeModificationFunctionMapping>) esm1.ModificationFunctionMappings), (esm1, mfm1) => new
      {
        esm1 = esm1,
        mfm1 = mfm1
      }).SelectMany(_param1 => this._target.EntityContainerMapping.EntitySetMappings, (_param0, esm2) => new
      {
        \u003C\u003Eh__TransparentIdentifier12b = _param0,
        esm2 = esm2
      }).SelectMany(_param0 => (IEnumerable<EntityTypeModificationFunctionMapping>) _param0.esm2.ModificationFunctionMappings, (_param0, mfm2) => new
      {
        \u003C\u003Eh__TransparentIdentifier12c = _param0,
        mfm2 = mfm2
      }).Where(_param0 => _param0.\u003C\u003Eh__TransparentIdentifier12c.\u003C\u003Eh__TransparentIdentifier12b.mfm1.EntityType.Identity == _param0.mfm2.EntityType.Identity).SelectMany(_param1 => this.DiffModificationFunctions(_param1.\u003C\u003Eh__TransparentIdentifier12c.\u003C\u003Eh__TransparentIdentifier12b.mfm1, _param1.mfm2, modificationCommandTreeGenerator, migrationSqlGenerator), (_param0, o) => o).Concat<AlterProcedureOperation>(this._source.EntityContainerMapping.AssociationSetMappings.Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm1 => asm1.ModificationFunctionMapping != null)).SelectMany((Func<AssociationSetMapping, IEnumerable<AssociationSetMapping>>) (asm1 => this._target.EntityContainerMapping.AssociationSetMappings), (asm1, asm2) => new
      {
        asm1 = asm1,
        asm2 = asm2
      }).Where(_param0 =>
      {
        if (_param0.asm2.ModificationFunctionMapping != null)
          return _param0.asm1.ModificationFunctionMapping.AssociationSet.Identity == _param0.asm2.ModificationFunctionMapping.AssociationSet.Identity;
        return false;
      }).SelectMany(_param1 => this.DiffModificationFunctions(_param1.asm1.ModificationFunctionMapping, _param1.asm2.ModificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator), (_param0, o) => o));
    }

    private IEnumerable<AlterProcedureOperation> DiffModificationFunctions(
      AssociationSetModificationFunctionMapping sourceModificationFunctionMapping,
      AssociationSetModificationFunctionMapping targetModificationFunctionMapping,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      if (!this.DiffModificationFunction(sourceModificationFunctionMapping.InsertFunctionMapping, targetModificationFunctionMapping.InsertFunctionMapping))
        yield return this.BuildAlterProcedureOperation(targetModificationFunctionMapping.InsertFunctionMapping.Function, this.GenerateInsertFunctionBody(targetModificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
      if (!this.DiffModificationFunction(sourceModificationFunctionMapping.DeleteFunctionMapping, targetModificationFunctionMapping.DeleteFunctionMapping))
        yield return this.BuildAlterProcedureOperation(targetModificationFunctionMapping.DeleteFunctionMapping.Function, this.GenerateDeleteFunctionBody(targetModificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
    }

    private IEnumerable<AlterProcedureOperation> DiffModificationFunctions(
      EntityTypeModificationFunctionMapping sourceModificationFunctionMapping,
      EntityTypeModificationFunctionMapping targetModificationFunctionMapping,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      if (!this.DiffModificationFunction(sourceModificationFunctionMapping.InsertFunctionMapping, targetModificationFunctionMapping.InsertFunctionMapping))
        yield return this.BuildAlterProcedureOperation(targetModificationFunctionMapping.InsertFunctionMapping.Function, this.GenerateInsertFunctionBody(targetModificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
      if (!this.DiffModificationFunction(sourceModificationFunctionMapping.UpdateFunctionMapping, targetModificationFunctionMapping.UpdateFunctionMapping))
        yield return this.BuildAlterProcedureOperation(targetModificationFunctionMapping.UpdateFunctionMapping.Function, this.GenerateUpdateFunctionBody(targetModificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
      if (!this.DiffModificationFunction(sourceModificationFunctionMapping.DeleteFunctionMapping, targetModificationFunctionMapping.DeleteFunctionMapping))
        yield return this.BuildAlterProcedureOperation(targetModificationFunctionMapping.DeleteFunctionMapping.Function, this.GenerateDeleteFunctionBody(targetModificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
    }

    private string GenerateInsertFunctionBody(
      EntityTypeModificationFunctionMapping modificationFunctionMapping,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      return this.GenerateFunctionBody<DbModificationCommandTree>(modificationFunctionMapping, (Func<ModificationCommandTreeGenerator, string, IEnumerable<DbModificationCommandTree>>) ((m, s) => m.GenerateInsert(s)), modificationCommandTreeGenerator, migrationSqlGenerator, modificationFunctionMapping.InsertFunctionMapping.Function.FunctionName, (string) null);
    }

    private string GenerateInsertFunctionBody(
      AssociationSetModificationFunctionMapping modificationFunctionMapping,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      return this.GenerateFunctionBody<DbInsertCommandTree>(modificationFunctionMapping, (Func<ModificationCommandTreeGenerator, string, IEnumerable<DbInsertCommandTree>>) ((m, s) => m.GenerateAssociationInsert(s)), modificationCommandTreeGenerator, migrationSqlGenerator, (string) null);
    }

    private string GenerateUpdateFunctionBody(
      EntityTypeModificationFunctionMapping modificationFunctionMapping,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      return this.GenerateFunctionBody<DbModificationCommandTree>(modificationFunctionMapping, (Func<ModificationCommandTreeGenerator, string, IEnumerable<DbModificationCommandTree>>) ((m, s) => m.GenerateUpdate(s)), modificationCommandTreeGenerator, migrationSqlGenerator, modificationFunctionMapping.UpdateFunctionMapping.Function.FunctionName, modificationFunctionMapping.UpdateFunctionMapping.RowsAffectedParameterName);
    }

    private string GenerateDeleteFunctionBody(
      EntityTypeModificationFunctionMapping modificationFunctionMapping,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      return this.GenerateFunctionBody<DbModificationCommandTree>(modificationFunctionMapping, (Func<ModificationCommandTreeGenerator, string, IEnumerable<DbModificationCommandTree>>) ((m, s) => m.GenerateDelete(s)), modificationCommandTreeGenerator, migrationSqlGenerator, modificationFunctionMapping.DeleteFunctionMapping.Function.FunctionName, modificationFunctionMapping.DeleteFunctionMapping.RowsAffectedParameterName);
    }

    private string GenerateDeleteFunctionBody(
      AssociationSetModificationFunctionMapping modificationFunctionMapping,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      return this.GenerateFunctionBody<DbDeleteCommandTree>(modificationFunctionMapping, (Func<ModificationCommandTreeGenerator, string, IEnumerable<DbDeleteCommandTree>>) ((m, s) => m.GenerateAssociationDelete(s)), modificationCommandTreeGenerator, migrationSqlGenerator, modificationFunctionMapping.DeleteFunctionMapping.RowsAffectedParameterName);
    }

    private string GenerateFunctionBody<TCommandTree>(
      EntityTypeModificationFunctionMapping modificationFunctionMapping,
      Func<ModificationCommandTreeGenerator, string, IEnumerable<TCommandTree>> treeGenerator,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator,
      string functionName,
      string rowsAffectedParameterName)
      where TCommandTree : DbModificationCommandTree
    {
      TCommandTree[] commandTrees = new TCommandTree[0];
      if (modificationCommandTreeGenerator != null)
      {
        DynamicToFunctionModificationCommandConverter commandConverter = new DynamicToFunctionModificationCommandConverter(modificationFunctionMapping, this._target.EntityContainerMapping);
        try
        {
          commandTrees = commandConverter.Convert<TCommandTree>(treeGenerator(modificationCommandTreeGenerator.Value, modificationFunctionMapping.EntityType.Identity)).ToArray<TCommandTree>();
        }
        catch (UpdateException ex)
        {
          throw new InvalidOperationException(Strings.ErrorGeneratingCommandTree((object) functionName, (object) modificationFunctionMapping.EntityType.Name), (Exception) ex);
        }
      }
      return this.GenerateFunctionBody<TCommandTree>(migrationSqlGenerator, rowsAffectedParameterName, commandTrees);
    }

    private string GenerateFunctionBody<TCommandTree>(
      AssociationSetModificationFunctionMapping modificationFunctionMapping,
      Func<ModificationCommandTreeGenerator, string, IEnumerable<TCommandTree>> treeGenerator,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator,
      string rowsAffectedParameterName)
      where TCommandTree : DbModificationCommandTree
    {
      TCommandTree[] commandTrees = new TCommandTree[0];
      if (modificationCommandTreeGenerator != null)
        commandTrees = new DynamicToFunctionModificationCommandConverter(modificationFunctionMapping, this._target.EntityContainerMapping).Convert<TCommandTree>(treeGenerator(modificationCommandTreeGenerator.Value, modificationFunctionMapping.AssociationSet.ElementType.Identity)).ToArray<TCommandTree>();
      return this.GenerateFunctionBody<TCommandTree>(migrationSqlGenerator, rowsAffectedParameterName, commandTrees);
    }

    private string GenerateFunctionBody<TCommandTree>(
      MigrationSqlGenerator migrationSqlGenerator,
      string rowsAffectedParameterName,
      TCommandTree[] commandTrees)
      where TCommandTree : DbModificationCommandTree
    {
      if (migrationSqlGenerator == null)
        return (string) null;
      string providerManifestToken = this._target.ProviderInfo.ProviderManifestToken;
      return migrationSqlGenerator.GenerateProcedureBody((ICollection<DbModificationCommandTree>) commandTrees, rowsAffectedParameterName, providerManifestToken);
    }

    private bool DiffModificationFunction(
      ModificationFunctionMapping functionMapping1,
      ModificationFunctionMapping functionMapping2)
    {
      if (!functionMapping1.RowsAffectedParameterName.EqualsOrdinal(functionMapping2.RowsAffectedParameterName) || !functionMapping1.ParameterBindings.SequenceEqual<ModificationFunctionParameterBinding>((IEnumerable<ModificationFunctionParameterBinding>) functionMapping2.ParameterBindings, new Func<ModificationFunctionParameterBinding, ModificationFunctionParameterBinding, bool>(this.DiffParameterBinding)))
        return false;
      IEnumerable<ModificationFunctionResultBinding> functionResultBindings = Enumerable.Empty<ModificationFunctionResultBinding>();
      return ((IEnumerable<ModificationFunctionResultBinding>) functionMapping1.ResultBindings ?? functionResultBindings).SequenceEqual<ModificationFunctionResultBinding>((IEnumerable<ModificationFunctionResultBinding>) functionMapping2.ResultBindings ?? functionResultBindings, new Func<ModificationFunctionResultBinding, ModificationFunctionResultBinding, bool>(EdmModelDiffer.DiffResultBinding));
    }

    private bool DiffParameterBinding(
      ModificationFunctionParameterBinding parameterBinding1,
      ModificationFunctionParameterBinding parameterBinding2)
    {
      FunctionParameter parameter1 = parameterBinding1.Parameter;
      FunctionParameter parameter2 = parameterBinding2.Parameter;
      if (!parameter1.Name.EqualsOrdinal(parameter2.Name) || parameter1.Mode != parameter2.Mode || parameterBinding1.IsCurrent != parameterBinding2.IsCurrent || !parameterBinding1.MemberPath.Members.SequenceEqual<EdmMember>((IEnumerable<EdmMember>) parameterBinding2.MemberPath.Members, (Func<EdmMember, EdmMember, bool>) ((m1, m2) => m1.Identity.EqualsOrdinal(m2.Identity))))
        return false;
      if (this._source.ProviderInfo.Equals((object) this._target.ProviderInfo))
      {
        if (parameter1.TypeName.EqualsIgnoreCase(parameter2.TypeName))
          return parameter1.TypeUsage.EdmEquals((MetadataItem) parameter2.TypeUsage);
        return false;
      }
      byte? precision1 = parameter1.Precision;
      byte? precision2 = parameter2.Precision;
      if (((int) precision1.GetValueOrDefault() != (int) precision2.GetValueOrDefault() ? 0 : (precision1.HasValue == precision2.HasValue ? 1 : 0)) == 0)
        return false;
      byte? scale1 = parameter1.Scale;
      byte? scale2 = parameter2.Scale;
      if ((int) scale1.GetValueOrDefault() == (int) scale2.GetValueOrDefault())
        return scale1.HasValue == scale2.HasValue;
      return false;
    }

    private static bool DiffResultBinding(
      ModificationFunctionResultBinding resultBinding1,
      ModificationFunctionResultBinding resultBinding2)
    {
      return resultBinding1.ColumnName.EqualsOrdinal(resultBinding2.ColumnName) && resultBinding1.Property.Identity.EqualsOrdinal(resultBinding2.Property.Identity);
    }

    private IEnumerable<CreateProcedureOperation> BuildCreateProcedureOperations(
      EntityTypeModificationFunctionMapping modificationFunctionMapping,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      yield return this.BuildCreateProcedureOperation(modificationFunctionMapping.InsertFunctionMapping.Function, this.GenerateInsertFunctionBody(modificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
      yield return this.BuildCreateProcedureOperation(modificationFunctionMapping.UpdateFunctionMapping.Function, this.GenerateUpdateFunctionBody(modificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
      yield return this.BuildCreateProcedureOperation(modificationFunctionMapping.DeleteFunctionMapping.Function, this.GenerateDeleteFunctionBody(modificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
    }

    private IEnumerable<CreateProcedureOperation> BuildCreateProcedureOperations(
      AssociationSetModificationFunctionMapping modificationFunctionMapping,
      Lazy<ModificationCommandTreeGenerator> modificationCommandTreeGenerator,
      MigrationSqlGenerator migrationSqlGenerator)
    {
      yield return this.BuildCreateProcedureOperation(modificationFunctionMapping.InsertFunctionMapping.Function, this.GenerateInsertFunctionBody(modificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
      yield return this.BuildCreateProcedureOperation(modificationFunctionMapping.DeleteFunctionMapping.Function, this.GenerateDeleteFunctionBody(modificationFunctionMapping, modificationCommandTreeGenerator, migrationSqlGenerator));
    }

    private CreateProcedureOperation BuildCreateProcedureOperation(
      EdmFunction function,
      string bodySql)
    {
      CreateProcedureOperation createProcedureOperation = new CreateProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(function), bodySql, (object) null);
      function.Parameters.Each<FunctionParameter>((Action<FunctionParameter>) (p => createProcedureOperation.Parameters.Add(EdmModelDiffer.BuildParameterModel(p, this._target))));
      return createProcedureOperation;
    }

    private AlterProcedureOperation BuildAlterProcedureOperation(
      EdmFunction function,
      string bodySql)
    {
      AlterProcedureOperation alterProcedureOperation = new AlterProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(function), bodySql, (object) null);
      function.Parameters.Each<FunctionParameter>((Action<FunctionParameter>) (p => alterProcedureOperation.Parameters.Add(EdmModelDiffer.BuildParameterModel(p, this._target))));
      return alterProcedureOperation;
    }

    private static ParameterModel BuildParameterModel(
      FunctionParameter functionParameter,
      EdmModelDiffer.ModelMetadata modelMetadata)
    {
      TypeUsage modelTypeUsage = functionParameter.TypeUsage.ModelTypeUsage;
      string name = modelMetadata.ProviderManifest.GetStoreType(modelTypeUsage).EdmType.Name;
      ParameterModel parameterModel1 = new ParameterModel(((PrimitiveType) modelTypeUsage.EdmType).PrimitiveTypeKind, modelTypeUsage);
      parameterModel1.Name = functionParameter.Name;
      parameterModel1.IsOutParameter = functionParameter.Mode == ParameterMode.Out;
      parameterModel1.StoreType = !functionParameter.TypeName.EqualsIgnoreCase(name) ? functionParameter.TypeName : (string) null;
      ParameterModel parameterModel2 = parameterModel1;
      Facet facet;
      if (modelTypeUsage.Facets.TryGetValue("MaxLength", true, out facet) && facet.Value != null)
        parameterModel2.MaxLength = facet.Value as int?;
      if (modelTypeUsage.Facets.TryGetValue("Precision", true, out facet) && facet.Value != null)
        parameterModel2.Precision = (byte?) facet.Value;
      if (modelTypeUsage.Facets.TryGetValue("Scale", true, out facet) && facet.Value != null)
        parameterModel2.Scale = (byte?) facet.Value;
      if (modelTypeUsage.Facets.TryGetValue("FixedLength", true, out facet) && facet.Value != null && (bool) facet.Value)
        parameterModel2.IsFixedLength = new bool?(true);
      if (modelTypeUsage.Facets.TryGetValue("Unicode", true, out facet) && facet.Value != null && !(bool) facet.Value)
        parameterModel2.IsUnicode = new bool?(false);
      return parameterModel2;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "<>h__TransparentIdentifier0")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private IEnumerable<DropProcedureOperation> FindDroppedModificationFunctions()
    {
      return this._source.EntityContainerMapping.EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm1 => (IEnumerable<EntityTypeModificationFunctionMapping>) esm1.ModificationFunctionMappings), (esm1, mfm1) => new
      {
        esm1 = esm1,
        mfm1 = mfm1
      }).Where(_param1 => !this._target.EntityContainerMapping.EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm2 => (IEnumerable<EntityTypeModificationFunctionMapping>) esm2.ModificationFunctionMappings), (esm2, mfm2) => new
      {
        esm2 = esm2,
        mfm2 = mfm2
      }).Where(_param1 => _param1.mfm1.EntityType.Identity == _param1.mfm2.EntityType.Identity).Select(_param0 => _param0.mfm2).Any<EntityTypeModificationFunctionMapping>()).SelectMany(_param0 => (IEnumerable<DropProcedureOperation>) new DropProcedureOperation[3]
      {
        new DropProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(_param0.mfm1.InsertFunctionMapping.Function), (object) null),
        new DropProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(_param0.mfm1.UpdateFunctionMapping.Function), (object) null),
        new DropProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(_param0.mfm1.DeleteFunctionMapping.Function), (object) null)
      }, (_param0, o) => o).Concat<DropProcedureOperation>(this._source.EntityContainerMapping.AssociationSetMappings.Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm1 => asm1.ModificationFunctionMapping != null)).Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm1 => !this._target.EntityContainerMapping.AssociationSetMappings.Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm2 =>
      {
        if (asm2.ModificationFunctionMapping != null)
          return asm1.ModificationFunctionMapping.AssociationSet.Identity == asm2.ModificationFunctionMapping.AssociationSet.Identity;
        return false;
      })).Select<AssociationSetMapping, AssociationSetModificationFunctionMapping>((Func<AssociationSetMapping, AssociationSetModificationFunctionMapping>) (asm2 => asm2.ModificationFunctionMapping)).Any<AssociationSetModificationFunctionMapping>())).SelectMany<AssociationSetMapping, DropProcedureOperation, DropProcedureOperation>((Func<AssociationSetMapping, IEnumerable<DropProcedureOperation>>) (asm1 => (IEnumerable<DropProcedureOperation>) new DropProcedureOperation[2]
      {
        new DropProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(asm1.ModificationFunctionMapping.InsertFunctionMapping.Function), (object) null),
        new DropProcedureOperation(EdmModelDiffer.GetSchemaQualifiedName(asm1.ModificationFunctionMapping.DeleteFunctionMapping.Function), (object) null)
      }), (Func<AssociationSetMapping, DropProcedureOperation, DropProcedureOperation>) ((asm1, o) => o)));
    }

    private static IEnumerable<RenameTableOperation> FindRenamedTables(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs)
    {
      return tablePairs.Where<Tuple<EntitySet, EntitySet>>((Func<Tuple<EntitySet, EntitySet>, bool>) (p => !p.Item1.Table.EqualsIgnoreCase(p.Item2.Table))).Select<Tuple<EntitySet, EntitySet>, RenameTableOperation>((Func<Tuple<EntitySet, EntitySet>, RenameTableOperation>) (p => new RenameTableOperation(EdmModelDiffer.GetSchemaQualifiedName(p.Item1), p.Item2.Table, (object) null)));
    }

    private IEnumerable<CreateTableOperation> FindAddedTables(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs)
    {
      return this._target.StoreEntityContainer.EntitySets.Except<EntitySet>(tablePairs.Select<Tuple<EntitySet, EntitySet>, EntitySet>((Func<Tuple<EntitySet, EntitySet>, EntitySet>) (p => p.Item2))).Select<EntitySet, CreateTableOperation>((Func<EntitySet, CreateTableOperation>) (es => EdmModelDiffer.BuildCreateTableOperation(es, this._target)));
    }

    private IEnumerable<MoveTableOperation> FindMovedTables(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs)
    {
      return tablePairs.Where<Tuple<EntitySet, EntitySet>>((Func<Tuple<EntitySet, EntitySet>, bool>) (p => !p.Item1.Schema.EqualsIgnoreCase(p.Item2.Schema))).Select<Tuple<EntitySet, EntitySet>, MoveTableOperation>((Func<Tuple<EntitySet, EntitySet>, MoveTableOperation>) (p => new MoveTableOperation(new DatabaseName(p.Item2.Table, p.Item1.Schema).ToString(), p.Item2.Schema, (object) null)
      {
        CreateTableOperation = EdmModelDiffer.BuildCreateTableOperation(p.Item2, this._target)
      }));
    }

    private IEnumerable<DropTableOperation> FindDroppedTables(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs)
    {
      return this._source.StoreEntityContainer.EntitySets.Except<EntitySet>(tablePairs.Select<Tuple<EntitySet, EntitySet>, EntitySet>((Func<Tuple<EntitySet, EntitySet>, EntitySet>) (p => p.Item1))).Select<EntitySet, DropTableOperation>((Func<EntitySet, DropTableOperation>) (es => new DropTableOperation(EdmModelDiffer.GetSchemaQualifiedName(es), (IDictionary<string, object>) EdmModelDiffer.GetAnnotations((MetadataItem) es.ElementType), (IDictionary<string, IDictionary<string, object>>) es.ElementType.Properties.Where<EdmProperty>((Func<EdmProperty, bool>) (p => EdmModelDiffer.GetAnnotations((MetadataItem) p).Count > 0)).ToDictionary<EdmProperty, string, IDictionary<string, object>>((Func<EdmProperty, string>) (p => p.Name), (Func<EdmProperty, IDictionary<string, object>>) (p => (IDictionary<string, object>) EdmModelDiffer.GetAnnotations((MetadataItem) p))), EdmModelDiffer.BuildCreateTableOperation(es, this._source), (object) null)));
    }

    private IEnumerable<AlterTableOperation> FindAlteredTables(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs)
    {
      return tablePairs.Where<Tuple<EntitySet, EntitySet>>((Func<Tuple<EntitySet, EntitySet>, bool>) (p => !EdmModelDiffer.GetAnnotations((MetadataItem) p.Item1.ElementType).SequenceEqual<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) EdmModelDiffer.GetAnnotations((MetadataItem) p.Item2.ElementType)))).Select<Tuple<EntitySet, EntitySet>, AlterTableOperation>((Func<Tuple<EntitySet, EntitySet>, AlterTableOperation>) (p => this.BuildAlterTableAnnotationsOperation(p.Item1, p.Item2)));
    }

    private AlterTableOperation BuildAlterTableAnnotationsOperation(
      EntitySet sourceTable,
      EntitySet destinationTable)
    {
      AlterTableOperation operation = new AlterTableOperation(EdmModelDiffer.GetSchemaQualifiedName(destinationTable), EdmModelDiffer.BuildAnnotationPairs((IDictionary<string, object>) EdmModelDiffer.GetAnnotations((MetadataItem) sourceTable.ElementType), (IDictionary<string, object>) EdmModelDiffer.GetAnnotations((MetadataItem) destinationTable.ElementType)), (object) null);
      destinationTable.ElementType.Properties.Each<EdmProperty>((Action<EdmProperty>) (p => operation.Columns.Add(EdmModelDiffer.BuildColumnModel(p, this._target, (IDictionary<string, AnnotationValues>) EdmModelDiffer.GetAnnotations((MetadataItem) p).ToDictionary<KeyValuePair<string, object>, string, AnnotationValues>((Func<KeyValuePair<string, object>, string>) (a => a.Key), (Func<KeyValuePair<string, object>, AnnotationValues>) (a => new AnnotationValues(a.Value, a.Value)))))));
      return operation;
    }

    internal static Dictionary<string, object> GetAnnotations(MetadataItem item)
    {
      return item.Annotations.Where<MetadataProperty>((Func<MetadataProperty, bool>) (a =>
      {
        if (a.Name.StartsWith("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:", StringComparison.Ordinal))
          return !a.Name.EndsWith("Index", StringComparison.Ordinal);
        return false;
      })).ToDictionary<MetadataProperty, string, object>((Func<MetadataProperty, string>) (a => a.Name.Substring("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:".Length)), (Func<MetadataProperty, object>) (a => a.Value));
    }

    private IEnumerable<MigrationOperation> FindAlteredPrimaryKeys(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs,
      ICollection<RenameColumnOperation> renamedColumns,
      ICollection<AlterColumnOperation> alteredColumns)
    {
      return tablePairs.Select(ts => new
      {
        ts = ts,
        t2 = EdmModelDiffer.GetSchemaQualifiedName(ts.Item2)
      }).Where(_param1 =>
      {
        if (_param1.ts.Item1.ElementType.KeyProperties.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) _param1.ts.Item2.ElementType.KeyProperties, (Func<EdmProperty, EdmProperty, bool>) ((p1, p2) =>
        {
          if (!p1.Name.EqualsIgnoreCase(p2.Name))
            return renamedColumns.Any<RenameColumnOperation>((Func<RenameColumnOperation, bool>) (rc =>
            {
              if (rc.Table.EqualsIgnoreCase(_param1.t2) && rc.Name.EqualsIgnoreCase(p1.Name))
                return rc.NewName.EqualsIgnoreCase(p2.Name);
              return false;
            }));
          return true;
        })))
          return _param1.ts.Item2.ElementType.KeyProperties.Any<EdmProperty>((Func<EdmProperty, bool>) (p => alteredColumns.Any<AlterColumnOperation>((Func<AlterColumnOperation, bool>) (ac =>
          {
            if (ac.Table.EqualsIgnoreCase(_param1.t2))
              return ac.Column.Name.EqualsIgnoreCase(p.Name);
            return false;
          }))));
        return true;
      }).SelectMany(_param1 => this.BuildChangePrimaryKeyOperations(_param1.ts), (_param0, o) => o);
    }

    private IEnumerable<MigrationOperation> BuildChangePrimaryKeyOperations(
      Tuple<EntitySet, EntitySet> tablePair)
    {
      List<ReferentialConstraint> sourceReferencedForeignKeys = this._source.StoreItemCollection.GetItems<AssociationType>().Select<AssociationType, ReferentialConstraint>((Func<AssociationType, ReferentialConstraint>) (at => at.Constraint)).Where<ReferentialConstraint>((Func<ReferentialConstraint, bool>) (c => c.FromProperties.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) tablePair.Item1.ElementType.KeyProperties))).ToList<ReferentialConstraint>();
      foreach (ReferentialConstraint referentialConstraint in sourceReferencedForeignKeys)
        yield return (MigrationOperation) EdmModelDiffer.BuildDropForeignKeyOperation(referentialConstraint, this._source);
      DropPrimaryKeyOperation primaryKeyOperation1 = new DropPrimaryKeyOperation((object) null);
      primaryKeyOperation1.Table = EdmModelDiffer.GetSchemaQualifiedName(tablePair.Item2);
      DropPrimaryKeyOperation dropPrimaryKeyOperation = primaryKeyOperation1;
      tablePair.Item1.ElementType.KeyProperties.Each<EdmProperty>((Action<EdmProperty>) (pr => dropPrimaryKeyOperation.Columns.Add(pr.Name)));
      yield return (MigrationOperation) dropPrimaryKeyOperation;
      AddPrimaryKeyOperation primaryKeyOperation2 = new AddPrimaryKeyOperation((object) null);
      primaryKeyOperation2.Table = EdmModelDiffer.GetSchemaQualifiedName(tablePair.Item2);
      AddPrimaryKeyOperation addPrimaryKeyOperation = primaryKeyOperation2;
      tablePair.Item2.ElementType.KeyProperties.Each<EdmProperty>((Action<EdmProperty>) (pr => addPrimaryKeyOperation.Columns.Add(pr.Name)));
      yield return (MigrationOperation) addPrimaryKeyOperation;
      List<ReferentialConstraint> targetReferencedForeignKeys = this._target.StoreItemCollection.GetItems<AssociationType>().Select<AssociationType, ReferentialConstraint>((Func<AssociationType, ReferentialConstraint>) (at => at.Constraint)).Where<ReferentialConstraint>((Func<ReferentialConstraint, bool>) (c => c.FromProperties.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) tablePair.Item2.ElementType.KeyProperties))).ToList<ReferentialConstraint>();
      foreach (ReferentialConstraint referentialConstraint in targetReferencedForeignKeys)
        yield return (MigrationOperation) EdmModelDiffer.BuildAddForeignKeyOperation(referentialConstraint, this._target);
    }

    private IEnumerable<AddForeignKeyOperation> FindAddedForeignKeys(
      ICollection<Tuple<AssociationType, AssociationType>> assocationTypePairs,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      return this._target.StoreItemCollection.GetItems<AssociationType>().Except<AssociationType>(assocationTypePairs.Select<Tuple<AssociationType, AssociationType>, AssociationType>((Func<Tuple<AssociationType, AssociationType>, AssociationType>) (p => p.Item2))).Concat<AssociationType>(assocationTypePairs.Where<Tuple<AssociationType, AssociationType>>((Func<Tuple<AssociationType, AssociationType>, bool>) (at => !this.DiffAssociations(at.Item1.Constraint, at.Item2.Constraint, renamedColumns))).Select<Tuple<AssociationType, AssociationType>, AssociationType>((Func<Tuple<AssociationType, AssociationType>, AssociationType>) (at => at.Item2))).Select<AssociationType, AddForeignKeyOperation>((Func<AssociationType, AddForeignKeyOperation>) (at => EdmModelDiffer.BuildAddForeignKeyOperation(at.Constraint, this._target)));
    }

    private IEnumerable<DropForeignKeyOperation> FindDroppedForeignKeys(
      ICollection<Tuple<AssociationType, AssociationType>> assocationTypePairs,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      return this._source.StoreItemCollection.GetItems<AssociationType>().Except<AssociationType>(assocationTypePairs.Select<Tuple<AssociationType, AssociationType>, AssociationType>((Func<Tuple<AssociationType, AssociationType>, AssociationType>) (p => p.Item1))).Concat<AssociationType>(assocationTypePairs.Where<Tuple<AssociationType, AssociationType>>((Func<Tuple<AssociationType, AssociationType>, bool>) (at => !this.DiffAssociations(at.Item1.Constraint, at.Item2.Constraint, renamedColumns))).Select<Tuple<AssociationType, AssociationType>, AssociationType>((Func<Tuple<AssociationType, AssociationType>, AssociationType>) (at => at.Item1))).Select<AssociationType, DropForeignKeyOperation>((Func<AssociationType, DropForeignKeyOperation>) (at => EdmModelDiffer.BuildDropForeignKeyOperation(at.Constraint, this._source)));
    }

    private bool DiffAssociations(
      ReferentialConstraint referentialConstraint1,
      ReferentialConstraint referentialConstraint2,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      string targetTable = EdmModelDiffer.GetSchemaQualifiedName(this._target.StoreEntityContainer.EntitySets.Single<EntitySet>((Func<EntitySet, bool>) (es => es.ElementType == referentialConstraint2.DependentEnd.GetEntityType())));
      if (referentialConstraint1.ToProperties.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) referentialConstraint2.ToProperties, (Func<EdmProperty, EdmProperty, bool>) ((p1, p2) =>
      {
        if (!p1.Name.EqualsIgnoreCase(p2.Name))
          return renamedColumns.Any<RenameColumnOperation>((Func<RenameColumnOperation, bool>) (rc =>
          {
            if (rc.Table.EqualsIgnoreCase(targetTable) && rc.Name.EqualsIgnoreCase(p1.Name))
              return rc.NewName.EqualsIgnoreCase(p2.Name);
            return false;
          }));
        return true;
      })))
        return referentialConstraint1.PrincipalEnd.DeleteBehavior == referentialConstraint2.PrincipalEnd.DeleteBehavior;
      return false;
    }

    private static AddForeignKeyOperation BuildAddForeignKeyOperation(
      ReferentialConstraint referentialConstraint,
      EdmModelDiffer.ModelMetadata modelMetadata)
    {
      AddForeignKeyOperation addForeignKeyOperation = new AddForeignKeyOperation((object) null);
      EdmModelDiffer.BuildForeignKeyOperation(referentialConstraint, (ForeignKeyOperation) addForeignKeyOperation, modelMetadata);
      referentialConstraint.FromProperties.Each<EdmProperty>((Action<EdmProperty>) (pr => addForeignKeyOperation.PrincipalColumns.Add(pr.Name)));
      addForeignKeyOperation.CascadeDelete = referentialConstraint.PrincipalEnd.DeleteBehavior == OperationAction.Cascade;
      return addForeignKeyOperation;
    }

    private static DropForeignKeyOperation BuildDropForeignKeyOperation(
      ReferentialConstraint referentialConstraint,
      EdmModelDiffer.ModelMetadata modelMetadata)
    {
      DropForeignKeyOperation foreignKeyOperation = new DropForeignKeyOperation(EdmModelDiffer.BuildAddForeignKeyOperation(referentialConstraint, modelMetadata), (object) null);
      EdmModelDiffer.BuildForeignKeyOperation(referentialConstraint, (ForeignKeyOperation) foreignKeyOperation, modelMetadata);
      return foreignKeyOperation;
    }

    private static void BuildForeignKeyOperation(
      ReferentialConstraint referentialConstraint,
      ForeignKeyOperation foreignKeyOperation,
      EdmModelDiffer.ModelMetadata modelMetadata)
    {
      foreignKeyOperation.PrincipalTable = EdmModelDiffer.GetSchemaQualifiedName(modelMetadata.StoreEntityContainer.EntitySets.Single<EntitySet>((Func<EntitySet, bool>) (es => es.ElementType == referentialConstraint.PrincipalEnd.GetEntityType())));
      foreignKeyOperation.DependentTable = EdmModelDiffer.GetSchemaQualifiedName(modelMetadata.StoreEntityContainer.EntitySets.Single<EntitySet>((Func<EntitySet, bool>) (es => es.ElementType == referentialConstraint.DependentEnd.GetEntityType())));
      referentialConstraint.ToProperties.Each<EdmProperty>((Action<EdmProperty>) (pr => foreignKeyOperation.DependentColumns.Add(pr.Name)));
    }

    private IEnumerable<AddColumnOperation> FindAddedColumns(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      return tablePairs.Select(p => new
      {
        p = p,
        t = EdmModelDiffer.GetSchemaQualifiedName(p.Item2)
      }).SelectMany(_param0 => _param0.p.Item2.ElementType.Properties.Except<EdmProperty>((IEnumerable<EdmProperty>) _param0.p.Item1.ElementType.Properties, (Func<EdmProperty, EdmProperty, bool>) ((c1, c2) => c1.Name.EqualsIgnoreCase(c2.Name))), (_param0, c) => new
      {
        \u003C\u003Eh__TransparentIdentifier1fd = _param0,
        c = c
      }).Where(_param1 => !renamedColumns.Any<RenameColumnOperation>((Func<RenameColumnOperation, bool>) (cr =>
      {
        if (cr.Table.EqualsIgnoreCase(_param1.\u003C\u003Eh__TransparentIdentifier1fd.t))
          return cr.NewName.EqualsIgnoreCase(_param1.c.Name);
        return false;
      }))).Select(_param1 => new AddColumnOperation(_param1.\u003C\u003Eh__TransparentIdentifier1fd.t, EdmModelDiffer.BuildColumnModel(_param1.c, this._target, (IDictionary<string, AnnotationValues>) EdmModelDiffer.GetAnnotations((MetadataItem) _param1.c).ToDictionary<KeyValuePair<string, object>, string, AnnotationValues>((Func<KeyValuePair<string, object>, string>) (a => a.Key), (Func<KeyValuePair<string, object>, AnnotationValues>) (a => new AnnotationValues((object) null, a.Value)))), (object) null));
    }

    private IEnumerable<DropColumnOperation> FindDroppedColumns(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      return tablePairs.Select(p => new
      {
        p = p,
        t = EdmModelDiffer.GetSchemaQualifiedName(p.Item2)
      }).SelectMany(_param0 => _param0.p.Item1.ElementType.Properties.Except<EdmProperty>((IEnumerable<EdmProperty>) _param0.p.Item2.ElementType.Properties, (Func<EdmProperty, EdmProperty, bool>) ((c1, c2) => c1.Name.EqualsIgnoreCase(c2.Name))), (_param0, c) => new
      {
        \u003C\u003Eh__TransparentIdentifier212 = _param0,
        c = c
      }).Where(_param1 => !renamedColumns.Any<RenameColumnOperation>((Func<RenameColumnOperation, bool>) (rc =>
      {
        if (rc.Table.EqualsIgnoreCase(_param1.\u003C\u003Eh__TransparentIdentifier212.t))
          return rc.Name.EqualsIgnoreCase(_param1.c.Name);
        return false;
      }))).Select(_param1 => new DropColumnOperation(_param1.\u003C\u003Eh__TransparentIdentifier212.t, _param1.c.Name, (IDictionary<string, object>) EdmModelDiffer.GetAnnotations((MetadataItem) _param1.c), new AddColumnOperation(_param1.\u003C\u003Eh__TransparentIdentifier212.t, EdmModelDiffer.BuildColumnModel(_param1.c, this._source, (IDictionary<string, AnnotationValues>) EdmModelDiffer.GetAnnotations((MetadataItem) _param1.c).ToDictionary<KeyValuePair<string, object>, string, AnnotationValues>((Func<KeyValuePair<string, object>, string>) (a => a.Key), (Func<KeyValuePair<string, object>, AnnotationValues>) (a => new AnnotationValues((object) null, a.Value)))), (object) null), (object) null));
    }

    private IEnumerable<DropColumnOperation> FindOrphanedColumns(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      return tablePairs.Select(p => new
      {
        p = p,
        t = EdmModelDiffer.GetSchemaQualifiedName(p.Item2)
      }).SelectMany(_param1 => (IEnumerable<RenameColumnOperation>) renamedColumns, (_param0, rc1) => new
      {
        \u003C\u003Eh__TransparentIdentifier227 = _param0,
        rc1 = rc1
      }).Where(_param0 => _param0.rc1.Table.EqualsIgnoreCase(_param0.\u003C\u003Eh__TransparentIdentifier227.t)).SelectMany(_param0 => (IEnumerable<EdmProperty>) _param0.\u003C\u003Eh__TransparentIdentifier227.p.Item1.ElementType.Properties, (_param0, c) => new
      {
        \u003C\u003Eh__TransparentIdentifier228 = _param0,
        c = c
      }).Where(_param1 =>
      {
        if (_param1.c.Name.EqualsIgnoreCase(_param1.\u003C\u003Eh__TransparentIdentifier228.rc1.NewName))
          return !renamedColumns.Any<RenameColumnOperation>((Func<RenameColumnOperation, bool>) (rc2 =>
          {
            if (rc2 != _param1.\u003C\u003Eh__TransparentIdentifier228.rc1 && rc2.Table.EqualsIgnoreCase(_param1.\u003C\u003Eh__TransparentIdentifier228.rc1.Table))
              return rc2.Name.EqualsIgnoreCase(_param1.\u003C\u003Eh__TransparentIdentifier228.rc1.NewName);
            return false;
          }));
        return false;
      }).Select(_param1 => new DropColumnOperation(_param1.\u003C\u003Eh__TransparentIdentifier228.\u003C\u003Eh__TransparentIdentifier227.t, _param1.c.Name, (IDictionary<string, object>) EdmModelDiffer.GetAnnotations((MetadataItem) _param1.c), new AddColumnOperation(_param1.\u003C\u003Eh__TransparentIdentifier228.\u003C\u003Eh__TransparentIdentifier227.t, EdmModelDiffer.BuildColumnModel(_param1.c, this._source, (IDictionary<string, AnnotationValues>) EdmModelDiffer.GetAnnotations((MetadataItem) _param1.c).ToDictionary<KeyValuePair<string, object>, string, AnnotationValues>((Func<KeyValuePair<string, object>, string>) (a => a.Key), (Func<KeyValuePair<string, object>, AnnotationValues>) (a => new AnnotationValues((object) null, a.Value)))), (object) null), (object) null));
    }

    private IEnumerable<AlterColumnOperation> FindAlteredColumns(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      return tablePairs.Select(p => new
      {
        p = p,
        t = EdmModelDiffer.GetSchemaQualifiedName(p.Item2)
      }).SelectMany(_param0 => (IEnumerable<EdmProperty>) _param0.p.Item1.ElementType.Properties, (_param0, p1) => new
      {
        \u003C\u003Eh__TransparentIdentifier240 = _param0,
        p1 = p1
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier241 = _param1,
        p2 = _param1.\u003C\u003Eh__TransparentIdentifier240.p.Item2.ElementType.Properties.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (c =>
        {
          if (_param1.p1.Name.EqualsIgnoreCase(c.Name) || renamedColumns.Any<RenameColumnOperation>((Func<RenameColumnOperation, bool>) (rc =>
          {
            if (rc.Table.EqualsIgnoreCase(_param1.\u003C\u003Eh__TransparentIdentifier240.t) && rc.Name.EqualsIgnoreCase(_param1.p1.Name))
              return rc.NewName.EqualsIgnoreCase(c.Name);
            return false;
          })))
            return !this.DiffColumns(_param1.p1, c);
          return false;
        }))
      }).Where(_param0 => _param0.p2 != null).Select(_param1 => this.BuildAlterColumnOperation(_param1.\u003C\u003Eh__TransparentIdentifier241.\u003C\u003Eh__TransparentIdentifier240.t, _param1.p2, this._target, _param1.\u003C\u003Eh__TransparentIdentifier241.p1, this._source));
    }

    private IEnumerable<ConsolidatedIndex> FindSourceIndexes(
      ICollection<Tuple<EntitySet, EntitySet>> tablePairs)
    {
      return this._source.StoreEntityContainer.EntitySets.Select(es => new
      {
        es = es,
        p = tablePairs.SingleOrDefault<Tuple<EntitySet, EntitySet>>((Func<Tuple<EntitySet, EntitySet>, bool>) (p => p.Item1 == es))
      }).Select(_param0 => new
      {
        \u003C\u003Eh__TransparentIdentifier255 = _param0,
        t = EdmModelDiffer.GetSchemaQualifiedName(_param0.p != null ? _param0.p.Item2 : _param0.es)
      }).SelectMany(_param0 => ConsolidatedIndex.BuildIndexes(_param0.t, _param0.\u003C\u003Eh__TransparentIdentifier255.es.ElementType.Properties.Select<EdmProperty, Tuple<string, EdmProperty>>((Func<EdmProperty, Tuple<string, EdmProperty>>) (c => Tuple.Create<string, EdmProperty>(c.Name, c)))), (_param0, i) => i);
    }

    private IEnumerable<ConsolidatedIndex> FindTargetIndexes()
    {
      return this._target.StoreEntityContainer.EntitySets.SelectMany<EntitySet, ConsolidatedIndex, ConsolidatedIndex>((Func<EntitySet, IEnumerable<ConsolidatedIndex>>) (es => ConsolidatedIndex.BuildIndexes(EdmModelDiffer.GetSchemaQualifiedName(es), es.ElementType.Properties.Select<EdmProperty, Tuple<string, EdmProperty>>((Func<EdmProperty, Tuple<string, EdmProperty>>) (p => Tuple.Create<string, EdmProperty>(p.Name, p))))), (Func<EntitySet, ConsolidatedIndex, ConsolidatedIndex>) ((es, i) => i));
    }

    private static IEnumerable<CreateIndexOperation> FindAddedIndexes(
      ICollection<ConsolidatedIndex> sourceIndexes,
      ICollection<ConsolidatedIndex> targetIndexes,
      ICollection<AlterColumnOperation> alteredColumns,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      return targetIndexes.Except<ConsolidatedIndex>((IEnumerable<ConsolidatedIndex>) sourceIndexes, (Func<ConsolidatedIndex, ConsolidatedIndex, bool>) ((i1, i2) =>
      {
        if (EdmModelDiffer.IndexesEqual(i1, i2, renamedColumns))
          return !alteredColumns.Any<AlterColumnOperation>((Func<AlterColumnOperation, bool>) (ac =>
          {
            if (ac.Table.EqualsIgnoreCase(i2.Table))
              return i2.Columns.Contains<string>(ac.Column.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            return false;
          }));
        return false;
      })).Select<ConsolidatedIndex, CreateIndexOperation>((Func<ConsolidatedIndex, CreateIndexOperation>) (i => i.CreateCreateIndexOperation()));
    }

    private static IEnumerable<DropIndexOperation> FindDroppedIndexes(
      ICollection<ConsolidatedIndex> sourceIndexes,
      ICollection<ConsolidatedIndex> targetIndexes,
      ICollection<AlterColumnOperation> alteredColumns,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      return sourceIndexes.Except<ConsolidatedIndex>((IEnumerable<ConsolidatedIndex>) targetIndexes, (Func<ConsolidatedIndex, ConsolidatedIndex, bool>) ((i2, i1) =>
      {
        if (EdmModelDiffer.IndexesEqual(i1, i2, renamedColumns))
          return !alteredColumns.Any<AlterColumnOperation>((Func<AlterColumnOperation, bool>) (ac =>
          {
            if (ac.Table.EqualsIgnoreCase(i2.Table))
              return i2.Columns.Contains<string>(ac.Column.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            return false;
          }));
        return false;
      })).Select<ConsolidatedIndex, DropIndexOperation>((Func<ConsolidatedIndex, DropIndexOperation>) (i => i.CreateDropIndexOperation()));
    }

    private static bool IndexesEqual(
      ConsolidatedIndex consolidatedIndex1,
      ConsolidatedIndex consolidatedIndex2,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      if (!consolidatedIndex1.Table.EqualsIgnoreCase(consolidatedIndex2.Table) || !consolidatedIndex1.Index.Equals((object) consolidatedIndex2.Index))
        return false;
      return consolidatedIndex1.Columns.Select<string, string>((Func<string, string>) (c => renamedColumns.Where<RenameColumnOperation>((Func<RenameColumnOperation, bool>) (rc =>
      {
        if (rc.Table.EqualsIgnoreCase(consolidatedIndex1.Table))
          return rc.Name.EqualsIgnoreCase(c);
        return false;
      })).Select<RenameColumnOperation, string>((Func<RenameColumnOperation, string>) (rc => rc.NewName)).SingleOrDefault<string>() ?? c)).SequenceEqual<string>(consolidatedIndex2.Columns, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private static IEnumerable<RenameIndexOperation> FindRenamedIndexes(
      ICollection<CreateIndexOperation> addedIndexes,
      ICollection<DropIndexOperation> droppedIndexes,
      ICollection<AlterColumnOperation> alteredColumns,
      ICollection<RenameColumnOperation> renamedColumns)
    {
      return addedIndexes.ToList<CreateIndexOperation>().SelectMany((Func<CreateIndexOperation, IEnumerable<DropIndexOperation>>) (ci1 => (IEnumerable<DropIndexOperation>) droppedIndexes.ToList<DropIndexOperation>()), (ci1, di) => new
      {
        ci1 = ci1,
        di = di
      }).Select(_param0 => new
      {
        \u003C\u003Eh__TransparentIdentifier285 = _param0,
        ci2 = (CreateIndexOperation) _param0.di.Inverse
      }).Where(_param1 =>
      {
        if (_param1.\u003C\u003Eh__TransparentIdentifier285.ci1.Table.EqualsIgnoreCase(_param1.ci2.Table) && !_param1.\u003C\u003Eh__TransparentIdentifier285.ci1.Name.EqualsIgnoreCase(_param1.ci2.Name) && (_param1.\u003C\u003Eh__TransparentIdentifier285.ci1.Columns.SequenceEqual<string>(_param1.ci2.Columns.Select<string, string>((Func<string, string>) (c => renamedColumns.Where<RenameColumnOperation>((Func<RenameColumnOperation, bool>) (rc =>
        {
          if (rc.Table.EqualsIgnoreCase(_param1.ci2.Table))
            return rc.Name.EqualsIgnoreCase(c);
          return false;
        })).Select<RenameColumnOperation, string>((Func<RenameColumnOperation, string>) (rc => rc.NewName)).SingleOrDefault<string>() ?? c)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && _param1.\u003C\u003Eh__TransparentIdentifier285.ci1.IsClustered == _param1.ci2.IsClustered) && (_param1.\u003C\u003Eh__TransparentIdentifier285.ci1.IsUnique == _param1.ci2.IsUnique && !alteredColumns.Any<AlterColumnOperation>((Func<AlterColumnOperation, bool>) (ac =>
        {
          if (ac.Table.EqualsIgnoreCase(_param1.\u003C\u003Eh__TransparentIdentifier285.ci1.Table))
            return _param1.\u003C\u003Eh__TransparentIdentifier285.ci1.Columns.Contains<string>(ac.Column.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          return false;
        })) && addedIndexes.Remove(_param1.\u003C\u003Eh__TransparentIdentifier285.ci1)))
          return droppedIndexes.Remove(_param1.\u003C\u003Eh__TransparentIdentifier285.di);
        return false;
      }).Select(_param0 => new RenameIndexOperation(_param0.\u003C\u003Eh__TransparentIdentifier285.ci1.Table, _param0.\u003C\u003Eh__TransparentIdentifier285.di.Name, _param0.\u003C\u003Eh__TransparentIdentifier285.ci1.Name, (object) null));
    }

    private bool DiffColumns(EdmProperty column1, EdmProperty column2)
    {
      if (column1.Nullable != column2.Nullable || column1.PrimitiveType.PrimitiveTypeKind != column2.PrimitiveType.PrimitiveTypeKind || column1.StoreGeneratedPattern != column2.StoreGeneratedPattern || !EdmModelDiffer.GetAnnotations((MetadataItem) column1).OrderBy<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (a => a.Key)).SequenceEqual<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) EdmModelDiffer.GetAnnotations((MetadataItem) column2).OrderBy<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (a => a.Key))))
        return false;
      if (this._source.ProviderInfo.Equals((object) this._target.ProviderInfo))
      {
        if (column1.TypeName.EqualsIgnoreCase(column2.TypeName))
          return column1.TypeUsage.EdmEquals((MetadataItem) column2.TypeUsage);
        return false;
      }
      byte? precision1 = column1.Precision;
      byte? precision2 = column2.Precision;
      if (((int) precision1.GetValueOrDefault() != (int) precision2.GetValueOrDefault() ? 0 : (precision1.HasValue == precision2.HasValue ? 1 : 0)) != 0)
      {
        byte? scale1 = column1.Scale;
        byte? scale2 = column2.Scale;
        if (((int) scale1.GetValueOrDefault() != (int) scale2.GetValueOrDefault() ? 0 : (scale1.HasValue == scale2.HasValue ? 1 : 0)) != 0)
        {
          bool? isUnicode1 = column1.IsUnicode;
          bool? isUnicode2 = column2.IsUnicode;
          if ((isUnicode1.GetValueOrDefault() != isUnicode2.GetValueOrDefault() ? 0 : (isUnicode1.HasValue == isUnicode2.HasValue ? 1 : 0)) != 0)
          {
            bool? isFixedLength1 = column1.IsFixedLength;
            bool? isFixedLength2 = column2.IsFixedLength;
            if (isFixedLength1.GetValueOrDefault() == isFixedLength2.GetValueOrDefault())
              return isFixedLength1.HasValue == isFixedLength2.HasValue;
            return false;
          }
        }
      }
      return false;
    }

    private AlterColumnOperation BuildAlterColumnOperation(
      string table,
      EdmProperty targetProperty,
      EdmModelDiffer.ModelMetadata targetModelMetadata,
      EdmProperty sourceProperty,
      EdmModelDiffer.ModelMetadata sourceModelMetadata)
    {
      IDictionary<string, AnnotationValues> dictionary1 = EdmModelDiffer.BuildAnnotationPairs((IDictionary<string, object>) EdmModelDiffer.GetAnnotations((MetadataItem) sourceProperty), (IDictionary<string, object>) EdmModelDiffer.GetAnnotations((MetadataItem) targetProperty));
      Dictionary<string, AnnotationValues> dictionary2 = dictionary1.ToDictionary<KeyValuePair<string, AnnotationValues>, string, AnnotationValues>((Func<KeyValuePair<string, AnnotationValues>, string>) (a => a.Key), (Func<KeyValuePair<string, AnnotationValues>, AnnotationValues>) (a => new AnnotationValues(a.Value.NewValue, a.Value.OldValue)));
      ColumnModel column1 = EdmModelDiffer.BuildColumnModel(targetProperty, targetModelMetadata, dictionary1);
      ColumnModel column2 = EdmModelDiffer.BuildColumnModel(sourceProperty, sourceModelMetadata, (IDictionary<string, AnnotationValues>) dictionary2);
      column2.Name = column1.Name;
      return new AlterColumnOperation(table, column1, column1.IsNarrowerThan(column2, this._target.ProviderManifest), new AlterColumnOperation(table, column2, column2.IsNarrowerThan(column1, this._target.ProviderManifest), (object) null), (object) null);
    }

    private static IDictionary<string, AnnotationValues> BuildAnnotationPairs(
      IDictionary<string, object> rawSourceAnnotations,
      IDictionary<string, object> rawTargetAnnotations)
    {
      Dictionary<string, AnnotationValues> dictionary = new Dictionary<string, AnnotationValues>();
      foreach (string key in rawTargetAnnotations.Keys.Concat<string>((IEnumerable<string>) rawSourceAnnotations.Keys).Distinct<string>())
      {
        if (!rawSourceAnnotations.ContainsKey(key))
          dictionary[key] = new AnnotationValues((object) null, rawTargetAnnotations[key]);
        else if (!rawTargetAnnotations.ContainsKey(key))
          dictionary[key] = new AnnotationValues(rawSourceAnnotations[key], (object) null);
        else if (!object.Equals(rawSourceAnnotations[key], rawTargetAnnotations[key]))
          dictionary[key] = new AnnotationValues(rawSourceAnnotations[key], rawTargetAnnotations[key]);
      }
      return (IDictionary<string, AnnotationValues>) dictionary;
    }

    private IEnumerable<RenameColumnOperation> FindRenamedColumns(
      ICollection<Tuple<MappingFragment, MappingFragment>> mappingFragmentPairs,
      ICollection<Tuple<AssociationType, AssociationType>> associationTypePairs)
    {
      return EdmModelDiffer.FindRenamedMappedColumns(mappingFragmentPairs).Concat<RenameColumnOperation>(this.FindRenamedForeignKeyColumns(associationTypePairs)).Concat<RenameColumnOperation>(EdmModelDiffer.FindRenamedDiscriminatorColumns(mappingFragmentPairs)).Distinct<RenameColumnOperation>((IEqualityComparer<RenameColumnOperation>) new DynamicEqualityComparer<RenameColumnOperation>((Func<RenameColumnOperation, RenameColumnOperation, bool>) ((c1, c2) =>
      {
        if (c1.Table.EqualsIgnoreCase(c2.Table) && c1.Name.EqualsIgnoreCase(c2.Name))
          return c1.NewName.EqualsIgnoreCase(c2.NewName);
        return false;
      })));
    }

    private static IEnumerable<RenameColumnOperation> FindRenamedMappedColumns(
      ICollection<Tuple<MappingFragment, MappingFragment>> mappingFragmentPairs)
    {
      return mappingFragmentPairs.Select(mfs => new
      {
        mfs = mfs,
        t = EdmModelDiffer.GetSchemaQualifiedName(mfs.Item2.StoreEntitySet)
      }).SelectMany(_param0 => EdmModelDiffer.FindRenamedMappedColumns(_param0.mfs.Item1, _param0.mfs.Item2, _param0.t), (_param0, cr) => cr);
    }

    private static IEnumerable<RenameColumnOperation> FindRenamedMappedColumns(
      MappingFragment mappingFragment1,
      MappingFragment mappingFragment2,
      string table)
    {
      return mappingFragment1.FlattenedProperties.SelectMany((Func<ColumnMappingBuilder, IEnumerable<ColumnMappingBuilder>>) (cmb1 => mappingFragment2.FlattenedProperties), (cmb1, cmb2) => new
      {
        cmb1 = cmb1,
        cmb2 = cmb2
      }).Where(_param0 =>
      {
        if (_param0.cmb1.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) _param0.cmb2.PropertyPath, (IEqualityComparer<EdmProperty>) new DynamicEqualityComparer<EdmProperty>((Func<EdmProperty, EdmProperty, bool>) ((p1, p2) => p1.EdmEquals((MetadataItem) p2)))))
          return !_param0.cmb1.ColumnProperty.Name.EqualsIgnoreCase(_param0.cmb2.ColumnProperty.Name);
        return false;
      }).Select(_param1 => new RenameColumnOperation(table, _param1.cmb1.ColumnProperty.Name, _param1.cmb2.ColumnProperty.Name, (object) null));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private IEnumerable<RenameColumnOperation> FindRenamedForeignKeyColumns(
      ICollection<Tuple<AssociationType, AssociationType>> associationTypePairs)
    {
      return associationTypePairs.Select(ats => new
      {
        ats = ats,
        rc1 = ats.Item1.Constraint
      }).Select(_param0 => new
      {
        \u003C\u003Eh__TransparentIdentifier2b7 = _param0,
        rc2 = _param0.ats.Item2.Constraint
      }).SelectMany(_param0 => _param0.\u003C\u003Eh__TransparentIdentifier2b7.rc1.ToProperties.Zip<EdmProperty, EdmProperty>((IEnumerable<EdmProperty>) _param0.rc2.ToProperties), (_param0, ps) => new
      {
        \u003C\u003Eh__TransparentIdentifier2b8 = _param0,
        ps = ps
      }).Where(_param0 =>
      {
        if (_param0.ps.Key.Name.EqualsIgnoreCase(_param0.ps.Value.Name))
          return false;
        if (_param0.\u003C\u003Eh__TransparentIdentifier2b8.rc2.DependentEnd.GetEntityType().Properties.Any<EdmProperty>((Func<EdmProperty, bool>) (p => p.Name.EqualsIgnoreCase(_param0.ps.Key.Name))))
          return _param0.\u003C\u003Eh__TransparentIdentifier2b8.\u003C\u003Eh__TransparentIdentifier2b7.rc1.DependentEnd.GetEntityType().Properties.Any<EdmProperty>((Func<EdmProperty, bool>) (p => p.Name.EqualsIgnoreCase(_param0.ps.Value.Name)));
        return true;
      }).Select(_param1 => new RenameColumnOperation(EdmModelDiffer.GetSchemaQualifiedName(this._target.StoreEntityContainer.EntitySets.Single<EntitySet>((Func<EntitySet, bool>) (es => es.ElementType == _param1.\u003C\u003Eh__TransparentIdentifier2b8.rc2.DependentEnd.GetEntityType()))), _param1.ps.Key.Name, _param1.ps.Value.Name, (object) null));
    }

    private static IEnumerable<RenameColumnOperation> FindRenamedDiscriminatorColumns(
      ICollection<Tuple<MappingFragment, MappingFragment>> mappingFragmentPairs)
    {
      return mappingFragmentPairs.Select(mfs => new
      {
        mfs = mfs,
        t = EdmModelDiffer.GetSchemaQualifiedName(mfs.Item2.StoreEntitySet)
      }).SelectMany(_param0 => EdmModelDiffer.FindRenamedDiscriminatorColumns(_param0.mfs.Item1, _param0.mfs.Item2, _param0.t), (_param0, cr) => cr);
    }

    private static IEnumerable<RenameColumnOperation> FindRenamedDiscriminatorColumns(
      MappingFragment mappingFragment1,
      MappingFragment mappingFragment2,
      string table)
    {
      return mappingFragment1.Conditions.SelectMany((Func<ConditionPropertyMapping, IEnumerable<ConditionPropertyMapping>>) (c1 => (IEnumerable<ConditionPropertyMapping>) mappingFragment2.Conditions), (c1, c2) => new
      {
        c1 = c1,
        c2 = c2
      }).Where(_param0 => object.Equals(_param0.c1.Value, _param0.c2.Value)).Where(_param0 => !_param0.c1.Column.Name.EqualsIgnoreCase(_param0.c2.Column.Name)).Select(_param1 => new RenameColumnOperation(table, _param1.c1.Column.Name, _param1.c2.Column.Name, (object) null));
    }

    private static CreateTableOperation BuildCreateTableOperation(
      EntitySet entitySet,
      EdmModelDiffer.ModelMetadata modelMetadata)
    {
      CreateTableOperation createTableOperation = new CreateTableOperation(EdmModelDiffer.GetSchemaQualifiedName(entitySet), (IDictionary<string, object>) EdmModelDiffer.GetAnnotations((MetadataItem) entitySet.ElementType), (object) null);
      entitySet.ElementType.Properties.Each<EdmProperty>((Action<EdmProperty>) (p => createTableOperation.Columns.Add(EdmModelDiffer.BuildColumnModel(p, modelMetadata, (IDictionary<string, AnnotationValues>) EdmModelDiffer.GetAnnotations((MetadataItem) p).ToDictionary<KeyValuePair<string, object>, string, AnnotationValues>((Func<KeyValuePair<string, object>, string>) (a => a.Key), (Func<KeyValuePair<string, object>, AnnotationValues>) (a => new AnnotationValues((object) null, a.Value)))))));
      AddPrimaryKeyOperation addPrimaryKeyOperation = new AddPrimaryKeyOperation((object) null);
      entitySet.ElementType.KeyProperties.Each<EdmProperty>((Action<EdmProperty>) (p => addPrimaryKeyOperation.Columns.Add(p.Name)));
      createTableOperation.PrimaryKey = addPrimaryKeyOperation;
      return createTableOperation;
    }

    private static ColumnModel BuildColumnModel(
      EdmProperty property,
      EdmModelDiffer.ModelMetadata modelMetadata,
      IDictionary<string, AnnotationValues> annotations)
    {
      TypeUsage edmType = modelMetadata.ProviderManifest.GetEdmType(property.TypeUsage);
      TypeUsage storeType = modelMetadata.ProviderManifest.GetStoreType(edmType);
      return EdmModelDiffer.BuildColumnModel(property, edmType, storeType, annotations);
    }

    public static ColumnModel BuildColumnModel(
      EdmProperty property,
      TypeUsage conceptualTypeUsage,
      TypeUsage defaultStoreTypeUsage,
      IDictionary<string, AnnotationValues> annotations)
    {
      ColumnModel columnModel1 = new ColumnModel(property.PrimitiveType.PrimitiveTypeKind, conceptualTypeUsage);
      columnModel1.Name = property.Name;
      columnModel1.IsNullable = !property.Nullable ? new bool?(false) : new bool?();
      columnModel1.StoreType = !property.TypeName.EqualsIgnoreCase(defaultStoreTypeUsage.EdmType.Name) ? property.TypeName : (string) null;
      columnModel1.IsIdentity = property.IsStoreGeneratedIdentity && ((IEnumerable<PrimitiveTypeKind>) EdmModelDiffer._validIdentityTypes).Contains<PrimitiveTypeKind>(property.PrimitiveType.PrimitiveTypeKind);
      ColumnModel columnModel2 = columnModel1;
      int num;
      if (property.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.Binary)
      {
        int? maxLength = property.MaxLength;
        if ((maxLength.GetValueOrDefault() != 8 ? 0 : (maxLength.HasValue ? 1 : 0)) != 0)
        {
          num = property.IsStoreGeneratedComputed ? 1 : 0;
          goto label_4;
        }
      }
      num = 0;
label_4:
      columnModel2.IsTimestamp = num != 0;
      ColumnModel columnModel3 = columnModel1;
      bool? isUnicode = property.IsUnicode;
      bool? nullable1 = (isUnicode.GetValueOrDefault() ? 0 : (isUnicode.HasValue ? 1 : 0)) != 0 ? new bool?(false) : new bool?();
      columnModel3.IsUnicode = nullable1;
      ColumnModel columnModel4 = columnModel1;
      bool? isFixedLength = property.IsFixedLength;
      bool? nullable2 = (!isFixedLength.GetValueOrDefault() ? 0 : (isFixedLength.HasValue ? 1 : 0)) != 0 ? new bool?(true) : new bool?();
      columnModel4.IsFixedLength = nullable2;
      columnModel1.Annotations = annotations;
      ColumnModel columnModel5 = columnModel1;
      Facet facet;
      if (property.TypeUsage.Facets.TryGetValue("MaxLength", true, out facet) && !facet.IsUnbounded && !facet.Description.IsConstant)
        columnModel5.MaxLength = (int?) facet.Value;
      if (property.TypeUsage.Facets.TryGetValue("Precision", true, out facet) && !facet.IsUnbounded && !facet.Description.IsConstant)
        columnModel5.Precision = (byte?) facet.Value;
      if (property.TypeUsage.Facets.TryGetValue("Scale", true, out facet) && !facet.IsUnbounded && !facet.Description.IsConstant)
        columnModel5.Scale = (byte?) facet.Value;
      return columnModel5;
    }

    private static DbProviderManifest GetProviderManifest(
      DbProviderInfo providerInfo)
    {
      return DbConfiguration.DependencyResolver.GetService<DbProviderFactory>((object) providerInfo.ProviderInvariantName).GetProviderServices().GetProviderManifest(providerInfo.ProviderManifestToken);
    }

    private static string GetSchemaQualifiedName(EntitySet entitySet)
    {
      return new DatabaseName(entitySet.Table, entitySet.Schema).ToString();
    }

    private static string GetSchemaQualifiedName(EdmFunction function)
    {
      return new DatabaseName(function.FunctionName, function.Schema).ToString();
    }

    private class ModelMetadata
    {
      public EdmItemCollection EdmItemCollection { get; set; }

      public StoreItemCollection StoreItemCollection { get; set; }

      public EntityContainerMapping EntityContainerMapping { get; set; }

      public EntityContainer StoreEntityContainer { get; set; }

      public DbProviderManifest ProviderManifest { get; set; }

      public DbProviderInfo ProviderInfo { get; set; }
    }
  }
}
