// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.DiscriminatedNewEntityOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class DiscriminatedNewEntityOp : NewEntityBaseOp
  {
    internal static readonly DiscriminatedNewEntityOp Pattern = new DiscriminatedNewEntityOp();
    private readonly ExplicitDiscriminatorMap m_discriminatorMap;

    internal DiscriminatedNewEntityOp(
      TypeUsage type,
      ExplicitDiscriminatorMap discriminatorMap,
      EntitySet entitySet,
      List<RelProperty> relProperties)
      : base(OpType.DiscriminatedNewEntity, type, true, entitySet, relProperties)
    {
      this.m_discriminatorMap = discriminatorMap;
    }

    private DiscriminatedNewEntityOp()
      : base(OpType.DiscriminatedNewEntity)
    {
    }

    internal ExplicitDiscriminatorMap DiscriminatorMap
    {
      get
      {
        return this.m_discriminatorMap;
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
