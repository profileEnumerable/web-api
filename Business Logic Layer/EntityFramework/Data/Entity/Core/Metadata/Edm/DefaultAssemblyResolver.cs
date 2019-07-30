// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.DefaultAssemblyResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class DefaultAssemblyResolver : MetadataArtifactAssemblyResolver
  {
    internal override bool TryResolveAssemblyReference(
      AssemblyName refernceName,
      out Assembly assembly)
    {
      assembly = this.ResolveAssembly(refernceName);
      return assembly != (Assembly) null;
    }

    internal override IEnumerable<Assembly> GetWildcardAssemblies()
    {
      return DefaultAssemblyResolver.GetAllDiscoverableAssemblies();
    }

    internal virtual Assembly ResolveAssembly(AssemblyName referenceName)
    {
      Assembly assembly = (Assembly) null;
      foreach (Assembly nonSystemAssembly in DefaultAssemblyResolver.GetAlreadyLoadedNonSystemAssemblies())
      {
        if (AssemblyName.ReferenceMatchesDefinition(referenceName, new AssemblyName(nonSystemAssembly.FullName)))
          return nonSystemAssembly;
      }
      if (assembly == (Assembly) null)
      {
        assembly = MetadataAssemblyHelper.SafeLoadReferencedAssembly(referenceName);
        if (assembly != (Assembly) null)
          return assembly;
      }
      DefaultAssemblyResolver.TryFindWildcardAssemblyMatch(referenceName, out assembly);
      return assembly;
    }

    private static bool TryFindWildcardAssemblyMatch(
      AssemblyName referenceName,
      out Assembly assembly)
    {
      foreach (Assembly discoverableAssembly in DefaultAssemblyResolver.GetAllDiscoverableAssemblies())
      {
        if (AssemblyName.ReferenceMatchesDefinition(referenceName, new AssemblyName(discoverableAssembly.FullName)))
        {
          assembly = discoverableAssembly;
          return true;
        }
      }
      assembly = (Assembly) null;
      return false;
    }

    private static IEnumerable<Assembly> GetAlreadyLoadedNonSystemAssemblies()
    {
      return ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Func<Assembly, bool>) (a =>
      {
        if (a != (Assembly) null)
          return !MetadataAssemblyHelper.ShouldFilterAssembly(a);
        return false;
      }));
    }

    private static IEnumerable<Assembly> GetAllDiscoverableAssemblies()
    {
      Assembly entryAssembly = Assembly.GetEntryAssembly();
      HashSet<Assembly> source = new HashSet<Assembly>((IEqualityComparer<Assembly>) DefaultAssemblyResolver.AssemblyComparer.Instance);
      foreach (Assembly nonSystemAssembly in DefaultAssemblyResolver.GetAlreadyLoadedNonSystemAssemblies())
        source.Add(nonSystemAssembly);
      AspProxy aspProxy = new AspProxy();
      if (!aspProxy.IsAspNetEnvironment())
      {
        if (entryAssembly == (Assembly) null)
          return (IEnumerable<Assembly>) source;
        source.Add(entryAssembly);
        foreach (Assembly referencedAssembly in MetadataAssemblyHelper.GetNonSystemReferencedAssemblies(entryAssembly))
          source.Add(referencedAssembly);
        return (IEnumerable<Assembly>) source;
      }
      if (aspProxy.HasBuildManagerType())
      {
        IEnumerable<Assembly> referencedAssemblies = aspProxy.GetBuildManagerReferencedAssemblies();
        if (referencedAssemblies != null)
        {
          foreach (Assembly assembly in referencedAssemblies)
          {
            if (!MetadataAssemblyHelper.ShouldFilterAssembly(assembly))
              source.Add(assembly);
          }
        }
      }
      return source.Where<Assembly>((Func<Assembly, bool>) (a => a != (Assembly) null));
    }

    internal sealed class AssemblyComparer : IEqualityComparer<Assembly>
    {
      private static readonly DefaultAssemblyResolver.AssemblyComparer _instance = new DefaultAssemblyResolver.AssemblyComparer();

      private AssemblyComparer()
      {
      }

      public static DefaultAssemblyResolver.AssemblyComparer Instance
      {
        get
        {
          return DefaultAssemblyResolver.AssemblyComparer._instance;
        }
      }

      public bool Equals(Assembly x, Assembly y)
      {
        AssemblyName assemblyName1 = new AssemblyName(x.FullName);
        AssemblyName assemblyName2 = new AssemblyName(y.FullName);
        if (object.ReferenceEquals((object) x, (object) y))
          return true;
        if (AssemblyName.ReferenceMatchesDefinition(assemblyName1, assemblyName2))
          return AssemblyName.ReferenceMatchesDefinition(assemblyName2, assemblyName1);
        return false;
      }

      public int GetHashCode(Assembly assembly)
      {
        return assembly.FullName.GetHashCode();
      }
    }
  }
}
