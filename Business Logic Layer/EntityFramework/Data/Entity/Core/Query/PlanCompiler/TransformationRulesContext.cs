// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.TransformationRulesContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class TransformationRulesContext : RuleProcessingContext
  {
    private readonly Stack<System.Data.Entity.Core.Query.InternalTrees.Node> m_relOpAncestors = new Stack<System.Data.Entity.Core.Query.InternalTrees.Node>();
    private readonly System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler m_compilerState;
    private readonly VarRemapper m_remapper;
    private readonly Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node> m_suppressions;
    private readonly VarVec m_remappedVars;
    private bool m_projectionPrunningRequired;
    private bool m_reapplyNullabilityRules;

    internal System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler PlanCompiler
    {
      get
      {
        return this.m_compilerState;
      }
    }

    internal bool ProjectionPrunningRequired
    {
      get
      {
        return this.m_projectionPrunningRequired;
      }
    }

    internal bool ReapplyNullabilityRules
    {
      get
      {
        return this.m_reapplyNullabilityRules;
      }
    }

    internal void RemapSubtree(System.Data.Entity.Core.Query.InternalTrees.Node subTree)
    {
      this.m_remapper.RemapSubtree(subTree);
    }

    internal void AddVarMapping(Var oldVar, Var newVar)
    {
      this.m_remapper.AddMapping(oldVar, newVar);
      this.m_remappedVars.Set(oldVar);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "scalarOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal System.Data.Entity.Core.Query.InternalTrees.Node ReMap(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> varMap)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node.Op.IsScalarOp, "Expected a scalarOp: Found " + Dump.AutoString.ToString(node.Op.OpType));
      if (node.Op.OpType == OpType.VarRef)
      {
        VarRefOp op = node.Op as VarRefOp;
        System.Data.Entity.Core.Query.InternalTrees.Node node1 = (System.Data.Entity.Core.Query.InternalTrees.Node) null;
        if (!varMap.TryGetValue(op.Var, out node1))
          return node;
        node1 = this.Copy(node1);
        return node1;
      }
      for (int index = 0; index < node.Children.Count; ++index)
        node.Children[index] = this.ReMap(node.Children[index], varMap);
      this.Command.RecomputeNodeInfo(node);
      return node;
    }

    internal System.Data.Entity.Core.Query.InternalTrees.Node Copy(System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      if (node.Op.OpType == OpType.VarRef)
        return this.Command.CreateNode((Op) this.Command.CreateVarRefOp((node.Op as VarRefOp).Var));
      return OpCopier.Copy(this.Command, node);
    }

    internal bool IsScalarOpTree(System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      int nonLeafNodeCount = 0;
      return this.IsScalarOpTree(node, (Dictionary<Var, int>) null, ref nonLeafNodeCount);
    }

    internal bool IsNonNullable(Var variable)
    {
      if (variable.VarType == VarType.Parameter && !TypeSemantics.IsNullable(variable.Type))
        return true;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node relOpAncestor in this.m_relOpAncestors)
      {
        this.Command.RecomputeNodeInfo(relOpAncestor);
        ExtendedNodeInfo extendedNodeInfo = this.Command.GetExtendedNodeInfo(relOpAncestor);
        if (extendedNodeInfo.NonNullableVisibleDefinitions.IsSet(variable))
          return true;
        if (extendedNodeInfo.LocalDefinitions.IsSet(variable))
          return false;
      }
      return false;
    }

    internal bool CanChangeNullSentinelValue
    {
      get
      {
        if (this.m_compilerState.HasSortingOnNullSentinels || this.m_relOpAncestors.Any<System.Data.Entity.Core.Query.InternalTrees.Node>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, bool>) (a => TransformationRulesContext.IsOpNotSafeForNullSentinelValueChange(a.Op.OpType))))
          return false;
        foreach (System.Data.Entity.Core.Query.InternalTrees.Node node in this.m_relOpAncestors.Where<System.Data.Entity.Core.Query.InternalTrees.Node>((Func<System.Data.Entity.Core.Query.InternalTrees.Node, bool>) (a =>
        {
          if (a.Op.OpType != OpType.CrossApply)
            return a.Op.OpType == OpType.OuterApply;
          return true;
        })))
        {
          if (!this.m_relOpAncestors.Contains(node.Child1) && TransformationRulesContext.HasOpNotSafeForNullSentinelValueChange(node.Child1))
            return false;
        }
        return true;
      }
    }

    internal static bool IsOpNotSafeForNullSentinelValueChange(OpType optype)
    {
      if (optype != OpType.Distinct && optype != OpType.GroupBy && optype != OpType.Intersect)
        return optype == OpType.Except;
      return true;
    }

    internal static bool HasOpNotSafeForNullSentinelValueChange(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      if (TransformationRulesContext.IsOpNotSafeForNullSentinelValueChange(n.Op.OpType))
        return true;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in n.Children)
      {
        if (TransformationRulesContext.HasOpNotSafeForNullSentinelValueChange(child))
          return true;
      }
      return false;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "varRef")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal bool IsScalarOpTree(System.Data.Entity.Core.Query.InternalTrees.Node node, Dictionary<Var, int> varRefMap)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(varRefMap != null, "Null varRef map");
      int nonLeafNodeCount = 0;
      return this.IsScalarOpTree(node, varRefMap, ref nonLeafNodeCount);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "varDef")]
    internal Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> GetVarMap(
      System.Data.Entity.Core.Query.InternalTrees.Node varDefListNode,
      Dictionary<Var, int> varRefMap)
    {
      VarDefListOp op1 = (VarDefListOp) varDefListNode.Op;
      Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> dictionary = new Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node>();
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in varDefListNode.Children)
      {
        VarDefOp op2 = (VarDefOp) child.Op;
        int nonLeafNodeCount = 0;
        int num = 0;
        if (!this.IsScalarOpTree(child.Child0, (Dictionary<Var, int>) null, ref nonLeafNodeCount) || nonLeafNodeCount > 100 && varRefMap != null && (varRefMap.TryGetValue(op2.Var, out num) && num > 2))
          return (Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node>) null;
        System.Data.Entity.Core.Query.InternalTrees.Node node;
        if (dictionary.TryGetValue(op2.Var, out node))
          System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node == child.Child0, "reusing varDef for different Node?");
        else
          dictionary.Add(op2.Var, child.Child0);
      }
      return dictionary;
    }

    internal System.Data.Entity.Core.Query.InternalTrees.Node BuildNullIfExpression(
      Var conditionVar,
      System.Data.Entity.Core.Query.InternalTrees.Node expr)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node1 = this.Command.CreateNode((Op) this.Command.CreateConditionalOp(OpType.IsNull), this.Command.CreateNode((Op) this.Command.CreateVarRefOp(conditionVar)));
      System.Data.Entity.Core.Query.InternalTrees.Node node2 = expr;
      System.Data.Entity.Core.Query.InternalTrees.Node node3 = this.Command.CreateNode((Op) this.Command.CreateNullOp(node2.Op.Type));
      return this.Command.CreateNode((Op) this.Command.CreateCaseOp(node2.Op.Type), node1, node3, node2);
    }

    internal void SuppressFilterPushdown(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.m_suppressions[n] = n;
    }

    internal bool IsFilterPushdownSuppressed(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      return this.m_suppressions.ContainsKey(n);
    }

    internal static bool TryGetInt32Var(IEnumerable<Var> varList, out Var int32Var)
    {
      foreach (Var var in varList)
      {
        PrimitiveTypeKind typeKind;
        if (TypeHelpers.TryGetPrimitiveTypeKind(var.Type, out typeKind) && typeKind == PrimitiveTypeKind.Int32)
        {
          int32Var = var;
          return true;
        }
      }
      int32Var = (Var) null;
      return false;
    }

    internal TransformationRulesContext(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState)
      : base(compilerState.Command)
    {
      this.m_compilerState = compilerState;
      this.m_remapper = new VarRemapper(compilerState.Command);
      this.m_suppressions = new Dictionary<System.Data.Entity.Core.Query.InternalTrees.Node, System.Data.Entity.Core.Query.InternalTrees.Node>();
      this.m_remappedVars = compilerState.Command.CreateVarVec();
    }

    internal override void PreProcess(System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      this.m_remapper.RemapNode(n);
      this.Command.RecomputeNodeInfo(n);
    }

    internal override void PreProcessSubTree(System.Data.Entity.Core.Query.InternalTrees.Node subTree)
    {
      if (subTree.Op.IsRelOp)
        this.m_relOpAncestors.Push(subTree);
      if (this.m_remappedVars.IsEmpty)
        return;
      foreach (Var externalReference in this.Command.GetNodeInfo(subTree).ExternalReferences)
      {
        if (this.m_remappedVars.IsSet(externalReference))
        {
          this.m_remapper.RemapSubtree(subTree);
          break;
        }
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RelOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal override void PostProcessSubTree(System.Data.Entity.Core.Query.InternalTrees.Node subtree)
    {
      if (!subtree.Op.IsRelOp)
        return;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this.m_relOpAncestors.Count != 0, "The RelOp ancestors stack is empty when post processing a RelOp subtree");
      System.Data.Entity.Core.Query.InternalTrees.Node node = this.m_relOpAncestors.Pop();
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(object.ReferenceEquals((object) subtree, (object) node), "The popped ancestor is not equal to the root of the subtree being post processed");
    }

    internal override void PostProcess(System.Data.Entity.Core.Query.InternalTrees.Node n, System.Data.Entity.Core.Query.InternalTrees.Rule rule)
    {
      if (rule == null)
        return;
      if (!this.m_projectionPrunningRequired && TransformationRules.RulesRequiringProjectionPruning.Contains(rule))
        this.m_projectionPrunningRequired = true;
      if (!this.m_reapplyNullabilityRules && TransformationRules.RulesRequiringNullabilityRulesToBeReapplied.Contains(rule))
        this.m_reapplyNullabilityRules = true;
      this.Command.RecomputeNodeInfo(n);
    }

    internal override int GetHashCode(System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      return this.Command.GetNodeInfo(node).HashValue;
    }

    private bool IsScalarOpTree(
      System.Data.Entity.Core.Query.InternalTrees.Node node,
      Dictionary<Var, int> varRefMap,
      ref int nonLeafNodeCount)
    {
      if (!node.Op.IsScalarOp)
        return false;
      if (node.HasChild0)
        ++nonLeafNodeCount;
      if (varRefMap != null && node.Op.OpType == OpType.VarRef)
      {
        VarRefOp op = (VarRefOp) node.Op;
        int num1;
        int num2 = varRefMap.TryGetValue(op.Var, out num1) ? num1 + 1 : 1;
        varRefMap[op.Var] = num2;
      }
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node child in node.Children)
      {
        if (!this.IsScalarOpTree(child, varRefMap, ref nonLeafNodeCount))
          return false;
      }
      return true;
    }
  }
}
