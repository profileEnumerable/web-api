// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder.EdmFunctions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
{
  /// <summary>
  /// Provides an API to construct <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />s that invoke canonical EDM functions, and allows that API to be accessed as extension methods on the expression type itself.
  /// </summary>
  public static class EdmFunctions
  {
    private static EdmFunction ResolveCanonicalFunction(
      string functionName,
      TypeUsage[] argumentTypes)
    {
      List<EdmFunction> edmFunctionList = new List<EdmFunction>(EdmProviderManifest.Instance.GetStoreFunctions().Where<EdmFunction>((Func<EdmFunction, bool>) (func => string.Equals(func.Name, functionName, StringComparison.Ordinal))));
      EdmFunction edmFunction = (EdmFunction) null;
      bool isAmbiguous = false;
      if (edmFunctionList.Count > 0)
      {
        edmFunction = FunctionOverloadResolver.ResolveFunctionOverloads((IList<EdmFunction>) edmFunctionList, (IList<TypeUsage>) argumentTypes, false, out isAmbiguous);
        if (isAmbiguous)
          throw new ArgumentException(Strings.Cqt_Function_CanonicalFunction_AmbiguousMatch((object) functionName));
      }
      if (edmFunction == null)
        throw new ArgumentException(Strings.Cqt_Function_CanonicalFunction_NotFound((object) functionName));
      return edmFunction;
    }

    internal static DbFunctionExpression InvokeCanonicalFunction(
      string functionName,
      params DbExpression[] arguments)
    {
      TypeUsage[] argumentTypes = new TypeUsage[arguments.Length];
      for (int index = 0; index < arguments.Length; ++index)
        argumentTypes[index] = arguments[index].ResultType;
      return EdmFunctions.ResolveCanonicalFunction(functionName, argumentTypes).Invoke(arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Avg' function over the specified collection. The result type of the expression is the same as the element type of the collection.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the average value.</returns>
    /// <param name="collection">An expression that specifies the collection from which the average value should be computed.</param>
    public static DbFunctionExpression Average(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction("Avg", collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Count' function over the specified collection. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the count value.</returns>
    /// <param name="collection">An expression that specifies the collection over which the count value should be computed.</param>
    public static DbFunctionExpression Count(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Count), collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'BigCount' function over the specified collection. The result type of the expression is Edm.Int64.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the count value.</returns>
    /// <param name="collection">An expression that specifies the collection over which the count value should be computed.</param>
    public static DbFunctionExpression LongCount(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction("BigCount", collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Max' function over the specified collection. The result type of the expression is the same as the element type of the collection.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the maximum value.</returns>
    /// <param name="collection">An expression that specifies the collection from which the maximum value should be retrieved</param>
    public static DbFunctionExpression Max(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Max), collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Min' function over the specified collection. The result type of the expression is the same as the element type of the collection.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the minimum value.</returns>
    /// <param name="collection">An expression that specifies the collection from which the minimum value should be retrieved.</param>
    public static DbFunctionExpression Min(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Min), collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Sum' function over the specified collection. The result type of the expression is the same as the element type of the collection.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the sum.</returns>
    /// <param name="collection">An expression that specifies the collection from which the sum should be computed.</param>
    public static DbFunctionExpression Sum(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Sum), collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'StDev' function over the non-null members of the specified collection. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the standard deviation value over non-null members of the collection.</returns>
    /// <param name="collection">An expression that specifies the collection for which the standard deviation should be computed.</param>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "St")]
    public static DbFunctionExpression StDev(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction(nameof (StDev), collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'StDevP' function over the population of the specified collection. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the standard deviation value.</returns>
    /// <param name="collection">An expression that specifies the collection for which the standard deviation should be computed.</param>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "St")]
    public static DbFunctionExpression StDevP(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction(nameof (StDevP), collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Var' function over the non-null members of the specified collection. The result type of the expression is Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the statistical variance value for the non-null members of the collection.</returns>
    /// <param name="collection">An expression that specifies the collection for which the statistical variance should be computed.</param>
    public static DbFunctionExpression Var(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Var), collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'VarP' function over the population of the specified collection. The result type of the expression Edm.Double.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the statistical variance value.</returns>
    /// <param name="collection">An expression that specifies the collection for which the statistical variance should be computed.</param>
    public static DbFunctionExpression VarP(this DbExpression collection)
    {
      Check.NotNull<DbExpression>(collection, nameof (collection));
      return EdmFunctions.InvokeCanonicalFunction(nameof (VarP), collection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Concat' function with the specified arguments, which must each have a string result type. The result type of the expression is string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the concatenated string.</returns>
    /// <param name="string1">An expression that specifies the string that should appear first in the concatenated result string.</param>
    /// <param name="string2">An expression that specifies the string that should appear second in the concatenated result string.</param>
    public static DbFunctionExpression Concat(
      this DbExpression string1,
      DbExpression string2)
    {
      Check.NotNull<DbExpression>(string1, nameof (string1));
      Check.NotNull<DbExpression>(string2, nameof (string2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Concat), string1, string2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Contains' function with the specified arguments, which must each have a string result type. The result type of the expression is Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether or not searchedForString occurs within searchedString.</returns>
    /// <param name="searchedString">An expression that specifies the string to search for any occurence of searchedForString.</param>
    /// <param name="searchedForString">An expression that specifies the string to search for in searchedString.</param>
    public static DbExpression Contains(
      this DbExpression searchedString,
      DbExpression searchedForString)
    {
      Check.NotNull<DbExpression>(searchedString, nameof (searchedString));
      Check.NotNull<DbExpression>(searchedForString, nameof (searchedForString));
      return (DbExpression) EdmFunctions.InvokeCanonicalFunction(nameof (Contains), searchedString, searchedForString);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'EndsWith' function with the specified arguments, which must each have a string result type. The result type of the expression is Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether or not stringArgument ends with suffix.</returns>
    /// <param name="stringArgument">An expression that specifies the string that is searched at the end for string suffix.</param>
    /// <param name="suffix">An expression that specifies the target string that is searched for at the end of stringArgument.</param>
    public static DbFunctionExpression EndsWith(
      this DbExpression stringArgument,
      DbExpression suffix)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      Check.NotNull<DbExpression>(suffix, nameof (suffix));
      return EdmFunctions.InvokeCanonicalFunction(nameof (EndsWith), stringArgument, suffix);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'IndexOf' function with the specified arguments, which must each have a string result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the first index of stringToFind in searchString.</returns>
    /// <param name="searchString">An expression that specifies the string to search for stringToFind.</param>
    /// <param name="stringToFind">An expression that specifies the string to locate within searchString should be checked.</param>
    public static DbFunctionExpression IndexOf(
      this DbExpression searchString,
      DbExpression stringToFind)
    {
      Check.NotNull<DbExpression>(searchString, nameof (searchString));
      Check.NotNull<DbExpression>(stringToFind, nameof (stringToFind));
      return EdmFunctions.InvokeCanonicalFunction(nameof (IndexOf), stringToFind, searchString);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Left' function with the specified arguments, which must have a string and integer numeric result type. The result type of the expression is string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the the leftmost substring of length length from stringArgument.</returns>
    /// <param name="stringArgument">An expression that specifies the string from which to extract the leftmost substring.</param>
    /// <param name="length">An expression that specifies the length of the leftmost substring to extract from stringArgument.</param>
    public static DbFunctionExpression Left(
      this DbExpression stringArgument,
      DbExpression length)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      Check.NotNull<DbExpression>(length, nameof (length));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Left), stringArgument, length);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Length' function with the specified argument, which must have a string result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the length of stringArgument.</returns>
    /// <param name="stringArgument">An expression that specifies the string for which the length should be computed.</param>
    public static DbFunctionExpression Length(this DbExpression stringArgument)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Length), stringArgument);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Replace' function with the specified arguments, which must each have a string result type. The result type of the expression is also string.
    /// </summary>
    /// <returns>A new DbFunctionExpression than returns a new string based on stringArgument where every occurence of toReplace is replaced by replacement.</returns>
    /// <param name="stringArgument">An expression that specifies the string in which to perform the replacement operation.</param>
    /// <param name="toReplace">An expression that specifies the string that is replaced.</param>
    /// <param name="replacement">An expression that specifies the replacement string.</param>
    public static DbFunctionExpression Replace(
      this DbExpression stringArgument,
      DbExpression toReplace,
      DbExpression replacement)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      Check.NotNull<DbExpression>(toReplace, nameof (toReplace));
      Check.NotNull<DbExpression>(replacement, nameof (replacement));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Replace), stringArgument, toReplace, replacement);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Reverse' function with the specified argument, which must have a string result type. The result type of the expression is also string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that produces the reversed value of stringArgument.</returns>
    /// <param name="stringArgument">An expression that specifies the string to reverse.</param>
    public static DbFunctionExpression Reverse(this DbExpression stringArgument)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Reverse), stringArgument);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Right' function with the specified arguments, which must have a string and integer numeric result type. The result type of the expression is string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the the rightmost substring of length length from stringArgument.</returns>
    /// <param name="stringArgument">An expression that specifies the string from which to extract the rightmost substring.</param>
    /// <param name="length">An expression that specifies the length of the rightmost substring to extract from stringArgument.</param>
    public static DbFunctionExpression Right(
      this DbExpression stringArgument,
      DbExpression length)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      Check.NotNull<DbExpression>(length, nameof (length));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Right), stringArgument, length);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'StartsWith' function with the specified arguments, which must each have a string result type. The result type of the expression is Boolean.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a Boolean value indicating whether or not stringArgument starts with prefix.</returns>
    /// <param name="stringArgument">An expression that specifies the string that is searched at the start for string prefix.</param>
    /// <param name="prefix">An expression that specifies the target string that is searched for at the start of stringArgument.</param>
    public static DbFunctionExpression StartsWith(
      this DbExpression stringArgument,
      DbExpression prefix)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      Check.NotNull<DbExpression>(prefix, nameof (prefix));
      return EdmFunctions.InvokeCanonicalFunction(nameof (StartsWith), stringArgument, prefix);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Substring' function with the specified arguments, which must have a string and integer numeric result types. The result type of the expression is string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the substring of length length from stringArgument starting at start.</returns>
    /// <param name="stringArgument">An expression that specifies the string from which to extract the substring.</param>
    /// <param name="start">An expression that specifies the starting index from which the substring should be taken.</param>
    /// <param name="length">An expression that specifies the length of the substring.</param>
    public static DbFunctionExpression Substring(
      this DbExpression stringArgument,
      DbExpression start,
      DbExpression length)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      Check.NotNull<DbExpression>(start, nameof (start));
      Check.NotNull<DbExpression>(length, nameof (length));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Substring), stringArgument, start, length);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'ToLower' function with the specified argument, which must have a string result type. The result type of the expression is also string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns value of stringArgument converted to lower case.</returns>
    /// <param name="stringArgument">An expression that specifies the string that should be converted to lower case.</param>
    public static DbFunctionExpression ToLower(this DbExpression stringArgument)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      return EdmFunctions.InvokeCanonicalFunction(nameof (ToLower), stringArgument);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'ToUpper' function with the specified argument, which must have a string result type. The result type of the expression is also string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns value of stringArgument converted to upper case.</returns>
    /// <param name="stringArgument">An expression that specifies the string that should be converted to upper case.</param>
    public static DbFunctionExpression ToUpper(this DbExpression stringArgument)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      return EdmFunctions.InvokeCanonicalFunction(nameof (ToUpper), stringArgument);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Trim' function with the specified argument, which must have a string result type. The result type of the expression is also string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns value of stringArgument with leading and trailing space removed.</returns>
    /// <param name="stringArgument">An expression that specifies the string from which leading and trailing space should be removed.</param>
    public static DbFunctionExpression Trim(this DbExpression stringArgument)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Trim), stringArgument);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'RTrim' function with the specified argument, which must have a string result type. The result type of the expression is also string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns value of stringArgument with trailing space removed.</returns>
    /// <param name="stringArgument">An expression that specifies the string from which trailing space should be removed.</param>
    public static DbFunctionExpression TrimEnd(this DbExpression stringArgument)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      return EdmFunctions.InvokeCanonicalFunction("RTrim", stringArgument);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'LTrim' function with the specified argument, which must have a string result type. The result type of the expression is also string.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns value of stringArgument with leading space removed.</returns>
    /// <param name="stringArgument">An expression that specifies the string from which leading space should be removed.</param>
    public static DbFunctionExpression TrimStart(
      this DbExpression stringArgument)
    {
      Check.NotNull<DbExpression>(stringArgument, nameof (stringArgument));
      return EdmFunctions.InvokeCanonicalFunction("LTrim", stringArgument);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Year' function with the specified argument, which must have a DateTime or DateTimeOffset result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the integer year value from dateValue.</returns>
    /// <param name="dateValue">An expression that specifies the value from which the year should be retrieved.</param>
    public static DbFunctionExpression Year(this DbExpression dateValue)
    {
      Check.NotNull<DbExpression>(dateValue, nameof (dateValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Year), dateValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Month' function with the specified argument, which must have a DateTime or DateTimeOffset result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the integer month value from dateValue.</returns>
    /// <param name="dateValue">An expression that specifies the value from which the month should be retrieved.</param>
    public static DbFunctionExpression Month(this DbExpression dateValue)
    {
      Check.NotNull<DbExpression>(dateValue, nameof (dateValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Month), dateValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Day' function with the specified argument, which must have a DateTime or DateTimeOffset result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the integer day value from dateValue.</returns>
    /// <param name="dateValue">An expression that specifies the value from which the day should be retrieved.</param>
    public static DbFunctionExpression Day(this DbExpression dateValue)
    {
      Check.NotNull<DbExpression>(dateValue, nameof (dateValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Day), dateValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DayOfYear' function with the specified argument, which must have a DateTime or DateTimeOffset result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the integer day of year value from dateValue.</returns>
    /// <param name="dateValue">An expression that specifies the value from which the day within the year should be retrieved.</param>
    public static DbFunctionExpression DayOfYear(this DbExpression dateValue)
    {
      Check.NotNull<DbExpression>(dateValue, nameof (dateValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DayOfYear), dateValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Hour' function with the specified argument, which must have a DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the integer hour value from timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value from which the hour should be retrieved.</param>
    public static DbFunctionExpression Hour(this DbExpression timeValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Hour), timeValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Minute' function with the specified argument, which must have a DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the integer minute value from timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value from which the minute should be retrieved.</param>
    public static DbFunctionExpression Minute(this DbExpression timeValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Minute), timeValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Second' function with the specified argument, which must have a DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the integer second value from timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value from which the second should be retrieved.</param>
    public static DbFunctionExpression Second(this DbExpression timeValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Second), timeValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Millisecond' function with the specified argument, which must have a DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the integer millisecond value from timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value from which the millisecond should be retrieved.</param>
    public static DbFunctionExpression Millisecond(this DbExpression timeValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Millisecond), timeValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'GetTotalOffsetMinutes' function with the specified argument, which must have a DateTimeOffset result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of minutes dateTimeOffsetArgument is offset from GMT.</returns>
    /// <param name="dateTimeOffsetArgument">An expression that specifies the DateTimeOffset value from which the minute offset from GMT should be retrieved.</param>
    public static DbFunctionExpression GetTotalOffsetMinutes(
      this DbExpression dateTimeOffsetArgument)
    {
      Check.NotNull<DbExpression>(dateTimeOffsetArgument, nameof (dateTimeOffsetArgument));
      return EdmFunctions.InvokeCanonicalFunction(nameof (GetTotalOffsetMinutes), dateTimeOffsetArgument);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'CurrentDateTime' function.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the current date and time as an Edm.DateTime instance.</returns>
    public static DbFunctionExpression CurrentDateTime()
    {
      return EdmFunctions.InvokeCanonicalFunction(nameof (CurrentDateTime));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'CurrentDateTimeOffset' function.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the current date and time as an Edm.DateTimeOffset instance.</returns>
    public static DbFunctionExpression CurrentDateTimeOffset()
    {
      return EdmFunctions.InvokeCanonicalFunction(nameof (CurrentDateTimeOffset));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'CurrentUtcDateTime' function.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the current UTC date and time as an Edm.DateTime instance.</returns>
    public static DbFunctionExpression CurrentUtcDateTime()
    {
      return EdmFunctions.InvokeCanonicalFunction(nameof (CurrentUtcDateTime));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'TruncateTime' function with the specified argument, which must have a DateTime or DateTimeOffset result type. The result type of the expression is the same as the result type of dateValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the value of dateValue with time set to zero.</returns>
    /// <param name="dateValue">An expression that specifies the value for which the time portion should be truncated.</param>
    public static DbFunctionExpression TruncateTime(this DbExpression dateValue)
    {
      Check.NotNull<DbExpression>(dateValue, nameof (dateValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (TruncateTime), dateValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'CreateDateTime' function with the specified arguments. second must have a result type of Edm.Double, while all other arguments must have a result type of Edm.Int32. The result type of the expression is Edm.DateTime.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new DateTime based on the specified values.</returns>
    /// <param name="year">An expression that provides the year value for the new DateTime instance.</param>
    /// <param name="month">An expression that provides the month value for the new DateTime instance.</param>
    /// <param name="day">An expression that provides the day value for the new DateTime instance.</param>
    /// <param name="hour">An expression that provides the hour value for the new DateTime instance.</param>
    /// <param name="minute">An expression that provides the minute value for the new DateTime instance.</param>
    /// <param name="second">An expression that provides the second value for the new DateTime instance.</param>
    public static DbFunctionExpression CreateDateTime(
      DbExpression year,
      DbExpression month,
      DbExpression day,
      DbExpression hour,
      DbExpression minute,
      DbExpression second)
    {
      Check.NotNull<DbExpression>(year, nameof (year));
      Check.NotNull<DbExpression>(month, nameof (month));
      Check.NotNull<DbExpression>(day, nameof (day));
      Check.NotNull<DbExpression>(hour, nameof (hour));
      Check.NotNull<DbExpression>(minute, nameof (minute));
      Check.NotNull<DbExpression>(second, nameof (second));
      return EdmFunctions.InvokeCanonicalFunction(nameof (CreateDateTime), year, month, day, hour, minute, second);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'CreateDateTimeOffset' function with the specified arguments. second must have a result type of Edm.Double, while all other arguments must have a result type of Edm.Int32. The result type of the expression is Edm.DateTimeOffset.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new DateTimeOffset based on the specified values.</returns>
    /// <param name="year">An expression that provides the year value for the new DateTimeOffset instance.</param>
    /// <param name="month">An expression that provides the month value for the new DateTimeOffset instance.</param>
    /// <param name="day">An expression that provides the day value for the new DateTimeOffset instance.</param>
    /// <param name="hour">An expression that provides the hour value for the new DateTimeOffset instance.</param>
    /// <param name="minute">An expression that provides the minute value for the new DateTimeOffset instance.</param>
    /// <param name="second">An expression that provides the second value for the new DateTimeOffset instance.</param>
    /// <param name="timeZoneOffset">An expression that provides the number of minutes in the time zone offset value for the new DateTimeOffset instance.</param>
    public static DbFunctionExpression CreateDateTimeOffset(
      DbExpression year,
      DbExpression month,
      DbExpression day,
      DbExpression hour,
      DbExpression minute,
      DbExpression second,
      DbExpression timeZoneOffset)
    {
      Check.NotNull<DbExpression>(year, nameof (year));
      Check.NotNull<DbExpression>(month, nameof (month));
      Check.NotNull<DbExpression>(day, nameof (day));
      Check.NotNull<DbExpression>(hour, nameof (hour));
      Check.NotNull<DbExpression>(minute, nameof (minute));
      Check.NotNull<DbExpression>(second, nameof (second));
      Check.NotNull<DbExpression>(timeZoneOffset, nameof (timeZoneOffset));
      return EdmFunctions.InvokeCanonicalFunction(nameof (CreateDateTimeOffset), year, month, day, hour, minute, second, timeZoneOffset);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'CreateTime' function with the specified arguments. second must have a result type of Edm.Double, while all other arguments must have a result type of Edm.Int32. The result type of the expression is Edm.Time.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new Time based on the specified values.</returns>
    /// <param name="hour">An expression that provides the hour value for the new DateTime instance.</param>
    /// <param name="minute">An expression that provides the minute value for the new DateTime instance.</param>
    /// <param name="second">An expression that provides the second value for the new DateTime instance.</param>
    public static DbFunctionExpression CreateTime(
      DbExpression hour,
      DbExpression minute,
      DbExpression second)
    {
      Check.NotNull<DbExpression>(hour, nameof (hour));
      Check.NotNull<DbExpression>(minute, nameof (minute));
      Check.NotNull<DbExpression>(second, nameof (second));
      return EdmFunctions.InvokeCanonicalFunction(nameof (CreateTime), hour, minute, second);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AddYears' function with the specified arguments, which must have DateTime or DateTimeOffset and integer result types. The result type of the expression is the same as the result type of dateValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that adds the number of years specified by addValue to the value specified by dateValue.</returns>
    /// <param name="dateValue">An expression that specifies the value to which addValueshould be added.</param>
    /// <param name="addValue">An expression that specifies the number of years to add to dateValue.</param>
    public static DbFunctionExpression AddYears(
      this DbExpression dateValue,
      DbExpression addValue)
    {
      Check.NotNull<DbExpression>(dateValue, nameof (dateValue));
      Check.NotNull<DbExpression>(addValue, nameof (addValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AddYears), dateValue, addValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AddMonths' function with the specified arguments, which must have DateTime or DateTimeOffset and integer result types. The result type of the expression is the same as the result type of dateValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that adds the number of months specified by addValue to the value specified by dateValue.</returns>
    /// <param name="dateValue">An expression that specifies the value to which addValueshould be added.</param>
    /// <param name="addValue">An expression that specifies the number of months to add to dateValue.</param>
    public static DbFunctionExpression AddMonths(
      this DbExpression dateValue,
      DbExpression addValue)
    {
      Check.NotNull<DbExpression>(dateValue, nameof (dateValue));
      Check.NotNull<DbExpression>(addValue, nameof (addValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AddMonths), dateValue, addValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AddDays' function with the specified arguments, which must have DateTime or DateTimeOffset and integer result types. The result type of the expression is the same as the result type of dateValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that adds the number of days specified by addValue to the value specified by dateValue.</returns>
    /// <param name="dateValue">An expression that specifies the value to which addValueshould be added.</param>
    /// <param name="addValue">An expression that specifies the number of days to add to dateValue.</param>
    public static DbFunctionExpression AddDays(
      this DbExpression dateValue,
      DbExpression addValue)
    {
      Check.NotNull<DbExpression>(dateValue, nameof (dateValue));
      Check.NotNull<DbExpression>(addValue, nameof (addValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AddDays), dateValue, addValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AddHours' function with the specified arguments, which must have DateTime, DateTimeOffset or Time, and integer result types. The result type of the expression is the same as the result type of timeValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that adds the number of hours specified by addValue to the value specified by timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value to which addValueshould be added.</param>
    /// <param name="addValue">An expression that specifies the number of hours to add to timeValue.</param>
    public static DbFunctionExpression AddHours(
      this DbExpression timeValue,
      DbExpression addValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      Check.NotNull<DbExpression>(addValue, nameof (addValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AddHours), timeValue, addValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AddMinutes' function with the specified arguments, which must have DateTime, DateTimeOffset or Time, and integer result types. The result type of the expression is the same as the result type of timeValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that adds the number of minutes specified by addValue to the value specified by timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value to which addValueshould be added.</param>
    /// <param name="addValue">An expression that specifies the number of minutes to add to timeValue.</param>
    public static DbFunctionExpression AddMinutes(
      this DbExpression timeValue,
      DbExpression addValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      Check.NotNull<DbExpression>(addValue, nameof (addValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AddMinutes), timeValue, addValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AddSeconds' function with the specified arguments, which must have DateTime, DateTimeOffset or Time, and integer result types. The result type of the expression is the same as the result type of timeValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that adds the number of seconds specified by addValue to the value specified by timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value to which addValueshould be added.</param>
    /// <param name="addValue">An expression that specifies the number of seconds to add to timeValue.</param>
    public static DbFunctionExpression AddSeconds(
      this DbExpression timeValue,
      DbExpression addValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      Check.NotNull<DbExpression>(addValue, nameof (addValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AddSeconds), timeValue, addValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AddMilliseconds' function with the specified arguments, which must have DateTime, DateTimeOffset or Time, and integer result types. The result type of the expression is the same as the result type of timeValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that adds the number of milliseconds specified by addValue to the value specified by timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value to which addValueshould be added.</param>
    /// <param name="addValue">An expression that specifies the number of milliseconds to add to timeValue.</param>
    public static DbFunctionExpression AddMilliseconds(
      this DbExpression timeValue,
      DbExpression addValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      Check.NotNull<DbExpression>(addValue, nameof (addValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AddMilliseconds), timeValue, addValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AddMicroseconds' function with the specified arguments, which must have DateTime, DateTimeOffset or Time, and integer result types. The result type of the expression is the same as the result type of timeValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that adds the number of microseconds specified by addValue to the value specified by timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value to which addValueshould be added.</param>
    /// <param name="addValue">An expression that specifies the number of microseconds to add to timeValue.</param>
    public static DbFunctionExpression AddMicroseconds(
      this DbExpression timeValue,
      DbExpression addValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      Check.NotNull<DbExpression>(addValue, nameof (addValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AddMicroseconds), timeValue, addValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'AddNanoseconds' function with the specified arguments, which must have DateTime, DateTimeOffset or Time, and integer result types. The result type of the expression is the same as the result type of timeValue.
    /// </summary>
    /// <returns>A new DbFunctionExpression that adds the number of nanoseconds specified by addValue to the value specified by timeValue.</returns>
    /// <param name="timeValue">An expression that specifies the value to which addValueshould be added.</param>
    /// <param name="addValue">An expression that specifies the number of nanoseconds to add to timeValue.</param>
    public static DbFunctionExpression AddNanoseconds(
      this DbExpression timeValue,
      DbExpression addValue)
    {
      Check.NotNull<DbExpression>(timeValue, nameof (timeValue));
      Check.NotNull<DbExpression>(addValue, nameof (addValue));
      return EdmFunctions.InvokeCanonicalFunction(nameof (AddNanoseconds), timeValue, addValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DiffYears' function with the specified arguments, which must each have DateTime or DateTimeOffset result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of years that is the difference between dateValue1 and dateValue2.</returns>
    /// <param name="dateValue1">An expression that specifies the first date value argument.</param>
    /// <param name="dateValue2">An expression that specifies the second date value argument.</param>
    public static DbFunctionExpression DiffYears(
      this DbExpression dateValue1,
      DbExpression dateValue2)
    {
      Check.NotNull<DbExpression>(dateValue1, nameof (dateValue1));
      Check.NotNull<DbExpression>(dateValue2, nameof (dateValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DiffYears), dateValue1, dateValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DiffMonths' function with the specified arguments, which must each have DateTime or DateTimeOffset result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of months that is the difference between dateValue1 and dateValue2.</returns>
    /// <param name="dateValue1">An expression that specifies the first date value argument.</param>
    /// <param name="dateValue2">An expression that specifies the second date value argument.</param>
    public static DbFunctionExpression DiffMonths(
      this DbExpression dateValue1,
      DbExpression dateValue2)
    {
      Check.NotNull<DbExpression>(dateValue1, nameof (dateValue1));
      Check.NotNull<DbExpression>(dateValue2, nameof (dateValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DiffMonths), dateValue1, dateValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DiffDays' function with the specified arguments, which must each have DateTime or DateTimeOffset result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of days that is the difference between dateValue1 and dateValue2.</returns>
    /// <param name="dateValue1">An expression that specifies the first date value argument.</param>
    /// <param name="dateValue2">An expression that specifies the second date value argument.</param>
    public static DbFunctionExpression DiffDays(
      this DbExpression dateValue1,
      DbExpression dateValue2)
    {
      Check.NotNull<DbExpression>(dateValue1, nameof (dateValue1));
      Check.NotNull<DbExpression>(dateValue2, nameof (dateValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DiffDays), dateValue1, dateValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DiffHours' function with the specified arguments, which must each have DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of hours that is the difference between timeValue1 and timeValue2.</returns>
    /// <param name="timeValue1">An expression that specifies the first time value argument.</param>
    /// <param name="timeValue2">An expression that specifies the second time value argument.</param>
    public static DbFunctionExpression DiffHours(
      this DbExpression timeValue1,
      DbExpression timeValue2)
    {
      Check.NotNull<DbExpression>(timeValue1, nameof (timeValue1));
      Check.NotNull<DbExpression>(timeValue2, nameof (timeValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DiffHours), timeValue1, timeValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DiffMinutes' function with the specified arguments, which must each have DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of minutes that is the difference between timeValue1 and timeValue2.</returns>
    /// <param name="timeValue1">An expression that specifies the first time value argument.</param>
    /// <param name="timeValue2">An expression that specifies the second time value argument.</param>
    public static DbFunctionExpression DiffMinutes(
      this DbExpression timeValue1,
      DbExpression timeValue2)
    {
      Check.NotNull<DbExpression>(timeValue1, nameof (timeValue1));
      Check.NotNull<DbExpression>(timeValue2, nameof (timeValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DiffMinutes), timeValue1, timeValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DiffSeconds' function with the specified arguments, which must each have DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of seconds that is the difference between timeValue1 and timeValue2.</returns>
    /// <param name="timeValue1">An expression that specifies the first time value argument.</param>
    /// <param name="timeValue2">An expression that specifies the second time value argument.</param>
    public static DbFunctionExpression DiffSeconds(
      this DbExpression timeValue1,
      DbExpression timeValue2)
    {
      Check.NotNull<DbExpression>(timeValue1, nameof (timeValue1));
      Check.NotNull<DbExpression>(timeValue2, nameof (timeValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DiffSeconds), timeValue1, timeValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DiffMilliseconds' function with the specified arguments, which must each have DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of milliseconds that is the difference between timeValue1 and timeValue2.</returns>
    /// <param name="timeValue1">An expression that specifies the first time value argument.</param>
    /// <param name="timeValue2">An expression that specifies the second time value argument.</param>
    public static DbFunctionExpression DiffMilliseconds(
      this DbExpression timeValue1,
      DbExpression timeValue2)
    {
      Check.NotNull<DbExpression>(timeValue1, nameof (timeValue1));
      Check.NotNull<DbExpression>(timeValue2, nameof (timeValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DiffMilliseconds), timeValue1, timeValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DiffMicroseconds' function with the specified arguments, which must each have DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of microseconds that is the difference between timeValue1 and timeValue2.</returns>
    /// <param name="timeValue1">An expression that specifies the first time value argument.</param>
    /// <param name="timeValue2">An expression that specifies the second time value argument.</param>
    public static DbFunctionExpression DiffMicroseconds(
      this DbExpression timeValue1,
      DbExpression timeValue2)
    {
      Check.NotNull<DbExpression>(timeValue1, nameof (timeValue1));
      Check.NotNull<DbExpression>(timeValue2, nameof (timeValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DiffMicroseconds), timeValue1, timeValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'DiffNanoseconds' function with the specified arguments, which must each have DateTime, DateTimeOffset or Time result type. The result type of the expression is Edm.Int32.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the number of nanoseconds that is the difference between timeValue1 and timeValue2.</returns>
    /// <param name="timeValue1">An expression that specifies the first time value argument.</param>
    /// <param name="timeValue2">An expression that specifies the second time value argument.</param>
    public static DbFunctionExpression DiffNanoseconds(
      this DbExpression timeValue1,
      DbExpression timeValue2)
    {
      Check.NotNull<DbExpression>(timeValue1, nameof (timeValue1));
      Check.NotNull<DbExpression>(timeValue2, nameof (timeValue2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (DiffNanoseconds), timeValue1, timeValue2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Round' function with the specified argument, which must each have a single, double or decimal result type. The result type of the expression is the same as the result type of value.
    /// </summary>
    /// <returns>A new DbFunctionExpression that rounds the specified argument to the nearest integer value.</returns>
    /// <param name="value">An expression that specifies the numeric value to round.</param>
    public static DbFunctionExpression Round(this DbExpression value)
    {
      Check.NotNull<DbExpression>(value, nameof (value));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Round), value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Round' function with the specified arguments, which must have a single, double or decimal, and integer result types. The result type of the expression is the same as the result type of value.
    /// </summary>
    /// <returns>A new DbFunctionExpression that rounds the specified argument to the nearest integer value, with precision as specified by digits.</returns>
    /// <param name="value">An expression that specifies the numeric value to round.</param>
    /// <param name="digits">An expression that specifies the number of digits of precision to use when rounding.</param>
    public static DbFunctionExpression Round(
      this DbExpression value,
      DbExpression digits)
    {
      Check.NotNull<DbExpression>(value, nameof (value));
      Check.NotNull<DbExpression>(digits, nameof (digits));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Round), value, digits);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Floor' function with the specified argument, which must each have a single, double or decimal result type. The result type of the expression is the same as the result type of value.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the largest integer value not greater than value.</returns>
    /// <param name="value">An expression that specifies the numeric value.</param>
    public static DbFunctionExpression Floor(this DbExpression value)
    {
      Check.NotNull<DbExpression>(value, nameof (value));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Floor), value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Ceiling' function with the specified argument, which must each have a single, double or decimal result type. The result type of the expression is the same as the result type of value.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the smallest integer value not less than than value.</returns>
    /// <param name="value">An expression that specifies the numeric value.</param>
    public static DbFunctionExpression Ceiling(this DbExpression value)
    {
      Check.NotNull<DbExpression>(value, nameof (value));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Ceiling), value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Abs' function with the specified argument, which must each have a numeric result type. The result type of the expression is the same as the result type of value.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the absolute value of value.</returns>
    /// <param name="value">An expression that specifies the numeric value.</param>
    public static DbFunctionExpression Abs(this DbExpression value)
    {
      Check.NotNull<DbExpression>(value, nameof (value));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Abs), value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Truncate' function with the specified arguments, which must have a single, double or decimal, and integer result types. The result type of the expression is the same as the result type of value.
    /// </summary>
    /// <returns>A new DbFunctionExpression that truncates the specified argument to the nearest integer value, with precision as specified by digits.</returns>
    /// <param name="value">An expression that specifies the numeric value to truncate.</param>
    /// <param name="digits">An expression that specifies the number of digits of precision to use when truncating.</param>
    public static DbFunctionExpression Truncate(
      this DbExpression value,
      DbExpression digits)
    {
      Check.NotNull<DbExpression>(value, nameof (value));
      Check.NotNull<DbExpression>(digits, nameof (digits));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Truncate), value, digits);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'Power' function with the specified arguments, which must have numeric result types. The result type of the expression is the same as the result type of baseArgument.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the value of baseArgument raised to the power specified by exponent.</returns>
    /// <param name="baseArgument">An expression that specifies the numeric value to raise to the given power.</param>
    /// <param name="exponent">An expression that specifies the power to which baseArgument should be raised.</param>
    public static DbFunctionExpression Power(
      this DbExpression baseArgument,
      DbExpression exponent)
    {
      Check.NotNull<DbExpression>(baseArgument, nameof (baseArgument));
      Check.NotNull<DbExpression>(exponent, nameof (exponent));
      return EdmFunctions.InvokeCanonicalFunction(nameof (Power), baseArgument, exponent);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'BitwiseAnd' function with the specified arguments, which must have the same integer numeric result type. The result type of the expression is the same as the type of the arguments.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the value produced by performing the bitwise AND of value1 and value2.</returns>
    /// <param name="value1">An expression that specifies the first operand.</param>
    /// <param name="value2">An expression that specifies the second operand.</param>
    public static DbFunctionExpression BitwiseAnd(
      this DbExpression value1,
      DbExpression value2)
    {
      Check.NotNull<DbExpression>(value1, nameof (value1));
      Check.NotNull<DbExpression>(value2, nameof (value2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (BitwiseAnd), value1, value2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'BitwiseOr' function with the specified arguments, which must have the same integer numeric result type. The result type of the expression is the same as the type of the arguments.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the value produced by performing the bitwise OR of value1 and value2.</returns>
    /// <param name="value1">An expression that specifies the first operand.</param>
    /// <param name="value2">An expression that specifies the second operand.</param>
    public static DbFunctionExpression BitwiseOr(
      this DbExpression value1,
      DbExpression value2)
    {
      Check.NotNull<DbExpression>(value1, nameof (value1));
      Check.NotNull<DbExpression>(value2, nameof (value2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (BitwiseOr), value1, value2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'BitwiseNot' function with the specified argument, which must have an integer numeric result type. The result type of the expression is the same as the type of the arguments.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the value produced by performing the bitwise NOT of value.</returns>
    /// <param name="value">An expression that specifies the first operand.</param>
    public static DbFunctionExpression BitwiseNot(this DbExpression value)
    {
      Check.NotNull<DbExpression>(value, nameof (value));
      return EdmFunctions.InvokeCanonicalFunction(nameof (BitwiseNot), value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'BitwiseXor' function with the specified arguments, which must have the same integer numeric result type. The result type of the expression is the same as the type of the arguments.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns the value produced by performing the bitwise XOR (exclusive OR) of value1 and value2.</returns>
    /// <param name="value1">An expression that specifies the first operand.</param>
    /// <param name="value2">An expression that specifies the second operand.</param>
    public static DbFunctionExpression BitwiseXor(
      this DbExpression value1,
      DbExpression value2)
    {
      Check.NotNull<DbExpression>(value1, nameof (value1));
      Check.NotNull<DbExpression>(value2, nameof (value2));
      return EdmFunctions.InvokeCanonicalFunction(nameof (BitwiseXor), value1, value2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> that invokes the canonical 'NewGuid' function.
    /// </summary>
    /// <returns>A new DbFunctionExpression that returns a new GUID value.</returns>
    public static DbFunctionExpression NewGuid()
    {
      return EdmFunctions.InvokeCanonicalFunction(nameof (NewGuid));
    }
  }
}
