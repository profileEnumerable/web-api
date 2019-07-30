// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.DropTableOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents dropping an existing table.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class DropTableOperation : MigrationOperation, IAnnotationTarget
  {
    private readonly string _name;
    private readonly CreateTableOperation _inverse;
    private readonly IDictionary<string, IDictionary<string, object>> _removedColumnAnnotations;
    private readonly IDictionary<string, object> _removedAnnotations;

    /// <summary>
    /// Initializes a new instance of the DropTableOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="name"> The name of the table to be dropped. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public DropTableOperation(string name, object anonymousArguments = null)
      : this(name, (IDictionary<string, object>) null, (IDictionary<string, IDictionary<string, object>>) null, (CreateTableOperation) null, anonymousArguments)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DropTableOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="name"> The name of the table to be dropped. </param>
    /// <param name="removedAnnotations">Custom annotations that exist on the table that is being dropped. May be null or empty.</param>
    /// <param name="removedColumnAnnotations">Custom annotations that exist on columns of the table that is being dropped. May be null or empty.</param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public DropTableOperation(
      string name,
      IDictionary<string, object> removedAnnotations,
      IDictionary<string, IDictionary<string, object>> removedColumnAnnotations,
      object anonymousArguments = null)
      : this(name, removedAnnotations, removedColumnAnnotations, (CreateTableOperation) null, anonymousArguments)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DropTableOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="name"> The name of the table to be dropped. </param>
    /// <param name="inverse"> An operation that represents reverting dropping the table. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public DropTableOperation(string name, CreateTableOperation inverse, object anonymousArguments = null)
      : this(name, (IDictionary<string, object>) null, (IDictionary<string, IDictionary<string, object>>) null, inverse, anonymousArguments)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DropTableOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="name"> The name of the table to be dropped. </param>
    /// <param name="removedAnnotations">Custom annotations that exist on the table that is being dropped. May be null or empty.</param>
    /// <param name="removedColumnAnnotations">Custom annotations that exist on columns of the table that is being dropped. May be null or empty.</param>
    /// <param name="inverse"> An operation that represents reverting dropping the table. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public DropTableOperation(
      string name,
      IDictionary<string, object> removedAnnotations,
      IDictionary<string, IDictionary<string, object>> removedColumnAnnotations,
      CreateTableOperation inverse,
      object anonymousArguments = null)
      : base(anonymousArguments)
    {
      Check.NotEmpty(name, nameof (name));
      this._name = name;
      this._removedAnnotations = removedAnnotations ?? (IDictionary<string, object>) new Dictionary<string, object>();
      this._removedColumnAnnotations = removedColumnAnnotations ?? (IDictionary<string, IDictionary<string, object>>) new Dictionary<string, IDictionary<string, object>>();
      this._inverse = inverse;
    }

    /// <summary>Gets the name of the table to be dropped.</summary>
    public virtual string Name
    {
      get
      {
        return this._name;
      }
    }

    /// <summary>
    /// Gets custom annotations that exist on the table that is being dropped.
    /// </summary>
    public virtual IDictionary<string, object> RemovedAnnotations
    {
      get
      {
        return this._removedAnnotations;
      }
    }

    /// <summary>
    /// Gets custom annotations that exist on columns of the table that is being dropped.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public IDictionary<string, IDictionary<string, object>> RemovedColumnAnnotations
    {
      get
      {
        return this._removedColumnAnnotations;
      }
    }

    /// <summary>
    /// Gets an operation that represents reverting dropping the table.
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
        return true;
      }
    }

    bool IAnnotationTarget.HasAnnotations
    {
      get
      {
        CreateTableOperation inverse = this.Inverse as CreateTableOperation;
        if (this.RemovedAnnotations.Any<KeyValuePair<string, object>>() || this.RemovedColumnAnnotations.Any<KeyValuePair<string, IDictionary<string, object>>>())
          return true;
        if (inverse != null)
          return ((IAnnotationTarget) inverse).HasAnnotations;
        return false;
      }
    }
  }
}
