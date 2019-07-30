// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.MappingViews.DefaultDbMappingViewCacheFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Infrastructure.MappingViews
{
  internal class DefaultDbMappingViewCacheFactory : DbMappingViewCacheFactory
  {
    private readonly Type _cacheType;

    public DefaultDbMappingViewCacheFactory(Type cacheType)
    {
      this._cacheType = cacheType;
    }

    public override DbMappingViewCache Create(
      string conceptualModelContainerName,
      string storeModelContainerName)
    {
      return (DbMappingViewCache) Activator.CreateInstance(this._cacheType);
    }

    public override int GetHashCode()
    {
      return this._cacheType.GetHashCode() * 397 ^ typeof (DefaultDbMappingViewCacheFactory).GetHashCode();
    }

    public override bool Equals(object obj)
    {
      DefaultDbMappingViewCacheFactory viewCacheFactory = obj as DefaultDbMappingViewCacheFactory;
      if (viewCacheFactory != null)
        return object.ReferenceEquals((object) viewCacheFactory._cacheType, (object) this._cacheType);
      return false;
    }
  }
}
