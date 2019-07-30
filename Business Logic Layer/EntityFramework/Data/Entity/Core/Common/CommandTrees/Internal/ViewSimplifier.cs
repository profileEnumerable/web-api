// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.Internal.ViewSimplifier
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Common.CommandTrees.Internal
{
  internal class ViewSimplifier
  {
    private static readonly Func<DbExpression, bool> _patternEntityConstructor = Patterns.MatchProject(Patterns.AnyExpression, Patterns.And(Patterns.MatchEntityType, Patterns.Or(Patterns.MatchNewInstance(), Patterns.MatchCase(Patterns.AnyExpressions, Patterns.MatchForAll(Patterns.MatchNewInstance()), Patterns.MatchNewInstance()))));
    private static readonly Func<DbExpression, bool> _patternNestedTphDiscriminator = Patterns.MatchProject(Patterns.MatchFilter(Patterns.MatchProject(Patterns.MatchFilter(Patterns.AnyExpression, Patterns.Or(Patterns.MatchKind(DbExpressionKind.Equals), Patterns.MatchKind(DbExpressionKind.Or))), Patterns.And(Patterns.MatchRowType, Patterns.MatchNewInstance(Patterns.MatchForAll(Patterns.Or(Patterns.And(Patterns.MatchNewInstance(), Patterns.MatchComplexType), Patterns.MatchKind(DbExpressionKind.Property), Patterns.MatchKind(DbExpressionKind.Case)))))), Patterns.Or(Patterns.MatchKind(DbExpressionKind.Property), Patterns.MatchKind(DbExpressionKind.Or))), Patterns.And(Patterns.MatchEntityType, Patterns.MatchCase(Patterns.MatchForAll(Patterns.MatchKind(DbExpressionKind.Property)), Patterns.MatchForAll(Patterns.MatchKind(DbExpressionKind.NewInstance)), Patterns.MatchKind(DbExpressionKind.NewInstance))));
    private static readonly Func<DbExpression, bool> _patternCase = Patterns.MatchKind(DbExpressionKind.Case);
    private static readonly Func<DbExpression, bool> _patternCollapseNestedProjection = Patterns.MatchProject(Patterns.MatchProject(Patterns.AnyExpression, Patterns.MatchKind(DbExpressionKind.NewInstance)), Patterns.AnyExpression);
    private readonly EntitySetBase extent;
    private bool doNotProcess;

    internal static DbQueryCommandTree SimplifyView(
      EntitySetBase extent,
      DbQueryCommandTree view)
    {
      view = new ViewSimplifier(extent).Simplify(view);
      return view;
    }

    private ViewSimplifier(EntitySetBase viewTarget)
    {
      this.extent = viewTarget;
    }

    private DbQueryCommandTree Simplify(DbQueryCommandTree view)
    {
      DbExpression query = PatternMatchRuleProcessor.Create(PatternMatchRule.Create(ViewSimplifier._patternCollapseNestedProjection, new Func<DbExpression, DbExpression>(ViewSimplifier.CollapseNestedProjection)), PatternMatchRule.Create(ViewSimplifier._patternCase, new Func<DbExpression, DbExpression>(ViewSimplifier.SimplifyCaseStatement)), PatternMatchRule.Create(ViewSimplifier._patternNestedTphDiscriminator, new Func<DbExpression, DbExpression>(ViewSimplifier.SimplifyNestedTphDiscriminator)), PatternMatchRule.Create(ViewSimplifier._patternEntityConstructor, new Func<DbExpression, DbExpression>(this.AddFkRelatedEntityRefs)))(view.Query);
      view = DbQueryCommandTree.FromValidExpression(view.MetadataWorkspace, view.DataSpace, query, view.UseDatabaseNullSemantics);
      return view;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private DbExpression AddFkRelatedEntityRefs(DbExpression viewConstructor)
    {
      if (this.doNotProcess)
        return (DbExpression) null;
      if (this.extent.BuiltInTypeKind != BuiltInTypeKind.EntitySet || this.extent.EntityContainer.DataSpace != DataSpace.CSpace)
      {
        this.doNotProcess = true;
        return (DbExpression) null;
      }
      EntitySet targetSet = (EntitySet) this.extent;
      List<AssociationSet> list1 = targetSet.EntityContainer.BaseEntitySets.Where<EntitySetBase>((Func<EntitySetBase, bool>) (es => es.BuiltInTypeKind == BuiltInTypeKind.AssociationSet)).Cast<AssociationSet>().Where<AssociationSet>((Func<AssociationSet, bool>) (assocSet =>
      {
        if (assocSet.ElementType.IsForeignKey)
          return assocSet.AssociationSetEnds.Any<AssociationSetEnd>((Func<AssociationSetEnd, bool>) (se => se.EntitySet == targetSet));
        return false;
      })).ToList<AssociationSet>();
      if (list1.Count == 0)
      {
        this.doNotProcess = true;
        return (DbExpression) null;
      }
      HashSet<Tuple<EntityType, AssociationSetEnd, ReferentialConstraint>> source = new HashSet<Tuple<EntityType, AssociationSetEnd, ReferentialConstraint>>();
      foreach (AssociationSet associationSet in list1)
      {
        ReferentialConstraint referentialConstraint = associationSet.ElementType.ReferentialConstraints[0];
        AssociationSetEnd associationSetEnd1 = associationSet.AssociationSetEnds[referentialConstraint.ToRole.Name];
        if (associationSetEnd1.EntitySet == targetSet)
        {
          EntityType elementType = (EntityType) TypeHelpers.GetEdmType<RefType>(associationSetEnd1.CorrespondingAssociationEndMember.TypeUsage).ElementType;
          AssociationSetEnd associationSetEnd2 = associationSet.AssociationSetEnds[referentialConstraint.FromRole.Name];
          source.Add(Tuple.Create<EntityType, AssociationSetEnd, ReferentialConstraint>(elementType, associationSetEnd2, referentialConstraint));
        }
      }
      if (source.Count == 0)
      {
        this.doNotProcess = true;
        return (DbExpression) null;
      }
      DbProjectExpression projectExpression = (DbProjectExpression) viewConstructor;
      List<DbNewInstanceExpression> instanceExpressionList = new List<DbNewInstanceExpression>();
      List<DbExpression> dbExpressionList1 = (List<DbExpression>) null;
      if (projectExpression.Projection.ExpressionKind == DbExpressionKind.Case)
      {
        DbCaseExpression projection = (DbCaseExpression) projectExpression.Projection;
        dbExpressionList1 = new List<DbExpression>(projection.When.Count);
        for (int index = 0; index < projection.When.Count; ++index)
        {
          dbExpressionList1.Add(projection.When[index]);
          instanceExpressionList.Add((DbNewInstanceExpression) projection.Then[index]);
        }
        instanceExpressionList.Add((DbNewInstanceExpression) projection.Else);
      }
      else
        instanceExpressionList.Add((DbNewInstanceExpression) projectExpression.Projection);
      bool flag = false;
      for (int index = 0; index < instanceExpressionList.Count; ++index)
      {
        DbNewInstanceExpression entityConstructor = instanceExpressionList[index];
        EntityType constructedEntityType = TypeHelpers.GetEdmType<EntityType>(entityConstructor.ResultType);
        List<DbRelatedEntityRef> list2 = source.Where<Tuple<EntityType, AssociationSetEnd, ReferentialConstraint>>((Func<Tuple<EntityType, AssociationSetEnd, ReferentialConstraint>, bool>) (psdt =>
        {
          if (constructedEntityType != psdt.Item1)
            return constructedEntityType.IsSubtypeOf((EdmType) psdt.Item1);
          return true;
        })).Select<Tuple<EntityType, AssociationSetEnd, ReferentialConstraint>, DbRelatedEntityRef>((Func<Tuple<EntityType, AssociationSetEnd, ReferentialConstraint>, DbRelatedEntityRef>) (psdt => ViewSimplifier.RelatedEntityRefFromAssociationSetEnd(constructedEntityType, entityConstructor, psdt.Item2, psdt.Item3))).ToList<DbRelatedEntityRef>();
        if (list2.Count > 0)
        {
          if (entityConstructor.HasRelatedEntityReferences)
            list2 = entityConstructor.RelatedEntityReferences.Concat<DbRelatedEntityRef>((IEnumerable<DbRelatedEntityRef>) list2).ToList<DbRelatedEntityRef>();
          entityConstructor = DbExpressionBuilder.CreateNewEntityWithRelationshipsExpression(constructedEntityType, entityConstructor.Arguments, (IList<DbRelatedEntityRef>) list2);
          instanceExpressionList[index] = entityConstructor;
          flag = true;
        }
      }
      DbExpression dbExpression = (DbExpression) null;
      if (flag)
      {
        if (dbExpressionList1 != null)
        {
          List<DbExpression> dbExpressionList2 = new List<DbExpression>(dbExpressionList1.Count);
          List<DbExpression> dbExpressionList3 = new List<DbExpression>(dbExpressionList1.Count);
          for (int index = 0; index < dbExpressionList1.Count; ++index)
          {
            dbExpressionList2.Add(dbExpressionList1[index]);
            dbExpressionList3.Add((DbExpression) instanceExpressionList[index]);
          }
          dbExpression = (DbExpression) projectExpression.Input.Project((DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) dbExpressionList2, (IEnumerable<DbExpression>) dbExpressionList3, (DbExpression) instanceExpressionList[dbExpressionList1.Count]));
        }
        else
          dbExpression = (DbExpression) projectExpression.Input.Project((DbExpression) instanceExpressionList[0]);
      }
      this.doNotProcess = true;
      return dbExpression;
    }

    private static DbRelatedEntityRef RelatedEntityRefFromAssociationSetEnd(
      EntityType constructedEntityType,
      DbNewInstanceExpression entityConstructor,
      AssociationSetEnd principalSetEnd,
      ReferentialConstraint fkConstraint)
    {
      EntityType elementType = (EntityType) TypeHelpers.GetEdmType<RefType>(fkConstraint.FromRole.TypeUsage).ElementType;
      IEnumerable<Tuple<string, DbExpression>> source = constructedEntityType.Properties.Select<EdmProperty, Tuple<EdmProperty, DbExpression>>((Func<EdmProperty, int, Tuple<EdmProperty, DbExpression>>) ((p, idx) => Tuple.Create<EdmProperty, DbExpression>(p, entityConstructor.Arguments[idx]))).Join<Tuple<EdmProperty, DbExpression>, Tuple<EdmProperty, EdmProperty>, EdmProperty, Tuple<string, DbExpression>>(fkConstraint.FromProperties.Select<EdmProperty, Tuple<EdmProperty, EdmProperty>>((Func<EdmProperty, int, Tuple<EdmProperty, EdmProperty>>) ((fp, idx) => Tuple.Create<EdmProperty, EdmProperty>(fp, fkConstraint.ToProperties[idx]))), (Func<Tuple<EdmProperty, DbExpression>, EdmProperty>) (pv => pv.Item1), (Func<Tuple<EdmProperty, EdmProperty>, EdmProperty>) (ft => ft.Item2), (Func<Tuple<EdmProperty, DbExpression>, Tuple<EdmProperty, EdmProperty>, Tuple<string, DbExpression>>) ((pv, ft) => Tuple.Create<string, DbExpression>(ft.Item1.Name, pv.Item2)));
      IList<DbExpression> dbExpressionList;
      if (fkConstraint.FromProperties.Count == 1)
      {
        dbExpressionList = (IList<DbExpression>) new DbExpression[1]
        {
          source.Single<Tuple<string, DbExpression>>().Item2
        };
      }
      else
      {
        Dictionary<string, DbExpression> keyValueMap = source.ToDictionary<Tuple<string, DbExpression>, string, DbExpression>((Func<Tuple<string, DbExpression>, string>) (pav => pav.Item1), (Func<Tuple<string, DbExpression>, DbExpression>) (pav => pav.Item2), (IEqualityComparer<string>) StringComparer.Ordinal);
        dbExpressionList = (IList<DbExpression>) ((IEnumerable<string>) elementType.KeyMemberNames).Select<string, DbExpression>((Func<string, DbExpression>) (memberName => keyValueMap[memberName])).ToList<DbExpression>();
      }
      DbRefExpression dbRefExpression = principalSetEnd.EntitySet.CreateRef(elementType, (IEnumerable<DbExpression>) dbExpressionList);
      return DbExpressionBuilder.CreateRelatedEntityRef(fkConstraint.ToRole, fkConstraint.FromRole, (DbExpression) dbRefExpression);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static DbExpression SimplifyNestedTphDiscriminator(DbExpression expression)
    {
      DbProjectExpression projectExpression = (DbProjectExpression) expression;
      DbFilterExpression booleanColumnFilter = (DbFilterExpression) projectExpression.Input.Expression;
      DbProjectExpression expression1 = (DbProjectExpression) booleanColumnFilter.Input.Expression;
      DbFilterExpression expression2 = (DbFilterExpression) expression1.Input.Expression;
      List<DbExpression> list1 = ViewSimplifier.FlattenOr(booleanColumnFilter.Predicate).ToList<DbExpression>();
      List<DbPropertyExpression> list2 = list1.OfType<DbPropertyExpression>().Where<DbPropertyExpression>((Func<DbPropertyExpression, bool>) (px =>
      {
        if (px.Instance.ExpressionKind == DbExpressionKind.VariableReference)
          return ((DbVariableReferenceExpression) px.Instance).VariableName == booleanColumnFilter.Input.VariableName;
        return false;
      })).ToList<DbPropertyExpression>();
      if (list1.Count != list2.Count)
        return (DbExpression) null;
      List<string> list3 = list2.Select<DbPropertyExpression, string>((Func<DbPropertyExpression, string>) (px => px.Property.Name)).ToList<string>();
      Dictionary<object, DbComparisonExpression> discriminatorPredicates = new Dictionary<object, DbComparisonExpression>();
      if (!TypeSemantics.IsEntityType(expression2.Input.VariableType) || !ViewSimplifier.TryMatchDiscriminatorPredicate(expression2, (Action<DbComparisonExpression, object>) ((compEx, discValue) => discriminatorPredicates.Add(discValue, compEx))))
        return (DbExpression) null;
      EdmProperty property1 = (EdmProperty) ((DbPropertyExpression) discriminatorPredicates.First<KeyValuePair<object, DbComparisonExpression>>().Value.Left).Property;
      DbNewInstanceExpression projection1 = (DbNewInstanceExpression) expression1.Projection;
      RowType edmType = TypeHelpers.GetEdmType<RowType>(projection1.ResultType);
      Dictionary<string, DbComparisonExpression> dictionary1 = new Dictionary<string, DbComparisonExpression>();
      Dictionary<string, DbComparisonExpression> dictionary2 = new Dictionary<string, DbComparisonExpression>();
      Dictionary<string, DbExpression> propertyValues = new Dictionary<string, DbExpression>(projection1.Arguments.Count);
      for (int index = 0; index < projection1.Arguments.Count; ++index)
      {
        string name = edmType.Properties[index].Name;
        DbExpression dbExpression = projection1.Arguments[index];
        if (list3.Contains(name))
        {
          if (dbExpression.ExpressionKind != DbExpressionKind.Case)
            return (DbExpression) null;
          DbCaseExpression dbCaseExpression = (DbCaseExpression) dbExpression;
          if (dbCaseExpression.When.Count != 1 || !TypeSemantics.IsBooleanType(dbCaseExpression.Then[0].ResultType) || (!TypeSemantics.IsBooleanType(dbCaseExpression.Else.ResultType) || dbCaseExpression.Then[0].ExpressionKind != DbExpressionKind.Constant) || (dbCaseExpression.Else.ExpressionKind != DbExpressionKind.Constant || !(bool) ((DbConstantExpression) dbCaseExpression.Then[0]).Value || (bool) ((DbConstantExpression) dbCaseExpression.Else).Value))
            return (DbExpression) null;
          DbPropertyExpression property2;
          object key;
          if (!ViewSimplifier.TryMatchPropertyEqualsValue(dbCaseExpression.When[0], expression1.Input.VariableName, out property2, out key) || property2.Property != property1 || !discriminatorPredicates.ContainsKey(key))
            return (DbExpression) null;
          dictionary1.Add(name, discriminatorPredicates[key]);
          dictionary2.Add(name, (DbComparisonExpression) dbCaseExpression.When[0]);
        }
        else
          propertyValues.Add(name, dbExpression);
      }
      DbExpression predicate = Helpers.BuildBalancedTreeInPlace<DbExpression>((IList<DbExpression>) new List<DbExpression>((IEnumerable<DbExpression>) dictionary1.Values), (Func<DbExpression, DbExpression, DbExpression>) ((left, right) => (DbExpression) left.Or(right)));
      DbFilterExpression input = expression2.Input.Filter(predicate);
      DbCaseExpression projection2 = (DbCaseExpression) projectExpression.Projection;
      List<DbExpression> dbExpressionList1 = new List<DbExpression>(projection2.When.Count);
      List<DbExpression> dbExpressionList2 = new List<DbExpression>(projection2.Then.Count);
      for (int index = 0; index < projection2.When.Count; ++index)
      {
        DbPropertyExpression propertyExpression = (DbPropertyExpression) projection2.When[index];
        DbNewInstanceExpression instanceExpression = (DbNewInstanceExpression) projection2.Then[index];
        DbComparisonExpression comparisonExpression;
        if (!dictionary2.TryGetValue(propertyExpression.Property.Name, out comparisonExpression))
          return (DbExpression) null;
        dbExpressionList1.Add((DbExpression) comparisonExpression);
        DbExpression dbExpression = ViewSimplifier.ValueSubstituter.Substitute((DbExpression) instanceExpression, projectExpression.Input.VariableName, propertyValues);
        dbExpressionList2.Add(dbExpression);
      }
      DbExpression elseExpression = ViewSimplifier.ValueSubstituter.Substitute(projection2.Else, projectExpression.Input.VariableName, propertyValues);
      DbCaseExpression dbCaseExpression1 = DbExpressionBuilder.Case((IEnumerable<DbExpression>) dbExpressionList1, (IEnumerable<DbExpression>) dbExpressionList2, elseExpression);
      return (DbExpression) input.BindAs(expression1.Input.VariableName).Project((DbExpression) dbCaseExpression1);
    }

    private static DbExpression SimplifyCaseStatement(DbExpression expression)
    {
      DbCaseExpression dbCaseExpression = (DbCaseExpression) expression;
      bool flag = false;
      List<DbExpression> dbExpressionList = new List<DbExpression>(dbCaseExpression.When.Count);
      foreach (DbExpression predicate in (IEnumerable<DbExpression>) dbCaseExpression.When)
      {
        DbExpression simplified;
        if (ViewSimplifier.TrySimplifyPredicate(predicate, out simplified))
        {
          dbExpressionList.Add(simplified);
          flag = true;
        }
        else
          dbExpressionList.Add(predicate);
      }
      if (!flag)
        return (DbExpression) null;
      return (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) dbExpressionList, (IEnumerable<DbExpression>) dbCaseExpression.Then, dbCaseExpression.Else);
    }

    private static bool TrySimplifyPredicate(DbExpression predicate, out DbExpression simplified)
    {
      simplified = (DbExpression) null;
      if (predicate.ExpressionKind != DbExpressionKind.Case)
        return false;
      DbCaseExpression dbCaseExpression = (DbCaseExpression) predicate;
      if (dbCaseExpression.Then.Count != 1 && dbCaseExpression.Then[0].ExpressionKind == DbExpressionKind.Constant || !true.Equals(((DbConstantExpression) dbCaseExpression.Then[0]).Value) || dbCaseExpression.Else != null && (dbCaseExpression.Else.ExpressionKind != DbExpressionKind.Constant || true.Equals(((DbConstantExpression) dbCaseExpression.Else).Value)))
        return false;
      simplified = dbCaseExpression.When[0];
      return true;
    }

    private static DbExpression CollapseNestedProjection(DbExpression expression)
    {
      DbProjectExpression projectExpression = (DbProjectExpression) expression;
      DbExpression projection1 = projectExpression.Projection;
      DbProjectExpression expression1 = (DbProjectExpression) projectExpression.Input.Expression;
      DbNewInstanceExpression projection2 = (DbNewInstanceExpression) expression1.Projection;
      Dictionary<string, DbExpression> varRefMemberBindings = new Dictionary<string, DbExpression>(projection2.Arguments.Count);
      RowType edmType = (RowType) projection2.ResultType.EdmType;
      for (int index = 0; index < edmType.Members.Count; ++index)
        varRefMemberBindings[edmType.Members[index].Name] = projection2.Arguments[index];
      ViewSimplifier.ProjectionCollapser projectionCollapser = new ViewSimplifier.ProjectionCollapser(varRefMemberBindings, projectExpression.Input);
      DbExpression projection3 = projectionCollapser.CollapseProjection(projection1);
      if (projectionCollapser.IsDoomed)
        return (DbExpression) null;
      return (DbExpression) expression1.Input.Project(projection3);
    }

    internal static IEnumerable<DbExpression> FlattenOr(
      DbExpression expression)
    {
      return Helpers.GetLeafNodes<DbExpression>(expression, (Func<DbExpression, bool>) (exp => exp.ExpressionKind != DbExpressionKind.Or), (Func<DbExpression, IEnumerable<DbExpression>>) (exp =>
      {
        DbOrExpression dbOrExpression = (DbOrExpression) exp;
        return (IEnumerable<DbExpression>) new DbExpression[2]
        {
          dbOrExpression.Left,
          dbOrExpression.Right
        };
      }));
    }

    internal static bool TryMatchDiscriminatorPredicate(
      DbFilterExpression filter,
      Action<DbComparisonExpression, object> onMatchedComparison)
    {
      EdmProperty edmProperty = (EdmProperty) null;
      foreach (DbExpression expression in ViewSimplifier.FlattenOr(filter.Predicate))
      {
        DbPropertyExpression property;
        object obj;
        if (!ViewSimplifier.TryMatchPropertyEqualsValue(expression, filter.Input.VariableName, out property, out obj))
          return false;
        if (edmProperty == null)
          edmProperty = (EdmProperty) property.Property;
        else if (edmProperty != property.Property)
          return false;
        onMatchedComparison((DbComparisonExpression) expression, obj);
      }
      return true;
    }

    internal static bool TryMatchPropertyEqualsValue(
      DbExpression expression,
      string propertyVariable,
      out DbPropertyExpression property,
      out object value)
    {
      property = (DbPropertyExpression) null;
      value = (object) null;
      if (expression.ExpressionKind != DbExpressionKind.Equals)
        return false;
      DbBinaryExpression binaryExpression = (DbBinaryExpression) expression;
      if (binaryExpression.Left.ExpressionKind != DbExpressionKind.Property)
        return false;
      property = (DbPropertyExpression) binaryExpression.Left;
      return ViewSimplifier.TryMatchConstant(binaryExpression.Right, out value) && property.Instance.ExpressionKind == DbExpressionKind.VariableReference && !(((DbVariableReferenceExpression) property.Instance).VariableName != propertyVariable);
    }

    private static bool TryMatchConstant(DbExpression expression, out object value)
    {
      if (expression.ExpressionKind == DbExpressionKind.Constant)
      {
        value = ((DbConstantExpression) expression).Value;
        return true;
      }
      if (expression.ExpressionKind == DbExpressionKind.Cast && expression.ResultType.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType && ViewSimplifier.TryMatchConstant(((DbUnaryExpression) expression).Argument, out value))
      {
        PrimitiveType edmType = (PrimitiveType) expression.ResultType.EdmType;
        value = Convert.ChangeType(value, edmType.ClrEquivalentType, (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      value = (object) null;
      return false;
    }

    private class ValueSubstituter : DefaultExpressionVisitor
    {
      private readonly string variableName;
      private readonly Dictionary<string, DbExpression> replacements;

      internal static DbExpression Substitute(
        DbExpression original,
        string referencedVariable,
        Dictionary<string, DbExpression> propertyValues)
      {
        return new ViewSimplifier.ValueSubstituter(referencedVariable, propertyValues).VisitExpression(original);
      }

      private ValueSubstituter(string varName, Dictionary<string, DbExpression> replValues)
      {
        this.variableName = varName;
        this.replacements = replValues;
      }

      public override DbExpression Visit(DbPropertyExpression expression)
      {
        Check.NotNull<DbPropertyExpression>(expression, nameof (expression));
        DbExpression dbExpression;
        return expression.Instance.ExpressionKind != DbExpressionKind.VariableReference || !(((DbVariableReferenceExpression) expression.Instance).VariableName == this.variableName) || !this.replacements.TryGetValue(expression.Property.Name, out dbExpression) ? base.Visit(expression) : dbExpression;
      }
    }

    private class ProjectionCollapser : DefaultExpressionVisitor
    {
      private readonly Dictionary<string, DbExpression> m_varRefMemberBindings;
      private readonly DbExpressionBinding m_outerBinding;
      private bool m_doomed;

      internal ProjectionCollapser(
        Dictionary<string, DbExpression> varRefMemberBindings,
        DbExpressionBinding outerBinding)
      {
        this.m_varRefMemberBindings = varRefMemberBindings;
        this.m_outerBinding = outerBinding;
      }

      internal DbExpression CollapseProjection(DbExpression expression)
      {
        return this.VisitExpression(expression);
      }

      public override DbExpression Visit(DbPropertyExpression property)
      {
        Check.NotNull<DbPropertyExpression>(property, nameof (property));
        if (property.Instance.ExpressionKind == DbExpressionKind.VariableReference && this.IsOuterBindingVarRef((DbVariableReferenceExpression) property.Instance))
          return this.m_varRefMemberBindings[property.Property.Name];
        return base.Visit(property);
      }

      public override DbExpression Visit(DbVariableReferenceExpression varRef)
      {
        Check.NotNull<DbVariableReferenceExpression>(varRef, nameof (varRef));
        if (this.IsOuterBindingVarRef(varRef))
          this.m_doomed = true;
        return base.Visit(varRef);
      }

      private bool IsOuterBindingVarRef(DbVariableReferenceExpression varRef)
      {
        return varRef.VariableName == this.m_outerBinding.VariableName;
      }

      internal bool IsDoomed
      {
        get
        {
          return this.m_doomed;
        }
      }
    }
  }
}
