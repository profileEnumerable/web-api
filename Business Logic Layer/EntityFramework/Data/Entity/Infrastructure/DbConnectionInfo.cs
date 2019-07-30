// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbConnectionInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Configuration;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>Represents information about a database connection.</summary>
  [Serializable]
  public class DbConnectionInfo
  {
    private readonly string _connectionName;
    private readonly string _connectionString;
    private readonly string _providerInvariantName;

    /// <summary>
    /// Creates a new instance of DbConnectionInfo representing a connection that is specified in the application configuration file.
    /// </summary>
    /// <param name="connectionName"> The name of the connection string in the application configuration. </param>
    public DbConnectionInfo(string connectionName)
    {
      Check.NotEmpty(connectionName, nameof (connectionName));
      this._connectionName = connectionName;
    }

    /// <summary>
    /// Creates a new instance of DbConnectionInfo based on a connection string.
    /// </summary>
    /// <param name="connectionString"> The connection string to use for the connection. </param>
    /// <param name="providerInvariantName"> The name of the provider to use for the connection. Use 'System.Data.SqlClient' for SQL Server. </param>
    public DbConnectionInfo(string connectionString, string providerInvariantName)
    {
      Check.NotEmpty(connectionString, nameof (connectionString));
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      this._connectionString = connectionString;
      this._providerInvariantName = providerInvariantName;
    }

    internal ConnectionStringSettings GetConnectionString(AppConfig config)
    {
      if (this._connectionName == null)
        return new ConnectionStringSettings((string) null, this._connectionString, this._providerInvariantName);
      ConnectionStringSettings connectionString = config.GetConnectionString(this._connectionName);
      if (connectionString == null)
        throw Error.DbConnectionInfo_ConnectionStringNotFound((object) this._connectionName);
      return connectionString;
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
