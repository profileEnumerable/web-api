// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.RetryAction`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics;

namespace System.Data.Entity.Internal
{
  internal class RetryAction<TInput>
  {
    private readonly object _lock = new object();
    private Action<TInput> _action;

    public RetryAction(Action<TInput> action)
    {
      this._action = action;
    }

    [DebuggerStepThrough]
    public void PerformAction(TInput input)
    {
      lock (this._lock)
      {
        if (this._action == null)
          return;
        Action<TInput> action = this._action;
        this._action = (Action<TInput>) null;
        try
        {
          action(input);
        }
        catch (Exception ex)
        {
          this._action = action;
          throw;
        }
      }
    }
  }
}
