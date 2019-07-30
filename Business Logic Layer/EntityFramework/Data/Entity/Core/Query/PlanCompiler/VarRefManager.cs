// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.VarRefManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class VarRefManager
  {
    private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node> m_nodeToParentMap;
    private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> m_nodeToSiblingNumber;
    private readonly Command m_command;

    internal VarRefManager(Command command)
    {
      this.m_nodeToParentMap = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node>();
      this.m_nodeToSiblingNumber = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int>();
      this.m_command = command;
    }

    internal void AddChildren(System.Data.Entity.Core.Query.InternalTrees.Node parent)
    {
      for (int index = 0; index < parent.Children.Count; ++index)
      {
        this.m_nodeToParentMap[parent.Children[index]] = parent;
        this.m_nodeToSiblingNumber[parent.Children[index]] = index;
      }
    }

    internal bool HasKeyReferences(VarVec keys, System.Data.Entity.Core.Query.InternalTrees.Node definingNode, System.Data.Entity.Core.Query.InternalTrees.Node targetJoinNode)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node key = definingNode;
      System.Data.Entity.Core.Query.InternalTrees.Node node;
      for (bool continueUp = true; continueUp & this.m_nodeToParentMap.TryGetValue(key, out node); key = node)
      {
        if (node != targetJoinNode)
        {
          if (VarRefManager.HasVarReferencesShallow(node, keys, this.m_nodeToSiblingNumber[key], out continueUp))
            return true;
          for (int index = this.m_nodeToSiblingNumber[key] + 1; index < node.Children.Count; ++index)
          {
            if (node.Children[index].GetNodeInfo(this.m_command).ExternalReferences.Overlaps(keys))
              return true;
          }
        }
      }
      return false;
    }

    private static bool HasVarReferencesShallow(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      VarVec vars,
      int childIndex,
      out bool continueUp)
    {
      switch (node.Op.OpType)
      {
        case OpType.Project:
          continueUp = false;
          return VarRefManager.HasVarReferences(((ProjectOp) node.Op).Outputs, vars);
        case OpType.Sort:
        case OpType.ConstrainedSort:
          continueUp = true;
          return VarRefManager.HasVarReferences(((SortBaseOp) node.Op).Keys, vars);
        case OpType.GroupBy:
          continueUp = false;
          return VarRefManager.HasVarReferences(((GroupByBaseOp) node.Op).Keys, vars);
        case OpType.UnionAll:
        case OpType.Intersect:
        case OpType.Except:
          continueUp = false;
          return VarRefManager.HasVarReferences((SetOp) node.Op, vars, childIndex);
        case OpType.Distinct:
          continueUp = false;
          return VarRefManager.HasVarReferences(((DistinctOp) node.Op).Keys, vars);
        case OpType.PhysicalProject:
          continueUp = false;
          return VarRefManager.HasVarReferences(((PhysicalProjectOp) node.Op).Outputs, vars);
        default:
          continueUp = true;
          return false;
      }
    }

    private static bool HasVarReferences(VarList listToCheck, VarVec vars)
    {
      foreach (Var var in vars)
      {
        if (listToCheck.Contains(var))
          return true;
      }
      return false;
    }

    private static bool HasVarReferences(VarVec listToCheck, VarVec vars)
    {
      return listToCheck.Overlaps(vars);
    }

    private static bool HasVarReferences(List<SortKey> listToCheck, VarVec vars)
    {
      foreach (SortKey sortKey in listToCheck)
      {
        if (vars.IsSet(sortKey.Var))
          return true;
      }
      return false;
    }

    private static bool HasVarReferences(SetOp op, VarVec vars, int index)
    {
      foreach (Var v in op.VarMap[index].Values)
      {
        if (vars.IsSet(v))
          return true;
      }
      return false;
    }
  }
}
