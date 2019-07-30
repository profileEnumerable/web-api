// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.AddColumnOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents a column being added to a table.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class AddColumnOperation : MigrationOperation, IAnnotationTarget
  {
    private readonly string _table;
    private readonly ColumnModel _column;

    /// <summary>
    /// Initializes a new instance of the AddColumnOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="table"> The name of the table the column should be added to. </param>
    /// <param name="column"> Details of the column being added. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public AddColumnOperation(string table, ColumnModel column, object anonymousArguments = null)
      : base(anonymousArguments)
    {
      Check.NotEmpty(table, nameof (table));
      Check.NotNull<ColumnModel>(column, nameof (column));
      this._table = table;
      this._column = column;
    }

    /// <summary>
    /// Gets the name of the table the column should be added to.
    /// </summary>
    public string Table
    {
      get
      {
        return this._table;
      }
    }

    /// <summary>Gets the details of the column being added.</summary>
    public ColumnModel Column
    {
      get
      {
        return this._column;
      }
    }

    /// <summary>
    /// Gets an operation that represents dropping the added column.
    /// </summary>
    public override MigrationOperation Inverse
    {
      get
      {
        return (MigrationOperation) new DropColumnOperation(this.Table, this.Column.Name, (IDictionary<string, object>) this.Column.Annotations.ToDictionary<KeyValuePair<string, AnnotationValues>, string, object>((Func<KeyValuePair<string, AnnotationValues>, string>) (a => a.Key), (Func<KeyValuePair<string, AnnotationValues>, object>) (a => a.Value.NewValue)), (object) null);
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

    bool IAnnotationTarget.HasAnnotations
    {
      get
      {
        return this.Column.Annotations.Any<KeyValuePair<string, AnnotationValues>>();
      }
    }
  }
}
