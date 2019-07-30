// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.ExecutionStrategyKey
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// A key used for resolving <see cref="T:System.Func`1" />. It consists of the ADO.NET provider invariant name
  /// and the database server name as specified in the connection string.
  /// </summary>
  public class ExecutionStrategyKey
  {
    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Infrastructure.ExecutionStrategyKey" />
    /// </summary>
    /// <param name="providerInvariantName">
    /// The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this execution strategy will be used.
    /// </param>
    /// <param name="serverName"> A string that will be matched against the server name in the connection string. </param>
    public ExecutionStrategyKey(string providerInvariantName, string serverName)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      this.ProviderInvariantName = providerInvariantName;
      this.ServerName = serverName;
    }

    /// <summary>
    /// The ADO.NET provider invariant name indicating the type of ADO.NET connection for which this execution strategy will be used.
    /// </summary>
    public string ProviderInvariantName { get; private set; }

    /// <summary>
    /// A string that will be matched against the server name in the connection string.
    /// </summary>
    public string ServerName { get; private set; }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      ExecutionStrategyKey executionStrategyKey = obj as ExecutionStrategyKey;
      if (object.ReferenceEquals((object) executionStrategyKey, (object) null) || !this.ProviderInvariantName.Equals(executionStrategyKey.ProviderInvariantName, StringComparison.Ordinal))
        return false;
      if (this.ServerName == null && executionStrategyKey.ServerName == null)
        return true;
      if (this.ServerName != null)
        return this.ServerName.Equals(executionStrategyKey.ServerName, StringComparison.Ordinal);
      return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      if (this.ServerName != null)
        return this.ProviderInvariantName.GetHashCode() ^ this.ServerName.GetHashCode();
      return this.ProviderInvariantName.GetHashCode();
    }
  }
}
