// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.ColumnMappingBuilderExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class ColumnMappingBuilderExtensions
  {
    public static void SyncNullabilityCSSpace(
      this ColumnMappingBuilder propertyMappingBuilder,
      DbDatabaseMapping databaseMapping,
      IEnumerable<EntitySet> entitySets,
      EntityType toTable)
    {
      EdmProperty edmProperty = propertyMappingBuilder.PropertyPath.Last<EdmProperty>();
      EntitySetMapping entitySetMapping = (EntitySetMapping) null;
      EntityType baseType = (EntityType) edmProperty.DeclaringType.BaseType;
      if (baseType != null)
        entitySetMapping = ColumnMappingBuilderExtensions.GetEntitySetMapping(databaseMapping, baseType, entitySets);
      for (; baseType != null; baseType = (EntityType) baseType.BaseType)
      {
        if (toTable == entitySetMapping.EntityTypeMappings.First<EntityTypeMapping>((Func<EntityTypeMapping, bool>) (m => m.EntityType == baseType)).GetPrimaryTable())
          return;
      }
      propertyMappingBuilder.ColumnProperty.Nullable = edmProperty.Nullable;
    }

    private static EntitySetMapping GetEntitySetMapping(
      DbDatabaseMapping databaseMapping,
      EntityType cSpaceEntityType,
      IEnumerable<EntitySet> entitySets)
    {
      while (cSpaceEntityType.BaseType != null)
        cSpaceEntityType = (EntityType) cSpaceEntityType.BaseType;
      EntitySet cSpaceEntitySet = entitySets.First<EntitySet>((Func<EntitySet, bool>) (s => s.ElementType == cSpaceEntityType));
      return databaseMapping.EntityContainerMappings.First<EntityContainerMapping>().EntitySetMappings.First<EntitySetMapping>((Func<EntitySetMapping, bool>) (m => m.EntitySet == cSpaceEntitySet));
    }
  }
}
