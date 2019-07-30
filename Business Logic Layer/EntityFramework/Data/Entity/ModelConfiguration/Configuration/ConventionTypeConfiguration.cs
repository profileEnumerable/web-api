// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Allows configuration to be performed for an entity type in a model.
  /// This configuration functionality is available via lightweight conventions.
  /// </summary>
  public class ConventionTypeConfiguration
  {
    private static readonly List<ConventionTypeConfiguration.ConfigurationAspect> ConfigurationAspectsConflictingWithIgnoreType = new List<ConventionTypeConfiguration.ConfigurationAspect>()
    {
      ConventionTypeConfiguration.ConfigurationAspect.IsComplexType,
      ConventionTypeConfiguration.ConfigurationAspect.HasEntitySetName,
      ConventionTypeConfiguration.ConfigurationAspect.Ignore,
      ConventionTypeConfiguration.ConfigurationAspect.HasKey,
      ConventionTypeConfiguration.ConfigurationAspect.MapToStoredProcedures,
      ConventionTypeConfiguration.ConfigurationAspect.NavigationProperty,
      ConventionTypeConfiguration.ConfigurationAspect.Property,
      ConventionTypeConfiguration.ConfigurationAspect.ToTable,
      ConventionTypeConfiguration.ConfigurationAspect.HasTableAnnotation
    };
    private static readonly List<ConventionTypeConfiguration.ConfigurationAspect> ConfigurationAspectsConflictingWithComplexType = new List<ConventionTypeConfiguration.ConfigurationAspect>()
    {
      ConventionTypeConfiguration.ConfigurationAspect.HasEntitySetName,
      ConventionTypeConfiguration.ConfigurationAspect.HasKey,
      ConventionTypeConfiguration.ConfigurationAspect.MapToStoredProcedures,
      ConventionTypeConfiguration.ConfigurationAspect.NavigationProperty,
      ConventionTypeConfiguration.ConfigurationAspect.ToTable,
      ConventionTypeConfiguration.ConfigurationAspect.HasTableAnnotation
    };
    private readonly Type _type;
    private readonly Func<EntityTypeConfiguration> _entityTypeConfiguration;
    private readonly System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration _modelConfiguration;
    private readonly Func<ComplexTypeConfiguration> _complexTypeConfiguration;
    private ConventionTypeConfiguration.ConfigurationAspect _currentConfigurationAspect;

    internal ConventionTypeConfiguration(Type type, System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
      : this(type, (Func<EntityTypeConfiguration>) null, (Func<ComplexTypeConfiguration>) null, modelConfiguration)
    {
    }

    internal ConventionTypeConfiguration(
      Type type,
      Func<EntityTypeConfiguration> entityTypeConfiguration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
      : this(type, entityTypeConfiguration, (Func<ComplexTypeConfiguration>) null, modelConfiguration)
    {
    }

    internal ConventionTypeConfiguration(
      Type type,
      Func<ComplexTypeConfiguration> complexTypeConfiguration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
      : this(type, (Func<EntityTypeConfiguration>) null, complexTypeConfiguration, modelConfiguration)
    {
    }

    private ConventionTypeConfiguration(
      Type type,
      Func<EntityTypeConfiguration> entityTypeConfiguration,
      Func<ComplexTypeConfiguration> complexTypeConfiguration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      this._type = type;
      this._entityTypeConfiguration = entityTypeConfiguration;
      this._complexTypeConfiguration = complexTypeConfiguration;
      this._modelConfiguration = modelConfiguration;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of this entity type.
    /// </summary>
    public Type ClrType
    {
      get
      {
        return this._type;
      }
    }

    /// <summary>
    /// Configures the entity set name to be used for this entity type.
    /// The entity set name can only be configured for the base type in each set.
    /// </summary>
    /// <param name="entitySetName"> The name of the entity set. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public ConventionTypeConfiguration HasEntitySetName(
      string entitySetName)
    {
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.HasEntitySetName);
      if (this._entityTypeConfiguration != null && this._entityTypeConfiguration().EntitySetName == null)
        this._entityTypeConfiguration().EntitySetName = entitySetName;
      return this;
    }

    /// <summary>
    /// Excludes this entity type from the model so that it will not be mapped to the database.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    public ConventionTypeConfiguration Ignore()
    {
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.IgnoreType);
      if (this._entityTypeConfiguration == null && this._complexTypeConfiguration == null)
        this._modelConfiguration.Ignore(this._type);
      return this;
    }

    /// <summary>Changes this entity type to a complex type.</summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    public ConventionTypeConfiguration IsComplexType()
    {
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.IsComplexType);
      if (this._entityTypeConfiguration == null && this._complexTypeConfiguration == null)
        this._modelConfiguration.ComplexType(this._type);
      return this;
    }

    /// <summary>
    /// Excludes a property from the model so that it will not be mapped to the database.
    /// </summary>
    /// <param name="propertyName"> The name of the property to be configured. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect if the property does not exist.
    /// </remarks>
    public ConventionTypeConfiguration Ignore(string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      PropertyInfo instanceProperty = this._type.GetInstanceProperty(propertyName);
      if (instanceProperty == (PropertyInfo) null)
        throw new InvalidOperationException(Strings.NoSuchProperty((object) propertyName, (object) this._type.Name));
      this.Ignore(instanceProperty);
      return this;
    }

    /// <summary>
    /// Excludes a property from the model so that it will not be mapped to the database.
    /// </summary>
    /// <param name="propertyInfo"> The property to be configured. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect if the property does not exist.
    /// </remarks>
    public ConventionTypeConfiguration Ignore(PropertyInfo propertyInfo)
    {
      Check.NotNull<PropertyInfo>(propertyInfo, nameof (propertyInfo));
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.Ignore);
      if (propertyInfo != (PropertyInfo) null)
      {
        if (this._entityTypeConfiguration != null)
          this._entityTypeConfiguration().Ignore(propertyInfo);
        if (this._complexTypeConfiguration != null)
          this._complexTypeConfiguration().Ignore(propertyInfo);
      }
      return this;
    }

    /// <summary>Configures a property that is defined on this type.</summary>
    /// <param name="propertyName"> The name of the property being configured. </param>
    /// <returns> A configuration object that can be used to configure the property. </returns>
    public ConventionPrimitivePropertyConfiguration Property(
      string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      PropertyInfo instanceProperty = this._type.GetInstanceProperty(propertyName);
      if (instanceProperty == (PropertyInfo) null)
        throw new InvalidOperationException(Strings.NoSuchProperty((object) propertyName, (object) this._type.Name));
      return this.Property(instanceProperty);
    }

    /// <summary>Configures a property that is defined on this type.</summary>
    /// <param name="propertyInfo"> The property being configured. </param>
    /// <returns> A configuration object that can be used to configure the property. </returns>
    public ConventionPrimitivePropertyConfiguration Property(
      PropertyInfo propertyInfo)
    {
      Check.NotNull<PropertyInfo>(propertyInfo, nameof (propertyInfo));
      return this.Property(new PropertyPath(propertyInfo));
    }

    internal ConventionPrimitivePropertyConfiguration Property(
      PropertyPath propertyPath)
    {
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.Property);
      PropertyInfo propertyInfo = propertyPath.Last<PropertyInfo>();
      if (!propertyInfo.IsValidEdmScalarProperty())
        throw new InvalidOperationException(Strings.LightweightEntityConfiguration_NonScalarProperty((object) propertyPath));
      System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration propertyConfiguration = this._entityTypeConfiguration != null ? this._entityTypeConfiguration().Property(propertyPath, new OverridableConfigurationParts?()) : (this._complexTypeConfiguration != null ? this._complexTypeConfiguration().Property(propertyPath, new OverridableConfigurationParts?()) : (System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration) null);
      return new ConventionPrimitivePropertyConfiguration(propertyInfo, (Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration>) (() => propertyConfiguration));
    }

    internal ConventionNavigationPropertyConfiguration NavigationProperty(
      string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      PropertyInfo instanceProperty = this._type.GetInstanceProperty(propertyName);
      if (instanceProperty == (PropertyInfo) null)
        throw new InvalidOperationException(Strings.NoSuchProperty((object) propertyName, (object) this._type.Name));
      return this.NavigationProperty(instanceProperty);
    }

    internal ConventionNavigationPropertyConfiguration NavigationProperty(
      PropertyInfo propertyInfo)
    {
      Check.NotNull<PropertyInfo>(propertyInfo, nameof (propertyInfo));
      return this.NavigationProperty(new PropertyPath(propertyInfo));
    }

    internal ConventionNavigationPropertyConfiguration NavigationProperty(
      PropertyPath propertyPath)
    {
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.NavigationProperty);
      PropertyInfo propertyInfo = propertyPath.Last<PropertyInfo>();
      if (!propertyInfo.IsValidEdmNavigationProperty())
        throw new InvalidOperationException(Strings.LightweightEntityConfiguration_InvalidNavigationProperty((object) propertyPath));
      return new ConventionNavigationPropertyConfiguration(this._entityTypeConfiguration != null ? this._entityTypeConfiguration().Navigation(propertyInfo) : (NavigationPropertyConfiguration) null, this._modelConfiguration);
    }

    /// <summary>
    /// Configures the primary key property for this entity type.
    /// </summary>
    /// <param name="propertyName"> The name of the property to be used as the primary key. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    public ConventionTypeConfiguration HasKey(string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      PropertyInfo instanceProperty = this._type.GetInstanceProperty(propertyName);
      if (instanceProperty == (PropertyInfo) null)
        throw new InvalidOperationException(Strings.NoSuchProperty((object) propertyName, (object) this._type.Name));
      return this.HasKey(instanceProperty);
    }

    /// <summary>
    /// Configures the primary key property for this entity type.
    /// </summary>
    /// <param name="propertyInfo"> The property to be used as the primary key. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    public ConventionTypeConfiguration HasKey(PropertyInfo propertyInfo)
    {
      Check.NotNull<PropertyInfo>(propertyInfo, nameof (propertyInfo));
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.HasKey);
      if (this._entityTypeConfiguration != null && !this._entityTypeConfiguration().IsKeyConfigured)
        this._entityTypeConfiguration().Key(propertyInfo);
      return this;
    }

    /// <summary>
    /// Configures the primary key property(s) for this entity type.
    /// </summary>
    /// <param name="propertyNames"> The names of the properties to be used as the primary key. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    public ConventionTypeConfiguration HasKey(
      IEnumerable<string> propertyNames)
    {
      Check.NotNull<IEnumerable<string>>(propertyNames, nameof (propertyNames));
      return this.HasKey((IEnumerable<PropertyInfo>) propertyNames.Select<string, PropertyInfo>((Func<string, PropertyInfo>) (n =>
      {
        PropertyInfo instanceProperty = this._type.GetInstanceProperty(n);
        if (instanceProperty == (PropertyInfo) null)
          throw new InvalidOperationException(Strings.NoSuchProperty((object) n, (object) this._type.Name));
        return instanceProperty;
      })).ToArray<PropertyInfo>());
    }

    /// <summary>
    /// Configures the primary key property(s) for this entity type.
    /// </summary>
    /// <param name="keyProperties"> The properties to be used as the primary key. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured or if any
    /// property does not exist.
    /// </remarks>
    public ConventionTypeConfiguration HasKey(
      IEnumerable<PropertyInfo> keyProperties)
    {
      Check.NotNull<IEnumerable<PropertyInfo>>(keyProperties, nameof (keyProperties));
      EntityUtil.CheckArgumentContainsNull<PropertyInfo>(ref keyProperties, nameof (keyProperties));
      EntityUtil.CheckArgumentEmpty<PropertyInfo>(ref keyProperties, (Func<string, string>) (p => Strings.CollectionEmpty((object) p, (object) nameof (HasKey))), nameof (keyProperties));
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.HasKey);
      if (this._entityTypeConfiguration != null && !this._entityTypeConfiguration().IsKeyConfigured)
        this._entityTypeConfiguration().Key(keyProperties);
      return this;
    }

    /// <summary>
    /// Configures the table name that this entity type is mapped to.
    /// </summary>
    /// <param name="tableName"> The name of the table. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public ConventionTypeConfiguration ToTable(string tableName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.ToTable);
      if (this._entityTypeConfiguration != null && !this._entityTypeConfiguration().IsTableNameConfigured)
      {
        DatabaseName databaseName = DatabaseName.Parse(tableName);
        this._entityTypeConfiguration().ToTable(databaseName.Name, databaseName.Schema);
      }
      return this;
    }

    /// <summary>
    /// Configures the table name that this entity type is mapped to.
    /// </summary>
    /// <param name="tableName"> The name of the table. </param>
    /// <param name="schemaName"> The database schema of the table. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public ConventionTypeConfiguration ToTable(
      string tableName,
      string schemaName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.ToTable);
      if (this._entityTypeConfiguration != null && !this._entityTypeConfiguration().IsTableNameConfigured)
        this._entityTypeConfiguration().ToTable(tableName, schemaName);
      return this;
    }

    /// <summary>
    /// Sets an annotation in the model for the table to which this entity is mapped. The annotation
    /// value can later be used when processing the table such as when creating migrations.
    /// </summary>
    /// <remarks>
    /// It will likely be necessary to register a <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /> if the type of
    /// the annotation value is anything other than a string. Calling this method will have no effect if the
    /// annotation with the given name has already been configured.
    /// </remarks>
    /// <param name="name">The annotation name, which must be a valid C#/EDM identifier.</param>
    /// <param name="value">The annotation value, which may be a string or some other type that
    /// can be serialized with an <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /></param>
    /// .
    ///             <returns>The same configuration instance so that multiple calls can be chained.</returns>
    public ConventionTypeConfiguration HasTableAnnotation(
      string name,
      object value)
    {
      Check.NotEmpty(name, nameof (name));
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.HasTableAnnotation);
      if (this._entityTypeConfiguration != null && !this._entityTypeConfiguration().Annotations.ContainsKey(name))
        this._entityTypeConfiguration().SetAnnotation(name, value);
      return this;
    }

    /// <summary>
    /// Configures this type to use stored procedures for insert, update and delete.
    /// The default conventions for procedure and parameter names will be used.
    /// </summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    public ConventionTypeConfiguration MapToStoredProcedures()
    {
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.MapToStoredProcedures);
      if (this._entityTypeConfiguration != null)
        this._entityTypeConfiguration().MapToStoredProcedures();
      return this;
    }

    /// <summary>
    /// Configures this type to use stored procedures for insert, update and delete.
    /// </summary>
    /// <param name="modificationStoredProceduresConfigurationAction">
    /// Configuration to override the default conventions for procedure and parameter names.
    /// </param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    public ConventionTypeConfiguration MapToStoredProcedures(
      Action<ConventionModificationStoredProceduresConfiguration> modificationStoredProceduresConfigurationAction)
    {
      Check.NotNull<Action<ConventionModificationStoredProceduresConfiguration>>(modificationStoredProceduresConfigurationAction, nameof (modificationStoredProceduresConfigurationAction));
      this.ValidateConfiguration(ConventionTypeConfiguration.ConfigurationAspect.MapToStoredProcedures);
      ConventionModificationStoredProceduresConfiguration proceduresConfiguration = new ConventionModificationStoredProceduresConfiguration(this._type);
      modificationStoredProceduresConfigurationAction(proceduresConfiguration);
      this.MapToStoredProcedures(proceduresConfiguration.Configuration);
      return this;
    }

    internal void MapToStoredProcedures(
      ModificationStoredProceduresConfiguration modificationStoredProceduresConfiguration)
    {
      if (this._entityTypeConfiguration == null)
        return;
      this._entityTypeConfiguration().MapToStoredProcedures(modificationStoredProceduresConfiguration, false);
    }

    private void ValidateConfiguration(
      ConventionTypeConfiguration.ConfigurationAspect aspect)
    {
      this._currentConfigurationAspect |= aspect;
      if (this._currentConfigurationAspect.HasFlag((Enum) ConventionTypeConfiguration.ConfigurationAspect.IgnoreType) && ConventionTypeConfiguration.ConfigurationAspectsConflictingWithIgnoreType.Any<ConventionTypeConfiguration.ConfigurationAspect>((Func<ConventionTypeConfiguration.ConfigurationAspect, bool>) (ca => this._currentConfigurationAspect.HasFlag((Enum) ca))))
        throw new InvalidOperationException(Strings.LightweightEntityConfiguration_ConfigurationConflict_IgnoreType((object) ConventionTypeConfiguration.ConfigurationAspectsConflictingWithIgnoreType.First<ConventionTypeConfiguration.ConfigurationAspect>((Func<ConventionTypeConfiguration.ConfigurationAspect, bool>) (ca => this._currentConfigurationAspect.HasFlag((Enum) ca))), (object) this._type.Name));
      if (this._currentConfigurationAspect.HasFlag((Enum) ConventionTypeConfiguration.ConfigurationAspect.IsComplexType) && ConventionTypeConfiguration.ConfigurationAspectsConflictingWithComplexType.Any<ConventionTypeConfiguration.ConfigurationAspect>((Func<ConventionTypeConfiguration.ConfigurationAspect, bool>) (ca => this._currentConfigurationAspect.HasFlag((Enum) ca))))
        throw new InvalidOperationException(Strings.LightweightEntityConfiguration_ConfigurationConflict_ComplexType((object) ConventionTypeConfiguration.ConfigurationAspectsConflictingWithComplexType.First<ConventionTypeConfiguration.ConfigurationAspect>((Func<ConventionTypeConfiguration.ConfigurationAspect, bool>) (ca => this._currentConfigurationAspect.HasFlag((Enum) ca))), (object) this._type.Name));
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

    [Flags]
    private enum ConfigurationAspect : uint
    {
      None = 0,
      HasEntitySetName = 1,
      HasKey = 2,
      IgnoreType = 4,
      Ignore = 8,
      IsComplexType = 16, // 0x00000010
      MapToStoredProcedures = 32, // 0x00000020
      Property = 64, // 0x00000040
      NavigationProperty = 128, // 0x00000080
      ToTable = 256, // 0x00000100
      HasTableAnnotation = 512, // 0x00000200
    }
  }
}
