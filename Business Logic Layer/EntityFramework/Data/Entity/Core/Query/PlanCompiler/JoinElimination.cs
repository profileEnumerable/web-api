// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.JoinElimination
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class JoinElimination : BasicOpVisitorOfNode
  {
    private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node> m_joinGraphUnnecessaryMap = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node>();
    private readonly System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler m_compilerState;
    private readonly VarRemapper m_varRemapper;
    private bool m_treeModified;
    private readonly VarRefManager m_varRefManager;

    private Command Command
    {
      get
      {
        return this.m_compilerState.Command;
      }
    }

    private ConstraintManager ConstraintManager
    {
      get
      {
        return this.m_compilerState.ConstraintManager;
      }
    }

    private JoinElimination(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState)
    {
      this.m_compilerState = compilerState;
      this.m_varRemapper = new VarRemapper(this.m_compilerState.Command);
      this.m_varRefManager = new VarRefManager(this.m_compilerState.Command);
    }

    internal static bool Process(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState)
    {
      JoinElimination joinElimination = new JoinElimination(compilerState);
      joinElimination.Process();
      return joinElimination.m_treeModified;
    }

    private void Process()
    {
      this.Command.Root = this.VisitNode(this.Command.Root);
    }

    private bool NeedsJoinGraph(System.Data.Entity.Core.Query.InternalTrees.Node joinNode)
    {
      return !this.m_joinGraphUnnecessaryMap.ContainsKey(joinNode);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node ProcessJoinGraph(System.Data.Entity.Core.Query.InternalTrees.Node joinNode)
    {
      VarMap varMap;
      Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node> processedNodes;
      System.Data.Entity.Core.Query.InternalTrees.Node node = new JoinGraph(this.Command, this.ConstraintManager, this.m_varRefManager, joinNode).DoJoinElimination(out varMap, out processedNodes);
      foreach (KeyValuePair<Var, Var> keyValuePair in (Dictionary<Var, Var>) varMap)
        this.m_varRemapper.AddMapping(keyValuePair.Key, keyValuePair.Value);
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node key in processedNodes.Keys)
        this.m_joinGraphUnnecessaryMap[key] = key;
      return node;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node VisitDefaultForAllNodes(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      this.m_varRemapper.RemapNode(n);
      this.Command.RecomputeNodeInfo(n);
      return n;
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitDefault(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.m_varRefManager.AddChildren(n);
      return this.VisitDefaultForAllNodes(n);
    }

    protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitJoinOp(
      JoinBaseOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node joinNode)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node n;
      if (this.NeedsJoinGraph(joinNode))
      {
        n = this.ProcessJoinGraph(joinNode);
        if (n != joinNode)
          this.m_treeModified = true;
      }
      else
        n = joinNode;
      return this.VisitDefaultForAllNodes(n);
    }
  }
}
