// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.RootDependencyResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.Internal;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class RootDependencyResolver : IDbDependencyResolver
  {
    private readonly ResolverChain _defaultProviderResolvers = new ResolverChain();
    private readonly ResolverChain _defaultResolvers = new ResolverChain();
    private readonly ResolverChain _resolvers = new ResolverChain();
    private readonly DatabaseInitializerResolver _databaseInitializerResolver;

    public RootDependencyResolver()
      : this(new DefaultProviderServicesResolver(), new DatabaseInitializerResolver())
    {
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Reliability", "CA2000: Dispose objects before losing scope")]
    public RootDependencyResolver(
      DefaultProviderServicesResolver defaultProviderServicesResolver,
      DatabaseInitializerResolver databaseInitializerResolver)
    {
      this._databaseInitializerResolver = databaseInitializerResolver;
      this._resolvers.Add((IDbDependencyResolver) new TransactionContextInitializerResolver());
      this._resolvers.Add((IDbDependencyResolver) this._databaseInitializerResolver);
      this._resolvers.Add((IDbDependencyResolver) new DefaultExecutionStrategyResolver());
      this._resolvers.Add((IDbDependencyResolver) new CachingDependencyResolver((IDbDependencyResolver) defaultProviderServicesResolver));
      this._resolvers.Add((IDbDependencyResolver) new CachingDependencyResolver((IDbDependencyResolver) new DefaultProviderFactoryResolver()));
      this._resolvers.Add((IDbDependencyResolver) new CachingDependencyResolver((IDbDependencyResolver) new DefaultInvariantNameResolver()));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<IDbConnectionFactory>((IDbConnectionFactory) new SqlConnectionFactory()));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<Func<DbContext, IDbModelCacheKey>>(new Func<DbContext, IDbModelCacheKey>(new DefaultModelCacheKeyFactory().Create)));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<IManifestTokenResolver>((IManifestTokenResolver) new DefaultManifestTokenResolver()));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<Func<DbConnection, string, HistoryContext>>(HistoryContext.DefaultFactory));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<IPluralizationService>((IPluralizationService) new EnglishPluralizationService()));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<AttributeProvider>(new AttributeProvider()));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<Func<DbContext, Action<string>, DatabaseLogFormatter>>((Func<DbContext, Action<string>, DatabaseLogFormatter>) ((c, w) => new DatabaseLogFormatter(c, w))));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<Func<TransactionHandler>>((Func<TransactionHandler>) (() => (TransactionHandler) new DefaultTransactionHandler()), (Func<object, bool>) (k => k is ExecutionStrategyKey)));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<IDbProviderFactoryResolver>((IDbProviderFactoryResolver) new DefaultDbProviderFactoryResolver()));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<Func<IMetadataAnnotationSerializer>>((Func<IMetadataAnnotationSerializer>) (() => (IMetadataAnnotationSerializer) new ClrTypeAnnotationSerializer()), (object) "ClrType"));
      this._resolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<Func<IMetadataAnnotationSerializer>>((Func<IMetadataAnnotationSerializer>) (() => (IMetadataAnnotationSerializer) new IndexAnnotationSerializer()), (object) "Index"));
    }

    public DatabaseInitializerResolver DatabaseInitializerResolver
    {
      get
      {
        return this._databaseInitializerResolver;
      }
    }

    public virtual object GetService(Type type, object key)
    {
      return this._defaultResolvers.GetService(type, key) ?? this._defaultProviderResolvers.GetService(type, key) ?? this._resolvers.GetService(type, key);
    }

    public virtual void AddDefaultResolver(IDbDependencyResolver resolver)
    {
      this._defaultResolvers.Add(resolver);
    }

    public virtual void SetDefaultProviderServices(
      DbProviderServices provider,
      string invariantName)
    {
      this._defaultProviderResolvers.Add((IDbDependencyResolver) new SingletonDependencyResolver<DbProviderServices>(provider, (object) invariantName));
      this._defaultProviderResolvers.Add((IDbDependencyResolver) provider);
    }

    public IEnumerable<object> GetServices(Type type, object key)
    {
      return this._defaultResolvers.GetServices(type, key).Concat<object>(this._resolvers.GetServices(type, key));
    }
  }
}
