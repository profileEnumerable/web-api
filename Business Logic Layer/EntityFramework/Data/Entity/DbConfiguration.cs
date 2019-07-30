// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.DbConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Resources;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Data.Entity
{
  /// <summary>
  /// A class derived from this class can be placed in the same assembly as a class derived from
  /// <see cref="T:System.Data.Entity.DbContext" /> to define Entity Framework configuration for an application.
  /// Configuration is set by calling protected methods and setting protected properties of this
  /// class in the constructor of your derived type.
  /// The type to use can also be registered in the config file of the application.
  /// See http://go.microsoft.com/fwlink/?LinkId=260883 for more information about Entity Framework configuration.
  /// </summary>
  public class DbConfiguration
  {
    private readonly InternalConfiguration _internalConfiguration;

    /// <summary>
    /// Any class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> must have a public parameterless constructor
    /// and that constructor should call this constructor.
    /// </summary>
    protected internal DbConfiguration()
      : this(new InternalConfiguration((ResolverChain) null, (ResolverChain) null, (RootDependencyResolver) null, (AppConfigDependencyResolver) null, (Func<DbDispatchers>) null))
    {
      this._internalConfiguration.Owner = this;
    }

    internal DbConfiguration(InternalConfiguration internalConfiguration)
    {
      this._internalConfiguration = internalConfiguration;
      this._internalConfiguration.Owner = this;
    }

    /// <summary>
    /// The Singleton instance of <see cref="T:System.Data.Entity.DbConfiguration" /> for this app domain. This can be
    /// set at application start before any Entity Framework features have been used and afterwards
    /// should be treated as read-only.
    /// </summary>
    /// <param name="configuration">The instance of <see cref="T:System.Data.Entity.DbConfiguration" />.</param>
    public static void SetConfiguration(DbConfiguration configuration)
    {
      Check.NotNull<DbConfiguration>(configuration, nameof (configuration));
      InternalConfiguration.Instance = configuration.InternalConfiguration;
    }

    /// <summary>
    /// Attempts to discover and load the <see cref="T:System.Data.Entity.DbConfiguration" /> associated with the given
    /// <see cref="T:System.Data.Entity.DbContext" /> type. This method is intended to be used by tooling to ensure that
    /// the correct configuration is loaded into the app domain. Tooling should use this method
    /// before accessing the <see cref="P:System.Data.Entity.DbConfiguration.DependencyResolver" /> property.
    /// </summary>
    /// <param name="contextType">A <see cref="T:System.Data.Entity.DbContext" /> type to use for configuration discovery.</param>
    public static void LoadConfiguration(Type contextType)
    {
      Check.NotNull<Type>(contextType, nameof (contextType));
      if (!typeof (DbContext).IsAssignableFrom(contextType))
        throw new ArgumentException(Strings.BadContextTypeForDiscovery((object) contextType.Name));
      DbConfigurationManager.Instance.EnsureLoadedForContext(contextType);
    }

    /// <summary>
    /// Attempts to discover and load the <see cref="T:System.Data.Entity.DbConfiguration" /> from the given assembly.
    /// This method is intended to be used by tooling to ensure that the correct configuration is loaded into
    /// the app domain. Tooling should use this method before accessing the <see cref="P:System.Data.Entity.DbConfiguration.DependencyResolver" />
    /// property. If the tooling knows the <see cref="T:System.Data.Entity.DbContext" /> type being used, then the
    /// <see cref="M:System.Data.Entity.DbConfiguration.LoadConfiguration(System.Type)" /> method should be used since it gives a greater chance that
    /// the correct configuration will be found.
    /// </summary>
    /// <param name="assemblyHint">An <see cref="T:System.Reflection.Assembly" /> to use for configuration discovery.</param>
    public static void LoadConfiguration(Assembly assemblyHint)
    {
      Check.NotNull<Assembly>(assemblyHint, nameof (assemblyHint));
      DbConfigurationManager.Instance.EnsureLoadedForAssembly(assemblyHint, (Type) null);
    }

    /// <summary>
    /// Occurs during EF initialization after the DbConfiguration has been constructed but just before
    /// it is locked ready for use. Use this event to inspect and/or override services that have been
    /// registered before the configuration is locked. Note that this event should be used carefully
    /// since it may prevent tooling from discovering the same configuration that is used at runtime.
    /// </summary>
    /// <remarks>
    /// Handlers can only be added before EF starts to use the configuration and so handlers should
    /// generally be added as part of application initialization. Do not access the DbConfiguration
    /// static methods inside the handler; instead use the the members of <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.DbConfigurationLoadedEventArgs" />
    /// to get current services and/or add overrides.
    /// </remarks>
    public static event EventHandler<DbConfigurationLoadedEventArgs> Loaded
    {
      add
      {
        Check.NotNull<EventHandler<DbConfigurationLoadedEventArgs>>(value, nameof (value));
        DbConfigurationManager.Instance.AddLoadedHandler(value);
      }
      remove
      {
        Check.NotNull<EventHandler<DbConfigurationLoadedEventArgs>>(value, nameof (value));
        DbConfigurationManager.Instance.RemoveLoadedHandler(value);
      }
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to
    /// add a <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver" /> instance to the Chain of Responsibility of resolvers that
    /// are used to resolve dependencies needed by the Entity Framework.
    /// </summary>
    /// <remarks>
    /// Resolvers are asked to resolve dependencies in reverse order from which they are added. This means
    /// that a resolver can be added to override resolution of a dependency that would already have been
    /// resolved in a different way.
    /// The exceptions to this is that any dependency registered in the application's config file
    /// will always be used in preference to using a dependency resolver added here.
    /// </remarks>
    /// <param name="resolver"> The resolver to add. </param>
    protected internal void AddDependencyResolver(IDbDependencyResolver resolver)
    {
      Check.NotNull<IDbDependencyResolver>(resolver, nameof (resolver));
      this._internalConfiguration.CheckNotLocked(nameof (AddDependencyResolver));
      this._internalConfiguration.AddDependencyResolver(resolver, false);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to
    /// add a <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver" /> instance to the Chain of Responsibility of resolvers that
    /// are used to resolve dependencies needed by the Entity Framework. Unlike the AddDependencyResolver
    /// method, this method puts the resolver at the bottom of the Chain of Responsibility such that it will only
    /// be used to resolve a dependency that could not be resolved by any of the other resolvers.
    /// </summary>
    /// <remarks>
    /// A <see cref="T:System.Data.Entity.Core.Common.DbProviderServices" /> implementation is automatically registered as a default resolver
    /// when it is added with a call to <see cref="M:System.Data.Entity.DbConfiguration.SetProviderServices(System.String,System.Data.Entity.Core.Common.DbProviderServices)" />. This allows EF providers to act as
    /// resolvers for other services that may need to be overridden by the provider.
    /// </remarks>
    /// <param name="resolver"> The resolver to add. </param>
    protected internal void AddDefaultResolver(IDbDependencyResolver resolver)
    {
      Check.NotNull<IDbDependencyResolver>(resolver, nameof (resolver));
      this._internalConfiguration.CheckNotLocked(nameof (AddDefaultResolver));
      this._internalConfiguration.AddDefaultResolver(resolver);
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver" /> that is being used to resolve service
    /// dependencies in the Entity Framework.
    /// </summary>
    public static IDbDependencyResolver DependencyResolver
    {
      get
      {
        return InternalConfiguration.Instance.DependencyResolver;
      }
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to register
    /// an Entity Framework provider.
    /// </summary>
    /// <remarks>
    /// Note that the provider is both registered as a service itself and also registered as a default resolver with
    /// a call to AddDefaultResolver.  This allows EF providers to act as resolvers for other services that
    /// may need to be overridden by the provider.
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Core.Common.DbProviderServices" /> and also using AddDefaultResolver to add the provider as a default
    /// resolver. This means that, if desired, the same functionality can be achieved using a custom resolver or a
    /// resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName"> The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this provider will be used. </param>
    /// <param name="provider"> The provider instance. </param>
    protected internal void SetProviderServices(
      string providerInvariantName,
      DbProviderServices provider)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<DbProviderServices>(provider, nameof (provider));
      this._internalConfiguration.CheckNotLocked(nameof (SetProviderServices));
      this._internalConfiguration.RegisterSingleton<DbProviderServices>(provider, (object) providerInvariantName);
      this.AddDefaultResolver((IDbDependencyResolver) provider);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to register
    /// an ADO.NET provider.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolvers for
    /// <see cref="T:System.Data.Common.DbProviderFactory" /> and <see cref="T:System.Data.Entity.Infrastructure.IProviderInvariantName" />. This means that, if desired,
    /// the same functionality can be achieved using a custom resolver or a resolver backed by an
    /// Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName"> The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this provider will be used. </param>
    /// <param name="providerFactory"> The provider instance. </param>
    protected internal void SetProviderFactory(
      string providerInvariantName,
      DbProviderFactory providerFactory)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<DbProviderFactory>(providerFactory, nameof (providerFactory));
      this._internalConfiguration.CheckNotLocked(nameof (SetProviderFactory));
      this._internalConfiguration.RegisterSingleton<DbProviderFactory>(providerFactory, (object) providerInvariantName);
      this._internalConfiguration.AddDependencyResolver((IDbDependencyResolver) new InvariantNameResolver(providerFactory, providerInvariantName), false);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to register an
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbExecutionStrategy" /> for use with the provider represented by the given invariant name.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbExecutionStrategy" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName"> The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this execution strategy will be used. </param>
    /// <param name="getExecutionStrategy"> A function that returns a new instance of an execution strategy. </param>
    protected internal void SetExecutionStrategy(
      string providerInvariantName,
      Func<IDbExecutionStrategy> getExecutionStrategy)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<Func<IDbExecutionStrategy>>(getExecutionStrategy, nameof (getExecutionStrategy));
      this._internalConfiguration.CheckNotLocked(nameof (SetExecutionStrategy));
      this._internalConfiguration.AddDependencyResolver((IDbDependencyResolver) new ExecutionStrategyResolver<IDbExecutionStrategy>(providerInvariantName, (string) null, getExecutionStrategy), false);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to register an
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbExecutionStrategy" /> for use with the provider represented by the given invariant name and
    /// for a given server name.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using <see cref="M:System.Data.Entity.DbConfiguration.AddDependencyResolver(System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver)" /> to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbExecutionStrategy" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName">
    /// The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this execution strategy will be used.
    /// </param>
    /// <param name="getExecutionStrategy"> A function that returns a new instance of an execution strategy. </param>
    /// <param name="serverName"> A string that will be matched against the server name in the connection string. </param>
    protected internal void SetExecutionStrategy(
      string providerInvariantName,
      Func<IDbExecutionStrategy> getExecutionStrategy,
      string serverName)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotEmpty(serverName, nameof (serverName));
      Check.NotNull<Func<IDbExecutionStrategy>>(getExecutionStrategy, nameof (getExecutionStrategy));
      this._internalConfiguration.CheckNotLocked(nameof (SetExecutionStrategy));
      this._internalConfiguration.AddDependencyResolver((IDbDependencyResolver) new ExecutionStrategyResolver<IDbExecutionStrategy>(providerInvariantName, serverName, getExecutionStrategy), false);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to register a
    /// <see cref="T:System.Data.Entity.Infrastructure.TransactionHandler" />.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using <see cref="M:System.Data.Entity.DbConfiguration.AddDependencyResolver(System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver)" /> to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.TransactionHandler" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="transactionHandlerFactory"> A function that returns a new instance of a transaction handler. </param>
    protected internal void SetDefaultTransactionHandler(
      Func<TransactionHandler> transactionHandlerFactory)
    {
      Check.NotNull<Func<TransactionHandler>>(transactionHandlerFactory, nameof (transactionHandlerFactory));
      this._internalConfiguration.CheckNotLocked("SetTransactionHandler");
      this._internalConfiguration.AddDependencyResolver((IDbDependencyResolver) new TransactionHandlerResolver(transactionHandlerFactory, (string) null, (string) null), false);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to register a
    /// <see cref="T:System.Data.Entity.Infrastructure.TransactionHandler" /> for use with the provider represented by the given invariant name.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using <see cref="M:System.Data.Entity.DbConfiguration.AddDependencyResolver(System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver)" /> to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.TransactionHandler" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName">
    /// The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this transaction handler will be used.
    /// </param>
    /// <param name="transactionHandlerFactory"> A function that returns a new instance of a transaction handler. </param>
    protected internal void SetTransactionHandler(
      string providerInvariantName,
      Func<TransactionHandler> transactionHandlerFactory)
    {
      Check.NotNull<Func<TransactionHandler>>(transactionHandlerFactory, nameof (transactionHandlerFactory));
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      this._internalConfiguration.CheckNotLocked(nameof (SetTransactionHandler));
      this._internalConfiguration.AddDependencyResolver((IDbDependencyResolver) new TransactionHandlerResolver(transactionHandlerFactory, providerInvariantName, (string) null), false);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to register a
    /// <see cref="T:System.Data.Entity.Infrastructure.TransactionHandler" /> for use with the provider represented by the given invariant name and
    /// for a given server name.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using <see cref="M:System.Data.Entity.DbConfiguration.AddDependencyResolver(System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver)" /> to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.TransactionHandler" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName">
    /// The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this transaction handler will be used.
    /// </param>
    /// <param name="transactionHandlerFactory"> A function that returns a new instance of a transaction handler. </param>
    /// <param name="serverName"> A string that will be matched against the server name in the connection string. </param>
    protected internal void SetTransactionHandler(
      string providerInvariantName,
      Func<TransactionHandler> transactionHandlerFactory,
      string serverName)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<Func<TransactionHandler>>(transactionHandlerFactory, nameof (transactionHandlerFactory));
      Check.NotEmpty(serverName, nameof (serverName));
      this._internalConfiguration.CheckNotLocked(nameof (SetTransactionHandler));
      this._internalConfiguration.AddDependencyResolver((IDbDependencyResolver) new TransactionHandlerResolver(transactionHandlerFactory, providerInvariantName, serverName), false);
    }

    /// <summary>
    /// Sets the <see cref="T:System.Data.Entity.Infrastructure.IDbConnectionFactory" /> that is used to create connections by convention if no other
    /// connection string or connection is given to or can be discovered by <see cref="T:System.Data.Entity.DbContext" />.
    /// Note that a default connection factory is set in the app.config or web.config file whenever the
    /// EntityFramework NuGet package is installed. As for all config file settings, the default connection factory
    /// set in the config file will take precedence over any setting made with this method. Therefore the setting
    /// must be removed from the config file before calling this method will have any effect.
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to change
    /// the default connection factory being used.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbConnectionFactory" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="connectionFactory"> The connection factory. </param>
    protected internal void SetDefaultConnectionFactory(IDbConnectionFactory connectionFactory)
    {
      Check.NotNull<IDbConnectionFactory>(connectionFactory, nameof (connectionFactory));
      this._internalConfiguration.CheckNotLocked(nameof (SetDefaultConnectionFactory));
      this._internalConfiguration.RegisterSingleton<IDbConnectionFactory>(connectionFactory);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to
    /// set the pluralization service.
    /// </summary>
    /// <param name="pluralizationService"> The pluralization service to use. </param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pluralization")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "pluralization")]
    protected internal void SetPluralizationService(IPluralizationService pluralizationService)
    {
      Check.NotNull<IPluralizationService>(pluralizationService, nameof (pluralizationService));
      this._internalConfiguration.CheckNotLocked(nameof (SetPluralizationService));
      this._internalConfiguration.RegisterSingleton<IPluralizationService>(pluralizationService);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to
    /// set the database initializer to use for the given context type.  The database initializer is called when a
    /// the given <see cref="T:System.Data.Entity.DbContext" /> type is used to access a database for the first time.
    /// The default strategy for Code First contexts is an instance of <see cref="T:System.Data.Entity.CreateDatabaseIfNotExists`1" />.
    /// </summary>
    /// <remarks>
    /// Calling this method is equivalent to calling <see cref="M:System.Data.Entity.Database.SetInitializer``1(System.Data.Entity.IDatabaseInitializer{``0})" />.
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.IDatabaseInitializer`1" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <typeparam name="TContext"> The type of the context. </typeparam>
    /// <param name="initializer"> The initializer to use, or null to disable initialization for the given context type. </param>
    protected internal void SetDatabaseInitializer<TContext>(
      IDatabaseInitializer<TContext> initializer)
      where TContext : DbContext
    {
      this._internalConfiguration.CheckNotLocked(nameof (SetDatabaseInitializer));
      this._internalConfiguration.RegisterSingleton<IDatabaseInitializer<TContext>>(initializer ?? (IDatabaseInitializer<TContext>) new NullDatabaseInitializer<TContext>());
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to register a
    /// <see cref="T:System.Data.Entity.Migrations.Sql.MigrationSqlGenerator" /> for use with the provider represented by the given invariant name.
    /// </summary>
    /// <remarks>
    /// This method is typically used by providers to register an associated SQL generator for Code First Migrations.
    /// It is different from setting the generator in the <see cref="T:System.Data.Entity.Migrations.DbMigrationsConfiguration" /> because it allows
    /// EF to use the Migrations pipeline to create a database even when there is no Migrations configuration in the project
    /// and/or Migrations are not being explicitly used.
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Migrations.Sql.MigrationSqlGenerator" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName"> The invariant name of the ADO.NET provider for which this generator should be used. </param>
    /// <param name="sqlGenerator"> A delegate that returns a new instance of the SQL generator each time it is called. </param>
    protected internal void SetMigrationSqlGenerator(
      string providerInvariantName,
      Func<MigrationSqlGenerator> sqlGenerator)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<Func<MigrationSqlGenerator>>(sqlGenerator, nameof (sqlGenerator));
      this._internalConfiguration.CheckNotLocked(nameof (SetMigrationSqlGenerator));
      this._internalConfiguration.RegisterSingleton<Func<MigrationSqlGenerator>>(sqlGenerator, (object) providerInvariantName);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// an implementation of <see cref="T:System.Data.Entity.Infrastructure.IManifestTokenResolver" /> which allows provider manifest tokens to
    /// be obtained from connections without necessarily opening the connection.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.IManifestTokenResolver" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="resolver"> The manifest token resolver. </param>
    protected internal void SetManifestTokenResolver(IManifestTokenResolver resolver)
    {
      Check.NotNull<IManifestTokenResolver>(resolver, nameof (resolver));
      this._internalConfiguration.CheckNotLocked(nameof (SetManifestTokenResolver));
      this._internalConfiguration.RegisterSingleton<IManifestTokenResolver>(resolver);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// a factory for implementations of <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /> which allows custom annotations
    /// represented by <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataProperty" /> instances to be serialized to and from the EDMX XML.
    /// </summary>
    /// <remarks>
    /// Note that an <see cref="T:System.Func`1" /> is not needed if the annotation uses a simple string value.
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="annotationName"> The name of custom annotation that will be handled by this serializer. </param>
    /// <param name="serializerFactory"> A delegate that will be used to create serializer instances. </param>
    protected internal void SetMetadataAnnotationSerializer(
      string annotationName,
      Func<IMetadataAnnotationSerializer> serializerFactory)
    {
      Check.NotEmpty(annotationName, nameof (annotationName));
      Check.NotNull<Func<IMetadataAnnotationSerializer>>(serializerFactory, nameof (serializerFactory));
      this._internalConfiguration.CheckNotLocked(nameof (SetMetadataAnnotationSerializer));
      this._internalConfiguration.RegisterSingleton<Func<IMetadataAnnotationSerializer>>(serializerFactory, (object) annotationName);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// an implementation of <see cref="T:System.Data.Entity.Infrastructure.IDbProviderFactoryResolver" /> which allows a <see cref="T:System.Data.Common.DbProviderFactory" />
    /// to be obtained from a <see cref="T:System.Data.Common.DbConnection" /> in cases where the default implementation is not
    /// sufficient.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbProviderFactoryResolver" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerFactoryResolver"> The provider factory service. </param>
    protected internal void SetProviderFactoryResolver(
      IDbProviderFactoryResolver providerFactoryResolver)
    {
      Check.NotNull<IDbProviderFactoryResolver>(providerFactoryResolver, nameof (providerFactoryResolver));
      this._internalConfiguration.CheckNotLocked(nameof (SetProviderFactoryResolver));
      this._internalConfiguration.RegisterSingleton<IDbProviderFactoryResolver>(providerFactoryResolver);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// a <see cref="T:System.Func`2" /> as the model cache key factory which allows the key
    /// used to cache the model behind a <see cref="T:System.Data.Entity.DbContext" /> to be changed.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Func`2" />. This means that, if desired, the same functionality can
    /// be achieved using a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="keyFactory"> The key factory. </param>
    protected internal void SetModelCacheKey(Func<DbContext, IDbModelCacheKey> keyFactory)
    {
      Check.NotNull<Func<DbContext, IDbModelCacheKey>>(keyFactory, nameof (keyFactory));
      this._internalConfiguration.CheckNotLocked(nameof (SetModelCacheKey));
      this._internalConfiguration.RegisterSingleton<Func<DbContext, IDbModelCacheKey>>(keyFactory);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// a <see cref="T:System.Func`3" /> delegate which which be used for
    /// creation of the default  <see cref="T:System.Data.Entity.Migrations.History.HistoryContext" /> for a any
    /// <see cref="T:System.Data.Entity.Migrations.DbMigrationsConfiguration" />. This default factory will only be used if no factory is
    /// set explicitly in the <see cref="T:System.Data.Entity.Migrations.DbMigrationsConfiguration" /> and if no factory has been registered
    /// for the provider in use using the
    /// <see cref="M:System.Data.Entity.DbConfiguration.SetHistoryContext(System.String,System.Func{System.Data.Common.DbConnection,System.String,System.Data.Entity.Migrations.History.HistoryContext})" />
    /// method.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Func`3" />. This means that, if desired, the same functionality
    /// can be achieved using a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="factory">
    /// A factory for creating <see cref="T:System.Data.Entity.Migrations.History.HistoryContext" /> instances for a given <see cref="T:System.Data.Common.DbConnection" /> and
    /// <see cref="T:System.String" /> representing the default schema.
    /// </param>
    protected internal void SetDefaultHistoryContext(
      Func<DbConnection, string, HistoryContext> factory)
    {
      Check.NotNull<Func<DbConnection, string, HistoryContext>>(factory, nameof (factory));
      this._internalConfiguration.CheckNotLocked(nameof (SetDefaultHistoryContext));
      this._internalConfiguration.RegisterSingleton<Func<DbConnection, string, HistoryContext>>(factory);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// a <see cref="T:System.Func`3" /> delegate which allows for creation of a customized
    /// <see cref="T:System.Data.Entity.Migrations.History.HistoryContext" /> for the given provider for any <see cref="T:System.Data.Entity.Migrations.DbMigrationsConfiguration" />
    /// that does not have an explicit factory set.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Func`3" />. This means that, if desired, the same functionality
    /// can be achieved using a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName"> The invariant name of the ADO.NET provider for which this generator should be used. </param>
    /// <param name="factory">
    /// A factory for creating <see cref="T:System.Data.Entity.Migrations.History.HistoryContext" /> instances for a given <see cref="T:System.Data.Common.DbConnection" /> and
    /// <see cref="T:System.String" /> representing the default schema.
    /// </param>
    protected internal void SetHistoryContext(
      string providerInvariantName,
      Func<DbConnection, string, HistoryContext> factory)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<Func<DbConnection, string, HistoryContext>>(factory, nameof (factory));
      this._internalConfiguration.CheckNotLocked(nameof (SetHistoryContext));
      this._internalConfiguration.RegisterSingleton<Func<DbConnection, string, HistoryContext>>(factory, (object) providerInvariantName);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// the global instance of <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" /> which will be used whenever a spatial provider is
    /// required and a provider-specific spatial provider cannot be found. Normally, a provider-specific spatial provider
    /// is obtained from the a <see cref="T:System.Data.Entity.Core.Common.DbProviderServices" /> implementation which is in turn returned by resolving
    /// a service for <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" /> passing the provider invariant name as a key. However, this
    /// cannot work for stand-alone instances of <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> and <see cref="T:System.Data.Entity.Spatial.DbGeography" /> since
    /// it is impossible to know the spatial provider to use. Therefore, when creating stand-alone instances
    /// of <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> and <see cref="T:System.Data.Entity.Spatial.DbGeography" /> the global spatial provider is always used.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="spatialProvider"> The spatial provider. </param>
    protected internal void SetDefaultSpatialServices(DbSpatialServices spatialProvider)
    {
      Check.NotNull<DbSpatialServices>(spatialProvider, nameof (spatialProvider));
      this._internalConfiguration.CheckNotLocked(nameof (SetDefaultSpatialServices));
      this._internalConfiguration.RegisterSingleton<DbSpatialServices>(spatialProvider);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// an implementation of <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" /> to use for a specific provider and provider
    /// manifest token.
    /// </summary>
    /// <remarks>
    /// Use <see cref="M:System.Data.Entity.DbConfiguration.SetSpatialServices(System.Data.Entity.Infrastructure.DbProviderInfo,System.Data.Entity.Spatial.DbSpatialServices)" />
    /// to register spatial services for use only when a specific manifest token is returned by the provider.
    /// Use <see cref="M:System.Data.Entity.DbConfiguration.SetDefaultSpatialServices(System.Data.Entity.Spatial.DbSpatialServices)" /> to register global
    /// spatial services to be used when provider information is not available or no provider-specific
    /// spatial services are found.
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="key">
    /// The <see cref="T:System.Data.Entity.Infrastructure.DbProviderInfo" /> indicating the type of ADO.NET connection for which this spatial provider will be used.
    /// </param>
    /// <param name="spatialProvider"> The spatial provider. </param>
    protected internal void SetSpatialServices(
      DbProviderInfo key,
      DbSpatialServices spatialProvider)
    {
      Check.NotNull<DbProviderInfo>(key, nameof (key));
      Check.NotNull<DbSpatialServices>(spatialProvider, nameof (spatialProvider));
      this._internalConfiguration.CheckNotLocked(nameof (SetSpatialServices));
      this._internalConfiguration.RegisterSingleton<DbSpatialServices>(spatialProvider, (object) key);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// an implementation of <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" /> to use for a specific provider with any
    /// manifest token.
    /// </summary>
    /// <remarks>
    /// Use <see cref="M:System.Data.Entity.DbConfiguration.SetSpatialServices(System.String,System.Data.Entity.Spatial.DbSpatialServices)" />
    /// to register spatial services for use when any manifest token is returned by the provider.
    /// Use <see cref="M:System.Data.Entity.DbConfiguration.SetDefaultSpatialServices(System.Data.Entity.Spatial.DbSpatialServices)" /> to register global
    /// spatial services to be used when provider information is not available or no provider-specific
    /// spatial services are found.
    /// 
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName"> The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this spatial provider will be used. </param>
    /// <param name="spatialProvider"> The spatial provider. </param>
    protected internal void SetSpatialServices(
      string providerInvariantName,
      DbSpatialServices spatialProvider)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<DbSpatialServices>(spatialProvider, nameof (spatialProvider));
      this._internalConfiguration.CheckNotLocked(nameof (SetSpatialServices));
      this.RegisterSpatialServices(providerInvariantName, spatialProvider);
    }

    private void RegisterSpatialServices(
      string providerInvariantName,
      DbSpatialServices spatialProvider)
    {
      this._internalConfiguration.RegisterSingleton<DbSpatialServices>(spatialProvider, (Func<object, bool>) (k =>
      {
        DbProviderInfo dbProviderInfo = k as DbProviderInfo;
        if (dbProviderInfo != null)
          return dbProviderInfo.ProviderInvariantName == providerInvariantName;
        return false;
      }));
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// a factory for the type of <see cref="T:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter" /> to use with <see cref="P:System.Data.Entity.Database.Log" />.
    /// </summary>
    /// <remarks>
    /// Note that setting the type of formatter to use with this method does change the way command are
    /// logged when <see cref="P:System.Data.Entity.Database.Log" /> is used. It is still necessary to set a <see cref="T:System.IO.TextWriter" />
    /// instance onto <see cref="P:System.Data.Entity.Database.Log" /> before any commands will be logged.
    /// For more low-level control over logging/interception see <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" /> and
    /// <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" />.
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Func`1" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="logFormatterFactory">A delegate that will create formatter instances.</param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    protected internal void SetDatabaseLogFormatter(
      Func<DbContext, Action<string>, DatabaseLogFormatter> logFormatterFactory)
    {
      Check.NotNull<Func<DbContext, Action<string>, DatabaseLogFormatter>>(logFormatterFactory, nameof (logFormatterFactory));
      this._internalConfiguration.CheckNotLocked(nameof (SetDatabaseLogFormatter));
      this._internalConfiguration.RegisterSingleton<Func<DbContext, Action<string>, DatabaseLogFormatter>>(logFormatterFactory);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to
    /// register an <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbInterceptor" /> at application startup. Note that interceptors can also
    /// be added and removed at any time using <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" />.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbInterceptor" />. This means that, if desired, the same functionality can be achieved using
    /// a custom resolver or a resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="interceptor">The interceptor to register.</param>
    protected internal void AddInterceptor(IDbInterceptor interceptor)
    {
      Check.NotNull<IDbInterceptor>(interceptor, nameof (interceptor));
      this._internalConfiguration.CheckNotLocked(nameof (AddInterceptor));
      this._internalConfiguration.RegisterSingleton<IDbInterceptor>(interceptor);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// a factory to allow <see cref="T:System.Data.Entity.Infrastructure.DbContextInfo" /> to create instances of a context that does not have a public,
    /// parameterless constructor.
    /// </summary>
    /// <remarks>
    /// This is typically needed to allow design-time tools like Migrations or scaffolding code to use contexts that
    /// do not have public, parameterless constructors.
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Func`1" /> with the context <see cref="T:System.Type" /> as the key. This means that, if desired,
    /// the same functionality can be achieved using a custom resolver or a resolver backed by an
    /// Inversion-of-Control container.
    /// </remarks>
    /// <param name="contextType">The context type for which the factory should be used.</param>
    /// <param name="factory">The delegate to use to create context instances.</param>
    protected internal void SetContextFactory(Type contextType, Func<DbContext> factory)
    {
      Check.NotNull<Type>(contextType, nameof (contextType));
      Check.NotNull<Func<DbContext>>(factory, nameof (factory));
      if (!typeof (DbContext).IsAssignableFrom(contextType))
        throw new ArgumentException(Strings.ContextFactoryContextType((object) contextType.FullName));
      this._internalConfiguration.CheckNotLocked(nameof (SetContextFactory));
      this._internalConfiguration.RegisterSingleton<Func<DbContext>>(factory, (object) contextType);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to set
    /// a factory to allow <see cref="T:System.Data.Entity.Infrastructure.DbContextInfo" /> to create instances of a context that does not have a public,
    /// parameterless constructor.
    /// </summary>
    /// <remarks>
    /// This is typically needed to allow design-time tools like Migrations or scaffolding code to use contexts that
    /// do not have public, parameterless constructors.
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Func`1" /> with the context <see cref="T:System.Type" /> as the key. This means that, if desired,
    /// the same functionality can be achieved using a custom resolver or a resolver backed by an
    /// Inversion-of-Control container.
    /// </remarks>
    /// <typeparam name="TContext">The context type for which the factory should be used.</typeparam>
    /// <param name="factory">The delegate to use to create context instances.</param>
    protected internal void SetContextFactory<TContext>(Func<TContext> factory) where TContext : DbContext
    {
      Check.NotNull<Func<TContext>>(factory, nameof (factory));
      this.SetContextFactory(typeof (TContext), (Func<DbContext>) factory);
    }

    /// <summary>
    /// Call this method from the constructor of a class derived from <see cref="T:System.Data.Entity.DbConfiguration" /> to register
    /// a database table existence checker for a given provider.
    /// </summary>
    /// <remarks>
    /// This method is provided as a convenient and discoverable way to add configuration to the Entity Framework.
    /// Internally it works in the same way as using AddDependencyResolver to add an appropriate resolver for
    /// <see cref="T:System.Data.Entity.Core.Common.DbProviderServices" /> and also using AddDefaultResolver to add the provider as a default
    /// resolver. This means that, if desired, the same functionality can be achieved using a custom resolver or a
    /// resolver backed by an Inversion-of-Control container.
    /// </remarks>
    /// <param name="providerInvariantName"> The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this provider will be used. </param>
    /// <param name="tableExistenceChecker"> The table existence checker to use. </param>
    protected internal void SetTableExistenceChecker(
      string providerInvariantName,
      TableExistenceChecker tableExistenceChecker)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<TableExistenceChecker>(tableExistenceChecker, nameof (tableExistenceChecker));
      this._internalConfiguration.CheckNotLocked(nameof (SetTableExistenceChecker));
      this._internalConfiguration.RegisterSingleton<TableExistenceChecker>(tableExistenceChecker, (object) providerInvariantName);
    }

    internal virtual InternalConfiguration InternalConfiguration
    {
      get
      {
        return this._internalConfiguration;
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

    /// <summary>
    /// Creates a shallow copy of the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>A shallow copy of the current <see cref="T:System.Object" />.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new object MemberwiseClone()
    {
      return base.MemberwiseClone();
    }
  }
}
