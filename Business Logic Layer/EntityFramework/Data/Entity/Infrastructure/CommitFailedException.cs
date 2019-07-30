// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.CommitFailedException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;
using System.Runtime.Serialization;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Thrown when an error occurs committing a <see cref="T:System.Data.Common.DbTransaction" />.
  /// </summary>
  [Serializable]
  public class CommitFailedException : DataException
  {
    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Infrastructure.CommitFailedException" />
    /// </summary>
    public CommitFailedException()
      : base(Strings.CommitFailed)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Infrastructure.CommitFailedException" />
    /// </summary>
    /// <param name="message"> The exception message. </param>
    public CommitFailedException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Infrastructure.CommitFailedException" />
    /// </summary>
    /// <param name="message"> The exception message. </param>
    /// <param name="innerException"> The inner exception. </param>
    public CommitFailedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.CommitFailedException" /> class.
    /// </summary>
    /// <param name="info">The data necessary to serialize or deserialize an object.</param>
    /// <param name="context">Description of the source and destination of the specified serialized stream.</param>
    protected CommitFailedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
