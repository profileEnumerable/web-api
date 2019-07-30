// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.CreateProcedureOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// A migration operation to add a new stored procedure to the database.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class CreateProcedureOperation : ProcedureOperation
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Migrations.Model.CreateProcedureOperation" /> class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="bodySql">The body of the stored procedure expressed in SQL.</param>
    /// <param name="anonymousArguments"> Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public CreateProcedureOperation(string name, string bodySql, object anonymousArguments = null)
      : base(name, bodySql, anonymousArguments)
    {
    }

    /// <summary>Gets an operation to drop the stored procedure.</summary>
    public override MigrationOperation Inverse
    {
      get
      {
        return (MigrationOperation) new DropProcedureOperation(this.Name, (object) null);
      }
    }
  }
}
