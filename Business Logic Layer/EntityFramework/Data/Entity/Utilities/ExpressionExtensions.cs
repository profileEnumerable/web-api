// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.ExpressionExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Resources;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Utilities
{
  internal static class ExpressionExtensions
  {
    public static PropertyPath GetSimplePropertyAccess(
      this LambdaExpression propertyAccessExpression)
    {
      PropertyPath propertyPath = propertyAccessExpression.Parameters.Single<ParameterExpression>().MatchSimplePropertyAccess(propertyAccessExpression.Body);
      if (propertyPath == (PropertyPath) null)
        throw Error.InvalidPropertyExpression((object) propertyAccessExpression);
      return propertyPath;
    }

    public static PropertyPath GetComplexPropertyAccess(
      this LambdaExpression propertyAccessExpression)
    {
      PropertyPath propertyPath = propertyAccessExpression.Parameters.Single<ParameterExpression>().MatchComplexPropertyAccess(propertyAccessExpression.Body);
      if (propertyPath == (PropertyPath) null)
        throw Error.InvalidComplexPropertyExpression((object) propertyAccessExpression);
      return propertyPath;
    }

    public static IEnumerable<PropertyPath> GetSimplePropertyAccessList(
      this LambdaExpression propertyAccessExpression)
    {
      IEnumerable<PropertyPath> propertyPaths = propertyAccessExpression.MatchPropertyAccessList((Func<Expression, Expression, PropertyPath>) ((p, e) => e.MatchSimplePropertyAccess(p)));
      if (propertyPaths == null)
        throw Error.InvalidPropertiesExpression((object) propertyAccessExpression);
      return propertyPaths;
    }

    public static IEnumerable<PropertyPath> GetComplexPropertyAccessList(
      this LambdaExpression propertyAccessExpression)
    {
      IEnumerable<PropertyPath> propertyPaths = propertyAccessExpression.MatchPropertyAccessList((Func<Expression, Expression, PropertyPath>) ((p, e) => e.MatchComplexPropertyAccess(p)));
      if (propertyPaths == null)
        throw Error.InvalidComplexPropertiesExpression((object) propertyAccessExpression);
      return propertyPaths;
    }

    private static IEnumerable<PropertyPath> MatchPropertyAccessList(
      this LambdaExpression lambdaExpression,
      Func<Expression, Expression, PropertyPath> propertyMatcher)
    {
      NewExpression newExpression = lambdaExpression.Body.RemoveConvert() as NewExpression;
      if (newExpression != null)
      {
        ParameterExpression parameterExpression = lambdaExpression.Parameters.Single<ParameterExpression>();
        IEnumerable<PropertyPath> propertyPaths = newExpression.Arguments.Select<Expression, PropertyPath>((Func<Expression, PropertyPath>) (a => propertyMatcher(a, (Expression) parameterExpression))).Where<PropertyPath>((Func<PropertyPath, bool>) (p => p != (PropertyPath) null));
        if (propertyPaths.Count<PropertyPath>() == newExpression.Arguments.Count<Expression>())
        {
          if (!newExpression.HasDefaultMembersOnly(propertyPaths))
            return (IEnumerable<PropertyPath>) null;
          return propertyPaths;
        }
      }
      PropertyPath propertyPath = propertyMatcher(lambdaExpression.Body, (Expression) lambdaExpression.Parameters.Single<ParameterExpression>());
      if (!(propertyPath != (PropertyPath) null))
        return (IEnumerable<PropertyPath>) null;
      return (IEnumerable<PropertyPath>) new PropertyPath[1]
      {
        propertyPath
      };
    }

    private static bool HasDefaultMembersOnly(
      this NewExpression newExpression,
      IEnumerable<PropertyPath> propertyPaths)
    {
      return !newExpression.Members.Where<MemberInfo>((Func<MemberInfo, int, bool>) ((t, i) => !string.Equals(t.Name, propertyPaths.ElementAt<PropertyPath>(i).Last<PropertyInfo>().Name, StringComparison.Ordinal))).Any<MemberInfo>();
    }

    private static PropertyPath MatchSimplePropertyAccess(
      this Expression parameterExpression,
      Expression propertyAccessExpression)
    {
      PropertyPath propertyPath = parameterExpression.MatchPropertyAccess(propertyAccessExpression);
      if (!(propertyPath != (PropertyPath) null) || propertyPath.Count != 1)
        return (PropertyPath) null;
      return propertyPath;
    }

    private static PropertyPath MatchComplexPropertyAccess(
      this Expression parameterExpression,
      Expression propertyAccessExpression)
    {
      return parameterExpression.MatchPropertyAccess(propertyAccessExpression);
    }

    private static PropertyPath MatchPropertyAccess(
      this Expression parameterExpression,
      Expression propertyAccessExpression)
    {
      List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
      MemberExpression memberExpression;
      do
      {
        memberExpression = propertyAccessExpression.RemoveConvert() as MemberExpression;
        if (memberExpression == null)
          return (PropertyPath) null;
        PropertyInfo member = memberExpression.Member as PropertyInfo;
        if (member == (PropertyInfo) null)
          return (PropertyPath) null;
        propertyInfoList.Insert(0, member);
        propertyAccessExpression = memberExpression.Expression;
      }
      while (memberExpression.Expression != parameterExpression);
      return new PropertyPath((IEnumerable<PropertyInfo>) propertyInfoList);
    }

    public static Expression RemoveConvert(this Expression expression)
    {
      while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
        expression = ((UnaryExpression) expression).Operand;
      return expression;
    }

    public static bool IsNullConstant(this Expression expression)
    {
      expression = expression.RemoveConvert();
      if (expression.NodeType != ExpressionType.Constant)
        return false;
      return ((ConstantExpression) expression).Value == null;
    }

    public static bool IsStringAddExpression(this Expression expression)
    {
      BinaryExpression binaryExpression = expression as BinaryExpression;
      if (binaryExpression == null || binaryExpression.Method == (MethodInfo) null || (binaryExpression.NodeType != ExpressionType.Add || !(binaryExpression.Method.DeclaringType == typeof (string))))
        return false;
      return string.Equals(binaryExpression.Method.Name, "Concat", StringComparison.Ordinal);
    }
  }
}
