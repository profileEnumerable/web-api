// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Spatial.SpatialServicesLoader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;

namespace System.Data.Entity.Spatial
{
  internal class SpatialServicesLoader
  {
    private readonly IDbDependencyResolver _resolver;

    public SpatialServicesLoader(IDbDependencyResolver resolver)
    {
      this._resolver = resolver;
    }

    public virtual DbSpatialServices LoadDefaultServices()
    {
      DbSpatialServices service1 = this._resolver.GetService<DbSpatialServices>();
      if (service1 != null)
        return service1;
      DbSpatialServices service2 = this._resolver.GetService<DbSpatialServices>((object) new DbProviderInfo("System.Data.SqlClient", "2012"));
      if (service2 != null && service2.NativeTypesAvailable)
        return service2;
      return (DbSpatialServices) DefaultSpatialServices.Instance;
    }
  }
}
