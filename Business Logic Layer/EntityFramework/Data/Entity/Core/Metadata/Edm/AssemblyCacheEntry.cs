// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.AssemblyCacheEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal abstract class AssemblyCacheEntry
  {
    internal abstract IList<EdmType> TypesInAssembly { get; }

    internal abstract IList<Assembly> ClosureAssemblies { get; }

    internal bool TryGetEdmType(string typeName, out EdmType edmType)
    {
      edmType = (EdmType) null;
      foreach (EdmType edmType1 in (IEnumerable<EdmType>) this.TypesInAssembly)
      {
        if (edmType1.Identity == typeName)
        {
          edmType = edmType1;
          break;
        }
      }
      return edmType != null;
    }

    internal bool ContainsType(string typeName)
    {
      EdmType edmType = (EdmType) null;
      return this.TryGetEdmType(typeName, out edmType);
    }
  }
}
