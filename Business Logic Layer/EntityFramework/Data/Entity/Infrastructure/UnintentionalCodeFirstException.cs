// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.UnintentionalCodeFirstException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;
using System.Runtime.Serialization;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Thrown when a context is generated from the <see cref="T:System.Data.Entity.DbContext" /> templates in Database First or Model
  /// First mode and is then used in Code First mode.
  /// </summary>
  /// <remarks>
  /// Code generated using the T4 templates provided for Database First and Model First use may not work
  /// correctly if used in Code First mode. To use these classes with Code First please add any additional
  /// configuration using attributes or the DbModelBuilder API and then remove the code that throws this
  /// exception.
  /// </remarks>
  [Serializable]
  public class UnintentionalCodeFirstException : InvalidOperationException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.UnintentionalCodeFirstException" /> class.
    /// </summary>
    public UnintentionalCodeFirstException()
      : base(Strings.UnintentionalCodeFirstException_Message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.UnintentionalCodeFirstException" /> class.
    /// </summary>
    /// <param name="info"> The object that holds the serialized object data. </param>
    /// <param name="context"> The contextual information about the source or destination. </param>
    protected UnintentionalCodeFirstException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.UnintentionalCodeFirstException" /> class.
    /// </summary>
    /// <param name="message"> The message. </param>
    public UnintentionalCodeFirstException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.UnintentionalCodeFirstException" /> class.
    /// </summary>
    /// <param name="message"> The message. </param>
    /// <param name="innerException"> The inner exception. </param>
    public UnintentionalCodeFirstException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
