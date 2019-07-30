// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.DropIndexOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents dropping an existing index.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class DropIndexOperation : IndexOperation
  {
    private readonly CreateIndexOperation _inverse;

    /// <summary>
    /// Initializes a new instance of the DropIndexOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public DropIndexOperation(object anonymousArguments = null)
      : base(anonymousArguments)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DropIndexOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="inverse"> The operation that represents reverting dropping the index. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public DropIndexOperation(CreateIndexOperation inverse, object anonymousArguments = null)
      : base(anonymousArguments)
    {
      Check.NotNull<CreateIndexOperation>(inverse, nameof (inverse));
      this._inverse = inverse;
    }

    /// <summary>
    /// Gets an operation that represents reverting dropping the index.
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
