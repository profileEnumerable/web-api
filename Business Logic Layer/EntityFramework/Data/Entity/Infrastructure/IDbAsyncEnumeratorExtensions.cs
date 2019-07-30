// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IDbAsyncEnumeratorExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure
{
  internal static class IDbAsyncEnumeratorExtensions
  {
    public static Task<bool> MoveNextAsync(this IDbAsyncEnumerator enumerator)
    {
      Check.NotNull<IDbAsyncEnumerator>(enumerator, nameof (enumerator));
      return enumerator.MoveNextAsync(CancellationToken.None);
    }

    internal static IDbAsyncEnumerator<TResult> Cast<TResult>(
      this IDbAsyncEnumerator source)
    {
      return (IDbAsyncEnumerator<TResult>) new IDbAsyncEnumeratorExtensions.CastDbAsyncEnumerator<TResult>(source);
    }

    private class CastDbAsyncEnumerator<TResult> : IDbAsyncEnumerator<TResult>, IDbAsyncEnumerator, IDisposable
    {
      private readonly IDbAsyncEnumerator _underlyingEnumerator;

      public CastDbAsyncEnumerator(IDbAsyncEnumerator sourceEnumerator)
      {
        this._underlyingEnumerator = sourceEnumerator;
      }

      public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
      {
        return this._underlyingEnumerator.MoveNextAsync(cancellationToken);
      }

      public TResult Current
      {
        get
        {
          return (TResult) this._underlyingEnumerator.Current;
        }
      }

      object IDbAsyncEnumerator.Current
      {
        get
        {
          return this._underlyingEnumerator.Current;
        }
      }

      public void Dispose()
      {
        this._underlyingEnumerator.Dispose();
      }
    }
  }
}
