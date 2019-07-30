// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.ScalarRestriction
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Data.Entity.Resources;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class ScalarRestriction : MemberRestriction
  {
    internal ScalarRestriction(MemberPath member, Constant value)
      : base(new MemberProjectedSlot(member), value)
    {
    }

    internal ScalarRestriction(
      MemberPath member,
      IEnumerable<Constant> values,
      IEnumerable<Constant> possibleValues)
      : base(new MemberProjectedSlot(member), values, possibleValues)
    {
    }

    internal ScalarRestriction(MemberProjectedSlot slot, Domain domain)
      : base(slot, domain)
    {
    }

    internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> FixRange(
      Set<Constant> range,
      MemberDomainMap memberDomainMap)
    {
      IEnumerable<Constant> domain = memberDomainMap.GetDomain(this.RestrictedMemberSlot.MemberPath);
      return new ScalarRestriction(this.RestrictedMemberSlot, new Domain((IEnumerable<Constant>) range, domain)).GetDomainBoolExpression(memberDomainMap);
    }

    internal override BoolLiteral RemapBool(Dictionary<MemberPath, MemberPath> remap)
    {
      return (BoolLiteral) new ScalarRestriction(this.RestrictedMemberSlot.RemapSlot(remap), this.Domain);
    }

    internal override MemberRestriction CreateCompleteMemberRestriction(
      IEnumerable<Constant> possibleValues)
    {
      return (MemberRestriction) new ScalarRestriction(this.RestrictedMemberSlot, new Domain(this.Domain.Values, possibleValues));
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      return this.ToStringHelper(builder, blockAlias, skipIsNotNull, false);
    }

    internal override DbExpression AsCqt(DbExpression row, bool skipIsNotNull)
    {
      DbExpression cqt = (DbExpression) null;
      this.AsCql((Action<NegatedConstant, IEnumerable<Constant>>) ((negated, domainValues) => cqt = negated.AsCqt(row, domainValues, this.RestrictedMemberSlot.MemberPath, skipIsNotNull)), (Action<Set<Constant>>) (domainValues =>
      {
        cqt = this.RestrictedMemberSlot.MemberPath.AsCqt(row);
        if (domainValues.Count == 1)
          cqt = (DbExpression) cqt.Equal(domainValues.Single<Constant>().AsCqt(row, this.RestrictedMemberSlot.MemberPath));
        else
          cqt = Helpers.BuildBalancedTreeInPlace<DbExpression>((IList<DbExpression>) domainValues.Select<Constant, DbExpression>((Func<Constant, DbExpression>) (c => (DbExpression) cqt.Equal(c.AsCqt(row, this.RestrictedMemberSlot.MemberPath)))).ToList<DbExpression>(), (Func<DbExpression, DbExpression, DbExpression>) ((prev, next) => (DbExpression) prev.Or(next)));
      }), (Action) (() =>
      {
        DbExpression right = (DbExpression) this.RestrictedMemberSlot.MemberPath.AsCqt(row).IsNull().Not();
        cqt = cqt != null ? (DbExpression) cqt.And(right) : right;
      }), (Action) (() =>
      {
        DbExpression left = (DbExpression) this.RestrictedMemberSlot.MemberPath.AsCqt(row).IsNull();
        cqt = cqt != null ? (DbExpression) left.Or(cqt) : left;
      }), skipIsNotNull);
      return cqt;
    }

    internal override StringBuilder AsUserString(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      return this.ToStringHelper(builder, blockAlias, skipIsNotNull, true);
    }

    private StringBuilder ToStringHelper(
      StringBuilder inputBuilder,
      string blockAlias,
      bool skipIsNotNull,
      bool userString)
    {
      StringBuilder builder = new StringBuilder();
      this.AsCql((Action<NegatedConstant, IEnumerable<Constant>>) ((negated, domainValues) =>
      {
        if (userString)
          negated.AsUserString(builder, blockAlias, domainValues, this.RestrictedMemberSlot.MemberPath, skipIsNotNull);
        else
          negated.AsEsql(builder, blockAlias, domainValues, this.RestrictedMemberSlot.MemberPath, skipIsNotNull);
      }), (Action<Set<Constant>>) (domainValues =>
      {
        this.RestrictedMemberSlot.MemberPath.AsEsql(builder, blockAlias);
        if (domainValues.Count == 1)
        {
          builder.Append(" = ");
          if (userString)
            domainValues.Single<Constant>().ToCompactString(builder);
          else
            domainValues.Single<Constant>().AsEsql(builder, this.RestrictedMemberSlot.MemberPath, blockAlias);
        }
        else
        {
          builder.Append(" IN {");
          bool flag = true;
          foreach (Constant domainValue in domainValues)
          {
            if (!flag)
              builder.Append(", ");
            if (userString)
              domainValue.ToCompactString(builder);
            else
              domainValue.AsEsql(builder, this.RestrictedMemberSlot.MemberPath, blockAlias);
            flag = false;
          }
          builder.Append('}');
        }
      }), (Action) (() =>
      {
        bool flag = builder.Length == 0;
        builder.Insert(0, '(');
        if (!flag)
          builder.Append(" AND ");
        if (userString)
        {
          this.RestrictedMemberSlot.MemberPath.ToCompactString(builder, Strings.ViewGen_EntityInstanceToken);
          builder.Append(" is not NULL)");
        }
        else
        {
          this.RestrictedMemberSlot.MemberPath.AsEsql(builder, blockAlias);
          builder.Append(" IS NOT NULL)");
        }
      }), (Action) (() =>
      {
        bool flag = builder.Length == 0;
        StringBuilder stringBuilder = new StringBuilder();
        if (!flag)
          stringBuilder.Append('(');
        if (userString)
        {
          this.RestrictedMemberSlot.MemberPath.ToCompactString(stringBuilder, blockAlias);
          stringBuilder.Append(" is NULL");
        }
        else
        {
          this.RestrictedMemberSlot.MemberPath.AsEsql(stringBuilder, blockAlias);
          stringBuilder.Append(" IS NULL");
        }
        if (!flag)
          stringBuilder.Append(" OR ");
        builder.Insert(0, stringBuilder.ToString());
        if (flag)
          return;
        builder.Append(')');
      }), skipIsNotNull);
      inputBuilder.Append((object) builder);
      return inputBuilder;
    }

    private void AsCql(
      Action<NegatedConstant, IEnumerable<Constant>> negatedConstantAsCql,
      Action<Set<Constant>> varInDomain,
      Action varIsNotNull,
      Action varIsNull,
      bool skipIsNotNull)
    {
      NegatedConstant negatedConstant = (NegatedConstant) this.Domain.Values.FirstOrDefault<Constant>((Func<Constant, bool>) (c => c is NegatedConstant));
      if (negatedConstant != null)
      {
        negatedConstantAsCql(negatedConstant, this.Domain.Values);
      }
      else
      {
        Set<Constant> set = new Set<Constant>(this.Domain.Values, Constant.EqualityComparer);
        bool flag1 = false;
        if (set.Contains(Constant.Null))
        {
          flag1 = true;
          set.Remove(Constant.Null);
        }
        if (set.Contains(Constant.Undefined))
        {
          flag1 = true;
          set.Remove(Constant.Undefined);
        }
        bool flag2 = !skipIsNotNull && this.RestrictedMemberSlot.MemberPath.IsNullable;
        if (set.Count > 0)
          varInDomain(set);
        if (flag2)
          varIsNotNull();
        if (!flag1)
          return;
        varIsNull();
      }
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      this.RestrictedMemberSlot.ToCompactString(builder);
      builder.Append(" IN (");
      StringUtil.ToCommaSeparatedStringSorted(builder, (IEnumerable) this.Domain.Values);
      builder.Append(")");
    }
  }
}
