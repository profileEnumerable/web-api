// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.DefaultModelCacheKeyFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Internal
{
  internal sealed class DefaultModelCacheKeyFactory
  {
    public IDbModelCacheKey Create(DbContext context)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      string customKey = (string) null;
      IDbModelCacheKeyProvider cacheKeyProvider = context as IDbModelCacheKeyProvider;
      if (cacheKeyProvider != null)
        customKey = cacheKeyProvider.CacheKey;
      return (IDbModelCacheKey) new DefaultModelCacheKey(context.GetType(), context.InternalContext.ProviderName, context.InternalContext.ProviderFactory.GetType(), customKey);
    }
  }
}
