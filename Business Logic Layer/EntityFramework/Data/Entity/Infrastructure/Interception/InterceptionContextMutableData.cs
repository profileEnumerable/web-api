// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.InterceptionContextMutableData
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure.Interception
{
  internal class InterceptionContextMutableData
  {
    private Exception _exception;
    private bool _isSuppressed;

    public bool HasExecuted { get; set; }

    public Exception OriginalException { get; set; }

    public TaskStatus TaskStatus { get; set; }

    public object UserState { get; set; }

    public bool IsExecutionSuppressed
    {
      get
      {
        return this._isSuppressed;
      }
    }

    public void SuppressExecution()
    {
      if (!this._isSuppressed && this.HasExecuted)
        throw new InvalidOperationException(Strings.SuppressionAfterExecution);
      this._isSuppressed = true;
    }

    public Exception Exception
    {
      get
      {
        return this._exception;
      }
      set
      {
        if (!this.HasExecuted)
          this.SuppressExecution();
        this._exception = value;
      }
    }

    public void SetExceptionThrown(Exception exception)
    {
      this.HasExecuted = true;
      this.OriginalException = exception;
      this.Exception = exception;
    }
  }
}
