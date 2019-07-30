// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataArtifactLoaderXmlReaderWrapper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MetadataArtifactLoaderXmlReaderWrapper : MetadataArtifactLoader, IComparable
  {
    private readonly XmlReader _reader;
    private readonly string _resourceUri;

    public MetadataArtifactLoaderXmlReaderWrapper(XmlReader xmlReader)
    {
      this._reader = xmlReader;
      this._resourceUri = xmlReader.BaseURI;
    }

    public override string Path
    {
      get
      {
        if (string.IsNullOrEmpty(this._resourceUri))
          return string.Empty;
        return this._resourceUri;
      }
    }

    public int CompareTo(object obj)
    {
      MetadataArtifactLoaderXmlReaderWrapper xmlReaderWrapper = obj as MetadataArtifactLoaderXmlReaderWrapper;
      return xmlReaderWrapper != null && object.ReferenceEquals((object) this._reader, (object) xmlReaderWrapper._reader) ? 0 : -1;
    }

    public override bool Equals(object obj)
    {
      return this.CompareTo(obj) == 0;
    }

    public override int GetHashCode()
    {
      return this._reader.GetHashCode();
    }

    public override List<string> GetPaths(DataSpace spaceToGet)
    {
      List<string> stringList = new List<string>();
      if (MetadataArtifactLoader.IsArtifactOfDataSpace(this.Path, spaceToGet))
        stringList.Add(this.Path);
      return stringList;
    }

    public override List<string> GetPaths()
    {
      return new List<string>((IEnumerable<string>) new string[1]
      {
        this.Path
      });
    }

    public override List<XmlReader> GetReaders(
      Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      xmlReaderList.Add(this._reader);
      sourceDictionary?.Add((MetadataArtifactLoader) this, this._reader);
      return xmlReaderList;
    }

    public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      if (MetadataArtifactLoader.IsArtifactOfDataSpace(this.Path, spaceToGet))
        xmlReaderList.Add(this._reader);
      return xmlReaderList;
    }
  }
}
