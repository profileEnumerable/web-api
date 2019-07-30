// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.PocoPropertyAccessorStrategy
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class PocoPropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    internal static readonly MethodInfo AddToCollectionGeneric = typeof (PocoPropertyAccessorStrategy).GetOnlyDeclaredMethod("AddToCollection");
    internal static readonly MethodInfo RemoveFromCollectionGeneric = typeof (PocoPropertyAccessorStrategy).GetOnlyDeclaredMethod("RemoveFromCollection");
    private readonly object _entity;

    public PocoPropertyAccessorStrategy(object entity)
    {
      this._entity = entity;
    }

    public object GetNavigationPropertyValue(RelatedEnd relatedEnd)
    {
      object obj = (object) null;
      if (relatedEnd != null)
      {
        if (relatedEnd.TargetAccessor.ValueGetter == null)
        {
          Type declaringType = PocoPropertyAccessorStrategy.GetDeclaringType(relatedEnd);
          PropertyInfo topProperty = declaringType.GetTopProperty(relatedEnd.TargetAccessor.PropertyName);
          if (topProperty == (PropertyInfo) null)
            throw new EntityException(Strings.PocoEntityWrapper_UnableToSetFieldOrProperty((object) relatedEnd.TargetAccessor.PropertyName, (object) declaringType.FullName));
          EntityProxyFactory entityProxyFactory = new EntityProxyFactory();
          relatedEnd.TargetAccessor.ValueGetter = entityProxyFactory.CreateBaseGetter(topProperty.DeclaringType, topProperty);
        }
        bool state = relatedEnd.DisableLazyLoading();
        try
        {
          obj = relatedEnd.TargetAccessor.ValueGetter(this._entity);
        }
        catch (Exception ex)
        {
          throw new EntityException(Strings.PocoEntityWrapper_UnableToSetFieldOrProperty((object) relatedEnd.TargetAccessor.PropertyName, (object) this._entity.GetType().FullName), ex);
        }
        finally
        {
          relatedEnd.ResetLazyLoading(state);
        }
      }
      return obj;
    }

    public void SetNavigationPropertyValue(RelatedEnd relatedEnd, object value)
    {
      if (relatedEnd == null)
        return;
      if (relatedEnd.TargetAccessor.ValueSetter == null)
      {
        Type declaringType = PocoPropertyAccessorStrategy.GetDeclaringType(relatedEnd);
        PropertyInfo topProperty = declaringType.GetTopProperty(relatedEnd.TargetAccessor.PropertyName);
        if (topProperty == (PropertyInfo) null)
          throw new EntityException(Strings.PocoEntityWrapper_UnableToSetFieldOrProperty((object) relatedEnd.TargetAccessor.PropertyName, (object) declaringType.FullName));
        EntityProxyFactory entityProxyFactory = new EntityProxyFactory();
        relatedEnd.TargetAccessor.ValueSetter = entityProxyFactory.CreateBaseSetter(topProperty.DeclaringType, topProperty);
      }
      try
      {
        relatedEnd.TargetAccessor.ValueSetter(this._entity, value);
      }
      catch (Exception ex)
      {
        throw new EntityException(Strings.PocoEntityWrapper_UnableToSetFieldOrProperty((object) relatedEnd.TargetAccessor.PropertyName, (object) this._entity.GetType().FullName), ex);
      }
    }

    private static Type GetDeclaringType(RelatedEnd relatedEnd)
    {
      if (relatedEnd.NavigationProperty != null)
        return Util.GetObjectMapping((EdmType) relatedEnd.NavigationProperty.DeclaringType, relatedEnd.WrappedOwner.Context.MetadataWorkspace).ClrType.ClrType;
      return relatedEnd.WrappedOwner.IdentityType;
    }

    private static Type GetNavigationPropertyType(Type entityType, string propertyName)
    {
      PropertyInfo topProperty = entityType.GetTopProperty(propertyName);
      if (topProperty != (PropertyInfo) null)
        return topProperty.PropertyType;
      FieldInfo field = entityType.GetField(propertyName);
      if (field != (FieldInfo) null)
        return field.FieldType;
      throw new EntityException(Strings.PocoEntityWrapper_UnableToSetFieldOrProperty((object) propertyName, (object) entityType.FullName));
    }

    public void CollectionAdd(RelatedEnd relatedEnd, object value)
    {
      object entity = this._entity;
      try
      {
        object navigationPropertyValue = this.GetNavigationPropertyValue(relatedEnd);
        if (navigationPropertyValue == null)
        {
          navigationPropertyValue = this.CollectionCreate(relatedEnd);
          this.SetNavigationPropertyValue(relatedEnd, navigationPropertyValue);
        }
        if (object.ReferenceEquals(navigationPropertyValue, (object) relatedEnd))
          return;
        if (relatedEnd.TargetAccessor.CollectionAdd == null)
          relatedEnd.TargetAccessor.CollectionAdd = PocoPropertyAccessorStrategy.CreateCollectionAddFunction(PocoPropertyAccessorStrategy.GetDeclaringType(relatedEnd), relatedEnd.TargetAccessor.PropertyName);
        relatedEnd.TargetAccessor.CollectionAdd(navigationPropertyValue, value);
      }
      catch (Exception ex)
      {
        throw new EntityException(Strings.PocoEntityWrapper_UnableToSetFieldOrProperty((object) relatedEnd.TargetAccessor.PropertyName, (object) entity.GetType().FullName), ex);
      }
    }

    private static Action<object, object> CreateCollectionAddFunction(
      Type type,
      string propertyName)
    {
      Type collectionElementType = EntityUtil.GetCollectionElementType(PocoPropertyAccessorStrategy.GetNavigationPropertyType(type, propertyName));
      return (Action<object, object>) PocoPropertyAccessorStrategy.AddToCollectionGeneric.MakeGenericMethod(collectionElementType).Invoke((object) null, (object[]) null);
    }

    private static Action<object, object> AddToCollection<T>()
    {
      return (Action<object, object>) ((collectionArg, item) =>
      {
        ICollection<T> objs = (ICollection<T>) collectionArg;
        Array array = objs as Array;
        if (array != null && array.IsFixedSize)
          throw new InvalidOperationException(Strings.RelatedEnd_CannotAddToFixedSizeArray((object) array.GetType()));
        objs.Add((T) item);
      });
    }

    public bool CollectionRemove(RelatedEnd relatedEnd, object value)
    {
      object entity = this._entity;
      try
      {
        object navigationPropertyValue = this.GetNavigationPropertyValue(relatedEnd);
        if (navigationPropertyValue != null)
        {
          if (object.ReferenceEquals(navigationPropertyValue, (object) relatedEnd))
            return true;
          if (relatedEnd.TargetAccessor.CollectionRemove == null)
            relatedEnd.TargetAccessor.CollectionRemove = PocoPropertyAccessorStrategy.CreateCollectionRemoveFunction(PocoPropertyAccessorStrategy.GetDeclaringType(relatedEnd), relatedEnd.TargetAccessor.PropertyName);
          return relatedEnd.TargetAccessor.CollectionRemove(navigationPropertyValue, value);
        }
      }
      catch (Exception ex)
      {
        throw new EntityException(Strings.PocoEntityWrapper_UnableToSetFieldOrProperty((object) relatedEnd.TargetAccessor.PropertyName, (object) entity.GetType().FullName), ex);
      }
      return false;
    }

    private static Func<object, object, bool> CreateCollectionRemoveFunction(
      Type type,
      string propertyName)
    {
      Type collectionElementType = EntityUtil.GetCollectionElementType(PocoPropertyAccessorStrategy.GetNavigationPropertyType(type, propertyName));
      return (Func<object, object, bool>) PocoPropertyAccessorStrategy.RemoveFromCollectionGeneric.MakeGenericMethod(collectionElementType).Invoke((object) null, (object[]) null);
    }

    private static Func<object, object, bool> RemoveFromCollection<T>()
    {
      return (Func<object, object, bool>) ((collectionArg, item) =>
      {
        ICollection<T> objs = (ICollection<T>) collectionArg;
        Array array = objs as Array;
        if (array != null && array.IsFixedSize)
          throw new InvalidOperationException(Strings.RelatedEnd_CannotRemoveFromFixedSizeArray((object) array.GetType()));
        return objs.Remove((T) item);
      });
    }

    public object CollectionCreate(RelatedEnd relatedEnd)
    {
      if (this._entity is IEntityWithRelationships)
        return (object) relatedEnd;
      if (relatedEnd.TargetAccessor.CollectionCreate == null)
      {
        Type declaringType = PocoPropertyAccessorStrategy.GetDeclaringType(relatedEnd);
        string propertyName = relatedEnd.TargetAccessor.PropertyName;
        Type navigationPropertyType = PocoPropertyAccessorStrategy.GetNavigationPropertyType(declaringType, propertyName);
        relatedEnd.TargetAccessor.CollectionCreate = PocoPropertyAccessorStrategy.CreateCollectionCreateDelegate(navigationPropertyType, propertyName);
      }
      return relatedEnd.TargetAccessor.CollectionCreate();
    }

    private static Func<object> CreateCollectionCreateDelegate(
      Type navigationPropertyType,
      string propName)
    {
      Type collectionType = EntityUtil.DetermineCollectionType(navigationPropertyType);
      if (collectionType == (Type) null)
        throw new EntityException(Strings.PocoEntityWrapper_UnableToMaterializeArbitaryNavPropType((object) propName, (object) navigationPropertyType));
      return Expression.Lambda<Func<object>>((Expression) DelegateFactory.GetNewExpressionForCollectionType(collectionType), new ParameterExpression[0]).Compile();
    }
  }
}
