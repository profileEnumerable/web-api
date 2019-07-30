// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.Services.StructuralTypeMappingGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm.Services
{
  internal abstract class StructuralTypeMappingGenerator
  {
    protected readonly DbProviderManifest _providerManifest;

    protected StructuralTypeMappingGenerator(DbProviderManifest providerManifest)
    {
      this._providerManifest = providerManifest;
    }

    protected EdmProperty MapTableColumn(
      EdmProperty property,
      string columnName,
      bool isInstancePropertyOnDerivedType)
    {
      TypeUsage storeType = this._providerManifest.GetStoreType(TypeUsage.Create((EdmType) property.UnderlyingPrimitiveType, (IEnumerable<Facet>) property.TypeUsage.Facets));
      EdmProperty column = new EdmProperty(columnName, storeType)
      {
        Nullable = isInstancePropertyOnDerivedType || property.Nullable
      };
      if (column.IsPrimaryKeyColumn)
        column.Nullable = false;
      StoreGeneratedPattern? generatedPattern = property.GetStoreGeneratedPattern();
      if (generatedPattern.HasValue)
        column.StoreGeneratedPattern = generatedPattern.Value;
      StructuralTypeMappingGenerator.MapPrimitivePropertyFacets(property, column, storeType);
      return column;
    }

    internal static void MapPrimitivePropertyFacets(
      EdmProperty property,
      EdmProperty column,
      TypeUsage typeUsage)
    {
      if (StructuralTypeMappingGenerator.IsValidFacet(typeUsage, "FixedLength") && property.IsFixedLength.HasValue)
        column.IsFixedLength = property.IsFixedLength;
      if (StructuralTypeMappingGenerator.IsValidFacet(typeUsage, "MaxLength"))
      {
        column.IsMaxLength = property.IsMaxLength;
        if (!column.IsMaxLength || property.MaxLength.HasValue)
          column.MaxLength = property.MaxLength;
      }
      if (StructuralTypeMappingGenerator.IsValidFacet(typeUsage, "Unicode") && property.IsUnicode.HasValue)
        column.IsUnicode = property.IsUnicode;
      if (StructuralTypeMappingGenerator.IsValidFacet(typeUsage, "Precision"))
      {
        byte? precision = property.Precision;
        if ((precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
          column.Precision = property.Precision;
      }
      if (!StructuralTypeMappingGenerator.IsValidFacet(typeUsage, "Scale"))
        return;
      byte? scale = property.Scale;
      if (!(scale.HasValue ? new int?((int) scale.GetValueOrDefault()) : new int?()).HasValue)
        return;
      column.Scale = property.Scale;
    }

    private static bool IsValidFacet(TypeUsage typeUsage, string name)
    {
      Facet facet;
      if (typeUsage.Facets.TryGetValue(name, false, out facet))
        return !facet.Description.IsConstant;
      return false;
    }

    protected static EntityTypeMapping GetEntityTypeMappingInHierarchy(
      DbDatabaseMapping databaseMapping,
      EntityType entityType)
    {
      EntityTypeMapping entityTypeMapping = databaseMapping.GetEntityTypeMapping(entityType);
      if (entityTypeMapping == null)
      {
        EntitySetMapping entitySetMapping = databaseMapping.GetEntitySetMapping(databaseMapping.Model.GetEntitySet(entityType));
        if (entitySetMapping != null)
          entityTypeMapping = entitySetMapping.EntityTypeMappings.First<EntityTypeMapping>((Func<EntityTypeMapping, bool>) (etm => entityType.DeclaredProperties.All<EdmProperty>((Func<EdmProperty, bool>) (dp => etm.MappingFragments.First<MappingFragment>().ColumnMappings.Select<ColumnMappingBuilder, EdmProperty>((Func<ColumnMappingBuilder, EdmProperty>) (pm => pm.PropertyPath.First<EdmProperty>())).Contains<EdmProperty>(dp)))));
      }
      return entityTypeMapping;
    }
  }
}
