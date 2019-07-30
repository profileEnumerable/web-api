// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbProviderInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Groups a pair of strings that identify a provider and server version together into a single object.
  /// </summary>
  /// <remarks>
  /// Instances of this class act as the key for resolving a <see cref="T:System.Data.Entity.Spatial.DbSpatialServices" /> for a specific
  /// provider from a <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver" />. This is typically used when registering spatial services
  /// in <see cref="T:System.Data.Entity.DbConfiguration" /> or when the spatial services specific to a provider is
  /// resolved by an implementation of <see cref="T:System.Data.Entity.Core.Common.DbProviderServices" />.
  /// </remarks>
  public sealed class DbProviderInfo
  {
    private readonly string _providerInvariantName;
    private readonly string _providerManifestToken;

    /// <summary>
    /// Creates a new object for a given provider invariant name and manifest token.
    /// </summary>
    /// <param name="providerInvariantName">
    /// A string that identifies that provider. For example, the SQL Server
    /// provider uses the string "System.Data.SqlCient".
    /// </param>
    /// <param name="providerManifestToken">
    /// A string that identifies that version of the database server being used. For example, the SQL Server
    /// provider uses the string "2008" for SQL Server 2008. This cannot be null but may be empty.
    /// The manifest token is sometimes referred to as a version hint.
    /// </param>
    public DbProviderInfo(string providerInvariantName, string providerManifestToken)
    {
      Check.NotEmpty(providerInvariantName, nameof (providerInvariantName));
      Check.NotNull<string>(providerManifestToken, nameof (providerManifestToken));
      this._providerInvariantName = providerInvariantName;
      this._providerManifestToken = providerManifestToken;
    }

    /// <summary>
    /// A string that identifies that provider. For example, the SQL Server
    /// provider uses the string "System.Data.SqlCient".
    /// </summary>
    public string ProviderInvariantName
    {
      get
      {
        return this._providerInvariantName;
      }
    }

    /// <summary>
    /// A string that identifies that version of the database server being used. For example, the SQL Server
    /// provider uses the string "2008" for SQL Server 2008. This cannot be null but may be empty.
    /// </summary>
    public string ProviderManifestToken
    {
      get
      {
        return this._providerManifestToken;
      }
    }

    private bool Equals(DbProviderInfo other)
    {
      if (string.Equals(this._providerInvariantName, other._providerInvariantName))
        return string.Equals(this._providerManifestToken, other._providerManifestToken);
      return false;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      DbProviderInfo other = obj as DbProviderInfo;
      if (other != null)
        return this.Equals(other);
      return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return this._providerInvariantName.GetHashCode() * 397 ^ this._providerManifestToken.GetHashCode();
    }
  }
}
