// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.PropertyInfoExtensions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.SqlServer.Utilities
{
  internal static class PropertyInfoExtensions
  {
    public static bool IsSameAs(this PropertyInfo propertyInfo, PropertyInfo otherPropertyInfo)
    {
      if (propertyInfo == otherPropertyInfo)
        return true;
      if (!(propertyInfo.Name == otherPropertyInfo.Name))
        return false;
      if (!(propertyInfo.DeclaringType == otherPropertyInfo.DeclaringType) && !propertyInfo.DeclaringType.IsSubclassOf(otherPropertyInfo.DeclaringType) && (!otherPropertyInfo.DeclaringType.IsSubclassOf(propertyInfo.DeclaringType) && !((IEnumerable<Type>) propertyInfo.DeclaringType.GetInterfaces()).Contains<Type>(otherPropertyInfo.DeclaringType)))
        return ((IEnumerable<Type>) otherPropertyInfo.DeclaringType.GetInterfaces()).Contains<Type>(propertyInfo.DeclaringType);
      return true;
    }

    public static bool ContainsSame(
      this IEnumerable<PropertyInfo> enumerable,
      PropertyInfo propertyInfo)
    {
      return enumerable.Any<PropertyInfo>(new Func<PropertyInfo, bool>(((PropertyInfoExtensions) propertyInfo).IsSameAs));
    }

    public static bool IsValidStructuralProperty(this PropertyInfo propertyInfo)
    {
      if (propertyInfo.IsValidInterfaceStructuralProperty())
        return !propertyInfo.Getter().IsAbstract;
      return false;
    }

    public static bool IsValidInterfaceStructuralProperty(this PropertyInfo propertyInfo)
    {
      if (propertyInfo.CanRead && (propertyInfo.CanWriteExtended() || propertyInfo.PropertyType.IsCollection()) && propertyInfo.GetIndexParameters().Length == 0)
        return propertyInfo.PropertyType.IsValidStructuralPropertyType();
      return false;
    }

    public static bool IsValidEdmScalarProperty(this PropertyInfo propertyInfo)
    {
      if (propertyInfo.IsValidInterfaceStructuralProperty())
        return propertyInfo.PropertyType.IsValidEdmScalarType();
      return false;
    }

    public static bool IsValidEdmNavigationProperty(this PropertyInfo propertyInfo)
    {
      if (!propertyInfo.IsValidInterfaceStructuralProperty())
        return false;
      Type elementType;
      if (!propertyInfo.PropertyType.IsCollection(out elementType) || !elementType.IsValidStructuralType())
        return propertyInfo.PropertyType.IsValidStructuralType();
      return true;
    }

    public static EdmProperty AsEdmPrimitiveProperty(this PropertyInfo propertyInfo)
    {
      Type underlyingType = propertyInfo.PropertyType;
      bool flag = underlyingType.TryUnwrapNullableType(out underlyingType) || !underlyingType.IsValueType();
      PrimitiveType primitiveType;
      if (!underlyingType.IsPrimitiveType(out primitiveType))
        return (EdmProperty) null;
      EdmProperty primitive = EdmProperty.CreatePrimitive(propertyInfo.Name, primitiveType);
      primitive.Nullable = flag;
      return primitive;
    }

    public static bool CanWriteExtended(this PropertyInfo propertyInfo)
    {
      if (propertyInfo.CanWrite)
        return true;
      PropertyInfo declaredProperty = PropertyInfoExtensions.GetDeclaredProperty(propertyInfo);
      if (declaredProperty != (PropertyInfo) null)
        return declaredProperty.CanWrite;
      return false;
    }

    public static PropertyInfo GetPropertyInfoForSet(this PropertyInfo propertyInfo)
    {
      if (propertyInfo.CanWrite)
        return propertyInfo;
      PropertyInfo declaredProperty = PropertyInfoExtensions.GetDeclaredProperty(propertyInfo);
      if ((object) declaredProperty != null)
        return declaredProperty;
      return propertyInfo;
    }

    private static PropertyInfo GetDeclaredProperty(PropertyInfo propertyInfo)
    {
      if (!(propertyInfo.DeclaringType == propertyInfo.ReflectedType))
        return propertyInfo.DeclaringType.GetInstanceProperties().SingleOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
        {
          if (p.Name == propertyInfo.Name && !((IEnumerable<ParameterInfo>) p.GetIndexParameters()).Any<ParameterInfo>())
            return p.PropertyType == propertyInfo.PropertyType;
          return false;
        }));
      return propertyInfo;
    }

    public static IEnumerable<PropertyInfo> GetPropertiesInHierarchy(
      this PropertyInfo property)
    {
      List<PropertyInfo> source = new List<PropertyInfo>()
      {
        property
      };
      PropertyInfoExtensions.CollectProperties(property, (IList<PropertyInfo>) source);
      return source.Distinct<PropertyInfo>();
    }

    private static void CollectProperties(PropertyInfo property, IList<PropertyInfo> collection)
    {
      PropertyInfoExtensions.FindNextProperty(property, collection, true);
      PropertyInfoExtensions.FindNextProperty(property, collection, false);
    }

    private static void FindNextProperty(
      PropertyInfo property,
      IList<PropertyInfo> collection,
      bool getter)
    {
      MethodInfo methodInfo = getter ? property.Getter() : property.Setter();
      if (!(methodInfo != (MethodInfo) null))
        return;
      Type type = methodInfo.DeclaringType.BaseType();
      if (!(type != (Type) null) || !(type != typeof (object)))
        return;
      MethodInfo baseMethod = methodInfo.GetBaseDefinition();
      PropertyInfo property1 = type.GetInstanceProperties().Select(p => new
      {
        p = p,
        candidateMethod = getter ? p.Getter() : p.Setter()
      }).Where(_param1 =>
      {
        if (_param1.candidateMethod != (MethodInfo) null)
          return _param1.candidateMethod.GetBaseDefinition() == baseMethod;
        return false;
      }).Select(_param0 => _param0.p).FirstOrDefault<PropertyInfo>();
      if (!(property1 != (PropertyInfo) null))
        return;
      collection.Add(property1);
      PropertyInfoExtensions.CollectProperties(property1, collection);
    }

    public static MethodInfo Getter(this PropertyInfo property)
    {
      return property.GetMethod;
    }

    public static MethodInfo Setter(this PropertyInfo property)
    {
      return property.SetMethod;
    }

    public static bool IsStatic(this PropertyInfo property)
    {
      MethodInfo methodInfo = property.Getter();
      if ((object) methodInfo == null)
        methodInfo = property.Setter();
      return methodInfo.IsStatic;
    }

    public static bool IsPublic(this PropertyInfo property)
    {
      MethodInfo methodInfo1 = property.Getter();
      MethodAttributes methodAttributes1 = methodInfo1 == (MethodInfo) null ? MethodAttributes.Private : methodInfo1.Attributes & MethodAttributes.MemberAccessMask;
      MethodInfo methodInfo2 = property.Setter();
      MethodAttributes methodAttributes2 = methodInfo2 == (MethodInfo) null ? MethodAttributes.Private : methodInfo2.Attributes & MethodAttributes.MemberAccessMask;
      return (methodAttributes1 > methodAttributes2 ? (int) methodAttributes1 : (int) methodAttributes2) == 6;
    }
  }
}
