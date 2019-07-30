// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.TreeNode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Text;

namespace System.Data.Entity.Core.Common.Utils
{
  internal class TreeNode
  {
    private readonly List<TreeNode> _children = new List<TreeNode>();
    private readonly StringBuilder _text;

    internal TreeNode()
    {
      this._text = new StringBuilder();
    }

    internal TreeNode(string text, params TreeNode[] children)
    {
      this._text = !string.IsNullOrEmpty(text) ? new StringBuilder(text) : new StringBuilder();
      if (children == null)
        return;
      this._children.AddRange((IEnumerable<TreeNode>) children);
    }

    internal TreeNode(string text, List<TreeNode> children)
      : this(text)
    {
      if (children == null)
        return;
      this._children.AddRange((IEnumerable<TreeNode>) children);
    }

    internal StringBuilder Text
    {
      get
      {
        return this._text;
      }
    }

    internal IList<TreeNode> Children
    {
      get
      {
        return (IList<TreeNode>) this._children;
      }
    }

    internal int Position { get; set; }
  }
}
