﻿// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectQuery`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// ObjectQuery implements strongly-typed queries at the object-layer.
  /// Queries are specified using Entity-SQL strings and may be created by calling
  /// the Entity-SQL-based query builder methods declared by ObjectQuery.
  /// </summary>
  /// <typeparam name="T"> The result type of this ObjectQuery </typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public class ObjectQuery<T> : ObjectQuery, IOrderedQueryable<T>, IQueryable<T>, IOrderedQueryable, IQueryable, IEnumerable<T>, IEnumerable, IDbAsyncEnumerable<T>, IDbAsyncEnumerable
  {
    internal static readonly MethodInfo MergeAsMethod = typeof (ObjectQuery<T>).GetOnlyDeclaredMethod("MergeAs");
    internal static readonly MethodInfo IncludeSpanMethod = typeof (ObjectQuery<T>).GetOnlyDeclaredMethod("IncludeSpan");
    private string _name = "it";
    private const string DefaultName = "it";

    private static bool IsLinqQuery(ObjectQuery query)
    {
      return query.QueryState is ELinqQueryState;
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance using the specified Entity SQL command as the initial query.
    /// </summary>
    /// <param name="commandText">The Entity SQL query.</param>
    /// <param name="context">
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> on which to execute the query.
    /// </param>
    public ObjectQuery(string commandText, ObjectContext context)
      : this((ObjectQueryState) new EntitySqlQueryState(typeof (T), commandText, false, context, (ObjectParameterCollection) null, (Span) null))
    {
      context.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (T), Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance using the specified Entity SQL command as the initial query and the specified merge option.
    /// </summary>
    /// <param name="commandText">The Entity SQL query.</param>
    /// <param name="context">
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> on which to execute the query.
    /// </param>
    /// <param name="mergeOption">
    /// Specifies how the entities that are retrieved through this query should be merged with the entities that have been returned from previous queries against the same
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </param>
    public ObjectQuery(string commandText, ObjectContext context, MergeOption mergeOption)
      : this((ObjectQueryState) new EntitySqlQueryState(typeof (T), commandText, false, context, (ObjectParameterCollection) null, (Span) null))
    {
      EntityUtil.CheckArgumentMergeOption(mergeOption);
      this.QueryState.UserSpecifiedMergeOption = new MergeOption?(mergeOption);
      context.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (T), Assembly.GetCallingAssembly());
    }

    internal ObjectQuery(EntitySetBase entitySet, ObjectContext context, MergeOption mergeOption)
      : this((ObjectQueryState) new EntitySqlQueryState(typeof (T), ObjectQuery<T>.BuildScanEntitySetEsql(entitySet), (DbExpression) entitySet.Scan(), false, context, (ObjectParameterCollection) null, (Span) null, (ObjectQueryExecutionPlanFactory) null))
    {
      EntityUtil.CheckArgumentMergeOption(mergeOption);
      this.QueryState.UserSpecifiedMergeOption = new MergeOption?(mergeOption);
      context.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (T), Assembly.GetCallingAssembly());
    }

    private static string BuildScanEntitySetEsql(EntitySetBase entitySet)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) EntityUtil.QuoteIdentifier(entitySet.EntityContainer.Name), (object) EntityUtil.QuoteIdentifier(entitySet.Name));
    }

    internal ObjectQuery(ObjectQueryState queryState)
      : base(queryState)
    {
    }

    internal ObjectQuery()
    {
    }

    /// <summary>Gets or sets the name of this object query.</summary>
    /// <returns>
    /// A string value that is the name of this <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">The value specified on set is not valid.</exception>
    public string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        Check.NotNull<string>(value, nameof (value));
        if (!ObjectParameter.ValidateParameterName(value))
          throw new ArgumentException(Strings.ObjectQuery_InvalidQueryName((object) value), nameof (value));
        this._name = value;
      }
    }

    /// <summary>Executes the object query with the specified merge option.</summary>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> to use when executing the query.
    /// The default is <see cref="F:System.Data.Entity.Core.Objects.MergeOption.AppendOnly" />.
    /// </param>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" /> that contains a collection of entity objects returned by the query.
    /// </returns>
    public ObjectResult<T> Execute(MergeOption mergeOption)
    {
      EntityUtil.CheckArgumentMergeOption(mergeOption);
      return this.GetResults(new MergeOption?(mergeOption));
    }

    /// <summary>
    /// Asynchronously executes the object query with the specified merge option.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> to use when executing the query.
    /// The default is <see cref="F:System.Data.Entity.Core.Objects.MergeOption.AppendOnly" />.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" />
    /// that contains a collection of entity objects returned by the query.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public Task<ObjectResult<T>> ExecuteAsync(MergeOption mergeOption)
    {
      return this.ExecuteAsync(mergeOption, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously executes the object query with the specified merge option.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> to use when executing the query.
    /// The default is <see cref="F:System.Data.Entity.Core.Objects.MergeOption.AppendOnly" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" />
    /// that contains a collection of entity objects returned by the query.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public Task<ObjectResult<T>> ExecuteAsync(
      MergeOption mergeOption,
      CancellationToken cancellationToken)
    {
      EntityUtil.CheckArgumentMergeOption(mergeOption);
      return this.GetResultsAsync(new MergeOption?(mergeOption), cancellationToken);
    }

    /// <summary>Specifies the related objects to include in the query results.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> with the defined query path.
    /// </returns>
    /// <param name="path">Dot-separated list of related objects to return in the query results.</param>
    /// <exception cref="T:System.ArgumentNullException"> path  is null.</exception>
    /// <exception cref="T:System.ArgumentException"> path  is empty.</exception>
    public ObjectQuery<T> Include(string path)
    {
      Check.NotEmpty(path, nameof (path));
      return new ObjectQuery<T>(this.QueryState.Include<T>(this, path));
    }

    /// <summary>Limits the query to unique results.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance that is equivalent to the original instance with SELECT DISTINCT applied.
    /// </returns>
    public ObjectQuery<T> Distinct()
    {
      if (ObjectQuery<T>.IsLinqQuery((ObjectQuery) this))
        return (ObjectQuery<T>) this.Distinct<T>();
      return new ObjectQuery<T>(EntitySqlQueryBuilder.Distinct(this.QueryState));
    }

    /// <summary>
    /// This query-builder method creates a new query whose results are all of
    /// the results of this query, except those that are also part of the other
    /// query specified.
    /// </summary>
    /// <param name="query"> A query representing the results to exclude. </param>
    /// <returns> a new ObjectQuery instance. </returns>
    /// <exception cref="T:System.ArgumentNullException">If the query parameter is null.</exception>
    public ObjectQuery<T> Except(ObjectQuery<T> query)
    {
      Check.NotNull<ObjectQuery<T>>(query, nameof (query));
      if (ObjectQuery<T>.IsLinqQuery((ObjectQuery) this) || ObjectQuery<T>.IsLinqQuery((ObjectQuery) query))
        return (ObjectQuery<T>) this.Except<T>((IEnumerable<T>) query);
      return new ObjectQuery<T>(EntitySqlQueryBuilder.Except(this.QueryState, query.QueryState));
    }

    /// <summary>Groups the query results by the specified criteria.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance of type
    /// <see cref="T:System.Data.Common.DbDataRecord" />
    /// that is equivalent to the original instance with GROUP BY applied.
    /// </returns>
    /// <param name="keys">The key columns by which to group the results.</param>
    /// <param name="projection">The list of selected properties that defines the projection. </param>
    /// <param name="parameters">Zero or more parameters that are used in this method.</param>
    /// <exception cref="T:System.ArgumentNullException">The  query  parameter is null or an empty string
    /// or the  projection  parameter is null or an empty string.</exception>
    public ObjectQuery<DbDataRecord> GroupBy(
      string keys,
      string projection,
      params ObjectParameter[] parameters)
    {
      Check.NotEmpty(keys, nameof (keys));
      Check.NotEmpty(projection, nameof (projection));
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      return new ObjectQuery<DbDataRecord>(EntitySqlQueryBuilder.GroupBy(this.QueryState, this.Name, keys, projection, parameters));
    }

    /// <summary>
    /// This query-builder method creates a new query whose results are those that
    /// are both in this query and the other query specified.
    /// </summary>
    /// <param name="query"> A query representing the results to intersect with. </param>
    /// <returns> a new ObjectQuery instance. </returns>
    /// <exception cref="T:System.ArgumentNullException">If the query parameter is null.</exception>
    public ObjectQuery<T> Intersect(ObjectQuery<T> query)
    {
      Check.NotNull<ObjectQuery<T>>(query, nameof (query));
      if (ObjectQuery<T>.IsLinqQuery((ObjectQuery) this) || ObjectQuery<T>.IsLinqQuery((ObjectQuery) query))
        return (ObjectQuery<T>) this.Intersect<T>((IEnumerable<T>) query);
      return new ObjectQuery<T>(EntitySqlQueryBuilder.Intersect(this.QueryState, query.QueryState));
    }

    /// <summary>Limits the query to only results of a specific type.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance that is equivalent to the original instance with OFTYPE applied.
    /// </returns>
    /// <typeparam name="TResultType">
    /// The type of the <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" /> returned when the query is executed with the applied filter.
    /// </typeparam>
    /// <exception cref="T:System.Data.Entity.Core.EntitySqlException">The type specified is not valid.</exception>
    public ObjectQuery<TResultType> OfType<TResultType>()
    {
      if (ObjectQuery<T>.IsLinqQuery((ObjectQuery) this))
        return (ObjectQuery<TResultType>) Queryable.OfType<TResultType>(this);
      this.QueryState.ObjectContext.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResultType), Assembly.GetCallingAssembly());
      Type type1 = typeof (TResultType);
      EdmType type2;
      if (!this.QueryState.ObjectContext.MetadataWorkspace.GetItemCollection(DataSpace.OSpace).TryGetType(type1.Name, type1.NestingNamespace() ?? string.Empty, out type2) || !Helper.IsEntityType(type2) && !Helper.IsComplexType(type2))
        throw new EntitySqlException(Strings.ObjectQuery_QueryBuilder_InvalidResultType((object) typeof (TResultType).FullName));
      return new ObjectQuery<TResultType>(EntitySqlQueryBuilder.OfType(this.QueryState, type2, type1));
    }

    /// <summary>Orders the query results by the specified criteria.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance that is equivalent to the original instance with ORDER BY applied.
    /// </returns>
    /// <param name="keys">The key columns by which to order the results.</param>
    /// <param name="parameters">Zero or more parameters that are used in this method.</param>
    /// <exception cref="T:System.ArgumentNullException">The  keys  or  parameters  parameter is null.</exception>
    /// <exception cref="T:System.ArgumentException">The  key  is an empty string.</exception>
    public ObjectQuery<T> OrderBy(string keys, params ObjectParameter[] parameters)
    {
      Check.NotEmpty(keys, nameof (keys));
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      return new ObjectQuery<T>(EntitySqlQueryBuilder.OrderBy(this.QueryState, this.Name, keys, parameters));
    }

    /// <summary>Limits the query results to only the properties that are defined in the specified projection.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance of type
    /// <see cref="T:System.Data.Common.DbDataRecord" />
    /// that is equivalent to the original instance with SELECT applied.
    /// </returns>
    /// <param name="projection">The list of selected properties that defines the projection.</param>
    /// <param name="parameters">Zero or more parameters that are used in this method.</param>
    /// <exception cref="T:System.ArgumentNullException"> projection  is null or parameters is null.</exception>
    /// <exception cref="T:System.ArgumentException">The  projection  is an empty string.</exception>
    public ObjectQuery<DbDataRecord> Select(
      string projection,
      params ObjectParameter[] parameters)
    {
      Check.NotEmpty(projection, nameof (projection));
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      return new ObjectQuery<DbDataRecord>(EntitySqlQueryBuilder.Select(this.QueryState, this.Name, projection, parameters));
    }

    /// <summary>Limits the query results to only the property specified in the projection.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance of a type compatible with the specific projection. The returned
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" />
    /// is equivalent to the original instance with SELECT VALUE applied.
    /// </returns>
    /// <param name="projection">The projection list.</param>
    /// <param name="parameters">An optional set of query parameters that should be in scope when parsing.</param>
    /// <typeparam name="TResultType">
    /// The type of the <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.ObjectQuery`1.SelectValue``1(System.String,System.Data.Entity.Core.Objects.ObjectParameter[])" />
    /// method.
    /// </typeparam>
    /// <exception cref="T:System.ArgumentNullException"> projection  is null or parameters  is null.</exception>
    /// <exception cref="T:System.ArgumentException">The  projection  is an empty string.</exception>
    public ObjectQuery<TResultType> SelectValue<TResultType>(
      string projection,
      params ObjectParameter[] parameters)
    {
      Check.NotEmpty(projection, nameof (projection));
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      this.QueryState.ObjectContext.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResultType), Assembly.GetCallingAssembly());
      return new ObjectQuery<TResultType>(EntitySqlQueryBuilder.SelectValue(this.QueryState, this.Name, projection, parameters, typeof (TResultType)));
    }

    /// <summary>Orders the query results by the specified criteria and skips a specified number of results.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance that is equivalent to the original instance with both ORDER BY and SKIP applied.
    /// </returns>
    /// <param name="keys">The key columns by which to order the results.</param>
    /// <param name="count">The number of results to skip. This must be either a constant or a parameter reference.</param>
    /// <param name="parameters">An optional set of query parameters that should be in scope when parsing.</param>
    /// <exception cref="T:System.ArgumentNullException">Any argument is null.</exception>
    /// <exception cref="T:System.ArgumentException"> keys  is an empty string or count  is an empty string.</exception>
    public ObjectQuery<T> Skip(
      string keys,
      string count,
      params ObjectParameter[] parameters)
    {
      Check.NotEmpty(keys, nameof (keys));
      Check.NotEmpty(count, nameof (count));
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      return new ObjectQuery<T>(EntitySqlQueryBuilder.Skip(this.QueryState, this.Name, keys, count, parameters));
    }

    /// <summary>Limits the query results to a specified number of items.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance that is equivalent to the original instance with TOP applied.
    /// </returns>
    /// <param name="count">The number of items in the results as a string. </param>
    /// <param name="parameters">An optional set of query parameters that should be in scope when parsing.</param>
    /// <exception cref="T:System.ArgumentNullException"> count  is null.</exception>
    /// <exception cref="T:System.ArgumentException"> count  is an empty string.</exception>
    public ObjectQuery<T> Top(string count, params ObjectParameter[] parameters)
    {
      Check.NotEmpty(count, nameof (count));
      return new ObjectQuery<T>(EntitySqlQueryBuilder.Top(this.QueryState, this.Name, count, parameters));
    }

    /// <summary>
    /// This query-builder method creates a new query whose results are all of
    /// the results of this query, plus all of the results of the other query,
    /// without duplicates (i.e., results are unique).
    /// </summary>
    /// <param name="query"> A query representing the results to add. </param>
    /// <returns> a new ObjectQuery instance. </returns>
    /// <exception cref="T:System.ArgumentNullException">If the query parameter is null.</exception>
    public ObjectQuery<T> Union(ObjectQuery<T> query)
    {
      Check.NotNull<ObjectQuery<T>>(query, nameof (query));
      if (ObjectQuery<T>.IsLinqQuery((ObjectQuery) this) || ObjectQuery<T>.IsLinqQuery((ObjectQuery) query))
        return (ObjectQuery<T>) this.Union<T>((IEnumerable<T>) query);
      return new ObjectQuery<T>(EntitySqlQueryBuilder.Union(this.QueryState, query.QueryState));
    }

    /// <summary>
    /// This query-builder method creates a new query whose results are all of
    /// the results of this query, plus all of the results of the other query,
    /// including any duplicates (i.e., results are not necessarily unique).
    /// </summary>
    /// <param name="query"> A query representing the results to add. </param>
    /// <returns> a new ObjectQuery instance. </returns>
    /// <exception cref="T:System.ArgumentNullException">If the query parameter is null.</exception>
    public ObjectQuery<T> UnionAll(ObjectQuery<T> query)
    {
      Check.NotNull<ObjectQuery<T>>(query, nameof (query));
      return new ObjectQuery<T>(EntitySqlQueryBuilder.UnionAll(this.QueryState, query.QueryState));
    }

    /// <summary>Limits the query to results that match specified filtering criteria.</summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" /> instance that is equivalent to the original instance with WHERE applied.
    /// </returns>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="parameters">Zero or more parameters that are used in this method.</param>
    /// <exception cref="T:System.ArgumentNullException"> predicate  is null or parameters  is null.</exception>
    /// <exception cref="T:System.ArgumentException">The  predicate  is an empty string.</exception>
    public ObjectQuery<T> Where(string predicate, params ObjectParameter[] parameters)
    {
      Check.NotEmpty(predicate, nameof (predicate));
      Check.NotNull<ObjectParameter[]>(parameters, nameof (parameters));
      return new ObjectQuery<T>(EntitySqlQueryBuilder.Where(this.QueryState, this.Name, predicate, parameters));
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      this.QueryState.ObjectContext.AsyncMonitor.EnsureNotEntered();
      return (IEnumerator<T>) new LazyEnumerator<T>((Func<ObjectResult<T>>) (() => this.GetResults(new MergeOption?())));
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IDbAsyncEnumerator<T> IDbAsyncEnumerable<T>.GetAsyncEnumerator()
    {
      this.QueryState.ObjectContext.AsyncMonitor.EnsureNotEntered();
      return (IDbAsyncEnumerator<T>) new LazyAsyncEnumerator<T>((Func<CancellationToken, Task<ObjectResult<T>>>) (cancellationToken => this.GetResultsAsync(new MergeOption?(), cancellationToken)));
    }

    internal override IEnumerator GetEnumeratorInternal()
    {
      return (IEnumerator) ((IEnumerable<T>) this).GetEnumerator();
    }

    internal override IDbAsyncEnumerator GetAsyncEnumeratorInternal()
    {
      return (IDbAsyncEnumerator) ((IDbAsyncEnumerable<T>) this).GetAsyncEnumerator();
    }

    internal override IList GetIListSourceListInternal()
    {
      return ((IListSource) this.GetResults(new MergeOption?())).GetList();
    }

    internal override ObjectResult ExecuteInternal(MergeOption mergeOption)
    {
      return (ObjectResult) this.GetResults(new MergeOption?(mergeOption));
    }

    internal override async Task<ObjectResult> ExecuteInternalAsync(
      MergeOption mergeOption,
      CancellationToken cancellationToken)
    {
      return (ObjectResult) await this.GetResultsAsync(new MergeOption?(mergeOption), cancellationToken).WithCurrentCulture<ObjectResult<T>>();
    }

    internal override Expression GetExpression()
    {
      Expression expression;
      if (!this.QueryState.TryGetExpression(out expression))
        expression = (Expression) Expression.Constant((object) this);
      if (this.QueryState.UserSpecifiedMergeOption.HasValue)
        expression = (Expression) Expression.Call(TypeSystem.EnsureType(expression, typeof (ObjectQuery<T>)), ObjectQuery<T>.MergeAsMethod, (Expression) Expression.Constant((object) this.QueryState.UserSpecifiedMergeOption.Value));
      if (this.QueryState.Span != null)
        expression = (Expression) Expression.Call(TypeSystem.EnsureType(expression, typeof (ObjectQuery<T>)), ObjectQuery<T>.IncludeSpanMethod, (Expression) Expression.Constant((object) this.QueryState.Span));
      return expression;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "mergeOption")]
    internal ObjectQuery<T> MergeAs(MergeOption mergeOption)
    {
      throw new InvalidOperationException(Strings.ELinq_MethodNotDirectlyCallable);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "span")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    internal ObjectQuery<T> IncludeSpan(Span span)
    {
      throw new InvalidOperationException(Strings.ELinq_MethodNotDirectlyCallable);
    }

    private ObjectResult<T> GetResults(MergeOption? forMergeOption)
    {
      this.QueryState.ObjectContext.AsyncMonitor.EnsureNotEntered();
      IDbExecutionStrategy executionStrategy = this.ExecutionStrategy ?? DbProviderServices.GetExecutionStrategy(this.QueryState.ObjectContext.Connection, this.QueryState.ObjectContext.MetadataWorkspace);
      if (executionStrategy.RetriesOnFailure && this.QueryState.EffectiveStreamingBehavior)
        throw new InvalidOperationException(Strings.ExecutionStrategy_StreamingNotSupported((object) executionStrategy.GetType().Name));
      return executionStrategy.Execute<ObjectResult<T>>((Func<ObjectResult<T>>) (() => this.QueryState.ObjectContext.ExecuteInTransaction<ObjectResult<T>>((Func<ObjectResult<T>>) (() => this.QueryState.GetExecutionPlan(forMergeOption).Execute<T>(this.QueryState.ObjectContext, this.QueryState.Parameters)), executionStrategy, false, !this.QueryState.EffectiveStreamingBehavior)));
    }

    private Task<ObjectResult<T>> GetResultsAsync(
      MergeOption? forMergeOption,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      this.QueryState.ObjectContext.AsyncMonitor.EnsureNotEntered();
      IDbExecutionStrategy executionStrategy = this.ExecutionStrategy ?? DbProviderServices.GetExecutionStrategy(this.QueryState.ObjectContext.Connection, this.QueryState.ObjectContext.MetadataWorkspace);
      if (executionStrategy.RetriesOnFailure && this.QueryState.EffectiveStreamingBehavior)
        throw new InvalidOperationException(Strings.ExecutionStrategy_StreamingNotSupported((object) executionStrategy.GetType().Name));
      return this.GetResultsAsync(forMergeOption, executionStrategy, cancellationToken);
    }

    private async Task<ObjectResult<T>> GetResultsAsync(
      MergeOption? forMergeOption,
      IDbExecutionStrategy executionStrategy,
      CancellationToken cancellationToken)
    {
      MergeOption mergeOption = forMergeOption.HasValue ? forMergeOption.Value : this.QueryState.EffectiveMergeOption;
      if (mergeOption != MergeOption.NoTracking)
        this.QueryState.ObjectContext.AsyncMonitor.Enter();
      ObjectResult<T> objectResult;
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        objectResult = await executionStrategy.ExecuteAsync<ObjectResult<T>>((Func<Task<ObjectResult<T>>>) (() => this.\u003C\u003E4__this.QueryState.ObjectContext.ExecuteInTransactionAsync<ObjectResult<T>>((Func<Task<ObjectResult<T>>>) (() => this.\u003C\u003E4__this.QueryState.GetExecutionPlan(forMergeOption).ExecuteAsync<T>(this.\u003C\u003E4__this.QueryState.ObjectContext, this.\u003C\u003E4__this.QueryState.Parameters, cancellationToken)), executionStrategy, false, !this.\u003C\u003E4__this.QueryState.EffectiveStreamingBehavior, cancellationToken)), cancellationToken).WithCurrentCulture<ObjectResult<T>>();
      }
      finally
      {
        if (mergeOption != MergeOption.NoTracking)
          this.QueryState.ObjectContext.AsyncMonitor.Exit();
      }
      return objectResult;
    }
  }
}
