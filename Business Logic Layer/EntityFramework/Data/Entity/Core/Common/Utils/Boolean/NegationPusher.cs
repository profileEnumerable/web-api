// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.NegationPusher
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Linq;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal static class NegationPusher
  {
    internal static BoolExpr<DomainConstraint<T_Variable, T_Element>> EliminateNot<T_Variable, T_Element>(
      BoolExpr<DomainConstraint<T_Variable, T_Element>> expression)
    {
      return expression.Accept<BoolExpr<DomainConstraint<T_Variable, T_Element>>>((Visitor<DomainConstraint<T_Variable, T_Element>, BoolExpr<DomainConstraint<T_Variable, T_Element>>>) NegationPusher.NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element>.Instance);
    }

    private class NonNegatedTreeVisitor<T_Identifier> : BasicVisitor<T_Identifier>
    {
      internal static readonly NegationPusher.NonNegatedTreeVisitor<T_Identifier> Instance = new NegationPusher.NonNegatedTreeVisitor<T_Identifier>();

      protected NonNegatedTreeVisitor()
      {
      }

      internal override BoolExpr<T_Identifier> VisitNot(NotExpr<T_Identifier> expression)
      {
        return expression.Child.Accept<BoolExpr<T_Identifier>>((Visitor<T_Identifier, BoolExpr<T_Identifier>>) NegationPusher.NegatedTreeVisitor<T_Identifier>.Instance);
      }
    }

    private class NegatedTreeVisitor<T_Identifier> : Visitor<T_Identifier, BoolExpr<T_Identifier>>
    {
      internal static readonly NegationPusher.NegatedTreeVisitor<T_Identifier> Instance = new NegationPusher.NegatedTreeVisitor<T_Identifier>();

      protected NegatedTreeVisitor()
      {
      }

      internal override BoolExpr<T_Identifier> VisitTrue(TrueExpr<T_Identifier> expression)
      {
        return (BoolExpr<T_Identifier>) FalseExpr<T_Identifier>.Value;
      }

      internal override BoolExpr<T_Identifier> VisitFalse(FalseExpr<T_Identifier> expression)
      {
        return (BoolExpr<T_Identifier>) TrueExpr<T_Identifier>.Value;
      }

      internal override BoolExpr<T_Identifier> VisitTerm(TermExpr<T_Identifier> expression)
      {
        return (BoolExpr<T_Identifier>) new NotExpr<T_Identifier>((BoolExpr<T_Identifier>) expression);
      }

      internal override BoolExpr<T_Identifier> VisitNot(NotExpr<T_Identifier> expression)
      {
        return expression.Child.Accept<BoolExpr<T_Identifier>>((Visitor<T_Identifier, BoolExpr<T_Identifier>>) NegationPusher.NonNegatedTreeVisitor<T_Identifier>.Instance);
      }

      internal override BoolExpr<T_Identifier> VisitAnd(AndExpr<T_Identifier> expression)
      {
        return (BoolExpr<T_Identifier>) new OrExpr<T_Identifier>(expression.Children.Select<BoolExpr<T_Identifier>, BoolExpr<T_Identifier>>((Func<BoolExpr<T_Identifier>, BoolExpr<T_Identifier>>) (child => child.Accept<BoolExpr<T_Identifier>>((Visitor<T_Identifier, BoolExpr<T_Identifier>>) this))));
      }

      internal override BoolExpr<T_Identifier> VisitOr(OrExpr<T_Identifier> expression)
      {
        return (BoolExpr<T_Identifier>) new AndExpr<T_Identifier>(expression.Children.Select<BoolExpr<T_Identifier>, BoolExpr<T_Identifier>>((Func<BoolExpr<T_Identifier>, BoolExpr<T_Identifier>>) (child => child.Accept<BoolExpr<T_Identifier>>((Visitor<T_Identifier, BoolExpr<T_Identifier>>) this))));
      }
    }

    private class NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element> : NegationPusher.NonNegatedTreeVisitor<DomainConstraint<T_Variable, T_Element>>
    {
      internal static readonly NegationPusher.NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element> Instance = new NegationPusher.NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element>();

      private NonNegatedDomainConstraintTreeVisitor()
      {
      }

      internal override BoolExpr<DomainConstraint<T_Variable, T_Element>> VisitNot(
        NotExpr<DomainConstraint<T_Variable, T_Element>> expression)
      {
        return expression.Child.Accept<BoolExpr<DomainConstraint<T_Variable, T_Element>>>((Visitor<DomainConstraint<T_Variable, T_Element>, BoolExpr<DomainConstraint<T_Variable, T_Element>>>) NegationPusher.NegatedDomainConstraintTreeVisitor<T_Variable, T_Element>.Instance);
      }
    }

    private class NegatedDomainConstraintTreeVisitor<T_Variable, T_Element> : NegationPusher.NegatedTreeVisitor<DomainConstraint<T_Variable, T_Element>>
    {
      internal static readonly NegationPusher.NegatedDomainConstraintTreeVisitor<T_Variable, T_Element> Instance = new NegationPusher.NegatedDomainConstraintTreeVisitor<T_Variable, T_Element>();

      private NegatedDomainConstraintTreeVisitor()
      {
      }

      internal override BoolExpr<DomainConstraint<T_Variable, T_Element>> VisitNot(
        NotExpr<DomainConstraint<T_Variable, T_Element>> expression)
      {
        return expression.Child.Accept<BoolExpr<DomainConstraint<T_Variable, T_Element>>>((Visitor<DomainConstraint<T_Variable, T_Element>, BoolExpr<DomainConstraint<T_Variable, T_Element>>>) NegationPusher.NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element>.Instance);
      }

      internal override BoolExpr<DomainConstraint<T_Variable, T_Element>> VisitTerm(
        TermExpr<DomainConstraint<T_Variable, T_Element>> expression)
      {
        return (BoolExpr<DomainConstraint<T_Variable, T_Element>>) new TermExpr<DomainConstraint<T_Variable, T_Element>>(expression.Identifier.InvertDomainConstraint());
      }
    }
  }
}
