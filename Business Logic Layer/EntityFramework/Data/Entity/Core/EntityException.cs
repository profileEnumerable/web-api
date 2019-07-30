// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;
using System.Runtime.Serialization;

namespace System.Data.Entity.Core
{
  /// <summary>Provider exception - Used by the entity client.</summary>
  [Serializable]
  public class EntityException : DataException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityException" /> class.
    /// </summary>
    public EntityException()
      : base(Strings.EntityClient_ProviderGeneralError)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EntityException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that caused the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public EntityException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityException" /> class.
    /// </summary>
    /// <param name="info">
    /// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
    /// </param>
    protected EntityException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
