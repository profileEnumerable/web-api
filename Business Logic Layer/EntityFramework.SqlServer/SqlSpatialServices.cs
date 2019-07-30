// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlSpatialServices
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer.Resources;
using System.Data.Entity.SqlServer.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Data.Entity.SqlServer
{
  /// <summary>
  /// An implementation of <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" /> to provide support for geospatial types when using
  /// Entity Framework with Microsoft SQL Server.
  /// </summary>
  [Serializable]
  public class SqlSpatialServices : DbSpatialServices
  {
    internal static readonly SqlSpatialServices Instance = new SqlSpatialServices();
    private static Dictionary<string, SqlSpatialServices> _otherSpatialServices;
    [NonSerialized]
    private readonly SqlTypesAssemblyLoader _loader;

    internal SqlSpatialServices()
    {
    }

    internal SqlSpatialServices(SqlTypesAssemblyLoader loader)
    {
      this._loader = loader;
    }

    /// <inheritdoc />
    public override bool NativeTypesAvailable
    {
      get
      {
        return (this._loader ?? SqlTypesAssemblyLoader.DefaultInstance).TryGetSqlTypesAssembly() != null;
      }
    }

    private static bool TryGetSpatialServiceFromAssembly(
      Assembly assembly,
      out SqlSpatialServices services)
    {
      if (SqlSpatialServices._otherSpatialServices == null || !SqlSpatialServices._otherSpatialServices.TryGetValue(assembly.FullName, out services))
      {
        lock (SqlSpatialServices.Instance)
        {
          if (SqlSpatialServices._otherSpatialServices != null)
          {
            if (SqlSpatialServices._otherSpatialServices.TryGetValue(assembly.FullName, out services))
              goto label_12;
          }
          SqlTypesAssembly sqlAssembly;
          if (SqlTypesAssemblyLoader.DefaultInstance.TryGetSqlTypesAssembly(assembly, out sqlAssembly))
          {
            if (SqlSpatialServices._otherSpatialServices == null)
              SqlSpatialServices._otherSpatialServices = new Dictionary<string, SqlSpatialServices>(1);
            services = new SqlSpatialServices(new SqlTypesAssemblyLoader(sqlAssembly));
            SqlSpatialServices._otherSpatialServices.Add(assembly.FullName, services);
          }
          else
            services = (SqlSpatialServices) null;
        }
      }
label_12:
      return services != null;
    }

    internal SqlTypesAssembly SqlTypes
    {
      get
      {
        return (this._loader ?? SqlTypesAssemblyLoader.DefaultInstance).GetSqlTypesAssembly();
      }
    }

    /// <inheritdoc />
    public override object CreateProviderValue(DbGeographyWellKnownValue wellKnownValue)
    {
      Check.NotNull<DbGeographyWellKnownValue>(wellKnownValue, nameof (wellKnownValue));
      if (wellKnownValue.WellKnownText != null)
        return this.SqlTypes.SqlTypesGeographyFromText(wellKnownValue.WellKnownText, wellKnownValue.CoordinateSystemId);
      if (wellKnownValue.WellKnownBinary != null)
        return this.SqlTypes.SqlTypesGeographyFromBinary(wellKnownValue.WellKnownBinary, wellKnownValue.CoordinateSystemId);
      throw new ArgumentException(Strings.Spatial_WellKnownGeographyValueNotValid, nameof (wellKnownValue));
    }

    /// <inheritdoc />
    public override DbGeography GeographyFromProviderValue(object providerValue)
    {
      Check.NotNull<object>(providerValue, nameof (providerValue));
      object obj = this.NormalizeProviderValue(providerValue, this.SqlTypes.SqlGeographyType);
      if (!this.SqlTypes.IsSqlGeographyNull(obj))
        return DbSpatialServices.CreateGeography((DbSpatialServices) this, obj);
      return (DbGeography) null;
    }

    private object NormalizeProviderValue(object providerValue, Type expectedSpatialType)
    {
      Type type = providerValue.GetType();
      if (type != expectedSpatialType)
      {
        SqlSpatialServices services;
        if (SqlSpatialServices.TryGetSpatialServiceFromAssembly(providerValue.GetType().Assembly(), out services))
        {
          if (expectedSpatialType == this.SqlTypes.SqlGeographyType)
          {
            if (type == services.SqlTypes.SqlGeographyType)
              return this.ConvertToSqlValue(services.GeographyFromProviderValue(providerValue), nameof (providerValue));
          }
          else if (type == services.SqlTypes.SqlGeometryType)
            return this.ConvertToSqlValue(services.GeometryFromProviderValue(providerValue), nameof (providerValue));
        }
        throw new ArgumentException(Strings.SqlSpatialServices_ProviderValueNotSqlType((object) expectedSpatialType.AssemblyQualifiedName), nameof (providerValue));
      }
      return providerValue;
    }

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    public override DbGeographyWellKnownValue CreateWellKnownValue(
      DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return SqlSpatialServices.CreateWellKnownValue<DbGeographyWellKnownValue>(geographyValue.AsSpatialValue(), (Func<Exception>) (() => (Exception) new ArgumentException(Strings.SqlSpatialservices_CouldNotCreateWellKnownGeographyValueNoSrid, nameof (geographyValue))), (Func<Exception>) (() => (Exception) new ArgumentException(Strings.SqlSpatialservices_CouldNotCreateWellKnownGeographyValueNoWkbOrWkt, nameof (geographyValue))), (Func<int, byte[], string, DbGeographyWellKnownValue>) ((coordinateSystemId, wkb, wkt) => new DbGeographyWellKnownValue()
      {
        CoordinateSystemId = coordinateSystemId,
        WellKnownBinary = wkb,
        WellKnownText = wkt
      }));
    }

    /// <inheritdoc />
    public override object CreateProviderValue(DbGeometryWellKnownValue wellKnownValue)
    {
      Check.NotNull<DbGeometryWellKnownValue>(wellKnownValue, nameof (wellKnownValue));
      if (wellKnownValue.WellKnownText != null)
        return this.SqlTypes.SqlTypesGeometryFromText(wellKnownValue.WellKnownText, wellKnownValue.CoordinateSystemId);
      if (wellKnownValue.WellKnownBinary != null)
        return this.SqlTypes.SqlTypesGeometryFromBinary(wellKnownValue.WellKnownBinary, wellKnownValue.CoordinateSystemId);
      throw new ArgumentException(Strings.Spatial_WellKnownGeometryValueNotValid, nameof (wellKnownValue));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryFromProviderValue(object providerValue)
    {
      Check.NotNull<object>(providerValue, nameof (providerValue));
      object obj = this.NormalizeProviderValue(providerValue, this.SqlTypes.SqlGeometryType);
      if (!this.SqlTypes.IsSqlGeometryNull(obj))
        return DbSpatialServices.CreateGeometry((DbSpatialServices) this, obj);
      return (DbGeometry) null;
    }

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    public override DbGeometryWellKnownValue CreateWellKnownValue(
      DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return SqlSpatialServices.CreateWellKnownValue<DbGeometryWellKnownValue>(geometryValue.AsSpatialValue(), (Func<Exception>) (() => (Exception) new ArgumentException(Strings.SqlSpatialservices_CouldNotCreateWellKnownGeometryValueNoSrid, nameof (geometryValue))), (Func<Exception>) (() => (Exception) new ArgumentException(Strings.SqlSpatialservices_CouldNotCreateWellKnownGeometryValueNoWkbOrWkt, nameof (geometryValue))), (Func<int, byte[], string, DbGeometryWellKnownValue>) ((coordinateSystemId, wkb, wkt) => new DbGeometryWellKnownValue()
      {
        CoordinateSystemId = coordinateSystemId,
        WellKnownBinary = wkb,
        WellKnownText = wkt
      }));
    }

    private static TValue CreateWellKnownValue<TValue>(
      IDbSpatialValue spatialValue,
      Func<Exception> onMissingcoordinateSystemId,
      Func<Exception> onMissingWkbAndWkt,
      Func<int, byte[], string, TValue> onValidValue)
    {
      int? coordinateSystemId = spatialValue.CoordinateSystemId;
      if (!coordinateSystemId.HasValue)
        throw onMissingcoordinateSystemId();
      string wellKnownText = spatialValue.WellKnownText;
      if (wellKnownText != null)
        return onValidValue(coordinateSystemId.Value, (byte[]) null, wellKnownText);
      byte[] wellKnownBinary = spatialValue.WellKnownBinary;
      if (wellKnownBinary != null)
        return onValidValue(coordinateSystemId.Value, wellKnownBinary, (string) null);
      throw onMissingWkbAndWkt();
    }

    /// <inheritdoc />
    public override string AsTextIncludingElevationAndMeasure(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.SqlTypes.GeographyAsTextZM(geographyValue);
    }

    /// <inheritdoc />
    public override string AsTextIncludingElevationAndMeasure(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.SqlTypes.GeometryAsTextZM(geometryValue);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argumentName")]
    private object ConvertToSqlValue(DbGeography geographyValue, string argumentName)
    {
      if (geographyValue == null)
        return (object) null;
      return this.SqlTypes.ConvertToSqlTypesGeography(geographyValue);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argumentName")]
    private object ConvertToSqlValue(DbGeometry geometryValue, string argumentName)
    {
      if (geometryValue == null)
        return (object) null;
      return this.SqlTypes.ConvertToSqlTypesGeometry(geometryValue);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argumentName")]
    private object ConvertToSqlBytes(byte[] binaryValue, string argumentName)
    {
      if (binaryValue == null)
        return (object) null;
      return this.SqlTypes.SqlBytesFromByteArray(binaryValue);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argumentName")]
    private object ConvertToSqlChars(string stringValue, string argumentName)
    {
      if (stringValue == null)
        return (object) null;
      return this.SqlTypes.SqlCharsFromString(stringValue);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argumentName")]
    private object ConvertToSqlString(string stringValue, string argumentName)
    {
      if (stringValue == null)
        return (object) null;
      return this.SqlTypes.SqlStringFromString(stringValue);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argumentName")]
    private object ConvertToSqlXml(string stringValue, string argumentName)
    {
      if (stringValue == null)
        return (object) null;
      return this.SqlTypes.SqlXmlFromString(stringValue);
    }

    private bool ConvertSqlBooleanToBoolean(object sqlBoolean)
    {
      return this.SqlTypes.SqlBooleanToBoolean(sqlBoolean);
    }

    private bool? ConvertSqlBooleanToNullableBoolean(object sqlBoolean)
    {
      return this.SqlTypes.SqlBooleanToNullableBoolean(sqlBoolean);
    }

    private byte[] ConvertSqlBytesToBinary(object sqlBytes)
    {
      return this.SqlTypes.SqlBytesToByteArray(sqlBytes);
    }

    private string ConvertSqlCharsToString(object sqlCharsValue)
    {
      return this.SqlTypes.SqlCharsToString(sqlCharsValue);
    }

    private string ConvertSqlStringToString(object sqlCharsValue)
    {
      return this.SqlTypes.SqlStringToString(sqlCharsValue);
    }

    private double ConvertSqlDoubleToDouble(object sqlDoubleValue)
    {
      return this.SqlTypes.SqlDoubleToDouble(sqlDoubleValue);
    }

    private double? ConvertSqlDoubleToNullableDouble(object sqlDoubleValue)
    {
      return this.SqlTypes.SqlDoubleToNullableDouble(sqlDoubleValue);
    }

    private int ConvertSqlInt32ToInt(object sqlInt32Value)
    {
      return this.SqlTypes.SqlInt32ToInt(sqlInt32Value);
    }

    private int? ConvertSqlInt32ToNullableInt(object sqlInt32Value)
    {
      return this.SqlTypes.SqlInt32ToNullableInt(sqlInt32Value);
    }

    private string ConvertSqlXmlToString(object sqlXmlValue)
    {
      return this.SqlTypes.SqlXmlToString(sqlXmlValue);
    }

    /// <inheritdoc />
    public override DbGeography GeographyFromText(string wellKnownText)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyParse.Value.Invoke((object) null, new object[1]
      {
        this.ConvertToSqlString(wellKnownText, nameof (wellKnownText))
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyFromText(
      string wellKnownText,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStGeomFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(wellKnownText, nameof (wellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyPointFromText(
      string pointWellKnownText,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStPointFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(pointWellKnownText, nameof (pointWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyLineFromText(
      string lineWellKnownText,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStLineFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(lineWellKnownText, nameof (lineWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyPolygonFromText(
      string polygonWellKnownText,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStPolyFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(polygonWellKnownText, nameof (polygonWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyMultiPointFromText(
      string multiPointWellKnownText,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStmPointFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(multiPointWellKnownText, nameof (multiPointWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyMultiLineFromText(
      string multiLineWellKnownText,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStmLineFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(multiLineWellKnownText, nameof (multiLineWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyMultiPolygonFromText(
      string multiPolygonKnownText,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStmPolyFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(multiPolygonKnownText, "multiPolygonWellKnownText"),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyCollectionFromText(
      string geographyCollectionWellKnownText,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStGeomCollFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(geographyCollectionWellKnownText, nameof (geographyCollectionWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyFromBinary(
      byte[] wellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStGeomFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(wellKnownBinary, nameof (wellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyFromBinary(byte[] wellKnownBinary)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStGeomFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(wellKnownBinary, nameof (wellKnownBinary)),
        (object) 4326
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyPointFromBinary(
      byte[] pointWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStPointFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(pointWellKnownBinary, nameof (pointWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyLineFromBinary(
      byte[] lineWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStLineFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(lineWellKnownBinary, nameof (lineWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyPolygonFromBinary(
      byte[] polygonWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStPolyFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(polygonWellKnownBinary, nameof (polygonWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyMultiPointFromBinary(
      byte[] multiPointWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStmPointFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(multiPointWellKnownBinary, nameof (multiPointWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyMultiLineFromBinary(
      byte[] multiLineWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStmLineFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(multiLineWellKnownBinary, nameof (multiLineWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyMultiPolygonFromBinary(
      byte[] multiPolygonWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStmPolyFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(multiPolygonWellKnownBinary, nameof (multiPolygonWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyCollectionFromBinary(
      byte[] geographyCollectionWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyStGeomCollFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(geographyCollectionWellKnownBinary, nameof (geographyCollectionWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyFromGml(string geographyMarkup)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyGeomFromGml.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlXml(geographyMarkup, nameof (geographyMarkup)),
        (object) 4326
      }));
    }

    /// <inheritdoc />
    public override DbGeography GeographyFromGml(
      string geographyMarkup,
      int coordinateSystemId)
    {
      return this.GeographyFromProviderValue(this.SqlTypes.SmiSqlGeographyGeomFromGml.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlXml(geographyMarkup, nameof (geographyMarkup)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override int GetCoordinateSystemId(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlInt32ToInt(this.SqlTypes.IpiSqlGeographyStSrid.Value.GetValue(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override string GetSpatialTypeName(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlStringToString(this.SqlTypes.ImiSqlGeographyStGeometryType.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override int GetDimension(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlInt32ToInt(this.SqlTypes.ImiSqlGeographyStDimension.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override byte[] AsBinary(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlBytesToBinary(this.SqlTypes.ImiSqlGeographyStAsBinary.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override string AsGml(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlXmlToString(this.SqlTypes.ImiSqlGeographyAsGml.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override string AsText(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlCharsToString(this.SqlTypes.ImiSqlGeographyStAsText.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override bool GetIsEmpty(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeographyStIsEmpty.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override bool SpatialEquals(DbGeography geographyValue, DbGeography otherGeography)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeographyStEquals.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeography, nameof (otherGeography))
      }));
    }

    /// <inheritdoc />
    public override bool Disjoint(DbGeography geographyValue, DbGeography otherGeography)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeographyStDisjoint.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeography, nameof (otherGeography))
      }));
    }

    /// <inheritdoc />
    public override bool Intersects(DbGeography geographyValue, DbGeography otherGeography)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeographyStIntersects.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeography, nameof (otherGeography))
      }));
    }

    /// <inheritdoc />
    public override DbGeography Buffer(DbGeography geographyValue, double distance)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.GeographyFromProviderValue(this.SqlTypes.ImiSqlGeographyStBuffer.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        (object) distance
      }));
    }

    /// <inheritdoc />
    public override double Distance(DbGeography geographyValue, DbGeography otherGeography)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlDoubleToDouble(this.SqlTypes.ImiSqlGeographyStDistance.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeography, nameof (otherGeography))
      }));
    }

    /// <inheritdoc />
    public override DbGeography Intersection(
      DbGeography geographyValue,
      DbGeography otherGeography)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.GeographyFromProviderValue(this.SqlTypes.ImiSqlGeographyStIntersection.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeography, nameof (otherGeography))
      }));
    }

    /// <inheritdoc />
    public override DbGeography Union(
      DbGeography geographyValue,
      DbGeography otherGeography)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.GeographyFromProviderValue(this.SqlTypes.ImiSqlGeographyStUnion.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeography, nameof (otherGeography))
      }));
    }

    /// <inheritdoc />
    public override DbGeography Difference(
      DbGeography geographyValue,
      DbGeography otherGeography)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.GeographyFromProviderValue(this.SqlTypes.ImiSqlGeographyStDifference.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeography, nameof (otherGeography))
      }));
    }

    /// <inheritdoc />
    public override DbGeography SymmetricDifference(
      DbGeography geographyValue,
      DbGeography otherGeography)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.GeographyFromProviderValue(this.SqlTypes.ImiSqlGeographyStSymDifference.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeography, nameof (otherGeography))
      }));
    }

    /// <inheritdoc />
    public override int? GetElementCount(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlInt32ToNullableInt(this.SqlTypes.ImiSqlGeographyStNumGeometries.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeography ElementAt(DbGeography geographyValue, int index)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.GeographyFromProviderValue(this.SqlTypes.ImiSqlGeographyStGeometryN.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        (object) index
      }));
    }

    /// <inheritdoc />
    public override double? GetLatitude(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.IpiSqlGeographyLat.Value.GetValue(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override double? GetLongitude(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.IpiSqlGeographyLong.Value.GetValue(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override double? GetElevation(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.IpiSqlGeographyZ.Value.GetValue(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override double? GetMeasure(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.IpiSqlGeographyM.Value.GetValue(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override double? GetLength(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.ImiSqlGeographyStLength.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeography GetStartPoint(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.GeographyFromProviderValue(this.SqlTypes.ImiSqlGeographyStStartPoint.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeography GetEndPoint(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.GeographyFromProviderValue(this.SqlTypes.ImiSqlGeographyStEndPoint.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override bool? GetIsClosed(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlBooleanToNullableBoolean(this.SqlTypes.ImiSqlGeographyStIsClosed.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override int? GetPointCount(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlInt32ToNullableInt(this.SqlTypes.ImiSqlGeographyStNumPoints.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeography PointAt(DbGeography geographyValue, int index)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.GeographyFromProviderValue(this.SqlTypes.ImiSqlGeographyStPointN.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[1]
      {
        (object) index
      }));
    }

    /// <inheritdoc />
    public override double? GetArea(DbGeography geographyValue)
    {
      Check.NotNull<DbGeography>(geographyValue, nameof (geographyValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.ImiSqlGeographyStArea.Value.Invoke(this.ConvertToSqlValue(geographyValue, nameof (geographyValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryFromText(string wellKnownText)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryParse.Value.Invoke((object) null, new object[1]
      {
        this.ConvertToSqlString(wellKnownText, nameof (wellKnownText))
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryFromText(
      string wellKnownText,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStGeomFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(wellKnownText, nameof (wellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryPointFromText(
      string pointWellKnownText,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStPointFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(pointWellKnownText, nameof (pointWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryLineFromText(
      string lineWellKnownText,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStLineFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(lineWellKnownText, nameof (lineWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryPolygonFromText(
      string polygonWellKnownText,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStPolyFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(polygonWellKnownText, nameof (polygonWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryMultiPointFromText(
      string multiPointWellKnownText,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStmPointFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(multiPointWellKnownText, nameof (multiPointWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryMultiLineFromText(
      string multiLineWellKnownText,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStmLineFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(multiLineWellKnownText, nameof (multiLineWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryMultiPolygonFromText(
      string multiPolygonKnownText,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStmPolyFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(multiPolygonKnownText, nameof (multiPolygonKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryCollectionFromText(
      string geometryCollectionWellKnownText,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStGeomCollFromText.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlChars(geometryCollectionWellKnownText, nameof (geometryCollectionWellKnownText)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryFromBinary(byte[] wellKnownBinary)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStGeomFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(wellKnownBinary, nameof (wellKnownBinary)),
        (object) 0
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryFromBinary(
      byte[] wellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStGeomFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(wellKnownBinary, nameof (wellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryPointFromBinary(
      byte[] pointWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStPointFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(pointWellKnownBinary, nameof (pointWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryLineFromBinary(
      byte[] lineWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStLineFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(lineWellKnownBinary, nameof (lineWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryPolygonFromBinary(
      byte[] polygonWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStPolyFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(polygonWellKnownBinary, nameof (polygonWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryMultiPointFromBinary(
      byte[] multiPointWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStmPointFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(multiPointWellKnownBinary, nameof (multiPointWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryMultiLineFromBinary(
      byte[] multiLineWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStmLineFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(multiLineWellKnownBinary, nameof (multiLineWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryMultiPolygonFromBinary(
      byte[] multiPolygonWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStmPolyFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(multiPolygonWellKnownBinary, nameof (multiPolygonWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryCollectionFromBinary(
      byte[] geometryCollectionWellKnownBinary,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryStGeomCollFromWkb.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlBytes(geometryCollectionWellKnownBinary, nameof (geometryCollectionWellKnownBinary)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryFromGml(string geometryMarkup)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryGeomFromGml.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlXml(geometryMarkup, nameof (geometryMarkup)),
        (object) 0
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GeometryFromGml(
      string geometryMarkup,
      int coordinateSystemId)
    {
      return this.GeometryFromProviderValue(this.SqlTypes.SmiSqlGeometryGeomFromGml.Value.Invoke((object) null, new object[2]
      {
        this.ConvertToSqlXml(geometryMarkup, nameof (geometryMarkup)),
        (object) coordinateSystemId
      }));
    }

    /// <inheritdoc />
    public override int GetCoordinateSystemId(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlInt32ToInt(this.SqlTypes.IpiSqlGeometryStSrid.Value.GetValue(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override string GetSpatialTypeName(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlStringToString(this.SqlTypes.ImiSqlGeometryStGeometryType.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override int GetDimension(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlInt32ToInt(this.SqlTypes.ImiSqlGeometryStDimension.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry GetEnvelope(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStEnvelope.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override byte[] AsBinary(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBytesToBinary(this.SqlTypes.ImiSqlGeometryStAsBinary.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override string AsGml(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlXmlToString(this.SqlTypes.ImiSqlGeometryAsGml.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override string AsText(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlCharsToString(this.SqlTypes.ImiSqlGeometryStAsText.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override bool GetIsEmpty(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStIsEmpty.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override bool GetIsSimple(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStIsSimple.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry GetBoundary(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStBoundary.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override bool GetIsValid(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStIsValid.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override bool SpatialEquals(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStEquals.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override bool Disjoint(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStDisjoint.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override bool Intersects(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStIntersects.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override bool Touches(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStTouches.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override bool Crosses(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStCrosses.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override bool Within(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStWithin.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override bool Contains(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStContains.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override bool Overlaps(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStOverlaps.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override bool Relate(DbGeometry geometryValue, DbGeometry otherGeometry, string matrix)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToBoolean(this.SqlTypes.ImiSqlGeometryStRelate.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[2]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry)),
        (object) matrix
      }));
    }

    /// <inheritdoc />
    public override DbGeometry Buffer(DbGeometry geometryValue, double distance)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStBuffer.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        (object) distance
      }));
    }

    /// <inheritdoc />
    public override double Distance(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlDoubleToDouble(this.SqlTypes.ImiSqlGeometryStDistance.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override DbGeometry GetConvexHull(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStConvexHull.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry Intersection(
      DbGeometry geometryValue,
      DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStIntersection.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override DbGeometry Union(DbGeometry geometryValue, DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStUnion.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override DbGeometry Difference(
      DbGeometry geometryValue,
      DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStDifference.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override DbGeometry SymmetricDifference(
      DbGeometry geometryValue,
      DbGeometry otherGeometry)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStSymDifference.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        this.ConvertToSqlValue(otherGeometry, nameof (otherGeometry))
      }));
    }

    /// <inheritdoc />
    public override int? GetElementCount(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlInt32ToNullableInt(this.SqlTypes.ImiSqlGeometryStNumGeometries.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry ElementAt(DbGeometry geometryValue, int index)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStGeometryN.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        (object) index
      }));
    }

    /// <inheritdoc />
    public override double? GetXCoordinate(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.IpiSqlGeometryStx.Value.GetValue(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override double? GetYCoordinate(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.IpiSqlGeometrySty.Value.GetValue(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override double? GetElevation(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.IpiSqlGeometryZ.Value.GetValue(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override double? GetMeasure(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.IpiSqlGeometryM.Value.GetValue(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), (object[]) null));
    }

    /// <inheritdoc />
    public override double? GetLength(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.ImiSqlGeometryStLength.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry GetStartPoint(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStStartPoint.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry GetEndPoint(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStEndPoint.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override bool? GetIsClosed(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToNullableBoolean(this.SqlTypes.ImiSqlGeometryStIsClosed.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override bool? GetIsRing(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlBooleanToNullableBoolean(this.SqlTypes.ImiSqlGeometryStIsRing.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override int? GetPointCount(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlInt32ToNullableInt(this.SqlTypes.ImiSqlGeometryStNumPoints.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry PointAt(DbGeometry geometryValue, int index)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStPointN.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        (object) index
      }));
    }

    /// <inheritdoc />
    public override double? GetArea(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlDoubleToNullableDouble(this.SqlTypes.ImiSqlGeometryStArea.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry GetCentroid(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStCentroid.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry GetPointOnSurface(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStPointOnSurface.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry GetExteriorRing(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStExteriorRing.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override int? GetInteriorRingCount(DbGeometry geometryValue)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.ConvertSqlInt32ToNullableInt(this.SqlTypes.ImiSqlGeometryStNumInteriorRing.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[0]));
    }

    /// <inheritdoc />
    public override DbGeometry InteriorRingAt(DbGeometry geometryValue, int index)
    {
      Check.NotNull<DbGeometry>(geometryValue, nameof (geometryValue));
      return this.GeometryFromProviderValue(this.SqlTypes.ImiSqlGeometryStInteriorRingN.Value.Invoke(this.ConvertToSqlValue(geometryValue, nameof (geometryValue)), new object[1]
      {
        (object) index
      }));
    }
  }
}
