// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DbCommandDispatcher
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// Used for dispatching operations to a <see cref="T:System.Data.Common.DbCommand" /> such that any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />
  /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> will be notified before and after the
  /// operation executes.
  /// Instances of this class are obtained through the the <see cref="P:System.Data.Entity.Infrastructure.Interception.DbInterception.Dispatch" /> fluent API.
  /// </summary>
  /// <remarks>
  /// This class is used internally by Entity Framework when executing commands. It is provided publicly so that
  /// code that runs outside of the core EF assemblies can opt-in to command interception/tracing. This is
  /// typically done by EF providers that are executing commands on behalf of EF.
  /// </remarks>
  public class DbCommandDispatcher
  {
    private readonly System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbCommandInterceptor> _internalDispatcher = new System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbCommandInterceptor>();

    internal System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbCommandInterceptor> InternalDispatcher
    {
      get
      {
        return this._internalDispatcher;
      }
    }

    internal DbCommandDispatcher()
    {
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.NonQueryExecuting(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Int32})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.NonQueryExecuted(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Int32})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbCommand.ExecuteNonQuery" />.
    /// </summary>
    /// <remarks>
    /// Note that the result of executing the command is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="command">The command on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual int NonQuery(DbCommand command, DbCommandInterceptionContext interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this._internalDispatcher.Dispatch<DbCommand, DbCommandInterceptionContext<int>, int>(command, (Func<DbCommand, DbCommandInterceptionContext<int>, int>) ((t, c) => t.ExecuteNonQuery()), new DbCommandInterceptionContext<int>((DbInterceptionContext) interceptionContext), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<int>>) ((i, t, c) => i.NonQueryExecuting(t, c)), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<int>>) ((i, t, c) => i.NonQueryExecuted(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.ScalarExecuting(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Object})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.ScalarExecuted(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Object})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbCommand.ExecuteScalar" />.
    /// </summary>
    /// <remarks>
    /// Note that the result of executing the command is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="command">The command on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual object Scalar(
      DbCommand command,
      DbCommandInterceptionContext interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this._internalDispatcher.Dispatch<DbCommand, DbCommandInterceptionContext<object>, object>(command, (Func<DbCommand, DbCommandInterceptionContext<object>, object>) ((t, c) => t.ExecuteScalar()), new DbCommandInterceptionContext<object>((DbInterceptionContext) interceptionContext), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<object>>) ((i, t, c) => i.ScalarExecuting(t, c)), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<object>>) ((i, t, c) => i.ScalarExecuted(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.ReaderExecuting(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Data.Common.DbDataReader})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.ReaderExecuted(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Data.Common.DbDataReader})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbCommand.ExecuteReader(System.Data.CommandBehavior)" />.
    /// </summary>
    /// <remarks>
    /// Note that the result of executing the command is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="command">The command on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual DbDataReader Reader(
      DbCommand command,
      DbCommandInterceptionContext interceptionContext)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this._internalDispatcher.Dispatch<DbCommand, DbCommandInterceptionContext<DbDataReader>, DbDataReader>(command, (Func<DbCommand, DbCommandInterceptionContext<DbDataReader>, DbDataReader>) ((t, c) => t.ExecuteReader(c.CommandBehavior)), new DbCommandInterceptionContext<DbDataReader>((DbInterceptionContext) interceptionContext), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<DbDataReader>>) ((i, t, c) => i.ReaderExecuting(t, c)), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<DbDataReader>>) ((i, t, c) => i.ReaderExecuted(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.NonQueryExecuting(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Int32})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.NonQueryExecuted(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Int32})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbCommand.ExecuteNonQueryAsync(System.Threading.CancellationToken)" />.
    /// </summary>
    /// <remarks>
    /// Note that the result of executing the command is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="command">The command on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual Task<int> NonQueryAsync(
      DbCommand command,
      DbCommandInterceptionContext interceptionContext,
      CancellationToken cancellationToken)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this._internalDispatcher.DispatchAsync<DbCommand, DbCommandInterceptionContext<int>, int>(command, (Func<DbCommand, DbCommandInterceptionContext<int>, CancellationToken, Task<int>>) ((t, c, ct) => t.ExecuteNonQueryAsync(ct)), new DbCommandInterceptionContext<int>((DbInterceptionContext) interceptionContext).AsAsync(), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<int>>) ((i, t, c) => i.NonQueryExecuting(t, c)), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<int>>) ((i, t, c) => i.NonQueryExecuted(t, c)), cancellationToken);
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.ScalarExecuting(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Object})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.ScalarExecuted(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Object})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbCommand.ExecuteScalarAsync(System.Threading.CancellationToken)" />.
    /// </summary>
    /// <remarks>
    /// Note that the result of executing the command is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="command">The command on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual Task<object> ScalarAsync(
      DbCommand command,
      DbCommandInterceptionContext interceptionContext,
      CancellationToken cancellationToken)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this._internalDispatcher.DispatchAsync<DbCommand, DbCommandInterceptionContext<object>, object>(command, (Func<DbCommand, DbCommandInterceptionContext<object>, CancellationToken, Task<object>>) ((t, c, ct) => t.ExecuteScalarAsync(ct)), new DbCommandInterceptionContext<object>((DbInterceptionContext) interceptionContext).AsAsync(), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<object>>) ((i, t, c) => i.ScalarExecuting(t, c)), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<object>>) ((i, t, c) => i.ScalarExecuted(t, c)), cancellationToken);
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.ReaderExecuting(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Data.Common.DbDataReader})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor.ReaderExecuted(System.Data.Common.DbCommand,System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext{System.Data.Common.DbDataReader})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbCommand.ExecuteReaderAsync(System.Data.CommandBehavior,System.Threading.CancellationToken)" />.
    /// </summary>
    /// <remarks>
    /// Note that the result of executing the command is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="command">The command on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual Task<DbDataReader> ReaderAsync(
      DbCommand command,
      DbCommandInterceptionContext interceptionContext,
      CancellationToken cancellationToken)
    {
      Check.NotNull<DbCommand>(command, nameof (command));
      Check.NotNull<DbCommandInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this._internalDispatcher.DispatchAsync<DbCommand, DbCommandInterceptionContext<DbDataReader>, DbDataReader>(command, (Func<DbCommand, DbCommandInterceptionContext<DbDataReader>, CancellationToken, Task<DbDataReader>>) ((t, c, ct) => t.ExecuteReaderAsync(c.CommandBehavior, ct)), new DbCommandInterceptionContext<DbDataReader>((DbInterceptionContext) interceptionContext).AsAsync(), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<DbDataReader>>) ((i, t, c) => i.ReaderExecuting(t, c)), (Action<IDbCommandInterceptor, DbCommand, DbCommandInterceptionContext<DbDataReader>>) ((i, t, c) => i.ReaderExecuted(t, c)), cancellationToken);
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

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the current instance.
    /// </summary>
    /// <returns>The exact runtime type of the current instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
