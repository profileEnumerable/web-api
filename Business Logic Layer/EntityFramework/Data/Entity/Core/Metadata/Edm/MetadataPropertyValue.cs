// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataPropertyValue
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal sealed class MetadataPropertyValue
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly MetadataItem _item;

    internal MetadataPropertyValue(PropertyInfo propertyInfo, MetadataItem item)
    {
      this._propertyInfo = propertyInfo;
      this._item = item;
    }

    internal object GetValue()
    {
      return this._propertyInfo.GetValue((object) this._item, new object[0]);
    }
  }
}
