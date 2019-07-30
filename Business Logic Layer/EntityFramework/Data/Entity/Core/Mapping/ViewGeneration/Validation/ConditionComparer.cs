// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Validation.ConditionComparer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Validation
{
  internal class ConditionComparer : IEqualityComparer<Dictionary<MemberPath, Set<Constant>>>
  {
    public bool Equals(
      Dictionary<MemberPath, Set<Constant>> one,
      Dictionary<MemberPath, Set<Constant>> two)
    {
      Set<MemberPath> set = new Set<MemberPath>((IEnumerable<MemberPath>) one.Keys, MemberPath.EqualityComparer);
      Set<MemberPath> other = new Set<MemberPath>((IEnumerable<MemberPath>) two.Keys, MemberPath.EqualityComparer);
      if (!set.SetEquals(other))
        return false;
      foreach (MemberPath index in set)
      {
        if (!one[index].SetEquals(two[index]))
          return false;
      }
      return true;
    }

    public int GetHashCode(Dictionary<MemberPath, Set<Constant>> obj)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (MemberPath key in obj.Keys)
        stringBuilder.Append((object) key);
      return stringBuilder.ToString().GetHashCode();
    }
  }
}
