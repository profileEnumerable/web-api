// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.VarList
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  [DebuggerDisplay("{{{ToString()}}}")]
  internal class VarList : List<Var>
  {
    internal VarList()
    {
    }

    internal VarList(IEnumerable<Var> vars)
      : base(vars)
    {
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = string.Empty;
      foreach (Var var in (List<Var>) this)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) str, (object) var.Id);
        str = ",";
      }
      return stringBuilder.ToString();
    }
  }
}
