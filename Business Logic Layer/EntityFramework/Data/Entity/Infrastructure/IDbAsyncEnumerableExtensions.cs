// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IDbAsyncEnumerableExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure
{
  internal static class IDbAsyncEnumerableExtensions
  {
    internal static async Task ForEachAsync(
      this IDbAsyncEnumerable source,
      Action<object> action,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      using (IDbAsyncEnumerator enumerator = source.GetAsyncEnumerator())
      {
        if (!await enumerator.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          return;
        Task<bool> moveNextTask;
        do
        {
          cancellationToken.ThrowIfCancellationRequested();
          object current = enumerator.Current;
          moveNextTask = enumerator.MoveNextAsync(cancellationToken);
          action(current);
        }
        while (await moveNextTask.WithCurrentCulture<bool>());
      }
    }

    internal static Task ForEachAsync<T>(
      this IDbAsyncEnumerable<T> source,
      Action<T> action,
      CancellationToken cancellationToken)
    {
      return IDbAsyncEnumerableExtensions.ForEachAsync<T>(source.GetAsyncEnumerator(), action, cancellationToken);
    }

    private static async Task ForEachAsync<T>(
      IDbAsyncEnumerator<T> enumerator,
      Action<T> action,
      CancellationToken cancellationToken)
    {
      using (enumerator)
      {
        cancellationToken.ThrowIfCancellationRequested();
        if (!await enumerator.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          return;
        Task<bool> moveNextTask;
        do
        {
          cancellationToken.ThrowIfCancellationRequested();
          T current = enumerator.Current;
          moveNextTask = enumerator.MoveNextAsync(cancellationToken);
          action(current);
        }
        while (await moveNextTask.WithCurrentCulture<bool>());
      }
    }

    internal static Task<List<T>> ToListAsync<T>(this IDbAsyncEnumerable source)
    {
      return source.ToListAsync<T>(CancellationToken.None);
    }

    internal static async Task<List<T>> ToListAsync<T>(
      this IDbAsyncEnumerable source,
      CancellationToken cancellationToken)
    {
      List<T> list = new List<T>();
      await source.ForEachAsync((Action<object>) (e => list.Add((T) e)), cancellationToken).WithCurrentCulture();
      return list;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<List<T>> ToListAsync<T>(this IDbAsyncEnumerable<T> source)
    {
      return IDbAsyncEnumerableExtensions.ToListAsync<T>(source, CancellationToken.None);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<List<T>> ToListAsync<T>(
      this IDbAsyncEnumerable<T> source,
      CancellationToken cancellationToken)
    {
      TaskCompletionSource<List<T>> tcs = new TaskCompletionSource<List<T>>();
      List<T> list = new List<T>();
      source.ForEachAsync<T>(new Action<T>(list.Add), cancellationToken).ContinueWith((Action<Task>) (t =>
      {
        if (t.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) t.Exception.InnerExceptions);
        else if (t.IsCanceled)
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult(list);
      }), TaskContinuationOptions.ExecuteSynchronously);
      return tcs.Task;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<T[]> ToArrayAsync<T>(this IDbAsyncEnumerable<T> source)
    {
      return source.ToArrayAsync<T>(CancellationToken.None);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static async Task<T[]> ToArrayAsync<T>(
      this IDbAsyncEnumerable<T> source,
      CancellationToken cancellationToken)
    {
      List<T> list = await IDbAsyncEnumerableExtensions.ToListAsync<T>(source, cancellationToken).WithCurrentCulture<List<T>>();
      return list.ToArray();
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      return source.ToDictionaryAsync<TSource, TKey, TSource>(keySelector, IDbAsyncEnumerableExtensions.IdentityFunction<TSource>.Instance, (IEqualityComparer<TKey>) null, CancellationToken.None);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      CancellationToken cancellationToken)
    {
      return source.ToDictionaryAsync<TSource, TKey, TSource>(keySelector, IDbAsyncEnumerableExtensions.IdentityFunction<TSource>.Instance, (IEqualityComparer<TKey>) null, cancellationToken);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      return source.ToDictionaryAsync<TSource, TKey, TSource>(keySelector, IDbAsyncEnumerableExtensions.IdentityFunction<TSource>.Instance, comparer, CancellationToken.None);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      IEqualityComparer<TKey> comparer,
      CancellationToken cancellationToken)
    {
      return source.ToDictionaryAsync<TSource, TKey, TSource>(keySelector, IDbAsyncEnumerableExtensions.IdentityFunction<TSource>.Instance, comparer, cancellationToken);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector)
    {
      return source.ToDictionaryAsync<TSource, TKey, TElement>(keySelector, elementSelector, (IEqualityComparer<TKey>) null, CancellationToken.None);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      CancellationToken cancellationToken)
    {
      return source.ToDictionaryAsync<TSource, TKey, TElement>(keySelector, elementSelector, (IEqualityComparer<TKey>) null, cancellationToken);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      IEqualityComparer<TKey> comparer)
    {
      return source.ToDictionaryAsync<TSource, TKey, TElement>(keySelector, elementSelector, comparer, CancellationToken.None);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    internal static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      IEqualityComparer<TKey> comparer,
      CancellationToken cancellationToken)
    {
      Dictionary<TKey, TElement> d = new Dictionary<TKey, TElement>(comparer);
      await source.ForEachAsync<TSource>((Action<TSource>) (element => d.Add(keySelector(element), elementSelector(element))), cancellationToken).WithCurrentCulture();
      return d;
    }

    internal static IDbAsyncEnumerable<TResult> Cast<TResult>(
      this IDbAsyncEnumerable source)
    {
      return (IDbAsyncEnumerable<TResult>) new IDbAsyncEnumerableExtensions.CastDbAsyncEnumerable<TResult>(source);
    }

    internal static Task<TSource> FirstAsync<TSource>(this IDbAsyncEnumerable<TSource> source)
    {
      return source.FirstAsync<TSource>(CancellationToken.None);
    }

    internal static Task<TSource> FirstAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate)
    {
      return source.FirstAsync<TSource>(predicate, CancellationToken.None);
    }

    internal static async Task<TSource> FirstAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      TSource current;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
        {
          current = e.Current;
          goto label_8;
        }
      }
      throw Error.EmptySequence();
label_8:
      return current;
    }

    internal static async Task<TSource> FirstAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      TSource current;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
        {
          if (predicate(e.Current))
          {
            current = e.Current;
            goto label_9;
          }
        }
      }
      throw Error.NoMatch();
label_9:
      return current;
    }

    internal static Task<TSource> FirstOrDefaultAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source)
    {
      return source.FirstOrDefaultAsync<TSource>(CancellationToken.None);
    }

    internal static Task<TSource> FirstOrDefaultAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate)
    {
      return source.FirstOrDefaultAsync<TSource>(predicate, CancellationToken.None);
    }

    internal static async Task<TSource> FirstOrDefaultAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          return e.Current;
      }
      return default (TSource);
    }

    internal static async Task<TSource> FirstOrDefaultAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
        {
          if (predicate(e.Current))
            return e.Current;
        }
      }
      return default (TSource);
    }

    internal static Task<TSource> SingleAsync<TSource>(this IDbAsyncEnumerable<TSource> source)
    {
      return source.SingleAsync<TSource>(CancellationToken.None);
    }

    internal static async Task<TSource> SingleAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      TSource source1;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        if (!await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          throw Error.EmptySequence();
        cancellationToken.ThrowIfCancellationRequested();
        TSource result = e.Current;
        if (!await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
        {
          source1 = result;
          goto label_11;
        }
      }
      throw Error.MoreThanOneElement();
label_11:
      return source1;
    }

    internal static Task<TSource> SingleAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate)
    {
      return source.SingleAsync<TSource>(predicate, CancellationToken.None);
    }

    internal static async Task<TSource> SingleAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      TSource result = default (TSource);
      long count = 0;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!predicate(e.Current));
          result = e.Current;
          checked { ++count; }
        }
      }
