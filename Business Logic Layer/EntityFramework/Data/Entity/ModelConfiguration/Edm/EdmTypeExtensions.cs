// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.EdmTypeExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class EdmTypeExtensions
  {
    public static Type GetClrType(this EdmType item)
    {
      EntityType entityType = item as EntityType;
      if (entityType != null)
        return EntityTypeExtensions.GetClrType(entityType);
      EnumType enumType = item as EnumType;
      if (enumType != null)
        return EnumTypeExtensions.GetClrType(enumType);
      ComplexType complexType = item as ComplexType;
      if (complexType != null)
        return ComplexTypeExtensions.GetClrType(complexType);
      return (Type) null;
    }
  }
}
