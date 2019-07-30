// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration.CqlBlock
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration
{
  internal abstract class CqlBlock : InternalBase
  {
    private ReadOnlyCollection<SlotInfo> m_slots;
    private readonly ReadOnlyCollection<CqlBlock> m_children;
    private readonly BoolExpression m_whereClause;
    private readonly string m_blockAlias;
    private CqlBlock.JoinTreeContext m_joinTreeContext;

    protected CqlBlock(
      SlotInfo[] slotInfos,
      List<CqlBlock> children,
      BoolExpression whereClause,
      CqlIdentifiers identifiers,
      int blockAliasNum)
    {
      this.m_slots = new ReadOnlyCollection<SlotInfo>((IList<SlotInfo>) slotInfos);
      this.m_children = new ReadOnlyCollection<CqlBlock>((IList<CqlBlock>) children);
      this.m_whereClause = whereClause;
      this.m_blockAlias = identifiers.GetBlockAlias(blockAliasNum);
    }

    internal ReadOnlyCollection<SlotInfo> Slots
    {
      get
      {
        return this.m_slots;
      }
      set
      {
        this.m_slots = value;
      }
    }

    protected ReadOnlyCollection<CqlBlock> Children
    {
      get
      {
        return this.m_children;
      }
    }

    protected BoolExpression WhereClause
    {
      get
      {
        return this.m_whereClause;
      }
    }

    internal string CqlAlias
    {
      get
      {
        return this.m_blockAlias;
      }
    }

    internal abstract StringBuilder AsEsql(
      StringBuilder builder,
      bool isTopLevel,
      int indentLevel);

    internal abstract DbExpression AsCqt(bool isTopLevel);

    internal QualifiedSlot QualifySlotWithBlockAlias(int slotNum)
    {
      return new QualifiedSlot(this, this.m_slots[slotNum].SlotValue);
    }

    internal ProjectedSlot SlotValue(int slotNum)
    {
      return this.m_slots[slotNum].SlotValue;
    }

    internal MemberPath MemberPath(int slotNum)
    {
      return this.m_slots[slotNum].OutputMember;
    }

    internal bool IsProjected(int slotNum)
    {
      return this.m_slots[slotNum].IsProjected;
    }

    protected void GenerateProjectionEsql(
      StringBuilder builder,
      string blockAlias,
      bool addNewLineAfterEachSlot,
      int indentLevel,
      bool isTopLevel)
    {
      bool flag = true;
      foreach (SlotInfo slot in this.Slots)
      {
        if (slot.IsRequiredByParent)
        {
          if (!flag)
            builder.Append(", ");
          if (addNewLineAfterEachSlot)
            StringUtil.IndentNewLine(builder, indentLevel + 1);
          slot.AsEsql(builder, blockAlias, indentLevel);
          if (!isTopLevel && (!(slot.SlotValue is QualifiedSlot) || slot.IsEnforcedNotNull))
            builder.Append(" AS ").Append(slot.CqlFieldAlias);
          flag = false;
        }
      }
      if (!addNewLineAfterEachSlot)
        return;
      StringUtil.IndentNewLine(builder, indentLevel);
    }

    protected DbExpression GenerateProjectionCqt(DbExpression row, bool isTopLevel)
    {
      if (isTopLevel)
        return this.Slots.Where<SlotInfo>((Func<SlotInfo, bool>) (slot => slot.IsRequiredByParent)).Single<SlotInfo>().AsCqt(row);
      return (DbExpression) DbExpressionBuilder.NewRow(this.Slots.Where<SlotInfo>((Func<SlotInfo, bool>) (slot => slot.IsRequiredByParent)).Select<SlotInfo, KeyValuePair<string, DbExpression>>((Func<SlotInfo, KeyValuePair<string, DbExpression>>) (slot => new KeyValuePair<string, DbExpression>(slot.CqlFieldAlias, slot.AsCqt(row)))));
    }

    internal void SetJoinTreeContext(IList<string> parentQualifiers, string leafQualifier)
    {
      this.m_joinTreeContext = new CqlBlock.JoinTreeContext(parentQualifiers, leafQualifier);
    }

    internal DbExpression GetInput(DbExpression row)
    {
      if (this.m_joinTreeContext == null)
        return row;
      return this.m_joinTreeContext.FindInput(row);
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      for (int index = 0; index < this.m_slots.Count; ++index)
      {
        StringUtil.FormatStringBuilder(builder, "{0}: ", (object) index);
        this.m_slots[index].ToCompactString(builder);
        builder.Append(' ');
      }
      this.m_whereClause.ToCompactString(builder);
    }

    private sealed class JoinTreeContext
    {
      private readonly IList<string> m_parentQualifiers;
      private readonly int m_indexInParentQualifiers;
      private readonly string m_leafQualifier;

      internal JoinTreeContext(IList<string> parentQualifiers, string leafQualifier)
      {
        this.m_parentQualifiers = parentQualifiers;
        this.m_indexInParentQualifiers = parentQualifiers.Count;
        this.m_leafQualifier = leafQualifier;
      }

      internal DbExpression FindInput(DbExpression row)
      {
        DbExpression instance = row;
        for (int index = this.m_parentQualifiers.Count - 1; index >= this.m_indexInParentQualifiers; --index)
          instance = (DbExpression) instance.Property(this.m_parentQualifiers[index]);
        return (DbExpression) instance.Property(this.m_leafQualifier);
      }
    }
  }
}
