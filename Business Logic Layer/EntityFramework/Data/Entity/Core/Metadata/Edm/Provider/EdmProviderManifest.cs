// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.Provider.EdmProviderManifest
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm.Provider
{
  internal class EdmProviderManifest : DbProviderManifest
  {
    private static readonly EdmProviderManifest _instance = new EdmProviderManifest();
    internal const string ConcurrencyModeFacetName = "ConcurrencyMode";
    internal const string StoreGeneratedPatternFacetName = "StoreGeneratedPattern";
    internal const byte MaximumDecimalPrecision = 255;
    internal const byte MaximumDateTimePrecision = 255;
    private Dictionary<PrimitiveType, ReadOnlyCollection<FacetDescription>> _facetDescriptions;
    private ReadOnlyCollection<PrimitiveType> _primitiveTypes;
    private ReadOnlyCollection<EdmFunction> _functions;
    private ReadOnlyCollection<PrimitiveType>[] _promotionTypes;
    private static TypeUsage[] _canonicalModelTypes;

    private EdmProviderManifest()
    {
    }

    internal static EdmProviderManifest Instance
    {
      get
      {
        return EdmProviderManifest._instance;
      }
    }

    public override string NamespaceName
    {
      get
      {
        return "Edm";
      }
    }

    internal virtual string Token
    {
      get
      {
        return string.Empty;
      }
    }

    public override ReadOnlyCollection<EdmFunction> GetStoreFunctions()
    {
      this.InitializeCanonicalFunctions();
      return this._functions;
    }

    public override ReadOnlyCollection<FacetDescription> GetFacetDescriptions(
      EdmType type)
    {
      this.InitializeFacetDescriptions();
      ReadOnlyCollection<FacetDescription> readOnlyCollection = (ReadOnlyCollection<FacetDescription>) null;
      if (this._facetDescriptions.TryGetValue(type as PrimitiveType, out readOnlyCollection))
        return readOnlyCollection;
      return Helper.EmptyFacetDescriptionEnumerable;
    }

    public PrimitiveType GetPrimitiveType(PrimitiveTypeKind primitiveTypeKind)
    {
      this.InitializePrimitiveTypes();
      return this._primitiveTypes[(int) primitiveTypeKind];
    }

    private void InitializePrimitiveTypes()
    {
      if (this._primitiveTypes != null)
        return;
      PrimitiveType[] primitiveTypeArray = new PrimitiveType[31];
      primitiveTypeArray[0] = new PrimitiveType();
      primitiveTypeArray[1] = new PrimitiveType();
      primitiveTypeArray[2] = new PrimitiveType();
      primitiveTypeArray[3] = new PrimitiveType();
      primitiveTypeArray[4] = new PrimitiveType();
      primitiveTypeArray[5] = new PrimitiveType();
      primitiveTypeArray[7] = new PrimitiveType();
      primitiveTypeArray[6] = new PrimitiveType();
      primitiveTypeArray[9] = new PrimitiveType();
      primitiveTypeArray[10] = new PrimitiveType();
      primitiveTypeArray[11] = new PrimitiveType();
      primitiveTypeArray[8] = new PrimitiveType();
      primitiveTypeArray[12] = new PrimitiveType();
      primitiveTypeArray[13] = new PrimitiveType();
      primitiveTypeArray[14] = new PrimitiveType();
      primitiveTypeArray[15] = new PrimitiveType();
      primitiveTypeArray[17] = new PrimitiveType();
      primitiveTypeArray[18] = new PrimitiveType();
      primitiveTypeArray[19] = new PrimitiveType();
      primitiveTypeArray[20] = new PrimitiveType();
      primitiveTypeArray[21] = new PrimitiveType();
      primitiveTypeArray[22] = new PrimitiveType();
      primitiveTypeArray[23] = new PrimitiveType();
      primitiveTypeArray[16] = new PrimitiveType();
      primitiveTypeArray[24] = new PrimitiveType();
      primitiveTypeArray[25] = new PrimitiveType();
      primitiveTypeArray[26] = new PrimitiveType();
      primitiveTypeArray[27] = new PrimitiveType();
      primitiveTypeArray[28] = new PrimitiveType();
      primitiveTypeArray[29] = new PrimitiveType();
      primitiveTypeArray[30] = new PrimitiveType();
      this.InitializePrimitiveType(primitiveTypeArray[0], PrimitiveTypeKind.Binary, "Binary", typeof (byte[]));
      this.InitializePrimitiveType(primitiveTypeArray[1], PrimitiveTypeKind.Boolean, "Boolean", typeof (bool));
      this.InitializePrimitiveType(primitiveTypeArray[2], PrimitiveTypeKind.Byte, "Byte", typeof (byte));
      this.InitializePrimitiveType(primitiveTypeArray[3], PrimitiveTypeKind.DateTime, "DateTime", typeof (DateTime));
      this.InitializePrimitiveType(primitiveTypeArray[4], PrimitiveTypeKind.Decimal, "Decimal", typeof (Decimal));
      this.InitializePrimitiveType(primitiveTypeArray[5], PrimitiveTypeKind.Double, "Double", typeof (double));
      this.InitializePrimitiveType(primitiveTypeArray[7], PrimitiveTypeKind.Single, "Single", typeof (float));
      this.InitializePrimitiveType(primitiveTypeArray[6], PrimitiveTypeKind.Guid, "Guid", typeof (Guid));
      this.InitializePrimitiveType(primitiveTypeArray[9], PrimitiveTypeKind.Int16, "Int16", typeof (short));
      this.InitializePrimitiveType(primitiveTypeArray[10], PrimitiveTypeKind.Int32, "Int32", typeof (int));
      this.InitializePrimitiveType(primitiveTypeArray[11], PrimitiveTypeKind.Int64, "Int64", typeof (long));
      this.InitializePrimitiveType(primitiveTypeArray[8], PrimitiveTypeKind.SByte, "SByte", typeof (sbyte));
      this.InitializePrimitiveType(primitiveTypeArray[12], PrimitiveTypeKind.String, "String", typeof (string));
      this.InitializePrimitiveType(primitiveTypeArray[13], PrimitiveTypeKind.Time, "Time", typeof (TimeSpan));
      this.InitializePrimitiveType(primitiveTypeArray[14], PrimitiveTypeKind.DateTimeOffset, "DateTimeOffset", typeof (DateTimeOffset));
      this.InitializePrimitiveType(primitiveTypeArray[16], PrimitiveTypeKind.Geography, "Geography", typeof (DbGeography));
      this.InitializePrimitiveType(primitiveTypeArray[24], PrimitiveTypeKind.GeographyPoint, "GeographyPoint", typeof (DbGeography));
      this.InitializePrimitiveType(primitiveTypeArray[25], PrimitiveTypeKind.GeographyLineString, "GeographyLineString", typeof (DbGeography));
      this.InitializePrimitiveType(primitiveTypeArray[26], PrimitiveTypeKind.GeographyPolygon, "GeographyPolygon", typeof (DbGeography));
      this.InitializePrimitiveType(primitiveTypeArray[27], PrimitiveTypeKind.GeographyMultiPoint, "GeographyMultiPoint", typeof (DbGeography));
      this.InitializePrimitiveType(primitiveTypeArray[28], PrimitiveTypeKind.GeographyMultiLineString, "GeographyMultiLineString", typeof (DbGeography));
      this.InitializePrimitiveType(primitiveTypeArray[29], PrimitiveTypeKind.GeographyMultiPolygon, "GeographyMultiPolygon", typeof (DbGeography));
      this.InitializePrimitiveType(primitiveTypeArray[30], PrimitiveTypeKind.GeographyCollection, "GeographyCollection", typeof (DbGeography));
      this.InitializePrimitiveType(primitiveTypeArray[15], PrimitiveTypeKind.Geometry, "Geometry", typeof (DbGeometry));
      this.InitializePrimitiveType(primitiveTypeArray[17], PrimitiveTypeKind.GeometryPoint, "GeometryPoint", typeof (DbGeometry));
      this.InitializePrimitiveType(primitiveTypeArray[18], PrimitiveTypeKind.GeometryLineString, "GeometryLineString", typeof (DbGeometry));
      this.InitializePrimitiveType(primitiveTypeArray[19], PrimitiveTypeKind.GeometryPolygon, "GeometryPolygon", typeof (DbGeometry));
      this.InitializePrimitiveType(primitiveTypeArray[20], PrimitiveTypeKind.GeometryMultiPoint, "GeometryMultiPoint", typeof (DbGeometry));
      this.InitializePrimitiveType(primitiveTypeArray[21], PrimitiveTypeKind.GeometryMultiLineString, "GeometryMultiLineString", typeof (DbGeometry));
      this.InitializePrimitiveType(primitiveTypeArray[22], PrimitiveTypeKind.GeometryMultiPolygon, "GeometryMultiPolygon", typeof (DbGeometry));
      this.InitializePrimitiveType(primitiveTypeArray[23], PrimitiveTypeKind.GeometryCollection, "GeometryCollection", typeof (DbGeometry));
      foreach (PrimitiveType primitiveType in primitiveTypeArray)
      {
        primitiveType.ProviderManifest = (DbProviderManifest) this;
        primitiveType.SetReadOnly();
      }
      Interlocked.CompareExchange<ReadOnlyCollection<PrimitiveType>>(ref this._primitiveTypes, new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) primitiveTypeArray), (ReadOnlyCollection<PrimitiveType>) null);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "clrType")]
    private void InitializePrimitiveType(
      PrimitiveType primitiveType,
      PrimitiveTypeKind primitiveTypeKind,
      string name,
      Type clrType)
    {
      EdmType.Initialize((EdmType) primitiveType, name, "Edm", DataSpace.CSpace, true, (EdmType) null);
      PrimitiveType.Initialize(primitiveType, primitiveTypeKind, (DbProviderManifest) this);
    }

    private void InitializeFacetDescriptions()
    {
      if (this._facetDescriptions != null)
        return;
      this.InitializePrimitiveTypes();
      Dictionary<PrimitiveType, ReadOnlyCollection<FacetDescription>> dictionary = new Dictionary<PrimitiveType, ReadOnlyCollection<FacetDescription>>();
      FacetDescription[] facetDescriptions1 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.String);
      PrimitiveType primitiveType1 = this._primitiveTypes[12];
      dictionary.Add(primitiveType1, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions1));
      FacetDescription[] facetDescriptions2 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.Binary);
      PrimitiveType primitiveType2 = this._primitiveTypes[0];
      dictionary.Add(primitiveType2, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions2));
      FacetDescription[] facetDescriptions3 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.DateTime);
      PrimitiveType primitiveType3 = this._primitiveTypes[3];
      dictionary.Add(primitiveType3, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions3));
      FacetDescription[] facetDescriptions4 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.Time);
      PrimitiveType primitiveType4 = this._primitiveTypes[13];
      dictionary.Add(primitiveType4, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions4));
      FacetDescription[] facetDescriptions5 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.DateTimeOffset);
      PrimitiveType primitiveType5 = this._primitiveTypes[14];
      dictionary.Add(primitiveType5, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions5));
      FacetDescription[] facetDescriptions6 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.Decimal);
      PrimitiveType primitiveType6 = this._primitiveTypes[4];
      dictionary.Add(primitiveType6, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions6));
      FacetDescription[] facetDescriptions7 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.Geography);
      PrimitiveType primitiveType7 = this._primitiveTypes[16];
      dictionary.Add(primitiveType7, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions7));
      FacetDescription[] facetDescriptions8 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeographyPoint);
      PrimitiveType primitiveType8 = this._primitiveTypes[24];
      dictionary.Add(primitiveType8, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions8));
      FacetDescription[] facetDescriptions9 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeographyLineString);
      PrimitiveType primitiveType9 = this._primitiveTypes[25];
      dictionary.Add(primitiveType9, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions9));
      FacetDescription[] facetDescriptions10 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeographyPolygon);
      PrimitiveType primitiveType10 = this._primitiveTypes[26];
      dictionary.Add(primitiveType10, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions10));
      FacetDescription[] facetDescriptions11 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeographyMultiPoint);
      PrimitiveType primitiveType11 = this._primitiveTypes[27];
      dictionary.Add(primitiveType11, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions11));
      FacetDescription[] facetDescriptions12 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeographyMultiLineString);
      PrimitiveType primitiveType12 = this._primitiveTypes[28];
      dictionary.Add(primitiveType12, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions12));
      FacetDescription[] facetDescriptions13 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeographyMultiPolygon);
      PrimitiveType primitiveType13 = this._primitiveTypes[29];
      dictionary.Add(primitiveType13, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions13));
      FacetDescription[] facetDescriptions14 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeographyCollection);
      PrimitiveType primitiveType14 = this._primitiveTypes[30];
      dictionary.Add(primitiveType14, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions14));
      FacetDescription[] facetDescriptions15 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.Geometry);
      PrimitiveType primitiveType15 = this._primitiveTypes[15];
      dictionary.Add(primitiveType15, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions15));
      FacetDescription[] facetDescriptions16 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeometryPoint);
      PrimitiveType primitiveType16 = this._primitiveTypes[17];
      dictionary.Add(primitiveType16, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions16));
      FacetDescription[] facetDescriptions17 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeometryLineString);
      PrimitiveType primitiveType17 = this._primitiveTypes[18];
      dictionary.Add(primitiveType17, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions17));
      FacetDescription[] facetDescriptions18 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeometryPolygon);
      PrimitiveType primitiveType18 = this._primitiveTypes[19];
      dictionary.Add(primitiveType18, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions18));
      FacetDescription[] facetDescriptions19 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeometryMultiPoint);
      PrimitiveType primitiveType19 = this._primitiveTypes[20];
      dictionary.Add(primitiveType19, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions19));
      FacetDescription[] facetDescriptions20 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeometryMultiLineString);
      PrimitiveType primitiveType20 = this._primitiveTypes[21];
      dictionary.Add(primitiveType20, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions20));
      FacetDescription[] facetDescriptions21 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeometryMultiPolygon);
      PrimitiveType primitiveType21 = this._primitiveTypes[22];
      dictionary.Add(primitiveType21, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions21));
      FacetDescription[] facetDescriptions22 = EdmProviderManifest.GetInitialFacetDescriptions(PrimitiveTypeKind.GeometryCollection);
      PrimitiveType primitiveType22 = this._primitiveTypes[23];
      dictionary.Add(primitiveType22, new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptions22));
      Interlocked.CompareExchange<Dictionary<PrimitiveType, ReadOnlyCollection<FacetDescription>>>(ref this._facetDescriptions, dictionary, (Dictionary<PrimitiveType, ReadOnlyCollection<FacetDescription>>) null);
    }

    internal static FacetDescription[] GetInitialFacetDescriptions(
      PrimitiveTypeKind primitiveTypeKind)
    {
      switch (primitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          return new FacetDescription[2]
          {
            new FacetDescription("MaxLength", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Int32), new int?(0), new int?(int.MaxValue), (object) null),
            new FacetDescription("FixedLength", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), new int?(), new int?(), (object) null)
          };
        case PrimitiveTypeKind.DateTime:
          return new FacetDescription[1]
          {
            new FacetDescription("Precision", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), new int?(0), new int?((int) byte.MaxValue), (object) null)
          };
        case PrimitiveTypeKind.Decimal:
          return new FacetDescription[2]
          {
            new FacetDescription("Precision", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), new int?(1), new int?((int) byte.MaxValue), (object) null),
            new FacetDescription("Scale", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), new int?(0), new int?((int) byte.MaxValue), (object) null)
          };
        case PrimitiveTypeKind.String:
          return new FacetDescription[3]
          {
            new FacetDescription("MaxLength", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Int32), new int?(0), new int?(int.MaxValue), (object) null),
            new FacetDescription("Unicode", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), new int?(), new int?(), (object) null),
            new FacetDescription("FixedLength", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), new int?(), new int?(), (object) null)
          };
        case PrimitiveTypeKind.Time:
          return new FacetDescription[1]
          {
            new FacetDescription("Precision", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), new int?(0), new int?((int) byte.MaxValue), (object) TypeUsage.DefaultDateTimePrecisionFacetValue)
          };
        case PrimitiveTypeKind.DateTimeOffset:
          return new FacetDescription[1]
          {
            new FacetDescription("Precision", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), new int?(0), new int?((int) byte.MaxValue), (object) TypeUsage.DefaultDateTimePrecisionFacetValue)
          };
        case PrimitiveTypeKind.Geometry:
        case PrimitiveTypeKind.GeometryPoint:
        case PrimitiveTypeKind.GeometryLineString:
        case PrimitiveTypeKind.GeometryPolygon:
        case PrimitiveTypeKind.GeometryMultiPoint:
        case PrimitiveTypeKind.GeometryMultiLineString:
        case PrimitiveTypeKind.GeometryMultiPolygon:
        case PrimitiveTypeKind.GeometryCollection:
          return new FacetDescription[2]
          {
            new FacetDescription("SRID", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Int32), new int?(0), new int?(int.MaxValue), (object) DbGeometry.DefaultCoordinateSystemId),
            new FacetDescription("IsStrict", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), new int?(), new int?(), (object) true)
          };
        case PrimitiveTypeKind.Geography:
        case PrimitiveTypeKind.GeographyPoint:
        case PrimitiveTypeKind.GeographyLineString:
        case PrimitiveTypeKind.GeographyPolygon:
        case PrimitiveTypeKind.GeographyMultiPoint:
        case PrimitiveTypeKind.GeographyMultiLineString:
        case PrimitiveTypeKind.GeographyMultiPolygon:
        case PrimitiveTypeKind.GeographyCollection:
          return new FacetDescription[2]
          {
            new FacetDescription("SRID", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Int32), new int?(0), new int?(int.MaxValue), (object) DbGeography.DefaultCoordinateSystemId),
            new FacetDescription("IsStrict", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), new int?(), new int?(), (object) true)
          };
        default:
          return (FacetDescription[]) null;
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void InitializeCanonicalFunctions()
    {
      if (this._functions != null)
        return;
      this.InitializePrimitiveTypes();
      EdmProviderManifestFunctionBuilder functions = new EdmProviderManifestFunctionBuilder(this._primitiveTypes);
      PrimitiveTypeKind[] primitiveTypeKindArray1 = new PrimitiveTypeKind[13]
      {
        PrimitiveTypeKind.Byte,
        PrimitiveTypeKind.DateTime,
        PrimitiveTypeKind.Decimal,
        PrimitiveTypeKind.Double,
        PrimitiveTypeKind.Int16,
        PrimitiveTypeKind.Int32,
        PrimitiveTypeKind.Int64,
        PrimitiveTypeKind.SByte,
        PrimitiveTypeKind.Single,
        PrimitiveTypeKind.String,
        PrimitiveTypeKind.Binary,
        PrimitiveTypeKind.Time,
        PrimitiveTypeKind.DateTimeOffset
      };
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray1, (Action<PrimitiveTypeKind>) (type => functions.AddAggregate("Max", type)));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray1, (Action<PrimitiveTypeKind>) (type => functions.AddAggregate("Min", type)));
      PrimitiveTypeKind[] primitiveTypeKindArray2 = new PrimitiveTypeKind[4]
      {
        PrimitiveTypeKind.Decimal,
        PrimitiveTypeKind.Double,
        PrimitiveTypeKind.Int32,
        PrimitiveTypeKind.Int64
      };
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray2, (Action<PrimitiveTypeKind>) (type => functions.AddAggregate("Avg", type)));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray2, (Action<PrimitiveTypeKind>) (type => functions.AddAggregate("Sum", type)));
      PrimitiveTypeKind[] primitiveTypeKindArray3 = new PrimitiveTypeKind[4]
      {
        PrimitiveTypeKind.Decimal,
        PrimitiveTypeKind.Double,
        PrimitiveTypeKind.Int32,
        PrimitiveTypeKind.Int64
      };
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray3, (Action<PrimitiveTypeKind>) (type => functions.AddAggregate(PrimitiveTypeKind.Double, "StDev", type)));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray3, (Action<PrimitiveTypeKind>) (type => functions.AddAggregate(PrimitiveTypeKind.Double, "StDevP", type)));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray3, (Action<PrimitiveTypeKind>) (type => functions.AddAggregate(PrimitiveTypeKind.Double, "Var", type)));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray3, (Action<PrimitiveTypeKind>) (type => functions.AddAggregate(PrimitiveTypeKind.Double, "VarP", type)));
      EdmProviderManifestFunctionBuilder.ForAllBasePrimitiveTypes((Action<PrimitiveTypeKind>) (type => functions.AddAggregate(PrimitiveTypeKind.Int32, "Count", type)));
      EdmProviderManifestFunctionBuilder.ForAllBasePrimitiveTypes((Action<PrimitiveTypeKind>) (type => functions.AddAggregate(PrimitiveTypeKind.Int64, "BigCount", type)));
      functions.AddFunction(PrimitiveTypeKind.String, "Trim", PrimitiveTypeKind.String, "stringArgument");
      functions.AddFunction(PrimitiveTypeKind.String, "RTrim", PrimitiveTypeKind.String, "stringArgument");
      functions.AddFunction(PrimitiveTypeKind.String, "LTrim", PrimitiveTypeKind.String, "stringArgument");
      functions.AddFunction(PrimitiveTypeKind.String, "Concat", PrimitiveTypeKind.String, "string1", PrimitiveTypeKind.String, "string2");
      functions.AddFunction(PrimitiveTypeKind.Int32, "Length", PrimitiveTypeKind.String, "stringArgument");
      PrimitiveTypeKind[] primitiveTypeKindArray4 = new PrimitiveTypeKind[5]
      {
        PrimitiveTypeKind.Byte,
        PrimitiveTypeKind.Int16,
        PrimitiveTypeKind.Int32,
        PrimitiveTypeKind.Int64,
        PrimitiveTypeKind.SByte
      };
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray4, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.String, "Substring", PrimitiveTypeKind.String, "stringArgument", type, "start", type, "length")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray4, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.String, "Left", PrimitiveTypeKind.String, "stringArgument", type, "length")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray4, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.String, "Right", PrimitiveTypeKind.String, "stringArgument", type, "length")));
      functions.AddFunction(PrimitiveTypeKind.String, "Replace", PrimitiveTypeKind.String, "stringArgument", PrimitiveTypeKind.String, "toReplace", PrimitiveTypeKind.String, "replacement");
      functions.AddFunction(PrimitiveTypeKind.Int32, "IndexOf", PrimitiveTypeKind.String, "searchString", PrimitiveTypeKind.String, "stringToFind");
      functions.AddFunction(PrimitiveTypeKind.String, "ToUpper", PrimitiveTypeKind.String, "stringArgument");
      functions.AddFunction(PrimitiveTypeKind.String, "ToLower", PrimitiveTypeKind.String, "stringArgument");
      functions.AddFunction(PrimitiveTypeKind.String, "Reverse", PrimitiveTypeKind.String, "stringArgument");
      functions.AddFunction(PrimitiveTypeKind.Boolean, "Contains", PrimitiveTypeKind.String, "searchedString", PrimitiveTypeKind.String, "searchedForString");
      functions.AddFunction(PrimitiveTypeKind.Boolean, "StartsWith", PrimitiveTypeKind.String, "stringArgument", PrimitiveTypeKind.String, "prefix");
      functions.AddFunction(PrimitiveTypeKind.Boolean, "EndsWith", PrimitiveTypeKind.String, "stringArgument", PrimitiveTypeKind.String, "suffix");
      PrimitiveTypeKind[] primitiveTypeKindArray5 = new PrimitiveTypeKind[2]
      {
        PrimitiveTypeKind.DateTimeOffset,
        PrimitiveTypeKind.DateTime
      };
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "Year", type, "dateValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "Month", type, "dateValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "Day", type, "dateValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DayOfYear", type, "dateValue")));
      PrimitiveTypeKind[] primitiveTypeKindArray6 = new PrimitiveTypeKind[3]
      {
        PrimitiveTypeKind.DateTimeOffset,
        PrimitiveTypeKind.DateTime,
        PrimitiveTypeKind.Time
      };
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "Hour", type, "timeValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "Minute", type, "timeValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "Second", type, "timeValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "Millisecond", type, "timeValue")));
      functions.AddFunction(PrimitiveTypeKind.DateTime, "CurrentDateTime");
      functions.AddFunction(PrimitiveTypeKind.DateTimeOffset, "CurrentDateTimeOffset");
      functions.AddFunction(PrimitiveTypeKind.Int32, "GetTotalOffsetMinutes", PrimitiveTypeKind.DateTimeOffset, "dateTimeOffsetArgument");
      functions.AddFunction(PrimitiveTypeKind.DateTime, "CurrentUtcDateTime");
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "TruncateTime", type, "dateValue")));
      functions.AddFunction(PrimitiveTypeKind.DateTime, "CreateDateTime", PrimitiveTypeKind.Int32, "year", PrimitiveTypeKind.Int32, "month", PrimitiveTypeKind.Int32, "day", PrimitiveTypeKind.Int32, "hour", PrimitiveTypeKind.Int32, "minute", PrimitiveTypeKind.Double, "second");
      functions.AddFunction(PrimitiveTypeKind.DateTimeOffset, "CreateDateTimeOffset", PrimitiveTypeKind.Int32, "year", PrimitiveTypeKind.Int32, "month", PrimitiveTypeKind.Int32, "day", PrimitiveTypeKind.Int32, "hour", PrimitiveTypeKind.Int32, "minute", PrimitiveTypeKind.Double, "second", PrimitiveTypeKind.Int32, "timeZoneOffset");
      functions.AddFunction(PrimitiveTypeKind.Time, "CreateTime", PrimitiveTypeKind.Int32, "hour", PrimitiveTypeKind.Int32, "minute", PrimitiveTypeKind.Double, "second");
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "AddYears", type, "dateValue", PrimitiveTypeKind.Int32, "addValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "AddMonths", type, "dateValue", PrimitiveTypeKind.Int32, "addValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "AddDays", type, "dateValue", PrimitiveTypeKind.Int32, "addValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "AddHours", type, "timeValue", PrimitiveTypeKind.Int32, "addValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "AddMinutes", type, "timeValue", PrimitiveTypeKind.Int32, "addValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "AddSeconds", type, "timeValue", PrimitiveTypeKind.Int32, "addValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "AddMilliseconds", type, "timeValue", PrimitiveTypeKind.Int32, "addValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "AddMicroseconds", type, "timeValue", PrimitiveTypeKind.Int32, "addValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "AddNanoseconds", type, "timeValue", PrimitiveTypeKind.Int32, "addValue")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DiffYears", type, "dateValue1", type, "dateValue2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DiffMonths", type, "dateValue1", type, "dateValue2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray5, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DiffDays", type, "dateValue1", type, "dateValue2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DiffHours", type, "timeValue1", type, "timeValue2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DiffMinutes", type, "timeValue1", type, "timeValue2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DiffSeconds", type, "timeValue1", type, "timeValue2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DiffMilliseconds", type, "timeValue1", type, "timeValue2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DiffMicroseconds", type, "timeValue1", type, "timeValue2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray6, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(PrimitiveTypeKind.Int32, "DiffNanoseconds", type, "timeValue1", type, "timeValue2")));
      PrimitiveTypeKind[] primitiveTypeKindArray7 = new PrimitiveTypeKind[3]
      {
        PrimitiveTypeKind.Single,
        PrimitiveTypeKind.Double,
        PrimitiveTypeKind.Decimal
      };
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray7, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "Round", type, "value")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray7, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "Floor", type, "value")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray7, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "Ceiling", type, "value")));
      PrimitiveTypeKind[] primitiveTypeKindArray8 = new PrimitiveTypeKind[2]
      {
        PrimitiveTypeKind.Double,
        PrimitiveTypeKind.Decimal
      };
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray8, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "Round", type, "value", PrimitiveTypeKind.Int32, "digits")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray8, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "Truncate", type, "value", PrimitiveTypeKind.Int32, "digits")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) new PrimitiveTypeKind[7]
      {
        PrimitiveTypeKind.Decimal,
        PrimitiveTypeKind.Double,
        PrimitiveTypeKind.Int16,
        PrimitiveTypeKind.Int32,
        PrimitiveTypeKind.Int64,
        PrimitiveTypeKind.Byte,
        PrimitiveTypeKind.Single
      }, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "Abs", type, "value")));
      PrimitiveTypeKind[] primitiveTypeKindArray9 = new PrimitiveTypeKind[4]
      {
        PrimitiveTypeKind.Decimal,
        PrimitiveTypeKind.Double,
        PrimitiveTypeKind.Int32,
        PrimitiveTypeKind.Int64
      };
      PrimitiveTypeKind[] primitiveTypeKindArray10 = new PrimitiveTypeKind[3]
      {
        PrimitiveTypeKind.Decimal,
        PrimitiveTypeKind.Double,
        PrimitiveTypeKind.Int64
      };
      foreach (PrimitiveTypeKind primitiveTypeKind in primitiveTypeKindArray9)
      {
        foreach (PrimitiveTypeKind argument2TypeKind in primitiveTypeKindArray10)
          functions.AddFunction(primitiveTypeKind, "Power", primitiveTypeKind, "baseArgument", argument2TypeKind, "exponent");
      }
      PrimitiveTypeKind[] primitiveTypeKindArray11 = new PrimitiveTypeKind[4]
      {
        PrimitiveTypeKind.Int16,
        PrimitiveTypeKind.Int32,
        PrimitiveTypeKind.Int64,
        PrimitiveTypeKind.Byte
      };
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray11, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "BitwiseAnd", type, "value1", type, "value2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray11, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "BitwiseOr", type, "value1", type, "value2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray11, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "BitwiseXor", type, "value1", type, "value2")));
      EdmProviderManifestFunctionBuilder.ForTypes((IEnumerable<PrimitiveTypeKind>) primitiveTypeKindArray11, (Action<PrimitiveTypeKind>) (type => functions.AddFunction(type, "BitwiseNot", type, "value")));
      functions.AddFunction(PrimitiveTypeKind.Guid, "NewGuid");
      EdmProviderManifestSpatialFunctions.AddFunctions(functions);
      Interlocked.CompareExchange<ReadOnlyCollection<EdmFunction>>(ref this._functions, functions.ToFunctionCollection(), (ReadOnlyCollection<EdmFunction>) null);
    }

    internal ReadOnlyCollection<PrimitiveType> GetPromotionTypes(
      PrimitiveType primitiveType)
    {
      this.InitializePromotableTypes();
      return this._promotionTypes[(int) primitiveType.PrimitiveTypeKind];
    }

    private void InitializePromotableTypes()
    {
      if (this._promotionTypes != null)
        return;
      ReadOnlyCollection<PrimitiveType>[] promotionTypes = new ReadOnlyCollection<PrimitiveType>[31];
      for (int index = 0; index < 31; ++index)
        promotionTypes[index] = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) new PrimitiveType[1]
        {
          this._primitiveTypes[index]
        });
      promotionTypes[2] = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) new PrimitiveType[7]
      {
        this._primitiveTypes[2],
        this._primitiveTypes[9],
        this._primitiveTypes[10],
        this._primitiveTypes[11],
        this._primitiveTypes[4],
        this._primitiveTypes[7],
        this._primitiveTypes[5]
      });
      promotionTypes[9] = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) new PrimitiveType[6]
      {
        this._primitiveTypes[9],
        this._primitiveTypes[10],
        this._primitiveTypes[11],
        this._primitiveTypes[4],
        this._primitiveTypes[7],
        this._primitiveTypes[5]
      });
      promotionTypes[10] = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) new PrimitiveType[5]
      {
        this._primitiveTypes[10],
        this._primitiveTypes[11],
        this._primitiveTypes[4],
        this._primitiveTypes[7],
        this._primitiveTypes[5]
      });
      promotionTypes[11] = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) new PrimitiveType[4]
      {
        this._primitiveTypes[11],
        this._primitiveTypes[4],
        this._primitiveTypes[7],
        this._primitiveTypes[5]
      });
      promotionTypes[7] = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) new PrimitiveType[2]
      {
        this._primitiveTypes[7],
        this._primitiveTypes[5]
      });
      this.InitializeSpatialPromotionGroup(promotionTypes, new PrimitiveTypeKind[7]
      {
        PrimitiveTypeKind.GeographyPoint,
        PrimitiveTypeKind.GeographyLineString,
        PrimitiveTypeKind.GeographyPolygon,
        PrimitiveTypeKind.GeographyMultiPoint,
        PrimitiveTypeKind.GeographyMultiLineString,
        PrimitiveTypeKind.GeographyMultiPolygon,
        PrimitiveTypeKind.GeographyCollection
      }, PrimitiveTypeKind.Geography);
      this.InitializeSpatialPromotionGroup(promotionTypes, new PrimitiveTypeKind[7]
      {
        PrimitiveTypeKind.GeometryPoint,
        PrimitiveTypeKind.GeometryLineString,
        PrimitiveTypeKind.GeometryPolygon,
        PrimitiveTypeKind.GeometryMultiPoint,
        PrimitiveTypeKind.GeometryMultiLineString,
        PrimitiveTypeKind.GeometryMultiPolygon,
        PrimitiveTypeKind.GeometryCollection
      }, PrimitiveTypeKind.Geometry);
      Interlocked.CompareExchange<ReadOnlyCollection<PrimitiveType>[]>(ref this._promotionTypes, promotionTypes, (ReadOnlyCollection<PrimitiveType>[]) null);
    }

    private void InitializeSpatialPromotionGroup(
      ReadOnlyCollection<PrimitiveType>[] promotionTypes,
      PrimitiveTypeKind[] promotableKinds,
      PrimitiveTypeKind baseKind)
    {
      foreach (PrimitiveTypeKind promotableKind in promotableKinds)
        promotionTypes[(int) promotableKind] = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) new PrimitiveType[2]
        {
          this._primitiveTypes[(int) promotableKind],
          this._primitiveTypes[(int) baseKind]
        });
    }

    internal TypeUsage GetCanonicalModelTypeUsage(PrimitiveTypeKind primitiveTypeKind)
    {
      if (EdmProviderManifest._canonicalModelTypes == null)
        this.InitializeCanonicalModelTypes();
      return EdmProviderManifest._canonicalModelTypes[(int) primitiveTypeKind];
    }

    private void InitializeCanonicalModelTypes()
    {
      this.InitializePrimitiveTypes();
      TypeUsage[] typeUsageArray = new TypeUsage[31];
      for (int index = 0; index < 31; ++index)
      {
        TypeUsage defaultTypeUsage = TypeUsage.CreateDefaultTypeUsage((EdmType) this._primitiveTypes[index]);
        typeUsageArray[index] = defaultTypeUsage;
      }
      Interlocked.CompareExchange<TypeUsage[]>(ref EdmProviderManifest._canonicalModelTypes, typeUsageArray, (TypeUsage[]) null);
    }

    public override ReadOnlyCollection<PrimitiveType> GetStoreTypes()
    {
      this.InitializePrimitiveTypes();
      return this._primitiveTypes;
    }

    public override TypeUsage GetEdmType(TypeUsage storeType)
    {
      Check.NotNull<TypeUsage>(storeType, nameof (storeType));
      throw new NotImplementedException();
    }

    public override TypeUsage GetStoreType(TypeUsage edmType)
    {
      Check.NotNull<TypeUsage>(edmType, nameof (edmType));
      throw new NotImplementedException();
    }

    internal TypeUsage ForgetScalarConstraints(TypeUsage type)
    {
      PrimitiveType edmType = type.EdmType as PrimitiveType;
      if (edmType != null)
        return this.GetCanonicalModelTypeUsage(edmType.PrimitiveTypeKind);
      return type;
    }

    protected override XmlReader GetDbInformation(string informationType)
    {
      throw new NotImplementedException();
    }
  }
}
