// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.SourceScopeEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class SourceScopeEntry : ScopeEntry, IGroupExpressionExtendedInfo, IGetAlternativeName
  {
    private readonly string[] _alternativeName;
    private List<string> _propRefs;
    private DbExpression _varBasedExpression;
    private DbExpression _groupVarBasedExpression;
    private DbExpression _groupAggBasedExpression;

    internal SourceScopeEntry(DbVariableReferenceExpression varRef)
      : this(varRef, (string[]) null)
    {
    }

    internal SourceScopeEntry(DbVariableReferenceExpression varRef, string[] alternativeName)
      : base(ScopeEntryKind.SourceVar)
    {
      this._varBasedExpression = (DbExpression) varRef;
      this._alternativeName = alternativeName;
    }

    internal override DbExpression GetExpression(string refName, ErrorContext errCtx)
    {
      return this._varBasedExpression;
    }

    DbExpression IGroupExpressionExtendedInfo.GroupVarBasedExpression
    {
      get
      {
        return this._groupVarBasedExpression;
      }
    }

    DbExpression IGroupExpressionExtendedInfo.GroupAggBasedExpression
    {
      get
      {
        return this._groupAggBasedExpression;
      }
    }

    internal bool IsJoinClauseLeftExpr { get; set; }

    string[] IGetAlternativeName.AlternativeName
    {
      get
      {
        return this._alternativeName;
      }
    }

    internal SourceScopeEntry AddParentVar(
      DbVariableReferenceExpression parentVarRef)
    {
      if (this._propRefs == null)
      {
        this._propRefs = new List<string>(2);
        this._propRefs.Add(((DbVariableReferenceExpression) this._varBasedExpression).VariableName);
      }
      this._varBasedExpression = (DbExpression) parentVarRef;
      for (int index = this._propRefs.Count - 1; index >= 0; --index)
        this._varBasedExpression = (DbExpression) this._varBasedExpression.Property(this._propRefs[index]);
      this._propRefs.Add(parentVarRef.VariableName);
      return this;
    }

    internal void ReplaceParentVar(DbVariableReferenceExpression parentVarRef)
    {
      if (this._propRefs == null)
      {
        this._varBasedExpression = (DbExpression) parentVarRef;
      }
      else
      {
        this._propRefs.RemoveAt(this._propRefs.Count - 1);
        this.AddParentVar(parentVarRef);
      }
    }

    internal void AdjustToGroupVar(
      DbVariableReferenceExpression parentVarRef,
      DbVariableReferenceExpression parentGroupVarRef,
      DbVariableReferenceExpression groupAggRef)
    {
      this.ReplaceParentVar(parentVarRef);
      this._groupVarBasedExpression = (DbExpression) parentGroupVarRef;
      this._groupAggBasedExpression = (DbExpression) groupAggRef;
      if (this._propRefs == null)
        return;
      for (int index = this._propRefs.Count - 2; index >= 0; --index)
      {
        this._groupVarBasedExpression = (DbExpression) this._groupVarBasedExpression.Property(this._propRefs[index]);
        this._groupAggBasedExpression = (DbExpression) this._groupAggBasedExpression.Property(this._propRefs[index]);
      }
    }

    internal void RollbackAdjustmentToGroupVar(DbVariableReferenceExpression pregroupParentVarRef)
    {
      this._groupVarBasedExpression = (DbExpression) null;
      this._groupAggBasedExpression = (DbExpression) null;
      this.ReplaceParentVar(pregroupParentVarRef);
    }
  }
}
