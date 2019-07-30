// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IDbAsyncEnumerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Asynchronous version of the <see cref="T:System.Collections.IEnumerator" /> interface that allows elements to be retrieved asynchronously.
  /// This interface is used to interact with Entity Framework queries and shouldn't be implemented by custom classes.
  /// </summary>
  public interface IDbAsyncEnumerator : IDisposable
  {
    /// <summary>
    /// Advances the enumerator to the next element in the sequence, returning the result asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the sequence.
    /// </returns>
    Task<bool> MoveNextAsync(CancellationToken cancellationToken);

    /// <summary>Gets the current element in the iteration.</summary>
    object Current { get; }
  }
}
