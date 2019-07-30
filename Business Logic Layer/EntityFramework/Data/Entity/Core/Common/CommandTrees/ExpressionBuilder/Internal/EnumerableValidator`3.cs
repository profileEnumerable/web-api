// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder.Internal.EnumerableValidator`3
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder.Internal
{
  internal sealed class EnumerableValidator<TElementIn, TElementOut, TResult>
  {
    private int expectedElementCount = -1;
    private readonly string argumentName;
    private readonly IEnumerable<TElementIn> target;

    internal EnumerableValidator(IEnumerable<TElementIn> argument, string argumentName)
    {
      this.argumentName = argumentName;
      this.target = argument;
    }

    public bool AllowEmpty { get; set; }

    public int ExpectedElementCount
    {
      get
      {
        return this.expectedElementCount;
      }
      set
      {
        this.expectedElementCount = value;
      }
    }

    public Func<TElementIn, int, TElementOut> ConvertElement { get; set; }

    public Func<List<TElementOut>, TResult> CreateResult { get; set; }

    public Func<TElementIn, int, string> GetName { get; set; }

    internal TResult Validate()
    {
      return EnumerableValidator<TElementIn, TElementOut, TResult>.Validate(this.target, this.argumentName, this.ExpectedElementCount, this.AllowEmpty, this.ConvertElement, this.CreateResult, this.GetName);
    }

    private static TResult Validate(
      IEnumerable<TElementIn> argument,
      string argumentName,
      int expectedElementCount,
      bool allowEmpty,
      Func<TElementIn, int, TElementOut> map,
      Func<List<TElementOut>, TResult> collect,
      Func<TElementIn, int, string> deriveName)
    {
      bool flag1 = (object) default (TElementIn) == null;
      bool flag2 = expectedElementCount != -1;
      Dictionary<string, int> dictionary = (Dictionary<string, int>) null;
      if (deriveName != null)
        dictionary = new Dictionary<string, int>();
      int index = 0;
      List<TElementOut> elementOutList = new List<TElementOut>();
      foreach (TElementIn elementIn in argument)
      {
        if (flag2 && index == expectedElementCount)
          throw new ArgumentException(Strings.Cqt_ExpressionList_IncorrectElementCount, argumentName);
        if (flag1 && (object) elementIn == null)
          throw new ArgumentNullException(StringUtil.FormatIndex(argumentName, index));
        TElementOut elementOut = map(elementIn, index);
        elementOutList.Add(elementOut);
        if (deriveName != null)
        {
          string key = deriveName(elementIn, index);
          int num = -1;
          if (dictionary.TryGetValue(key, out num))
            throw new ArgumentException(Strings.Cqt_Util_CheckListDuplicateName((object) num, (object) index, (object) key), StringUtil.FormatIndex(argumentName, index));
          dictionary[key] = index;
        }
        ++index;
      }
      if (flag2)
      {
        if (index != expectedElementCount)
          throw new ArgumentException(Strings.Cqt_ExpressionList_IncorrectElementCount, argumentName);
      }
      else if (index == 0 && !allowEmpty)
        throw new ArgumentException(Strings.Cqt_Util_CheckListEmptyInvalid, argumentName);
      return collect(elementOutList);
    }
  }
}
