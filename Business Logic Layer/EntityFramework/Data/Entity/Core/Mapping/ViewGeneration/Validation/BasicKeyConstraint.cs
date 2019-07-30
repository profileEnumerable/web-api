// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Validation.BasicKeyConstraint
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Validation
{
  internal class BasicKeyConstraint : KeyConstraint<BasicCellRelation, MemberProjectedSlot>
  {
    internal BasicKeyConstraint(
      BasicCellRelation relation,
      IEnumerable<MemberProjectedSlot> keySlots)
      : base(relation, keySlots, (IEqualityComparer<MemberProjectedSlot>) ProjectedSlot.EqualityComparer)
    {
    }

    internal ViewKeyConstraint Propagate()
    {
      ViewCellRelation viewCellRelation = this.CellRelation.ViewCellRelation;
      List<ViewCellSlot> viewCellSlotList = new List<ViewCellSlot>();
      foreach (MemberProjectedSlot keySlot in this.KeySlots)
      {
        ViewCellSlot viewCellSlot = viewCellRelation.LookupViewSlot(keySlot);
        if (viewCellSlot == null)
          return (ViewKeyConstraint) null;
        viewCellSlotList.Add(viewCellSlot);
      }
      return new ViewKeyConstraint(viewCellRelation, (IEnumerable<ViewCellSlot>) viewCellSlotList);
    }
  }
}
