// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.ShapelessBufferedDataRecord
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Spatial;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class ShapelessBufferedDataRecord : BufferedDataRecord
  {
    private object[] _currentRow;
    private List<object[]> _resultSet;
    private DbSpatialDataReader _spatialDataReader;
    private bool[] _geographyColumns;
    private bool[] _geometryColumns;

    protected ShapelessBufferedDataRecord()
    {
    }

    internal static ShapelessBufferedDataRecord Initialize(
      string providerManifestToken,
      DbProviderServices providerSerivces,
      DbDataReader reader)
    {
      ShapelessBufferedDataRecord bufferedDataRecord = new ShapelessBufferedDataRecord();
      bufferedDataRecord.ReadMetadata(providerManifestToken, providerSerivces, reader);
      int fieldCount = bufferedDataRecord.FieldCount;
      List<object[]> objArrayList = new List<object[]>();
      if (bufferedDataRecord._spatialDataReader != null)
      {
        while (reader.Read())
        {
          object[] objArray = new object[fieldCount];
          for (int ordinal = 0; ordinal < fieldCount; ++ordinal)
            objArray[ordinal] = !reader.IsDBNull(ordinal) ? (!bufferedDataRecord._geographyColumns[ordinal] ? (!bufferedDataRecord._geometryColumns[ordinal] ? reader.GetValue(ordinal) : (object) bufferedDataRecord._spatialDataReader.GetGeometry(ordinal)) : (object) bufferedDataRecord._spatialDataReader.GetGeography(ordinal)) : (object) DBNull.Value;
          objArrayList.Add(objArray);
        }
      }
      else
      {
        while (reader.Read())
        {
          object[] values = new object[fieldCount];
          reader.GetValues(values);
          objArrayList.Add(values);
        }
      }
      bufferedDataRecord._rowCount = objArrayList.Count;
      bufferedDataRecord._resultSet = objArrayList;
      return bufferedDataRecord;
    }

    internal static async Task<ShapelessBufferedDataRecord> InitializeAsync(
      string providerManifestToken,
      DbProviderServices providerSerivces,
      DbDataReader reader,
      CancellationToken cancellationToken)
    {
      ShapelessBufferedDataRecord record = new ShapelessBufferedDataRecord();
      record.ReadMetadata(providerManifestToken, providerSerivces, reader);
      int fieldCount = record.FieldCount;
      List<object[]> resultSet = new List<object[]>();
      while (true)
      {
        if (await reader.ReadAsync(cancellationToken).WithCurrentCulture<bool>())
        {
          object[] row = new object[fieldCount];
          for (int i = 0; i < fieldCount; ++i)
          {
            if (await reader.IsDBNullAsync(i, cancellationToken).WithCurrentCulture<bool>())
              row[i] = (object) DBNull.Value;
            else if (record._spatialDataReader != null && record._geographyColumns[i])
              row[i] = (object) await record._spatialDataReader.GetGeographyAsync(i, cancellationToken).WithCurrentCulture<DbGeography>();
            else if (record._spatialDataReader != null && record._geometryColumns[i])
              row[i] = (object) await record._spatialDataReader.GetGeometryAsync(i, cancellationToken).WithCurrentCulture<DbGeometry>();
            else
              row[i] = await reader.GetFieldValueAsync<object>(i, cancellationToken).WithCurrentCulture<object>();
          }
          resultSet.Add(row);
        }
        else
          break;
      }
      record._rowCount = resultSet.Count;
      record._resultSet = resultSet;
      return record;
    }

    protected override void ReadMetadata(
      string providerManifestToken,
      DbProviderServices providerServices,
      DbDataReader reader)
    {
      base.ReadMetadata(providerManifestToken, providerServices, reader);
      int fieldCount = this.FieldCount;
      bool flag = false;
      DbSpatialDataReader spatialDataReader = (DbSpatialDataReader) null;
      if (fieldCount > 0)
        spatialDataReader = providerServices.GetSpatialDataReader(reader, providerManifestToken);
      if (spatialDataReader != null)
      {
        this._geographyColumns = new bool[fieldCount];
        this._geometryColumns = new bool[fieldCount];
        for (int ordinal = 0; ordinal < fieldCount; ++ordinal)
        {
          this._geographyColumns[ordinal] = spatialDataReader.IsGeographyColumn(ordinal);
          this._geometryColumns[ordinal] = spatialDataReader.IsGeometryColumn(ordinal);
          flag = flag || this._geographyColumns[ordinal] || this._geometryColumns[ordinal];
        }
      }
      this._spatialDataReader = flag ? spatialDataReader : (DbSpatialDataReader) null;
    }

    public override bool GetBoolean(int ordinal)
    {
      return this.GetFieldValue<bool>(ordinal);
    }

    public override byte GetByte(int ordinal)
    {
      return this.GetFieldValue<byte>(ordinal);
    }

    public override char GetChar(int ordinal)
    {
      return this.GetFieldValue<char>(ordinal);
    }

    public override DateTime GetDateTime(int ordinal)
    {
      return this.GetFieldValue<DateTime>(ordinal);
    }

    public override Decimal GetDecimal(int ordinal)
    {
      return this.GetFieldValue<Decimal>(ordinal);
    }

    public override double GetDouble(int ordinal)
    {
      return this.GetFieldValue<double>(ordinal);
    }

    public override float GetFloat(int ordinal)
    {
      return this.GetFieldValue<float>(ordinal);
    }

    public override Guid GetGuid(int ordinal)
    {
      return this.GetFieldValue<Guid>(ordinal);
    }

    public override short GetInt16(int ordinal)
    {
      return this.GetFieldValue<short>(ordinal);
    }

    public override int GetInt32(int ordinal)
    {
      return this.GetFieldValue<int>(ordinal);
    }

    public override long GetInt64(int ordinal)
    {
      return this.GetFieldValue<long>(ordinal);
    }

    public override string GetString(int ordinal)
    {
      return this.GetFieldValue<string>(ordinal);
    }

    public override T GetFieldValue<T>(int ordinal)
    {
      return (T) this._currentRow[ordinal];
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "cancellationToken")]
    public override Task<T> GetFieldValueAsync<T>(
      int ordinal,
      CancellationToken cancellationToken)
    {
      return Task.FromResult<T>((T) this._currentRow[ordinal]);
    }

    public override object GetValue(int ordinal)
    {
      return this.GetFieldValue<object>(ordinal);
    }

    public override int GetValues(object[] values)
    {
      int num = Math.Min(values.Length, this.FieldCount);
      for (int ordinal = 0; ordinal < num; ++ordinal)
        values[ordinal] = this.GetValue(ordinal);
      return num;
    }

    public override bool IsDBNull(int ordinal)
    {
      if (this._currentRow.Length == 0)
        return true;
      return DBNull.Value == this._currentRow[ordinal];
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "cancellationToken")]
    public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
    {
      return Task.FromResult<bool>(this.IsDBNull(ordinal));
    }

    public override bool Read()
    {
      if (++this._currentRowNumber < this._rowCount)
      {
        this._currentRow = this._resultSet[this._currentRowNumber];
        this.IsDataReady = true;
      }
      else
      {
        this._currentRow = (object[]) null;
        this.IsDataReady = false;
      }
      return this.IsDataReady;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "cancellationToken")]
    public override Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
      return Task.FromResult<bool>(this.Read());
    }
  }
}
