// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.Scope
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class Scope : IEnumerable<KeyValuePair<string, ScopeEntry>>, IEnumerable
  {
    private readonly Dictionary<string, ScopeEntry> _scopeEntries;

    internal Scope(IEqualityComparer<string> keyComparer)
    {
      this._scopeEntries = new Dictionary<string, ScopeEntry>(keyComparer);
    }

    internal Scope Add(string key, ScopeEntry value)
    {
      this._scopeEntries.Add(key, value);
      return this;
    }

    internal void Remove(string key)
    {
      this._scopeEntries.Remove(key);
    }

    internal void Replace(string key, ScopeEntry value)
    {
      this._scopeEntries[key] = value;
    }

    internal bool Contains(string key)
    {
      return this._scopeEntries.ContainsKey(key);
    }

    internal bool TryLookup(string key, out ScopeEntry value)
    {
      return this._scopeEntries.TryGetValue(key, out value);
    }

    public Dictionary<string, ScopeEntry>.Enumerator GetEnumerator()
    {
      return this._scopeEntries.GetEnumerator();
    }

    IEnumerator<KeyValuePair<string, ScopeEntry>> IEnumerable<KeyValuePair<string, ScopeEntry>>.GetEnumerator()
    {
      return (IEnumerator<KeyValuePair<string, ScopeEntry>>) this._scopeEntries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._scopeEntries.GetEnumerator();
    }
  }
}
