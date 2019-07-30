// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Pluralization.PluralizationServiceUtil
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Infrastructure.Pluralization
{
  internal static class PluralizationServiceUtil
  {
    internal static bool DoesWordContainSuffix(
      string word,
      IEnumerable<string> suffixes,
      CultureInfo culture)
    {
      return suffixes.Any<string>((Func<string, bool>) (s => word.EndsWith(s, true, culture)));
    }

    internal static bool TryGetMatchedSuffixForWord(
      string word,
      IEnumerable<string> suffixes,
      CultureInfo culture,
      out string matchedSuffix)
    {
      matchedSuffix = (string) null;
      if (!PluralizationServiceUtil.DoesWordContainSuffix(word, suffixes, culture))
        return false;
      matchedSuffix = suffixes.First<string>((Func<string, bool>) (s => word.EndsWith(s, true, culture)));
      return true;
    }

    internal static bool TryInflectOnSuffixInWord(
      string word,
      IEnumerable<string> suffixes,
      Func<string, string> operationOnWord,
      CultureInfo culture,
      out string newWord)
    {
      newWord = (string) null;
      string matchedSuffix;
      if (!PluralizationServiceUtil.TryGetMatchedSuffixForWord(word, suffixes, culture, out matchedSuffix))
        return false;
      newWord = operationOnWord(word);
      return true;
    }
  }
}
