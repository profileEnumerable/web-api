// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.AddForeignKeyOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents a foreign key constraint being added to a table.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class AddForeignKeyOperation : ForeignKeyOperation
  {
    private readonly List<string> _principalColumns = new List<string>();

    /// <summary>
    /// Initializes a new instance of the AddForeignKeyOperation class.
    /// The PrincipalTable, PrincipalColumns, DependentTable and DependentColumns properties should also be populated.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public AddForeignKeyOperation(object anonymousArguments = null)
      : base(anonymousArguments)
    {
    }

    /// <summary>
    /// The names of the column(s) that the foreign key constraint should target.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public IList<string> PrincipalColumns
    {
      get
      {
        return (IList<string>) this._principalColumns;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating if cascade delete should be configured on the foreign key constraint.
    /// </summary>
    public bool CascadeDelete { get; set; }

    /// <summary>
    /// Gets an operation to create an index on the foreign key column(s).
    /// </summary>
    /// <returns> An operation to add the index. </returns>
    public virtual CreateIndexOperation CreateCreateIndexOperation()
    {
      CreateIndexOperation createIndexOperation1 = new CreateIndexOperation((object) null);
      createIndexOperation1.Table = this.DependentTable;
      CreateIndexOperation createIndexOperation = createIndexOperation1;
      this.DependentColumns.Each<string>((Action<string>) (c => createIndexOperation.Columns.Add(c)));
      return createIndexOperation;
    }

    /// <summary>Gets an operation to drop the foreign key constraint.</summary>
    public override MigrationOperation Inverse
    {
      get
      {
        DropForeignKeyOperation foreignKeyOperation = new DropForeignKeyOperation((object) null);
        foreignKeyOperation.Name = this.Name;
        foreignKeyOperation.PrincipalTable = this.PrincipalTable;
        foreignKeyOperation.DependentTable = this.DependentTable;
        DropForeignKeyOperation dropForeignKeyOperation = foreignKeyOperation;
        this.DependentColumns.Each<string>((Action<string>) (c => dropForeignKeyOperation.DependentColumns.Add(c)));
        return (MigrationOperation) dropForeignKeyOperation;
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
