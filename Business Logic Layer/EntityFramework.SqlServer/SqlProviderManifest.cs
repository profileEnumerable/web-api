// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlProviderManifest
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.SqlServer.Resources;
using System.Data.Entity.SqlServer.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;

namespace System.Data.Entity.SqlServer
{
  internal class SqlProviderManifest : DbXmlEnabledProviderManifest
  {
    private readonly SqlVersion _version = SqlVersion.Sql9;
    internal const string TokenSql8 = "2000";
    internal const string TokenSql9 = "2005";
    internal const string TokenSql10 = "2008";
    internal const string TokenSql11 = "2012";
    internal const string TokenAzure11 = "2012.Azure";
    internal const char LikeEscapeChar = '~';
    internal const string LikeEscapeCharToString = "~";
    private const int varcharMaxSize = 8000;
    private const int nvarcharMaxSize = 4000;
    private const int binaryMaxSize = 8000;
    private ReadOnlyCollection<PrimitiveType> _primitiveTypes;
    private ReadOnlyCollection<EdmFunction> _functions;

    public SqlProviderManifest(string manifestToken)
      : base(SqlProviderManifest.GetProviderManifest())
    {
      this._version = SqlVersionUtils.GetSqlVersion(manifestToken);
      this.Initialize();
    }

    private void Initialize()
    {
      if (this._version == SqlVersion.Sql10 || this._version == SqlVersion.Sql11)
      {
        this._primitiveTypes = base.GetStoreTypes();
        this._functions = base.GetStoreFunctions();
      }
      else
      {
        List<PrimitiveType> primitiveTypeList = new List<PrimitiveType>((IEnumerable<PrimitiveType>) base.GetStoreTypes());
        primitiveTypeList.RemoveAll((Predicate<PrimitiveType>) (primitiveType =>
        {
          if (!primitiveType.Name.Equals("time", StringComparison.OrdinalIgnoreCase) && !primitiveType.Name.Equals("date", StringComparison.OrdinalIgnoreCase) && (!primitiveType.Name.Equals("datetime2", StringComparison.OrdinalIgnoreCase) && !primitiveType.Name.Equals("datetimeoffset", StringComparison.OrdinalIgnoreCase)) && !primitiveType.Name.Equals("geography", StringComparison.OrdinalIgnoreCase))
            return primitiveType.Name.Equals("geometry", StringComparison.OrdinalIgnoreCase);
          return true;
        }));
        if (this._version == SqlVersion.Sql8)
          primitiveTypeList.RemoveAll((Predicate<PrimitiveType>) (primitiveType =>
          {
            if (!primitiveType.Name.Equals("xml", StringComparison.OrdinalIgnoreCase))
              return primitiveType.Name.EndsWith("(max)", StringComparison.OrdinalIgnoreCase);
            return true;
          }));
        this._primitiveTypes = new ReadOnlyCollection<PrimitiveType>((IList<PrimitiveType>) primitiveTypeList);
        IEnumerable<EdmFunction> source = base.GetStoreFunctions().Where<EdmFunction>((Func<EdmFunction, bool>) (f => !SqlProviderManifest.IsKatmaiOrNewer(f)));
        if (this._version == SqlVersion.Sql8)
          source = source.Where<EdmFunction>((Func<EdmFunction, bool>) (f => !SqlProviderManifest.IsYukonOrNewer(f)));
        this._functions = new ReadOnlyCollection<EdmFunction>((IList<EdmFunction>) source.ToList<EdmFunction>());
      }
    }

    internal SqlVersion SqlVersion
    {
      get
      {
        return this._version;
      }
    }

    private static XmlReader GetXmlResource(string resourceName)
    {
      return XmlReader.Create(typeof (SqlProviderManifest).Assembly().GetManifestResourceStream(resourceName));
    }

    internal static XmlReader GetProviderManifest()
    {
      return SqlProviderManifest.GetXmlResource("System.Data.Resources.SqlClient.SqlProviderServices.ProviderManifest.xml");
    }

