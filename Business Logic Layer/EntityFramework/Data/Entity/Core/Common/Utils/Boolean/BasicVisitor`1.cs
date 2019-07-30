// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.BasicVisitor`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal abstract class BasicVisitor<T_Identifier> : Visitor<T_Identifier, BoolExpr<T_Identifier>>
  {
    internal override BoolExpr<T_Identifier> VisitFalse(FalseExpr<T_Identifier> expression)
    {
      return (BoolExpr<T_Identifier>) expression;
    }

    internal override BoolExpr<T_Identifier> VisitTrue(TrueExpr<T_Identifier> expression)
    {
      return (BoolExpr<T_Identifier>) expression;
    }

    internal override BoolExpr<T_Identifier> VisitTerm(TermExpr<T_Identifier> expression)
    {
      return (BoolExpr<T_Identifier>) expression;
    }

    internal override BoolExpr<T_Identifier> VisitNot(NotExpr<T_Identifier> expression)
    {
      return (BoolExpr<T_Identifier>) new NotExpr<T_Identifier>(expression.Child.Accept<BoolExpr<T_Identifier>>((Visitor<T_Identifier, BoolExpr<T_Identifier>>) this));
    }

    internal override BoolExpr<T_Identifier> VisitAnd(AndExpr<T_Identifier> expression)
    {
      return (BoolExpr<T_Identifier>) new AndExpr<T_Identifier>(this.AcceptChildren((IEnumerable<BoolExpr<T_Identifier>>) expression.Children));
    }

    internal override BoolExpr<T_Identifier> VisitOr(OrExpr<T_Identifier> expression)
    {
      return (BoolExpr<T_Identifier>) new OrExpr<T_Identifier>(this.AcceptChildren((IEnumerable<BoolExpr<T_Identifier>>) expression.Children));
    }

    private IEnumerable<BoolExpr<T_Identifier>> AcceptChildren(
      IEnumerable<BoolExpr<T_Identifier>> children)
    {
      foreach (BoolExpr<T_Identifier> child in children)
        yield return child.Accept<BoolExpr<T_Identifier>>((Visitor<T_Identifier, BoolExpr<T_Identifier>>) this);
    }
  }
}
