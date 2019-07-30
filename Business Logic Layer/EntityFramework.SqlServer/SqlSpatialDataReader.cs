// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlSpatialDataReader
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer.Resources;
using System.Data.Entity.SqlServer.Utilities;
using System.Data.SqlTypes;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.SqlServer
{
  internal sealed class SqlSpatialDataReader : DbSpatialDataReader
  {
    private static readonly Lazy<Func<BinaryReader, object>> _sqlGeographyFromBinaryReader = new Lazy<Func<BinaryReader, object>>((Func<Func<BinaryReader, object>>) (() => SqlSpatialDataReader.CreateBinaryReadDelegate(SqlTypesAssemblyLoader.DefaultInstance.GetSqlTypesAssembly().SqlGeographyType)), true);
    private static readonly Lazy<Func<BinaryReader, object>> _sqlGeometryFromBinaryReader = new Lazy<Func<BinaryReader, object>>((Func<Func<BinaryReader, object>>) (() => SqlSpatialDataReader.CreateBinaryReadDelegate(SqlTypesAssemblyLoader.DefaultInstance.GetSqlTypesAssembly().SqlGeometryType)), true);
    private const string GeometrySqlType = "sys.geometry";
    private const string GeographySqlType = "sys.geography";
    private readonly DbSpatialServices _spatialServices;
    private readonly SqlDataReaderWrapper _reader;
    private readonly bool[] _geographyColumns;
    private readonly bool[] _geometryColumns;

    internal SqlSpatialDataReader(
      DbSpatialServices spatialServices,
      SqlDataReaderWrapper underlyingReader)
    {
      this._spatialServices = spatialServices;
      this._reader = underlyingReader;
      int fieldCount = this._reader.FieldCount;
      this._geographyColumns = new bool[fieldCount];
      this._geometryColumns = new bool[fieldCount];
      for (int i = 0; i < this._reader.FieldCount; ++i)
      {
        string dataTypeName = this._reader.GetDataTypeName(i);
        if (dataTypeName.EndsWith("sys.geography", StringComparison.Ordinal))
          this._geographyColumns[i] = true;
        else if (dataTypeName.EndsWith("sys.geometry", StringComparison.Ordinal))
          this._geometryColumns[i] = true;
      }
    }

    public override DbGeography GetGeography(int ordinal)
    {
      this.EnsureGeographyColumn(ordinal);
      SqlBytes sqlBytes = this._reader.GetSqlBytes(ordinal);
      return this._spatialServices.GeographyFromProviderValue(SqlSpatialDataReader._sqlGeographyFromBinaryReader.Value(new BinaryReader(sqlBytes.Stream)));
    }

    public override DbGeometry GetGeometry(int ordinal)
    {
      this.EnsureGeometryColumn(ordinal);
      SqlBytes sqlBytes = this._reader.GetSqlBytes(ordinal);
      return this._spatialServices.GeometryFromProviderValue(SqlSpatialDataReader._sqlGeometryFromBinaryReader.Value(new BinaryReader(sqlBytes.Stream)));
    }

    public override bool IsGeographyColumn(int ordinal)
    {
      return this._geographyColumns[ordinal];
    }

    public override bool IsGeometryColumn(int ordinal)
    {
      return this._geometryColumns[ordinal];
    }

    private void EnsureGeographyColumn(int ordinal)
    {
      if (!this.IsGeographyColumn(ordinal))
        throw new InvalidDataException(Strings.SqlProvider_InvalidGeographyColumn((object) this._reader.GetDataTypeName(ordinal)));
    }

    private void EnsureGeometryColumn(int ordinal)
    {
      if (!this.IsGeometryColumn(ordinal))
        throw new InvalidDataException(Strings.SqlProvider_InvalidGeometryColumn((object) this._reader.GetDataTypeName(ordinal)));
    }

    private static Func<BinaryReader, object> CreateBinaryReadDelegate(
      Type spatialType)
    {
      ParameterExpression parameterExpression1 = Expression.Parameter(typeof (BinaryReader));
      ParameterExpression parameterExpression2 = Expression.Variable(spatialType);
      MethodInfo publicInstanceMethod = spatialType.GetPublicInstanceMethod("Read", typeof (BinaryReader));
      return ((Expression<Func<BinaryReader, object>>) (parameterExpression => Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        parameterExpression2
      }, (Expression) Expression.Assign((Expression) parameterExpression2, (Expression) Expression.New(spatialType)), (Expression) Expression.Call((Expression) parameterExpression2, publicInstanceMethod, (Expression) parameterExpression1), (Expression) parameterExpression2))).Compile();
    }
  }
}
