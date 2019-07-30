// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.OSpaceTypeFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal abstract class OSpaceTypeFactory
  {
    public abstract List<Action> ReferenceResolutions { get; }

    public abstract void LogLoadMessage(string message, EdmType relatedType);

    public abstract void LogError(string errorMessage, EdmType relatedType);

    public abstract void TrackClosure(Type type);

    public abstract Dictionary<EdmType, EdmType> CspaceToOspace { get; }

    public abstract Dictionary<string, EdmType> LoadedTypes { get; }

    public abstract void AddToTypesInAssembly(EdmType type);

    public virtual EdmType TryCreateType(Type type, EdmType cspaceType)
    {
      if (Helper.IsEnumType(cspaceType) ^ type.IsEnum())
      {
        this.LogLoadMessage(Strings.Validator_OSpace_Convention_SSpaceOSpaceTypeMismatch((object) cspaceType.FullName, (object) cspaceType.FullName), cspaceType);
        return (EdmType) null;
      }
      if (Helper.IsEnumType(cspaceType))
      {
        EdmType newOSpaceType;
        this.TryCreateEnumType(type, (EnumType) cspaceType, out newOSpaceType);
        return newOSpaceType;
      }
      EdmType newOSpaceType1;
      this.TryCreateStructuralType(type, (StructuralType) cspaceType, out newOSpaceType1);
      return newOSpaceType1;
    }

    private bool TryCreateEnumType(
      Type enumType,
      EnumType cspaceEnumType,
      out EdmType newOSpaceType)
    {
      newOSpaceType = (EdmType) null;
      if (!this.UnderlyingEnumTypesMatch(enumType, cspaceEnumType) || !this.EnumMembersMatch(enumType, cspaceEnumType))
        return false;
      newOSpaceType = (EdmType) new ClrEnumType(enumType, cspaceEnumType.NamespaceName, cspaceEnumType.Name);
      this.LoadedTypes.Add(enumType.FullName, newOSpaceType);
      return true;
    }

    private bool TryCreateStructuralType(
      Type type,
      StructuralType cspaceType,
      out EdmType newOSpaceType)
    {
      List<Action> referenceResolutionListForCurrentType = new List<Action>();
      newOSpaceType = (EdmType) null;
      StructuralType ospaceType = !Helper.IsEntityType((EdmType) cspaceType) ? (StructuralType) new ClrComplexType(type, cspaceType.NamespaceName, cspaceType.Name) : (StructuralType) new ClrEntityType(type, cspaceType.NamespaceName, cspaceType.Name);
      if (cspaceType.BaseType != null)
      {
        if (OSpaceTypeFactory.TypesMatchByConvention(type.BaseType(), cspaceType.BaseType))
        {
          this.TrackClosure(type.BaseType());
          referenceResolutionListForCurrentType.Add((Action) (() => ospaceType.BaseType = this.ResolveBaseType((StructuralType) cspaceType.BaseType, type)));
        }
        else
        {
          this.LogLoadMessage(Strings.Validator_OSpace_Convention_BaseTypeIncompatible((object) type.BaseType().FullName, (object) type.FullName, (object) cspaceType.BaseType.FullName), (EdmType) cspaceType);
          return false;
        }
      }
      if (!this.TryCreateMembers(type, cspaceType, ospaceType, referenceResolutionListForCurrentType))
        return false;
      this.LoadedTypes.Add(type.FullName, (EdmType) ospaceType);
      foreach (Action action in referenceResolutionListForCurrentType)
        this.ReferenceResolutions.Add(action);
      newOSpaceType = (EdmType) ospaceType;
      return true;
    }

    internal static bool TypesMatchByConvention(Type type, EdmType cspaceType)
    {
      return type.Name == cspaceType.Name;
    }

    private bool UnderlyingEnumTypesMatch(Type enumType, EnumType cspaceEnumType)
    {
      PrimitiveType primitiveType;
      if (!ClrProviderManifest.Instance.TryGetPrimitiveType(enumType.GetEnumUnderlyingType(), out primitiveType))
      {
        this.LogLoadMessage(Strings.Validator_UnsupportedEnumUnderlyingType((object) enumType.GetEnumUnderlyingType().FullName), (EdmType) cspaceEnumType);
        return false;
      }
      if (primitiveType.PrimitiveTypeKind == cspaceEnumType.UnderlyingType.PrimitiveTypeKind)
        return true;
      this.LogLoadMessage(Strings.Validator_OSpace_Convention_NonMatchingUnderlyingTypes, (EdmType) cspaceEnumType);
      return false;
    }

    private bool EnumMembersMatch(Type enumType, EnumType cspaceEnumType)
    {
      Type enumUnderlyingType = enumType.GetEnumUnderlyingType();
      IEnumerator<EnumMember> enumerator1 = cspaceEnumType.Members.OrderBy<EnumMember, string>((Func<EnumMember, string>) (m => m.Name)).GetEnumerator();
      IEnumerator<string> enumerator2 = ((IEnumerable<string>) enumType.GetEnumNames()).OrderBy<string, string>((Func<string, string>) (n => n)).GetEnumerator();
      if (!enumerator1.MoveNext())
        return true;
      while (enumerator2.MoveNext())
      {
        if (enumerator1.Current.Name == enumerator2.Current && enumerator1.Current.Value.Equals(Convert.ChangeType(Enum.Parse(enumType, enumerator2.Current), enumUnderlyingType, (IFormatProvider) CultureInfo.InvariantCulture)) && !enumerator1.MoveNext())
          return true;
      }
      this.LogLoadMessage(Strings.Mapping_Enum_OCMapping_MemberMismatch((object) enumType.FullName, (object) enumerator1.Current.Name, enumerator1.Current.Value, (object) cspaceEnumType.FullName), (EdmType) cspaceEnumType);
      return false;
    }

    private bool TryCreateMembers(
      Type type,
      StructuralType cspaceType,
      StructuralType ospaceType,
      List<Action> referenceResolutionListForCurrentType)
    {
      IEnumerable<PropertyInfo> clrProperties = (cspaceType.BaseType == null ? type.GetRuntimeProperties() : type.GetDeclaredProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !p.IsStatic()));
      return this.TryFindAndCreatePrimitiveProperties(type, cspaceType, ospaceType, clrProperties) && this.TryFindAndCreateEnumProperties(type, cspaceType, ospaceType, clrProperties, referenceResolutionListForCurrentType) && (this.TryFindComplexProperties(type, cspaceType, ospaceType, clrProperties, referenceResolutionListForCurrentType) && this.TryFindNavigationProperties(type, cspaceType, ospaceType, clrProperties, referenceResolutionListForCurrentType));
    }

    private bool TryFindComplexProperties(
      Type type,
      StructuralType cspaceType,
      StructuralType ospaceType,
      IEnumerable<PropertyInfo> clrProperties,
      List<Action> referenceResolutionListForCurrentType)
    {
      List<KeyValuePair<EdmProperty, PropertyInfo>> keyValuePairList = new List<KeyValuePair<EdmProperty, PropertyInfo>>();
      foreach (EdmProperty edmProperty in cspaceType.GetDeclaredOnlyMembers<EdmProperty>().Where<EdmProperty>((Func<EdmProperty, bool>) (m => Helper.IsComplexType(m.TypeUsage.EdmType))))
      {
        EdmProperty cspaceProperty = edmProperty;
        PropertyInfo propertyInfo = clrProperties.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p => OSpaceTypeFactory.MemberMatchesByConvention(p, (EdmMember) cspaceProperty)));
        if (propertyInfo != (PropertyInfo) null)
        {
          keyValuePairList.Add(new KeyValuePair<EdmProperty, PropertyInfo>(cspaceProperty, propertyInfo));
        }
        else
        {
          this.LogLoadMessage(Strings.Validator_OSpace_Convention_MissingRequiredProperty((object) cspaceProperty.Name, (object) type.FullName), (EdmType) cspaceType);
          return false;
        }
      }
      foreach (KeyValuePair<EdmProperty, PropertyInfo> keyValuePair in keyValuePairList)
      {
        this.TrackClosure(keyValuePair.Value.PropertyType);
        StructuralType ot = ospaceType;
        EdmProperty cp = keyValuePair.Key;
        PropertyInfo clrp = keyValuePair.Value;
        referenceResolutionListForCurrentType.Add((Action) (() => this.CreateAndAddComplexType(type, ot, cp, clrp)));
      }
      return true;
    }

    private bool TryFindNavigationProperties(
      Type type,
      StructuralType cspaceType,
      StructuralType ospaceType,
      IEnumerable<PropertyInfo> clrProperties,
      List<Action> referenceResolutionListForCurrentType)
    {
      List<KeyValuePair<NavigationProperty, PropertyInfo>> keyValuePairList = new List<KeyValuePair<NavigationProperty, PropertyInfo>>();
      foreach (NavigationProperty declaredOnlyMember in cspaceType.GetDeclaredOnlyMembers<NavigationProperty>())
      {
        NavigationProperty cspaceProperty = declaredOnlyMember;
        PropertyInfo propertyInfo = clrProperties.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p => OSpaceTypeFactory.NonPrimitiveMemberMatchesByConvention(p, (EdmMember) cspaceProperty)));
        if (propertyInfo != (PropertyInfo) null)
        {
          bool flag = cspaceProperty.ToEndMember.RelationshipMultiplicity != RelationshipMultiplicity.Many;
          if (propertyInfo.CanRead && (!flag || propertyInfo.CanWriteExtended()))
            keyValuePairList.Add(new KeyValuePair<NavigationProperty, PropertyInfo>(cspaceProperty, propertyInfo));
        }
        else
        {
          this.LogLoadMessage(Strings.Validator_OSpace_Convention_MissingRequiredProperty((object) cspaceProperty.Name, (object) type.FullName), (EdmType) cspaceType);
          return false;
        }
      }
      foreach (KeyValuePair<NavigationProperty, PropertyInfo> keyValuePair in keyValuePairList)
      {
        this.TrackClosure(keyValuePair.Value.PropertyType);
        StructuralType ct = cspaceType;
        StructuralType ot = ospaceType;
        NavigationProperty cp = keyValuePair.Key;
        referenceResolutionListForCurrentType.Add((Action) (() => this.CreateAndAddNavigationProperty(ct, ot, cp)));
      }
      return true;
    }

    private EdmType ResolveBaseType(StructuralType baseCSpaceType, Type type)
    {
      EdmType edmType;
      if (!this.CspaceToOspace.TryGetValue((EdmType) baseCSpaceType, out edmType))
        this.LogError(Strings.Validator_OSpace_Convention_BaseTypeNotLoaded((object) type, (object) baseCSpaceType), (EdmType) baseCSpaceType);
      return edmType;
    }

    private bool TryFindAndCreatePrimitiveProperties(
      Type type,
      StructuralType cspaceType,
      StructuralType ospaceType,
      IEnumerable<PropertyInfo> clrProperties)
    {
      foreach (EdmProperty edmProperty in cspaceType.GetDeclaredOnlyMembers<EdmProperty>().Where<EdmProperty>((Func<EdmProperty, bool>) (p => Helper.IsPrimitiveType(p.TypeUsage.EdmType))))
      {
        EdmProperty cspaceProperty = edmProperty;
        PropertyInfo propertyInfo = clrProperties.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p => OSpaceTypeFactory.MemberMatchesByConvention(p, (EdmMember) cspaceProperty)));
        if (propertyInfo != (PropertyInfo) null)
        {
          PrimitiveType primitiveType;
          if (OSpaceTypeFactory.TryGetPrimitiveType(propertyInfo.PropertyType, out primitiveType))
          {
            if (propertyInfo.CanRead && propertyInfo.CanWriteExtended())
            {
              OSpaceTypeFactory.AddScalarMember(type, propertyInfo, ospaceType, cspaceProperty, (EdmType) primitiveType);
            }
            else
            {
              this.LogLoadMessage(Strings.Validator_OSpace_Convention_ScalarPropertyMissginGetterOrSetter((object) propertyInfo.Name, (object) type.FullName, (object) type.Assembly().FullName), (EdmType) cspaceType);
              return false;
            }
          }
          else
          {
            this.LogLoadMessage(Strings.Validator_OSpace_Convention_NonPrimitiveTypeProperty((object) propertyInfo.Name, (object) type.FullName, (object) propertyInfo.PropertyType.FullName), (EdmType) cspaceType);
            return false;
          }
        }
        else
        {
          this.LogLoadMessage(Strings.Validator_OSpace_Convention_MissingRequiredProperty((object) cspaceProperty.Name, (object) type.FullName), (EdmType) cspaceType);
          return false;
        }
      }
      return true;
    }

    protected static bool TryGetPrimitiveType(Type type, out PrimitiveType primitiveType)
    {
      ClrProviderManifest instance = ClrProviderManifest.Instance;
      Type clrType = Nullable.GetUnderlyingType(type);
      if ((object) clrType == null)
        clrType = type;
      ref PrimitiveType local = ref primitiveType;
      return instance.TryGetPrimitiveType(clrType, out local);
    }

    private bool TryFindAndCreateEnumProperties(
      Type type,
      StructuralType cspaceType,
      StructuralType ospaceType,
      IEnumerable<PropertyInfo> clrProperties,
      List<Action> referenceResolutionListForCurrentType)
    {
      List<KeyValuePair<EdmProperty, PropertyInfo>> keyValuePairList = new List<KeyValuePair<EdmProperty, PropertyInfo>>();
      foreach (EdmProperty edmProperty in cspaceType.GetDeclaredOnlyMembers<EdmProperty>().Where<EdmProperty>((Func<EdmProperty, bool>) (p => Helper.IsEnumType(p.TypeUsage.EdmType))))
      {
        EdmProperty cspaceProperty = edmProperty;
        PropertyInfo propertyInfo = clrProperties.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p => OSpaceTypeFactory.MemberMatchesByConvention(p, (EdmMember) cspaceProperty)));
        if (propertyInfo != (PropertyInfo) null)
        {
          keyValuePairList.Add(new KeyValuePair<EdmProperty, PropertyInfo>(cspaceProperty, propertyInfo));
        }
        else
        {
          this.LogLoadMessage(Strings.Validator_OSpace_Convention_MissingRequiredProperty((object) cspaceProperty.Name, (object) type.FullName), (EdmType) cspaceType);
          return false;
        }
      }
      foreach (KeyValuePair<EdmProperty, PropertyInfo> keyValuePair in keyValuePairList)
      {
        this.TrackClosure(keyValuePair.Value.PropertyType);
        StructuralType ot = ospaceType;
        EdmProperty cp = keyValuePair.Key;
        PropertyInfo clrp = keyValuePair.Value;
        referenceResolutionListForCurrentType.Add((Action) (() => this.CreateAndAddEnumProperty(type, ot, cp, clrp)));
      }
      return true;
    }

    private static bool MemberMatchesByConvention(PropertyInfo clrProperty, EdmMember cspaceMember)
    {
      return clrProperty.Name == cspaceMember.Name;
    }

    private void CreateAndAddComplexType(
      Type type,
      StructuralType ospaceType,
      EdmProperty cspaceProperty,
      PropertyInfo clrProperty)
    {
      EdmType edmType;
      if (this.CspaceToOspace.TryGetValue(cspaceProperty.TypeUsage.EdmType, out edmType))
      {
        EdmProperty edmProperty = new EdmProperty(cspaceProperty.Name, TypeUsage.Create(edmType, new FacetValues()
        {
          Nullable = (FacetValueContainer<bool?>) new bool?(false)
        }), clrProperty, type);
        ospaceType.AddMember((EdmMember) edmProperty);
      }
      else
        this.LogError(Strings.Validator_OSpace_Convention_MissingOSpaceType((object) cspaceProperty.TypeUsage.EdmType.FullName), cspaceProperty.TypeUsage.EdmType);
    }

    private static bool NonPrimitiveMemberMatchesByConvention(
      PropertyInfo clrProperty,
      EdmMember cspaceMember)
    {
      if (!clrProperty.PropertyType.IsValueType() && !clrProperty.PropertyType.IsAssignableFrom(typeof (string)))
        return clrProperty.Name == cspaceMember.Name;
      return false;
    }

    private void CreateAndAddNavigationProperty(
      StructuralType cspaceType,
      StructuralType ospaceType,
      NavigationProperty cspaceProperty)
    {
      EdmType edmType1;
      if (this.CspaceToOspace.TryGetValue((EdmType) cspaceProperty.RelationshipType, out edmType1))
      {
        EdmType edmType2 = (EdmType) null;
        if (Helper.IsCollectionType((GlobalItem) cspaceProperty.TypeUsage.EdmType))
        {
          EdmType edmType3;
          if (this.CspaceToOspace.TryGetValue(((CollectionType) cspaceProperty.TypeUsage.EdmType).TypeUsage.EdmType, out edmType3))
            edmType2 = (EdmType) edmType3.GetCollectionType();
        }
        else
        {
          EdmType edmType3;
          if (this.CspaceToOspace.TryGetValue(cspaceProperty.TypeUsage.EdmType, out edmType3))
            edmType2 = edmType3;
        }
        NavigationProperty navigationProperty = new NavigationProperty(cspaceProperty.Name, TypeUsage.Create(edmType2));
        RelationshipType relationshipType = (RelationshipType) edmType1;
        navigationProperty.RelationshipType = relationshipType;
        navigationProperty.ToEndMember = (RelationshipEndMember) relationshipType.Members.First<EdmMember>((Func<EdmMember, bool>) (e => e.Name == cspaceProperty.ToEndMember.Name));
        navigationProperty.FromEndMember = (RelationshipEndMember) relationshipType.Members.First<EdmMember>((Func<EdmMember, bool>) (e => e.Name == cspaceProperty.FromEndMember.Name));
        ospaceType.AddMember((EdmMember) navigationProperty);
      }
      else
      {
        EntityTypeBase entityTypeBase = cspaceProperty.RelationshipType.RelationshipEndMembers.Select<RelationshipEndMember, EntityTypeBase>((Func<RelationshipEndMember, EntityTypeBase>) (e => ((RefType) e.TypeUsage.EdmType).ElementType)).First<EntityTypeBase>((Func<EntityTypeBase, bool>) (e => e != cspaceType));
        this.LogError(Strings.Validator_OSpace_Convention_RelationshipNotLoaded((object) cspaceProperty.RelationshipType.FullName, (object) entityTypeBase.FullName), (EdmType) entityTypeBase);
      }
    }

    private void CreateAndAddEnumProperty(
      Type type,
      StructuralType ospaceType,
      EdmProperty cspaceProperty,
      PropertyInfo clrProperty)
    {
      EdmType propertyType;
      if (this.CspaceToOspace.TryGetValue(cspaceProperty.TypeUsage.EdmType, out propertyType))
      {
        if (clrProperty.CanRead && clrProperty.CanWriteExtended())
          OSpaceTypeFactory.AddScalarMember(type, clrProperty, ospaceType, cspaceProperty, propertyType);
        else
          this.LogError(Strings.Validator_OSpace_Convention_ScalarPropertyMissginGetterOrSetter((object) clrProperty.Name, (object) type.FullName, (object) type.Assembly().FullName), cspaceProperty.TypeUsage.EdmType);
      }
      else
        this.LogError(Strings.Validator_OSpace_Convention_MissingOSpaceType((object) cspaceProperty.TypeUsage.EdmType.FullName), cspaceProperty.TypeUsage.EdmType);
    }

    private static void AddScalarMember(
      Type type,
      PropertyInfo clrProperty,
      StructuralType ospaceType,
      EdmProperty cspaceProperty,
      EdmType propertyType)
    {
      StructuralType declaringType = cspaceProperty.DeclaringType;
      bool flag1 = Helper.IsEntityType((EdmType) declaringType) && ((IEnumerable<string>) ((EntityTypeBase) declaringType).KeyMemberNames).Contains<string>(clrProperty.Name);
      bool flag2 = !flag1 && (!clrProperty.PropertyType.IsValueType() || Nullable.GetUnderlyingType(clrProperty.PropertyType) != (Type) null);
      EdmProperty edmProperty = new EdmProperty(cspaceProperty.Name, TypeUsage.Create(propertyType, new FacetValues()
      {
        Nullable = (FacetValueContainer<bool?>) new bool?(flag2)
      }), clrProperty, type);
      if (flag1)
        ((EntityTypeBase) ospaceType).AddKeyMember((EdmMember) edmProperty);
      else
        ospaceType.AddMember((EdmMember) edmProperty);
    }

    public virtual void CreateRelationships(EdmItemCollection edmItemCollection)
    {
      foreach (AssociationType associationType1 in edmItemCollection.GetItems<AssociationType>())
      {
        if (!this.CspaceToOspace.ContainsKey((EdmType) associationType1))
        {
          EdmType[] edmTypeArray = new EdmType[2];
          if (this.CspaceToOspace.TryGetValue((EdmType) OSpaceTypeFactory.GetRelationshipEndType(associationType1.RelationshipEndMembers[0]), out edmTypeArray[0]) && this.CspaceToOspace.TryGetValue((EdmType) OSpaceTypeFactory.GetRelationshipEndType(associationType1.RelationshipEndMembers[1]), out edmTypeArray[1]))
          {
            AssociationType associationType2 = new AssociationType(associationType1.Name, associationType1.NamespaceName, associationType1.IsForeignKey, DataSpace.OSpace);
            for (int index = 0; index < associationType1.RelationshipEndMembers.Count; ++index)
            {
              EntityType entityType = (EntityType) edmTypeArray[index];
              RelationshipEndMember relationshipEndMember = associationType1.RelationshipEndMembers[index];
              associationType2.AddKeyMember((EdmMember) new AssociationEndMember(relationshipEndMember.Name, entityType.GetReferenceType(), relationshipEndMember.RelationshipMultiplicity));
            }
            this.AddToTypesInAssembly((EdmType) associationType2);
            this.LoadedTypes.Add(associationType2.FullName, (EdmType) associationType2);
            this.CspaceToOspace.Add((EdmType) associationType1, (EdmType) associationType2);
          }
        }
      }
    }

    private static StructuralType GetRelationshipEndType(
      RelationshipEndMember relationshipEndMember)
    {
      return (StructuralType) ((RefType) relationshipEndMember.TypeUsage.EdmType).ElementType;
    }
  }
}
