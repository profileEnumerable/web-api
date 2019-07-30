// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.StorageMappingFragmentExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class StorageMappingFragmentExtensions
  {
    private const string DefaultDiscriminatorAnnotation = "DefaultDiscriminator";
    private const string ConditionOnlyFragmentAnnotation = "ConditionOnlyFragment";
    private const string UnmappedPropertiesFragmentAnnotation = "UnmappedPropertiesFragment";

    public static EdmProperty GetDefaultDiscriminator(
      this MappingFragment entityTypeMapppingFragment)
    {
      return (EdmProperty) entityTypeMapppingFragment.Annotations.GetAnnotation("DefaultDiscriminator");
    }

    public static void SetDefaultDiscriminator(
      this MappingFragment entityTypeMappingFragment,
      EdmProperty discriminator)
    {
      entityTypeMappingFragment.Annotations.SetAnnotation("DefaultDiscriminator", (object) discriminator);
    }

    public static void RemoveDefaultDiscriminatorAnnotation(
      this MappingFragment entityTypeMappingFragment)
    {
      entityTypeMappingFragment.Annotations.RemoveAnnotation("DefaultDiscriminator");
    }

    public static void RemoveDefaultDiscriminator(
      this MappingFragment entityTypeMappingFragment,
      EntitySetMapping entitySetMapping)
    {
      EdmProperty discriminatorColumn = entityTypeMappingFragment.RemoveDefaultDiscriminatorCondition();
      if (discriminatorColumn != null)
      {
        EntityType table = entityTypeMappingFragment.Table;
        table.Properties.Where<EdmProperty>((Func<EdmProperty, bool>) (c => c.Name.Equals(discriminatorColumn.Name, StringComparison.Ordinal))).ToList<EdmProperty>().Each<EdmProperty>(new Action<EdmProperty>(((StructuralType) table).RemoveMember));
      }
      if (entitySetMapping == null || !entityTypeMappingFragment.IsConditionOnlyFragment() || entityTypeMappingFragment.ColumnConditions.Any<ConditionPropertyMapping>())
        return;
      EntityTypeMapping typeMapping = entitySetMapping.EntityTypeMappings.Single<EntityTypeMapping>((Func<EntityTypeMapping, bool>) (etm => etm.MappingFragments.Contains(entityTypeMappingFragment)));
      typeMapping.RemoveFragment(entityTypeMappingFragment);
      if (typeMapping.MappingFragments.Count != 0)
        return;
      entitySetMapping.RemoveTypeMapping(typeMapping);
    }

    public static EdmProperty RemoveDefaultDiscriminatorCondition(
      this MappingFragment entityTypeMappingFragment)
    {
      EdmProperty defaultDiscriminator = entityTypeMappingFragment.GetDefaultDiscriminator();
      if (defaultDiscriminator != null && entityTypeMappingFragment.ColumnConditions.Any<ConditionPropertyMapping>())
        entityTypeMappingFragment.ClearConditions();
      entityTypeMappingFragment.RemoveDefaultDiscriminatorAnnotation();
      return defaultDiscriminator;
    }

    public static void AddDiscriminatorCondition(
      this MappingFragment entityTypeMapppingFragment,
      EdmProperty discriminatorColumn,
      object value)
    {
      entityTypeMapppingFragment.AddConditionProperty((ConditionPropertyMapping) new ValueConditionMapping(discriminatorColumn, value));
    }

    public static void AddNullabilityCondition(
      this MappingFragment entityTypeMapppingFragment,
      EdmProperty column,
      bool isNull)
    {
      entityTypeMapppingFragment.AddConditionProperty((ConditionPropertyMapping) new IsNullConditionMapping(column, isNull));
    }

    public static bool IsConditionOnlyFragment(this MappingFragment entityTypeMapppingFragment)
    {
      object annotation = entityTypeMapppingFragment.Annotations.GetAnnotation("ConditionOnlyFragment");
      if (annotation != null)
        return (bool) annotation;
      return false;
    }

    public static void SetIsConditionOnlyFragment(
      this MappingFragment entityTypeMapppingFragment,
      bool isConditionOnlyFragment)
    {
      if (isConditionOnlyFragment)
        entityTypeMapppingFragment.Annotations.SetAnnotation("ConditionOnlyFragment", (object) isConditionOnlyFragment);
      else
        entityTypeMapppingFragment.Annotations.RemoveAnnotation("ConditionOnlyFragment");
    }

    public static bool IsUnmappedPropertiesFragment(this MappingFragment entityTypeMapppingFragment)
    {
      object annotation = entityTypeMapppingFragment.Annotations.GetAnnotation("UnmappedPropertiesFragment");
      if (annotation != null)
        return (bool) annotation;
      return false;
    }

    public static void SetIsUnmappedPropertiesFragment(
      this MappingFragment entityTypeMapppingFragment,
      bool isUnmappedPropertiesFragment)
    {
      if (isUnmappedPropertiesFragment)
        entityTypeMapppingFragment.Annotations.SetAnnotation("UnmappedPropertiesFragment", (object) isUnmappedPropertiesFragment);
      else
        entityTypeMapppingFragment.Annotations.RemoveAnnotation("UnmappedPropertiesFragment");
    }
  }
}
