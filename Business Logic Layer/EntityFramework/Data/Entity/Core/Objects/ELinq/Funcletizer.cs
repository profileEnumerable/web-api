// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ELinq.Funcletizer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Objects.ELinq
{
  internal sealed class Funcletizer
  {
    private readonly HashSet<Expression> _linqExpressionStack = new HashSet<Expression>();
    private const string s_parameterPrefix = "p__linq__";
    private readonly ParameterExpression _rootContextParameter;
    private readonly ObjectContext _rootContext;
    private readonly ConstantExpression _rootContextExpression;
    private readonly ReadOnlyCollection<ParameterExpression> _compiledQueryParameters;
    private readonly Funcletizer.Mode _mode;
    private long _parameterNumber;

    private Funcletizer(
      Funcletizer.Mode mode,
      ObjectContext rootContext,
      ParameterExpression rootContextParameter,
      ReadOnlyCollection<ParameterExpression> compiledQueryParameters)
    {
      this._mode = mode;
      this._rootContext = rootContext;
      this._rootContextParameter = rootContextParameter;
      this._compiledQueryParameters = compiledQueryParameters;
      if (this._rootContextParameter == null || this._rootContext == null)
        return;
      this._rootContextExpression = Expression.Constant((object) this._rootContext);
    }

    internal static Funcletizer CreateCompiledQueryEvaluationFuncletizer(
      ObjectContext rootContext,
      ParameterExpression rootContextParameter,
      ReadOnlyCollection<ParameterExpression> compiledQueryParameters)
    {
      return new Funcletizer(Funcletizer.Mode.CompiledQueryEvaluation, rootContext, rootContextParameter, compiledQueryParameters);
    }

    internal static Funcletizer CreateCompiledQueryLockdownFuncletizer()
    {
      return new Funcletizer(Funcletizer.Mode.CompiledQueryLockdown, (ObjectContext) null, (ParameterExpression) null, (ReadOnlyCollection<ParameterExpression>) null);
    }

    internal static Funcletizer CreateQueryFuncletizer(ObjectContext rootContext)
    {
      return new Funcletizer(Funcletizer.Mode.ConventionalQuery, rootContext, (ParameterExpression) null, (ReadOnlyCollection<ParameterExpression>) null);
    }

    internal ObjectContext RootContext
    {
      get
      {
        return this._rootContext;
      }
    }

    internal ParameterExpression RootContextParameter
    {
      get
      {
        return this._rootContextParameter;
      }
    }

    internal ConstantExpression RootContextExpression
    {
      get
      {
        return this._rootContextExpression;
      }
    }

    internal bool IsCompiledQuery
    {
      get
      {
        if (this._mode != Funcletizer.Mode.CompiledQueryEvaluation)
          return this._mode == Funcletizer.Mode.CompiledQueryLockdown;
        return true;
      }
    }

    internal Expression Funcletize(
      Expression expression,
      out Func<bool> recompileRequired)
    {
      expression = this.ReplaceRootContextParameter(expression);
      Func<Expression, bool> isClientConstant;
      Func<Expression, bool> isClientVariable;
      if (this._mode == Funcletizer.Mode.CompiledQueryEvaluation)
      {
        isClientConstant = Funcletizer.Nominate(expression, new Func<Expression, bool>(this.IsClosureExpression));
        isClientVariable = Funcletizer.Nominate(expression, new Func<Expression, bool>(this.IsCompiledQueryParameterVariable));
      }
      else if (this._mode == Funcletizer.Mode.CompiledQueryLockdown)
      {
        isClientConstant = Funcletizer.Nominate(expression, new Func<Expression, bool>(this.IsClosureExpression));
        isClientVariable = (Func<Expression, bool>) (exp => false);
      }
      else
      {
        isClientConstant = Funcletizer.Nominate(expression, new Func<Expression, bool>(this.IsImmutable));
        isClientVariable = Funcletizer.Nominate(expression, new Func<Expression, bool>(this.IsClosureExpression));
      }
      Funcletizer.FuncletizingVisitor funcletizingVisitor = new Funcletizer.FuncletizingVisitor(this, isClientConstant, isClientVariable);
      Expression expression1 = funcletizingVisitor.Visit(expression);
      recompileRequired = funcletizingVisitor.GetRecompileRequiredFunction();
      return expression1;
    }

    private Expression ReplaceRootContextParameter(Expression expression)
    {
      if (this._rootContextExpression != null)
        return EntityExpressionVisitor.Visit(expression, (Func<Expression, Func<Expression, Expression>, Expression>) ((exp, baseVisit) =>
        {
          if (exp != this._rootContextParameter)
            return baseVisit(exp);
          return (Expression) this._rootContextExpression;
        }));
      return expression;
    }

    private static Func<Expression, bool> Nominate(
      Expression expression,
      Func<Expression, bool> localCriterion)
    {
      HashSet<Expression> candidates = new HashSet<Expression>();
      bool cannotBeNominated = false;
      Func<Expression, Func<Expression, Expression>, Expression> visit = (Func<Expression, Func<Expression, Expression>, Expression>) ((exp, baseVisit) =>
      {
        if (exp != null)
        {
          bool flag = cannotBeNominated;
          cannotBeNominated = false;
          Expression expression1 = baseVisit(exp);
          if (!cannotBeNominated)
          {
            if (localCriterion(exp))
              candidates.Add(exp);
            else
              cannotBeNominated = true;
          }
          cannotBeNominated |= flag;
        }
        return exp;
      });
      EntityExpressionVisitor.Visit(expression, visit);
      return new Func<Expression, bool>(candidates.Contains);
    }

    private bool IsImmutable(Expression expression)
    {
      if (expression == null)
        return false;
      switch (expression.NodeType)
      {
        case ExpressionType.Constant:
          return true;
        case ExpressionType.Convert:
          return true;
        case ExpressionType.New:
          PrimitiveType primitiveType;
          return ClrProviderManifest.Instance.TryGetPrimitiveType(TypeSystem.GetNonNullableType(expression.Type), out primitiveType);
        case ExpressionType.NewArrayInit:
          return typeof (byte[]) == expression.Type;
        default:
          return false;
      }
    }

    private bool IsClosureExpression(Expression expression)
    {
      if (expression == null)
        return false;
      if (this.IsImmutable(expression))
        return true;
      if (ExpressionType.MemberAccess != expression.NodeType)
        return false;
      MemberExpression memberExpression = (MemberExpression) expression;
      if (memberExpression.Member.MemberType == MemberTypes.Property)
        return ExpressionConverter.CanFuncletizePropertyInfo((PropertyInfo) memberExpression.Member);
      return true;
    }

    private bool IsCompiledQueryParameterVariable(Expression expression)
    {
      if (expression == null)
        return false;
      if (this.IsClosureExpression(expression))
        return true;
      if (ExpressionType.Parameter == expression.NodeType)
        return this._compiledQueryParameters.Contains((ParameterExpression) expression);
      return false;
    }

    private bool TryGetTypeUsageForTerminal(Expression expression, out TypeUsage typeUsage)
    {
      Type type = expression.Type;
      if (this._rootContext.Perspective.TryGetTypeByName(TypeSystem.GetNonNullableType(type).FullNameWithNesting(), false, out typeUsage) && TypeSemantics.IsScalarType(typeUsage))
      {
        if (expression.NodeType == ExpressionType.Convert)
          type = ((UnaryExpression) expression).Operand.Type;
        if (type.IsValueType && Nullable.GetUnderlyingType(type) == (Type) null && TypeSemantics.IsNullable(typeUsage))
          typeUsage = typeUsage.ShallowCopy(new FacetValues()
          {
            Nullable = (FacetValueContainer<bool?>) new bool?(false)
          });
        return true;
      }
      typeUsage = (TypeUsage) null;
      return false;
    }

    internal string GenerateParameterName()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) "p__linq__", (object) this._parameterNumber++);
    }

    private enum Mode
    {
      CompiledQueryLockdown,
      CompiledQueryEvaluation,
      ConventionalQuery,
    }

    private sealed class FuncletizingVisitor : EntityExpressionVisitor
    {
      private readonly List<Func<bool>> _recompileRequiredDelegates = new List<Func<bool>>();
      private readonly Funcletizer _funcletizer;
      private readonly Func<Expression, bool> _isClientConstant;
      private readonly Func<Expression, bool> _isClientVariable;

      internal FuncletizingVisitor(
        Funcletizer funcletizer,
        Func<Expression, bool> isClientConstant,
        Func<Expression, bool> isClientVariable)
      {
        this._funcletizer = funcletizer;
        this._isClientConstant = isClientConstant;
        this._isClientVariable = isClientVariable;
      }

      internal Func<bool> GetRecompileRequiredFunction()
      {
        ReadOnlyCollection<Func<bool>> recompileRequiredDelegates = new ReadOnlyCollection<Func<bool>>((IList<Func<bool>>) this._recompileRequiredDelegates);
        return (Func<bool>) (() => recompileRequiredDelegates.Any<Func<bool>>((Func<Func<bool>, bool>) (d => d())));
      }

      internal override Expression Visit(Expression exp)
      {
        if (exp == null)
          return base.Visit(exp);
        if (!this._funcletizer._linqExpressionStack.Add(exp))
          throw new InvalidOperationException(Strings.ELinq_CycleDetected);
        try
        {
          if (this._isClientConstant(exp))
            return this.InlineValue(exp, false);
          if (!this._isClientVariable(exp))
            return base.Visit(exp);
          TypeUsage typeUsage;
          if (this._funcletizer.TryGetTypeUsageForTerminal(exp, out typeUsage))
            return (Expression) new QueryParameterExpression(typeUsage.Parameter(this._funcletizer.GenerateParameterName()), exp, (IEnumerable<ParameterExpression>) this._funcletizer._compiledQueryParameters);
          if (this._funcletizer.IsCompiledQuery)
            throw Funcletizer.FuncletizingVisitor.InvalidCompiledQueryParameterException(exp);
          return this.InlineValue(exp, true);
        }
        finally
        {
          this._funcletizer._linqExpressionStack.Remove(exp);
        }
      }

      private static NotSupportedException InvalidCompiledQueryParameterException(
        Expression expression)
      {
        ParameterExpression parameterExpression;
        if (expression.NodeType == ExpressionType.Parameter)
        {
          parameterExpression = (ParameterExpression) expression;
        }
        else
        {
          HashSet<ParameterExpression> parameters = new HashSet<ParameterExpression>();
          EntityExpressionVisitor.Visit(expression, (Func<Expression, Func<Expression, Expression>, Expression>) ((exp, baseVisit) =>
          {
            if (exp != null && exp.NodeType == ExpressionType.Parameter)
              parameters.Add((ParameterExpression) exp);
            return baseVisit(exp);
          }));
          if (parameters.Count != 1)
            return new NotSupportedException(Strings.CompiledELinq_UnsupportedParameterTypes((object) expression.Type.FullName));
          parameterExpression = parameters.Single<ParameterExpression>();
        }
        if (parameterExpression.Type.Equals(expression.Type))
          return new NotSupportedException(Strings.CompiledELinq_UnsupportedNamedParameterType((object) parameterExpression.Name, (object) parameterExpression.Type.FullName));
        return new NotSupportedException(Strings.CompiledELinq_UnsupportedNamedParameterUseAsType((object) parameterExpression.Name, (object) expression.Type.FullName));
      }

      private static Func<object> CompileExpression(Expression expression)
      {
        return Expression.Lambda<Func<object>>(TypeSystem.EnsureType(expression, typeof (object)), new ParameterExpression[0]).Compile();
      }

      private Expression InlineValue(Expression expression, bool recompileOnChange)
      {
        Func<object> getValue = (Func<object>) null;
        object obj = (object) null;
        if (expression.NodeType == ExpressionType.Constant)
        {
          obj = ((ConstantExpression) expression).Value;
        }
        else
        {
          bool flag = false;
          if (expression.NodeType == ExpressionType.Convert)
          {
            UnaryExpression unaryExpression = (UnaryExpression) expression;
            if (!recompileOnChange && unaryExpression.Operand.NodeType == ExpressionType.Constant && typeof (IQueryable).IsAssignableFrom(unaryExpression.Operand.Type))
            {
              obj = ((ConstantExpression) unaryExpression.Operand).Value;
              flag = true;
            }
          }
          if (!flag)
          {
            getValue = Funcletizer.FuncletizingVisitor.CompileExpression(expression);
            obj = getValue();
          }
        }
        ObjectQuery objectQuery = (obj as IQueryable).TryGetObjectQuery();
        Expression expression1;
        if (objectQuery != null)
        {
          expression1 = this.InlineObjectQuery(objectQuery, objectQuery.GetType());
        }
        else
        {
          LambdaExpression lambdaExpression = obj as LambdaExpression;
          expression1 = lambdaExpression == null ? (expression.NodeType == ExpressionType.Constant ? expression : (Expression) Expression.Constant(obj, expression.Type)) : this.InlineExpression((Expression) Expression.Quote((Expression) lambdaExpression));
        }
        if (recompileOnChange)
          this.AddRecompileRequiredDelegates(getValue, obj);
        return expression1;
      }

      private void AddRecompileRequiredDelegates(Func<object> getValue, object value)
      {
        ObjectQuery originalQuery = (value as IQueryable).TryGetObjectQuery();
        if (originalQuery != null)
        {
          MergeOption? originalMergeOption = originalQuery.QueryState.UserSpecifiedMergeOption;
          if (getValue == null)
            this._recompileRequiredDelegates.Add((Func<bool>) (() =>
            {
              MergeOption? specifiedMergeOption = originalQuery.QueryState.UserSpecifiedMergeOption;
              MergeOption? nullable = originalMergeOption;
              if (specifiedMergeOption.GetValueOrDefault() == nullable.GetValueOrDefault())
                return specifiedMergeOption.HasValue != nullable.HasValue;
              return true;
            }));
          else
            this._recompileRequiredDelegates.Add((Func<bool>) (() =>
            {
              ObjectQuery objectQuery = (getValue() as IQueryable).TryGetObjectQuery();
              if (!object.ReferenceEquals((object) originalQuery, (object) objectQuery))
                return true;
              MergeOption? specifiedMergeOption = objectQuery.QueryState.UserSpecifiedMergeOption;
              MergeOption? nullable = originalMergeOption;
              if (specifiedMergeOption.GetValueOrDefault() == nullable.GetValueOrDefault())
                return specifiedMergeOption.HasValue != nullable.HasValue;
              return true;
            }));
        }
        else
        {
          if (getValue == null)
            return;
          this._recompileRequiredDelegates.Add((Func<bool>) (() => !object.ReferenceEquals(value, getValue())));
        }
      }

      private Expression InlineObjectQuery(ObjectQuery inlineQuery, Type expressionType)
      {
        Expression expression1;
        if (this._funcletizer._mode == Funcletizer.Mode.CompiledQueryLockdown)
        {
          expression1 = (Expression) Expression.Constant((object) inlineQuery, expressionType);
        }
        else
        {
          if (!object.ReferenceEquals((object) this._funcletizer._rootContext, (object) inlineQuery.QueryState.ObjectContext))
            throw new NotSupportedException(Strings.ELinq_UnsupportedDifferentContexts);
          Expression expression2 = inlineQuery.GetExpression();
          if (!(inlineQuery.QueryState is EntitySqlQueryState))
            expression2 = this.InlineExpression(expression2);
          expression1 = TypeSystem.EnsureType(expression2, expressionType);
        }
        return expression1;
      }

      private Expression InlineExpression(Expression exp)
      {
        Func<bool> recompileRequired;
        exp = this._funcletizer.Funcletize(exp, out recompileRequired);
        if (!this._funcletizer.IsCompiledQuery)
          this._recompileRequiredDelegates.Add(recompileRequired);
        return exp;
      }
    }
  }
}
