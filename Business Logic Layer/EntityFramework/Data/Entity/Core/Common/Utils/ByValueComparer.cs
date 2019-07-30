// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.ByValueComparer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils
{
  internal class ByValueComparer : IComparer
  {
    internal static readonly IComparer Default = (IComparer) new ByValueComparer((IComparer) Comparer<object>.Default);
    private readonly IComparer nonByValueComparer;

    private ByValueComparer(IComparer comparer)
    {
      this.nonByValueComparer = comparer;
    }

    int IComparer.Compare(object x, object y)
    {
      if (object.ReferenceEquals(x, y))
        return 0;
      if (object.ReferenceEquals(x, (object) DBNull.Value))
        x = (object) null;
      if (object.ReferenceEquals(y, (object) DBNull.Value))
        y = (object) null;
      if (x != null && y != null)
      {
        byte[] numArray1 = x as byte[];
        byte[] numArray2 = y as byte[];
        if (numArray1 != null && numArray2 != null)
        {
          int num1 = numArray1.Length - numArray2.Length;
          if (num1 == 0)
          {
            for (int index = 0; num1 == 0 && index < numArray1.Length; ++index)
            {
              byte num2 = numArray1[index];
              byte num3 = numArray2[index];
              if ((int) num2 != (int) num3)
                num1 = (int) num2 - (int) num3;
            }
          }
          return num1;
        }
      }
      return this.nonByValueComparer.Compare(x, y);
    }
  }
}
