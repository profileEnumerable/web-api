// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.QueryCache.CompiledQueryCacheEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.Internal;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common.QueryCache
{
  internal sealed class CompiledQueryCacheEntry : QueryCacheEntry
  {
    public readonly MergeOption? PropagatedMergeOption;
    private readonly ConcurrentDictionary<string, ObjectQueryExecutionPlan> _plans;

    internal CompiledQueryCacheEntry(QueryCacheKey queryCacheKey, MergeOption? mergeOption)
      : base(queryCacheKey, (object) null)
    {
      this.PropagatedMergeOption = mergeOption;
      this._plans = new ConcurrentDictionary<string, ObjectQueryExecutionPlan>();
    }

    internal ObjectQueryExecutionPlan GetExecutionPlan(
      MergeOption mergeOption,
      bool useCSharpNullComparisonBehavior)
    {
      ObjectQueryExecutionPlan queryExecutionPlan;
      this._plans.TryGetValue(CompiledQueryCacheEntry.GenerateLocalCacheKey(mergeOption, useCSharpNullComparisonBehavior), out queryExecutionPlan);
      return queryExecutionPlan;
    }

    internal ObjectQueryExecutionPlan SetExecutionPlan(
      ObjectQueryExecutionPlan newPlan,
      bool useCSharpNullComparisonBehavior)
    {
      return this._plans.GetOrAdd(CompiledQueryCacheEntry.GenerateLocalCacheKey(newPlan.MergeOption, useCSharpNullComparisonBehavior), newPlan);
    }

    internal bool TryGetResultType(out TypeUsage resultType)
    {
      using (IEnumerator<ObjectQueryExecutionPlan> enumerator = this._plans.Values.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          ObjectQueryExecutionPlan current = enumerator.Current;
          resultType = current.ResultType;
          return true;
        }
      }
      resultType = (TypeUsage) null;
      return false;
    }

    internal override object GetTarget()
    {
      return (object) this;
    }

    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    private static string GenerateLocalCacheKey(
      MergeOption mergeOption,
      bool useCSharpNullComparisonBehavior)
    {
      switch (mergeOption)
      {
        case MergeOption.AppendOnly:
        case MergeOption.OverwriteChanges:
        case MergeOption.PreserveChanges:
        case MergeOption.NoTracking:
          return string.Join("", (object) Enum.GetName(typeof (MergeOption), (object) mergeOption), (object) useCSharpNullComparisonBehavior);
        default:
          throw new ArgumentOutOfRangeException("newPlan.MergeOption");
      }
    }
  }
}
