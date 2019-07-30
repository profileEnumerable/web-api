// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.PropertyOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class PropertyOp : ScalarOp
  {
    internal static readonly PropertyOp Pattern = new PropertyOp();
    private readonly EdmMember m_property;

    internal PropertyOp(TypeUsage type, EdmMember property)
      : base(OpType.Property, type)
    {
      this.m_property = property;
    }

    private PropertyOp()
      : base(OpType.Property)
    {
    }

    internal override int Arity
    {
      get
      {
        return 1;
      }
    }

    internal EdmMember PropertyInfo
    {
      get
      {
        return this.m_property;
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

    internal override bool IsEquivalent(Op other)
    {
      PropertyOp propertyOp = other as PropertyOp;
      if (propertyOp != null && propertyOp.PropertyInfo.EdmEquals((MetadataItem) this.PropertyInfo))
        return base.IsEquivalent(other);
      return false;
    }
  }
}
