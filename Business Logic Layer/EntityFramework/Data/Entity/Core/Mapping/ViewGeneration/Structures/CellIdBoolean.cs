// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.CellIdBoolean
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class CellIdBoolean : TrueFalseLiteral
  {
    private readonly int m_index;
    private readonly string m_slotName;

    internal CellIdBoolean(CqlIdentifiers identifiers, int index)
    {
      this.m_index = index;
      this.m_slotName = identifiers.GetFromVariable(index);
    }

    internal string SlotName
    {
      get
      {
        return this.m_slotName;
      }
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      string qualifiedName = CqlWriter.GetQualifiedName(blockAlias, this.SlotName);
      builder.Append(qualifiedName);
      return builder;
    }

    internal override DbExpression AsCqt(DbExpression row, bool skipIsNotNull)
    {
      return (DbExpression) row.Property(this.SlotName);
    }

    internal override StringBuilder AsUserString(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      return this.AsEsql(builder, blockAlias, skipIsNotNull);
    }

    internal override StringBuilder AsNegatedUserString(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      builder.Append("NOT(");
      builder = this.AsUserString(builder, blockAlias, skipIsNotNull);
      builder.Append(")");
      return builder;
    }

    internal override void GetRequiredSlots(
      MemberProjectionIndex projectedSlotMap,
      bool[] requiredSlots)
    {
      int numBoolSlots = requiredSlots.Length - projectedSlotMap.Count;
      int slot = projectedSlotMap.BoolIndexToSlot(this.m_index, numBoolSlots);
      requiredSlots[slot] = true;
    }

    protected override bool IsEqualTo(BoolLiteral right)
    {
      CellIdBoolean cellIdBoolean = right as CellIdBoolean;
      if (cellIdBoolean == null)
        return false;
      return this.m_index == cellIdBoolean.m_index;
    }

    public override int GetHashCode()
    {
      return this.m_index.GetHashCode();
    }

    internal override BoolLiteral RemapBool(Dictionary<MemberPath, MemberPath> remap)
    {
      return (BoolLiteral) this;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append(this.SlotName);
    }
  }
}
