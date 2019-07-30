// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// This is the default log formatter used when some <see cref="T:System.Action`1" /> is set onto the <see cref="P:System.Data.Entity.Database.Log" />
  /// property. A different formatter can be used by creating a class that inherits from this class and overrides
  /// some or all methods to change behavior.
  /// </summary>
  /// <remarks>
  /// To set the new formatter create a code-based configuration for EF using <see cref="T:System.Data.Entity.DbConfiguration" /> and then
  /// set the formatter class to use with <see cref="M:System.Data.Entity.DbConfiguration.SetDatabaseLogFormatter(System.Func{System.Data.Entity.DbContext,System.Action{System.String},System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter})" />.
  /// Note that setting the type of formatter to use with this method does change the way command are
  /// logged when <see cref="P:System.Data.Entity.Database.Log" /> is used. It is still necessary to set a <see cref="T:System.Action`1" />
  /// onto <see cref="P:System.Data.Entity.Database.Log" /> before any commands will be logged.
  /// For more low-level control over logging/interception see <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" /> and
  /// <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" />.
  /// Interceptors can also be registered in the config file of the application.
  /// See http://go.microsoft.com/fwlink/?LinkId=260883 for more information about Entity Framework configuration.
  /// </remarks>
  public class DatabaseLogFormatter : IDbCommandInterceptor, IDbConnectionInterceptor, IDbTransactionInterceptor, IDbInterceptor
  {
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private readonly WeakReference _context;
    private readonly Action<string> _writeAction;

    /// <summary>
    /// Creates a formatter that will not filter by any <see cref="T:System.Data.Entity.DbContext" /> and will instead log every command
    /// from any context and also commands that do not originate from a context.
    /// </summary>
    /// <remarks>
    /// This constructor is not used when a delegate is set on <see cref="P:System.Data.Entity.Database.Log" />. Instead it can be
    /// used by setting the formatter directly using <see cref="M:System.Data.Entity.Infrastructure.Interception.DbInterception.Add(System.Data.Entity.Infrastructure.Interception.IDbInterceptor)" />.
    /// </remarks>
    /// <param name="writeAction">The delegate to which output will be sent.</param>
    public DatabaseLogFormatter(Action<string> writeAction)
    {
      Check.NotNull<Action<string>>(writeAction, nameof (writeAction));
      this._writeAction = writeAction;
    }

    /// <summary>
    /// Creates a formatter that will only log commands the come from the given <see cref="T:System.Data.Entity.DbContext" /> instance.
    /// </summary>
    /// <remarks>
    /// This constructor must be called by a class that inherits from this class to override the behavior
    /// of <see cref="P:System.Data.Entity.Database.Log" />.
    /// </remarks>
    /// <param name="context">
    /// The context for which commands should be logged. Pass null to log every command
    /// from any context and also commands that do not originate from a context.
    /// </param>
    /// <param name="writeAction">The delegate to which output will be sent.</param>
    public DatabaseLogFormatter(DbContext context, Action<string> writeAction)
    {
      Check.NotNull<Action<string>>(writeAction, nameof (writeAction));
      this._context = new WeakReference((object) context);
      this._writeAction = writeAction;
    }

    /// <summary>
    /// The context for which commands are being logged, or null if commands from all contexts are
    /// being logged.
    /// </summary>
    protected internal DbContext Context
    {
      get
      {
        if (this._context == null || !this._context.IsAlive)
          return (DbContext) null;
        return (DbContext) this._context.Target;
      }
    }

    internal Action<string> WriteAction
    {
      get
      {
        return this._writeAction;
      }
    }

    /// <summary>
    /// Writes the given string to the underlying write delegate.
    /// </summary>
    /// <param name="output">The string to write.</param>
    protected virtual void Write(string output)
    {
      this._writeAction(output);
    }

    /// <summary>
    /// The stop watch used to time executions. This stop watch is started at the end of
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.NonQueryExecuting(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Int32})" />, <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.ScalarExecuting(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Object})" />, and <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.ReaderExecuting(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Data.Common.DbDataReader})" />
    /// methods and is stopped at the beginning of the <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.NonQueryExecuted(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Int32})" />, <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.ScalarExecuted(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Object})" />,
    /// and <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.ReaderExecuted(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Data.Common.DbDataReader})" /> methods. If these methods are overridden and the stop watch is being used
    /// then the overrides should either call the base method or start/stop the watch themselves.
    /// </summary>
    protected internal Stopwatch Stopwatch
    {
      get
      {
        return this._stopwatch;
      }
    }

    /// <summary>
    /// This method is called before a call to <see cref="M:System.Data.Common.DbCommand.ExecuteNonQuery" /> or
    /// one of its async counterparts is made.
    /// The default implementation calls <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Executing``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" /> and starts <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Stopwatch" />.
    /// </summary>
    /// <param name="command">The command being executed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void NonQueryExecuting(
      DbCommand command,
      DbCommandInterceptionContext<int> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<int>>(interceptionContext, nameof (interceptionContext));
      this.Executing<int>(command, interceptionContext);
      this.Stopwatch.Restart();
    }

    /// <summary>
    /// This method is called after a call to <see cref="M:System.Data.Common.DbCommand.ExecuteNonQuery" /> or
    /// one of its async counterparts is made.
    /// The default implementation stops <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Stopwatch" /> and calls <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Executed``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" />.
    /// </summary>
    /// <param name="command">The command being executed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void NonQueryExecuted(
      DbCommand command,
      DbCommandInterceptionContext<int> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<int>>(interceptionContext, nameof (interceptionContext));
      this.Stopwatch.Stop();
      this.Executed<int>(command, interceptionContext);
    }

    /// <summary>
    /// This method is called before a call to <see cref="M:System.Data.Common.DbCommand.ExecuteReader(System.Data.CommandBehavior)" /> or
    /// one of its async counterparts is made.
    /// The default implementation calls <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Executing``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" /> and starts <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Stopwatch" />.
    /// </summary>
    /// <param name="command">The command being executed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ReaderExecuting(
      DbCommand command,
      DbCommandInterceptionContext<DbDataReader> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<DbDataReader>>(interceptionContext, nameof (interceptionContext));
      this.Executing<DbDataReader>(command, interceptionContext);
      this.Stopwatch.Restart();
    }

    /// <summary>
    /// This method is called after a call to <see cref="M:System.Data.Common.DbCommand.ExecuteReader(System.Data.CommandBehavior)" /> or
    /// one of its async counterparts is made.
    /// The default implementation stops <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Stopwatch" /> and calls <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Executed``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" />.
    /// </summary>
    /// <param name="command">The command being executed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ReaderExecuted(
      DbCommand command,
      DbCommandInterceptionContext<DbDataReader> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<DbDataReader>>(interceptionContext, nameof (interceptionContext));
      this.Stopwatch.Stop();
      this.Executed<DbDataReader>(command, interceptionContext);
    }

    /// <summary>
    /// This method is called before a call to <see cref="M:System.Data.Common.DbCommand.ExecuteScalar" />  or
    /// one of its async counterparts is made.
    /// The default implementation calls <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Executing``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" /> and starts <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Stopwatch" />.
    /// </summary>
    /// <param name="command">The command being executed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ScalarExecuting(
      DbCommand command,
      DbCommandInterceptionContext<object> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<object>>(interceptionContext, nameof (interceptionContext));
      this.Executing<object>(command, interceptionContext);
      this.Stopwatch.Restart();
    }

    /// <summary>
    /// This method is called after a call to <see cref="M:System.Data.Common.DbCommand.ExecuteScalar" />  or
    /// one of its async counterparts is made.
    /// The default implementation stops <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Stopwatch" /> and calls <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Executed``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" />.
    /// </summary>
    /// <param name="command">The command being executed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ScalarExecuted(
      DbCommand command,
      DbCommandInterceptionContext<object> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<object>>(interceptionContext, nameof (interceptionContext));
      this.Stopwatch.Stop();
      this.Executed<object>(command, interceptionContext);
    }

    /// <summary>
    /// Called whenever a command is about to be executed. The default implementation of this method
    /// filters by <see cref="T:System.Data.Entity.DbContext" /> set into <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Context" />, if any, and then calls
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.LogCommand``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" />. This method would typically only be overridden to change the
    /// context filtering behavior.
    /// </summary>
    /// <typeparam name="TResult">The type of the operation's results.</typeparam>
    /// <param name="command">The command that will be executed.</param>
    /// <param name="interceptionContext">Contextual information associated with the command.</param>
    public virtual void Executing<TResult>(
      DbCommand command,
      DbCommandInterceptionContext<TResult> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<TResult>>(interceptionContext, nameof (interceptionContext));
      if (this.Context != null && !interceptionContext.DbContexts.Contains<DbContext>(this.Context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)))
        return;
      this.LogCommand<TResult>(command, interceptionContext);
    }

    /// <summary>
    /// Called whenever a command has completed executing. The default implementation of this method
    /// filters by <see cref="T:System.Data.Entity.DbContext" /> set into <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Context" />, if any, and then calls
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.LogResult``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" />. This method would typically only be overridden to change the context
    /// filtering behavior.
    /// </summary>
    /// <typeparam name="TResult">The type of the operation's results.</typeparam>
    /// <param name="command">The command that was executed.</param>
    /// <param name="interceptionContext">Contextual information associated with the command.</param>
    public virtual void Executed<TResult>(
      DbCommand command,
      DbCommandInterceptionContext<TResult> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<TResult>>(interceptionContext, nameof (interceptionContext));
      if (this.Context != null && !interceptionContext.DbContexts.Contains<DbContext>(this.Context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)))
        return;
      this.LogResult<TResult>(command, interceptionContext);
    }

    /// <summary>
    /// Called to log a command that is about to be executed. Override this method to change how the
    /// command is logged to <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.WriteAction" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the operation's results.</typeparam>
    /// <param name="command">The command to be logged.</param>
    /// <param name="interceptionContext">Contextual information associated with the command.</param>
    public virtual void LogCommand<TResult>(
      DbCommand command,
      DbCommandInterceptionContext<TResult> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<TResult>>(interceptionContext, nameof (interceptionContext));
      string output = command.CommandText ?? "<null>";
      if (output.EndsWith(Environment.NewLine, StringComparison.Ordinal))
      {
        this.Write(output);
      }
      else
      {
        this.Write(output);
        this.Write(Environment.NewLine);
      }
      if (command.Parameters != null)
      {
        foreach (DbParameter parameter in command.Parameters.OfType<DbParameter>())
          this.LogParameter<TResult>(command, interceptionContext, parameter);
      }
      this.Write(interceptionContext.IsAsync ? Strings.CommandLogAsync((object) DateTimeOffset.Now, (object) Environment.NewLine) : Strings.CommandLogNonAsync((object) DateTimeOffset.Now, (object) Environment.NewLine));
    }

    /// <summary>
    /// Called by <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.LogCommand``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" /> to log each parameter. This method can be called from an overridden
    /// implementation of <see cref="M:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.LogCommand``1(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{``0})" /> to log parameters, and/or can be overridden to
    /// change the way that parameters are logged to <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.WriteAction" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the operation's results.</typeparam>
    /// <param name="command">The command being logged.</param>
    /// <param name="interceptionContext">Contextual information associated with the command.</param>
    /// <param name="parameter">The parameter to log.</param>
    public virtual void LogParameter<TResult>(
      DbCommand command,
      DbCommandInterceptionContext<TResult> interceptionContext,
      DbParameter parameter)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<TResult>>(interceptionContext, nameof (interceptionContext));
      Check.NotNull<DbParameter>(parameter, nameof (parameter));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("-- ").Append(parameter.ParameterName).Append(": '").Append(parameter.Value == null || parameter.Value == DBNull.Value ? (object) "null" : parameter.Value).Append("' (Type = ").Append((object) parameter.DbType);
      if (parameter.Direction != ParameterDirection.Input)
        stringBuilder.Append(", Direction = ").Append((object) parameter.Direction);
      if (!parameter.IsNullable)
        stringBuilder.Append(", IsNullable = false");
      if (parameter.Size != 0)
        stringBuilder.Append(", Size = ").Append(parameter.Size);
      if (((IDbDataParameter) parameter).Precision != (byte) 0)
        stringBuilder.Append(", Precision = ").Append(((IDbDataParameter) parameter).Precision);
      if (((IDbDataParameter) parameter).Scale != (byte) 0)
        stringBuilder.Append(", Scale = ").Append(((IDbDataParameter) parameter).Scale);
      stringBuilder.Append(")").Append(Environment.NewLine);
      this.Write(stringBuilder.ToString());
    }

    /// <summary>
    /// Called to log the result of executing a command. Override this method to change how results are
    /// logged to <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.WriteAction" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the operation's results.</typeparam>
    /// <param name="command">The command being logged.</param>
    /// <param name="interceptionContext">Contextual information associated with the command.</param>
    public virtual void LogResult<TResult>(
      DbCommand command,
      DbCommandInterceptionContext<TResult> interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext<TResult>>(interceptionContext, nameof (interceptionContext));
      if (interceptionContext.Exception != null)
        this.Write(Strings.CommandLogFailed((object) this.Stopwatch.ElapsedMilliseconds, (object) interceptionContext.Exception.Message, (object) Environment.NewLine));
      else if (interceptionContext.TaskStatus.HasFlag((Enum) TaskStatus.Canceled))
      {
        this.Write(Strings.CommandLogCanceled((object) this.Stopwatch.ElapsedMilliseconds, (object) Environment.NewLine));
      }
      else
      {
        TResult result = interceptionContext.Result;
        this.Write(Strings.CommandLogComplete((object) this.Stopwatch.ElapsedMilliseconds, (object) result == null ? (object) "null" : ((object) result is DbDataReader ? (object) result.GetType().Name : (object) result.ToString()), (object) Environment.NewLine));
      }
      this.Write(Environment.NewLine);
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection beginning the transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void BeginningTransaction(
      DbConnection connection,
      BeginTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>
    /// Called after <see cref="M:System.Data.Common.DbConnection.BeginTransaction(System.Data.IsolationLevel)" /> is invoked.
    /// The default implementation of this method filters by <see cref="T:System.Data.Entity.DbContext" /> set into
    /// <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Context" />, if any, and then logs the event.
    /// </summary>
    /// <param name="connection">The connection that began the transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void BeganTransaction(
      DbConnection connection,
      BeginTransactionInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<BeginTransactionInterceptionContext>(interceptionContext, nameof (interceptionContext));
      if (this.Context != null && !interceptionContext.DbContexts.Contains<DbContext>(this.Context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)))
        return;
      if (interceptionContext.Exception != null)
        this.Write(Strings.TransactionStartErrorLog((object) DateTimeOffset.Now, (object) interceptionContext.Exception.Message, (object) Environment.NewLine));
      else
        this.Write(Strings.TransactionStartedLog((object) DateTimeOffset.Now, (object) Environment.NewLine));
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void EnlistingTransaction(
      DbConnection connection,
      EnlistTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void EnlistedTransaction(
      DbConnection connection,
      EnlistTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection being opened.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Opening(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    /// <summary>
    /// Called after <see cref="M:System.Data.Common.DbConnection.Open" /> or its async counterpart is invoked.
    /// The default implementation of this method filters by <see cref="T:System.Data.Entity.DbContext" /> set into
    /// <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Context" />, if any, and then logs the event.
    /// </summary>
    /// <param name="connection">The connection that was opened.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Opened(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbConnectionInterceptionContext>(interceptionContext, nameof (interceptionContext));
      if (this.Context != null && !interceptionContext.DbContexts.Contains<DbContext>(this.Context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)))
        return;
      if (interceptionContext.Exception != null)
        this.Write(interceptionContext.IsAsync ? Strings.ConnectionOpenErrorLogAsync((object) DateTimeOffset.Now, (object) interceptionContext.Exception.Message, (object) Environment.NewLine) : Strings.ConnectionOpenErrorLog((object) DateTimeOffset.Now, (object) interceptionContext.Exception.Message, (object) Environment.NewLine));
      else if (interceptionContext.TaskStatus.HasFlag((Enum) TaskStatus.Canceled))
        this.Write(Strings.ConnectionOpenCanceledLog((object) DateTimeOffset.Now, (object) Environment.NewLine));
      else
        this.Write(interceptionContext.IsAsync ? Strings.ConnectionOpenedLogAsync((object) DateTimeOffset.Now, (object) Environment.NewLine) : Strings.ConnectionOpenedLog((object) DateTimeOffset.Now, (object) Environment.NewLine));
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection being closed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Closing(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    /// <summary>
    /// Called after <see cref="M:System.Data.Common.DbConnection.Close" /> is invoked.
    /// The default implementation of this method filters by <see cref="T:System.Data.Entity.DbContext" /> set into
    /// <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Context" />, if any, and then logs the event.
    /// </summary>
    /// <param name="connection">The connection that was closed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Closed(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbConnectionInterceptionContext>(interceptionContext, nameof (interceptionContext));
      if (this.Context != null && !interceptionContext.DbContexts.Contains<DbContext>(this.Context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)))
        return;
      if (interceptionContext.Exception != null)
        this.Write(Strings.ConnectionCloseErrorLog((object) DateTimeOffset.Now, (object) interceptionContext.Exception.Message, (object) Environment.NewLine));
      else
        this.Write(Strings.ConnectionClosedLog((object) DateTimeOffset.Now, (object) Environment.NewLine));
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ConnectionStringGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ConnectionStringGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ConnectionStringSetting(
      DbConnection connection,
      DbConnectionPropertyInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ConnectionStringSet(
      DbConnection connection,
      DbConnectionPropertyInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ConnectionTimeoutGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<int> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ConnectionTimeoutGot(
      DbConnection connection,
      DbConnectionInterceptionContext<int> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void DatabaseGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void DatabaseGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void DataSourceGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void DataSourceGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>
    /// Called before <see cref="M:System.ComponentModel.Component.Dispose" /> is invoked.
    /// The default implementation of this method filters by <see cref="T:System.Data.Entity.DbContext" /> set into
    /// <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Context" />, if any, and then logs the event.
    /// </summary>
    /// <param name="connection">The connection being disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Disposing(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbConnectionInterceptionContext>(interceptionContext, nameof (interceptionContext));
      if (this.Context != null && !interceptionContext.DbContexts.Contains<DbContext>(this.Context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)) || connection.State != ConnectionState.Open)
        return;
      this.Write(Strings.ConnectionDisposedLog((object) DateTimeOffset.Now, (object) Environment.NewLine));
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection that was disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Disposed(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ServerVersionGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ServerVersionGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void StateGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<ConnectionState> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void StateGot(
      DbConnection connection,
      DbConnectionInterceptionContext<ConnectionState> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ConnectionGetting(
      DbTransaction transaction,
      DbTransactionInterceptionContext<DbConnection> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void ConnectionGot(
      DbTransaction transaction,
      DbTransactionInterceptionContext<DbConnection> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void IsolationLevelGetting(
      DbTransaction transaction,
      DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void IsolationLevelGot(
      DbTransaction transaction,
      DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="transaction">The transaction being commited.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Committing(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>
    /// This method is called after <see cref="M:System.Data.Common.DbTransaction.Commit" /> is invoked.
    /// The default implementation of this method filters by <see cref="T:System.Data.Entity.DbContext" /> set into
    /// <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Context" />, if any, and then logs the event.
    /// </summary>
    /// <param name="transaction">The transaction that was commited.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Committed(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
      Check.NotNull<DbTransaction>(transaction, nameof (transaction));
      Check.NotNull<DbTransactionInterceptionContext>(interceptionContext, nameof (interceptionContext));
      if (this.Context != null && !interceptionContext.DbContexts.Contains<DbContext>(this.Context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)))
        return;
      if (interceptionContext.Exception != null)
        this.Write(Strings.TransactionCommitErrorLog((object) DateTimeOffset.Now, (object) interceptionContext.Exception.Message, (object) Environment.NewLine));
      else
        this.Write(Strings.TransactionCommittedLog((object) DateTimeOffset.Now, (object) Environment.NewLine));
    }

    /// <summary>
    /// This method is called before <see cref="M:System.Data.Common.DbTransaction.Dispose" /> is invoked.
    /// The default implementation of this method filters by <see cref="T:System.Data.Entity.DbContext" /> set into
    /// <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Context" />, if any, and then logs the event.
    /// </summary>
    /// <param name="transaction">The transaction being disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Disposing(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
      Check.NotNull<DbTransaction>(transaction, nameof (transaction));
      Check.NotNull<DbTransactionInterceptionContext>(interceptionContext, nameof (interceptionContext));
      if (this.Context != null && !interceptionContext.DbContexts.Contains<DbContext>(this.Context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)) || transaction.Connection == null)
        return;
      this.Write(Strings.TransactionDisposedLog((object) DateTimeOffset.Now, (object) Environment.NewLine));
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="transaction">The transaction that was disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Disposed(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Does not write to log unless overridden.</summary>
    /// <param name="transaction">The transaction being rolled back.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void RollingBack(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>
    /// This method is called after <see cref="M:System.Data.Common.DbTransaction.Rollback" /> is invoked.
    /// The default implementation of this method filters by <see cref="T:System.Data.Entity.DbContext" /> set into
    /// <see cref="P:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter.Context" />, if any, and then logs the event.
    /// </summary>
    /// <param name="transaction">The transaction that was rolled back.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void RolledBack(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
      Check.NotNull<DbTransaction>(transaction, nameof (transaction));
      Check.NotNull<DbTransactionInterceptionContext>(interceptionContext, nameof (interceptionContext));
      if (this.Context != null && !interceptionContext.DbContexts.Contains<DbContext>(this.Context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)))
        return;
      if (interceptionContext.Exception != null)
        this.Write(Strings.TransactionRollbackErrorLog((object) DateTimeOffset.Now, (object) interceptionContext.Exception.Message, (object) Environment.NewLine));
      else
        this.Write(Strings.TransactionRolledBackLog((object) DateTimeOffset.Now, (object) Environment.NewLine));
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
