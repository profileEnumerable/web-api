// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.ResultAssembly.BridgeDataReader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Internal.Materialization;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.PlanCompiler;
using System.Data.Entity.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Query.ResultAssembly
{
  internal class BridgeDataReader : DbDataReader, IExtendedDataRecord, IDataRecord
  {
    private Shaper<RecordState> _shaper;
    private IEnumerator<KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>>> _nextResultShaperInfoEnumerator;
    private CoordinatorFactory<RecordState> _coordinatorFactory;
    private RecordState _defaultRecordState;
    private BridgeDataRecord _dataRecord;
    private bool _hasRows;
    private bool _isClosed;
    private int _initialized;
    private readonly Action _initialize;
    private readonly Func<CancellationToken, Task> _initializeAsync;

    internal BridgeDataReader(
      Shaper<RecordState> shaper,
      CoordinatorFactory<RecordState> coordinatorFactory,
      int depth,
      IEnumerator<KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>>> nextResultShaperInfos)
    {
      BridgeDataReader bridgeDataReader = this;
      this._nextResultShaperInfoEnumerator = nextResultShaperInfos;
      this._initialize = (Action) (() => bridgeDataReader.SetShaper(shaper, coordinatorFactory, depth));
      this._initializeAsync = (Func<CancellationToken, Task>) (ct => bridgeDataReader.SetShaperAsync(shaper, coordinatorFactory, depth, ct));
    }

    protected virtual void EnsureInitialized()
    {
      if (Interlocked.CompareExchange(ref this._initialized, 1, 0) != 0)
        return;
      this._initialize();
    }

    protected virtual Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (Interlocked.CompareExchange(ref this._initialized, 1, 0) != 0)
        return (Task) Task.FromResult<object>((object) null);
      return this._initializeAsync(cancellationToken);
    }

    private void SetShaper(
      Shaper<RecordState> shaper,
      CoordinatorFactory<RecordState> coordinatorFactory,
      int depth)
    {
      this._shaper = shaper;
      this._coordinatorFactory = coordinatorFactory;
      this._dataRecord = new BridgeDataRecord(shaper, depth);
      if (!this._shaper.DataWaiting)
        this._shaper.DataWaiting = this._shaper.RootEnumerator.MoveNext();
      this.InitializeHasRows();
    }

    private async Task SetShaperAsync(
      Shaper<RecordState> shaper,
      CoordinatorFactory<RecordState> coordinatorFactory,
      int depth,
      CancellationToken cancellationToken)
    {
      this._shaper = shaper;
      this._coordinatorFactory = coordinatorFactory;
      this._dataRecord = new BridgeDataRecord(shaper, depth);
      if (!this._shaper.DataWaiting)
        this._shaper.DataWaiting = (await this._shaper.RootEnumerator.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>() ? 1 : 0) != 0;
      this.InitializeHasRows();
    }

    private void InitializeHasRows()
    {
      this._hasRows = false;
      if (this._shaper.DataWaiting)
      {
        RecordState current = this._shaper.RootEnumerator.Current;
        if (current != null)
          this._hasRows = current.CoordinatorFactory == this._coordinatorFactory;
      }
      this._defaultRecordState = this._coordinatorFactory.GetDefaultRecordState(this._shaper);
    }

    private void AssertReaderIsOpen(string methodName)
    {
      if (!this.IsClosed)
        return;
      if (this._dataRecord.IsImplicitlyClosed)
        throw Error.ADP_ImplicitlyClosedDataReaderError();
      if (this._dataRecord.IsExplicitlyClosed)
        throw Error.ADP_DataReaderClosed((object) methodName);
    }

    internal void CloseImplicitly()
    {
      this.EnsureInitialized();
      this.Consume();
      this._dataRecord.CloseImplicitly();
    }

    internal async Task CloseImplicitlyAsync(CancellationToken cancellationToken)
    {
      await this.EnsureInitializedAsync(cancellationToken).WithCurrentCulture();
      await this.ConsumeAsync(cancellationToken).WithCurrentCulture();
      await this._dataRecord.CloseImplicitlyAsync(cancellationToken).WithCurrentCulture();
    }

    private void Consume()
    {
      do
        ;
      while (this.ReadInternal());
    }

    private async Task ConsumeAsync(CancellationToken cancellationToken)
    {
      do
        ;
      while (await this.ReadInternalAsync(cancellationToken).WithCurrentCulture<bool>());
    }

    internal static Type GetClrTypeFromTypeMetadata(TypeUsage typeUsage)
    {
      PrimitiveType type;
      return !TypeHelpers.TryGetEdmType<PrimitiveType>(typeUsage, out type) ? (!TypeSemantics.IsReferenceType(typeUsage) ? (!TypeUtils.IsStructuredType(typeUsage) ? (!TypeUtils.IsCollectionType(typeUsage) ? (!TypeUtils.IsEnumerationType(typeUsage) ? typeof (object) : ((EnumType) typeUsage.EdmType).UnderlyingType.ClrEquivalentType) : typeof (DbDataReader)) : typeof (DbDataRecord)) : typeof (EntityKey)) : type.ClrEquivalentType;
    }

    public override int Depth
    {
      get
      {
        this.EnsureInitialized();
        this.AssertReaderIsOpen(nameof (Depth));
        return this._dataRecord.Depth;
      }
    }

    public override bool HasRows
    {
      get
      {
        this.EnsureInitialized();
        this.AssertReaderIsOpen(nameof (HasRows));
        return this._hasRows;
      }
    }

    public override bool IsClosed
    {
      get
      {
        this.EnsureInitialized();
        if (!this._isClosed)
          return this._dataRecord.IsClosed;
        return true;
      }
    }

    public override int RecordsAffected
    {
      get
      {
        this.EnsureInitialized();
        int num = -1;
        if (this._dataRecord.Depth == 0)
          num = this._shaper.Reader.RecordsAffected;
        return num;
      }
    }

    public override void Close()
    {
      this.EnsureInitialized();
      this._dataRecord.CloseExplicitly();
      if (!this._isClosed)
      {
        this._isClosed = true;
        if (this._dataRecord.Depth == 0)
          this._shaper.Reader.Close();
        else
          this.Consume();
      }
      if (this._nextResultShaperInfoEnumerator == null)
        return;
      this._nextResultShaperInfoEnumerator.Dispose();
      this._nextResultShaperInfoEnumerator = (IEnumerator<KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>>>) null;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override IEnumerator GetEnumerator()
    {
      return (IEnumerator) new DbEnumerator((IDataReader) this, true);
    }

    public override DataTable GetSchemaTable()
    {
      throw new NotSupportedException(Strings.ADP_GetSchemaTableIsNotSupported);
    }

    public override bool NextResult()
    {
      this.EnsureInitialized();
      this.AssertReaderIsOpen(nameof (NextResult));
      if (this._nextResultShaperInfoEnumerator != null && this._shaper.Reader.NextResult() && this._nextResultShaperInfoEnumerator.MoveNext())
      {
        KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>> current = this._nextResultShaperInfoEnumerator.Current;
        this._dataRecord.CloseImplicitly();
        this.SetShaper(current.Key, current.Value, 0);
        return true;
      }
      if (this._dataRecord.Depth == 0)
        CommandHelper.ConsumeReader(this._shaper.Reader);
      else
        this.Consume();
      this.CloseImplicitly();
      this._dataRecord.SetRecordSource((RecordState) null, false);
      return false;
    }

    public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
    {
      await this.EnsureInitializedAsync(cancellationToken).WithCurrentCulture();
      this.AssertReaderIsOpen("NextResult");
      if (this._nextResultShaperInfoEnumerator != null)
      {
        if (await this._shaper.Reader.NextResultAsync(cancellationToken).WithCurrentCulture<bool>() && this._nextResultShaperInfoEnumerator.MoveNext())
        {
          KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>> nextResultShaperInfo = this._nextResultShaperInfoEnumerator.Current;
          await this._dataRecord.CloseImplicitlyAsync(cancellationToken).WithCurrentCulture();
          this.SetShaper(nextResultShaperInfo.Key, nextResultShaperInfo.Value, 0);
          return true;
        }
      }
      if (this._dataRecord.Depth == 0)
        await CommandHelper.ConsumeReaderAsync(this._shaper.Reader, cancellationToken).WithCurrentCulture();
      else
        await this.ConsumeAsync(cancellationToken).WithCurrentCulture();
      await this.CloseImplicitlyAsync(cancellationToken).WithCurrentCulture();
      this._dataRecord.SetRecordSource((RecordState) null, false);
      return false;
    }

    public override bool Read()
    {
      this.EnsureInitialized();
      this.AssertReaderIsOpen(nameof (Read));
      this._dataRecord.CloseImplicitly();
      bool hasData = this.ReadInternal();
      this._dataRecord.SetRecordSource(this._shaper.RootEnumerator.Current, hasData);
      return hasData;
    }

    public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
      await this.EnsureInitializedAsync(cancellationToken).WithCurrentCulture();
      this.AssertReaderIsOpen("Read");
      await this._dataRecord.CloseImplicitlyAsync(cancellationToken).WithCurrentCulture();
      bool result = await this.ReadInternalAsync(cancellationToken).WithCurrentCulture<bool>();
      this._dataRecord.SetRecordSource(this._shaper.RootEnumerator.Current, result);
      return result;
    }

    private bool ReadInternal()
    {
      bool flag = false;
      if (!this._shaper.DataWaiting)
        this._shaper.DataWaiting = this._shaper.RootEnumerator.MoveNext();
      while (this._shaper.DataWaiting && this._shaper.RootEnumerator.Current.CoordinatorFactory != this._coordinatorFactory && this._shaper.RootEnumerator.Current.CoordinatorFactory.Depth > this._coordinatorFactory.Depth)
        this._shaper.DataWaiting = this._shaper.RootEnumerator.MoveNext();
      if (this._shaper.DataWaiting && this._shaper.RootEnumerator.Current.CoordinatorFactory == this._coordinatorFactory)
      {
        this._shaper.DataWaiting = false;
        this._shaper.RootEnumerator.Current.AcceptPendingValues();
        flag = true;
      }
      return flag;
    }

    private async Task<bool> ReadInternalAsync(CancellationToken cancellationToken)
    {
      bool result = false;
      if (!this._shaper.DataWaiting)
        this._shaper.DataWaiting = (await this._shaper.RootEnumerator.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>() ? 1 : 0) != 0;
      Shaper<RecordState> shaper;
      int num;
      for (; this._shaper.DataWaiting && this._shaper.RootEnumerator.Current.CoordinatorFactory != this._coordinatorFactory && this._shaper.RootEnumerator.Current.CoordinatorFactory.Depth > this._coordinatorFactory.Depth; shaper.DataWaiting = num != 0)
      {
        shaper = this._shaper;
        num = await this._shaper.RootEnumerator.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>() ? 1 : 0;
      }
      if (this._shaper.DataWaiting && this._shaper.RootEnumerator.Current.CoordinatorFactory == this._coordinatorFactory)
      {
        this._shaper.DataWaiting = false;
        this._shaper.RootEnumerator.Current.AcceptPendingValues();
        result = true;
      }
      return result;
    }

    public override int FieldCount
    {
      get
      {
        this.EnsureInitialized();
        this.AssertReaderIsOpen(nameof (FieldCount));
        return this._defaultRecordState.ColumnCount;
      }
    }

    public override string GetDataTypeName(int ordinal)
    {
      this.EnsureInitialized();
      this.AssertReaderIsOpen(nameof (GetDataTypeName));
      return !this._dataRecord.HasData ? this._defaultRecordState.GetTypeUsage(ordinal).ToString() : this._dataRecord.GetDataTypeName(ordinal);
    }

    public override Type GetFieldType(int ordinal)
    {
      this.EnsureInitialized();
      this.AssertReaderIsOpen(nameof (GetFieldType));
      return !this._dataRecord.HasData ? BridgeDataReader.GetClrTypeFromTypeMetadata(this._defaultRecordState.GetTypeUsage(ordinal)) : this._dataRecord.GetFieldType(ordinal);
    }

    public override string GetName(int ordinal)
    {
      this.EnsureInitialized();
      this.AssertReaderIsOpen(nameof (GetName));
      return !this._dataRecord.HasData ? this._defaultRecordState.GetName(ordinal) : this._dataRecord.GetName(ordinal);
    }

    public override int GetOrdinal(string name)
    {
      this.EnsureInitialized();
      this.AssertReaderIsOpen(nameof (GetOrdinal));
      return !this._dataRecord.HasData ? this._defaultRecordState.GetOrdinal(name) : this._dataRecord.GetOrdinal(name);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Type GetProviderSpecificFieldType(int ordinal)
    {
      throw new NotSupportedException();
    }

    public override object this[int ordinal]
    {
      get
      {
        this.EnsureInitialized();
        return this._dataRecord[ordinal];
      }
    }

    public override object this[string name]
    {
      get
      {
        this.EnsureInitialized();
        return this._dataRecord[this.GetOrdinal(name)];
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override object GetProviderSpecificValue(int ordinal)
    {
      throw new NotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetProviderSpecificValues(object[] values)
    {
      throw new NotSupportedException();
    }

    public override object GetValue(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetValue(ordinal);
    }

    public override async Task<T> GetFieldValueAsync<T>(
      int ordinal,
      CancellationToken cancellationToken)
    {
      await this.EnsureInitializedAsync(cancellationToken).WithCurrentCulture();
      return await base.GetFieldValueAsync<T>(ordinal, cancellationToken).WithCurrentCulture<T>();
    }

    public override int GetValues(object[] values)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetValues(values);
    }

    public override bool GetBoolean(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetBoolean(ordinal);
    }

    public override byte GetByte(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetByte(ordinal);
    }

    public override char GetChar(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetChar(ordinal);
    }

    public override DateTime GetDateTime(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetDateTime(ordinal);
    }

    public override Decimal GetDecimal(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetDecimal(ordinal);
    }

    public override double GetDouble(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetDouble(ordinal);
    }

    public override float GetFloat(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetFloat(ordinal);
    }

    public override Guid GetGuid(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetGuid(ordinal);
    }

    public override short GetInt16(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetInt16(ordinal);
    }

    public override int GetInt32(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetInt32(ordinal);
    }

    public override long GetInt64(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetInt64(ordinal);
    }

    public override string GetString(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetString(ordinal);
    }

    public override bool IsDBNull(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.IsDBNull(ordinal);
    }

    public override long GetBytes(
      int ordinal,
      long dataOffset,
      byte[] buffer,
      int bufferOffset,
      int length)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
    }

    public override long GetChars(
      int ordinal,
      long dataOffset,
      char[] buffer,
      int bufferOffset,
      int length)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
    }

    protected override DbDataReader GetDbDataReader(int ordinal)
    {
      this.EnsureInitialized();
      return (DbDataReader) this._dataRecord.GetData(ordinal);
    }

    public DataRecordInfo DataRecordInfo
    {
      get
      {
        this.EnsureInitialized();
        this.AssertReaderIsOpen(nameof (DataRecordInfo));
        return !this._dataRecord.HasData ? this._defaultRecordState.DataRecordInfo : this._dataRecord.DataRecordInfo;
      }
    }

    public DbDataRecord GetDataRecord(int ordinal)
    {
      this.EnsureInitialized();
      return this._dataRecord.GetDataRecord(ordinal);
    }

    public DbDataReader GetDataReader(int ordinal)
    {
      this.EnsureInitialized();
      return this.GetDbDataReader(ordinal);
    }
  }
}
