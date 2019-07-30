// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.TransformationRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class TransformationRules
  {
    internal static readonly ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>> AllRulesTable = TransformationRules.BuildLookupTableForRules((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) TransformationRules.AllRules);
    internal static readonly ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>> ProjectRulesTable = TransformationRules.BuildLookupTableForRules((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) ProjectOpRules.Rules);
    internal static readonly ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>> PostJoinEliminationRulesTable = TransformationRules.BuildLookupTableForRules((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) TransformationRules.PostJoinEliminationRules);
    internal static readonly ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>> NullabilityRulesTable = TransformationRules.BuildLookupTableForRules((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) TransformationRules.NullabilityRules);
    internal static readonly HashSet<System.Data.Entity.Core.Query.InternalTrees.Rule> RulesRequiringProjectionPruning = TransformationRules.InitializeRulesRequiringProjectionPruning();
    internal static readonly HashSet<System.Data.Entity.Core.Query.InternalTrees.Rule> RulesRequiringNullabilityRulesToBeReapplied = TransformationRules.InitializeRulesRequiringNullabilityRulesToBeReapplied();
    internal static readonly ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>> NullSemanticsRulesTable = TransformationRules.BuildLookupTableForRules((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) TransformationRules.NullSemanticsRules);
    private static List<System.Data.Entity.Core.Query.InternalTrees.Rule> allRules;
    private static List<System.Data.Entity.Core.Query.InternalTrees.Rule> postJoinEliminationRules;
    private static List<System.Data.Entity.Core.Query.InternalTrees.Rule> nullabilityRules;
    private static List<System.Data.Entity.Core.Query.InternalTrees.Rule> nullSemanticsRules;

    private static List<System.Data.Entity.Core.Query.InternalTrees.Rule> AllRules
    {
      get
      {
        if (TransformationRules.allRules == null)
        {
          TransformationRules.allRules = new List<System.Data.Entity.Core.Query.InternalTrees.Rule>();
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) ScalarOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) FilterOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) ProjectOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) ApplyOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) JoinOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) SingleRowOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) SetOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) GroupByOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) SortOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) ConstrainedSortOpRules.Rules);
          TransformationRules.allRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) DistinctOpRules.Rules);
        }
        return TransformationRules.allRules;
      }
    }

    private static List<System.Data.Entity.Core.Query.InternalTrees.Rule> PostJoinEliminationRules
    {
      get
      {
        if (TransformationRules.postJoinEliminationRules == null)
        {
          TransformationRules.postJoinEliminationRules = new List<System.Data.Entity.Core.Query.InternalTrees.Rule>();
          TransformationRules.postJoinEliminationRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) ProjectOpRules.Rules);
          TransformationRules.postJoinEliminationRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) DistinctOpRules.Rules);
          TransformationRules.postJoinEliminationRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) FilterOpRules.Rules);
          TransformationRules.postJoinEliminationRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) ApplyOpRules.Rules);
          TransformationRules.postJoinEliminationRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) JoinOpRules.Rules);
          TransformationRules.postJoinEliminationRules.AddRange((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule>) TransformationRules.NullabilityRules);
        }
        return TransformationRules.postJoinEliminationRules;
      }
    }

    private static List<System.Data.Entity.Core.Query.InternalTrees.Rule> NullabilityRules
    {
      get
      {
        if (TransformationRules.nullabilityRules == null)
        {
          TransformationRules.nullabilityRules = new List<System.Data.Entity.Core.Query.InternalTrees.Rule>();
          TransformationRules.nullabilityRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_IsNullOverVarRef);
          TransformationRules.nullabilityRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_AndOverConstantPred1);
          TransformationRules.nullabilityRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_AndOverConstantPred2);
          TransformationRules.nullabilityRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_SimplifyCase);
          TransformationRules.nullabilityRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_NotOverConstantPred);
        }
        return TransformationRules.nullabilityRules;
      }
    }

    private static List<System.Data.Entity.Core.Query.InternalTrees.Rule> NullSemanticsRules
    {
      get
      {
        if (TransformationRules.nullSemanticsRules == null)
        {
          TransformationRules.nullSemanticsRules = new List<System.Data.Entity.Core.Query.InternalTrees.Rule>();
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_IsNullOverAnything);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_NullCast);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_EqualsOverConstant);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_AndOverConstantPred1);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_AndOverConstantPred2);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_OrOverConstantPred1);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_OrOverConstantPred2);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_NotOverConstantPred);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_LikeOverConstants);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_SimplifyCase);
          TransformationRules.nullSemanticsRules.Add((System.Data.Entity.Core.Query.InternalTrees.Rule) ScalarOpRules.Rule_FlattenCase);
        }
        return TransformationRules.nullSemanticsRules;
      }
    }

    private static ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>> BuildLookupTableForRules(
      IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Rule> rules)
    {
      ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule> readOnlyCollection = new ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>((IList<System.Data.Entity.Core.Query.InternalTrees.Rule>) new System.Data.Entity.Core.Query.InternalTrees.Rule[0]);
      List<System.Data.Entity.Core.Query.InternalTrees.Rule>[] ruleListArray = new List<System.Data.Entity.Core.Query.InternalTrees.Rule>[73];
      foreach (System.Data.Entity.Core.Query.InternalTrees.Rule rule in rules)
      {
        List<System.Data.Entity.Core.Query.InternalTrees.Rule> ruleList = ruleListArray[(int) rule.RuleOpType];
        if (ruleList == null)
        {
          ruleList = new List<System.Data.Entity.Core.Query.InternalTrees.Rule>();
          ruleListArray[(int) rule.RuleOpType] = ruleList;
        }
        ruleList.Add(rule);
      }
      ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>[] readOnlyCollectionArray = new ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>[ruleListArray.Length];
      for (int index = 0; index < ruleListArray.Length; ++index)
        readOnlyCollectionArray[index] = ruleListArray[index] == null ? readOnlyCollection : new ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>((IList<System.Data.Entity.Core.Query.InternalTrees.Rule>) ruleListArray[index].ToArray());
      return new ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>>((IList<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>>) readOnlyCollectionArray);
    }

    private static HashSet<System.Data.Entity.Core.Query.InternalTrees.Rule> InitializeRulesRequiringProjectionPruning()
    {
      return new HashSet<System.Data.Entity.Core.Query.InternalTrees.Rule>()
      {
        (System.Data.Entity.Core.Query.InternalTrees.Rule) ApplyOpRules.Rule_OuterApplyOverProject,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_CrossJoinOverProject1,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_CrossJoinOverProject2,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_InnerJoinOverProject1,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_InnerJoinOverProject2,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) JoinOpRules.Rule_OuterJoinOverProject2,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) ProjectOpRules.Rule_ProjectWithNoLocalDefs,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverProject,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterWithConstantPredicate,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) GroupByOpRules.Rule_GroupByOverProject,
        (System.Data.Entity.Core.Query.InternalTrees.Rule) GroupByOpRules.Rule_GroupByOpWithSimpleVarRedefinitions
      };
    }

    private static HashSet<System.Data.Entity.Core.Query.InternalTrees.Rule> InitializeRulesRequiringNullabilityRulesToBeReapplied()
    {
      return new HashSet<System.Data.Entity.Core.Query.InternalTrees.Rule>()
      {
        (System.Data.Entity.Core.Query.InternalTrees.Rule) FilterOpRules.Rule_FilterOverLeftOuterJoin
      };
    }

    internal static bool Process(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState, TransformationRulesGroup rulesGroup)
    {
      ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>> rulesTable = (ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>>) null;
      switch (rulesGroup)
      {
        case TransformationRulesGroup.All:
          rulesTable = TransformationRules.AllRulesTable;
          break;
        case TransformationRulesGroup.Project:
          rulesTable = TransformationRules.ProjectRulesTable;
          break;
        case TransformationRulesGroup.PostJoinElimination:
          rulesTable = TransformationRules.PostJoinEliminationRulesTable;
          break;
        case TransformationRulesGroup.NullSemantics:
          rulesTable = TransformationRules.NullSemanticsRulesTable;
          break;
      }
      bool projectionPruningRequired1;
      if (TransformationRules.Process(compilerState, rulesTable, out projectionPruningRequired1))
      {
        bool projectionPruningRequired2;
        TransformationRules.Process(compilerState, TransformationRules.NullabilityRulesTable, out projectionPruningRequired2);
        projectionPruningRequired1 = projectionPruningRequired1 || projectionPruningRequired2;
      }
      return projectionPruningRequired1;
    }

    private static bool Process(
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState,
      ReadOnlyCollection<ReadOnlyCollection<System.Data.Entity.Core.Query.InternalTrees.Rule>> rulesTable,
      out bool projectionPruningRequired)
    {
      RuleProcessor ruleProcessor = new RuleProcessor();
      TransformationRulesContext transformationRulesContext = new TransformationRulesContext(compilerState);
      compilerState.Command.Root = ruleProcessor.ApplyRulesToSubtree((RuleProcessingContext) transformationRulesContext, rulesTable, compilerState.Command.Root);
      projectionPruningRequired = transformationRulesContext.ProjectionPrunningRequired;
      return transformationRulesContext.ReapplyNullabilityRules;
    }
  }
}
