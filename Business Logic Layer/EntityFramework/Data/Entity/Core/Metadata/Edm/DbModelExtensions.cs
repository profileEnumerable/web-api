// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.DbModelExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Extension methods for <see cref="T:System.Data.Entity.Infrastructure.DbModel" />.
  /// </summary>
  [Obsolete("ConceptualModel and StoreModel are now available as properties directly on DbModel.")]
  public static class DbModelExtensions
  {
    /// <summary>Gets the conceptual model from the specified DbModel.</summary>
    /// <param name="model">An instance of a class that implements IEdmModelAdapter (ex. DbModel).</param>
    /// <returns>An instance of EdmModel that represents the conceptual model.</returns>
    [Obsolete("ConceptualModel is now available as a property directly on DbModel.")]
    public static EdmModel GetConceptualModel(this IEdmModelAdapter model)
    {
      Check.NotNull<IEdmModelAdapter>(model, nameof (model));
      return model.ConceptualModel;
    }

    /// <summary>Gets the store model from the specified DbModel.</summary>
    /// <param name="model">An instance of a class that implements IEdmModelAdapter (ex. DbModel).</param>
    /// <returns>An instance of EdmModel that represents the store model.</returns>
    [Obsolete("StoreModel is now available as a property directly on DbModel.")]
    public static EdmModel GetStoreModel(this IEdmModelAdapter model)
    {
      Check.NotNull<IEdmModelAdapter>(model, nameof (model));
      return model.StoreModel;
    }
  }
}
