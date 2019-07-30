// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectResult`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common.Internal.Materialization;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// This class represents the result of the <see cref="M:System.Data.Entity.Core.Objects.ObjectQuery`1.Execute(System.Data.Entity.Core.Objects.MergeOption)" /> method.
  /// </summary>
  /// <typeparam name="T">The type of the result.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public class ObjectResult<T> : ObjectResult, IEnumerable<T>, IEnumerable, IDbAsyncEnumerable<T>, IDbAsyncEnumerable
  {
    private Shaper<T> _shaper;
    private DbDataReader _reader;
    private DbCommand _command;
    private readonly EntitySet _singleEntitySet;
    private readonly TypeUsage _resultItemType;
    private readonly bool _readerOwned;
    private readonly bool _shouldReleaseConnection;
    private IBindingList _cachedBindingList;
    private NextResultGenerator _nextResultGenerator;
    private Action<object, EventArgs> _onReaderDispose;

    /// <summary>
    ///     This constructor is intended only for use when creating test doubles that will override members
    ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
    ///     behavior including but not limited to throwing <see cref="T:System.NullReferenceException" />.
    /// </summary>
    protected ObjectResult()
    {
    }

    internal ObjectResult(Shaper<T> shaper, EntitySet singleEntitySet, TypeUsage resultItemType)
      : this(shaper, singleEntitySet, resultItemType, true, true, (DbCommand) null)
    {
    }

    internal ObjectResult(
      Shaper<T> shaper,
      EntitySet singleEntitySet,
      TypeUsage resultItemType,
      bool readerOwned,
      bool shouldReleaseConnection,
      DbCommand command = null)
      : this(shaper, singleEntitySet, resultItemType, readerOwned, shouldReleaseConnection, (NextResultGenerator) null, (Action<object, EventArgs>) null, command)
    {
    }

    internal ObjectResult(
      Shaper<T> shaper,
      EntitySet singleEntitySet,
      TypeUsage resultItemType,
      bool readerOwned,
      bool shouldReleaseConnection,
      NextResultGenerator nextResultGenerator,
      Action<object, EventArgs> onReaderDispose,
      DbCommand command = null)
    {
      this._shaper = shaper;
      this._reader = this._shaper.Reader;
      this._command = command;
      this._singleEntitySet = singleEntitySet;
      this._resultItemType = resultItemType;
      this._readerOwned = readerOwned;
      this._shouldReleaseConnection = shouldReleaseConnection;
      this._nextResultGenerator = nextResultGenerator;
      this._onReaderDispose = onReaderDispose;
    }

    private void EnsureCanEnumerateResults()
    {
      if (this._shaper == null)
        throw new InvalidOperationException(Strings.Materializer_CannotReEnumerateQueryResults);
    }

    /// <summary>Returns an enumerator that iterates through the query results.</summary>
    /// <returns>An enumerator that iterates through the query results.</returns>
    public virtual IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) this.GetDbEnumerator();
    }

    internal virtual IDbEnumerator<T> GetDbEnumerator()
    {
      this.EnsureCanEnumerateResults();
      Shaper<T> shaper = this._shaper;
      this._shaper = (Shaper<T>) null;
      return shaper.GetEnumerator();
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IDbAsyncEnumerator<T> IDbAsyncEnumerable<T>.GetAsyncEnumerator()
    {
      return (IDbAsyncEnumerator<T>) this.GetDbEnumerator();
    }

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" /> and optionally releases the managed resources.</summary>
    /// <param name="disposing">true to release managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      DbDataReader reader = this._reader;
      this._reader = (DbDataReader) null;
      this._nextResultGenerator = (NextResultGenerator) null;
      if (reader != null && this._readerOwned)
      {
        reader.Dispose();
        if (this._onReaderDispose != null)
        {
          this._onReaderDispose((object) this, new EventArgs());
          this._onReaderDispose = (Action<object, EventArgs>) null;
        }
      }
      if (this._shaper != null)
      {
        if (this._shaper.Context != null && this._readerOwned && this._shouldReleaseConnection)
          this._shaper.Context.ReleaseConnection();
        this._shaper = (Shaper<T>) null;
      }
      if (this._command == null)
        return;
      this._command.Dispose();
      this._command = (DbCommand) null;
    }

    internal override IDbAsyncEnumerator GetAsyncEnumeratorInternal()
    {
      return (IDbAsyncEnumerator) this.GetDbEnumerator();
    }

    internal override IEnumerator GetEnumeratorInternal()
    {
      return (IEnumerator) this.GetDbEnumerator();
    }

    internal override IList GetIListSourceListInternal()
    {
      if (this._cachedBindingList == null)
      {
        this.EnsureCanEnumerateResults();
        this._cachedBindingList = ObjectViewFactory.CreateViewForQuery<T>(this._resultItemType, (IEnumerable<T>) this, this._shaper.Context, this._shaper.MergeOption == MergeOption.NoTracking, this._singleEntitySet);
      }
      return (IList) this._cachedBindingList;
    }

    internal override ObjectResult<TElement> GetNextResultInternal<TElement>()
    {
      if (this._nextResultGenerator == null)
        return (ObjectResult<TElement>) null;
      return this._nextResultGenerator.GetNextResult<TElement>(this._reader);
    }

    /// <summary>
    /// Gets the type of the <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Type" /> that is the type of the <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1" />.
    /// </returns>
    public override Type ElementType
    {
      get
      {
        return typeof (T);
      }
    }
  }
}
