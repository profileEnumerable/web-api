// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.Internal.EntityProviderServices
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Core.Query.ResultAssembly;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.EntityClient.Internal
{
  internal sealed class EntityProviderServices : DbProviderServices
  {
    internal static readonly EntityProviderServices Instance = new EntityProviderServices();

    protected override DbCommandDefinition CreateDbCommandDefinition(
      DbProviderManifest providerManifest,
      DbCommandTree commandTree)
    {
      Check.NotNull<DbProviderManifest>(providerManifest, nameof (providerManifest));
      Check.NotNull<DbCommandTree>(commandTree, nameof (commandTree));
      return this.CreateDbCommandDefinition(providerManifest, commandTree, new DbInterceptionContext());
    }

    internal static EntityCommandDefinition CreateCommandDefinition(
      DbProviderFactory storeProviderFactory,
      DbCommandTree commandTree,
      DbInterceptionContext interceptionContext,
      IDbDependencyResolver resolver = null)
    {
      return new EntityCommandDefinition(storeProviderFactory, commandTree, interceptionContext, resolver, (BridgeDataReaderFactory) null, (ColumnMapFactory) null);
    }

    internal override DbCommandDefinition CreateDbCommandDefinition(
      DbProviderManifest providerManifest,
      DbCommandTree commandTree,
      DbInterceptionContext interceptionContext)
    {
      return (DbCommandDefinition) EntityProviderServices.CreateCommandDefinition(((StoreItemCollection) commandTree.MetadataWorkspace.GetItemCollection(DataSpace.SSpace)).ProviderFactory, commandTree, interceptionContext, (IDbDependencyResolver) null);
    }

    internal override void ValidateDataSpace(DbCommandTree commandTree)
    {
      if (commandTree.DataSpace != DataSpace.CSpace)
        throw new ProviderIncompatibleException(Strings.EntityClient_RequiresNonStoreCommandTree);
    }

    public override DbCommandDefinition CreateCommandDefinition(
      DbCommand prototype)
    {
      Check.NotNull<DbCommand>(prototype, nameof (prototype));
      return (DbCommandDefinition) ((EntityCommand) prototype).GetCommandDefinition();
    }

    protected override string GetDbProviderManifestToken(DbConnection connection)
    {
      Check.NotNull<DbConnection>(connection, nameof (connection));
      if (connection.GetType() != typeof (EntityConnection))
        throw new ArgumentException(Strings.Mapping_Provider_WrongConnectionType((object) typeof (EntityConnection)));
      return MetadataItem.EdmProviderManifest.Token;
    }

    protected override DbProviderManifest GetDbProviderManifest(
      string manifestToken)
    {
      Check.NotNull<string>(manifestToken, nameof (manifestToken));
      return (DbProviderManifest) MetadataItem.EdmProviderManifest;
    }
  }
}
