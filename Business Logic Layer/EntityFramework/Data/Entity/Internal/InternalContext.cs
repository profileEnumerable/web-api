// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal.Linq;
using System.Data.Entity.Internal.MockingProxies;
using System.Data.Entity.Internal.Validation;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Migrations.Utilities;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace System.Data.Entity.Internal
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal abstract class InternalContext : IDisposable
  {
    public static readonly MethodInfo CreateObjectAsObjectMethod = typeof (InternalContext).GetOnlyDeclaredMethod("CreateObjectAsObject");
    private static readonly ConcurrentDictionary<Type, Func<InternalContext, object>> _entityFactories = new ConcurrentDictionary<Type, Func<InternalContext, object>>();
    public static readonly MethodInfo ExecuteSqlQueryAsIEnumeratorMethod = typeof (InternalContext).GetOnlyDeclaredMethod("ExecuteSqlQueryAsIEnumerator");
    public static readonly MethodInfo ExecuteSqlQueryAsIDbAsyncEnumeratorMethod = typeof (InternalContext).GetOnlyDeclaredMethod("ExecuteSqlQueryAsIDbAsyncEnumerator");
    private static readonly ConcurrentDictionary<Type, Func<InternalContext, string, bool?, object[], IEnumerator>> _queryExecutors = new ConcurrentDictionary<Type, Func<InternalContext, string, bool?, object[], IEnumerator>>();
    private static readonly ConcurrentDictionary<Type, Func<InternalContext, string, bool?, object[], IDbAsyncEnumerator>> _asyncQueryExecutors = new ConcurrentDictionary<Type, Func<InternalContext, string, bool?, object[], IDbAsyncEnumerator>>();
    private static readonly ConcurrentDictionary<Type, Func<InternalContext, IInternalSet, IInternalSetAdapter>> _setFactories = new ConcurrentDictionary<Type, Func<InternalContext, IInternalSet, IInternalSetAdapter>>();
    public static readonly MethodInfo CreateInitializationActionMethod = typeof (InternalContext).GetOnlyDeclaredMethod("CreateInitializationAction");
    private AppConfig _appConfig = AppConfig.DefaultInstance;
    private readonly Dictionary<Type, IInternalSetAdapter> _genericSets = new Dictionary<Type, IInternalSetAdapter>();
    private readonly Dictionary<Type, IInternalSetAdapter> _nonGenericSets = new Dictionary<Type, IInternalSetAdapter>();
    private readonly ValidationProvider _validationProvider = new ValidationProvider((EntityValidatorBuilder) null, DbConfiguration.DependencyResolver.GetService<AttributeProvider>());
    private readonly DbContext _owner;
    private ClonedObjectContext _tempObjectContext;
    private int _tempObjectContextCount;
    private bool _oSpaceLoadingForced;
    private DbProviderFactory _providerFactory;
    private readonly Lazy<DbDispatchers> _dispatchers;
    private DatabaseLogFormatter _logFormatter;
    private Func<DbMigrationsConfiguration> _migrationsConfiguration;
    private bool? _migrationsConfigurationDiscovered;
    private DbContextInfo _contextInfo;
    private string _defaultContextKey;

    public event EventHandler<EventArgs> OnDisposing;

    protected InternalContext(DbContext owner, Lazy<DbDispatchers> dispatchers = null)
    {
      this._owner = owner;
      this._dispatchers = dispatchers ?? new Lazy<DbDispatchers>((Func<DbDispatchers>) (() => DbInterception.Dispatch));
      this.AutoDetectChangesEnabled = true;
      this.ValidateOnSaveEnabled = true;
    }

    protected InternalContext()
    {
    }

    public DbContext Owner
    {
      get
      {
        return this._owner;
      }
    }

    public abstract ObjectContext ObjectContext { get; }

    public abstract ObjectContext GetObjectContextWithoutDatabaseInitialization();

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public virtual ClonedObjectContext CreateObjectContextForDdlOps()
    {
      this.InitializeContext();
      return new ClonedObjectContext(new ObjectContextProxy(this.GetObjectContextWithoutDatabaseInitialization()), this.Connection, this.OriginalConnectionString, false);
    }

    protected ObjectContext TempObjectContext
    {
      get
      {
        return (ObjectContext) (this._tempObjectContext == null ? (ObjectContextProxy) null : this._tempObjectContext.ObjectContext);
      }
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public virtual void UseTempObjectContext()
    {
      ++this._tempObjectContextCount;
      if (this._tempObjectContext != null)
        return;
      this._tempObjectContext = new ClonedObjectContext(new ObjectContextProxy(this.GetObjectContextWithoutDatabaseInitialization()), this.Connection, this.OriginalConnectionString, true);
      this.ResetDbSets();
    }

    public virtual void DisposeTempObjectContext()
    {
      if (this._tempObjectContextCount <= 0 || (--this._tempObjectContextCount != 0 || this._tempObjectContext == null))
        return;
      this._tempObjectContext.Dispose();
      this._tempObjectContext = (ClonedObjectContext) null;
      this.ResetDbSets();
    }

    public virtual DbCompiledModel CodeFirstModel
    {
      get
      {
        return (DbCompiledModel) null;
      }
    }

    public virtual DbModel ModelBeingInitialized
    {
      get
      {
        return (DbModel) null;
      }
    }

    public virtual void CreateDatabase(
      ObjectContext objectContext,
      DatabaseExistenceState existenceState)
    {
      new DatabaseCreator().CreateDatabase(this, (Func<DbMigrationsConfiguration, DbContext, MigratorBase>) ((config, context) => (MigratorBase) new DbMigrator(config, context, existenceState, true)), objectContext);
    }

    public virtual bool CompatibleWithModel(
      bool throwIfNoMetadata,
      DatabaseExistenceState existenceState)
    {
      return new ModelCompatibilityChecker().CompatibleWithModel(this, new ModelHashCalculator(), throwIfNoMetadata, existenceState);
    }

    public virtual bool ModelMatches(VersionedModel model)
    {
      return !new EdmModelDiffer().Diff(model.Model, this.Owner.GetModel(), (Lazy<ModificationCommandTreeGenerator>) null, (MigrationSqlGenerator) null, model.Version, (string) null).Any<MigrationOperation>();
    }

    public virtual string QueryForModelHash()
    {
      return new EdmMetadataRepository(this, this.OriginalConnectionString, this.ProviderFactory).QueryForModelHash((Func<DbConnection, EdmMetadataContext>) (c => new EdmMetadataContext(c)));
    }

    public virtual VersionedModel QueryForModel(DatabaseExistenceState existenceState)
    {
      string migrationId;
      string productVersion;
      XDocument lastModel = this.CreateHistoryRepository(existenceState).GetLastModel(out migrationId, out productVersion, (string) null);
      if (lastModel == null)
        return (VersionedModel) null;
      return new VersionedModel(lastModel, productVersion);
    }

    public virtual void SaveMetadataToDatabase()
    {
      if (this.CodeFirstModel == null)
        return;
      this.PerformInitializationAction((Action) (() => this.CreateHistoryRepository(DatabaseExistenceState.Unknown).BootstrapUsingEFProviderDdl(new VersionedModel(this.Owner.GetModel(), (string) null))));
    }

    public virtual bool HasHistoryTableEntry()
    {
      return this.CreateHistoryRepository(DatabaseExistenceState.Unknown).HasMigrations();
    }

    private HistoryRepository CreateHistoryRepository(
      DatabaseExistenceState existenceState = DatabaseExistenceState.Unknown)
    {
      this.DiscoverMigrationsConfiguration();
      string connectionString = this.OriginalConnectionString;
      DbProviderFactory providerFactory = this.ProviderFactory;
      string contextKey = this._migrationsConfiguration().ContextKey;
      int? commandTimeout = this.CommandTimeout;
      Func<DbConnection, string, HistoryContext> historyContextFactory = this.HistoryContextFactory;
      object obj;
      if (this.DefaultSchema == null)
        obj = (object) Enumerable.Empty<string>();
      else
        obj = (object) new string[1]{ this.DefaultSchema };
      DbContext owner = this.Owner;
      int num = (int) existenceState;
      return new HistoryRepository(this, connectionString, providerFactory, contextKey, commandTimeout, historyContextFactory, (IEnumerable<string>) obj, owner, (DatabaseExistenceState) num);
    }

    public virtual DbTransaction TryGetCurrentStoreTransaction()
    {
      return ((EntityConnection) this.GetObjectContextWithoutDatabaseInitialization().Connection).CurrentTransaction?.StoreTransaction;
    }

    protected bool InInitializationAction { get; set; }

    public void PerformInitializationAction(Action action)
    {
      if (this.InInitializationAction)
      {
        action();
      }
      else
      {
        try
        {
          this.InInitializationAction = true;
          action();
        }
        catch (DataException ex)
        {
          throw new DataException(Strings.Database_InitializationException, (Exception) ex);
        }
        finally
        {
          this.InInitializationAction = false;
        }
      }
    }

    public virtual void RegisterObjectStateManagerChangedEvent(CollectionChangeEventHandler handler)
    {
      this.ObjectContext.ObjectStateManager.ObjectStateManagerChanged += handler;
    }

    public virtual bool EntityInContextAndNotDeleted(object entity)
    {
      ObjectStateEntry entry;
      if (this.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
        return entry.State != EntityState.Deleted;
      return false;
    }

    public virtual int SaveChanges()
    {
      try
      {
        if (this.ValidateOnSaveEnabled)
        {
          IEnumerable<DbEntityValidationResult> validationErrors = this.Owner.GetValidationErrors();
          if (validationErrors.Any<DbEntityValidationResult>())
            throw new DbEntityValidationException(Strings.DbEntityValidationException_ValidationFailed, validationErrors);
        }
        return this.ObjectContext.SaveChanges((System.Data.Entity.Core.Objects.SaveOptions) (1 | (this.AutoDetectChangesEnabled && !this.ValidateOnSaveEnabled ? 2 : 0)));
      }
      catch (UpdateException ex)
      {
        throw this.WrapUpdateException(ex);
      }
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (this.ValidateOnSaveEnabled)
      {
        IEnumerable<DbEntityValidationResult> validationErrors = this.Owner.GetValidationErrors();
        if (validationErrors.Any<DbEntityValidationResult>())
          throw new DbEntityValidationException(Strings.DbEntityValidationException_ValidationFailed, validationErrors);
      }
      TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
      this.ObjectContext.SaveChangesAsync((System.Data.Entity.Core.Objects.SaveOptions) (1 | (this.AutoDetectChangesEnabled && !this.ValidateOnSaveEnabled ? 2 : 0)), cancellationToken).ContinueWith((Action<Task<int>>) (t =>
      {
        if (t.IsFaulted)
          tcs.TrySetException(t.Exception.InnerExceptions.Select<Exception, Exception>((Func<Exception, Exception>) (ex =>
          {
            UpdateException updateException = ex as UpdateException;
            if (updateException != null)
              return (Exception) this.WrapUpdateException(updateException);
            return ex;
          })));
        else if (t.IsCanceled)
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult(t.Result);
      }), TaskContinuationOptions.ExecuteSynchronously);
      return tcs.Task;
    }

    public void Initialize()
    {
      this.InitializeContext();
      this.InitializeDatabase();
    }

    protected abstract void InitializeContext();

    public abstract void MarkDatabaseNotInitialized();

    protected abstract void InitializeDatabase();

    public abstract void MarkDatabaseInitialized();

    public void PerformDatabaseInitialization()
    {
      object obj1 = DbConfiguration.DependencyResolver.GetService(typeof (IDatabaseInitializer<>).MakeGenericType(this.Owner.GetType()));
      if (obj1 == null)
      {
        IDatabaseInitializer<DbContext> defaultInitializer = this.DefaultInitializer;
        obj1 = defaultInitializer != null ? (object) defaultInitializer : (object) new NullDatabaseInitializer<DbContext>();
      }
      object obj2 = obj1;
      Action action = (Action) InternalContext.CreateInitializationActionMethod.MakeGenericMethod(this.Owner.GetType()).Invoke((object) this, new object[1]
      {
        obj2
      });
      bool detectChangesEnabled = this.AutoDetectChangesEnabled;
      bool validateOnSaveEnabled = this.ValidateOnSaveEnabled;
      try
      {
        if (!(this.Owner is TransactionContext))
          this.UseTempObjectContext();
        this.PerformInitializationAction(action);
      }
      finally
      {
        if (!(this.Owner is TransactionContext))
          this.DisposeTempObjectContext();
        this.AutoDetectChangesEnabled = detectChangesEnabled;
        this.ValidateOnSaveEnabled = validateOnSaveEnabled;
      }
    }

    private Action CreateInitializationAction<TContext>(
      IDatabaseInitializer<TContext> initializer)
      where TContext : DbContext
    {
      return (Action) (() => initializer.InitializeDatabase((TContext) this.Owner));
    }

    public abstract IDatabaseInitializer<DbContext> DefaultInitializer { get; }

    public abstract bool EnsureTransactionsForFunctionsAndCommands { get; set; }

    public abstract bool LazyLoadingEnabled { get; set; }

    public abstract bool ProxyCreationEnabled { get; set; }

    public abstract bool UseDatabaseNullSemantics { get; set; }

    public abstract int? CommandTimeout { get; set; }

    public bool AutoDetectChangesEnabled { get; set; }

    public bool ValidateOnSaveEnabled { get; set; }

    ~InternalContext()
    {
      this.DisposeContext(false);
    }

    public void Dispose()
    {
      this.DisposeContext(true);
      GC.SuppressFinalize((object) this);
    }

    public virtual void DisposeContext(bool disposing)
    {
      if (this.IsDisposed)
        return;
      if (disposing && this.OnDisposing != null)
      {
        this.OnDisposing((object) this, new EventArgs());
        this.OnDisposing = (EventHandler<EventArgs>) null;
      }
      if (this._tempObjectContext != null)
        this._tempObjectContext.Dispose();
      this.Log = (Action<string>) null;
      this.IsDisposed = true;
    }

    public bool IsDisposed { get; private set; }

    public virtual void DetectChanges(bool force = false)
    {
      if (!this.AutoDetectChangesEnabled && !force)
        return;
      this.ObjectContext.DetectChanges();
    }

    public virtual IDbSet<TEntity> Set<TEntity>() where TEntity : class
    {
      if (typeof (TEntity) != ObjectContextTypeCache.GetObjectType(typeof (TEntity)))
        throw Error.CannotCallGenericSetWithProxyType();
      IInternalSetAdapter internalSetAdapter;
      if (!this._genericSets.TryGetValue(typeof (TEntity), out internalSetAdapter))
      {
        internalSetAdapter = (IInternalSetAdapter) new DbSet<TEntity>(this._nonGenericSets.TryGetValue(typeof (TEntity), out internalSetAdapter) ? (InternalSet<TEntity>) internalSetAdapter.InternalSet : new InternalSet<TEntity>(this));
        this._genericSets.Add(typeof (TEntity), internalSetAdapter);
      }
      return (IDbSet<TEntity>) internalSetAdapter;
    }

    public virtual IInternalSetAdapter Set(Type entityType)
    {
      entityType = ObjectContextTypeCache.GetObjectType(entityType);
      IInternalSetAdapter internalSet;
      if (!this._nonGenericSets.TryGetValue(entityType, out internalSet))
      {
        internalSet = this.CreateInternalSet(entityType, this._genericSets.TryGetValue(entityType, out internalSet) ? internalSet.InternalSet : (IInternalSet) null);
        this._nonGenericSets.Add(entityType, internalSet);
      }
      return internalSet;
    }

    private IInternalSetAdapter CreateInternalSet(
      Type entityType,
      IInternalSet internalSet)
    {
      Func<InternalContext, IInternalSet, IInternalSetAdapter> func;
      if (!InternalContext._setFactories.TryGetValue(entityType, out func))
      {
        if (entityType.IsValueType())
          throw Error.DbSet_EntityTypeNotInModel((object) entityType.Name);
        func = (Func<InternalContext, IInternalSet, IInternalSetAdapter>) Delegate.CreateDelegate(typeof (Func<InternalContext, IInternalSet, IInternalSetAdapter>), typeof (InternalDbSet<>).MakeGenericType(entityType).GetDeclaredMethod("Create", typeof (InternalContext), typeof (IInternalSet)));
        InternalContext._setFactories.TryAdd(entityType, func);
      }
      return func(this, internalSet);
    }

    public virtual EntitySetTypePair GetEntitySetAndBaseTypeForType(Type entityType)
    {
      this.Initialize();
      this.UpdateEntitySetMappingsForType(entityType);
      return this.GetEntitySetMappingForType(entityType);
    }

    public virtual EntitySetTypePair TryGetEntitySetAndBaseTypeForType(
      Type entityType)
    {
      this.Initialize();
      if (!this.TryUpdateEntitySetMappingsForType(entityType))
        return (EntitySetTypePair) null;
      return this.GetEntitySetMappingForType(entityType);
    }

    public virtual bool IsEntityTypeMapped(Type entityType)
    {
      this.Initialize();
      return this.TryUpdateEntitySetMappingsForType(entityType);
    }

    public virtual IEnumerable<TEntity> GetLocalEntities<TEntity>()
    {
      return this.ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Unchanged | EntityState.Added | EntityState.Modified).Where<ObjectStateEntry>((Func<ObjectStateEntry, bool>) (e => e.Entity is TEntity)).Select<ObjectStateEntry, TEntity>((Func<ObjectStateEntry, TEntity>) (e => (TEntity) e.Entity));
    }

    public virtual IEnumerator<TElement> ExecuteSqlQuery<TElement>(
      string sql,
      bool? streaming,
      object[] parameters)
    {
      this.ObjectContext.AsyncMonitor.EnsureNotEntered();
      return (IEnumerator<TElement>) new LazyEnumerator<TElement>((Func<ObjectResult<TElement>>) (() =>
      {
        this.Initialize();
        return this.ObjectContext.ExecuteStoreQuery<TElement>(sql, new ExecutionOptions(MergeOption.AppendOnly, streaming), parameters);
      }));
    }

    public virtual IDbAsyncEnumerator<TElement> ExecuteSqlQueryAsync<TElement>(
      string sql,
      bool? streaming,
      object[] parameters)
    {
      this.ObjectContext.AsyncMonitor.EnsureNotEntered();
      return (IDbAsyncEnumerator<TElement>) new LazyAsyncEnumerator<TElement>((Func<CancellationToken, Task<ObjectResult<TElement>>>) (cancellationToken =>
      {
        this.Initialize();
        return this.ObjectContext.ExecuteStoreQueryAsync<TElement>(sql, new ExecutionOptions(MergeOption.AppendOnly, streaming), cancellationToken, parameters);
      }));
    }

    public virtual IEnumerator ExecuteSqlQuery(
      Type elementType,
      string sql,
      bool? streaming,
      object[] parameters)
    {
      Func<InternalContext, string, bool?, object[], IEnumerator> func;
      if (!InternalContext._queryExecutors.TryGetValue(elementType, out func))
      {
        func = (Func<InternalContext, string, bool?, object[], IEnumerator>) Delegate.CreateDelegate(typeof (Func<InternalContext, string, bool?, object[], IEnumerator>), InternalContext.ExecuteSqlQueryAsIEnumeratorMethod.MakeGenericMethod(elementType));
        InternalContext._queryExecutors.TryAdd(elementType, func);
      }
      return func(this, sql, streaming, parameters);
    }

    private IEnumerator ExecuteSqlQueryAsIEnumerator<TElement>(
      string sql,
      bool? streaming,
      object[] parameters)
    {
      return (IEnumerator) this.ExecuteSqlQuery<TElement>(sql, streaming, parameters);
    }

    public virtual IDbAsyncEnumerator ExecuteSqlQueryAsync(
      Type elementType,
      string sql,
      bool? streaming,
      object[] parameters)
    {
      Func<InternalContext, string, bool?, object[], IDbAsyncEnumerator> func;
      if (!InternalContext._asyncQueryExecutors.TryGetValue(elementType, out func))
      {
        func = (Func<InternalContext, string, bool?, object[], IDbAsyncEnumerator>) Delegate.CreateDelegate(typeof (Func<InternalContext, string, bool?, object[], IDbAsyncEnumerator>), InternalContext.ExecuteSqlQueryAsIDbAsyncEnumeratorMethod.MakeGenericMethod(elementType));
        InternalContext._asyncQueryExecutors.TryAdd(elementType, func);
      }
      return func(this, sql, streaming, parameters);
    }

    private IDbAsyncEnumerator ExecuteSqlQueryAsIDbAsyncEnumerator<TElement>(
      string sql,
      bool? streaming,
      object[] parameters)
    {
      return (IDbAsyncEnumerator) this.ExecuteSqlQueryAsync<TElement>(sql, streaming, parameters);
    }

    public virtual int ExecuteSqlCommand(
      TransactionalBehavior transactionalBehavior,
      string sql,
      object[] parameters)
    {
      this.Initialize();
      return this.ObjectContext.ExecuteStoreCommand(transactionalBehavior, sql, parameters);
    }

    public virtual Task<int> ExecuteSqlCommandAsync(
      TransactionalBehavior transactionalBehavior,
      string sql,
      CancellationToken cancellationToken,
      object[] parameters)
    {
      this.Initialize();
      return this.ObjectContext.ExecuteStoreCommandAsync(transactionalBehavior, sql, cancellationToken, parameters);
    }

    public virtual IEntityStateEntry GetStateEntry(object entity)
    {
      this.DetectChanges(false);
      ObjectStateEntry entry;
      if (!this.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
        return (IEntityStateEntry) null;
      return (IEntityStateEntry) new StateEntryAdapter(entry);
    }

    public virtual IEnumerable<IEntityStateEntry> GetStateEntries()
    {
      return this.GetStateEntries((Func<ObjectStateEntry, bool>) (e => e.Entity != null));
    }

    public virtual IEnumerable<IEntityStateEntry> GetStateEntries<TEntity>() where TEntity : class
    {
      return this.GetStateEntries((Func<ObjectStateEntry, bool>) (e => e.Entity is TEntity));
    }

    private IEnumerable<IEntityStateEntry> GetStateEntries(
      Func<ObjectStateEntry, bool> predicate)
    {
      this.DetectChanges(false);
      return (IEnumerable<IEntityStateEntry>) this.ObjectContext.ObjectStateManager.GetObjectStateEntries(~EntityState.Detached).Where<ObjectStateEntry>(predicate).Select<ObjectStateEntry, StateEntryAdapter>((Func<ObjectStateEntry, StateEntryAdapter>) (e => new StateEntryAdapter(e)));
    }

    public virtual DbUpdateException WrapUpdateException(
      UpdateException updateException)
    {
      if (updateException.StateEntries != null && updateException.StateEntries.Any<ObjectStateEntry>((Func<ObjectStateEntry, bool>) (e => e.Entity == null)))
        return new DbUpdateException(this, updateException, true);
      OptimisticConcurrencyException innerException = updateException as OptimisticConcurrencyException;
      if (innerException == null)
        return new DbUpdateException(this, updateException, false);
      return (DbUpdateException) new DbUpdateConcurrencyException(this, innerException);
    }

    public virtual TEntity CreateObject<TEntity>() where TEntity : class
    {
      return this.ObjectContext.CreateObject<TEntity>();
    }

    public virtual object CreateObject(Type type)
    {
      Func<InternalContext, object> func;
      if (!InternalContext._entityFactories.TryGetValue(type, out func))
      {
        func = (Func<InternalContext, object>) Delegate.CreateDelegate(typeof (Func<InternalContext, object>), InternalContext.CreateObjectAsObjectMethod.MakeGenericMethod(type));
        InternalContext._entityFactories.TryAdd(type, func);
      }
      return func(this);
    }

    private object CreateObjectAsObject<TEntity>() where TEntity : class
    {
      return (object) this.CreateObject<TEntity>();
    }

    public abstract DbConnection Connection { get; }

    public abstract string OriginalConnectionString { get; }

    public abstract DbConnectionStringOrigin ConnectionStringOrigin { get; }

    public abstract void OverrideConnection(IInternalConnection connection);

    public virtual AppConfig AppConfig
    {
      get
      {
        this.CheckContextNotDisposed();
        return this._appConfig;
      }
      set
      {
        this.CheckContextNotDisposed();
        this._appConfig = value;
      }
    }

    public virtual DbProviderInfo ModelProviderInfo
    {
      get
      {
        return (DbProviderInfo) null;
      }
      set
      {
      }
    }

    public virtual string ConnectionStringName
    {
      get
      {
        return (string) null;
      }
    }

    public virtual string ProviderName
    {
      get
      {
        return this.Connection.GetProviderInvariantName();
      }
    }

    public DbProviderFactory ProviderFactory
    {
      get
      {
        return this._providerFactory ?? (this._providerFactory = DbProviderServices.GetProviderFactory(this.Connection));
      }
    }

    public virtual Action<DbModelBuilder> OnModelCreating
    {
      get
      {
        return (Action<DbModelBuilder>) null;
      }
      set
      {
      }
    }

    public bool InitializerDisabled { get; set; }

    public virtual DatabaseOperations DatabaseOperations
    {
      get
      {
        return new DatabaseOperations();
      }
    }

    protected void CheckContextNotDisposed()
    {
      if (this.IsDisposed)
        throw Error.DbContext_Disposed();
    }

    protected void ResetDbSets()
    {
      foreach (IInternalSetAdapter internalSetAdapter in this._genericSets.Values.Union<IInternalSetAdapter>((IEnumerable<IInternalSetAdapter>) this._nonGenericSets.Values))
        internalSetAdapter.InternalSet.ResetQuery();
    }

    public void ForceOSpaceLoadingForKnownEntityTypes()
    {
      if (this._oSpaceLoadingForced)
        return;
      this._oSpaceLoadingForced = true;
      this.Initialize();
      foreach (IInternalSetAdapter internalSetAdapter in this._genericSets.Values.Union<IInternalSetAdapter>((IEnumerable<IInternalSetAdapter>) this._nonGenericSets.Values))
        internalSetAdapter.InternalSet.TryInitialize();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryUpdateEntitySetMappingsForType(Type entityType)
    {
      return this.GetObjectContextWithoutDatabaseInitialization().MetadataWorkspace.MetadataOptimization.TryUpdateEntitySetMappingsForType(entityType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private EntitySetTypePair GetEntitySetMappingForType(Type entityType)
    {
      return this.GetObjectContextWithoutDatabaseInitialization().MetadataWorkspace.MetadataOptimization.EntitySetMappingCache[entityType];
    }

    private void UpdateEntitySetMappingsForType(Type entityType)
    {
      if (this.TryUpdateEntitySetMappingsForType(entityType))
        return;
      if (this.IsComplexType(entityType))
        throw Error.DbSet_DbSetUsedWithComplexType((object) entityType.Name);
      if (InternalContext.IsPocoTypeInNonPocoAssembly(entityType))
        throw Error.DbSet_PocoAndNonPocoMixedInSameAssembly((object) entityType.Name);
      throw Error.DbSet_EntityTypeNotInModel((object) entityType.Name);
    }

    private static bool IsPocoTypeInNonPocoAssembly(Type entityType)
    {
      if (entityType.Assembly().GetCustomAttributes<EdmSchemaAttribute>().Any<EdmSchemaAttribute>())
        return !entityType.GetCustomAttributes<EdmEntityTypeAttribute>(true).Any<EdmEntityTypeAttribute>();
      return false;
    }

    private bool IsComplexType(Type clrType)
    {
      MetadataWorkspace metadataWorkspace = this.GetObjectContextWithoutDatabaseInitialization().MetadataWorkspace;
      ObjectItemCollection objectItemCollection = (ObjectItemCollection) metadataWorkspace.GetItemCollection(DataSpace.OSpace);
      return metadataWorkspace.GetItems<ComplexType>(DataSpace.OSpace).Any<ComplexType>((Func<ComplexType, bool>) (t => objectItemCollection.GetClrType((StructuralType) t) == clrType));
    }

    public void ApplyContextInfo(DbContextInfo info)
    {
      if (this._contextInfo != null)
        return;
      this.InitializerDisabled = true;
      this._contextInfo = info;
      this._contextInfo.ConfigureContext(this.Owner);
    }

    public virtual ValidationProvider ValidationProvider
    {
      get
      {
        return this._validationProvider;
      }
    }

    public virtual string DefaultSchema
    {
      get
      {
        return (string) null;
      }
    }

    public string DefaultContextKey
    {
      get
      {
        return this._defaultContextKey ?? this.OwnerShortTypeName;
      }
      set
      {
        this._defaultContextKey = value;
      }
    }

    public DbMigrationsConfiguration MigrationsConfiguration
    {
      get
      {
        this.DiscoverMigrationsConfiguration();
        return this._migrationsConfiguration();
      }
    }

    public Func<DbConnection, string, HistoryContext> HistoryContextFactory
    {
      get
      {
        this.DiscoverMigrationsConfiguration();
        return this._migrationsConfiguration().GetHistoryContextFactory(this.ProviderName);
      }
    }

    public virtual bool MigrationsConfigurationDiscovered
    {
      get
      {
        this.DiscoverMigrationsConfiguration();
        return this._migrationsConfigurationDiscovered.Value;
      }
    }

    private void DiscoverMigrationsConfiguration()
    {
      if (this._migrationsConfigurationDiscovered.HasValue)
        return;
      Type contextType = this.Owner.GetType();
      DbMigrationsConfiguration discoveredConfig = new MigrationsConfigurationFinder(new TypeFinder(contextType.Assembly)).FindMigrationsConfiguration(contextType, (string) null, (Func<string, Exception>) null, (Func<string, IEnumerable<Type>, Exception>) null, (Func<string, string, Exception>) null, (Func<string, string, Exception>) null);
      if (discoveredConfig != null)
      {
        this._migrationsConfiguration = (Func<DbMigrationsConfiguration>) (() => discoveredConfig);
        this._migrationsConfigurationDiscovered = new bool?(true);
      }
      else
      {
        this._migrationsConfiguration = (Func<DbMigrationsConfiguration>) (() => new Lazy<DbMigrationsConfiguration>((Func<DbMigrationsConfiguration>) (() => new DbMigrationsConfiguration()
        {
          ContextType = contextType,
          AutomaticMigrationsEnabled = true,
          MigrationsAssembly = contextType.Assembly,
          MigrationsNamespace = contextType.Namespace,
          ContextKey = this.DefaultContextKey,
          TargetDatabase = new DbConnectionInfo(this.OriginalConnectionString, this.ProviderName),
          CommandTimeout = this.CommandTimeout
        })).Value);
        this._migrationsConfigurationDiscovered = new bool?(false);
      }
    }

    internal virtual string OwnerShortTypeName
    {
      get
      {
        return this.Owner.GetType().ToString();
      }
    }

    public virtual Action<string> Log
    {
      get
      {
        if (this._logFormatter == null)
          return (Action<string>) null;
        return this._logFormatter.WriteAction;
      }
      set
      {
        if (this._logFormatter != null && !(this._logFormatter.WriteAction != value))
          return;
        if (this._logFormatter != null)
        {
          this._dispatchers.Value.RemoveInterceptor((IDbInterceptor) this._logFormatter);
          this._logFormatter = (DatabaseLogFormatter) null;
        }
        if (value == null)
          return;
        this._logFormatter = DbConfiguration.DependencyResolver.GetService<Func<DbContext, Action<string>, DatabaseLogFormatter>>()(this.Owner, value);
        this._dispatchers.Value.AddInterceptor((IDbInterceptor) this._logFormatter);
      }
    }
  }
}
