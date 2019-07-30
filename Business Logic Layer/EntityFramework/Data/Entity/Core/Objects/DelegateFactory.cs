// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DelegateFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Objects
{
  internal static class DelegateFactory
  {
    private static readonly MethodInfo _throwSetInvalidValue = typeof (EntityUtil).GetDeclaredMethod("ThrowSetInvalidValue", typeof (object), typeof (Type), typeof (string), typeof (string));

    internal static Func<object> GetConstructorDelegateForType(ClrComplexType clrType)
    {
      return clrType.Constructor ?? (clrType.Constructor = DelegateFactory.CreateConstructor(clrType.ClrType));
    }

    internal static Func<object> GetConstructorDelegateForType(ClrEntityType clrType)
    {
      return clrType.Constructor ?? (clrType.Constructor = DelegateFactory.CreateConstructor(clrType.ClrType));
    }

    internal static object GetValue(EdmProperty property, object target)
    {
      return DelegateFactory.GetGetterDelegateForProperty(property)(target);
    }

    internal static Func<object, object> GetGetterDelegateForProperty(EdmProperty property)
    {
      return property.ValueGetter ?? (property.ValueGetter = DelegateFactory.CreatePropertyGetter(property.EntityDeclaringType, property.PropertyInfo));
    }

    internal static void SetValue(EdmProperty property, object target, object value)
    {
      DelegateFactory.GetSetterDelegateForProperty(property)(target, value);
    }

    internal static Action<object, object> GetSetterDelegateForProperty(EdmProperty property)
    {
      Action<object, object> action = property.ValueSetter;
      if (action == null)
      {
        action = DelegateFactory.CreatePropertySetter(property.EntityDeclaringType, property.PropertyInfo, property.Nullable);
        property.ValueSetter = action;
      }
      return action;
    }

    internal static RelatedEnd GetRelatedEnd(
      RelationshipManager sourceRelationshipManager,
      AssociationEndMember sourceMember,
      AssociationEndMember targetMember,
      RelatedEnd existingRelatedEnd)
    {
      Func<RelationshipManager, RelatedEnd, RelatedEnd> func = sourceMember.GetRelatedEnd;
      if (func == null)
      {
        func = DelegateFactory.CreateGetRelatedEndMethod(sourceMember, targetMember);
        sourceMember.GetRelatedEnd = func;
      }
      return func(sourceRelationshipManager, existingRelatedEnd);
    }

    internal static Action<object, object> CreateNavigationPropertySetter(
      Type declaringType,
      PropertyInfo navigationProperty)
    {
      PropertyInfo propertyInfoForSet = navigationProperty.GetPropertyInfoForSet();
      MethodInfo methodInfo = propertyInfoForSet.Setter();
      if (methodInfo == (MethodInfo) null)
        throw new InvalidOperationException(Strings.CodeGen_PropertyNoSetter);
      if (methodInfo.IsStatic)
        throw new InvalidOperationException(Strings.CodeGen_PropertyIsStatic);
      if (methodInfo.DeclaringType.IsValueType())
        throw new InvalidOperationException(Strings.CodeGen_PropertyDeclaringTypeIsValueType);
      ParameterExpression parameterExpression;
      return ((Expression<Action<object, object>>) ((entity, target) => Expression.Assign((Expression) Expression.Property((Expression) Expression.Convert((Expression) parameterExpression, declaringType), propertyInfoForSet), (Expression) Expression.Convert(target, navigationProperty.PropertyType)))).Compile();
    }

    internal static ConstructorInfo GetConstructorForType(Type type)
    {
      ConstructorInfo declaredConstructor = type.GetDeclaredConstructor();
      if ((ConstructorInfo) null == declaredConstructor)
        throw new InvalidOperationException(Strings.CodeGen_ConstructorNoParameterless((object) type.FullName));
      return declaredConstructor;
    }

    internal static NewExpression GetNewExpressionForCollectionType(Type type)
    {
      if (!type.IsGenericType() || !(type.GetGenericTypeDefinition() == typeof (HashSet<>)))
        return Expression.New(DelegateFactory.GetConstructorForType(type));
      return Expression.New(type.GetDeclaredConstructor(typeof (IEqualityComparer<>).MakeGenericType(type.GetGenericArguments())), (Expression) Expression.New(typeof (ObjectReferenceEqualityComparer)));
    }

    internal static Func<object> CreateConstructor(Type type)
    {
      DelegateFactory.GetConstructorForType(type);
      return ((Expression<Func<object>>) (() => Expression.New(type))).Compile();
    }

    internal static Func<object, object> CreatePropertyGetter(
      Type entityDeclaringType,
      PropertyInfo propertyInfo)
    {
      MethodInfo methodInfo = propertyInfo.Getter();
      if (methodInfo == (MethodInfo) null)
        throw new InvalidOperationException(Strings.CodeGen_PropertyNoGetter);
      if (methodInfo.IsStatic)
        throw new InvalidOperationException(Strings.CodeGen_PropertyIsStatic);
      if (propertyInfo.DeclaringType.IsValueType())
        throw new InvalidOperationException(Strings.CodeGen_PropertyDeclaringTypeIsValueType);
      if (((IEnumerable<ParameterInfo>) propertyInfo.GetIndexParameters()).Any<ParameterInfo>())
        throw new InvalidOperationException(Strings.CodeGen_PropertyIsIndexed);
      Type propertyType = propertyInfo.PropertyType;
      if (propertyType.IsPointer)
        throw new InvalidOperationException(Strings.CodeGen_PropertyUnsupportedType);
      ParameterExpression parameterExpression = Expression.Parameter(typeof (object), "entity");
      Expression expression = (Expression) Expression.Property((Expression) Expression.Convert((Expression) parameterExpression, entityDeclaringType), propertyInfo);
      if (propertyType.IsValueType())
        expression = (Expression) Expression.Convert(expression, typeof (object));
      return Expression.Lambda<Func<object, object>>(expression, parameterExpression).Compile();
    }

    internal static Action<object, object> CreatePropertySetter(
      Type entityDeclaringType,
      PropertyInfo propertyInfo,
      bool allowNull)
    {
      PropertyInfo property = DelegateFactory.ValidateSetterProperty(propertyInfo);
      ParameterExpression parameterExpression3 = Expression.Parameter(typeof (object), "entity");
      ParameterExpression parameterExpression4 = Expression.Parameter(typeof (object), "target");
      Type propertyType = propertyInfo.PropertyType;
      if (propertyType.IsValueType() && Nullable.GetUnderlyingType(propertyType) == (Type) null)
        allowNull = false;
      Expression expression = (Expression) Expression.TypeIs((Expression) parameterExpression4, propertyType);
      if (allowNull)
        expression = (Expression) Expression.Or((Expression) Expression.ReferenceEqual((Expression) parameterExpression4, (Expression) Expression.Constant((object) null)), expression);
      return ((Expression<Action<object, object>>) ((parameterExpression1, parameterExpression2) => Expression.IfThenElse(expression, Expression.Assign((Expression) Expression.Property((Expression) Expression.Convert((Expression) parameterExpression3, entityDeclaringType), property), (Expression) Expression.Convert(parameterExpression2, propertyInfo.PropertyType)), Expression.Call(DelegateFactory._throwSetInvalidValue, parameterExpression2, propertyType, entityDeclaringType.Name, propertyInfo.Name)))).Compile();
    }

    internal static PropertyInfo ValidateSetterProperty(PropertyInfo propertyInfo)
    {
      PropertyInfo propertyInfoForSet = propertyInfo.GetPropertyInfoForSet();
      MethodInfo methodInfo = propertyInfoForSet.Setter();
      if (methodInfo == (MethodInfo) null)
        throw new InvalidOperationException(Strings.CodeGen_PropertyNoSetter);
      if (methodInfo.IsStatic)
        throw new InvalidOperationException(Strings.CodeGen_PropertyIsStatic);
      if (propertyInfoForSet.DeclaringType.IsValueType())
        throw new InvalidOperationException(Strings.CodeGen_PropertyDeclaringTypeIsValueType);
      if (((IEnumerable<ParameterInfo>) propertyInfoForSet.GetIndexParameters()).Any<ParameterInfo>())
        throw new InvalidOperationException(Strings.CodeGen_PropertyIsIndexed);
      if (propertyInfoForSet.PropertyType.IsPointer)
        throw new InvalidOperationException(Strings.CodeGen_PropertyUnsupportedType);
      return propertyInfoForSet;
    }

    private static Func<RelationshipManager, RelatedEnd, RelatedEnd> CreateGetRelatedEndMethod(
      AssociationEndMember sourceMember,
      AssociationEndMember targetMember)
    {
      EntityType entityTypeForEnd1 = MetadataHelper.GetEntityTypeForEnd(sourceMember);
      EntityType entityTypeForEnd2 = MetadataHelper.GetEntityTypeForEnd(targetMember);
      NavigationPropertyAccessor propertyAccessor1 = MetadataHelper.GetNavigationPropertyAccessor(entityTypeForEnd2, targetMember, sourceMember);
      NavigationPropertyAccessor propertyAccessor2 = MetadataHelper.GetNavigationPropertyAccessor(entityTypeForEnd1, sourceMember, targetMember);
      return (Func<RelationshipManager, RelatedEnd, RelatedEnd>) typeof (DelegateFactory).GetDeclaredMethod(nameof (CreateGetRelatedEndMethod), typeof (AssociationEndMember), typeof (AssociationEndMember), typeof (NavigationPropertyAccessor), typeof (NavigationPropertyAccessor)).MakeGenericMethod(entityTypeForEnd1.ClrType, entityTypeForEnd2.ClrType).Invoke((object) null, new object[4]
      {
        (object) sourceMember,
        (object) targetMember,
        (object) propertyAccessor1,
        (object) propertyAccessor2
      });
    }

    private static Func<RelationshipManager, RelatedEnd, RelatedEnd> CreateGetRelatedEndMethod<TSource, TTarget>(
      AssociationEndMember sourceMember,
      AssociationEndMember targetMember,
      NavigationPropertyAccessor sourceAccessor,
      NavigationPropertyAccessor targetAccessor)
      where TSource : class
      where TTarget : class
    {
      switch (targetMember.RelationshipMultiplicity)
      {
        case RelationshipMultiplicity.ZeroOrOne:
        case RelationshipMultiplicity.One:
          return (Func<RelationshipManager, RelatedEnd, RelatedEnd>) ((manager, relatedEnd) => (RelatedEnd) manager.GetRelatedReference<TSource, TTarget>(sourceMember, targetMember, sourceAccessor, targetAccessor, relatedEnd));
        case RelationshipMultiplicity.Many:
          return (Func<RelationshipManager, RelatedEnd, RelatedEnd>) ((manager, relatedEnd) => (RelatedEnd) manager.GetRelatedCollection<TSource, TTarget>(sourceMember, targetMember, sourceAccessor, targetAccessor, relatedEnd));
        default:
          Type type = typeof (RelationshipMultiplicity);
          throw new ArgumentOutOfRangeException(type.Name, Strings.ADP_InvalidEnumerationValue((object) type.Name, (object) ((int) targetMember.RelationshipMultiplicity).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }
  }
}
