// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.AliasedExpr
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class AliasedExpr : Node
  {
    private readonly Node _expr;
    private readonly Identifier _alias;

    internal AliasedExpr(Node expr, Identifier alias)
    {
      if (string.IsNullOrEmpty(alias.Name))
        throw EntitySqlException.Create(alias.ErrCtx, Strings.InvalidEmptyIdentifier, (Exception) null);
      this._expr = expr;
      this._alias = alias;
    }

    internal AliasedExpr(Node expr)
    {
      this._expr = expr;
    }

    internal Node Expr
    {
      get
      {
        return this._expr;
      }
    }

    internal Identifier Alias
    {
      get
      {
        return this._alias;
      }
    }
  }
}
