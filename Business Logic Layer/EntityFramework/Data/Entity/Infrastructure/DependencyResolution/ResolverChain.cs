// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.ResolverChain
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class ResolverChain : IDbDependencyResolver
  {
    private readonly ConcurrentStack<IDbDependencyResolver> _resolvers = new ConcurrentStack<IDbDependencyResolver>();
    private volatile IDbDependencyResolver[] _resolversSnapshot = new IDbDependencyResolver[0];

    public virtual void Add(IDbDependencyResolver resolver)
    {
      Check.NotNull<IDbDependencyResolver>(resolver, nameof (resolver));
      this._resolvers.Push(resolver);
      this._resolversSnapshot = this._resolvers.ToArray();
    }

    public virtual IEnumerable<IDbDependencyResolver> Resolvers
    {
      get
      {
        return ((IEnumerable<IDbDependencyResolver>) this._resolversSnapshot).Reverse<IDbDependencyResolver>();
      }
    }

    public virtual object GetService(Type type, object key)
    {
      return ((IEnumerable<IDbDependencyResolver>) this._resolversSnapshot).Select<IDbDependencyResolver, object>((Func<IDbDependencyResolver, object>) (r => r.GetService(type, key))).FirstOrDefault<object>((Func<object, bool>) (s => s != null));
    }

    public virtual IEnumerable<object> GetServices(Type type, object key)
    {
      return ((IEnumerable<IDbDependencyResolver>) this._resolversSnapshot).SelectMany<IDbDependencyResolver, object>((Func<IDbDependencyResolver, IEnumerable<object>>) (r => r.GetServices(type, key)));
    }
  }
}
