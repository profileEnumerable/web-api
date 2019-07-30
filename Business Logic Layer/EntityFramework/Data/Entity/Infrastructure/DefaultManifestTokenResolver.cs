// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DefaultManifestTokenResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// A default implementation of <see cref="T:System.Data.Entity.Infrastructure.IManifestTokenResolver" /> that uses the
  /// underlying provider to get the manifest token.
  /// Note that to avoid multiple queries, this implementation using caching based on the actual type of
  /// <see cref="T:System.Data.Common.DbConnection" /> instance, the <see cref="P:System.Data.Common.DbConnection.DataSource" /> property,
  /// and the <see cref="P:System.Data.Common.DbConnection.Database" /> property.
  /// </summary>
  public class DefaultManifestTokenResolver : IManifestTokenResolver
  {
    private readonly ConcurrentDictionary<Tuple<Type, string, string>, string> _cachedTokens = new ConcurrentDictionary<Tuple<Type, string, string>, string>();

    /// <inheritdoc />
    public string ResolveManifestToken(DbConnection connection)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      DbInterceptionContext interceptionContext = new DbInterceptionContext();
      return this._cachedTokens.GetOrAdd(Tuple.Create<Type, string, string>(connection.GetType(), DbInterception.Dispatch.Connection.GetDataSource(connection, interceptionContext), DbInterception.Dispatch.Connection.GetDatabase(connection, interceptionContext)), (Func<Tuple<Type, string, string>, string>) (k => DbProviderServices.GetProviderServices(connection).GetProviderManifestTokenChecked(connection)));
    }
  }
}
