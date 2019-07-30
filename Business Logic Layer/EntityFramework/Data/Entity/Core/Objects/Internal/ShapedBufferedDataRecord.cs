// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.ShapedBufferedDataRecord
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Spatial;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class ShapedBufferedDataRecord : BufferedDataRecord
  {
    private int _rowCapacity = 1;
    private BitArray _bools;
    private bool[] _tempBools;
    private int _boolCount;
    private byte[] _bytes;
    private int _byteCount;
    private char[] _chars;
    private int _charCount;
    private DateTime[] _dateTimes;
    private int _dateTimeCount;
    private Decimal[] _decimals;
    private int _decimalCount;
    private double[] _doubles;
    private int _doubleCount;
    private float[] _floats;
    private int _floatCount;
    private Guid[] _guids;
    private int _guidCount;
    private short[] _shorts;
    private int _shortCount;
    private int[] _ints;
    private int _intCount;
    private long[] _longs;
    private int _longCount;
    private object[] _objects;
    private int _objectCount;
    private int[] _ordinalToIndexMap;
    private BitArray _nulls;
    private bool[] _tempNulls;
    private int _nullCount;
    private int[] _nullOrdinalToIndexMap;
    private ShapedBufferedDataRecord.TypeCase[] _columnTypeCases;

    protected ShapedBufferedDataRecord()
    {
    }

    internal static BufferedDataRecord Initialize(
      string providerManifestToken,
      DbProviderServices providerServices,
      DbDataReader reader,
      Type[] columnTypes,
      bool[] nullableColumns)
    {
      ShapedBufferedDataRecord bufferedDataRecord = new ShapedBufferedDataRecord();
      bufferedDataRecord.ReadMetadata(providerManifestToken, providerServices, reader);
      DbSpatialDataReader spatialDataReader = (DbSpatialDataReader) null;
      if (((IEnumerable<Type>) columnTypes).Any<Type>((Func<Type, bool>) (t =>
      {
        if (!(t == typeof (DbGeography)))
          return t == typeof (DbGeometry);
        return true;
      })))
        spatialDataReader = providerServices.GetSpatialDataReader(reader, providerManifestToken);
      return bufferedDataRecord.Initialize(reader, spatialDataReader, columnTypes, nullableColumns);
    }

    internal static Task<BufferedDataRecord> InitializeAsync(
      string providerManifestToken,
      DbProviderServices providerServices,
      DbDataReader reader,
      Type[] columnTypes,
      bool[] nullableColumns,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      ShapedBufferedDataRecord bufferedDataRecord = new ShapedBufferedDataRecord();
      bufferedDataRecord.ReadMetadata(providerManifestToken, providerServices, reader);
      DbSpatialDataReader spatialDataReader = (DbSpatialDataReader) null;
      if (((IEnumerable<Type>) columnTypes).Any<Type>((Func<Type, bool>) (t =>
      {
        if (!(t == typeof (DbGeography)))
          return t == typeof (DbGeometry);
        return true;
      })))
        spatialDataReader = providerServices.GetSpatialDataReader(reader, providerManifestToken);
      return bufferedDataRecord.InitializeAsync(reader, spatialDataReader, columnTypes, nullableColumns, cancellationToken);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private BufferedDataRecord Initialize(
      DbDataReader reader,
      DbSpatialDataReader spatialDataReader,
      Type[] columnTypes,
      bool[] nullableColumns)
    {
      this.InitializeFields(columnTypes, nullableColumns);
      while (reader.Read())
      {
        ++this._currentRowNumber;
        if (this._rowCapacity == this._currentRowNumber)
          this.DoubleBufferCapacity();
        int num = Math.Max(columnTypes.Length, nullableColumns.Length);
        for (int ordinal = 0; ordinal < num; ++ordinal)
        {
          if (ordinal < this._columnTypeCases.Length)
          {
            switch (this._columnTypeCases[ordinal])
            {
              case ShapedBufferedDataRecord.TypeCase.Empty:
                if (nullableColumns[ordinal])
                {
                  this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal);
                  continue;
                }
                continue;
              case ShapedBufferedDataRecord.TypeCase.Bool:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadBool(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadBool(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.Byte:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadByte(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadByte(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.Char:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadChar(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadChar(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.DateTime:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadDateTime(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadDateTime(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.Decimal:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadDecimal(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadDecimal(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.Double:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadDouble(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadDouble(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.Float:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadFloat(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadFloat(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.Guid:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadGuid(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadGuid(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.Short:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadShort(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadShort(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.Int:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadInt(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadInt(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.Long:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadLong(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadLong(reader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.DbGeography:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadGeography(spatialDataReader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadGeography(spatialDataReader, ordinal);
                continue;
              case ShapedBufferedDataRecord.TypeCase.DbGeometry:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadGeometry(spatialDataReader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadGeometry(spatialDataReader, ordinal);
                continue;
              default:
                if (nullableColumns[ordinal])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal)))
                  {
                    this.ReadObject(reader, ordinal);
                    continue;
                  }
                  continue;
                }
                this.ReadObject(reader, ordinal);
                continue;
            }
          }
          else if (nullableColumns[ordinal])
            this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]] = reader.IsDBNull(ordinal);
        }
      }
      this._bools = new BitArray(this._tempBools);
      this._tempBools = (bool[]) null;
      this._nulls = new BitArray(this._tempNulls);
      this._tempNulls = (bool[]) null;
      this._rowCount = this._currentRowNumber + 1;
      this._currentRowNumber = -1;
      return (BufferedDataRecord) this;
    }

    private async Task<BufferedDataRecord> InitializeAsync(
      DbDataReader reader,
      DbSpatialDataReader spatialDataReader,
      Type[] columnTypes,
      bool[] nullableColumns,
      CancellationToken cancellationToken)
    {
      this.InitializeFields(columnTypes, nullableColumns);
label_104:
      System.Data.Entity.Utilities.TaskExtensions.CultureAwaiter<bool> cultureAwaiter1 = reader.ReadAsync(cancellationToken).WithCurrentCulture<bool>();
      if (await cultureAwaiter1)
      {
        cancellationToken.ThrowIfCancellationRequested();
        ++this._currentRowNumber;
        if (this._rowCapacity == this._currentRowNumber)
          this.DoubleBufferCapacity();
        int columnCount = columnTypes.Length > nullableColumns.Length ? columnTypes.Length : nullableColumns.Length;
        System.Data.Entity.Utilities.TaskExtensions.CultureAwaiter cultureAwaiter2;
        for (int i = 0; i < columnCount; ++i)
        {
          if (i < this._columnTypeCases.Length)
          {
            switch (this._columnTypeCases[i])
            {
              case ShapedBufferedDataRecord.TypeCase.Empty:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num = await cultureAwaiter1 ? 1 : 0;
                  tempNulls[index] = num != 0;
                  continue;
                }
                continue;
              case ShapedBufferedDataRecord.TypeCase.Bool:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int num1 = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int index = num1;
                  bool[] flagArray = tempNulls;
                  int num2;
                  bool flag = (num2 = await cultureAwaiter1 ? 1 : 0) != 0;
                  flagArray[index] = num2 != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadBoolAsync(reader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                await this.ReadBoolAsync(reader, i, cancellationToken).WithCurrentCulture();
                continue;
              case ShapedBufferedDataRecord.TypeCase.Byte:
                if (nullableColumns[i])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i]] = (await reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>() ? 1 : 0) != 0))
                  {
                    await this.ReadByteAsync(reader, i, cancellationToken).WithCurrentCulture();
                    continue;
                  }
                  continue;
                }
                await this.ReadByteAsync(reader, i, cancellationToken).WithCurrentCulture();
                continue;
              case ShapedBufferedDataRecord.TypeCase.Char:
                if (nullableColumns[i])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i]] = (await reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>() ? 1 : 0) != 0))
                  {
                    await this.ReadCharAsync(reader, i, cancellationToken).WithCurrentCulture();
                    continue;
                  }
                  continue;
                }
                await this.ReadCharAsync(reader, i, cancellationToken).WithCurrentCulture();
                continue;
              case ShapedBufferedDataRecord.TypeCase.DateTime:
                if (nullableColumns[i])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i]] = (await reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>() ? 1 : 0) != 0))
                  {
                    await this.ReadDateTimeAsync(reader, i, cancellationToken).WithCurrentCulture();
                    continue;
                  }
                  continue;
                }
                await this.ReadDateTimeAsync(reader, i, cancellationToken).WithCurrentCulture();
                continue;
              case ShapedBufferedDataRecord.TypeCase.Decimal:
                if (nullableColumns[i])
                {
                  if (!(this._tempNulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i]] = (await reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>() ? 1 : 0) != 0))
                  {
                    await this.ReadDecimalAsync(reader, i, cancellationToken).WithCurrentCulture();
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadDecimalAsync(reader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
              case ShapedBufferedDataRecord.TypeCase.Double:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num;
                  bool flag = (num = await cultureAwaiter1 ? 1 : 0) != 0;
                  tempNulls[index] = num != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadDoubleAsync(reader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadDoubleAsync(reader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
              case ShapedBufferedDataRecord.TypeCase.Float:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num;
                  bool flag = (num = await cultureAwaiter1 ? 1 : 0) != 0;
                  tempNulls[index] = num != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadFloatAsync(reader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadFloatAsync(reader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
              case ShapedBufferedDataRecord.TypeCase.Guid:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num;
                  bool flag = (num = await cultureAwaiter1 ? 1 : 0) != 0;
                  tempNulls[index] = num != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadGuidAsync(reader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadGuidAsync(reader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
              case ShapedBufferedDataRecord.TypeCase.Short:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num;
                  bool flag = (num = await cultureAwaiter1 ? 1 : 0) != 0;
                  tempNulls[index] = num != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadShortAsync(reader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadShortAsync(reader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
              case ShapedBufferedDataRecord.TypeCase.Int:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num;
                  bool flag = (num = await cultureAwaiter1 ? 1 : 0) != 0;
                  tempNulls[index] = num != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadIntAsync(reader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadIntAsync(reader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
              case ShapedBufferedDataRecord.TypeCase.Long:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num;
                  bool flag = (num = await cultureAwaiter1 ? 1 : 0) != 0;
                  tempNulls[index] = num != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadLongAsync(reader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadLongAsync(reader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
              case ShapedBufferedDataRecord.TypeCase.DbGeography:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num;
                  bool flag = (num = await cultureAwaiter1 ? 1 : 0) != 0;
                  tempNulls[index] = num != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadGeographyAsync(spatialDataReader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadGeographyAsync(spatialDataReader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
              case ShapedBufferedDataRecord.TypeCase.DbGeometry:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num;
                  bool flag = (num = await cultureAwaiter1 ? 1 : 0) != 0;
                  tempNulls[index] = num != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadGeometryAsync(spatialDataReader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadGeometryAsync(spatialDataReader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
              default:
                if (nullableColumns[i])
                {
                  bool[] tempNulls = this._tempNulls;
                  int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
                  cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
                  int num;
                  bool flag = (num = await cultureAwaiter1 ? 1 : 0) != 0;
                  tempNulls[index] = num != 0;
                  if (!flag)
                  {
                    cultureAwaiter2 = this.ReadObjectAsync(reader, i, cancellationToken).WithCurrentCulture();
                    await cultureAwaiter2;
                    continue;
                  }
                  continue;
                }
                cultureAwaiter2 = this.ReadObjectAsync(reader, i, cancellationToken).WithCurrentCulture();
                await cultureAwaiter2;
                continue;
            }
          }
          else if (nullableColumns[i])
          {
            bool[] tempNulls = this._tempNulls;
            int index = this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[i];
            cultureAwaiter1 = reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>();
            int num = await cultureAwaiter1 ? 1 : 0;
            tempNulls[index] = num != 0;
          }
        }
        goto label_104;
      }
      else
      {
        this._bools = new BitArray(this._tempBools);
        this._tempBools = (bool[]) null;
        this._nulls = new BitArray(this._tempNulls);
        this._tempNulls = (bool[]) null;
        this._rowCount = this._currentRowNumber + 1;
        this._currentRowNumber = -1;
        return (BufferedDataRecord) this;
      }
    }

    private void InitializeFields(Type[] columnTypes, bool[] nullableColumns)
    {
      this._columnTypeCases = Enumerable.Repeat<ShapedBufferedDataRecord.TypeCase>(ShapedBufferedDataRecord.TypeCase.Empty, columnTypes.Length).ToArray<ShapedBufferedDataRecord.TypeCase>();
      int count = Math.Max(this.FieldCount, Math.Max(columnTypes.Length, nullableColumns.Length));
      this._ordinalToIndexMap = Enumerable.Repeat<int>(-1, count).ToArray<int>();
      for (int index = 0; index < columnTypes.Length; ++index)
      {
        Type columnType = columnTypes[index];
        if (!(columnType == (Type) null))
        {
          if (columnType == typeof (bool))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Bool;
            this._ordinalToIndexMap[index] = this._boolCount;
            ++this._boolCount;
          }
          else if (columnType == typeof (byte))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Byte;
            this._ordinalToIndexMap[index] = this._byteCount;
            ++this._byteCount;
          }
          else if (columnType == typeof (char))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Char;
            this._ordinalToIndexMap[index] = this._charCount;
            ++this._charCount;
          }
          else if (columnType == typeof (DateTime))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.DateTime;
            this._ordinalToIndexMap[index] = this._dateTimeCount;
            ++this._dateTimeCount;
          }
          else if (columnType == typeof (Decimal))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Decimal;
            this._ordinalToIndexMap[index] = this._decimalCount;
            ++this._decimalCount;
          }
          else if (columnType == typeof (double))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Double;
            this._ordinalToIndexMap[index] = this._doubleCount;
            ++this._doubleCount;
          }
          else if (columnType == typeof (float))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Float;
            this._ordinalToIndexMap[index] = this._floatCount;
            ++this._floatCount;
          }
          else if (columnType == typeof (Guid))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Guid;
            this._ordinalToIndexMap[index] = this._guidCount;
            ++this._guidCount;
          }
          else if (columnType == typeof (short))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Short;
            this._ordinalToIndexMap[index] = this._shortCount;
            ++this._shortCount;
          }
          else if (columnType == typeof (int))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Int;
            this._ordinalToIndexMap[index] = this._intCount;
            ++this._intCount;
          }
          else if (columnType == typeof (long))
          {
            this._columnTypeCases[index] = ShapedBufferedDataRecord.TypeCase.Long;
            this._ordinalToIndexMap[index] = this._longCount;
            ++this._longCount;
          }
          else
          {
            this._columnTypeCases[index] = !(columnType == typeof (DbGeography)) ? (!(columnType == typeof (DbGeometry)) ? ShapedBufferedDataRecord.TypeCase.Object : ShapedBufferedDataRecord.TypeCase.DbGeometry) : ShapedBufferedDataRecord.TypeCase.DbGeography;
            this._ordinalToIndexMap[index] = this._objectCount;
            ++this._objectCount;
          }
        }
      }
      this._tempBools = new bool[this._rowCapacity * this._boolCount];
      this._bytes = new byte[this._rowCapacity * this._byteCount];
      this._chars = new char[this._rowCapacity * this._charCount];
      this._dateTimes = new DateTime[this._rowCapacity * this._dateTimeCount];
      this._decimals = new Decimal[this._rowCapacity * this._decimalCount];
      this._doubles = new double[this._rowCapacity * this._doubleCount];
      this._floats = new float[this._rowCapacity * this._floatCount];
      this._guids = new Guid[this._rowCapacity * this._guidCount];
      this._shorts = new short[this._rowCapacity * this._shortCount];
      this._ints = new int[this._rowCapacity * this._intCount];
      this._longs = new long[this._rowCapacity * this._longCount];
      this._objects = new object[this._rowCapacity * this._objectCount];
      this._nullOrdinalToIndexMap = Enumerable.Repeat<int>(-1, count).ToArray<int>();
      for (int index = 0; index < nullableColumns.Length; ++index)
      {
        if (nullableColumns[index])
        {
          this._nullOrdinalToIndexMap[index] = this._nullCount;
          ++this._nullCount;
        }
      }
      this._tempNulls = new bool[this._rowCapacity * this._nullCount];
    }

    private void DoubleBufferCapacity()
    {
      this._rowCapacity <<= 1;
      bool[] flagArray1 = new bool[this._tempBools.Length << 1];
      Array.Copy((Array) this._tempBools, (Array) flagArray1, this._tempBools.Length);
      this._tempBools = flagArray1;
      byte[] numArray1 = new byte[this._bytes.Length << 1];
      Array.Copy((Array) this._bytes, (Array) numArray1, this._bytes.Length);
      this._bytes = numArray1;
      char[] chArray = new char[this._chars.Length << 1];
      Array.Copy((Array) this._chars, (Array) chArray, this._chars.Length);
      this._chars = chArray;
      DateTime[] dateTimeArray = new DateTime[this._dateTimes.Length << 1];
      Array.Copy((Array) this._dateTimes, (Array) dateTimeArray, this._dateTimes.Length);
      this._dateTimes = dateTimeArray;
      Decimal[] numArray2 = new Decimal[this._decimals.Length << 1];
      Array.Copy((Array) this._decimals, (Array) numArray2, this._decimals.Length);
      this._decimals = numArray2;
      double[] numArray3 = new double[this._doubles.Length << 1];
      Array.Copy((Array) this._doubles, (Array) numArray3, this._doubles.Length);
      this._doubles = numArray3;
      float[] numArray4 = new float[this._floats.Length << 1];
      Array.Copy((Array) this._floats, (Array) numArray4, this._floats.Length);
      this._floats = numArray4;
      Guid[] guidArray = new Guid[this._guids.Length << 1];
      Array.Copy((Array) this._guids, (Array) guidArray, this._guids.Length);
      this._guids = guidArray;
      short[] numArray5 = new short[this._shorts.Length << 1];
      Array.Copy((Array) this._shorts, (Array) numArray5, this._shorts.Length);
      this._shorts = numArray5;
      int[] numArray6 = new int[this._ints.Length << 1];
      Array.Copy((Array) this._ints, (Array) numArray6, this._ints.Length);
      this._ints = numArray6;
      long[] numArray7 = new long[this._longs.Length << 1];
      Array.Copy((Array) this._longs, (Array) numArray7, this._longs.Length);
      this._longs = numArray7;
      object[] objArray = new object[this._objects.Length << 1];
      Array.Copy((Array) this._objects, (Array) objArray, this._objects.Length);
      this._objects = objArray;
      bool[] flagArray2 = new bool[this._tempNulls.Length << 1];
      Array.Copy((Array) this._tempNulls, (Array) flagArray2, this._tempNulls.Length);
      this._tempNulls = flagArray2;
    }

    private void ReadBool(DbDataReader reader, int ordinal)
    {
      this._tempBools[this._currentRowNumber * this._boolCount + this._ordinalToIndexMap[ordinal]] = reader.GetBoolean(ordinal);
    }

    private async Task ReadBoolAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      bool[] tempBools = this._tempBools;
      int index = this._currentRowNumber * this._boolCount + this._ordinalToIndexMap[ordinal];
      bool[] flagArray = tempBools;
      int num = await reader.GetFieldValueAsync<bool>(ordinal, cancellationToken).WithCurrentCulture<bool>() ? 1 : 0;
      flagArray[index] = num != 0;
    }

    private void ReadByte(DbDataReader reader, int ordinal)
    {
      this._bytes[this._currentRowNumber * this._byteCount + this._ordinalToIndexMap[ordinal]] = reader.GetByte(ordinal);
    }

    private async Task ReadByteAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      byte[] bytes = this._bytes;
      int index = this._currentRowNumber * this._byteCount + this._ordinalToIndexMap[ordinal];
      byte[] numArray = bytes;
      int num = (int) await reader.GetFieldValueAsync<byte>(ordinal, cancellationToken).WithCurrentCulture<byte>();
      numArray[index] = (byte) num;
    }

    private void ReadChar(DbDataReader reader, int ordinal)
    {
      this._chars[this._currentRowNumber * this._charCount + this._ordinalToIndexMap[ordinal]] = reader.GetChar(ordinal);
    }

    private async Task ReadCharAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      char[] chars = this._chars;
      int index = this._currentRowNumber * this._charCount + this._ordinalToIndexMap[ordinal];
      char[] chArray = chars;
      int num = (int) await reader.GetFieldValueAsync<char>(ordinal, cancellationToken).WithCurrentCulture<char>();
      chArray[index] = (char) num;
    }

    private void ReadDateTime(DbDataReader reader, int ordinal)
    {
      this._dateTimes[this._currentRowNumber * this._dateTimeCount + this._ordinalToIndexMap[ordinal]] = reader.GetDateTime(ordinal);
    }

    private async Task ReadDateTimeAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      DateTime[] dateTimes = this._dateTimes;
      int index1 = this._currentRowNumber * this._dateTimeCount + this._ordinalToIndexMap[ordinal];
      ref DateTime local = ref dateTimes[index1];
      int index2 = index1;
      DateTime[] dateTimeArray = dateTimes;
      DateTime dateTime = await reader.GetFieldValueAsync<DateTime>(ordinal, cancellationToken).WithCurrentCulture<DateTime>();
      dateTimeArray[index2] = dateTime;
    }

    private void ReadDecimal(DbDataReader reader, int ordinal)
    {
      this._decimals[this._currentRowNumber * this._decimalCount + this._ordinalToIndexMap[ordinal]] = reader.GetDecimal(ordinal);
    }

    private async Task ReadDecimalAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      Decimal[] decimals = this._decimals;
      int index1 = this._currentRowNumber * this._decimalCount + this._ordinalToIndexMap[ordinal];
      ref Decimal local = ref decimals[index1];
      int index2 = index1;
      Decimal[] numArray = decimals;
      Decimal num = await reader.GetFieldValueAsync<Decimal>(ordinal, cancellationToken).WithCurrentCulture<Decimal>();
      numArray[index2] = num;
    }

    private void ReadDouble(DbDataReader reader, int ordinal)
    {
      this._doubles[this._currentRowNumber * this._doubleCount + this._ordinalToIndexMap[ordinal]] = reader.GetDouble(ordinal);
    }

    private async Task ReadDoubleAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      double[] doubles = this._doubles;
      int index = this._currentRowNumber * this._doubleCount + this._ordinalToIndexMap[ordinal];
      double[] numArray = doubles;
      double num = await reader.GetFieldValueAsync<double>(ordinal, cancellationToken).WithCurrentCulture<double>();
      numArray[index] = num;
    }

    private void ReadFloat(DbDataReader reader, int ordinal)
    {
      this._floats[this._currentRowNumber * this._floatCount + this._ordinalToIndexMap[ordinal]] = reader.GetFloat(ordinal);
    }

    private async Task ReadFloatAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      float[] floats = this._floats;
      int index = this._currentRowNumber * this._floatCount + this._ordinalToIndexMap[ordinal];
      float[] numArray = floats;
      double num = (double) await reader.GetFieldValueAsync<float>(ordinal, cancellationToken).WithCurrentCulture<float>();
      numArray[index] = (float) num;
    }

    private void ReadGuid(DbDataReader reader, int ordinal)
    {
      this._guids[this._currentRowNumber * this._guidCount + this._ordinalToIndexMap[ordinal]] = reader.GetGuid(ordinal);
    }

    private async Task ReadGuidAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      Guid[] guids = this._guids;
      int index1 = this._currentRowNumber * this._guidCount + this._ordinalToIndexMap[ordinal];
      ref Guid local = ref guids[index1];
      int index2 = index1;
      Guid[] guidArray = guids;
      Guid guid = await reader.GetFieldValueAsync<Guid>(ordinal, cancellationToken).WithCurrentCulture<Guid>();
      guidArray[index2] = guid;
    }

    private void ReadShort(DbDataReader reader, int ordinal)
    {
      this._shorts[this._currentRowNumber * this._shortCount + this._ordinalToIndexMap[ordinal]] = reader.GetInt16(ordinal);
    }

    private async Task ReadShortAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      short[] shorts = this._shorts;
      int index = this._currentRowNumber * this._shortCount + this._ordinalToIndexMap[ordinal];
      short[] numArray = shorts;
      int num = (int) await reader.GetFieldValueAsync<short>(ordinal, cancellationToken).WithCurrentCulture<short>();
      numArray[index] = (short) num;
    }

    private void ReadInt(DbDataReader reader, int ordinal)
    {
      this._ints[this._currentRowNumber * this._intCount + this._ordinalToIndexMap[ordinal]] = reader.GetInt32(ordinal);
    }

    private async Task ReadIntAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      int[] ints = this._ints;
      int index = this._currentRowNumber * this._intCount + this._ordinalToIndexMap[ordinal];
      int[] numArray = ints;
      int num = await reader.GetFieldValueAsync<int>(ordinal, cancellationToken).WithCurrentCulture<int>();
      numArray[index] = num;
    }

    private void ReadLong(DbDataReader reader, int ordinal)
    {
      this._longs[this._currentRowNumber * this._longCount + this._ordinalToIndexMap[ordinal]] = reader.GetInt64(ordinal);
    }

    private async Task ReadLongAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      long[] longs = this._longs;
      int index = this._currentRowNumber * this._longCount + this._ordinalToIndexMap[ordinal];
      long[] numArray = longs;
      long num = await reader.GetFieldValueAsync<long>(ordinal, cancellationToken).WithCurrentCulture<long>();
      numArray[index] = num;
    }

    private void ReadObject(DbDataReader reader, int ordinal)
    {
      this._objects[this._currentRowNumber * this._objectCount + this._ordinalToIndexMap[ordinal]] = reader.GetValue(ordinal);
    }

    private async Task ReadObjectAsync(
      DbDataReader reader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      object[] objects = this._objects;
      int index = this._currentRowNumber * this._objectCount + this._ordinalToIndexMap[ordinal];
      object[] objArray = objects;
      object obj = await reader.GetFieldValueAsync<object>(ordinal, cancellationToken).WithCurrentCulture<object>();
      objArray[index] = obj;
    }

    private void ReadGeography(DbSpatialDataReader spatialReader, int ordinal)
    {
      this._objects[this._currentRowNumber * this._objectCount + this._ordinalToIndexMap[ordinal]] = (object) spatialReader.GetGeography(ordinal);
    }

    private async Task ReadGeographyAsync(
      DbSpatialDataReader spatialReader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      object[] objects = this._objects;
      int index = this._currentRowNumber * this._objectCount + this._ordinalToIndexMap[ordinal];
      object[] objArray = objects;
      DbGeography dbGeography = await spatialReader.GetGeographyAsync(ordinal, cancellationToken).WithCurrentCulture<DbGeography>();
      objArray[index] = (object) dbGeography;
    }

    private void ReadGeometry(DbSpatialDataReader spatialReader, int ordinal)
    {
      this._objects[this._currentRowNumber * this._objectCount + this._ordinalToIndexMap[ordinal]] = (object) spatialReader.GetGeometry(ordinal);
    }

    private async Task ReadGeometryAsync(
      DbSpatialDataReader spatialReader,
      int ordinal,
      CancellationToken cancellationToken)
    {
      object[] objects = this._objects;
      int index = this._currentRowNumber * this._objectCount + this._ordinalToIndexMap[ordinal];
      object[] objArray = objects;
      DbGeometry dbGeometry = await spatialReader.GetGeometryAsync(ordinal, cancellationToken).WithCurrentCulture<DbGeometry>();
      objArray[index] = (object) dbGeometry;
    }

    public override bool GetBoolean(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Bool)
        return this._bools[this._currentRowNumber * this._boolCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<bool>(ordinal);
    }

    public override byte GetByte(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Byte)
        return this._bytes[this._currentRowNumber * this._byteCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<byte>(ordinal);
    }

    public override char GetChar(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Char)
        return this._chars[this._currentRowNumber * this._charCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<char>(ordinal);
    }

    public override DateTime GetDateTime(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.DateTime)
        return this._dateTimes[this._currentRowNumber * this._dateTimeCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<DateTime>(ordinal);
    }

    public override Decimal GetDecimal(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Decimal)
        return this._decimals[this._currentRowNumber * this._decimalCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<Decimal>(ordinal);
    }

    public override double GetDouble(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Double)
        return this._doubles[this._currentRowNumber * this._doubleCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<double>(ordinal);
    }

    public override float GetFloat(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Float)
        return this._floats[this._currentRowNumber * this._floatCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<float>(ordinal);
    }

    public override Guid GetGuid(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Guid)
        return this._guids[this._currentRowNumber * this._guidCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<Guid>(ordinal);
    }

    public override short GetInt16(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Short)
        return this._shorts[this._currentRowNumber * this._shortCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<short>(ordinal);
    }

    public override int GetInt32(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Int)
        return this._ints[this._currentRowNumber * this._intCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<int>(ordinal);
    }

    public override long GetInt64(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Long)
        return this._longs[this._currentRowNumber * this._longCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<long>(ordinal);
    }

    public override string GetString(int ordinal)
    {
      if (this._columnTypeCases[ordinal] == ShapedBufferedDataRecord.TypeCase.Object)
        return (string) this._objects[this._currentRowNumber * this._objectCount + this._ordinalToIndexMap[ordinal]];
      return this.GetFieldValue<string>(ordinal);
    }

    public override object GetValue(int ordinal)
    {
      return this.GetFieldValue<object>(ordinal);
    }

    public override int GetValues(object[] values)
    {
      throw new NotSupportedException();
    }

    public override T GetFieldValue<T>(int ordinal)
    {
      switch (this._columnTypeCases[ordinal])
      {
        case ShapedBufferedDataRecord.TypeCase.Empty:
          return default (T);
        case ShapedBufferedDataRecord.TypeCase.Bool:
          return (T) (System.ValueType) this.GetBoolean(ordinal);
        case ShapedBufferedDataRecord.TypeCase.Byte:
          return (T) (System.ValueType) this.GetByte(ordinal);
        case ShapedBufferedDataRecord.TypeCase.Char:
          return (T) (System.ValueType) this.GetChar(ordinal);
        case ShapedBufferedDataRecord.TypeCase.DateTime:
          return (T) (System.ValueType) this.GetDateTime(ordinal);
        case ShapedBufferedDataRecord.TypeCase.Decimal:
          return (T) (System.ValueType) this.GetDecimal(ordinal);
        case ShapedBufferedDataRecord.TypeCase.Double:
          return (T) (System.ValueType) this.GetDouble(ordinal);
        case ShapedBufferedDataRecord.TypeCase.Float:
          return (T) (System.ValueType) this.GetFloat(ordinal);
        case ShapedBufferedDataRecord.TypeCase.Guid:
          return (T) (System.ValueType) this.GetGuid(ordinal);
        case ShapedBufferedDataRecord.TypeCase.Short:
          return (T) (System.ValueType) this.GetInt16(ordinal);
        case ShapedBufferedDataRecord.TypeCase.Int:
          return (T) (System.ValueType) this.GetInt32(ordinal);
        case ShapedBufferedDataRecord.TypeCase.Long:
          return (T) (System.ValueType) this.GetInt64(ordinal);
        default:
          return (T) this._objects[this._currentRowNumber * this._objectCount + this._ordinalToIndexMap[ordinal]];
      }
    }

    public override Task<T> GetFieldValueAsync<T>(
      int ordinal,
      CancellationToken cancellationToken)
    {
      return Task.FromResult<T>(this.GetFieldValue<T>(ordinal));
    }

    public override bool IsDBNull(int ordinal)
    {
      return this._nulls[this._currentRowNumber * this._nullCount + this._nullOrdinalToIndexMap[ordinal]];
    }

    public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
    {
      return Task.FromResult<bool>(this.IsDBNull(ordinal));
    }

    public override bool Read()
    {
      return this.IsDataReady = ++this._currentRowNumber < this._rowCount;
    }

    public override Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
      return Task.FromResult<bool>(this.Read());
    }

    private enum TypeCase
    {
      Empty,
      Object,
      Bool,
      Byte,
      Char,
      DateTime,
      Decimal,
      Double,
      Float,
      Guid,
      Short,
      Int,
      Long,
      DbGeography,
      DbGeometry,
    }
  }
}
