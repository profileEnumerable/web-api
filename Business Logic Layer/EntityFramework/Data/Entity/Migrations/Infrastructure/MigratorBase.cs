// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.MigratorBase
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace System.Data.Entity.Migrations.Infrastructure
{
  /// <summary>
  /// Base class for decorators that wrap the core <see cref="T:System.Data.Entity.Migrations.DbMigrator" />
  /// </summary>
  [DebuggerStepThrough]
  public abstract class MigratorBase
  {
    private MigratorBase _this;

    /// <summary>Initializes a new instance of the MigratorBase class.</summary>
    /// <param name="innerMigrator"> The migrator that this decorator is wrapping. </param>
    protected MigratorBase(MigratorBase innerMigrator)
    {
      if (innerMigrator == null)
      {
        this._this = this;
      }
      else
      {
        this._this = innerMigrator;
        MigratorBase migratorBase = innerMigrator;
        while (migratorBase._this != innerMigrator)
          migratorBase = migratorBase._this;
        migratorBase._this = this;
      }
    }

    /// <summary>
    /// Gets a list of the pending migrations that have not been applied to the database.
    /// </summary>
    /// <returns> List of migration Ids </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public virtual IEnumerable<string> GetPendingMigrations()
    {
      return this._this.GetPendingMigrations();
    }

    /// <summary>
    /// Gets the configuration being used for the migrations process.
    /// </summary>
    public virtual DbMigrationsConfiguration Configuration
    {
      get
      {
        return this._this.Configuration;
      }
    }

    /// <summary>Updates the target database to the latest migration.</summary>
    public void Update()
    {
      this.Update((string) null);
    }

    /// <summary>Updates the target database to a given migration.</summary>
    /// <param name="targetMigration"> The migration to upgrade/downgrade to. </param>
    public virtual void Update(string targetMigration)
    {
      this._this.Update(targetMigration);
    }

    internal virtual string GetMigrationId(string migration)
    {
      return this._this.GetMigrationId(migration);
    }

    /// <summary>
    /// Gets a list of the migrations that are defined in the assembly.
    /// </summary>
    /// <returns> List of migration Ids </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public virtual IEnumerable<string> GetLocalMigrations()
    {
      return this._this.GetLocalMigrations();
    }

    /// <summary>
    /// Gets a list of the migrations that have been applied to the database.
    /// </summary>
    /// <returns> List of migration Ids </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public virtual IEnumerable<string> GetDatabaseMigrations()
    {
      return this._this.GetDatabaseMigrations();
    }

    internal virtual void AutoMigrate(
      string migrationId,
      VersionedModel sourceModel,
      VersionedModel targetModel,
      bool downgrading)
    {
      this._this.AutoMigrate(migrationId, sourceModel, targetModel, downgrading);
    }

    internal virtual void ApplyMigration(DbMigration migration, DbMigration lastMigration)
    {
      this._this.ApplyMigration(migration, lastMigration);
    }

    internal virtual void EnsureDatabaseExists(Action mustSucceedToKeepDatabase)
    {
      this._this.EnsureDatabaseExists(mustSucceedToKeepDatabase);
    }

    internal virtual void RevertMigration(
      string migrationId,
      DbMigration migration,
      XDocument targetModel)
    {
      this._this.RevertMigration(migrationId, migration, targetModel);
    }

    internal virtual void SeedDatabase()
    {
      this._this.SeedDatabase();
    }

    internal virtual void ExecuteStatements(
      IEnumerable<MigrationStatement> migrationStatements)
    {
      this._this.ExecuteStatements(migrationStatements);
    }

    internal virtual IEnumerable<MigrationStatement> GenerateStatements(
      IList<MigrationOperation> operations,
      string migrationId)
    {
      return this._this.GenerateStatements(operations, migrationId);
    }

    internal virtual IEnumerable<DbQueryCommandTree> CreateDiscoveryQueryTrees()
    {
      return this._this.CreateDiscoveryQueryTrees();
    }

    internal virtual void ExecuteSql(
      MigrationStatement migrationStatement,
      DbConnection connection,
      DbTransaction transaction,
      DbInterceptionContext interceptionContext)
    {
      this._this.ExecuteSql(migrationStatement, connection, transaction, interceptionContext);
    }

    internal virtual void Upgrade(
      IEnumerable<string> pendingMigrations,
      string targetMigrationId,
      string lastMigrationId)
    {
      this._this.Upgrade(pendingMigrations, targetMigrationId, lastMigrationId);
    }

    internal virtual void Downgrade(IEnumerable<string> pendingMigrations)
    {
      this._this.Downgrade(pendingMigrations);
    }

    internal virtual void UpgradeHistory(IEnumerable<MigrationOperation> upgradeOperations)
    {
      this._this.UpgradeHistory(upgradeOperations);
    }

    internal virtual string TargetDatabase
    {
      get
      {
        return this._this.TargetDatabase;
      }
    }

    internal virtual bool HistoryExists()
    {
      return this._this.HistoryExists();
    }
  }
}
