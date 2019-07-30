// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ReadOnlySet`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Internal
{
  internal class ReadOnlySet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable
  {
    private readonly ISet<T> _set;

    public ReadOnlySet(ISet<T> set)
    {
      this._set = set;
    }

    public bool Add(T item)
    {
      throw Error.DbPropertyValues_PropertyValueNamesAreReadonly();
    }

    public void ExceptWith(IEnumerable<T> other)
    {
      this._set.ExceptWith(other);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
      this._set.IntersectWith(other);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
      return this._set.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
      return this._set.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
      return this._set.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
      return this._set.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
      return this._set.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
      return this._set.SetEquals(other);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
      this._set.SymmetricExceptWith(other);
    }

    public void UnionWith(IEnumerable<T> other)
    {
      this._set.UnionWith(other);
    }

    void ICollection<T>.Add(T item)
    {
      throw Error.DbPropertyValues_PropertyValueNamesAreReadonly();
    }

    public void Clear()
    {
      throw Error.DbPropertyValues_PropertyValueNamesAreReadonly();
    }

    public bool Contains(T item)
    {
      return this._set.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      this._set.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get
      {
        return this._set.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public bool Remove(T item)
    {
      throw Error.DbPropertyValues_PropertyValueNamesAreReadonly();
    }

    public IEnumerator<T> GetEnumerator()
    {
      return this._set.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this._set.GetEnumerator();
    }
  }
}
