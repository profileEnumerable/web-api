// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.VarDefListOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class VarDefListOp : AncillaryOp
  {
    internal static readonly VarDefListOp Instance = new VarDefListOp();
    internal static readonly VarDefListOp Pattern = VarDefListOp.Instance;

    private VarDefListOp()
      : base(OpType.VarDefList)
    {
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
