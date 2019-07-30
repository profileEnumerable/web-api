// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.OrderByClause
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class OrderByClause : Node
  {
    private readonly NodeList<System.Data.Entity.Core.Common.EntitySql.AST.OrderByClauseItem> _orderByClauseItem;
    private readonly Node _skipExpr;
    private readonly Node _limitExpr;
    private readonly uint _methodCallCount;

    internal OrderByClause(
      NodeList<System.Data.Entity.Core.Common.EntitySql.AST.OrderByClauseItem> orderByClauseItem,
      Node skipExpr,
      Node limitExpr,
      uint methodCallCount)
    {
      this._orderByClauseItem = orderByClauseItem;
      this._skipExpr = skipExpr;
      this._limitExpr = limitExpr;
      this._methodCallCount = methodCallCount;
    }

    internal NodeList<System.Data.Entity.Core.Common.EntitySql.AST.OrderByClauseItem> OrderByClauseItem
    {
      get
      {
        return this._orderByClauseItem;
      }
    }

    internal Node SkipSubClause
    {
      get
      {
        return this._skipExpr;
      }
    }

    internal Node LimitSubClause
    {
      get
      {
        return this._limitExpr;
      }
    }

    internal bool HasMethodCall
    {
      get
      {
        return this._methodCallCount > 0U;
      }
    }
  }
}
