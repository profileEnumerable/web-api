// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.History.HistoryRepository
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal;
using System.Data.Entity.Migrations.Edm;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;
using System.Xml.Linq;

namespace System.Data.Entity.Migrations.History
{
  internal class HistoryRepository : RepositoryBase
  {
    private static readonly string _productVersion = typeof (HistoryRepository).Assembly().GetInformationalVersion();
    public static readonly PropertyInfo MigrationIdProperty = typeof (HistoryRow).GetDeclaredProperty("MigrationId");
    public static readonly PropertyInfo ContextKeyProperty = typeof (HistoryRow).GetDeclaredProperty("ContextKey");
    private readonly string _contextKey;
    private readonly int? _commandTimeout;
    private readonly IEnumerable<string> _schemas;
    private readonly Func<DbConnection, string, HistoryContext> _historyContextFactory;
    private readonly DbContext _contextForInterception;
    private readonly int _contextKeyMaxLength;
    private readonly int _migrationIdMaxLength;
    private readonly DatabaseExistenceState _initialExistence;
    private readonly DbTransaction _existingTransaction;
    private string _currentSchema;
    private bool? _exists;
    private bool _contextKeyColumnExists;

    public HistoryRepository(
      InternalContext usersContext,
      string connectionString,
      DbProviderFactory providerFactory,
      string contextKey,
      int? commandTimeout,
      Func<DbConnection, string, HistoryContext> historyContextFactory,
      IEnumerable<string> schemas = null,
      DbContext contextForInterception = null,
      DatabaseExistenceState initialExistence = DatabaseExistenceState.Unknown)
      : base(usersContext, connectionString, providerFactory)
    {
      this._initialExistence = initialExistence;
      this._commandTimeout = commandTimeout;
      this._existingTransaction = usersContext.TryGetCurrentStoreTransaction();
      this._schemas = ((IEnumerable<string>) new string[1]
      {
        "dbo"
      }).Concat<string>(schemas ?? Enumerable.Empty<string>()).Distinct<string>();
      this._contextForInterception = contextForInterception;
      this._historyContextFactory = historyContextFactory;
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
        {
          EntityType entityType = ((IObjectContextAdapter) context).ObjectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace).Single<EntityType>((Func<EntityType, bool>) (et => EntityTypeExtensions.GetClrType(et) == typeof (HistoryRow)));
          int? maxLength = entityType.Properties.Single<EdmProperty>((Func<EdmProperty, bool>) (p => p.GetClrPropertyInfo().IsSameAs(HistoryRepository.MigrationIdProperty))).MaxLength;
          this._migrationIdMaxLength = maxLength.HasValue ? maxLength.Value : 150;
          maxLength = entityType.Properties.Single<EdmProperty>((Func<EdmProperty, bool>) (p => p.GetClrPropertyInfo().IsSameAs(HistoryRepository.ContextKeyProperty))).MaxLength;
          this._contextKeyMaxLength = maxLength.HasValue ? maxLength.Value : 300;
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
      this._contextKey = contextKey.RestrictTo(this._contextKeyMaxLength);
    }

    public int ContextKeyMaxLength
    {
      get
      {
        return this._contextKeyMaxLength;
      }
    }

    public int MigrationIdMaxLength
    {
      get
      {
        return this._migrationIdMaxLength;
      }
    }

    public string CurrentSchema
    {
      get
      {
        return this._currentSchema;
      }
      set
      {
        this._currentSchema = value;
      }
    }

