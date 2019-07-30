// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConventionsTypeFilter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  internal class ConventionsTypeFilter
  {
    public virtual bool IsConvention(Type conventionType)
    {
      if (!ConventionsTypeFilter.IsConfigurationConvention(conventionType) && !ConventionsTypeFilter.IsConceptualModelConvention(conventionType) && !ConventionsTypeFilter.IsConceptualToStoreMappingConvention(conventionType))
        return ConventionsTypeFilter.IsStoreModelConvention(conventionType);
      return true;
    }

    public static bool IsConfigurationConvention(Type conventionType)
    {
      if (!typeof (IConfigurationConvention).IsAssignableFrom(conventionType) && !typeof (Convention).IsAssignableFrom(conventionType) && !conventionType.GetGenericTypeImplementations(typeof (IConfigurationConvention<>)).Any<Type>())
        return conventionType.GetGenericTypeImplementations(typeof (IConfigurationConvention<,>)).Any<Type>();
      return true;
    }

    public static bool IsConceptualModelConvention(Type conventionType)
    {
      return conventionType.GetGenericTypeImplementations(typeof (IConceptualModelConvention<>)).Any<Type>();
    }

    public static bool IsStoreModelConvention(Type conventionType)
    {
      return conventionType.GetGenericTypeImplementations(typeof (IStoreModelConvention<>)).Any<Type>();
    }

    public static bool IsConceptualToStoreMappingConvention(Type conventionType)
    {
      return typeof (IDbMappingConvention).IsAssignableFrom(conventionType);
    }
  }
}
