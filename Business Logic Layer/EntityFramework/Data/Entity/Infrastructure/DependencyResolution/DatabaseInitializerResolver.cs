// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.DatabaseInitializerResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class DatabaseInitializerResolver : IDbDependencyResolver
  {
    private readonly ConcurrentDictionary<Type, object> _initializers = new ConcurrentDictionary<Type, object>();

    public virtual object GetService(Type type, object key)
    {
      Type elementType = type.TryGetElementType(typeof (IDatabaseInitializer<>));
      object obj;
      if (elementType != (Type) null && this._initializers.TryGetValue(elementType, out obj))
        return obj;
      return (object) null;
    }

    public virtual void SetInitializer(Type contextType, object initializer)
    {
      this._initializers.AddOrUpdate(contextType, initializer, (Func<Type, object, object>) ((c, i) => initializer));
    }

    public IEnumerable<object> GetServices(Type type, object key)
    {
      return this.GetServiceAsServices(type, key);
    }
  }
}
