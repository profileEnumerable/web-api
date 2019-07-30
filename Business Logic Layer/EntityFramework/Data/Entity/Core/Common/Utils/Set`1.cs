// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Set`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Common.Utils
{
  internal class Set<TElement> : InternalBase, IEnumerable<TElement>, IEnumerable
  {
    internal static readonly Set<TElement> Empty = new Set<TElement>().MakeReadOnly();
    private readonly HashSet<TElement> _values;
    private bool _isReadOnly;

    internal Set(Set<TElement> other)
      : this((IEnumerable<TElement>) other._values, other.Comparer)
    {
    }

    internal Set()
      : this((IEnumerable<TElement>) null, (IEqualityComparer<TElement>) null)
    {
    }

    internal Set(IEnumerable<TElement> elements)
      : this(elements, (IEqualityComparer<TElement>) null)
    {
    }

    internal Set(IEqualityComparer<TElement> comparer)
      : this((IEnumerable<TElement>) null, comparer)
    {
    }

    internal Set(IEnumerable<TElement> elements, IEqualityComparer<TElement> comparer)
    {
      this._values = new HashSet<TElement>(elements ?? Enumerable.Empty<TElement>(), comparer ?? (IEqualityComparer<TElement>) EqualityComparer<TElement>.Default);
    }

    internal int Count
    {
      get
      {
        return this._values.Count;
      }
    }

    internal IEqualityComparer<TElement> Comparer
    {
      get
      {
        return this._values.Comparer;
      }
    }

    internal bool Contains(TElement element)
    {
      return this._values.Contains(element);
    }

    internal void Add(TElement element)
    {
      this._values.Add(element);
    }

    internal void AddRange(IEnumerable<TElement> elements)
    {
      foreach (TElement element in elements)
        this.Add(element);
    }

    internal void Remove(TElement element)
    {
      this._values.Remove(element);
    }

    internal void Clear()
    {
      this._values.Clear();
    }

    internal TElement[] ToArray()
    {
      return this._values.ToArray<TElement>();
    }

    internal bool SetEquals(Set<TElement> other)
    {
      if (this._values.Count == other._values.Count)
        return this._values.IsSubsetOf((IEnumerable<TElement>) other._values);
      return false;
    }

    internal bool IsSubsetOf(Set<TElement> other)
    {
      return this._values.IsSubsetOf((IEnumerable<TElement>) other._values);
    }

    internal bool Overlaps(Set<TElement> other)
    {
      return this._values.Overlaps((IEnumerable<TElement>) other._values);
    }

    internal void Subtract(IEnumerable<TElement> other)
    {
      this._values.ExceptWith(other);
    }

    internal Set<TElement> Difference(IEnumerable<TElement> other)
    {
      Set<TElement> set = new Set<TElement>(this);
      set.Subtract(other);
      return set;
    }

    internal void Unite(IEnumerable<TElement> other)
    {
      this._values.UnionWith(other);
    }

    internal Set<TElement> Union(IEnumerable<TElement> other)
    {
      Set<TElement> set = new Set<TElement>(this);
      set.Unite(other);
      return set;
    }

    internal void Intersect(Set<TElement> other)
    {
      this._values.IntersectWith((IEnumerable<TElement>) other._values);
    }

    internal Set<TElement> AsReadOnly()
    {
      if (this._isReadOnly)
        return this;
      return new Set<TElement>(this) { _isReadOnly = true };
    }

    internal Set<TElement> MakeReadOnly()
    {
      this._isReadOnly = true;
      return this;
    }

    internal int GetElementsHashCode()
    {
      int num = 0;
      foreach (TElement element in this)
        num ^= this.Comparer.GetHashCode(element);
      return num;
    }

    public HashSet<TElement>.Enumerator GetEnumerator()
    {
      return this._values.GetEnumerator();
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [Conditional("DEBUG")]
    private void AssertReadWrite()
    {
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [Conditional("DEBUG")]
    private void AssertSetCompatible(Set<TElement> other)
    {
    }

    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
    {
      return (IEnumerator<TElement>) this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      StringUtil.ToCommaSeparatedStringSorted(builder, (IEnumerable) this);
    }

    public class Enumerator : IEnumerator<TElement>, IDisposable, IEnumerator
    {
      private Dictionary<TElement, bool>.KeyCollection.Enumerator keys;

      internal Enumerator(
        Dictionary<TElement, bool>.KeyCollection.Enumerator keys)
      {
        this.keys = keys;
      }

      public TElement Current
      {
        get
        {
          return this.keys.Current;
        }
      }

      public void Dispose()
      {
        this.keys.Dispose();
      }

      object IEnumerator.Current
      {
        get
        {
          return ((IEnumerator) this.keys).Current;
        }
      }

      public bool MoveNext()
      {
        return this.keys.MoveNext();
      }

      void IEnumerator.Reset()
      {
        ((IEnumerator) this.keys).Reset();
      }
    }
  }
}
