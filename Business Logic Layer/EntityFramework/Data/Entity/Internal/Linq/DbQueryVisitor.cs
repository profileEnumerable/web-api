// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Linq.DbQueryVisitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Internal.Linq
{
  internal class DbQueryVisitor : ExpressionVisitor
  {
    private static readonly ConcurrentDictionary<Type, Func<ObjectQuery, object>> _wrapperFactories = new ConcurrentDictionary<Type, Func<ObjectQuery, object>>();
    private const BindingFlags SetAccessBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
      Check.NotNull<MethodCallExpression>(node, nameof (node));
      if (typeof (DbContext).IsAssignableFrom(node.Method.DeclaringType))
      {
        MemberExpression memberExpression = node.Object as MemberExpression;
        if (memberExpression != null)
        {
          DbContext constantExpression = DbQueryVisitor.GetContextFromConstantExpression(memberExpression.Expression, memberExpression.Member);
          if (constantExpression != null && !node.Method.GetCustomAttributes<DbFunctionAttribute>(false).Any<DbFunctionAttribute>() && node.Method.GetParameters().Length == 0)
          {
            Expression objectQueryConstant = DbQueryVisitor.CreateObjectQueryConstant(node.Method.Invoke((object) constantExpression, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, (object[]) null, (CultureInfo) null));
            if (objectQueryConstant != null)
              return objectQueryConstant;
          }
        }
      }
      return base.VisitMethodCall(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
      Check.NotNull<MemberExpression>(node, nameof (node));
      PropertyInfo member = node.Member as PropertyInfo;
      MemberExpression expression = node.Expression as MemberExpression;
      if (member != (PropertyInfo) null && expression != null && (typeof (IQueryable).IsAssignableFrom(member.PropertyType) && typeof (DbContext).IsAssignableFrom(node.Member.DeclaringType)))
      {
        DbContext constantExpression = DbQueryVisitor.GetContextFromConstantExpression(expression.Expression, expression.Member);
        if (constantExpression != null)
        {
          Expression objectQueryConstant = DbQueryVisitor.CreateObjectQueryConstant(member.GetValue((object) constantExpression, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, (object[]) null, (CultureInfo) null));
          if (objectQueryConstant != null)
            return objectQueryConstant;
        }
      }
      return base.VisitMember(node);
    }

    private static DbContext GetContextFromConstantExpression(
      Expression expression,
      MemberInfo member)
    {
      if (expression == null)
        return DbQueryVisitor.GetContextFromMember(member, (object) null);
      ConstantExpression constantExpression = expression as ConstantExpression;
      if (constantExpression != null)
      {
        object obj = constantExpression.Value;
        if (obj != null)
          return DbQueryVisitor.GetContextFromMember(member, obj);
      }
      return (DbContext) null;
    }

    private static DbContext GetContextFromMember(MemberInfo member, object value)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo != (FieldInfo) null)
        return fieldInfo.GetValue(value) as DbContext;
      PropertyInfo propertyInfo = member as PropertyInfo;
      if (propertyInfo != (PropertyInfo) null)
        return propertyInfo.GetValue(value, (object[]) null) as DbContext;
      return (DbContext) null;
    }

    private static Expression CreateObjectQueryConstant(object dbQuery)
    {
      ObjectQuery objectQuery = DbQueryVisitor.ExtractObjectQuery(dbQuery);
      if (objectQuery == null)
        return (Expression) null;
      Type key = ((IEnumerable<Type>) objectQuery.GetType().GetGenericArguments()).Single<Type>();
      Func<ObjectQuery, object> func;
      if (!DbQueryVisitor._wrapperFactories.TryGetValue(key, out func))
      {
        func = (Func<ObjectQuery, object>) Delegate.CreateDelegate(typeof (Func<ObjectQuery, object>), typeof (ReplacementDbQueryWrapper<>).MakeGenericType(key).GetDeclaredMethod("Create", typeof (ObjectQuery)));
        DbQueryVisitor._wrapperFactories.TryAdd(key, func);
      }
      object obj = func(objectQuery);
      return (Expression) Expression.Property((Expression) Expression.Constant(obj, obj.GetType()), "Query");
    }

    private static ObjectQuery ExtractObjectQuery(object dbQuery)
    {
      return (dbQuery as IInternalQueryAdapter)?.InternalQuery.ObjectQuery;
    }
  }
}
