// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.SqlDataReaderWrapper
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.Data.Entity.SqlServer.Utilities
{
  internal class SqlDataReaderWrapper : MarshalByRefObject
  {
    private readonly SqlDataReader _sqlDataReader;

    protected SqlDataReaderWrapper()
    {
    }

    public SqlDataReaderWrapper(SqlDataReader sqlDataReader)
    {
      this._sqlDataReader = sqlDataReader;
    }

    public virtual IDataReader GetData(int i)
    {
      return ((IDataRecord) this._sqlDataReader).GetData(i);
    }

    public virtual void Dispose()
    {
      this._sqlDataReader.Dispose();
    }

    public virtual Task<T> GetFieldValueAsync<T>(int ordinal)
    {
      return this._sqlDataReader.GetFieldValueAsync<T>(ordinal);
    }

    public virtual Task<bool> IsDBNullAsync(int ordinal)
    {
      return this._sqlDataReader.IsDBNullAsync(ordinal);
    }

    public virtual Task<bool> ReadAsync()
    {
      return this._sqlDataReader.ReadAsync();
    }

    public virtual Task<bool> NextResultAsync()
    {
      return this._sqlDataReader.NextResultAsync();
    }

    public virtual void Close()
    {
      this._sqlDataReader.Close();
    }

    public virtual string GetDataTypeName(int i)
    {
      return this._sqlDataReader.GetDataTypeName(i);
    }

    public virtual IEnumerator GetEnumerator()
    {
      return this._sqlDataReader.GetEnumerator();
    }

    public virtual Type GetFieldType(int i)
    {
      return this._sqlDataReader.GetFieldType(i);
    }

    public virtual string GetName(int i)
    {
      return this._sqlDataReader.GetName(i);
    }

    public virtual Type GetProviderSpecificFieldType(int i)
    {
      return this._sqlDataReader.GetProviderSpecificFieldType(i);
    }

    public virtual int GetOrdinal(string name)
    {
      return this._sqlDataReader.GetOrdinal(name);
    }

    public virtual object GetProviderSpecificValue(int i)
    {
      return this._sqlDataReader.GetProviderSpecificValue(i);
    }

    public virtual int GetProviderSpecificValues(object[] values)
    {
      return this._sqlDataReader.GetProviderSpecificValues(values);
    }

    public virtual DataTable GetSchemaTable()
    {
      return this._sqlDataReader.GetSchemaTable();
    }

    public virtual bool GetBoolean(int i)
    {
      return this._sqlDataReader.GetBoolean(i);
    }

    public virtual XmlReader GetXmlReader(int i)
    {
      return this._sqlDataReader.GetXmlReader(i);
    }

    public virtual Stream GetStream(int i)
    {
      return this._sqlDataReader.GetStream(i);
    }

    public virtual byte GetByte(int i)
    {
      return this._sqlDataReader.GetByte(i);
    }

    public virtual long GetBytes(
      int i,
      long dataIndex,
      byte[] buffer,
      int bufferIndex,
      int length)
    {
      return this._sqlDataReader.GetBytes(i, dataIndex, buffer, bufferIndex, length);
    }

    public virtual TextReader GetTextReader(int i)
    {
      return this._sqlDataReader.GetTextReader(i);
    }

    public virtual char GetChar(int i)
    {
      return this._sqlDataReader.GetChar(i);
    }

    public virtual long GetChars(
      int i,
      long dataIndex,
      char[] buffer,
      int bufferIndex,
      int length)
    {
      return this._sqlDataReader.GetChars(i, dataIndex, buffer, bufferIndex, length);
    }

    public virtual DateTime GetDateTime(int i)
    {
      return this._sqlDataReader.GetDateTime(i);
    }

    public virtual Decimal GetDecimal(int i)
    {
      return this._sqlDataReader.GetDecimal(i);
    }

    public virtual double GetDouble(int i)
    {
      return this._sqlDataReader.GetDouble(i);
    }

    public virtual float GetFloat(int i)
    {
      return this._sqlDataReader.GetFloat(i);
    }

    public virtual Guid GetGuid(int i)
    {
      return this._sqlDataReader.GetGuid(i);
    }

    public virtual short GetInt16(int i)
    {
      return this._sqlDataReader.GetInt16(i);
    }

    public virtual int GetInt32(int i)
    {
      return this._sqlDataReader.GetInt32(i);
    }

    public virtual long GetInt64(int i)
    {
      return this._sqlDataReader.GetInt64(i);
    }

    public virtual SqlBoolean GetSqlBoolean(int i)
    {
      return this._sqlDataReader.GetSqlBoolean(i);
    }

    public virtual SqlBinary GetSqlBinary(int i)
    {
      return this._sqlDataReader.GetSqlBinary(i);
    }

    public virtual SqlByte GetSqlByte(int i)
    {
      return this._sqlDataReader.GetSqlByte(i);
    }

    public virtual SqlBytes GetSqlBytes(int i)
    {
      return this._sqlDataReader.GetSqlBytes(i);
    }

    public virtual SqlChars GetSqlChars(int i)
    {
      return this._sqlDataReader.GetSqlChars(i);
    }

    public virtual SqlDateTime GetSqlDateTime(int i)
    {
      return this._sqlDataReader.GetSqlDateTime(i);
    }

    public virtual SqlDecimal GetSqlDecimal(int i)
    {
      return this._sqlDataReader.GetSqlDecimal(i);
    }

    public virtual SqlGuid GetSqlGuid(int i)
    {
      return this._sqlDataReader.GetSqlGuid(i);
    }

    public virtual SqlDouble GetSqlDouble(int i)
    {
      return this._sqlDataReader.GetSqlDouble(i);
    }

    public virtual SqlInt16 GetSqlInt16(int i)
    {
      return this._sqlDataReader.GetSqlInt16(i);
    }

    public virtual SqlInt32 GetSqlInt32(int i)
    {
      return this._sqlDataReader.GetSqlInt32(i);
    }

    public virtual SqlInt64 GetSqlInt64(int i)
    {
      return this._sqlDataReader.GetSqlInt64(i);
    }

    public virtual SqlMoney GetSqlMoney(int i)
    {
      return this._sqlDataReader.GetSqlMoney(i);
    }

    public virtual SqlSingle GetSqlSingle(int i)
    {
      return this._sqlDataReader.GetSqlSingle(i);
    }

    public virtual SqlString GetSqlString(int i)
    {
      return this._sqlDataReader.GetSqlString(i);
    }

    public virtual SqlXml GetSqlXml(int i)
    {
      return this._sqlDataReader.GetSqlXml(i);
    }

    public virtual object GetSqlValue(int i)
    {
      return this._sqlDataReader.GetSqlValue(i);
    }

    public virtual int GetSqlValues(object[] values)
    {
      return this._sqlDataReader.GetSqlValues(values);
    }

    public virtual string GetString(int i)
    {
      return this._sqlDataReader.GetString(i);
    }

    public virtual T GetFieldValue<T>(int i)
    {
      return this._sqlDataReader.GetFieldValue<T>(i);
    }

    public virtual object GetValue(int i)
    {
      return this._sqlDataReader.GetValue(i);
    }

    public virtual TimeSpan GetTimeSpan(int i)
    {
      return this._sqlDataReader.GetTimeSpan(i);
    }

    public virtual DateTimeOffset GetDateTimeOffset(int i)
    {
      return this._sqlDataReader.GetDateTimeOffset(i);
    }

    public virtual int GetValues(object[] values)
    {
      return this._sqlDataReader.GetValues(values);
    }

    public virtual bool IsDBNull(int i)
    {
      return this._sqlDataReader.IsDBNull(i);
    }

    public virtual bool NextResult()
    {
      return this._sqlDataReader.NextResult();
    }

    public virtual bool Read()
    {
      return this._sqlDataReader.Read();
    }

    public virtual Task<bool> NextResultAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return this._sqlDataReader.NextResultAsync(cancellationToken);
    }

    public virtual Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return this._sqlDataReader.ReadAsync(cancellationToken);
    }

    public virtual Task<bool> IsDBNullAsync(int i, CancellationToken cancellationToken)
    {
      return this._sqlDataReader.IsDBNullAsync(i, cancellationToken);
    }

    public virtual Task<T> GetFieldValueAsync<T>(int i, CancellationToken cancellationToken)
    {
      return this._sqlDataReader.GetFieldValueAsync<T>(i, cancellationToken);
    }

    public virtual int Depth
    {
      get
      {
        return this._sqlDataReader.Depth;
      }
    }

    public virtual int FieldCount
    {
      get
      {
        return this._sqlDataReader.FieldCount;
      }
    }

    public virtual bool HasRows
    {
      get
      {
        return this._sqlDataReader.HasRows;
      }
    }

    public virtual bool IsClosed
    {
      get
      {
        return this._sqlDataReader.IsClosed;
      }
    }

    public virtual int RecordsAffected
    {
      get
      {
        return this._sqlDataReader.RecordsAffected;
      }
    }

    public virtual int VisibleFieldCount
    {
      get
      {
        return this._sqlDataReader.VisibleFieldCount;
      }
    }

    public virtual object this[int i]
    {
      get
      {
        return this._sqlDataReader[i];
      }
    }

    public virtual object this[string name]
    {
      get
      {
        return this._sqlDataReader[name];
      }
    }
  }
}
