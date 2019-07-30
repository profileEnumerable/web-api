// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.DataModelErrorEventArgs
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Information about an error that occurred processing an Entity Framework model.
  /// </summary>
  [Serializable]
  public class DataModelErrorEventArgs : EventArgs
  {
    [NonSerialized]
    private MetadataItem _item;

    /// <summary>
    /// Gets an optional value indicating which property of the source item caused the event to be raised.
    /// </summary>
    public string PropertyName { get; internal set; }

    /// <summary>
    /// Gets an optional descriptive message the describes the error that is being raised.
    /// </summary>
    public string ErrorMessage { get; internal set; }

    /// <summary>
    /// Gets a value indicating the <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataItem" /> that caused the event to be raised.
    /// </summary>
    public MetadataItem Item
    {
      get
      {
        return this._item;
      }
      set
      {
        this._item = value;
      }
    }
  }
}
