// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.CellQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Validation;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class CellQuery : InternalBase
  {
    private List<BoolExpression> m_boolExprs;
    private readonly ProjectedSlot[] m_projectedSlots;
    private BoolExpression m_whereClause;
    private readonly BoolExpression m_originalWhereClause;
    private readonly CellQuery.SelectDistinct m_selectDistinct;
    private readonly MemberPath m_extentMemberPath;
    private BasicCellRelation m_basicCellRelation;

    internal CellQuery(
      List<ProjectedSlot> slots,
      BoolExpression whereClause,
      MemberPath rootMember,
      CellQuery.SelectDistinct eliminateDuplicates)
      : this(slots.ToArray(), whereClause, new List<BoolExpression>(), eliminateDuplicates, rootMember)
    {
    }

    internal CellQuery(
      ProjectedSlot[] projectedSlots,
      BoolExpression whereClause,
      List<BoolExpression> boolExprs,
      CellQuery.SelectDistinct elimDupl,
      MemberPath rootMember)
    {
      this.m_boolExprs = boolExprs;
      this.m_projectedSlots = projectedSlots;
      this.m_whereClause = whereClause;
      this.m_originalWhereClause = whereClause;
      this.m_selectDistinct = elimDupl;
      this.m_extentMemberPath = rootMember;
    }

    internal CellQuery(CellQuery source)
    {
      this.m_basicCellRelation = source.m_basicCellRelation;
      this.m_boolExprs = source.m_boolExprs;
      this.m_selectDistinct = source.m_selectDistinct;
      this.m_extentMemberPath = source.m_extentMemberPath;
      this.m_originalWhereClause = source.m_originalWhereClause;
      this.m_projectedSlots = source.m_projectedSlots;
      this.m_whereClause = source.m_whereClause;
    }

    private CellQuery(CellQuery existing, ProjectedSlot[] newSlots)
      : this(newSlots, existing.m_whereClause, existing.m_boolExprs, existing.m_selectDistinct, existing.m_extentMemberPath)
    {
    }

    internal CellQuery.SelectDistinct SelectDistinctFlag
    {
      get
      {
        return this.m_selectDistinct;
      }
    }

    internal EntitySetBase Extent
    {
      get
      {
        return this.m_extentMemberPath.Extent;
      }
    }

    internal int NumProjectedSlots
    {
      get
      {
        return this.m_projectedSlots.Length;
      }
    }

    internal ProjectedSlot[] ProjectedSlots
    {
      get
      {
        return this.m_projectedSlots;
      }
    }

    internal List<BoolExpression> BoolVars
    {
      get
      {
        return this.m_boolExprs;
      }
    }

    internal int NumBoolVars
    {
      get
      {
        return this.m_boolExprs.Count;
      }
    }

    internal BoolExpression WhereClause
    {
      get
      {
        return this.m_whereClause;
      }
    }

    internal MemberPath SourceExtentMemberPath
    {
      get
      {
        return this.m_extentMemberPath;
      }
    }

    internal BasicCellRelation BasicCellRelation
    {
      get
      {
        return this.m_basicCellRelation;
      }
    }

    internal IEnumerable<MemberRestriction> Conditions
    {
      get
      {
        return this.GetConjunctsFromOriginalWhereClause();
      }
    }

    internal ProjectedSlot ProjectedSlotAt(int slotNum)
    {
      return this.m_projectedSlots[slotNum];
    }

    internal ErrorLog.Record CheckForDuplicateFields(CellQuery cQuery, Cell sourceCell)
    {
      KeyToListMap<MemberProjectedSlot, int> keyToListMap = new KeyToListMap<MemberProjectedSlot, int>((IEqualityComparer<MemberProjectedSlot>) ProjectedSlot.EqualityComparer);
      for (int index = 0; index < this.m_projectedSlots.Length; ++index)
      {
        MemberProjectedSlot projectedSlot = this.m_projectedSlots[index] as MemberProjectedSlot;
        keyToListMap.Add(projectedSlot, index);
      }
      StringBuilder stringBuilder1 = (StringBuilder) null;
      bool flag = false;
      foreach (MemberProjectedSlot key in keyToListMap.Keys)
      {
        ReadOnlyCollection<int> cSideSlotIndexes = keyToListMap.ListForKey(key);
        if (cSideSlotIndexes.Count > 1 && !cQuery.AreSlotsEquivalentViaRefConstraints(cSideSlotIndexes))
        {
          flag = true;
          if (stringBuilder1 == null)
          {
            stringBuilder1 = new StringBuilder(Strings.ViewGen_Duplicate_CProperties((object) this.Extent.Name));
            stringBuilder1.AppendLine();
          }
          StringBuilder stringBuilder2 = new StringBuilder();
          for (int index1 = 0; index1 < cSideSlotIndexes.Count; ++index1)
          {
            int index2 = cSideSlotIndexes[index1];
            if (index1 != 0)
              stringBuilder2.Append(", ");
            MemberProjectedSlot projectedSlot = (MemberProjectedSlot) cQuery.m_projectedSlots[index2];
            stringBuilder2.Append(projectedSlot.ToUserString());
          }
          stringBuilder1.AppendLine(Strings.ViewGen_Duplicate_CProperties_IsMapped((object) key.ToUserString(), (object) stringBuilder2.ToString()));
        }
      }
      if (!flag)
        return (ErrorLog.Record) null;
      return new ErrorLog.Record(ViewGenErrorCode.DuplicateCPropertiesMapped, stringBuilder1.ToString(), sourceCell, string.Empty);
    }

    private bool AreSlotsEquivalentViaRefConstraints(ReadOnlyCollection<int> cSideSlotIndexes)
    {
      if (!(this.Extent is AssociationSet) || cSideSlotIndexes.Count > 2)
        return false;
      return ((MemberProjectedSlot) this.m_projectedSlots[cSideSlotIndexes[0]]).MemberPath.IsEquivalentViaRefConstraint(((MemberProjectedSlot) this.m_projectedSlots[cSideSlotIndexes[1]]).MemberPath);
    }

    internal ErrorLog.Record CheckForProjectedNotNullSlots(
      Cell sourceCell,
      IEnumerable<Cell> associationSets)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag1 = false;
      foreach (MemberRestriction condition in this.Conditions)
      {
        if (condition.Domain.ContainsNotNull() && MemberProjectedSlot.GetSlotForMember((IEnumerable<ProjectedSlot>) this.m_projectedSlots, condition.RestrictedMemberSlot.MemberPath) == null)
        {
          bool flag2 = true;
          if (this.Extent is EntitySet)
          {
            bool flag3 = sourceCell.CQuery == this;
            ViewTarget target = flag3 ? ViewTarget.QueryView : ViewTarget.UpdateView;
            CellQuery cellQuery = flag3 ? sourceCell.SQuery : sourceCell.CQuery;
            EntitySet rightExtent = cellQuery.Extent as EntitySet;
            if (rightExtent != null)
            {
              foreach (AssociationSet associationSet in MetadataHelper.GetAssociationsForEntitySet((EntitySetBase) (cellQuery.Extent as EntitySet)).Where<AssociationSet>((Func<AssociationSet, bool>) (association => association.AssociationSetEnds.Any<AssociationSetEnd>((Func<AssociationSetEnd, bool>) (end =>
              {
                if (end.CorrespondingAssociationEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)
                  return MetadataHelper.GetOppositeEnd(end).EntitySet.EdmEquals((MetadataItem) rightExtent);
                return false;
              })))))
              {
                AssociationSet association = associationSet;
                foreach (Cell cell in associationSets.Where<Cell>((Func<Cell, bool>) (c => c.GetRightQuery(target).Extent.EdmEquals((MetadataItem) association))))
                {
                  if (MemberProjectedSlot.GetSlotForMember((IEnumerable<ProjectedSlot>) cell.GetLeftQuery(target).ProjectedSlots, condition.RestrictedMemberSlot.MemberPath) != null)
                    flag2 = false;
                }
              }
            }
          }
          if (flag2)
          {
            stringBuilder.AppendLine(Strings.ViewGen_NotNull_No_Projected_Slot((object) condition.RestrictedMemberSlot.MemberPath.PathToString(new bool?(false))));
            flag1 = true;
          }
        }
      }
      if (!flag1)
        return (ErrorLog.Record) null;
      return new ErrorLog.Record(ViewGenErrorCode.NotNullNoProjectedSlot, stringBuilder.ToString(), sourceCell, string.Empty);
    }

    internal void FixMissingSlotAsDefaultConstant(int slotNumber, ConstantProjectedSlot slot)
    {
      this.m_projectedSlots[slotNumber] = (ProjectedSlot) slot;
    }

    internal void CreateFieldAlignedCellQueries(
      CellQuery otherQuery,
      MemberProjectionIndex projectedSlotMap,
      out CellQuery newMainQuery,
      out CellQuery newOtherQuery)
    {
      int count = projectedSlotMap.Count;
      ProjectedSlot[] newSlots1 = new ProjectedSlot[count];
      ProjectedSlot[] newSlots2 = new ProjectedSlot[count];
      for (int index1 = 0; index1 < this.m_projectedSlots.Length; ++index1)
      {
        MemberProjectedSlot projectedSlot = this.m_projectedSlots[index1] as MemberProjectedSlot;
        int index2 = projectedSlotMap.IndexOf(projectedSlot.MemberPath);
        newSlots1[index2] = this.m_projectedSlots[index1];
        newSlots2[index2] = otherQuery.m_projectedSlots[index1];
      }
      newMainQuery = new CellQuery(this, newSlots1);
      newOtherQuery = new CellQuery(otherQuery, newSlots2);
    }

    internal Set<MemberPath> GetNonNullSlots()
    {
      Set<MemberPath> set = new Set<MemberPath>(MemberPath.EqualityComparer);
      foreach (ProjectedSlot projectedSlot in this.m_projectedSlots)
      {
        if (projectedSlot != null)
        {
          MemberProjectedSlot memberProjectedSlot = projectedSlot as MemberProjectedSlot;
          set.Add(memberProjectedSlot.MemberPath);
        }
      }
      return set;
    }

    internal ErrorLog.Record VerifyKeysPresent(
      Cell ownerCell,
      Func<object, object, string> formatEntitySetMessage,
      Func<object, object, object, string> formatAssociationSetMessage,
      ViewGenErrorCode errorCode)
    {
      List<MemberPath> memberPathList = new List<MemberPath>(1);
      List<ExtentKey> extentKeyList = new List<ExtentKey>(1);
      if (this.Extent is EntitySet)
      {
        MemberPath prefix = new MemberPath(this.Extent);
        memberPathList.Add(prefix);
        EntityType elementType = (EntityType) this.Extent.ElementType;
        List<ExtentKey> keysForEntityType = ExtentKey.GetKeysForEntityType(prefix, elementType);
        extentKeyList.Add(keysForEntityType[0]);
      }
      else
      {
        AssociationSet extent = (AssociationSet) this.Extent;
        foreach (AssociationSetEnd associationSetEnd in extent.AssociationSetEnds)
        {
          AssociationEndMember associationEndMember = associationSetEnd.CorrespondingAssociationEndMember;
          MemberPath prefix = new MemberPath((EntitySetBase) extent, (EdmMember) associationEndMember);
          memberPathList.Add(prefix);
          List<ExtentKey> keysForEntityType = ExtentKey.GetKeysForEntityType(prefix, MetadataHelper.GetEntityTypeForEnd(associationEndMember));
          extentKeyList.Add(keysForEntityType[0]);
        }
      }
      for (int index = 0; index < memberPathList.Count; ++index)
      {
        MemberPath prefix = memberPathList[index];
        if (MemberProjectedSlot.GetKeySlots(this.GetMemberProjectedSlots(), prefix) == null)
        {
          ExtentKey extentKey = extentKeyList[index];
          string message;
          if (this.Extent is EntitySet)
          {
            string userString = MemberPath.PropertiesToUserString(extentKey.KeyFields, true);
            message = formatEntitySetMessage((object) userString, (object) this.Extent.Name);
          }
          else
          {
            string name = prefix.RootEdmMember.Name;
            string userString = MemberPath.PropertiesToUserString(extentKey.KeyFields, false);
            message = formatAssociationSetMessage((object) userString, (object) name, (object) this.Extent.Name);
          }
          return new ErrorLog.Record(errorCode, message, ownerCell, string.Empty);
        }
      }
      return (ErrorLog.Record) null;
    }

    internal IEnumerable<MemberPath> GetProjectedMembers()
    {
      foreach (MemberProjectedSlot memberProjectedSlot in this.GetMemberProjectedSlots())
        yield return memberProjectedSlot.MemberPath;
    }

    private IEnumerable<MemberProjectedSlot> GetMemberProjectedSlots()
    {
      foreach (ProjectedSlot projectedSlot in this.m_projectedSlots)
      {
        MemberProjectedSlot memberSlot = projectedSlot as MemberProjectedSlot;
        if (memberSlot != null)
          yield return memberSlot;
      }
    }

    internal List<MemberProjectedSlot> GetAllQuerySlots()
    {
      HashSet<MemberProjectedSlot> memberProjectedSlotSet = new HashSet<MemberProjectedSlot>(this.GetMemberProjectedSlots());
      memberProjectedSlotSet.Add(new MemberProjectedSlot(this.SourceExtentMemberPath));
      foreach (MemberRestriction condition in this.Conditions)
        memberProjectedSlotSet.Add(condition.RestrictedMemberSlot);
      return new List<MemberProjectedSlot>((IEnumerable<MemberProjectedSlot>) memberProjectedSlotSet);
    }

    internal int GetProjectedPosition(MemberProjectedSlot slot)
    {
      for (int index = 0; index < this.m_projectedSlots.Length; ++index)
      {
        if (ProjectedSlot.EqualityComparer.Equals((ProjectedSlot) slot, this.m_projectedSlots[index]))
          return index;
      }
      return -1;
    }

    internal List<int> GetProjectedPositions(MemberPath member)
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < this.m_projectedSlots.Length; ++index)
      {
        MemberProjectedSlot projectedSlot = this.m_projectedSlots[index] as MemberProjectedSlot;
        if (projectedSlot != null && MemberPath.EqualityComparer.Equals(member, projectedSlot.MemberPath))
          intList.Add(index);
      }
      return intList;
    }

    internal List<int> GetProjectedPositions(IEnumerable<MemberPath> paths)
    {
      List<int> intList = new List<int>();
      foreach (MemberPath path in paths)
      {
        List<int> projectedPositions = this.GetProjectedPositions(path);
        if (projectedPositions.Count == 0)
          return (List<int>) null;
        intList.Add(projectedPositions[0]);
      }
      return intList;
    }

    internal List<int> GetAssociationEndSlots(AssociationEndMember endMember)
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < this.m_projectedSlots.Length; ++index)
      {
        MemberProjectedSlot projectedSlot = this.m_projectedSlots[index] as MemberProjectedSlot;
        if (projectedSlot != null && projectedSlot.MemberPath.RootEdmMember.Equals((object) endMember))
          intList.Add(index);
      }
      return intList;
    }

    internal List<int> GetProjectedPositions(
      IEnumerable<MemberPath> paths,
      List<int> slotsToSearchFrom)
    {
      List<int> intList = new List<int>();
      foreach (MemberPath path in paths)
      {
        List<int> projectedPositions = this.GetProjectedPositions(path);
        if (projectedPositions.Count == 0)
          return (List<int>) null;
        int num = -1;
        if (projectedPositions.Count > 1)
        {
          for (int index = 0; index < projectedPositions.Count; ++index)
          {
            if (slotsToSearchFrom.Contains(projectedPositions[index]))
              num = projectedPositions[index];
          }
          if (num == -1)
            return (List<int>) null;
        }
        else
          num = projectedPositions[0];
        intList.Add(num);
      }
      return intList;
    }

    internal void UpdateWhereClause(MemberDomainMap domainMap)
    {
      List<BoolExpression> boolExpressionList = new List<BoolExpression>();
      foreach (BoolExpression atom in this.WhereClause.Atoms)
      {
        MemberRestriction asLiteral = atom.AsLiteral as MemberRestriction;
        IEnumerable<Constant> domain = domainMap.GetDomain(asLiteral.RestrictedMemberSlot.MemberPath);
        MemberRestriction memberRestriction = asLiteral.CreateCompleteMemberRestriction(domain);
        ScalarRestriction scalarRestriction = asLiteral as ScalarRestriction;
        bool flag = scalarRestriction != null && !scalarRestriction.Domain.Contains(Constant.Null) && !scalarRestriction.Domain.Contains(Constant.NotNull) && !scalarRestriction.Domain.Contains(Constant.Undefined);
        if (flag)
          domainMap.AddSentinel(memberRestriction.RestrictedMemberSlot.MemberPath);
        boolExpressionList.Add(BoolExpression.CreateLiteral((BoolLiteral) memberRestriction, domainMap));
        if (flag)
          domainMap.RemoveSentinel(memberRestriction.RestrictedMemberSlot.MemberPath);
      }
      if (boolExpressionList.Count <= 0)
        return;
      this.m_whereClause = BoolExpression.CreateAnd(boolExpressionList.ToArray());
    }

    internal BoolExpression GetBoolVar(int varNum)
    {
      return this.m_boolExprs[varNum];
    }

    internal void InitializeBoolExpressions(int numBoolVars, int cellNum)
    {
      this.m_boolExprs = new List<BoolExpression>(numBoolVars);
      for (int index = 0; index < numBoolVars; ++index)
        this.m_boolExprs.Add((BoolExpression) null);
      this.m_boolExprs[cellNum] = BoolExpression.True;
    }

    internal IEnumerable<MemberRestriction> GetConjunctsFromWhereClause()
    {
      return CellQuery.GetConjunctsFromWhereClause(this.m_whereClause);
    }

    internal IEnumerable<MemberRestriction> GetConjunctsFromOriginalWhereClause()
    {
      return CellQuery.GetConjunctsFromWhereClause(this.m_originalWhereClause);
    }

    private static IEnumerable<MemberRestriction> GetConjunctsFromWhereClause(
      BoolExpression whereClause)
    {
      foreach (BoolExpression atom in whereClause.Atoms)
      {
        if (!atom.IsTrue)
        {
          MemberRestriction result = atom.AsLiteral as MemberRestriction;
          yield return result;
        }
      }
    }

    internal void GetIdentifiers(CqlIdentifiers identifiers)
    {
      foreach (ProjectedSlot projectedSlot in this.m_projectedSlots)
        (projectedSlot as MemberProjectedSlot)?.MemberPath.GetIdentifiers(identifiers);
      this.m_extentMemberPath.GetIdentifiers(identifiers);
    }

    internal void CreateBasicCellRelation(ViewCellRelation viewCellRelation)
    {
      List<MemberProjectedSlot> allQuerySlots = this.GetAllQuerySlots();
      this.m_basicCellRelation = new BasicCellRelation(this, viewCellRelation, (IEnumerable<MemberProjectedSlot>) allQuerySlots);
    }

    internal override void ToCompactString(StringBuilder stringBuilder)
    {
      List<BoolExpression> boolExprs = this.m_boolExprs;
      int num = 0;
      bool flag = true;
      foreach (BoolExpression boolExpression in boolExprs)
      {
        if (boolExpression != null)
        {
          if (!flag)
            stringBuilder.Append(",");
          else
            stringBuilder.Append("[");
          StringUtil.FormatStringBuilder(stringBuilder, "C{0}", (object) num);
          flag = false;
        }
        ++num;
      }
      if (flag)
        this.ToFullString(stringBuilder);
      else
        stringBuilder.Append("]");
    }

    internal override void ToFullString(StringBuilder builder)
    {
      builder.Append("SELECT ");
      if (this.m_selectDistinct == CellQuery.SelectDistinct.Yes)
        builder.Append("DISTINCT ");
      StringUtil.ToSeparatedString(builder, (IEnumerable) this.m_projectedSlots, ", ", "_");
      if (this.m_boolExprs.Count > 0)
      {
        builder.Append(", Bool[");
        StringUtil.ToSeparatedString(builder, (IEnumerable) this.m_boolExprs, ", ", "_");
        builder.Append("]");
      }
      builder.Append(" FROM ");
      this.m_extentMemberPath.ToFullString(builder);
      if (this.m_whereClause.IsTrue)
        return;
      builder.Append(" WHERE ");
      this.m_whereClause.ToFullString(builder);
    }

    public override string ToString()
    {
      return this.ToFullString();
    }

    internal enum SelectDistinct
    {
      Yes,
      No,
    }
  }
}
