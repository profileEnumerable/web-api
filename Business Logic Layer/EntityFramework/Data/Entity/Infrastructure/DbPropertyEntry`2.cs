// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbPropertyEntry`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Instances of this class are returned from the Property method of
  /// <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry`1" /> and allow access to the state of the scalar
  /// or complex property.
  /// </summary>
  /// <typeparam name="TEntity"> The type of the entity to which this property belongs. </typeparam>
  /// <typeparam name="TProperty"> The type of the property. </typeparam>
  public class DbPropertyEntry<TEntity, TProperty> : DbMemberEntry<TEntity, TProperty> where TEntity : class
  {
    private readonly InternalPropertyEntry _internalPropertyEntry;

    internal static DbPropertyEntry<TEntity, TProperty> Create(
      InternalPropertyEntry internalPropertyEntry)
    {
      return (DbPropertyEntry<TEntity, TProperty>) internalPropertyEntry.CreateDbMemberEntry<TEntity, TProperty>();
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
    public TProperty OriginalValue
    {
      get
      {
        return (TProperty) this._internalPropertyEntry.OriginalValue;
      }
      set
      {
        this._internalPropertyEntry.OriginalValue = (object) value;
      }
    }

    /// <summary>Gets or sets the current value of this property.</summary>
    /// <value> The current value. </value>
    public override TProperty CurrentValue
    {
      get
      {
        return (TProperty) this._internalPropertyEntry.CurrentValue;
      }
      set
      {
        this._internalPropertyEntry.CurrentValue = (object) value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the value of this property has been modified since
    /// it was loaded from the database.
    /// </summary>
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
    /// Returns a new instance of the non-generic <see cref="T:System.Data.Entity.Infrastructure.DbPropertyEntry" /> class for
    /// the property represented by this object.
    /// </summary>
    /// <param name="entry">The object representing the property.</param>
    /// <returns> A non-generic version. </returns>
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Intentionally just implicit to reduce API clutter.")]
    public static implicit operator DbPropertyEntry(
      DbPropertyEntry<TEntity, TProperty> entry)
    {
      return DbPropertyEntry.Create(entry._internalPropertyEntry);
    }

    /// <summary>
    /// The <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry`1" /> to which this property belongs.
    /// </summary>
    /// <value> An entry for the entity that owns this property. </value>
    public override DbEntityEntry<TEntity> EntityEntry
    {
      get
      {
        return new DbEntityEntry<TEntity>(this._internalPropertyEntry.InternalEntityEntry);
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

    internal InternalPropertyEntry InternalPropertyEntry
    {
      get
      {
        return this._internalPropertyEntry;
      }
    }

    internal override InternalMemberEntry InternalMemberEntry
    {
      get
      {
        return (InternalMemberEntry) this.InternalPropertyEntry;
      }
    }
  }
}
