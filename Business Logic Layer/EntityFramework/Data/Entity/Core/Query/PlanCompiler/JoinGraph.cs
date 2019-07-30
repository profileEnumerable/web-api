// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.JoinGraph
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class JoinGraph
  {
    private readonly Command m_command;
    private readonly AugmentedJoinNode m_root;
    private readonly List<AugmentedNode> m_vertexes;
    private readonly Dictionary<Table, AugmentedTableNode> m_tableVertexMap;
    private VarMap m_varMap;
    private readonly Dictionary<Var, VarVec> m_reverseVarMap;
    private readonly Dictionary<Var, AugmentedTableNode> m_varToDefiningNodeMap;
    private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node> m_processedNodes;
    private bool m_modifiedGraph;
    private readonly ConstraintManager m_constraintManager;
    private readonly VarRefManager m_varRefManager;

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal JoinGraph(
      Command command,
      ConstraintManager constraintManager,
      VarRefManager varRefManager,
      System.Data.Entity.Core.Query.InternalTrees.Node joinNode)
    {
      this.m_command = command;
      this.m_constraintManager = constraintManager;
      this.m_varRefManager = varRefManager;
      this.m_vertexes = new List<AugmentedNode>();
      this.m_tableVertexMap = new Dictionary<Table, AugmentedTableNode>();
      this.m_varMap = new VarMap();
      this.m_reverseVarMap = new Dictionary<Var, VarVec>();
      this.m_varToDefiningNodeMap = new Dictionary<Var, AugmentedTableNode>();
      this.m_processedNodes = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node>();
      this.m_root = this.BuildAugmentedNodeTree(joinNode) as AugmentedJoinNode;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this.m_root != null, "The root isn't a join?");
      this.BuildJoinEdges(this.m_root, this.m_root.Id);
    }

    internal System.Data.Entity.Core.Query.InternalTrees.Node DoJoinElimination(
      out VarMap varMap,
      out Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node> processedNodes)
    {
      this.TryTurnLeftOuterJoinsIntoInnerJoins();
      this.GenerateTransitiveEdges();
      this.EliminateSelfJoins();
      this.EliminateParentChildJoins();
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.BuildNodeTree();
      varMap = this.m_varMap;
      processedNodes = this.m_processedNodes;
      return node;
    }

    private VarVec GetColumnVars(VarVec varVec)
    {
      VarVec varVec1 = this.m_command.CreateVarVec();
      foreach (Var v in varVec)
      {
        if (v.VarType == VarType.Column)
          varVec1.Set(v);
      }
      return varVec1;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "columnVar")]
    private static void GetColumnVars(List<ColumnVar> columnVars, IEnumerable<Var> vec)
    {
      foreach (Var var in vec)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(var.VarType == VarType.Column, "Expected a columnVar. Found " + (object) var.VarType);
        columnVars.Add((ColumnVar) var);
      }
    }

    private void SplitPredicate(
      System.Data.Entity.Core.Query.InternalTrees.Node joinNode,
      out List<ColumnVar> leftVars,
      out List<ColumnVar> rightVars,
      out System.Data.Entity.Core.Query.InternalTrees.Node otherPredicateNode)
    {
      leftVars = new List<ColumnVar>();
      rightVars = new List<ColumnVar>();
      otherPredicateNode = joinNode.Child2;
      if (joinNode.Op.OpType == OpType.FullOuterJoin)
        return;
      Predicate predicate = new Predicate(this.m_command, joinNode.Child2);
      ExtendedNodeInfo extendedNodeInfo1 = this.m_command.GetExtendedNodeInfo(joinNode.Child0);
      ExtendedNodeInfo extendedNodeInfo2 = this.m_command.GetExtendedNodeInfo(joinNode.Child1);
      VarVec columnVars1 = this.GetColumnVars(extendedNodeInfo1.Definitions);
      VarVec columnVars2 = this.GetColumnVars(extendedNodeInfo2.Definitions);
      List<Var> leftTableEquiJoinColumns;
      List<Var> rightTableEquiJoinColumns;
      Predicate otherPredicates;
      predicate.GetEquiJoinPredicates(columnVars1, columnVars2, out leftTableEquiJoinColumns, out rightTableEquiJoinColumns, out otherPredicates);
      otherPredicateNode = otherPredicates.BuildAndTree();
      JoinGraph.GetColumnVars(leftVars, (IEnumerable<Var>) leftTableEquiJoinColumns);
      JoinGraph.GetColumnVars(rightVars, (IEnumerable<Var>) rightTableEquiJoinColumns);
    }

    private AugmentedNode BuildAugmentedNodeTree(System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      AugmentedNode augmentedNode;
      switch (node.Op.OpType)
      {
        case OpType.ScanTable:
          this.m_processedNodes[node] = node;
          ScanTableOp op = (ScanTableOp) node.Op;
          augmentedNode = (AugmentedNode) new AugmentedTableNode(this.m_vertexes.Count, node);
          this.m_tableVertexMap[op.Table] = (AugmentedTableNode) augmentedNode;
          break;
        case OpType.InnerJoin:
        case OpType.LeftOuterJoin:
        case OpType.FullOuterJoin:
          this.m_processedNodes[node] = node;
          AugmentedNode leftChild = this.BuildAugmentedNodeTree(node.Child0);
          AugmentedNode rightChild = this.BuildAugmentedNodeTree(node.Child1);
          List<ColumnVar> leftVars;
          List<ColumnVar> rightVars;
          System.Data.Entity.Core.Query.InternalTrees.Node otherPredicateNode;
          this.SplitPredicate(node, out leftVars, out rightVars, out otherPredicateNode);
          this.m_varRefManager.AddChildren(node);
          augmentedNode = (AugmentedNode) new AugmentedJoinNode(this.m_vertexes.Count, node, leftChild, rightChild, leftVars, rightVars, otherPredicateNode);
          break;
        case OpType.CrossJoin:
          this.m_processedNodes[node] = node;
          List<AugmentedNode> children = new List<AugmentedNode>();
          foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in node.Children)
            children.Add(this.BuildAugmentedNodeTree(child));
          augmentedNode = (AugmentedNode) new AugmentedJoinNode(this.m_vertexes.Count, node, children);
          this.m_varRefManager.AddChildren(node);
          break;
        default:
          augmentedNode = new AugmentedNode(this.m_vertexes.Count, node);
          break;
      }
      this.m_vertexes.Add(augmentedNode);
      return augmentedNode;
    }

    private bool AddJoinEdge(AugmentedJoinNode joinNode, ColumnVar leftVar, ColumnVar rightVar)
    {
      AugmentedTableNode left;
      AugmentedTableNode right;
      if (!this.m_tableVertexMap.TryGetValue(leftVar.Table, out left) || !this.m_tableVertexMap.TryGetValue(rightVar.Table, out right))
        return false;
      foreach (JoinEdge joinEdge in left.JoinEdges)
      {
        if (joinEdge.Right.Table.Equals((object) rightVar.Table))
          return joinEdge.AddCondition(joinNode, leftVar, rightVar);
      }
      JoinEdge joinEdge1 = JoinEdge.CreateJoinEdge(left, right, joinNode, leftVar, rightVar);
      left.JoinEdges.Add(joinEdge1);
      joinNode.JoinEdges.Add(joinEdge1);
      return true;
    }

    private static bool SingleTableVars(IEnumerable<ColumnVar> varList)
    {
      Table table = (Table) null;
      foreach (ColumnVar var in varList)
      {
        if (table == null)
          table = var.Table;
        else if (var.Table != table)
          return false;
      }
      return true;
    }

    private void BuildJoinEdges(AugmentedJoinNode joinNode, int maxVisibility)
    {
      OpType opType = joinNode.Node.Op.OpType;
      int maxVisibility1;
      int maxVisibility2;
      switch (opType)
      {
        case OpType.LeftOuterJoin:
          maxVisibility1 = maxVisibility;
          maxVisibility2 = joinNode.Id;
          break;
        case OpType.FullOuterJoin:
          maxVisibility1 = joinNode.Id;
          maxVisibility2 = joinNode.Id;
          break;
        case OpType.CrossJoin:
          using (List<AugmentedNode>.Enumerator enumerator = joinNode.Children.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.BuildJoinEdges(enumerator.Current, maxVisibility);
            return;
          }
        default:
          maxVisibility1 = maxVisibility;
          maxVisibility2 = maxVisibility;
          break;
      }
      this.BuildJoinEdges(joinNode.Children[0], maxVisibility1);
      this.BuildJoinEdges(joinNode.Children[1], maxVisibility2);
      if (joinNode.Node.Op.OpType == OpType.FullOuterJoin || joinNode.LeftVars.Count == 0 || opType == OpType.LeftOuterJoin && (!JoinGraph.SingleTableVars((IEnumerable<ColumnVar>) joinNode.RightVars) || !JoinGraph.SingleTableVars((IEnumerable<ColumnVar>) joinNode.LeftVars)))
        return;
      JoinKind joinKind = opType == OpType.LeftOuterJoin ? JoinKind.LeftOuter : JoinKind.Inner;
      for (int index = 0; index < joinNode.LeftVars.Count; ++index)
      {
        if (this.AddJoinEdge(joinNode, joinNode.LeftVars[index], joinNode.RightVars[index]) && joinKind == JoinKind.Inner)
          this.AddJoinEdge(joinNode, joinNode.RightVars[index], joinNode.LeftVars[index]);
      }
    }

    private void BuildJoinEdges(AugmentedNode node, int maxVisibility)
    {
      switch (node.Node.Op.OpType)
      {
        case OpType.ScanTable:
          ((AugmentedTableNode) node).LastVisibleId = maxVisibility;
          break;
        case OpType.InnerJoin:
        case OpType.LeftOuterJoin:
        case OpType.FullOuterJoin:
        case OpType.CrossJoin:
          this.BuildJoinEdges(node as AugmentedJoinNode, maxVisibility);
          break;
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static bool GenerateTransitiveEdge(JoinEdge edge1, JoinEdge edge2)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(edge1.Right == edge2.Left, "need a common table for transitive predicate generation");
      if (edge1.RestrictedElimination || edge2.RestrictedElimination || (edge2.Right == edge1.Left || edge1.JoinKind != edge2.JoinKind) || edge1.JoinKind == JoinKind.LeftOuter && (edge1.Left != edge1.Right || edge2.Left != edge2.Right) || edge1.JoinKind == JoinKind.LeftOuter && edge1.RightVars.Count != edge2.LeftVars.Count)
        return false;
      foreach (JoinEdge joinEdge in edge1.Left.JoinEdges)
      {
        if (joinEdge.Right == edge2.Right)
          return false;
      }
      IEnumerable<KeyValuePair<ColumnVar, ColumnVar>> orderedKeyValueList1 = JoinGraph.CreateOrderedKeyValueList(edge1.RightVars, edge1.LeftVars);
      IEnumerable<KeyValuePair<ColumnVar, ColumnVar>> orderedKeyValueList2 = JoinGraph.CreateOrderedKeyValueList(edge2.LeftVars, edge2.RightVars);
      IEnumerator<KeyValuePair<ColumnVar, ColumnVar>> enumerator1 = orderedKeyValueList1.GetEnumerator();
      IEnumerator<KeyValuePair<ColumnVar, ColumnVar>> enumerator2 = orderedKeyValueList2.GetEnumerator();
      List<ColumnVar> columnVarList1 = new List<ColumnVar>();
      List<ColumnVar> columnVarList2 = new List<ColumnVar>();
      bool flag = enumerator1.MoveNext() && enumerator2.MoveNext();
      while (flag)
      {
        if (enumerator1.Current.Key == enumerator2.Current.Key)
        {
          columnVarList1.Add(enumerator1.Current.Value);
          columnVarList2.Add(enumerator2.Current.Value);
          flag = enumerator1.MoveNext() && enumerator2.MoveNext();
        }
        else
        {
          if (edge1.JoinKind == JoinKind.LeftOuter)
            return false;
          flag = enumerator1.Current.Key.Id <= enumerator2.Current.Key.Id ? enumerator1.MoveNext() : enumerator2.MoveNext();
        }
      }
      JoinEdge transitiveJoinEdge1 = JoinEdge.CreateTransitiveJoinEdge(edge1.Left, edge2.Right, edge1.JoinKind, columnVarList1, columnVarList2);
      edge1.Left.JoinEdges.Add(transitiveJoinEdge1);
      if (edge1.JoinKind == JoinKind.Inner)
      {
        JoinEdge transitiveJoinEdge2 = JoinEdge.CreateTransitiveJoinEdge(edge2.Right, edge1.Left, edge1.JoinKind, columnVarList2, columnVarList1);
        edge2.Right.JoinEdges.Add(transitiveJoinEdge2);
      }
      return true;
    }

    private static IEnumerable<KeyValuePair<ColumnVar, ColumnVar>> CreateOrderedKeyValueList(
      List<ColumnVar> keyVars,
      List<ColumnVar> valueVars)
    {
      List<KeyValuePair<ColumnVar, ColumnVar>> source = new List<KeyValuePair<ColumnVar, ColumnVar>>(keyVars.Count);
      for (int index = 0; index < keyVars.Count; ++index)
        source.Add(new KeyValuePair<ColumnVar, ColumnVar>(keyVars[index], valueVars[index]));
      return (IEnumerable<KeyValuePair<ColumnVar, ColumnVar>>) source.OrderBy<KeyValuePair<ColumnVar, ColumnVar>, int>((Func<KeyValuePair<ColumnVar, ColumnVar>, int>) (kv => kv.Key.Id));
    }

    private void TryTurnLeftOuterJoinsIntoInnerJoins()
    {
      foreach (AugmentedJoinNode joinNode in this.m_vertexes.OfType<AugmentedJoinNode>().Where<AugmentedJoinNode>((Func<AugmentedJoinNode, bool>) (j =>
      {
        if (j.Node.Op.OpType == OpType.LeftOuterJoin)
          return j.JoinEdges.Count > 0;
        return false;
      })))
      {
        if (this.CanAllJoinEdgesBeTurnedIntoInnerJoins(joinNode.Children[1], (IEnumerable<JoinEdge>) joinNode.JoinEdges))
        {
          joinNode.Node.Op = (Op) this.m_command.CreateInnerJoinOp();
          this.m_modifiedGraph = true;
          List<JoinEdge> joinEdgeList = new List<JoinEdge>(joinNode.JoinEdges.Count);
          foreach (JoinEdge joinEdge1 in joinNode.JoinEdges)
          {
            joinEdge1.JoinKind = JoinKind.Inner;
            if (!JoinGraph.ContainsJoinEdgeForTable((IEnumerable<JoinEdge>) joinEdge1.Right.JoinEdges, joinEdge1.Left.Table))
            {
              JoinEdge joinEdge2 = JoinEdge.CreateJoinEdge(joinEdge1.Right, joinEdge1.Left, joinNode, joinEdge1.RightVars[0], joinEdge1.LeftVars[0]);
              joinEdge1.Right.JoinEdges.Add(joinEdge2);
              joinEdgeList.Add(joinEdge2);
              for (int index = 1; index < joinEdge1.LeftVars.Count; ++index)
                joinEdge2.AddCondition(joinNode, joinEdge1.RightVars[index], joinEdge1.LeftVars[index]);
            }
          }
          joinNode.JoinEdges.AddRange((IEnumerable<JoinEdge>) joinEdgeList);
        }
      }
    }

    private static bool AreAllTableRowsPreserved(AugmentedNode root, AugmentedTableNode table)
    {
      if (root is AugmentedTableNode)
        return true;
      AugmentedNode augmentedNode = (AugmentedNode) table;
      do
      {
        AugmentedJoinNode parent = (AugmentedJoinNode) augmentedNode.Parent;
        if (parent.Node.Op.OpType != OpType.LeftOuterJoin || parent.Children[0] != augmentedNode)
          return false;
        augmentedNode = (AugmentedNode) parent;
      }
      while (augmentedNode != root);
      return true;
    }

    private static bool ContainsJoinEdgeForTable(IEnumerable<JoinEdge> joinEdges, Table table)
    {
      foreach (JoinEdge joinEdge in joinEdges)
      {
        if (joinEdge.Right.Table.Equals((object) table))
          return true;
      }
      return false;
    }

    private bool CanAllJoinEdgesBeTurnedIntoInnerJoins(
      AugmentedNode rightNode,
      IEnumerable<JoinEdge> joinEdges)
    {
      foreach (JoinEdge joinEdge in joinEdges)
      {
        if (!this.CanJoinEdgeBeTurnedIntoInnerJoin(rightNode, joinEdge))
          return false;
      }
      return true;
    }

    private bool CanJoinEdgeBeTurnedIntoInnerJoin(AugmentedNode rightNode, JoinEdge joinEdge)
    {
      if (!joinEdge.RestrictedElimination && JoinGraph.AreAllTableRowsPreserved(rightNode, joinEdge.Right))
        return this.IsConstraintPresentForTurningIntoInnerJoin(joinEdge);
      return false;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private bool IsConstraintPresentForTurningIntoInnerJoin(JoinEdge joinEdge)
    {
      List<ForeignKeyConstraint> constraints;
      if (this.m_constraintManager.IsParentChildRelationship(joinEdge.Right.Table.TableMetadata.Extent, joinEdge.Left.Table.TableMetadata.Extent, out constraints))
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(constraints != null && constraints.Count > 0, "Invalid foreign key constraints");
        foreach (ForeignKeyConstraint fkConstraint in constraints)
        {
          IList<ColumnVar> childForeignKeyVars;
          if (JoinGraph.IsJoinOnFkConstraint(fkConstraint, (IList<ColumnVar>) joinEdge.RightVars, (IList<ColumnVar>) joinEdge.LeftVars, out childForeignKeyVars) && fkConstraint.ParentKeys.Count == joinEdge.RightVars.Count && childForeignKeyVars.Where<ColumnVar>((Func<ColumnVar, bool>) (v => v.ColumnMetadata.IsNullable)).Count<ColumnVar>() == 0)
            return true;
        }
      }
      return false;
    }

    private void GenerateTransitiveEdges()
    {
      foreach (AugmentedNode vertex in this.m_vertexes)
      {
        AugmentedTableNode augmentedTableNode = vertex as AugmentedTableNode;
        if (augmentedTableNode != null)
        {
          for (int index1 = 0; index1 < augmentedTableNode.JoinEdges.Count; ++index1)
          {
            JoinEdge joinEdge1 = augmentedTableNode.JoinEdges[index1];
            int index2 = 0;
            for (AugmentedTableNode right = joinEdge1.Right; index2 < right.JoinEdges.Count; ++index2)
            {
              JoinEdge joinEdge2 = right.JoinEdges[index2];
              JoinGraph.GenerateTransitiveEdge(joinEdge1, joinEdge2);
            }
          }
        }
      }
    }

    private static bool CanBeEliminatedBasedOnLojParticipation(
      AugmentedTableNode table,
      AugmentedTableNode replacingTable)
    {
      if (replacingTable.Id < table.NewLocationId)
        return JoinGraph.CanBeMovedBasedOnLojParticipation(table, replacingTable);
      return JoinGraph.CanBeMovedBasedOnLojParticipation(replacingTable, table);
    }

    private static bool CanBeEliminatedViaStarJoinBasedOnOtherJoinParticipation(
      JoinEdge tableJoinEdge,
      JoinEdge replacingTableJoinEdge)
    {
      if (tableJoinEdge.JoinNode == null || replacingTableJoinEdge.JoinNode == null)
        return false;
      AugmentedNode leastCommonAncestor = JoinGraph.GetLeastCommonAncestor((AugmentedNode) tableJoinEdge.Right, (AugmentedNode) replacingTableJoinEdge.Right);
      if (!JoinGraph.CanGetFileredByJoins(tableJoinEdge, leastCommonAncestor, true))
        return !JoinGraph.CanGetFileredByJoins(replacingTableJoinEdge, leastCommonAncestor, false);
      return false;
    }

    private static bool CanGetFileredByJoins(
      JoinEdge joinEdge,
      AugmentedNode leastCommonAncestor,
      bool disallowAnyJoin)
    {
      AugmentedNode augmentedNode = (AugmentedNode) joinEdge.Right;
      for (AugmentedNode parent = augmentedNode.Parent; parent != null && augmentedNode != leastCommonAncestor; parent = augmentedNode.Parent)
      {
        if (parent.Node != joinEdge.JoinNode.Node && (disallowAnyJoin || parent.Node.Op.OpType != OpType.LeftOuterJoin || parent.Children[0] != augmentedNode))
          return true;
        augmentedNode = augmentedNode.Parent;
      }
      return false;
    }

    private static bool CanBeMovedBasedOnLojParticipation(
      AugmentedTableNode table,
      AugmentedTableNode replacingTable)
    {
      AugmentedNode leastCommonAncestor = JoinGraph.GetLeastCommonAncestor((AugmentedNode) table, (AugmentedNode) replacingTable);
      for (AugmentedNode augmentedNode = (AugmentedNode) table; augmentedNode.Parent != null && augmentedNode != leastCommonAncestor; augmentedNode = augmentedNode.Parent)
      {
        if (augmentedNode.Parent.Node.Op.OpType == OpType.LeftOuterJoin && augmentedNode.Parent.Children[0] == augmentedNode)
          return false;
      }
      return true;
    }

    private static AugmentedNode GetLeastCommonAncestor(
      AugmentedNode node1,
      AugmentedNode node2)
    {
      if (node1.Id == node2.Id)
        return node1;
      AugmentedNode augmentedNode1;
      AugmentedNode augmentedNode2;
      if (node1.Id < node2.Id)
      {
        augmentedNode1 = node1;
        augmentedNode2 = node2;
      }
      else
      {
        augmentedNode1 = node2;
        augmentedNode2 = node1;
      }
      while (augmentedNode1.Id < augmentedNode2.Id)
        augmentedNode1 = augmentedNode1.Parent;
      return augmentedNode1;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "vars")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void MarkTableAsEliminated<T>(
      AugmentedTableNode tableNode,
      AugmentedTableNode replacementNode,
      List<T> tableVars,
      List<T> replacementVars)
      where T : Var
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(tableVars != null && replacementVars != null, "null vars");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(tableVars.Count == replacementVars.Count, "var count mismatch");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(tableVars.Count > 0, "no vars in the table ?");
      this.m_modifiedGraph = true;
      if (tableNode.Id < replacementNode.NewLocationId)
      {
        tableNode.ReplacementTable = replacementNode;
        replacementNode.NewLocationId = tableNode.Id;
      }
      else
        tableNode.ReplacementTable = (AugmentedTableNode) null;
      for (int index = 0; index < tableVars.Count; ++index)
      {
        if (tableNode.Table.ReferencedColumns.IsSet((Var) tableVars[index]))
        {
          this.m_varMap[(Var) tableVars[index]] = (Var) replacementVars[index];
          this.AddReverseMapping((Var) replacementVars[index], (Var) tableVars[index]);
          replacementNode.Table.ReferencedColumns.Set((Var) replacementVars[index]);
        }
      }
      foreach (Var referencedColumn in replacementNode.Table.ReferencedColumns)
        this.m_varToDefiningNodeMap[referencedColumn] = replacementNode;
    }

    private void AddReverseMapping(Var replacingVar, Var replacedVar)
    {
      VarVec other;
      if (this.m_reverseVarMap.TryGetValue(replacedVar, out other))
        this.m_reverseVarMap.Remove(replacedVar);
      VarVec varVec;
      if (!this.m_reverseVarMap.TryGetValue(replacingVar, out varVec))
      {
        varVec = other == null ? this.m_command.CreateVarVec() : other;
        this.m_reverseVarMap[replacingVar] = varVec;
      }
      else if (other != null)
        varVec.Or(other);
      varVec.Set(replacedVar);
    }

    private void EliminateSelfJoinedTable(
      AugmentedTableNode tableNode,
      AugmentedTableNode replacementNode)
    {
      this.MarkTableAsEliminated<Var>(tableNode, replacementNode, (List<Var>) tableNode.Table.Columns, (List<Var>) replacementNode.Table.Columns);
    }

    private void EliminateStarSelfJoin(List<JoinEdge> joinEdges)
    {
      List<List<JoinEdge>> source = new List<List<JoinEdge>>();
      foreach (JoinEdge joinEdge in joinEdges)
      {
        bool flag = false;
        foreach (List<JoinEdge> joinEdgeList in source)
        {
          if (JoinGraph.AreMatchingForStarSelfJoinElimination(joinEdgeList[0], joinEdge))
          {
            joinEdgeList.Add(joinEdge);
            flag = true;
            break;
          }
        }
        if (!flag && this.QualifiesForStarSelfJoinGroup(joinEdge))
          source.Add(new List<JoinEdge>() { joinEdge });
      }
      foreach (List<JoinEdge> joinEdgeList in source.Where<List<JoinEdge>>((Func<List<JoinEdge>, bool>) (l => l.Count > 1)))
      {
        JoinEdge replacingTableJoinEdge = joinEdgeList[0];
        foreach (JoinEdge joinEdge in joinEdgeList)
        {
          if (replacingTableJoinEdge.Right.Id > joinEdge.Right.Id)
            replacingTableJoinEdge = joinEdge;
        }
        foreach (JoinEdge tableJoinEdge in joinEdgeList)
        {
          if (tableJoinEdge != replacingTableJoinEdge && JoinGraph.CanBeEliminatedViaStarJoinBasedOnOtherJoinParticipation(tableJoinEdge, replacingTableJoinEdge))
            this.EliminateSelfJoinedTable(tableJoinEdge.Right, replacingTableJoinEdge.Right);
        }
      }
    }

    private static bool AreMatchingForStarSelfJoinElimination(JoinEdge edge1, JoinEdge edge2)
    {
      if (edge2.LeftVars.Count != edge1.LeftVars.Count || edge2.JoinKind != edge1.JoinKind)
        return false;
      for (int index = 0; index < edge2.LeftVars.Count; ++index)
      {
        if (!edge2.LeftVars[index].Equals((object) edge1.LeftVars[index]) || !edge2.RightVars[index].ColumnMetadata.Name.Equals(edge1.RightVars[index].ColumnMetadata.Name))
          return false;
      }
      return JoinGraph.MatchOtherPredicates(edge1, edge2);
    }

    private static bool MatchOtherPredicates(JoinEdge edge1, JoinEdge edge2)
    {
      if (edge1.JoinNode == null)
        return edge2.JoinNode == null;
      if (edge2.JoinNode == null)
        return false;
      if (edge1.JoinNode.OtherPredicate == null)
        return edge2.JoinNode.OtherPredicate == null;
      if (edge2.JoinNode.OtherPredicate == null)
        return false;
      return JoinGraph.MatchOtherPredicates(edge1.JoinNode.OtherPredicate, edge2.JoinNode.OtherPredicate);
    }

    private static bool MatchOtherPredicates(System.Data.Entity.Core.Query.InternalTrees.Node x, System.Data.Entity.Core.Query.InternalTrees.Node y)
    {
      if (x.Children.Count != y.Children.Count)
        return false;
      if (x.Op.IsEquivalent(y.Op))
        return !x.Children.Where<System.Data.Entity.Core.Query.InternalTrees.Node>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, int, bool>) ((t, i) => !JoinGraph.MatchOtherPredicates(t, y.Children[i]))).Any<System.Data.Entity.Core.Query.InternalTrees.Node>();
      VarRefOp op1 = x.Op as VarRefOp;
      if (op1 == null)
        return false;
      VarRefOp op2 = y.Op as VarRefOp;
      if (op2 == null)
        return false;
      ColumnVar var1 = op1.Var as ColumnVar;
      if (var1 == null)
        return false;
      ColumnVar var2 = op2.Var as ColumnVar;
      if (var2 == null)
        return false;
      return var1.ColumnMetadata.Name.Equals(var2.ColumnMetadata.Name);
    }

    private bool QualifiesForStarSelfJoinGroup(JoinEdge joinEdge)
    {
      VarVec varVec = this.m_command.CreateVarVec(joinEdge.Right.Table.Keys);
      foreach (Var rightVar in joinEdge.RightVars)
      {
        if (joinEdge.JoinKind == JoinKind.LeftOuter && !varVec.IsSet(rightVar))
          return false;
        varVec.Clear(rightVar);
      }
      if (!varVec.IsEmpty)
        return false;
      if (joinEdge.JoinNode != null && joinEdge.JoinNode.OtherPredicate != null)
        return JoinGraph.QualifiesForStarSelfJoinGroup(joinEdge.JoinNode.OtherPredicate, this.m_command.GetExtendedNodeInfo(joinEdge.Right.Node).Definitions);
      return true;
    }

    private static bool QualifiesForStarSelfJoinGroup(
      System.Data.Entity.Core.Query.InternalTrees.Node otherPredicateNode,
      VarVec rightTableColumnVars)
    {
      VarRefOp op = otherPredicateNode.Op as VarRefOp;
      if (op == null)
        return true;
      ColumnVar var = op.Var as ColumnVar;
      if (var == null)
        return true;
      if (rightTableColumnVars.IsSet((Var) var))
        return otherPredicateNode.Children.All<System.Data.Entity.Core.Query.InternalTrees.Node>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, bool>) (node => JoinGraph.QualifiesForStarSelfJoinGroup(node, rightTableColumnVars)));
      return false;
    }

    private void EliminateStarSelfJoins(AugmentedTableNode tableNode)
    {
      Dictionary<EntitySetBase, List<JoinEdge>> dictionary = new Dictionary<EntitySetBase, List<JoinEdge>>();
      foreach (JoinEdge joinEdge in tableNode.JoinEdges)
      {
        if (!joinEdge.IsEliminated)
        {
          List<JoinEdge> joinEdgeList;
          if (!dictionary.TryGetValue(joinEdge.Right.Table.TableMetadata.Extent, out joinEdgeList))
          {
            joinEdgeList = new List<JoinEdge>();
            dictionary[joinEdge.Right.Table.TableMetadata.Extent] = joinEdgeList;
          }
          joinEdgeList.Add(joinEdge);
        }
      }
      foreach (KeyValuePair<EntitySetBase, List<JoinEdge>> keyValuePair in dictionary)
      {
        if (keyValuePair.Value.Count > 1)
          this.EliminateStarSelfJoin(keyValuePair.Value);
      }
    }

    private bool EliminateSelfJoin(JoinEdge joinEdge)
    {
      if (joinEdge.RestrictedElimination || joinEdge.IsEliminated || !joinEdge.Left.Table.TableMetadata.Extent.Equals((object) joinEdge.Right.Table.TableMetadata.Extent))
        return false;
      for (int index = 0; index < joinEdge.LeftVars.Count; ++index)
      {
        if (!joinEdge.LeftVars[index].ColumnMetadata.Name.Equals(joinEdge.RightVars[index].ColumnMetadata.Name))
          return false;
      }
      VarVec varVec = this.m_command.CreateVarVec(joinEdge.Left.Table.Keys);
      foreach (Var leftVar in joinEdge.LeftVars)
      {
        if (joinEdge.JoinKind == JoinKind.LeftOuter && !varVec.IsSet(leftVar))
          return false;
        varVec.Clear(leftVar);
      }
      if (!varVec.IsEmpty || !JoinGraph.CanBeEliminatedBasedOnLojParticipation(joinEdge.Right, joinEdge.Left))
        return false;
      this.EliminateSelfJoinedTable(joinEdge.Right, joinEdge.Left);
      return true;
    }

    private void EliminateSelfJoins(AugmentedTableNode tableNode)
    {
      if (tableNode.IsEliminated)
        return;
      foreach (JoinEdge joinEdge in tableNode.JoinEdges)
        this.EliminateSelfJoin(joinEdge);
    }

    private void EliminateSelfJoins()
    {
      foreach (AugmentedNode vertex in this.m_vertexes)
      {
        AugmentedTableNode tableNode = vertex as AugmentedTableNode;
        if (tableNode != null)
        {
          this.EliminateSelfJoins(tableNode);
          this.EliminateStarSelfJoins(tableNode);
        }
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void EliminateLeftTable(JoinEdge joinEdge)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(joinEdge.JoinKind == JoinKind.Inner, "Expected inner join");
      this.MarkTableAsEliminated<ColumnVar>(joinEdge.Left, joinEdge.Right, joinEdge.LeftVars, joinEdge.RightVars);
      if (joinEdge.Right.NullableColumns == null)
        joinEdge.Right.NullableColumns = this.m_command.CreateVarVec();
      foreach (ColumnVar rightVar in joinEdge.RightVars)
      {
        if (rightVar.ColumnMetadata.IsNullable)
          joinEdge.Right.NullableColumns.Set((Var) rightVar);
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void EliminateRightTable(JoinEdge joinEdge)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(joinEdge.JoinKind == JoinKind.LeftOuter, "Expected left-outer-join");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert((joinEdge.Left.Id < joinEdge.Right.Id ? 1 : 0) != 0, "(left-id, right-id) = (" + (object) joinEdge.Left.Id + "," + (object) joinEdge.Right.Id + ")");
      this.MarkTableAsEliminated<ColumnVar>(joinEdge.Right, joinEdge.Left, joinEdge.RightVars, joinEdge.LeftVars);
    }

    private static bool HasNonKeyReferences(Table table)
    {
      return !table.Keys.Subsumes(table.ReferencedColumns);
    }

    private bool RightTableHasKeyReferences(JoinEdge joinEdge)
    {
      if (joinEdge.JoinNode == null)
        return true;
      VarVec keys = (VarVec) null;
      foreach (Var key in joinEdge.Right.Table.Keys)
      {
        VarVec other;
        if (this.m_reverseVarMap.TryGetValue(key, out other))
        {
          if (keys == null)
            keys = joinEdge.Right.Table.Keys.Clone();
          keys.Or(other);
        }
      }
      if (keys == null)
        keys = joinEdge.Right.Table.Keys;
      return this.m_varRefManager.HasKeyReferences(keys, joinEdge.Right.Node, joinEdge.JoinNode.Node);
    }

    private bool TryEliminateParentChildJoin(JoinEdge joinEdge, ForeignKeyConstraint fkConstraint)
    {
      IList<ColumnVar> childForeignKeyVars;
      if (joinEdge.JoinKind == JoinKind.LeftOuter && fkConstraint.ChildMultiplicity == RelationshipMultiplicity.Many || !JoinGraph.IsJoinOnFkConstraint(fkConstraint, (IList<ColumnVar>) joinEdge.LeftVars, (IList<ColumnVar>) joinEdge.RightVars, out childForeignKeyVars))
        return false;
      if (joinEdge.JoinKind != JoinKind.Inner)
        return this.TryEliminateRightTable(joinEdge, fkConstraint.ChildKeys.Count, fkConstraint.ChildMultiplicity == RelationshipMultiplicity.One);
      if (JoinGraph.HasNonKeyReferences(joinEdge.Left.Table) || !JoinGraph.CanBeEliminatedBasedOnLojParticipation(joinEdge.Right, joinEdge.Left))
        return false;
      this.EliminateLeftTable(joinEdge);
      return true;
    }

    private static bool IsJoinOnFkConstraint(
      ForeignKeyConstraint fkConstraint,
      IList<ColumnVar> parentVars,
      IList<ColumnVar> childVars,
      out IList<ColumnVar> childForeignKeyVars)
    {
      childForeignKeyVars = (IList<ColumnVar>) new List<ColumnVar>(fkConstraint.ChildKeys.Count);
      foreach (string parentKey in fkConstraint.ParentKeys)
      {
        bool flag = false;
        foreach (ColumnVar parentVar in (IEnumerable<ColumnVar>) parentVars)
        {
          if (parentVar.ColumnMetadata.Name.Equals(parentKey))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return false;
      }
      foreach (string childKey in fkConstraint.ChildKeys)
      {
        bool flag = false;
        for (int index = 0; index < parentVars.Count; ++index)
        {
          ColumnVar childVar = childVars[index];
          if (childVar.ColumnMetadata.Name.Equals(childKey))
          {
            childForeignKeyVars.Add(childVar);
            flag = true;
            ColumnVar parentVar = parentVars[index];
            string parentPropertyName;
            if (!fkConstraint.GetParentProperty(childVar.ColumnMetadata.Name, out parentPropertyName) || !parentPropertyName.Equals(parentVar.ColumnMetadata.Name))
              return false;
            break;
          }
        }
        if (!flag)
          return false;
      }
      return true;
    }

    private bool TryEliminateChildParentJoin(JoinEdge joinEdge, ForeignKeyConstraint fkConstraint)
    {
      IList<ColumnVar> childForeignKeyVars;
      if (!JoinGraph.IsJoinOnFkConstraint(fkConstraint, (IList<ColumnVar>) joinEdge.RightVars, (IList<ColumnVar>) joinEdge.LeftVars, out childForeignKeyVars) || childForeignKeyVars.Count > 1 && childForeignKeyVars.Where<ColumnVar>((Func<ColumnVar, bool>) (v => v.ColumnMetadata.IsNullable)).Count<ColumnVar>() > 0)
        return false;
      return this.TryEliminateRightTable(joinEdge, fkConstraint.ParentKeys.Count, true);
    }

    private bool TryEliminateRightTable(
      JoinEdge joinEdge,
      int fkConstraintKeyCount,
      bool allowRefsForJoinedOnFkOnly)
    {
      if (JoinGraph.HasNonKeyReferences(joinEdge.Right.Table) || (!allowRefsForJoinedOnFkOnly || joinEdge.RightVars.Count != fkConstraintKeyCount) && this.RightTableHasKeyReferences(joinEdge) || !JoinGraph.CanBeEliminatedBasedOnLojParticipation(joinEdge.Right, joinEdge.Left))
        return false;
      this.EliminateRightTable(joinEdge);
      return true;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void EliminateParentChildJoin(JoinEdge joinEdge)
    {
      if (joinEdge.RestrictedElimination)
        return;
      List<ForeignKeyConstraint> constraints;
      if (this.m_constraintManager.IsParentChildRelationship(joinEdge.Left.Table.TableMetadata.Extent, joinEdge.Right.Table.TableMetadata.Extent, out constraints))
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(constraints != null && constraints.Count > 0, "Invalid foreign key constraints");
        foreach (ForeignKeyConstraint fkConstraint in constraints)
        {
          if (this.TryEliminateParentChildJoin(joinEdge, fkConstraint))
            return;
        }
      }
      if (joinEdge.JoinKind != JoinKind.LeftOuter || !this.m_constraintManager.IsParentChildRelationship(joinEdge.Right.Table.TableMetadata.Extent, joinEdge.Left.Table.TableMetadata.Extent, out constraints))
        return;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(constraints != null && constraints.Count > 0, "Invalid foreign key constraints");
      foreach (ForeignKeyConstraint fkConstraint in constraints)
      {
        if (this.TryEliminateChildParentJoin(joinEdge, fkConstraint))
          break;
      }
    }

    private void EliminateParentChildJoins(AugmentedTableNode tableNode)
    {
      foreach (JoinEdge joinEdge in tableNode.JoinEdges)
      {
        this.EliminateParentChildJoin(joinEdge);
        if (tableNode.IsEliminated)
          break;
      }
    }

    private void EliminateParentChildJoins()
    {
      foreach (AugmentedNode vertex in this.m_vertexes)
      {
        AugmentedTableNode tableNode = vertex as AugmentedTableNode;
        if (tableNode != null && !tableNode.IsEliminated)
          this.EliminateParentChildJoins(tableNode);
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node BuildNodeTree()
    {
      if (!this.m_modifiedGraph)
        return this.m_root.Node;
      VarMap varMap = new VarMap();
      foreach (KeyValuePair<Var, Var> var1 in (Dictionary<Var, Var>) this.m_varMap)
      {
        Var key;
        Var var2;
        for (key = var1.Value; this.m_varMap.TryGetValue(key, out var2); key = var2)
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(var2 != null, "null var mapping?");
        varMap[var1.Key] = key;
      }
      this.m_varMap = varMap;
      Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> predicates;
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.RebuildNodeTree(this.m_root, out predicates);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node != null, "Resulting node tree is null");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(predicates == null || predicates.Count == 0, "Leaking predicates?");
      return node;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node BuildFilterForNullableColumns(
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode,
      VarVec nonNullableColumns)
    {
      if (nonNullableColumns == null)
        return inputNode;
      VarVec varVec = nonNullableColumns.Remap((Dictionary<Var, Var>) this.m_varMap);
      if (varVec.IsEmpty)
        return inputNode;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      foreach (Var v in varVec)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = this.m_command.CreateNode((Op) this.m_command.CreateConditionalOp(OpType.Not), this.m_command.CreateNode((Op) this.m_command.CreateConditionalOp(OpType.IsNull), this.m_command.CreateNode((Op) this.m_command.CreateVarRefOp(v))));
        node1 = node1 != null ? this.m_command.CreateNode((Op) this.m_command.CreateConditionalOp(OpType.And), node1, node2) : node2;
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node1 != null, "Null predicate?");
      return this.m_command.CreateNode((Op) this.m_command.CreateFilterOp(), inputNode, node1);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node BuildFilterNode(
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode,
      System.Data.Entity.Core.Query.InternalTrees.Node predicateNode)
    {
      if (predicateNode == null)
        return inputNode;
      return this.m_command.CreateNode((Op) this.m_command.CreateFilterOp(), inputNode, predicateNode);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node RebuildPredicate(
      AugmentedJoinNode joinNode,
      out int minLocationId)
    {
      minLocationId = joinNode.Id;
      if (joinNode.OtherPredicate != null)
      {
        foreach (Var externalReference in joinNode.OtherPredicate.GetNodeInfo(this.m_command).ExternalReferences)
        {
          Var var;
          if (!this.m_varMap.TryGetValue(externalReference, out var))
            var = externalReference;
          minLocationId = this.GetLeastCommonAncestor(minLocationId, this.GetLocationId(var, minLocationId));
        }
      }
      System.Data.Entity.Core.Query.InternalTrees.Node predicate2 = joinNode.OtherPredicate;
      for (int index = 0; index < joinNode.LeftVars.Count; ++index)
      {
        Var leftVar;
        if (!this.m_varMap.TryGetValue((Var) joinNode.LeftVars[index], out leftVar))
          leftVar = (Var) joinNode.LeftVars[index];
        Var rightVar;
        if (!this.m_varMap.TryGetValue((Var) joinNode.RightVars[index], out rightVar))
          rightVar = (Var) joinNode.RightVars[index];
        if (!leftVar.Equals((object) rightVar))
        {
          minLocationId = this.GetLeastCommonAncestor(minLocationId, this.GetLocationId(leftVar, minLocationId));
          minLocationId = this.GetLeastCommonAncestor(minLocationId, this.GetLocationId(rightVar, minLocationId));
          System.Data.Entity.Core.Query.InternalTrees.Node node = this.m_command.CreateNode((Op) this.m_command.CreateComparisonOp(OpType.EQ, false), this.m_command.CreateNode((Op) this.m_command.CreateVarRefOp(leftVar)), this.m_command.CreateNode((Op) this.m_command.CreateVarRefOp(rightVar)));
          predicate2 = predicate2 == null ? node : PlanCompilerUtil.CombinePredicates(node, predicate2, this.m_command);
        }
      }
      return predicate2;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private System.Data.Entity.Core.Query.InternalTrees.Node RebuildNodeTreeForCrossJoins(
      AugmentedJoinNode joinNode)
    {
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (AugmentedNode child in joinNode.Children)
      {
        Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> predicates;
        args.Add(this.RebuildNodeTree(child, out predicates));
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(predicates == null || predicates.Count == 0, "Leaking predicates");
      }
      if (args.Count == 0)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      if (args.Count == 1)
        return args[0];
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.m_command.CreateNode((Op) this.m_command.CreateCrossJoinOp(), args);
      this.m_processedNodes[node] = node;
      return node;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node RebuildNodeTree(
      AugmentedJoinNode joinNode,
      out Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> predicates)
    {
      if (joinNode.Node.Op.OpType == OpType.CrossJoin)
      {
        predicates = (Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int>) null;
        return this.RebuildNodeTreeForCrossJoins(joinNode);
      }
      Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> predicates1;
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode1 = this.RebuildNodeTree(joinNode.Children[0], out predicates1);
      Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> predicates2;
      System.Data.Entity.Core.Query.InternalTrees.Node inputNode2 = this.RebuildNodeTree(joinNode.Children[1], out predicates2);
      int minLocationId;
      System.Data.Entity.Core.Query.InternalTrees.Node localPredicateNode;
      if (inputNode1 != null && inputNode2 == null && joinNode.Node.Op.OpType == OpType.LeftOuterJoin)
      {
        minLocationId = joinNode.Id;
        localPredicateNode = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      }
      else
        localPredicateNode = this.RebuildPredicate(joinNode, out minLocationId);
      System.Data.Entity.Core.Query.InternalTrees.Node predicateNode = this.CombinePredicateNodes(joinNode.Id, localPredicateNode, minLocationId, predicates1, predicates2, out predicates);
      if (inputNode1 == null && inputNode2 == null)
      {
        if (predicateNode == null)
          return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
        return this.BuildFilterNode(this.m_command.CreateNode((Op) this.m_command.CreateSingleRowTableOp()), predicateNode);
      }
      if (inputNode1 == null)
        return this.BuildFilterNode(inputNode2, predicateNode);
      if (inputNode2 == null)
        return this.BuildFilterNode(inputNode1, predicateNode);
      if (predicateNode == null)
        predicateNode = this.m_command.CreateNode((Op) this.m_command.CreateTrueOp());
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.m_command.CreateNode(joinNode.Node.Op, inputNode1, inputNode2, predicateNode);
      this.m_processedNodes[node] = node;
      return node;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node RebuildNodeTree(
      AugmentedTableNode tableNode)
    {
      AugmentedTableNode augmentedTableNode = tableNode;
      if (tableNode.IsMoved)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      while (augmentedTableNode.IsEliminated)
      {
        augmentedTableNode = augmentedTableNode.ReplacementTable;
        if (augmentedTableNode == null)
          return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      }
      if (augmentedTableNode.NewLocationId < tableNode.Id)
        return (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      return this.BuildFilterForNullableColumns(augmentedTableNode.Node, augmentedTableNode.NullableColumns);
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node RebuildNodeTree(
      AugmentedNode augmentedNode,
      out Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> predicates)
    {
      switch (augmentedNode.Node.Op.OpType)
      {
        case OpType.ScanTable:
          predicates = (Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int>) null;
          return this.RebuildNodeTree((AugmentedTableNode) augmentedNode);
        case OpType.InnerJoin:
        case OpType.LeftOuterJoin:
        case OpType.FullOuterJoin:
        case OpType.CrossJoin:
          return this.RebuildNodeTree((AugmentedJoinNode) augmentedNode, out predicates);
        default:
          predicates = (Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int>) null;
          return augmentedNode.Node;
      }
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CombinePredicateNodes(
      int targetNodeId,
      System.Data.Entity.Core.Query.InternalTrees.Node localPredicateNode,
      int localPredicateMinLocationId,
      Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> leftPredicates,
      Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> rightPredicates,
      out Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> outPredicates)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node result = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      outPredicates = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int>();
      if (localPredicateNode != null)
        result = this.ClassifyPredicate(targetNodeId, localPredicateNode, localPredicateMinLocationId, result, outPredicates);
      if (leftPredicates != null)
      {
        foreach (KeyValuePair<System.Data.Entity.Core.Query.InternalTrees.Node, int> leftPredicate in leftPredicates)
          result = this.ClassifyPredicate(targetNodeId, leftPredicate.Key, leftPredicate.Value, result, outPredicates);
      }
      if (rightPredicates != null)
      {
        foreach (KeyValuePair<System.Data.Entity.Core.Query.InternalTrees.Node, int> rightPredicate in rightPredicates)
          result = this.ClassifyPredicate(targetNodeId, rightPredicate.Key, rightPredicate.Value, result, outPredicates);
      }
      return result;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node ClassifyPredicate(
      int targetNodeId,
      System.Data.Entity.Core.Query.InternalTrees.Node predicateNode,
      int predicateMinLocationId,
      System.Data.Entity.Core.Query.InternalTrees.Node result,
      Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, int> outPredicates)
    {
      if (targetNodeId >= predicateMinLocationId)
        result = this.CombinePredicates(result, predicateNode);
      else
        outPredicates.Add(predicateNode, predicateMinLocationId);
      return result;
    }

    private System.Data.Entity.Core.Query.InternalTrees.Node CombinePredicates(
      System.Data.Entity.Core.Query.InternalTrees.Node node1,
      System.Data.Entity.Core.Query.InternalTrees.Node node2)
    {
      if (node1 == null)
        return node2;
      if (node2 == null)
        return node1;
      return PlanCompilerUtil.CombinePredicates(node1, node2, this.m_command);
    }

    private int GetLocationId(Var var, int defaultLocationId)
    {
      AugmentedTableNode augmentedTableNode;
      if (!this.m_varToDefiningNodeMap.TryGetValue(var, out augmentedTableNode))
        return defaultLocationId;
      if (augmentedTableNode.IsMoved)
        return augmentedTableNode.NewLocationId;
      return augmentedTableNode.Id;
    }

    private int GetLeastCommonAncestor(int nodeId1, int nodeId2)
    {
      if (nodeId1 == nodeId2)
        return nodeId1;
      AugmentedNode root = (AugmentedNode) this.m_root;
      AugmentedNode augmentedNode1 = root;
      for (AugmentedNode augmentedNode2 = root; augmentedNode1 == augmentedNode2; augmentedNode2 = JoinGraph.PickSubtree(nodeId2, root))
      {
        root = augmentedNode1;
        if (root.Id == nodeId1 || root.Id == nodeId2)
          return root.Id;
        augmentedNode1 = JoinGraph.PickSubtree(nodeId1, root);
      }
      return root.Id;
    }

    private static AugmentedNode PickSubtree(int nodeId, AugmentedNode root)
    {
      AugmentedNode child = root.Children[0];
      for (int index = 1; child.Id < nodeId && index < root.Children.Count; ++index)
        child = root.Children[index];
      return child;
    }
  }
}
