// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.FragmentQueryProcessor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class FragmentQueryProcessor : TileQueryProcessor<FragmentQuery>
  {
    private readonly FragmentQueryKBChaseSupport _kb;

    public FragmentQueryProcessor(FragmentQueryKBChaseSupport kb)
    {
      this._kb = kb;
    }

    internal static FragmentQueryProcessor Merge(
      FragmentQueryProcessor qp1,
      FragmentQueryProcessor qp2)
    {
      FragmentQueryKBChaseSupport kb = new FragmentQueryKBChaseSupport();
      kb.AddKnowledgeBase((System.Data.Entity.Core.Common.Utils.Boolean.KnowledgeBase<DomainConstraint<BoolLiteral, Constant>>) qp1.KnowledgeBase);
      kb.AddKnowledgeBase((System.Data.Entity.Core.Common.Utils.Boolean.KnowledgeBase<DomainConstraint<BoolLiteral, Constant>>) qp2.KnowledgeBase);
      return new FragmentQueryProcessor(kb);
    }

    internal FragmentQueryKB KnowledgeBase
    {
      get
      {
        return (FragmentQueryKB) this._kb;
      }
    }

    [SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCode", Justification = "Based on Bug VSTS Pioneer #433188: IsVisibleOutsideAssembly is wrong on generic instantiations.")]
    internal override FragmentQuery Union(FragmentQuery q1, FragmentQuery q2)
    {
      HashSet<MemberPath> memberPathSet = new HashSet<MemberPath>((IEnumerable<MemberPath>) q1.Attributes);
      memberPathSet.IntersectWith((IEnumerable<MemberPath>) q2.Attributes);
      BoolExpression or = BoolExpression.CreateOr(q1.Condition, q2.Condition);
      return FragmentQuery.Create((IEnumerable<MemberPath>) memberPathSet, or);
    }

    internal bool IsDisjointFrom(FragmentQuery q1, FragmentQuery q2)
    {
      return !this.IsSatisfiable(this.Intersect(q1, q2));
    }

    internal bool IsContainedIn(FragmentQuery q1, FragmentQuery q2)
    {
      return !this.IsSatisfiable(this.Difference(q1, q2));
    }

    internal bool IsEquivalentTo(FragmentQuery q1, FragmentQuery q2)
    {
      if (this.IsContainedIn(q1, q2))
        return this.IsContainedIn(q2, q1);
      return false;
    }

    [SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCode", Justification = "Based on Bug VSTS Pioneer #433188: IsVisibleOutsideAssembly is wrong on generic instantiations.")]
    internal override FragmentQuery Intersect(FragmentQuery q1, FragmentQuery q2)
    {
      HashSet<MemberPath> memberPathSet = new HashSet<MemberPath>((IEnumerable<MemberPath>) q1.Attributes);
      memberPathSet.IntersectWith((IEnumerable<MemberPath>) q2.Attributes);
      BoolExpression and = BoolExpression.CreateAnd(q1.Condition, q2.Condition);
      return FragmentQuery.Create((IEnumerable<MemberPath>) memberPathSet, and);
    }

    internal override FragmentQuery Difference(FragmentQuery qA, FragmentQuery qB)
    {
      return FragmentQuery.Create((IEnumerable<MemberPath>) qA.Attributes, BoolExpression.CreateAndNot(qA.Condition, qB.Condition));
    }

    internal override bool IsSatisfiable(FragmentQuery query)
    {
      return this.IsSatisfiable(query.Condition);
    }

    private bool IsSatisfiable(BoolExpression condition)
    {
      return this._kb.IsSatisfiable(condition.Tree);
    }

    internal override FragmentQuery CreateDerivedViewBySelectingConstantAttributes(
      FragmentQuery view)
    {
      HashSet<MemberPath> memberPathSet = new HashSet<MemberPath>();
      foreach (DomainVariable<BoolLiteral, Constant> variable in view.Condition.Variables)
      {
        MemberRestriction identifier1 = variable.Identifier as MemberRestriction;
        if (identifier1 != null)
        {
          MemberPath memberPath = identifier1.RestrictedMemberSlot.MemberPath;
          Domain domain = identifier1.Domain;
          if (!view.Attributes.Contains(memberPath) && !domain.AllPossibleValues.Any<Constant>((Func<Constant, bool>) (it => it.HasNotNull())))
          {
            foreach (Constant constant in domain.Values)
            {
              DomainConstraint<BoolLiteral, Constant> identifier2 = new DomainConstraint<BoolLiteral, Constant>(variable, new Set<Constant>((IEnumerable<Constant>) new Constant[1]
              {
                constant
              }, Constant.EqualityComparer));
              if (!this.IsSatisfiable(view.Condition.Create((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>(new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[2]
              {
                view.Condition.Tree,
                (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new NotExpr<DomainConstraint<BoolLiteral, Constant>>((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new TermExpr<DomainConstraint<BoolLiteral, Constant>>(identifier2))
              }))))
                memberPathSet.Add(memberPath);
            }
          }
        }
      }
      if (memberPathSet.Count <= 0)
        return (FragmentQuery) null;
      memberPathSet.UnionWith((IEnumerable<MemberPath>) view.Attributes);
      return new FragmentQuery(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "project({0})", (object) view.Description), view.FromVariable, (IEnumerable<MemberPath>) memberPathSet, view.Condition);
    }

    public override string ToString()
    {
      return this._kb.ToString();
    }

    private class AttributeSetComparator : IEqualityComparer<HashSet<MemberPath>>
    {
      [SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCode", Justification = "Based on Bug VSTS Pioneer #433188: IsVisibleOutsideAssembly is wrong on generic instantiations.")]
      public bool Equals(HashSet<MemberPath> x, HashSet<MemberPath> y)
      {
        return x.SetEquals((IEnumerable<MemberPath>) y);
      }

      public int GetHashCode(HashSet<MemberPath> attrs)
      {
        int num = 123;
        foreach (MemberPath attr in attrs)
          num += MemberPath.EqualityComparer.GetHashCode(attr) * 7;
        return num;
      }
    }
  }
}
