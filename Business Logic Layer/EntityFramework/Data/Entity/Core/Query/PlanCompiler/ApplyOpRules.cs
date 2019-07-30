// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ApplyOpRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class ApplyOpRules
  {
    internal static readonly PatternMatchRule Rule_CrossApplyOverFilter = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyOverFilter));
    internal static readonly PatternMatchRule Rule_OuterApplyOverFilter = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) OuterApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyOverFilter));
    internal static readonly PatternMatchRule Rule_OuterApplyOverProjectInternalConstantOverFilter = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) OuterApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
        {
          new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
          new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
        }),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) VarDefListOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
        {
          new System.Data.Entity.Core.Query.InternalTrees.Node((Op) VarDefOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
          {
            new System.Data.Entity.Core.Query.InternalTrees.Node((Op) InternalConstantOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
          })
        })
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessOuterApplyOverDummyProjectOverFilter));
    internal static readonly PatternMatchRule Rule_OuterApplyOverProjectNullSentinelOverFilter = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) OuterApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) FilterOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
        {
          new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
          new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
        }),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) VarDefListOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
        {
          new System.Data.Entity.Core.Query.InternalTrees.Node((Op) VarDefOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[1]
          {
            new System.Data.Entity.Core.Query.InternalTrees.Node((Op) NullSentinelOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
          })
        })
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessOuterApplyOverDummyProjectOverFilter));
    internal static readonly PatternMatchRule Rule_CrossApplyOverProject = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessCrossApplyOverProject));
    internal static readonly PatternMatchRule Rule_OuterApplyOverProject = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) OuterApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ProjectOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessOuterApplyOverProject));
    internal static readonly PatternMatchRule Rule_CrossApplyOverAnything = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyOverAnything));
    internal static readonly PatternMatchRule Rule_OuterApplyOverAnything = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) OuterApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyOverAnything));
    internal static readonly PatternMatchRule Rule_CrossApplyIntoScalarSubquery = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyIntoScalarSubquery));
    internal static readonly PatternMatchRule Rule_OuterApplyIntoScalarSubquery = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) OuterApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyIntoScalarSubquery));
    internal static readonly PatternMatchRule Rule_CrossApplyOverLeftOuterJoinOverSingleRowTable = new PatternMatchRule(new System.Data.Entity.Core.Query.InternalTrees.Node((Op) CrossApplyOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[2]
    {
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
      new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeftOuterJoinOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[3]
      {
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) SingleRowTableOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) LeafOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0]),
        new System.Data.Entity.Core.Query.InternalTrees.Node((Op) ConstantPredicateOp.Pattern, new System.Data.Entity.Core.Query.InternalTrees.Node[0])
      })
    }), new System.Data.Entity.Core.Query.InternalTrees.Rule.ProcessNodeDelegate(ApplyOpRules.ProcessCrossApplyOverLeftOuterJoinOverSingleRowTable));
    internal static readonly System.Data.Entity.Core.Query.InternalTrees.Rule[] Rules = new System.Data.Entity.Core.Query.InternalTrees.Rule[11]
    {
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_CrossApplyOverAnything,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_CrossApplyOverFilter,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_CrossApplyOverProject,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_OuterApplyOverAnything,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_OuterApplyOverProjectInternalConstantOverFilter,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_OuterApplyOverProjectNullSentinelOverFilter,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_OuterApplyOverProject,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_OuterApplyOverFilter,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_CrossApplyOverLeftOuterJoinOverSingleRowTable,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_CrossApplyIntoScalarSubquery,
      (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_OuterApplyIntoScalarSubquery
    };

    private static bool ProcessApplyOverFilter(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node applyNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = applyNode;
      if (((TransformationRulesContext) context).PlanCompiler.TransformationsDeferred)
        return false;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = applyNode.Child1;
      Command command = context.Command;
      if (command.GetNodeInfo(child1.Child0).ExternalReferences.Overlaps(command.GetExtendedNodeInfo(applyNode.Child0).Definitions))
        return false;
      JoinBaseOp joinBaseOp = applyNode.Op.OpType != OpType.CrossApply ? (JoinBaseOp) command.CreateLeftOuterJoinOp() : (JoinBaseOp) command.CreateInnerJoinOp();
      newNode = command.CreateNode((Op) joinBaseOp, applyNode.Child0, child1.Child0, child1.Child1);
      return true;
    }

    private static bool ProcessOuterApplyOverDummyProjectOverFilter(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node applyNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = applyNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = applyNode.Child1;
      ProjectOp op = (ProjectOp) child1.Op;
      System.Data.Entity.Core.Query.InternalTrees.Node child0_1 = child1.Child0;
      System.Data.Entity.Core.Query.InternalTrees.Node child0_2 = child0_1.Child0;
      Command command = context.Command;
      ExtendedNodeInfo extendedNodeInfo1 = command.GetExtendedNodeInfo(child0_2);
      ExtendedNodeInfo extendedNodeInfo2 = command.GetExtendedNodeInfo(applyNode.Child0);
      if (op.Outputs.Overlaps(extendedNodeInfo2.Definitions) || extendedNodeInfo1.ExternalReferences.Overlaps(extendedNodeInfo2.Definitions))
        return false;
      bool flag1 = false;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Var int32Var;
      bool flag2;
      if (TransformationRulesContext.TryGetInt32Var((IEnumerable<Var>) extendedNodeInfo1.NonNullableDefinitions, out int32Var))
      {
        flag2 = true;
      }
      else
      {
        int32Var = extendedNodeInfo1.NonNullableDefinitions.First;
        flag2 = false;
      }
      System.Data.Entity.Core.Query.InternalTrees.Node node1;
      if (int32Var != null)
      {
        flag1 = true;
        System.Data.Entity.Core.Query.InternalTrees.Node child0_3 = child1.Child1.Child0;
        child0_3.Child0 = child0_3.Child0.Op.OpType != OpType.NullSentinel || !flag2 || !transformationRulesContext.CanChangeNullSentinelValue ? transformationRulesContext.BuildNullIfExpression(int32Var, child0_3.Child0) : context.Command.CreateNode((Op) context.Command.CreateVarRefOp(int32Var));
        command.RecomputeNodeInfo(child0_3);
        command.RecomputeNodeInfo(child1.Child1);
        node1 = child0_2;
      }
      else
      {
        node1 = child1;
        foreach (Var externalReference in command.GetNodeInfo(child0_1.Child1).ExternalReferences)
        {
          if (extendedNodeInfo1.Definitions.IsSet(externalReference))
            op.Outputs.Set(externalReference);
        }
        child1.Child0 = child0_2;
      }
      context.Command.RecomputeNodeInfo(child1);
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = command.CreateNode((Op) command.CreateLeftOuterJoinOp(), applyNode.Child0, node1, child0_1.Child1);
      if (flag1)
      {
        ExtendedNodeInfo extendedNodeInfo3 = command.GetExtendedNodeInfo(node2);
        child1.Child0 = node2;
        op.Outputs.Or(extendedNodeInfo3.Definitions);
        newNode = child1;
      }
      else
        newNode = node2;
      return true;
    }

    private static bool ProcessCrossApplyOverProject(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node applyNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = applyNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = applyNode.Child1;
      ProjectOp op = (ProjectOp) child1.Op;
      Command command = context.Command;
      ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(applyNode);
      VarVec varVec = command.CreateVarVec(op.Outputs);
      varVec.Or(extendedNodeInfo.Definitions);
      op.Outputs.InitFrom(varVec);
      applyNode.Child1 = child1.Child0;
      context.Command.RecomputeNodeInfo(applyNode);
      child1.Child0 = applyNode;
      newNode = child1;
      return true;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VarDefOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private static bool ProcessOuterApplyOverProject(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node applyNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = applyNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child1_1 = applyNode.Child1;
      System.Data.Entity.Core.Query.InternalTrees.Node child1_2 = child1_1.Child1;
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      Var computedVar = context.Command.GetExtendedNodeInfo(child1_1.Child0).NonNullableDefinitions.First;
      if (computedVar == null && child1_2.Children.Count == 1 && (child1_2.Child0.Child0.Op.OpType == OpType.InternalConstant || child1_2.Child0.Child0.Op.OpType == OpType.NullSentinel))
        return false;
      Command command = context.Command;
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
      InternalConstantOp internalConstantOp = (InternalConstantOp) null;
      ExtendedNodeInfo extendedNodeInfo1 = command.GetExtendedNodeInfo(child1_1.Child0);
      bool flag = false;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in child1_2.Children)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(child.Op.OpType == OpType.VarDef, "Expected VarDefOp. Found " + (object) child.Op.OpType + " instead");
        VarRefOp op = child.Child0.Op as VarRefOp;
        if (op == null || !extendedNodeInfo1.Definitions.IsSet(op.Var))
        {
          if (computedVar == null)
          {
            internalConstantOp = command.CreateInternalConstantOp(command.IntegerType, (object) 1);
            System.Data.Entity.Core.Query.InternalTrees.Node node2 = command.CreateNode((Op) internalConstantOp);
            System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = command.CreateVarDefListNode(node2, out computedVar);
            ProjectOp projectOp = command.CreateProjectOp(computedVar);
            projectOp.Outputs.Or(extendedNodeInfo1.Definitions);
            node1 = command.CreateNode((Op) projectOp, child1_1.Child0, varDefListNode);
          }
          System.Data.Entity.Core.Query.InternalTrees.Node node3 = internalConstantOp == null || !internalConstantOp.IsEquivalent(child.Child0.Op) && child.Child0.Op.OpType != OpType.NullSentinel ? transformationRulesContext.BuildNullIfExpression(computedVar, child.Child0) : command.CreateNode((Op) command.CreateVarRefOp(computedVar));
          child.Child0 = node3;
          command.RecomputeNodeInfo(child);
          flag = true;
        }
      }
      if (flag)
        command.RecomputeNodeInfo(child1_2);
      applyNode.Child1 = node1 ?? child1_1.Child0;
      command.RecomputeNodeInfo(applyNode);
      child1_1.Child0 = applyNode;
      ExtendedNodeInfo extendedNodeInfo2 = command.GetExtendedNodeInfo(applyNode.Child0);
      ((ProjectOp) child1_1.Op).Outputs.Or(extendedNodeInfo2.Definitions);
      newNode = child1_1;
      return true;
    }

    private static bool ProcessApplyOverAnything(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node applyNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = applyNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child0 = applyNode.Child0;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = applyNode.Child1;
      ApplyBaseOp applyBaseOp = (ApplyBaseOp) applyNode.Op;
      Command command = context.Command;
      ExtendedNodeInfo extendedNodeInfo1 = command.GetExtendedNodeInfo(child1);
      ExtendedNodeInfo extendedNodeInfo2 = command.GetExtendedNodeInfo(child0);
      bool flag = false;
      if (applyBaseOp.OpType == OpType.OuterApply && extendedNodeInfo1.MinRows >= RowCount.One)
      {
        applyBaseOp = (ApplyBaseOp) command.CreateCrossApplyOp();
        flag = true;
      }
      if (extendedNodeInfo1.ExternalReferences.Overlaps(extendedNodeInfo2.Definitions))
      {
        if (!flag)
          return false;
        newNode = command.CreateNode((Op) applyBaseOp, child0, child1);
        return true;
      }
      if (applyBaseOp.OpType == OpType.CrossApply)
      {
        newNode = command.CreateNode((Op) command.CreateCrossJoinOp(), child0, child1);
      }
      else
      {
        LeftOuterJoinOp leftOuterJoinOp = command.CreateLeftOuterJoinOp();
        ConstantPredicateOp trueOp = command.CreateTrueOp();
        System.Data.Entity.Core.Query.InternalTrees.Node node = command.CreateNode((Op) trueOp);
        newNode = command.CreateNode((Op) leftOuterJoinOp, child0, child1, node);
      }
      return true;
    }

    private static bool ProcessApplyIntoScalarSubquery(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node applyNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      Command command = context.Command;
      ExtendedNodeInfo extendedNodeInfo1 = command.GetExtendedNodeInfo(applyNode.Child1);
      OpType opType = applyNode.Op.OpType;
      if (!ApplyOpRules.CanRewriteApply(applyNode.Child1, extendedNodeInfo1, opType))
      {
        newNode = applyNode;
        return false;
      }
      ExtendedNodeInfo extendedNodeInfo2 = command.GetExtendedNodeInfo(applyNode.Child0);
      Var first = extendedNodeInfo1.Definitions.First;
      VarVec varVec = command.CreateVarVec(extendedNodeInfo2.Definitions);
      TransformationRulesContext transformationRulesContext = (TransformationRulesContext) context;
      transformationRulesContext.RemapSubtree(applyNode.Child1);
      ApplyOpRules.VarDefinitionRemapper.RemapSubtree(applyNode.Child1, command, first);
      System.Data.Entity.Core.Query.InternalTrees.Node node = command.CreateNode((Op) command.CreateElementOp(first.Type), applyNode.Child1);
      Var computedVar;
      System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode = command.CreateVarDefListNode(node, out computedVar);
      varVec.Set(computedVar);
      newNode = command.CreateNode((Op) command.CreateProjectOp(varVec), applyNode.Child0, varDefListNode);
      transformationRulesContext.AddVarMapping(first, computedVar);
      return true;
    }

    private static bool CanRewriteApply(
      System.Data.Entity.Core.Query.InternalTrees.Node rightChild,
      ExtendedNodeInfo applyRightChildNodeInfo,
      OpType applyKind)
    {
      return applyRightChildNodeInfo.Definitions.Count == 1 && applyRightChildNodeInfo.MaxRows == RowCount.One && (applyKind != OpType.CrossApply || applyRightChildNodeInfo.MinRows == RowCount.One) && ApplyOpRules.OutputCountVisitor.CountOutputs(rightChild) == 1;
    }

    private static bool ProcessCrossApplyOverLeftOuterJoinOverSingleRowTable(
      RuleProcessingContext context,
      System.Data.Entity.Core.Query.InternalTrees.Node applyNode,
      out System.Data.Entity.Core.Query.InternalTrees.Node newNode)
    {
      newNode = applyNode;
      System.Data.Entity.Core.Query.InternalTrees.Node child1 = applyNode.Child1;
      if (((ConstantPredicateOp) child1.Child2.Op).IsFalse)
        return false;
      applyNode.Op = (Op) context.Command.CreateOuterApplyOp();
      applyNode.Child1 = child1.Child1;
      return true;
    }

    internal class OutputCountVisitor : BasicOpVisitorOfT<int>
    {
      internal static int CountOutputs(System.Data.Entity.Core.Query.InternalTrees.Node node)
      {
        return new ApplyOpRules.OutputCountVisitor().VisitNode(node);
      }

      internal int VisitChildren(System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        int num = 0;
        foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
          num += this.VisitNode(child);
        return num;
      }

      protected override int VisitDefault(System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        return this.VisitChildren(n);
      }

      protected override int VisitSetOp(SetOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        return op.Outputs.Count;
      }

      public override int Visit(DistinctOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        return op.Keys.Count;
      }

      public override int Visit(FilterOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        return this.VisitNode(n.Child0);
      }

      public override int Visit(GroupByOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        return op.Outputs.Count;
      }

      public override int Visit(ProjectOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        return op.Outputs.Count;
      }

      public override int Visit(ScanTableOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        return op.Table.Columns.Count;
      }

      public override int Visit(SingleRowTableOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        return 0;
      }

      protected override int VisitSortOp(SortBaseOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        return this.VisitNode(n.Child0);
      }
    }

    internal class VarDefinitionRemapper : VarRemapper
    {
      private readonly Var m_oldVar;

      private VarDefinitionRemapper(Var oldVar, Command command)
        : base(command)
      {
        this.m_oldVar = oldVar;
      }

      internal static void RemapSubtree(System.Data.Entity.Core.Query.InternalTrees.Node root, Command command, Var oldVar)
      {
        new ApplyOpRules.VarDefinitionRemapper(oldVar, command).RemapSubtree(root);
      }

      internal override void RemapSubtree(System.Data.Entity.Core.Query.InternalTrees.Node subTree)
      {
        foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in subTree.Children)
          this.RemapSubtree(child);
        this.VisitNode(subTree);
        this.m_command.RecomputeNodeInfo(subTree);
      }

      public override void Visit(VarDefOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        if (op.Var != this.m_oldVar)
          return;
        Var computedVar = (Var) this.m_command.CreateComputedVar(n.Child0.Op.Type);
        n.Op = (Op) this.m_command.CreateVarDefOp(computedVar);
        this.AddMapping(this.m_oldVar, computedVar);
      }

      public override void Visit(ScanTableOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        if (!op.Table.Columns.Contains(this.m_oldVar))
          return;
        ScanTableOp scanTableOp = this.m_command.CreateScanTableOp(op.Table.TableMetadata);
        this.m_command.CreateVarDefListOp();
        for (int index = 0; index < op.Table.Columns.Count; ++index)
          this.AddMapping(op.Table.Columns[index], scanTableOp.Table.Columns[index]);
        n.Op = (Op) scanTableOp;
      }

      protected override void VisitSetOp(SetOp op, System.Data.Entity.Core.Query.InternalTrees.Node n)
      {
        base.VisitSetOp(op, n);
        if (!op.Outputs.IsSet(this.m_oldVar))
          return;
        Var setOpVar = (Var) this.m_command.CreateSetOpVar(this.m_oldVar.Type);
        op.Outputs.Clear(this.m_oldVar);
        op.Outputs.Set(setOpVar);
        this.RemapVarMapKey(op.VarMap[0], setOpVar);
        this.RemapVarMapKey(op.VarMap[1], setOpVar);
        this.AddMapping(this.m_oldVar, setOpVar);
      }

      private void RemapVarMapKey(VarMap varMap, Var newVar)
      {
        Var var = varMap[this.m_oldVar];
        varMap.Remove(this.m_oldVar);
        varMap.Add(newVar, var);
      }
    }
  }
}
