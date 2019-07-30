// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataCache
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.EntityClient.Internal;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MetadataCache
  {
    public static readonly MetadataCache Instance = new MetadataCache();
    private Memoizer<string, List<MetadataArtifactLoader>> _artifactLoaderCache = new Memoizer<string, List<MetadataArtifactLoader>>(new Func<string, List<MetadataArtifactLoader>>(MetadataCache.SplitPaths), (IEqualityComparer<string>) null);
    private readonly ConcurrentDictionary<string, MetadataWorkspace> _cachedWorkspaces = new ConcurrentDictionary<string, MetadataWorkspace>();
    private const string DataDirectory = "|datadirectory|";
    private const string MetadataPathSeparator = "|";
    private const string SemicolonSeparator = ";";

    private static List<MetadataArtifactLoader> SplitPaths(string paths)
    {
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<string> stringList = new List<string>();
      int num1 = paths.IndexOf("|datadirectory|", StringComparison.OrdinalIgnoreCase);
      int startIndex;
      while (true)
      {
        int num2;
        switch (num1)
        {
          case -1:
            goto label_7;
          case 0:
            num2 = -1;
            break;
          default:
            num2 = paths.LastIndexOf("|", num1 - 1, StringComparison.Ordinal);
            break;
        }
        startIndex = num2 + 1;
        int num3 = paths.IndexOf("|", num1 + "|datadirectory|".Length, StringComparison.Ordinal);
        if (num3 != -1)
        {
          stringList.Add(paths.Substring(startIndex, num3 - startIndex));
          paths = paths.Remove(startIndex, num3 - startIndex);
          num1 = paths.IndexOf("|datadirectory|", StringComparison.OrdinalIgnoreCase);
        }
        else
          break;
      }
      stringList.Add(paths.Substring(startIndex));
      paths = paths.Remove(startIndex);
label_7:
      string[] strArray = paths.Split(new string[1]{ "|" }, StringSplitOptions.RemoveEmptyEntries);
      if (stringList.Count > 0)
      {
        stringList.AddRange((IEnumerable<string>) strArray);
        strArray = stringList.ToArray();
      }
      List<MetadataArtifactLoader> metadataArtifactLoaderList1 = new List<MetadataArtifactLoader>();
      List<MetadataArtifactLoader> metadataArtifactLoaderList2 = new List<MetadataArtifactLoader>();
      List<MetadataArtifactLoader> metadataArtifactLoaderList3 = new List<MetadataArtifactLoader>();
      List<MetadataArtifactLoader> metadataArtifactLoaderList4 = new List<MetadataArtifactLoader>();
      for (int index = 0; index < strArray.Length; ++index)
      {
        strArray[index] = strArray[index].Trim();
        if (strArray[index].Length > 0)
        {
          MetadataArtifactLoader metadataArtifactLoader = MetadataArtifactLoader.Create(strArray[index], MetadataArtifactLoader.ExtensionCheck.All, (string) null, (ICollection<string>) stringSet);
          if (strArray[index].EndsWith(".csdl", StringComparison.OrdinalIgnoreCase))
            metadataArtifactLoaderList1.Add(metadataArtifactLoader);
          else if (strArray[index].EndsWith(".msl", StringComparison.OrdinalIgnoreCase))
            metadataArtifactLoaderList2.Add(metadataArtifactLoader);
          else if (strArray[index].EndsWith(".ssdl", StringComparison.OrdinalIgnoreCase))
            metadataArtifactLoaderList3.Add(metadataArtifactLoader);
          else
            metadataArtifactLoaderList4.Add(metadataArtifactLoader);
        }
      }
      metadataArtifactLoaderList4.AddRange((IEnumerable<MetadataArtifactLoader>) metadataArtifactLoaderList3);
      metadataArtifactLoaderList4.AddRange((IEnumerable<MetadataArtifactLoader>) metadataArtifactLoaderList2);
      metadataArtifactLoaderList4.AddRange((IEnumerable<MetadataArtifactLoader>) metadataArtifactLoaderList1);
      return metadataArtifactLoaderList4;
    }

    public MetadataWorkspace GetMetadataWorkspace(
      DbConnectionOptions effectiveConnectionOptions)
    {
      MetadataArtifactLoader artifactLoader = this.GetArtifactLoader(effectiveConnectionOptions);
      return this.GetMetadataWorkspace(MetadataCache.CreateMetadataCacheKey((IList<string>) artifactLoader.GetPaths(), effectiveConnectionOptions["provider"]), artifactLoader);
    }

    public MetadataArtifactLoader GetArtifactLoader(
      DbConnectionOptions effectiveConnectionOptions)
    {
      string connectionOption = effectiveConnectionOptions["metadata"];
      if (string.IsNullOrEmpty(connectionOption))
        return MetadataArtifactLoader.Create(new List<MetadataArtifactLoader>());
      List<MetadataArtifactLoader> metadataArtifactLoaderList = this._artifactLoaderCache.Evaluate(connectionOption);
      return MetadataArtifactLoader.Create(MetadataCache.ShouldRecalculateMetadataArtifactLoader((IEnumerable<MetadataArtifactLoader>) metadataArtifactLoaderList) ? MetadataCache.SplitPaths(connectionOption) : metadataArtifactLoaderList);
    }

    public MetadataWorkspace GetMetadataWorkspace(
      string cacheKey,
      MetadataArtifactLoader artifactLoader)
    {
      return this._cachedWorkspaces.GetOrAdd(cacheKey, (Func<string, MetadataWorkspace>) (k =>
      {
        EdmItemCollection edmItemCollection = MetadataCache.LoadEdmItemCollection(artifactLoader);
        Lazy<StorageMappingItemCollection> mappingLoader = new Lazy<StorageMappingItemCollection>((Func<StorageMappingItemCollection>) (() => MetadataCache.LoadStoreCollection(edmItemCollection, artifactLoader)));
        return new MetadataWorkspace((Func<EdmItemCollection>) (() => edmItemCollection), (Func<StoreItemCollection>) (() => mappingLoader.Value.StoreItemCollection), (Func<StorageMappingItemCollection>) (() => mappingLoader.Value));
      }));
    }

    public void Clear()
    {
      this._cachedWorkspaces.Clear();
      Interlocked.CompareExchange<Memoizer<string, List<MetadataArtifactLoader>>>(ref this._artifactLoaderCache, new Memoizer<string, List<MetadataArtifactLoader>>(new Func<string, List<MetadataArtifactLoader>>(MetadataCache.SplitPaths), (IEqualityComparer<string>) null), this._artifactLoaderCache);
    }

    private static StorageMappingItemCollection LoadStoreCollection(
      EdmItemCollection edmItemCollection,
      MetadataArtifactLoader loader)
    {
      List<XmlReader> readers1 = loader.CreateReaders(DataSpace.SSpace);
      StoreItemCollection storeCollection;
      try
      {
        storeCollection = new StoreItemCollection((IEnumerable<XmlReader>) readers1, (IEnumerable<string>) loader.GetPaths(DataSpace.SSpace));
      }
      finally
      {
        Helper.DisposeXmlReaders((IEnumerable<XmlReader>) readers1);
      }
      List<XmlReader> readers2 = loader.CreateReaders(DataSpace.CSSpace);
      try
      {
        return new StorageMappingItemCollection(edmItemCollection, storeCollection, (IEnumerable<XmlReader>) readers2, (IList<string>) loader.GetPaths(DataSpace.CSSpace));
      }
      finally
      {
        Helper.DisposeXmlReaders((IEnumerable<XmlReader>) readers2);
      }
    }

    private static EdmItemCollection LoadEdmItemCollection(
      MetadataArtifactLoader loader)
    {
      List<XmlReader> readers = loader.CreateReaders(DataSpace.CSpace);
      try
      {
        return new EdmItemCollection((IEnumerable<XmlReader>) readers, (IEnumerable<string>) loader.GetPaths(DataSpace.CSpace), false);
      }
      finally
      {
        Helper.DisposeXmlReaders((IEnumerable<XmlReader>) readers);
      }
    }

    private static bool ShouldRecalculateMetadataArtifactLoader(
      IEnumerable<MetadataArtifactLoader> loaders)
    {
      return loaders.Any<MetadataArtifactLoader>((Func<MetadataArtifactLoader, bool>) (loader => loader.GetType() == typeof (MetadataArtifactLoaderCompositeFile)));
    }

    private static string CreateMetadataCacheKey(IList<string> paths, string providerName)
    {
      int resultCount = 0;
      string result;
      MetadataCache.CreateMetadataCacheKeyWithCount(paths, providerName, false, ref resultCount, out result);
      MetadataCache.CreateMetadataCacheKeyWithCount(paths, providerName, true, ref resultCount, out result);
      return result;
    }

    private static void CreateMetadataCacheKeyWithCount(
      IList<string> paths,
      string providerName,
      bool buildResult,
      ref int resultCount,
      out string result)
    {
      StringBuilder stringBuilder = buildResult ? new StringBuilder(resultCount) : (StringBuilder) null;
      resultCount = 0;
      if (!string.IsNullOrEmpty(providerName))
      {
        resultCount += providerName.Length + 1;
        if (buildResult)
        {
          stringBuilder.Append(providerName);
          stringBuilder.Append(";");
        }
      }
      if (paths != null)
      {
        for (int index = 0; index < paths.Count; ++index)
        {
          if (paths[index].Length > 0)
          {
            if (index > 0)
            {
              ++resultCount;
              if (buildResult)
                stringBuilder.Append("|");
            }
            resultCount += paths[index].Length;
            if (buildResult)
              stringBuilder.Append(paths[index]);
          }
        }
        ++resultCount;
        if (buildResult)
          stringBuilder.Append(";");
      }
      result = buildResult ? stringBuilder.ToString() : (string) null;
    }
  }
}
