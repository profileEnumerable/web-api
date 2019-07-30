// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.AppConfig
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal.ConfigFile;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Internal
{
  internal class AppConfig
  {
    private static readonly AppConfig _defaultInstance = new AppConfig();
    private readonly Lazy<IDbConnectionFactory> _defaultDefaultConnectionFactory = new Lazy<IDbConnectionFactory>((Func<IDbConnectionFactory>) (() => (IDbConnectionFactory) null), true);
    public const string EFSectionName = "entityFramework";
    private readonly KeyValueConfigurationCollection _appSettings;
    private readonly ConnectionStringSettingsCollection _connectionStrings;
    private readonly EntityFrameworkSection _entityFrameworkSettings;
    private readonly Lazy<IDbConnectionFactory> _defaultConnectionFactory;
    private readonly ProviderServicesFactory _providerServicesFactory;
    private readonly Lazy<IList<NamedDbProviderService>> _providerServices;

    public AppConfig(System.Configuration.Configuration configuration)
      : this(configuration.ConnectionStrings.ConnectionStrings, configuration.AppSettings.Settings, (EntityFrameworkSection) configuration.GetSection("entityFramework"), (ProviderServicesFactory) null)
    {
    }

    public AppConfig(
      ConnectionStringSettingsCollection connectionStrings)
      : this(connectionStrings, (KeyValueConfigurationCollection) null, (EntityFrameworkSection) null, (ProviderServicesFactory) null)
    {
    }

    private AppConfig()
      : this(ConfigurationManager.ConnectionStrings, AppConfig.Convert(ConfigurationManager.AppSettings), (EntityFrameworkSection) ConfigurationManager.GetSection("entityFramework"), (ProviderServicesFactory) null)
    {
    }

    internal AppConfig(
      ConnectionStringSettingsCollection connectionStrings,
      KeyValueConfigurationCollection appSettings,
      EntityFrameworkSection entityFrameworkSettings,
      ProviderServicesFactory providerServicesFactory = null)
    {
      this._connectionStrings = connectionStrings;
      this._appSettings = appSettings ?? new KeyValueConfigurationCollection();
      this._entityFrameworkSettings = entityFrameworkSettings ?? new EntityFrameworkSection();
      this._providerServicesFactory = providerServicesFactory ?? new ProviderServicesFactory();
      this._providerServices = new Lazy<IList<NamedDbProviderService>>((Func<IList<NamedDbProviderService>>) (() => (IList<NamedDbProviderService>) this._entityFrameworkSettings.Providers.OfType<ProviderElement>().Select<ProviderElement, NamedDbProviderService>((Func<ProviderElement, NamedDbProviderService>) (e => new NamedDbProviderService(e.InvariantName, this._providerServicesFactory.GetInstance(e.ProviderTypeName, e.InvariantName)))).ToList<NamedDbProviderService>()));
      if (this._entityFrameworkSettings.DefaultConnectionFactory.ElementInformation.IsPresent)
        this._defaultConnectionFactory = new Lazy<IDbConnectionFactory>((Func<IDbConnectionFactory>) (() =>
        {
          DefaultConnectionFactoryElement connectionFactory = this._entityFrameworkSettings.DefaultConnectionFactory;
          try
          {
            return (IDbConnectionFactory) Activator.CreateInstance(connectionFactory.GetFactoryType(), connectionFactory.Parameters.GetTypedParameterValues());
          }
          catch (Exception ex)
          {
            throw new InvalidOperationException(Strings.SetConnectionFactoryFromConfigFailed((object) connectionFactory.FactoryTypeName), ex);
          }
        }), true);
      else
        this._defaultConnectionFactory = this._defaultDefaultConnectionFactory;
    }

    public virtual IDbConnectionFactory TryGetDefaultConnectionFactory()
    {
      return this._defaultConnectionFactory.Value;
    }

    public ConnectionStringSettings GetConnectionString(string name)
    {
      return this._connectionStrings[name];
    }

    public static AppConfig DefaultInstance
    {
      get
      {
        return AppConfig._defaultInstance;
      }
    }

    private static KeyValueConfigurationCollection Convert(
      NameValueCollection collection)
    {
      KeyValueConfigurationCollection configurationCollection = new KeyValueConfigurationCollection();
      foreach (string allKey in collection.AllKeys)
        configurationCollection.Add(allKey, ConfigurationManager.AppSettings[allKey]);
      return configurationCollection;
    }

    public virtual InitializerConfig Initializers
    {
      get
      {
        return new InitializerConfig(this._entityFrameworkSettings, this._appSettings);
      }
    }

    public virtual string ConfigurationTypeName
    {
      get
      {
        return this._entityFrameworkSettings.ConfigurationTypeName;
      }
    }

    public virtual IList<NamedDbProviderService> DbProviderServices
    {
      get
      {
        return this._providerServices.Value;
      }
    }

    public virtual IEnumerable<IDbInterceptor> Interceptors
    {
      get
      {
        return this._entityFrameworkSettings.Interceptors.Interceptors;
      }
    }

    public virtual QueryCacheConfig QueryCache
    {
      get
      {
        return new QueryCacheConfig(this._entityFrameworkSettings);
      }
    }
  }
}
