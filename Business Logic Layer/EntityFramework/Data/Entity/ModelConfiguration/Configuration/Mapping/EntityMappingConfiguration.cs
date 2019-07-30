// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Mapping.EntityMappingConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.ModelConfiguration.Edm.Services;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration.Mapping
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class EntityMappingConfiguration
  {
    private readonly List<ValueConditionConfiguration> _valueConditions = new List<ValueConditionConfiguration>();
    private readonly List<NotNullConditionConfiguration> _notNullConditions = new List<NotNullConditionConfiguration>();
    private readonly Dictionary<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> _primitivePropertyConfigurations = new Dictionary<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>();
    private readonly IDictionary<string, object> _annotations = (IDictionary<string, object>) new Dictionary<string, object>();
    private DatabaseName _tableName;
    private List<PropertyPath> _properties;

    internal EntityMappingConfiguration()
    {
    }

    private EntityMappingConfiguration(EntityMappingConfiguration source)
    {
      this._tableName = source._tableName;
      this.MapInheritedProperties = source.MapInheritedProperties;
      if (source._properties != null)
        this._properties = new List<PropertyPath>((IEnumerable<PropertyPath>) source._properties);
      this._valueConditions.AddRange(source._valueConditions.Select<ValueConditionConfiguration, ValueConditionConfiguration>((Func<ValueConditionConfiguration, ValueConditionConfiguration>) (c => c.Clone(this))));
      this._notNullConditions.AddRange(source._notNullConditions.Select<NotNullConditionConfiguration, NotNullConditionConfiguration>((Func<NotNullConditionConfiguration, NotNullConditionConfiguration>) (c => c.Clone(this))));
      source._primitivePropertyConfigurations.Each<KeyValuePair<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>>((Action<KeyValuePair<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>>) (c => this._primitivePropertyConfigurations.Add(c.Key, c.Value.Clone())));
      foreach (KeyValuePair<string, object> annotation in (IEnumerable<KeyValuePair<string, object>>) source._annotations)
        this._annotations.Add(annotation);
    }

    internal virtual EntityMappingConfiguration Clone()
    {
      return new EntityMappingConfiguration(this);
    }

    public bool MapInheritedProperties { get; set; }

    public DatabaseName TableName
    {
      get
      {
        return this._tableName;
      }
      set
      {
        this._tableName = value;
      }
    }

    public IDictionary<string, object> Annotations
    {
      get
      {
        return this._annotations;
      }
    }

    public virtual void SetAnnotation(string name, object value)
    {
      if (!name.IsValidUndottedName())
        throw new ArgumentException(Strings.BadAnnotationName((object) name));
      this._annotations[name] = value;
    }

    internal List<PropertyPath> Properties
    {
      get
      {
        return this._properties;
      }
      set
      {
        if (this._properties == null)
          this._properties = new List<PropertyPath>();
        value.Each<PropertyPath>(new Action<PropertyPath>(this.Property));
      }
    }

    internal IDictionary<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> PrimitivePropertyConfigurations
    {
      get
      {
        return (IDictionary<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>) this._primitivePropertyConfigurations;
      }
    }

    internal TPrimitivePropertyConfiguration Property<TPrimitivePropertyConfiguration>(
      PropertyPath propertyPath,
      Func<TPrimitivePropertyConfiguration> primitivePropertyConfigurationCreator)
      where TPrimitivePropertyConfiguration : System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration
    {
      if (this._properties == null)
        this._properties = new List<PropertyPath>();
      this.Property(propertyPath);
      System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration propertyConfiguration;
      if (!this._primitivePropertyConfigurations.TryGetValue(propertyPath, out propertyConfiguration))
        this._primitivePropertyConfigurations.Add(propertyPath, propertyConfiguration = (System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration) primitivePropertyConfigurationCreator());
      return (TPrimitivePropertyConfiguration) propertyConfiguration;
    }

    private void Property(PropertyPath property)
    {
      if (this._properties.Where<PropertyPath>((Func<PropertyPath, bool>) (pp => pp.SequenceEqual<PropertyInfo>((IEnumerable<PropertyInfo>) property))).Any<PropertyPath>())
        return;
      this._properties.Add(property);
    }

    public List<ValueConditionConfiguration> ValueConditions
    {
      get
      {
        return this._valueConditions;
      }
    }

    public void AddValueCondition(ValueConditionConfiguration valueCondition)
    {
      ValueConditionConfiguration conditionConfiguration = this.ValueConditions.SingleOrDefault<ValueConditionConfiguration>((Func<ValueConditionConfiguration, bool>) (vc => vc.Discriminator.Equals(valueCondition.Discriminator, StringComparison.Ordinal)));
      if (conditionConfiguration == null)
        this.ValueConditions.Add(valueCondition);
      else
        conditionConfiguration.Value = valueCondition.Value;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    public List<NotNullConditionConfiguration> NullabilityConditions
    {
      get
      {
        return this._notNullConditions;
      }
      set
      {
        value.Each<NotNullConditionConfiguration>(new Action<NotNullConditionConfiguration>(this.AddNullabilityCondition));
      }
    }

    public void AddNullabilityCondition(
      NotNullConditionConfiguration notNullConditionConfiguration)
    {
      if (this.NullabilityConditions.Contains(notNullConditionConfiguration))
        return;
      this.NullabilityConditions.Add(notNullConditionConfiguration);
    }

    public bool MapsAnyInheritedProperties(EntityType entityType)
    {
      HashSet<EdmPropertyPath> properties = new HashSet<EdmPropertyPath>();
      if (this.Properties != null)
        this.Properties.Each<PropertyPath>((Action<PropertyPath>) (p => properties.AddRange<EdmPropertyPath>(EntityMappingConfiguration.PropertyPathToEdmPropertyPath(p, entityType))));
      if (!this.MapInheritedProperties)
        return properties.Any<EdmPropertyPath>((Func<EdmPropertyPath, bool>) (x =>
        {
          if (!entityType.KeyProperties().Contains<EdmProperty>(x.First<EdmProperty>()))
            return !entityType.DeclaredProperties.Contains(x.First<EdmProperty>());
          return false;
        }));
      return true;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public void Configure(
      DbDatabaseMapping databaseMapping,
      ICollection<EntitySet> entitySets,
      DbProviderManifest providerManifest,
      EntityType entityType,
      ref EntityTypeMapping entityTypeMapping,
      bool isMappingAnyInheritedProperty,
      int configurationIndex,
      int configurationCount,
      IDictionary<string, object> commonAnnotations)
    {
      EntityType baseType = (EntityType) entityType.BaseType;
      bool flag = baseType == null && configurationIndex == 0;
      MappingFragment typeMappingFragment1 = this.FindOrCreateTypeMappingFragment(databaseMapping, ref entityTypeMapping, configurationIndex, entityType, providerManifest);
      EntityType table = typeMappingFragment1.Table;
      bool isTableSharing;
      EntityType createTargetTable = this.FindOrCreateTargetTable(databaseMapping, typeMappingFragment1, entityType, table, out isTableSharing);
      bool isSharingTableWithBase = this.DiscoverIsSharingWithBase(databaseMapping, entityType, createTargetTable);
      HashSet<EdmPropertyPath> contain = this.DiscoverAllMappingsToContain(databaseMapping, entityType, createTargetTable, isSharingTableWithBase);
      List<ColumnMappingBuilder> list = typeMappingFragment1.ColumnMappings.ToList<ColumnMappingBuilder>();
      foreach (EdmPropertyPath edmPropertyPath in contain)
      {
        EdmPropertyPath propertyPath = edmPropertyPath;
        ColumnMappingBuilder columnMappingBuilder = typeMappingFragment1.ColumnMappings.SingleOrDefault<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => pm.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) propertyPath)));
        if (columnMappingBuilder == null)
          throw Error.EntityMappingConfiguration_DuplicateMappedProperty((object) entityType.Name, (object) propertyPath.ToString());
        list.Remove(columnMappingBuilder);
      }
      if (!flag)
      {
        bool isSplitting;
        EntityType parentTable = EntityMappingConfiguration.FindParentTable(databaseMapping, table, entityTypeMapping, createTargetTable, isMappingAnyInheritedProperty, configurationIndex, configurationCount, out isSplitting);
        if (parentTable != null)
          DatabaseOperations.AddTypeConstraint(databaseMapping.Database, entityType, parentTable, createTargetTable, isSplitting);
      }
      if (table != createTargetTable)
      {
        if (this.Properties == null)
        {
          AssociationMappingOperations.MoveAllDeclaredAssociationSetMappings(databaseMapping, entityType, table, createTargetTable, !isTableSharing);
          ForeignKeyPrimitiveOperations.MoveAllDeclaredForeignKeyConstraintsForPrimaryKeyColumns(entityType, table, createTargetTable);
        }
        if (isMappingAnyInheritedProperty)
        {
          IEnumerable<EntityType> baseTables = databaseMapping.GetEntityTypeMappings(baseType).SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments)).Select<MappingFragment, EntityType>((Func<MappingFragment, EntityType>) (mf => mf.Table));
          AssociationSetMapping associationSetMapping = databaseMapping.EntityContainerMappings.SelectMany<EntityContainerMapping, AssociationSetMapping>((Func<EntityContainerMapping, IEnumerable<AssociationSetMapping>>) (asm => asm.AssociationSetMappings)).FirstOrDefault<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (a =>
          {
            if (!baseTables.Contains<EntityType>(a.Table))
              return false;
            if (baseType != a.AssociationSet.ElementType.SourceEnd.GetEntityType())
              return baseType == a.AssociationSet.ElementType.TargetEnd.GetEntityType();
            return true;
          }));
          if (associationSetMapping != null)
          {
            AssociationType elementType = associationSetMapping.AssociationSet.ElementType;
            throw Error.EntityMappingConfiguration_TPCWithIAsOnNonLeafType((object) elementType.Name, (object) elementType.SourceEnd.GetEntityType().Name, (object) elementType.TargetEnd.GetEntityType().Name);
          }
          ForeignKeyPrimitiveOperations.CopyAllForeignKeyConstraintsForPrimaryKeyColumns(databaseMapping.Database, table, createTargetTable);
        }
      }
      if (list.Any<ColumnMappingBuilder>())
      {
        EntityType extraTable = (EntityType) null;
        if (configurationIndex < configurationCount - 1)
        {
          ColumnMappingBuilder pm = list.First<ColumnMappingBuilder>();
          extraTable = EntityMappingConfiguration.FindTableForTemporaryExtraPropertyMapping(databaseMapping, entityType, table, createTargetTable, pm);
          MappingFragment typeMappingFragment2 = EntityMappingOperations.CreateTypeMappingFragment(entityTypeMapping, typeMappingFragment1, databaseMapping.Database.GetEntitySet(extraTable));
          bool requiresUpdate = extraTable != table;
          foreach (ColumnMappingBuilder propertyMappingBuilder in list)
            EntityMappingOperations.MovePropertyMapping(databaseMapping, (IEnumerable<EntitySet>) entitySets, typeMappingFragment1, typeMappingFragment2, propertyMappingBuilder, requiresUpdate, true);
        }
        else
        {
          EntityType unmappedTable = (EntityType) null;
          foreach (ColumnMappingBuilder columnMappingBuilder in list)
          {
            extraTable = EntityMappingConfiguration.FindTableForExtraPropertyMapping(databaseMapping, entityType, table, createTargetTable, ref unmappedTable, columnMappingBuilder);
            MappingFragment mappingFragment = entityTypeMapping.MappingFragments.SingleOrDefault<MappingFragment>((Func<MappingFragment, bool>) (tmf => tmf.Table == extraTable));
            if (mappingFragment == null)
            {
              mappingFragment = EntityMappingOperations.CreateTypeMappingFragment(entityTypeMapping, typeMappingFragment1, databaseMapping.Database.GetEntitySet(extraTable));
              mappingFragment.SetIsUnmappedPropertiesFragment(true);
            }
            if (extraTable == table)
              EntityMappingConfiguration.CopyDefaultDiscriminator(typeMappingFragment1, mappingFragment);
            bool requiresUpdate = extraTable != table;
            EntityMappingOperations.MovePropertyMapping(databaseMapping, (IEnumerable<EntitySet>) entitySets, typeMappingFragment1, mappingFragment, columnMappingBuilder, requiresUpdate, true);
          }
        }
      }
      EntityMappingOperations.UpdatePropertyMappings(databaseMapping, (IEnumerable<EntitySet>) entitySets, table, typeMappingFragment1, !isTableSharing);
      this.ConfigureDefaultDiscriminator(entityType, typeMappingFragment1);
      this.ConfigureConditions(databaseMapping, entityType, typeMappingFragment1, providerManifest);
      EntityMappingOperations.UpdateConditions(databaseMapping.Database, table, typeMappingFragment1);
      ForeignKeyPrimitiveOperations.UpdatePrincipalTables(databaseMapping, entityType, table, createTargetTable, isMappingAnyInheritedProperty);
      EntityMappingConfiguration.CleanupUnmappedArtifacts(databaseMapping, table);
      EntityMappingConfiguration.CleanupUnmappedArtifacts(databaseMapping, createTargetTable);
      EntityMappingConfiguration.ConfigureAnnotations((EdmType) createTargetTable, commonAnnotations);
      EntityMappingConfiguration.ConfigureAnnotations((EdmType) createTargetTable, this._annotations);
      createTargetTable.SetConfiguration((object) this);
    }

    private static void ConfigureAnnotations(
      EdmType toTable,
      IDictionary<string, object> annotations)
    {
      foreach (KeyValuePair<string, object> annotation1 in (IEnumerable<KeyValuePair<string, object>>) annotations)
      {
        KeyValuePair<string, object> annotation = annotation1;
        string name = "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:" + annotation.Key;
        MetadataProperty metadataProperty = toTable.Annotations.FirstOrDefault<MetadataProperty>((Func<MetadataProperty, bool>) (a =>
        {
          if (a.Name == name)
            return !object.Equals(a.Value, annotation.Value);
          return false;
        }));
        if (metadataProperty != null)
          throw new InvalidOperationException(Strings.ConflictingTypeAnnotation((object) annotation.Key, annotation.Value, metadataProperty.Value, (object) toTable.Name));
        toTable.AddAnnotation(name, annotation.Value);
      }
    }

    internal void ConfigurePropertyMappings(
      IList<Tuple<ColumnMappingBuilder, EntityType>> propertyMappings,
      DbProviderManifest providerManifest,
      bool allowOverride = false)
    {
      foreach (KeyValuePair<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> propertyConfiguration in this._primitivePropertyConfigurations)
      {
        PropertyPath propertyPath = propertyConfiguration.Key;
        propertyConfiguration.Value.Configure(propertyMappings.Where<Tuple<ColumnMappingBuilder, EntityType>>((Func<Tuple<ColumnMappingBuilder, EntityType>, bool>) (pm =>
        {
          if (propertyPath.Equals(new PropertyPath(pm.Item1.PropertyPath.Skip<EdmProperty>(pm.Item1.PropertyPath.Count - propertyPath.Count).Select<EdmProperty, PropertyInfo>((Func<EdmProperty, PropertyInfo>) (p => p.GetClrPropertyInfo())))))
            return object.Equals((object) this.TableName, (object) pm.Item2.GetTableName());
          return false;
        })), providerManifest, allowOverride, true);
      }
    }

    private void ConfigureDefaultDiscriminator(EntityType entityType, MappingFragment fragment)
    {
      if (!this.ValueConditions.Any<ValueConditionConfiguration>() && !this.NullabilityConditions.Any<NotNullConditionConfiguration>())
        return;
      EdmProperty edmProperty = fragment.RemoveDefaultDiscriminatorCondition();
      if (edmProperty == null || entityType.BaseType == null)
        return;
      edmProperty.Nullable = true;
    }

    private static void CopyDefaultDiscriminator(
      MappingFragment fromFragment,
      MappingFragment toFragment)
    {
      EdmProperty discriminatorColumn = fromFragment.GetDefaultDiscriminator();
      if (discriminatorColumn == null)
        return;
      ConditionPropertyMapping conditionPropertyMapping = fromFragment.ColumnConditions.SingleOrDefault<ConditionPropertyMapping>((Func<ConditionPropertyMapping, bool>) (cc => cc.Column == discriminatorColumn));
      if (conditionPropertyMapping == null)
        return;
      toFragment.AddDiscriminatorCondition(conditionPropertyMapping.Column, conditionPropertyMapping.Value);
      toFragment.SetDefaultDiscriminator(conditionPropertyMapping.Column);
    }

    private static EntityType FindTableForTemporaryExtraPropertyMapping(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      EntityType fromTable,
      EntityType toTable,
      ColumnMappingBuilder pm)
    {
      return fromTable != toTable ? (entityType.BaseType != null ? EntityMappingConfiguration.FindBaseTableForExtraPropertyMapping(databaseMapping, entityType, pm) ?? fromTable : fromTable) : databaseMapping.Database.AddTable(entityType.Name, fromTable);
    }

    private static EntityType FindTableForExtraPropertyMapping(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      EntityType fromTable,
      EntityType toTable,
      ref EntityType unmappedTable,
      ColumnMappingBuilder pm)
    {
      EntityType entityType1 = EntityMappingConfiguration.FindBaseTableForExtraPropertyMapping(databaseMapping, entityType, pm);
      if (entityType1 == null)
      {
        if (fromTable != toTable && entityType.BaseType == null)
          return fromTable;
        if (unmappedTable == null)
          unmappedTable = databaseMapping.Database.AddTable(fromTable.Name, fromTable);
        entityType1 = unmappedTable;
      }
      return entityType1;
    }

    private static EntityType FindBaseTableForExtraPropertyMapping(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      ColumnMappingBuilder pm)
    {
      EntityType baseType = (EntityType) entityType.BaseType;
      for (MappingFragment mappingFragment = (MappingFragment) null; baseType != null && mappingFragment == null; baseType = (EntityType) baseType.BaseType)
      {
        EntityTypeMapping entityTypeMapping = databaseMapping.GetEntityTypeMapping(baseType);
        if (entityTypeMapping != null)
        {
          mappingFragment = entityTypeMapping.MappingFragments.SingleOrDefault<MappingFragment>((Func<MappingFragment, bool>) (f => f.ColumnMappings.Any<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (bpm => bpm.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) pm.PropertyPath)))));
          if (mappingFragment != null)
            return mappingFragment.Table;
        }
      }
      return (EntityType) null;
    }

    private bool DiscoverIsSharingWithBase(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      EntityType toTable)
    {
      bool flag1 = false;
      if (entityType.BaseType != null)
      {
        EdmType baseType = entityType.BaseType;
        bool flag2 = false;
        for (; baseType != null && !flag1; baseType = baseType.BaseType)
        {
          IList<EntityTypeMapping> entityTypeMappings = databaseMapping.GetEntityTypeMappings((EntityType) baseType);
          if (entityTypeMappings.Any<EntityTypeMapping>())
          {
            flag1 = entityTypeMappings.SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (m => (IEnumerable<MappingFragment>) m.MappingFragments)).Any<MappingFragment>((Func<MappingFragment, bool>) (tmf => tmf.Table == toTable));
            flag2 = true;
          }
        }
        if (!flag2)
          flag1 = this.TableName == null || string.IsNullOrWhiteSpace(this.TableName.Name);
      }
      return flag1;
    }

    private static EntityType FindParentTable(
      DbDatabaseMapping databaseMapping,
      EntityType fromTable,
      EntityTypeMapping entityTypeMapping,
      EntityType toTable,
      bool isMappingInheritedProperties,
      int configurationIndex,
      int configurationCount,
      out bool isSplitting)
    {
      EntityType entityType = (EntityType) null;
      isSplitting = false;
      if ((entityTypeMapping.UsesOtherTables(toTable) || configurationCount > 1) && configurationIndex != 0)
      {
        entityType = entityTypeMapping.GetPrimaryTable();
        isSplitting = true;
      }
      if (entityType == null && fromTable != toTable && !isMappingInheritedProperties)
      {
        for (EdmType baseType = entityTypeMapping.EntityType.BaseType; baseType != null && entityType == null; baseType = baseType.BaseType)
        {
          EntityTypeMapping entityTypeMapping1 = databaseMapping.GetEntityTypeMappings((EntityType) baseType).FirstOrDefault<EntityTypeMapping>();
          if (entityTypeMapping1 != null)
            entityType = entityTypeMapping1.GetPrimaryTable();
        }
      }
      return entityType;
    }

    private MappingFragment FindOrCreateTypeMappingFragment(
      DbDatabaseMapping databaseMapping,
      ref EntityTypeMapping entityTypeMapping,
      int configurationIndex,
      EntityType entityType,
      DbProviderManifest providerManifest)
    {
      if (entityTypeMapping == null)
      {
        new TableMappingGenerator(providerManifest).Generate(entityType, databaseMapping);
        entityTypeMapping = databaseMapping.GetEntityTypeMapping(entityType);
        configurationIndex = 0;
      }
      MappingFragment mappingFragment;
      if (configurationIndex < entityTypeMapping.MappingFragments.Count)
      {
        mappingFragment = entityTypeMapping.MappingFragments[configurationIndex];
      }
      else
      {
        if (this.MapInheritedProperties)
          throw Error.EntityMappingConfiguration_DuplicateMapInheritedProperties((object) entityType.Name);
        if (this.Properties == null)
          throw Error.EntityMappingConfiguration_DuplicateMappedProperties((object) entityType.Name);
        this.Properties.Each<PropertyPath>((Action<PropertyPath>) (p =>
        {
          if (EntityMappingConfiguration.PropertyPathToEdmPropertyPath(p, entityType).Any<EdmPropertyPath>((Func<EdmPropertyPath, bool>) (pp => !entityType.KeyProperties().Contains<EdmProperty>(pp.First<EdmProperty>()))))
            throw Error.EntityMappingConfiguration_DuplicateMappedProperty((object) entityType.Name, (object) p.ToString());
        }));
        EntityType table = entityTypeMapping.MappingFragments[0].Table;
        EntityType entityType1 = databaseMapping.Database.AddTable(table.Name, table);
        mappingFragment = EntityMappingOperations.CreateTypeMappingFragment(entityTypeMapping, entityTypeMapping.MappingFragments[0], databaseMapping.Database.GetEntitySet(entityType1));
      }
      return mappingFragment;
    }

    private EntityType FindOrCreateTargetTable(
      DbDatabaseMapping databaseMapping,
      MappingFragment fragment,
      EntityType entityType,
      EntityType fromTable,
      out bool isTableSharing)
    {
      isTableSharing = false;
      EntityType entityType1;
      if (this.TableName == null)
      {
        entityType1 = fragment.Table;
      }
      else
      {
        entityType1 = databaseMapping.Database.FindTableByName(this.TableName) ?? (entityType.BaseType != null ? databaseMapping.Database.AddTable(this.TableName.Name, fromTable) : fragment.Table);
        isTableSharing = EntityMappingConfiguration.UpdateColumnNamesForTableSharing(databaseMapping, entityType, entityType1, fragment);
        fragment.TableSet = databaseMapping.Database.GetEntitySet(entityType1);
        foreach (ColumnMappingBuilder columnMappingBuilder in fragment.ColumnMappings.Where<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (cm => cm.ColumnProperty.IsPrimaryKeyColumn)))
        {
          ColumnMappingBuilder columnMapping = columnMappingBuilder;
          EdmProperty edmProperty = entityType1.Properties.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (c => string.Equals(c.Name, columnMapping.ColumnProperty.Name, StringComparison.Ordinal)));
          columnMapping.ColumnProperty = edmProperty ?? columnMapping.ColumnProperty;
        }
        entityType1.SetTableName(this.TableName);
      }
      return entityType1;
    }

    private HashSet<EdmPropertyPath> DiscoverAllMappingsToContain(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      EntityType toTable,
      bool isSharingTableWithBase)
    {
      HashSet<EdmPropertyPath> mappingsToContain = new HashSet<EdmPropertyPath>();
      entityType.KeyProperties().Each<EdmProperty>((Action<EdmProperty>) (p => mappingsToContain.AddRange<EdmPropertyPath>((IEnumerable<EdmPropertyPath>) p.ToPropertyPathList())));
      if (this.MapInheritedProperties)
        entityType.Properties.Except<EdmProperty>((IEnumerable<EdmProperty>) entityType.DeclaredProperties).Each<EdmProperty>((Action<EdmProperty>) (p => mappingsToContain.AddRange<EdmPropertyPath>((IEnumerable<EdmPropertyPath>) p.ToPropertyPathList())));
      if (isSharingTableWithBase)
      {
        HashSet<EdmPropertyPath> baseMappingsToContain = new HashSet<EdmPropertyPath>();
        EntityType baseType = (EntityType) entityType.BaseType;
        EntityTypeMapping entityTypeMapping = (EntityTypeMapping) null;
        MappingFragment mappingFragment = (MappingFragment) null;
        for (; baseType != null && entityTypeMapping == null; baseType = (EntityType) baseType.BaseType)
        {
          entityTypeMapping = databaseMapping.GetEntityTypeMapping((EntityType) entityType.BaseType);
          if (entityTypeMapping != null)
            mappingFragment = entityTypeMapping.MappingFragments.SingleOrDefault<MappingFragment>((Func<MappingFragment, bool>) (tmf => tmf.Table == toTable));
          if (mappingFragment == null)
            baseType.DeclaredProperties.Each<EdmProperty>((Action<EdmProperty>) (p => baseMappingsToContain.AddRange<EdmPropertyPath>((IEnumerable<EdmPropertyPath>) p.ToPropertyPathList())));
        }
        if (mappingFragment != null)
        {
          foreach (ColumnMappingBuilder columnMapping in mappingFragment.ColumnMappings)
            mappingsToContain.Add(new EdmPropertyPath((IEnumerable<EdmProperty>) columnMapping.PropertyPath));
        }
        mappingsToContain.AddRange<EdmPropertyPath>((IEnumerable<EdmPropertyPath>) baseMappingsToContain);
      }
      if (this.Properties == null)
        entityType.DeclaredProperties.Each<EdmProperty>((Action<EdmProperty>) (p => mappingsToContain.AddRange<EdmPropertyPath>((IEnumerable<EdmPropertyPath>) p.ToPropertyPathList())));
      else
        this.Properties.Each<PropertyPath>((Action<PropertyPath>) (p => mappingsToContain.AddRange<EdmPropertyPath>(EntityMappingConfiguration.PropertyPathToEdmPropertyPath(p, entityType))));
      return mappingsToContain;
    }

    private void ConfigureConditions(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      MappingFragment fragment,
      DbProviderManifest providerManifest)
    {
      if (!this.ValueConditions.Any<ValueConditionConfiguration>() && !this.NullabilityConditions.Any<NotNullConditionConfiguration>())
        return;
      fragment.ClearConditions();
      foreach (ValueConditionConfiguration valueCondition in this.ValueConditions)
        valueCondition.Configure(databaseMapping, fragment, entityType, providerManifest);
      foreach (NotNullConditionConfiguration nullabilityCondition in this.NullabilityConditions)
        nullabilityCondition.Configure(databaseMapping, fragment, entityType);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    internal static void CleanupUnmappedArtifacts(
      DbDatabaseMapping databaseMapping,
      EntityType table)
    {
      AssociationSetMapping[] array1 = databaseMapping.EntityContainerMappings.SelectMany<EntityContainerMapping, AssociationSetMapping>((Func<EntityContainerMapping, IEnumerable<AssociationSetMapping>>) (ecm => ecm.AssociationSetMappings)).Where<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm => asm.Table == table)).ToArray<AssociationSetMapping>();
      MappingFragment[] array2 = databaseMapping.EntityContainerMappings.SelectMany<EntityContainerMapping, EntitySetMapping>((Func<EntityContainerMapping, IEnumerable<EntitySetMapping>>) (ecm => ecm.EntitySetMappings)).SelectMany<EntitySetMapping, EntityTypeMapping>((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (esm => (IEnumerable<EntityTypeMapping>) esm.EntityTypeMappings)).SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments)).Where<MappingFragment>((Func<MappingFragment, bool>) (f => f.Table == table)).ToArray<MappingFragment>();
      if (!((IEnumerable<AssociationSetMapping>) array1).Any<AssociationSetMapping>() && !((IEnumerable<MappingFragment>) array2).Any<MappingFragment>())
      {
        databaseMapping.Database.RemoveEntityType(table);
        ((IEnumerable<AssociationType>) databaseMapping.Database.AssociationTypes.Where<AssociationType>((Func<AssociationType, bool>) (t =>
        {
          if (t.SourceEnd.GetEntityType() != table)
            return t.TargetEnd.GetEntityType() == table;
          return true;
        })).ToArray<AssociationType>()).Each<AssociationType>((Action<AssociationType>) (t => databaseMapping.Database.RemoveAssociationType(t)));
      }
      else
      {
        foreach (EdmProperty edmProperty in table.Properties.ToArray<EdmProperty>())
        {
          EdmProperty column = edmProperty;
          if (((IEnumerable<MappingFragment>) array2).SelectMany<MappingFragment, ColumnMappingBuilder>((Func<MappingFragment, IEnumerable<ColumnMappingBuilder>>) (f => f.ColumnMappings)).All<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => pm.ColumnProperty != column)) && ((IEnumerable<MappingFragment>) array2).SelectMany<MappingFragment, ConditionPropertyMapping>((Func<MappingFragment, IEnumerable<ConditionPropertyMapping>>) (f => f.ColumnConditions)).All<ConditionPropertyMapping>((Func<ConditionPropertyMapping, bool>) (cc => cc.Column != column)) && (((IEnumerable<AssociationSetMapping>) array1).SelectMany<AssociationSetMapping, ScalarPropertyMapping>((Func<AssociationSetMapping, IEnumerable<ScalarPropertyMapping>>) (am => (IEnumerable<ScalarPropertyMapping>) am.SourceEndMapping.PropertyMappings)).All<ScalarPropertyMapping>((Func<ScalarPropertyMapping, bool>) (pm => pm.Column != column)) && ((IEnumerable<AssociationSetMapping>) array1).SelectMany<AssociationSetMapping, ScalarPropertyMapping>((Func<AssociationSetMapping, IEnumerable<ScalarPropertyMapping>>) (am => (IEnumerable<ScalarPropertyMapping>) am.SourceEndMapping.PropertyMappings)).All<ScalarPropertyMapping>((Func<ScalarPropertyMapping, bool>) (pm => pm.Column != column))))
          {
            ForeignKeyPrimitiveOperations.RemoveAllForeignKeyConstraintsForColumn(table, column, databaseMapping);
            TablePrimitiveOperations.RemoveColumn(table, column);
          }
        }
        ((IEnumerable<ForeignKeyBuilder>) table.ForeignKeyBuilders.Where<ForeignKeyBuilder>((Func<ForeignKeyBuilder, bool>) (fk =>
        {
          if (fk.PrincipalTable == table)
            return fk.DependentColumns.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) table.KeyProperties);
          return false;
        })).ToArray<ForeignKeyBuilder>()).Each<ForeignKeyBuilder>(new Action<ForeignKeyBuilder>(table.RemoveForeignKey));
      }
    }

    internal static IEnumerable<EdmPropertyPath> PropertyPathToEdmPropertyPath(
      PropertyPath path,
      EntityType entityType)
    {
      List<EdmProperty> edmPropertyList = new List<EdmProperty>();
      StructuralType structuralType = (StructuralType) entityType;
      for (int i = 0; i < path.Count; ++i)
      {
        EdmProperty edmProperty = structuralType.Members.OfType<EdmProperty>().SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (p => p.GetClrPropertyInfo().IsSameAs(path[i])));
        if (edmProperty == null)
          throw Error.EntityMappingConfiguration_CannotMapIgnoredProperty((object) entityType.Name, (object) path.ToString());
        edmPropertyList.Add(edmProperty);
        if (edmProperty.IsComplexType)
          structuralType = (StructuralType) edmProperty.ComplexType;
      }
      EdmProperty property = edmPropertyList.Last<EdmProperty>();
      if (property.IsUnderlyingPrimitiveType)
        return (IEnumerable<EdmPropertyPath>) new EdmPropertyPath[1]
        {
          new EdmPropertyPath((IEnumerable<EdmProperty>) edmPropertyList)
        };
      if (property.IsComplexType)
      {
        edmPropertyList.Remove(property);
        return (IEnumerable<EdmPropertyPath>) property.ToPropertyPathList(edmPropertyList);
      }
      return (IEnumerable<EdmPropertyPath>) new EdmPropertyPath[1]
      {
        EdmPropertyPath.Empty
      };
    }

    private static List<EntityTypeMapping> FindAllTypeMappingsUsingTable(
      DbDatabaseMapping databaseMapping,
      EntityType toTable)
    {
      List<EntityTypeMapping> entityTypeMappingList = new List<EntityTypeMapping>();
      IList<EntityContainerMapping> containerMappings = databaseMapping.EntityContainerMappings;
      for (int index1 = 0; index1 < containerMappings.Count; ++index1)
      {
        List<EntitySetMapping> list = containerMappings[index1].EntitySetMappings.ToList<EntitySetMapping>();
        for (int index2 = 0; index2 < list.Count; ++index2)
        {
          ReadOnlyCollection<EntityTypeMapping> entityTypeMappings = list[index2].EntityTypeMappings;
          for (int index3 = 0; index3 < entityTypeMappings.Count; ++index3)
          {
            EntityTypeMapping entityTypeMapping = entityTypeMappings[index3];
            EntityTypeConfiguration configuration = entityTypeMapping.EntityType.GetConfiguration() as EntityTypeConfiguration;
            for (int index4 = 0; index4 < entityTypeMapping.MappingFragments.Count; ++index4)
            {
              bool flag = configuration != null && configuration.IsTableNameConfigured;
              if (!flag && entityTypeMapping.MappingFragments[index4].Table == toTable || flag && EntityMappingConfiguration.IsTableNameEqual(toTable, configuration.GetTableName()))
              {
                entityTypeMappingList.Add(entityTypeMapping);
                break;
              }
            }
          }
        }
      }
      return entityTypeMappingList;
    }

    private static bool IsTableNameEqual(EntityType table, DatabaseName otherTableName)
    {
      DatabaseName tableName = table.GetTableName();
      if (tableName != null)
        return otherTableName.Equals(tableName);
      if (otherTableName.Name.Equals(table.Name, StringComparison.Ordinal))
        return otherTableName.Schema == null;
      return false;
    }

    private static IEnumerable<AssociationType> FindAllOneToOneFKAssociationTypes(
      EdmModel model,
      EntityType entityType,
      EntityType candidateType)
    {
      List<AssociationType> associationTypeList = new List<AssociationType>();
      foreach (EntityContainer container in model.Containers)
      {
        ReadOnlyMetadataCollection<AssociationSet> associationSets = container.AssociationSets;
        for (int index = 0; index < associationSets.Count; ++index)
        {
          AssociationSet associationSet = associationSets[index];
          AssociationEndMember sourceEnd = associationSet.ElementType.SourceEnd;
          AssociationEndMember targetEnd = associationSet.ElementType.TargetEnd;
          EntityType entityType1 = sourceEnd.GetEntityType();
          EntityType entityType2 = targetEnd.GetEntityType();
          if (associationSet.ElementType.Constraint != null && sourceEnd.RelationshipMultiplicity == RelationshipMultiplicity.One && targetEnd.RelationshipMultiplicity == RelationshipMultiplicity.One && (entityType1 == entityType && entityType2 == candidateType || entityType2 == entityType && entityType1 == candidateType))
            associationTypeList.Add(associationSet.ElementType);
        }
      }
      return (IEnumerable<AssociationType>) associationTypeList;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private static bool UpdateColumnNamesForTableSharing(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      EntityType toTable,
      MappingFragment fragment)
    {
      List<EntityTypeMapping> mappingsUsingTable = EntityMappingConfiguration.FindAllTypeMappingsUsingTable(databaseMapping, toTable);
      Dictionary<EntityType, List<AssociationType>> dictionary = new Dictionary<EntityType, List<AssociationType>>();
      foreach (EntityTypeMapping entityTypeMapping in mappingsUsingTable)
      {
        EntityType entityType1 = entityTypeMapping.EntityType;
        if (entityType != entityType1)
        {
          IEnumerable<AssociationType> associationTypes = EntityMappingConfiguration.FindAllOneToOneFKAssociationTypes(databaseMapping.Model, entityType, entityType1);
          EntityType rootType = entityType1.GetRootType();
          if (!dictionary.ContainsKey(rootType))
            dictionary.Add(rootType, associationTypes.ToList<AssociationType>());
          else
            dictionary[rootType].AddRange(associationTypes);
        }
      }
      List<EntityType> source1 = new List<EntityType>();
      foreach (KeyValuePair<EntityType, List<AssociationType>> keyValuePair in dictionary)
      {
        if (keyValuePair.Key != entityType.GetRootType() && keyValuePair.Value.Count == 0)
          source1.Add(keyValuePair.Key);
      }
      if (source1.Count > 0 && source1.Count == dictionary.Count)
      {
        DatabaseName tableName = toTable.GetTableName();
        throw Error.EntityMappingConfiguration_InvalidTableSharing((object) entityType.Name, (object) source1.First<EntityType>().Name, tableName != null ? (object) tableName.Name : (object) databaseMapping.Database.GetEntitySet(toTable).Table);
      }
      IEnumerable<AssociationType> source2 = dictionary.Values.SelectMany<List<AssociationType>, AssociationType>((Func<List<AssociationType>, IEnumerable<AssociationType>>) (l => (IEnumerable<AssociationType>) l));
      if (!source2.Any<AssociationType>())
        return false;
      AssociationType associationType = source2.First<AssociationType>();
      EntityType entityType2 = associationType.Constraint.FromRole.GetEntityType();
      EntityType dependentEntityType = entityType == entityType2 ? associationType.Constraint.ToRole.GetEntityType() : entityType;
      MappingFragment mappingFragment = entityType == entityType2 ? mappingsUsingTable.Single<EntityTypeMapping>((Func<EntityTypeMapping, bool>) (etm => etm.EntityType == dependentEntityType)).Fragments.SingleOrDefault<MappingFragment>((Func<MappingFragment, bool>) (mf => mf.Table == toTable)) : fragment;
      if (mappingFragment != null)
      {
        List<EdmProperty> list1 = entityType2.KeyProperties().ToList<EdmProperty>();
        List<EdmProperty> list2 = dependentEntityType.KeyProperties().ToList<EdmProperty>();
        for (int index = 0; index < list1.Count; ++index)
        {
          EdmProperty dependentKey = list2[index];
          dependentKey.SetStoreGeneratedPattern(StoreGeneratedPattern.None);
          mappingFragment.ColumnMappings.Single<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => pm.PropertyPath.First<EdmProperty>() == dependentKey)).ColumnProperty.Name = list1[index].Name;
        }
      }
      return true;
    }
  }
}
