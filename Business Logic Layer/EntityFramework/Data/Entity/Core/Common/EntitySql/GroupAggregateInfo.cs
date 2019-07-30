// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.GroupAggregateInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.EntitySql.AST;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal abstract class GroupAggregateInfo
  {
    private ScopeRegion _innermostReferencedScopeRegion;
    private List<GroupAggregateInfo> _containedAggregates;
    internal readonly GroupAggregateKind AggregateKind;
    internal readonly GroupAggregateExpr AstNode;
    internal readonly ErrorContext ErrCtx;
    internal readonly ScopeRegion DefiningScopeRegion;
    private ScopeRegion _evaluatingScopeRegion;
    private GroupAggregateInfo _containingAggregate;
    internal string AggregateName;
    internal DbNullExpression AggregateStubExpression;

    protected GroupAggregateInfo(
      GroupAggregateKind aggregateKind,
      GroupAggregateExpr astNode,
      ErrorContext errCtx,
      GroupAggregateInfo containingAggregate,
      ScopeRegion definingScopeRegion)
    {
      this.AggregateKind = aggregateKind;
      this.AstNode = astNode;
      this.ErrCtx = errCtx;
      this.DefiningScopeRegion = definingScopeRegion;
      this.SetContainingAggregate(containingAggregate);
    }

    protected void AttachToAstNode(string aggregateName, TypeUsage resultType)
    {
      this.AggregateName = aggregateName;
      this.AggregateStubExpression = resultType.Null();
      this.AstNode.AggregateInfo = this;
    }

    internal void DetachFromAstNode()
    {
      this.AstNode.AggregateInfo = (GroupAggregateInfo) null;
    }

    internal void UpdateScopeIndex(int referencedScopeIndex, SemanticResolver sr)
    {
      ScopeRegion definingScopeRegion = sr.GetDefiningScopeRegion(referencedScopeIndex);
      if (this._innermostReferencedScopeRegion != null && this._innermostReferencedScopeRegion.ScopeRegionIndex >= definingScopeRegion.ScopeRegionIndex)
        return;
      this._innermostReferencedScopeRegion = definingScopeRegion;
    }

    internal ScopeRegion InnermostReferencedScopeRegion
    {
      get
      {
        return this._innermostReferencedScopeRegion;
      }
      set
      {
        this._innermostReferencedScopeRegion = value;
      }
    }

    internal void ValidateAndComputeEvaluatingScopeRegion(SemanticResolver sr)
    {
      this._evaluatingScopeRegion = this._innermostReferencedScopeRegion ?? this.DefiningScopeRegion;
      if (!this._evaluatingScopeRegion.IsAggregating)
      {
        int scopeRegionIndex = this._evaluatingScopeRegion.ScopeRegionIndex;
        this._evaluatingScopeRegion = (ScopeRegion) null;
        foreach (ScopeRegion scopeRegion in sr.ScopeRegions.Skip<ScopeRegion>(scopeRegionIndex))
        {
          if (scopeRegion.IsAggregating)
          {
            this._evaluatingScopeRegion = scopeRegion;
            break;
          }
        }
        if (this._evaluatingScopeRegion == null)
          throw new EntitySqlException(Strings.GroupVarNotFoundInScope);
      }
      this.ValidateContainedAggregates(this._evaluatingScopeRegion.ScopeRegionIndex, this.DefiningScopeRegion.ScopeRegionIndex);
    }

    private void ValidateContainedAggregates(
      int outerBoundaryScopeRegionIndex,
      int innerBoundaryScopeRegionIndex)
    {
      if (this._containedAggregates == null)
        return;
      foreach (GroupAggregateInfo containedAggregate in this._containedAggregates)
      {
        if (containedAggregate.EvaluatingScopeRegion.ScopeRegionIndex >= outerBoundaryScopeRegionIndex && containedAggregate.EvaluatingScopeRegion.ScopeRegionIndex <= innerBoundaryScopeRegionIndex)
        {
          int lineNumber;
          int columnNumber;
          string str = EntitySqlException.FormatErrorContext(this.ErrCtx.CommandText, this.ErrCtx.InputPosition, this.ErrCtx.ErrorContextInfo, this.ErrCtx.UseContextInfoAsResourceIdentifier, out lineNumber, out columnNumber);
          throw new EntitySqlException(Strings.NestedAggregateCannotBeUsedInAggregate((object) EntitySqlException.FormatErrorContext(containedAggregate.ErrCtx.CommandText, containedAggregate.ErrCtx.InputPosition, containedAggregate.ErrCtx.ErrorContextInfo, containedAggregate.ErrCtx.UseContextInfoAsResourceIdentifier, out lineNumber, out columnNumber), (object) str));
        }
        containedAggregate.ValidateContainedAggregates(outerBoundaryScopeRegionIndex, innerBoundaryScopeRegionIndex);
      }
    }

    internal void SetContainingAggregate(GroupAggregateInfo containingAggregate)
    {
      if (this._containingAggregate != null)
        this._containingAggregate.RemoveContainedAggregate(this);
      this._containingAggregate = containingAggregate;
      if (this._containingAggregate == null)
        return;
      this._containingAggregate.AddContainedAggregate(this);
    }

    private void AddContainedAggregate(GroupAggregateInfo containedAggregate)
    {
      if (this._containedAggregates == null)
        this._containedAggregates = new List<GroupAggregateInfo>();
      this._containedAggregates.Add(containedAggregate);
    }

    private void RemoveContainedAggregate(GroupAggregateInfo containedAggregate)
    {
      this._containedAggregates.Remove(containedAggregate);
    }

    internal ScopeRegion EvaluatingScopeRegion
    {
      get
      {
        return this._evaluatingScopeRegion;
      }
    }

    internal GroupAggregateInfo ContainingAggregate
    {
      get
      {
        return this._containingAggregate;
      }
    }
  }
}
