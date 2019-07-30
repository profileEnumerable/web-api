// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectStateEntryDbUpdatableDataRecord
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class ObjectStateEntryDbUpdatableDataRecord : CurrentValueRecord
  {
    internal ObjectStateEntryDbUpdatableDataRecord(
      EntityEntry cacheEntry,
      StateManagerTypeMetadata metadata,
      object userObject)
      : base((ObjectStateEntry) cacheEntry, metadata, userObject)
    {
      switch (cacheEntry.State)
      {
      }
    }

    internal ObjectStateEntryDbUpdatableDataRecord(RelationshipEntry cacheEntry)
      : base((ObjectStateEntry) cacheEntry)
    {
      switch (cacheEntry.State)
      {
      }
    }

    protected override object GetRecordValue(int ordinal)
    {
      if (this._cacheEntry.IsRelationship)
        return (this._cacheEntry as RelationshipEntry).GetCurrentRelationValue(ordinal);
      return (this._cacheEntry as EntityEntry).GetCurrentEntityValue(this._metadata, ordinal, this._userObject, ObjectStateValueRecord.CurrentUpdatable);
    }

    protected override void SetRecordValue(int ordinal, object value)
    {
      if (this._cacheEntry.IsRelationship)
        throw new InvalidOperationException(Strings.ObjectStateEntry_CantModifyRelationValues);
      (this._cacheEntry as EntityEntry).SetCurrentEntityValue(this._metadata, ordinal, this._userObject, value);
    }
  }
}
