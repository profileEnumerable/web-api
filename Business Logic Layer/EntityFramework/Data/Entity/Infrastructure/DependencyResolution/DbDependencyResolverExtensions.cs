// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.DbDependencyResolverExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  /// <summary>
  /// Extension methods to call the <see cref="M:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver.GetService(System.Type,System.Object)" /> method using
  /// a generic type parameter and/or no name.
  /// </summary>
  public static class DbDependencyResolverExtensions
  {
    /// <summary>
    /// Calls <see cref="M:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver.GetService(System.Type,System.Object)" /> passing the generic type of the method and the given
    /// name as arguments.
    /// </summary>
    /// <typeparam name="T"> The contract type to resolve. </typeparam>
    /// <param name="resolver"> The resolver to use. </param>
    /// <param name="key"> The key of the dependency to resolve. </param>
    /// <returns> The resolved dependency, or null if the resolver could not resolve it. </returns>
    public static T GetService<T>(this IDbDependencyResolver resolver, object key)
    {
      Check.NotNull<IDbDependencyResolver>(resolver, nameof (resolver));
      return (T) resolver.GetService(typeof (T), key);
    }

    /// <summary>
    /// Calls <see cref="M:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver.GetService(System.Type,System.Object)" /> passing the generic type of the method as
    /// the type argument and null for the name argument.
    /// </summary>
    /// <typeparam name="T"> The contract type to resolve. </typeparam>
    /// <param name="resolver"> The resolver to use. </param>
    /// <returns> The resolved dependency, or null if the resolver could not resolve it. </returns>
    public static T GetService<T>(this IDbDependencyResolver resolver)
    {
      Check.NotNull<IDbDependencyResolver>(resolver, nameof (resolver));
      return (T) resolver.GetService(typeof (T), (object) null);
    }

    /// <summary>
    /// Calls <see cref="M:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver.GetService(System.Type,System.Object)" /> passing the given type argument and using
    /// null for the name argument.
    /// </summary>
    /// <param name="resolver"> The resolver to use. </param>
    /// <param name="type"> The contract type to resolve. </param>
    /// <returns> The resolved dependency, or null if the resolver could not resolve it. </returns>
    public static object GetService(this IDbDependencyResolver resolver, Type type)
    {
      Check.NotNull<IDbDependencyResolver>(resolver, nameof (resolver));
      Check.NotNull<Type>(type, nameof (type));
      return resolver.GetService(type, (object) null);
    }

    /// <summary>
    /// Calls <see cref="M:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver.GetServices(System.Type,System.Object)" /> passing the generic type of the method and the given
    /// name as arguments.
    /// </summary>
    /// <typeparam name="T"> The contract type to resolve. </typeparam>
    /// <param name="resolver"> The resolver to use. </param>
    /// <param name="key"> The key of the dependency to resolve. </param>
    /// <returns> All resolved dependencies, or an <see cref="T:System.Collections.Generic.IEnumerable`1" /> if no services are resolved.</returns>
    public static IEnumerable<T> GetServices<T>(
      this IDbDependencyResolver resolver,
      object key)
    {
      Check.NotNull<IDbDependencyResolver>(resolver, nameof (resolver));
      return resolver.GetServices(typeof (T), key).OfType<T>();
    }

    /// <summary>
    /// Calls <see cref="M:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver.GetServices(System.Type,System.Object)" /> passing the generic type of the method as
    /// the type argument and null for the name argument.
    /// </summary>
    /// <typeparam name="T"> The contract type to resolve. </typeparam>
    /// <param name="resolver"> The resolver to use. </param>
    /// <returns> All resolved dependencies, or an <see cref="T:System.Collections.Generic.IEnumerable`1" /> if no services are resolved.</returns>
    public static IEnumerable<T> GetServices<T>(this IDbDependencyResolver resolver)
    {
      Check.NotNull<IDbDependencyResolver>(resolver, nameof (resolver));
      return resolver.GetServices(typeof (T), (object) null).OfType<T>();
    }

    /// <summary>
    /// Calls <see cref="M:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver.GetServices(System.Type,System.Object)" /> passing the given type argument and using
    /// null for the name argument.
    /// </summary>
    /// <param name="resolver"> The resolver to use. </param>
    /// <param name="type"> The contract type to resolve. </param>
    /// <returns> All resolved dependencies, or an <see cref="T:System.Collections.Generic.IEnumerable`1" /> if no services are resolved.</returns>
    public static IEnumerable<object> GetServices(
      this IDbDependencyResolver resolver,
      Type type)
    {
      Check.NotNull<IDbDependencyResolver>(resolver, nameof (resolver));
      Check.NotNull<Type>(type, nameof (type));
      return resolver.GetServices(type, (object) null);
    }

    internal static IEnumerable<object> GetServiceAsServices(
      this IDbDependencyResolver resolver,
      Type type,
      object key)
    {
      object service = resolver.GetService(type, key);
      if (service == null)
        return Enumerable.Empty<object>();
      return (IEnumerable<object>) new object[1]
      {
        service
      };
    }
  }
}
