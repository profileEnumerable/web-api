// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.PrimaryKeyOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Common base class to represent operations affecting primary keys.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public abstract class PrimaryKeyOperation : MigrationOperation
  {
    private readonly List<string> _columns = new List<string>();
    private string _table;
    private string _name;

    /// <summary>Returns the default name for the primary key.</summary>
    /// <param name="table">The target table name.</param>
    /// <returns>The default primary key name.</returns>
    public static string BuildDefaultName(string table)
    {
      Check.NotEmpty(table, nameof (table));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PK_{0}", (object) table).RestrictTo(128);
    }

    /// <summary>
    /// Initializes a new instance of the PrimaryKeyOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    protected PrimaryKeyOperation(object anonymousArguments = null)
      : base(anonymousArguments)
    {
    }

    /// <summary>
    /// Gets or sets the name of the table that contains the primary key.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public string Table
    {
      get
      {
        return this._table;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this._table = value;
      }
    }

    /// <summary>
    /// Gets the column(s) that make up the primary key.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public IList<string> Columns
    {
      get
      {
        return (IList<string>) this._columns;
      }
    }

    /// <summary>
    /// Gets a value indicating if a specific name has been supplied for this primary key.
    /// </summary>
    public bool HasDefaultName
    {
      get
      {
        return string.Equals(this.Name, this.DefaultName, StringComparison.Ordinal);
      }
    }

    /// <summary>
    /// Gets or sets the name of this primary key.
    /// If no name is supplied, a default name will be calculated.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public string Name
    {
      get
      {
        return this._name ?? this.DefaultName;
      }
      set
      {
        this._name = value;
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

    internal string DefaultName
    {
      get
      {
        return PrimaryKeyOperation.BuildDefaultName(this.Table);
      }
    }
  }
}
