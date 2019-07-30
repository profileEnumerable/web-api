// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.MethodExpr
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class MethodExpr : GroupAggregateExpr
  {
    private readonly Node _expr;
    private readonly NodeList<Node> _args;
    private readonly NodeList<RelshipNavigationExpr> _relationships;

    internal MethodExpr(Node expr, DistinctKind distinctKind, NodeList<Node> args)
      : this(expr, distinctKind, args, (NodeList<RelshipNavigationExpr>) null)
    {
    }

    internal MethodExpr(
      Node expr,
      DistinctKind distinctKind,
      NodeList<Node> args,
      NodeList<RelshipNavigationExpr> relationships)
      : base(distinctKind)
    {
      this._expr = expr;
      this._args = args;
      this._relationships = relationships;
    }

    internal Node Expr
    {
      get
      {
        return this._expr;
      }
    }

    internal NodeList<Node> Args
    {
      get
      {
        return this._args;
      }
    }

    internal bool HasRelationships
    {
      get
      {
        if (this._relationships != null)
          return this._relationships.Count > 0;
        return false;
      }
    }

    internal NodeList<RelshipNavigationExpr> Relationships
    {
      get
      {
        return this._relationships;
      }
    }
  }
}
