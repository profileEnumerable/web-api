// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.NodeInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class NodeInfo
  {
    private readonly VarVec m_externalReferences;
    protected int m_hashValue;

    internal NodeInfo(Command cmd)
    {
      this.m_externalReferences = cmd.CreateVarVec();
    }

    internal virtual void Clear()
    {
      this.m_externalReferences.Clear();
      this.m_hashValue = 0;
    }

    internal VarVec ExternalReferences
    {
      get
      {
        return this.m_externalReferences;
      }
    }

    internal int HashValue
    {
      get
      {
        return this.m_hashValue;
      }
    }

    internal static int GetHashValue(VarVec vec)
    {
      int num = 0;
      foreach (Var var in vec)
        num ^= var.GetHashCode();
      return num;
    }

    internal virtual void ComputeHashValue(Command cmd, Node n)
    {
      this.m_hashValue = 0;
      foreach (Node child in n.Children)
        this.m_hashValue ^= cmd.GetNodeInfo(child).HashValue;
      this.m_hashValue = (int) ((OpType) (this.m_hashValue << 4) ^ n.Op.OpType);
      this.m_hashValue = this.m_hashValue << 4 ^ NodeInfo.GetHashValue(this.m_externalReferences);
    }
  }
}
