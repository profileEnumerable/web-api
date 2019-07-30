// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Linq.InternalQuery`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Internal.Linq
{
  internal class InternalQuery<TElement> : IInternalQuery<TElement>, IInternalQuery
  {
    private readonly InternalContext _internalContext;
    private System.Data.Entity.Core.Objects.ObjectQuery<TElement> _objectQuery;

    public InternalQuery(InternalContext internalContext)
    {
      this._internalContext = internalContext;
    }

    public InternalQuery(InternalContext internalContext, System.Data.Entity.Core.Objects.ObjectQuery objectQuery)
    {
      this._internalContext = internalContext;
      this._objectQuery = (System.Data.Entity.Core.Objects.ObjectQuery<TElement>) objectQuery;
    }

    public virtual void ResetQuery()
    {
      this._objectQuery = (System.Data.Entity.Core.Objects.ObjectQuery<TElement>) null;
    }

    public virtual InternalContext InternalContext
    {
      get
      {
        return this._internalContext;
      }
    }

    public virtual IInternalQuery<TElement> Include(string path)
    {
      return (IInternalQuery<TElement>) new InternalQuery<TElement>(this._internalContext, (System.Data.Entity.Core.Objects.ObjectQuery) this._objectQuery.Include(path));
    }

    public virtual IInternalQuery<TElement> AsNoTracking()
    {
      return (IInternalQuery<TElement>) new InternalQuery<TElement>(this._internalContext, (System.Data.Entity.Core.Objects.ObjectQuery) DbHelpers.CreateNoTrackingQuery((System.Data.Entity.Core.Objects.ObjectQuery) this._objectQuery));
    }

    public virtual IInternalQuery<TElement> AsStreaming()
    {
      return (IInternalQuery<TElement>) new InternalQuery<TElement>(this._internalContext, (System.Data.Entity.Core.Objects.ObjectQuery) DbHelpers.CreateStreamingQuery((System.Data.Entity.Core.Objects.ObjectQuery) this._objectQuery));
    }

    public virtual IInternalQuery<TElement> WithExecutionStrategy(
      IDbExecutionStrategy executionStrategy)
    {
      return (IInternalQuery<TElement>) new InternalQuery<TElement>(this._internalContext, (System.Data.Entity.Core.Objects.ObjectQuery) DbHelpers.CreateQueryWithExecutionStrategy((System.Data.Entity.Core.Objects.ObjectQuery) this._objectQuery, executionStrategy));
    }

    public virtual System.Data.Entity.Core.Objects.ObjectQuery<TElement> ObjectQuery
    {
      get
      {
        return this._objectQuery;
      }
    }

    System.Data.Entity.Core.Objects.ObjectQuery IInternalQuery.ObjectQuery
    {
      get
      {
        return (System.Data.Entity.Core.Objects.ObjectQuery) this.ObjectQuery;
      }
    }

    protected void InitializeQuery(System.Data.Entity.Core.Objects.ObjectQuery<TElement> objectQuery)
    {
      this._objectQuery = objectQuery;
    }

    public override string ToString()
    {
      return this._objectQuery.ToTraceString();
    }

    public virtual Expression Expression
    {
      get
      {
        return ((IQueryable) this._objectQuery).Expression;
      }
    }

    public virtual ObjectQueryProvider ObjectQueryProvider
    {
      get
      {
        return this._objectQuery.ObjectQueryProvider;
      }
    }

    public Type ElementType
    {
      get
      {
        return typeof (TElement);
      }
    }

    public virtual IEnumerator<TElement> GetEnumerator()
    {
      this.InternalContext.Initialize();
      return ((IEnumerable<TElement>) this._objectQuery).GetEnumerator();
    }

    IEnumerator IInternalQuery.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public virtual IDbAsyncEnumerator<TElement> GetAsyncEnumerator()
    {
      this.InternalContext.Initialize();
      return ((IDbAsyncEnumerable<TElement>) this._objectQuery).GetAsyncEnumerator();
    }

    IDbAsyncEnumerator IInternalQuery.GetAsyncEnumerator()
    {
      return (IDbAsyncEnumerator) this.GetAsyncEnumerator();
    }
  }
}
