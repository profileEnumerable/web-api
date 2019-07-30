// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.ProviderServicesFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class ProviderServicesFactory
  {
    public virtual DbProviderServices TryGetInstance(string providerTypeName)
    {
      Type type = Type.GetType(providerTypeName, false);
      if (!(type == (Type) null))
        return ProviderServicesFactory.GetInstance(type);
      return (DbProviderServices) null;
    }

    public virtual DbProviderServices GetInstance(
      string providerTypeName,
      string providerInvariantName)
    {
      Type type = Type.GetType(providerTypeName, false);
      if (type == (Type) null)
        throw new InvalidOperationException(Strings.EF6Providers_ProviderTypeMissing((object) providerTypeName, (object) providerInvariantName));
      return ProviderServicesFactory.GetInstance(type);
    }

    private static DbProviderServices GetInstance(Type providerType)
    {
      MemberInfo memberInfo1 = (MemberInfo) providerType.GetStaticProperty("Instance");
      if ((object) memberInfo1 == null)
        memberInfo1 = (MemberInfo) providerType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      MemberInfo memberInfo2 = memberInfo1;
      if (memberInfo2 == (MemberInfo) null)
        throw new InvalidOperationException(Strings.EF6Providers_InstanceMissing((object) providerType.AssemblyQualifiedName));
      DbProviderServices providerServices = memberInfo2.GetValue() as DbProviderServices;
      if (providerServices == null)
        throw new InvalidOperationException(Strings.EF6Providers_NotDbProviderServices((object) providerType.AssemblyQualifiedName));
      return providerServices;
    }
  }
}
