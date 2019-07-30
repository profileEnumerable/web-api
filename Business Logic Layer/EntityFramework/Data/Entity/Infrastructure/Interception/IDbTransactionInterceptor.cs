// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.IDbTransactionInterceptor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// An object that implements this interface can be registered with <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> to
  /// receive notifications when Entity Framework commits or rollbacks a transaction.
  /// </summary>
  /// <remarks>
  /// Interceptors can also be registered in the config file of the application.
  /// See http://go.microsoft.com/fwlink/?LinkId=260883 for more information about Entity Framework configuration.
  /// </remarks>
  public interface IDbTransactionInterceptor : IDbInterceptor
  {
    /// <summary>
    /// Called before <see cref="P:System.Data.Common.DbTransaction.Connection" /> is retrieved.
    /// </summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ConnectionGetting(
      DbTransaction transaction,
      DbTransactionInterceptionContext<DbConnection> interceptionContext);

    /// <summary>
    /// Called after <see cref="P:System.Data.Common.DbTransaction.Connection" /> is retrieved.
    /// </summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ConnectionGot(
      DbTransaction transaction,
      DbTransactionInterceptionContext<DbConnection> interceptionContext);

    /// <summary>
    /// Called before <see cref="P:System.Data.Common.DbTransaction.IsolationLevel" /> is retrieved.
    /// </summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void IsolationLevelGetting(
      DbTransaction transaction,
      DbTransactionInterceptionContext<IsolationLevel> interceptionContext);

    /// <summary>
    /// Called after <see cref="P:System.Data.Common.DbTransaction.IsolationLevel" /> is retrieved.
    /// </summary>
    /// <param name="transaction">The transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void IsolationLevelGot(
      DbTransaction transaction,
      DbTransactionInterceptionContext<IsolationLevel> interceptionContext);

    /// <summary>
    /// This method is called before <see cref="M:System.Data.Common.DbTransaction.Commit" /> is invoked.
    /// </summary>
    /// <param name="transaction">The transaction being commited.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Committing(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext);

    /// <summary>
    /// This method is called after <see cref="M:System.Data.Common.DbTransaction.Commit" /> is invoked.
    /// </summary>
    /// <param name="transaction">The transaction that was commited.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Committed(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext);

    /// <summary>
    /// This method is called before <see cref="M:System.Data.Common.DbTransaction.Dispose" /> is invoked.
    /// </summary>
    /// <param name="transaction">The transaction being disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Disposing(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext);

    /// <summary>
    /// This method is called after <see cref="M:System.Data.Common.DbTransaction.Dispose" /> is invoked.
    /// </summary>
    /// <param name="transaction">The transaction that was disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Disposed(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext);

    /// <summary>
    /// This method is called before <see cref="M:System.Data.Common.DbTransaction.Rollback" /> is invoked.
    /// </summary>
    /// <param name="transaction">The transaction being rolled back.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void RollingBack(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext);

    /// <summary>
    /// This method is called after <see cref="M:System.Data.Common.DbTransaction.Rollback" /> is invoked.
    /// </summary>
    /// <param name="transaction">The transaction that was rolled back.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void RolledBack(
      DbTransaction transaction,
      DbTransactionInterceptionContext interceptionContext);
  }
}
