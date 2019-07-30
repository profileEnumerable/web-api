// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.Disposer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal class Disposer : IDisposable
  {
    private readonly Action _action;

    internal Disposer(Action action)
    {
      this._action = action;
    }

    public void Dispose()
    {
      this._action();
      GC.SuppressFinalize((object) this);
    }
  }
}
