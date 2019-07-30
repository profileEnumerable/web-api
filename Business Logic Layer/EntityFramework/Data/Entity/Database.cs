// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Database
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity
{
  /// <summary>
  /// An instance of this class is obtained from an <see cref="T:System.Data.Entity.DbContext" /> object and can be used
  /// to manage the actual database backing a DbContext or connection.
  /// This includes creating, deleting, and checking for the existence of a database.
  /// Note that deletion and checking for existence of a database can be performed using just a
  /// connection (i.e. without a full context) by using the static methods of this class.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "The DbContextTransaction and EntityTransaction should never be disposed by this class")]
  public class Database
  {
    private static readonly Lazy<IDbConnectionFactory> _defaultDefaultConnectionFactory = new Lazy<IDbConnectionFactory>((Func<IDbConnectionFactory>) (() => AppConfig.DefaultInstance.TryGetDefaultConnectionFactory() ?? (IDbConnectionFactory) new SqlConnectionFactory()), true);
    private static volatile Lazy<IDbConnectionFactory> _defaultConnectionFactory = Database._defaultDefaultConnectionFactory;
    private readonly InternalContext _internalContext;
    private EntityTransaction _entityTransaction;
    private DbContextTransaction _dbContextTransaction;

    internal Database(InternalContext internalContext)
    {
      this._internalContext = internalContext;
    }

    /// <summary>
    /// Gets the transaction the underlying store connection is enlisted in.  May be null.
    /// </summary>
    public DbContextTransaction CurrentTransaction
    {
      get
      {
        EntityTransaction currentTransaction = ((EntityConnection) this._internalContext.ObjectContext.Connection).CurrentTransaction;
        if (this._dbContextTransaction == null || this._entityTransaction != currentTransaction)
        {
          this._entityTransaction = currentTransaction;
          this._dbContextTransaction = currentTransaction == null ? (DbContextTransaction) null : new DbContextTransaction(currentTransaction);
        }
        return this._dbContextTransaction;
      }
    }

    /// <summary>
    /// Enables the user to pass in a database transaction created outside of the <see cref="T:System.Data.Entity.Database" /> object
    /// if you want the Entity Framework to execute commands within that external transaction.
    /// Alternatively, pass in null to clear the framework's knowledge of that transaction.
    /// </summary>
    /// <param name="transaction">the external transaction</param>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the transaction is already completed</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// Thrown if the connection associated with the <see cref="T:System.Data.Entity.Database" /> object is already enlisted in a
    /// <see cref="T:System.Transactions.TransactionScope" />
    /// transaction
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// Thrown if the connection associated with the <see cref="T:System.Data.Entity.Database" /> object is already participating in a transaction
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the connection associated with the transaction does not match the Entity Framework's connection</exception>
    public void UseTransaction(DbTransaction transaction)
    {
      this._entityTransaction = ((EntityConnection) this._internalContext.GetObjectContextWithoutDatabaseInitialization().Connection).UseStoreTransaction(transaction);
      this._dbContextTransaction = (DbContextTransaction) null;
    }

    /// <summary>
    /// Begins a transaction on the underlying store connection
    /// </summary>
    /// <returns>
    /// a <see cref="T:System.Data.Entity.DbContextTransaction" /> object wrapping access to the underlying store's transaction object
    /// </returns>
    public DbContextTransaction BeginTransaction()
    {
      EntityConnection connection = (EntityConnection) this._internalContext.ObjectContext.Connection;
      this._dbContextTransaction = new DbContextTransaction(connection);
      this._entityTransaction = connection.CurrentTransaction;
      return this._dbContextTransaction;
    }

    /// <summary>
    /// Begins a transaction on the underlying store connection using the specified isolation level
    /// </summary>
    /// <param name="isolationLevel">The database isolation level with which the underlying store transaction will be created</param>
    /// <returns>
    /// a <see cref="T:System.Data.Entity.DbContextTransaction" /> object wrapping access to the underlying store's transaction object
    /// </returns>
    public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
      EntityConnection connection = (EntityConnection) this._internalContext.ObjectContext.Connection;
      this._dbContextTransaction = new DbContextTransaction(connection, isolationLevel);
      this._entityTransaction = connection.CurrentTransaction;
      return this._dbContextTransaction;
    }

    /// <summary>
    /// Returns the connection being used by this context.  This may cause the
    /// connection to be created if it does not already exist.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
    public DbConnection Connection
    {
      get
      {
        return this._internalContext.Connection;
      }
    }

    /// <summary>
    /// Sets the database initializer to use for the given context type.  The database initializer is called when a
    /// the given <see cref="T:System.Data.Entity.DbContext" /> type is used to access a database for the first time.
    /// The default strategy for Code First contexts is an instance of <see cref="T:System.Data.Entity.CreateDatabaseIfNotExists`1" />.
    /// </summary>
    /// <typeparam name="TContext"> The type of the context. </typeparam>
    /// <param name="strategy"> The initializer to use, or null to disable initialization for the given context type. </param>
    public static void SetInitializer<TContext>(IDatabaseInitializer<TContext> strategy) where TContext : DbContext
    {
      DbConfigurationManager.Instance.EnsureLoadedForContext(typeof (TContext));
      InternalConfiguration.Instance.RootResolver.DatabaseInitializerResolver.SetInitializer(typeof (TContext), (object) (strategy ?? (IDatabaseInitializer<TContext>) new NullDatabaseInitializer<TContext>()));
    }

    /// <summary>
    /// Runs the the registered <see cref="T:System.Data.Entity.IDatabaseInitializer`1" /> on this context.
    /// If "force" is set to true, then the initializer is run regardless of whether or not it
    /// has been run before.  This can be useful if a database is deleted while an app is running
    /// and needs to be reinitialized.
    /// If "force" is set to false, then the initializer is only run if it has not already been
    /// run for this context, model, and connection in this app domain. This method is typically
    /// used when it is necessary to ensure that the database has been created and seeded
    /// before starting some operation where doing so lazily will cause issues, such as when the
    /// operation is part of a transaction.
    /// </summary>
    /// <param name="force">
    /// If set to <c>true</c> the initializer is run even if it has already been run.
    /// </param>
    public void Initialize(bool force)
    {
      if (force)
      {
        this._internalContext.MarkDatabaseInitialized();
        this._internalContext.PerformDatabaseInitialization();
      }
      else
        this._internalContext.Initialize();
    }

    /// <summary>
    /// Checks whether or not the database is compatible with the the current Code First model.
    /// </summary>
    /// <remarks>
    /// Model compatibility currently uses the following rules.
    /// If the context was created using either the Model First or Database First approach then the
    /// model is assumed to be compatible with the database and this method returns true.
    /// For Code First the model is considered compatible if the model is stored in the database
    /// in the Migrations history table and that model has no differences from the current model as
    /// determined by Migrations model differ.
    /// If the model is not stored in the database but an EF 4.1/4.2 model hash is found instead,
    /// then this is used to check for compatibility.
    /// </remarks>
    /// <param name="throwIfNoMetadata">
    /// If set to <c>true</c> then an exception will be thrown if no model metadata is found in the database. If set to <c>false</c> then this method will return <c>true</c> if metadata is not found.
    /// </param>
    /// <returns> True if the model hash in the context and the database match; false otherwise. </returns>
    public bool CompatibleWithModel(bool throwIfNoMetadata)
    {
      return this.CompatibleWithModel(throwIfNoMetadata, DatabaseExistenceState.Unknown);
    }

    internal bool CompatibleWithModel(bool throwIfNoMetadata, DatabaseExistenceState existenceState)
    {
      return this._internalContext.CompatibleWithModel(throwIfNoMetadata, existenceState);
    }

    /// <summary>
    /// Creates a new database on the database server for the model defined in the backing context.
    /// Note that calling this method before the database initialization strategy has run will disable
    /// executing that strategy.
    /// </summary>
    public void Create()
    {
      this.Create(DatabaseExistenceState.Unknown);
    }

    internal void Create(DatabaseExistenceState existenceState)
    {
      if (existenceState == DatabaseExistenceState.Unknown)
      {
        if (this._internalContext.DatabaseOperations.Exists(this._internalContext.Connection, this._internalContext.CommandTimeout, new Lazy<StoreItemCollection>(new Func<StoreItemCollection>(this.CreateStoreItemCollection))))
          throw Error.Database_DatabaseAlreadyExists((object) DbInterception.Dispatch.Connection.GetDatabase(this._internalContext.Connection, new DbInterceptionContext().WithDbContext(this._internalContext.Owner)));
        existenceState = DatabaseExistenceState.DoesNotExist;
      }
      using (ClonedObjectContext contextForDdlOps = this._internalContext.CreateObjectContextForDdlOps())
        this._internalContext.CreateDatabase((ObjectContext) contextForDdlOps.ObjectContext, existenceState);
    }

    /// <summary>
    /// Creates a new database on the database server for the model defined in the backing context, but only
    /// if a database with the same name does not already exist on the server.
    /// </summary>
    /// <returns> True if the database did not exist and was created; false otherwise. </returns>
    public bool CreateIfNotExists()
    {
      if (this._internalContext.DatabaseOperations.Exists(this._internalContext.Connection, this._internalContext.CommandTimeout, new Lazy<StoreItemCollection>(new Func<StoreItemCollection>(this.CreateStoreItemCollection))))
        return false;
      using (ClonedObjectContext contextForDdlOps = this._internalContext.CreateObjectContextForDdlOps())
        this._internalContext.CreateDatabase((ObjectContext) contextForDdlOps.ObjectContext, DatabaseExistenceState.DoesNotExist);
      return true;
    }

    /// <summary>
    /// Checks whether or not the database exists on the server.
    /// </summary>
    /// <returns> True if the database exists; false otherwise. </returns>
    public bool Exists()
    {
      return this._internalContext.DatabaseOperations.Exists(this._internalContext.Connection, this._internalContext.CommandTimeout, new Lazy<StoreItemCollection>(new Func<StoreItemCollection>(this.CreateStoreItemCollection)));
    }

    /// <summary>
    /// Deletes the database on the database server if it exists, otherwise does nothing.
    /// Calling this method from outside of an initializer will mark the database as having
    /// not been initialized. This means that if an attempt is made to use the database again
    /// after it has been deleted, then any initializer set will run again and, usually, will
    /// try to create the database again automatically.
    /// </summary>
    /// <returns> True if the database did exist and was deleted; false otherwise. </returns>
    public bool Delete()
    {
      if (!this._internalContext.DatabaseOperations.Exists(this._internalContext.Connection, this._internalContext.CommandTimeout, new Lazy<StoreItemCollection>(new Func<StoreItemCollection>(this.CreateStoreItemCollection))))
        return false;
      using (ClonedObjectContext contextForDdlOps = this._internalContext.CreateObjectContextForDdlOps())
      {
        this._internalContext.DatabaseOperations.Delete((ObjectContext) contextForDdlOps.ObjectContext);
        this._internalContext.MarkDatabaseNotInitialized();
      }
      return true;
    }

    /// <summary>
    /// Checks whether or not the database exists on the server.
    /// The connection to the database is created using the given database name or connection string
    /// in the same way as is described in the documentation for the <see cref="T:System.Data.Entity.DbContext" /> class.
    /// </summary>
    /// <param name="nameOrConnectionString"> The database name or a connection string to the database. </param>
    /// <returns> True if the database exists; false otherwise. </returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public static bool Exists(string nameOrConnectionString)
    {
      Check.NotEmpty(nameOrConnectionString, nameof (nameOrConnectionString));
      using (LazyInternalConnection internalConnection = new LazyInternalConnection(nameOrConnectionString))
        return new DatabaseOperations().Exists(internalConnection.Connection, new int?(), new Lazy<StoreItemCollection>((Func<StoreItemCollection>) (() => new StoreItemCollection())));
    }

    /// <summary>
    /// Deletes the database on the database server if it exists, otherwise does nothing.
    /// The connection to the database is created using the given database name or connection string
    /// in the same way as is described in the documentation for the <see cref="T:System.Data.Entity.DbContext" /> class.
    /// </summary>
    /// <param name="nameOrConnectionString"> The database name or a connection string to the database. </param>
    /// <returns> True if the database did exist and was deleted; false otherwise. </returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public static bool Delete(string nameOrConnectionString)
    {
      Check.NotEmpty(nameOrConnectionString, nameof (nameOrConnectionString));
      if (!Database.Exists(nameOrConnectionString))
        return false;
      using (LazyInternalConnection internalConnection = new LazyInternalConnection(nameOrConnectionString))
      {
        using (ObjectContext emptyObjectContext = Database.CreateEmptyObjectContext(internalConnection.Connection))
          new DatabaseOperations().Delete(emptyObjectContext);
      }
      return true;
    }

    /// <summary>
    /// Checks whether or not the database exists on the server.
    /// </summary>
    /// <param name="existingConnection"> An existing connection to the database. </param>
    /// <returns> True if the database exists; false otherwise. </returns>
    public static bool Exists(DbConnection existingConnection)
    {
      Check.NotNull<DbConnection>(existingConnection, nameof (existingConnection));
      return new DatabaseOperations().Exists(existingConnection, new int?(), new Lazy<StoreItemCollection>((Func<StoreItemCollection>) (() => new StoreItemCollection())));
    }

    /// <summary>
    /// Deletes the database on the database server if it exists, otherwise does nothing.
    /// </summary>
    /// <param name="existingConnection"> An existing connection to the database. </param>
    /// <returns> True if the database did exist and was deleted; false otherwise. </returns>
    public static bool Delete(DbConnection existingConnection)
    {
      Check.NotNull<DbConnection>(existingConnection, nameof (existingConnection));
      if (!Database.Exists(existingConnection))
        return false;
      using (ObjectContext emptyObjectContext = Database.CreateEmptyObjectContext(existingConnection))
        new DatabaseOperations().Delete(emptyObjectContext);
      return true;
    }

    /// <summary>
    /// The connection factory to use when creating a <see cref="T:System.Data.Common.DbConnection" /> from just
    /// a database name or a connection string.
    /// </summary>
    /// <remarks>
    /// This is used when just a database name or connection string is given to <see cref="T:System.Data.Entity.DbContext" /> or when
    /// the no database name or connection is given to DbContext in which case the name of
    /// the context class is passed to this factory in order to generate a DbConnection.
    /// By default, the <see cref="T:System.Data.Entity.Infrastructure.IDbConnectionFactory" /> instance to use is read from the application's .config
    /// file from the "EntityFramework DefaultConnectionFactory" entry in appSettings. If no entry is found in
    /// the config file then <see cref="T:System.Data.Entity.Infrastructure.SqlConnectionFactory" /> is used. Setting this property in code
    /// always overrides whatever value is found in the config file.
    /// </remarks>
    [Obsolete("The default connection factory should be set in the config file or using the DbConfiguration class. (See http://go.microsoft.com/fwlink/?LinkId=260883)")]
    public static IDbConnectionFactory DefaultConnectionFactory
    {
      get
      {
        return DbConfiguration.DependencyResolver.GetService<IDbConnectionFactory>();
      }
      set
      {
        Check.NotNull<IDbConnectionFactory>(value, nameof (value));
        Database._defaultConnectionFactory = new Lazy<IDbConnectionFactory>((Func<IDbConnectionFactory>) (() => value), true);
      }
    }

    internal static IDbConnectionFactory SetDefaultConnectionFactory
    {
      get
      {
        return Database._defaultConnectionFactory.Value;
      }
    }

    internal static bool DefaultConnectionFactoryChanged
    {
      get
      {
        return !object.ReferenceEquals((object) Database._defaultConnectionFactory, (object) Database._defaultDefaultConnectionFactory);
      }
    }

    internal static void ResetDefaultConnectionFactory()
    {
      Database._defaultConnectionFactory = Database._defaultDefaultConnectionFactory;
    }

    private static ObjectContext CreateEmptyObjectContext(DbConnection connection)
    {
      return new DbModelBuilder().Build(connection).Compile().CreateObjectContext<ObjectContext>(connection);
    }

    /// <summary>
    /// Creates a raw SQL query that will return elements of the given generic type.
    /// The type can be any type that has properties that match the names of the columns returned
    /// from the query, or can be a simple primitive type.  The type does not have to be an
    /// entity type. The results of this query are never tracked by the context even if the
    /// type of object returned is an entity type.  Use the <see cref="M:System.Data.Entity.DbSet`1.SqlQuery(System.String,System.Object[])" />
    /// method to return entities that are tracked by the context.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.Database.SqlQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.Database.SqlQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <typeparam name="TElement"> The type of object returned by the query. </typeparam>
    /// <param name="sql"> The SQL query string. </param>
    /// <param name="parameters">
    /// The parameters to apply to the SQL query string. If output parameters are used, their values will
    /// not be available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Infrastructure.DbRawSqlQuery`1" /> object that will execute the query when it is enumerated.
    /// </returns>
    public DbRawSqlQuery<TElement> SqlQuery<TElement>(
      string sql,
      params object[] parameters)
    {
      Check.NotEmpty(sql, nameof (sql));
      Check.NotNull<object[]>(parameters, nameof (parameters));
      return new DbRawSqlQuery<TElement>((InternalSqlQuery) new InternalSqlNonSetQuery(this._internalContext, typeof (TElement), sql, parameters));
    }

    /// <summary>
    /// Creates a raw SQL query that will return elements of the given type.
    /// The type can be any type that has properties that match the names of the columns returned
    /// from the query, or can be a simple primitive type.  The type does not have to be an
    /// entity type. The results of this query are never tracked by the context even if the
    /// type of object returned is an entity type.  Use the <see cref="M:System.Data.Entity.DbSet.SqlQuery(System.String,System.Object[])" />
    /// method to return entities that are tracked by the context.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.Database.SqlQuery(typeof(Post), "SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.Database.SqlQuery(typeof(Post), "SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <param name="elementType"> The type of object returned by the query. </param>
    /// <param name="sql"> The SQL query string. </param>
    /// <param name="parameters">
    /// The parameters to apply to the SQL query string. If output parameters are used, their values
    /// will not be available until the results have been read completely. This is due to the underlying
    /// behavior of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Infrastructure.DbRawSqlQuery" /> object that will execute the query when it is enumerated.
    /// </returns>
    public DbRawSqlQuery SqlQuery(
      Type elementType,
      string sql,
      params object[] parameters)
    {
      Check.NotNull<Type>(elementType, nameof (elementType));
      Check.NotEmpty(sql, nameof (sql));
      Check.NotNull<object[]>(parameters, nameof (parameters));
      return new DbRawSqlQuery((InternalSqlQuery) new InternalSqlNonSetQuery(this._internalContext, elementType, sql, parameters));
    }

    /// <summary>
    /// Executes the given DDL/DML command against the database.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// If there isn't an existing local or ambient transaction a new transaction will be used
    /// to execute the command.
    /// </remarks>
    /// <param name="sql"> The command string. </param>
    /// <param name="parameters"> The parameters to apply to the command string. </param>
    /// <returns> The result returned by the database after executing the command. </returns>
    public int ExecuteSqlCommand(string sql, params object[] parameters)
    {
      return this.ExecuteSqlCommand(this._internalContext.EnsureTransactionsForFunctionsAndCommands ? TransactionalBehavior.EnsureTransaction : TransactionalBehavior.DoNotEnsureTransaction, sql, parameters);
    }

    /// <summary>
    /// Executes the given DDL/DML command against the database.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <param name="transactionalBehavior"> Controls the creation of a transaction for this command. </param>
    /// <param name="sql"> The command string. </param>
    /// <param name="parameters"> The parameters to apply to the command string. </param>
    /// <returns> The result returned by the database after executing the command. </returns>
    public int ExecuteSqlCommand(
      TransactionalBehavior transactionalBehavior,
      string sql,
      params object[] parameters)
    {
      Check.NotEmpty(sql, nameof (sql));
      Check.NotNull<object[]>(parameters, nameof (parameters));
      return this._internalContext.ExecuteSqlCommand(transactionalBehavior, sql, parameters);
    }

    /// <summary>
    /// Asynchronously executes the given DDL/DML command against the database.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// 
    /// If there isn't an existing local transaction a new transaction will be used
    /// to execute the command.
    /// </remarks>
    /// <param name="sql"> The command string. </param>
    /// <param name="parameters"> The parameters to apply to the command string. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the database after executing the command.
    /// </returns>
    public Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
    {
      return this.ExecuteSqlCommandAsync(sql, CancellationToken.None, parameters);
    }

    /// <summary>
    /// Asynchronously executes the given DDL/DML command against the database.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="transactionalBehavior"> Controls the creation of a transaction for this command. </param>
    /// <param name="sql"> The command string. </param>
    /// <param name="parameters"> The parameters to apply to the command string. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the database after executing the command.
    /// </returns>
    public Task<int> ExecuteSqlCommandAsync(
      TransactionalBehavior transactionalBehavior,
      string sql,
      params object[] parameters)
    {
      return this.ExecuteSqlCommandAsync(transactionalBehavior, sql, CancellationToken.None, parameters);
    }

    /// <summary>
    /// Asynchronously executes the given DDL/DML command against the database.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// 
    /// If there isn't an existing local transaction a new transaction will be used
    /// to execute the command.
    /// </remarks>
    /// <param name="sql"> The command string. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="parameters"> The parameters to apply to the command string. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the database after executing the command.
    /// </returns>
    public Task<int> ExecuteSqlCommandAsync(
      string sql,
      CancellationToken cancellationToken,
      params object[] parameters)
    {
      return this.ExecuteSqlCommandAsync(this._internalContext.EnsureTransactionsForFunctionsAndCommands ? TransactionalBehavior.EnsureTransaction : TransactionalBehavior.DoNotEnsureTransaction, sql, cancellationToken, parameters);
    }

    /// <summary>
    /// Asynchronously executes the given DDL/DML command against the database.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="transactionalBehavior"> Controls the creation of a transaction for this command. </param>
    /// <param name="sql"> The command string. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="parameters"> The parameters to apply to the command string. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the result returned by the database after executing the command.
    /// </returns>
    public Task<int> ExecuteSqlCommandAsync(
      TransactionalBehavior transactionalBehavior,
      string sql,
      CancellationToken cancellationToken,
      params object[] parameters)
    {
      Check.NotEmpty(sql, nameof (sql));
      Check.NotNull<object[]>(parameters, nameof (parameters));
      cancellationToken.ThrowIfCancellationRequested();
      return this._internalContext.ExecuteSqlCommandAsync(transactionalBehavior, sql, cancellationToken, parameters);
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

    private StoreItemCollection CreateStoreItemCollection()
    {
      using (ClonedObjectContext contextForDdlOps = this._internalContext.CreateObjectContextForDdlOps())
        return (StoreItemCollection) (EntityConnection) contextForDdlOps.ObjectContext.Connection.GetMetadataWorkspace().GetItemCollection(DataSpace.SSpace);
    }

    /// <summary>
    /// Gets or sets the timeout value, in seconds, for all context operations.
    /// The default value is null, where null indicates that the default value of the underlying
    /// provider will be used.
    /// </summary>
    /// <value>
    /// The timeout, in seconds, or null to use the provider default.
    /// </value>
    public int? CommandTimeout
    {
      get
      {
        return this._internalContext.CommandTimeout;
      }
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          if ((nullable.GetValueOrDefault() >= 0 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
            throw new ArgumentException(Strings.ObjectContext_InvalidCommandTimeout);
        }
        this._internalContext.CommandTimeout = value;
      }
    }

    /// <summary>
    /// Set this property to log the SQL generated by the <see cref="T:System.Data.Entity.DbContext" /> to the given
    /// delegate. For example, to log to the console, set this property to <see cref="M:System.Console.Write(System.String)" />.
    /// </summary>
    /// <remarks>
    /// The format of the log text can be changed by creating a new formatter that derives from
    /// <see cref="T:System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter" /> and setting it with <see cref="M:System.Data.Entity.DbConfiguration.SetDatabaseLogFormatter(System.Func{System.Data.Entity.DbContext,System.Action{System.String},System.Data.Entity.Infrastructure.Interception.DatabaseLogFormatter})" />.
    /// For more low-level control over logging/interception see <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" /> and
    /// <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" />.
    /// </remarks>
    public Action<string> Log
    {
      get
      {
        return this._internalContext.Log;
      }
      set
      {
        this._internalContext.Log = value;
      }
    }
  }
}
