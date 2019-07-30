// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.TreeExpr`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal abstract class TreeExpr<T_Identifier> : BoolExpr<T_Identifier>
  {
    private readonly Set<BoolExpr<T_Identifier>> _children;
    private readonly int _hashCode;

    protected TreeExpr(IEnumerable<BoolExpr<T_Identifier>> children)
    {
      this._children = new Set<BoolExpr<T_Identifier>>(children);
      this._children.MakeReadOnly();
      this._hashCode = this._children.GetElementsHashCode();
    }

    internal Set<BoolExpr<T_Identifier>> Children
    {
      get
      {
        return this._children;
      }
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as BoolExpr<T_Identifier>);
    }

    public override int GetHashCode()
    {
      return this._hashCode;
    }

    public override string ToString()
    {
      return StringUtil.FormatInvariant("{0}({1})", (object) this.ExprType, (object) this._children);
    }

    protected override bool EquivalentTypeEquals(BoolExpr<T_Identifier> other)
    {
      return ((TreeExpr<T_Identifier>) other).Children.SetEquals(this.Children);
    }
  }
}
