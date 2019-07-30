// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration.CaseCqlBlock
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
  internal sealed class CaseCqlBlock : CqlBlock
  {
    private readonly SlotInfo m_caseSlotInfo;

    internal CaseCqlBlock(
      SlotInfo[] slots,
      int caseSlot,
      CqlBlock child,
      BoolExpression whereClause,
      CqlIdentifiers identifiers,
      int blockAliasNum)
      : base(slots, new List<CqlBlock>((IEnumerable<CqlBlock>) new CqlBlock[1]
      {
        child
      }), whereClause, identifiers, blockAliasNum)
    {
      this.m_caseSlotInfo = slots[caseSlot];
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      bool isTopLevel,
      int indentLevel)
    {
      StringUtil.IndentNewLine(builder, indentLevel);
      builder.Append("SELECT ");
      if (isTopLevel)
        builder.Append("VALUE ");
      builder.Append("-- Constructing ").Append(this.m_caseSlotInfo.OutputMember.LeafName);
      CqlBlock child = this.Children[0];
      this.GenerateProjectionEsql(builder, child.CqlAlias, true, indentLevel, isTopLevel);
      builder.Append("FROM (");
      child.AsEsql(builder, false, indentLevel + 1);
      StringUtil.IndentNewLine(builder, indentLevel);
      builder.Append(") AS ").Append(child.CqlAlias);
      if (!BoolExpression.EqualityComparer.Equals(this.WhereClause, BoolExpression.True))
      {
        StringUtil.IndentNewLine(builder, indentLevel);
        builder.Append("WHERE ");
        this.WhereClause.AsEsql(builder, child.CqlAlias);
      }
      return builder;
    }

    internal override DbExpression AsCqt(bool isTopLevel)
    {
      DbExpression source = this.Children[0].AsCqt(false);
      if (!BoolExpression.EqualityComparer.Equals(this.WhereClause, BoolExpression.True))
        source = (DbExpression) source.Where((Func<DbExpression, DbExpression>) (row => this.WhereClause.AsCqt(row)));
      return (DbExpression) source.Select<DbExpression>((Func<DbExpression, DbExpression>) (row => this.GenerateProjectionCqt(row, isTopLevel)));
    }
  }
}
