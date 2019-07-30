// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.ModelContainerConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// This <see cref="T:System.Data.Entity.DbModelBuilder" /> convention uses the name of the derived
  /// <see cref="T:System.Data.Entity.DbContext" /> class as the container for the conceptual model built by
  /// Code First.
  /// </summary>
  public class ModelContainerConvention : IConceptualModelConvention<EntityContainer>, IConvention
  {
    private readonly string _containerName;

    internal ModelContainerConvention(string containerName)
    {
      this._containerName = containerName;
    }

    /// <summary>Applies the convention to the given model.</summary>
    /// <param name="item"> The container to apply the convention to. </param>
    /// <param name="model"> The model. </param>
    public virtual void Apply(EntityContainer item, DbModel model)
    {
      Check.NotNull<DbModel>(model, nameof (model));
      item.Name = this._containerName;
    }
  }
}
