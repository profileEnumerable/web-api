// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.SelectClause
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class SelectClause : Node
  {
    private readonly NodeList<AliasedExpr> _selectClauseItems;
    private readonly SelectKind _selectKind;
    private readonly DistinctKind _distinctKind;
    private readonly Node _topExpr;
    private readonly uint _methodCallCount;

    internal SelectClause(
      NodeList<AliasedExpr> items,
      SelectKind selectKind,
      DistinctKind distinctKind,
      Node topExpr,
      uint methodCallCount)
    {
      this._selectKind = selectKind;
      this._selectClauseItems = items;
      this._distinctKind = distinctKind;
      this._topExpr = topExpr;
      this._methodCallCount = methodCallCount;
    }

    internal NodeList<AliasedExpr> Items
    {
      get
      {
        return this._selectClauseItems;
      }
    }

    internal SelectKind SelectKind
    {
      get
      {
        return this._selectKind;
      }
    }

    internal DistinctKind DistinctKind
    {
      get
      {
        return this._distinctKind;
      }
    }

    internal Node TopExpr
    {
      get
      {
        return this._topExpr;
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
