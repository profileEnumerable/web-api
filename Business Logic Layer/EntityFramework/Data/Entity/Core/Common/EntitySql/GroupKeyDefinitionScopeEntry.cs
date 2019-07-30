// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.GroupKeyDefinitionScopeEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class GroupKeyDefinitionScopeEntry : ScopeEntry, IGroupExpressionExtendedInfo, IGetAlternativeName
  {
    private readonly DbExpression _varBasedExpression;
    private readonly DbExpression _groupVarBasedExpression;
    private readonly DbExpression _groupAggBasedExpression;
    private readonly string[] _alternativeName;

    internal GroupKeyDefinitionScopeEntry(
      DbExpression varBasedExpression,
      DbExpression groupVarBasedExpression,
      DbExpression groupAggBasedExpression,
      string[] alternativeName)
      : base(ScopeEntryKind.GroupKeyDefinition)
    {
      this._varBasedExpression = varBasedExpression;
      this._groupVarBasedExpression = groupVarBasedExpression;
      this._groupAggBasedExpression = groupAggBasedExpression;
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

    string[] IGetAlternativeName.AlternativeName
    {
      get
      {
        return this._alternativeName;
      }
    }
  }
}
