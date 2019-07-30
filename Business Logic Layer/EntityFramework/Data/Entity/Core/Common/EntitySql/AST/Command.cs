// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.Command
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class Command : Node
  {
    private readonly NodeList<NamespaceImport> _namespaceImportList;
    private readonly Statement _statement;

    internal Command(NodeList<NamespaceImport> nsImportList, Statement statement)
    {
      this._namespaceImportList = nsImportList;
      this._statement = statement;
    }

    internal NodeList<NamespaceImport> NamespaceImportList
    {
      get
      {
        return this._namespaceImportList;
      }
    }

    internal Statement Statement
    {
      get
      {
        return this._statement;
      }
    }
  }
}
