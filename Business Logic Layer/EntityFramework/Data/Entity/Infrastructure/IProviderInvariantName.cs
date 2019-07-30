// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IProviderInvariantName
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Used by <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver" /> and <see cref="T:System.Data.Entity.DbConfiguration" /> when resolving
  /// a provider invariant name from a <see cref="T:System.Data.Common.DbProviderFactory" />.
  /// </summary>
  public interface IProviderInvariantName
  {
    /// <summary>Gets the name of the provider.</summary>
    /// <returns>The name of the provider.</returns>
    string Name { get; }
  }
}
