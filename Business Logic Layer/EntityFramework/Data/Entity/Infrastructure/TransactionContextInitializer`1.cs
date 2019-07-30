// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.TransactionContextInitializer`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Internal;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Transactions;

namespace System.Data.Entity.Infrastructure
{
  internal class TransactionContextInitializer<TContext> : IDatabaseInitializer<TContext>
    where TContext : TransactionContext
  {
    public void InitializeDatabase(TContext context)
    {
      EntityConnection connection = (EntityConnection) context.ObjectContext.Connection;
      if (connection.State != ConnectionState.Open)
        return;
      if (connection.CurrentTransaction == null)
        return;
      try
      {
        using (new TransactionScope(TransactionScopeOption.Suppress))
          context.Transactions.AsNoTracking<TransactionRow>().WithExecutionStrategy<TransactionRow>((IDbExecutionStrategy) new DefaultExecutionStrategy()).Count<TransactionRow>();
      }
      catch (EntityException ex)
      {
        IEnumerable<MigrationStatement> migrationStatements = TransactionContextInitializer<TContext>.GenerateMigrationStatements((TransactionContext) context);
        DbMigrator dbMigrator = new DbMigrator(context.InternalContext.MigrationsConfiguration, (DbContext) context, DatabaseExistenceState.Exists, true);
        using (new TransactionScope(TransactionScopeOption.Suppress))
          dbMigrator.ExecuteStatements(migrationStatements, connection.CurrentTransaction.StoreTransaction);
      }
    }

    internal static IEnumerable<MigrationStatement> GenerateMigrationStatements(
      TransactionContext context)
    {
      if (DbConfiguration.DependencyResolver.GetService<Func<MigrationSqlGenerator>>((object) context.InternalContext.ProviderName) != null)
      {
        MigrationSqlGenerator sqlGenerator = context.InternalContext.MigrationsConfiguration.GetSqlGenerator(context.InternalContext.ProviderName);
        DbConnection connection = context.Database.Connection;
        CreateTableOperation createTableOperation = (CreateTableOperation) new EdmModelDiffer().Diff(new DbModelBuilder().Build(connection).GetModel(), context.GetModel(), (Lazy<ModificationCommandTreeGenerator>) null, (MigrationSqlGenerator) null, (string) null, (string) null).Single<MigrationOperation>();
        string providerManifestToken = context.InternalContext.ModelProviderInfo != null ? context.InternalContext.ModelProviderInfo.ProviderManifestToken : DbConfiguration.DependencyResolver.GetService<IManifestTokenResolver>().ResolveManifestToken(connection);
        return sqlGenerator.Generate((IEnumerable<MigrationOperation>) new CreateTableOperation[1]
        {
          createTableOperation
        }, providerManifestToken);
      }
      return (IEnumerable<MigrationStatement>) new MigrationStatement[1]
      {
        new MigrationStatement()
        {
          Sql = ((IObjectContextAdapter) context).ObjectContext.CreateDatabaseScript(),
          SuppressTransaction = true
        }
      };
    }
  }
}
