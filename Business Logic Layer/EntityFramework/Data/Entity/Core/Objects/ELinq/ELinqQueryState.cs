// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ELinq.ELinqQueryState
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Common.QueryCache;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Objects.ELinq
{
  internal class ELinqQueryState : ObjectQueryState
  {
    private readonly Expression _expression;
    private Func<bool> _recompileRequired;
    private IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>> _linqParameters;
    private bool _useCSharpNullComparisonBehavior;
    private readonly ObjectQueryExecutionPlanFactory _objectQueryExecutionPlanFactory;

    internal ELinqQueryState(
      Type elementType,
      ObjectContext context,
      Expression expression,
      ObjectQueryExecutionPlanFactory objectQueryExecutionPlanFactory = null)
      : base(elementType, context, (ObjectParameterCollection) null, (Span) null)
    {
      this._expression = expression;
      this._useCSharpNullComparisonBehavior = context.ContextOptions.UseCSharpNullComparisonBehavior;
      this._objectQueryExecutionPlanFactory = objectQueryExecutionPlanFactory ?? new ObjectQueryExecutionPlanFactory((System.Data.Entity.Core.Common.Internal.Materialization.Translator) null);
    }

    internal ELinqQueryState(
      Type elementType,
      ObjectQuery query,
      Expression expression,
      ObjectQueryExecutionPlanFactory objectQueryExecutionPlanFactory = null)
      : base(elementType, query)
    {
      this._expression = expression;
      this._objectQueryExecutionPlanFactory = objectQueryExecutionPlanFactory ?? new ObjectQueryExecutionPlanFactory((System.Data.Entity.Core.Common.Internal.Materialization.Translator) null);
    }

    protected override TypeUsage GetResultType()
    {
      return this.CreateExpressionConverter().Convert().ResultType;
    }

    internal override ObjectQueryExecutionPlan GetExecutionPlan(
      MergeOption? forMergeOption)
    {
      ObjectQueryExecutionPlan queryExecutionPlan1 = this._cachedPlan;
      if (queryExecutionPlan1 != null)
      {
        MergeOption? mergeOption = ObjectQueryState.GetMergeOption(forMergeOption, this.UserSpecifiedMergeOption);
        if (mergeOption.HasValue && mergeOption.Value != queryExecutionPlan1.MergeOption || (this._recompileRequired() || this.ObjectContext.ContextOptions.UseCSharpNullComparisonBehavior != this._useCSharpNullComparisonBehavior))
          queryExecutionPlan1 = (ObjectQueryExecutionPlan) null;
      }
      if (queryExecutionPlan1 == null)
      {
        this._recompileRequired = (Func<bool>) null;
        this.ResetParameters();
        ExpressionConverter expressionConverter = this.CreateExpressionConverter();
        DbExpression dbExpression = expressionConverter.Convert();
        this._recompileRequired = expressionConverter.RecompileRequired;
        MergeOption mergeOption = ObjectQueryState.EnsureMergeOption(forMergeOption, this.UserSpecifiedMergeOption, expressionConverter.PropagatedMergeOption);
        this._useCSharpNullComparisonBehavior = this.ObjectContext.ContextOptions.UseCSharpNullComparisonBehavior;
        this._linqParameters = expressionConverter.GetParameters();
        if (this._linqParameters != null && this._linqParameters.Any<Tuple<ObjectParameter, QueryParameterExpression>>())
        {
          ObjectParameterCollection parameterCollection = this.EnsureParameters();
          parameterCollection.SetReadOnly(false);
          foreach (Tuple<ObjectParameter, QueryParameterExpression> linqParameter in this._linqParameters)
          {
            ObjectParameter objectParameter = linqParameter.Item1;
            parameterCollection.Add(objectParameter);
          }
          parameterCollection.SetReadOnly(true);
        }
        QueryCacheManager queryCacheManager = (QueryCacheManager) null;
        LinqQueryCacheKey key1 = (LinqQueryCacheKey) null;
        string key2;
        if (this.PlanCachingEnabled && !this._recompileRequired() && ExpressionKeyGen.TryGenerateKey(dbExpression, out key2))
        {
          key1 = new LinqQueryCacheKey(key2, this.Parameters == null ? 0 : this.Parameters.Count, this.Parameters == null ? (string) null : this.Parameters.GetCacheKey(), expressionConverter.PropagatedSpan == null ? (string) null : expressionConverter.PropagatedSpan.GetCacheKey(), mergeOption, this.EffectiveStreamingBehavior, this._useCSharpNullComparisonBehavior, this.ElementType);
          queryCacheManager = this.ObjectContext.MetadataWorkspace.GetQueryCacheManager();
          ObjectQueryExecutionPlan queryExecutionPlan2 = (ObjectQueryExecutionPlan) null;
          if (queryCacheManager.TryCacheLookup<LinqQueryCacheKey, ObjectQueryExecutionPlan>(key1, out queryExecutionPlan2))
            queryExecutionPlan1 = queryExecutionPlan2;
        }
        if (queryExecutionPlan1 == null)
        {
          queryExecutionPlan1 = this._objectQueryExecutionPlanFactory.Prepare(this.ObjectContext, DbQueryCommandTree.FromValidExpression(this.ObjectContext.MetadataWorkspace, DataSpace.CSpace, dbExpression, !this._useCSharpNullComparisonBehavior), this.ElementType, mergeOption, this.EffectiveStreamingBehavior, expressionConverter.PropagatedSpan, (IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>>) null, expressionConverter.AliasGenerator);
          if (key1 != null)
          {
            QueryCacheEntry inQueryCacheEntry = new QueryCacheEntry((QueryCacheKey) key1, (object) queryExecutionPlan1);
            QueryCacheEntry outQueryCacheEntry = (QueryCacheEntry) null;
            if (queryCacheManager.TryLookupAndAdd(inQueryCacheEntry, out outQueryCacheEntry))
              queryExecutionPlan1 = (ObjectQueryExecutionPlan) outQueryCacheEntry.GetTarget();
          }
        }
        this._cachedPlan = queryExecutionPlan1;
      }
      if (this._linqParameters != null)
      {
        foreach (Tuple<ObjectParameter, QueryParameterExpression> linqParameter in this._linqParameters)
        {
          ObjectParameter objectParameter = linqParameter.Item1;
          QueryParameterExpression parameterExpression = linqParameter.Item2;
          if (parameterExpression != null)
            objectParameter.Value = parameterExpression.EvaluateParameter((object[]) null);
        }
      }
      return queryExecutionPlan1;
    }

    internal override ObjectQueryState Include<TElementType>(
      ObjectQuery<TElementType> sourceQuery,
      string includePath)
    {
      MethodInfo includeMethod = ELinqQueryState.GetIncludeMethod<TElementType>(sourceQuery);
      ObjectQueryState other = (ObjectQueryState) new ELinqQueryState(this.ElementType, this.ObjectContext, (Expression) Expression.Call((Expression) Expression.Constant((object) sourceQuery), includeMethod, (Expression) Expression.Constant((object) includePath, typeof (string))), (ObjectQueryExecutionPlanFactory) null);
      this.ApplySettingsTo(other);
      return other;
    }

    internal static MethodInfo GetIncludeMethod<TElementType>(
      ObjectQuery<TElementType> sourceQuery)
    {
      return sourceQuery.GetType().GetOnlyDeclaredMethod("Include");
    }

    internal override bool TryGetCommandText(out string commandText)
    {
      commandText = (string) null;
      return false;
    }

    internal override bool TryGetExpression(out Expression expression)
    {
      expression = this.Expression;
      return true;
    }

    internal virtual Expression Expression
    {
      get
      {
        return this._expression;
      }
    }

    protected virtual ExpressionConverter CreateExpressionConverter()
    {
      return new ExpressionConverter(Funcletizer.CreateQueryFuncletizer(this.ObjectContext), this._expression);
    }

    private void ResetParameters()
    {
      if (this.Parameters != null)
      {
        bool isReadOnly = ((ICollection<ObjectParameter>) this.Parameters).IsReadOnly;
        if (isReadOnly)
          this.Parameters.SetReadOnly(false);
        this.Parameters.Clear();
        if (isReadOnly)
          this.Parameters.SetReadOnly(true);
      }
      this._linqParameters = (IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>>) null;
    }
  }
}
