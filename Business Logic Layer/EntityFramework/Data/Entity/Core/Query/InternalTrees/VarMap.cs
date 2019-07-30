// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.VarMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class VarMap : Dictionary<Var, Var>
  {
    internal VarMap GetReverseMap()
    {
      VarMap varMap = new VarMap();
      foreach (KeyValuePair<Var, Var> keyValuePair in (Dictionary<Var, Var>) this)
      {
        Var var;
        if (!varMap.TryGetValue(keyValuePair.Value, out var))
          varMap[keyValuePair.Value] = keyValuePair.Key;
      }
      return varMap;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = string.Empty;
      foreach (Var key in this.Keys)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1},{2})", (object) str, (object) key.Id, (object) this[key].Id);
        str = ",";
      }
      return stringBuilder.ToString();
    }
  }
}
