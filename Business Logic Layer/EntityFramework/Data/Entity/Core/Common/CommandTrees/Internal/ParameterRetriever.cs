// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.Internal.ParameterRetriever
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Common.CommandTrees.Internal
{
  internal sealed class ParameterRetriever : BasicCommandTreeVisitor
  {
    private readonly Dictionary<string, DbParameterReferenceExpression> paramMappings = new Dictionary<string, DbParameterReferenceExpression>();

    private ParameterRetriever()
    {
    }

    internal static ReadOnlyCollection<DbParameterReferenceExpression> GetParameters(
      DbCommandTree tree)
    {
      ParameterRetriever parameterRetriever = new ParameterRetriever();
      parameterRetriever.VisitCommandTree(tree);
      return new ReadOnlyCollection<DbParameterReferenceExpression>((IList<DbParameterReferenceExpression>) parameterRetriever.paramMappings.Values.ToList<DbParameterReferenceExpression>());
    }

    public override void Visit(DbParameterReferenceExpression expression)
    {
      Check.NotNull<DbParameterReferenceExpression>(expression, nameof (expression));
      this.paramMappings[expression.ParameterName] = expression;
    }
  }
}
