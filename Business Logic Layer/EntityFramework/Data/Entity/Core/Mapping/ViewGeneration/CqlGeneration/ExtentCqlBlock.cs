// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration.ExtentCqlBlock
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration
{
  internal sealed class ExtentCqlBlock : CqlBlock
  {
    private static readonly List<CqlBlock> _emptyChildren = new List<CqlBlock>();
    private readonly EntitySetBase m_extent;
    private readonly string m_nodeTableAlias;
    private readonly CellQuery.SelectDistinct m_selectDistinct;

    internal ExtentCqlBlock(
      EntitySetBase extent,
      CellQuery.SelectDistinct selectDistinct,
      SlotInfo[] slots,
      BoolExpression whereClause,
      CqlIdentifiers identifiers,
      int blockAliasNum)
      : base(slots, ExtentCqlBlock._emptyChildren, whereClause, identifiers, blockAliasNum)
    {
      this.m_extent = extent;
      this.m_nodeTableAlias = identifiers.GetBlockAlias();
      this.m_selectDistinct = selectDistinct;
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      bool isTopLevel,
      int indentLevel)
    {
      StringUtil.IndentNewLine(builder, indentLevel);
      builder.Append("SELECT ");
      if (this.m_selectDistinct == CellQuery.SelectDistinct.Yes)
        builder.Append("DISTINCT ");
      this.GenerateProjectionEsql(builder, this.m_nodeTableAlias, true, indentLevel, isTopLevel);
      builder.Append("FROM ");
      CqlWriter.AppendEscapedQualifiedName(builder, this.m_extent.EntityContainer.Name, this.m_extent.Name);
      builder.Append(" AS ").Append(this.m_nodeTableAlias);
      if (!BoolExpression.EqualityComparer.Equals(this.WhereClause, BoolExpression.True))
      {
        StringUtil.IndentNewLine(builder, indentLevel);
        builder.Append("WHERE ");
        this.WhereClause.AsEsql(builder, this.m_nodeTableAlias);
      }
      return builder;
    }

    internal override DbExpression AsCqt(bool isTopLevel)
    {
      DbExpression source = (DbExpression) this.m_extent.Scan();
      if (!BoolExpression.EqualityComparer.Equals(this.WhereClause, BoolExpression.True))
        source = (DbExpression) source.Where((Func<DbExpression, DbExpression>) (row => this.WhereClause.AsCqt(row)));
      DbExpression dbExpression = (DbExpression) source.Select<DbExpression>((Func<DbExpression, DbExpression>) (row => this.GenerateProjectionCqt(row, isTopLevel)));
      if (this.m_selectDistinct == CellQuery.SelectDistinct.Yes)
        dbExpression = (DbExpression) dbExpression.Distinct();
      return dbExpression;
    }
  }
}
