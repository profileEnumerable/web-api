// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Resources.Error
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.CodeDom.Compiler;

namespace System.Data.Entity.SqlServer.Resources
{
  [GeneratedCode("Resources.SqlServer.tt", "1.0.0.0")]
  internal static class Error
  {
    internal static Exception InvalidDatabaseName(object p0)
    {
      return (Exception) new ArgumentException(Strings.InvalidDatabaseName(p0));
    }

    internal static Exception SqlServerMigrationSqlGenerator_UnknownOperation(
      object p0,
      object p1)
    {
      return (Exception) new InvalidOperationException(Strings.SqlServerMigrationSqlGenerator_UnknownOperation(p0, p1));
    }

    internal static Exception ArgumentOutOfRange(string paramName)
    {
      return (Exception) new ArgumentOutOfRangeException(paramName);
    }

    internal static Exception NotImplemented()
    {
      return (Exception) new NotImplementedException();
    }

    internal static Exception NotSupported()
    {
      return (Exception) new NotSupportedException();
    }
  }
}
