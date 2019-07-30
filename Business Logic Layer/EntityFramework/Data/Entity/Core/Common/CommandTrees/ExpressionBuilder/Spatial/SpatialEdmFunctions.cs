// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder.Spatial.SpatialEdmFunctions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder.Spatial
{
  /// <summary>
  /// Provides an API to construct <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />s that invoke spatial realted canonical EDM functions, and, where appropriate, allows that API to be accessed as extension methods on the expression type itself.
  /// </summary>
  public static class SpatialEdmFunctions
  {
    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryFromText' function with the specified argument, which must have a string result type. The result type of the expression is Edm.Geometry. Its value has the default coordinate system id (SRID) of the underlying provider.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry value based on the specified value.</returns>
    /// <param name="wellKnownText">An expression that provides the well known text representation of the geometry value.</param>
    public static DbFunctionExpression GeometryFromText(
      DbExpression wellKnownText)
    {
      Check.NotNull<DbExpression>(wellKnownText, nameof (wellKnownText));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryFromText), wellKnownText);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryFromText' function with the specified arguments. wellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry value based on the specified values.</returns>
    /// <param name="wellKnownText">An expression that provides the well known text representation of the geometry value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry value's coordinate system.</param>
    public static DbFunctionExpression GeometryFromText(
      DbExpression wellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(wellKnownText, nameof (wellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryFromText), wellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryPointFromText' function with the specified arguments. pointWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry point value based on the specified values.</returns>
    /// <param name="pointWellKnownText">An expression that provides the well known text representation of the geometry point value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry point value's coordinate system.</param>
    public static DbFunctionExpression GeometryPointFromText(
      DbExpression pointWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(pointWellKnownText, nameof (pointWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryPointFromText), pointWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryLineFromText' function with the specified arguments. lineWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry line value based on the specified values.</returns>
    /// <param name="lineWellKnownText">An expression that provides the well known text representation of the geometry line value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry line value's coordinate system.</param>
    public static DbFunctionExpression GeometryLineFromText(
      DbExpression lineWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(lineWellKnownText, nameof (lineWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryLineFromText), lineWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryPolygonFromText' function with the specified arguments. polygonWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry polygon value based on the specified values.</returns>
    /// <param name="polygonWellKnownText">An expression that provides the well known text representation of the geometry polygon value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry polygon value's coordinate system.</param>
    public static DbFunctionExpression GeometryPolygonFromText(
      DbExpression polygonWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(polygonWellKnownText, nameof (polygonWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryPolygonFromText), polygonWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryMultiPointFromText' function with the specified arguments. multiPointWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry multi-point value based on the specified values.</returns>
    /// <param name="multiPointWellKnownText">An expression that provides the well known text representation of the geometry multi-point value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry multi-point value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "MultiPoint")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "multiPoint")]
    public static DbFunctionExpression GeometryMultiPointFromText(
      DbExpression multiPointWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiPointWellKnownText, nameof (multiPointWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryMultiPointFromText), multiPointWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryMultiLineFromText' function with the specified arguments. multiLineWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry multi-line value based on the specified values.</returns>
    /// <param name="multiLineWellKnownText">An expression that provides the well known text representation of the geometry multi-line value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry multi-line value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "MultiLine")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "multiLine")]
    public static DbFunctionExpression GeometryMultiLineFromText(
      DbExpression multiLineWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiLineWellKnownText, nameof (multiLineWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryMultiLineFromText), multiLineWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryMultiPolygonFromText' function with the specified arguments. multiPolygonWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry multi-polygon value based on the specified values.</returns>
    /// <param name="multiPolygonWellKnownText">An expression that provides the well known text representation of the geometry multi-polygon value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry multi-polygon value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    public static DbFunctionExpression GeometryMultiPolygonFromText(
      DbExpression multiPolygonWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiPolygonWellKnownText, nameof (multiPolygonWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryMultiPolygonFromText), multiPolygonWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryCollectionFromText' function with the specified arguments. geometryCollectionWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry collection value based on the specified values.</returns>
    /// <param name="geometryCollectionWellKnownText">An expression that provides the well known text representation of the geometry collection value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry collection value's coordinate system.</param>
    public static DbFunctionExpression GeometryCollectionFromText(
      DbExpression geometryCollectionWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(geometryCollectionWellKnownText, nameof (geometryCollectionWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryCollectionFromText), geometryCollectionWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryFromBinary' function with the specified argument, which must have a binary result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry value based on the specified binary value.</returns>
    /// <param name="wellKnownBinaryValue">An expression that provides the well known binary representation of the geometry value.</param>
    public static DbFunctionExpression GeometryFromBinary(
      DbExpression wellKnownBinaryValue)
    {
      Check.NotNull<DbExpression>(wellKnownBinaryValue, nameof (wellKnownBinaryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryFromBinary), wellKnownBinaryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryFromBinary' function with the specified arguments. wellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry value based on the specified values.</returns>
    /// <param name="wellKnownBinaryValue">An expression that provides the well known binary representation of the geometry value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry value's coordinate system.</param>
    public static DbFunctionExpression GeometryFromBinary(
      DbExpression wellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(wellKnownBinaryValue, nameof (wellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryFromBinary), wellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryPointFromBinary' function with the specified arguments. pointWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry point value based on the specified values.</returns>
    /// <param name="pointWellKnownBinaryValue">An expression that provides the well known binary representation of the geometry point value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry point value's coordinate system.</param>
    public static DbFunctionExpression GeometryPointFromBinary(
      DbExpression pointWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(pointWellKnownBinaryValue, nameof (pointWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryPointFromBinary), pointWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryLineFromBinary' function with the specified arguments. lineWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry line value based on the specified values.</returns>
    /// <param name="lineWellKnownBinaryValue">An expression that provides the well known binary representation of the geometry line value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry line value's coordinate system.</param>
    public static DbFunctionExpression GeometryLineFromBinary(
      DbExpression lineWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(lineWellKnownBinaryValue, nameof (lineWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryLineFromBinary), lineWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryPolygonFromBinary' function with the specified arguments. polygonWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry polygon value based on the specified values.</returns>
    /// <param name="polygonWellKnownBinaryValue">An expression that provides the well known binary representation of the geometry polygon value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry polygon value's coordinate system.</param>
    public static DbFunctionExpression GeometryPolygonFromBinary(
      DbExpression polygonWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(polygonWellKnownBinaryValue, nameof (polygonWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryPolygonFromBinary), polygonWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryMultiPointFromBinary' function with the specified arguments. multiPointWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry multi-point value based on the specified values.</returns>
    /// <param name="multiPointWellKnownBinaryValue">An expression that provides the well known binary representation of the geometry multi-point value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry multi-point value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "MultiPoint")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "multiPoint")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    public static DbFunctionExpression GeometryMultiPointFromBinary(
      DbExpression multiPointWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiPointWellKnownBinaryValue, nameof (multiPointWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryMultiPointFromBinary), multiPointWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryMultiLineFromBinary' function with the specified arguments. multiLineWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry multi-line value based on the specified values.</returns>
    /// <param name="multiLineWellKnownBinaryValue">An expression that provides the well known binary representation of the geometry multi-line value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry multi-line value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "MultiLine")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "multiLine")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    public static DbFunctionExpression GeometryMultiLineFromBinary(
      DbExpression multiLineWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiLineWellKnownBinaryValue, nameof (multiLineWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryMultiLineFromBinary), multiLineWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryMultiPolygonFromBinary' function with the specified arguments. multiPolygonWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry multi-polygon value based on the specified values.</returns>
    /// <param name="multiPolygonWellKnownBinaryValue">An expression that provides the well known binary representation of the geometry multi-polygon value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry multi-polygon value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    public static DbFunctionExpression GeometryMultiPolygonFromBinary(
      DbExpression multiPolygonWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiPolygonWellKnownBinaryValue, nameof (multiPolygonWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryMultiPolygonFromBinary), multiPolygonWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryCollectionFromBinary' function with the specified arguments. geometryCollectionWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry collection value based on the specified values.</returns>
    /// <param name="geometryCollectionWellKnownBinaryValue">An expression that provides the well known binary representation of the geometry collection value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry collection value's coordinate system.</param>
    public static DbFunctionExpression GeometryCollectionFromBinary(
      DbExpression geometryCollectionWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(geometryCollectionWellKnownBinaryValue, nameof (geometryCollectionWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryCollectionFromBinary), geometryCollectionWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryFromGml' function with the specified argument, which must have a string result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry value based on the specified value with the default coordinate system id (SRID) of the underlying provider.</returns>
    /// <param name="geometryMarkup">An expression that provides the Geography Markup Language (GML) representation of the geometry value.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Abbreviation more meaningful than what it stands for", MessageId = "Gml")]
    public static DbFunctionExpression GeometryFromGml(
      DbExpression geometryMarkup)
    {
      Check.NotNull<DbExpression>(geometryMarkup, nameof (geometryMarkup));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryFromGml), geometryMarkup);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeometryFromGml' function with the specified arguments. geometryMarkup must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geometry value based on the specified values.</returns>
    /// <param name="geometryMarkup">An expression that provides the Geography Markup Language (GML) representation of the geometry value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geometry value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Abbreviation more meaningful than what it stands for", MessageId = "Gml")]
    public static DbFunctionExpression GeometryFromGml(
      DbExpression geometryMarkup,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(geometryMarkup, nameof (geometryMarkup));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeometryFromGml), geometryMarkup, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyFromText' function with the specified argument, which must have a string result type. The result type of the expression is Edm.Geography. Its value has the default coordinate system id (SRID) of the underlying provider.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography value based on the specified value.</returns>
    /// <param name="wellKnownText">An expression that provides the well known text representation of the geography value.</param>
    public static DbFunctionExpression GeographyFromText(
      DbExpression wellKnownText)
    {
      Check.NotNull<DbExpression>(wellKnownText, nameof (wellKnownText));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyFromText), wellKnownText);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyFromText' function with the specified arguments. wellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography value based on the specified values.</returns>
    /// <param name="wellKnownText">An expression that provides the well known text representation of the geography value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography value's coordinate system.</param>
    public static DbFunctionExpression GeographyFromText(
      DbExpression wellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(wellKnownText, nameof (wellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyFromText), wellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyPointFromText' function with the specified arguments.
    /// </summary>
    /// <returns>The canonical 'GeographyPointFromText' function.</returns>
    /// <param name="pointWellKnownText">An expression that provides the well-known text representation of the geography point value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography point value's coordinate systempointWellKnownTextValue.</param>
    public static DbFunctionExpression GeographyPointFromText(
      DbExpression pointWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(pointWellKnownText, nameof (pointWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyPointFromText), pointWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyLineFromText' function with the specified arguments. lineWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography line value based on the specified values.</returns>
    /// <param name="lineWellKnownText">An expression that provides the well known text representation of the geography line value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography line value's coordinate system.</param>
    public static DbFunctionExpression GeographyLineFromText(
      DbExpression lineWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(lineWellKnownText, nameof (lineWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyLineFromText), lineWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyPolygonFromText' function with the specified arguments. polygonWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography polygon value based on the specified values.</returns>
    /// <param name="polygonWellKnownText">An expression that provides the well known text representation of the geography polygon value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography polygon value's coordinate system.</param>
    public static DbFunctionExpression GeographyPolygonFromText(
      DbExpression polygonWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(polygonWellKnownText, nameof (polygonWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyPolygonFromText), polygonWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyMultiPointFromText' function with the specified arguments. multiPointWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography multi-point value based on the specified values.</returns>
    /// <param name="multiPointWellKnownText">An expression that provides the well known text representation of the geography multi-point value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography multi-point value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "MultiPoint")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "multiPoint")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    public static DbFunctionExpression GeographyMultiPointFromText(
      DbExpression multiPointWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiPointWellKnownText, nameof (multiPointWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyMultiPointFromText), multiPointWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyMultiLineFromText' function with the specified arguments. multiLineWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography multi-line value based on the specified values.</returns>
    /// <param name="multiLineWellKnownText">An expression that provides the well known text representation of the geography multi-line value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography multi-line value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "MultiLine")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "multiLine")]
    public static DbFunctionExpression GeographyMultiLineFromText(
      DbExpression multiLineWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiLineWellKnownText, nameof (multiLineWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyMultiLineFromText), multiLineWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyMultiPolygonFromText' function with the specified arguments. multiPolygonWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography multi-polygon value based on the specified values.</returns>
    /// <param name="multiPolygonWellKnownText">An expression that provides the well known text representation of the geography multi-polygon value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography multi-polygon value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    public static DbFunctionExpression GeographyMultiPolygonFromText(
      DbExpression multiPolygonWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiPolygonWellKnownText, nameof (multiPolygonWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyMultiPolygonFromText), multiPolygonWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyCollectionFromText' function with the specified arguments. geographyCollectionWellKnownText must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography collection value based on the specified values.</returns>
    /// <param name="geographyCollectionWellKnownText">An expression that provides the well known text representation of the geography collection value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography collection value's coordinate system.</param>
    public static DbFunctionExpression GeographyCollectionFromText(
      DbExpression geographyCollectionWellKnownText,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(geographyCollectionWellKnownText, nameof (geographyCollectionWellKnownText));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyCollectionFromText), geographyCollectionWellKnownText, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyFromBinary' function with the specified argument, which must have a binary result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography value based on the specified binary value.</returns>
    /// <param name="wellKnownBinaryValue">An expression that provides the well known binary representation of the geography value.</param>
    public static DbFunctionExpression GeographyFromBinary(
      DbExpression wellKnownBinaryValue)
    {
      Check.NotNull<DbExpression>(wellKnownBinaryValue, nameof (wellKnownBinaryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyFromBinary), wellKnownBinaryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyFromBinary' function with the specified arguments. wellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography value based on the specified values.</returns>
    /// <param name="wellKnownBinaryValue">An expression that provides the well known binary representation of the geography value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography value's coordinate system.</param>
    public static DbFunctionExpression GeographyFromBinary(
      DbExpression wellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(wellKnownBinaryValue, nameof (wellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyFromBinary), wellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyPointFromBinary' function with the specified arguments. pointWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography point value based on the specified values.</returns>
    /// <param name="pointWellKnownBinaryValue">An expression that provides the well known binary representation of the geography point value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography point value's coordinate systempointWellKnownBinaryValue.</param>
    public static DbFunctionExpression GeographyPointFromBinary(
      DbExpression pointWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(pointWellKnownBinaryValue, nameof (pointWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyPointFromBinary), pointWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyLineFromBinary' function with the specified arguments. lineWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography line value based on the specified values.</returns>
    /// <param name="lineWellKnownBinaryValue">An expression that provides the well known binary representation of the geography line value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography line value's coordinate system.</param>
    public static DbFunctionExpression GeographyLineFromBinary(
      DbExpression lineWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(lineWellKnownBinaryValue, nameof (lineWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyLineFromBinary), lineWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyPolygonFromBinary' function with the specified arguments. polygonWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography polygon value based on the specified values.</returns>
    /// <param name="polygonWellKnownBinaryValue">An expression that provides the well known binary representation of the geography polygon value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography polygon value's coordinate system.</param>
    public static DbFunctionExpression GeographyPolygonFromBinary(
      DbExpression polygonWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(polygonWellKnownBinaryValue, nameof (polygonWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyPolygonFromBinary), polygonWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyMultiPointFromBinary' function with the specified arguments. multiPointWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography multi-point value based on the specified values.</returns>
    /// <param name="multiPointWellKnownBinaryValue">An expression that provides the well known binary representation of the geography multi-point value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography multi-point value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "MultiPoint")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "multiPoint")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    public static DbFunctionExpression GeographyMultiPointFromBinary(
      DbExpression multiPointWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiPointWellKnownBinaryValue, nameof (multiPointWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyMultiPointFromBinary), multiPointWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyMultiLineFromBinary' function with the specified arguments. multiLineWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography multi-line value based on the specified values.</returns>
    /// <param name="multiLineWellKnownBinaryValue">An expression that provides the well known binary representation of the geography multi-line value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography multi-line value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "multiLine")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Match OGC, EDM", MessageId = "MultiLine")]
    public static DbFunctionExpression GeographyMultiLineFromBinary(
      DbExpression multiLineWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiLineWellKnownBinaryValue, nameof (multiLineWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyMultiLineFromBinary), multiLineWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyMultiPolygonFromBinary' function with the specified arguments. multiPolygonWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography multi-polygon value based on the specified values.</returns>
    /// <param name="multiPolygonWellKnownBinaryValue">An expression that provides the well known binary representation of the geography multi-polygon value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography multi-polygon value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "multi")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Match OGC, EDM", MessageId = "Multi")]
    public static DbFunctionExpression GeographyMultiPolygonFromBinary(
      DbExpression multiPolygonWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(multiPolygonWellKnownBinaryValue, nameof (multiPolygonWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyMultiPolygonFromBinary), multiPolygonWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyCollectionFromBinary' function with the specified arguments. geographyCollectionWellKnownBinaryValue must have a binary result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography collection value based on the specified values.</returns>
    /// <param name="geographyCollectionWellKnownBinaryValue">An expression that provides the well known binary representation of the geography collection value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography collection value's coordinate system.</param>
    public static DbFunctionExpression GeographyCollectionFromBinary(
      DbExpression geographyCollectionWellKnownBinaryValue,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(geographyCollectionWellKnownBinaryValue, nameof (geographyCollectionWellKnownBinaryValue));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyCollectionFromBinary), geographyCollectionWellKnownBinaryValue, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyFromGml' function with the specified argument, which must have a string result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography value based on the specified value with the default coordinate system id (SRID) of the underlying provider.</returns>
    /// <param name="geographyMarkup">An expression that provides the Geography Markup Language (GML) representation of the geography value.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml")]
    public static DbFunctionExpression GeographyFromGml(
      DbExpression geographyMarkup)
    {
      Check.NotNull<DbExpression>(geographyMarkup, nameof (geographyMarkup));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyFromGml), geographyMarkup);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GeographyFromGml' function with the specified arguments. geographyMarkup must have a string result type, while coordinateSystemId must have an integer numeric result type. The result type of the expression is Edm.Geography.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new geography value based on the specified values.</returns>
    /// <param name="geographyMarkup">An expression that provides the Geography Markup Language (GML) representation of the geography value.</param>
    /// <param name="coordinateSystemId">An expression that provides the coordinate system id (SRID) of the geography value's coordinate system.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml")]
    public static DbFunctionExpression GeographyFromGml(
      DbExpression geographyMarkup,
      DbExpression coordinateSystemId)
    {
      Check.NotNull<DbExpression>(geographyMarkup, nameof (geographyMarkup));
      Check.NotNull<DbExpression>(coordinateSystemId, nameof (coordinateSystemId));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GeographyFromGml), geographyMarkup, coordinateSystemId);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'CoordinateSystemId' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the integer SRID value from spatialValue.</returns>
    /// <param name="spatialValue">An expression that specifies the value from which the coordinate system id (SRID) should be retrieved.</param>
    public static DbFunctionExpression CoordinateSystemId(
      this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (CoordinateSystemId), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialTypeName' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.String.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the string Geometry Type name from spatialValue.</returns>
    /// <param name="spatialValue">An expression that specifies the value from which the Geometry Type name should be retrieved.</param>
    public static DbFunctionExpression SpatialTypeName(
      this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialTypeName), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialDimension' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the Dimension value from spatialValue.</returns>
    /// <param name="spatialValue">An expression that specifies the value from which the Dimension value should be retrieved.</param>
    public static DbFunctionExpression SpatialDimension(
      this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialDimension), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialEnvelope' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the the minimum bounding box for geometryValue.</returns>
    /// <param name="geometryValue">An expression that specifies the value from which the Envelope value should be retrieved.</param>
    public static DbFunctionExpression SpatialEnvelope(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialEnvelope), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AsBinary' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Binary.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the well known binary representation of spatialValue.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial value from which the well known binary representation should be produced.</param>
    public static DbFunctionExpression AsBinary(this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AsBinary), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AsGml' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.String.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the Geography Markup Language (GML) representation of spatialValue.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial value from which the Geography Markup Language (GML) representation should be produced.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml")]
    public static DbFunctionExpression AsGml(this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AsGml), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AsText' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.String.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the well known text representation of spatialValue.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial value from which the well known text representation should be produced.</param>
    public static DbFunctionExpression AsText(this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AsText), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'IsEmptySpatial' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether spatialValue is empty.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial value from which the IsEmptySptiaal value should be retrieved.</param>
    public static DbFunctionExpression IsEmptySpatial(
      this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (IsEmptySpatial), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'IsSimpleGeometry' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether geometryValue is a simple geometry.</returns>
    /// <param name="geometryValue">The geometry value.</param>
    public static DbFunctionExpression IsSimpleGeometry(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (IsSimpleGeometry), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialBoundary' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the the boundary for geometryValue.</returns>
    /// <param name="geometryValue">An expression that specifies the geometry value from which the SpatialBoundary value should be retrieved.</param>
    public static DbFunctionExpression SpatialBoundary(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialBoundary), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'IsValidGeometry' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether geometryValue is valid.</returns>
    /// <param name="geometryValue">An expression that specifies the geometry value which should be tested for spatial validity.</param>
    public static DbFunctionExpression IsValidGeometry(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (IsValidGeometry), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialEquals' function with the specified arguments, which must each have an Edm.Geography or Edm.Geometry result type. The result type of spatialValue1 must match the result type of spatialValue2. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether spatialValue1 and spatialValue2 are equal.</returns>
    /// <param name="spatialValue1">An expression that specifies the first spatial value.</param>
    /// <param name="spatialValue2">An expression that specifies the spatial value that should be compared with spatialValue1 for equality.</param>
    public static DbFunctionExpression SpatialEquals(
      this DbExpression spatialValue1,
      DbExpression spatialValue2)
    {
      Check.NotNull<DbExpression>(spatialValue1, nameof (spatialValue1));
      Check.NotNull<DbExpression>(spatialValue2, nameof (spatialValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialEquals), spatialValue1, spatialValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialDisjoint' function with the specified arguments, which must each have an Edm.Geography or Edm.Geometry result type. The result type of spatialValue1 must match the result type of spatialValue2. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether spatialValue1 and spatialValue2 are spatially disjoint.</returns>
    /// <param name="spatialValue1">An expression that specifies the first spatial value.</param>
    /// <param name="spatialValue2">An expression that specifies the spatial value that should be compared with spatialValue1 for disjointness.</param>
    public static DbFunctionExpression SpatialDisjoint(
      this DbExpression spatialValue1,
      DbExpression spatialValue2)
    {
      Check.NotNull<DbExpression>(spatialValue1, nameof (spatialValue1));
      Check.NotNull<DbExpression>(spatialValue2, nameof (spatialValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialDisjoint), spatialValue1, spatialValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialIntersects' function with the specified arguments, which must each have an Edm.Geography or Edm.Geometry result type. The result type of spatialValue1 must match the result type of spatialValue2. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether spatialValue1 and spatialValue2 intersect.</returns>
    /// <param name="spatialValue1">An expression that specifies the first spatial value.</param>
    /// <param name="spatialValue2">An expression that specifies the spatial value that should be compared with spatialValue1 for intersection.</param>
    public static DbFunctionExpression SpatialIntersects(
      this DbExpression spatialValue1,
      DbExpression spatialValue2)
    {
      Check.NotNull<DbExpression>(spatialValue1, nameof (spatialValue1));
      Check.NotNull<DbExpression>(spatialValue2, nameof (spatialValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialIntersects), spatialValue1, spatialValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialTouches' function with the specified arguments, which must each have an Edm.Geometry result type. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether geometryValue1 touches geometryValue2.</returns>
    /// <param name="geometryValue1">An expression that specifies the first geometry value.</param>
    /// <param name="geometryValue2">An expression that specifies the geometry value that should be compared with geometryValue1.</param>
    public static DbFunctionExpression SpatialTouches(
      this DbExpression geometryValue1,
      DbExpression geometryValue2)
    {
      Check.NotNull<DbExpression>(geometryValue1, nameof (geometryValue1));
      Check.NotNull<DbExpression>(geometryValue2, nameof (geometryValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialTouches), geometryValue1, geometryValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialCrosses' function with the specified arguments, which must each have an Edm.Geometry result type. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether geometryValue1 crosses geometryValue2 intersect.</returns>
    /// <param name="geometryValue1">An expression that specifies the first geometry value.</param>
    /// <param name="geometryValue2">An expression that specifies the geometry value that should be compared with geometryValue1.</param>
    public static DbFunctionExpression SpatialCrosses(
      this DbExpression geometryValue1,
      DbExpression geometryValue2)
    {
      Check.NotNull<DbExpression>(geometryValue1, nameof (geometryValue1));
      Check.NotNull<DbExpression>(geometryValue2, nameof (geometryValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialCrosses), geometryValue1, geometryValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialWithin' function with the specified arguments, which must each have an Edm.Geometry result type. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether geometryValue1 is spatially within geometryValue2.</returns>
    /// <param name="geometryValue1">An expression that specifies the first geometry value.</param>
    /// <param name="geometryValue2">An expression that specifies the geometry value that should be compared with geometryValue1.</param>
    public static DbFunctionExpression SpatialWithin(
      this DbExpression geometryValue1,
      DbExpression geometryValue2)
    {
      Check.NotNull<DbExpression>(geometryValue1, nameof (geometryValue1));
      Check.NotNull<DbExpression>(geometryValue2, nameof (geometryValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialWithin), geometryValue1, geometryValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialContains' function with the specified arguments, which must each have an Edm.Geometry result type. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether geometryValue1 spatially contains geometryValue2.</returns>
    /// <param name="geometryValue1">An expression that specifies the first geometry value.</param>
    /// <param name="geometryValue2">An expression that specifies the geometry value that should be compared with geometryValue1.</param>
    public static DbFunctionExpression SpatialContains(
      this DbExpression geometryValue1,
      DbExpression geometryValue2)
    {
      Check.NotNull<DbExpression>(geometryValue1, nameof (geometryValue1));
      Check.NotNull<DbExpression>(geometryValue2, nameof (geometryValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialContains), geometryValue1, geometryValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialOverlaps' function with the specified arguments, which must each have an Edm.Geometry result type. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether geometryValue1 spatially overlaps geometryValue2.</returns>
    /// <param name="geometryValue1">An expression that specifies the first geometry value.</param>
    /// <param name="geometryValue2">An expression that specifies the geometry value that should be compared with geometryValue1.</param>
    public static DbFunctionExpression SpatialOverlaps(
      this DbExpression geometryValue1,
      DbExpression geometryValue2)
    {
      Check.NotNull<DbExpression>(geometryValue1, nameof (geometryValue1));
      Check.NotNull<DbExpression>(geometryValue2, nameof (geometryValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialOverlaps), geometryValue1, geometryValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialRelate' function with the specified arguments, which must have Edm.Geometry and string result types. The result type of the expression is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether geometryValue1 is spatially related to geometryValue2 according to the spatial relationship designated by intersectionPatternMatrix.</returns>
    /// <param name="geometryValue1">An expression that specifies the first geometry value.</param>
    /// <param name="geometryValue2">An expression that specifies the geometry value that should be compared with geometryValue1.</param>
    /// <param name="intersectionPatternMatrix">An expression that specifies the text representation of the Dimensionally Extended Nine-Intersection Model (DE-9IM) intersection pattern used to compare geometryValue1 and geometryValue2.</param>
    public static DbFunctionExpression SpatialRelate(
      this DbExpression geometryValue1,
      DbExpression geometryValue2,
      DbExpression intersectionPatternMatrix)
    {
      Check.NotNull<DbExpression>(geometryValue1, nameof (geometryValue1));
      Check.NotNull<DbExpression>(geometryValue2, nameof (geometryValue2));
      Check.NotNull<DbExpression>(intersectionPatternMatrix, nameof (intersectionPatternMatrix));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialRelate), geometryValue1, geometryValue2, intersectionPatternMatrix);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialBuffer' function with the specified arguments, which must have a Edm.Geography or Edm.Geometry and Edm.Double result types. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a geometry value representing all points less than or equal to distance from spatialValue.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial value.</param>
    /// <param name="distance">An expression that specifies the buffer distance.</param>
    public static DbFunctionExpression SpatialBuffer(
      this DbExpression spatialValue,
      DbExpression distance)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      Check.NotNull<DbExpression>(distance, nameof (distance));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialBuffer), spatialValue, distance);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Distance' function with the specified arguments, which must each have an Edm.Geography or Edm.Geometry result type.  The result type of spatialValue1 must match the result type of spatialValue2. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the distance between the closest points in spatialValue1 and spatialValue1.</returns>
    /// <param name="spatialValue1">An expression that specifies the first spatial value.</param>
    /// <param name="spatialValue2">An expression that specifies the spatial value from which the distance from spatialValue1 should be measured.</param>
    public static DbFunctionExpression Distance(
      this DbExpression spatialValue1,
      DbExpression spatialValue2)
    {
      Check.NotNull<DbExpression>(spatialValue1, nameof (spatialValue1));
      Check.NotNull<DbExpression>(spatialValue2, nameof (spatialValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Distance), spatialValue1, spatialValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialConvexHull' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the the convex hull for geometryValue.</returns>
    /// <param name="geometryValue">An expression that specifies the geometry value from which the convex hull value should be retrieved.</param>
    public static DbFunctionExpression SpatialConvexHull(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialConvexHull), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialIntersection' function with the specified arguments, which must each have an Edm.Geography or Edm.Geometry result type. The result type of spatialValue1 must match the result type of spatialValue2. The result type of the expression is the same as the type of spatialValue1 and spatialValue2.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the spatial value representing the intersection of spatialValue1 and spatialValue2.</returns>
    /// <param name="spatialValue1">An expression that specifies the first spatial value.</param>
    /// <param name="spatialValue2">An expression that specifies the spatial value for which the intersection with spatialValue1 should be computed.</param>
    public static DbFunctionExpression SpatialIntersection(
      this DbExpression spatialValue1,
      DbExpression spatialValue2)
    {
      Check.NotNull<DbExpression>(spatialValue1, nameof (spatialValue1));
      Check.NotNull<DbExpression>(spatialValue2, nameof (spatialValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialIntersection), spatialValue1, spatialValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialUnion' function with the specified arguments, which must each have an Edm.Geography or Edm.Geometry result type. The result type of spatialValue1 must match the result type of spatialValue2. The result type of the expression is the same as the type of spatialValue1 and spatialValue2.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the spatial value representing the union of spatialValue1 and spatialValue2.</returns>
    /// <param name="spatialValue1">An expression that specifies the first spatial value.</param>
    /// <param name="spatialValue2">An expression that specifies the spatial value for which the union with spatialValue1 should be computed.</param>
    public static DbFunctionExpression SpatialUnion(
      this DbExpression spatialValue1,
      DbExpression spatialValue2)
    {
      Check.NotNull<DbExpression>(spatialValue1, nameof (spatialValue1));
      Check.NotNull<DbExpression>(spatialValue2, nameof (spatialValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialUnion), spatialValue1, spatialValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialDifference' function with the specified arguments, which must each have an Edm.Geography or Edm.Geometry result type. The result type of spatialValue1 must match the result type of spatialValue2. The result type of the expression is the same as the type of spatialValue1 and spatialValue2.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the geometry value representing the difference of spatialValue2 with spatialValue1.</returns>
    /// <param name="spatialValue1">An expression that specifies the first spatial value.</param>
    /// <param name="spatialValue2">An expression that specifies the spatial value for which the difference with spatialValue1 should be computed.</param>
    public static DbFunctionExpression SpatialDifference(
      this DbExpression spatialValue1,
      DbExpression spatialValue2)
    {
      Check.NotNull<DbExpression>(spatialValue1, nameof (spatialValue1));
      Check.NotNull<DbExpression>(spatialValue2, nameof (spatialValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialDifference), spatialValue1, spatialValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialSymmetricDifference' function with the specified arguments, which must each have an Edm.Geography or Edm.Geometry result type. The result type of spatialValue1 must match the result type of spatialValue2. The result type of the expression is the same as the type of spatialValue1 and spatialValue2.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the geometry value representing the symmetric difference of spatialValue2 with spatialValue1.</returns>
    /// <param name="spatialValue1">An expression that specifies the first spatial value.</param>
    /// <param name="spatialValue2">An expression that specifies the spatial value for which the symmetric difference with spatialValue1 should be computed.</param>
    public static DbFunctionExpression SpatialSymmetricDifference(
      this DbExpression spatialValue1,
      DbExpression spatialValue2)
    {
      Check.NotNull<DbExpression>(spatialValue1, nameof (spatialValue1));
      Check.NotNull<DbExpression>(spatialValue2, nameof (spatialValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialSymmetricDifference), spatialValue1, spatialValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialElementCount' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the number of elements in spatialValue or null if spatialValue is not a collection.</returns>
    /// <param name="spatialValue">An expression that specifies the geography or geometry collection value from which the number of elements should be retrieved.</param>
    public static DbFunctionExpression SpatialElementCount(
      this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialElementCount), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialElementAt' function with the specified arguments. The first argument must have an Edm.Geography or Edm.Geometry result type. The second argument must have an integer numeric result type. The result type of the expression is the same as that of spatialValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the collection element at position indexValue in spatialValue or null if spatialValue is not a collection.</returns>
    /// <param name="spatialValue">An expression that specifies the geography or geometry collection value.</param>
    /// <param name="indexValue">An expression that specifies the position of the element to be retrieved from within the geometry or geography collection.</param>
    public static DbFunctionExpression SpatialElementAt(
      this DbExpression spatialValue,
      DbExpression indexValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      Check.NotNull<DbExpression>(indexValue, nameof (indexValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialElementAt), spatialValue, indexValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'XCoordinate' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the X co-ordinate value of geometryValue or null if geometryValue is not a point.</returns>
    /// <param name="geometryValue">An expression that specifies the geometry point value from which the X co-ordinate value should be retrieved.</param>
    public static DbFunctionExpression XCoordinate(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (XCoordinate), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'YCoordinate' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the Y co-ordinate value of geometryValue or null if geometryValue is not a point.</returns>
    /// <param name="geometryValue">An expression that specifies the geometry point value from which the Y co-ordinate value should be retrieved.</param>
    public static DbFunctionExpression YCoordinate(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (YCoordinate), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Elevation' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the elevation value of spatialValue or null if spatialValue is not a point.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial point value from which the elevation (Z co-ordinate) value should be retrieved.</param>
    public static DbFunctionExpression Elevation(this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Elevation), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Measure' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the Measure of spatialValue or null if spatialValue is not a point.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial point value from which the Measure (M) co-ordinate value should be retrieved.</param>
    public static DbFunctionExpression Measure(this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Measure), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Latitude' function with the specified argument, which must have an Edm.Geography result type. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the Latitude value of geographyValue or null if geographyValue is not a point.</returns>
    /// <param name="geographyValue">An expression that specifies the geography point value from which the Latitude value should be retrieved.</param>
    public static DbFunctionExpression Latitude(this DbExpression geographyValue)
    {
      Check.NotNull<DbExpression>(geographyValue, nameof (geographyValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Latitude), geographyValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Longitude' function with the specified argument, which must have an Edm.Geography result type. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the Longitude value of geographyValue or null if geographyValue is not a point.</returns>
    /// <param name="geographyValue">An expression that specifies the geography point value from which the Longitude value should be retrieved.</param>
    public static DbFunctionExpression Longitude(
      this DbExpression geographyValue)
    {
      Check.NotNull<DbExpression>(geographyValue, nameof (geographyValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Longitude), geographyValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'SpatialLength' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the length of spatialValue or null if spatialValue is not a curve.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial curve value from which the length should be retrieved.</param>
    public static DbFunctionExpression SpatialLength(
      this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (SpatialLength), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'StartPoint' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type is the same as that of spatialValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the start point of spatialValue or null if spatialValue is not a curve.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial curve value from which the start point should be retrieved.</param>
    public static DbFunctionExpression StartPoint(this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (StartPoint), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'EndPoint' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type is the same as that of spatialValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the end point of spatialValue or null if spatialValue is not a curve.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial curve value from which the end point should be retrieved.</param>
    public static DbFunctionExpression EndPoint(this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (EndPoint), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'IsClosedSpatial' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either a Boolean value indicating whether spatialValue is closed, or null if spatialValue is not a curve.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial curve value from which the IsClosedSpatial value should be retrieved.</param>
    public static DbFunctionExpression IsClosedSpatial(
      this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (IsClosedSpatial), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'IsRing' function with the specified argument, which must have an Edm.Geometry result type. The result type is Edm.Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either a Boolean value indicating whether geometryValue is a ring (both closed and simple), or null if geometryValue is not a curve.</returns>
    /// <param name="geometryValue">An expression that specifies the geometry curve value from which the IsRing value should be retrieved.</param>
    public static DbFunctionExpression IsRing(this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (IsRing), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'PointCount' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the number of points in spatialValue or null if spatialValue is not a line string.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial line string value from which the number of points should be retrieved.</param>
    public static DbFunctionExpression PointCount(this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (PointCount), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'PointAt' function with the specified arguments. The first argument must have an Edm.Geography or Edm.Geometry result type. The second argument must have an integer numeric result type. The result type of the expression is the same as that of spatialValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the point at position indexValue in spatialValue or null if spatialValue is not a line string.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial line string value.</param>
    /// <param name="indexValue">An expression that specifies the position of the point to be retrieved from within the line string.</param>
    public static DbFunctionExpression PointAt(
      this DbExpression spatialValue,
      DbExpression indexValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      Check.NotNull<DbExpression>(indexValue, nameof (indexValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (PointAt), spatialValue, indexValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Area' function with the specified argument, which must have an Edm.Geography or Edm.Geometry result type. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the area of spatialValue or null if spatialValue is not a surface.</returns>
    /// <param name="spatialValue">An expression that specifies the spatial surface value for which the area should be calculated.</param>
    public static DbFunctionExpression Area(this DbExpression spatialValue)
    {
      Check.NotNull<DbExpression>(spatialValue, nameof (spatialValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Area), spatialValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Centroid' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the centroid point of geometryValue (which may not be on the surface itself) or null if geometryValue is not a surface.</returns>
    /// <param name="geometryValue">An expression that specifies the geometry surface value from which the centroid should be retrieved.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Standard bame", MessageId = "Centroid")]
    public static DbFunctionExpression Centroid(this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Centroid), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'PointOnSurface' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either a point guaranteed to be on the surface geometryValue or null if geometryValue is not a surface.</returns>
    /// <param name="geometryValue">An expression that specifies the geometry surface value from which the point should be retrieved.</param>
    public static DbFunctionExpression PointOnSurface(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (PointOnSurface), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'ExteriorRing' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the exterior ring of the polygon geometryValue or null if geometryValue is not a polygon.</returns>
    /// <param name="geometryValue">The geometry value.</param>
    public static DbFunctionExpression ExteriorRing(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (ExteriorRing), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'InteriorRingCount' function with the specified argument, which must have an Edm.Geometry result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the number of interior rings in the polygon geometryValue or null if geometryValue is not a polygon.</returns>
    /// <param name="geometryValue">The geometry value.</param>
    public static DbFunctionExpression InteriorRingCount(
      this DbExpression geometryValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (InteriorRingCount), geometryValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'InteriorRingAt' function with the specified arguments. The first argument must have an Edm.Geometry result type. The second argument must have an integer numeric result types. The result type of the expression is Edm.Geometry.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns either the interior ring at position indexValue in geometryValue or null if geometryValue is not a polygon.</returns>
    /// <param name="geometryValue">The geometry value.</param>
    /// <param name="indexValue">An expression that specifies the position of the interior ring to be retrieved from within the polygon.</param>
    public static DbFunctionExpression InteriorRingAt(
      this DbExpression geometryValue,
      DbExpression indexValue)
    {
      Check.NotNull<DbExpression>(geometryValue, nameof (geometryValue));
      Check.NotNull<DbExpression>(indexValue, nameof (indexValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (InteriorRingAt), geometryValue, indexValue);
    }
  }
}
