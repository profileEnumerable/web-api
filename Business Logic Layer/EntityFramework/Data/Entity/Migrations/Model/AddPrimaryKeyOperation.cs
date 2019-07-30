// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents adding a primary key to a table.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class AddPrimaryKeyOperation : PrimaryKeyOperation
  {
    /// <summary>
    /// Initializes a new instance of the AddPrimaryKeyOperation class.
    /// The Table and Columns properties should also be populated.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public AddPrimaryKeyOperation(object anonymousArguments = null)
      : base(anonymousArguments)
    {
      this.IsClustered = true;
    }

    /// <summary>Gets an operation to drop the primary key.</summary>
    public override MigrationOperation Inverse
    {
      get
      {
        DropPrimaryKeyOperation primaryKeyOperation = new DropPrimaryKeyOperation((object) null);
        primaryKeyOperation.Name = this.Name;
        primaryKeyOperation.Table = this.Table;
        DropPrimaryKeyOperation dropPrimaryKeyOperation = primaryKeyOperation;
        this.Columns.Each<string>((Action<string>) (c => dropPrimaryKeyOperation.Columns.Add(c)));
        return (MigrationOperation) dropPrimaryKeyOperation;
      }
    }

    /// <summary>Gets or sets whether this is a clustered primary key.</summary>
    public bool IsClustered { get; set; }
  }
}
