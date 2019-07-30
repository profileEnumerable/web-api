// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.InternalDispatcher`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure.Interception
{
  internal class InternalDispatcher<TInterceptor> where TInterceptor : class, IDbInterceptor
  {
    private volatile List<TInterceptor> _interceptors = new List<TInterceptor>();
    private readonly object _lock = new object();

    public void Add(IDbInterceptor interceptor)
    {
      TInterceptor interceptor1 = interceptor as TInterceptor;
      if ((object) interceptor1 == null)
        return;
      lock (this._lock)
      {
        List<TInterceptor> list = this._interceptors.ToList<TInterceptor>();
        list.Add(interceptor1);
        this._interceptors = list;
      }
    }

    public void Remove(IDbInterceptor interceptor)
    {
      TInterceptor interceptor1 = interceptor as TInterceptor;
      if ((object) interceptor1 == null)
        return;
      lock (this._lock)
      {
        List<TInterceptor> list = this._interceptors.ToList<TInterceptor>();
        list.Remove(interceptor1);
        this._interceptors = list;
      }
    }

    public TResult Dispatch<TResult>(
      TResult result,
      Func<TResult, TInterceptor, TResult> accumulator)
    {
      if (this._interceptors.Count != 0)
        return this._interceptors.Aggregate<TInterceptor, TResult>(result, accumulator);
      return result;
    }

    public void Dispatch(Action<TInterceptor> action)
    {
      if (this._interceptors.Count == 0)
        return;
      this._interceptors.Each<TInterceptor>(action);
    }

    public TResult Dispatch<TInterceptionContext, TResult>(
      TResult result,
      TInterceptionContext interceptionContext,
      Action<TInterceptor, TInterceptionContext> intercept)
      where TInterceptionContext : DbInterceptionContext, IDbMutableInterceptionContext<TResult>
    {
      if (this._interceptors.Count == 0)
        return result;
      interceptionContext.MutableData.SetExecuted(result);
      foreach (TInterceptor interceptor in this._interceptors)
        intercept(interceptor, interceptionContext);
      if (interceptionContext.MutableData.Exception != null)
        throw interceptionContext.MutableData.Exception;
      return interceptionContext.MutableData.Result;
    }

    public void Dispatch<TTarget, TInterceptionContext>(
      TTarget target,
      Action<TTarget, TInterceptionContext> operation,
      TInterceptionContext interceptionContext,
      Action<TInterceptor, TTarget, TInterceptionContext> executing,
      Action<TInterceptor, TTarget, TInterceptionContext> executed)
      where TInterceptionContext : DbInterceptionContext, IDbMutableInterceptionContext
    {
      if (this._interceptors.Count == 0)
      {
        operation(target, interceptionContext);
      }
      else
      {
        foreach (TInterceptor interceptor in this._interceptors)
          executing(interceptor, target, interceptionContext);
        if (!interceptionContext.MutableData.IsExecutionSuppressed)
        {
          try
          {
            operation(target, interceptionContext);
            interceptionContext.MutableData.HasExecuted = true;
          }
          catch (Exception ex)
          {
            interceptionContext.MutableData.SetExceptionThrown(ex);
            foreach (TInterceptor interceptor in this._interceptors)
              executed(interceptor, target, interceptionContext);
            if (object.ReferenceEquals((object) interceptionContext.MutableData.Exception, (object) ex))
              throw;
          }
        }
        if (interceptionContext.MutableData.OriginalException == null)
        {
          foreach (TInterceptor interceptor in this._interceptors)
            executed(interceptor, target, interceptionContext);
        }
        if (interceptionContext.MutableData.Exception != null)
          throw interceptionContext.MutableData.Exception;
      }
    }

    public TResult Dispatch<TTarget, TInterceptionContext, TResult>(
      TTarget target,
      Func<TTarget, TInterceptionContext, TResult> operation,
      TInterceptionContext interceptionContext,
      Action<TInterceptor, TTarget, TInterceptionContext> executing,
      Action<TInterceptor, TTarget, TInterceptionContext> executed)
      where TInterceptionContext : DbInterceptionContext, IDbMutableInterceptionContext<TResult>
    {
      if (this._interceptors.Count == 0)
        return operation(target, interceptionContext);
      foreach (TInterceptor interceptor in this._interceptors)
        executing(interceptor, target, interceptionContext);
      if (!interceptionContext.MutableData.IsExecutionSuppressed)
      {
        try
        {
          interceptionContext.MutableData.SetExecuted(operation(target, interceptionContext));
        }
        catch (Exception ex)
        {
          interceptionContext.MutableData.SetExceptionThrown(ex);
          foreach (TInterceptor interceptor in this._interceptors)
            executed(interceptor, target, interceptionContext);
          if (object.ReferenceEquals((object) interceptionContext.MutableData.Exception, (object) ex))
            throw;
        }
      }
      if (interceptionContext.MutableData.OriginalException == null)
      {
        foreach (TInterceptor interceptor in this._interceptors)
          executed(interceptor, target, interceptionContext);
      }
      if (interceptionContext.MutableData.Exception != null)
        throw interceptionContext.MutableData.Exception;
      return interceptionContext.MutableData.Result;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public Task DispatchAsync<TTarget, TInterceptionContext>(
      TTarget target,
      Func<TTarget, TInterceptionContext, CancellationToken, Task> operation,
      TInterceptionContext interceptionContext,
      Action<TInterceptor, TTarget, TInterceptionContext> executing,
      Action<TInterceptor, TTarget, TInterceptionContext> executed,
      CancellationToken cancellationToken)
      where TInterceptionContext : DbInterceptionContext, IDbMutableInterceptionContext
    {
      if (this._interceptors.Count == 0)
        return operation(target, interceptionContext, cancellationToken);
      foreach (TInterceptor interceptor in this._interceptors)
        executing(interceptor, target, interceptionContext);
      Task task = interceptionContext.MutableData.IsExecutionSuppressed ? (Task) Task.FromResult<object>((object) null) : operation(target, interceptionContext, cancellationToken);
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      task.ContinueWith((Action<Task>) (t =>
      {
        interceptionContext.MutableData.TaskStatus = t.Status;
        if (t.IsFaulted)
          interceptionContext.MutableData.SetExceptionThrown(t.Exception.InnerException);
        else if (!interceptionContext.MutableData.IsExecutionSuppressed)
          interceptionContext.MutableData.HasExecuted = true;
        try
        {
          foreach (TInterceptor interceptor in this._interceptors)
            executed(interceptor, target, interceptionContext);
        }
        catch (Exception ex)
        {
          interceptionContext.MutableData.Exception = ex;
        }
        if (interceptionContext.MutableData.Exception != null)
          tcs.SetException(interceptionContext.MutableData.Exception);
        else if (t.IsCanceled)
          tcs.SetCanceled();
        else
          tcs.SetResult((object) null);
      }), TaskContinuationOptions.ExecuteSynchronously);
      return (Task) tcs.Task;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public Task<TResult> DispatchAsync<TTarget, TInterceptionContext, TResult>(
      TTarget target,
      Func<TTarget, TInterceptionContext, CancellationToken, Task<TResult>> operation,
      TInterceptionContext interceptionContext,
      Action<TInterceptor, TTarget, TInterceptionContext> executing,
      Action<TInterceptor, TTarget, TInterceptionContext> executed,
      CancellationToken cancellationToken)
      where TInterceptionContext : DbInterceptionContext, IDbMutableInterceptionContext<TResult>
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (this._interceptors.Count == 0)
        return operation(target, interceptionContext, cancellationToken);
      foreach (TInterceptor interceptor in this._interceptors)
        executing(interceptor, target, interceptionContext);
      Task<TResult> task = interceptionContext.MutableData.IsExecutionSuppressed ? Task.FromResult<TResult>(interceptionContext.MutableData.Result) : operation(target, interceptionContext, cancellationToken);
      TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
      task.ContinueWith((Action<Task<TResult>>) (t =>
      {
        interceptionContext.MutableData.TaskStatus = t.Status;
        if (t.IsFaulted)
          interceptionContext.MutableData.SetExceptionThrown(t.Exception.InnerException);
        else if (!interceptionContext.MutableData.IsExecutionSuppressed)
          interceptionContext.MutableData.SetExecuted(t.IsCanceled || t.IsFaulted ? default (TResult) : t.Result);
        try
        {
          foreach (TInterceptor interceptor in this._interceptors)
            executed(interceptor, target, interceptionContext);
        }
        catch (Exception ex)
        {
          interceptionContext.MutableData.Exception = ex;
        }
        if (interceptionContext.MutableData.Exception != null)
          tcs.SetException(interceptionContext.MutableData.Exception);
        else if (t.IsCanceled)
          tcs.SetCanceled();
        else
          tcs.SetResult(interceptionContext.MutableData.Result);
      }), TaskContinuationOptions.ExecuteSynchronously);
      return tcs.Task;
    }
  }
}
