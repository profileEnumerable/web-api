// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.ScopeEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal abstract class ScopeEntry
  {
    private readonly ScopeEntryKind _scopeEntryKind;

    internal ScopeEntry(ScopeEntryKind scopeEntryKind)
    {
      this._scopeEntryKind = scopeEntryKind;
    }

    internal ScopeEntryKind EntryKind
    {
      get
      {
        return this._scopeEntryKind;
      }
    }

    internal abstract DbExpression GetExpression(string refName, ErrorContext errCtx);
  }
}
