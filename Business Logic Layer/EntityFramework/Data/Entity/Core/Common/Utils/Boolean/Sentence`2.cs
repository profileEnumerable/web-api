// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.Sentence`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal abstract class Sentence<T_Identifier, T_Clause> : NormalFormNode<T_Identifier>
    where T_Clause : Clause<T_Identifier>, IEquatable<T_Clause>
  {
    private readonly Set<T_Clause> _clauses;

    protected Sentence(Set<T_Clause> clauses, ExprType treeType)
      : base(Sentence<T_Identifier, T_Clause>.ConvertClausesToExpr(clauses, treeType))
    {
      this._clauses = clauses.AsReadOnly();
    }

    private static BoolExpr<T_Identifier> ConvertClausesToExpr(
      Set<T_Clause> clauses,
      ExprType treeType)
    {
      bool flag = ExprType.And == treeType;
      IEnumerable<BoolExpr<T_Identifier>> children = clauses.Select<T_Clause, BoolExpr<T_Identifier>>(new Func<T_Clause, BoolExpr<T_Identifier>>(NormalFormNode<T_Identifier>.ExprSelector<T_Clause>));
      if (flag)
        return (BoolExpr<T_Identifier>) new AndExpr<T_Identifier>(children);
      return (BoolExpr<T_Identifier>) new OrExpr<T_Identifier>(children);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Sentence{");
      stringBuilder.Append((object) this._clauses);
      return stringBuilder.Append("}").ToString();
    }
  }
}
