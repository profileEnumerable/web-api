// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.IEdmModelAdapter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// An interface to get the underlying store and conceptual model for a <see cref="T:System.Data.Entity.Infrastructure.DbModel" />.
  /// </summary>
  [Obsolete("ConceptualModel and StoreModel are now available as properties directly on DbModel.")]
  public interface IEdmModelAdapter
  {
    /// <summary>Gets the conceptual model.</summary>
    [Obsolete("ConceptualModel is now available as a property directly on DbModel.")]
    EdmModel ConceptualModel { get; }

    /// <summary>Gets the store model.</summary>
    [Obsolete("StoreModel is now available as a property directly on DbModel.")]
    EdmModel StoreModel { get; }
  }
}
