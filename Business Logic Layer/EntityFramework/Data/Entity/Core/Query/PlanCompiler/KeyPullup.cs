// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.KeyPullup
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class KeyPullup : BasicOpVisitor
  {
    private readonly Command m_command;

    internal KeyPullup(Command command)
    {
      this.m_command = command;
    }

    internal KeyVec GetKeys(System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      ExtendedNodeInfo extendedNodeInfo = node.GetExtendedNodeInfo(this.m_command);
      if (extendedNodeInfo.Keys.NoKeys)
        this.VisitNode(node);
      return extendedNodeInfo.Keys;
    }

    protected override void VisitChildren(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (child.Op.IsRelOp || child.Op.IsPhysicalOp)
          this.GetKeys(child);
      }
    }

    protected override void VisitRelOpDefault(RelOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      this.m_command.RecomputeNodeInfo(n);
    }

    public override void Visit(ScanTableOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      op.Table.ReferencedColumns.Or(op.Table.Keys);
      this.m_command.RecomputeNodeInfo(n);
    }

    public override void Visit(ProjectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      ExtendedNodeInfo extendedNodeInfo = n.Child0.GetExtendedNodeInfo(this.m_command);
      if (!extendedNodeInfo.Keys.NoKeys)
      {
        VarVec varVec = this.m_command.CreateVarVec(op.Outputs);
        Dictionary<Var, Var> varRemappings = NodeInfoVisitor.ComputeVarRemappings(n.Child1);
        VarVec other = extendedNodeInfo.Keys.KeyVars.Remap(varRemappings);
        varVec.Or(other);
        op.Outputs.InitFrom(varVec);
      }
      this.m_command.RecomputeNodeInfo(n);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override void Visit(UnionAllOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.VisitChildren(n);
      Var var1 = (Var) this.m_command.CreateSetOpVar(this.m_command.IntegerType);
      VarList varList1 = Command.CreateVarList();
      VarVec[] varVecArray = new VarVec[n.Children.Count];
      for (int index = 0; index < n.Children.Count; ++index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child = n.Children[index];
        VarVec v = this.m_command.GetExtendedNodeInfo(child).Keys.KeyVars.Remap((Dictionary<Var, Var>) op.VarMap[index]);
        varVecArray[index] = this.m_command.CreateVarVec(v);
        varVecArray[index].Minus(op.Outputs);
        if (OpType.UnionAll == child.Op.OpType)
        {
          UnionAllOp op1 = (UnionAllOp) child.Op;
          varVecArray[index].Clear(op1.BranchDiscriminator);
        }
        varList1.AddRange((IEnumerable<Var>) varVecArray[index]);
      }
      VarList varList2 = Command.CreateVarList();
      foreach (Var var2 in (List<Var>) varList1)
      {
        Var setOpVar = (Var) this.m_command.CreateSetOpVar(var2.Type);
        varList2.Add(setOpVar);
      }
      for (int index1 = 0; index1 < n.Children.Count; ++index1)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child = n.Children[index1];
        ExtendedNodeInfo extendedNodeInfo1 = this.m_command.GetExtendedNodeInfo(child);
        VarVec varVec = this.m_command.CreateVarVec();
        List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
        Var computedVar1;
        if (OpType.UnionAll == child.Op.OpType && ((UnionAllOp) child.Op).BranchDiscriminator != null)
        {
          computedVar1 = ((UnionAllOp) child.Op).BranchDiscriminator;
          if (!op.VarMap[index1].ContainsValue(computedVar1))
          {
            op.VarMap[index1].Add(var1, computedVar1);
          }
          else
          {
            System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(0 == index1, "right branch has a discriminator var that the left branch doesn't have?");
            var1 = op.VarMap[index1].GetReverseMap()[computedVar1];
          }
        }
        else
        {
          args.Add(this.m_command.CreateVarDefNode(this.m_command.CreateNode((Op) this.m_command.CreateConstantOp(this.m_command.IntegerType, (object) this.m_command.NextBranchDiscriminatorValue)), out computedVar1));
          varVec.Set(computedVar1);
          op.VarMap[index1].Add(var1, computedVar1);
        }
        for (int index2 = 0; index2 < varList1.Count; ++index2)
        {
          Var computedVar2 = varList1[index2];
          if (!varVecArray[index1].IsSet(computedVar2))
          {
            args.Add(this.m_command.CreateVarDefNode(this.m_command.CreateNode((Op) this.m_command.CreateNullOp(computedVar2.Type)), out computedVar2));
            varVec.Set(computedVar2);
          }
          op.VarMap[index1].Add(varList2[index2], computedVar2);
        }
        if (varVec.IsEmpty)
        {
          extendedNodeInfo1.Keys.KeyVars.Set(computedVar1);
        }
        else
        {
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(args.Count != 0, "no new nodes?");
          foreach (Var v in op.VarMap[index1].Values)
            varVec.Set(v);
          n.Children[index1] = this.m_command.CreateNode((Op) this.m_command.CreateProjectOp(varVec), child, this.m_command.CreateNode((Op) this.m_command.CreateVarDefListOp(), args));
          this.m_command.RecomputeNodeInfo(n.Children[index1]);
          ExtendedNodeInfo extendedNodeInfo2 = this.m_command.GetExtendedNodeInfo(n.Children[index1]);
          extendedNodeInfo2.Keys.KeyVars.InitFrom(extendedNodeInfo1.Keys.KeyVars);
          extendedNodeInfo2.Keys.KeyVars.Set(computedVar1);
        }
      }
      n.Op = (Op) this.m_command.CreateUnionAllOp(op.VarMap[0], op.VarMap[1], var1);
      this.m_command.RecomputeNodeInfo(n);
    }
  }
}
