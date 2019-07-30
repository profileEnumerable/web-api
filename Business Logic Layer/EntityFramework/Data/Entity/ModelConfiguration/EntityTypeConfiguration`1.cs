// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.EntityTypeConfiguration`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration
{
  /// <summary>
  /// Allows configuration to be performed for an entity type in a model.
  /// An EntityTypeConfiguration can be obtained via the Entity method on
  /// <see cref="T:System.Data.Entity.DbModelBuilder" /> or a custom type derived from EntityTypeConfiguration
  /// can be registered via the Configurations property on <see cref="T:System.Data.Entity.DbModelBuilder" />.
  /// </summary>
  /// <typeparam name="TEntityType">The entity type being configured.</typeparam>
  public class EntityTypeConfiguration<TEntityType> : StructuralTypeConfiguration<TEntityType>
    where TEntityType : class
  {
    private readonly EntityTypeConfiguration _entityTypeConfiguration;

    /// <summary>Initializes a new instance of EntityTypeConfiguration</summary>
    public EntityTypeConfiguration()
      : this(new EntityTypeConfiguration(typeof (TEntityType)))
    {
    }

    internal EntityTypeConfiguration(EntityTypeConfiguration entityTypeConfiguration)
    {
      this._entityTypeConfiguration = entityTypeConfiguration;
    }

    internal override StructuralTypeConfiguration Configuration
    {
      get
      {
        return (StructuralTypeConfiguration) this._entityTypeConfiguration;
      }
    }

    internal override TPrimitivePropertyConfiguration Property<TPrimitivePropertyConfiguration>(
      LambdaExpression lambdaExpression)
    {
      return this.Configuration.Property<TPrimitivePropertyConfiguration>(lambdaExpression.GetComplexPropertyAccess(), (Func<TPrimitivePropertyConfiguration>) (() => new TPrimitivePropertyConfiguration()
      {
        OverridableConfigurationParts = OverridableConfigurationParts.None
      }));
    }

    /// <summary>
    /// Configures the primary key property(s) for this entity type.
    /// </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    /// <param name="keyExpression"> A lambda expression representing the property to be used as the primary key. C#: t =&gt; t.Id VB.Net: Function(t) t.Id If the primary key is made up of multiple properties then specify an anonymous type including the properties. C#: t =&gt; new { t.Id1, t.Id2 } VB.Net: Function(t) New With { t.Id1, t.Id2 } </param>
    /// <returns> The same EntityTypeConfiguration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public EntityTypeConfiguration<TEntityType> HasKey<TKey>(
      Expression<Func<TEntityType, TKey>> keyExpression)
    {
      Check.NotNull<Expression<Func<TEntityType, TKey>>>(keyExpression, nameof (keyExpression));
      this._entityTypeConfiguration.Key(keyExpression.GetSimplePropertyAccessList().Select<PropertyPath, PropertyInfo>((Func<PropertyPath, PropertyInfo>) (p => p.Single<PropertyInfo>())));
      return this;
    }

    /// <summary>
    /// Configures the entity set name to be used for this entity type.
    /// The entity set name can only be configured for the base type in each set.
    /// </summary>
    /// <param name="entitySetName"> The name of the entity set. </param>
    /// <returns> The same EntityTypeConfiguration instance so that multiple calls can be chained. </returns>
    public EntityTypeConfiguration<TEntityType> HasEntitySetName(
      string entitySetName)
    {
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      this._entityTypeConfiguration.EntitySetName = entitySetName;
      return this;
    }

    /// <summary>
    /// Excludes a property from the model so that it will not be mapped to the database.
    /// </summary>
    /// <typeparam name="TProperty"> The type of the property to be ignored. </typeparam>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> The same EntityTypeConfiguration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public EntityTypeConfiguration<TEntityType> Ignore<TProperty>(
      Expression<Func<TEntityType, TProperty>> propertyExpression)
    {
      Check.NotNull<Expression<Func<TEntityType, TProperty>>>(propertyExpression, nameof (propertyExpression));
      this.Configuration.Ignore(propertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>());
      return this;
    }

    /// <summary>
    /// Configures the table name that this entity type is mapped to.
    /// </summary>
    /// <param name="tableName"> The name of the table. </param>
    /// <returns> The same EntityTypeConfiguration instance so that multiple calls can be chained. </returns>
    public EntityTypeConfiguration<TEntityType> ToTable(string tableName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      DatabaseName databaseName = DatabaseName.Parse(tableName);
      this._entityTypeConfiguration.ToTable(databaseName.Name, databaseName.Schema);
      return this;
    }

    /// <summary>
    /// Configures the table name that this entity type is mapped to.
    /// </summary>
    /// <param name="tableName"> The name of the table. </param>
    /// <param name="schemaName"> The database schema of the table. </param>
    /// <returns> The same EntityTypeConfiguration instance so that multiple calls can be chained. </returns>
    public EntityTypeConfiguration<TEntityType> ToTable(
      string tableName,
      string schemaName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      this._entityTypeConfiguration.ToTable(tableName, schemaName);
      return this;
    }

    /// <summary>
    /// Sets an annotation in the model for the table to which this entity is mapped. The annotation
    /// value can later be used when processing the table such as when creating migrations.
    /// </summary>
    /// <remarks>
    /// It will likely be necessary to register a <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /> if the type of
    /// the annotation value is anything other than a string. Passing a null value clears any annotation with
    /// the given name on the column that had been previously set.
    /// </remarks>
    /// <param name="name">The annotation name, which must be a valid C#/EDM identifier.</param>
    /// <param name="value">The annotation value, which may be a string or some other type that
    /// can be serialized with an <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /></param>
    /// .
    ///             <returns>The same configuration instance so that multiple calls can be chained.</returns>
    public EntityTypeConfiguration<TEntityType> HasTableAnnotation(
      string name,
      object value)
    {
      Check.NotEmpty(name, nameof (name));
      this._entityTypeConfiguration.SetAnnotation(name, value);
      return this;
    }

    /// <summary>
    /// Configures this type to use stored procedures for insert, update and delete.
    /// The default conventions for procedure and parameter names will be used.
    /// </summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    public EntityTypeConfiguration<TEntityType> MapToStoredProcedures()
    {
      this._entityTypeConfiguration.MapToStoredProcedures();
      return this;
    }

    /// <summary>
    /// Configures this type to use stored procedures for insert, update and delete.
    /// </summary>
    /// <param name="modificationStoredProcedureMappingConfigurationAction">
    /// Configuration to override the default conventions for procedure and parameter names.
    /// </param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public EntityTypeConfiguration<TEntityType> MapToStoredProcedures(
      Action<ModificationStoredProceduresConfiguration<TEntityType>> modificationStoredProcedureMappingConfigurationAction)
    {
      Check.NotNull<Action<ModificationStoredProceduresConfiguration<TEntityType>>>(modificationStoredProcedureMappingConfigurationAction, nameof (modificationStoredProcedureMappingConfigurationAction));
      ModificationStoredProceduresConfiguration<TEntityType> proceduresConfiguration = new ModificationStoredProceduresConfiguration<TEntityType>();
      modificationStoredProcedureMappingConfigurationAction(proceduresConfiguration);
      this._entityTypeConfiguration.MapToStoredProcedures(proceduresConfiguration.Configuration, true);
      return this;
    }

    /// <summary>
    /// Allows advanced configuration related to how this entity type is mapped to the database schema.
    /// By default, any configuration will also apply to any type derived from this entity type.
    /// Derived types can be configured via the overload of Map that configures a derived type or
    /// by using an EntityTypeConfiguration for the derived type.
    /// The properties of an entity can be split between multiple tables using multiple Map calls.
    /// Calls to Map are additive, subsequent calls will not override configuration already preformed via Map.
    /// </summary>
    /// <param name="entityMappingConfigurationAction">
    /// An action that performs configuration against an
    /// <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.EntityMappingConfiguration`1" />
    /// .
    /// </param>
    /// <returns> The same EntityTypeConfiguration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public EntityTypeConfiguration<TEntityType> Map(
      Action<EntityMappingConfiguration<TEntityType>> entityMappingConfigurationAction)
    {
      Check.NotNull<Action<EntityMappingConfiguration<TEntityType>>>(entityMappingConfigurationAction, nameof (entityMappingConfigurationAction));
      EntityMappingConfiguration<TEntityType> mappingConfiguration = new EntityMappingConfiguration<TEntityType>();
      entityMappingConfigurationAction(mappingConfiguration);
      this._entityTypeConfiguration.AddMappingConfiguration(mappingConfiguration.EntityMappingConfigurationInstance, true);
      return this;
    }

    /// <summary>
    /// Allows advanced configuration related to how a derived entity type is mapped to the database schema.
    /// Calls to Map are additive, subsequent calls will not override configuration already preformed via Map.
    /// </summary>
    /// <typeparam name="TDerived"> The derived entity type to be configured. </typeparam>
    /// <param name="derivedTypeMapConfigurationAction">
    /// An action that performs configuration against an
    /// <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.EntityMappingConfiguration`1" />
    /// .
    /// </param>
    /// <returns> The same EntityTypeConfiguration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public EntityTypeConfiguration<TEntityType> Map<TDerived>(
      Action<EntityMappingConfiguration<TDerived>> derivedTypeMapConfigurationAction)
      where TDerived : class, TEntityType
    {
      Check.NotNull<Action<EntityMappingConfiguration<TDerived>>>(derivedTypeMapConfigurationAction, nameof (derivedTypeMapConfigurationAction));
      EntityMappingConfiguration<TDerived> mappingConfiguration = new EntityMappingConfiguration<TDerived>();
      DatabaseName tableName = this._entityTypeConfiguration.GetTableName();
      if (tableName != null)
        mappingConfiguration.EntityMappingConfigurationInstance.TableName = tableName;
      derivedTypeMapConfigurationAction(mappingConfiguration);
      if (typeof (TDerived) == typeof (TEntityType))
        this._entityTypeConfiguration.AddMappingConfiguration(mappingConfiguration.EntityMappingConfigurationInstance, true);
      else
        this._entityTypeConfiguration.AddSubTypeMappingConfiguration(typeof (TDerived), mappingConfiguration.EntityMappingConfigurationInstance);
      return this;
    }

    /// <summary>
    /// Configures an optional relationship from this entity type.
    /// Instances of the entity type will be able to be saved to the database without this relationship being specified.
    /// The foreign key in the database will be nullable.
    /// </summary>
    /// <typeparam name="TTargetEntity"> The type of the entity at the other end of the relationship. </typeparam>
    /// <param name="navigationPropertyExpression"> A lambda expression representing the navigation property for the relationship. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public OptionalNavigationPropertyConfiguration<TEntityType, TTargetEntity> HasOptional<TTargetEntity>(
      Expression<Func<TEntityType, TTargetEntity>> navigationPropertyExpression)
      where TTargetEntity : class
    {
      Check.NotNull<Expression<Func<TEntityType, TTargetEntity>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      return new OptionalNavigationPropertyConfiguration<TEntityType, TTargetEntity>(this._entityTypeConfiguration.Navigation(navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>()));
    }

    /// <summary>
    /// Configures a required relationship from this entity type.
    /// Instances of the entity type will not be able to be saved to the database unless this relationship is specified.
    /// The foreign key in the database will be non-nullable.
    /// </summary>
    /// <typeparam name="TTargetEntity"> The type of the entity at the other end of the relationship. </typeparam>
    /// <param name="navigationPropertyExpression"> A lambda expression representing the navigation property for the relationship. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public RequiredNavigationPropertyConfiguration<TEntityType, TTargetEntity> HasRequired<TTargetEntity>(
      Expression<Func<TEntityType, TTargetEntity>> navigationPropertyExpression)
      where TTargetEntity : class
    {
      Check.NotNull<Expression<Func<TEntityType, TTargetEntity>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      return new RequiredNavigationPropertyConfiguration<TEntityType, TTargetEntity>(this._entityTypeConfiguration.Navigation(navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>()));
    }

    /// <summary>Configures a many relationship from this entity type.</summary>
    /// <typeparam name="TTargetEntity"> The type of the entity at the other end of the relationship. </typeparam>
    /// <param name="navigationPropertyExpression"> A lambda expression representing the navigation property for the relationship. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ManyNavigationPropertyConfiguration<TEntityType, TTargetEntity> HasMany<TTargetEntity>(
      Expression<Func<TEntityType, ICollection<TTargetEntity>>> navigationPropertyExpression)
      where TTargetEntity : class
    {
      Check.NotNull<Expression<Func<TEntityType, ICollection<TTargetEntity>>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      return new ManyNavigationPropertyConfiguration<TEntityType, TTargetEntity>(this._entityTypeConfiguration.Navigation(navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>()));
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

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
