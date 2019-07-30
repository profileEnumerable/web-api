// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.GroupByBaseOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class GroupByBaseOp : RelOp
  {
    private readonly VarVec m_keys;
    private readonly VarVec m_outputs;

    protected GroupByBaseOp(OpType opType)
      : base(opType)
    {
    }

    internal GroupByBaseOp(OpType opType, VarVec keys, VarVec outputs)
      : this(opType)
    {
      this.m_keys = keys;
      this.m_outputs = outputs;
    }

    internal VarVec Keys
    {
      get
      {
        return this.m_keys;
      }
    }

    internal VarVec Outputs
    {
      get
      {
        return this.m_outputs;
      }
    }

    [DebuggerNonUserCode]
    internal override void Accept(BasicOpVisitor v, Node n)
    {
      v.Visit((Op) this, n);
    }

    [DebuggerNonUserCode]
    internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n)
    {
      return v.Visit((Op) this, n);
    }
  }
}
