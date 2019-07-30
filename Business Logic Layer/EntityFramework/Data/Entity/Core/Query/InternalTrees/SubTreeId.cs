// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.SubTreeId
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class SubTreeId
  {
    public Node m_subTreeRoot;
    private readonly int m_hashCode;
    private readonly Node m_parent;
    private readonly int m_childIndex;

    internal SubTreeId(RuleProcessingContext context, Node node, Node parent, int childIndex)
    {
      this.m_subTreeRoot = node;
      this.m_parent = parent;
      this.m_childIndex = childIndex;
      this.m_hashCode = context.GetHashCode(node);
    }

    public override int GetHashCode()
    {
      return this.m_hashCode;
    }

    public override bool Equals(object obj)
    {
      SubTreeId subTreeId = obj as SubTreeId;
      if (subTreeId == null || this.m_hashCode != subTreeId.m_hashCode)
        return false;
      if (subTreeId.m_subTreeRoot == this.m_subTreeRoot)
        return true;
      if (subTreeId.m_parent == this.m_parent)
        return subTreeId.m_childIndex == this.m_childIndex;
      return false;
    }
  }
}
