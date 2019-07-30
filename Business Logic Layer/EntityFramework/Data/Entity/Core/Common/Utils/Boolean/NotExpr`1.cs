// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.NotExpr`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class NotExpr<T_Identifier> : TreeExpr<T_Identifier>
  {
    internal NotExpr(BoolExpr<T_Identifier> child)
      : base((IEnumerable<BoolExpr<T_Identifier>>) new BoolExpr<T_Identifier>[1]
      {
        child
      })
    {
    }

    internal override ExprType ExprType
    {
      get
      {
        return ExprType.Not;
      }
    }

    internal BoolExpr<T_Identifier> Child
    {
      get
      {
        return this.Children.First<BoolExpr<T_Identifier>>();
      }
    }

    internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor)
    {
      return visitor.VisitNot(this);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "!{0}", (object) this.Child);
    }

    internal override BoolExpr<T_Identifier> MakeNegated()
    {
      return this.Child;
    }
  }
}
