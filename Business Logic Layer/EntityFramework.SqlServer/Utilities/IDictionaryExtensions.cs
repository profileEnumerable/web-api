// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.IDictionaryExtensions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;

namespace System.Data.Entity.SqlServer.Utilities
{
  internal static class IDictionaryExtensions
  {
    internal static void Add<TKey, TValue>(
      this IDictionary<TKey, IList<TValue>> map,
      TKey key,
      TValue value)
    {
      IList<TValue> objList;
      if (!map.TryGetValue(key, out objList))
      {
        objList = (IList<TValue>) new List<TValue>();
        map[key] = objList;
      }
      objList.Add(value);
    }
  }
}
