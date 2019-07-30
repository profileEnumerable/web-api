// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.AppConfigDependencyResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class AppConfigDependencyResolver : IDbDependencyResolver
  {
    private readonly ConcurrentDictionary<Tuple<Type, object>, Func<object>> _serviceFactories = new ConcurrentDictionary<Tuple<Type, object>, Func<object>>();
    private readonly ConcurrentDictionary<Tuple<Type, object>, IEnumerable<Func<object>>> _servicesFactories = new ConcurrentDictionary<Tuple<Type, object>, IEnumerable<Func<object>>>();
    private readonly Dictionary<string, DbProviderServices> _providerFactories = new Dictionary<string, DbProviderServices>();
    private readonly AppConfig _appConfig;
    private readonly InternalConfiguration _internalConfiguration;
    private bool _providersRegistered;
    private readonly ProviderServicesFactory _providerServicesFactory;

    public AppConfigDependencyResolver()
    {
    }

    public AppConfigDependencyResolver(
      AppConfig appConfig,
      InternalConfiguration internalConfiguration,
      ProviderServicesFactory providerServicesFactory = null)
    {
      this._appConfig = appConfig;
      this._internalConfiguration = internalConfiguration;
      this._providerServicesFactory = providerServicesFactory ?? new ProviderServicesFactory();
    }

    public virtual object GetService(Type type, object key)
    {
      return this._serviceFactories.GetOrAdd(Tuple.Create<Type, object>(type, key), (Func<Tuple<Type, object>, Func<object>>) (t => this.GetServiceFactory(type, key as string)))();
    }

    public IEnumerable<object> GetServices(Type type, object key)
    {
      return (IEnumerable<object>) this._servicesFactories.GetOrAdd(Tuple.Create<Type, object>(type, key), (Func<Tuple<Type, object>, IEnumerable<Func<object>>>) (t => this.GetServicesFactory(type, key))).Select<Func<object>, object>((Func<Func<object>, object>) (f => f())).Where<object>((Func<object, bool>) (s => s != null)).ToList<object>();
    }

    public virtual IEnumerable<Func<object>> GetServicesFactory(
      Type type,
      object key)
    {
      if (type == typeof (IDbInterceptor))
        return (IEnumerable<Func<object>>) this._appConfig.Interceptors.Select<IDbInterceptor, Func<object>>((Func<IDbInterceptor, Func<object>>) (i => (Func<object>) (() => (object) i))).ToList<Func<object>>();
      return (IEnumerable<Func<object>>) new List<Func<object>>()
      {
        this.GetServiceFactory(type, key as string)
      };
    }

    public virtual Func<object> GetServiceFactory(Type type, string name)
    {
      if (!this._providersRegistered)
      {
        lock (this._providerFactories)
        {
          if (!this._providersRegistered)
          {
            this.RegisterDbProviderServices();
            this._providersRegistered = true;
          }
        }
      }
      if (!string.IsNullOrWhiteSpace(name) && type == typeof (DbProviderServices))
      {
        DbProviderServices providerFactory;
        this._providerFactories.TryGetValue(name, out providerFactory);
        return (Func<object>) (() => (object) providerFactory);
      }
      if (type == typeof (IDbConnectionFactory))
      {
        if (!Database.DefaultConnectionFactoryChanged)
        {
          IDbConnectionFactory connectionFactory = this._appConfig.TryGetDefaultConnectionFactory();
          if (connectionFactory != null)
            Database.DefaultConnectionFactory = connectionFactory;
        }
        return (Func<object>) (() =>
        {
          if (!Database.DefaultConnectionFactoryChanged)
            return (object) null;
          return (object) Database.SetDefaultConnectionFactory;
        });
      }
      Type elementType = type.TryGetElementType(typeof (IDatabaseInitializer<>));
      if (!(elementType != (Type) null))
        return (Func<object>) (() => (object) null);
      object initializer = this._appConfig.Initializers.TryGetInitializer(elementType);
      return (Func<object>) (() => initializer);
    }

    private void RegisterDbProviderServices()
    {
      IList<NamedDbProviderService> providerServices = this._appConfig.DbProviderServices;
      if (providerServices.All<NamedDbProviderService>((Func<NamedDbProviderService, bool>) (p => p.InvariantName != "System.Data.SqlClient")))
        this.RegisterSqlServerProvider();
      providerServices.Each<NamedDbProviderService>((Action<NamedDbProviderService>) (p =>
      {
        this._providerFactories[p.InvariantName] = p.ProviderServices;
        this._internalConfiguration.AddDefaultResolver((IDbDependencyResolver) p.ProviderServices);
      }));
    }

    private void RegisterSqlServerProvider()
    {
      DbProviderServices instance = this._providerServicesFactory.TryGetInstance(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer, Version={0}, Culture=neutral, PublicKeyToken=b77a5c561934e089", (object) new AssemblyName(typeof (DbContext).Assembly().FullName).Version));
      if (instance == null)
        return;
      this._internalConfiguration.SetDefaultProviderServices(instance, "System.Data.SqlClient");
    }
  }
}
