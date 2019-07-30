// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.MultipartIdentifier
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Data.Entity.Core.Common.Internal
{
  internal static class MultipartIdentifier
  {
    private const int MaxParts = 4;
    internal const int ServerIndex = 0;
    internal const int CatalogIndex = 1;
    internal const int SchemaIndex = 2;
    internal const int TableIndex = 3;

    private static void IncrementStringCount(List<string> ary, ref int position)
    {
      ++position;
      ary.Add(string.Empty);
    }

    private static bool IsWhitespace(char ch)
    {
      return char.IsWhiteSpace(ch);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    internal static List<string> ParseMultipartIdentifier(
      string name,
      string leftQuote,
      string rightQuote,
      char separator)
    {
      List<string> ary = new List<string>();
      ary.Add((string) null);
      int position = 0;
      MultipartIdentifier.MPIState mpiState = MultipartIdentifier.MPIState.MPI_Value;
      StringBuilder stringBuilder1 = new StringBuilder(name.Length);
      StringBuilder stringBuilder2 = (StringBuilder) null;
      char ch1 = ' ';
      for (int index1 = 0; index1 < name.Length; ++index1)
      {
        char ch2 = name[index1];
        switch (mpiState)
        {
          case MultipartIdentifier.MPIState.MPI_Value:
            if (!MultipartIdentifier.IsWhitespace(ch2))
            {
              if ((int) ch2 == (int) separator)
              {
                ary[position] = string.Empty;
                MultipartIdentifier.IncrementStringCount(ary, ref position);
                break;
              }
              int index2;
              if (-1 != (index2 = leftQuote.IndexOf(ch2)))
              {
                ch1 = rightQuote[index2];
                stringBuilder1.Length = 0;
                mpiState = MultipartIdentifier.MPIState.MPI_ParseQuote;
                break;
              }
              if (-1 != rightQuote.IndexOf(ch2))
                throw new ArgumentException(Strings.ADP_InvalidMultipartNameDelimiterUsage, "path");
              stringBuilder1.Length = 0;
              stringBuilder1.Append(ch2);
              mpiState = MultipartIdentifier.MPIState.MPI_ParseNonQuote;
              break;
            }
            break;
          case MultipartIdentifier.MPIState.MPI_ParseNonQuote:
            if ((int) ch2 == (int) separator)
            {
              ary[position] = stringBuilder1.ToString();
              MultipartIdentifier.IncrementStringCount(ary, ref position);
              mpiState = MultipartIdentifier.MPIState.MPI_Value;
              break;
            }
            if (-1 != rightQuote.IndexOf(ch2))
              throw new ArgumentException(Strings.ADP_InvalidMultipartNameDelimiterUsage, "path");
            if (-1 != leftQuote.IndexOf(ch2))
              throw new ArgumentException(Strings.ADP_InvalidMultipartNameDelimiterUsage, "path");
            if (MultipartIdentifier.IsWhitespace(ch2))
            {
              ary[position] = stringBuilder1.ToString();
              if (stringBuilder2 == null)
                stringBuilder2 = new StringBuilder();
              stringBuilder2.Length = 0;
              stringBuilder2.Append(ch2);
              mpiState = MultipartIdentifier.MPIState.MPI_LookForNextCharOrSeparator;
              break;
            }
            stringBuilder1.Append(ch2);
            break;
          case MultipartIdentifier.MPIState.MPI_LookForSeparator:
            if (!MultipartIdentifier.IsWhitespace(ch2))
            {
              if ((int) ch2 != (int) separator)
                throw new ArgumentException(Strings.ADP_InvalidMultipartNameDelimiterUsage, "path");
              MultipartIdentifier.IncrementStringCount(ary, ref position);
              mpiState = MultipartIdentifier.MPIState.MPI_Value;
              break;
            }
            break;
          case MultipartIdentifier.MPIState.MPI_LookForNextCharOrSeparator:
            if (!MultipartIdentifier.IsWhitespace(ch2))
            {
              if ((int) ch2 == (int) separator)
              {
                MultipartIdentifier.IncrementStringCount(ary, ref position);
                mpiState = MultipartIdentifier.MPIState.MPI_Value;
                break;
              }
              stringBuilder1.Append((object) stringBuilder2);
              stringBuilder1.Append(ch2);
              ary[position] = stringBuilder1.ToString();
              mpiState = MultipartIdentifier.MPIState.MPI_ParseNonQuote;
              break;
            }
            stringBuilder2.Append(ch2);
            break;
          case MultipartIdentifier.MPIState.MPI_ParseQuote:
            if ((int) ch2 == (int) ch1)
            {
              mpiState = MultipartIdentifier.MPIState.MPI_RightQuote;
              break;
            }
            stringBuilder1.Append(ch2);
            break;
          case MultipartIdentifier.MPIState.MPI_RightQuote:
            if ((int) ch2 == (int) ch1)
            {
              stringBuilder1.Append(ch2);
              mpiState = MultipartIdentifier.MPIState.MPI_ParseQuote;
              break;
            }
            if ((int) ch2 == (int) separator)
            {
              ary[position] = stringBuilder1.ToString();
              MultipartIdentifier.IncrementStringCount(ary, ref position);
              mpiState = MultipartIdentifier.MPIState.MPI_Value;
              break;
            }
            if (!MultipartIdentifier.IsWhitespace(ch2))
              throw new ArgumentException(Strings.ADP_InvalidMultipartNameDelimiterUsage, "path");
            ary[position] = stringBuilder1.ToString();
            mpiState = MultipartIdentifier.MPIState.MPI_LookForSeparator;
            break;
        }
      }
      switch (mpiState)
      {
        case MultipartIdentifier.MPIState.MPI_Value:
        case MultipartIdentifier.MPIState.MPI_LookForSeparator:
        case MultipartIdentifier.MPIState.MPI_LookForNextCharOrSeparator:
          return ary;
        case MultipartIdentifier.MPIState.MPI_ParseNonQuote:
        case MultipartIdentifier.MPIState.MPI_RightQuote:
          ary[position] = stringBuilder1.ToString();
          goto case MultipartIdentifier.MPIState.MPI_Value;
        default:
          throw new ArgumentException(Strings.ADP_InvalidMultipartNameDelimiterUsage, "path");
      }
    }

    private enum MPIState
    {
      MPI_Value,
      MPI_ParseNonQuote,
      MPI_LookForSeparator,
      MPI_LookForNextCharOrSeparator,
      MPI_ParseQuote,
      MPI_RightQuote,
    }
  }
}
