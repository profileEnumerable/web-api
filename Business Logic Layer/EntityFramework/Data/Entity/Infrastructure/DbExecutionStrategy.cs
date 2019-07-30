// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbExecutionStrategy
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Provides the base implementation of the retry mechanism for unreliable operations and transient conditions that uses
  /// exponentially increasing delays between retries.
  /// </summary>
  /// <remarks>
  /// A new instance will be created each time an operation is executed.
  /// The following formula is used to calculate the delay after <c>retryCount</c> number of attempts:
  /// <code>min(random(1, 1.1) * (2 ^ retryCount - 1), maxDelay)</code>
  /// The <c>retryCount</c> starts at 0.
  /// The random factor distributes uniformly the retry attempts from multiple simultaneous operations failing simultaneously.
  /// </remarks>
  public abstract class DbExecutionStrategy : IDbExecutionStrategy
  {
    private static readonly TimeSpan DefaultCoefficient = TimeSpan.FromSeconds(1.0);
    private static readonly TimeSpan DefaultMaxDelay = TimeSpan.FromSeconds(30.0);
    private readonly List<Exception> _exceptionsEncountered = new List<Exception>();
    private readonly Random _random = new Random();
    private const int DefaultMaxRetryCount = 5;
    private const double DefaultRandomFactor = 1.1;
    private const double DefaultExponentialBase = 2.0;
    private readonly int _maxRetryCount;
    private readonly TimeSpan _maxDelay;

    /// <summary>
    /// Creates a new instance of <see cref="T:System.Data.Entity.Infrastructure.DbExecutionStrategy" />.
    /// </summary>
    /// <remarks>
    /// The default retry limit is 5, which means that the total amount of time spent between retries is 26 seconds plus the random factor.
    /// </remarks>
    protected DbExecutionStrategy()
      : this(5, DbExecutionStrategy.DefaultMaxDelay)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="T:System.Data.Entity.Infrastructure.DbExecutionStrategy" /> with the specified limits for number of retries and the delay between retries.
    /// </summary>
    /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
    /// <param name="maxDelay"> The maximum delay in milliseconds between retries. </param>
    protected DbExecutionStrategy(int maxRetryCount, TimeSpan maxDelay)
    {
      if (maxRetryCount < 0)
        throw new ArgumentOutOfRangeException(nameof (maxRetryCount));
      if (maxDelay.TotalMilliseconds < 0.0)
        throw new ArgumentOutOfRangeException(nameof (maxDelay));
      this._maxRetryCount = maxRetryCount;
      this._maxDelay = maxDelay;
    }

    /// <summary>
    /// Returns <c>true</c> to indicate that <see cref="T:System.Data.Entity.Infrastructure.DbExecutionStrategy" /> might retry the execution after a failure.
    /// </summary>
    public bool RetriesOnFailure
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Repetitively executes the specified operation while it satisfies the current retry policy.
    /// </summary>
    /// <param name="operation">A delegate representing an executable operation that doesn't return any results.</param>
    /// <exception cref="T:System.Data.Entity.Infrastructure.RetryLimitExceededException">if the retry delay strategy determines the operation shouldn't be retried anymore</exception>
    /// <exception cref="T:System.InvalidOperationException">if an existing transaction is detected and the execution strategy doesn't support it</exception>
    /// <exception cref="T:System.InvalidOperationException">if this instance was already used to execute an operation</exception>
    public void Execute(Action operation)
    {
      Check.NotNull<Action>(operation, nameof (operation));
      this.Execute<object>((Func<object>) (() =>
      {
        operation();
        return (object) null;
      }));
    }

    /// <summary>
    /// Repetitively executes the specified operation while it satisfies the current retry policy.
    /// </summary>
    /// <typeparam name="TResult">The type of result expected from the executable operation.</typeparam>
    /// <param name="operation">
    /// A delegate representing an executable operation that returns the result of type <typeparamref name="TResult" />.
    /// </param>
    /// <returns>The result from the operation.</returns>
    /// <exception cref="T:System.Data.Entity.Infrastructure.RetryLimitExceededException">if the retry delay strategy determines the operation shouldn't be retried anymore</exception>
    /// <exception cref="T:System.InvalidOperationException">if an existing transaction is detected and the execution strategy doesn't support it</exception>
    /// <exception cref="T:System.InvalidOperationException">if this instance was already used to execute an operation</exception>
    public TResult Execute<TResult>(Func<TResult> operation)
    {
      Check.NotNull<Func<TResult>>(operation, nameof (operation));
      this.EnsurePreexecutionState();
      TimeSpan? nextDelay;
      while (true)
      {
        try
        {
          return operation();
        }
        catch (Exception ex)
        {
          if (!DbExecutionStrategy.UnwrapAndHandleException<bool>(ex, new Func<Exception, bool>(this.ShouldRetryOn)))
          {
            throw;
          }
          else
          {
            nextDelay = this.GetNextDelay(ex);
            if (!nextDelay.HasValue)
              throw new RetryLimitExceededException(Strings.ExecutionStrategy_RetryLimitExceeded((object) this._maxRetryCount, (object) this.GetType().Name), ex);
          }
        }
        TimeSpan? nullable = nextDelay;
        TimeSpan zero = TimeSpan.Zero;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() < zero ? 1 : 0) : 0) == 0)
          Thread.Sleep(nextDelay.Value);
        else
          break;
      }
      throw new InvalidOperationException(Strings.ExecutionStrategy_NegativeDelay((object) nextDelay));
    }

    /// <summary>
    /// Repetitively executes the specified asynchronous operation while it satisfies the current retry policy.
    /// </summary>
    /// <param name="operation">A function that returns a started task.</param>
    /// <param name="cancellationToken">
    /// A cancellation token used to cancel the retry operation, but not operations that are already in flight
    /// or that already completed successfully.
    /// </param>
    /// <returns>
    /// A task that will run to completion if the original task completes successfully (either the
    /// first time or after retrying transient failures). If the task fails with a non-transient error or
    /// the retry limit is reached, the returned task will become faulted and the exception must be observed.
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Infrastructure.RetryLimitExceededException">if the retry delay strategy determines the operation shouldn't be retried anymore</exception>
    /// <exception cref="T:System.InvalidOperationException">if an existing transaction is detected and the execution strategy doesn't support it</exception>
    /// <exception cref="T:System.InvalidOperationException">if this instance was already used to execute an operation</exception>
    public Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken)
    {
      Check.NotNull<Func<Task>>(operation, nameof (operation));
      this.EnsurePreexecutionState();
      cancellationToken.ThrowIfCancellationRequested();
      return (Task) this.ProtectedExecuteAsync<bool>((Func<Task<bool>>) (async () =>
      {
        await operation().WithCurrentCulture();
        return true;
      }), cancellationToken);
    }

    /// <summary>
    /// Repeatedly executes the specified asynchronous operation while it satisfies the current retry policy.
    /// </summary>
    /// <typeparam name="TResult">
    /// The result type of the <see cref="T:System.Threading.Tasks.Task`1" /> returned by <paramref name="operation" />.
    /// </typeparam>
    /// <param name="operation">
    /// A function that returns a started task of type <typeparamref name="TResult" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token used to cancel the retry operation, but not operations that are already in flight
    /// or that already completed successfully.
    /// </param>
    /// <returns>
    /// A task that will run to completion if the original task completes successfully (either the
    /// first time or after retrying transient failures). If the task fails with a non-transient error or
    /// the retry limit is reached, the returned task will become faulted and the exception must be observed.
    /// </returns>
    /// <exception cref="T:System.Data.Entity.Infrastructure.RetryLimitExceededException">if the retry delay strategy determines the operation shouldn't be retried anymore</exception>
    /// <exception cref="T:System.InvalidOperationException">if an existing transaction is detected and the execution strategy doesn't support it</exception>
    /// <exception cref="T:System.InvalidOperationException">if this instance was already used to execute an operation</exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public Task<TResult> ExecuteAsync<TResult>(
      Func<Task<TResult>> operation,
      CancellationToken cancellationToken)
    {
      Check.NotNull<Func<Task<TResult>>>(operation, nameof (operation));
      this.EnsurePreexecutionState();
      cancellationToken.ThrowIfCancellationRequested();
      return this.ProtectedExecuteAsync<TResult>(operation, cancellationToken);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    private async Task<TResult> ProtectedExecuteAsync<TResult>(
      Func<Task<TResult>> operation,
      CancellationToken cancellationToken)
    {
      TimeSpan? delay;
      TResult result;
      while (true)
      {
        try
        {
          result = await operation().WithCurrentCulture<TResult>();
          goto label_10;
        }
        catch (Exception ex)
        {
          if (!DbExecutionStrategy.UnwrapAndHandleException<bool>(ex, new Func<Exception, bool>(this.ShouldRetryOn)))
          {
            throw;
          }
          else
          {
            delay = this.GetNextDelay(ex);
            if (!delay.HasValue)
              throw new RetryLimitExceededException(Strings.ExecutionStrategy_RetryLimitExceeded((object) this._maxRetryCount, (object) this.GetType().Name), ex);
          }
        }
        TimeSpan? nullable = delay;
        TimeSpan zero = TimeSpan.Zero;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() < zero ? 1 : 0) : 0) == 0)
          await Task.Delay(delay.Value, cancellationToken).WithCurrentCulture();
        else
          break;
      }
      throw new InvalidOperationException(Strings.ExecutionStrategy_NegativeDelay((object) delay));
