// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.HavingClause
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class HavingClause : Node
  {
    private readonly Node _havingExpr;
    private readonly uint _methodCallCount;

    internal HavingClause(Node havingExpr, uint methodCallCounter)
    {
      this._havingExpr = havingExpr;
      this._methodCallCount = methodCallCounter;
    }

    internal Node HavingPredicate
    {
      get
      {
        return this._havingExpr;
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
