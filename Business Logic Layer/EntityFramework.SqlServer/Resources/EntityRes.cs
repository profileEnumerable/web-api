// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Resources.EntityRes
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.CodeDom.Compiler;
using System.Data.Entity.SqlServer.Utilities;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace System.Data.Entity.SqlServer.Resources
{
  [GeneratedCode("Resources.SqlServer.tt", "1.0.0.0")]
  internal sealed class EntityRes
  {
    internal const string ArgumentIsNullOrWhitespace = "ArgumentIsNullOrWhitespace";
    internal const string SqlProvider_GeographyValueNotSqlCompatible = "SqlProvider_GeographyValueNotSqlCompatible";
    internal const string SqlProvider_GeometryValueNotSqlCompatible = "SqlProvider_GeometryValueNotSqlCompatible";
    internal const string ProviderReturnedNullForGetDbInformation = "ProviderReturnedNullForGetDbInformation";
    internal const string ProviderDoesNotSupportType = "ProviderDoesNotSupportType";
    internal const string NoStoreTypeForEdmType = "NoStoreTypeForEdmType";
    internal const string Mapping_Provider_WrongManifestType = "Mapping_Provider_WrongManifestType";
    internal const string ADP_InternalProviderError = "ADP_InternalProviderError";
    internal const string UnableToDetermineStoreVersion = "UnableToDetermineStoreVersion";
    internal const string SqlProvider_NeedSqlDataReader = "SqlProvider_NeedSqlDataReader";
    internal const string SqlProvider_Sql2008RequiredForSpatial = "SqlProvider_Sql2008RequiredForSpatial";
    internal const string SqlProvider_SqlTypesAssemblyNotFound = "SqlProvider_SqlTypesAssemblyNotFound";
    internal const string SqlProvider_IncompleteCreateDatabase = "SqlProvider_IncompleteCreateDatabase";
    internal const string SqlProvider_IncompleteCreateDatabaseAggregate = "SqlProvider_IncompleteCreateDatabaseAggregate";
    internal const string SqlProvider_DdlGeneration_MissingInitialCatalog = "SqlProvider_DdlGeneration_MissingInitialCatalog";
    internal const string SqlProvider_DdlGeneration_CannotDeleteDatabaseNoInitialCatalog = "SqlProvider_DdlGeneration_CannotDeleteDatabaseNoInitialCatalog";
    internal const string SqlProvider_DdlGeneration_CannotTellIfDatabaseExists = "SqlProvider_DdlGeneration_CannotTellIfDatabaseExists";
    internal const string SqlProvider_CredentialsMissingForMasterConnection = "SqlProvider_CredentialsMissingForMasterConnection";
    internal const string SqlProvider_InvalidGeographyColumn = "SqlProvider_InvalidGeographyColumn";
    internal const string SqlProvider_InvalidGeometryColumn = "SqlProvider_InvalidGeometryColumn";
    internal const string Mapping_Provider_WrongConnectionType = "Mapping_Provider_WrongConnectionType";
    internal const string Update_NotSupportedServerGenKey = "Update_NotSupportedServerGenKey";
    internal const string Update_NotSupportedIdentityType = "Update_NotSupportedIdentityType";
    internal const string Update_SqlEntitySetWithoutDmlFunctions = "Update_SqlEntitySetWithoutDmlFunctions";
    internal const string Cqt_General_UnsupportedExpression = "Cqt_General_UnsupportedExpression";
    internal const string SqlGen_ApplyNotSupportedOnSql8 = "SqlGen_ApplyNotSupportedOnSql8";
    internal const string SqlGen_NiladicFunctionsCannotHaveParameters = "SqlGen_NiladicFunctionsCannotHaveParameters";
    internal const string SqlGen_InvalidDatePartArgumentExpression = "SqlGen_InvalidDatePartArgumentExpression";
    internal const string SqlGen_InvalidDatePartArgumentValue = "SqlGen_InvalidDatePartArgumentValue";
    internal const string SqlGen_TypedNaNNotSupported = "SqlGen_TypedNaNNotSupported";
    internal const string SqlGen_TypedPositiveInfinityNotSupported = "SqlGen_TypedPositiveInfinityNotSupported";
    internal const string SqlGen_TypedNegativeInfinityNotSupported = "SqlGen_TypedNegativeInfinityNotSupported";
    internal const string SqlGen_PrimitiveTypeNotSupportedPriorSql10 = "SqlGen_PrimitiveTypeNotSupportedPriorSql10";
    internal const string SqlGen_CanonicalFunctionNotSupportedPriorSql10 = "SqlGen_CanonicalFunctionNotSupportedPriorSql10";
    internal const string SqlGen_ParameterForLimitNotSupportedOnSql8 = "SqlGen_ParameterForLimitNotSupportedOnSql8";
    internal const string SqlGen_ParameterForSkipNotSupportedOnSql8 = "SqlGen_ParameterForSkipNotSupportedOnSql8";
    internal const string Spatial_WellKnownGeographyValueNotValid = "Spatial_WellKnownGeographyValueNotValid";
    internal const string Spatial_WellKnownGeometryValueNotValid = "Spatial_WellKnownGeometryValueNotValid";
    internal const string SqlSpatialServices_ProviderValueNotSqlType = "SqlSpatialServices_ProviderValueNotSqlType";
    internal const string SqlSpatialservices_CouldNotCreateWellKnownGeographyValueNoSrid = "SqlSpatialservices_CouldNotCreateWellKnownGeographyValueNoSrid";
    internal const string SqlSpatialservices_CouldNotCreateWellKnownGeographyValueNoWkbOrWkt = "SqlSpatialservices_CouldNotCreateWellKnownGeographyValueNoWkbOrWkt";
    internal const string SqlSpatialservices_CouldNotCreateWellKnownGeometryValueNoSrid = "SqlSpatialservices_CouldNotCreateWellKnownGeometryValueNoSrid";
    internal const string SqlSpatialservices_CouldNotCreateWellKnownGeometryValueNoWkbOrWkt = "SqlSpatialservices_CouldNotCreateWellKnownGeometryValueNoWkbOrWkt";
    internal const string TransientExceptionDetected = "TransientExceptionDetected";
    internal const string ELinq_DbFunctionDirectCall = "ELinq_DbFunctionDirectCall";
    internal const string AutomaticMigration = "AutomaticMigration";
    internal const string InvalidDatabaseName = "InvalidDatabaseName";
    internal const string SqlServerMigrationSqlGenerator_UnknownOperation = "SqlServerMigrationSqlGenerator_UnknownOperation";
    private static EntityRes loader;
    private readonly ResourceManager resources;

    private EntityRes()
    {
      this.resources = new ResourceManager("System.Data.Entity.SqlServer.Properties.Resources.SqlServer", typeof (SqlProviderServices).Assembly());
    }

    private static EntityRes GetLoader()
    {
      if (EntityRes.loader == null)
      {
        EntityRes entityRes = new EntityRes();
        Interlocked.CompareExchange<EntityRes>(ref EntityRes.loader, entityRes, (EntityRes) null);
      }
      return EntityRes.loader;
    }

    private static CultureInfo Culture
    {
      get
      {
        return (CultureInfo) null;
      }
    }

    public static ResourceManager Resources
    {
      get
      {
        return EntityRes.GetLoader().resources;
      }
    }

    public static string GetString(string name, params object[] args)
    {
      EntityRes loader = EntityRes.GetLoader();
      if (loader == null)
        return (string) null;
      string format = loader.resources.GetString(name, EntityRes.Culture);
      if (args == null || args.Length <= 0)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        string str = args[index] as string;
        if (str != null && str.Length > 1024)
          args[index] = (object) (str.Substring(0, 1021) + "...");
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string GetString(string name)
    {
      return EntityRes.GetLoader()?.resources.GetString(name, EntityRes.Culture);
    }

    public static string GetString(string name, out bool usedFallback)
    {
      usedFallback = false;
      return EntityRes.GetString(name);
    }

    public static object GetObject(string name)
    {
      return EntityRes.GetLoader()?.resources.GetObject(name, EntityRes.Culture);
    }
  }
}
