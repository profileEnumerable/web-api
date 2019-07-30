// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.CaseStatement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal sealed class CaseStatement : InternalBase
  {
    private readonly MemberPath m_memberPath;
    private List<CaseStatement.WhenThen> m_clauses;
    private ProjectedSlot m_elseValue;
    private bool m_simplified;

    internal CaseStatement(MemberPath memberPath)
    {
      this.m_memberPath = memberPath;
      this.m_clauses = new List<CaseStatement.WhenThen>();
    }

    internal MemberPath MemberPath
    {
      get
      {
        return this.m_memberPath;
      }
    }

    internal List<CaseStatement.WhenThen> Clauses
    {
      get
      {
        return this.m_clauses;
      }
    }

    internal ProjectedSlot ElseValue
    {
      get
      {
        return this.m_elseValue;
      }
    }

    internal CaseStatement DeepQualify(CqlBlock block)
    {
      CaseStatement caseStatement = new CaseStatement(this.m_memberPath);
      foreach (CaseStatement.WhenThen clause in this.m_clauses)
      {
        CaseStatement.WhenThen whenThen = clause.ReplaceWithQualifiedSlot(block);
        caseStatement.m_clauses.Add(whenThen);
      }
      if (this.m_elseValue != null)
        caseStatement.m_elseValue = this.m_elseValue.DeepQualify(block);
      caseStatement.m_simplified = this.m_simplified;
      return caseStatement;
    }

    internal void AddWhenThen(BoolExpression condition, ProjectedSlot value)
    {
      condition.ExpensiveSimplify();
      this.m_clauses.Add(new CaseStatement.WhenThen(condition, value));
    }

    internal bool DependsOnMemberValue
    {
      get
      {
        if (this.m_elseValue is MemberProjectedSlot)
          return true;
        foreach (CaseStatement.WhenThen clause in this.m_clauses)
        {
          if (clause.Value is MemberProjectedSlot)
            return true;
        }
        return false;
      }
    }

    internal IEnumerable<EdmType> InstantiatedTypes
    {
      get
      {
        foreach (CaseStatement.WhenThen clause in this.m_clauses)
        {
          EdmType type;
          if (CaseStatement.TryGetInstantiatedType(clause.Value, out type))
            yield return type;
        }
        EdmType elseType;
        if (CaseStatement.TryGetInstantiatedType(this.m_elseValue, out elseType))
          yield return elseType;
      }
    }

    private static bool TryGetInstantiatedType(ProjectedSlot slot, out EdmType type)
    {
      type = (EdmType) null;
      ConstantProjectedSlot constantProjectedSlot = slot as ConstantProjectedSlot;
      if (constantProjectedSlot != null)
      {
        TypeConstant cellConstant = constantProjectedSlot.CellConstant as TypeConstant;
        if (cellConstant != null)
        {
          type = cellConstant.EdmType;
          return true;
        }
      }
      return false;
    }

    internal void Simplify()
    {
      if (this.m_simplified)
        return;
      List<CaseStatement.WhenThen> whenThenList = new List<CaseStatement.WhenThen>();
      bool flag = false;
      foreach (CaseStatement.WhenThen clause in this.m_clauses)
      {
        ConstantProjectedSlot constantProjectedSlot = clause.Value as ConstantProjectedSlot;
        if (constantProjectedSlot != null && (constantProjectedSlot.CellConstant.IsNull() || constantProjectedSlot.CellConstant.IsUndefined()))
        {
          flag = true;
        }
        else
        {
          whenThenList.Add(clause);
          if (clause.Condition.IsTrue)
            break;
        }
      }
      if (flag && whenThenList.Count == 0)
        this.m_elseValue = (ProjectedSlot) new ConstantProjectedSlot(Constant.Null);
      if (whenThenList.Count > 0 && !flag)
      {
        int index = whenThenList.Count - 1;
        this.m_elseValue = whenThenList[index].Value;
        whenThenList.RemoveAt(index);
      }
      this.m_clauses = whenThenList;
      this.m_simplified = true;
    }

    internal StringBuilder AsEsql(
      StringBuilder builder,
      IEnumerable<WithRelationship> withRelationships,
      string blockAlias,
      int indentLevel)
    {
      if (this.Clauses.Count == 0)
      {
        CaseStatement.CaseSlotValueAsEsql(builder, this.ElseValue, this.MemberPath, blockAlias, withRelationships, indentLevel);
        return builder;
      }
      builder.Append("CASE");
      foreach (CaseStatement.WhenThen clause in this.Clauses)
      {
        StringUtil.IndentNewLine(builder, indentLevel + 2);
        builder.Append("WHEN ");
        clause.Condition.AsEsql(builder, blockAlias);
        builder.Append(" THEN ");
        CaseStatement.CaseSlotValueAsEsql(builder, clause.Value, this.MemberPath, blockAlias, withRelationships, indentLevel + 2);
      }
      if (this.ElseValue != null)
      {
        StringUtil.IndentNewLine(builder, indentLevel + 2);
        builder.Append("ELSE ");
        CaseStatement.CaseSlotValueAsEsql(builder, this.ElseValue, this.MemberPath, blockAlias, withRelationships, indentLevel + 2);
      }
      StringUtil.IndentNewLine(builder, indentLevel + 1);
      builder.Append("END");
      return builder;
    }

    internal DbExpression AsCqt(
      DbExpression row,
      IEnumerable<WithRelationship> withRelationships)
    {
      List<DbExpression> dbExpressionList1 = new List<DbExpression>();
      List<DbExpression> dbExpressionList2 = new List<DbExpression>();
      foreach (CaseStatement.WhenThen clause in this.Clauses)
      {
        dbExpressionList1.Add(clause.Condition.AsCqt(row));
        dbExpressionList2.Add(CaseStatement.CaseSlotValueAsCqt(row, clause.Value, this.MemberPath, withRelationships));
      }
      DbExpression elseExpression = this.ElseValue != null ? CaseStatement.CaseSlotValueAsCqt(row, this.ElseValue, this.MemberPath, withRelationships) : Constant.Null.AsCqt(row, this.MemberPath);
      if (this.Clauses.Count > 0)
        return (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) dbExpressionList1, (IEnumerable<DbExpression>) dbExpressionList2, elseExpression);
      return elseExpression;
    }

    private static StringBuilder CaseSlotValueAsEsql(
      StringBuilder builder,
      ProjectedSlot slot,
      MemberPath outputMember,
      string blockAlias,
      IEnumerable<WithRelationship> withRelationships,
      int indentLevel)
    {
      slot.AsEsql(builder, outputMember, blockAlias, 1);
      CaseStatement.WithRelationshipsClauseAsEsql(builder, withRelationships, blockAlias, indentLevel, slot);
      return builder;
    }

    private static void WithRelationshipsClauseAsEsql(
      StringBuilder builder,
      IEnumerable<WithRelationship> withRelationships,
      string blockAlias,
      int indentLevel,
      ProjectedSlot slot)
    {
      bool first = true;
      CaseStatement.WithRelationshipsClauseAsCql((Action<WithRelationship>) (withRelationship =>
      {
        if (first)
        {
          builder.Append(" WITH ");
          first = false;
        }
        withRelationship.AsEsql(builder, blockAlias, indentLevel);
      }), withRelationships, slot);
    }

    private static DbExpression CaseSlotValueAsCqt(
      DbExpression row,
      ProjectedSlot slot,
      MemberPath outputMember,
      IEnumerable<WithRelationship> withRelationships)
    {
      DbExpression slotValueExpr = slot.AsCqt(row, outputMember);
      return CaseStatement.WithRelationshipsClauseAsCqt(row, slotValueExpr, withRelationships, slot);
    }

    private static DbExpression WithRelationshipsClauseAsCqt(
      DbExpression row,
      DbExpression slotValueExpr,
      IEnumerable<WithRelationship> withRelationships,
      ProjectedSlot slot)
    {
      List<DbRelatedEntityRef> relatedEntityRefs = new List<DbRelatedEntityRef>();
      CaseStatement.WithRelationshipsClauseAsCql((Action<WithRelationship>) (withRelationship => relatedEntityRefs.Add(withRelationship.AsCqt(row))), withRelationships, slot);
      if (relatedEntityRefs.Count <= 0)
        return slotValueExpr;
      DbNewInstanceExpression instanceExpression = slotValueExpr as DbNewInstanceExpression;
      return (DbExpression) DbExpressionBuilder.CreateNewEntityWithRelationshipsExpression((EntityType) instanceExpression.ResultType.EdmType, instanceExpression.Arguments, (IList<DbRelatedEntityRef>) relatedEntityRefs);
    }

    private static void WithRelationshipsClauseAsCql(
      Action<WithRelationship> emitWithRelationship,
      IEnumerable<WithRelationship> withRelationships,
      ProjectedSlot slot)
    {
      if (withRelationships == null || withRelationships.Count<WithRelationship>() <= 0)
        return;
      EdmType edmType = ((slot as ConstantProjectedSlot).CellConstant as TypeConstant).EdmType;
      foreach (WithRelationship withRelationship in withRelationships)
      {
        if (withRelationship.FromEndEntityType.IsAssignableFrom(edmType))
          emitWithRelationship(withRelationship);
      }
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.AppendLine("CASE");
      foreach (CaseStatement.WhenThen clause in this.m_clauses)
      {
        builder.Append(" WHEN ");
        clause.Condition.ToCompactString(builder);
        builder.Append(" THEN ");
        clause.Value.ToCompactString(builder);
        builder.AppendLine();
      }
      if (this.m_elseValue != null)
      {
        builder.Append(" ELSE ");
        this.m_elseValue.ToCompactString(builder);
        builder.AppendLine();
      }
      builder.Append(" END AS ");
      this.m_memberPath.ToCompactString(builder);
    }

    internal sealed class WhenThen : InternalBase
    {
      private readonly BoolExpression m_condition;
      private readonly ProjectedSlot m_value;

      internal WhenThen(BoolExpression condition, ProjectedSlot value)
      {
        this.m_condition = condition;
        this.m_value = value;
      }

      internal BoolExpression Condition
      {
        get
        {
          return this.m_condition;
        }
      }

      internal ProjectedSlot Value
      {
        get
        {
          return this.m_value;
        }
      }

      internal CaseStatement.WhenThen ReplaceWithQualifiedSlot(CqlBlock block)
      {
        return new CaseStatement.WhenThen(this.m_condition, this.m_value.DeepQualify(block));
      }

      internal override void ToCompactString(StringBuilder builder)
      {
        builder.Append("WHEN ");
        this.m_condition.ToCompactString(builder);
        builder.Append("THEN ");
        this.m_value.ToCompactString(builder);
      }
    }
  }
}
