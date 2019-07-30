// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.InterceptionContextMutableData`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Infrastructure.Interception
{
  internal class InterceptionContextMutableData<TResult> : InterceptionContextMutableData
  {
    private TResult _result;

    public TResult OriginalResult { get; set; }

    public TResult Result
    {
      get
      {
        return this._result;
      }
      set
      {
        if (!this.HasExecuted)
          this.SuppressExecution();
        this._result = value;
      }
    }

    public void SetExecuted(TResult result)
    {
      this.HasExecuted = true;
      this.OriginalResult = result;
      this.Result = result;
    }
  }
}
