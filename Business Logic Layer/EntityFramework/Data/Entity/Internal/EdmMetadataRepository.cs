// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.EdmMetadataRepository
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Internal
{
  internal class EdmMetadataRepository : RepositoryBase
  {
    private readonly DbTransaction _existingTransaction;

    public EdmMetadataRepository(
      InternalContext usersContext,
      string connectionString,
      DbProviderFactory providerFactory)
      : base(usersContext, connectionString, providerFactory)
    {
      this._existingTransaction = usersContext.TryGetCurrentStoreTransaction();
    }

    public virtual string QueryForModelHash(
      Func<DbConnection, EdmMetadataContext> createContext)
    {
      DbConnection connection = this.CreateConnection();
      try
      {
        using (EdmMetadataContext edmMetadataContext = createContext(connection))
        {
          if (this._existingTransaction != null)
          {
            if (this._existingTransaction.Connection == connection)
              edmMetadataContext.Database.UseTransaction(this._existingTransaction);
          }
          try
          {
            return edmMetadataContext.Metadata.AsNoTracking<EdmMetadata>().OrderByDescending<EdmMetadata, int>((Expression<Func<EdmMetadata, int>>) (m => m.Id)).FirstOrDefault<EdmMetadata>()?.ModelHash;
          }
          catch (EntityCommandExecutionException ex)
          {
            return (string) null;
          }
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }
  }
}
