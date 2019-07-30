// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.DefaultProviderServicesResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class DefaultProviderServicesResolver : IDbDependencyResolver
  {
    public virtual object GetService(Type type, object key)
    {
      if (type == typeof (DbProviderServices))
        throw new InvalidOperationException(Strings.EF6Providers_NoProviderFound((object) DefaultProviderServicesResolver.CheckKey(key)));
      return (object) null;
    }

    private static string CheckKey(object key)
    {
      string str = key as string;
      if (string.IsNullOrWhiteSpace(str))
        throw new ArgumentException(Strings.DbDependencyResolver_NoProviderInvariantName((object) typeof (DbProviderServices).Name));
      return str;
    }

    public virtual IEnumerable<object> GetServices(Type type, object key)
    {
      if (type == typeof (DbProviderServices))
        DefaultProviderServicesResolver.CheckKey(key);
      return Enumerable.Empty<object>();
    }
  }
}
