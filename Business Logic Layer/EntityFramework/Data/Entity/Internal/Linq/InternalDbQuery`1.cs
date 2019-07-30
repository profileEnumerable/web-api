// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Linq.InternalDbQuery`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Internal.Linq
{
  internal class InternalDbQuery<TElement> : DbQuery, IOrderedQueryable<TElement>, IQueryable<TElement>, IEnumerable<TElement>, IOrderedQueryable, IQueryable, IEnumerable, IDbAsyncEnumerable<TElement>, IDbAsyncEnumerable
  {
    private readonly IInternalQuery<TElement> _internalQuery;

    public InternalDbQuery(IInternalQuery<TElement> internalQuery)
    {
      this._internalQuery = internalQuery;
    }

    internal override IInternalQuery InternalQuery
    {
      get
      {
        return (IInternalQuery) this._internalQuery;
      }
    }

    public override DbQuery Include(string path)
    {
      Check.NotEmpty(path, nameof (path));
      return (DbQuery) new InternalDbQuery<TElement>(this._internalQuery.Include(path));
    }

    public override DbQuery AsNoTracking()
    {
      return (DbQuery) new InternalDbQuery<TElement>(this._internalQuery.AsNoTracking());
    }

    [Obsolete("Queries are now streaming by default unless a retrying ExecutionStrategy is used. Calling this method will have no effect.")]
    public override DbQuery AsStreaming()
    {
      return (DbQuery) new InternalDbQuery<TElement>(this._internalQuery.AsStreaming());
    }

    internal override DbQuery WithExecutionStrategy(IDbExecutionStrategy executionStrategy)
    {
      return (DbQuery) new InternalDbQuery<TElement>(this._internalQuery.WithExecutionStrategy(executionStrategy));
    }

    internal override IInternalQuery GetInternalQueryWithCheck(string memberName)
    {
      return (IInternalQuery) this._internalQuery;
    }

    public IEnumerator<TElement> GetEnumerator()
    {
      return this._internalQuery.GetEnumerator();
    }

    public IDbAsyncEnumerator<TElement> GetAsyncEnumerator()
    {
      return this._internalQuery.GetAsyncEnumerator();
    }
  }
}
