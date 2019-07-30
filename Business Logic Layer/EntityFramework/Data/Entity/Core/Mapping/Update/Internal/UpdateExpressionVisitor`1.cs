// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.UpdateExpressionVisitor`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal abstract class UpdateExpressionVisitor<TReturn> : DbExpressionVisitor<TReturn>
  {
    protected abstract string VisitorName { get; }

    protected NotSupportedException ConstructNotSupportedException(
      DbExpression node)
    {
      return new NotSupportedException(Strings.Update_UnsupportedExpressionKind(node == null ? (object) (string) null : (object) node.ExpressionKind.ToString(), (object) this.VisitorName));
    }

    public override TReturn Visit(DbExpression expression)
    {
      Check.NotNull<DbExpression>(expression, nameof (expression));
      if (expression != null)
        return expression.Accept<TReturn>((DbExpressionVisitor<TReturn>) this);
      throw this.ConstructNotSupportedException(expression);
    }

    public override TReturn Visit(DbAndExpression expression)
    {
      Check.NotNull<DbAndExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbApplyExpression expression)
    {
      Check.NotNull<DbApplyExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbArithmeticExpression expression)
    {
      Check.NotNull<DbArithmeticExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbCaseExpression expression)
    {
      Check.NotNull<DbCaseExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbCastExpression expression)
    {
      Check.NotNull<DbCastExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbComparisonExpression expression)
    {
      Check.NotNull<DbComparisonExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbConstantExpression expression)
    {
      Check.NotNull<DbConstantExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbCrossJoinExpression expression)
    {
      Check.NotNull<DbCrossJoinExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbDerefExpression expression)
    {
      Check.NotNull<DbDerefExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbDistinctExpression expression)
    {
      Check.NotNull<DbDistinctExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbElementExpression expression)
    {
      Check.NotNull<DbElementExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbExceptExpression expression)
    {
      Check.NotNull<DbExceptExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbFilterExpression expression)
    {
      Check.NotNull<DbFilterExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbFunctionExpression expression)
    {
      Check.NotNull<DbFunctionExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbLambdaExpression expression)
    {
      Check.NotNull<DbLambdaExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbEntityRefExpression expression)
    {
      Check.NotNull<DbEntityRefExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbRefKeyExpression expression)
    {
      Check.NotNull<DbRefKeyExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbGroupByExpression expression)
    {
      Check.NotNull<DbGroupByExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbIntersectExpression expression)
    {
      Check.NotNull<DbIntersectExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbIsEmptyExpression expression)
    {
      Check.NotNull<DbIsEmptyExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbIsNullExpression expression)
    {
      Check.NotNull<DbIsNullExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbIsOfExpression expression)
    {
      Check.NotNull<DbIsOfExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbJoinExpression expression)
    {
      Check.NotNull<DbJoinExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbLikeExpression expression)
    {
      Check.NotNull<DbLikeExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbLimitExpression expression)
    {
      Check.NotNull<DbLimitExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbNewInstanceExpression expression)
    {
      Check.NotNull<DbNewInstanceExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbNotExpression expression)
    {
      Check.NotNull<DbNotExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbNullExpression expression)
    {
      Check.NotNull<DbNullExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbOfTypeExpression expression)
    {
      Check.NotNull<DbOfTypeExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbOrExpression expression)
    {
      Check.NotNull<DbOrExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbInExpression expression)
    {
      Check.NotNull<DbInExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbParameterReferenceExpression expression)
    {
      Check.NotNull<DbParameterReferenceExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbProjectExpression expression)
    {
      Check.NotNull<DbProjectExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbPropertyExpression expression)
    {
      Check.NotNull<DbPropertyExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbQuantifierExpression expression)
    {
      Check.NotNull<DbQuantifierExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbRefExpression expression)
    {
      Check.NotNull<DbRefExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbRelationshipNavigationExpression expression)
    {
      Check.NotNull<DbRelationshipNavigationExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbSkipExpression expression)
    {
      Check.NotNull<DbSkipExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbSortExpression expression)
    {
      Check.NotNull<DbSortExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbTreatExpression expression)
    {
      Check.NotNull<DbTreatExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbUnionAllExpression expression)
    {
      Check.NotNull<DbUnionAllExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbVariableReferenceExpression expression)
    {
      Check.NotNull<DbVariableReferenceExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }

    public override TReturn Visit(DbScanExpression expression)
    {
      Check.NotNull<DbScanExpression>(expression, nameof (expression));
      throw this.ConstructNotSupportedException((DbExpression) expression);
    }
  }
}
