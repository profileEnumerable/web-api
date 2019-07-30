// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.SetOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class SetOp : RelOp
  {
    private readonly System.Data.Entity.Core.Query.InternalTrees.VarMap[] m_varMap;
    private readonly VarVec m_outputVars;

    internal SetOp(OpType opType, VarVec outputs, System.Data.Entity.Core.Query.InternalTrees.VarMap left, System.Data.Entity.Core.Query.InternalTrees.VarMap right)
      : this(opType)
    {
      this.m_varMap = new System.Data.Entity.Core.Query.InternalTrees.VarMap[2];
      this.m_varMap[0] = left;
      this.m_varMap[1] = right;
      this.m_outputVars = outputs;
    }

    protected SetOp(OpType opType)
      : base(opType)
    {
    }

    internal override int Arity
    {
      get
      {
        return 2;
      }
    }

    internal System.Data.Entity.Core.Query.InternalTrees.VarMap[] VarMap
    {
      get
      {
        return this.m_varMap;
      }
    }

    internal VarVec Outputs
    {
      get
      {
        return this.m_outputVars;
      }
    }
  }
}
