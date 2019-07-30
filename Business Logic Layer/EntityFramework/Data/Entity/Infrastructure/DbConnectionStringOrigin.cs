// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbConnectionStringOrigin
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Describes the origin of the database connection string associated with a <see cref="T:System.Data.Entity.DbContext" />.
  /// </summary>
  public enum DbConnectionStringOrigin
  {
    /// <summary>The connection string was created by convention.</summary>
    Convention,
    /// <summary>
    /// The connection string was read from external configuration.
    /// </summary>
    Configuration,
    /// <summary>
    /// The connection string was explicitly specified at runtime.
    /// </summary>
    UserCode,
    /// <summary>
    /// The connection string was overriden by connection information supplied to DbContextInfo.
    /// </summary>
    DbContextInfo,
  }
}
