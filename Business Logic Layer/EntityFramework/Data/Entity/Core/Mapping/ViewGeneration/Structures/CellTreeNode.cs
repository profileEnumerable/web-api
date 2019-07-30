// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.CellTreeNode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal abstract class CellTreeNode : InternalBase
  {
    private readonly ViewgenContext m_viewgenContext;

    protected CellTreeNode(ViewgenContext context)
    {
      this.m_viewgenContext = context;
    }

    internal CellTreeNode MakeCopy()
    {
      return this.Accept<bool, CellTreeNode>((CellTreeNode.CellTreeVisitor<bool, CellTreeNode>) new CellTreeNode.DefaultCellTreeVisitor<bool>(), true);
    }

    internal abstract CellTreeOpType OpType { get; }

    internal abstract MemberDomainMap RightDomainMap { get; }

    internal abstract FragmentQuery LeftFragmentQuery { get; }

    internal abstract FragmentQuery RightFragmentQuery { get; }

    internal bool IsEmptyRightFragmentQuery
    {
      get
      {
        return !this.m_viewgenContext.RightFragmentQP.IsSatisfiable(this.RightFragmentQuery);
      }
    }

    internal abstract Set<MemberPath> Attributes { get; }

    internal abstract List<CellTreeNode> Children { get; }

    internal abstract int NumProjectedSlots { get; }

    internal abstract int NumBoolSlots { get; }

    internal MemberProjectionIndex ProjectedSlotMap
    {
      get
      {
        return this.m_viewgenContext.MemberMaps.ProjectedSlotMap;
      }
    }

    internal ViewgenContext ViewgenContext
    {
      get
      {
        return this.m_viewgenContext;
      }
    }

    internal abstract CqlBlock ToCqlBlock(
      bool[] requiredSlots,
      CqlIdentifiers identifiers,
      ref int blockAliasNum,
      ref List<WithRelationship> withRelationships);

    internal abstract bool IsProjectedSlot(int slot);

    internal abstract TOutput Accept<TInput, TOutput>(
      CellTreeNode.CellTreeVisitor<TInput, TOutput> visitor,
      TInput param);

    internal abstract TOutput Accept<TInput, TOutput>(
      CellTreeNode.SimpleCellTreeVisitor<TInput, TOutput> visitor,
      TInput param);

    internal CellTreeNode Flatten()
    {
      return CellTreeNode.FlatteningVisitor.Flatten(this);
    }

    internal List<LeftCellWrapper> GetLeaves()
    {
      return this.GetLeafNodes().Select<LeafCellTreeNode, LeftCellWrapper>((Func<LeafCellTreeNode, LeftCellWrapper>) (leafNode => leafNode.LeftCellWrapper)).ToList<LeftCellWrapper>();
    }

    internal IEnumerable<LeafCellTreeNode> GetLeafNodes()
    {
      return CellTreeNode.LeafVisitor.GetLeaves(this);
    }

    internal CellTreeNode AssociativeFlatten()
    {
      return CellTreeNode.AssociativeOpFlatteningVisitor.Flatten(this);
    }

    internal static bool IsAssociativeOp(CellTreeOpType opType)
    {
      if (opType != CellTreeOpType.IJ && opType != CellTreeOpType.Union)
        return opType == CellTreeOpType.FOJ;
      return true;
    }

    internal bool[] GetProjectedSlots()
    {
      int length = this.ProjectedSlotMap.Count + this.NumBoolSlots;
      bool[] flagArray = new bool[length];
      for (int slot = 0; slot < length; ++slot)
        flagArray[slot] = this.IsProjectedSlot(slot);
      return flagArray;
    }

    protected MemberPath GetMemberPath(int slotNum)
    {
      return this.ProjectedSlotMap.GetMemberPath(slotNum, this.NumBoolSlots);
    }

    protected int BoolIndexToSlot(int boolIndex)
    {
      return this.ProjectedSlotMap.BoolIndexToSlot(boolIndex, this.NumBoolSlots);
    }

    protected int SlotToBoolIndex(int slotNum)
    {
      return this.ProjectedSlotMap.SlotToBoolIndex(slotNum, this.NumBoolSlots);
    }

    protected bool IsKeySlot(int slotNum)
    {
      return this.ProjectedSlotMap.IsKeySlot(slotNum, this.NumBoolSlots);
    }

    protected bool IsBoolSlot(int slotNum)
    {
      return this.ProjectedSlotMap.IsBoolSlot(slotNum, this.NumBoolSlots);
    }

    protected IEnumerable<int> KeySlots
    {
      get
      {
        int numMembers = this.ProjectedSlotMap.Count;
        for (int slotNum = 0; slotNum < numMembers; ++slotNum)
        {
          if (this.IsKeySlot(slotNum))
            yield return slotNum;
        }
      }
    }

    internal override void ToFullString(StringBuilder builder)
    {
      int blockAliasNum = 0;
      bool[] projectedSlots = this.GetProjectedSlots();
      CqlIdentifiers identifiers = new CqlIdentifiers();
      List<WithRelationship> withRelationships = new List<WithRelationship>();
      this.ToCqlBlock(projectedSlots, identifiers, ref blockAliasNum, ref withRelationships).AsEsql(builder, false, 1);
    }

    internal abstract class CellTreeVisitor<TInput, TOutput>
    {
      internal abstract TOutput VisitLeaf(LeafCellTreeNode node, TInput param);

      internal abstract TOutput VisitUnion(OpCellTreeNode node, TInput param);

      internal abstract TOutput VisitInnerJoin(OpCellTreeNode node, TInput param);

      internal abstract TOutput VisitLeftOuterJoin(OpCellTreeNode node, TInput param);

      internal abstract TOutput VisitFullOuterJoin(OpCellTreeNode node, TInput param);

      internal abstract TOutput VisitLeftAntiSemiJoin(OpCellTreeNode node, TInput param);
    }

    internal abstract class SimpleCellTreeVisitor<TInput, TOutput>
    {
      internal abstract TOutput VisitLeaf(LeafCellTreeNode node, TInput param);

      internal abstract TOutput VisitOpNode(OpCellTreeNode node, TInput param);
    }

    private class DefaultCellTreeVisitor<TInput> : CellTreeNode.CellTreeVisitor<TInput, CellTreeNode>
    {
      internal override CellTreeNode VisitLeaf(LeafCellTreeNode node, TInput param)
      {
        return (CellTreeNode) node;
      }

      internal override CellTreeNode VisitUnion(OpCellTreeNode node, TInput param)
      {
        return (CellTreeNode) this.AcceptChildren(node, param);
      }

      internal override CellTreeNode VisitInnerJoin(OpCellTreeNode node, TInput param)
      {
        return (CellTreeNode) this.AcceptChildren(node, param);
      }

      internal override CellTreeNode VisitLeftOuterJoin(
        OpCellTreeNode node,
        TInput param)
      {
        return (CellTreeNode) this.AcceptChildren(node, param);
      }

      internal override CellTreeNode VisitFullOuterJoin(
        OpCellTreeNode node,
        TInput param)
      {
        return (CellTreeNode) this.AcceptChildren(node, param);
      }

      internal override CellTreeNode VisitLeftAntiSemiJoin(
        OpCellTreeNode node,
        TInput param)
      {
        return (CellTreeNode) this.AcceptChildren(node, param);
      }

      private OpCellTreeNode AcceptChildren(OpCellTreeNode node, TInput param)
      {
        List<CellTreeNode> cellTreeNodeList = new List<CellTreeNode>();
        foreach (CellTreeNode child in node.Children)
          cellTreeNodeList.Add(child.Accept<TInput, CellTreeNode>((CellTreeNode.CellTreeVisitor<TInput, CellTreeNode>) this, param));
        return new OpCellTreeNode(node.ViewgenContext, node.OpType, (IEnumerable<CellTreeNode>) cellTreeNodeList);
      }
    }

    private class FlatteningVisitor : CellTreeNode.SimpleCellTreeVisitor<bool, CellTreeNode>
    {
      protected FlatteningVisitor()
      {
      }

      internal static CellTreeNode Flatten(CellTreeNode node)
      {
        CellTreeNode.FlatteningVisitor flatteningVisitor = new CellTreeNode.FlatteningVisitor();
        return node.Accept<bool, CellTreeNode>((CellTreeNode.SimpleCellTreeVisitor<bool, CellTreeNode>) flatteningVisitor, true);
      }

      internal override CellTreeNode VisitLeaf(LeafCellTreeNode node, bool dummy)
      {
        return (CellTreeNode) node;
      }

      internal override CellTreeNode VisitOpNode(OpCellTreeNode node, bool dummy)
      {
        List<CellTreeNode> cellTreeNodeList = new List<CellTreeNode>();
        foreach (CellTreeNode child in node.Children)
        {
          CellTreeNode cellTreeNode = child.Accept<bool, CellTreeNode>((CellTreeNode.SimpleCellTreeVisitor<bool, CellTreeNode>) this, dummy);
          cellTreeNodeList.Add(cellTreeNode);
        }
        if (cellTreeNodeList.Count == 1)
          return cellTreeNodeList[0];
        return (CellTreeNode) new OpCellTreeNode(node.ViewgenContext, node.OpType, (IEnumerable<CellTreeNode>) cellTreeNodeList);
      }
    }

    private class AssociativeOpFlatteningVisitor : CellTreeNode.SimpleCellTreeVisitor<bool, CellTreeNode>
    {
      private AssociativeOpFlatteningVisitor()
      {
      }

      internal static CellTreeNode Flatten(CellTreeNode node)
      {
        return CellTreeNode.FlatteningVisitor.Flatten(node).Accept<bool, CellTreeNode>((CellTreeNode.SimpleCellTreeVisitor<bool, CellTreeNode>) new CellTreeNode.AssociativeOpFlatteningVisitor(), true);
      }

      internal override CellTreeNode VisitLeaf(LeafCellTreeNode node, bool dummy)
      {
        return (CellTreeNode) node;
      }

      internal override CellTreeNode VisitOpNode(OpCellTreeNode node, bool dummy)
      {
        List<CellTreeNode> cellTreeNodeList1 = new List<CellTreeNode>();
        foreach (CellTreeNode child in node.Children)
        {
          CellTreeNode cellTreeNode = child.Accept<bool, CellTreeNode>((CellTreeNode.SimpleCellTreeVisitor<bool, CellTreeNode>) this, dummy);
          cellTreeNodeList1.Add(cellTreeNode);
        }
        List<CellTreeNode> cellTreeNodeList2 = cellTreeNodeList1;
        if (CellTreeNode.IsAssociativeOp(node.OpType))
        {
          cellTreeNodeList2 = new List<CellTreeNode>();
          foreach (CellTreeNode cellTreeNode in cellTreeNodeList1)
          {
            if (cellTreeNode.OpType == node.OpType)
              cellTreeNodeList2.AddRange((IEnumerable<CellTreeNode>) cellTreeNode.Children);
            else
              cellTreeNodeList2.Add(cellTreeNode);
          }
        }
        return (CellTreeNode) new OpCellTreeNode(node.ViewgenContext, node.OpType, (IEnumerable<CellTreeNode>) cellTreeNodeList2);
      }
    }

    private class LeafVisitor : CellTreeNode.SimpleCellTreeVisitor<bool, IEnumerable<LeafCellTreeNode>>
    {
      private LeafVisitor()
      {
      }

      internal static IEnumerable<LeafCellTreeNode> GetLeaves(
        CellTreeNode node)
      {
        CellTreeNode.LeafVisitor leafVisitor = new CellTreeNode.LeafVisitor();
        return node.Accept<bool, IEnumerable<LeafCellTreeNode>>((CellTreeNode.SimpleCellTreeVisitor<bool, IEnumerable<LeafCellTreeNode>>) leafVisitor, true);
      }

      internal override IEnumerable<LeafCellTreeNode> VisitLeaf(
        LeafCellTreeNode node,
        bool dummy)
      {
        yield return node;
      }

      internal override IEnumerable<LeafCellTreeNode> VisitOpNode(
        OpCellTreeNode node,
        bool dummy)
      {
        foreach (CellTreeNode child in node.Children)
        {
          IEnumerable<LeafCellTreeNode> children = child.Accept<bool, IEnumerable<LeafCellTreeNode>>((CellTreeNode.SimpleCellTreeVisitor<bool, IEnumerable<LeafCellTreeNode>>) this, dummy);
          foreach (LeafCellTreeNode leafCellTreeNode in children)
            yield return leafCellTreeNode;
        }
      }
    }
  }
}
