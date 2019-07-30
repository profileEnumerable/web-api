// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.MappingItem
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Base class for items in the mapping space (DataSpace.CSSpace)
  /// </summary>
  public abstract class MappingItem
  {
    private readonly List<MetadataProperty> _annotations = new List<MetadataProperty>();
    private bool _readOnly;

    internal bool IsReadOnly
    {
      get
      {
        return this._readOnly;
      }
    }

    internal IList<MetadataProperty> Annotations
    {
      get
      {
        return (IList<MetadataProperty>) this._annotations;
      }
    }

    internal virtual void SetReadOnly()
    {
      this._annotations.TrimExcess();
      this._readOnly = true;
    }

    internal void ThrowIfReadOnly()
    {
      if (this.IsReadOnly)
        throw new InvalidOperationException(Strings.OperationOnReadOnlyItem);
    }

    internal static void SetReadOnly(MappingItem item)
    {
      item?.SetReadOnly();
    }

    internal static void SetReadOnly(IEnumerable<MappingItem> items)
    {
      if (items == null)
        return;
      foreach (MappingItem mappingItem in items)
        MappingItem.SetReadOnly(mappingItem);
    }
  }
}
