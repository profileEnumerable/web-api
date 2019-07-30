﻿// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.BufferedDataRecord
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal abstract class BufferedDataRecord
  {
    protected int _currentRowNumber = -1;
    protected int _rowCount;
    private string[] _dataTypeNames;
    private Type[] _fieldTypes;
    private string[] _columnNames;
    private Lazy<FieldNameLookup> _fieldNameLookup;

    protected virtual void ReadMetadata(
      string providerManifestToken,
      DbProviderServices providerServices,
      DbDataReader reader)
    {
      int fieldCount = reader.FieldCount;
      string[] strArray = new string[fieldCount];
      Type[] typeArray = new Type[fieldCount];
      string[] columnNames = new string[fieldCount];
      for (int ordinal = 0; ordinal < fieldCount; ++ordinal)
      {
        strArray[ordinal] = reader.GetDataTypeName(ordinal);
        typeArray[ordinal] = reader.GetFieldType(ordinal);
        columnNames[ordinal] = reader.GetName(ordinal);
      }
      this._dataTypeNames = strArray;
      this._fieldTypes = typeArray;
      this._columnNames = columnNames;
      this._fieldNameLookup = new Lazy<FieldNameLookup>((Func<FieldNameLookup>) (() => new FieldNameLookup(new ReadOnlyCollection<string>((IList<string>) columnNames))), false);
    }

    public bool IsDataReady { get; protected set; }

    public bool HasRows
    {
      get
      {
        return this._rowCount > 0;
      }
    }

    public int FieldCount
    {
      get
      {
        return this._dataTypeNames.Length;
      }
    }

    public abstract bool GetBoolean(int ordinal);

    public abstract byte GetByte(int ordinal);

    public abstract char GetChar(int ordinal);

    public abstract DateTime GetDateTime(int ordinal);

    public abstract Decimal GetDecimal(int ordinal);

    public abstract double GetDouble(int ordinal);

    public abstract float GetFloat(int ordinal);

    public abstract Guid GetGuid(int ordinal);

    public abstract short GetInt16(int ordinal);

    public abstract int GetInt32(int ordinal);

    public abstract long GetInt64(int ordinal);

    public abstract string GetString(int ordinal);

    public abstract T GetFieldValue<T>(int ordinal);

    public abstract Task<T> GetFieldValueAsync<T>(
      int ordinal,
      CancellationToken cancellationToken);

    public abstract object GetValue(int ordinal);

    public abstract int GetValues(object[] values);

    public abstract bool IsDBNull(int ordinal);

    public abstract Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken);

    public string GetDataTypeName(int ordinal)
    {
      return this._dataTypeNames[ordinal];
    }

    public Type GetFieldType(int ordinal)
    {
      return this._fieldTypes[ordinal];
    }

    public string GetName(int ordinal)
    {
      return this._columnNames[ordinal];
    }

    public int GetOrdinal(string name)
    {
      return this._fieldNameLookup.Value.GetOrdinal(name);
    }

    public abstract bool Read();

    public abstract Task<bool> ReadAsync(CancellationToken cancellationToken);
  }
}
