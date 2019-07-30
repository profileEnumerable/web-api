// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.DbTypeMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;

namespace System.Data.Entity.Core.Common.Internal
{
  internal static class DbTypeMap
  {
    internal static readonly TypeUsage AnsiString = DbTypeMap.CreateType(PrimitiveTypeKind.String, new FacetValues()
    {
      Unicode = (FacetValueContainer<bool?>) new bool?(false),
      FixedLength = (FacetValueContainer<bool?>) new bool?(false),
      MaxLength = (FacetValueContainer<int?>) new int?()
    });
    internal static readonly TypeUsage AnsiStringFixedLength = DbTypeMap.CreateType(PrimitiveTypeKind.String, new FacetValues()
    {
      Unicode = (FacetValueContainer<bool?>) new bool?(false),
      FixedLength = (FacetValueContainer<bool?>) new bool?(true),
      MaxLength = (FacetValueContainer<int?>) new int?()
    });
    internal static readonly TypeUsage String = DbTypeMap.CreateType(PrimitiveTypeKind.String, new FacetValues()
    {
      Unicode = (FacetValueContainer<bool?>) new bool?(true),
      FixedLength = (FacetValueContainer<bool?>) new bool?(false),
      MaxLength = (FacetValueContainer<int?>) new int?()
    });
    internal static readonly TypeUsage StringFixedLength = DbTypeMap.CreateType(PrimitiveTypeKind.String, new FacetValues()
    {
      Unicode = (FacetValueContainer<bool?>) new bool?(true),
      FixedLength = (FacetValueContainer<bool?>) new bool?(true),
      MaxLength = (FacetValueContainer<int?>) new int?()
    });
    internal static readonly TypeUsage Xml = DbTypeMap.CreateType(PrimitiveTypeKind.String, new FacetValues()
    {
      Unicode = (FacetValueContainer<bool?>) new bool?(true),
      FixedLength = (FacetValueContainer<bool?>) new bool?(false),
      MaxLength = (FacetValueContainer<int?>) new int?()
    });
    internal static readonly TypeUsage Binary = DbTypeMap.CreateType(PrimitiveTypeKind.Binary, new FacetValues()
    {
      MaxLength = (FacetValueContainer<int?>) new int?()
    });
    internal static readonly TypeUsage Boolean = DbTypeMap.CreateType(PrimitiveTypeKind.Boolean);
    internal static readonly TypeUsage Byte = DbTypeMap.CreateType(PrimitiveTypeKind.Byte);
    internal static readonly TypeUsage DateTime = DbTypeMap.CreateType(PrimitiveTypeKind.DateTime);
    internal static readonly TypeUsage Date = DbTypeMap.CreateType(PrimitiveTypeKind.DateTime);
    internal static readonly TypeUsage DateTime2 = DbTypeMap.CreateType(PrimitiveTypeKind.DateTime, new FacetValues()
    {
      Precision = (FacetValueContainer<byte?>) new byte?()
    });
    internal static readonly TypeUsage Time = DbTypeMap.CreateType(PrimitiveTypeKind.Time, new FacetValues()
    {
      Precision = (FacetValueContainer<byte?>) new byte?()
    });
    internal static readonly TypeUsage DateTimeOffset = DbTypeMap.CreateType(PrimitiveTypeKind.DateTimeOffset, new FacetValues()
    {
      Precision = (FacetValueContainer<byte?>) new byte?()
    });
    internal static readonly TypeUsage Decimal = DbTypeMap.CreateType(PrimitiveTypeKind.Decimal, new FacetValues()
    {
      Precision = (FacetValueContainer<byte?>) new byte?(),
      Scale = (FacetValueContainer<byte?>) new byte?()
    });
    internal static readonly TypeUsage Currency = DbTypeMap.CreateType(PrimitiveTypeKind.Decimal, new FacetValues()
    {
      Precision = (FacetValueContainer<byte?>) new byte?(),
      Scale = (FacetValueContainer<byte?>) new byte?()
    });
    internal static readonly TypeUsage Double = DbTypeMap.CreateType(PrimitiveTypeKind.Double);
    internal static readonly TypeUsage Guid = DbTypeMap.CreateType(PrimitiveTypeKind.Guid);
    internal static readonly TypeUsage Int16 = DbTypeMap.CreateType(PrimitiveTypeKind.Int16);
    internal static readonly TypeUsage Int32 = DbTypeMap.CreateType(PrimitiveTypeKind.Int32);
    internal static readonly TypeUsage Int64 = DbTypeMap.CreateType(PrimitiveTypeKind.Int64);
    internal static readonly TypeUsage Single = DbTypeMap.CreateType(PrimitiveTypeKind.Single);
    internal static readonly TypeUsage SByte = DbTypeMap.CreateType(PrimitiveTypeKind.SByte);

    internal static bool TryGetModelTypeUsage(DbType dbType, out TypeUsage modelType)
    {
      switch (dbType)
      {
        case DbType.AnsiString:
          modelType = DbTypeMap.AnsiString;
          break;
        case DbType.Binary:
          modelType = DbTypeMap.Binary;
          break;
        case DbType.Byte:
          modelType = DbTypeMap.Byte;
          break;
        case DbType.Boolean:
          modelType = DbTypeMap.Boolean;
          break;
        case DbType.Currency:
          modelType = DbTypeMap.Currency;
          break;
        case DbType.Date:
          modelType = DbTypeMap.Date;
          break;
        case DbType.DateTime:
          modelType = DbTypeMap.DateTime;
          break;
        case DbType.Decimal:
          modelType = DbTypeMap.Decimal;
          break;
        case DbType.Double:
          modelType = DbTypeMap.Double;
          break;
        case DbType.Guid:
          modelType = DbTypeMap.Guid;
          break;
        case DbType.Int16:
          modelType = DbTypeMap.Int16;
          break;
        case DbType.Int32:
          modelType = DbTypeMap.Int32;
          break;
        case DbType.Int64:
          modelType = DbTypeMap.Int64;
          break;
        case DbType.SByte:
          modelType = DbTypeMap.SByte;
          break;
        case DbType.Single:
          modelType = DbTypeMap.Single;
          break;
        case DbType.String:
          modelType = DbTypeMap.String;
          break;
        case DbType.Time:
          modelType = DbTypeMap.Time;
          break;
        case DbType.VarNumeric:
          modelType = (TypeUsage) null;
          break;
        case DbType.AnsiStringFixedLength:
          modelType = DbTypeMap.AnsiStringFixedLength;
          break;
        case DbType.StringFixedLength:
          modelType = DbTypeMap.StringFixedLength;
          break;
        case DbType.Xml:
          modelType = DbTypeMap.Xml;
          break;
        case DbType.DateTime2:
          modelType = DbTypeMap.DateTime2;
          break;
        case DbType.DateTimeOffset:
          modelType = DbTypeMap.DateTimeOffset;
          break;
        default:
          modelType = (TypeUsage) null;
          break;
      }
      return modelType != null;
    }

    private static TypeUsage CreateType(PrimitiveTypeKind type)
    {
      return DbTypeMap.CreateType(type, new FacetValues());
    }

    private static TypeUsage CreateType(PrimitiveTypeKind type, FacetValues facets)
    {
      return TypeUsage.Create((EdmType) EdmProviderManifest.Instance.GetPrimitiveType(type), facets);
    }
  }
}
