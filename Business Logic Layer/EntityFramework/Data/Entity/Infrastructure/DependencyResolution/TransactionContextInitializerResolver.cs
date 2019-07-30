// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.TransactionContextInitializerResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class TransactionContextInitializerResolver : IDbDependencyResolver
  {
    private readonly ConcurrentDictionary<Type, object> _initializers = new ConcurrentDictionary<Type, object>();

    public object GetService(Type type, object key)
    {
      Check.NotNull<Type>(type, nameof (type));
      Type elementType = type.TryGetElementType(typeof (IDatabaseInitializer<>));
      if (elementType != (Type) null && typeof (TransactionContext).IsAssignableFrom(elementType))
        return this._initializers.GetOrAdd(elementType, new Func<Type, object>(this.CreateInitializerInstance));
      return (object) null;
    }

    private object CreateInitializerInstance(Type type)
    {
      return Activator.CreateInstance(typeof (TransactionContextInitializer<>).MakeGenericType(type));
    }

    public IEnumerable<object> GetServices(Type type, object key)
    {
      return this.GetServiceAsServices(type, key);
    }
  }
}
