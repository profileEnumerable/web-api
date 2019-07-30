// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.DatabaseTableChecker
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Transactions;

namespace System.Data.Entity.Internal
{
  internal class DatabaseTableChecker
  {
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public DatabaseExistenceState AnyModelTableExists(
      InternalContext internalContext)
    {
      if (!internalContext.DatabaseOperations.Exists(internalContext.Connection, internalContext.CommandTimeout, new Lazy<StoreItemCollection>((Func<StoreItemCollection>) (() => DatabaseTableChecker.CreateStoreItemCollection(internalContext)))))
        return DatabaseExistenceState.DoesNotExist;
      using (ClonedObjectContext contextForDdlOps = internalContext.CreateObjectContextForDdlOps())
      {
        try
        {
          if (internalContext.CodeFirstModel == null)
            return DatabaseExistenceState.Exists;
          TableExistenceChecker service = DbConfiguration.DependencyResolver.GetService<TableExistenceChecker>((object) internalContext.ProviderName);
          if (service == null)
            return DatabaseExistenceState.Exists;
          List<EntitySet> list = this.GetModelTables(internalContext).ToList<EntitySet>();
          if (!list.Any<EntitySet>() || this.QueryForTableExistence(service, contextForDdlOps, list))
            return DatabaseExistenceState.Exists;
          return internalContext.HasHistoryTableEntry() ? DatabaseExistenceState.Exists : DatabaseExistenceState.ExistsConsideredEmpty;
        }
        catch (Exception ex)
        {
          return DatabaseExistenceState.Exists;
        }
      }
    }

    private static StoreItemCollection CreateStoreItemCollection(
      InternalContext internalContext)
    {
      using (ClonedObjectContext contextForDdlOps = internalContext.CreateObjectContextForDdlOps())
        return (StoreItemCollection) (EntityConnection) contextForDdlOps.ObjectContext.Connection.GetMetadataWorkspace().GetItemCollection(DataSpace.SSpace);
    }

    public virtual bool QueryForTableExistence(
      TableExistenceChecker checker,
      ClonedObjectContext clonedObjectContext,
      List<EntitySet> modelTables)
    {
      using (new TransactionScope(TransactionScopeOption.Suppress))
      {
        if (checker.AnyModelTableExistsInDatabase((ObjectContext) clonedObjectContext.ObjectContext, clonedObjectContext.Connection, (IEnumerable<EntitySet>) modelTables, "EdmMetadata"))
          return true;
      }
      return false;
    }

    public virtual IEnumerable<EntitySet> GetModelTables(
      InternalContext internalContext)
    {
      return internalContext.ObjectContext.MetadataWorkspace.GetItemCollection(DataSpace.SSpace).GetItems<EntityContainer>().Single<EntityContainer>().BaseEntitySets.OfType<EntitySet>().Where<EntitySet>((Func<EntitySet, bool>) (s =>
      {
        if (s.MetadataProperties.Contains("Type"))
          return (string) s.MetadataProperties["Type"].Value == "Tables";
        return true;
      }));
    }
  }
}
