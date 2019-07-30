// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.OrderByClauseItem
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class OrderByClauseItem : Node
  {
    private readonly Node _orderExpr;
    private readonly OrderKind _orderKind;
    private readonly Identifier _optCollationIdentifier;

    internal OrderByClauseItem(Node orderExpr, OrderKind orderKind)
      : this(orderExpr, orderKind, (Identifier) null)
    {
    }

    internal OrderByClauseItem(
      Node orderExpr,
      OrderKind orderKind,
      Identifier optCollationIdentifier)
    {
      this._orderExpr = orderExpr;
      this._orderKind = orderKind;
      this._optCollationIdentifier = optCollationIdentifier;
    }

    internal Node OrderExpr
    {
      get
      {
        return this._orderExpr;
      }
    }

    internal OrderKind OrderKind
    {
      get
      {
        return this._orderKind;
      }
    }

    internal Identifier Collation
    {
      get
      {
        return this._optCollationIdentifier;
      }
    }
  }
}
