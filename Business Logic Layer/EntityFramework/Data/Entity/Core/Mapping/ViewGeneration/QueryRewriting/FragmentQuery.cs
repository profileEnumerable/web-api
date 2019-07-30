// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.FragmentQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class FragmentQuery : ITileQuery
  {
    private readonly BoolExpression m_fromVariable;
    private readonly string m_label;
    private readonly HashSet<MemberPath> m_attributes;
    private readonly BoolExpression m_condition;

    public HashSet<MemberPath> Attributes
    {
      get
      {
        return this.m_attributes;
      }
    }

    public BoolExpression Condition
    {
      get
      {
        return this.m_condition;
      }
    }

    public static FragmentQuery Create(
      BoolExpression fromVariable,
      CellQuery cellQuery)
    {
      BoolExpression condition = cellQuery.WhereClause.MakeCopy();
      condition.ExpensiveSimplify();
      return new FragmentQuery((string) null, fromVariable, (IEnumerable<MemberPath>) new HashSet<MemberPath>(cellQuery.GetProjectedMembers()), condition);
    }

    public static FragmentQuery Create(
      string label,
      RoleBoolean roleBoolean,
      CellQuery cellQuery)
    {
      BoolExpression condition = BoolExpression.CreateAnd(cellQuery.WhereClause.Create((BoolLiteral) roleBoolean), cellQuery.WhereClause).MakeCopy();
      condition.ExpensiveSimplify();
      return new FragmentQuery(label, (BoolExpression) null, (IEnumerable<MemberPath>) new HashSet<MemberPath>(), condition);
    }

    public static FragmentQuery Create(
      IEnumerable<MemberPath> attrs,
      BoolExpression whereClause)
    {
      return new FragmentQuery((string) null, (BoolExpression) null, attrs, whereClause);
    }

    public static FragmentQuery Create(BoolExpression whereClause)
    {
      return new FragmentQuery((string) null, (BoolExpression) null, (IEnumerable<MemberPath>) new MemberPath[0], whereClause);
    }

    internal FragmentQuery(
      string label,
      BoolExpression fromVariable,
      IEnumerable<MemberPath> attrs,
      BoolExpression condition)
    {
      this.m_label = label;
      this.m_fromVariable = fromVariable;
      this.m_condition = condition;
      this.m_attributes = new HashSet<MemberPath>(attrs);
    }

    public BoolExpression FromVariable
    {
      get
      {
        return this.m_fromVariable;
      }
    }

    public string Description
    {
      get
      {
        string label = this.m_label;
        if (label == null && this.m_fromVariable != null)
          label = this.m_fromVariable.ToString();
        return label;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (MemberPath attribute in this.Attributes)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(',');
        stringBuilder.Append((object) attribute);
      }
      if (this.Description != null && this.Description != stringBuilder.ToString())
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: [{1} where {2}]", (object) this.Description, (object) stringBuilder, (object) this.Condition);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0} where {1}]", (object) stringBuilder, (object) this.Condition);
    }

    internal static BoolExpression CreateMemberCondition(
      MemberPath path,
      Constant domainValue,
      MemberDomainMap domainMap)
    {
      if (domainValue is TypeConstant)
        return BoolExpression.CreateLiteral((BoolLiteral) new TypeRestriction(new MemberProjectedSlot(path), new Domain(domainValue, domainMap.GetDomain(path))), domainMap);
      return BoolExpression.CreateLiteral((BoolLiteral) new ScalarRestriction(new MemberProjectedSlot(path), new Domain(domainValue, domainMap.GetDomain(path))), domainMap);
    }

    internal static IEqualityComparer<FragmentQuery> GetEqualityComparer(
      FragmentQueryProcessor qp)
    {
      return (IEqualityComparer<FragmentQuery>) new FragmentQuery.FragmentQueryEqualityComparer(qp);
    }

    private class FragmentQueryEqualityComparer : IEqualityComparer<FragmentQuery>
    {
      private readonly FragmentQueryProcessor _qp;

      internal FragmentQueryEqualityComparer(FragmentQueryProcessor qp)
      {
        this._qp = qp;
      }

      [SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCode", Justification = "Based on Bug VSTS Pioneer #433188: IsVisibleOutsideAssembly is wrong on generic instantiations.")]
      public bool Equals(FragmentQuery x, FragmentQuery y)
      {
        if (!x.Attributes.SetEquals((IEnumerable<MemberPath>) y.Attributes))
          return false;
        return this._qp.IsEquivalentTo(x, y);
      }

      public int GetHashCode(FragmentQuery q)
      {
        int num1 = 0;
        foreach (MemberPath attribute in q.Attributes)
          num1 ^= MemberPath.EqualityComparer.GetHashCode(attribute);
        int num2 = 0;
        int num3 = 0;
        foreach (MemberRestriction memberRestriction in q.Condition.MemberRestrictions)
        {
          num2 ^= MemberPath.EqualityComparer.GetHashCode(memberRestriction.RestrictedMemberSlot.MemberPath);
          foreach (Constant constant in memberRestriction.Domain.Values)
            num3 ^= Constant.EqualityComparer.GetHashCode(constant);
        }
        return num1 * 13 + num2 * 7 + num3;
      }
    }
  }
}
