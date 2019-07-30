// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.Domain
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class Domain : InternalBase
  {
    private readonly Set<Constant> m_domain;
    private readonly Set<Constant> m_possibleValues;

    internal Domain(Constant value, IEnumerable<Constant> possibleDiscreteValues)
      : this((IEnumerable<Constant>) new Constant[1]
      {
        value
      }, possibleDiscreteValues)
    {
    }

    internal Domain(IEnumerable<Constant> values, IEnumerable<Constant> possibleDiscreteValues)
    {
      this.m_possibleValues = Domain.DeterminePossibleValues(values, possibleDiscreteValues);
      this.m_domain = Domain.ExpandNegationsInDomain(values, (IEnumerable<Constant>) this.m_possibleValues);
      this.AssertInvariant();
    }

    internal Domain(Domain domain)
    {
      this.m_domain = new Set<Constant>((IEnumerable<Constant>) domain.m_domain, Constant.EqualityComparer);
      this.m_possibleValues = new Set<Constant>((IEnumerable<Constant>) domain.m_possibleValues, Constant.EqualityComparer);
      this.AssertInvariant();
    }

    internal IEnumerable<Constant> AllPossibleValues
    {
      get
      {
        return (IEnumerable<Constant>) this.AllPossibleValuesInternal;
      }
    }

    private Set<Constant> AllPossibleValuesInternal
    {
      get
      {
        return this.m_possibleValues.Union((IEnumerable<Constant>) new Constant[1]
        {
          (Constant) new NegatedConstant((IEnumerable<Constant>) this.m_possibleValues)
        });
      }
    }

    internal int Count
    {
      get
      {
        return this.m_domain.Count;
      }
    }

    internal IEnumerable<Constant> Values
    {
      get
      {
        return (IEnumerable<Constant>) this.m_domain;
      }
    }

    internal static Set<Constant> DeriveDomainFromMemberPath(
      MemberPath memberPath,
      EdmItemCollection edmItemCollection,
      bool leaveDomainUnbounded)
    {
      Set<Constant> set = Domain.DeriveDomainFromType(memberPath.EdmType, edmItemCollection, leaveDomainUnbounded);
      if (memberPath.IsNullable)
        set.Add(Constant.Null);
      return set;
    }

    private static Set<Constant> DeriveDomainFromType(
      EdmType type,
      EdmItemCollection edmItemCollection,
      bool leaveDomainUnbounded)
    {
      Set<Constant> set;
      if (Helper.IsScalarType(type))
      {
        if (MetadataHelper.HasDiscreteDomain(type))
        {
          set = new Set<Constant>(Domain.CreateList((object) true, (object) false), Constant.EqualityComparer);
        }
        else
        {
          set = new Set<Constant>(Constant.EqualityComparer);
          if (leaveDomainUnbounded)
            set.Add(Constant.NotNull);
        }
      }
      else
      {
        if (Helper.IsRefType((GlobalItem) type))
          type = (EdmType) ((RefType) type).ElementType;
        List<Constant> constantList = new List<Constant>();
        foreach (EdmType type1 in MetadataHelper.GetTypeAndSubtypesOf(type, (ItemCollection) edmItemCollection, false))
        {
          TypeConstant typeConstant = new TypeConstant(type1);
          constantList.Add((Constant) typeConstant);
        }
        set = new Set<Constant>((IEnumerable<Constant>) constantList, Constant.EqualityComparer);
      }
      return set;
    }

    internal static bool TryGetDefaultValueForMemberPath(
      MemberPath memberPath,
      out Constant defaultConstant)
    {
      object defaultValue = memberPath.DefaultValue;
      defaultConstant = Constant.Null;
      if (defaultValue != null)
      {
        defaultConstant = (Constant) new ScalarConstant(defaultValue);
        return true;
      }
      return memberPath.IsNullable || memberPath.IsComputed;
    }

    internal static Constant GetDefaultValueForMemberPath(
      MemberPath memberPath,
      IEnumerable<LeftCellWrapper> wrappersForErrorReporting,
      ConfigViewGenerator config)
    {
      Constant defaultConstant = (Constant) null;
      if (!Domain.TryGetDefaultValueForMemberPath(memberPath, out defaultConstant))
        ExceptionHelpers.ThrowMappingException(new ErrorLog.Record(ViewGenErrorCode.NoDefaultValue, Strings.ViewGen_No_Default_Value((object) memberPath.Extent.Name, (object) memberPath.PathToString(new bool?(false))), wrappersForErrorReporting, string.Empty), config);
      return defaultConstant;
    }

    internal int GetHash()
    {
      int num = 0;
      foreach (Constant constant in this.m_domain)
        num ^= Constant.EqualityComparer.GetHashCode(constant);
      return num;
    }

    internal bool IsEqualTo(Domain second)
    {
      return this.m_domain.SetEquals(second.m_domain);
    }

    internal bool ContainsNotNull()
    {
      NegatedConstant negatedConstant = Domain.GetNegatedConstant((IEnumerable<Constant>) this.m_domain);
      if (negatedConstant != null)
        return negatedConstant.Contains(Constant.Null);
      return false;
    }

    internal bool Contains(Constant constant)
    {
      return this.m_domain.Contains(constant);
    }

    internal static Set<Constant> ExpandNegationsInDomain(
      IEnumerable<Constant> domain,
      IEnumerable<Constant> otherPossibleValues)
    {
      Set<Constant> possibleValues = Domain.DeterminePossibleValues(domain, otherPossibleValues);
      Set<Constant> set1 = new Set<Constant>(Constant.EqualityComparer);
      foreach (Constant element in domain)
      {
        NegatedConstant negatedConstant = element as NegatedConstant;
        if (negatedConstant != null)
        {
          set1.Add((Constant) new NegatedConstant((IEnumerable<Constant>) possibleValues));
          Set<Constant> set2 = possibleValues.Difference(negatedConstant.Elements);
          set1.AddRange((IEnumerable<Constant>) set2);
        }
        else
          set1.Add(element);
      }
      return set1;
    }

    internal static Set<Constant> ExpandNegationsInDomain(IEnumerable<Constant> domain)
    {
      return Domain.ExpandNegationsInDomain(domain, domain);
    }

    private static Set<Constant> DeterminePossibleValues(IEnumerable<Constant> domain)
    {
      Set<Constant> set = new Set<Constant>(Constant.EqualityComparer);
      foreach (Constant element1 in domain)
      {
        NegatedConstant negatedConstant = element1 as NegatedConstant;
        if (negatedConstant != null)
        {
          foreach (Constant element2 in negatedConstant.Elements)
            set.Add(element2);
        }
        else
          set.Add(element1);
      }
      return set;
    }

    internal static Dictionary<MemberPath, Set<Constant>> ComputeConstantDomainSetsForSlotsInQueryViews(
      IEnumerable<Cell> cells,
      EdmItemCollection edmItemCollection,
      bool isValidationEnabled)
    {
      Dictionary<MemberPath, Set<Constant>> dictionary = new Dictionary<MemberPath, Set<Constant>>(MemberPath.EqualityComparer);
      foreach (Cell cell in cells)
      {
        foreach (MemberRestriction memberRestriction in cell.CQuery.GetConjunctsFromWhereClause())
        {
          MemberProjectedSlot restrictedMemberSlot = memberRestriction.RestrictedMemberSlot;
          Set<Constant> set1 = Domain.DeriveDomainFromMemberPath(restrictedMemberSlot.MemberPath, edmItemCollection, isValidationEnabled);
          set1.AddRange(memberRestriction.Domain.Values.Where<Constant>((Func<Constant, bool>) (c =>
          {
            if (!c.Equals((object) Constant.Null))
              return !c.Equals((object) Constant.NotNull);
            return false;
          })));
          Set<Constant> set2;
          if (!dictionary.TryGetValue(restrictedMemberSlot.MemberPath, out set2))
            dictionary[restrictedMemberSlot.MemberPath] = set1;
          else
            set2.AddRange((IEnumerable<Constant>) set1);
        }
      }
      return dictionary;
    }

    private static bool GetRestrictedOrUnrestrictedDomain(
      MemberProjectedSlot slot,
      CellQuery cellQuery,
      EdmItemCollection edmItemCollection,
      out Set<Constant> domain)
    {
      return Domain.TryGetDomainRestrictedByWhereClause((IEnumerable<Constant>) Domain.DeriveDomainFromMemberPath(slot.MemberPath, edmItemCollection, true), slot, cellQuery, out domain);
    }

    internal static Dictionary<MemberPath, Set<Constant>> ComputeConstantDomainSetsForSlotsInUpdateViews(
      IEnumerable<Cell> cells,
      EdmItemCollection edmItemCollection)
    {
      Dictionary<MemberPath, Set<Constant>> dictionary = new Dictionary<MemberPath, Set<Constant>>(MemberPath.EqualityComparer);
      foreach (Cell cell in cells)
      {
        CellQuery cquery = cell.CQuery;
        CellQuery squery = cell.SQuery;
        foreach (MemberProjectedSlot slot in squery.GetConjunctsFromWhereClause().Select<MemberRestriction, MemberProjectedSlot>((Func<MemberRestriction, MemberProjectedSlot>) (oneOfConst => oneOfConst.RestrictedMemberSlot)))
        {
          Set<Constant> domain;
          if (!Domain.GetRestrictedOrUnrestrictedDomain(slot, squery, edmItemCollection, out domain))
          {
            int projectedPosition = squery.GetProjectedPosition(slot);
            if (projectedPosition >= 0 && !Domain.GetRestrictedOrUnrestrictedDomain(cquery.ProjectedSlotAt(projectedPosition) as MemberProjectedSlot, cquery, edmItemCollection, out domain))
              continue;
          }
          MemberPath memberPath = slot.MemberPath;
          Constant defaultConstant;
          if (Domain.TryGetDefaultValueForMemberPath(memberPath, out defaultConstant))
            domain.Add(defaultConstant);
          Set<Constant> set;
          if (!dictionary.TryGetValue(memberPath, out set))
            dictionary[memberPath] = domain;
          else
            set.AddRange((IEnumerable<Constant>) domain);
        }
      }
      return dictionary;
    }

    private static bool TryGetDomainRestrictedByWhereClause(
      IEnumerable<Constant> domain,
      MemberProjectedSlot slot,
      CellQuery cellQuery,
      out Set<Constant> result)
    {
      IEnumerable<Set<Constant>> source = cellQuery.GetConjunctsFromWhereClause().Where<MemberRestriction>((Func<MemberRestriction, bool>) (restriction => MemberPath.EqualityComparer.Equals(restriction.RestrictedMemberSlot.MemberPath, slot.MemberPath))).Select<MemberRestriction, Set<Constant>>((Func<MemberRestriction, Set<Constant>>) (restriction => new Set<Constant>(restriction.Domain.Values, Constant.EqualityComparer)));
      if (!source.Any<Set<Constant>>())
      {
        result = new Set<Constant>(domain);
        return false;
      }
      Set<Constant> possibleValues = Domain.DeterminePossibleValues(source.SelectMany<Set<Constant>, Constant>((Func<Set<Constant>, IEnumerable<Constant>>) (m => m.Select<Constant, Constant>((Func<Constant, Constant>) (c => c)))), domain);
      Domain domain1 = new Domain(domain, (IEnumerable<Constant>) possibleValues);
      foreach (Set<Constant> set in source)
        domain1 = domain1.Intersect(new Domain((IEnumerable<Constant>) set, (IEnumerable<Constant>) possibleValues));
      result = new Set<Constant>(domain1.Values, Constant.EqualityComparer);
      return !domain.SequenceEqual<Constant>((IEnumerable<Constant>) result);
    }

    private Domain Intersect(Domain second)
    {
      Domain domain = new Domain(this);
      domain.m_domain.Intersect(second.m_domain);
      return domain;
    }

    private static NegatedConstant GetNegatedConstant(IEnumerable<Constant> constants)
    {
      NegatedConstant negatedConstant1 = (NegatedConstant) null;
      foreach (Constant constant in constants)
      {
        NegatedConstant negatedConstant2 = constant as NegatedConstant;
        if (negatedConstant2 != null)
          negatedConstant1 = negatedConstant2;
      }
      return negatedConstant1;
    }

    private static Set<Constant> DeterminePossibleValues(
      IEnumerable<Constant> domain1,
      IEnumerable<Constant> domain2)
    {
      return Domain.DeterminePossibleValues((IEnumerable<Constant>) new Set<Constant>(domain1, Constant.EqualityComparer).Union(domain2));
    }

    [Conditional("DEBUG")]
    private static void CheckTwoDomainInvariants(Domain domain1, Domain domain2)
    {
      domain1.AssertInvariant();
      domain2.AssertInvariant();
    }

    private static IEnumerable<Constant> CreateList(object value1, object value2)
    {
      yield return (Constant) new ScalarConstant(value1);
      yield return (Constant) new ScalarConstant(value2);
    }

    internal void AssertInvariant()
    {
      Domain.GetNegatedConstant((IEnumerable<Constant>) this.m_domain);
      Domain.GetNegatedConstant((IEnumerable<Constant>) this.m_possibleValues);
    }

    internal string ToUserString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (Constant constant in this.m_domain)
      {
        if (!flag)
          stringBuilder.Append(", ");
        stringBuilder.Append(constant.ToUserString());
        flag = false;
      }
      return stringBuilder.ToString();
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append(this.ToUserString());
    }
  }
}
