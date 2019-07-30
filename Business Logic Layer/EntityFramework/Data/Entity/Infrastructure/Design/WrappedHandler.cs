// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Design.WrappedHandler
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Infrastructure.Design
{
  internal class WrappedHandler : IResultHandler
  {
    private readonly IResultHandler _resultHandler;

    public WrappedHandler(object handler)
    {
      HandlerBase handlerBase = handler as HandlerBase ?? new ForwardingProxy<HandlerBase>(handler).GetTransparentProxy();
      this._resultHandler = handler as IResultHandler ?? (handlerBase.ImplementsContract(typeof (IResultHandler).FullName) ? new ForwardingProxy<IResultHandler>(handler).GetTransparentProxy() : (IResultHandler) null);
    }

    public void SetResult(object value)
    {
      if (this._resultHandler == null)
        return;
      this._resultHandler.SetResult(value);
    }
  }
}
