// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataArtifactLoaderCompositeFile
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MetadataArtifactLoaderCompositeFile : MetadataArtifactLoader
  {
    private ReadOnlyCollection<MetadataArtifactLoaderFile> _csdlChildren;
    private ReadOnlyCollection<MetadataArtifactLoaderFile> _ssdlChildren;
    private ReadOnlyCollection<MetadataArtifactLoaderFile> _mslChildren;
    private readonly string _path;
    private readonly ICollection<string> _uriRegistry;

    public MetadataArtifactLoaderCompositeFile(string path, ICollection<string> uriRegistry)
    {
      this._path = path;
      this._uriRegistry = uriRegistry;
    }

    public override string Path
    {
      get
      {
        return this._path;
      }
    }

    public override bool IsComposite
    {
      get
      {
        return true;
      }
    }

    internal ReadOnlyCollection<MetadataArtifactLoaderFile> CsdlChildren
    {
      get
      {
        this.LoadCollections();
        return this._csdlChildren;
      }
    }

    internal ReadOnlyCollection<MetadataArtifactLoaderFile> SsdlChildren
    {
      get
      {
        this.LoadCollections();
        return this._ssdlChildren;
      }
    }

    internal ReadOnlyCollection<MetadataArtifactLoaderFile> MslChildren
    {
      get
      {
        this.LoadCollections();
        return this._mslChildren;
      }
    }

    private void LoadCollections()
    {
      if (this._csdlChildren == null)
        Interlocked.CompareExchange<ReadOnlyCollection<MetadataArtifactLoaderFile>>(ref this._csdlChildren, new ReadOnlyCollection<MetadataArtifactLoaderFile>((IList<MetadataArtifactLoaderFile>) MetadataArtifactLoaderCompositeFile.GetArtifactsInDirectory(this._path, ".csdl", this._uriRegistry)), (ReadOnlyCollection<MetadataArtifactLoaderFile>) null);
      if (this._ssdlChildren == null)
        Interlocked.CompareExchange<ReadOnlyCollection<MetadataArtifactLoaderFile>>(ref this._ssdlChildren, new ReadOnlyCollection<MetadataArtifactLoaderFile>((IList<MetadataArtifactLoaderFile>) MetadataArtifactLoaderCompositeFile.GetArtifactsInDirectory(this._path, ".ssdl", this._uriRegistry)), (ReadOnlyCollection<MetadataArtifactLoaderFile>) null);
      if (this._mslChildren != null)
        return;
      Interlocked.CompareExchange<ReadOnlyCollection<MetadataArtifactLoaderFile>>(ref this._mslChildren, new ReadOnlyCollection<MetadataArtifactLoaderFile>((IList<MetadataArtifactLoaderFile>) MetadataArtifactLoaderCompositeFile.GetArtifactsInDirectory(this._path, ".msl", this._uriRegistry)), (ReadOnlyCollection<MetadataArtifactLoaderFile>) null);
    }

    public override List<string> GetOriginalPaths(DataSpace spaceToGet)
    {
      return this.GetOriginalPaths();
    }

    public override List<string> GetPaths(DataSpace spaceToGet)
    {
      List<string> stringList = new List<string>();
      IList<MetadataArtifactLoaderFile> files;
      if (!this.TryGetListForSpace(spaceToGet, out files))
        return stringList;
      foreach (MetadataArtifactLoaderFile artifactLoaderFile in (IEnumerable<MetadataArtifactLoaderFile>) files)
        stringList.AddRange((IEnumerable<string>) artifactLoaderFile.GetPaths(spaceToGet));
      return stringList;
    }

    private bool TryGetListForSpace(
      DataSpace spaceToGet,
      out IList<MetadataArtifactLoaderFile> files)
    {
      switch (spaceToGet)
      {
        case DataSpace.CSpace:
          files = (IList<MetadataArtifactLoaderFile>) this.CsdlChildren;
          return true;
        case DataSpace.SSpace:
          files = (IList<MetadataArtifactLoaderFile>) this.SsdlChildren;
          return true;
        case DataSpace.CSSpace:
          files = (IList<MetadataArtifactLoaderFile>) this.MslChildren;
          return true;
        default:
          files = (IList<MetadataArtifactLoaderFile>) null;
          return false;
      }
    }

    public override List<string> GetPaths()
    {
      List<string> stringList = new List<string>();
      foreach (MetadataArtifactLoaderFile csdlChild in this.CsdlChildren)
        stringList.AddRange((IEnumerable<string>) csdlChild.GetPaths());
      foreach (MetadataArtifactLoaderFile ssdlChild in this.SsdlChildren)
        stringList.AddRange((IEnumerable<string>) ssdlChild.GetPaths());
      foreach (MetadataArtifactLoaderFile mslChild in this.MslChildren)
        stringList.AddRange((IEnumerable<string>) mslChild.GetPaths());
      return stringList;
    }

    public override List<XmlReader> GetReaders(
      Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      foreach (MetadataArtifactLoaderFile csdlChild in this.CsdlChildren)
        xmlReaderList.AddRange((IEnumerable<XmlReader>) csdlChild.GetReaders(sourceDictionary));
      foreach (MetadataArtifactLoaderFile ssdlChild in this.SsdlChildren)
        xmlReaderList.AddRange((IEnumerable<XmlReader>) ssdlChild.GetReaders(sourceDictionary));
      foreach (MetadataArtifactLoaderFile mslChild in this.MslChildren)
        xmlReaderList.AddRange((IEnumerable<XmlReader>) mslChild.GetReaders(sourceDictionary));
      return xmlReaderList;
    }

    public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      IList<MetadataArtifactLoaderFile> files;
      if (!this.TryGetListForSpace(spaceToGet, out files))
        return xmlReaderList;
      foreach (MetadataArtifactLoaderFile artifactLoaderFile in (IEnumerable<MetadataArtifactLoaderFile>) files)
        xmlReaderList.AddRange((IEnumerable<XmlReader>) artifactLoaderFile.CreateReaders(spaceToGet));
      return xmlReaderList;
    }

    private static List<MetadataArtifactLoaderFile> GetArtifactsInDirectory(
      string directory,
      string extension,
      ICollection<string> uriRegistry)
    {
      List<MetadataArtifactLoaderFile> artifactLoaderFileList = new List<MetadataArtifactLoaderFile>();
      foreach (string file in Directory.GetFiles(directory, MetadataArtifactLoader.wildcard + extension, SearchOption.TopDirectoryOnly))
      {
        string path = System.IO.Path.Combine(directory, file);
        if (!uriRegistry.Contains(path) && file.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
          artifactLoaderFileList.Add(new MetadataArtifactLoaderFile(path, uriRegistry));
      }
      return artifactLoaderFileList;
    }
  }
}
