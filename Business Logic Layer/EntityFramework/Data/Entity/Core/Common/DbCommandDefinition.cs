// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.DbCommandDefinition
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Common
{
  /// <summary>
  /// A prepared command definition, can be cached and reused to avoid
  /// repreparing a command.
  /// </summary>
  public class DbCommandDefinition
  {
    private readonly DbCommand _prototype;
    private readonly Func<DbCommand, DbCommand> _cloneMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Common.DbCommandDefinition" /> class using the supplied
    /// <see cref="T:System.Data.Common.DbCommand" />
    /// .
    /// </summary>
    /// <param name="prototype">
    /// The supplied <see cref="T:System.Data.Common.DbCommand" />.
    /// </param>
    /// <param name="cloneMethod"> method used to clone the <see cref="T:System.Data.Common.DbCommand" /> </param>
    protected internal DbCommandDefinition(
      DbCommand prototype,
      Func<DbCommand, DbCommand> cloneMethod)
    {
      Check.NotNull<DbCommand>(prototype, nameof (prototype));
      Check.NotNull<Func<DbCommand, DbCommand>>(cloneMethod, nameof (cloneMethod));
      this._prototype = prototype;
      this._cloneMethod = cloneMethod;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Common.DbCommandDefinition" /> class.
    /// </summary>
    protected DbCommandDefinition()
    {
    }

    /// <summary>
    /// Creates and returns a <see cref="T:System.Data.Common.DbCommand" /> object that can be executed.
    /// </summary>
    /// <returns>The command for database.</returns>
    public virtual DbCommand CreateCommand()
    {
      return this._cloneMethod(this._prototype);
    }

    internal static void PopulateParameterFromTypeUsage(
      DbParameter parameter,
      TypeUsage type,
      bool isOutParam)
    {
      parameter.IsNullable = TypeSemantics.IsNullable(type);
      DbType dbType;
      if (!Helper.IsPrimitiveType(type.EdmType) || !DbCommandDefinition.TryGetDbTypeFromPrimitiveType((PrimitiveType) type.EdmType, out dbType))
        return;
      switch (dbType)
      {
        case DbType.Binary:
          DbCommandDefinition.PopulateBinaryParameter(parameter, type, dbType, isOutParam);
          break;
        case DbType.DateTime:
        case DbType.Time:
        case DbType.DateTimeOffset:
          DbCommandDefinition.PopulateDateTimeParameter(parameter, type, dbType);
          break;
        case DbType.Decimal:
          DbCommandDefinition.PopulateDecimalParameter(parameter, type, dbType);
          break;
        case DbType.String:
          DbCommandDefinition.PopulateStringParameter(parameter, type, isOutParam);
          break;
        default:
          parameter.DbType = dbType;
          break;
      }
    }

    internal static bool TryGetDbTypeFromPrimitiveType(PrimitiveType type, out DbType dbType)
    {
      switch (type.PrimitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          dbType = DbType.Binary;
          return true;
        case PrimitiveTypeKind.Boolean:
          dbType = DbType.Boolean;
          return true;
        case PrimitiveTypeKind.Byte:
          dbType = DbType.Byte;
          return true;
        case PrimitiveTypeKind.DateTime:
          dbType = DbType.DateTime;
          return true;
        case PrimitiveTypeKind.Decimal:
          dbType = DbType.Decimal;
          return true;
        case PrimitiveTypeKind.Double:
          dbType = DbType.Double;
          return true;
        case PrimitiveTypeKind.Guid:
          dbType = DbType.Guid;
          return true;
        case PrimitiveTypeKind.Single:
          dbType = DbType.Single;
          return true;
        case PrimitiveTypeKind.SByte:
          dbType = DbType.SByte;
          return true;
        case PrimitiveTypeKind.Int16:
          dbType = DbType.Int16;
          return true;
        case PrimitiveTypeKind.Int32:
          dbType = DbType.Int32;
          return true;
        case PrimitiveTypeKind.Int64:
          dbType = DbType.Int64;
          return true;
        case PrimitiveTypeKind.String:
          dbType = DbType.String;
          return true;
        case PrimitiveTypeKind.Time:
          dbType = DbType.Time;
          return true;
        case PrimitiveTypeKind.DateTimeOffset:
          dbType = DbType.DateTimeOffset;
          return true;
        default:
          dbType = DbType.AnsiString;
          return false;
      }
    }

    private static void PopulateBinaryParameter(
      DbParameter parameter,
      TypeUsage type,
      DbType dbType,
      bool isOutParam)
    {
      parameter.DbType = dbType;
      DbCommandDefinition.SetParameterSize(parameter, type, isOutParam);
    }

    private static void PopulateDecimalParameter(
      DbParameter parameter,
      TypeUsage type,
      DbType dbType)
    {
      parameter.DbType = dbType;
      IDbDataParameter dbDataParameter = (IDbDataParameter) parameter;
      byte precision;
      if (TypeHelpers.TryGetPrecision(type, out precision))
        dbDataParameter.Precision = precision;
      byte scale;
      if (!TypeHelpers.TryGetScale(type, out scale))
        return;
      dbDataParameter.Scale = scale;
    }

    private static void PopulateDateTimeParameter(
      DbParameter parameter,
      TypeUsage type,
      DbType dbType)
    {
      parameter.DbType = dbType;
      IDbDataParameter dbDataParameter = (IDbDataParameter) parameter;
      byte precision;
      if (!TypeHelpers.TryGetPrecision(type, out precision))
        return;
      dbDataParameter.Precision = precision;
    }

    private static void PopulateStringParameter(
      DbParameter parameter,
      TypeUsage type,
      bool isOutParam)
    {
      bool isUnicode = true;
      bool isFixedLength = false;
      if (!TypeHelpers.TryGetIsFixedLength(type, out isFixedLength))
        isFixedLength = false;
      if (!TypeHelpers.TryGetIsUnicode(type, out isUnicode))
        isUnicode = true;
      parameter.DbType = !isFixedLength ? (isUnicode ? DbType.String : DbType.AnsiString) : (isUnicode ? DbType.StringFixedLength : DbType.AnsiStringFixedLength);
      DbCommandDefinition.SetParameterSize(parameter, type, isOutParam);
    }

    private static void SetParameterSize(DbParameter parameter, TypeUsage type, bool isOutParam)
    {
      Facet facet;
      if (!type.Facets.TryGetValue("MaxLength", true, out facet) || facet.Value == null)
        return;
      if (!Helper.IsUnboundedFacetValue(facet))
      {
        parameter.Size = (int) facet.Value;
      }
      else
      {
        if (!isOutParam)
          return;
        parameter.Size = int.MaxValue;
      }
    }
  }
}