label_9:
      switch (count)
      {
        case 0:
          throw Error.NoMatch();
        case 1:
          return result;
        default:
          throw Error.MoreThanOneMatch();
      }
    }

    internal static Task<TSource> SingleOrDefaultAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source)
    {
      return source.SingleOrDefaultAsync<TSource>(CancellationToken.None);
    }

    internal static async Task<TSource> SingleOrDefaultAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        if (!await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          return default (TSource);
        cancellationToken.ThrowIfCancellationRequested();
        TSource result = e.Current;
        if (!await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          return result;
      }
      throw Error.MoreThanOneElement();
    }

    internal static Task<TSource> SingleOrDefaultAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate)
    {
      return source.SingleOrDefaultAsync<TSource>(predicate, CancellationToken.None);
    }

    internal static async Task<TSource> SingleOrDefaultAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      TSource result = default (TSource);
      long count = 0;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!predicate(e.Current));
          result = e.Current;
          checked { ++count; }
        }
      }
label_9:
      if (count < 2L)
        return result;
      throw Error.MoreThanOneMatch();
    }

    internal static Task<bool> ContainsAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      TSource value)
    {
      return source.ContainsAsync<TSource>(value, CancellationToken.None);
    }

    internal static async Task<bool> ContainsAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      TSource value,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            if (!EqualityComparer<TSource>.Default.Equals(e.Current, value))
              cancellationToken.ThrowIfCancellationRequested();
            else
              break;
          }
          else
            goto label_10;
        }
        return true;
      }
