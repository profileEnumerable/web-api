// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.FilteredReadOnlyMetadataCollection`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class FilteredReadOnlyMetadataCollection<TDerived, TBase> : ReadOnlyMetadataCollection<TDerived>, IBaseList<TBase>, IList, ICollection, IEnumerable
    where TDerived : TBase
    where TBase : MetadataItem
  {
    private readonly ReadOnlyMetadataCollection<TBase> _source;
    private readonly Predicate<TBase> _predicate;

    internal FilteredReadOnlyMetadataCollection(
      ReadOnlyMetadataCollection<TBase> collection,
      Predicate<TBase> predicate)
      : base(FilteredReadOnlyMetadataCollection<TDerived, TBase>.FilterCollection(collection, predicate))
    {
      this._source = collection;
      this._predicate = predicate;
    }

    public override TDerived this[string identity]
    {
      get
      {
        TBase @base = this._source[identity];
        if (this._predicate(@base))
          return (TDerived) (object) @base;
        throw new ArgumentException(Strings.ItemInvalidIdentity((object) identity), nameof (identity));
      }
    }

    public override TDerived GetValue(string identity, bool ignoreCase)
    {
      TBase @base = this._source.GetValue(identity, ignoreCase);
      if (this._predicate(@base))
        return (TDerived) (object) @base;
      throw new ArgumentException(Strings.ItemInvalidIdentity((object) identity), nameof (identity));
    }

    public override bool Contains(string identity)
    {
      TBase @base;
      if (this._source.TryGetValue(identity, false, out @base))
        return this._predicate(@base);
      return false;
    }

    public override bool TryGetValue(string identity, bool ignoreCase, out TDerived item)
    {
      item = default (TDerived);
      TBase @base;
      if (!this._source.TryGetValue(identity, ignoreCase, out @base) || !this._predicate(@base))
        return false;
      item = (TDerived) (object) @base;
      return true;
    }

    internal static List<TDerived> FilterCollection(
      ReadOnlyMetadataCollection<TBase> collection,
      Predicate<TBase> predicate)
    {
      List<TDerived> derivedList = new List<TDerived>(collection.Count);
      for (int index = 0; index < collection.Count; ++index)
      {
        TBase @base = collection[index];
        if (predicate(@base))
          derivedList.Add((TDerived) (object) @base);
      }
      return derivedList;
    }

    [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
    public override int IndexOf(TDerived value)
    {
      TBase @base;
      if (this._source.TryGetValue(value.Identity, false, out @base) && this._predicate(@base))
        return base.IndexOf((TDerived) (object) @base);
      return -1;
    }

    TBase IBaseList<TBase>.this[string identity]
    {
      get
      {
        return (TBase) this[identity];
      }
    }

    TBase IBaseList<TBase>.this[int index]
    {
      get
      {
        return (TBase) this[index];
      }
    }

    int IBaseList<TBase>.IndexOf(TBase item)
    {
      if (this._predicate(item))
        return this.IndexOf((TDerived) (object) item);
      return -1;
    }

    [SpecialName]
    bool IList.get_IsReadOnly()
    {
      return this.IsReadOnly;
    }
  }
}
