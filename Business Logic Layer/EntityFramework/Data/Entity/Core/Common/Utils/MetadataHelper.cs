// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.MetadataHelper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;

namespace System.Data.Entity.Core.Common.Utils
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal static class MetadataHelper
  {
    internal static bool TryGetFunctionImportReturnType<T>(
      EdmFunction functionImport,
      int resultSetIndex,
      out T returnType)
      where T : EdmType
    {
      T resultType;
      if (MetadataHelper.TryGetWrappedReturnEdmTypeFromFunctionImport<T>(functionImport, resultSetIndex, out resultType) && (typeof (EntityType).Equals(typeof (T)) && (object) resultType is EntityType || typeof (ComplexType).Equals(typeof (T)) && (object) resultType is ComplexType || (typeof (StructuralType).Equals(typeof (T)) && (object) resultType is StructuralType || typeof (EdmType).Equals(typeof (T)) && (object) resultType != null)))
      {
        returnType = resultType;
        return true;
      }
      returnType = default (T);
      return false;
    }

    private static bool TryGetWrappedReturnEdmTypeFromFunctionImport<T>(
      EdmFunction functionImport,
      int resultSetIndex,
      out T resultType)
      where T : EdmType
    {
      resultType = default (T);
      CollectionType collectionType;
      if (!MetadataHelper.TryGetFunctionImportReturnCollectionType(functionImport, resultSetIndex, out collectionType))
        return false;
      resultType = collectionType.TypeUsage.EdmType as T;
      return true;
    }

    internal static bool TryGetFunctionImportReturnCollectionType(
      EdmFunction functionImport,
      int resultSetIndex,
      out CollectionType collectionType)
    {
      FunctionParameter returnParameter = MetadataHelper.GetReturnParameter(functionImport, resultSetIndex);
      if (returnParameter != null && returnParameter.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
      {
        collectionType = (CollectionType) returnParameter.TypeUsage.EdmType;
        return true;
      }
      collectionType = (CollectionType) null;
      return false;
    }

    internal static FunctionParameter GetReturnParameter(
      EdmFunction functionImport,
      int resultSetIndex)
    {
      if (functionImport.ReturnParameters.Count <= resultSetIndex)
        return (FunctionParameter) null;
      return functionImport.ReturnParameters[resultSetIndex];
    }

    internal static EdmFunction GetFunctionImport(
      string functionName,
      string defaultContainerName,
      MetadataWorkspace workspace,
      out string containerName,
      out string functionImportName)
    {
      CommandHelper.ParseFunctionImportCommandText(functionName, defaultContainerName, out containerName, out functionImportName);
      return CommandHelper.FindFunctionImport(workspace, containerName, functionImportName);
    }

    internal static EdmType GetAndCheckFunctionImportReturnType<TElement>(
      EdmFunction functionImport,
      int resultSetIndex,
      MetadataWorkspace workspace)
    {
      EdmType returnType;
      if (!MetadataHelper.TryGetFunctionImportReturnType<EdmType>(functionImport, resultSetIndex, out returnType))
        throw EntityUtil.ExecuteFunctionCalledWithNonReaderFunction(functionImport);
      MetadataHelper.CheckFunctionImportReturnType<TElement>(returnType, workspace);
      return returnType;
    }

    internal static void CheckFunctionImportReturnType<TElement>(
      EdmType expectedEdmType,
      MetadataWorkspace workspace)
    {
      EdmType edmType = expectedEdmType;
      bool isGeographic;
      if (Helper.IsSpatialType(expectedEdmType, out isGeographic))
        edmType = (EdmType) PrimitiveType.GetEdmPrimitiveType(isGeographic ? PrimitiveTypeKind.Geography : PrimitiveTypeKind.Geometry);
      EdmType modelEdmType;
      if (!workspace.TryDetermineCSpaceModelType<TElement>(out modelEdmType) || !modelEdmType.EdmEquals((MetadataItem) edmType))
        throw new InvalidOperationException(Strings.ObjectContext_ExecuteFunctionTypeMismatch((object) typeof (TElement).FullName, (object) expectedEdmType.FullName));
    }

    internal static ParameterDirection ParameterModeToParameterDirection(
      ParameterMode mode)
    {
      switch (mode)
      {
        case ParameterMode.In:
          return ParameterDirection.Input;
        case ParameterMode.Out:
          return ParameterDirection.Output;
        case ParameterMode.InOut:
          return ParameterDirection.InputOutput;
        case ParameterMode.ReturnValue:
          return ParameterDirection.ReturnValue;
        default:
          return (ParameterDirection) 0;
      }
    }

    internal static bool DoesMemberExist(StructuralType type, EdmMember member)
    {
      foreach (object member1 in type.Members)
      {
        if (member1.Equals((object) member))
          return true;
      }
      return false;
    }

    internal static bool IsNonRefSimpleMember(EdmMember member)
    {
      if (member.TypeUsage.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
        return member.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.EnumType;
      return true;
    }

    internal static bool HasDiscreteDomain(EdmType edmType)
    {
      PrimitiveType primitiveType = edmType as PrimitiveType;
      if (primitiveType != null)
        return primitiveType.PrimitiveTypeKind == PrimitiveTypeKind.Boolean;
      return false;
    }

    internal static EntityType GetEntityTypeForEnd(AssociationEndMember end)
    {
      return (EntityType) ((RefType) end.TypeUsage.EdmType).ElementType;
    }

    internal static EntitySet GetEntitySetAtEnd(
      AssociationSet associationSet,
      AssociationEndMember endMember)
    {
      return associationSet.AssociationSetEnds[endMember.Name].EntitySet;
    }

    internal static AssociationEndMember GetOtherAssociationEnd(
      AssociationEndMember endMember)
    {
      ReadOnlyMetadataCollection<EdmMember> members = endMember.DeclaringType.Members;
      EdmMember edmMember = members[0];
      if (!object.ReferenceEquals((object) endMember, (object) edmMember))
        return (AssociationEndMember) edmMember;
      return (AssociationEndMember) members[1];
    }

    internal static bool IsEveryOtherEndAtLeastOne(
      AssociationSet associationSet,
      AssociationEndMember member)
    {
      foreach (AssociationSetEnd associationSetEnd in associationSet.AssociationSetEnds)
      {
        AssociationEndMember associationEndMember = associationSetEnd.CorrespondingAssociationEndMember;
        if (!associationEndMember.Equals((object) member) && MetadataHelper.GetLowerBoundOfMultiplicity(associationEndMember.RelationshipMultiplicity) == 0)
          return false;
      }
      return true;
    }

    internal static bool IsAssociationValidForEntityType(AssociationSetEnd toEnd, EntityType type)
    {
      return MetadataHelper.GetEntityTypeForEnd(MetadataHelper.GetOppositeEnd(toEnd).CorrespondingAssociationEndMember).IsAssignableFrom((EdmType) type);
    }

    internal static AssociationSetEnd GetOppositeEnd(AssociationSetEnd end)
    {
      return end.ParentAssociationSet.AssociationSetEnds.Where<AssociationSetEnd>((Func<AssociationSetEnd, bool>) (e => !e.EdmEquals((MetadataItem) end))).Single<AssociationSetEnd>();
    }

    internal static bool IsComposable(EdmFunction function)
    {
      MetadataProperty metadataProperty;
      if (function.MetadataProperties.TryGetValue("IsComposableAttribute", false, out metadataProperty))
        return (bool) metadataProperty.Value;
      return !function.IsFunctionImport;
    }

    internal static bool IsMemberNullable(EdmMember member)
    {
      if (Helper.IsEdmProperty(member))
        return ((EdmProperty) member).Nullable;
      return false;
    }

    internal static IEnumerable<EntitySet> GetInfluencingEntitySetsForTable(
      EntitySet table,
      MetadataWorkspace workspace)
    {
      ItemCollection collection = (ItemCollection) null;
      workspace.TryGetItemCollection(DataSpace.CSSpace, out collection);
      return MappingMetadataHelper.GetEntityContainerMap((StorageMappingItemCollection) collection, table.EntityContainer).EntitySetMaps.Where<EntitySetBaseMapping>((Func<EntitySetBaseMapping, bool>) (map => map.TypeMappings.Any<TypeMapping>((Func<TypeMapping, bool>) (typeMap => typeMap.MappingFragments.Any<MappingFragment>((Func<MappingFragment, bool>) (mappingFrag => mappingFrag.TableSet.EdmEquals((MetadataItem) table))))))).Select<EntitySetBaseMapping, EntitySetBase>((Func<EntitySetBaseMapping, EntitySetBase>) (m => m.Set)).Cast<EntitySet>().Distinct<EntitySet>();
    }

    internal static IEnumerable<EdmType> GetTypeAndSubtypesOf(
      EdmType type,
      MetadataWorkspace workspace,
      bool includeAbstractTypes)
    {
      return MetadataHelper.GetTypeAndSubtypesOf(type, workspace.GetItemCollection(DataSpace.CSpace), includeAbstractTypes);
    }

    internal static IEnumerable<EdmType> GetTypeAndSubtypesOf(
      EdmType type,
      ItemCollection itemCollection,
      bool includeAbstractTypes)
    {
      if (Helper.IsRefType((GlobalItem) type))
        type = (EdmType) ((RefType) type).ElementType;
      if (includeAbstractTypes || !type.Abstract)
        yield return type;
      foreach (EdmType edmType in MetadataHelper.GetTypeAndSubtypesOf<EntityType>(type, itemCollection, includeAbstractTypes))
        yield return edmType;
      foreach (EdmType edmType in MetadataHelper.GetTypeAndSubtypesOf<ComplexType>(type, itemCollection, includeAbstractTypes))
        yield return edmType;
    }

    private static IEnumerable<EdmType> GetTypeAndSubtypesOf<T_EdmType>(
      EdmType type,
      ItemCollection itemCollection,
      bool includeAbstractTypes)
      where T_EdmType : EdmType
    {
      T_EdmType specificType = type as T_EdmType;
      if ((object) specificType != null)
      {
        IEnumerable<T_EdmType> typesInWorkSpace = (IEnumerable<T_EdmType>) itemCollection.GetItems<T_EdmType>();
        foreach (T_EdmType edmType in typesInWorkSpace)
        {
          if (!specificType.Equals((object) edmType) && Helper.IsSubtypeOf((EdmType) edmType, (EdmType) specificType) && (includeAbstractTypes || !edmType.Abstract))
            yield return (EdmType) edmType;
        }
      }
    }

    internal static IEnumerable<EdmType> GetTypeAndParentTypesOf(
      EdmType type,
      bool includeAbstractTypes)
    {
      if (Helper.IsRefType((GlobalItem) type))
        type = (EdmType) ((RefType) type).ElementType;
      for (EdmType specificType = type; specificType != null; specificType = (EdmType) (specificType.BaseType as EntityType))
      {
        if (includeAbstractTypes || !specificType.Abstract)
          yield return specificType;
      }
    }

    internal static Dictionary<EntityType, Set<EntityType>> BuildUndirectedGraphOfTypes(
      EdmItemCollection edmItemCollection)
    {
      Dictionary<EntityType, Set<EntityType>> graph = new Dictionary<EntityType, Set<EntityType>>();
      foreach (EntityType entityType in (IEnumerable<EntityType>) edmItemCollection.GetItems<EntityType>())
      {
        if (entityType.BaseType != null)
        {
          EntityType baseType = entityType.BaseType as EntityType;
          MetadataHelper.AddDirectedEdgeBetweenEntityTypes(graph, entityType, baseType);
          MetadataHelper.AddDirectedEdgeBetweenEntityTypes(graph, baseType, entityType);
        }
      }
      return graph;
    }

    internal static bool IsParentOf(EntityType a, EntityType b)
    {
      for (EntityType baseType = b.BaseType as EntityType; baseType != null; baseType = baseType.BaseType as EntityType)
      {
        if (baseType.EdmEquals((MetadataItem) a))
          return true;
      }
      return false;
    }

    private static void AddDirectedEdgeBetweenEntityTypes(
      Dictionary<EntityType, Set<EntityType>> graph,
      EntityType a,
      EntityType b)
    {
      Set<EntityType> set;
      if (graph.ContainsKey(a))
      {
        set = graph[a];
      }
      else
      {
        set = new Set<EntityType>();
        graph.Add(a, set);
      }
      set.Add(b);
    }

    [SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCode", Justification = "Based on Bug VSTS Pioneer #433188: IsVisibleOutsideAssembly is wrong on generic instantiations.")]
    internal static bool DoesEndKeySubsumeAssociationSetKey(
      AssociationSet assocSet,
      AssociationEndMember thisEnd,
      HashSet<Pair<EdmMember, EntityType>> associationkeys)
    {
      AssociationType elementType1 = assocSet.ElementType;
      EntityType thisEndsEntityType = (EntityType) ((RefType) thisEnd.TypeUsage.EdmType).ElementType;
      HashSet<Pair<EdmMember, EntityType>> pairSet = new HashSet<Pair<EdmMember, EntityType>>(thisEndsEntityType.KeyMembers.Select<EdmMember, Pair<EdmMember, EntityType>>((Func<EdmMember, Pair<EdmMember, EntityType>>) (edmMember => new Pair<EdmMember, EntityType>(edmMember, thisEndsEntityType))));
      foreach (ReferentialConstraint referentialConstraint in elementType1.ReferentialConstraints)
      {
        IEnumerable<EdmMember> edmMembers;
        EntityType elementType2;
        if (thisEnd.Equals((object) referentialConstraint.ToRole))
        {
          edmMembers = Helpers.AsSuperTypeList<EdmProperty, EdmMember>((IEnumerable<EdmProperty>) referentialConstraint.FromProperties);
          elementType2 = (EntityType) ((RefType) referentialConstraint.FromRole.TypeUsage.EdmType).ElementType;
        }
        else if (thisEnd.Equals((object) referentialConstraint.FromRole))
        {
          edmMembers = Helpers.AsSuperTypeList<EdmProperty, EdmMember>((IEnumerable<EdmProperty>) referentialConstraint.ToProperties);
          elementType2 = (EntityType) ((RefType) referentialConstraint.ToRole.TypeUsage.EdmType).ElementType;
        }
        else
          continue;
        foreach (EdmMember first in edmMembers)
          associationkeys.Remove(new Pair<EdmMember, EntityType>(first, elementType2));
      }
      return associationkeys.IsSubsetOf((IEnumerable<Pair<EdmMember, EntityType>>) pairSet);
    }

    internal static bool DoesEndFormKey(AssociationSet associationSet, AssociationEndMember end)
    {
      foreach (AssociationEndMember member in associationSet.ElementType.Members)
      {
        if (!member.Equals((object) end) && member.RelationshipMultiplicity == RelationshipMultiplicity.Many)
          return false;
      }
      return true;
    }

    internal static bool IsExtentAtSomeRelationshipEnd(
      AssociationSet relationshipSet,
      EntitySetBase extent)
    {
      if (Helper.IsEntitySet(extent))
        return MetadataHelper.GetSomeEndForEntitySet(relationshipSet, extent) != null;
      return false;
    }

    internal static AssociationEndMember GetSomeEndForEntitySet(
      AssociationSet associationSet,
      EntitySetBase entitySet)
    {
      foreach (AssociationSetEnd associationSetEnd in associationSet.AssociationSetEnds)
      {
        if (associationSetEnd.EntitySet.Equals((object) entitySet))
          return associationSetEnd.CorrespondingAssociationEndMember;
      }
      return (AssociationEndMember) null;
    }

    internal static List<AssociationSet> GetAssociationsForEntitySets(
      EntitySet entitySet1,
      EntitySet entitySet2)
    {
      List<AssociationSet> associationSetList = new List<AssociationSet>();
      foreach (EntitySetBase baseEntitySet in entitySet1.EntityContainer.BaseEntitySets)
      {
        if (Helper.IsRelationshipSet(baseEntitySet))
        {
          AssociationSet relationshipSet = (AssociationSet) baseEntitySet;
          if (MetadataHelper.IsExtentAtSomeRelationshipEnd(relationshipSet, (EntitySetBase) entitySet1) && MetadataHelper.IsExtentAtSomeRelationshipEnd(relationshipSet, (EntitySetBase) entitySet2))
            associationSetList.Add(relationshipSet);
        }
      }
      return associationSetList;
    }

    internal static List<AssociationSet> GetAssociationsForEntitySet(
      EntitySetBase entitySet)
    {
      List<AssociationSet> associationSetList = new List<AssociationSet>();
      foreach (EntitySetBase baseEntitySet in entitySet.EntityContainer.BaseEntitySets)
      {
        if (Helper.IsRelationshipSet(baseEntitySet))
        {
          AssociationSet relationshipSet = (AssociationSet) baseEntitySet;
          if (MetadataHelper.IsExtentAtSomeRelationshipEnd(relationshipSet, entitySet))
            associationSetList.Add(relationshipSet);
        }
      }
      return associationSetList;
    }

    internal static bool IsSuperTypeOf(EdmType superType, EdmType subType)
    {
      for (EdmType edmType = subType; edmType != null; edmType = edmType.BaseType)
      {
        if (edmType.Equals((object) superType))
          return true;
      }
      return false;
    }

    internal static bool IsPartOfEntityTypeKey(EdmMember member)
    {
      if (Helper.IsEntityType((EdmType) member.DeclaringType) && Helper.IsEdmProperty(member))
        return ((EntityTypeBase) member.DeclaringType).KeyMembers.Contains(member);
      return false;
    }

    internal static TypeUsage GetElementType(TypeUsage typeUsage)
    {
      if (BuiltInTypeKind.CollectionType == typeUsage.EdmType.BuiltInTypeKind)
        return MetadataHelper.GetElementType(((CollectionType) typeUsage.EdmType).TypeUsage);
      return typeUsage;
    }

    internal static int GetLowerBoundOfMultiplicity(RelationshipMultiplicity multiplicity)
    {
      return multiplicity == RelationshipMultiplicity.Many || multiplicity == RelationshipMultiplicity.ZeroOrOne ? 0 : 1;
    }

    internal static int? GetUpperBoundOfMultiplicity(RelationshipMultiplicity multiplicity)
    {
      if (multiplicity == RelationshipMultiplicity.One || multiplicity == RelationshipMultiplicity.ZeroOrOne)
        return new int?(1);
      return new int?();
    }

    internal static Set<EdmMember> GetConcurrencyMembersForTypeHierarchy(
      EntityTypeBase superType,
      EdmItemCollection edmItemCollection)
    {
      Set<EdmMember> set = new Set<EdmMember>();
      foreach (StructuralType structuralType in MetadataHelper.GetTypeAndSubtypesOf((EdmType) superType, (ItemCollection) edmItemCollection, true))
      {
        foreach (EdmMember member in structuralType.Members)
        {
          if (MetadataHelper.GetConcurrencyMode(member) == ConcurrencyMode.Fixed)
            set.Add(member);
        }
      }
      return set;
    }

    internal static ConcurrencyMode GetConcurrencyMode(EdmMember member)
    {
      return MetadataHelper.GetConcurrencyMode(member.TypeUsage);
    }

    internal static ConcurrencyMode GetConcurrencyMode(TypeUsage typeUsage)
    {
      Facet facet;
      if (typeUsage.Facets.TryGetValue("ConcurrencyMode", false, out facet) && facet.Value != null)
        return (ConcurrencyMode) facet.Value;
      return ConcurrencyMode.None;
    }

    internal static StoreGeneratedPattern GetStoreGeneratedPattern(
      EdmMember member)
    {
      Facet facet;
      if (member.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out facet) && facet.Value != null)
        return (StoreGeneratedPattern) facet.Value;
      return StoreGeneratedPattern.None;
    }

    internal static bool CheckIfAllErrorsAreWarnings(IList<EdmSchemaError> schemaErrors)
    {
      int count = schemaErrors.Count;
      for (int index = 0; index < count; ++index)
      {
        if (schemaErrors[index].Severity != EdmSchemaErrorSeverity.Warning)
          return false;
      }
      return true;
    }

    [SuppressMessage("Microsoft.Cryptographic.Standard", "CA5350:Microsoft.Cryptographic.Standard", Justification = "MD5CryptoServiceProvider is not used for cryptography/security purposes and we do it only for v1 and v1.1 for compatibility reasons.")]
    internal static HashAlgorithm CreateMetadataHashAlgorithm(double schemaVersion)
    {
      return schemaVersion >= 2.0 ? (HashAlgorithm) MetadataHelper.CreateSHA256HashAlgorithm() : (HashAlgorithm) new MD5CryptoServiceProvider();
    }

    internal static SHA256 CreateSHA256HashAlgorithm()
    {
      try
      {
        return (SHA256) new SHA256CryptoServiceProvider();
      }
      catch (PlatformNotSupportedException ex)
      {
        return (SHA256) new SHA256Managed();
      }
    }

    internal static TypeUsage ConvertStoreTypeUsageToEdmTypeUsage(TypeUsage storeTypeUsage)
    {
      return storeTypeUsage.ModelTypeUsage.ShallowCopy(FacetValues.NullFacetValues);
    }

    internal static byte GetPrecision(this TypeUsage type)
    {
      return type.GetFacetValue<byte>("Precision");
    }

    internal static byte GetScale(this TypeUsage type)
    {
      return type.GetFacetValue<byte>("Scale");
    }

    internal static int GetMaxLength(this TypeUsage type)
    {
      return type.GetFacetValue<int>("MaxLength");
    }

    internal static T GetFacetValue<T>(this TypeUsage type, string facetName)
    {
      return (T) type.Facets[facetName].Value;
    }

    internal static NavigationPropertyAccessor GetNavigationPropertyAccessor(
      EntityType sourceEntityType,
      AssociationEndMember sourceMember,
      AssociationEndMember targetMember)
    {
      return MetadataHelper.GetNavigationPropertyAccessor(sourceEntityType, sourceMember.DeclaringType.FullName, sourceMember.Name, targetMember.Name);
    }

    internal static NavigationPropertyAccessor GetNavigationPropertyAccessor(
      EntityType entityType,
      string relationshipType,
      string fromName,
      string toName)
    {
      NavigationProperty navigationProperty;
      if (entityType.TryGetNavigationProperty(relationshipType, fromName, toName, out navigationProperty))
        return navigationProperty.Accessor;
      return NavigationPropertyAccessor.NoNavigationProperty;
    }
  }
}
