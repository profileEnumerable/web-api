// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.DbDatabaseMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class DbDatabaseMapping
  {
    private readonly List<EntityContainerMapping> _entityContainerMappings = new List<EntityContainerMapping>();

    public EdmModel Model { get; set; }

    public EdmModel Database { get; set; }

    public DbProviderInfo ProviderInfo
    {
      get
      {
        return this.Database.ProviderInfo;
      }
    }

    public DbProviderManifest ProviderManifest
    {
      get
      {
        return this.Database.ProviderManifest;
      }
    }

    internal IList<EntityContainerMapping> EntityContainerMappings
    {
      get
      {
        return (IList<EntityContainerMapping>) this._entityContainerMappings;
      }
    }

    internal void AddEntityContainerMapping(EntityContainerMapping entityContainerMapping)
    {
      Check.NotNull<EntityContainerMapping>(entityContainerMapping, nameof (entityContainerMapping));
      this._entityContainerMappings.Add(entityContainerMapping);
    }
  }
}
