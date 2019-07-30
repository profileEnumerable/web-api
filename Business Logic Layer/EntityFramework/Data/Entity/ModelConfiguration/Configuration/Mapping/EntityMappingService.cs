// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Mapping.EntityMappingService
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration.Mapping
{
  internal class EntityMappingService
  {
    private readonly DbDatabaseMapping _databaseMapping;
    private Dictionary<EntityType, TableMapping> _tableMappings;
    private SortedEntityTypeIndex _entityTypes;

    public EntityMappingService(DbDatabaseMapping databaseMapping)
    {
      this._databaseMapping = databaseMapping;
    }

    public void Configure()
    {
      this.Analyze();
      this.Transform();
    }

    private void Analyze()
    {
      this._tableMappings = new Dictionary<EntityType, TableMapping>();
      this._entityTypes = new SortedEntityTypeIndex();
      foreach (EntitySetMapping entitySetMapping in this._databaseMapping.EntityContainerMappings.SelectMany<EntityContainerMapping, EntitySetMapping>((Func<EntityContainerMapping, IEnumerable<EntitySetMapping>>) (ecm => ecm.EntitySetMappings)))
      {
        foreach (EntityTypeMapping entityTypeMapping in entitySetMapping.EntityTypeMappings)
        {
          this._entityTypes.Add(entitySetMapping.EntitySet, entityTypeMapping.EntityType);
          foreach (MappingFragment mappingFragment in entityTypeMapping.MappingFragments)
            this.FindOrCreateTableMapping(mappingFragment.Table).AddEntityTypeMappingFragment(entitySetMapping.EntitySet, entityTypeMapping.EntityType, mappingFragment);
        }
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void Transform()
    {
      foreach (EntitySet entitySet1 in this._entityTypes.GetEntitySets())
      {
        EntitySet entitySet = entitySet1;
        Dictionary<TableMapping, Dictionary<EntityType, EntityTypeMapping>> dictionary = new Dictionary<TableMapping, Dictionary<EntityType, EntityTypeMapping>>();
        foreach (EntityType entityType1 in this._entityTypes.GetEntityTypes(entitySet))
        {
          EntityType entityType = entityType1;
          foreach (TableMapping tableMapping in this._tableMappings.Values.Where<TableMapping>((Func<TableMapping, bool>) (tm => tm.EntityTypes.Contains(entitySet, entityType))))
          {
            Dictionary<EntityType, EntityTypeMapping> rootMappings;
            if (!dictionary.TryGetValue(tableMapping, out rootMappings))
            {
              rootMappings = new Dictionary<EntityType, EntityTypeMapping>();
              dictionary.Add(tableMapping, rootMappings);
            }
            EntityMappingService.RemoveRedundantDefaultDiscriminators(tableMapping);
            bool requiresIsTypeOf = this.DetermineRequiresIsTypeOf(tableMapping, entitySet, entityType);
            EntityTypeMapping propertiesTypeMapping;
            MappingFragment propertiesTypeMappingFragment;
            if (this.FindPropertyEntityTypeMapping(tableMapping, entitySet, entityType, requiresIsTypeOf, out propertiesTypeMapping, out propertiesTypeMappingFragment))
            {
              bool entityTypeMapping1 = EntityMappingService.DetermineRequiresSplitEntityTypeMapping(tableMapping, entityType, requiresIsTypeOf);
              EntityTypeMapping conditionTypeMapping = this.FindConditionTypeMapping(entityType, entityTypeMapping1, propertiesTypeMapping);
              MappingFragment typeMappingFragment = EntityMappingService.FindConditionTypeMappingFragment(this._databaseMapping.Database.GetEntitySet(tableMapping.Table), propertiesTypeMappingFragment, conditionTypeMapping);
              if (requiresIsTypeOf)
              {
                if (!propertiesTypeMapping.IsHierarchyMapping)
                {
                  EntityTypeMapping entityTypeMapping2 = this._databaseMapping.GetEntityTypeMappings(entityType).SingleOrDefault<EntityTypeMapping>((Func<EntityTypeMapping, bool>) (etm => etm.IsHierarchyMapping));
                  if (entityTypeMapping2 == null)
                  {
                    if (propertiesTypeMapping.MappingFragments.Count > 1)
                    {
                      EntityTypeMapping typeMapping = propertiesTypeMapping.Clone();
                      this._databaseMapping.GetEntitySetMappings().Single<EntitySetMapping>((Func<EntitySetMapping, bool>) (esm => esm.EntityTypeMappings.Contains(propertiesTypeMapping))).AddTypeMapping(typeMapping);
                      foreach (MappingFragment fragment in propertiesTypeMapping.MappingFragments.Where<MappingFragment>((Func<MappingFragment, bool>) (tmf => tmf != propertiesTypeMappingFragment)).ToArray<MappingFragment>())
                      {
                        propertiesTypeMapping.RemoveFragment(fragment);
                        typeMapping.AddFragment(fragment);
                      }
                    }
                    propertiesTypeMapping.AddIsOfType(propertiesTypeMapping.EntityType);
                  }
                  else
                  {
                    propertiesTypeMapping.RemoveFragment(propertiesTypeMappingFragment);
                    if (propertiesTypeMapping.MappingFragments.Count == 0)
                      this._databaseMapping.GetEntitySetMapping(entitySet).RemoveTypeMapping(propertiesTypeMapping);
                    propertiesTypeMapping = entityTypeMapping2;
                    propertiesTypeMapping.AddFragment(propertiesTypeMappingFragment);
                  }
                }
                rootMappings.Add(entityType, propertiesTypeMapping);
              }
              EntityMappingService.ConfigureTypeMappings(tableMapping, rootMappings, entityType, propertiesTypeMappingFragment, typeMappingFragment);
              if (propertiesTypeMappingFragment.IsUnmappedPropertiesFragment() && propertiesTypeMappingFragment.ColumnMappings.All<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => entityType.GetKeyProperties().Contains(pm.PropertyPath.First<EdmProperty>()))))
              {
                this.RemoveFragment(entitySet, propertiesTypeMapping, propertiesTypeMappingFragment);
                if (entityTypeMapping1 && typeMappingFragment.ColumnMappings.All<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => entityType.GetKeyProperties().Contains(pm.PropertyPath.First<EdmProperty>()))))
                  this.RemoveFragment(entitySet, conditionTypeMapping, typeMappingFragment);
              }
              EntityMappingConfiguration.CleanupUnmappedArtifacts(this._databaseMapping, tableMapping.Table);
              foreach (ForeignKeyBuilder foreignKeyBuilder in tableMapping.Table.ForeignKeyBuilders)
              {
                AssociationType associationType = foreignKeyBuilder.GetAssociationType();
                if (associationType != null && associationType.IsRequiredToNonRequired())
                {
                  AssociationEndMember principalEnd;
                  AssociationEndMember dependentEnd;
                  foreignKeyBuilder.GetAssociationType().TryGuessPrincipalAndDependentEnds(out principalEnd, out dependentEnd);
                  if (dependentEnd.GetEntityType() == entityType)
                    this.MarkColumnsAsNonNullableIfNoTableSharing(entitySet, tableMapping.Table, entityType, foreignKeyBuilder.DependentColumns);
                }
              }
            }
          }
        }
        this.ConfigureAssociationSetMappingForeignKeys(entitySet);
      }
    }

    private void ConfigureAssociationSetMappingForeignKeys(EntitySet entitySet)
    {
      foreach (AssociationSetMapping associationSetMapping in this._databaseMapping.EntityContainerMappings.SelectMany<EntityContainerMapping, AssociationSetMapping>((Func<EntityContainerMapping, IEnumerable<AssociationSetMapping>>) (ecm => ecm.AssociationSetMappings)).Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm =>
      {
        if (asm.AssociationSet.SourceSet == entitySet || asm.AssociationSet.TargetSet == entitySet)
          return asm.AssociationSet.ElementType.IsRequiredToNonRequired();
        return false;
      })))
      {
        AssociationEndMember principalEnd;
        AssociationEndMember dependentEnd;
        associationSetMapping.AssociationSet.ElementType.TryGuessPrincipalAndDependentEnds(out principalEnd, out dependentEnd);
        if (dependentEnd == associationSetMapping.AssociationSet.ElementType.SourceEnd && associationSetMapping.AssociationSet.SourceSet == entitySet || dependentEnd == associationSetMapping.AssociationSet.ElementType.TargetEnd && associationSetMapping.AssociationSet.TargetSet == entitySet)
        {
          EndPropertyMapping endPropertyMapping = associationSetMapping.SourceEndMapping.AssociationEnd == dependentEnd ? associationSetMapping.TargetEndMapping : associationSetMapping.SourceEndMapping;
          this.MarkColumnsAsNonNullableIfNoTableSharing(entitySet, associationSetMapping.Table, dependentEnd.GetEntityType(), endPropertyMapping.PropertyMappings.Select<ScalarPropertyMapping, EdmProperty>((Func<ScalarPropertyMapping, EdmProperty>) (pm => pm.Column)));
        }
      }
    }

    private void MarkColumnsAsNonNullableIfNoTableSharing(
      EntitySet entitySet,
      EntityType table,
      EntityType dependentEndEntityType,
      IEnumerable<EdmProperty> columns)
    {
      IEnumerable<EntityType> source = this._tableMappings[table].EntityTypes.GetEntityTypes(entitySet).Where<EntityType>((Func<EntityType, bool>) (et =>
      {
        if (et == dependentEndEntityType)
          return false;
        if (!et.IsAncestorOf(dependentEndEntityType))
          return !dependentEndEntityType.IsAncestorOf(et);
        return true;
      }));
      if (source.Count<EntityType>() != 0 && !source.All<EntityType>((Func<EntityType, bool>) (et => et.Abstract)))
        return;
      columns.Each<EdmProperty, bool>((Func<EdmProperty, bool>) (c => c.Nullable = false));
    }

    private static void ConfigureTypeMappings(
      TableMapping tableMapping,
      Dictionary<EntityType, EntityTypeMapping> rootMappings,
      EntityType entityType,
      MappingFragment propertiesTypeMappingFragment,
      MappingFragment conditionTypeMappingFragment)
    {
      List<ColumnMappingBuilder> columnMappingBuilderList = new List<ColumnMappingBuilder>(propertiesTypeMappingFragment.ColumnMappings.Where<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => !pm.ColumnProperty.IsPrimaryKeyColumn)));
      List<ConditionPropertyMapping> conditionPropertyMappingList = new List<ConditionPropertyMapping>(propertiesTypeMappingFragment.ColumnConditions);
      foreach (var data in tableMapping.ColumnMappings.SelectMany((Func<ColumnMapping, IEnumerable<PropertyMappingSpecification>>) (cm => (IEnumerable<PropertyMappingSpecification>) cm.PropertyMappings), (cm, pm) => new
      {
        cm = cm,
        pm = pm
      }).Where(_param1 => _param1.pm.EntityType == entityType).Select(_param0 => new
      {
        Column = _param0.cm.Column,
        Property = _param0.pm
      }))
      {
        var columnMapping = data;
        if (columnMapping.Property.PropertyPath != null && !EntityMappingService.IsRootTypeMapping(rootMappings, columnMapping.Property.EntityType, columnMapping.Property.PropertyPath))
        {
          ColumnMappingBuilder columnMappingBuilder1 = propertiesTypeMappingFragment.ColumnMappings.SingleOrDefault<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (x => x.PropertyPath == columnMapping.Property.PropertyPath));
          if (columnMappingBuilder1 != null)
          {
            columnMappingBuilderList.Remove(columnMappingBuilder1);
          }
          else
          {
            ColumnMappingBuilder columnMappingBuilder2 = new ColumnMappingBuilder(columnMapping.Column, columnMapping.Property.PropertyPath);
            propertiesTypeMappingFragment.AddColumnMapping(columnMappingBuilder2);
          }
        }
        if (columnMapping.Property.Conditions != null)
        {
          foreach (ConditionPropertyMapping condition in (IEnumerable<ConditionPropertyMapping>) columnMapping.Property.Conditions)
          {
            if (conditionTypeMappingFragment.ColumnConditions.Contains<ConditionPropertyMapping>(condition))
              conditionPropertyMappingList.Remove(condition);
            else if (!entityType.Abstract)
              conditionTypeMappingFragment.AddConditionProperty(condition);
          }
        }
      }
      foreach (ColumnMappingBuilder columnMappingBuilder in columnMappingBuilderList)
        propertiesTypeMappingFragment.RemoveColumnMapping(columnMappingBuilder);
      foreach (ConditionPropertyMapping condition in conditionPropertyMappingList)
        conditionTypeMappingFragment.RemoveConditionProperty(condition);
      if (!entityType.Abstract)
        return;
      propertiesTypeMappingFragment.ClearConditions();
    }

    private static MappingFragment FindConditionTypeMappingFragment(
      EntitySet tableSet,
      MappingFragment propertiesTypeMappingFragment,
      EntityTypeMapping conditionTypeMapping)
    {
      EntityType table = tableSet.ElementType;
      MappingFragment mappingFragment = conditionTypeMapping.MappingFragments.SingleOrDefault<MappingFragment>((Func<MappingFragment, bool>) (x => x.Table == table));
      if (mappingFragment == null)
      {
        mappingFragment = EntityMappingOperations.CreateTypeMappingFragment(conditionTypeMapping, propertiesTypeMappingFragment, tableSet);
        mappingFragment.SetIsConditionOnlyFragment(true);
        if (propertiesTypeMappingFragment.GetDefaultDiscriminator() != null)
        {
          mappingFragment.SetDefaultDiscriminator(propertiesTypeMappingFragment.GetDefaultDiscriminator());
          propertiesTypeMappingFragment.RemoveDefaultDiscriminatorAnnotation();
        }
      }
      return mappingFragment;
    }

    private EntityTypeMapping FindConditionTypeMapping(
      EntityType entityType,
      bool requiresSplit,
      EntityTypeMapping propertiesTypeMapping)
    {
      EntityTypeMapping typeMapping = propertiesTypeMapping;
      if (requiresSplit)
      {
        if (!entityType.Abstract)
        {
          typeMapping = propertiesTypeMapping.Clone();
          typeMapping.RemoveIsOfType(typeMapping.EntityType);
          this._databaseMapping.GetEntitySetMappings().Single<EntitySetMapping>((Func<EntitySetMapping, bool>) (esm => esm.EntityTypeMappings.Contains(propertiesTypeMapping))).AddTypeMapping(typeMapping);
        }
        propertiesTypeMapping.MappingFragments.Each<MappingFragment>((Action<MappingFragment>) (tmf => tmf.ClearConditions()));
      }
      return typeMapping;
    }

    private bool DetermineRequiresIsTypeOf(
      TableMapping tableMapping,
      EntitySet entitySet,
      EntityType entityType)
    {
      if (!entityType.IsRootOfSet(tableMapping.EntityTypes.GetEntityTypes(entitySet)))
        return false;
      if (tableMapping.EntityTypes.GetEntityTypes(entitySet).Count<EntityType>() <= 1 || !tableMapping.EntityTypes.GetEntityTypes(entitySet).Any<EntityType>((Func<EntityType, bool>) (et =>
      {
        if (et != entityType)
          return !et.Abstract;
        return false;
      })))
        return this._tableMappings.Values.Any<TableMapping>((Func<TableMapping, bool>) (tm =>
        {
          if (tm != tableMapping)
            return tm.Table.ForeignKeyBuilders.Any<ForeignKeyBuilder>((Func<ForeignKeyBuilder, bool>) (fk =>
            {
              if (fk.GetIsTypeConstraint())
                return fk.PrincipalTable == tableMapping.Table;
              return false;
            }));
          return false;
        }));
      return true;
    }

    private static bool DetermineRequiresSplitEntityTypeMapping(
      TableMapping tableMapping,
      EntityType entityType,
      bool requiresIsTypeOf)
    {
      if (requiresIsTypeOf)
        return EntityMappingService.HasConditions(tableMapping, entityType);
      return false;
    }

    private bool FindPropertyEntityTypeMapping(
      TableMapping tableMapping,
      EntitySet entitySet,
      EntityType entityType,
      bool requiresIsTypeOf,
      out EntityTypeMapping entityTypeMapping,
      out MappingFragment fragment)
    {
      entityTypeMapping = (EntityTypeMapping) null;
      fragment = (MappingFragment) null;
      var data = this._databaseMapping.GetEntityTypeMappings(entityType).SelectMany((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments), (etm, tmf) => new
      {
        etm = etm,
        tmf = tmf
      }).Where(_param1 => _param1.tmf.Table == tableMapping.Table).Select(_param0 => new
      {
        TypeMapping = _param0.etm,
        Fragment = _param0.tmf
      }).SingleOrDefault();
      if (data == null)
        return false;
      entityTypeMapping = data.TypeMapping;
      fragment = data.Fragment;
      if (requiresIsTypeOf || !entityType.Abstract)
        return true;
      this.RemoveFragment(entitySet, data.TypeMapping, data.Fragment);
      return false;
    }

    private void RemoveFragment(
      EntitySet entitySet,
      EntityTypeMapping entityTypeMapping,
      MappingFragment fragment)
    {
      EdmProperty defaultDiscriminator = fragment.GetDefaultDiscriminator();
      if (defaultDiscriminator != null && entityTypeMapping.EntityType.BaseType != null && !entityTypeMapping.EntityType.Abstract)
      {
        ColumnMapping columnMapping = this._tableMappings[fragment.Table].ColumnMappings.SingleOrDefault<ColumnMapping>((Func<ColumnMapping, bool>) (cm => cm.Column == defaultDiscriminator));
        if (columnMapping != null)
        {
          PropertyMappingSpecification mappingSpecification = columnMapping.PropertyMappings.SingleOrDefault<PropertyMappingSpecification>((Func<PropertyMappingSpecification, bool>) (pm => pm.EntityType == entityTypeMapping.EntityType));
          if (mappingSpecification != null)
            columnMapping.PropertyMappings.Remove(mappingSpecification);
        }
        defaultDiscriminator.Nullable = true;
      }
      if (entityTypeMapping.EntityType.Abstract)
      {
        foreach (ColumnMapping columnMapping in this._tableMappings[fragment.Table].ColumnMappings.Where<ColumnMapping>((Func<ColumnMapping, bool>) (cm => cm.PropertyMappings.All<PropertyMappingSpecification>((Func<PropertyMappingSpecification, bool>) (pm => pm.EntityType == entityTypeMapping.EntityType)))))
          fragment.Table.RemoveMember((EdmMember) columnMapping.Column);
      }
      entityTypeMapping.RemoveFragment(fragment);
      if (entityTypeMapping.MappingFragments.Any<MappingFragment>())
        return;
      this._databaseMapping.GetEntitySetMapping(entitySet).RemoveTypeMapping(entityTypeMapping);
    }

    private static void RemoveRedundantDefaultDiscriminators(TableMapping tableMapping)
    {
      foreach (EntitySet entitySet1 in tableMapping.EntityTypes.GetEntitySets())
      {
        EntitySet entitySet = entitySet1;
        tableMapping.ColumnMappings.SelectMany((Func<ColumnMapping, IEnumerable<PropertyMappingSpecification>>) (cm => (IEnumerable<PropertyMappingSpecification>) cm.PropertyMappings), (cm, pm) => new
        {
          cm = cm,
          pm = pm
        }).Where(_param1 => _param1.cm.PropertyMappings.Where<PropertyMappingSpecification>((Func<PropertyMappingSpecification, bool>) (pm1 => tableMapping.EntityTypes.GetEntityTypes(entitySet).Contains<EntityType>(pm1.EntityType))).Count<PropertyMappingSpecification>((Func<PropertyMappingSpecification, bool>) (pms => pms.IsDefaultDiscriminatorCondition)) == 1).Select(_param0 => new
        {
          ColumnMapping = _param0.cm,
          PropertyMapping = _param0.pm
        }).ToArray().Each(x =>
        {
          x.PropertyMapping.Conditions.Clear();
          if (x.PropertyMapping.PropertyPath != null)
            return;
          x.ColumnMapping.PropertyMappings.Remove(x.PropertyMapping);
        });
      }
    }

    private static bool HasConditions(TableMapping tableMapping, EntityType entityType)
    {
      return tableMapping.ColumnMappings.SelectMany<ColumnMapping, PropertyMappingSpecification>((Func<ColumnMapping, IEnumerable<PropertyMappingSpecification>>) (cm => (IEnumerable<PropertyMappingSpecification>) cm.PropertyMappings)).Any<PropertyMappingSpecification>((Func<PropertyMappingSpecification, bool>) (pm =>
      {
        if (pm.EntityType == entityType)
          return pm.Conditions.Count > 0;
        return false;
      }));
    }

    private static bool IsRootTypeMapping(
      Dictionary<EntityType, EntityTypeMapping> rootMappings,
      EntityType entityType,
      IList<EdmProperty> propertyPath)
    {
      for (EntityType baseType = (EntityType) entityType.BaseType; baseType != null; baseType = (EntityType) baseType.BaseType)
      {
        EntityTypeMapping entityTypeMapping;
        if (rootMappings.TryGetValue(baseType, out entityTypeMapping))
          return entityTypeMapping.MappingFragments.SelectMany<MappingFragment, ColumnMappingBuilder>((Func<MappingFragment, IEnumerable<ColumnMappingBuilder>>) (etmf => etmf.ColumnMappings)).Any<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => pm.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) propertyPath)));
      }
      return false;
    }

    private TableMapping FindOrCreateTableMapping(EntityType table)
    {
      TableMapping tableMapping;
      if (!this._tableMappings.TryGetValue(table, out tableMapping))
      {
        tableMapping = new TableMapping(table);
        this._tableMappings.Add(table, tableMapping);
      }
      return tableMapping;
    }
  }
}