    internal static XmlReader GetStoreSchemaMapping(string mslName)
    {
      return SqlProviderManifest.GetXmlResource("System.Data.Resources.SqlClient.SqlProviderServices." + mslName + ".msl");
    }

    internal XmlReader GetStoreSchemaDescription(string ssdlName)
    {
      if (this._version == SqlVersion.Sql8)
        return SqlProviderManifest.GetXmlResource("System.Data.Resources.SqlClient.SqlProviderServices." + ssdlName + "_Sql8.ssdl");
      return SqlProviderManifest.GetXmlResource("System.Data.Resources.SqlClient.SqlProviderServices." + ssdlName + ".ssdl");
    }

    internal static string EscapeLikeText(
      string text,
      bool alwaysEscapeEscapeChar,
      out bool usedEscapeChar)
    {
      usedEscapeChar = false;
      if (!text.Contains("%") && !text.Contains("_") && (!text.Contains("[") && !text.Contains("^")) && (!alwaysEscapeEscapeChar || !text.Contains("~")))
        return text;
      StringBuilder stringBuilder = new StringBuilder(text.Length);
      foreach (char ch in text)
      {
        switch (ch)
        {
          case '%':
          case '[':
          case '^':
          case '_':
          case '~':
            stringBuilder.Append('~');
            usedEscapeChar = true;
            break;
        }
        stringBuilder.Append(ch);
      }
      return stringBuilder.ToString();
    }

    protected override XmlReader GetDbInformation(string informationType)
    {
      if (informationType == "StoreSchemaDefinitionVersion3" || informationType == "StoreSchemaDefinition")
        return this.GetStoreSchemaDescription(informationType);
      if (informationType == "StoreSchemaMappingVersion3" || informationType == "StoreSchemaMapping")
        return SqlProviderManifest.GetStoreSchemaMapping(informationType);
      if (informationType == "ConceptualSchemaDefinitionVersion3" || informationType == "ConceptualSchemaDefinition")
        return (XmlReader) null;
      throw new ProviderIncompatibleException(Strings.ProviderReturnedNullForGetDbInformation((object) informationType));
    }

    public override ReadOnlyCollection<PrimitiveType> GetStoreTypes()
    {
      return this._primitiveTypes;
    }

