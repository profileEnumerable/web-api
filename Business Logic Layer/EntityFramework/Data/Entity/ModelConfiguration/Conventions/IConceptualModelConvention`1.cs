// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.IConceptualModelConvention`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// A convention that operates on the conceptual section of the model after the model is created.
  /// </summary>
  /// <typeparam name="T">The type of metadata item that this convention operates on.</typeparam>
  public interface IConceptualModelConvention<T> : IConvention where T : MetadataItem
  {
    /// <summary>Applies this convention to an item in the model.</summary>
    /// <param name="item">The item to apply the convention to.</param>
    /// <param name="model">The model.</param>
    void Apply(T item, DbModel model);
  }
}
