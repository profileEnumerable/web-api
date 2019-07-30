// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.SymbolTable
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal sealed class SymbolTable
  {
    private readonly List<Dictionary<string, Symbol>> symbols = new List<Dictionary<string, Symbol>>();

    internal void EnterScope()
    {
      this.symbols.Add(new Dictionary<string, Symbol>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
    }

    internal void ExitScope()
    {
      this.symbols.RemoveAt(this.symbols.Count - 1);
    }

    internal void Add(string name, Symbol value)
    {
      this.symbols[this.symbols.Count - 1][name] = value;
    }

    internal Symbol Lookup(string name)
    {
      for (int index = this.symbols.Count - 1; index >= 0; --index)
      {
        if (this.symbols[index].ContainsKey(name))
          return this.symbols[index][name];
      }
      return (Symbol) null;
    }
  }
}
