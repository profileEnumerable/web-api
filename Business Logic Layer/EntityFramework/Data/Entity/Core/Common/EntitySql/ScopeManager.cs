// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.ScopeManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class ScopeManager
  {
    private readonly List<Scope> _scopes = new List<Scope>();
    private readonly IEqualityComparer<string> _keyComparer;

    internal ScopeManager(IEqualityComparer<string> keyComparer)
    {
      this._keyComparer = keyComparer;
    }

    internal void EnterScope()
    {
      this._scopes.Add(new Scope(this._keyComparer));
    }

    internal void LeaveScope()
    {
      this._scopes.RemoveAt(this.CurrentScopeIndex);
    }

    internal int CurrentScopeIndex
    {
      get
      {
        return this._scopes.Count - 1;
      }
    }

    internal Scope CurrentScope
    {
      get
      {
        return this._scopes[this.CurrentScopeIndex];
      }
    }

    internal Scope GetScopeByIndex(int scopeIndex)
    {
      if (0 > scopeIndex || scopeIndex > this.CurrentScopeIndex)
        throw new EntitySqlException(Strings.InvalidScopeIndex);
      return this._scopes[scopeIndex];
    }

    internal void RollbackToScope(int scopeIndex)
    {
      if (scopeIndex > this.CurrentScopeIndex || scopeIndex < 0 || this.CurrentScopeIndex < 0)
        throw new EntitySqlException(Strings.InvalidSavePoint);
      if (this.CurrentScopeIndex - scopeIndex <= 0)
        return;
      this._scopes.RemoveRange(scopeIndex + 1, this.CurrentScopeIndex - scopeIndex);
    }

    internal bool IsInCurrentScope(string key)
    {
      return this.CurrentScope.Contains(key);
    }
  }
}
