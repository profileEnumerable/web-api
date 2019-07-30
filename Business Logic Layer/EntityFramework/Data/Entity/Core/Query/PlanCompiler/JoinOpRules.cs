// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.JoinOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class JoinOpRules
  {
    internal static readonly PatternMatchRule Rule_CrossJoinOverProject1 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
    internal static readonly PatternMatchRule Rule_CrossJoinOverProject2 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
    internal static readonly PatternMatchRule Rule_InnerJoinOverProject1 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InnerJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
    internal static readonly PatternMatchRule Rule_InnerJoinOverProject2 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InnerJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
    internal static readonly PatternMatchRule Rule_OuterJoinOverProject2 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeftOuterJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
    internal static readonly PatternMatchRule Rule_CrossJoinOverFilter1 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
    internal static readonly PatternMatchRule Rule_CrossJoinOverFilter2 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
    internal static readonly PatternMatchRule Rule_InnerJoinOverFilter1 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InnerJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
    internal static readonly PatternMatchRule Rule_InnerJoinOverFilter2 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InnerJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
    internal static readonly PatternMatchRule Rule_OuterJoinOverFilter2 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeftOuterJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      }),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
    internal static readonly PatternMatchRule Rule_CrossJoinOverSingleRowTable1 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) SingleRowTableOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverSingleRowTable));
    internal static readonly PatternMatchRule Rule_CrossJoinOverSingleRowTable2 = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) SingleRowTableOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverSingleRowTable));
    internal static readonly PatternMatchRule Rule_LeftOuterJoinOverSingleRowTable = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeftOuterJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) SingleRowTableOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverSingleRowTable));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[13]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_CrossJoinOverProject1,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_CrossJoinOverProject2,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_InnerJoinOverProject1,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_InnerJoinOverProject2,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_OuterJoinOverProject2,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_CrossJoinOverFilter1,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_CrossJoinOverFilter2,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_InnerJoinOverFilter1,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_InnerJoinOverFilter2,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_OuterJoinOverFilter2,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_CrossJoinOverSingleRowTable1,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_CrossJoinOverSingleRowTable2,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_LeftOuterJoinOverSingleRowTable
    };

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "non-LeftOuterJoin")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static bool ProcessJoinOverProject(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node joinNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = joinNode;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Command command = transformationRulesContext.Command;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = joinNode.HasChild2 ? joinNode.Child2 : (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      Dictionary<Var, int> varRefMap = new Dictionary<Var, int>();
      if (node1 != null && !transformationRulesContext.IsScalarOpTree(node1, varRefMap))
        return false;
      VarVec varVec1 = command.CreateVarVec();
      List<System.Data.Entity.Core.Query.InternalTrees.Node> args = new List<System.Data.Entity.Core.Query.InternalTrees.Node>();
      if (joinNode.Op.OpType != OpType.LeftOuterJoin && joinNode.Child0.Op.OpType == OpType.Project && joinNode.Child1.Op.OpType == OpType.Project)
      {
        ProjectOp op1 = (ProjectOp) joinNode.Child0.Op;
        ProjectOp op2 = (ProjectOp) joinNode.Child1.Op;
        Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varMap1 = transformationRulesContext.GetVarMap(joinNode.Child0.Child1, varRefMap);
        Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varMap2 = transformationRulesContext.GetVarMap(joinNode.Child1.Child1, varRefMap);
        if (varMap1 == null || varMap2 == null)
          return false;
        System.Data.Entity.Core.Query.InternalTrees.Node node2;
        if (node1 != null)
        {
          System.Data.Entity.Core.Query.InternalTrees.Node node3 = transformationRulesContext.ReMap(node1, varMap1);
          System.Data.Entity.Core.Query.InternalTrees.Node node4 = transformationRulesContext.ReMap(node3, varMap2);
          node2 = context.Command.CreateNode(joinNode.Op, joinNode.Child0.Child0, joinNode.Child1.Child0, node4);
        }
        else
          node2 = context.Command.CreateNode(joinNode.Op, joinNode.Child0.Child0, joinNode.Child1.Child0);
        varVec1.InitFrom(op1.Outputs);
        foreach (Var output in op2.Outputs)
          varVec1.Set(output);
        ProjectOp projectOp = command.CreateProjectOp(varVec1);
        args.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) joinNode.Child0.Child1.Children);
        args.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) joinNode.Child1.Child1.Children);
        System.Data.Entity.Core.Query.InternalTrees.Node node5 = command.CreateNode((Op) command.CreateVarDefListOp(), args);
        System.Data.Entity.Core.Query.InternalTrees.Node node6 = command.CreateNode((Op) projectOp, node2, node5);
        newNode = node6;
        return true;
      }
      int index1;
      int index2;
      if (joinNode.Child0.Op.OpType == OpType.Project)
      {
        index1 = 0;
        index2 = 1;
      }
      else
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(joinNode.Op.OpType != OpType.LeftOuterJoin, "unexpected non-LeftOuterJoin");
        index1 = 1;
        index2 = 0;
      }
      System.Data.Entity.Core.Query.InternalTrees.Node child = joinNode.Children[index1];
      ProjectOp op = child.Op as ProjectOp;
      Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varMap = transformationRulesContext.GetVarMap(child.Child1, varRefMap);
      if (varMap == null)
        return false;
      ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(joinNode.Children[index2]);
      VarVec varVec2 = command.CreateVarVec(op.Outputs);
      varVec2.Or(extendedNodeInfo.Definitions);
      op.Outputs.InitFrom(varVec2);
      if (node1 != null)
      {
        System.Data.Entity.Core.Query.InternalTrees.Node node2 = transformationRulesContext.ReMap(node1, varMap);
        joinNode.Child2 = node2;
      }
      joinNode.Children[index1] = child.Child0;
      context.Command.RecomputeNodeInfo(joinNode);
      newNode = context.Command.CreateNode((Op) op, joinNode, child.Child1);
      return true;
    }

    private static bool ProcessJoinOverFilter(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node joinNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = joinNode;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Command command = transformationRulesContext.Command;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = joinNode.Child0;
      if (joinNode.Child0.Op.OpType == OpType.Filter)
      {
        node1 = joinNode.Child0.Child1;
        child0 = joinNode.Child0.Child0;
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = joinNode.Child1;
      if (joinNode.Child1.Op.OpType == OpType.Filter && joinNode.Op.OpType != OpType.LeftOuterJoin)
      {
        node1 = node1 != null ? command.CreateNode((Op) command.CreateConditionalOp(OpType.And), node1, joinNode.Child1.Child1) : joinNode.Child1.Child1;
        node2 = joinNode.Child1.Child0;
      }
      if (node1 == null)
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = joinNode.Op.OpType != OpType.CrossJoin ? command.CreateNode(joinNode.Op, child0, node2, joinNode.Child2) : command.CreateNode(joinNode.Op, child0, node2);
      FilterOp filterOp = command.CreateFilterOp();
      newNode = command.CreateNode((Op) filterOp, node3, node1);
      transformationRulesContext.SuppressFilterPushdown(newNode);
      return true;
    }

    private static bool ProcessJoinOverSingleRowTable(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node joinNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = joinNode;
      newNode = joinNode.Child0.Op.OpType != OpType.SingleRowTable ? joinNode.Child0 : joinNode.Child1;
      return true;
    }
  }
}
