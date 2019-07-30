// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CellTreeSimplifier
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal class CellTreeSimplifier : InternalBase
  {
    private readonly ViewgenContext m_viewgenContext;

    private CellTreeSimplifier(ViewgenContext context)
    {
      this.m_viewgenContext = context;
    }

    internal static CellTreeNode MergeNodes(CellTreeNode rootNode)
    {
      return new CellTreeSimplifier(rootNode.ViewgenContext).SimplifyTreeByMergingNodes(rootNode);
    }

    private CellTreeNode SimplifyTreeByMergingNodes(CellTreeNode rootNode)
    {
      if (rootNode is LeafCellTreeNode)
        return rootNode;
      rootNode = this.RestructureTreeForMerges(rootNode);
      List<CellTreeNode> children = rootNode.Children;
      for (int index = 0; index < children.Count; ++index)
        children[index] = this.SimplifyTreeByMergingNodes(children[index]);
      bool flag1 = CellTreeNode.IsAssociativeOp(rootNode.OpType);
      List<CellTreeNode> cellTreeNodeList = !flag1 ? CellTreeSimplifier.GroupNonAssociativeLeafChildren(children) : CellTreeSimplifier.GroupLeafChildrenByExtent(children);
      OpCellTreeNode opCellTreeNode = new OpCellTreeNode(this.m_viewgenContext, rootNode.OpType);
      CellTreeNode node1 = (CellTreeNode) null;
      bool flag2 = false;
      foreach (CellTreeNode node2 in cellTreeNodeList)
      {
        if (node1 == null)
        {
          node1 = node2;
        }
        else
        {
          bool flag3 = false;
          if (!flag2 && node1.OpType == CellTreeOpType.Leaf && node2.OpType == CellTreeOpType.Leaf)
            flag3 = this.TryMergeCellQueries(rootNode.OpType, ref node1, node2);
          if (!flag3)
          {
            opCellTreeNode.Add(node1);
            node1 = node2;
            if (!flag1)
              flag2 = true;
          }
        }
      }
      opCellTreeNode.Add(node1);
      return opCellTreeNode.AssociativeFlatten();
    }

    private CellTreeNode RestructureTreeForMerges(CellTreeNode rootNode)
    {
      List<CellTreeNode> children = rootNode.Children;
      if (!CellTreeNode.IsAssociativeOp(rootNode.OpType) || children.Count <= 1)
        return rootNode;
      Set<LeafCellTreeNode> commonGrandChildren = CellTreeSimplifier.GetCommonGrandChildren(children);
      if (commonGrandChildren == null)
        return rootNode;
      CellTreeOpType opType = children[0].OpType;
      List<OpCellTreeNode> opCellTreeNodeList = new List<OpCellTreeNode>(children.Count);
      foreach (OpCellTreeNode opCellTreeNode1 in children)
      {
        List<LeafCellTreeNode> leafCellTreeNodeList = new List<LeafCellTreeNode>(opCellTreeNode1.Children.Count);
        foreach (LeafCellTreeNode child in opCellTreeNode1.Children)
        {
          if (!commonGrandChildren.Contains(child))
            leafCellTreeNodeList.Add(child);
        }
        OpCellTreeNode opCellTreeNode2 = new OpCellTreeNode(this.m_viewgenContext, opCellTreeNode1.OpType, Helpers.AsSuperTypeList<LeafCellTreeNode, CellTreeNode>((IEnumerable<LeafCellTreeNode>) leafCellTreeNodeList));
        opCellTreeNodeList.Add(opCellTreeNode2);
      }
      CellTreeNode cellTreeNode1 = (CellTreeNode) new OpCellTreeNode(this.m_viewgenContext, rootNode.OpType, Helpers.AsSuperTypeList<OpCellTreeNode, CellTreeNode>((IEnumerable<OpCellTreeNode>) opCellTreeNodeList));
      CellTreeNode cellTreeNode2 = (CellTreeNode) new OpCellTreeNode(this.m_viewgenContext, opType, Helpers.AsSuperTypeList<LeafCellTreeNode, CellTreeNode>((IEnumerable<LeafCellTreeNode>) commonGrandChildren));
      return new OpCellTreeNode(this.m_viewgenContext, opType, new CellTreeNode[2]
      {
        cellTreeNode2,
        cellTreeNode1
      }).AssociativeFlatten();
    }

    private static Set<LeafCellTreeNode> GetCommonGrandChildren(
      List<CellTreeNode> nodes)
    {
      Set<LeafCellTreeNode> set = (Set<LeafCellTreeNode>) null;
      CellTreeOpType cellTreeOpType = CellTreeOpType.Leaf;
      foreach (CellTreeNode node in nodes)
      {
        OpCellTreeNode opCellTreeNode = node as OpCellTreeNode;
        if (opCellTreeNode == null)
          return (Set<LeafCellTreeNode>) null;
        if (cellTreeOpType == CellTreeOpType.Leaf)
          cellTreeOpType = opCellTreeNode.OpType;
        else if (!CellTreeNode.IsAssociativeOp(opCellTreeNode.OpType) || cellTreeOpType != opCellTreeNode.OpType)
          return (Set<LeafCellTreeNode>) null;
        Set<LeafCellTreeNode> other = new Set<LeafCellTreeNode>(LeafCellTreeNode.EqualityComparer);
        foreach (CellTreeNode child in opCellTreeNode.Children)
        {
          LeafCellTreeNode element = child as LeafCellTreeNode;
          if (element == null)
            return (Set<LeafCellTreeNode>) null;
          other.Add(element);
        }
        if (set == null)
          set = other;
        else
          set.Intersect(other);
      }
      if (set.Count == 0)
        return (Set<LeafCellTreeNode>) null;
      return set;
    }

    private static List<CellTreeNode> GroupLeafChildrenByExtent(
      List<CellTreeNode> nodes)
    {
      KeyToListMap<EntitySetBase, CellTreeNode> keyToListMap = new KeyToListMap<EntitySetBase, CellTreeNode>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
      List<CellTreeNode> cellTreeNodeList = new List<CellTreeNode>();
      foreach (CellTreeNode node in nodes)
      {
        LeafCellTreeNode leafCellTreeNode = node as LeafCellTreeNode;
        if (leafCellTreeNode != null)
          keyToListMap.Add(leafCellTreeNode.LeftCellWrapper.RightCellQuery.Extent, (CellTreeNode) leafCellTreeNode);
        else
          cellTreeNodeList.Add(node);
      }
      cellTreeNodeList.AddRange(keyToListMap.AllValues);
      return cellTreeNodeList;
    }

    private static List<CellTreeNode> GroupNonAssociativeLeafChildren(
      List<CellTreeNode> nodes)
    {
      KeyToListMap<EntitySetBase, CellTreeNode> keyToListMap = new KeyToListMap<EntitySetBase, CellTreeNode>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
      List<CellTreeNode> cellTreeNodeList1 = new List<CellTreeNode>();
      List<CellTreeNode> cellTreeNodeList2 = new List<CellTreeNode>();
      cellTreeNodeList1.Add(nodes[0]);
      for (int index = 1; index < nodes.Count; ++index)
      {
        CellTreeNode node = nodes[index];
        LeafCellTreeNode leafCellTreeNode = node as LeafCellTreeNode;
        if (leafCellTreeNode != null)
          keyToListMap.Add(leafCellTreeNode.LeftCellWrapper.RightCellQuery.Extent, (CellTreeNode) leafCellTreeNode);
        else
          cellTreeNodeList2.Add(node);
      }
      LeafCellTreeNode node1 = nodes[0] as LeafCellTreeNode;
      if (node1 != null)
      {
        EntitySetBase extent = node1.LeftCellWrapper.RightCellQuery.Extent;
        if (keyToListMap.ContainsKey(extent))
        {
          cellTreeNodeList1.AddRange((IEnumerable<CellTreeNode>) keyToListMap.ListForKey(extent));
          keyToListMap.RemoveKey(extent);
        }
      }
      cellTreeNodeList1.AddRange(keyToListMap.AllValues);
      cellTreeNodeList1.AddRange((IEnumerable<CellTreeNode>) cellTreeNodeList2);
      return cellTreeNodeList1;
    }

    private bool TryMergeCellQueries(
      CellTreeOpType opType,
      ref CellTreeNode node1,
      CellTreeNode node2)
    {
      LeafCellTreeNode leafCellTreeNode1 = node1 as LeafCellTreeNode;
      LeafCellTreeNode leafCellTreeNode2 = node2 as LeafCellTreeNode;
      CellQuery mergedQuery1;
      CellQuery mergedQuery2;
      if (!CellTreeSimplifier.TryMergeTwoCellQueries(leafCellTreeNode1.LeftCellWrapper.RightCellQuery, leafCellTreeNode2.LeftCellWrapper.RightCellQuery, opType, out mergedQuery1) || !CellTreeSimplifier.TryMergeTwoCellQueries(leafCellTreeNode1.LeftCellWrapper.LeftCellQuery, leafCellTreeNode2.LeftCellWrapper.LeftCellQuery, opType, out mergedQuery2))
        return false;
      OpCellTreeNode opCellTreeNode = new OpCellTreeNode(this.m_viewgenContext, opType);
      opCellTreeNode.Add(node1);
      opCellTreeNode.Add(node2);
      if (opType != CellTreeOpType.FOJ)
        ;
      LeftCellWrapper cellWrapper = new LeftCellWrapper(this.m_viewgenContext.ViewTarget, opCellTreeNode.Attributes, opCellTreeNode.LeftFragmentQuery, mergedQuery2, mergedQuery1, this.m_viewgenContext.MemberMaps, leafCellTreeNode1.LeftCellWrapper.Cells.Concat<Cell>(leafCellTreeNode2.LeftCellWrapper.Cells));
      node1 = (CellTreeNode) new LeafCellTreeNode(this.m_viewgenContext, cellWrapper, opCellTreeNode.RightFragmentQuery);
      return true;
    }

    internal static bool TryMergeTwoCellQueries(
      CellQuery query1,
      CellQuery query2,
      CellTreeOpType opType,
      out CellQuery mergedQuery)
    {
      mergedQuery = (CellQuery) null;
      BoolExpression boolExpression1 = (BoolExpression) null;
      BoolExpression boolExpression2 = (BoolExpression) null;
      switch (opType)
      {
        case CellTreeOpType.Union:
        case CellTreeOpType.FOJ:
          boolExpression1 = BoolExpression.True;
          boolExpression2 = BoolExpression.True;
          break;
        case CellTreeOpType.LOJ:
        case CellTreeOpType.LASJ:
          boolExpression2 = BoolExpression.True;
          break;
      }
      Dictionary<MemberPath, MemberPath> remap = new Dictionary<MemberPath, MemberPath>(MemberPath.EqualityComparer);
      if (!query1.Extent.Equals((object) query2.Extent))
        return false;
      MemberPath extentMemberPath = query1.SourceExtentMemberPath;
      BoolExpression and1 = BoolExpression.True;
      BoolExpression and2 = BoolExpression.True;
      BoolExpression boolExpression3 = (BoolExpression) null;
      switch (opType)
      {
        case CellTreeOpType.Union:
        case CellTreeOpType.FOJ:
          and1 = BoolExpression.CreateAnd(query1.WhereClause, boolExpression1);
          and2 = BoolExpression.CreateAnd(query2.WhereClause, boolExpression2);
          boolExpression3 = BoolExpression.CreateOr(BoolExpression.CreateAnd(query1.WhereClause, boolExpression1), BoolExpression.CreateAnd(query2.WhereClause, boolExpression2));
          break;
        case CellTreeOpType.LOJ:
          and2 = BoolExpression.CreateAnd(query2.WhereClause, boolExpression2);
          boolExpression3 = query1.WhereClause;
          break;
        case CellTreeOpType.IJ:
          boolExpression3 = BoolExpression.CreateAnd(query1.WhereClause, query2.WhereClause);
          break;
        case CellTreeOpType.LASJ:
          and2 = BoolExpression.CreateAnd(query2.WhereClause, boolExpression2);
          boolExpression3 = BoolExpression.CreateAnd(query1.WhereClause, BoolExpression.CreateNot(and2));
          break;
      }
      List<BoolExpression> boolExprs = CellTreeSimplifier.MergeBoolExpressions(query1, query2, and1, and2, opType);
      ProjectedSlot[] result;
      if (!ProjectedSlot.TryMergeRemapSlots(query1.ProjectedSlots, query2.ProjectedSlots, out result))
        return false;
      BoolExpression whereClause = boolExpression3.RemapBool(remap);
      CellQuery.SelectDistinct elimDupl = CellTreeSimplifier.MergeDupl(query1.SelectDistinctFlag, query2.SelectDistinctFlag);
      whereClause.ExpensiveSimplify();
      mergedQuery = new CellQuery(result, whereClause, boolExprs, elimDupl, extentMemberPath);
      return true;
    }

    private static CellQuery.SelectDistinct MergeDupl(
      CellQuery.SelectDistinct d1,
      CellQuery.SelectDistinct d2)
    {
      return d1 == CellQuery.SelectDistinct.Yes || d2 == CellQuery.SelectDistinct.Yes ? CellQuery.SelectDistinct.Yes : CellQuery.SelectDistinct.No;
    }

    private static List<BoolExpression> MergeBoolExpressions(
      CellQuery query1,
      CellQuery query2,
      BoolExpression conjunct1,
      BoolExpression conjunct2,
      CellTreeOpType opType)
    {
      List<BoolExpression> bools1 = query1.BoolVars;
      List<BoolExpression> bools2 = query2.BoolVars;
      if (!conjunct1.IsTrue)
        bools1 = BoolExpression.AddConjunctionToBools(bools1, conjunct1);
      if (!conjunct2.IsTrue)
        bools2 = BoolExpression.AddConjunctionToBools(bools2, conjunct2);
      List<BoolExpression> boolExpressionList = new List<BoolExpression>();
      for (int index = 0; index < bools1.Count; ++index)
      {
        BoolExpression boolExpression = (BoolExpression) null;
        if (bools1[index] == null)
          boolExpression = bools2[index];
        else if (bools2[index] == null)
        {
          boolExpression = bools1[index];
        }
        else
        {
          switch (opType)
          {
            case CellTreeOpType.Union:
              boolExpression = BoolExpression.CreateOr(bools1[index], bools2[index]);
              break;
            case CellTreeOpType.IJ:
              boolExpression = BoolExpression.CreateAnd(bools1[index], bools2[index]);
              break;
            case CellTreeOpType.LASJ:
              boolExpression = BoolExpression.CreateAnd(bools1[index], BoolExpression.CreateNot(bools2[index]));
              break;
          }
        }
        boolExpression?.ExpensiveSimplify();
        boolExpressionList.Add(boolExpression);
      }
      return boolExpressionList;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      this.m_viewgenContext.MemberMaps.ProjectedSlotMap.ToCompactString(builder);
    }
  }
}
