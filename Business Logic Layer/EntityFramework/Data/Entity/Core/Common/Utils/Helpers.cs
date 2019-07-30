// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Helpers
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace System.Data.Entity.Core.Common.Utils
{
  internal static class Helpers
  {
    internal static void FormatTraceLine(string format, params object[] args)
    {
      Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args));
    }

    internal static void StringTrace(string arg)
    {
      Trace.Write(arg);
    }

    internal static void StringTraceLine(string arg)
    {
      Trace.WriteLine(arg);
    }

    internal static bool IsSetEqual<Type>(
      IEnumerable<Type> list1,
      IEnumerable<Type> list2,
      IEqualityComparer<Type> comparer)
    {
      return new Set<Type>(list1, comparer).SetEquals(new Set<Type>(list2, comparer));
    }

    internal static IEnumerable<SuperType> AsSuperTypeList<SubType, SuperType>(
      IEnumerable<SubType> values)
      where SubType : SuperType
    {
      foreach (SubType subType in values)
        yield return (SuperType) subType;
    }

    internal static TElement[] Prepend<TElement>(TElement[] args, TElement arg)
    {
      TElement[] elementArray = new TElement[args.Length + 1];
      elementArray[0] = arg;
      for (int index = 0; index < args.Length; ++index)
        elementArray[index + 1] = args[index];
      return elementArray;
    }

    internal static TNode BuildBalancedTreeInPlace<TNode>(
      IList<TNode> nodes,
      Func<TNode, TNode, TNode> combinator)
    {
      if (nodes.Count == 1)
        return nodes[0];
      if (nodes.Count == 2)
        return combinator(nodes[0], nodes[1]);
      for (int count = nodes.Count; count != 1; count /= 2)
      {
        bool flag = (count & 1) == 1;
        if (flag)
          --count;
        int num = 0;
        for (int index = 0; index < count; index += 2)
          nodes[num++] = combinator(nodes[index], nodes[index + 1]);
        if (flag)
        {
          int index = num - 1;
          nodes[index] = combinator(nodes[index], nodes[count]);
        }
      }
      return nodes[0];
    }

    internal static IEnumerable<TNode> GetLeafNodes<TNode>(
      TNode root,
      Func<TNode, bool> isLeaf,
      Func<TNode, IEnumerable<TNode>> getImmediateSubNodes)
    {
      Stack<TNode> nodes = new Stack<TNode>();
      nodes.Push(root);
      while (nodes.Count > 0)
      {
        TNode current = nodes.Pop();
        if (isLeaf(current))
        {
          yield return current;
        }
        else
        {
          List<TNode> nodeList = new List<TNode>(getImmediateSubNodes(current));
          for (int index = nodeList.Count - 1; index > -1; --index)
            nodes.Push(nodeList[index]);
        }
      }
    }
  }
}
