// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ELinq.BindingContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Core.Objects.ELinq
{
  internal sealed class BindingContext
  {
    private readonly Stack<Binding> _scopes;

    internal BindingContext()
    {
      this._scopes = new Stack<Binding>();
    }

    internal void PushBindingScope(Binding binding)
    {
      this._scopes.Push(binding);
    }

    internal void PopBindingScope()
    {
      this._scopes.Pop();
    }

    internal bool TryGetBoundExpression(Expression linqExpression, out DbExpression cqtExpression)
    {
      cqtExpression = this._scopes.Where<Binding>((Func<Binding, bool>) (binding => binding.LinqExpression == linqExpression)).Select<Binding, DbExpression>((Func<Binding, DbExpression>) (binding => binding.CqtExpression)).FirstOrDefault<DbExpression>();
      return cqtExpression != null;
    }
  }
}
