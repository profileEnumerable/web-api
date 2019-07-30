// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.DbSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal;
using System.Data.Entity.Internal.Linq;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity
{
  /// <summary>
  /// A non-generic version of <see cref="T:System.Data.Entity.DbSet`1" /> which can be used when the type of entity
  /// is not known at build time.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Name is intentional")]
  public abstract class DbSet : DbQuery, IInternalSetAdapter
  {
    /// <summary>
    /// Creates an instance of a <see cref="T:System.Data.Entity.DbSet" /> when called from the constructor of a derived
    /// type that will be used as a test double for DbSets. Methods and properties that will be used by the
    /// test double must be implemented by the test double except AsNoTracking, AsStreaming, an Include where
    /// the default implementation is a no-op.
    /// </summary>
    protected internal DbSet()
    {
    }

    /// <summary>
    /// Finds an entity with the given primary key values.
    /// If an entity with the given primary key values exists in the context, then it is
    /// returned immediately without making a request to the store.  Otherwise, a request
    /// is made to the store for an entity with the given primary key values and this entity,
    /// if found, is attached to the context and returned.  If no entity is found in the
    /// context or the store, then null is returned.
    /// </summary>
    /// <remarks>
    /// The ordering of composite key values is as defined in the EDM, which is in turn as defined in
    /// the designer, by the Code First fluent API, or by the DataMember attribute.
    /// </remarks>
    /// <param name="keyValues"> The values of the primary key for the entity to be found. </param>
    /// <returns> The entity found, or null. </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if multiple entities exist in the context with the primary key values given.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the type of entity is not part of the data model for this context.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the types of the key values do not match the types of the key values for the entity type to be found.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
    public virtual object Find(params object[] keyValues)
    {
      throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) nameof (Find), (object) this.GetType().Name, (object) typeof (DbSet).Name));
    }

    /// <summary>
    /// Asynchronously finds an entity with the given primary key values.
    /// If an entity with the given primary key values exists in the context, then it is
    /// returned immediately without making a request to the store.  Otherwise, a request
    /// is made to the store for an entity with the given primary key values and this entity,
    /// if found, is attached to the context and returned.  If no entity is found in the
    /// context or the store, then null is returned.
    /// </summary>
    /// <remarks>
    /// The ordering of composite key values is as defined in the EDM, which is in turn as defined in
    /// the designer, by the Code First fluent API, or by the DataMember attribute.
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="keyValues"> The values of the primary key for the entity to be found. </param>
    /// <returns> A task that represents the asynchronous find operation. The task result contains the entity found, or null. </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if multiple entities exist in the context with the primary key values given.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the type of entity is not part of the data model for this context.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the types of the key values do not match the types of the key values for the entity type to be found.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
    public virtual Task<object> FindAsync(params object[] keyValues)
    {
      return this.FindAsync(CancellationToken.None, keyValues);
    }

    /// <summary>
    /// Asynchronously finds an entity with the given primary key values.
    /// If an entity with the given primary key values exists in the context, then it is
    /// returned immediately without making a request to the store.  Otherwise, a request
    /// is made to the store for an entity with the given primary key values and this entity,
    /// if found, is attached to the context and returned.  If no entity is found in the
    /// context or the store, then null is returned.
    /// </summary>
    /// <remarks>
    /// The ordering of composite key values is as defined in the EDM, which is in turn as defined in
    /// the designer, by the Code First fluent API, or by the DataMember attribute.
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="keyValues"> The values of the primary key for the entity to be found. </param>
    /// <returns> A task that represents the asynchronous find operation. The task result contains the entity found, or null. </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if multiple entities exist in the context with the primary key values given.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the type of entity is not part of the data model for this context.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the types of the key values do not match the types of the key values for the entity type to be found.</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
    public virtual Task<object> FindAsync(
      CancellationToken cancellationToken,
      params object[] keyValues)
    {
      throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) nameof (FindAsync), (object) this.GetType().Name, (object) typeof (DbSet).Name));
    }

    /// <summary>
    /// Gets an <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" /> that represents a local view of all Added, Unchanged,
    /// and Modified entities in this set.  This local view will stay in sync as entities are added or
    /// removed from the context.  Likewise, entities added to or removed from the local view will automatically
    /// be added to or removed from the context.
    /// </summary>
    /// <remarks>
    /// This property can be used for data binding by populating the set with data, for example by using the Load
    /// extension method, and then binding to the local data through this property.  For WPF bind to this property
    /// directly.  For Windows Forms bind to the result of calling ToBindingList on this property
    /// </remarks>
    /// <value> The local view. </value>
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public virtual IList Local
    {
      get
      {
        throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) nameof (Local), (object) this.GetType().Name, (object) typeof (DbSet).Name));
      }
    }

    /// <summary>
    /// Attaches the given entity to the context underlying the set.  That is, the entity is placed
    /// into the context in the Unchanged state, just as if it had been read from the database.
    /// </summary>
    /// <param name="entity"> The entity to attach. </param>
    /// <returns> The entity. </returns>
    /// <remarks>
    /// Attach is used to repopulate a context with an entity that is known to already exist in the database.
    /// SaveChanges will therefore not attempt to insert an attached entity into the database because
    /// it is assumed to already be there.
    /// Note that entities that are already in the context in some other state will have their state set
    /// to Unchanged.  Attach is a no-op if the entity is already in the context in the Unchanged state.
    /// </remarks>
    public virtual object Attach(object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      this.GetInternalSetWithCheck(nameof (Attach)).Attach(entity);
      return entity;
    }

    /// <summary>
    /// Adds the given entity to the context underlying the set in the Added state such that it will
    /// be inserted into the database when SaveChanges is called.
    /// </summary>
    /// <param name="entity"> The entity to add. </param>
    /// <returns> The entity. </returns>
    /// <remarks>
    /// Note that entities that are already in the context in some other state will have their state set
    /// to Added.  Add is a no-op if the entity is already in the context in the Added state.
    /// </remarks>
    public virtual object Add(object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      this.GetInternalSetWithCheck(nameof (Add)).Add(entity);
      return entity;
    }

    /// <summary>
    /// Adds the given collection of entities into context underlying the set with each entity being put into
    /// the Added state such that it will be inserted into the database when SaveChanges is called.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    /// <returns>The collection of entities.</returns>
    /// <remarks>
    /// Note that if <see cref="P:System.Data.Entity.Infrastructure.DbContextConfiguration.AutoDetectChangesEnabled" /> is set to true (which is
    /// the default), then DetectChanges will be called once before adding any entities and will not be called
    /// again. This means that in some situations AddRange may perform significantly better than calling
    /// Add multiple times would do.
    /// Note that entities that are already in the context in some other state will have their state set to
    /// Added.  AddRange is a no-op for entities that are already in the context in the Added state.
    /// </remarks>
    public virtual IEnumerable AddRange(IEnumerable entities)
    {
      Check.NotNull<IEnumerable>(entities, nameof (entities));
      this.GetInternalSetWithCheck(nameof (AddRange)).AddRange(entities);
      return entities;
    }

    /// <summary>
    /// Marks the given entity as Deleted such that it will be deleted from the database when SaveChanges
    /// is called.  Note that the entity must exist in the context in some other state before this method
    /// is called.
    /// </summary>
    /// <param name="entity"> The entity to remove. </param>
    /// <returns> The entity. </returns>
    /// <remarks>
    /// Note that if the entity exists in the context in the Added state, then this method
    /// will cause it to be detached from the context.  This is because an Added entity is assumed not to
    /// exist in the database such that trying to delete it does not make sense.
    /// </remarks>
    public virtual object Remove(object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      this.GetInternalSetWithCheck(nameof (Remove)).Remove(entity);
      return entity;
    }

    /// <summary>
    /// Removes the given collection of entities from the context underlying the set with each entity being put into
    /// the Deleted state such that it will be deleted from the database when SaveChanges is called.
    /// </summary>
    /// <param name="entities">The collection of entities to delete.</param>
    /// <returns>The collection of entities.</returns>
    /// <remarks>
    /// Note that if <see cref="P:System.Data.Entity.Infrastructure.DbContextConfiguration.AutoDetectChangesEnabled" /> is set to true (which is
    /// the default), then DetectChanges will be called once before delete any entities and will not be called
    /// again. This means that in some situations RemoveRange may perform significantly better than calling
    /// Remove multiple times would do.
    /// Note that if any entity exists in the context in the Added state, then this method
    /// will cause it to be detached from the context.  This is because an Added entity is assumed not to
    /// exist in the database such that trying to delete it does not make sense.
    /// </remarks>
    public virtual IEnumerable RemoveRange(IEnumerable entities)
    {
      Check.NotNull<IEnumerable>(entities, nameof (entities));
      this.GetInternalSetWithCheck(nameof (RemoveRange)).RemoveRange(entities);
      return entities;
    }

    /// <summary>
    /// Creates a new instance of an entity for the type of this set.
    /// Note that this instance is NOT added or attached to the set.
    /// The instance returned will be a proxy if the underlying context is configured to create
    /// proxies and the entity type meets the requirements for creating a proxy.
    /// </summary>
    /// <returns> The entity instance, which may be a proxy. </returns>
    public virtual object Create()
    {
      throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) nameof (Create), (object) this.GetType().Name, (object) typeof (DbSet).Name));
    }

    /// <summary>
    /// Creates a new instance of an entity for the type of this set or for a type derived
    /// from the type of this set.
    /// Note that this instance is NOT added or attached to the set.
    /// The instance returned will be a proxy if the underlying context is configured to create
    /// proxies and the entity type meets the requirements for creating a proxy.
    /// </summary>
    /// <param name="derivedEntityType">The type of entity to create.</param>
    /// <returns> The entity instance, which may be a proxy. </returns>
    public virtual object Create(Type derivedEntityType)
    {
      throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) nameof (Create), (object) this.GetType().Name, (object) typeof (DbSet).Name));
    }

    /// <summary>
    /// Returns the equivalent generic <see cref="T:System.Data.Entity.DbSet`1" /> object.
    /// </summary>
    /// <typeparam name="TEntity"> The type of entity for which the set was created. </typeparam>
    /// <returns> The generic set object. </returns>
    public DbSet<TEntity> Cast<TEntity>() where TEntity : class
    {
      if (this.InternalSet == null)
        throw new NotSupportedException(Strings.TestDoublesCannotBeConverted);
      if (typeof (TEntity) != this.InternalSet.ElementType)
        throw Error.DbEntity_BadTypeForCast((object) typeof (DbSet).Name, (object) typeof (TEntity).Name, (object) this.InternalSet.ElementType.Name);
      return (DbSet<TEntity>) this.InternalSet.InternalContext.Set<TEntity>();
    }

    IInternalSet IInternalSetAdapter.InternalSet
    {
      get
      {
        return this.InternalSet;
      }
    }

    internal virtual IInternalSet InternalSet
    {
      get
      {
        return (IInternalSet) null;
      }
    }

    internal virtual IInternalSet GetInternalSetWithCheck(string memberName)
    {
      throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) memberName, (object) this.GetType().Name, (object) typeof (DbSet).Name));
    }

    /// <summary>
    /// Creates a raw SQL query that will return entities in this set.  By default, the
    /// entities returned are tracked by the context; this can be changed by calling
    /// AsNoTracking on the <see cref="T:System.Data.Entity.Infrastructure.DbRawSqlQuery" /> returned.
    /// Note that the entities returned are always of the type for this set and never of
    /// a derived type.  If the table or tables queried may contain data for other entity
    /// types, then the SQL query must be written appropriately to ensure that only entities of
    /// the correct type are returned.
    /// 
    /// As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    /// context.Set(typeof(Blog)).SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    /// Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    /// context.Set(typeof(Blog)).SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// </summary>
    /// <param name="sql"> The SQL query string. </param>
    /// <param name="parameters">
    /// The parameters to apply to the SQL query string. If output parameters are used, their values
    /// will not be available until the results have been read completely. This is due to the underlying
    /// behavior of DbDataReader, see http://go.microsoft.com/fwlink/?LinkID=398589 for more details.
    /// </param>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Infrastructure.DbSqlQuery" /> object that will execute the query when it is enumerated.
    /// </returns>
    public virtual DbSqlQuery SqlQuery(string sql, params object[] parameters)
    {
      Check.NotEmpty(sql, nameof (sql));
      Check.NotNull<object[]>(parameters, nameof (parameters));
      return new DbSqlQuery(this.InternalSet == null ? (InternalSqlQuery) null : (InternalSqlQuery) new InternalSqlSetQuery(this.InternalSet, sql, false, parameters));
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
