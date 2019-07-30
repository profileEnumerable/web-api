// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.KnownAssembliesSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class KnownAssembliesSet
  {
    private readonly Dictionary<Assembly, KnownAssemblyEntry> _assemblies;

    internal KnownAssembliesSet()
    {
      this._assemblies = new Dictionary<Assembly, KnownAssemblyEntry>();
    }

    internal KnownAssembliesSet(KnownAssembliesSet set)
    {
      this._assemblies = new Dictionary<Assembly, KnownAssemblyEntry>((IDictionary<Assembly, KnownAssemblyEntry>) set._assemblies);
    }

    internal virtual bool TryGetKnownAssembly(
      Assembly assembly,
      object loaderCookie,
      EdmItemCollection itemCollection,
      out KnownAssemblyEntry entry)
    {
      return this._assemblies.TryGetValue(assembly, out entry) && entry.HaveSeenInCompatibleContext(loaderCookie, itemCollection);
    }

    internal IEnumerable<Assembly> Assemblies
    {
      get
      {
        return (IEnumerable<Assembly>) this._assemblies.Keys;
      }
    }

    public IEnumerable<KnownAssemblyEntry> GetEntries(
      object loaderCookie,
      EdmItemCollection itemCollection)
    {
      return this._assemblies.Values.Where<KnownAssemblyEntry>((Func<KnownAssemblyEntry, bool>) (e => e.HaveSeenInCompatibleContext(loaderCookie, itemCollection)));
    }

    internal bool Contains(
      Assembly assembly,
      object loaderCookie,
      EdmItemCollection itemCollection)
    {
      KnownAssemblyEntry entry;
      return this.TryGetKnownAssembly(assembly, loaderCookie, itemCollection, out entry);
    }

    internal void Add(Assembly assembly, KnownAssemblyEntry knownAssemblyEntry)
    {
      KnownAssemblyEntry knownAssemblyEntry1;
      if (this._assemblies.TryGetValue(assembly, out knownAssemblyEntry1))
        this._assemblies[assembly] = knownAssemblyEntry;
      else
        this._assemblies.Add(assembly, knownAssemblyEntry);
    }
  }
}
