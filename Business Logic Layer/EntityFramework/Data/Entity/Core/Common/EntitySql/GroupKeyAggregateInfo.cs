// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.GroupKeyAggregateInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.EntitySql.AST;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class GroupKeyAggregateInfo : GroupAggregateInfo
  {
    internal GroupKeyAggregateInfo(
      GroupAggregateKind aggregateKind,
      ErrorContext errCtx,
      GroupAggregateInfo containingAggregate,
      ScopeRegion definingScopeRegion)
      : base(aggregateKind, (GroupAggregateExpr) null, errCtx, containingAggregate, definingScopeRegion)
    {
    }
  }
}
