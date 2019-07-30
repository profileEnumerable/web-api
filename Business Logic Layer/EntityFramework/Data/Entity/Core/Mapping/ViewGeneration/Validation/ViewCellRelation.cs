// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Validation.ViewCellRelation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Validation
{
  internal class ViewCellRelation : CellRelation
  {
    private readonly Cell m_cell;
    private readonly List<ViewCellSlot> m_slots;

    internal ViewCellRelation(Cell cell, List<ViewCellSlot> slots, int cellNumber)
      : base(cellNumber)
    {
      this.m_cell = cell;
      this.m_slots = slots;
      this.m_cell.CQuery.CreateBasicCellRelation(this);
      this.m_cell.SQuery.CreateBasicCellRelation(this);
    }

    internal Cell Cell
    {
      get
      {
        return this.m_cell;
      }
    }

    internal ViewCellSlot LookupViewSlot(MemberProjectedSlot slot)
    {
      foreach (ViewCellSlot slot1 in this.m_slots)
      {
        if (ProjectedSlot.EqualityComparer.Equals((ProjectedSlot) slot, (ProjectedSlot) slot1.CSlot) || ProjectedSlot.EqualityComparer.Equals((ProjectedSlot) slot, (ProjectedSlot) slot1.SSlot))
          return slot1;
      }
      return (ViewCellSlot) null;
    }

    protected override int GetHash()
    {
      return this.m_cell.GetHashCode();
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append("ViewRel[");
      this.m_cell.ToCompactString(builder);
      builder.Append(']');
    }
  }
}
