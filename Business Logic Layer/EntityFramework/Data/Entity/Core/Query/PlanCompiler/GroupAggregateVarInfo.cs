// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.GroupAggregateVarInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class GroupAggregateVarInfo
  {
    private readonly System.Data.Entity.Core.Query.InternalTrees.Node _definingGroupByNode;
    private HashSet<KeyValuePair<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node>> _candidateAggregateNodes;
    private readonly Var _groupAggregateVar;

    internal GroupAggregateVarInfo(System.Data.Entity.Core.Query.InternalTrees.Node defingingGroupNode, Var groupAggregateVar)
    {
      this._definingGroupByNode = defingingGroupNode;
      this._groupAggregateVar = groupAggregateVar;
    }

    internal HashSet<KeyValuePair<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node>> CandidateAggregateNodes
    {
      get
      {
        if (this._candidateAggregateNodes == null)
          this._candidateAggregateNodes = new HashSet<KeyValuePair<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node>>();
        return this._candidateAggregateNodes;
      }
    }

    internal bool HasCandidateAggregateNodes
    {
      get
      {
        if (this._candidateAggregateNodes != null)
          return this._candidateAggregateNodes.Count != 0;
        return false;
      }
    }

    internal System.Data.Entity.Core.Query.InternalTrees.Node DefiningGroupNode
    {
      get
      {
        return this._definingGroupByNode;
      }
    }

    internal Var GroupAggregateVar
    {
      get
      {
        return this._groupAggregateVar;
      }
    }
  }
}
