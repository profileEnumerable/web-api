// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.NamespaceImport
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class NamespaceImport : Node
  {
    private readonly Identifier _namespaceAlias;
    private readonly Node _namespaceName;

    internal NamespaceImport(Identifier idenitifier)
    {
      this._namespaceName = (Node) idenitifier;
    }

    internal NamespaceImport(DotExpr dorExpr)
    {
      this._namespaceName = (Node) dorExpr;
    }

    internal NamespaceImport(BuiltInExpr bltInExpr)
    {
      this._namespaceAlias = (Identifier) null;
      Identifier identifier = bltInExpr.Arg1 as Identifier;
      if (identifier == null)
        throw EntitySqlException.Create(bltInExpr.Arg1.ErrCtx, Strings.InvalidNamespaceAlias, (Exception) null);
      this._namespaceAlias = identifier;
      this._namespaceName = bltInExpr.Arg2;
    }

    internal Identifier Alias
    {
      get
      {
        return this._namespaceAlias;
      }
    }

    internal Node NamespaceName
    {
      get
      {
        return this._namespaceName;
      }
    }
  }
}
