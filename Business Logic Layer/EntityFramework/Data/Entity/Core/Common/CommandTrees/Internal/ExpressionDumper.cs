// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.Internal.ExpressionDumper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Common.CommandTrees.Internal
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal abstract class ExpressionDumper : DbExpressionVisitor
  {
    internal void Begin(string name)
    {
      this.Begin(name, (Dictionary<string, object>) null);
    }

    internal abstract void Begin(string name, Dictionary<string, object> attrs);

    internal abstract void End(string name);

    internal void Dump(DbExpression target)
    {
      target.Accept((DbExpressionVisitor) this);
    }

    internal void Dump(DbExpression e, string name)
    {
      this.Begin(name);
      this.Dump(e);
      this.End(name);
    }

    internal void Dump(DbExpressionBinding binding, string name)
    {
      this.Begin(name);
      this.Dump(binding);
      this.End(name);
    }

    internal void Dump(DbExpressionBinding binding)
    {
      this.Begin("DbExpressionBinding", "VariableName", (object) binding.VariableName);
      this.Begin("Expression");
      this.Dump(binding.Expression);
      this.End("Expression");
      this.End("DbExpressionBinding");
    }

    internal void Dump(DbGroupExpressionBinding binding, string name)
    {
      this.Begin(name);
      this.Dump(binding);
      this.End(name);
    }

    internal void Dump(DbGroupExpressionBinding binding)
    {
      this.Begin("DbGroupExpressionBinding", "VariableName", (object) binding.VariableName, "GroupVariableName", (object) binding.GroupVariableName);
      this.Begin("Expression");
      this.Dump(binding.Expression);
      this.End("Expression");
      this.End("DbGroupExpressionBinding");
    }

    internal void Dump(IEnumerable<DbExpression> exprs, string pluralName, string singularName)
    {
      this.Begin(pluralName);
      foreach (DbExpression expr in exprs)
      {
        this.Begin(singularName);
        this.Dump(expr);
        this.End(singularName);
      }
      this.End(pluralName);
    }

    internal void Dump(IEnumerable<FunctionParameter> paramList)
    {
      this.Begin("Parameters");
      foreach (FunctionParameter functionParameter in paramList)
      {
        this.Begin("Parameter", "Name", (object) functionParameter.Name);
        this.Dump(functionParameter.TypeUsage, "ParameterType");
        this.End("Parameter");
      }
      this.End("Parameters");
    }

    internal void Dump(TypeUsage type, string name)
    {
      this.Begin(name);
      this.Dump(type);
      this.End(name);
    }

    internal void Dump(TypeUsage type)
    {
      Dictionary<string, object> attrs = new Dictionary<string, object>();
      foreach (Facet facet in type.Facets)
        attrs.Add(facet.Name, facet.Value);
      this.Begin("TypeUsage", attrs);
      this.Dump(type.EdmType);
      this.End("TypeUsage");
    }

    internal void Dump(EdmType type, string name)
    {
      this.Begin(name);
      this.Dump(type);
      this.End(name);
    }

    internal void Dump(EdmType type)
    {
      this.Begin("EdmType", "BuiltInTypeKind", (object) Enum.GetName(typeof (BuiltInTypeKind), (object) type.BuiltInTypeKind), "Namespace", (object) type.NamespaceName, "Name", (object) type.Name);
      this.End("EdmType");
    }

    internal void Dump(RelationshipType type, string name)
    {
      this.Begin(name);
      this.Dump(type);
      this.End(name);
    }

    internal void Dump(RelationshipType type)
    {
      this.Begin("RelationshipType", "Namespace", (object) type.NamespaceName, "Name", (object) type.Name);
      this.End("RelationshipType");
    }

    internal void Dump(EdmFunction function)
    {
      this.Begin("Function", "Name", (object) function.Name, "Namespace", (object) function.NamespaceName);
      this.Dump((IEnumerable<FunctionParameter>) function.Parameters);
      if (function.ReturnParameters.Count == 1)
      {
        this.Dump(function.ReturnParameters[0].TypeUsage, "ReturnType");
      }
      else
      {
        this.Begin("ReturnTypes");
        foreach (FunctionParameter returnParameter in function.ReturnParameters)
          this.Dump(returnParameter.TypeUsage, returnParameter.Name);
        this.End("ReturnTypes");
      }
      this.End("Function");
    }

    internal void Dump(EdmProperty prop)
    {
      this.Begin("Property", "Name", (object) prop.Name, "Nullable", (object) prop.Nullable);
      this.Dump((EdmType) prop.DeclaringType, "DeclaringType");
      this.Dump(prop.TypeUsage, "PropertyType");
      this.End("Property");
    }

    internal void Dump(RelationshipEndMember end, string name)
    {
      this.Begin(name);
      this.Begin("RelationshipEndMember", "Name", (object) end.Name, "RelationshipMultiplicity", (object) Enum.GetName(typeof (RelationshipMultiplicity), (object) end.RelationshipMultiplicity));
      this.Dump((EdmType) end.DeclaringType, "DeclaringRelation");
      this.Dump(end.TypeUsage, "EndType");
      this.End("RelationshipEndMember");
      this.End(name);
    }

    internal void Dump(NavigationProperty navProp, string name)
    {
      this.Begin(name);
      this.Begin("NavigationProperty", "Name", (object) navProp.Name, "RelationshipTypeName", (object) navProp.RelationshipType.FullName, "ToEndMemberName", (object) navProp.ToEndMember.Name);
      this.Dump((EdmType) navProp.DeclaringType, "DeclaringType");
      this.Dump(navProp.TypeUsage, "PropertyType");
      this.End("NavigationProperty");
      this.End(name);
    }

    internal void Dump(DbLambda lambda)
    {
      this.Begin("DbLambda");
      this.Dump(lambda.Variables.Cast<DbExpression>(), "Variables", "Variable");
      this.Dump(lambda.Body, "Body");
      this.End("DbLambda");
    }

    private void Begin(DbExpression expr)
    {
      this.Begin(expr, new Dictionary<string, object>());
    }

    private void Begin(DbExpression expr, Dictionary<string, object> attrs)
    {
      attrs.Add("DbExpressionKind", (object) Enum.GetName(typeof (DbExpressionKind), (object) expr.ExpressionKind));
      this.Begin(expr.GetType().Name, attrs);
      this.Dump(expr.ResultType, "ResultType");
    }

    private void Begin(DbExpression expr, string attributeName, object attributeValue)
    {
      this.Begin(expr, new Dictionary<string, object>()
      {
        {
          attributeName,
          attributeValue
        }
      });
    }

    private void Begin(string expr, string attributeName, object attributeValue)
    {
      this.Begin(expr, new Dictionary<string, object>()
      {
        {
          attributeName,
          attributeValue
        }
      });
    }

    private void Begin(
      string expr,
      string attributeName1,
      object attributeValue1,
      string attributeName2,
      object attributeValue2)
    {
      this.Begin(expr, new Dictionary<string, object>()
      {
        {
          attributeName1,
          attributeValue1
        },
        {
          attributeName2,
          attributeValue2
        }
      });
    }

    private void Begin(
      string expr,
      string attributeName1,
      object attributeValue1,
      string attributeName2,
      object attributeValue2,
      string attributeName3,
      object attributeValue3)
    {
      this.Begin(expr, new Dictionary<string, object>()
      {
        {
          attributeName1,
          attributeValue1
        },
        {
          attributeName2,
          attributeValue2
        },
        {
          attributeName3,
          attributeValue3
        }
      });
    }

    private void End(DbExpression expr)
    {
      this.End(expr.GetType().Name);
    }

    private void BeginUnary(DbUnaryExpression e)
    {
      this.Begin((DbExpression) e);
      this.Begin("Argument");
      this.Dump(e.Argument);
      this.End("Argument");
    }

    private void BeginBinary(DbBinaryExpression e)
    {
      this.Begin((DbExpression) e);
      this.Begin("Left");
      this.Dump(e.Left);
      this.End("Left");
      this.Begin("Right");
      this.Dump(e.Right);
      this.End("Right");
    }

    public override void Visit(DbExpression e)
    {
      Check.NotNull<DbExpression>(e, nameof (e));
      this.Begin(e);
      this.End(e);
    }

    public override void Visit(DbConstantExpression e)
    {
      Check.NotNull<DbConstantExpression>(e, nameof (e));
      this.Begin((DbExpression) e, new Dictionary<string, object>()
      {
        {
          "Value",
          e.Value
        }
      });
      this.End((DbExpression) e);
    }

    public override void Visit(DbNullExpression e)
    {
      Check.NotNull<DbNullExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbVariableReferenceExpression e)
    {
      Check.NotNull<DbVariableReferenceExpression>(e, nameof (e));
      this.Begin((DbExpression) e, new Dictionary<string, object>()
      {
        {
          "VariableName",
          (object) e.VariableName
        }
      });
      this.End((DbExpression) e);
    }

    public override void Visit(DbParameterReferenceExpression e)
    {
      Check.NotNull<DbParameterReferenceExpression>(e, nameof (e));
      this.Begin((DbExpression) e, new Dictionary<string, object>()
      {
        {
          "ParameterName",
          (object) e.ParameterName
        }
      });
      this.End((DbExpression) e);
    }

    public override void Visit(DbFunctionExpression e)
    {
      Check.NotNull<DbFunctionExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Function);
      this.Dump((IEnumerable<DbExpression>) e.Arguments, "Arguments", "Argument");
      this.End((DbExpression) e);
    }

    public override void Visit(DbLambdaExpression expression)
    {
      Check.NotNull<DbLambdaExpression>(expression, nameof (expression));
      this.Begin((DbExpression) expression);
      this.Dump(expression.Lambda);
      this.Dump((IEnumerable<DbExpression>) expression.Arguments, "Arguments", "Argument");
      this.End((DbExpression) expression);
    }

    public override void Visit(DbPropertyExpression e)
    {
      Check.NotNull<DbPropertyExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      RelationshipEndMember property = e.Property as RelationshipEndMember;
      if (property != null)
        this.Dump(property, "Property");
      else if (Helper.IsNavigationProperty(e.Property))
        this.Dump((NavigationProperty) e.Property, "Property");
      else
        this.Dump((EdmProperty) e.Property);
      if (e.Instance != null)
        this.Dump(e.Instance, "Instance");
      this.End((DbExpression) e);
    }

    public override void Visit(DbComparisonExpression e)
    {
      Check.NotNull<DbComparisonExpression>(e, nameof (e));
      this.BeginBinary((DbBinaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbLikeExpression e)
    {
      Check.NotNull<DbLikeExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Argument, "Argument");
      this.Dump(e.Pattern, "Pattern");
      this.Dump(e.Escape, "Escape");
      this.End((DbExpression) e);
    }

    public override void Visit(DbLimitExpression e)
    {
      Check.NotNull<DbLimitExpression>(e, nameof (e));
      this.Begin((DbExpression) e, "WithTies", (object) e.WithTies);
      this.Dump(e.Argument, "Argument");
      this.Dump(e.Limit, "Limit");
      this.End((DbExpression) e);
    }

    public override void Visit(DbIsNullExpression e)
    {
      Check.NotNull<DbIsNullExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbArithmeticExpression e)
    {
      Check.NotNull<DbArithmeticExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump((IEnumerable<DbExpression>) e.Arguments, "Arguments", "Argument");
      this.End((DbExpression) e);
    }

    public override void Visit(DbAndExpression e)
    {
      Check.NotNull<DbAndExpression>(e, nameof (e));
      this.BeginBinary((DbBinaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbOrExpression e)
    {
      Check.NotNull<DbOrExpression>(e, nameof (e));
      this.BeginBinary((DbBinaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbInExpression e)
    {
      Check.NotNull<DbInExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Item);
      this.Dump((IEnumerable<DbExpression>) e.List, "List", "Item");
      this.End((DbExpression) e);
    }

    public override void Visit(DbNotExpression e)
    {
      Check.NotNull<DbNotExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbDistinctExpression e)
    {
      Check.NotNull<DbDistinctExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbElementExpression e)
    {
      Check.NotNull<DbElementExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbIsEmptyExpression e)
    {
      Check.NotNull<DbIsEmptyExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbUnionAllExpression e)
    {
      Check.NotNull<DbUnionAllExpression>(e, nameof (e));
      this.BeginBinary((DbBinaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbIntersectExpression e)
    {
      Check.NotNull<DbIntersectExpression>(e, nameof (e));
      this.BeginBinary((DbBinaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbExceptExpression e)
    {
      Check.NotNull<DbExceptExpression>(e, nameof (e));
      this.BeginBinary((DbBinaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbTreatExpression e)
    {
      Check.NotNull<DbTreatExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbIsOfExpression e)
    {
      Check.NotNull<DbIsOfExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.Dump(e.OfType, "OfType");
      this.End((DbExpression) e);
    }

    public override void Visit(DbCastExpression e)
    {
      Check.NotNull<DbCastExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbCaseExpression e)
    {
      Check.NotNull<DbCaseExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump((IEnumerable<DbExpression>) e.When, "Whens", "When");
      this.Dump((IEnumerable<DbExpression>) e.Then, "Thens", "Then");
      this.Dump(e.Else, "Else");
    }

    public override void Visit(DbOfTypeExpression e)
    {
      Check.NotNull<DbOfTypeExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.Dump(e.OfType, "OfType");
      this.End((DbExpression) e);
    }

    public override void Visit(DbNewInstanceExpression e)
    {
      Check.NotNull<DbNewInstanceExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump((IEnumerable<DbExpression>) e.Arguments, "Arguments", "Argument");
      if (e.HasRelatedEntityReferences)
      {
        this.Begin("RelatedEntityReferences");
        foreach (DbRelatedEntityRef relatedEntityReference in e.RelatedEntityReferences)
        {
          this.Begin("DbRelatedEntityRef");
          this.Dump(relatedEntityReference.SourceEnd, "SourceEnd");
          this.Dump(relatedEntityReference.TargetEnd, "TargetEnd");
          this.Dump(relatedEntityReference.TargetEntityReference, "TargetEntityReference");
          this.End("DbRelatedEntityRef");
        }
        this.End("RelatedEntityReferences");
      }
      this.End((DbExpression) e);
    }

    public override void Visit(DbRelationshipNavigationExpression e)
    {
      Check.NotNull<DbRelationshipNavigationExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.NavigateFrom, "NavigateFrom");
      this.Dump(e.NavigateTo, "NavigateTo");
      this.Dump(e.Relationship, "Relationship");
      this.Dump(e.NavigationSource, "NavigationSource");
      this.End((DbExpression) e);
    }

    public override void Visit(DbRefExpression e)
    {
      Check.NotNull<DbRefExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbDerefExpression e)
    {
      Check.NotNull<DbDerefExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbRefKeyExpression e)
    {
      Check.NotNull<DbRefKeyExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbEntityRefExpression e)
    {
      Check.NotNull<DbEntityRefExpression>(e, nameof (e));
      this.BeginUnary((DbUnaryExpression) e);
      this.End((DbExpression) e);
    }

    public override void Visit(DbScanExpression e)
    {
      Check.NotNull<DbScanExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Begin("Target", "Name", (object) e.Target.Name, "Container", (object) e.Target.EntityContainer.Name);
      this.Dump((EdmType) e.Target.ElementType, "TargetElementType");
      this.End("Target");
      this.End((DbExpression) e);
    }

    public override void Visit(DbFilterExpression e)
    {
      Check.NotNull<DbFilterExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Input, "Input");
      this.Dump(e.Predicate, "Predicate");
      this.End((DbExpression) e);
    }

    public override void Visit(DbProjectExpression e)
    {
      Check.NotNull<DbProjectExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Input, "Input");
      this.Dump(e.Projection, "Projection");
      this.End((DbExpression) e);
    }

    public override void Visit(DbCrossJoinExpression e)
    {
      Check.NotNull<DbCrossJoinExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Begin("Inputs");
      foreach (DbExpressionBinding input in (IEnumerable<DbExpressionBinding>) e.Inputs)
        this.Dump(input, "Input");
      this.End("Inputs");
      this.End((DbExpression) e);
    }

    public override void Visit(DbJoinExpression e)
    {
      Check.NotNull<DbJoinExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Left, "Left");
      this.Dump(e.Right, "Right");
      this.Dump(e.JoinCondition, "JoinCondition");
      this.End((DbExpression) e);
    }

    public override void Visit(DbApplyExpression e)
    {
      Check.NotNull<DbApplyExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Input, "Input");
      this.Dump(e.Apply, "Apply");
      this.End((DbExpression) e);
    }

    public override void Visit(DbGroupByExpression e)
    {
      Check.NotNull<DbGroupByExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Input, "Input");
      this.Dump((IEnumerable<DbExpression>) e.Keys, "Keys", "Key");
      this.Begin("Aggregates");
      foreach (DbAggregate aggregate in (IEnumerable<DbAggregate>) e.Aggregates)
      {
        DbFunctionAggregate functionAggregate = aggregate as DbFunctionAggregate;
        if (functionAggregate != null)
        {
          this.Begin("DbFunctionAggregate");
          this.Dump(functionAggregate.Function);
          this.Dump((IEnumerable<DbExpression>) functionAggregate.Arguments, "Arguments", "Argument");
          this.End("DbFunctionAggregate");
        }
        else
        {
          DbGroupAggregate dbGroupAggregate = aggregate as DbGroupAggregate;
          this.Begin("DbGroupAggregate");
          this.Dump((IEnumerable<DbExpression>) dbGroupAggregate.Arguments, "Arguments", "Argument");
          this.End("DbGroupAggregate");
        }
      }
      this.End("Aggregates");
      this.End((DbExpression) e);
    }

    protected virtual void Dump(IList<DbSortClause> sortOrder)
    {
      this.Begin("SortOrder");
      foreach (DbSortClause dbSortClause in (IEnumerable<DbSortClause>) sortOrder)
      {
        string str = dbSortClause.Collation ?? "";
        this.Begin("DbSortClause", "Ascending", (object) dbSortClause.Ascending, "Collation", (object) str);
        this.Dump(dbSortClause.Expression, "Expression");
        this.End("DbSortClause");
      }
      this.End("SortOrder");
    }

    public override void Visit(DbSkipExpression e)
    {
      Check.NotNull<DbSkipExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Input, "Input");
      this.Dump(e.SortOrder);
      this.Dump(e.Count, "Count");
      this.End((DbExpression) e);
    }

    public override void Visit(DbSortExpression e)
    {
      Check.NotNull<DbSortExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Input, "Input");
      this.Dump(e.SortOrder);
      this.End((DbExpression) e);
    }

    public override void Visit(DbQuantifierExpression e)
    {
      Check.NotNull<DbQuantifierExpression>(e, nameof (e));
      this.Begin((DbExpression) e);
      this.Dump(e.Input, "Input");
      this.Dump(e.Predicate, "Predicate");
      this.End((DbExpression) e);
    }
  }
}
