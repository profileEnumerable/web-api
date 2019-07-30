// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.JoinClauseItem
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class JoinClauseItem : Node
  {
    private readonly FromClauseItem _joinLeft;
    private readonly FromClauseItem _joinRight;
    private readonly Node _onExpr;

    internal JoinClauseItem(FromClauseItem joinLeft, FromClauseItem joinRight, JoinKind joinKind)
      : this(joinLeft, joinRight, joinKind, (Node) null)
    {
    }

    internal JoinClauseItem(
      FromClauseItem joinLeft,
      FromClauseItem joinRight,
      JoinKind joinKind,
      Node onExpr)
    {
      this._joinLeft = joinLeft;
      this._joinRight = joinRight;
      this.JoinKind = joinKind;
      this._onExpr = onExpr;
    }

    internal FromClauseItem LeftExpr
    {
      get
      {
        return this._joinLeft;
      }
    }

    internal FromClauseItem RightExpr
    {
      get
      {
        return this._joinRight;
      }
    }

    internal JoinKind JoinKind { get; set; }

    internal Node OnExpr
    {
      get
      {
        return this._onExpr;
      }
    }
  }
}
