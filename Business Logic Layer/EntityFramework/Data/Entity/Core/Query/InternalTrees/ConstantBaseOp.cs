// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ConstantBaseOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class ConstantBaseOp : ScalarOp
  {
    private readonly object m_value;

    protected ConstantBaseOp(OpType opType, TypeUsage type, object value)
      : base(opType, type)
    {
      this.m_value = value;
    }

    protected ConstantBaseOp(OpType opType)
      : base(opType)
    {
    }

    internal virtual object Value
    {
      get
      {
        return this.m_value;
      }
    }

    internal override int Arity
    {
      get
      {
        return 0;
      }
    }

    internal override bool IsEquivalent(Op other)
    {
      ConstantBaseOp constantBaseOp = other as ConstantBaseOp;
      if (constantBaseOp == null || this.OpType != other.OpType || !constantBaseOp.Type.EdmEquals((MetadataItem) this.Type))
        return false;
      if (constantBaseOp.Value != null || this.Value != null)
        return constantBaseOp.Value.Equals(this.Value);
      return true;
    }
  }
}
