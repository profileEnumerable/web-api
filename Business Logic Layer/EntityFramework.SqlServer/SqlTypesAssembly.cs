// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlTypesAssembly
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

namespace System.Data.Entity.SqlServer
{
  internal class SqlTypesAssembly
  {
    private readonly Func<object, bool> sqlBooleanToBoolean;
    private readonly Func<object, bool?> sqlBooleanToNullableBoolean;
    private readonly Func<byte[], object> sqlBytesFromByteArray;
    private readonly Func<object, byte[]> sqlBytesToByteArray;
    private readonly Func<string, object> sqlStringFromString;
    private readonly Func<string, object> sqlCharsFromString;
    private readonly Func<object, string> sqlCharsToString;
    private readonly Func<object, string> sqlStringToString;
    private readonly Func<object, double> sqlDoubleToDouble;
    private readonly Func<object, double?> sqlDoubleToNullableDouble;
    private readonly Func<object, int> sqlInt32ToInt;
    private readonly Func<object, int?> sqlInt32ToNullableInt;
    private readonly Func<XmlReader, object> sqlXmlFromXmlReader;
    private readonly Func<object, string> sqlXmlToString;
    private readonly Func<object, bool> isSqlGeographyNull;
    private readonly Func<object, bool> isSqlGeometryNull;
    private readonly Func<object, object> geographyAsTextZMAsSqlChars;
    private readonly Func<object, object> geometryAsTextZMAsSqlChars;
    private readonly Func<string, int, object> sqlGeographyFromWKTString;
    private readonly Func<byte[], int, object> sqlGeographyFromWKBByteArray;
    private readonly Func<XmlReader, int, object> sqlGeographyFromGMLReader;
    private readonly Func<string, int, object> sqlGeometryFromWKTString;
    private readonly Func<byte[], int, object> sqlGeometryFromWKBByteArray;
    private readonly Func<XmlReader, int, object> sqlGeometryFromGMLReader;
    private readonly Lazy<MethodInfo> _smiSqlGeographyParse;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStGeomFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStPointFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStLineFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStPolyFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStmPointFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStmLineFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStmPolyFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStGeomCollFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStGeomFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStPointFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStLineFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStPolyFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStmPointFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStmLineFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStmPolyFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeographyStGeomCollFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeographyGeomFromGml;
    private readonly Lazy<PropertyInfo> _ipiSqlGeographyStSrid;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStGeometryType;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStDimension;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStAsBinary;
    private readonly Lazy<MethodInfo> _imiSqlGeographyAsGml;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStAsText;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStIsEmpty;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStEquals;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStDisjoint;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStIntersects;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStBuffer;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStDistance;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStIntersection;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStUnion;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStDifference;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStSymDifference;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStNumGeometries;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStGeometryN;
    private readonly Lazy<PropertyInfo> _ipiSqlGeographyLat;
    private readonly Lazy<PropertyInfo> _ipiSqlGeographyLong;
    private readonly Lazy<PropertyInfo> _ipiSqlGeographyZ;
    private readonly Lazy<PropertyInfo> _ipiSqlGeographyM;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStLength;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStStartPoint;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStEndPoint;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStIsClosed;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStNumPoints;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStPointN;
    private readonly Lazy<MethodInfo> _imiSqlGeographyStArea;
    private readonly Lazy<MethodInfo> _smiSqlGeometryParse;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStGeomFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStPointFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStLineFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStPolyFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStmPointFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStmLineFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStmPolyFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStGeomCollFromText;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStGeomFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStPointFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStLineFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStPolyFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStmPointFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStmLineFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStmPolyFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeometryStGeomCollFromWkb;
    private readonly Lazy<MethodInfo> _smiSqlGeometryGeomFromGml;
    private readonly Lazy<PropertyInfo> _ipiSqlGeometryStSrid;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStGeometryType;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStDimension;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStEnvelope;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStAsBinary;
    private readonly Lazy<MethodInfo> _imiSqlGeometryAsGml;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStAsText;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStIsEmpty;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStIsSimple;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStBoundary;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStIsValid;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStEquals;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStDisjoint;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStIntersects;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStTouches;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStCrosses;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStWithin;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStContains;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStOverlaps;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStRelate;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStBuffer;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStDistance;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStConvexHull;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStIntersection;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStUnion;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStDifference;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStSymDifference;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStNumGeometries;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStGeometryN;
    private readonly Lazy<PropertyInfo> _ipiSqlGeometryStx;
    private readonly Lazy<PropertyInfo> _ipiSqlGeometrySty;
    private readonly Lazy<PropertyInfo> _ipiSqlGeometryZ;
    private readonly Lazy<PropertyInfo> _ipiSqlGeometryM;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStLength;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStStartPoint;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStEndPoint;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStIsClosed;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStIsRing;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStNumPoints;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStPointN;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStArea;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStCentroid;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStPointOnSurface;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStExteriorRing;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStNumInteriorRing;
    private readonly Lazy<MethodInfo> _imiSqlGeometryStInteriorRingN;

    public SqlTypesAssembly()
    {
    }

