// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.History.HistoryRow
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.History
{
  /// <summary>
  /// This class is used by Code First Migrations to read and write migration history
  /// from the database.
  /// </summary>
  public class HistoryRow
  {
    /// <summary>
    /// Gets or sets the Id of the migration this row represents.
    /// </summary>
    public string MigrationId { get; set; }

    /// <summary>
    /// Gets or sets a key representing to which context the row applies.
    /// </summary>
    public string ContextKey { get; set; }

    /// <summary>
    /// Gets or sets the state of the model after this migration was applied.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
    public byte[] Model { get; set; }

    /// <summary>
    /// Gets or sets the version of Entity Framework that created this entry.
    /// </summary>
    public string ProductVersion { get; set; }
  }
}
