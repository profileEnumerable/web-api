// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.DropForeignKeyOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents a foreign key constraint being dropped from a table.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class DropForeignKeyOperation : ForeignKeyOperation
  {
    private readonly AddForeignKeyOperation _inverse;

    /// <summary>
    /// Initializes a new instance of the DropForeignKeyOperation class.
    /// The PrincipalTable, DependentTable and DependentColumns properties should also be populated.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public DropForeignKeyOperation(object anonymousArguments = null)
      : base(anonymousArguments)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DropForeignKeyOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc..
    /// </summary>
    /// <param name="inverse"> The operation that represents reverting dropping the foreign key constraint. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public DropForeignKeyOperation(AddForeignKeyOperation inverse, object anonymousArguments = null)
      : base(anonymousArguments)
    {
      Check.NotNull<AddForeignKeyOperation>(inverse, nameof (inverse));
      this._inverse = inverse;
    }

    /// <summary>
    /// Gets an operation to drop the associated index on the foreign key column(s).
    /// </summary>
    /// <returns> An operation to drop the index. </returns>
    public virtual DropIndexOperation CreateDropIndexOperation()
    {
      DropIndexOperation dropIndexOperation1 = new DropIndexOperation(this._inverse.CreateCreateIndexOperation(), (object) null);
      dropIndexOperation1.Table = this.DependentTable;
      DropIndexOperation dropIndexOperation = dropIndexOperation1;
      this.DependentColumns.Each<string>((Action<string>) (c => dropIndexOperation.Columns.Add(c)));
      return dropIndexOperation;
    }

    /// <summary>
    /// Gets an operation that represents reverting dropping the foreign key constraint.
    /// The inverse cannot be automatically calculated,
    /// if it was not supplied to the constructor this property will return null.
    /// </summary>
    public override MigrationOperation Inverse
    {
      get
      {
        return (MigrationOperation) this._inverse;
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
