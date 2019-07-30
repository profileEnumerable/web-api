// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.INamedDataModelItemExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class INamedDataModelItemExtensions
  {
    public static string UniquifyName(
      this IEnumerable<INamedDataModelItem> namedDataModelItems,
      string name)
    {
      return namedDataModelItems.Select<INamedDataModelItem, string>((Func<INamedDataModelItem, string>) (i => i.Name)).Uniquify(name);
    }
  }
}
