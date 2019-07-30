// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.TypeRestriction
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class TypeRestriction : MemberRestriction
  {
    internal TypeRestriction(MemberPath member, IEnumerable<EdmType> values)
      : base(new MemberProjectedSlot(member), TypeRestriction.CreateTypeConstants(values))
    {
    }

    internal TypeRestriction(MemberPath member, Constant value)
      : base(new MemberProjectedSlot(member), value)
    {
    }

    internal TypeRestriction(MemberProjectedSlot slot, Domain domain)
      : base(slot, domain)
    {
    }

    internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> FixRange(
      Set<Constant> range,
      MemberDomainMap memberDomainMap)
    {
      IEnumerable<Constant> domain = memberDomainMap.GetDomain(this.RestrictedMemberSlot.MemberPath);
      return new TypeRestriction(this.RestrictedMemberSlot, new Domain((IEnumerable<Constant>) range, domain)).GetDomainBoolExpression(memberDomainMap);
    }

    internal override BoolLiteral RemapBool(Dictionary<MemberPath, MemberPath> remap)
    {
      return (BoolLiteral) new TypeRestriction(this.RestrictedMemberSlot.RemapSlot(remap), this.Domain);
    }

    internal override MemberRestriction CreateCompleteMemberRestriction(
      IEnumerable<Constant> possibleValues)
    {
      return (MemberRestriction) new TypeRestriction(this.RestrictedMemberSlot, new Domain(this.Domain.Values, possibleValues));
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      if (this.Domain.Count > 1)
        builder.Append('(');
      bool flag = true;
      foreach (Constant constant in this.Domain.Values)
      {
        TypeConstant typeConstant = constant as TypeConstant;
        if (!flag)
          builder.Append(" OR ");
        flag = false;
        if (Helper.IsRefType((GlobalItem) this.RestrictedMemberSlot.MemberPath.EdmType))
        {
          builder.Append("Deref(");
          this.RestrictedMemberSlot.MemberPath.AsEsql(builder, blockAlias);
          builder.Append(')');
        }
        else
          this.RestrictedMemberSlot.MemberPath.AsEsql(builder, blockAlias);
        if (constant.IsNull())
        {
          builder.Append(" IS NULL");
        }
        else
        {
          builder.Append(" IS OF (ONLY ");
          CqlWriter.AppendEscapedTypeName(builder, typeConstant.EdmType);
          builder.Append(')');
        }
      }
      if (this.Domain.Count > 1)
        builder.Append(')');
      return builder;
    }

    internal override DbExpression AsCqt(DbExpression row, bool skipIsNotNull)
    {
      DbExpression cqt = this.RestrictedMemberSlot.MemberPath.AsCqt(row);
      if (Helper.IsRefType((GlobalItem) this.RestrictedMemberSlot.MemberPath.EdmType))
        cqt = (DbExpression) cqt.Deref();
      if (this.Domain.Count == 1)
        cqt = (DbExpression) cqt.IsOfOnly(TypeUsage.Create(((TypeConstant) this.Domain.Values.Single<Constant>()).EdmType));
      else
        cqt = Helpers.BuildBalancedTreeInPlace<DbExpression>((IList<DbExpression>) this.Domain.Values.Select<Constant, DbExpression>((Func<Constant, DbExpression>) (t => (DbExpression) cqt.IsOfOnly(TypeUsage.Create(((TypeConstant) t).EdmType)))).ToList<DbExpression>(), (Func<DbExpression, DbExpression, DbExpression>) ((prev, next) => (DbExpression) prev.Or(next)));
      return cqt;
    }

    internal override StringBuilder AsUserString(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      if (Helper.IsRefType((GlobalItem) this.RestrictedMemberSlot.MemberPath.EdmType))
      {
        builder.Append("Deref(");
        this.RestrictedMemberSlot.MemberPath.AsEsql(builder, blockAlias);
        builder.Append(')');
      }
      else
        this.RestrictedMemberSlot.MemberPath.AsEsql(builder, blockAlias);
      if (this.Domain.Count > 1)
        builder.Append(" is a (");
      else
        builder.Append(" is type ");
      bool flag = true;
      foreach (Constant constant in this.Domain.Values)
      {
        TypeConstant typeConstant = constant as TypeConstant;
        if (!flag)
          builder.Append(" OR ");
        if (constant.IsNull())
          builder.Append(" NULL");
        else
          CqlWriter.AppendEscapedTypeName(builder, typeConstant.EdmType);
        flag = false;
      }
      if (this.Domain.Count > 1)
        builder.Append(')');
      return builder;
    }

    private static IEnumerable<Constant> CreateTypeConstants(
      IEnumerable<EdmType> types)
    {
      foreach (EdmType type in types)
      {
        if (type == null)
          yield return Constant.Null;
        else
          yield return (Constant) new TypeConstant(type);
      }
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append("type(");
      this.RestrictedMemberSlot.ToCompactString(builder);
      builder.Append(") IN (");
      StringUtil.ToCommaSeparatedStringSorted(builder, (IEnumerable) this.Domain.Values);
      builder.Append(")");
    }
  }
}
