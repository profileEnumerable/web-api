// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.EntityConnection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Data.Entity.Core.EntityClient
{
  /// <summary>
  /// Class representing a connection for the conceptual layer. An entity connection may only
  /// be initialized once (by opening the connection). It is subsequently not possible to change
  /// the connection string, attach a new store connection, or change the store connection string.
  /// </summary>
  public class EntityConnection : DbConnection
  {
    private static readonly System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions _emptyConnectionOptions = new System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions(string.Empty, (IList<string>) new string[0]);
    private readonly object _connectionStringLock = new object();
    private readonly bool _entityConnectionOwnsStoreConnection = true;
    private readonly List<ObjectContext> _associatedContexts = new List<ObjectContext>();
    private const string EntityClientProviderName = "System.Data.EntityClient";
    private const string ProviderInvariantName = "provider";
    private const string ProviderConnectionString = "provider connection string";
    private const string ReaderPrefix = "reader://";
    private System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions _userConnectionOptions;
    private System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions _effectiveConnectionOptions;
    private ConnectionState _entityClientConnectionState;
    private DbProviderFactory _providerFactory;
    private DbConnection _storeConnection;
    private MetadataWorkspace _metadataWorkspace;
    private EntityTransaction _currentTransaction;
    private Transaction _enlistedTransaction;
    private bool _initialized;
    private ConnectionState? _fakeConnectionState;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> class.
    /// </summary>
    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Object is in fact passed to property of the class and gets Disposed properly in the Dispose() method.")]
    public EntityConnection()
      : this(string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> class, based on the connection string.
    /// </summary>
    /// <param name="connectionString">The provider-specific connection string.</param>
    /// <exception cref="T:System.ArgumentException">An invalid connection string keyword has been provided, or a required connection string keyword has not been provided.</exception>
    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Object is in fact passed to property of the class and gets Disposed properly in the Dispose() method.")]
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EntityConnection(string connectionString)
    {
      this.ChangeConnectionString(connectionString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> class with a specified
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> and
    /// <see cref="T:System.Data.Common.DbConnection" />.
    /// </summary>
    /// <param name="workspace">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> to be associated with this
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />.
    /// </param>
    /// <param name="connection">
    /// The underlying data source connection for this <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> object.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">The  workspace  or  connection  parameter is null.</exception>
    /// <exception cref="T:System.ArgumentException">The conceptual model is missing from the workspace.-or-The mapping file is missing from the workspace.-or-The storage model is missing from the workspace.-or-The  connection  is not in a closed state.</exception>
    /// <exception cref="T:System.Data.Entity.Core.ProviderIncompatibleException">The  connection  is not from an ADO.NET Entity Framework-compatible provider.</exception>
    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Object is in fact passed to property of the class and gets Disposed properly in the Dispose() method.")]
    public EntityConnection(MetadataWorkspace workspace, DbConnection connection)
      : this(Check.NotNull<MetadataWorkspace>(workspace, nameof (workspace)), Check.NotNull<DbConnection>(connection, nameof (connection)), false, false)
    {
    }

    /// <summary>
    /// Constructs the EntityConnection from Metadata loaded in memory
    /// </summary>
    /// <param name="workspace"> Workspace containing metadata information. </param>
    /// <param name="connection"> Store connection. </param>
    /// <param name="entityConnectionOwnsStoreConnection"> If set to true the store connection is disposed when the entity connection is disposed, otherwise the caller must dispose the store connection. </param>
    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Object is in fact passed to property of the class and gets Disposed properly in the Dispose() method.")]
    public EntityConnection(
      MetadataWorkspace workspace,
      DbConnection connection,
      bool entityConnectionOwnsStoreConnection)
      : this(Check.NotNull<MetadataWorkspace>(workspace, nameof (workspace)), Check.NotNull<DbConnection>(connection, nameof (connection)), false, entityConnectionOwnsStoreConnection)
    {
    }

    internal EntityConnection(
      MetadataWorkspace workspace,
      DbConnection connection,
      bool skipInitialization,
      bool entityConnectionOwnsStoreConnection)
    {
      if (!skipInitialization)
      {
        if (!workspace.IsItemCollectionAlreadyRegistered(DataSpace.CSpace))
          throw new ArgumentException(Strings.EntityClient_ItemCollectionsNotRegisteredInWorkspace((object) "EdmItemCollection"));
        if (!workspace.IsItemCollectionAlreadyRegistered(DataSpace.SSpace))
          throw new ArgumentException(Strings.EntityClient_ItemCollectionsNotRegisteredInWorkspace((object) "StoreItemCollection"));
        if (!workspace.IsItemCollectionAlreadyRegistered(DataSpace.CSSpace))
          throw new ArgumentException(Strings.EntityClient_ItemCollectionsNotRegisteredInWorkspace((object) "StorageMappingItemCollection"));
        if (connection.GetProviderFactory() == null)
          throw new ProviderIncompatibleException(Strings.EntityClient_DbConnectionHasNoProvider((object) connection));
        this._providerFactory = ((StoreItemCollection) workspace.GetItemCollection(DataSpace.SSpace)).ProviderFactory;
        this._initialized = true;
      }
      this._metadataWorkspace = workspace;
      this._storeConnection = connection;
      this._entityConnectionOwnsStoreConnection = entityConnectionOwnsStoreConnection;
      if (this._storeConnection != null)
        this._entityClientConnectionState = DbInterception.Dispatch.Connection.GetState(this._storeConnection, this.InterceptionContext);
      this.SubscribeToStoreConnectionStateChangeEvents();
    }

    private void SubscribeToStoreConnectionStateChangeEvents()
    {
      if (this._storeConnection == null)
        return;
      this._storeConnection.StateChange += new StateChangeEventHandler(this.StoreConnectionStateChangeHandler);
    }

    private void UnsubscribeFromStoreConnectionStateChangeEvents()
    {
      if (this._storeConnection == null)
        return;
      this._storeConnection.StateChange -= new StateChangeEventHandler(this.StoreConnectionStateChangeHandler);
    }

    internal virtual void StoreConnectionStateChangeHandler(
      object sender,
      StateChangeEventArgs stateChange)
    {
      ConnectionState currentState = stateChange.CurrentState;
      if (this._entityClientConnectionState == currentState)
        return;
      ConnectionState clientConnectionState = this._entityClientConnectionState;
      this._entityClientConnectionState = stateChange.CurrentState;
      this.OnStateChange(new StateChangeEventArgs(clientConnectionState, currentState));
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> connection string.
    /// </summary>
    /// <returns>The connection string required to establish the initial connection to a data source. The default value is an empty string. On a closed connection, the currently set value is returned. If no value has been set, an empty string is returned.</returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// An attempt was made to set the <see cref="P:System.Data.Entity.Core.EntityClient.EntityConnection.ConnectionString" /> property after the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// ’s <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> was initialized. The
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" />
    /// is initialized either when the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> instance is constructed through the overload that takes a
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" />
    /// as a parameter, or when the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// instance has been opened.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">An invalid connection string keyword has been provided or a required connection string keyword has not been provided.</exception>
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public override string ConnectionString
    {
      get
      {
        if (this._userConnectionOptions == null)
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={3}{4};{1}={5};{2}=\"{6}\";", (object) "metadata", (object) "provider", (object) "provider connection string", (object) "reader://", (object) this._metadataWorkspace.MetadataWorkspaceId, (object) this._storeConnection.GetProviderInvariantName(), (object) DbInterception.Dispatch.Connection.GetConnectionString(this._storeConnection, this.InterceptionContext));
        string connectionString1 = this._userConnectionOptions.UsersConnectionString;
        if (object.ReferenceEquals((object) this._userConnectionOptions, (object) this._effectiveConnectionOptions) && this._storeConnection != null)
        {
          string connectionString2;
          try
          {
            connectionString2 = DbInterception.Dispatch.Connection.GetConnectionString(this._storeConnection, this.InterceptionContext);
          }
          catch (Exception ex)
          {
            if (ex.IsCatchableExceptionType())
              throw new EntityException(Strings.EntityClient_ProviderSpecificError((object) nameof (ConnectionString)), ex);
            throw;
          }
          string connectionOption = this._userConnectionOptions["provider connection string"];
          if (connectionString2 != connectionOption && (!string.IsNullOrEmpty(connectionString2) || !string.IsNullOrEmpty(connectionOption)))
            return new EntityConnectionStringBuilder(connectionString1)
            {
              ProviderConnectionString = connectionString2
            }.ConnectionString;
        }
        return connectionString1;
      }
      set
      {
        if (this._initialized)
          throw new InvalidOperationException(Strings.EntityClient_SettingsCannotBeChangedOnOpenConnection);
        this.ChangeConnectionString(value);
      }
    }

    internal IEnumerable<ObjectContext> AssociatedContexts
    {
      get
      {
        return (IEnumerable<ObjectContext>) this._associatedContexts;
      }
    }

    internal virtual void AssociateContext(ObjectContext context)
    {
      if (this._associatedContexts.Count != 0)
      {
        foreach (ObjectContext objectContext in this._associatedContexts.ToArray())
        {
          if (object.ReferenceEquals((object) context, (object) objectContext) || objectContext.IsDisposed)
            this._associatedContexts.Remove(objectContext);
        }
      }
      this._associatedContexts.Add(context);
    }

    internal DbInterceptionContext InterceptionContext
    {
      get
      {
        return DbInterceptionContext.Combine(this.AssociatedContexts.Select<ObjectContext, DbInterceptionContext>((Func<ObjectContext, DbInterceptionContext>) (c => c.InterceptionContext)));
      }
    }

    /// <summary>Gets the number of seconds to wait when attempting to establish a connection before ending the attempt and generating an error.</summary>
    /// <returns>The time (in seconds) to wait for a connection to open. The default value is the underlying data provider's default time-out. </returns>
    /// <exception cref="T:System.ArgumentException">The value set is less than 0. </exception>
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public override int ConnectionTimeout
    {
      get
      {
        if (this._storeConnection == null)
          return 0;
        try
        {
          return DbInterception.Dispatch.Connection.GetConnectionTimeout(this._storeConnection, this.InterceptionContext);
        }
        catch (Exception ex)
        {
          if (ex.IsCatchableExceptionType())
            throw new EntityException(Strings.EntityClient_ProviderSpecificError((object) nameof (ConnectionTimeout)), ex);
          throw;
        }
      }
    }

    /// <summary>Gets the name of the current database, or the database that will be used after a connection is opened.</summary>
    /// <returns>The value of the Database property of the underlying data provider.</returns>
    /// <exception cref="T:System.InvalidOperationException">The underlying data provider is not known. </exception>
    public override string Database
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets the state of the EntityConnection, which is set up to track the state of the underlying
    /// database connection that is wrapped by this EntityConnection.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public override ConnectionState State
    {
      get
      {
        return this._fakeConnectionState ?? this._entityClientConnectionState;
      }
    }

    /// <summary>Gets the name or network address of the data source to connect to.</summary>
    /// <returns>The name of the data source. The default value is an empty string.</returns>
    /// <exception cref="T:System.InvalidOperationException">The underlying data provider is not known. </exception>
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public override string DataSource
    {
      get
      {
        if (this._storeConnection == null)
          return string.Empty;
        try
        {
          return DbInterception.Dispatch.Connection.GetDataSource(this._storeConnection, this.InterceptionContext);
        }
        catch (Exception ex)
        {
          if (ex.IsCatchableExceptionType())
            throw new EntityException(Strings.EntityClient_ProviderSpecificError((object) nameof (DataSource)), ex);
          throw;
        }
      }
    }

    /// <summary>Gets a string that contains the version of the data source to which the client is connected.</summary>
    /// <returns>The version of the data source that is contained in the provider connection string.</returns>
    /// <exception cref="T:System.InvalidOperationException">The connection is closed. </exception>
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public override string ServerVersion
    {
      get
      {
        if (this._storeConnection == null)
          throw Error.EntityClient_ConnectionStringNeededBeforeOperation();
        if (this.State != ConnectionState.Open)
          throw Error.EntityClient_ConnectionNotOpen();
        try
        {
          return DbInterception.Dispatch.Connection.GetServerVersion(this._storeConnection, this.InterceptionContext);
        }
        catch (Exception ex)
        {
          if (ex.IsCatchableExceptionType())
            throw new EntityException(Strings.EntityClient_ProviderSpecificError((object) nameof (ServerVersion)), ex);
          throw;
        }
      }
    }

    /// <summary>
    /// Gets the provider factory associated with EntityConnection
    /// </summary>
    protected override DbProviderFactory DbProviderFactory
    {
      get
      {
        return (DbProviderFactory) EntityProviderFactory.Instance;
      }
    }

    internal virtual DbProviderFactory StoreProviderFactory
    {
      get
      {
        return this._providerFactory;
      }
    }

    /// <summary>
    /// Provides access to the underlying data source connection that is used by the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// object.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Common.DbConnection" /> for the data source connection.
    /// </returns>
    public virtual DbConnection StoreConnection
    {
      get
      {
        return this._storeConnection;
      }
    }

    /// <summary>
    /// Returns the <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> associated with this
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// .
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> associated with this
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// .
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Core.MetadataException">The inline connection string contains an invalid Metadata keyword value.</exception>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public virtual MetadataWorkspace GetMetadataWorkspace()
    {
      if (this._metadataWorkspace != null)
        return this._metadataWorkspace;
      this._metadataWorkspace = MetadataCache.Instance.GetMetadataWorkspace(this._effectiveConnectionOptions);
      this._initialized = true;
      return this._metadataWorkspace;
    }

    /// <summary>
    /// Gets the current transaction that this connection is enlisted in. May be null.
    /// </summary>
    public virtual EntityTransaction CurrentTransaction
    {
      get
      {
        if (this._currentTransaction != null && (DbInterception.Dispatch.Transaction.GetConnection(this._currentTransaction.StoreTransaction, this.InterceptionContext) == null || this.State == ConnectionState.Closed))
          this.ClearCurrentTransaction();
        return this._currentTransaction;
      }
    }

    internal virtual bool EnlistedInUserTransaction
    {
      get
      {
        try
        {
          return this._enlistedTransaction != (Transaction) null && this._enlistedTransaction.TransactionInformation.Status == TransactionStatus.Active;
        }
        catch (ObjectDisposedException ex)
        {
          this._enlistedTransaction = (Transaction) null;
          return false;
        }
      }
    }

    /// <summary>Establishes a connection to the data source by calling the underlying data provider's Open method.</summary>
    /// <exception cref="T:System.InvalidOperationException">An error occurs when you open the connection, or the name of the underlying data provider is not known.</exception>
    /// <exception cref="T:System.Data.Entity.Core.MetadataException">The inline connection string contains an invalid Metadata keyword value.</exception>
    public override void Open()
    {
      this._fakeConnectionState = new ConnectionState?();
      if (!DbInterception.Dispatch.CancelableEntityConnection.Opening(this, this.InterceptionContext))
      {
        this._fakeConnectionState = new ConnectionState?(ConnectionState.Open);
      }
      else
      {
        if (this._storeConnection == null)
          throw Error.EntityClient_ConnectionStringNeededBeforeOperation();
        if (this.State == ConnectionState.Broken)
          throw Error.EntityClient_CannotOpenBrokenConnection();
        if (DbInterception.Dispatch.Connection.GetState(this._storeConnection, this.InterceptionContext) != ConnectionState.Open)
        {
          MetadataWorkspace metadataWorkspace = this.GetMetadataWorkspace();
          try
          {
            DbProviderServices.GetExecutionStrategy(this._storeConnection, metadataWorkspace).Execute((Action) (() => DbInterception.Dispatch.Connection.Open(this._storeConnection, this.InterceptionContext)));
          }
          catch (Exception ex)
          {
            if (ex.IsCatchableExceptionType())
              throw new EntityException(Strings.EntityClient_ProviderSpecificError((object) nameof (Open)), ex);
            throw;
          }
          this.ClearTransactions();
        }
        if (this._storeConnection == null || DbInterception.Dispatch.Connection.GetState(this._storeConnection, this.InterceptionContext) != ConnectionState.Open)
          throw Error.EntityClient_ConnectionNotOpen();
      }
    }

    /// <summary>
    /// Asynchronously establishes a connection to the data store by calling the Open method on the underlying data provider
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public override async Task OpenAsync(CancellationToken cancellationToken)
    {
      if (this._storeConnection == null)
        throw Error.EntityClient_ConnectionStringNeededBeforeOperation();
      if (this.State == ConnectionState.Broken)
        throw Error.EntityClient_CannotOpenBrokenConnection();
      cancellationToken.ThrowIfCancellationRequested();
      if (DbInterception.Dispatch.Connection.GetState(this._storeConnection, this.InterceptionContext) != ConnectionState.Open)
      {
        MetadataWorkspace metadataWorkspace = this.GetMetadataWorkspace();
        try
        {
          IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this._storeConnection, metadataWorkspace);
          await executionStrategy.ExecuteAsync((Func<Task>) (() => DbInterception.Dispatch.Connection.OpenAsync(this._storeConnection, this.InterceptionContext, cancellationToken)), cancellationToken).WithCurrentCulture();
        }
        catch (Exception ex)
        {
          if (ex.IsCatchableExceptionType())
            throw new EntityException(Strings.EntityClient_ProviderSpecificError((object) "Open"), ex);
          throw;
        }
        this.ClearTransactions();
      }
      if (this._storeConnection == null || DbInterception.Dispatch.Connection.GetState(this._storeConnection, this.InterceptionContext) != ConnectionState.Open)
        throw Error.EntityClient_ConnectionNotOpen();
    }

    /// <summary>
    /// Creates a new instance of an <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" />, with the
    /// <see cref="P:System.Data.Entity.Core.EntityClient.EntityCommand.Connection" />
    /// set to this
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// .
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" /> object.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The name of the underlying data provider is not known.</exception>
    public virtual EntityCommand CreateCommand()
    {
      return new EntityCommand((string) null, this);
    }

    /// <summary>
    /// Create a new command object that uses this connection object
    /// </summary>
    /// <returns>The command object.</returns>
    protected override DbCommand CreateDbCommand()
    {
      return (DbCommand) this.CreateCommand();
    }

    /// <summary>Closes the connection to the database.</summary>
    /// <exception cref="T:System.InvalidOperationException">An error occurred when closing the connection.</exception>
    public override void Close()
    {
      this._fakeConnectionState = new ConnectionState?();
      if (this._storeConnection == null)
        return;
      this.StoreCloseHelper();
    }

    /// <summary>Not supported.</summary>
    /// <param name="databaseName">Not supported. </param>
    /// <exception cref="T:System.NotSupportedException">When the method is called. </exception>
    public override void ChangeDatabase(string databaseName)
    {
      throw new NotSupportedException();
    }

    /// <summary>Begins a transaction by using the underlying provider. </summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.EntityClient.EntityTransaction" />. The returned
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityTransaction" />
    /// instance can later be associated with the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" />
    /// to execute the command under that transaction.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// The underlying provider is not known.-or-The call to
    /// <see cref="M:System.Data.Entity.Core.EntityClient.EntityConnection.BeginTransaction" />
    /// was made on an
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// that already has a current transaction.-or-The state of the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// is not
    /// <see cref="F:System.Data.ConnectionState.Open" />
    /// .
    /// </exception>
    public virtual EntityTransaction BeginTransaction()
    {
      return base.BeginTransaction() as EntityTransaction;
    }

    /// <summary>Begins a transaction with the specified isolation level by using the underlying provider. </summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.EntityClient.EntityTransaction" />. The returned
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityTransaction" />
    /// instance can later be associated with the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" />
    /// to execute the command under that transaction.
    /// </returns>
    /// <param name="isolationLevel">The isolation level of the transaction.</param>
    /// <exception cref="T:System.InvalidOperationException">
    /// The underlying provider is not known.-or-The call to
    /// <see cref="M:System.Data.Entity.Core.EntityClient.EntityConnection.BeginTransaction" />
    /// was made on an
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// that already has a current transaction.-or-The state of the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// is not
    /// <see cref="F:System.Data.ConnectionState.Open" />
    /// .
    /// </exception>
    public virtual EntityTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
      return base.BeginTransaction(isolationLevel) as EntityTransaction;
    }

    /// <summary>Begins a database transaction</summary>
    /// <param name="isolationLevel"> The isolation level of the transaction </param>
    /// <returns> An object representing the new transaction </returns>
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
      if (this._fakeConnectionState.HasValue)
        return (DbTransaction) new EntityTransaction();
      if (this.CurrentTransaction != null)
        throw new InvalidOperationException(Strings.EntityClient_TransactionAlreadyStarted);
      if (this._storeConnection == null)
        throw Error.EntityClient_ConnectionStringNeededBeforeOperation();
      if (this.State != ConnectionState.Open)
        throw Error.EntityClient_ConnectionNotOpen();
      BeginTransactionInterceptionContext interceptionContext = new BeginTransactionInterceptionContext(this.InterceptionContext);
      if (isolationLevel != IsolationLevel.Unspecified)
        interceptionContext = interceptionContext.WithIsolationLevel(isolationLevel);
      DbTransaction storeTransaction;
      try
      {
        storeTransaction = DbProviderServices.GetExecutionStrategy(this._storeConnection, this.GetMetadataWorkspace()).Execute<DbTransaction>((Func<DbTransaction>) (() =>
        {
          if (DbInterception.Dispatch.Connection.GetState(this._storeConnection, this.InterceptionContext) == ConnectionState.Broken)
            DbInterception.Dispatch.Connection.Close(this._storeConnection, (DbInterceptionContext) interceptionContext);
          if (DbInterception.Dispatch.Connection.GetState(this._storeConnection, this.InterceptionContext) == ConnectionState.Closed)
            DbInterception.Dispatch.Connection.Open(this._storeConnection, (DbInterceptionContext) interceptionContext);
          return DbInterception.Dispatch.Connection.BeginTransaction(this._storeConnection, interceptionContext);
        }));
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType())
          throw new EntityException(Strings.EntityClient_ErrorInBeginningTransaction, ex);
        throw;
      }
      if (storeTransaction == null)
        throw new ProviderIncompatibleException(Strings.EntityClient_ReturnedNullOnProviderMethod((object) "BeginTransaction", (object) this._storeConnection.GetType().Name));
      this._currentTransaction = new EntityTransaction(this, storeTransaction);
      return (DbTransaction) this._currentTransaction;
    }

    internal virtual EntityTransaction UseStoreTransaction(
      DbTransaction storeTransaction)
    {
      if (storeTransaction == null)
      {
        this.ClearCurrentTransaction();
      }
      else
      {
        if (this.CurrentTransaction != null)
          throw new InvalidOperationException(Strings.DbContext_TransactionAlreadyStarted);
        if (this.EnlistedInUserTransaction)
          throw new InvalidOperationException(Strings.DbContext_TransactionAlreadyEnlistedInUserTransaction);
        DbConnection connection = DbInterception.Dispatch.Transaction.GetConnection(storeTransaction, this.InterceptionContext);
        if (connection == null)
          throw new InvalidOperationException(Strings.DbContext_InvalidTransactionNoConnection);
        if (connection != this.StoreConnection)
          throw new InvalidOperationException(Strings.DbContext_InvalidTransactionForConnection);
        this._currentTransaction = new EntityTransaction(this, storeTransaction);
      }
      return this._currentTransaction;
    }

    /// <summary>
    /// Enlists this <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> in the specified transaction.
    /// </summary>
    /// <param name="transaction">The transaction object to enlist into.</param>
    /// <exception cref="T:System.InvalidOperationException">
    /// The state of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> is not
    /// <see cref="F:System.Data.ConnectionState.Open" />
    /// .
    /// </exception>
    public override void EnlistTransaction(Transaction transaction)
    {
      if (this._storeConnection == null)
        throw Error.EntityClient_ConnectionStringNeededBeforeOperation();
      if (this.State != ConnectionState.Open)
        throw Error.EntityClient_ConnectionNotOpen();
      try
      {
        DbInterception.Dispatch.Connection.EnlistTransaction(this._storeConnection, new EnlistTransactionInterceptionContext(this.InterceptionContext).WithTransaction(transaction));
        if (transaction != (Transaction) null && !this.EnlistedInUserTransaction)
          transaction.TransactionCompleted += new TransactionCompletedEventHandler(this.EnlistedTransactionCompleted);
        this._enlistedTransaction = transaction;
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType())
          throw new EntityException(Strings.EntityClient_ProviderSpecificError((object) nameof (EnlistTransaction)), ex);
        throw;
      }
    }

    /// <summary>Cleans up this connection object</summary>
    /// <param name="disposing"> true to release both managed and unmanaged resources; false to release only unmanaged resources </param>
    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_currentTransaction")]
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.ClearTransactions();
        if (this._storeConnection != null)
        {
          if (this._entityConnectionOwnsStoreConnection)
            this.StoreCloseHelper();
          this.UnsubscribeFromStoreConnectionStateChangeEvents();
          if (this._entityConnectionOwnsStoreConnection)
            DbInterception.Dispatch.Connection.Dispose(this._storeConnection, this.InterceptionContext);
          this._storeConnection = (DbConnection) null;
        }
        this._entityClientConnectionState = ConnectionState.Closed;
        this.ChangeConnectionString(string.Empty);
      }
      base.Dispose(disposing);
    }

    internal virtual void ClearCurrentTransaction()
    {
      this._currentTransaction = (EntityTransaction) null;
    }

    private void ChangeConnectionString(string newConnectionString)
    {
      System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions connectionOptions = EntityConnection._emptyConnectionOptions;
      if (!string.IsNullOrEmpty(newConnectionString))
        connectionOptions = new System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions(newConnectionString, (IList<string>) EntityConnectionStringBuilder.ValidKeywords);
      DbProviderFactory factory = (DbProviderFactory) null;
      DbConnection connection = (DbConnection) null;
      System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions effectiveConnectionOptions = connectionOptions;
      if (!connectionOptions.IsEmpty)
      {
        string index = connectionOptions["name"];
        if (!string.IsNullOrEmpty(index))
        {
          if (1 < connectionOptions.Parsetable.Count)
            throw new ArgumentException(Strings.EntityClient_ExtraParametersWithNamedConnection);
          ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[index];
          if (connectionString == null || connectionString.ProviderName != "System.Data.EntityClient")
            throw new ArgumentException(Strings.EntityClient_InvalidNamedConnection);
          effectiveConnectionOptions = new System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions(connectionString.ConnectionString, (IList<string>) EntityConnectionStringBuilder.ValidKeywords);
          if (!string.IsNullOrEmpty(effectiveConnectionOptions["name"]))
            throw new ArgumentException(Strings.EntityClient_NestedNamedConnection((object) index));
        }
        EntityConnection.ValidateValueForTheKeyword(effectiveConnectionOptions, "metadata");
        factory = DbConfiguration.DependencyResolver.GetService<DbProviderFactory>((object) EntityConnection.ValidateValueForTheKeyword(effectiveConnectionOptions, "provider"));
        connection = EntityConnection.GetStoreConnection(factory);
        try
        {
          string str = effectiveConnectionOptions["provider connection string"];
          if (str != null)
            DbInterception.Dispatch.Connection.SetConnectionString(connection, new DbConnectionPropertyInterceptionContext<string>(this.InterceptionContext).WithValue(str));
        }
        catch (Exception ex)
        {
          if (ex.IsCatchableExceptionType())
            throw new EntityException(Strings.EntityClient_ProviderSpecificError((object) "ConnectionString"), ex);
          throw;
        }
      }
      lock (this._connectionStringLock)
      {
        this._providerFactory = factory;
        this._metadataWorkspace = (MetadataWorkspace) null;
        this.ClearTransactions();
        this.UnsubscribeFromStoreConnectionStateChangeEvents();
        this._storeConnection = connection;
        this.SubscribeToStoreConnectionStateChangeEvents();
        this._userConnectionOptions = connectionOptions;
        this._effectiveConnectionOptions = effectiveConnectionOptions;
      }
    }

    private static string ValidateValueForTheKeyword(
      System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions effectiveConnectionOptions,
      string keywordName)
    {
      string str = effectiveConnectionOptions[keywordName];
      if (!string.IsNullOrEmpty(str))
        str = str.Trim();
      if (string.IsNullOrEmpty(str))
        throw new ArgumentException(Strings.EntityClient_ConnectionStringMissingInfo((object) keywordName));
      return str;
    }

    private void ClearTransactions()
    {
      this.ClearCurrentTransaction();
      this.ClearEnlistedTransaction();
    }

    private void ClearEnlistedTransaction()
    {
      if (this.EnlistedInUserTransaction)
        this._enlistedTransaction.TransactionCompleted -= new TransactionCompletedEventHandler(this.EnlistedTransactionCompleted);
      this._enlistedTransaction = (Transaction) null;
    }

    private void EnlistedTransactionCompleted(object sender, TransactionEventArgs e)
    {
      e.Transaction.TransactionCompleted -= new TransactionCompletedEventHandler(this.EnlistedTransactionCompleted);
    }

    private void StoreCloseHelper()
    {
      try
      {
        if (this._storeConnection != null && DbInterception.Dispatch.Connection.GetState(this._storeConnection, this.InterceptionContext) != ConnectionState.Closed)
          DbInterception.Dispatch.Connection.Close(this._storeConnection, this.InterceptionContext);
        this.ClearTransactions();
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType())
          throw new EntityException(Strings.EntityClient_ErrorInClosingConnection, ex);
        throw;
      }
    }

    private static DbConnection GetStoreConnection(DbProviderFactory factory)
    {
      DbConnection connection = factory.CreateConnection();
      if (connection == null)
        throw new ProviderIncompatibleException(Strings.EntityClient_ReturnedNullOnProviderMethod((object) "CreateConnection", (object) factory.GetType().Name));
      return connection;
    }
  }
}
