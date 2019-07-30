// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.MigrationsLogger
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Migrations.Infrastructure
{
  /// <summary>
  /// Base class for loggers that can be used for the migrations process.
  /// </summary>
  public abstract class MigrationsLogger : MarshalByRefObject
  {
    /// <summary>Logs an informational message.</summary>
    /// <param name="message"> The message to be logged. </param>
    public abstract void Info(string message);

    /// <summary>Logs a warning that the user should be made aware of.</summary>
    /// <param name="message"> The message to be logged. </param>
    public abstract void Warning(string message);

    /// <summary>
    /// Logs some additional information that should only be presented to the user if they request verbose output.
    /// </summary>
    /// <param name="message"> The message to be logged. </param>
    public abstract void Verbose(string message);
  }
}
