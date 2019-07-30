// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.AugmentedTableNode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal sealed class AugmentedTableNode : AugmentedNode
  {
    private readonly Table m_table;
    private AugmentedTableNode m_replacementTable;
    private int m_newLocationId;

    internal AugmentedTableNode(int id, Node node)
      : base(id, node)
    {
      this.m_table = ((ScanTableBaseOp) node.Op).Table;
      this.LastVisibleId = id;
      this.m_replacementTable = this;
      this.m_newLocationId = id;
    }

    internal Table Table
    {
      get
      {
        return this.m_table;
      }
    }

    internal int LastVisibleId { get; set; }

    internal bool IsEliminated
    {
      get
      {
        return this.m_replacementTable != this;
      }
    }

    internal AugmentedTableNode ReplacementTable
    {
      get
      {
        return this.m_replacementTable;
      }
      set
      {
        this.m_replacementTable = value;
      }
    }

    internal int NewLocationId
    {
      get
      {
        return this.m_newLocationId;
      }
      set
      {
        this.m_newLocationId = value;
      }
    }

    internal bool IsMoved
    {
      get
      {
        return this.m_newLocationId != this.Id;
      }
    }

    internal VarVec NullableColumns { get; set; }
  }
}
