// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.TrueExpr`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class TrueExpr<T_Identifier> : BoolExpr<T_Identifier>
  {
    private static readonly TrueExpr<T_Identifier> _value = new TrueExpr<T_Identifier>();

    private TrueExpr()
    {
    }

    internal static TrueExpr<T_Identifier> Value
    {
      get
      {
        return TrueExpr<T_Identifier>._value;
      }
    }

    internal override ExprType ExprType
    {
      get
      {
        return ExprType.True;
      }
    }

    internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor)
    {
      return visitor.VisitTrue(this);
    }

    internal override BoolExpr<T_Identifier> MakeNegated()
    {
      return (BoolExpr<T_Identifier>) FalseExpr<T_Identifier>.Value;
    }

    protected override bool EquivalentTypeEquals(BoolExpr<T_Identifier> other)
    {
      return object.ReferenceEquals((object) this, (object) other);
    }
  }
}
