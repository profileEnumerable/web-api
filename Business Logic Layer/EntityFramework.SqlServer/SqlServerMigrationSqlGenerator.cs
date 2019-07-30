// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlServerMigrationSqlGenerator
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Migrations.Utilities;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer.Resources;
using System.Data.Entity.SqlServer.SqlGen;
using System.Data.Entity.SqlServer.Utilities;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace System.Data.Entity.SqlServer
{
  /// <summary>
  /// Provider to convert provider agnostic migration operations into SQL commands
  /// that can be run against a Microsoft SQL Server database.
  /// </summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  public class SqlServerMigrationSqlGenerator : MigrationSqlGenerator
  {
    private const string BatchTerminator = "GO";
    internal const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffK";
    internal const string DateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:ss.fffzzz";
    private SqlGenerator _sqlGenerator;
    private List<MigrationStatement> _statements;
    private HashSet<string> _generatedSchemas;
    private string _providerManifestToken;
    private int _variableCounter;

    /// <summary>
    /// Converts a set of migration operations into Microsoft SQL Server specific SQL.
    /// </summary>
    /// <param name="migrationOperations"> The operations to be converted. </param>
    /// <param name="providerManifestToken"> Token representing the version of SQL Server being targeted (i.e. "2005", "2008"). </param>
    /// <returns> A list of SQL statements to be executed to perform the migration operations. </returns>
    public override IEnumerable<MigrationStatement> Generate(
      IEnumerable<MigrationOperation> migrationOperations,
      string providerManifestToken)
    {
      Check.NotNull<IEnumerable<MigrationOperation>>(migrationOperations, nameof (migrationOperations));
      Check.NotNull<string>(providerManifestToken, nameof (providerManifestToken));
      this._statements = new List<MigrationStatement>();
      this._generatedSchemas = new HashSet<string>();
      this.InitializeProviderServices(providerManifestToken);
      this.GenerateStatements(migrationOperations);
      return (IEnumerable<MigrationStatement>) this._statements;
    }

    private void GenerateStatements(
      IEnumerable<MigrationOperation> migrationOperations)
    {
      Check.NotNull<IEnumerable<MigrationOperation>>(migrationOperations, nameof (migrationOperations));
      ((IEnumerable<object>) SqlServerMigrationSqlGenerator.DetectHistoryRebuild(migrationOperations)).Each<object>((Action<object>) (o =>
      {
        // ISSUE: reference to a compiler-generated field
        if (SqlServerMigrationSqlGenerator.\u003CGenerateStatements\u003Eo__SiteContainer0.\u003C\u003Ep__Site1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          SqlServerMigrationSqlGenerator.\u003CGenerateStatements\u003Eo__SiteContainer0.\u003C\u003Ep__Site1 = CallSite<Action<CallSite, SqlServerMigrationSqlGenerator, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "Generate", (IEnumerable<Type>) null, typeof (SqlServerMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        SqlServerMigrationSqlGenerator.\u003CGenerateStatements\u003Eo__SiteContainer0.\u003C\u003Ep__Site1.Target((CallSite) SqlServerMigrationSqlGenerator.\u003CGenerateStatements\u003Eo__SiteContainer0.\u003C\u003Ep__Site1, this, o);
      }));
    }

    /// <summary>Generates the SQL body for a stored procedure.</summary>
    /// <param name="commandTrees">The command trees representing the commands for an insert, update or delete operation.</param>
    /// <param name="rowsAffectedParameter">The rows affected parameter name.</param>
    /// <param name="providerManifestToken">The provider manifest token.</param>
    /// <returns>The SQL body for the stored procedure.</returns>
    public override string GenerateProcedureBody(
      ICollection<DbModificationCommandTree> commandTrees,
      string rowsAffectedParameter,
      string providerManifestToken)
    {
      Check.NotNull<ICollection<DbModificationCommandTree>>(commandTrees, nameof (commandTrees));
      Check.NotEmpty(providerManifestToken, nameof (providerManifestToken));
      if (!commandTrees.Any<DbModificationCommandTree>())
        return "RETURN";
      this.InitializeProviderServices(providerManifestToken);
      return this.GenerateFunctionSql(commandTrees, rowsAffectedParameter);
    }

    private void InitializeProviderServices(string providerManifestToken)
    {
      Check.NotEmpty(providerManifestToken, nameof (providerManifestToken));
      this._providerManifestToken = providerManifestToken;
      using (DbConnection connection = this.CreateConnection())
      {
        this.ProviderManifest = DbProviderServices.GetProviderServices(connection).GetProviderManifest(providerManifestToken);
        this._sqlGenerator = new SqlGenerator(SqlVersionUtils.GetSqlVersion(providerManifestToken));
      }
    }

    private string GenerateFunctionSql(
      ICollection<DbModificationCommandTree> commandTrees,
      string rowsAffectedParameter)
    {
      DmlFunctionSqlGenerator functionSqlGenerator = new DmlFunctionSqlGenerator(this._sqlGenerator);
      switch (commandTrees.First<DbModificationCommandTree>().CommandTreeKind)
      {
        case DbCommandTreeKind.Update:
          return functionSqlGenerator.GenerateUpdate((ICollection<DbUpdateCommandTree>) commandTrees.Cast<DbUpdateCommandTree>().ToList<DbUpdateCommandTree>(), rowsAffectedParameter);
        case DbCommandTreeKind.Insert:
          return functionSqlGenerator.GenerateInsert((ICollection<DbInsertCommandTree>) commandTrees.Cast<DbInsertCommandTree>().ToList<DbInsertCommandTree>());
        case DbCommandTreeKind.Delete:
          return functionSqlGenerator.GenerateDelete((ICollection<DbDeleteCommandTree>) commandTrees.Cast<DbDeleteCommandTree>().ToList<DbDeleteCommandTree>(), rowsAffectedParameter);
        default:
          return (string) null;
      }
    }

    /// <summary>
    /// Generates the specified update database operation which represents applying a series of migrations.
    /// The generated script is idempotent, meaning it contains conditional logic to check if individual migrations
    /// have already been applied and only apply the pending ones.
    /// </summary>
    /// <param name="updateDatabaseOperation">The update database operation.</param>
    protected virtual void Generate(UpdateDatabaseOperation updateDatabaseOperation)
    {
      Check.NotNull<UpdateDatabaseOperation>(updateDatabaseOperation, nameof (updateDatabaseOperation));
      if (!updateDatabaseOperation.Migrations.Any<UpdateDatabaseOperation.Migration>())
        return;
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.WriteLine("DECLARE @CurrentMigration [nvarchar](max)");
        writer.WriteLine();
        foreach (DbQueryCommandTree historyQueryTree in (IEnumerable<DbQueryCommandTree>) updateDatabaseOperation.HistoryQueryTrees)
        {
          HashSet<string> paramsToForceNonUnicode;
          string sql = this._sqlGenerator.GenerateSql(historyQueryTree, out paramsToForceNonUnicode);
          writer.Write("IF object_id('");
          writer.Write(SqlServerMigrationSqlGenerator.Escape(this._sqlGenerator.Targets.Single<string>()));
          writer.WriteLine("') IS NOT NULL");
          ++writer.Indent;
          writer.WriteLine("SELECT @CurrentMigration =");
          ++writer.Indent;
          writer.Write("(");
          writer.Write(SqlServerMigrationSqlGenerator.Indent(sql, writer.CurrentIndentation()));
          writer.WriteLine(")");
          writer.Indent -= 2;
          writer.WriteLine();
        }
        writer.WriteLine("IF @CurrentMigration IS NULL");
        ++writer.Indent;
        writer.WriteLine("SET @CurrentMigration = '0'");
        this.Statement(writer, (string) null);
      }
      List<MigrationStatement> statements = this._statements;
      foreach (UpdateDatabaseOperation.Migration migration in (IEnumerable<UpdateDatabaseOperation.Migration>) updateDatabaseOperation.Migrations)
      {
        using (IndentedTextWriter indentedTextWriter = SqlServerMigrationSqlGenerator.Writer())
        {
          this._statements = new List<MigrationStatement>();
          this.GenerateStatements((IEnumerable<MigrationOperation>) migration.Operations);
          if (this._statements.Count > 0)
          {
            indentedTextWriter.Write("IF @CurrentMigration < '");
            indentedTextWriter.Write(SqlServerMigrationSqlGenerator.Escape(migration.MigrationId));
            indentedTextWriter.WriteLine("'");
            indentedTextWriter.Write("BEGIN");
            using (IndentedTextWriter blockWriter = SqlServerMigrationSqlGenerator.Writer())
            {
              blockWriter.WriteLine();
              ++blockWriter.Indent;
              foreach (MigrationStatement statement in this._statements)
              {
                if (string.IsNullOrWhiteSpace(statement.BatchTerminator))
                {
                  statement.Sql.EachLine(new Action<string>(((TextWriter) blockWriter).WriteLine));
                }
                else
                {
                  blockWriter.WriteLine("EXECUTE('");
                  ++blockWriter.Indent;
                  statement.Sql.EachLine((Action<string>) (l => blockWriter.WriteLine(SqlServerMigrationSqlGenerator.Escape(l))));
                  --blockWriter.Indent;
                  blockWriter.WriteLine("')");
                }
              }
              indentedTextWriter.WriteLine(blockWriter.InnerWriter.ToString().TrimEnd());
            }
            indentedTextWriter.WriteLine("END");
            statements.Add(new MigrationStatement()
            {
              Sql = indentedTextWriter.InnerWriter.ToString()
            });
          }
        }
      }
      this._statements = statements;
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.MigrationOperation" />.
    /// Allows derived providers to handle additional operation types.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="migrationOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(MigrationOperation migrationOperation)
    {
      Check.NotNull<MigrationOperation>(migrationOperation, nameof (migrationOperation));
      throw Error.SqlServerMigrationSqlGenerator_UnknownOperation((object) this.GetType().Name, (object) migrationOperation.GetType().FullName);
    }

    /// <summary>
    /// Creates an empty connection for the current provider.
    /// Allows derived providers to use connection other than <see cref="T:System.Data.SqlClient.SqlConnection" />.
    /// </summary>
    /// <returns> An empty connection for the current provider. </returns>
    protected virtual DbConnection CreateConnection()
    {
      return DbConfiguration.DependencyResolver.GetService<DbProviderFactory>((object) "System.Data.SqlClient").CreateConnection();
    }

    /// <summary>Generates the specified create procedure operation.</summary>
    /// <param name="createProcedureOperation">The create procedure operation.</param>
    protected virtual void Generate(CreateProcedureOperation createProcedureOperation)
    {
      Check.NotNull<CreateProcedureOperation>(createProcedureOperation, nameof (createProcedureOperation));
      this.Generate((ProcedureOperation) createProcedureOperation, "CREATE");
    }

    /// <summary>Generates the specified alter procedure operation.</summary>
    /// <param name="alterProcedureOperation">The alter procedure operation.</param>
    protected virtual void Generate(AlterProcedureOperation alterProcedureOperation)
    {
      Check.NotNull<AlterProcedureOperation>(alterProcedureOperation, nameof (alterProcedureOperation));
      this.Generate((ProcedureOperation) alterProcedureOperation, "ALTER");
    }

    private void Generate(ProcedureOperation procedureOperation, string modifier)
    {
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write(modifier);
        writer.WriteLine(" PROCEDURE " + this.Name(procedureOperation.Name));
        ++writer.Indent;
        procedureOperation.Parameters.Each<ParameterModel>((Action<ParameterModel, int>) ((p, i) =>
        {
          this.Generate(p, writer);
          writer.WriteLine(i < procedureOperation.Parameters.Count - 1 ? "," : string.Empty);
        }));
        --writer.Indent;
        writer.WriteLine("AS");
        writer.WriteLine("BEGIN");
        ++writer.Indent;
        writer.WriteLine(!string.IsNullOrWhiteSpace(procedureOperation.BodySql) ? SqlServerMigrationSqlGenerator.Indent(procedureOperation.BodySql, writer.CurrentIndentation()) : "RETURN");
        --writer.Indent;
        writer.Write("END");
        this.Statement(writer, "GO");
      }
    }

    private void Generate(ParameterModel parameterModel, IndentedTextWriter writer)
    {
      writer.Write("@");
      writer.Write(parameterModel.Name);
      writer.Write(" ");
      writer.Write(this.BuildPropertyType((PropertyModel) parameterModel));
      if (parameterModel.IsOutParameter)
        writer.Write(" OUT");
      if (parameterModel.DefaultValue != null)
      {
        writer.Write(" = ");
        // ISSUE: reference to a compiler-generated field
        if (SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainere.\u003C\u003Ep__Sitef == null)
        {
          // ISSUE: reference to a compiler-generated field
          SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainere.\u003C\u003Ep__Sitef = CallSite<Action<CallSite, IndentedTextWriter, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Write", (IEnumerable<Type>) null, typeof (SqlServerMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, IndentedTextWriter, object> target = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainere.\u003C\u003Ep__Sitef.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, IndentedTextWriter, object>> pSitef = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainere.\u003C\u003Ep__Sitef;
        IndentedTextWriter indentedTextWriter = writer;
        // ISSUE: reference to a compiler-generated field
        if (SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainere.\u003C\u003Ep__Site10 == null)
        {
          // ISSUE: reference to a compiler-generated field
          SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainere.\u003C\u003Ep__Site10 = CallSite<Func<CallSite, SqlServerMigrationSqlGenerator, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, nameof (Generate), (IEnumerable<Type>) null, typeof (SqlServerMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainere.\u003C\u003Ep__Site10.Target((CallSite) SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainere.\u003C\u003Ep__Site10, this, parameterModel.DefaultValue);
        target((CallSite) pSitef, indentedTextWriter, obj);
      }
      else
      {
        if (string.IsNullOrWhiteSpace(parameterModel.DefaultValueSql))
          return;
        writer.Write(" = ");
        writer.Write(parameterModel.DefaultValueSql);
      }
    }

    /// <summary>Generates the specified drop procedure operation.</summary>
    /// <param name="dropProcedureOperation">The drop procedure operation.</param>
    protected virtual void Generate(DropProcedureOperation dropProcedureOperation)
    {
      Check.NotNull<DropProcedureOperation>(dropProcedureOperation, nameof (dropProcedureOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("DROP PROCEDURE ");
        writer.Write(this.Name(dropProcedureOperation.Name));
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.CreateTableOperation" />. This method differs from
    /// <see cref="M:System.Data.Entity.SqlServer.SqlServerMigrationSqlGenerator.WriteCreateTable(System.Data.Entity.Migrations.Model.CreateTableOperation)" /> in that it will
    /// create the target database schema if it does not already exist.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="createTableOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(CreateTableOperation createTableOperation)
    {
      Check.NotNull<CreateTableOperation>(createTableOperation, nameof (createTableOperation));
      DatabaseName databaseName = DatabaseName.Parse(createTableOperation.Name);
      if (!string.IsNullOrWhiteSpace(databaseName.Schema) && !databaseName.Schema.EqualsIgnoreCase("dbo") && !this._generatedSchemas.Contains(databaseName.Schema))
      {
        this.GenerateCreateSchema(databaseName.Schema);
        this._generatedSchemas.Add(databaseName.Schema);
      }
      this.WriteCreateTable(createTableOperation);
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.CreateTableOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="createTableOperation"> The operation to produce SQL for. </param>
    protected virtual void WriteCreateTable(CreateTableOperation createTableOperation)
    {
      Check.NotNull<CreateTableOperation>(createTableOperation, nameof (createTableOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        this.WriteCreateTable(createTableOperation, writer);
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>Writes CREATE TABLE SQL to the target writer.</summary>
    /// <param name="createTableOperation"> The operation to produce SQL for. </param>
    /// <param name="writer"> The target writer. </param>
    protected virtual void WriteCreateTable(
      CreateTableOperation createTableOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<CreateTableOperation>(createTableOperation, nameof (createTableOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.WriteLine("CREATE TABLE " + this.Name(createTableOperation.Name) + " (");
      ++writer.Indent;
      createTableOperation.Columns.Each<ColumnModel>((Action<ColumnModel, int>) ((c, i) =>
      {
        this.Generate(c, writer);
        if (i >= createTableOperation.Columns.Count - 1)
          return;
        writer.WriteLine(",");
      }));
      if (createTableOperation.PrimaryKey != null)
      {
        writer.WriteLine(",");
        writer.Write("CONSTRAINT ");
        writer.Write(this.Quote(createTableOperation.PrimaryKey.Name));
        writer.Write(" PRIMARY KEY ");
        if (!createTableOperation.PrimaryKey.IsClustered)
          writer.Write("NONCLUSTERED ");
        writer.Write("(");
        writer.Write(createTableOperation.PrimaryKey.Columns.Join<string>(new Func<string, string>(this.Quote), ", "));
        writer.WriteLine(")");
      }
      else
        writer.WriteLine();
      --writer.Indent;
      writer.Write(")");
    }

    /// <summary>
    /// Override this method to generate SQL when the definition of a table or its attributes are changed.
    /// The default implementation of this method does nothing.
    /// </summary>
    /// <param name="alterTableOperation"> The operation describing changes to the table. </param>
    protected internal virtual void Generate(AlterTableOperation alterTableOperation)
    {
      Check.NotNull<AlterTableOperation>(alterTableOperation, nameof (alterTableOperation));
    }

    /// <summary>
    /// Generates SQL to mark a table as a system table.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="createTableOperation"> The table to mark as a system table. </param>
    /// <param name="writer"> The <see cref="T:System.Data.Entity.Migrations.Utilities.IndentedTextWriter" /> to write the generated SQL to. </param>
    protected virtual void GenerateMakeSystemTable(
      CreateTableOperation createTableOperation,
      IndentedTextWriter writer)
    {
      Check.NotNull<CreateTableOperation>(createTableOperation, nameof (createTableOperation));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.WriteLine("BEGIN TRY");
      ++writer.Indent;
      writer.WriteLine("EXECUTE sp_MS_marksystemobject '" + SqlServerMigrationSqlGenerator.Escape(createTableOperation.Name) + "'");
      --writer.Indent;
      writer.WriteLine("END TRY");
      writer.WriteLine("BEGIN CATCH");
      writer.Write("END CATCH");
    }

    /// <summary>
    /// Generates SQL to create a database schema.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="schema"> The name of the schema to create. </param>
    protected virtual void GenerateCreateSchema(string schema)
    {
      Check.NotEmpty(schema, nameof (schema));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("IF schema_id('");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(schema));
        writer.WriteLine("') IS NULL");
        ++writer.Indent;
        writer.Write("EXECUTE('CREATE SCHEMA ");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(this.Quote(schema)));
        writer.Write("')");
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.AddForeignKeyOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="addForeignKeyOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(AddForeignKeyOperation addForeignKeyOperation)
    {
      Check.NotNull<AddForeignKeyOperation>(addForeignKeyOperation, nameof (addForeignKeyOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("ALTER TABLE ");
        writer.Write(this.Name(addForeignKeyOperation.DependentTable));
        writer.Write(" ADD CONSTRAINT ");
        writer.Write(this.Quote(addForeignKeyOperation.Name));
        writer.Write(" FOREIGN KEY (");
        writer.Write(addForeignKeyOperation.DependentColumns.Select<string, string>(new Func<string, string>(this.Quote)).Join<string>((Func<string, string>) null, ", "));
        writer.Write(") REFERENCES ");
        writer.Write(this.Name(addForeignKeyOperation.PrincipalTable));
        writer.Write(" (");
        writer.Write(addForeignKeyOperation.PrincipalColumns.Select<string, string>(new Func<string, string>(this.Quote)).Join<string>((Func<string, string>) null, ", "));
        writer.Write(")");
        if (addForeignKeyOperation.CascadeDelete)
          writer.Write(" ON DELETE CASCADE");
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.DropForeignKeyOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="dropForeignKeyOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(DropForeignKeyOperation dropForeignKeyOperation)
    {
      Check.NotNull<DropForeignKeyOperation>(dropForeignKeyOperation, nameof (dropForeignKeyOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("IF object_id(N'");
        string schema = DatabaseName.Parse(dropForeignKeyOperation.DependentTable).Schema;
        if (schema != null)
        {
          writer.Write(SqlServerMigrationSqlGenerator.Escape(this.Quote(schema)));
          writer.Write(".");
        }
        writer.Write(SqlServerMigrationSqlGenerator.Escape(this.Quote(dropForeignKeyOperation.Name)));
        writer.WriteLine("', N'F') IS NOT NULL");
        ++writer.Indent;
        writer.Write("ALTER TABLE ");
        writer.Write(this.Name(dropForeignKeyOperation.DependentTable));
        writer.Write(" DROP CONSTRAINT ");
        writer.Write(this.Quote(dropForeignKeyOperation.Name));
        --writer.Indent;
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.CreateIndexOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="createIndexOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(CreateIndexOperation createIndexOperation)
    {
      Check.NotNull<CreateIndexOperation>(createIndexOperation, nameof (createIndexOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("CREATE ");
        if (createIndexOperation.IsUnique)
          writer.Write("UNIQUE ");
        if (createIndexOperation.IsClustered)
          writer.Write("CLUSTERED ");
        writer.Write("INDEX ");
        writer.Write(this.Quote(createIndexOperation.Name));
        writer.Write(" ON ");
        writer.Write(this.Name(createIndexOperation.Table));
        writer.Write("(");
        writer.Write(createIndexOperation.Columns.Join<string>(new Func<string, string>(this.Quote), ", "));
        writer.Write(")");
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.DropIndexOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="dropIndexOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(DropIndexOperation dropIndexOperation)
    {
      Check.NotNull<DropIndexOperation>(dropIndexOperation, nameof (dropIndexOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(dropIndexOperation.Name));
        writer.Write("' AND object_id = object_id(N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(this.Name(dropIndexOperation.Table)));
        writer.WriteLine("', N'U'))");
        ++writer.Indent;
        writer.Write("DROP INDEX ");
        writer.Write(this.Quote(dropIndexOperation.Name));
        writer.Write(" ON ");
        writer.Write(this.Name(dropIndexOperation.Table));
        --writer.Indent;
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.AddPrimaryKeyOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="addPrimaryKeyOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(AddPrimaryKeyOperation addPrimaryKeyOperation)
    {
      Check.NotNull<AddPrimaryKeyOperation>(addPrimaryKeyOperation, nameof (addPrimaryKeyOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("ALTER TABLE ");
        writer.Write(this.Name(addPrimaryKeyOperation.Table));
        writer.Write(" ADD CONSTRAINT ");
        writer.Write(this.Quote(addPrimaryKeyOperation.Name));
        writer.Write(" PRIMARY KEY ");
        if (!addPrimaryKeyOperation.IsClustered)
          writer.Write("NONCLUSTERED ");
        writer.Write("(");
        writer.Write(addPrimaryKeyOperation.Columns.Select<string, string>(new Func<string, string>(this.Quote)).Join<string>((Func<string, string>) null, ", "));
        writer.Write(")");
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.DropPrimaryKeyOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="dropPrimaryKeyOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(DropPrimaryKeyOperation dropPrimaryKeyOperation)
    {
      Check.NotNull<DropPrimaryKeyOperation>(dropPrimaryKeyOperation, nameof (dropPrimaryKeyOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("ALTER TABLE ");
        writer.Write(this.Name(dropPrimaryKeyOperation.Table));
        writer.Write(" DROP CONSTRAINT ");
        writer.Write(this.Quote(dropPrimaryKeyOperation.Name));
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.AddColumnOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="addColumnOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(AddColumnOperation addColumnOperation)
    {
      Check.NotNull<AddColumnOperation>(addColumnOperation, nameof (addColumnOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("ALTER TABLE ");
        writer.Write(this.Name(addColumnOperation.Table));
        writer.Write(" ADD ");
        ColumnModel column = addColumnOperation.Column;
        this.Generate(column, writer);
        if (column.IsNullable.HasValue && (!column.IsNullable.Value && column.DefaultValue == null && (string.IsNullOrWhiteSpace(column.DefaultValueSql) && !column.IsIdentity) && (!column.IsTimestamp && !column.StoreType.EqualsIgnoreCase("rowversion") && !column.StoreType.EqualsIgnoreCase("timestamp"))))
        {
          writer.Write(" DEFAULT ");
          if (column.Type == PrimitiveTypeKind.DateTime)
          {
            writer.Write(this.Generate(DateTime.Parse("1900-01-01 00:00:00", (IFormatProvider) CultureInfo.InvariantCulture)));
          }
          else
          {
            // ISSUE: reference to a compiler-generated field
            if (SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer14.\u003C\u003Ep__Site15 == null)
            {
              // ISSUE: reference to a compiler-generated field
              SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer14.\u003C\u003Ep__Site15 = CallSite<Action<CallSite, IndentedTextWriter, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Write", (IEnumerable<Type>) null, typeof (SqlServerMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Action<CallSite, IndentedTextWriter, object> target = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer14.\u003C\u003Ep__Site15.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Action<CallSite, IndentedTextWriter, object>> pSite15 = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer14.\u003C\u003Ep__Site15;
            IndentedTextWriter indentedTextWriter = writer;
            // ISSUE: reference to a compiler-generated field
            if (SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer14.\u003C\u003Ep__Site16 == null)
            {
              // ISSUE: reference to a compiler-generated field
              SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer14.\u003C\u003Ep__Site16 = CallSite<Func<CallSite, SqlServerMigrationSqlGenerator, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, nameof (Generate), (IEnumerable<Type>) null, typeof (SqlServerMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer14.\u003C\u003Ep__Site16.Target((CallSite) SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer14.\u003C\u003Ep__Site16, this, column.ClrDefaultValue);
            target((CallSite) pSite15, indentedTextWriter, obj);
          }
        }
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.DropColumnOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="dropColumnOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(DropColumnOperation dropColumnOperation)
    {
      Check.NotNull<DropColumnOperation>(dropColumnOperation, nameof (dropColumnOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        this.DropDefaultConstraint(dropColumnOperation.Table, dropColumnOperation.Name, writer);
        writer.Write("ALTER TABLE ");
        writer.Write(this.Name(dropColumnOperation.Table));
        writer.Write(" DROP COLUMN ");
        writer.Write(this.Quote(dropColumnOperation.Name));
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.AlterColumnOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="alterColumnOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(AlterColumnOperation alterColumnOperation)
    {
      Check.NotNull<AlterColumnOperation>(alterColumnOperation, nameof (alterColumnOperation));
      ColumnModel column = alterColumnOperation.Column;
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        if (column.DefaultValue != null || !string.IsNullOrWhiteSpace(column.DefaultValueSql))
          this.DropDefaultConstraint(alterColumnOperation.Table, column.Name, writer);
        writer.Write("ALTER TABLE ");
        writer.Write(this.Name(alterColumnOperation.Table));
        writer.Write(" ALTER COLUMN ");
        writer.Write(this.Quote(column.Name));
        writer.Write(" ");
        writer.Write(this.BuildColumnType(column));
        if (column.IsNullable.HasValue && !column.IsNullable.Value)
          writer.Write(" NOT");
        writer.Write(" NULL");
        if (column.DefaultValue != null || !string.IsNullOrWhiteSpace(column.DefaultValueSql))
        {
          writer.WriteLine();
          writer.Write("ALTER TABLE ");
          writer.Write(this.Name(alterColumnOperation.Table));
          writer.Write(" ADD CONSTRAINT ");
          writer.Write(this.Quote("DF_" + alterColumnOperation.Table + "_" + column.Name));
          writer.Write(" DEFAULT ");
          // ISSUE: reference to a compiler-generated field
          if (SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer17.\u003C\u003Ep__Site18 == null)
          {
            // ISSUE: reference to a compiler-generated field
            SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer17.\u003C\u003Ep__Site18 = CallSite<Action<CallSite, IndentedTextWriter, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Write", (IEnumerable<Type>) null, typeof (SqlServerMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Action<CallSite, IndentedTextWriter, object> target = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer17.\u003C\u003Ep__Site18.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Action<CallSite, IndentedTextWriter, object>> pSite18 = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer17.\u003C\u003Ep__Site18;
          IndentedTextWriter indentedTextWriter = writer;
          object obj;
          if (column.DefaultValue == null)
          {
            obj = (object) column.DefaultValueSql;
          }
          else
          {
            // ISSUE: reference to a compiler-generated field
            if (SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer17.\u003C\u003Ep__Site19 == null)
            {
              // ISSUE: reference to a compiler-generated field
              SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer17.\u003C\u003Ep__Site19 = CallSite<Func<CallSite, SqlServerMigrationSqlGenerator, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, nameof (Generate), (IEnumerable<Type>) null, typeof (SqlServerMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            obj = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer17.\u003C\u003Ep__Site19.Target((CallSite) SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer17.\u003C\u003Ep__Site19, this, column.DefaultValue);
          }
          target((CallSite) pSite18, indentedTextWriter, obj);
          writer.Write(" FOR ");
          writer.Write(this.Quote(column.Name));
        }
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Call this method to generate SQL that will attempt to drop the default constraint created
    /// when a column is created. This method is usually called by code that overrides the creation or
    /// altering of columns.
    /// </summary>
    /// <param name="table">The table to which the constraint applies.</param>
    /// <param name="column">The column to which the constraint applies.</param>
    /// <param name="writer">The writer to which generated SQL should be written.</param>
    protected internal virtual void DropDefaultConstraint(
      string table,
      string column,
      IndentedTextWriter writer)
    {
      Check.NotEmpty(table, nameof (table));
      Check.NotEmpty(column, nameof (column));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      string str = "@var" + (object) this._variableCounter++;
      writer.Write("DECLARE ");
      writer.Write(str);
      writer.WriteLine(" nvarchar(128)");
      writer.Write("SELECT ");
      writer.Write(str);
      writer.WriteLine(" = name");
      writer.WriteLine("FROM sys.default_constraints");
      writer.Write("WHERE parent_object_id = object_id(N'");
      writer.Write(table);
      writer.WriteLine("')");
      writer.Write("AND col_name(parent_object_id, parent_column_id) = '");
      writer.Write(column);
      writer.WriteLine("';");
      writer.Write("IF ");
      writer.Write(str);
      writer.WriteLine(" IS NOT NULL");
      ++writer.Indent;
      writer.Write("EXECUTE('ALTER TABLE ");
      writer.Write(SqlServerMigrationSqlGenerator.Escape(this.Name(table)));
      writer.Write(" DROP CONSTRAINT [' + ");
      writer.Write(str);
      writer.WriteLine(" + ']')");
      --writer.Indent;
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.DropTableOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="dropTableOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(DropTableOperation dropTableOperation)
    {
      Check.NotNull<DropTableOperation>(dropTableOperation, nameof (dropTableOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("DROP TABLE ");
        writer.Write(this.Name(dropTableOperation.Name));
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.SqlOperation" />.
    /// Generated SQL should be added using the Statement or StatementBatch methods.
    /// </summary>
    /// <param name="sqlOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(SqlOperation sqlOperation)
    {
      Check.NotNull<SqlOperation>(sqlOperation, nameof (sqlOperation));
      this.StatementBatch(sqlOperation.Sql, sqlOperation.SuppressTransaction);
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.RenameColumnOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="renameColumnOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(RenameColumnOperation renameColumnOperation)
    {
      Check.NotNull<RenameColumnOperation>(renameColumnOperation, nameof (renameColumnOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("EXECUTE sp_rename @objname = N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(renameColumnOperation.Table));
        writer.Write(".");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(renameColumnOperation.Name));
        writer.Write("', @newname = N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(renameColumnOperation.NewName));
        writer.Write("', @objtype = N'COLUMN'");
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.RenameIndexOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="renameIndexOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(RenameIndexOperation renameIndexOperation)
    {
      Check.NotNull<RenameIndexOperation>(renameIndexOperation, nameof (renameIndexOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("EXECUTE sp_rename @objname = N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(renameIndexOperation.Table));
        writer.Write(".");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(renameIndexOperation.Name));
        writer.Write("', @newname = N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(renameIndexOperation.NewName));
        writer.Write("', @objtype = N'INDEX'");
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.RenameTableOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="renameTableOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(RenameTableOperation renameTableOperation)
    {
      Check.NotNull<RenameTableOperation>(renameTableOperation, nameof (renameTableOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        SqlServerMigrationSqlGenerator.WriteRenameTable(renameTableOperation, writer);
        string identifier = PrimaryKeyOperation.BuildDefaultName(renameTableOperation.Name);
        string s = PrimaryKeyOperation.BuildDefaultName(((RenameTableOperation) renameTableOperation.Inverse).Name);
        writer.WriteLine();
        writer.Write("IF object_id('");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(this.Quote(identifier)));
        writer.WriteLine("') IS NOT NULL BEGIN");
        ++writer.Indent;
        writer.Write("EXECUTE sp_rename @objname = N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(this.Quote(identifier)));
        writer.Write("', @newname = N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(s));
        writer.WriteLine("', @objtype = N'OBJECT'");
        --writer.Indent;
        writer.Write("END");
        this.Statement(writer, (string) null);
      }
    }

    private static void WriteRenameTable(
      RenameTableOperation renameTableOperation,
      IndentedTextWriter writer)
    {
      writer.Write("EXECUTE sp_rename @objname = N'");
      writer.Write(SqlServerMigrationSqlGenerator.Escape(renameTableOperation.Name));
      writer.Write("', @newname = N'");
      writer.Write(SqlServerMigrationSqlGenerator.Escape(renameTableOperation.NewName));
      writer.Write("', @objtype = N'OBJECT'");
    }

    /// <summary>Generates the specified rename procedure operation.</summary>
    /// <param name="renameProcedureOperation">The rename procedure operation.</param>
    protected virtual void Generate(RenameProcedureOperation renameProcedureOperation)
    {
      Check.NotNull<RenameProcedureOperation>(renameProcedureOperation, nameof (renameProcedureOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("EXECUTE sp_rename @objname = N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(renameProcedureOperation.Name));
        writer.Write("', @newname = N'");
        writer.Write(SqlServerMigrationSqlGenerator.Escape(renameProcedureOperation.NewName));
        writer.Write("', @objtype = N'OBJECT'");
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>Generates the specified move procedure operation.</summary>
    /// <param name="moveProcedureOperation">The move procedure operation.</param>
    protected virtual void Generate(MoveProcedureOperation moveProcedureOperation)
    {
      Check.NotNull<MoveProcedureOperation>(moveProcedureOperation, nameof (moveProcedureOperation));
      string str = moveProcedureOperation.NewSchema ?? "dbo";
      if (!str.EqualsIgnoreCase("dbo") && !this._generatedSchemas.Contains(str))
      {
        this.GenerateCreateSchema(str);
        this._generatedSchemas.Add(str);
      }
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        writer.Write("ALTER SCHEMA ");
        writer.Write(this.Quote(str));
        writer.Write(" TRANSFER ");
        writer.Write(this.Name(moveProcedureOperation.Name));
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.MoveTableOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="moveTableOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(MoveTableOperation moveTableOperation)
    {
      Check.NotNull<MoveTableOperation>(moveTableOperation, nameof (moveTableOperation));
      string str = moveTableOperation.NewSchema ?? "dbo";
      if (!str.EqualsIgnoreCase("dbo") && !this._generatedSchemas.Contains(str))
      {
        this.GenerateCreateSchema(str);
        this._generatedSchemas.Add(str);
      }
      if (!moveTableOperation.IsSystem)
      {
        using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
        {
          writer.Write("ALTER SCHEMA ");
          writer.Write(this.Quote(str));
          writer.Write(" TRANSFER ");
          writer.Write(this.Name(moveTableOperation.Name));
          this.Statement(writer, (string) null);
        }
      }
      else
      {
        using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
        {
          writer.Write("IF object_id('");
          writer.Write(moveTableOperation.CreateTableOperation.Name);
          writer.WriteLine("') IS NULL BEGIN");
          ++writer.Indent;
          this.WriteCreateTable(moveTableOperation.CreateTableOperation, writer);
          writer.WriteLine();
          --writer.Indent;
          writer.WriteLine("END");
          writer.Write("INSERT INTO ");
          writer.WriteLine(this.Name(moveTableOperation.CreateTableOperation.Name));
          writer.Write("SELECT * FROM ");
          writer.WriteLine(this.Name(moveTableOperation.Name));
          writer.Write("WHERE [ContextKey] = ");
          writer.WriteLine(this.Generate(moveTableOperation.ContextKey));
          writer.Write("DELETE ");
          writer.WriteLine(this.Name(moveTableOperation.Name));
          writer.Write("WHERE [ContextKey] = ");
          writer.WriteLine(this.Generate(moveTableOperation.ContextKey));
          writer.Write("IF NOT EXISTS(SELECT * FROM ");
          writer.Write(this.Name(moveTableOperation.Name));
          writer.WriteLine(")");
          ++writer.Indent;
          writer.Write("DROP TABLE ");
          writer.Write(this.Name(moveTableOperation.Name));
          --writer.Indent;
          this.Statement(writer, (string) null);
        }
      }
    }

    /// <summary>
    /// Generates SQL for the given column model. This method is called by other methods that
    /// process columns and can be overridden to change the SQL generated.
    /// </summary>
    /// <param name="column">The column for which SQL is being generated.</param>
    /// <param name="writer">The writer to which generated SQL should be written.</param>
    protected internal virtual void Generate(ColumnModel column, IndentedTextWriter writer)
    {
      Check.NotNull<ColumnModel>(column, nameof (column));
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      writer.Write(this.Quote(column.Name));
      writer.Write(" ");
      writer.Write(this.BuildColumnType(column));
      if (column.IsNullable.HasValue && !column.IsNullable.Value)
        writer.Write(" NOT NULL");
      if (column.DefaultValue != null)
      {
        writer.Write(" DEFAULT ");
        // ISSUE: reference to a compiler-generated field
        if (SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer1a.\u003C\u003Ep__Site1b == null)
        {
          // ISSUE: reference to a compiler-generated field
          SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer1a.\u003C\u003Ep__Site1b = CallSite<Action<CallSite, IndentedTextWriter, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Write", (IEnumerable<Type>) null, typeof (SqlServerMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, IndentedTextWriter, object> target = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer1a.\u003C\u003Ep__Site1b.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, IndentedTextWriter, object>> pSite1b = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer1a.\u003C\u003Ep__Site1b;
        IndentedTextWriter indentedTextWriter = writer;
        // ISSUE: reference to a compiler-generated field
        if (SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer1a.\u003C\u003Ep__Site1c == null)
        {
          // ISSUE: reference to a compiler-generated field
          SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer1a.\u003C\u003Ep__Site1c = CallSite<Func<CallSite, SqlServerMigrationSqlGenerator, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, nameof (Generate), (IEnumerable<Type>) null, typeof (SqlServerMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj = SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer1a.\u003C\u003Ep__Site1c.Target((CallSite) SqlServerMigrationSqlGenerator.\u003CGenerate\u003Eo__SiteContainer1a.\u003C\u003Ep__Site1c, this, column.DefaultValue);
        target((CallSite) pSite1b, indentedTextWriter, obj);
      }
      else if (!string.IsNullOrWhiteSpace(column.DefaultValueSql))
      {
        writer.Write(" DEFAULT ");
        writer.Write(column.DefaultValueSql);
      }
      else
      {
        if (!column.IsIdentity)
          return;
        if (column.Type == PrimitiveTypeKind.Guid && column.DefaultValue == null)
          writer.Write(" DEFAULT " + this.GuidColumnDefault);
        else
          writer.Write(" IDENTITY");
      }
    }

    /// <summary>
    /// Returns the column default value to use for store-generated GUID columns when
    /// no default value is explicitly specified in the migration.
    /// Returns newsequentialid() for on-premises SQL Server 2005 and later.
    /// Returns newid() for SQL Azure.
    /// </summary>
    /// <value>Either newsequentialid() or newid() as described above.</value>
    protected virtual string GuidColumnDefault
    {
      get
      {
        return !(this._providerManifestToken != "2012.Azure") || !(this._providerManifestToken != "2000") ? "newid()" : "newsequentialid()";
      }
    }

    /// <summary>
    /// Generates SQL for a <see cref="T:System.Data.Entity.Migrations.Model.HistoryOperation" />.
    /// Generated SQL should be added using the Statement method.
    /// </summary>
    /// <param name="historyOperation"> The operation to produce SQL for. </param>
    protected virtual void Generate(HistoryOperation historyOperation)
    {
      Check.NotNull<HistoryOperation>(historyOperation, nameof (historyOperation));
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        historyOperation.CommandTrees.Each<DbModificationCommandTree>((Action<DbModificationCommandTree>) (commandTree =>
        {
          List<SqlParameter> parameters;
          switch (commandTree.CommandTreeKind)
          {
            case DbCommandTreeKind.Insert:
              writer.Write(DmlSqlGenerator.GenerateInsertSql((DbInsertCommandTree) commandTree, this._sqlGenerator, out parameters, false, true, false));
              break;
            case DbCommandTreeKind.Delete:
              writer.Write(DmlSqlGenerator.GenerateDeleteSql((DbDeleteCommandTree) commandTree, this._sqlGenerator, out parameters, true, false));
              break;
          }
        }));
        this.Statement(writer, (string) null);
      }
    }

    /// <summary>
    /// Generates SQL to specify a constant byte[] default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(byte[] defaultValue)
    {
      Check.NotNull<byte[]>(defaultValue, nameof (defaultValue));
      return "0x" + ((IEnumerable<byte>) defaultValue).ToHexString();
    }

    /// <summary>
    /// Generates SQL to specify a constant bool default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(bool defaultValue)
    {
      return !defaultValue ? "0" : "1";
    }

    /// <summary>
    /// Generates SQL to specify a constant DateTime default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(DateTime defaultValue)
    {
      return "'" + defaultValue.ToString("yyyy-MM-ddTHH:mm:ss.fffK", (IFormatProvider) CultureInfo.InvariantCulture) + "'";
    }

    /// <summary>
    /// Generates SQL to specify a constant DateTimeOffset default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(DateTimeOffset defaultValue)
    {
      return "'" + defaultValue.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", (IFormatProvider) CultureInfo.InvariantCulture) + "'";
    }

    /// <summary>
    /// Generates SQL to specify a constant Guid default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(Guid defaultValue)
    {
      return "'" + (object) defaultValue + "'";
    }

    /// <summary>
    /// Generates SQL to specify a constant string default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(string defaultValue)
    {
      Check.NotNull<string>(defaultValue, nameof (defaultValue));
      return "'" + defaultValue + "'";
    }

    /// <summary>
    /// Generates SQL to specify a constant TimeSpan default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(TimeSpan defaultValue)
    {
      return "'" + (object) defaultValue + "'";
    }

    /// <summary>
    /// Generates SQL to specify a constant geogrpahy default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(DbGeography defaultValue)
    {
      return "'" + (object) defaultValue + "'";
    }

    /// <summary>
    /// Generates SQL to specify a constant geometry default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(DbGeometry defaultValue)
    {
      return "'" + (object) defaultValue + "'";
    }

    /// <summary>
    /// Generates SQL to specify a constant default value being set on a column.
    /// This method just generates the actual value, not the SQL to set the default value.
    /// </summary>
    /// <param name="defaultValue"> The value to be set. </param>
    /// <returns> SQL representing the default value. </returns>
    protected virtual string Generate(object defaultValue)
    {
      Check.NotNull<object>(defaultValue, nameof (defaultValue));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", defaultValue);
    }

    /// <summary>
    /// Generates SQL to specify the data type of a column.
    /// This method just generates the actual type, not the SQL to create the column.
    /// </summary>
    /// <param name="columnModel"> The definition of the column. </param>
    /// <returns> SQL representing the data type. </returns>
    protected virtual string BuildColumnType(ColumnModel columnModel)
    {
      Check.NotNull<ColumnModel>(columnModel, nameof (columnModel));
      if (columnModel.IsTimestamp)
        return "rowversion";
      return this.BuildPropertyType((PropertyModel) columnModel);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private string BuildPropertyType(PropertyModel propertyModel)
    {
      string storeTypeName = propertyModel.StoreType;
      TypeUsage type = this.ProviderManifest.GetStoreType(propertyModel.TypeUsage);
      if (string.IsNullOrWhiteSpace(storeTypeName))
        storeTypeName = type.EdmType.Name;
      else
        type = this.BuildStoreTypeUsage(storeTypeName, propertyModel) ?? type;
      string identifier = storeTypeName;
      string str = !identifier.EndsWith("(max)", StringComparison.Ordinal) ? this.Quote(identifier) : this.Quote(identifier.Substring(0, identifier.Length - "(max)".Length)) + "(max)";
      switch (storeTypeName)
      {
        case "decimal":
        case "numeric":
          object[] objArray1 = new object[6]
          {
            (object) str,
            (object) "(",
            null,
            null,
            null,
            null
          };
          object[] objArray2 = objArray1;
          byte? precision1 = propertyModel.Precision;
          // ISSUE: variable of a boxed type
          __Boxed<byte> local1 = (System.ValueType) (byte) (precision1.HasValue ? (int) precision1.GetValueOrDefault() : (int) type.GetPrecision());
          objArray2[2] = (object) local1;
          objArray1[3] = (object) ", ";
          object[] objArray3 = objArray1;
          byte? scale = propertyModel.Scale;
          // ISSUE: variable of a boxed type
          __Boxed<byte> local2 = (System.ValueType) (byte) (scale.HasValue ? (int) scale.GetValueOrDefault() : (int) type.GetScale());
          objArray3[4] = (object) local2;
          objArray1[5] = (object) ")";
          str = string.Concat(objArray1);
          break;
        case "datetime2":
        case "datetimeoffset":
        case "time":
          object[] objArray4 = new object[4]
          {
            (object) str,
            (object) "(",
            null,
            null
          };
          object[] objArray5 = objArray4;
          byte? precision2 = propertyModel.Precision;
          // ISSUE: variable of a boxed type
          __Boxed<byte> local3 = (System.ValueType) (byte) (precision2.HasValue ? (int) precision2.GetValueOrDefault() : (int) type.GetPrecision());
          objArray5[2] = (object) local3;
          objArray4[3] = (object) ")";
          str = string.Concat(objArray4);
          break;
        case "binary":
        case "varbinary":
        case "nvarchar":
        case "varchar":
        case "char":
        case "nchar":
          str = str + "(" + (object) (propertyModel.MaxLength ?? type.GetMaxLength()) + ")";
          break;
      }
      return str;
    }

    /// <summary>
    /// Generates a quoted name. The supplied name may or may not contain the schema.
    /// </summary>
    /// <param name="name"> The name to be quoted. </param>
    /// <returns> The quoted name. </returns>
    [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#")]
    protected virtual string Name(string name)
    {
      Check.NotEmpty(name, nameof (name));
      DatabaseName databaseName = DatabaseName.Parse(name);
      return ((IEnumerable<string>) new string[2]
      {
        databaseName.Schema,
        databaseName.Name
      }).Join<string>(new Func<string, string>(this.Quote), ".");
    }

    /// <summary>Quotes an identifier for SQL Server.</summary>
    /// <param name="identifier"> The identifier to be quoted. </param>
    /// <returns> The quoted identifier. </returns>
    protected virtual string Quote(string identifier)
    {
      Check.NotEmpty(identifier, nameof (identifier));
      return SqlGenerator.QuoteIdentifier(identifier);
    }

    private static string Escape(string s)
    {
      return s.Replace("'", "''");
    }

    private static string Indent(string s, string indentation)
    {
      return new Regex("\\r?\\n *").Replace(s, Environment.NewLine + indentation);
    }

    /// <summary>
    /// Adds a new Statement to be executed against the database.
    /// </summary>
    /// <param name="sql"> The statement to be executed. </param>
    /// <param name="suppressTransaction"> Gets or sets a value indicating whether this statement should be performed outside of the transaction scope that is used to make the migration process transactional. If set to true, this operation will not be rolled back if the migration process fails. </param>
    /// <param name="batchTerminator">The batch terminator for the database provider.</param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    protected void Statement(string sql, bool suppressTransaction = false, string batchTerminator = null)
    {
      Check.NotEmpty(sql, nameof (sql));
      this._statements.Add(new MigrationStatement()
      {
        Sql = sql,
        SuppressTransaction = suppressTransaction,
        BatchTerminator = batchTerminator
      });
    }

    /// <summary>
    /// Gets a new <see cref="T:System.Data.Entity.Migrations.Utilities.IndentedTextWriter" /> that can be used to build SQL.
    /// This is just a helper method to create a writer. Writing to the writer will
    /// not cause SQL to be registered for execution. You must pass the generated
    /// SQL to the Statement method.
    /// </summary>
    /// <returns> An empty text writer to use for SQL generation. </returns>
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    protected static IndentedTextWriter Writer()
    {
      return new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Adds a new Statement to be executed against the database.
    /// </summary>
    /// <param name="writer"> The writer containing the SQL to be executed. </param>
    /// <param name="batchTerminator">The batch terminator for the database provider.</param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    protected void Statement(IndentedTextWriter writer, string batchTerminator = null)
    {
      Check.NotNull<IndentedTextWriter>(writer, nameof (writer));
      this.Statement(writer.InnerWriter.ToString(), false, batchTerminator);
    }

    /// <summary>
    /// Breaks sql string into one or more statements, handling T-SQL utility statements as necessary.
    /// </summary>
    /// <param name="sqlBatch"> The SQL to split into one ore more statements to be executed. </param>
    /// <param name="suppressTransaction"> Gets or sets a value indicating whether this statement should be performed outside of the transaction scope that is used to make the migration process transactional. If set to true, this operation will not be rolled back if the migration process fails. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    protected void StatementBatch(string sqlBatch, bool suppressTransaction = false)
    {
      Check.NotNull<string>(sqlBatch, nameof (sqlBatch));
      sqlBatch = Regex.Replace(sqlBatch, "\\\\(\\r\\n|\\r|\\n)", "");
      string[] strArray = Regex.Split(sqlBatch, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "^\\s*({0}[ \\t]+[0-9]+|{0})(?:\\s+|$)", (object) "GO"), RegexOptions.IgnoreCase | RegexOptions.Multiline);
      for (int index1 = 0; index1 < strArray.Length; ++index1)
      {
        if (!strArray[index1].StartsWith("GO", StringComparison.OrdinalIgnoreCase) && (index1 != strArray.Length - 1 || !string.IsNullOrWhiteSpace(strArray[index1])))
        {
          if (strArray.Length > index1 + 1 && strArray[index1 + 1].StartsWith("GO", StringComparison.OrdinalIgnoreCase))
          {
            int num = 1;
            if (!strArray[index1 + 1].EqualsIgnoreCase("GO"))
              num = int.Parse(Regex.Match(strArray[index1 + 1], "([0-9]+)").Value, (IFormatProvider) CultureInfo.InvariantCulture);
            for (int index2 = 0; index2 < num; ++index2)
              this.Statement(strArray[index1], suppressTransaction, "GO");
          }
          else
            this.Statement(strArray[index1], suppressTransaction, (string) null);
        }
      }
    }

    private static IEnumerable<MigrationOperation> DetectHistoryRebuild(
      IEnumerable<MigrationOperation> operations)
    {
      IEnumerator<MigrationOperation> enumerator = operations.GetEnumerator();
      while (enumerator.MoveNext())
      {
        SqlServerMigrationSqlGenerator.HistoryRebuildOperationSequence sequence = SqlServerMigrationSqlGenerator.HistoryRebuildOperationSequence.Detect(enumerator);
        yield return (MigrationOperation) sequence ?? enumerator.Current;
      }
    }

    private void Generate(
      SqlServerMigrationSqlGenerator.HistoryRebuildOperationSequence sequence)
    {
      CreateTableOperation createTableOperation1 = sequence.DropPrimaryKeyOperation.CreateTableOperation;
      CreateTableOperation createTableOperation2 = SqlServerMigrationSqlGenerator.ResolveNameConflicts(createTableOperation1);
      RenameTableOperation renameTableOperation = new RenameTableOperation(createTableOperation2.Name, "__MigrationHistory", (object) null);
      using (IndentedTextWriter writer = SqlServerMigrationSqlGenerator.Writer())
      {
        this.WriteCreateTable(createTableOperation2, writer);
        writer.WriteLine();
        writer.Write("INSERT INTO ");
        writer.WriteLine(this.Name(createTableOperation2.Name));
        writer.Write("SELECT ");
        bool flag = true;
        foreach (ColumnModel column in (IEnumerable<ColumnModel>) createTableOperation1.Columns)
        {
          if (flag)
            flag = false;
          else
            writer.Write(", ");
          IndentedTextWriter indentedTextWriter = writer;
          string str;
          if (!(column.Name == sequence.AddColumnOperation.Column.Name))
          {
            if (column.Type != PrimitiveTypeKind.String)
              str = this.Name(column.Name);
            else
              str = "LEFT(" + this.Name(column.Name) + ", " + (object) column.MaxLength + ")";
          }
          else
            str = this.Generate((string) sequence.AddColumnOperation.Column.DefaultValue);
          indentedTextWriter.Write(str);
        }
        writer.Write(" FROM ");
        writer.WriteLine(this.Name(createTableOperation1.Name));
        writer.Write("DROP TABLE ");
        writer.WriteLine(this.Name(createTableOperation1.Name));
        SqlServerMigrationSqlGenerator.WriteRenameTable(renameTableOperation, writer);
        this.Statement(writer, (string) null);
      }
    }

    private static CreateTableOperation ResolveNameConflicts(
      CreateTableOperation source)
    {
      CreateTableOperation target = new CreateTableOperation(source.Name + "2", (object) null)
      {
        PrimaryKey = new AddPrimaryKeyOperation((object) null)
      };
      source.Columns.Each<ColumnModel>((Action<ColumnModel>) (c => target.Columns.Add(c)));
      source.PrimaryKey.Columns.Each<string>((Action<string>) (c => target.PrimaryKey.Columns.Add(c)));
      return target;
    }

    private class HistoryRebuildOperationSequence : MigrationOperation
    {
      public readonly AddColumnOperation AddColumnOperation;
      public readonly DropPrimaryKeyOperation DropPrimaryKeyOperation;

      private HistoryRebuildOperationSequence(
        AddColumnOperation addColumnOperation,
        DropPrimaryKeyOperation dropPrimaryKeyOperation)
        : base((object) null)
      {
        this.AddColumnOperation = addColumnOperation;
        this.DropPrimaryKeyOperation = dropPrimaryKeyOperation;
      }

      public override bool IsDestructiveChange
      {
        get
        {
          return false;
        }
      }

      public static SqlServerMigrationSqlGenerator.HistoryRebuildOperationSequence Detect(
        IEnumerator<MigrationOperation> enumerator)
      {
        AddColumnOperation current1 = enumerator.Current as AddColumnOperation;
        if (current1 == null || current1.Table != "dbo.__MigrationHistory" || current1.Column.Name != "ContextKey")
          return (SqlServerMigrationSqlGenerator.HistoryRebuildOperationSequence) null;
        enumerator.MoveNext();
        DropPrimaryKeyOperation current2 = (DropPrimaryKeyOperation) enumerator.Current;
        enumerator.MoveNext();
        AlterColumnOperation current3 = (AlterColumnOperation) enumerator.Current;
        enumerator.MoveNext();
        AddPrimaryKeyOperation current4 = (AddPrimaryKeyOperation) enumerator.Current;
        return new SqlServerMigrationSqlGenerator.HistoryRebuildOperationSequence(current1, current2);
      }
    }
  }
}
