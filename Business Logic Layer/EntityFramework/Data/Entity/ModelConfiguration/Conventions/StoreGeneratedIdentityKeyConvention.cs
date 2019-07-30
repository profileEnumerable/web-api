// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.StoreGeneratedIdentityKeyConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to configure integer primary keys to be identity.
  /// </summary>
  public class StoreGeneratedIdentityKeyConvention : IConceptualModelConvention<EntityType>, IConvention
  {
    private static readonly IEnumerable<PrimitiveTypeKind> _applicableTypes = (IEnumerable<PrimitiveTypeKind>) new PrimitiveTypeKind[3]
    {
      PrimitiveTypeKind.Int16,
      PrimitiveTypeKind.Int32,
      PrimitiveTypeKind.Int64
    };

    /// <inheritdoc />
    public virtual void Apply(EntityType item, DbModel model)
    {
      Check.NotNull<EntityType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      if (item.BaseType != null || item.KeyProperties.Count != 1 || item.DeclaredProperties.Select(p => new
      {
        p = p,
        sgp = p.GetStoreGeneratedPattern()
      }).Where(_param0 =>
      {
        if (!_param0.sgp.HasValue)
          return false;
        StoreGeneratedPattern? sgp = _param0.sgp;
        if (sgp.GetValueOrDefault() == StoreGeneratedPattern.Identity)
          return sgp.HasValue;
        return false;
      }).Select(_param0 => _param0.sgp).Any<StoreGeneratedPattern?>())
        return;
      EdmProperty property = item.KeyProperties.Single<EdmProperty>();
      if (property.GetStoreGeneratedPattern().HasValue || property.PrimitiveType == null || !StoreGeneratedIdentityKeyConvention._applicableTypes.Contains<PrimitiveTypeKind>(property.PrimitiveType.PrimitiveTypeKind) || (model.ConceptualModel.AssociationTypes.Any<AssociationType>((Func<AssociationType, bool>) (a => StoreGeneratedIdentityKeyConvention.IsNonTableSplittingForeignKey(a, property))) || StoreGeneratedIdentityKeyConvention.ParentOfTpc(item, model.ConceptualModel)))
        return;
      property.SetStoreGeneratedPattern(StoreGeneratedPattern.Identity);
    }

    private static bool IsNonTableSplittingForeignKey(
      AssociationType association,
      EdmProperty property)
    {
      if (association.Constraint == null || !association.Constraint.ToProperties.Contains(property))
        return false;
      EntityTypeConfiguration configuration1 = (EntityTypeConfiguration) association.SourceEnd.GetEntityType().GetConfiguration();
      EntityTypeConfiguration configuration2 = (EntityTypeConfiguration) association.TargetEnd.GetEntityType().GetConfiguration();
      if (configuration1 != null && configuration2 != null && (configuration1.GetTableName() != null && configuration2.GetTableName() != null))
        return !configuration1.GetTableName().Equals(configuration2.GetTableName());
      return true;
    }

    private static bool ParentOfTpc(EntityType entityType, EdmModel model)
    {
      return model.EntityTypes.Where<EntityType>((Func<EntityType, bool>) (et => et.GetRootType() == entityType)).Select(e => new
      {
        e = e,
        configuration = e.GetConfiguration() as EntityTypeConfiguration
      }).Where(_param0 =>
      {
        if (_param0.configuration != null)
          return _param0.configuration.IsMappingAnyInheritedProperty(_param0.e);
        return false;
      }).Select(_param0 => _param0.e).Any<EntityType>();
    }
  }
}
