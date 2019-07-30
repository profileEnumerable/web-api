// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.SqlOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents a provider specific SQL statement to be executed directly against the target database.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class SqlOperation : MigrationOperation
  {
    private readonly string _sql;

    /// <summary>
    /// Initializes a new instance of the SqlOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="sql"> The SQL to be executed. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public SqlOperation(string sql, object anonymousArguments = null)
      : base(anonymousArguments)
    {
      Check.NotEmpty(sql, nameof (sql));
      this._sql = sql;
    }

    /// <summary>Gets the SQL to be executed.</summary>
    public virtual string Sql
    {
      get
      {
        return this._sql;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this statement should be performed outside of
    /// the transaction scope that is used to make the migration process transactional.
    /// If set to true, this operation will not be rolled back if the migration process fails.
    /// </summary>
    public virtual bool SuppressTransaction { get; set; }

    /// <inheritdoc />
    public override bool IsDestructiveChange
    {
      get
      {
        return true;
      }
    }
  }
}
