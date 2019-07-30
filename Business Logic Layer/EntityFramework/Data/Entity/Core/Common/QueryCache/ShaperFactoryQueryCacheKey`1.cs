// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.QueryCache.ShaperFactoryQueryCacheKey`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Core.Common.QueryCache
{
  internal class ShaperFactoryQueryCacheKey<T> : QueryCacheKey
  {
    private readonly string _columnMapKey;
    private readonly MergeOption _mergeOption;
    private readonly bool _isValueLayer;
    private readonly bool _streaming;

    internal ShaperFactoryQueryCacheKey(
      string columnMapKey,
      MergeOption mergeOption,
      bool streaming,
      bool isValueLayer)
    {
      this._columnMapKey = columnMapKey;
      this._mergeOption = mergeOption;
      this._isValueLayer = isValueLayer;
      this._streaming = streaming;
    }

    public override bool Equals(object obj)
    {
      ShaperFactoryQueryCacheKey<T> factoryQueryCacheKey = obj as ShaperFactoryQueryCacheKey<T>;
      if (factoryQueryCacheKey == null || !this._columnMapKey.Equals(factoryQueryCacheKey._columnMapKey, QueryCacheKey._stringComparison) || (this._mergeOption != factoryQueryCacheKey._mergeOption || this._isValueLayer != factoryQueryCacheKey._isValueLayer))
        return false;
      return this._streaming == factoryQueryCacheKey._streaming;
    }

    public override int GetHashCode()
    {
      return this._columnMapKey.GetHashCode();
    }
  }
}
