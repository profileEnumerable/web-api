// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.NestBaseOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class NestBaseOp : PhysicalOp
  {
    private readonly List<SortKey> m_prefixSortKeys;
    private readonly VarVec m_outputs;
    private readonly List<System.Data.Entity.Core.Query.InternalTrees.CollectionInfo> m_collectionInfoList;

    internal List<SortKey> PrefixSortKeys
    {
      get
      {
        return this.m_prefixSortKeys;
      }
    }

    internal VarVec Outputs
    {
      get
      {
        return this.m_outputs;
      }
    }

    internal List<System.Data.Entity.Core.Query.InternalTrees.CollectionInfo> CollectionInfo
    {
      get
      {
        return this.m_collectionInfoList;
      }
    }

    internal NestBaseOp(
      OpType opType,
      List<SortKey> prefixSortKeys,
      VarVec outputVars,
      List<System.Data.Entity.Core.Query.InternalTrees.CollectionInfo> collectionInfoList)
      : base(opType)
    {
      this.m_outputs = outputVars;
      this.m_collectionInfoList = collectionInfoList;
      this.m_prefixSortKeys = prefixSortKeys;
    }
  }
}
