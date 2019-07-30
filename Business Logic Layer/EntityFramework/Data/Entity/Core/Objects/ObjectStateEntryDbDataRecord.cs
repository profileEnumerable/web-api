// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectStateEntryDbDataRecord
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class ObjectStateEntryDbDataRecord : DbDataRecord, IExtendedDataRecord, IDataRecord
  {
    private readonly StateManagerTypeMetadata _metadata;
    private readonly ObjectStateEntry _cacheEntry;
    private readonly object _userObject;
    private DataRecordInfo _recordInfo;

    internal ObjectStateEntryDbDataRecord(
      EntityEntry cacheEntry,
      StateManagerTypeMetadata metadata,
      object userObject)
    {
      switch (cacheEntry.State)
      {
        case EntityState.Unchanged:
        case EntityState.Deleted:
        case EntityState.Modified:
          this._cacheEntry = (ObjectStateEntry) cacheEntry;
          this._userObject = userObject;
          this._metadata = metadata;
          break;
      }
    }

    internal ObjectStateEntryDbDataRecord(RelationshipEntry cacheEntry)
    {
      switch (cacheEntry.State)
      {
        case EntityState.Unchanged:
        case EntityState.Deleted:
        case EntityState.Modified:
          this._cacheEntry = (ObjectStateEntry) cacheEntry;
          break;
      }
    }

    public override int FieldCount
    {
      get
      {
        return this._cacheEntry.GetFieldCount(this._metadata);
      }
    }

    public override object this[int ordinal]
    {
      get
      {
        return this.GetValue(ordinal);
      }
    }

    public override object this[string name]
    {
      get
      {
        return this.GetValue(this.GetOrdinal(name));
      }
    }

    public override bool GetBoolean(int ordinal)
    {
      return (bool) this.GetValue(ordinal);
    }

    public override byte GetByte(int ordinal)
    {
      return (byte) this.GetValue(ordinal);
    }

    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    public override long GetBytes(
      int ordinal,
      long dataIndex,
      byte[] buffer,
      int bufferIndex,
      int length)
    {
      byte[] numArray = (byte[]) this.GetValue(ordinal);
      if (buffer == null)
        return (long) numArray.Length;
      int num1 = (int) dataIndex;
      int num2 = Math.Min(numArray.Length - num1, length);
      if (num1 < 0)
        throw new ArgumentOutOfRangeException(nameof (dataIndex), Strings.ADP_InvalidSourceBufferIndex((object) numArray.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) ((long) num1).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      if (bufferIndex < 0 || bufferIndex > 0 && bufferIndex >= buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (bufferIndex), Strings.ADP_InvalidDestinationBufferIndex((object) buffer.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) bufferIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      if (0 < num2)
      {
        Array.Copy((Array) numArray, dataIndex, (Array) buffer, (long) bufferIndex, (long) num2);
      }
      else
      {
        if (length < 0)
          throw new IndexOutOfRangeException(Strings.ADP_InvalidDataLength((object) ((long) length).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        num2 = 0;
      }
      return (long) num2;
    }

    public override char GetChar(int ordinal)
    {
      return (char) this.GetValue(ordinal);
    }

    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    public override long GetChars(
      int ordinal,
      long dataIndex,
      char[] buffer,
      int bufferIndex,
      int length)
    {
      char[] chArray = (char[]) this.GetValue(ordinal);
      if (buffer == null)
        return (long) chArray.Length;
      int num1 = (int) dataIndex;
      int num2 = Math.Min(chArray.Length - num1, length);
      if (num1 < 0)
        throw new ArgumentOutOfRangeException(nameof (bufferIndex), Strings.ADP_InvalidSourceBufferIndex((object) buffer.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) ((long) bufferIndex).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      if (bufferIndex < 0 || bufferIndex > 0 && bufferIndex >= buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (bufferIndex), Strings.ADP_InvalidDestinationBufferIndex((object) buffer.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) bufferIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      if (0 < num2)
      {
        Array.Copy((Array) chArray, dataIndex, (Array) buffer, (long) bufferIndex, (long) num2);
      }
      else
      {
        if (length < 0)
          throw new IndexOutOfRangeException(Strings.ADP_InvalidDataLength((object) ((long) length).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        num2 = 0;
      }
      return (long) num2;
    }

    protected override DbDataReader GetDbDataReader(int ordinal)
    {
      throw new NotSupportedException();
    }

    public override string GetDataTypeName(int ordinal)
    {
      return this.GetFieldType(ordinal).Name;
    }

    public override DateTime GetDateTime(int ordinal)
    {
      return (DateTime) this.GetValue(ordinal);
    }

    public override Decimal GetDecimal(int ordinal)
    {
      return (Decimal) this.GetValue(ordinal);
    }

    public override double GetDouble(int ordinal)
    {
      return (double) this.GetValue(ordinal);
    }

    public override Type GetFieldType(int ordinal)
    {
      return this._cacheEntry.GetFieldType(ordinal, this._metadata);
    }

    public override float GetFloat(int ordinal)
    {
      return (float) this.GetValue(ordinal);
    }

    public override Guid GetGuid(int ordinal)
    {
      return (Guid) this.GetValue(ordinal);
    }

    public override short GetInt16(int ordinal)
    {
      return (short) this.GetValue(ordinal);
    }

    public override int GetInt32(int ordinal)
    {
      return (int) this.GetValue(ordinal);
    }

    public override long GetInt64(int ordinal)
    {
      return (long) this.GetValue(ordinal);
    }

    public override string GetName(int ordinal)
    {
      return this._cacheEntry.GetCLayerName(ordinal, this._metadata);
    }

    public override int GetOrdinal(string name)
    {
      int ordinalforClayerName = this._cacheEntry.GetOrdinalforCLayerName(name, this._metadata);
      if (ordinalforClayerName == -1)
        throw new ArgumentOutOfRangeException(nameof (name));
      return ordinalforClayerName;
    }

    public override string GetString(int ordinal)
    {
      return (string) this.GetValue(ordinal);
    }

    public override object GetValue(int ordinal)
    {
      if (this._cacheEntry.IsRelationship)
        return (this._cacheEntry as RelationshipEntry).GetOriginalRelationValue(ordinal);
      return (this._cacheEntry as EntityEntry).GetOriginalEntityValue(this._metadata, ordinal, this._userObject, ObjectStateValueRecord.OriginalReadonly);
    }

    public override int GetValues(object[] values)
    {
      Check.NotNull<object[]>(values, nameof (values));
      int num = Math.Min(values.Length, this.FieldCount);
      for (int i = 0; i < num; ++i)
        values[i] = this.GetValue(i);
      return num;
    }

    public override bool IsDBNull(int ordinal)
    {
      return this.GetValue(ordinal) == DBNull.Value;
    }

    public DataRecordInfo DataRecordInfo
    {
      get
      {
        if (this._recordInfo == null)
          this._recordInfo = this._cacheEntry.GetDataRecordInfo(this._metadata, this._userObject);
        return this._recordInfo;
      }
    }

    public DbDataRecord GetDataRecord(int ordinal)
    {
      return (DbDataRecord) this.GetValue(ordinal);
    }

    public DbDataReader GetDataReader(int i)
    {
      return this.GetDbDataReader(i);
    }
  }
}
