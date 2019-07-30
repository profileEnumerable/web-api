// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.SortBaseOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class SortBaseOp : RelOp
  {
    private readonly List<SortKey> m_keys;

    internal SortBaseOp(OpType opType)
      : base(opType)
    {
    }

    internal SortBaseOp(OpType opType, List<SortKey> sortKeys)
      : this(opType)
    {
      this.m_keys = sortKeys;
    }

    internal List<SortKey> Keys
    {
      get
      {
        return this.m_keys;
      }
    }
  }
}
