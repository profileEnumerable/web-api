// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DebugCheck
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics;

namespace System.Data.Entity.Utilities
{
  internal class DebugCheck
  {
    [Conditional("DEBUG")]
    public static void NotNull<T>(T value) where T : class
    {
    }

    [Conditional("DEBUG")]
    public static void NotNull<T>(T? value) where T : struct
    {
    }

    [Conditional("DEBUG")]
    public static void NotEmpty(string value)
    {
    }
  }
}
