// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.FromClauseItem
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class FromClauseItem : Node
  {
    private readonly Node _fromClauseItemExpr;
    private readonly FromClauseItemKind _fromClauseItemKind;

    internal FromClauseItem(AliasedExpr aliasExpr)
    {
      this._fromClauseItemExpr = (Node) aliasExpr;
      this._fromClauseItemKind = FromClauseItemKind.AliasedFromClause;
    }

    internal FromClauseItem(JoinClauseItem joinClauseItem)
    {
      this._fromClauseItemExpr = (Node) joinClauseItem;
      this._fromClauseItemKind = FromClauseItemKind.JoinFromClause;
    }

    internal FromClauseItem(ApplyClauseItem applyClauseItem)
    {
      this._fromClauseItemExpr = (Node) applyClauseItem;
      this._fromClauseItemKind = FromClauseItemKind.ApplyFromClause;
    }

    internal Node FromExpr
    {
      get
      {
        return this._fromClauseItemExpr;
      }
    }

    internal FromClauseItemKind FromClauseItemKind
    {
      get
      {
        return this._fromClauseItemKind;
      }
    }
  }
}
