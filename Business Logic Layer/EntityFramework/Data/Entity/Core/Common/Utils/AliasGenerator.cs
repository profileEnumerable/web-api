// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.AliasGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace System.Data.Entity.Core.Common.Utils
{
  internal sealed class AliasGenerator
  {
    private static readonly string[] _counterNames = new string[250];
    private const int MaxPrefixCount = 500;
    private const int CacheSize = 250;
    private static Dictionary<string, string[]> _prefixCounter;
    private int _counter;
    private readonly string _prefix;
    private readonly string[] _cache;

    internal AliasGenerator(string prefix)
      : this(prefix, 250)
    {
    }

    [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.Collections.Generic.Dictionary`2<System.String,System.String[]>.#ctor(System.Int32,System.Collections.Generic.IEqualityComparer`1<System.String>)")]
    internal AliasGenerator(string prefix, int cacheSize)
    {
      this._prefix = prefix ?? string.Empty;
      if (0 >= cacheSize)
        return;
      string[] strArray = (string[]) null;
      Dictionary<string, string[]> prefixCounter;
      while ((prefixCounter = AliasGenerator._prefixCounter) == null || !prefixCounter.TryGetValue(prefix, out this._cache))
      {
        if (strArray == null)
          strArray = new string[cacheSize];
        int capacity = 1 + (prefixCounter != null ? prefixCounter.Count : 0);
        Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>(capacity, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        if (prefixCounter != null && capacity < 500)
        {
          foreach (KeyValuePair<string, string[]> keyValuePair in prefixCounter)
            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }
        dictionary.Add(prefix, strArray);
        Interlocked.CompareExchange<Dictionary<string, string[]>>(ref AliasGenerator._prefixCounter, dictionary, prefixCounter);
      }
    }

    internal string Next()
    {
      this._counter = Math.Max(1 + this._counter, 0);
      return this.GetName(this._counter);
    }

    internal string GetName(int index)
    {
      string str;
      if (this._cache == null || (uint) this._cache.Length <= (uint) index)
        str = this._prefix + index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      else if ((str = this._cache[index]) == null)
      {
        string counterName;
        if ((uint) AliasGenerator._counterNames.Length <= (uint) index)
          counterName = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        else if ((counterName = AliasGenerator._counterNames[index]) == null)
          AliasGenerator._counterNames[index] = counterName = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this._cache[index] = str = this._prefix + counterName;
      }
      return str;
    }
  }
}
