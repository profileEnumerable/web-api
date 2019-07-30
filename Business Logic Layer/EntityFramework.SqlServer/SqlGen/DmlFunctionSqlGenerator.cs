// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.DmlFunctionSqlGenerator
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class DmlFunctionSqlGenerator
  {
    private readonly SqlGenerator _sqlGenerator;

    public DmlFunctionSqlGenerator(SqlGenerator sqlGenerator)
    {
      this._sqlGenerator = sqlGenerator;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public string GenerateInsert(ICollection<DbInsertCommandTree> commandTrees)
    {
      StringBuilder stringBuilder = new StringBuilder();
      DbInsertCommandTree insertCommandTree1 = commandTrees.First<DbInsertCommandTree>();
      List<SqlParameter> parameters;
      stringBuilder.Append(DmlSqlGenerator.GenerateInsertSql(insertCommandTree1, this._sqlGenerator, out parameters, false, true, false));
      stringBuilder.AppendLine();
      EntityType elementType = (EntityType) ((DbScanExpression) insertCommandTree1.Target.Expression).Target.ElementType;
      stringBuilder.Append(this.IntroduceRequiredLocalVariables(elementType, insertCommandTree1));
      foreach (DbInsertCommandTree tree in commandTrees.Skip<DbInsertCommandTree>(1))
      {
        stringBuilder.Append(DmlSqlGenerator.GenerateInsertSql(tree, this._sqlGenerator, out parameters, false, true, false));
        stringBuilder.AppendLine();
      }
      List<DbInsertCommandTree> list = commandTrees.Where<DbInsertCommandTree>((Func<DbInsertCommandTree, bool>) (ct => ct.Returning != null)).ToList<DbInsertCommandTree>();
      if (list.Any<DbInsertCommandTree>())
      {
        DmlFunctionSqlGenerator.ReturningSelectSqlGenerator selectSqlGenerator = new DmlFunctionSqlGenerator.ReturningSelectSqlGenerator();
        foreach (DbInsertCommandTree insertCommandTree2 in list)
        {
          insertCommandTree2.Target.Expression.Accept((DbExpressionVisitor) selectSqlGenerator);
          insertCommandTree2.Returning.Accept((DbExpressionVisitor) selectSqlGenerator);
        }
        foreach (EdmProperty keyProperty1 in elementType.KeyProperties)
        {
          EdmProperty keyProperty = keyProperty1;
          DbExpression right = insertCommandTree1.SetClauses.Cast<DbSetClause>().Where<DbSetClause>((Func<DbSetClause, bool>) (sc => ((DbPropertyExpression) sc.Property).Property == keyProperty)).Select<DbSetClause, DbExpression>((Func<DbSetClause, DbExpression>) (sc => sc.Value)).SingleOrDefault<DbExpression>() ?? (DbExpression) keyProperty.TypeUsage.Parameter(keyProperty.Name);
          insertCommandTree1.Target.Variable.Property(keyProperty).Equal(right).Accept((DbExpressionVisitor) selectSqlGenerator);
        }
        stringBuilder.Append(selectSqlGenerator.Sql);
      }
      return stringBuilder.ToString().TrimEnd();
    }

    private string IntroduceRequiredLocalVariables(
      EntityType entityType,
      DbInsertCommandTree commandTree)
    {
      List<EdmProperty> list = entityType.KeyProperties.Where<EdmProperty>((Func<EdmProperty, bool>) (p => p.IsStoreGeneratedIdentity)).ToList<EdmProperty>();
      SqlStringBuilder commandText = new SqlStringBuilder()
      {
        UpperCaseKeywords = true
      };
      if (list.Any<EdmProperty>())
      {
        foreach (EdmProperty edmProperty in list)
        {
          commandText.Append(commandText.Length == 0 ? "DECLARE " : ", ");
          commandText.Append("@");
          commandText.Append(edmProperty.Name);
          commandText.Append(" ");
          commandText.Append(DmlSqlGenerator.GetVariableType(this._sqlGenerator, (EdmMember) edmProperty));
        }
        commandText.AppendLine();
        DmlSqlGenerator.ExpressionTranslator translator = new DmlSqlGenerator.ExpressionTranslator(commandText, (DbModificationCommandTree) commandTree, true, this._sqlGenerator, (ICollection<EdmProperty>) entityType.KeyProperties, true);
        DmlSqlGenerator.GenerateReturningSql(commandText, (DbModificationCommandTree) commandTree, entityType, translator, commandTree.Returning, DmlSqlGenerator.UseGeneratedValuesVariable(commandTree, this._sqlGenerator.SqlVersion));
        commandText.AppendLine();
        commandText.AppendLine();
      }
      return commandText.ToString();
    }

    public string GenerateUpdate(
      ICollection<DbUpdateCommandTree> commandTrees,
      string rowsAffectedParameter)
    {
      if (!commandTrees.Any<DbUpdateCommandTree>())
        return (string) null;
      StringBuilder sql = new StringBuilder();
      List<SqlParameter> parameters;
      sql.AppendLine(DmlSqlGenerator.GenerateUpdateSql(commandTrees.First<DbUpdateCommandTree>(), this._sqlGenerator, out parameters, false, true));
      foreach (DbUpdateCommandTree tree in commandTrees.Skip<DbUpdateCommandTree>(1))
      {
        sql.Append(DmlSqlGenerator.GenerateUpdateSql(tree, this._sqlGenerator, out parameters, false, true));
        sql.AppendLine("AND @@ROWCOUNT > 0");
        sql.AppendLine();
      }
      List<DbUpdateCommandTree> list = commandTrees.Where<DbUpdateCommandTree>((Func<DbUpdateCommandTree, bool>) (ct => ct.Returning != null)).ToList<DbUpdateCommandTree>();
      if (list.Any<DbUpdateCommandTree>())
      {
        DmlFunctionSqlGenerator.ReturningSelectSqlGenerator selectSqlGenerator = new DmlFunctionSqlGenerator.ReturningSelectSqlGenerator();
        foreach (DbUpdateCommandTree updateCommandTree in list)
        {
          updateCommandTree.Target.Expression.Accept((DbExpressionVisitor) selectSqlGenerator);
          updateCommandTree.Returning.Accept((DbExpressionVisitor) selectSqlGenerator);
          updateCommandTree.Predicate.Accept((DbExpressionVisitor) selectSqlGenerator);
        }
        sql.AppendLine(selectSqlGenerator.Sql);
        sql.AppendLine();
      }
      DmlFunctionSqlGenerator.AppendSetRowsAffected(sql, rowsAffectedParameter);
      return sql.ToString().TrimEnd();
    }

    public string GenerateDelete(
      ICollection<DbDeleteCommandTree> commandTrees,
      string rowsAffectedParameter)
    {
      StringBuilder sql = new StringBuilder();
      List<SqlParameter> parameters;
      sql.AppendLine(DmlSqlGenerator.GenerateDeleteSql(commandTrees.First<DbDeleteCommandTree>(), this._sqlGenerator, out parameters, true, true));
      sql.AppendLine();
      foreach (DbDeleteCommandTree tree in commandTrees.Skip<DbDeleteCommandTree>(1))
      {
        sql.AppendLine(DmlSqlGenerator.GenerateDeleteSql(tree, this._sqlGenerator, out parameters, true, true));
        sql.AppendLine("AND @@ROWCOUNT > 0");
        sql.AppendLine();
      }
      DmlFunctionSqlGenerator.AppendSetRowsAffected(sql, rowsAffectedParameter);
      return sql.ToString().TrimEnd();
    }

    private static void AppendSetRowsAffected(StringBuilder sql, string rowsAffectedParameter)
    {
      if (string.IsNullOrWhiteSpace(rowsAffectedParameter))
        return;
      sql.Append("SET @");
      sql.Append(rowsAffectedParameter);
      sql.AppendLine(" = @@ROWCOUNT");
      sql.AppendLine();
    }

    private sealed class ReturningSelectSqlGenerator : BasicExpressionVisitor
    {
      private readonly StringBuilder _select = new StringBuilder();
      private readonly StringBuilder _from = new StringBuilder();
      private readonly StringBuilder _where = new StringBuilder();
      private int _aliasCount;
      private string _currentTableAlias;
      private EntityType _baseTable;
      private string _nextPropertyAlias;

      public string Sql
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine(this._select.ToString());
          stringBuilder.AppendLine(this._from.ToString());
          stringBuilder.Append("WHERE @@ROWCOUNT > 0");
          stringBuilder.Append((object) this._where);
          return stringBuilder.ToString();
        }
      }

      public override void Visit(DbNewInstanceExpression newInstanceExpression)
      {
        ReadOnlyMetadataCollection<EdmProperty> properties = ((RowType) newInstanceExpression.ResultType.EdmType).Properties;
        for (int index = 0; index < properties.Count; ++index)
        {
          this._select.Append(this._select.Length == 0 ? "SELECT " : ", ");
          this._nextPropertyAlias = properties[index].Name;
          newInstanceExpression.Arguments[index].Accept((DbExpressionVisitor) this);
        }
        this._nextPropertyAlias = (string) null;
      }

      public override void Visit(DbScanExpression scanExpression)
      {
        string str = SqlGenerator.GetTargetTSql(scanExpression.Target) + " AS " + (this._currentTableAlias = "t" + (object) this._aliasCount++);
        EntityTypeBase elementType = scanExpression.Target.ElementType;
        if (this._from.Length == 0)
        {
          this._baseTable = (EntityType) elementType;
          this._from.Append("FROM ");
          this._from.Append(str);
        }
        else
        {
          this._from.AppendLine();
          this._from.Append("JOIN ");
          this._from.Append(str);
          this._from.Append(" ON ");
          for (int index = 0; index < elementType.KeyMembers.Count; ++index)
          {
            if (index > 0)
              this._from.Append(" AND ");
            this._from.Append(this._currentTableAlias + ".");
            this._from.Append(SqlGenerator.QuoteIdentifier(elementType.KeyMembers[index].Name));
            this._from.Append(" = t0.");
            this._from.Append(SqlGenerator.QuoteIdentifier(this._baseTable.KeyMembers[index].Name));
          }
        }
      }

      public override void Visit(DbPropertyExpression propertyExpression)
      {
        this._select.Append(this._currentTableAlias);
        this._select.Append(".");
        this._select.Append(SqlGenerator.QuoteIdentifier(propertyExpression.Property.Name));
        if (string.IsNullOrWhiteSpace(this._nextPropertyAlias) || string.Equals(this._nextPropertyAlias, propertyExpression.Property.Name, StringComparison.Ordinal))
          return;
        this._select.Append(" AS ");
        this._select.Append(this._nextPropertyAlias);
      }

      public override void Visit(DbParameterReferenceExpression expression)
      {
        this._where.Append("@" + expression.ParameterName);
      }

      public override void Visit(DbIsNullExpression expression)
      {
      }

      public override void Visit(DbComparisonExpression comparisonExpression)
      {
        EdmMember property = ((DbPropertyExpression) comparisonExpression.Left).Property;
        if (!this._baseTable.KeyMembers.Contains(property))
          return;
        this._where.Append(" AND t0.");
        this._where.Append(SqlGenerator.QuoteIdentifier(property.Name));
        this._where.Append(" = ");
        comparisonExpression.Right.Accept((DbExpressionVisitor) this);
      }
    }
  }
}
