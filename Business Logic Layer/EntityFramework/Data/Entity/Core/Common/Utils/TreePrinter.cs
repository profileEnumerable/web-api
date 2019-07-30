// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.TreePrinter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Text;

namespace System.Data.Entity.Core.Common.Utils
{
  internal abstract class TreePrinter
  {
    private readonly List<TreeNode> _scopes = new List<TreeNode>();
    private bool _showLines = true;
    private char _horizontals = '_';
    private char _verticals = '|';

    internal virtual string Print(TreeNode node)
    {
      this.PreProcess(node);
      StringBuilder text = new StringBuilder();
      this.PrintNode(text, node);
      return text.ToString();
    }

    internal virtual void PreProcess(TreeNode node)
    {
    }

    internal virtual void AfterAppend(TreeNode node, StringBuilder text)
    {
    }

    internal virtual void BeforeAppend(TreeNode node, StringBuilder text)
    {
    }

    internal virtual void PrintNode(StringBuilder text, TreeNode node)
    {
      this.IndentLine(text);
      this.BeforeAppend(node, text);
      text.Append((object) node.Text);
      this.AfterAppend(node, text);
      this.PrintChildren(text, node);
    }

    internal virtual void PrintChildren(StringBuilder text, TreeNode node)
    {
      this._scopes.Add(node);
      node.Position = 0;
      foreach (TreeNode child in (IEnumerable<TreeNode>) node.Children)
      {
        text.AppendLine();
        ++node.Position;
        this.PrintNode(text, child);
      }
      this._scopes.RemoveAt(this._scopes.Count - 1);
    }

    private void IndentLine(StringBuilder text)
    {
      int num = 0;
      for (int index = 0; index < this._scopes.Count; ++index)
      {
        TreeNode scope = this._scopes[index];
        if (!this._showLines || scope.Position == scope.Children.Count && index != this._scopes.Count - 1)
          text.Append(' ');
        else
          text.Append(this._verticals);
        ++num;
        if (this._scopes.Count == num && this._showLines)
          text.Append(this._horizontals);
        else
          text.Append(' ');
      }
    }
  }
}
