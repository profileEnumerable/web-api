// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IDbContextFactory`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// A factory for creating derived <see cref="T:System.Data.Entity.DbContext" /> instances. Implement this
  /// interface to enable design-time services for context types that do not have a
  /// public default constructor.
  /// At design-time, derived <see cref="T:System.Data.Entity.DbContext" /> instances can be created in order to enable specific
  /// design-time experiences such as model rendering, DDL generation etc. To enable design-time instantiation
  /// for derived <see cref="T:System.Data.Entity.DbContext" /> types that do not have a public, default constructor, implement
  /// this interface. Design-time services will auto-discover implementations of this interface that are in the
  /// same assembly as the derived <see cref="T:System.Data.Entity.DbContext" /> type.
  /// </summary>
  /// <typeparam name="TContext">The type of the context.</typeparam>
  public interface IDbContextFactory<out TContext> where TContext : DbContext
  {
    /// <summary>
    /// Creates a new instance of a derived <see cref="T:System.Data.Entity.DbContext" /> type.
    /// </summary>
    /// <returns> An instance of TContext </returns>
    TContext Create();
  }
}
