// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Allows configuration to be performed for an entity type in a model.
  /// This configuration functionality is available via lightweight conventions.
  /// </summary>
  /// <typeparam name="T"> A type inherited by the entity type. </typeparam>
  public class ConventionTypeConfiguration<T> where T : class
  {
    private readonly ConventionTypeConfiguration _configuration;

    internal ConventionTypeConfiguration(
      Type type,
      Func<EntityTypeConfiguration> entityTypeConfiguration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      this._configuration = new ConventionTypeConfiguration(type, entityTypeConfiguration, modelConfiguration);
    }

    internal ConventionTypeConfiguration(
      Type type,
      Func<ComplexTypeConfiguration> complexTypeConfiguration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      this._configuration = new ConventionTypeConfiguration(type, complexTypeConfiguration, modelConfiguration);
    }

    internal ConventionTypeConfiguration(Type type, System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      this._configuration = new ConventionTypeConfiguration(type, modelConfiguration);
    }

    [Conditional("DEBUG")]
    private static void VerifyType(Type type)
    {
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of this entity type.
    /// </summary>
    public Type ClrType
    {
      get
      {
        return this._configuration.ClrType;
      }
    }

    /// <summary>
    /// Configures the entity set name to be used for this entity type.
    /// The entity set name can only be configured for the base type in each set.
    /// </summary>
    /// <param name="entitySetName"> The name of the entity set. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public ConventionTypeConfiguration<T> HasEntitySetName(
      string entitySetName)
    {
      this._configuration.HasEntitySetName(entitySetName);
      return this;
    }

    /// <summary>
    /// Excludes this entity type from the model so that it will not be mapped to the database.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1" /> instance so that multiple calls can be chained.
    /// </returns>
    public ConventionTypeConfiguration<T> Ignore()
    {
      this._configuration.Ignore();
      return this;
    }

    /// <summary>Changes this entity type to a complex type.</summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1" /> instance so that multiple calls can be chained.
    /// </returns>
    public ConventionTypeConfiguration<T> IsComplexType()
    {
      this._configuration.IsComplexType();
      return this;
    }

    /// <summary>
    /// Excludes a property from the model so that it will not be mapped to the database.
    /// </summary>
    /// <typeparam name="TProperty"> The type of the property to be ignored. </typeparam>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1" /> instance so that multiple calls can be chained.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ConventionTypeConfiguration<T> Ignore<TProperty>(
      Expression<Func<T, TProperty>> propertyExpression)
    {
      Check.NotNull<Expression<Func<T, TProperty>>>(propertyExpression, nameof (propertyExpression));
      this._configuration.Ignore(propertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>());
      return this;
    }

    /// <summary>Configures a property that is defined on this type.</summary>
    /// <typeparam name="TProperty"> The type of the property being configured. </typeparam>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to configure the property. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ConventionPrimitivePropertyConfiguration Property<TProperty>(
      Expression<Func<T, TProperty>> propertyExpression)
    {
      Check.NotNull<Expression<Func<T, TProperty>>>(propertyExpression, nameof (propertyExpression));
      return this._configuration.Property(propertyExpression.GetComplexPropertyAccess());
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal ConventionNavigationPropertyConfiguration NavigationProperty<TProperty>(
      Expression<Func<T, TProperty>> propertyExpression)
    {
      Check.NotNull<Expression<Func<T, TProperty>>>(propertyExpression, nameof (propertyExpression));
      return this._configuration.NavigationProperty(propertyExpression.GetComplexPropertyAccess());
    }

    /// <summary>
    /// Configures the primary key property(s) for this entity type.
    /// </summary>
    /// <typeparam name="TProperty"> The type of the key. </typeparam>
    /// <param name="keyExpression"> A lambda expression representing the property to be used as the primary key. C#: t =&gt; t.Id VB.Net: Function(t) t.Id If the primary key is made up of multiple properties then specify an anonymous type including the properties. C#: t =&gt; new { t.Id1, t.Id2 } VB.Net: Function(t) New With { t.Id1, t.Id2 } </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ConventionTypeConfiguration<T> HasKey<TProperty>(
      Expression<Func<T, TProperty>> keyExpression)
    {
      Check.NotNull<Expression<Func<T, TProperty>>>(keyExpression, nameof (keyExpression));
      this._configuration.HasKey(keyExpression.GetSimplePropertyAccessList().Select<PropertyPath, PropertyInfo>((Func<PropertyPath, PropertyInfo>) (p => p.Single<PropertyInfo>())));
      return this;
    }

    /// <summary>
    /// Configures the table name that this entity type is mapped to.
    /// </summary>
    /// <param name="tableName"> The name of the table. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public ConventionTypeConfiguration<T> ToTable(string tableName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      this._configuration.ToTable(tableName);
      return this;
    }

    /// <summary>
    /// Configures the table name that this entity type is mapped to.
    /// </summary>
    /// <param name="tableName"> The name of the table. </param>
    /// <param name="schemaName"> The database schema of the table. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public ConventionTypeConfiguration<T> ToTable(
      string tableName,
      string schemaName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      this._configuration.ToTable(tableName, schemaName);
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
    public ConventionTypeConfiguration<T> HasTableAnnotation(
      string name,
      object value)
    {
      Check.NotEmpty(name, nameof (name));
      this._configuration.HasTableAnnotation(name, value);
      return this;
    }

    /// <summary>
    /// Configures this type to use stored procedures for insert, update and delete.
    /// The default conventions for procedure and parameter names will be used.
    /// </summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    public ConventionTypeConfiguration<T> MapToStoredProcedures()
    {
      this._configuration.MapToStoredProcedures();
      return this;
    }

    /// <summary>
    /// Configures this type to use stored procedures for insert, update and delete.
    /// </summary>
    /// <param name="modificationStoredProceduresConfigurationAction">
    /// Configuration to override the default conventions for procedure and parameter names.
    /// </param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ConventionTypeConfiguration<T> MapToStoredProcedures(
      Action<ModificationStoredProceduresConfiguration<T>> modificationStoredProceduresConfigurationAction)
    {
      Check.NotNull<Action<ModificationStoredProceduresConfiguration<T>>>(modificationStoredProceduresConfigurationAction, nameof (modificationStoredProceduresConfigurationAction));
      ModificationStoredProceduresConfiguration<T> proceduresConfiguration = new ModificationStoredProceduresConfiguration<T>();
      modificationStoredProceduresConfigurationAction(proceduresConfiguration);
      this._configuration.MapToStoredProcedures(proceduresConfiguration.Configuration);
      return this;
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
