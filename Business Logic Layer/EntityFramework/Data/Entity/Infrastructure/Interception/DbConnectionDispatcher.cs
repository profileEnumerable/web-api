// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DbConnectionDispatcher
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
  /// Used for dispatching operations to a <see cref="T:System.Data.Common.DbConnection" /> such that any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
  /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> will be notified before and after the
  /// operation executes.
  /// Instances of this class are obtained through the the <see cref="P:System.Data.Entity.Infrastructure.Interception.DbInterception.Dispatch" /> fluent API.
  /// </summary>
  /// <remarks>
  /// This class is used internally by Entity Framework when interacting with <see cref="T:System.Data.Common.DbConnection" />.
  /// It is provided publicly so that code that runs outside of the core EF assemblies can opt-in to command
  /// interception/tracing. This is typically done by EF providers that are executing commands on behalf of EF.
  /// </remarks>
  public class DbConnectionDispatcher
  {
    private readonly System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbConnectionInterceptor> _internalDispatcher = new System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbConnectionInterceptor>();

    internal System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbConnectionInterceptor> InternalDispatcher
    {
      get
      {
        return this._internalDispatcher;
      }
    }

    internal DbConnectionDispatcher()
    {
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.BeginningTransaction(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext)" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.BeganTransaction(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext)" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbConnection.BeginTransaction(System.Data.IsolationLevel)" />.
    /// </summary>
    /// <remarks>
    /// Note that the result of executing the command is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual DbTransaction BeginTransaction(
      DbConnection connection,
      BeginTransactionInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<BeginTransactionInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.Dispatch<DbConnection, BeginTransactionInterceptionContext, DbTransaction>(connection, (Func<DbConnection, BeginTransactionInterceptionContext, DbTransaction>) ((t, c) => t.BeginTransaction(c.IsolationLevel)), new BeginTransactionInterceptionContext((DbInterceptionContext) interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, BeginTransactionInterceptionContext>) ((i, t, c) => i.BeginningTransaction(t, c)), (Action<IDbConnectionInterceptor, DbConnection, BeginTransactionInterceptionContext>) ((i, t, c) => i.BeganTransaction(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Closing(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Closed(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbConnection.Close" />.
    /// </summary>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    public virtual void Close(DbConnection connection, DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      this.InternalDispatcher.Dispatch<DbConnection, DbConnectionInterceptionContext>(connection, (Action<DbConnection, DbConnectionInterceptionContext>) ((t, c) => t.Close()), new DbConnectionInterceptionContext(interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext>) ((i, t, c) => i.Closing(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext>) ((i, t, c) => i.Closed(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Disposing(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Disposed(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.ComponentModel.Component.Dispose" />.
    /// </summary>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    public virtual void Dispose(DbConnection connection, DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      this.InternalDispatcher.Dispatch<DbConnection, DbConnectionInterceptionContext>(connection, (Action<DbConnection, DbConnectionInterceptionContext>) ((t, c) =>
      {
        using (t)
          ;
      }), new DbConnectionInterceptionContext(interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext>) ((i, t, c) => i.Disposing(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext>) ((i, t, c) => i.Disposed(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionStringGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionStringGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after
    /// getting <see cref="P:System.Data.Common.DbConnection.ConnectionString" />.
    /// </summary>
    /// <remarks>
    /// Note that the value of the property is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual string GetConnectionString(
      DbConnection connection,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.Dispatch<DbConnection, DbConnectionInterceptionContext<string>, string>(connection, (Func<DbConnection, DbConnectionInterceptionContext<string>, string>) ((t, c) => t.ConnectionString), new DbConnectionInterceptionContext<string>(interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<string>>) ((i, t, c) => i.ConnectionStringGetting(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<string>>) ((i, t, c) => i.ConnectionStringGot(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionStringSetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext{System.String})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionStringSet(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext{System.String})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after
    /// setting <see cref="P:System.Data.Common.DbConnection.ConnectionString" />.
    /// </summary>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Information about the context of the call being made, including the value to be set.</param>
    public virtual void SetConnectionString(
      DbConnection connection,
      DbConnectionPropertyInterceptionContext<string> interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbConnectionPropertyInterceptionContext<string>>(interceptionContext, nameof (interceptionContext));
      this.InternalDispatcher.Dispatch<DbConnection, DbConnectionPropertyInterceptionContext<string>>(connection, (Action<DbConnection, DbConnectionPropertyInterceptionContext<string>>) ((t, c) => t.ConnectionString = c.Value), new DbConnectionPropertyInterceptionContext<string>((DbInterceptionContext) interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionPropertyInterceptionContext<string>>) ((i, t, c) => i.ConnectionStringSetting(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionPropertyInterceptionContext<string>>) ((i, t, c) => i.ConnectionStringSet(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionTimeoutGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.Int32})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionTimeoutGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.Int32})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after
    /// getting <see cref="P:System.Data.Common.DbConnection.ConnectionTimeout" />.
    /// </summary>
    /// <remarks>
    /// Note that the value of the property is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual int GetConnectionTimeout(
      DbConnection connection,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.Dispatch<DbConnection, DbConnectionInterceptionContext<int>, int>(connection, (Func<DbConnection, DbConnectionInterceptionContext<int>, int>) ((t, c) => t.ConnectionTimeout), new DbConnectionInterceptionContext<int>(interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<int>>) ((i, t, c) => i.ConnectionTimeoutGetting(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<int>>) ((i, t, c) => i.ConnectionTimeoutGot(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.DatabaseGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.DatabaseGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after
    /// getting <see cref="P:System.Data.Common.DbConnection.Database" />.
    /// </summary>
    /// <remarks>
    /// Note that the value of the property is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual string GetDatabase(
      DbConnection connection,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.Dispatch<DbConnection, DbConnectionInterceptionContext<string>, string>(connection, (Func<DbConnection, DbConnectionInterceptionContext<string>, string>) ((t, c) => t.Database), new DbConnectionInterceptionContext<string>(interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<string>>) ((i, t, c) => i.DatabaseGetting(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<string>>) ((i, t, c) => i.DatabaseGot(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.DataSourceGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.DataSourceGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after
    /// getting <see cref="P:System.Data.Common.DbConnection.DataSource" />.
    /// </summary>
    /// <remarks>
    /// Note that the value of the property is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual string GetDataSource(
      DbConnection connection,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.Dispatch<DbConnection, DbConnectionInterceptionContext<string>, string>(connection, (Func<DbConnection, DbConnectionInterceptionContext<string>, string>) ((t, c) => t.DataSource), new DbConnectionInterceptionContext<string>(interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<string>>) ((i, t, c) => i.DataSourceGetting(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<string>>) ((i, t, c) => i.DataSourceGot(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.EnlistingTransaction(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.EnlistTransactionInterceptionContext)" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.EnlistedTransaction(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.EnlistTransactionInterceptionContext)" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbConnection.EnlistTransaction(System.Transactions.Transaction)" />.
    /// </summary>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    public virtual void EnlistTransaction(
      DbConnection connection,
      EnlistTransactionInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<EnlistTransactionInterceptionContext>(interceptionContext, nameof (interceptionContext));
      this.InternalDispatcher.Dispatch<DbConnection, EnlistTransactionInterceptionContext>(connection, (Action<DbConnection, EnlistTransactionInterceptionContext>) ((t, c) => t.EnlistTransaction(c.Transaction)), new EnlistTransactionInterceptionContext((DbInterceptionContext) interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, EnlistTransactionInterceptionContext>) ((i, t, c) => i.EnlistingTransaction(t, c)), (Action<IDbConnectionInterceptor, DbConnection, EnlistTransactionInterceptionContext>) ((i, t, c) => i.EnlistedTransaction(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Opening(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Opened(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbConnection.Open" />.
    /// </summary>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    public virtual void Open(DbConnection connection, DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      this.InternalDispatcher.Dispatch<DbConnection, DbConnectionInterceptionContext>(connection, (Action<DbConnection, DbConnectionInterceptionContext>) ((t, c) => t.Open()), new DbConnectionInterceptionContext(interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext>) ((i, t, c) => i.Opening(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext>) ((i, t, c) => i.Opened(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Opening(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Opened(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbConnection.Open" />.
    /// </summary>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task OpenAsync(
      DbConnection connection,
      DbInterceptionContext interceptionContext,
      CancellationToken cancellationToken)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.DispatchAsync<DbConnection, DbConnectionInterceptionContext>(connection, (Func<DbConnection, DbConnectionInterceptionContext, CancellationToken, Task>) ((t, c, ct) => t.OpenAsync(ct)), new DbConnectionInterceptionContext(interceptionContext).AsAsync(), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext>) ((i, t, c) => i.Opening(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext>) ((i, t, c) => i.Opened(t, c)), cancellationToken);
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ServerVersionGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ServerVersionGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after
    /// getting <see cref="P:System.Data.Common.DbConnection.ServerVersion" />.
    /// </summary>
    /// <remarks>
    /// Note that the value of the property is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual string GetServerVersion(
      DbConnection connection,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.Dispatch<DbConnection, DbConnectionInterceptionContext<string>, string>(connection, (Func<DbConnection, DbConnectionInterceptionContext<string>, string>) ((t, c) => t.ServerVersion), new DbConnectionInterceptionContext<string>(interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<string>>) ((i, t, c) => i.ServerVersionGetting(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<string>>) ((i, t, c) => i.ServerVersionGot(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.StateGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.Data.ConnectionState})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.StateGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.Data.ConnectionState})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after
    /// getting <see cref="P:System.Data.Common.DbConnection.State" />.
    /// </summary>
    /// <remarks>
    /// Note that the value of the property is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="connection">The connection on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual ConnectionState GetState(
      DbConnection connection,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.Dispatch<DbConnection, DbConnectionInterceptionContext<ConnectionState>, ConnectionState>(connection, (Func<DbConnection, DbConnectionInterceptionContext<ConnectionState>, ConnectionState>) ((t, c) => t.State), new DbConnectionInterceptionContext<ConnectionState>(interceptionContext), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<ConnectionState>>) ((i, t, c) => i.StateGetting(t, c)), (Action<IDbConnectionInterceptor, DbConnection, DbConnectionInterceptionContext<ConnectionState>>) ((i, t, c) => i.StateGot(t, c)));
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
