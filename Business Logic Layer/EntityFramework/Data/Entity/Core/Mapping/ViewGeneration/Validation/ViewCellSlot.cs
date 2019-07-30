// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Validation.ViewCellSlot
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Validation
{
  internal class ViewCellSlot : ProjectedSlot
  {
    private readonly int m_slotNum;
    private readonly MemberProjectedSlot m_cSlot;
    private readonly MemberProjectedSlot m_sSlot;

    internal ViewCellSlot(int slotNum, MemberProjectedSlot cSlot, MemberProjectedSlot sSlot)
    {
      this.m_slotNum = slotNum;
      this.m_cSlot = cSlot;
      this.m_sSlot = sSlot;
    }

    internal MemberProjectedSlot CSlot
    {
      get
      {
        return this.m_cSlot;
      }
    }

    internal MemberProjectedSlot SSlot
    {
      get
      {
        return this.m_sSlot;
      }
    }

    protected override bool IsEqualTo(ProjectedSlot right)
    {
      ViewCellSlot viewCellSlot = right as ViewCellSlot;
      if (viewCellSlot == null || this.m_slotNum != viewCellSlot.m_slotNum || !ProjectedSlot.EqualityComparer.Equals((ProjectedSlot) this.m_cSlot, (ProjectedSlot) viewCellSlot.m_cSlot))
        return false;
      return ProjectedSlot.EqualityComparer.Equals((ProjectedSlot) this.m_sSlot, (ProjectedSlot) viewCellSlot.m_sSlot);
    }

    protected override int GetHash()
    {
      return ProjectedSlot.EqualityComparer.GetHashCode((ProjectedSlot) this.m_cSlot) ^ ProjectedSlot.EqualityComparer.GetHashCode((ProjectedSlot) this.m_sSlot) ^ this.m_slotNum;
    }

    internal static string SlotsToUserString(IEnumerable<ViewCellSlot> slots, bool isFromCside)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (ViewCellSlot slot in slots)
      {
        if (!flag)
          stringBuilder.Append(", ");
        stringBuilder.Append(ViewCellSlot.SlotToUserString(slot, isFromCside));
        flag = false;
      }
      return stringBuilder.ToString();
    }

    internal static string SlotToUserString(ViewCellSlot slot, bool isFromCside)
    {
      return StringUtil.FormatInvariant("{0}", (object) (isFromCside ? slot.CSlot : slot.SSlot));
    }

    internal override string GetCqlFieldAlias(MemberPath outputMember)
    {
      return (string) null;
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      MemberPath outputMember,
      string blockAlias,
      int indentLevel)
    {
      return (StringBuilder) null;
    }

    internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
    {
      return (DbExpression) null;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append('<');
      StringUtil.FormatStringBuilder(builder, "{0}", (object) this.m_slotNum);
      builder.Append(':');
      this.m_cSlot.ToCompactString(builder);
      builder.Append('-');
      this.m_sSlot.ToCompactString(builder);
      builder.Append('>');
    }
  }
}
