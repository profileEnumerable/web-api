// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.ExecutionStrategyResolver`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  /// <summary>
  /// An <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver" /> implementation used for resolving <see cref="T:System.Data.Entity.Infrastructure.IDbExecutionStrategy" />
  /// factories.
  /// </summary>
  /// <remarks>
  /// This class can be used by <see cref="T:System.Data.Entity.Core.Common.DbProviderServices" /> to aid in the resolving
  /// of <see cref="T:System.Data.Entity.Infrastructure.IDbExecutionStrategy" /> factories as a default service for the provider.
  /// </remarks>
  /// <typeparam name="T">The type of execution strategy that is resolved.</typeparam>
  public class ExecutionStrategyResolver<T> : IDbDependencyResolver where T : IDbExecutionStrategy
  {
    private readonly Func<T> _getExecutionStrategy;
    private readonly string _providerInvariantName;
    private readonly string _serverName;

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.ExecutionStrategyResolver`1" />
    /// </summary>
    /// <param name="providerInvariantName">
    /// The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this execution strategy will be used.
    /// </param>
    /// <param name="serverName">
    /// A string that will be matched against the server name in the connection string. <c>null</c> will match anything.
    /// </param>
    /// <param name="getExecutionStrategy">A function that returns a new instance of an execution strategy.</param>
    public ExecutionStrategyResolver(
      string providerInvariantName,
      string serverName,
      Func<T> getExecutionStrategy)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<Func<T>>(getExecutionStrategy, nameof (getExecutionStrategy));
      this._providerInvariantName = providerInvariantName;
      this._serverName = serverName;
      this._getExecutionStrategy = getExecutionStrategy;
    }

    /// <summary>
    /// If the given type is <see cref="T:System.Func`1" />, then this resolver will attempt
    /// to return the service to use, otherwise it will return null. When the given type is
    /// Func{IExecutionStrategy}, then the key is expected to be an <see cref="T:System.Data.Entity.Infrastructure.ExecutionStrategyKey" />.
    /// </summary>
    /// <param name="type">The service type to resolve.</param>
    /// <param name="key">A key used to make a determination of the service to return.</param>
    /// <returns>
    /// An <see cref="T:System.Func`1" />, or null.
    /// </returns>
    public object GetService(Type type, object key)
    {
      if (!(type == typeof (Func<IDbExecutionStrategy>)))
        return (object) null;
      ExecutionStrategyKey executionStrategyKey = key as ExecutionStrategyKey;
      if (executionStrategyKey == null)
        throw new ArgumentException(Strings.DbDependencyResolver_InvalidKey((object) typeof (ExecutionStrategyKey).Name, (object) "Func<IExecutionStrategy>"));
      if (!executionStrategyKey.ProviderInvariantName.Equals(this._providerInvariantName, StringComparison.Ordinal))
        return (object) null;
      if (this._serverName != null && !this._serverName.Equals(executionStrategyKey.ServerName, StringComparison.Ordinal))
        return (object) null;
      return (object) this._getExecutionStrategy;
    }

    /// <summary>
    /// If the given type is <see cref="T:System.Func`1" />, then this resolver will attempt
    /// to return the service to use, otherwise it will return an empty enumeration. When the given type is
    /// Func{IExecutionStrategy}, then the key is expected to be an <see cref="T:System.Data.Entity.Infrastructure.ExecutionStrategyKey" />.
    /// </summary>
    /// <param name="type">The service type to resolve.</param>
    /// <param name="key">A key used to make a determination of the service to return.</param>
    /// <returns>
    /// An enumerable of <see cref="T:System.Func`1" />, or an empty enumeration.
    /// </returns>
    public IEnumerable<object> GetServices(Type type, object key)
    {
      return this.GetServiceAsServices(type, key);
    }
  }
}
