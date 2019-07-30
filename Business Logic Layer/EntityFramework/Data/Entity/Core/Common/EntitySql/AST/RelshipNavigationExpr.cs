// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.RelshipNavigationExpr
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class RelshipNavigationExpr : Node
  {
    private readonly Node _refExpr;
    private readonly Node _relshipTypeName;
    private readonly Identifier _toEndIdentifier;
    private readonly Identifier _fromEndIdentifier;

    internal RelshipNavigationExpr(
      Node refExpr,
      Node relshipTypeName,
      Identifier toEndIdentifier,
      Identifier fromEndIdentifier)
    {
      this._refExpr = refExpr;
      this._relshipTypeName = relshipTypeName;
      this._toEndIdentifier = toEndIdentifier;
      this._fromEndIdentifier = fromEndIdentifier;
    }

    internal Node RefExpr
    {
      get
      {
        return this._refExpr;
      }
    }

    internal Node TypeName
    {
      get
      {
        return this._relshipTypeName;
      }
    }

    internal Identifier ToEndIdentifier
    {
      get
      {
        return this._toEndIdentifier;
      }
    }

    internal Identifier FromEndIdentifier
    {
      get
      {
        return this._fromEndIdentifier;
      }
    }
  }
}
