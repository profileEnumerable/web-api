// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Mappers.AttributeMapper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Mappers
{
  internal sealed class AttributeMapper
  {
    private readonly AttributeProvider _attributeProvider;

    public AttributeMapper(AttributeProvider attributeProvider)
    {
      this._attributeProvider = attributeProvider;
    }

    public void Map(PropertyInfo propertyInfo, ICollection<MetadataProperty> annotations)
    {
      annotations.SetClrAttributes((IList<Attribute>) this._attributeProvider.GetAttributes(propertyInfo).ToList<Attribute>());
    }

    public void Map(Type type, ICollection<MetadataProperty> annotations)
    {
      annotations.SetClrAttributes((IList<Attribute>) this._attributeProvider.GetAttributes(type).ToList<Attribute>());
    }
  }
}
