// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.GroupAggregateVarRefInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class GroupAggregateVarRefInfo
  {
    private readonly Node _computation;
    private readonly GroupAggregateVarInfo _groupAggregateVarInfo;
    private readonly bool _isUnnested;

    internal GroupAggregateVarRefInfo(
      GroupAggregateVarInfo groupAggregateVarInfo,
      Node computation,
      bool isUnnested)
    {
      this._groupAggregateVarInfo = groupAggregateVarInfo;
      this._computation = computation;
      this._isUnnested = isUnnested;
    }

    internal Node Computation
    {
      get
      {
        return this._computation;
      }
    }

    internal GroupAggregateVarInfo GroupAggregateVarInfo
    {
      get
      {
        return this._groupAggregateVarInfo;
      }
    }

    internal bool IsUnnested
    {
      get
      {
        return this._isUnnested;
      }
    }
  }
}
