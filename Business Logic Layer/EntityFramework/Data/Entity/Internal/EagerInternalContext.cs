// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.EagerInternalContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal
{
  internal class EagerInternalContext : InternalContext
  {
    private readonly ObjectContext _objectContext;
    private readonly bool _objectContextOwned;
    private readonly string _originalConnectionString;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public EagerInternalContext(DbContext owner)
      : base(owner, (Lazy<DbDispatchers>) null)
    {
    }

    public EagerInternalContext(
      DbContext owner,
      ObjectContext objectContext,
      bool objectContextOwned)
      : base(owner, (Lazy<DbDispatchers>) null)
    {
      this._objectContext = objectContext;
      this._objectContextOwned = objectContextOwned;
      this._originalConnectionString = InternalConnection.GetStoreConnectionString(this._objectContext.Connection);
      this._objectContext.InterceptionContext = this._objectContext.InterceptionContext.WithDbContext(owner);
      this.ResetDbSets();
      this._objectContext.InitializeMappingViewCacheFactory(this.Owner);
    }

    public override ObjectContext ObjectContext
    {
      get
      {
        this.Initialize();
        return this.ObjectContextInUse;
      }
    }

    public override ObjectContext GetObjectContextWithoutDatabaseInitialization()
    {
      this.InitializeContext();
      return this.ObjectContextInUse;
    }

    private ObjectContext ObjectContextInUse
    {
      get
      {
        return this.TempObjectContext ?? this._objectContext;
      }
    }

    protected override void InitializeContext()
    {
      this.CheckContextNotDisposed();
    }

    public override void MarkDatabaseNotInitialized()
    {
    }

    public override void MarkDatabaseInitialized()
    {
    }

    protected override void InitializeDatabase()
    {
    }

    public override IDatabaseInitializer<DbContext> DefaultInitializer
    {
      get
      {
        return (IDatabaseInitializer<DbContext>) null;
      }
    }

    public override void DisposeContext(bool disposing)
    {
      if (this.IsDisposed)
        return;
      base.DisposeContext(disposing);
      if (!disposing || !this._objectContextOwned)
        return;
      this._objectContext.Dispose();
    }

    public override DbConnection Connection
    {
      get
      {
        this.CheckContextNotDisposed();
        return ((EntityConnection) this._objectContext.Connection).StoreConnection;
      }
    }

    public override string OriginalConnectionString
    {
      get
      {
        return this._originalConnectionString;
      }
    }

    public override DbConnectionStringOrigin ConnectionStringOrigin
    {
      get
      {
        return DbConnectionStringOrigin.UserCode;
      }
    }

    public override void OverrideConnection(IInternalConnection connection)
    {
      throw Error.EagerInternalContext_CannotSetConnectionInfo();
    }

    public override bool EnsureTransactionsForFunctionsAndCommands
    {
      get
      {
        return this.ObjectContextInUse.ContextOptions.EnsureTransactionsForFunctionsAndCommands;
      }
      set
      {
        this.ObjectContextInUse.ContextOptions.EnsureTransactionsForFunctionsAndCommands = value;
      }
    }

    public override bool LazyLoadingEnabled
    {
      get
      {
        return this.ObjectContextInUse.ContextOptions.LazyLoadingEnabled;
      }
      set
      {
        this.ObjectContextInUse.ContextOptions.LazyLoadingEnabled = value;
      }
    }

    public override bool ProxyCreationEnabled
    {
      get
      {
        return this.ObjectContextInUse.ContextOptions.ProxyCreationEnabled;
      }
      set
      {
        this.ObjectContextInUse.ContextOptions.ProxyCreationEnabled = value;
      }
    }

    public override bool UseDatabaseNullSemantics
    {
      get
      {
        return !this.ObjectContextInUse.ContextOptions.UseCSharpNullComparisonBehavior;
      }
      set
      {
        this.ObjectContextInUse.ContextOptions.UseCSharpNullComparisonBehavior = !value;
      }
    }

    public override int? CommandTimeout
    {
      get
      {
        return this.ObjectContextInUse.CommandTimeout;
      }
      set
      {
        this.ObjectContextInUse.CommandTimeout = value;
      }
    }
  }
}
