// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.SpanIndex
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class SpanIndex
  {
    private Dictionary<RowType, Dictionary<int, AssociationEndMember>> _spanMap;
    private Dictionary<RowType, TypeUsage> _rowMap;

    internal void AddSpannedRowType(RowType spannedRowType, TypeUsage originalRowType)
    {
      if (this._rowMap == null)
        this._rowMap = new Dictionary<RowType, TypeUsage>((IEqualityComparer<RowType>) SpanIndex.RowTypeEqualityComparer.Instance);
      this._rowMap[spannedRowType] = originalRowType;
    }

    internal TypeUsage GetSpannedRowType(RowType spannedRowType)
    {
      TypeUsage typeUsage;
      if (this._rowMap != null && this._rowMap.TryGetValue(spannedRowType, out typeUsage))
        return typeUsage;
      return (TypeUsage) null;
    }

    internal bool HasSpanMap(RowType spanRowType)
    {
      if (this._spanMap == null)
        return false;
      return this._spanMap.ContainsKey(spanRowType);
    }

    internal void AddSpanMap(RowType rowType, Dictionary<int, AssociationEndMember> columnMap)
    {
      if (this._spanMap == null)
        this._spanMap = new Dictionary<RowType, Dictionary<int, AssociationEndMember>>((IEqualityComparer<RowType>) SpanIndex.RowTypeEqualityComparer.Instance);
      this._spanMap[rowType] = columnMap;
    }

    internal Dictionary<int, AssociationEndMember> GetSpanMap(
      RowType rowType)
    {
      Dictionary<int, AssociationEndMember> dictionary = (Dictionary<int, AssociationEndMember>) null;
      if (this._spanMap != null && this._spanMap.TryGetValue(rowType, out dictionary))
        return dictionary;
      return (Dictionary<int, AssociationEndMember>) null;
    }

    private sealed class RowTypeEqualityComparer : IEqualityComparer<RowType>
    {
      internal static readonly SpanIndex.RowTypeEqualityComparer Instance = new SpanIndex.RowTypeEqualityComparer();

      private RowTypeEqualityComparer()
      {
      }

      public bool Equals(RowType x, RowType y)
      {
        if (x == null || y == null)
          return false;
        return x.EdmEquals((MetadataItem) y);
      }

      public int GetHashCode(RowType obj)
      {
        return obj.Identity.GetHashCode();
      }
    }
  }
}
