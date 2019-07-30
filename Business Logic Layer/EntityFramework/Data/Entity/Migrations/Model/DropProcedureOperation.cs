// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.DropProcedureOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Drops a stored procedure from the database.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class DropProcedureOperation : MigrationOperation
  {
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Migrations.Model.DropProcedureOperation" /> class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="name">The name of the stored procedure to drop.</param>
    /// <param name="anonymousArguments"> Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public DropProcedureOperation(string name, object anonymousArguments = null)
      : base(anonymousArguments)
    {
      Check.NotEmpty(name, nameof (name));
      this._name = name;
    }

    /// <summary>Gets the name of the stored procedure to drop.</summary>
    /// <value>The name of the stored procedure to drop.</value>
    public virtual string Name
    {
      get
      {
        return this._name;
      }
    }

    /// <summary>
    /// Gets an operation that will revert this operation.
    /// Always returns a <see cref="T:System.Data.Entity.Migrations.Model.NotSupportedOperation" />.
    /// </summary>
    public override MigrationOperation Inverse
    {
      get
      {
        return (MigrationOperation) NotSupportedOperation.Instance;
      }
    }

    /// <summary>
    /// Gets a value indicating if this operation may result in data loss. Always returns false.
    /// </summary>
    public override bool IsDestructiveChange
    {
      get
      {
        return false;
      }
    }
  }
}
