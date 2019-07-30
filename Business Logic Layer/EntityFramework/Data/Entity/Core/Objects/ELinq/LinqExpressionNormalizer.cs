// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ELinq.LinqExpressionNormalizer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Objects.ELinq
{
  internal class LinqExpressionNormalizer : EntityExpressionVisitor
  {
    internal static readonly MethodInfo RelationalOperatorPlaceholderMethod = typeof (LinqExpressionNormalizer).GetOnlyDeclaredMethod("RelationalOperatorPlaceholder");
    private readonly Dictionary<Expression, LinqExpressionNormalizer.Pattern> _patterns = new Dictionary<Expression, LinqExpressionNormalizer.Pattern>();
    private const bool LiftToNull = false;

    internal override Expression VisitBinary(BinaryExpression b)
    {
      b = (BinaryExpression) base.VisitBinary(b);
      if (b.NodeType == ExpressionType.Equal)
      {
        Expression left = LinqExpressionNormalizer.UnwrapObjectConvert(b.Left);
        Expression right = LinqExpressionNormalizer.UnwrapObjectConvert(b.Right);
        if (left != b.Left || right != b.Right)
          b = LinqExpressionNormalizer.CreateRelationalOperator(ExpressionType.Equal, left, right);
      }
      LinqExpressionNormalizer.Pattern pattern;
      if (this._patterns.TryGetValue(b.Left, out pattern) && pattern.Kind == LinqExpressionNormalizer.PatternKind.Compare && LinqExpressionNormalizer.IsConstantZero(b.Right))
      {
        LinqExpressionNormalizer.ComparePattern comparePattern = (LinqExpressionNormalizer.ComparePattern) pattern;
        BinaryExpression result;
        if (LinqExpressionNormalizer.TryCreateRelationalOperator(b.NodeType, comparePattern.Left, comparePattern.Right, out result))
          b = result;
      }
      return (Expression) b;
    }

    private static Expression UnwrapObjectConvert(Expression input)
    {
      if (input.NodeType == ExpressionType.Constant && input.Type == typeof (object))
      {
        ConstantExpression constantExpression = (ConstantExpression) input;
        if (constantExpression.Value != null && constantExpression.Value.GetType() != typeof (object))
          return (Expression) Expression.Constant(constantExpression.Value, constantExpression.Value.GetType());
      }
      while (ExpressionType.Convert == input.NodeType && typeof (object) == input.Type)
        input = ((UnaryExpression) input).Operand;
      return input;
    }

    private static bool IsConstantZero(Expression expression)
    {
      if (expression.NodeType == ExpressionType.Constant)
        return ((ConstantExpression) expression).Value.Equals((object) 0);
      return false;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal override Expression VisitMethodCall(MethodCallExpression m)
    {
      m = (MethodCallExpression) base.VisitMethodCall(m);
      if (m.Method.IsStatic)
      {
        if (m.Method.Name.StartsWith("op_", StringComparison.Ordinal))
        {
          if (m.Arguments.Count == 2)
          {
            switch (m.Method.Name)
            {
              case "op_Equality":
                return (Expression) Expression.Equal(m.Arguments[0], m.Arguments[1], false, m.Method);
              case "op_Inequality":
                return (Expression) Expression.NotEqual(m.Arguments[0], m.Arguments[1], false, m.Method);
              case "op_GreaterThan":
                return (Expression) Expression.GreaterThan(m.Arguments[0], m.Arguments[1], false, m.Method);
              case "op_GreaterThanOrEqual":
                return (Expression) Expression.GreaterThanOrEqual(m.Arguments[0], m.Arguments[1], false, m.Method);
              case "op_LessThan":
                return (Expression) Expression.LessThan(m.Arguments[0], m.Arguments[1], false, m.Method);
              case "op_LessThanOrEqual":
                return (Expression) Expression.LessThanOrEqual(m.Arguments[0], m.Arguments[1], false, m.Method);
              case "op_Multiply":
                return (Expression) Expression.Multiply(m.Arguments[0], m.Arguments[1], m.Method);
              case "op_Subtraction":
                return (Expression) Expression.Subtract(m.Arguments[0], m.Arguments[1], m.Method);
              case "op_Addition":
                return (Expression) Expression.Add(m.Arguments[0], m.Arguments[1], m.Method);
              case "op_Division":
                return (Expression) Expression.Divide(m.Arguments[0], m.Arguments[1], m.Method);
              case "op_Modulus":
                return (Expression) Expression.Modulo(m.Arguments[0], m.Arguments[1], m.Method);
              case "op_BitwiseAnd":
                return (Expression) Expression.And(m.Arguments[0], m.Arguments[1], m.Method);
              case "op_BitwiseOr":
                return (Expression) Expression.Or(m.Arguments[0], m.Arguments[1], m.Method);
              case "op_ExclusiveOr":
                return (Expression) Expression.ExclusiveOr(m.Arguments[0], m.Arguments[1], m.Method);
            }
          }
          if (m.Arguments.Count == 1)
          {
            switch (m.Method.Name)
            {
              case "op_UnaryNegation":
                return (Expression) Expression.Negate(m.Arguments[0], m.Method);
              case "op_UnaryPlus":
                return (Expression) Expression.UnaryPlus(m.Arguments[0], m.Method);
              case "op_Explicit":
              case "op_Implicit":
                return (Expression) Expression.Convert(m.Arguments[0], m.Type, m.Method);
              case "op_OnesComplement":
              case "op_False":
                return (Expression) Expression.Not(m.Arguments[0], m.Method);
            }
          }
        }
        if (m.Method.Name == "Equals" && m.Arguments.Count > 1)
          return (Expression) Expression.Equal(m.Arguments[0], m.Arguments[1], false, m.Method);
        if (m.Method.Name == "CompareString" && m.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators" || m.Method.Name == "Compare" && m.Arguments.Count > 1 && m.Method.ReturnType == typeof (int))
          return this.CreateCompareExpression(m.Arguments[0], m.Arguments[1]);
      }
      else
      {
        if (m.Method.Name == "Equals" && m.Arguments.Count > 0)
        {
          Type parameterType = m.Method.GetParameters()[0].ParameterType;
          if (parameterType != typeof (DbGeography) && parameterType != typeof (DbGeometry))
            return (Expression) LinqExpressionNormalizer.CreateRelationalOperator(ExpressionType.Equal, m.Object, m.Arguments[0]);
        }
        if (m.Method.Name == "CompareTo" && m.Arguments.Count == 1 && m.Method.ReturnType == typeof (int))
          return this.CreateCompareExpression(m.Object, m.Arguments[0]);
        if (m.Method.Name == "Contains" && m.Arguments.Count == 1)
        {
          Type declaringType = m.Method.DeclaringType;
          MethodInfo method;
          if (declaringType.IsGenericType() && declaringType.GetGenericTypeDefinition() == typeof (List<>) && ReflectionUtil.TryLookupMethod(SequenceMethod.Contains, out method))
            return (Expression) Expression.Call(method.MakeGenericMethod(declaringType.GetGenericArguments()), m.Object, m.Arguments[0]);
        }
      }
      return (Expression) LinqExpressionNormalizer.NormalizePredicateArgument(m);
    }

    private static MethodCallExpression NormalizePredicateArgument(
      MethodCallExpression callExpression)
    {
      int argumentOrdinal;
      Expression normalized;
      MethodCallExpression methodCallExpression;
      if (LinqExpressionNormalizer.HasPredicateArgument(callExpression, out argumentOrdinal) && LinqExpressionNormalizer.TryMatchCoalescePattern(callExpression.Arguments[argumentOrdinal], out normalized))
        methodCallExpression = Expression.Call(callExpression.Object, callExpression.Method, (IEnumerable<Expression>) new List<Expression>((IEnumerable<Expression>) callExpression.Arguments)
        {
          [argumentOrdinal] = normalized
        });
      else
        methodCallExpression = callExpression;
      return methodCallExpression;
    }

    private static bool HasPredicateArgument(
      MethodCallExpression callExpression,
      out int argumentOrdinal)
    {
      argumentOrdinal = 0;
      bool flag = false;
      SequenceMethod sequenceMethod;
      if (2 <= callExpression.Arguments.Count && ReflectionUtil.TryIdentifySequenceMethod(callExpression.Method, out sequenceMethod))
      {
        switch (sequenceMethod)
        {
          case SequenceMethod.Where:
          case SequenceMethod.WhereOrdinal:
          case SequenceMethod.TakeWhile:
          case SequenceMethod.TakeWhileOrdinal:
          case SequenceMethod.SkipWhile:
          case SequenceMethod.SkipWhileOrdinal:
          case SequenceMethod.FirstPredicate:
          case SequenceMethod.FirstOrDefaultPredicate:
          case SequenceMethod.LastPredicate:
          case SequenceMethod.LastOrDefaultPredicate:
          case SequenceMethod.SinglePredicate:
          case SequenceMethod.SingleOrDefaultPredicate:
          case SequenceMethod.AnyPredicate:
          case SequenceMethod.All:
          case SequenceMethod.CountPredicate:
          case SequenceMethod.LongCountPredicate:
            argumentOrdinal = 1;
            flag = true;
            break;
        }
      }
      return flag;
    }

    private static bool TryMatchCoalescePattern(Expression expression, out Expression normalized)
    {
      normalized = (Expression) null;
      bool flag = false;
      if (expression.NodeType == ExpressionType.Quote)
      {
        if (LinqExpressionNormalizer.TryMatchCoalescePattern(((UnaryExpression) expression).Operand, out normalized))
        {
          flag = true;
          normalized = (Expression) Expression.Quote(normalized);
        }
      }
      else if (expression.NodeType == ExpressionType.Lambda)
      {
        LambdaExpression lambdaExpression = (LambdaExpression) expression;
        if (lambdaExpression.Body.NodeType == ExpressionType.Coalesce && lambdaExpression.Body.Type == typeof (bool))
        {
          BinaryExpression body = (BinaryExpression) lambdaExpression.Body;
          if (body.Right.NodeType == ExpressionType.Constant && false.Equals(((ConstantExpression) body.Right).Value))
          {
            normalized = (Expression) Expression.Lambda(lambdaExpression.Type, (Expression) Expression.Convert(body.Left, typeof (bool)), (IEnumerable<ParameterExpression>) lambdaExpression.Parameters);
            flag = true;
          }
        }
      }
      return flag;
    }

    private static bool RelationalOperatorPlaceholder<TLeft, TRight>(TLeft left, TRight right)
    {
      return object.ReferenceEquals((object) left, (object) right);
    }

    private static BinaryExpression CreateRelationalOperator(
      ExpressionType op,
      Expression left,
      Expression right)
    {
      BinaryExpression result;
      LinqExpressionNormalizer.TryCreateRelationalOperator(op, left, right, out result);
      return result;
    }

    private static bool TryCreateRelationalOperator(
      ExpressionType op,
      Expression left,
      Expression right,
      out BinaryExpression result)
    {
      MethodInfo method = LinqExpressionNormalizer.RelationalOperatorPlaceholderMethod.MakeGenericMethod(left.Type, right.Type);
      switch (op)
      {
        case ExpressionType.Equal:
          result = Expression.Equal(left, right, false, method);
          return true;
        case ExpressionType.GreaterThan:
          result = Expression.GreaterThan(left, right, false, method);
          return true;
        case ExpressionType.GreaterThanOrEqual:
          result = Expression.GreaterThanOrEqual(left, right, false, method);
          return true;
        case ExpressionType.LessThan:
          result = Expression.LessThan(left, right, false, method);
          return true;
        case ExpressionType.LessThanOrEqual:
          result = Expression.LessThanOrEqual(left, right, false, method);
          return true;
        case ExpressionType.NotEqual:
          result = Expression.NotEqual(left, right, false, method);
          return true;
        default:
          result = (BinaryExpression) null;
          return false;
      }
    }

    private Expression CreateCompareExpression(Expression left, Expression right)
    {
      Expression index = (Expression) Expression.Condition((Expression) LinqExpressionNormalizer.CreateRelationalOperator(ExpressionType.Equal, left, right), (Expression) Expression.Constant((object) 0), (Expression) Expression.Condition((Expression) LinqExpressionNormalizer.CreateRelationalOperator(ExpressionType.GreaterThan, left, right), (Expression) Expression.Constant((object) 1), (Expression) Expression.Constant((object) -1)));
      this._patterns[index] = (LinqExpressionNormalizer.Pattern) new LinqExpressionNormalizer.ComparePattern(left, right);
      return index;
    }

    private abstract class Pattern
    {
      internal abstract LinqExpressionNormalizer.PatternKind Kind { get; }
    }

    private enum PatternKind
    {
      Compare,
    }

    private sealed class ComparePattern : LinqExpressionNormalizer.Pattern
    {
      internal readonly Expression Left;
      internal readonly Expression Right;

      internal ComparePattern(Expression left, Expression right)
      {
        this.Left = left;
        this.Right = right;
      }

      internal override LinqExpressionNormalizer.PatternKind Kind
      {
        get
        {
          return LinqExpressionNormalizer.PatternKind.Compare;
        }
      }
    }
  }
}
