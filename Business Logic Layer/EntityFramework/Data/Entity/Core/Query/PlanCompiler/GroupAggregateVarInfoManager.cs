// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.GroupAggregateVarInfoManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class GroupAggregateVarInfoManager
  {
    private readonly Dictionary<Var, GroupAggregateVarRefInfo> _groupAggregateVarRelatedVarToInfo = new Dictionary<Var, GroupAggregateVarRefInfo>();
    private readonly HashSet<GroupAggregateVarInfo> _groupAggregateVarInfos = new HashSet<GroupAggregateVarInfo>();
    private Dictionary<Var, Dictionary<EdmMember, GroupAggregateVarRefInfo>> _groupAggregateVarRelatedVarPropertyToInfo;

    internal IEnumerable<GroupAggregateVarInfo> GroupAggregateVarInfos
    {
      get
      {
        return (IEnumerable<GroupAggregateVarInfo>) this._groupAggregateVarInfos;
      }
    }

    internal void Add(
      Var var,
      GroupAggregateVarInfo groupAggregateVarInfo,
      System.Data.Entity.Core.Query.InternalTrees.Node computationTemplate,
      bool isUnnested)
    {
      this._groupAggregateVarRelatedVarToInfo.Add(var, new GroupAggregateVarRefInfo(groupAggregateVarInfo, computationTemplate, isUnnested));
      this._groupAggregateVarInfos.Add(groupAggregateVarInfo);
    }

    internal void Add(
      Var var,
      GroupAggregateVarInfo groupAggregateVarInfo,
      System.Data.Entity.Core.Query.InternalTrees.Node computationTemplate,
      bool isUnnested,
      EdmMember property)
    {
      if (property == null)
      {
        this.Add(var, groupAggregateVarInfo, computationTemplate, isUnnested);
      }
      else
      {
        if (this._groupAggregateVarRelatedVarPropertyToInfo == null)
          this._groupAggregateVarRelatedVarPropertyToInfo = new Dictionary<Var, Dictionary<EdmMember, GroupAggregateVarRefInfo>>();
        Dictionary<EdmMember, GroupAggregateVarRefInfo> dictionary;
        if (!this._groupAggregateVarRelatedVarPropertyToInfo.TryGetValue(var, out dictionary))
        {
          dictionary = new Dictionary<EdmMember, GroupAggregateVarRefInfo>();
          this._groupAggregateVarRelatedVarPropertyToInfo.Add(var, dictionary);
        }
        dictionary.Add(property, new GroupAggregateVarRefInfo(groupAggregateVarInfo, computationTemplate, isUnnested));
        this._groupAggregateVarInfos.Add(groupAggregateVarInfo);
      }
    }

    internal bool TryGetReferencedGroupAggregateVarInfo(
      Var var,
      out GroupAggregateVarRefInfo groupAggregateVarRefInfo)
    {
      return this._groupAggregateVarRelatedVarToInfo.TryGetValue(var, out groupAggregateVarRefInfo);
    }

    internal bool TryGetReferencedGroupAggregateVarInfo(
      Var var,
      EdmMember property,
      out GroupAggregateVarRefInfo groupAggregateVarRefInfo)
    {
      if (property == null)
        return this.TryGetReferencedGroupAggregateVarInfo(var, out groupAggregateVarRefInfo);
      Dictionary<EdmMember, GroupAggregateVarRefInfo> dictionary;
      if (this._groupAggregateVarRelatedVarPropertyToInfo != null && this._groupAggregateVarRelatedVarPropertyToInfo.TryGetValue(var, out dictionary))
        return dictionary.TryGetValue(property, out groupAggregateVarRefInfo);
      groupAggregateVarRefInfo = (GroupAggregateVarRefInfo) null;
      return false;
    }
  }
}
