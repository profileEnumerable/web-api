// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.ForeignKeyOperation
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
  /// Base class for changes that affect foreign key constraints.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public abstract class ForeignKeyOperation : MigrationOperation
  {
    private readonly List<string> _dependentColumns = new List<string>();
    private string _principalTable;
    private string _dependentTable;
    private string _name;

    /// <summary>
    /// Initializes a new instance of the ForeignKeyOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    protected ForeignKeyOperation(object anonymousArguments = null)
      : base(anonymousArguments)
    {
    }

    /// <summary>
    /// Gets or sets the name of the table that the foreign key constraint targets.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public string PrincipalTable
    {
      get
      {
        return this._principalTable;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this._principalTable = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the table that the foreign key columns exist in.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public string DependentTable
    {
      get
      {
        return this._dependentTable;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this._dependentTable = value;
      }
    }

    /// <summary>
    /// The names of the foreign key column(s).
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public IList<string> DependentColumns
    {
      get
      {
        return (IList<string>) this._dependentColumns;
      }
    }

    /// <summary>
    /// Gets a value indicating if a specific name has been supplied for this foreign key constraint.
    /// </summary>
    public bool HasDefaultName
    {
      get
      {
        return string.Equals(this.Name, this.DefaultName, StringComparison.Ordinal);
      }
    }

    /// <summary>
    /// Gets or sets the name of this foreign key constraint.
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

    internal string DefaultName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FK_{0}_{1}_{2}", (object) this.DependentTable, (object) this.PrincipalTable, (object) this.DependentColumns.Join<string>((Func<string, string>) null, "_")).RestrictTo(128);
      }
    }
  }
}
