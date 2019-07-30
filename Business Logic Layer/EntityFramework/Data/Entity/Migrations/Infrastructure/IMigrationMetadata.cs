// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.IMigrationMetadata
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Migrations.Infrastructure
{
  /// <summary>
  /// Provides additional metadata about a code-based migration.
  /// </summary>
  public interface IMigrationMetadata
  {
    /// <summary>Gets the unique identifier for the migration.</summary>
    string Id { get; }

    /// <summary>
    /// Gets the state of the model before this migration is run.
    /// </summary>
    string Source { get; }

    /// <summary>
    /// Gets the state of the model after this migration is run.
    /// </summary>
    string Target { get; }
  }
}
