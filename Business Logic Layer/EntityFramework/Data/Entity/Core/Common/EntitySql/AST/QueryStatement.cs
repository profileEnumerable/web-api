// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.QueryStatement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class QueryStatement : Statement
  {
    private readonly NodeList<FunctionDefinition> _functionDefList;
    private readonly Node _expr;

    internal QueryStatement(NodeList<FunctionDefinition> functionDefList, Node expr)
    {
      this._functionDefList = functionDefList;
      this._expr = expr;
    }

    internal NodeList<FunctionDefinition> FunctionDefList
    {
      get
      {
        return this._functionDefList;
      }
    }

    internal Node Expr
    {
      get
      {
        return this._expr;
      }
    }
  }
}
