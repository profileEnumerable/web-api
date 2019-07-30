// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Linq.DbQueryProvider
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Internal.Linq
{
  internal class DbQueryProvider : IDbAsyncQueryProvider, IQueryProvider
  {
    private readonly InternalContext _internalContext;
    private readonly IInternalQuery _internalQuery;

    public DbQueryProvider(InternalContext internalContext, IInternalQuery internalQuery)
    {
      this._internalContext = internalContext;
      this._internalQuery = internalQuery;
    }

    public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      ObjectQuery objectQuery = this.CreateObjectQuery(expression);
      if (typeof (TElement) != ((IQueryable) objectQuery).ElementType)
        return (IQueryable<TElement>) this.CreateQuery(objectQuery);
      return (IQueryable<TElement>) new DbQuery<TElement>((IInternalQuery<TElement>) new InternalQuery<TElement>(this._internalContext, objectQuery));
    }

    public virtual IQueryable CreateQuery(Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      return this.CreateQuery(this.CreateObjectQuery(expression));
    }

    public virtual TResult Execute<TResult>(Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      this._internalContext.Initialize();
      return ((IQueryProvider) this._internalQuery.ObjectQueryProvider).Execute<TResult>(expression);
    }

    public virtual object Execute(Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      this._internalContext.Initialize();
      return ((IQueryProvider) this._internalQuery.ObjectQueryProvider).Execute(expression);
    }

    Task<TResult> IDbAsyncQueryProvider.ExecuteAsync<TResult>(
      Expression expression,
      CancellationToken cancellationToken)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      cancellationToken.ThrowIfCancellationRequested();
      this._internalContext.Initialize();
      return ((IDbAsyncQueryProvider) this._internalQuery.ObjectQueryProvider).ExecuteAsync<TResult>(expression, cancellationToken);
    }

    Task<object> IDbAsyncQueryProvider.ExecuteAsync(
      Expression expression,
      CancellationToken cancellationToken)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      cancellationToken.ThrowIfCancellationRequested();
      this._internalContext.Initialize();
      return ((IDbAsyncQueryProvider) this._internalQuery.ObjectQueryProvider).ExecuteAsync(expression, cancellationToken);
    }

    private IQueryable CreateQuery(ObjectQuery objectQuery)
    {
      IInternalQuery internalQuery = this.CreateInternalQuery(objectQuery);
      return (IQueryable) ((IEnumerable<ConstructorInfo>) typeof (DbQuery<>).MakeGenericType(internalQuery.ElementType).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)).Single<ConstructorInfo>().Invoke(new object[1]
      {
        (object) internalQuery
      });
    }

    protected ObjectQuery CreateObjectQuery(Expression expression)
    {
      expression = new DbQueryVisitor().Visit(expression);
      return (ObjectQuery) ((IQueryProvider) this._internalQuery.ObjectQueryProvider).CreateQuery(expression);
    }

    protected IInternalQuery CreateInternalQuery(ObjectQuery objectQuery)
    {
      return (IInternalQuery) typeof (InternalQuery<>).MakeGenericType(((IQueryable) objectQuery).ElementType).GetDeclaredConstructor(typeof (InternalContext), typeof (ObjectQuery)).Invoke(new object[2]
      {
        (object) this._internalContext,
        (object) objectQuery
      });
    }

    public InternalContext InternalContext
    {
      get
      {
        return this._internalContext;
      }
    }
  }
}
