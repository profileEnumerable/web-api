// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.EdmFunctionExtensions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.SqlServer.Utilities
{
  internal static class EdmFunctionExtensions
  {
    internal static bool IsCSpace(this EdmFunction function)
    {
      MetadataProperty metadataProperty = function.MetadataProperties.FirstOrDefault<MetadataProperty>((Func<MetadataProperty, bool>) (p => p.Name == "DataSpace"));
      if (metadataProperty != null)
        return (DataSpace) metadataProperty.Value == DataSpace.CSpace;
      return false;
    }

    internal static bool IsCanonicalFunction(this EdmFunction function)
    {
      if (function.IsCSpace())
        return function.NamespaceName == "Edm";
      return false;
    }
  }
}
