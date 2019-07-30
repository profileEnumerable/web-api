// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.TypeSemantics
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal static class TypeSemantics
  {
    [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
    private static ReadOnlyCollection<PrimitiveType>[,] _commonTypeClosure;

    internal static bool IsEqual(TypeUsage type1, TypeUsage type2)
    {
      return TypeSemantics.CompareTypes(type1, type2, false);
    }

    internal static bool IsStructurallyEqual(TypeUsage fromType, TypeUsage toType)
    {
      return TypeSemantics.CompareTypes(fromType, toType, true);
    }

    internal static bool IsStructurallyEqualOrPromotableTo(TypeUsage fromType, TypeUsage toType)
    {
      if (!TypeSemantics.IsStructurallyEqual(fromType, toType))
        return TypeSemantics.IsPromotableTo(fromType, toType);
      return true;
    }

    internal static bool IsStructurallyEqualOrPromotableTo(EdmType fromType, EdmType toType)
    {
      return TypeSemantics.IsStructurallyEqualOrPromotableTo(TypeUsage.Create(fromType), TypeUsage.Create(toType));
    }

    internal static bool IsSubTypeOf(TypeUsage subType, TypeUsage superType)
    {
      if (subType.EdmEquals((MetadataItem) superType))
        return true;
      if (Helper.IsPrimitiveType(subType.EdmType) && Helper.IsPrimitiveType(superType.EdmType))
        return TypeSemantics.IsPrimitiveTypeSubTypeOf(subType, superType);
      return subType.IsSubtypeOf(superType);
    }

    internal static bool IsSubTypeOf(EdmType subEdmType, EdmType superEdmType)
    {
      return subEdmType.IsSubtypeOf(superEdmType);
    }

    internal static bool IsPromotableTo(TypeUsage fromType, TypeUsage toType)
    {
      if (toType.EdmType.EdmEquals((MetadataItem) fromType.EdmType))
        return true;
      if (Helper.IsPrimitiveType(fromType.EdmType) && Helper.IsPrimitiveType(toType.EdmType))
        return TypeSemantics.IsPrimitiveTypePromotableTo(fromType, toType);
      if (Helper.IsCollectionType((GlobalItem) fromType.EdmType) && Helper.IsCollectionType((GlobalItem) toType.EdmType))
        return TypeSemantics.IsPromotableTo(TypeHelpers.GetElementTypeUsage(fromType), TypeHelpers.GetElementTypeUsage(toType));
      if (Helper.IsEntityTypeBase(fromType.EdmType) && Helper.IsEntityTypeBase(toType.EdmType))
        return fromType.EdmType.IsSubtypeOf(toType.EdmType);
      if (Helper.IsRefType((GlobalItem) fromType.EdmType) && Helper.IsRefType((GlobalItem) toType.EdmType))
        return TypeSemantics.IsPromotableTo(TypeHelpers.GetElementTypeUsage(fromType), TypeHelpers.GetElementTypeUsage(toType));
      if (Helper.IsRowType((GlobalItem) fromType.EdmType) && Helper.IsRowType((GlobalItem) toType.EdmType))
        return TypeSemantics.IsPromotableTo((RowType) fromType.EdmType, (RowType) toType.EdmType);
      return false;
    }

    internal static IEnumerable<TypeUsage> FlattenType(TypeUsage type)
    {
      Func<TypeUsage, bool> isLeaf = (Func<TypeUsage, bool>) (t => !Helper.IsTransientType(t.EdmType));
      Func<TypeUsage, IEnumerable<TypeUsage>> getImmediateSubNodes = (Func<TypeUsage, IEnumerable<TypeUsage>>) (t =>
      {
        if (Helper.IsCollectionType((GlobalItem) t.EdmType) || Helper.IsRefType((GlobalItem) t.EdmType))
          return (IEnumerable<TypeUsage>) new TypeUsage[1]
          {
            TypeHelpers.GetElementTypeUsage(t)
          };
        if (Helper.IsRowType((GlobalItem) t.EdmType))
          return ((RowType) t.EdmType).Properties.Select<EdmProperty, TypeUsage>((Func<EdmProperty, TypeUsage>) (p => p.TypeUsage));
        return (IEnumerable<TypeUsage>) new TypeUsage[0];
      });
      return Helpers.GetLeafNodes<TypeUsage>(type, isLeaf, getImmediateSubNodes);
    }

    internal static bool IsCastAllowed(TypeUsage fromType, TypeUsage toType)
    {
      if (Helper.IsPrimitiveType(fromType.EdmType) && Helper.IsPrimitiveType(toType.EdmType) || Helper.IsPrimitiveType(fromType.EdmType) && Helper.IsEnumType(toType.EdmType) || Helper.IsEnumType(fromType.EdmType) && Helper.IsPrimitiveType(toType.EdmType))
        return true;
      if (Helper.IsEnumType(fromType.EdmType) && Helper.IsEnumType(toType.EdmType))
        return fromType.EdmType.Equals((object) toType.EdmType);
      return false;
    }

    internal static bool TryGetCommonType(
      TypeUsage type1,
      TypeUsage type2,
      out TypeUsage commonType)
    {
      commonType = (TypeUsage) null;
      if (type1.EdmEquals((MetadataItem) type2))
      {
        commonType = TypeSemantics.ForgetConstraints(type2);
        return true;
      }
      if (Helper.IsPrimitiveType(type1.EdmType) && Helper.IsPrimitiveType(type2.EdmType))
        return TypeSemantics.TryGetCommonPrimitiveType(type1, type2, out commonType);
      EdmType commonEdmType;
      if (TypeSemantics.TryGetCommonType(type1.EdmType, type2.EdmType, out commonEdmType))
      {
        commonType = TypeSemantics.ForgetConstraints(TypeUsage.Create(commonEdmType));
        return true;
      }
      commonType = (TypeUsage) null;
      return false;
    }

    internal static TypeUsage GetCommonType(TypeUsage type1, TypeUsage type2)
    {
      TypeUsage commonType = (TypeUsage) null;
      if (TypeSemantics.TryGetCommonType(type1, type2, out commonType))
        return commonType;
      return (TypeUsage) null;
    }

    internal static bool IsAggregateFunction(EdmFunction function)
    {
      return function.AggregateAttribute;
    }

    internal static bool IsValidPolymorphicCast(TypeUsage fromType, TypeUsage toType)
    {
      if (!TypeSemantics.IsPolymorphicType(fromType) || !TypeSemantics.IsPolymorphicType(toType))
        return false;
      if (!TypeSemantics.IsStructurallyEqual(fromType, toType) && !TypeSemantics.IsSubTypeOf(fromType, toType))
        return TypeSemantics.IsSubTypeOf(toType, fromType);
      return true;
    }

    internal static bool IsValidPolymorphicCast(EdmType fromEdmType, EdmType toEdmType)
    {
      return TypeSemantics.IsValidPolymorphicCast(TypeUsage.Create(fromEdmType), TypeUsage.Create(toEdmType));
    }

    internal static bool IsNominalType(TypeUsage type)
    {
      if (!TypeSemantics.IsEntityType(type))
        return TypeSemantics.IsComplexType(type);
      return true;
    }

    internal static bool IsCollectionType(TypeUsage type)
    {
      return Helper.IsCollectionType((GlobalItem) type.EdmType);
    }

    internal static bool IsComplexType(TypeUsage type)
    {
      return BuiltInTypeKind.ComplexType == type.EdmType.BuiltInTypeKind;
    }

    internal static bool IsEntityType(TypeUsage type)
    {
      return Helper.IsEntityType(type.EdmType);
    }

    internal static bool IsRelationshipType(TypeUsage type)
    {
      return BuiltInTypeKind.AssociationType == type.EdmType.BuiltInTypeKind;
    }

    internal static bool IsEnumerationType(TypeUsage type)
    {
      return Helper.IsEnumType(type.EdmType);
    }

    internal static bool IsScalarType(TypeUsage type)
    {
      return TypeSemantics.IsScalarType(type.EdmType);
    }

    internal static bool IsScalarType(EdmType type)
    {
      if (!Helper.IsPrimitiveType(type))
        return Helper.IsEnumType(type);
      return true;
    }

    internal static bool IsNumericType(TypeUsage type)
    {
      if (!TypeSemantics.IsIntegerNumericType(type) && !TypeSemantics.IsFixedPointNumericType(type))
        return TypeSemantics.IsFloatPointNumericType(type);
      return true;
    }

    internal static bool IsIntegerNumericType(TypeUsage type)
    {
      PrimitiveTypeKind typeKind;
      if (!TypeHelpers.TryGetPrimitiveTypeKind(type, out typeKind))
        return false;
      switch (typeKind)
      {
        case PrimitiveTypeKind.Byte:
        case PrimitiveTypeKind.SByte:
        case PrimitiveTypeKind.Int16:
        case PrimitiveTypeKind.Int32:
        case PrimitiveTypeKind.Int64:
          return true;
        default:
          return false;
      }
    }

    internal static bool IsFixedPointNumericType(TypeUsage type)
    {
      PrimitiveTypeKind typeKind;
      if (TypeHelpers.TryGetPrimitiveTypeKind(type, out typeKind))
        return typeKind == PrimitiveTypeKind.Decimal;
      return false;
    }

    internal static bool IsFloatPointNumericType(TypeUsage type)
    {
      PrimitiveTypeKind typeKind;
      if (!TypeHelpers.TryGetPrimitiveTypeKind(type, out typeKind))
        return false;
      if (typeKind != PrimitiveTypeKind.Double)
        return typeKind == PrimitiveTypeKind.Single;
      return true;
    }

    internal static bool IsUnsignedNumericType(TypeUsage type)
    {
      PrimitiveTypeKind typeKind;
      return TypeHelpers.TryGetPrimitiveTypeKind(type, out typeKind) && typeKind == PrimitiveTypeKind.Byte;
    }

    internal static bool IsPolymorphicType(TypeUsage type)
    {
      if (!TypeSemantics.IsEntityType(type))
        return TypeSemantics.IsComplexType(type);
      return true;
    }

    internal static bool IsBooleanType(TypeUsage type)
    {
      return TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Boolean);
    }

    internal static bool IsPrimitiveType(TypeUsage type)
    {
      return Helper.IsPrimitiveType(type.EdmType);
    }

    internal static bool IsPrimitiveType(TypeUsage type, PrimitiveTypeKind primitiveTypeKind)
    {
      PrimitiveTypeKind typeKind;
      if (TypeHelpers.TryGetPrimitiveTypeKind(type, out typeKind))
        return typeKind == primitiveTypeKind;
      return false;
    }

    internal static bool IsRowType(TypeUsage type)
    {
      return Helper.IsRowType((GlobalItem) type.EdmType);
    }

    internal static bool IsReferenceType(TypeUsage type)
    {
      return Helper.IsRefType((GlobalItem) type.EdmType);
    }

    internal static bool IsSpatialType(TypeUsage type)
    {
      return Helper.IsSpatialType(type);
    }

    internal static bool IsStrongSpatialType(TypeUsage type)
    {
      if (TypeSemantics.IsPrimitiveType(type))
        return Helper.IsStrongSpatialTypeKind(((PrimitiveType) type.EdmType).PrimitiveTypeKind);
      return false;
    }

    internal static bool IsStructuralType(TypeUsage type)
    {
      return Helper.IsStructuralType(type.EdmType);
    }

    internal static bool IsPartOfKey(EdmMember edmMember)
    {
      if (Helper.IsRelationshipEndMember(edmMember))
        return ((EntityTypeBase) edmMember.DeclaringType).KeyMembers.Contains(edmMember);
      if (!Helper.IsEdmProperty(edmMember) || !Helper.IsEntityTypeBase((EdmType) edmMember.DeclaringType))
        return false;
      return ((EntityTypeBase) edmMember.DeclaringType).KeyMembers.Contains(edmMember);
    }

    internal static bool IsNullable(TypeUsage type)
    {
      Facet facet;
      if (type.Facets.TryGetValue("Nullable", false, out facet))
        return (bool) facet.Value;
      return true;
    }

    internal static bool IsNullable(EdmMember edmMember)
    {
      return TypeSemantics.IsNullable(edmMember.TypeUsage);
    }

    internal static bool IsEqualComparable(TypeUsage type)
    {
      return TypeSemantics.IsEqualComparable(type.EdmType);
    }

    internal static bool IsEqualComparableTo(TypeUsage type1, TypeUsage type2)
    {
      if (TypeSemantics.IsEqualComparable(type1) && TypeSemantics.IsEqualComparable(type2))
        return TypeSemantics.HasCommonType(type1, type2);
      return false;
    }

    internal static bool IsOrderComparable(TypeUsage type)
    {
      return TypeSemantics.IsOrderComparable(type.EdmType);
    }

    internal static bool IsOrderComparableTo(TypeUsage type1, TypeUsage type2)
    {
      if (TypeSemantics.IsOrderComparable(type1) && TypeSemantics.IsOrderComparable(type2))
        return TypeSemantics.HasCommonType(type1, type2);
      return false;
    }

    internal static TypeUsage ForgetConstraints(TypeUsage type)
    {
      if (Helper.IsPrimitiveType(type.EdmType))
        return EdmProviderManifest.Instance.ForgetScalarConstraints(type);
      return type;
    }

    [Conditional("DEBUG")]
    internal static void AssertTypeInvariant(string message, Func<bool> assertPredicate)
    {
    }

    private static bool IsPrimitiveTypeSubTypeOf(TypeUsage fromType, TypeUsage toType)
    {
      return TypeSemantics.IsSubTypeOf((PrimitiveType) fromType.EdmType, (PrimitiveType) toType.EdmType);
    }

    private static bool IsSubTypeOf(
      PrimitiveType subPrimitiveType,
      PrimitiveType superPrimitiveType)
    {
      if (object.ReferenceEquals((object) subPrimitiveType, (object) superPrimitiveType) || Helper.AreSameSpatialUnionType(subPrimitiveType, superPrimitiveType))
        return true;
      return -1 != EdmProviderManifest.Instance.GetPromotionTypes(subPrimitiveType).IndexOf(superPrimitiveType);
    }

    private static bool IsPromotableTo(RowType fromRowType, RowType toRowType)
    {
      if (fromRowType.Properties.Count != toRowType.Properties.Count)
        return false;
      for (int index = 0; index < fromRowType.Properties.Count; ++index)
      {
        if (!TypeSemantics.IsPromotableTo(fromRowType.Properties[index].TypeUsage, toRowType.Properties[index].TypeUsage))
          return false;
      }
      return true;
    }

    private static bool IsPrimitiveTypePromotableTo(TypeUsage fromType, TypeUsage toType)
    {
      return TypeSemantics.IsSubTypeOf((PrimitiveType) fromType.EdmType, (PrimitiveType) toType.EdmType);
    }

    private static bool TryGetCommonType(
      EdmType edmType1,
      EdmType edmType2,
      out EdmType commonEdmType)
    {
      if (edmType2 == edmType1)
      {
        commonEdmType = edmType1;
        return true;
      }
      if (Helper.IsPrimitiveType(edmType1) && Helper.IsPrimitiveType(edmType2))
        return TypeSemantics.TryGetCommonType((PrimitiveType) edmType1, (PrimitiveType) edmType2, out commonEdmType);
      if (Helper.IsCollectionType((GlobalItem) edmType1) && Helper.IsCollectionType((GlobalItem) edmType2))
        return TypeSemantics.TryGetCommonType((CollectionType) edmType1, (CollectionType) edmType2, out commonEdmType);
      if (Helper.IsEntityTypeBase(edmType1) && Helper.IsEntityTypeBase(edmType2))
        return TypeSemantics.TryGetCommonBaseType(edmType1, edmType2, out commonEdmType);
      if (Helper.IsRefType((GlobalItem) edmType1) && Helper.IsRefType((GlobalItem) edmType2))
        return TypeSemantics.TryGetCommonType((RefType) edmType1, (RefType) edmType2, out commonEdmType);
      if (Helper.IsRowType((GlobalItem) edmType1) && Helper.IsRowType((GlobalItem) edmType2))
        return TypeSemantics.TryGetCommonType((RowType) edmType1, (RowType) edmType2, out commonEdmType);
      commonEdmType = (EdmType) null;
      return false;
    }

    private static bool TryGetCommonPrimitiveType(
      TypeUsage type1,
      TypeUsage type2,
      out TypeUsage commonType)
    {
      commonType = (TypeUsage) null;
      if (TypeSemantics.IsPromotableTo(type1, type2))
      {
        commonType = TypeSemantics.ForgetConstraints(type2);
        return true;
      }
      if (TypeSemantics.IsPromotableTo(type2, type1))
      {
        commonType = TypeSemantics.ForgetConstraints(type1);
        return true;
      }
      ReadOnlyCollection<PrimitiveType> commonSuperTypes = TypeSemantics.GetPrimitiveCommonSuperTypes((PrimitiveType) type1.EdmType, (PrimitiveType) type2.EdmType);
      if (commonSuperTypes.Count == 0)
        return false;
      commonType = TypeUsage.CreateDefaultTypeUsage((EdmType) commonSuperTypes[0]);
      return null != commonType;
    }

    private static bool TryGetCommonType(
      PrimitiveType primitiveType1,
      PrimitiveType primitiveType2,
      out EdmType commonType)
    {
      commonType = (EdmType) null;
      if (TypeSemantics.IsSubTypeOf(primitiveType1, primitiveType2))
      {
        commonType = (EdmType) primitiveType2;
        return true;
      }
      if (TypeSemantics.IsSubTypeOf(primitiveType2, primitiveType1))
      {
        commonType = (EdmType) primitiveType1;
        return true;
      }
      ReadOnlyCollection<PrimitiveType> commonSuperTypes = TypeSemantics.GetPrimitiveCommonSuperTypes(primitiveType1, primitiveType2);
      if (commonSuperTypes.Count <= 0)
        return false;
      commonType = (EdmType) commonSuperTypes[0];
      return true;
    }

    private static bool TryGetCommonType(
      CollectionType collectionType1,
      CollectionType collectionType2,
      out EdmType commonType)
    {
      TypeUsage commonType1 = (TypeUsage) null;
      if (!TypeSemantics.TryGetCommonType(collectionType1.TypeUsage, collectionType2.TypeUsage, out commonType1))
      {
        commonType = (EdmType) null;
        return false;
      }
      commonType = (EdmType) new CollectionType(commonType1);
      return true;
    }

    private static bool TryGetCommonType(
      RefType refType1,
      RefType reftype2,
      out EdmType commonType)
    {
      if (!TypeSemantics.TryGetCommonType((EdmType) refType1.ElementType, (EdmType) reftype2.ElementType, out commonType))
        return false;
      commonType = (EdmType) new RefType((EntityType) commonType);
      return true;
    }

    private static bool TryGetCommonType(
      RowType rowType1,
      RowType rowType2,
      out EdmType commonRowType)
    {
      if (rowType1.Properties.Count != rowType2.Properties.Count || rowType1.InitializerMetadata != rowType2.InitializerMetadata)
      {
        commonRowType = (EdmType) null;
        return false;
      }
      List<EdmProperty> edmPropertyList = new List<EdmProperty>();
      for (int index = 0; index < rowType1.Properties.Count; ++index)
      {
        TypeUsage commonType;
        if (!TypeSemantics.TryGetCommonType(rowType1.Properties[index].TypeUsage, rowType2.Properties[index].TypeUsage, out commonType))
        {
          commonRowType = (EdmType) null;
          return false;
        }
        edmPropertyList.Add(new EdmProperty(rowType1.Properties[index].Name, commonType));
      }
      commonRowType = (EdmType) new RowType((IEnumerable<EdmProperty>) edmPropertyList, rowType1.InitializerMetadata);
      return true;
    }

    internal static bool TryGetCommonBaseType(
      EdmType type1,
      EdmType type2,
      out EdmType commonBaseType)
    {
      Dictionary<EdmType, byte> dictionary = new Dictionary<EdmType, byte>();
      for (EdmType key = type2; key != null; key = key.BaseType)
        dictionary.Add(key, (byte) 0);
      for (EdmType key = type1; key != null; key = key.BaseType)
      {
        if (dictionary.ContainsKey(key))
        {
          commonBaseType = key;
          return true;
        }
      }
      commonBaseType = (EdmType) null;
      return false;
    }

    private static bool HasCommonType(TypeUsage type1, TypeUsage type2)
    {
      return null != TypeHelpers.GetCommonTypeUsage(type1, type2);
    }

    private static bool IsEqualComparable(EdmType edmType)
    {
      if (Helper.IsPrimitiveType(edmType) || Helper.IsRefType((GlobalItem) edmType) || (Helper.IsEntityType(edmType) || Helper.IsEnumType(edmType)))
        return true;
      if (!Helper.IsRowType((GlobalItem) edmType))
        return false;
      foreach (EdmMember property in ((RowType) edmType).Properties)
      {
        if (!TypeSemantics.IsEqualComparable(property.TypeUsage))
          return false;
      }
      return true;
    }

    private static bool IsOrderComparable(EdmType edmType)
    {
      return Helper.IsScalarType(edmType);
    }

    private static bool CompareTypes(TypeUsage fromType, TypeUsage toType, bool equivalenceOnly)
    {
      if (object.ReferenceEquals((object) fromType, (object) toType))
        return true;
      if (fromType.EdmType.BuiltInTypeKind != toType.EdmType.BuiltInTypeKind)
        return false;
      if (fromType.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
        return TypeSemantics.CompareTypes(((CollectionType) fromType.EdmType).TypeUsage, ((CollectionType) toType.EdmType).TypeUsage, equivalenceOnly);
      if (fromType.EdmType.BuiltInTypeKind == BuiltInTypeKind.RefType)
        return ((RefType) fromType.EdmType).ElementType.EdmEquals((MetadataItem) ((RefType) toType.EdmType).ElementType);
      if (fromType.EdmType.BuiltInTypeKind != BuiltInTypeKind.RowType)
        return fromType.EdmType.EdmEquals((MetadataItem) toType.EdmType);
      RowType edmType1 = (RowType) fromType.EdmType;
      RowType edmType2 = (RowType) toType.EdmType;
      if (edmType1.Properties.Count != edmType2.Properties.Count)
        return false;
      for (int index = 0; index < edmType1.Properties.Count; ++index)
      {
        EdmProperty property1 = edmType1.Properties[index];
        EdmProperty property2 = edmType2.Properties[index];
        if (!equivalenceOnly && property1.Name != property2.Name || !TypeSemantics.CompareTypes(property1.TypeUsage, property2.TypeUsage, equivalenceOnly))
          return false;
      }
      return true;
    }

    [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
    private static void ComputeCommonTypeClosure()
    {
      if (TypeSemantics._commonTypeClosure != null)
        return;
      ReadOnlyCollection<PrimitiveType>[,] readOnlyCollectionArray = new ReadOnlyCollection<PrimitiveType>[31, 31];
      for (int index = 0; index < 31; ++index)
        readOnlyCollectionArray[index, index] = Helper.EmptyPrimitiveTypeReadOnlyCollection;
      ReadOnlyCollection<PrimitiveType> storeTypes = EdmProviderManifest.Instance.GetStoreTypes();
      for (int index1 = 0; index1 < 31; ++index1)
      {
        for (int index2 = 0; index2 < index1; ++index2)
        {
          readOnlyCollectionArray[index1, index2] = TypeSemantics.Intersect((IList<PrimitiveType>) EdmProviderManifest.Instance.GetPromotionTypes(storeTypes[index1]), (IList<PrimitiveType>) EdmProviderManifest.Instance.GetPromotionTypes(storeTypes[index2]));
          readOnlyCollectionArray[index2, index1] = readOnlyCollectionArray[index1, index2];
        }
      }
      Interlocked.CompareExchange<ReadOnlyCollection<PrimitiveType>[,]>(ref TypeSemantics._commonTypeClosure, readOnlyCollectionArray, (ReadOnlyCollection<PrimitiveType>[,]) null);
    }

    private static ReadOnlyCollection<PrimitiveType> Intersect(
      IList<PrimitiveType> types1,
      IList<PrimitiveType> types2)
    {
      List<PrimitiveType> primitiveTypeList = new List<PrimitiveType>();
      for (int index = 0; index < types1.Count; ++index)
      {
        if (types2.Contains(types1[index]))
          primitiveTypeList.Add(types1[index]);
      }
      if (primitiveTypeList.Count == 0)
        return Helper.EmptyPrimitiveTypeReadOnlyCollection;
      return new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) primitiveTypeList);
    }

    private static ReadOnlyCollection<PrimitiveType> GetPrimitiveCommonSuperTypes(
      PrimitiveType primitiveType1,
      PrimitiveType primitiveType2)
    {
      TypeSemantics.ComputeCommonTypeClosure();
      return TypeSemantics._commonTypeClosure[(int) primitiveType1.PrimitiveTypeKind, (int) primitiveType2.PrimitiveTypeKind];
    }
  }
}
