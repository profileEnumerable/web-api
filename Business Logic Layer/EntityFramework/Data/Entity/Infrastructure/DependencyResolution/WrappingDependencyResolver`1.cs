// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.WrappingDependencyResolver`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class WrappingDependencyResolver<TService> : IDbDependencyResolver
  {
    private readonly IDbDependencyResolver _snapshot;
    private readonly Func<TService, object, TService> _serviceWrapper;

    public WrappingDependencyResolver(
      IDbDependencyResolver snapshot,
      Func<TService, object, TService> serviceWrapper)
    {
      this._snapshot = snapshot;
      this._serviceWrapper = serviceWrapper;
    }

    public object GetService(Type type, object key)
    {
      if (!(type == typeof (TService)))
        return (object) null;
      return (object) this._serviceWrapper(this._snapshot.GetService<TService>(key), key);
    }

    public IEnumerable<object> GetServices(Type type, object key)
    {
      if (!(type == typeof (TService)))
        return Enumerable.Empty<object>();
      return (IEnumerable<object>) this._snapshot.GetServices<TService>(key).Select<TService, TService>((Func<TService, TService>) (s => this._serviceWrapper(s, key)));
    }
  }
}
