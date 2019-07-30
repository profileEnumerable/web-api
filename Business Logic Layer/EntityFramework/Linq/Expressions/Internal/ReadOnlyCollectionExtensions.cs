// Decompiled with JetBrains decompiler
// Type: System.Linq.Expressions.Internal.ReadOnlyCollectionExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Linq.Expressions.Internal
{
  internal static class ReadOnlyCollectionExtensions
  {
    internal static ReadOnlyCollection<T> ToReadOnlyCollection<T>(
      this IEnumerable<T> sequence)
    {
      if (sequence == null)
        return ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>.Empty;
      return sequence as ReadOnlyCollection<T> ?? new ReadOnlyCollection<T>((IList<T>) sequence.ToArray<T>());
    }

    private static class DefaultReadOnlyCollection<T>
    {
      private static ReadOnlyCollection<T> _defaultCollection;

      internal static ReadOnlyCollection<T> Empty
      {
        get
        {
          if (ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>._defaultCollection == null)
            ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>._defaultCollection = new ReadOnlyCollection<T>((IList<T>) new T[0]);
          return ReadOnlyCollectionExtensions.DefaultReadOnlyCollection<T>._defaultCollection;
        }
      }
    }
  }
}
