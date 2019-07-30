// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DatabaseLogger
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// A simple logger for logging SQL and other database operations to the console or a file.
  /// A logger can be registered in code or in the application's web.config /app.config file.
  /// </summary>
  public class DatabaseLogger : IDisposable, IDbConfigurationInterceptor, IDbInterceptor
  {
    private readonly object _lock = new object();
    private TextWriter _writer;
    private DatabaseLogFormatter _formatter;

    /// <summary>
    /// Creates a new logger that will send log output to the console.
    /// </summary>
    public DatabaseLogger()
    {
    }

    /// <summary>
    /// Creates a new logger that will send log output to a file. If the file already exists then
    /// it is overwritten.
    /// </summary>
    /// <param name="path">A path to the file to which log output will be written.</param>
    public DatabaseLogger(string path)
      : this(path, false)
    {
    }

    /// <summary>
    /// Creates a new logger that will send log output to a file.
    /// </summary>
    /// <param name="path">A path to the file to which log output will be written.</param>
    /// <param name="append">True to append data to the file if it exists; false to overwrite the file.</param>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public DatabaseLogger(string path, bool append)
    {
      Check.NotEmpty(path, nameof (path));
      this._writer = (TextWriter) new StreamWriter(path, append)
      {
        AutoFlush = true
      };
    }

    /// <summary>
    /// Stops logging and closes the underlying file if output is being written to a file.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Stops logging and closes the underlying file if output is being written to a file.
    /// </summary>
    /// <param name="disposing">
    /// True to release both managed and unmanaged resources; False to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      this.StopLogging();
      if (!disposing || this._writer == null)
        return;
      this._writer.Dispose();
      this._writer = (TextWriter) null;
    }

    /// <summary>
    /// Starts logging. This method is a no-op if logging is already started.
    /// </summary>
    public void StartLogging()
    {
      this.StartLogging(DbConfiguration.DependencyResolver);
    }

    /// <summary>
    /// Stops logging. This method is a no-op if logging is not started.
    /// </summary>
    public void StopLogging()
    {
      if (this._formatter == null)
        return;
      DbInterception.Remove((IDbInterceptor) this._formatter);
      this._formatter = (DatabaseLogFormatter) null;
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IDbConfigurationInterceptor.Loaded(
      DbConfigurationLoadedEventArgs loadedEventArgs,
      DbConfigurationInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConfigurationLoadedEventArgs>(loadedEventArgs, nameof (loadedEventArgs));
      Check.NotNull<DbConfigurationInterceptionContext>(interceptionContext, nameof (interceptionContext));
      this.StartLogging(loadedEventArgs.DependencyResolver);
    }

    private void StartLogging(IDbDependencyResolver resolver)
    {
      if (this._formatter != null)
        return;
      this._formatter = resolver.GetService<Func<DbContext, Action<string>, DatabaseLogFormatter>>()((DbContext) null, this._writer == null ? new Action<string>(Console.Write) : new Action<string>(this.WriteThreadSafe));
      DbInterception.Add((IDbInterceptor) this._formatter);
    }

    private void WriteThreadSafe(string value)
    {
      lock (this._lock)
        this._writer.Write(value);
    }
  }
}
