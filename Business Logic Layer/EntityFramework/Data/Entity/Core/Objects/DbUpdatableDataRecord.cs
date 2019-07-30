// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DbUpdatableDataRecord
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
  /// <summary>
  /// Provides access to the original values of object data. The DbUpdatableDataRecord implements methods that allow updates to the original values of an object.
  /// </summary>
  public abstract class DbUpdatableDataRecord : DbDataRecord, IExtendedDataRecord, IDataRecord
  {
    internal readonly StateManagerTypeMetadata _metadata;
    internal readonly ObjectStateEntry _cacheEntry;
    internal readonly object _userObject;
    internal DataRecordInfo _recordInfo;

    internal DbUpdatableDataRecord(
      ObjectStateEntry cacheEntry,
      StateManagerTypeMetadata metadata,
      object userObject)
    {
      this._cacheEntry = cacheEntry;
      this._userObject = userObject;
      this._metadata = metadata;
    }

    internal DbUpdatableDataRecord(ObjectStateEntry cacheEntry)
      : this(cacheEntry, (StateManagerTypeMetadata) null, (object) null)
    {
    }

    /// <summary>Gets the number of fields in the record.</summary>
    /// <returns>An integer value that is the field count.</returns>
    public override int FieldCount
    {
      get
      {
        return this._cacheEntry.GetFieldCount(this._metadata);
      }
    }

    /// <summary>Returns a value that has the given field ordinal.</summary>
    /// <returns>The value that has the given field ordinal.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override object this[int i]
    {
      get
      {
        return this.GetValue(i);
      }
    }

    /// <summary>Gets a value that has the given field name.</summary>
    /// <returns>The field value.</returns>
    /// <param name="name">The name of the field.</param>
    public override object this[string name]
    {
      get
      {
        return this.GetValue(this.GetOrdinal(name));
      }
    }

    /// <summary>Retrieves the field value as a Boolean.</summary>
    /// <returns>The field value as a Boolean.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override bool GetBoolean(int i)
    {
      return (bool) this.GetValue(i);
    }

    /// <summary>Retrieves the field value as a byte.</summary>
    /// <returns>The field value as a byte.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override byte GetByte(int i)
    {
      return (byte) this.GetValue(i);
    }

    /// <summary>Retrieves the field value as a byte array.</summary>
    /// <returns>The number of bytes copied.</returns>
    /// <param name="i">The ordinal of the field.</param>
    /// <param name="dataIndex">The index at which to start copying data.</param>
    /// <param name="buffer">The destination buffer where data is copied.</param>
    /// <param name="bufferIndex">The index in the destination buffer where copying will begin.</param>
    /// <param name="length">The number of bytes to copy.</param>
    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    public override long GetBytes(
      int i,
      long dataIndex,
      byte[] buffer,
      int bufferIndex,
      int length)
    {
      byte[] numArray = (byte[]) this.GetValue(i);
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

    /// <summary>Retrieves the field value as a char.</summary>
    /// <returns>The field value as a char.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override char GetChar(int i)
    {
      return (char) this.GetValue(i);
    }

    /// <summary>Retrieves the field value as a char array.</summary>
    /// <returns>The number of characters copied.</returns>
    /// <param name="i">The ordinal of the field.</param>
    /// <param name="dataIndex">The index at which to start copying data.</param>
    /// <param name="buffer">The destination buffer where data is copied.</param>
    /// <param name="bufferIndex">The index in the destination buffer where copying will begin.</param>
    /// <param name="length">The number of characters to copy.</param>
    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    public override long GetChars(
      int i,
      long dataIndex,
      char[] buffer,
      int bufferIndex,
      int length)
    {
      char[] chArray = (char[]) this.GetValue(i);
      if (buffer == null)
        return (long) chArray.Length;
      int num1 = (int) dataIndex;
      int num2 = Math.Min(chArray.Length - num1, length);
      if (num1 < 0)
        throw new ArgumentOutOfRangeException(nameof (dataIndex), Strings.ADP_InvalidSourceBufferIndex((object) chArray.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) ((long) num1).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
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

    IDataReader IDataRecord.GetData(int ordinal)
    {
      return (IDataReader) this.GetDbDataReader(ordinal);
    }

    /// <summary>
    /// Retrieves the field value as a <see cref="T:System.Common.DbDataReader" />
    /// </summary>
    /// <returns>
    /// The field value as a <see cref="T:System.Data.Common.DbDataReader" />.
    /// </returns>
    /// <param name="i">The ordinal of the field.</param>
    protected override DbDataReader GetDbDataReader(int i)
    {
      throw new NotSupportedException();
    }

    /// <summary>Retrieves the name of the field data type.</summary>
    /// <returns>The name of the field data type.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override string GetDataTypeName(int i)
    {
      return this.GetFieldType(i).Name;
    }

    /// <summary>
    /// Retrieves the field value as a <see cref="T:System.DateTime" />.
    /// </summary>
    /// <returns>
    /// The field value as a <see cref="T:System.DateTime" />.
    /// </returns>
    /// <param name="i">The ordinal of the field.</param>
    public override DateTime GetDateTime(int i)
    {
      return (DateTime) this.GetValue(i);
    }

    /// <summary>Retrieves the field value as a decimal.</summary>
    /// <returns>The field value as a decimal.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override Decimal GetDecimal(int i)
    {
      return (Decimal) this.GetValue(i);
    }

    /// <summary>Retrieves the field value as a double.</summary>
    /// <returns>The field value as a double.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override double GetDouble(int i)
    {
      return (double) this.GetValue(i);
    }

    /// <summary>Retrieves the type of a field.</summary>
    /// <returns>The field type.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override Type GetFieldType(int i)
    {
      return this._cacheEntry.GetFieldType(i, this._metadata);
    }

    /// <summary>Retrieves the field value as a float.</summary>
    /// <returns>The field value as a float.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override float GetFloat(int i)
    {
      return (float) this.GetValue(i);
    }

    /// <summary>
    /// Retrieves the field value as a <see cref="T:System.Guid" />.
    /// </summary>
    /// <returns>
    /// The field value as a <see cref="T:System.Guid" />.
    /// </returns>
    /// <param name="i">The ordinal of the field.</param>
    public override Guid GetGuid(int i)
    {
      return (Guid) this.GetValue(i);
    }

    /// <summary>
    /// Retrieves the field value as an <see cref="T:System.Int16" />.
    /// </summary>
    /// <returns>
    /// The field value as an <see cref="T:System.Int16" />.
    /// </returns>
    /// <param name="i">The ordinal of the field.</param>
    public override short GetInt16(int i)
    {
      return (short) this.GetValue(i);
    }

    /// <summary>
    /// Retrieves the field value as an <see cref="T:System.Int32" />.
    /// </summary>
    /// <returns>
    /// The field value as an <see cref="T:System.Int32" />.
    /// </returns>
    /// <param name="i">The ordinal of the field.</param>
    public override int GetInt32(int i)
    {
      return (int) this.GetValue(i);
    }

    /// <summary>
    /// Retrieves the field value as an <see cref="T:System.Int64" />.
    /// </summary>
    /// <returns>
    /// The field value as an <see cref="T:System.Int64" />.
    /// </returns>
    /// <param name="i">The ordinal of the field.</param>
    public override long GetInt64(int i)
    {
      return (long) this.GetValue(i);
    }

    /// <summary>Retrieves the name of a field.</summary>
    /// <returns>The name of the field.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override string GetName(int i)
    {
      return this._cacheEntry.GetCLayerName(i, this._metadata);
    }

    /// <summary>Retrieves the ordinal of a field by using the name of the field.</summary>
    /// <returns>The ordinal of the field.</returns>
    /// <param name="name">The name of the field.</param>
    public override int GetOrdinal(string name)
    {
      int ordinalforClayerName = this._cacheEntry.GetOrdinalforCLayerName(name, this._metadata);
      if (ordinalforClayerName == -1)
        throw new ArgumentOutOfRangeException(nameof (name));
      return ordinalforClayerName;
    }

    /// <summary>Retrieves the field value as a string.</summary>
    /// <returns>The field value.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override string GetString(int i)
    {
      return (string) this.GetValue(i);
    }

    /// <summary>Retrieves the value of a field.</summary>
    /// <returns>The field value.</returns>
    /// <param name="i">The ordinal of the field.</param>
    public override object GetValue(int i)
    {
      return this.GetRecordValue(i);
    }

    /// <summary>Retrieves the value of a field.</summary>
    /// <returns>The field value.</returns>
    /// <param name="ordinal">The ordinal of the field.</param>
    protected abstract object GetRecordValue(int ordinal);

    /// <summary>Populates an array of objects with the field values of the current record.</summary>
    /// <returns>The number of field values returned.</returns>
    /// <param name="values">An array of objects to store the field values.</param>
    public override int GetValues(object[] values)
    {
      Check.NotNull<object[]>(values, nameof (values));
      int num = Math.Min(values.Length, this.FieldCount);
      for (int i = 0; i < num; ++i)
        values[i] = this.GetValue(i);
      return num;
    }

    /// <summary>
    /// Returns whether the specified field is set to <see cref="T:System.DBNull" />.
    /// </summary>
    /// <returns>
    /// true if the field is set to <see cref="T:System.DBNull" />; otherwise false.
    /// </returns>
    /// <param name="i">The ordinal of the field.</param>
    public override bool IsDBNull(int i)
    {
      return this.GetValue(i) == DBNull.Value;
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetBoolean(int ordinal, bool value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetByte(int ordinal, byte value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetChar(int ordinal, char value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetDataRecord(int ordinal, IDataRecord value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetDateTime(int ordinal, DateTime value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetDecimal(int ordinal, Decimal value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetDouble(int ordinal, double value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetFloat(int ordinal, float value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetGuid(int ordinal, Guid value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetInt16(int ordinal, short value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetInt32(int ordinal, int value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetInt64(int ordinal, long value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetString(int ordinal, string value)
    {
      this.SetValue(ordinal, (object) value);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    public void SetValue(int ordinal, object value)
    {
      this.SetRecordValue(ordinal, value);
    }

    /// <summary>Sets field values in a record.</summary>
    /// <returns>The number of the fields that were set.</returns>
    /// <param name="values">The values of the field.</param>
    public int SetValues(params object[] values)
    {
      int num = Math.Min(values.Length, this.FieldCount);
      for (int ordinal = 0; ordinal < num; ++ordinal)
        this.SetRecordValue(ordinal, values[ordinal]);
      return num;
    }

    /// <summary>
    /// Sets a field to the <see cref="T:System.DBNull" /> value.
    /// </summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    public void SetDBNull(int ordinal)
    {
      this.SetRecordValue(ordinal, (object) DBNull.Value);
    }

    /// <summary>Gets data record information.</summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.DataRecordInfo" /> object.
    /// </returns>
    public virtual DataRecordInfo DataRecordInfo
    {
      get
      {
        if (this._recordInfo == null)
          this._recordInfo = this._cacheEntry.GetDataRecordInfo(this._metadata, this._userObject);
        return this._recordInfo;
      }
    }

    /// <summary>
    /// Retrieves a field value as a <see cref="T:System.Data.Common.DbDataRecord" />.
    /// </summary>
    /// <returns>
    /// A field value as a <see cref="T:System.Data.Common.DbDataRecord" />.
    /// </returns>
    /// <param name="i">The ordinal of the field.</param>
    public DbDataRecord GetDataRecord(int i)
    {
      return (DbDataRecord) this.GetValue(i);
    }

    /// <summary>
    /// Retrieves the field value as a <see cref="T:System.Common.DbDataReader" />.
    /// </summary>
    /// <returns>
    /// The field value as a <see cref="T:System.Data.Common.DbDataReader" />.
    /// </returns>
    /// <param name="i">The ordinal of the field.</param>
    public DbDataReader GetDataReader(int i)
    {
      return this.GetDbDataReader(i);
    }

    /// <summary>Sets the value of a field in a record.</summary>
    /// <param name="ordinal">The ordinal of the field.</param>
    /// <param name="value">The value of the field.</param>
    protected abstract void SetRecordValue(int ordinal, object value);
  }
}
