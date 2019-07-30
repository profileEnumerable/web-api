// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.StorageEntityTypeMappingExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class StorageEntityTypeMappingExtensions
  {
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    public static object GetConfiguration(this EntityTypeMapping entityTypeMapping)
    {
      return entityTypeMapping.Annotations.GetConfiguration();
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    public static void SetConfiguration(
      this EntityTypeMapping entityTypeMapping,
      object configuration)
    {
      entityTypeMapping.Annotations.SetConfiguration(configuration);
    }

    public static ColumnMappingBuilder GetPropertyMapping(
      this EntityTypeMapping entityTypeMapping,
      params EdmProperty[] propertyPath)
    {
      return entityTypeMapping.MappingFragments.SelectMany<MappingFragment, ColumnMappingBuilder>((Func<MappingFragment, IEnumerable<ColumnMappingBuilder>>) (f => f.ColumnMappings)).Single<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (p => p.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) propertyPath)));
    }

    public static EntityType GetPrimaryTable(this EntityTypeMapping entityTypeMapping)
    {
      return entityTypeMapping.MappingFragments.First<MappingFragment>().Table;
    }

    public static bool UsesOtherTables(this EntityTypeMapping entityTypeMapping, EntityType table)
    {
      return entityTypeMapping.MappingFragments.Any<MappingFragment>((Func<MappingFragment, bool>) (f => f.Table != table));
    }

    public static Type GetClrType(this EntityTypeMapping entityTypeMappping)
    {
      return entityTypeMappping.Annotations.GetClrType();
    }

    public static void SetClrType(this EntityTypeMapping entityTypeMapping, Type type)
    {
      entityTypeMapping.Annotations.SetClrType(type);
    }

    public static EntityTypeMapping Clone(this EntityTypeMapping entityTypeMapping)
    {
      EntityTypeMapping entityTypeMapping1 = new EntityTypeMapping((EntitySetMapping) null);
      entityTypeMapping1.AddType(entityTypeMapping.EntityType);
      entityTypeMapping.Annotations.Copy((ICollection<MetadataProperty>) entityTypeMapping1.Annotations);
      return entityTypeMapping1;
    }
  }
}
