// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.Helper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal static class Helper
  {
    internal static readonly EdmMember[] EmptyArrayEdmProperty = new EdmMember[0];
    private static readonly Dictionary<PrimitiveTypeKind, long[]> _enumUnderlyingTypeRanges = new Dictionary<PrimitiveTypeKind, long[]>()
    {
      {
        PrimitiveTypeKind.Byte,
        new long[2]{ 0L, (long) byte.MaxValue }
      },
      {
        PrimitiveTypeKind.SByte,
        new long[2]{ (long) sbyte.MinValue, (long) sbyte.MaxValue }
      },
      {
        PrimitiveTypeKind.Int16,
        new long[2]{ (long) short.MinValue, (long) short.MaxValue }
      },
      {
        PrimitiveTypeKind.Int32,
        new long[2]{ (long) int.MinValue, (long) int.MaxValue }
      },
      {
        PrimitiveTypeKind.Int64,
        new long[2]{ long.MinValue, long.MaxValue }
      }
    };
    internal static readonly ReadOnlyCollection<KeyValuePair<string, object>> EmptyKeyValueStringObjectList = new ReadOnlyCollection<KeyValuePair<string, object>>((IList<KeyValuePair<string, object>>) new KeyValuePair<string, object>[0]);
    internal static readonly ReadOnlyCollection<string> EmptyStringList = new ReadOnlyCollection<string>((IList<string>) new string[0]);
    internal static readonly ReadOnlyCollection<FacetDescription> EmptyFacetDescriptionEnumerable = new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) new FacetDescription[0]);
    internal static readonly ReadOnlyCollection<EdmFunction> EmptyEdmFunctionReadOnlyCollection = new ReadOnlyCollection<EdmFunction>((IList<EdmFunction>) new EdmFunction[0]);
    internal static readonly ReadOnlyCollection<PrimitiveType> EmptyPrimitiveTypeReadOnlyCollection = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) new PrimitiveType[0]);
    internal static readonly KeyValuePair<string, object>[] EmptyKeyValueStringObjectArray = new KeyValuePair<string, object>[0];
    internal const char PeriodSymbol = '.';
    internal const char CommaSymbol = ',';

    internal static string GetAttributeValue(XPathNavigator nav, string attributeName)
    {
      nav = nav.Clone();
      string str = (string) null;
      if (nav.MoveToAttribute(attributeName, string.Empty))
        str = nav.Value;
      return str;
    }

    internal static object GetTypedAttributeValue(
      XPathNavigator nav,
      string attributeName,
      Type clrType)
    {
      nav = nav.Clone();
      object obj = (object) null;
      if (nav.MoveToAttribute(attributeName, string.Empty))
        obj = nav.ValueAs(clrType);
      return obj;
    }

    internal static FacetDescription GetFacet(
      IEnumerable<FacetDescription> facetCollection,
      string facetName)
    {
      foreach (FacetDescription facet in facetCollection)
      {
        if (facet.FacetName == facetName)
          return facet;
      }
      return (FacetDescription) null;
    }

    internal static bool IsAssignableFrom(EdmType firstType, EdmType secondType)
    {
      if (secondType == null)
        return false;
      if (!firstType.Equals((object) secondType))
        return Helper.IsSubtypeOf(secondType, firstType);
      return true;
    }

    internal static bool IsSubtypeOf(EdmType firstType, EdmType secondType)
    {
      if (secondType == null)
        return false;
      for (EdmType baseType = firstType.BaseType; baseType != null; baseType = baseType.BaseType)
      {
        if (baseType == secondType)
          return true;
      }
      return false;
    }

    internal static IList GetAllStructuralMembers(EdmType edmType)
    {
      switch (edmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.AssociationType:
          return (IList) ((AssociationType) edmType).AssociationEndMembers;
        case BuiltInTypeKind.ComplexType:
          return (IList) ((ComplexType) edmType).Properties;
        case BuiltInTypeKind.EntityType:
          return (IList) ((EntityType) edmType).Properties;
        case BuiltInTypeKind.RowType:
          return (IList) ((RowType) edmType).Properties;
        default:
          return (IList) Helper.EmptyArrayEdmProperty;
      }
    }

    internal static AssociationEndMember GetEndThatShouldBeMappedToKey(
      AssociationType associationType)
    {
      if (associationType.AssociationEndMembers.Any<AssociationEndMember>((Func<AssociationEndMember, bool>) (it => it.RelationshipMultiplicity.Equals((object) RelationshipMultiplicity.One))))
        return associationType.AssociationEndMembers.SingleOrDefault<AssociationEndMember>((Func<AssociationEndMember, bool>) (it =>
        {
          if (!it.RelationshipMultiplicity.Equals((object) RelationshipMultiplicity.Many))
            return it.RelationshipMultiplicity.Equals((object) RelationshipMultiplicity.ZeroOrOne);
          return true;
        }));
      if (associationType.AssociationEndMembers.Any<AssociationEndMember>((Func<AssociationEndMember, bool>) (it => it.RelationshipMultiplicity.Equals((object) RelationshipMultiplicity.ZeroOrOne))))
        return associationType.AssociationEndMembers.SingleOrDefault<AssociationEndMember>((Func<AssociationEndMember, bool>) (it => it.RelationshipMultiplicity.Equals((object) RelationshipMultiplicity.Many)));
      return (AssociationEndMember) null;
    }

    internal static string GetCommaDelimitedString(IEnumerable<string> stringList)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (string str in stringList)
      {
        if (!flag)
          stringBuilder.Append(", ");
        else
          flag = false;
        stringBuilder.Append(str);
      }
      return stringBuilder.ToString();
    }

    internal static IEnumerable<T> Concat<T>(params IEnumerable<T>[] sources)
    {
      foreach (IEnumerable<T> source in sources)
      {
        if (source != null)
        {
          IEnumerator<T> enumerator = source.GetEnumerator();
          while (enumerator.MoveNext())
          {
            T element = enumerator.Current;
            yield return element;
          }
        }
      }
    }

    internal static void DisposeXmlReaders(IEnumerable<XmlReader> xmlReaders)
    {
      foreach (IDisposable xmlReader in xmlReaders)
        xmlReader.Dispose();
    }

    internal static bool IsStructuralType(EdmType type)
    {
      if (!Helper.IsComplexType(type) && !Helper.IsEntityType(type) && !Helper.IsRelationshipType(type))
        return Helper.IsRowType((GlobalItem) type);
      return true;
    }

    internal static bool IsCollectionType(GlobalItem item)
    {
      return BuiltInTypeKind.CollectionType == item.BuiltInTypeKind;
    }

    internal static bool IsEntityType(EdmType type)
    {
      return BuiltInTypeKind.EntityType == type.BuiltInTypeKind;
    }

    internal static bool IsComplexType(EdmType type)
    {
      return BuiltInTypeKind.ComplexType == type.BuiltInTypeKind;
    }

    internal static bool IsPrimitiveType(EdmType type)
    {
      return BuiltInTypeKind.PrimitiveType == type.BuiltInTypeKind;
    }

    internal static bool IsRefType(GlobalItem item)
    {
      return BuiltInTypeKind.RefType == item.BuiltInTypeKind;
    }

    internal static bool IsRowType(GlobalItem item)
    {
      return BuiltInTypeKind.RowType == item.BuiltInTypeKind;
    }

    internal static bool IsAssociationType(EdmType type)
    {
      return BuiltInTypeKind.AssociationType == type.BuiltInTypeKind;
    }

    internal static bool IsRelationshipType(EdmType type)
    {
      return BuiltInTypeKind.AssociationType == type.BuiltInTypeKind;
    }

    internal static bool IsEdmProperty(EdmMember member)
    {
      return BuiltInTypeKind.EdmProperty == member.BuiltInTypeKind;
    }

    internal static bool IsRelationshipEndMember(EdmMember member)
    {
      return BuiltInTypeKind.AssociationEndMember == member.BuiltInTypeKind;
    }

    internal static bool IsAssociationEndMember(EdmMember member)
    {
      return BuiltInTypeKind.AssociationEndMember == member.BuiltInTypeKind;
    }

    internal static bool IsNavigationProperty(EdmMember member)
    {
      return BuiltInTypeKind.NavigationProperty == member.BuiltInTypeKind;
    }

    internal static bool IsEntityTypeBase(EdmType edmType)
    {
      if (!Helper.IsEntityType(edmType))
        return Helper.IsRelationshipType(edmType);
      return true;
    }

    internal static bool IsTransientType(EdmType edmType)
    {
      if (!Helper.IsCollectionType((GlobalItem) edmType) && !Helper.IsRefType((GlobalItem) edmType))
        return Helper.IsRowType((GlobalItem) edmType);
      return true;
    }

    internal static bool IsAssociationSet(EntitySetBase entitySetBase)
    {
      return BuiltInTypeKind.AssociationSet == entitySetBase.BuiltInTypeKind;
    }

    internal static bool IsEntitySet(EntitySetBase entitySetBase)
    {
      return BuiltInTypeKind.EntitySet == entitySetBase.BuiltInTypeKind;
    }

    internal static bool IsRelationshipSet(EntitySetBase entitySetBase)
    {
      return BuiltInTypeKind.AssociationSet == entitySetBase.BuiltInTypeKind;
    }

    internal static bool IsEntityContainer(GlobalItem item)
    {
      return BuiltInTypeKind.EntityContainer == item.BuiltInTypeKind;
    }

    internal static bool IsEdmFunction(GlobalItem item)
    {
      return BuiltInTypeKind.EdmFunction == item.BuiltInTypeKind;
    }

    internal static string GetFileNameFromUri(Uri uri)
    {
      Check.NotNull<Uri>(uri, nameof (uri));
      if (uri.IsFile)
        return uri.LocalPath;
      if (uri.IsAbsoluteUri)
        return uri.AbsolutePath;
      throw new ArgumentException(Strings.UnacceptableUri((object) uri), nameof (uri));
    }

    internal static bool IsEnumType(EdmType edmType)
    {
      return BuiltInTypeKind.EnumType == edmType.BuiltInTypeKind;
    }

    internal static bool IsUnboundedFacetValue(Facet facet)
    {
      return object.ReferenceEquals(facet.Value, (object) EdmConstants.UnboundedValue);
    }

    internal static bool IsVariableFacetValue(Facet facet)
    {
      return object.ReferenceEquals(facet.Value, (object) EdmConstants.VariableValue);
    }

    internal static bool IsScalarType(EdmType edmType)
    {
      if (!Helper.IsEnumType(edmType))
        return Helper.IsPrimitiveType(edmType);
      return true;
    }

    internal static bool IsSpatialType(PrimitiveType type)
    {
      if (!Helper.IsGeographicType(type))
        return Helper.IsGeometricType(type);
      return true;
    }

    internal static bool IsSpatialType(EdmType type, out bool isGeographic)
    {
      PrimitiveType type1 = type as PrimitiveType;
      if (type1 == null)
      {
        isGeographic = false;
        return false;
      }
      isGeographic = Helper.IsGeographicType(type1);
      if (!isGeographic)
        return Helper.IsGeometricType(type1);
      return true;
    }

    internal static bool IsGeographicType(PrimitiveType type)
    {
      return Helper.IsGeographicTypeKind(type.PrimitiveTypeKind);
    }

    internal static bool AreSameSpatialUnionType(PrimitiveType firstType, PrimitiveType secondType)
    {
      return Helper.IsGeographicTypeKind(firstType.PrimitiveTypeKind) && Helper.IsGeographicTypeKind(secondType.PrimitiveTypeKind) || Helper.IsGeometricTypeKind(firstType.PrimitiveTypeKind) && Helper.IsGeometricTypeKind(secondType.PrimitiveTypeKind);
    }

    internal static bool IsGeographicTypeKind(PrimitiveTypeKind kind)
    {
      if (kind != PrimitiveTypeKind.Geography)
        return Helper.IsStrongGeographicTypeKind(kind);
      return true;
    }

    internal static bool IsGeometricType(PrimitiveType type)
    {
      return Helper.IsGeometricTypeKind(type.PrimitiveTypeKind);
    }

    internal static bool IsGeometricTypeKind(PrimitiveTypeKind kind)
    {
      if (kind != PrimitiveTypeKind.Geometry)
        return Helper.IsStrongGeometricTypeKind(kind);
      return true;
    }

    internal static bool IsStrongSpatialTypeKind(PrimitiveTypeKind kind)
    {
      if (!Helper.IsStrongGeometricTypeKind(kind))
        return Helper.IsStrongGeographicTypeKind(kind);
      return true;
    }

    private static bool IsStrongGeometricTypeKind(PrimitiveTypeKind kind)
    {
      if (kind >= PrimitiveTypeKind.GeometryPoint)
        return kind <= PrimitiveTypeKind.GeometryCollection;
      return false;
    }

    private static bool IsStrongGeographicTypeKind(PrimitiveTypeKind kind)
    {
      if (kind >= PrimitiveTypeKind.GeographyPoint)
        return kind <= PrimitiveTypeKind.GeographyCollection;
      return false;
    }

    internal static bool IsSpatialType(TypeUsage type)
    {
      if (type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
        return Helper.IsSpatialType((PrimitiveType) type.EdmType);
      return false;
    }

    internal static bool IsSpatialType(TypeUsage type, out PrimitiveTypeKind spatialType)
    {
      if (type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
      {
        PrimitiveType edmType = (PrimitiveType) type.EdmType;
        if (Helper.IsGeographicTypeKind(edmType.PrimitiveTypeKind) || Helper.IsGeometricTypeKind(edmType.PrimitiveTypeKind))
        {
          spatialType = edmType.PrimitiveTypeKind;
          return true;
        }
      }
      spatialType = PrimitiveTypeKind.Binary;
      return false;
    }

    internal static string ToString(ParameterDirection value)
    {
      switch (value)
      {
        case ParameterDirection.Input:
          return "Input";
        case ParameterDirection.Output:
          return "Output";
        case ParameterDirection.InputOutput:
          return "InputOutput";
        case ParameterDirection.ReturnValue:
          return "ReturnValue";
        default:
          return value.ToString();
      }
    }

    internal static string ToString(ParameterMode value)
    {
      switch (value)
      {
        case ParameterMode.In:
          return "In";
        case ParameterMode.Out:
          return "Out";
        case ParameterMode.InOut:
          return "InOut";
        case ParameterMode.ReturnValue:
          return "ReturnValue";
        default:
          return value.ToString();
      }
    }

    internal static bool IsSupportedEnumUnderlyingType(PrimitiveTypeKind typeKind)
    {
      if (typeKind != PrimitiveTypeKind.Byte && typeKind != PrimitiveTypeKind.SByte && (typeKind != PrimitiveTypeKind.Int16 && typeKind != PrimitiveTypeKind.Int32))
        return typeKind == PrimitiveTypeKind.Int64;
      return true;
    }

    internal static bool IsEnumMemberValueInRange(PrimitiveTypeKind underlyingTypeKind, long value)
    {
      if (value >= Helper._enumUnderlyingTypeRanges[underlyingTypeKind][0])
        return value <= Helper._enumUnderlyingTypeRanges[underlyingTypeKind][1];
      return false;
    }

    internal static PrimitiveType AsPrimitive(EdmType type)
    {
      if (!Helper.IsEnumType(type))
        return (PrimitiveType) type;
      return Helper.GetUnderlyingEdmTypeForEnumType(type);
    }

    internal static PrimitiveType GetUnderlyingEdmTypeForEnumType(EdmType type)
    {
      return ((EnumType) type).UnderlyingType;
    }

    internal static PrimitiveType GetSpatialNormalizedPrimitiveType(EdmType type)
    {
      PrimitiveType type1 = (PrimitiveType) type;
      if (Helper.IsGeographicType(type1) && type1.PrimitiveTypeKind != PrimitiveTypeKind.Geography)
        return PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Geography);
      if (Helper.IsGeometricType(type1) && type1.PrimitiveTypeKind != PrimitiveTypeKind.Geometry)
        return PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Geometry);
      return type1;
    }

    internal static string CombineErrorMessage(IEnumerable<EdmSchemaError> errors)
    {
      StringBuilder stringBuilder = new StringBuilder(Environment.NewLine);
      int num = 0;
      foreach (EdmSchemaError error in errors)
      {
        if (num++ != 0)
          stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append((object) error);
      }
      return stringBuilder.ToString();
    }

    internal static string CombineErrorMessage(IEnumerable<EdmItemError> errors)
    {
      StringBuilder stringBuilder = new StringBuilder(Environment.NewLine);
      int num = 0;
      foreach (EdmItemError error in errors)
      {
        if (num++ != 0)
          stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(error.Message);
      }
      return stringBuilder.ToString();
    }

    internal static IEnumerable<KeyValuePair<T, S>> PairEnumerations<T, S>(
      IBaseList<T> left,
      IEnumerable<S> right)
    {
      IEnumerator leftEnumerator = left.GetEnumerator();
      IEnumerator<S> rightEnumerator = right.GetEnumerator();
      while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
        yield return new KeyValuePair<T, S>((T) leftEnumerator.Current, rightEnumerator.Current);
    }

    internal static TypeUsage GetModelTypeUsage(TypeUsage typeUsage)
    {
      return typeUsage.ModelTypeUsage;
    }

    internal static TypeUsage GetModelTypeUsage(EdmMember member)
    {
      return Helper.GetModelTypeUsage(member.TypeUsage);
    }

    internal static TypeUsage ValidateAndConvertTypeUsage(
      EdmProperty edmProperty,
      EdmProperty columnProperty)
    {
      return Helper.ValidateAndConvertTypeUsage(edmProperty.TypeUsage, columnProperty.TypeUsage);
    }

    internal static TypeUsage ValidateAndConvertTypeUsage(
      TypeUsage cspaceType,
      TypeUsage sspaceType)
    {
      TypeUsage storeType = sspaceType;
      if (sspaceType.EdmType.DataSpace == DataSpace.SSpace)
        storeType = sspaceType.ModelTypeUsage;
      if (Helper.ValidateScalarTypesAreCompatible(cspaceType, storeType))
        return storeType;
      return (TypeUsage) null;
    }

    private static bool ValidateScalarTypesAreCompatible(TypeUsage cspaceType, TypeUsage storeType)
    {
      if (Helper.IsEnumType(cspaceType.EdmType))
        return TypeSemantics.IsSubTypeOf(TypeUsage.Create((EdmType) Helper.GetUnderlyingEdmTypeForEnumType(cspaceType.EdmType)), storeType);
      return TypeSemantics.IsSubTypeOf(cspaceType, storeType);
    }
  }
}
