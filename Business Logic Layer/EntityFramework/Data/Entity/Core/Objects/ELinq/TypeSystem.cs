// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ELinq.TypeSystem
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Objects.ELinq
{
  internal static class TypeSystem
  {
    internal static readonly MethodInfo GetDefaultMethod = typeof (TypeSystem).GetOnlyDeclaredMethod("GetDefault");

    private static T GetDefault<T>()
    {
      return default (T);
    }

    internal static object GetDefaultValue(Type type)
    {
      if (!type.IsValueType() || type.IsGenericType() && typeof (Nullable<>) == type.GetGenericTypeDefinition())
        return (object) null;
      return TypeSystem.GetDefaultMethod.MakeGenericMethod(type).Invoke((object) null, new object[0]);
    }

    internal static bool IsSequenceType(Type seqType)
    {
      return TypeSystem.FindIEnumerable(seqType) != (Type) null;
    }

    internal static Type GetDelegateType(IEnumerable<Type> inputTypes, Type returnType)
    {
      inputTypes = inputTypes ?? Enumerable.Empty<Type>();
      int num = inputTypes.Count<Type>();
      Type[] typeArray = new Type[num + 1];
      int index = 0;
      foreach (Type inputType in inputTypes)
        typeArray[index++] = inputType;
      typeArray[index] = returnType;
      Type type;
      switch (num)
      {
        case 0:
          type = typeof (Func<>);
          break;
        case 1:
          type = typeof (Func<,>);
          break;
        case 2:
          type = typeof (Func<,,>);
          break;
        case 3:
          type = typeof (Func<,,,>);
          break;
        case 4:
          type = typeof (Func<,,,,>);
          break;
        case 5:
          type = typeof (Func<,,,,,>);
          break;
        case 6:
          type = typeof (Func<,,,,,,>);
          break;
        case 7:
          type = typeof (Func<,,,,,,,>);
          break;
        case 8:
          type = typeof (Func<,,,,,,,,>);
          break;
        case 9:
          type = typeof (Func<,,,,,,,,,>);
          break;
        case 10:
          type = typeof (Func<,,,,,,,,,,>);
          break;
        case 11:
          type = typeof (Func<,,,,,,,,,,,>);
          break;
        case 12:
          type = typeof (Func<,,,,,,,,,,,,>);
          break;
        case 13:
          type = typeof (Func<,,,,,,,,,,,,,>);
          break;
        case 14:
          type = typeof (Func<,,,,,,,,,,,,,,>);
          break;
        case 15:
          type = typeof (Func<,,,,,,,,,,,,,,,>);
          break;
        default:
          type = (Type) null;
          break;
      }
      return type.MakeGenericType(typeArray);
    }

    internal static Expression EnsureType(Expression expression, Type requiredType)
    {
      if (expression.Type != requiredType)
        expression = (Expression) Expression.Convert(expression, requiredType);
      return expression;
    }

    internal static MemberInfo PropertyOrField(
      MemberInfo member,
      out string name,
      out Type type)
    {
      name = (string) null;
      type = (Type) null;
      if (member.MemberType == MemberTypes.Field)
      {
        FieldInfo fieldInfo = (FieldInfo) member;
        name = fieldInfo.Name;
        type = fieldInfo.FieldType;
        return (MemberInfo) fieldInfo;
      }
      if (member.MemberType == MemberTypes.Property)
      {
        PropertyInfo propertyInfo = (PropertyInfo) member;
        if (propertyInfo.GetIndexParameters().Length != 0)
          throw new NotSupportedException(Strings.ELinq_PropertyIndexNotSupported);
        name = propertyInfo.Name;
        type = propertyInfo.PropertyType;
        return (MemberInfo) propertyInfo;
      }
      if (member.MemberType == MemberTypes.Method)
      {
        MethodInfo methodInfo = (MethodInfo) member;
        if (methodInfo.IsSpecialName)
        {
          foreach (PropertyInfo runtimeProperty in methodInfo.DeclaringType.GetRuntimeProperties())
          {
            if (runtimeProperty.CanRead && runtimeProperty.Getter() == methodInfo)
              return TypeSystem.PropertyOrField((MemberInfo) runtimeProperty, out name, out type);
          }
        }
      }
      throw new NotSupportedException(Strings.ELinq_NotPropertyOrField((object) member.Name));
    }

    private static Type FindIEnumerable(Type seqType)
    {
      if (seqType == (Type) null || seqType == typeof (string) || seqType == typeof (byte[]))
        return (Type) null;
      if (seqType.IsArray)
        return typeof (IEnumerable<>).MakeGenericType(seqType.GetElementType());
      if (seqType.IsGenericType())
      {
        foreach (Type genericArgument in seqType.GetGenericArguments())
        {
          Type type = typeof (IEnumerable<>).MakeGenericType(genericArgument);
          if (type.IsAssignableFrom(seqType))
            return type;
        }
      }
      Type[] interfaces = seqType.GetInterfaces();
      if (interfaces != null && interfaces.Length > 0)
      {
        foreach (Type seqType1 in interfaces)
        {
          Type ienumerable = TypeSystem.FindIEnumerable(seqType1);
          if (ienumerable != (Type) null)
            return ienumerable;
        }
      }
      if (seqType.BaseType() != (Type) null && seqType.BaseType() != typeof (object))
        return TypeSystem.FindIEnumerable(seqType.BaseType());
      return (Type) null;
    }

    internal static Type GetElementType(Type seqType)
    {
      Type ienumerable = TypeSystem.FindIEnumerable(seqType);
      if (ienumerable == (Type) null)
        return seqType;
      return ienumerable.GetGenericArguments()[0];
    }

    internal static Type GetNonNullableType(Type type)
    {
      if (!(type != (Type) null))
        return (Type) null;
      Type underlyingType = Nullable.GetUnderlyingType(type);
      if ((object) underlyingType != null)
        return underlyingType;
      return type;
    }

    internal static bool IsImplementationOfGenericInterfaceMethod(
      this MethodInfo test,
      Type match,
      out Type[] genericTypeArguments)
    {
      genericTypeArguments = (Type[]) null;
      if ((MethodInfo) null == test || (Type) null == match || (!match.IsInterface() || !match.IsGenericTypeDefinition()) || (Type) null == test.DeclaringType)
        return false;
      if (test.DeclaringType.IsInterface() && test.DeclaringType.IsGenericType() && test.DeclaringType.GetGenericTypeDefinition() == match)
        return true;
      foreach (Type type in test.DeclaringType.GetInterfaces())
      {
        if (type.IsGenericType() && type.GetGenericTypeDefinition() == match && ((IEnumerable<MethodInfo>) test.DeclaringType.GetInterfaceMap(type).TargetMethods).Contains<MethodInfo>(test))
        {
          genericTypeArguments = type.GetGenericArguments();
          return true;
        }
      }
      return false;
    }

    internal static bool IsImplementationOf(this PropertyInfo propertyInfo, Type interfaceType)
    {
      PropertyInfo declaredProperty = interfaceType.GetDeclaredProperty(propertyInfo.Name);
      if ((PropertyInfo) null == declaredProperty)
        return false;
      if (propertyInfo.DeclaringType.IsInterface())
        return declaredProperty.Equals((object) propertyInfo);
      bool flag = false;
      MethodInfo methodInfo1 = declaredProperty.Getter();
      InterfaceMapping interfaceMap = propertyInfo.DeclaringType.GetInterfaceMap(interfaceType);
      int index = Array.IndexOf<MethodInfo>(interfaceMap.InterfaceMethods, methodInfo1);
      MethodInfo[] targetMethods = interfaceMap.TargetMethods;
      if (index > -1 && index < targetMethods.Length)
      {
        MethodInfo methodInfo2 = propertyInfo.Getter();
        if (methodInfo2 != (MethodInfo) null)
          flag = methodInfo2.Equals((object) targetMethods[index]);
      }
      return flag;
    }
  }
}
