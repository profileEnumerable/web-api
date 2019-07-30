// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.EntityState
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity
{
  /// <summary>Describes the state of an entity.</summary>
  [Flags]
  [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]
  public enum EntityState
  {
    /// <summary>
    /// The entity is not being tracked by the context.
    /// An entity is in this state immediately after it has been created with the new operator
    /// or with one of the <see cref="T:System.Data.Entity.DbSet" /> Create methods.
    /// </summary>
    Detached = 1,
    /// <summary>
    /// The entity is being tracked by the context and exists in the database, and its property
    /// values have not changed from the values in the database.
    /// </summary>
    Unchanged = 2,
    /// <summary>
    /// The entity is being tracked by the context but does not yet exist in the database.
    /// </summary>
    Added = 4,
    /// <summary>
    /// The entity is being tracked by the context and exists in the database, but has been marked
    /// for deletion from the database the next time SaveChanges is called.
    /// </summary>
    Deleted = 8,
    /// <summary>
    /// The entity is being tracked by the context and exists in the database, and some or all of its
    /// property values have been modified.
    /// </summary>
    Modified = 16, // 0x00000010
  }
}
