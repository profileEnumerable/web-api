// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.NotNullConditionConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Mapping;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Configures a condition used to discriminate between types in an inheritance hierarchy based on the values assigned to a property.
  /// This configuration functionality is available via the Code First Fluent API, see <see cref="T:System.Data.Entity.DbModelBuilder" />.
  /// </summary>
  public class NotNullConditionConfiguration
  {
    private readonly EntityMappingConfiguration _entityMappingConfiguration;

    internal PropertyPath PropertyPath { get; set; }

    internal NotNullConditionConfiguration(
      EntityMappingConfiguration entityMapConfiguration,
      PropertyPath propertyPath)
    {
      this._entityMappingConfiguration = entityMapConfiguration;
      this.PropertyPath = propertyPath;
    }

    private NotNullConditionConfiguration(
      EntityMappingConfiguration owner,
      NotNullConditionConfiguration source)
    {
      this._entityMappingConfiguration = owner;
      this.PropertyPath = source.PropertyPath;
    }

    internal virtual NotNullConditionConfiguration Clone(
      EntityMappingConfiguration owner)
    {
      return new NotNullConditionConfiguration(owner, this);
    }

    /// <summary>
    /// Configures the condition to require a value in the property.
    /// Rows that do not have a value assigned to column that this property is stored in are
    /// assumed to be of the base type of this entity type.
    /// </summary>
    public void HasValue()
    {
      this._entityMappingConfiguration.AddNullabilityCondition(this);
    }

    internal void Configure(
      DbDatabaseMapping databaseMapping,
      MappingFragment fragment,
      EntityType entityType)
    {
      IEnumerable<EdmPropertyPath> edmPropertyPath = EntityMappingConfiguration.PropertyPathToEdmPropertyPath(this.PropertyPath, entityType);
      if (edmPropertyPath.Count<EdmPropertyPath>() > 1)
        throw Error.InvalidNotNullCondition((object) this.PropertyPath.ToString(), (object) entityType.Name);
      EdmProperty column = fragment.ColumnMappings.Where<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => pm.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) edmPropertyPath.Single<EdmPropertyPath>()))).Select<ColumnMappingBuilder, EdmProperty>((Func<ColumnMappingBuilder, EdmProperty>) (pm => pm.ColumnProperty)).SingleOrDefault<EdmProperty>();
      if (column == null || !fragment.Table.Properties.Contains(column))
        throw Error.InvalidNotNullCondition((object) this.PropertyPath.ToString(), (object) entityType.Name);
      if (ValueConditionConfiguration.AnyBaseTypeToTableWithoutColumnCondition(databaseMapping, entityType, fragment.Table, column))
        column.Nullable = true;
      new System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration()
      {
        IsNullable = new bool?(false),
        OverridableConfigurationParts = OverridableConfigurationParts.OverridableInSSpace
      }.Configure(edmPropertyPath.Single<EdmPropertyPath>().Last<EdmProperty>());
      fragment.AddNullabilityCondition(column, false);
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
