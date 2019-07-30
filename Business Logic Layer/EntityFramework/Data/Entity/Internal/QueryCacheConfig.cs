// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.QueryCacheConfig
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal.ConfigFile;

namespace System.Data.Entity.Internal
{
  internal class QueryCacheConfig
  {
    private const int DefaultSize = 1000;
    private const int DefaultCleaningIntervalInSeconds = 60;
    private readonly EntityFrameworkSection _entityFrameworkSection;

    public QueryCacheConfig(EntityFrameworkSection entityFrameworkSection)
    {
      this._entityFrameworkSection = entityFrameworkSection;
    }

    public int GetQueryCacheSize()
    {
      int size = this._entityFrameworkSection.QueryCache.Size;
      if (size == 0)
        return 1000;
      return size;
    }

    public int GetCleaningIntervalInSeconds()
    {
      int intervalInSeconds = this._entityFrameworkSection.QueryCache.CleaningIntervalInSeconds;
      if (intervalInSeconds == 0)
        return 60;
      return intervalInSeconds;
    }
  }
}
