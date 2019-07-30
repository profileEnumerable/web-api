// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlProviderServices
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer.Resources;
using System.Data.Entity.SqlServer.SqlGen;
using System.Data.Entity.SqlServer.Utilities;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace System.Data.Entity.SqlServer
{
  /// <summary>
  /// The DbProviderServices implementation for the SqlClient provider for SQL Server.
  /// </summary>
  /// <remarks>
  /// Note that instance of this type also resolve additional provider services for Microsoft SQL Server
  /// when this type is registered as an EF provider either using an entry in the application's config file
  /// or through code-based registration in <see cref="T:System.Data.Entity.DbConfiguration" />.
  /// The services resolved are:
  /// Requests for <see cref="T:System.Data.Entity.Infrastructure.IDbConnectionFactory" /> are resolved to a Singleton instance of
  /// <see cref="T:System.Data.Entity.Infrastructure.SqlConnectionFactory" /> to create connections to SQL Express by default.
  /// Requests for <see cref="T:System.Func`1" /> for the invariant name "System.Data.SqlClient"
  /// for any server name are resolved to a delegate that returns a <see cref="T:System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy" />
  /// to provide a non-retrying policy for SQL Server.
  /// Requests for <see cref="T:System.Data.Entity.Migrations.Sql.MigrationSqlGenerator" /> for the invariant name "System.Data.SqlClient" are
  /// resolved to <see cref="T:System.Data.Entity.SqlServer.SqlServerMigrationSqlGenerator" /> instances to provide default Migrations SQL
  /// generation for SQL Server.
  /// Requests for <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" /> for the invariant name "System.Data.SqlClient" are
  /// resolved to a Singleton instance of <see cref="T:System.Data.Entity.SqlServer.SqlSpatialServices" /> to provide default spatial
  /// services for SQL Server.
  /// </remarks>
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  public sealed class SqlProviderServices : DbProviderServices
  {
    private static readonly SqlProviderServices _providerInstance = new SqlProviderServices();
    private static bool _truncateDecimalsToScale = true;
    private ConcurrentDictionary<string, SqlProviderManifest> _providerManifests = new ConcurrentDictionary<string, SqlProviderManifest>();
    /// <summary>
    /// This is the well-known string using in configuration files and code-based configuration as
    /// the "provider invariant name" used to specify Microsoft SQL Server for ADO.NET and
    /// Entity Framework provider services.
    /// </summary>
    public const string ProviderInvariantName = "System.Data.SqlClient";

    private SqlProviderServices()
    {
      this.AddDependencyResolver((IDbDependencyResolver) new SingletonDependencyResolver<IDbConnectionFactory>((IDbConnectionFactory) new System.Data.Entity.Infrastructure.SqlConnectionFactory()));
      this.AddDependencyResolver((IDbDependencyResolver) new ExecutionStrategyResolver<DefaultSqlExecutionStrategy>("System.Data.SqlClient", (string) null, (Func<DefaultSqlExecutionStrategy>) (() => new DefaultSqlExecutionStrategy())));
      this.AddDependencyResolver((IDbDependencyResolver) new SingletonDependencyResolver<Func<MigrationSqlGenerator>>((Func<MigrationSqlGenerator>) (() => (MigrationSqlGenerator) new SqlServerMigrationSqlGenerator()), (object) "System.Data.SqlClient"));
      this.AddDependencyResolver((IDbDependencyResolver) new SingletonDependencyResolver<TableExistenceChecker>((TableExistenceChecker) new SqlTableExistenceChecker(), (object) "System.Data.SqlClient"));
      this.AddDependencyResolver((IDbDependencyResolver) new SingletonDependencyResolver<DbSpatialServices>((DbSpatialServices) SqlSpatialServices.Instance, (Func<object, bool>) (k =>
      {
        if (k == null)
          return true;
        DbProviderInfo dbProviderInfo = k as DbProviderInfo;
        if (dbProviderInfo != null && dbProviderInfo.ProviderInvariantName == "System.Data.SqlClient")
          return SqlProviderServices.SupportsSpatial(dbProviderInfo.ProviderManifestToken);
        return false;
      })));
    }

    /// <summary>
    /// The Singleton instance of the SqlProviderServices type.
    /// </summary>
    public static SqlProviderServices Instance
    {
      get
      {
        return SqlProviderServices._providerInstance;
      }
    }

    /// <summary>
    /// Set to the full name of the Microsoft.SqlServer.Types assembly to override the default selection
    /// </summary>
    public static string SqlServerTypesAssemblyName { get; set; }

    /// <summary>
    /// Set this flag to false to prevent <see cref="T:System.Decimal" /> values from being truncated to
    /// the scale (number of decimal places) defined for the column. The default value is true,
    /// indicating that decimal values will be truncated, in order to prevent breaking existing
    /// applications that depend on this behavior.
    /// </summary>
    /// <remarks>
    /// With this flag set to true <see cref="T:System.Data.SqlClient.SqlParameter" /> objects are created with their Scale
    /// properties set. When this flag is set to false then the Scale properties are not set, meaning
    /// that the truncation behavior of SqlParameter is avoided.
    /// </remarks>
    public static bool TruncateDecimalsToScale
    {
      get
      {
        return SqlProviderServices._truncateDecimalsToScale;
      }
      set
      {
        SqlProviderServices._truncateDecimalsToScale = value;
      }
    }

    /// <summary>
    /// Registers a handler to process non-error messages coming from the database provider.
    /// </summary>
    /// <param name="connection"> The connection to receive information for. </param>
    /// <param name="handler"> The handler to process messages. </param>
    public override void RegisterInfoMessageHandler(DbConnection connection, Action<string> handler)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<Action<string>>(handler, nameof (handler));
      SqlConnection sqlConnection = connection as SqlConnection;
      if (sqlConnection == null)
        throw new ArgumentException(Strings.Mapping_Provider_WrongConnectionType((object) typeof (SqlConnection)));
      sqlConnection.InfoMessage += (SqlInfoMessageEventHandler) ((_, e) =>
      {
        if (string.IsNullOrWhiteSpace(e.Message))
          return;
        handler(e.Message);
      });
    }

    /// <summary>
    /// Create a Command Definition object, given the connection and command tree
    /// </summary>
    /// <param name="providerManifest"> provider manifest that was determined from metadata </param>
    /// <param name="commandTree"> command tree for the statement </param>
    /// <returns> an executable command definition object </returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    protected override DbCommandDefinition CreateDbCommandDefinition(
      DbProviderManifest providerManifest,
      DbCommandTree commandTree)
    {
      Check.NotNull<DbProviderManifest>(providerManifest, nameof (providerManifest));
      Check.NotNull<DbCommandTree>(commandTree, nameof (commandTree));
      return this.CreateCommandDefinition(SqlProviderServices.CreateCommand(providerManifest, commandTree));
    }

    /// <summary>
    /// See issue 2390 - cloning the DesignTimeVisible property on the
    /// <see cref="T:System.Data.SqlClient.SqlCommand" /> can cause deadlocks.
    /// So here overriding to provide a method that does not clone DesignTimeVisible.
    /// </summary>
    /// <param name="fromDbCommand"> the <see cref="T:System.Data.Common.DbCommand" /> object to clone </param>
    /// <returns>a clone of the <see cref="T:System.Data.Common.DbCommand" /> </returns>
    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Not changing the CommandText at all - simply providing a clone of the DbCommand with the same CommandText")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    protected override DbCommand CloneDbCommand(DbCommand fromDbCommand)
    {
      Check.NotNull<DbCommand>(fromDbCommand, nameof (fromDbCommand));
      SqlCommand sqlCommand1 = fromDbCommand as SqlCommand;
      if (sqlCommand1 == null)
        return base.CloneDbCommand(fromDbCommand);
      SqlCommand sqlCommand2 = new SqlCommand();
      sqlCommand2.CommandText = sqlCommand1.CommandText;
      sqlCommand2.CommandTimeout = sqlCommand1.CommandTimeout;
      sqlCommand2.CommandType = sqlCommand1.CommandType;
      sqlCommand2.Connection = sqlCommand1.Connection;
      sqlCommand2.Transaction = sqlCommand1.Transaction;
      sqlCommand2.UpdatedRowSource = sqlCommand1.UpdatedRowSource;
      foreach (object parameter in (DbParameterCollection) sqlCommand1.Parameters)
      {
        ICloneable cloneable = parameter as ICloneable;
        sqlCommand2.Parameters.Add(cloneable == null ? parameter : cloneable.Clone());
      }
      return (DbCommand) sqlCommand2;
    }

    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private static DbCommand CreateCommand(
      DbProviderManifest providerManifest,
      DbCommandTree commandTree)
    {
      SqlProviderManifest providerManifest1 = providerManifest as SqlProviderManifest;
      if (providerManifest1 == null)
        throw new ArgumentException(Strings.Mapping_Provider_WrongManifestType((object) typeof (SqlProviderManifest)));
      SqlVersion sqlVersion = providerManifest1.SqlVersion;
      SqlCommand sqlCommand = new SqlCommand();
      List<SqlParameter> parameters;
      CommandType commandType;
      HashSet<string> paramsToForceNonUnicode;
      sqlCommand.CommandText = SqlGenerator.GenerateSql(commandTree, sqlVersion, out parameters, out commandType, out paramsToForceNonUnicode);
      sqlCommand.CommandType = commandType;
      EdmFunction edmFunction = (EdmFunction) null;
      if (commandTree.CommandTreeKind == DbCommandTreeKind.Function)
        edmFunction = ((DbFunctionCommandTree) commandTree).EdmFunction;
      foreach (KeyValuePair<string, TypeUsage> parameter in commandTree.Parameters)
      {
        FunctionParameter functionParameter;
        SqlParameter sqlParameter;
        if (edmFunction != null && edmFunction.Parameters.TryGetValue(parameter.Key, false, out functionParameter))
        {
          sqlParameter = SqlProviderServices.CreateSqlParameter(functionParameter.Name, functionParameter.TypeUsage, functionParameter.Mode, (object) DBNull.Value, false, sqlVersion);
        }
        else
        {
          TypeUsage type = paramsToForceNonUnicode == null || !paramsToForceNonUnicode.Contains(parameter.Key) ? parameter.Value : parameter.Value.ForceNonUnicode();
          sqlParameter = SqlProviderServices.CreateSqlParameter(parameter.Key, type, ParameterMode.In, (object) DBNull.Value, false, sqlVersion);
        }
        sqlCommand.Parameters.Add(sqlParameter);
      }
      if (parameters != null && 0 < parameters.Count)
      {
        if (commandTree.CommandTreeKind != DbCommandTreeKind.Delete && commandTree.CommandTreeKind != DbCommandTreeKind.Insert && commandTree.CommandTreeKind != DbCommandTreeKind.Update)
          throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1017));
        foreach (SqlParameter sqlParameter in parameters)
          sqlCommand.Parameters.Add(sqlParameter);
      }
      return (DbCommand) sqlCommand;
    }

    /// <summary>
    /// Sets the parameter value and appropriate facets for the given <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="parameterType">The type of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    protected override void SetDbParameterValue(
      DbParameter parameter,
      TypeUsage parameterType,
      object value)
    {
      Check.NotNull<DbParameter>(parameter, nameof (parameter));
      Check.NotNull<TypeUsage>(parameterType, nameof (parameterType));
      value = SqlProviderServices.EnsureSqlParameterValue(value);
      if (parameterType.IsPrimitiveType(PrimitiveTypeKind.String) || parameterType.IsPrimitiveType(PrimitiveTypeKind.Binary))
      {
        if (!SqlProviderServices.GetParameterSize(parameterType, (parameter.Direction & ParameterDirection.Output) == ParameterDirection.Output).HasValue)
        {
          int size = parameter.Size;
          parameter.Size = 0;
          parameter.Value = value;
          if (size > -1)
          {
            if (parameter.Size >= size)
              return;
            parameter.Size = size;
          }
          else
          {
            int nonMaxLength = SqlProviderServices.GetNonMaxLength(((SqlParameter) parameter).SqlDbType);
            if (parameter.Size < nonMaxLength)
            {
              parameter.Size = nonMaxLength;
            }
            else
            {
              if (parameter.Size <= nonMaxLength)
                return;
              parameter.Size = -1;
            }
          }
        }
        else
          parameter.Value = value;
      }
      else
        parameter.Value = value;
    }

    /// <summary>
    /// Returns provider manifest token for a given connection.
    /// </summary>
    /// <param name="connection"> Connection to find manifest token from. </param>
    /// <returns> The provider manifest token for the specified connection. </returns>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    protected override string GetDbProviderManifestToken(DbConnection connection)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      if (string.IsNullOrEmpty(DbInterception.Dispatch.Connection.GetConnectionString(connection, new DbInterceptionContext())))
        throw new ArgumentException(Strings.UnableToDetermineStoreVersion);
      string providerManifestToken = (string) null;
      try
      {
        SqlProviderServices.UsingConnection(connection, (Action<DbConnection>) (conn => providerManifestToken = SqlProviderServices.QueryForManifestToken(conn)));
        return providerManifestToken;
      }
      catch
      {
      }
      try
      {
        SqlProviderServices.UsingMasterConnection(connection, (Action<DbConnection>) (conn => providerManifestToken = SqlProviderServices.QueryForManifestToken(conn)));
        return providerManifestToken;
      }
      catch
      {
      }
      return "2008";
    }

    private static string QueryForManifestToken(DbConnection conn)
    {
      SqlVersion sqlVersion = SqlVersionUtils.GetSqlVersion(conn);
      ServerType serverType = sqlVersion >= SqlVersion.Sql11 ? SqlVersionUtils.GetServerType(conn) : ServerType.OnPremises;
      return SqlVersionUtils.GetVersionHint(sqlVersion, serverType);
    }

    /// <summary>
    /// Returns the provider manifest by using the specified version information.
    /// </summary>
    /// <param name="versionHint"> The token information associated with the provider manifest. </param>
    /// <returns> The provider manifest by using the specified version information. </returns>
    protected override DbProviderManifest GetDbProviderManifest(string versionHint)
    {
      if (string.IsNullOrEmpty(versionHint))
        throw new ArgumentException(Strings.UnableToDetermineStoreVersion);
      return (DbProviderManifest) this._providerManifests.GetOrAdd(versionHint, (Func<string, SqlProviderManifest>) (s => new SqlProviderManifest(s)));
    }

    /// <summary>Gets a spatial data reader for SQL Server.</summary>
    /// <param name="fromReader"> The reader where the spatial data came from. </param>
    /// <param name="versionHint"> The manifest token associated with the provider manifest. </param>
    /// <returns> The spatial data reader. </returns>
    protected override DbSpatialDataReader GetDbSpatialDataReader(
      DbDataReader fromReader,
      string versionHint)
    {
      SqlDataReader sqlDataReader = fromReader as SqlDataReader;
      if (sqlDataReader == null)
        throw new ProviderIncompatibleException(Strings.SqlProvider_NeedSqlDataReader((object) fromReader.GetType()));
      if (!SqlProviderServices.SupportsSpatial(versionHint))
        return (DbSpatialDataReader) null;
      return (DbSpatialDataReader) new SqlSpatialDataReader(this.GetSpatialServices(new DbProviderInfo("System.Data.SqlClient", versionHint)), new SqlDataReaderWrapper(sqlDataReader));
    }

    /// <summary>Gets a spatial data reader for SQL Server.</summary>
    /// <param name="versionHint"> The manifest token associated with the provider manifest. </param>
    /// <returns> The spatial data reader. </returns>
    [Obsolete("Return DbSpatialServices from the GetService method. See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.")]
    protected override DbSpatialServices DbGetSpatialServices(string versionHint)
    {
      if (!SqlProviderServices.SupportsSpatial(versionHint))
        return (DbSpatialServices) null;
      return (DbSpatialServices) SqlSpatialServices.Instance;
    }

    private static bool SupportsSpatial(string versionHint)
    {
      if (string.IsNullOrEmpty(versionHint))
        throw new ArgumentException(Strings.UnableToDetermineStoreVersion);
      return SqlVersionUtils.GetSqlVersion(versionHint) >= SqlVersion.Sql10;
    }

    internal static SqlParameter CreateSqlParameter(
      string name,
      TypeUsage type,
      ParameterMode mode,
      object value,
      bool preventTruncation,
      SqlVersion version)
    {
      value = SqlProviderServices.EnsureSqlParameterValue(value);
      SqlParameter sqlParameter = new SqlParameter(name, value);
      ParameterDirection parameterDirection = SqlProviderServices.ParameterModeToParameterDirection(mode);
      if (sqlParameter.Direction != parameterDirection)
        sqlParameter.Direction = parameterDirection;
      bool isOutParam = mode != ParameterMode.In;
      int? size;
      byte? precision;
      byte? scale;
      string udtName;
      SqlDbType sqlDbType = SqlProviderServices.GetSqlDbType(type, isOutParam, version, out size, out precision, out scale, out udtName);
      if (sqlParameter.SqlDbType != sqlDbType)
        sqlParameter.SqlDbType = sqlDbType;
      if (sqlDbType == SqlDbType.Udt)
        sqlParameter.UdtTypeName = udtName;
      if (size.HasValue)
      {
        if (isOutParam || sqlParameter.Size != size.Value)
        {
          if (preventTruncation && size.Value != -1)
            sqlParameter.Size = Math.Max(sqlParameter.Size, size.Value);
          else
            sqlParameter.Size = size.Value;
        }
      }
      else
      {
        switch (((PrimitiveType) type.EdmType).PrimitiveTypeKind)
        {
          case PrimitiveTypeKind.Binary:
            sqlParameter.Size = SqlProviderServices.GetDefaultBinaryMaxLength(version);
            break;
          case PrimitiveTypeKind.String:
            sqlParameter.Size = SqlProviderServices.GetDefaultStringMaxLength(version, sqlDbType);
            break;
        }
      }
      if (precision.HasValue && (isOutParam || (int) sqlParameter.Precision != (int) precision.Value && SqlProviderServices._truncateDecimalsToScale))
        sqlParameter.Precision = precision.Value;
      if (scale.HasValue && (isOutParam || (int) sqlParameter.Scale != (int) scale.Value && SqlProviderServices._truncateDecimalsToScale))
        sqlParameter.Scale = scale.Value;
      bool flag = type.IsNullable();
      if (isOutParam || flag != sqlParameter.IsNullable)
        sqlParameter.IsNullable = flag;
      return sqlParameter;
    }

    private static ParameterDirection ParameterModeToParameterDirection(
      ParameterMode mode)
    {
      switch (mode)
      {
        case ParameterMode.In:
          return ParameterDirection.Input;
        case ParameterMode.Out:
          return ParameterDirection.Output;
        case ParameterMode.InOut:
          return ParameterDirection.InputOutput;
        case ParameterMode.ReturnValue:
          return ParameterDirection.ReturnValue;
        default:
          return (ParameterDirection) 0;
      }
    }

    internal static object EnsureSqlParameterValue(object value)
    {
      if (value != null && value != DBNull.Value && value.GetType().IsClass())
      {
        DbGeography geographyValue = value as DbGeography;
        if (geographyValue != null)
        {
          value = SqlTypesAssemblyLoader.DefaultInstance.GetSqlTypesAssembly().ConvertToSqlTypesGeography(geographyValue);
        }
        else
        {
          DbGeometry geometryValue = value as DbGeometry;
          if (geometryValue != null)
            value = SqlTypesAssemblyLoader.DefaultInstance.GetSqlTypesAssembly().ConvertToSqlTypesGeometry(geometryValue);
        }
      }
      return value;
    }

    private static SqlDbType GetSqlDbType(
      TypeUsage type,
      bool isOutParam,
      SqlVersion version,
      out int? size,
      out byte? precision,
      out byte? scale,
      out string udtName)
    {
      PrimitiveTypeKind primitiveTypeKind = ((PrimitiveType) type.EdmType).PrimitiveTypeKind;
      size = new int?();
      precision = new byte?();
      scale = new byte?();
      udtName = (string) null;
      switch (primitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          size = SqlProviderServices.GetParameterSize(type, isOutParam);
          return SqlProviderServices.GetBinaryDbType(type);
        case PrimitiveTypeKind.Boolean:
          return SqlDbType.Bit;
        case PrimitiveTypeKind.Byte:
          return SqlDbType.TinyInt;
        case PrimitiveTypeKind.DateTime:
          if (SqlVersionUtils.IsPreKatmai(version))
            return SqlDbType.DateTime;
          precision = SqlProviderServices.GetKatmaiDateTimePrecision(type, isOutParam);
          return SqlDbType.DateTime2;
        case PrimitiveTypeKind.Decimal:
          precision = SqlProviderServices.GetParameterPrecision(type, new byte?());
          scale = SqlProviderServices.GetScale(type);
          return SqlDbType.Decimal;
        case PrimitiveTypeKind.Double:
          return SqlDbType.Float;
        case PrimitiveTypeKind.Guid:
          return SqlDbType.UniqueIdentifier;
        case PrimitiveTypeKind.Single:
          return SqlDbType.Real;
        case PrimitiveTypeKind.SByte:
          return SqlDbType.SmallInt;
        case PrimitiveTypeKind.Int16:
          return SqlDbType.SmallInt;
        case PrimitiveTypeKind.Int32:
          return SqlDbType.Int;
        case PrimitiveTypeKind.Int64:
          return SqlDbType.BigInt;
        case PrimitiveTypeKind.String:
          size = SqlProviderServices.GetParameterSize(type, isOutParam);
          return SqlProviderServices.GetStringDbType(type);
        case PrimitiveTypeKind.Time:
          if (!SqlVersionUtils.IsPreKatmai(version))
            precision = SqlProviderServices.GetKatmaiDateTimePrecision(type, isOutParam);
          return SqlDbType.Time;
        case PrimitiveTypeKind.DateTimeOffset:
          if (!SqlVersionUtils.IsPreKatmai(version))
            precision = SqlProviderServices.GetKatmaiDateTimePrecision(type, isOutParam);
          return SqlDbType.DateTimeOffset;
        case PrimitiveTypeKind.Geometry:
          udtName = "geometry";
          return SqlDbType.Udt;
        case PrimitiveTypeKind.Geography:
          udtName = "geography";
          return SqlDbType.Udt;
        default:
          return SqlDbType.Variant;
      }
    }

    private static int? GetParameterSize(TypeUsage type, bool isOutParam)
    {
      Facet facet;
      if (type.Facets.TryGetValue("MaxLength", false, out facet) && facet.Value != null)
      {
        if (facet.IsUnbounded)
          return new int?(-1);
        return (int?) facet.Value;
      }
      if (isOutParam)
        return new int?(-1);
      return new int?();
    }

    private static int GetNonMaxLength(SqlDbType type)
    {
      int num = -1;
      switch (type)
      {
        case SqlDbType.Binary:
        case SqlDbType.Char:
        case SqlDbType.VarBinary:
        case SqlDbType.VarChar:
          num = 8000;
          break;
        case SqlDbType.NChar:
        case SqlDbType.NVarChar:
          num = 4000;
          break;
      }
      return num;
    }

    private static int GetDefaultStringMaxLength(SqlVersion version, SqlDbType type)
    {
      return version >= SqlVersion.Sql9 ? -1 : (type == SqlDbType.NChar || type == SqlDbType.NVarChar ? 4000 : 8000);
    }

    private static int GetDefaultBinaryMaxLength(SqlVersion version)
    {
      return version >= SqlVersion.Sql9 ? -1 : 8000;
    }

    private static byte? GetKatmaiDateTimePrecision(TypeUsage type, bool isOutParam)
    {
      byte? defaultIfUndefined = isOutParam ? new byte?((byte) 7) : new byte?();
      return SqlProviderServices.GetParameterPrecision(type, defaultIfUndefined);
    }

    private static byte? GetParameterPrecision(TypeUsage type, byte? defaultIfUndefined)
    {
      byte precision;
      if (type.TryGetPrecision(out precision))
        return new byte?(precision);
      return defaultIfUndefined;
    }

    private static byte? GetScale(TypeUsage type)
    {
      byte scale;
      if (type.TryGetScale(out scale))
        return new byte?(scale);
      return new byte?();
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    private static SqlDbType GetStringDbType(TypeUsage type)
    {
      SqlDbType sqlDbType;
      if (type.EdmType.Name.ToLowerInvariant() == "xml")
      {
        sqlDbType = SqlDbType.Xml;
      }
      else
      {
        bool isUnicode;
        if (!type.TryGetIsUnicode(out isUnicode))
          isUnicode = true;
        sqlDbType = !type.IsFixedLength() ? (isUnicode ? SqlDbType.NVarChar : SqlDbType.VarChar) : (isUnicode ? SqlDbType.NChar : SqlDbType.Char);
      }
      return sqlDbType;
    }

    private static SqlDbType GetBinaryDbType(TypeUsage type)
    {
      return !type.IsFixedLength() ? SqlDbType.VarBinary : SqlDbType.Binary;
    }

    /// <summary>
    /// Generates a data definition language (DDL) script that creates schema objects
    /// (tables, primary keys, foreign keys) based on the contents of the StoreItemCollection
    /// parameter and targeted for the version of the database corresponding to the provider manifest token.
    /// </summary>
    /// <param name="providerManifestToken"> The provider manifest token identifying the target version. </param>
    /// <param name="storeItemCollection"> The structure of the database. </param>
    /// <returns>
    /// A DDL script that creates schema objects based on the contents of the StoreItemCollection parameter
    /// and targeted for the version of the database corresponding to the provider manifest token.
    /// </returns>
    protected override string DbCreateDatabaseScript(
      string providerManifestToken,
      StoreItemCollection storeItemCollection)
    {
      Check.NotNull<string>(providerManifestToken, nameof (providerManifestToken));
      Check.NotNull<StoreItemCollection>(storeItemCollection, nameof (storeItemCollection));
      return SqlProviderServices.CreateObjectsScript(SqlVersionUtils.GetSqlVersion(providerManifestToken), storeItemCollection);
    }

    /// <summary>
    /// Create the database and the database objects.
    /// If initial catalog is not specified, but AttachDBFilename is specified, we generate a random database name based on the AttachDBFilename.
    /// Note: this causes pollution of the db, as when the connection string is later used, the mdf will get attached under a different name.
    /// However if we try to replicate the name under which it would be attached, the following scenario would fail:
    /// The file does not exist, but registered with database.
    /// The user calls:  If (DatabaseExists) DeleteDatabase
    /// CreateDatabase
    /// For further details on the behavior when AttachDBFilename is specified see Dev10# 188936
    /// </summary>
    /// <param name="connection">Connection to a non-existent database that needs to be created and populated with the store objects indicated with the storeItemCollection parameter.</param>
    /// <param name="commandTimeout">Execution timeout for any commands needed to create the database.</param>
    /// <param name="storeItemCollection">The collection of all store items based on which the script should be created.</param>
    protected override void DbCreateDatabase(
      DbConnection connection,
      int? commandTimeout,
      StoreItemCollection storeItemCollection)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<StoreItemCollection>(storeItemCollection, nameof (storeItemCollection));
      SqlConnection requiredSqlConnection = SqlProviderUtilities.GetRequiredSqlConnection(connection);
      string databaseName;
      string dataFileName;
      string logFileName;
      SqlProviderServices.GetOrGenerateDatabaseNameAndGetFileNames(requiredSqlConnection, out databaseName, out dataFileName, out logFileName);
      string databaseScript = SqlDdlBuilder.CreateDatabaseScript(databaseName, dataFileName, logFileName);
      SqlVersion databaseFromScript = SqlProviderServices.CreateDatabaseFromScript(commandTimeout, (DbConnection) requiredSqlConnection, databaseScript);
      try
      {
        SqlConnection.ClearPool(requiredSqlConnection);
        string setDatabaseOptionsScript = SqlDdlBuilder.SetDatabaseOptionsScript(databaseFromScript, databaseName);
        if (!string.IsNullOrEmpty(setDatabaseOptionsScript))
          SqlProviderServices.UsingMasterConnection((DbConnection) requiredSqlConnection, (Action<DbConnection>) (conn =>
          {
            using (DbCommand command = SqlProviderServices.CreateCommand(conn, setDatabaseOptionsScript, commandTimeout))
              DbInterception.Dispatch.Command.NonQuery(command, new DbCommandInterceptionContext());
          }));
        string createObjectsScript = SqlProviderServices.CreateObjectsScript(databaseFromScript, storeItemCollection);
        if (string.IsNullOrWhiteSpace(createObjectsScript))
          return;
        SqlProviderServices.UsingConnection((DbConnection) requiredSqlConnection, (Action<DbConnection>) (conn =>
        {
          using (DbCommand command = SqlProviderServices.CreateCommand(conn, createObjectsScript, commandTimeout))
            DbInterception.Dispatch.Command.NonQuery(command, new DbCommandInterceptionContext());
        }));
      }
      catch (Exception ex1)
      {
        try
        {
          SqlProviderServices.DropDatabase(requiredSqlConnection, commandTimeout, databaseName);
        }
        catch (Exception ex2)
        {
          throw new InvalidOperationException(Strings.SqlProvider_IncompleteCreateDatabase, (Exception) new AggregateException(Strings.SqlProvider_IncompleteCreateDatabaseAggregate, new Exception[2]
          {
            ex1,
            ex2
          }));
        }
        throw;
      }
    }

    private static void GetOrGenerateDatabaseNameAndGetFileNames(
      SqlConnection sqlConnection,
      out string databaseName,
      out string dataFileName,
      out string logFileName)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(DbInterception.Dispatch.Connection.GetConnectionString((DbConnection) sqlConnection, new DbInterceptionContext()));
      string attachDbFilename = connectionStringBuilder.AttachDBFilename;
      if (string.IsNullOrEmpty(attachDbFilename))
      {
        dataFileName = (string) null;
        logFileName = (string) null;
      }
      else
      {
        dataFileName = SqlProviderServices.GetMdfFileName(attachDbFilename);
        logFileName = SqlProviderServices.GetLdfFileName(dataFileName);
      }
      if (!string.IsNullOrEmpty(connectionStringBuilder.InitialCatalog))
      {
        databaseName = connectionStringBuilder.InitialCatalog;
      }
      else
      {
        if (dataFileName == null)
          throw new InvalidOperationException(Strings.SqlProvider_DdlGeneration_MissingInitialCatalog);
        databaseName = SqlProviderServices.GenerateDatabaseName(dataFileName);
      }
    }

    private static string GetLdfFileName(string dataFileName)
    {
      return Path.Combine(new FileInfo(dataFileName).Directory.FullName, Path.GetFileNameWithoutExtension(dataFileName) + "_log.ldf");
    }

    private static string GenerateDatabaseName(string mdfFileName)
    {
      char[] charArray = Path.GetFileNameWithoutExtension(mdfFileName.ToUpper(CultureInfo.InvariantCulture)).ToCharArray();
      for (int index = 0; index < charArray.Length; ++index)
      {
        if (!char.IsLetterOrDigit(charArray[index]))
          charArray[index] = '_';
      }
      string str = new string(charArray);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) (str.Length > 30 ? str.Substring(0, 30) : str), (object) Guid.NewGuid().ToString("N", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static string GetMdfFileName(string attachDBFile)
    {
      return DbProviderServices.ExpandDataDirectory(attachDBFile);
    }

    internal static SqlVersion CreateDatabaseFromScript(
      int? commandTimeout,
      DbConnection sqlConnection,
      string createDatabaseScript)
    {
      SqlVersion sqlVersion = (SqlVersion) 0;
      SqlProviderServices.UsingMasterConnection(sqlConnection, (Action<DbConnection>) (conn =>
      {
        using (DbCommand command = SqlProviderServices.CreateCommand(conn, createDatabaseScript, commandTimeout))
          DbInterception.Dispatch.Command.NonQuery(command, new DbCommandInterceptionContext());
        sqlVersion = SqlVersionUtils.GetSqlVersion(conn);
      }));
      return sqlVersion;
    }

    /// <summary>
    /// Determines whether the database for the given connection exists.
    /// There are three cases:
    /// 1.  Initial Catalog = X, AttachDBFilename = null:   (SELECT Count(*) FROM sys.databases WHERE [name]= X) &gt; 0
    /// 2.  Initial Catalog = X, AttachDBFilename = F:      if (SELECT Count(*) FROM sys.databases WHERE [name]= X) &gt;  true,
    /// if not, try to open the connection and then return (SELECT Count(*) FROM sys.databases WHERE [name]= X) &gt; 0
    /// 3.  Initial Catalog = null, AttachDBFilename = F:   Try to open the connection. If that succeeds the result is true, otherwise
    /// if the there are no databases corresponding to the given file return false, otherwise throw.
    /// Note: We open the connection to cover the scenario when the mdf exists, but is not attached.
    /// Given that opening the connection would auto-attach it, it would not be appropriate to return false in this case.
    /// Also note that checking for the existence of the file does not work for a remote server.  (Dev11 #290487)
    /// For further details on the behavior when AttachDBFilename is specified see Dev10# 188936
    /// </summary>
    /// <param name="connection">Connection to a database whose existence is checked by this method.</param>
    /// <param name="commandTimeout">Execution timeout for any commands needed to determine the existence of the database.</param>
    /// <param name="storeItemCollection">The collection of all store items from the model. This parameter is no longer used for determining database existence.</param>
    /// <returns>True if the provider can deduce the database only based on the connection.</returns>
    protected override bool DbDatabaseExists(
      DbConnection connection,
      int? commandTimeout,
      StoreItemCollection storeItemCollection)
    {
      return this.DbDatabaseExists(connection, commandTimeout, new Lazy<StoreItemCollection>((Func<StoreItemCollection>) (() => storeItemCollection)));
    }

    /// <summary>
    /// Determines whether the database for the given connection exists.
    /// There are three cases:
    /// 1.  Initial Catalog = X, AttachDBFilename = null:   (SELECT Count(*) FROM sys.databases WHERE [name]= X) &gt; 0
    /// 2.  Initial Catalog = X, AttachDBFilename = F:      if (SELECT Count(*) FROM sys.databases WHERE [name]= X) &gt;  true,
    /// if not, try to open the connection and then return (SELECT Count(*) FROM sys.databases WHERE [name]= X) &gt; 0
    /// 3.  Initial Catalog = null, AttachDBFilename = F:   Try to open the connection. If that succeeds the result is true, otherwise
    /// if the there are no databases corresponding to the given file return false, otherwise throw.
    /// Note: We open the connection to cover the scenario when the mdf exists, but is not attached.
    /// Given that opening the connection would auto-attach it, it would not be appropriate to return false in this case.
    /// Also note that checking for the existence of the file does not work for a remote server.  (Dev11 #290487)
    /// For further details on the behavior when AttachDBFilename is specified see Dev10# 188936
    /// </summary>
    /// <param name="connection">Connection to a database whose existence is checked by this method.</param>
    /// <param name="commandTimeout">Execution timeout for any commands needed to determine the existence of the database.</param>
    /// <param name="storeItemCollection">The collection of all store items from the model. This parameter is no longer used for determining database existence.</param>
    /// <returns>True if the provider can deduce the database only based on the connection.</returns>
    protected override bool DbDatabaseExists(
      DbConnection connection,
      int? commandTimeout,
      Lazy<StoreItemCollection> storeItemCollection)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<Lazy<StoreItemCollection>>(storeItemCollection, nameof (storeItemCollection));
      if (connection.State == ConnectionState.Open)
        return true;
      SqlConnection requiredSqlConnection = SqlProviderUtilities.GetRequiredSqlConnection(connection);
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(DbInterception.Dispatch.Connection.GetConnectionString((DbConnection) requiredSqlConnection, new DbInterceptionContext()));
      if (string.IsNullOrEmpty(connectionStringBuilder.InitialCatalog) && string.IsNullOrEmpty(connectionStringBuilder.AttachDBFilename))
        throw new InvalidOperationException(Strings.SqlProvider_DdlGeneration_MissingInitialCatalog);
      if (!string.IsNullOrEmpty(connectionStringBuilder.InitialCatalog) && SqlProviderServices.CheckDatabaseExists(requiredSqlConnection, commandTimeout, connectionStringBuilder.InitialCatalog))
        return true;
      if (string.IsNullOrEmpty(connectionStringBuilder.AttachDBFilename))
        return false;
      try
      {
        SqlProviderServices.UsingConnection((DbConnection) requiredSqlConnection, (Action<DbConnection>) (con => {}));
        return true;
      }
      catch (SqlException ex)
      {
        if (!string.IsNullOrEmpty(connectionStringBuilder.InitialCatalog))
          return SqlProviderServices.CheckDatabaseExists(requiredSqlConnection, commandTimeout, connectionStringBuilder.InitialCatalog);
        string fileName = SqlProviderServices.GetMdfFileName(connectionStringBuilder.AttachDBFilename);
        bool databaseDoesNotExistInSysTables = false;
        SqlProviderServices.UsingMasterConnection((DbConnection) requiredSqlConnection, (Action<DbConnection>) (conn =>
        {
          string onFileNameScript = SqlDdlBuilder.CreateCountDatabasesBasedOnFileNameScript(fileName, SqlVersionUtils.GetSqlVersion(conn) == SqlVersion.Sql8);
          using (DbCommand command = SqlProviderServices.CreateCommand(conn, onFileNameScript, commandTimeout))
            databaseDoesNotExistInSysTables = (int) DbInterception.Dispatch.Command.Scalar(command, new DbCommandInterceptionContext()) == 0;
        }));
        if (databaseDoesNotExistInSysTables)
          return false;
        throw new InvalidOperationException(Strings.SqlProvider_DdlGeneration_CannotTellIfDatabaseExists, (Exception) ex);
      }
    }

    private static bool CheckDatabaseExists(
      SqlConnection sqlConnection,
      int? commandTimeout,
      string databaseName)
    {
      bool databaseExists = false;
      SqlProviderServices.UsingMasterConnection((DbConnection) sqlConnection, (Action<DbConnection>) (conn =>
      {
        string databaseExistsScript = SqlDdlBuilder.CreateDatabaseExistsScript(databaseName);
        using (DbCommand command = SqlProviderServices.CreateCommand(conn, databaseExistsScript, commandTimeout))
          databaseExists = (int) DbInterception.Dispatch.Command.Scalar(command, new DbCommandInterceptionContext()) >= 1;
      }));
      return databaseExists;
    }

    /// <summary>
    /// Delete the database for the given connection.
    /// There are three cases:
    /// 1.  If Initial Catalog is specified (X) drop database X
    /// 2.  Else if AttachDBFilename is specified (F) drop all the databases corresponding to F
    /// if none throw
    /// 3.  If niether the catalog not the file name is specified - throw
    /// Note that directly deleting the files does not work for a remote server.  However, even for not attached
    /// databases the current logic would work assuming the user does: if (DatabaseExists) DeleteDatabase
    /// </summary>
    /// <param name="connection"> Connection </param>
    /// <param name="commandTimeout"> Timeout for internal commands. </param>
    /// <param name="storeItemCollection"> Item Collection. </param>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    protected override void DbDeleteDatabase(
      DbConnection connection,
      int? commandTimeout,
      StoreItemCollection storeItemCollection)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      Check.NotNull<StoreItemCollection>(storeItemCollection, nameof (storeItemCollection));
      SqlConnection requiredSqlConnection = SqlProviderUtilities.GetRequiredSqlConnection(connection);
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(DbInterception.Dispatch.Connection.GetConnectionString((DbConnection) requiredSqlConnection, new DbInterceptionContext()));
      string initialCatalog = connectionStringBuilder.InitialCatalog;
      string attachDbFilename = connectionStringBuilder.AttachDBFilename;
      if (!string.IsNullOrEmpty(initialCatalog))
      {
        SqlProviderServices.DropDatabase(requiredSqlConnection, commandTimeout, initialCatalog);
      }
      else
      {
        if (string.IsNullOrEmpty(attachDbFilename))
          throw new InvalidOperationException(Strings.SqlProvider_DdlGeneration_MissingInitialCatalog);
        string fullFileName = SqlProviderServices.GetMdfFileName(attachDbFilename);
        List<string> databaseNames = new List<string>();
        SqlProviderServices.UsingMasterConnection((DbConnection) requiredSqlConnection, (Action<DbConnection>) (conn =>
        {
          string onFileNameScript = SqlDdlBuilder.CreateGetDatabaseNamesBasedOnFileNameScript(fullFileName, SqlVersionUtils.GetSqlVersion(conn) == SqlVersion.Sql8);
          using (DbDataReader dbDataReader = DbInterception.Dispatch.Command.Reader(SqlProviderServices.CreateCommand(conn, onFileNameScript, commandTimeout), new DbCommandInterceptionContext()))
          {
            while (dbDataReader.Read())
              databaseNames.Add(dbDataReader.GetString(0));
          }
        }));
        if (databaseNames.Count <= 0)
          throw new InvalidOperationException(Strings.SqlProvider_DdlGeneration_CannotDeleteDatabaseNoInitialCatalog);
        foreach (string databaseName in databaseNames)
          SqlProviderServices.DropDatabase(requiredSqlConnection, commandTimeout, databaseName);
      }
    }

    private static void DropDatabase(
      SqlConnection sqlConnection,
      int? commandTimeout,
      string databaseName)
    {
      SqlConnection.ClearAllPools();
      string dropDatabaseScript = SqlDdlBuilder.DropDatabaseScript(databaseName);
      try
      {
        SqlProviderServices.UsingMasterConnection((DbConnection) sqlConnection, (Action<DbConnection>) (conn =>
        {
          using (DbCommand command = SqlProviderServices.CreateCommand(conn, dropDatabaseScript, commandTimeout))
            DbInterception.Dispatch.Command.NonQuery(command, new DbCommandInterceptionContext());
        }));
      }
      catch (SqlException ex)
      {
        foreach (SqlError error in ex.Errors)
        {
          if (error.Number == 5120)
            return;
        }
        throw;
      }
    }

    private static string CreateObjectsScript(
      SqlVersion version,
      StoreItemCollection storeItemCollection)
    {
      return SqlDdlBuilder.CreateObjectsScript(storeItemCollection, version != SqlVersion.Sql8);
    }

    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by caller")]
    private static DbCommand CreateCommand(
      DbConnection sqlConnection,
      string commandText,
      int? commandTimeout)
    {
      if (string.IsNullOrEmpty(commandText))
        commandText = Environment.NewLine;
      DbCommand command = sqlConnection.CreateCommand();
      command.CommandText = commandText;
      if (commandTimeout.HasValue)
        command.CommandTimeout = commandTimeout.Value;
      return command;
    }

    private static void UsingConnection(DbConnection sqlConnection, Action<DbConnection> act)
    {
      DbInterceptionContext interceptionContext = new DbInterceptionContext();
      string holdConnectionString = DbInterception.Dispatch.Connection.GetConnectionString(sqlConnection, interceptionContext);
      DbProviderServices.GetExecutionStrategy(sqlConnection, "System.Data.SqlClient").Execute((Action) (() =>
      {
        bool flag = DbInterception.Dispatch.Connection.GetState(sqlConnection, interceptionContext) == ConnectionState.Closed;
        if (flag)
        {
          if (DbInterception.Dispatch.Connection.GetState(sqlConnection, new DbInterceptionContext()) == ConnectionState.Closed && !DbInterception.Dispatch.Connection.GetConnectionString(sqlConnection, interceptionContext).Equals(holdConnectionString, StringComparison.Ordinal))
            DbInterception.Dispatch.Connection.SetConnectionString(sqlConnection, new DbConnectionPropertyInterceptionContext<string>().WithValue(holdConnectionString));
          DbInterception.Dispatch.Connection.Open(sqlConnection, interceptionContext);
        }
        try
        {
          act(sqlConnection);
        }
        finally
        {
          if (flag && DbInterception.Dispatch.Connection.GetState(sqlConnection, interceptionContext) == ConnectionState.Open)
          {
            DbInterception.Dispatch.Connection.Close(sqlConnection, interceptionContext);
            if (!DbInterception.Dispatch.Connection.GetConnectionString(sqlConnection, interceptionContext).Equals(holdConnectionString, StringComparison.Ordinal))
              DbInterception.Dispatch.Connection.SetConnectionString(sqlConnection, new DbConnectionPropertyInterceptionContext<string>().WithValue(holdConnectionString));
          }
        }
      }));
    }

    private static void UsingMasterConnection(DbConnection sqlConnection, Action<DbConnection> act)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(DbInterception.Dispatch.Connection.GetConnectionString(sqlConnection, new DbInterceptionContext()))
      {
        InitialCatalog = "master",
        AttachDBFilename = string.Empty
      };
      try
      {
        using (DbConnection connection = DbProviderServices.GetProviderFactory(sqlConnection).CreateConnection())
        {
          DbInterception.Dispatch.Connection.SetConnectionString(connection, new DbConnectionPropertyInterceptionContext<string>().WithValue(connectionStringBuilder.ConnectionString));
          SqlProviderServices.UsingConnection(connection, act);
        }
      }
      catch (SqlException ex)
      {
        if (!connectionStringBuilder.IntegratedSecurity && (string.IsNullOrEmpty(connectionStringBuilder.UserID) || string.IsNullOrEmpty(connectionStringBuilder.Password)))
          throw new InvalidOperationException(Strings.SqlProvider_CredentialsMissingForMasterConnection, (Exception) ex);
        throw;
      }
    }
  }
}
