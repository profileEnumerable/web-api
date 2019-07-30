// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.MaterializedDataRecord
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class MaterializedDataRecord : DbDataRecord, IExtendedDataRecord, IDataRecord, ICustomTypeDescriptor
  {
    private FieldNameLookup _fieldNameLookup;
    private DataRecordInfo _recordInfo;
    private readonly MetadataWorkspace _workspace;
    private readonly TypeUsage _edmUsage;
    private readonly object[] _values;
    private PropertyDescriptorCollection _propertyDescriptors;
    private MaterializedDataRecord.FilterCache _filterCache;
    private Dictionary<object, AttributeCollection> _attrCache;

    internal MaterializedDataRecord(
      MetadataWorkspace workspace,
      TypeUsage edmUsage,
      object[] values)
    {
      this._workspace = workspace;
      this._edmUsage = edmUsage;
      this._values = values;
    }

    public DataRecordInfo DataRecordInfo
    {
      get
      {
        if (this._recordInfo == null)
          this._recordInfo = this._workspace != null ? new DataRecordInfo(this._workspace.GetOSpaceTypeUsage(this._edmUsage)) : new DataRecordInfo(this._edmUsage);
        return this._recordInfo;
      }
    }

    public override int FieldCount
    {
      get
      {
        return this._values.Length;
      }
    }

    public override object this[int ordinal]
    {
      get
      {
        return this.GetValue(ordinal);
      }
    }

    public override object this[string name]
    {
      get
      {
        return this.GetValue(this.GetOrdinal(name));
      }
    }

    public override bool GetBoolean(int ordinal)
    {
      return (bool) this._values[ordinal];
    }

    public override byte GetByte(int ordinal)
    {
      return (byte) this._values[ordinal];
    }

    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    public override long GetBytes(
      int ordinal,
      long fieldOffset,
      byte[] buffer,
      int bufferOffset,
      int length)
    {
      byte[] numArray = (byte[]) this._values[ordinal];
      int length1 = numArray.Length;
      if (fieldOffset > (long) int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (fieldOffset), Strings.ADP_InvalidSourceBufferIndex((object) length1.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) fieldOffset.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      int sourceIndex = (int) fieldOffset;
      if (buffer == null)
        return (long) length1;
      try
      {
        if (sourceIndex < length1)
        {
          if (sourceIndex + length > length1)
            length1 -= sourceIndex;
          else
            length1 = length;
        }
        Array.Copy((Array) numArray, sourceIndex, (Array) buffer, bufferOffset, length1);
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType())
        {
          int length2 = numArray.Length;
          if (length < 0)
            throw new IndexOutOfRangeException(Strings.ADP_InvalidDataLength((object) ((long) length).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (bufferOffset < 0 || bufferOffset >= buffer.Length)
            throw new ArgumentOutOfRangeException(nameof (bufferOffset), Strings.ADP_InvalidDestinationBufferIndex((object) length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) bufferOffset.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (fieldOffset < 0L || fieldOffset >= (long) length2)
            throw new ArgumentOutOfRangeException(nameof (fieldOffset), Strings.ADP_InvalidSourceBufferIndex((object) length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) fieldOffset.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (length2 + bufferOffset > buffer.Length)
            throw new IndexOutOfRangeException(Strings.ADP_InvalidBufferSizeOrIndex((object) length2.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) bufferOffset.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        }
        throw;
      }
      return (long) length1;
    }

    public override char GetChar(int ordinal)
    {
      return ((string) this.GetValue(ordinal))[0];
    }

    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
    public override long GetChars(
      int ordinal,
      long fieldOffset,
      char[] buffer,
      int bufferOffset,
      int length)
    {
      string str = (string) this._values[ordinal];
      int count = str.Length;
      if (fieldOffset > (long) int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (fieldOffset), Strings.ADP_InvalidSourceBufferIndex((object) count.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) fieldOffset.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      int sourceIndex = (int) fieldOffset;
      if (buffer == null)
        return (long) count;
      try
      {
        if (sourceIndex < count)
        {
          if (sourceIndex + length > count)
            count -= sourceIndex;
          else
            count = length;
        }
        str.CopyTo(sourceIndex, buffer, bufferOffset, count);
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType())
        {
          int length1 = str.Length;
          if (length < 0)
            throw new IndexOutOfRangeException(Strings.ADP_InvalidDataLength((object) ((long) length).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (bufferOffset < 0 || bufferOffset >= buffer.Length)
            throw new ArgumentOutOfRangeException(nameof (bufferOffset), Strings.ADP_InvalidDestinationBufferIndex((object) buffer.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) bufferOffset.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (fieldOffset < 0L || fieldOffset >= (long) length1)
            throw new ArgumentOutOfRangeException(nameof (fieldOffset), Strings.ADP_InvalidSourceBufferIndex((object) length1.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) fieldOffset.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (length1 + bufferOffset > buffer.Length)
            throw new IndexOutOfRangeException(Strings.ADP_InvalidBufferSizeOrIndex((object) length1.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) bufferOffset.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        }
        throw;
      }
      return (long) count;
    }

    public DbDataRecord GetDataRecord(int ordinal)
    {
      return (DbDataRecord) this._values[ordinal];
    }

    public DbDataReader GetDataReader(int i)
    {
      return this.GetDbDataReader(i);
    }

    public override string GetDataTypeName(int ordinal)
    {
      return this.GetMember(ordinal).TypeUsage.EdmType.Name;
    }

    public override DateTime GetDateTime(int ordinal)
    {
      return (DateTime) this._values[ordinal];
    }

    public override Decimal GetDecimal(int ordinal)
    {
      return (Decimal) this._values[ordinal];
    }

    public override double GetDouble(int ordinal)
    {
      return (double) this._values[ordinal];
    }

    public override Type GetFieldType(int ordinal)
    {
      Type clrType = this.GetMember(ordinal).TypeUsage.EdmType.ClrType;
      if ((object) clrType != null)
        return clrType;
      return typeof (object);
    }

    public override float GetFloat(int ordinal)
    {
      return (float) this._values[ordinal];
    }

    public override Guid GetGuid(int ordinal)
    {
      return (Guid) this._values[ordinal];
    }

    public override short GetInt16(int ordinal)
    {
      return (short) this._values[ordinal];
    }

    public override int GetInt32(int ordinal)
    {
      return (int) this._values[ordinal];
    }

    public override long GetInt64(int ordinal)
    {
      return (long) this._values[ordinal];
    }

    public override string GetName(int ordinal)
    {
      return this.GetMember(ordinal).Name;
    }

    public override int GetOrdinal(string name)
    {
      if (this._fieldNameLookup == null)
        this._fieldNameLookup = new FieldNameLookup((IDataRecord) this);
      return this._fieldNameLookup.GetOrdinal(name);
    }

    public override string GetString(int ordinal)
    {
      return (string) this._values[ordinal];
    }

    public override object GetValue(int ordinal)
    {
      return this._values[ordinal];
    }

    public override int GetValues(object[] values)
    {
      Check.NotNull<object[]>(values, nameof (values));
      int num = Math.Min(values.Length, this.FieldCount);
      for (int index = 0; index < num; ++index)
        values[index] = this._values[index];
      return num;
    }

    private EdmMember GetMember(int ordinal)
    {
      return this.DataRecordInfo.FieldMetadata[ordinal].FieldType;
    }

    public override bool IsDBNull(int ordinal)
    {
      return DBNull.Value == this._values[ordinal];
    }

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return TypeDescriptor.GetAttributes((object) this, true);
    }

    string ICustomTypeDescriptor.GetClassName()
    {
      return (string) null;
    }

    string ICustomTypeDescriptor.GetComponentName()
    {
      return (string) null;
    }

    private PropertyDescriptorCollection InitializePropertyDescriptors()
    {
      if (this._values == null)
        return (PropertyDescriptorCollection) null;
      if (this._propertyDescriptors == null && 0 < this._values.Length)
        this._propertyDescriptors = MaterializedDataRecord.CreatePropertyDescriptorCollection(this.DataRecordInfo.RecordType.EdmType as StructuralType, typeof (MaterializedDataRecord), true);
      return this._propertyDescriptors;
    }

    internal static PropertyDescriptorCollection CreatePropertyDescriptorCollection(
      StructuralType structuralType,
      Type componentType,
      bool isReadOnly)
    {
      List<PropertyDescriptor> propertyDescriptorList = new List<PropertyDescriptor>();
      if (structuralType != null)
      {
        foreach (EdmMember member in structuralType.Members)
        {
          if (member.BuiltInTypeKind == BuiltInTypeKind.EdmProperty)
          {
            EdmProperty property = (EdmProperty) member;
            FieldDescriptor fieldDescriptor = new FieldDescriptor(componentType, isReadOnly, property);
            propertyDescriptorList.Add((PropertyDescriptor) fieldDescriptor);
          }
        }
      }
      return new PropertyDescriptorCollection(propertyDescriptorList.ToArray());
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      return ((ICustomTypeDescriptor) this).GetProperties((Attribute[]) null);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(
      Attribute[] attributes)
    {
      bool flag = attributes != null && 0 < attributes.Length;
      PropertyDescriptorCollection descriptorCollection1 = this.InitializePropertyDescriptors();
      if (descriptorCollection1 == null)
        return descriptorCollection1;
      MaterializedDataRecord.FilterCache filterCache = this._filterCache;
      if (flag && filterCache != null && filterCache.IsValid(attributes))
        return filterCache.FilteredProperties;
      if (!flag && descriptorCollection1 != null)
        return descriptorCollection1;
      if (this._attrCache == null && attributes != null && 0 < attributes.Length)
      {
        this._attrCache = new Dictionary<object, AttributeCollection>();
        foreach (FieldDescriptor propertyDescriptor in this._propertyDescriptors)
        {
          object[] customAttributes = propertyDescriptor.GetValue((object) this).GetType().GetCustomAttributes(false);
          Attribute[] attributeArray = new Attribute[customAttributes.Length];
          customAttributes.CopyTo((Array) attributeArray, 0);
          this._attrCache.Add((object) propertyDescriptor, new AttributeCollection(attributeArray));
        }
      }
      PropertyDescriptorCollection descriptorCollection2 = new PropertyDescriptorCollection((PropertyDescriptor[]) null);
      foreach (PropertyDescriptor propertyDescriptor in this._propertyDescriptors)
      {
        if (this._attrCache[(object) propertyDescriptor].Matches(attributes))
          descriptorCollection2.Add(propertyDescriptor);
      }
      if (flag)
        this._filterCache = new MaterializedDataRecord.FilterCache()
        {
          Attributes = attributes,
          FilteredProperties = descriptorCollection2
        };
      return descriptorCollection2;
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return (object) this;
    }

    private class FilterCache
    {
      public Attribute[] Attributes;
      public PropertyDescriptorCollection FilteredProperties;

      public bool IsValid(Attribute[] other)
      {
        if (other == null || this.Attributes == null || this.Attributes.Length != other.Length)
          return false;
        for (int index = 0; index < other.Length; ++index)
        {
          if (!this.Attributes[index].Match((object) other[index]))
            return false;
        }
        return true;
      }
    }
  }
}
