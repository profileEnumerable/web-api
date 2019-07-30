// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.TypeExtensions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Spatial;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.SqlServer.Utilities
{
  internal static class TypeExtensions
  {
    private static readonly Dictionary<Type, PrimitiveType> _primitiveTypesMap = new Dictionary<Type, PrimitiveType>();

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static TypeExtensions()
    {
      foreach (PrimitiveType edmPrimitiveType in PrimitiveType.GetEdmPrimitiveTypes())
      {
        if (!TypeExtensions._primitiveTypesMap.ContainsKey(edmPrimitiveType.ClrEquivalentType))
          TypeExtensions._primitiveTypesMap.Add(edmPrimitiveType.ClrEquivalentType, edmPrimitiveType);
      }
    }

    public static bool IsCollection(this Type type)
    {
      return type.IsCollection(out type);
    }

    public static bool IsCollection(this Type type, out Type elementType)
    {
      elementType = type.TryGetElementType(typeof (ICollection<>));
      if (!(elementType == (Type) null) && !type.IsArray)
        return true;
      elementType = type;
      return false;
    }

    public static IEnumerable<PropertyInfo> GetNonIndexerProperties(
      this Type type)
    {
      return type.GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (p.IsPublic())
          return !((IEnumerable<ParameterInfo>) p.GetIndexParameters()).Any<ParameterInfo>();
        return false;
      }));
    }

    public static Type TryGetElementType(this Type type, Type interfaceOrBaseType)
    {
      if (type.IsGenericTypeDefinition())
        return (Type) null;
      List<Type> list = type.GetGenericTypeImplementations(interfaceOrBaseType).ToList<Type>();
      if (list.Count != 1)
        return (Type) null;
      return ((IEnumerable<Type>) list[0].GetGenericArguments()).FirstOrDefault<Type>();
    }

    public static IEnumerable<Type> GetGenericTypeImplementations(
      this Type type,
      Type interfaceOrBaseType)
    {
      if (type.IsGenericTypeDefinition())
        return Enumerable.Empty<Type>();
      return ((IEnumerable<Type>) (interfaceOrBaseType.IsInterface() ? (object) type.GetInterfaces() : (object) type.GetBaseTypes())).Union<Type>((IEnumerable<Type>) new Type[1]
      {
        type
      }).Where<Type>((Func<Type, bool>) (t =>
      {
        if (t.IsGenericType())
          return t.GetGenericTypeDefinition() == interfaceOrBaseType;
        return false;
      }));
    }

    public static IEnumerable<Type> GetBaseTypes(this Type type)
    {
      for (type = type.BaseType(); type != (Type) null; type = type.BaseType())
        yield return type;
    }

    public static Type GetTargetType(this Type type)
    {
      Type elementType;
      if (!type.IsCollection(out elementType))
        elementType = type;
      return elementType;
    }

    public static bool TryUnwrapNullableType(this Type type, out Type underlyingType)
    {
      ref Type local = ref underlyingType;
      Type type1 = Nullable.GetUnderlyingType(type);
      if ((object) type1 == null)
        type1 = type;
      local = type1;
      return underlyingType != type;
    }

    public static bool IsNullable(this Type type)
    {
      if (type.IsValueType())
        return Nullable.GetUnderlyingType(type) != (Type) null;
      return true;
    }

    public static bool IsValidStructuralType(this Type type)
    {
      if (!type.IsGenericType() && !type.IsValueType() && (!type.IsPrimitive() && !type.IsInterface()) && (!type.IsArray && !(type == typeof (string)) && (!(type == typeof (DbGeography)) && !(type == typeof (DbGeometry)))))
        return type.IsValidStructuralPropertyType();
      return false;
    }

    public static bool IsValidStructuralPropertyType(this Type type)
    {
      if (!type.IsGenericTypeDefinition() && !type.IsPointer && (!(type == typeof (object)) && !typeof (ComplexObject).IsAssignableFrom(type)) && (!typeof (EntityObject).IsAssignableFrom(type) && !typeof (StructuralObject).IsAssignableFrom(type) && !typeof (EntityKey).IsAssignableFrom(type)))
        return !typeof (EntityReference).IsAssignableFrom(type);
      return false;
    }

    public static bool IsPrimitiveType(this Type type, out PrimitiveType primitiveType)
    {
      return TypeExtensions._primitiveTypesMap.TryGetValue(type, out primitiveType);
    }

    public static bool IsValidEdmScalarType(this Type type)
    {
      type.TryUnwrapNullableType(out type);
      PrimitiveType primitiveType;
      if (!type.IsPrimitiveType(out primitiveType))
        return type.IsEnum();
      return true;
    }

    public static string NestingNamespace(this Type type)
    {
      if (!type.IsNested)
        return type.Namespace;
      string fullName = type.FullName;
      return fullName.Substring(0, fullName.Length - type.Name.Length - 1).Replace('+', '.');
    }

    public static string FullNameWithNesting(this Type type)
    {
      if (!type.IsNested)
        return type.FullName;
      return type.FullName.Replace('+', '.');
    }

    public static bool OverridesEqualsOrGetHashCode(this Type type)
    {
      for (; type != typeof (object); type = type.BaseType())
      {
        if (type.GetDeclaredMethods().Any<MethodInfo>((Func<MethodInfo, bool>) (m =>
        {
          if ((m.Name == "Equals" || m.Name == "GetHashCode") && m.DeclaringType != typeof (object))
            return m.GetBaseDefinition().DeclaringType == typeof (object);
          return false;
        })))
          return true;
      }
      return false;
    }

    public static bool IsPublic(this Type type)
    {
      TypeInfo typeInfo = type.GetTypeInfo();
      if (typeInfo.IsPublic)
        return true;
      if (typeInfo.IsNestedPublic)
        return type.DeclaringType.IsPublic();
      return false;
    }

    public static bool IsNotPublic(this Type type)
    {
      return !type.IsPublic();
    }

    public static MethodInfo GetOnlyDeclaredMethod(this Type type, string name)
    {
      return type.GetDeclaredMethods(name).SingleOrDefault<MethodInfo>();
    }

    public static MethodInfo GetDeclaredMethod(
      this Type type,
      string name,
      params Type[] parameterTypes)
    {
      return type.GetDeclaredMethods(name).SingleOrDefault<MethodInfo>((Func<MethodInfo, bool>) (m => ((IEnumerable<ParameterInfo>) m.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).SequenceEqual<Type>((IEnumerable<Type>) parameterTypes)));
    }

    public static MethodInfo GetPublicInstanceMethod(
      this Type type,
      string name,
      params Type[] parameterTypes)
    {
      return type.GetRuntimeMethod(name, (Func<MethodInfo, bool>) (m =>
      {
        if (m.IsPublic)
          return !m.IsStatic;
        return false;
      }), parameterTypes);
    }

    public static MethodInfo GetRuntimeMethod(
      this Type type,
      string name,
      Func<MethodInfo, bool> predicate,
      params Type[][] parameterTypes)
    {
      return ((IEnumerable<Type[]>) parameterTypes).Select<Type[], MethodInfo>((Func<Type[], MethodInfo>) (t => type.GetRuntimeMethod(name, predicate, t))).FirstOrDefault<MethodInfo>((Func<MethodInfo, bool>) (m => m != (MethodInfo) null));
    }

    private static MethodInfo GetRuntimeMethod(
      this Type type,
      string name,
      Func<MethodInfo, bool> predicate,
      Type[] parameterTypes)
    {
      MethodInfo[] methods = type.GetRuntimeMethods().Where<MethodInfo>((Func<MethodInfo, bool>) (m =>
      {
        if (name == m.Name && predicate(m))
          return ((IEnumerable<ParameterInfo>) m.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).SequenceEqual<Type>((IEnumerable<Type>) parameterTypes);
        return false;
      })).ToArray<MethodInfo>();
      if (methods.Length == 1)
        return methods[0];
      return ((IEnumerable<MethodInfo>) methods).SingleOrDefault<MethodInfo>((Func<MethodInfo, bool>) (m => !((IEnumerable<MethodInfo>) methods).Any<MethodInfo>((Func<MethodInfo, bool>) (m2 => m2.DeclaringType.IsSubclassOf(m.DeclaringType)))));
    }

    public static IEnumerable<MethodInfo> GetDeclaredMethods(this Type type)
    {
      return type.GetTypeInfo().DeclaredMethods;
    }

    public static IEnumerable<MethodInfo> GetDeclaredMethods(
      this Type type,
      string name)
    {
      return type.GetTypeInfo().GetDeclaredMethods(name);
    }

    public static PropertyInfo GetDeclaredProperty(this Type type, string name)
    {
      return type.GetTypeInfo().GetDeclaredProperty(name);
    }

    public static IEnumerable<PropertyInfo> GetDeclaredProperties(
      this Type type)
    {
      return type.GetTypeInfo().DeclaredProperties;
    }

    public static IEnumerable<PropertyInfo> GetInstanceProperties(
      this Type type)
    {
      return type.GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !p.IsStatic()));
    }

    public static IEnumerable<PropertyInfo> GetNonHiddenProperties(
      this Type type)
    {
      return type.GetRuntimeProperties().GroupBy<PropertyInfo, string>((Func<PropertyInfo, string>) (property => property.Name)).Select<IGrouping<string, PropertyInfo>, PropertyInfo>((Func<IGrouping<string, PropertyInfo>, PropertyInfo>) (propertyGroup => TypeExtensions.MostDerived((IEnumerable<PropertyInfo>) propertyGroup)));
    }

    private static PropertyInfo MostDerived(IEnumerable<PropertyInfo> properties)
    {
      PropertyInfo propertyInfo = (PropertyInfo) null;
      foreach (PropertyInfo property in properties)
      {
        if (propertyInfo == (PropertyInfo) null || propertyInfo.DeclaringType != (Type) null && propertyInfo.DeclaringType.IsAssignableFrom(property.DeclaringType))
          propertyInfo = property;
      }
      return propertyInfo;
    }

    public static PropertyInfo GetAnyProperty(this Type type, string name)
    {
      List<PropertyInfo> list = type.GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name == name)).ToList<PropertyInfo>();
      if (list.Count<PropertyInfo>() > 1)
        throw new AmbiguousMatchException();
      return list.SingleOrDefault<PropertyInfo>();
    }

    public static PropertyInfo GetInstanceProperty(this Type type, string name)
    {
      List<PropertyInfo> list = type.GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (p.Name == name)
          return !p.IsStatic();
        return false;
      })).ToList<PropertyInfo>();
      if (list.Count<PropertyInfo>() > 1)
        throw new AmbiguousMatchException();
      return list.SingleOrDefault<PropertyInfo>();
    }

    public static PropertyInfo GetStaticProperty(this Type type, string name)
    {
      List<PropertyInfo> list = type.GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (p.Name == name)
          return p.IsStatic();
        return false;
      })).ToList<PropertyInfo>();
      if (list.Count<PropertyInfo>() > 1)
        throw new AmbiguousMatchException();
      return list.SingleOrDefault<PropertyInfo>();
    }

    public static PropertyInfo GetTopProperty(this Type type, string name)
    {
      do
      {
        TypeInfo typeInfo = type.GetTypeInfo();
        PropertyInfo declaredProperty = typeInfo.GetDeclaredProperty(name);
        if (declaredProperty != (PropertyInfo) null)
        {
          MethodInfo methodInfo = declaredProperty.GetMethod;
          if ((object) methodInfo == null)
            methodInfo = declaredProperty.SetMethod;
          if (!methodInfo.IsStatic)
            return declaredProperty;
        }
        type = typeInfo.BaseType;
      }
      while (type != (Type) null);
      return (PropertyInfo) null;
    }

    public static Assembly Assembly(this Type type)
    {
      return type.GetTypeInfo().Assembly;
    }

    public static Type BaseType(this Type type)
    {
      return type.GetTypeInfo().BaseType;
    }

    public static bool IsGenericType(this Type type)
    {
      return type.GetTypeInfo().IsGenericType;
    }

    public static bool IsGenericTypeDefinition(this Type type)
    {
      return type.GetTypeInfo().IsGenericTypeDefinition;
    }

    public static TypeAttributes Attributes(this Type type)
    {
      return type.GetTypeInfo().Attributes;
    }

    public static bool IsClass(this Type type)
    {
      return type.GetTypeInfo().IsClass;
    }

    public static bool IsInterface(this Type type)
    {
      return type.GetTypeInfo().IsInterface;
    }

    public static bool IsValueType(this Type type)
    {
      return type.GetTypeInfo().IsValueType;
    }

    public static bool IsAbstract(this Type type)
    {
      return type.GetTypeInfo().IsAbstract;
    }

    public static bool IsSealed(this Type type)
    {
      return type.GetTypeInfo().IsSealed;
    }

    public static bool IsEnum(this Type type)
    {
      return type.GetTypeInfo().IsEnum;
    }

    public static bool IsSerializable(this Type type)
    {
      return type.GetTypeInfo().IsSerializable;
    }

    public static bool IsGenericParameter(this Type type)
    {
      return type.GetTypeInfo().IsGenericParameter;
    }

    public static bool ContainsGenericParameters(this Type type)
    {
      return type.GetTypeInfo().ContainsGenericParameters;
    }

    public static bool IsPrimitive(this Type type)
    {
      return type.GetTypeInfo().IsPrimitive;
    }

    public static IEnumerable<ConstructorInfo> GetDeclaredConstructors(
      this Type type)
    {
      return type.GetTypeInfo().DeclaredConstructors;
    }

    public static ConstructorInfo GetDeclaredConstructor(
      this Type type,
      params Type[] parameterTypes)
    {
      return type.GetDeclaredConstructors().SingleOrDefault<ConstructorInfo>((Func<ConstructorInfo, bool>) (c =>
      {
        if (!c.IsStatic)
          return ((IEnumerable<ParameterInfo>) c.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).SequenceEqual<Type>((IEnumerable<Type>) parameterTypes);
        return false;
      }));
    }

    public static ConstructorInfo GetPublicConstructor(
      this Type type,
      params Type[] parameterTypes)
    {
      ConstructorInfo declaredConstructor = type.GetDeclaredConstructor(parameterTypes);
      if (!(declaredConstructor != (ConstructorInfo) null) || !declaredConstructor.IsPublic)
        return (ConstructorInfo) null;
      return declaredConstructor;
    }

    public static ConstructorInfo GetDeclaredConstructor(
      this Type type,
      Func<ConstructorInfo, bool> predicate,
      params Type[][] parameterTypes)
    {
      return ((IEnumerable<Type[]>) parameterTypes).Select<Type[], ConstructorInfo>((Func<Type[], ConstructorInfo>) (p => type.GetDeclaredConstructor(p))).FirstOrDefault<ConstructorInfo>((Func<ConstructorInfo, bool>) (c =>
      {
        if (c != (ConstructorInfo) null)
          return predicate(c);
        return false;
      }));
    }

    public static bool IsSubclassOf(this Type type, Type otherType)
    {
      return type.GetTypeInfo().IsSubclassOf(otherType);
    }
  }
}
