// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ObjectItemAttributeAssemblyLoader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal sealed class ObjectItemAttributeAssemblyLoader : ObjectItemAssemblyLoader
  {
    private readonly List<Action> _unresolvedNavigationProperties = new List<Action>();
    private readonly List<Action> _referenceResolutions = new List<Action>();

    private MutableAssemblyCacheEntry CacheEntry
    {
      get
      {
        return (MutableAssemblyCacheEntry) base.CacheEntry;
      }
    }

    internal ObjectItemAttributeAssemblyLoader(
      Assembly assembly,
      ObjectItemLoadingSessionData sessionData)
      : base(assembly, (AssemblyCacheEntry) new MutableAssemblyCacheEntry(), sessionData)
    {
    }

    internal override void OnLevel1SessionProcessing()
    {
      foreach (Action referenceResolution in this._referenceResolutions)
        referenceResolution();
    }

    internal override void OnLevel2SessionProcessing()
    {
      foreach (Action navigationProperty in this._unresolvedNavigationProperties)
        navigationProperty();
    }

    internal override void Load()
    {
      base.Load();
    }

    protected override void AddToAssembliesLoaded()
    {
      this.SessionData.AssembliesLoaded.Add(this.SourceAssembly, this.CacheEntry);
    }

    private bool TryGetLoadedType(Type clrType, out EdmType edmType)
    {
      if (this.SessionData.TypesInLoading.TryGetValue(clrType.FullName, out edmType) || this.TryGetCachedEdmType(clrType, out edmType))
      {
        if (!(edmType.ClrType != clrType))
          return true;
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.NewTypeConflictsWithExistingType((object) clrType.AssemblyQualifiedName, (object) edmType.ClrType.AssemblyQualifiedName)));
        edmType = (EdmType) null;
        return false;
      }
      if (clrType.IsGenericType())
      {
        clrType.GetGenericTypeDefinition();
        EdmType edmType1;
        if (!this.TryGetLoadedType(clrType.GetGenericArguments()[0], out edmType1))
          return false;
        if (typeof (IEnumerable).IsAssignableFrom(clrType))
        {
          EntityType entityType = edmType1 as EntityType;
          if (entityType == null)
            return false;
          edmType = (EdmType) entityType.GetCollectionType();
        }
        else
          edmType = edmType1;
        return true;
      }
      edmType = (EdmType) null;
      return false;
    }

    private bool TryGetCachedEdmType(Type clrType, out EdmType edmType)
    {
      ImmutableAssemblyCacheEntry cacheEntry;
      if (this.SessionData.LockedAssemblyCache.TryGetValue(clrType.Assembly(), out cacheEntry))
        return cacheEntry.TryGetEdmType(clrType.FullName, out edmType);
      edmType = (EdmType) null;
      return false;
    }

    protected override void LoadTypesFromAssembly()
    {
      this.LoadRelationshipTypes();
      foreach (Type accessibleType in this.SourceAssembly.GetAccessibleTypes())
      {
        if (accessibleType.GetCustomAttributes<EdmTypeAttribute>(false).Any<EdmTypeAttribute>())
        {
          if (accessibleType.IsGenericType())
            this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.GenericTypeNotSupported((object) accessibleType.FullName)));
          else
            this.LoadType(accessibleType);
        }
      }
      if (this._referenceResolutions.Count != 0)
        this.SessionData.RegisterForLevel1PostSessionProcessing((ObjectItemAssemblyLoader) this);
      if (this._unresolvedNavigationProperties.Count == 0)
        return;
      this.SessionData.RegisterForLevel2PostSessionProcessing((ObjectItemAssemblyLoader) this);
    }

    private void LoadRelationshipTypes()
    {
      foreach (EdmRelationshipAttribute customAttribute in this.SourceAssembly.GetCustomAttributes<EdmRelationshipAttribute>())
      {
        if (!this.TryFindNullParametersInRelationshipAttribute(customAttribute))
        {
          bool flag = false;
          if (customAttribute.Role1Name == customAttribute.Role2Name)
          {
            this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.SameRoleNameOnRelationshipAttribute((object) customAttribute.RelationshipName, (object) customAttribute.Role2Name)));
            flag = true;
          }
          if (!flag)
          {
            AssociationType associationType = new AssociationType(customAttribute.RelationshipName, customAttribute.RelationshipNamespaceName, customAttribute.IsForeignKey, DataSpace.OSpace);
            this.SessionData.TypesInLoading.Add(associationType.FullName, (EdmType) associationType);
            this.TrackClosure(customAttribute.Role1Type);
            this.TrackClosure(customAttribute.Role2Type);
            string r1Name = customAttribute.Role1Name;
            Type r1Type = customAttribute.Role1Type;
            RelationshipMultiplicity r1Multiplicity = customAttribute.Role1Multiplicity;
            this.AddTypeResolver((Action) (() => this.ResolveAssociationEnd(associationType, r1Name, r1Type, r1Multiplicity)));
            string r2Name = customAttribute.Role2Name;
            Type r2Type = customAttribute.Role2Type;
            RelationshipMultiplicity r2Multiplicity = customAttribute.Role2Multiplicity;
            this.AddTypeResolver((Action) (() => this.ResolveAssociationEnd(associationType, r2Name, r2Type, r2Multiplicity)));
            this.CacheEntry.TypesInAssembly.Add((EdmType) associationType);
          }
        }
      }
    }

    private void ResolveAssociationEnd(
      AssociationType associationType,
      string roleName,
      Type clrType,
      RelationshipMultiplicity multiplicity)
    {
      EntityType entityType;
      if (!this.TryGetRelationshipEndEntityType(clrType, out entityType))
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.RoleTypeInEdmRelationshipAttributeIsInvalidType((object) associationType.Name, (object) roleName, (object) clrType)));
      else
        associationType.AddKeyMember((EdmMember) new AssociationEndMember(roleName, entityType.GetReferenceType(), multiplicity));
    }

    private void LoadType(Type clrType)
    {
      EdmType edmType = (EdmType) null;
      IEnumerable<EdmTypeAttribute> customAttributes = clrType.GetCustomAttributes<EdmTypeAttribute>(false);
      if (!customAttributes.Any<EdmTypeAttribute>())
        return;
      if (clrType.IsNested)
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.NestedClassNotSupported((object) clrType.FullName, (object) clrType.Assembly().FullName)));
      }
      else
      {
        EdmTypeAttribute edmTypeAttribute = customAttributes.First<EdmTypeAttribute>();
        string cspaceTypeName = string.IsNullOrEmpty(edmTypeAttribute.Name) ? clrType.Name : edmTypeAttribute.Name;
        if (string.IsNullOrEmpty(edmTypeAttribute.NamespaceName) && clrType.Namespace == null)
        {
          this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.Validator_TypeHasNoNamespace));
        }
        else
        {
          string cspaceNamespaceName = string.IsNullOrEmpty(edmTypeAttribute.NamespaceName) ? clrType.Namespace : edmTypeAttribute.NamespaceName;
          if (edmTypeAttribute.GetType() == typeof (EdmEntityTypeAttribute))
            edmType = (EdmType) new ClrEntityType(clrType, cspaceNamespaceName, cspaceTypeName);
          else if (edmTypeAttribute.GetType() == typeof (EdmComplexTypeAttribute))
          {
            edmType = (EdmType) new ClrComplexType(clrType, cspaceNamespaceName, cspaceTypeName);
          }
          else
          {
            PrimitiveType primitiveType;
            if (!ClrProviderManifest.Instance.TryGetPrimitiveType(clrType.GetEnumUnderlyingType(), out primitiveType))
            {
              this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.Validator_UnsupportedEnumUnderlyingType((object) clrType.GetEnumUnderlyingType().FullName)));
              return;
            }
            edmType = (EdmType) new ClrEnumType(clrType, cspaceNamespaceName, cspaceTypeName);
          }
          this.CacheEntry.TypesInAssembly.Add(edmType);
          this.SessionData.TypesInLoading.Add(clrType.FullName, edmType);
          if (!Helper.IsStructuralType(edmType))
            return;
          if (Helper.IsEntityType(edmType))
          {
            this.TrackClosure(clrType.BaseType());
            this.AddTypeResolver((Action) (() => edmType.BaseType = this.ResolveBaseType(clrType.BaseType())));
          }
          this.LoadPropertiesFromType((StructuralType) edmType);
        }
      }
    }

    private void AddTypeResolver(Action resolver)
    {
      this._referenceResolutions.Add(resolver);
    }

    private EdmType ResolveBaseType(Type type)
    {
      EdmType edmType;
      if (type.GetCustomAttributes<EdmEntityTypeAttribute>(false).Any<EdmEntityTypeAttribute>() && this.TryGetLoadedType(type, out edmType))
        return edmType;
      return (EdmType) null;
    }

    private bool TryFindNullParametersInRelationshipAttribute(EdmRelationshipAttribute roleAttribute)
    {
      if (roleAttribute.RelationshipName == null)
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.NullRelationshipNameforEdmRelationshipAttribute((object) this.SourceAssembly.FullName)));
        return true;
      }
      bool flag = false;
      if (roleAttribute.RelationshipNamespaceName == null)
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute((object) "RelationshipNamespaceName", (object) roleAttribute.RelationshipName)));
        flag = true;
      }
      if (roleAttribute.Role1Name == null)
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute((object) "Role1Name", (object) roleAttribute.RelationshipName)));
        flag = true;
      }
      if (roleAttribute.Role1Type == (Type) null)
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute((object) "Role1Type", (object) roleAttribute.RelationshipName)));
        flag = true;
      }
      if (roleAttribute.Role2Name == null)
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute((object) "Role2Name", (object) roleAttribute.RelationshipName)));
        flag = true;
      }
      if (roleAttribute.Role2Type == (Type) null)
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute((object) "Role2Type", (object) roleAttribute.RelationshipName)));
        flag = true;
      }
      return flag;
    }

    private bool TryGetRelationshipEndEntityType(Type type, out EntityType entityType)
    {
      if (type == (Type) null)
      {
        entityType = (EntityType) null;
        return false;
      }
      EdmType edmType;
      if (!this.TryGetLoadedType(type, out edmType) || !Helper.IsEntityType(edmType))
      {
        entityType = (EntityType) null;
        return false;
      }
      entityType = (EntityType) edmType;
      return true;
    }

    private void LoadPropertiesFromType(StructuralType structuralType)
    {
      foreach (PropertyInfo propertyInfo in structuralType.ClrType.GetDeclaredProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !p.IsStatic())))
      {
        EdmMember member = (EdmMember) null;
        bool isEntityKeyProperty = false;
        if (propertyInfo.GetCustomAttributes<EdmRelationshipNavigationPropertyAttribute>(false).Any<EdmRelationshipNavigationPropertyAttribute>())
        {
          PropertyInfo pi = propertyInfo;
          this._unresolvedNavigationProperties.Add((Action) (() => this.ResolveNavigationProperty(structuralType, pi)));
        }
        else if (propertyInfo.GetCustomAttributes<EdmScalarPropertyAttribute>(false).Any<EdmScalarPropertyAttribute>())
        {
          Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
          if ((object) type == null)
            type = propertyInfo.PropertyType;
          if (type.IsEnum())
          {
            this.TrackClosure(propertyInfo.PropertyType);
            PropertyInfo local = propertyInfo;
            this.AddTypeResolver((Action) (() => this.ResolveEnumTypeProperty(structuralType, local)));
          }
          else
            member = this.LoadScalarProperty(structuralType.ClrType, propertyInfo, out isEntityKeyProperty);
        }
        else if (propertyInfo.GetCustomAttributes<EdmComplexPropertyAttribute>(false).Any<EdmComplexPropertyAttribute>())
        {
          this.TrackClosure(propertyInfo.PropertyType);
          PropertyInfo local = propertyInfo;
          this.AddTypeResolver((Action) (() => this.ResolveComplexTypeProperty(structuralType, local)));
        }
        if (member != null)
        {
          structuralType.AddMember(member);
          if (Helper.IsEntityType((EdmType) structuralType) && isEntityKeyProperty)
            ((EntityTypeBase) structuralType).AddKeyMember(member);
        }
      }
    }

    internal void ResolveNavigationProperty(StructuralType declaringType, PropertyInfo propertyInfo)
    {
      IEnumerable<EdmRelationshipNavigationPropertyAttribute> customAttributes = propertyInfo.GetCustomAttributes<EdmRelationshipNavigationPropertyAttribute>(false);
      EdmType edmType;
      if (!this.TryGetLoadedType(propertyInfo.PropertyType, out edmType) || edmType.BuiltInTypeKind != BuiltInTypeKind.EntityType && edmType.BuiltInTypeKind != BuiltInTypeKind.CollectionType)
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.Validator_OSpace_InvalidNavPropReturnType((object) propertyInfo.Name, (object) propertyInfo.DeclaringType.FullName, (object) propertyInfo.PropertyType.FullName)));
      }
      else
      {
        EdmRelationshipNavigationPropertyAttribute propertyAttribute = customAttributes.First<EdmRelationshipNavigationPropertyAttribute>();
        EdmMember member = (EdmMember) null;
        EdmType type;
        if (this.SessionData.TypesInLoading.TryGetValue(propertyAttribute.RelationshipNamespaceName + "." + propertyAttribute.RelationshipName, out type) && Helper.IsAssociationType(type))
        {
          AssociationType associationType = (AssociationType) type;
          if (associationType != null)
          {
            NavigationProperty navigationProperty = new NavigationProperty(propertyInfo.Name, TypeUsage.Create(edmType));
            navigationProperty.RelationshipType = (RelationshipType) associationType;
            member = (EdmMember) navigationProperty;
            if (associationType.Members[0].Name == propertyAttribute.TargetRoleName)
            {
              navigationProperty.ToEndMember = (RelationshipEndMember) associationType.Members[0];
              navigationProperty.FromEndMember = (RelationshipEndMember) associationType.Members[1];
            }
            else if (associationType.Members[1].Name == propertyAttribute.TargetRoleName)
            {
              navigationProperty.ToEndMember = (RelationshipEndMember) associationType.Members[1];
              navigationProperty.FromEndMember = (RelationshipEndMember) associationType.Members[0];
            }
            else
            {
              this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.TargetRoleNameInNavigationPropertyNotValid((object) propertyInfo.Name, (object) propertyInfo.DeclaringType.FullName, (object) propertyAttribute.TargetRoleName, (object) propertyAttribute.RelationshipName)));
              member = (EdmMember) null;
            }
            if (member != null && ((RefType) navigationProperty.FromEndMember.TypeUsage.EdmType).ElementType.ClrType != declaringType.ClrType)
            {
              this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.NavigationPropertyRelationshipEndTypeMismatch((object) declaringType.FullName, (object) navigationProperty.Name, (object) associationType.FullName, (object) navigationProperty.FromEndMember.Name, (object) ((RefType) navigationProperty.FromEndMember.TypeUsage.EdmType).ElementType.ClrType)));
              member = (EdmMember) null;
            }
          }
        }
        else
          this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.RelationshipNameInNavigationPropertyNotValid((object) propertyInfo.Name, (object) propertyInfo.DeclaringType.FullName, (object) propertyAttribute.RelationshipName)));
        if (member == null)
          return;
        declaringType.AddMember(member);
      }
    }

    private EdmMember LoadScalarProperty(
      Type clrType,
      PropertyInfo property,
      out bool isEntityKeyProperty)
    {
      EdmMember edmMember = (EdmMember) null;
      isEntityKeyProperty = false;
      PrimitiveType primitiveType;
      if (!ObjectItemAssemblyLoader.TryGetPrimitiveType(property.PropertyType, out primitiveType))
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.Validator_OSpace_ScalarPropertyNotPrimitive((object) property.Name, (object) property.DeclaringType.FullName, (object) property.PropertyType.FullName)));
      }
      else
      {
        IEnumerable<EdmScalarPropertyAttribute> customAttributes = property.GetCustomAttributes<EdmScalarPropertyAttribute>(false);
        isEntityKeyProperty = customAttributes.First<EdmScalarPropertyAttribute>().EntityKeyProperty;
        bool isNullable = customAttributes.First<EdmScalarPropertyAttribute>().IsNullable;
        edmMember = (EdmMember) new EdmProperty(property.Name, TypeUsage.Create((EdmType) primitiveType, new FacetValues()
        {
          Nullable = (FacetValueContainer<bool?>) new bool?(isNullable)
        }), property, clrType);
      }
      return edmMember;
    }

    private void ResolveEnumTypeProperty(StructuralType declaringType, PropertyInfo clrProperty)
    {
      EdmType edmType;
      if (!this.TryGetLoadedType(clrProperty.PropertyType, out edmType) || !Helper.IsEnumType(edmType))
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.Validator_OSpace_ScalarPropertyNotPrimitive((object) clrProperty.Name, (object) clrProperty.DeclaringType.FullName, (object) clrProperty.PropertyType.FullName)));
      }
      else
      {
        EdmScalarPropertyAttribute propertyAttribute = clrProperty.GetCustomAttributes<EdmScalarPropertyAttribute>(false).Single<EdmScalarPropertyAttribute>();
        EdmProperty edmProperty = new EdmProperty(clrProperty.Name, TypeUsage.Create(edmType, new FacetValues()
        {
          Nullable = (FacetValueContainer<bool?>) new bool?(propertyAttribute.IsNullable)
        }), clrProperty, declaringType.ClrType);
        declaringType.AddMember((EdmMember) edmProperty);
        if (declaringType.BuiltInTypeKind != BuiltInTypeKind.EntityType || !propertyAttribute.EntityKeyProperty)
          return;
        ((EntityTypeBase) declaringType).AddKeyMember((EdmMember) edmProperty);
      }
    }

    private void ResolveComplexTypeProperty(StructuralType type, PropertyInfo clrProperty)
    {
      EdmType edmType;
      if (!this.TryGetLoadedType(clrProperty.PropertyType, out edmType) || edmType.BuiltInTypeKind != BuiltInTypeKind.ComplexType)
      {
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.Validator_OSpace_ComplexPropertyNotComplex((object) clrProperty.Name, (object) clrProperty.DeclaringType.FullName, (object) clrProperty.PropertyType.FullName)));
      }
      else
      {
        EdmProperty edmProperty = new EdmProperty(clrProperty.Name, TypeUsage.Create(edmType, new FacetValues()
        {
          Nullable = (FacetValueContainer<bool?>) new bool?(false)
        }), clrProperty, type.ClrType);
        type.AddMember((EdmMember) edmProperty);
      }
    }

    private void TrackClosure(Type type)
    {
      if (this.SourceAssembly != type.Assembly() && !this.CacheEntry.ClosureAssemblies.Contains(type.Assembly()) && ObjectItemAttributeAssemblyLoader.IsSchemaAttributePresent(type.Assembly()) && (!type.IsGenericType() || !EntityUtil.IsAnICollection(type) && !(type.GetGenericTypeDefinition() == typeof (EntityReference<>)) && !(type.GetGenericTypeDefinition() == typeof (Nullable<>))))
        this.CacheEntry.ClosureAssemblies.Add(type.Assembly());
      if (!type.IsGenericType())
        return;
      foreach (Type genericArgument in type.GetGenericArguments())
        this.TrackClosure(genericArgument);
    }

    internal static bool IsSchemaAttributePresent(Assembly assembly)
    {
      return assembly.GetCustomAttributes<EdmSchemaAttribute>().Any<EdmSchemaAttribute>();
    }

    internal static ObjectItemAssemblyLoader Create(
      Assembly assembly,
      ObjectItemLoadingSessionData sessionData)
    {
      if (!ObjectItemAttributeAssemblyLoader.IsSchemaAttributePresent(assembly))
        return (ObjectItemAssemblyLoader) new ObjectItemNoOpAssemblyLoader(assembly, sessionData);
      return (ObjectItemAssemblyLoader) new ObjectItemAttributeAssemblyLoader(assembly, sessionData);
    }
  }
}
