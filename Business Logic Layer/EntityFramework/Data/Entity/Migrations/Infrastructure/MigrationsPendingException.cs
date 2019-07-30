// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.MigrationsPendingException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Runtime.Serialization;

namespace System.Data.Entity.Migrations.Infrastructure
{
  /// <summary>
  /// Thrown when an operation can't be performed because there are existing migrations that have not been applied to the database.
  /// </summary>
  [Serializable]
  public sealed class MigrationsPendingException : MigrationsException
  {
    /// <summary>
    /// Initializes a new instance of the MigrationsPendingException class.
    /// </summary>
    public MigrationsPendingException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the MigrationsPendingException class.
    /// </summary>
    /// <param name="message"> The message that describes the error. </param>
    public MigrationsPendingException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the MigrationsPendingException class.
    /// </summary>
    /// <param name="message"> The message that describes the error. </param>
    /// <param name="innerException"> The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
    public MigrationsPendingException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private MigrationsPendingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
