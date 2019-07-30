// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Design.ScaffoldedMigration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Design
{
  /// <summary>
  /// Represents a code-based migration that has been scaffolded and is ready to be written to a file.
  /// </summary>
  [Serializable]
  public class ScaffoldedMigration
  {
    private readonly Dictionary<string, object> _resources = new Dictionary<string, object>();
    private string _migrationId;
    private string _userCode;
    private string _designerCode;
    private string _language;
    private string _directory;

    /// <summary>
    /// Gets or sets the unique identifier for this migration.
    /// Typically used for the file name of the generated code.
    /// </summary>
    public string MigrationId
    {
      get
      {
        return this._migrationId;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this._migrationId = value;
      }
    }

    /// <summary>
    /// Gets or sets the scaffolded migration code that the user can edit.
    /// </summary>
    public string UserCode
    {
      get
      {
        return this._userCode;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this._userCode = value;
      }
    }

    /// <summary>
    /// Gets or sets the scaffolded migration code that should be stored in a code behind file.
    /// </summary>
    public string DesignerCode
    {
      get
      {
        return this._designerCode;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this._designerCode = value;
      }
    }

    /// <summary>
    /// Gets or sets the programming language used for this migration.
    /// Typically used for the file extension of the generated code.
    /// </summary>
    public string Language
    {
      get
      {
        return this._language;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this._language = value;
      }
    }

    /// <summary>
    /// Gets or sets the subdirectory in the user's project that this migration should be saved in.
    /// </summary>
    public string Directory
    {
      get
      {
        return this._directory;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this._directory = value;
      }
    }

    /// <summary>
    /// Gets a dictionary of string resources to add to the migration resource file.
    /// </summary>
    public IDictionary<string, object> Resources
    {
      get
      {
        return (IDictionary<string, object>) this._resources;
      }
    }

    /// <summary>Gets or sets whether the migration was re-scaffolded.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rescaffold")]
    public bool IsRescaffold { get; set; }
  }
}
