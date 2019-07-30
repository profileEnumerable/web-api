// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.MappingItemCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Class for representing a collection of mapping items in Edm space.
  /// </summary>
  public abstract class MappingItemCollection : ItemCollection
  {
    internal MappingItemCollection(DataSpace dataSpace)
      : base(dataSpace)
    {
    }

    internal virtual bool TryGetMap(string identity, DataSpace typeSpace, out MappingBase map)
    {
      throw Error.NotSupported();
    }

    internal virtual MappingBase GetMap(GlobalItem item)
    {
      throw Error.NotSupported();
    }

    internal virtual bool TryGetMap(GlobalItem item, out MappingBase map)
    {
      throw Error.NotSupported();
    }

    internal virtual MappingBase GetMap(
      string identity,
      DataSpace typeSpace,
      bool ignoreCase)
    {
      throw Error.NotSupported();
    }

    internal virtual bool TryGetMap(
      string identity,
      DataSpace typeSpace,
      bool ignoreCase,
      out MappingBase map)
    {
      throw Error.NotSupported();
    }

    internal virtual MappingBase GetMap(string identity, DataSpace typeSpace)
    {
      throw Error.NotSupported();
    }
  }
}
