// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.FunctionAggregateInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.EntitySql.AST;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class FunctionAggregateInfo : GroupAggregateInfo
  {
    internal DbAggregate AggregateDefinition;

    internal FunctionAggregateInfo(
      MethodExpr methodExpr,
      ErrorContext errCtx,
      GroupAggregateInfo containingAggregate,
      ScopeRegion definingScopeRegion)
      : base(GroupAggregateKind.Function, (GroupAggregateExpr) methodExpr, errCtx, containingAggregate, definingScopeRegion)
    {
    }

    internal void AttachToAstNode(string aggregateName, DbAggregate aggregateDefinition)
    {
      this.AttachToAstNode(aggregateName, aggregateDefinition.ResultType);
      this.AggregateDefinition = aggregateDefinition;
    }
  }
}
