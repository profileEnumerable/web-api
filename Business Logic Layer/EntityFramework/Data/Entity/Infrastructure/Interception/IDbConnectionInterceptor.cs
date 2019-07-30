// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// An object that implements this interface can be registered with <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> to
  /// receive notifications when Entity Framework performs operations on a <see cref="T:System.Data.Common.DbTransaction" />.
  /// </summary>
  /// <remarks>
  /// Interceptors can also be registered in the config file of the application.
  /// See http://go.microsoft.com/fwlink/?LinkId=260883 for more information about Entity Framework configuration.
  /// </remarks>
  public interface IDbConnectionInterceptor : IDbInterceptor
  {
    /// <summary>
    /// Called before <see cref="M:System.Data.Common.DbConnection.BeginTransaction(System.Data.IsolationLevel)" /> is invoked.
    /// </summary>
    /// <param name="connection">The connection beginning the transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void BeginningTransaction(
      DbConnection connection,
      BeginTransactionInterceptionContext interceptionContext);

    /// <summary>
    /// Called after <see cref="M:System.Data.Common.DbConnection.BeginTransaction(System.Data.IsolationLevel)" /> is invoked.
    /// The transaction used by Entity Framework can be changed by setting
    /// <see cref="P:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext`1.Result" />.
    /// </summary>
    /// <param name="connection">The connection that began the transaction.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void BeganTransaction(
      DbConnection connection,
      BeginTransactionInterceptionContext interceptionContext);

    /// <summary>
    /// Called before <see cref="M:System.Data.Common.DbConnection.Close" /> is invoked.
    /// </summary>
    /// <param name="connection">The connection being closed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Closing(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext);

    /// <summary>
    /// Called after <see cref="M:System.Data.Common.DbConnection.Close" /> is invoked.
    /// </summary>
    /// <param name="connection">The connection that was closed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Closed(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext);

    /// <summary>
    /// Called before <see cref="P:System.Data.Common.DbConnection.ConnectionString" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ConnectionStringGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called after <see cref="P:System.Data.Common.DbConnection.ConnectionString" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ConnectionStringGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called before <see cref="P:System.Data.Common.DbConnection.ConnectionString" /> is set.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ConnectionStringSetting(
      DbConnection connection,
      DbConnectionPropertyInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called after <see cref="P:System.Data.Common.DbConnection.ConnectionString" /> is set.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ConnectionStringSet(
      DbConnection connection,
      DbConnectionPropertyInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called before <see cref="P:System.Data.Common.DbConnection.ConnectionTimeout" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ConnectionTimeoutGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<int> interceptionContext);

    /// <summary>
    /// Called after <see cref="P:System.Data.Common.DbConnection.ConnectionTimeout" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ConnectionTimeoutGot(
      DbConnection connection,
      DbConnectionInterceptionContext<int> interceptionContext);

    /// <summary>
    /// Called before <see cref="P:System.Data.Common.DbConnection.Database" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void DatabaseGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called after <see cref="P:System.Data.Common.DbConnection.Database" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void DatabaseGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called before <see cref="P:System.Data.Common.DbConnection.DataSource" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void DataSourceGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called after <see cref="P:System.Data.Common.DbConnection.DataSource" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void DataSourceGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called before <see cref="M:System.ComponentModel.Component.Dispose" /> is invoked.
    /// </summary>
    /// <param name="connection">The connection being disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Disposing(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext);

    /// <summary>
    /// Called after <see cref="M:System.ComponentModel.Component.Dispose" /> is invoked.
    /// </summary>
    /// <param name="connection">The connection that was disposed.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Disposed(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext);

    /// <summary>
    /// Called before <see cref="M:System.Data.Common.DbConnection.EnlistTransaction(System.Transactions.Transaction)" /> is invoked.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void EnlistingTransaction(
      DbConnection connection,
      EnlistTransactionInterceptionContext interceptionContext);

    /// <summary>
    /// Called after <see cref="M:System.Data.Common.DbConnection.EnlistTransaction(System.Transactions.Transaction)" /> is invoked.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void EnlistedTransaction(
      DbConnection connection,
      EnlistTransactionInterceptionContext interceptionContext);

    /// <summary>
    /// Called before <see cref="M:System.Data.Common.DbConnection.Open" /> or its async counterpart is invoked.
    /// </summary>
    /// <param name="connection">The connection being opened.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Opening(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext);

    /// <summary>
    /// Called after <see cref="M:System.Data.Common.DbConnection.Open" /> or its async counterpart is invoked.
    /// </summary>
    /// <param name="connection">The connection that was opened.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void Opened(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext);

    /// <summary>
    /// Called before <see cref="P:System.Data.Common.DbConnection.ServerVersion" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ServerVersionGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called after <see cref="P:System.Data.Common.DbConnection.ServerVersion" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void ServerVersionGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext);

    /// <summary>
    /// Called before <see cref="P:System.Data.Common.DbConnection.State" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void StateGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<ConnectionState> interceptionContext);

    /// <summary>
    /// Called after <see cref="P:System.Data.Common.DbConnection.State" /> is retrieved.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="interceptionContext">Contextual information associated with the call.</param>
    void StateGot(
      DbConnection connection,
      DbConnectionInterceptionContext<ConnectionState> interceptionContext);
  }
}
