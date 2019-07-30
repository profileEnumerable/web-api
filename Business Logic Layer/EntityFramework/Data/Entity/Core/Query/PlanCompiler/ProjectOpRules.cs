// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ProjectOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Linq;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class ProjectOpRules
  {
    internal static readonly PatternMatchRule Rule_ProjectOverProject = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ProjectOpRules.ProcessProjectOverProject));
    internal static readonly PatternMatchRule Rule_ProjectWithNoLocalDefs = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) VarDefListOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ProjectOpRules.ProcessProjectWithNoLocalDefinitions));
    internal static readonly SimpleRule Rule_ProjectOpWithSimpleVarRedefinitions = new SimpleRule(OpType.Project, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ProjectOpRules.ProcessProjectWithSimpleVarRedefinitions));
    internal static readonly SimpleRule Rule_ProjectOpWithNullSentinel = new SimpleRule(OpType.Project, new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ProjectOpRules.ProcessProjectOpWithNullSentinel));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[4]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ProjectOpRules.Rule_ProjectOpWithNullSentinel,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ProjectOpRules.Rule_ProjectOpWithSimpleVarRedefinitions,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ProjectOpRules.Rule_ProjectOverProject,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ProjectOpRules.Rule_ProjectWithNoLocalDefs
    };

    private static bool ProcessProjectOverProject(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node projectNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = projectNode;
      ProjectOp op1 = (ProjectOp) projectNode.Op;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = projectNode.Child1;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = projectNode.Child0;
      ProjectOp op2 = (ProjectOp) child0.Op;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Dictionary<Var, int> varRefMap = new Dictionary<Var, int>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in child1.Children)
      {
        if (!transformationRulesContext.IsScalarOpTree(child.Child0, varRefMap))
          return false;
      }
      Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varMap = transformationRulesContext.GetVarMap(child0.Child1, varRefMap);
      if (varMap == null)
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node node = transformationRulesContext.Command.CreateNode((Op) transformationRulesContext.Command.CreateVarDefListOp());
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in child1.Children)
      {
        child.Child0 = transformationRulesContext.ReMap(child.Child0, varMap);
        transformationRulesContext.Command.RecomputeNodeInfo(child);
        node.Children.Add(child);
      }
      ExtendedNodeInfo extendedNodeInfo = transformationRulesContext.Command.GetExtendedNodeInfo(projectNode);
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in child0.Child1.Children)
      {
        VarDefOp op3 = (VarDefOp) child.Op;
        if (extendedNodeInfo.Definitions.IsSet(op3.Var))
          node.Children.Add(child);
      }
      projectNode.Child0 = child0.Child0;
      projectNode.Child1 = node;
      return true;
    }

    private static bool ProcessProjectWithNoLocalDefinitions(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = n;
      if (!context.Command.GetNodeInfo(n).ExternalReferences.IsEmpty)
        return false;
      newNode = n.Child0;
      return true;
    }

    private static bool ProcessProjectWithSimpleVarRedefinitions(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = n;
      ProjectOp op1 = (ProjectOp) n.Op;
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
          {
            flag = true;
            break;
          }
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
          transformationRulesContext.AddVarMapping(op2.Var, op3.Var);
        }
        else
          args.Add(child);
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node = command.CreateNode((Op) command.CreateVarDefListOp(), args);
      n.Child1 = node;
      return true;
    }

    private static bool ProcessProjectOpWithNullSentinel(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = n;
      ProjectOp op = (ProjectOp) n.Op;
      if (n.Child1.Children.Where<System.Data.Entity.Core.Query.InternalTrees.Node>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, bool>) (c => c.Child0.Op.OpType == OpType.NullSentinel)).Count<System.Data.Entity.Core.Query.InternalTrees.Node>() == 0)
        return false;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Command command = transformationRulesContext.Command;
      ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(n.Child0);
      bool flag1 = false;
      bool nullSentinelValue = transformationRulesContext.CanChangeNullSentinelValue;
      Var int32Var;
      if (!nullSentinelValue || !TransformationRulesContext.TryGetInt32Var((IEnumerable<Var>) extendedNodeInfo.NonNullableDefinitions, out int32Var))
      {
        flag1 = true;
        if (!nullSentinelValue || !TransformationRulesContext.TryGetInt32Var(n.Child1.Children.Where<System.Data.Entity.Core.Query.InternalTrees.Node>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, bool>) (child =>
        {
          if (child.Child0.Op.OpType != OpType.Constant)
            return child.Child0.Op.OpType == OpType.InternalConstant;
          return true;
        })).Select<System.Data.Entity.Core.Query.InternalTrees.Node, Var>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, Var>) (child => ((VarDefOp) child.Op).Var)), out int32Var))
        {
          int32Var = n.Child1.Children.Where<System.Data.Entity.Core.Query.InternalTrees.Node>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, bool>) (child => child.Child0.Op.OpType == OpType.NullSentinel)).Select<System.Data.Entity.Core.Query.InternalTrees.Node, Var>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, Var>) (child => ((VarDefOp) child.Op).Var)).FirstOrDefault<Var>();
          if (int32Var == null)
            return false;
        }
      }
      bool flag2 = false;
      for (int index = n.Child1.Children.Count - 1; index >= 0; --index)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node child = n.Child1.Children[index];
        if (child.Child0.Op.OpType == OpType.NullSentinel)
        {
          if (!flag1)
          {
            VarRefOp varRefOp = command.CreateVarRefOp(int32Var);
            child.Child0 = command.CreateNode((Op) varRefOp);
            command.RecomputeNodeInfo(child);
            flag2 = true;
          }
          else if (!int32Var.Equals((object) ((VarDefOp) child.Op).Var))
          {
            op.Outputs.Clear(((VarDefOp) child.Op).Var);
            n.Child1.Children.RemoveAt(index);
            transformationRulesContext.AddVarMapping(((VarDefOp) child.Op).Var, int32Var);
            flag2 = true;
          }
        }
      }
      if (flag2)
        command.RecomputeNodeInfo(n.Child1);
      return flag2;
    }
  }
}
