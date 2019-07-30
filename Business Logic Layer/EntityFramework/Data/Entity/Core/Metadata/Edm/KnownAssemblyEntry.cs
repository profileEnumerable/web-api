// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.KnownAssemblyEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal sealed class KnownAssemblyEntry
  {
    private readonly AssemblyCacheEntry _cacheEntry;

    internal KnownAssemblyEntry(AssemblyCacheEntry cacheEntry, bool seenWithEdmItemCollection)
    {
      this._cacheEntry = cacheEntry;
      this.ReferencedAssembliesAreLoaded = false;
      this.SeenWithEdmItemCollection = seenWithEdmItemCollection;
    }

    internal AssemblyCacheEntry CacheEntry
    {
      get
      {
        return this._cacheEntry;
      }
    }

    public bool ReferencedAssembliesAreLoaded { get; set; }

    public bool SeenWithEdmItemCollection { get; set; }

    public bool HaveSeenInCompatibleContext(object loaderCookie, EdmItemCollection itemCollection)
    {
      if (!this.SeenWithEdmItemCollection && itemCollection != null)
        return ObjectItemAssemblyLoader.IsAttributeLoader(loaderCookie);
      return true;
    }
  }
}
