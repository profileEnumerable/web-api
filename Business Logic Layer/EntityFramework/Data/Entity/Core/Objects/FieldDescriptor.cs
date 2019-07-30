// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.FieldDescriptor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class FieldDescriptor : PropertyDescriptor
  {
    private readonly EdmProperty _property;
    private readonly Type _fieldType;
    private readonly Type _itemType;
    private readonly bool _isReadOnly;

    internal FieldDescriptor(string propertyName)
      : base(propertyName, (Attribute[]) null)
    {
    }

    internal FieldDescriptor(Type itemType, bool isReadOnly, EdmProperty property)
      : base(property.Name, (Attribute[]) null)
    {
      this._itemType = itemType;
      this._property = property;
      this._isReadOnly = isReadOnly;
      this._fieldType = this.DetermineClrType(this._property.TypeUsage);
    }

    private Type DetermineClrType(TypeUsage typeUsage)
    {
      Type type = (Type) null;
      EdmType edmType = typeUsage.EdmType;
      switch (edmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.CollectionType:
          type = typeof (IEnumerable<>).MakeGenericType(this.DetermineClrType(((CollectionType) edmType).TypeUsage));
          break;
        case BuiltInTypeKind.ComplexType:
        case BuiltInTypeKind.EntityType:
          type = edmType.ClrType;
          break;
        case BuiltInTypeKind.EnumType:
        case BuiltInTypeKind.PrimitiveType:
          type = edmType.ClrType;
          Facet facet;
          if (type.IsValueType() && typeUsage.Facets.TryGetValue("Nullable", false, out facet) && (bool) facet.Value)
          {
            type = typeof (Nullable<>).MakeGenericType(type);
            break;
          }
          break;
        case BuiltInTypeKind.RefType:
          type = typeof (EntityKey);
          break;
        case BuiltInTypeKind.RowType:
          type = typeof (IDataRecord);
          break;
      }
      return type;
    }

    internal EdmProperty EdmProperty
    {
      get
      {
        return this._property;
      }
    }

    public override Type ComponentType
    {
      get
      {
        return this._itemType;
      }
    }

    public override bool IsReadOnly
    {
      get
      {
        return this._isReadOnly;
      }
    }

    public override Type PropertyType
    {
      get
      {
        return this._fieldType;
      }
    }

    public override bool CanResetValue(object item)
    {
      return false;
    }

    public override object GetValue(object item)
    {
      Check.NotNull<object>(item, nameof (item));
      if (!this._itemType.IsAssignableFrom(item.GetType()))
        throw new ArgumentException(Strings.ObjectView_IncompatibleArgument);
      DbDataRecord dbDataRecord = item as DbDataRecord;
      return dbDataRecord == null ? DelegateFactory.GetValue(this._property, item) : dbDataRecord.GetValue(dbDataRecord.GetOrdinal(this._property.Name));
    }

    public override void ResetValue(object item)
    {
      throw new NotSupportedException();
    }

    public override void SetValue(object item, object value)
    {
      Check.NotNull<object>(item, nameof (item));
      if (!this._itemType.IsAssignableFrom(item.GetType()))
        throw new ArgumentException(Strings.ObjectView_IncompatibleArgument);
      if (this._isReadOnly)
        throw new InvalidOperationException(Strings.ObjectView_WriteOperationNotAllowedOnReadOnlyBindingList);
      DelegateFactory.SetValue(this._property, item, value);
    }

    public override bool ShouldSerializeValue(object item)
    {
      return false;
    }

    public override bool IsBrowsable
    {
      get
      {
        return true;
      }
    }
  }
}
