// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ThrowingMonitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Data.Entity.Internal
{
  internal class ThrowingMonitor
  {
    private int _isInCriticalSection;

    public void Enter()
    {
      if (Interlocked.CompareExchange(ref this._isInCriticalSection, 1, 0) != 0)
        throw new NotSupportedException(Strings.ConcurrentMethodInvocation);
    }

    [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Used in the debug build", MessageId = "state")]
    public void Exit()
    {
      Interlocked.Exchange(ref this._isInCriticalSection, 0);
    }

    public void EnsureNotEntered()
    {
      Thread.MemoryBarrier();
      if (this._isInCriticalSection != 0)
        throw new NotSupportedException(Strings.ConcurrentMethodInvocation);
    }
  }
}
