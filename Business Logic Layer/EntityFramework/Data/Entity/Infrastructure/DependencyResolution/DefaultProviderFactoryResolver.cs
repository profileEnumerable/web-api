// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.DefaultProviderFactoryResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class DefaultProviderFactoryResolver : IDbDependencyResolver
  {
    public virtual object GetService(Type type, object key)
    {
      return DefaultProviderFactoryResolver.GetService(type, key, (Func<ArgumentException, string, object>) ((e, n) =>
      {
        throw new ArgumentException(Strings.EntityClient_InvalidStoreProvider((object) n), (Exception) e);
      }));
    }

    private static object GetService(
      Type type,
      object key,
      Func<ArgumentException, string, object> handleFailedLookup)
    {
      if (!(type == typeof (DbProviderFactory)))
        return (object) null;
      string providerInvariantName = key as string;
      if (string.IsNullOrWhiteSpace(providerInvariantName))
        throw new ArgumentException(Strings.DbDependencyResolver_NoProviderInvariantName((object) typeof (DbProviderFactory).Name));
      try
      {
        return (object) DbProviderFactories.GetFactory(providerInvariantName);
      }
      catch (ArgumentException ex)
      {
        return handleFailedLookup(ex, providerInvariantName);
      }
    }

    public IEnumerable<object> GetServices(Type type, object key)
    {
      object service = DefaultProviderFactoryResolver.GetService(type, key, (Func<ArgumentException, string, object>) ((e, n) => (object) null));
      if (service == null)
        return Enumerable.Empty<object>();
      return (IEnumerable<object>) new object[1]
      {
        service
      };
    }
  }
}
