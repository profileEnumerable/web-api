// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.LazyEnumerator`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Internal
{
  internal class LazyEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
  {
    private readonly Func<ObjectResult<T>> _getObjectResult;
    private IEnumerator<T> _objectResultEnumerator;

    public LazyEnumerator(Func<ObjectResult<T>> getObjectResult)
    {
      this._getObjectResult = getObjectResult;
    }

    public T Current
    {
      get
      {
        if (this._objectResultEnumerator != null)
          return this._objectResultEnumerator.Current;
        return default (T);
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this.Current;
      }
    }

    public void Dispose()
    {
      if (this._objectResultEnumerator == null)
        return;
      this._objectResultEnumerator.Dispose();
    }

    public bool MoveNext()
    {
      if (this._objectResultEnumerator == null)
      {
        ObjectResult<T> objectResult = this._getObjectResult();
        try
        {
          this._objectResultEnumerator = objectResult.GetEnumerator();
        }
        catch
        {
          objectResult.Dispose();
          throw;
        }
      }
      return this._objectResultEnumerator.MoveNext();
    }

    public void Reset()
    {
      if (this._objectResultEnumerator == null)
        return;
      this._objectResultEnumerator.Reset();
    }
  }
}
