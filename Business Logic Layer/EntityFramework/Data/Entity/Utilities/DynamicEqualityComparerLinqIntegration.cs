// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DynamicEqualityComparerLinqIntegration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Utilities
{
  internal static class DynamicEqualityComparerLinqIntegration
  {
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public static IEnumerable<T> Distinct<T>(
      this IEnumerable<T> source,
      Func<T, T, bool> func)
      where T : class
    {
      return source.Distinct<T>((IEqualityComparer<T>) new DynamicEqualityComparer<T>(func));
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public static IEnumerable<IGrouping<TSource, TSource>> GroupBy<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, TSource, bool> func)
      where TSource : class
    {
      return source.GroupBy<TSource, TSource>((Func<TSource, TSource>) (t => t), (IEqualityComparer<TSource>) new DynamicEqualityComparer<TSource>(func));
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public static IEnumerable<T> Intersect<T>(
      this IEnumerable<T> first,
      IEnumerable<T> second,
      Func<T, T, bool> func)
      where T : class
    {
      return first.Intersect<T>(second, (IEqualityComparer<T>) new DynamicEqualityComparer<T>(func));
    }

    public static IEnumerable<T> Except<T>(
      this IEnumerable<T> first,
      IEnumerable<T> second,
      Func<T, T, bool> func)
      where T : class
    {
      return first.Except<T>(second, (IEqualityComparer<T>) new DynamicEqualityComparer<T>(func));
    }

    public static bool Contains<T>(this IEnumerable<T> source, T value, Func<T, T, bool> func) where T : class
    {
      return source.Contains<T>(value, (IEqualityComparer<T>) new DynamicEqualityComparer<T>(func));
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public static bool SequenceEqual<TSource>(
      this IEnumerable<TSource> source,
      IEnumerable<TSource> other,
      Func<TSource, TSource, bool> func)
      where TSource : class
    {
      return source.SequenceEqual<TSource>(other, (IEqualityComparer<TSource>) new DynamicEqualityComparer<TSource>(func));
    }
  }
}
