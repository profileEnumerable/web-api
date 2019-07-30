// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.EntityCommand
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.Entity.Core.Common.QueryCache;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.EntityClient.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.EntityClient
{
  /// <summary>Class representing a command for the conceptual layer</summary>
  public class EntityCommand : DbCommand
  {
    private bool _designTimeVisible;
    private string _esqlCommandText;
    private EntityConnection _connection;
    private DbCommandTree _preparedCommandTree;
    private readonly EntityParameterCollection _parameters;
    private int? _commandTimeout;
    private CommandType _commandType;
    private EntityTransaction _transaction;
    private UpdateRowSource _updatedRowSource;
    private EntityCommandDefinition _commandDefinition;
    private bool _isCommandDefinitionBased;
    private DbCommandTree _commandTreeSetByUser;
    private DbDataReader _dataReader;
    private bool _enableQueryPlanCaching;
    private DbCommand _storeProviderCommand;
    private readonly EntityCommand.EntityDataReaderFactory _entityDataReaderFactory;
    private readonly IDbDependencyResolver _dependencyResolver;
    private readonly DbInterceptionContext _interceptionContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" /> class using the specified values.
    /// </summary>
    public EntityCommand()
      : this(new DbInterceptionContext())
    {
    }

    internal EntityCommand(DbInterceptionContext interceptionContext)
      : this(interceptionContext, new EntityCommand.EntityDataReaderFactory())
    {
    }

    internal EntityCommand(
      DbInterceptionContext interceptionContext,
      EntityCommand.EntityDataReaderFactory factory)
    {
      this._designTimeVisible = true;
      this._commandType = CommandType.Text;
      this._updatedRowSource = UpdateRowSource.Both;
      this._parameters = new EntityParameterCollection();
      this._interceptionContext = interceptionContext;
      this._enableQueryPlanCaching = true;
      this._entityDataReaderFactory = factory ?? new EntityCommand.EntityDataReaderFactory();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" /> class with the specified statement.
    /// </summary>
    /// <param name="statement">The text of the command.</param>
    public EntityCommand(string statement)
      : this(statement, new DbInterceptionContext(), new EntityCommand.EntityDataReaderFactory())
    {
    }

    internal EntityCommand(
      string statement,
      DbInterceptionContext context,
      EntityCommand.EntityDataReaderFactory factory)
      : this(context, factory)
    {
      this._esqlCommandText = statement;
    }

    /// <summary>
    /// Constructs the EntityCommand object with the given eSQL statement and the connection object to use
    /// </summary>
    /// <param name="statement"> The eSQL command text to execute </param>
    /// <param name="connection"> The connection object </param>
    /// <param name="resolver"> Resolver used to resolve DbProviderServices </param>
    public EntityCommand(
      string statement,
      EntityConnection connection,
      IDbDependencyResolver resolver)
      : this(statement, connection)
    {
      this._dependencyResolver = resolver;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" /> class with the specified statement and connection.
    /// </summary>
    /// <param name="statement">The text of the command.</param>
    /// <param name="connection">A connection to the data source.</param>
    public EntityCommand(string statement, EntityConnection connection)
      : this(statement, connection, new EntityCommand.EntityDataReaderFactory())
    {
    }

    internal EntityCommand(
      string statement,
      EntityConnection connection,
      EntityCommand.EntityDataReaderFactory factory)
      : this(statement, new DbInterceptionContext(), factory)
    {
      this._connection = connection;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" /> class with the specified statement, connection and transaction.
    /// </summary>
    /// <param name="statement">The text of the command.</param>
    /// <param name="connection">A connection to the data source.</param>
    /// <param name="transaction">The transaction in which the command executes.</param>
    public EntityCommand(
      string statement,
      EntityConnection connection,
      EntityTransaction transaction)
      : this(statement, connection, transaction, new EntityCommand.EntityDataReaderFactory())
    {
    }

    internal EntityCommand(
      string statement,
      EntityConnection connection,
      EntityTransaction transaction,
      EntityCommand.EntityDataReaderFactory factory)
      : this(statement, connection, factory)
    {
      this._transaction = transaction;
    }

    internal EntityCommand(
      EntityCommandDefinition commandDefinition,
      DbInterceptionContext context,
      EntityCommand.EntityDataReaderFactory factory = null)
      : this(context, factory)
    {
      this._commandDefinition = commandDefinition;
      this._parameters = new EntityParameterCollection();
      foreach (EntityParameter parameter in commandDefinition.Parameters)
        this._parameters.Add(parameter.Clone());
      this._parameters.ResetIsDirty();
      this._isCommandDefinitionBased = true;
    }

    internal EntityCommand(
      EntityConnection connection,
      EntityCommandDefinition entityCommandDefinition,
      DbInterceptionContext context,
      EntityCommand.EntityDataReaderFactory factory = null)
      : this(entityCommandDefinition, context, factory)
    {
      this._connection = connection;
    }

    internal virtual DbInterceptionContext InterceptionContext
    {
      get
      {
        return this._interceptionContext;
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> used by the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" />
    /// .
    /// </summary>
    /// <returns>The connection used by the entity command.</returns>
    public virtual EntityConnection Connection
    {
      get
      {
        return this._connection;
      }
      set
      {
        this.ThrowIfDataReaderIsOpen();
        if (this._connection == value)
          return;
        if (this._connection != null)
          this.Unprepare();
        this._connection = value;
        this._transaction = (EntityTransaction) null;
      }
    }

    /// <summary>The connection object used for executing the command</summary>
    protected override DbConnection DbConnection
    {
      get
      {
        return (DbConnection) this.Connection;
      }
      set
      {
        this.Connection = (EntityConnection) value;
      }
    }

    /// <summary>Gets or sets an Entity SQL statement that specifies a command or stored procedure to execute.</summary>
    /// <returns>The Entity SQL statement that specifies a command or stored procedure to execute.</returns>
    public override string CommandText
    {
      get
      {
        if (this._commandTreeSetByUser != null)
          throw new InvalidOperationException(Strings.EntityClient_CannotGetCommandText);
        return this._esqlCommandText ?? "";
      }
      set
      {
        this.ThrowIfDataReaderIsOpen();
        if (this._commandTreeSetByUser != null)
          throw new InvalidOperationException(Strings.EntityClient_CannotSetCommandText);
        if (!(this._esqlCommandText != value))
          return;
        this._esqlCommandText = value;
        this.Unprepare();
        this._isCommandDefinitionBased = false;
      }
    }

    /// <summary>Gets or sets the command tree to execute; only one of the command tree or the command text can be set, not both.</summary>
    /// <returns>The command tree to execute.</returns>
    public virtual DbCommandTree CommandTree
    {
      get
      {
        if (!string.IsNullOrEmpty(this._esqlCommandText))
          throw new InvalidOperationException(Strings.EntityClient_CannotGetCommandTree);
        return this._commandTreeSetByUser;
      }
      set
      {
        this.ThrowIfDataReaderIsOpen();
        if (!string.IsNullOrEmpty(this._esqlCommandText))
          throw new InvalidOperationException(Strings.EntityClient_CannotSetCommandTree);
        if (CommandType.Text != this.CommandType)
          throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1026));
        if (this._commandTreeSetByUser == value)
          return;
        this._commandTreeSetByUser = value;
        this.Unprepare();
        this._isCommandDefinitionBased = false;
      }
    }

    /// <summary>Gets or sets the amount of time to wait before timing out.</summary>
    /// <returns>The time in seconds to wait for the command to execute.</returns>
    public override int CommandTimeout
    {
      get
      {
        if (this._commandTimeout.HasValue)
          return this._commandTimeout.Value;
        if (this._connection != null && this._connection.StoreProviderFactory != null)
        {
          DbCommand command = this._connection.StoreProviderFactory.CreateCommand();
          if (command != null)
            return command.CommandTimeout;
        }
        return 0;
      }
      set
      {
        this.ThrowIfDataReaderIsOpen();
        this._commandTimeout = new int?(value);
      }
    }

    /// <summary>
    /// Gets or sets a value that indicates how the
    /// <see cref="P:System.Data.Entity.Core.EntityClient.EntityCommand.CommandText" />
    /// property is to be interpreted.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Data.CommandType" /> enumeration values.
    /// </returns>
    public override CommandType CommandType
    {
      get
      {
        return this._commandType;
      }
      set
      {
        this.ThrowIfDataReaderIsOpen();
        if (value != CommandType.Text && value != CommandType.StoredProcedure)
          throw new NotSupportedException(Strings.EntityClient_UnsupportedCommandType);
        this._commandType = value;
      }
    }

    /// <summary>Gets the parameters of the Entity SQL statement or stored procedure.</summary>
    /// <returns>The parameters of the Entity SQL statement or stored procedure.</returns>
    public virtual EntityParameterCollection Parameters
    {
      get
      {
        return this._parameters;
      }
    }

    /// <summary>The collection of parameters for this command</summary>
    protected override DbParameterCollection DbParameterCollection
    {
      get
      {
        return (DbParameterCollection) this.Parameters;
      }
    }

    /// <summary>
    /// Gets or sets the transaction within which the <see cref="T:System.Data.SqlClient.SqlCommand" /> executes.
    /// </summary>
    /// <returns>
    /// The transaction within which the <see cref="T:System.Data.SqlClient.SqlCommand" /> executes.
    /// </returns>
    public virtual EntityTransaction Transaction
    {
      get
      {
        return this._transaction;
      }
      set
      {
        this.ThrowIfDataReaderIsOpen();
        this._transaction = value;
      }
    }

    /// <summary>The transaction that this command executes in</summary>
    protected override DbTransaction DbTransaction
    {
      get
      {
        return (DbTransaction) this.Transaction;
      }
      set
      {
        this.Transaction = (EntityTransaction) value;
      }
    }

    /// <summary>Gets or sets how command results are applied to rows being updated.</summary>
    /// <returns>
    /// One of the <see cref="T:System.Data.UpdateRowSource" /> values.
    /// </returns>
    public override UpdateRowSource UpdatedRowSource
    {
      get
      {
        return this._updatedRowSource;
      }
      set
      {
        this.ThrowIfDataReaderIsOpen();
        this._updatedRowSource = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether the command object should be visible in a Windows Form Designer control.</summary>
    /// <returns>true if the command object should be visible in a Windows Form Designer control; otherwise, false.</returns>
    public override bool DesignTimeVisible
    {
      get
      {
        return this._designTimeVisible;
      }
      set
      {
        this.ThrowIfDataReaderIsOpen();
        this._designTimeVisible = value;
        TypeDescriptor.Refresh((object) this);
      }
    }

    /// <summary>Gets or sets a value that indicates whether the query plan caching is enabled.</summary>
    /// <returns>true if the query plan caching is enabled; otherwise, false.</returns>
    public virtual bool EnablePlanCaching
    {
      get
      {
        return this._enableQueryPlanCaching;
      }
      set
      {
        this.ThrowIfDataReaderIsOpen();
        this._enableQueryPlanCaching = value;
      }
    }

    /// <summary>
    /// Cancels the execution of an <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" />.
    /// </summary>
    public override void Cancel()
    {
    }

    /// <summary>
    /// Creates a new instance of an <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object.
    /// </summary>
    /// <returns>
    /// A new instance of an <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> object.
    /// </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public virtual EntityParameter CreateParameter()
    {
      return new EntityParameter();
    }

    /// <summary>
    /// Create and return a new parameter object representing a parameter in the eSQL statement
    /// </summary>
    /// <returns>The parameter object.</returns>
    protected override DbParameter CreateDbParameter()
    {
      return (DbParameter) this.CreateParameter();
    }

    /// <summary>Executes the command and returns a data reader.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" /> that contains the results.
    /// </returns>
    public virtual EntityDataReader ExecuteReader()
    {
      return this.ExecuteReader(CommandBehavior.Default);
    }

    /// <summary>
    /// Compiles the <see cref="P:System.Data.Entity.Core.EntityClient.EntityCommand.CommandText" /> into a command tree and passes it to the underlying store provider for execution, then builds an
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" />
    /// out of the produced result set using the specified
    /// <see cref="T:System.Data.CommandBehavior" />
    /// .
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" /> that contains the results.
    /// </returns>
    /// <param name="behavior">
    /// One of the <see cref="T:System.Data.CommandBehavior" /> values.
    /// </param>
    public virtual EntityDataReader ExecuteReader(CommandBehavior behavior)
    {
      this.Prepare();
      EntityDataReader entityDataReader = this._entityDataReaderFactory.CreateEntityDataReader(this, this._commandDefinition.Execute(this, behavior), behavior);
      this._dataReader = (DbDataReader) entityDataReader;
      return entityDataReader;
    }

    /// <summary>
    /// Asynchronously executes the command and returns a data reader for reading the results. May only
    /// be called on CommandType.CommandText (otherwise, use the standard Execute* methods)
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an EntityDataReader object.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// For stored procedure commands, if called
    /// for anything but an entity collection result
    /// </exception>
    public virtual Task<EntityDataReader> ExecuteReaderAsync()
    {
      return this.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously executes the command and returns a data reader for reading the results. May only
    /// be called on CommandType.CommandText (otherwise, use the standard Execute* methods)
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an EntityDataReader object.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// For stored procedure commands, if called
    /// for anything but an entity collection result
    /// </exception>
    public virtual Task<EntityDataReader> ExecuteReaderAsync(
      CancellationToken cancellationToken)
    {
      return this.ExecuteReaderAsync(CommandBehavior.Default, cancellationToken);
    }

    /// <summary>
    /// Asynchronously executes the command and returns a data reader for reading the results. May only
    /// be called on CommandType.CommandText (otherwise, use the standard Execute* methods)
    /// </summary>
    /// <param name="behavior"> The behavior to use when executing the command </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an EntityDataReader object.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// For stored procedure commands, if called
    /// for anything but an entity collection result
    /// </exception>
    public virtual Task<EntityDataReader> ExecuteReaderAsync(
      CommandBehavior behavior)
    {
      return this.ExecuteReaderAsync(behavior, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously executes the command and returns a data reader for reading the results. May only
    /// be called on CommandType.CommandText (otherwise, use the standard Execute* methods)
    /// </summary>
    /// <param name="behavior"> The behavior to use when executing the command </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an EntityDataReader object.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// For stored procedure commands, if called
    /// for anything but an entity collection result
    /// </exception>
    public virtual async Task<EntityDataReader> ExecuteReaderAsync(
      CommandBehavior behavior,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      this.Prepare();
      DbDataReader dbDataReader = await this._commandDefinition.ExecuteAsync(this, behavior, cancellationToken).WithCurrentCulture<DbDataReader>();
      EntityDataReader reader = this._entityDataReaderFactory.CreateEntityDataReader(this, dbDataReader, behavior);
      this._dataReader = (DbDataReader) reader;
      return reader;
    }

    /// <summary>
    /// Executes the command and returns a data reader for reading the results
    /// </summary>
    /// <param name="behavior"> The behavior to use when executing the command </param>
    /// <returns> A DbDataReader object </returns>
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
      return (DbDataReader) this.ExecuteReader(behavior);
    }

    /// <summary>
    /// Asynchronously executes the command and returns a data reader for reading the results
    /// </summary>
    /// <param name="behavior"> The behavior to use when executing the command </param>
    /// <param name="cancellationToken"> The token to monitor for cancellation requests </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a DbDataReader object.
    /// </returns>
    protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(
      CommandBehavior behavior,
      CancellationToken cancellationToken)
    {
      return (DbDataReader) await this.ExecuteReaderAsync(behavior, cancellationToken).WithCurrentCulture<EntityDataReader>();
    }

    /// <summary>Executes the current command.</summary>
    /// <returns>The number of rows affected.</returns>
    public override int ExecuteNonQuery()
    {
      using (EntityDataReader entityDataReader = this.ExecuteReader(CommandBehavior.SequentialAccess))
      {
        CommandHelper.ConsumeReader((DbDataReader) entityDataReader);
        return entityDataReader.RecordsAffected;
      }
    }

    /// <summary>
    /// Asynchronously executes the command and discard any results returned from the command
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of rows affected.
    /// </returns>
    public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
    {
      int recordsAffected;
      using (EntityDataReader reader = await this.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).WithCurrentCulture<EntityDataReader>())
      {
        await CommandHelper.ConsumeReaderAsync((DbDataReader) reader, cancellationToken).WithCurrentCulture();
        recordsAffected = reader.RecordsAffected;
      }
      return recordsAffected;
    }

    /// <summary>Executes the command, and returns the first column of the first row in the result set. Additional columns or rows are ignored.</summary>
    /// <returns>The first column of the first row in the result set, or a null reference (Nothing in Visual Basic) if the result set is empty.</returns>
    public override object ExecuteScalar()
    {
      using (EntityDataReader entityDataReader = this.ExecuteReader(CommandBehavior.SequentialAccess))
      {
        object obj = entityDataReader.Read() ? entityDataReader.GetValue(0) : (object) null;
        CommandHelper.ConsumeReader((DbDataReader) entityDataReader);
        return obj;
      }
    }

    internal virtual void Unprepare()
    {
      this._commandDefinition = (EntityCommandDefinition) null;
      this._preparedCommandTree = (DbCommandTree) null;
      this._parameters.ResetIsDirty();
    }

    /// <summary>Compiles the entity-level command and creates a prepared version of the command.</summary>
    public override void Prepare()
    {
      this.ThrowIfDataReaderIsOpen();
      this.CheckIfReadyToPrepare();
      this.InnerPrepare();
    }

    private void InnerPrepare()
    {
      if (this._parameters.IsDirty)
        this.Unprepare();
      this._commandDefinition = this.GetCommandDefinition();
    }

    private DbCommandTree MakeCommandTree()
    {
      DbCommandTree dbCommandTree = (DbCommandTree) null;
      if (this._commandTreeSetByUser != null)
        dbCommandTree = this._commandTreeSetByUser;
      else if (CommandType.Text == this.CommandType)
      {
        if (!string.IsNullOrEmpty(this._esqlCommandText))
        {
          dbCommandTree = CqlQuery.Compile(this._esqlCommandText, (Perspective) new ModelPerspective(this._connection.GetMetadataWorkspace()), (ParserOptions) null, this.GetParameterTypeUsage().Select<KeyValuePair<string, TypeUsage>, DbParameterReferenceExpression>((Func<KeyValuePair<string, TypeUsage>, DbParameterReferenceExpression>) (paramInfo => paramInfo.Value.Parameter(paramInfo.Key)))).CommandTree;
        }
        else
        {
          if (this._isCommandDefinitionBased)
            throw new InvalidOperationException(Strings.EntityClient_CannotReprepareCommandDefinitionBasedCommand);
          throw new InvalidOperationException(Strings.EntityClient_NoCommandText);
        }
      }
      else if (CommandType.StoredProcedure == this.CommandType)
        dbCommandTree = (DbCommandTree) new DbFunctionCommandTree(this.Connection.GetMetadataWorkspace(), DataSpace.CSpace, this.DetermineFunctionImport(), (TypeUsage) null, (IEnumerable<KeyValuePair<string, TypeUsage>>) this.GetParameterTypeUsage());
      return dbCommandTree;
    }

    private EdmFunction DetermineFunctionImport()
    {
      if (string.IsNullOrEmpty(this.CommandText) || string.IsNullOrEmpty(this.CommandText.Trim()))
        throw new InvalidOperationException(Strings.EntityClient_FunctionImportEmptyCommandText);
      string containerName;
      string functionImportName;
      CommandHelper.ParseFunctionImportCommandText(this.CommandText, (string) null, out containerName, out functionImportName);
      return CommandHelper.FindFunctionImport(this._connection.GetMetadataWorkspace(), containerName, functionImportName);
    }

    internal virtual EntityCommandDefinition GetCommandDefinition()
    {
      EntityCommandDefinition entityCommandDefinition = this._commandDefinition;
      if (entityCommandDefinition == null)
      {
        if (!this.TryGetEntityCommandDefinitionFromQueryCache(out entityCommandDefinition))
          entityCommandDefinition = this.CreateCommandDefinition();
        this._commandDefinition = entityCommandDefinition;
      }
      return entityCommandDefinition;
    }

    internal virtual EntityTransaction ValidateAndGetEntityTransaction()
    {
      if (this.Transaction != null && this.Transaction != this.Connection.CurrentTransaction)
        throw new InvalidOperationException(Strings.EntityClient_InvalidTransactionForCommand);
      return this.Connection.CurrentTransaction;
    }

    /// <summary>Compiles the entity-level command and returns the store command text.</summary>
    /// <returns>The store command text.</returns>
    [Browsable(false)]
    public virtual string ToTraceString()
    {
      this.CheckConnectionPresent();
      this.InnerPrepare();
      EntityCommandDefinition commandDefinition = this._commandDefinition;
      if (commandDefinition != null)
        return commandDefinition.ToTraceString();
      return string.Empty;
    }

    private bool TryGetEntityCommandDefinitionFromQueryCache(
      out EntityCommandDefinition entityCommandDefinition)
    {
      entityCommandDefinition = (EntityCommandDefinition) null;
      if (!this._enableQueryPlanCaching || string.IsNullOrEmpty(this._esqlCommandText))
        return false;
      EntityClientCacheKey key = new EntityClientCacheKey(this);
      QueryCacheManager queryCacheManager = this._connection.GetMetadataWorkspace().GetQueryCacheManager();
      if (!queryCacheManager.TryCacheLookup<EntityClientCacheKey, EntityCommandDefinition>(key, out entityCommandDefinition))
      {
        entityCommandDefinition = this.CreateCommandDefinition();
        QueryCacheEntry outQueryCacheEntry = (QueryCacheEntry) null;
        if (queryCacheManager.TryLookupAndAdd(new QueryCacheEntry((QueryCacheKey) key, (object) entityCommandDefinition), out outQueryCacheEntry))
          entityCommandDefinition = (EntityCommandDefinition) outQueryCacheEntry.GetTarget();
      }
      return true;
    }

    private EntityCommandDefinition CreateCommandDefinition()
    {
      if (this._preparedCommandTree == null)
        this._preparedCommandTree = this.MakeCommandTree();
      if (!this._preparedCommandTree.MetadataWorkspace.IsMetadataWorkspaceCSCompatible(this.Connection.GetMetadataWorkspace()))
        throw new InvalidOperationException(Strings.EntityClient_CommandTreeMetadataIncompatible);
      return EntityProviderServices.CreateCommandDefinition(this._connection.StoreProviderFactory, this._preparedCommandTree, this._interceptionContext, this._dependencyResolver);
    }

    private void CheckConnectionPresent()
    {
      if (this._connection == null)
        throw new InvalidOperationException(Strings.EntityClient_NoConnectionForCommand);
    }

    private void CheckIfReadyToPrepare()
    {
      this.CheckConnectionPresent();
      if (this._connection.StoreProviderFactory == null || this._connection.StoreConnection == null)
        throw Error.EntityClient_ConnectionStringNeededBeforeOperation();
      if (this._connection.State == ConnectionState.Closed || this._connection.State == ConnectionState.Broken)
        throw new InvalidOperationException(Strings.EntityClient_ExecutingOnClosedConnection(this._connection.State == ConnectionState.Closed ? (object) Strings.EntityClient_ConnectionStateClosed : (object) Strings.EntityClient_ConnectionStateBroken));
    }

    private void ThrowIfDataReaderIsOpen()
    {
      if (this._dataReader != null)
        throw new InvalidOperationException(Strings.EntityClient_DataReaderIsStillOpen);
    }

    internal virtual Dictionary<string, TypeUsage> GetParameterTypeUsage()
    {
      Dictionary<string, TypeUsage> dictionary = new Dictionary<string, TypeUsage>(this._parameters.Count);
      foreach (EntityParameter parameter in (DbParameterCollection) this._parameters)
      {
        string parameterName = parameter.ParameterName;
        if (string.IsNullOrEmpty(parameterName))
          throw new InvalidOperationException(Strings.EntityClient_EmptyParameterName);
        if (this.CommandType == CommandType.Text && parameter.Direction != ParameterDirection.Input)
          throw new InvalidOperationException(Strings.EntityClient_InvalidParameterDirection((object) parameter.ParameterName));
        if (parameter.EdmType == null && parameter.DbType == DbType.Object && (parameter.Value == null || parameter.Value is DBNull))
          throw new InvalidOperationException(Strings.EntityClient_UnknownParameterType((object) parameterName));
        TypeUsage typeUsage = parameter.GetTypeUsage();
        try
        {
          dictionary.Add(parameterName, typeUsage);
        }
        catch (ArgumentException ex)
        {
          throw new InvalidOperationException(Strings.EntityClient_DuplicateParameterNames((object) parameter.ParameterName), (Exception) ex);
        }
      }
      return dictionary;
    }

    internal virtual void NotifyDataReaderClosing()
    {
      this._dataReader = (DbDataReader) null;
      if (this._storeProviderCommand != null)
      {
        CommandHelper.SetEntityParameterValues(this, this._storeProviderCommand, this._connection);
        this._storeProviderCommand = (DbCommand) null;
      }
      if (!this.IsNotNullOnDataReaderClosingEvent())
        return;
      this.InvokeOnDataReaderClosingEvent(this, new EventArgs());
    }

    internal virtual void SetStoreProviderCommand(DbCommand storeProviderCommand)
    {
      this._storeProviderCommand = storeProviderCommand;
    }

    internal virtual bool IsNotNullOnDataReaderClosingEvent()
    {
      return null != this.OnDataReaderClosing;
    }

    internal virtual void InvokeOnDataReaderClosingEvent(EntityCommand sender, EventArgs e)
    {
      this.OnDataReaderClosing((object) sender, e);
    }

    internal event EventHandler OnDataReaderClosing;

    internal class EntityDataReaderFactory
    {
      internal virtual EntityDataReader CreateEntityDataReader(
        EntityCommand entityCommand,
        DbDataReader storeDataReader,
        CommandBehavior behavior)
      {
        return new EntityDataReader(entityCommand, storeDataReader, behavior);
      }
    }
  }
}
