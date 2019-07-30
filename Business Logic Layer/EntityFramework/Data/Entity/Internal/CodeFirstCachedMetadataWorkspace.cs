// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.CodeFirstCachedMetadataWorkspace
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Internal
{
  internal class CodeFirstCachedMetadataWorkspace : ICachedMetadataWorkspace
  {
    private readonly MetadataWorkspace _metadataWorkspace;
    private readonly IEnumerable<Assembly> _assemblies;
    private readonly DbProviderInfo _providerInfo;
    private readonly string _defaultContainerName;

    public CodeFirstCachedMetadataWorkspace(DbDatabaseMapping databaseMapping)
    {
      this._providerInfo = databaseMapping.ProviderInfo;
      this._metadataWorkspace = databaseMapping.ToMetadataWorkspace();
      this._assemblies = (IEnumerable<Assembly>) databaseMapping.Model.GetClrTypes().Select<Type, Assembly>((Func<Type, Assembly>) (t => t.Assembly())).Distinct<Assembly>().ToList<Assembly>();
      this._defaultContainerName = databaseMapping.Model.Containers.First<EntityContainer>().Name;
    }

    public MetadataWorkspace GetMetadataWorkspace(DbConnection connection)
    {
      if (!string.Equals(this._providerInfo.ProviderInvariantName, connection.GetProviderInvariantName(), StringComparison.Ordinal))
        throw Error.CodeFirstCachedMetadataWorkspace_SameModelDifferentProvidersNotSupported();
      return this._metadataWorkspace;
    }

    public string DefaultContainerName
    {
      get
      {
        return this._defaultContainerName;
      }
    }

    public IEnumerable<Assembly> Assemblies
    {
      get
      {
        return this._assemblies;
      }
    }

    public DbProviderInfo ProviderInfo
    {
      get
      {
        return this._providerInfo;
      }
    }
  }
}
