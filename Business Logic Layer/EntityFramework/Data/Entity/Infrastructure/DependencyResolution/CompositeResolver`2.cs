// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.CompositeResolver`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class CompositeResolver<TFirst, TSecond> : IDbDependencyResolver
    where TFirst : class, IDbDependencyResolver
    where TSecond : class, IDbDependencyResolver
  {
    private readonly TFirst _firstResolver;
    private readonly TSecond _secondResolver;

    public CompositeResolver(TFirst firstResolver, TSecond secondResolver)
    {
      this._firstResolver = firstResolver;
      this._secondResolver = secondResolver;
    }

    public TFirst First
    {
      get
      {
        return this._firstResolver;
      }
    }

    public TSecond Second
    {
      get
      {
        return this._secondResolver;
      }
    }

    public virtual object GetService(Type type, object key)
    {
      return this._firstResolver.GetService(type, key) ?? this._secondResolver.GetService(type, key);
    }

    public IEnumerable<object> GetServices(Type type, object key)
    {
      return this._firstResolver.GetServices(type, key).Concat<object>(this._secondResolver.GetServices(type, key));
    }
  }
}
