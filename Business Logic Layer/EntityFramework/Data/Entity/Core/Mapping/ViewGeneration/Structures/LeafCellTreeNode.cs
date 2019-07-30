// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.LeafCellTreeNode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class LeafCellTreeNode : CellTreeNode
  {
    internal static readonly IEqualityComparer<LeafCellTreeNode> EqualityComparer = (IEqualityComparer<LeafCellTreeNode>) new LeafCellTreeNode.LeafCellTreeNodeComparer();
    private readonly LeftCellWrapper m_cellWrapper;
    private readonly FragmentQuery m_rightFragmentQuery;

    internal LeafCellTreeNode(ViewgenContext context, LeftCellWrapper cellWrapper)
      : base(context)
    {
      this.m_cellWrapper = cellWrapper;
      this.m_rightFragmentQuery = FragmentQuery.Create(cellWrapper.OriginalCellNumberString, cellWrapper.CreateRoleBoolean(), cellWrapper.RightCellQuery);
    }

    internal LeafCellTreeNode(
      ViewgenContext context,
      LeftCellWrapper cellWrapper,
      FragmentQuery rightFragmentQuery)
      : base(context)
    {
      this.m_cellWrapper = cellWrapper;
      this.m_rightFragmentQuery = rightFragmentQuery;
    }

    internal LeftCellWrapper LeftCellWrapper
    {
      get
      {
        return this.m_cellWrapper;
      }
    }

    internal override MemberDomainMap RightDomainMap
    {
      get
      {
        return this.m_cellWrapper.RightDomainMap;
      }
    }

    internal override FragmentQuery LeftFragmentQuery
    {
      get
      {
        return this.m_cellWrapper.FragmentQuery;
      }
    }

    internal override FragmentQuery RightFragmentQuery
    {
      get
      {
        return this.m_rightFragmentQuery;
      }
    }

    internal override Set<MemberPath> Attributes
    {
      get
      {
        return this.m_cellWrapper.Attributes;
      }
    }

    internal override List<CellTreeNode> Children
    {
      get
      {
        return new List<CellTreeNode>();
      }
    }

    internal override CellTreeOpType OpType
    {
      get
      {
        return CellTreeOpType.Leaf;
      }
    }

    internal override int NumProjectedSlots
    {
      get
      {
        return this.LeftCellWrapper.RightCellQuery.NumProjectedSlots;
      }
    }

    internal override int NumBoolSlots
    {
      get
      {
        return this.LeftCellWrapper.RightCellQuery.NumBoolVars;
      }
    }

    internal override TOutput Accept<TInput, TOutput>(
      CellTreeNode.CellTreeVisitor<TInput, TOutput> visitor,
      TInput param)
    {
      return visitor.VisitLeaf(this, param);
    }

    internal override TOutput Accept<TInput, TOutput>(
      CellTreeNode.SimpleCellTreeVisitor<TInput, TOutput> visitor,
      TInput param)
    {
      return visitor.VisitLeaf(this, param);
    }

    internal override bool IsProjectedSlot(int slot)
    {
      CellQuery rightCellQuery = this.LeftCellWrapper.RightCellQuery;
      if (this.IsBoolSlot(slot))
        return rightCellQuery.GetBoolVar(this.SlotToBoolIndex(slot)) != null;
      return rightCellQuery.ProjectedSlotAt(slot) != null;
    }

    internal override CqlBlock ToCqlBlock(
      bool[] requiredSlots,
      CqlIdentifiers identifiers,
      ref int blockAliasNum,
      ref List<WithRelationship> withRelationships)
    {
      int length = requiredSlots.Length;
      CellQuery rightCellQuery = this.LeftCellWrapper.RightCellQuery;
      SlotInfo[] slotInfoArray = new SlotInfo[length];
      for (int index = 0; index < rightCellQuery.NumProjectedSlots; ++index)
      {
        ProjectedSlot slotValue = rightCellQuery.ProjectedSlotAt(index);
        if (requiredSlots[index] && slotValue == null)
        {
          ConstantProjectedSlot slot = new ConstantProjectedSlot(Domain.GetDefaultValueForMemberPath(this.ProjectedSlotMap[index], (IEnumerable<LeftCellWrapper>) this.GetLeaves(), this.ViewgenContext.Config));
          rightCellQuery.FixMissingSlotAsDefaultConstant(index, slot);
          slotValue = (ProjectedSlot) slot;
        }
        SlotInfo slotInfo = new SlotInfo(requiredSlots[index], slotValue != null, slotValue, this.ProjectedSlotMap[index]);
        slotInfoArray[index] = slotInfo;
      }
      for (int index = 0; index < rightCellQuery.NumBoolVars; ++index)
      {
        BoolExpression boolVar = rightCellQuery.GetBoolVar(index);
        BooleanProjectedSlot booleanProjectedSlot = boolVar == null ? new BooleanProjectedSlot(BoolExpression.False, identifiers, index) : new BooleanProjectedSlot(boolVar, identifiers, index);
        int slot = this.BoolIndexToSlot(index);
        SlotInfo slotInfo = new SlotInfo(requiredSlots[slot], boolVar != null, (ProjectedSlot) booleanProjectedSlot, (MemberPath) null);
        slotInfoArray[slot] = slotInfo;
      }
      IEnumerable<SlotInfo> source = (IEnumerable<SlotInfo>) slotInfoArray;
      if (rightCellQuery.Extent.EntityContainer.DataSpace == DataSpace.SSpace && this.m_cellWrapper.LeftExtent.BuiltInTypeKind == BuiltInTypeKind.EntitySet)
      {
        IEnumerable<AssociationSetMapping> relationshipSetMappingsFor = this.ViewgenContext.EntityContainerMapping.GetRelationshipSetMappingsFor(this.m_cellWrapper.LeftExtent, rightCellQuery.Extent);
        List<SlotInfo> foreignKeySlots = new List<SlotInfo>();
        foreach (AssociationSetMapping colocatedAssociationSetMap in relationshipSetMappingsFor)
        {
          WithRelationship withRelationship;
          if (LeafCellTreeNode.TryGetWithRelationship(colocatedAssociationSetMap, this.m_cellWrapper.LeftExtent, rightCellQuery.SourceExtentMemberPath, ref foreignKeySlots, out withRelationship))
          {
            withRelationships.Add(withRelationship);
            source = ((IEnumerable<SlotInfo>) slotInfoArray).Concat<SlotInfo>((IEnumerable<SlotInfo>) foreignKeySlots);
          }
        }
      }
      return (CqlBlock) new ExtentCqlBlock(rightCellQuery.Extent, rightCellQuery.SelectDistinctFlag, source.ToArray<SlotInfo>(), rightCellQuery.WhereClause, identifiers, ++blockAliasNum);
    }

    private static bool TryGetWithRelationship(
      AssociationSetMapping colocatedAssociationSetMap,
      EntitySetBase thisExtent,
      MemberPath sRootNode,
      ref List<SlotInfo> foreignKeySlots,
      out WithRelationship withRelationship)
    {
      withRelationship = (WithRelationship) null;
      EndPropertyMapping fromAssocitionMap = LeafCellTreeNode.GetForeignKeyEndMapFromAssocitionMap(colocatedAssociationSetMap);
      if (fromAssocitionMap == null || fromAssocitionMap.AssociationEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many)
        return false;
      AssociationEndMember associationEnd = fromAssocitionMap.AssociationEnd;
      AssociationEndMember otherAssociationEnd = MetadataHelper.GetOtherAssociationEnd(associationEnd);
      EntityType elementType1 = (EntityType) ((RefType) associationEnd.TypeUsage.EdmType).ElementType;
      EntityType elementType2 = (EntityType) ((RefType) otherAssociationEnd.TypeUsage.EdmType).ElementType;
      AssociationSet set = (AssociationSet) colocatedAssociationSetMap.Set;
      MemberPath prefix = new MemberPath((EntitySetBase) set, (EdmMember) associationEnd);
      IEnumerable<ScalarPropertyMapping> source = fromAssocitionMap.PropertyMappings.Cast<ScalarPropertyMapping>();
      List<MemberPath> memberPathList = new List<MemberPath>();
      foreach (EdmProperty keyMember in elementType1.KeyMembers)
      {
        EdmProperty edmProperty = keyMember;
        ScalarPropertyMapping scalarPropertyMapping = source.Where<ScalarPropertyMapping>((Func<ScalarPropertyMapping, bool>) (propMap => propMap.Property.Equals((object) edmProperty))).First<ScalarPropertyMapping>();
        MemberProjectedSlot memberProjectedSlot = new MemberProjectedSlot(new MemberPath(sRootNode, (EdmMember) scalarPropertyMapping.Column));
        MemberPath outputMember = new MemberPath(prefix, (EdmMember) edmProperty);
        memberPathList.Add(outputMember);
        foreignKeySlots.Add(new SlotInfo(true, true, (ProjectedSlot) memberProjectedSlot, outputMember));
      }
      if (!thisExtent.ElementType.IsAssignableFrom((EdmType) elementType2))
        return false;
      withRelationship = new WithRelationship(set, otherAssociationEnd, elementType2, associationEnd, elementType1, (IEnumerable<MemberPath>) memberPathList);
      return true;
    }

    private static EndPropertyMapping GetForeignKeyEndMapFromAssocitionMap(
      AssociationSetMapping colocatedAssociationSetMap)
    {
      MappingFragment mappingFragment = colocatedAssociationSetMap.TypeMappings.First<TypeMapping>().MappingFragments.First<MappingFragment>();
      IEnumerable<EdmMember> keyMembers = (IEnumerable<EdmMember>) colocatedAssociationSetMap.StoreEntitySet.ElementType.KeyMembers;
      foreach (EndPropertyMapping propertyMapping in mappingFragment.PropertyMappings)
      {
        EndPropertyMapping endMap = propertyMapping;
        if (endMap.StoreProperties.SequenceEqual<EdmMember>(keyMembers, (IEqualityComparer<EdmMember>) System.Collections.Generic.EqualityComparer<EdmMember>.Default))
          return mappingFragment.PropertyMappings.OfType<EndPropertyMapping>().Where<EndPropertyMapping>((Func<EndPropertyMapping, bool>) (eMap => !eMap.Equals((object) endMap))).First<EndPropertyMapping>();
      }
      return (EndPropertyMapping) null;
    }

    internal override void ToCompactString(StringBuilder stringBuilder)
    {
      this.m_cellWrapper.ToCompactString(stringBuilder);
    }

    private class LeafCellTreeNodeComparer : IEqualityComparer<LeafCellTreeNode>
    {
      public bool Equals(LeafCellTreeNode left, LeafCellTreeNode right)
      {
        if (object.ReferenceEquals((object) left, (object) right))
          return true;
        if (left == null || right == null)
          return false;
        return left.m_cellWrapper.Equals((object) right.m_cellWrapper);
      }

      public int GetHashCode(LeafCellTreeNode node)
      {
        return node.m_cellWrapper.GetHashCode();
      }
    }
  }
}
