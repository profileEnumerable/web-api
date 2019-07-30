// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to set the table name to be a pluralized version of the entity type name.
  /// </summary>
  public class PluralizingTableNameConvention : IStoreModelConvention<EntityType>, IConvention
  {
    private IPluralizationService _pluralizationService = DbConfiguration.DependencyResolver.GetService<IPluralizationService>();

    /// <inheritdoc />
    public virtual void Apply(EntityType item, DbModel model)
    {
      Check.NotNull<EntityType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      this._pluralizationService = DbConfiguration.DependencyResolver.GetService<IPluralizationService>();
      if (item.GetTableName() != null)
        return;
      EntitySet entitySet = model.StoreModel.GetEntitySet(item);
      entitySet.Table = model.StoreModel.GetEntitySets().Where<EntitySet>((Func<EntitySet, bool>) (es => es.Schema == entitySet.Schema)).Except<EntitySet>((IEnumerable<EntitySet>) new EntitySet[1]
      {
        entitySet
      }).Select<EntitySet, string>((Func<EntitySet, string>) (n => n.Table)).Uniquify(this._pluralizationService.Pluralize(entitySet.Table));
    }
  }
}
