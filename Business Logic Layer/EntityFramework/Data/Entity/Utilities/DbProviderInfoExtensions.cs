// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DbProviderInfoExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Utilities
{
  internal static class DbProviderInfoExtensions
  {
    public static bool IsSqlCe(this DbProviderInfo providerInfo)
    {
      if (!string.IsNullOrWhiteSpace(providerInfo.ProviderInvariantName))
        return providerInfo.ProviderInvariantName.StartsWith("System.Data.SqlServerCe", StringComparison.OrdinalIgnoreCase);
      return false;
    }
  }
}
