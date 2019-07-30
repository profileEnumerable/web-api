// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.Op
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class Op
  {
    internal const int ArityVarying = -1;
    private readonly OpType m_opType;

    internal Op(OpType opType)
    {
      this.m_opType = opType;
    }

    internal OpType OpType
    {
      get
      {
        return this.m_opType;
      }
    }

    internal virtual int Arity
    {
      get
      {
        return -1;
      }
    }

    internal virtual bool IsScalarOp
    {
      get
      {
        return false;
      }
    }

    internal virtual bool IsRulePatternOp
    {
      get
      {
        return false;
      }
    }

    internal virtual bool IsRelOp
    {
      get
      {
        return false;
      }
    }

    internal virtual bool IsAncillaryOp
    {
      get
      {
        return false;
      }
    }

    internal virtual bool IsPhysicalOp
    {
      get
      {
        return false;
      }
    }

    internal virtual bool IsEquivalent(Op other)
    {
      return false;
    }

    internal virtual TypeUsage Type
    {
      get
      {
        return (TypeUsage) null;
      }
      set
      {
        throw Error.NotSupported();
      }
    }

    [DebuggerNonUserCode]
    internal virtual void Accept(BasicOpVisitor v, Node n)
    {
      v.Visit(this, n);
    }

    [DebuggerNonUserCode]
    internal virtual TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n)
    {
      return v.Visit(this, n);
    }
  }
}
