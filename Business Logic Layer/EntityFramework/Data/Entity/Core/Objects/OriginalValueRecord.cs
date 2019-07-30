// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.OriginalValueRecord
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// The original values of the properties of an entity when it was retrieved from the database.
  /// </summary>
  public abstract class OriginalValueRecord : DbUpdatableDataRecord
  {
    internal OriginalValueRecord(
      ObjectStateEntry cacheEntry,
      StateManagerTypeMetadata metadata,
      object userObject)
      : base(cacheEntry, metadata, userObject)
    {
    }
  }
}
