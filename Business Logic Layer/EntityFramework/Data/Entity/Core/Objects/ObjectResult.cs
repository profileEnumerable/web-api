// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectResult
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// This class implements IEnumerable and IDisposable. Instance of this class
  /// is returned from ObjectQuery.Execute method.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public abstract class ObjectResult : IEnumerable, IDisposable, IListSource, IDbAsyncEnumerable
  {
    /// <summary>
    ///     This constructor is intended only for use when creating test doubles that will override members
    ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
    ///     behavior including but not limited to throwing <see cref="T:System.NullReferenceException" />.
    /// </summary>
    protected internal ObjectResult()
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
    {
      return this.GetAsyncEnumeratorInternal();
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumeratorInternal();
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
      return this.GetIListSourceListInternal();
    }

    /// <summary>
    /// When overridden in a derived class, gets the type of the generic
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" />
    /// .
    /// </summary>
    /// <returns>
    /// The type of the generic <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" />.
    /// </returns>
    public abstract Type ElementType { get; }

    /// <summary>Performs tasks associated with freeing, releasing, or resetting resources.</summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>Releases the resources used by the object result.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected abstract void Dispose(bool disposing);

    /// <summary>Gets the next result set of a stored procedure.</summary>
    /// <returns>An ObjectResult that enumerates the values of the next result set. Null, if there are no more, or if the ObjectResult is not the result of a stored procedure call.</returns>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public virtual ObjectResult<TElement> GetNextResult<TElement>()
    {
      return this.GetNextResultInternal<TElement>();
    }

    internal abstract IDbAsyncEnumerator GetAsyncEnumeratorInternal();

    internal abstract IEnumerator GetEnumeratorInternal();

    internal abstract IList GetIListSourceListInternal();

    internal abstract ObjectResult<TElement> GetNextResultInternal<TElement>();
  }
}
