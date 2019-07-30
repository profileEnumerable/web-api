// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ELinq.QueryParameterExpression
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Objects.ELinq
{
  internal sealed class QueryParameterExpression : Expression
  {
    private readonly DbParameterReferenceExpression _parameterReference;
    private readonly Type _type;
    private readonly Expression _funcletizedExpression;
    private readonly IEnumerable<ParameterExpression> _compiledQueryParameters;
    private Delegate _cachedDelegate;

    internal QueryParameterExpression(
      DbParameterReferenceExpression parameterReference,
      Expression funcletizedExpression,
      IEnumerable<ParameterExpression> compiledQueryParameters)
    {
      this._compiledQueryParameters = compiledQueryParameters ?? Enumerable.Empty<ParameterExpression>();
      this._parameterReference = parameterReference;
      this._type = funcletizedExpression.Type;
      this._funcletizedExpression = funcletizedExpression;
      this._cachedDelegate = (Delegate) null;
    }

    internal object EvaluateParameter(object[] arguments)
    {
      if ((object) this._cachedDelegate == null)
      {
        if (this._funcletizedExpression.NodeType == ExpressionType.Constant)
          return ((ConstantExpression) this._funcletizedExpression).Value;
        ConstantExpression constantExpression;
        if (QueryParameterExpression.TryEvaluatePath(this._funcletizedExpression, out constantExpression))
          return constantExpression.Value;
      }
      try
      {
        if ((object) this._cachedDelegate == null)
          this._cachedDelegate = Expression.Lambda(TypeSystem.GetDelegateType(this._compiledQueryParameters.Select<ParameterExpression, Type>((Func<ParameterExpression, Type>) (p => p.Type)), this._type), this._funcletizedExpression, this._compiledQueryParameters).Compile();
        return this._cachedDelegate.DynamicInvoke(arguments);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    internal QueryParameterExpression EscapeParameterForLike(
      Func<string, string> method)
    {
      return new QueryParameterExpression(this._parameterReference, (Expression) Expression.Invoke((Expression) Expression.Constant((object) method), this._funcletizedExpression), this._compiledQueryParameters);
    }

    internal DbParameterReferenceExpression ParameterReference
    {
      get
      {
        return this._parameterReference;
      }
    }

    public override Type Type
    {
      get
      {
        return this._type;
      }
    }

    public override ExpressionType NodeType
    {
      get
      {
        return ~ExpressionType.Add;
      }
    }

    private static bool TryEvaluatePath(
      Expression expression,
      out ConstantExpression constantExpression)
    {
      memberExpression = expression as MemberExpression;
      constantExpression = (ConstantExpression) null;
      if (memberExpression != null)
      {
        Stack<MemberExpression> memberExpressionStack = new Stack<MemberExpression>();
        memberExpressionStack.Push(memberExpression);
        while (memberExpression.Expression is MemberExpression memberExpression)
          memberExpressionStack.Push(memberExpression);
        MemberExpression me1 = memberExpressionStack.Pop();
        object memberValue;
        if (me1.Expression is ConstantExpression && QueryParameterExpression.TryGetFieldOrPropertyValue(me1, ((ConstantExpression) me1.Expression).Value, out memberValue))
        {
          if (memberExpressionStack.Count > 0)
          {
            foreach (MemberExpression me2 in memberExpressionStack)
            {
              if (!QueryParameterExpression.TryGetFieldOrPropertyValue(me2, memberValue, out memberValue))
                return false;
            }
          }
          constantExpression = Expression.Constant(memberValue, expression.Type);
          return true;
        }
      }
      return false;
    }

    private static bool TryGetFieldOrPropertyValue(
      MemberExpression me,
      object instance,
      out object memberValue)
    {
      bool flag = false;
      memberValue = (object) null;
      try
      {
        if (me.Member.MemberType == MemberTypes.Field)
        {
          memberValue = ((FieldInfo) me.Member).GetValue(instance);
          flag = true;
        }
        else if (me.Member.MemberType == MemberTypes.Property)
        {
          memberValue = ((PropertyInfo) me.Member).GetValue(instance, (object[]) null);
          flag = true;
        }
        return flag;
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }
  }
}
