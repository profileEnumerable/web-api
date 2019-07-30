// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbRawSqlQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Represents a SQL query for non-entities that is created from a <see cref="T:System.Data.Entity.DbContext" />
  /// and is executed using the connection from that context.
  /// Instances of this class are obtained from the <see cref="P:System.Data.Entity.DbContext.Database" /> instance.
  /// The query is not executed when this object is created; it is executed
  /// each time it is enumerated, for example by using foreach.
  /// SQL queries for entities are created using <see cref="M:System.Data.Entity.DbSet.SqlQuery(System.String,System.Object[])" />.
  /// See <see cref="T:System.Data.Entity.Infrastructure.DbRawSqlQuery`1" /> for a generic version of this class.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public class DbRawSqlQuery : IEnumerable, IListSource, IDbAsyncEnumerable
  {
    private readonly InternalSqlQuery _internalQuery;

    internal DbRawSqlQuery(InternalSqlQuery internalQuery)
    {
      this._internalQuery = internalQuery;
    }

    /// <summary>
    /// Returns a new query that will stream the results instead of buffering.
    /// </summary>
    /// <returns> A new query with AsStreaming applied. </returns>
    [Obsolete("Queries are now streaming by default unless a retrying ExecutionStrategy is used. Calling this method will have no effect.")]
    public virtual DbRawSqlQuery AsStreaming()
    {
      if (this._internalQuery != null)
        return new DbRawSqlQuery(this._internalQuery.AsStreaming());
      return this;
    }

    /// <summary>
    /// Returns an <see cref="T:System.Collections.IEnumerator" /> which when enumerated will execute the SQL query against the database.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the elements.
    /// </returns>
    public virtual IEnumerator GetEnumerator()
    {
      return this.GetInternalQueryWithCheck(nameof (GetEnumerator)).GetEnumerator();
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
    {
      return this.GetInternalQueryWithCheck("IDbAsyncEnumerable.GetAsyncEnumerator").GetAsyncEnumerator();
    }

    /// <summary>
    /// Asynchronously enumerates the query results and performs the specified action on each element.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="action"> The action to perform on each element. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public virtual Task ForEachAsync(Action<object> action)
    {
      Check.NotNull<Action<object>>(action, nameof (action));
      return IDbAsyncEnumerableExtensions.ForEachAsync(this, action, CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously enumerates the query results and performs the specified action on each element.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="action"> The action to perform on each element. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public virtual Task ForEachAsync(Action<object> action, CancellationToken cancellationToken)
    {
      Check.NotNull<Action<object>>(action, nameof (action));
      return IDbAsyncEnumerableExtensions.ForEachAsync(this, action, cancellationToken);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.List`1" /> from the query by enumerating it asynchronously.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.List`1" /> that contains elements from the query.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual Task<List<object>> ToListAsync()
    {
      return this.ToListAsync<object>();
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.List`1" /> from the query by enumerating it asynchronously.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="T:System.Collections.Generic.List`1" /> that contains elements from the query.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual Task<List<object>> ToListAsync(CancellationToken cancellationToken)
    {
      return this.ToListAsync<object>(cancellationToken);
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that contains the SQL string that was set
    /// when the query was created.  The parameters are not included.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      if (this._internalQuery != null)
        return this._internalQuery.ToString();
      return base.ToString();
    }

    internal InternalSqlQuery InternalQuery
    {
      get
      {
        return this._internalQuery;
      }
    }

    private InternalSqlQuery GetInternalQueryWithCheck(string memberName)
    {
      if (this._internalQuery == null)
        throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) memberName, (object) this.GetType().Name, (object) typeof (DbSqlQuery).Name));
      return this._internalQuery;
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
