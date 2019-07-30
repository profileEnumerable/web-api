// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Utilities.UtcNowGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Globalization;
using System.Threading;

namespace System.Data.Entity.Migrations.Utilities
{
  internal static class UtcNowGenerator
  {
    private static readonly ThreadLocal<DateTime> _lastNow = new ThreadLocal<DateTime>((Func<DateTime>) (() => DateTime.UtcNow));
    public const string MigrationIdFormat = "yyyyMMddHHmmssf";

    public static DateTime UtcNow()
    {
      DateTime dateTime1 = DateTime.UtcNow;
      DateTime dateTime2 = UtcNowGenerator._lastNow.Value;
      if (dateTime1 <= dateTime2 || dateTime1.ToString("yyyyMMddHHmmssf", (IFormatProvider) CultureInfo.InvariantCulture).Equals(dateTime2.ToString("yyyyMMddHHmmssf", (IFormatProvider) CultureInfo.InvariantCulture), StringComparison.Ordinal))
        dateTime1 = dateTime2.AddMilliseconds(100.0);
      UtcNowGenerator._lastNow.Value = dateTime1;
      return dateTime1;
    }

    public static string UtcNowAsMigrationIdTimestamp()
    {
      return UtcNowGenerator.UtcNow().ToString("yyyyMMddHHmmssf", (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
