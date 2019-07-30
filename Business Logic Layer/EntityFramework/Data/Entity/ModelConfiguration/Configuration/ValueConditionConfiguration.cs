// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ValueConditionConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Mapping;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.ModelConfiguration.Edm.Services;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Configures a discriminator column used to differentiate between types in an inheritance hierarchy.
  /// This configuration functionality is available via the Code First Fluent API, see <see cref="T:System.Data.Entity.DbModelBuilder" />.
  /// </summary>
  [DebuggerDisplay("{Discriminator}")]
  public class ValueConditionConfiguration
  {
    private readonly EntityMappingConfiguration _entityMappingConfiguration;
    private System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration _configuration;

    internal string Discriminator { get; set; }

    internal object Value { get; set; }

    internal ValueConditionConfiguration(
      EntityMappingConfiguration entityMapConfiguration,
      string discriminator)
    {
      this._entityMappingConfiguration = entityMapConfiguration;
      this.Discriminator = discriminator;
    }

    private ValueConditionConfiguration(
      EntityMappingConfiguration owner,
      ValueConditionConfiguration source)
    {
      this._entityMappingConfiguration = owner;
      this.Discriminator = source.Discriminator;
      this.Value = source.Value;
      this._configuration = source._configuration == null ? (System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration) null : source._configuration.Clone();
    }

    internal virtual ValueConditionConfiguration Clone(
      EntityMappingConfiguration owner)
    {
      return new ValueConditionConfiguration(owner, this);
    }

    private T GetOrCreateConfiguration<T>() where T : System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration, new()
    {
      if (this._configuration == null)
        this._configuration = (System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration) new T();
      else if (!(this._configuration is T))
      {
        T obj = new T();
        obj.CopyFrom(this._configuration);
        this._configuration = (System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration) obj;
      }
      this._configuration.OverridableConfigurationParts = OverridableConfigurationParts.None;
      return (T) this._configuration;
    }

    /// <summary>
    /// Configures the discriminator value used to identify the entity type being
    /// configured from other types in the inheritance hierarchy.
    /// </summary>
    /// <typeparam name="T"> Type of the discriminator value. </typeparam>
    /// <param name="value"> The value to be used to identify the entity type. </param>
    /// <returns> A configuration object to configure the column used to store discriminator values. </returns>
    public PrimitiveColumnConfiguration HasValue<T>(T value) where T : struct
    {
      ValueConditionConfiguration.ValidateValueType((object) value);
      this.Value = (object) value;
      this._entityMappingConfiguration.AddValueCondition(this);
      return new PrimitiveColumnConfiguration(this.GetOrCreateConfiguration<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>());
    }

    /// <summary>
    /// Configures the discriminator value used to identify the entity type being
    /// configured from other types in the inheritance hierarchy.
    /// </summary>
    /// <typeparam name="T"> Type of the discriminator value. </typeparam>
    /// <param name="value"> The value to be used to identify the entity type. </param>
    /// <returns> A configuration object to configure the column used to store discriminator values. </returns>
    public PrimitiveColumnConfiguration HasValue<T>(T? value) where T : struct
    {
      ValueConditionConfiguration.ValidateValueType((object) value);
      this.Value = (object) value;
      this._entityMappingConfiguration.AddValueCondition(this);
      return new PrimitiveColumnConfiguration(this.GetOrCreateConfiguration<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>());
    }

    /// <summary>
    /// Configures the discriminator value used to identify the entity type being
    /// configured from other types in the inheritance hierarchy.
    /// </summary>
    /// <param name="value"> The value to be used to identify the entity type. </param>
    /// <returns> A configuration object to configure the column used to store discriminator values. </returns>
    public StringColumnConfiguration HasValue(string value)
    {
      this.Value = (object) value;
      this._entityMappingConfiguration.AddValueCondition(this);
      return new StringColumnConfiguration(this.GetOrCreateConfiguration<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration>());
    }

    private static void ValidateValueType(object value)
    {
      PrimitiveType primitiveType;
      if (value != null && !value.GetType().IsPrimitiveType(out primitiveType))
        throw Error.InvalidDiscriminatorType((object) value.GetType().Name);
    }

    internal static IEnumerable<MappingFragment> GetMappingFragmentsWithColumnAsDefaultDiscriminator(
      DbDatabaseMapping databaseMapping,
      EntityType table,
      EdmProperty column)
    {
      return databaseMapping.EntityContainerMappings.SelectMany<EntityContainerMapping, EntitySetMapping>((Func<EntityContainerMapping, IEnumerable<EntitySetMapping>>) (ecm => ecm.EntitySetMappings)).SelectMany<EntitySetMapping, EntityTypeMapping>((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (esm => (IEnumerable<EntityTypeMapping>) esm.EntityTypeMappings)).SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments)).Where<MappingFragment>((Func<MappingFragment, bool>) (tmf =>
      {
        if (tmf.Table == table)
          return tmf.GetDefaultDiscriminator() == column;
        return false;
      }));
    }

    internal static bool AnyBaseTypeToTableWithoutColumnCondition(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      EntityType table,
      EdmProperty column)
    {
      for (EdmType baseType = entityType.BaseType; baseType != null; baseType = baseType.BaseType)
      {
        if (!baseType.Abstract)
        {
          List<MappingFragment> list = databaseMapping.GetEntityTypeMappings((EntityType) baseType).SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments)).Where<MappingFragment>((Func<MappingFragment, bool>) (tmf => tmf.Table == table)).ToList<MappingFragment>();
          if (list.Any<MappingFragment>() && list.SelectMany<MappingFragment, ConditionPropertyMapping>((Func<MappingFragment, IEnumerable<ConditionPropertyMapping>>) (etmf => etmf.ColumnConditions)).All<ConditionPropertyMapping>((Func<ConditionPropertyMapping, bool>) (cc => cc.Column != column)))
            return true;
        }
      }
      return false;
    }

    internal void Configure(
      DbDatabaseMapping databaseMapping,
      MappingFragment fragment,
      EntityType entityType,
      DbProviderManifest providerManifest)
    {
      EdmProperty edmProperty = fragment.Table.Properties.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (c => string.Equals(c.Name, this.Discriminator, StringComparison.Ordinal)));
      if (edmProperty != null && ValueConditionConfiguration.GetMappingFragmentsWithColumnAsDefaultDiscriminator(databaseMapping, fragment.Table, edmProperty).Any<MappingFragment>())
      {
        edmProperty.Name = fragment.Table.Properties.Select<EdmProperty, string>((Func<EdmProperty, string>) (p => p.Name)).Uniquify(edmProperty.Name);
        edmProperty = (EdmProperty) null;
      }
      if (edmProperty == null)
      {
        edmProperty = new EdmProperty(this.Discriminator, providerManifest.GetStoreType(DatabaseMappingGenerator.DiscriminatorTypeUsage))
        {
          Nullable = false
        };
        TablePrimitiveOperations.AddColumn(fragment.Table, edmProperty);
      }
      if (ValueConditionConfiguration.AnyBaseTypeToTableWithoutColumnCondition(databaseMapping, entityType, fragment.Table, edmProperty))
        edmProperty.Nullable = true;
      System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration configuration = edmProperty.GetConfiguration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration;
      if (this.Value != null)
      {
        this.ConfigureColumnType(providerManifest, configuration, edmProperty);
        fragment.AddDiscriminatorCondition(edmProperty, this.Value);
      }
      else
      {
        if (string.IsNullOrWhiteSpace(edmProperty.TypeName))
        {
          TypeUsage storeType = providerManifest.GetStoreType(DatabaseMappingGenerator.DiscriminatorTypeUsage);
          edmProperty.PrimitiveType = (PrimitiveType) storeType.EdmType;
          edmProperty.MaxLength = new int?(128);
          edmProperty.Nullable = false;
        }
        this.GetOrCreateConfiguration<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>().IsNullable = new bool?(true);
        fragment.AddNullabilityCondition(edmProperty, true);
      }
      if (this._configuration == null)
        return;
      string errorMessage;
      if (configuration != null && (configuration.OverridableConfigurationParts & OverridableConfigurationParts.OverridableInCSpace) != OverridableConfigurationParts.OverridableInCSpace && !configuration.IsCompatible(this._configuration, true, out errorMessage))
        throw Error.ConflictingColumnConfiguration((object) edmProperty, (object) fragment.Table, (object) errorMessage);
      if (this._configuration.IsNullable.HasValue)
        edmProperty.Nullable = this._configuration.IsNullable.Value;
      this._configuration.Configure(edmProperty, fragment.Table, providerManifest, false, false);
    }

    private void ConfigureColumnType(
      DbProviderManifest providerManifest,
      System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration existingConfiguration,
      EdmProperty discriminatorColumn)
    {
      if (existingConfiguration != null && existingConfiguration.ColumnType != null || this._configuration != null && this._configuration.ColumnType != null)
        return;
      PrimitiveType primitiveType;
      this.Value.GetType().IsPrimitiveType(out primitiveType);
      PrimitiveType edmType = (PrimitiveType) providerManifest.GetStoreType(primitiveType == PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.String) ? DatabaseMappingGenerator.DiscriminatorTypeUsage : TypeUsage.Create((EdmType) PrimitiveType.GetEdmPrimitiveType(primitiveType.PrimitiveTypeKind))).EdmType;
      if (existingConfiguration != null && !discriminatorColumn.TypeName.Equals(edmType.Name, StringComparison.OrdinalIgnoreCase))
        throw Error.ConflictingInferredColumnType((object) discriminatorColumn.Name, (object) discriminatorColumn.TypeName, (object) edmType.Name);
      discriminatorColumn.PrimitiveType = edmType;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the current instance.
    /// </summary>
    /// <returns>The exact runtime type of the current instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
