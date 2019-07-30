// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.NodeList`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class NodeList<T> : Node, IEnumerable<T>, IEnumerable where T : Node
  {
    private readonly List<T> _list = new List<T>();

    internal NodeList()
    {
    }

    internal NodeList(T item)
    {
      this._list.Add(item);
    }

    internal NodeList<T> Add(T item)
    {
      this._list.Add(item);
      return this;
    }

    internal int Count
    {
      get
      {
        return this._list.Count;
      }
    }

    internal T this[int index]
    {
      get
      {
        return this._list[index];
      }
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return (IEnumerator<T>) this._list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._list.GetEnumerator();
    }
  }
}
