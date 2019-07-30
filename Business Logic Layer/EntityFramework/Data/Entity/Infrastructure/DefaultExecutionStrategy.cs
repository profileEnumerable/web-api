// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DefaultExecutionStrategy
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// An <see cref="T:System.Data.Entity.Infrastructure.IDbExecutionStrategy" /> that doesn't retry operations if they fail.
  /// </summary>
  public class DefaultExecutionStrategy : IDbExecutionStrategy
  {
    /// <summary>
    /// Returns <c>false</c> to indicate that <see cref="T:System.Data.Entity.Infrastructure.DefaultExecutionStrategy" /> will not retry the execution after a failure.
    /// </summary>
    public bool RetriesOnFailure
    {
      get
      {
        return false;
      }
    }

    /// <summary>Executes the specified operation once.</summary>
    /// <param name="operation">A delegate representing an executable operation that doesn't return any results.</param>
    public void Execute(Action operation)
    {
      operation();
    }

    /// <summary>
    /// Executes the specified operation once and returns the result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The return type of <paramref name="operation" />.
    /// </typeparam>
    /// <param name="operation">
    /// A delegate representing an executable operation that returns the result of type <typeparamref name="TResult" />.
    /// </param>
    /// <returns>The result from the operation.</returns>
    public TResult Execute<TResult>(Func<TResult> operation)
    {
      return operation();
    }

    /// <summary>
    /// Executes the specified asynchronous operation once, without retrying on failure.
    /// </summary>
    /// <param name="operation">A function that returns a started task.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to cancel the retry operation, but not operations that are already in flight
    /// or that already completed successfully.
    /// </param>
    /// <returns>
    /// A task that will run to completion if the original task completes successfully.
    /// </returns>
    public Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return operation();
    }

    /// <summary>
    /// Executes the specified asynchronous operation once, without retrying on failure.
    /// </summary>
    /// <typeparam name="TResult">
    /// The result type of the <see cref="T:System.Threading.Tasks.Task`1" /> returned by <paramref name="operation" />.
    /// </typeparam>
    /// <param name="operation">A function that returns a started task.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to cancel the retry operation, but not operations that are already in flight
    /// or that already completed successfully.
    /// </param>
    /// <returns>
    /// A task that will run to completion if the original task completes successfully.
    /// </returns>
    public Task<TResult> ExecuteAsync<TResult>(
      Func<Task<TResult>> operation,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return operation();
    }
  }
}
