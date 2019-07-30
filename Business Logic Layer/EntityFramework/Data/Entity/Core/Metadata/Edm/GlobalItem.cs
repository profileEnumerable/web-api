// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.GlobalItem
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Represents the base item class for all the metadata</summary>
  public abstract class GlobalItem : MetadataItem
  {
    internal GlobalItem()
    {
    }

    internal GlobalItem(MetadataItem.MetadataFlags flags)
      : base(flags)
    {
    }

    [MetadataProperty(typeof (DataSpace), false)]
    internal virtual DataSpace DataSpace
    {
      get
      {
        return this.GetDataSpace();
      }
      set
      {
        this.SetDataSpace(value);
      }
    }
  }
}
