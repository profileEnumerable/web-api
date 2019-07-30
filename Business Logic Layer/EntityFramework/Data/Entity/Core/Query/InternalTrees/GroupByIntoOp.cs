// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.GroupByIntoOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class GroupByIntoOp : GroupByBaseOp
  {
    internal static readonly GroupByIntoOp Pattern = new GroupByIntoOp();
    private readonly VarVec m_inputs;

    private GroupByIntoOp()
      : base(OpType.GroupByInto)
    {
    }

    internal GroupByIntoOp(VarVec keys, VarVec inputs, VarVec outputs)
      : base(OpType.GroupByInto, keys, outputs)
    {
      this.m_inputs = inputs;
    }

    internal VarVec Inputs
    {
      get
      {
        return this.m_inputs;
      }
    }

    internal override int Arity
    {
      get
      {
        return 4;
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
