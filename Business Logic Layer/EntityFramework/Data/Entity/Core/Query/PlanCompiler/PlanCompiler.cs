// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class PlanCompiler
  {
    private static readonly BooleanSwitch _applyTransformationsRegardlessOfSize = new BooleanSwitch("System.Data.Entity.Core.EntityClient.IgnoreOptimizationLimit", "The Entity Framework should try to optimize the query regardless of its size");
    private const int MaxNodeCountForTransformations = 100000;
    private readonly DbCommandTree m_ctree;
    private Command m_command;
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    private PlanCompilerPhase m_phase;
    private int _precedingPhases;
    private int m_neededPhases;
    private ConstraintManager m_constraintManager;
    private bool? m_mayApplyTransformationRules;

    private PlanCompiler(DbCommandTree ctree)
    {
      this.m_ctree = ctree;
    }

    internal static void Assert(bool condition, string message)
    {
      if (!condition)
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.AssertionFailed, 0, (object) message);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal static void Compile(
      DbCommandTree ctree,
      out List<ProviderCommandInfo> providerCommands,
      out ColumnMap resultColumnMap,
      out int columnCount,
      out Set<EntitySet> entitySets)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(ctree != null, "Expected a valid, non-null Command Tree input");
      new System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler(ctree).Compile(out providerCommands, out resultColumnMap, out columnCount, out entitySets);
    }

    internal Command Command
    {
      get
      {
        return this.m_command;
      }
    }

    internal bool HasSortingOnNullSentinels { get; set; }

    internal ConstraintManager ConstraintManager
    {
      get
      {
        if (this.m_constraintManager == null)
          this.m_constraintManager = new ConstraintManager();
        return this.m_constraintManager;
      }
    }

    internal MetadataWorkspace MetadataWorkspace
    {
      get
      {
        return this.m_ctree.MetadataWorkspace;
      }
    }

    internal bool IsPhaseNeeded(PlanCompilerPhase phase)
    {
      return (this.m_neededPhases & 1 << (int) (phase & (PlanCompilerPhase) 31)) != 0;
    }

    internal void MarkPhaseAsNeeded(PlanCompilerPhase phase)
    {
      this.m_neededPhases |= 1 << (int) (phase & (PlanCompilerPhase) 31);
    }

    internal bool IsAfterPhase(PlanCompilerPhase phase)
    {
      return (this._precedingPhases & 1 << (int) (phase & (PlanCompilerPhase) 31)) != 0;
    }

    private void Compile(
      out List<ProviderCommandInfo> providerCommands,
      out ColumnMap resultColumnMap,
      out int columnCount,
      out Set<EntitySet> entitySets)
    {
      this.Initialize();
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string empty3 = string.Empty;
      string empty4 = string.Empty;
      string empty5 = string.Empty;
      string empty6 = string.Empty;
      string empty7 = string.Empty;
      string empty8 = string.Empty;
      string empty9 = string.Empty;
      string empty10 = string.Empty;
      string empty11 = string.Empty;
      string empty12 = string.Empty;
      string empty13 = string.Empty;
      string empty14 = string.Empty;
      string empty15 = string.Empty;
      this.m_neededPhases = 593;
      this.SwitchToPhase(PlanCompilerPhase.PreProcessor);
      StructuredTypeInfo typeInfo;
      Dictionary<EdmFunction, EdmProperty[]> tvfResultKeys;
      PreProcessor.Process(this, out typeInfo, out tvfResultKeys);
      entitySets = typeInfo.GetEntitySets();
      if (this.IsPhaseNeeded(PlanCompilerPhase.AggregatePushdown))
      {
        this.SwitchToPhase(PlanCompilerPhase.AggregatePushdown);
        AggregatePushdown.Process(this);
      }
      if (this.IsPhaseNeeded(PlanCompilerPhase.Normalization))
      {
        this.SwitchToPhase(PlanCompilerPhase.Normalization);
        Normalizer.Process(this);
      }
      if (this.IsPhaseNeeded(PlanCompilerPhase.NTE))
      {
        this.SwitchToPhase(PlanCompilerPhase.NTE);
        NominalTypeEliminator.Process(this, typeInfo, tvfResultKeys);
      }
      if (this.IsPhaseNeeded(PlanCompilerPhase.ProjectionPruning))
      {
        this.SwitchToPhase(PlanCompilerPhase.ProjectionPruning);
        ProjectionPruner.Process(this);
      }
      if (this.IsPhaseNeeded(PlanCompilerPhase.NestPullup))
      {
        this.SwitchToPhase(PlanCompilerPhase.NestPullup);
        NestPullup.Process(this);
        this.SwitchToPhase(PlanCompilerPhase.ProjectionPruning);
        ProjectionPruner.Process(this);
      }
      if (this.IsPhaseNeeded(PlanCompilerPhase.Transformations) && this.ApplyTransformations(ref empty8, TransformationRulesGroup.All))
      {
        this.SwitchToPhase(PlanCompilerPhase.ProjectionPruning);
        ProjectionPruner.Process(this);
        this.ApplyTransformations(ref empty10, TransformationRulesGroup.Project);
      }
      if (this.IsPhaseNeeded(PlanCompilerPhase.NullSemantics))
      {
        this.SwitchToPhase(PlanCompilerPhase.NullSemantics);
        if (!this.m_ctree.UseDatabaseNullSemantics && NullSemantics.Process(this.Command))
          this.ApplyTransformations(ref empty12, TransformationRulesGroup.NullSemantics);
      }
      if (this.IsPhaseNeeded(PlanCompilerPhase.JoinElimination))
      {
        for (int index = 0; index < 10; ++index)
        {
          this.SwitchToPhase(PlanCompilerPhase.JoinElimination);
          if (JoinElimination.Process(this) || this.TransformationsDeferred)
          {
            this.TransformationsDeferred = false;
            this.ApplyTransformations(ref empty14, TransformationRulesGroup.PostJoinElimination);
          }
          else
            break;
        }
      }
      this.SwitchToPhase(PlanCompilerPhase.CodeGen);
      CodeGen.Process(this, out providerCommands, out resultColumnMap, out columnCount);
    }

    private bool ApplyTransformations(ref string dumpString, TransformationRulesGroup rulesGroup)
    {
      if (!this.MayApplyTransformationRules)
        return false;
      dumpString = this.SwitchToPhase(PlanCompilerPhase.Transformations);
      return TransformationRules.Process(this, rulesGroup);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newPhase")]
    private string SwitchToPhase(PlanCompilerPhase newPhase)
    {
      string empty = string.Empty;
      if (newPhase != this.m_phase)
        this._precedingPhases |= 1 << (int) (this.m_phase & (PlanCompilerPhase) 31);
      this.m_phase = newPhase;
      return empty;
    }

    private bool MayApplyTransformationRules
    {
      get
      {
        if (!this.m_mayApplyTransformationRules.HasValue)
          this.m_mayApplyTransformationRules = new bool?(this.ComputeMayApplyTransformations());
        return this.m_mayApplyTransformationRules.Value;
      }
    }

    internal bool TransformationsDeferred { get; set; }

    private bool ComputeMayApplyTransformations()
    {
      if (System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler._applyTransformationsRegardlessOfSize.Enabled || this.m_command.NextNodeId < 100000)
        return true;
      return NodeCounter.Count(this.m_command.Root) < 100000;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void Initialize()
    {
      DbQueryCommandTree ctree = this.m_ctree as DbQueryCommandTree;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(ctree != null, "Unexpected command tree kind. Only query command tree is supported.");
      this.m_command = ITreeGenerator.Generate(ctree);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(this.m_command != null, "Unable to generate internal tree from Command Tree");
    }
  }
}
