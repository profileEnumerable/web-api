// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbPropertyEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// A non-generic version of the <see cref="T:System.Data.Entity.Infrastructure.DbPropertyEntry`2" /> class.
  /// </summary>
  public class DbPropertyEntry : DbMemberEntry
  {
    private readonly InternalPropertyEntry _internalPropertyEntry;

    internal static DbPropertyEntry Create(
      InternalPropertyEntry internalPropertyEntry)
    {
      return (DbPropertyEntry) internalPropertyEntry.CreateDbMemberEntry();
    }

    internal DbPropertyEntry(InternalPropertyEntry internalPropertyEntry)
    {
      this._internalPropertyEntry = internalPropertyEntry;
    }

    /// <summary>Gets the property name.</summary>
    /// <value> The property name. </value>
    public override string Name
    {
      get
      {
        return this._internalPropertyEntry.Name;
      }
    }

    /// <summary>Gets or sets the original value of this property.</summary>
    /// <value> The original value. </value>
    public object OriginalValue
    {
      get
      {
        return this._internalPropertyEntry.OriginalValue;
      }
      set
      {
        this._internalPropertyEntry.OriginalValue = value;
      }
    }

    /// <summary>Gets or sets the current value of this property.</summary>
    /// <value> The current value. </value>
    public override object CurrentValue
    {
      get
      {
        return this._internalPropertyEntry.CurrentValue;
      }
      set
      {
        this._internalPropertyEntry.CurrentValue = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the value of this property has been modified since
    /// it was loaded from the database.
    /// </summary>
    /// <remarks>
    /// Setting this value to false for a modified property will revert the change by setting the
    /// current value to the original value. If the result is that no properties of the entity are
    /// marked as modified, then the entity will be marked as Unchanged.
    /// Setting this value to false for properties of Added, Unchanged, or Deleted entities
    /// is a no-op.
    /// </remarks>
    /// <value>
    /// <c>true</c> if this instance is modified; otherwise, <c>false</c> .
    /// </value>
    public bool IsModified
    {
      get
      {
        return this._internalPropertyEntry.IsModified;
      }
      set
      {
        this._internalPropertyEntry.IsModified = value;
      }
    }

    /// <summary>
    /// The <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry" /> to which this property belongs.
    /// </summary>
    /// <value> An entry for the entity that owns this property. </value>
    public override DbEntityEntry EntityEntry
    {
      get
      {
        return new DbEntityEntry(this._internalPropertyEntry.InternalEntityEntry);
      }
    }

    /// <summary>
    /// The <see cref="T:System.Data.Entity.Infrastructure.DbPropertyEntry" /> of the property for which this is a nested property.
    /// This method will only return a non-null entry for properties of complex objects; it will
    /// return null for properties of the entity itself.
    /// </summary>
    /// <value> An entry for the parent complex property, or null if this is an entity property. </value>
    public DbComplexPropertyEntry ParentProperty
    {
      get
      {
        InternalPropertyEntry parentPropertyEntry = this._internalPropertyEntry.ParentPropertyEntry;
        if (parentPropertyEntry == null)
          return (DbComplexPropertyEntry) null;
        return DbComplexPropertyEntry.Create(parentPropertyEntry);
      }
    }

    internal override InternalMemberEntry InternalMemberEntry
    {
      get
      {
        return (InternalMemberEntry) this._internalPropertyEntry;
      }
    }

    /// <summary>
    /// Returns the equivalent generic <see cref="T:System.Data.Entity.Infrastructure.DbPropertyEntry`2" /> object.
    /// </summary>
    /// <typeparam name="TEntity"> The type of entity on which the member is declared. </typeparam>
    /// <typeparam name="TProperty"> The type of the property. </typeparam>
    /// <returns> The equivalent generic object. </returns>
    public DbPropertyEntry<TEntity, TProperty> Cast<TEntity, TProperty>() where TEntity : class
    {
      PropertyEntryMetadata entryMetadata = this._internalPropertyEntry.EntryMetadata;
      if (!typeof (TEntity).IsAssignableFrom(entryMetadata.DeclaringType) || !typeof (TProperty).IsAssignableFrom(entryMetadata.ElementType))
        throw Error.DbMember_BadTypeForCast((object) typeof (DbPropertyEntry).Name, (object) typeof (TEntity).Name, (object) typeof (TProperty).Name, (object) entryMetadata.DeclaringType.Name, (object) entryMetadata.MemberType.Name);
      return DbPropertyEntry<TEntity, TProperty>.Create(this._internalPropertyEntry);
    }
  }
}
