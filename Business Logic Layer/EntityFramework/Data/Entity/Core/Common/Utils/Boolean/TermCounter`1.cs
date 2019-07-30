// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.TermCounter`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal class TermCounter<T_Identifier> : Visitor<T_Identifier, int>
  {
    private static readonly TermCounter<T_Identifier> _instance = new TermCounter<T_Identifier>();

    internal static int CountTerms(BoolExpr<T_Identifier> expression)
    {
      return expression.Accept<int>((Visitor<T_Identifier, int>) TermCounter<T_Identifier>._instance);
    }

    internal override int VisitTrue(TrueExpr<T_Identifier> expression)
    {
      return 0;
    }

    internal override int VisitFalse(FalseExpr<T_Identifier> expression)
    {
      return 0;
    }

    internal override int VisitTerm(TermExpr<T_Identifier> expression)
    {
      return 1;
    }

    internal override int VisitNot(NotExpr<T_Identifier> expression)
    {
      return expression.Child.Accept<int>((Visitor<T_Identifier, int>) this);
    }

    internal override int VisitAnd(AndExpr<T_Identifier> expression)
    {
      return this.VisitTree((TreeExpr<T_Identifier>) expression);
    }

    internal override int VisitOr(OrExpr<T_Identifier> expression)
    {
      return this.VisitTree((TreeExpr<T_Identifier>) expression);
    }

    private int VisitTree(TreeExpr<T_Identifier> expression)
    {
      int num = 0;
      foreach (BoolExpr<T_Identifier> child in expression.Children)
        num += child.Accept<int>((Visitor<T_Identifier, int>) this);
      return num;
    }
  }
}
