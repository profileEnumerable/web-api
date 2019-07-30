// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.TrailingSpaceStringComparer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils
{
  internal class TrailingSpaceStringComparer : IEqualityComparer<string>
  {
    internal static readonly TrailingSpaceStringComparer Instance = new TrailingSpaceStringComparer();

    private TrailingSpaceStringComparer()
    {
    }

    public bool Equals(string x, string y)
    {
      return StringComparer.OrdinalIgnoreCase.Equals(TrailingSpaceStringComparer.NormalizeString(x), TrailingSpaceStringComparer.NormalizeString(y));
    }

    public int GetHashCode(string obj)
    {
      return StringComparer.OrdinalIgnoreCase.GetHashCode(TrailingSpaceStringComparer.NormalizeString(obj));
    }

    internal static string NormalizeString(string value)
    {
      if (value == null || !value.EndsWith(" ", StringComparison.Ordinal))
        return value;
      return value.TrimEnd(' ');
    }
  }
}
