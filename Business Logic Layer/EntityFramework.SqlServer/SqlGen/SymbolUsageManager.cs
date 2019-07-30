// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.SymbolUsageManager
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class SymbolUsageManager
  {
    private readonly Dictionary<Symbol, BoolWrapper> optionalColumnUsage = new Dictionary<Symbol, BoolWrapper>();

    internal bool ContainsKey(Symbol key)
    {
      return this.optionalColumnUsage.ContainsKey(key);
    }

    internal bool TryGetValue(Symbol key, out bool value)
    {
      BoolWrapper boolWrapper;
      if (this.optionalColumnUsage.TryGetValue(key, out boolWrapper))
      {
        value = boolWrapper.Value;
        return true;
      }
      value = false;
      return false;
    }

    internal void Add(Symbol sourceSymbol, Symbol symbolToAdd)
    {
      BoolWrapper boolWrapper;
      if (sourceSymbol == null || !this.optionalColumnUsage.TryGetValue(sourceSymbol, out boolWrapper))
        boolWrapper = new BoolWrapper();
      this.optionalColumnUsage.Add(symbolToAdd, boolWrapper);
    }

    internal void MarkAsUsed(Symbol key)
    {
      if (!this.optionalColumnUsage.ContainsKey(key))
        return;
      this.optionalColumnUsage[key].Value = true;
    }

    internal bool IsUsed(Symbol key)
    {
      return this.optionalColumnUsage[key].Value;
    }
  }
}
