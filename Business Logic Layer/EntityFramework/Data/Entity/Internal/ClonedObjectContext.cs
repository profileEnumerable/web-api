// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ClonedObjectContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal.MockingProxies;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Internal
{
  internal class ClonedObjectContext : IDisposable
  {
    private ObjectContextProxy _objectContext;
    private readonly bool _connectionCloned;
    private readonly EntityConnectionProxy _clonedEntityConnection;

    protected ClonedObjectContext()
    {
    }

    public ClonedObjectContext(
      ObjectContextProxy objectContext,
      DbConnection connection,
      string connectionString,
      bool transferLoadedAssemblies = true)
    {
      if (connection == null || connection.State != ConnectionState.Open)
      {
        connection = DbProviderServices.GetProviderFactory(objectContext.Connection.StoreConnection).CreateConnection();
        DbInterception.Dispatch.Connection.SetConnectionString(connection, new DbConnectionPropertyInterceptionContext<string>().WithValue(connectionString));
        this._connectionCloned = true;
      }
      this._clonedEntityConnection = objectContext.Connection.CreateNew(connection);
      this._objectContext = objectContext.CreateNew(this._clonedEntityConnection);
      this._objectContext.CopyContextOptions(objectContext);
      if (!string.IsNullOrWhiteSpace(objectContext.DefaultContainerName))
        this._objectContext.DefaultContainerName = objectContext.DefaultContainerName;
      if (!transferLoadedAssemblies)
        return;
      this.TransferLoadedAssemblies(objectContext);
    }

    public virtual ObjectContextProxy ObjectContext
    {
      get
      {
        return this._objectContext;
      }
    }

    public virtual DbConnection Connection
    {
      get
      {
        return this._objectContext.Connection.StoreConnection;
      }
    }

    private void TransferLoadedAssemblies(ObjectContextProxy source)
    {
      IEnumerable<GlobalItem> objectItemCollection = source.GetObjectItemCollection();
      foreach (Assembly assembly in objectItemCollection.Where<GlobalItem>((Func<GlobalItem, bool>) (i =>
      {
        if (!(i is EntityType))
          return i is ComplexType;
        return true;
      })).Select<GlobalItem, Assembly>((Func<GlobalItem, Assembly>) (i => source.GetClrType((StructuralType) i).Assembly())).Union<Assembly>(objectItemCollection.OfType<EnumType>().Select<EnumType, Assembly>((Func<EnumType, Assembly>) (i => source.GetClrType(i).Assembly()))).Distinct<Assembly>())
        this._objectContext.LoadFromAssembly(assembly);
    }

    public void Dispose()
    {
      if (this._objectContext == null)
        return;
      ObjectContextProxy objectContext = this._objectContext;
      DbConnection connection = this.Connection;
      this._objectContext = (ObjectContextProxy) null;
      objectContext.Dispose();
      this._clonedEntityConnection.Dispose();
      if (!this._connectionCloned)
        return;
      DbInterception.Dispatch.Connection.Dispose(connection, new DbInterceptionContext());
    }
  }
}
