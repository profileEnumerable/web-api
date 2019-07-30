// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.StringUtil
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Common.Utils
{
  internal static class StringUtil
  {
    private const string s_defaultDelimiter = ", ";

    internal static string BuildDelimitedList<T>(
      IEnumerable<T> values,
      StringUtil.ToStringConverter<T> converter,
      string delimiter)
    {
      if (values == null)
        return string.Empty;
      if (converter == null)
        converter = new StringUtil.ToStringConverter<T>(StringUtil.InvariantConvertToString<T>);
      if (delimiter == null)
        delimiter = ", ";
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (T obj in values)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(delimiter);
        stringBuilder.Append(converter(obj));
      }
      return stringBuilder.ToString();
    }

    internal static string ToCommaSeparatedString(IEnumerable list)
    {
      return StringUtil.ToSeparatedString(list, ", ", string.Empty);
    }

    internal static string ToSeparatedString(IEnumerable list, string separator, string nullValue)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StringUtil.ToSeparatedString(stringBuilder, list, separator, nullValue);
      return stringBuilder.ToString();
    }

    internal static string ToCommaSeparatedStringSorted(IEnumerable list)
    {
      return StringUtil.ToSeparatedStringSorted(list, ", ", string.Empty);
    }

    internal static string ToSeparatedStringSorted(
      IEnumerable list,
      string separator,
      string nullValue)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StringUtil.ToSeparatedStringPrivate(stringBuilder, list, separator, nullValue, true);
      return stringBuilder.ToString();
    }

    internal static string MembersToCommaSeparatedString(IEnumerable members)
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("{");
      StringUtil.ToCommaSeparatedString(builder, members);
      builder.Append("}");
      return builder.ToString();
    }

    internal static void ToCommaSeparatedString(StringBuilder builder, IEnumerable list)
    {
      StringUtil.ToSeparatedStringPrivate(builder, list, ", ", string.Empty, false);
    }

    internal static void ToCommaSeparatedStringSorted(StringBuilder builder, IEnumerable list)
    {
      StringUtil.ToSeparatedStringPrivate(builder, list, ", ", string.Empty, true);
    }

    internal static void ToSeparatedString(
      StringBuilder builder,
      IEnumerable list,
      string separator)
    {
      StringUtil.ToSeparatedStringPrivate(builder, list, separator, string.Empty, false);
    }

    internal static void ToSeparatedStringSorted(
      StringBuilder builder,
      IEnumerable list,
      string separator)
    {
      StringUtil.ToSeparatedStringPrivate(builder, list, separator, string.Empty, true);
    }

    internal static void ToSeparatedString(
      StringBuilder stringBuilder,
      IEnumerable list,
      string separator,
      string nullValue)
    {
      StringUtil.ToSeparatedStringPrivate(stringBuilder, list, separator, nullValue, false);
    }

    private static void ToSeparatedStringPrivate(
      StringBuilder stringBuilder,
      IEnumerable list,
      string separator,
      string nullValue,
      bool toSort)
    {
      if (list == null)
        return;
      bool flag = true;
      List<string> stringList = new List<string>();
      foreach (object obj in list)
      {
        string str;
        if (obj == null)
          str = nullValue;
        else
          str = StringUtil.FormatInvariant("{0}", obj);
        stringList.Add(str);
      }
      if (toSort)
        stringList.Sort((IComparer<string>) StringComparer.Ordinal);
      foreach (string str in stringList)
      {
        if (!flag)
          stringBuilder.Append(separator);
        stringBuilder.Append(str);
        flag = false;
      }
    }

    internal static string FormatInvariant(string format, params object[] args)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);
    }

    internal static StringBuilder FormatStringBuilder(
      StringBuilder builder,
      string format,
      params object[] args)
    {
      builder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, format, args);
      return builder;
    }

    internal static StringBuilder IndentNewLine(StringBuilder builder, int indent)
    {
      builder.AppendLine();
      for (int index = 0; index < indent; ++index)
        builder.Append("    ");
      return builder;
    }

    internal static string FormatIndex(string arrayVarName, int index)
    {
      return new StringBuilder(arrayVarName.Length + 10 + 2).Append(arrayVarName).Append('[').Append(index).Append(']').ToString();
    }

    private static string InvariantConvertToString<T>(T value)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) value);
    }

    internal delegate string ToStringConverter<T>(T value);
  }
}
