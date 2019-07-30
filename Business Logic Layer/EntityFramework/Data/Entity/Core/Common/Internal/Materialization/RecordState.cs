// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.RecordState
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class RecordState
  {
    private readonly RecordStateFactory RecordStateFactory;
    internal readonly CoordinatorFactory CoordinatorFactory;
    private bool _pendingIsNull;
    private bool _currentIsNull;
    private EntityRecordInfo _currentEntityRecordInfo;
    private EntityRecordInfo _pendingEntityRecordInfo;
    internal object[] CurrentColumnValues;
    internal object[] PendingColumnValues;

    internal RecordState(
      RecordStateFactory recordStateFactory,
      CoordinatorFactory coordinatorFactory)
    {
      this.RecordStateFactory = recordStateFactory;
      this.CoordinatorFactory = coordinatorFactory;
      this.CurrentColumnValues = new object[this.RecordStateFactory.ColumnCount];
      this.PendingColumnValues = new object[this.RecordStateFactory.ColumnCount];
    }

    internal void AcceptPendingValues()
    {
      object[] currentColumnValues = this.CurrentColumnValues;
      this.CurrentColumnValues = this.PendingColumnValues;
      this.PendingColumnValues = currentColumnValues;
      this._currentEntityRecordInfo = this._pendingEntityRecordInfo;
      this._pendingEntityRecordInfo = (EntityRecordInfo) null;
      this._currentIsNull = this._pendingIsNull;
      if (!this.RecordStateFactory.HasNestedColumns)
        return;
      for (int index = 0; index < this.CurrentColumnValues.Length; ++index)
      {
        if (this.RecordStateFactory.IsColumnNested[index])
          (this.CurrentColumnValues[index] as RecordState)?.AcceptPendingValues();
      }
    }

    internal int ColumnCount
    {
      get
      {
        return this.RecordStateFactory.ColumnCount;
      }
    }

    internal DataRecordInfo DataRecordInfo
    {
      get
      {
        return (DataRecordInfo) this._currentEntityRecordInfo ?? this.RecordStateFactory.DataRecordInfo;
      }
    }

    internal bool IsNull
    {
      get
      {
        return this._currentIsNull;
      }
    }

    internal long GetBytes(
      int ordinal,
      long dataOffset,
      byte[] buffer,
      int bufferOffset,
      int length)
    {
      byte[] currentColumnValue = (byte[]) this.CurrentColumnValues[ordinal];
      int length1 = currentColumnValue.Length;
      int srcOffset = (int) dataOffset;
      int num = length1 - srcOffset;
      if (buffer != null)
      {
        num = Math.Min(num, length);
        if (0 < num)
          Buffer.BlockCopy((Array) currentColumnValue, srcOffset, (Array) buffer, bufferOffset, num);
      }
      return (long) Math.Max(0, num);
    }

    internal long GetChars(
      int ordinal,
      long dataOffset,
      char[] buffer,
      int bufferOffset,
      int length)
    {
      string currentColumnValue = this.CurrentColumnValues[ordinal] as string;
      char[] chArray = currentColumnValue == null ? (char[]) this.CurrentColumnValues[ordinal] : currentColumnValue.ToCharArray();
      int length1 = chArray.Length;
      int num1 = (int) dataOffset;
      int num2 = length1 - num1;
      if (buffer != null)
      {
        num2 = Math.Min(num2, length);
        if (0 < num2)
          Buffer.BlockCopy((Array) chArray, num1 * 2, (Array) buffer, bufferOffset * 2, num2 * 2);
      }
      return (long) Math.Max(0, num2);
    }

    internal string GetName(int ordinal)
    {
      if (ordinal < 0 || ordinal >= this.RecordStateFactory.ColumnCount)
        throw new ArgumentOutOfRangeException(nameof (ordinal));
      return this.RecordStateFactory.ColumnNames[ordinal];
    }

    internal int GetOrdinal(string name)
    {
      return this.RecordStateFactory.FieldNameLookup.GetOrdinal(name);
    }

    internal TypeUsage GetTypeUsage(int ordinal)
    {
      return this.RecordStateFactory.TypeUsages[ordinal];
    }

    internal bool IsNestedObject(int ordinal)
    {
      return this.RecordStateFactory.IsColumnNested[ordinal];
    }

    internal void ResetToDefaultState()
    {
      this._currentEntityRecordInfo = (EntityRecordInfo) null;
    }

    internal RecordState GatherData(Shaper shaper)
    {
      int num = this.RecordStateFactory.GatherData(shaper) ? 1 : 0;
      this._pendingIsNull = false;
      return this;
    }

    internal bool SetColumnValue(int ordinal, object value)
    {
      this.PendingColumnValues[ordinal] = value;
      return true;
    }

    internal bool SetEntityRecordInfo(EntityKey entityKey, EntitySet entitySet)
    {
      this._pendingEntityRecordInfo = new EntityRecordInfo(this.RecordStateFactory.DataRecordInfo, entityKey, entitySet);
      return true;
    }

    internal RecordState SetNullRecord()
    {
      for (int index = 0; index < this.PendingColumnValues.Length; ++index)
        this.PendingColumnValues[index] = (object) DBNull.Value;
      this._pendingEntityRecordInfo = (EntityRecordInfo) null;
      this._pendingIsNull = true;
      return this;
    }
  }
}
