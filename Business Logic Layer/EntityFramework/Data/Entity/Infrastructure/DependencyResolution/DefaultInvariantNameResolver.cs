// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.DefaultInvariantNameResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class DefaultInvariantNameResolver : IDbDependencyResolver
  {
    public virtual object GetService(Type type, object key)
    {
      if (!(type == typeof (IProviderInvariantName)))
        return (object) null;
      DbProviderFactory factory = key as DbProviderFactory;
      if (factory == null)
        throw new ArgumentException(Strings.DbDependencyResolver_InvalidKey((object) typeof (DbProviderFactory).Name, (object) typeof (IProviderInvariantName)));
      return (object) new ProviderInvariantName(factory.GetProviderInvariantName());
    }

    public IEnumerable<object> GetServices(Type type, object key)
    {
      return this.GetServiceAsServices(type, key);
    }
  }
}
