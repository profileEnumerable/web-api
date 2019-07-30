// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.DmlSqlGenerator
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.SqlServer.Resources;
using System.Data.Entity.SqlServer.Utilities;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal static class DmlSqlGenerator
  {
    private const int CommandTextBuilderInitialCapacity = 256;
    private const string GeneratedValuesVariableName = "@generated_keys";

    internal static string GenerateUpdateSql(
      DbUpdateCommandTree tree,
      SqlGenerator sqlGenerator,
      out List<SqlParameter> parameters,
      bool generateReturningSql = true,
      bool upperCaseKeywords = true)
    {
      SqlStringBuilder commandText = new SqlStringBuilder(256)
      {
        UpperCaseKeywords = upperCaseKeywords
      };
      DmlSqlGenerator.ExpressionTranslator translator = new DmlSqlGenerator.ExpressionTranslator(commandText, (DbModificationCommandTree) tree, null != tree.Returning, sqlGenerator, (ICollection<EdmProperty>) null, true);
      if (tree.SetClauses.Count == 0)
      {
        commandText.AppendKeyword("declare ");
        commandText.AppendLine("@p int");
      }
      commandText.AppendKeyword("update ");
      tree.Target.Expression.Accept((DbExpressionVisitor) translator);
      commandText.AppendLine();
      bool flag = true;
      commandText.AppendKeyword("set ");
      foreach (DbSetClause setClause in (IEnumerable<DbModificationClause>) tree.SetClauses)
      {
        if (flag)
          flag = false;
        else
          commandText.Append(", ");
        setClause.Property.Accept((DbExpressionVisitor) translator);
        commandText.Append(" = ");
        setClause.Value.Accept((DbExpressionVisitor) translator);
      }
      if (flag)
        commandText.Append("@p = 0");
      commandText.AppendLine();
      commandText.AppendKeyword("where ");
      tree.Predicate.Accept((DbExpressionVisitor) translator);
      commandText.AppendLine();
      if (generateReturningSql)
        DmlSqlGenerator.GenerateReturningSql(commandText, (DbModificationCommandTree) tree, (EntityType) null, translator, tree.Returning, false);
      parameters = translator.Parameters;
      return commandText.ToString();
    }

    internal static string GenerateDeleteSql(
      DbDeleteCommandTree tree,
      SqlGenerator sqlGenerator,
      out List<SqlParameter> parameters,
      bool upperCaseKeywords = true,
      bool createParameters = true)
    {
      SqlStringBuilder commandText = new SqlStringBuilder(256)
      {
        UpperCaseKeywords = upperCaseKeywords
      };
      DmlSqlGenerator.ExpressionTranslator expressionTranslator = new DmlSqlGenerator.ExpressionTranslator(commandText, (DbModificationCommandTree) tree, false, sqlGenerator, (ICollection<EdmProperty>) null, createParameters);
      commandText.AppendKeyword("delete ");
      tree.Target.Expression.Accept((DbExpressionVisitor) expressionTranslator);
      commandText.AppendLine();
      commandText.AppendKeyword("where ");
      tree.Predicate.Accept((DbExpressionVisitor) expressionTranslator);
      parameters = expressionTranslator.Parameters;
      return commandText.ToString();
    }

    internal static string GenerateInsertSql(
      DbInsertCommandTree tree,
      SqlGenerator sqlGenerator,
      out List<SqlParameter> parameters,
      bool generateReturningSql = true,
      bool upperCaseKeywords = true,
      bool createParameters = true)
    {
      SqlStringBuilder commandText = new SqlStringBuilder(256)
      {
        UpperCaseKeywords = upperCaseKeywords
      };
      DmlSqlGenerator.ExpressionTranslator translator = new DmlSqlGenerator.ExpressionTranslator(commandText, (DbModificationCommandTree) tree, null != tree.Returning, sqlGenerator, (ICollection<EdmProperty>) null, createParameters);
      bool useGeneratedValuesVariable = DmlSqlGenerator.UseGeneratedValuesVariable(tree, sqlGenerator.SqlVersion);
      EntityType elementType = (EntityType) ((DbScanExpression) tree.Target.Expression).Target.ElementType;
      if (useGeneratedValuesVariable)
      {
        commandText.AppendKeyword("declare ").Append("@generated_keys").Append(" table(");
        bool flag = true;
        foreach (EdmMember keyMember in elementType.KeyMembers)
        {
          if (flag)
            flag = false;
          else
            commandText.Append(", ");
          commandText.Append(DmlSqlGenerator.GenerateMemberTSql(keyMember)).Append(" ").Append(DmlSqlGenerator.GetVariableType(sqlGenerator, keyMember));
          Facet facet;
          if (keyMember.TypeUsage.Facets.TryGetValue("Collation", false, out facet))
          {
            string s = facet.Value as string;
            if (!string.IsNullOrEmpty(s))
              commandText.AppendKeyword(" collate ").Append(s);
          }
        }
        commandText.AppendLine(")");
      }
      commandText.AppendKeyword("insert ");
      tree.Target.Expression.Accept((DbExpressionVisitor) translator);
      if (0 < tree.SetClauses.Count)
      {
        commandText.Append("(");
        bool flag = true;
        foreach (DbSetClause setClause in (IEnumerable<DbModificationClause>) tree.SetClauses)
        {
          if (flag)
            flag = false;
          else
            commandText.Append(", ");
          setClause.Property.Accept((DbExpressionVisitor) translator);
        }
        commandText.AppendLine(")");
      }
      else
        commandText.AppendLine();
      if (useGeneratedValuesVariable)
      {
        commandText.AppendKeyword("output ");
        bool flag = true;
        foreach (EdmMember keyMember in elementType.KeyMembers)
        {
          if (flag)
            flag = false;
          else
            commandText.Append(", ");
          commandText.Append("inserted.");
          commandText.Append(DmlSqlGenerator.GenerateMemberTSql(keyMember));
        }
        commandText.AppendKeyword(" into ").AppendLine("@generated_keys");
      }
      if (0 < tree.SetClauses.Count)
      {
        bool flag = true;
        commandText.AppendKeyword("values (");
        foreach (DbSetClause setClause in (IEnumerable<DbModificationClause>) tree.SetClauses)
        {
          if (flag)
            flag = false;
          else
            commandText.Append(", ");
          setClause.Value.Accept((DbExpressionVisitor) translator);
          translator.RegisterMemberValue(setClause.Property, setClause.Value);
        }
        commandText.AppendLine(")");
      }
      else
      {
        commandText.AppendKeyword("default values");
        commandText.AppendLine();
      }
      if (generateReturningSql)
        DmlSqlGenerator.GenerateReturningSql(commandText, (DbModificationCommandTree) tree, elementType, translator, tree.Returning, useGeneratedValuesVariable);
      parameters = translator.Parameters;
      return commandText.ToString();
    }

    internal static string GetVariableType(SqlGenerator sqlGenerator, EdmMember column)
    {
      string str = SqlGenerator.GenerateSqlForStoreType(sqlGenerator.SqlVersion, column.TypeUsage);
      if (str == "rowversion" || str == "timestamp")
        str = "binary(8)";
      return str;
    }

    internal static bool UseGeneratedValuesVariable(DbInsertCommandTree tree, SqlVersion sqlVersion)
    {
      bool flag1 = false;
      if (sqlVersion > SqlVersion.Sql8 && tree.Returning != null)
      {
        HashSet<EdmMember> edmMemberSet = new HashSet<EdmMember>(tree.SetClauses.Cast<DbSetClause>().Select<DbSetClause, EdmMember>((Func<DbSetClause, EdmMember>) (s => ((DbPropertyExpression) s.Property).Property)));
        bool flag2 = false;
        foreach (EdmMember keyMember in ((DbScanExpression) tree.Target.Expression).Target.ElementType.KeyMembers)
        {
          if (!edmMemberSet.Contains(keyMember))
          {
            if (flag2)
            {
              flag1 = true;
              break;
            }
            flag2 = true;
            if (!DmlSqlGenerator.IsValidScopeIdentityColumnType(keyMember.TypeUsage))
            {
              flag1 = true;
              break;
            }
          }
        }
      }
      return flag1;
    }

    internal static string GenerateMemberTSql(EdmMember member)
    {
      return SqlGenerator.QuoteIdentifier(member.Name);
    }

    internal static void GenerateReturningSql(
      SqlStringBuilder commandText,
      DbModificationCommandTree tree,
      EntityType tableType,
      DmlSqlGenerator.ExpressionTranslator translator,
      DbExpression returning,
      bool useGeneratedValuesVariable)
    {
      if (returning == null)
        return;
      commandText.AppendKeyword("select ");
      if (useGeneratedValuesVariable)
        translator.PropertyAlias = "t";
      returning.Accept((DbExpressionVisitor) translator);
      if (useGeneratedValuesVariable)
        translator.PropertyAlias = (string) null;
      commandText.AppendLine();
      if (useGeneratedValuesVariable)
      {
        commandText.AppendKeyword("from ");
        commandText.Append("@generated_keys");
        commandText.AppendKeyword(" as ");
        commandText.Append("g");
        commandText.AppendKeyword(" join ");
        tree.Target.Expression.Accept((DbExpressionVisitor) translator);
        commandText.AppendKeyword(" as ");
        commandText.Append("t");
        commandText.AppendKeyword(" on ");
        string keyword = string.Empty;
        foreach (EdmMember keyMember in tableType.KeyMembers)
        {
          commandText.AppendKeyword(keyword);
          keyword = " and ";
          commandText.Append("g.");
          string memberTsql = DmlSqlGenerator.GenerateMemberTSql(keyMember);
          commandText.Append(memberTsql);
          commandText.Append(" = t.");
          commandText.Append(memberTsql);
        }
        commandText.AppendLine();
        commandText.AppendKeyword("where @@ROWCOUNT > 0");
      }
      else
      {
        commandText.AppendKeyword("from ");
        tree.Target.Expression.Accept((DbExpressionVisitor) translator);
        commandText.AppendLine();
        commandText.AppendKeyword("where @@ROWCOUNT > 0");
        EntitySetBase target = ((DbScanExpression) tree.Target.Expression).Target;
        bool flag = false;
        foreach (EdmMember keyMember in target.ElementType.KeyMembers)
        {
          commandText.AppendKeyword(" and ");
          commandText.Append(DmlSqlGenerator.GenerateMemberTSql(keyMember));
          commandText.Append(" = ");
          SqlParameter sqlParameter;
          if (translator.MemberValues.TryGetValue(keyMember, out sqlParameter))
          {
            commandText.Append(sqlParameter.ParameterName);
          }
          else
          {
            if (flag)
              throw new NotSupportedException(Strings.Update_NotSupportedServerGenKey((object) target.Name));
            if (!DmlSqlGenerator.IsValidScopeIdentityColumnType(keyMember.TypeUsage))
              throw new InvalidOperationException(Strings.Update_NotSupportedIdentityType((object) keyMember.Name, (object) keyMember.TypeUsage.ToString()));
            commandText.Append("scope_identity()");
            flag = true;
          }
        }
      }
    }

    private static bool IsValidScopeIdentityColumnType(TypeUsage typeUsage)
    {
      if (typeUsage.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
        return false;
      string name = typeUsage.EdmType.Name;
      if (name == "tinyint" || name == "smallint" || (name == "int" || name == "bigint"))
        return true;
      Facet facet;
      if ((name == "decimal" || name == "numeric") && typeUsage.Facets.TryGetValue("Scale", false, out facet))
        return Convert.ToInt32(facet.Value, (IFormatProvider) CultureInfo.InvariantCulture) == 0;
      return false;
    }

    internal class ExpressionTranslator : BasicExpressionVisitor
    {
      private readonly SqlStringBuilder _commandText;
      private readonly DbModificationCommandTree _commandTree;
      private readonly List<SqlParameter> _parameters;
      private readonly Dictionary<EdmMember, SqlParameter> _memberValues;
      private readonly SqlGenerator _sqlGenerator;
      private readonly ICollection<EdmProperty> _localVariableBindings;
      private readonly bool _createParameters;

      internal ExpressionTranslator(
        SqlStringBuilder commandText,
        DbModificationCommandTree commandTree,
        bool preserveMemberValues,
        SqlGenerator sqlGenerator,
        ICollection<EdmProperty> localVariableBindings = null,
        bool createParameters = true)
      {
        this._commandText = commandText;
        this._commandTree = commandTree;
        this._sqlGenerator = sqlGenerator;
        this._localVariableBindings = localVariableBindings;
        this._parameters = new List<SqlParameter>();
        this._memberValues = preserveMemberValues ? new Dictionary<EdmMember, SqlParameter>() : (Dictionary<EdmMember, SqlParameter>) null;
        this._createParameters = createParameters;
      }

      internal List<SqlParameter> Parameters
      {
        get
        {
          return this._parameters;
        }
      }

      internal Dictionary<EdmMember, SqlParameter> MemberValues
      {
        get
        {
          return this._memberValues;
        }
      }

      internal string PropertyAlias { get; set; }

      internal SqlParameter CreateParameter(object value, TypeUsage type, string name = null)
      {
        SqlParameter sqlParameter = SqlProviderServices.CreateSqlParameter(name ?? DmlSqlGenerator.ExpressionTranslator.GetParameterName(this._parameters.Count), type, ParameterMode.In, value, true, this._sqlGenerator.SqlVersion);
        this._parameters.Add(sqlParameter);
        return sqlParameter;
      }

      internal static string GetParameterName(int index)
      {
        return "@" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }

      public override void Visit(DbAndExpression expression)
      {
        Check.NotNull<DbAndExpression>(expression, nameof (expression));
        this.VisitBinary((DbBinaryExpression) expression, " and ");
      }

      public override void Visit(DbOrExpression expression)
      {
        Check.NotNull<DbOrExpression>(expression, nameof (expression));
        this.VisitBinary((DbBinaryExpression) expression, " or ");
      }

      public override void Visit(DbComparisonExpression expression)
      {
        Check.NotNull<DbComparisonExpression>(expression, nameof (expression));
        this.VisitBinary((DbBinaryExpression) expression, " = ");
        this.RegisterMemberValue(expression.Left, expression.Right);
      }

      internal void RegisterMemberValue(DbExpression propertyExpression, DbExpression value)
      {
        if (this._memberValues == null)
          return;
        EdmMember property = ((DbPropertyExpression) propertyExpression).Property;
        if (value.ExpressionKind == DbExpressionKind.Null)
          return;
        this._memberValues[property] = this._parameters[this._parameters.Count - 1];
      }

      public override void Visit(DbIsNullExpression expression)
      {
        Check.NotNull<DbIsNullExpression>(expression, nameof (expression));
        expression.Argument.Accept((DbExpressionVisitor) this);
        this._commandText.AppendKeyword(" is null");
      }

      public override void Visit(DbNotExpression expression)
      {
        Check.NotNull<DbNotExpression>(expression, nameof (expression));
        this._commandText.AppendKeyword("not (");
        expression.Accept((DbExpressionVisitor) this);
        this._commandText.Append(")");
      }

      public override void Visit(DbConstantExpression expression)
      {
        Check.NotNull<DbConstantExpression>(expression, nameof (expression));
        SqlParameter parameter = this.CreateParameter(expression.Value, expression.ResultType, (string) null);
        if (this._createParameters)
        {
          this._commandText.Append(parameter.ParameterName);
        }
        else
        {
          using (SqlWriter writer = new SqlWriter(this._commandText.InnerBuilder))
            this._sqlGenerator.WriteSql(writer, expression.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this._sqlGenerator));
        }
      }

      public override void Visit(DbParameterReferenceExpression expression)
      {
        Check.NotNull<DbParameterReferenceExpression>(expression, nameof (expression));
        this._commandText.Append(this.CreateParameter((object) DBNull.Value, expression.ResultType, "@" + expression.ParameterName).ParameterName);
      }

      public override void Visit(DbScanExpression expression)
      {
        Check.NotNull<DbScanExpression>(expression, nameof (expression));
        if (expression.Target.GetMetadataPropertyValue<string>("DefiningQuery") != null)
        {
          string str = !(this._commandTree is DbDeleteCommandTree) ? (!(this._commandTree is DbInsertCommandTree) ? "UpdateFunction" : "InsertFunction") : "DeleteFunction";
          throw new UpdateException(Strings.Update_SqlEntitySetWithoutDmlFunctions((object) expression.Target.Name, (object) str, (object) "ModificationFunctionMapping"));
        }
        this._commandText.Append(SqlGenerator.GetTargetTSql(expression.Target));
      }

      public override void Visit(DbPropertyExpression expression)
      {
        Check.NotNull<DbPropertyExpression>(expression, nameof (expression));
        if (!string.IsNullOrEmpty(this.PropertyAlias))
        {
          this._commandText.Append(this.PropertyAlias);
          this._commandText.Append(".");
        }
        this._commandText.Append(DmlSqlGenerator.GenerateMemberTSql(expression.Property));
      }

      public override void Visit(DbNullExpression expression)
      {
        Check.NotNull<DbNullExpression>(expression, nameof (expression));
        this._commandText.AppendKeyword("null");
      }

      public override void Visit(DbNewInstanceExpression expression)
      {
        Check.NotNull<DbNewInstanceExpression>(expression, nameof (expression));
        bool flag = true;
        foreach (DbExpression dbExpression in (IEnumerable<DbExpression>) expression.Arguments)
        {
          EdmMember property = ((DbPropertyExpression) dbExpression).Property;
          string s = this._localVariableBindings != null ? (((IEnumerable<EdmMember>) this._localVariableBindings).Contains<EdmMember>(property) ? "@" + property.Name + " = " : (string) null) : string.Empty;
          if (s != null)
          {
            if (flag)
              flag = false;
            else
              this._commandText.Append(", ");
            this._commandText.Append(s);
            dbExpression.Accept((DbExpressionVisitor) this);
          }
        }
      }

      private void VisitBinary(DbBinaryExpression expression, string separator)
      {
        this._commandText.Append("(");
        expression.Left.Accept((DbExpressionVisitor) this);
        this._commandText.AppendKeyword(separator);
        expression.Right.Accept((DbExpressionVisitor) this);
        this._commandText.Append(")");
      }
    }
  }
}
