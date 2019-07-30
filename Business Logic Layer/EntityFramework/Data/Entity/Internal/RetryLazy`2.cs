// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.RetryLazy`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics;

namespace System.Data.Entity.Internal
{
  internal class RetryLazy<TInput, TResult> where TResult : class
  {
    private readonly object _lock = new object();
    private Func<TInput, TResult> _valueFactory;
    private TResult _value;

    public RetryLazy(Func<TInput, TResult> valueFactory)
    {
      this._valueFactory = valueFactory;
    }

    [DebuggerStepThrough]
    public TResult GetValue(TInput input)
    {
      lock (this._lock)
      {
        if ((object) this._value == null)
        {
          Func<TInput, TResult> valueFactory = this._valueFactory;
          try
          {
            this._valueFactory = (Func<TInput, TResult>) null;
            this._value = valueFactory(input);
          }
          catch (Exception ex)
          {
            this._valueFactory = valueFactory;
            throw;
          }
        }
        return this._value;
      }
    }
  }
}
