// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.RenameColumnOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents renaming an existing column.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class RenameColumnOperation : MigrationOperation
  {
    private readonly string _table;
    private readonly string _name;
    private string _newName;

    /// <summary>
    /// Initializes a new instance of the RenameColumnOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="table"> Name of the table the column belongs to. </param>
    /// <param name="name"> Name of the column to be renamed. </param>
    /// <param name="newName"> New name for the column. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public RenameColumnOperation(
      string table,
      string name,
      string newName,
      object anonymousArguments = null)
      : base(anonymousArguments)
    {
      Check.NotEmpty(table, nameof (table));
      Check.NotEmpty(name, nameof (name));
      Check.NotEmpty(newName, nameof (newName));
      this._table = table;
      this._name = name;
      this._newName = newName;
    }

    /// <summary>Gets the name of the table the column belongs to.</summary>
    public virtual string Table
    {
      get
      {
        return this._table;
      }
    }

    /// <summary>Gets the name of the column to be renamed.</summary>
    public virtual string Name
    {
      get
      {
        return this._name;
      }
    }

    /// <summary>Gets the new name for the column.</summary>
    public virtual string NewName
    {
      get
      {
        return this._newName;
      }
      internal set
      {
        this._newName = value;
      }
    }

    /// <summary>Gets an operation that reverts the rename.</summary>
    public override MigrationOperation Inverse
    {
      get
      {
        return (MigrationOperation) new RenameColumnOperation(this.Table, this.NewName, this.Name, (object) null);
      }
    }

    /// <inheritdoc />
    public override bool IsDestructiveChange
    {
      get
      {
        return false;
      }
    }
  }
}
