// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalConnection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Internal
{
  internal abstract class InternalConnection : IInternalConnection, IDisposable
  {
    private string _key;
    private string _providerName;
    private string _originalConnectionString;
    private string _originalDatabaseName;
    private string _originalDataSource;

    public InternalConnection(DbInterceptionContext interceptionContext)
    {
      this.InterceptionContext = interceptionContext ?? new DbInterceptionContext();
    }

    protected DbInterceptionContext InterceptionContext { get; private set; }

    public virtual DbConnection Connection
    {
      get
      {
        EntityConnection underlyingConnection = this.UnderlyingConnection as EntityConnection;
        if (underlyingConnection == null)
          return this.UnderlyingConnection;
        return underlyingConnection.StoreConnection;
      }
    }

    public virtual string ConnectionKey
    {
      get
      {
        string key = this._key;
        if (key != null)
          return key;
        return this._key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0};{1}", (object) this.UnderlyingConnection.GetType(), (object) this.OriginalConnectionString);
      }
    }

    public virtual bool ConnectionHasModel
    {
      get
      {
        return this.UnderlyingConnection is EntityConnection;
      }
    }

    public abstract DbConnectionStringOrigin ConnectionStringOrigin { get; }

    public virtual AppConfig AppConfig { get; set; }

    public virtual string ProviderName
    {
      get
      {
        return this._providerName ?? (this._providerName = this.UnderlyingConnection == null ? (string) null : this.Connection.GetProviderInvariantName());
      }
      set
      {
        this._providerName = value;
      }
    }

    public virtual string ConnectionStringName
    {
      get
      {
        return (string) null;
      }
    }

    public virtual string OriginalConnectionString
    {
      get
      {
        string b1 = this.UnderlyingConnection is EntityConnection ? this.UnderlyingConnection.Database : DbInterception.Dispatch.Connection.GetDatabase(this.UnderlyingConnection, this.InterceptionContext);
        string b2 = this.UnderlyingConnection is EntityConnection ? this.UnderlyingConnection.DataSource : DbInterception.Dispatch.Connection.GetDataSource(this.UnderlyingConnection, this.InterceptionContext);
        if (!string.Equals(this._originalDatabaseName, b1, StringComparison.OrdinalIgnoreCase) || !string.Equals(this._originalDataSource, b2, StringComparison.OrdinalIgnoreCase))
          this.OnConnectionInitialized();
        return this._originalConnectionString;
      }
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public virtual ObjectContext CreateObjectContextFromConnectionModel()
    {
      ObjectContext objectContext = new ObjectContext((EntityConnection) this.UnderlyingConnection);
      ReadOnlyCollection<EntityContainer> items = objectContext.MetadataWorkspace.GetItems<EntityContainer>(DataSpace.CSpace);
      if (items.Count == 1)
        objectContext.DefaultContainerName = items.Single<EntityContainer>().Name;
      return objectContext;
    }

    public abstract void Dispose();

    protected DbConnection UnderlyingConnection { get; set; }

    protected void OnConnectionInitialized()
    {
      this._originalConnectionString = InternalConnection.GetStoreConnectionString(this.UnderlyingConnection);
      try
      {
        this._originalDatabaseName = this.UnderlyingConnection is EntityConnection ? this.UnderlyingConnection.Database : DbInterception.Dispatch.Connection.GetDatabase(this.UnderlyingConnection, this.InterceptionContext);
      }
      catch (NotImplementedException ex)
      {
      }
      try
      {
        this._originalDataSource = this.UnderlyingConnection is EntityConnection ? this.UnderlyingConnection.DataSource : DbInterception.Dispatch.Connection.GetDataSource(this.UnderlyingConnection, this.InterceptionContext);
      }
      catch (NotImplementedException ex)
      {
      }
    }

    public static string GetStoreConnectionString(DbConnection connection)
    {
      EntityConnection entityConnection = connection as EntityConnection;
      string str;
      if (entityConnection != null)
      {
        connection = entityConnection.StoreConnection;
        str = connection != null ? DbInterception.Dispatch.Connection.GetConnectionString(connection, new DbInterceptionContext()) : (string) null;
      }
      else
        str = DbInterception.Dispatch.Connection.GetConnectionString(connection, new DbInterceptionContext());
      return str;
    }
  }
}
