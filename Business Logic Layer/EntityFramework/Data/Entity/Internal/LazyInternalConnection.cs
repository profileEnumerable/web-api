// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.LazyInternalConnection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Internal
{
  internal class LazyInternalConnection : InternalConnection
  {
    private readonly string _nameOrConnectionString;
    private DbConnectionStringOrigin _connectionStringOrigin;
    private string _connectionStringName;
    private readonly DbConnectionInfo _connectionInfo;
    private bool? _hasModel;

    public LazyInternalConnection(string nameOrConnectionString)
      : this((DbContext) null, nameOrConnectionString)
    {
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public LazyInternalConnection(DbContext context, string nameOrConnectionString)
      : base(context == null ? (DbInterceptionContext) null : new DbInterceptionContext().WithDbContext(context))
    {
      this._nameOrConnectionString = nameOrConnectionString;
      this.AppConfig = AppConfig.DefaultInstance;
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public LazyInternalConnection(DbContext context, DbConnectionInfo connectionInfo)
      : base(new DbInterceptionContext().WithDbContext(context))
    {
      this._connectionInfo = connectionInfo;
      this.AppConfig = AppConfig.DefaultInstance;
    }

    public override DbConnection Connection
    {
      get
      {
        this.Initialize();
        return base.Connection;
      }
    }

    public override DbConnectionStringOrigin ConnectionStringOrigin
    {
      get
      {
        this.Initialize();
        return this._connectionStringOrigin;
      }
    }

    public override string ConnectionStringName
    {
      get
      {
        this.Initialize();
        return this._connectionStringName;
      }
    }

    public override string ConnectionKey
    {
      get
      {
        this.Initialize();
        return base.ConnectionKey;
      }
    }

    public override string OriginalConnectionString
    {
      get
      {
        this.Initialize();
        return base.OriginalConnectionString;
      }
    }

    public override string ProviderName
    {
      get
      {
        this.Initialize();
        return base.ProviderName;
      }
      set
      {
        base.ProviderName = value;
      }
    }

    public override bool ConnectionHasModel
    {
      get
      {
        if (!this._hasModel.HasValue)
        {
          if (this.UnderlyingConnection == null)
          {
            string connectionString = this._nameOrConnectionString;
            if (this._connectionInfo != null)
            {
              connectionString = this._connectionInfo.GetConnectionString(this.AppConfig).ConnectionString;
            }
            else
            {
              string name;
              if (DbHelpers.TryGetConnectionName(this._nameOrConnectionString, out name))
              {
                ConnectionStringSettings connectionInConfig = LazyInternalConnection.FindConnectionInConfig(name, this.AppConfig);
                if (connectionInConfig == null && DbHelpers.TreatAsConnectionString(this._nameOrConnectionString))
                  throw Error.DbContext_ConnectionStringNotFound((object) name);
                if (connectionInConfig != null)
                  connectionString = connectionInConfig.ConnectionString;
              }
            }
            this._hasModel = new bool?(DbHelpers.IsFullEFConnectionString(connectionString));
          }
          else
            this._hasModel = new bool?(this.UnderlyingConnection is EntityConnection);
        }
        return this._hasModel.Value;
      }
    }

    public override ObjectContext CreateObjectContextFromConnectionModel()
    {
      this.Initialize();
      return base.CreateObjectContextFromConnectionModel();
    }

    public override void Dispose()
    {
      if (this.UnderlyingConnection == null)
        return;
      if (this.UnderlyingConnection is EntityConnection)
        this.UnderlyingConnection.Dispose();
      else
        DbInterception.Dispatch.Connection.Dispose(this.UnderlyingConnection, this.InterceptionContext);
      this.UnderlyingConnection = (DbConnection) null;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal bool IsInitialized
    {
      get
      {
        return this.UnderlyingConnection != null;
      }
    }

    private void Initialize()
    {
      if (this.UnderlyingConnection != null)
        return;
      if (this._connectionInfo != null)
      {
        ConnectionStringSettings connectionString = this._connectionInfo.GetConnectionString(this.AppConfig);
        this.InitializeFromConnectionStringSetting(connectionString);
        this._connectionStringOrigin = DbConnectionStringOrigin.DbContextInfo;
        this._connectionStringName = connectionString.Name;
      }
      else
      {
        string name;
        if (!DbHelpers.TryGetConnectionName(this._nameOrConnectionString, out name) || !this.TryInitializeFromAppConfig(name, this.AppConfig))
        {
          if (name != null && DbHelpers.TreatAsConnectionString(this._nameOrConnectionString))
            throw Error.DbContext_ConnectionStringNotFound((object) name);
          if (DbHelpers.IsFullEFConnectionString(this._nameOrConnectionString))
            this.UnderlyingConnection = (DbConnection) new EntityConnection(this._nameOrConnectionString);
          else if (base.ProviderName != null)
          {
            this.CreateConnectionFromProviderName(base.ProviderName);
          }
          else
          {
            this.UnderlyingConnection = DbConfiguration.DependencyResolver.GetService<IDbConnectionFactory>().CreateConnection(name ?? this._nameOrConnectionString);
            if (this.UnderlyingConnection == null)
              throw Error.DbContext_ConnectionFactoryReturnedNullConnection();
          }
          if (name != null)
          {
            this._connectionStringOrigin = DbConnectionStringOrigin.Convention;
            this._connectionStringName = name;
          }
          else
            this._connectionStringOrigin = DbConnectionStringOrigin.UserCode;
        }
      }
      this.OnConnectionInitialized();
    }

    private bool TryInitializeFromAppConfig(string name, AppConfig config)
    {
      ConnectionStringSettings connectionInConfig = LazyInternalConnection.FindConnectionInConfig(name, config);
      if (connectionInConfig == null)
        return false;
      this.InitializeFromConnectionStringSetting(connectionInConfig);
      this._connectionStringOrigin = DbConnectionStringOrigin.Configuration;
      this._connectionStringName = connectionInConfig.Name;
      return true;
    }

    private static ConnectionStringSettings FindConnectionInConfig(
      string name,
      AppConfig config)
    {
      List<string> source = new List<string>() { name };
      int num = name.LastIndexOf('.');
      if (num >= 0 && num + 1 < name.Length)
        source.Add(name.Substring(num + 1));
      return source.Where<string>((Func<string, bool>) (c => config.GetConnectionString(c) != null)).Select<string, ConnectionStringSettings>((Func<string, ConnectionStringSettings>) (c => config.GetConnectionString(c))).FirstOrDefault<ConnectionStringSettings>();
    }

    private void InitializeFromConnectionStringSetting(ConnectionStringSettings appConfigConnection)
    {
      string providerName = appConfigConnection.ProviderName;
      if (string.IsNullOrWhiteSpace(providerName))
        throw Error.DbContext_ProviderNameMissing((object) appConfigConnection.Name);
      if (string.Equals(providerName, "System.Data.EntityClient", StringComparison.OrdinalIgnoreCase))
      {
        this.UnderlyingConnection = (DbConnection) new EntityConnection(appConfigConnection.ConnectionString);
      }
      else
      {
        this.CreateConnectionFromProviderName(providerName);
        DbInterception.Dispatch.Connection.SetConnectionString(this.UnderlyingConnection, new DbConnectionPropertyInterceptionContext<string>().WithValue(appConfigConnection.ConnectionString));
      }
    }

    private void CreateConnectionFromProviderName(string providerInvariantName)
    {
      this.UnderlyingConnection = DbConfiguration.DependencyResolver.GetService<DbProviderFactory>((object) providerInvariantName).CreateConnection();
      if (this.UnderlyingConnection == null)
        throw Error.DbContext_ProviderReturnedNullConnection();
    }
  }
}
