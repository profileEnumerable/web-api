// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.DbDataRecordPropertyValuesItem
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Internal
{
  internal class DbDataRecordPropertyValuesItem : IPropertyValuesItem
  {
    private readonly DbUpdatableDataRecord _dataRecord;
    private readonly int _ordinal;
    private object _value;

    public DbDataRecordPropertyValuesItem(
      DbUpdatableDataRecord dataRecord,
      int ordinal,
      object value)
    {
      this._dataRecord = dataRecord;
      this._ordinal = ordinal;
      this._value = value;
    }

    public object Value
    {
      get
      {
        return this._value;
      }
      set
      {
        this._dataRecord.SetValue(this._ordinal, value);
        this._value = value;
      }
    }

    public string Name
    {
      get
      {
        return this._dataRecord.GetName(this._ordinal);
      }
    }

    public bool IsComplex
    {
      get
      {
        return this._dataRecord.DataRecordInfo.FieldMetadata[this._ordinal].FieldType.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType;
      }
    }

    public Type Type
    {
      get
      {
        return this._dataRecord.GetFieldType(this._ordinal);
      }
    }
  }
}
