// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.BoolExpression
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class BoolExpression : InternalBase
  {
    internal static readonly IEqualityComparer<BoolExpression> EqualityComparer = (IEqualityComparer<BoolExpression>) new BoolExpression.BoolComparer();
    internal static readonly BoolExpression True = new BoolExpression(true);
    internal static readonly BoolExpression False = new BoolExpression(false);
    private static readonly BoolExpression.CopyVisitor _copyVisitorInstance = new BoolExpression.CopyVisitor();
    private BoolExpr<DomainConstraint<BoolLiteral, Constant>> m_tree;
    private readonly MemberDomainMap m_memberDomainMap;
    private Converter<DomainConstraint<BoolLiteral, Constant>> m_converter;

    internal static BoolExpression CreateLiteral(
      BoolLiteral literal,
      MemberDomainMap memberDomainMap)
    {
      return new BoolExpression(literal.GetDomainBoolExpression(memberDomainMap), memberDomainMap);
    }

    internal BoolExpression Create(BoolLiteral literal)
    {
      return new BoolExpression(literal.GetDomainBoolExpression(this.m_memberDomainMap), this.m_memberDomainMap);
    }

    internal static BoolExpression CreateNot(BoolExpression expression)
    {
      return new BoolExpression(ExprType.Not, (IEnumerable<BoolExpression>) new BoolExpression[1]
      {
        expression
      });
    }

    internal static BoolExpression CreateAnd(params BoolExpression[] children)
    {
      return new BoolExpression(ExprType.And, (IEnumerable<BoolExpression>) children);
    }

    internal static BoolExpression CreateOr(params BoolExpression[] children)
    {
      return new BoolExpression(ExprType.Or, (IEnumerable<BoolExpression>) children);
    }

    internal static BoolExpression CreateAndNot(BoolExpression e1, BoolExpression e2)
    {
      return BoolExpression.CreateAnd(e1, BoolExpression.CreateNot(e2));
    }

    internal BoolExpression Create(
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression)
    {
      return new BoolExpression(expression, this.m_memberDomainMap);
    }

    private BoolExpression(bool isTrue)
    {
      if (isTrue)
        this.m_tree = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) TrueExpr<DomainConstraint<BoolLiteral, Constant>>.Value;
      else
        this.m_tree = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) FalseExpr<DomainConstraint<BoolLiteral, Constant>>.Value;
    }

    private BoolExpression(ExprType opType, IEnumerable<BoolExpression> children)
    {
      List<BoolExpression> boolExpressionList = new List<BoolExpression>(children);
      foreach (BoolExpression child in children)
      {
        if (child.m_memberDomainMap != null)
        {
          this.m_memberDomainMap = child.m_memberDomainMap;
          break;
        }
      }
      switch (opType)
      {
        case ExprType.And:
          this.m_tree = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>(BoolExpression.ToBoolExprList((IEnumerable<BoolExpression>) boolExpressionList));
          break;
        case ExprType.Not:
          this.m_tree = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new NotExpr<DomainConstraint<BoolLiteral, Constant>>(boolExpressionList[0].m_tree);
          break;
        case ExprType.Or:
          this.m_tree = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new OrExpr<DomainConstraint<BoolLiteral, Constant>>(BoolExpression.ToBoolExprList((IEnumerable<BoolExpression>) boolExpressionList));
          break;
      }
    }

    internal BoolExpression(
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> expr,
      MemberDomainMap memberDomainMap)
    {
      this.m_tree = expr;
      this.m_memberDomainMap = memberDomainMap;
    }

    internal IEnumerable<BoolExpression> Atoms
    {
      get
      {
        IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>> atoms = BoolExpression.TermVisitor.GetTerms(this.m_tree, false);
        foreach (TermExpr<DomainConstraint<BoolLiteral, Constant>> termExpr in atoms)
          yield return new BoolExpression((BoolExpr<DomainConstraint<BoolLiteral, Constant>>) termExpr, this.m_memberDomainMap);
      }
    }

    internal BoolLiteral AsLiteral
    {
      get
      {
        TermExpr<DomainConstraint<BoolLiteral, Constant>> tree = this.m_tree as TermExpr<DomainConstraint<BoolLiteral, Constant>>;
        if (tree == null)
          return (BoolLiteral) null;
        return BoolExpression.GetBoolLiteral(tree);
      }
    }

    internal static BoolLiteral GetBoolLiteral(
      TermExpr<DomainConstraint<BoolLiteral, Constant>> term)
    {
      return term.Identifier.Variable.Identifier;
    }

    internal bool IsTrue
    {
      get
      {
        return this.m_tree.ExprType == ExprType.True;
      }
    }

    internal bool IsFalse
    {
      get
      {
        return this.m_tree.ExprType == ExprType.False;
      }
    }

    internal bool IsAlwaysTrue()
    {
      this.InitializeConverter();
      return this.m_converter.Vertex.IsOne();
    }

    internal bool IsSatisfiable()
    {
      return !this.IsUnsatisfiable();
    }

    internal bool IsUnsatisfiable()
    {
      this.InitializeConverter();
      return this.m_converter.Vertex.IsZero();
    }

    internal BoolExpr<DomainConstraint<BoolLiteral, Constant>> Tree
    {
      get
      {
        return this.m_tree;
      }
    }

    internal IEnumerable<DomainConstraint<BoolLiteral, Constant>> VariableConstraints
    {
      get
      {
        return LeafVisitor<DomainConstraint<BoolLiteral, Constant>>.GetLeaves(this.m_tree);
      }
    }

    internal IEnumerable<DomainVariable<BoolLiteral, Constant>> Variables
    {
      get
      {
        return this.VariableConstraints.Select<DomainConstraint<BoolLiteral, Constant>, DomainVariable<BoolLiteral, Constant>>((Func<DomainConstraint<BoolLiteral, Constant>, DomainVariable<BoolLiteral, Constant>>) (domainConstraint => domainConstraint.Variable));
      }
    }

    internal IEnumerable<MemberRestriction> MemberRestrictions
    {
      get
      {
        foreach (DomainVariable<BoolLiteral, Constant> variable in this.Variables)
        {
          MemberRestriction variableCondition = variable.Identifier as MemberRestriction;
          if (variableCondition != null)
            yield return variableCondition;
        }
      }
    }

    private static IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, Constant>>> ToBoolExprList(
      IEnumerable<BoolExpression> nodes)
    {
      foreach (BoolExpression node in nodes)
        yield return node.m_tree;
    }

    internal bool RepresentsAllTypeConditions
    {
      get
      {
        return this.MemberRestrictions.All<MemberRestriction>((Func<MemberRestriction, bool>) (var => var is TypeRestriction));
      }
    }

    internal BoolExpression RemapLiterals(Dictionary<BoolLiteral, BoolLiteral> remap)
    {
      return new BoolExpression(this.m_tree.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) new BooleanExpressionTermRewriter<DomainConstraint<BoolLiteral, Constant>, DomainConstraint<BoolLiteral, Constant>>((Func<TermExpr<DomainConstraint<BoolLiteral, Constant>>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) (term =>
      {
        BoolLiteral boolLiteral;
        if (!remap.TryGetValue(BoolExpression.GetBoolLiteral(term), out boolLiteral))
          return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) term;
        return boolLiteral.GetDomainBoolExpression(this.m_memberDomainMap);
      }))), this.m_memberDomainMap);
    }

    internal virtual void GetRequiredSlots(
      MemberProjectionIndex projectedSlotMap,
      bool[] requiredSlots)
    {
      BoolExpression.RequiredSlotsVisitor.GetRequiredSlots(this.m_tree, projectedSlotMap, requiredSlots);
    }

    internal StringBuilder AsEsql(StringBuilder builder, string blockAlias)
    {
      return BoolExpression.AsEsqlVisitor.AsEsql(this.m_tree, builder, blockAlias);
    }

    internal DbExpression AsCqt(DbExpression row)
    {
      return BoolExpression.AsCqtVisitor.AsCqt(this.m_tree, row);
    }

    internal StringBuilder AsUserString(
      StringBuilder builder,
      string blockAlias,
      bool writeRoundtrippingMessage)
    {
      if (writeRoundtrippingMessage)
      {
        builder.AppendLine(Strings.Viewgen_ConfigurationErrorMsg((object) blockAlias));
        builder.Append("  ");
      }
      return BoolExpression.AsUserStringVisitor.AsUserString(this.m_tree, builder, blockAlias);
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      BoolExpression.CompactStringVisitor.ToBuilder(this.m_tree, builder);
    }

    internal BoolExpression RemapBool(Dictionary<MemberPath, MemberPath> remap)
    {
      return new BoolExpression(BoolExpression.RemapBoolVisitor.RemapExtentTreeNodes(this.m_tree, this.m_memberDomainMap, remap), this.m_memberDomainMap);
    }

    internal static List<BoolExpression> AddConjunctionToBools(
      List<BoolExpression> bools,
      BoolExpression conjunct)
    {
      List<BoolExpression> boolExpressionList = new List<BoolExpression>();
      foreach (BoolExpression boolExpression in bools)
      {
        if (boolExpression == null)
          boolExpressionList.Add((BoolExpression) null);
        else
          boolExpressionList.Add(BoolExpression.CreateAnd(boolExpression, conjunct));
      }
      return boolExpressionList;
    }

    private void InitializeConverter()
    {
      if (this.m_converter != null)
        return;
      this.m_converter = new Converter<DomainConstraint<BoolLiteral, Constant>>(this.m_tree, IdentifierService<DomainConstraint<BoolLiteral, Constant>>.Instance.CreateConversionContext());
    }

    internal BoolExpression MakeCopy()
    {
      return this.Create(this.m_tree.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) BoolExpression._copyVisitorInstance));
    }

    internal void ExpensiveSimplify()
    {
      if (!this.IsFinal())
      {
        this.m_tree = this.m_tree.Simplify();
      }
      else
      {
        this.InitializeConverter();
        this.m_tree = this.m_tree.ExpensiveSimplify(out this.m_converter);
        this.FixDomainMap(this.m_memberDomainMap);
      }
    }

    internal void FixDomainMap(MemberDomainMap domainMap)
    {
      this.m_tree = BoolExpression.FixRangeVisitor.FixRange(this.m_tree, domainMap);
    }

    private bool IsFinal()
    {
      if (this.m_memberDomainMap != null)
        return BoolExpression.IsFinalVisitor.IsFinal(this.m_tree);
      return false;
    }

    private class CopyVisitor : BasicVisitor<DomainConstraint<BoolLiteral, Constant>>
    {
    }

    private class BoolComparer : IEqualityComparer<BoolExpression>
    {
      public bool Equals(BoolExpression left, BoolExpression right)
      {
        if (object.ReferenceEquals((object) left, (object) right))
          return true;
        if (left == null || right == null)
          return false;
        return left.m_tree.Equals(right.m_tree);
      }

      public int GetHashCode(BoolExpression expression)
      {
        return expression.m_tree.GetHashCode();
      }
    }

    private class FixRangeVisitor : BasicVisitor<DomainConstraint<BoolLiteral, Constant>>
    {
      private readonly MemberDomainMap m_memberDomainMap;

      private FixRangeVisitor(MemberDomainMap memberDomainMap)
      {
        this.m_memberDomainMap = memberDomainMap;
      }

      internal static BoolExpr<DomainConstraint<BoolLiteral, Constant>> FixRange(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        MemberDomainMap memberDomainMap)
      {
        BoolExpression.FixRangeVisitor fixRangeVisitor = new BoolExpression.FixRangeVisitor(memberDomainMap);
        return expression.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) fixRangeVisitor);
      }

      internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitTerm(
        TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return BoolExpression.GetBoolLiteral(expression).FixRange(expression.Identifier.Range, this.m_memberDomainMap);
      }
    }

    private class IsFinalVisitor : Visitor<DomainConstraint<BoolLiteral, Constant>, bool>
    {
      internal static bool IsFinal(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        BoolExpression.IsFinalVisitor isFinalVisitor = new BoolExpression.IsFinalVisitor();
        return expression.Accept<bool>((Visitor<DomainConstraint<BoolLiteral, Constant>, bool>) isFinalVisitor);
      }

      internal override bool VisitTrue(
        TrueExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return true;
      }

      internal override bool VisitFalse(
        FalseExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return true;
      }

      internal override bool VisitTerm(
        TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        MemberRestriction boolLiteral = BoolExpression.GetBoolLiteral(expression) as MemberRestriction;
        return boolLiteral == null || boolLiteral.IsComplete;
      }

      internal override bool VisitNot(
        NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return expression.Child.Accept<bool>((Visitor<DomainConstraint<BoolLiteral, Constant>, bool>) this);
      }

      internal override bool VisitAnd(
        AndExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression);
      }

      internal override bool VisitOr(
        OrExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression);
      }

      private bool VisitAndOr(
        TreeExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        bool flag1 = true;
        bool flag2 = true;
        foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> child in expression.Children)
        {
          if (!(child is FalseExpr<DomainConstraint<BoolLiteral, Constant>>) && !(child is TrueExpr<DomainConstraint<BoolLiteral, Constant>>))
          {
            bool flag3 = child.Accept<bool>((Visitor<DomainConstraint<BoolLiteral, Constant>, bool>) this);
            if (flag1)
              flag2 = flag3;
            flag1 = false;
          }
        }
        return flag2;
      }
    }

    private class RemapBoolVisitor : BasicVisitor<DomainConstraint<BoolLiteral, Constant>>
    {
      private readonly Dictionary<MemberPath, MemberPath> m_remap;
      private readonly MemberDomainMap m_memberDomainMap;

      private RemapBoolVisitor(
        MemberDomainMap memberDomainMap,
        Dictionary<MemberPath, MemberPath> remap)
      {
        this.m_remap = remap;
        this.m_memberDomainMap = memberDomainMap;
      }

      internal static BoolExpr<DomainConstraint<BoolLiteral, Constant>> RemapExtentTreeNodes(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        MemberDomainMap memberDomainMap,
        Dictionary<MemberPath, MemberPath> remap)
      {
        BoolExpression.RemapBoolVisitor remapBoolVisitor = new BoolExpression.RemapBoolVisitor(memberDomainMap, remap);
        return expression.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) remapBoolVisitor);
      }

      internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitTerm(
        TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return BoolExpression.GetBoolLiteral(expression).RemapBool(this.m_remap).GetDomainBoolExpression(this.m_memberDomainMap);
      }
    }

    private class RequiredSlotsVisitor : BasicVisitor<DomainConstraint<BoolLiteral, Constant>>
    {
      private readonly MemberProjectionIndex m_projectedSlotMap;
      private readonly bool[] m_requiredSlots;

      private RequiredSlotsVisitor(MemberProjectionIndex projectedSlotMap, bool[] requiredSlots)
      {
        this.m_projectedSlotMap = projectedSlotMap;
        this.m_requiredSlots = requiredSlots;
      }

      internal static void GetRequiredSlots(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        MemberProjectionIndex projectedSlotMap,
        bool[] requiredSlots)
      {
        BoolExpression.RequiredSlotsVisitor requiredSlotsVisitor = new BoolExpression.RequiredSlotsVisitor(projectedSlotMap, requiredSlots);
        expression.Accept<BoolExpr<DomainConstraint<BoolLiteral, Constant>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, BoolExpr<DomainConstraint<BoolLiteral, Constant>>>) requiredSlotsVisitor);
      }

      internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> VisitTerm(
        TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        BoolExpression.GetBoolLiteral(expression).GetRequiredSlots(this.m_projectedSlotMap, this.m_requiredSlots);
        return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) expression;
      }
    }

    private abstract class AsCqlVisitor<T_Return> : Visitor<DomainConstraint<BoolLiteral, Constant>, T_Return>
    {
      private bool m_skipIsNotNull;

      protected AsCqlVisitor()
      {
        this.m_skipIsNotNull = true;
      }

      internal override T_Return VisitTerm(
        TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.BooleanLiteralAsCql(BoolExpression.GetBoolLiteral(expression), this.m_skipIsNotNull);
      }

      protected abstract T_Return BooleanLiteralAsCql(BoolLiteral literal, bool skipIsNotNull);

      internal override T_Return VisitNot(
        NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_skipIsNotNull = false;
        return this.NotExprAsCql(expression);
      }

      protected abstract T_Return NotExprAsCql(
        NotExpr<DomainConstraint<BoolLiteral, Constant>> expression);
    }

    private sealed class AsEsqlVisitor : BoolExpression.AsCqlVisitor<StringBuilder>
    {
      private readonly StringBuilder m_builder;
      private readonly string m_blockAlias;

      internal static StringBuilder AsEsql(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        StringBuilder builder,
        string blockAlias)
      {
        BoolExpression.AsEsqlVisitor asEsqlVisitor = new BoolExpression.AsEsqlVisitor(builder, blockAlias);
        return expression.Accept<StringBuilder>((Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>) asEsqlVisitor);
      }

      private AsEsqlVisitor(StringBuilder builder, string blockAlias)
      {
        this.m_builder = builder;
        this.m_blockAlias = blockAlias;
      }

      internal override StringBuilder VisitTrue(
        TrueExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_builder.Append("True");
        return this.m_builder;
      }

      internal override StringBuilder VisitFalse(
        FalseExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_builder.Append("False");
        return this.m_builder;
      }

      protected override StringBuilder BooleanLiteralAsCql(
        BoolLiteral literal,
        bool skipIsNotNull)
      {
        return literal.AsEsql(this.m_builder, this.m_blockAlias, skipIsNotNull);
      }

      protected override StringBuilder NotExprAsCql(
        NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_builder.Append("NOT(");
        expression.Child.Accept<StringBuilder>((Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>) this);
        this.m_builder.Append(")");
        return this.m_builder;
      }

      internal override StringBuilder VisitAnd(
        AndExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression, ExprType.And);
      }

      internal override StringBuilder VisitOr(
        OrExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression, ExprType.Or);
      }

      private StringBuilder VisitAndOr(
        TreeExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        ExprType kind)
      {
        this.m_builder.Append('(');
        bool flag = true;
        foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> child in expression.Children)
        {
          if (!flag)
          {
            if (kind == ExprType.And)
              this.m_builder.Append(" AND ");
            else
              this.m_builder.Append(" OR ");
          }
          flag = false;
          child.Accept<StringBuilder>((Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>) this);
        }
        this.m_builder.Append(')');
        return this.m_builder;
      }
    }

    private sealed class AsCqtVisitor : BoolExpression.AsCqlVisitor<DbExpression>
    {
      private readonly DbExpression m_row;

      internal static DbExpression AsCqt(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        DbExpression row)
      {
        BoolExpression.AsCqtVisitor asCqtVisitor = new BoolExpression.AsCqtVisitor(row);
        return expression.Accept<DbExpression>((Visitor<DomainConstraint<BoolLiteral, Constant>, DbExpression>) asCqtVisitor);
      }

      private AsCqtVisitor(DbExpression row)
      {
        this.m_row = row;
      }

      internal override DbExpression VisitTrue(
        TrueExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return (DbExpression) DbExpressionBuilder.True;
      }

      internal override DbExpression VisitFalse(
        FalseExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return (DbExpression) DbExpressionBuilder.False;
      }

      protected override DbExpression BooleanLiteralAsCql(
        BoolLiteral literal,
        bool skipIsNotNull)
      {
        return literal.AsCqt(this.m_row, skipIsNotNull);
      }

      protected override DbExpression NotExprAsCql(
        NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return (DbExpression) expression.Child.Accept<DbExpression>((Visitor<DomainConstraint<BoolLiteral, Constant>, DbExpression>) this).Not();
      }

      internal override DbExpression VisitAnd(
        AndExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression, new Func<DbExpression, DbExpression, DbExpression>(DbExpressionBuilder.And));
      }

      internal override DbExpression VisitOr(
        OrExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression, new Func<DbExpression, DbExpression, DbExpression>(DbExpressionBuilder.Or));
      }

      private DbExpression VisitAndOr(
        TreeExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        Func<DbExpression, DbExpression, DbExpression> op)
      {
        DbExpression dbExpression = (DbExpression) null;
        foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> child in expression.Children)
          dbExpression = dbExpression != null ? op(dbExpression, child.Accept<DbExpression>((Visitor<DomainConstraint<BoolLiteral, Constant>, DbExpression>) this)) : child.Accept<DbExpression>((Visitor<DomainConstraint<BoolLiteral, Constant>, DbExpression>) this);
        return dbExpression;
      }
    }

    private class AsUserStringVisitor : Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>
    {
      private readonly StringBuilder m_builder;
      private readonly string m_blockAlias;
      private bool m_skipIsNotNull;

      private AsUserStringVisitor(StringBuilder builder, string blockAlias)
      {
        this.m_builder = builder;
        this.m_blockAlias = blockAlias;
        this.m_skipIsNotNull = true;
      }

      internal static StringBuilder AsUserString(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        StringBuilder builder,
        string blockAlias)
      {
        BoolExpression.AsUserStringVisitor userStringVisitor = new BoolExpression.AsUserStringVisitor(builder, blockAlias);
        return expression.Accept<StringBuilder>((Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>) userStringVisitor);
      }

      internal override StringBuilder VisitTrue(
        TrueExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_builder.Append("True");
        return this.m_builder;
      }

      internal override StringBuilder VisitFalse(
        FalseExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_builder.Append("False");
        return this.m_builder;
      }

      internal override StringBuilder VisitTerm(
        TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        BoolLiteral boolLiteral = BoolExpression.GetBoolLiteral(expression);
        if (boolLiteral is ScalarRestriction || boolLiteral is TypeRestriction)
          return boolLiteral.AsUserString(this.m_builder, Strings.ViewGen_EntityInstanceToken, this.m_skipIsNotNull);
        return boolLiteral.AsUserString(this.m_builder, this.m_blockAlias, this.m_skipIsNotNull);
      }

      internal override StringBuilder VisitNot(
        NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_skipIsNotNull = false;
        TermExpr<DomainConstraint<BoolLiteral, Constant>> child = expression.Child as TermExpr<DomainConstraint<BoolLiteral, Constant>>;
        if (child != null)
          return BoolExpression.GetBoolLiteral(child).AsNegatedUserString(this.m_builder, this.m_blockAlias, this.m_skipIsNotNull);
        this.m_builder.Append("NOT(");
        expression.Child.Accept<StringBuilder>((Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>) this);
        this.m_builder.Append(")");
        return this.m_builder;
      }

      internal override StringBuilder VisitAnd(
        AndExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression, ExprType.And);
      }

      internal override StringBuilder VisitOr(
        OrExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression, ExprType.Or);
      }

      private StringBuilder VisitAndOr(
        TreeExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        ExprType kind)
      {
        this.m_builder.Append('(');
        bool flag = true;
        foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> child in expression.Children)
        {
          if (!flag)
          {
            if (kind == ExprType.And)
              this.m_builder.Append(" AND ");
            else
              this.m_builder.Append(" OR ");
          }
          flag = false;
          child.Accept<StringBuilder>((Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>) this);
        }
        this.m_builder.Append(')');
        return this.m_builder;
      }
    }

    private class TermVisitor : Visitor<DomainConstraint<BoolLiteral, Constant>, IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>>>
    {
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "allowAllOperators", Scope = "member", Target = "System.Data.Entity.Core.Mapping.ViewGeneration.Structures.BoolExpression+TermVisitor.#.ctor(System.Boolean)")]
      private TermVisitor(bool allowAllOperators)
      {
      }

      internal static IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>> GetTerms(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        bool allowAllOperators)
      {
        BoolExpression.TermVisitor termVisitor = new BoolExpression.TermVisitor(allowAllOperators);
        return expression.Accept<IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>>>) termVisitor);
      }

      internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>> VisitTrue(
        TrueExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        yield break;
      }

      internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>> VisitFalse(
        FalseExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        yield break;
      }

      internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>> VisitTerm(
        TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        yield return expression;
      }

      internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>> VisitNot(
        NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitTreeNode((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression);
      }

      private IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>> VisitTreeNode(
        TreeExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> child in expression.Children)
        {
          foreach (TermExpr<DomainConstraint<BoolLiteral, Constant>> termExpr in child.Accept<IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>>>((Visitor<DomainConstraint<BoolLiteral, Constant>, IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>>>) this))
            yield return termExpr;
        }
      }

      internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>> VisitAnd(
        AndExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitTreeNode((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression);
      }

      internal override IEnumerable<TermExpr<DomainConstraint<BoolLiteral, Constant>>> VisitOr(
        OrExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitTreeNode((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression);
      }
    }

    private class CompactStringVisitor : Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>
    {
      private StringBuilder m_builder;

      private CompactStringVisitor(StringBuilder builder)
      {
        this.m_builder = builder;
      }

      internal static StringBuilder ToBuilder(
        BoolExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        StringBuilder builder)
      {
        BoolExpression.CompactStringVisitor compactStringVisitor = new BoolExpression.CompactStringVisitor(builder);
        return expression.Accept<StringBuilder>((Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>) compactStringVisitor);
      }

      internal override StringBuilder VisitTrue(
        TrueExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_builder.Append("True");
        return this.m_builder;
      }

      internal override StringBuilder VisitFalse(
        FalseExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_builder.Append("False");
        return this.m_builder;
      }

      internal override StringBuilder VisitTerm(
        TermExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        BoolExpression.GetBoolLiteral(expression).ToCompactString(this.m_builder);
        return this.m_builder;
      }

      internal override StringBuilder VisitNot(
        NotExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        this.m_builder.Append("NOT(");
        expression.Child.Accept<StringBuilder>((Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>) this);
        this.m_builder.Append(")");
        return this.m_builder;
      }

      internal override StringBuilder VisitAnd(
        AndExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression, "AND");
      }

      internal override StringBuilder VisitOr(
        OrExpr<DomainConstraint<BoolLiteral, Constant>> expression)
      {
        return this.VisitAndOr((TreeExpr<DomainConstraint<BoolLiteral, Constant>>) expression, "OR");
      }

      private StringBuilder VisitAndOr(
        TreeExpr<DomainConstraint<BoolLiteral, Constant>> expression,
        string opAsString)
      {
        List<string> stringList = new List<string>();
        StringBuilder builder = this.m_builder;
        foreach (BoolExpr<DomainConstraint<BoolLiteral, Constant>> child in expression.Children)
        {
          this.m_builder = new StringBuilder();
          child.Accept<StringBuilder>((Visitor<DomainConstraint<BoolLiteral, Constant>, StringBuilder>) this);
          stringList.Add(this.m_builder.ToString());
        }
        this.m_builder = builder;
        this.m_builder.Append('(');
        StringUtil.ToSeparatedStringSorted(this.m_builder, (IEnumerable) stringList, " " + opAsString + " ");
        this.m_builder.Append(')');
        return this.m_builder;
      }
    }
  }
}
