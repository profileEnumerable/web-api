// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Validation.RewritingValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Resources;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Validation
{
  internal class RewritingValidator
  {
    private readonly ViewgenContext _viewgenContext;
    private readonly MemberDomainMap _domainMap;
    private readonly CellTreeNode _basicView;
    private readonly IEnumerable<MemberPath> _keyAttributes;
    private readonly ErrorLog _errorLog;

    internal RewritingValidator(ViewgenContext context, CellTreeNode basicView)
    {
      this._viewgenContext = context;
      this._basicView = basicView;
      this._domainMap = this._viewgenContext.MemberMaps.UpdateDomainMap;
      this._keyAttributes = MemberPath.GetKeyMembers(this._viewgenContext.Extent, this._domainMap);
      this._errorLog = new ErrorLog();
    }

    internal void Validate()
    {
      Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode> memberValueTrees1 = this.CreateMemberValueTrees(false);
      Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode> memberValueTrees2 = this.CreateMemberValueTrees(true);
      RewritingValidator.WhereClauseVisitor whereClauseVisitor1 = new RewritingValidator.WhereClauseVisitor(this._basicView, memberValueTrees1);
      RewritingValidator.WhereClauseVisitor whereClauseVisitor2 = new RewritingValidator.WhereClauseVisitor(this._basicView, memberValueTrees2);
      foreach (LeftCellWrapper leftCellWrapper in this._viewgenContext.AllWrappersForExtent)
      {
        Cell onlyInputCell = leftCellWrapper.OnlyInputCell;
        CellTreeNode cellTreeNode1 = (CellTreeNode) new LeafCellTreeNode(this._viewgenContext, leftCellWrapper);
        CellTreeNode cellTreeNode2 = whereClauseVisitor2.GetCellTreeNode(onlyInputCell.SQuery.WhereClause);
        if (cellTreeNode2 != null)
        {
          CellTreeNode sQueryTree;
          if (cellTreeNode2 != this._basicView)
            sQueryTree = (CellTreeNode) new OpCellTreeNode(this._viewgenContext, CellTreeOpType.IJ, new CellTreeNode[2]
            {
              cellTreeNode2,
              this._basicView
            });
          else
            sQueryTree = this._basicView;
          BoolExpression literal = BoolExpression.CreateLiteral((BoolLiteral) leftCellWrapper.CreateRoleBoolean(), this._viewgenContext.MemberMaps.QueryDomainMap);
          BoolExpression unsatisfiedConstraint;
          if (!this.CheckEquivalence(cellTreeNode1.RightFragmentQuery, sQueryTree.RightFragmentQuery, literal, out unsatisfiedConstraint))
          {
            string str = StringUtil.FormatInvariant("{0}", (object) this._viewgenContext.Extent);
            cellTreeNode1.RightFragmentQuery.Condition.ExpensiveSimplify();
            sQueryTree.RightFragmentQuery.Condition.ExpensiveSimplify();
            this.ReportConstraintViolation(Strings.ViewGen_CQ_PartitionConstraint((object) str), unsatisfiedConstraint, ViewGenErrorCode.PartitionConstraintViolation, cellTreeNode1.GetLeaves().Concat<LeftCellWrapper>((IEnumerable<LeftCellWrapper>) sQueryTree.GetLeaves()));
          }
          CellTreeNode cellTreeNode3 = whereClauseVisitor1.GetCellTreeNode(onlyInputCell.SQuery.WhereClause);
          if (cellTreeNode3 != null)
          {
            RewritingValidator.DomainConstraintVisitor.CheckConstraints(cellTreeNode3, leftCellWrapper, this._viewgenContext, this._errorLog);
            if (this._errorLog.Count <= 0)
            {
              this.CheckConstraintsOnProjectedConditionMembers(memberValueTrees1, leftCellWrapper, sQueryTree, literal);
              if (this._errorLog.Count > 0)
                continue;
            }
            else
              continue;
          }
          this.CheckConstraintsOnNonNullableMembers(leftCellWrapper);
        }
      }
      if (this._errorLog.Count <= 0)
        return;
      ExceptionHelpers.ThrowMappingException(this._errorLog, this._viewgenContext.Config);
    }

    private bool CheckEquivalence(
      FragmentQuery cQuery,
      FragmentQuery sQuery,
      BoolExpression inExtentCondition,
      out BoolExpression unsatisfiedConstraint)
    {
      FragmentQuery fragmentQuery1 = this._viewgenContext.RightFragmentQP.Difference(cQuery, sQuery);
      FragmentQuery fragmentQuery2 = this._viewgenContext.RightFragmentQP.Difference(sQuery, cQuery);
      FragmentQuery query1 = FragmentQuery.Create(BoolExpression.CreateAnd(fragmentQuery1.Condition, inExtentCondition));
      FragmentQuery query2 = FragmentQuery.Create(BoolExpression.CreateAnd(fragmentQuery2.Condition, inExtentCondition));
      unsatisfiedConstraint = (BoolExpression) null;
      bool flag1 = true;
      bool flag2 = true;
      if (this._viewgenContext.RightFragmentQP.IsSatisfiable(query1))
      {
        unsatisfiedConstraint = query1.Condition;
        flag1 = false;
      }
      if (this._viewgenContext.RightFragmentQP.IsSatisfiable(query2))
      {
        unsatisfiedConstraint = query2.Condition;
        flag2 = false;
      }
      if (flag1 && flag2)
        return true;
      unsatisfiedConstraint.ExpensiveSimplify();
      return false;
    }

    private void ReportConstraintViolation(
      string message,
      BoolExpression extraConstraint,
      ViewGenErrorCode errorCode,
      IEnumerable<LeftCellWrapper> relevantWrappers)
    {
      if (ErrorPatternMatcher.FindMappingErrors(this._viewgenContext, this._domainMap, this._errorLog))
        return;
      extraConstraint.ExpensiveSimplify();
      HashSet<LeftCellWrapper> leftCellWrapperSet = new HashSet<LeftCellWrapper>(relevantWrappers);
      new List<LeftCellWrapper>((IEnumerable<LeftCellWrapper>) leftCellWrapperSet).Sort(LeftCellWrapper.OriginalCellIdComparer);
      StringBuilder builder = new StringBuilder();
      builder.AppendLine(message);
      RewritingValidator.EntityConfigurationToUserString(extraConstraint, builder);
      this._errorLog.AddEntry(new ErrorLog.Record(errorCode, builder.ToString(), (IEnumerable<LeftCellWrapper>) leftCellWrapperSet, ""));
    }

    private Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode> CreateMemberValueTrees(
      bool complementElse)
    {
      Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode> dictionary = new Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode>();
      foreach (MemberPath conditionMember in this._domainMap.ConditionMembers(this._viewgenContext.Extent))
      {
        List<Constant> constantList = new List<Constant>(this._domainMap.GetDomain(conditionMember));
        OpCellTreeNode opCellTreeNode = new OpCellTreeNode(this._viewgenContext, CellTreeOpType.Union);
        for (int index1 = 0; index1 < constantList.Count; ++index1)
        {
          Constant domainValue = constantList[index1];
          RewritingValidator.MemberValueBinding index2 = new RewritingValidator.MemberValueBinding(conditionMember, domainValue);
          Tile<FragmentQuery> rewriting;
          if (this._viewgenContext.TryGetCachedRewriting(QueryRewriter.CreateMemberConditionQuery(conditionMember, domainValue, this._keyAttributes, this._domainMap), out rewriting))
          {
            CellTreeNode cellTree = QueryRewriter.TileToCellTree(rewriting, this._viewgenContext);
            dictionary[index2] = cellTree;
            if (index1 < constantList.Count - 1)
              opCellTreeNode.Add(cellTree);
          }
        }
        if (complementElse && constantList.Count > 1)
        {
          Constant constant = constantList[constantList.Count - 1];
          RewritingValidator.MemberValueBinding index = new RewritingValidator.MemberValueBinding(conditionMember, constant);
          dictionary[index] = (CellTreeNode) new OpCellTreeNode(this._viewgenContext, CellTreeOpType.LASJ, new CellTreeNode[2]
          {
            this._basicView,
            (CellTreeNode) opCellTreeNode
          });
        }
      }
      return dictionary;
    }

    private void CheckConstraintsOnProjectedConditionMembers(
      Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode> memberValueTrees,
      LeftCellWrapper wrapper,
      CellTreeNode sQueryTree,
      BoolExpression inExtentCondition)
    {
      foreach (MemberPath conditionMember in this._domainMap.ConditionMembers(this._viewgenContext.Extent))
      {
        int slotNum = this._viewgenContext.MemberMaps.ProjectedSlotMap.IndexOf(conditionMember);
        MemberProjectedSlot memberProjectedSlot = wrapper.RightCellQuery.ProjectedSlotAt(slotNum) as MemberProjectedSlot;
        if (memberProjectedSlot != null)
        {
          foreach (Constant constant in this._domainMap.GetDomain(conditionMember))
          {
            CellTreeNode cellTreeNode1;
            if (memberValueTrees.TryGetValue(new RewritingValidator.MemberValueBinding(conditionMember, constant), out cellTreeNode1))
            {
              FragmentQuery cQuery = FragmentQuery.Create(RewritingValidator.PropagateCellConstantsToWhereClause(wrapper, wrapper.RightCellQuery.WhereClause, constant, conditionMember, this._viewgenContext.MemberMaps));
              CellTreeNode cellTreeNode2;
              if (sQueryTree != this._basicView)
                cellTreeNode2 = (CellTreeNode) new OpCellTreeNode(this._viewgenContext, CellTreeOpType.IJ, new CellTreeNode[2]
                {
                  cellTreeNode1,
                  sQueryTree
                });
              else
                cellTreeNode2 = cellTreeNode1;
              CellTreeNode cellTreeNode3 = cellTreeNode2;
              BoolExpression unsatisfiedConstraint;
              if (!this.CheckEquivalence(cQuery, cellTreeNode3.RightFragmentQuery, inExtentCondition, out unsatisfiedConstraint))
                this.ReportConstraintViolation(Strings.ViewGen_CQ_DomainConstraint((object) memberProjectedSlot.ToUserString()), unsatisfiedConstraint, ViewGenErrorCode.DomainConstraintViolation, cellTreeNode3.GetLeaves().Concat<LeftCellWrapper>((IEnumerable<LeftCellWrapper>) new LeftCellWrapper[1]
                {
                  wrapper
                }));
            }
          }
        }
      }
    }

    internal static BoolExpression PropagateCellConstantsToWhereClause(
      LeftCellWrapper wrapper,
      BoolExpression expression,
      Constant constant,
      MemberPath member,
      MemberMaps memberMaps)
    {
      MemberProjectedSlot mappedSlotForSmember = wrapper.GetCSideMappedSlotForSMember(member);
      if (mappedSlotForSmember == null)
        return expression;
      NegatedConstant negatedConstant = constant as NegatedConstant;
      IEnumerable<Constant> domain = memberMaps.QueryDomainMap.GetDomain(mappedSlotForSmember.MemberPath);
      Set<Constant> set = new Set<Constant>(Constant.EqualityComparer);
      if (negatedConstant != null)
      {
        set.Unite(domain);
        set.Difference(negatedConstant.Elements);
      }
      else
        set.Add(constant);
      MemberRestriction memberRestriction = (MemberRestriction) new ScalarRestriction(mappedSlotForSmember.MemberPath, (IEnumerable<Constant>) set, domain);
      return BoolExpression.CreateAnd(expression, BoolExpression.CreateLiteral((BoolLiteral) memberRestriction, memberMaps.QueryDomainMap));
    }

    private static FragmentQuery AddNullConditionOnCSideFragment(
      LeftCellWrapper wrapper,
      MemberPath member,
      MemberMaps memberMaps)
    {
      MemberProjectedSlot mappedSlotForSmember = wrapper.GetCSideMappedSlotForSMember(member);
      if (mappedSlotForSmember == null || !mappedSlotForSmember.MemberPath.IsNullable)
        return (FragmentQuery) null;
      BoolExpression whereClause = wrapper.RightCellQuery.WhereClause;
      IEnumerable<Constant> domain = memberMaps.QueryDomainMap.GetDomain(mappedSlotForSmember.MemberPath);
      MemberRestriction memberRestriction = (MemberRestriction) new ScalarRestriction(mappedSlotForSmember.MemberPath, (IEnumerable<Constant>) new Set<Constant>(Constant.EqualityComparer)
      {
        Constant.Null
      }, domain);
      return FragmentQuery.Create(BoolExpression.CreateAnd(whereClause, BoolExpression.CreateLiteral((BoolLiteral) memberRestriction, memberMaps.QueryDomainMap)));
    }

    private void CheckConstraintsOnNonNullableMembers(LeftCellWrapper wrapper)
    {
      foreach (MemberPath nonConditionMember in this._domainMap.NonConditionMembers(this._viewgenContext.Extent))
      {
        bool flag = nonConditionMember.EdmType is System.Data.Entity.Core.Metadata.Edm.SimpleType;
        if (!nonConditionMember.IsNullable && flag)
        {
          FragmentQuery query = RewritingValidator.AddNullConditionOnCSideFragment(wrapper, nonConditionMember, this._viewgenContext.MemberMaps);
          if (query != null && this._viewgenContext.RightFragmentQP.IsSatisfiable(query))
            this._errorLog.AddEntry(new ErrorLog.Record(ViewGenErrorCode.NullableMappingForNonNullableColumn, Strings.Viewgen_NullableMappingForNonNullableColumn((object) wrapper.LeftExtent.ToString(), (object) nonConditionMember.ToFullString()), wrapper.Cells, ""));
        }
      }
    }

    internal static void EntityConfigurationToUserString(
      BoolExpression condition,
      StringBuilder builder)
    {
      RewritingValidator.EntityConfigurationToUserString(condition, builder, true);
    }

    internal static void EntityConfigurationToUserString(
      BoolExpression condition,
      StringBuilder builder,
      bool writeRoundTrippingMessage)
    {
      condition.AsUserString(builder, "PK", writeRoundTrippingMessage);
    }

    private class WhereClauseVisitor : Visitor<DomainConstraint<BoolLiteral, Constant>, CellTreeNode>
    {
      private readonly ViewgenContext _viewgenContext;
      private readonly CellTreeNode _topLevelTree;
      private readonly Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode> _memberValueTrees;

      internal WhereClauseVisitor(
        CellTreeNode topLevelTree,
        Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode> memberValueTrees)
      {
        this._topLevelTree = topLevelTree;
        this._memberValueTrees = memberValueTrees;
        this._viewgenContext = topLevelTree.ViewgenContext;
      }

      internal CellTreeNode GetCellTreeNode(BoolExpression whereClause)
      {
        return whereClause.Tree.Accept<CellTreeNode>((Visitor<DomainConstraint<BoolLiteral, Constant>, CellTreeNode>) this);
      }

      internal override CellTreeNode VisitAnd(
        AndExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        IEnumerable<CellTreeNode> cellTreeNodes = this.AcceptChildren((IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) expression.Children);
        OpCellTreeNode opCellTreeNode = new OpCellTreeNode(this._viewgenContext, CellTreeOpType.IJ);
        foreach (CellTreeNode child in cellTreeNodes)
        {
          if (child == null)
            return (CellTreeNode) null;
          if (child != this._topLevelTree)
            opCellTreeNode.Add(child);
        }
        if (opCellTreeNode.Children.Count != 0)
          return (CellTreeNode) opCellTreeNode;
        return this._topLevelTree;
      }

      internal override CellTreeNode VisitTrue(
        TrueExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this._topLevelTree;
      }

      internal override CellTreeNode VisitTerm(
        TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        MemberRestriction identifier = (MemberRestriction) expression.Identifier.Variable.Identifier;
        Set<Constant> range = expression.Identifier.Range;
        OpCellTreeNode opCellTreeNode = new OpCellTreeNode(this._viewgenContext, CellTreeOpType.Union);
        CellTreeNode singleNode = (CellTreeNode) null;
        foreach (Constant constant in range)
        {
          if (this.TryGetCellTreeNode(identifier.RestrictedMemberSlot.MemberPath, constant, out singleNode))
            opCellTreeNode.Add(singleNode);
        }
        switch (opCellTreeNode.Children.Count)
        {
          case 0:
            return (CellTreeNode) null;
          case 1:
            return singleNode;
          default:
            return (CellTreeNode) opCellTreeNode;
        }
      }

      internal override CellTreeNode VisitFalse(
        FalseExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        throw new NotImplementedException();
      }

      internal override CellTreeNode VisitNot(
        NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        throw new NotImplementedException();
      }

      internal override CellTreeNode VisitOr(
        OrExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        throw new NotImplementedException();
      }

      private bool TryGetCellTreeNode(
        MemberPath memberPath,
        Constant value,
        out CellTreeNode singleNode)
      {
        return this._memberValueTrees.TryGetValue(new RewritingValidator.MemberValueBinding(memberPath, value), out singleNode);
      }

      private IEnumerable<CellTreeNode> AcceptChildren(
        IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> children)
      {
        foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> child in children)
          yield return child.Accept<CellTreeNode>((Visitor<DomainConstraint<BoolLiteral, Constant>, CellTreeNode>) this);
      }
    }

    internal class DomainConstraintVisitor : CellTreeNode.SimpleCellTreeVisitor<bool, bool>
    {
      private readonly LeftCellWrapper m_wrapper;
      private readonly ViewgenContext m_viewgenContext;
      private readonly ErrorLog m_errorLog;

      private DomainConstraintVisitor(
        LeftCellWrapper wrapper,
        ViewgenContext context,
        ErrorLog errorLog)
      {
        this.m_wrapper = wrapper;
        this.m_viewgenContext = context;
        this.m_errorLog = errorLog;
      }

      internal static void CheckConstraints(
        CellTreeNode node,
        LeftCellWrapper wrapper,
        ViewgenContext context,
        ErrorLog errorLog)
      {
        RewritingValidator.DomainConstraintVisitor constraintVisitor = new RewritingValidator.DomainConstraintVisitor(wrapper, context, errorLog);
        node.Accept<bool, bool>((CellTreeNode.SimpleCellTreeVisitor<bool, bool>) constraintVisitor, true);
      }

      internal override bool VisitLeaf(LeafCellTreeNode node, bool dummy)
      {
        CellQuery rightCellQuery1 = this.m_wrapper.RightCellQuery;
        CellQuery rightCellQuery2 = node.LeftCellWrapper.RightCellQuery;
        List<MemberPath> memberPathList = new List<MemberPath>();
        if (rightCellQuery1 != rightCellQuery2)
        {
          for (int slotNum = 0; slotNum < rightCellQuery1.NumProjectedSlots; ++slotNum)
          {
            MemberProjectedSlot memberProjectedSlot1 = rightCellQuery1.ProjectedSlotAt(slotNum) as MemberProjectedSlot;
            if (memberProjectedSlot1 != null)
            {
              MemberProjectedSlot memberProjectedSlot2 = rightCellQuery2.ProjectedSlotAt(slotNum) as MemberProjectedSlot;
              if (memberProjectedSlot2 != null)
              {
                MemberPath projectedSlot = this.m_viewgenContext.MemberMaps.ProjectedSlotMap[slotNum];
                if (!projectedSlot.IsPartOfKey && !MemberPath.EqualityComparer.Equals(memberProjectedSlot1.MemberPath, memberProjectedSlot2.MemberPath))
                  memberPathList.Add(projectedSlot);
              }
            }
          }
        }
        if (memberPathList.Count > 0)
          this.m_errorLog.AddEntry(new ErrorLog.Record(ViewGenErrorCode.NonKeyProjectedWithOverlappingPartitions, Strings.ViewGen_NonKeyProjectedWithOverlappingPartitions((object) MemberPath.PropertiesToUserString((IEnumerable<MemberPath>) memberPathList, false)), (IEnumerable<LeftCellWrapper>) new LeftCellWrapper[2]
          {
            this.m_wrapper,
            node.LeftCellWrapper
          }, string.Empty));
        return true;
      }

      internal override bool VisitOpNode(OpCellTreeNode node, bool dummy)
      {
        if (node.OpType == CellTreeOpType.LASJ)
        {
          node.Children[0].Accept<bool, bool>((CellTreeNode.SimpleCellTreeVisitor<bool, bool>) this, dummy);
        }
        else
        {
          foreach (CellTreeNode child in node.Children)
            child.Accept<bool, bool>((CellTreeNode.SimpleCellTreeVisitor<bool, bool>) this, dummy);
        }
        return true;
      }
    }

    private struct MemberValueBinding : IEquatable<RewritingValidator.MemberValueBinding>
    {
      internal readonly MemberPath Member;
      internal readonly Constant Value;

      public MemberValueBinding(MemberPath member, Constant value)
      {
        this.Member = member;
        this.Value = value;
      }

      public override string ToString()
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) this.Member, (object) this.Value);
      }

      public bool Equals(RewritingValidator.MemberValueBinding other)
      {
        if (MemberPath.EqualityComparer.Equals(this.Member, other.Member))
          return Constant.EqualityComparer.Equals(this.Value, other.Value);
        return false;
      }
    }
  }
}
