// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.FalseExpr`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class FalseExpr<T_Identifier> : BoolExpr<T_Identifier>
  {
    private static readonly FalseExpr<T_Identifier> _value = new FalseExpr<T_Identifier>();

    private FalseExpr()
    {
    }

    internal static FalseExpr<T_Identifier> Value
    {
      get
      {
        return FalseExpr<T_Identifier>._value;
      }
    }

    internal override ExprType ExprType
    {
      get
      {
        return ExprType.False;
      }
    }

    internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor)
    {
      return visitor.VisitFalse(this);
    }

    internal override BoolExpr<T_Identifier> MakeNegated()
    {
      return (BoolExpr<T_Identifier>) TrueExpr<T_Identifier>.Value;
    }

    protected override bool EquivalentTypeEquals(BoolExpr<T_Identifier> other)
    {
      return object.ReferenceEquals((object) this, (object) other);
    }
  }
}
