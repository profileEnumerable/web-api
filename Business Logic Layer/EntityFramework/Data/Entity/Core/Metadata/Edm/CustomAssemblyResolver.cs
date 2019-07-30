// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.CustomAssemblyResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class CustomAssemblyResolver : MetadataArtifactAssemblyResolver
  {
    private readonly Func<AssemblyName, Assembly> _referenceResolver;
    private readonly Func<IEnumerable<Assembly>> _wildcardAssemblyEnumerator;

    internal CustomAssemblyResolver(
      Func<IEnumerable<Assembly>> wildcardAssemblyEnumerator,
      Func<AssemblyName, Assembly> referenceResolver)
    {
      this._wildcardAssemblyEnumerator = wildcardAssemblyEnumerator;
      this._referenceResolver = referenceResolver;
    }

    internal override bool TryResolveAssemblyReference(
      AssemblyName refernceName,
      out Assembly assembly)
    {
      assembly = this._referenceResolver(refernceName);
      return assembly != (Assembly) null;
    }

    internal override IEnumerable<Assembly> GetWildcardAssemblies()
    {
      IEnumerable<Assembly> assemblies = this._wildcardAssemblyEnumerator();
      if (assemblies == null)
        throw new InvalidOperationException(Strings.WildcardEnumeratorReturnedNull);
      return assemblies;
    }
  }
}
