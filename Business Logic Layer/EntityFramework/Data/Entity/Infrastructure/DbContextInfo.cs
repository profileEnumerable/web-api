// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbContextInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Provides runtime information about a given <see cref="T:System.Data.Entity.DbContext" /> type.
  /// </summary>
  public class DbContextInfo
  {
    private readonly Func<IDbDependencyResolver> _resolver = (Func<IDbDependencyResolver>) (() => DbConfiguration.DependencyResolver);
    [ThreadStatic]
    private static DbContextInfo _currentInfo;
    private readonly Type _contextType;
    private readonly DbProviderInfo _modelProviderInfo;
    private readonly DbConnectionInfo _connectionInfo;
    private readonly AppConfig _appConfig;
    private readonly Func<DbContext> _activator;
    private readonly string _connectionString;
    private readonly string _connectionProviderName;
    private readonly bool _isConstructible;
    private readonly DbConnectionStringOrigin _connectionStringOrigin;
    private readonly string _connectionStringName;
    private Action<DbModelBuilder> _onModelCreating;

    /// <summary>
    /// Creates a new instance representing a given <see cref="T:System.Data.Entity.DbContext" /> type.
    /// </summary>
    /// <param name="contextType">
    /// The type deriving from <see cref="T:System.Data.Entity.DbContext" />.
    /// </param>
    public DbContextInfo(Type contextType)
      : this(contextType, (Func<IDbDependencyResolver>) null)
    {
    }

    internal DbContextInfo(Type contextType, Func<IDbDependencyResolver> resolver)
      : this(Check.NotNull<Type>(contextType, nameof (contextType)), (DbProviderInfo) null, AppConfig.DefaultInstance, (DbConnectionInfo) null, resolver)
    {
    }

    /// <summary>
    /// Creates a new instance representing a given <see cref="T:System.Data.Entity.DbContext" /> targeting a specific database.
    /// </summary>
    /// <param name="contextType">
    /// The type deriving from <see cref="T:System.Data.Entity.DbContext" />.
    /// </param>
    /// <param name="connectionInfo"> Connection information for the database to be used. </param>
    public DbContextInfo(Type contextType, DbConnectionInfo connectionInfo)
      : this(Check.NotNull<Type>(contextType, nameof (contextType)), (DbProviderInfo) null, AppConfig.DefaultInstance, Check.NotNull<DbConnectionInfo>(connectionInfo, nameof (connectionInfo)), (Func<IDbDependencyResolver>) null)
    {
    }

    /// <summary>
    /// Creates a new instance representing a given <see cref="T:System.Data.Entity.DbContext" /> type. An external list of
    /// connection strings can be supplied and will be used during connection string resolution in place
    /// of any connection strings specified in external configuration files.
    /// </summary>
    /// <remarks>
    /// It is preferable to use the constructor that accepts the entire config document instead of using this
    /// constructor. Providing the entire config document allows DefaultConnectionFactroy entries in the config
    /// to be found in addition to explicitly specified connection strings.
    /// </remarks>
    /// <param name="contextType">
    /// The type deriving from <see cref="T:System.Data.Entity.DbContext" />.
    /// </param>
    /// <param name="connectionStringSettings"> A collection of connection strings. </param>
    [Obsolete("The application configuration can contain multiple settings that affect the connection used by a DbContext. To ensure all configuration is taken into account, use a DbContextInfo constructor that accepts System.Configuration.Configuration")]
    public DbContextInfo(
      Type contextType,
      ConnectionStringSettingsCollection connectionStringSettings)
      : this(Check.NotNull<Type>(contextType, nameof (contextType)), (DbProviderInfo) null, new AppConfig(Check.NotNull<ConnectionStringSettingsCollection>(connectionStringSettings, nameof (connectionStringSettings))), (DbConnectionInfo) null, (Func<IDbDependencyResolver>) null)
    {
    }

    /// <summary>
    /// Creates a new instance representing a given <see cref="T:System.Data.Entity.DbContext" /> type. An external config
    /// object (e.g. app.config or web.config) can be supplied and will be used during connection string
    /// resolution. This includes looking for connection strings and DefaultConnectionFactory entries.
    /// </summary>
    /// <param name="contextType">
    /// The type deriving from <see cref="T:System.Data.Entity.DbContext" />.
    /// </param>
    /// <param name="config"> An object representing the config file. </param>
    public DbContextInfo(Type contextType, System.Configuration.Configuration config)
      : this(Check.NotNull<Type>(contextType, nameof (contextType)), (DbProviderInfo) null, new AppConfig(Check.NotNull<System.Configuration.Configuration>(config, nameof (config))), (DbConnectionInfo) null, (Func<IDbDependencyResolver>) null)
    {
    }

    /// <summary>
    /// Creates a new instance representing a given <see cref="T:System.Data.Entity.DbContext" />, targeting a specific database.
    /// An external config object (e.g. app.config or web.config) can be supplied and will be used during connection string
    /// resolution. This includes looking for connection strings and DefaultConnectionFactory entries.
    /// </summary>
    /// <param name="contextType">
    /// The type deriving from <see cref="T:System.Data.Entity.DbContext" />.
    /// </param>
    /// <param name="config"> An object representing the config file. </param>
    /// <param name="connectionInfo"> Connection information for the database to be used. </param>
    public DbContextInfo(Type contextType, System.Configuration.Configuration config, DbConnectionInfo connectionInfo)
      : this(Check.NotNull<Type>(contextType, nameof (contextType)), (DbProviderInfo) null, new AppConfig(Check.NotNull<System.Configuration.Configuration>(config, nameof (config))), Check.NotNull<DbConnectionInfo>(connectionInfo, nameof (connectionInfo)), (Func<IDbDependencyResolver>) null)
    {
    }

    /// <summary>
    /// Creates a new instance representing a given <see cref="T:System.Data.Entity.DbContext" /> type.  A <see cref="T:System.Data.Entity.Infrastructure.DbProviderInfo" />
    /// can be supplied in order to override the default determined provider used when constructing
    /// the underlying EDM model.
    /// </summary>
    /// <param name="contextType">
    /// The type deriving from <see cref="T:System.Data.Entity.DbContext" />.
    /// </param>
    /// <param name="modelProviderInfo">
    /// A <see cref="T:System.Data.Entity.Infrastructure.DbProviderInfo" /> specifying the underlying ADO.NET provider to target.
    /// </param>
    public DbContextInfo(Type contextType, DbProviderInfo modelProviderInfo)
      : this(Check.NotNull<Type>(contextType, nameof (contextType)), Check.NotNull<DbProviderInfo>(modelProviderInfo, nameof (modelProviderInfo)), AppConfig.DefaultInstance, (DbConnectionInfo) null, (Func<IDbDependencyResolver>) null)
    {
    }

    /// <summary>
    /// Creates a new instance representing a given <see cref="T:System.Data.Entity.DbContext" /> type. An external config
    /// object (e.g. app.config or web.config) can be supplied and will be used during connection string
    /// resolution. This includes looking for connection strings and DefaultConnectionFactory entries.
    /// A <see cref="T:System.Data.Entity.Infrastructure.DbProviderInfo" /> can be supplied in order to override the default determined
    /// provider used when constructing the underlying EDM model. This can be useful to prevent EF from
    /// connecting to discover a manifest token.
    /// </summary>
    /// <param name="contextType">
    /// The type deriving from <see cref="T:System.Data.Entity.DbContext" />.
    /// </param>
    /// <param name="config"> An object representing the config file. </param>
    /// <param name="modelProviderInfo">
    /// A <see cref="T:System.Data.Entity.Infrastructure.DbProviderInfo" /> specifying the underlying ADO.NET provider to target.
    /// </param>
    public DbContextInfo(Type contextType, System.Configuration.Configuration config, DbProviderInfo modelProviderInfo)
      : this(Check.NotNull<Type>(contextType, nameof (contextType)), Check.NotNull<DbProviderInfo>(modelProviderInfo, nameof (modelProviderInfo)), new AppConfig(Check.NotNull<System.Configuration.Configuration>(config, nameof (config))), (DbConnectionInfo) null, (Func<IDbDependencyResolver>) null)
    {
    }

    internal DbContextInfo(DbContext context, Func<IDbDependencyResolver> resolver = null)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      this._resolver = resolver ?? (Func<IDbDependencyResolver>) (() => DbConfiguration.DependencyResolver);
      this._contextType = context.GetType();
      this._appConfig = AppConfig.DefaultInstance;
      InternalContext internalContext = context.InternalContext;
      this._connectionProviderName = internalContext.ProviderName;
      this._connectionInfo = new DbConnectionInfo(internalContext.OriginalConnectionString, this._connectionProviderName);
      this._connectionString = internalContext.OriginalConnectionString;
      this._connectionStringName = internalContext.ConnectionStringName;
      this._connectionStringOrigin = internalContext.ConnectionStringOrigin;
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    private DbContextInfo(
      Type contextType,
      DbProviderInfo modelProviderInfo,
      AppConfig config,
      DbConnectionInfo connectionInfo,
      Func<IDbDependencyResolver> resolver = null)
    {
      if (!typeof (DbContext).IsAssignableFrom(contextType))
        throw new ArgumentOutOfRangeException(nameof (contextType));
      this._resolver = resolver ?? (Func<IDbDependencyResolver>) (() => DbConfiguration.DependencyResolver);
      this._contextType = contextType;
      this._modelProviderInfo = modelProviderInfo;
      this._appConfig = config;
      this._connectionInfo = connectionInfo;
      this._activator = this.CreateActivator();
      if (this._activator == null)
        return;
      DbContext instance = this.CreateInstance();
      if (instance == null)
        return;
      this._isConstructible = true;
      using (instance)
      {
        this._connectionString = DbInterception.Dispatch.Connection.GetConnectionString(instance.InternalContext.Connection, new DbInterceptionContext().WithDbContext(instance));
        this._connectionStringName = instance.InternalContext.ConnectionStringName;
        this._connectionProviderName = instance.InternalContext.ProviderName;
        this._connectionStringOrigin = instance.InternalContext.ConnectionStringOrigin;
      }
    }

    /// <summary>
    /// The concrete <see cref="T:System.Data.Entity.DbContext" /> type.
    /// </summary>
    public virtual Type ContextType
    {
      get
      {
        return this._contextType;
      }
    }

    /// <summary>
    /// Whether or not instances of the underlying <see cref="T:System.Data.Entity.DbContext" /> type can be created.
    /// </summary>
    public virtual bool IsConstructible
    {
      get
      {
        return this._isConstructible;
      }
    }

    /// <summary>
    /// The connection string used by the underlying <see cref="T:System.Data.Entity.DbContext" /> type.
    /// </summary>
    public virtual string ConnectionString
    {
      get
      {
        return this._connectionString;
      }
    }

    /// <summary>
    /// The connection string name used by the underlying <see cref="T:System.Data.Entity.DbContext" /> type.
    /// </summary>
    public virtual string ConnectionStringName
    {
      get
      {
        return this._connectionStringName;
      }
    }

    /// <summary>
    /// The ADO.NET provider name of the connection used by the underlying <see cref="T:System.Data.Entity.DbContext" /> type.
    /// </summary>
    public virtual string ConnectionProviderName
    {
      get
      {
        return this._connectionProviderName;
      }
    }

    /// <summary>
    /// The origin of the connection string used by the underlying <see cref="T:System.Data.Entity.DbContext" /> type.
    /// </summary>
    public virtual DbConnectionStringOrigin ConnectionStringOrigin
    {
      get
      {
        return this._connectionStringOrigin;
      }
    }

    /// <summary>
    /// An action to be run on the DbModelBuilder after OnModelCreating has been run on the context.
    /// </summary>
    public virtual Action<DbModelBuilder> OnModelCreating
    {
      get
      {
        return this._onModelCreating;
      }
      set
      {
        this._onModelCreating = value;
      }
    }

    /// <summary>
    /// If instances of the underlying <see cref="T:System.Data.Entity.DbContext" /> type can be created, returns
    /// a new instance; otherwise returns null.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.DbContext" /> instance.
    /// </returns>
    public virtual DbContext CreateInstance()
    {
      bool flag = DbConfigurationManager.Instance.PushConfiguration(this._appConfig, this._contextType);
      DbContextInfo.CurrentInfo = this;
      DbContext dbContext = (DbContext) null;
      try
      {
        try
        {
          dbContext = this._activator == null ? (DbContext) null : this._activator();
        }
        catch (TargetInvocationException ex)
        {
          ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
          throw ex.InnerException;
        }
        if (dbContext == null)
          return (DbContext) null;
        dbContext.InternalContext.OnDisposing += (EventHandler<EventArgs>) ((_, __) => DbContextInfo.CurrentInfo = (DbContextInfo) null);
        if (flag)
          dbContext.InternalContext.OnDisposing += (EventHandler<EventArgs>) ((_, __) => DbConfigurationManager.Instance.PopConfiguration(this._appConfig));
        dbContext.InternalContext.ApplyContextInfo(this);
        return dbContext;
      }
      catch (Exception ex)
      {
        dbContext?.Dispose();
        throw;
      }
      finally
      {
        if (dbContext == null)
        {
          DbContextInfo.CurrentInfo = (DbContextInfo) null;
          if (flag)
            DbConfigurationManager.Instance.PopConfiguration(this._appConfig);
        }
      }
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    internal void ConfigureContext(DbContext context)
    {
      if (this._modelProviderInfo != null)
        context.InternalContext.ModelProviderInfo = this._modelProviderInfo;
      context.InternalContext.AppConfig = this._appConfig;
      if (this._connectionInfo != null)
        context.InternalContext.OverrideConnection((IInternalConnection) new LazyInternalConnection(context, this._connectionInfo));
      else if (this._modelProviderInfo != null && this._appConfig == AppConfig.DefaultInstance)
        context.InternalContext.OverrideConnection((IInternalConnection) new EagerInternalConnection(context, this._resolver().GetService<DbProviderFactory>((object) this._modelProviderInfo.ProviderInvariantName).CreateConnection(), true));
      if (this._onModelCreating == null)
        return;
      context.InternalContext.OnModelCreating = this._onModelCreating;
    }

    private Func<DbContext> CreateActivator()
    {
      if (this._contextType.GetPublicConstructor() != (ConstructorInfo) null)
        return (Func<DbContext>) (() => (DbContext) Activator.CreateInstance(this._contextType));
      Func<DbContext> service = this._resolver().GetService<Func<DbContext>>((object) this._contextType);
      if (service != null)
        return service;
      Type type = this._contextType.Assembly().GetAccessibleTypes().Where<Type>((Func<Type, bool>) (t =>
      {
        if (!t.IsClass())
          return false;
        return typeof (IDbContextFactory<>).MakeGenericType(this._contextType).IsAssignableFrom(t);
      })).FirstOrDefault<Type>();
      if (type == (Type) null)
        return (Func<DbContext>) null;
      if (type.GetPublicConstructor() == (ConstructorInfo) null)
        throw Error.DbContextServices_MissingDefaultCtor((object) type);
      return new Func<DbContext>(((IDbContextFactory<DbContext>) Activator.CreateInstance(type)).Create);
    }

    internal static DbContextInfo CurrentInfo
    {
      get
      {
        return DbContextInfo._currentInfo;
      }
      set
      {
        DbContextInfo._currentInfo = value;
      }
    }
  }
}
