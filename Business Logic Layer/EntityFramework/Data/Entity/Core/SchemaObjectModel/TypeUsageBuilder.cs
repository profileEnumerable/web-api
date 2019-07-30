// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.TypeUsageBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class TypeUsageBuilder
  {
    private readonly Dictionary<string, object> _facetValues;
    private readonly SchemaElement _element;
    private string _default;
    private object _defaultObject;
    private bool? _nullable;
    private TypeUsage _typeUsage;
    private bool _hasUserDefinedFacets;

    internal TypeUsageBuilder(SchemaElement element)
    {
      this._element = element;
      this._facetValues = new Dictionary<string, object>();
    }

    internal TypeUsage TypeUsage
    {
      get
      {
        return this._typeUsage;
      }
    }

    internal bool Nullable
    {
      get
      {
        if (this._nullable.HasValue)
          return this._nullable.Value;
        return true;
      }
    }

    internal string Default
    {
      get
      {
        return this._default;
      }
    }

    internal object DefaultAsObject
    {
      get
      {
        return this._defaultObject;
      }
    }

    internal bool HasUserDefinedFacets
    {
      get
      {
        return this._hasUserDefinedFacets;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    private bool TryGetFacets(
      EdmType edmType,
      bool complainOnMissingFacet,
      out Dictionary<string, Facet> calculatedFacets)
    {
      bool flag = true;
      Dictionary<string, Facet> dictionary = edmType.GetAssociatedFacetDescriptions().ToDictionary<FacetDescription, string, Facet>((Func<FacetDescription, string>) (f => f.FacetName), (Func<FacetDescription, Facet>) (f => f.DefaultValueFacet));
      calculatedFacets = new Dictionary<string, Facet>();
      foreach (Facet facet in dictionary.Values)
      {
        object obj;
        if (this._facetValues.TryGetValue(facet.Name, out obj))
        {
          if (facet.Description.IsConstant)
          {
            this._element.AddError(ErrorCode.ConstantFacetSpecifiedInSchema, EdmSchemaErrorSeverity.Error, this._element, (object) Strings.ConstantFacetSpecifiedInSchema((object) facet.Name, (object) edmType.Name));
            flag = false;
          }
          else
            calculatedFacets.Add(facet.Name, Facet.Create(facet.Description, obj));
          this._facetValues.Remove(facet.Name);
        }
        else if (complainOnMissingFacet && facet.Description.IsRequired)
        {
          this._element.AddError(ErrorCode.RequiredFacetMissing, EdmSchemaErrorSeverity.Error, (object) Strings.RequiredFacetMissing((object) facet.Name, (object) edmType.Name));
          flag = false;
        }
        else
          calculatedFacets.Add(facet.Name, facet);
      }
      foreach (KeyValuePair<string, object> facetValue in this._facetValues)
      {
        if (facetValue.Key == "StoreGeneratedPattern")
        {
          Facet facet = Facet.Create(Converter.StoreGeneratedPatternFacet, facetValue.Value);
          calculatedFacets.Add(facet.Name, facet);
        }
        else if (facetValue.Key == "ConcurrencyMode")
        {
          Facet facet = Facet.Create(Converter.ConcurrencyModeFacet, facetValue.Value);
          calculatedFacets.Add(facet.Name, facet);
        }
        else if (edmType is PrimitiveType && ((PrimitiveType) edmType).PrimitiveTypeKind == PrimitiveTypeKind.String && facetValue.Key == "Collation")
        {
          Facet facet = Facet.Create(Converter.CollationFacet, facetValue.Value);
          calculatedFacets.Add(facet.Name, facet);
        }
        else
          this._element.AddError(ErrorCode.FacetNotAllowedByType, EdmSchemaErrorSeverity.Error, (object) Strings.FacetNotAllowed((object) facetValue.Key, (object) edmType.Name));
      }
      return flag;
    }

    internal void ValidateAndSetTypeUsage(EdmType edmType, bool complainOnMissingFacet)
    {
      Dictionary<string, Facet> calculatedFacets;
      this.TryGetFacets(edmType, complainOnMissingFacet, out calculatedFacets);
      this._typeUsage = TypeUsage.Create(edmType, (IEnumerable<Facet>) calculatedFacets.Values);
    }

    internal void ValidateAndSetTypeUsage(ScalarType scalar, bool complainOnMissingFacet)
    {
      Trace.Assert(this._element != null);
      Trace.Assert(scalar != null);
      if (Helper.IsSpatialType(scalar.Type) && !this._facetValues.ContainsKey("IsStrict") && !this._element.Schema.UseStrongSpatialTypes)
        this._facetValues.Add("IsStrict", (object) false);
      Dictionary<string, Facet> calculatedFacets;
      if (this.TryGetFacets((EdmType) scalar.Type, complainOnMissingFacet, out calculatedFacets))
      {
        switch (scalar.TypeKind)
        {
          case PrimitiveTypeKind.Binary:
            this.ValidateAndSetBinaryFacets((EdmType) scalar.Type, calculatedFacets);
            break;
          case PrimitiveTypeKind.DateTime:
          case PrimitiveTypeKind.Time:
          case PrimitiveTypeKind.DateTimeOffset:
            this.ValidatePrecisionFacetsForDateTimeFamily((EdmType) scalar.Type, calculatedFacets);
            break;
          case PrimitiveTypeKind.Decimal:
            this.ValidateAndSetDecimalFacets((EdmType) scalar.Type, calculatedFacets);
            break;
          case PrimitiveTypeKind.String:
            this.ValidateAndSetStringFacets((EdmType) scalar.Type, calculatedFacets);
            break;
          case PrimitiveTypeKind.Geometry:
          case PrimitiveTypeKind.Geography:
          case PrimitiveTypeKind.GeometryPoint:
          case PrimitiveTypeKind.GeometryLineString:
          case PrimitiveTypeKind.GeometryPolygon:
          case PrimitiveTypeKind.GeometryMultiPoint:
          case PrimitiveTypeKind.GeometryMultiLineString:
          case PrimitiveTypeKind.GeometryMultiPolygon:
          case PrimitiveTypeKind.GeometryCollection:
          case PrimitiveTypeKind.GeographyPoint:
          case PrimitiveTypeKind.GeographyLineString:
          case PrimitiveTypeKind.GeographyPolygon:
          case PrimitiveTypeKind.GeographyMultiPoint:
          case PrimitiveTypeKind.GeographyMultiLineString:
          case PrimitiveTypeKind.GeographyMultiPolygon:
          case PrimitiveTypeKind.GeographyCollection:
            this.ValidateSpatialFacets((EdmType) scalar.Type, calculatedFacets);
            break;
        }
      }
      this._typeUsage = TypeUsage.Create((EdmType) scalar.Type, (IEnumerable<Facet>) calculatedFacets.Values);
    }

    internal void ValidateEnumFacets(SchemaEnumType schemaEnumType)
    {
      foreach (KeyValuePair<string, object> facetValue in this._facetValues)
      {
        if (facetValue.Key != "Nullable" && facetValue.Key != "StoreGeneratedPattern" && facetValue.Key != "ConcurrencyMode")
          this._element.AddError(ErrorCode.FacetNotAllowedByType, EdmSchemaErrorSeverity.Error, (object) Strings.FacetNotAllowed((object) facetValue.Key, (object) schemaEnumType.FQName));
      }
    }

    internal bool HandleAttribute(XmlReader reader)
    {
      bool flag = this.InternalHandleAttribute(reader);
      this._hasUserDefinedFacets |= flag;
      return flag;
    }

    private bool InternalHandleAttribute(XmlReader reader)
    {
      if (SchemaElement.CanHandleAttribute(reader, "Nullable"))
      {
        this.HandleNullableAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "DefaultValue"))
      {
        this.HandleDefaultAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Precision"))
      {
        this.HandlePrecisionAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Scale"))
      {
        this.HandleScaleAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "StoreGeneratedPattern"))
      {
        this.HandleStoreGeneratedPatternAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "ConcurrencyMode"))
      {
        this.HandleConcurrencyModeAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "MaxLength"))
      {
        this.HandleMaxLengthAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Unicode"))
      {
        this.HandleUnicodeAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Collation"))
      {
        this.HandleCollationAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "FixedLength"))
      {
        this.HandleIsFixedLengthAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Nullable"))
      {
        this.HandleNullableAttribute(reader);
        return true;
      }
      if (!SchemaElement.CanHandleAttribute(reader, "SRID"))
        return false;
      this.HandleSridAttribute(reader);
      return true;
    }

    private void ValidateAndSetBinaryFacets(EdmType type, Dictionary<string, Facet> facets)
    {
      this.ValidateLengthFacets(type, facets);
    }

    private void ValidateAndSetDecimalFacets(EdmType type, Dictionary<string, Facet> facets)
    {
      PrimitiveType primitiveType = (PrimitiveType) type;
      byte? nullable1 = new byte?();
      Facet facet1;
      if (facets.TryGetValue("Precision", out facet1) && facet1.Value != null)
      {
        nullable1 = new byte?((byte) facet1.Value);
        FacetDescription facet2 = Helper.GetFacet((IEnumerable<FacetDescription>) primitiveType.FacetDescriptions, "Precision");
        byte? nullable2 = nullable1;
        int num1 = facet2.MinValue.Value;
        if (((int) nullable2.GetValueOrDefault() >= num1 ? 0 : (nullable2.HasValue ? 1 : 0)) == 0)
        {
          byte? nullable3 = nullable1;
          int num2 = facet2.MaxValue.Value;
          if (((int) nullable3.GetValueOrDefault() <= num2 ? 0 : (nullable3.HasValue ? 1 : 0)) == 0)
            goto label_4;
        }
        this._element.AddError(ErrorCode.PrecisionOutOfRange, EdmSchemaErrorSeverity.Error, (object) Strings.PrecisionOutOfRange((object) nullable1, (object) facet2.MinValue.Value, (object) facet2.MaxValue.Value, (object) primitiveType.Name));
      }
label_4:
      Facet facet3;
      if (!facets.TryGetValue("Scale", out facet3) || facet3.Value == null)
        return;
      byte num3 = (byte) facet3.Value;
      FacetDescription facet4 = Helper.GetFacet((IEnumerable<FacetDescription>) primitiveType.FacetDescriptions, "Scale");
      if ((int) num3 < facet4.MinValue.Value || (int) num3 > facet4.MaxValue.Value)
      {
        this._element.AddError(ErrorCode.ScaleOutOfRange, EdmSchemaErrorSeverity.Error, (object) Strings.ScaleOutOfRange((object) num3, (object) facet4.MinValue.Value, (object) facet4.MaxValue.Value, (object) primitiveType.Name));
      }
      else
      {
        if (!nullable1.HasValue)
          return;
        byte? nullable2 = nullable1;
        int num1 = (int) num3;
        if (((int) nullable2.GetValueOrDefault() >= num1 ? 0 : (nullable2.HasValue ? 1 : 0)) == 0)
          return;
        this._element.AddError(ErrorCode.BadPrecisionAndScale, EdmSchemaErrorSeverity.Error, (object) Strings.BadPrecisionAndScale((object) nullable1, (object) num3));
      }
    }

    private void ValidatePrecisionFacetsForDateTimeFamily(
      EdmType type,
      Dictionary<string, Facet> facets)
    {
      PrimitiveType primitiveType = (PrimitiveType) type;
      byte? nullable1 = new byte?();
      Facet facet1;
      if (!facets.TryGetValue("Precision", out facet1) || facet1.Value == null)
        return;
      nullable1 = new byte?((byte) facet1.Value);
      FacetDescription facet2 = Helper.GetFacet((IEnumerable<FacetDescription>) primitiveType.FacetDescriptions, "Precision");
      byte? nullable2 = nullable1;
      int num1 = facet2.MinValue.Value;
      if (((int) nullable2.GetValueOrDefault() >= num1 ? 0 : (nullable2.HasValue ? 1 : 0)) == 0)
      {
        byte? nullable3 = nullable1;
        int num2 = facet2.MaxValue.Value;
        if (((int) nullable3.GetValueOrDefault() <= num2 ? 0 : (nullable3.HasValue ? 1 : 0)) == 0)
          return;
      }
      this._element.AddError(ErrorCode.PrecisionOutOfRange, EdmSchemaErrorSeverity.Error, (object) Strings.PrecisionOutOfRange((object) nullable1, (object) facet2.MinValue.Value, (object) facet2.MaxValue.Value, (object) primitiveType.Name));
    }

    private void ValidateAndSetStringFacets(EdmType type, Dictionary<string, Facet> facets)
    {
      this.ValidateLengthFacets(type, facets);
    }

    private void ValidateLengthFacets(EdmType type, Dictionary<string, Facet> facets)
    {
      PrimitiveType primitiveType = (PrimitiveType) type;
      Facet facet1;
      if (!facets.TryGetValue("MaxLength", out facet1) || facet1.Value == null || Helper.IsUnboundedFacetValue(facet1))
        return;
      int num1 = (int) facet1.Value;
      FacetDescription facet2 = Helper.GetFacet((IEnumerable<FacetDescription>) primitiveType.FacetDescriptions, "MaxLength");
      int num2 = facet2.MaxValue.Value;
      int num3 = facet2.MinValue.Value;
      if (num1 >= num3 && num1 <= num2)
        return;
      this._element.AddError(ErrorCode.InvalidSize, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidSize((object) num1, (object) num3, (object) num2, (object) primitiveType.Name));
    }

    private void ValidateSpatialFacets(EdmType type, Dictionary<string, Facet> facets)
    {
      PrimitiveType primitiveType = (PrimitiveType) type;
      if (this._facetValues.ContainsKey("ConcurrencyMode"))
        this._element.AddError(ErrorCode.FacetNotAllowedByType, EdmSchemaErrorSeverity.Error, (object) Strings.FacetNotAllowed((object) "ConcurrencyMode", (object) type.FullName));
      Facet facet1;
      if (this._element.Schema.DataModel == SchemaDataModelOption.EntityDataModel && (!facets.TryGetValue("IsStrict", out facet1) || (bool) facet1.Value))
        this._element.AddError(ErrorCode.UnexpectedSpatialType, EdmSchemaErrorSeverity.Error, (object) Strings.SpatialWithUseStrongSpatialTypesFalse);
      Facet facet2;
      if (!facets.TryGetValue("SRID", out facet2) || facet2.Value == null || Helper.IsVariableFacetValue(facet2))
        return;
      int num1 = (int) facet2.Value;
      FacetDescription facet3 = Helper.GetFacet((IEnumerable<FacetDescription>) primitiveType.FacetDescriptions, "SRID");
      int num2 = facet3.MaxValue.Value;
      int num3 = facet3.MinValue.Value;
      if (num1 >= num3 && num1 <= num2)
        return;
      this._element.AddError(ErrorCode.InvalidSystemReferenceId, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidSystemReferenceId((object) num1, (object) num3, (object) num2, (object) primitiveType.Name));
    }

    internal void HandleMaxLengthAttribute(XmlReader reader)
    {
      if (reader.Value.Trim() == "Max")
      {
        this._facetValues.Add("MaxLength", (object) EdmConstants.UnboundedValue);
      }
      else
      {
        int field = 0;
        if (!this._element.HandleIntAttribute(reader, ref field))
          return;
        this._facetValues.Add("MaxLength", (object) field);
      }
    }

    internal void HandleSridAttribute(XmlReader reader)
    {
      if (reader.Value.Trim() == "Variable")
      {
        this._facetValues.Add("SRID", (object) EdmConstants.VariableValue);
      }
      else
      {
        int field = 0;
        if (!this._element.HandleIntAttribute(reader, ref field))
          return;
        this._facetValues.Add("SRID", (object) field);
      }
    }

    private void HandleNullableAttribute(XmlReader reader)
    {
      bool field = false;
      if (!this._element.HandleBoolAttribute(reader, ref field))
        return;
      this._facetValues.Add("Nullable", (object) field);
      this._nullable = new bool?(field);
    }

    internal void HandleStoreGeneratedPatternAttribute(XmlReader reader)
    {
      string str = reader.Value;
      StoreGeneratedPattern generatedPattern;
      if (str == "None")
        generatedPattern = StoreGeneratedPattern.None;
      else if (str == "Identity")
      {
        generatedPattern = StoreGeneratedPattern.Identity;
      }
      else
      {
        if (!(str == "Computed"))
          return;
        generatedPattern = StoreGeneratedPattern.Computed;
      }
      this._facetValues.Add("StoreGeneratedPattern", (object) generatedPattern);
    }

    internal void HandleConcurrencyModeAttribute(XmlReader reader)
    {
      string str = reader.Value;
      ConcurrencyMode concurrencyMode;
      if (str == "None")
      {
        concurrencyMode = ConcurrencyMode.None;
      }
      else
      {
        if (!(str == "Fixed"))
          return;
        concurrencyMode = ConcurrencyMode.Fixed;
      }
      this._facetValues.Add("ConcurrencyMode", (object) concurrencyMode);
    }

    private void HandleDefaultAttribute(XmlReader reader)
    {
      this._default = reader.Value;
    }

    private void HandlePrecisionAttribute(XmlReader reader)
    {
      byte field = 0;
      if (!this._element.HandleByteAttribute(reader, ref field))
        return;
      this._facetValues.Add("Precision", (object) field);
    }

    private void HandleScaleAttribute(XmlReader reader)
    {
      byte field = 0;
      if (!this._element.HandleByteAttribute(reader, ref field))
        return;
      this._facetValues.Add("Scale", (object) field);
    }

    private void HandleUnicodeAttribute(XmlReader reader)
    {
      bool field = false;
      if (!this._element.HandleBoolAttribute(reader, ref field))
        return;
      this._facetValues.Add("Unicode", (object) field);
    }

    private void HandleCollationAttribute(XmlReader reader)
    {
      if (string.IsNullOrEmpty(reader.Value))
        return;
      this._facetValues.Add("Collation", (object) reader.Value);
    }

    private void HandleIsFixedLengthAttribute(XmlReader reader)
    {
      bool field = false;
      if (!this._element.HandleBoolAttribute(reader, ref field))
        return;
      this._facetValues.Add("FixedLength", (object) field);
    }

    internal void ValidateDefaultValue(SchemaType type)
    {
      if (this._default == null)
        return;
      ScalarType scalar = type as ScalarType;
      if (scalar != null)
        this.ValidateScalarMemberDefaultValue(scalar);
      else
        this._element.AddError(ErrorCode.DefaultNotAllowed, EdmSchemaErrorSeverity.Error, (object) Strings.DefaultNotAllowed);
    }

    private void ValidateScalarMemberDefaultValue(ScalarType scalar)
    {
      if (scalar == null)
        return;
      switch (scalar.TypeKind)
      {
        case PrimitiveTypeKind.Binary:
          this.ValidateBinaryDefaultValue(scalar);
          break;
        case PrimitiveTypeKind.Boolean:
          this.ValidateBooleanDefaultValue(scalar);
          break;
        case PrimitiveTypeKind.Byte:
          this.ValidateIntegralDefaultValue(scalar, 0L, (long) byte.MaxValue);
          break;
        case PrimitiveTypeKind.DateTime:
          this.ValidateDateTimeDefaultValue(scalar);
          break;
        case PrimitiveTypeKind.Decimal:
          this.ValidateDecimalDefaultValue(scalar);
          break;
        case PrimitiveTypeKind.Double:
          this.ValidateFloatingPointDefaultValue(scalar, double.MinValue, double.MaxValue);
          break;
        case PrimitiveTypeKind.Guid:
          this.ValidateGuidDefaultValue(scalar);
          break;
        case PrimitiveTypeKind.Single:
          this.ValidateFloatingPointDefaultValue(scalar, -3.40282346638529E+38, 3.40282346638529E+38);
          break;
        case PrimitiveTypeKind.Int16:
          this.ValidateIntegralDefaultValue(scalar, (long) short.MinValue, (long) short.MaxValue);
          break;
        case PrimitiveTypeKind.Int32:
          this.ValidateIntegralDefaultValue(scalar, (long) int.MinValue, (long) int.MaxValue);
          break;
        case PrimitiveTypeKind.Int64:
          this.ValidateIntegralDefaultValue(scalar, long.MinValue, long.MaxValue);
          break;
        case PrimitiveTypeKind.String:
          this._defaultObject = (object) this._default;
          break;
        case PrimitiveTypeKind.Time:
          this.ValidateTimeDefaultValue(scalar);
          break;
        case PrimitiveTypeKind.DateTimeOffset:
          this.ValidateDateTimeOffsetDefaultValue(scalar);
          break;
        default:
          this._element.AddError(ErrorCode.DefaultNotAllowed, EdmSchemaErrorSeverity.Error, (object) Strings.DefaultNotAllowed);
          break;
      }
    }

    private void ValidateBinaryDefaultValue(ScalarType scalar)
    {
      if (scalar.TryParse(this._default, out this._defaultObject))
        return;
      this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDefaultBinaryWithNoMaxLength((object) this._default));
    }

    private void ValidateBooleanDefaultValue(ScalarType scalar)
    {
      if (scalar.TryParse(this._default, out this._defaultObject))
        return;
      this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDefaultBoolean((object) this._default));
    }

    private void ValidateIntegralDefaultValue(ScalarType scalar, long minValue, long maxValue)
    {
      if (scalar.TryParse(this._default, out this._defaultObject))
        return;
      this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDefaultIntegral((object) this._default, (object) minValue, (object) maxValue));
    }

    private void ValidateDateTimeDefaultValue(ScalarType scalar)
    {
      if (scalar.TryParse(this._default, out this._defaultObject))
        return;
      this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDefaultDateTime((object) this._default, (object) "yyyy-MM-dd HH\\:mm\\:ss.fffZ".Replace("\\", "")));
    }

    private void ValidateTimeDefaultValue(ScalarType scalar)
    {
      if (scalar.TryParse(this._default, out this._defaultObject))
        return;
      this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDefaultTime((object) this._default, (object) "HH\\:mm\\:ss.fffffffZ".Replace("\\", "")));
    }

    private void ValidateDateTimeOffsetDefaultValue(ScalarType scalar)
    {
      if (scalar.TryParse(this._default, out this._defaultObject))
        return;
      this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDefaultDateTimeOffset((object) this._default, (object) "yyyy-MM-dd HH\\:mm\\:ss.fffffffz".Replace("\\", "")));
    }

    private void ValidateDecimalDefaultValue(ScalarType scalar)
    {
      if (scalar.TryParse(this._default, out this._defaultObject))
        return;
      this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDefaultDecimal((object) this._default, (object) 38, (object) 38));
    }

    private void ValidateFloatingPointDefaultValue(
      ScalarType scalar,
      double minValue,
      double maxValue)
    {
      if (scalar.TryParse(this._default, out this._defaultObject))
        return;
      this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDefaultFloatingPoint((object) this._default, (object) minValue, (object) maxValue));
    }

    private void ValidateGuidDefaultValue(ScalarType scalar)
    {
      if (scalar.TryParse(this._default, out this._defaultObject))
        return;
      this._element.AddError(ErrorCode.InvalidDefault, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDefaultGuid((object) this._default));
    }
  }
}