label_10:
      return false;
    }

    internal static Task<bool> AnyAsync<TSource>(this IDbAsyncEnumerable<TSource> source)
    {
      return source.AnyAsync<TSource>(CancellationToken.None);
    }

    internal static async Task<bool> AnyAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          return true;
      }
      return false;
    }

    internal static Task<bool> AnyAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate)
    {
      return source.AnyAsync<TSource>(predicate, CancellationToken.None);
    }

    internal static async Task<bool> AnyAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            if (!predicate(e.Current))
              cancellationToken.ThrowIfCancellationRequested();
            else
              break;
          }
          else
            goto label_10;
        }
        return true;
      }
label_10:
      return false;
    }

    internal static Task<bool> AllAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate)
    {
      return source.AllAsync<TSource>(predicate, CancellationToken.None);
    }

    internal static async Task<bool> AllAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            if (predicate(e.Current))
              cancellationToken.ThrowIfCancellationRequested();
            else
              break;
          }
          else
            goto label_10;
        }
        return false;
      }
label_10:
      return true;
    }

    internal static Task<int> CountAsync<TSource>(this IDbAsyncEnumerable<TSource> source)
    {
      return source.CountAsync<TSource>(CancellationToken.None);
    }

    internal static async Task<int> CountAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      int count = 0;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            checked { ++count; }
          }
          else
            break;
        }
      }
      return count;
    }

    internal static Task<int> CountAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate)
    {
      return source.CountAsync<TSource>(predicate, CancellationToken.None);
    }

    internal static async Task<int> CountAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      int count = 0;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!predicate(e.Current));
          checked { ++count; }
        }
      }
label_9:
      return count;
    }

    internal static Task<long> LongCountAsync<TSource>(this IDbAsyncEnumerable<TSource> source)
    {
      return source.LongCountAsync<TSource>(CancellationToken.None);
    }

    internal static async Task<long> LongCountAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long count = 0;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            checked { ++count; }
          }
          else
            break;
        }
      }
      return count;
    }

    internal static Task<long> LongCountAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate)
    {
      return source.LongCountAsync<TSource>(predicate, CancellationToken.None);
    }

    internal static async Task<long> LongCountAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      Func<TSource, bool> predicate,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long count = 0;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!predicate(e.Current));
          checked { ++count; }
        }
      }
