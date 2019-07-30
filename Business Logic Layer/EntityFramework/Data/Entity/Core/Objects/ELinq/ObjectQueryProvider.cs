// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ELinq.ObjectQueryProvider
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Objects.ELinq
{
  internal class ObjectQueryProvider : IDbAsyncQueryProvider, IQueryProvider
  {
    private readonly ObjectContext _context;
    private readonly ObjectQuery _query;

    internal ObjectQueryProvider(ObjectContext context)
    {
      this._context = context;
    }

    internal ObjectQueryProvider(ObjectQuery query)
      : this(query.Context)
    {
      this._query = query;
    }

    internal virtual ObjectQuery<TElement> CreateQuery<TElement>(Expression expression)
    {
      return this.GetObjectQueryState(this._query, expression, typeof (TElement)).CreateObjectQuery<TElement>();
    }

    internal virtual ObjectQuery CreateQuery(Expression expression, Type ofType)
    {
      return this.GetObjectQueryState(this._query, expression, ofType).CreateQuery();
    }

    private ObjectQueryState GetObjectQueryState(
      ObjectQuery query,
      Expression expression,
      Type ofType)
    {
      if (query != null)
        return (ObjectQueryState) new ELinqQueryState(ofType, this._query, expression, (ObjectQueryExecutionPlanFactory) null);
      return (ObjectQueryState) new ELinqQueryState(ofType, this._context, expression, (ObjectQueryExecutionPlanFactory) null);
    }

    IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(
      Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      if (!typeof (IQueryable<TElement>).IsAssignableFrom(expression.Type))
        throw new ArgumentException(Strings.ELinq_ExpressionMustBeIQueryable, nameof (expression));
      return (IQueryable<TElement>) this.CreateQuery<TElement>(expression);
    }

    TResult IQueryProvider.Execute<TResult>(Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      return ObjectQueryProvider.ExecuteSingle<TResult>((IEnumerable<TResult>) this.CreateQuery<TResult>(expression), expression);
    }

    IQueryable IQueryProvider.CreateQuery(Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      if (!typeof (IQueryable).IsAssignableFrom(expression.Type))
        throw new ArgumentException(Strings.ELinq_ExpressionMustBeIQueryable, nameof (expression));
      Type elementType = TypeSystem.GetElementType(expression.Type);
      return (IQueryable) this.CreateQuery(expression, elementType);
    }

    object IQueryProvider.Execute(Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      return ObjectQueryProvider.ExecuteSingle<object>(Enumerable.Cast<object>(this.CreateQuery(expression, expression.Type)), expression);
    }

    Task<TResult> IDbAsyncQueryProvider.ExecuteAsync<TResult>(
      Expression expression,
      CancellationToken cancellationToken)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      cancellationToken.ThrowIfCancellationRequested();
      return ObjectQueryProvider.ExecuteSingleAsync<TResult>((IDbAsyncEnumerable<TResult>) this.CreateQuery<TResult>(expression), expression, cancellationToken);
    }

    Task<object> IDbAsyncQueryProvider.ExecuteAsync(
      Expression expression,
      CancellationToken cancellationToken)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      cancellationToken.ThrowIfCancellationRequested();
      return ObjectQueryProvider.ExecuteSingleAsync<object>(IDbAsyncEnumerableExtensions.Cast<object>(this.CreateQuery(expression, expression.Type)), expression, cancellationToken);
    }

    internal static TResult ExecuteSingle<TResult>(IEnumerable<TResult> query, Expression queryRoot)
    {
      return ObjectQueryProvider.GetElementFunction<TResult>(queryRoot)(query);
    }

    private static Func<IEnumerable<TResult>, TResult> GetElementFunction<TResult>(
      Expression queryRoot)
    {
      SequenceMethod sequenceMethod;
      if (ReflectionUtil.TryIdentifySequenceMethod(queryRoot, true, out sequenceMethod))
      {
        switch (sequenceMethod)
        {
          case SequenceMethod.First:
          case SequenceMethod.FirstPredicate:
            return (Func<IEnumerable<TResult>, TResult>) (sequence => sequence.First<TResult>());
          case SequenceMethod.FirstOrDefault:
          case SequenceMethod.FirstOrDefaultPredicate:
            return (Func<IEnumerable<TResult>, TResult>) (sequence => sequence.FirstOrDefault<TResult>());
          case SequenceMethod.SingleOrDefault:
          case SequenceMethod.SingleOrDefaultPredicate:
            return (Func<IEnumerable<TResult>, TResult>) (sequence => sequence.SingleOrDefault<TResult>());
        }
      }
      return (Func<IEnumerable<TResult>, TResult>) (sequence => sequence.Single<TResult>());
    }

    internal static Task<TResult> ExecuteSingleAsync<TResult>(
      IDbAsyncEnumerable<TResult> query,
      Expression queryRoot,
      CancellationToken cancellationToken)
    {
      return ObjectQueryProvider.GetAsyncElementFunction<TResult>(queryRoot)(query, cancellationToken);
    }

    private static Func<IDbAsyncEnumerable<TResult>, CancellationToken, Task<TResult>> GetAsyncElementFunction<TResult>(
      Expression queryRoot)
    {
      SequenceMethod sequenceMethod;
      if (ReflectionUtil.TryIdentifySequenceMethod(queryRoot, true, out sequenceMethod))
      {
        switch (sequenceMethod)
        {
          case SequenceMethod.First:
          case SequenceMethod.FirstPredicate:
            return (Func<IDbAsyncEnumerable<TResult>, CancellationToken, Task<TResult>>) ((sequence, cancellationToken) => sequence.FirstAsync<TResult>(cancellationToken));
          case SequenceMethod.FirstOrDefault:
          case SequenceMethod.FirstOrDefaultPredicate:
            return (Func<IDbAsyncEnumerable<TResult>, CancellationToken, Task<TResult>>) ((sequence, cancellationToken) => sequence.FirstOrDefaultAsync<TResult>(cancellationToken));
          case SequenceMethod.SingleOrDefault:
          case SequenceMethod.SingleOrDefaultPredicate:
            return (Func<IDbAsyncEnumerable<TResult>, CancellationToken, Task<TResult>>) ((sequence, cancellationToken) => sequence.SingleOrDefaultAsync<TResult>(cancellationToken));
        }
      }
      return (Func<IDbAsyncEnumerable<TResult>, CancellationToken, Task<TResult>>) ((sequence, cancellationToken) => sequence.SingleAsync<TResult>(cancellationToken));
    }
  }
}
