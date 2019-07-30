// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.OpCellTreeNode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Resources;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class OpCellTreeNode : CellTreeNode
  {
    private readonly Set<MemberPath> m_attrs;
    private readonly List<CellTreeNode> m_children;
    private readonly CellTreeOpType m_opType;
    private FragmentQuery m_leftFragmentQuery;
    private FragmentQuery m_rightFragmentQuery;

    internal OpCellTreeNode(ViewgenContext context, CellTreeOpType opType)
      : base(context)
    {
      this.m_opType = opType;
      this.m_attrs = new Set<MemberPath>(MemberPath.EqualityComparer);
      this.m_children = new List<CellTreeNode>();
    }

    internal OpCellTreeNode(
      ViewgenContext context,
      CellTreeOpType opType,
      params CellTreeNode[] children)
      : this(context, opType, (IEnumerable<CellTreeNode>) children)
    {
    }

    internal OpCellTreeNode(
      ViewgenContext context,
      CellTreeOpType opType,
      IEnumerable<CellTreeNode> children)
      : this(context, opType)
    {
      foreach (CellTreeNode child in children)
        this.Add(child);
    }

    internal override CellTreeOpType OpType
    {
      get
      {
        return this.m_opType;
      }
    }

    internal override FragmentQuery LeftFragmentQuery
    {
      get
      {
        if (this.m_leftFragmentQuery == null)
          this.m_leftFragmentQuery = OpCellTreeNode.GenerateFragmentQuery((IEnumerable<CellTreeNode>) this.Children, true, this.ViewgenContext, this.OpType);
        return this.m_leftFragmentQuery;
      }
    }

    internal override FragmentQuery RightFragmentQuery
    {
      get
      {
        if (this.m_rightFragmentQuery == null)
          this.m_rightFragmentQuery = OpCellTreeNode.GenerateFragmentQuery((IEnumerable<CellTreeNode>) this.Children, false, this.ViewgenContext, this.OpType);
        return this.m_rightFragmentQuery;
      }
    }

    internal override MemberDomainMap RightDomainMap
    {
      get
      {
        return this.m_children[0].RightDomainMap;
      }
    }

    internal override Set<MemberPath> Attributes
    {
      get
      {
        return this.m_attrs;
      }
    }

    internal override List<CellTreeNode> Children
    {
      get
      {
        return this.m_children;
      }
    }

    internal override int NumProjectedSlots
    {
      get
      {
        return this.m_children[0].NumProjectedSlots;
      }
    }

    internal override int NumBoolSlots
    {
      get
      {
        return this.m_children[0].NumBoolSlots;
      }
    }

    internal override TOutput Accept<TInput, TOutput>(
      CellTreeNode.SimpleCellTreeVisitor<TInput, TOutput> visitor,
      TInput param)
    {
      return visitor.VisitOpNode(this, param);
    }

    internal override TOutput Accept<TInput, TOutput>(
      CellTreeNode.CellTreeVisitor<TInput, TOutput> visitor,
      TInput param)
    {
      switch (this.OpType)
      {
        case CellTreeOpType.Union:
          return visitor.VisitUnion(this, param);
        case CellTreeOpType.FOJ:
          return visitor.VisitFullOuterJoin(this, param);
        case CellTreeOpType.LOJ:
          return visitor.VisitLeftOuterJoin(this, param);
        case CellTreeOpType.IJ:
          return visitor.VisitInnerJoin(this, param);
        case CellTreeOpType.LASJ:
          return visitor.VisitLeftAntiSemiJoin(this, param);
        default:
          return visitor.VisitInnerJoin(this, param);
      }
    }

    internal void Add(CellTreeNode child)
    {
      this.Insert(this.m_children.Count, child);
    }

    internal void AddFirst(CellTreeNode child)
    {
      this.Insert(0, child);
    }

    private void Insert(int index, CellTreeNode child)
    {
      this.m_attrs.Unite((IEnumerable<MemberPath>) child.Attributes);
      this.m_children.Insert(index, child);
      this.m_leftFragmentQuery = (FragmentQuery) null;
      this.m_rightFragmentQuery = (FragmentQuery) null;
    }

    internal override CqlBlock ToCqlBlock(
      bool[] requiredSlots,
      CqlIdentifiers identifiers,
      ref int blockAliasNum,
      ref List<WithRelationship> withRelationships)
    {
      return this.OpType != CellTreeOpType.Union ? this.JoinToCqlBlock(requiredSlots, identifiers, ref blockAliasNum, ref withRelationships) : this.UnionToCqlBlock(requiredSlots, identifiers, ref blockAliasNum, ref withRelationships);
    }

    internal override bool IsProjectedSlot(int slot)
    {
      foreach (CellTreeNode child in this.Children)
      {
        if (child.IsProjectedSlot(slot))
          return true;
      }
      return false;
    }

    private CqlBlock UnionToCqlBlock(
      bool[] requiredSlots,
      CqlIdentifiers identifiers,
      ref int blockAliasNum,
      ref List<WithRelationship> withRelationships)
    {
      List<CqlBlock> children = new List<CqlBlock>();
      List<Tuple<CqlBlock, SlotInfo>> tupleList = new List<Tuple<CqlBlock, SlotInfo>>();
      int length1 = requiredSlots.Length;
      foreach (CellTreeNode child in this.Children)
      {
        bool[] projectedSlots = child.GetProjectedSlots();
        OpCellTreeNode.AndWith(projectedSlots, requiredSlots);
        CqlBlock cqlBlock = child.ToCqlBlock(projectedSlots, identifiers, ref blockAliasNum, ref withRelationships);
        for (int length2 = projectedSlots.Length; length2 < cqlBlock.Slots.Count; ++length2)
          tupleList.Add(Tuple.Create<CqlBlock, SlotInfo>(cqlBlock, cqlBlock.Slots[length2]));
        SlotInfo[] slotInfoArray = new SlotInfo[cqlBlock.Slots.Count];
        for (int slotNum = 0; slotNum < length1; ++slotNum)
        {
          if (requiredSlots[slotNum] && !projectedSlots[slotNum])
          {
            if (this.IsBoolSlot(slotNum))
            {
              slotInfoArray[slotNum] = new SlotInfo(true, true, (ProjectedSlot) new BooleanProjectedSlot(BoolExpression.False, identifiers, this.SlotToBoolIndex(slotNum)), (MemberPath) null);
            }
            else
            {
              MemberPath outputMember = cqlBlock.MemberPath(slotNum);
              slotInfoArray[slotNum] = new SlotInfo(true, true, (ProjectedSlot) new ConstantProjectedSlot(Constant.Null), outputMember);
            }
          }
          else
            slotInfoArray[slotNum] = cqlBlock.Slots[slotNum];
        }
        cqlBlock.Slots = new ReadOnlyCollection<SlotInfo>((IList<SlotInfo>) slotInfoArray);
        children.Add(cqlBlock);
      }
      if (tupleList.Count != 0)
      {
        foreach (CqlBlock cqlBlock in children)
        {
          SlotInfo[] array = new SlotInfo[length1 + tupleList.Count];
          cqlBlock.Slots.CopyTo(array, 0);
          int index = length1;
          foreach (Tuple<CqlBlock, SlotInfo> tuple in tupleList)
          {
            SlotInfo slotInfo = tuple.Item2;
            array[index] = !tuple.Item1.Equals((object) cqlBlock) ? new SlotInfo(true, true, (ProjectedSlot) new ConstantProjectedSlot(Constant.Null), slotInfo.OutputMember) : new SlotInfo(true, true, slotInfo.SlotValue, slotInfo.OutputMember);
            ++index;
          }
          cqlBlock.Slots = new ReadOnlyCollection<SlotInfo>((IList<SlotInfo>) array);
        }
      }
      SlotInfo[] slotInfos = new SlotInfo[length1 + tupleList.Count];
      CqlBlock cqlBlock1 = children[0];
      for (int index = 0; index < length1; ++index)
      {
        SlotInfo slot = cqlBlock1.Slots[index];
        bool requiredSlot = requiredSlots[index];
        slotInfos[index] = new SlotInfo(requiredSlot, requiredSlot, slot.SlotValue, slot.OutputMember);
      }
      for (int index = length1; index < length1 + tupleList.Count; ++index)
      {
        SlotInfo slot = cqlBlock1.Slots[index];
        slotInfos[index] = new SlotInfo(true, true, slot.SlotValue, slot.OutputMember);
      }
      return (CqlBlock) new UnionCqlBlock(slotInfos, children, identifiers, ++blockAliasNum);
    }

    private static void AndWith(bool[] boolArray, bool[] another)
    {
      for (int index = 0; index < boolArray.Length; ++index)
        boolArray[index] &= another[index];
    }

    private CqlBlock JoinToCqlBlock(
      bool[] requiredSlots,
      CqlIdentifiers identifiers,
      ref int blockAliasNum,
      ref List<WithRelationship> withRelationships)
    {
      int length1 = requiredSlots.Length;
      List<CqlBlock> children = new List<CqlBlock>();
      List<Tuple<QualifiedSlot, MemberPath>> tupleList = new List<Tuple<QualifiedSlot, MemberPath>>();
      foreach (CellTreeNode child in this.Children)
      {
        bool[] projectedSlots = child.GetProjectedSlots();
        OpCellTreeNode.AndWith(projectedSlots, requiredSlots);
        CqlBlock cqlBlock = child.ToCqlBlock(projectedSlots, identifiers, ref blockAliasNum, ref withRelationships);
        children.Add(cqlBlock);
        for (int length2 = projectedSlots.Length; length2 < cqlBlock.Slots.Count; ++length2)
          tupleList.Add(Tuple.Create<QualifiedSlot, MemberPath>(cqlBlock.QualifySlotWithBlockAlias(length2), cqlBlock.MemberPath(length2)));
      }
      SlotInfo[] slotInfos = new SlotInfo[length1 + tupleList.Count];
      for (int slotNum = 0; slotNum < length1; ++slotNum)
      {
        SlotInfo joinSlotInfo = this.GetJoinSlotInfo(this.OpType, requiredSlots[slotNum], children, slotNum, identifiers);
        slotInfos[slotNum] = joinSlotInfo;
      }
      int index1 = 0;
      int index2 = length1;
      while (index2 < length1 + tupleList.Count)
      {
        slotInfos[index2] = new SlotInfo(true, true, (ProjectedSlot) tupleList[index1].Item1, tupleList[index1].Item2);
        ++index2;
        ++index1;
      }
      List<JoinCqlBlock.OnClause> onClauses = new List<JoinCqlBlock.OnClause>();
      for (int index3 = 1; index3 < children.Count; ++index3)
      {
        CqlBlock cqlBlock = children[index3];
        JoinCqlBlock.OnClause onClause = new JoinCqlBlock.OnClause();
        foreach (int keySlot in this.KeySlots)
        {
          if (!this.ViewgenContext.Config.IsValidationEnabled && (!cqlBlock.IsProjected(keySlot) || !children[0].IsProjected(keySlot)))
          {
            ErrorLog errorLog = new ErrorLog();
            errorLog.AddEntry(new ErrorLog.Record(ViewGenErrorCode.NoJoinKeyOrFKProvidedInMapping, Strings.Viewgen_NoJoinKeyOrFK, (IEnumerable<LeftCellWrapper>) this.ViewgenContext.AllWrappersForExtent, string.Empty));
            ExceptionHelpers.ThrowMappingException(errorLog, this.ViewgenContext.Config);
          }
          QualifiedSlot leftSlot = children[0].QualifySlotWithBlockAlias(keySlot);
          QualifiedSlot rightSlot = cqlBlock.QualifySlotWithBlockAlias(keySlot);
          MemberPath outputMember = slotInfos[keySlot].OutputMember;
          onClause.Add(leftSlot, outputMember, rightSlot, outputMember);
        }
        onClauses.Add(onClause);
      }
      return (CqlBlock) new JoinCqlBlock(this.OpType, slotInfos, children, onClauses, identifiers, ++blockAliasNum);
    }

    private SlotInfo GetJoinSlotInfo(
      CellTreeOpType opType,
      bool isRequiredSlot,
      List<CqlBlock> children,
      int slotNum,
      CqlIdentifiers identifiers)
    {
      if (!isRequiredSlot)
        return new SlotInfo(false, false, (ProjectedSlot) null, this.GetMemberPath(slotNum));
      int index1 = -1;
      CaseStatement caseStatement = (CaseStatement) null;
      for (int index2 = 0; index2 < children.Count; ++index2)
      {
        CqlBlock child = children[index2];
        if (child.IsProjected(slotNum))
        {
          if (this.IsKeySlot(slotNum))
          {
            index1 = index2;
            break;
          }
          if (opType == CellTreeOpType.IJ)
          {
            index1 = OpCellTreeNode.GetInnerJoinChildForSlot(children, slotNum);
            break;
          }
          if (index1 != -1)
          {
            if (caseStatement == null)
            {
              caseStatement = new CaseStatement(this.GetMemberPath(slotNum));
              this.AddCaseForOuterJoins(caseStatement, children[index1], slotNum, identifiers);
            }
            this.AddCaseForOuterJoins(caseStatement, child, slotNum, identifiers);
          }
          index1 = index2;
        }
      }
      MemberPath memberPath = this.GetMemberPath(slotNum);
      ProjectedSlot slotValue;
      if (caseStatement != null && (caseStatement.Clauses.Count > 0 || caseStatement.ElseValue != null))
      {
        caseStatement.Simplify();
        slotValue = (ProjectedSlot) new CaseStatementProjectedSlot(caseStatement, (IEnumerable<WithRelationship>) null);
      }
      else
        slotValue = index1 < 0 ? (!this.IsBoolSlot(slotNum) ? (ProjectedSlot) new ConstantProjectedSlot(Domain.GetDefaultValueForMemberPath(memberPath, (IEnumerable<LeftCellWrapper>) this.GetLeaves(), this.ViewgenContext.Config)) : (ProjectedSlot) new BooleanProjectedSlot(BoolExpression.False, identifiers, this.SlotToBoolIndex(slotNum))) : (ProjectedSlot) children[index1].QualifySlotWithBlockAlias(slotNum);
      bool enforceNotNull = this.IsBoolSlot(slotNum) && (opType == CellTreeOpType.LOJ && index1 > 0 || opType == CellTreeOpType.FOJ);
      return new SlotInfo(true, true, slotValue, memberPath, enforceNotNull);
    }

    private static int GetInnerJoinChildForSlot(List<CqlBlock> children, int slotNum)
    {
      int num = -1;
      for (int index = 0; index < children.Count; ++index)
      {
        CqlBlock child = children[index];
        if (child.IsProjected(slotNum))
        {
          ProjectedSlot projectedSlot = child.SlotValue(slotNum);
          ConstantProjectedSlot constantProjectedSlot = projectedSlot as ConstantProjectedSlot;
          if (projectedSlot is MemberProjectedSlot)
            num = index;
          else if (constantProjectedSlot != null && constantProjectedSlot.CellConstant.IsNull())
          {
            if (num == -1)
              num = index;
          }
          else
            num = index;
        }
      }
      return num;
    }

    private void AddCaseForOuterJoins(
      CaseStatement caseForOuterJoins,
      CqlBlock child,
      int slotNum,
      CqlIdentifiers identifiers)
    {
      ConstantProjectedSlot constantProjectedSlot = child.SlotValue(slotNum) as ConstantProjectedSlot;
      if (constantProjectedSlot != null && constantProjectedSlot.CellConstant.IsNull())
        return;
      BoolExpression or = BoolExpression.False;
      for (int index = 0; index < this.NumBoolSlots; ++index)
      {
        int slot = this.BoolIndexToSlot(index);
        if (child.IsProjected(slot))
        {
          QualifiedCellIdBoolean qualifiedCellIdBoolean = new QualifiedCellIdBoolean(child, identifiers, index);
          or = BoolExpression.CreateOr(or, BoolExpression.CreateLiteral((BoolLiteral) qualifiedCellIdBoolean, this.RightDomainMap));
        }
      }
      QualifiedSlot qualifiedSlot = child.QualifySlotWithBlockAlias(slotNum);
      caseForOuterJoins.AddWhenThen(or, (ProjectedSlot) qualifiedSlot);
    }

    private static FragmentQuery GenerateFragmentQuery(
      IEnumerable<CellTreeNode> children,
      bool isLeft,
      ViewgenContext context,
      CellTreeOpType OpType)
    {
      FragmentQuery fragmentQuery1 = isLeft ? children.First<CellTreeNode>().LeftFragmentQuery : children.First<CellTreeNode>().RightFragmentQuery;
      FragmentQueryProcessor fragmentQueryProcessor = isLeft ? context.LeftFragmentQP : context.RightFragmentQP;
      foreach (CellTreeNode cellTreeNode in children.Skip<CellTreeNode>(1))
      {
        FragmentQuery fragmentQuery2 = isLeft ? cellTreeNode.LeftFragmentQuery : cellTreeNode.RightFragmentQuery;
        switch (OpType)
        {
          case CellTreeOpType.LOJ:
            continue;
          case CellTreeOpType.IJ:
            fragmentQuery1 = fragmentQueryProcessor.Intersect(fragmentQuery1, fragmentQuery2);
            continue;
          case CellTreeOpType.LASJ:
            fragmentQuery1 = fragmentQueryProcessor.Difference(fragmentQuery1, fragmentQuery2);
            continue;
          default:
            fragmentQuery1 = fragmentQueryProcessor.Union(fragmentQuery1, fragmentQuery2);
            continue;
        }
      }
      return fragmentQuery1;
    }

    internal static string OpToEsql(CellTreeOpType opType)
    {
      switch (opType)
      {
        case CellTreeOpType.Union:
          return "UNION ALL";
        case CellTreeOpType.FOJ:
          return "FULL OUTER JOIN";
        case CellTreeOpType.LOJ:
          return "LEFT OUTER JOIN";
        case CellTreeOpType.IJ:
          return "INNER JOIN";
        default:
          return (string) null;
      }
    }

    internal override void ToCompactString(StringBuilder stringBuilder)
    {
      stringBuilder.Append("(");
      for (int index = 0; index < this.m_children.Count; ++index)
      {
        this.m_children[index].ToCompactString(stringBuilder);
        if (index != this.m_children.Count - 1)
          StringUtil.FormatStringBuilder(stringBuilder, " {0} ", (object) this.OpType);
      }
      stringBuilder.Append(")");
    }
  }
}
