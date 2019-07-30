// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Net40DefaultDbProviderFactoryResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Infrastructure
{
  internal class Net40DefaultDbProviderFactoryResolver : IDbProviderFactoryResolver
  {
    private readonly ConcurrentDictionary<Type, DbProviderFactory> _cache = new ConcurrentDictionary<Type, DbProviderFactory>((IEnumerable<KeyValuePair<Type, DbProviderFactory>>) new KeyValuePair<Type, DbProviderFactory>[1]
    {
      new KeyValuePair<Type, DbProviderFactory>(typeof (EntityConnection), (DbProviderFactory) EntityProviderFactory.Instance)
    });
    private readonly ProviderRowFinder _finder;

    public Net40DefaultDbProviderFactoryResolver()
      : this(new ProviderRowFinder())
    {
    }

    public Net40DefaultDbProviderFactoryResolver(ProviderRowFinder finder)
    {
      this._finder = finder;
    }

    public DbProviderFactory ResolveProviderFactory(DbConnection connection)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      return this.GetProviderFactory(connection, DbProviderFactories.GetFactoryClasses().Rows.OfType<DataRow>());
    }

    public DbProviderFactory GetProviderFactory(
      DbConnection connection,
      IEnumerable<DataRow> dataRows)
    {
      return this._cache.GetOrAdd(connection.GetType(), (Func<Type, DbProviderFactory>) (t =>
      {
        DataRow providerRow = this._finder.FindRow(t, (Func<DataRow, bool>) (r => Net40DefaultDbProviderFactoryResolver.ExactMatch(r, t)), dataRows) ?? this._finder.FindRow((Type) null, (Func<DataRow, bool>) (r => Net40DefaultDbProviderFactoryResolver.ExactMatch(r, t)), dataRows) ?? this._finder.FindRow(t, (Func<DataRow, bool>) (r => Net40DefaultDbProviderFactoryResolver.AssignableMatch(r, t)), dataRows) ?? this._finder.FindRow((Type) null, (Func<DataRow, bool>) (r => Net40DefaultDbProviderFactoryResolver.AssignableMatch(r, t)), dataRows);
        if (providerRow == null)
          throw new NotSupportedException(Strings.ProviderNotFound((object) connection.ToString()));
        return DbProviderFactories.GetFactory(providerRow);
      }));
    }

    private static bool ExactMatch(DataRow row, Type connectionType)
    {
      return DbProviderFactories.GetFactory(row).CreateConnection().GetType() == connectionType;
    }

    private static bool AssignableMatch(DataRow row, Type connectionType)
    {
      return connectionType.IsInstanceOfType((object) DbProviderFactories.GetFactory(row).CreateConnection());
    }
  }
}
