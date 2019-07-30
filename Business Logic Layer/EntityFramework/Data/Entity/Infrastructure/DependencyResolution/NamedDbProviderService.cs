// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.NamedDbProviderService
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class NamedDbProviderService
  {
    private readonly string _invariantName;
    private readonly DbProviderServices _providerServices;

    public NamedDbProviderService(string invariantName, DbProviderServices providerServices)
    {
      this._invariantName = invariantName;
      this._providerServices = providerServices;
    }

    public string InvariantName
    {
      get
      {
        return this._invariantName;
      }
    }

    public DbProviderServices ProviderServices
    {
      get
      {
        return this._providerServices;
      }
    }
  }
}
