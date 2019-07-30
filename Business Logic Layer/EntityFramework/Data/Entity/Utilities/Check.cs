// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.Check
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;

namespace System.Data.Entity.Utilities
{
  internal class Check
  {
    public static T NotNull<T>(T value, string parameterName) where T : class
    {
      if ((object) value == null)
        throw new ArgumentNullException(parameterName);
      return value;
    }

    public static T? NotNull<T>(T? value, string parameterName) where T : struct
    {
      if (!value.HasValue)
        throw new ArgumentNullException(parameterName);
      return value;
    }

    public static string NotEmpty(string value, string parameterName)
    {
      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException(Strings.ArgumentIsNullOrWhitespace((object) parameterName));
      return value;
    }
  }
}
