// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Validation.DbUnexpectedValidationException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Data.Entity.Validation
{
  /// <summary>
  /// Exception thrown from <see cref="M:System.Data.Entity.DbContext.GetValidationErrors" /> when an exception is thrown from the validation
  /// code.
  /// </summary>
  [Serializable]
  public class DbUnexpectedValidationException : DataException
  {
    /// <summary>
    /// Initializes a new instance of DbUnexpectedValidationException.
    /// </summary>
    public DbUnexpectedValidationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of DbUnexpectedValidationException.
    /// </summary>
    /// <param name="message"> The exception message. </param>
    public DbUnexpectedValidationException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of DbUnexpectedValidationException.
    /// </summary>
    /// <param name="message"> The exception message. </param>
    /// <param name="innerException"> The inner exception. </param>
    public DbUnexpectedValidationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of DbUnexpectedValidationException with the specified serialization info and
    /// context.
    /// </summary>
    /// <param name="info"> The serialization info. </param>
    /// <param name="context"> The streaming context. </param>
    [ExcludeFromCodeCoverage]
    protected DbUnexpectedValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
