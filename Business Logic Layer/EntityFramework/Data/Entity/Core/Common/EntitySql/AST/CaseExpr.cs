// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.CaseExpr
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class CaseExpr : Node
  {
    private readonly NodeList<WhenThenExpr> _whenThenExpr;
    private readonly Node _elseExpr;

    internal CaseExpr(NodeList<WhenThenExpr> whenThenExpr)
      : this(whenThenExpr, (Node) null)
    {
    }

    internal CaseExpr(NodeList<WhenThenExpr> whenThenExpr, Node elseExpr)
    {
      this._whenThenExpr = whenThenExpr;
      this._elseExpr = elseExpr;
    }

    internal NodeList<WhenThenExpr> WhenThenExprList
    {
      get
      {
        return this._whenThenExpr;
      }
    }

    internal Node ElseExpr
    {
      get
      {
        return this._elseExpr;
      }
    }
  }
}
