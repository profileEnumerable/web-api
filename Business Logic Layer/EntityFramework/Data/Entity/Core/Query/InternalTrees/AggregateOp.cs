// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.AggregateOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class AggregateOp : ScalarOp
  {
    internal static readonly AggregateOp Pattern = new AggregateOp();
    private readonly EdmFunction m_aggFunc;
    private readonly bool m_distinctAgg;

    internal AggregateOp(EdmFunction aggFunc, bool distinctAgg)
      : base(OpType.Aggregate, aggFunc.ReturnParameter.TypeUsage)
    {
      this.m_aggFunc = aggFunc;
      this.m_distinctAgg = distinctAgg;
    }

    private AggregateOp()
      : base(OpType.Aggregate)
    {
    }

    internal EdmFunction AggFunc
    {
      get
      {
        return this.m_aggFunc;
      }
    }

    internal bool IsDistinctAggregate
    {
      get
      {
        return this.m_distinctAgg;
      }
    }

    internal override bool IsAggregateOp
    {
      get
      {
        return true;
      }
    }

    [DebuggerNonUserCode]
    internal override void Accept(BasicOpVisitor v, Node n)
    {
      v.Visit(this, n);
    }

    [DebuggerNonUserCode]
    internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n)
    {
      return v.Visit(this, n);
    }
  }
}
