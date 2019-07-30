// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.CoordinatorFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal abstract class CoordinatorFactory
  {
    private static readonly Func<Shaper, bool> _alwaysTrue = (Func<Shaper, bool>) (s => true);
    private static readonly Func<Shaper, bool> _alwaysFalse = (Func<Shaper, bool>) (s => false);
    internal readonly int Depth;
    internal readonly int StateSlot;
    internal readonly Func<Shaper, bool> HasData;
    internal readonly Func<Shaper, bool> SetKeys;
    internal readonly Func<Shaper, bool> CheckKeys;
    internal readonly ReadOnlyCollection<CoordinatorFactory> NestedCoordinators;
    internal readonly bool IsLeafResult;
    internal readonly bool IsSimple;
    internal readonly ReadOnlyCollection<RecordStateFactory> RecordStateFactories;

    protected CoordinatorFactory(
      int depth,
      int stateSlot,
      Func<Shaper, bool> hasData,
      Func<Shaper, bool> setKeys,
      Func<Shaper, bool> checkKeys,
      CoordinatorFactory[] nestedCoordinators,
      RecordStateFactory[] recordStateFactories)
    {
      this.Depth = depth;
      this.StateSlot = stateSlot;
      this.IsLeafResult = 0 == nestedCoordinators.Length;
      this.HasData = hasData != null ? hasData : CoordinatorFactory._alwaysTrue;
      this.SetKeys = setKeys != null ? setKeys : CoordinatorFactory._alwaysTrue;
      this.CheckKeys = checkKeys != null ? checkKeys : (!this.IsLeafResult ? CoordinatorFactory._alwaysTrue : CoordinatorFactory._alwaysFalse);
      this.NestedCoordinators = new ReadOnlyCollection<CoordinatorFactory>((IList<CoordinatorFactory>) nestedCoordinators);
      this.RecordStateFactories = new ReadOnlyCollection<RecordStateFactory>((IList<RecordStateFactory>) recordStateFactories);
      this.IsSimple = this.IsLeafResult && checkKeys == null && null == hasData;
    }

    internal abstract Coordinator CreateCoordinator(Coordinator parent, Coordinator next);
  }
}
