// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.ByteExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Utilities
{
  internal static class ByteExtensions
  {
    public static string ToHexString(this IEnumerable<byte> bytes)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in bytes)
        stringBuilder.Append(num.ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture));
      return stringBuilder.ToString();
    }
  }
}
