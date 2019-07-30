// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.CaseStatementProjectedSlot
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal sealed class CaseStatementProjectedSlot : ProjectedSlot
  {
    private readonly CaseStatement m_caseStatement;
    private readonly IEnumerable<WithRelationship> m_withRelationships;

    internal CaseStatementProjectedSlot(
      CaseStatement statement,
      IEnumerable<WithRelationship> withRelationships)
    {
      this.m_caseStatement = statement;
      this.m_withRelationships = withRelationships;
    }

    internal override ProjectedSlot DeepQualify(CqlBlock block)
    {
      return (ProjectedSlot) new CaseStatementProjectedSlot(this.m_caseStatement.DeepQualify(block), (IEnumerable<WithRelationship>) null);
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      MemberPath outputMember,
      string blockAlias,
      int indentLevel)
    {
      this.m_caseStatement.AsEsql(builder, this.m_withRelationships, blockAlias, indentLevel);
      return builder;
    }

    internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
    {
      return this.m_caseStatement.AsCqt(row, this.m_withRelationships);
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      this.m_caseStatement.ToCompactString(builder);
    }
  }
}
