// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ELinq.CompiledELinqQueryState
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.QueryCache;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.Internal;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Core.Objects.ELinq
{
  internal sealed class CompiledELinqQueryState : ELinqQueryState
  {
    private readonly Guid _cacheToken;
    private readonly object[] _parameterValues;
    private CompiledQueryCacheEntry _cacheEntry;
    private readonly ObjectQueryExecutionPlanFactory _objectQueryExecutionPlanFactory;

    internal CompiledELinqQueryState(
      Type elementType,
      ObjectContext context,
      LambdaExpression lambda,
      Guid cacheToken,
      object[] parameterValues,
      ObjectQueryExecutionPlanFactory objectQueryExecutionPlanFactory = null)
      : base(elementType, context, (Expression) lambda, (ObjectQueryExecutionPlanFactory) null)
    {
      this._cacheToken = cacheToken;
      this._parameterValues = parameterValues;
      this.EnsureParameters();
      this.Parameters.SetReadOnly(true);
      this._objectQueryExecutionPlanFactory = objectQueryExecutionPlanFactory ?? new ObjectQueryExecutionPlanFactory((System.Data.Entity.Core.Common.Internal.Materialization.Translator) null);
    }

    internal override ObjectQueryExecutionPlan GetExecutionPlan(
      MergeOption? forMergeOption)
    {
      ObjectQueryExecutionPlan queryExecutionPlan = (ObjectQueryExecutionPlan) null;
      CompiledQueryCacheEntry compiledQueryCacheEntry = this._cacheEntry;
      bool comparisonBehavior = this.ObjectContext.ContextOptions.UseCSharpNullComparisonBehavior;
      if (compiledQueryCacheEntry != null)
      {
        MergeOption mergeOption = ObjectQueryState.EnsureMergeOption(forMergeOption, this.UserSpecifiedMergeOption, compiledQueryCacheEntry.PropagatedMergeOption);
        queryExecutionPlan = compiledQueryCacheEntry.GetExecutionPlan(mergeOption, comparisonBehavior);
        if (queryExecutionPlan == null)
        {
          ExpressionConverter expressionConverter = this.CreateExpressionConverter();
          DbExpression query = expressionConverter.Convert();
          IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>> parameters = expressionConverter.GetParameters();
          ObjectQueryExecutionPlan newPlan = this._objectQueryExecutionPlanFactory.Prepare(this.ObjectContext, DbQueryCommandTree.FromValidExpression(this.ObjectContext.MetadataWorkspace, DataSpace.CSpace, query, !comparisonBehavior), this.ElementType, mergeOption, this.EffectiveStreamingBehavior, expressionConverter.PropagatedSpan, parameters, expressionConverter.AliasGenerator);
          queryExecutionPlan = compiledQueryCacheEntry.SetExecutionPlan(newPlan, comparisonBehavior);
        }
      }
      else
      {
        QueryCacheManager queryCacheManager = this.ObjectContext.MetadataWorkspace.GetQueryCacheManager();
        CompiledQueryCacheKey key = new CompiledQueryCacheKey(this._cacheToken);
        if (queryCacheManager.TryCacheLookup<CompiledQueryCacheKey, CompiledQueryCacheEntry>(key, out compiledQueryCacheEntry))
        {
          this._cacheEntry = compiledQueryCacheEntry;
          MergeOption mergeOption = ObjectQueryState.EnsureMergeOption(forMergeOption, this.UserSpecifiedMergeOption, compiledQueryCacheEntry.PropagatedMergeOption);
          queryExecutionPlan = compiledQueryCacheEntry.GetExecutionPlan(mergeOption, comparisonBehavior);
        }
        if (queryExecutionPlan == null)
        {
          ExpressionConverter expressionConverter = this.CreateExpressionConverter();
          DbExpression query = expressionConverter.Convert();
          IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>> parameters = expressionConverter.GetParameters();
          DbQueryCommandTree tree = DbQueryCommandTree.FromValidExpression(this.ObjectContext.MetadataWorkspace, DataSpace.CSpace, query, !comparisonBehavior);
          if (compiledQueryCacheEntry == null)
          {
            compiledQueryCacheEntry = new CompiledQueryCacheEntry((QueryCacheKey) key, expressionConverter.PropagatedMergeOption);
            QueryCacheEntry outQueryCacheEntry;
            if (queryCacheManager.TryLookupAndAdd((QueryCacheEntry) compiledQueryCacheEntry, out outQueryCacheEntry))
              compiledQueryCacheEntry = (CompiledQueryCacheEntry) outQueryCacheEntry;
            this._cacheEntry = compiledQueryCacheEntry;
          }
          MergeOption mergeOption = ObjectQueryState.EnsureMergeOption(forMergeOption, this.UserSpecifiedMergeOption, compiledQueryCacheEntry.PropagatedMergeOption);
          queryExecutionPlan = compiledQueryCacheEntry.GetExecutionPlan(mergeOption, comparisonBehavior);
          if (queryExecutionPlan == null)
          {
            ObjectQueryExecutionPlan newPlan = this._objectQueryExecutionPlanFactory.Prepare(this.ObjectContext, tree, this.ElementType, mergeOption, this.EffectiveStreamingBehavior, expressionConverter.PropagatedSpan, parameters, expressionConverter.AliasGenerator);
            queryExecutionPlan = compiledQueryCacheEntry.SetExecutionPlan(newPlan, comparisonBehavior);
          }
        }
      }
      ObjectParameterCollection parameterCollection = this.EnsureParameters();
      if (queryExecutionPlan.CompiledQueryParameters != null && queryExecutionPlan.CompiledQueryParameters.Any<Tuple<ObjectParameter, QueryParameterExpression>>())
      {
        parameterCollection.SetReadOnly(false);
        parameterCollection.Clear();
        foreach (Tuple<ObjectParameter, QueryParameterExpression> compiledQueryParameter in queryExecutionPlan.CompiledQueryParameters)
        {
          ObjectParameter objectParameter = compiledQueryParameter.Item1.ShallowCopy();
          QueryParameterExpression parameterExpression = compiledQueryParameter.Item2;
          parameterCollection.Add(objectParameter);
          if (parameterExpression != null)
            objectParameter.Value = parameterExpression.EvaluateParameter(this._parameterValues);
        }
      }
      parameterCollection.SetReadOnly(true);
      return queryExecutionPlan;
    }

    protected override TypeUsage GetResultType()
    {
      CompiledQueryCacheEntry cacheEntry = this._cacheEntry;
      TypeUsage resultType;
      if (cacheEntry != null && cacheEntry.TryGetResultType(out resultType))
        return resultType;
      return base.GetResultType();
    }

    internal override Expression Expression
    {
      get
      {
        return CompiledELinqQueryState.CreateDonateableExpressionVisitor.Replace((LambdaExpression) base.Expression, this.ObjectContext, this._parameterValues);
      }
    }

    protected override ExpressionConverter CreateExpressionConverter()
    {
      LambdaExpression expression = (LambdaExpression) base.Expression;
      return new ExpressionConverter(Funcletizer.CreateCompiledQueryEvaluationFuncletizer(this.ObjectContext, expression.Parameters.First<ParameterExpression>(), new ReadOnlyCollection<ParameterExpression>((IList<ParameterExpression>) expression.Parameters.Skip<ParameterExpression>(1).ToList<ParameterExpression>())), expression.Body);
    }

    private sealed class CreateDonateableExpressionVisitor : EntityExpressionVisitor
    {
      private readonly Dictionary<ParameterExpression, object> _parameterToValueLookup;

      private CreateDonateableExpressionVisitor(
        Dictionary<ParameterExpression, object> parameterToValueLookup)
      {
        this._parameterToValueLookup = parameterToValueLookup;
      }

      internal static Expression Replace(
        LambdaExpression query,
        ObjectContext objectContext,
        object[] parameterValues)
      {
        Dictionary<ParameterExpression, object> dictionary = query.Parameters.Skip<ParameterExpression>(1).Zip<ParameterExpression, object>((IEnumerable<object>) parameterValues).ToDictionary<KeyValuePair<ParameterExpression, object>, ParameterExpression, object>((Func<KeyValuePair<ParameterExpression, object>, ParameterExpression>) (pair => pair.Key), (Func<KeyValuePair<ParameterExpression, object>, object>) (pair => pair.Value));
        dictionary.Add(query.Parameters.First<ParameterExpression>(), (object) objectContext);
        return new CompiledELinqQueryState.CreateDonateableExpressionVisitor(dictionary).Visit(query.Body);
      }

      internal override Expression VisitParameter(ParameterExpression p)
      {
        object obj;
        return !this._parameterToValueLookup.TryGetValue(p, out obj) ? base.VisitParameter(p) : (Expression) Expression.Constant(obj, p.Type);
      }
    }
  }
}
