// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.CancelableEntityConnectionDispatcher
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.EntityClient;

namespace System.Data.Entity.Infrastructure.Interception
{
  internal class CancelableEntityConnectionDispatcher
  {
    private readonly System.Data.Entity.Infrastructure.Interception.InternalDispatcher<ICancelableEntityConnectionInterceptor> _internalDispatcher = new System.Data.Entity.Infrastructure.Interception.InternalDispatcher<ICancelableEntityConnectionInterceptor>();

    public System.Data.Entity.Infrastructure.Interception.InternalDispatcher<ICancelableEntityConnectionInterceptor> InternalDispatcher
    {
      get
      {
        return this._internalDispatcher;
      }
    }

    public virtual bool Opening(
      EntityConnection entityConnection,
      DbInterceptionContext interceptionContext)
    {
      return this._internalDispatcher.Dispatch<bool>(true, (Func<bool, ICancelableEntityConnectionInterceptor, bool>) ((b, i) =>
      {
        if (i.ConnectionOpening(entityConnection, interceptionContext))
          return b;
        return false;
      }));
    }
  }
}
