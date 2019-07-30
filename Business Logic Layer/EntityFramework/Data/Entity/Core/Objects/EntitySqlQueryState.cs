// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.EntitySqlQueryState
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.Entity.Core.Common.QueryCache;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Utilities;
using System.Linq.Expressions;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class EntitySqlQueryState : ObjectQueryState
  {
    private readonly string _queryText;
    private readonly DbExpression _queryExpression;
    private readonly bool _allowsLimit;
    private readonly ObjectQueryExecutionPlanFactory _objectQueryExecutionPlanFactory;

    internal EntitySqlQueryState(
      Type elementType,
      string commandText,
      bool allowsLimit,
      ObjectContext context,
      ObjectParameterCollection parameters,
      Span span)
      : this(elementType, commandText, (DbExpression) null, allowsLimit, context, parameters, span, (ObjectQueryExecutionPlanFactory) null)
    {
    }

    internal EntitySqlQueryState(
      Type elementType,
      string commandText,
      DbExpression expression,
      bool allowsLimit,
      ObjectContext context,
      ObjectParameterCollection parameters,
      Span span,
      ObjectQueryExecutionPlanFactory objectQueryExecutionPlanFactory = null)
      : base(elementType, context, parameters, span)
    {
      Check.NotEmpty(commandText, nameof (commandText));
      this._queryText = commandText;
      this._queryExpression = expression;
      this._allowsLimit = allowsLimit;
      this._objectQueryExecutionPlanFactory = objectQueryExecutionPlanFactory ?? new ObjectQueryExecutionPlanFactory((System.Data.Entity.Core.Common.Internal.Materialization.Translator) null);
    }

    internal bool AllowsLimitSubclause
    {
      get
      {
        return this._allowsLimit;
      }
    }

    internal override bool TryGetCommandText(out string commandText)
    {
      commandText = this._queryText;
      return true;
    }

    internal override bool TryGetExpression(out Expression expression)
    {
      expression = (Expression) null;
      return false;
    }

    protected override TypeUsage GetResultType()
    {
      return this.Parse().ResultType;
    }

    internal override ObjectQueryState Include<TElementType>(
      ObjectQuery<TElementType> sourceQuery,
      string includePath)
    {
      ObjectQueryState other = (ObjectQueryState) new EntitySqlQueryState(this.ElementType, this._queryText, this._queryExpression, this._allowsLimit, this.ObjectContext, ObjectParameterCollection.DeepCopy(this.Parameters), Span.IncludeIn(this.Span, includePath), (ObjectQueryExecutionPlanFactory) null);
      this.ApplySettingsTo(other);
      return other;
    }

    internal override ObjectQueryExecutionPlan GetExecutionPlan(
      MergeOption? forMergeOption)
    {
      MergeOption mergeOption = ObjectQueryState.EnsureMergeOption(forMergeOption, this.UserSpecifiedMergeOption);
      ObjectQueryExecutionPlan queryExecutionPlan1 = this._cachedPlan;
      if (queryExecutionPlan1 != null)
      {
        if (queryExecutionPlan1.MergeOption == mergeOption && queryExecutionPlan1.Streaming == this.EffectiveStreamingBehavior)
          return queryExecutionPlan1;
        queryExecutionPlan1 = (ObjectQueryExecutionPlan) null;
      }
      QueryCacheManager queryCacheManager = (QueryCacheManager) null;
      EntitySqlQueryCacheKey key = (EntitySqlQueryCacheKey) null;
      if (this.PlanCachingEnabled)
      {
        key = new EntitySqlQueryCacheKey(this.ObjectContext.DefaultContainerName, this._queryText, this.Parameters == null ? 0 : this.Parameters.Count, this.Parameters == null ? (string) null : this.Parameters.GetCacheKey(), this.Span == null ? (string) null : this.Span.GetCacheKey(), mergeOption, this.EffectiveStreamingBehavior, this.ElementType);
        queryCacheManager = this.ObjectContext.MetadataWorkspace.GetQueryCacheManager();
        ObjectQueryExecutionPlan queryExecutionPlan2 = (ObjectQueryExecutionPlan) null;
        if (queryCacheManager.TryCacheLookup<EntitySqlQueryCacheKey, ObjectQueryExecutionPlan>(key, out queryExecutionPlan2))
          queryExecutionPlan1 = queryExecutionPlan2;
      }
      if (queryExecutionPlan1 == null)
      {
        queryExecutionPlan1 = this._objectQueryExecutionPlanFactory.Prepare(this.ObjectContext, DbQueryCommandTree.FromValidExpression(this.ObjectContext.MetadataWorkspace, DataSpace.CSpace, this.Parse(), true), this.ElementType, mergeOption, this.EffectiveStreamingBehavior, this.Span, (IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>>) null, DbExpressionBuilder.AliasGenerator);
        if (key != null)
        {
          QueryCacheEntry inQueryCacheEntry = new QueryCacheEntry((QueryCacheKey) key, (object) queryExecutionPlan1);
          QueryCacheEntry outQueryCacheEntry = (QueryCacheEntry) null;
          if (queryCacheManager.TryLookupAndAdd(inQueryCacheEntry, out outQueryCacheEntry))
            queryExecutionPlan1 = (ObjectQueryExecutionPlan) outQueryCacheEntry.GetTarget();
        }
      }
      if (this.Parameters != null)
        this.Parameters.SetReadOnly(true);
      this._cachedPlan = queryExecutionPlan1;
      return queryExecutionPlan1;
    }

    internal DbExpression Parse()
    {
      if (this._queryExpression != null)
        return this._queryExpression;
      List<DbParameterReferenceExpression> referenceExpressionList = (List<DbParameterReferenceExpression>) null;
      if (this.Parameters != null)
      {
        referenceExpressionList = new List<DbParameterReferenceExpression>(this.Parameters.Count);
        foreach (ObjectParameter parameter in this.Parameters)
        {
          TypeUsage typeUsage = parameter.TypeUsage;
          if (typeUsage == null)
            this.ObjectContext.Perspective.TryGetTypeByName(parameter.MappableType.FullNameWithNesting(), false, out typeUsage);
          referenceExpressionList.Add(typeUsage.Parameter(parameter.Name));
        }
      }
      return CqlQuery.CompileQueryCommandLambda(this._queryText, (Perspective) this.ObjectContext.Perspective, (ParserOptions) null, (IEnumerable<DbParameterReferenceExpression>) referenceExpressionList, (IEnumerable<DbVariableReferenceExpression>) null).Body;
    }
  }
}
