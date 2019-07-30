// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.OrExpr`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal class OrExpr<T_Identifier> : TreeExpr<T_Identifier>
  {
    internal OrExpr(params BoolExpr<T_Identifier>[] children)
      : this((IEnumerable<BoolExpr<T_Identifier>>) children)
    {
    }

    internal OrExpr(IEnumerable<BoolExpr<T_Identifier>> children)
      : base(children)
    {
    }

    internal override ExprType ExprType
    {
      get
      {
        return ExprType.Or;
      }
    }

    internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor)
    {
      return visitor.VisitOr(this);
    }
  }
}
