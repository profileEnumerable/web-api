// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.EntityDataReader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.EntityClient
{
  /// <summary>A data reader class for the entity client provider</summary>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
  public class EntityDataReader : DbDataReader, IExtendedDataRecord, IDataRecord
  {
    private EntityCommand _command;
    private readonly CommandBehavior _behavior;
    private readonly DbDataReader _storeDataReader;
    private readonly IExtendedDataRecord _storeExtendedDataRecord;
    private bool _disposed;

    internal EntityDataReader(
      EntityCommand command,
      DbDataReader storeDataReader,
      CommandBehavior behavior)
    {
      this._command = command;
      this._storeDataReader = storeDataReader;
      this._storeExtendedDataRecord = storeDataReader as IExtendedDataRecord;
      this._behavior = behavior;
    }

    internal EntityDataReader()
    {
    }

    /// <summary>Gets a value indicating the depth of nesting for the current row.</summary>
    /// <returns>The depth of nesting for the current row.</returns>
    public override int Depth
    {
      get
      {
        return this._storeDataReader.Depth;
      }
    }

    /// <summary>Gets the number of columns in the current row.</summary>
    /// <returns>The number of columns in the current row.</returns>
    public override int FieldCount
    {
      get
      {
        return this._storeDataReader.FieldCount;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether this <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" /> contains one or more rows.
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" /> contains one or more rows; otherwise, false.
    /// </returns>
    public override bool HasRows
    {
      get
      {
        return this._storeDataReader.HasRows;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" /> is closed.
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" /> is closed; otherwise, false.
    /// </returns>
    public override bool IsClosed
    {
      get
      {
        return this._storeDataReader.IsClosed;
      }
    }

    /// <summary>Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.</summary>
    /// <returns>The number of rows changed, inserted, or deleted. Returns -1 for SELECT statements; 0 if no rows were affected or the statement failed.</returns>
    public override int RecordsAffected
    {
      get
      {
        return this._storeDataReader.RecordsAffected;
      }
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal</param>
    public override object this[int ordinal]
    {
      get
      {
        return this._storeDataReader[ordinal];
      }
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="name">The name of the column.</param>
    public override object this[string name]
    {
      get
      {
        Check.NotNull<string>(name, nameof (name));
        return this._storeDataReader[name];
      }
    }

    /// <summary>
    /// Gets the number of fields in the <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" /> that are not hidden.
    /// </summary>
    /// <returns>The number of fields that are not hidden.</returns>
    public override int VisibleFieldCount
    {
      get
      {
        return this._storeDataReader.VisibleFieldCount;
      }
    }

    /// <summary>
    /// Gets <see cref="T:System.Data.Entity.Core.Common.DataRecordInfo" /> for this
    /// <see cref="T:System.Data.Entity.Core.IExtendedDataRecord" />
    /// .
    /// </summary>
    /// <returns>The information of a data record.</returns>
    public DataRecordInfo DataRecordInfo
    {
      get
      {
        if (this._storeExtendedDataRecord == null)
          return (DataRecordInfo) null;
        return this._storeExtendedDataRecord.DataRecordInfo;
      }
    }

    /// <summary>
    /// Closes the <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" /> object.
    /// </summary>
    public override void Close()
    {
      if (this._command == null)
        return;
      this._storeDataReader.Close();
      this._command.NotifyDataReaderClosing();
      if ((this._behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
        this._command.Connection.Close();
      this._command = (EntityCommand) null;
    }

    /// <summary>
    /// Releases the resources consumed by this <see cref="T:System.Data.Entity.Core.EntityClient.EntityDataReader" /> and calls
    /// <see cref="M:System.Data.Entity.Core.EntityClient.EntityDataReader.Close" />
    /// .
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      if (!this._disposed && disposing)
        this._storeDataReader.Dispose();
      this._disposed = true;
      base.Dispose(disposing);
    }

    /// <summary>Gets the value of the specified column as a Boolean.</summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override bool GetBoolean(int ordinal)
    {
      return this._storeDataReader.GetBoolean(ordinal);
    }

    /// <summary>Gets the value of the specified column as a byte.</summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override byte GetByte(int ordinal)
    {
      return this._storeDataReader.GetByte(ordinal);
    }

    /// <summary>Reads a stream of bytes from the specified column, starting at location indicated by  dataIndex , into the buffer, starting at the location indicated by  bufferIndex .</summary>
    /// <returns>The actual number of bytes read.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
    /// <param name="buffer">The buffer into which to copy the data.</param>
    /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
    /// <param name="length">The maximum number of characters to read.</param>
    public override long GetBytes(
      int ordinal,
      long dataOffset,
      byte[] buffer,
      int bufferOffset,
      int length)
    {
      return this._storeDataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
    }

    /// <summary>Gets the value of the specified column as a single character.</summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override char GetChar(int ordinal)
    {
      return this._storeDataReader.GetChar(ordinal);
    }

    /// <summary>Reads a stream of characters from the specified column, starting at location indicated by  dataIndex , into the buffer, starting at the location indicated by  bufferIndex .</summary>
    /// <returns>The actual number of characters read.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
    /// <param name="buffer">The buffer into which to copy the data.</param>
    /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
    /// <param name="length">The maximum number of characters to read.</param>
    public override long GetChars(
      int ordinal,
      long dataOffset,
      char[] buffer,
      int bufferOffset,
      int length)
    {
      return this._storeDataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
    }

    /// <summary>Gets the name of the data type of the specified column.</summary>
    /// <returns>The name of the data type.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override string GetDataTypeName(int ordinal)
    {
      return this._storeDataReader.GetDataTypeName(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a <see cref="T:System.DateTime" /> object.
    /// </summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override DateTime GetDateTime(int ordinal)
    {
      return this._storeDataReader.GetDateTime(ordinal);
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Common.DbDataReader" /> object for the requested column ordinal that can be overridden with a provider-specific implementation.
    /// </summary>
    /// <returns>A data reader.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    protected override DbDataReader GetDbDataReader(int ordinal)
    {
      return this._storeDataReader.GetData(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as a <see cref="T:System.Decimal" /> object.
    /// </summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override Decimal GetDecimal(int ordinal)
    {
      return this._storeDataReader.GetDecimal(ordinal);
    }

    /// <summary>Gets the value of the specified column as a double-precision floating point number.</summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override double GetDouble(int ordinal)
    {
      return this._storeDataReader.GetDouble(ordinal);
    }

    /// <summary>Gets the data type of the specified column.</summary>
    /// <returns>The data type of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override Type GetFieldType(int ordinal)
    {
      return this._storeDataReader.GetFieldType(ordinal);
    }

    /// <summary>Gets the value of the specified column as a single-precision floating point number.</summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override float GetFloat(int ordinal)
    {
      return this._storeDataReader.GetFloat(ordinal);
    }

    /// <summary>Gets the value of the specified column as a globally-unique identifier (GUID).</summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override Guid GetGuid(int ordinal)
    {
      return this._storeDataReader.GetGuid(ordinal);
    }

    /// <summary>Gets the value of the specified column as a 16-bit signed integer.</summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override short GetInt16(int ordinal)
    {
      return this._storeDataReader.GetInt16(ordinal);
    }

    /// <summary>Gets the value of the specified column as a 32-bit signed integer.</summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override int GetInt32(int ordinal)
    {
      return this._storeDataReader.GetInt32(ordinal);
    }

    /// <summary>Gets the value of the specified column as a 64-bit signed integer.</summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override long GetInt64(int ordinal)
    {
      return this._storeDataReader.GetInt64(ordinal);
    }

    /// <summary>Gets the name of the column, given the zero-based column ordinal.</summary>
    /// <returns>The name of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override string GetName(int ordinal)
    {
      return this._storeDataReader.GetName(ordinal);
    }

    /// <summary>Gets the column ordinal given the name of the column.</summary>
    /// <returns>The zero-based column ordinal.</returns>
    /// <param name="name">The name of the column.</param>
    /// <exception cref="T:System.IndexOutOfRangeException">The name specified is not a valid column name.</exception>
    public override int GetOrdinal(string name)
    {
      Check.NotNull<string>(name, nameof (name));
      return this._storeDataReader.GetOrdinal(name);
    }

    /// <summary>Returns the provider-specific field type of the specified column.</summary>
    /// <returns>
    /// The <see cref="T:System.Type" /> object that describes the data type of the specified column.
    /// </returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Type GetProviderSpecificFieldType(int ordinal)
    {
      return this._storeDataReader.GetProviderSpecificFieldType(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override object GetProviderSpecificValue(int ordinal)
    {
      return this._storeDataReader.GetProviderSpecificValue(ordinal);
    }

    /// <summary>Gets all provider-specific attribute columns in the collection for the current row.</summary>
    /// <returns>
    /// The number of instances of <see cref="T:System.Object" /> in the array.
    /// </returns>
    /// <param name="values">
    /// An array of <see cref="T:System.Object" /> into which to copy the attribute columns.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetProviderSpecificValues(object[] values)
    {
      return this._storeDataReader.GetProviderSpecificValues(values);
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the
    /// <see cref="T:System.Data.Common.DbDataReader" />
    /// .
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
    /// </returns>
    public override DataTable GetSchemaTable()
    {
      return this._storeDataReader.GetSchemaTable();
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="T:System.String" />.
    /// </summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override string GetString(int ordinal)
    {
      return this._storeDataReader.GetString(ordinal);
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>The value of the specified column.</returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override object GetValue(int ordinal)
    {
      return this._storeDataReader.GetValue(ordinal);
    }

    /// <summary>Populates an array of objects with the column values of the current row.</summary>
    /// <returns>
    /// The number of instances of <see cref="T:System.Object" /> in the array.
    /// </returns>
    /// <param name="values">
    /// An array of <see cref="T:System.Object" /> into which to copy the attribute columns.
    /// </param>
    public override int GetValues(object[] values)
    {
      return this._storeDataReader.GetValues(values);
    }

    /// <summary>Gets a value that indicates whether the column contains nonexistent or missing values.</summary>
    /// <returns>
    /// true if the specified column is equivalent to <see cref="T:System.DBNull" />; otherwise, false.
    /// </returns>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    public override bool IsDBNull(int ordinal)
    {
      return this._storeDataReader.IsDBNull(ordinal);
    }

    /// <summary>Advances the reader to the next result when reading the results of a batch of statements.</summary>
    /// <returns>true if there are more result sets; otherwise, false.</returns>
    public override bool NextResult()
    {
      try
      {
        return this._storeDataReader.NextResult();
      }
      catch (Exception ex)
      {
        throw new EntityCommandExecutionException(Strings.EntityClient_StoreReaderFailed, ex);
      }
    }

    /// <summary>
    /// Asynchronously moves the reader to the next result set when reading a batch of statements
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains true if there are more result sets; false otherwise.
    /// </returns>
    public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      bool flag;
      try
      {
        flag = await this._storeDataReader.NextResultAsync(cancellationToken).WithCurrentCulture<bool>();
      }
      catch (Exception ex)
      {
        throw new EntityCommandExecutionException(Strings.EntityClient_StoreReaderFailed, ex);
      }
      return flag;
    }

    /// <summary>Advances the reader to the next record in a result set.</summary>
    /// <returns>true if there are more rows; otherwise, false.</returns>
    public override bool Read()
    {
      return this._storeDataReader.Read();
    }

    /// <summary>
    /// Asynchronously moves the reader to the next row of the current result set
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains true if there are more rows; false otherwise.
    /// </returns>
    public override Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return this._storeDataReader.ReadAsync(cancellationToken);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.
    /// </returns>
    public override IEnumerator GetEnumerator()
    {
      return this._storeDataReader.GetEnumerator();
    }

    /// <summary>
    /// Returns a nested <see cref="T:System.Data.Common.DbDataRecord" />.
    /// </summary>
    /// <returns>The nested data record.</returns>
    /// <param name="i">The number of the DbDataRecord to return.</param>
    public DbDataRecord GetDataRecord(int i)
    {
      if (this._storeExtendedDataRecord == null)
        throw new ArgumentOutOfRangeException(nameof (i));
      return this._storeExtendedDataRecord.GetDataRecord(i);
    }

    /// <summary>
    /// Returns nested readers as <see cref="T:System.Data.Common.DbDataReader" /> objects.
    /// </summary>
    /// <returns>
    /// The nested readers as <see cref="T:System.Data.Common.DbDataReader" /> objects.
    /// </returns>
    /// <param name="i">The ordinal of the column.</param>
    public DbDataReader GetDataReader(int i)
    {
      return this.GetDbDataReader(i);
    }
  }
}
