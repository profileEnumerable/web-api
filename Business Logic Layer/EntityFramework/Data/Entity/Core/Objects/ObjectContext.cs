// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Internal.Materialization;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.EntityClient.Internal;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// ObjectContext is the top-level object that encapsulates a connection between the CLR and the database,
  /// serving as a gateway for Create, Read, Update, and Delete operations.
  /// </summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  public class ObjectContext : IDisposable, IObjectContextAdapter
  {
    private static readonly ConcurrentDictionary<Type, bool> _contextTypesWithViewCacheInitialized = new ConcurrentDictionary<Type, bool>();
    private readonly ObjectContextOptions _options = new ObjectContextOptions();
    private readonly ThrowingMonitor _asyncMonitor = new ThrowingMonitor();
    private const string UseLegacyPreserveChangesBehavior = "EntityFramework_UseLegacyPreserveChangesBehavior";
    private bool _disposed;
    private readonly IEntityAdapter _adapter;
    private EntityConnection _connection;
    private readonly MetadataWorkspace _workspace;
    private ObjectStateManager _objectStateManager;
    private ClrPerspective _perspective;
    private bool _contextOwnsConnection;
    private bool _openedConnection;
    private int _connectionRequestCount;
    private int? _queryTimeout;
    private Transaction _lastTransaction;
    private readonly bool _disallowSettingDefaultContainerName;
    private EventHandler _onSavingChanges;
    private ObjectMaterializedEventHandler _onObjectMaterialized;
    private ObjectQueryProvider _queryProvider;
    private readonly EntityWrapperFactory _entityWrapperFactory;
    private readonly ObjectQueryExecutionPlanFactory _objectQueryExecutionPlanFactory;
    private readonly System.Data.Entity.Core.Common.Internal.Materialization.Translator _translator;
    private readonly ColumnMapFactory _columnMapFactory;
    private DbInterceptionContext _interceptionContext;
    private TransactionHandler _transactionHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> class with the given connection. During construction, the metadata workspace is extracted from the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// object.
    /// </summary>
    /// <param name="connection">
    /// An <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> that contains references to the model and to the data source connection.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">The  connection  is null.</exception>
    /// <exception cref="T:System.ArgumentException">The  connection  is invalid or the metadata workspace is invalid. </exception>
    public ObjectContext(EntityConnection connection)
      : this(connection, true, (ObjectQueryExecutionPlanFactory) null, (System.Data.Entity.Core.Common.Internal.Materialization.Translator) null, (ColumnMapFactory) null)
    {
      this._contextOwnsConnection = false;
    }

    /// <summary>
    /// Creates an ObjectContext with the given connection and metadata workspace.
    /// </summary>
    /// <param name="connection"> connection to the store </param>
    /// <param name="contextOwnsConnection"> If set to true the connection is disposed when the context is disposed, otherwise the caller must dispose the connection. </param>
    public ObjectContext(EntityConnection connection, bool contextOwnsConnection)
      : this(connection, true, (ObjectQueryExecutionPlanFactory) null, (System.Data.Entity.Core.Common.Internal.Materialization.Translator) null, (ColumnMapFactory) null)
    {
      this._contextOwnsConnection = contextOwnsConnection;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> class with the given connection string and default entity container name.
    /// </summary>
    /// <param name="connectionString">The connection string, which also provides access to the metadata information.</param>
    /// <exception cref="T:System.ArgumentNullException">The  connectionString  is null.</exception>
    /// <exception cref="T:System.ArgumentException">The  connectionString  is invalid or the metadata workspace is not valid. </exception>
    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Object is in fact passed to property of the class and gets Disposed properly in the Dispose() method.")]
    public ObjectContext(string connectionString)
      : this(ObjectContext.CreateEntityConnection(connectionString), false, (ObjectQueryExecutionPlanFactory) null, (System.Data.Entity.Core.Common.Internal.Materialization.Translator) null, (ColumnMapFactory) null)
    {
      this._contextOwnsConnection = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> class with a given connection string and entity container name.
    /// </summary>
    /// <param name="connectionString">The connection string, which also provides access to the metadata information.</param>
    /// <param name="defaultContainerName">The name of the default entity container. When the  defaultContainerName  is set through this method, the property becomes read-only.</param>
    /// <exception cref="T:System.ArgumentNullException">The  connectionString  is null.</exception>
    /// <exception cref="T:System.ArgumentException">The  connectionString ,  defaultContainerName , or metadata workspace is not valid.</exception>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Class is internal and methods are made virtual for testing purposes only. They cannot be overrided by user.")]
    protected ObjectContext(string connectionString, string defaultContainerName)
      : this(connectionString)
    {
      this.DefaultContainerName = defaultContainerName;
      if (string.IsNullOrEmpty(defaultContainerName))
        return;
      this._disallowSettingDefaultContainerName = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> class with a given connection and entity container name.
    /// </summary>
    /// <param name="connection">
    /// An <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> that contains references to the model and to the data source connection.
    /// </param>
    /// <param name="defaultContainerName">The name of the default entity container. When the  defaultContainerName  is set through this method, the property becomes read-only.</param>
    /// <exception cref="T:System.ArgumentNullException">The  connection  is null.</exception>
    /// <exception cref="T:System.ArgumentException">The  connection ,  defaultContainerName , or metadata workspace is not valid.</exception>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Class is internal and methods are made virtual for testing purposes only. They cannot be overrided by user.")]
    protected ObjectContext(EntityConnection connection, string defaultContainerName)
      : this(connection)
    {
      this.DefaultContainerName = defaultContainerName;
      if (string.IsNullOrEmpty(defaultContainerName))
        return;
      this._disallowSettingDefaultContainerName = true;
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    internal ObjectContext(
      EntityConnection connection,
      bool isConnectionConstructor,
      ObjectQueryExecutionPlanFactory objectQueryExecutionPlanFactory,
      System.Data.Entity.Core.Common.Internal.Materialization.Translator translator = null,
      ColumnMapFactory columnMapFactory = null)
    {
      Check.NotNull<EntityConnection>(connection, nameof (connection));
      this._interceptionContext = new DbInterceptionContext().WithObjectContext(this);
      this._objectQueryExecutionPlanFactory = objectQueryExecutionPlanFactory ?? new ObjectQueryExecutionPlanFactory((System.Data.Entity.Core.Common.Internal.Materialization.Translator) null);
      this._translator = translator ?? new System.Data.Entity.Core.Common.Internal.Materialization.Translator();
      this._columnMapFactory = columnMapFactory ?? new ColumnMapFactory();
      this._adapter = (IEntityAdapter) new EntityAdapter(this);
      this._connection = connection;
      this._connection.AssociateContext(this);
      this._connection.StateChange += new StateChangeEventHandler(this.ConnectionStateChange);
      this._entityWrapperFactory = new EntityWrapperFactory();
      string connectionString = connection.ConnectionString;
      if (connectionString != null)
      {
        if (connectionString.Trim().Length != 0)
        {
          try
          {
            this._workspace = this.RetrieveMetadataWorkspaceFromConnection();
          }
          catch (InvalidOperationException ex)
          {
            throw isConnectionConstructor ? new ArgumentException(Strings.ObjectContext_InvalidConnection, nameof (connection), (Exception) ex) : new ArgumentException(Strings.ObjectContext_InvalidConnectionString, "connectionString", (Exception) ex);
          }
          string appSetting = ConfigurationManager.AppSettings["EntityFramework_UseLegacyPreserveChangesBehavior"];
          bool result = false;
          if (bool.TryParse(appSetting, out result))
            this.ContextOptions.UseLegacyPreserveChangesBehavior = result;
          this.InitializeMappingViewCacheFactory((DbContext) null);
          return;
        }
      }
      throw isConnectionConstructor ? new ArgumentException(Strings.ObjectContext_InvalidConnection, nameof (connection), (Exception) null) : new ArgumentException(Strings.ObjectContext_InvalidConnectionString, "connectionString", (Exception) null);
    }

    internal ObjectContext(
      ObjectQueryExecutionPlanFactory objectQueryExecutionPlanFactory = null,
      System.Data.Entity.Core.Common.Internal.Materialization.Translator translator = null,
      ColumnMapFactory columnMapFactory = null,
      IEntityAdapter adapter = null)
    {
      this._interceptionContext = new DbInterceptionContext().WithObjectContext(this);
      this._objectQueryExecutionPlanFactory = objectQueryExecutionPlanFactory ?? new ObjectQueryExecutionPlanFactory((System.Data.Entity.Core.Common.Internal.Materialization.Translator) null);
      this._translator = translator ?? new System.Data.Entity.Core.Common.Internal.Materialization.Translator();
      this._columnMapFactory = columnMapFactory ?? new ColumnMapFactory();
      this._adapter = adapter ?? (IEntityAdapter) new EntityAdapter(this);
    }

    /// <summary>Gets the connection used by the object context.</summary>
    /// <returns>
    /// A <see cref="T:System.Data.Common.DbConnection" /> object that is the connection.
    /// </returns>
    /// <exception cref="T:System.ObjectDisposedException">
    /// When the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> instance has been disposed.
    /// </exception>
    public virtual DbConnection Connection
    {
      get
      {
        if (this._connection == null)
          throw new ObjectDisposedException((string) null, Strings.ObjectContext_ObjectDisposed);
        return (DbConnection) this._connection;
      }
    }

    /// <summary>Gets or sets the default container name.</summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that is the default container name.
    /// </returns>
    public virtual string DefaultContainerName
    {
      get
      {
        EntityContainer defaultContainer = this.Perspective.GetDefaultContainer();
        if (defaultContainer == null)
          return string.Empty;
        return defaultContainer.Name;
      }
      set
      {
        if (this._disallowSettingDefaultContainerName)
          throw new InvalidOperationException(Strings.ObjectContext_CannotSetDefaultContainerName);
        this.Perspective.SetDefaultContainer(value);
      }
    }

    /// <summary>Gets the metadata workspace used by the object context. </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> object associated with this
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </returns>
    public virtual MetadataWorkspace MetadataWorkspace
    {
      get
      {
        return this._workspace;
      }
    }

    /// <summary>Gets the object state manager used by the object context to track object changes.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" /> used by this
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </returns>
    public virtual ObjectStateManager ObjectStateManager
    {
      get
      {
        if (this._objectStateManager == null)
          this._objectStateManager = new ObjectStateManager(this._workspace);
        return this._objectStateManager;
      }
    }

    internal bool ContextOwnsConnection
    {
      set
      {
        this._contextOwnsConnection = value;
      }
    }

    internal ClrPerspective Perspective
    {
      get
      {
        if (this._perspective == null)
          this._perspective = new ClrPerspective(this.MetadataWorkspace);
        return this._perspective;
      }
    }

    /// <summary>Gets or sets the timeout value, in seconds, for all object context operations. A null value indicates that the default value of the underlying provider will be used.</summary>
    /// <returns>
    /// An <see cref="T:System.Int32" /> value that is the timeout value, in seconds.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">The timeout value is less than 0. </exception>
    public virtual int? CommandTimeout
    {
      get
      {
        return this._queryTimeout;
      }
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          if ((nullable.GetValueOrDefault() >= 0 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
            throw new ArgumentException(Strings.ObjectContext_InvalidCommandTimeout, nameof (value));
        }
        this._queryTimeout = value;
      }
    }

    /// <summary>Gets the LINQ query provider associated with this object context.</summary>
    /// <returns>
    /// The <see cref="T:System.Linq.IQueryProvider" /> instance used by this object context.
    /// </returns>
    protected internal virtual IQueryProvider QueryProvider
    {
      get
      {
        if (this._queryProvider == null)
          this._queryProvider = new ObjectQueryProvider(this);
        return (IQueryProvider) this._queryProvider;
      }
    }

    internal bool InMaterialization { get; set; }

    internal ThrowingMonitor AsyncMonitor
    {
      get
      {
        return this._asyncMonitor;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Objects.ObjectContextOptions" /> instance that contains options that affect the behavior of the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectContextOptions" /> instance that contains options that affect the behavior of the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </returns>
    public virtual ObjectContextOptions ContextOptions
    {
      get
      {
        return this._options;
      }
    }

    internal CollectionColumnMap ColumnMapBuilder { get; set; }

    internal virtual EntityWrapperFactory EntityWrapperFactory
    {
      get
      {
        return this._entityWrapperFactory;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    ObjectContext IObjectContextAdapter.ObjectContext
    {
      get
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the transaction handler in use by this context. May be null if no transaction have been started.
    /// </summary>
    /// <value>The transaction handler.</value>
    public TransactionHandler TransactionHandler
    {
      get
      {
        this.EnsureTransactionHandlerRegistered();
        return this._transactionHandler;
      }
    }

    /// <summary>
    /// Returns the <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext" /> being used for this context.
    /// </summary>
    public DbInterceptionContext InterceptionContext
    {
      get
      {
        return this._interceptionContext;
      }
      internal set
      {
        this._interceptionContext = value;
      }
    }

    /// <summary>Occurs when changes are saved to the data source. </summary>
    public event EventHandler SavingChanges
    {
      add
      {
        this._onSavingChanges += value;
      }
      remove
      {
        this._onSavingChanges -= value;
      }
    }

    private void OnSavingChanges()
    {
      if (this._onSavingChanges == null)
        return;
      this._onSavingChanges((object) this, new EventArgs());
    }

    /// <summary>Occurs when a new entity object is created from data in the data source as part of a query or load operation. </summary>
    public event ObjectMaterializedEventHandler ObjectMaterialized
    {
      add
      {
        this._onObjectMaterialized += value;
      }
      remove
      {
        this._onObjectMaterialized -= value;
      }
    }

    internal void OnObjectMaterialized(object entity)
    {
      if (this._onObjectMaterialized == null)
        return;
      this._onObjectMaterialized((object) this, new ObjectMaterializedEventArgs(entity));
    }

    internal bool OnMaterializedHasHandlers
    {
      get
      {
        if (this._onObjectMaterialized != null)
          return this._onObjectMaterialized.GetInvocationList().Length != 0;
        return false;
      }
    }

    /// <summary>Accepts all changes made to objects in the object context.</summary>
    public virtual void AcceptAllChanges()
    {
      if (this.ObjectStateManager.SomeEntryWithConceptualNullExists())
        throw new InvalidOperationException(Strings.ObjectContext_CommitWithConceptualNull);
      foreach (ObjectStateEntry objectStateEntry in this.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted))
        objectStateEntry.AcceptChanges();
      foreach (ObjectStateEntry objectStateEntry in this.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified))
        objectStateEntry.AcceptChanges();
    }

    private void VerifyRootForAdd(
      bool doAttach,
      string entitySetName,
      IEntityWrapper wrappedEntity,
      EntityEntry existingEntry,
      out EntitySet entitySet,
      out bool isNoOperation)
    {
      isNoOperation = false;
      EntitySet entitySet1 = (EntitySet) null;
      if (doAttach)
      {
        if (!string.IsNullOrEmpty(entitySetName))
          entitySet1 = this.GetEntitySetFromName(entitySetName);
      }
      else
        entitySet1 = this.GetEntitySetFromName(entitySetName);
      EntitySet entitySet2 = (EntitySet) null;
      EntityKey key = existingEntry != null ? existingEntry.EntityKey : wrappedEntity.GetEntityKeyFromEntity();
      if ((object) key != null)
      {
        entitySet2 = key.GetEntitySet(this.MetadataWorkspace);
        if (entitySet1 != null)
          EntityUtil.ValidateEntitySetInKey(key, entitySet1, nameof (entitySetName));
        key.ValidateEntityKey(this._workspace, entitySet2);
      }
      entitySet = entitySet2 ?? entitySet1;
      if (entitySet == null)
        throw new InvalidOperationException(Strings.ObjectContext_EntitySetNameOrEntityKeyRequired);
      this.ValidateEntitySet(entitySet, wrappedEntity.IdentityType);
      if (doAttach && existingEntry == null)
      {
        if ((object) key == null)
          key = this.ObjectStateManager.CreateEntityKey(entitySet, wrappedEntity.Entity);
        existingEntry = this.ObjectStateManager.FindEntityEntry(key);
      }
      if (existingEntry == null || doAttach && existingEntry.IsKeyEntry)
        return;
      if (!object.ReferenceEquals(existingEntry.Entity, wrappedEntity.Entity))
        throw new InvalidOperationException(Strings.ObjectStateManager_ObjectStateManagerContainsThisEntityKey((object) wrappedEntity.IdentityType.FullName));
      EntityState entityState = doAttach ? EntityState.Unchanged : EntityState.Added;
      if (existingEntry.State != entityState)
        throw doAttach ? new InvalidOperationException(Strings.ObjectContext_EntityAlreadyExistsInObjectStateManager) : new InvalidOperationException(Strings.ObjectStateManager_DoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity((object) existingEntry.State));
      isNoOperation = true;
    }

    /// <summary>Adds an object to the object context. </summary>
    /// <param name="entitySetName">Represents the entity set name, which may optionally be qualified by the entity container name. </param>
    /// <param name="entity">
    /// The <see cref="T:System.Object" /> to add.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">The  entity  parameter is null or the  entitySetName  does not qualify.</exception>
    public virtual void AddObject(string entitySetName, object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      EntityEntry existingEntry;
      IEntityWrapper wrappedEntity = this.EntityWrapperFactory.WrapEntityUsingContextGettingEntry(entity, this, out existingEntry);
      if (existingEntry == null)
        this.MetadataWorkspace.ImplicitLoadAssemblyForType(wrappedEntity.IdentityType, (Assembly) null);
      EntitySet entitySet;
      bool isNoOperation;
      this.VerifyRootForAdd(false, entitySetName, wrappedEntity, existingEntry, out entitySet, out isNoOperation);
      if (isNoOperation)
        return;
      TransactionManager transactionManager = this.ObjectStateManager.TransactionManager;
      transactionManager.BeginAddTracking();
      try
      {
        RelationshipManager relationshipManager = wrappedEntity.RelationshipManager;
        bool flag = true;
        try
        {
          this.AddSingleObject(entitySet, wrappedEntity, nameof (entity));
          flag = false;
        }
        finally
        {
          if (flag && wrappedEntity.Context == this)
          {
            EntityEntry entityEntry = this.ObjectStateManager.FindEntityEntry(wrappedEntity.Entity);
            if (entityEntry != null && entityEntry.EntityKey.IsTemporary)
            {
              relationshipManager.NodeVisited = true;
              RelationshipManager.RemoveRelatedEntitiesFromObjectStateManager(wrappedEntity);
              RelatedEnd.RemoveEntityFromObjectStateManager(wrappedEntity);
            }
          }
        }
        relationshipManager.AddRelatedEntitiesToObjectStateManager(false);
      }
      finally
      {
        transactionManager.EndAddTracking();
      }
    }

    internal void AddSingleObject(
      EntitySet entitySet,
      IEntityWrapper wrappedEntity,
      string argumentName)
    {
      EntityKey entityKeyFromEntity = wrappedEntity.GetEntityKeyFromEntity();
      if ((object) entityKeyFromEntity != null)
      {
        EntityUtil.ValidateEntitySetInKey(entityKeyFromEntity, entitySet);
        entityKeyFromEntity.ValidateEntityKey(this._workspace, entitySet);
      }
      this.VerifyContextForAddOrAttach(wrappedEntity);
      wrappedEntity.Context = this;
      EntityEntry entityEntry = this.ObjectStateManager.AddEntry(wrappedEntity, (EntityKey) null, entitySet, argumentName, true);
      this.ObjectStateManager.TransactionManager.ProcessedEntities.Add(wrappedEntity);
      wrappedEntity.AttachContext(this, entitySet, MergeOption.AppendOnly);
      entityEntry.FixupFKValuesFromNonAddedReferences();
      this.ObjectStateManager.FixupReferencesByForeignKeys(entityEntry, false);
      wrappedEntity.TakeSnapshotOfRelationships(entityEntry);
    }

    /// <summary>Explicitly loads an object related to the supplied object by the specified navigation property and using the default merge option. </summary>
    /// <param name="entity">The entity for which related objects are to be loaded.</param>
    /// <param name="navigationProperty">The name of the navigation property that returns the related objects to be loaded.</param>
    /// <exception cref="T:System.InvalidOperationException">
    /// The  entity  is in a <see cref="F:System.Data.Entity.EntityState.Detached" />,
    /// <see cref="F:System.Data.Entity.EntityState.Added," />
    /// or <see cref="F:System.Data.Entity.EntityState.Deleted" /> state or the  entity  is attached to another instance of
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </exception>
    public virtual void LoadProperty(object entity, string navigationProperty)
    {
      this.WrapEntityAndCheckContext(entity, "property").RelationshipManager.GetRelatedEnd(navigationProperty, false).Load();
    }

    /// <summary>Explicitly loads an object that is related to the supplied object by the specified navigation property and using the specified merge option. </summary>
    /// <param name="entity">The entity for which related objects are to be loaded.</param>
    /// <param name="navigationProperty">The name of the navigation property that returns the related objects to be loaded.</param>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> value to use when you load the related objects.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// The  entity  is in a <see cref="F:System.Data.Entity.EntityState.Detached" />,
    /// <see cref="F:System.Data.Entity.EntityState.Added," />
    /// or <see cref="F:System.Data.Entity.EntityState.Deleted" /> state or the  entity  is attached to another instance of
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </exception>
    public virtual void LoadProperty(
      object entity,
      string navigationProperty,
      MergeOption mergeOption)
    {
      this.WrapEntityAndCheckContext(entity, "property").RelationshipManager.GetRelatedEnd(navigationProperty, false).Load(mergeOption);
    }

    /// <summary>Explicitly loads an object that is related to the supplied object by the specified LINQ query and by using the default merge option. </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The source object for which related objects are to be loaded.</param>
    /// <param name="selector">A LINQ expression that defines the related objects to be loaded.</param>
    /// <exception cref="T:System.ArgumentException"> selector  does not supply a valid input parameter.</exception>
    /// <exception cref="T:System.ArgumentNullException"> selector  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// The  entity  is in a <see cref="F:System.Data.Entity.EntityState.Detached" />,
    /// <see cref="F:System.Data.Entity.EntityState.Added," />
    /// or <see cref="F:System.Data.Entity.EntityState.Deleted" /> state or the  entity  is attached to another instance of
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual void LoadProperty<TEntity>(
      TEntity entity,
      Expression<Func<TEntity, object>> selector)
    {
      bool removedConvert;
      string selectorExpression = ObjectContext.ParsePropertySelectorExpression<TEntity>(selector, out removedConvert);
      this.WrapEntityAndCheckContext((object) entity, "property").RelationshipManager.GetRelatedEnd(selectorExpression, removedConvert).Load();
    }

    /// <summary>Explicitly loads an object that is related to the supplied object by the specified LINQ query and by using the specified merge option. </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The source object for which related objects are to be loaded.</param>
    /// <param name="selector">A LINQ expression that defines the related objects to be loaded.</param>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> value to use when you load the related objects.
    /// </param>
    /// <exception cref="T:System.ArgumentException"> selector  does not supply a valid input parameter.</exception>
    /// <exception cref="T:System.ArgumentNullException"> selector  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// The  entity  is in a <see cref="F:System.Data.Entity.EntityState.Detached" />,
    /// <see cref="F:System.Data.Entity.EntityState.Added," />
    /// or <see cref="F:System.Data.Entity.EntityState.Deleted" /> state or the  entity  is attached to another instance of
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual void LoadProperty<TEntity>(
      TEntity entity,
      Expression<Func<TEntity, object>> selector,
      MergeOption mergeOption)
    {
      bool removedConvert;
      string selectorExpression = ObjectContext.ParsePropertySelectorExpression<TEntity>(selector, out removedConvert);
      this.WrapEntityAndCheckContext((object) entity, "property").RelationshipManager.GetRelatedEnd(selectorExpression, removedConvert).Load(mergeOption);
    }

    private IEntityWrapper WrapEntityAndCheckContext(object entity, string refType)
    {
      IEntityWrapper entityWrapper = this.EntityWrapperFactory.WrapEntityUsingContext(entity, this);
      if (entityWrapper.Context == null)
        throw new InvalidOperationException(Strings.ObjectContext_CannotExplicitlyLoadDetachedRelationships((object) refType));
      if (entityWrapper.Context != this)
        throw new InvalidOperationException(Strings.ObjectContext_CannotLoadReferencesUsingDifferentContext((object) refType));
      return entityWrapper;
    }

    internal static string ParsePropertySelectorExpression<TEntity>(
      Expression<Func<TEntity, object>> selector,
      out bool removedConvert)
    {
      Check.NotNull<Expression<Func<TEntity, object>>>(selector, nameof (selector));
      removedConvert = false;
      Expression expression;
      for (expression = selector.Body; expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked; expression = ((UnaryExpression) expression).Operand)
        removedConvert = true;
      MemberExpression memberExpression = expression as MemberExpression;
      if (memberExpression == null || !memberExpression.Member.DeclaringType.IsAssignableFrom(typeof (TEntity)) || memberExpression.Expression.NodeType != ExpressionType.Parameter)
        throw new ArgumentException(Strings.ObjectContext_SelectorExpressionMustBeMemberAccess);
      return memberExpression.Member.Name;
    }

    /// <summary>Applies property changes from a detached object to an object already attached to the object context.</summary>
    /// <param name="entitySetName">The name of the entity set to which the object belongs.</param>
    /// <param name="changed">The detached object that has property updates to apply to the original object.</param>
    /// <exception cref="T:System.ArgumentNullException">When  entitySetName  is null or an empty string or when  changed  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> from  entitySetName  does not match the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />
    /// of the object
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// or when the entity is in a state other than
    /// <see cref="F:System.Data.Entity.EntityState.Modified" />
    /// or
    /// <see cref="F:System.Data.Entity.EntityState.Unchanged" />
    /// or the original object is not attached to the context.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">When the type of the  changed  object is not the same type as the original object.</exception>
    [Obsolete("Use ApplyCurrentValues instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public virtual void ApplyPropertyChanges(string entitySetName, object changed)
    {
      Check.NotNull<object>(changed, nameof (changed));
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      this.ApplyCurrentValues<object>(entitySetName, changed);
    }

    /// <summary>
    /// Copies the scalar values from the supplied object into the object in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// that has the same key.
    /// </summary>
    /// <returns>The updated object.</returns>
    /// <param name="entitySetName">The name of the entity set to which the object belongs.</param>
    /// <param name="currentEntity">
    /// The detached object that has property updates to apply to the original object. The entity key of  currentEntity  must match the
    /// <see cref="P:System.Data.Entity.Core.Objects.ObjectStateEntry.EntityKey" />
    /// property of an entry in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </param>
    /// <typeparam name="TEntity">The entity type of the object.</typeparam>
    /// <exception cref="T:System.ArgumentNullException"> entitySetName  or  current  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> from  entitySetName  does not match the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />
    /// of the object
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    ///  or the object is not in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" />
    /// or it is in a
    /// <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// state or the entity key of the supplied object is invalid.
    /// </exception>
    /// <exception cref="T:System.ArgumentException"> entitySetName  is an empty string.</exception>
    public virtual TEntity ApplyCurrentValues<TEntity>(string entitySetName, TEntity currentEntity) where TEntity : class
    {
      Check.NotNull<TEntity>(currentEntity, nameof (currentEntity));
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      IEntityWrapper wrappedCurrentEntity = this.EntityWrapperFactory.WrapEntityUsingContext((object) currentEntity, this);
      this.MetadataWorkspace.ImplicitLoadAssemblyForType(wrappedCurrentEntity.IdentityType, (Assembly) null);
      EntitySet entitySetFromName = this.GetEntitySetFromName(entitySetName);
      EntityKey entityKey = wrappedCurrentEntity.EntityKey;
      if ((object) entityKey != null)
      {
        EntityUtil.ValidateEntitySetInKey(entityKey, entitySetFromName, nameof (entitySetName));
        entityKey.ValidateEntityKey(this._workspace, entitySetFromName);
      }
      else
        entityKey = this.ObjectStateManager.CreateEntityKey(entitySetFromName, (object) currentEntity);
      EntityEntry entityEntry = this.ObjectStateManager.FindEntityEntry(entityKey);
      if (entityEntry == null || entityEntry.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectStateManager_EntityNotTracked);
      entityEntry.ApplyCurrentValuesInternal(wrappedCurrentEntity);
      return (TEntity) entityEntry.Entity;
    }

    /// <summary>
    /// Copies the scalar values from the supplied object into set of original values for the object in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// that has the same key.
    /// </summary>
    /// <returns>The updated object.</returns>
    /// <param name="entitySetName">The name of the entity set to which the object belongs.</param>
    /// <param name="originalEntity">
    /// The detached object that has original values to apply to the object. The entity key of  originalEntity  must match the
    /// <see cref="P:System.Data.Entity.Core.Objects.ObjectStateEntry.EntityKey" />
    /// property of an entry in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </param>
    /// <typeparam name="TEntity">The type of the entity object.</typeparam>
    /// <exception cref="T:System.ArgumentNullException"> entitySetName  or  original  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> from  entitySetName  does not match the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />
    /// of the object
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    ///  or an
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// for the object cannot be found in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" />
    ///  or the object is in an
    /// <see cref="F:System.Data.Entity.EntityState.Added" />
    /// or a
    /// <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// state  or the entity key of the supplied object is invalid or has property changes.
    /// </exception>
    /// <exception cref="T:System.ArgumentException"> entitySetName  is an empty string.</exception>
    public virtual TEntity ApplyOriginalValues<TEntity>(
      string entitySetName,
      TEntity originalEntity)
      where TEntity : class
    {
      Check.NotNull<TEntity>(originalEntity, nameof (originalEntity));
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      IEntityWrapper entityWrapper = this.EntityWrapperFactory.WrapEntityUsingContext((object) originalEntity, this);
      this.MetadataWorkspace.ImplicitLoadAssemblyForType(entityWrapper.IdentityType, (Assembly) null);
      EntitySet entitySetFromName = this.GetEntitySetFromName(entitySetName);
      EntityKey entityKey = entityWrapper.EntityKey;
      if ((object) entityKey != null)
      {
        EntityUtil.ValidateEntitySetInKey(entityKey, entitySetFromName, nameof (entitySetName));
        entityKey.ValidateEntityKey(this._workspace, entitySetFromName);
      }
      else
        entityKey = this.ObjectStateManager.CreateEntityKey(entitySetFromName, (object) originalEntity);
      EntityEntry entityEntry = this.ObjectStateManager.FindEntityEntry(entityKey);
      if (entityEntry == null || entityEntry.IsKeyEntry)
        throw new InvalidOperationException(Strings.ObjectContext_EntityNotTrackedOrHasTempKey);
      if (entityEntry.State != EntityState.Modified && entityEntry.State != EntityState.Unchanged && entityEntry.State != EntityState.Deleted)
        throw new InvalidOperationException(Strings.ObjectContext_EntityMustBeUnchangedOrModifiedOrDeleted((object) entityEntry.State.ToString()));
      if (entityEntry.WrappedEntity.IdentityType != entityWrapper.IdentityType)
        throw new ArgumentException(Strings.ObjectContext_EntitiesHaveDifferentType((object) entityEntry.Entity.GetType().FullName, (object) originalEntity.GetType().FullName));
      entityEntry.CompareKeyProperties((object) originalEntity);
      entityEntry.UpdateOriginalValues(entityWrapper.Entity);
      return (TEntity) entityEntry.Entity;
    }

    /// <summary>Attaches an object or object graph to the object context in a specific entity set. </summary>
    /// <param name="entitySetName">Represents the entity set name, which may optionally be qualified by the entity container name. </param>
    /// <param name="entity">
    /// The <see cref="T:System.Object" /> to attach.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">The  entity  is null. </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// Invalid entity set  or the object has a temporary key or the object has an
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// and the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />
    /// does not match with the entity set passed in as an argument of the method or the object does not have an
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// and no entity set is provided or any object from the object graph has a temporary
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    ///  or any object from the object graph has an invalid
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// (for example, values in the key do not match values in the object) or the entity set could not be found from a given  entitySetName  name and entity container name or any object from the object graph already exists in another state manager.
    /// </exception>
    public virtual void AttachTo(string entitySetName, object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      EntityEntry existingEntry;
      IEntityWrapper wrappedEntity = this.EntityWrapperFactory.WrapEntityUsingContextGettingEntry(entity, this, out existingEntry);
      if (existingEntry == null)
        this.MetadataWorkspace.ImplicitLoadAssemblyForType(wrappedEntity.IdentityType, (Assembly) null);
      EntitySet entitySet;
      bool isNoOperation;
      this.VerifyRootForAdd(true, entitySetName, wrappedEntity, existingEntry, out entitySet, out isNoOperation);
      if (isNoOperation)
        return;
      TransactionManager transactionManager = this.ObjectStateManager.TransactionManager;
      transactionManager.BeginAttachTracking();
      try
      {
        this.ObjectStateManager.TransactionManager.OriginalMergeOption = new MergeOption?(wrappedEntity.MergeOption);
        RelationshipManager relationshipManager = wrappedEntity.RelationshipManager;
        bool flag = true;
        try
        {
          this.AttachSingleObject(wrappedEntity, entitySet);
          flag = false;
        }
        finally
        {
          if (flag && wrappedEntity.Context == this)
          {
            relationshipManager.NodeVisited = true;
            RelationshipManager.RemoveRelatedEntitiesFromObjectStateManager(wrappedEntity);
            RelatedEnd.RemoveEntityFromObjectStateManager(wrappedEntity);
          }
        }
        relationshipManager.AddRelatedEntitiesToObjectStateManager(true);
      }
      finally
      {
        transactionManager.EndAttachTracking();
      }
    }

    /// <summary>Attaches an object or object graph to the object context when the object has an entity key. </summary>
    /// <param name="entity">The object to attach.</param>
    /// <exception cref="T:System.ArgumentNullException">The  entity  is null. </exception>
    /// <exception cref="T:System.InvalidOperationException">Invalid entity key. </exception>
    public virtual void Attach(IEntityWithKey entity)
    {
      Check.NotNull<IEntityWithKey>(entity, nameof (entity));
      if ((object) entity.EntityKey == null)
        throw new InvalidOperationException(Strings.ObjectContext_CannotAttachEntityWithoutKey);
      this.AttachTo((string) null, (object) entity);
    }

    internal void AttachSingleObject(IEntityWrapper wrappedEntity, EntitySet entitySet)
    {
      RelationshipManager relationshipManager = wrappedEntity.RelationshipManager;
      EntityKey entityKey = wrappedEntity.GetEntityKeyFromEntity();
      if ((object) entityKey != null)
      {
        EntityUtil.ValidateEntitySetInKey(entityKey, entitySet);
        entityKey.ValidateEntityKey(this._workspace, entitySet);
      }
      else
        entityKey = this.ObjectStateManager.CreateEntityKey(entitySet, wrappedEntity.Entity);
      if (entityKey.IsTemporary)
        throw new InvalidOperationException(Strings.ObjectContext_CannotAttachEntityWithTemporaryKey);
      if (wrappedEntity.EntityKey != entityKey)
        wrappedEntity.EntityKey = entityKey;
      EntityEntry entityEntry1 = this.ObjectStateManager.FindEntityEntry(entityKey);
      if (entityEntry1 != null)
      {
        if (!entityEntry1.IsKeyEntry)
          throw new InvalidOperationException(Strings.ObjectStateManager_ObjectStateManagerContainsThisEntityKey((object) wrappedEntity.IdentityType.FullName));
        this.ObjectStateManager.PromoteKeyEntryInitialization(this, entityEntry1, wrappedEntity, false);
        this.ObjectStateManager.TransactionManager.ProcessedEntities.Add(wrappedEntity);
        wrappedEntity.TakeSnapshotOfRelationships(entityEntry1);
        this.ObjectStateManager.PromoteKeyEntry(entityEntry1, wrappedEntity, false, false, true);
        this.ObjectStateManager.FixupReferencesByForeignKeys(entityEntry1, false);
        relationshipManager.CheckReferentialConstraintProperties(entityEntry1);
      }
      else
      {
        this.VerifyContextForAddOrAttach(wrappedEntity);
        wrappedEntity.Context = this;
        EntityEntry entityEntry2 = this.ObjectStateManager.AttachEntry(entityKey, wrappedEntity, entitySet);
        this.ObjectStateManager.TransactionManager.ProcessedEntities.Add(wrappedEntity);
        wrappedEntity.AttachContext(this, entitySet, MergeOption.AppendOnly);
        this.ObjectStateManager.FixupReferencesByForeignKeys(entityEntry2, false);
        wrappedEntity.TakeSnapshotOfRelationships(entityEntry2);
        relationshipManager.CheckReferentialConstraintProperties(entityEntry2);
      }
    }

    private void VerifyContextForAddOrAttach(IEntityWrapper wrappedEntity)
    {
      if (wrappedEntity.Context != null && wrappedEntity.Context != this && (!wrappedEntity.Context.ObjectStateManager.IsDisposed && wrappedEntity.MergeOption != MergeOption.NoTracking))
        throw new InvalidOperationException(Strings.Entity_EntityCantHaveMultipleChangeTrackers);
    }

    /// <summary>Creates the entity key for a specific object, or returns the entity key if it already exists. </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.EntityKey" /> of the object.
    /// </returns>
    /// <param name="entitySetName">The fully qualified name of the entity set to which the entity object belongs.</param>
    /// <param name="entity">The object for which the entity key is being retrieved. </param>
    /// <exception cref="T:System.ArgumentNullException">When either parameter is null. </exception>
    /// <exception cref="T:System.ArgumentException">When  entitySetName  is empty or when the type of the  entity  object does not exist in the entity set or when the  entitySetName  is not fully qualified.</exception>
    /// <exception cref="T:System.InvalidOperationException">When the entity key cannot be constructed successfully based on the supplied parameters.</exception>
    public virtual EntityKey CreateEntityKey(string entitySetName, object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      this.MetadataWorkspace.ImplicitLoadAssemblyForType(EntityUtil.GetEntityIdentityType(entity.GetType()), (Assembly) null);
      return this.ObjectStateManager.CreateEntityKey(this.GetEntitySetFromName(entitySetName), entity);
    }

    internal EntitySet GetEntitySetFromName(string entitySetName)
    {
      string entityset;
      string container;
      ObjectContext.GetEntitySetName(entitySetName, nameof (entitySetName), this, out entityset, out container);
      return this.GetEntitySet(entityset, container);
    }

    private void AddRefreshKey(
      object entityLike,
      Dictionary<EntityKey, EntityEntry> entities,
      Dictionary<EntitySet, List<EntityKey>> currentKeys)
    {
      if (entityLike == null)
        throw new InvalidOperationException(Strings.ObjectContext_NthElementIsNull((object) entities.Count));
      EntityKey entityKey = this.EntityWrapperFactory.WrapEntityUsingContext(entityLike, this).EntityKey;
      this.RefreshCheck(entities, entityKey);
      EntitySet entitySet = entityKey.GetEntitySet(this.MetadataWorkspace);
      List<EntityKey> entityKeyList = (List<EntityKey>) null;
      if (!currentKeys.TryGetValue(entitySet, out entityKeyList))
      {
        entityKeyList = new List<EntityKey>();
        currentKeys.Add(entitySet, entityKeyList);
      }
      entityKeyList.Add(entityKey);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" /> instance that is used to query, add, modify, and delete objects of the specified entity type.
    /// </summary>
    /// <returns>
    /// The new <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" /> instance.
    /// </returns>
    /// <typeparam name="TEntity">
    /// Entity type of the requested <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" />.
    /// </typeparam>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="P:System.Data.Entity.Core.Objects.ObjectContext.DefaultContainerName" /> property is not set on the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    ///  or the specified type belongs to more than one entity set.
    /// </exception>
    public virtual ObjectSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class
    {
      return new ObjectSet<TEntity>(this.GetEntitySetForType(typeof (TEntity), nameof (TEntity)), this);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" /> instance that is used to query, add, modify, and delete objects of the specified type and with the specified entity set name.
    /// </summary>
    /// <returns>
    /// The new <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" /> instance.
    /// </returns>
    /// <param name="entitySetName">
    /// Name of the entity set for the returned <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" />. The string must be qualified by the default container name if the
    /// <see cref="P:System.Data.Entity.Core.Objects.ObjectContext.DefaultContainerName" />
    /// property is not set on the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type of the requested <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" />.
    /// </typeparam>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> from  entitySetName  does not match the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />
    /// of the object
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    ///  or the
    /// <see cref="P:System.Data.Entity.Core.Objects.ObjectContext.DefaultContainerName" />
    /// property is not set on the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// and the name is not qualified as part of the  entitySetName  parameter or the specified type belongs to more than one entity set.
    /// </exception>
    public virtual ObjectSet<TEntity> CreateObjectSet<TEntity>(string entitySetName) where TEntity : class
    {
      return new ObjectSet<TEntity>(this.GetEntitySetForNameAndType(entitySetName, typeof (TEntity), nameof (TEntity)), this);
    }

    private EntitySet GetEntitySetForType(
      Type entityCLRType,
      string exceptionParameterName)
    {
      EntitySet entitySet = (EntitySet) null;
      EntityContainer defaultContainer = this.Perspective.GetDefaultContainer();
      if (defaultContainer == null)
      {
        foreach (EntityContainer container in this.MetadataWorkspace.GetItems<EntityContainer>(DataSpace.CSpace))
        {
          EntitySet setFromContainer = this.GetEntitySetFromContainer(container, entityCLRType, exceptionParameterName);
          if (setFromContainer != null)
          {
            if (entitySet != null)
              throw new ArgumentException(Strings.ObjectContext_MultipleEntitySetsFoundInAllContainers((object) entityCLRType.FullName), exceptionParameterName);
            entitySet = setFromContainer;
          }
        }
      }
      else
        entitySet = this.GetEntitySetFromContainer(defaultContainer, entityCLRType, exceptionParameterName);
      if (entitySet == null)
        throw new ArgumentException(Strings.ObjectContext_NoEntitySetFoundForType((object) entityCLRType.FullName), exceptionParameterName);
      return entitySet;
    }

    private EntitySet GetEntitySetFromContainer(
      EntityContainer container,
      Type entityCLRType,
      string exceptionParameterName)
    {
      EdmType edmType = this.GetTypeUsage(entityCLRType).EdmType;
      EntitySet entitySet = (EntitySet) null;
      foreach (EntitySetBase baseEntitySet in container.BaseEntitySets)
      {
        if (baseEntitySet.BuiltInTypeKind == BuiltInTypeKind.EntitySet && baseEntitySet.ElementType == edmType)
        {
          if (entitySet != null)
            throw new ArgumentException(Strings.ObjectContext_MultipleEntitySetsFoundInSingleContainer((object) entityCLRType.FullName, (object) container.Name), exceptionParameterName);
          entitySet = (EntitySet) baseEntitySet;
        }
      }
      return entitySet;
    }

    private EntitySet GetEntitySetForNameAndType(
      string entitySetName,
      Type entityCLRType,
      string exceptionParameterName)
    {
      EntitySet entitySetFromName = this.GetEntitySetFromName(entitySetName);
      EdmType edmType = this.GetTypeUsage(entityCLRType).EdmType;
      if (entitySetFromName.ElementType != edmType)
        throw new ArgumentException(Strings.ObjectContext_InvalidObjectSetTypeForEntitySet((object) entityCLRType.FullName, (object) entitySetFromName.ElementType.FullName, (object) entitySetName), exceptionParameterName);
      return entitySetFromName;
    }

    internal virtual void EnsureConnection(bool shouldMonitorTransactions)
    {
      if (shouldMonitorTransactions)
        this.EnsureTransactionHandlerRegistered();
      if (this.Connection.State == ConnectionState.Broken)
        this.Connection.Close();
      if (this.Connection.State == ConnectionState.Closed)
      {
        this.Connection.Open();
        this._openedConnection = true;
      }
      if (this._openedConnection)
        ++this._connectionRequestCount;
      try
      {
        Transaction current = Transaction.Current;
        this.EnsureContextIsEnlistedInCurrentTransaction<bool>(current, (Func<bool>) (() =>
        {
          this.Connection.Open();
          return true;
        }), false);
        this._lastTransaction = current;
      }
      catch (Exception ex)
      {
        this.ReleaseConnection();
        throw;
      }
    }

    internal virtual async Task EnsureConnectionAsync(
      bool shouldMonitorTransactions,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (shouldMonitorTransactions)
        this.EnsureTransactionHandlerRegistered();
      if (this.Connection.State == ConnectionState.Broken)
        this.Connection.Close();
      if (this.Connection.State == ConnectionState.Closed)
      {
        await this.Connection.OpenAsync(cancellationToken).WithCurrentCulture();
        this._openedConnection = true;
      }
      if (this._openedConnection)
        ++this._connectionRequestCount;
      try
      {
        Transaction currentTransaction = Transaction.Current;
        int num = await this.EnsureContextIsEnlistedInCurrentTransaction<Task<bool>>(currentTransaction, (Func<Task<bool>>) (async () =>
        {
          await this.Connection.OpenAsync(cancellationToken).WithCurrentCulture();
          return true;
        }), Task.FromResult<bool>(false)).WithCurrentCulture<bool>() ? 1 : 0;
        this._lastTransaction = currentTransaction;
      }
      catch (Exception ex)
      {
        this.ReleaseConnection();
        throw;
      }
    }

    private void EnsureTransactionHandlerRegistered()
    {
      if (this._transactionHandler != null || this.InterceptionContext.DbContexts.Any<DbContext>((Func<DbContext, bool>) (dbc => dbc is TransactionContext)))
        return;
      Func<TransactionHandler> service = DbConfiguration.DependencyResolver.GetService<Func<TransactionHandler>>((object) new ExecutionStrategyKey(DbConfiguration.DependencyResolver.GetService<IProviderInvariantName>((object) ((StoreItemCollection) this.MetadataWorkspace.GetItemCollection(DataSpace.SSpace)).ProviderFactory).Name, this.Connection.DataSource));
      if (service == null)
        return;
      this._transactionHandler = service();
      this._transactionHandler.Initialize(this);
    }

    private T EnsureContextIsEnlistedInCurrentTransaction<T>(
      Transaction currentTransaction,
      Func<T> openConnection,
      T defaultValue)
    {
      if (this.Connection.State != ConnectionState.Open)
        throw new InvalidOperationException(Strings.BadConnectionWrapping);
      if ((Transaction) null != currentTransaction && !currentTransaction.Equals((object) this._lastTransaction) || (Transaction) null != this._lastTransaction && !this._lastTransaction.Equals((object) currentTransaction))
      {
        if (!this._openedConnection)
        {
          if (currentTransaction != (Transaction) null)
            this.Connection.EnlistTransaction(currentTransaction);
        }
        else if (this._connectionRequestCount > 1)
        {
          if ((Transaction) null == this._lastTransaction)
          {
            this.Connection.EnlistTransaction(currentTransaction);
          }
          else
          {
            this.Connection.Close();
            return openConnection();
          }
        }
      }
      return defaultValue;
    }

    private void ConnectionStateChange(object sender, StateChangeEventArgs e)
    {
      if (e.CurrentState != ConnectionState.Closed)
        return;
      this._connectionRequestCount = 0;
      this._openedConnection = false;
    }

    internal virtual void ReleaseConnection()
    {
      if (this._disposed)
        throw new ObjectDisposedException((string) null, Strings.ObjectContext_ObjectDisposed);
      if (!this._openedConnection)
        return;
      if (this._connectionRequestCount > 0)
        --this._connectionRequestCount;
      if (this._connectionRequestCount != 0)
        return;
      this.Connection.Close();
      this._openedConnection = false;
    }

    /// <summary>
    /// Creates an <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> in the current object context by using the specified query string.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> of the specified type.
    /// </returns>
    /// <param name="queryString">The query string to be executed.</param>
    /// <param name="parameters">Parameters to pass to the query.</param>
    /// <typeparam name="T">
    /// The entity type of the returned <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" />.
    /// </typeparam>
    /// <exception cref="T:System.ArgumentNullException">The  queryString  or  parameters  parameter is null.</exception>
    public virtual ObjectQuery<T> CreateQuery<T>(
      string queryString,
      params ObjectParameter[] parameters)
    {
      Check.NotNull<string>(queryString, nameof (queryString));
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      this.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (T), Assembly.GetCallingAssembly());
      ObjectQuery<T> objectQuery = new ObjectQuery<T>(queryString, this, MergeOption.AppendOnly);
      foreach (ObjectParameter parameter in parameters)
        objectQuery.Parameters.Add(parameter);
      return objectQuery;
    }

    private static EntityConnection CreateEntityConnection(string connectionString)
    {
      Check.NotEmpty(connectionString, nameof (connectionString));
      return new EntityConnection(connectionString);
    }

    private MetadataWorkspace RetrieveMetadataWorkspaceFromConnection()
    {
      if (this._disposed)
        throw new ObjectDisposedException((string) null, Strings.ObjectContext_ObjectDisposed);
      return this._connection.GetMetadataWorkspace();
    }

    /// <summary>Marks an object for deletion. </summary>
    /// <param name="entity">
    /// An object that specifies the entity to delete. The object can be in any state except
    /// <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// .
    /// </param>
    public virtual void DeleteObject(object entity)
    {
      this.DeleteObject(entity, (EntitySet) null);
    }

    internal void DeleteObject(object entity, EntitySet expectedEntitySet)
    {
      EntityEntry entityEntry = this.ObjectStateManager.FindEntityEntry(entity);
      if (entityEntry == null || !object.ReferenceEquals(entityEntry.Entity, entity))
        throw new InvalidOperationException(Strings.ObjectContext_CannotDeleteEntityNotInObjectStateManager);
      if (expectedEntitySet != null)
      {
        EntitySetBase entitySet = entityEntry.EntitySet;
        if (entitySet != expectedEntitySet)
          throw new InvalidOperationException(Strings.ObjectContext_EntityNotInObjectSet_Delete((object) entitySet.EntityContainer.Name, (object) entitySet.Name, (object) expectedEntitySet.EntityContainer.Name, (object) expectedEntitySet.Name));
      }
      entityEntry.Delete();
    }

    /// <summary>Removes the object from the object context.</summary>
    /// <param name="entity">
    /// Object to be detached. Only the  entity  is removed; if there are any related objects that are being tracked by the same
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" />
    /// , those will not be detached automatically.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">The  entity  is null. </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// The  entity  is not associated with this <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> (for example, was newly created and not associated with any context yet, or was obtained through some other context, or was already detached).
    /// </exception>
    public virtual void Detach(object entity)
    {
      this.Detach(entity, (EntitySet) null);
    }

    internal void Detach(object entity, EntitySet expectedEntitySet)
    {
      EntityEntry entityEntry = this.ObjectStateManager.FindEntityEntry(entity);
      if (entityEntry == null || !object.ReferenceEquals(entityEntry.Entity, entity) || entityEntry.Entity == null)
        throw new InvalidOperationException(Strings.ObjectContext_CannotDetachEntityNotInObjectStateManager);
      if (expectedEntitySet != null)
      {
        EntitySetBase entitySet = entityEntry.EntitySet;
        if (entitySet != expectedEntitySet)
          throw new InvalidOperationException(Strings.ObjectContext_EntityNotInObjectSet_Detach((object) entitySet.EntityContainer.Name, (object) entitySet.Name, (object) expectedEntitySet.EntityContainer.Name, (object) expectedEntitySet.Name));
      }
      entityEntry.Detach();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> class.
    /// </summary>
    ~ObjectContext()
    {
      this.Dispose(false);
    }

    /// <summary>Releases the resources used by the object context.</summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>Releases the resources used by the object context.</summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (this._transactionHandler != null)
        this._transactionHandler.Dispose();
      if (disposing)
      {
        if (this._connection != null)
        {
          this._connection.StateChange -= new StateChangeEventHandler(this.ConnectionStateChange);
          if (this._contextOwnsConnection)
            this._connection.Dispose();
        }
        this._connection = (EntityConnection) null;
        if (this._objectStateManager != null)
          this._objectStateManager.Dispose();
      }
      this._disposed = true;
    }

    internal bool IsDisposed
    {
      get
      {
        return this._disposed;
      }
    }

    internal EntitySet GetEntitySet(string entitySetName, string entityContainerName)
    {
      EntityContainer entityContainer = (EntityContainer) null;
      if (string.IsNullOrEmpty(entityContainerName))
        entityContainer = this.Perspective.GetDefaultContainer();
      else if (!this.MetadataWorkspace.TryGetEntityContainer(entityContainerName, DataSpace.CSpace, out entityContainer))
        throw new InvalidOperationException(Strings.ObjectContext_EntityContainerNotFoundForName((object) entityContainerName));
      EntitySet entitySet = (EntitySet) null;
      if (!entityContainer.TryGetEntitySetByName(entitySetName, false, out entitySet))
        throw new InvalidOperationException(Strings.ObjectContext_EntitySetNotFoundForName((object) TypeHelpers.GetFullName(entityContainer.Name, entitySetName)));
      return entitySet;
    }

    private static void GetEntitySetName(
      string qualifiedName,
      string parameterName,
      ObjectContext context,
      out string entityset,
      out string container)
    {
      entityset = (string) null;
      container = (string) null;
      Check.NotEmpty(qualifiedName, parameterName);
      string[] strArray = qualifiedName.Split('.');
      if (strArray.Length > 2)
        throw new ArgumentException(Strings.ObjectContext_QualfiedEntitySetName, parameterName);
      if (strArray.Length == 1)
      {
        entityset = strArray[0];
      }
      else
      {
        container = strArray[0];
        entityset = strArray[1];
        if (container == null || container.Length == 0)
          throw new ArgumentException(Strings.ObjectContext_QualfiedEntitySetName, parameterName);
      }
      if (entityset == null || entityset.Length == 0)
        throw new ArgumentException(Strings.ObjectContext_QualfiedEntitySetName, parameterName);
      if (context != null && string.IsNullOrEmpty(container) && context.Perspective.GetDefaultContainer() == null)
        throw new ArgumentException(Strings.ObjectContext_ContainerQualifiedEntitySetNameRequired, parameterName);
    }

    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    private void ValidateEntitySet(EntitySet entitySet, Type entityType)
    {
      TypeUsage typeUsage = this.GetTypeUsage(entityType);
      if (!entitySet.ElementType.IsAssignableFrom(typeUsage.EdmType))
        throw new ArgumentException(Strings.ObjectContext_InvalidEntitySetOnEntity((object) entitySet.Name, (object) entityType), "entity");
    }

    internal TypeUsage GetTypeUsage(Type entityCLRType)
    {
      this.MetadataWorkspace.ImplicitLoadAssemblyForType(entityCLRType, Assembly.GetCallingAssembly());
      TypeUsage outTypeUsage = (TypeUsage) null;
      if (!this.Perspective.TryGetType(entityCLRType, out outTypeUsage) || !TypeSemantics.IsEntityType(outTypeUsage))
        throw new InvalidOperationException(Strings.ObjectContext_NoMappingForEntityType((object) entityCLRType.FullName));
      return outTypeUsage;
    }

    /// <summary>Returns an object that has the specified entity key.</summary>
    /// <returns>
    /// An <see cref="T:System.Object" /> that is an instance of an entity type.
    /// </returns>
    /// <param name="key">The key of the object to be found.</param>
    /// <exception cref="T:System.ArgumentNullException">The  key  parameter is null.</exception>
    /// <exception cref="T:System.Data.Entity.Core.ObjectNotFoundException">
    /// The object is not found in either the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" /> or the data source.
    /// </exception>
    public virtual object GetObjectByKey(EntityKey key)
    {
      Check.NotNull<EntityKey>(key, nameof (key));
      this.MetadataWorkspace.ImplicitLoadFromEntityType(key.GetEntitySet(this.MetadataWorkspace).ElementType, Assembly.GetCallingAssembly());
      object obj;
      if (!this.TryGetObjectByKey(key, out obj))
        throw new ObjectNotFoundException(Strings.ObjectContext_ObjectNotFound);
      return obj;
    }

    /// <summary>Updates a collection of objects in the object context with data from the database. </summary>
    /// <param name="refreshMode">
    /// A <see cref="T:System.Data.Entity.Core.Objects.RefreshMode" /> value that indicates whether
    /// property changes in the object context are overwritten with property values from the database.
    /// </param>
    /// <param name="collection">
    /// An <see cref="T:System.Collections.IEnumerable" /> collection of objects to refresh.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException"> collection  is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException"> refreshMode  is not valid.</exception>
    /// <exception cref="T:System.ArgumentException"> collection is empty or an object is not attached to the context. </exception>
    public virtual void Refresh(RefreshMode refreshMode, IEnumerable collection)
    {
      Check.NotNull<IEnumerable>(collection, nameof (collection));
      EntityUtil.CheckArgumentRefreshMode(refreshMode);
      this.RefreshEntities(refreshMode, collection);
    }

    /// <summary>Updates an object in the object context with data from the database. </summary>
    /// <param name="refreshMode">
    /// A <see cref="T:System.Data.Entity.Core.Objects.RefreshMode" /> value that indicates whether
    /// property changes in the object context are overwritten with property values from the database.
    /// </param>
    /// <param name="entity">The object to be refreshed. </param>
    /// <exception cref="T:System.ArgumentNullException"> entity  is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException"> refreshMode  is not valid.</exception>
    /// <exception cref="T:System.ArgumentException"> entity is not attached to the context. </exception>
    public virtual void Refresh(RefreshMode refreshMode, object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      EntityUtil.CheckArgumentRefreshMode(refreshMode);
      this.RefreshEntities(refreshMode, (IEnumerable) new object[1]
      {
        entity
      });
    }

    /// <summary>Asynchronously updates a collection of objects in the object context with data from the database. </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="refreshMode">
    /// A <see cref="T:System.Data.Entity.Core.Objects.RefreshMode" /> value that indicates whether
    /// property changes in the object context are overwritten with property values from the database.
    /// </param>
    /// <param name="collection">
    /// An <see cref="T:System.Collections.IEnumerable" /> collection of objects to refresh.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="T:System.ArgumentNullException"> collection  is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException"> refreshMode  is not valid.</exception>
    /// <exception cref="T:System.ArgumentException"> collection is empty or an object is not attached to the context. </exception>
    public Task RefreshAsync(RefreshMode refreshMode, IEnumerable collection)
    {
      return this.RefreshAsync(refreshMode, collection, CancellationToken.None);
    }

    /// <summary>Asynchronously updates a collection of objects in the object context with data from the database. </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="refreshMode">
    /// A <see cref="T:System.Data.Entity.Core.Objects.RefreshMode" /> value that indicates whether
    /// property changes in the object context are overwritten with property values from the database.
    /// </param>
    /// <param name="collection">
    /// An <see cref="T:System.Collections.IEnumerable" /> collection of objects to refresh.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="T:System.ArgumentNullException"> collection  is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException"> refreshMode  is not valid.</exception>
    /// <exception cref="T:System.ArgumentException"> collection is empty or an object is not attached to the context. </exception>
    public virtual Task RefreshAsync(
      RefreshMode refreshMode,
      IEnumerable collection,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IEnumerable>(collection, nameof (collection));
      cancellationToken.ThrowIfCancellationRequested();
      this.AsyncMonitor.EnsureNotEntered();
      EntityUtil.CheckArgumentRefreshMode(refreshMode);
      return this.RefreshEntitiesAsync(refreshMode, collection, cancellationToken);
    }

    /// <summary>Asynchronously updates an object in the object context with data from the database. </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="refreshMode">
    /// A <see cref="T:System.Data.Entity.Core.Objects.RefreshMode" /> value that indicates whether
    /// property changes in the object context are overwritten with property values from the database.
    /// </param>
    /// <param name="entity">The object to be refreshed. </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="T:System.ArgumentNullException"> entity  is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException"> refreshMode  is not valid.</exception>
    /// <exception cref="T:System.ArgumentException"> entity is not attached to the context. </exception>
    public Task RefreshAsync(RefreshMode refreshMode, object entity)
    {
      return this.RefreshAsync(refreshMode, entity, CancellationToken.None);
    }

    /// <summary>Asynchronously updates an object in the object context with data from the database. </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="refreshMode">
    /// A <see cref="T:System.Data.Entity.Core.Objects.RefreshMode" /> value that indicates whether
    /// property changes in the object context are overwritten with property values from the database.
    /// </param>
    /// <param name="entity">The object to be refreshed. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="T:System.ArgumentNullException"> entity  is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException"> refreshMode  is not valid.</exception>
    /// <exception cref="T:System.ArgumentException"> entity is not attached to the context. </exception>
    public virtual Task RefreshAsync(
      RefreshMode refreshMode,
      object entity,
      CancellationToken cancellationToken)
    {
      Check.NotNull<object>(entity, nameof (entity));
      cancellationToken.ThrowIfCancellationRequested();
      this.AsyncMonitor.EnsureNotEntered();
      EntityUtil.CheckArgumentRefreshMode(refreshMode);
      return this.RefreshEntitiesAsync(refreshMode, (IEnumerable) new object[1]
      {
        entity
      }, cancellationToken);
    }

    private void RefreshCheck(Dictionary<EntityKey, EntityEntry> entities, EntityKey key)
    {
      EntityEntry entityEntry = this.ObjectStateManager.FindEntityEntry(key);
      if (entityEntry == null)
        throw new InvalidOperationException(Strings.ObjectContext_NthElementNotInObjectStateManager((object) entities.Count));
      if (EntityState.Added == entityEntry.State)
        throw new InvalidOperationException(Strings.ObjectContext_NthElementInAddedState((object) entities.Count));
      try
      {
        entities.Add(key, entityEntry);
      }
      catch (ArgumentException ex)
      {
        throw new InvalidOperationException(Strings.ObjectContext_NthElementIsDuplicate((object) entities.Count));
      }
    }

    private void RefreshEntities(RefreshMode refreshMode, IEnumerable collection)
    {
      this.AsyncMonitor.EnsureNotEntered();
      bool flag = false;
      try
      {
        Dictionary<EntityKey, EntityEntry> dictionary = new Dictionary<EntityKey, EntityEntry>(ObjectContext.RefreshEntitiesSize(collection));
        Dictionary<EntitySet, List<EntityKey>> currentKeys = new Dictionary<EntitySet, List<EntityKey>>();
        foreach (object entityLike in collection)
          this.AddRefreshKey(entityLike, dictionary, currentKeys);
        if (currentKeys.Count > 0)
        {
          this.EnsureConnection(false);
          flag = true;
          foreach (EntitySet key in currentKeys.Keys)
          {
            List<EntityKey> targetKeys = currentKeys[key];
            int startFrom = 0;
            while (startFrom < targetKeys.Count)
              startFrom = this.BatchRefreshEntitiesByKey(refreshMode, dictionary, key, targetKeys, startFrom);
          }
        }
        if (RefreshMode.StoreWins == refreshMode)
        {
          foreach (KeyValuePair<EntityKey, EntityEntry> keyValuePair in dictionary)
          {
            if (EntityState.Detached != keyValuePair.Value.State)
            {
              this.ObjectStateManager.TransactionManager.BeginDetaching();
              try
              {
                keyValuePair.Value.Delete();
              }
              finally
              {
                this.ObjectStateManager.TransactionManager.EndDetaching();
              }
              keyValuePair.Value.AcceptChanges();
            }
          }
        }
        else
        {
          if (RefreshMode.ClientWins != refreshMode || 0 >= dictionary.Count)
            return;
          string str = string.Empty;
          StringBuilder stringBuilder = new StringBuilder();
          foreach (KeyValuePair<EntityKey, EntityEntry> keyValuePair in dictionary)
          {
            if (keyValuePair.Value.State == EntityState.Deleted)
            {
              keyValuePair.Value.AcceptChanges();
            }
            else
            {
              stringBuilder.Append(str).Append(Environment.NewLine);
              stringBuilder.Append('\'').Append(keyValuePair.Key.ConcatKeyValue()).Append('\'');
              str = ",";
            }
          }
          if (stringBuilder.Length > 0)
            throw new InvalidOperationException(Strings.ObjectContext_ClientEntityRemovedFromStore((object) stringBuilder.ToString()));
        }
      }
      finally
      {
        if (flag)
          this.ReleaseConnection();
      }
    }

    private int BatchRefreshEntitiesByKey(
      RefreshMode refreshMode,
      Dictionary<EntityKey, EntityEntry> trackedEntities,
      EntitySet targetSet,
      List<EntityKey> targetKeys,
      int startFrom)
    {
      Tuple<ObjectQueryExecutionPlan, int> queryPlanAndNextPosition = this.PrepareRefreshQuery(refreshMode, targetSet, targetKeys, startFrom);
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      ObjectResult<object> results = executionStrategy.Execute<ObjectResult<object>>((Func<ObjectResult<object>>) (() => this.ExecuteInTransaction<ObjectResult<object>>((Func<ObjectResult<object>>) (() => queryPlanAndNextPosition.Item1.Execute<object>(this, (ObjectParameterCollection) null)), executionStrategy, false, true)));
      this.ProcessRefreshedEntities(trackedEntities, results);
      return queryPlanAndNextPosition.Item2;
    }

    private async Task RefreshEntitiesAsync(
      RefreshMode refreshMode,
      IEnumerable collection,
      CancellationToken cancellationToken)
    {
      this.AsyncMonitor.Enter();
      bool openedConnection = false;
      try
      {
        Dictionary<EntityKey, EntityEntry> entities = new Dictionary<EntityKey, EntityEntry>(ObjectContext.RefreshEntitiesSize(collection));
        Dictionary<EntitySet, List<EntityKey>> refreshKeys = new Dictionary<EntitySet, List<EntityKey>>();
        foreach (object entityLike in collection)
          this.AddRefreshKey(entityLike, entities, refreshKeys);
        if (refreshKeys.Count > 0)
        {
          await this.EnsureConnectionAsync(false, cancellationToken).WithCurrentCulture();
          openedConnection = true;
          foreach (EntitySet key in refreshKeys.Keys)
          {
            List<EntityKey> setKeys = refreshKeys[key];
            int refreshedCount = 0;
            while (refreshedCount < setKeys.Count)
              refreshedCount = await this.BatchRefreshEntitiesByKeyAsync(refreshMode, entities, key, setKeys, refreshedCount, cancellationToken).WithCurrentCulture<int>();
          }
        }
        if (RefreshMode.StoreWins == refreshMode)
        {
          foreach (KeyValuePair<EntityKey, EntityEntry> keyValuePair in entities)
          {
            if (EntityState.Detached != keyValuePair.Value.State)
            {
              this.ObjectStateManager.TransactionManager.BeginDetaching();
              try
              {
                keyValuePair.Value.Delete();
              }
              finally
              {
                this.ObjectStateManager.TransactionManager.EndDetaching();
              }
              keyValuePair.Value.AcceptChanges();
            }
          }
        }
        else
        {
          if (RefreshMode.ClientWins != refreshMode || 0 >= entities.Count)
            return;
          string str = string.Empty;
          StringBuilder stringBuilder = new StringBuilder();
          foreach (KeyValuePair<EntityKey, EntityEntry> keyValuePair in entities)
          {
            if (keyValuePair.Value.State == EntityState.Deleted)
            {
              keyValuePair.Value.AcceptChanges();
            }
            else
            {
              stringBuilder.Append(str).Append(Environment.NewLine);
              stringBuilder.Append('\'').Append(keyValuePair.Key.ConcatKeyValue()).Append('\'');
              str = ",";
            }
          }
          if (stringBuilder.Length > 0)
            throw new InvalidOperationException(Strings.ObjectContext_ClientEntityRemovedFromStore((object) stringBuilder.ToString()));
        }
      }
      finally
      {
        if (openedConnection)
          this.ReleaseConnection();
        this.AsyncMonitor.Exit();
      }
    }

    private async Task<int> BatchRefreshEntitiesByKeyAsync(
      RefreshMode refreshMode,
      Dictionary<EntityKey, EntityEntry> trackedEntities,
      EntitySet targetSet,
      List<EntityKey> targetKeys,
      int startFrom,
      CancellationToken cancellationToken)
    {
      Tuple<ObjectQueryExecutionPlan, int> queryPlanAndNextPosition = this.PrepareRefreshQuery(refreshMode, targetSet, targetKeys, startFrom);
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      ObjectResult<object> results = await executionStrategy.ExecuteAsync<ObjectResult<object>>((Func<Task<ObjectResult<object>>>) (() => this.ExecuteInTransactionAsync<ObjectResult<object>>((Func<Task<ObjectResult<object>>>) (() => queryPlanAndNextPosition.Item1.ExecuteAsync<object>(this, (ObjectParameterCollection) null, cancellationToken)), executionStrategy, false, true, cancellationToken)), cancellationToken).WithCurrentCulture<ObjectResult<object>>();
      this.ProcessRefreshedEntities(trackedEntities, results);
      return queryPlanAndNextPosition.Item2;
    }

    internal virtual Tuple<ObjectQueryExecutionPlan, int> PrepareRefreshQuery(
      RefreshMode refreshMode,
      EntitySet targetSet,
      List<EntityKey> targetKeys,
      int startFrom)
    {
      DbExpressionBinding input = targetSet.Scan().BindAs("EntitySet");
      DbExpression refKey = (DbExpression) input.Variable.GetEntityRef().GetRefKey();
      int length = Math.Min(250, targetKeys.Count - startFrom);
      DbExpression[] dbExpressionArray = new DbExpression[length];
      for (int index = 0; index < length; ++index)
      {
        DbExpression right = (DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) targetKeys[startFrom++].GetKeyValueExpressions(targetSet));
        dbExpressionArray[index] = (DbExpression) refKey.Equal(right);
      }
      DbExpression predicate = Helpers.BuildBalancedTreeInPlace<DbExpression>((IList<DbExpression>) dbExpressionArray, new Func<DbExpression, DbExpression, DbExpression>(DbExpressionBuilder.Or));
      return new Tuple<ObjectQueryExecutionPlan, int>(this._objectQueryExecutionPlanFactory.Prepare(this, DbQueryCommandTree.FromValidExpression(this.MetadataWorkspace, DataSpace.CSpace, (DbExpression) input.Filter(predicate), true), typeof (object), RefreshMode.StoreWins == refreshMode ? MergeOption.OverwriteChanges : MergeOption.PreserveChanges, false, (Span) null, (IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>>) null, DbExpressionBuilder.AliasGenerator), startFrom);
    }

    private void ProcessRefreshedEntities(
      Dictionary<EntityKey, EntityEntry> trackedEntities,
      ObjectResult<object> results)
    {
      foreach (object result in results)
      {
        EntityEntry entityEntry = this.ObjectStateManager.FindEntityEntry(result);
        if (entityEntry != null && entityEntry.State == EntityState.Modified)
          entityEntry.SetModifiedAll();
        EntityKey entityKey = this.EntityWrapperFactory.WrapEntityUsingContext(result, this).EntityKey;
        if ((object) entityKey == null)
          throw Error.EntityKey_UnexpectedNull();
        if (!trackedEntities.Remove(entityKey))
          throw new InvalidOperationException(Strings.ObjectContext_StoreEntityNotPresentInClient);
      }
    }

    private static int RefreshEntitiesSize(IEnumerable collection)
    {
      ICollection collection1 = collection as ICollection;
      if (collection1 == null)
        return 0;
      return collection1.Count;
    }

    /// <summary>Persists all updates to the database and resets change tracking in the object context.</summary>
    /// <returns>
    /// The number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property
    /// included in the entity class (often referred to as independent associations).
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Core.OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
    public virtual int SaveChanges()
    {
      return this.SaveChanges(SaveOptions.AcceptAllChangesAfterSave | SaveOptions.DetectChangesBeforeSave);
    }

    /// <summary>Asynchronously persists all updates to the database and resets change tracking in the object context.</summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// The task result contains the number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property
    /// included in the entity class (often referred to as independent associations).
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Core.OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
    public virtual Task<int> SaveChangesAsync()
    {
      return this.SaveChangesAsync(SaveOptions.AcceptAllChangesAfterSave | SaveOptions.DetectChangesBeforeSave, CancellationToken.None);
    }

    /// <summary>Asynchronously persists all updates to the database and resets change tracking in the object context.</summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// The task result contains the number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property
    /// included in the entity class (often referred to as independent associations).
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Core.OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
      return this.SaveChangesAsync(SaveOptions.AcceptAllChangesAfterSave | SaveOptions.DetectChangesBeforeSave, cancellationToken);
    }

    /// <summary>Persists all updates to the database and optionally resets change tracking in the object context.</summary>
    /// <param name="acceptChangesDuringSave">
    /// This parameter is needed for client-side transaction support. If true, the change tracking on all objects is reset after
    /// <see cref="M:System.Data.Entity.Core.Objects.ObjectContext.SaveChanges(System.Boolean)" />
    /// finishes. If false, you must call the <see cref="M:System.Data.Entity.Core.Objects.ObjectContext.AcceptAllChanges" />
    /// method after <see cref="M:System.Data.Entity.Core.Objects.ObjectContext.SaveChanges(System.Boolean)" />.
    /// </param>
    /// <returns>
    /// The number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property
    /// included in the entity class (often referred to as independent associations).
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Core.OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("Use SaveChanges(SaveOptions options) instead.")]
    public virtual int SaveChanges(bool acceptChangesDuringSave)
    {
      return this.SaveChanges(acceptChangesDuringSave ? SaveOptions.AcceptAllChangesAfterSave | SaveOptions.DetectChangesBeforeSave : SaveOptions.DetectChangesBeforeSave);
    }

    /// <summary>Persists all updates to the database and optionally resets change tracking in the object context.</summary>
    /// <param name="options">
    /// A <see cref="T:System.Data.Entity.Core.Objects.SaveOptions" /> value that determines the behavior of the operation.
    /// </param>
    /// <returns>
    /// The number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property
    /// included in the entity class (often referred to as independent associations).
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Core.OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
    public virtual int SaveChanges(SaveOptions options)
    {
      return this.SaveChangesInternal(options, false);
    }

    internal int SaveChangesInternal(SaveOptions options, bool executeInExistingTransaction)
    {
      this.AsyncMonitor.EnsureNotEntered();
      this.PrepareToSaveChanges(options);
      int num = 0;
      if (this.ObjectStateManager.HasChanges())
      {
        if (executeInExistingTransaction)
        {
          num = this.SaveChangesToStore(options, (IDbExecutionStrategy) null, false);
        }
        else
        {
          IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
          num = executionStrategy.Execute<int>((Func<int>) (() => this.SaveChangesToStore(options, executionStrategy, true)));
        }
      }
      return num;
    }

    /// <summary>Asynchronously persists all updates to the database and optionally resets change tracking in the object context.</summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="options">
    /// A <see cref="T:System.Data.Entity.Core.Objects.SaveOptions" /> value that determines the behavior of the operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// The task result contains the number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property
    /// included in the entity class (often referred to as independent associations).
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Core.OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
    public virtual Task<int> SaveChangesAsync(SaveOptions options)
    {
      return this.SaveChangesAsync(options, CancellationToken.None);
    }

    /// <summary>Asynchronously persists all updates to the database and optionally resets change tracking in the object context.</summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="options">
    /// A <see cref="T:System.Data.Entity.Core.Objects.SaveOptions" /> value that determines the behavior of the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// The task result contains the number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property
    /// included in the entity class (often referred to as independent associations).
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Core.OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
    public virtual Task<int> SaveChangesAsync(
      SaveOptions options,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      this.AsyncMonitor.EnsureNotEntered();
      return this.SaveChangesInternalAsync(options, false, cancellationToken);
    }

    internal async Task<int> SaveChangesInternalAsync(
      SaveOptions options,
      bool executeInExistingTransaction,
      CancellationToken cancellationToken)
    {
      this.AsyncMonitor.Enter();
      int num;
      try
      {
        this.PrepareToSaveChanges(options);
        int entriesAffected = 0;
        if (this.ObjectStateManager.HasChanges())
        {
          if (executeInExistingTransaction)
          {
            entriesAffected = await this.SaveChangesToStoreAsync(options, (IDbExecutionStrategy) null, false, cancellationToken).WithCurrentCulture<int>();
          }
          else
          {
            IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
            entriesAffected = await executionStrategy.ExecuteAsync<int>((Func<Task<int>>) (() => this.SaveChangesToStoreAsync(options, executionStrategy, true, cancellationToken)), cancellationToken).WithCurrentCulture<int>();
          }
        }
        num = entriesAffected;
      }
      finally
      {
        this.AsyncMonitor.Exit();
      }
      return num;
    }

    private void PrepareToSaveChanges(SaveOptions options)
    {
      if (this._disposed)
        throw new ObjectDisposedException((string) null, Strings.ObjectContext_ObjectDisposed);
      this.OnSavingChanges();
      if ((SaveOptions.DetectChangesBeforeSave & options) != SaveOptions.None)
        this.ObjectStateManager.DetectChanges();
      if (this.ObjectStateManager.SomeEntryWithConceptualNullExists())
        throw new InvalidOperationException(Strings.ObjectContext_CommitWithConceptualNull);
    }

    private int SaveChangesToStore(
      SaveOptions options,
      IDbExecutionStrategy executionStrategy,
      bool startLocalTransaction)
    {
      this._adapter.AcceptChangesDuringUpdate = false;
      this._adapter.Connection = this.Connection;
      this._adapter.CommandTimeout = this.CommandTimeout;
      int num = this.ExecuteInTransaction<int>((Func<int>) (() => this._adapter.Update()), executionStrategy, startLocalTransaction, true);
      if ((SaveOptions.AcceptAllChangesAfterSave & options) != SaveOptions.None)
      {
        try
        {
          this.AcceptAllChanges();
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException(Strings.ObjectContext_AcceptAllChangesFailure((object) ex.Message), ex);
        }
      }
      return num;
    }

    private async Task<int> SaveChangesToStoreAsync(
      SaveOptions options,
      IDbExecutionStrategy executionStrategy,
      bool startLocalTransaction,
      CancellationToken cancellationToken)
    {
      this._adapter.AcceptChangesDuringUpdate = false;
      this._adapter.Connection = this.Connection;
      this._adapter.CommandTimeout = this.CommandTimeout;
      int entriesAffected = await this.ExecuteInTransactionAsync<int>((Func<Task<int>>) (() => this._adapter.UpdateAsync(cancellationToken)), executionStrategy, startLocalTransaction, true, cancellationToken).WithCurrentCulture<int>();
      if ((SaveOptions.AcceptAllChangesAfterSave & options) != SaveOptions.None)
      {
        try
        {
          this.AcceptAllChanges();
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException(Strings.ObjectContext_AcceptAllChangesFailure((object) ex.Message), ex);
        }
      }
      return entriesAffected;
    }

    internal virtual T ExecuteInTransaction<T>(
      Func<T> func,
      IDbExecutionStrategy executionStrategy,
      bool startLocalTransaction,
      bool releaseConnectionOnSuccess)
    {
      this.EnsureConnection(startLocalTransaction);
      bool flag = false;
      EntityConnection connection = (EntityConnection) this.Connection;
      if (connection.CurrentTransaction == null && !connection.EnlistedInUserTransaction && this._lastTransaction == (Transaction) null)
        flag = startLocalTransaction;
      else if (executionStrategy != null && executionStrategy.RetriesOnFailure)
        throw new InvalidOperationException(Strings.ExecutionStrategy_ExistingTransaction((object) executionStrategy.GetType().Name));
      DbTransaction dbTransaction = (DbTransaction) null;
      try
      {
        if (flag)
          dbTransaction = (DbTransaction) connection.BeginTransaction();
        T obj = func();
        dbTransaction?.Commit();
        if (releaseConnectionOnSuccess)
          this.ReleaseConnection();
        return obj;
      }
      catch (Exception ex)
      {
        this.ReleaseConnection();
        throw;
      }
      finally
      {
        dbTransaction?.Dispose();
      }
    }

    internal virtual async Task<T> ExecuteInTransactionAsync<T>(
      Func<Task<T>> func,
      IDbExecutionStrategy executionStrategy,
      bool startLocalTransaction,
      bool releaseConnectionOnSuccess,
      CancellationToken cancellationToken)
    {
      await this.EnsureConnectionAsync(startLocalTransaction, cancellationToken).WithCurrentCulture();
      bool needLocalTransaction = false;
      EntityConnection connection = (EntityConnection) this.Connection;
      if (connection.CurrentTransaction == null && !connection.EnlistedInUserTransaction && this._lastTransaction == (Transaction) null)
        needLocalTransaction = startLocalTransaction;
      else if (executionStrategy.RetriesOnFailure)
        throw new InvalidOperationException(Strings.ExecutionStrategy_ExistingTransaction((object) executionStrategy.GetType().Name));
      DbTransaction localTransaction = (DbTransaction) null;
      T obj;
      try
      {
        if (needLocalTransaction)
          localTransaction = (DbTransaction) connection.BeginTransaction();
        T result = await func().WithCurrentCulture<T>();
        localTransaction?.Commit();
        if (releaseConnectionOnSuccess)
          this.ReleaseConnection();
        obj = result;
      }
      catch (Exception ex)
      {
        this.ReleaseConnection();
        throw;
      }
      finally
      {
        localTransaction?.Dispose();
      }
      return obj;
    }

    /// <summary>
    /// Ensures that <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> changes are synchronized with changes in all objects that are tracked by the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" />
    /// .
    /// </summary>
    public virtual void DetectChanges()
    {
      this.ObjectStateManager.DetectChanges();
    }

    /// <summary>Returns an object that has the specified entity key.</summary>
    /// <returns>true if the object was retrieved successfully. false if the  key  is temporary, the connection is null, or the  value  is null.</returns>
    /// <param name="key">The key of the object to be found.</param>
    /// <param name="value">When this method returns, contains the object.</param>
    /// <exception cref="T:System.ArgumentException">Incompatible metadata for  key .</exception>
    /// <exception cref="T:System.ArgumentNullException"> key  is null.</exception>
    [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
    public virtual bool TryGetObjectByKey(EntityKey key, out object value)
    {
      EntityEntry entry;
      this.ObjectStateManager.TryGetEntityEntry(key, out entry);
      if (entry != null && !entry.IsKeyEntry)
      {
        value = entry.Entity;
        return value != null;
      }
      if (key.IsTemporary)
      {
        value = (object) null;
        return false;
      }
      EntitySet entitySet = key.GetEntitySet(this.MetadataWorkspace);
      key.ValidateEntityKey(this._workspace, entitySet, true, nameof (key));
      this.MetadataWorkspace.ImplicitLoadFromEntityType(entitySet.ElementType, Assembly.GetCallingAssembly());
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("SELECT VALUE X FROM {0}.{1} AS X WHERE ", (object) EntityUtil.QuoteIdentifier(entitySet.EntityContainer.Name), (object) EntityUtil.QuoteIdentifier(entitySet.Name));
      EntityKeyMember[] entityKeyValues = key.EntityKeyValues;
      ReadOnlyMetadataCollection<EdmMember> keyMembers = entitySet.ElementType.KeyMembers;
      ObjectParameter[] objectParameterArray = new ObjectParameter[entityKeyValues.Length];
      for (int index = 0; index < entityKeyValues.Length; ++index)
      {
        if (index > 0)
          stringBuilder.Append(" AND ");
        string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "p{0}", (object) index.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.AppendFormat("X.{0} = @{1}", (object) EntityUtil.QuoteIdentifier(entityKeyValues[index].Key), (object) name);
        objectParameterArray[index] = new ObjectParameter(name, entityKeyValues[index].Value);
        EdmMember edmMember = (EdmMember) null;
        if (keyMembers.TryGetValue(entityKeyValues[index].Key, true, out edmMember))
          objectParameterArray[index].TypeUsage = edmMember.TypeUsage;
      }
      object obj1 = (object) null;
      foreach (object obj2 in this.CreateQuery<object>(stringBuilder.ToString(), objectParameterArray).Execute(MergeOption.AppendOnly))
        obj1 = obj2;
      value = obj1;
      return value != null;
    }

    /// <summary>
    /// Executes a stored procedure or function that is defined in the data source and mapped in the conceptual model, with the specified parameters. Returns a typed
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" />
    /// .
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" /> for the data that is returned by the stored procedure.
    /// </returns>
    /// <param name="functionName">The name of the stored procedure or function. The name can include the container name, such as &lt;Container Name&gt;.&lt;Function Name&gt;. When the default container name is known, only the function name is required.</param>
    /// <param name="parameters">
    /// An array of <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> objects. If output parameters are used,
    /// their values will not be available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <typeparam name="TElement">
    /// The entity type of the <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" /> returned when the function is executed against the data source. This type must implement
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.IEntityWithChangeTracker" />
    /// .
    /// </typeparam>
    /// <exception cref="T:System.ArgumentException"> function  is null or empty or function  is not found.</exception>
    /// <exception cref="T:System.InvalidOperationException">The entity reader does not support this  function or there is a type mismatch on the reader and the  function .</exception>
    public ObjectResult<TElement> ExecuteFunction<TElement>(
      string functionName,
      params ObjectParameter[] parameters)
    {
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      return this.ExecuteFunction<TElement>(functionName, MergeOption.AppendOnly, parameters);
    }

    /// <summary>
    /// Executes the given stored procedure or function that is defined in the data source and expressed in the conceptual model, with the specified parameters, and merge option. Returns a typed
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" />
    /// .
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" /> for the data that is returned by the stored procedure.
    /// </returns>
    /// <param name="functionName">The name of the stored procedure or function. The name can include the container name, such as &lt;Container Name&gt;.&lt;Function Name&gt;. When the default container name is known, only the function name is required.</param>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> to use when executing the query.
    /// </param>
    /// <param name="parameters">
    /// An array of <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> objects. If output parameters are used,
    /// their values will not be available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <typeparam name="TElement">
    /// The entity type of the <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" /> returned when the function is executed against the data source. This type must implement
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.IEntityWithChangeTracker" />
    /// .
    /// </typeparam>
    /// <exception cref="T:System.ArgumentException"> function  is null or empty or function  is not found.</exception>
    /// <exception cref="T:System.InvalidOperationException">The entity reader does not support this  function or there is a type mismatch on the reader and the  function .</exception>
    public virtual ObjectResult<TElement> ExecuteFunction<TElement>(
      string functionName,
      MergeOption mergeOption,
      params ObjectParameter[] parameters)
    {
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      Check.NotEmpty(functionName, nameof (functionName));
      return this.ExecuteFunction<TElement>(functionName, new ExecutionOptions(mergeOption), parameters);
    }

    /// <summary>Executes the given function on the default container.</summary>
    /// <typeparam name="TElement"> Element type for function results. </typeparam>
    /// <param name="functionName">
    /// Name of function. May include container (e.g. ContainerName.FunctionName) or just function name when DefaultContainerName is known.
    /// </param>
    /// <param name="executionOptions"> The options for executing this function. </param>
    /// <param name="parameters">
    /// The parameter values to use for the function. If output parameters are used, their values
    /// will not be available until the results have been read completely. This is due to the underlying
    /// behavior of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>An object representing the result of executing this function.</returns>
    /// <exception cref="T:System.ArgumentException"> If function is null or empty </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// If function is invalid (syntax,
    /// does not exist, refers to a function with return type incompatible with T)
    /// </exception>
    public virtual ObjectResult<TElement> ExecuteFunction<TElement>(
      string functionName,
      ExecutionOptions executionOptions,
      params ObjectParameter[] parameters)
    {
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      Check.NotEmpty(functionName, nameof (functionName));
      this.AsyncMonitor.EnsureNotEntered();
      EdmFunction functionImport;
      EntityCommand entityCommand = this.CreateEntityCommandForFunctionImport(functionName, out functionImport, parameters);
      int length = Math.Max(1, functionImport.ReturnParameters.Count);
      EdmType[] expectedEdmTypes = new EdmType[length];
      expectedEdmTypes[0] = MetadataHelper.GetAndCheckFunctionImportReturnType<TElement>(functionImport, 0, this.MetadataWorkspace);
      for (int resultSetIndex = 1; resultSetIndex < length; ++resultSetIndex)
      {
        if (!MetadataHelper.TryGetFunctionImportReturnType<EdmType>(functionImport, resultSetIndex, out expectedEdmTypes[resultSetIndex]))
          throw EntityUtil.ExecuteFunctionCalledWithNonReaderFunction(functionImport);
      }
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      if (executionStrategy.RetriesOnFailure && executionOptions.UserSpecifiedStreaming.HasValue && executionOptions.UserSpecifiedStreaming.Value)
        throw new InvalidOperationException(Strings.ExecutionStrategy_StreamingNotSupported((object) executionStrategy.GetType().Name));
      if (!executionOptions.UserSpecifiedStreaming.HasValue)
        executionOptions = new ExecutionOptions(executionOptions.MergeOption, !executionStrategy.RetriesOnFailure);
      bool startLocalTransaction = !executionOptions.UserSpecifiedStreaming.Value && this._options.EnsureTransactionsForFunctionsAndCommands;
      return executionStrategy.Execute<ObjectResult<TElement>>((Func<ObjectResult<TElement>>) (() => this.ExecuteInTransaction<ObjectResult<TElement>>((Func<ObjectResult<TElement>>) (() => this.CreateFunctionObjectResult<TElement>(entityCommand, functionImport.EntitySets, expectedEdmTypes, executionOptions)), executionStrategy, startLocalTransaction, !executionOptions.UserSpecifiedStreaming.Value)));
    }

    /// <summary>Executes a stored procedure or function that is defined in the data source and expressed in the conceptual model; discards any results returned from the function; and returns the number of rows affected by the execution.</summary>
    /// <returns>The number of rows affected.</returns>
    /// <param name="functionName">The name of the stored procedure or function. The name can include the container name, such as &lt;Container Name&gt;.&lt;Function Name&gt;. When the default container name is known, only the function name is required.</param>
    /// <param name="parameters">
    /// An array of <see cref="T:System.Data.Entity.Core.Objects.ObjectParameter" /> objects. If output parameters are used,
    /// their values will not be available until the results have been read completely. This is due to the underlying
    /// behavior of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <exception cref="T:System.ArgumentException"> function  is null or empty or function  is not found.</exception>
    /// <exception cref="T:System.InvalidOperationException">The entity reader does not support this  function or there is a type mismatch on the reader and the  function .</exception>
    public virtual int ExecuteFunction(string functionName, params ObjectParameter[] parameters)
    {
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      Check.NotEmpty(functionName, nameof (functionName));
      this.AsyncMonitor.EnsureNotEntered();
      EdmFunction functionImport;
      EntityCommand entityCommand = this.CreateEntityCommandForFunctionImport(functionName, out functionImport, parameters);
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      return executionStrategy.Execute<int>((Func<int>) (() => this.ExecuteInTransaction<int>((Func<int>) (() => ObjectContext.ExecuteFunctionCommand(entityCommand)), executionStrategy, this._options.EnsureTransactionsForFunctionsAndCommands, true)));
    }

    private static int ExecuteFunctionCommand(EntityCommand entityCommand)
    {
      entityCommand.Prepare();
      try
      {
        return entityCommand.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableEntityExceptionType())
          throw new EntityCommandExecutionException(Strings.EntityClient_CommandExecutionFailed, ex);
        throw;
      }
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    private EntityCommand CreateEntityCommandForFunctionImport(
      string functionName,
      out EdmFunction functionImport,
      params ObjectParameter[] parameters)
    {
      for (int index = 0; index < parameters.Length; ++index)
      {
        if (parameters[index] == null)
          throw new InvalidOperationException(Strings.ObjectContext_ExecuteFunctionCalledWithNullParameter((object) index));
      }
      string containerName;
      string functionImportName;
      functionImport = MetadataHelper.GetFunctionImport(functionName, this.DefaultContainerName, this.MetadataWorkspace, out containerName, out functionImportName);
      EntityConnection connection = (EntityConnection) this.Connection;
      EntityCommand command = new EntityCommand(this.InterceptionContext);
      command.CommandType = CommandType.StoredProcedure;
      command.CommandText = containerName + "." + functionImportName;
      command.Connection = connection;
      if (this.CommandTimeout.HasValue)
        command.CommandTimeout = this.CommandTimeout.Value;
      this.PopulateFunctionImportEntityCommandParameters(parameters, functionImport, command);
      return command;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reader disposed by the returned ObjectResult")]
    private ObjectResult<TElement> CreateFunctionObjectResult<TElement>(
      EntityCommand entityCommand,
      ReadOnlyCollection<EntitySet> entitySets,
      EdmType[] edmTypes,
      ExecutionOptions executionOptions)
    {
      EntityCommandDefinition commandDefinition = entityCommand.GetCommandDefinition();
      DbDataReader dbDataReader;
      try
      {
        dbDataReader = commandDefinition.ExecuteStoreCommands(entityCommand, executionOptions.UserSpecifiedStreaming.Value ? CommandBehavior.Default : CommandBehavior.SequentialAccess);
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableEntityExceptionType())
          throw new EntityCommandExecutionException(Strings.EntityClient_CommandExecutionFailed, ex);
        throw;
      }
      ShaperFactory<TElement> shaperFactory = (ShaperFactory<TElement>) null;
      if (!executionOptions.UserSpecifiedStreaming.Value)
      {
        BufferedDataReader bufferedDataReader = (BufferedDataReader) null;
        try
        {
          StoreItemCollection itemCollection = (StoreItemCollection) this.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
          DbProviderServices service = DbConfiguration.DependencyResolver.GetService<DbProviderServices>((object) itemCollection.ProviderInvariantName);
          shaperFactory = this._translator.TranslateColumnMap<TElement>(commandDefinition.CreateColumnMap(dbDataReader, 0), this.MetadataWorkspace, (SpanIndex) null, executionOptions.MergeOption, false, false);
          bufferedDataReader = new BufferedDataReader(dbDataReader);
          bufferedDataReader.Initialize(itemCollection.ProviderManifestToken, service, shaperFactory.ColumnTypes, shaperFactory.NullableColumns);
          dbDataReader = (DbDataReader) bufferedDataReader;
        }
        catch (Exception ex)
        {
          bufferedDataReader?.Dispose();
          throw;
        }
      }
      return this.MaterializedDataRecord<TElement>(entityCommand, dbDataReader, 0, entitySets, edmTypes, shaperFactory, executionOptions.MergeOption, executionOptions.UserSpecifiedStreaming.Value);
    }

    internal ObjectResult<TElement> MaterializedDataRecord<TElement>(
      EntityCommand entityCommand,
      DbDataReader storeReader,
      int resultSetIndex,
      ReadOnlyCollection<EntitySet> entitySets,
      EdmType[] edmTypes,
      ShaperFactory<TElement> shaperFactory,
      MergeOption mergeOption,
      bool streaming)
    {
      EntityCommandDefinition commandDefinition = entityCommand.GetCommandDefinition();
      try
      {
        bool readerOwned = edmTypes.Length <= resultSetIndex + 1;
        EntitySet singleEntitySet = entitySets.Count > resultSetIndex ? entitySets[resultSetIndex] : (EntitySet) null;
        if (shaperFactory == null)
          shaperFactory = this._translator.TranslateColumnMap<TElement>(commandDefinition.CreateColumnMap(storeReader, resultSetIndex), this.MetadataWorkspace, (SpanIndex) null, mergeOption, streaming, false);
        Shaper<TElement> shaper = shaperFactory.Create(storeReader, this, this.MetadataWorkspace, mergeOption, readerOwned, streaming);
        bool onReaderDisposeHasRun = false;
        Action<object, EventArgs> onReaderDispose = (Action<object, EventArgs>) ((sender, e) =>
        {
          if (onReaderDisposeHasRun)
            return;
          onReaderDisposeHasRun = true;
          CommandHelper.ConsumeReader(storeReader);
          entityCommand.NotifyDataReaderClosing();
        });
        NextResultGenerator nextResultGenerator;
        if (readerOwned)
        {
          shaper.OnDone += new EventHandler(onReaderDispose.Invoke);
          nextResultGenerator = (NextResultGenerator) null;
        }
        else
          nextResultGenerator = new NextResultGenerator(this, entityCommand, edmTypes, entitySets, mergeOption, streaming, resultSetIndex + 1);
        return new ObjectResult<TElement>(shaper, singleEntitySet, TypeUsage.Create(edmTypes[resultSetIndex]), true, streaming, nextResultGenerator, onReaderDispose, (DbCommand) null);
      }
      catch
      {
        this.ReleaseConnection();
        storeReader.Dispose();
        throw;
      }
    }

    private void PopulateFunctionImportEntityCommandParameters(
      ObjectParameter[] parameters,
      EdmFunction functionImport,
      EntityCommand command)
    {
      for (int ordinal = 0; ordinal < parameters.Length; ++ordinal)
      {
        ObjectParameter parameter = parameters[ordinal];
        EntityParameter entityParameter = new EntityParameter();
        FunctionParameter parameterMetadata = ObjectContext.FindParameterMetadata(functionImport, parameters, ordinal);
        if (parameterMetadata != null)
        {
          entityParameter.Direction = MetadataHelper.ParameterModeToParameterDirection(parameterMetadata.Mode);
          entityParameter.ParameterName = parameterMetadata.Name;
        }
        else
          entityParameter.ParameterName = parameter.Name;
        entityParameter.Value = parameter.Value ?? (object) DBNull.Value;
        if (DBNull.Value == entityParameter.Value || entityParameter.Direction != ParameterDirection.Input)
        {
          TypeUsage typeUsage;
          if (parameterMetadata != null)
            typeUsage = parameterMetadata.TypeUsage;
          else if (parameter.TypeUsage == null)
          {
            if (!this.Perspective.TryGetTypeByName(parameter.MappableType.FullNameWithNesting(), false, out typeUsage))
            {
              this.MetadataWorkspace.ImplicitLoadAssemblyForType(parameter.MappableType, (Assembly) null);
              this.Perspective.TryGetTypeByName(parameter.MappableType.FullNameWithNesting(), false, out typeUsage);
            }
          }
          else
            typeUsage = parameter.TypeUsage;
          EntityCommandDefinition.PopulateParameterFromTypeUsage(entityParameter, typeUsage, entityParameter.Direction != ParameterDirection.Input);
        }
        if (entityParameter.Direction != ParameterDirection.Input)
        {
          ObjectContext.ParameterBinder parameterBinder = new ObjectContext.ParameterBinder(entityParameter, parameter);
          command.OnDataReaderClosing += new EventHandler(parameterBinder.OnDataReaderClosingHandler);
        }
        command.Parameters.Add(entityParameter);
      }
    }

    private static FunctionParameter FindParameterMetadata(
      EdmFunction functionImport,
      ObjectParameter[] parameters,
      int ordinal)
    {
      string name = parameters[ordinal].Name;
      FunctionParameter functionParameter;
      if (!functionImport.Parameters.TryGetValue(name, false, out functionParameter))
      {
        int num = 0;
        for (int index = 0; index < parameters.Length && num < 2; ++index)
        {
          if (StringComparer.OrdinalIgnoreCase.Equals(parameters[index].Name, name))
            ++num;
        }
        if (num == 1)
          functionImport.Parameters.TryGetValue(name, true, out functionParameter);
      }
      return functionParameter;
    }

    /// <summary>Generates an equivalent type that can be used with the Entity Framework for each type in the supplied enumeration.</summary>
    /// <param name="types">
    /// An enumeration of <see cref="T:System.Type" /> objects that represent custom data classes that map to the conceptual model.
    /// </param>
    public virtual void CreateProxyTypes(IEnumerable<Type> types)
    {
      ObjectItemCollection ospaceItems = (ObjectItemCollection) this.MetadataWorkspace.GetItemCollection(DataSpace.OSpace);
      EntityProxyFactory.TryCreateProxyTypes(types.Select<Type, EntityType>((Func<Type, EntityType>) (type =>
      {
        this.MetadataWorkspace.ImplicitLoadAssemblyForType(type, (Assembly) null);
        EntityType entityType;
        ospaceItems.TryGetItem<EntityType>(type.FullNameWithNesting(), out entityType);
        return entityType;
      })).Where<EntityType>((Func<EntityType, bool>) (entityType => entityType != null)), this.MetadataWorkspace);
    }

    /// <summary>Returns all the existing proxy types.</summary>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of all the existing proxy types.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static IEnumerable<Type> GetKnownProxyTypes()
    {
      return EntityProxyFactory.GetKnownProxyTypes();
    }

    /// <summary>Returns the entity type of the POCO entity associated with a proxy object of a specified type.</summary>
    /// <returns>
    /// The <see cref="T:System.Type" /> of the associated POCO entity.
    /// </returns>
    /// <param name="type">
    /// The <see cref="T:System.Type" /> of the proxy object.
    /// </param>
    public static Type GetObjectType(Type type)
    {
      Check.NotNull<Type>(type, nameof (type));
      if (!EntityProxyFactory.IsProxyType(type))
        return type;
      return type.BaseType();
    }

    /// <summary>Creates and returns an instance of the requested type .</summary>
    /// <returns>An instance of the requested type  T , or an instance of a derived type that enables  T  to be used with the Entity Framework. The returned object is either an instance of the requested type or an instance of a derived type that enables the requested type to be used with the Entity Framework.</returns>
    /// <typeparam name="T">Type of object to be returned.</typeparam>
    public virtual T CreateObject<T>() where T : class
    {
      T obj1 = default (T);
      Type type = typeof (T);
      this.MetadataWorkspace.ImplicitLoadAssemblyForType(type, (Assembly) null);
      ClrEntityType clrEntityType = this.MetadataWorkspace.GetItem<ClrEntityType>(type.FullNameWithNesting(), DataSpace.OSpace);
      EntityProxyTypeInfo proxyType;
      T obj2;
      if (this.ContextOptions.ProxyCreationEnabled && (proxyType = EntityProxyFactory.GetProxyType(clrEntityType, this.MetadataWorkspace)) != null)
      {
        obj2 = (T) proxyType.CreateProxyObject();
        IEntityWrapper newWrapper = EntityWrapperFactory.CreateNewWrapper((object) obj2, (EntityKey) null);
        newWrapper.InitializingProxyRelatedEnds = true;
        try
        {
          newWrapper.AttachContext(this, (EntitySet) null, MergeOption.NoTracking);
          proxyType.SetEntityWrapper(newWrapper);
          if ((MethodInfo) proxyType.InitializeEntityCollections != (MethodInfo) null)
            proxyType.InitializeEntityCollections.Invoke((object) null, new object[1]
            {
              (object) newWrapper
            });
        }
        finally
        {
          newWrapper.InitializingProxyRelatedEnds = false;
          newWrapper.DetachContext();
        }
      }
      else
        obj2 = DelegateFactory.GetConstructorDelegateForType(clrEntityType)() as T;
      return obj2;
    }

    /// <summary>
    /// Executes an arbitrary command directly against the data source using the existing connection.
    /// The command is specified using the server's native query language, such as SQL.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// If there isn't an existing local transaction a new transaction will be used
    /// to execute the command.
    /// </remarks>
    /// <param name="commandText">The command specified in the server's native query language.</param>
    /// <param name="parameters"> The parameter values to use for the query. </param>
    /// <returns>The number of rows affected.</returns>
    public virtual int ExecuteStoreCommand(string commandText, params object[] parameters)
    {
      return this.ExecuteStoreCommand(this._options.EnsureTransactionsForFunctionsAndCommands ? TransactionalBehavior.EnsureTransaction : TransactionalBehavior.DoNotEnsureTransaction, commandText, parameters);
    }

    /// <summary>
    /// Executes an arbitrary command directly against the data source using the existing connection.
    /// The command is specified using the server's native query language, such as SQL.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <param name="transactionalBehavior"> Controls the creation of a transaction for this command. </param>
    /// <param name="commandText">The command specified in the server's native query language.</param>
    /// <param name="parameters"> The parameter values to use for the query. </param>
    /// <returns>The number of rows affected.</returns>
    public virtual int ExecuteStoreCommand(
      TransactionalBehavior transactionalBehavior,
      string commandText,
      params object[] parameters)
    {
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      this.AsyncMonitor.EnsureNotEntered();
      return executionStrategy.Execute<int>((Func<int>) (() => this.ExecuteInTransaction<int>((Func<int>) (() => this.CreateStoreCommand(commandText, parameters).ExecuteNonQuery()), executionStrategy, transactionalBehavior != TransactionalBehavior.DoNotEnsureTransaction, true)));
    }

    /// <summary>
    /// Asynchronously executes an arbitrary command directly against the data source using the existing connection.
    /// The command is specified using the server's native query language, such as SQL.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// 
    /// If there isn't an existing local transaction a new transaction will be used
    /// to execute the command.
    /// </remarks>
    /// <param name="commandText">The command specified in the server's native query language.</param>
    /// <param name="parameters"> The parameter values to use for the query. </param>
    /// <returns>
    ///  A task that represents the asynchronous operation.
    /// The task result contains the number of rows affected.
    /// </returns>
    public Task<int> ExecuteStoreCommandAsync(string commandText, params object[] parameters)
    {
      return this.ExecuteStoreCommandAsync(this._options.EnsureTransactionsForFunctionsAndCommands ? TransactionalBehavior.EnsureTransaction : TransactionalBehavior.DoNotEnsureTransaction, commandText, CancellationToken.None, parameters);
    }

    /// <summary>
    /// Asynchronously executes an arbitrary command directly against the data source using the existing connection.
    /// The command is specified using the server's native query language, such as SQL.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="transactionalBehavior"> Controls the creation of a transaction for this command. </param>
    /// <param name="commandText">The command specified in the server's native query language.</param>
    /// <param name="parameters"> The parameter values to use for the query. </param>
    /// <returns>
    ///  A task that represents the asynchronous operation.
    /// The task result contains the number of rows affected.
    /// </returns>
    public Task<int> ExecuteStoreCommandAsync(
      TransactionalBehavior transactionalBehavior,
      string commandText,
      params object[] parameters)
    {
      return this.ExecuteStoreCommandAsync(transactionalBehavior, commandText, CancellationToken.None, parameters);
    }

    /// <summary>
    /// Asynchronously executes an arbitrary command directly against the data source using the existing connection.
    /// The command is specified using the server's native query language, such as SQL.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// 
    /// If there isn't an existing local transaction a new transaction will be used
    /// to execute the command.
    /// </remarks>
    /// <param name="commandText">The command specified in the server's native query language.</param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="parameters"> The parameter values to use for the query. </param>
    /// <returns>
    ///  A task that represents the asynchronous operation.
    /// The task result contains the number of rows affected.
    /// </returns>
    public virtual Task<int> ExecuteStoreCommandAsync(
      string commandText,
      CancellationToken cancellationToken,
      params object[] parameters)
    {
      return this.ExecuteStoreCommandAsync(this._options.EnsureTransactionsForFunctionsAndCommands ? TransactionalBehavior.EnsureTransaction : TransactionalBehavior.DoNotEnsureTransaction, commandText, cancellationToken, parameters);
    }

    /// <summary>
    /// Asynchronously executes an arbitrary command directly against the data source using the existing connection.
    /// The command is specified using the server's native query language, such as SQL.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="transactionalBehavior"> Controls the creation of a transaction for this command. </param>
    /// <param name="commandText">The command specified in the server's native query language.</param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="parameters"> The parameter values to use for the query. </param>
    /// <returns>
    ///  A task that represents the asynchronous operation.
    /// The task result contains the number of rows affected.
    /// </returns>
    public virtual Task<int> ExecuteStoreCommandAsync(
      TransactionalBehavior transactionalBehavior,
      string commandText,
      CancellationToken cancellationToken,
      params object[] parameters)
    {
      cancellationToken.ThrowIfCancellationRequested();
      this.AsyncMonitor.EnsureNotEntered();
      return this.ExecuteStoreCommandInternalAsync(transactionalBehavior, commandText, cancellationToken, parameters);
    }

    private async Task<int> ExecuteStoreCommandInternalAsync(
      TransactionalBehavior transactionalBehavior,
      string commandText,
      CancellationToken cancellationToken,
      params object[] parameters)
    {
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      this.AsyncMonitor.Enter();
      int num;
      try
      {
        num = await executionStrategy.ExecuteAsync<int>((Func<Task<int>>) (() => this.ExecuteInTransactionAsync<int>((Func<Task<int>>) (() => this.CreateStoreCommand(commandText, parameters).ExecuteNonQueryAsync(cancellationToken)), executionStrategy, transactionalBehavior != TransactionalBehavior.DoNotEnsureTransaction, true, cancellationToken)), cancellationToken).WithCurrentCulture<int>();
      }
      finally
      {
        this.AsyncMonitor.Exit();
      }
      return num;
    }

    /// <summary>
    /// Executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// Results are not tracked by the context, use the overload that specifies an entity set name to track results.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// An enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    public virtual ObjectResult<TElement> ExecuteStoreQuery<TElement>(
      string commandText,
      params object[] parameters)
    {
      return this.ExecuteStoreQueryReliably<TElement>(commandText, (string) null, ExecutionOptions.Default, parameters);
    }

    /// <summary>
    /// Executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// Results are not tracked by the context, use the overload that specifies an entity set name to track results.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="executionOptions"> The options for executing this query. </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior of
    /// DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// An enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    public virtual ObjectResult<TElement> ExecuteStoreQuery<TElement>(
      string commandText,
      ExecutionOptions executionOptions,
      params object[] parameters)
    {
      return this.ExecuteStoreQueryReliably<TElement>(commandText, (string) null, executionOptions, parameters);
    }

    /// <summary>
    /// Executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// If an entity set name is specified, results are tracked by the context.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="entitySetName">The entity set of the  TResult  type. If an entity set name is not provided, the results are not going to be tracked.</param>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> to use when executing the query. The default is
    /// <see cref="F:System.Data.Entity.Core.Objects.MergeOption.AppendOnly" />.
    /// </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// An enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    public virtual ObjectResult<TElement> ExecuteStoreQuery<TElement>(
      string commandText,
      string entitySetName,
      MergeOption mergeOption,
      params object[] parameters)
    {
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      return this.ExecuteStoreQueryReliably<TElement>(commandText, entitySetName, new ExecutionOptions(mergeOption), parameters);
    }

    /// <summary>
    /// Executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// If an entity set name is specified, results are tracked by the context.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQuery&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="entitySetName">The entity set of the  TResult  type. If an entity set name is not provided, the results are not going to be tracked.</param>
    /// <param name="executionOptions"> The options for executing this query. </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// An enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    public virtual ObjectResult<TElement> ExecuteStoreQuery<TElement>(
      string commandText,
      string entitySetName,
      ExecutionOptions executionOptions,
      params object[] parameters)
    {
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      return this.ExecuteStoreQueryReliably<TElement>(commandText, entitySetName, executionOptions, parameters);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Buffer disposed by the returned ObjectResult")]
    private ObjectResult<TElement> ExecuteStoreQueryReliably<TElement>(
      string commandText,
      string entitySetName,
      ExecutionOptions executionOptions,
      params object[] parameters)
    {
      this.AsyncMonitor.EnsureNotEntered();
      this.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TElement), Assembly.GetCallingAssembly());
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      if (executionStrategy.RetriesOnFailure && executionOptions.UserSpecifiedStreaming.HasValue && executionOptions.UserSpecifiedStreaming.Value)
        throw new InvalidOperationException(Strings.ExecutionStrategy_StreamingNotSupported((object) executionStrategy.GetType().Name));
      if (!executionOptions.UserSpecifiedStreaming.HasValue)
        executionOptions = new ExecutionOptions(executionOptions.MergeOption, !executionStrategy.RetriesOnFailure);
      return executionStrategy.Execute<ObjectResult<TElement>>((Func<ObjectResult<TElement>>) (() => this.ExecuteInTransaction<ObjectResult<TElement>>((Func<ObjectResult<TElement>>) (() => this.ExecuteStoreQueryInternal<TElement>(commandText, entitySetName, executionOptions, parameters)), executionStrategy, false, !executionOptions.UserSpecifiedStreaming.Value)));
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by ObjectResult")]
    private ObjectResult<TElement> ExecuteStoreQueryInternal<TElement>(
      string commandText,
      string entitySetName,
      ExecutionOptions executionOptions,
      params object[] parameters)
    {
      DbDataReader reader = (DbDataReader) null;
      DbCommand command = (DbCommand) null;
      EntitySet entitySet;
      TypeUsage edmType;
      ShaperFactory<TElement> shaperFactory;
      try
      {
        command = this.CreateStoreCommand(commandText, parameters);
        reader = command.ExecuteReader(executionOptions.UserSpecifiedStreaming.Value ? CommandBehavior.Default : CommandBehavior.SequentialAccess);
        shaperFactory = this.InternalTranslate<TElement>(reader, entitySetName, executionOptions.MergeOption, executionOptions.UserSpecifiedStreaming.Value, out entitySet, out edmType);
      }
      catch
      {
        reader?.Dispose();
        command?.Dispose();
        throw;
      }
      if (!executionOptions.UserSpecifiedStreaming.Value)
      {
        BufferedDataReader bufferedDataReader = (BufferedDataReader) null;
        try
        {
          StoreItemCollection itemCollection = (StoreItemCollection) this.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
          DbProviderServices service = DbConfiguration.DependencyResolver.GetService<DbProviderServices>((object) itemCollection.ProviderInvariantName);
          bufferedDataReader = new BufferedDataReader(reader);
          bufferedDataReader.Initialize(itemCollection.ProviderManifestToken, service, shaperFactory.ColumnTypes, shaperFactory.NullableColumns);
          reader = (DbDataReader) bufferedDataReader;
        }
        catch
        {
          bufferedDataReader?.Dispose();
          throw;
        }
      }
      return this.ShapeResult<TElement>(reader, executionOptions.MergeOption, true, executionOptions.UserSpecifiedStreaming.Value, shaperFactory, entitySet, edmType, command);
    }

    /// <summary>
    /// Asynchronously executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// Results are not tracked by the context, use the overload that specifies an entity set name to track results.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public Task<ObjectResult<TElement>> ExecuteStoreQueryAsync<TElement>(
      string commandText,
      params object[] parameters)
    {
      return this.ExecuteStoreQueryAsync<TElement>(commandText, CancellationToken.None, parameters);
    }

    /// <summary>
    /// Asynchronously executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// Results are not tracked by the context, use the overload that specifies an entity set name to track results.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual Task<ObjectResult<TElement>> ExecuteStoreQueryAsync<TElement>(
      string commandText,
      CancellationToken cancellationToken,
      params object[] parameters)
    {
      this.AsyncMonitor.EnsureNotEntered();
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      return this.ExecuteStoreQueryReliablyAsync<TElement>(commandText, (string) null, ExecutionOptions.Default, cancellationToken, executionStrategy, parameters);
    }

    /// <summary>
    /// Asynchronously executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// Results are not tracked by the context, use the overload that specifies an entity set name to track results.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="executionOptions"> The options for executing this query. </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual Task<ObjectResult<TElement>> ExecuteStoreQueryAsync<TElement>(
      string commandText,
      ExecutionOptions executionOptions,
      params object[] parameters)
    {
      return this.ExecuteStoreQueryAsync<TElement>(commandText, executionOptions, CancellationToken.None, parameters);
    }

    /// <summary>
    /// Asynchronously executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// Results are not tracked by the context, use the overload that specifies an entity set name to track results.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="executionOptions"> The options for executing this query. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual Task<ObjectResult<TElement>> ExecuteStoreQueryAsync<TElement>(
      string commandText,
      ExecutionOptions executionOptions,
      CancellationToken cancellationToken,
      params object[] parameters)
    {
      this.AsyncMonitor.EnsureNotEntered();
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      if (executionStrategy.RetriesOnFailure && executionOptions.UserSpecifiedStreaming.HasValue && executionOptions.UserSpecifiedStreaming.Value)
        throw new InvalidOperationException(Strings.ExecutionStrategy_StreamingNotSupported((object) executionStrategy.GetType().Name));
      return this.ExecuteStoreQueryReliablyAsync<TElement>(commandText, (string) null, executionOptions, cancellationToken, executionStrategy, parameters);
    }

    /// <summary>
    /// Asynchronously executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// If an entity set name is specified, results are tracked by the context.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="entitySetName">The entity set of the  TResult  type. If an entity set name is not provided, the results are not going to be tracked.</param>
    /// <param name="executionOptions"> The options for executing this query. </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public Task<ObjectResult<TElement>> ExecuteStoreQueryAsync<TElement>(
      string commandText,
      string entitySetName,
      ExecutionOptions executionOptions,
      params object[] parameters)
    {
      return this.ExecuteStoreQueryAsync<TElement>(commandText, entitySetName, executionOptions, CancellationToken.None, parameters);
    }

    /// <summary>
    /// Asynchronously executes a query directly against the data source and returns a sequence of typed results.
    /// The query is specified using the server's native query language, such as SQL.
    /// If an entity set name is specified, results are tracked by the context.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.ExecuteStoreQueryAsync&lt;Post&gt;("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TElement"> The element type of the result sequence. </typeparam>
    /// <param name="commandText"> The query specified in the server's native query language. </param>
    /// <param name="entitySetName">The entity set of the  TResult  type. If an entity set name is not provided, the results are not going to be tracked.</param>
    /// <param name="executionOptions"> The options for executing this query. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="parameters">
    /// The parameter values to use for the query. If output parameters are used, their values will not be
    /// available until the results have been read completely. This is due to the underlying behavior
    /// of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumeration of objects of type <typeparamref name="TElement" /> .
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual Task<ObjectResult<TElement>> ExecuteStoreQueryAsync<TElement>(
      string commandText,
      string entitySetName,
      ExecutionOptions executionOptions,
      CancellationToken cancellationToken,
      params object[] parameters)
    {
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      this.AsyncMonitor.EnsureNotEntered();
      IDbExecutionStrategy executionStrategy = DbProviderServices.GetExecutionStrategy(this.Connection, this.MetadataWorkspace);
      if (executionStrategy.RetriesOnFailure && executionOptions.UserSpecifiedStreaming.HasValue && executionOptions.UserSpecifiedStreaming.Value)
        throw new InvalidOperationException(Strings.ExecutionStrategy_StreamingNotSupported((object) executionStrategy.GetType().Name));
      return this.ExecuteStoreQueryReliablyAsync<TElement>(commandText, entitySetName, executionOptions, cancellationToken, executionStrategy, parameters);
    }

    private async Task<ObjectResult<TElement>> ExecuteStoreQueryReliablyAsync<TElement>(
      string commandText,
      string entitySetName,
      ExecutionOptions executionOptions,
      CancellationToken cancellationToken,
      IDbExecutionStrategy executionStrategy,
      params object[] parameters)
    {
      if (executionOptions.MergeOption != MergeOption.NoTracking)
        this.AsyncMonitor.Enter();
      ObjectResult<TElement> objectResult;
      try
      {
        this.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TElement), Assembly.GetCallingAssembly());
        if (!executionOptions.UserSpecifiedStreaming.HasValue)
          executionOptions = new ExecutionOptions(executionOptions.MergeOption, !executionStrategy.RetriesOnFailure);
        objectResult = await executionStrategy.ExecuteAsync<ObjectResult<TElement>>((Func<Task<ObjectResult<TElement>>>) (() => this.ExecuteInTransactionAsync<ObjectResult<TElement>>((Func<Task<ObjectResult<TElement>>>) (() => this.ExecuteStoreQueryInternalAsync<TElement>(commandText, entitySetName, executionOptions, cancellationToken, parameters)), executionStrategy, false, !executionOptions.UserSpecifiedStreaming.Value, cancellationToken)), cancellationToken).WithCurrentCulture<ObjectResult<TElement>>();
      }
      finally
      {
        if (executionOptions.MergeOption != MergeOption.NoTracking)
          this.AsyncMonitor.Exit();
      }
      return objectResult;
    }

    private async Task<ObjectResult<TElement>> ExecuteStoreQueryInternalAsync<TElement>(
      string commandText,
      string entitySetName,
      ExecutionOptions executionOptions,
      CancellationToken cancellationToken,
      params object[] parameters)
    {
      DbDataReader reader = (DbDataReader) null;
      DbCommand command = (DbCommand) null;
      EntitySet entitySet;
      TypeUsage edmType;
      ShaperFactory<TElement> shaperFactory;
      try
      {
        command = this.CreateStoreCommand(commandText, parameters);
        reader = await command.ExecuteReaderAsync(executionOptions.UserSpecifiedStreaming.Value ? CommandBehavior.Default : CommandBehavior.SequentialAccess, cancellationToken).WithCurrentCulture<DbDataReader>();
        shaperFactory = this.InternalTranslate<TElement>(reader, entitySetName, executionOptions.MergeOption, executionOptions.UserSpecifiedStreaming.Value, out entitySet, out edmType);
      }
      catch
      {
        reader?.Dispose();
        command?.Dispose();
        throw;
      }
      if (!executionOptions.UserSpecifiedStreaming.Value)
      {
        BufferedDataReader bufferedReader = (BufferedDataReader) null;
        try
        {
          StoreItemCollection storeItemCollection = (StoreItemCollection) this.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
          DbProviderServices providerServices = DbConfiguration.DependencyResolver.GetService<DbProviderServices>((object) storeItemCollection.ProviderInvariantName);
          bufferedReader = new BufferedDataReader(reader);
          await bufferedReader.InitializeAsync(storeItemCollection.ProviderManifestToken, providerServices, shaperFactory.ColumnTypes, shaperFactory.NullableColumns, cancellationToken).WithCurrentCulture();
          reader = (DbDataReader) bufferedReader;
        }
        catch
        {
          bufferedReader?.Dispose();
          throw;
        }
      }
      return this.ShapeResult<TElement>(reader, executionOptions.MergeOption, true, executionOptions.UserSpecifiedStreaming.Value, shaperFactory, entitySet, edmType, command);
    }

    /// <summary>
    /// Translates a <see cref="T:System.Data.Common.DbDataReader" /> that contains rows of entity data to objects of the requested entity type.
    /// </summary>
    /// <typeparam name="TElement">The entity type.</typeparam>
    /// <returns>An enumeration of objects of type  TResult .</returns>
    /// <param name="reader">
    /// The <see cref="T:System.Data.Common.DbDataReader" /> that contains entity data to translate into entity objects.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">When  reader  is null.</exception>
    public virtual ObjectResult<TElement> Translate<TElement>(DbDataReader reader)
    {
      this.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TElement), Assembly.GetCallingAssembly());
      EntitySet entitySet;
      TypeUsage edmType;
      ShaperFactory<TElement> shaperFactory = this.InternalTranslate<TElement>(reader, (string) null, MergeOption.AppendOnly, false, out entitySet, out edmType);
      return this.ShapeResult<TElement>(reader, MergeOption.AppendOnly, false, false, shaperFactory, entitySet, edmType, (DbCommand) null);
    }

    /// <summary>
    /// Translates a <see cref="T:System.Data.Common.DbDataReader" /> that contains rows of entity data to objects of the requested entity type, in a specific entity set, and with the specified merge option.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>An enumeration of objects of type  TResult .</returns>
    /// <param name="reader">
    /// The <see cref="T:System.Data.Common.DbDataReader" /> that contains entity data to translate into entity objects.
    /// </param>
    /// <param name="entitySetName">The entity set of the  TResult  type.</param>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> to use when translated objects are added to the object context. The default is
    /// <see cref="F:System.Data.Entity.Core.Objects.MergeOption.AppendOnly" />
    /// .
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">When  reader  is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// When the supplied  mergeOption  is not a valid <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> value.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">When the supplied  entitySetName  is not a valid entity set for the  TResult  type. </exception>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generic parameters are required for strong-typing of the return type.")]
    public virtual ObjectResult<TEntity> Translate<TEntity>(
      DbDataReader reader,
      string entitySetName,
      MergeOption mergeOption)
    {
      Check.NotEmpty(entitySetName, nameof (entitySetName));
      this.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TEntity), Assembly.GetCallingAssembly());
      EntitySet entitySet;
      TypeUsage edmType;
      ShaperFactory<TEntity> shaperFactory = this.InternalTranslate<TEntity>(reader, entitySetName, mergeOption, false, out entitySet, out edmType);
      return this.ShapeResult<TEntity>(reader, mergeOption, false, false, shaperFactory, entitySet, edmType, (DbCommand) null);
    }

    private ShaperFactory<TElement> InternalTranslate<TElement>(
      DbDataReader reader,
      string entitySetName,
      MergeOption mergeOption,
      bool streaming,
      out EntitySet entitySet,
      out TypeUsage edmType)
    {
      EntityUtil.CheckArgumentMergeOption(mergeOption);
      entitySet = (EntitySet) null;
      if (!string.IsNullOrEmpty(entitySetName))
        entitySet = this.GetEntitySetFromName(entitySetName);
      Type type1 = Nullable.GetUnderlyingType(typeof (TElement));
      if ((object) type1 == null)
        type1 = typeof (TElement);
      Type type2 = type1;
      EdmType modelEdmType;
      CollectionColumnMap collectionColumnMap;
      if (this.MetadataWorkspace.TryDetermineCSpaceModelType<TElement>(out modelEdmType) || type2.IsEnum() && this.MetadataWorkspace.TryDetermineCSpaceModelType(type2.GetEnumUnderlyingType(), out modelEdmType))
      {
        if (entitySet != null && !entitySet.ElementType.IsAssignableFrom(modelEdmType))
          throw new InvalidOperationException(Strings.ObjectContext_InvalidEntitySetForStoreQuery((object) entitySet.EntityContainer.Name, (object) entitySet.Name, (object) typeof (TElement)));
        collectionColumnMap = this._columnMapFactory.CreateColumnMapFromReaderAndType(reader, modelEdmType, entitySet, (Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping>) null);
      }
      else
        collectionColumnMap = this._columnMapFactory.CreateColumnMapFromReaderAndClrType(reader, typeof (TElement), this.MetadataWorkspace);
      edmType = collectionColumnMap.Type;
      return this._translator.TranslateColumnMap<TElement>((ColumnMap) collectionColumnMap, this.MetadataWorkspace, (SpanIndex) null, mergeOption, streaming, false);
    }

    private ObjectResult<TElement> ShapeResult<TElement>(
      DbDataReader reader,
      MergeOption mergeOption,
      bool readerOwned,
      bool streaming,
      ShaperFactory<TElement> shaperFactory,
      EntitySet entitySet,
      TypeUsage edmType,
      DbCommand command = null)
    {
      return new ObjectResult<TElement>(shaperFactory.Create(reader, this, this.MetadataWorkspace, mergeOption, readerOwned, streaming), entitySet, MetadataHelper.GetElementType(edmType), readerOwned, streaming, command);
    }

    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    private DbCommand CreateStoreCommand(string commandText, params object[] parameters)
    {
      DbCommand command = ((EntityConnection) this.Connection).StoreConnection.CreateCommand();
      command.CommandText = commandText;
      if (this.CommandTimeout.HasValue)
        command.CommandTimeout = this.CommandTimeout.Value;
      EntityTransaction currentTransaction = ((EntityConnection) this.Connection).CurrentTransaction;
      if (currentTransaction != null)
        command.Transaction = currentTransaction.StoreTransaction;
      if (parameters != null && parameters.Length > 0)
      {
        DbParameter[] dbParameterArray = new DbParameter[parameters.Length];
        if (((IEnumerable<object>) parameters).All<object>((Func<object, bool>) (p => p is DbParameter)))
        {
          for (int index = 0; index < parameters.Length; ++index)
            dbParameterArray[index] = (DbParameter) parameters[index];
        }
        else
        {
          if (((IEnumerable<object>) parameters).Any<object>((Func<object, bool>) (p => p is DbParameter)))
            throw new InvalidOperationException(Strings.ObjectContext_ExecuteCommandWithMixOfDbParameterAndValues);
          string[] strArray1 = new string[parameters.Length];
          string[] strArray2 = new string[parameters.Length];
          for (int index = 0; index < parameters.Length; ++index)
          {
            strArray1[index] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "p{0}", (object) index);
            dbParameterArray[index] = command.CreateParameter();
            dbParameterArray[index].ParameterName = strArray1[index];
            dbParameterArray[index].Value = parameters[index] ?? (object) DBNull.Value;
            strArray2[index] = "@" + strArray1[index];
          }
          command.CommandText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, command.CommandText, (object[]) strArray2);
        }
        command.Parameters.AddRange((Array) dbParameterArray);
      }
      return (DbCommand) new InterceptableDbCommand(command, this.InterceptionContext, (DbDispatchers) null);
    }

    /// <summary>
    /// Creates the database by using the current data source connection and the metadata in the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.StoreItemCollection" />
    /// .
    /// </summary>
    public virtual void CreateDatabase()
    {
      this.GetStoreItemCollection().ProviderFactory.GetProviderServices().CreateDatabase(((EntityConnection) this.Connection).StoreConnection, this.CommandTimeout, this.GetStoreItemCollection());
    }

    /// <summary>Deletes the database that is specified as the database in the current data source connection.</summary>
    public virtual void DeleteDatabase()
    {
      this.GetStoreItemCollection().ProviderFactory.GetProviderServices().DeleteDatabase(((EntityConnection) this.Connection).StoreConnection, this.CommandTimeout, this.GetStoreItemCollection());
    }

    /// <summary>
    /// Checks if the database that is specified as the database in the current store connection exists on the store. Most of the actual work
    /// is done by the DbProviderServices implementation for the current store connection.
    /// </summary>
    /// <returns>true if the database exists; otherwise, false.</returns>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public virtual bool DatabaseExists()
    {
      DbConnection storeConnection = ((EntityConnection) this.Connection).StoreConnection;
      DbProviderServices providerServices = this.GetStoreItemCollection().ProviderFactory.GetProviderServices();
      try
      {
        return providerServices.DatabaseExists(storeConnection, this.CommandTimeout, this.GetStoreItemCollection());
      }
      catch (Exception ex1)
      {
        if (this.Connection.State == ConnectionState.Open)
          return true;
        try
        {
          this.Connection.Open();
          return true;
        }
        catch (EntityException ex2)
        {
          return false;
        }
        finally
        {
          this.Connection.Close();
        }
      }
    }

    private StoreItemCollection GetStoreItemCollection()
    {
      return (StoreItemCollection) ((EntityConnection) this.Connection).GetMetadataWorkspace().GetItemCollection(DataSpace.SSpace);
    }

    /// <summary>
    /// Generates a data definition language (DDL) script that creates schema objects (tables, primary keys, foreign keys) for the metadata in the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.StoreItemCollection" />
    /// . The
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.StoreItemCollection" />
    /// loads metadata from store schema definition language (SSDL) files.
    /// </summary>
    /// <returns>
    /// A DDL script that creates schema objects for the metadata in the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.StoreItemCollection" />
    /// .
    /// </returns>
    public virtual string CreateDatabaseScript()
    {
      return this.GetStoreItemCollection().ProviderFactory.GetProviderServices().CreateDatabaseScript(this.GetStoreItemCollection().ProviderManifestToken, this.GetStoreItemCollection());
    }

    internal void InitializeMappingViewCacheFactory(DbContext owner = null)
    {
      StorageMappingItemCollection itemCollection = (StorageMappingItemCollection) this.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
      if (itemCollection == null)
        return;
      Type key = owner != null ? owner.GetType() : this.GetType();
      ObjectContext._contextTypesWithViewCacheInitialized.GetOrAdd(key, (Func<Type, bool>) (t =>
      {
        IEnumerable<DbMappingViewCacheTypeAttribute> source = t.Assembly().GetCustomAttributes<DbMappingViewCacheTypeAttribute>().Where<DbMappingViewCacheTypeAttribute>((Func<DbMappingViewCacheTypeAttribute, bool>) (a => a.ContextType == t));
        int num = source.Count<DbMappingViewCacheTypeAttribute>();
        if (num > 1)
          throw new InvalidOperationException(Strings.DbMappingViewCacheTypeAttribute_MultipleInstancesWithSameContextType((object) t));
        if (num == 1)
          itemCollection.MappingViewCacheFactory = (DbMappingViewCacheFactory) new DefaultDbMappingViewCacheFactory(source.First<DbMappingViewCacheTypeAttribute>().CacheType);
        return true;
      }));
    }

    private class ParameterBinder
    {
      private readonly EntityParameter _entityParameter;
      private readonly ObjectParameter _objectParameter;

      internal ParameterBinder(EntityParameter entityParameter, ObjectParameter objectParameter)
      {
        this._entityParameter = entityParameter;
        this._objectParameter = objectParameter;
      }

      internal void OnDataReaderClosingHandler(object sender, EventArgs args)
      {
        if (this._entityParameter.Value != DBNull.Value && this._objectParameter.MappableType.IsEnum())
          this._objectParameter.Value = Enum.ToObject(this._objectParameter.MappableType, this._entityParameter.Value);
        else
          this._objectParameter.Value = this._entityParameter.Value;
      }
    }
  }
}
