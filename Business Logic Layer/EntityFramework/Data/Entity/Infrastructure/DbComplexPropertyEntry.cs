// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbComplexPropertyEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// A non-generic version of the <see cref="T:System.Data.Entity.Infrastructure.DbComplexPropertyEntry`2" /> class.
  /// </summary>
  public class DbComplexPropertyEntry : DbPropertyEntry
  {
    internal static DbComplexPropertyEntry Create(
      InternalPropertyEntry internalPropertyEntry)
    {
      return (DbComplexPropertyEntry) internalPropertyEntry.CreateDbMemberEntry();
    }

    internal DbComplexPropertyEntry(InternalPropertyEntry internalPropertyEntry)
      : base(internalPropertyEntry)
    {
    }

    /// <summary>
    /// Gets an object that represents a nested property of this property.
    /// This method can be used for both scalar or complex properties.
    /// </summary>
    /// <param name="propertyName"> The name of the nested property. </param>
    /// <returns> An object representing the nested property. </returns>
    public DbPropertyEntry Property(string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      return DbPropertyEntry.Create(((InternalPropertyEntry) this.InternalMemberEntry).Property(propertyName, (Type) null, false));
    }

    /// <summary>
    /// Gets an object that represents a nested complex property of this property.
    /// </summary>
    /// <param name="propertyName"> The name of the nested property. </param>
    /// <returns> An object representing the nested property. </returns>
    [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", Justification = "Rule predates more fluent naming conventions.", MessageId = "0#")]
    public DbComplexPropertyEntry ComplexProperty(string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      return DbComplexPropertyEntry.Create(((InternalPropertyEntry) this.InternalMemberEntry).Property(propertyName, (Type) null, true));
    }

    /// <summary>
    /// Returns the equivalent generic <see cref="T:System.Data.Entity.Infrastructure.DbComplexPropertyEntry`2" /> object.
    /// </summary>
    /// <typeparam name="TEntity"> The type of entity on which the member is declared. </typeparam>
    /// <typeparam name="TComplexProperty"> The type of the complex property. </typeparam>
    /// <returns> The equivalent generic object. </returns>
    public DbComplexPropertyEntry<TEntity, TComplexProperty> Cast<TEntity, TComplexProperty>() where TEntity : class
    {
      MemberEntryMetadata entryMetadata = this.InternalMemberEntry.EntryMetadata;
      if (!typeof (TEntity).IsAssignableFrom(entryMetadata.DeclaringType) || !typeof (TComplexProperty).IsAssignableFrom(entryMetadata.ElementType))
        throw Error.DbMember_BadTypeForCast((object) typeof (DbComplexPropertyEntry).Name, (object) typeof (TEntity).Name, (object) typeof (TComplexProperty).Name, (object) entryMetadata.DeclaringType.Name, (object) entryMetadata.MemberType.Name);
      return DbComplexPropertyEntry<TEntity, TComplexProperty>.Create((InternalPropertyEntry) this.InternalMemberEntry);
    }
  }
}
