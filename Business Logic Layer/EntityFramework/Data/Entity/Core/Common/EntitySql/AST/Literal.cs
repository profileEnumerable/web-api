// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.Literal
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;
using System.Globalization;

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class Literal : Node
  {
    private static readonly byte[] _emptyByteArray = new byte[0];
    private static readonly char[] _numberSuffixes = new char[10]
    {
      'U',
      'u',
      'L',
      'l',
      'F',
      'f',
      'M',
      'm',
      'D',
      'd'
    };
    private static readonly char[] _floatTokens = new char[3]
    {
      '.',
      'E',
      'e'
    };
    private static readonly char[] _datetimeSeparators = new char[4]
    {
      ' ',
      ':',
      '-',
      '.'
    };
    private static readonly char[] _datetimeOffsetSeparators = new char[6]
    {
      ' ',
      ':',
      '-',
      '.',
      '+',
      '-'
    };
    private readonly LiteralKind _literalKind;
    private string _originalValue;
    private bool _wasValueComputed;
    private object _computedValue;
    private Type _type;

    internal Literal(string originalValue, LiteralKind kind, string query, int inputPos)
      : base(query, inputPos)
    {
      this._originalValue = originalValue;
      this._literalKind = kind;
    }

    internal static Literal NewBooleanLiteral(bool value)
    {
      return new Literal(value);
    }

    private Literal(bool boolLiteral)
      : base((string) null, 0)
    {
      this._wasValueComputed = true;
      this._originalValue = string.Empty;
      this._computedValue = (object) boolLiteral;
      this._type = typeof (bool);
    }

    internal bool IsNumber
    {
      get
      {
        return this._literalKind == LiteralKind.Number;
      }
    }

    internal bool IsSignedNumber
    {
      get
      {
        if (!this.IsNumber)
          return false;
        if (this._originalValue[0] != '-')
          return this._originalValue[0] == '+';
        return true;
      }
    }

    internal bool IsString
    {
      get
      {
        if (this._literalKind != LiteralKind.String)
          return this._literalKind == LiteralKind.UnicodeString;
        return true;
      }
    }

    internal bool IsUnicodeString
    {
      get
      {
        return this._literalKind == LiteralKind.UnicodeString;
      }
    }

    internal bool IsNullLiteral
    {
      get
      {
        return this._literalKind == LiteralKind.Null;
      }
    }

    internal string OriginalValue
    {
      get
      {
        return this._originalValue;
      }
    }

    internal void PrefixSign(string sign)
    {
      this._originalValue = sign + this._originalValue;
    }

    internal object Value
    {
      get
      {
        this.ComputeValue();
        return this._computedValue;
      }
    }

    internal Type Type
    {
      get
      {
        this.ComputeValue();
        return this._type;
      }
    }

    private void ComputeValue()
    {
      if (this._wasValueComputed)
        return;
      this._wasValueComputed = true;
      switch (this._literalKind)
      {
        case LiteralKind.Number:
          this._computedValue = Literal.ConvertNumericLiteral(this.ErrCtx, this._originalValue);
          break;
        case LiteralKind.String:
          this._computedValue = (object) Literal.GetStringLiteralValue(this._originalValue, false);
          break;
        case LiteralKind.UnicodeString:
          this._computedValue = (object) Literal.GetStringLiteralValue(this._originalValue, true);
          break;
        case LiteralKind.Boolean:
          this._computedValue = (object) Literal.ConvertBooleanLiteralValue(this.ErrCtx, this._originalValue);
          break;
        case LiteralKind.Binary:
          this._computedValue = (object) Literal.ConvertBinaryLiteralValue(this._originalValue);
          break;
        case LiteralKind.DateTime:
          this._computedValue = (object) Literal.ConvertDateTimeLiteralValue(this._originalValue);
          break;
        case LiteralKind.Time:
          this._computedValue = (object) Literal.ConvertTimeLiteralValue(this._originalValue);
          break;
        case LiteralKind.DateTimeOffset:
          this._computedValue = (object) Literal.ConvertDateTimeOffsetLiteralValue(this.ErrCtx, this._originalValue);
          break;
        case LiteralKind.Guid:
          this._computedValue = (object) Literal.ConvertGuidLiteralValue(this._originalValue);
          break;
        case LiteralKind.Null:
          this._computedValue = (object) null;
          break;
        default:
          throw new NotSupportedException(Strings.LiteralTypeNotSupported((object) this._literalKind.ToString()));
      }
      this._type = this.IsNullLiteral ? (Type) null : this._computedValue.GetType();
    }

    private static object ConvertNumericLiteral(ErrorContext errCtx, string numericString)
    {
      int startIndex = numericString.IndexOfAny(Literal._numberSuffixes);
      if (-1 != startIndex)
      {
        string upperInvariant = numericString.Substring(startIndex).ToUpperInvariant();
        string s = numericString.Substring(0, numericString.Length - upperInvariant.Length);
        switch (upperInvariant)
        {
          case "U":
            uint result1;
            if (!uint.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
            {
              string errorMessage = Strings.CannotConvertNumericLiteral((object) numericString, (object) "unsigned int");
              throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
            }
            return (object) result1;
          case "L":
            long result2;
            if (!long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
            {
              string errorMessage = Strings.CannotConvertNumericLiteral((object) numericString, (object) "long");
              throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
            }
            return (object) result2;
          case "UL":
          case "LU":
            ulong result3;
            if (!ulong.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result3))
            {
              string errorMessage = Strings.CannotConvertNumericLiteral((object) numericString, (object) "unsigned long");
              throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
            }
            return (object) result3;
          case "F":
            float result4;
            if (!float.TryParse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result4))
            {
              string errorMessage = Strings.CannotConvertNumericLiteral((object) numericString, (object) "float");
              throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
            }
            return (object) result4;
          case "M":
            Decimal result5;
            if (!Decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, (IFormatProvider) CultureInfo.InvariantCulture, out result5))
            {
              string errorMessage = Strings.CannotConvertNumericLiteral((object) numericString, (object) "decimal");
              throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
            }
            return (object) result5;
          case "D":
            double result6;
            if (!double.TryParse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result6))
            {
              string errorMessage = Strings.CannotConvertNumericLiteral((object) numericString, (object) "double");
              throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
            }
            return (object) result6;
        }
      }
      return Literal.DefaultNumericConversion(numericString, errCtx);
    }

    private static object DefaultNumericConversion(string numericString, ErrorContext errCtx)
    {
      if (-1 != numericString.IndexOfAny(Literal._floatTokens))
      {
        double result;
        if (!double.TryParse(numericString, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        {
          string errorMessage = Strings.CannotConvertNumericLiteral((object) numericString, (object) "double");
          throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
        }
        return (object) result;
      }
      int result1;
      if (int.TryParse(numericString, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
        return (object) result1;
      long result2;
      if (!long.TryParse(numericString, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
      {
        string errorMessage = Strings.CannotConvertNumericLiteral((object) numericString, (object) "long");
        throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
      }
      return (object) result2;
    }

    private static bool ConvertBooleanLiteralValue(ErrorContext errCtx, string booleanLiteralValue)
    {
      bool result = false;
      if (!bool.TryParse(booleanLiteralValue, out result))
      {
        string errorMessage = Strings.InvalidLiteralFormat((object) "Boolean", (object) booleanLiteralValue);
        throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
      }
      return result;
    }

    private static string GetStringLiteralValue(string stringLiteralValue, bool isUnicode)
    {
      int startIndex = isUnicode ? 2 : 1;
      char c = stringLiteralValue[startIndex - 1];
      switch (c)
      {
        case '"':
        case '\'':
          int num = stringLiteralValue.Split(c).Length - 1;
          if (num % 2 != 0)
            throw new EntitySqlException(Strings.MalformedStringLiteralPayload);
          string str = stringLiteralValue.Substring(startIndex, stringLiteralValue.Length - (1 + startIndex)).Replace(new string(c, 2), new string(c, 1));
          if (str.Split(c).Length - 1 != (num - 2) / 2)
            throw new EntitySqlException(Strings.MalformedStringLiteralPayload);
          return str;
        default:
          throw new EntitySqlException(Strings.MalformedStringLiteralPayload);
      }
    }

    private static byte[] ConvertBinaryLiteralValue(string binaryLiteralValue)
    {
      if (string.IsNullOrEmpty(binaryLiteralValue))
        return Literal._emptyByteArray;
      int num1 = 0;
      int num2 = binaryLiteralValue.Length - 1;
      int num3 = num2 - num1 + 1;
      int length = num3 / 2;
      bool flag = 0 != num3 % 2;
      if (flag)
        ++length;
      byte[] numArray1 = new byte[length];
      int num4 = 0;
      if (flag)
        numArray1[num4++] = (byte) Literal.HexDigitToBinaryValue(binaryLiteralValue[num1++]);
      while (num1 < num2)
      {
        byte[] numArray2 = numArray1;
        int index1 = num4++;
        string str1 = binaryLiteralValue;
        int index2 = num1;
        int num5 = index2 + 1;
        int num6 = Literal.HexDigitToBinaryValue(str1[index2]) << 4;
        string str2 = binaryLiteralValue;
        int index3 = num5;
        num1 = index3 + 1;
        int binaryValue = Literal.HexDigitToBinaryValue(str2[index3]);
        int num7 = (int) (byte) (num6 | binaryValue);
        numArray2[index1] = (byte) num7;
      }
      return numArray1;
    }

    private static int HexDigitToBinaryValue(char hexChar)
    {
      if (hexChar >= '0' && hexChar <= '9')
        return (int) hexChar - 48;
      if (hexChar >= 'A' && hexChar <= 'F')
        return (int) hexChar - 65 + 10;
      if (hexChar >= 'a' && hexChar <= 'f')
        return (int) hexChar - 97 + 10;
      throw new ArgumentOutOfRangeException(nameof (hexChar));
    }

    private static DateTime ConvertDateTimeLiteralValue(string datetimeLiteralValue)
    {
      string[] datetimeParts = datetimeLiteralValue.Split(Literal._datetimeSeparators, StringSplitOptions.RemoveEmptyEntries);
      int year;
      int month;
      int day;
      Literal.GetDateParts(datetimeLiteralValue, datetimeParts, out year, out month, out day);
      int hour;
      int minute;
      int second;
      int ticks;
      Literal.GetTimeParts(datetimeLiteralValue, datetimeParts, 3, out hour, out minute, out second, out ticks);
      DateTime dateTime = new DateTime(year, month, day, hour, minute, second, 0);
      dateTime = dateTime.AddTicks((long) ticks);
      return dateTime;
    }

    private static DateTimeOffset ConvertDateTimeOffsetLiteralValue(
      ErrorContext errCtx,
      string datetimeLiteralValue)
    {
      string[] datetimeParts1 = datetimeLiteralValue.Split(Literal._datetimeOffsetSeparators, StringSplitOptions.RemoveEmptyEntries);
      int year;
      int month;
      int day;
      Literal.GetDateParts(datetimeLiteralValue, datetimeParts1, out year, out month, out day);
      string[] datetimeParts2 = new string[datetimeParts1.Length - 2];
      Array.Copy((Array) datetimeParts1, (Array) datetimeParts2, datetimeParts1.Length - 2);
      int hour;
      int minute;
      int second;
      int ticks;
      Literal.GetTimeParts(datetimeLiteralValue, datetimeParts2, 3, out hour, out minute, out second, out ticks);
      TimeSpan offset = new TimeSpan(int.Parse(datetimeParts1[datetimeParts1.Length - 2], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture), int.Parse(datetimeParts1[datetimeParts1.Length - 1], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture), 0);
      if (datetimeLiteralValue.IndexOf('+') == -1)
        offset = offset.Negate();
      DateTime dateTime = new DateTime(year, month, day, hour, minute, second, 0).AddTicks((long) ticks);
      try
      {
        return new DateTimeOffset(dateTime, offset);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        string errorMessage = Strings.InvalidDateTimeOffsetLiteral((object) datetimeLiteralValue);
        throw EntitySqlException.Create(errCtx, errorMessage, (Exception) ex);
      }
    }

    private static TimeSpan ConvertTimeLiteralValue(string datetimeLiteralValue)
    {
      string[] datetimeParts = datetimeLiteralValue.Split(Literal._datetimeSeparators, StringSplitOptions.RemoveEmptyEntries);
      int hour;
      int minute;
      int second;
      int ticks;
      Literal.GetTimeParts(datetimeLiteralValue, datetimeParts, 0, out hour, out minute, out second, out ticks);
      TimeSpan timeSpan = new TimeSpan(hour, minute, second);
      timeSpan = timeSpan.Add(new TimeSpan((long) ticks));
      return timeSpan;
    }

    private static void GetTimeParts(
      string datetimeLiteralValue,
      string[] datetimeParts,
      int timePartStartIndex,
      out int hour,
      out int minute,
      out int second,
      out int ticks)
    {
      hour = int.Parse(datetimeParts[timePartStartIndex], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
      if (hour > 23)
        throw new EntitySqlException(Strings.InvalidHour((object) datetimeParts[timePartStartIndex], (object) datetimeLiteralValue));
      minute = int.Parse(datetimeParts[++timePartStartIndex], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
      if (minute > 59)
        throw new EntitySqlException(Strings.InvalidMinute((object) datetimeParts[timePartStartIndex], (object) datetimeLiteralValue));
      second = 0;
      ticks = 0;
      ++timePartStartIndex;
      if (datetimeParts.Length <= timePartStartIndex)
        return;
      second = int.Parse(datetimeParts[timePartStartIndex], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
      if (second > 59)
        throw new EntitySqlException(Strings.InvalidSecond((object) datetimeParts[timePartStartIndex], (object) datetimeLiteralValue));
      ++timePartStartIndex;
      if (datetimeParts.Length <= timePartStartIndex)
        return;
      string s = datetimeParts[timePartStartIndex].PadRight(7, '0');
      ticks = int.Parse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    private static void GetDateParts(
      string datetimeLiteralValue,
      string[] datetimeParts,
      out int year,
      out int month,
      out int day)
    {
      year = int.Parse(datetimeParts[0], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
      if (year < 1 || year > 9999)
        throw new EntitySqlException(Strings.InvalidYear((object) datetimeParts[0], (object) datetimeLiteralValue));
      month = int.Parse(datetimeParts[1], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
      if (month < 1 || month > 12)
        throw new EntitySqlException(Strings.InvalidMonth((object) datetimeParts[1], (object) datetimeLiteralValue));
      day = int.Parse(datetimeParts[2], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
      if (day < 1)
        throw new EntitySqlException(Strings.InvalidDay((object) datetimeParts[2], (object) datetimeLiteralValue));
      if (day > DateTime.DaysInMonth(year, month))
        throw new EntitySqlException(Strings.InvalidDayInMonth((object) datetimeParts[2], (object) datetimeParts[1], (object) datetimeLiteralValue));
    }

    private static Guid ConvertGuidLiteralValue(string guidLiteralValue)
    {
      return new Guid(guidLiteralValue);
    }
  }
}
