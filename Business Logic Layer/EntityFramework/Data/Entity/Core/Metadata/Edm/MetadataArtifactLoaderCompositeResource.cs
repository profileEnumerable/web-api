// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataArtifactLoaderCompositeResource
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Resources;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MetadataArtifactLoaderCompositeResource : MetadataArtifactLoader
  {
    private readonly ReadOnlyCollection<MetadataArtifactLoaderResource> _children;
    private readonly string _originalPath;

    internal MetadataArtifactLoaderCompositeResource(
      string originalPath,
      string assemblyName,
      string resourceName,
      ICollection<string> uriRegistry,
      MetadataArtifactAssemblyResolver resolver)
    {
      this._originalPath = originalPath;
      this._children = new ReadOnlyCollection<MetadataArtifactLoaderResource>((IList<MetadataArtifactLoaderResource>) MetadataArtifactLoaderCompositeResource.LoadResources(assemblyName, resourceName, uriRegistry, resolver));
    }

    public override string Path
    {
      get
      {
        return this._originalPath;
      }
    }

    public override bool IsComposite
    {
      get
      {
        return true;
      }
    }

    public override List<string> GetOriginalPaths(DataSpace spaceToGet)
    {
      return this.GetOriginalPaths();
    }

    public override List<string> GetPaths(DataSpace spaceToGet)
    {
      List<string> stringList = new List<string>();
      foreach (MetadataArtifactLoaderResource child in this._children)
        stringList.AddRange((IEnumerable<string>) child.GetPaths(spaceToGet));
      return stringList;
    }

    public override List<string> GetPaths()
    {
      List<string> stringList = new List<string>();
      foreach (MetadataArtifactLoaderResource child in this._children)
        stringList.AddRange((IEnumerable<string>) child.GetPaths());
      return stringList;
    }

    public override List<XmlReader> GetReaders(
      Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      foreach (MetadataArtifactLoaderResource child in this._children)
        xmlReaderList.AddRange((IEnumerable<XmlReader>) child.GetReaders(sourceDictionary));
      return xmlReaderList;
    }

    public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      foreach (MetadataArtifactLoaderResource child in this._children)
        xmlReaderList.AddRange((IEnumerable<XmlReader>) child.CreateReaders(spaceToGet));
      return xmlReaderList;
    }

    private static List<MetadataArtifactLoaderResource> LoadResources(
      string assemblyName,
      string resourceName,
      ICollection<string> uriRegistry,
      MetadataArtifactAssemblyResolver resolver)
    {
      List<MetadataArtifactLoaderResource> loaders = new List<MetadataArtifactLoaderResource>();
      if (assemblyName == MetadataArtifactLoader.wildcard)
      {
        foreach (Assembly wildcardAssembly in resolver.GetWildcardAssemblies())
        {
          if (MetadataArtifactLoaderCompositeResource.AssemblyContainsResource(wildcardAssembly, ref resourceName))
            MetadataArtifactLoaderCompositeResource.LoadResourcesFromAssembly(wildcardAssembly, resourceName, uriRegistry, loaders);
        }
      }
      else
        MetadataArtifactLoaderCompositeResource.LoadResourcesFromAssembly(MetadataArtifactLoaderCompositeResource.ResolveAssemblyName(assemblyName, resolver), resourceName, uriRegistry, loaders);
      if (resourceName != null && loaders.Count == 0)
        throw new System.Data.Entity.Core.MetadataException(Strings.UnableToLoadResource);
      return loaders;
    }

    private static bool AssemblyContainsResource(Assembly assembly, ref string resourceName)
    {
      if (resourceName == null)
        return true;
      foreach (string b in MetadataArtifactLoaderCompositeResource.GetManifestResourceNamesForAssembly(assembly))
      {
        if (string.Equals(resourceName, b, StringComparison.OrdinalIgnoreCase))
        {
          resourceName = b;
          return true;
        }
      }
      return false;
    }

    private static void LoadResourcesFromAssembly(
      Assembly assembly,
      string resourceName,
      ICollection<string> uriRegistry,
      List<MetadataArtifactLoaderResource> loaders)
    {
      if (resourceName == null)
      {
        MetadataArtifactLoaderCompositeResource.LoadAllResourcesFromAssembly(assembly, uriRegistry, loaders);
      }
      else
      {
        if (!MetadataArtifactLoaderCompositeResource.AssemblyContainsResource(assembly, ref resourceName))
          throw new System.Data.Entity.Core.MetadataException(Strings.UnableToLoadResource);
        MetadataArtifactLoaderCompositeResource.CreateAndAddSingleResourceLoader(assembly, resourceName, uriRegistry, loaders);
      }
    }

    private static void LoadAllResourcesFromAssembly(
      Assembly assembly,
      ICollection<string> uriRegistry,
      List<MetadataArtifactLoaderResource> loaders)
    {
      foreach (string resourceName in MetadataArtifactLoaderCompositeResource.GetManifestResourceNamesForAssembly(assembly))
        MetadataArtifactLoaderCompositeResource.CreateAndAddSingleResourceLoader(assembly, resourceName, uriRegistry, loaders);
    }

    private static void CreateAndAddSingleResourceLoader(
      Assembly assembly,
      string resourceName,
      ICollection<string> uriRegistry,
      List<MetadataArtifactLoaderResource> loaders)
    {
      string resPath = MetadataArtifactLoaderCompositeResource.CreateResPath(assembly, resourceName);
      if (uriRegistry.Contains(resPath))
        return;
      loaders.Add(new MetadataArtifactLoaderResource(assembly, resourceName, uriRegistry));
    }

    internal static string CreateResPath(Assembly assembly, string resourceName)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) MetadataArtifactLoader.resPathPrefix, (object) assembly.FullName, (object) MetadataArtifactLoader.resPathSeparator, (object) resourceName);
    }

    internal static string[] GetManifestResourceNamesForAssembly(Assembly assembly)
    {
      if (assembly.IsDynamic)
        return new string[0];
      return assembly.GetManifestResourceNames();
    }

    private static Assembly ResolveAssemblyName(
      string assemblyName,
      MetadataArtifactAssemblyResolver resolver)
    {
      AssemblyName refernceName = new AssemblyName(assemblyName);
      Assembly assembly;
      if (!resolver.TryResolveAssemblyReference(refernceName, out assembly))
        throw new FileNotFoundException(Strings.UnableToResolveAssembly((object) assemblyName));
      return assembly;
    }

    internal static MetadataArtifactLoader CreateResourceLoader(
      string path,
      MetadataArtifactLoader.ExtensionCheck extensionCheck,
      string validExtension,
      ICollection<string> uriRegistry,
      MetadataArtifactAssemblyResolver resolver)
    {
      string assemblyName = (string) null;
      string resourceName = (string) null;
      MetadataArtifactLoaderCompositeResource.ParseResourcePath(path, out assemblyName, out resourceName);
      bool flag = assemblyName != null && (resourceName == null || assemblyName.Trim() == MetadataArtifactLoader.wildcard);
      MetadataArtifactLoaderCompositeResource.ValidateExtension(extensionCheck, validExtension, resourceName);
      if (flag)
        return (MetadataArtifactLoader) new MetadataArtifactLoaderCompositeResource(path, assemblyName, resourceName, uriRegistry, resolver);
      return (MetadataArtifactLoader) new MetadataArtifactLoaderResource(MetadataArtifactLoaderCompositeResource.ResolveAssemblyName(assemblyName, resolver), resourceName, uriRegistry);
    }

    private static void ValidateExtension(
      MetadataArtifactLoader.ExtensionCheck extensionCheck,
      string validExtension,
      string resourceName)
    {
      if (resourceName == null)
        return;
      switch (extensionCheck)
      {
        case MetadataArtifactLoader.ExtensionCheck.Specific:
          MetadataArtifactLoader.CheckArtifactExtension(resourceName, validExtension);
          break;
        case MetadataArtifactLoader.ExtensionCheck.All:
          if (MetadataArtifactLoader.IsValidArtifact(resourceName))
            break;
          throw new System.Data.Entity.Core.MetadataException(Strings.InvalidMetadataPath);
      }
    }

    private static void ParseResourcePath(
      string path,
      out string assemblyName,
      out string resourceName)
    {
      int length = MetadataArtifactLoader.resPathPrefix.Length;
      string[] strArray = path.Substring(length).Split(new string[2]
      {
        MetadataArtifactLoader.resPathSeparator,
        MetadataArtifactLoader.altPathSeparator
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0 || strArray.Length > 2)
        throw new System.Data.Entity.Core.MetadataException(Strings.InvalidMetadataPath);
      assemblyName = strArray.Length < 1 ? (string) null : strArray[0];
      if (strArray.Length == 2)
        resourceName = strArray[1];
      else
        resourceName = (string) null;
    }
  }
}
