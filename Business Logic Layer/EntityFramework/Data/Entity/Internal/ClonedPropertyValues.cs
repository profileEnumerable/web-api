// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ClonedPropertyValues
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;

namespace System.Data.Entity.Internal
{
  internal class ClonedPropertyValues : InternalPropertyValues
  {
    private readonly ISet<string> _propertyNames;
    private readonly IDictionary<string, ClonedPropertyValuesItem> _propertyValues;

    internal ClonedPropertyValues(InternalPropertyValues original, DbDataRecord valuesRecord = null)
      : base(original.InternalContext, original.ObjectType, original.IsEntityValues)
    {
      this._propertyNames = original.PropertyNames;
      this._propertyValues = (IDictionary<string, ClonedPropertyValuesItem>) new Dictionary<string, ClonedPropertyValuesItem>(this._propertyNames.Count);
      foreach (string propertyName in (IEnumerable<string>) this._propertyNames)
      {
        IPropertyValuesItem propertyValuesItem = original.GetItem(propertyName);
        object obj = propertyValuesItem.Value;
        InternalPropertyValues original1 = obj as InternalPropertyValues;
        if (original1 != null)
        {
          DbDataRecord valuesRecord1 = valuesRecord == null ? (DbDataRecord) null : (DbDataRecord) valuesRecord[propertyName];
          obj = (object) new ClonedPropertyValues(original1, valuesRecord1);
        }
        else if (valuesRecord != null)
        {
          obj = valuesRecord[propertyName];
          if (obj == DBNull.Value)
            obj = (object) null;
        }
        this._propertyValues[propertyName] = new ClonedPropertyValuesItem(propertyName, obj, propertyValuesItem.Type, propertyValuesItem.IsComplex);
      }
    }

    protected override IPropertyValuesItem GetItemImpl(string propertyName)
    {
      return (IPropertyValuesItem) this._propertyValues[propertyName];
    }

    public override ISet<string> PropertyNames
    {
      get
      {
        return this._propertyNames;
      }
    }
  }
}
