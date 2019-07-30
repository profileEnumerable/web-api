// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.EntityProxyTypeInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class EntityProxyTypeInfo
  {
    private readonly Dictionary<string, AssociationType> _navigationPropertyAssociationTypes = new Dictionary<string, AssociationType>();
    internal const string EntityWrapperFieldName = "_entityWrapper";
    private const string InitializeEntityCollectionsName = "InitializeEntityCollections";
    private readonly Type _proxyType;
    private readonly ClrEntityType _entityType;
    private readonly DynamicMethod _initializeCollections;
    private readonly Func<object, string, object> _baseGetter;
    private readonly HashSet<string> _propertiesWithBaseGetter;
    private readonly Action<object, string, object> _baseSetter;
    private readonly HashSet<string> _propertiesWithBaseSetter;
    private readonly Func<object, object> Proxy_GetEntityWrapper;
    private readonly Func<object, object, object> Proxy_SetEntityWrapper;
    private readonly Func<object> _createObject;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    internal EntityProxyTypeInfo(
      Type proxyType,
      ClrEntityType ospaceEntityType,
      DynamicMethod initializeCollections,
      List<PropertyInfo> baseGetters,
      List<PropertyInfo> baseSetters,
      MetadataWorkspace workspace)
    {
      this._proxyType = proxyType;
      this._entityType = ospaceEntityType;
      this._initializeCollections = initializeCollections;
      foreach (AssociationType associationType in EntityProxyTypeInfo.GetAllRelationshipsForType(workspace, proxyType))
      {
        this._navigationPropertyAssociationTypes.Add(associationType.FullName, associationType);
        if (associationType.Name != associationType.FullName)
          this._navigationPropertyAssociationTypes.Add(associationType.Name, associationType);
      }
      FieldInfo field = proxyType.GetField("_entityWrapper", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ParameterExpression parameterExpression4 = Expression.Parameter(typeof (object), "proxy");
      ParameterExpression parameterExpression5 = Expression.Parameter(typeof (object), "value");
      Func<object, object> getEntityWrapperDelegate = Expression.Lambda<Func<object, object>>((Expression) Expression.Field((Expression) Expression.Convert((Expression) parameterExpression4, field.DeclaringType), field), parameterExpression4).Compile();
      this.Proxy_GetEntityWrapper = (Func<object, object>) (proxy =>
      {
        IEntityWrapper entityWrapper = (IEntityWrapper) getEntityWrapperDelegate(proxy);
        if (entityWrapper != null && !object.ReferenceEquals(entityWrapper.Entity, proxy))
          throw new InvalidOperationException(Strings.EntityProxyTypeInfo_ProxyHasWrongWrapper);
        return (object) entityWrapper;
      });
      this.Proxy_SetEntityWrapper = ((Expression<Func<object, object, object>>) ((parameterExpression1, parameterExpression2) => Expression.Assign((Expression) Expression.Field((Expression) Expression.Convert((Expression) parameterExpression4, field.DeclaringType), field), parameterExpression2))).Compile();
      ParameterExpression parameterExpression6 = Expression.Parameter(typeof (string), "propertyName");
      MethodInfo publicInstanceMethod1 = proxyType.GetPublicInstanceMethod("GetBasePropertyValue", typeof (string));
      if (publicInstanceMethod1 != (MethodInfo) null)
        this._baseGetter = Expression.Lambda<Func<object, string, object>>((Expression) Expression.Call((Expression) Expression.Convert((Expression) parameterExpression4, proxyType), publicInstanceMethod1, (Expression) parameterExpression6), parameterExpression4, parameterExpression6).Compile();
      ParameterExpression parameterExpression7 = Expression.Parameter(typeof (object), "propertyName");
      MethodInfo publicInstanceMethod2 = proxyType.GetPublicInstanceMethod("SetBasePropertyValue", typeof (string), typeof (object));
      if (publicInstanceMethod2 != (MethodInfo) null)
        this._baseSetter = ((Expression<Action<object, string, object>>) ((parameterExpression1, parameterExpression2, parameterExpression3) => Expression.Call((Expression) Expression.Convert(parameterExpression1, proxyType), publicInstanceMethod2, parameterExpression2, parameterExpression3))).Compile();
      this._propertiesWithBaseGetter = new HashSet<string>(baseGetters.Select<PropertyInfo, string>((Func<PropertyInfo, string>) (p => p.Name)));
      this._propertiesWithBaseSetter = new HashSet<string>(baseSetters.Select<PropertyInfo, string>((Func<PropertyInfo, string>) (p => p.Name)));
      this._createObject = DelegateFactory.CreateConstructor(proxyType);
    }

    internal static IEnumerable<AssociationType> GetAllRelationshipsForType(
      MetadataWorkspace workspace,
      Type clrType)
    {
      return workspace.GetItemCollection(DataSpace.OSpace).GetItems<AssociationType>().Where<AssociationType>((Func<AssociationType, bool>) (a =>
      {
        if (!EntityProxyTypeInfo.IsEndMemberForType(a.AssociationEndMembers[0], clrType))
          return EntityProxyTypeInfo.IsEndMemberForType(a.AssociationEndMembers[1], clrType);
        return true;
      }));
    }

    private static bool IsEndMemberForType(AssociationEndMember end, Type clrType)
    {
      RefType edmType = end.TypeUsage.EdmType as RefType;
      if (edmType != null)
        return edmType.ElementType.ClrType.IsAssignableFrom(clrType);
      return false;
    }

    internal object CreateProxyObject()
    {
      return this._createObject();
    }

    internal Type ProxyType
    {
      get
      {
        return this._proxyType;
      }
    }

    internal DynamicMethod InitializeEntityCollections
    {
      get
      {
        return this._initializeCollections;
      }
    }

    public Func<object, string, object> BaseGetter
    {
      get
      {
        return this._baseGetter;
      }
    }

    public bool ContainsBaseGetter(string propertyName)
    {
      if (this.BaseGetter != null)
        return this._propertiesWithBaseGetter.Contains(propertyName);
      return false;
    }

    public bool ContainsBaseSetter(string propertyName)
    {
      if (this.BaseSetter != null)
        return this._propertiesWithBaseSetter.Contains(propertyName);
      return false;
    }

    public Action<object, string, object> BaseSetter
    {
      get
      {
        return this._baseSetter;
      }
    }

    public bool TryGetNavigationPropertyAssociationType(
      string relationshipName,
      out AssociationType associationType)
    {
      return this._navigationPropertyAssociationTypes.TryGetValue(relationshipName, out associationType);
    }

    public IEnumerable<AssociationType> GetAllAssociationTypes()
    {
      return this._navigationPropertyAssociationTypes.Values.Distinct<AssociationType>();
    }

    public void ValidateType(ClrEntityType ospaceEntityType)
    {
      if (ospaceEntityType != this._entityType && ospaceEntityType.HashedDescription != this._entityType.HashedDescription)
        throw new InvalidOperationException(Strings.EntityProxyTypeInfo_DuplicateOSpaceType((object) ospaceEntityType.ClrType.FullName));
    }

    internal IEntityWrapper SetEntityWrapper(IEntityWrapper wrapper)
    {
      return this.Proxy_SetEntityWrapper(wrapper.Entity, (object) wrapper) as IEntityWrapper;
    }

    internal IEntityWrapper GetEntityWrapper(object entity)
    {
      return this.Proxy_GetEntityWrapper(entity) as IEntityWrapper;
    }

    internal Func<object, object> EntityWrapperDelegate
    {
      get
      {
        return this.Proxy_GetEntityWrapper;
      }
    }
  }
}
