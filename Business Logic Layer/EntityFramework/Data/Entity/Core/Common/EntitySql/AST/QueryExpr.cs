// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.QueryExpr
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class QueryExpr : Node
  {
    private readonly SelectClause _selectClause;
    private readonly FromClause _fromClause;
    private readonly Node _whereClause;
    private readonly GroupByClause _groupByClause;
    private readonly HavingClause _havingClause;
    private readonly OrderByClause _orderByClause;

    internal QueryExpr(
      SelectClause selectClause,
      FromClause fromClause,
      Node whereClause,
      GroupByClause groupByClause,
      HavingClause havingClause,
      OrderByClause orderByClause)
    {
      this._selectClause = selectClause;
      this._fromClause = fromClause;
      this._whereClause = whereClause;
      this._groupByClause = groupByClause;
      this._havingClause = havingClause;
      this._orderByClause = orderByClause;
    }

    internal SelectClause SelectClause
    {
      get
      {
        return this._selectClause;
      }
    }

    internal FromClause FromClause
    {
      get
      {
        return this._fromClause;
      }
    }

    internal Node WhereClause
    {
      get
      {
        return this._whereClause;
      }
    }

    internal GroupByClause GroupByClause
    {
      get
      {
        return this._groupByClause;
      }
    }

    internal HavingClause HavingClause
    {
      get
      {
        return this._havingClause;
      }
    }

    internal OrderByClause OrderByClause
    {
      get
      {
        return this._orderByClause;
      }
    }

    internal bool HasMethodCall
    {
      get
      {
        if (this._selectClause.HasMethodCall || this._havingClause != null && this._havingClause.HasMethodCall)
          return true;
        if (this._orderByClause != null)
          return this._orderByClause.HasMethodCall;
        return false;
      }
    }
  }
}
