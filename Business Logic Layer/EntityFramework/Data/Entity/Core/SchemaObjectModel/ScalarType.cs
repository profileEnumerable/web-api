// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.ScalarType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class ScalarType : SchemaType
  {
    private static readonly Regex _binaryValueValidator = new Regex("^0[xX][0-9a-fA-F]+$", RegexOptions.Compiled);
    private static readonly Regex _guidValueValidator = new Regex("[0-9a-fA-F]{8,8}(-[0-9a-fA-F]{4,4}){3,3}-[0-9a-fA-F]{12,12}", RegexOptions.Compiled);
    internal const string DateTimeFormat = "yyyy-MM-dd HH\\:mm\\:ss.fffZ";
    internal const string TimeFormat = "HH\\:mm\\:ss.fffffffZ";
    internal const string DateTimeOffsetFormat = "yyyy-MM-dd HH\\:mm\\:ss.fffffffz";
    private readonly PrimitiveType _primitiveType;

    internal ScalarType(Schema parentElement, string typeName, PrimitiveType primitiveType)
      : base(parentElement)
    {
      this.Name = typeName;
      this._primitiveType = primitiveType;
    }

    public bool TryParse(string text, out object value)
    {
      switch (this._primitiveType.PrimitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          return ScalarType.TryParseBinary(text, out value);
        case PrimitiveTypeKind.Boolean:
          return ScalarType.TryParseBoolean(text, out value);
        case PrimitiveTypeKind.Byte:
          return ScalarType.TryParseByte(text, out value);
        case PrimitiveTypeKind.DateTime:
          return ScalarType.TryParseDateTime(text, out value);
        case PrimitiveTypeKind.Decimal:
          return ScalarType.TryParseDecimal(text, out value);
        case PrimitiveTypeKind.Double:
          return ScalarType.TryParseDouble(text, out value);
        case PrimitiveTypeKind.Guid:
          return ScalarType.TryParseGuid(text, out value);
        case PrimitiveTypeKind.Single:
          return ScalarType.TryParseSingle(text, out value);
        case PrimitiveTypeKind.SByte:
          return ScalarType.TryParseSByte(text, out value);
        case PrimitiveTypeKind.Int16:
          return ScalarType.TryParseInt16(text, out value);
        case PrimitiveTypeKind.Int32:
          return ScalarType.TryParseInt32(text, out value);
        case PrimitiveTypeKind.Int64:
          return ScalarType.TryParseInt64(text, out value);
        case PrimitiveTypeKind.String:
          return ScalarType.TryParseString(text, out value);
        case PrimitiveTypeKind.Time:
          return ScalarType.TryParseTime(text, out value);
        case PrimitiveTypeKind.DateTimeOffset:
          return ScalarType.TryParseDateTimeOffset(text, out value);
        default:
          throw new NotSupportedException(this._primitiveType.FullName);
      }
    }

    public PrimitiveTypeKind TypeKind
    {
      get
      {
        return this._primitiveType.PrimitiveTypeKind;
      }
    }

    public PrimitiveType Type
    {
      get
      {
        return this._primitiveType;
      }
    }

    private static bool TryParseBoolean(string text, out object value)
    {
      bool result;
      if (!bool.TryParse(text, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseByte(string text, out object value)
    {
      byte result;
      if (!byte.TryParse(text, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseSByte(string text, out object value)
    {
      sbyte result;
      if (!sbyte.TryParse(text, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseInt16(string text, out object value)
    {
      short result;
      if (!short.TryParse(text, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseInt32(string text, out object value)
    {
      int result;
      if (!int.TryParse(text, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseInt64(string text, out object value)
    {
      long result;
      if (!long.TryParse(text, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseDouble(string text, out object value)
    {
      double result;
      if (!double.TryParse(text, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseDecimal(string text, out object value)
    {
      Decimal result;
      if (!Decimal.TryParse(text, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseDateTime(string text, out object value)
    {
      DateTime result;
      if (!DateTime.TryParseExact(text, "yyyy-MM-dd HH\\:mm\\:ss.fffZ", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseTime(string text, out object value)
    {
      DateTime result;
      if (!DateTime.TryParseExact(text, "HH\\:mm\\:ss.fffffffZ", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) new TimeSpan(result.Ticks);
      return true;
    }

    private static bool TryParseDateTimeOffset(string text, out object value)
    {
      DateTimeOffset result;
      if (!DateTimeOffset.TryParse(text, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }

    private static bool TryParseGuid(string text, out object value)
    {
      if (!ScalarType._guidValueValidator.IsMatch(text))
      {
        value = (object) null;
        return false;
      }
      value = (object) new Guid(text);
      return true;
    }

    private static bool TryParseString(string text, out object value)
    {
      value = (object) text;
      return true;
    }

    private static bool TryParseBinary(string text, out object value)
    {
      if (!ScalarType._binaryValueValidator.IsMatch(text))
      {
        value = (object) null;
        return false;
      }
      string text1 = text.Substring(2);
      value = (object) ScalarType.ConvertToByteArray(text1);
      return true;
    }

    internal static byte[] ConvertToByteArray(string text)
    {
      int length1 = 2;
      int length2 = text.Length / 2;
      if (text.Length % 2 == 1)
      {
        length1 = 1;
        ++length2;
      }
      byte[] numArray = new byte[length2];
      int startIndex = 0;
      int index = 0;
      while (startIndex < text.Length)
      {
        numArray[index] = byte.Parse(text.Substring(startIndex, length1), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
        startIndex += length1;
        length1 = 2;
        ++index;
      }
      return numArray;
    }

    private static bool TryParseSingle(string text, out object value)
    {
      float result;
      if (!float.TryParse(text, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        value = (object) null;
        return false;
      }
      value = (object) result;
      return true;
    }
  }
}
