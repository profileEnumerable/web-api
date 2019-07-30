// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.Literal`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class Literal<T_Identifier> : NormalFormNode<T_Identifier>, IEquatable<Literal<T_Identifier>>
  {
    private readonly TermExpr<T_Identifier> _term;
    private readonly bool _isTermPositive;

    internal Literal(TermExpr<T_Identifier> term, bool isTermPositive)
      : base(isTermPositive ? (BoolExpr<T_Identifier>) term : (BoolExpr<T_Identifier>) new NotExpr<T_Identifier>((BoolExpr<T_Identifier>) term))
    {
      this._term = term;
      this._isTermPositive = isTermPositive;
    }

    internal TermExpr<T_Identifier> Term
    {
      get
      {
        return this._term;
      }
    }

    internal bool IsTermPositive
    {
      get
      {
        return this._isTermPositive;
      }
    }

    internal Literal<T_Identifier> MakeNegated()
    {
      return IdentifierService<T_Identifier>.Instance.NegateLiteral(this);
    }

    public override string ToString()
    {
      return StringUtil.FormatInvariant("{0}{1}", this._isTermPositive ? (object) string.Empty : (object) "!", (object) this._term);
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as Literal<T_Identifier>);
    }

    public bool Equals(Literal<T_Identifier> other)
    {
      if (other != null && other._isTermPositive == this._isTermPositive)
        return other._term.Equals(this._term);
      return false;
    }

    public override int GetHashCode()
    {
      return this._term.GetHashCode();
    }
  }
}