    public virtual XDocument GetLastModel(
      out string migrationId,
      out string productVersion,
      string contextKey = null)
    {
      migrationId = (string) null;
      productVersion = (string) null;
      if (!this.Exists(contextKey))
        return (XDocument) null;
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
        {
          using (new TransactionScope(TransactionScopeOption.Suppress))
          {
            var data = this.CreateHistoryQuery(context, contextKey).OrderByDescending<HistoryRow, string>((Expression<Func<HistoryRow, string>>) (h => h.MigrationId)).Select(s => new
            {
              MigrationId = s.MigrationId,
              Model = s.Model,
              ProductVersion = s.ProductVersion
            }).FirstOrDefault();
            if (data == null)
              return (XDocument) null;
            migrationId = data.MigrationId;
            productVersion = data.ProductVersion;
            return new ModelCompressor().Decompress(data.Model);
          }
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public virtual XDocument GetModel(string migrationId, out string productVersion)
    {
      productVersion = (string) null;
      if (!this.Exists((string) null))
        return (XDocument) null;
      migrationId = migrationId.RestrictTo(this._migrationIdMaxLength);
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
        {
          var data = this.CreateHistoryQuery(context, (string) null).Where<HistoryRow>((Expression<Func<HistoryRow, bool>>) (h => h.MigrationId == migrationId)).Select(h => new
          {
            Model = h.Model,
            ProductVersion = h.ProductVersion
          }).SingleOrDefault();
          if (data == null)
            return (XDocument) null;
          productVersion = data.ProductVersion;
          return new ModelCompressor().Decompress(data.Model);
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public virtual IEnumerable<string> GetPendingMigrations(
      IEnumerable<string> localMigrations)
    {
      if (!this.Exists((string) null))
        return localMigrations;
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
        {
          List<string> list;
          using (new TransactionScope(TransactionScopeOption.Suppress))
            list = this.CreateHistoryQuery(context, (string) null).Select<HistoryRow, string>((Expression<Func<HistoryRow, string>>) (h => h.MigrationId)).ToList<string>();
          localMigrations = (IEnumerable<string>) localMigrations.Select<string, string>((Func<string, string>) (m => m.RestrictTo(this._migrationIdMaxLength))).ToArray<string>();
          IEnumerable<string> source = localMigrations.Except<string>((IEnumerable<string>) list);
          string migrationId1 = list.FirstOrDefault<string>();
          string migrationId2 = localMigrations.FirstOrDefault<string>();
          if (migrationId1 != migrationId2 && migrationId1 != null && (migrationId1.MigrationName() == Strings.InitialCreate && migrationId2 != null) && migrationId2.MigrationName() == Strings.InitialCreate)
            source = source.Skip<string>(1);
          return (IEnumerable<string>) source.ToList<string>();
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public virtual IEnumerable<string> GetMigrationsSince(string migrationId)
    {
      bool flag = this.Exists((string) null);
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
        {
          IQueryable<HistoryRow> source = this.CreateHistoryQuery(context, (string) null);
          migrationId = migrationId.RestrictTo(this._migrationIdMaxLength);
          if (migrationId != "0")
          {
            if (flag)
            {
              if (source.Any<HistoryRow>((Expression<Func<HistoryRow, bool>>) (h => h.MigrationId == migrationId)))
              {
                source = source.Where<HistoryRow>((Expression<Func<HistoryRow, bool>>) (h => string.Compare(h.MigrationId, migrationId, StringComparison.Ordinal) > 0));
                goto label_9;
              }
            }
            throw Error.MigrationNotFound((object) migrationId);
          }
          if (!flag)
            return Enumerable.Empty<string>();
label_9:
          return (IEnumerable<string>) source.OrderByDescending<HistoryRow, string>((Expression<Func<HistoryRow, string>>) (h => h.MigrationId)).Select<HistoryRow, string>((Expression<Func<HistoryRow, string>>) (h => h.MigrationId)).ToList<string>();
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public virtual string GetMigrationId(string migrationName)
    {
      if (!this.Exists((string) null))
        return (string) null;
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
        {
          List<string> list = this.CreateHistoryQuery(context, (string) null).Select<HistoryRow, string>((Expression<Func<HistoryRow, string>>) (h => h.MigrationId)).Where<string>((Expression<Func<string, bool>>) (m => m.Substring(16) == migrationName)).ToList<string>();
          if (!list.Any<string>())
            return (string) null;
          if (list.Count<string>() == 1)
            return list.Single<string>();
          throw Error.AmbiguousMigrationName((object) migrationName);
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    private IQueryable<HistoryRow> CreateHistoryQuery(
      HistoryContext context,
      string contextKey = null)
    {
      IQueryable<HistoryRow> source = (IQueryable<HistoryRow>) context.History;
      contextKey = !string.IsNullOrWhiteSpace(contextKey) ? contextKey.RestrictTo(this._contextKeyMaxLength) : this._contextKey;
      if (this._contextKeyColumnExists)
        source = source.Where<HistoryRow>((Expression<Func<HistoryRow, bool>>) (h => h.ContextKey == contextKey));
      return source;
    }

    public virtual bool IsShared()
    {
      if (!this.Exists((string) null) || !this._contextKeyColumnExists)
        return false;
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
          return context.History.Any<HistoryRow>((Expression<Func<HistoryRow, bool>>) (hr => hr.ContextKey != this._contextKey));
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public virtual bool HasMigrations()
    {
      if (!this.Exists((string) null))
        return false;
      if (!this._contextKeyColumnExists)
        return true;
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
          return context.History.Count<HistoryRow>((Expression<Func<HistoryRow, bool>>) (hr => hr.ContextKey == this._contextKey)) > 0;
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public virtual bool Exists(string contextKey = null)
    {
      if (!this._exists.HasValue)
        this._exists = new bool?(this.QueryExists(contextKey ?? this._contextKey));
      return this._exists.Value;
    }

    private bool QueryExists(string contextKey)
    {
      if (this._initialExistence == DatabaseExistenceState.DoesNotExist)
        return false;
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        if (this._initialExistence == DatabaseExistenceState.Unknown)
        {
          using (HistoryContext context = this.CreateContext(connection, (string) null))
          {
            if (!context.Database.Exists())
              return false;
          }
        }
        foreach (string schema in this._schemas.Reverse<string>())
        {
          using (HistoryContext context = this.CreateContext(connection, schema))
          {
            this._currentSchema = schema;
            this._contextKeyColumnExists = true;
            try
            {
              using (new TransactionScope(TransactionScopeOption.Suppress))
              {
                contextKey = contextKey.RestrictTo(this._contextKeyMaxLength);
                if (context.History.Count<HistoryRow>((Expression<Func<HistoryRow, bool>>) (hr => hr.ContextKey == contextKey)) > 0)
                  return true;
              }
            }
            catch (EntityException ex)
            {
              this._contextKeyColumnExists = false;
            }
            if (!this._contextKeyColumnExists)
            {
              try
              {
                using (new TransactionScope(TransactionScopeOption.Suppress))
                  context.History.Count<HistoryRow>();
              }
              catch (EntityException ex)
              {
                this._currentSchema = (string) null;
              }
            }
          }
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
      return !string.IsNullOrWhiteSpace(this._currentSchema);
    }

    public virtual void ResetExists()
    {
      this._exists = new bool?();
    }

    public virtual IEnumerable<MigrationOperation> GetUpgradeOperations()
    {
      if (this.Exists((string) null))
      {
        DbConnection connection = (DbConnection) null;
        try
        {
          connection = this.CreateConnection();
          string tableName = "dbo.__MigrationHistory";
          DbProviderManifest providerManifest;
          if (connection.GetProviderInfo(out providerManifest).IsSqlCe())
            tableName = "__MigrationHistory";
          using (LegacyHistoryContext context = new LegacyHistoryContext(connection))
          {
            bool createdOnExists = false;
            try
            {
              this.InjectInterceptionContext((DbContext) context);
              using (new TransactionScope(TransactionScopeOption.Suppress))
                context.History.Select<LegacyHistoryRow, DateTime>((Expression<Func<LegacyHistoryRow, DateTime>>) (h => h.CreatedOn)).FirstOrDefault<DateTime>();
              createdOnExists = true;
            }
            catch (EntityException ex)
            {
            }
            if (createdOnExists)
              yield return (MigrationOperation) new DropColumnOperation(tableName, "CreatedOn", (object) null);
          }
          using (HistoryContext context = this.CreateContext(connection, (string) null))
          {
            if (!this._contextKeyColumnExists)
            {
              if (this._historyContextFactory != HistoryContext.DefaultFactory)
                throw Error.UnableToUpgradeHistoryWhenCustomFactory();
              string table1 = tableName;
              ColumnModel columnModel1 = new ColumnModel(PrimitiveTypeKind.String);
              columnModel1.MaxLength = new int?(this._contextKeyMaxLength);
              columnModel1.Name = "ContextKey";
              columnModel1.IsNullable = new bool?(false);
              columnModel1.DefaultValue = (object) this._contextKey;
              ColumnModel column1 = columnModel1;
              yield return (MigrationOperation) new AddColumnOperation(table1, column1, (object) null);
              XDocument emptyModel = new DbModelBuilder().Build(connection).GetModel();
              CreateTableOperation createTableOperation = (CreateTableOperation) new EdmModelDiffer().Diff(emptyModel, context.GetModel(), (Lazy<ModificationCommandTreeGenerator>) null, (MigrationSqlGenerator) null, (string) null, (string) null).Single<MigrationOperation>();
              DropPrimaryKeyOperation primaryKeyOperation1 = new DropPrimaryKeyOperation((object) null);
              primaryKeyOperation1.Table = tableName;
              primaryKeyOperation1.CreateTableOperation = createTableOperation;
              DropPrimaryKeyOperation dropPrimaryKeyOperation = primaryKeyOperation1;
              dropPrimaryKeyOperation.Columns.Add("MigrationId");
              yield return (MigrationOperation) dropPrimaryKeyOperation;
              string table2 = tableName;
              ColumnModel columnModel2 = new ColumnModel(PrimitiveTypeKind.String);
              columnModel2.MaxLength = new int?(this._migrationIdMaxLength);
              columnModel2.Name = "MigrationId";
              columnModel2.IsNullable = new bool?(false);
              ColumnModel column2 = columnModel2;
              yield return (MigrationOperation) new AlterColumnOperation(table2, column2, false, (object) null);
              AddPrimaryKeyOperation primaryKeyOperation2 = new AddPrimaryKeyOperation((object) null);
              primaryKeyOperation2.Table = tableName;
              AddPrimaryKeyOperation addPrimaryKeyOperation = primaryKeyOperation2;
              addPrimaryKeyOperation.Columns.Add("MigrationId");
              addPrimaryKeyOperation.Columns.Add("ContextKey");
              yield return (MigrationOperation) addPrimaryKeyOperation;
            }
          }
        }
        finally
        {
          this.DisposeConnection(connection);
        }
      }
    }

    public virtual MigrationOperation CreateInsertOperation(
      string migrationId,
      VersionedModel versionedModel)
    {
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
        {
          context.History.Add(new HistoryRow()
          {
            MigrationId = migrationId.RestrictTo(this._migrationIdMaxLength),
            ContextKey = this._contextKey,
            Model = new ModelCompressor().Compress(versionedModel.Model),
            ProductVersion = versionedModel.Version ?? HistoryRepository._productVersion
          });
          using (CommandTracer commandTracer = new CommandTracer((DbContext) context))
          {
            context.SaveChanges();
            return (MigrationOperation) new HistoryOperation((IList<DbModificationCommandTree>) commandTracer.CommandTrees.OfType<DbModificationCommandTree>().ToList<DbModificationCommandTree>(), (object) null);
          }
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public virtual MigrationOperation CreateDeleteOperation(string migrationId)
    {
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
        {
          HistoryRow entity = new HistoryRow()
          {
            MigrationId = migrationId.RestrictTo(this._migrationIdMaxLength),
            ContextKey = this._contextKey
          };
          context.History.Attach(entity);
          context.History.Remove(entity);
          using (CommandTracer commandTracer = new CommandTracer((DbContext) context))
          {
            context.SaveChanges();
            return (MigrationOperation) new HistoryOperation((IList<DbModificationCommandTree>) commandTracer.CommandTrees.OfType<DbModificationCommandTree>().ToList<DbModificationCommandTree>(), (object) null);
          }
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public virtual IEnumerable<DbQueryCommandTree> CreateDiscoveryQueryTrees()
    {
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        foreach (string schema in this._schemas)
        {
          using (HistoryContext context = this.CreateContext(connection, schema))
          {
            IOrderedQueryable<string> query = context.History.Where<HistoryRow>((Expression<Func<HistoryRow, bool>>) (h => h.ContextKey == this._contextKey)).Select<HistoryRow, string>((Expression<Func<HistoryRow, string>>) (s => s.MigrationId)).OrderByDescending<string, string>((Expression<Func<string, string>>) (s => s));
            DbQuery<string> dbQuery = query as DbQuery<string>;
            if (dbQuery != null)
              dbQuery.InternalQuery.ObjectQuery.EnablePlanCaching = false;
            using (CommandTracer commandTracer = new CommandTracer((DbContext) context))
            {
              query.First<string>();
              DbQueryCommandTree queryTree = commandTracer.CommandTrees.OfType<DbQueryCommandTree>().Single<DbQueryCommandTree>((Func<DbQueryCommandTree, bool>) (t => t.DataSpace == DataSpace.SSpace));
              yield return new DbQueryCommandTree(queryTree.MetadataWorkspace, queryTree.DataSpace, queryTree.Query.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) new HistoryRepository.ParameterInliner(commandTracer.DbCommands.Single<DbCommand>().Parameters)));
            }
          }
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public virtual void BootstrapUsingEFProviderDdl(VersionedModel versionedModel)
    {
      DbConnection connection = (DbConnection) null;
      try
      {
        connection = this.CreateConnection();
        using (HistoryContext context = this.CreateContext(connection, (string) null))
        {
          context.Database.ExecuteSqlCommand(((IObjectContextAdapter) context).ObjectContext.CreateDatabaseScript());
          context.History.Add(new HistoryRow()
          {
            MigrationId = MigrationAssembly.CreateMigrationId(Strings.InitialCreate).RestrictTo(this._migrationIdMaxLength),
            ContextKey = this._contextKey,
            Model = new ModelCompressor().Compress(versionedModel.Model),
            ProductVersion = versionedModel.Version ?? HistoryRepository._productVersion
          });
          context.SaveChanges();
        }
      }
      finally
      {
        this.DisposeConnection(connection);
      }
    }

    public HistoryContext CreateContext(DbConnection connection, string schema = null)
    {
      HistoryContext historyContext = this._historyContextFactory(connection, schema ?? this.CurrentSchema);
      historyContext.Database.CommandTimeout = this._commandTimeout;
      if (this._existingTransaction != null && this._existingTransaction.Connection == connection)
        historyContext.Database.UseTransaction(this._existingTransaction);
      this.InjectInterceptionContext((DbContext) historyContext);
      return historyContext;
    }

    private void InjectInterceptionContext(DbContext context)
    {
      if (this._contextForInterception == null)
        return;
      ObjectContext objectContext = context.InternalContext.ObjectContext;
      objectContext.InterceptionContext = objectContext.InterceptionContext.WithDbContext(this._contextForInterception);
    }

    private class ParameterInliner : DefaultExpressionVisitor
    {
      private readonly DbParameterCollection _parameters;

      public ParameterInliner(DbParameterCollection parameters)
      {
        this._parameters = parameters;
      }

      public override DbExpression Visit(DbParameterReferenceExpression expression)
      {
        return (DbExpression) DbExpressionBuilder.Constant(this._parameters[expression.ParameterName].Value);
      }

      public override DbExpression Visit(DbOrExpression expression)
      {
        return expression.Left.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) this);
      }

      public override DbExpression Visit(DbAndExpression expression)
      {
        if (expression.Right is DbNotExpression)
          return expression.Left.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) this);
        return base.Visit(expression);
      }
    }
  }
}
