// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.Sql8ConformanceChecker
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.SqlServer.Resources;
using System.Data.Entity.SqlServer.Utilities;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class Sql8ConformanceChecker : DbExpressionVisitor<bool>
  {
    internal static bool NeedsRewrite(DbExpression expr)
    {
      Sql8ConformanceChecker conformanceChecker = new Sql8ConformanceChecker();
      return expr.Accept<bool>((DbExpressionVisitor<bool>) conformanceChecker);
    }

    private Sql8ConformanceChecker()
    {
    }

    private bool VisitUnaryExpression(DbUnaryExpression expr)
    {
      return this.VisitExpression(expr.Argument);
    }

    private bool VisitBinaryExpression(DbBinaryExpression expr)
    {
      bool flag1 = this.VisitExpression(expr.Left);
      bool flag2 = this.VisitExpression(expr.Right);
      if (!flag1)
        return flag2;
      return true;
    }

    private bool VisitAggregate(DbAggregate aggregate)
    {
      return this.VisitExpressionList(aggregate.Arguments);
    }

    private bool VisitExpressionBinding(DbExpressionBinding expressionBinding)
    {
      return this.VisitExpression(expressionBinding.Expression);
    }

    private bool VisitExpression(DbExpression expression)
    {
      if (expression == null)
        return false;
      return expression.Accept<bool>((DbExpressionVisitor<bool>) this);
    }

    private bool VisitSortClause(DbSortClause sortClause)
    {
      return this.VisitExpression(sortClause.Expression);
    }

    private static bool VisitList<TElementType>(
      Sql8ConformanceChecker.ListElementHandler<TElementType> handler,
      IList<TElementType> list)
    {
      bool flag1 = false;
      foreach (TElementType element in (IEnumerable<TElementType>) list)
      {
        bool flag2 = handler(element);
        flag1 = flag1 || flag2;
      }
      return flag1;
    }

    private bool VisitAggregateList(IList<DbAggregate> list)
    {
      return Sql8ConformanceChecker.VisitList<DbAggregate>(new Sql8ConformanceChecker.ListElementHandler<DbAggregate>(this.VisitAggregate), list);
    }

    private bool VisitExpressionBindingList(IList<DbExpressionBinding> list)
    {
      return Sql8ConformanceChecker.VisitList<DbExpressionBinding>(new Sql8ConformanceChecker.ListElementHandler<DbExpressionBinding>(this.VisitExpressionBinding), list);
    }

    private bool VisitExpressionList(IList<DbExpression> list)
    {
      return Sql8ConformanceChecker.VisitList<DbExpression>(new Sql8ConformanceChecker.ListElementHandler<DbExpression>(this.VisitExpression), list);
    }

    private bool VisitSortClauseList(IList<DbSortClause> list)
    {
      return Sql8ConformanceChecker.VisitList<DbSortClause>(new Sql8ConformanceChecker.ListElementHandler<DbSortClause>(this.VisitSortClause), list);
    }

    public override bool Visit(DbExpression expression)
    {
      Check.NotNull<DbExpression>(expression, nameof (expression));
      throw new NotSupportedException(Strings.Cqt_General_UnsupportedExpression((object) expression.GetType().FullName));
    }

    public override bool Visit(DbAndExpression expression)
    {
      Check.NotNull<DbAndExpression>(expression, nameof (expression));
      return this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override bool Visit(DbApplyExpression expression)
    {
      Check.NotNull<DbApplyExpression>(expression, nameof (expression));
      throw new NotSupportedException(Strings.SqlGen_ApplyNotSupportedOnSql8);
    }

    public override bool Visit(DbArithmeticExpression expression)
    {
      Check.NotNull<DbArithmeticExpression>(expression, nameof (expression));
      return this.VisitExpressionList(expression.Arguments);
    }

    public override bool Visit(DbCaseExpression expression)
    {
      Check.NotNull<DbCaseExpression>(expression, nameof (expression));
      bool flag1 = this.VisitExpressionList(expression.When);
      bool flag2 = this.VisitExpressionList(expression.Then);
      bool flag3 = this.VisitExpression(expression.Else);
      if (!flag1 && !flag2)
        return flag3;
      return true;
    }

    public override bool Visit(DbCastExpression expression)
    {
      Check.NotNull<DbCastExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbComparisonExpression expression)
    {
      Check.NotNull<DbComparisonExpression>(expression, nameof (expression));
      return this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override bool Visit(DbConstantExpression expression)
    {
      Check.NotNull<DbConstantExpression>(expression, nameof (expression));
      return false;
    }

    public override bool Visit(DbCrossJoinExpression expression)
    {
      Check.NotNull<DbCrossJoinExpression>(expression, nameof (expression));
      return this.VisitExpressionBindingList(expression.Inputs);
    }

    public override bool Visit(DbDerefExpression expression)
    {
      Check.NotNull<DbDerefExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbDistinctExpression expression)
    {
      Check.NotNull<DbDistinctExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbElementExpression expression)
    {
      Check.NotNull<DbElementExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbEntityRefExpression expression)
    {
      Check.NotNull<DbEntityRefExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbExceptExpression expression)
    {
      Check.NotNull<DbExceptExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Left);
      this.VisitExpression(expression.Right);
      return true;
    }

    public override bool Visit(DbFilterExpression expression)
    {
      Check.NotNull<DbFilterExpression>(expression, nameof (expression));
      bool flag1 = this.VisitExpressionBinding(expression.Input);
      bool flag2 = this.VisitExpression(expression.Predicate);
      if (!flag1)
        return flag2;
      return true;
    }

    public override bool Visit(DbFunctionExpression expression)
    {
      Check.NotNull<DbFunctionExpression>(expression, nameof (expression));
      return this.VisitExpressionList(expression.Arguments);
    }

    public override bool Visit(DbLambdaExpression expression)
    {
      Check.NotNull<DbLambdaExpression>(expression, nameof (expression));
      bool flag1 = this.VisitExpressionList(expression.Arguments);
      bool flag2 = this.VisitExpression(expression.Lambda.Body);
      if (!flag1)
        return flag2;
      return true;
    }

    public override bool Visit(DbGroupByExpression expression)
    {
      Check.NotNull<DbGroupByExpression>(expression, nameof (expression));
      bool flag1 = this.VisitExpression(expression.Input.Expression);
      bool flag2 = this.VisitExpressionList(expression.Keys);
      bool flag3 = this.VisitAggregateList(expression.Aggregates);
      if (!flag1 && !flag2)
        return flag3;
      return true;
    }

    public override bool Visit(DbIntersectExpression expression)
    {
      Check.NotNull<DbIntersectExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Left);
      this.VisitExpression(expression.Right);
      return true;
    }

    public override bool Visit(DbIsEmptyExpression expression)
    {
      Check.NotNull<DbIsEmptyExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbIsNullExpression expression)
    {
      Check.NotNull<DbIsNullExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbIsOfExpression expression)
    {
      Check.NotNull<DbIsOfExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbJoinExpression expression)
    {
      Check.NotNull<DbJoinExpression>(expression, nameof (expression));
      bool flag1 = this.VisitExpressionBinding(expression.Left);
      bool flag2 = this.VisitExpressionBinding(expression.Right);
      bool flag3 = this.VisitExpression(expression.JoinCondition);
      if (!flag1 && !flag2)
        return flag3;
      return true;
    }

    public override bool Visit(DbLikeExpression expression)
    {
      Check.NotNull<DbLikeExpression>(expression, nameof (expression));
      bool flag1 = this.VisitExpression(expression.Argument);
      bool flag2 = this.VisitExpression(expression.Pattern);
      bool flag3 = this.VisitExpression(expression.Escape);
      if (!flag1 && !flag2)
        return flag3;
      return true;
    }

    public override bool Visit(DbLimitExpression expression)
    {
      Check.NotNull<DbLimitExpression>(expression, nameof (expression));
      if (expression.Limit is DbParameterReferenceExpression)
        throw new NotSupportedException(Strings.SqlGen_ParameterForLimitNotSupportedOnSql8);
      return this.VisitExpression(expression.Argument);
    }

    public override bool Visit(DbNewInstanceExpression expression)
    {
      Check.NotNull<DbNewInstanceExpression>(expression, nameof (expression));
      return this.VisitExpressionList(expression.Arguments);
    }

    public override bool Visit(DbNotExpression expression)
    {
      Check.NotNull<DbNotExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbNullExpression expression)
    {
      Check.NotNull<DbNullExpression>(expression, nameof (expression));
      return false;
    }

    public override bool Visit(DbOfTypeExpression expression)
    {
      Check.NotNull<DbOfTypeExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbOrExpression expression)
    {
      Check.NotNull<DbOrExpression>(expression, nameof (expression));
      return this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override bool Visit(DbInExpression expression)
    {
      Check.NotNull<DbInExpression>(expression, nameof (expression));
      if (!this.VisitExpression(expression.Item))
        return this.VisitExpressionList(expression.List);
      return true;
    }

    public override bool Visit(DbParameterReferenceExpression expression)
    {
      Check.NotNull<DbParameterReferenceExpression>(expression, nameof (expression));
      return false;
    }

    public override bool Visit(DbProjectExpression expression)
    {
      Check.NotNull<DbProjectExpression>(expression, nameof (expression));
      bool flag1 = this.VisitExpressionBinding(expression.Input);
      bool flag2 = this.VisitExpression(expression.Projection);
      if (!flag1)
        return flag2;
      return true;
    }

    public override bool Visit(DbPropertyExpression expression)
    {
      Check.NotNull<DbPropertyExpression>(expression, nameof (expression));
      return this.VisitExpression(expression.Instance);
    }

    public override bool Visit(DbQuantifierExpression expression)
    {
      Check.NotNull<DbQuantifierExpression>(expression, nameof (expression));
      bool flag1 = this.VisitExpressionBinding(expression.Input);
      bool flag2 = this.VisitExpression(expression.Predicate);
      if (!flag1)
        return flag2;
      return true;
    }

    public override bool Visit(DbRefExpression expression)
    {
      Check.NotNull<DbRefExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbRefKeyExpression expression)
    {
      Check.NotNull<DbRefKeyExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbRelationshipNavigationExpression expression)
    {
      Check.NotNull<DbRelationshipNavigationExpression>(expression, nameof (expression));
      return this.VisitExpression(expression.NavigationSource);
    }

    public override bool Visit(DbScanExpression expression)
    {
      Check.NotNull<DbScanExpression>(expression, nameof (expression));
      return false;
    }

    public override bool Visit(DbSkipExpression expression)
    {
      Check.NotNull<DbSkipExpression>(expression, nameof (expression));
      if (expression.Count is DbParameterReferenceExpression)
        throw new NotSupportedException(Strings.SqlGen_ParameterForSkipNotSupportedOnSql8);
      this.VisitExpressionBinding(expression.Input);
      this.VisitSortClauseList(expression.SortOrder);
      this.VisitExpression(expression.Count);
      return true;
    }

    public override bool Visit(DbSortExpression expression)
    {
      Check.NotNull<DbSortExpression>(expression, nameof (expression));
      bool flag1 = this.VisitExpressionBinding(expression.Input);
      bool flag2 = this.VisitSortClauseList(expression.SortOrder);
      if (!flag1)
        return flag2;
      return true;
    }

    public override bool Visit(DbTreatExpression expression)
    {
      Check.NotNull<DbTreatExpression>(expression, nameof (expression));
      return this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override bool Visit(DbUnionAllExpression expression)
    {
      Check.NotNull<DbUnionAllExpression>(expression, nameof (expression));
      return this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override bool Visit(DbVariableReferenceExpression expression)
    {
      Check.NotNull<DbVariableReferenceExpression>(expression, nameof (expression));
      return false;
    }

    private delegate bool ListElementHandler<TElementType>(TElementType element);
  }
}
