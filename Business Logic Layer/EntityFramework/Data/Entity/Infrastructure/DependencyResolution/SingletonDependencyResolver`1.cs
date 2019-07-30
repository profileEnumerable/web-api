// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.SingletonDependencyResolver`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  /// <summary>
  /// Implements <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver" /> to resolve a dependency such that it always returns
  /// the same instance.
  /// </summary>
  /// <typeparam name="T"> The type that defines the contract for the dependency that will be resolved. </typeparam>
  /// <remarks>
  /// This class is immutable such that instances can be accessed by multiple threads at the same time.
  /// </remarks>
  public class SingletonDependencyResolver<T> : IDbDependencyResolver where T : class
  {
    private readonly T _singletonInstance;
    private readonly Func<object, bool> _keyPredicate;

    /// <summary>
    /// Constructs a new resolver that will return the given instance for the contract type
    /// regardless of the key passed to the Get method.
    /// </summary>
    /// <param name="singletonInstance"> The instance to return. </param>
    public SingletonDependencyResolver(T singletonInstance)
      : this(singletonInstance, (object) null)
    {
    }

    /// <summary>
    /// Constructs a new resolver that will return the given instance for the contract type
    /// if the given key matches exactly the key passed to the Get method.
    /// </summary>
    /// <param name="singletonInstance"> The instance to return. </param>
    /// <param name="key"> Optionally, the key of the dependency to be resolved. This may be null for dependencies that are not differentiated by key. </param>
    public SingletonDependencyResolver(T singletonInstance, object key)
    {
      Check.NotNull<T>(singletonInstance, nameof (singletonInstance));
      this._singletonInstance = singletonInstance;
      this._keyPredicate = (Func<object, bool>) (k =>
      {
        if (key != null)
          return object.Equals(key, k);
        return true;
      });
    }

    /// <summary>
    /// Constructs a new resolver that will return the given instance for the contract type
    /// if the given key matches the key passed to the Get method based on the given predicate.
    /// </summary>
    /// <param name="singletonInstance"> The instance to return. </param>
    /// <param name="keyPredicate"> A predicate that takes the key object and returns true if and only if it matches. </param>
    public SingletonDependencyResolver(T singletonInstance, Func<object, bool> keyPredicate)
    {
      Check.NotNull<T>(singletonInstance, nameof (singletonInstance));
      Check.NotNull<Func<object, bool>>(keyPredicate, nameof (keyPredicate));
      this._singletonInstance = singletonInstance;
      this._keyPredicate = keyPredicate;
    }

    /// <inheritdoc />
    public object GetService(Type type, object key)
    {
      return (object) (!(type == typeof (T)) || !this._keyPredicate(key) ? default (T) : this._singletonInstance);
    }

    /// <inheritdoc />
    public IEnumerable<object> GetServices(Type type, object key)
    {
      return this.GetServiceAsServices(type, key);
    }
  }
}
