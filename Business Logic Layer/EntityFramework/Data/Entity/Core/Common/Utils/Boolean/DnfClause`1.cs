// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.DnfClause`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class DnfClause<T_Identifier> : Clause<T_Identifier>, IEquatable<DnfClause<T_Identifier>>
  {
    internal DnfClause(Set<Literal<T_Identifier>> literals)
      : base(literals, ExprType.And)
    {
    }

    public bool Equals(DnfClause<T_Identifier> other)
    {
      if (other != null)
        return other.Literals.SetEquals(this.Literals);
      return false;
    }
  }
}
