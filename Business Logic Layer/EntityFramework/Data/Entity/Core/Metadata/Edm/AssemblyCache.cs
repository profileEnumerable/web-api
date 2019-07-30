// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.AssemblyCache
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal static class AssemblyCache
  {
    private static readonly Dictionary<Assembly, ImmutableAssemblyCacheEntry> _globalAssemblyCache = new Dictionary<Assembly, ImmutableAssemblyCacheEntry>();
    private static readonly object _assemblyCacheLock = new object();

    internal static LockedAssemblyCache AquireLockedAssemblyCache()
    {
      return new LockedAssemblyCache(AssemblyCache._assemblyCacheLock, AssemblyCache._globalAssemblyCache);
    }

    internal static void LoadAssembly(
      Assembly assembly,
      bool loadReferencedAssemblies,
      KnownAssembliesSet knownAssemblies,
      out Dictionary<string, EdmType> typesInLoading,
      out List<EdmItemError> errors)
    {
      object loaderCookie = (object) null;
      AssemblyCache.LoadAssembly(assembly, loadReferencedAssemblies, knownAssemblies, (EdmItemCollection) null, (Action<string>) null, ref loaderCookie, out typesInLoading, out errors);
    }

    internal static void LoadAssembly(
      Assembly assembly,
      bool loadReferencedAssemblies,
      KnownAssembliesSet knownAssemblies,
      EdmItemCollection edmItemCollection,
      Action<string> logLoadMessage,
      ref object loaderCookie,
      out Dictionary<string, EdmType> typesInLoading,
      out List<EdmItemError> errors)
    {
      typesInLoading = (Dictionary<string, EdmType>) null;
      errors = (List<EdmItemError>) null;
      using (LockedAssemblyCache lockedAssemblyCache = AssemblyCache.AquireLockedAssemblyCache())
      {
        ObjectItemLoadingSessionData loadingData = new ObjectItemLoadingSessionData(knownAssemblies, lockedAssemblyCache, edmItemCollection, logLoadMessage, loaderCookie);
        AssemblyCache.LoadAssembly(assembly, loadReferencedAssemblies, loadingData);
        loaderCookie = loadingData.LoaderCookie;
        loadingData.CompleteSession();
        if (loadingData.EdmItemErrors.Count == 0)
        {
          new EdmValidator() { SkipReadOnlyItems = true }.Validate<EdmType>((IEnumerable<EdmType>) loadingData.TypesInLoading.Values, loadingData.EdmItemErrors);
          if (loadingData.EdmItemErrors.Count == 0)
          {
            if (ObjectItemAssemblyLoader.IsAttributeLoader(loadingData.ObjectItemAssemblyLoaderFactory))
              AssemblyCache.UpdateCache(lockedAssemblyCache, loadingData.AssembliesLoaded);
            else if (loadingData.EdmItemCollection != null && ObjectItemAssemblyLoader.IsConventionLoader(loadingData.ObjectItemAssemblyLoaderFactory))
              AssemblyCache.UpdateCache(loadingData.EdmItemCollection, loadingData.AssembliesLoaded);
          }
        }
        if (loadingData.TypesInLoading.Count > 0)
        {
          foreach (MetadataItem metadataItem in loadingData.TypesInLoading.Values)
            metadataItem.SetReadOnly();
        }
        typesInLoading = loadingData.TypesInLoading;
        errors = loadingData.EdmItemErrors;
      }
    }

    private static void LoadAssembly(
      Assembly assembly,
      bool loadReferencedAssemblies,
      ObjectItemLoadingSessionData loadingData)
    {
      KnownAssemblyEntry entry;
      bool flag;
      if (loadingData.KnownAssemblies.TryGetKnownAssembly(assembly, (object) loadingData.ObjectItemAssemblyLoaderFactory, loadingData.EdmItemCollection, out entry))
      {
        flag = !entry.ReferencedAssembliesAreLoaded && loadReferencedAssemblies;
      }
      else
      {
        ObjectItemAssemblyLoader.CreateLoader(assembly, loadingData).Load();
        flag = loadReferencedAssemblies;
      }
      if (!flag)
        return;
      if (entry == null && loadingData.KnownAssemblies.TryGetKnownAssembly(assembly, (object) loadingData.ObjectItemAssemblyLoaderFactory, loadingData.EdmItemCollection, out entry) || entry != null)
        entry.ReferencedAssembliesAreLoaded = true;
      foreach (Assembly referencedAssembly in MetadataAssemblyHelper.GetNonSystemReferencedAssemblies(assembly))
        AssemblyCache.LoadAssembly(referencedAssembly, loadReferencedAssemblies, loadingData);
    }

    private static void UpdateCache(
      EdmItemCollection edmItemCollection,
      Dictionary<Assembly, MutableAssemblyCacheEntry> assemblies)
    {
      foreach (KeyValuePair<Assembly, MutableAssemblyCacheEntry> assembly in assemblies)
        edmItemCollection.ConventionalOcCache.AddAssemblyToOcCacheFromAssemblyCache(assembly.Key, new ImmutableAssemblyCacheEntry(assembly.Value));
    }

    private static void UpdateCache(
      LockedAssemblyCache lockedAssemblyCache,
      Dictionary<Assembly, MutableAssemblyCacheEntry> assemblies)
    {
      foreach (KeyValuePair<Assembly, MutableAssemblyCacheEntry> assembly in assemblies)
        lockedAssemblyCache.Add(assembly.Key, new ImmutableAssemblyCacheEntry(assembly.Value));
    }
  }
}
