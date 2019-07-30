// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.DbConfigurationManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class DbConfigurationManager
  {
    private static readonly DbConfigurationManager _configManager = new DbConfigurationManager(new DbConfigurationLoader(), new DbConfigurationFinder());
    private volatile Type _newConfigurationType = typeof (DbConfiguration);
    private readonly object _lock = new object();
    private readonly ConcurrentDictionary<Assembly, object> _knownAssemblies = new ConcurrentDictionary<Assembly, object>();
    private readonly Lazy<List<Tuple<AppConfig, InternalConfiguration>>> _configurationOverrides = new Lazy<List<Tuple<AppConfig, InternalConfiguration>>>((Func<List<Tuple<AppConfig, InternalConfiguration>>>) (() => new List<Tuple<AppConfig, InternalConfiguration>>()));
    private EventHandler<DbConfigurationLoadedEventArgs> _loadedHandler;
    private readonly DbConfigurationLoader _loader;
    private readonly DbConfigurationFinder _finder;
    private readonly Lazy<InternalConfiguration> _configuration;
    private volatile DbConfiguration _newConfiguration;

    public DbConfigurationManager(DbConfigurationLoader loader, DbConfigurationFinder finder)
    {
      this._loader = loader;
      this._finder = finder;
      this._configuration = new Lazy<InternalConfiguration>((Func<InternalConfiguration>) (() =>
      {
        DbConfiguration dbConfiguration = this._newConfiguration ?? this._newConfigurationType.CreateInstance<DbConfiguration>(new Func<string, string, string>(Strings.CreateInstance_BadDbConfigurationType), (Func<string, Exception>) null);
        dbConfiguration.InternalConfiguration.Lock();
        return dbConfiguration.InternalConfiguration;
      }));
    }

    public static DbConfigurationManager Instance
    {
      get
      {
        return DbConfigurationManager._configManager;
      }
    }

    public virtual void AddLoadedHandler(
      EventHandler<DbConfigurationLoadedEventArgs> handler)
    {
      if (this.ConfigurationSet)
        throw new InvalidOperationException(Strings.AddHandlerToInUseConfiguration);
      this._loadedHandler += handler;
    }

    public virtual void RemoveLoadedHandler(
      EventHandler<DbConfigurationLoadedEventArgs> handler)
    {
      this._loadedHandler -= handler;
    }

    public virtual void OnLoaded(InternalConfiguration configuration)
    {
      DbConfigurationLoadedEventArgs configurationLoadedEventArgs = new DbConfigurationLoadedEventArgs(configuration);
      EventHandler<DbConfigurationLoadedEventArgs> loadedHandler = this._loadedHandler;
      if (loadedHandler != null)
        loadedHandler((object) configuration.Owner, configurationLoadedEventArgs);
      configuration.DispatchLoadedInterceptors(configurationLoadedEventArgs);
    }

    public virtual InternalConfiguration GetConfiguration()
    {
      if (this._configurationOverrides.IsValueCreated)
      {
        lock (this._lock)
        {
          if (this._configurationOverrides.Value.Count != 0)
            return this._configurationOverrides.Value.Last<Tuple<AppConfig, InternalConfiguration>>().Item2;
        }
      }
      return this._configuration.Value;
    }

    public virtual void SetConfigurationType(Type configurationType)
    {
      this._newConfigurationType = configurationType;
    }

    public virtual void SetConfiguration(InternalConfiguration configuration)
    {
      Type type = this._loader.TryLoadFromConfig(AppConfig.DefaultInstance);
      if (type != (Type) null)
        configuration = type.CreateInstance<DbConfiguration>(new Func<string, string, string>(Strings.CreateInstance_BadDbConfigurationType), (Func<string, Exception>) null).InternalConfiguration;
      this._newConfiguration = configuration.Owner;
      if (!(this._configuration.Value.Owner.GetType() != configuration.Owner.GetType()))
        return;
      if (this._configuration.Value.Owner.GetType() == typeof (DbConfiguration))
        throw new InvalidOperationException(Strings.DefaultConfigurationUsedBeforeSet((object) configuration.Owner.GetType().Name));
      throw new InvalidOperationException(Strings.ConfigurationSetTwice((object) configuration.Owner.GetType().Name, (object) this._configuration.Value.Owner.GetType().Name));
    }

    public virtual void EnsureLoadedForContext(Type contextType)
    {
      this.EnsureLoadedForAssembly(contextType.Assembly(), contextType);
    }

    public virtual void EnsureLoadedForAssembly(Assembly assemblyHint, Type contextTypeHint)
    {
      if (contextTypeHint == typeof (DbContext) || this._knownAssemblies.ContainsKey(assemblyHint))
        return;
      if (this._configurationOverrides.IsValueCreated)
      {
        lock (this._lock)
        {
          if (this._configurationOverrides.Value.Count != 0)
            return;
        }
      }
      if (!this.ConfigurationSet)
      {
        Type type = this._loader.TryLoadFromConfig(AppConfig.DefaultInstance);
        if ((object) type == null)
          type = this._finder.TryFindConfigurationType(assemblyHint, this._finder.TryFindContextType(assemblyHint, contextTypeHint, (IEnumerable<Type>) null), (IEnumerable<Type>) null);
        Type configurationType = type;
        if (configurationType != (Type) null)
          this.SetConfigurationType(configurationType);
      }
      else if (!assemblyHint.IsDynamic && !this._loader.AppConfigContainsDbConfigurationType(AppConfig.DefaultInstance))
      {
        contextTypeHint = this._finder.TryFindContextType(assemblyHint, contextTypeHint, (IEnumerable<Type>) null);
        Type configurationType = this._finder.TryFindConfigurationType(assemblyHint, contextTypeHint, (IEnumerable<Type>) null);
        if (configurationType != (Type) null)
        {
          if (this._configuration.Value.Owner.GetType() == typeof (DbConfiguration))
            throw new InvalidOperationException(Strings.ConfigurationNotDiscovered((object) configurationType.Name));
          if (contextTypeHint != (Type) null && configurationType != this._configuration.Value.Owner.GetType())
            throw new InvalidOperationException(Strings.SetConfigurationNotDiscovered((object) this._configuration.Value.Owner.GetType().Name, (object) contextTypeHint.Name));
        }
      }
      this._knownAssemblies.TryAdd(assemblyHint, (object) null);
    }

    private bool ConfigurationSet
    {
      get
      {
        return this._configuration.IsValueCreated;
      }
    }

    public virtual bool PushConfiguration(AppConfig config, Type contextType)
    {
      if (config == AppConfig.DefaultInstance && (contextType == typeof (DbContext) || this._knownAssemblies.ContainsKey(contextType.Assembly())))
        return false;
      Type type = this._loader.TryLoadFromConfig(config);
      if ((object) type == null)
      {
        Type configurationType = this._finder.TryFindConfigurationType(contextType, (IEnumerable<Type>) null);
        type = (object) configurationType != null ? configurationType : typeof (DbConfiguration);
      }
      InternalConfiguration internalConfiguration = type.CreateInstance<DbConfiguration>(new Func<string, string, string>(Strings.CreateInstance_BadDbConfigurationType), (Func<string, Exception>) null).InternalConfiguration;
      internalConfiguration.SwitchInRootResolver(this._configuration.Value.RootResolver);
      internalConfiguration.AddAppConfigResolver((IDbDependencyResolver) new AppConfigDependencyResolver(config, internalConfiguration, (ProviderServicesFactory) null));
      lock (this._lock)
        this._configurationOverrides.Value.Add(Tuple.Create<AppConfig, InternalConfiguration>(config, internalConfiguration));
      internalConfiguration.Lock();
      return true;
    }

    public virtual void PopConfiguration(AppConfig config)
    {
      lock (this._lock)
      {
        Tuple<AppConfig, InternalConfiguration> tuple = this._configurationOverrides.Value.FirstOrDefault<Tuple<AppConfig, InternalConfiguration>>((Func<Tuple<AppConfig, InternalConfiguration>, bool>) (c => c.Item1 == config));
        if (tuple == null)
          return;
        this._configurationOverrides.Value.Remove(tuple);
      }
    }
  }
}
