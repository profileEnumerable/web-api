// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Design.PluralizationServices.StringBidirectionalDictionary
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Design.PluralizationServices
{
  internal class StringBidirectionalDictionary : BidirectionalDictionary<string, string>
  {
    internal StringBidirectionalDictionary()
    {
    }

    internal StringBidirectionalDictionary(Dictionary<string, string> firstToSecondDictionary)
      : base(firstToSecondDictionary)
    {
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    internal override bool ExistsInFirst(string value)
    {
      return base.ExistsInFirst(value.ToLowerInvariant());
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    internal override bool ExistsInSecond(string value)
    {
      return base.ExistsInSecond(value.ToLowerInvariant());
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    internal override string GetFirstValue(string value)
    {
      return base.GetFirstValue(value.ToLowerInvariant());
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    internal override string GetSecondValue(string value)
    {
      return base.GetSecondValue(value.ToLowerInvariant());
    }
  }
}
