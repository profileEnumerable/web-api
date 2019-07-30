// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DbTransactionDispatcher
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// Used for dispatching operations to a <see cref="T:System.Data.Common.DbTransaction" /> such that any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor" />
  /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> will be notified before and after the
  /// operation executes.
  /// Instances of this class are obtained through the the <see cref="P:System.Data.Entity.Infrastructure.Interception.DbInterception.Dispatch" /> fluent API.
  /// </summary>
  /// <remarks>
  /// This class is used internally by Entity Framework when interacting with <see cref="T:System.Data.Common.DbTransaction" />.
  /// It is provided publicly so that code that runs outside of the core EF assemblies can opt-in to command
  /// interception/tracing. This is typically done by EF providers that are executing commands on behalf of EF.
  /// </remarks>
  public class DbTransactionDispatcher
  {
    private readonly System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbTransactionInterceptor> _internalDispatcher = new System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbTransactionInterceptor>();

    internal System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbTransactionInterceptor> InternalDispatcher
    {
      get
      {
        return this._internalDispatcher;
      }
    }

    internal DbTransactionDispatcher()
    {
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.ConnectionGetting(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext{System.Data.Common.DbConnection})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.ConnectionGot(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext{System.Data.Common.DbConnection})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after
    /// getting <see cref="P:System.Data.Common.DbTransaction.Connection" />.
    /// </summary>
    /// <remarks>
    /// Note that the value of the property is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="transaction">The transaction on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual DbConnection GetConnection(
      DbTransaction transaction,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbTransaction>(transaction, nameof (transaction));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.Dispatch<DbTransaction, DbTransactionInterceptionContext<DbConnection>, DbConnection>(transaction, (Func<DbTransaction, DbTransactionInterceptionContext<DbConnection>, DbConnection>) ((t, c) => t.Connection), new DbTransactionInterceptionContext<DbConnection>(interceptionContext), (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext<DbConnection>>) ((i, t, c) => i.ConnectionGetting(t, c)), (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext<DbConnection>>) ((i, t, c) => i.ConnectionGot(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.IsolationLevelGetting(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext{System.Data.IsolationLevel})" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.IsolationLevelGot(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext{System.Data.IsolationLevel})" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after
    /// getting <see cref="P:System.Data.Common.DbTransaction.IsolationLevel" />.
    /// </summary>
    /// <remarks>
    /// Note that the value of the property is returned by this method. The result is not available
    /// in the interception context passed into this method since the interception context is cloned before
    /// being passed to interceptors.
    /// </remarks>
    /// <param name="transaction">The transaction on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    /// <returns>The result of the operation, which may have been modified by interceptors.</returns>
    public virtual IsolationLevel GetIsolationLevel(
      DbTransaction transaction,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbTransaction>(transaction, nameof (transaction));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      return this.InternalDispatcher.Dispatch<DbTransaction, DbTransactionInterceptionContext<IsolationLevel>, IsolationLevel>(transaction, (Func<DbTransaction, DbTransactionInterceptionContext<IsolationLevel>, IsolationLevel>) ((t, c) => t.IsolationLevel), new DbTransactionInterceptionContext<IsolationLevel>(interceptionContext), (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext<IsolationLevel>>) ((i, t, c) => i.IsolationLevelGetting(t, c)), (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext<IsolationLevel>>) ((i, t, c) => i.IsolationLevelGot(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.Committing(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.Committed(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbTransaction.Commit" />.
    /// </summary>
    /// <param name="transaction">The transaction on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    public virtual void Commit(DbTransaction transaction, DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbTransaction>(transaction, nameof (transaction));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      this.InternalDispatcher.Dispatch<DbTransaction, DbTransactionInterceptionContext>(transaction, (Action<DbTransaction, DbTransactionInterceptionContext>) ((t, c) => t.Commit()), new DbTransactionInterceptionContext(interceptionContext).WithConnection(transaction.Connection), (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext>) ((i, t, c) => i.Committing(t, c)), (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext>) ((i, t, c) => i.Committed(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.Disposing(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.Disposed(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbTransaction.Dispose" />.
    /// </summary>
    /// <param name="transaction">The transaction on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    public virtual void Dispose(
      DbTransaction transaction,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbTransaction>(transaction, nameof (transaction));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      DbTransactionInterceptionContext interceptionContext1 = new DbTransactionInterceptionContext(interceptionContext);
      if (transaction.Connection != null)
        interceptionContext1 = interceptionContext1.WithConnection(transaction.Connection);
      this.InternalDispatcher.Dispatch<DbTransaction, DbTransactionInterceptionContext>(transaction, (Action<DbTransaction, DbTransactionInterceptionContext>) ((t, c) => t.Dispose()), interceptionContext1, (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext>) ((i, t, c) => i.Disposing(t, c)), (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext>) ((i, t, c) => i.Disposed(t, c)));
    }

    /// <summary>
    /// Sends <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.RollingBack(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" /> and
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.RolledBack(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" /> to any <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor" />
    /// registered on <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> before/after making a
    /// call to <see cref="M:System.Data.Common.DbTransaction.Rollback" />.
    /// </summary>
    /// <param name="transaction">The transaction on which the operation will be executed.</param>
    /// <param name="interceptionContext">Optional information about the context of the call being made.</param>
    public virtual void Rollback(
      DbTransaction transaction,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbTransaction>(transaction, nameof (transaction));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      this.InternalDispatcher.Dispatch<DbTransaction, DbTransactionInterceptionContext>(transaction, (Action<DbTransaction, DbTransactionInterceptionContext>) ((t, c) => t.Rollback()), new DbTransactionInterceptionContext(interceptionContext).WithConnection(transaction.Connection), (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext>) ((i, t, c) => i.RollingBack(t, c)), (Action<IDbTransactionInterceptor, DbTransaction, DbTransactionInterceptionContext>) ((i, t, c) => i.RolledBack(t, c)));
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