    [SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
    public SqlTypesAssembly(Assembly sqlSpatialAssembly)
    {
      Type type1 = sqlSpatialAssembly.GetType("Microsoft.SqlServer.Types.SqlGeography", true);
      Type type2 = sqlSpatialAssembly.GetType("Microsoft.SqlServer.Types.SqlGeometry", true);
      this.SqlGeographyType = type1;
      this.sqlGeographyFromWKTString = SqlTypesAssembly.CreateStaticConstructorDelegate<string>(type1, "STGeomFromText");
      this.sqlGeographyFromWKBByteArray = SqlTypesAssembly.CreateStaticConstructorDelegate<byte[]>(type1, "STGeomFromWKB");
      this.sqlGeographyFromGMLReader = SqlTypesAssembly.CreateStaticConstructorDelegate<XmlReader>(type1, "GeomFromGml");
      this.SqlGeometryType = type2;
      this.sqlGeometryFromWKTString = SqlTypesAssembly.CreateStaticConstructorDelegate<string>(type2, "STGeomFromText");
      this.sqlGeometryFromWKBByteArray = SqlTypesAssembly.CreateStaticConstructorDelegate<byte[]>(type2, "STGeomFromWKB");
      this.sqlGeometryFromGMLReader = SqlTypesAssembly.CreateStaticConstructorDelegate<XmlReader>(type2, "GeomFromGml");
      this.SqlCharsType = this.SqlGeometryType.GetPublicInstanceMethod("STAsText").ReturnType;
      this.SqlStringType = this.SqlCharsType.Assembly().GetType("System.Data.SqlTypes.SqlString", true);
      this.SqlBooleanType = this.SqlCharsType.Assembly().GetType("System.Data.SqlTypes.SqlBoolean", true);
      this.SqlBytesType = this.SqlCharsType.Assembly().GetType("System.Data.SqlTypes.SqlBytes", true);
      this.SqlDoubleType = this.SqlCharsType.Assembly().GetType("System.Data.SqlTypes.SqlDouble", true);
      this.SqlInt32Type = this.SqlCharsType.Assembly().GetType("System.Data.SqlTypes.SqlInt32", true);
      this.SqlXmlType = this.SqlCharsType.Assembly().GetType("System.Data.SqlTypes.SqlXml", true);
      this.sqlBytesFromByteArray = System.Data.Entity.SqlServer.Expressions.Lambda<byte[], object>("binaryValue", (Func<ParameterExpression, Expression>) (bytesVal => SqlTypesAssembly.BuildConvertToSqlBytes((Expression) bytesVal, this.SqlBytesType))).Compile();
      this.sqlStringFromString = System.Data.Entity.SqlServer.Expressions.Lambda<string, object>("stringValue", (Func<ParameterExpression, Expression>) (stringVal => SqlTypesAssembly.BuildConvertToSqlString((Expression) stringVal, this.SqlStringType))).Compile();
      this.sqlCharsFromString = System.Data.Entity.SqlServer.Expressions.Lambda<string, object>("stringValue", (Func<ParameterExpression, Expression>) (stringVal => SqlTypesAssembly.BuildConvertToSqlChars((Expression) stringVal, this.SqlCharsType))).Compile();
      this.sqlXmlFromXmlReader = System.Data.Entity.SqlServer.Expressions.Lambda<XmlReader, object>("readerVaue", (Func<ParameterExpression, Expression>) (readerVal => SqlTypesAssembly.BuildConvertToSqlXml((Expression) readerVal, this.SqlXmlType))).Compile();
      this.sqlBooleanToBoolean = System.Data.Entity.SqlServer.Expressions.Lambda<object, bool>("sqlBooleanValue", (Func<ParameterExpression, Expression>) (sqlBoolVal => sqlBoolVal.ConvertTo(this.SqlBooleanType).ConvertTo<bool>())).Compile();
      this.sqlBooleanToNullableBoolean = System.Data.Entity.SqlServer.Expressions.Lambda<object, bool?>("sqlBooleanValue", (Func<ParameterExpression, Expression>) (sqlBoolVal => sqlBoolVal.ConvertTo(this.SqlBooleanType).Property<bool>("IsNull").IfTrueThen(System.Data.Entity.SqlServer.Expressions.Null<bool?>()).Else(sqlBoolVal.ConvertTo(this.SqlBooleanType).ConvertTo<bool>().ConvertTo<bool?>()))).Compile();
      this.sqlBytesToByteArray = System.Data.Entity.SqlServer.Expressions.Lambda<object, byte[]>("sqlBytesValue", (Func<ParameterExpression, Expression>) (sqlBytesVal => sqlBytesVal.ConvertTo(this.SqlBytesType).Property<byte[]>("Value"))).Compile();
      this.sqlCharsToString = System.Data.Entity.SqlServer.Expressions.Lambda<object, string>("sqlCharsValue", (Func<ParameterExpression, Expression>) (sqlCharsVal => sqlCharsVal.ConvertTo(this.SqlCharsType).Call("ToSqlString").Property<string>("Value"))).Compile();
      this.sqlStringToString = System.Data.Entity.SqlServer.Expressions.Lambda<object, string>("sqlStringValue", (Func<ParameterExpression, Expression>) (sqlStringVal => sqlStringVal.ConvertTo(this.SqlStringType).Property<string>("Value"))).Compile();
      this.sqlDoubleToDouble = System.Data.Entity.SqlServer.Expressions.Lambda<object, double>("sqlDoubleValue", (Func<ParameterExpression, Expression>) (sqlDoubleVal => sqlDoubleVal.ConvertTo(this.SqlDoubleType).ConvertTo<double>())).Compile();
      this.sqlDoubleToNullableDouble = System.Data.Entity.SqlServer.Expressions.Lambda<object, double?>("sqlDoubleValue", (Func<ParameterExpression, Expression>) (sqlDoubleVal => sqlDoubleVal.ConvertTo(this.SqlDoubleType).Property<bool>("IsNull").IfTrueThen(System.Data.Entity.SqlServer.Expressions.Null<double?>()).Else(sqlDoubleVal.ConvertTo(this.SqlDoubleType).ConvertTo<double>().ConvertTo<double?>()))).Compile();
      this.sqlInt32ToInt = System.Data.Entity.SqlServer.Expressions.Lambda<object, int>("sqlInt32Value", (Func<ParameterExpression, Expression>) (sqlInt32Val => sqlInt32Val.ConvertTo(this.SqlInt32Type).ConvertTo<int>())).Compile();
      this.sqlInt32ToNullableInt = System.Data.Entity.SqlServer.Expressions.Lambda<object, int?>("sqlInt32Value", (Func<ParameterExpression, Expression>) (sqlInt32Val => sqlInt32Val.ConvertTo(this.SqlInt32Type).Property<bool>("IsNull").IfTrueThen(System.Data.Entity.SqlServer.Expressions.Null<int?>()).Else(sqlInt32Val.ConvertTo(this.SqlInt32Type).ConvertTo<int>().ConvertTo<int?>()))).Compile();
      this.sqlXmlToString = System.Data.Entity.SqlServer.Expressions.Lambda<object, string>("sqlXmlValue", (Func<ParameterExpression, Expression>) (sqlXmlVal => sqlXmlVal.ConvertTo(this.SqlXmlType).Property<string>("Value"))).Compile();
      this.isSqlGeographyNull = System.Data.Entity.SqlServer.Expressions.Lambda<object, bool>("sqlGeographyValue", (Func<ParameterExpression, Expression>) (sqlGeographyValue => sqlGeographyValue.ConvertTo(this.SqlGeographyType).Property<bool>("IsNull"))).Compile();
      this.isSqlGeometryNull = System.Data.Entity.SqlServer.Expressions.Lambda<object, bool>("sqlGeometryValue", (Func<ParameterExpression, Expression>) (sqlGeometryValue => sqlGeometryValue.ConvertTo(this.SqlGeometryType).Property<bool>("IsNull"))).Compile();
      this.geographyAsTextZMAsSqlChars = System.Data.Entity.SqlServer.Expressions.Lambda<object, object>("sqlGeographyValue", (Func<ParameterExpression, Expression>) (sqlGeographyValue => sqlGeographyValue.ConvertTo(this.SqlGeographyType).Call("AsTextZM"))).Compile();
      this.geometryAsTextZMAsSqlChars = System.Data.Entity.SqlServer.Expressions.Lambda<object, object>("sqlGeometryValue", (Func<ParameterExpression, Expression>) (sqlGeometryValue => sqlGeometryValue.ConvertTo(this.SqlGeometryType).Call("AsTextZM"))).Compile();
      this._smiSqlGeographyParse = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("Parse", this.SqlStringType)), true);
      this._smiSqlGeographyStGeomFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STGeomFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeographyStPointFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STPointFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeographyStLineFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STLineFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeographyStPolyFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STPolyFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeographyStmPointFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STMPointFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeographyStmLineFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STMLineFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeographyStmPolyFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STMPolyFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeographyStGeomCollFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STGeomCollFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeographyStGeomFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STGeomFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeographyStPointFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STPointFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeographyStLineFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STLineFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeographyStPolyFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STPolyFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeographyStmPointFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STMPointFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeographyStmLineFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STMLineFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeographyStmPolyFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STMPolyFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeographyStGeomCollFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("STGeomCollFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeographyGeomFromGml = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyStaticMethod("GeomFromGml", this.SqlXmlType, typeof (int))), true);
      this._ipiSqlGeographyStSrid = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeographyProperty("STSrid")), true);
      this._imiSqlGeographyStGeometryType = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STGeometryType")), true);
      this._imiSqlGeographyStDimension = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STDimension")), true);
      this._imiSqlGeographyStAsBinary = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STAsBinary")), true);
      this._imiSqlGeographyAsGml = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("AsGml")), true);
      this._imiSqlGeographyStAsText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STAsText")), true);
      this._imiSqlGeographyStIsEmpty = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STIsEmpty")), true);
      this._imiSqlGeographyStEquals = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STEquals", this.SqlGeographyType)), true);
      this._imiSqlGeographyStDisjoint = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STDisjoint", this.SqlGeographyType)), true);
      this._imiSqlGeographyStIntersects = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STIntersects", this.SqlGeographyType)), true);
      this._imiSqlGeographyStBuffer = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STBuffer", typeof (double))), true);
      this._imiSqlGeographyStDistance = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STDistance", this.SqlGeographyType)), true);
      this._imiSqlGeographyStIntersection = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STIntersection", this.SqlGeographyType)), true);
      this._imiSqlGeographyStUnion = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STUnion", this.SqlGeographyType)), true);
      this._imiSqlGeographyStDifference = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STDifference", this.SqlGeographyType)), true);
      this._imiSqlGeographyStSymDifference = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STSymDifference", this.SqlGeographyType)), true);
      this._imiSqlGeographyStNumGeometries = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STNumGeometries")), true);
      this._imiSqlGeographyStGeometryN = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STGeometryN", typeof (int))), true);
      this._ipiSqlGeographyLat = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeographyProperty("Lat")), true);
      this._ipiSqlGeographyLong = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeographyProperty("Long")), true);
      this._ipiSqlGeographyZ = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeographyProperty("Z")), true);
      this._ipiSqlGeographyM = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeographyProperty("M")), true);
      this._imiSqlGeographyStLength = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STLength")), true);
      this._imiSqlGeographyStStartPoint = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STStartPoint")), true);
      this._imiSqlGeographyStEndPoint = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STEndPoint")), true);
      this._imiSqlGeographyStIsClosed = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STIsClosed")), true);
      this._imiSqlGeographyStNumPoints = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STNumPoints")), true);
      this._imiSqlGeographyStPointN = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STPointN", typeof (int))), true);
      this._imiSqlGeographyStArea = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeographyMethod("STArea")), true);
      this._smiSqlGeometryParse = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("Parse", this.SqlStringType)), true);
      this._smiSqlGeometryStGeomFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STGeomFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeometryStPointFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STPointFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeometryStLineFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STLineFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeometryStPolyFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STPolyFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeometryStmPointFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STMPointFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeometryStmLineFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STMLineFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeometryStmPolyFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STMPolyFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeometryStGeomCollFromText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STGeomCollFromText", this.SqlCharsType, typeof (int))), true);
      this._smiSqlGeometryStGeomFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STGeomFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeometryStPointFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STPointFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeometryStLineFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STLineFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeometryStPolyFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STPolyFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeometryStmPointFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STMPointFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeometryStmLineFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STMLineFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeometryStmPolyFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STMPolyFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeometryStGeomCollFromWkb = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("STGeomCollFromWKB", this.SqlBytesType, typeof (int))), true);
      this._smiSqlGeometryGeomFromGml = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryStaticMethod("GeomFromGml", this.SqlXmlType, typeof (int))), true);
      this._ipiSqlGeometryStSrid = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeometryProperty("STSrid")), true);
      this._imiSqlGeometryStGeometryType = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STGeometryType")), true);
      this._imiSqlGeometryStDimension = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STDimension")), true);
      this._imiSqlGeometryStEnvelope = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STEnvelope")), true);
      this._imiSqlGeometryStAsBinary = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STAsBinary")), true);
      this._imiSqlGeometryAsGml = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("AsGml")), true);
      this._imiSqlGeometryStAsText = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STAsText")), true);
      this._imiSqlGeometryStIsEmpty = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STIsEmpty")), true);
      this._imiSqlGeometryStIsSimple = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STIsSimple")), true);
      this._imiSqlGeometryStBoundary = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STBoundary")), true);
      this._imiSqlGeometryStIsValid = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STIsValid")), true);
      this._imiSqlGeometryStEquals = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STEquals", this.SqlGeometryType)), true);
      this._imiSqlGeometryStDisjoint = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STDisjoint", this.SqlGeometryType)), true);
      this._imiSqlGeometryStIntersects = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STIntersects", this.SqlGeometryType)), true);
      this._imiSqlGeometryStTouches = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STTouches", this.SqlGeometryType)), true);
      this._imiSqlGeometryStCrosses = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STCrosses", this.SqlGeometryType)), true);
      this._imiSqlGeometryStWithin = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STWithin", this.SqlGeometryType)), true);
      this._imiSqlGeometryStContains = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STContains", this.SqlGeometryType)), true);
      this._imiSqlGeometryStOverlaps = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STOverlaps", this.SqlGeometryType)), true);
      this._imiSqlGeometryStRelate = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STRelate", this.SqlGeometryType, typeof (string))), true);
      this._imiSqlGeometryStBuffer = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STBuffer", typeof (double))), true);
      this._imiSqlGeometryStDistance = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STDistance", this.SqlGeometryType)), true);
      this._imiSqlGeometryStConvexHull = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STConvexHull")), true);
      this._imiSqlGeometryStIntersection = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STIntersection", this.SqlGeometryType)), true);
      this._imiSqlGeometryStUnion = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STUnion", this.SqlGeometryType)), true);
      this._imiSqlGeometryStDifference = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STDifference", this.SqlGeometryType)), true);
      this._imiSqlGeometryStSymDifference = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STSymDifference", this.SqlGeometryType)), true);
      this._imiSqlGeometryStNumGeometries = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STNumGeometries")), true);
      this._imiSqlGeometryStGeometryN = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STGeometryN", typeof (int))), true);
      this._ipiSqlGeometryStx = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeometryProperty("STX")), true);
      this._ipiSqlGeometrySty = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeometryProperty("STY")), true);
      this._ipiSqlGeometryZ = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeometryProperty("Z")), true);
      this._ipiSqlGeometryM = new Lazy<PropertyInfo>((Func<PropertyInfo>) (() => this.FindSqlGeometryProperty("M")), true);
      this._imiSqlGeometryStLength = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STLength")), true);
      this._imiSqlGeometryStStartPoint = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STStartPoint")), true);
      this._imiSqlGeometryStEndPoint = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STEndPoint")), true);
      this._imiSqlGeometryStIsClosed = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STIsClosed")), true);
      this._imiSqlGeometryStIsRing = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STIsRing")), true);
      this._imiSqlGeometryStNumPoints = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STNumPoints")), true);
      this._imiSqlGeometryStPointN = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STPointN", typeof (int))), true);
      this._imiSqlGeometryStArea = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STArea")), true);
      this._imiSqlGeometryStCentroid = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STCentroid")), true);
      this._imiSqlGeometryStPointOnSurface = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STPointOnSurface")), true);
      this._imiSqlGeometryStExteriorRing = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STExteriorRing")), true);
      this._imiSqlGeometryStNumInteriorRing = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STNumInteriorRing")), true);
      this._imiSqlGeometryStInteriorRingN = new Lazy<MethodInfo>((Func<MethodInfo>) (() => this.FindSqlGeometryMethod("STInteriorRingN", typeof (int))), true);
    }

    internal Type SqlBooleanType { get; private set; }

    internal Type SqlBytesType { get; private set; }

    internal Type SqlCharsType { get; private set; }

    internal Type SqlStringType { get; private set; }

    internal Type SqlDoubleType { get; private set; }

    internal Type SqlInt32Type { get; private set; }

    internal Type SqlXmlType { get; private set; }

    internal bool SqlBooleanToBoolean(object sqlBooleanValue)
    {
      return this.sqlBooleanToBoolean(sqlBooleanValue);
    }

    internal bool? SqlBooleanToNullableBoolean(object sqlBooleanValue)
    {
      if (this.sqlBooleanToBoolean == null)
        return new bool?();
      return this.sqlBooleanToNullableBoolean(sqlBooleanValue);
    }

    internal object SqlBytesFromByteArray(byte[] binaryValue)
    {
      return this.sqlBytesFromByteArray(binaryValue);
    }

    internal byte[] SqlBytesToByteArray(object sqlBytesValue)
    {
      if (sqlBytesValue == null)
        return (byte[]) null;
      return this.sqlBytesToByteArray(sqlBytesValue);
    }

    internal object SqlStringFromString(string stringValue)
    {
      return this.sqlStringFromString(stringValue);
    }

    internal object SqlCharsFromString(string stringValue)
    {
      return this.sqlCharsFromString(stringValue);
    }

    internal string SqlCharsToString(object sqlCharsValue)
    {
      if (sqlCharsValue == null)
        return (string) null;
      return this.sqlCharsToString(sqlCharsValue);
    }

    internal string SqlStringToString(object sqlStringValue)
    {
      if (sqlStringValue == null)
        return (string) null;
      return this.sqlStringToString(sqlStringValue);
    }

    internal double SqlDoubleToDouble(object sqlDoubleValue)
    {
      return this.sqlDoubleToDouble(sqlDoubleValue);
    }

    internal double? SqlDoubleToNullableDouble(object sqlDoubleValue)
    {
      if (sqlDoubleValue == null)
        return new double?();
      return this.sqlDoubleToNullableDouble(sqlDoubleValue);
    }

    internal int SqlInt32ToInt(object sqlInt32Value)
    {
      return this.sqlInt32ToInt(sqlInt32Value);
    }

    internal int? SqlInt32ToNullableInt(object sqlInt32Value)
    {
      if (sqlInt32Value == null)
        return new int?();
      return this.sqlInt32ToNullableInt(sqlInt32Value);
    }

    internal object SqlXmlFromString(string stringValue)
    {
      return this.sqlXmlFromXmlReader(SqlTypesAssembly.XmlReaderFromString(stringValue));
    }

    internal string SqlXmlToString(object sqlXmlValue)
    {
      if (sqlXmlValue == null)
        return (string) null;
      return this.sqlXmlToString(sqlXmlValue);
    }

    internal bool IsSqlGeographyNull(object sqlGeographyValue)
    {
      if (sqlGeographyValue == null)
        return true;
      return this.isSqlGeographyNull(sqlGeographyValue);
    }

    internal bool IsSqlGeometryNull(object sqlGeometryValue)
    {
      if (sqlGeometryValue == null)
        return true;
      return this.isSqlGeometryNull(sqlGeometryValue);
    }

    internal string GeographyAsTextZM(DbGeography geographyValue)
    {
      if (geographyValue == null)
        return (string) null;
      return this.SqlCharsToString(this.geographyAsTextZMAsSqlChars(this.ConvertToSqlTypesGeography(geographyValue)));
    }

    internal string GeometryAsTextZM(DbGeometry geometryValue)
    {
      if (geometryValue == null)
        return (string) null;
      return this.SqlCharsToString(this.geometryAsTextZMAsSqlChars(this.ConvertToSqlTypesGeometry(geometryValue)));
    }

    internal Type SqlGeographyType { get; private set; }

    internal Type SqlGeometryType { get; private set; }

    internal object ConvertToSqlTypesGeography(DbGeography geographyValue)
    {
      return this.GetSqlTypesSpatialValue(geographyValue.AsSpatialValue(), this.SqlGeographyType);
    }

    internal object SqlTypesGeographyFromBinary(byte[] wellKnownBinary, int srid)
    {
      return this.sqlGeographyFromWKBByteArray(wellKnownBinary, srid);
    }

    internal object SqlTypesGeographyFromText(string wellKnownText, int srid)
    {
      return this.sqlGeographyFromWKTString(wellKnownText, srid);
    }

    internal object ConvertToSqlTypesGeometry(DbGeometry geometryValue)
    {
      return this.GetSqlTypesSpatialValue(geometryValue.AsSpatialValue(), this.SqlGeometryType);
    }

    internal object SqlTypesGeometryFromBinary(byte[] wellKnownBinary, int srid)
    {
      return this.sqlGeometryFromWKBByteArray(wellKnownBinary, srid);
    }

    internal object SqlTypesGeometryFromText(string wellKnownText, int srid)
    {
      return this.sqlGeometryFromWKTString(wellKnownText, srid);
    }

    private object GetSqlTypesSpatialValue(
      IDbSpatialValue spatialValue,
      Type requiredProviderValueType)
    {
      object providerValue = spatialValue.ProviderValue;
      if (providerValue != null && providerValue.GetType() == requiredProviderValueType)
        return providerValue;
      int? coordinateSystemId = spatialValue.CoordinateSystemId;
      if (coordinateSystemId.HasValue)
      {
        byte[] wellKnownBinary = spatialValue.WellKnownBinary;
        if (wellKnownBinary != null)
        {
          if (!spatialValue.IsGeography)
            return this.sqlGeometryFromWKBByteArray(wellKnownBinary, coordinateSystemId.Value);
          return this.sqlGeographyFromWKBByteArray(wellKnownBinary, coordinateSystemId.Value);
        }
        string wellKnownText = spatialValue.WellKnownText;
        if (wellKnownText != null)
        {
          if (!spatialValue.IsGeography)
            return this.sqlGeometryFromWKTString(wellKnownText, coordinateSystemId.Value);
          return this.sqlGeographyFromWKTString(wellKnownText, coordinateSystemId.Value);
        }
        string gmlString = spatialValue.GmlString;
        if (gmlString != null)
        {
          XmlReader xmlReader = SqlTypesAssembly.XmlReaderFromString(gmlString);
          if (!spatialValue.IsGeography)
            return this.sqlGeometryFromGMLReader(xmlReader, coordinateSystemId.Value);
          return this.sqlGeographyFromGMLReader(xmlReader, coordinateSystemId.Value);
        }
      }
      throw spatialValue.NotSqlCompatible();
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    private static XmlReader XmlReaderFromString(string stringValue)
    {
      return XmlReader.Create((TextReader) new StringReader(stringValue));
    }

    private static Func<TArg, int, object> CreateStaticConstructorDelegate<TArg>(
      Type spatialType,
      string methodName)
    {
      ParameterExpression parameterExpression3 = Expression.Parameter(typeof (TArg));
      ParameterExpression parameterExpression4 = Expression.Parameter(typeof (int));
      MethodInfo onlyDeclaredMethod = spatialType.GetOnlyDeclaredMethod(methodName);
      Expression sqlType = SqlTypesAssembly.BuildConvertToSqlType((Expression) parameterExpression3, onlyDeclaredMethod.GetParameters()[0].ParameterType);
      return ((Expression<Func<TArg, int, object>>) ((parameterExpression1, parameterExpression2) => Expression.Call((Expression) null, onlyDeclaredMethod, sqlType, parameterExpression2))).Compile();
    }

    private static Expression BuildConvertToSqlType(Expression toConvert, Type convertTo)
    {
      if (toConvert.Type == typeof (byte[]))
        return SqlTypesAssembly.BuildConvertToSqlBytes(toConvert, convertTo);
      if (toConvert.Type == typeof (string))
      {
        if (convertTo.Name == "SqlString")
          return SqlTypesAssembly.BuildConvertToSqlString(toConvert, convertTo);
        return SqlTypesAssembly.BuildConvertToSqlChars(toConvert, convertTo);
      }
      if (toConvert.Type == typeof (XmlReader))
        return SqlTypesAssembly.BuildConvertToSqlXml(toConvert, convertTo);
      return toConvert;
    }

    private static Expression BuildConvertToSqlBytes(
      Expression toConvert,
      Type sqlBytesType)
    {
      return (Expression) Expression.New(sqlBytesType.GetDeclaredConstructor(toConvert.Type), toConvert);
    }

    private static Expression BuildConvertToSqlChars(
      Expression toConvert,
      Type sqlCharsType)
    {
      Type type = sqlCharsType.Assembly().GetType("System.Data.SqlTypes.SqlString", true);
      return (Expression) Expression.New(sqlCharsType.GetDeclaredConstructor(type), (Expression) Expression.New(type.GetDeclaredConstructor(typeof (string)), toConvert));
    }

    private static Expression BuildConvertToSqlString(
      Expression toConvert,
      Type sqlStringType)
    {
      return (Expression) Expression.Convert((Expression) Expression.New(sqlStringType.GetDeclaredConstructor(typeof (string)), toConvert), typeof (object));
    }

    private static Expression BuildConvertToSqlXml(Expression toConvert, Type sqlXmlType)
    {
      return (Expression) Expression.New(sqlXmlType.GetDeclaredConstructor(toConvert.Type), toConvert);
    }

    public Lazy<MethodInfo> SmiSqlGeographyParse
    {
      get
      {
        return this._smiSqlGeographyParse;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStGeomFromText
    {
      get
      {
        return this._smiSqlGeographyStGeomFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStPointFromText
    {
      get
      {
        return this._smiSqlGeographyStPointFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStLineFromText
    {
      get
      {
        return this._smiSqlGeographyStLineFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStPolyFromText
    {
      get
      {
        return this._smiSqlGeographyStPolyFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStmPointFromText
    {
      get
      {
        return this._smiSqlGeographyStmPointFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStmLineFromText
    {
      get
      {
        return this._smiSqlGeographyStmLineFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStmPolyFromText
    {
      get
      {
        return this._smiSqlGeographyStmPolyFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStGeomCollFromText
    {
      get
      {
        return this._smiSqlGeographyStGeomCollFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStGeomFromWkb
    {
      get
      {
        return this._smiSqlGeographyStGeomFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStPointFromWkb
    {
      get
      {
        return this._smiSqlGeographyStPointFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStLineFromWkb
    {
      get
      {
        return this._smiSqlGeographyStLineFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStPolyFromWkb
    {
      get
      {
        return this._smiSqlGeographyStPolyFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStmPointFromWkb
    {
      get
      {
        return this._smiSqlGeographyStmPointFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStmLineFromWkb
    {
      get
      {
        return this._smiSqlGeographyStmLineFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStmPolyFromWkb
    {
      get
      {
        return this._smiSqlGeographyStmPolyFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyStGeomCollFromWkb
    {
      get
      {
        return this._smiSqlGeographyStGeomCollFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeographyGeomFromGml
    {
      get
      {
        return this._smiSqlGeographyGeomFromGml;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeographyStSrid
    {
      get
      {
        return this._ipiSqlGeographyStSrid;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStGeometryType
    {
      get
      {
        return this._imiSqlGeographyStGeometryType;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStDimension
    {
      get
      {
        return this._imiSqlGeographyStDimension;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStAsBinary
    {
      get
      {
        return this._imiSqlGeographyStAsBinary;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyAsGml
    {
      get
      {
        return this._imiSqlGeographyAsGml;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStAsText
    {
      get
      {
        return this._imiSqlGeographyStAsText;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStIsEmpty
    {
      get
      {
        return this._imiSqlGeographyStIsEmpty;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStEquals
    {
      get
      {
        return this._imiSqlGeographyStEquals;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStDisjoint
    {
      get
      {
        return this._imiSqlGeographyStDisjoint;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStIntersects
    {
      get
      {
        return this._imiSqlGeographyStIntersects;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStBuffer
    {
      get
      {
        return this._imiSqlGeographyStBuffer;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStDistance
    {
      get
      {
        return this._imiSqlGeographyStDistance;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStIntersection
    {
      get
      {
        return this._imiSqlGeographyStIntersection;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStUnion
    {
      get
      {
        return this._imiSqlGeographyStUnion;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStDifference
    {
      get
      {
        return this._imiSqlGeographyStDifference;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStSymDifference
    {
      get
      {
        return this._imiSqlGeographyStSymDifference;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStNumGeometries
    {
      get
      {
        return this._imiSqlGeographyStNumGeometries;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStGeometryN
    {
      get
      {
        return this._imiSqlGeographyStGeometryN;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeographyLat
    {
      get
      {
        return this._ipiSqlGeographyLat;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeographyLong
    {
      get
      {
        return this._ipiSqlGeographyLong;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeographyZ
    {
      get
      {
        return this._ipiSqlGeographyZ;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeographyM
    {
      get
      {
        return this._ipiSqlGeographyM;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStLength
    {
      get
      {
        return this._imiSqlGeographyStLength;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStStartPoint
    {
      get
      {
        return this._imiSqlGeographyStStartPoint;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStEndPoint
    {
      get
      {
        return this._imiSqlGeographyStEndPoint;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStIsClosed
    {
      get
      {
        return this._imiSqlGeographyStIsClosed;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStNumPoints
    {
      get
      {
        return this._imiSqlGeographyStNumPoints;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStPointN
    {
      get
      {
        return this._imiSqlGeographyStPointN;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeographyStArea
    {
      get
      {
        return this._imiSqlGeographyStArea;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryParse
    {
      get
      {
        return this._smiSqlGeometryParse;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStGeomFromText
    {
      get
      {
        return this._smiSqlGeometryStGeomFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStPointFromText
    {
      get
      {
        return this._smiSqlGeometryStPointFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStLineFromText
    {
      get
      {
        return this._smiSqlGeometryStLineFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStPolyFromText
    {
      get
      {
        return this._smiSqlGeometryStPolyFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStmPointFromText
    {
      get
      {
        return this._smiSqlGeometryStmPointFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStmLineFromText
    {
      get
      {
        return this._smiSqlGeometryStmLineFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStmPolyFromText
    {
      get
      {
        return this._smiSqlGeometryStmPolyFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStGeomCollFromText
    {
      get
      {
        return this._smiSqlGeometryStGeomCollFromText;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStGeomFromWkb
    {
      get
      {
        return this._smiSqlGeometryStGeomFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStPointFromWkb
    {
      get
      {
        return this._smiSqlGeometryStPointFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStLineFromWkb
    {
      get
      {
        return this._smiSqlGeometryStLineFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStPolyFromWkb
    {
      get
      {
        return this._smiSqlGeometryStPolyFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStmPointFromWkb
    {
      get
      {
        return this._smiSqlGeometryStmPointFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStmLineFromWkb
    {
      get
      {
        return this._smiSqlGeometryStmLineFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStmPolyFromWkb
    {
      get
      {
        return this._smiSqlGeometryStmPolyFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryStGeomCollFromWkb
    {
      get
      {
        return this._smiSqlGeometryStGeomCollFromWkb;
      }
    }

    public Lazy<MethodInfo> SmiSqlGeometryGeomFromGml
    {
      get
      {
        return this._smiSqlGeometryGeomFromGml;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeometryStSrid
    {
      get
      {
        return this._ipiSqlGeometryStSrid;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStGeometryType
    {
      get
      {
        return this._imiSqlGeometryStGeometryType;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStDimension
    {
      get
      {
        return this._imiSqlGeometryStDimension;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStEnvelope
    {
      get
      {
        return this._imiSqlGeometryStEnvelope;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStAsBinary
    {
      get
      {
        return this._imiSqlGeometryStAsBinary;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryAsGml
    {
      get
      {
        return this._imiSqlGeometryAsGml;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStAsText
    {
      get
      {
        return this._imiSqlGeometryStAsText;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStIsEmpty
    {
      get
      {
        return this._imiSqlGeometryStIsEmpty;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStIsSimple
    {
      get
      {
        return this._imiSqlGeometryStIsSimple;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStBoundary
    {
      get
      {
        return this._imiSqlGeometryStBoundary;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStIsValid
    {
      get
      {
        return this._imiSqlGeometryStIsValid;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStEquals
    {
      get
      {
        return this._imiSqlGeometryStEquals;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStDisjoint
    {
      get
      {
        return this._imiSqlGeometryStDisjoint;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStIntersects
    {
      get
      {
        return this._imiSqlGeometryStIntersects;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStTouches
    {
      get
      {
        return this._imiSqlGeometryStTouches;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStCrosses
    {
      get
      {
        return this._imiSqlGeometryStCrosses;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStWithin
    {
      get
      {
        return this._imiSqlGeometryStWithin;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStContains
    {
      get
      {
        return this._imiSqlGeometryStContains;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStOverlaps
    {
      get
      {
        return this._imiSqlGeometryStOverlaps;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStRelate
    {
      get
      {
        return this._imiSqlGeometryStRelate;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStBuffer
    {
      get
      {
        return this._imiSqlGeometryStBuffer;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStDistance
    {
      get
      {
        return this._imiSqlGeometryStDistance;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStConvexHull
    {
      get
      {
        return this._imiSqlGeometryStConvexHull;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStIntersection
    {
      get
      {
        return this._imiSqlGeometryStIntersection;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStUnion
    {
      get
      {
        return this._imiSqlGeometryStUnion;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStDifference
    {
      get
      {
        return this._imiSqlGeometryStDifference;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStSymDifference
    {
      get
      {
        return this._imiSqlGeometryStSymDifference;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStNumGeometries
    {
      get
      {
        return this._imiSqlGeometryStNumGeometries;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStGeometryN
    {
      get
      {
        return this._imiSqlGeometryStGeometryN;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeometryStx
    {
      get
      {
        return this._ipiSqlGeometryStx;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeometrySty
    {
      get
      {
        return this._ipiSqlGeometrySty;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeometryZ
    {
      get
      {
        return this._ipiSqlGeometryZ;
      }
    }

    public Lazy<PropertyInfo> IpiSqlGeometryM
    {
      get
      {
        return this._ipiSqlGeometryM;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStLength
    {
      get
      {
        return this._imiSqlGeometryStLength;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStStartPoint
    {
      get
      {
        return this._imiSqlGeometryStStartPoint;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStEndPoint
    {
      get
      {
        return this._imiSqlGeometryStEndPoint;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStIsClosed
    {
      get
      {
        return this._imiSqlGeometryStIsClosed;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStIsRing
    {
      get
      {
        return this._imiSqlGeometryStIsRing;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStNumPoints
    {
      get
      {
        return this._imiSqlGeometryStNumPoints;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStPointN
    {
      get
      {
        return this._imiSqlGeometryStPointN;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStArea
    {
      get
      {
        return this._imiSqlGeometryStArea;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStCentroid
    {
      get
      {
        return this._imiSqlGeometryStCentroid;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStPointOnSurface
    {
      get
      {
        return this._imiSqlGeometryStPointOnSurface;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStExteriorRing
    {
      get
      {
        return this._imiSqlGeometryStExteriorRing;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStNumInteriorRing
    {
      get
      {
        return this._imiSqlGeometryStNumInteriorRing;
      }
    }

    public Lazy<MethodInfo> ImiSqlGeometryStInteriorRingN
    {
      get
      {
        return this._imiSqlGeometryStInteriorRingN;
      }
    }

    private MethodInfo FindSqlGeographyMethod(string methodName, params Type[] argTypes)
    {
      return this.SqlGeographyType.GetDeclaredMethod(methodName, argTypes);
    }

    private MethodInfo FindSqlGeographyStaticMethod(
      string methodName,
      params Type[] argTypes)
    {
      return this.SqlGeographyType.GetDeclaredMethod(methodName, argTypes);
    }

    private PropertyInfo FindSqlGeographyProperty(string propertyName)
    {
      return this.SqlGeographyType.GetRuntimeProperty(propertyName);
    }

    private MethodInfo FindSqlGeometryStaticMethod(
      string methodName,
      params Type[] argTypes)
    {
      return this.SqlGeometryType.GetDeclaredMethod(methodName, argTypes);
    }

    private MethodInfo FindSqlGeometryMethod(string methodName, params Type[] argTypes)
    {
      return this.SqlGeometryType.GetDeclaredMethod(methodName, argTypes);
    }

    private PropertyInfo FindSqlGeometryProperty(string propertyName)
    {
      return this.SqlGeometryType.GetRuntimeProperty(propertyName);
    }
  }
}
