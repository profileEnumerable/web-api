// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ObjectItemCachedAssemblyLoader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal sealed class ObjectItemCachedAssemblyLoader : ObjectItemAssemblyLoader
  {
    private ImmutableAssemblyCacheEntry CacheEntry
    {
      get
      {
        return (ImmutableAssemblyCacheEntry) base.CacheEntry;
      }
    }

    internal ObjectItemCachedAssemblyLoader(
      Assembly assembly,
      ImmutableAssemblyCacheEntry cacheEntry,
      ObjectItemLoadingSessionData sessionData)
      : base(assembly, (AssemblyCacheEntry) cacheEntry, sessionData)
    {
    }

    protected override void AddToAssembliesLoaded()
    {
    }

    protected override void LoadTypesFromAssembly()
    {
      foreach (EdmType edmType in (IEnumerable<EdmType>) this.CacheEntry.TypesInAssembly)
      {
        if (!this.SessionData.TypesInLoading.ContainsKey(edmType.Identity))
          this.SessionData.TypesInLoading.Add(edmType.Identity, edmType);
      }
    }
  }
}
