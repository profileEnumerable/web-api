// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.MetdataItemExtensions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.SqlServer.Utilities
{
  internal static class MetdataItemExtensions
  {
    public static T GetMetadataPropertyValue<T>(this MetadataItem item, string propertyName)
    {
      MetadataProperty metadataProperty = item.MetadataProperties.FirstOrDefault<MetadataProperty>((Func<MetadataProperty, bool>) (p => p.Name == propertyName));
      if (metadataProperty != null)
        return (T) metadataProperty.Value;
      return default (T);
    }
  }
}
