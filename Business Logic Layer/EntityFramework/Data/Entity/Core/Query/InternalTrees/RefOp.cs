// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.RefOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class RefOp : ScalarOp
  {
    internal static readonly RefOp Pattern = new RefOp();
    private readonly EntitySet m_entitySet;

    internal RefOp(EntitySet entitySet, TypeUsage type)
      : base(OpType.Ref, type)
    {
      this.m_entitySet = entitySet;
    }

    private RefOp()
      : base(OpType.Ref)
    {
    }

    internal override int Arity
    {
      get
      {
        return 1;
      }
    }

    internal EntitySet EntitySet
    {
      get
      {
        return this.m_entitySet;
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
