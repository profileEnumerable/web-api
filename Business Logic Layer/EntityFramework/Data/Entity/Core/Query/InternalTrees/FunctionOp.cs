// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.FunctionOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class FunctionOp : ScalarOp
  {
    internal static readonly FunctionOp Pattern = new FunctionOp();
    private readonly EdmFunction m_function;

    internal FunctionOp(EdmFunction function)
      : base(OpType.Function, function.ReturnParameter.TypeUsage)
    {
      this.m_function = function;
    }

    private FunctionOp()
      : base(OpType.Function)
    {
    }

    internal EdmFunction Function
    {
      get
      {
        return this.m_function;
      }
    }

    internal override bool IsEquivalent(Op other)
    {
      FunctionOp functionOp = other as FunctionOp;
      if (functionOp != null)
        return functionOp.Function.EdmEquals((MetadataItem) this.Function);
      return false;
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
