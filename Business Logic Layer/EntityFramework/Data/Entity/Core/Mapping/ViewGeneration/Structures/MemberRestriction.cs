// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.MemberRestriction
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal abstract class MemberRestriction : BoolLiteral
  {
    private readonly MemberProjectedSlot m_restrictedMemberSlot;
    private readonly Domain m_domain;
    private readonly bool m_isComplete;

    protected MemberRestriction(MemberProjectedSlot slot, Constant value)
      : this(slot, (IEnumerable<Constant>) new Constant[1]
      {
        value
      })
    {
    }

    protected MemberRestriction(MemberProjectedSlot slot, IEnumerable<Constant> values)
    {
      this.m_restrictedMemberSlot = slot;
      this.m_domain = new Domain(values, values);
    }

    protected MemberRestriction(MemberProjectedSlot slot, Domain domain)
    {
      this.m_restrictedMemberSlot = slot;
      this.m_domain = domain;
      this.m_isComplete = true;
    }

    protected MemberRestriction(
      MemberProjectedSlot slot,
      IEnumerable<Constant> values,
      IEnumerable<Constant> possibleValues)
      : this(slot, new Domain(values, possibleValues))
    {
    }

    internal bool IsComplete
    {
      get
      {
        return this.m_isComplete;
      }
    }

    internal MemberProjectedSlot RestrictedMemberSlot
    {
      get
      {
        return this.m_restrictedMemberSlot;
      }
    }

    internal Domain Domain
    {
      get
      {
        return this.m_domain;
      }
    }

    internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> GetDomainBoolExpression(
      MemberDomainMap domainMap)
    {
      return domainMap == null ? (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) BoolLiteral.MakeTermExpression((BoolLiteral) this, this.m_domain.AllPossibleValues, this.m_domain.Values) : (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) BoolLiteral.MakeTermExpression((BoolLiteral) this, domainMap.GetDomain(this.m_restrictedMemberSlot.MemberPath), this.m_domain.Values);
    }

    internal abstract MemberRestriction CreateCompleteMemberRestriction(
      IEnumerable<Constant> possibleValues);

    internal override void GetRequiredSlots(
      MemberProjectionIndex projectedSlotMap,
      bool[] requiredSlots)
    {
      MemberPath memberPath = this.RestrictedMemberSlot.MemberPath;
      int index = projectedSlotMap.IndexOf(memberPath);
      requiredSlots[index] = true;
    }

    protected override bool IsEqualTo(BoolLiteral right)
    {
      MemberRestriction memberRestriction = right as MemberRestriction;
      if (memberRestriction == null)
        return false;
      if (object.ReferenceEquals((object) this, (object) memberRestriction))
        return true;
      if (!ProjectedSlot.EqualityComparer.Equals((ProjectedSlot) this.m_restrictedMemberSlot, (ProjectedSlot) memberRestriction.m_restrictedMemberSlot))
        return false;
      return this.m_domain.IsEqualTo(memberRestriction.m_domain);
    }

    public override int GetHashCode()
    {
      return ProjectedSlot.EqualityComparer.GetHashCode((ProjectedSlot) this.m_restrictedMemberSlot) ^ this.m_domain.GetHash();
    }

    protected override bool IsIdentifierEqualTo(BoolLiteral right)
    {
      MemberRestriction memberRestriction = right as MemberRestriction;
      if (memberRestriction == null)
        return false;
      if (object.ReferenceEquals((object) this, (object) memberRestriction))
        return true;
      return ProjectedSlot.EqualityComparer.Equals((ProjectedSlot) this.m_restrictedMemberSlot, (ProjectedSlot) memberRestriction.m_restrictedMemberSlot);
    }

    protected override int GetIdentifierHash()
    {
      return ProjectedSlot.EqualityComparer.GetHashCode((ProjectedSlot) this.m_restrictedMemberSlot);
    }

    internal override StringBuilder AsUserString(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      return this.AsEsql(builder, blockAlias, skipIsNotNull);
    }

    internal override StringBuilder AsNegatedUserString(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      builder.Append("NOT(");
      builder = this.AsUserString(builder, blockAlias, skipIsNotNull);
      builder.Append(")");
      return builder;
    }
  }
}
