// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.KeyDiscoveryConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Base class for conventions that discover primary key properties.
  /// </summary>
  public abstract class KeyDiscoveryConvention : IConceptualModelConvention<EntityType>, IConvention
  {
    /// <inheritdoc />
    public virtual void Apply(EntityType item, DbModel model)
    {
      Check.NotNull<EntityType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      if (item.KeyProperties.Count > 0 || item.BaseType != null)
        return;
      foreach (EdmProperty edmProperty in this.MatchKeyProperty(item, item.GetDeclaredPrimitiveProperties()))
      {
        edmProperty.Nullable = false;
        item.AddKeyMember((EdmMember) edmProperty);
      }
    }

    /// <summary>
    /// When overriden returns the subset of properties that will be part of the primary key.
    /// </summary>
    /// <param name="entityType"> The entity type. </param>
    /// <param name="primitiveProperties"> The primitive types of the entities</param>
    /// <returns> The properties that should be part of the primary key. </returns>
    protected abstract IEnumerable<EdmProperty> MatchKeyProperty(
      EntityType entityType,
      IEnumerable<EdmProperty> primitiveProperties);
  }
}
