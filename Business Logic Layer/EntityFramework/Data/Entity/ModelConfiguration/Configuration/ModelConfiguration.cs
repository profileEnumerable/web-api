// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Mapping;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
  internal class ModelConfiguration : ConfigurationBase
  {
    private readonly Dictionary<Type, EntityTypeConfiguration> _entityConfigurations = new Dictionary<Type, EntityTypeConfiguration>();
    private readonly Dictionary<Type, ComplexTypeConfiguration> _complexTypeConfigurations = new Dictionary<Type, ComplexTypeConfiguration>();
    private readonly HashSet<Type> _ignoredTypes = new HashSet<Type>();

    internal ModelConfiguration()
    {
    }

    private ModelConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration source)
    {
      source._entityConfigurations.Each<KeyValuePair<Type, EntityTypeConfiguration>>((Action<KeyValuePair<Type, EntityTypeConfiguration>>) (c => this._entityConfigurations.Add(c.Key, c.Value.Clone())));
      source._complexTypeConfigurations.Each<KeyValuePair<Type, ComplexTypeConfiguration>>((Action<KeyValuePair<Type, ComplexTypeConfiguration>>) (c => this._complexTypeConfigurations.Add(c.Key, c.Value.Clone())));
      this._ignoredTypes.AddRange<Type>((IEnumerable<Type>) source._ignoredTypes);
      this.DefaultSchema = source.DefaultSchema;
      this.ModelNamespace = source.ModelNamespace;
    }

    internal virtual System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration Clone()
    {
      return new System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration(this);
    }

    public virtual IEnumerable<Type> ConfiguredTypes
    {
      get
      {
        return this._entityConfigurations.Keys.Union<Type>((IEnumerable<Type>) this._complexTypeConfigurations.Keys).Union<Type>((IEnumerable<Type>) this._ignoredTypes);
      }
    }

    internal virtual IEnumerable<Type> Entities
    {
      get
      {
        return (IEnumerable<Type>) this._entityConfigurations.Keys.Except<Type>((IEnumerable<Type>) this._ignoredTypes).ToList<Type>();
      }
    }

    internal virtual IEnumerable<Type> ComplexTypes
    {
      get
      {
        return (IEnumerable<Type>) this._complexTypeConfigurations.Keys.Except<Type>((IEnumerable<Type>) this._ignoredTypes).ToList<Type>();
      }
    }

    internal virtual IEnumerable<Type> StructuralTypes
    {
      get
      {
        return (IEnumerable<Type>) this._entityConfigurations.Keys.Union<Type>((IEnumerable<Type>) this._complexTypeConfigurations.Keys).Except<Type>((IEnumerable<Type>) this._ignoredTypes).ToList<Type>();
      }
    }

    public string DefaultSchema { get; set; }

    public string ModelNamespace { get; set; }

    internal virtual void Add(EntityTypeConfiguration entityTypeConfiguration)
    {
      EntityTypeConfiguration existing;
      if (this._entityConfigurations.TryGetValue(entityTypeConfiguration.ClrType, out existing) && !existing.IsReplaceable || this._complexTypeConfigurations.ContainsKey(entityTypeConfiguration.ClrType))
        throw Error.DuplicateStructuralTypeConfiguration((object) entityTypeConfiguration.ClrType);
      if (existing != null && existing.IsReplaceable)
      {
        this._entityConfigurations.Remove(existing.ClrType);
        entityTypeConfiguration.ReplaceFrom(existing);
      }
      else
        entityTypeConfiguration.IsReplaceable = false;
      this._entityConfigurations.Add(entityTypeConfiguration.ClrType, entityTypeConfiguration);
    }

    internal virtual void Add(ComplexTypeConfiguration complexTypeConfiguration)
    {
      if (this._entityConfigurations.ContainsKey(complexTypeConfiguration.ClrType) || this._complexTypeConfigurations.ContainsKey(complexTypeConfiguration.ClrType))
        throw Error.DuplicateStructuralTypeConfiguration((object) complexTypeConfiguration.ClrType);
      this._complexTypeConfigurations.Add(complexTypeConfiguration.ClrType, complexTypeConfiguration);
    }

    public virtual EntityTypeConfiguration Entity(Type entityType)
    {
      Check.NotNull<Type>(entityType, nameof (entityType));
      return this.Entity(entityType, false);
    }

    internal virtual EntityTypeConfiguration Entity(
      Type entityType,
      bool explicitEntity)
    {
      if (this._complexTypeConfigurations.ContainsKey(entityType))
        throw Error.EntityTypeConfigurationMismatch((object) entityType.Name);
      EntityTypeConfiguration typeConfiguration;
      if (!this._entityConfigurations.TryGetValue(entityType, out typeConfiguration))
        this._entityConfigurations.Add(entityType, typeConfiguration = new EntityTypeConfiguration(entityType)
        {
          IsExplicitEntity = explicitEntity
        });
      return typeConfiguration;
    }

    [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#")]
    public virtual ComplexTypeConfiguration ComplexType(Type complexType)
    {
      Check.NotNull<Type>(complexType, nameof (complexType));
      if (this._entityConfigurations.ContainsKey(complexType))
        throw Error.ComplexTypeConfigurationMismatch((object) complexType.Name);
      ComplexTypeConfiguration typeConfiguration;
      if (!this._complexTypeConfigurations.TryGetValue(complexType, out typeConfiguration))
        this._complexTypeConfigurations.Add(complexType, typeConfiguration = new ComplexTypeConfiguration(complexType));
      return typeConfiguration;
    }

    public virtual void Ignore(Type type)
    {
      Check.NotNull<Type>(type, nameof (type));
      this._ignoredTypes.Add(type);
    }

    internal virtual StructuralTypeConfiguration GetStructuralTypeConfiguration(
      Type type)
    {
      EntityTypeConfiguration typeConfiguration1;
      if (this._entityConfigurations.TryGetValue(type, out typeConfiguration1))
        return (StructuralTypeConfiguration) typeConfiguration1;
      ComplexTypeConfiguration typeConfiguration2;
      if (this._complexTypeConfigurations.TryGetValue(type, out typeConfiguration2))
        return (StructuralTypeConfiguration) typeConfiguration2;
      return (StructuralTypeConfiguration) null;
    }

    public virtual bool IsComplexType(Type type)
    {
      Check.NotNull<Type>(type, nameof (type));
      return this._complexTypeConfigurations.ContainsKey(type);
    }

    public virtual bool IsIgnoredType(Type type)
    {
      Check.NotNull<Type>(type, nameof (type));
      return this._ignoredTypes.Contains(type);
    }

    public virtual IEnumerable<PropertyInfo> GetConfiguredProperties(
      Type type)
    {
      Check.NotNull<Type>(type, nameof (type));
      StructuralTypeConfiguration typeConfiguration = this.GetStructuralTypeConfiguration(type);
      if (typeConfiguration == null)
        return Enumerable.Empty<PropertyInfo>();
      return typeConfiguration.ConfiguredProperties;
    }

    public virtual bool IsIgnoredProperty(Type type, PropertyInfo propertyInfo)
    {
      Check.NotNull<Type>(type, nameof (type));
      Check.NotNull<PropertyInfo>(propertyInfo, nameof (propertyInfo));
      for (; type != (Type) null; type = type.BaseType)
      {
        StructuralTypeConfiguration typeConfiguration = this.GetStructuralTypeConfiguration(type);
        if (typeConfiguration != null && typeConfiguration.IgnoredProperties.Any<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.IsSameAs(propertyInfo))))
          return true;
        if (propertyInfo.DeclaringType == type)
          break;
      }
      return false;
    }

    internal void Configure(EdmModel model)
    {
      this.ConfigureEntities(model);
      this.ConfigureComplexTypes(model);
    }

    private void ConfigureEntities(EdmModel model)
    {
      foreach (EntityTypeConfiguration entityConfiguration in this.ActiveEntityConfigurations)
        this.ConfigureFunctionMappings(model, entityConfiguration, model.GetEntityType(entityConfiguration.ClrType));
      foreach (EntityTypeConfiguration entityConfiguration in this.ActiveEntityConfigurations)
        entityConfiguration.Configure(model.GetEntityType(entityConfiguration.ClrType), model);
    }

    private void ConfigureFunctionMappings(
      EdmModel model,
      EntityTypeConfiguration entityTypeConfiguration,
      EntityType entityType)
    {
      if (entityTypeConfiguration.ModificationStoredProceduresConfiguration == null)
        return;
      for (; entityType.BaseType != null; entityType = (EntityType) entityType.BaseType)
      {
        Type clrType = EntityTypeExtensions.GetClrType((EntityType) entityType.BaseType);
        EntityTypeConfiguration typeConfiguration;
        if (!entityType.BaseType.Abstract && (!this._entityConfigurations.TryGetValue(clrType, out typeConfiguration) || typeConfiguration.ModificationStoredProceduresConfiguration == null))
          throw Error.BaseTypeNotMappedToFunctions((object) clrType.Name, (object) entityTypeConfiguration.ClrType.Name);
      }
      model.GetSelfAndAllDerivedTypes(entityType).Each<EntityType>((Action<EntityType>) (e =>
      {
        EntityTypeConfiguration typeConfiguration = this.Entity(EntityTypeExtensions.GetClrType(e));
        if (typeConfiguration.ModificationStoredProceduresConfiguration != null)
          return;
        typeConfiguration.MapToStoredProcedures();
      }));
    }

    private void ConfigureComplexTypes(EdmModel model)
    {
      foreach (ComplexTypeConfiguration typeConfiguration in this.ActiveComplexTypeConfigurations)
      {
        ComplexType complexType = model.GetComplexType(typeConfiguration.ClrType);
        typeConfiguration.Configure(complexType);
      }
    }

    internal void Configure(DbDatabaseMapping databaseMapping, DbProviderManifest providerManifest)
    {
      foreach (StructuralTypeConfiguration typeConfiguration in databaseMapping.Model.ComplexTypes.Select<ComplexType, object>((Func<ComplexType, object>) (ct => ct.GetConfiguration())).Cast<StructuralTypeConfiguration>().Where<StructuralTypeConfiguration>((Func<StructuralTypeConfiguration, bool>) (c => c != null)))
        typeConfiguration.ConfigurePropertyMappings((IList<Tuple<ColumnMappingBuilder, EntityType>>) databaseMapping.GetComplexPropertyMappings(typeConfiguration.ClrType).ToList<Tuple<ColumnMappingBuilder, EntityType>>(), providerManifest, false);
      this.ConfigureEntityTypes(databaseMapping, (ICollection<EntitySet>) databaseMapping.Model.Container.EntitySets, providerManifest);
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.RemoveRedundantColumnConditions(databaseMapping);
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.RemoveRedundantTables(databaseMapping);
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.ConfigureTables(databaseMapping.Database);
      this.ConfigureDefaultSchema(databaseMapping);
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.UniquifyFunctionNames(databaseMapping);
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.ConfigureFunctionParameters(databaseMapping);
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.RemoveDuplicateTphColumns(databaseMapping);
    }

    private static void ConfigureFunctionParameters(DbDatabaseMapping databaseMapping)
    {
      foreach (StructuralTypeConfiguration typeConfiguration in databaseMapping.Model.ComplexTypes.Select<ComplexType, object>((Func<ComplexType, object>) (ct => ct.GetConfiguration())).Cast<StructuralTypeConfiguration>().Where<StructuralTypeConfiguration>((Func<StructuralTypeConfiguration, bool>) (c => c != null)))
        typeConfiguration.ConfigureFunctionParameters((IList<ModificationFunctionParameterBinding>) databaseMapping.GetComplexParameterBindings(typeConfiguration.ClrType).ToList<ModificationFunctionParameterBinding>());
      foreach (EntityType entityType in databaseMapping.Model.EntityTypes.Where<EntityType>((Func<EntityType, bool>) (e => e.GetConfiguration() != null)))
        ((EntityTypeConfiguration) entityType.GetConfiguration()).ConfigureFunctionParameters(databaseMapping, entityType);
    }

    private static void UniquifyFunctionNames(DbDatabaseMapping databaseMapping)
    {
      foreach (EntityTypeModificationFunctionMapping modificationFunctionMapping in databaseMapping.GetEntitySetMappings().SelectMany<EntitySetMapping, EntityTypeModificationFunctionMapping>((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm => (IEnumerable<EntityTypeModificationFunctionMapping>) esm.ModificationFunctionMappings)))
      {
        EntityTypeConfiguration configuration = (EntityTypeConfiguration) modificationFunctionMapping.EntityType.GetConfiguration();
        if (configuration.ModificationStoredProceduresConfiguration != null)
        {
          ModificationStoredProceduresConfiguration proceduresConfiguration = configuration.ModificationStoredProceduresConfiguration;
          System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.UniquifyFunctionName(databaseMapping, proceduresConfiguration.InsertModificationStoredProcedureConfiguration, modificationFunctionMapping.InsertFunctionMapping);
          System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.UniquifyFunctionName(databaseMapping, proceduresConfiguration.UpdateModificationStoredProcedureConfiguration, modificationFunctionMapping.UpdateFunctionMapping);
          System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.UniquifyFunctionName(databaseMapping, proceduresConfiguration.DeleteModificationStoredProcedureConfiguration, modificationFunctionMapping.DeleteFunctionMapping);
        }
      }
      foreach (AssociationSetModificationFunctionMapping modificationFunctionMapping in databaseMapping.GetAssociationSetMappings().Select<AssociationSetMapping, AssociationSetModificationFunctionMapping>((Func<AssociationSetMapping, AssociationSetModificationFunctionMapping>) (asm => asm.ModificationFunctionMapping)).Where<AssociationSetModificationFunctionMapping>((Func<AssociationSetModificationFunctionMapping, bool>) (asm => asm != null)))
      {
        NavigationPropertyConfiguration configuration = (NavigationPropertyConfiguration) modificationFunctionMapping.AssociationSet.ElementType.GetConfiguration();
        if (configuration.ModificationStoredProceduresConfiguration != null)
        {
          System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.UniquifyFunctionName(databaseMapping, configuration.ModificationStoredProceduresConfiguration.InsertModificationStoredProcedureConfiguration, modificationFunctionMapping.InsertFunctionMapping);
          System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.UniquifyFunctionName(databaseMapping, configuration.ModificationStoredProceduresConfiguration.DeleteModificationStoredProcedureConfiguration, modificationFunctionMapping.DeleteFunctionMapping);
        }
      }
    }

    private static void UniquifyFunctionName(
      DbDatabaseMapping databaseMapping,
      ModificationStoredProcedureConfiguration modificationStoredProcedureConfiguration,
      ModificationFunctionMapping functionMapping)
    {
      if (modificationStoredProcedureConfiguration != null && !string.IsNullOrWhiteSpace(modificationStoredProcedureConfiguration.Name))
        return;
      functionMapping.Function.StoreFunctionNameAttribute = databaseMapping.Database.Functions.Except<EdmFunction>((IEnumerable<EdmFunction>) new EdmFunction[1]
      {
        functionMapping.Function
      }).Select<EdmFunction, string>((Func<EdmFunction, string>) (f => f.FunctionName)).Uniquify(functionMapping.Function.FunctionName);
    }

    private void ConfigureDefaultSchema(DbDatabaseMapping databaseMapping)
    {
      databaseMapping.Database.GetEntitySets().Where<EntitySet>((Func<EntitySet, bool>) (es => string.IsNullOrWhiteSpace(es.Schema))).Each<EntitySet, string>((Func<EntitySet, string>) (es => es.Schema = this.DefaultSchema ?? "dbo"));
      databaseMapping.Database.Functions.Where<EdmFunction>((Func<EdmFunction, bool>) (f => string.IsNullOrWhiteSpace(f.Schema))).Each<EdmFunction, string>((Func<EdmFunction, string>) (f => f.Schema = this.DefaultSchema ?? "dbo"));
    }

    private void ConfigureEntityTypes(
      DbDatabaseMapping databaseMapping,
      ICollection<EntitySet> entitySets,
      DbProviderManifest providerManifest)
    {
      IList<EntityTypeConfiguration> sortedEntityConfigurations = this.SortEntityConfigurationsByInheritance(databaseMapping);
      foreach (EntityTypeConfiguration typeConfiguration in (IEnumerable<EntityTypeConfiguration>) sortedEntityConfigurations)
      {
        EntityTypeMapping entityTypeMapping = databaseMapping.GetEntityTypeMapping(typeConfiguration.ClrType);
        typeConfiguration.ConfigureTablesAndConditions(entityTypeMapping, databaseMapping, entitySets, providerManifest);
        System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.ConfigureUnconfiguredDerivedTypes(databaseMapping, entitySets, providerManifest, databaseMapping.Model.GetEntityType(typeConfiguration.ClrType), sortedEntityConfigurations);
      }
      new EntityMappingService(databaseMapping).Configure();
      foreach (EntityType entityType in databaseMapping.Model.EntityTypes.Where<EntityType>((Func<EntityType, bool>) (e => e.GetConfiguration() != null)))
        ((EntityTypeConfiguration) entityType.GetConfiguration()).Configure(entityType, databaseMapping, providerManifest);
    }

    private static void ConfigureUnconfiguredDerivedTypes(
      DbDatabaseMapping databaseMapping,
      ICollection<EntitySet> entitySets,
      DbProviderManifest providerManifest,
      EntityType entityType,
      IList<EntityTypeConfiguration> sortedEntityConfigurations)
    {
      List<EntityType> list = databaseMapping.Model.GetDerivedTypes(entityType).ToList<EntityType>();
      while (list.Count > 0)
      {
        EntityType currentType = list[0];
        list.RemoveAt(0);
        if (!currentType.Abstract && sortedEntityConfigurations.All<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (etc => etc.ClrType != EntityTypeExtensions.GetClrType(currentType))))
        {
          EntityTypeConfiguration.ConfigureUnconfiguredType(databaseMapping, entitySets, providerManifest, currentType, (IDictionary<string, object>) new Dictionary<string, object>());
          list.AddRange(databaseMapping.Model.GetDerivedTypes(currentType));
        }
      }
    }

    private static void ConfigureTables(EdmModel database)
    {
      foreach (EntityType table in database.EntityTypes.ToList<EntityType>())
        System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration.ConfigureTable(database, table);
    }

    private static void ConfigureTable(EdmModel database, EntityType table)
    {
      DatabaseName tableName = table.GetTableName();
      if (tableName == null)
        return;
      EntitySet entitySet = database.GetEntitySet(table);
      if (!string.IsNullOrWhiteSpace(tableName.Schema))
        entitySet.Schema = tableName.Schema;
      entitySet.Table = tableName.Name;
    }

    private IList<EntityTypeConfiguration> SortEntityConfigurationsByInheritance(
      DbDatabaseMapping databaseMapping)
    {
      List<EntityTypeConfiguration> typeConfigurationList = new List<EntityTypeConfiguration>();
      foreach (EntityTypeConfiguration entityConfiguration in this.ActiveEntityConfigurations)
      {
        EntityType entityType = databaseMapping.Model.GetEntityType(entityConfiguration.ClrType);
        if (entityType != null)
        {
          if (entityType.BaseType == null)
          {
            if (!typeConfigurationList.Contains(entityConfiguration))
              typeConfigurationList.Add(entityConfiguration);
          }
          else
          {
            Stack<EntityType> entityTypeStack = new Stack<EntityType>();
            for (; entityType != null; entityType = (EntityType) entityType.BaseType)
              entityTypeStack.Push(entityType);
            while (entityTypeStack.Count > 0)
            {
              entityType = entityTypeStack.Pop();
              EntityTypeConfiguration typeConfiguration = this.ActiveEntityConfigurations.SingleOrDefault<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (ec => ec.ClrType == EntityTypeExtensions.GetClrType(entityType)));
              if (typeConfiguration != null && !typeConfigurationList.Contains(typeConfiguration))
                typeConfigurationList.Add(typeConfiguration);
            }
          }
        }
      }
      return (IList<EntityTypeConfiguration>) typeConfigurationList;
    }

    internal void NormalizeConfigurations()
    {
      this.DiscoverIndirectlyConfiguredComplexTypes();
      this.ReassignSubtypeMappings();
    }

    private void DiscoverIndirectlyConfiguredComplexTypes()
    {
      this.ActiveEntityConfigurations.SelectMany<EntityTypeConfiguration, Type>((Func<EntityTypeConfiguration, IEnumerable<Type>>) (ec => ec.ConfiguredComplexTypes)).Each<Type, ComplexTypeConfiguration>((Func<Type, ComplexTypeConfiguration>) (t => this.ComplexType(t)));
    }

    private void ReassignSubtypeMappings()
    {
      foreach (EntityTypeConfiguration entityConfiguration in this.ActiveEntityConfigurations)
      {
        foreach (KeyValuePair<Type, EntityMappingConfiguration> mappingConfiguration in entityConfiguration.SubTypeMappingConfigurations)
        {
          Type subTypeClrType = mappingConfiguration.Key;
          EntityTypeConfiguration typeConfiguration = this.ActiveEntityConfigurations.SingleOrDefault<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (ec => ec.ClrType == subTypeClrType));
          if (typeConfiguration == null)
          {
            typeConfiguration = new EntityTypeConfiguration(subTypeClrType);
            this._entityConfigurations.Add(subTypeClrType, typeConfiguration);
          }
          typeConfiguration.AddMappingConfiguration(mappingConfiguration.Value, false);
        }
      }
    }

    private static void RemoveDuplicateTphColumns(DbDatabaseMapping databaseMapping)
    {
      foreach (EntityType entityType in databaseMapping.Database.EntityTypes)
      {
        EntityType currentTable = entityType;
        new TphColumnFixer(databaseMapping.GetEntitySetMappings().SelectMany<EntitySetMapping, EntityTypeMapping>((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (e => (IEnumerable<EntityTypeMapping>) e.EntityTypeMappings)).SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (e => (IEnumerable<MappingFragment>) e.MappingFragments)).Where<MappingFragment>((Func<MappingFragment, bool>) (f => f.Table == currentTable)).SelectMany<MappingFragment, ColumnMappingBuilder>((Func<MappingFragment, IEnumerable<ColumnMappingBuilder>>) (f => f.ColumnMappings)), currentTable, databaseMapping.Database).RemoveDuplicateTphColumns();
      }
    }

    private static void RemoveRedundantColumnConditions(DbDatabaseMapping databaseMapping)
    {
      databaseMapping.GetEntitySetMappings().Select(esm => new
      {
        Set = esm,
        Fragments = esm.EntityTypeMappings.SelectMany((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments), (etm, etmf) => new
        {
          etm = etm,
          etmf = etmf
        }).GroupBy(_param0 => _param0.etmf.Table, _param0 => _param0.etmf).Where<IGrouping<EntityType, MappingFragment>>((Func<IGrouping<EntityType, MappingFragment>, bool>) (g => g.Count<MappingFragment>((Func<MappingFragment, bool>) (x => x.GetDefaultDiscriminator() != null)) == 1)).Select<IGrouping<EntityType, MappingFragment>, MappingFragment>((Func<IGrouping<EntityType, MappingFragment>, MappingFragment>) (g => g.Single<MappingFragment>((Func<MappingFragment, bool>) (x => x.GetDefaultDiscriminator() != null))))
      }).Each(x => x.Fragments.Each<MappingFragment>((Action<MappingFragment>) (f => f.RemoveDefaultDiscriminator(x.Set))));
    }

    private static void RemoveRedundantTables(DbDatabaseMapping databaseMapping)
    {
      databaseMapping.Database.EntityTypes.Where<EntityType>((Func<EntityType, bool>) (t =>
      {
        if (databaseMapping.GetEntitySetMappings().SelectMany<EntitySetMapping, EntityTypeMapping>((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (esm => (IEnumerable<EntityTypeMapping>) esm.EntityTypeMappings)).SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments)).All<MappingFragment>((Func<MappingFragment, bool>) (etmf => etmf.Table != t)))
          return databaseMapping.GetAssociationSetMappings().All<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm => asm.Table != t));
        return false;
      })).ToList<EntityType>().Each<EntityType>((Action<EntityType>) (t =>
      {
        DatabaseName tableName = t.GetTableName();
        if (tableName != null)
          throw Error.OrphanedConfiguredTableDetected((object) tableName);
        databaseMapping.Database.RemoveEntityType(t);
        databaseMapping.Database.AssociationTypes.Where<AssociationType>((Func<AssociationType, bool>) (at =>
        {
          if (at.SourceEnd.GetEntityType() != t)
            return at.TargetEnd.GetEntityType() == t;
          return true;
        })).ToList<AssociationType>().Each<AssociationType>((Action<AssociationType>) (at => databaseMapping.Database.RemoveAssociationType(at)));
      }));
    }

    private IEnumerable<EntityTypeConfiguration> ActiveEntityConfigurations
    {
      get
      {
        return (IEnumerable<EntityTypeConfiguration>) this._entityConfigurations.Where<KeyValuePair<Type, EntityTypeConfiguration>>((Func<KeyValuePair<Type, EntityTypeConfiguration>, bool>) (keyValuePair => !this._ignoredTypes.Contains(keyValuePair.Key))).Select<KeyValuePair<Type, EntityTypeConfiguration>, EntityTypeConfiguration>((Func<KeyValuePair<Type, EntityTypeConfiguration>, EntityTypeConfiguration>) (keyValuePair => keyValuePair.Value)).ToList<EntityTypeConfiguration>();
      }
    }

    private IEnumerable<ComplexTypeConfiguration> ActiveComplexTypeConfigurations
    {
      get
      {
        return (IEnumerable<ComplexTypeConfiguration>) this._complexTypeConfigurations.Where<KeyValuePair<Type, ComplexTypeConfiguration>>((Func<KeyValuePair<Type, ComplexTypeConfiguration>, bool>) (keyValuePair => !this._ignoredTypes.Contains(keyValuePair.Key))).Select<KeyValuePair<Type, ComplexTypeConfiguration>, ComplexTypeConfiguration>((Func<KeyValuePair<Type, ComplexTypeConfiguration>, ComplexTypeConfiguration>) (keyValuePair => keyValuePair.Value)).ToList<ComplexTypeConfiguration>();
      }
    }
  }
}
