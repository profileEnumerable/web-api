// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.PrimitiveTypeKind
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Primitive Types as defined by EDM</summary>
  public enum PrimitiveTypeKind
  {
    /// <summary>Binary Type Kind</summary>
    Binary,
    /// <summary>Boolean Type Kind</summary>
    Boolean,
    /// <summary>Byte Type Kind</summary>
    Byte,
    /// <summary>DateTime Type Kind</summary>
    DateTime,
    /// <summary>Decimal Type Kind</summary>
    Decimal,
    /// <summary>Double Type Kind</summary>
    Double,
    /// <summary>Guid Type Kind</summary>
    Guid,
    /// <summary>Single Type Kind</summary>
    Single,
    /// <summary>SByte Type Kind</summary>
    SByte,
    /// <summary>Int16 Type Kind</summary>
    Int16,
    /// <summary>Int32 Type Kind</summary>
    Int32,
    /// <summary>Int64 Type Kind</summary>
    Int64,
    /// <summary>String Type Kind</summary>
    String,
    /// <summary>Time Type Kind</summary>
    Time,
    /// <summary>DateTimeOffset Type Kind</summary>
    DateTimeOffset,
    /// <summary>Geometry Type Kind</summary>
    Geometry,
    /// <summary>Geography Type Kind</summary>
    Geography,
    /// <summary>Geometric point type kind</summary>
    GeometryPoint,
    /// <summary>Geometric linestring type kind</summary>
    GeometryLineString,
    /// <summary>Geometric polygon type kind</summary>
    GeometryPolygon,
    /// <summary>Geometric multi-point type kind</summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi"), SuppressMessage("Microsoft.Naming", "CA1702", MessageId = "MultiPoint")] GeometryMultiPoint,
    /// <summary>Geometric multi-linestring type kind</summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi"), SuppressMessage("Microsoft.Naming", "CA1702", MessageId = "MultiLine")] GeometryMultiLineString,
    /// <summary>Geometric multi-polygon type kind</summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")] GeometryMultiPolygon,
    /// <summary>Geometric collection type kind</summary>
    GeometryCollection,
    /// <summary>Geographic point type kind</summary>
    GeographyPoint,
    /// <summary>Geographic linestring type kind</summary>
    GeographyLineString,
    /// <summary>Geographic polygon type kind</summary>
    GeographyPolygon,
    /// <summary>Geographic multi-point type kind</summary>
    [SuppressMessage("Microsoft.Naming", "CA1702", MessageId = "MultiPoint"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")] GeographyMultiPoint,
    /// <summary>Geographic multi-linestring type kind</summary>
    [SuppressMessage("Microsoft.Naming", "CA1702", MessageId = "MultiLine"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")] GeographyMultiLineString,
    /// <summary>Geographic multi-polygon type kind</summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")] GeographyMultiPolygon,
    /// <summary>Geographic collection type kind</summary>
    GeographyCollection,
  }
}
