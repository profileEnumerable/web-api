// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlFunctions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.SqlServer.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.SqlServer
{
  /// <summary>
  /// Contains function stubs that expose SqlServer methods in Linq to Entities.
  /// </summary>
  public static class SqlFunctions
  {
    /// <summary>Returns the checksum of the values in a collection. Null values are ignored.</summary>
    /// <returns>The checksum computed from the input collection.</returns>
    /// <param name="arg">The collection of values over which the checksum is computed.</param>
    [DbFunction("SqlServer", "CHECKSUM_AGG")]
    public static int? ChecksumAggregate(IEnumerable<int> arg)
    {
      return SqlFunctions.BootstrapFunction<int, int?>((Expression<Func<IEnumerable<int>, int?>>) (a => SqlFunctions.ChecksumAggregate(a)), arg);
    }

    /// <summary>Returns the checksum of the values in a collection. Null values are ignored.</summary>
    /// <returns>The checksum computed from the input collection.</returns>
    /// <param name="arg">The collection of values over which the checksum is computed.</param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [DbFunction("SqlServer", "CHECKSUM_AGG")]
    public static int? ChecksumAggregate(IEnumerable<int?> arg)
    {
      return SqlFunctions.BootstrapFunction<int?, int?>((Expression<Func<IEnumerable<int?>, int?>>) (a => SqlFunctions.ChecksumAggregate(a)), arg);
    }

    private static TOut BootstrapFunction<TIn, TOut>(
      Expression<Func<IEnumerable<TIn>, TOut>> methodExpression,
      IEnumerable<TIn> arg)
    {
      IQueryable queryable = arg as IQueryable;
      if (queryable != null)
        return queryable.Provider.Execute<TOut>((Expression) Expression.Call(((MethodCallExpression) methodExpression.Body).Method, (Expression) Expression.Constant((object) arg)));
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the ASCII code value of the left-most character of a character expression.</summary>
    /// <returns>The ASCII code of the first character in the input string.</returns>
    /// <param name="arg">A valid string.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "ASCII")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ascii")]
    public static int? Ascii(string arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the character that corresponds to the specified integer ASCII value.</summary>
    /// <returns>The character that corresponds to the specified ASCII value.</returns>
    /// <param name="arg">An ASCII code.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "CHAR")]
    public static string Char(int? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the starting position of one expression found within another expression.</summary>
    /// <returns>The starting position of  target  if it is found in  toSearch .</returns>
    /// <param name="toSearch">The string expression to be searched.</param>
    /// <param name="target">The string expression to be found.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "toSearch")]
    [DbFunction("SqlServer", "CHARINDEX")]
    public static int? CharIndex(string toSearch, string target)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the starting position of one expression found within another expression.</summary>
    /// <returns>The starting position of  target  if it is found in  toSearch .</returns>
    /// <param name="toSearch">The string expression to be searched.</param>
    /// <param name="target">The string expression to be found.</param>
    [DbFunction("SqlServer", "CHARINDEX")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "toSearch")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target")]
    public static int? CharIndex(byte[] toSearch, byte[] target)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the starting position of one expression found within another expression.</summary>
    /// <returns>The starting position of  target  if it is found in  toSearch .</returns>
    /// <param name="toSearch">The string expression to be searched.</param>
    /// <param name="target">The string expression to be found.</param>
    /// <param name="startLocation">The character position in  toSearch  where searching begins.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "toSearch")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startLocation")]
    [DbFunction("SqlServer", "CHARINDEX")]
    public static int? CharIndex(string toSearch, string target, int? startLocation)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the starting position of one expression found within another expression.</summary>
    /// <returns>The starting position of  target  if it is found in  toSearch .</returns>
    /// <param name="toSearch">The string expression to be searched.</param>
    /// <param name="target">The string expression to be found.</param>
    /// <param name="startLocation">The character position in  toSearch  where searching begins.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target")]
    [DbFunction("SqlServer", "CHARINDEX")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "toSearch")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startLocation")]
    public static int? CharIndex(byte[] toSearch, byte[] target, int? startLocation)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the starting position of one expression found within another expression.</summary>
    /// <returns>
    /// A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" /> value that is the starting position of  target  if it is found in  toSearch .
    /// </returns>
    /// <param name="toSearch">The string expression to be searched.</param>
    /// <param name="target">The string expression to be found.</param>
    /// <param name="startLocation">The character position in  toSearch  where searching begins.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startLocation")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "toSearch")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target")]
    [DbFunction("SqlServer", "CHARINDEX")]
    public static long? CharIndex(string toSearch, string target, long? startLocation)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the starting position of one expression found within another expression.</summary>
    /// <returns>The starting position of  target  if it is found in  toSearch .</returns>
    /// <param name="toSearch">The string expression to be searched.</param>
    /// <param name="target">The string expression to be found.</param>
    /// <param name="startLocation">The character position in  toSearch  at which searching begins.</param>
    [DbFunction("SqlServer", "CHARINDEX")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startLocation")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "toSearch")]
    public static long? CharIndex(byte[] toSearch, byte[] target, long? startLocation)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns an integer value that indicates the difference between the SOUNDEX values of two character expressions.</summary>
    /// <returns>The SOUNDEX difference between the two strings.</returns>
    /// <param name="string1">The first string.</param>
    /// <param name="string2">The second string.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "string2")]
    [DbFunction("SqlServer", "DIFFERENCE")]
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "string1")]
    public static int? Difference(string string1, string string2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the Unicode character with the specified integer code, as defined by the Unicode standard.</summary>
    /// <returns>The character that corresponds to the input character code.</returns>
    /// <param name="arg">A character code.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "NCHAR")]
    public static string NChar(int? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the starting position of the first occurrence of a pattern in a specified expression, or zeros if the pattern is not found, on all valid text and character data types.</summary>
    /// <returns>The starting character position where the string pattern was found.</returns>
    /// <param name="stringPattern">A string pattern to search for.</param>
    /// <param name="target">The string to search.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stringPattern")]
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target")]
    [DbFunction("SqlServer", "PATINDEX")]
    public static int? PatIndex(string stringPattern, string target)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a Unicode string with the delimiters added to make the input string a valid Microsoft SQL Server delimited identifier.</summary>
    /// <returns>The original string with brackets added.</returns>
    /// <param name="stringArg">The expression that quote characters will be added to.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stringArg")]
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
    [DbFunction("SqlServer", "QUOTENAME")]
    public static string QuoteName(string stringArg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a Unicode string with the delimiters added to make the input string a valid Microsoft SQL Server delimited identifier.</summary>
    /// <returns>The original string with the specified quote characters added.</returns>
    /// <param name="stringArg">The expression that quote characters will be added to.</param>
    /// <param name="quoteCharacter">The one-character string to use as the delimiter. It can be a single quotation mark ( ' ), a left or right bracket ( [ ] ), or a double quotation mark ( " ). If quote_character is not specified, brackets are used.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "quoteCharacter")]
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stringArg")]
    [DbFunction("SqlServer", "QUOTENAME")]
    public static string QuoteName(string stringArg, string quoteCharacter)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Repeats a string value a specified number of times.</summary>
    /// <returns>The target string, repeated the number of times specified by  count .</returns>
    /// <param name="target">A valid string.</param>
    /// <param name="count">The value that specifies how many time to repeat  target .</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "count")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target")]
    [DbFunction("SqlServer", "REPLICATE")]
    public static string Replicate(string target, int? count)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Converts an alphanumeric string to a four-character (SOUNDEX) code to find similar-sounding words or names.</summary>
    /// <returns>The SOUNDEX code of the input string.</returns>
    /// <param name="arg">A valid string.</param>
    [DbFunction("SqlServer", "SOUNDEX")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static string SoundCode(string arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a string of repeated spaces.</summary>
    /// <returns>A string that consists of the specified number of spaces.</returns>
    /// <param name="arg1">The number of spaces. If negative, a null string is returned.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "SPACE")]
    public static string Space(int? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns character data converted from numeric data.</summary>
    /// <returns>The numeric input expression converted to a string.</returns>
    /// <param name="number">A numeric expression.</param>
    [DbFunction("SqlServer", "STR")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    public static string StringConvert(double? number)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns character data converted from numeric data.</summary>
    /// <returns>The input expression converted to a string.</returns>
    /// <param name="number">A numeric expression.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    [DbFunction("SqlServer", "STR")]
    public static string StringConvert(Decimal? number)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns character data converted from numeric data.</summary>
    /// <returns>The numeric input expression converted to a string.</returns>
    /// <param name="number">A numeric expression.</param>
    /// <param name="length">The total length of the string. This includes decimal point, sign, digits, and spaces. The default is 10.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "length")]
    [DbFunction("SqlServer", "STR")]
    public static string StringConvert(double? number, int? length)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns character data converted from numeric data.</summary>
    /// <returns>The input expression converted to a string.</returns>
    /// <param name="number">A numeric expression.</param>
    /// <param name="length">The total length of the string. This includes decimal point, sign, digits, and spaces. The default is 10.</param>
    [DbFunction("SqlServer", "STR")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "length")]
    public static string StringConvert(Decimal? number, int? length)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns character data converted from numeric data.</summary>
    /// <returns>The numeric input expression converted to a string.</returns>
    /// <param name="number">A numeric expression.</param>
    /// <param name="length">The total length of the string. This includes decimal point, sign, digits, and spaces. The default is 10.</param>
    /// <param name="decimalArg">The number of places to the right of the decimal point.  decimal  must be less than or equal to 16. If  decimal  is more than 16 then the result is truncated to sixteen places to the right of the decimal point.</param>
    [DbFunction("SqlServer", "STR")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "length")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "decimalArg")]
    public static string StringConvert(double? number, int? length, int? decimalArg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns character data converted from numeric data.</summary>
    /// <returns>The input expression converted to a string.</returns>
    /// <param name="number">A numeric expression.</param>
    /// <param name="length">The total length of the string. This includes decimal point, sign, digits, and spaces. The default is 10.</param>
    /// <param name="decimalArg">The number of places to the right of the decimal point.  decimal  must be less than or equal to 16. If  decimal  is more than 16 then the result is truncated to sixteen places to the right of the decimal point.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    [DbFunction("SqlServer", "STR")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "decimalArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "length")]
    public static string StringConvert(Decimal? number, int? length, int? decimalArg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Inserts a string into another string. It deletes a specified length of characters in the target string at the start position and then inserts the second string into the target string at the start position.</summary>
    /// <returns>A string consisting of the two strings.</returns>
    /// <param name="stringInput">The target string.</param>
    /// <param name="start">The character position in  stringinput  where the replacement string is to be inserted.</param>
    /// <param name="length">The number of characters to delete from  stringInput . If  length  is longer than  stringInput , deletion occurs up to the last character in  stringReplacement .</param>
    /// <param name="stringReplacement">The substring to be inserted into  stringInput .</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "start")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stringInput")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stringReplacement")]
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "length")]
    [DbFunction("SqlServer", "STUFF")]
    public static string Stuff(
      string stringInput,
      int? start,
      int? length,
      string stringReplacement)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the integer value, as defined by the Unicode standard, for the first character of the input expression.</summary>
    /// <returns>The character code for the first character in the input string.</returns>
    /// <param name="arg">A valid string.</param>
    [DbFunction("SqlServer", "UNICODE")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static int? Unicode(string arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>A mathematical function that returns the angle, in radians, whose cosine is the specified numerical value. This angle is called the arccosine.</summary>
    /// <returns>The angle, in radians, defined by the input cosine value.</returns>
    /// <param name="arg1">The cosine of an angle.</param>
    [DbFunction("SqlServer", "ACOS")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static double? Acos(double? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>A mathematical function that returns the angle, in radians, whose cosine is the specified numerical value. This angle is called the arccosine.</summary>
    /// <returns>An angle, measured in radians.</returns>
    /// <param name="arg1">The cosine of an angle.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "ACOS")]
    public static double? Acos(Decimal? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>A mathematical function that returns the angle, in radians, whose sine is the specified numerical value. This angle is called the arcsine.</summary>
    /// <returns>An angle, measured in radians.</returns>
    /// <param name="arg">The sine of an angle.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "ASIN")]
    public static double? Asin(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>A mathematical function that returns the angle, in radians, whose sine is the specified numerical value. This angle is called the arcsine.</summary>
    /// <returns>An angle, measured in radians.</returns>
    /// <param name="arg">The sine of an angle.</param>
    [DbFunction("SqlServer", "ASIN")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? Asin(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>A mathematical function that returns the angle, in radians, whose tangent is the specified numerical value. This angle is called the arctangent.</summary>
    /// <returns>An angle, measured in radians.</returns>
    /// <param name="arg">The tangent of an angle.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "ATAN")]
    public static double? Atan(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>A mathematical function that returns the angle, in radians, whose tangent is the specified numerical value. This angle is called the arctangent.</summary>
    /// <returns>An angle, measured in radians.</returns>
    /// <param name="arg">The tangent of an angle.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "ATAN")]
    public static double? Atan(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the positive angle, in radians, between the positive x-axis and the ray from the origin through the point (x, y), where x and y are the two specified numerical values. The first parameter passed to the function is the y-value and the second parameter is the x-value.</summary>
    /// <returns>An angle, measured in radians.</returns>
    /// <param name="arg1">The y-coordinate of a point.</param>
    /// <param name="arg2">The x-coordinate of a point.</param>
    [DbFunction("SqlServer", "ATN2")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    public static double? Atan2(double? arg1, double? arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the positive angle, in radians, between the positive x-axis and the ray from the origin through the point (x, y), where x and y are the two specified numerical values. The first parameter passed to the function is the y-value and the second parameter is the x-value.</summary>
    /// <returns>An angle, measured in radians.</returns>
    /// <param name="arg1">The y-coordinate of a point.</param>
    /// <param name="arg2">The x-coordinate of a point.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "ATN2")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static double? Atan2(Decimal? arg1, Decimal? arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the trigonometric cosine of the specified angle, in radians, in the specified expression.</summary>
    /// <returns>The trigonometric cosine of the specified angle.</returns>
    /// <param name="arg">An angle, measured in radians.</param>
    [DbFunction("SqlServer", "COS")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? Cos(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the trigonometric cosine of the specified angle, in radians, in the specified expression.</summary>
    /// <returns>The trigonometric cosine of the specified angle.</returns>
    /// <param name="arg">An angle, measured in radians.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "COS")]
    public static double? Cos(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>A mathematical function that returns the trigonometric cotangent of the specified angle, in radians.</summary>
    /// <returns>The trigonometric cotangent of the specified angle.</returns>
    /// <param name="arg">An angle, measured in radians.</param>
    [DbFunction("SqlServer", "COT")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? Cot(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>A mathematical function that returns the trigonometric cotangent of the specified angle, in radians.</summary>
    /// <returns>The trigonometric cotangent of the specified angle.</returns>
    /// <param name="arg">An angle, measured in radians.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "COT")]
    public static double? Cot(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the corresponding angle in degrees for an angle specified in radians.</summary>
    /// <returns>The specified angle converted to degrees.</returns>
    /// <param name="arg1">An angle, measured in radians.</param>
    [DbFunction("SqlServer", "DEGREES")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Degrees(int? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the corresponding angle in degrees for an angle specified in radians.</summary>
    /// <returns>The specified angle converted to degrees.</returns>
    /// <param name="arg1">An angle, measured in radians.</param>
    [DbFunction("SqlServer", "DEGREES")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static long? Degrees(long? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the corresponding angle in degrees for an angle specified in radians.</summary>
    /// <returns>The specified angle converted to degrees.</returns>
    /// <param name="arg1">An angle, measured in radians.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "DEGREES")]
    public static Decimal? Degrees(Decimal? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the corresponding angle in degrees for an angle specified in radians.</summary>
    /// <returns>The specified angle converted to degrees.</returns>
    /// <param name="arg1">An angle, measured in radians.</param>
    [DbFunction("SqlServer", "DEGREES")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static double? Degrees(double? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the exponential value of the specified float expression.</summary>
    /// <returns>The constant e raised to the power of the input value.</returns>
    /// <param name="arg">The input value.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Exp")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "EXP")]
    public static double? Exp(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the exponential value of the specified float expression.</summary>
    /// <returns>The constant e raised to the power of the input value.</returns>
    /// <param name="arg">The input value.</param>
    [DbFunction("SqlServer", "EXP")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Exp")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? Exp(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the natural logarithm of the specified input value.</summary>
    /// <returns>The natural logarithm of the input value.</returns>
    /// <param name="arg">A numeric expression.</param>
    [DbFunction("SqlServer", "LOG")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? Log(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the natural logarithm of the specified input value.</summary>
    /// <returns>The natural logarithm of the input value.</returns>
    /// <param name="arg">A numeric expression.</param>
    [DbFunction("SqlServer", "LOG")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? Log(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the base-10 logarithm of the specified input value.</summary>
    /// <returns>The base-10 logarithm of the input value.</returns>
    /// <param name="arg">A numeric expression.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "LOG10")]
    public static double? Log10(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the base-10 logarithm of the specified input value.</summary>
    /// <returns>The base-10 logarithm of the input value.</returns>
    /// <param name="arg">A numeric expression.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "LOG10")]
    public static double? Log10(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the constant value of pi.</summary>
    /// <returns>The numeric value of pi.</returns>
    [DbFunction("SqlServer", "PI")]
    public static double? Pi()
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the radian measure corresponding to the specified angle in degrees.</summary>
    /// <returns>The radian measure of the specified angle.</returns>
    /// <param name="arg">The angle, measured in degrees</param>
    [DbFunction("SqlServer", "RADIANS")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static int? Radians(int? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the radian measure corresponding to the specified angle in degrees.</summary>
    /// <returns>The radian measure of the specified angle.</returns>
    /// <param name="arg">The angle, measured in degrees</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "RADIANS")]
    public static long? Radians(long? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the radian measure corresponding to the specified angle in degrees.</summary>
    /// <returns>The radian measure of the specified angle.</returns>
    /// <param name="arg">The angle, measured in degrees.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "RADIANS")]
    public static Decimal? Radians(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the radian measure corresponding to the specified angle in degrees.</summary>
    /// <returns>The radian measure of the specified angle.</returns>
    /// <param name="arg">The angle, measured in degrees.</param>
    [DbFunction("SqlServer", "RADIANS")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? Radians(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a pseudo-random float value from 0 through 1, exclusive.</summary>
    /// <returns>The pseudo-random value.</returns>
    [DbFunction("SqlServer", "RAND")]
    public static double? Rand()
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a pseudo-random float value from 0 through 1, exclusive.</summary>
    /// <returns>The pseudo-random value.</returns>
    /// <param name="seed">The seed value. If  seed  is not specified, the SQL Server Database Engine assigns a seed value at random. For a specified seed value, the result returned is always the same.</param>
    [DbFunction("SqlServer", "RAND")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "seed")]
    public static double? Rand(int? seed)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the positive (+1), zero (0), or negative (-1) sign of the specified expression.</summary>
    /// <returns>The sign of the input expression.</returns>
    /// <param name="arg">A numeric expression.</param>
    [DbFunction("SqlServer", "SIGN")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static int? Sign(int? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the positive (+1), zero (0), or negative (-1) sign of the specified expression.</summary>
    /// <returns>The sign of the input expression.</returns>
    /// <param name="arg">A numeric expression.</param>
    [DbFunction("SqlServer", "SIGN")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static long? Sign(long? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the positive (+1), zero (0), or negative (-1) sign of the specified expression.</summary>
    /// <returns>The sign of the input expression.</returns>
    /// <param name="arg">A numeric expression.</param>
    [DbFunction("SqlServer", "SIGN")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static Decimal? Sign(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the positive (+1), zero (0), or negative (-1) sign of the specified expression.</summary>
    /// <returns>The sign of the input expression.</returns>
    /// <param name="arg">A numeric expression.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "SIGN")]
    public static double? Sign(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the trigonometric sine of the specified angle.</summary>
    /// <returns>The trigonometric sine of the input expression.</returns>
    /// <param name="arg">An angle, measured in radians.</param>
    [DbFunction("SqlServer", "SIN")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? Sin(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the trigonometric sine of the specified angle.</summary>
    /// <returns>The trigonometric sine of the input expression.</returns>
    /// <param name="arg">An angle, measured in radians.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "SIN")]
    public static double? Sin(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the square root of the specified number.</summary>
    /// <returns>The square root of the input value.</returns>
    /// <param name="arg">A numeric expression.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "SQRT")]
    public static double? SquareRoot(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the square root of the specified number.</summary>
    /// <returns>The square root of the input value.</returns>
    /// <param name="arg">A numeric expression.</param>
    [DbFunction("SqlServer", "SQRT")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? SquareRoot(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the square of the specified number.</summary>
    /// <returns>The square of the input value.</returns>
    /// <param name="arg1">A numeric expression.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "SQUARE")]
    public static double? Square(double? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the square of the specified number.</summary>
    /// <returns>The square of the input value.</returns>
    /// <param name="arg1">A numeric expression.</param>
    [DbFunction("SqlServer", "SQUARE")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static double? Square(Decimal? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the trigonometric tangent of the input expression.</summary>
    /// <returns>The tangent of the input angle.</returns>
    /// <param name="arg">An angle, measured in radians.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "TAN")]
    public static double? Tan(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the trigonometric tangent of the input expression.</summary>
    /// <returns>The tangent of the input angle.</returns>
    /// <param name="arg">An angle, measured in radians.</param>
    [DbFunction("SqlServer", "TAN")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static double? Tan(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a new datetime value based on adding an interval to the specified date.</summary>
    /// <returns>The new date.</returns>
    /// <param name="datePartArg">The part of the date to increment. </param>
    /// <param name="number">The value used to increment a date by a specified amount.</param>
    /// <param name="date">The date to increment.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    [DbFunction("SqlServer", "DATEADD")]
    public static DateTime? DateAdd(string datePartArg, double? number, DateTime? date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a new time span value based on adding an interval to the specified time span.</summary>
    /// <returns>The new time span.</returns>
    /// <param name="datePartArg">The part of the date to increment.</param>
    /// <param name="number">The value used to increment a date by a specified amount.</param>
    /// <param name="time">The time span to increment.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "time")]
    [DbFunction("SqlServer", "DATEADD")]
    public static TimeSpan? DateAdd(string datePartArg, double? number, TimeSpan? time)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a new date value based on adding an interval to the specified date.</summary>
    /// <returns>The new point in time, expressed as a date and time of day, relative to Coordinated Universal Time (UTC).</returns>
    /// <param name="datePartArg">The part of the date to increment.</param>
    /// <param name="number">The value used to increment a date by a specified amount.</param>
    /// <param name="dateTimeOffsetArg">The date to increment.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dateTimeOffsetArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [DbFunction("SqlServer", "DATEADD")]
    public static DateTimeOffset? DateAdd(
      string datePartArg,
      double? number,
      DateTimeOffset? dateTimeOffsetArg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a new datetime value based on adding an interval to the specified date.</summary>
    /// <returns>
    /// A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" /> value that is the new date.
    /// </returns>
    /// <param name="datePartArg">The part of the date to increment.</param>
    /// <param name="number">The value used to increment a date by a specified amount.</param>
    /// <param name="date">The date to increment.</param>
    [DbFunction("SqlServer", "DATEADD")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "number")]
    public static DateTime? DateAdd(string datePartArg, double? number, string date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [DbFunction("SqlServer", "DATEDIFF")]
    public static int? DateDiff(string datePartArg, DateTime? startDate, DateTime? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [DbFunction("SqlServer", "DATEDIFF")]
    public static int? DateDiff(
      string datePartArg,
      DateTimeOffset? startDate,
      DateTimeOffset? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    public static int? DateDiff(string datePartArg, TimeSpan? startDate, TimeSpan? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    public static int? DateDiff(string datePartArg, string startDate, DateTime? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    public static int? DateDiff(string datePartArg, string startDate, DateTimeOffset? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The value specifying the number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    public static int? DateDiff(string datePartArg, string startDate, TimeSpan? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    public static int? DateDiff(string datePartArg, TimeSpan? startDate, string endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [DbFunction("SqlServer", "DATEDIFF")]
    public static int? DateDiff(string datePartArg, DateTime? startDate, string endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    public static int? DateDiff(string datePartArg, DateTimeOffset? startDate, string endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    public static int? DateDiff(string datePartArg, string startDate, string endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    public static int? DateDiff(string datePartArg, TimeSpan? startDate, DateTime? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    public static int? DateDiff(string datePartArg, TimeSpan? startDate, DateTimeOffset? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    public static int? DateDiff(string datePartArg, DateTime? startDate, TimeSpan? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two Dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    public static int? DateDiff(string datePartArg, DateTimeOffset? startDate, TimeSpan? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [DbFunction("SqlServer", "DATEDIFF")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    public static int? DateDiff(string datePartArg, DateTime? startDate, DateTimeOffset? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the count of the specified datepart boundaries crossed between the specified start date and end date.</summary>
    /// <returns>The number of time intervals between the two dates.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="startDate">The first date.</param>
    /// <param name="endDate">The second date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "endDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "startDate")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [DbFunction("SqlServer", "DATEDIFF")]
    public static int? DateDiff(string datePartArg, DateTimeOffset? startDate, DateTime? endDate)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a character string that represents the specified datepart of the specified date.</summary>
    /// <returns>The specified part of the specified date.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="date">The date.</param>
    [DbFunction("SqlServer", "DATENAME")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    public static string DateName(string datePartArg, DateTime? date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a character string that represents the specified datepart of the specified date.</summary>
    /// <returns>The specified part of the specified date.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="date">The date.</param>
    [DbFunction("SqlServer", "DATENAME")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    public static string DateName(string datePartArg, string date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a character string that represents the specified datepart of the specified date.</summary>
    /// <returns>The specified part of the specified date.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="date">The date.</param>
    [DbFunction("SqlServer", "DATENAME")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    public static string DateName(string datePartArg, TimeSpan? date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a character string that represents the specified datepart of the specified date.</summary>
    /// <returns>The specified part of the specified date.</returns>
    /// <param name="datePartArg">The part of the date to calculate the differing number of time intervals.</param>
    /// <param name="date">The date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [DbFunction("SqlServer", "DATENAME")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    public static string DateName(string datePartArg, DateTimeOffset? date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns an integer that represents the specified datepart of the specified date.</summary>
    /// <returns>The the specified datepart of the specified date.</returns>
    /// <param name="datePartArg">The part of the date to return the value.</param>
    /// <param name="date">The date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [DbFunction("SqlServer", "DATEPART")]
    public static int? DatePart(string datePartArg, DateTime? date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns an integer that represents the specified datepart of the specified date.</summary>
    /// <returns>The specified datepart of the specified date.</returns>
    /// <param name="datePartArg">The part of the date to return the value.</param>
    /// <param name="date">The date.</param>
    [DbFunction("SqlServer", "DATEPART")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    public static int? DatePart(string datePartArg, DateTimeOffset? date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns an integer that represents the specified datepart of the specified date.</summary>
    /// <returns>The specified datepart of the specified date.</returns>
    /// <param name="datePartArg">The part of the date to return the value.</param>
    /// <param name="date">The date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    [DbFunction("SqlServer", "DATEPART")]
    public static int? DatePart(string datePartArg, string date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns an integer that represents the specified datepart of the specified date.</summary>
    /// <returns>The specified datepart of the specified date.</returns>
    /// <param name="datePartArg">The part of the date to return the value.</param>
    /// <param name="date">The date.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "date")]
    [DbFunction("SqlServer", "DATEPART")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "datePartArg")]
    public static int? DatePart(string datePartArg, TimeSpan? date)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the current database system timestamp as a datetime value without the database time zone offset. This value is derived from the operating system of the computer on which the instance of SQL Server is running.</summary>
    /// <returns>The current database timestamp.</returns>
    [DbFunction("SqlServer", "GETDATE")]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static DateTime? GetDate()
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the current database system timestamp as a datetime value. The database time zone offset is not included. This value represents the current UTC time (Coordinated Universal Time). This value is derived from the operating system of the computer on which the instance of SQL Server is running.</summary>
    /// <returns>The current database UTC timestamp.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [DbFunction("SqlServer", "GETUTCDATE")]
    public static DateTime? GetUtcDate()
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the number of bytes used to represent any expression.</summary>
    /// <returns>The number of bytes in the input value.</returns>
    /// <param name="arg">The value to be examined for data length.</param>
    [DbFunction("SqlServer", "DATALENGTH")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static int? DataLength(bool? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the number of bytes used to represent any expression.</summary>
    /// <returns>The number of bytes in the input value.</returns>
    /// <param name="arg">The value to be examined for data length.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "DATALENGTH")]
    public static int? DataLength(double? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the number of bytes used to represent any expression.</summary>
    /// <returns>The number of bytes in the input value.</returns>
    /// <param name="arg">The value to be examined for data length.</param>
    [DbFunction("SqlServer", "DATALENGTH")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static int? DataLength(Decimal? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the number of bytes used to represent any expression.</summary>
    /// <returns>The number of bytes in the input value.</returns>
    /// <param name="arg">The value to be examined for data length.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "DATALENGTH")]
    public static int? DataLength(DateTime? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the number of bytes used to represent any expression.</summary>
    /// <returns>The number of bytes in the input value.</returns>
    /// <param name="arg">The value to be examined for data length.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "DATALENGTH")]
    public static int? DataLength(TimeSpan? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the number of bytes used to represent any expression.</summary>
    /// <returns>The number of bytes in the input value.</returns>
    /// <param name="arg">The value to be examined for data length.</param>
    [DbFunction("SqlServer", "DATALENGTH")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static int? DataLength(DateTimeOffset? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the number of bytes used to represent any expression.</summary>
    /// <returns>The number of bytes in the input value.</returns>
    /// <param name="arg">The value to be examined for data length.</param>
    [DbFunction("SqlServer", "DATALENGTH")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static int? DataLength(string arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the number of bytes used to represent any expression.</summary>
    /// <returns>The number of bytes in the input value.</returns>
    /// <param name="arg">The value to be examined for length.</param>
    [DbFunction("SqlServer", "DATALENGTH")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static int? DataLength(byte[] arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the number of bytes used to represent any expression.</summary>
    /// <returns>The number of bytes in the input value.</returns>
    /// <param name="arg">The value to be examined for data length.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "DATALENGTH")]
    public static int? DataLength(Guid? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input argument.</summary>
    /// <returns>The checksum computed over the input value.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(bool? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input argument.</summary>
    /// <returns>The checksum computed over the input value.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(double? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input argument.</summary>
    /// <returns>The checksum computed over the input value.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(Decimal? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input argument.</summary>
    /// <returns>The checksum computed over the input value.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(string arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input argument.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(DateTime? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input argument.</summary>
    /// <returns>The checksum computed over the input value.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(TimeSpan? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input argument.</summary>
    /// <returns>The checksum computed over the input value.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(DateTimeOffset? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input argument.</summary>
    /// <returns>The checksum computed over the input value.</returns>
    /// <param name="arg1">The character array for which the checksum is calculated.</param>
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(byte[] arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input argument.</summary>
    /// <returns>The checksum computed over the input value.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(Guid? arg1)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(bool? arg1, bool? arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(double? arg1, double? arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(Decimal? arg1, Decimal? arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(string arg1, string arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(DateTime? arg1, DateTime? arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(TimeSpan? arg1, TimeSpan? arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(DateTimeOffset? arg1, DateTimeOffset? arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The character array for which the checksum is calculated.</param>
    /// <param name="arg2">The character array for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(byte[] arg1, byte[] arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(Guid? arg1, Guid? arg2)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    /// <param name="arg3">The value for which the checksum is calculated.</param>
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    public static int? Checksum(bool? arg1, bool? arg2, bool? arg3)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    /// <param name="arg3">The value for which the checksum is calculated.</param>
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3")]
    public static int? Checksum(double? arg1, double? arg2, double? arg3)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    /// <param name="arg3">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(Decimal? arg1, Decimal? arg2, Decimal? arg3)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    /// <param name="arg3">The value for which the checksum is calculated.</param>
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    public static int? Checksum(string arg1, string arg2, string arg3)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    /// <param name="arg3">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3")]
    public static int? Checksum(DateTime? arg1, DateTime? arg2, DateTime? arg3)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    /// <param name="arg3">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [DbFunction("SqlServer", "CHECKSUM")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    public static int? Checksum(DateTimeOffset? arg1, DateTimeOffset? arg2, DateTimeOffset? arg3)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    /// <param name="arg3">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(TimeSpan? arg1, TimeSpan? arg2, TimeSpan? arg3)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The character array for which the checksum is calculated.</param>
    /// <param name="arg2">The character array for which the checksum is calculated.</param>
    /// <param name="arg3">The character array for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(byte[] arg1, byte[] arg2, byte[] arg3)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the checksum value computed over the input arguments.</summary>
    /// <returns>The checksum computed over the input values.</returns>
    /// <param name="arg1">The value for which the checksum is calculated.</param>
    /// <param name="arg2">The value for which the checksum is calculated.</param>
    /// <param name="arg3">The value for which the checksum is calculated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg1")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg2")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg3")]
    [DbFunction("SqlServer", "CHECKSUM")]
    public static int? Checksum(Guid? arg1, Guid? arg2, Guid? arg3)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the current date and time. </summary>
    /// <returns>The current date and time.</returns>
    [DbFunction("SqlServer", "CURRENT_TIMESTAMP")]
    public static DateTime? CurrentTimestamp()
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the name of the current user.</summary>
    /// <returns>The name of the current user.</returns>
    [DbFunction("SqlServer", "CURRENT_USER")]
    public static string CurrentUser()
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns the workstation name.</summary>
    /// <returns>The name of the workstation.</returns>
    [DbFunction("SqlServer", "HOST_NAME")]
    public static string HostName()
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a database user name corresponding to a specified identification number.</summary>
    /// <returns>The user name.</returns>
    /// <param name="arg">A user ID.</param>
    [DbFunction("SqlServer", "USER_NAME")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static string UserName(int? arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Returns a database user name corresponding to a specified identification number.</summary>
    /// <returns>The user name.</returns>
    [DbFunction("SqlServer", "USER_NAME")]
    public static string UserName()
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Indicates whether the input value is a valid numeric type.</summary>
    /// <returns>1 if the input expression is a valid numeric data type; otherwise, 0.</returns>
    /// <param name="arg">A string expression.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    [DbFunction("SqlServer", "ISNUMERIC")]
    public static int? IsNumeric(string arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }

    /// <summary>Indicates whether the input value is a valid date or time.</summary>
    /// <returns>1 if the input expression is a valid date or time value of datetime or smalldatetime data types; otherwise, 0.</returns>
    /// <param name="arg">The tested value.</param>
    [DbFunction("SqlServer", "ISDATE")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arg")]
    public static int? IsDate(string arg)
    {
      throw new NotSupportedException(Strings.ELinq_DbFunctionDirectCall);
    }
  }
}
