// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.MemberProjectionIndex
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal sealed class MemberProjectionIndex : InternalBase
  {
    private readonly Dictionary<MemberPath, int> m_indexMap;
    private readonly List<MemberPath> m_members;

    internal static MemberProjectionIndex Create(
      EntitySetBase extent,
      EdmItemCollection edmItemCollection)
    {
      MemberProjectionIndex index = new MemberProjectionIndex();
      MemberProjectionIndex.GatherPartialSignature(index, edmItemCollection, new MemberPath(extent), false);
      return index;
    }

    private MemberProjectionIndex()
    {
      this.m_indexMap = new Dictionary<MemberPath, int>(MemberPath.EqualityComparer);
      this.m_members = new List<MemberPath>();
    }

    internal int Count
    {
      get
      {
        return this.m_members.Count;
      }
    }

    internal MemberPath this[int index]
    {
      get
      {
        return this.m_members[index];
      }
    }

    internal IEnumerable<int> KeySlots
    {
      get
      {
        List<int> intList = new List<int>();
        for (int slotNum = 0; slotNum < this.Count; ++slotNum)
        {
          if (this.IsKeySlot(slotNum, 0))
            intList.Add(slotNum);
        }
        return (IEnumerable<int>) intList;
      }
    }

    internal IEnumerable<MemberPath> Members
    {
      get
      {
        return (IEnumerable<MemberPath>) this.m_members;
      }
    }

    internal int IndexOf(MemberPath member)
    {
      int num;
      if (this.m_indexMap.TryGetValue(member, out num))
        return num;
      return -1;
    }

    internal int CreateIndex(MemberPath member)
    {
      int count;
      if (!this.m_indexMap.TryGetValue(member, out count))
      {
        count = this.m_indexMap.Count;
        this.m_indexMap[member] = count;
        this.m_members.Add(member);
      }
      return count;
    }

    internal MemberPath GetMemberPath(int slotNum, int numBoolSlots)
    {
      return this.IsBoolSlot(slotNum, numBoolSlots) ? (MemberPath) null : this[slotNum];
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "numBoolSlots")]
    internal int BoolIndexToSlot(int boolIndex, int numBoolSlots)
    {
      return this.Count + boolIndex;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "numBoolSlots")]
    internal int SlotToBoolIndex(int slotNum, int numBoolSlots)
    {
      return slotNum - this.Count;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "numBoolSlots")]
    internal bool IsKeySlot(int slotNum, int numBoolSlots)
    {
      if (slotNum < this.Count)
        return this[slotNum].IsPartOfKey;
      return false;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "numBoolSlots")]
    internal bool IsBoolSlot(int slotNum, int numBoolSlots)
    {
      return slotNum >= this.Count;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append('<');
      StringUtil.ToCommaSeparatedString(builder, (IEnumerable) this.m_members);
      builder.Append('>');
    }

    private static void GatherPartialSignature(
      MemberProjectionIndex index,
      EdmItemCollection edmItemCollection,
      MemberPath member,
      bool needKeysOnly)
    {
      EdmType edmType1 = member.EdmType;
      if (edmType1 is ComplexType && needKeysOnly)
        return;
      index.CreateIndex(member);
      foreach (EdmType edmType2 in MetadataHelper.GetTypeAndSubtypesOf(edmType1, (ItemCollection) edmItemCollection, false))
      {
        StructuralType possibleType = edmType2 as StructuralType;
        MemberProjectionIndex.GatherSignatureFromTypeStructuralMembers(index, edmItemCollection, member, possibleType, needKeysOnly);
      }
    }

    private static void GatherSignatureFromTypeStructuralMembers(
      MemberProjectionIndex index,
      EdmItemCollection edmItemCollection,
      MemberPath member,
      StructuralType possibleType,
      bool needKeysOnly)
    {
      foreach (EdmMember structuralMember in (IEnumerable) Helper.GetAllStructuralMembers((EdmType) possibleType))
      {
        if (MetadataHelper.IsNonRefSimpleMember(structuralMember))
        {
          if (!needKeysOnly || MetadataHelper.IsPartOfEntityTypeKey(structuralMember))
          {
            MemberPath member1 = new MemberPath(member, structuralMember);
            index.CreateIndex(member1);
          }
        }
        else
        {
          MemberPath member1 = new MemberPath(member, structuralMember);
          MemberProjectionIndex.GatherPartialSignature(index, edmItemCollection, member1, needKeysOnly || Helper.IsAssociationEndMember(structuralMember));
        }
      }
    }
  }
}
