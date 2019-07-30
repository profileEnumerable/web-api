// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.CancelableDbCommandDispatcher
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;

namespace System.Data.Entity.Infrastructure.Interception
{
  internal class CancelableDbCommandDispatcher
  {
    private readonly System.Data.Entity.Infrastructure.Interception.InternalDispatcher<ICancelableDbCommandInterceptor> _internalDispatcher = new System.Data.Entity.Infrastructure.Interception.InternalDispatcher<ICancelableDbCommandInterceptor>();

    public System.Data.Entity.Infrastructure.Interception.InternalDispatcher<ICancelableDbCommandInterceptor> InternalDispatcher
    {
      get
      {
        return this._internalDispatcher;
      }
    }

    public virtual bool Executing(DbCommand command, DbInterceptionContext interceptionContext)
    {
      return this._internalDispatcher.Dispatch<bool>(true, (Func<bool, ICancelableDbCommandInterceptor, bool>) ((b, i) =>
      {
        if (i.CommandExecuting(command, interceptionContext))
          return b;
        return false;
      }));
    }
  }
}
