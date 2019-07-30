// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.LazyAsyncEnumerator`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Internal
{
  internal class LazyAsyncEnumerator<T> : IDbAsyncEnumerator<T>, IDbAsyncEnumerator, IDisposable
  {
    private readonly Func<CancellationToken, Task<ObjectResult<T>>> _getObjectResultAsync;
    private IDbAsyncEnumerator<T> _objectResultAsyncEnumerator;

    public LazyAsyncEnumerator(
      Func<CancellationToken, Task<ObjectResult<T>>> getObjectResultAsync)
    {
      this._getObjectResultAsync = getObjectResultAsync;
    }

    public T Current
    {
      get
      {
        if (this._objectResultAsyncEnumerator != null)
          return this._objectResultAsyncEnumerator.Current;
        return default (T);
      }
    }

    object IDbAsyncEnumerator.Current
    {
      get
      {
        return (object) this.Current;
      }
    }

    public void Dispose()
    {
      if (this._objectResultAsyncEnumerator == null)
        return;
      this._objectResultAsyncEnumerator.Dispose();
    }

    public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (this._objectResultAsyncEnumerator != null)
        return this._objectResultAsyncEnumerator.MoveNextAsync(cancellationToken);
      return this.FirstMoveNextAsync(cancellationToken);
    }

    private async Task<bool> FirstMoveNextAsync(CancellationToken cancellationToken)
    {
      ObjectResult<T> objectResult = await this._getObjectResultAsync(cancellationToken).WithCurrentCulture<ObjectResult<T>>();
      try
      {
        this._objectResultAsyncEnumerator = ((IDbAsyncEnumerable<T>) objectResult).GetAsyncEnumerator();
      }
      catch
      {
        objectResult.Dispose();
        throw;
      }
      return await this._objectResultAsyncEnumerator.MoveNextAsync(cancellationToken).WithCurrentCulture<bool>();
    }
  }
}
