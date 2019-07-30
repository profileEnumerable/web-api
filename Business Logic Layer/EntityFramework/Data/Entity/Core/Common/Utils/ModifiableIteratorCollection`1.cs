// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.ModifiableIteratorCollection`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Data.Entity.Core.Common.Utils
{
  internal class ModifiableIteratorCollection<TElement> : InternalBase
  {
    private readonly List<TElement> m_elements;
    private int m_currentIteratorIndex;

    internal ModifiableIteratorCollection(IEnumerable<TElement> elements)
    {
      this.m_elements = new List<TElement>(elements);
      this.m_currentIteratorIndex = -1;
    }

    internal bool IsEmpty
    {
      get
      {
        return this.m_elements.Count == 0;
      }
    }

    internal TElement RemoveOneElement()
    {
      return this.Remove(this.m_elements.Count - 1);
    }

    internal void ResetIterator()
    {
      this.m_currentIteratorIndex = -1;
    }

    internal void RemoveCurrentOfIterator()
    {
      this.Remove(this.m_currentIteratorIndex);
      --this.m_currentIteratorIndex;
    }

    internal IEnumerable<TElement> Elements()
    {
      for (this.m_currentIteratorIndex = 0; this.m_currentIteratorIndex < this.m_elements.Count; ++this.m_currentIteratorIndex)
        yield return this.m_elements[this.m_currentIteratorIndex];
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      StringUtil.ToCommaSeparatedString(builder, (IEnumerable) this.m_elements);
    }

    private TElement Remove(int index)
    {
      TElement element = this.m_elements[index];
      int index1 = this.m_elements.Count - 1;
      this.m_elements[index] = this.m_elements[index1];
      this.m_elements.RemoveAt(index1);
      return element;
    }
  }
}
