// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DbCommandTreeDispatcher
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;

namespace System.Data.Entity.Infrastructure.Interception
{
  internal class DbCommandTreeDispatcher
  {
    private readonly System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbCommandTreeInterceptor> _internalDispatcher = new System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbCommandTreeInterceptor>();

    public System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbCommandTreeInterceptor> InternalDispatcher
    {
      get
      {
        return this._internalDispatcher;
      }
    }

    public virtual DbCommandTree Created(
      DbCommandTree commandTree,
      DbInterceptionContext interceptionContext)
    {
      return this._internalDispatcher.Dispatch<DbCommandTreeInterceptionContext, DbCommandTree>(commandTree, new DbCommandTreeInterceptionContext(interceptionContext), (Action<IDbCommandTreeInterceptor, DbCommandTreeInterceptionContext>) ((i, c) => i.TreeCreated(c)));
    }
  }
}
