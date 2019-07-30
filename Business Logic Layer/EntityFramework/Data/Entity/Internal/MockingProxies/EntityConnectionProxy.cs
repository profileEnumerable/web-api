// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.MockingProxies.EntityConnectionProxy
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal.MockingProxies
{
  internal class EntityConnectionProxy
  {
    private readonly EntityConnection _entityConnection;

    protected EntityConnectionProxy()
    {
    }

    public EntityConnectionProxy(EntityConnection entityConnection)
    {
      this._entityConnection = entityConnection;
    }

    public static implicit operator EntityConnection(EntityConnectionProxy proxy)
    {
      return proxy._entityConnection;
    }

    public virtual DbConnection StoreConnection
    {
      get
      {
        return this._entityConnection.StoreConnection;
      }
    }

    public virtual void Dispose()
    {
      this._entityConnection.Dispose();
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public virtual EntityConnectionProxy CreateNew(DbConnection storeConnection)
    {
      EntityConnection entityConnection = new EntityConnection(this._entityConnection.GetMetadataWorkspace(), storeConnection);
      EntityTransaction currentTransaction = this._entityConnection.CurrentTransaction;
      if (currentTransaction != null && DbInterception.Dispatch.Transaction.GetConnection(currentTransaction.StoreTransaction, this._entityConnection.InterceptionContext) == storeConnection)
        entityConnection.UseStoreTransaction(currentTransaction.StoreTransaction);
      return new EntityConnectionProxy(entityConnection);
    }
  }
}
