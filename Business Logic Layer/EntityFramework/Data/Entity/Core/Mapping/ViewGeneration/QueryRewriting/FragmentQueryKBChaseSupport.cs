// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.FragmentQueryKBChaseSupport
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class FragmentQueryKBChaseSupport : FragmentQueryKB
  {
    private Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> _residualFacts = new Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>();
    private int _residueSize = -1;
    private Dictionary<TermExpr<DomainConstraint<BoolLiteral, Constant>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>> _implications;
    private readonly FragmentQueryKBChaseSupport.AtomicConditionRuleChase _chase;
    private int _kbSize;

    internal FragmentQueryKBChaseSupport()
    {
      this._chase = new FragmentQueryKBChaseSupport.AtomicConditionRuleChase(this);
    }

    internal Dictionary<TermExpr<DomainConstraint<BoolLiteral, Constant>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>> Implications
    {
      get
      {
        if (this._implications == null)
        {
          this._implications = new Dictionary<TermExpr<DomainConstraint<BoolLiteral, Constant>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>();
          foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> fact in this.Facts)
            this.CacheFact(fact);
        }
        return this._implications;
      }
    }

    internal override void AddFact(
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> fact)
    {
      base.AddFact(fact);
      this._kbSize += fact.CountTerms();
      if (this._implications == null)
        return;
      this.CacheFact(fact);
    }

    private void CacheFact(
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> fact)
    {
      KnowledgeBase<DomainConstraint<BoolLiteral, Constant>>.Implication implication = fact as KnowledgeBase<DomainConstraint<BoolLiteral, Constant>>.Implication;
      KnowledgeBase<DomainConstraint<BoolLiteral, Constant>>.Equivalence equivalence = fact as KnowledgeBase<DomainConstraint<BoolLiteral, Constant>>.Equivalence;
      if (implication != null)
        this.CacheImplication(implication.Condition, implication.Implies);
      else if (equivalence != null)
      {
        this.CacheImplication(equivalence.Left, equivalence.Right);
        this.CacheImplication(equivalence.Right, equivalence.Left);
      }
      else
        this.CacheResidualFact(fact);
    }

    private IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> ResidueInternal
    {
      get
      {
        if (this._residueSize < 0 && this._residualFacts.Count > 0)
          this.PrepareResidue();
        return (IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) this._residualFacts;
      }
    }

    private int ResidueSize
    {
      get
      {
        if (this._residueSize < 0)
          this.PrepareResidue();
        return this._residueSize;
      }
    }

    internal BoolExpr<DomainConstraint<BoolLiteral, Constant>> Chase(
      TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
    {
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> boolExpr;
      this.Implications.TryGetValue(expression, out boolExpr);
      return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>(new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[2]
      {
        (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) expression,
        boolExpr ?? (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) TrueExpr<DomainConstraint<BoolLiteral, Constant>>.Value
      });
    }

    internal bool IsSatisfiable(
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression)
    {
      ConversionContext<DomainConstraint<BoolLiteral, Constant>> conversionContext = IdentifierService<DomainConstraint<BoolLiteral, Constant>>.Instance.CreateConversionContext();
      Converter<DomainConstraint<BoolLiteral, Constant>> converter = new Converter<DomainConstraint<BoolLiteral, Constant>>(expression, conversionContext);
      if (converter.Vertex.IsZero())
        return false;
      if (this.KbExpression.ExprType == ExprType.True)
        return true;
      int num = expression.CountTerms() + this._kbSize;
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> expr1 = converter.Dnf.Expr;
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> boolExpr = this._chase.Chase(FragmentQueryKBChaseSupport.Normalizer.ToNnfAndSplitRange(FragmentQueryKBChaseSupport.Normalizer.EstimateNnfAndSplitTermCount(expr1) > FragmentQueryKBChaseSupport.Normalizer.EstimateNnfAndSplitTermCount(expression) ? expression : expr1));
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> expr2;
      if (boolExpr.CountTerms() + this.ResidueSize > num)
      {
        expr2 = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>(new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[2]
        {
          this.KbExpression,
          expression
        });
      }
      else
      {
        expr2 = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>((IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) new List<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>(this.ResidueInternal)
        {
          boolExpr
        });
        conversionContext = IdentifierService<DomainConstraint<BoolLiteral, Constant>>.Instance.CreateConversionContext();
      }
      return !new Converter<DomainConstraint<BoolLiteral, Constant>>(expr2, conversionContext).Vertex.IsZero();
    }

    internal BoolExpr<DomainConstraint<BoolLiteral, Constant>> Chase(
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression)
    {
      if (this.Implications.Count != 0)
        return this._chase.Chase(FragmentQueryKBChaseSupport.Normalizer.ToNnfAndSplitRange(expression));
      return expression;
    }

    private void CacheImplication(
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> condition,
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> implies)
    {
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> dnf = FragmentQueryKBChaseSupport.Normalizer.ToDnf(condition, false);
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> nnfAndSplitRange = FragmentQueryKBChaseSupport.Normalizer.ToNnfAndSplitRange(implies);
      switch (dnf.ExprType)
      {
        case ExprType.Or:
          using (HashSet<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>.Enumerator enumerator = ((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) dnf).Children.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              BoolExpr<DomainConstraint<BoolLiteral, Constant>> current = enumerator.Current;
              if (current.ExprType != ExprType.Term)
                this.CacheResidualFact((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new OrExpr<DomainConstraint<BoolLiteral, Constant>>(new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[2]
                {
                  (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new NotExpr<DomainConstraint<BoolLiteral, Constant>>(current),
                  implies
                }));
              else
                this.CacheNormalizedImplication((TermExpr<DomainConstraint<BoolLiteral, Constant>>) current, nnfAndSplitRange);
            }
            break;
          }
        case ExprType.Term:
          this.CacheNormalizedImplication((TermExpr<DomainConstraint<BoolLiteral, Constant>>) dnf, nnfAndSplitRange);
          break;
        default:
          this.CacheResidualFact((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new OrExpr<DomainConstraint<BoolLiteral, Constant>>(new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[2]
          {
            (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new NotExpr<DomainConstraint<BoolLiteral, Constant>>(condition),
            implies
          }));
          break;
      }
    }

    private void CacheNormalizedImplication(
      TermExpr<DomainConstraint<BoolLiteral, Constant>> condition,
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> implies)
    {
      foreach (TermExpr<DomainConstraint<BoolLiteral, Constant>> key in this.Implications.Keys)
      {
        if (key.Identifier.Variable.Equals((object) condition.Identifier.Variable) && !key.Identifier.Range.SetEquals(condition.Identifier.Range))
        {
          this.CacheResidualFact((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new OrExpr<DomainConstraint<BoolLiteral, Constant>>(new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[2]
          {
            (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new NotExpr<DomainConstraint<BoolLiteral, Constant>>((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) condition),
            implies
          }));
          return;
        }
      }
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> expr1 = new Converter<DomainConstraint<BoolLiteral, Constant>>(this.Chase(implies), IdentifierService<DomainConstraint<BoolLiteral, Constant>>.Instance.CreateConversionContext()).Dnf.Expr;
      FragmentQueryKBChaseSupport queryKbChaseSupport = new FragmentQueryKBChaseSupport();
      queryKbChaseSupport.Implications[condition] = expr1;
      bool flag = true;
      foreach (TermExpr<DomainConstraint<BoolLiteral, Constant>> index in new Set<TermExpr<DomainConstraint<BoolLiteral, Constant>>>((IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>>) this.Implications.Keys))
      {
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expr2 = queryKbChaseSupport.Chase(this.Implications[index]);
        if (index.Equals(condition))
        {
          flag = false;
          expr2 = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>(new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[2]
          {
            expr2,
            expr1
          });
        }
        this.Implications[index] = new Converter<DomainConstraint<BoolLiteral, Constant>>(expr2, IdentifierService<DomainConstraint<BoolLiteral, Constant>>.Instance.CreateConversionContext()).Dnf.Expr;
      }
      if (flag)
        this.Implications[condition] = expr1;
      this._residueSize = -1;
    }

    private void CacheResidualFact(
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> fact)
    {
      this._residualFacts.Add(fact);
      this._residueSize = -1;
    }

    private void PrepareResidue()
    {
      int num = 0;
      if (this.Implications.Count > 0 && this._residualFacts.Count > 0)
      {
        Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> set = new Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>();
        foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> residualFact in this._residualFacts)
        {
          BoolExpr<DomainConstraint<BoolLiteral, Constant>> expr = new Converter<DomainConstraint<BoolLiteral, Constant>>(this.Chase(residualFact), IdentifierService<DomainConstraint<BoolLiteral, Constant>>.Instance.CreateConversionContext()).Dnf.Expr;
          set.Add(expr);
          num += expr.CountTerms();
          this._residueSize = num;
        }
        this._residualFacts = set;
      }
      this._residueSize = num;
    }

    private static class Normalizer
    {
      internal static BoolExpr<DomainConstraint<BoolLiteral, Constant>> ToNnfAndSplitRange(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expr)
      {
        return expr.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) FragmentQueryKBChaseSupport.Normalizer.NonNegatedTreeVisitor.Instance);
      }

      internal static int EstimateNnfAndSplitTermCount(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expr)
      {
        return expr.Accept<int>((Visitor<DomainConstraint<BoolLiteral, Constant>, int>) FragmentQueryKBChaseSupport.Normalizer.NonNegatedNnfSplitCounter.Instance);
      }

      internal static BoolExpr<DomainConstraint<BoolLiteral, Constant>> ToDnf(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expr,
        bool isNnf)
      {
        if (!isNnf)
          expr = FragmentQueryKBChaseSupport.Normalizer.ToNnfAndSplitRange(expr);
        return expr.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) FragmentQueryKBChaseSupport.Normalizer.DnfTreeVisitor.Instance);
      }

      private class NonNegatedTreeVisitor : BasicVisitor<DomainConstraint<BoolLiteral, Constant>>
      {
        internal static readonly FragmentQueryKBChaseSupport.Normalizer.NonNegatedTreeVisitor Instance = new FragmentQueryKBChaseSupport.Normalizer.NonNegatedTreeVisitor();

        private NonNegatedTreeVisitor()
        {
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitNot(
          NotExpr<DomainConstraint<BoolLiteral, Constant>> expr)
        {
          return expr.Child.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) FragmentQueryKBChaseSupport.Normalizer.NegatedTreeVisitor.Instance);
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitTerm(
          TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          switch (expression.Identifier.Range.Count)
          {
            case 0:
              return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) FalseExpr<DomainConstraint<BoolLiteral, Constant>>.Value;
            case 1:
              return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) expression;
            default:
              List<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> boolExprList = new List<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>();
              DomainVariable<BoolLiteral, Constant> variable = expression.Identifier.Variable;
              foreach (Constant constant in expression.Identifier.Range)
                boolExprList.Add((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new DomainConstraint<BoolLiteral, Constant>(variable, new Set<Constant>((IEnumerable<Constant>) new Constant[1]
                {
                  constant
                }, Constant.EqualityComparer)));
              return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new OrExpr<DomainConstraint<BoolLiteral, Constant>>((IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) boolExprList);
          }
        }
      }

      private class NegatedTreeVisitor : Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>
      {
        internal static readonly FragmentQueryKBChaseSupport.Normalizer.NegatedTreeVisitor Instance = new FragmentQueryKBChaseSupport.Normalizer.NegatedTreeVisitor();

        private NegatedTreeVisitor()
        {
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitTrue(
          TrueExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) FalseExpr<DomainConstraint<BoolLiteral, Constant>>.Value;
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitFalse(
          FalseExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) TrueExpr<DomainConstraint<BoolLiteral, Constant>>.Value;
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitNot(
          NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return expression.Child.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) FragmentQueryKBChaseSupport.Normalizer.NonNegatedTreeVisitor.Instance);
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitAnd(
          AndExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new OrExpr<DomainConstraint<BoolLiteral, Constant>>(expression.Children.Select<BoolExpr<DomainConstraint<BoolLiteral, Constant>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Func<BoolExpr<DomainConstraint<BoolLiteral, Constant>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) (child => child.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) this))));
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitOr(
          OrExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>(expression.Children.Select<BoolExpr<DomainConstraint<BoolLiteral, Constant>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Func<BoolExpr<DomainConstraint<BoolLiteral, Constant>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) (child => child.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) this))));
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitTerm(
          TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          DomainConstraint<BoolLiteral, Constant> domainConstraint = expression.Identifier.InvertDomainConstraint();
          if (domainConstraint.Range.Count == 0)
            return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) FalseExpr<DomainConstraint<BoolLiteral, Constant>>.Value;
          List<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> boolExprList = new List<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>();
          DomainVariable<BoolLiteral, Constant> variable = domainConstraint.Variable;
          foreach (Constant constant in domainConstraint.Range)
            boolExprList.Add((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new DomainConstraint<BoolLiteral, Constant>(variable, new Set<Constant>((IEnumerable<Constant>) new Constant[1]
            {
              constant
            }, Constant.EqualityComparer)));
          return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new OrExpr<DomainConstraint<BoolLiteral, Constant>>((IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) boolExprList);
        }
      }

      private class NonNegatedNnfSplitCounter : TermCounter<DomainConstraint<BoolLiteral, Constant>>
      {
        internal static readonly FragmentQueryKBChaseSupport.Normalizer.NonNegatedNnfSplitCounter Instance = new FragmentQueryKBChaseSupport.Normalizer.NonNegatedNnfSplitCounter();

        private NonNegatedNnfSplitCounter()
        {
        }

        internal override int VisitNot(
          NotExpr<DomainConstraint<BoolLiteral, Constant>> expr)
        {
          return expr.Child.Accept<int>((Visitor<DomainConstraint<BoolLiteral, Constant>, int>) FragmentQueryKBChaseSupport.Normalizer.NegatedNnfSplitCountEstimator.Instance);
        }

        internal override int VisitTerm(
          TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return expression.Identifier.Range.Count;
        }
      }

      private class NegatedNnfSplitCountEstimator : TermCounter<DomainConstraint<BoolLiteral, Constant>>
      {
        internal static readonly FragmentQueryKBChaseSupport.Normalizer.NegatedNnfSplitCountEstimator Instance = new FragmentQueryKBChaseSupport.Normalizer.NegatedNnfSplitCountEstimator();

        private NegatedNnfSplitCountEstimator()
        {
        }

        internal override int VisitNot(
          NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return expression.Child.Accept<int>((Visitor<DomainConstraint<BoolLiteral, Constant>, int>) FragmentQueryKBChaseSupport.Normalizer.NonNegatedNnfSplitCounter.Instance);
        }

        internal override int VisitTerm(
          TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return expression.Identifier.Variable.Domain.Count - expression.Identifier.Range.Count;
        }
      }

      private class DnfTreeVisitor : BasicVisitor<DomainConstraint<BoolLiteral, Constant>>
      {
        internal static readonly FragmentQueryKBChaseSupport.Normalizer.DnfTreeVisitor Instance = new FragmentQueryKBChaseSupport.Normalizer.DnfTreeVisitor();

        private DnfTreeVisitor()
        {
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitNot(
          NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) expression;
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitAnd(
          AndExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          BoolExpr<DomainConstraint<BoolLiteral, Constant>> boolExpr = base.VisitAnd(expression);
          TreeExpr<DomainConstraint<BoolLiteral, Constant>> treeExpr = boolExpr as TreeExpr<DomainConstraint<BoolLiteral, Constant>>;
          if (treeExpr == null)
            return boolExpr;
          Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> set = new Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>();
          Set<Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>> source = new Set<Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>>();
          foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> child in treeExpr.Children)
          {
            OrExpr<DomainConstraint<BoolLiteral, Constant>> orExpr = child as OrExpr<DomainConstraint<BoolLiteral, Constant>>;
            if (orExpr != null)
              source.Add(new Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>(orExpr.Children));
            else
              set.Add(child);
          }
          source.Add(new Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[1]
          {
            (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>((IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) set)
          }));
          IEnumerable<IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>> seed = (IEnumerable<IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>>) new IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>[1]
          {
            Enumerable.Empty<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>()
          };
          IEnumerable<IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>> boolExprs = source.Aggregate<Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>, IEnumerable<IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>>>(seed, (Func<IEnumerable<IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>>, Set<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>, IEnumerable<IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>>>) ((accumulator, bucket) => accumulator.SelectMany<IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>, IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>>((Func<IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>, IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>>) (accseq => (IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) bucket), (Func<IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>, IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>>) ((accseq, item) => accseq.Concat<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[1]
          {
            item
          })))));
          List<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> boolExprList = new List<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>();
          foreach (IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> children in boolExprs)
            boolExprList.Add((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>(children));
          return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new OrExpr<DomainConstraint<BoolLiteral, Constant>>((IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) boolExprList);
        }
      }
    }

    private class AtomicConditionRuleChase
    {
      private readonly FragmentQueryKBChaseSupport.AtomicConditionRuleChase.NonNegatedDomainConstraintTreeVisitor _visitor;

      internal AtomicConditionRuleChase(FragmentQueryKBChaseSupport kb)
      {
        this._visitor = new FragmentQueryKBChaseSupport.AtomicConditionRuleChase.NonNegatedDomainConstraintTreeVisitor(kb);
      }

      internal BoolExpr<DomainConstraint<BoolLiteral, Constant>> Chase(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return expression.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) this._visitor);
      }

      private class NonNegatedDomainConstraintTreeVisitor : BasicVisitor<DomainConstraint<BoolLiteral, Constant>>
      {
        private readonly FragmentQueryKBChaseSupport _kb;

        internal NonNegatedDomainConstraintTreeVisitor(FragmentQueryKBChaseSupport kb)
        {
          this._kb = kb;
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitTerm(
          TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return this._kb.Chase(expression);
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitNot(
          NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
        {
          return base.VisitNot(expression);
        }
      }
    }
  }
}
