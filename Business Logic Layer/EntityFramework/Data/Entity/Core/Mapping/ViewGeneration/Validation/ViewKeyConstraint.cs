// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Validation.ViewKeyConstraint
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Validation
{
  internal class ViewKeyConstraint : KeyConstraint<ViewCellRelation, ViewCellSlot>
  {
    internal ViewKeyConstraint(ViewCellRelation relation, IEnumerable<ViewCellSlot> keySlots)
      : base(relation, keySlots, (IEqualityComparer<ViewCellSlot>) ProjectedSlot.EqualityComparer)
    {
    }

    internal Cell Cell
    {
      get
      {
        return this.CellRelation.Cell;
      }
    }

    internal bool Implies(ViewKeyConstraint second)
    {
      if (!object.ReferenceEquals((object) this.CellRelation, (object) second.CellRelation))
        return false;
      if (this.KeySlots.IsSubsetOf(second.KeySlots))
        return true;
      Set<ViewCellSlot> set = new Set<ViewCellSlot>(second.KeySlots);
      foreach (ViewCellSlot keySlot in this.KeySlots)
      {
        bool flag = false;
        foreach (ViewCellSlot element in set)
        {
          if (ProjectedSlot.EqualityComparer.Equals((ProjectedSlot) keySlot.SSlot, (ProjectedSlot) element.SSlot))
          {
            MemberPath memberPath1 = keySlot.CSlot.MemberPath;
            MemberPath memberPath2 = element.CSlot.MemberPath;
            if (MemberPath.EqualityComparer.Equals(memberPath1, memberPath2) || memberPath1.IsEquivalentViaRefConstraint(memberPath2))
            {
              set.Remove(element);
              flag = true;
              break;
            }
          }
        }
        if (!flag)
          return false;
      }
      return true;
    }

    internal static ErrorLog.Record GetErrorRecord(ViewKeyConstraint rightKeyConstraint)
    {
      List<ViewCellSlot> viewCellSlotList = new List<ViewCellSlot>((IEnumerable<ViewCellSlot>) rightKeyConstraint.KeySlots);
      EntitySetBase extent1 = viewCellSlotList[0].SSlot.MemberPath.Extent;
      EntitySetBase extent2 = viewCellSlotList[0].CSlot.MemberPath.Extent;
      MemberPath prefix1 = new MemberPath(extent1);
      MemberPath prefix2 = new MemberPath(extent2);
      ExtentKey keyForEntityType = ExtentKey.GetPrimaryKeyForEntityType(prefix1, (EntityType) extent1.ElementType);
      ExtentKey extentKey = !(extent2 is EntitySet) ? ExtentKey.GetKeyForRelationType(prefix2, (AssociationType) extent2.ElementType) : ExtentKey.GetPrimaryKeyForEntityType(prefix2, (EntityType) extent2.ElementType);
      string message = Strings.ViewGen_KeyConstraint_Violation((object) extent1.Name, (object) ViewCellSlot.SlotsToUserString((IEnumerable<ViewCellSlot>) rightKeyConstraint.KeySlots, false), (object) keyForEntityType.ToUserString(), (object) extent2.Name, (object) ViewCellSlot.SlotsToUserString((IEnumerable<ViewCellSlot>) rightKeyConstraint.KeySlots, true), (object) extentKey.ToUserString());
      string debugMessage = StringUtil.FormatInvariant("PROBLEM: Not implied {0}", (object) rightKeyConstraint);
      return new ErrorLog.Record(ViewGenErrorCode.KeyConstraintViolation, message, rightKeyConstraint.CellRelation.Cell, debugMessage);
    }

    internal static ErrorLog.Record GetErrorRecord(
      IEnumerable<ViewKeyConstraint> rightKeyConstraints)
    {
      ViewKeyConstraint viewKeyConstraint = (ViewKeyConstraint) null;
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (ViewKeyConstraint rightKeyConstraint in rightKeyConstraints)
      {
        string userString = ViewCellSlot.SlotsToUserString((IEnumerable<ViewCellSlot>) rightKeyConstraint.KeySlots, true);
        if (!flag)
          stringBuilder.Append("; ");
        flag = false;
        stringBuilder.Append(userString);
        viewKeyConstraint = rightKeyConstraint;
      }
      List<ViewCellSlot> viewCellSlotList = new List<ViewCellSlot>((IEnumerable<ViewCellSlot>) viewKeyConstraint.KeySlots);
      EntitySetBase extent1 = viewCellSlotList[0].SSlot.MemberPath.Extent;
      EntitySetBase extent2 = viewCellSlotList[0].CSlot.MemberPath.Extent;
      ExtentKey keyForEntityType = ExtentKey.GetPrimaryKeyForEntityType(new MemberPath(extent1), (EntityType) extent1.ElementType);
      string message;
      if (extent2 is EntitySet)
      {
        message = Strings.ViewGen_KeyConstraint_Update_Violation_EntitySet((object) stringBuilder.ToString(), (object) extent2.Name, (object) keyForEntityType.ToUserString(), (object) extent1.Name);
      }
      else
      {
        AssociationEndMember shouldBeMappedToKey = Helper.GetEndThatShouldBeMappedToKey(((AssociationSet) extent2).ElementType);
        message = shouldBeMappedToKey == null ? Strings.ViewGen_KeyConstraint_Update_Violation_AssociationSet((object) extent2.Name, (object) keyForEntityType.ToUserString(), (object) extent1.Name) : Strings.ViewGen_AssociationEndShouldBeMappedToKey((object) shouldBeMappedToKey.Name, (object) extent1.Name);
      }
      string debugMessage = StringUtil.FormatInvariant("PROBLEM: Not implied {0}", (object) viewKeyConstraint);
      return new ErrorLog.Record(ViewGenErrorCode.KeyConstraintUpdateViolation, message, viewKeyConstraint.CellRelation.Cell, debugMessage);
    }
  }
}
