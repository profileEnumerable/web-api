// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Types.EntityTypeConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Mapping;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.ModelConfiguration.Edm.Services;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration.Types
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class EntityTypeConfiguration : StructuralTypeConfiguration
  {
    private readonly List<PropertyInfo> _keyProperties = new List<PropertyInfo>();
    private readonly Dictionary<PropertyInfo, NavigationPropertyConfiguration> _navigationPropertyConfigurations = new Dictionary<PropertyInfo, NavigationPropertyConfiguration>((IEqualityComparer<PropertyInfo>) new DynamicEqualityComparer<PropertyInfo>((Func<PropertyInfo, PropertyInfo, bool>) ((p1, p2) => p1.IsSameAs(p2))));
    private readonly List<EntityMappingConfiguration> _entityMappingConfigurations = new List<EntityMappingConfiguration>();
    private readonly Dictionary<Type, EntityMappingConfiguration> _entitySubTypesMappingConfigurations = new Dictionary<Type, EntityMappingConfiguration>();
    private readonly List<EntityMappingConfiguration> _nonCloneableMappings = new List<EntityMappingConfiguration>();
    private readonly IDictionary<string, object> _annotations = (IDictionary<string, object>) new Dictionary<string, object>();
    private bool _isKeyConfigured;
    private string _entitySetName;
    private ModificationStoredProceduresConfiguration _modificationStoredProceduresConfiguration;

    internal EntityTypeConfiguration(Type structuralType)
      : base(structuralType)
    {
      this.IsReplaceable = false;
    }

    private EntityTypeConfiguration(EntityTypeConfiguration source)
      : base((StructuralTypeConfiguration) source)
    {
      this._keyProperties.AddRange((IEnumerable<PropertyInfo>) source._keyProperties);
      source._navigationPropertyConfigurations.Each<KeyValuePair<PropertyInfo, NavigationPropertyConfiguration>>((Action<KeyValuePair<PropertyInfo, NavigationPropertyConfiguration>>) (c => this._navigationPropertyConfigurations.Add(c.Key, c.Value.Clone())));
      source._entitySubTypesMappingConfigurations.Each<KeyValuePair<Type, EntityMappingConfiguration>>((Action<KeyValuePair<Type, EntityMappingConfiguration>>) (c => this._entitySubTypesMappingConfigurations.Add(c.Key, c.Value.Clone())));
      this._entityMappingConfigurations.AddRange(source._entityMappingConfigurations.Except<EntityMappingConfiguration>((IEnumerable<EntityMappingConfiguration>) source._nonCloneableMappings).Select<EntityMappingConfiguration, EntityMappingConfiguration>((Func<EntityMappingConfiguration, EntityMappingConfiguration>) (e => e.Clone())));
      this._isKeyConfigured = source._isKeyConfigured;
      this._entitySetName = source._entitySetName;
      if (source._modificationStoredProceduresConfiguration != null)
        this._modificationStoredProceduresConfiguration = source._modificationStoredProceduresConfiguration.Clone();
      this.IsReplaceable = source.IsReplaceable;
      this.IsTableNameConfigured = source.IsTableNameConfigured;
      this.IsExplicitEntity = source.IsExplicitEntity;
      foreach (KeyValuePair<string, object> annotation in (IEnumerable<KeyValuePair<string, object>>) source._annotations)
        this._annotations.Add(annotation);
    }

    internal virtual EntityTypeConfiguration Clone()
    {
      return new EntityTypeConfiguration(this);
    }

    internal IEnumerable<Type> ConfiguredComplexTypes
    {
      get
      {
        return this.PrimitivePropertyConfigurations.Where<KeyValuePair<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>>((Func<KeyValuePair<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>, bool>) (c => c.Key.Count > 1)).Select<KeyValuePair<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>, IEnumerable<PropertyInfo>>((Func<KeyValuePair<PropertyPath, System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>, IEnumerable<PropertyInfo>>) (c => c.Key.Reverse<PropertyInfo>().Skip<PropertyInfo>(1))).SelectMany<IEnumerable<PropertyInfo>, PropertyInfo>((Func<IEnumerable<PropertyInfo>, IEnumerable<PropertyInfo>>) (p => p)).Select<PropertyInfo, Type>((Func<PropertyInfo, Type>) (pi => pi.PropertyType));
      }
    }

    internal bool IsStructuralConfigurationOnly
    {
      get
      {
        if (!this._keyProperties.Any<PropertyInfo>() && !this._navigationPropertyConfigurations.Any<KeyValuePair<PropertyInfo, NavigationPropertyConfiguration>>() && (!this._entityMappingConfigurations.Any<EntityMappingConfiguration>() && !this._entitySubTypesMappingConfigurations.Any<KeyValuePair<Type, EntityMappingConfiguration>>()))
          return this._entitySetName == null;
        return false;
      }
    }

    internal override void RemoveProperty(PropertyPath propertyPath)
    {
      base.RemoveProperty(propertyPath);
      this._navigationPropertyConfigurations.Remove(propertyPath.Single<PropertyInfo>());
    }

    internal bool IsKeyConfigured
    {
      get
      {
        return this._isKeyConfigured;
      }
    }

    internal IEnumerable<PropertyInfo> KeyProperties
    {
      get
      {
        return (IEnumerable<PropertyInfo>) this._keyProperties;
      }
    }

    internal virtual void Key(IEnumerable<PropertyInfo> keyProperties)
    {
      this.ClearKey();
      foreach (PropertyInfo keyProperty in keyProperties)
        this.Key(keyProperty, new OverridableConfigurationParts?(OverridableConfigurationParts.None));
      this._isKeyConfigured = true;
    }

    public void Key(PropertyInfo propertyInfo)
    {
      Check.NotNull<PropertyInfo>(propertyInfo, nameof (propertyInfo));
      this.Key(propertyInfo, new OverridableConfigurationParts?());
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    internal virtual void Key(
      PropertyInfo propertyInfo,
      OverridableConfigurationParts? overridableConfigurationParts)
    {
      if (!propertyInfo.IsValidEdmScalarProperty())
        throw Error.ModelBuilder_KeyPropertiesMustBePrimitive((object) propertyInfo.Name, (object) this.ClrType);
      if (this._isKeyConfigured || this._keyProperties.ContainsSame(propertyInfo))
        return;
      this._keyProperties.Add(propertyInfo);
      this.Property(new PropertyPath(propertyInfo), overridableConfigurationParts);
    }

    internal void ClearKey()
    {
      this._keyProperties.Clear();
      this._isKeyConfigured = false;
    }

    public bool IsTableNameConfigured { get; private set; }

    internal bool IsReplaceable { get; set; }

    internal bool IsExplicitEntity { get; set; }

    internal ModificationStoredProceduresConfiguration ModificationStoredProceduresConfiguration
    {
      get
      {
        return this._modificationStoredProceduresConfiguration;
      }
    }

    internal virtual void MapToStoredProcedures()
    {
      if (this._modificationStoredProceduresConfiguration != null)
        return;
      this._modificationStoredProceduresConfiguration = new ModificationStoredProceduresConfiguration();
    }

    internal virtual void MapToStoredProcedures(
      ModificationStoredProceduresConfiguration modificationStoredProceduresConfiguration,
      bool allowOverride)
    {
      if (this._modificationStoredProceduresConfiguration == null)
        this._modificationStoredProceduresConfiguration = modificationStoredProceduresConfiguration;
      else
        this._modificationStoredProceduresConfiguration.Merge(modificationStoredProceduresConfiguration, allowOverride);
    }

    internal void ReplaceFrom(EntityTypeConfiguration existing)
    {
      if (this.EntitySetName != null)
        return;
      this.EntitySetName = existing.EntitySetName;
    }

    public virtual string EntitySetName
    {
      get
      {
        return this._entitySetName;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this._entitySetName = value;
      }
    }

    internal override IEnumerable<PropertyInfo> ConfiguredProperties
    {
      get
      {
        return base.ConfiguredProperties.Union<PropertyInfo>((IEnumerable<PropertyInfo>) this._navigationPropertyConfigurations.Keys);
      }
    }

    public string TableName
    {
      get
      {
        if (!this.IsTableNameConfigured)
          return (string) null;
        return this.GetTableName().Name;
      }
    }

    public string SchemaName
    {
      get
      {
        if (!this.IsTableNameConfigured)
          return (string) null;
        return this.GetTableName().Schema;
      }
    }

    internal DatabaseName GetTableName()
    {
      if (!this.IsTableNameConfigured)
        return (DatabaseName) null;
      return this._entityMappingConfigurations.First<EntityMappingConfiguration>().TableName;
    }

    public void ToTable(string tableName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      this.ToTable(tableName, (string) null);
    }

    public void ToTable(string tableName, string schemaName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      this.IsTableNameConfigured = true;
      if (!this._entityMappingConfigurations.Any<EntityMappingConfiguration>())
        this._entityMappingConfigurations.Add(new EntityMappingConfiguration());
      this._entityMappingConfigurations.First<EntityMappingConfiguration>().TableName = string.IsNullOrWhiteSpace(schemaName) ? new DatabaseName(tableName) : new DatabaseName(tableName, schemaName);
      this.UpdateTableNameForSubTypes();
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

    private void UpdateTableNameForSubTypes()
    {
      this._entitySubTypesMappingConfigurations.Where<KeyValuePair<Type, EntityMappingConfiguration>>((Func<KeyValuePair<Type, EntityMappingConfiguration>, bool>) (stmc => stmc.Value.TableName == null)).Select<KeyValuePair<Type, EntityMappingConfiguration>, EntityMappingConfiguration>((Func<KeyValuePair<Type, EntityMappingConfiguration>, EntityMappingConfiguration>) (tphs => tphs.Value)).Each<EntityMappingConfiguration, DatabaseName>((Func<EntityMappingConfiguration, DatabaseName>) (tphmc => tphmc.TableName = this.GetTableName()));
    }

    internal void AddMappingConfiguration(
      EntityMappingConfiguration mappingConfiguration,
      bool cloneable = true)
    {
      if (this._entityMappingConfigurations.Contains(mappingConfiguration))
        return;
      DatabaseName tableName = mappingConfiguration.TableName;
      if (tableName != null && this._entityMappingConfigurations.SingleOrDefault<EntityMappingConfiguration>((Func<EntityMappingConfiguration, bool>) (mf => tableName.Equals(mf.TableName))) != null)
        throw Error.InvalidTableMapping((object) this.ClrType.Name, (object) tableName);
      this._entityMappingConfigurations.Add(mappingConfiguration);
      if (this._entityMappingConfigurations.Count > 1 && this._entityMappingConfigurations.Any<EntityMappingConfiguration>((Func<EntityMappingConfiguration, bool>) (mc => mc.TableName == null)))
        throw Error.InvalidTableMapping_NoTableName((object) this.ClrType.Name);
      this.IsTableNameConfigured |= tableName != null;
      if (cloneable)
        return;
      this._nonCloneableMappings.Add(mappingConfiguration);
    }

    internal void AddSubTypeMappingConfiguration(
      Type subType,
      EntityMappingConfiguration mappingConfiguration)
    {
      EntityMappingConfiguration mappingConfiguration1;
      if (this._entitySubTypesMappingConfigurations.TryGetValue(subType, out mappingConfiguration1))
        throw Error.InvalidChainedMappingSyntax((object) subType.Name);
      this._entitySubTypesMappingConfigurations.Add(subType, mappingConfiguration);
    }

    internal Dictionary<Type, EntityMappingConfiguration> SubTypeMappingConfigurations
    {
      get
      {
        return this._entitySubTypesMappingConfigurations;
      }
    }

    internal NavigationPropertyConfiguration Navigation(
      PropertyInfo propertyInfo)
    {
      NavigationPropertyConfiguration propertyConfiguration;
      if (!this._navigationPropertyConfigurations.TryGetValue(propertyInfo, out propertyConfiguration))
        this._navigationPropertyConfigurations.Add(propertyInfo, propertyConfiguration = new NavigationPropertyConfiguration(propertyInfo));
      return propertyConfiguration;
    }

    internal virtual void Configure(EntityType entityType, EdmModel model)
    {
      this.ConfigureKey(entityType);
      this.Configure(entityType.Name, (IEnumerable<EdmProperty>) entityType.Properties, (ICollection<MetadataProperty>) entityType.GetMetadataProperties());
      this.ConfigureAssociations(entityType, model);
      this.ConfigureEntitySetName(entityType, model);
    }

    private void ConfigureEntitySetName(EntityType entityType, EdmModel model)
    {
      if (this.EntitySetName == null || entityType.BaseType != null)
        return;
      EntitySet entitySet = model.GetEntitySet(entityType);
      entitySet.Name = ((IEnumerable<INamedDataModelItem>) model.GetEntitySets().Except<EntitySet>((IEnumerable<EntitySet>) new EntitySet[1]
      {
        entitySet
      })).UniquifyName(this.EntitySetName);
      entitySet.SetConfiguration((object) this);
    }

    private void ConfigureKey(EntityType entityType)
    {
      if (!this._keyProperties.Any<PropertyInfo>())
        return;
      if (entityType.BaseType != null)
        throw Error.KeyRegisteredOnDerivedType((object) this.ClrType, (object) EntityTypeExtensions.GetClrType(entityType.GetRootType()));
      IEnumerable<PropertyInfo> propertyInfos = this._keyProperties.AsEnumerable<PropertyInfo>();
      if (!this._isKeyConfigured)
      {
        IEnumerable<\u003C\u003Ef__AnonymousType41<PropertyInfo, int?>> source = this._keyProperties.Select(p => new
        {
          PropertyInfo = p,
          ColumnOrder = this.Property(new PropertyPath(p), new OverridableConfigurationParts?()).ColumnOrder
        });
        if (this._keyProperties.Count > 1 && source.Any(p => !p.ColumnOrder.HasValue))
          throw Error.ModelGeneration_UnableToDetermineKeyOrder((object) this.ClrType);
        propertyInfos = source.OrderBy(p => p.ColumnOrder).Select(p => p.PropertyInfo);
      }
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        EdmProperty primitiveProperty = entityType.GetDeclaredPrimitiveProperty(propertyInfo);
        if (primitiveProperty == null)
          throw Error.KeyPropertyNotFound((object) propertyInfo.Name, (object) entityType.Name);
        primitiveProperty.Nullable = false;
        entityType.AddKeyMember((EdmMember) primitiveProperty);
      }
    }

    private void ConfigureAssociations(EntityType entityType, EdmModel model)
    {
      foreach (KeyValuePair<PropertyInfo, NavigationPropertyConfiguration> propertyConfiguration1 in this._navigationPropertyConfigurations)
      {
        PropertyInfo propertyInfo = propertyConfiguration1.Key;
        NavigationPropertyConfiguration propertyConfiguration2 = propertyConfiguration1.Value;
        NavigationProperty navigationProperty = entityType.GetNavigationProperty(propertyInfo);
        if (navigationProperty == null)
        {
          EdmProperty edmProperty = entityType.Properties.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (p => p.GetClrPropertyInfo() == propertyInfo));
          if (edmProperty != null && edmProperty.ComplexType != null)
            throw new InvalidOperationException(Strings.InvalidNavigationPropertyComplexType((object) propertyInfo.Name, (object) entityType.Name, (object) edmProperty.ComplexType.Name));
          throw Error.NavigationPropertyNotFound((object) propertyInfo.Name, (object) entityType.Name);
        }
        if (entityType.DeclaredNavigationProperties.Any<NavigationProperty>((Func<NavigationProperty, bool>) (np => np.GetClrPropertyInfo().IsSameAs(propertyInfo))))
          propertyConfiguration2.Configure(navigationProperty, model, this);
      }
    }

    internal void ConfigureTablesAndConditions(
      EntityTypeMapping entityTypeMapping,
      DbDatabaseMapping databaseMapping,
      ICollection<EntitySet> entitySets,
      DbProviderManifest providerManifest)
    {
      EntityType entityType = entityTypeMapping != null ? entityTypeMapping.EntityType : databaseMapping.Model.GetEntityType(this.ClrType);
      if (this._entityMappingConfigurations.Any<EntityMappingConfiguration>())
      {
        for (int configurationIndex = 0; configurationIndex < this._entityMappingConfigurations.Count; ++configurationIndex)
          this._entityMappingConfigurations[configurationIndex].Configure(databaseMapping, entitySets, providerManifest, entityType, ref entityTypeMapping, this.IsMappingAnyInheritedProperty(entityType), configurationIndex, this._entityMappingConfigurations.Count, this._annotations);
      }
      else
        EntityTypeConfiguration.ConfigureUnconfiguredType(databaseMapping, entitySets, providerManifest, entityType, this._annotations);
    }

    internal bool IsMappingAnyInheritedProperty(EntityType entityType)
    {
      return this._entityMappingConfigurations.Any<EntityMappingConfiguration>((Func<EntityMappingConfiguration, bool>) (emc => emc.MapsAnyInheritedProperties(entityType)));
    }

    internal bool IsNavigationPropertyConfigured(PropertyInfo propertyInfo)
    {
      return this._navigationPropertyConfigurations.ContainsKey(propertyInfo);
    }

    internal static void ConfigureUnconfiguredType(
      DbDatabaseMapping databaseMapping,
      ICollection<EntitySet> entitySets,
      DbProviderManifest providerManifest,
      EntityType entityType,
      IDictionary<string, object> commonAnnotations)
    {
      EntityMappingConfiguration mappingConfiguration = new EntityMappingConfiguration();
      EntityTypeMapping entityTypeMapping = databaseMapping.GetEntityTypeMapping(EntityTypeExtensions.GetClrType(entityType));
      mappingConfiguration.Configure(databaseMapping, entitySets, providerManifest, entityType, ref entityTypeMapping, false, 0, 1, commonAnnotations);
    }

    internal void Configure(
      EntityType entityType,
      DbDatabaseMapping databaseMapping,
      DbProviderManifest providerManifest)
    {
      EntityTypeMapping entityTypeMapping = databaseMapping.GetEntityTypeMapping(EntityTypeExtensions.GetClrType(entityType));
      if (entityTypeMapping != null)
        EntityTypeConfiguration.VerifyAllCSpacePropertiesAreMapped((ICollection<EntityTypeMapping>) databaseMapping.GetEntityTypeMappings(entityType).ToList<EntityTypeMapping>(), (IEnumerable<EdmProperty>) entityTypeMapping.EntityType.DeclaredProperties, (IList<EdmProperty>) new List<EdmProperty>());
      this.ConfigurePropertyMappings(databaseMapping, entityType, providerManifest, false);
      this.ConfigureAssociationMappings(databaseMapping, entityType, providerManifest);
      EntityTypeConfiguration.ConfigureDependentKeys(databaseMapping, providerManifest);
      this.ConfigureModificationStoredProcedures(databaseMapping, entityType, providerManifest);
    }

    internal void ConfigureFunctionParameters(
      DbDatabaseMapping databaseMapping,
      EntityType entityType)
    {
      this.ConfigureFunctionParameters((IList<ModificationFunctionParameterBinding>) databaseMapping.GetEntitySetMappings().SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm => (IEnumerable<EntityTypeModificationFunctionMapping>) esm.ModificationFunctionMappings), (esm, mfm) => new
      {
        esm = esm,
        mfm = mfm
      }).Where(_param1 => _param1.mfm.EntityType == entityType).SelectMany(_param0 => _param0.mfm.PrimaryParameterBindings, (_param0, pb) => pb).ToList<ModificationFunctionParameterBinding>());
      foreach (EntityType entityType1 in databaseMapping.Model.EntityTypes.Where<EntityType>((Func<EntityType, bool>) (et => et.BaseType == entityType)))
        this.ConfigureFunctionParameters(databaseMapping, entityType1);
    }

    private void ConfigureModificationStoredProcedures(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      DbProviderManifest providerManifest)
    {
      if (this._modificationStoredProceduresConfiguration == null)
        return;
      new ModificationFunctionMappingGenerator(providerManifest).Generate(entityType, databaseMapping);
      EntityTypeModificationFunctionMapping modificationStoredProcedureMapping = databaseMapping.GetEntitySetMappings().SelectMany<EntitySetMapping, EntityTypeModificationFunctionMapping>((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm => (IEnumerable<EntityTypeModificationFunctionMapping>) esm.ModificationFunctionMappings)).SingleOrDefault<EntityTypeModificationFunctionMapping>((Func<EntityTypeModificationFunctionMapping, bool>) (mfm => mfm.EntityType == entityType));
      if (modificationStoredProcedureMapping == null)
        return;
      this._modificationStoredProceduresConfiguration.Configure(modificationStoredProcedureMapping, providerManifest);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void ConfigurePropertyMappings(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      DbProviderManifest providerManifest,
      bool allowOverride = false)
    {
      List<Tuple<ColumnMappingBuilder, EntityType>> propertyMappings = databaseMapping.GetEntityTypeMappings(entityType).SelectMany((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments), (etm, etmf) => new
      {
        etm = etm,
        etmf = etmf
      }).SelectMany(_param0 => _param0.etmf.ColumnMappings, (_param0, pm) => Tuple.Create<ColumnMappingBuilder, EntityType>(pm, _param0.etmf.Table)).ToList<Tuple<ColumnMappingBuilder, EntityType>>();
      this.ConfigurePropertyMappings((IList<Tuple<ColumnMappingBuilder, EntityType>>) propertyMappings, providerManifest, allowOverride);
      this._entityMappingConfigurations.Each<EntityMappingConfiguration>((Action<EntityMappingConfiguration>) (c => c.ConfigurePropertyMappings((IList<Tuple<ColumnMappingBuilder, EntityType>>) propertyMappings, providerManifest, allowOverride)));
      List<Tuple<ColumnMappingBuilder, EntityType>> inheritedPropertyMappings = databaseMapping.GetEntitySetMappings().SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (esm => (IEnumerable<EntityTypeMapping>) esm.EntityTypeMappings), (esm, etm) => new
      {
        esm = esm,
        etm = etm
      }).Where(_param1 =>
      {
        if (_param1.etm.IsHierarchyMapping)
          return _param1.etm.EntityType.IsAncestorOf(entityType);
        return false;
      }).SelectMany(_param0 => (IEnumerable<MappingFragment>) _param0.etm.MappingFragments, (_param0, etmf) => new
      {
        \u003C\u003Eh__TransparentIdentifier43 = _param0,
        etmf = etmf
      }).SelectMany(_param0 => _param0.etmf.ColumnMappings, (_param0, pm1) => new
      {
        \u003C\u003Eh__TransparentIdentifier44 = _param0,
        pm1 = pm1
      }).Where(_param1 => !propertyMappings.Any<Tuple<ColumnMappingBuilder, EntityType>>((Func<Tuple<ColumnMappingBuilder, EntityType>, bool>) (pm2 => pm2.Item1.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) _param1.pm1.PropertyPath)))).Select(_param0 => Tuple.Create<ColumnMappingBuilder, EntityType>(_param0.pm1, _param0.\u003C\u003Eh__TransparentIdentifier44.etmf.Table)).ToList<Tuple<ColumnMappingBuilder, EntityType>>();
      this.ConfigurePropertyMappings((IList<Tuple<ColumnMappingBuilder, EntityType>>) inheritedPropertyMappings, providerManifest, false);
      this._entityMappingConfigurations.Each<EntityMappingConfiguration>((Action<EntityMappingConfiguration>) (c => c.ConfigurePropertyMappings((IList<Tuple<ColumnMappingBuilder, EntityType>>) inheritedPropertyMappings, providerManifest, false)));
      foreach (EntityType entityType1 in databaseMapping.Model.EntityTypes.Where<EntityType>((Func<EntityType, bool>) (et => et.BaseType == entityType)))
        this.ConfigurePropertyMappings(databaseMapping, entityType1, providerManifest, true);
    }

    private void ConfigureAssociationMappings(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      DbProviderManifest providerManifest)
    {
      foreach (KeyValuePair<PropertyInfo, NavigationPropertyConfiguration> propertyConfiguration1 in this._navigationPropertyConfigurations)
      {
        PropertyInfo key = propertyConfiguration1.Key;
        NavigationPropertyConfiguration propertyConfiguration2 = propertyConfiguration1.Value;
        NavigationProperty navigationProperty = entityType.GetNavigationProperty(key);
        if (navigationProperty == null)
          throw Error.NavigationPropertyNotFound((object) key.Name, (object) entityType.Name);
        AssociationSetMapping associationSetMapping = databaseMapping.GetAssociationSetMappings().SingleOrDefault<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm => asm.AssociationSet.ElementType == navigationProperty.Association));
        if (associationSetMapping != null)
          propertyConfiguration2.Configure(associationSetMapping, databaseMapping, providerManifest);
      }
    }

    private static void ConfigureDependentKeys(
      DbDatabaseMapping databaseMapping,
      DbProviderManifest providerManifest)
    {
      IList<EntityType> entityTypeList = databaseMapping.Database.EntityTypes as IList<EntityType> ?? (IList<EntityType>) databaseMapping.Database.EntityTypes.ToList<EntityType>();
      for (int index1 = 0; index1 < entityTypeList.Count; ++index1)
      {
        EntityType entityType = entityTypeList[index1];
        IList<ForeignKeyBuilder> foreignKeyBuilderList = entityType.ForeignKeyBuilders as IList<ForeignKeyBuilder> ?? (IList<ForeignKeyBuilder>) entityType.ForeignKeyBuilders.ToList<ForeignKeyBuilder>();
        for (int index2 = 0; index2 < foreignKeyBuilderList.Count; ++index2)
        {
          ForeignKeyBuilder foreignKeyBuilder = foreignKeyBuilderList[index2];
          IEnumerable<EdmProperty> dependentColumns = foreignKeyBuilder.DependentColumns;
          IList<EdmProperty> edmPropertyList = dependentColumns as IList<EdmProperty> ?? (IList<EdmProperty>) dependentColumns.ToList<EdmProperty>();
          for (int index3 = 0; index3 < edmPropertyList.Count; ++index3)
          {
            EdmProperty edmProperty = edmPropertyList[index3];
            System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration configuration = edmProperty.GetConfiguration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration;
            if (configuration == null || configuration.ColumnType == null)
            {
              EdmProperty other = foreignKeyBuilder.PrincipalTable.KeyProperties.ElementAt<EdmProperty>(index3);
              edmProperty.PrimitiveType = providerManifest.GetStoreTypeFromName(other.TypeName);
              edmProperty.CopyFrom(other);
            }
          }
        }
      }
    }

    private static void VerifyAllCSpacePropertiesAreMapped(
      ICollection<EntityTypeMapping> entityTypeMappings,
      IEnumerable<EdmProperty> properties,
      IList<EdmProperty> propertyPath)
    {
      EntityType entityType = entityTypeMappings.First<EntityTypeMapping>().EntityType;
      foreach (EdmProperty property in properties)
      {
        propertyPath.Add(property);
        if (property.IsComplexType)
          EntityTypeConfiguration.VerifyAllCSpacePropertiesAreMapped(entityTypeMappings, (IEnumerable<EdmProperty>) property.ComplexType.Properties, propertyPath);
        else if (!entityTypeMappings.SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments)).SelectMany<MappingFragment, ColumnMappingBuilder>((Func<MappingFragment, IEnumerable<ColumnMappingBuilder>>) (mf => mf.ColumnMappings)).Any<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => pm.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) propertyPath))) && !entityType.Abstract)
          throw Error.InvalidEntitySplittingProperties((object) entityType.Name);
        propertyPath.Remove(property);
      }
    }
  }
}
