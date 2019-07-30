// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.QueryCache.CompiledQueryCacheKey
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.QueryCache
{
  internal sealed class CompiledQueryCacheKey : QueryCacheKey
  {
    private readonly Guid _cacheIdentity;

    internal CompiledQueryCacheKey(Guid cacheIdentity)
    {
      this._cacheIdentity = cacheIdentity;
    }

    public override bool Equals(object compareTo)
    {
      if (typeof (CompiledQueryCacheKey) != compareTo.GetType())
        return false;
      return ((CompiledQueryCacheKey) compareTo)._cacheIdentity.Equals(this._cacheIdentity);
    }

    public override int GetHashCode()
    {
      return this._cacheIdentity.GetHashCode();
    }

    public override string ToString()
    {
      return this._cacheIdentity.ToString();
    }
  }
}
