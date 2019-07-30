// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlAzureRetriableExceptionDetector
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.SqlClient;

namespace System.Data.Entity.SqlServer
{
  internal static class SqlAzureRetriableExceptionDetector
  {
    public static bool ShouldRetryOn(Exception ex)
    {
      SqlException sqlException = ex as SqlException;
      if (sqlException != null)
      {
        foreach (SqlError error in sqlException.Errors)
        {
          switch (error.Number)
          {
            case 20:
            case 64:
            case 233:
            case 10053:
            case 10054:
            case 10060:
            case 10928:
            case 10929:
            case 40197:
            case 40501:
            case 40613:
            case 41301:
            case 41302:
            case 41305:
            case 41325:
              return true;
            default:
              continue;
          }
        }
        return false;
      }
      return ex is TimeoutException;
    }
  }
}
