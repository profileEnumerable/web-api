// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.NavigateOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class NavigateOp : ScalarOp
  {
    internal static readonly NavigateOp Pattern = new NavigateOp();
    private readonly RelProperty m_property;

    internal NavigateOp(TypeUsage type, RelProperty relProperty)
      : base(OpType.Navigate, type)
    {
      this.m_property = relProperty;
    }

    private NavigateOp()
      : base(OpType.Navigate)
    {
    }

    internal override int Arity
    {
      get
      {
        return 1;
      }
    }

    internal RelProperty RelProperty
    {
      get
      {
        return this.m_property;
      }
    }

    internal RelationshipType Relationship
    {
      get
      {
        return this.m_property.Relationship;
      }
    }

    internal RelationshipEndMember FromEnd
    {
      get
      {
        return this.m_property.FromEnd;
      }
    }

    internal RelationshipEndMember ToEnd
    {
      get
      {
        return this.m_property.ToEnd;
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
