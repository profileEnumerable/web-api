// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ColumnMapProcessor
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

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class ColumnMapProcessor
  {
    private const string c_TypeIdColumnName = "__TypeId";
    private const string c_EntitySetIdColumnName = "__EntitySetId";
    private const string c_NullSentinelColumnName = "__NullSentinel";
    private readonly IEnumerator<Var> m_varList;
    private readonly VarInfo m_varInfo;
    private readonly VarRefColumnMap m_columnMap;
    private readonly StructuredTypeInfo m_typeInfo;

    internal ColumnMap ExpandColumnMap()
    {
      if (this.m_varInfo.Kind == VarInfoKind.CollectionVarInfo)
        return (ColumnMap) new VarRefColumnMap(this.m_columnMap.Var.Type, this.m_columnMap.Name, ((CollectionVarInfo) this.m_varInfo).NewVar);
      if (this.m_varInfo.Kind == VarInfoKind.PrimitiveTypeVarInfo)
        return (ColumnMap) new VarRefColumnMap(this.m_columnMap.Var.Type, this.m_columnMap.Name, ((PrimitiveTypeVarInfo) this.m_varInfo).NewVar);
      return this.CreateColumnMap(this.m_columnMap.Var.Type, this.m_columnMap.Name);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Vars")]
    internal ColumnMapProcessor(
      VarRefColumnMap columnMap,
      VarInfo varInfo,
      StructuredTypeInfo typeInfo)
    {
      this.m_columnMap = columnMap;
      this.m_varInfo = varInfo;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(varInfo.NewVars != null && varInfo.NewVars.Count > 0, "No new Vars specified");
      this.m_varList = (IEnumerator<Var>) varInfo.NewVars.GetEnumerator();
      this.m_typeInfo = typeInfo;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetNextVar")]
    private Var GetNextVar()
    {
      if (this.m_varList.MoveNext())
        return this.m_varList.Current;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "Could not GetNextVar");
      return (Var) null;
    }

    private ColumnMap CreateColumnMap(TypeUsage type, string name)
    {
      if (!TypeUtils.IsStructuredType(type))
        return (ColumnMap) this.CreateSimpleColumnMap(type, name);
      return this.CreateStructuralColumnMap(type, name);
    }

    private ComplexTypeColumnMap CreateComplexTypeColumnMap(
      TypeInfo typeInfo,
      string name,
      ComplexTypeColumnMap superTypeColumnMap,
      Dictionary<object, TypedColumnMap> discriminatorMap,
      List<TypedColumnMap> allMaps)
    {
      List<ColumnMap> columnMapList = new List<ColumnMap>();
      SimpleColumnMap nullSentinel = (SimpleColumnMap) null;
      if (typeInfo.HasNullSentinelProperty)
        nullSentinel = this.CreateSimpleColumnMap(Helper.GetModelTypeUsage((EdmMember) typeInfo.NullSentinelProperty), "__NullSentinel");
      IEnumerable structuralMembers;
      if (superTypeColumnMap != null)
      {
        foreach (ColumnMap property in superTypeColumnMap.Properties)
          columnMapList.Add(property);
        structuralMembers = TypeHelpers.GetDeclaredStructuralMembers(typeInfo.Type);
      }
      else
        structuralMembers = (IEnumerable) TypeHelpers.GetAllStructuralMembers(typeInfo.Type);
      foreach (EdmMember member in structuralMembers)
      {
        ColumnMap columnMap = this.CreateColumnMap(Helper.GetModelTypeUsage(member), member.Name);
        columnMapList.Add(columnMap);
      }
      ComplexTypeColumnMap superTypeColumnMap1 = new ComplexTypeColumnMap(typeInfo.Type, name, columnMapList.ToArray(), nullSentinel);
      if (discriminatorMap != null)
        discriminatorMap[typeInfo.TypeId] = (TypedColumnMap) superTypeColumnMap1;
      allMaps?.Add((TypedColumnMap) superTypeColumnMap1);
      foreach (TypeInfo immediateSubType in typeInfo.ImmediateSubTypes)
        this.CreateComplexTypeColumnMap(immediateSubType, name, superTypeColumnMap1, discriminatorMap, allMaps);
      return superTypeColumnMap1;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EntityType")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "keyColumnMap")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private EntityColumnMap CreateEntityColumnMap(
      TypeInfo typeInfo,
      string name,
      EntityColumnMap superTypeColumnMap,
      Dictionary<object, TypedColumnMap> discriminatorMap,
      List<TypedColumnMap> allMaps,
      bool handleRelProperties)
    {
      List<ColumnMap> columnMapList = new List<ColumnMap>();
      EntityColumnMap superTypeColumnMap1;
      if (superTypeColumnMap != null)
      {
        foreach (ColumnMap property in superTypeColumnMap.Properties)
          columnMapList.Add(property);
        foreach (EdmMember structuralMember in TypeHelpers.GetDeclaredStructuralMembers(typeInfo.Type))
        {
          ColumnMap columnMap = this.CreateColumnMap(Helper.GetModelTypeUsage(structuralMember), structuralMember.Name);
          columnMapList.Add(columnMap);
        }
        superTypeColumnMap1 = new EntityColumnMap(typeInfo.Type, name, columnMapList.ToArray(), superTypeColumnMap.EntityIdentity);
      }
      else
      {
        SimpleColumnMap entitySetIdColumnMap = (SimpleColumnMap) null;
        if (typeInfo.HasEntitySetIdProperty)
          entitySetIdColumnMap = this.CreateEntitySetIdColumnMap(typeInfo.EntitySetIdProperty);
        List<SimpleColumnMap> simpleColumnMapList = new List<SimpleColumnMap>();
        Dictionary<EdmProperty, ColumnMap> dictionary = new Dictionary<EdmProperty, ColumnMap>();
        foreach (EdmMember structuralMember in TypeHelpers.GetDeclaredStructuralMembers(typeInfo.Type))
        {
          ColumnMap columnMap = this.CreateColumnMap(Helper.GetModelTypeUsage(structuralMember), structuralMember.Name);
          columnMapList.Add(columnMap);
          if (TypeSemantics.IsPartOfKey(structuralMember))
          {
            EdmProperty index = structuralMember as EdmProperty;
            System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(index != null, "EntityType key member is not property?");
            dictionary[index] = columnMap;
          }
        }
        foreach (EdmMember keyMember in TypeHelpers.GetEdmType<EntityType>(typeInfo.Type).KeyMembers)
        {
          EdmProperty index = keyMember as EdmProperty;
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(index != null, "EntityType key member is not property?");
          SimpleColumnMap simpleColumnMap = dictionary[index] as SimpleColumnMap;
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(simpleColumnMap != null, "keyColumnMap is null");
          simpleColumnMapList.Add(simpleColumnMap);
        }
        EntityIdentity entityIdentity = this.CreateEntityIdentity((EntityType) typeInfo.Type.EdmType, entitySetIdColumnMap, simpleColumnMapList.ToArray());
        superTypeColumnMap1 = new EntityColumnMap(typeInfo.Type, name, columnMapList.ToArray(), entityIdentity);
      }
      if (discriminatorMap != null && typeInfo.TypeId != null)
        discriminatorMap[typeInfo.TypeId] = (TypedColumnMap) superTypeColumnMap1;
      allMaps?.Add((TypedColumnMap) superTypeColumnMap1);
      foreach (TypeInfo immediateSubType in typeInfo.ImmediateSubTypes)
        this.CreateEntityColumnMap(immediateSubType, name, superTypeColumnMap1, discriminatorMap, allMaps, false);
      if (handleRelProperties)
        this.BuildRelPropertyColumnMaps(typeInfo, true);
      return superTypeColumnMap1;
    }

    private void BuildRelPropertyColumnMaps(TypeInfo typeInfo, bool includeSupertypeRelProperties)
    {
      foreach (RelProperty relProperty in !includeSupertypeRelProperties ? this.m_typeInfo.RelPropertyHelper.GetDeclaredOnlyRelProperties(typeInfo.Type.EdmType as EntityTypeBase) : this.m_typeInfo.RelPropertyHelper.GetRelProperties(typeInfo.Type.EdmType as EntityTypeBase))
        this.CreateColumnMap(relProperty.ToEnd.TypeUsage, relProperty.ToString());
      foreach (TypeInfo immediateSubType in typeInfo.ImmediateSubTypes)
        this.BuildRelPropertyColumnMaps(immediateSubType, false);
    }

    private SimpleColumnMap CreateEntitySetIdColumnMap(EdmProperty prop)
    {
      return this.CreateSimpleColumnMap(Helper.GetModelTypeUsage((EdmMember) prop), "__EntitySetId");
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private SimplePolymorphicColumnMap CreatePolymorphicColumnMap(
      TypeInfo typeInfo,
      string name)
    {
      Dictionary<object, TypedColumnMap> dictionary = new Dictionary<object, TypedColumnMap>(typeInfo.RootType.DiscriminatorMap == null ? (IEqualityComparer<object>) null : (IEqualityComparer<object>) TrailingSpaceComparer.Instance);
      List<TypedColumnMap> allMaps = new List<TypedColumnMap>();
      TypeInfo rootType = (TypeInfo) typeInfo.RootType;
      SimpleColumnMap typeIdColumnMap = this.CreateTypeIdColumnMap(rootType.TypeIdProperty);
      if (TypeSemantics.IsComplexType(typeInfo.Type))
        this.CreateComplexTypeColumnMap(rootType, name, (ComplexTypeColumnMap) null, dictionary, allMaps);
      else
        this.CreateEntityColumnMap(rootType, name, (EntityColumnMap) null, dictionary, allMaps, true);
      TypedColumnMap typedColumnMap1 = (TypedColumnMap) null;
      foreach (TypedColumnMap typedColumnMap2 in allMaps)
      {
        if (TypeSemantics.IsStructurallyEqual(typedColumnMap2.Type, typeInfo.Type))
        {
          typedColumnMap1 = typedColumnMap2;
          break;
        }
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(null != typedColumnMap1, "Didn't find requested type in polymorphic type hierarchy?");
      return new SimplePolymorphicColumnMap(typeInfo.Type, name, typedColumnMap1.Properties, typeIdColumnMap, dictionary);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RowType")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private RecordColumnMap CreateRecordColumnMap(TypeInfo typeInfo, string name)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(typeInfo.Type.EdmType is RowType, "not RowType");
      SimpleColumnMap nullSentinel = (SimpleColumnMap) null;
      if (typeInfo.HasNullSentinelProperty)
        nullSentinel = this.CreateSimpleColumnMap(Helper.GetModelTypeUsage((EdmMember) typeInfo.NullSentinelProperty), "__NullSentinel");
      ReadOnlyMetadataCollection<EdmProperty> properties1 = TypeHelpers.GetProperties(typeInfo.Type);
      ColumnMap[] properties2 = new ColumnMap[properties1.Count];
      for (int index = 0; index < properties2.Length; ++index)
      {
        EdmMember member = (EdmMember) properties1[index];
        properties2[index] = this.CreateColumnMap(Helper.GetModelTypeUsage(member), member.Name);
      }
      return new RecordColumnMap(typeInfo.Type, name, properties2, nullSentinel);
    }

    private RefColumnMap CreateRefColumnMap(TypeInfo typeInfo, string name)
    {
      SimpleColumnMap entitySetIdColumnMap = (SimpleColumnMap) null;
      if (typeInfo.HasEntitySetIdProperty)
        entitySetIdColumnMap = this.CreateSimpleColumnMap(Helper.GetModelTypeUsage((EdmMember) typeInfo.EntitySetIdProperty), "__EntitySetId");
      EntityType elementType = (EntityType) TypeHelpers.GetEdmType<RefType>(typeInfo.Type).ElementType;
      SimpleColumnMap[] keyColumnMaps = new SimpleColumnMap[elementType.KeyMembers.Count];
      for (int index = 0; index < keyColumnMaps.Length; ++index)
      {
        EdmMember keyMember = elementType.KeyMembers[index];
        keyColumnMaps[index] = this.CreateSimpleColumnMap(Helper.GetModelTypeUsage(keyMember), keyMember.Name);
      }
      EntityIdentity entityIdentity = this.CreateEntityIdentity(elementType, entitySetIdColumnMap, keyColumnMaps);
      return new RefColumnMap(typeInfo.Type, name, entityIdentity);
    }

    private SimpleColumnMap CreateSimpleColumnMap(TypeUsage type, string name)
    {
      Var nextVar = this.GetNextVar();
      return (SimpleColumnMap) new VarRefColumnMap(type, name, nextVar);
    }

    private SimpleColumnMap CreateTypeIdColumnMap(EdmProperty prop)
    {
      return this.CreateSimpleColumnMap(Helper.GetModelTypeUsage((EdmMember) prop), "__TypeId");
    }

    private ColumnMap CreateStructuralColumnMap(TypeUsage type, string name)
    {
      TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(type);
      if (TypeSemantics.IsRowType(type))
        return (ColumnMap) this.CreateRecordColumnMap(typeInfo, name);
      if (TypeSemantics.IsReferenceType(type))
        return (ColumnMap) this.CreateRefColumnMap(typeInfo, name);
      if (typeInfo.HasTypeIdProperty)
        return (ColumnMap) this.CreatePolymorphicColumnMap(typeInfo, name);
      if (TypeSemantics.IsComplexType(type))
        return (ColumnMap) this.CreateComplexTypeColumnMap(typeInfo, name, (ComplexTypeColumnMap) null, (Dictionary<object, TypedColumnMap>) null, (List<TypedColumnMap>) null);
      if (TypeSemantics.IsEntityType(type))
        return (ColumnMap) this.CreateEntityColumnMap(typeInfo, name, (EntityColumnMap) null, (Dictionary<object, TypedColumnMap>) null, (List<TypedColumnMap>) null, true);
      throw new NotSupportedException(type.Identity);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "entitySet")]
    private EntityIdentity CreateEntityIdentity(
      EntityType entityType,
      SimpleColumnMap entitySetIdColumnMap,
      SimpleColumnMap[] keyColumnMaps)
    {
      if (entitySetIdColumnMap != null)
        return (EntityIdentity) new DiscriminatedEntityIdentity(entitySetIdColumnMap, this.m_typeInfo.EntitySetIdToEntitySetMap, keyColumnMaps);
      EntitySet entitySet = this.m_typeInfo.GetEntitySet((EntityTypeBase) entityType);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(entitySet != null, "Expected non-null entitySet when no entity set ID is required. Entity type = " + (object) entityType);
      return (EntityIdentity) new SimpleEntityIdentity(entitySet, keyColumnMaps);
    }
  }
}
