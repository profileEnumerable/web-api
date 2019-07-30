// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.SchemaElementLookUpTableEnumerator`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class SchemaElementLookUpTableEnumerator<T, S> : IEnumerator<T>, IDisposable, IEnumerator
    where T : S
    where S : SchemaElement
  {
    private readonly Dictionary<string, S> _data;
    private List<string>.Enumerator _enumerator;

    public SchemaElementLookUpTableEnumerator(Dictionary<string, S> data, List<string> keysInOrder)
    {
      this._data = data;
      this._enumerator = keysInOrder.GetEnumerator();
    }

    public void Reset()
    {
      ((IEnumerator) this._enumerator).Reset();
    }

    public T Current
    {
      get
      {
        return (object) this._data[this._enumerator.Current] as T;
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) ((object) this._data[this._enumerator.Current] as T);
      }
    }

    public bool MoveNext()
    {
      while (this._enumerator.MoveNext())
      {
        if ((object) this.Current != null)
          return true;
      }
      return false;
    }

    public void Dispose()
    {
    }
  }
}
