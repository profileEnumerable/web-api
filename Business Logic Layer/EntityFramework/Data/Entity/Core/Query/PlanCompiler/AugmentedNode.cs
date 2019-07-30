// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.AugmentedNode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class AugmentedNode
  {
    private readonly List<JoinEdge> m_joinEdges = new List<JoinEdge>();
    private readonly int m_id;
    private readonly System.Data.Entity.Core.Query.InternalTrees.Node m_node;
    protected AugmentedNode m_parent;
    private readonly List<AugmentedNode> m_children;

    internal AugmentedNode(int id, System.Data.Entity.Core.Query.InternalTrees.Node node)
      : this(id, node, new List<AugmentedNode>())
    {
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal AugmentedNode(int id, System.Data.Entity.Core.Query.InternalTrees.Node node, List<AugmentedNode> children)
    {
      this.m_id = id;
      this.m_node = node;
      this.m_children = children;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(children != null, "null children (gasp!)");
      foreach (AugmentedNode child in this.m_children)
        child.m_parent = this;
    }

    internal int Id
    {
      get
      {
        return this.m_id;
      }
    }

    internal System.Data.Entity.Core.Query.InternalTrees.Node Node
    {
      get
      {
        return this.m_node;
      }
    }

    internal AugmentedNode Parent
    {
      get
      {
        return this.m_parent;
      }
    }

    internal List<AugmentedNode> Children
    {
      get
      {
        return this.m_children;
      }
    }

    internal List<JoinEdge> JoinEdges
    {
      get
      {
        return this.m_joinEdges;
      }
    }
  }
}
