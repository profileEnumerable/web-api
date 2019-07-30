// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.EagerInternalConnection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;

namespace System.Data.Entity.Internal
{
  internal class EagerInternalConnection : InternalConnection
  {
    private readonly bool _connectionOwned;

    public EagerInternalConnection(
      DbContext context,
      DbConnection existingConnection,
      bool connectionOwned)
      : base(new DbInterceptionContext().WithDbContext(context))
    {
      this.UnderlyingConnection = existingConnection;
      this._connectionOwned = connectionOwned;
      this.OnConnectionInitialized();
    }

    public override DbConnectionStringOrigin ConnectionStringOrigin
    {
      get
      {
        return DbConnectionStringOrigin.UserCode;
      }
    }

    public override void Dispose()
    {
      if (!this._connectionOwned)
        return;
      if (this.UnderlyingConnection is EntityConnection)
        this.UnderlyingConnection.Dispose();
      else
        DbInterception.Dispatch.Connection.Dispose(this.UnderlyingConnection, this.InterceptionContext);
    }
  }
}
