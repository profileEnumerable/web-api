// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Mapping.EntityMappingOperations
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration.Mapping
{
  internal static class EntityMappingOperations
  {
    public static MappingFragment CreateTypeMappingFragment(
      EntityTypeMapping entityTypeMapping,
      MappingFragment templateFragment,
      EntitySet tableSet)
    {
      MappingFragment fragment = new MappingFragment(tableSet, (TypeMapping) entityTypeMapping, false);
      entityTypeMapping.AddFragment(fragment);
      foreach (ColumnMappingBuilder propertyMappingBuilder in templateFragment.ColumnMappings.Where<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => pm.ColumnProperty.IsPrimaryKeyColumn)))
        EntityMappingOperations.CopyPropertyMappingToFragment(propertyMappingBuilder, fragment, TablePrimitiveOperations.GetNameMatcher(propertyMappingBuilder.ColumnProperty.Name), true);
      return fragment;
    }

    private static void UpdatePropertyMapping(
      DbDatabaseMapping databaseMapping,
      IEnumerable<EntitySet> entitySets,
      Dictionary<EdmProperty, IList<ColumnMappingBuilder>> columnMappingIndex,
      ColumnMappingBuilder propertyMappingBuilder,
      EntityType fromTable,
      EntityType toTable,
      bool useExisting)
    {
      propertyMappingBuilder.ColumnProperty = TableOperations.CopyColumnAndAnyConstraints(databaseMapping.Database, fromTable, toTable, propertyMappingBuilder.ColumnProperty, EntityMappingOperations.GetPropertyPathMatcher(columnMappingIndex, propertyMappingBuilder), useExisting);
      propertyMappingBuilder.SyncNullabilityCSSpace(databaseMapping, entitySets, toTable);
    }

    private static Func<EdmProperty, bool> GetPropertyPathMatcher(
      Dictionary<EdmProperty, IList<ColumnMappingBuilder>> columnMappingIndex,
      ColumnMappingBuilder propertyMappingBuilder)
    {
      return (Func<EdmProperty, bool>) (c =>
      {
        if (!columnMappingIndex.ContainsKey(c))
          return false;
        IList<ColumnMappingBuilder> columnMappingBuilderList = columnMappingIndex[c];
        for (int index = 0; index < columnMappingBuilderList.Count; ++index)
        {
          if (columnMappingBuilderList[index].PropertyPath.PathEqual(propertyMappingBuilder.PropertyPath))
            return true;
        }
        return false;
      });
    }

    private static bool PathEqual(this IList<EdmProperty> listA, IList<EdmProperty> listB)
    {
      if (listA == null || listB == null || listA.Count != listB.Count)
        return false;
      for (int index = 0; index < listA.Count; ++index)
      {
        if (listA[index] != listB[index])
          return false;
      }
      return true;
    }

    private static Dictionary<EdmProperty, IList<ColumnMappingBuilder>> GetColumnMappingIndex(
      DbDatabaseMapping databaseMapping)
    {
      Dictionary<EdmProperty, IList<ColumnMappingBuilder>> dictionary = new Dictionary<EdmProperty, IList<ColumnMappingBuilder>>();
      IEnumerable<EntitySetMapping> entitySetMappings = databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>().EntitySetMappings;
      if (entitySetMappings == null)
        return dictionary;
      List<EntitySetMapping> list = entitySetMappings.ToList<EntitySetMapping>();
      for (int index1 = 0; index1 < list.Count; ++index1)
      {
        IList<EntityTypeMapping> entityTypeMappings = (IList<EntityTypeMapping>) list[index1].EntityTypeMappings;
        if (entityTypeMappings != null)
        {
          for (int index2 = 0; index2 < entityTypeMappings.Count; ++index2)
          {
            IList<MappingFragment> mappingFragments = (IList<MappingFragment>) entityTypeMappings[index2].MappingFragments;
            if (mappingFragments != null)
            {
              for (int index3 = 0; index3 < mappingFragments.Count; ++index3)
              {
                IList<ColumnMappingBuilder> columnMappings = mappingFragments[index3].ColumnMappings as IList<ColumnMappingBuilder>;
                if (columnMappings != null)
                {
                  for (int index4 = 0; index4 < columnMappings.Count; ++index4)
                  {
                    ColumnMappingBuilder columnMappingBuilder = columnMappings[index4];
                    IList<ColumnMappingBuilder> columnMappingBuilderList;
                    if (dictionary.ContainsKey(columnMappingBuilder.ColumnProperty))
                      columnMappingBuilderList = dictionary[columnMappingBuilder.ColumnProperty];
                    else
                      dictionary.Add(columnMappingBuilder.ColumnProperty, columnMappingBuilderList = (IList<ColumnMappingBuilder>) new List<ColumnMappingBuilder>());
                    columnMappingBuilderList.Add(columnMappingBuilder);
                  }
                }
              }
            }
          }
        }
      }
      return dictionary;
    }

    public static void UpdatePropertyMappings(
      DbDatabaseMapping databaseMapping,
      IEnumerable<EntitySet> entitySets,
      EntityType fromTable,
      MappingFragment fragment,
      bool useExisting)
    {
      if (fromTable == fragment.Table)
        return;
      Dictionary<EdmProperty, IList<ColumnMappingBuilder>> columnMappingIndex = EntityMappingOperations.GetColumnMappingIndex(databaseMapping);
      List<ColumnMappingBuilder> list = fragment.ColumnMappings.ToList<ColumnMappingBuilder>();
      for (int index = 0; index < list.Count; ++index)
        EntityMappingOperations.UpdatePropertyMapping(databaseMapping, entitySets, columnMappingIndex, list[index], fromTable, fragment.Table, useExisting);
    }

    public static void MovePropertyMapping(
      DbDatabaseMapping databaseMapping,
      IEnumerable<EntitySet> entitySets,
      MappingFragment fromFragment,
      MappingFragment toFragment,
      ColumnMappingBuilder propertyMappingBuilder,
      bool requiresUpdate,
      bool useExisting)
    {
      if (requiresUpdate && fromFragment.Table != toFragment.Table)
        EntityMappingOperations.UpdatePropertyMapping(databaseMapping, entitySets, EntityMappingOperations.GetColumnMappingIndex(databaseMapping), propertyMappingBuilder, fromFragment.Table, toFragment.Table, useExisting);
      fromFragment.RemoveColumnMapping(propertyMappingBuilder);
      toFragment.AddColumnMapping(propertyMappingBuilder);
    }

    public static void CopyPropertyMappingToFragment(
      ColumnMappingBuilder propertyMappingBuilder,
      MappingFragment fragment,
      Func<EdmProperty, bool> isCompatible,
      bool useExisting)
    {
      EdmProperty columnProperty = TablePrimitiveOperations.IncludeColumn(fragment.Table, propertyMappingBuilder.ColumnProperty, isCompatible, useExisting);
      fragment.AddColumnMapping(new ColumnMappingBuilder(columnProperty, propertyMappingBuilder.PropertyPath));
    }

    public static void UpdateConditions(
      EdmModel database,
      EntityType fromTable,
      MappingFragment fragment)
    {
      if (fromTable == fragment.Table)
        return;
      fragment.ColumnConditions.Each<ConditionPropertyMapping>((Action<ConditionPropertyMapping>) (cc => cc.Column = TableOperations.CopyColumnAndAnyConstraints(database, fromTable, fragment.Table, cc.Column, TablePrimitiveOperations.GetNameMatcher(cc.Column.Name), true)));
    }
  }
}
