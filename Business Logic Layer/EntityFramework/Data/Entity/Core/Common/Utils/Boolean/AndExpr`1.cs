// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.AndExpr`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal class AndExpr<T_Identifier> : TreeExpr<T_Identifier>
  {
    internal AndExpr(params BoolExpr<T_Identifier>[] children)
      : this((IEnumerable<BoolExpr<T_Identifier>>) children)
    {
    }

    internal AndExpr(IEnumerable<BoolExpr<T_Identifier>> children)
      : base(children)
    {
    }

    internal override ExprType ExprType
    {
      get
      {
        return ExprType.And;
      }
    }

    internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor)
    {
      return visitor.VisitAnd(this);
    }
  }
}
