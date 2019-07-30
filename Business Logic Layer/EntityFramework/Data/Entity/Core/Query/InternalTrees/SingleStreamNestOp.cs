// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.SingleStreamNestOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class SingleStreamNestOp : NestBaseOp
  {
    private readonly VarVec m_keys;
    private readonly Var m_discriminator;
    private readonly List<SortKey> m_postfixSortKeys;

    internal override int Arity
    {
      get
      {
        return 1;
      }
    }

    internal Var Discriminator
    {
      get
      {
        return this.m_discriminator;
      }
    }

    internal List<SortKey> PostfixSortKeys
    {
      get
      {
        return this.m_postfixSortKeys;
      }
    }

    internal VarVec Keys
    {
      get
      {
        return this.m_keys;
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

    internal SingleStreamNestOp(
      VarVec keys,
      List<SortKey> prefixSortKeys,
      List<SortKey> postfixSortKeys,
      VarVec outputVars,
      List<CollectionInfo> collectionInfoList,
      Var discriminatorVar)
      : base(OpType.SingleStreamNest, prefixSortKeys, outputVars, collectionInfoList)
    {
      this.m_keys = keys;
      this.m_postfixSortKeys = postfixSortKeys;
      this.m_discriminator = discriminatorVar;
    }
  }
}
