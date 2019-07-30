// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataPropertyCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal sealed class MetadataPropertyCollection : MetadataCollection<MetadataProperty>
  {
    private static readonly Memoizer<Type, MetadataPropertyCollection.ItemTypeInformation> _itemTypeMemoizer = new Memoizer<Type, MetadataPropertyCollection.ItemTypeInformation>((Func<Type, MetadataPropertyCollection.ItemTypeInformation>) (clrType => new MetadataPropertyCollection.ItemTypeInformation(clrType)), (IEqualityComparer<Type>) null);

    internal MetadataPropertyCollection(MetadataItem item)
      : base(MetadataPropertyCollection.GetSystemMetadataProperties(item))
    {
    }

    private static IEnumerable<MetadataProperty> GetSystemMetadataProperties(
      MetadataItem item)
    {
      return MetadataPropertyCollection.GetItemTypeInformation(item.GetType()).GetItemAttributes(item);
    }

    private static MetadataPropertyCollection.ItemTypeInformation GetItemTypeInformation(
      Type clrType)
    {
      return MetadataPropertyCollection._itemTypeMemoizer.Evaluate(clrType);
    }

    private class ItemTypeInformation
    {
      private readonly List<MetadataPropertyCollection.ItemPropertyInfo> _itemProperties;

      internal ItemTypeInformation(Type clrType)
      {
        this._itemProperties = MetadataPropertyCollection.ItemTypeInformation.GetItemProperties(clrType);
      }

      internal IEnumerable<MetadataProperty> GetItemAttributes(
        MetadataItem item)
      {
        foreach (MetadataPropertyCollection.ItemPropertyInfo itemProperty in this._itemProperties)
          yield return itemProperty.GetMetadataProperty(item);
      }

      private static List<MetadataPropertyCollection.ItemPropertyInfo> GetItemProperties(
        Type clrType)
      {
        List<MetadataPropertyCollection.ItemPropertyInfo> itemPropertyInfoList = new List<MetadataPropertyCollection.ItemPropertyInfo>();
        foreach (PropertyInfo instanceProperty in clrType.GetInstanceProperties())
        {
          foreach (MetadataPropertyAttribute customAttribute in instanceProperty.GetCustomAttributes<MetadataPropertyAttribute>(false))
            itemPropertyInfoList.Add(new MetadataPropertyCollection.ItemPropertyInfo(instanceProperty, customAttribute));
        }
        return itemPropertyInfoList;
      }
    }

    private class ItemPropertyInfo
    {
      private readonly MetadataPropertyAttribute _attribute;
      private readonly PropertyInfo _propertyInfo;

      internal ItemPropertyInfo(PropertyInfo propertyInfo, MetadataPropertyAttribute attribute)
      {
        this._propertyInfo = propertyInfo;
        this._attribute = attribute;
      }

      internal MetadataProperty GetMetadataProperty(MetadataItem item)
      {
        return new MetadataProperty(this._propertyInfo.Name, this._attribute.Type, this._attribute.IsCollectionType, (object) new MetadataPropertyValue(this._propertyInfo, item));
      }
    }
  }
}
