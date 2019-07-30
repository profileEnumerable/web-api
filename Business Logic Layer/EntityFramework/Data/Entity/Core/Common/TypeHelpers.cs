// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.TypeHelpers
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Core.Common
{
  internal static class TypeHelpers
  {
    internal static readonly ReadOnlyMetadataCollection<EdmMember> EmptyArrayEdmMember = new ReadOnlyMetadataCollection<EdmMember>(new MetadataCollection<EdmMember>().SetReadOnly());
    internal static readonly FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember> EmptyArrayEdmProperty = new FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember>(TypeHelpers.EmptyArrayEdmMember, (Predicate<EdmMember>) null);

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CSpace")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PrimitiveType")]
    [Conditional("DEBUG")]
    internal static void AssertEdmType(TypeUsage typeUsage)
    {
      EdmType edmType = typeUsage.EdmType;
      if (TypeSemantics.IsCollectionType(typeUsage))
        return;
      if (TypeSemantics.IsStructuralType(typeUsage) && !Helper.IsComplexType(typeUsage.EdmType) && !Helper.IsEntityType(typeUsage.EdmType))
      {
        foreach (EdmMember structuralMember in TypeHelpers.GetDeclaredStructuralMembers(typeUsage))
          ;
      }
      else
      {
        if (!TypeSemantics.IsPrimitiveType(typeUsage))
          return;
        PrimitiveType primitiveType = edmType as PrimitiveType;
        if (primitiveType != null && primitiveType.DataSpace != DataSpace.CSpace)
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PrimitiveType must be CSpace '{0}'", (object) typeUsage));
      }
    }

    [Conditional("DEBUG")]
    internal static void AssertEdmType(DbCommandTree commandTree)
    {
      DbQueryCommandTree queryCommandTree = commandTree as DbQueryCommandTree;
    }

    internal static bool IsValidSortOpKeyType(TypeUsage typeUsage)
    {
      if (!TypeSemantics.IsRowType(typeUsage))
        return TypeSemantics.IsOrderComparable(typeUsage);
      foreach (EdmMember property in ((RowType) typeUsage.EdmType).Properties)
      {
        if (!TypeHelpers.IsValidSortOpKeyType(property.TypeUsage))
          return false;
      }
      return true;
    }

    internal static bool IsValidGroupKeyType(TypeUsage typeUsage)
    {
      return TypeHelpers.IsSetComparableOpType(typeUsage);
    }

    internal static bool IsValidDistinctOpType(TypeUsage typeUsage)
    {
      return TypeHelpers.IsSetComparableOpType(typeUsage);
    }

    internal static bool IsSetComparableOpType(TypeUsage typeUsage)
    {
      if (Helper.IsEntityType(typeUsage.EdmType) || Helper.IsPrimitiveType(typeUsage.EdmType) || (Helper.IsEnumType(typeUsage.EdmType) || Helper.IsRefType((GlobalItem) typeUsage.EdmType)))
        return true;
      if (!TypeSemantics.IsRowType(typeUsage))
        return false;
      foreach (EdmMember property in ((RowType) typeUsage.EdmType).Properties)
      {
        if (!TypeHelpers.IsSetComparableOpType(property.TypeUsage))
          return false;
      }
      return true;
    }

    internal static bool IsValidIsNullOpType(TypeUsage typeUsage)
    {
      if (!TypeSemantics.IsReferenceType(typeUsage) && !TypeSemantics.IsEntityType(typeUsage) && !TypeSemantics.IsScalarType(typeUsage))
        return TypeSemantics.IsRowType(typeUsage);
      return true;
    }

    internal static bool IsValidInOpType(TypeUsage typeUsage)
    {
      if (!TypeSemantics.IsReferenceType(typeUsage) && !TypeSemantics.IsEntityType(typeUsage))
        return TypeSemantics.IsScalarType(typeUsage);
      return true;
    }

    internal static TypeUsage GetCommonTypeUsage(
      TypeUsage typeUsage1,
      TypeUsage typeUsage2)
    {
      return TypeSemantics.GetCommonType(typeUsage1, typeUsage2);
    }

    internal static TypeUsage GetCommonTypeUsage(IEnumerable<TypeUsage> types)
    {
      TypeUsage type1 = (TypeUsage) null;
      foreach (TypeUsage type in types)
      {
        if (type == null)
          return (TypeUsage) null;
        if (type1 == null)
        {
          type1 = type;
        }
        else
        {
          type1 = TypeSemantics.GetCommonType(type1, type);
          if (type1 == null)
            break;
        }
      }
      return type1;
    }

    internal static bool TryGetClosestPromotableType(
      TypeUsage fromType,
      out TypeUsage promotableType)
    {
      promotableType = (TypeUsage) null;
      if (Helper.IsPrimitiveType(fromType.EdmType))
      {
        PrimitiveType edmType = (PrimitiveType) fromType.EdmType;
        IList<PrimitiveType> promotionTypes = (IList<PrimitiveType>) EdmProviderManifest.Instance.GetPromotionTypes(edmType);
        int num = promotionTypes.IndexOf(edmType);
        if (-1 != num && num + 1 < promotionTypes.Count)
          promotableType = TypeUsage.Create((EdmType) promotionTypes[num + 1]);
      }
      return null != promotableType;
    }

    internal static bool TryGetBooleanFacetValue(
      TypeUsage type,
      string facetName,
      out bool boolValue)
    {
      boolValue = false;
      Facet facet;
      if (!type.Facets.TryGetValue(facetName, false, out facet) || facet.Value == null)
        return false;
      boolValue = (bool) facet.Value;
      return true;
    }

    internal static bool TryGetByteFacetValue(TypeUsage type, string facetName, out byte byteValue)
    {
      byteValue = (byte) 0;
      Facet facet;
      if (!type.Facets.TryGetValue(facetName, false, out facet) || facet.Value == null || Helper.IsUnboundedFacetValue(facet))
        return false;
      byteValue = (byte) facet.Value;
      return true;
    }

    internal static bool TryGetIntFacetValue(TypeUsage type, string facetName, out int intValue)
    {
      intValue = 0;
      Facet facet;
      if (!type.Facets.TryGetValue(facetName, false, out facet) || facet.Value == null || (Helper.IsUnboundedFacetValue(facet) || Helper.IsVariableFacetValue(facet)))
        return false;
      intValue = (int) facet.Value;
      return true;
    }

    internal static bool TryGetIsFixedLength(TypeUsage type, out bool isFixedLength)
    {
      if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.String) || TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Binary))
        return TypeHelpers.TryGetBooleanFacetValue(type, "FixedLength", out isFixedLength);
      isFixedLength = false;
      return false;
    }

    internal static bool TryGetIsUnicode(TypeUsage type, out bool isUnicode)
    {
      if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.String))
        return TypeHelpers.TryGetBooleanFacetValue(type, "Unicode", out isUnicode);
      isUnicode = false;
      return false;
    }

    internal static bool IsFacetValueConstant(TypeUsage type, string facetName)
    {
      return Helper.GetFacet((IEnumerable<FacetDescription>) ((PrimitiveType) type.EdmType).FacetDescriptions, facetName).IsConstant;
    }

    internal static bool TryGetMaxLength(TypeUsage type, out int maxLength)
    {
      if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.String) || TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Binary))
        return TypeHelpers.TryGetIntFacetValue(type, "MaxLength", out maxLength);
      maxLength = 0;
      return false;
    }

    internal static bool TryGetPrecision(TypeUsage type, out byte precision)
    {
      if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Decimal))
        return TypeHelpers.TryGetByteFacetValue(type, "Precision", out precision);
      precision = (byte) 0;
      return false;
    }

    internal static bool TryGetScale(TypeUsage type, out byte scale)
    {
      if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Decimal))
        return TypeHelpers.TryGetByteFacetValue(type, "Scale", out scale);
      scale = (byte) 0;
      return false;
    }

    internal static bool TryGetPrimitiveTypeKind(TypeUsage type, out PrimitiveTypeKind typeKind)
    {
      if (type != null && type.EdmType != null && type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
      {
        typeKind = ((PrimitiveType) type.EdmType).PrimitiveTypeKind;
        return true;
      }
      typeKind = PrimitiveTypeKind.Binary;
      return false;
    }

    internal static CollectionType CreateCollectionType(TypeUsage elementType)
    {
      return new CollectionType(elementType);
    }

    internal static TypeUsage CreateCollectionTypeUsage(TypeUsage elementType)
    {
      return TypeUsage.Create((EdmType) new CollectionType(elementType));
    }

    internal static RowType CreateRowType(
      IEnumerable<KeyValuePair<string, TypeUsage>> columns)
    {
      return TypeHelpers.CreateRowType(columns, (InitializerMetadata) null);
    }

    internal static RowType CreateRowType(
      IEnumerable<KeyValuePair<string, TypeUsage>> columns,
      InitializerMetadata initializerMetadata)
    {
      List<EdmProperty> edmPropertyList = new List<EdmProperty>();
      foreach (KeyValuePair<string, TypeUsage> column in columns)
        edmPropertyList.Add(new EdmProperty(column.Key, column.Value));
      return new RowType((IEnumerable<EdmProperty>) edmPropertyList, initializerMetadata);
    }

    internal static TypeUsage CreateRowTypeUsage(
      IEnumerable<KeyValuePair<string, TypeUsage>> columns)
    {
      return TypeUsage.Create((EdmType) TypeHelpers.CreateRowType(columns));
    }

    internal static RefType CreateReferenceType(EntityTypeBase entityType)
    {
      return new RefType((EntityType) entityType);
    }

    internal static TypeUsage CreateReferenceTypeUsage(EntityType entityType)
    {
      return TypeUsage.Create((EdmType) TypeHelpers.CreateReferenceType((EntityTypeBase) entityType));
    }

    internal static RowType CreateKeyRowType(EntityTypeBase entityType)
    {
      IEnumerable<EdmMember> keyMembers = (IEnumerable<EdmMember>) entityType.KeyMembers;
      if (keyMembers == null)
        throw new ArgumentException(Strings.Cqt_Metadata_EntityTypeNullKeyMembersInvalid, nameof (entityType));
      List<KeyValuePair<string, TypeUsage>> keyValuePairList = new List<KeyValuePair<string, TypeUsage>>();
      foreach (EdmProperty edmProperty in keyMembers)
        keyValuePairList.Add(new KeyValuePair<string, TypeUsage>(edmProperty.Name, Helper.GetModelTypeUsage((EdmMember) edmProperty)));
      if (keyValuePairList.Count < 1)
        throw new ArgumentException(Strings.Cqt_Metadata_EntityTypeEmptyKeyMembersInvalid, nameof (entityType));
      return TypeHelpers.CreateRowType((IEnumerable<KeyValuePair<string, TypeUsage>>) keyValuePairList);
    }

    internal static TypeUsage GetPrimitiveTypeUsageForScalar(TypeUsage scalarType)
    {
      if (!TypeSemantics.IsEnumerationType(scalarType))
        return scalarType;
      return TypeHelpers.CreateEnumUnderlyingTypeUsage(scalarType);
    }

    internal static TypeUsage CreateEnumUnderlyingTypeUsage(TypeUsage enumTypeUsage)
    {
      return TypeUsage.Create((EdmType) Helper.GetUnderlyingEdmTypeForEnumType(enumTypeUsage.EdmType), (IEnumerable<Facet>) enumTypeUsage.Facets);
    }

    internal static TypeUsage CreateSpatialUnionTypeUsage(TypeUsage spatialTypeUsage)
    {
      return TypeUsage.Create((EdmType) Helper.GetSpatialNormalizedPrimitiveType(spatialTypeUsage.EdmType), (IEnumerable<Facet>) spatialTypeUsage.Facets);
    }

    internal static IBaseList<EdmMember> GetAllStructuralMembers(TypeUsage type)
    {
      return TypeHelpers.GetAllStructuralMembers(type.EdmType);
    }

    internal static IBaseList<EdmMember> GetAllStructuralMembers(EdmType edmType)
    {
      switch (edmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.AssociationType:
          return (IBaseList<EdmMember>) ((AssociationType) edmType).AssociationEndMembers;
        case BuiltInTypeKind.ComplexType:
          return (IBaseList<EdmMember>) ((ComplexType) edmType).Properties;
        case BuiltInTypeKind.EntityType:
          return (IBaseList<EdmMember>) ((EntityType) edmType).Properties;
        case BuiltInTypeKind.RowType:
          return (IBaseList<EdmMember>) ((RowType) edmType).Properties;
        default:
          return (IBaseList<EdmMember>) TypeHelpers.EmptyArrayEdmProperty;
      }
    }

    internal static IEnumerable GetDeclaredStructuralMembers(TypeUsage type)
    {
      return TypeHelpers.GetDeclaredStructuralMembers(type.EdmType);
    }

    internal static IEnumerable GetDeclaredStructuralMembers(EdmType edmType)
    {
      switch (edmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.AssociationType:
          return (IEnumerable) ((StructuralType) edmType).GetDeclaredOnlyMembers<AssociationEndMember>();
        case BuiltInTypeKind.ComplexType:
          return (IEnumerable) ((StructuralType) edmType).GetDeclaredOnlyMembers<EdmProperty>();
        case BuiltInTypeKind.EntityType:
          return (IEnumerable) ((StructuralType) edmType).GetDeclaredOnlyMembers<EdmProperty>();
        case BuiltInTypeKind.RowType:
          return (IEnumerable) ((StructuralType) edmType).GetDeclaredOnlyMembers<EdmProperty>();
        default:
          return (IEnumerable) TypeHelpers.EmptyArrayEdmProperty;
      }
    }

    internal static ReadOnlyMetadataCollection<EdmProperty> GetProperties(
      TypeUsage typeUsage)
    {
      return TypeHelpers.GetProperties(typeUsage.EdmType);
    }

    internal static ReadOnlyMetadataCollection<EdmProperty> GetProperties(
      EdmType edmType)
    {
      switch (edmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.ComplexType:
          return ((ComplexType) edmType).Properties;
        case BuiltInTypeKind.EntityType:
          return ((EntityType) edmType).Properties;
        case BuiltInTypeKind.RowType:
          return ((RowType) edmType).Properties;
        default:
          return (ReadOnlyMetadataCollection<EdmProperty>) TypeHelpers.EmptyArrayEdmProperty;
      }
    }

    internal static TypeUsage GetElementTypeUsage(TypeUsage type)
    {
      if (TypeSemantics.IsCollectionType(type))
        return ((CollectionType) type.EdmType).TypeUsage;
      if (TypeSemantics.IsReferenceType(type))
        return TypeUsage.Create((EdmType) ((RefType) type.EdmType).ElementType);
      return (TypeUsage) null;
    }

    internal static RowType GetTvfReturnType(EdmFunction tvf)
    {
      if (tvf.ReturnParameter != null && TypeSemantics.IsCollectionType(tvf.ReturnParameter.TypeUsage))
      {
        TypeUsage typeUsage = ((CollectionType) tvf.ReturnParameter.TypeUsage.EdmType).TypeUsage;
        if (TypeSemantics.IsRowType(typeUsage))
          return (RowType) typeUsage.EdmType;
      }
      return (RowType) null;
    }

    internal static bool TryGetCollectionElementType(TypeUsage type, out TypeUsage elementType)
    {
      CollectionType type1;
      if (TypeHelpers.TryGetEdmType<CollectionType>(type, out type1))
      {
        elementType = type1.TypeUsage;
        return elementType != null;
      }
      elementType = (TypeUsage) null;
      return false;
    }

    internal static bool TryGetRefEntityType(TypeUsage type, out EntityType referencedEntityType)
    {
      RefType type1;
      if (TypeHelpers.TryGetEdmType<RefType>(type, out type1) && Helper.IsEntityType((EdmType) type1.ElementType))
      {
        referencedEntityType = (EntityType) type1.ElementType;
        return true;
      }
      referencedEntityType = (EntityType) null;
      return false;
    }

    internal static TEdmType GetEdmType<TEdmType>(TypeUsage typeUsage) where TEdmType : EdmType
    {
      return (TEdmType) typeUsage.EdmType;
    }

    internal static bool TryGetEdmType<TEdmType>(TypeUsage typeUsage, out TEdmType type) where TEdmType : EdmType
    {
      type = typeUsage.EdmType as TEdmType;
      return (object) type != null;
    }

    internal static TypeUsage GetReadOnlyType(TypeUsage type)
    {
      if (!type.IsReadOnly)
        type.SetReadOnly();
      return type;
    }

    internal static string GetFullName(string qualifier, string name)
    {
      if (!string.IsNullOrEmpty(qualifier))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) qualifier, (object) name);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) name);
    }

    internal static DbType ConvertClrTypeToDbType(Type clrType)
    {
      switch (Type.GetTypeCode(clrType))
      {
        case TypeCode.Empty:
          throw new ArgumentException(Strings.ADP_InvalidDataType((object) TypeCode.Empty.ToString()));
        case TypeCode.Object:
          if (clrType == typeof (byte[]))
            return DbType.Binary;
          if (clrType == typeof (char[]))
            return DbType.String;
          if (clrType == typeof (Guid))
            return DbType.Guid;
          if (clrType == typeof (TimeSpan))
            return DbType.Time;
          return clrType == typeof (DateTimeOffset) ? DbType.DateTimeOffset : DbType.Object;
        case TypeCode.DBNull:
          return DbType.Object;
        case TypeCode.Boolean:
          return DbType.Boolean;
        case TypeCode.Char:
          return DbType.String;
        case TypeCode.SByte:
          return DbType.SByte;
        case TypeCode.Byte:
          return DbType.Byte;
        case TypeCode.Int16:
          return DbType.Int16;
        case TypeCode.UInt16:
          return DbType.UInt16;
        case TypeCode.Int32:
          return DbType.Int32;
        case TypeCode.UInt32:
          return DbType.UInt32;
        case TypeCode.Int64:
          return DbType.Int64;
        case TypeCode.UInt64:
          return DbType.UInt64;
        case TypeCode.Single:
          return DbType.Single;
        case TypeCode.Double:
          return DbType.Double;
        case TypeCode.Decimal:
          return DbType.Decimal;
        case TypeCode.DateTime:
          return DbType.DateTime;
        case TypeCode.String:
          return DbType.String;
        default:
          throw new ArgumentException(Strings.ADP_UnknownDataTypeCode((object) ((int) Type.GetTypeCode(clrType)).ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) clrType.FullName));
      }
    }

    internal static bool IsIntegerConstant(TypeUsage valueType, object value, long expectedValue)
    {
      if (!TypeSemantics.IsIntegerNumericType(valueType) || value == null)
        return false;
      switch (((PrimitiveType) valueType.EdmType).PrimitiveTypeKind)
      {
        case PrimitiveTypeKind.Byte:
          return expectedValue == (long) (byte) value;
        case PrimitiveTypeKind.SByte:
          return expectedValue == (long) (sbyte) value;
        case PrimitiveTypeKind.Int16:
          return expectedValue == (long) (short) value;
        case PrimitiveTypeKind.Int32:
          return expectedValue == (long) (int) value;
        case PrimitiveTypeKind.Int64:
          return expectedValue == (long) value;
        default:
          return false;
      }
    }

    internal static TypeUsage GetLiteralTypeUsage(PrimitiveTypeKind primitiveTypeKind)
    {
      return TypeHelpers.GetLiteralTypeUsage(primitiveTypeKind, true);
    }

    internal static TypeUsage GetLiteralTypeUsage(
      PrimitiveTypeKind primitiveTypeKind,
      bool isUnicode)
    {
      PrimitiveType primitiveType = EdmProviderManifest.Instance.GetPrimitiveType(primitiveTypeKind);
      TypeUsage typeUsage;
      if (primitiveTypeKind == PrimitiveTypeKind.String)
        typeUsage = TypeUsage.Create((EdmType) primitiveType, new FacetValues()
        {
          Unicode = (FacetValueContainer<bool?>) new bool?(isUnicode),
          MaxLength = (FacetValueContainer<int?>) TypeUsage.DefaultMaxLengthFacetValue,
          FixedLength = (FacetValueContainer<bool?>) new bool?(false),
          Nullable = (FacetValueContainer<bool?>) new bool?(false)
        });
      else
        typeUsage = TypeUsage.Create((EdmType) primitiveType, new FacetValues()
        {
          Nullable = (FacetValueContainer<bool?>) new bool?(false)
        });
      return typeUsage;
    }

    internal static bool IsCanonicalFunction(EdmFunction function)
    {
      return function.DataSpace == DataSpace.CSpace && function.NamespaceName == "Edm";
    }
  }
}
