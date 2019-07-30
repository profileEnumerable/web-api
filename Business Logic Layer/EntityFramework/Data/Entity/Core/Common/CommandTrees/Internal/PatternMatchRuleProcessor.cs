// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.Internal.PatternMatchRuleProcessor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Data.Entity.Core.Common.CommandTrees.Internal
{
  internal class PatternMatchRuleProcessor : DbExpressionRuleProcessingVisitor
  {
    private readonly ReadOnlyCollection<PatternMatchRule> ruleSet;

    private PatternMatchRuleProcessor(ReadOnlyCollection<PatternMatchRule> rules)
    {
      this.ruleSet = rules;
    }

    private DbExpression Process(DbExpression expression)
    {
      expression = this.VisitExpression(expression);
      return expression;
    }

    protected override IEnumerable<DbExpressionRule> GetRules()
    {
      return (IEnumerable<DbExpressionRule>) this.ruleSet;
    }

    internal static Func<DbExpression, DbExpression> Create(
      params PatternMatchRule[] rules)
    {
      return new Func<DbExpression, DbExpression>(new PatternMatchRuleProcessor(new ReadOnlyCollection<PatternMatchRule>((IList<PatternMatchRule>) rules)).Process);
    }
  }
}
