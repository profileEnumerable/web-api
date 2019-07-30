// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.InternalConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class InternalConfiguration
  {
    private CompositeResolver<ResolverChain, ResolverChain> _resolvers;
    private RootDependencyResolver _rootResolver;
    private readonly Func<DbDispatchers> _dispatchers;
    private bool _isLocked;

    public InternalConfiguration(
      ResolverChain appConfigChain = null,
      ResolverChain normalResolverChain = null,
      RootDependencyResolver rootResolver = null,
      AppConfigDependencyResolver appConfigResolver = null,
      Func<DbDispatchers> dispatchers = null)
    {
      this._rootResolver = rootResolver ?? new RootDependencyResolver();
      this._resolvers = new CompositeResolver<ResolverChain, ResolverChain>(appConfigChain ?? new ResolverChain(), normalResolverChain ?? new ResolverChain());
      this._resolvers.Second.Add((IDbDependencyResolver) this._rootResolver);
      this._resolvers.First.Add((IDbDependencyResolver) (appConfigResolver ?? new AppConfigDependencyResolver(AppConfig.DefaultInstance, this, (ProviderServicesFactory) null)));
      this._dispatchers = dispatchers ?? (Func<DbDispatchers>) (() => DbInterception.Dispatch);
    }

    public static InternalConfiguration Instance
    {
      get
      {
        return DbConfigurationManager.Instance.GetConfiguration();
      }
      set
      {
        DbConfigurationManager.Instance.SetConfiguration(value);
      }
    }

    public virtual void Lock()
    {
      List<IDbInterceptor> list = this.DependencyResolver.GetServices<IDbInterceptor>().ToList<IDbInterceptor>();
      list.Each<IDbInterceptor>(new Action<IDbInterceptor>(this._dispatchers().AddInterceptor));
      DbConfigurationManager.Instance.OnLoaded(this);
      this._isLocked = true;
      this.DependencyResolver.GetServices<IDbInterceptor>().Except<IDbInterceptor>((IEnumerable<IDbInterceptor>) list).Each<IDbInterceptor>(new Action<IDbInterceptor>(this._dispatchers().AddInterceptor));
    }

    public void DispatchLoadedInterceptors(DbConfigurationLoadedEventArgs loadedEventArgs)
    {
      this._dispatchers().Configuration.Loaded(loadedEventArgs, new DbInterceptionContext());
    }

    public virtual void AddAppConfigResolver(IDbDependencyResolver resolver)
    {
      this._resolvers.First.Add(resolver);
    }

    public virtual void AddDependencyResolver(
      IDbDependencyResolver resolver,
      bool overrideConfigFile = false)
    {
      (overrideConfigFile ? this._resolvers.First : this._resolvers.Second).Add(resolver);
    }

    public virtual void AddDefaultResolver(IDbDependencyResolver resolver)
    {
      this._rootResolver.AddDefaultResolver(resolver);
    }

    public virtual void SetDefaultProviderServices(
      DbProviderServices provider,
      string invariantName)
    {
      this._rootResolver.SetDefaultProviderServices(provider, invariantName);
    }

    public virtual void RegisterSingleton<TService>(TService instance) where TService : class
    {
      this.AddDependencyResolver((IDbDependencyResolver) new SingletonDependencyResolver<TService>(instance, (object) null), false);
    }

    public virtual void RegisterSingleton<TService>(TService instance, object key) where TService : class
    {
      this.AddDependencyResolver((IDbDependencyResolver) new SingletonDependencyResolver<TService>(instance, key), false);
    }

    public virtual void RegisterSingleton<TService>(
      TService instance,
      Func<object, bool> keyPredicate)
      where TService : class
    {
      this.AddDependencyResolver((IDbDependencyResolver) new SingletonDependencyResolver<TService>(instance, keyPredicate), false);
    }

    public virtual TService GetService<TService>(object key)
    {
      return this._resolvers.GetService<TService>(key);
    }

    public virtual IDbDependencyResolver DependencyResolver
    {
      get
      {
        return (IDbDependencyResolver) this._resolvers;
      }
    }

    public virtual RootDependencyResolver RootResolver
    {
      get
      {
        return this._rootResolver;
      }
    }

    public virtual void SwitchInRootResolver(RootDependencyResolver value)
    {
      ResolverChain secondResolver = new ResolverChain();
      secondResolver.Add((IDbDependencyResolver) value);
      this._resolvers.Second.Resolvers.Skip<IDbDependencyResolver>(1).Each<IDbDependencyResolver>(new Action<IDbDependencyResolver>(secondResolver.Add));
      this._rootResolver = value;
      this._resolvers = new CompositeResolver<ResolverChain, ResolverChain>(this._resolvers.First, secondResolver);
    }

    public virtual IDbDependencyResolver ResolverSnapshot
    {
      get
      {
        ResolverChain resolverChain = new ResolverChain();
        this._resolvers.Second.Resolvers.Each<IDbDependencyResolver>(new Action<IDbDependencyResolver>(resolverChain.Add));
        this._resolvers.First.Resolvers.Each<IDbDependencyResolver>(new Action<IDbDependencyResolver>(resolverChain.Add));
        return (IDbDependencyResolver) resolverChain;
      }
    }

    public virtual DbConfiguration Owner { get; set; }

    public virtual void CheckNotLocked(string memberName)
    {
      if (this._isLocked)
        throw new InvalidOperationException(Strings.ConfigurationLocked((object) memberName));
    }
  }
}
