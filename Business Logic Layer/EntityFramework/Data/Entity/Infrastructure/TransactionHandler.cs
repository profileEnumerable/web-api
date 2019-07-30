// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.TransactionHandler
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// The base class for interceptors that handle the transaction operations. Derived classes can be registered using
  /// <see cref="M:System.Data.Entity.DbConfiguration.SetDefaultTransactionHandler(System.Func{System.Data.Entity.Infrastructure.TransactionHandler})" /> or
  /// <see cref="M:System.Data.Entity.DbConfiguration.SetTransactionHandler(System.String,System.Func{System.Data.Entity.Infrastructure.TransactionHandler},System.String)" />.
  /// </summary>
  public abstract class TransactionHandler : IDbTransactionInterceptor, IDbConnectionInterceptor, IDbInterceptor, IDisposable
  {
    private WeakReference _objectContext;
    private WeakReference _dbContext;
    private WeakReference _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.TransactionHandler" /> class.
    /// </summary>
    /// <remarks>
    /// One of the Initialize methods needs to be called before this instance can be used.
    /// </remarks>
    protected TransactionHandler()
    {
      DbInterception.Add((IDbInterceptor) this);
    }

    /// <summary>
    /// Initializes this instance using the specified context.
    /// </summary>
    /// <param name="context">The context for which transaction operations will be handled.</param>
    public virtual void Initialize(ObjectContext context)
    {
      Check.NotNull<ObjectContext>(context, nameof (context));
      if (this.ObjectContext != null || this.DbContext != null || this.Connection != null)
        throw new InvalidOperationException(Strings.TransactionHandler_AlreadyInitialized);
      this.ObjectContext = context;
      this.DbContext = context.InterceptionContext.DbContexts.FirstOrDefault<DbContext>();
      this.Connection = ((EntityConnection) this.ObjectContext.Connection).StoreConnection;
    }

    /// <summary>
    /// Initializes this instance using the specified context.
    /// </summary>
    /// <param name="context">The context for which transaction operations will be handled.</param>
    /// <param name="connection">The connection to use for the initialization.</param>
    /// <remarks>
    /// This method is called by migrations. It is important that no action is performed on the
    /// specified context that causes it to be initialized.
    /// </remarks>
    public virtual void Initialize(DbContext context, DbConnection connection)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      Check.NotNull<DbConnection>(connection, nameof (connection));
      if (this.ObjectContext != null || this.DbContext != null || this.Connection != null)
        throw new InvalidOperationException(Strings.TransactionHandler_AlreadyInitialized);
      this.DbContext = context;
      this.Connection = connection;
    }

    /// <summary>Gets the context.</summary>
    /// <value>
    /// The <see cref="P:System.Data.Entity.Infrastructure.TransactionHandler.ObjectContext" /> for which the transaction operations will be handled.
    /// </value>
    public ObjectContext ObjectContext
    {
      get
      {
        if (this._objectContext == null || !this._objectContext.IsAlive)
          return (ObjectContext) null;
        return (ObjectContext) this._objectContext.Target;
      }
      private set
      {
        this._objectContext = new WeakReference((object) value);
      }
    }

    /// <summary>Gets the context.</summary>
    /// <value>
    /// The <see cref="P:System.Data.Entity.Infrastructure.TransactionHandler.DbContext" /> for which the transaction operations will be handled, could be null.
    /// </value>
    public DbContext DbContext
    {
      get
      {
        if (this._dbContext == null || !this._dbContext.IsAlive)
          return (DbContext) null;
        return (DbContext) this._dbContext.Target;
      }
      private set
      {
        this._dbContext = new WeakReference((object) value);
      }
    }

    /// <summary>Gets the connection.</summary>
    /// <value>
    /// The <see cref="T:System.Data.Common.DbConnection" /> for which the transaction operations will be handled.
    /// </value>
    /// <remarks>
    /// This connection object is only used to determine whether a particular operation needs to be handled
    /// in cases where a context is not available.
    /// </remarks>
    public DbConnection Connection
    {
      get
      {
        if (this._connection == null || !this._connection.IsAlive)
          return (DbConnection) null;
        return (DbConnection) this._connection.Target;
      }
      private set
      {
        this._connection = new WeakReference((object) value);
      }
    }

    /// <inheritdoc />
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this transaction handler is disposed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if disposed; otherwise, <c>false</c>.
    /// </value>
    protected bool IsDisposed { get; set; }

    /// <summary>
    /// Releases the resources used by this transaction handler.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      if (this.IsDisposed)
        return;
      DbInterception.Remove((IDbInterceptor) this);
      this.IsDisposed = true;
    }

    /// <summary>
    /// Checks whether the supplied interception context contains the target context
    /// or the supplied connection is the same as the one used by the target context.
    /// </summary>
    /// <param name="connection">A connection.</param>
    /// <param name="interceptionContext">An interception context.</param>
    /// <returns>
    /// <c>true</c> if the supplied interception context contains the target context or
    /// the supplied connection is the same as the one used by the target context if
    /// the supplied interception context doesn't contain any contexts; <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// Note that calling this method will trigger initialization of any DbContext referenced from the <paramref name="interceptionContext" />
    /// </remarks>
    protected internal virtual bool MatchesParentContext(
      DbConnection connection,
      DbInterceptionContext interceptionContext)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<DbInterceptionContext>(interceptionContext, nameof (interceptionContext));
      if (this.DbContext != null && interceptionContext.DbContexts.Contains<DbContext>(this.DbContext, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)) || this.ObjectContext != null && interceptionContext.ObjectContexts.Contains<ObjectContext>(this.ObjectContext, new Func<ObjectContext, ObjectContext, bool>(object.ReferenceEquals)))
        return true;
      if (this.Connection != null && !interceptionContext.ObjectContexts.Any<ObjectContext>() && !interceptionContext.DbContexts.Any<DbContext>())
        return object.ReferenceEquals((object) connection, (object) this.Connection);
      return false;
    }

    /// <summary>
    /// When implemented in a derived class returns the script to prepare the database
    /// for this transaction handler.
    /// </summary>
    /// <returns>A script to change the database schema for this transaction handler.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public abstract string BuildDatabaseInitializationScript();

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection beginning the transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.BeginningTransaction(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext)" />
    public virtual void BeginningTransaction(
      DbConnection connection,
      BeginTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection that began the transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.BeganTransaction(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext)" />
    public virtual void BeganTransaction(
      DbConnection connection,
      BeginTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection being closed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Closing(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" />
    public virtual void Closing(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection that was closed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Closed(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" />
    public virtual void Closed(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionStringGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" />
    public virtual void ConnectionStringGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionStringGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" />
    public virtual void ConnectionStringGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionStringSetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext{System.String})" />
    public virtual void ConnectionStringSetting(
      DbConnection connection,
      DbConnectionPropertyInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionStringSet(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext{System.String})" />
    public virtual void ConnectionStringSet(
      DbConnection connection,
      DbConnectionPropertyInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionTimeoutGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.Int32})" />
    public virtual void ConnectionTimeoutGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<int> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ConnectionTimeoutGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.Int32})" />
    public virtual void ConnectionTimeoutGot(
      DbConnection connection,
      DbConnectionInterceptionContext<int> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.DatabaseGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" />
    public virtual void DatabaseGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.DatabaseGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" />
    public virtual void DatabaseGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.DataSourceGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" />
    public virtual void DataSourceGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.DataSourceGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" />
    public virtual void DataSourceGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection being disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Disposing(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection that was disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    public virtual void Disposed(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.EnlistingTransaction(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.EnlistTransactionInterceptionContext)" />
    public virtual void EnlistingTransaction(
      DbConnection connection,
      EnlistTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.EnlistedTransaction(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.EnlistTransactionInterceptionContext)" />
    public virtual void EnlistedTransaction(
      DbConnection connection,
      EnlistTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection being opened.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Opening(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" />
    public virtual void Opening(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection that was opened.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.Opened(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext)" />
    public virtual void Opened(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ServerVersionGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" />
    public virtual void ServerVersionGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.ServerVersionGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.String})" />
    public virtual void ServerVersionGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.StateGetting(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.Data.ConnectionState})" />
    public virtual void StateGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<ConnectionState> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor.StateGot(System.Data.Common.DbConnection,System.Data.Entity.Infrastructure.Interception.DbConnectionInterceptionContext{System.Data.ConnectionState})" />
    public virtual void StateGot(
      DbConnection connection,
      DbConnectionInterceptionContext<ConnectionState> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.ConnectionGetting(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext{System.Data.Common.DbConnection})" />
    public virtual void ConnectionGetting(
      DbTransaction transaction,
      DbTransactionInterceptionContext<DbConnection> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.ConnectionGot(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext{System.Data.Common.DbConnection})" />
    public virtual void ConnectionGot(
      DbTransaction transaction,
      DbTransactionInterceptionContext<DbConnection> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.IsolationLevelGetting(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext{System.Data.IsolationLevel})" />
    public virtual void IsolationLevelGetting(
      DbTransaction transaction,
      DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.IsolationLevelGot(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext{System.Data.IsolationLevel})" />
    public virtual void IsolationLevelGot(
      DbTransaction transaction,
      DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction being commited.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.Committing(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" />
    public virtual void Committing(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction that was commited.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.Committed(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" />
    public virtual void Committed(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction being disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.Disposing(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" />
    public virtual void Disposing(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction that was disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.Disposed(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" />
    public virtual void Disposed(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction being rolled back.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.RollingBack(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" />
    public virtual void RollingBack(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
    }

    /// <summary>Can be implemented in a derived class.</summary>
    /// <param name="transaction">The transaction that was rolled back.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor.RolledBack(System.Data.Common.DbTransaction,System.Data.Entity.Infrastructure.Interception.DbTransactionInterceptionContext)" />
    public virtual void RolledBack(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext)
    {
    }
  }
}
