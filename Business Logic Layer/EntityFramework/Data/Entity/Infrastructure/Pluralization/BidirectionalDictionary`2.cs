// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Pluralization.BidirectionalDictionary`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Infrastructure.Pluralization
{
  internal class BidirectionalDictionary<TFirst, TSecond>
  {
    internal Dictionary<TFirst, TSecond> FirstToSecondDictionary { get; set; }

    internal Dictionary<TSecond, TFirst> SecondToFirstDictionary { get; set; }

    internal BidirectionalDictionary()
    {
      this.FirstToSecondDictionary = new Dictionary<TFirst, TSecond>();
      this.SecondToFirstDictionary = new Dictionary<TSecond, TFirst>();
    }

    internal BidirectionalDictionary(
      Dictionary<TFirst, TSecond> firstToSecondDictionary)
      : this()
    {
      foreach (TFirst key in firstToSecondDictionary.Keys)
        this.AddValue(key, firstToSecondDictionary[key]);
    }

    internal virtual bool ExistsInFirst(TFirst value)
    {
      return this.FirstToSecondDictionary.ContainsKey(value);
    }

    internal virtual bool ExistsInSecond(TSecond value)
    {
      return this.SecondToFirstDictionary.ContainsKey(value);
    }

    internal virtual TSecond GetSecondValue(TFirst value)
    {
      if (this.ExistsInFirst(value))
        return this.FirstToSecondDictionary[value];
      return default (TSecond);
    }

    internal virtual TFirst GetFirstValue(TSecond value)
    {
      if (this.ExistsInSecond(value))
        return this.SecondToFirstDictionary[value];
      return default (TFirst);
    }

    internal void AddValue(TFirst firstValue, TSecond secondValue)
    {
      this.FirstToSecondDictionary.Add(firstValue, secondValue);
      if (this.SecondToFirstDictionary.ContainsKey(secondValue))
        return;
      this.SecondToFirstDictionary.Add(secondValue, firstValue);
    }
  }
}
