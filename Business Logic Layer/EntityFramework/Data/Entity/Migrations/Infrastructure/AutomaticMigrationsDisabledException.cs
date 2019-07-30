// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.AutomaticMigrationsDisabledException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Runtime.Serialization;

namespace System.Data.Entity.Migrations.Infrastructure
{
  /// <summary>
  /// Represents an error that occurs when there are pending model changes after applying the last migration and automatic migration is disabled.
  /// </summary>
  [Serializable]
  public sealed class AutomaticMigrationsDisabledException : MigrationsException
  {
    /// <summary>
    /// Initializes a new instance of the AutomaticMigrationsDisabledException class.
    /// </summary>
    public AutomaticMigrationsDisabledException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the AutomaticMigrationsDisabledException class.
    /// </summary>
    /// <param name="message"> The message that describes the error. </param>
    public AutomaticMigrationsDisabledException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the MigrationsException class.
    /// </summary>
    /// <param name="message"> The message that describes the error. </param>
    /// <param name="innerException"> The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
    public AutomaticMigrationsDisabledException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private AutomaticMigrationsDisabledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
