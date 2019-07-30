// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DbConnectionExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Utilities
{
  internal static class DbConnectionExtensions
  {
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public static string GetProviderInvariantName(this DbConnection connection)
    {
      return DbConfiguration.DependencyResolver.GetService<IProviderInvariantName>((object) DbProviderServices.GetProviderFactory(connection)).Name;
    }

    public static DbProviderInfo GetProviderInfo(
      this DbConnection connection,
      out DbProviderManifest providerManifest)
    {
      string str = DbConfiguration.DependencyResolver.GetService<IManifestTokenResolver>().ResolveManifestToken(connection);
      DbProviderInfo dbProviderInfo = new DbProviderInfo(connection.GetProviderInvariantName(), str);
      providerManifest = DbProviderServices.GetProviderServices(connection).GetProviderManifest(str);
      return dbProviderInfo;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public static DbProviderFactory GetProviderFactory(this DbConnection connection)
    {
      return DbConfiguration.DependencyResolver.GetService<IDbProviderFactoryResolver>().ResolveProviderFactory(connection);
    }
  }
}
