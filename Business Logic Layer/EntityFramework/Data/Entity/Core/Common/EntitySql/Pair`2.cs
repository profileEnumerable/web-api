// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.Pair`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class Pair<L, R>
  {
    internal L Left;
    internal R Right;

    internal Pair(L left, R right)
    {
      this.Left = left;
      this.Right = right;
    }

    internal KeyValuePair<L, R> GetKVP()
    {
      return new KeyValuePair<L, R>(this.Left, this.Right);
    }
  }
}
