// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Validation.ForeignConstraint
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Validation
{
  internal class ForeignConstraint : InternalBase
  {
    private readonly AssociationSet m_fKeySet;
    private readonly EntitySet m_parentTable;
    private readonly EntitySet m_childTable;
    private readonly List<MemberPath> m_parentColumns;
    private readonly List<MemberPath> m_childColumns;

    internal ForeignConstraint(
      AssociationSet i_fkeySet,
      EntitySet i_parentTable,
      EntitySet i_childTable,
      ReadOnlyMetadataCollection<EdmProperty> i_parentColumns,
      ReadOnlyMetadataCollection<EdmProperty> i_childColumns)
    {
      this.m_fKeySet = i_fkeySet;
      this.m_parentTable = i_parentTable;
      this.m_childTable = i_childTable;
      this.m_childColumns = new List<MemberPath>();
      foreach (EdmMember iChildColumn in i_childColumns)
        this.m_childColumns.Add(new MemberPath((EntitySetBase) this.m_childTable, iChildColumn));
      this.m_parentColumns = new List<MemberPath>();
      foreach (EdmMember iParentColumn in i_parentColumns)
        this.m_parentColumns.Add(new MemberPath((EntitySetBase) this.m_parentTable, iParentColumn));
    }

    internal EntitySet ParentTable
    {
      get
      {
        return this.m_parentTable;
      }
    }

    internal EntitySet ChildTable
    {
      get
      {
        return this.m_childTable;
      }
    }

    internal IEnumerable<MemberPath> ChildColumns
    {
      get
      {
        return (IEnumerable<MemberPath>) this.m_childColumns;
      }
    }

    internal IEnumerable<MemberPath> ParentColumns
    {
      get
      {
        return (IEnumerable<MemberPath>) this.m_parentColumns;
      }
    }

    internal static List<ForeignConstraint> GetForeignConstraints(
      EntityContainer container)
    {
      List<ForeignConstraint> foreignConstraintList = new List<ForeignConstraint>();
      foreach (EntitySetBase baseEntitySet in container.BaseEntitySets)
      {
        AssociationSet i_fkeySet = baseEntitySet as AssociationSet;
        if (i_fkeySet != null)
        {
          Dictionary<string, EntitySet> dictionary = new Dictionary<string, EntitySet>();
          foreach (AssociationSetEnd associationSetEnd in i_fkeySet.AssociationSetEnds)
            dictionary.Add(associationSetEnd.Name, associationSetEnd.EntitySet);
          foreach (ReferentialConstraint referentialConstraint in i_fkeySet.ElementType.ReferentialConstraints)
          {
            EntitySet i_parentTable = dictionary[referentialConstraint.FromRole.Name];
            EntitySet i_childTable = dictionary[referentialConstraint.ToRole.Name];
            ForeignConstraint foreignConstraint = new ForeignConstraint(i_fkeySet, i_parentTable, i_childTable, referentialConstraint.FromProperties, referentialConstraint.ToProperties);
            foreignConstraintList.Add(foreignConstraint);
          }
        }
      }
      return foreignConstraintList;
    }

    internal void CheckConstraint(
      Set<Cell> cells,
      QueryRewriter childRewriter,
      QueryRewriter parentRewriter,
      ErrorLog errorLog,
      ConfigViewGenerator config)
    {
      if (!this.IsConstraintRelevantForCells((IEnumerable<Cell>) cells))
        return;
      if (config.IsNormalTracing)
      {
        Trace.WriteLine(string.Empty);
        Trace.WriteLine(string.Empty);
        Trace.Write("Checking: ");
        Trace.WriteLine((object) this);
      }
      if (childRewriter == null && parentRewriter == null)
        return;
      if (childRewriter == null)
      {
        ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.ForeignKeyMissingTableMapping, Strings.ViewGen_Foreign_Key_Missing_Table_Mapping((object) this.ToUserString(), (object) this.ChildTable.Name), (IEnumerable<LeftCellWrapper>) parentRewriter.UsedCells, string.Empty);
        errorLog.AddEntry(record);
      }
      else if (parentRewriter == null)
      {
        ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.ForeignKeyMissingTableMapping, Strings.ViewGen_Foreign_Key_Missing_Table_Mapping((object) this.ToUserString(), (object) this.ParentTable.Name), (IEnumerable<LeftCellWrapper>) childRewriter.UsedCells, string.Empty);
        errorLog.AddEntry(record);
      }
      else
      {
        if (this.CheckIfConstraintMappedToForeignKeyAssociation(childRewriter, parentRewriter, cells))
          return;
        int count = errorLog.Count;
        if (this.IsForeignKeySuperSetOfPrimaryKeyInChildTable())
          this.GuaranteeForeignKeyConstraintInCSpace(childRewriter, parentRewriter, errorLog);
        else
          this.GuaranteeMappedRelationshipForForeignKey(childRewriter, parentRewriter, (IEnumerable<Cell>) cells, errorLog, config);
        if (count != errorLog.Count)
          return;
        this.CheckForeignKeyColumnOrder(cells, errorLog);
      }
    }

    private void GuaranteeForeignKeyConstraintInCSpace(
      QueryRewriter childRewriter,
      QueryRewriter parentRewriter,
      ErrorLog errorLog)
    {
      ViewgenContext viewgenContext1 = childRewriter.ViewgenContext;
      ViewgenContext viewgenContext2 = parentRewriter.ViewgenContext;
      CellTreeNode basicView1 = childRewriter.BasicView;
      CellTreeNode basicView2 = parentRewriter.BasicView;
      if (FragmentQueryProcessor.Merge(viewgenContext1.RightFragmentQP, viewgenContext2.RightFragmentQP).IsContainedIn(basicView1.RightFragmentQuery, basicView2.RightFragmentQuery))
        return;
      LeftCellWrapper.GetExtentListAsUserString((IEnumerable<LeftCellWrapper>) basicView1.GetLeaves());
      LeftCellWrapper.GetExtentListAsUserString((IEnumerable<LeftCellWrapper>) basicView2.GetLeaves());
      string message = Strings.ViewGen_Foreign_Key_Not_Guaranteed_InCSpace((object) this.ToUserString());
      Set<LeftCellWrapper> set = new Set<LeftCellWrapper>((IEnumerable<LeftCellWrapper>) basicView2.GetLeaves());
      set.AddRange((IEnumerable<LeftCellWrapper>) basicView1.GetLeaves());
      ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.ForeignKeyNotGuaranteedInCSpace, message, (IEnumerable<LeftCellWrapper>) set, string.Empty);
      errorLog.AddEntry(record);
    }

    private void GuaranteeMappedRelationshipForForeignKey(
      QueryRewriter childRewriter,
      QueryRewriter parentRewriter,
      IEnumerable<Cell> cells,
      ErrorLog errorLog,
      ConfigViewGenerator config)
    {
      ViewgenContext viewgenContext1 = childRewriter.ViewgenContext;
      ViewgenContext viewgenContext2 = parentRewriter.ViewgenContext;
      IEnumerable<MemberPath> keyFields = ExtentKey.GetPrimaryKeyForEntityType(new MemberPath((EntitySetBase) this.ChildTable), this.ChildTable.ElementType).KeyFields;
      bool flag1 = false;
      bool flag2 = false;
      List<ErrorLog.Record> errorList = (List<ErrorLog.Record>) null;
      foreach (Cell cell in cells)
      {
        if (cell.SQuery.Extent.Equals((object) this.ChildTable))
        {
          AssociationEndMember relationEndForColumns = ForeignConstraint.GetRelationEndForColumns(cell, this.ChildColumns);
          if (relationEndForColumns == null || this.CheckParentColumnsForForeignKey(cell, cells, relationEndForColumns, ref errorList))
          {
            flag2 = true;
            if (ForeignConstraint.GetRelationEndForColumns(cell, keyFields) != null && relationEndForColumns != null && ForeignConstraint.FindEntitySetForColumnsMappedToEntityKeys(cells, keyFields).Count > 0)
            {
              flag1 = true;
              this.CheckConstraintWhenParentChildMapped(cell, errorLog, relationEndForColumns, config);
              break;
            }
            if (relationEndForColumns != null)
            {
              AssociationSet extent = (AssociationSet) cell.CQuery.Extent;
              MetadataHelper.GetEntitySetAtEnd(extent, relationEndForColumns);
              flag1 = ForeignConstraint.CheckConstraintWhenOnlyParentMapped(extent, relationEndForColumns, childRewriter, parentRewriter);
              if (flag1)
                break;
            }
          }
        }
      }
      if (!flag2)
      {
        foreach (ErrorLog.Record record in errorList)
          errorLog.AddEntry(record);
      }
      else
      {
        if (flag1)
          return;
        string message = Strings.ViewGen_Foreign_Key_Missing_Relationship_Mapping((object) this.ToUserString());
        IEnumerable<LeftCellWrapper> wrappersFromContext1 = (IEnumerable<LeftCellWrapper>) ForeignConstraint.GetWrappersFromContext(viewgenContext2, (EntitySetBase) this.ParentTable);
        IEnumerable<LeftCellWrapper> wrappersFromContext2 = (IEnumerable<LeftCellWrapper>) ForeignConstraint.GetWrappersFromContext(viewgenContext1, (EntitySetBase) this.ChildTable);
        Set<LeftCellWrapper> set = new Set<LeftCellWrapper>(wrappersFromContext1);
        set.AddRange(wrappersFromContext2);
        ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.ForeignKeyMissingRelationshipMapping, message, (IEnumerable<LeftCellWrapper>) set, string.Empty);
        errorLog.AddEntry(record);
      }
    }

    private bool CheckIfConstraintMappedToForeignKeyAssociation(
      QueryRewriter childRewriter,
      QueryRewriter parentRewriter,
      Set<Cell> cells)
    {
      ViewgenContext viewgenContext1 = childRewriter.ViewgenContext;
      ViewgenContext viewgenContext2 = parentRewriter.ViewgenContext;
      List<Set<EdmProperty>> source1 = new List<Set<EdmProperty>>();
      List<Set<EdmProperty>> source2 = new List<Set<EdmProperty>>();
      foreach (Cell cell in cells)
      {
        if (cell.CQuery.Extent.BuiltInTypeKind != BuiltInTypeKind.AssociationSet)
        {
          Set<EdmProperty> cslotsForTableColumns1 = cell.GetCSlotsForTableColumns(this.ChildColumns);
          if (cslotsForTableColumns1 != null && cslotsForTableColumns1.Count != 0)
            source1.Add(cslotsForTableColumns1);
          Set<EdmProperty> cslotsForTableColumns2 = cell.GetCSlotsForTableColumns(this.ParentColumns);
          if (cslotsForTableColumns2 != null && cslotsForTableColumns2.Count != 0)
            source2.Add(cslotsForTableColumns2);
        }
      }
      if (source1.Count != 0 && source2.Count != 0)
      {
        foreach (AssociationType associationType in viewgenContext1.EntityContainerMapping.EdmEntityContainer.BaseEntitySets.OfType<AssociationSet>().Where<AssociationSet>((Func<AssociationSet, bool>) (it => it.ElementType.IsForeignKey)).Select<AssociationSet, AssociationType>((Func<AssociationSet, AssociationType>) (it => it.ElementType)))
        {
          ReferentialConstraint refConstraint = associationType.ReferentialConstraints.FirstOrDefault<ReferentialConstraint>();
          IEnumerable<Set<EdmProperty>> source3 = source1.Where<Set<EdmProperty>>((Func<Set<EdmProperty>, bool>) (it => it.SetEquals(new Set<EdmProperty>((IEnumerable<EdmProperty>) refConstraint.ToProperties))));
          IEnumerable<Set<EdmProperty>> source4 = source2.Where<Set<EdmProperty>>((Func<Set<EdmProperty>, bool>) (it => it.SetEquals(new Set<EdmProperty>((IEnumerable<EdmProperty>) refConstraint.FromProperties))));
          if (source3.Count<Set<EdmProperty>>() != 0 && source4.Count<Set<EdmProperty>>() != 0)
          {
            foreach (IEnumerable<EdmProperty> properties1_1 in source4)
            {
              Set<int> propertyIndexes = ForeignConstraint.GetPropertyIndexes(properties1_1, refConstraint.FromProperties);
              foreach (IEnumerable<EdmProperty> properties1_2 in source3)
              {
                if (ForeignConstraint.GetPropertyIndexes(properties1_2, refConstraint.ToProperties).SequenceEqual<int>((IEnumerable<int>) propertyIndexes))
                  return true;
              }
            }
          }
        }
      }
      return false;
    }

    private static Set<int> GetPropertyIndexes(
      IEnumerable<EdmProperty> properties1,
      ReadOnlyMetadataCollection<EdmProperty> properties2)
    {
      Set<int> set = new Set<int>();
      foreach (EdmProperty edmProperty in properties1)
        set.Add(properties2.IndexOf(edmProperty));
      return set;
    }

    private static bool CheckConstraintWhenOnlyParentMapped(
      AssociationSet assocSet,
      AssociationEndMember endMember,
      QueryRewriter childRewriter,
      QueryRewriter parentRewriter)
    {
      ViewgenContext viewgenContext1 = childRewriter.ViewgenContext;
      ViewgenContext viewgenContext2 = parentRewriter.ViewgenContext;
      CellTreeNode basicView = parentRewriter.BasicView;
      RoleBoolean roleBoolean = new RoleBoolean(assocSet.AssociationSetEnds[endMember.Name]);
      BoolExpression whereClause = basicView.RightFragmentQuery.Condition.Create((BoolLiteral) roleBoolean);
      FragmentQuery q1 = FragmentQuery.Create((IEnumerable<MemberPath>) basicView.RightFragmentQuery.Attributes, whereClause);
      return FragmentQueryProcessor.Merge(viewgenContext1.RightFragmentQP, viewgenContext2.RightFragmentQP).IsContainedIn(q1, basicView.RightFragmentQuery);
    }

    private bool CheckConstraintWhenParentChildMapped(
      Cell cell,
      ErrorLog errorLog,
      AssociationEndMember parentEnd,
      ConfigViewGenerator config)
    {
      bool flag = true;
      if (parentEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many)
      {
        ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.ForeignKeyUpperBoundMustBeOne, Strings.ViewGen_Foreign_Key_UpperBound_MustBeOne((object) this.ToUserString(), (object) cell.CQuery.Extent.Name, (object) parentEnd.Name), cell, string.Empty);
        errorLog.AddEntry(record);
        flag = false;
      }
      if (!MemberPath.AreAllMembersNullable(this.ChildColumns) && parentEnd.RelationshipMultiplicity != RelationshipMultiplicity.One)
      {
        ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.ForeignKeyLowerBoundMustBeOne, Strings.ViewGen_Foreign_Key_LowerBound_MustBeOne((object) this.ToUserString(), (object) cell.CQuery.Extent.Name, (object) parentEnd.Name), cell, string.Empty);
        errorLog.AddEntry(record);
        flag = false;
      }
      if (config.IsNormalTracing && flag)
        Trace.WriteLine("Foreign key mapped to relationship " + cell.CQuery.Extent.Name);
      return flag;
    }

    private bool CheckParentColumnsForForeignKey(
      Cell cell,
      IEnumerable<Cell> cells,
      AssociationEndMember parentEnd,
      ref List<ErrorLog.Record> errorList)
    {
      EntitySet entitySetAtEnd = MetadataHelper.GetEntitySetAtEnd((AssociationSet) cell.CQuery.Extent, parentEnd);
      if (ForeignConstraint.FindEntitySetForColumnsMappedToEntityKeys(cells, this.ParentColumns).Contains(entitySetAtEnd))
        return true;
      if (errorList == null)
        errorList = new List<ErrorLog.Record>();
      ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.ForeignKeyParentTableNotMappedToEnd, Strings.ViewGen_Foreign_Key_ParentTable_NotMappedToEnd((object) this.ToUserString(), (object) this.ChildTable.Name, (object) cell.CQuery.Extent.Name, (object) parentEnd.Name, (object) this.ParentTable.Name, (object) entitySetAtEnd.Name), cell, string.Empty);
      errorList.Add(record);
      return false;
    }

    private static IList<EntitySet> FindEntitySetForColumnsMappedToEntityKeys(
      IEnumerable<Cell> cells,
      IEnumerable<MemberPath> tableColumns)
    {
      List<EntitySet> entitySetList = new List<EntitySet>();
      foreach (Cell cell in cells)
      {
        CellQuery cquery = cell.CQuery;
        if (!(cquery.Extent is AssociationSet))
        {
          Set<EdmProperty> cslotsForTableColumns = cell.GetCSlotsForTableColumns(tableColumns);
          if (cslotsForTableColumns != null)
          {
            EntitySet extent = (EntitySet) cquery.Extent;
            List<EdmProperty> edmPropertyList = new List<EdmProperty>();
            foreach (EdmProperty keyMember in extent.ElementType.KeyMembers)
              edmPropertyList.Add(keyMember);
            if (new Set<EdmProperty>((IEnumerable<EdmProperty>) edmPropertyList).MakeReadOnly().SetEquals(cslotsForTableColumns))
              entitySetList.Add(extent);
          }
        }
      }
      return (IList<EntitySet>) entitySetList;
    }

    private static AssociationEndMember GetRelationEndForColumns(
      Cell cell,
      IEnumerable<MemberPath> columns)
    {
      if (cell.CQuery.Extent is EntitySet)
        return (AssociationEndMember) null;
      AssociationSet extent = (AssociationSet) cell.CQuery.Extent;
      foreach (AssociationSetEnd associationSetEnd in extent.AssociationSetEnds)
      {
        AssociationEndMember associationEndMember = associationSetEnd.CorrespondingAssociationEndMember;
        ExtentKey keyForEntityType = ExtentKey.GetPrimaryKeyForEntityType(new MemberPath((EntitySetBase) extent, (EdmMember) associationEndMember), associationSetEnd.EntitySet.ElementType);
        List<int> projectedPositions1 = cell.CQuery.GetProjectedPositions(keyForEntityType.KeyFields);
        if (projectedPositions1 != null)
        {
          List<int> projectedPositions2 = cell.SQuery.GetProjectedPositions(columns, projectedPositions1);
          if (projectedPositions2 != null && Helpers.IsSetEqual<int>((IEnumerable<int>) projectedPositions2, (IEnumerable<int>) projectedPositions1, (IEqualityComparer<int>) EqualityComparer<int>.Default))
            return associationEndMember;
        }
      }
      return (AssociationEndMember) null;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "extent")]
    private static List<LeftCellWrapper> GetWrappersFromContext(
      ViewgenContext context,
      EntitySetBase extent)
    {
      return context != null ? context.AllWrappersForExtent : new List<LeftCellWrapper>();
    }

    private bool CheckForeignKeyColumnOrder(Set<Cell> cells, ErrorLog errorLog)
    {
      List<Cell> cellList1 = new List<Cell>();
      List<Cell> cellList2 = new List<Cell>();
      foreach (Cell cell in cells)
      {
        if (cell.SQuery.Extent.Equals((object) this.ChildTable))
          cellList2.Add(cell);
        if (cell.SQuery.Extent.Equals((object) this.ParentTable))
          cellList1.Add(cell);
      }
      foreach (Cell cell1 in cellList2)
      {
        List<List<int>> slotNumsForColumns1 = ForeignConstraint.GetSlotNumsForColumns(cell1, this.ChildColumns);
        if (slotNumsForColumns1.Count != 0)
        {
          List<MemberPath> memberPathList1 = (List<MemberPath>) null;
          List<MemberPath> memberPathList2 = (List<MemberPath>) null;
          Cell cell2 = (Cell) null;
          foreach (List<int> intList1 in slotNumsForColumns1)
          {
            memberPathList1 = new List<MemberPath>(intList1.Count);
            foreach (int slotNum in intList1)
            {
              MemberProjectedSlot memberProjectedSlot = (MemberProjectedSlot) cell1.CQuery.ProjectedSlotAt(slotNum);
              memberPathList1.Add(memberProjectedSlot.MemberPath);
            }
            foreach (Cell cell3 in cellList1)
            {
              List<List<int>> slotNumsForColumns2 = ForeignConstraint.GetSlotNumsForColumns(cell3, this.ParentColumns);
              if (slotNumsForColumns2.Count != 0)
              {
                foreach (List<int> intList2 in slotNumsForColumns2)
                {
                  memberPathList2 = new List<MemberPath>(intList2.Count);
                  foreach (int slotNum in intList2)
                  {
                    MemberProjectedSlot memberProjectedSlot = (MemberProjectedSlot) cell3.CQuery.ProjectedSlotAt(slotNum);
                    memberPathList2.Add(memberProjectedSlot.MemberPath);
                  }
                  if (memberPathList1.Count == memberPathList2.Count)
                  {
                    bool flag = false;
                    for (int index = 0; index < memberPathList1.Count && !flag; ++index)
                    {
                      MemberPath memberPath = memberPathList2[index];
                      MemberPath path1 = memberPathList1[index];
                      if (!memberPath.LeafEdmMember.Equals((object) path1.LeafEdmMember))
                      {
                        if (memberPath.IsEquivalentViaRefConstraint(path1))
                          return true;
                        flag = true;
                      }
                    }
                    if (!flag)
                      return true;
                    cell2 = cell3;
                  }
                }
              }
            }
          }
          ErrorLog.Record record = new ErrorLog.Record(ViewGenErrorCode.ForeignKeyColumnOrderIncorrect, Strings.ViewGen_Foreign_Key_ColumnOrder_Incorrect((object) this.ToUserString(), (object) MemberPath.PropertiesToUserString(this.ChildColumns, false), (object) this.ChildTable.Name, (object) MemberPath.PropertiesToUserString((IEnumerable<MemberPath>) memberPathList1, false), (object) cell1.CQuery.Extent.Name, (object) MemberPath.PropertiesToUserString(this.ParentColumns, false), (object) this.ParentTable.Name, (object) MemberPath.PropertiesToUserString((IEnumerable<MemberPath>) memberPathList2, false), (object) cell2.CQuery.Extent.Name), (IEnumerable<Cell>) new Cell[2]
          {
            cell2,
            cell1
          }, string.Empty);
          errorLog.AddEntry(record);
          return false;
        }
      }
      return true;
    }

    private static List<List<int>> GetSlotNumsForColumns(
      Cell cell,
      IEnumerable<MemberPath> columns)
    {
      List<List<int>> intListList = new List<List<int>>();
      AssociationSet extent = cell.CQuery.Extent as AssociationSet;
      if (extent != null)
      {
        foreach (AssociationSetEnd associationSetEnd in extent.AssociationSetEnds)
        {
          List<int> associationEndSlots = cell.CQuery.GetAssociationEndSlots(associationSetEnd.CorrespondingAssociationEndMember);
          List<int> projectedPositions = cell.SQuery.GetProjectedPositions(columns, associationEndSlots);
          if (projectedPositions != null)
            intListList.Add(projectedPositions);
        }
      }
      else
      {
        List<int> projectedPositions = cell.SQuery.GetProjectedPositions(columns);
        if (projectedPositions != null)
          intListList.Add(projectedPositions);
      }
      return intListList;
    }

    private bool IsForeignKeySuperSetOfPrimaryKeyInChildTable()
    {
      bool flag1 = true;
      foreach (EdmProperty keyMember in this.m_childTable.ElementType.KeyMembers)
      {
        bool flag2 = false;
        foreach (MemberPath childColumn in this.m_childColumns)
        {
          if (childColumn.LeafEdmMember.Equals((object) keyMember))
          {
            flag2 = true;
            break;
          }
        }
        if (!flag2)
        {
          flag1 = false;
          break;
        }
      }
      return flag1;
    }

    private bool IsConstraintRelevantForCells(IEnumerable<Cell> cells)
    {
      bool flag = false;
      foreach (Cell cell in cells)
      {
        EntitySetBase extent = cell.SQuery.Extent;
        if (extent.Equals((object) this.m_parentTable) || extent.Equals((object) this.m_childTable))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    internal string ToUserString()
    {
      return Strings.ViewGen_Foreign_Key((object) this.m_fKeySet.Name, (object) this.m_childTable.Name, (object) MemberPath.PropertiesToUserString((IEnumerable<MemberPath>) this.m_childColumns, false), (object) this.m_parentTable.Name, (object) MemberPath.PropertiesToUserString((IEnumerable<MemberPath>) this.m_parentColumns, false));
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append(this.m_fKeySet.Name + ": ");
      builder.Append(this.ToUserString());
    }
  }
}
