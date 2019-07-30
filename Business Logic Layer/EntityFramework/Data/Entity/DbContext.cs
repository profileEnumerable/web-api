// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.DbContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity
{
  /// <summary>
  /// A DbContext instance represents a combination of the Unit Of Work and Repository patterns such that
  /// it can be used to query from a database and group together changes that will then be written
  /// back to the store as a unit.
  /// DbContext is conceptually similar to ObjectContext.
  /// </summary>
  /// <remarks>
  /// DbContext is usually used with a derived type that contains <see cref="T:System.Data.Entity.DbSet`1" /> properties for
  /// the root entities of the model. These sets are automatically initialized when the
  /// instance of the derived class is created.  This behavior can be modified by applying the
  /// <see cref="T:System.Data.Entity.Infrastructure.SuppressDbSetInitializationAttribute" />  attribute to either the entire derived context
  /// class, or to individual properties on the class.
  /// The Entity Data Model backing the context can be specified in several ways.  When using the Code First
  /// approach, the <see cref="T:System.Data.Entity.DbSet`1" /> properties on the derived context are used to build a model
  /// by convention.  The protected OnModelCreating method can be overridden to tweak this model.  More
  /// control over the model used for the Model First approach can be obtained by creating a <see cref="T:System.Data.Entity.Infrastructure.DbCompiledModel" />
  /// explicitly from a <see cref="T:System.Data.Entity.DbModelBuilder" /> and passing this model to one of the DbContext constructors.
  /// When using the Database First or Model First approach the Entity Data Model can be created using the
  /// Entity Designer (or manually through creation of an EDMX file) and then this model can be specified using
  /// entity connection string or an <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> object.
  /// The connection to the database (including the name of the database) can be specified in several ways.
  /// If the parameterless DbContext constructor is called from a derived context, then the name of the derived context
  /// is used to find a connection string in the app.config or web.config file.  If no connection string is found, then
  /// the name is passed to the DefaultConnectionFactory registered on the <see cref="T:System.Data.Entity.Database" /> class.  The connection
  /// factory then uses the context name as the database name in a default connection string.  (This default connection
  /// string points to .\SQLEXPRESS on the local machine unless a different DefaultConnectionFactory is registered.)
  /// Instead of using the derived context name, the connection/database name can also be specified explicitly by
  /// passing the name to one of the DbContext constructors that takes a string.  The name can also be passed in
  /// the form "name=myname", in which case the name must be found in the config file or an exception will be thrown.
  /// Note that the connection found in the app.config or web.config file can be a normal database connection
  /// string (not a special Entity Framework connection string) in which case the DbContext will use Code First.
  /// However, if the connection found in the config file is a special Entity Framework connection string, then the
  /// DbContext will use Database/Model First and the model specified in the connection string will be used.
  /// An existing or explicitly created DbConnection can also be used instead of the database/connection name.
  /// A <see cref="T:System.Data.Entity.DbModelBuilderVersionAttribute" /> can be applied to a class derived from DbContext to set the
  /// version of conventions used by the context when it creates a model. If no attribute is applied then the
  /// latest version of conventions will be used.
  /// </remarks>
  public class DbContext : IDisposable, IObjectContextAdapter
  {
    private InternalContext _internalContext;
    private Database _database;

    /// <summary>
    /// Constructs a new context instance using conventions to create the name of the database to
    /// which a connection will be made.  The by-convention name is the full name (namespace + class name)
    /// of the derived context class.
    /// See the class remarks for how this is used to create a connection.
    /// </summary>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    protected DbContext()
    {
      this.InitializeLazyInternalContext((IInternalConnection) new LazyInternalConnection(this, this.GetType().DatabaseName()), (DbCompiledModel) null);
    }

    /// <summary>
    /// Constructs a new context instance using conventions to create the name of the database to
    /// which a connection will be made, and initializes it from the given model.
    /// The by-convention name is the full name (namespace + class name) of the derived context class.
    /// See the class remarks for how this is used to create a connection.
    /// </summary>
    /// <param name="model"> The model that will back this context. </param>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    protected DbContext(DbCompiledModel model)
    {
      Check.NotNull<DbCompiledModel>(model, nameof (model));
      this.InitializeLazyInternalContext((IInternalConnection) new LazyInternalConnection(this, this.GetType().DatabaseName()), model);
    }

    /// <summary>
    /// Constructs a new context instance using the given string as the name or connection string for the
    /// database to which a connection will be made.
    /// See the class remarks for how this is used to create a connection.
    /// </summary>
    /// <param name="nameOrConnectionString"> Either the database name or a connection string. </param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public DbContext(string nameOrConnectionString)
    {
      Check.NotEmpty(nameOrConnectionString, nameof (nameOrConnectionString));
      this.InitializeLazyInternalContext((IInternalConnection) new LazyInternalConnection(this, nameOrConnectionString), (DbCompiledModel) null);
    }

    /// <summary>
    /// Constructs a new context instance using the given string as the name or connection string for the
    /// database to which a connection will be made, and initializes it from the given model.
    /// See the class remarks for how this is used to create a connection.
    /// </summary>
    /// <param name="nameOrConnectionString"> Either the database name or a connection string. </param>
    /// <param name="model"> The model that will back this context. </param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public DbContext(string nameOrConnectionString, DbCompiledModel model)
    {
      Check.NotEmpty(nameOrConnectionString, nameof (nameOrConnectionString));
      Check.NotNull<DbCompiledModel>(model, nameof (model));
      this.InitializeLazyInternalContext((IInternalConnection) new LazyInternalConnection(this, nameOrConnectionString), model);
    }

    /// <summary>
    /// Constructs a new context instance using the existing connection to connect to a database.
    /// The connection will not be disposed when the context is disposed if <paramref name="contextOwnsConnection" />
    /// is <c>false</c>.
    /// </summary>
    /// <param name="existingConnection"> An existing connection to use for the new context. </param>
    /// <param name="contextOwnsConnection">
    /// If set to <c>true</c> the connection is disposed when the context is disposed, otherwise the caller must dispose the connection.
    /// </param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public DbContext(DbConnection existingConnection, bool contextOwnsConnection)
    {
      Check.NotNull<DbConnection>(existingConnection, nameof (existingConnection));
      this.InitializeLazyInternalContext((IInternalConnection) new EagerInternalConnection(this, existingConnection, contextOwnsConnection), (DbCompiledModel) null);
    }

    /// <summary>
    /// Constructs a new context instance using the existing connection to connect to a database,
    /// and initializes it from the given model.
    /// The connection will not be disposed when the context is disposed if <paramref name="contextOwnsConnection" />
    /// is <c>false</c>.
    /// </summary>
    /// <param name="existingConnection"> An existing connection to use for the new context. </param>
    /// <param name="model"> The model that will back this context. </param>
    /// <param name="contextOwnsConnection">
    ///     If set to <c>true</c> the connection is disposed when the context is disposed, otherwise the caller must dispose the connection.
    /// </param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public DbContext(
      DbConnection existingConnection,
      DbCompiledModel model,
      bool contextOwnsConnection)
    {
      Check.NotNull<DbConnection>(existingConnection, nameof (existingConnection));
      Check.NotNull<DbCompiledModel>(model, nameof (model));
      this.InitializeLazyInternalContext((IInternalConnection) new EagerInternalConnection(this, existingConnection, contextOwnsConnection), model);
    }

    /// <summary>
    /// Constructs a new context instance around an existing ObjectContext.
    /// </summary>
    /// <param name="objectContext"> An existing ObjectContext to wrap with the new context. </param>
    /// <param name="dbContextOwnsObjectContext">
    ///     If set to <c>true</c> the ObjectContext is disposed when the DbContext is disposed, otherwise the caller must dispose the connection.
    /// </param>
    public DbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
    {
      Check.NotNull<ObjectContext>(objectContext, nameof (objectContext));
      DbConfigurationManager.Instance.EnsureLoadedForContext(this.GetType());
      this._internalContext = (InternalContext) new EagerInternalContext(this, objectContext, dbContextOwnsObjectContext);
      this.DiscoverAndInitializeSets();
    }

    internal virtual void InitializeLazyInternalContext(
      IInternalConnection internalConnection,
      DbCompiledModel model = null)
    {
      DbConfigurationManager.Instance.EnsureLoadedForContext(this.GetType());
      this._internalContext = (InternalContext) new LazyInternalContext(this, internalConnection, model, DbConfiguration.DependencyResolver.GetService<Func<DbContext, IDbModelCacheKey>>(), DbConfiguration.DependencyResolver.GetService<AttributeProvider>(), (Lazy<DbDispatchers>) null, (ObjectContext) null);
      this.DiscoverAndInitializeSets();
    }

    private void DiscoverAndInitializeSets()
    {
      new DbSetDiscoveryService(this).InitializeSets();
    }

    /// <summary>
    /// This method is called when the model for a derived context has been initialized, but
    /// before the model has been locked down and used to initialize the context.  The default
    /// implementation of this method does nothing, but it can be overridden in a derived class
    /// such that the model can be further configured before it is locked down.
    /// </summary>
    /// <remarks>
    /// Typically, this method is called only once when the first instance of a derived context
    /// is created.  The model for that context is then cached and is for all further instances of
    /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
    /// property on the given ModelBuidler, but note that this can seriously degrade performance.
    /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
    /// classes directly.
    /// </remarks>
    /// <param name="modelBuilder"> The builder that defines the model for the context being created. </param>
    protected virtual void OnModelCreating(DbModelBuilder modelBuilder)
    {
    }

    internal void CallOnModelCreating(DbModelBuilder modelBuilder)
    {
      this.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Creates a Database instance for this context that allows for creation/deletion/existence checks
    /// for the underlying database.
    /// </summary>
    public Database Database
    {
      get
      {
        if (this._database == null)
          this._database = new Database(this.InternalContext);
        return this._database;
      }
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.DbSet`1" /> instance for access to entities of the given type in the context
    /// and the underlying store.
    /// </summary>
    /// <remarks>
    /// Note that Entity Framework requires that this method return the same instance each time that it is called
    /// for a given context instance and entity type. Also, the non-generic <see cref="T:System.Data.Entity.DbSet" /> returned by the
    /// <see cref="M:System.Data.Entity.DbContext.Set(System.Type)" /> method must wrap the same underlying query and set of entities. These invariants must
    /// be maintained if this method is overridden for anything other than creating test doubles for unit testing.
    /// See the <see cref="T:System.Data.Entity.DbSet`1" /> class for more details.
    /// </remarks>
    /// <typeparam name="TEntity"> The type entity for which a set should be returned. </typeparam>
    /// <returns> A set for the given entity type. </returns>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set")]
    public virtual DbSet<TEntity> Set<TEntity>() where TEntity : class
    {
      return (DbSet<TEntity>) this.InternalContext.Set<TEntity>();
    }

    /// <summary>
    /// Returns a non-generic <see cref="T:System.Data.Entity.DbSet" /> instance for access to entities of the given type in the context
    /// and the underlying store.
    /// </summary>
    /// <param name="entityType"> The type of entity for which a set should be returned. </param>
    /// <returns> A set for the given entity type. </returns>
    /// <remarks>
    /// Note that Entity Framework requires that this method return the same instance each time that it is called
    /// for a given context instance and entity type. Also, the generic <see cref="T:System.Data.Entity.DbSet`1" /> returned by the
    /// <see cref="M:System.Data.Entity.DbContext.Set(System.Type)" /> method must wrap the same underlying query and set of entities. These invariants must
    /// be maintained if this method is overridden for anything other than creating test doubles for unit testing.
    /// See the <see cref="T:System.Data.Entity.DbSet" /> class for more details.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set")]
    public virtual DbSet Set(Type entityType)
    {
      Check.NotNull<Type>(entityType, nameof (entityType));
      return (DbSet) this.InternalContext.Set(entityType);
    }

    /// <summary>
    /// Saves all changes made in this context to the underlying database.
    /// </summary>
    /// <returns>
    /// The number of state entries written to the underlying database. This can include
    /// state entries for entities and/or relationships. Relationship state entries are created for
    /// many-to-many relationships and relationships where there is no foreign key property
    /// included in the entity class (often referred to as independent associations).
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateException">An error occurred sending updates to the database.</exception>
    /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
    /// A database command did not affect the expected number of rows. This usually indicates an optimistic
    /// concurrency violation; that is, a row has been changed in the database since it was queried.
    /// </exception>
    /// <exception cref="T:System.Data.Entity.Validation.DbEntityValidationException">
    /// The save was aborted because validation of entity property values failed.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">
    /// An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
    /// on the same context instance.</exception>
    /// <exception cref="T:System.ObjectDisposedException">The context or connection have been disposed.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// Some error occurred attempting to process entities in the context either before or after sending commands
    /// to the database.
    /// </exception>
    public virtual int SaveChanges()
    {
      return this.InternalContext.SaveChanges();
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the underlying database.
    /// </summary>
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
    /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateException">An error occurred sending updates to the database.</exception>
    /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
    /// A database command did not affect the expected number of rows. This usually indicates an optimistic
    /// concurrency violation; that is, a row has been changed in the database since it was queried.
    /// </exception>
    /// <exception cref="T:System.Data.Entity.Validation.DbEntityValidationException">
    /// The save was aborted because validation of entity property values failed.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">
    /// An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
    /// on the same context instance.</exception>
    /// <exception cref="T:System.ObjectDisposedException">The context or connection have been disposed.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// Some error occurred attempting to process entities in the context either before or after sending commands
    /// to the database.
    /// </exception>
    public virtual Task<int> SaveChangesAsync()
    {
      return this.SaveChangesAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the underlying database.
    /// </summary>
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
    /// <exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "cancellationToken")]
    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
      return this.InternalContext.SaveChangesAsync(cancellationToken);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    ObjectContext IObjectContextAdapter.ObjectContext
    {
      get
      {
        this.InternalContext.ForceOSpaceLoadingForKnownEntityTypes();
        return this.InternalContext.ObjectContext;
      }
    }

    /// <summary>
    /// Validates tracked entities and returns a Collection of <see cref="T:System.Data.Entity.Validation.DbEntityValidationResult" /> containing validation results.
    /// </summary>
    /// <returns> Collection of validation results for invalid entities. The collection is never null and must not contain null values or results for valid entities. </returns>
    /// <remarks>
    /// 1. This method calls DetectChanges() to determine states of the tracked entities unless
    /// DbContextConfiguration.AutoDetectChangesEnabled is set to false.
    /// 2. By default only Added on Modified entities are validated. The user is able to change this behavior
    /// by overriding ShouldValidateEntity method.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public IEnumerable<DbEntityValidationResult> GetValidationErrors()
    {
      List<DbEntityValidationResult> validationResultList = new List<DbEntityValidationResult>();
      foreach (DbEntityEntry entry in this.ChangeTracker.Entries())
      {
        if (entry.InternalEntry.EntityType != typeof (EdmMetadata) && this.ShouldValidateEntity(entry))
        {
          DbEntityValidationResult validationResult = this.ValidateEntity(entry, (IDictionary<object, object>) new Dictionary<object, object>());
          if (validationResult != null && !validationResult.IsValid)
            validationResultList.Add(validationResult);
        }
      }
      return (IEnumerable<DbEntityValidationResult>) validationResultList;
    }

    /// <summary>
    /// Extension point allowing the user to override the default behavior of validating only
    /// added and modified entities.
    /// </summary>
    /// <param name="entityEntry"> DbEntityEntry instance that is supposed to be validated. </param>
    /// <returns> true to proceed with validation; false otherwise. </returns>
    protected virtual bool ShouldValidateEntity(DbEntityEntry entityEntry)
    {
      Check.NotNull<DbEntityEntry>(entityEntry, nameof (entityEntry));
      return (entityEntry.State & (EntityState.Added | EntityState.Modified)) != (EntityState) 0;
    }

    /// <summary>
    /// Extension point allowing the user to customize validation of an entity or filter out validation results.
    /// Called by <see cref="M:System.Data.Entity.DbContext.GetValidationErrors" />.
    /// </summary>
    /// <param name="entityEntry"> DbEntityEntry instance to be validated. </param>
    /// <param name="items">
    /// User-defined dictionary containing additional info for custom validation. It will be passed to
    /// <see cref="T:System.ComponentModel.DataAnnotations.ValidationContext" />
    /// and will be exposed as
    /// <see cref="P:System.ComponentModel.DataAnnotations.ValidationContext.Items" />
    /// . This parameter is optional and can be null.
    /// </param>
    /// <returns> Entity validation result. Possibly null when overridden. </returns>
    protected virtual DbEntityValidationResult ValidateEntity(
      DbEntityEntry entityEntry,
      IDictionary<object, object> items)
    {
      Check.NotNull<DbEntityEntry>(entityEntry, nameof (entityEntry));
      return entityEntry.InternalEntry.GetValidationResult(items);
    }

    internal virtual DbEntityValidationResult CallValidateEntity(
      DbEntityEntry entityEntry)
    {
      return this.ValidateEntity(entityEntry, (IDictionary<object, object>) new Dictionary<object, object>());
    }

    /// <summary>
    /// Gets a <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry`1" /> object for the given entity providing access to
    /// information about the entity and the ability to perform actions on the entity.
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    /// <param name="entity"> The entity. </param>
    /// <returns> An entry for the entity. </returns>
    public DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
    {
      Check.NotNull<TEntity>(entity, nameof (entity));
      return new DbEntityEntry<TEntity>(new InternalEntityEntry(this.InternalContext, (object) entity));
    }

    /// <summary>
    /// Gets a <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry" /> object for the given entity providing access to
    /// information about the entity and the ability to perform actions on the entity.
    /// </summary>
    /// <param name="entity"> The entity. </param>
    /// <returns> An entry for the entity. </returns>
    public DbEntityEntry Entry(object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      return new DbEntityEntry(new InternalEntityEntry(this.InternalContext, entity));
    }

    /// <summary>
    /// Provides access to features of the context that deal with change tracking of entities.
    /// </summary>
    /// <value> An object used to access features that deal with change tracking. </value>
    public DbChangeTracker ChangeTracker
    {
      get
      {
        return new DbChangeTracker(this.InternalContext);
      }
    }

    /// <summary>
    /// Provides access to configuration options for the context.
    /// </summary>
    /// <value> An object used to access configuration options. </value>
    public DbContextConfiguration Configuration
    {
      get
      {
        return new DbContextConfiguration(this.InternalContext);
      }
    }

    /// <summary>Calls the protected Dispose method.</summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Disposes the context. The underlying <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> is also disposed if it was created
    /// is by this context or ownership was passed to this context when this context was created.
    /// The connection to the database (<see cref="T:System.Data.Common.DbConnection" /> object) is also disposed if it was created
    /// is by this context or ownership was passed to this context when this context was created.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      this._internalContext.Dispose();
    }

    internal virtual InternalContext InternalContext
    {
      get
      {
        return this._internalContext;
      }
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
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
