// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.BufferedDataReader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class BufferedDataReader : DbDataReader
  {
    private List<BufferedDataRecord> _bufferedDataRecords = new List<BufferedDataRecord>();
    private DbDataReader _underlyingReader;
    private BufferedDataRecord _currentResultSet;
    private int _currentResultSetNumber;
    private int _recordsAffected;
    private bool _disposed;
    private bool _isClosed;

    public BufferedDataReader(DbDataReader reader)
    {
      this._underlyingReader = reader;
    }

    public override int RecordsAffected
    {
      get
      {
        return this._recordsAffected;
      }
    }

    public override object this[string name]
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public override object this[int ordinal]
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public override int Depth
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public override int FieldCount
    {
      get
      {
        this.AssertReaderIsOpen();
        return this._currentResultSet.FieldCount;
      }
    }

    public override bool HasRows
    {
      get
      {
        this.AssertReaderIsOpen();
        return this._currentResultSet.HasRows;
      }
    }

    public override bool IsClosed
    {
      get
      {
        return this._isClosed;
      }
    }

    private void AssertReaderIsOpen()
    {
      if (this._isClosed)
        throw Error.ADP_ClosedDataReaderError();
    }

    private void AssertReaderIsOpenWithData()
    {
      if (this._isClosed)
        throw Error.ADP_ClosedDataReaderError();
      if (!this._currentResultSet.IsDataReady)
        throw Error.ADP_NoData();
    }

    [Conditional("DEBUG")]
    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    private void AssertFieldIsReady(int ordinal)
    {
      if (this._isClosed)
        throw Error.ADP_ClosedDataReaderError();
      if (!this._currentResultSet.IsDataReady)
        throw Error.ADP_NoData();
      if (0 > ordinal || ordinal > this._currentResultSet.FieldCount)
        throw new IndexOutOfRangeException();
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "columnTypes")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "nullableColumns")]
    internal void Initialize(
      string providerManifestToken,
      DbProviderServices providerServices,
      Type[] columnTypes,
      bool[] nullableColumns)
    {
      DbDataReader underlyingReader = this._underlyingReader;
      if (underlyingReader == null)
        return;
      this._underlyingReader = (DbDataReader) null;
      try
      {
        if (columnTypes != null && underlyingReader.GetType().Name != "SqlDataReader")
          this._bufferedDataRecords.Add(ShapedBufferedDataRecord.Initialize(providerManifestToken, providerServices, underlyingReader, columnTypes, nullableColumns));
        else
          this._bufferedDataRecords.Add((BufferedDataRecord) ShapelessBufferedDataRecord.Initialize(providerManifestToken, providerServices, underlyingReader));
        while (underlyingReader.NextResult())
          this._bufferedDataRecords.Add((BufferedDataRecord) ShapelessBufferedDataRecord.Initialize(providerManifestToken, providerServices, underlyingReader));
        this._recordsAffected = underlyingReader.RecordsAffected;
        this._currentResultSet = this._bufferedDataRecords[this._currentResultSetNumber];
      }
      finally
      {
        underlyingReader.Dispose();
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "columnTypes")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "nullableColumns")]
    internal async Task InitializeAsync(
      string providerManifestToken,
      DbProviderServices providerSerivces,
      Type[] columnTypes,
      bool[] nullableColumns,
      CancellationToken cancellationToken)
    {
      if (this._underlyingReader == null)
        return;
      cancellationToken.ThrowIfCancellationRequested();
      DbDataReader reader = this._underlyingReader;
      this._underlyingReader = (DbDataReader) null;
      try
      {
        if (columnTypes != null && reader.GetType().Name != "SqlDataReader")
          this._bufferedDataRecords.Add(await ShapedBufferedDataRecord.InitializeAsync(providerManifestToken, providerSerivces, reader, columnTypes, nullableColumns, cancellationToken).WithCurrentCulture<BufferedDataRecord>());
        else
          this._bufferedDataRecords.Add((BufferedDataRecord) await ShapelessBufferedDataRecord.InitializeAsync(providerManifestToken, providerSerivces, reader, cancellationToken).WithCurrentCulture<ShapelessBufferedDataRecord>());
        while (true)
        {
          if (await reader.NextResultAsync(cancellationToken).WithCurrentCulture<bool>())
            this._bufferedDataRecords.Add((BufferedDataRecord) await ShapelessBufferedDataRecord.InitializeAsync(providerManifestToken, providerSerivces, reader, cancellationToken).WithCurrentCulture<ShapelessBufferedDataRecord>());
          else
            break;
        }
        this._recordsAffected = reader.RecordsAffected;
        this._currentResultSet = this._bufferedDataRecords[this._currentResultSetNumber];
      }
      finally
      {
        reader.Dispose();
      }
    }

    public override void Close()
    {
      this._bufferedDataRecords = (List<BufferedDataRecord>) null;
      this._isClosed = true;
      DbDataReader underlyingReader = this._underlyingReader;
      if (underlyingReader == null)
        return;
      this._underlyingReader = (DbDataReader) null;
      underlyingReader.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
      if (!this._disposed && disposing && !this.IsClosed)
        this.Close();
      this._disposed = true;
      base.Dispose(disposing);
    }

    public override bool GetBoolean(int ordinal)
    {
      return this._currentResultSet.GetBoolean(ordinal);
    }

    public override byte GetByte(int ordinal)
    {
      return this._currentResultSet.GetByte(ordinal);
    }

    public override long GetBytes(
      int ordinal,
      long dataOffset,
      byte[] buffer,
      int bufferOffset,
      int length)
    {
      throw new NotSupportedException();
    }

    public override char GetChar(int ordinal)
    {
      return this._currentResultSet.GetChar(ordinal);
    }

    public override long GetChars(
      int ordinal,
      long dataOffset,
      char[] buffer,
      int bufferOffset,
      int length)
    {
      throw new NotSupportedException();
    }

    public override DateTime GetDateTime(int ordinal)
    {
      return this._currentResultSet.GetDateTime(ordinal);
    }

    public override Decimal GetDecimal(int ordinal)
    {
      return this._currentResultSet.GetDecimal(ordinal);
    }

    public override double GetDouble(int ordinal)
    {
      return this._currentResultSet.GetDouble(ordinal);
    }

    public override float GetFloat(int ordinal)
    {
      return this._currentResultSet.GetFloat(ordinal);
    }

    public override Guid GetGuid(int ordinal)
    {
      return this._currentResultSet.GetGuid(ordinal);
    }

    public override short GetInt16(int ordinal)
    {
      return this._currentResultSet.GetInt16(ordinal);
    }

    public override int GetInt32(int ordinal)
    {
      return this._currentResultSet.GetInt32(ordinal);
    }

    public override long GetInt64(int ordinal)
    {
      return this._currentResultSet.GetInt64(ordinal);
    }

    public override string GetString(int ordinal)
    {
      return this._currentResultSet.GetString(ordinal);
    }

    public override T GetFieldValue<T>(int ordinal)
    {
      return this._currentResultSet.GetFieldValue<T>(ordinal);
    }

    public override Task<T> GetFieldValueAsync<T>(
      int ordinal,
      CancellationToken cancellationToken)
    {
      return this._currentResultSet.GetFieldValueAsync<T>(ordinal, cancellationToken);
    }

    public override object GetValue(int ordinal)
    {
      return this._currentResultSet.GetValue(ordinal);
    }

    public override int GetValues(object[] values)
    {
      Check.NotNull<object[]>(values, nameof (values));
      this.AssertReaderIsOpenWithData();
      return this._currentResultSet.GetValues(values);
    }

    public override string GetDataTypeName(int ordinal)
    {
      this.AssertReaderIsOpen();
      return this._currentResultSet.GetDataTypeName(ordinal);
    }

    public override Type GetFieldType(int ordinal)
    {
      this.AssertReaderIsOpen();
      return this._currentResultSet.GetFieldType(ordinal);
    }

    public override string GetName(int ordinal)
    {
      this.AssertReaderIsOpen();
      return this._currentResultSet.GetName(ordinal);
    }

    public override int GetOrdinal(string name)
    {
      Check.NotNull<string>(name, nameof (name));
      this.AssertReaderIsOpen();
      return this._currentResultSet.GetOrdinal(name);
    }

    public override bool IsDBNull(int ordinal)
    {
      return this._currentResultSet.IsDBNull(ordinal);
    }

    public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
    {
      return this._currentResultSet.IsDBNullAsync(ordinal, cancellationToken);
    }

    public override IEnumerator GetEnumerator()
    {
      return (IEnumerator) new DbEnumerator((IDataReader) this);
    }

    public override DataTable GetSchemaTable()
    {
      throw new NotSupportedException();
    }

    public override bool NextResult()
    {
      this.AssertReaderIsOpen();
      if (++this._currentResultSetNumber < this._bufferedDataRecords.Count)
      {
        this._currentResultSet = this._bufferedDataRecords[this._currentResultSetNumber];
        return true;
      }
      this._currentResultSet = (BufferedDataRecord) null;
      return false;
    }

    public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return Task.FromResult<bool>(this.NextResult());
    }

    public override bool Read()
    {
      this.AssertReaderIsOpen();
      return this._currentResultSet.Read();
    }

    public override Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      this.AssertReaderIsOpen();
      return this._currentResultSet.ReadAsync(cancellationToken);
    }
  }
}
