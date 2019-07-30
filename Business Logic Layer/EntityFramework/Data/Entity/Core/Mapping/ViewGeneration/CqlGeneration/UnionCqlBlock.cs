// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration.UnionCqlBlock
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration
{
  internal sealed class UnionCqlBlock : CqlBlock
  {
    internal UnionCqlBlock(
      SlotInfo[] slotInfos,
      List<CqlBlock> children,
      CqlIdentifiers identifiers,
      int blockAliasNum)
      : base(slotInfos, children, BoolExpression.True, identifiers, blockAliasNum)
    {
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      bool isTopLevel,
      int indentLevel)
    {
      bool flag = true;
      foreach (CqlBlock child in this.Children)
      {
        if (!flag)
        {
          StringUtil.IndentNewLine(builder, indentLevel + 1);
          builder.Append(OpCellTreeNode.OpToEsql(CellTreeOpType.Union));
        }
        flag = false;
        builder.Append(" (");
        child.AsEsql(builder, isTopLevel, indentLevel + 1);
        builder.Append(')');
      }
      return builder;
    }

    internal override DbExpression AsCqt(bool isTopLevel)
    {
      DbExpression left = this.Children[0].AsCqt(isTopLevel);
      for (int index = 1; index < this.Children.Count; ++index)
        left = (DbExpression) left.UnionAll(this.Children[index].AsCqt(isTopLevel));
      return left;
    }
  }
}
