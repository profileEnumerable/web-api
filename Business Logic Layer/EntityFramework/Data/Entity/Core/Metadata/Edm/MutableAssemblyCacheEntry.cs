// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MutableAssemblyCacheEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MutableAssemblyCacheEntry : AssemblyCacheEntry
  {
    private readonly List<EdmType> _typesInAssembly = new List<EdmType>();
    private readonly List<Assembly> _closureAssemblies = new List<Assembly>();

    internal override IList<EdmType> TypesInAssembly
    {
      get
      {
        return (IList<EdmType>) this._typesInAssembly;
      }
    }

    internal override IList<Assembly> ClosureAssemblies
    {
      get
      {
        return (IList<Assembly>) this._closureAssemblies;
      }
    }
  }
}
