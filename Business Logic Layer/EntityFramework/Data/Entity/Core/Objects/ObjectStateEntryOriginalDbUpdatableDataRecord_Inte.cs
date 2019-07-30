// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectStateEntryOriginalDbUpdatableDataRecord_Internal
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Objects
{
  internal class ObjectStateEntryOriginalDbUpdatableDataRecord_Internal : OriginalValueRecord
  {
    internal ObjectStateEntryOriginalDbUpdatableDataRecord_Internal(
      EntityEntry cacheEntry,
      StateManagerTypeMetadata metadata,
      object userObject)
      : base((ObjectStateEntry) cacheEntry, metadata, userObject)
    {
      switch (cacheEntry.State)
      {
      }
    }

    protected override object GetRecordValue(int ordinal)
    {
      return (this._cacheEntry as EntityEntry).GetOriginalEntityValue(this._metadata, ordinal, this._userObject, ObjectStateValueRecord.OriginalUpdatableInternal);
    }

    protected override void SetRecordValue(int ordinal, object value)
    {
      (this._cacheEntry as EntityEntry).SetOriginalEntityValue(this._metadata, ordinal, this._userObject, value);
    }
  }
}
