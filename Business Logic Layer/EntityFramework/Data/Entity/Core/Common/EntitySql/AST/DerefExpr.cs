// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.DerefExpr
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class DerefExpr : Node
  {
    private readonly Node _argExpr;

    internal DerefExpr(Node derefArgExpr)
    {
      this._argExpr = derefArgExpr;
    }

    internal Node ArgExpr
    {
      get
      {
        return this._argExpr;
      }
    }
  }
}