label_9:
      return count;
    }

    internal static Task<TSource> MinAsync<TSource>(this IDbAsyncEnumerable<TSource> source)
    {
      return source.MinAsync<TSource>(CancellationToken.None);
    }

    internal static async Task<TSource> MinAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Comparer<TSource> comparer = Comparer<TSource>.Default;
      TSource value = default (TSource);
      if ((object) value == null)
      {
        using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
        {
          while (true)
          {
            do
            {
              if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
                cancellationToken.ThrowIfCancellationRequested();
              else
                goto label_10;
            }
            while ((object) e.Current == null || (object) value != null && comparer.Compare(e.Current, value) >= 0);
            value = e.Current;
          }
        }
label_10:
        return value;
      }
      bool hasValue = false;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
            {
              cancellationToken.ThrowIfCancellationRequested();
              if (!hasValue)
                goto label_16;
            }
            else
              goto label_22;
          }
          while (comparer.Compare(e.Current, value) >= 0);
          value = e.Current;
          continue;
label_16:
          value = e.Current;
          hasValue = true;
        }
      }
label_22:
      if (hasValue)
        return value;
      throw Error.EmptySequence();
    }

    internal static Task<TSource> MaxAsync<TSource>(this IDbAsyncEnumerable<TSource> source)
    {
      return source.MaxAsync<TSource>(CancellationToken.None);
    }

    internal static async Task<TSource> MaxAsync<TSource>(
      this IDbAsyncEnumerable<TSource> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Comparer<TSource> comparer = Comparer<TSource>.Default;
      TSource value = default (TSource);
      if ((object) value == null)
      {
        using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
        {
          while (true)
          {
            do
            {
              if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
                cancellationToken.ThrowIfCancellationRequested();
              else
                goto label_10;
            }
            while ((object) e.Current == null || (object) value != null && comparer.Compare(e.Current, value) <= 0);
            value = e.Current;
          }
        }
label_10:
        return value;
      }
      bool hasValue = false;
      using (IDbAsyncEnumerator<TSource> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
            {
              cancellationToken.ThrowIfCancellationRequested();
              if (!hasValue)
                goto label_16;
            }
            else
              goto label_22;
          }
          while (comparer.Compare(e.Current, value) <= 0);
          value = e.Current;
          continue;
label_16:
          value = e.Current;
          hasValue = true;
        }
      }
