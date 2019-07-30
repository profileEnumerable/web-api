// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataAssemblyHelper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.SchemaObjectModel;
using System.IO;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal static class MetadataAssemblyHelper
  {
    private static readonly byte[] _ecmaPublicKeyToken = ScalarType.ConvertToByteArray("b77a5c561934e089");
    private static readonly byte[] _msPublicKeyToken = ScalarType.ConvertToByteArray("b03f5f7f11d50a3a");
    private static readonly Memoizer<Assembly, bool> _filterAssemblyCacheByAssembly = new Memoizer<Assembly, bool>(new Func<Assembly, bool>(MetadataAssemblyHelper.ComputeShouldFilterAssembly), (IEqualityComparer<Assembly>) EqualityComparer<Assembly>.Default);
    private const string EcmaPublicKey = "b77a5c561934e089";
    private const string MicrosoftPublicKey = "b03f5f7f11d50a3a";

    internal static Assembly SafeLoadReferencedAssembly(AssemblyName assemblyName)
    {
      Assembly assembly = (Assembly) null;
      try
      {
        assembly = Assembly.Load(assemblyName);
      }
      catch (FileNotFoundException ex)
      {
      }
      catch (FileLoadException ex)
      {
      }
      return assembly;
    }

    private static bool ComputeShouldFilterAssembly(Assembly assembly)
    {
      return MetadataAssemblyHelper.ShouldFilterAssembly(new AssemblyName(assembly.FullName));
    }

    internal static bool ShouldFilterAssembly(Assembly assembly)
    {
      return MetadataAssemblyHelper._filterAssemblyCacheByAssembly.Evaluate(assembly);
    }

    private static bool ShouldFilterAssembly(AssemblyName assemblyName)
    {
      if (!MetadataAssemblyHelper.ArePublicKeyTokensEqual(assemblyName.GetPublicKeyToken(), MetadataAssemblyHelper._ecmaPublicKeyToken))
        return MetadataAssemblyHelper.ArePublicKeyTokensEqual(assemblyName.GetPublicKeyToken(), MetadataAssemblyHelper._msPublicKeyToken);
      return true;
    }

    private static bool ArePublicKeyTokensEqual(byte[] left, byte[] right)
    {
      if (left.Length != right.Length)
        return false;
      for (int index = 0; index < left.Length; ++index)
      {
        if ((int) left[index] != (int) right[index])
          return false;
      }
      return true;
    }

    internal static IEnumerable<Assembly> GetNonSystemReferencedAssemblies(
      Assembly assembly)
    {
      foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
      {
        if (!MetadataAssemblyHelper.ShouldFilterAssembly(referencedAssembly))
        {
          Assembly referenceAssembly = MetadataAssemblyHelper.SafeLoadReferencedAssembly(referencedAssembly);
          if (referenceAssembly != (Assembly) null)
            yield return referenceAssembly;
        }
      }
    }
  }
}
