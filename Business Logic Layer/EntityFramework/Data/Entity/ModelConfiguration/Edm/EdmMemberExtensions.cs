// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.EdmMemberExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class EdmMemberExtensions
  {
    public static PropertyInfo GetClrPropertyInfo(this EdmMember property)
    {
      return property.Annotations.GetClrPropertyInfo();
    }

    public static void SetClrPropertyInfo(this EdmMember property, PropertyInfo propertyInfo)
    {
      property.GetMetadataProperties().SetClrPropertyInfo(propertyInfo);
    }

    public static IEnumerable<T> GetClrAttributes<T>(this EdmMember property) where T : Attribute
    {
      IList<Attribute> clrAttributes = property.Annotations.GetClrAttributes();
      if (clrAttributes == null)
        return Enumerable.Empty<T>();
      return clrAttributes.OfType<T>();
    }
  }
}
