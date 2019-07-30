// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Memoizer`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Data.Entity.Core.Common.Utils
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
  internal sealed class Memoizer<TArg, TResult>
  {
    private readonly Func<TArg, TResult> _function;
    private readonly Dictionary<TArg, Memoizer<TArg, TResult>.Result> _resultCache;
    private readonly ReaderWriterLockSlim _lock;

    internal Memoizer(Func<TArg, TResult> function, IEqualityComparer<TArg> argComparer)
    {
      this._function = function;
      this._resultCache = new Dictionary<TArg, Memoizer<TArg, TResult>.Result>(argComparer);
      this._lock = new ReaderWriterLockSlim();
    }

    internal TResult Evaluate(TArg arg)
    {
      Memoizer<TArg, TResult>.Result result;
      if (!this.TryGetResult(arg, out result))
      {
        this._lock.EnterWriteLock();
        try
        {
          if (!this._resultCache.TryGetValue(arg, out result))
          {
            result = new Memoizer<TArg, TResult>.Result((Func<TResult>) (() => this._function(arg)));
            this._resultCache.Add(arg, result);
          }
        }
        finally
        {
          this._lock.ExitWriteLock();
        }
      }
      return result.GetValue();
    }

    internal bool TryGetValue(TArg arg, out TResult value)
    {
      Memoizer<TArg, TResult>.Result result;
      if (this.TryGetResult(arg, out result))
      {
        value = result.GetValue();
        return true;
      }
      value = default (TResult);
      return false;
    }

    private bool TryGetResult(TArg arg, out Memoizer<TArg, TResult>.Result result)
    {
      this._lock.EnterReadLock();
      try
      {
        return this._resultCache.TryGetValue(arg, out result);
      }
      finally
      {
        this._lock.ExitReadLock();
      }
    }

    private class Result
    {
      private TResult _value;
      private Func<TResult> _delegate;

      internal Result(Func<TResult> createValueDelegate)
      {
        this._delegate = createValueDelegate;
      }

      internal TResult GetValue()
      {
        if (this._delegate == null)
          return this._value;
        lock (this)
        {
          if (this._delegate == null)
            return this._value;
          this._value = this._delegate();
          this._delegate = (Func<TResult>) null;
          return this._value;
        }
      }
    }
  }
}
