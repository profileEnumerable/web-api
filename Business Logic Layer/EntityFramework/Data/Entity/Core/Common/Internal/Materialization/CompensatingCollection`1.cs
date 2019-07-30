// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.CompensatingCollection`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class CompensatingCollection<TElement> : IOrderedQueryable<TElement>, IQueryable<TElement>, IOrderedQueryable, IQueryable, IOrderedEnumerable<TElement>, IEnumerable<TElement>, IEnumerable
  {
    private readonly IEnumerable<TElement> _source;
    private readonly Expression _expression;

    public CompensatingCollection(IEnumerable<TElement> source)
    {
      this._source = source;
      this._expression = (Expression) Expression.Constant((object) source);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._source.GetEnumerator();
    }

    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
    {
      return this._source.GetEnumerator();
    }

    IOrderedEnumerable<TElement> IOrderedEnumerable<TElement>.CreateOrderedEnumerable<K>(
      Func<TElement, K> keySelector,
      IComparer<K> comparer,
      bool descending)
    {
      throw new NotSupportedException(Strings.ELinq_CreateOrderedEnumerableNotSupported);
    }

    Type IQueryable.ElementType
    {
      get
      {
        return typeof (TElement);
      }
    }

    Expression IQueryable.Expression
    {
      get
      {
        return this._expression;
      }
    }

    IQueryProvider IQueryable.Provider
    {
      get
      {
        throw new NotSupportedException(Strings.ELinq_UnsupportedQueryableMethod);
      }
    }
  }
}
