// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ScalarOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class ScalarOp : Op
  {
    private TypeUsage m_type;

    internal ScalarOp(OpType opType, TypeUsage type)
      : this(opType)
    {
      this.m_type = type;
    }

    protected ScalarOp(OpType opType)
      : base(opType)
    {
    }

    internal override bool IsScalarOp
    {
      get
      {
        return true;
      }
    }

    internal override bool IsEquivalent(Op other)
    {
      if (other.OpType == this.OpType)
        return TypeSemantics.IsStructurallyEqual(this.Type, other.Type);
      return false;
    }

    internal override TypeUsage Type
    {
      get
      {
        return this.m_type;
      }
      set
      {
        this.m_type = value;
      }
    }

    internal virtual bool IsAggregateOp
    {
      get
      {
        return false;
      }
    }
  }
}
