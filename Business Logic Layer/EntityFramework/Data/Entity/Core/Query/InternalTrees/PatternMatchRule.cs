// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.PatternMatchRule
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class PatternMatchRule : Rule
  {
    private readonly Node m_pattern;

    internal PatternMatchRule(Node pattern, Rule.ProcessNodeDelegate processDelegate)
      : base(pattern.Op.OpType, processDelegate)
    {
      this.m_pattern = pattern;
    }

    private bool Match(Node pattern, Node original)
    {
      if (pattern.Op.OpType == OpType.Leaf)
        return true;
      if (pattern.Op.OpType != original.Op.OpType || pattern.Children.Count != original.Children.Count)
        return false;
      for (int index = 0; index < pattern.Children.Count; ++index)
      {
        if (!this.Match(pattern.Children[index], original.Children[index]))
          return false;
      }
      return true;
    }

    internal override bool Match(Node node)
    {
      return this.Match(this.m_pattern, node);
    }
  }
}
