// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.TypeUsageExtensions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.SqlServer.Utilities
{
  internal static class TypeUsageExtensions
  {
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

    internal static bool IsFixedLength(this TypeUsage type)
    {
      Facet facet = type.Facets.SingleOrDefault<Facet>((Func<Facet, bool>) (f => f.Name == "FixedLength"));
      if (facet != null && facet.Value != null)
        return (bool) facet.Value;
      return false;
    }

    internal static bool TryGetPrecision(this TypeUsage type, out byte precision)
    {
      if (type.IsPrimitiveType(PrimitiveTypeKind.Decimal))
        return type.TryGetFacetValue<byte>("Precision", out precision);
      precision = (byte) 0;
      return false;
    }

    internal static bool TryGetScale(this TypeUsage type, out byte scale)
    {
      if (type.IsPrimitiveType(PrimitiveTypeKind.Decimal))
        return type.TryGetFacetValue<byte>("Scale", out scale);
      scale = (byte) 0;
      return false;
    }

    internal static bool TryGetFacetValue<T>(this TypeUsage type, string facetName, out T value)
    {
      value = default (T);
      Facet facet;
      if (!type.Facets.TryGetValue(facetName, false, out facet) || !(facet.Value is T))
        return false;
      value = (T) facet.Value;
      return true;
    }

    internal static bool IsPrimitiveType(this TypeUsage type, PrimitiveTypeKind primitiveTypeKind)
    {
      if (type.IsPrimitiveType())
        return ((PrimitiveType) type.EdmType).PrimitiveTypeKind == primitiveTypeKind;
      return false;
    }

    internal static bool IsPrimitiveType(this TypeUsage type)
    {
      if (type != null)
        return type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType;
      return false;
    }

    internal static bool IsNullable(this TypeUsage type)
    {
      Facet facet = type.Facets.SingleOrDefault<Facet>((Func<Facet, bool>) (f => f.Name == "Nullable"));
      if (facet != null && facet.Value != null)
        return (bool) facet.Value;
      return false;
    }

    internal static PrimitiveTypeKind GetPrimitiveTypeKind(this TypeUsage type)
    {
      return ((PrimitiveType) type.EdmType).PrimitiveTypeKind;
    }

    internal static bool TryGetIsUnicode(this TypeUsage type, out bool isUnicode)
    {
      if (type.IsPrimitiveType(PrimitiveTypeKind.String))
        return type.TryGetFacetValue<bool>("Unicode", out isUnicode);
      isUnicode = false;
      return false;
    }

    internal static bool TryGetMaxLength(this TypeUsage type, out int maxLength)
    {
      if (type.IsPrimitiveType(PrimitiveTypeKind.String) || type.IsPrimitiveType(PrimitiveTypeKind.Binary))
        return type.TryGetFacetValue<int>("MaxLength", out maxLength);
      maxLength = 0;
      return false;
    }

    internal static IEnumerable<EdmProperty> GetProperties(this TypeUsage type)
    {
      EdmType edmType = type.EdmType;
      switch (edmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.ComplexType:
          return (IEnumerable<EdmProperty>) ((ComplexType) edmType).Properties;
        case BuiltInTypeKind.EntityType:
          return (IEnumerable<EdmProperty>) ((EntityType) edmType).Properties;
        case BuiltInTypeKind.RowType:
          return (IEnumerable<EdmProperty>) ((RowType) edmType).Properties;
        default:
          return Enumerable.Empty<EdmProperty>();
      }
    }

    internal static TypeUsage GetElementTypeUsage(this TypeUsage type)
    {
      EdmType edmType = type.EdmType;
      if (BuiltInTypeKind.CollectionType == edmType.BuiltInTypeKind)
        return ((CollectionType) edmType).TypeUsage;
      if (BuiltInTypeKind.RefType == edmType.BuiltInTypeKind)
        return TypeUsage.CreateDefaultTypeUsage((EdmType) ((RefType) edmType).ElementType);
      return (TypeUsage) null;
    }

    internal static bool MustFacetBeConstant(this TypeUsage type, string facetName)
    {
      return ((PrimitiveType) type.EdmType).FacetDescriptions.Single<FacetDescription>((Func<FacetDescription, bool>) (f => f.FacetName == facetName)).IsConstant;
    }

    internal static bool IsSpatialType(this TypeUsage type)
    {
      if (type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
        return ((PrimitiveType) type.EdmType).IsSpatialType();
      return false;
    }

    internal static bool IsSpatialType(this TypeUsage type, out PrimitiveTypeKind spatialType)
    {
      if (type.IsSpatialType())
      {
        spatialType = ((PrimitiveType) type.EdmType).PrimitiveTypeKind;
        return true;
      }
      spatialType = PrimitiveTypeKind.Binary;
      return false;
    }

    internal static TypeUsage ForceNonUnicode(this TypeUsage typeUsage)
    {
      TypeUsage stringTypeUsage = TypeUsage.CreateStringTypeUsage((PrimitiveType) typeUsage.EdmType, false, false);
      return TypeUsage.Create(typeUsage.EdmType, typeUsage.Facets.Where<Facet>((Func<Facet, bool>) (f => f.Name != "Unicode")).Union<Facet>(stringTypeUsage.Facets.Where<Facet>((Func<Facet, bool>) (f => f.Name == "Unicode"))));
    }
  }
}
