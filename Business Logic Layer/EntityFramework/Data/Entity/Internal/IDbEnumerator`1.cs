// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.IDbEnumerator`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Internal
{
  internal interface IDbEnumerator<out T> : IEnumerator<T>, IEnumerator, IDbAsyncEnumerator<T>, IDbAsyncEnumerator, IDisposable
  {
    new T Current { get; }
  }
}
