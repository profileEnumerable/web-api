// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectStateEntryOriginalDbUpdatableDataRecord_Public
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class ObjectStateEntryOriginalDbUpdatableDataRecord_Public : ObjectStateEntryOriginalDbUpdatableDataRecord_Internal
  {
    private readonly int _parentEntityPropertyIndex;

    internal ObjectStateEntryOriginalDbUpdatableDataRecord_Public(
      EntityEntry cacheEntry,
      StateManagerTypeMetadata metadata,
      object userObject,
      int parentEntityPropertyIndex)
      : base(cacheEntry, metadata, userObject)
    {
      this._parentEntityPropertyIndex = parentEntityPropertyIndex;
    }

    protected override object GetRecordValue(int ordinal)
    {
      return (this._cacheEntry as EntityEntry).GetOriginalEntityValue(this._metadata, ordinal, this._userObject, ObjectStateValueRecord.OriginalUpdatablePublic, this.GetPropertyIndex(ordinal));
    }

    protected override void SetRecordValue(int ordinal, object value)
    {
      StateManagerMemberMetadata managerMemberMetadata = this._metadata.Member(ordinal);
      if (managerMemberMetadata.IsComplex)
        throw new InvalidOperationException(Strings.ObjectStateEntry_SetOriginalComplexProperties((object) managerMemberMetadata.CLayerName));
      object newFieldValue = value ?? (object) DBNull.Value;
      EntityEntry cacheEntry = this._cacheEntry as EntityEntry;
      EntityState state = cacheEntry.State;
      if (!cacheEntry.HasRecordValueChanged((DbDataRecord) this, ordinal, newFieldValue))
        return;
      if (managerMemberMetadata.IsPartOfKey)
        throw new InvalidOperationException(Strings.ObjectStateEntry_SetOriginalPrimaryKey((object) managerMemberMetadata.CLayerName));
      Type clrType = managerMemberMetadata.ClrType;
      if (DBNull.Value == newFieldValue && clrType.IsValueType() && !managerMemberMetadata.CdmMetadata.Nullable)
        throw new InvalidOperationException(Strings.ObjectStateEntry_NullOriginalValueForNonNullableProperty((object) managerMemberMetadata.CLayerName, (object) managerMemberMetadata.ClrMetadata.Name, (object) managerMemberMetadata.ClrMetadata.DeclaringType.FullName));
      base.SetRecordValue(ordinal, value);
      if (state == EntityState.Unchanged && cacheEntry.State == EntityState.Modified)
        cacheEntry.ObjectStateManager.ChangeState(cacheEntry, state, EntityState.Modified);
      cacheEntry.SetModifiedPropertyInternal(this.GetPropertyIndex(ordinal));
    }

    private int GetPropertyIndex(int ordinal)
    {
      if (this._parentEntityPropertyIndex != -1)
        return this._parentEntityPropertyIndex;
      return ordinal;
    }
  }
}
