// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration.JoinCqlBlock
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration
{
  internal sealed class JoinCqlBlock : CqlBlock
  {
    private readonly CellTreeOpType m_opType;
    private readonly List<JoinCqlBlock.OnClause> m_onClauses;

    internal JoinCqlBlock(
      CellTreeOpType opType,
      SlotInfo[] slotInfos,
      List<CqlBlock> children,
      List<JoinCqlBlock.OnClause> onClauses,
      CqlIdentifiers identifiers,
      int blockAliasNum)
      : base(slotInfos, children, BoolExpression.True, identifiers, blockAliasNum)
    {
      this.m_opType = opType;
      this.m_onClauses = onClauses;
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      bool isTopLevel,
      int indentLevel)
    {
      StringUtil.IndentNewLine(builder, indentLevel);
      builder.Append("SELECT ");
      this.GenerateProjectionEsql(builder, (string) null, false, indentLevel, isTopLevel);
      StringUtil.IndentNewLine(builder, indentLevel);
      builder.Append("FROM ");
      int num = 0;
      foreach (CqlBlock child in this.Children)
      {
        if (num > 0)
        {
          StringUtil.IndentNewLine(builder, indentLevel + 1);
          builder.Append(OpCellTreeNode.OpToEsql(this.m_opType));
        }
        builder.Append(" (");
        child.AsEsql(builder, false, indentLevel + 1);
        builder.Append(") AS ").Append(child.CqlAlias);
        if (num > 0)
        {
          StringUtil.IndentNewLine(builder, indentLevel + 1);
          builder.Append("ON ");
          this.m_onClauses[num - 1].AsEsql(builder);
        }
        ++num;
      }
      return builder;
    }

    internal override DbExpression AsCqt(bool isTopLevel)
    {
      CqlBlock child1 = this.Children[0];
      DbExpression dbExpression = child1.AsCqt(false);
      List<string> stringList = new List<string>();
      for (int index = 1; index < this.Children.Count; ++index)
      {
        CqlBlock child2 = this.Children[index];
        DbExpression right = child2.AsCqt(false);
        Func<DbExpression, DbExpression, DbExpression> joinCondition = new Func<DbExpression, DbExpression, DbExpression>(this.m_onClauses[index - 1].AsCqt);
        DbJoinExpression dbJoinExpression;
        switch (this.m_opType)
        {
          case CellTreeOpType.FOJ:
            dbJoinExpression = dbExpression.FullOuterJoin(right, joinCondition);
            break;
          case CellTreeOpType.LOJ:
            dbJoinExpression = dbExpression.LeftOuterJoin(right, joinCondition);
            break;
          case CellTreeOpType.IJ:
            dbJoinExpression = dbExpression.InnerJoin(right, joinCondition);
            break;
          default:
            return (DbExpression) null;
        }
        if (index == 1)
          child1.SetJoinTreeContext((IList<string>) stringList, dbJoinExpression.Left.VariableName);
        else
          stringList.Add(dbJoinExpression.Left.VariableName);
        child2.SetJoinTreeContext((IList<string>) stringList, dbJoinExpression.Right.VariableName);
        dbExpression = (DbExpression) dbJoinExpression;
      }
      return (DbExpression) dbExpression.Select<DbExpression>((Func<DbExpression, DbExpression>) (row => this.GenerateProjectionCqt(row, false)));
    }

    internal sealed class OnClause : InternalBase
    {
      private readonly List<JoinCqlBlock.OnClause.SingleClause> m_singleClauses;

      internal OnClause()
      {
        this.m_singleClauses = new List<JoinCqlBlock.OnClause.SingleClause>();
      }

      internal void Add(
        QualifiedSlot leftSlot,
        MemberPath leftSlotOutputMember,
        QualifiedSlot rightSlot,
        MemberPath rightSlotOutputMember)
      {
        this.m_singleClauses.Add(new JoinCqlBlock.OnClause.SingleClause(leftSlot, leftSlotOutputMember, rightSlot, rightSlotOutputMember));
      }

      internal StringBuilder AsEsql(StringBuilder builder)
      {
        bool flag = true;
        foreach (JoinCqlBlock.OnClause.SingleClause singleClause in this.m_singleClauses)
        {
          if (!flag)
            builder.Append(" AND ");
          singleClause.AsEsql(builder);
          flag = false;
        }
        return builder;
      }

      internal DbExpression AsCqt(DbExpression leftRow, DbExpression rightRow)
      {
        DbExpression left = this.m_singleClauses[0].AsCqt(leftRow, rightRow);
        for (int index = 1; index < this.m_singleClauses.Count; ++index)
          left = (DbExpression) left.And(this.m_singleClauses[index].AsCqt(leftRow, rightRow));
        return left;
      }

      internal override void ToCompactString(StringBuilder builder)
      {
        builder.Append("ON ");
        StringUtil.ToSeparatedString(builder, (IEnumerable) this.m_singleClauses, " AND ");
      }

      private sealed class SingleClause : InternalBase
      {
        private readonly QualifiedSlot m_leftSlot;
        private readonly MemberPath m_leftSlotOutputMember;
        private readonly QualifiedSlot m_rightSlot;
        private readonly MemberPath m_rightSlotOutputMember;

        internal SingleClause(
          QualifiedSlot leftSlot,
          MemberPath leftSlotOutputMember,
          QualifiedSlot rightSlot,
          MemberPath rightSlotOutputMember)
        {
          this.m_leftSlot = leftSlot;
          this.m_leftSlotOutputMember = leftSlotOutputMember;
          this.m_rightSlot = rightSlot;
          this.m_rightSlotOutputMember = rightSlotOutputMember;
        }

        internal StringBuilder AsEsql(StringBuilder builder)
        {
          builder.Append(this.m_leftSlot.GetQualifiedCqlName(this.m_leftSlotOutputMember)).Append(" = ").Append(this.m_rightSlot.GetQualifiedCqlName(this.m_rightSlotOutputMember));
          return builder;
        }

        internal DbExpression AsCqt(DbExpression leftRow, DbExpression rightRow)
        {
          return (DbExpression) this.m_leftSlot.AsCqt(leftRow, this.m_leftSlotOutputMember).Equal(this.m_rightSlot.AsCqt(rightRow, this.m_rightSlotOutputMember));
        }

        internal override void ToCompactString(StringBuilder builder)
        {
          this.m_leftSlot.ToCompactString(builder);
          builder.Append(" = ");
          this.m_rightSlot.ToCompactString(builder);
        }
      }
    }
  }
}
