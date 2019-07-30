// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbChangeTracker
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Returned by the ChangeTracker method of <see cref="T:System.Data.Entity.DbContext" /> to provide access to features of
  /// the context that are related to change tracking of entities.
  /// </summary>
  public class DbChangeTracker
  {
    private readonly InternalContext _internalContext;

    internal DbChangeTracker(InternalContext internalContext)
    {
      this._internalContext = internalContext;
    }

    /// <summary>
    /// Gets <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry" /> objects for all the entities tracked by this context.
    /// </summary>
    /// <returns> The entries. </returns>
    public IEnumerable<DbEntityEntry> Entries()
    {
      return this._internalContext.GetStateEntries().Select<IEntityStateEntry, DbEntityEntry>((Func<IEntityStateEntry, DbEntityEntry>) (e => new DbEntityEntry(new InternalEntityEntry(this._internalContext, e))));
    }

    /// <summary>
    /// Gets <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry" /> objects for all the entities of the given type
    /// tracked by this context.
    /// </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    /// <returns> The entries. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public IEnumerable<DbEntityEntry<TEntity>> Entries<TEntity>() where TEntity : class
    {
      return this._internalContext.GetStateEntries<TEntity>().Select<IEntityStateEntry, DbEntityEntry<TEntity>>((Func<IEntityStateEntry, DbEntityEntry<TEntity>>) (e => new DbEntityEntry<TEntity>(new InternalEntityEntry(this._internalContext, e))));
    }

    /// <summary>
    /// Checks if the <see cref="T:System.Data.Entity.DbContext" /> is tracking any new, deleted, or changed entities or
    /// relationships that will be sent to the database if <see cref="M:System.Data.Entity.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <remarks>
    /// Functionally, calling this method is equivalent to checking if there are any entities or
    /// relationships in the Added, Updated, or Deleted state.
    /// Note that this method calls <see cref="M:System.Data.Entity.Infrastructure.DbChangeTracker.DetectChanges" /> unless
    /// <see cref="P:System.Data.Entity.Infrastructure.DbContextConfiguration.AutoDetectChangesEnabled" /> has been set to false.
    /// </remarks>
    /// <returns>
    /// True if underlying <see cref="T:System.Data.Entity.DbContext" /> have changes, else false.
    /// </returns>
    public bool HasChanges()
    {
      this._internalContext.DetectChanges(false);
      return this._internalContext.ObjectContext.ObjectStateManager.HasChanges();
    }

    /// <summary>
    /// Detects changes made to the properties and relationships of POCO entities.  Note that some types of
    /// entity (such as change tracking proxies and entities that derive from
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityObject" />)
    /// report changes automatically and a call to DetectChanges is not normally needed for these types of entities.
    /// Also note that normally DetectChanges is called automatically by many of the methods of <see cref="T:System.Data.Entity.DbContext" />
    /// and its related classes such that it is rare that this method will need to be called explicitly.
    /// However, it may be desirable, usually for performance reasons, to turn off this automatic calling of
    /// DetectChanges using the AutoDetectChangesEnabled flag from <see cref="P:System.Data.Entity.DbContext.Configuration" />.
    /// </summary>
    public void DetectChanges()
    {
      this._internalContext.DetectChanges(true);
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
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
