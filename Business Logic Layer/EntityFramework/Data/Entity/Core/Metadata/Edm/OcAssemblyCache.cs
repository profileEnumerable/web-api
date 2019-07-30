// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.OcAssemblyCache
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class OcAssemblyCache
  {
    private readonly Dictionary<Assembly, ImmutableAssemblyCacheEntry> _conventionalOcCache;

    internal OcAssemblyCache()
    {
      this._conventionalOcCache = new Dictionary<Assembly, ImmutableAssemblyCacheEntry>();
    }

    internal bool TryGetConventionalOcCacheFromAssemblyCache(
      Assembly assemblyToLookup,
      out ImmutableAssemblyCacheEntry cacheEntry)
    {
      cacheEntry = (ImmutableAssemblyCacheEntry) null;
      return this._conventionalOcCache.TryGetValue(assemblyToLookup, out cacheEntry);
    }

    internal void AddAssemblyToOcCacheFromAssemblyCache(
      Assembly assembly,
      ImmutableAssemblyCacheEntry cacheEntry)
    {
      if (this._conventionalOcCache.ContainsKey(assembly))
        return;
      this._conventionalOcCache.Add(assembly, cacheEntry);
    }
  }
}
