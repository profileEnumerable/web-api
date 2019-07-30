// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DbProviderServicesExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Utilities
{
  internal static class DbProviderServicesExtensions
  {
    public static string GetProviderManifestTokenChecked(
      this DbProviderServices providerServices,
      DbConnection connection)
    {
      try
      {
        return providerServices.GetProviderManifestToken(connection);
      }
      catch (ProviderIncompatibleException ex)
      {
        throw new ProviderIncompatibleException(Strings.FailedToGetProviderInformation, (Exception) ex);
      }
    }
  }
}
