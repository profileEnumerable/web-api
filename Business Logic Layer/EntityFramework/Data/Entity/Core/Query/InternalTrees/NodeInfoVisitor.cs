// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.NodeInfoVisitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class NodeInfoVisitor : BasicOpVisitorOfT<NodeInfo>
  {
    private readonly Command m_command;

    internal void RecomputeNodeInfo(Node n)
    {
      if (!n.IsNodeInfoInitialized)
        return;
      this.VisitNode(n).ComputeHashValue(this.m_command, n);
    }

    internal NodeInfoVisitor(Command command)
    {
      this.m_command = command;
    }

    private NodeInfo GetNodeInfo(Node n)
    {
      return n.GetNodeInfo(this.m_command);
    }

    private ExtendedNodeInfo GetExtendedNodeInfo(Node n)
    {
      return n.GetExtendedNodeInfo(this.m_command);
    }

    private NodeInfo InitNodeInfo(Node n)
    {
      NodeInfo nodeInfo = this.GetNodeInfo(n);
      nodeInfo.Clear();
      return nodeInfo;
    }

    private ExtendedNodeInfo InitExtendedNodeInfo(Node n)
    {
      ExtendedNodeInfo extendedNodeInfo = this.GetExtendedNodeInfo(n);
      extendedNodeInfo.Clear();
      return extendedNodeInfo;
    }

    protected override NodeInfo VisitDefault(Node n)
    {
      NodeInfo nodeInfo1 = this.InitNodeInfo(n);
      foreach (Node child in n.Children)
      {
        NodeInfo nodeInfo2 = this.GetNodeInfo(child);
        nodeInfo1.ExternalReferences.Or(nodeInfo2.ExternalReferences);
      }
      return nodeInfo1;
    }

    private static bool IsDefinitionNonNullable(Node definition, VarVec nonNullableInputs)
    {
      if (definition.Op.OpType == OpType.Constant || definition.Op.OpType == OpType.InternalConstant || definition.Op.OpType == OpType.NullSentinel)
        return true;
      if (definition.Op.OpType == OpType.VarRef)
        return nonNullableInputs.IsSet(((VarRefOp) definition.Op).Var);
      return false;
    }

    public override NodeInfo Visit(VarRefOp op, Node n)
    {
      NodeInfo nodeInfo = this.InitNodeInfo(n);
      nodeInfo.ExternalReferences.Set(op.Var);
      return nodeInfo;
    }

    protected override NodeInfo VisitRelOpDefault(RelOp op, Node n)
    {
      return this.Unimplemented(n);
    }

    protected override NodeInfo VisitTableOp(ScanTableBaseOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo = this.InitExtendedNodeInfo(n);
      extendedNodeInfo.LocalDefinitions.Or(op.Table.ReferencedColumns);
      extendedNodeInfo.Definitions.Or(op.Table.ReferencedColumns);
      if (op.Table.ReferencedColumns.Subsumes(op.Table.Keys))
        extendedNodeInfo.Keys.InitFrom((IEnumerable<Var>) op.Table.Keys);
      extendedNodeInfo.NonNullableDefinitions.Or(op.Table.NonNullableColumns);
      extendedNodeInfo.NonNullableDefinitions.And(extendedNodeInfo.Definitions);
      return (NodeInfo) extendedNodeInfo;
    }

    public override NodeInfo Visit(UnnestOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo = this.InitExtendedNodeInfo(n);
      foreach (Var column in (List<Var>) op.Table.Columns)
      {
        extendedNodeInfo.LocalDefinitions.Set(column);
        extendedNodeInfo.Definitions.Set(column);
      }
      if (n.Child0.Op.OpType == OpType.VarDef && n.Child0.Child0.Op.OpType == OpType.Function && (op.Table.Keys.Count > 0 && op.Table.ReferencedColumns.Subsumes(op.Table.Keys)))
        extendedNodeInfo.Keys.InitFrom((IEnumerable<Var>) op.Table.Keys);
      if (n.HasChild0)
      {
        NodeInfo nodeInfo = this.GetNodeInfo(n.Child0);
        extendedNodeInfo.ExternalReferences.Or(nodeInfo.ExternalReferences);
      }
      else
        extendedNodeInfo.ExternalReferences.Set(op.Var);
      return (NodeInfo) extendedNodeInfo;
    }

    internal static Dictionary<Var, Var> ComputeVarRemappings(Node varDefListNode)
    {
      Dictionary<Var, Var> dictionary = new Dictionary<Var, Var>();
      foreach (Node child in varDefListNode.Children)
      {
        VarRefOp op1 = child.Child0.Op as VarRefOp;
        if (op1 != null)
        {
          VarDefOp op2 = child.Op as VarDefOp;
          dictionary[op1.Var] = op2.Var;
        }
      }
      return dictionary;
    }

    public override NodeInfo Visit(ProjectOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      foreach (Var output in op.Outputs)
      {
        if (extendedNodeInfo2.Definitions.IsSet(output))
          extendedNodeInfo1.Definitions.Set(output);
        else
          extendedNodeInfo1.ExternalReferences.Set(output);
      }
      extendedNodeInfo1.NonNullableDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableDefinitions.And(op.Outputs);
      extendedNodeInfo1.NonNullableVisibleDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      foreach (Node child in n.Child1.Children)
      {
        VarDefOp op1 = child.Op as VarDefOp;
        NodeInfo nodeInfo = this.GetNodeInfo(child.Child0);
        extendedNodeInfo1.LocalDefinitions.Set(op1.Var);
        extendedNodeInfo1.ExternalReferences.Clear(op1.Var);
        extendedNodeInfo1.Definitions.Set(op1.Var);
        extendedNodeInfo1.ExternalReferences.Or(nodeInfo.ExternalReferences);
        if (NodeInfoVisitor.IsDefinitionNonNullable(child.Child0, extendedNodeInfo1.NonNullableVisibleDefinitions))
          extendedNodeInfo1.NonNullableDefinitions.Set(op1.Var);
      }
      extendedNodeInfo1.ExternalReferences.Minus(extendedNodeInfo2.Definitions);
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo2.ExternalReferences);
      extendedNodeInfo1.Keys.NoKeys = true;
      if (!extendedNodeInfo2.Keys.NoKeys)
      {
        VarVec varVec1 = this.m_command.CreateVarVec(extendedNodeInfo2.Keys.KeyVars).Remap(NodeInfoVisitor.ComputeVarRemappings(n.Child1));
        VarVec varVec2 = varVec1.Clone();
        VarVec varVec3 = this.m_command.CreateVarVec(op.Outputs);
        varVec1.Minus(varVec3);
        if (varVec1.IsEmpty)
          extendedNodeInfo1.Keys.InitFrom((IEnumerable<Var>) varVec2);
      }
      extendedNodeInfo1.InitRowCountFrom(extendedNodeInfo2);
      return (NodeInfo) extendedNodeInfo1;
    }

    public override NodeInfo Visit(FilterOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      NodeInfo nodeInfo = this.GetNodeInfo(n.Child1);
      extendedNodeInfo1.Definitions.Or(extendedNodeInfo2.Definitions);
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo2.ExternalReferences);
      extendedNodeInfo1.ExternalReferences.Or(nodeInfo.ExternalReferences);
      extendedNodeInfo1.ExternalReferences.Minus(extendedNodeInfo2.Definitions);
      extendedNodeInfo1.Keys.InitFrom(extendedNodeInfo2.Keys);
      extendedNodeInfo1.NonNullableDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableVisibleDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.MinRows = RowCount.Zero;
      ConstantPredicateOp op1 = n.Child1.Op as ConstantPredicateOp;
      extendedNodeInfo1.MaxRows = op1 == null || !op1.IsFalse ? extendedNodeInfo2.MaxRows : RowCount.Zero;
      return (NodeInfo) extendedNodeInfo1;
    }

    protected override NodeInfo VisitGroupByOp(GroupByBaseOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      extendedNodeInfo1.Definitions.InitFrom(op.Outputs);
      extendedNodeInfo1.LocalDefinitions.InitFrom(extendedNodeInfo1.Definitions);
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo2.ExternalReferences);
      foreach (Node child in n.Child1.Children)
      {
        NodeInfo nodeInfo = this.GetNodeInfo(child.Child0);
        extendedNodeInfo1.ExternalReferences.Or(nodeInfo.ExternalReferences);
        if (NodeInfoVisitor.IsDefinitionNonNullable(child.Child0, extendedNodeInfo2.NonNullableDefinitions))
          extendedNodeInfo1.NonNullableDefinitions.Set(((VarDefOp) child.Op).Var);
      }
      extendedNodeInfo1.NonNullableDefinitions.Or(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableDefinitions.And(op.Keys);
      for (int index = 2; index < n.Children.Count; ++index)
      {
        foreach (Node child in n.Children[index].Children)
        {
          NodeInfo nodeInfo = this.GetNodeInfo(child.Child0);
          extendedNodeInfo1.ExternalReferences.Or(nodeInfo.ExternalReferences);
        }
      }
      extendedNodeInfo1.ExternalReferences.Minus(extendedNodeInfo2.Definitions);
      extendedNodeInfo1.Keys.InitFrom((IEnumerable<Var>) op.Keys);
      extendedNodeInfo1.MinRows = op.Keys.IsEmpty ? RowCount.One : (extendedNodeInfo2.MinRows == RowCount.One ? RowCount.One : RowCount.Zero);
      extendedNodeInfo1.MaxRows = op.Keys.IsEmpty ? RowCount.One : extendedNodeInfo2.MaxRows;
      return (NodeInfo) extendedNodeInfo1;
    }

    public override NodeInfo Visit(CrossJoinOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      List<KeyVec> keyVecList = new List<KeyVec>();
      RowCount maxRows = RowCount.Zero;
      RowCount minRows = RowCount.One;
      foreach (Node child in n.Children)
      {
        ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(child);
        extendedNodeInfo1.Definitions.Or(extendedNodeInfo2.Definitions);
        extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo2.ExternalReferences);
        keyVecList.Add(extendedNodeInfo2.Keys);
        extendedNodeInfo1.NonNullableDefinitions.Or(extendedNodeInfo2.NonNullableDefinitions);
        if (extendedNodeInfo2.MaxRows > maxRows)
          maxRows = extendedNodeInfo2.MaxRows;
        if (extendedNodeInfo2.MinRows < minRows)
          minRows = extendedNodeInfo2.MinRows;
      }
      extendedNodeInfo1.Keys.InitFrom(keyVecList);
      extendedNodeInfo1.SetRowCount(minRows, maxRows);
      return (NodeInfo) extendedNodeInfo1;
    }

    protected override NodeInfo VisitJoinOp(JoinBaseOp op, Node n)
    {
      if (op.OpType != OpType.InnerJoin && op.OpType != OpType.LeftOuterJoin && op.OpType != OpType.FullOuterJoin)
        return this.Unimplemented(n);
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      ExtendedNodeInfo extendedNodeInfo3 = this.GetExtendedNodeInfo(n.Child1);
      NodeInfo nodeInfo = this.GetNodeInfo(n.Child2);
      extendedNodeInfo1.Definitions.Or(extendedNodeInfo2.Definitions);
      extendedNodeInfo1.Definitions.Or(extendedNodeInfo3.Definitions);
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo2.ExternalReferences);
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo3.ExternalReferences);
      extendedNodeInfo1.ExternalReferences.Or(nodeInfo.ExternalReferences);
      extendedNodeInfo1.ExternalReferences.Minus(extendedNodeInfo1.Definitions);
      extendedNodeInfo1.Keys.InitFrom(extendedNodeInfo2.Keys, extendedNodeInfo3.Keys);
      if (op.OpType == OpType.InnerJoin || op.OpType == OpType.LeftOuterJoin)
        extendedNodeInfo1.NonNullableDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      if (op.OpType == OpType.InnerJoin)
        extendedNodeInfo1.NonNullableDefinitions.Or(extendedNodeInfo3.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableVisibleDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableVisibleDefinitions.Or(extendedNodeInfo3.NonNullableDefinitions);
      RowCount minRows;
      RowCount maxRows;
      if (op.OpType == OpType.FullOuterJoin)
      {
        minRows = RowCount.Zero;
        maxRows = RowCount.Unbounded;
      }
      else
      {
        maxRows = extendedNodeInfo2.MaxRows > RowCount.One || extendedNodeInfo3.MaxRows > RowCount.One ? RowCount.Unbounded : RowCount.One;
        minRows = op.OpType != OpType.LeftOuterJoin ? RowCount.Zero : extendedNodeInfo2.MinRows;
      }
      extendedNodeInfo1.SetRowCount(minRows, maxRows);
      return (NodeInfo) extendedNodeInfo1;
    }

    protected override NodeInfo VisitApplyOp(ApplyBaseOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      ExtendedNodeInfo extendedNodeInfo3 = this.GetExtendedNodeInfo(n.Child1);
      extendedNodeInfo1.Definitions.Or(extendedNodeInfo2.Definitions);
      extendedNodeInfo1.Definitions.Or(extendedNodeInfo3.Definitions);
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo2.ExternalReferences);
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo3.ExternalReferences);
      extendedNodeInfo1.ExternalReferences.Minus(extendedNodeInfo1.Definitions);
      extendedNodeInfo1.Keys.InitFrom(extendedNodeInfo2.Keys, extendedNodeInfo3.Keys);
      extendedNodeInfo1.NonNullableDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      if (op.OpType == OpType.CrossApply)
        extendedNodeInfo1.NonNullableDefinitions.Or(extendedNodeInfo3.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableVisibleDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableVisibleDefinitions.Or(extendedNodeInfo3.NonNullableDefinitions);
      RowCount maxRows = extendedNodeInfo2.MaxRows > RowCount.One || extendedNodeInfo3.MaxRows > RowCount.One ? RowCount.Unbounded : RowCount.One;
      RowCount minRows = op.OpType == OpType.CrossApply ? RowCount.Zero : extendedNodeInfo2.MinRows;
      extendedNodeInfo1.SetRowCount(minRows, maxRows);
      return (NodeInfo) extendedNodeInfo1;
    }

    protected override NodeInfo VisitSetOp(SetOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      extendedNodeInfo1.Definitions.InitFrom(op.Outputs);
      extendedNodeInfo1.LocalDefinitions.InitFrom(op.Outputs);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      ExtendedNodeInfo extendedNodeInfo3 = this.GetExtendedNodeInfo(n.Child1);
      RowCount rowCount = RowCount.Zero;
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo2.ExternalReferences);
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo3.ExternalReferences);
      if (op.OpType == OpType.UnionAll)
        rowCount = extendedNodeInfo2.MinRows > extendedNodeInfo3.MinRows ? extendedNodeInfo2.MinRows : extendedNodeInfo3.MinRows;
      if (op.OpType == OpType.Intersect || op.OpType == OpType.Except)
      {
        extendedNodeInfo1.Keys.InitFrom((IEnumerable<Var>) op.Outputs);
      }
      else
      {
        UnionAllOp unionAllOp = (UnionAllOp) op;
        if (unionAllOp.BranchDiscriminator == null)
        {
          extendedNodeInfo1.Keys.NoKeys = true;
        }
        else
        {
          VarVec varVec = this.m_command.CreateVarVec();
          for (int index = 0; index < n.Children.Count; ++index)
          {
            ExtendedNodeInfo extendedNodeInfo4 = n.Children[index].GetExtendedNodeInfo(this.m_command);
            if (!extendedNodeInfo4.Keys.NoKeys && !extendedNodeInfo4.Keys.KeyVars.IsEmpty)
            {
              VarVec other = extendedNodeInfo4.Keys.KeyVars.Remap((Dictionary<Var, Var>) unionAllOp.VarMap[index].GetReverseMap());
              varVec.Or(other);
            }
            else
            {
              varVec.Clear();
              break;
            }
          }
          if (varVec.IsEmpty)
            extendedNodeInfo1.Keys.NoKeys = true;
          else
            extendedNodeInfo1.Keys.InitFrom((IEnumerable<Var>) varVec);
        }
      }
      VarVec other1 = extendedNodeInfo2.NonNullableDefinitions.Remap((Dictionary<Var, Var>) op.VarMap[0].GetReverseMap());
      extendedNodeInfo1.NonNullableDefinitions.InitFrom(other1);
      if (op.OpType != OpType.Except)
      {
        VarVec other2 = extendedNodeInfo3.NonNullableDefinitions.Remap((Dictionary<Var, Var>) op.VarMap[1].GetReverseMap());
        if (op.OpType == OpType.Intersect)
          extendedNodeInfo1.NonNullableDefinitions.Or(other2);
        else
          extendedNodeInfo1.NonNullableDefinitions.And(other2);
      }
      extendedNodeInfo1.NonNullableDefinitions.And(op.Outputs);
      extendedNodeInfo1.MinRows = rowCount;
      return (NodeInfo) extendedNodeInfo1;
    }

    protected override NodeInfo VisitSortOp(SortBaseOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      extendedNodeInfo1.Definitions.Or(extendedNodeInfo2.Definitions);
      extendedNodeInfo1.ExternalReferences.Or(extendedNodeInfo2.ExternalReferences);
      extendedNodeInfo1.ExternalReferences.Minus(extendedNodeInfo2.Definitions);
      extendedNodeInfo1.Keys.InitFrom(extendedNodeInfo2.Keys);
      extendedNodeInfo1.NonNullableDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableVisibleDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.InitRowCountFrom(extendedNodeInfo2);
      if (OpType.ConstrainedSort == op.OpType && n.Child2.Op.OpType == OpType.Constant && !((ConstrainedSortOp) op).WithTies)
      {
        ConstantBaseOp op1 = (ConstantBaseOp) n.Child2.Op;
        if (TypeHelpers.IsIntegerConstant(op1.Type, op1.Value, 1L))
          extendedNodeInfo1.SetRowCount(RowCount.Zero, RowCount.One);
      }
      return (NodeInfo) extendedNodeInfo1;
    }

    public override NodeInfo Visit(DistinctOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      extendedNodeInfo1.Keys.InitFrom((IEnumerable<Var>) op.Keys, true);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      extendedNodeInfo1.ExternalReferences.InitFrom(extendedNodeInfo2.ExternalReferences);
      foreach (Var key in op.Keys)
      {
        if (extendedNodeInfo2.Definitions.IsSet(key))
          extendedNodeInfo1.Definitions.Set(key);
        else
          extendedNodeInfo1.ExternalReferences.Set(key);
      }
      extendedNodeInfo1.NonNullableDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableDefinitions.And(op.Keys);
      extendedNodeInfo1.InitRowCountFrom(extendedNodeInfo2);
      return (NodeInfo) extendedNodeInfo1;
    }

    public override NodeInfo Visit(SingleRowOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      extendedNodeInfo1.Definitions.InitFrom(extendedNodeInfo2.Definitions);
      extendedNodeInfo1.Keys.InitFrom(extendedNodeInfo2.Keys);
      extendedNodeInfo1.ExternalReferences.InitFrom(extendedNodeInfo2.ExternalReferences);
      extendedNodeInfo1.NonNullableDefinitions.InitFrom(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.SetRowCount(RowCount.Zero, RowCount.One);
      return (NodeInfo) extendedNodeInfo1;
    }

    public override NodeInfo Visit(SingleRowTableOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo = this.InitExtendedNodeInfo(n);
      extendedNodeInfo.Keys.NoKeys = false;
      extendedNodeInfo.SetRowCount(RowCount.One, RowCount.One);
      return (NodeInfo) extendedNodeInfo;
    }

    public override NodeInfo Visit(PhysicalProjectOp op, Node n)
    {
      ExtendedNodeInfo extendedNodeInfo1 = this.InitExtendedNodeInfo(n);
      foreach (Node child in n.Children)
      {
        NodeInfo nodeInfo = this.GetNodeInfo(child);
        extendedNodeInfo1.ExternalReferences.Or(nodeInfo.ExternalReferences);
      }
      extendedNodeInfo1.Definitions.InitFrom((IEnumerable<Var>) op.Outputs);
      extendedNodeInfo1.LocalDefinitions.InitFrom(extendedNodeInfo1.Definitions);
      ExtendedNodeInfo extendedNodeInfo2 = this.GetExtendedNodeInfo(n.Child0);
      if (!extendedNodeInfo2.Keys.NoKeys)
      {
        VarVec varVec = this.m_command.CreateVarVec(extendedNodeInfo2.Keys.KeyVars);
        varVec.Minus(extendedNodeInfo1.Definitions);
        if (varVec.IsEmpty)
          extendedNodeInfo1.Keys.InitFrom(extendedNodeInfo2.Keys);
      }
      extendedNodeInfo1.NonNullableDefinitions.Or(extendedNodeInfo2.NonNullableDefinitions);
      extendedNodeInfo1.NonNullableDefinitions.And(extendedNodeInfo1.Definitions);
      extendedNodeInfo1.NonNullableVisibleDefinitions.Or(extendedNodeInfo2.NonNullableVisibleDefinitions);
      return (NodeInfo) extendedNodeInfo1;
    }

    protected override NodeInfo VisitNestOp(NestBaseOp op, Node n)
    {
      SingleStreamNestOp singleStreamNestOp = op as SingleStreamNestOp;
      ExtendedNodeInfo extendedNodeInfo = this.InitExtendedNodeInfo(n);
      foreach (CollectionInfo collectionInfo in op.CollectionInfo)
        extendedNodeInfo.LocalDefinitions.Set(collectionInfo.CollectionVar);
      extendedNodeInfo.Definitions.InitFrom(op.Outputs);
      foreach (Node child in n.Children)
        extendedNodeInfo.ExternalReferences.Or(this.GetExtendedNodeInfo(child).ExternalReferences);
      extendedNodeInfo.ExternalReferences.Minus(extendedNodeInfo.Definitions);
      if (singleStreamNestOp == null)
        extendedNodeInfo.Keys.InitFrom(this.GetExtendedNodeInfo(n.Child0).Keys);
      else
        extendedNodeInfo.Keys.InitFrom((IEnumerable<Var>) singleStreamNestOp.Keys);
      return (NodeInfo) extendedNodeInfo;
    }
  }
}
