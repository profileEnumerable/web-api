// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.ComponentModel;
using System.Data.Entity.Internal.Linq;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Represents a non-generic LINQ to Entities query against a DbContext.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public abstract class DbQuery : IOrderedQueryable, IQueryable, IEnumerable, IListSource, IInternalQueryAdapter, IDbAsyncEnumerable
  {
    private IQueryProvider _provider;

    internal DbQuery()
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    bool IListSource.ContainsListCollection
    {
      get
      {
        return false;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IList IListSource.GetList()
    {
      throw Error.DbQuery_BindingToDbQueryNotSupported();
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetInternalQueryWithCheck("IEnumerable.GetEnumerator").GetEnumerator();
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
    {
      return this.GetInternalQueryWithCheck("IDbAsyncEnumerable.GetAsyncEnumerator").GetAsyncEnumerator();
    }

    /// <summary>The IQueryable element type.</summary>
    public virtual Type ElementType
    {
      get
      {
        return this.GetInternalQueryWithCheck(nameof (ElementType)).ElementType;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    Expression IQueryable.Expression
    {
      get
      {
        return this.GetInternalQueryWithCheck("IQueryable.Expression").Expression;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IQueryProvider IQueryable.Provider
    {
      get
      {
        return this._provider ?? (this._provider = (IQueryProvider) new NonGenericDbQueryProvider(this.GetInternalQueryWithCheck("IQueryable.Provider").InternalContext, this.GetInternalQueryWithCheck("IQueryable.Provider")));
      }
    }

    /// <summary>
    /// Specifies the related objects to include in the query results.
    /// </summary>
    /// <remarks>
    /// Paths are all-inclusive. For example, if an include call indicates Include("Orders.OrderLines"), not only will
    /// OrderLines be included, but also Orders.  When you call the Include method, the query path is only valid on
    /// the returned instance of the DbQuery&lt;T&gt;. Other instances of DbQuery&lt;T&gt; and the object context itself are not affected.
    /// Because the Include method returns the query object, you can call this method multiple times on an DbQuery&lt;T&gt; to
    /// specify multiple paths for the query.
    /// </remarks>
    /// <param name="path"> The dot-separated list of related objects to return in the query results. </param>
    /// <returns>A new DbQuery&lt;T&gt; with the defined query path.</returns>
    public virtual DbQuery Include(string path)
    {
      return this;
    }

    /// <summary>
    /// Returns a new query where the entities returned will not be cached in the <see cref="T:System.Data.Entity.DbContext" />.
    /// </summary>
    /// <returns> A new query with NoTracking applied. </returns>
    public virtual DbQuery AsNoTracking()
    {
      return this;
    }

    /// <summary>
    /// Returns a new query that will stream the results instead of buffering.
    /// </summary>
    /// <returns> A new query with AsStreaming applied. </returns>
    [Obsolete("Queries are now streaming by default unless a retrying ExecutionStrategy is used. Calling this method will have no effect.")]
    public virtual DbQuery AsStreaming()
    {
      return this;
    }

    internal virtual DbQuery WithExecutionStrategy(IDbExecutionStrategy executionStrategy)
    {
      return this;
    }

    /// <summary>
    /// Returns the equivalent generic <see cref="T:System.Data.Entity.Infrastructure.DbQuery`1" /> object.
    /// </summary>
    /// <typeparam name="TElement"> The type of element for which the query was created. </typeparam>
    /// <returns> The generic set object. </returns>
    public DbQuery<TElement> Cast<TElement>()
    {
      if (this.InternalQuery == null)
        throw new NotSupportedException(Strings.TestDoublesCannotBeConverted);
      if (typeof (TElement) != this.InternalQuery.ElementType)
        throw Error.DbEntity_BadTypeForCast((object) typeof (DbQuery).Name, (object) typeof (TElement).Name, (object) this.InternalQuery.ElementType.Name);
      return new DbQuery<TElement>((IInternalQuery<TElement>) this.InternalQuery);
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> representation of the underlying query.
    /// </summary>
    /// <returns> The query string. </returns>
    public override string ToString()
    {
      if (this.InternalQuery != null)
        return this.InternalQuery.ToString();
      return base.ToString();
    }

    internal virtual IInternalQuery InternalQuery
    {
      get
      {
        return (IInternalQuery) null;
      }
    }

    internal virtual IInternalQuery GetInternalQueryWithCheck(string memberName)
    {
      throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) memberName, (object) this.GetType().Name, (object) typeof (DbSet).Name));
    }

    IInternalQuery IInternalQueryAdapter.InternalQuery
    {
      get
      {
        return this.InternalQuery;
      }
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
