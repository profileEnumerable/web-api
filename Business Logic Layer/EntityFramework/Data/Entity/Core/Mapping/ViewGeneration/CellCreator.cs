// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CellCreator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal class CellCreator : InternalBase
  {
    private readonly EntityContainerMapping m_containerMapping;
    private int m_currentCellNumber;
    private readonly CqlIdentifiers m_identifiers;

    internal CellCreator(EntityContainerMapping containerMapping)
    {
      this.m_containerMapping = containerMapping;
      this.m_identifiers = new CqlIdentifiers();
    }

    internal CqlIdentifiers Identifiers
    {
      get
      {
        return this.m_identifiers;
      }
    }

    internal List<Cell> GenerateCells()
    {
      List<Cell> cells = new List<Cell>();
      this.ExtractCells(cells);
      this.ExpandCells(cells);
      this.m_identifiers.AddIdentifier(this.m_containerMapping.EdmEntityContainer.Name);
      this.m_identifiers.AddIdentifier(this.m_containerMapping.StorageEntityContainer.Name);
      foreach (Cell cell in cells)
        cell.GetIdentifiers(this.m_identifiers);
      return cells;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void ExpandCells(List<Cell> cells)
    {
      Set<MemberPath> set1 = new Set<MemberPath>();
      foreach (Cell cell1 in cells)
      {
        Cell cell = cell1;
        foreach (MemberPath element in cell.SQuery.GetProjectedMembers().Where<MemberPath>((Func<MemberPath, bool>) (member => CellCreator.IsBooleanMember(member))).Where<MemberPath>((Func<MemberPath, bool>) (boolMember => cell.SQuery.GetConjunctsFromWhereClause().Where<MemberRestriction>((Func<MemberRestriction, bool>) (restriction => restriction.Domain.Values.Contains<Constant>(Constant.NotNull))).Select<MemberRestriction, MemberPath>((Func<MemberRestriction, MemberPath>) (restriction => restriction.RestrictedMemberSlot.MemberPath)).Contains<MemberPath>(boolMember))))
          set1.Add(element);
      }
      Dictionary<MemberPath, Set<MemberPath>> dictionary = new Dictionary<MemberPath, Set<MemberPath>>();
      foreach (Cell cell1 in cells)
      {
        Cell cell = cell1;
        foreach (MemberPath index in set1)
        {
          IEnumerable<MemberPath> elements = cell.SQuery.GetProjectedPositions(index).Select<int, MemberPath>((Func<int, MemberPath>) (pos => ((MemberProjectedSlot) cell.CQuery.ProjectedSlotAt(pos)).MemberPath));
          Set<MemberPath> set2 = (Set<MemberPath>) null;
          if (!dictionary.TryGetValue(index, out set2))
          {
            set2 = new Set<MemberPath>();
            dictionary[index] = set2;
          }
          set2.AddRange(elements);
        }
      }
      foreach (Cell originalCell in cells.ToArray())
      {
        foreach (MemberPath memberToExpand1 in set1)
        {
          Set<MemberPath> set2 = dictionary[memberToExpand1];
          if (originalCell.SQuery.GetProjectedMembers().Contains<MemberPath>(memberToExpand1))
          {
            Cell result = (Cell) null;
            if (this.TryCreateAdditionalCellWithCondition(originalCell, memberToExpand1, true, ViewTarget.UpdateView, out result))
              cells.Add(result);
            if (this.TryCreateAdditionalCellWithCondition(originalCell, memberToExpand1, false, ViewTarget.UpdateView, out result))
              cells.Add(result);
          }
          else
          {
            foreach (MemberPath memberToExpand2 in originalCell.CQuery.GetProjectedMembers().Intersect<MemberPath>((IEnumerable<MemberPath>) set2))
            {
              Cell result = (Cell) null;
              if (this.TryCreateAdditionalCellWithCondition(originalCell, memberToExpand2, true, ViewTarget.QueryView, out result))
                cells.Add(result);
              if (this.TryCreateAdditionalCellWithCondition(originalCell, memberToExpand2, false, ViewTarget.QueryView, out result))
                cells.Add(result);
            }
          }
        }
      }
    }

    private bool TryCreateAdditionalCellWithCondition(
      Cell originalCell,
      MemberPath memberToExpand,
      bool conditionValue,
      ViewTarget viewTarget,
      out Cell result)
    {
      result = (Cell) null;
      MemberPath extentMemberPath1 = originalCell.GetLeftQuery(viewTarget).SourceExtentMemberPath;
      MemberPath extentMemberPath2 = originalCell.GetRightQuery(viewTarget).SourceExtentMemberPath;
      int slotNum1 = originalCell.GetLeftQuery(viewTarget).GetProjectedMembers().TakeWhile<MemberPath>((Func<MemberPath, bool>) (path => !path.Equals(memberToExpand))).Count<MemberPath>();
      MemberPath rightSidePath = ((MemberProjectedSlot) originalCell.GetRightQuery(viewTarget).ProjectedSlotAt(slotNum1)).MemberPath;
      List<ProjectedSlot> slots1 = new List<ProjectedSlot>();
      List<ProjectedSlot> slots2 = new List<ProjectedSlot>();
      ScalarConstant negatedCondition = new ScalarConstant((object) !conditionValue);
      if (originalCell.GetLeftQuery(viewTarget).Conditions.Where<MemberRestriction>((Func<MemberRestriction, bool>) (restriction => restriction.RestrictedMemberSlot.MemberPath.Equals(memberToExpand))).Where<MemberRestriction>((Func<MemberRestriction, bool>) (restriction => restriction.Domain.Values.Contains<Constant>((Constant) negatedCondition))).Any<MemberRestriction>() || originalCell.GetRightQuery(viewTarget).Conditions.Where<MemberRestriction>((Func<MemberRestriction, bool>) (restriction => restriction.RestrictedMemberSlot.MemberPath.Equals(rightSidePath))).Where<MemberRestriction>((Func<MemberRestriction, bool>) (restriction => restriction.Domain.Values.Contains<Constant>((Constant) negatedCondition))).Any<MemberRestriction>())
        return false;
      for (int slotNum2 = 0; slotNum2 < originalCell.GetLeftQuery(viewTarget).NumProjectedSlots; ++slotNum2)
        slots1.Add(originalCell.GetLeftQuery(viewTarget).ProjectedSlotAt(slotNum2));
      for (int slotNum2 = 0; slotNum2 < originalCell.GetRightQuery(viewTarget).NumProjectedSlots; ++slotNum2)
        slots2.Add(originalCell.GetRightQuery(viewTarget).ProjectedSlotAt(slotNum2));
      BoolExpression literal1 = BoolExpression.CreateLiteral((BoolLiteral) new ScalarRestriction(memberToExpand, (Constant) new ScalarConstant((object) conditionValue)), (MemberDomainMap) null);
      BoolExpression and1 = BoolExpression.CreateAnd(originalCell.GetLeftQuery(viewTarget).WhereClause, literal1);
      BoolExpression literal2 = BoolExpression.CreateLiteral((BoolLiteral) new ScalarRestriction(rightSidePath, (Constant) new ScalarConstant((object) conditionValue)), (MemberDomainMap) null);
      BoolExpression and2 = BoolExpression.CreateAnd(originalCell.GetRightQuery(viewTarget).WhereClause, literal2);
      CellQuery cellQuery1 = new CellQuery(slots2, and2, extentMemberPath2, originalCell.GetRightQuery(viewTarget).SelectDistinctFlag);
      CellQuery cellQuery2 = new CellQuery(slots1, and1, extentMemberPath1, originalCell.GetLeftQuery(viewTarget).SelectDistinctFlag);
      Cell cell = viewTarget != ViewTarget.UpdateView ? Cell.CreateCS(cellQuery2, cellQuery1, originalCell.CellLabel, this.m_currentCellNumber) : Cell.CreateCS(cellQuery1, cellQuery2, originalCell.CellLabel, this.m_currentCellNumber);
      ++this.m_currentCellNumber;
      result = cell;
      return true;
    }

    private void ExtractCells(List<Cell> cells)
    {
      foreach (EntitySetBaseMapping allSetMap in this.m_containerMapping.AllSetMaps)
      {
        foreach (TypeMapping typeMapping in allSetMap.TypeMappings)
        {
          EntityTypeMapping entityTypeMapping = typeMapping as EntityTypeMapping;
          Set<EdmType> allTypes = new Set<EdmType>();
          if (entityTypeMapping != null)
          {
            allTypes.AddRange((IEnumerable<EdmType>) entityTypeMapping.Types);
            foreach (EdmType isOfType in entityTypeMapping.IsOfTypes)
            {
              IEnumerable<EdmType> typeAndSubtypesOf = MetadataHelper.GetTypeAndSubtypesOf(isOfType, (ItemCollection) this.m_containerMapping.StorageMappingItemCollection.EdmItemCollection, false);
              allTypes.AddRange(typeAndSubtypesOf);
            }
          }
          EntitySetBase set = allSetMap.Set;
          foreach (MappingFragment mappingFragment in typeMapping.MappingFragments)
            this.ExtractCellsFromTableFragment(set, mappingFragment, allTypes, cells);
        }
      }
    }

    private void ExtractCellsFromTableFragment(
      EntitySetBase extent,
      MappingFragment fragmentMap,
      Set<EdmType> allTypes,
      List<Cell> cells)
    {
      MemberPath memberPath1 = new MemberPath(extent);
      BoolExpression literal = BoolExpression.True;
      List<ProjectedSlot> projectedSlotList1 = new List<ProjectedSlot>();
      if (allTypes.Count > 0)
        literal = BoolExpression.CreateLiteral((BoolLiteral) new TypeRestriction(memberPath1, (IEnumerable<EdmType>) allTypes), (MemberDomainMap) null);
      MemberPath memberPath2 = new MemberPath((EntitySetBase) fragmentMap.TableSet);
      BoolExpression sQueryWhereClause = BoolExpression.True;
      List<ProjectedSlot> projectedSlotList2 = new List<ProjectedSlot>();
      this.ExtractProperties((IEnumerable<PropertyMapping>) fragmentMap.AllProperties, memberPath1, projectedSlotList1, ref literal, memberPath2, projectedSlotList2, ref sQueryWhereClause);
      Cell cs = Cell.CreateCS(new CellQuery(projectedSlotList1, literal, memberPath1, CellQuery.SelectDistinct.No), new CellQuery(projectedSlotList2, sQueryWhereClause, memberPath2, fragmentMap.IsSQueryDistinct ? CellQuery.SelectDistinct.Yes : CellQuery.SelectDistinct.No), new CellLabel(fragmentMap), this.m_currentCellNumber);
      ++this.m_currentCellNumber;
      cells.Add(cs);
    }

    private void ExtractProperties(
      IEnumerable<PropertyMapping> properties,
      MemberPath cNode,
      List<ProjectedSlot> cSlots,
      ref BoolExpression cQueryWhereClause,
      MemberPath sRootExtent,
      List<ProjectedSlot> sSlots,
      ref BoolExpression sQueryWhereClause)
    {
      foreach (PropertyMapping property in properties)
      {
        ScalarPropertyMapping scalarPropertyMapping = property as ScalarPropertyMapping;
        ComplexPropertyMapping complexPropertyMapping = property as ComplexPropertyMapping;
        EndPropertyMapping endPropertyMapping = property as EndPropertyMapping;
        ConditionPropertyMapping conditionMap = property as ConditionPropertyMapping;
        if (scalarPropertyMapping != null)
        {
          MemberPath node1 = new MemberPath(cNode, (EdmMember) scalarPropertyMapping.Property);
          MemberPath node2 = new MemberPath(sRootExtent, (EdmMember) scalarPropertyMapping.Column);
          cSlots.Add((ProjectedSlot) new MemberProjectedSlot(node1));
          sSlots.Add((ProjectedSlot) new MemberProjectedSlot(node2));
        }
        if (complexPropertyMapping != null)
        {
          foreach (ComplexTypeMapping typeMapping in complexPropertyMapping.TypeMappings)
          {
            MemberPath memberPath = new MemberPath(cNode, (EdmMember) complexPropertyMapping.Property);
            Set<EdmType> set = new Set<EdmType>();
            IEnumerable<EdmType> elements = Helpers.AsSuperTypeList<ComplexType, EdmType>((IEnumerable<ComplexType>) typeMapping.Types);
            set.AddRange(elements);
            foreach (EdmType isOfType in typeMapping.IsOfTypes)
              set.AddRange(MetadataHelper.GetTypeAndSubtypesOf(isOfType, (ItemCollection) this.m_containerMapping.StorageMappingItemCollection.EdmItemCollection, false));
            BoolExpression literal = BoolExpression.CreateLiteral((BoolLiteral) new TypeRestriction(memberPath, (IEnumerable<EdmType>) set), (MemberDomainMap) null);
            cQueryWhereClause = BoolExpression.CreateAnd(cQueryWhereClause, literal);
            this.ExtractProperties((IEnumerable<PropertyMapping>) typeMapping.AllProperties, memberPath, cSlots, ref cQueryWhereClause, sRootExtent, sSlots, ref sQueryWhereClause);
          }
        }
        if (endPropertyMapping != null)
        {
          MemberPath cNode1 = new MemberPath(cNode, (EdmMember) endPropertyMapping.AssociationEnd);
          this.ExtractProperties((IEnumerable<PropertyMapping>) endPropertyMapping.PropertyMappings, cNode1, cSlots, ref cQueryWhereClause, sRootExtent, sSlots, ref sQueryWhereClause);
        }
        if (conditionMap != null)
        {
          if (conditionMap.Column != null)
          {
            BoolExpression conditionExpression = CellCreator.GetConditionExpression(sRootExtent, conditionMap);
            sQueryWhereClause = BoolExpression.CreateAnd(sQueryWhereClause, conditionExpression);
          }
          else
          {
            BoolExpression conditionExpression = CellCreator.GetConditionExpression(cNode, conditionMap);
            cQueryWhereClause = BoolExpression.CreateAnd(cQueryWhereClause, conditionExpression);
          }
        }
      }
    }

    private static BoolExpression GetConditionExpression(
      MemberPath member,
      ConditionPropertyMapping conditionMap)
    {
      EdmMember edmMember = conditionMap.Column != null ? (EdmMember) conditionMap.Column : (EdmMember) conditionMap.Property;
      MemberPath member1 = new MemberPath(member, edmMember);
      MemberRestriction memberRestriction;
      if (conditionMap.IsNull.HasValue)
      {
        Constant constant = conditionMap.IsNull.Value ? Constant.Null : Constant.NotNull;
        memberRestriction = !MetadataHelper.IsNonRefSimpleMember(edmMember) ? (MemberRestriction) new TypeRestriction(member1, constant) : (MemberRestriction) new ScalarRestriction(member1, constant);
      }
      else
        memberRestriction = (MemberRestriction) new ScalarRestriction(member1, (Constant) new ScalarConstant(conditionMap.Value));
      return BoolExpression.CreateLiteral((BoolLiteral) memberRestriction, (MemberDomainMap) null);
    }

    private static bool IsBooleanMember(MemberPath path)
    {
      PrimitiveType edmType = path.EdmType as PrimitiveType;
      if (edmType != null)
        return edmType.PrimitiveTypeKind == PrimitiveTypeKind.Boolean;
      return false;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append(nameof (CellCreator));
    }
  }
}