label_22:
      if (hasValue)
        return value;
      throw Error.EmptySequence();
    }

    internal static Task<int> SumAsync(this IDbAsyncEnumerable<int> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<int> SumAsync(
      this IDbAsyncEnumerable<int> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long sum = 0;
      using (IDbAsyncEnumerator<int> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            checked { sum += (long) e.Current; }
          }
          else
            break;
        }
      }
      return (int) sum;
    }

    internal static Task<int?> SumAsync(this IDbAsyncEnumerable<int?> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<int?> SumAsync(
      this IDbAsyncEnumerable<int?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long sum = 0;
      using (IDbAsyncEnumerator<int?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          checked { sum += (long) e.Current.GetValueOrDefault(); }
        }
      }
label_9:
      return new int?((int) sum);
    }

    internal static Task<long> SumAsync(this IDbAsyncEnumerable<long> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<long> SumAsync(
      this IDbAsyncEnumerable<long> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long sum = 0;
      using (IDbAsyncEnumerator<long> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            checked { sum += e.Current; }
          }
          else
            break;
        }
      }
      return sum;
    }

    internal static Task<long?> SumAsync(this IDbAsyncEnumerable<long?> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<long?> SumAsync(
      this IDbAsyncEnumerable<long?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long sum = 0;
      using (IDbAsyncEnumerator<long?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          checked { sum += e.Current.GetValueOrDefault(); }
        }
      }
label_9:
      return new long?(sum);
    }

    internal static Task<float> SumAsync(this IDbAsyncEnumerable<float> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<float> SumAsync(
      this IDbAsyncEnumerable<float> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      double sum = 0.0;
      using (IDbAsyncEnumerator<float> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            sum += (double) e.Current;
          }
          else
            break;
        }
      }
      return (float) sum;
    }

    internal static Task<float?> SumAsync(this IDbAsyncEnumerable<float?> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<float?> SumAsync(
      this IDbAsyncEnumerable<float?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      double sum = 0.0;
      using (IDbAsyncEnumerator<float?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          sum += (double) e.Current.GetValueOrDefault();
        }
      }
label_9:
      return new float?((float) sum);
    }

    internal static Task<double> SumAsync(this IDbAsyncEnumerable<double> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<double> SumAsync(
      this IDbAsyncEnumerable<double> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      double sum = 0.0;
      using (IDbAsyncEnumerator<double> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            sum += e.Current;
          }
          else
            break;
        }
      }
      return sum;
    }

    internal static Task<double?> SumAsync(this IDbAsyncEnumerable<double?> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<double?> SumAsync(
      this IDbAsyncEnumerable<double?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      double sum = 0.0;
      using (IDbAsyncEnumerator<double?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          sum += e.Current.GetValueOrDefault();
        }
      }
label_9:
      return new double?(sum);
    }

    internal static Task<Decimal> SumAsync(this IDbAsyncEnumerable<Decimal> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<Decimal> SumAsync(
      this IDbAsyncEnumerable<Decimal> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Decimal sum = new Decimal(0);
      using (IDbAsyncEnumerator<Decimal> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            sum += e.Current;
          }
          else
            break;
        }
      }
      return sum;
    }

    internal static Task<Decimal?> SumAsync(this IDbAsyncEnumerable<Decimal?> source)
    {
      return source.SumAsync(CancellationToken.None);
    }

    internal static async Task<Decimal?> SumAsync(
      this IDbAsyncEnumerable<Decimal?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Decimal sum = new Decimal(0);
      using (IDbAsyncEnumerator<Decimal?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          sum += e.Current.GetValueOrDefault();
        }
      }
