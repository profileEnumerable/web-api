// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConfigurationTypeFilter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  internal class ConfigurationTypeFilter
  {
    public virtual bool IsEntityTypeConfiguration(Type type)
    {
      return ConfigurationTypeFilter.IsStructuralTypeConfiguration(type, typeof (EntityTypeConfiguration<>));
    }

    public virtual bool IsComplexTypeConfiguration(Type type)
    {
      return ConfigurationTypeFilter.IsStructuralTypeConfiguration(type, typeof (ComplexTypeConfiguration<>));
    }

    private static bool IsStructuralTypeConfiguration(Type type, Type structuralTypeConfiguration)
    {
      if (!type.IsAbstract())
        return type.TryGetElementType(structuralTypeConfiguration) != (Type) null;
      return false;
    }
  }
}
