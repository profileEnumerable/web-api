// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.Internal.DbConnectionOptions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Data.Entity.Core.EntityClient.Internal
{
  internal class DbConnectionOptions
  {
    private readonly Dictionary<string, string> _parsetable = new Dictionary<string, string>();
    internal const string DataDirectory = "|datadirectory|";
    private readonly string _usersConnectionString;
    internal readonly NameValuePair KeyChain;

    internal DbConnectionOptions()
    {
    }

    internal DbConnectionOptions(string connectionString, IList<string> validKeywords)
    {
      this._usersConnectionString = connectionString ?? "";
      if (0 >= this._usersConnectionString.Length)
        return;
      this.KeyChain = DbConnectionOptions.ParseInternal((IDictionary<string, string>) this._parsetable, this._usersConnectionString, validKeywords);
    }

    internal string UsersConnectionString
    {
      get
      {
        return this._usersConnectionString ?? string.Empty;
      }
    }

    internal bool IsEmpty
    {
      get
      {
        return null == this.KeyChain;
      }
    }

    internal Dictionary<string, string> Parsetable
    {
      get
      {
        return this._parsetable;
      }
    }

    internal virtual string this[string keyword]
    {
      get
      {
        string str;
        this._parsetable.TryGetValue(keyword, out str);
        return str;
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    private static string GetKeyName(StringBuilder buffer)
    {
      int length = buffer.Length;
      while (0 < length && char.IsWhiteSpace(buffer[length - 1]))
        --length;
      return buffer.ToString(0, length).ToLowerInvariant();
    }

    private static string GetKeyValue(StringBuilder buffer, bool trimWhitespace)
    {
      int length = buffer.Length;
      int startIndex = 0;
      if (trimWhitespace)
      {
        while (startIndex < length && char.IsWhiteSpace(buffer[startIndex]))
          ++startIndex;
        while (0 < length && char.IsWhiteSpace(buffer[length - 1]))
          --length;
      }
      return buffer.ToString(startIndex, length - startIndex);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static int GetKeyValuePair(
      string connectionString,
      int currentPosition,
      StringBuilder buffer,
      out string keyname,
      out string keyvalue)
    {
      int num = currentPosition;
      buffer.Length = 0;
      keyname = (string) null;
      keyvalue = (string) null;
      char minValue = char.MinValue;
      DbConnectionOptions.ParserState parserState = DbConnectionOptions.ParserState.NothingYet;
      for (int length = connectionString.Length; currentPosition < length; ++currentPosition)
      {
        minValue = connectionString[currentPosition];
        switch (parserState)
        {
          case DbConnectionOptions.ParserState.NothingYet:
            if (';' != minValue && !char.IsWhiteSpace(minValue))
            {
              if (minValue == char.MinValue)
              {
                parserState = DbConnectionOptions.ParserState.NullTermination;
                continue;
              }
              if (char.IsControl(minValue))
                throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
              num = currentPosition;
              if ('=' != minValue)
              {
                parserState = DbConnectionOptions.ParserState.Key;
                break;
              }
              parserState = DbConnectionOptions.ParserState.KeyEqual;
              continue;
            }
            continue;
          case DbConnectionOptions.ParserState.Key:
            if ('=' == minValue)
            {
              parserState = DbConnectionOptions.ParserState.KeyEqual;
              continue;
            }
            if (!char.IsWhiteSpace(minValue) && char.IsControl(minValue))
              throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
            break;
          case DbConnectionOptions.ParserState.KeyEqual:
            if ('=' == minValue)
            {
              parserState = DbConnectionOptions.ParserState.Key;
              break;
            }
            keyname = DbConnectionOptions.GetKeyName(buffer);
            if (string.IsNullOrEmpty(keyname))
              throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
            buffer.Length = 0;
            parserState = DbConnectionOptions.ParserState.KeyEnd;
            goto case DbConnectionOptions.ParserState.KeyEnd;
          case DbConnectionOptions.ParserState.KeyEnd:
            if (!char.IsWhiteSpace(minValue))
            {
              if ('\'' == minValue)
              {
                parserState = DbConnectionOptions.ParserState.SingleQuoteValue;
                continue;
              }
              if ('"' == minValue)
              {
                parserState = DbConnectionOptions.ParserState.DoubleQuoteValue;
                continue;
              }
              if (';' != minValue && minValue != char.MinValue)
              {
                if (char.IsControl(minValue))
                  throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
                parserState = DbConnectionOptions.ParserState.UnquotedValue;
                break;
              }
              goto label_54;
            }
            else
              continue;
          case DbConnectionOptions.ParserState.UnquotedValue:
            if (char.IsWhiteSpace(minValue) || !char.IsControl(minValue) && ';' != minValue)
              break;
            goto label_54;
          case DbConnectionOptions.ParserState.DoubleQuoteValue:
            if ('"' == minValue)
            {
              parserState = DbConnectionOptions.ParserState.DoubleQuoteValueQuote;
              continue;
            }
            if (minValue == char.MinValue)
              throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
            break;
          case DbConnectionOptions.ParserState.DoubleQuoteValueQuote:
            if ('"' == minValue)
            {
              parserState = DbConnectionOptions.ParserState.DoubleQuoteValue;
              break;
            }
            keyvalue = DbConnectionOptions.GetKeyValue(buffer, false);
            parserState = DbConnectionOptions.ParserState.QuotedValueEnd;
            goto case DbConnectionOptions.ParserState.QuotedValueEnd;
          case DbConnectionOptions.ParserState.SingleQuoteValue:
            if ('\'' == minValue)
            {
              parserState = DbConnectionOptions.ParserState.SingleQuoteValueQuote;
              continue;
            }
            if (minValue == char.MinValue)
              throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
            break;
          case DbConnectionOptions.ParserState.SingleQuoteValueQuote:
            if ('\'' == minValue)
            {
              parserState = DbConnectionOptions.ParserState.SingleQuoteValue;
              break;
            }
            keyvalue = DbConnectionOptions.GetKeyValue(buffer, false);
            parserState = DbConnectionOptions.ParserState.QuotedValueEnd;
            goto case DbConnectionOptions.ParserState.QuotedValueEnd;
          case DbConnectionOptions.ParserState.QuotedValueEnd:
            if (!char.IsWhiteSpace(minValue))
            {
              if (';' != minValue)
              {
                if (minValue != char.MinValue)
                  throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
                parserState = DbConnectionOptions.ParserState.NullTermination;
                continue;
              }
              goto label_54;
            }
            else
              continue;
          case DbConnectionOptions.ParserState.NullTermination:
            if (minValue != char.MinValue && !char.IsWhiteSpace(minValue))
              throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) currentPosition));
            continue;
          default:
            throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1015));
        }
        buffer.Append(minValue);
      }
label_54:
      switch (parserState)
      {
        case DbConnectionOptions.ParserState.NothingYet:
        case DbConnectionOptions.ParserState.KeyEnd:
        case DbConnectionOptions.ParserState.NullTermination:
          if (';' == minValue && currentPosition < connectionString.Length)
            ++currentPosition;
          return currentPosition;
        case DbConnectionOptions.ParserState.Key:
        case DbConnectionOptions.ParserState.DoubleQuoteValue:
        case DbConnectionOptions.ParserState.SingleQuoteValue:
          throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
        case DbConnectionOptions.ParserState.KeyEqual:
          keyname = DbConnectionOptions.GetKeyName(buffer);
          if (string.IsNullOrEmpty(keyname))
            throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
          goto case DbConnectionOptions.ParserState.NothingYet;
        case DbConnectionOptions.ParserState.UnquotedValue:
          keyvalue = DbConnectionOptions.GetKeyValue(buffer, true);
          char ch = keyvalue[keyvalue.Length - 1];
          if ('\'' == ch || '"' == ch)
            throw new ArgumentException(Strings.ADP_ConnectionStringSyntax((object) num));
          goto case DbConnectionOptions.ParserState.NothingYet;
        case DbConnectionOptions.ParserState.DoubleQuoteValueQuote:
        case DbConnectionOptions.ParserState.SingleQuoteValueQuote:
        case DbConnectionOptions.ParserState.QuotedValueEnd:
          keyvalue = DbConnectionOptions.GetKeyValue(buffer, false);
          goto case DbConnectionOptions.ParserState.NothingYet;
        default:
          throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1016));
      }
    }

    private static NameValuePair ParseInternal(
      IDictionary<string, string> parsetable,
      string connectionString,
      IList<string> validKeywords)
    {
      StringBuilder buffer = new StringBuilder();
      NameValuePair nameValuePair1 = (NameValuePair) null;
      NameValuePair nameValuePair2 = (NameValuePair) null;
      int num = 0;
      int length = connectionString.Length;
      while (num < length)
      {
        int currentPosition = num;
        string keyname;
        string keyvalue;
        num = DbConnectionOptions.GetKeyValuePair(connectionString, currentPosition, buffer, out keyname, out keyvalue);
        if (!string.IsNullOrEmpty(keyname))
        {
          if (!validKeywords.Contains(keyname))
            throw new ArgumentException(Strings.ADP_KeywordNotSupported((object) keyname));
          parsetable[keyname] = keyvalue;
          if (nameValuePair1 != null)
            nameValuePair1 = nameValuePair1.Next = new NameValuePair();
          else
            nameValuePair2 = nameValuePair1 = new NameValuePair();
        }
        else
          break;
      }
      return nameValuePair2;
    }

    private enum ParserState
    {
      NothingYet = 1,
      Key = 2,
      KeyEqual = 3,
      KeyEnd = 4,
      UnquotedValue = 5,
      DoubleQuoteValue = 6,
      DoubleQuoteValueQuote = 7,
      SingleQuoteValue = 8,
      SingleQuoteValueQuote = 9,
      QuotedValueEnd = 10, // 0x0000000A
      NullTermination = 11, // 0x0000000B
    }
  }
}
