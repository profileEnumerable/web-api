// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataArtifactLoaderFile
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.SchemaObjectModel;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MetadataArtifactLoaderFile : MetadataArtifactLoader, IComparable
  {
    private readonly bool _alreadyLoaded;
    private readonly string _path;

    public MetadataArtifactLoaderFile(string path, ICollection<string> uriRegistry)
    {
      this._path = path;
      this._alreadyLoaded = uriRegistry.Contains(this._path);
      if (this._alreadyLoaded)
        return;
      uriRegistry.Add(this._path);
    }

    public override string Path
    {
      get
      {
        return this._path;
      }
    }

    public int CompareTo(object obj)
    {
      MetadataArtifactLoaderFile artifactLoaderFile = obj as MetadataArtifactLoaderFile;
      if (artifactLoaderFile != null)
        return string.Compare(this._path, artifactLoaderFile._path, StringComparison.OrdinalIgnoreCase);
      return -1;
    }

    public override bool Equals(object obj)
    {
      return this.CompareTo(obj) == 0;
    }

    public override int GetHashCode()
    {
      return this._path.GetHashCode();
    }

    public override List<string> GetPaths(DataSpace spaceToGet)
    {
      List<string> stringList = new List<string>();
      if (!this._alreadyLoaded && MetadataArtifactLoader.IsArtifactOfDataSpace(this._path, spaceToGet))
        stringList.Add(this._path);
      return stringList;
    }

    public override List<string> GetPaths()
    {
      List<string> stringList = new List<string>();
      if (!this._alreadyLoaded)
        stringList.Add(this._path);
      return stringList;
    }

    public override List<XmlReader> GetReaders(
      Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      if (!this._alreadyLoaded)
      {
        XmlReader xmlReader = this.CreateXmlReader();
        xmlReaderList.Add(xmlReader);
        sourceDictionary?.Add((MetadataArtifactLoader) this, xmlReader);
      }
      return xmlReaderList;
    }

    public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      if (!this._alreadyLoaded && MetadataArtifactLoader.IsArtifactOfDataSpace(this._path, spaceToGet))
      {
        XmlReader xmlReader = this.CreateXmlReader();
        xmlReaderList.Add(xmlReader);
      }
      return xmlReaderList;
    }

    private XmlReader CreateXmlReader()
    {
      XmlReaderSettings xmlReaderSettings = Schema.CreateEdmStandardXmlReaderSettings();
      xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
      return XmlReader.Create(this._path, xmlReaderSettings);
    }
  }
}
