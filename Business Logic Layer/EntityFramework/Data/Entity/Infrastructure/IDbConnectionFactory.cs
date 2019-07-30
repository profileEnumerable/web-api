// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IDbConnectionFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Implementations of this interface are used to create DbConnection objects for
  /// a type of database server based on a given database name.
  /// An Instance is set on the <see cref="T:System.Data.Entity.Database" /> class to
  /// cause all DbContexts created with no connection information or just a database
  /// name or connection string to use a certain type of database server by default.
  /// Two implementations of this interface are provided: <see cref="T:System.Data.Entity.Infrastructure.SqlConnectionFactory" />
  /// is used to create connections to Microsoft SQL Server, including EXPRESS editions.
  /// <see cref="T:System.Data.Entity.Infrastructure.SqlCeConnectionFactory" /> is used to create connections to Microsoft SQL
  /// Server Compact Editions.
  /// Other implementations for other database servers can be added as needed.
  /// Note that implementations should be thread safe or immutable since they may
  /// be accessed by multiple threads at the same time.
  /// </summary>
  public interface IDbConnectionFactory
  {
    /// <summary>
    /// Creates a connection based on the given database name or connection string.
    /// </summary>
    /// <param name="nameOrConnectionString"> The database name or connection string. </param>
    /// <returns> An initialized DbConnection. </returns>
    DbConnection CreateConnection(string nameOrConnectionString);
  }
}
