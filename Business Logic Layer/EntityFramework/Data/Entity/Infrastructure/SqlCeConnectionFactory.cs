// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.SqlCeConnectionFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.IO;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Instances of this class are used to create DbConnection objects for
  /// SQL Server Compact Edition based on a given database name or connection string.
  /// </summary>
  /// <remarks>
  /// It is necessary to provide the provider invariant name of the SQL Server Compact
  /// Edition to use when creating an instance of this class.  This is because different
  /// versions of SQL Server Compact Editions use different invariant names.
  /// An instance of this class can be set on the <see cref="T:System.Data.Entity.Database" /> class to
  /// cause all DbContexts created with no connection information or just a database
  /// name or connection string to use SQL Server Compact Edition by default.
  /// This class is immutable since multiple threads may access instances simultaneously
  /// when creating connections.
  /// </remarks>
  public sealed class SqlCeConnectionFactory : IDbConnectionFactory
  {
    private readonly string _databaseDirectory;
    private readonly string _baseConnectionString;
    private readonly string _providerInvariantName;

    /// <summary>
    /// Creates a new connection factory with empty (default) DatabaseDirectory and BaseConnectionString
    /// properties.
    /// </summary>
    /// <param name="providerInvariantName"> The provider invariant name that specifies the version of SQL Server Compact Edition that should be used. </param>
    public SqlCeConnectionFactory(string providerInvariantName)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      this._providerInvariantName = providerInvariantName;
      this._databaseDirectory = "|DataDirectory|";
      this._baseConnectionString = "";
    }

    /// <summary>
    /// Creates a new connection factory with the given DatabaseDirectory and BaseConnectionString properties.
    /// </summary>
    /// <param name="providerInvariantName"> The provider invariant name that specifies the version of SQL Server Compact Edition that should be used. </param>
    /// <param name="databaseDirectory"> The path to prepend to the database name that will form the file name used by SQL Server Compact Edition when it creates or reads the database file. An empty string means that SQL Server Compact Edition will use its default for the database file location. </param>
    /// <param name="baseConnectionString"> The connection string to use for options to the database other than the 'Data Source'. The Data Source will be prepended to this string based on the database name when CreateConnection is called. </param>
    public SqlCeConnectionFactory(
      string providerInvariantName,
      string databaseDirectory,
      string baseConnectionString)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<string>(databaseDirectory, nameof (databaseDirectory));
      Check.NotNull<string>(baseConnectionString, nameof (baseConnectionString));
      this._providerInvariantName = providerInvariantName;
      this._databaseDirectory = databaseDirectory;
      this._baseConnectionString = baseConnectionString;
    }

    /// <summary>
    /// The path to prepend to the database name that will form the file name used by
    /// SQL Server Compact Edition when it creates or reads the database file.
    /// The default value is "|DataDirectory|", which means the file will be placed
    /// in the designated data directory.
    /// </summary>
    public string DatabaseDirectory
    {
      get
      {
        return this._databaseDirectory;
      }
    }

    /// <summary>
    /// The connection string to use for options to the database other than the 'Data Source'.
    /// The Data Source will be prepended to this string based on the database name when
    /// CreateConnection is called.
    /// The default is the empty string, which means no other options will be used.
    /// </summary>
    public string BaseConnectionString
    {
      get
      {
        return this._baseConnectionString;
      }
    }

    /// <summary>
    /// The provider invariant name that specifies the version of SQL Server Compact Edition
    /// that should be used.
    /// </summary>
    public string ProviderInvariantName
    {
      get
      {
        return this._providerInvariantName;
      }
    }

    /// <summary>
    /// Creates a connection for SQL Server Compact Edition based on the given database name or connection string.
    /// If the given string contains an '=' character then it is treated as a full connection string,
    /// otherwise it is treated as a database name only.
    /// </summary>
    /// <param name="nameOrConnectionString"> The database name or connection string. </param>
    /// <returns> An initialized DbConnection. </returns>
    public DbConnection CreateConnection(string nameOrConnectionString)
    {
      Check.NotEmpty(nameOrConnectionString, nameof (nameOrConnectionString));
      DbConnection connection = DbConfiguration.DependencyResolver.GetService<DbProviderFactory>((object) this.ProviderInvariantName).CreateConnection();
      if (connection == null)
        throw Error.DbContext_ProviderReturnedNullConnection();
      string str;
      if (DbHelpers.TreatAsConnectionString(nameOrConnectionString))
      {
        str = nameOrConnectionString;
      }
      else
      {
        if (!nameOrConnectionString.EndsWith(".sdf", true, (CultureInfo) null))
          nameOrConnectionString += ".sdf";
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Data Source={0}; {1}", (object) (!this.DatabaseDirectory.StartsWith("|", StringComparison.Ordinal) || !this.DatabaseDirectory.EndsWith("|", StringComparison.Ordinal) ? Path.Combine(this.DatabaseDirectory, nameOrConnectionString) : this.DatabaseDirectory + nameOrConnectionString), (object) this.BaseConnectionString);
      }
      DbInterception.Dispatch.Connection.SetConnectionString(connection, new DbConnectionPropertyInterceptionContext<string>().WithValue(str));
      return connection;
    }
  }
}
