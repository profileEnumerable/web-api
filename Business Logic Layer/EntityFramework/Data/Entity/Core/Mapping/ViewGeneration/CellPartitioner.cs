// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CellPartitioner
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.Update.Internal;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Validation;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal class CellPartitioner : InternalBase
  {
    private readonly IEnumerable<Cell> m_cells;
    private readonly IEnumerable<ForeignConstraint> m_foreignKeyConstraints;

    internal CellPartitioner(
      IEnumerable<Cell> cells,
      IEnumerable<ForeignConstraint> foreignKeyConstraints)
    {
      this.m_foreignKeyConstraints = foreignKeyConstraints;
      this.m_cells = cells;
    }

    internal List<Set<Cell>> GroupRelatedCells()
    {
      UndirectedGraph<EntitySetBase> undirectedGraph = new UndirectedGraph<EntitySetBase>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
      Dictionary<EntitySetBase, Set<Cell>> extentToCell = new Dictionary<EntitySetBase, Set<Cell>>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
      foreach (Cell cell in this.m_cells)
      {
        EntitySetBase[] entitySetBaseArray = new EntitySetBase[2]
        {
          cell.CQuery.Extent,
          cell.SQuery.Extent
        };
        foreach (EntitySetBase index in entitySetBaseArray)
        {
          Set<Cell> set;
          if (!extentToCell.TryGetValue(index, out set))
            extentToCell[index] = set = new Set<Cell>();
          set.Add(cell);
          undirectedGraph.AddVertex(index);
        }
        undirectedGraph.AddEdge(cell.CQuery.Extent, cell.SQuery.Extent);
        AssociationSet extent = cell.CQuery.Extent as AssociationSet;
        if (extent != null)
        {
          foreach (AssociationSetEnd associationSetEnd in extent.AssociationSetEnds)
            undirectedGraph.AddEdge((EntitySetBase) associationSetEnd.EntitySet, (EntitySetBase) extent);
        }
      }
      foreach (ForeignConstraint foreignKeyConstraint in this.m_foreignKeyConstraints)
        undirectedGraph.AddEdge((EntitySetBase) foreignKeyConstraint.ChildTable, (EntitySetBase) foreignKeyConstraint.ParentTable);
      KeyToListMap<int, EntitySetBase> connectedComponents = undirectedGraph.GenerateConnectedComponents();
      List<Set<Cell>> setList = new List<Set<Cell>>();
      foreach (int key in connectedComponents.Keys)
      {
        IEnumerable<Set<Cell>> sets = connectedComponents.ListForKey(key).Select<EntitySetBase, Set<Cell>>((Func<EntitySetBase, Set<Cell>>) (e => extentToCell[e]));
        Set<Cell> set1 = new Set<Cell>();
        foreach (Set<Cell> set2 in sets)
          set1.AddRange((IEnumerable<Cell>) set2);
        setList.Add(set1);
      }
      return setList;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      Cell.CellsToBuilder(builder, this.m_cells);
    }
  }
}
