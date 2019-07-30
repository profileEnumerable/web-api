// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataArtifactLoader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Resources;
using System.IO;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal abstract class MetadataArtifactLoader
  {
    protected static readonly string resPathPrefix = "res://";
    protected static readonly string resPathSeparator = "/";
    protected static readonly string altPathSeparator = "\\";
    protected static readonly string wildcard = "*";

    public abstract string Path { get; }

    public static MetadataArtifactLoader Create(
      string path,
      MetadataArtifactLoader.ExtensionCheck extensionCheck,
      string validExtension,
      ICollection<string> uriRegistry)
    {
      return MetadataArtifactLoader.Create(path, extensionCheck, validExtension, uriRegistry, (MetadataArtifactAssemblyResolver) new DefaultAssemblyResolver());
    }

    internal static MetadataArtifactLoader Create(
      string path,
      MetadataArtifactLoader.ExtensionCheck extensionCheck,
      string validExtension,
      ICollection<string> uriRegistry,
      MetadataArtifactAssemblyResolver resolver)
    {
      if (MetadataArtifactLoader.PathStartsWithResPrefix(path))
        return MetadataArtifactLoaderCompositeResource.CreateResourceLoader(path, extensionCheck, validExtension, uriRegistry, resolver);
      string str = MetadataArtifactLoader.NormalizeFilePaths(path);
      if (Directory.Exists(str))
        return (MetadataArtifactLoader) new MetadataArtifactLoaderCompositeFile(str, uriRegistry);
      if (!File.Exists(str))
        throw new MetadataException(Strings.InvalidMetadataPath);
      switch (extensionCheck)
      {
        case MetadataArtifactLoader.ExtensionCheck.Specific:
          MetadataArtifactLoader.CheckArtifactExtension(str, validExtension);
          break;
        case MetadataArtifactLoader.ExtensionCheck.All:
          if (!MetadataArtifactLoader.IsValidArtifact(str))
            throw new MetadataException(Strings.InvalidMetadataPath);
          break;
      }
      return (MetadataArtifactLoader) new MetadataArtifactLoaderFile(str, uriRegistry);
    }

    public static MetadataArtifactLoader Create(
      List<MetadataArtifactLoader> allCollections)
    {
      return (MetadataArtifactLoader) new MetadataArtifactLoaderComposite(allCollections);
    }

    public static MetadataArtifactLoader CreateCompositeFromFilePaths(
      IEnumerable<string> filePaths,
      string validExtension)
    {
      return MetadataArtifactLoader.CreateCompositeFromFilePaths(filePaths, validExtension, (MetadataArtifactAssemblyResolver) new DefaultAssemblyResolver());
    }

    internal static MetadataArtifactLoader CreateCompositeFromFilePaths(
      IEnumerable<string> filePaths,
      string validExtension,
      MetadataArtifactAssemblyResolver resolver)
    {
      MetadataArtifactLoader.ExtensionCheck extensionCheck = !string.IsNullOrEmpty(validExtension) ? MetadataArtifactLoader.ExtensionCheck.Specific : MetadataArtifactLoader.ExtensionCheck.All;
      List<MetadataArtifactLoader> allCollections = new List<MetadataArtifactLoader>();
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string filePath in filePaths)
      {
        if (string.IsNullOrEmpty(filePath))
          throw new MetadataException(Strings.NotValidInputPath, (Exception) new ArgumentException(Strings.ADP_CollectionParameterElementIsNullOrEmpty((object) nameof (filePaths))));
        string path = filePath.Trim();
        if (path.Length > 0)
          allCollections.Add(MetadataArtifactLoader.Create(path, extensionCheck, validExtension, (ICollection<string>) stringSet, resolver));
      }
      return MetadataArtifactLoader.Create(allCollections);
    }

    public static MetadataArtifactLoader CreateCompositeFromXmlReaders(
      IEnumerable<XmlReader> xmlReaders)
    {
      List<MetadataArtifactLoader> allCollections = new List<MetadataArtifactLoader>();
      foreach (XmlReader xmlReader in xmlReaders)
      {
        if (xmlReader == null)
          throw new ArgumentException(Strings.ADP_CollectionParameterElementIsNull((object) nameof (xmlReaders)));
        allCollections.Add((MetadataArtifactLoader) new MetadataArtifactLoaderXmlReaderWrapper(xmlReader));
      }
      return MetadataArtifactLoader.Create(allCollections);
    }

    internal static void CheckArtifactExtension(string path, string validExtension)
    {
      string extension = MetadataArtifactLoader.GetExtension(path);
      if (!extension.Equals(validExtension, StringComparison.OrdinalIgnoreCase))
        throw new MetadataException(Strings.InvalidFileExtension((object) path, (object) extension, (object) validExtension));
    }

    public virtual List<string> GetOriginalPaths()
    {
      return new List<string>((IEnumerable<string>) new string[1]
      {
        this.Path
      });
    }

    public virtual List<string> GetOriginalPaths(DataSpace spaceToGet)
    {
      List<string> stringList = new List<string>();
      if (MetadataArtifactLoader.IsArtifactOfDataSpace(this.Path, spaceToGet))
        stringList.Add(this.Path);
      return stringList;
    }

    public virtual bool IsComposite
    {
      get
      {
        return false;
      }
    }

    public abstract List<string> GetPaths();

    public abstract List<string> GetPaths(DataSpace spaceToGet);

    public List<XmlReader> GetReaders()
    {
      return this.GetReaders((Dictionary<MetadataArtifactLoader, XmlReader>) null);
    }

    public abstract List<XmlReader> GetReaders(
      Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary);

    public abstract List<XmlReader> CreateReaders(DataSpace spaceToGet);

    internal static bool PathStartsWithResPrefix(string path)
    {
      return path.StartsWith(MetadataArtifactLoader.resPathPrefix, StringComparison.OrdinalIgnoreCase);
    }

    protected static bool IsCSpaceArtifact(string resource)
    {
      string extension = MetadataArtifactLoader.GetExtension(resource);
      if (!string.IsNullOrEmpty(extension))
        return string.Compare(extension, ".csdl", StringComparison.OrdinalIgnoreCase) == 0;
      return false;
    }

    protected static bool IsSSpaceArtifact(string resource)
    {
      string extension = MetadataArtifactLoader.GetExtension(resource);
      if (!string.IsNullOrEmpty(extension))
        return string.Compare(extension, ".ssdl", StringComparison.OrdinalIgnoreCase) == 0;
      return false;
    }

    protected static bool IsCSSpaceArtifact(string resource)
    {
      string extension = MetadataArtifactLoader.GetExtension(resource);
      if (!string.IsNullOrEmpty(extension))
        return string.Compare(extension, ".msl", StringComparison.OrdinalIgnoreCase) == 0;
      return false;
    }

    private static string GetExtension(string resource)
    {
      if (string.IsNullOrEmpty(resource))
        return string.Empty;
      int startIndex = resource.LastIndexOf('.');
      if (startIndex < 0)
        return string.Empty;
      return resource.Substring(startIndex);
    }

    internal static bool IsValidArtifact(string resource)
    {
      string extension = MetadataArtifactLoader.GetExtension(resource);
      if (string.IsNullOrEmpty(extension))
        return false;
      if (string.Compare(extension, ".csdl", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(extension, ".ssdl", StringComparison.OrdinalIgnoreCase) != 0)
        return string.Compare(extension, ".msl", StringComparison.OrdinalIgnoreCase) == 0;
      return true;
    }

    protected static bool IsArtifactOfDataSpace(string resource, DataSpace dataSpace)
    {
      switch (dataSpace)
      {
        case DataSpace.CSpace:
          return MetadataArtifactLoader.IsCSpaceArtifact(resource);
        case DataSpace.SSpace:
          return MetadataArtifactLoader.IsSSpaceArtifact(resource);
        case DataSpace.CSSpace:
          return MetadataArtifactLoader.IsCSSpaceArtifact(resource);
        default:
          return false;
      }
    }

    internal static string NormalizeFilePaths(string path)
    {
      bool flag = true;
      if (!string.IsNullOrEmpty(path))
      {
        path = path.Trim();
        if (path.StartsWith("~", StringComparison.Ordinal))
        {
          path = new AspProxy().MapWebPath(path);
          flag = false;
        }
        if (path.Length == 2 && (int) path[1] == (int) System.IO.Path.VolumeSeparatorChar)
        {
          path += (string) (object) System.IO.Path.DirectorySeparatorChar;
        }
        else
        {
          string str = DbProviderServices.ExpandDataDirectory(path);
          if (!path.Equals(str, StringComparison.Ordinal))
          {
            path = str;
            flag = false;
          }
        }
      }
      try
      {
        if (flag)
          path = System.IO.Path.GetFullPath(path);
      }
      catch (ArgumentException ex)
      {
        throw new MetadataException(Strings.NotValidInputPath, (Exception) ex);
      }
      catch (NotSupportedException ex)
      {
        throw new MetadataException(Strings.NotValidInputPath, (Exception) ex);
      }
      catch (PathTooLongException ex)
      {
        throw new MetadataException(Strings.NotValidInputPath);
      }
      return path;
    }

    public enum ExtensionCheck
    {
      /// <summary>Do not perform any extension check</summary>
      None,
      /// <summary>Check the extension against a specific value</summary>
      Specific,
      /// <summary>
      /// Check the extension against the set of acceptable extensions
      /// </summary>
      All,
    }
  }
}
