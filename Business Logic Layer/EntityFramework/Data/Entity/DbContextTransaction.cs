// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.DbContextTransaction
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity
{
  /// <summary>
  /// Wraps access to the transaction object on the underlying store connection and ensures that the
  /// Entity Framework executes commands on the database within the context of that transaction.
  /// An instance of this class is retrieved by calling BeginTransaction() on the <see cref="T:System.Data.Entity.DbContext" />
  /// <see cref="T:System.Data.Entity.Database" />
  /// object.
  /// </summary>
  public class DbContextTransaction : IDisposable
  {
    private readonly EntityConnection _connection;
    private readonly EntityTransaction _entityTransaction;
    private bool _shouldCloseConnection;
    private bool _isDisposed;

    internal DbContextTransaction(EntityConnection connection)
    {
      this._connection = connection;
      this.EnsureOpenConnection();
      this._entityTransaction = this._connection.BeginTransaction();
    }

    internal DbContextTransaction(EntityConnection connection, IsolationLevel isolationLevel)
    {
      this._connection = connection;
      this.EnsureOpenConnection();
      this._entityTransaction = this._connection.BeginTransaction(isolationLevel);
    }

    internal DbContextTransaction(EntityTransaction transaction)
    {
      this._connection = transaction.Connection;
      this.EnsureOpenConnection();
      this._entityTransaction = transaction;
    }

    private void EnsureOpenConnection()
    {
      if (ConnectionState.Open == this._connection.State)
        return;
      this._connection.Open();
      this._shouldCloseConnection = true;
    }

    /// <summary>
    /// Gets the database (store) transaction that is underlying this context transaction.
    /// </summary>
    public DbTransaction UnderlyingTransaction
    {
      get
      {
        return this._entityTransaction.StoreTransaction;
      }
    }

    /// <summary>Commits the underlying store transaction</summary>
    public void Commit()
    {
      this._entityTransaction.Commit();
    }

    /// <summary>Rolls back the underlying store transaction</summary>
    public void Rollback()
    {
      this._entityTransaction.Rollback();
    }

    /// <summary>
    /// Cleans up this transaction object and ensures the Entity Framework
    /// is no longer using that transaction.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Releases the resources used by this transaction object
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this._isDisposed)
        return;
      this._connection.ClearCurrentTransaction();
      this._entityTransaction.Dispose();
      if (this._shouldCloseConnection && this._connection.State != ConnectionState.Closed)
        this._connection.Close();
      this._isDisposed = true;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
