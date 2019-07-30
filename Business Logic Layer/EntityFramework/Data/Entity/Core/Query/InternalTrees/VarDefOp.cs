// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.VarDefOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class VarDefOp : AncillaryOp
  {
    internal static readonly VarDefOp Pattern = new VarDefOp();
    private readonly Var m_var;

    internal VarDefOp(Var v)
      : this()
    {
      this.m_var = v;
    }

    private VarDefOp()
      : base(OpType.VarDef)
    {
    }

    internal override int Arity
    {
      get
      {
        return 1;
      }
    }

    internal Var Var
    {
      get
      {
        return this.m_var;
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
