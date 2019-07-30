// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.StructuredTypeInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class StructuredTypeInfo
  {
    private TypeUsage m_stringType;
    private TypeUsage m_intType;
    private readonly Dictionary<TypeUsage, TypeInfo> m_typeInfoMap;
    private bool m_typeInfoMapPopulated;
    private EntitySet[] m_entitySetIdToEntitySetMap;
    private Dictionary<EntitySet, int> m_entitySetToEntitySetIdMap;
    private Dictionary<EntityTypeBase, EntitySet> m_entityTypeToEntitySetMap;
    private Dictionary<EntitySetBase, ExplicitDiscriminatorMap> m_discriminatorMaps;
    private RelPropertyHelper m_relPropertyHelper;
    private readonly HashSet<string> m_typesNeedingNullSentinel;

    private StructuredTypeInfo(HashSet<string> typesNeedingNullSentinel)
    {
      this.m_typeInfoMap = new Dictionary<TypeUsage, TypeInfo>((IEqualityComparer<TypeUsage>) TypeUsageEqualityComparer.Instance);
      this.m_typeInfoMapPopulated = false;
      this.m_typesNeedingNullSentinel = typesNeedingNullSentinel;
    }

    internal static void Process(
      Command itree,
      HashSet<TypeUsage> referencedTypes,
      HashSet<EntitySet> referencedEntitySets,
      HashSet<EntityType> freeFloatingEntityConstructorTypes,
      Dictionary<EntitySetBase, DiscriminatorMapInfo> discriminatorMaps,
      RelPropertyHelper relPropertyHelper,
      HashSet<string> typesNeedingNullSentinel,
      out StructuredTypeInfo structuredTypeInfo)
    {
      structuredTypeInfo = new StructuredTypeInfo(typesNeedingNullSentinel);
      structuredTypeInfo.Process(itree, referencedTypes, referencedEntitySets, freeFloatingEntityConstructorTypes, discriminatorMaps, relPropertyHelper);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "itree")]
    private void Process(
      Command itree,
      HashSet<TypeUsage> referencedTypes,
      HashSet<EntitySet> referencedEntitySets,
      HashSet<EntityType> freeFloatingEntityConstructorTypes,
      Dictionary<EntitySetBase, DiscriminatorMapInfo> discriminatorMaps,
      RelPropertyHelper relPropertyHelper)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(null != itree, "null itree?");
      this.m_stringType = itree.StringType;
      this.m_intType = itree.IntegerType;
      this.m_relPropertyHelper = relPropertyHelper;
      this.ProcessEntitySets(referencedEntitySets, freeFloatingEntityConstructorTypes);
      this.ProcessDiscriminatorMaps(discriminatorMaps);
      this.ProcessTypes(referencedTypes);
    }

    internal EntitySet[] EntitySetIdToEntitySetMap
    {
      get
      {
        return this.m_entitySetIdToEntitySetMap;
      }
    }

    internal RelPropertyHelper RelPropertyHelper
    {
      get
      {
        return this.m_relPropertyHelper;
      }
    }

    internal EntitySet GetEntitySet(EntityTypeBase type)
    {
      EntitySet entitySet;
      if (!this.m_entityTypeToEntitySetMap.TryGetValue(StructuredTypeInfo.GetRootType(type), out entitySet))
        return (EntitySet) null;
      return entitySet;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal int GetEntitySetId(EntitySet e)
    {
      int num = 0;
      if (!this.m_entitySetToEntitySetIdMap.TryGetValue(e, out num))
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "no such entity set?");
      return num;
    }

    internal Set<EntitySet> GetEntitySets()
    {
      return new Set<EntitySet>((IEnumerable<EntitySet>) this.m_entitySetIdToEntitySetMap).MakeReadOnly();
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "typeInfo")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal TypeInfo GetTypeInfo(TypeUsage type)
    {
      if (!TypeUtils.IsStructuredType(type))
        return (TypeInfo) null;
      TypeInfo typeInfo = (TypeInfo) null;
      if (!this.m_typeInfoMap.TryGetValue(type, out typeInfo))
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!TypeUtils.IsStructuredType(type) || !this.m_typeInfoMapPopulated, "cannot find typeInfo for type " + (object) type);
      return typeInfo;
    }

    private void AddEntityTypeToSetEntry(EntityType entityType, EntitySet entitySet)
    {
      EntityTypeBase rootType = StructuredTypeInfo.GetRootType((EntityTypeBase) entityType);
      bool flag = true;
      if (entitySet == null)
      {
        flag = false;
      }
      else
      {
        EntitySet entitySet1;
        if (this.m_entityTypeToEntitySetMap.TryGetValue(rootType, out entitySet1) && entitySet1 != entitySet)
          flag = false;
      }
      if (flag)
        this.m_entityTypeToEntitySetMap[rootType] = entitySet;
      else
        this.m_entityTypeToEntitySetMap[rootType] = (EntitySet) null;
    }

    private void ProcessEntitySets(
      HashSet<EntitySet> referencedEntitySets,
      HashSet<EntityType> freeFloatingEntityConstructorTypes)
    {
      this.AssignEntitySetIds(referencedEntitySets);
      this.m_entityTypeToEntitySetMap = new Dictionary<EntityTypeBase, EntitySet>();
      foreach (EntitySet referencedEntitySet in referencedEntitySets)
        this.AddEntityTypeToSetEntry(referencedEntitySet.ElementType, referencedEntitySet);
      foreach (EntityType entityConstructorType in freeFloatingEntityConstructorTypes)
        this.AddEntityTypeToSetEntry(entityConstructorType, (EntitySet) null);
    }

    private void ProcessDiscriminatorMaps(
      Dictionary<EntitySetBase, DiscriminatorMapInfo> discriminatorMaps)
    {
      Dictionary<EntitySetBase, ExplicitDiscriminatorMap> dictionary = (Dictionary<EntitySetBase, ExplicitDiscriminatorMap>) null;
      if (discriminatorMaps != null)
      {
        dictionary = new Dictionary<EntitySetBase, ExplicitDiscriminatorMap>(discriminatorMaps.Count, discriminatorMaps.Comparer);
        foreach (KeyValuePair<EntitySetBase, DiscriminatorMapInfo> discriminatorMap1 in discriminatorMaps)
        {
          EntitySetBase key = discriminatorMap1.Key;
          ExplicitDiscriminatorMap discriminatorMap2 = discriminatorMap1.Value.DiscriminatorMap;
          if (discriminatorMap2 != null && this.GetEntitySet(StructuredTypeInfo.GetRootType(key.ElementType)) != null)
            dictionary.Add(key, discriminatorMap2);
        }
        if (dictionary.Count == 0)
          dictionary = (Dictionary<EntitySetBase, ExplicitDiscriminatorMap>) null;
      }
      this.m_discriminatorMaps = dictionary;
    }

    private void AssignEntitySetIds(HashSet<EntitySet> referencedEntitySets)
    {
      this.m_entitySetIdToEntitySetMap = new EntitySet[referencedEntitySets.Count];
      this.m_entitySetToEntitySetIdMap = new Dictionary<EntitySet, int>();
      int index = 0;
      foreach (EntitySet referencedEntitySet in referencedEntitySets)
      {
        if (!this.m_entitySetToEntitySetIdMap.ContainsKey(referencedEntitySet))
        {
          this.m_entitySetIdToEntitySetMap[index] = referencedEntitySet;
          this.m_entitySetToEntitySetIdMap[referencedEntitySet] = index;
          ++index;
        }
      }
    }

    private void ProcessTypes(HashSet<TypeUsage> referencedTypes)
    {
      this.PopulateTypeInfoMap(referencedTypes);
      this.AssignTypeIds();
      this.ExplodeTypes();
    }

    private void PopulateTypeInfoMap(HashSet<TypeUsage> referencedTypes)
    {
      foreach (TypeUsage referencedType in referencedTypes)
        this.CreateTypeInfoForType(referencedType);
      this.m_typeInfoMapPopulated = true;
    }

    private bool TryGetDiscriminatorMap(EdmType type, out ExplicitDiscriminatorMap discriminatorMap)
    {
      discriminatorMap = (ExplicitDiscriminatorMap) null;
      EntitySet entitySet;
      if (this.m_discriminatorMaps == null || type.BuiltInTypeKind != BuiltInTypeKind.EntityType || (!this.m_entityTypeToEntitySetMap.TryGetValue(StructuredTypeInfo.GetRootType((EntityTypeBase) type), out entitySet) || entitySet == null))
        return false;
      return this.m_discriminatorMaps.TryGetValue((EntitySetBase) entitySet, out discriminatorMap);
    }

    private void CreateTypeInfoForType(TypeUsage type)
    {
      while (TypeUtils.IsCollectionType(type))
        type = TypeHelpers.GetEdmType<CollectionType>(type).TypeUsage;
      if (!TypeUtils.IsStructuredType(type))
        return;
      ExplicitDiscriminatorMap discriminatorMap;
      this.TryGetDiscriminatorMap(type.EdmType, out discriminatorMap);
      this.CreateTypeInfoForStructuredType(type, discriminatorMap);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private TypeInfo CreateTypeInfoForStructuredType(
      TypeUsage type,
      ExplicitDiscriminatorMap discriminatorMap)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeUtils.IsStructuredType(type), "expected structured type. Found " + (object) type);
      TypeInfo typeInfo1 = this.GetTypeInfo(type);
      if (typeInfo1 != null)
        return typeInfo1;
      TypeInfo superTypeInfo = (TypeInfo) null;
      if (type.EdmType.BaseType != null)
      {
        superTypeInfo = this.CreateTypeInfoForStructuredType(TypeUsage.Create(type.EdmType.BaseType), discriminatorMap);
      }
      else
      {
        RefType type1;
        if (TypeHelpers.TryGetEdmType<RefType>(type, out type1))
        {
          EntityType elementType = type1.ElementType as EntityType;
          if (elementType != null && elementType.BaseType != null)
            superTypeInfo = this.CreateTypeInfoForStructuredType(TypeHelpers.CreateReferenceTypeUsage(elementType.BaseType as EntityType), discriminatorMap);
        }
      }
      foreach (EdmMember structuralMember in TypeHelpers.GetDeclaredStructuralMembers(type))
        this.CreateTypeInfoForType(structuralMember.TypeUsage);
      EntityTypeBase type2;
      if (TypeHelpers.TryGetEdmType<EntityTypeBase>(type, out type2))
      {
        foreach (RelProperty declaredOnlyRelProperty in this.m_relPropertyHelper.GetDeclaredOnlyRelProperties(type2))
          this.CreateTypeInfoForType(declaredOnlyRelProperty.ToEnd.TypeUsage);
      }
      TypeInfo typeInfo2 = TypeInfo.Create(type, superTypeInfo, discriminatorMap);
      this.m_typeInfoMap.Add(type, typeInfo2);
      return typeInfo2;
    }

    private void AssignTypeIds()
    {
      int num = 0;
      foreach (KeyValuePair<TypeUsage, TypeInfo> typeInfo in this.m_typeInfoMap)
      {
        if (typeInfo.Value.RootType.DiscriminatorMap != null)
        {
          EntityType edmType = (EntityType) typeInfo.Key.EdmType;
          typeInfo.Value.TypeId = typeInfo.Value.RootType.DiscriminatorMap.GetTypeId(edmType);
        }
        else if (typeInfo.Value.IsRootType && (TypeSemantics.IsEntityType(typeInfo.Key) || TypeSemantics.IsComplexType(typeInfo.Key)))
        {
          this.AssignRootTypeId(typeInfo.Value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}X", (object) num));
          ++num;
        }
      }
    }

    private void AssignRootTypeId(TypeInfo typeInfo, string typeId)
    {
      typeInfo.TypeId = (object) typeId;
      this.AssignTypeIdsToSubTypes(typeInfo);
    }

    private void AssignTypeIdsToSubTypes(TypeInfo typeInfo)
    {
      int subtypeNum = 0;
      foreach (TypeInfo immediateSubType in typeInfo.ImmediateSubTypes)
      {
        this.AssignTypeId(immediateSubType, subtypeNum);
        ++subtypeNum;
      }
    }

    private void AssignTypeId(TypeInfo typeInfo, int subtypeNum)
    {
      typeInfo.TypeId = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}X", typeInfo.SuperType.TypeId, (object) subtypeNum);
      this.AssignTypeIdsToSubTypes(typeInfo);
    }

    private static bool NeedsTypeIdProperty(TypeInfo typeInfo)
    {
      if (typeInfo.ImmediateSubTypes.Count > 0)
        return !TypeSemantics.IsReferenceType(typeInfo.Type);
      return false;
    }

    private bool NeedsNullSentinelProperty(TypeInfo typeInfo)
    {
      return this.m_typesNeedingNullSentinel.Contains(typeInfo.Type.EdmType.Identity);
    }

    private bool NeedsEntitySetIdProperty(TypeInfo typeInfo)
    {
      RefType edmType = typeInfo.Type.EdmType as RefType;
      EntityType entityType = edmType == null ? typeInfo.Type.EdmType as EntityType : edmType.ElementType as EntityType;
      return entityType != null && this.GetEntitySet((EntityTypeBase) entityType) == null;
    }

    private void ExplodeTypes()
    {
      foreach (KeyValuePair<TypeUsage, TypeInfo> typeInfo in this.m_typeInfoMap)
      {
        if (typeInfo.Value.IsRootType)
          this.ExplodeType(typeInfo.Value);
      }
    }

    private TypeInfo ExplodeType(TypeUsage type)
    {
      if (TypeUtils.IsStructuredType(type))
      {
        TypeInfo typeInfo = this.GetTypeInfo(type);
        this.ExplodeType(typeInfo);
        return typeInfo;
      }
      if (!TypeUtils.IsCollectionType(type))
        return (TypeInfo) null;
      this.ExplodeType(TypeHelpers.GetEdmType<CollectionType>(type).TypeUsage);
      return (TypeInfo) null;
    }

    private void ExplodeType(TypeInfo typeInfo)
    {
      this.ExplodeRootStructuredType(typeInfo.RootType);
    }

    private void ExplodeRootStructuredType(RootTypeInfo rootType)
    {
      if (rootType.FlattenedType != null)
        return;
      if (StructuredTypeInfo.NeedsTypeIdProperty((TypeInfo) rootType))
      {
        rootType.AddPropertyRef((PropertyRef) TypeIdPropertyRef.Instance);
        if (rootType.DiscriminatorMap != null)
        {
          rootType.TypeIdKind = TypeIdKind.UserSpecified;
          rootType.TypeIdType = Helper.GetModelTypeUsage(rootType.DiscriminatorMap.DiscriminatorProperty);
        }
        else
        {
          rootType.TypeIdKind = TypeIdKind.Generated;
          rootType.TypeIdType = this.m_stringType;
        }
      }
      if (this.NeedsEntitySetIdProperty((TypeInfo) rootType))
        rootType.AddPropertyRef((PropertyRef) EntitySetIdPropertyRef.Instance);
      if (this.NeedsNullSentinelProperty((TypeInfo) rootType))
        rootType.AddPropertyRef((PropertyRef) NullSentinelPropertyRef.Instance);
      this.ExplodeRootStructuredTypeHelper((TypeInfo) rootType);
      if (TypeSemantics.IsEntityType(rootType.Type))
        this.AddRelProperties((TypeInfo) rootType);
      this.CreateFlattenedRecordType(rootType);
    }

    private void ExplodeRootStructuredTypeHelper(TypeInfo typeInfo)
    {
      RootTypeInfo rootType = typeInfo.RootType;
      RefType type;
      IEnumerable enumerable;
      if (TypeHelpers.TryGetEdmType<RefType>(typeInfo.Type, out type))
      {
        if (!typeInfo.IsRootType)
          return;
        enumerable = (IEnumerable) type.ElementType.KeyMembers;
      }
      else
        enumerable = TypeHelpers.GetDeclaredStructuralMembers(typeInfo.Type);
      foreach (EdmMember edmMember in enumerable)
      {
        TypeInfo typeInfo1 = this.ExplodeType(edmMember.TypeUsage);
        if (typeInfo1 == null)
        {
          rootType.AddPropertyRef((PropertyRef) new SimplePropertyRef(edmMember));
        }
        else
        {
          foreach (PropertyRef propertyRef in typeInfo1.PropertyRefList)
            rootType.AddPropertyRef(propertyRef.CreateNestedPropertyRef(edmMember));
        }
      }
      foreach (TypeInfo immediateSubType in typeInfo.ImmediateSubTypes)
        this.ExplodeRootStructuredTypeHelper(immediateSubType);
    }

    private void AddRelProperties(TypeInfo typeInfo)
    {
      foreach (RelProperty declaredOnlyRelProperty in this.m_relPropertyHelper.GetDeclaredOnlyRelProperties((EntityTypeBase) typeInfo.Type.EdmType))
      {
        EdmType edmType = declaredOnlyRelProperty.ToEnd.TypeUsage.EdmType;
        TypeInfo typeInfo1 = this.GetTypeInfo(declaredOnlyRelProperty.ToEnd.TypeUsage);
        this.ExplodeType(typeInfo1);
        foreach (PropertyRef propertyRef in typeInfo1.PropertyRefList)
          typeInfo.RootType.AddPropertyRef(propertyRef.CreateNestedPropertyRef(declaredOnlyRelProperty));
      }
      foreach (TypeInfo immediateSubType in typeInfo.ImmediateSubTypes)
        this.AddRelProperties(immediateSubType);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void CreateFlattenedRecordType(RootTypeInfo type)
    {
      bool flag = TypeSemantics.IsEntityType(type.Type) && type.ImmediateSubTypes.Count == 0;
      List<KeyValuePair<string, TypeUsage>> keyValuePairList = new List<KeyValuePair<string, TypeUsage>>();
      HashSet<string> stringSet = new HashSet<string>();
      int num = 0;
      foreach (PropertyRef propertyRef in type.PropertyRefList)
      {
        string key = (string) null;
        if (flag)
        {
          SimplePropertyRef simplePropertyRef = propertyRef as SimplePropertyRef;
          if (simplePropertyRef != null)
            key = simplePropertyRef.Property.Name;
        }
        if (key == null)
        {
          key = "F" + num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          ++num;
        }
        while (stringSet.Contains(key))
        {
          key = "F" + num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          ++num;
        }
        TypeUsage propertyType = this.GetPropertyType(type, propertyRef);
        keyValuePairList.Add(new KeyValuePair<string, TypeUsage>(key, propertyType));
        stringSet.Add(key);
      }
      type.FlattenedType = TypeHelpers.CreateRowType((IEnumerable<KeyValuePair<string, TypeUsage>>) keyValuePairList);
      IEnumerator<PropertyRef> enumerator = type.PropertyRefList.GetEnumerator();
      foreach (EdmProperty property in type.FlattenedType.Properties)
      {
        if (!enumerator.MoveNext())
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "property refs count and flattened type member count mismatch?");
        type.AddPropertyMapping(enumerator.Current, property);
      }
    }

    private TypeUsage GetNewType(TypeUsage type)
    {
      if (TypeUtils.IsStructuredType(type))
        return this.GetTypeInfo(type).FlattenedTypeUsage;
      TypeUsage elementType;
      if (TypeHelpers.TryGetCollectionElementType(type, out elementType))
      {
        TypeUsage newType = this.GetNewType(elementType);
        if (newType.EdmEquals((MetadataItem) elementType))
          return type;
        return TypeHelpers.CreateCollectionTypeUsage(newType);
      }
      if (TypeUtils.IsEnumerationType(type))
        return TypeHelpers.CreateEnumUnderlyingTypeUsage(type);
      if (TypeSemantics.IsStrongSpatialType(type))
        return TypeHelpers.CreateSpatialUnionTypeUsage(type);
      return type;
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private TypeUsage GetPropertyType(RootTypeInfo typeInfo, PropertyRef p)
    {
      TypeUsage type = (TypeUsage) null;
      PropertyRef propertyRef = (PropertyRef) null;
      while (p is NestedPropertyRef)
      {
        NestedPropertyRef nestedPropertyRef = (NestedPropertyRef) p;
        p = nestedPropertyRef.OuterProperty;
        propertyRef = nestedPropertyRef.InnerProperty;
      }
      if (p is TypeIdPropertyRef)
      {
        SimplePropertyRef simplePropertyRef = (SimplePropertyRef) propertyRef;
        type = simplePropertyRef == null ? typeInfo.TypeIdType : this.GetTypeInfo(simplePropertyRef.Property.TypeUsage).RootType.TypeIdType;
      }
      else if (p is EntitySetIdPropertyRef || p is NullSentinelPropertyRef)
        type = this.m_intType;
      else if (p is RelPropertyRef)
      {
        type = ((RelPropertyRef) p).Property.ToEnd.TypeUsage;
      }
      else
      {
        SimplePropertyRef simplePropertyRef = p as SimplePropertyRef;
        if (simplePropertyRef != null)
          type = Helper.GetModelTypeUsage(simplePropertyRef.Property);
      }
      TypeUsage newType = this.GetNewType(type);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(null != newType, "unrecognized property type?");
      return newType;
    }

    private static EntityTypeBase GetRootType(EntityTypeBase type)
    {
      while (type.BaseType != null)
        type = (EntityTypeBase) type.BaseType;
      return type;
    }
  }
}
