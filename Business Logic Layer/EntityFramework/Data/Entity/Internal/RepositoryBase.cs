// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.RepositoryBase
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace System.Data.Entity.Internal
{
  internal abstract class RepositoryBase
  {
    private readonly DbConnection _existingConnection;
    private readonly string _connectionString;
    private readonly DbProviderFactory _providerFactory;

    protected RepositoryBase(
      InternalContext usersContext,
      string connectionString,
      DbProviderFactory providerFactory)
    {
      DbConnection connection = usersContext.Connection;
      if (connection != null && connection.State == ConnectionState.Open)
        this._existingConnection = connection;
      this._connectionString = connectionString;
      this._providerFactory = providerFactory;
    }

    protected DbConnection CreateConnection()
    {
      if (this._existingConnection != null)
        return this._existingConnection;
      DbConnection connection = this._providerFactory.CreateConnection();
      DbInterception.Dispatch.Connection.SetConnectionString(connection, new DbConnectionPropertyInterceptionContext<string>().WithValue(this._connectionString));
      return connection;
    }

    protected void DisposeConnection(DbConnection connection)
    {
      if (connection == null || this._existingConnection != null)
        return;
      DbInterception.Dispatch.Connection.Dispose(connection, new DbInterceptionContext());
    }
  }
}
