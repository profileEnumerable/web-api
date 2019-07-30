// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.Clause`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal abstract class Clause<T_Identifier> : NormalFormNode<T_Identifier>
  {
    private readonly Set<Literal<T_Identifier>> _literals;
    private readonly int _hashCode;

    protected Clause(Set<Literal<T_Identifier>> literals, ExprType treeType)
      : base(Clause<T_Identifier>.ConvertLiteralsToExpr(literals, treeType))
    {
      this._literals = literals.AsReadOnly();
      this._hashCode = this._literals.GetElementsHashCode();
    }

    internal Set<Literal<T_Identifier>> Literals
    {
      get
      {
        return this._literals;
      }
    }

    private static BoolExpr<T_Identifier> ConvertLiteralsToExpr(
      Set<Literal<T_Identifier>> literals,
      ExprType treeType)
    {
      bool flag = ExprType.And == treeType;
      IEnumerable<BoolExpr<T_Identifier>> children = literals.Select<Literal<T_Identifier>, BoolExpr<T_Identifier>>(new Func<Literal<T_Identifier>, BoolExpr<T_Identifier>>(Clause<T_Identifier>.ConvertLiteralToExpression));
      if (flag)
        return (BoolExpr<T_Identifier>) new AndExpr<T_Identifier>(children);
      return (BoolExpr<T_Identifier>) new OrExpr<T_Identifier>(children);
    }

    private static BoolExpr<T_Identifier> ConvertLiteralToExpression(
      Literal<T_Identifier> literal)
    {
      return literal.Expr;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Clause{");
      stringBuilder.Append((object) this._literals);
      return stringBuilder.Append("}").ToString();
    }

    public override int GetHashCode()
    {
      return this._hashCode;
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }
  }
}