    public override ReadOnlyCollection<EdmFunction> GetStoreFunctions()
    {
      return this._functions;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static bool IsKatmaiOrNewer(EdmFunction edmFunction)
    {
      if (edmFunction.ReturnParameter != null && edmFunction.ReturnParameter.TypeUsage.IsSpatialType() || edmFunction.Parameters.Any<FunctionParameter>((Func<FunctionParameter, bool>) (p => p.TypeUsage.IsSpatialType())))
        return true;
      ReadOnlyMetadataCollection<FunctionParameter> parameters = edmFunction.Parameters;
      switch (edmFunction.Name.ToUpperInvariant())
      {
        case "COUNT":
        case "COUNT_BIG":
        case "MAX":
        case "MIN":
          string name1 = ((CollectionType) parameters[0].TypeUsage.EdmType).TypeUsage.EdmType.Name;
          if (!name1.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase))
            return name1.Equals("Time", StringComparison.OrdinalIgnoreCase);
          return true;
        case "DAY":
        case "MONTH":
        case "YEAR":
        case "DATALENGTH":
        case "CHECKSUM":
          string name2 = parameters[0].TypeUsage.EdmType.Name;
          if (!name2.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase))
            return name2.Equals("Time", StringComparison.OrdinalIgnoreCase);
          return true;
        case "DATEADD":
        case "DATEDIFF":
          string name3 = parameters[1].TypeUsage.EdmType.Name;
          string name4 = parameters[2].TypeUsage.EdmType.Name;
          if (!name3.Equals("Time", StringComparison.OrdinalIgnoreCase) && !name4.Equals("Time", StringComparison.OrdinalIgnoreCase) && !name3.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase))
            return name4.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase);
          return true;
        case "DATENAME":
        case "DATEPART":
          string name5 = parameters[1].TypeUsage.EdmType.Name;
          if (!name5.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase))
            return name5.Equals("Time", StringComparison.OrdinalIgnoreCase);
          return true;
        case "SYSUTCDATETIME":
        case "SYSDATETIME":
        case "SYSDATETIMEOFFSET":
          return true;
        default:
          return false;
      }
    }

    private static bool IsYukonOrNewer(EdmFunction edmFunction)
    {
      ReadOnlyMetadataCollection<FunctionParameter> parameters = edmFunction.Parameters;
      if (parameters == null || parameters.Count == 0)
        return false;
      switch (edmFunction.Name.ToUpperInvariant())
      {
        case "COUNT":
        case "COUNT_BIG":
          return ((CollectionType) parameters[0].TypeUsage.EdmType).TypeUsage.EdmType.Name.Equals("Guid", StringComparison.OrdinalIgnoreCase);
        case "CHARINDEX":
          using (ReadOnlyMetadataCollection<FunctionParameter>.Enumerator enumerator = parameters.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              if (enumerator.Current.TypeUsage.EdmType.Name.Equals("Int64", StringComparison.OrdinalIgnoreCase))
                return true;
            }
            break;
          }
      }
      return false;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public override TypeUsage GetEdmType(TypeUsage storeType)
    {
      Check.NotNull<TypeUsage>(storeType, nameof (storeType));
      string lowerInvariant = storeType.EdmType.Name.ToLowerInvariant();
      if (!this.StoreTypeNameToEdmPrimitiveType.ContainsKey(lowerInvariant))
        throw new ArgumentException(Strings.ProviderDoesNotSupportType((object) lowerInvariant));
      PrimitiveType primitiveType = this.StoreTypeNameToEdmPrimitiveType[lowerInvariant];
      int maxLength = 0;
      bool isUnicode = true;
      PrimitiveTypeKind primitiveTypeKind;
      bool flag;
      bool isFixedLength;
      switch (lowerInvariant)
      {
        case "tinyint":
        case "smallint":
        case "bigint":
        case "bit":
        case "uniqueidentifier":
        case "int":
        case "geography":
        case "geometry":
          return TypeUsage.CreateDefaultTypeUsage((EdmType) primitiveType);
        case "varchar":
          primitiveTypeKind = PrimitiveTypeKind.String;
          flag = !storeType.TryGetMaxLength(out maxLength);
          isUnicode = false;
          isFixedLength = false;
          break;
        case "char":
          primitiveTypeKind = PrimitiveTypeKind.String;
          flag = !storeType.TryGetMaxLength(out maxLength);
          isUnicode = false;
          isFixedLength = true;
          break;
        case "nvarchar":
          primitiveTypeKind = PrimitiveTypeKind.String;
          flag = !storeType.TryGetMaxLength(out maxLength);
          isUnicode = true;
          isFixedLength = false;
          break;
        case "nchar":
          primitiveTypeKind = PrimitiveTypeKind.String;
          flag = !storeType.TryGetMaxLength(out maxLength);
          isUnicode = true;
          isFixedLength = true;
          break;
        case "varchar(max)":
        case "text":
          primitiveTypeKind = PrimitiveTypeKind.String;
          flag = true;
          isUnicode = false;
          isFixedLength = false;
          break;
        case "nvarchar(max)":
        case "ntext":
        case "xml":
          primitiveTypeKind = PrimitiveTypeKind.String;
          flag = true;
          isUnicode = true;
          isFixedLength = false;
          break;
        case "binary":
          primitiveTypeKind = PrimitiveTypeKind.Binary;
          flag = !storeType.TryGetMaxLength(out maxLength);
          isFixedLength = true;
          break;
        case "varbinary":
          primitiveTypeKind = PrimitiveTypeKind.Binary;
          flag = !storeType.TryGetMaxLength(out maxLength);
          isFixedLength = false;
          break;
        case "varbinary(max)":
        case "image":
          primitiveTypeKind = PrimitiveTypeKind.Binary;
          flag = true;
          isFixedLength = false;
          break;
        case "timestamp":
        case "rowversion":
          return TypeUsage.CreateBinaryTypeUsage(primitiveType, true, 8);
        case "float":
        case "real":
          return TypeUsage.CreateDefaultTypeUsage((EdmType) primitiveType);
        case "decimal":
        case "numeric":
          byte precision;
          byte scale;
          if (storeType.TryGetPrecision(out precision) && storeType.TryGetScale(out scale))
            return TypeUsage.CreateDecimalTypeUsage(primitiveType, precision, scale);
          return TypeUsage.CreateDecimalTypeUsage(primitiveType);
        case "money":
          return TypeUsage.CreateDecimalTypeUsage(primitiveType, (byte) 19, (byte) 4);
        case "smallmoney":
          return TypeUsage.CreateDecimalTypeUsage(primitiveType, (byte) 10, (byte) 4);
        case "datetime":
        case "datetime2":
        case "smalldatetime":
          return TypeUsage.CreateDateTimeTypeUsage(primitiveType, new byte?());
        case "date":
          return TypeUsage.CreateDefaultTypeUsage((EdmType) primitiveType);
        case "time":
          return TypeUsage.CreateTimeTypeUsage(primitiveType, new byte?());
        case "datetimeoffset":
          return TypeUsage.CreateDateTimeOffsetTypeUsage(primitiveType, new byte?());
        default:
          throw new NotSupportedException(Strings.ProviderDoesNotSupportType((object) lowerInvariant));
      }
      switch (primitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          if (!flag)
            return TypeUsage.CreateBinaryTypeUsage(primitiveType, isFixedLength, maxLength);
          return TypeUsage.CreateBinaryTypeUsage(primitiveType, isFixedLength);
        case PrimitiveTypeKind.String:
          if (!flag)
            return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, isFixedLength, maxLength);
          return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, isFixedLength);
        default:
          throw new NotSupportedException(Strings.ProviderDoesNotSupportType((object) lowerInvariant));
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    public override TypeUsage GetStoreType(TypeUsage edmType)
    {
      Check.NotNull<TypeUsage>(edmType, nameof (edmType));
      PrimitiveType edmType1 = edmType.EdmType as PrimitiveType;
      if (edmType1 == null)
        throw new ArgumentException(Strings.ProviderDoesNotSupportType((object) edmType.EdmType.Name));
      ReadOnlyMetadataCollection<Facet> facets = edmType.Facets;
      switch (edmType1.PrimitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          bool flag1 = facets["FixedLength"].Value != null && (bool) facets["FixedLength"].Value;
          Facet facet1 = facets["MaxLength"];
          bool flag2 = facet1.IsUnbounded || facet1.Value == null || (int) facet1.Value > 8000;
          int maxLength1 = !flag2 ? (int) facet1.Value : int.MinValue;
          return !flag1 ? (!flag2 ? TypeUsage.CreateBinaryTypeUsage(this.StoreTypeNameToStorePrimitiveType["varbinary"], false, maxLength1) : (this._version == SqlVersion.Sql8 ? TypeUsage.CreateBinaryTypeUsage(this.StoreTypeNameToStorePrimitiveType["varbinary"], false, 8000) : TypeUsage.CreateBinaryTypeUsage(this.StoreTypeNameToStorePrimitiveType["varbinary(max)"], false))) : TypeUsage.CreateBinaryTypeUsage(this.StoreTypeNameToStorePrimitiveType["binary"], true, flag2 ? 8000 : maxLength1);
        case PrimitiveTypeKind.Boolean:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["bit"]);
        case PrimitiveTypeKind.Byte:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["tinyint"]);
        case PrimitiveTypeKind.DateTime:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["datetime"]);
        case PrimitiveTypeKind.Decimal:
          byte precision;
          if (!edmType.TryGetPrecision(out precision))
            precision = (byte) 18;
          byte scale;
          if (!edmType.TryGetScale(out scale))
            scale = (byte) 0;
          return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["decimal"], precision, scale);
        case PrimitiveTypeKind.Double:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["float"]);
        case PrimitiveTypeKind.Guid:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["uniqueidentifier"]);
        case PrimitiveTypeKind.Single:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["real"]);
        case PrimitiveTypeKind.Int16:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["smallint"]);
        case PrimitiveTypeKind.Int32:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["int"]);
        case PrimitiveTypeKind.Int64:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["bigint"]);
        case PrimitiveTypeKind.String:
          bool flag3 = facets["Unicode"].Value == null || (bool) facets["Unicode"].Value;
          bool flag4 = facets["FixedLength"].Value != null && (bool) facets["FixedLength"].Value;
          Facet facet2 = facets["MaxLength"];
          bool flag5 = facet2.IsUnbounded || facet2.Value == null || (int) facet2.Value > (flag3 ? 4000 : 8000);
          int maxLength2 = !flag5 ? (int) facet2.Value : int.MinValue;
          return !flag3 ? (!flag4 ? (!flag5 ? TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["varchar"], false, false, maxLength2) : (this._version == SqlVersion.Sql8 ? TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["varchar"], false, false, 8000) : TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["varchar(max)"], false, false))) : TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["char"], false, true, flag5 ? 8000 : maxLength2)) : (!flag4 ? (!flag5 ? TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["nvarchar"], true, false, maxLength2) : (this._version == SqlVersion.Sql8 ? TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["nvarchar"], true, false, 4000) : TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["nvarchar(max)"], true, false))) : TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["nchar"], true, true, flag5 ? 4000 : maxLength2));
        case PrimitiveTypeKind.Time:
          return this.GetStorePrimitiveTypeIfPostSql9("time", edmType.EdmType.Name, edmType1.PrimitiveTypeKind);
        case PrimitiveTypeKind.DateTimeOffset:
          return this.GetStorePrimitiveTypeIfPostSql9("datetimeoffset", edmType.EdmType.Name, edmType1.PrimitiveTypeKind);
        case PrimitiveTypeKind.Geometry:
        case PrimitiveTypeKind.GeometryPoint:
        case PrimitiveTypeKind.GeometryLineString:
        case PrimitiveTypeKind.GeometryPolygon:
        case PrimitiveTypeKind.GeometryMultiPoint:
        case PrimitiveTypeKind.GeometryMultiLineString:
        case PrimitiveTypeKind.GeometryMultiPolygon:
        case PrimitiveTypeKind.GeometryCollection:
          return this.GetStorePrimitiveTypeIfPostSql9("geometry", edmType.EdmType.Name, edmType1.PrimitiveTypeKind);
        case PrimitiveTypeKind.Geography:
        case PrimitiveTypeKind.GeographyPoint:
        case PrimitiveTypeKind.GeographyLineString:
        case PrimitiveTypeKind.GeographyPolygon:
        case PrimitiveTypeKind.GeographyMultiPoint:
        case PrimitiveTypeKind.GeographyMultiLineString:
        case PrimitiveTypeKind.GeographyMultiPolygon:
        case PrimitiveTypeKind.GeographyCollection:
          return this.GetStorePrimitiveTypeIfPostSql9("geography", edmType.EdmType.Name, edmType1.PrimitiveTypeKind);
        default:
          throw new NotSupportedException(Strings.NoStoreTypeForEdmType((object) edmType.EdmType.Name, (object) edmType1.PrimitiveTypeKind));
      }
    }

    private TypeUsage GetStorePrimitiveTypeIfPostSql9(
      string storeTypeName,
      string nameForException,
      PrimitiveTypeKind primitiveTypeKind)
    {
      if (this.SqlVersion != SqlVersion.Sql8 && this.SqlVersion != SqlVersion.Sql9)
        return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType[storeTypeName]);
      throw new NotSupportedException(Strings.NoStoreTypeForEdmType((object) nameForException, (object) primitiveTypeKind));
    }

    public override bool SupportsEscapingLikeArgument(out char escapeCharacter)
    {
      escapeCharacter = '~';
      return true;
    }

    public override string EscapeLikeArgument(string argument)
    {
      Check.NotNull<string>(argument, nameof (argument));
      bool usedEscapeChar;
      return SqlProviderManifest.EscapeLikeText(argument, true, out usedEscapeChar);
    }

    public override bool SupportsInExpression()
    {
      return true;
    }

    public override bool SupportsIntersectAndUnionAllFlattening()
    {
      return true;
    }
  }
}