label_10:
      return result;
    }

    private void EnsurePreexecutionState()
    {
      if (Transaction.Current != (Transaction) null)
        throw new InvalidOperationException(Strings.ExecutionStrategy_ExistingTransaction((object) this.GetType().Name));
      this._exceptionsEncountered.Clear();
    }

    /// <summary>
    /// Determines whether the operation should be retried and the delay before the next attempt.
    /// </summary>
    /// <param name="lastException">The exception thrown during the last execution attempt.</param>
    /// <returns>
    /// Returns the delay indicating how long to wait for before the next execution attempt if the operation should be retried;
    /// <c>null</c> otherwise
    /// </returns>
    protected internal virtual TimeSpan? GetNextDelay(Exception lastException)
    {
      this._exceptionsEncountered.Add(lastException);
      int num1 = this._exceptionsEncountered.Count - 1;
      if (num1 >= this._maxRetryCount)
        return new TimeSpan?();
      double num2 = (Math.Pow(2.0, (double) num1) - 1.0) * (1.0 + this._random.NextDouble() * 0.1);
      return new TimeSpan?(TimeSpan.FromMilliseconds(Math.Min(DbExecutionStrategy.DefaultCoefficient.TotalMilliseconds * num2, this._maxDelay.TotalMilliseconds)));
    }

    /// <summary>
    /// Recursively gets InnerException from <paramref name="exception" /> as long as it's an
    /// <see cref="T:System.Data.Entity.Core.EntityException" />, <see cref="T:System.Data.Entity.Infrastructure.DbUpdateException" /> or <see cref="T:System.Data.Entity.Core.UpdateException" />
    /// and passes it to <paramref name="exceptionHandler" />
    /// </summary>
    /// <typeparam name="T">The type of the unwrapped exception.</typeparam>
    /// <param name="exception"> The exception to be unwrapped. </param>
    /// <param name="exceptionHandler"> A delegate that will be called with the unwrapped exception. </param>
    /// <returns>
    /// The result from <paramref name="exceptionHandler" />.
    /// </returns>
    public static T UnwrapAndHandleException<T>(
      Exception exception,
      Func<Exception, T> exceptionHandler)
    {
      EntityException entityException = exception as EntityException;
      if (entityException != null)
        return DbExecutionStrategy.UnwrapAndHandleException<T>(entityException.InnerException, exceptionHandler);
      DbUpdateException dbUpdateException = exception as DbUpdateException;
      if (dbUpdateException != null)
        return DbExecutionStrategy.UnwrapAndHandleException<T>(dbUpdateException.InnerException, exceptionHandler);
      UpdateException updateException = exception as UpdateException;
      if (updateException != null)
        return DbExecutionStrategy.UnwrapAndHandleException<T>(updateException.InnerException, exceptionHandler);
      return exceptionHandler(exception);
    }

    /// <summary>
    /// Determines whether the specified exception represents a transient failure that can be compensated by a retry.
    /// </summary>
    /// <param name="exception">The exception object to be verified.</param>
    /// <returns>
    /// <c>true</c> if the specified exception is considered as transient, otherwise <c>false</c>.
    /// </returns>
    protected internal abstract bool ShouldRetryOn(Exception exception);
  }
}
