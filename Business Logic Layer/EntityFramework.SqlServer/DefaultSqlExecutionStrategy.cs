// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer.Resources;
using System.Data.Entity.SqlServer.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.SqlServer
{
  internal sealed class DefaultSqlExecutionStrategy : IDbExecutionStrategy
  {
    public bool RetriesOnFailure
    {
      get
      {
        return false;
      }
    }

    public void Execute(Action operation)
    {
      if (operation == null)
        throw new ArgumentNullException(nameof (operation));
      this.Execute<object>((Func<object>) (() =>
      {
        operation();
        return (object) null;
      }));
    }

    public TResult Execute<TResult>(Func<TResult> operation)
    {
      Check.NotNull<Func<TResult>>(operation, nameof (operation));
      try
      {
        return operation();
      }
      catch (Exception ex)
      {
        if (DbExecutionStrategy.UnwrapAndHandleException<bool>(ex, new Func<Exception, bool>(SqlAzureRetriableExceptionDetector.ShouldRetryOn)))
          throw new EntityException(Strings.TransientExceptionDetected, ex);
        throw;
      }
    }

    public Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken)
    {
      Check.NotNull<Func<Task>>(operation, nameof (operation));
      cancellationToken.ThrowIfCancellationRequested();
      return (Task) DefaultSqlExecutionStrategy.ExecuteAsyncImplementation<bool>((Func<Task<bool>>) (async () =>
      {
        await operation().ConfigureAwait(false);
        return true;
      }));
    }

    public Task<TResult> ExecuteAsync<TResult>(
      Func<Task<TResult>> operation,
      CancellationToken cancellationToken)
    {
      Check.NotNull<Func<Task<TResult>>>(operation, nameof (operation));
      cancellationToken.ThrowIfCancellationRequested();
      return DefaultSqlExecutionStrategy.ExecuteAsyncImplementation<TResult>(operation);
    }

    private static async Task<TResult> ExecuteAsyncImplementation<TResult>(
      Func<Task<TResult>> func)
    {
      TResult result;
      try
      {
        result = await func().ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        if (DbExecutionStrategy.UnwrapAndHandleException<bool>(ex, new Func<Exception, bool>(SqlAzureRetriableExceptionDetector.ShouldRetryOn)))
          throw new EntityException(Strings.TransientExceptionDetected, ex);
        throw;
      }
      return result;
    }
  }
}
