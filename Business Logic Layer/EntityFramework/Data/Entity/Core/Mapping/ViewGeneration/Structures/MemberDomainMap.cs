// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.MemberDomainMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class MemberDomainMap : InternalBase
  {
    private readonly Set<MemberPath> m_projectedConditionMembers = new Set<MemberPath>();
    private readonly Dictionary<MemberPath, Set<Constant>> m_conditionDomainMap;
    private readonly Dictionary<MemberPath, Set<Constant>> m_nonConditionDomainMap;
    private readonly EdmItemCollection m_edmItemCollection;

    private MemberDomainMap(
      Dictionary<MemberPath, Set<Constant>> domainMap,
      Dictionary<MemberPath, Set<Constant>> nonConditionDomainMap,
      EdmItemCollection edmItemCollection)
    {
      this.m_conditionDomainMap = domainMap;
      this.m_nonConditionDomainMap = nonConditionDomainMap;
      this.m_edmItemCollection = edmItemCollection;
    }

    internal MemberDomainMap(
      ViewTarget viewTarget,
      bool isValidationEnabled,
      IEnumerable<Cell> extentCells,
      EdmItemCollection edmItemCollection,
      ConfigViewGenerator config,
      Dictionary<EntityType, Set<EntityType>> inheritanceGraph)
    {
      this.m_conditionDomainMap = new Dictionary<MemberPath, Set<Constant>>(MemberPath.EqualityComparer);
      this.m_edmItemCollection = edmItemCollection;
      Dictionary<MemberPath, Set<Constant>> dictionary = viewTarget != ViewTarget.UpdateView ? Domain.ComputeConstantDomainSetsForSlotsInQueryViews(extentCells, this.m_edmItemCollection, isValidationEnabled) : Domain.ComputeConstantDomainSetsForSlotsInUpdateViews(extentCells, this.m_edmItemCollection);
      foreach (Cell extentCell in extentCells)
      {
        foreach (MemberRestriction memberRestriction in extentCell.GetLeftQuery(viewTarget).GetConjunctsFromWhereClause())
        {
          MemberPath memberPath = memberRestriction.RestrictedMemberSlot.MemberPath;
          Set<Constant> set;
          if (!dictionary.TryGetValue(memberPath, out set))
            set = Domain.DeriveDomainFromMemberPath(memberPath, edmItemCollection, isValidationEnabled);
          if (set.Contains(Constant.Null) || !memberRestriction.Domain.Values.All<Constant>((Func<Constant, bool>) (conditionConstant => conditionConstant.Equals((object) Constant.NotNull))))
          {
            if (set.Count <= 0 || !set.Contains(Constant.Null) && memberRestriction.Domain.Values.Contains<Constant>(Constant.Null))
              ExceptionHelpers.ThrowMappingException(new ErrorLog.Record(ViewGenErrorCode.InvalidCondition, Strings.ViewGen_InvalidCondition((object) memberPath.PathToString(new bool?(false))), extentCell, string.Empty), config);
            if (!memberPath.IsAlwaysDefined(inheritanceGraph))
              set.Add(Constant.Undefined);
            this.AddToDomainMap(memberPath, (IEnumerable<Constant>) set);
          }
        }
      }
      this.m_nonConditionDomainMap = new Dictionary<MemberPath, Set<Constant>>(MemberPath.EqualityComparer);
      foreach (Cell extentCell in extentCells)
      {
        foreach (MemberProjectedSlot allQuerySlot in extentCell.GetLeftQuery(viewTarget).GetAllQuerySlots())
        {
          MemberPath memberPath = allQuerySlot.MemberPath;
          if (!this.m_conditionDomainMap.ContainsKey(memberPath) && !this.m_nonConditionDomainMap.ContainsKey(memberPath))
          {
            Set<Constant> set = Domain.DeriveDomainFromMemberPath(memberPath, this.m_edmItemCollection, true);
            if (!memberPath.IsAlwaysDefined(inheritanceGraph))
              set.Add(Constant.Undefined);
            Set<Constant> iconstants = Domain.ExpandNegationsInDomain((IEnumerable<Constant>) set, (IEnumerable<Constant>) set);
            this.m_nonConditionDomainMap.Add(memberPath, (Set<Constant>) new MemberDomainMap.CellConstantSetInfo(iconstants));
          }
        }
      }
    }

    internal bool IsProjectedConditionMember(MemberPath memberPath)
    {
      return this.m_projectedConditionMembers.Contains(memberPath);
    }

    internal MemberDomainMap GetOpenDomain()
    {
      Dictionary<MemberPath, Set<Constant>> dictionary = this.m_conditionDomainMap.ToDictionary<KeyValuePair<MemberPath, Set<Constant>>, MemberPath, Set<Constant>>((Func<KeyValuePair<MemberPath, Set<Constant>>, MemberPath>) (p => p.Key), (Func<KeyValuePair<MemberPath, Set<Constant>>, Set<Constant>>) (p => new Set<Constant>((IEnumerable<Constant>) p.Value, Constant.EqualityComparer)));
      this.ExpandDomainsIfNeeded(dictionary);
      return new MemberDomainMap(dictionary, this.m_nonConditionDomainMap, this.m_edmItemCollection);
    }

    internal MemberDomainMap MakeCopy()
    {
      return new MemberDomainMap(this.m_conditionDomainMap.ToDictionary<KeyValuePair<MemberPath, Set<Constant>>, MemberPath, Set<Constant>>((Func<KeyValuePair<MemberPath, Set<Constant>>, MemberPath>) (p => p.Key), (Func<KeyValuePair<MemberPath, Set<Constant>>, Set<Constant>>) (p => new Set<Constant>((IEnumerable<Constant>) p.Value, Constant.EqualityComparer))), this.m_nonConditionDomainMap, this.m_edmItemCollection);
    }

    internal void ExpandDomainsToIncludeAllPossibleValues()
    {
      this.ExpandDomainsIfNeeded(this.m_conditionDomainMap);
    }

    private void ExpandDomainsIfNeeded(
      Dictionary<MemberPath, Set<Constant>> domainMapForMembers)
    {
      foreach (MemberPath key in domainMapForMembers.Keys)
      {
        Set<Constant> domainMapForMember = domainMapForMembers[key];
        if (key.IsScalarType() && !domainMapForMember.Any<Constant>((Func<Constant, bool>) (c => c is NegatedConstant)))
        {
          if (MetadataHelper.HasDiscreteDomain(key.EdmType))
          {
            Set<Constant> set = Domain.DeriveDomainFromMemberPath(key, this.m_edmItemCollection, true);
            domainMapForMember.Unite((IEnumerable<Constant>) set);
          }
          else
          {
            NegatedConstant negatedConstant = new NegatedConstant((IEnumerable<Constant>) domainMapForMember);
            domainMapForMember.Add((Constant) negatedConstant);
          }
        }
      }
    }

    internal void ReduceEnumerableDomainToEnumeratedValues(ConfigViewGenerator config)
    {
      MemberDomainMap.ReduceEnumerableDomainToEnumeratedValues(this.m_conditionDomainMap, config, this.m_edmItemCollection);
      MemberDomainMap.ReduceEnumerableDomainToEnumeratedValues(this.m_nonConditionDomainMap, config, this.m_edmItemCollection);
    }

    private static void ReduceEnumerableDomainToEnumeratedValues(
      Dictionary<MemberPath, Set<Constant>> domainMap,
      ConfigViewGenerator config,
      EdmItemCollection edmItemCollection)
    {
      foreach (MemberPath key in domainMap.Keys)
      {
        if (MetadataHelper.HasDiscreteDomain(key.EdmType))
        {
          Set<Constant> set1 = Domain.DeriveDomainFromMemberPath(key, edmItemCollection, true);
          Set<Constant> set2 = domainMap[key].Difference((IEnumerable<Constant>) set1);
          set2.Remove(Constant.Undefined);
          if (set2.Count > 0)
          {
            if (config.IsNormalTracing)
              Helpers.FormatTraceLine("Changed domain of {0} from {1} - subtract {2}", (object) key, (object) domainMap[key], (object) set2);
            domainMap[key].Subtract((IEnumerable<Constant>) set2);
          }
        }
      }
    }

    internal static void PropagateUpdateDomainToQueryDomain(
      IEnumerable<Cell> cells,
      MemberDomainMap queryDomainMap,
      MemberDomainMap updateDomainMap)
    {
      foreach (Cell cell in cells)
      {
        CellQuery cquery = cell.CQuery;
        CellQuery squery = cell.SQuery;
        for (int slotNum = 0; slotNum < cquery.NumProjectedSlots; ++slotNum)
        {
          MemberProjectedSlot memberProjectedSlot1 = cquery.ProjectedSlotAt(slotNum) as MemberProjectedSlot;
          MemberProjectedSlot memberProjectedSlot2 = squery.ProjectedSlotAt(slotNum) as MemberProjectedSlot;
          if (memberProjectedSlot1 != null && memberProjectedSlot2 != null)
          {
            MemberPath memberPath1 = memberProjectedSlot1.MemberPath;
            MemberPath memberPath2 = memberProjectedSlot2.MemberPath;
            queryDomainMap.GetDomainInternal(memberPath1).Unite(updateDomainMap.GetDomainInternal(memberPath2).Where<Constant>((Func<Constant, bool>) (constant =>
            {
              if (!constant.IsNull())
                return !(constant is NegatedConstant);
              return false;
            })));
            if (updateDomainMap.IsConditionMember(memberPath2) && !queryDomainMap.IsConditionMember(memberPath1))
              queryDomainMap.m_projectedConditionMembers.Add(memberPath1);
          }
        }
      }
      MemberDomainMap.ExpandNegationsInDomainMap(queryDomainMap.m_conditionDomainMap);
      MemberDomainMap.ExpandNegationsInDomainMap(queryDomainMap.m_nonConditionDomainMap);
    }

    private static void ExpandNegationsInDomainMap(Dictionary<MemberPath, Set<Constant>> domainMap)
    {
      foreach (MemberPath index in domainMap.Keys.ToArray<MemberPath>())
        domainMap[index] = Domain.ExpandNegationsInDomain((IEnumerable<Constant>) domainMap[index]);
    }

    internal bool IsConditionMember(MemberPath path)
    {
      return this.m_conditionDomainMap.ContainsKey(path);
    }

    internal IEnumerable<MemberPath> ConditionMembers(EntitySetBase extent)
    {
      foreach (MemberPath key in this.m_conditionDomainMap.Keys)
      {
        if (key.Extent.Equals((object) extent))
          yield return key;
      }
    }

    internal IEnumerable<MemberPath> NonConditionMembers(EntitySetBase extent)
    {
      foreach (MemberPath key in this.m_nonConditionDomainMap.Keys)
      {
        if (key.Extent.Equals((object) extent))
          yield return key;
      }
    }

    internal void AddSentinel(MemberPath path)
    {
      this.GetDomainInternal(path).Add(Constant.AllOtherConstants);
    }

    internal void RemoveSentinel(MemberPath path)
    {
      this.GetDomainInternal(path).Remove(Constant.AllOtherConstants);
    }

    internal IEnumerable<Constant> GetDomain(MemberPath path)
    {
      return (IEnumerable<Constant>) this.GetDomainInternal(path);
    }

    private Set<Constant> GetDomainInternal(MemberPath path)
    {
      Set<Constant> nonConditionDomain;
      if (!this.m_conditionDomainMap.TryGetValue(path, out nonConditionDomain))
        nonConditionDomain = this.m_nonConditionDomainMap[path];
      return nonConditionDomain;
    }

    internal void UpdateConditionMemberDomain(MemberPath path, IEnumerable<Constant> domainValues)
    {
      Set<Constant> conditionDomain = this.m_conditionDomainMap[path];
      conditionDomain.Clear();
      conditionDomain.Unite(domainValues);
    }

    private void AddToDomainMap(MemberPath member, IEnumerable<Constant> domainValues)
    {
      Set<Constant> set;
      if (!this.m_conditionDomainMap.TryGetValue(member, out set))
        set = new Set<Constant>(Constant.EqualityComparer);
      set.Unite(domainValues);
      this.m_conditionDomainMap[member] = Domain.ExpandNegationsInDomain((IEnumerable<Constant>) set, (IEnumerable<Constant>) set);
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      foreach (MemberPath key in this.m_conditionDomainMap.Keys)
      {
        builder.Append('(');
        key.ToCompactString(builder);
        IEnumerable<Constant> domain = this.GetDomain(key);
        builder.Append(": ");
        StringUtil.ToCommaSeparatedStringSorted(builder, (IEnumerable) domain);
        builder.Append(") ");
      }
    }

    private class CellConstantSetInfo : Set<Constant>
    {
      internal CellConstantSetInfo(Set<Constant> iconstants)
        : base(iconstants)
      {
      }
    }
  }
}
