// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.ProjectedSlot
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal abstract class ProjectedSlot : InternalBase, IEquatable<ProjectedSlot>
  {
    internal static readonly IEqualityComparer<ProjectedSlot> EqualityComparer = (IEqualityComparer<ProjectedSlot>) new ProjectedSlot.Comparer();

    protected virtual bool IsEqualTo(ProjectedSlot right)
    {
      return base.Equals((object) right);
    }

    protected virtual int GetHash()
    {
      return base.GetHashCode();
    }

    public bool Equals(ProjectedSlot right)
    {
      return ProjectedSlot.EqualityComparer.Equals(this, right);
    }

    public override bool Equals(object obj)
    {
      ProjectedSlot right = obj as ProjectedSlot;
      if (obj == null)
        return false;
      return this.Equals(right);
    }

    public override int GetHashCode()
    {
      return ProjectedSlot.EqualityComparer.GetHashCode(this);
    }

    internal virtual ProjectedSlot DeepQualify(CqlBlock block)
    {
      return (ProjectedSlot) new QualifiedSlot(block, this);
    }

    internal virtual string GetCqlFieldAlias(MemberPath outputMember)
    {
      return outputMember.CqlFieldAlias;
    }

    internal abstract StringBuilder AsEsql(
      StringBuilder builder,
      MemberPath outputMember,
      string blockAlias,
      int indentLevel);

    internal abstract DbExpression AsCqt(DbExpression row, MemberPath outputMember);

    internal static bool TryMergeRemapSlots(
      ProjectedSlot[] slots1,
      ProjectedSlot[] slots2,
      out ProjectedSlot[] result)
    {
      ProjectedSlot[] slots;
      if (!ProjectedSlot.TryMergeSlots(slots1, slots2, out slots))
      {
        result = (ProjectedSlot[]) null;
        return false;
      }
      result = slots;
      return true;
    }

    private static bool TryMergeSlots(
      ProjectedSlot[] slots1,
      ProjectedSlot[] slots2,
      out ProjectedSlot[] slots)
    {
      slots = new ProjectedSlot[slots1.Length];
      for (int index = 0; index < slots.Length; ++index)
      {
        ProjectedSlot projectedSlot1 = slots1[index];
        ProjectedSlot projectedSlot2 = slots2[index];
        if (projectedSlot1 == null)
          slots[index] = projectedSlot2;
        else if (projectedSlot2 == null)
        {
          slots[index] = projectedSlot1;
        }
        else
        {
          MemberProjectedSlot memberProjectedSlot1 = projectedSlot1 as MemberProjectedSlot;
          MemberProjectedSlot memberProjectedSlot2 = projectedSlot2 as MemberProjectedSlot;
          if (memberProjectedSlot1 != null && memberProjectedSlot2 != null && !ProjectedSlot.EqualityComparer.Equals((ProjectedSlot) memberProjectedSlot1, (ProjectedSlot) memberProjectedSlot2))
            return false;
          ProjectedSlot projectedSlot3 = memberProjectedSlot1 != null ? projectedSlot1 : projectedSlot2;
          slots[index] = projectedSlot3;
        }
      }
      return true;
    }

    private sealed class Comparer : IEqualityComparer<ProjectedSlot>
    {
      public bool Equals(ProjectedSlot left, ProjectedSlot right)
      {
        if (object.ReferenceEquals((object) left, (object) right))
          return true;
        if (left == null || right == null)
          return false;
        return left.IsEqualTo(right);
      }

      public int GetHashCode(ProjectedSlot key)
      {
        return key.GetHash();
      }
    }
  }
}
