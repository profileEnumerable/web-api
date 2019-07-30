// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.Check
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Entity.SqlServer.Resources;

namespace System.Data.Entity.SqlServer.Utilities
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
