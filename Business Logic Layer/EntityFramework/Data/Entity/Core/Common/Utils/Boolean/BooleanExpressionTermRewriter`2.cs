// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.BooleanExpressionTermRewriter`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal class BooleanExpressionTermRewriter<T_From, T_To> : Visitor<T_From, BoolExpr<T_To>>
  {
    private readonly Func<TermExpr<T_From>, BoolExpr<T_To>> _translator;

    internal BooleanExpressionTermRewriter(Func<TermExpr<T_From>, BoolExpr<T_To>> translator)
    {
      this._translator = translator;
    }

    internal override BoolExpr<T_To> VisitFalse(FalseExpr<T_From> expression)
    {
      return (BoolExpr<T_To>) FalseExpr<T_To>.Value;
    }

    internal override BoolExpr<T_To> VisitTrue(TrueExpr<T_From> expression)
    {
      return (BoolExpr<T_To>) TrueExpr<T_To>.Value;
    }

    internal override BoolExpr<T_To> VisitNot(NotExpr<T_From> expression)
    {
      return (BoolExpr<T_To>) new NotExpr<T_To>(expression.Child.Accept<BoolExpr<T_To>>((Visitor<T_From, BoolExpr<T_To>>) this));
    }

    internal override BoolExpr<T_To> VisitTerm(TermExpr<T_From> expression)
    {
      return this._translator(expression);
    }

    internal override BoolExpr<T_To> VisitAnd(AndExpr<T_From> expression)
    {
      return (BoolExpr<T_To>) new AndExpr<T_To>(this.VisitChildren((TreeExpr<T_From>) expression));
    }

    internal override BoolExpr<T_To> VisitOr(OrExpr<T_From> expression)
    {
      return (BoolExpr<T_To>) new OrExpr<T_To>(this.VisitChildren((TreeExpr<T_From>) expression));
    }

    private IEnumerable<BoolExpr<T_To>> VisitChildren(TreeExpr<T_From> expression)
    {
      foreach (BoolExpr<T_From> child in expression.Children)
        yield return child.Accept<BoolExpr<T_To>>((Visitor<T_From, BoolExpr<T_To>>) this);
    }
  }
}
