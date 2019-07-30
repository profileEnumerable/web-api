// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration.BooleanProjectedSlot
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
  internal sealed class BooleanProjectedSlot : ProjectedSlot
  {
    private readonly BoolExpression m_expr;
    private readonly CellIdBoolean m_originalCell;

    internal BooleanProjectedSlot(
      BoolExpression expr,
      CqlIdentifiers identifiers,
      int originalCellNum)
    {
      this.m_expr = expr;
      this.m_originalCell = new CellIdBoolean(identifiers, originalCellNum);
    }

    internal override string GetCqlFieldAlias(MemberPath outputMember)
    {
      return this.m_originalCell.SlotName;
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      MemberPath outputMember,
      string blockAlias,
      int indentLevel)
    {
      if (this.m_expr.IsTrue || this.m_expr.IsFalse)
      {
        this.m_expr.AsEsql(builder, blockAlias);
      }
      else
      {
        builder.Append("CASE WHEN ");
        this.m_expr.AsEsql(builder, blockAlias);
        builder.Append(" THEN True ELSE False END");
      }
      return builder;
    }

    internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
    {
      if (this.m_expr.IsTrue || this.m_expr.IsFalse)
        return this.m_expr.AsCqt(row);
      return (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) new DbExpression[1]
      {
        this.m_expr.AsCqt(row)
      }, (IEnumerable<DbExpression>) new DbExpression[1]
      {
        (DbExpression) DbExpressionBuilder.True
      }, (DbExpression) DbExpressionBuilder.False);
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      StringUtil.FormatStringBuilder(builder, "<{0}, ", (object) this.m_originalCell.SlotName);
      this.m_expr.ToCompactString(builder);
      builder.Append('>');
    }
  }
}
