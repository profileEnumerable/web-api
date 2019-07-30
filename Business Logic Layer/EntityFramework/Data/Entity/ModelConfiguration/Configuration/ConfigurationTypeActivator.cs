// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConfigurationTypeActivator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  internal class ConfigurationTypeActivator
  {
    public virtual TStructuralTypeConfiguration Activate<TStructuralTypeConfiguration>(Type type) where TStructuralTypeConfiguration : StructuralTypeConfiguration
    {
      if (type.GetDeclaredConstructor() == (ConstructorInfo) null)
        throw new InvalidOperationException(Strings.CreateConfigurationType_NoParameterlessConstructor((object) type.Name));
      return (TStructuralTypeConfiguration) typeof (StructuralTypeConfiguration<>).MakeGenericType(type.TryGetElementType(typeof (StructuralTypeConfiguration<>))).GetDeclaredProperty("Configuration").GetValue(Activator.CreateInstance(type, true), (object[]) null);
    }
  }
}
