// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbMemberEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// This is an abstract base class use to represent a scalar or complex property, or a navigation property
  /// of an entity.  Scalar and complex properties use the derived class <see cref="T:System.Data.Entity.Infrastructure.DbPropertyEntry" />,
  /// reference navigation properties use the derived class <see cref="T:System.Data.Entity.Infrastructure.DbReferenceEntry" />, and collection
  /// navigation properties use the derived class <see cref="T:System.Data.Entity.Infrastructure.DbCollectionEntry" />.
  /// </summary>
  public abstract class DbMemberEntry
  {
    internal static DbMemberEntry Create(InternalMemberEntry internalMemberEntry)
    {
      return internalMemberEntry.CreateDbMemberEntry();
    }

    /// <summary>Gets the name of the property.</summary>
    /// <value> The property name. </value>
    public abstract string Name { get; }

    /// <summary>Gets or sets the current value of this property.</summary>
    /// <value> The current value. </value>
    public abstract object CurrentValue { get; set; }

    /// <summary>
    /// The <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry" /> to which this member belongs.
    /// </summary>
    /// <value> An entry for the entity that owns this member. </value>
    public abstract DbEntityEntry EntityEntry { get; }

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

    internal abstract InternalMemberEntry InternalMemberEntry { get; }

    /// <summary>
    /// Returns the equivalent generic <see cref="T:System.Data.Entity.Infrastructure.DbMemberEntry`2" /> object.
    /// </summary>
    /// <typeparam name="TEntity"> The type of entity on which the member is declared. </typeparam>
    /// <typeparam name="TProperty"> The type of the property. </typeparam>
    /// <returns> The equivalent generic object. </returns>
    public DbMemberEntry<TEntity, TProperty> Cast<TEntity, TProperty>() where TEntity : class
    {
      MemberEntryMetadata entryMetadata = this.InternalMemberEntry.EntryMetadata;
      if (!typeof (TEntity).IsAssignableFrom(entryMetadata.DeclaringType) || !typeof (TProperty).IsAssignableFrom(entryMetadata.MemberType))
        throw Error.DbMember_BadTypeForCast((object) typeof (DbMemberEntry).Name, (object) typeof (TEntity).Name, (object) typeof (TProperty).Name, (object) entryMetadata.DeclaringType.Name, (object) entryMetadata.MemberType.Name);
      return DbMemberEntry<TEntity, TProperty>.Create(this.InternalMemberEntry);
    }
  }
}
