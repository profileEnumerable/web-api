// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbReferenceEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// A non-generic version of the <see cref="T:System.Data.Entity.Infrastructure.DbReferenceEntry`2" /> class.
  /// </summary>
  public class DbReferenceEntry : DbMemberEntry
  {
    private readonly InternalReferenceEntry _internalReferenceEntry;

    internal static DbReferenceEntry Create(
      InternalReferenceEntry internalReferenceEntry)
    {
      return (DbReferenceEntry) internalReferenceEntry.CreateDbMemberEntry();
    }

    internal DbReferenceEntry(InternalReferenceEntry internalReferenceEntry)
    {
      this._internalReferenceEntry = internalReferenceEntry;
    }

    /// <summary>Gets the property name.</summary>
    /// <value> The property name. </value>
    public override string Name
    {
      get
      {
        return this._internalReferenceEntry.Name;
      }
    }

    /// <summary>
    /// Gets or sets the current value of the navigation property.  The current value is
    /// the entity that the navigation property references.
    /// </summary>
    /// <value> The current value. </value>
    public override object CurrentValue
    {
      get
      {
        return this._internalReferenceEntry.CurrentValue;
      }
      set
      {
        this._internalReferenceEntry.CurrentValue = value;
      }
    }

    /// <summary>
    /// Loads the entity from the database.
    /// Note that if the entity already exists in the context, then it will not overwritten with values from the database.
    /// </summary>
    public void Load()
    {
      this._internalReferenceEntry.Load();
    }

    /// <summary>
    /// Asynchronously loads the entity from the database.
    /// Note that if the entity already exists in the context, then it will not overwritten with values from the database.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task LoadAsync()
    {
      return this.LoadAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously loads the entity from the database.
    /// Note that if the entity already exists in the context, then it will not overwritten with values from the database.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task LoadAsync(CancellationToken cancellationToken)
    {
      return this._internalReferenceEntry.LoadAsync(cancellationToken);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been loaded from the database.
    /// </summary>
    /// <remarks>
    /// Loading the related entity from the database either using lazy-loading, as part of a query, or explicitly
    /// with one of the Load methods will set the IsLoaded flag to true.
    /// IsLoaded can be explicitly set to true to prevent the related entity from being lazy-loaded.
    /// Note that explict loading using one of the Load methods will load the related entity from the database
    /// regardless of whether or not IsLoaded is true.
    /// When a related entity is detached the IsLoaded flag is reset to false indicating that the related entity is
    /// no longer loaded.
    /// </remarks>
    /// <value>
    /// <c>true</c> if the entity is loaded or the IsLoaded has been explicitly set to true; otherwise, <c>false</c>.
    /// </value>
    public bool IsLoaded
    {
      get
      {
        return this._internalReferenceEntry.IsLoaded;
      }
      set
      {
        this._internalReferenceEntry.IsLoaded = value;
      }
    }

    /// <summary>
    /// Returns the query that would be used to load this entity from the database.
    /// The returned query can be modified using LINQ to perform filtering or operations in the database.
    /// </summary>
    /// <returns> A query for the entity. </returns>
    public IQueryable Query()
    {
      return this._internalReferenceEntry.Query();
    }

    /// <summary>
    /// The <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry" /> to which this navigation property belongs.
    /// </summary>
    /// <value> An entry for the entity that owns this navigation property. </value>
    public override DbEntityEntry EntityEntry
    {
      get
      {
        return new DbEntityEntry(this._internalReferenceEntry.InternalEntityEntry);
      }
    }

    internal override InternalMemberEntry InternalMemberEntry
    {
      get
      {
        return (InternalMemberEntry) this._internalReferenceEntry;
      }
    }

    /// <summary>
    /// Returns the equivalent generic <see cref="T:System.Data.Entity.Infrastructure.DbReferenceEntry`2" /> object.
    /// </summary>
    /// <typeparam name="TEntity"> The type of entity on which the member is declared. </typeparam>
    /// <typeparam name="TProperty"> The type of the property. </typeparam>
    /// <returns> The equivalent generic object. </returns>
    public DbReferenceEntry<TEntity, TProperty> Cast<TEntity, TProperty>() where TEntity : class
    {
      MemberEntryMetadata entryMetadata = this._internalReferenceEntry.EntryMetadata;
      if (!typeof (TEntity).IsAssignableFrom(entryMetadata.DeclaringType) || !typeof (TProperty).IsAssignableFrom(entryMetadata.ElementType))
        throw Error.DbMember_BadTypeForCast((object) typeof (DbReferenceEntry).Name, (object) typeof (TEntity).Name, (object) typeof (TProperty).Name, (object) entryMetadata.DeclaringType.Name, (object) entryMetadata.MemberType.Name);
      return DbReferenceEntry<TEntity, TProperty>.Create(this._internalReferenceEntry);
    }
  }
}
