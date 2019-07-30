// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CqlGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal sealed class CqlGenerator : InternalBase
  {
    private readonly CellTreeNode m_view;
    private readonly Dictionary<MemberPath, CaseStatement> m_caseStatements;
    private readonly MemberProjectionIndex m_projectedSlotMap;
    private readonly int m_numBools;
    private int m_currentBlockNum;
    private readonly BoolExpression m_topLevelWhereClause;
    private readonly CqlIdentifiers m_identifiers;
    private readonly StorageMappingItemCollection m_mappingItemCollection;

    internal CqlGenerator(
      CellTreeNode view,
      Dictionary<MemberPath, CaseStatement> caseStatements,
      CqlIdentifiers identifiers,
      MemberProjectionIndex projectedSlotMap,
      int numCellsInView,
      BoolExpression topLevelWhereClause,
      StorageMappingItemCollection mappingItemCollection)
    {
      this.m_view = view;
      this.m_caseStatements = caseStatements;
      this.m_projectedSlotMap = projectedSlotMap;
      this.m_numBools = numCellsInView;
      this.m_topLevelWhereClause = topLevelWhereClause;
      this.m_identifiers = identifiers;
      this.m_mappingItemCollection = mappingItemCollection;
    }

    private int TotalSlots
    {
      get
      {
        return this.m_projectedSlotMap.Count + this.m_numBools;
      }
    }

    internal string GenerateEsql()
    {
      CqlBlock cqlBlockTree = this.GenerateCqlBlockTree();
      StringBuilder builder = new StringBuilder(1024);
      cqlBlockTree.AsEsql(builder, true, 1);
      return builder.ToString();
    }

    internal DbQueryCommandTree GenerateCqt()
    {
      return DbQueryCommandTree.FromValidExpression(this.m_mappingItemCollection.Workspace, DataSpace.SSpace, this.GenerateCqlBlockTree().AsCqt(true), true);
    }

    private CqlBlock GenerateCqlBlockTree()
    {
      bool[] requiredSlots = this.GetRequiredSlots();
      List<WithRelationship> withRelationships = new List<WithRelationship>();
      CqlBlock cqlBlock = this.m_view.ToCqlBlock(requiredSlots, this.m_identifiers, ref this.m_currentBlockNum, ref withRelationships);
      foreach (CaseStatement caseStatement in this.m_caseStatements.Values)
        caseStatement.Simplify();
      return this.ConstructCaseBlocks(cqlBlock, (IEnumerable<WithRelationship>) withRelationships);
    }

    private bool[] GetRequiredSlots()
    {
      bool[] requiredSlots = new bool[this.TotalSlots];
      foreach (CaseStatement caseStatement in this.m_caseStatements.Values)
        this.GetRequiredSlotsForCaseMember(caseStatement.MemberPath, requiredSlots);
      for (int index = this.TotalSlots - this.m_numBools; index < this.TotalSlots; ++index)
        requiredSlots[index] = true;
      foreach (CaseStatement caseStatement in this.m_caseStatements.Values)
      {
        if (!caseStatement.MemberPath.IsPartOfKey && !caseStatement.DependsOnMemberValue)
          requiredSlots[this.m_projectedSlotMap.IndexOf(caseStatement.MemberPath)] = false;
      }
      return requiredSlots;
    }

    private CqlBlock ConstructCaseBlocks(
      CqlBlock viewBlock,
      IEnumerable<WithRelationship> withRelationships)
    {
      bool[] flagArray = new bool[this.TotalSlots];
      flagArray[0] = true;
      this.m_topLevelWhereClause.GetRequiredSlots(this.m_projectedSlotMap, flagArray);
      return this.ConstructCaseBlocks(viewBlock, 0, flagArray, withRelationships);
    }

    private CqlBlock ConstructCaseBlocks(
      CqlBlock viewBlock,
      int startSlotNum,
      bool[] parentRequiredSlots,
      IEnumerable<WithRelationship> withRelationships)
    {
      int count = this.m_projectedSlotMap.Count;
      int caseStatementSlot = this.FindNextCaseStatementSlot(startSlotNum, parentRequiredSlots, count);
      if (caseStatementSlot == -1)
        return viewBlock;
      MemberPath projectedSlot = this.m_projectedSlotMap[caseStatementSlot];
      bool[] flagArray = new bool[this.TotalSlots];
      this.GetRequiredSlotsForCaseMember(projectedSlot, flagArray);
      for (int index = 0; index < this.TotalSlots; ++index)
      {
        if (parentRequiredSlots[index])
          flagArray[index] = true;
      }
      CaseStatement caseStatement = this.m_caseStatements[projectedSlot];
      flagArray[caseStatementSlot] = caseStatement.DependsOnMemberValue;
      CqlBlock cqlBlock = this.ConstructCaseBlocks(viewBlock, caseStatementSlot + 1, flagArray, (IEnumerable<WithRelationship>) null);
      SlotInfo[] forCaseStatement = this.CreateSlotInfosForCaseStatement(parentRequiredSlots, caseStatementSlot, cqlBlock, caseStatement, withRelationships);
      ++this.m_currentBlockNum;
      BoolExpression whereClause = startSlotNum == 0 ? this.m_topLevelWhereClause : BoolExpression.True;
      if (startSlotNum == 0)
      {
        for (int index = 1; index < forCaseStatement.Length; ++index)
          forCaseStatement[index].ResetIsRequiredByParent();
      }
      return (CqlBlock) new CaseCqlBlock(forCaseStatement, caseStatementSlot, cqlBlock, whereClause, this.m_identifiers, this.m_currentBlockNum);
    }

    private SlotInfo[] CreateSlotInfosForCaseStatement(
      bool[] parentRequiredSlots,
      int foundSlot,
      CqlBlock childBlock,
      CaseStatement thisCaseStatement,
      IEnumerable<WithRelationship> withRelationships)
    {
      int num = childBlock.Slots.Count - this.TotalSlots;
      SlotInfo[] slotInfoArray = new SlotInfo[this.TotalSlots + num];
      for (int slotNum = 0; slotNum < this.TotalSlots; ++slotNum)
      {
        bool isProjected = childBlock.IsProjected(slotNum);
        bool parentRequiredSlot = parentRequiredSlots[slotNum];
        ProjectedSlot slotValue = childBlock.SlotValue(slotNum);
        MemberPath outputMemberPath = this.GetOutputMemberPath(slotNum);
        if (slotNum == foundSlot)
        {
          slotValue = (ProjectedSlot) new CaseStatementProjectedSlot(thisCaseStatement.DeepQualify(childBlock), withRelationships);
          isProjected = true;
        }
        else if (isProjected && parentRequiredSlot)
          slotValue = (ProjectedSlot) childBlock.QualifySlotWithBlockAlias(slotNum);
        SlotInfo slotInfo = new SlotInfo(parentRequiredSlot && isProjected, isProjected, slotValue, outputMemberPath);
        slotInfoArray[slotNum] = slotInfo;
      }
      for (int totalSlots = this.TotalSlots; totalSlots < this.TotalSlots + num; ++totalSlots)
      {
        QualifiedSlot qualifiedSlot = childBlock.QualifySlotWithBlockAlias(totalSlots);
        slotInfoArray[totalSlots] = new SlotInfo(true, true, (ProjectedSlot) qualifiedSlot, childBlock.MemberPath(totalSlots));
      }
      return slotInfoArray;
    }

    private int FindNextCaseStatementSlot(
      int startSlotNum,
      bool[] parentRequiredSlots,
      int numMembers)
    {
      int num = -1;
      for (int index = startSlotNum; index < numMembers; ++index)
      {
        MemberPath projectedSlot = this.m_projectedSlotMap[index];
        if (parentRequiredSlots[index] && this.m_caseStatements.ContainsKey(projectedSlot))
        {
          num = index;
          break;
        }
      }
      return num;
    }

    private void GetRequiredSlotsForCaseMember(MemberPath caseMemberPath, bool[] requiredSlots)
    {
      CaseStatement caseStatement = this.m_caseStatements[caseMemberPath];
      bool flag = false;
      foreach (CaseStatement.WhenThen clause in caseStatement.Clauses)
      {
        clause.Condition.GetRequiredSlots(this.m_projectedSlotMap, requiredSlots);
        if (!(clause.Value is ConstantProjectedSlot))
          flag = true;
      }
      EdmType edmType = caseMemberPath.EdmType;
      if (Helper.IsEntityType(edmType) || Helper.IsComplexType(edmType))
      {
        foreach (EdmType instantiatedType in caseStatement.InstantiatedTypes)
        {
          foreach (EdmMember structuralMember in (IEnumerable) Helper.GetAllStructuralMembers(instantiatedType))
          {
            int slotIndex = this.GetSlotIndex(caseMemberPath, structuralMember);
            requiredSlots[slotIndex] = true;
          }
        }
      }
      else if (caseMemberPath.IsScalarType())
      {
        if (!flag)
          return;
        int index = this.m_projectedSlotMap.IndexOf(caseMemberPath);
        requiredSlots[index] = true;
      }
      else if (Helper.IsAssociationType(edmType))
      {
        foreach (AssociationEndMember associationEndMember in ((AssociationSet) caseMemberPath.Extent).ElementType.AssociationEndMembers)
        {
          int slotIndex = this.GetSlotIndex(caseMemberPath, (EdmMember) associationEndMember);
          requiredSlots[slotIndex] = true;
        }
      }
      else
      {
        EntityTypeBase elementType = (edmType as RefType).ElementType;
        MetadataHelper.GetEntitySetAtEnd((AssociationSet) caseMemberPath.Extent, (AssociationEndMember) caseMemberPath.LeafEdmMember);
        foreach (EdmMember keyMember in elementType.KeyMembers)
        {
          int slotIndex = this.GetSlotIndex(caseMemberPath, keyMember);
          requiredSlots[slotIndex] = true;
        }
      }
    }

    private MemberPath GetOutputMemberPath(int slotNum)
    {
      return this.m_projectedSlotMap.GetMemberPath(slotNum, this.TotalSlots - this.m_projectedSlotMap.Count);
    }

    private int GetSlotIndex(MemberPath member, EdmMember child)
    {
      return this.m_projectedSlotMap.IndexOf(new MemberPath(member, child));
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append("View: ");
      this.m_view.ToCompactString(builder);
      builder.Append("ProjectedSlotMap: ");
      this.m_projectedSlotMap.ToCompactString(builder);
      builder.Append("Case statements: ");
      foreach (MemberPath key in this.m_caseStatements.Keys)
      {
        this.m_caseStatements[key].ToCompactString(builder);
        builder.AppendLine();
      }
    }
  }
}
