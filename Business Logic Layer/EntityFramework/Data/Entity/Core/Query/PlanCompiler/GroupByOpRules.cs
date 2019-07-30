// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.GroupByOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class GroupByOpRules
  {
    internal static readonly SimpleRule Rule_GroupByOpWithSimpleVarRedefinitions = new SimpleRule(OpType.GroupBy, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(GroupByOpRules.ProcessGroupByWithSimpleVarRedefinitions));
    internal static readonly SimpleRule Rule_GroupByOpOnAllInputColumnsWithAggregateOperation = new SimpleRule(OpType.GroupBy, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(GroupByOpRules.ProcessGroupByOpOnAllInputColumnsWithAggregateOperation));
    internal static readonly PatternMatchRule Rule_GroupByOverProject = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) GroupByOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(GroupByOpRules.ProcessGroupByOverProject));
    internal static readonly PatternMatchRule Rule_GroupByOpWithNoAggregates = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) GroupByOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) VarDefListOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(GroupByOpRules.ProcessGroupByOpWithNoAggregates));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[4]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) GroupByOpRules.Rule_GroupByOpWithSimpleVarRedefinitions,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) GroupByOpRules.Rule_GroupByOverProject,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) GroupByOpRules.Rule_GroupByOpWithNoAggregates,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) GroupByOpRules.Rule_GroupByOpOnAllInputColumnsWithAggregateOperation
    };

    private static bool ProcessGroupByWithSimpleVarRedefinitions(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = n;
      GroupByOp op1 = (GroupByOp) n.Op;
      if (n.Child1.Children.Count == 0)
        return false;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Command command = transformationRulesContext.Command;
      ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(n);
      bool flag = false;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Child1.Children)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child0 = child.Child0;
        if (child0.Op.OpType == OpType.VarRef)
        {
          VarRefOp op2 = (VarRefOp) child0.Op;
          if (!extendedNodeInfo.ExternalReferences.IsSet(op2.Var))
            flag = true;
        }
      }
      if (!flag)
        return false;
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Child1.Children)
      {
        VarDefOp op2 = (VarDefOp) child.Op;
        VarRefOp op3 = child.Child0.Op as VarRefOp;
        if (op3 != null && !extendedNodeInfo.ExternalReferences.IsSet(op3.Var))
        {
          op1.Outputs.Clear(op2.Var);
          op1.Outputs.Set(op3.Var);
          op1.Keys.Clear(op2.Var);
          op1.Keys.Set(op3.Var);
          transformationRulesContext.AddVarMapping(op2.Var, op3.Var);
        }
        else
          args.Add(child);
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node = command.CreateNode((Op) command.CreateVarDefListOp(), args);
      n.Child1 = node;
      return true;
    }

    private static bool ProcessGroupByOpOnAllInputColumnsWithAggregateOperation(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = n;
      PhysicalProjectOp op1 = context.Command.Root.Op as PhysicalProjectOp;
      if (op1 == null || op1.Outputs.Count > 1 || (n.Child0.Op.OpType != OpType.ScanTable || n.Child2 == null) || (n.Child2.Child0 == null || n.Child2.Child0.Child0 == null || n.Child2.Child0.Child0.Op.OpType != OpType.Aggregate))
        return false;
      GroupByOp op2 = (GroupByOp) n.Op;
      Table table = ((ScanTableBaseOp) n.Child0.Op).Table;
      VarList columns = table.Columns;
      foreach (Var v in (List<Var>) columns)
      {
        if (!op2.Keys.IsSet(v))
          return false;
      }
      foreach (Var v in (List<Var>) columns)
      {
        op2.Outputs.Clear(v);
        op2.Keys.Clear(v);
      }
      Command command = context.Command;
      ScanTableOp scanTableOp = command.CreateScanTableOp(table.TableMetadata);
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = command.CreateNode((Op) scanTableOp);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = command.CreateNode((Op) command.CreateOuterApplyOp(), node1, n);
      Var computedVar;
      System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = command.CreateVarDefListNode(command.CreateNode((Op) command.CreateVarRefOp(op2.Outputs.First)), out computedVar);
      newNode = command.CreateNode((Op) command.CreateProjectOp(computedVar), node2, varDefListNode);
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      IEnumerator<Var> enumerator1 = scanTableOp.Table.Keys.GetEnumerator();
      IEnumerator<Var> enumerator2 = table.Keys.GetEnumerator();
      for (int index = 0; index < table.Keys.Count; ++index)
      {
        enumerator1.MoveNext();
        enumerator2.MoveNext();
        System.Data.Entity.Core.Query.InternalTrees.Node node4 = command.CreateNode((Op) command.CreateComparisonOp(OpType.EQ, false), command.CreateNode((Op) command.CreateVarRefOp(enumerator1.Current)), command.CreateNode((Op) command.CreateVarRefOp(enumerator2.Current)));
        node3 = node3 == null ? node4 : command.CreateNode((Op) command.CreateConditionalOp(OpType.And), node3, node4);
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node5 = command.CreateNode((Op) command.CreateFilterOp(), n.Child0, node3);
      n.Child0 = node5;
      return true;
    }

    private static bool ProcessGroupByOverProject(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = n;
      GroupByOp op = (GroupByOp) n.Op;
      Command command = context.Command;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = n.Child0;
      System.Data.Entity.Core.Query.InternalTrees.Node child1_1 = child0.Child1;
      System.Data.Entity.Core.Query.InternalTrees.Node child1_2 = n.Child1;
      System.Data.Entity.Core.Query.InternalTrees.Node child2 = n.Child2;
      if (child1_2.Children.Count > 0)
        return false;
      VarVec varVec = command.GetExtendedNodeInfo(child0).LocalDefinitions;
      if (op.Outputs.Overlaps(varVec))
        return false;
      bool flag = false;
      for (int index = 0; index < child1_1.Children.Count; ++index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child = child1_1.Children[index];
        if (child.Child0.Op.OpType == OpType.Constant || child.Child0.Op.OpType == OpType.InternalConstant || child.Child0.Op.OpType == OpType.NullSentinel)
        {
          if (!flag)
          {
            varVec = command.CreateVarVec(varVec);
            flag = true;
          }
          varVec.Clear(((VarDefOp) child.Op).Var);
        }
      }
      if (GroupByOpRules.VarRefUsageFinder.AnyVarUsedMoreThanOnce(varVec, child2, command))
        return false;
      Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varReplacementTable = new Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node>(child1_1.Children.Count);
      for (int index = 0; index < child1_1.Children.Count; ++index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child = child1_1.Children[index];
        Var var = ((VarDefOp) child.Op).Var;
        varReplacementTable.Add(var, child.Child0);
      }
      newNode.Child2 = GroupByOpRules.VarRefReplacer.Replace(varReplacementTable, child2, command);
      newNode.Child0 = child0.Child0;
      return true;
    }

    private static bool ProcessGroupByOpWithNoAggregates(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      Command command = context.Command;
      GroupByOp op = (GroupByOp) n.Op;
      ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(n.Child0);
      ProjectOp projectOp = command.CreateProjectOp(op.Keys);
      VarDefListOp varDefListOp = command.CreateVarDefListOp();
      command.CreateNode((Op) varDefListOp);
      newNode = command.CreateNode((Op) projectOp, n.Child0, n.Child1);
      if (extendedNodeInfo.Keys.NoKeys || !op.Keys.Subsumes(extendedNodeInfo.Keys.KeyVars))
        newNode = command.CreateNode((Op) command.CreateDistinctOp(command.CreateVarVec(op.Keys)), newNode);
      return true;
    }

    internal class VarRefReplacer : BasicOpVisitorOfNode
    {
      private readonly Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> m_varReplacementTable;
      private readonly Command m_command;

      private VarRefReplacer(Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varReplacementTable, Command command)
      {
        this.m_varReplacementTable = varReplacementTable;
        this.m_command = command;
      }

      internal static System.Data.Entity.Core.Query.InternalTrees.Node Replace(
        Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varReplacementTable,
        System.Data.Entity.Core.Query.InternalTrees.Node root,
        Command command)
      {
        return new GroupByOpRules.VarRefReplacer(varReplacementTable, command).VisitNode(root);
      }

      public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
        VarRefOp op,
        System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node;
        if (this.m_varReplacementTable.TryGetValue(op.Var, out node))
          return node;
        return n;
      }

      protected override System.Data.Entity.Core.Query.InternalTrees.Node VisitDefault(System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node n1 = base.VisitDefault(n);
        this.m_command.RecomputeNodeInfo(n1);
        return n1;
      }
    }

    internal class VarRefUsageFinder : BasicOpVisitor
    {
      private bool m_anyUsedMoreThenOnce;
      private readonly VarVec m_varVec;
      private readonly VarVec m_usedVars;

      private VarRefUsageFinder(VarVec varVec, Command command)
      {
        this.m_varVec = varVec;
        this.m_usedVars = command.CreateVarVec();
      }

      internal static bool AnyVarUsedMoreThanOnce(VarVec varVec, System.Data.Entity.Core.Query.InternalTrees.Node root, Command command)
      {
        GroupByOpRules.VarRefUsageFinder varRefUsageFinder = new GroupByOpRules.VarRefUsageFinder(varVec, command);
        varRefUsageFinder.VisitNode(root);
        return varRefUsageFinder.m_anyUsedMoreThenOnce;
      }

      public override void Visit(VarRefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        Var var = op.Var;
        if (!this.m_varVec.IsSet(var))
          return;
        if (this.m_usedVars.IsSet(var))
          this.m_anyUsedMoreThenOnce = true;
        else
          this.m_usedVars.Set(var);
      }

      protected override void VisitChildren(System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        if (this.m_anyUsedMoreThenOnce)
          return;
        base.VisitChildren(n);
      }
    }
  }
}
