// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.NegatedConstant
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Resources;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal sealed class NegatedConstant : Constant
  {
    private readonly Set<Constant> m_negatedDomain;

    internal NegatedConstant(IEnumerable<Constant> values)
    {
      this.m_negatedDomain = new Set<Constant>(values, Constant.EqualityComparer);
    }

    internal IEnumerable<Constant> Elements
    {
      get
      {
        return (IEnumerable<Constant>) this.m_negatedDomain;
      }
    }

    internal bool Contains(Constant constant)
    {
      return this.m_negatedDomain.Contains(constant);
    }

    internal override bool IsNull()
    {
      return false;
    }

    internal override bool IsNotNull()
    {
      if (object.ReferenceEquals((object) this, (object) Constant.NotNull))
        return true;
      if (this.m_negatedDomain.Count == 1)
        return this.m_negatedDomain.Contains(Constant.Null);
      return false;
    }

    internal override bool IsUndefined()
    {
      return false;
    }

    internal override bool HasNotNull()
    {
      return this.m_negatedDomain.Contains(Constant.Null);
    }

    public override int GetHashCode()
    {
      int num = 0;
      foreach (Constant constant in this.m_negatedDomain)
        num ^= Constant.EqualityComparer.GetHashCode(constant);
      return num;
    }

    protected override bool IsEqualTo(Constant right)
    {
      NegatedConstant negatedConstant = right as NegatedConstant;
      if (negatedConstant == null)
        return false;
      return this.m_negatedDomain.SetEquals(negatedConstant.m_negatedDomain);
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      MemberPath outputMember,
      string blockAlias)
    {
      return (StringBuilder) null;
    }

    internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
    {
      return (DbExpression) null;
    }

    internal StringBuilder AsEsql(
      StringBuilder builder,
      string blockAlias,
      IEnumerable<Constant> constants,
      MemberPath outputMember,
      bool skipIsNotNull)
    {
      return this.ToStringHelper(builder, blockAlias, constants, outputMember, skipIsNotNull, false);
    }

    internal DbExpression AsCqt(
      DbExpression row,
      IEnumerable<Constant> constants,
      MemberPath outputMember,
      bool skipIsNotNull)
    {
      DbExpression cqt = (DbExpression) null;
      this.AsCql((Action) (() => cqt = (DbExpression) DbExpressionBuilder.True), (Action) (() => cqt = (DbExpression) outputMember.AsCqt(row).IsNull().Not()), (Action<Constant>) (constant =>
      {
        DbExpression right = (DbExpression) outputMember.AsCqt(row).NotEqual(constant.AsCqt(row, outputMember));
        if (cqt != null)
          cqt = (DbExpression) cqt.And(right);
        else
          cqt = right;
      }), constants, outputMember, skipIsNotNull);
      return cqt;
    }

    internal StringBuilder AsUserString(
      StringBuilder builder,
      string blockAlias,
      IEnumerable<Constant> constants,
      MemberPath outputMember,
      bool skipIsNotNull)
    {
      return this.ToStringHelper(builder, blockAlias, constants, outputMember, skipIsNotNull, true);
    }

    private void AsCql(
      Action trueLiteral,
      Action varIsNotNull,
      Action<Constant> varNotEqualsTo,
      IEnumerable<Constant> constants,
      MemberPath outputMember,
      bool skipIsNotNull)
    {
      bool isNullable = outputMember.IsNullable;
      Set<Constant> set = new Set<Constant>(this.Elements, Constant.EqualityComparer);
      foreach (Constant constant in constants)
      {
        if (!constant.Equals((object) this))
          set.Remove(constant);
      }
      if (set.Count == 0)
      {
        trueLiteral();
      }
      else
      {
        bool flag = set.Contains(Constant.Null);
        set.Remove(Constant.Null);
        if (flag || isNullable && !skipIsNotNull)
          varIsNotNull();
        foreach (Constant constant in set)
          varNotEqualsTo(constant);
      }
    }

    private StringBuilder ToStringHelper(
      StringBuilder builder,
      string blockAlias,
      IEnumerable<Constant> constants,
      MemberPath outputMember,
      bool skipIsNotNull,
      bool userString)
    {
      bool anyAdded = false;
      this.AsCql((Action) (() => builder.Append("true")), (Action) (() =>
      {
        if (userString)
        {
          outputMember.ToCompactString(builder, blockAlias);
          builder.Append(" is not NULL");
        }
        else
        {
          outputMember.AsEsql(builder, blockAlias);
          builder.Append(" IS NOT NULL");
        }
        anyAdded = true;
      }), (Action<Constant>) (constant =>
      {
        if (anyAdded)
          builder.Append(" AND ");
        anyAdded = true;
        if (userString)
        {
          outputMember.ToCompactString(builder, blockAlias);
          builder.Append(" <>");
          constant.ToCompactString(builder);
        }
        else
        {
          outputMember.AsEsql(builder, blockAlias);
          builder.Append(" <>");
          constant.AsEsql(builder, outputMember, blockAlias);
        }
      }), constants, outputMember, skipIsNotNull);
      return builder;
    }

    internal override string ToUserString()
    {
      if (this.IsNotNull())
        return Strings.ViewGen_NotNull;
      StringBuilder stringBuilder1 = new StringBuilder();
      bool flag = true;
      foreach (Constant constant in this.m_negatedDomain)
      {
        if (this.m_negatedDomain.Count <= 1 || !constant.IsNull())
        {
          if (!flag)
            stringBuilder1.Append(Strings.ViewGen_CommaBlank);
          flag = false;
          stringBuilder1.Append(constant.ToUserString());
        }
      }
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.Append(Strings.ViewGen_NegatedCellConstant((object) stringBuilder1.ToString()));
      return stringBuilder2.ToString();
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      if (this.IsNotNull())
      {
        builder.Append("NOT_NULL");
      }
      else
      {
        builder.Append("NOT(");
        StringUtil.ToCommaSeparatedStringSorted(builder, (IEnumerable) this.m_negatedDomain);
        builder.Append(")");
      }
    }
  }
}
