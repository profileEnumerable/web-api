// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.BasicExpressionVisitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>
  /// An abstract base type for types that implement the IExpressionVisitor interface to derive from.
  /// </summary>
  public abstract class BasicExpressionVisitor : DbExpressionVisitor
  {
    /// <summary>
    /// Convenience method to visit the specified <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbUnaryExpression" />.
    /// </summary>
    /// <param name="expression"> The DbUnaryExpression to visit. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    protected virtual void VisitUnaryExpression(DbUnaryExpression expression)
    {
      Check.NotNull<DbUnaryExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Argument);
    }

    /// <summary>
    /// Convenience method to visit the specified <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbBinaryExpression" />.
    /// </summary>
    /// <param name="expression"> The DbBinaryExpression to visit. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    protected virtual void VisitBinaryExpression(DbBinaryExpression expression)
    {
      Check.NotNull<DbBinaryExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Left);
      this.VisitExpression(expression.Right);
    }

    /// <summary>
    /// Convenience method to visit the specified <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" />.
    /// </summary>
    /// <param name="binding"> The DbExpressionBinding to visit. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="binding" />
    /// is null
    /// </exception>
    protected virtual void VisitExpressionBindingPre(DbExpressionBinding binding)
    {
      Check.NotNull<DbExpressionBinding>(binding, nameof (binding));
      this.VisitExpression(binding.Expression);
    }

    /// <summary>
    /// Convenience method for post-processing after a DbExpressionBinding has been visited.
    /// </summary>
    /// <param name="binding"> The previously visited DbExpressionBinding. </param>
    protected virtual void VisitExpressionBindingPost(DbExpressionBinding binding)
    {
    }

    /// <summary>
    /// Convenience method to visit the specified <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupExpressionBinding" />.
    /// </summary>
    /// <param name="binding"> The DbGroupExpressionBinding to visit. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="binding" />
    /// is null
    /// </exception>
    protected virtual void VisitGroupExpressionBindingPre(DbGroupExpressionBinding binding)
    {
      Check.NotNull<DbGroupExpressionBinding>(binding, nameof (binding));
      this.VisitExpression(binding.Expression);
    }

    /// <summary>
    /// Convenience method indicating that the grouping keys of a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupByExpression" /> have been visited and the aggregates are now about to be visited.
    /// </summary>
    /// <param name="binding"> The DbGroupExpressionBinding of the DbGroupByExpression </param>
    protected virtual void VisitGroupExpressionBindingMid(DbGroupExpressionBinding binding)
    {
    }

    /// <summary>
    /// Convenience method for post-processing after a DbGroupExpressionBinding has been visited.
    /// </summary>
    /// <param name="binding"> The previously visited DbGroupExpressionBinding. </param>
    protected virtual void VisitGroupExpressionBindingPost(DbGroupExpressionBinding binding)
    {
    }

    /// <summary>
    /// Convenience method indicating that the body of a Lambda <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> is now about to be visited.
    /// </summary>
    /// <param name="lambda"> The DbLambda that is about to be visited </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="lambda" />
    /// is null
    /// </exception>
    protected virtual void VisitLambdaPre(DbLambda lambda)
    {
      Check.NotNull<DbLambda>(lambda, nameof (lambda));
    }

    /// <summary>
    /// Convenience method for post-processing after a DbLambda has been visited.
    /// </summary>
    /// <param name="lambda"> The previously visited DbLambda. </param>
    protected virtual void VisitLambdaPost(DbLambda lambda)
    {
    }

    /// <summary>
    /// Convenience method to visit the specified <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />, if non-null.
    /// </summary>
    /// <param name="expression"> The expression to visit. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public virtual void VisitExpression(DbExpression expression)
    {
      Check.NotNull<DbExpression>(expression, nameof (expression));
      expression.Accept((DbExpressionVisitor) this);
    }

    /// <summary>
    /// Convenience method to visit each <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> in the given list, if the list is non-null.
    /// </summary>
    /// <param name="expressionList"> The list of expressions to visit. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expressionList" />
    /// is null
    /// </exception>
    public virtual void VisitExpressionList(IList<DbExpression> expressionList)
    {
      Check.NotNull<IList<DbExpression>>(expressionList, nameof (expressionList));
      for (int index = 0; index < expressionList.Count; ++index)
        this.VisitExpression(expressionList[index]);
    }

    /// <summary>
    /// Convenience method to visit each <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbAggregate" /> in the list, if the list is non-null.
    /// </summary>
    /// <param name="aggregates"> The list of aggregates to visit. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="aggregates" />
    /// is null
    /// </exception>
    public virtual void VisitAggregateList(IList<DbAggregate> aggregates)
    {
      Check.NotNull<IList<DbAggregate>>(aggregates, nameof (aggregates));
      for (int index = 0; index < aggregates.Count; ++index)
        this.VisitAggregate(aggregates[index]);
    }

    /// <summary>
    /// Convenience method to visit the specified <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbAggregate" />.
    /// </summary>
    /// <param name="aggregate"> The aggregate to visit. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="aggregate" />
    /// is null
    /// </exception>
    public virtual void VisitAggregate(DbAggregate aggregate)
    {
      Check.NotNull<DbAggregate>(aggregate, nameof (aggregate));
      this.VisitExpressionList(aggregate.Arguments);
    }

    internal virtual void VisitRelatedEntityReferenceList(
      IList<DbRelatedEntityRef> relatedEntityReferences)
    {
      for (int index = 0; index < relatedEntityReferences.Count; ++index)
        this.VisitRelatedEntityReference(relatedEntityReferences[index]);
    }

    internal virtual void VisitRelatedEntityReference(DbRelatedEntityRef relatedEntityRef)
    {
      this.VisitExpression(relatedEntityRef.TargetEntityReference);
    }

    /// <summary>
    /// Called when an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> of an otherwise unrecognized type is encountered.
    /// </summary>
    /// <param name="expression"> The expression </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">
    /// Always thrown if this method is called, since it indicates that
    /// <paramref name="expression" />
    /// is of an unsupported type
    /// </exception>
    public override void Visit(DbExpression expression)
    {
      Check.NotNull<DbExpression>(expression, nameof (expression));
      throw new NotSupportedException(Strings.Cqt_General_UnsupportedExpression((object) expression.GetType().FullName));
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression" />.
    /// </summary>
    /// <param name="expression"> The DbConstantExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbConstantExpression expression)
    {
      Check.NotNull<DbConstantExpression>(expression, nameof (expression));
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNullExpression" />.
    /// </summary>
    /// <param name="expression"> The DbNullExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbNullExpression expression)
    {
      Check.NotNull<DbNullExpression>(expression, nameof (expression));
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbVariableReferenceExpression" />.
    /// </summary>
    /// <param name="expression"> The DbVariableReferenceExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbVariableReferenceExpression expression)
    {
      Check.NotNull<DbVariableReferenceExpression>(expression, nameof (expression));
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbParameterReferenceExpression" />.
    /// </summary>
    /// <param name="expression"> The DbParameterReferenceExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbParameterReferenceExpression expression)
    {
      Check.NotNull<DbParameterReferenceExpression>(expression, nameof (expression));
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" />.
    /// </summary>
    /// <param name="expression"> The DbFunctionExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbFunctionExpression expression)
    {
      Check.NotNull<DbFunctionExpression>(expression, nameof (expression));
      this.VisitExpressionList(expression.Arguments);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambdaExpression" />.
    /// </summary>
    /// <param name="expression"> The DbLambdaExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbLambdaExpression expression)
    {
      Check.NotNull<DbLambdaExpression>(expression, nameof (expression));
      this.VisitExpressionList(expression.Arguments);
      this.VisitLambdaPre(expression.Lambda);
      this.VisitExpression(expression.Lambda.Body);
      this.VisitLambdaPost(expression.Lambda);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbPropertyExpression" />.
    /// </summary>
    /// <param name="expression"> The DbPropertyExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbPropertyExpression expression)
    {
      Check.NotNull<DbPropertyExpression>(expression, nameof (expression));
      if (expression.Instance == null)
        return;
      this.VisitExpression(expression.Instance);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbComparisonExpression" />.
    /// </summary>
    /// <param name="expression"> The DbComparisonExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbComparisonExpression expression)
    {
      Check.NotNull<DbComparisonExpression>(expression, nameof (expression));
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLikeExpression" />.
    /// </summary>
    /// <param name="expression"> The DbLikeExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbLikeExpression expression)
    {
      Check.NotNull<DbLikeExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Argument);
      this.VisitExpression(expression.Pattern);
      this.VisitExpression(expression.Escape);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLimitExpression" />.
    /// </summary>
    /// <param name="expression"> The DbLimitExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbLimitExpression expression)
    {
      Check.NotNull<DbLimitExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Argument);
      this.VisitExpression(expression.Limit);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIsNullExpression" />.
    /// </summary>
    /// <param name="expression"> The DbIsNullExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbIsNullExpression expression)
    {
      Check.NotNull<DbIsNullExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbArithmeticExpression" />.
    /// </summary>
    /// <param name="expression"> The DbArithmeticExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbArithmeticExpression expression)
    {
      Check.NotNull<DbArithmeticExpression>(expression, nameof (expression));
      this.VisitExpressionList(expression.Arguments);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbAndExpression" />.
    /// </summary>
    /// <param name="expression"> The DbAndExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbAndExpression expression)
    {
      Check.NotNull<DbAndExpression>(expression, nameof (expression));
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbOrExpression" />.
    /// </summary>
    /// <param name="expression"> The DbOrExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbOrExpression expression)
    {
      Check.NotNull<DbOrExpression>(expression, nameof (expression));
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbInExpression" />.
    /// </summary>
    /// <param name="expression"> The DbInExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbInExpression expression)
    {
      Check.NotNull<DbInExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Item);
      this.VisitExpressionList(expression.List);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNotExpression" />.
    /// </summary>
    /// <param name="expression"> The DbNotExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbNotExpression expression)
    {
      Check.NotNull<DbNotExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbDistinctExpression" />.
    /// </summary>
    /// <param name="expression"> The DbDistinctExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbDistinctExpression expression)
    {
      Check.NotNull<DbDistinctExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbElementExpression" />.
    /// </summary>
    /// <param name="expression"> The DbElementExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbElementExpression expression)
    {
      Check.NotNull<DbElementExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIsEmptyExpression" />.
    /// </summary>
    /// <param name="expression"> The DbIsEmptyExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbIsEmptyExpression expression)
    {
      Check.NotNull<DbIsEmptyExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbUnionAllExpression" />.
    /// </summary>
    /// <param name="expression"> The DbUnionAllExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbUnionAllExpression expression)
    {
      Check.NotNull<DbUnionAllExpression>(expression, nameof (expression));
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIntersectExpression" />.
    /// </summary>
    /// <param name="expression"> The DbIntersectExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbIntersectExpression expression)
    {
      Check.NotNull<DbIntersectExpression>(expression, nameof (expression));
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExceptExpression" />.
    /// </summary>
    /// <param name="expression"> The DbExceptExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbExceptExpression expression)
    {
      Check.NotNull<DbExceptExpression>(expression, nameof (expression));
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbOfTypeExpression" />.
    /// </summary>
    /// <param name="expression"> The DbOfTypeExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbOfTypeExpression expression)
    {
      Check.NotNull<DbOfTypeExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbTreatExpression" />.
    /// </summary>
    /// <param name="expression"> The DbTreatExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbTreatExpression expression)
    {
      Check.NotNull<DbTreatExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCastExpression" />.
    /// </summary>
    /// <param name="expression"> The DbCastExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbCastExpression expression)
    {
      Check.NotNull<DbCastExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIsOfExpression" />.
    /// </summary>
    /// <param name="expression"> The DbIsOfExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbIsOfExpression expression)
    {
      Check.NotNull<DbIsOfExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />.
    /// </summary>
    /// <param name="expression"> The DbCaseExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbCaseExpression expression)
    {
      Check.NotNull<DbCaseExpression>(expression, nameof (expression));
      this.VisitExpressionList(expression.When);
      this.VisitExpressionList(expression.Then);
      this.VisitExpression(expression.Else);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNewInstanceExpression" />.
    /// </summary>
    /// <param name="expression"> The DbNewInstanceExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbNewInstanceExpression expression)
    {
      Check.NotNull<DbNewInstanceExpression>(expression, nameof (expression));
      this.VisitExpressionList(expression.Arguments);
      if (!expression.HasRelatedEntityReferences)
        return;
      this.VisitRelatedEntityReferenceList((IList<DbRelatedEntityRef>) expression.RelatedEntityReferences);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRefExpression" />.
    /// </summary>
    /// <param name="expression"> The DbRefExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbRefExpression expression)
    {
      Check.NotNull<DbRefExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRelationshipNavigationExpression" />.
    /// </summary>
    /// <param name="expression"> The DbRelationshipNavigationExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbRelationshipNavigationExpression expression)
    {
      Check.NotNull<DbRelationshipNavigationExpression>(expression, nameof (expression));
      this.VisitExpression(expression.NavigationSource);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbDerefExpression" />.
    /// </summary>
    /// <param name="expression"> The DeRefExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbDerefExpression expression)
    {
      Check.NotNull<DbDerefExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRefKeyExpression" />.
    /// </summary>
    /// <param name="expression"> The DbRefKeyExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbRefKeyExpression expression)
    {
      Check.NotNull<DbRefKeyExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbEntityRefExpression" />.
    /// </summary>
    /// <param name="expression"> The DbEntityRefExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbEntityRefExpression expression)
    {
      Check.NotNull<DbEntityRefExpression>(expression, nameof (expression));
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbScanExpression" />.
    /// </summary>
    /// <param name="expression"> The DbScanExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbScanExpression expression)
    {
      Check.NotNull<DbScanExpression>(expression, nameof (expression));
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFilterExpression" />.
    /// </summary>
    /// <param name="expression"> The DbFilterExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbFilterExpression expression)
    {
      Check.NotNull<DbFilterExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      this.VisitExpression(expression.Predicate);
      this.VisitExpressionBindingPost(expression.Input);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbProjectExpression" />.
    /// </summary>
    /// <param name="expression"> The DbProjectExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbProjectExpression expression)
    {
      Check.NotNull<DbProjectExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      this.VisitExpression(expression.Projection);
      this.VisitExpressionBindingPost(expression.Input);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCrossJoinExpression" />.
    /// </summary>
    /// <param name="expression"> The DbCrossJoinExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbCrossJoinExpression expression)
    {
      Check.NotNull<DbCrossJoinExpression>(expression, nameof (expression));
      foreach (DbExpressionBinding input in (IEnumerable<DbExpressionBinding>) expression.Inputs)
        this.VisitExpressionBindingPre(input);
      foreach (DbExpressionBinding input in (IEnumerable<DbExpressionBinding>) expression.Inputs)
        this.VisitExpressionBindingPost(input);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbJoinExpression" />.
    /// </summary>
    /// <param name="expression"> The DbJoinExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbJoinExpression expression)
    {
      Check.NotNull<DbJoinExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Left);
      this.VisitExpressionBindingPre(expression.Right);
      this.VisitExpression(expression.JoinCondition);
      this.VisitExpressionBindingPost(expression.Left);
      this.VisitExpressionBindingPost(expression.Right);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbApplyExpression" />.
    /// </summary>
    /// <param name="expression"> The DbApplyExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbApplyExpression expression)
    {
      Check.NotNull<DbApplyExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      if (expression.Apply != null)
        this.VisitExpression(expression.Apply.Expression);
      this.VisitExpressionBindingPost(expression.Input);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupByExpression" />.
    /// </summary>
    /// <param name="expression"> The DbExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbGroupByExpression expression)
    {
      Check.NotNull<DbGroupByExpression>(expression, nameof (expression));
      this.VisitGroupExpressionBindingPre(expression.Input);
      this.VisitExpressionList(expression.Keys);
      this.VisitGroupExpressionBindingMid(expression.Input);
      this.VisitAggregateList(expression.Aggregates);
      this.VisitGroupExpressionBindingPost(expression.Input);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSkipExpression" />.
    /// </summary>
    /// <param name="expression"> The DbSkipExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbSkipExpression expression)
    {
      Check.NotNull<DbSkipExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      foreach (DbSortClause dbSortClause in (IEnumerable<DbSortClause>) expression.SortOrder)
        this.VisitExpression(dbSortClause.Expression);
      this.VisitExpressionBindingPost(expression.Input);
      this.VisitExpression(expression.Count);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" />.
    /// </summary>
    /// <param name="expression"> The DbSortExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbSortExpression expression)
    {
      Check.NotNull<DbSortExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      for (int index = 0; index < expression.SortOrder.Count; ++index)
        this.VisitExpression(expression.SortOrder[index].Expression);
      this.VisitExpressionBindingPost(expression.Input);
    }

    /// <summary>
    /// Visitor pattern method for <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbQuantifierExpression" />.
    /// </summary>
    /// <param name="expression"> The DbQuantifierExpression that is being visited. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// is null
    /// </exception>
    public override void Visit(DbQuantifierExpression expression)
    {
      Check.NotNull<DbQuantifierExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      this.VisitExpression(expression.Predicate);
      this.VisitExpressionBindingPost(expression.Input);
    }
  }
}
