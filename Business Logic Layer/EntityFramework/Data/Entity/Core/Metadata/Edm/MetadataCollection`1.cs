// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataCollection`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Runtime.CompilerServices;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MetadataCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    where T : MetadataItem
  {
    internal const int UseDictionaryCrossover = 8;
    private bool _readOnly;
    private List<T> _metadataList;
    private volatile Dictionary<string, T> _caseSensitiveDictionary;
    private volatile Dictionary<string, int> _caseInsensitiveDictionary;

    internal MetadataCollection()
    {
      this._metadataList = new List<T>();
    }

    internal MetadataCollection(IEnumerable<T> items)
    {
      this._metadataList = new List<T>();
      if (items == null)
        return;
      foreach (T obj in items)
      {
        if ((object) obj == null)
          throw new ArgumentException(Strings.ADP_CollectionParameterElementIsNull((object) nameof (items)));
        this.AddInternal(obj);
      }
    }

    private MetadataCollection(List<T> items)
    {
      this._metadataList = items;
    }

    internal static MetadataCollection<T> Wrap(List<T> items)
    {
      return new MetadataCollection<T>(items);
    }

    public virtual int Count
    {
      get
      {
        return this._metadataList.Count;
      }
    }

    public virtual T this[int index]
    {
      get
      {
        return this._metadataList[index];
      }
      set
      {
        this.ThrowIfReadOnly();
        string identity = this._metadataList[index].Identity;
        this._metadataList[index] = value;
        this.HandleIdentityChange(value, identity, false);
      }
    }

    internal void HandleIdentityChange(T item, string initialIdentity)
    {
      this.HandleIdentityChange(item, initialIdentity, true);
    }

    private void HandleIdentityChange(T item, string initialIdentity, bool validate)
    {
      T obj;
      if (this._caseSensitiveDictionary != null && (!validate || this._caseSensitiveDictionary.TryGetValue(initialIdentity, out obj) && object.ReferenceEquals((object) obj, (object) item)))
      {
        this.RemoveFromCaseSensitiveDictionary(initialIdentity);
        string identity = item.Identity;
        if (this._caseSensitiveDictionary.ContainsKey(identity))
          this._caseSensitiveDictionary = (Dictionary<string, T>) null;
        else
          this._caseSensitiveDictionary.Add(identity, item);
      }
      this._caseInsensitiveDictionary = (Dictionary<string, int>) null;
    }

    public virtual T this[string identity]
    {
      get
      {
        return this.GetValue(identity, false);
      }
      set
      {
        throw new InvalidOperationException(Strings.OperationOnReadOnlyCollection);
      }
    }

    public virtual T GetValue(string identity, bool ignoreCase)
    {
      T obj;
      if (!this.TryGetValue(identity, ignoreCase, out obj))
        throw new ArgumentException(Strings.ItemInvalidIdentity((object) identity), nameof (identity));
      return obj;
    }

    public virtual bool TryGetValue(string identity, bool ignoreCase, out T item)
    {
      if (!ignoreCase)
        return this.FindCaseSensitive(identity, out item);
      return this.FindCaseInsensitive(identity, out item, false);
    }

    public virtual void Add(T item)
    {
      this.ThrowIfReadOnly();
      this.AddInternal(item);
    }

    private void AddInternal(T item)
    {
      string identity = item.Identity;
      if (this.ContainsIdentityCaseSensitive(identity))
        throw new ArgumentException(Strings.ItemDuplicateIdentity((object) identity), nameof (item));
      this._metadataList.Add(item);
      if (this._caseSensitiveDictionary != null)
        this._caseSensitiveDictionary.Add(identity, item);
      this._caseInsensitiveDictionary = (Dictionary<string, int>) null;
    }

    internal void AddRange(List<T> items)
    {
      Check.NotNull<List<T>>(items, nameof (items));
      foreach (T obj in items)
      {
        if ((object) obj == null)
          throw new ArgumentException(Strings.ADP_CollectionParameterElementIsNull((object) nameof (items)));
        this.AddInternal(obj);
      }
    }

    internal bool Remove(T item)
    {
      this.ThrowIfReadOnly();
      if (!this._metadataList.Remove(item))
        return false;
      if (this._caseSensitiveDictionary != null)
        this.RemoveFromCaseSensitiveDictionary(item.Identity);
      this._caseInsensitiveDictionary = (Dictionary<string, int>) null;
      return true;
    }

    public virtual ReadOnlyCollection<T> AsReadOnly
    {
      get
      {
        return new ReadOnlyCollection<T>((IList<T>) this._metadataList);
      }
    }

    public virtual ReadOnlyMetadataCollection<T> AsReadOnlyMetadataCollection()
    {
      return new ReadOnlyMetadataCollection<T>(this);
    }

    public bool IsReadOnly
    {
      get
      {
        return this._readOnly;
      }
    }

    internal void ResetReadOnly()
    {
      this._readOnly = false;
    }

    public MetadataCollection<T> SetReadOnly()
    {
      for (int index = 0; index < this._metadataList.Count; ++index)
        this._metadataList[index].SetReadOnly();
      this._readOnly = true;
      this._metadataList.TrimExcess();
      if (this._metadataList.Count <= 8)
      {
        this._caseSensitiveDictionary = (Dictionary<string, T>) null;
        this._caseInsensitiveDictionary = (Dictionary<string, int>) null;
      }
      return this;
    }

    void IList<T>.Insert(int index, T item)
    {
      throw new InvalidOperationException(Strings.OperationOnReadOnlyCollection);
    }

    bool ICollection<T>.Remove(T item)
    {
      throw new InvalidOperationException(Strings.OperationOnReadOnlyCollection);
    }

    void IList<T>.RemoveAt(int index)
    {
      throw new InvalidOperationException(Strings.OperationOnReadOnlyCollection);
    }

    void ICollection<T>.Clear()
    {
      throw new InvalidOperationException(Strings.OperationOnReadOnlyCollection);
    }

    public bool Contains(T item)
    {
      T obj;
      if (this.TryGetValue(item.Identity, false, out obj))
        return object.ReferenceEquals((object) obj, (object) item);
      return false;
    }

    public virtual bool ContainsIdentity(string identity)
    {
      return this.ContainsIdentityCaseSensitive(identity);
    }

    public virtual int IndexOf(T item)
    {
      return this._metadataList.IndexOf(item);
    }

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
      this._metadataList.CopyTo(array, arrayIndex);
    }

    public ReadOnlyMetadataCollection<T>.Enumerator GetEnumerator()
    {
      return new ReadOnlyMetadataCollection<T>.Enumerator((IList<T>) this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return (IEnumerator<T>) this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    internal void InvalidateCache()
    {
      this._caseSensitiveDictionary = (Dictionary<string, T>) null;
      this._caseInsensitiveDictionary = (Dictionary<string, int>) null;
    }

    internal bool HasCaseSensitiveDictionary
    {
      get
      {
        return this._caseSensitiveDictionary != null;
      }
    }

    internal bool HasCaseInsensitiveDictionary
    {
      get
      {
        return this._caseInsensitiveDictionary != null;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Dictionary<string, T> GetCaseSensitiveDictionary()
    {
      if (this._caseSensitiveDictionary == null && this._metadataList.Count > 8)
        this._caseSensitiveDictionary = this.CreateCaseSensitiveDictionary();
      return this._caseSensitiveDictionary;
    }

    private Dictionary<string, T> CreateCaseSensitiveDictionary()
    {
      Dictionary<string, T> dictionary = new Dictionary<string, T>(this._metadataList.Count, (IEqualityComparer<string>) StringComparer.Ordinal);
      for (int index = 0; index < this._metadataList.Count; ++index)
      {
        T metadata = this._metadataList[index];
        dictionary.Add(metadata.Identity, metadata);
      }
      return dictionary;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Dictionary<string, int> GetCaseInsensitiveDictionary()
    {
      if (this._caseInsensitiveDictionary == null && this._metadataList.Count > 8)
        this._caseInsensitiveDictionary = this.CreateCaseInsensitiveDictionary();
      return this._caseInsensitiveDictionary;
    }

    private Dictionary<string, int> CreateCaseInsensitiveDictionary()
    {
      Dictionary<string, int> dictionary = new Dictionary<string, int>(this._metadataList.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          this._metadataList[0].Identity,
          0
        }
      };
      for (int index = 1; index < this._metadataList.Count; ++index)
      {
        string identity = this._metadataList[index].Identity;
        int num;
        if (!dictionary.TryGetValue(identity, out num))
          dictionary[identity] = index;
        else if (num >= 0)
          dictionary[identity] = -1;
      }
      return dictionary;
    }

    private bool ContainsIdentityCaseSensitive(string identity)
    {
      Dictionary<string, T> sensitiveDictionary = this.GetCaseSensitiveDictionary();
      if (sensitiveDictionary != null)
        return sensitiveDictionary.ContainsKey(identity);
      return this.ListContainsIdentityCaseSensitive(identity);
    }

    private bool ListContainsIdentityCaseSensitive(string identity)
    {
      for (int index = 0; index < this._metadataList.Count; ++index)
      {
        if (this._metadataList[index].Identity.Equals(identity, StringComparison.Ordinal))
          return true;
      }
      return false;
    }

    private bool FindCaseSensitive(string identity, out T item)
    {
      Dictionary<string, T> sensitiveDictionary = this.GetCaseSensitiveDictionary();
      if (sensitiveDictionary == null)
        return this.ListFindCaseSensitive(identity, out item);
      return sensitiveDictionary.TryGetValue(identity, out item);
    }

    private bool ListFindCaseSensitive(string identity, out T item)
    {
      for (int index = 0; index < this._metadataList.Count; ++index)
      {
        T metadata = this._metadataList[index];
        if (metadata.Identity.Equals(identity, StringComparison.Ordinal))
        {
          item = metadata;
          return true;
        }
      }
      item = default (T);
      return false;
    }

    private bool FindCaseInsensitive(string identity, out T item, bool throwOnMultipleMatches)
    {
      Dictionary<string, int> insensitiveDictionary = this.GetCaseInsensitiveDictionary();
      if (insensitiveDictionary == null)
        return this.ListFindCaseInsensitive(identity, out item, throwOnMultipleMatches);
      int index;
      if (insensitiveDictionary.TryGetValue(identity, out index))
      {
        if (index >= 0)
        {
          item = this._metadataList[index];
          return true;
        }
        if (throwOnMultipleMatches)
          throw new InvalidOperationException(Strings.MoreThanOneItemMatchesIdentity((object) identity));
      }
      item = default (T);
      return false;
    }

    private bool ListFindCaseInsensitive(string identity, out T item, bool throwOnMultipleMatches)
    {
      bool flag = false;
      item = default (T);
      for (int index = 0; index < this._metadataList.Count; ++index)
      {
        T metadata = this._metadataList[index];
        if (metadata.Identity.Equals(identity, StringComparison.OrdinalIgnoreCase))
        {
          if (flag)
          {
            if (throwOnMultipleMatches)
              throw new InvalidOperationException(Strings.MoreThanOneItemMatchesIdentity((object) identity));
            item = default (T);
            return false;
          }
          flag = true;
          item = metadata;
        }
      }
      return flag;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RemoveFromCaseSensitiveDictionary(string identity)
    {
      this._caseSensitiveDictionary.Remove(identity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfReadOnly()
    {
      if (this.IsReadOnly)
        throw new InvalidOperationException(Strings.OperationOnReadOnlyCollection);
    }
  }
}
