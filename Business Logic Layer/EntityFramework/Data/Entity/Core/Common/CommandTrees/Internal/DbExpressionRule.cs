// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.Internal.DbExpressionRule
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.CommandTrees.Internal
{
  internal abstract class DbExpressionRule
  {
    internal abstract bool ShouldProcess(DbExpression expression);

    internal abstract bool TryProcess(DbExpression expression, out DbExpression result);

    internal abstract DbExpressionRule.ProcessedAction OnExpressionProcessed { get; }

    internal enum ProcessedAction
    {
      Continue,
      Reset,
      Stop,
    }
  }
}
