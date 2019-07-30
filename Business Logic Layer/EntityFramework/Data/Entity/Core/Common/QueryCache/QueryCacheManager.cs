// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.QueryCache.QueryCacheManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Data.Entity.Core.Common.QueryCache
{
  internal class QueryCacheManager : IDisposable
  {
    private static readonly int[] _agingFactor = new int[6]
    {
      1,
      1,
      2,
      4,
      8,
      16
    };
    private static readonly int _agingMaxIndex = QueryCacheManager._agingFactor.Length - 1;
    private readonly object _cacheDataLock = new object();
    private readonly Dictionary<QueryCacheKey, QueryCacheEntry> _cacheData = new Dictionary<QueryCacheKey, QueryCacheEntry>(32);
    private readonly int _maxNumberOfEntries;
    private readonly int _sweepingTriggerHighMark;
    private readonly QueryCacheManager.EvictionTimer _evictionTimer;

    internal static QueryCacheManager Create()
    {
      QueryCacheConfig queryCache = AppConfig.DefaultInstance.QueryCache;
      return new QueryCacheManager(queryCache.GetQueryCacheSize(), 0.8f, queryCache.GetCleaningIntervalInSeconds() * 1000);
    }

    private QueryCacheManager(int maximumSize, float loadFactor, int recycleMillis)
    {
      this._maxNumberOfEntries = maximumSize;
      this._sweepingTriggerHighMark = (int) ((double) this._maxNumberOfEntries * (double) loadFactor);
      this._evictionTimer = new QueryCacheManager.EvictionTimer(this, recycleMillis);
    }

    internal bool TryLookupAndAdd(
      QueryCacheEntry inQueryCacheEntry,
      out QueryCacheEntry outQueryCacheEntry)
    {
      outQueryCacheEntry = (QueryCacheEntry) null;
      lock (this._cacheDataLock)
      {
        if (!this._cacheData.TryGetValue(inQueryCacheEntry.QueryCacheKey, out outQueryCacheEntry))
        {
          this._cacheData.Add(inQueryCacheEntry.QueryCacheKey, inQueryCacheEntry);
          if (this._cacheData.Count > this._sweepingTriggerHighMark)
            this._evictionTimer.Start();
          return false;
        }
        outQueryCacheEntry.QueryCacheKey.UpdateHit();
        return true;
      }
    }

    internal bool TryCacheLookup<TK, TE>(TK key, out TE value) where TK : QueryCacheKey
    {
      value = default (TE);
      QueryCacheEntry queryCacheEntry = (QueryCacheEntry) null;
      bool flag = this.TryInternalCacheLookup((QueryCacheKey) key, out queryCacheEntry);
      if (flag)
        value = (TE) queryCacheEntry.GetTarget();
      return flag;
    }

    internal void Clear()
    {
      lock (this._cacheDataLock)
        this._cacheData.Clear();
    }

    private bool TryInternalCacheLookup(
      QueryCacheKey queryCacheKey,
      out QueryCacheEntry queryCacheEntry)
    {
      queryCacheEntry = (QueryCacheEntry) null;
      bool flag = false;
      lock (this._cacheDataLock)
        flag = this._cacheData.TryGetValue(queryCacheKey, out queryCacheEntry);
      if (flag)
        queryCacheEntry.QueryCacheKey.UpdateHit();
      return flag;
    }

    private static void CacheRecyclerHandler(object state)
    {
      ((QueryCacheManager) state).SweepCache();
    }

    private void SweepCache()
    {
      if (!this._evictionTimer.Suspend())
        return;
      bool flag = false;
      lock (this._cacheDataLock)
      {
        if (this._cacheData.Count > this._sweepingTriggerHighMark)
        {
          uint num = 0;
          List<QueryCacheKey> queryCacheKeyList = new List<QueryCacheKey>(this._cacheData.Count);
          queryCacheKeyList.AddRange((IEnumerable<QueryCacheKey>) this._cacheData.Keys);
          for (int index1 = 0; index1 < queryCacheKeyList.Count; ++index1)
          {
            if (queryCacheKeyList[index1].HitCount == 0U)
            {
              this._cacheData.Remove(queryCacheKeyList[index1]);
              ++num;
            }
            else
            {
              int index2 = queryCacheKeyList[index1].AgingIndex + 1;
              if (index2 > QueryCacheManager._agingMaxIndex)
                index2 = QueryCacheManager._agingMaxIndex;
              queryCacheKeyList[index1].AgingIndex = index2;
              queryCacheKeyList[index1].HitCount >>= QueryCacheManager._agingFactor[index2];
            }
          }
        }
        else
        {
          this._evictionTimer.Stop();
          flag = true;
        }
      }
      if (flag)
        return;
      this._evictionTimer.Resume();
    }

    public void Dispose()
    {
      GC.SuppressFinalize((object) this);
      if (!this._evictionTimer.Stop())
        return;
      this.Clear();
    }

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    private sealed class EvictionTimer
    {
      private readonly object _sync = new object();
      private readonly int _period;
      private readonly QueryCacheManager _cacheManager;
      private Timer _timer;

      internal EvictionTimer(QueryCacheManager cacheManager, int recyclePeriod)
      {
        this._cacheManager = cacheManager;
        this._period = recyclePeriod;
      }

      internal void Start()
      {
        lock (this._sync)
        {
          if (this._timer != null)
            return;
          this._timer = new Timer(new TimerCallback(QueryCacheManager.CacheRecyclerHandler), (object) this._cacheManager, this._period, this._period);
        }
      }

      internal bool Stop()
      {
        lock (this._sync)
        {
          if (this._timer == null)
            return false;
          this._timer.Dispose();
          this._timer = (Timer) null;
          return true;
        }
      }

      internal bool Suspend()
      {
        lock (this._sync)
        {
          if (this._timer == null)
            return false;
          this._timer.Change(-1, -1);
          return true;
        }
      }

      internal void Resume()
      {
        lock (this._sync)
        {
          if (this._timer == null)
            return;
          this._timer.Change(this._period, this._period);
        }
      }
    }
  }
}
