// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.StringExtensions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.SqlServer.Resources;
using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Data.Entity.SqlServer.Utilities
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
