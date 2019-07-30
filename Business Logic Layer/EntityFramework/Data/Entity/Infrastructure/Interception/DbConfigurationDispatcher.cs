// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DbConfigurationDispatcher
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;

namespace System.Data.Entity.Infrastructure.Interception
{
  internal class DbConfigurationDispatcher
  {
    private readonly System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbConfigurationInterceptor> _internalDispatcher = new System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbConfigurationInterceptor>();

    public System.Data.Entity.Infrastructure.Interception.InternalDispatcher<IDbConfigurationInterceptor> InternalDispatcher
    {
      get
      {
        return this._internalDispatcher;
      }
    }

    public virtual void Loaded(
      DbConfigurationLoadedEventArgs loadedEventArgs,
      DbInterceptionContext interceptionContext)
    {
      DbConfigurationInterceptionContext clonedInterceptionContext = new DbConfigurationInterceptionContext(interceptionContext);
      this._internalDispatcher.Dispatch((Action<IDbConfigurationInterceptor>) (i => i.Loaded(loadedEventArgs, clonedInterceptionContext)));
    }
  }
}
