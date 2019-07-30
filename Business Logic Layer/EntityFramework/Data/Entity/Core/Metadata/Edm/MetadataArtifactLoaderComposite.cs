// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataArtifactLoaderComposite
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MetadataArtifactLoaderComposite : MetadataArtifactLoader, IEnumerable<MetadataArtifactLoader>, IEnumerable
  {
    private readonly ReadOnlyCollection<MetadataArtifactLoader> _children;

    public MetadataArtifactLoaderComposite(List<MetadataArtifactLoader> children)
    {
      this._children = new ReadOnlyCollection<MetadataArtifactLoader>((IList<MetadataArtifactLoader>) new List<MetadataArtifactLoader>((IEnumerable<MetadataArtifactLoader>) children));
    }

    public override string Path
    {
      get
      {
        return string.Empty;
      }
    }

    public override bool IsComposite
    {
      get
      {
        return true;
      }
    }

    public override List<string> GetOriginalPaths()
    {
      List<string> stringList = new List<string>();
      foreach (MetadataArtifactLoader child in this._children)
        stringList.AddRange((IEnumerable<string>) child.GetOriginalPaths());
      return stringList;
    }

    public override List<string> GetOriginalPaths(DataSpace spaceToGet)
    {
      List<string> stringList = new List<string>();
      foreach (MetadataArtifactLoader child in this._children)
        stringList.AddRange((IEnumerable<string>) child.GetOriginalPaths(spaceToGet));
      return stringList;
    }

    public override List<string> GetPaths(DataSpace spaceToGet)
    {
      List<string> stringList = new List<string>();
      foreach (MetadataArtifactLoader child in this._children)
        stringList.AddRange((IEnumerable<string>) child.GetPaths(spaceToGet));
      return stringList;
    }

    public override List<string> GetPaths()
    {
      List<string> stringList = new List<string>();
      foreach (MetadataArtifactLoader child in this._children)
        stringList.AddRange((IEnumerable<string>) child.GetPaths());
      return stringList;
    }

    public override List<XmlReader> GetReaders(
      Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      foreach (MetadataArtifactLoader child in this._children)
        xmlReaderList.AddRange((IEnumerable<XmlReader>) child.GetReaders(sourceDictionary));
      return xmlReaderList;
    }

    public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
    {
      List<XmlReader> xmlReaderList = new List<XmlReader>();
      foreach (MetadataArtifactLoader child in this._children)
        xmlReaderList.AddRange((IEnumerable<XmlReader>) child.CreateReaders(spaceToGet));
      return xmlReaderList;
    }

    public IEnumerator<MetadataArtifactLoader> GetEnumerator()
    {
      return this._children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._children.GetEnumerator();
    }
  }
}
