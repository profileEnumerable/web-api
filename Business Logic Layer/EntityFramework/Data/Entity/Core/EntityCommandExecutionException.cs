// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityCommandExecutionException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Runtime.Serialization;

namespace System.Data.Entity.Core
{
  /// <summary>
  /// Represents a failure while trying to prepare or execute a CommandExecution
  /// This exception is intended to provide a common exception that people can catch to
  /// hold provider exceptions (SqlException, OracleException) when using the EntityCommand
  /// to execute statements.
  /// </summary>
  [Serializable]
  public sealed class EntityCommandExecutionException : EntityException
  {
    private const int HResultCommandExecution = -2146232004;

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Core.EntityCommandExecutionException" />.
    /// </summary>
    public EntityCommandExecutionException()
    {
      this.HResult = -2146232004;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Core.EntityCommandExecutionException" />.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EntityCommandExecutionException(string message)
      : base(message)
    {
      this.HResult = -2146232004;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Core.EntityCommandExecutionException" />.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that caused the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public EntityCommandExecutionException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.HResult = -2146232004;
    }

    private EntityCommandExecutionException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
      this.HResult = -2146232004;
    }
  }
}