label_9:
      return new Decimal?(sum);
    }

    internal static Task<double> AverageAsync(this IDbAsyncEnumerable<int> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<double> AverageAsync(
      this IDbAsyncEnumerable<int> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long sum = 0;
      long count = 0;
      using (IDbAsyncEnumerator<int> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            checked { sum += (long) e.Current; }
            checked { ++count; }
          }
          else
            break;
        }
      }
      if (count > 0L)
        return (double) sum / (double) count;
      throw Error.EmptySequence();
    }

    internal static Task<double?> AverageAsync(this IDbAsyncEnumerable<int?> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<double?> AverageAsync(
      this IDbAsyncEnumerable<int?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long sum = 0;
      long count = 0;
      using (IDbAsyncEnumerator<int?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          checked { sum += (long) e.Current.GetValueOrDefault(); }
          checked { ++count; }
        }
      }
label_9:
      if (count > 0L)
        return new double?((double) sum / (double) count);
      throw Error.EmptySequence();
    }

    internal static Task<double> AverageAsync(this IDbAsyncEnumerable<long> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<double> AverageAsync(
      this IDbAsyncEnumerable<long> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long sum = 0;
      long count = 0;
      using (IDbAsyncEnumerator<long> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            checked { sum += e.Current; }
            checked { ++count; }
          }
          else
            break;
        }
      }
      if (count > 0L)
        return (double) sum / (double) count;
      throw Error.EmptySequence();
    }

    internal static Task<double?> AverageAsync(this IDbAsyncEnumerable<long?> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<double?> AverageAsync(
      this IDbAsyncEnumerable<long?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      long sum = 0;
      long count = 0;
      using (IDbAsyncEnumerator<long?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          checked { sum += e.Current.GetValueOrDefault(); }
          checked { ++count; }
        }
      }
label_9:
      if (count > 0L)
        return new double?((double) sum / (double) count);
      throw Error.EmptySequence();
    }

    internal static Task<float> AverageAsync(this IDbAsyncEnumerable<float> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<float> AverageAsync(
      this IDbAsyncEnumerable<float> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      double sum = 0.0;
      long count = 0;
      using (IDbAsyncEnumerator<float> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            sum += (double) e.Current;
            checked { ++count; }
          }
          else
            break;
        }
      }
      if (count > 0L)
        return (float) sum / (float) count;
      throw Error.EmptySequence();
    }

    internal static Task<float?> AverageAsync(this IDbAsyncEnumerable<float?> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<float?> AverageAsync(
      this IDbAsyncEnumerable<float?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      double sum = 0.0;
      long count = 0;
      using (IDbAsyncEnumerator<float?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          sum += (double) e.Current.GetValueOrDefault();
          checked { ++count; }
        }
      }
label_9:
      if (count > 0L)
        return new float?((float) sum / (float) count);
      throw Error.EmptySequence();
    }

    internal static Task<double> AverageAsync(this IDbAsyncEnumerable<double> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<double> AverageAsync(
      this IDbAsyncEnumerable<double> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      double sum = 0.0;
      long count = 0;
      using (IDbAsyncEnumerator<double> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            sum += e.Current;
            checked { ++count; }
          }
          else
            break;
        }
      }
      if (count > 0L)
        return (double) ((float) sum / (float) count);
      throw Error.EmptySequence();
    }

    internal static Task<double?> AverageAsync(this IDbAsyncEnumerable<double?> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<double?> AverageAsync(
      this IDbAsyncEnumerable<double?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      double sum = 0.0;
      long count = 0;
      using (IDbAsyncEnumerator<double?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          sum += e.Current.GetValueOrDefault();
          checked { ++count; }
        }
      }
label_9:
      if (count > 0L)
        return new double?((double) ((float) sum / (float) count));
      throw Error.EmptySequence();
    }

    internal static Task<Decimal> AverageAsync(this IDbAsyncEnumerable<Decimal> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<Decimal> AverageAsync(
      this IDbAsyncEnumerable<Decimal> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Decimal sum = new Decimal(0);
      long count = 0;
      using (IDbAsyncEnumerator<Decimal> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
          {
            cancellationToken.ThrowIfCancellationRequested();
            sum += e.Current;
            checked { ++count; }
          }
          else
            break;
        }
      }
      if (count > 0L)
        return sum / (Decimal) count;
      throw Error.EmptySequence();
    }

    internal static Task<Decimal?> AverageAsync(this IDbAsyncEnumerable<Decimal?> source)
    {
      return source.AverageAsync(CancellationToken.None);
    }

    internal static async Task<Decimal?> AverageAsync(
      this IDbAsyncEnumerable<Decimal?> source,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Decimal sum = new Decimal(0);
      long count = 0;
      using (IDbAsyncEnumerator<Decimal?> e = source.GetAsyncEnumerator())
      {
        while (true)
        {
          do
          {
            if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>())
              cancellationToken.ThrowIfCancellationRequested();
            else
              goto label_9;
          }
          while (!e.Current.HasValue);
          sum += e.Current.GetValueOrDefault();
          checked { ++count; }
        }
      }
label_9:
      if (count > 0L)
        return new Decimal?(sum / (Decimal) count);
      throw Error.EmptySequence();
    }

    private class CastDbAsyncEnumerable<TResult> : IDbAsyncEnumerable<TResult>, IDbAsyncEnumerable
    {
      private readonly IDbAsyncEnumerable _underlyingEnumerable;

      public CastDbAsyncEnumerable(IDbAsyncEnumerable sourceEnumerable)
      {
        this._underlyingEnumerable = sourceEnumerable;
      }

      public IDbAsyncEnumerator<TResult> GetAsyncEnumerator()
      {
        return this._underlyingEnumerable.GetAsyncEnumerator().Cast<TResult>();
      }

      IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
      {
        return this._underlyingEnumerable.GetAsyncEnumerator();
      }
    }

    private static class IdentityFunction<TElement>
    {
      internal static Func<TElement, TElement> Instance
      {
        get
        {
          return (Func<TElement, TElement>) (x => x);
        }
      }
    }
  }
}
