// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.IDbMigration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Migrations.Model;

namespace System.Data.Entity.Migrations.Infrastructure
{
  /// <summary>
  /// Explicitly implemented by <see cref="T:System.Data.Entity.Migrations.DbMigration" /> to prevent certain members from showing up
  /// in the IntelliSense of scaffolded migrations.
  /// </summary>
  public interface IDbMigration
  {
    /// <summary>
    /// Adds a custom <see cref="T:System.Data.Entity.Migrations.Model.MigrationOperation" /> to the migration.
    /// Custom operation implementors are encouraged to create extension methods on
    /// <see cref="T:System.Data.Entity.Migrations.Infrastructure.IDbMigration" /> that provide a fluent-style API for adding new operations.
    /// </summary>
    /// <param name="migrationOperation"> The operation to add. </param>
    void AddOperation(MigrationOperation migrationOperation);
  }
}
