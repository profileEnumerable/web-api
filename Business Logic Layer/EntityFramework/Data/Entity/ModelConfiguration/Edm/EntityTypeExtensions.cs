// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.EntityTypeExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class EntityTypeExtensions
  {
    private const string TableNameAnnotation = "TableName";

    public static void AddColumn(this EntityType table, EdmProperty column)
    {
      column.SetPreferredName(column.Name);
      column.Name = ((IEnumerable<INamedDataModelItem>) table.Properties).UniquifyName(column.Name);
      table.AddMember((EdmMember) column);
    }

    public static void SetConfiguration(this EntityType table, object configuration)
    {
      table.GetMetadataProperties().SetConfiguration(configuration);
    }

    public static DatabaseName GetTableName(this EntityType table)
    {
      return (DatabaseName) table.Annotations.GetAnnotation("TableName");
    }

    public static void SetTableName(this EntityType table, DatabaseName tableName)
    {
      table.GetMetadataProperties().SetAnnotation("TableName", (object) tableName);
    }

    internal static IEnumerable<EntityType> ToHierarchy(this EntityType edmType)
    {
      return EdmType.SafeTraverseHierarchy<EntityType>(edmType);
    }

    public static IEnumerable<EdmProperty> GetValidKey(
      this EntityType entityType)
    {
      List<EdmProperty> edmPropertyList = (List<EdmProperty>) null;
      List<EntityType> list = entityType.ToHierarchy().ToList<EntityType>();
      for (int index1 = list.Count - 1; index1 >= 0; --index1)
      {
        EntityType entityType1 = list[index1];
        if (entityType1.BaseType == null && entityType1.KeyProperties.Count > 0)
        {
          if (edmPropertyList != null)
            return Enumerable.Empty<EdmProperty>();
          edmPropertyList = new List<EdmProperty>();
          HashSet<EdmProperty> edmPropertySet1 = new HashSet<EdmProperty>();
          HashSet<string> stringSet = new HashSet<string>();
          HashSet<EdmProperty> edmPropertySet2 = new HashSet<EdmProperty>(entityType1.DeclaredProperties.Where<EdmProperty>((Func<EdmProperty, bool>) (p => p != null)));
          for (int index2 = 0; index2 < entityType1.KeyProperties.Count; ++index2)
          {
            EdmProperty keyProperty = entityType1.KeyProperties[index2];
            if (keyProperty == null || edmPropertySet1.Contains(keyProperty) || (!edmPropertySet2.Contains(keyProperty) || string.IsNullOrEmpty(keyProperty.Name)) || (string.IsNullOrWhiteSpace(keyProperty.Name) || stringSet.Contains(keyProperty.Name)))
              return Enumerable.Empty<EdmProperty>();
            edmPropertyList.Add(keyProperty);
            edmPropertySet1.Add(keyProperty);
            stringSet.Add(keyProperty.Name);
          }
        }
      }
      return (IEnumerable<EdmProperty>) edmPropertyList ?? Enumerable.Empty<EdmProperty>();
    }

    public static List<EdmProperty> GetKeyProperties(this EntityType entityType)
    {
      HashSet<EntityType> visitedTypes = new HashSet<EntityType>();
      List<EdmProperty> keyProperties = new List<EdmProperty>();
      EntityTypeExtensions.GetKeyProperties(visitedTypes, entityType, keyProperties);
      return keyProperties;
    }

    private static void GetKeyProperties(
      HashSet<EntityType> visitedTypes,
      EntityType visitingType,
      List<EdmProperty> keyProperties)
    {
      if (visitedTypes.Contains(visitingType))
        return;
      visitedTypes.Add(visitingType);
      if (visitingType.BaseType != null)
      {
        EntityTypeExtensions.GetKeyProperties(visitedTypes, (EntityType) visitingType.BaseType, keyProperties);
      }
      else
      {
        ReadOnlyMetadataCollection<EdmProperty> keyProperties1 = visitingType.KeyProperties;
        if (keyProperties1.Count <= 0)
          return;
        keyProperties.AddRange((IEnumerable<EdmProperty>) keyProperties1);
      }
    }

    public static EntityType GetRootType(this EntityType entityType)
    {
      EdmType edmType = (EdmType) entityType;
      while (edmType.BaseType != null)
        edmType = edmType.BaseType;
      return (EntityType) edmType;
    }

    public static bool IsAncestorOf(this EntityType ancestor, EntityType entityType)
    {
      for (; entityType != null; entityType = (EntityType) entityType.BaseType)
      {
        if (entityType.BaseType == ancestor)
          return true;
      }
      return false;
    }

    public static IEnumerable<EdmProperty> KeyProperties(
      this EntityType entityType)
    {
      return (IEnumerable<EdmProperty>) entityType.GetRootType().KeyProperties;
    }

    public static object GetConfiguration(this EntityType entityType)
    {
      return entityType.Annotations.GetConfiguration();
    }

    public static Type GetClrType(this EntityType entityType)
    {
      return entityType.Annotations.GetClrType();
    }

    public static IEnumerable<EntityType> TypeHierarchyIterator(
      this EntityType entityType,
      EdmModel model)
    {
      yield return entityType;
      IEnumerable<EntityType> derivedEntityTypes = model.GetDerivedTypes(entityType);
      if (derivedEntityTypes != null)
      {
        foreach (EntityType entityType1 in derivedEntityTypes)
        {
          foreach (EntityType entityType2 in entityType1.TypeHierarchyIterator(model))
            yield return entityType2;
        }
      }
    }

    public static EdmProperty AddComplexProperty(
      this EntityType entityType,
      string name,
      ComplexType complexType)
    {
      EdmProperty complex = EdmProperty.CreateComplex(name, complexType);
      entityType.AddMember((EdmMember) complex);
      return complex;
    }

    public static EdmProperty GetDeclaredPrimitiveProperty(
      this EntityType entityType,
      PropertyInfo propertyInfo)
    {
      return entityType.GetDeclaredPrimitiveProperties().SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (p => p.GetClrPropertyInfo().IsSameAs(propertyInfo)));
    }

    public static IEnumerable<EdmProperty> GetDeclaredPrimitiveProperties(
      this EntityType entityType)
    {
      return entityType.DeclaredProperties.Where<EdmProperty>((Func<EdmProperty, bool>) (p => p.IsUnderlyingPrimitiveType));
    }

    public static NavigationProperty AddNavigationProperty(
      this EntityType entityType,
      string name,
      AssociationType associationType)
    {
      EntityType entityType1 = associationType.TargetEnd.GetEntityType();
      EdmType edmType = associationType.TargetEnd.RelationshipMultiplicity.IsMany() ? (EdmType) entityType1.GetCollectionType() : (EdmType) entityType1;
      NavigationProperty navigationProperty = new NavigationProperty(name, TypeUsage.Create(edmType))
      {
        RelationshipType = (RelationshipType) associationType,
        FromEndMember = (RelationshipEndMember) associationType.SourceEnd,
        ToEndMember = (RelationshipEndMember) associationType.TargetEnd
      };
      entityType.AddMember((EdmMember) navigationProperty);
      return navigationProperty;
    }

    public static NavigationProperty GetNavigationProperty(
      this EntityType entityType,
      PropertyInfo propertyInfo)
    {
      return entityType.NavigationProperties.SingleOrDefault<NavigationProperty>((Func<NavigationProperty, bool>) (np => np.GetClrPropertyInfo().IsSameAs(propertyInfo)));
    }

    public static bool IsRootOfSet(this EntityType entityType, IEnumerable<EntityType> set)
    {
      return set.All<EntityType>((Func<EntityType, bool>) (et =>
      {
        if (et != entityType && !entityType.IsAncestorOf(et))
          return et.GetRootType() != entityType.GetRootType();
        return true;
      }));
    }
  }
}
