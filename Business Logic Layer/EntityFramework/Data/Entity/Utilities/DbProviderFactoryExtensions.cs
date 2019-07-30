// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DbProviderFactoryExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.EntityClient.Internal;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Utilities
{
  internal static class DbProviderFactoryExtensions
  {
    public static string GetProviderInvariantName(this DbProviderFactory factory)
    {
      IEnumerable<DataRow> dataRows = DbProviderFactories.GetFactoryClasses().Rows.OfType<DataRow>();
      DataRow row = new ProviderRowFinder().FindRow(factory.GetType(), (Func<DataRow, bool>) (r => DbProviderFactories.GetFactory(r).GetType() == factory.GetType()), dataRows);
      if (row == null)
        throw new NotSupportedException(Strings.ProviderNameNotFound((object) factory));
      return (string) row[2];
    }

    internal static DbProviderServices GetProviderServices(
      this DbProviderFactory factory)
    {
      if (factory is EntityProviderFactory)
        return (DbProviderServices) EntityProviderServices.Instance;
      return DbConfiguration.DependencyResolver.GetService<DbProviderServices>((object) DbConfiguration.DependencyResolver.GetService<IProviderInvariantName>((object) factory).Name);
    }
  }
}
