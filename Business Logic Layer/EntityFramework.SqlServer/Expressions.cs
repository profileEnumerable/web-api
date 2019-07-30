// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Expressions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.SqlServer
{
  internal static class Expressions
  {
    internal static Expression Null<TNullType>()
    {
      return (Expression) Expression.Constant((object) null, typeof (TNullType));
    }

    internal static Expression Null(Type nullType)
    {
      return (Expression) Expression.Constant((object) null, nullType);
    }

    internal static Expression<Func<TArg, TResult>> Lambda<TArg, TResult>(
      string argumentName,
      Func<ParameterExpression, Expression> createLambdaBodyGivenParameter)
    {
      ParameterExpression parameterExpression;
      return Expression.Lambda<Func<TArg, TResult>>(createLambdaBodyGivenParameter(parameterExpression), parameterExpression);
    }

    internal static Expression Call(this Expression exp, string methodName)
    {
      return (Expression) Expression.Call(exp, methodName, Type.EmptyTypes);
    }

    internal static Expression ConvertTo(this Expression exp, Type convertToType)
    {
      return (Expression) Expression.Convert(exp, convertToType);
    }

    internal static Expression ConvertTo<TConvertToType>(this Expression exp)
    {
      return (Expression) Expression.Convert(exp, typeof (TConvertToType));
    }

    internal static System.Data.Entity.SqlServer.Expressions.ConditionalExpressionBuilder IfTrueThen(
      this Expression conditionExp,
      Expression resultIfTrue)
    {
      return new System.Data.Entity.SqlServer.Expressions.ConditionalExpressionBuilder(conditionExp, resultIfTrue);
    }

    internal static Expression Property<TPropertyType>(
      this Expression exp,
      string propertyName)
    {
      PropertyInfo runtimeProperty = exp.Type.GetRuntimeProperty(propertyName);
      return (Expression) Expression.Property(exp, runtimeProperty);
    }

    internal sealed class ConditionalExpressionBuilder
    {
      private readonly Expression condition;
      private readonly Expression ifTrueThen;

      internal ConditionalExpressionBuilder(
        Expression conditionExpression,
        Expression ifTrueExpression)
      {
        this.condition = conditionExpression;
        this.ifTrueThen = ifTrueExpression;
      }

      internal Expression Else(Expression resultIfFalse)
      {
        return (Expression) Expression.Condition(this.condition, this.ifTrueThen, resultIfFalse);
      }
    }
  }
}
