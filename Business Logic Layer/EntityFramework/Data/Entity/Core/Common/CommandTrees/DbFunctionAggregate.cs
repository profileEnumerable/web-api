// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbFunctionAggregate
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Supports standard aggregate functions, such as MIN, MAX, AVG, SUM, and so on. This class cannot be inherited.</summary>
  public sealed class DbFunctionAggregate : DbAggregate
  {
    private readonly bool _distinct;
    private readonly EdmFunction _aggregateFunction;

    internal DbFunctionAggregate(
      TypeUsage resultType,
      DbExpressionList arguments,
      EdmFunction function,
      bool isDistinct)
      : base(resultType, arguments)
    {
      this._aggregateFunction = function;
      this._distinct = isDistinct;
    }

    /// <summary>Gets a value indicating whether this aggregate is a distinct aggregate.</summary>
    /// <returns>true if the aggregate is a distinct aggregate; otherwise, false. </returns>
    public bool Distinct
    {
      get
      {
        return this._distinct;
      }
    }

    /// <summary>Gets the method metadata that specifies the aggregate function to invoke.</summary>
    /// <returns>The method metadata that specifies the aggregate function to invoke.</returns>
    public EdmFunction Function
    {
      get
      {
        return this._aggregateFunction;
      }
    }
  }
}
