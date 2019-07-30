// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.BasicViewGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Validation;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal class BasicViewGenerator : InternalBase
  {
    private readonly MemberProjectionIndex m_projectedSlotMap;
    private readonly List<LeftCellWrapper> m_usedCells;
    private readonly FragmentQuery m_activeDomain;
    private readonly ViewgenContext m_viewgenContext;
    private readonly ErrorLog m_errorLog;
    private readonly ConfigViewGenerator m_config;
    private readonly MemberDomainMap m_domainMap;

    internal BasicViewGenerator(
      MemberProjectionIndex projectedSlotMap,
      List<LeftCellWrapper> usedCells,
      FragmentQuery activeDomain,
      ViewgenContext context,
      MemberDomainMap domainMap,
      ErrorLog errorLog,
      ConfigViewGenerator config)
    {
      this.m_projectedSlotMap = projectedSlotMap;
      this.m_usedCells = usedCells;
      this.m_viewgenContext = context;
      this.m_activeDomain = activeDomain;
      this.m_errorLog = errorLog;
      this.m_config = config;
      this.m_domainMap = domainMap;
    }

    private FragmentQueryProcessor LeftQP
    {
      get
      {
        return this.m_viewgenContext.LeftFragmentQP;
      }
    }

    internal CellTreeNode CreateViewExpression()
    {
      OpCellTreeNode opCellTreeNode = new OpCellTreeNode(this.m_viewgenContext, CellTreeOpType.FOJ);
      foreach (LeftCellWrapper usedCell in this.m_usedCells)
      {
        LeafCellTreeNode leafCellTreeNode = new LeafCellTreeNode(this.m_viewgenContext, usedCell);
        opCellTreeNode.Add((CellTreeNode) leafCellTreeNode);
      }
      CellTreeNode rootNode = this.IsolateByOperator(this.IsolateByOperator(this.IsolateByOperator(this.IsolateUnions(this.GroupByRightExtent((CellTreeNode) opCellTreeNode)), CellTreeOpType.Union), CellTreeOpType.IJ), CellTreeOpType.LOJ);
      if (this.m_viewgenContext.ViewTarget == ViewTarget.QueryView)
        rootNode = this.ConvertUnionsToNormalizedLOJs(rootNode);
      return rootNode;
    }

    internal CellTreeNode GroupByRightExtent(CellTreeNode rootNode)
    {
      KeyToListMap<EntitySetBase, LeafCellTreeNode> keyToListMap = new KeyToListMap<EntitySetBase, LeafCellTreeNode>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
      foreach (LeafCellTreeNode child in rootNode.Children)
      {
        EntitySetBase extent = child.LeftCellWrapper.RightCellQuery.Extent;
        keyToListMap.Add(extent, child);
      }
      OpCellTreeNode opCellTreeNode1 = new OpCellTreeNode(this.m_viewgenContext, CellTreeOpType.FOJ);
      foreach (EntitySetBase key in keyToListMap.Keys)
      {
        OpCellTreeNode opCellTreeNode2 = new OpCellTreeNode(this.m_viewgenContext, CellTreeOpType.FOJ);
        foreach (LeafCellTreeNode leafCellTreeNode in keyToListMap.ListForKey(key))
          opCellTreeNode2.Add((CellTreeNode) leafCellTreeNode);
        opCellTreeNode1.Add((CellTreeNode) opCellTreeNode2);
      }
      return opCellTreeNode1.Flatten();
    }

    private CellTreeNode IsolateUnions(CellTreeNode rootNode)
    {
      if (rootNode.Children.Count <= 1)
        return rootNode;
      for (int index = 0; index < rootNode.Children.Count; ++index)
        rootNode.Children[index] = this.IsolateUnions(rootNode.Children[index]);
      OpCellTreeNode opCellTreeNode1 = new OpCellTreeNode(this.m_viewgenContext, CellTreeOpType.Union);
      ModifiableIteratorCollection<CellTreeNode> iteratorCollection = new ModifiableIteratorCollection<CellTreeNode>((IEnumerable<CellTreeNode>) rootNode.Children);
      while (!iteratorCollection.IsEmpty)
      {
        OpCellTreeNode opCellTreeNode2 = new OpCellTreeNode(this.m_viewgenContext, CellTreeOpType.FOJ);
        CellTreeNode child = iteratorCollection.RemoveOneElement();
        opCellTreeNode2.Add(child);
        foreach (CellTreeNode element in iteratorCollection.Elements())
        {
          if (!this.IsDisjoint((CellTreeNode) opCellTreeNode2, element))
          {
            opCellTreeNode2.Add(element);
            iteratorCollection.RemoveCurrentOfIterator();
            iteratorCollection.ResetIterator();
          }
        }
        opCellTreeNode1.Add((CellTreeNode) opCellTreeNode2);
      }
      return opCellTreeNode1.Flatten();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private CellTreeNode ConvertUnionsToNormalizedLOJs(CellTreeNode rootNode)
    {
      for (int index = 0; index < rootNode.Children.Count; ++index)
        rootNode.Children[index] = this.ConvertUnionsToNormalizedLOJs(rootNode.Children[index]);
      if (rootNode.OpType != CellTreeOpType.LOJ || rootNode.Children.Count < 2)
        return rootNode;
      OpCellTreeNode opCellTreeNode1 = new OpCellTreeNode(this.m_viewgenContext, rootNode.OpType);
      List<CellTreeNode> cellTreeNodeList = new List<CellTreeNode>();
      OpCellTreeNode opCellTreeNode2 = (OpCellTreeNode) null;
      HashSet<CellTreeNode> cellTreeNodeSet = (HashSet<CellTreeNode>) null;
      if (rootNode.Children[0].OpType == CellTreeOpType.IJ)
      {
        opCellTreeNode2 = new OpCellTreeNode(this.m_viewgenContext, rootNode.Children[0].OpType);
        opCellTreeNode1.Add((CellTreeNode) opCellTreeNode2);
        cellTreeNodeList.AddRange((IEnumerable<CellTreeNode>) rootNode.Children[0].Children);
        cellTreeNodeSet = new HashSet<CellTreeNode>((IEnumerable<CellTreeNode>) rootNode.Children[0].Children);
      }
      else
        opCellTreeNode1.Add(rootNode.Children[0]);
      foreach (CellTreeNode cellTreeNode in rootNode.Children.Skip<CellTreeNode>(1))
      {
        OpCellTreeNode opCellTreeNode3 = cellTreeNode as OpCellTreeNode;
        if (opCellTreeNode3 != null && opCellTreeNode3.OpType == CellTreeOpType.Union)
          cellTreeNodeList.AddRange((IEnumerable<CellTreeNode>) opCellTreeNode3.Children);
        else
          cellTreeNodeList.Add(cellTreeNode);
      }
      KeyToListMap<EntitySet, LeafCellTreeNode> keyToListMap1 = new KeyToListMap<EntitySet, LeafCellTreeNode>((IEqualityComparer<EntitySet>) EqualityComparer<EntitySet>.Default);
      foreach (CellTreeNode child in cellTreeNodeList)
      {
        LeafCellTreeNode leaf = child as LeafCellTreeNode;
        if (leaf != null)
        {
          EntitySetBase leafNodeTable = (EntitySetBase) BasicViewGenerator.GetLeafNodeTable(leaf);
          if (leafNodeTable != null)
            keyToListMap1.Add((EntitySet) leafNodeTable, leaf);
        }
        else if (cellTreeNodeSet != null && cellTreeNodeSet.Contains(child))
          opCellTreeNode2.Add(child);
        else
          opCellTreeNode1.Add(child);
      }
      foreach (KeyValuePair<EntitySet, List<LeafCellTreeNode>> keyValuePair in keyToListMap1.KeyValuePairs.Where<KeyValuePair<EntitySet, List<LeafCellTreeNode>>>((Func<KeyValuePair<EntitySet, List<LeafCellTreeNode>>, bool>) (m => m.Value.Count > 1)).ToArray<KeyValuePair<EntitySet, List<LeafCellTreeNode>>>())
      {
        keyToListMap1.RemoveKey(keyValuePair.Key);
        foreach (LeafCellTreeNode leafCellTreeNode in keyValuePair.Value)
        {
          if (cellTreeNodeSet != null && cellTreeNodeSet.Contains((CellTreeNode) leafCellTreeNode))
            opCellTreeNode2.Add((CellTreeNode) leafCellTreeNode);
          else
            opCellTreeNode1.Add((CellTreeNode) leafCellTreeNode);
        }
      }
      KeyToListMap<EntitySet, EntitySet> keyToListMap2 = new KeyToListMap<EntitySet, EntitySet>((IEqualityComparer<EntitySet>) EqualityComparer<EntitySet>.Default);
      Dictionary<EntitySet, OpCellTreeNode> dictionary = new Dictionary<EntitySet, OpCellTreeNode>((IEqualityComparer<EntitySet>) EqualityComparer<EntitySet>.Default);
      foreach (KeyValuePair<EntitySet, List<LeafCellTreeNode>> keyValuePair in keyToListMap1.KeyValuePairs)
      {
        EntitySet key = keyValuePair.Key;
        foreach (EntitySet fkOverPkDependent in BasicViewGenerator.GetFKOverPKDependents(key))
        {
          ReadOnlyCollection<LeafCellTreeNode> valueCollection;
          if (keyToListMap1.TryGetListForKey(fkOverPkDependent, out valueCollection) && (cellTreeNodeSet == null || !cellTreeNodeSet.Contains((CellTreeNode) valueCollection.Single<LeafCellTreeNode>())))
            keyToListMap2.Add(key, fkOverPkDependent);
        }
        OpCellTreeNode opCellTreeNode3 = new OpCellTreeNode(this.m_viewgenContext, CellTreeOpType.LOJ);
        opCellTreeNode3.Add((CellTreeNode) keyValuePair.Value.Single<LeafCellTreeNode>());
        dictionary.Add(key, opCellTreeNode3);
      }
      Dictionary<EntitySet, EntitySet> nestedExtents = new Dictionary<EntitySet, EntitySet>((IEqualityComparer<EntitySet>) EqualityComparer<EntitySet>.Default);
      foreach (KeyValuePair<EntitySet, List<EntitySet>> keyValuePair in keyToListMap2.KeyValuePairs)
      {
        EntitySet key = keyValuePair.Key;
        foreach (EntitySet entitySet in keyValuePair.Value)
        {
          OpCellTreeNode opCellTreeNode3;
          if (dictionary.TryGetValue(entitySet, out opCellTreeNode3) && !nestedExtents.ContainsKey(entitySet) && !BasicViewGenerator.CheckLOJCycle(entitySet, key, nestedExtents))
          {
            dictionary[keyValuePair.Key].Add((CellTreeNode) opCellTreeNode3);
            nestedExtents.Add(entitySet, key);
          }
        }
      }
      foreach (KeyValuePair<EntitySet, OpCellTreeNode> keyValuePair in dictionary)
      {
        if (!nestedExtents.ContainsKey(keyValuePair.Key))
        {
          OpCellTreeNode opCellTreeNode3 = keyValuePair.Value;
          if (cellTreeNodeSet != null && cellTreeNodeSet.Contains(opCellTreeNode3.Children[0]))
            opCellTreeNode2.Add((CellTreeNode) opCellTreeNode3);
          else
            opCellTreeNode1.Add((CellTreeNode) opCellTreeNode3);
        }
      }
      return opCellTreeNode1.Flatten();
    }

    private static IEnumerable<EntitySet> GetFKOverPKDependents(
      EntitySet principal)
    {
      foreach (Tuple<AssociationSet, ReferentialConstraint> foreignKeyPrincipal in principal.ForeignKeyPrincipals)
      {
        Tuple<AssociationSet, ReferentialConstraint> pkFkInfo = foreignKeyPrincipal;
        ReadOnlyMetadataCollection<EdmMember> pkColumns = pkFkInfo.Item2.ToRole.GetEntityType().KeyMembers;
        ReadOnlyMetadataCollection<EdmProperty> fkColumns = pkFkInfo.Item2.ToProperties;
        if (pkColumns.Count == fkColumns.Count)
        {
          int i = 0;
          while (i < pkColumns.Count && pkColumns[i].EdmEquals((MetadataItem) fkColumns[i]))
            ++i;
          if (i == pkColumns.Count)
            yield return pkFkInfo.Item1.AssociationSetEnds.Where<AssociationSetEnd>((Func<AssociationSetEnd, bool>) (ase => ase.Name == pkFkInfo.Item2.ToRole.Name)).Single<AssociationSetEnd>().EntitySet;
        }
      }
    }

    private static EntitySet GetLeafNodeTable(LeafCellTreeNode leaf)
    {
      return leaf.LeftCellWrapper.RightCellQuery.Extent as EntitySet;
    }

    private static bool CheckLOJCycle(
      EntitySet child,
      EntitySet parent,
      Dictionary<EntitySet, EntitySet> nestedExtents)
    {
      while (!EqualityComparer<EntitySet>.Default.Equals(parent, child))
      {
        if (!nestedExtents.TryGetValue(parent, out parent))
          return false;
      }
      return true;
    }

    internal CellTreeNode IsolateByOperator(
      CellTreeNode rootNode,
      CellTreeOpType opTypeToIsolate)
    {
      List<CellTreeNode> children = rootNode.Children;
      if (children.Count <= 1)
        return rootNode;
      for (int index = 0; index < children.Count; ++index)
        children[index] = this.IsolateByOperator(children[index], opTypeToIsolate);
      if (rootNode.OpType != CellTreeOpType.FOJ && rootNode.OpType != CellTreeOpType.LOJ || rootNode.OpType == opTypeToIsolate)
        return rootNode;
      OpCellTreeNode opCellTreeNode = new OpCellTreeNode(this.m_viewgenContext, rootNode.OpType);
      ModifiableIteratorCollection<CellTreeNode> iteratorCollection = new ModifiableIteratorCollection<CellTreeNode>((IEnumerable<CellTreeNode>) children);
      while (!iteratorCollection.IsEmpty)
      {
        OpCellTreeNode groupNode = new OpCellTreeNode(this.m_viewgenContext, opTypeToIsolate);
        CellTreeNode child = iteratorCollection.RemoveOneElement();
        groupNode.Add(child);
        foreach (CellTreeNode element in iteratorCollection.Elements())
        {
          if (this.TryAddChildToGroup(opTypeToIsolate, element, groupNode))
          {
            iteratorCollection.RemoveCurrentOfIterator();
            if (opTypeToIsolate == CellTreeOpType.LOJ)
              iteratorCollection.ResetIterator();
          }
        }
        opCellTreeNode.Add((CellTreeNode) groupNode);
      }
      return opCellTreeNode.Flatten();
    }

    private bool TryAddChildToGroup(
      CellTreeOpType opTypeToIsolate,
      CellTreeNode childNode,
      OpCellTreeNode groupNode)
    {
      switch (opTypeToIsolate)
      {
        case CellTreeOpType.Union:
          if (this.IsDisjoint(childNode, (CellTreeNode) groupNode))
          {
            groupNode.Add(childNode);
            return true;
          }
          break;
        case CellTreeOpType.LOJ:
          if (this.IsContainedIn(childNode, (CellTreeNode) groupNode))
          {
            groupNode.Add(childNode);
            return true;
          }
          if (this.IsContainedIn((CellTreeNode) groupNode, childNode))
          {
            groupNode.AddFirst(childNode);
            return true;
          }
          break;
        case CellTreeOpType.IJ:
          if (this.IsEquivalentTo(childNode, (CellTreeNode) groupNode))
          {
            groupNode.Add(childNode);
            return true;
          }
          break;
      }
      return false;
    }

    private bool IsDisjoint(CellTreeNode n1, CellTreeNode n2)
    {
      int viewTarget = (int) this.m_viewgenContext.ViewTarget;
      bool flag = this.LeftQP.IsDisjointFrom(n1.LeftFragmentQuery, n2.LeftFragmentQuery);
      if (flag && this.m_viewgenContext.ViewTarget == ViewTarget.QueryView)
        return true;
      bool rightFragmentQuery = new OpCellTreeNode(this.m_viewgenContext, CellTreeOpType.IJ, new CellTreeNode[2]
      {
        n1,
        n2
      }).IsEmptyRightFragmentQuery;
      if (this.m_viewgenContext.ViewTarget == ViewTarget.UpdateView && flag && !rightFragmentQuery)
      {
        if (ErrorPatternMatcher.FindMappingErrors(this.m_viewgenContext, this.m_domainMap, this.m_errorLog))
          return false;
        StringBuilder builder = new StringBuilder(Strings.Viewgen_RightSideNotDisjoint((object) this.m_viewgenContext.Extent.ToString()));
        builder.AppendLine();
        FragmentQuery query = this.LeftQP.Intersect(n1.RightFragmentQuery, n2.RightFragmentQuery);
        if (this.LeftQP.IsSatisfiable(query))
        {
          query.Condition.ExpensiveSimplify();
          RewritingValidator.EntityConfigurationToUserString(query.Condition, builder);
        }
        this.m_errorLog.AddEntry(new ErrorLog.Record(ViewGenErrorCode.DisjointConstraintViolation, builder.ToString(), (IEnumerable<LeftCellWrapper>) this.m_viewgenContext.AllWrappersForExtent, string.Empty));
        ExceptionHelpers.ThrowMappingException(this.m_errorLog, this.m_config);
        return false;
      }
      if (!flag)
        return rightFragmentQuery;
      return true;
    }

    private bool IsContainedIn(CellTreeNode n1, CellTreeNode n2)
    {
      if (this.LeftQP.IsContainedIn(this.LeftQP.Intersect(n1.LeftFragmentQuery, this.m_activeDomain), this.LeftQP.Intersect(n2.LeftFragmentQuery, this.m_activeDomain)))
        return true;
      return new OpCellTreeNode(this.m_viewgenContext, CellTreeOpType.LASJ, new CellTreeNode[2]
      {
        n1,
        n2
      }).IsEmptyRightFragmentQuery;
    }

    private bool IsEquivalentTo(CellTreeNode n1, CellTreeNode n2)
    {
      if (this.IsContainedIn(n1, n2))
        return this.IsContainedIn(n2, n1);
      return false;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      this.m_projectedSlotMap.ToCompactString(builder);
    }
  }
}
