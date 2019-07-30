// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbMemberEntry`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Internal;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// This is an abstract base class use to represent a scalar or complex property, or a navigation property
  /// of an entity.  Scalar and complex properties use the derived class <see cref="T:System.Data.Entity.Infrastructure.DbPropertyEntry`2" />,
  /// reference navigation properties use the derived class <see cref="T:System.Data.Entity.Infrastructure.DbReferenceEntry`2" />, and collection
  /// navigation properties use the derived class <see cref="T:System.Data.Entity.Infrastructure.DbCollectionEntry`2" />.
  /// </summary>
  /// <typeparam name="TEntity"> The type of the entity to which this property belongs. </typeparam>
  /// <typeparam name="TProperty"> The type of the property. </typeparam>
  public abstract class DbMemberEntry<TEntity, TProperty> where TEntity : class
  {
    internal static DbMemberEntry<TEntity, TProperty> Create(
      InternalMemberEntry internalMemberEntry)
    {
      return internalMemberEntry.CreateDbMemberEntry<TEntity, TProperty>();
    }

    /// <summary> Gets the name of the property. </summary>
    /// <returns> The name of the property. </returns>
    public abstract string Name { get; }

    /// <summary>Gets or sets the current value of this property.</summary>
    /// <value> The current value. </value>
    public abstract TProperty CurrentValue { get; set; }

    /// <summary>
    /// Returns a new instance of the non-generic <see cref="T:System.Data.Entity.Infrastructure.DbMemberEntry" /> class for
    /// the property represented by this object.
    /// </summary>
    /// <param name="entry">The object representing the property.</param>
    /// <returns> A non-generic version. </returns>
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Intentionally just implicit to reduce API clutter.")]
    public static implicit operator DbMemberEntry(
      DbMemberEntry<TEntity, TProperty> entry)
    {
      return DbMemberEntry.Create(entry.InternalMemberEntry);
    }

    internal abstract InternalMemberEntry InternalMemberEntry { get; }

    /// <summary>
    /// The <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry`1" /> to which this member belongs.
    /// </summary>
    /// <value> An entry for the entity that owns this member. </value>
    public abstract DbEntityEntry<TEntity> EntityEntry { get; }

    /// <summary>Validates this property.</summary>
    /// <returns>
    /// Collection of <see cref="T:System.Data.Entity.Validation.DbValidationError" /> objects. Never null. If the entity is valid the collection will be empty.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public ICollection<DbValidationError> GetValidationErrors()
    {
      return (ICollection<DbValidationError>) this.InternalMemberEntry.GetValidationErrors().ToList<DbValidationError>();
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
