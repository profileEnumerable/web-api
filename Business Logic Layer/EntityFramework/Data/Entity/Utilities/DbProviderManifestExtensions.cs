// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DbProviderManifestExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.Utilities
{
  internal static class DbProviderManifestExtensions
  {
    public static PrimitiveType GetStoreTypeFromName(
      this DbProviderManifest providerManifest,
      string name)
    {
      return providerManifest.GetStoreTypes().Single<PrimitiveType>((Func<PrimitiveType, bool>) (p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)));
    }
  }
}
