// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.TrailingSpaceComparer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils
{
  internal class TrailingSpaceComparer : IEqualityComparer<object>
  {
    internal static readonly TrailingSpaceComparer Instance = new TrailingSpaceComparer();
    private static readonly IEqualityComparer<object> _template = (IEqualityComparer<object>) EqualityComparer<object>.Default;

    private TrailingSpaceComparer()
    {
    }

    bool IEqualityComparer<object>.Equals(object x, object y)
    {
      string x1 = x as string;
      if (x1 != null)
      {
        string y1 = y as string;
        if (y1 != null)
          return TrailingSpaceStringComparer.Instance.Equals(x1, y1);
      }
      return TrailingSpaceComparer._template.Equals(x, y);
    }

    int IEqualityComparer<object>.GetHashCode(object obj)
    {
      string str = obj as string;
      if (str != null)
        return TrailingSpaceStringComparer.Instance.GetHashCode(str);
      return TrailingSpaceComparer._template.GetHashCode(obj);
    }
  }
}
