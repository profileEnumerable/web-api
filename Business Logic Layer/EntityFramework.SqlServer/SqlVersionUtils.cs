// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlVersionUtils
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.SqlServer.Resources;
using System.Globalization;

namespace System.Data.Entity.SqlServer
{
  internal static class SqlVersionUtils
  {
    internal static SqlVersion GetSqlVersion(DbConnection connection)
    {
      int num = int.Parse(DbInterception.Dispatch.Connection.GetServerVersion(connection, new DbInterceptionContext()).Substring(0, 2), (IFormatProvider) CultureInfo.InvariantCulture);
      if (num >= 11)
        return SqlVersion.Sql11;
      if (num == 10)
        return SqlVersion.Sql10;
      return num == 9 ? SqlVersion.Sql9 : SqlVersion.Sql8;
    }

    internal static ServerType GetServerType(DbConnection connection)
    {
      using (DbCommand command = connection.CreateCommand())
      {
        command.CommandText = "select cast(serverproperty('EngineEdition') as int)";
        using (DbDataReader dbDataReader = DbInterception.Dispatch.Command.Reader(command, new DbCommandInterceptionContext()))
        {
          dbDataReader.Read();
          return dbDataReader.GetInt32(0) == 5 ? ServerType.Cloud : ServerType.OnPremises;
        }
      }
    }

    internal static string GetVersionHint(SqlVersion version, ServerType serverType)
    {
      if (serverType == ServerType.Cloud)
        return "2012.Azure";
      switch (version)
      {
        case SqlVersion.Sql8:
          return "2000";
        case SqlVersion.Sql9:
          return "2005";
        case SqlVersion.Sql10:
          return "2008";
        case SqlVersion.Sql11:
          return "2012";
        default:
          throw new ArgumentException(Strings.UnableToDetermineStoreVersion);
      }
    }

    internal static SqlVersion GetSqlVersion(string versionHint)
    {
      if (!string.IsNullOrEmpty(versionHint))
      {
        switch (versionHint)
        {
          case "2000":
            return SqlVersion.Sql8;
          case "2005":
            return SqlVersion.Sql9;
          case "2008":
            return SqlVersion.Sql10;
          case "2012":
            return SqlVersion.Sql11;
          case "2012.Azure":
            return SqlVersion.Sql11;
        }
      }
      throw new ArgumentException(Strings.UnableToDetermineStoreVersion);
    }

    internal static bool IsPreKatmai(SqlVersion sqlVersion)
    {
      if (sqlVersion != SqlVersion.Sql8)
        return sqlVersion == SqlVersion.Sql9;
      return true;
    }
  }
}
