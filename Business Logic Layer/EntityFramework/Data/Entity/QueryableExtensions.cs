// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.QueryableExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal;
using System.Data.Entity.Internal.Linq;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity
{
  /// <summary>
  /// Useful extension methods for use with Entity Framework LINQ queries.
  /// </summary>
  public static class QueryableExtensions
  {
    private static readonly MethodInfo _first = QueryableExtensions.GetMethod("First", (Func<Type, Type[]>) (T => new Type[1]
    {
      typeof (IQueryable<>).MakeGenericType(T)
    }));
    private static readonly MethodInfo _first_Predicate = QueryableExtensions.GetMethod("First", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (bool)))
    }));
    private static readonly MethodInfo _firstOrDefault = QueryableExtensions.GetMethod("FirstOrDefault", (Func<Type, Type[]>) (T => new Type[1]
    {
      typeof (IQueryable<>).MakeGenericType(T)
    }));
    private static readonly MethodInfo _firstOrDefault_Predicate = QueryableExtensions.GetMethod("FirstOrDefault", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (bool)))
    }));
    private static readonly MethodInfo _single = QueryableExtensions.GetMethod("Single", (Func<Type, Type[]>) (T => new Type[1]
    {
      typeof (IQueryable<>).MakeGenericType(T)
    }));
    private static readonly MethodInfo _single_Predicate = QueryableExtensions.GetMethod("Single", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (bool)))
    }));
    private static readonly MethodInfo _singleOrDefault = QueryableExtensions.GetMethod("SingleOrDefault", (Func<Type, Type[]>) (T => new Type[1]
    {
      typeof (IQueryable<>).MakeGenericType(T)
    }));
    private static readonly MethodInfo _singleOrDefault_Predicate = QueryableExtensions.GetMethod("SingleOrDefault", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (bool)))
    }));
    private static readonly MethodInfo _contains = QueryableExtensions.GetMethod("Contains", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      T
    }));
    private static readonly MethodInfo _any = QueryableExtensions.GetMethod("Any", (Func<Type, Type[]>) (T => new Type[1]
    {
      typeof (IQueryable<>).MakeGenericType(T)
    }));
    private static readonly MethodInfo _any_Predicate = QueryableExtensions.GetMethod("Any", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (bool)))
    }));
    private static readonly MethodInfo _all_Predicate = QueryableExtensions.GetMethod("All", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (bool)))
    }));
    private static readonly MethodInfo _count = QueryableExtensions.GetMethod("Count", (Func<Type, Type[]>) (T => new Type[1]
    {
      typeof (IQueryable<>).MakeGenericType(T)
    }));
    private static readonly MethodInfo _count_Predicate = QueryableExtensions.GetMethod("Count", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (bool)))
    }));
    private static readonly MethodInfo _longCount = QueryableExtensions.GetMethod("LongCount", (Func<Type, Type[]>) (T => new Type[1]
    {
      typeof (IQueryable<>).MakeGenericType(T)
    }));
    private static readonly MethodInfo _longCount_Predicate = QueryableExtensions.GetMethod("LongCount", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (bool)))
    }));
    private static readonly MethodInfo _min = QueryableExtensions.GetMethod("Min", (Func<Type, Type[]>) (T => new Type[1]
    {
      typeof (IQueryable<>).MakeGenericType(T)
    }));
    private static readonly MethodInfo _min_Selector = QueryableExtensions.GetMethod("Min", (Func<Type, Type, Type[]>) ((T, U) => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, U))
    }));
    private static readonly MethodInfo _max = QueryableExtensions.GetMethod("Max", (Func<Type, Type[]>) (T => new Type[1]
    {
      typeof (IQueryable<>).MakeGenericType(T)
    }));
    private static readonly MethodInfo _max_Selector = QueryableExtensions.GetMethod("Max", (Func<Type, Type, Type[]>) ((T, U) => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, U))
    }));
    private static readonly MethodInfo _sum_Int = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<int>)
    }));
    private static readonly MethodInfo _sum_IntNullable = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<int?>)
    }));
    private static readonly MethodInfo _sum_Long = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<long>)
    }));
    private static readonly MethodInfo _sum_LongNullable = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<long?>)
    }));
    private static readonly MethodInfo _sum_Float = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<float>)
    }));
    private static readonly MethodInfo _sum_FloatNullable = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<float?>)
    }));
    private static readonly MethodInfo _sum_Double = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<double>)
    }));
    private static readonly MethodInfo _sum_DoubleNullable = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<double?>)
    }));
    private static readonly MethodInfo _sum_Decimal = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<Decimal>)
    }));
    private static readonly MethodInfo _sum_DecimalNullable = QueryableExtensions.GetMethod("Sum", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<Decimal?>)
    }));
    private static readonly MethodInfo _sum_Int_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (int)))
    }));
    private static readonly MethodInfo _sum_IntNullable_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (int?)))
    }));
    private static readonly MethodInfo _sum_Long_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (long)))
    }));
    private static readonly MethodInfo _sum_LongNullable_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (long?)))
    }));
    private static readonly MethodInfo _sum_Float_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (float)))
    }));
    private static readonly MethodInfo _sum_FloatNullable_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (float?)))
    }));
    private static readonly MethodInfo _sum_Double_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (double)))
    }));
    private static readonly MethodInfo _sum_DoubleNullable_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (double?)))
    }));
    private static readonly MethodInfo _sum_Decimal_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (Decimal)))
    }));
    private static readonly MethodInfo _sum_DecimalNullable_Selector = QueryableExtensions.GetMethod("Sum", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (Decimal?)))
    }));
    private static readonly MethodInfo _average_Int = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<int>)
    }));
    private static readonly MethodInfo _average_IntNullable = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<int?>)
    }));
    private static readonly MethodInfo _average_Long = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<long>)
    }));
    private static readonly MethodInfo _average_LongNullable = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<long?>)
    }));
    private static readonly MethodInfo _average_Float = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<float>)
    }));
    private static readonly MethodInfo _average_FloatNullable = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<float?>)
    }));
    private static readonly MethodInfo _average_Double = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<double>)
    }));
    private static readonly MethodInfo _average_DoubleNullable = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<double?>)
    }));
    private static readonly MethodInfo _average_Decimal = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<Decimal>)
    }));
    private static readonly MethodInfo _average_DecimalNullable = QueryableExtensions.GetMethod("Average", (Func<Type[]>) (() => new Type[1]
    {
      typeof (IQueryable<Decimal?>)
    }));
    private static readonly MethodInfo _average_Int_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (int)))
    }));
    private static readonly MethodInfo _average_IntNullable_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (int?)))
    }));
    private static readonly MethodInfo _average_Long_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (long)))
    }));
    private static readonly MethodInfo _average_LongNullable_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (long?)))
    }));
    private static readonly MethodInfo _average_Float_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (float)))
    }));
    private static readonly MethodInfo _average_FloatNullable_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (float?)))
    }));
    private static readonly MethodInfo _average_Double_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (double)))
    }));
    private static readonly MethodInfo _average_DoubleNullable_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (double?)))
    }));
    private static readonly MethodInfo _average_Decimal_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (Decimal)))
    }));
    private static readonly MethodInfo _average_DecimalNullable_Selector = QueryableExtensions.GetMethod("Average", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(T, typeof (Decimal?)))
    }));
    private static readonly MethodInfo _skip = QueryableExtensions.GetMethod("Skip", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (int)
    }));
    private static readonly MethodInfo _take = QueryableExtensions.GetMethod("Take", (Func<Type, Type[]>) (T => new Type[2]
    {
      typeof (IQueryable<>).MakeGenericType(T),
      typeof (int)
    }));

    /// <summary>
    /// Specifies the related objects to include in the query results.
    /// </summary>
    /// <remarks>
    /// This extension method calls the Include(String) method of the source <see cref="T:System.Linq.IQueryable`1" /> object,
    /// if such a method exists. If the source <see cref="T:System.Linq.IQueryable`1" /> does not have a matching method,
    /// then this method does nothing. The <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" />, <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" />,
    /// <see cref="T:System.Data.Entity.Infrastructure.DbQuery`1" /> and <see cref="T:System.Data.Entity.DbSet`1" /> types all have an appropriate Include method to call.
    /// Paths are all-inclusive. For example, if an include call indicates Include("Orders.OrderLines"), not only will
    /// OrderLines be included, but also Orders.  When you call the Include method, the query path is only valid on
    /// the returned instance of the <see cref="T:System.Linq.IQueryable`1" />. Other instances of <see cref="T:System.Linq.IQueryable`1" />
    /// and the object context itself are not affected. Because the Include method returns the query object,
    /// you can call this method multiple times on an <see cref="T:System.Linq.IQueryable`1" /> to specify multiple paths for the query.
    /// </remarks>
    /// <typeparam name="T"> The type of entity being queried. </typeparam>
    /// <param name="source">
    /// The source <see cref="T:System.Linq.IQueryable`1" /> on which to call Include.
    /// </param>
    /// <param name="path"> The dot-separated list of related objects to return in the query results. </param>
    /// <returns>
    /// A new <see cref="T:System.Linq.IQueryable`1" /> with the defined query path.
    /// </returns>
    public static IQueryable<T> Include<T>(this IQueryable<T> source, string path)
    {
      Check.NotNull<IQueryable<T>>(source, nameof (source));
      Check.NotEmpty(path, nameof (path));
      DbQuery<T> dbQuery = source as DbQuery<T>;
      if (dbQuery != null)
        return (IQueryable<T>) dbQuery.Include(path);
      ObjectQuery<T> objectQuery = source as ObjectQuery<T>;
      if (objectQuery != null)
        return (IQueryable<T>) objectQuery.Include(path);
      return QueryableExtensions.CommonInclude<IQueryable<T>>(source, path);
    }

    /// <summary>
    /// Specifies the related objects to include in the query results.
    /// </summary>
    /// <remarks>
    /// This extension method calls the Include(String) method of the source <see cref="T:System.Linq.IQueryable" /> object,
    /// if such a method exists. If the source <see cref="T:System.Linq.IQueryable" /> does not have a matching method,
    /// then this method does nothing. The <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery" />, <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" />,
    /// <see cref="T:System.Data.Entity.Infrastructure.DbQuery" /> and <see cref="T:System.Data.Entity.DbSet" /> types all have an appropriate Include method to call.
    /// Paths are all-inclusive. For example, if an include call indicates Include("Orders.OrderLines"), not only will
    /// OrderLines be included, but also Orders.  When you call the Include method, the query path is only valid on
    /// the returned instance of the <see cref="T:System.Linq.IQueryable" />. Other instances of <see cref="T:System.Linq.IQueryable" />
    /// and the object context itself are not affected. Because the Include method returns the query object,
    /// you can call this method multiple times on an <see cref="T:System.Linq.IQueryable" /> to specify multiple paths for the query.
    /// </remarks>
    /// <param name="source">
    /// The source <see cref="T:System.Linq.IQueryable" /> on which to call Include.
    /// </param>
    /// <param name="path"> The dot-separated list of related objects to return in the query results. </param>
    /// <returns>
    /// A new <see cref="T:System.Linq.IQueryable" /> with the defined query path.
    /// </returns>
    public static IQueryable Include(this IQueryable source, string path)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      Check.NotEmpty(path, nameof (path));
      DbQuery dbQuery = source as DbQuery;
      if (dbQuery == null)
        return QueryableExtensions.CommonInclude<IQueryable>(source, path);
      return (IQueryable) dbQuery.Include(path);
    }

    private static T CommonInclude<T>(T source, string path)
    {
      MethodInfo runtimeMethod = source.GetType().GetRuntimeMethod("Include", (Func<MethodInfo, bool>) (p =>
      {
        if (p.IsPublic)
          return !p.IsStatic;
        return false;
      }), new Type[1]{ typeof (string) }, new Type[1]
      {
        typeof (IComparable)
      }, new Type[1]{ typeof (ICloneable) }, new Type[1]
      {
        typeof (IComparable<string>)
      }, new Type[1]{ typeof (IEnumerable<char>) }, new Type[1]
      {
        typeof (IEnumerable)
      }, new Type[1]{ typeof (IEquatable<string>) }, new Type[1]
      {
        typeof (object)
      });
      if (!(runtimeMethod != (MethodInfo) null) || !typeof (T).IsAssignableFrom(runtimeMethod.ReturnType))
        return source;
      return (T) runtimeMethod.Invoke((object) source, new object[1]
      {
        (object) path
      });
    }

    /// <summary>
    /// Specifies the related objects to include in the query results.
    /// </summary>
    /// <remarks>
    /// The path expression must be composed of simple property access expressions together with calls to Select for
    /// composing additional includes after including a collection proprty.  Examples of possible include paths are:
    /// To include a single reference: query.Include(e =&gt; e.Level1Reference)
    /// To include a single collection: query.Include(e =&gt; e.Level1Collection)
    /// To include a reference and then a reference one level down: query.Include(e =&gt; e.Level1Reference.Level2Reference)
    /// To include a reference and then a collection one level down: query.Include(e =&gt; e.Level1Reference.Level2Collection)
    /// To include a collection and then a reference one level down: query.Include(e =&gt; e.Level1Collection.Select(l1 =&gt; l1.Level2Reference))
    /// To include a collection and then a collection one level down: query.Include(e =&gt; e.Level1Collection.Select(l1 =&gt; l1.Level2Collection))
    /// To include a collection and then a reference one level down: query.Include(e =&gt; e.Level1Collection.Select(l1 =&gt; l1.Level2Reference))
    /// To include a collection and then a collection one level down: query.Include(e =&gt; e.Level1Collection.Select(l1 =&gt; l1.Level2Collection))
    /// To include a collection, a reference, and a reference two levels down: query.Include(e =&gt; e.Level1Collection.Select(l1 =&gt; l1.Level2Reference.Level3Reference))
    /// To include a collection, a collection, and a reference two levels down: query.Include(e =&gt; e.Level1Collection.Select(l1 =&gt; l1.Level2Collection.Select(l2 =&gt; l2.Level3Reference)))
    /// This extension method calls the Include(String) method of the source IQueryable object, if such a method exists.
    /// If the source IQueryable does not have a matching method, then this method does nothing.
    /// The Entity Framework ObjectQuery, ObjectSet, DbQuery, and DbSet types all have an appropriate Include method to call.
    /// When you call the Include method, the query path is only valid on the returned instance of the IQueryable&lt;T&gt;. Other
    /// instances of IQueryable&lt;T&gt; and the object context itself are not affected.  Because the Include method returns the
    /// query object, you can call this method multiple times on an IQueryable&lt;T&gt; to specify multiple paths for the query.
    /// </remarks>
    /// <typeparam name="T"> The type of entity being queried. </typeparam>
    /// <typeparam name="TProperty"> The type of navigation property being included. </typeparam>
    /// <param name="source"> The source IQueryable on which to call Include. </param>
    /// <param name="path"> A lambda expression representing the path to include. </param>
    /// <returns>
    /// A new IQueryable&lt;T&gt; with the defined query path.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static IQueryable<T> Include<T, TProperty>(
      this IQueryable<T> source,
      Expression<Func<T, TProperty>> path)
    {
      Check.NotNull<IQueryable<T>>(source, nameof (source));
      Check.NotNull<Expression<Func<T, TProperty>>>(path, nameof (path));
      string path1;
      if (!DbHelpers.TryParsePath(path.Body, out path1) || path1 == null)
        throw new ArgumentException(Strings.DbExtensions_InvalidIncludePathExpression, nameof (path));
      return source.Include<T>(path1);
    }

    /// <summary>
    /// Returns a new query where the entities returned will not be cached in the <see cref="T:System.Data.Entity.DbContext" />
    /// or <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.  This method works by calling the AsNoTracking method of the
    /// underlying query object.  If the underlying query object does not have an AsNoTracking method,
    /// then calling this method will have no affect.
    /// </summary>
    /// <typeparam name="T"> The element type. </typeparam>
    /// <param name="source"> The source query. </param>
    /// <returns> A new query with NoTracking applied, or the source query if NoTracking is not supported. </returns>
    public static IQueryable<T> AsNoTracking<T>(this IQueryable<T> source) where T : class
    {
      Check.NotNull<IQueryable<T>>(source, nameof (source));
      DbQuery<T> dbQuery = source as DbQuery<T>;
      if (dbQuery == null)
        return QueryableExtensions.CommonAsNoTracking<IQueryable<T>>(source);
      return (IQueryable<T>) dbQuery.AsNoTracking();
    }

    /// <summary>
    /// Returns a new query where the entities returned will not be cached in the <see cref="T:System.Data.Entity.DbContext" />
    /// or <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.  This method works by calling the AsNoTracking method of the
    /// underlying query object.  If the underlying query object does not have an AsNoTracking method,
    /// then calling this method will have no affect.
    /// </summary>
    /// <param name="source"> The source query. </param>
    /// <returns> A new query with NoTracking applied, or the source query if NoTracking is not supported. </returns>
    public static IQueryable AsNoTracking(this IQueryable source)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      DbQuery dbQuery = source as DbQuery;
      if (dbQuery == null)
        return QueryableExtensions.CommonAsNoTracking<IQueryable>(source);
      return (IQueryable) dbQuery.AsNoTracking();
    }

    private static T CommonAsNoTracking<T>(T source) where T : class
    {
      ObjectQuery query = (object) source as ObjectQuery;
      if (query != null)
        return (T) DbHelpers.CreateNoTrackingQuery(query);
      MethodInfo publicInstanceMethod = source.GetType().GetPublicInstanceMethod("AsNoTracking");
      if (publicInstanceMethod != (MethodInfo) null && typeof (T).IsAssignableFrom(publicInstanceMethod.ReturnType))
        return (T) publicInstanceMethod.Invoke((object) source, (object[]) null);
      return source;
    }

    /// <summary>
    /// Returns a new query that will stream the results instead of buffering. This method works by calling
    /// the AsStreaming method of the underlying query object. If the underlying query object does not have
    /// an AsStreaming method, then calling this method will have no affect.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to apply AsStreaming to.
    /// </param>
    /// <returns> A new query with AsStreaming applied, or the source query if AsStreaming is not supported. </returns>
    [Obsolete("LINQ queries are now streaming by default unless a retrying ExecutionStrategy is used. Calling this method will have no effect.")]
    public static IQueryable<T> AsStreaming<T>(this IQueryable<T> source)
    {
      Check.NotNull<IQueryable<T>>(source, nameof (source));
      DbQuery<T> dbQuery = source as DbQuery<T>;
      if (dbQuery == null)
        return QueryableExtensions.CommonAsStreaming<IQueryable<T>>(source);
      return (IQueryable<T>) dbQuery.AsStreaming();
    }

    /// <summary>
    /// Returns a new query that will stream the results instead of buffering. This method works by calling
    /// the AsStreaming method of the underlying query object. If the underlying query object does not have
    /// an AsStreaming method, then calling this method will have no affect.
    /// </summary>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable" /> to apply AsStreaming to.
    /// </param>
    /// <returns> A new query with AsStreaming applied, or the source query if AsStreaming is not supported. </returns>
    [Obsolete("LINQ queries are now streaming by default unless a retrying ExecutionStrategy is used. Calling this method will have no effect.")]
    public static IQueryable AsStreaming(this IQueryable source)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      DbQuery dbQuery = source as DbQuery;
      if (dbQuery == null)
        return QueryableExtensions.CommonAsStreaming<IQueryable>(source);
      return (IQueryable) dbQuery.AsStreaming();
    }

    private static T CommonAsStreaming<T>(T source) where T : class
    {
      ObjectQuery query = (object) source as ObjectQuery;
      if (query != null)
        return (T) DbHelpers.CreateStreamingQuery(query);
      MethodInfo publicInstanceMethod = source.GetType().GetPublicInstanceMethod("AsStreaming");
      if (publicInstanceMethod != (MethodInfo) null && typeof (T).IsAssignableFrom(publicInstanceMethod.ReturnType))
        return (T) publicInstanceMethod.Invoke((object) source, (object[]) null);
      return source;
    }

    internal static IQueryable<T> WithExecutionStrategy<T>(
      this IQueryable<T> source,
      IDbExecutionStrategy executionStrategy)
    {
      Check.NotNull<IQueryable<T>>(source, nameof (source));
      DbQuery<T> dbQuery = source as DbQuery<T>;
      if (dbQuery == null)
        return QueryableExtensions.CommonWithExecutionStrategy<IQueryable<T>>(source, executionStrategy);
      return (IQueryable<T>) dbQuery.WithExecutionStrategy(executionStrategy);
    }

    internal static IQueryable WithExecutionStrategy(
      this IQueryable source,
      IDbExecutionStrategy executionStrategy)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      DbQuery dbQuery = source as DbQuery;
      if (dbQuery == null)
        return QueryableExtensions.CommonWithExecutionStrategy<IQueryable>(source, executionStrategy);
      return (IQueryable) dbQuery.WithExecutionStrategy(executionStrategy);
    }

    private static T CommonWithExecutionStrategy<T>(
      T source,
      IDbExecutionStrategy executionStrategy)
      where T : class
    {
      ObjectQuery query = (object) source as ObjectQuery;
      if (query != null)
        return (T) DbHelpers.CreateQueryWithExecutionStrategy(query, executionStrategy);
      MethodInfo publicInstanceMethod = source.GetType().GetPublicInstanceMethod("WithExecutionStrategy");
      if (!(publicInstanceMethod != (MethodInfo) null) || !typeof (T).IsAssignableFrom(publicInstanceMethod.ReturnType))
        return source;
      return (T) publicInstanceMethod.Invoke((object) source, new object[1]
      {
        (object) executionStrategy
      });
    }

    /// <summary>
    /// Enumerates the query such that for server queries such as those of <see cref="T:System.Data.Entity.DbSet`1" />,
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" />
    /// ,
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" />, and others the results of the query will be loaded into the associated
    /// <see cref="T:System.Data.Entity.DbContext" />
    /// ,
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> or other cache on the client.
    /// This is equivalent to calling ToList and then throwing away the list without the overhead of actually creating the list.
    /// </summary>
    /// <param name="source"> The source query. </param>
    public static void Load(this IQueryable source)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      IEnumerator enumerator = source.GetEnumerator();
      try
      {
        do
          ;
        while (enumerator.MoveNext());
      }
      finally
      {
        (enumerator as IDisposable)?.Dispose();
      }
    }

    /// <summary>
    /// Asynchronously enumerates the query such that for server queries such as those of <see cref="T:System.Data.Entity.DbSet`1" />,
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" />
    /// ,
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" />, and others the results of the query will be loaded into the associated
    /// <see cref="T:System.Data.Entity.DbContext" />
    /// ,
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> or other cache on the client.
    /// This is equivalent to calling ToList and then throwing away the list without the overhead of actually creating the list.
    /// </summary>
    /// <param name="source"> The source query. </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task LoadAsync(this IQueryable source)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      return source.LoadAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously enumerates the query such that for server queries such as those of <see cref="T:System.Data.Entity.DbSet`1" />,
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" />
    /// ,
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" />, and others the results of the query will be loaded into the associated
    /// <see cref="T:System.Data.Entity.DbContext" />
    /// ,
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> or other cache on the client.
    /// This is equivalent to calling ToList and then throwing away the list without the overhead of actually creating the list.
    /// </summary>
    /// <param name="source"> The source query. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task LoadAsync(this IQueryable source, CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      return source.ForEachAsync((Action<object>) (e => {}), cancellationToken);
    }

    /// <summary>
    /// Asynchronously enumerates the query results and performs the specified action on each element.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable" /> to enumerate.
    /// </param>
    /// <param name="action"> The action to perform on each element. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public static Task ForEachAsync(this IQueryable source, Action<object> action)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      Check.NotNull<Action<object>>(action, nameof (action));
      return source.AsDbAsyncEnumerable().ForEachAsync(action, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously enumerates the query results and performs the specified action on each element.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable" /> to enumerate.
    /// </param>
    /// <param name="action"> The action to perform on each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public static Task ForEachAsync(
      this IQueryable source,
      Action<object> action,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      Check.NotNull<Action<object>>(action, nameof (action));
      return source.AsDbAsyncEnumerable().ForEachAsync(action, cancellationToken);
    }

    /// <summary>
    /// Asynchronously enumerates the query results and performs the specified action on each element.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to enumerate.
    /// </param>
    /// <param name="action"> The action to perform on each element. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public static Task ForEachAsync<T>(this IQueryable<T> source, Action<T> action)
    {
      Check.NotNull<IQueryable<T>>(source, nameof (source));
      Check.NotNull<Action<T>>(action, nameof (action));
      return source.AsDbAsyncEnumerable<T>().ForEachAsync<T>(action, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously enumerates the query results and performs the specified action on each element.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to enumerate.
    /// </param>
    /// <param name="action"> The action to perform on each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public static Task ForEachAsync<T>(
      this IQueryable<T> source,
      Action<T> action,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<T>>(source, nameof (source));
      Check.NotNull<Action<T>>(action, nameof (action));
      return source.AsDbAsyncEnumerable<T>().ForEachAsync<T>(action, cancellationToken);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.List`1" /> from an <see cref="T:System.Linq.IQueryable" /> by enumerating it asynchronously.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable" /> to create a <see cref="T:System.Collections.Generic.List`1" /> from.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.List`1" /> that contains elements from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<List<object>> ToListAsync(this IQueryable source)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      return source.AsDbAsyncEnumerable().ToListAsync<object>();
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.List`1" /> from an <see cref="T:System.Linq.IQueryable" /> by enumerating it asynchronously.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable" /> to create a <see cref="T:System.Collections.Generic.List`1" /> from.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.List`1" /> that contains elements from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<List<object>> ToListAsync(
      this IQueryable source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable>(source, nameof (source));
      return source.AsDbAsyncEnumerable().ToListAsync<object>(cancellationToken);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.List`1" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.List`1" /> from.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.List`1" /> that contains elements from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return IDbAsyncEnumerableExtensions.ToListAsync<TSource>(source.AsDbAsyncEnumerable<TSource>());
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.List`1" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a list from.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.List`1" /> that contains elements from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<List<TSource>> ToListAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return IDbAsyncEnumerableExtensions.ToListAsync<TSource>(source.AsDbAsyncEnumerable<TSource>(), cancellationToken);
    }

    /// <summary>
    /// Creates an array from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create an array from.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an array that contains elements from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.AsDbAsyncEnumerable<TSource>().ToArrayAsync<TSource>();
    }

    /// <summary>
    /// Creates an array from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create an array from.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an array that contains elements from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TSource[]> ToArrayAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.AsDbAsyncEnumerable<TSource>().ToArrayAsync<TSource>(cancellationToken);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously
    /// according to a specified key selector function.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key returned by <paramref name="keySelector" /> .
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
    /// </param>
    /// <param name="keySelector"> A function to extract a key from each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains selected keys and values.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Func<TSource, TKey>>(keySelector, nameof (keySelector));
      return source.AsDbAsyncEnumerable<TSource>().ToDictionaryAsync<TSource, TKey>(keySelector);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously
    /// according to a specified key selector function.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key returned by <paramref name="keySelector" /> .
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
    /// </param>
    /// <param name="keySelector"> A function to extract a key from each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains selected keys and values.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Func<TSource, TKey>>(keySelector, nameof (keySelector));
      return source.AsDbAsyncEnumerable<TSource>().ToDictionaryAsync<TSource, TKey>(keySelector, cancellationToken);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously
    /// according to a specified key selector function and a comparer.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key returned by <paramref name="keySelector" /> .
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
    /// </param>
    /// <param name="keySelector"> A function to extract a key from each element. </param>
    /// <param name="comparer">
    /// An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains selected keys and values.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Func<TSource, TKey>>(keySelector, nameof (keySelector));
      return source.AsDbAsyncEnumerable<TSource>().ToDictionaryAsync<TSource, TKey>(keySelector, comparer);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously
    /// according to a specified key selector function and a comparer.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key returned by <paramref name="keySelector" /> .
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
    /// </param>
    /// <param name="keySelector"> A function to extract a key from each element. </param>
    /// <param name="comparer">
    /// An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains selected keys and values.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      IEqualityComparer<TKey> comparer,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Func<TSource, TKey>>(keySelector, nameof (keySelector));
      return source.AsDbAsyncEnumerable<TSource>().ToDictionaryAsync<TSource, TKey>(keySelector, comparer, cancellationToken);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously
    /// according to a specified key selector and an element selector function.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key returned by <paramref name="keySelector" /> .
    /// </typeparam>
    /// <typeparam name="TElement">
    /// The type of the value returned by <paramref name="elementSelector" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
    /// </param>
    /// <param name="keySelector"> A function to extract a key from each element. </param>
    /// <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type
    /// <typeparamref name="TElement" /> selected from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Func<TSource, TKey>>(keySelector, nameof (keySelector));
      Check.NotNull<Func<TSource, TElement>>(elementSelector, nameof (elementSelector));
      return source.AsDbAsyncEnumerable<TSource>().ToDictionaryAsync<TSource, TKey, TElement>(keySelector, elementSelector);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously
    /// according to a specified key selector and an element selector function.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key returned by <paramref name="keySelector" /> .
    /// </typeparam>
    /// <typeparam name="TElement">
    /// The type of the value returned by <paramref name="elementSelector" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
    /// </param>
    /// <param name="keySelector"> A function to extract a key from each element. </param>
    /// <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type
    /// <typeparamref name="TElement" /> selected from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Func<TSource, TKey>>(keySelector, nameof (keySelector));
      Check.NotNull<Func<TSource, TElement>>(elementSelector, nameof (elementSelector));
      return source.AsDbAsyncEnumerable<TSource>().ToDictionaryAsync<TSource, TKey, TElement>(keySelector, elementSelector, cancellationToken);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously
    /// according to a specified key selector function, a comparer, and an element selector function.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key returned by <paramref name="keySelector" /> .
    /// </typeparam>
    /// <typeparam name="TElement">
    /// The type of the value returned by <paramref name="elementSelector" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
    /// </param>
    /// <param name="keySelector"> A function to extract a key from each element. </param>
    /// <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
    /// <param name="comparer">
    /// An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type
    /// <typeparamref name="TElement" /> selected from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      IEqualityComparer<TKey> comparer)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Func<TSource, TKey>>(keySelector, nameof (keySelector));
      Check.NotNull<Func<TSource, TElement>>(elementSelector, nameof (elementSelector));
      return source.AsDbAsyncEnumerable<TSource>().ToDictionaryAsync<TSource, TKey, TElement>(keySelector, elementSelector, comparer);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously
    /// according to a specified key selector function, a comparer, and an element selector function.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key returned by <paramref name="keySelector" /> .
    /// </typeparam>
    /// <typeparam name="TElement">
    /// The type of the value returned by <paramref name="elementSelector" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
    /// </param>
    /// <param name="keySelector"> A function to extract a key from each element. </param>
    /// <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
    /// <param name="comparer">
    /// An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type
    /// <typeparamref name="TElement" /> selected from the input sequence.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IQueryable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      IEqualityComparer<TKey> comparer,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Func<TSource, TKey>>(keySelector, nameof (keySelector));
      Check.NotNull<Func<TSource, TElement>>(elementSelector, nameof (elementSelector));
      return source.AsDbAsyncEnumerable<TSource>().ToDictionaryAsync<TSource, TKey, TElement>(keySelector, elementSelector, comparer, cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the first element of a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the first element in <paramref name="source" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" /> doesn't implement <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
    public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.FirstAsync<TSource>(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the first element of a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the first element in <paramref name="source" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
    public static Task<TSource> FirstAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._first.MakeGenericMethod(typeof (TSource)), source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the first element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the first element in <paramref name="source" /> that passes the test in
    /// <paramref name="predicate" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// No element satisfies the condition in
    /// <paramref name="predicate" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TSource> FirstAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      return source.FirstAsync<TSource>(predicate, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the first element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the first element in <paramref name="source" /> that passes the test in
    /// <paramref name="predicate" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// No element satisfies the condition in
    /// <paramref name="predicate" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TSource> FirstAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._first_Predicate.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) predicate)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>default</c> ( <typeparamref name="TSource" /> ) if
    /// <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.FirstOrDefaultAsync<TSource>(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>default</c> ( <typeparamref name="TSource" /> ) if
    /// <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<TSource> FirstOrDefaultAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._firstOrDefault.MakeGenericMethod(typeof (TSource)), source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the first element of a sequence that satisfies a specified condition
    /// or a default value if no such element is found.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>default</c> ( <typeparamref name="TSource" /> ) if <paramref name="source" />
    /// is empty or if no element passes the test specified by <paramref name="predicate" /> ; otherwise, the first
    /// element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TSource> FirstOrDefaultAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      return source.FirstOrDefaultAsync<TSource>(predicate, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the first element of a sequence that satisfies a specified condition
    /// or a default value if no such element is found.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>default</c> ( <typeparamref name="TSource" /> ) if <paramref name="source" />
    /// is empty or if no element passes the test specified by <paramref name="predicate" /> ; otherwise, the first
    /// element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// has more than one element.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TSource> FirstOrDefaultAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._firstOrDefault_Predicate.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) predicate)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the only element of a sequence, and throws an exception
    /// if there is not exactly one element in the sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single element of the input sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
    public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.SingleAsync<TSource>(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the only element of a sequence, and throws an exception
    /// if there is not exactly one element in the sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single element of the input sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// has more than one element.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
    public static Task<TSource> SingleAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._single.MakeGenericMethod(typeof (TSource)), source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the only element of a sequence that satisfies a specified condition,
    /// and throws an exception if more than one such element exists.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the the single element of.
    /// </param>
    /// <param name="predicate"> A function to test an element for a condition. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single element of the input sequence that satisfies the condition in
    /// <paramref name="predicate" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// No element satisfies the condition in
    /// <paramref name="predicate" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// More than one element satisfies the condition in
    /// <paramref name="predicate" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TSource> SingleAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      return source.SingleAsync<TSource>(predicate, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the only element of a sequence that satisfies a specified condition,
    /// and throws an exception if more than one such element exists.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
    /// </param>
    /// <param name="predicate"> A function to test an element for a condition. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single element of the input sequence that satisfies the condition in
    /// <paramref name="predicate" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// No element satisfies the condition in
    /// <paramref name="predicate" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// More than one element satisfies the condition in
    /// <paramref name="predicate" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TSource> SingleAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._single_Predicate.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) predicate)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the only element of a sequence, or a default value if the sequence is empty;
    /// this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single element of the input sequence, or <c>default</c> (<typeparamref name="TSource" />)
    /// if the sequence contains no elements.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// has more than one element.
    /// </exception>
    public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.SingleOrDefaultAsync<TSource>(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the only element of a sequence, or a default value if the sequence is empty;
    /// this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single element of the input sequence, or <c>default</c> (<typeparamref name="TSource" />)
    /// if the sequence contains no elements.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// has more than one element.
    /// </exception>
    public static Task<TSource> SingleOrDefaultAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._singleOrDefault.MakeGenericMethod(typeof (TSource)), source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the only element of a sequence that satisfies a specified condition or
    /// a default value if no such element exists; this method throws an exception if more than one element
    /// satisfies the condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
    /// </param>
    /// <param name="predicate"> A function to test an element for a condition. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single element of the input sequence that satisfies the condition in
    /// <paramref name="predicate" />, or <c>default</c> ( <typeparamref name="TSource" /> ) if no such element is found.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TSource> SingleOrDefaultAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      return source.SingleOrDefaultAsync<TSource>(predicate, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the only element of a sequence that satisfies a specified condition or
    /// a default value if no such element exists; this method throws an exception if more than one element
    /// satisfies the condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
    /// </param>
    /// <param name="predicate"> A function to test an element for a condition. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single element of the input sequence that satisfies the condition in
    /// <paramref name="predicate" />, or <c>default</c> ( <typeparamref name="TSource" /> ) if no such element is found.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<TSource> SingleOrDefaultAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._singleOrDefault_Predicate.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) predicate)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously determines whether a sequence contains a specified element by using the default equality comparer.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
    /// </param>
    /// <param name="item"> The object to locate in the sequence. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>true</c> if the input sequence contains the specified value; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<bool> ContainsAsync<TSource>(
      this IQueryable<TSource> source,
      TSource item)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.ContainsAsync<TSource>(item, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously determines whether a sequence contains a specified element by using the default equality comparer.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
    /// </param>
    /// <param name="item"> The object to locate in the sequence. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>true</c> if the input sequence contains the specified value; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<bool> ContainsAsync<TSource>(
      this IQueryable<TSource> source,
      TSource item,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<bool>((Expression) Expression.Call((Expression) null, QueryableExtensions._contains.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Constant((object) item, typeof (TSource))
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously determines whether a sequence contains any elements.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to check for being empty.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>true</c> if the source sequence contains any elements; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.AnyAsync<TSource>(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously determines whether a sequence contains any elements.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> to check for being empty.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>true</c> if the source sequence contains any elements; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<bool> AnyAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<bool>((Expression) Expression.Call((Expression) null, QueryableExtensions._any.MakeGenericMethod(typeof (TSource)), source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously determines whether any element of a sequence satisfies a condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> whose elements to test for a condition.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>true</c> if any elements in the source sequence pass the test in the specified predicate; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<bool> AnyAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      return source.AnyAsync<TSource>(predicate, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously determines whether any element of a sequence satisfies a condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> whose elements to test for a condition.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>true</c> if any elements in the source sequence pass the test in the specified predicate; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<bool> AnyAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<bool>((Expression) Expression.Call((Expression) null, QueryableExtensions._any_Predicate.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) predicate)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously determines whether all the elements of a sequence satisfy a condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> whose elements to test for a condition.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>true</c> if every element of the source sequence passes the test in the specified predicate; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<bool> AllAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      return source.AllAsync<TSource>(predicate, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously determines whether all the elements of a sequence satisfy a condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> whose elements to test for a condition.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>true</c> if every element of the source sequence passes the test in the specified predicate; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<bool> AllAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<bool>((Expression) Expression.Call((Expression) null, QueryableExtensions._all_Predicate.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) predicate)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the number of elements in a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of elements in the input sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.CountAsync<TSource>(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the number of elements in a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of elements in the input sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    public static Task<int> CountAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<int>((Expression) Expression.Call((Expression) null, QueryableExtensions._count.MakeGenericMethod(typeof (TSource)), source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the number of elements in a sequence that satisfy a condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of elements in the sequence that satisfy the condition in the predicate function.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// that satisfy the condition in the predicate function
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<int> CountAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      return source.CountAsync<TSource>(predicate, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the number of elements in a sequence that satisfy a condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of elements in the sequence that satisfy the condition in the predicate function.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// that satisfy the condition in the predicate function
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<int> CountAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<int>((Expression) Expression.Call((Expression) null, QueryableExtensions._count_Predicate.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) predicate)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns an <see cref="T:System.Int64" /> that represents the total number of elements in a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of elements in the input sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.LongCountAsync<TSource>(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns an <see cref="T:System.Int64" /> that represents the total number of elements in a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of elements in the input sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    public static Task<long> LongCountAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<long>((Expression) Expression.Call((Expression) null, QueryableExtensions._longCount.MakeGenericMethod(typeof (TSource)), source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns an <see cref="T:System.Int64" /> that represents the number of elements in a sequence
    /// that satisfy a condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of elements in the sequence that satisfy the condition in the predicate function.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// that satisfy the condition in the predicate function
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<long> LongCountAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      return source.LongCountAsync<TSource>(predicate, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns an <see cref="T:System.Int64" /> that represents the number of elements in a sequence
    /// that satisfy a condition.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of elements in the sequence that satisfy the condition in the predicate function.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="predicate" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// that satisfy the condition in the predicate function
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<long> LongCountAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> predicate,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, bool>>>(predicate, nameof (predicate));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<long>((Expression) Expression.Call((Expression) null, QueryableExtensions._longCount_Predicate.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) predicate)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the minimum value of a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the minimum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the minimum value in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<TSource> MinAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.MinAsync<TSource>(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the minimum value of a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the minimum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the minimum value in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<TSource> MinAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._min.MakeGenericMethod(typeof (TSource)), source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously invokes a projection function on each element of a sequence and returns the minimum resulting value.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the value returned by the function represented by <paramref name="selector" /> .
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the minimum of.
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the minimum value in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TResult> MinAsync<TSource, TResult>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, TResult>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, TResult>>>(selector, nameof (selector));
      return source.MinAsync<TSource, TResult>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously invokes a projection function on each element of a sequence and returns the minimum resulting value.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the value returned by the function represented by <paramref name="selector" /> .
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the minimum of.
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the minimum value in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<TResult> MinAsync<TSource, TResult>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, TResult>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, TResult>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TResult>((Expression) Expression.Call((Expression) null, QueryableExtensions._min_Selector.MakeGenericMethod(typeof (TSource), typeof (TResult)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously returns the maximum value of a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the maximum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the maximum value in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<TSource> MaxAsync<TSource>(this IQueryable<TSource> source)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      return source.MaxAsync<TSource>(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously returns the maximum value of a sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the maximum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the maximum value in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<TSource> MaxAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._max.MakeGenericMethod(typeof (TSource)), source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously invokes a projection function on each element of a sequence and returns the maximum resulting value.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the value returned by the function represented by <paramref name="selector" /> .
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the maximum of.
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the maximum value in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TResult> MaxAsync<TSource, TResult>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, TResult>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, TResult>>>(selector, nameof (selector));
      return source.MaxAsync<TSource, TResult>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously invokes a projection function on each element of a sequence and returns the maximum resulting value.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the value returned by the function represented by <paramref name="selector" /> .
    /// </typeparam>
    /// <param name="source">
    /// An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the maximum of.
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the maximum value in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<TResult> MaxAsync<TSource, TResult>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, TResult>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, TResult>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<TResult>((Expression) Expression.Call((Expression) null, QueryableExtensions._max_Selector.MakeGenericMethod(typeof (TSource), typeof (TResult)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Int32" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Int32" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains  the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    public static Task<int> SumAsync(this IQueryable<int> source)
    {
      Check.NotNull<IQueryable<int>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Int32" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Int32" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    public static Task<int> SumAsync(
      this IQueryable<int> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<int>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<int>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Int, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Int32" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Int32" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<int?> SumAsync(this IQueryable<int?> source)
    {
      Check.NotNull<IQueryable<int?>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Int32" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Int32" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<int?> SumAsync(
      this IQueryable<int?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<int?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<int?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_IntNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Int64" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Int64" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    public static Task<long> SumAsync(this IQueryable<long> source)
    {
      Check.NotNull<IQueryable<long>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Int64" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Int64" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    public static Task<long> SumAsync(
      this IQueryable<long> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<long>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<long>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Long, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Int64" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Int64" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<long?> SumAsync(this IQueryable<long?> source)
    {
      Check.NotNull<IQueryable<long?>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Int64" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Int64" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<long?> SumAsync(
      this IQueryable<long?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<long?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<long?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_LongNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Single" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Single" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<float> SumAsync(this IQueryable<float> source)
    {
      Check.NotNull<IQueryable<float>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Single" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Single" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<float> SumAsync(
      this IQueryable<float> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<float>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<float>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Float, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Single" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Single" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float?> SumAsync(this IQueryable<float?> source)
    {
      Check.NotNull<IQueryable<float?>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Single" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Single" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float?> SumAsync(
      this IQueryable<float?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<float?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<float?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_FloatNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Double" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Double" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<double> SumAsync(this IQueryable<double> source)
    {
      Check.NotNull<IQueryable<double>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Double" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Double" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<double> SumAsync(
      this IQueryable<double> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<double>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Double, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Double" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Double" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> SumAsync(this IQueryable<double?> source)
    {
      Check.NotNull<IQueryable<double?>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Double" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Double" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> SumAsync(
      this IQueryable<double?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<double?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_DoubleNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Decimal" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Decimal" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<Decimal> SumAsync(this IQueryable<Decimal> source)
    {
      Check.NotNull<IQueryable<Decimal>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of <see cref="T:System.Decimal" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Decimal" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    public static Task<Decimal> SumAsync(
      this IQueryable<Decimal> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<Decimal>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<Decimal>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Decimal, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Decimal" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Decimal" /> values to calculate the sum of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal?> SumAsync(this IQueryable<Decimal?> source)
    {
      Check.NotNull<IQueryable<Decimal?>>(source, nameof (source));
      return source.SumAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of a sequence of nullable <see cref="T:System.Decimal" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Decimal" /> values to calculate the sum of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the values in the sequence.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Decimal.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal?> SumAsync(
      this IQueryable<Decimal?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<Decimal?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<Decimal?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_DecimalNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Int32" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<int> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, int>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, int>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Int32" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<int> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, int>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, int>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<int>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Int_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Int32" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<int?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, int?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, int?>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Int32" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int32.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<int?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, int?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, int?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<int?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_IntNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Int64" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<long> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, long>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Int64" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<long> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, long>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<long>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Long_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Int64" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<long?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, long?>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Int64" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Int64.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<long?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, long?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<long?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_LongNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Single" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, float>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, float>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Single" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<float> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, float>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, float>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<float>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Float_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Single" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, float?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, float?>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Single" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, float?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, float?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<float?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_FloatNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Double" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, double>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, double>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Double" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, double>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, double>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Double_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Double" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, double?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, double?>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Double" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, double?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, double?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_DoubleNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Decimal" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Decimal.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, Decimal>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, Decimal>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="T:System.Decimal" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Decimal.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<Decimal> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, Decimal>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, Decimal>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<Decimal>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_Decimal_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Decimal" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Decimal.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, Decimal?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, Decimal?>>>(selector, nameof (selector));
      return source.SumAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the sum of the sequence of nullable <see cref="T:System.Decimal" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source">
    /// A sequence of values of type <typeparamref name="TSource" /> .
    /// </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the sum of the projected values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    /// The number of elements in
    /// <paramref name="source" />
    /// is larger than
    /// <see cref="F:System.Decimal.MaxValue" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal?> SumAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, Decimal?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, Decimal?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<Decimal?>((Expression) Expression.Call((Expression) null, QueryableExtensions._sum_DecimalNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Int32" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Int32" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<double> AverageAsync(this IQueryable<int> source)
    {
      Check.NotNull<IQueryable<int>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Int32" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Int32" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<double> AverageAsync(
      this IQueryable<int> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<int>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Int, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Int32" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Int32" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync(this IQueryable<int?> source)
    {
      Check.NotNull<IQueryable<int?>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Int32" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Int32" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync(
      this IQueryable<int?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<int?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_IntNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Int64" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Int64" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<double> AverageAsync(this IQueryable<long> source)
    {
      Check.NotNull<IQueryable<long>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Int64" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Int64" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<double> AverageAsync(
      this IQueryable<long> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<long>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Long, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Int64" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Int64" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync(this IQueryable<long?> source)
    {
      Check.NotNull<IQueryable<long?>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Int64" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Int64" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync(
      this IQueryable<long?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<long?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_LongNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Single" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Single" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<float> AverageAsync(this IQueryable<float> source)
    {
      Check.NotNull<IQueryable<float>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Single" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Single" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<float> AverageAsync(
      this IQueryable<float> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<float>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<float>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Float, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Single" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Single" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float?> AverageAsync(this IQueryable<float?> source)
    {
      Check.NotNull<IQueryable<float?>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Single" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Single" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float?> AverageAsync(
      this IQueryable<float?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<float?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<float?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_FloatNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Double" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Double" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<double> AverageAsync(this IQueryable<double> source)
    {
      Check.NotNull<IQueryable<double>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Double" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Double" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<double> AverageAsync(
      this IQueryable<double> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<double>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Double, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Double" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Double" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync(this IQueryable<double?> source)
    {
      Check.NotNull<IQueryable<double?>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Double" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Double" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync(
      this IQueryable<double?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<double?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_DoubleNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Decimal" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Decimal" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<Decimal> AverageAsync(this IQueryable<Decimal> source)
    {
      Check.NotNull<IQueryable<Decimal>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Decimal" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of <see cref="T:System.Decimal" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    public static Task<Decimal> AverageAsync(
      this IQueryable<Decimal> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<Decimal>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<Decimal>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Decimal, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Decimal" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Decimal" /> values to calculate the average of.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal?> AverageAsync(this IQueryable<Decimal?> source)
    {
      Check.NotNull<IQueryable<Decimal?>>(source, nameof (source));
      return source.AverageAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Decimal" /> values.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="source">
    /// A sequence of nullable <see cref="T:System.Decimal" /> values to calculate the average of.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal?> AverageAsync(
      this IQueryable<Decimal?> source,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<Decimal?>>(source, nameof (source));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<Decimal?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_DecimalNullable, source.Expression), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Int32" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, int>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, int>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Int32" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, int>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, int>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Int_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Int32" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, int?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, int?>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Int32" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<double?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, int?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, int?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_IntNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Int64" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, long>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Int64" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<double> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, long>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Long_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Int64" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, long?>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Int64" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, long?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, long?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_LongNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Single" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, float>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, float>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Single" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, float>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, float>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<float>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Float_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Single" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, float?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, float?>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Single" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<float?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, float?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, float?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<float?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_FloatNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Double" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, double>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, double>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Double" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, double>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, double>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Double_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Double" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<double?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, double?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, double?>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Double" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static Task<double?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, double?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, double?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<double?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_DoubleNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Decimal" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, Decimal>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, Decimal>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="T:System.Decimal" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// contains no elements.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, Decimal>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, Decimal>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<Decimal>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_Decimal_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Decimal" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, Decimal?>> selector)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, Decimal?>>>(selector, nameof (selector));
      return source.AverageAsync<TSource>(selector, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously computes the average of a sequence of nullable <see cref="T:System.Decimal" /> values that is obtained
    /// by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    /// The type of the elements of <paramref name="source" /> .
    /// </typeparam>
    /// <param name="source"> A sequence of values to calculate the average of. </param>
    /// <param name="selector"> A projection function to apply to each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the average of the sequence of values.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" />
    /// or
    /// <paramref name="selector" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// <paramref name="source" />
    /// doesn't implement
    /// <see cref="T:System.Data.Entity.Infrastructure.IDbAsyncQueryProvider" />
    /// .
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Task<Decimal?> AverageAsync<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, Decimal?>> selector,
      CancellationToken cancellationToken)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<TSource, Decimal?>>>(selector, nameof (selector));
      cancellationToken.ThrowIfCancellationRequested();
      IDbAsyncQueryProvider provider = source.Provider as IDbAsyncQueryProvider;
      if (provider == null)
        throw Error.IQueryable_Provider_Not_Async();
      return provider.ExecuteAsync<Decimal?>((Expression) Expression.Call((Expression) null, QueryableExtensions._average_DecimalNullable_Selector.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Quote((Expression) selector)
      }), cancellationToken);
    }

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">A sequence to return elements from.</param>
    /// <param name="countAccessor">An expression that evaluates to the number of elements to skip.</param>
    /// <returns>A sequence that contains elements that occur after the specified index in the
    /// input sequence.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static IQueryable<TSource> Skip<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<int>> countAccessor)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<int>>>(countAccessor, nameof (countAccessor));
      return source.Provider.CreateQuery<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._skip.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        countAccessor.Body
      }));
    }

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="countAccessor">An expression that evaluates to the number of elements
    /// to return.</param>
    /// <returns>A sequence that contains the specified number of elements from the
    /// start of the input sequence.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static IQueryable<TSource> Take<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<int>> countAccessor)
    {
      Check.NotNull<IQueryable<TSource>>(source, nameof (source));
      Check.NotNull<Expression<Func<int>>>(countAccessor, nameof (countAccessor));
      return source.Provider.CreateQuery<TSource>((Expression) Expression.Call((Expression) null, QueryableExtensions._take.MakeGenericMethod(typeof (TSource)), new Expression[2]
      {
        source.Expression,
        countAccessor.Body
      }));
    }

    internal static ObjectQuery TryGetObjectQuery(this IQueryable source)
    {
      if (source == null)
        return (ObjectQuery) null;
      ObjectQuery objectQuery = source as ObjectQuery;
      if (objectQuery != null)
        return objectQuery;
      return (source as IInternalQueryAdapter)?.InternalQuery.ObjectQuery;
    }

    private static IDbAsyncEnumerable AsDbAsyncEnumerable(this IQueryable source)
    {
      IDbAsyncEnumerable dbAsyncEnumerable = source as IDbAsyncEnumerable;
      if (dbAsyncEnumerable != null)
        return dbAsyncEnumerable;
      throw Error.IQueryable_Not_Async((object) string.Empty);
    }

    private static IDbAsyncEnumerable<T> AsDbAsyncEnumerable<T>(
      this IQueryable<T> source)
    {
      IDbAsyncEnumerable<T> dbAsyncEnumerable = source as IDbAsyncEnumerable<T>;
      if (dbAsyncEnumerable != null)
        return dbAsyncEnumerable;
      throw Error.IQueryable_Not_Async((object) ("<" + (object) typeof (T) + ">"));
    }

    private static MethodInfo GetMethod(string methodName, Func<Type[]> getParameterTypes)
    {
      return QueryableExtensions.GetMethod(methodName, (Delegate) getParameterTypes, 0);
    }

    private static MethodInfo GetMethod(
      string methodName,
      Func<Type, Type, Type[]> getParameterTypes)
    {
      return QueryableExtensions.GetMethod(methodName, (Delegate) getParameterTypes, 2);
    }

    private static MethodInfo GetMethod(
      string methodName,
      Func<Type, Type[]> getParameterTypes)
    {
      return QueryableExtensions.GetMethod(methodName, (Delegate) getParameterTypes, 1);
    }

    private static MethodInfo GetMethod(
      string methodName,
      Delegate getParameterTypesDelegate,
      int genericArgumentsCount)
    {
      foreach (MethodInfo declaredMethod in typeof (Queryable).GetDeclaredMethods(methodName))
      {
        Type[] genericArguments = declaredMethod.GetGenericArguments();
        if (genericArguments.Length == genericArgumentsCount && QueryableExtensions.Matches(declaredMethod, (Type[]) getParameterTypesDelegate.DynamicInvoke((object[]) genericArguments)))
          return declaredMethod;
      }
      return (MethodInfo) null;
    }

    private static bool Matches(MethodInfo methodInfo, Type[] parameterTypes)
    {
      return ((IEnumerable<ParameterInfo>) methodInfo.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).SequenceEqual<Type>((IEnumerable<Type>) parameterTypes);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called from an assert")]
    private static string PrettyPrint(MethodInfo getParameterTypesMethod, int genericArgumentsCount)
    {
      Type[] typeArray1 = new Type[genericArgumentsCount];
      for (int index = 0; index < genericArgumentsCount; ++index)
        typeArray1[index] = typeof (object);
      Type[] typeArray2 = (Type[]) getParameterTypesMethod.Invoke((object) null, (object[]) typeArray1);
      string[] strArray = new string[typeArray2.Length];
      for (int index = 0; index < typeArray2.Length; ++index)
        strArray[index] = typeArray2[index].ToString();
      return "(" + string.Join(", ", strArray) + ")";
    }
  }
}
