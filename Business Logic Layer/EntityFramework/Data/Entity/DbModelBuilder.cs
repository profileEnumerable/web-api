// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.DbModelBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Conventions.Sets;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.ModelConfiguration.Mappers;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity
{
  /// <summary>
  /// DbModelBuilder is used to map CLR classes to a database schema.
  /// This code centric approach to building an Entity Data Model (EDM) model is known as 'Code First'.
  /// </summary>
  /// <remarks>
  /// DbModelBuilder is typically used to configure a model by overriding
  /// DbContext.OnModelCreating(DbModelBuilder)
  /// .
  /// You can also use DbModelBuilder independently of DbContext to build a model and then construct a
  /// <see cref="T:System.Data.Entity.DbContext" /> or <see cref="T:System.Data.Objects.ObjectContext" />.
  /// The recommended approach, however, is to use OnModelCreating in <see cref="T:System.Data.Entity.DbContext" /> as
  /// the workflow is more intuitive and takes care of common tasks, such as caching the created model.
  /// Types that form your model are registered with DbModelBuilder and optional configuration can be
  /// performed by applying data annotations to your classes and/or using the fluent style DbModelBuilder
  /// API.
  /// When the Build method is called a set of conventions are run to discover the initial model.
  /// These conventions will automatically discover aspects of the model, such as primary keys, and
  /// will also process any data annotations that were specified on your classes. Finally
  /// any configuration that was performed using the DbModelBuilder API is applied.
  /// Configuration done via the DbModelBuilder API takes precedence over data annotations which
  /// in turn take precedence over the default conventions.
  /// </remarks>
  public class DbModelBuilder
  {
    private readonly object _lock = new object();
    private readonly System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration _modelConfiguration;
    private readonly ConventionsConfiguration _conventionsConfiguration;
    private readonly DbModelBuilderVersion _modelBuilderVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.DbModelBuilder" /> class.
    /// The process of discovering the initial model will use the set of conventions included
    /// in the most recent version of the Entity Framework installed on your machine.
    /// </summary>
    /// <remarks>
    /// Upgrading to newer versions of the Entity Framework may cause breaking changes
    /// in your application because new conventions may cause the initial model to be
    /// configured differently. There is an alternate constructor that allows a specific
    /// version of conventions to be specified.
    /// </remarks>
    public DbModelBuilder()
      : this(new System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration(), DbModelBuilderVersion.Latest)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.DbModelBuilder" /> class that will use
    /// a specific set of conventions to discover the initial model.
    /// </summary>
    /// <param name="modelBuilderVersion"> The version of conventions to be used. </param>
    public DbModelBuilder(DbModelBuilderVersion modelBuilderVersion)
      : this(new System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration(), modelBuilderVersion)
    {
      if (!Enum.IsDefined(typeof (DbModelBuilderVersion), (object) modelBuilderVersion))
        throw new ArgumentOutOfRangeException(nameof (modelBuilderVersion));
    }

    internal DbModelBuilder(
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration,
      DbModelBuilderVersion modelBuilderVersion = DbModelBuilderVersion.Latest)
      : this(modelConfiguration, new ConventionsConfiguration(DbModelBuilder.SelectConventionSet(modelBuilderVersion)), modelBuilderVersion)
    {
    }

    private static ConventionSet SelectConventionSet(
      DbModelBuilderVersion modelBuilderVersion)
    {
      switch (modelBuilderVersion)
      {
        case DbModelBuilderVersion.Latest:
        case DbModelBuilderVersion.V5_0_Net4:
        case DbModelBuilderVersion.V5_0:
        case DbModelBuilderVersion.V6_0:
          return V2ConventionSet.Conventions;
        case DbModelBuilderVersion.V4_1:
          return V1ConventionSet.Conventions;
        default:
          throw new ArgumentOutOfRangeException(nameof (modelBuilderVersion));
      }
    }

    private DbModelBuilder(
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration,
      ConventionsConfiguration conventionsConfiguration,
      DbModelBuilderVersion modelBuilderVersion = DbModelBuilderVersion.Latest)
    {
      if (!Enum.IsDefined(typeof (DbModelBuilderVersion), (object) modelBuilderVersion))
        throw new ArgumentOutOfRangeException(nameof (modelBuilderVersion));
      this._modelConfiguration = modelConfiguration;
      this._conventionsConfiguration = conventionsConfiguration;
      this._modelBuilderVersion = modelBuilderVersion;
    }

    private DbModelBuilder(DbModelBuilder source)
    {
      this._modelConfiguration = source._modelConfiguration.Clone();
      this._conventionsConfiguration = source._conventionsConfiguration.Clone();
      this._modelBuilderVersion = source._modelBuilderVersion;
    }

    internal virtual DbModelBuilder Clone()
    {
      lock (this._lock)
        return new DbModelBuilder(this);
    }

    internal DbModel BuildDynamicUpdateModel(DbProviderInfo providerInfo)
    {
      DbModel dbModel = this.Build(providerInfo);
      EntityContainerMapping containerMapping = dbModel.DatabaseMapping.EntityContainerMappings.Single<EntityContainerMapping>();
      containerMapping.EntitySetMappings.Each<EntitySetMapping>((Action<EntitySetMapping>) (esm => esm.ClearModificationFunctionMappings()));
      containerMapping.AssociationSetMappings.Each<AssociationSetMapping, AssociationSetModificationFunctionMapping>((Func<AssociationSetMapping, AssociationSetModificationFunctionMapping>) (asm => asm.ModificationFunctionMapping = (AssociationSetModificationFunctionMapping) null));
      return dbModel;
    }

    /// <summary>
    /// Excludes a type from the model. This is used to remove types from the model that were added
    /// by convention during initial model discovery.
    /// </summary>
    /// <typeparam name="T"> The type to be excluded. </typeparam>
    /// <returns> The same DbModelBuilder instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    public virtual DbModelBuilder Ignore<T>() where T : class
    {
      this._modelConfiguration.Ignore(typeof (T));
      return this;
    }

    /// <summary>
    /// Configures the default database schema name. This default database schema name is used
    /// for database objects that do not have an explicitly configured schema name.
    /// </summary>
    /// <param name="schema"> The name of the default database schema. </param>
    /// <returns> The same DbModelBuilder instance so that multiple calls can be chained. </returns>
    public virtual DbModelBuilder HasDefaultSchema(string schema)
    {
      this._modelConfiguration.DefaultSchema = schema;
      return this;
    }

    /// <summary>
    /// Excludes the specified type(s) from the model. This is used to remove types from the model that were added
    /// by convention during initial model discovery.
    /// </summary>
    /// <param name="types"> The types to be excluded from the model. </param>
    /// <returns> The same DbModelBuilder instance so that multiple calls can be chained. </returns>
    public virtual DbModelBuilder Ignore(IEnumerable<Type> types)
    {
      Check.NotNull<IEnumerable<Type>>(types, nameof (types));
      foreach (Type type in types)
        this._modelConfiguration.Ignore(type);
      return this;
    }

    /// <summary>
    /// Registers an entity type as part of the model and returns an object that can be used to
    /// configure the entity. This method can be called multiple times for the same entity to
    /// perform multiple lines of configuration.
    /// </summary>
    /// <typeparam name="TEntityType"> The type to be registered or configured. </typeparam>
    /// <returns> The configuration object for the specified entity type. </returns>
    public virtual EntityTypeConfiguration<TEntityType> Entity<TEntityType>() where TEntityType : class
    {
      return new EntityTypeConfiguration<TEntityType>(this._modelConfiguration.Entity(typeof (TEntityType), true));
    }

    /// <summary>Registers an entity type as part of the model.</summary>
    /// <param name="entityType"> The type to be registered. </param>
    /// <remarks>
    /// This method is provided as a convenience to allow entity types to be registered dynamically
    /// without the need to use MakeGenericMethod in order to call the normal generic Entity method.
    /// This method does not allow further configuration of the entity type using the fluent APIs since
    /// these APIs make extensive use of generic type parameters.
    /// </remarks>
    public virtual void RegisterEntityType(Type entityType)
    {
      Check.NotNull<Type>(entityType, nameof (entityType));
      this.Entity(entityType);
    }

    internal virtual EntityTypeConfiguration Entity(Type entityType)
    {
      EntityTypeConfiguration typeConfiguration = this._modelConfiguration.Entity(entityType);
      typeConfiguration.IsReplaceable = true;
      return typeConfiguration;
    }

    /// <summary>
    /// Registers a type as a complex type in the model and returns an object that can be used to
    /// configure the complex type. This method can be called multiple times for the same type to
    /// perform multiple lines of configuration.
    /// </summary>
    /// <typeparam name="TComplexType"> The type to be registered or configured. </typeparam>
    /// <returns> The configuration object for the specified complex type. </returns>
    public virtual ComplexTypeConfiguration<TComplexType> ComplexType<TComplexType>() where TComplexType : class
    {
      return new ComplexTypeConfiguration<TComplexType>(this._modelConfiguration.ComplexType(typeof (TComplexType)));
    }

    /// <summary>
    /// Begins configuration of a lightweight convention that applies to all entities and complex types in
    /// the model.
    /// </summary>
    /// <returns> A configuration object for the convention. </returns>
    public TypeConventionConfiguration Types()
    {
      return new TypeConventionConfiguration(this._conventionsConfiguration);
    }

    /// <summary>
    /// Begins configuration of a lightweight convention that applies to all entities and complex types
    /// in the model that inherit from or implement the type specified by the generic argument.
    /// This method does not register types as part of the model.
    /// </summary>
    /// <typeparam name="T"> The type of the entities or complex types that this convention will apply to. </typeparam>
    /// <returns> A configuration object for the convention. </returns>
    public TypeConventionConfiguration<T> Types<T>() where T : class
    {
      return new TypeConventionConfiguration<T>(this._conventionsConfiguration);
    }

    /// <summary>
    /// Begins configuration of a lightweight convention that applies to all properties
    /// in the model.
    /// </summary>
    /// <returns> A configuration object for the convention. </returns>
    public PropertyConventionConfiguration Properties()
    {
      return new PropertyConventionConfiguration(this._conventionsConfiguration);
    }

    /// <summary>
    /// Begins configuration of a lightweight convention that applies to all primitive
    /// properties of the specified type in the model.
    /// </summary>
    /// <typeparam name="T"> The type of the properties that the convention will apply to. </typeparam>
    /// <returns> A configuration object for the convention. </returns>
    /// <remarks>
    /// The convention will apply to both nullable and non-nullable properties of the
    /// specified type.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    public PropertyConventionConfiguration Properties<T>()
    {
      if (!typeof (T).IsValidEdmScalarType())
        throw Error.ModelBuilder_PropertyFilterTypeMustBePrimitive((object) typeof (T));
      return new PropertyConventionConfiguration(this._conventionsConfiguration).Where((Func<PropertyInfo, bool>) (p =>
      {
        Type underlyingType;
        p.PropertyType.TryUnwrapNullableType(out underlyingType);
        return underlyingType == typeof (T);
      }));
    }

    /// <summary>
    /// Provides access to the settings of this DbModelBuilder that deal with conventions.
    /// </summary>
    public virtual ConventionsConfiguration Conventions
    {
      get
      {
        return this._conventionsConfiguration;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar" /> for this DbModelBuilder.
    /// The registrar allows derived entity and complex type configurations to be registered with this builder.
    /// </summary>
    public virtual ConfigurationRegistrar Configurations
    {
      get
      {
        return new ConfigurationRegistrar(this._modelConfiguration);
      }
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Infrastructure.DbModel" /> based on the configuration performed using this builder.
    /// The connection is used to determine the database provider being used as this
    /// affects the database layer of the generated model.
    /// </summary>
    /// <param name="providerConnection"> Connection to use to determine provider information. </param>
    /// <returns> The model that was built. </returns>
    public virtual DbModel Build(DbConnection providerConnection)
    {
      Check.NotNull<DbConnection>(providerConnection, nameof (providerConnection));
      DbProviderManifest providerManifest;
      DbProviderInfo providerInfo = providerConnection.GetProviderInfo(out providerManifest);
      return this.Build(providerManifest, providerInfo);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Infrastructure.DbModel" /> based on the configuration performed using this builder.
    /// Provider information must be specified because this affects the database layer of the generated model.
    /// For SqlClient the invariant name is 'System.Data.SqlClient' and the manifest token is the version year (i.e. '2005', '2008' etc.)
    /// </summary>
    /// <param name="providerInfo"> The database provider that the model will be used with. </param>
    /// <returns> The model that was built. </returns>
    public virtual DbModel Build(DbProviderInfo providerInfo)
    {
      Check.NotNull<DbProviderInfo>(providerInfo, nameof (providerInfo));
      return this.Build(DbModelBuilder.GetProviderManifest(providerInfo), providerInfo);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    internal DbModelBuilderVersion Version
    {
      get
      {
        return this._modelBuilderVersion;
      }
    }

    private DbModel Build(DbProviderManifest providerManifest, DbProviderInfo providerInfo)
    {
      double edmVersion = this._modelBuilderVersion.GetEdmVersion();
      DbModelBuilder modelBuilder = this.Clone();
      DbModel model1 = new DbModel(new DbDatabaseMapping()
      {
        Model = EdmModel.CreateConceptualModel(edmVersion),
        Database = EdmModel.CreateStoreModel(providerInfo, providerManifest, edmVersion)
      }, modelBuilder);
      model1.ConceptualModel.Container.AddAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:UseClrTypes", (object) "true");
      this._conventionsConfiguration.ApplyModelConfiguration(this._modelConfiguration);
      this._modelConfiguration.NormalizeConfigurations();
      this.MapTypes(model1.ConceptualModel);
      this._modelConfiguration.Configure(model1.ConceptualModel);
      this._conventionsConfiguration.ApplyConceptualModel(model1);
      model1.ConceptualModel.Validate();
      DbModel model2 = new DbModel(model1.ConceptualModel.GenerateDatabaseMapping(providerInfo, providerManifest), modelBuilder);
      this._conventionsConfiguration.ApplyPluralizingTableNameConvention(model2);
      this._modelConfiguration.Configure(model2.DatabaseMapping, providerManifest);
      this._conventionsConfiguration.ApplyStoreModel(model2);
      this._conventionsConfiguration.ApplyMapping(model2.DatabaseMapping);
      model2.StoreModel.Validate();
      return model2;
    }

    private static DbProviderManifest GetProviderManifest(
      DbProviderInfo providerInfo)
    {
      return DbConfiguration.DependencyResolver.GetService<DbProviderFactory>((object) providerInfo.ProviderInvariantName).GetProviderServices().GetProviderManifest(providerInfo.ProviderManifestToken);
    }

    private void MapTypes(EdmModel model)
    {
      TypeMapper typeMapper = new TypeMapper(new MappingContext(this._modelConfiguration, this._conventionsConfiguration, model, this._modelBuilderVersion, DbConfiguration.DependencyResolver.GetService<AttributeProvider>()));
      IList<Type> typeList1 = this._modelConfiguration.Entities as IList<Type> ?? (IList<Type>) this._modelConfiguration.Entities.ToList<Type>();
      for (int index = 0; index < typeList1.Count; ++index)
      {
        Type type = typeList1[index];
        if (typeMapper.MapEntityType(type) == null)
          throw Error.InvalidEntityType((object) type);
      }
      IList<Type> typeList2 = this._modelConfiguration.ComplexTypes as IList<Type> ?? (IList<Type>) this._modelConfiguration.ComplexTypes.ToList<Type>();
      for (int index = 0; index < typeList2.Count; ++index)
      {
        Type type = typeList2[index];
        if (typeMapper.MapComplexType(type, false) == null)
          throw Error.CodeFirstInvalidComplexType((object) type);
      }
    }

    internal System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration ModelConfiguration
    {
      get
      {
        return this._modelConfiguration;
      }
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
