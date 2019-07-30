// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.StringExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Data.Entity.Utilities
{
  internal static class StringExtensions
  {
    private static readonly Regex _undottedNameValidator = new Regex("^[\\p{L}\\p{Nl}_][\\p{L}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Pc}\\p{Cf}]{0,}$", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex _migrationIdPattern = new Regex("\\d{15}_.+");
    private static readonly string[] _lineEndings = new string[2]
    {
      "\r\n",
      "\n"
    };
    private const string StartCharacterExp = "[\\p{L}\\p{Nl}_]";
    private const string OtherCharacterExp = "[\\p{L}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Pc}\\p{Cf}]";
    private const string NameExp = "[\\p{L}\\p{Nl}_][\\p{L}\\p{Nl}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Pc}\\p{Cf}]{0,}";

    public static bool EqualsIgnoreCase(this string s1, string s2)
    {
      return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
    }

    internal static bool EqualsOrdinal(this string s1, string s2)
    {
      return string.Equals(s1, s2, StringComparison.Ordinal);
    }

    public static string MigrationName(this string migrationId)
    {
      return migrationId.Substring(16);
    }

    public static string RestrictTo(this string s, int size)
    {
      if (string.IsNullOrEmpty(s) || s.Length <= size)
        return s;
      return s.Substring(0, size);
    }

    public static void EachLine(this string s, Action<string> action)
    {
      ((IEnumerable<string>) s.Split(StringExtensions._lineEndings, StringSplitOptions.None)).Each<string>(action);
    }

    public static bool IsValidMigrationId(this string migrationId)
    {
      if (!StringExtensions._migrationIdPattern.IsMatch(migrationId))
        return migrationId == "0";
      return true;
    }

    public static bool IsAutomaticMigration(this string migrationId)
    {
      return migrationId.EndsWith(Strings.AutomaticMigration, StringComparison.Ordinal);
    }

    public static string ToAutomaticMigrationId(this string migrationId)
    {
      return (Convert.ToInt64(migrationId.Substring(0, 15), (IFormatProvider) CultureInfo.InvariantCulture) - 1L).ToString() + migrationId.Substring(15) + "_" + Strings.AutomaticMigration;
    }

    public static bool IsValidUndottedName(this string name)
    {
      if (!string.IsNullOrEmpty(name))
        return StringExtensions._undottedNameValidator.IsMatch(name);
      return false;
    }
  }
}
