// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.DefaultObjectMappingItemCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Mapping
{
  internal class DefaultObjectMappingItemCollection : MappingItemCollection
  {
    private Dictionary<string, int> _clrTypeIndexes = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.Ordinal);
    private Dictionary<string, int> _edmTypeIndexes = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.Ordinal);
    private readonly object _lock = new object();
    private readonly ObjectItemCollection _objectCollection;
    private readonly EdmItemCollection _edmCollection;

    public DefaultObjectMappingItemCollection(
      EdmItemCollection edmCollection,
      ObjectItemCollection objectCollection)
      : base(DataSpace.OCSpace)
    {
      this._edmCollection = edmCollection;
      this._objectCollection = objectCollection;
      foreach (PrimitiveType primitiveType in this._edmCollection.GetPrimitiveTypes())
        this.AddInternalMapping(new ObjectTypeMapping((EdmType) this._objectCollection.GetMappedPrimitiveType(primitiveType.PrimitiveTypeKind), (EdmType) primitiveType), this._clrTypeIndexes, this._edmTypeIndexes);
    }

    public ObjectItemCollection ObjectItemCollection
    {
      get
      {
        return this._objectCollection;
      }
    }

    public EdmItemCollection EdmItemCollection
    {
      get
      {
        return this._edmCollection;
      }
    }

    internal override MappingBase GetMap(
      string identity,
      DataSpace typeSpace,
      bool ignoreCase)
    {
      MappingBase map;
      if (!this.TryGetMap(identity, typeSpace, ignoreCase, out map))
        throw new InvalidOperationException(Strings.Mapping_Object_InvalidType((object) identity));
      return map;
    }

    internal override bool TryGetMap(
      string identity,
      DataSpace typeSpace,
      bool ignoreCase,
      out MappingBase map)
    {
      EdmType edmType1 = (EdmType) null;
      EdmType edmType2 = (EdmType) null;
      switch (typeSpace)
      {
        case DataSpace.OSpace:
          if (ignoreCase)
          {
            if (!this._objectCollection.TryGetItem<EdmType>(identity, true, out edmType2))
            {
              map = (MappingBase) null;
              return false;
            }
            identity = edmType2.Identity;
          }
          int index1;
          if (this._clrTypeIndexes.TryGetValue(identity, out index1))
          {
            map = (MappingBase) this[index1];
            return true;
          }
          if (edmType2 != null || this._objectCollection.TryGetItem<EdmType>(identity, ignoreCase, out edmType2))
          {
            this._edmCollection.TryGetItem<EdmType>(ObjectItemCollection.TryGetMappingCSpaceTypeIdentity(edmType2), out edmType1);
            break;
          }
          break;
        case DataSpace.CSpace:
          if (ignoreCase)
          {
            if (!this._edmCollection.TryGetItem<EdmType>(identity, true, out edmType1))
            {
              map = (MappingBase) null;
              return false;
            }
            identity = edmType1.Identity;
          }
          int index2;
          if (this._edmTypeIndexes.TryGetValue(identity, out index2))
          {
            map = (MappingBase) this[index2];
            return true;
          }
          if (edmType1 != null || this._edmCollection.TryGetItem<EdmType>(identity, ignoreCase, out edmType1))
          {
            this._objectCollection.TryGetOSpaceType(edmType1, out edmType2);
            break;
          }
          break;
      }
      if (edmType2 == null || edmType1 == null)
      {
        map = (MappingBase) null;
        return false;
      }
      map = this.GetDefaultMapping(edmType1, edmType2);
      return true;
    }

    internal override MappingBase GetMap(string identity, DataSpace typeSpace)
    {
      return this.GetMap(identity, typeSpace, false);
    }

    internal override bool TryGetMap(string identity, DataSpace typeSpace, out MappingBase map)
    {
      return this.TryGetMap(identity, typeSpace, false, out map);
    }

    internal override MappingBase GetMap(GlobalItem item)
    {
      MappingBase map;
      if (!this.TryGetMap(item, out map))
        throw new InvalidOperationException(Strings.Mapping_Object_InvalidType((object) item.Identity));
      return map;
    }

    internal override bool TryGetMap(GlobalItem item, out MappingBase map)
    {
      if (item == null)
      {
        map = (MappingBase) null;
        return false;
      }
      DataSpace dataSpace = item.DataSpace;
      EdmType edmType = item as EdmType;
      if (edmType == null || !Helper.IsTransientType(edmType))
        return this.TryGetMap(item.Identity, dataSpace, out map);
      map = this.GetOCMapForTransientType(edmType, dataSpace);
      return map != null;
    }

    private MappingBase GetDefaultMapping(EdmType cdmType, EdmType clrType)
    {
      return (MappingBase) DefaultObjectMappingItemCollection.LoadObjectMapping(cdmType, clrType, this);
    }

    private MappingBase GetOCMapForTransientType(EdmType edmType, DataSpace typeSpace)
    {
      EdmType clrType = (EdmType) null;
      EdmType cdmType = (EdmType) null;
      int index1 = -1;
      if (typeSpace != DataSpace.OSpace)
      {
        if (this._edmTypeIndexes.TryGetValue(edmType.Identity, out index1))
          return (MappingBase) this[index1];
        cdmType = edmType;
        clrType = this.ConvertCSpaceToOSpaceType(edmType);
      }
      else if (typeSpace == DataSpace.OSpace)
      {
        if (this._clrTypeIndexes.TryGetValue(edmType.Identity, out index1))
          return (MappingBase) this[index1];
        clrType = edmType;
        cdmType = this.ConvertOSpaceToCSpaceType(clrType);
      }
      ObjectTypeMapping objectMap = new ObjectTypeMapping(clrType, cdmType);
      if (BuiltInTypeKind.RowType == edmType.BuiltInTypeKind)
      {
        RowType rowType1 = (RowType) clrType;
        RowType rowType2 = (RowType) cdmType;
        for (int index2 = 0; index2 < rowType1.Properties.Count; ++index2)
          objectMap.AddMemberMap((ObjectMemberMapping) new ObjectPropertyMapping(rowType2.Properties[index2], rowType1.Properties[index2]));
      }
      if (!this._edmTypeIndexes.ContainsKey(cdmType.Identity) && !this._clrTypeIndexes.ContainsKey(clrType.Identity))
      {
        lock (this._lock)
        {
          Dictionary<string, int> clrTypeIndexes = new Dictionary<string, int>((IDictionary<string, int>) this._clrTypeIndexes);
          Dictionary<string, int> edmTypeIndexes = new Dictionary<string, int>((IDictionary<string, int>) this._edmTypeIndexes);
          objectMap = this.AddInternalMapping(objectMap, clrTypeIndexes, edmTypeIndexes);
          this._clrTypeIndexes = clrTypeIndexes;
          this._edmTypeIndexes = edmTypeIndexes;
        }
      }
      return (MappingBase) objectMap;
    }

    private EdmType ConvertCSpaceToOSpaceType(EdmType cdmType)
    {
      EdmType edmType;
      if (Helper.IsCollectionType((GlobalItem) cdmType))
        edmType = (EdmType) new CollectionType(this.ConvertCSpaceToOSpaceType(((CollectionType) cdmType).TypeUsage.EdmType));
      else if (Helper.IsRowType((GlobalItem) cdmType))
      {
        List<EdmProperty> edmPropertyList = new List<EdmProperty>();
        RowType rowType = (RowType) cdmType;
        foreach (EdmProperty property in rowType.Properties)
        {
          EdmType ospaceType = this.ConvertCSpaceToOSpaceType(property.TypeUsage.EdmType);
          EdmProperty edmProperty = new EdmProperty(property.Name, TypeUsage.Create(ospaceType));
          edmPropertyList.Add(edmProperty);
        }
        edmType = (EdmType) new RowType((IEnumerable<EdmProperty>) edmPropertyList, rowType.InitializerMetadata);
      }
      else
        edmType = !Helper.IsRefType((GlobalItem) cdmType) ? (!Helper.IsPrimitiveType(cdmType) ? ((ObjectTypeMapping) this.GetMap((GlobalItem) cdmType)).ClrType : (EdmType) this._objectCollection.GetMappedPrimitiveType(((PrimitiveType) cdmType).PrimitiveTypeKind)) : (EdmType) new RefType((EntityType) this.ConvertCSpaceToOSpaceType((EdmType) ((RefType) cdmType).ElementType));
      return edmType;
    }

    private EdmType ConvertOSpaceToCSpaceType(EdmType clrType)
    {
      EdmType edmType;
      if (Helper.IsCollectionType((GlobalItem) clrType))
        edmType = (EdmType) new CollectionType(this.ConvertOSpaceToCSpaceType(((CollectionType) clrType).TypeUsage.EdmType));
      else if (Helper.IsRowType((GlobalItem) clrType))
      {
        List<EdmProperty> edmPropertyList = new List<EdmProperty>();
        RowType rowType = (RowType) clrType;
        foreach (EdmProperty property in rowType.Properties)
        {
          EdmType cspaceType = this.ConvertOSpaceToCSpaceType(property.TypeUsage.EdmType);
          EdmProperty edmProperty = new EdmProperty(property.Name, TypeUsage.Create(cspaceType));
          edmPropertyList.Add(edmProperty);
        }
        edmType = (EdmType) new RowType((IEnumerable<EdmProperty>) edmPropertyList, rowType.InitializerMetadata);
      }
      else
        edmType = !Helper.IsRefType((GlobalItem) clrType) ? ((ObjectTypeMapping) this.GetMap((GlobalItem) clrType)).EdmType : (EdmType) new RefType((EntityType) this.ConvertOSpaceToCSpaceType((EdmType) ((RefType) clrType).ElementType));
      return edmType;
    }

    private void AddInternalMappings(IEnumerable<ObjectTypeMapping> typeMappings)
    {
      lock (this._lock)
      {
        Dictionary<string, int> clrTypeIndexes = new Dictionary<string, int>((IDictionary<string, int>) this._clrTypeIndexes);
        Dictionary<string, int> edmTypeIndexes = new Dictionary<string, int>((IDictionary<string, int>) this._edmTypeIndexes);
        foreach (ObjectTypeMapping typeMapping in typeMappings)
          this.AddInternalMapping(typeMapping, clrTypeIndexes, edmTypeIndexes);
        this._clrTypeIndexes = clrTypeIndexes;
        this._edmTypeIndexes = edmTypeIndexes;
      }
    }

    private ObjectTypeMapping AddInternalMapping(
      ObjectTypeMapping objectMap,
      Dictionary<string, int> clrTypeIndexes,
      Dictionary<string, int> edmTypeIndexes)
    {
      if (this.Source.ContainsIdentity(objectMap.Identity))
        return (ObjectTypeMapping) this.Source[objectMap.Identity];
      objectMap.DataSpace = DataSpace.OCSpace;
      int count = this.Count;
      this.AddInternal((GlobalItem) objectMap);
      string identity1 = objectMap.ClrType.Identity;
      if (!clrTypeIndexes.ContainsKey(identity1))
        clrTypeIndexes.Add(identity1, count);
      string identity2 = objectMap.EdmType.Identity;
      if (!edmTypeIndexes.ContainsKey(identity2))
        edmTypeIndexes.Add(identity2, count);
      return objectMap;
    }

    internal static ObjectTypeMapping LoadObjectMapping(
      EdmType cdmType,
      EdmType objectType,
      DefaultObjectMappingItemCollection ocItemCollection)
    {
      Dictionary<string, ObjectTypeMapping> typeMappings = new Dictionary<string, ObjectTypeMapping>((IEqualityComparer<string>) StringComparer.Ordinal);
      ObjectTypeMapping objectTypeMapping = DefaultObjectMappingItemCollection.LoadObjectMapping(cdmType, objectType, ocItemCollection, typeMappings);
      ocItemCollection?.AddInternalMappings((IEnumerable<ObjectTypeMapping>) typeMappings.Values);
      return objectTypeMapping;
    }

    private static ObjectTypeMapping LoadObjectMapping(
      EdmType edmType,
      EdmType objectType,
      DefaultObjectMappingItemCollection ocItemCollection,
      Dictionary<string, ObjectTypeMapping> typeMappings)
    {
      if (Helper.IsEnumType(edmType) ^ Helper.IsEnumType(objectType))
        throw new MappingException(Strings.Mapping_EnumTypeMappingToNonEnumType((object) edmType.FullName, (object) objectType.FullName));
      if (edmType.Abstract != objectType.Abstract)
        throw new MappingException(Strings.Mapping_AbstractTypeMappingToNonAbstractType((object) edmType.FullName, (object) objectType.FullName));
      ObjectTypeMapping objectMapping = new ObjectTypeMapping(objectType, edmType);
      typeMappings.Add(edmType.FullName, objectMapping);
      if (Helper.IsEntityType(edmType) || Helper.IsComplexType(edmType))
        DefaultObjectMappingItemCollection.LoadEntityTypeOrComplexTypeMapping(objectMapping, edmType, objectType, ocItemCollection, typeMappings);
      else if (Helper.IsEnumType(edmType))
        DefaultObjectMappingItemCollection.ValidateEnumTypeMapping((EnumType) edmType, (EnumType) objectType);
      else
        DefaultObjectMappingItemCollection.LoadAssociationTypeMapping(objectMapping, edmType, objectType, ocItemCollection, typeMappings);
      return objectMapping;
    }

    private static EdmMember GetObjectMember(
      EdmMember edmMember,
      StructuralType objectType)
    {
      EdmMember edmMember1;
      if (!objectType.Members.TryGetValue(edmMember.Name, false, out edmMember1))
        throw new MappingException(Strings.Mapping_Default_OCMapping_Clr_Member((object) edmMember.Name, (object) edmMember.DeclaringType.FullName, (object) objectType.FullName));
      return edmMember1;
    }

    private static void ValidateMembersMatch(EdmMember edmMember, EdmMember objectMember)
    {
      if (edmMember.BuiltInTypeKind != objectMember.BuiltInTypeKind)
        throw new MappingException(Strings.Mapping_Default_OCMapping_MemberKind_Mismatch((object) edmMember.Name, (object) edmMember.DeclaringType.FullName, (object) edmMember.BuiltInTypeKind, (object) objectMember.Name, (object) objectMember.DeclaringType.FullName, (object) objectMember.BuiltInTypeKind));
      if (edmMember.TypeUsage.EdmType.BuiltInTypeKind != objectMember.TypeUsage.EdmType.BuiltInTypeKind)
        throw Error.Mapping_Default_OCMapping_Member_Type_Mismatch((object) edmMember.TypeUsage.EdmType.Name, (object) edmMember.TypeUsage.EdmType.BuiltInTypeKind, (object) edmMember.Name, (object) edmMember.DeclaringType.FullName, (object) objectMember.TypeUsage.EdmType.Name, (object) objectMember.TypeUsage.EdmType.BuiltInTypeKind, (object) objectMember.Name, (object) objectMember.DeclaringType.FullName);
      if (Helper.IsPrimitiveType(edmMember.TypeUsage.EdmType))
      {
        if (Helper.GetSpatialNormalizedPrimitiveType(edmMember.TypeUsage.EdmType).PrimitiveTypeKind != ((PrimitiveType) objectMember.TypeUsage.EdmType).PrimitiveTypeKind)
          throw new MappingException(Strings.Mapping_Default_OCMapping_Invalid_MemberType((object) edmMember.TypeUsage.EdmType.FullName, (object) edmMember.Name, (object) edmMember.DeclaringType.FullName, (object) objectMember.TypeUsage.EdmType.FullName, (object) objectMember.Name, (object) objectMember.DeclaringType.FullName));
      }
      else if (Helper.IsEnumType(edmMember.TypeUsage.EdmType))
      {
        DefaultObjectMappingItemCollection.ValidateEnumTypeMapping((EnumType) edmMember.TypeUsage.EdmType, (EnumType) objectMember.TypeUsage.EdmType);
      }
      else
      {
        EdmType edmType1;
        EdmType edmType2;
        if (edmMember.BuiltInTypeKind == BuiltInTypeKind.AssociationEndMember)
        {
          edmType1 = (EdmType) ((RefType) edmMember.TypeUsage.EdmType).ElementType;
          edmType2 = (EdmType) ((RefType) objectMember.TypeUsage.EdmType).ElementType;
        }
        else if (BuiltInTypeKind.NavigationProperty == edmMember.BuiltInTypeKind && Helper.IsCollectionType((GlobalItem) edmMember.TypeUsage.EdmType))
        {
          edmType1 = ((CollectionType) edmMember.TypeUsage.EdmType).TypeUsage.EdmType;
          edmType2 = ((CollectionType) objectMember.TypeUsage.EdmType).TypeUsage.EdmType;
        }
        else
        {
          edmType1 = edmMember.TypeUsage.EdmType;
          edmType2 = objectMember.TypeUsage.EdmType;
        }
        if (edmType1.Identity != ObjectItemCollection.TryGetMappingCSpaceTypeIdentity(edmType2))
          throw new MappingException(Strings.Mapping_Default_OCMapping_Invalid_MemberType((object) edmMember.TypeUsage.EdmType.FullName, (object) edmMember.Name, (object) edmMember.DeclaringType.FullName, (object) objectMember.TypeUsage.EdmType.FullName, (object) objectMember.Name, (object) objectMember.DeclaringType.FullName));
      }
    }

    private static ObjectPropertyMapping LoadScalarPropertyMapping(
      EdmProperty edmProperty,
      EdmProperty objectProperty)
    {
      return new ObjectPropertyMapping(edmProperty, objectProperty);
    }

    private static void LoadEntityTypeOrComplexTypeMapping(
      ObjectTypeMapping objectMapping,
      EdmType edmType,
      EdmType objectType,
      DefaultObjectMappingItemCollection ocItemCollection,
      Dictionary<string, ObjectTypeMapping> typeMappings)
    {
      StructuralType cdmStructuralType = (StructuralType) edmType;
      StructuralType structuralType = (StructuralType) objectType;
      DefaultObjectMappingItemCollection.ValidateAllMembersAreMapped(cdmStructuralType, structuralType);
      foreach (EdmMember member in cdmStructuralType.Members)
      {
        EdmMember objectMember = DefaultObjectMappingItemCollection.GetObjectMember(member, structuralType);
        DefaultObjectMappingItemCollection.ValidateMembersMatch(member, objectMember);
        if (Helper.IsEdmProperty(member))
        {
          EdmProperty edmProperty1 = (EdmProperty) member;
          EdmProperty edmProperty2 = (EdmProperty) objectMember;
          if (Helper.IsComplexType(member.TypeUsage.EdmType))
            objectMapping.AddMemberMap((ObjectMemberMapping) DefaultObjectMappingItemCollection.LoadComplexMemberMapping(edmProperty1, edmProperty2, ocItemCollection, typeMappings));
          else
            objectMapping.AddMemberMap((ObjectMemberMapping) DefaultObjectMappingItemCollection.LoadScalarPropertyMapping(edmProperty1, edmProperty2));
        }
        else
        {
          NavigationProperty edmNavigationProperty = (NavigationProperty) member;
          NavigationProperty clrNavigationProperty = (NavigationProperty) objectMember;
          DefaultObjectMappingItemCollection.LoadTypeMapping((EdmType) edmNavigationProperty.RelationshipType, (EdmType) clrNavigationProperty.RelationshipType, ocItemCollection, typeMappings);
          objectMapping.AddMemberMap((ObjectMemberMapping) new ObjectNavigationPropertyMapping(edmNavigationProperty, clrNavigationProperty));
        }
      }
    }

    private static void ValidateAllMembersAreMapped(
      StructuralType cdmStructuralType,
      StructuralType objectStructuralType)
    {
      if (cdmStructuralType.Members.Count != objectStructuralType.Members.Count)
        throw new MappingException(Strings.Mapping_Default_OCMapping_Member_Count_Mismatch((object) cdmStructuralType.FullName, (object) objectStructuralType.FullName));
      foreach (EdmMember member in objectStructuralType.Members)
      {
        if (!cdmStructuralType.Members.Contains(member.Identity))
          throw new MappingException(Strings.Mapping_Default_OCMapping_Clr_Member2((object) member.Name, (object) objectStructuralType.FullName, (object) cdmStructuralType.FullName));
      }
    }

    private static void ValidateEnumTypeMapping(EnumType edmEnumType, EnumType objectEnumType)
    {
      if (edmEnumType.UnderlyingType.PrimitiveTypeKind != objectEnumType.UnderlyingType.PrimitiveTypeKind)
        throw new MappingException(Strings.Mapping_Enum_OCMapping_UnderlyingTypesMismatch((object) edmEnumType.UnderlyingType.Name, (object) edmEnumType.FullName, (object) objectEnumType.UnderlyingType.Name, (object) objectEnumType.FullName));
      IEnumerator<EnumMember> enumerator1 = edmEnumType.Members.OrderBy<EnumMember, long>((Func<EnumMember, long>) (m => Convert.ToInt64(m.Value, (IFormatProvider) CultureInfo.InvariantCulture))).ThenBy<EnumMember, string>((Func<EnumMember, string>) (m => m.Name)).GetEnumerator();
      IEnumerator<EnumMember> enumerator2 = objectEnumType.Members.OrderBy<EnumMember, long>((Func<EnumMember, long>) (m => Convert.ToInt64(m.Value, (IFormatProvider) CultureInfo.InvariantCulture))).ThenBy<EnumMember, string>((Func<EnumMember, string>) (m => m.Name)).GetEnumerator();
      if (enumerator1.MoveNext())
      {
        while (enumerator2.MoveNext())
        {
          if (enumerator1.Current.Name == enumerator2.Current.Name && enumerator1.Current.Value.Equals(enumerator2.Current.Value) && !enumerator1.MoveNext())
            return;
        }
        throw new MappingException(Strings.Mapping_Enum_OCMapping_MemberMismatch((object) objectEnumType.FullName, (object) enumerator1.Current.Name, enumerator1.Current.Value, (object) edmEnumType.FullName));
      }
    }

    private static void LoadAssociationTypeMapping(
      ObjectTypeMapping objectMapping,
      EdmType edmType,
      EdmType objectType,
      DefaultObjectMappingItemCollection ocItemCollection,
      Dictionary<string, ObjectTypeMapping> typeMappings)
    {
      AssociationType associationType1 = (AssociationType) edmType;
      AssociationType associationType2 = (AssociationType) objectType;
      foreach (AssociationEndMember associationEndMember in associationType1.AssociationEndMembers)
      {
        AssociationEndMember objectMember = (AssociationEndMember) DefaultObjectMappingItemCollection.GetObjectMember((EdmMember) associationEndMember, (StructuralType) associationType2);
        DefaultObjectMappingItemCollection.ValidateMembersMatch((EdmMember) associationEndMember, (EdmMember) objectMember);
        if (associationEndMember.RelationshipMultiplicity != objectMember.RelationshipMultiplicity)
          throw new MappingException(Strings.Mapping_Default_OCMapping_MultiplicityMismatch((object) associationEndMember.RelationshipMultiplicity, (object) associationEndMember.Name, (object) associationType1.FullName, (object) objectMember.RelationshipMultiplicity, (object) objectMember.Name, (object) associationType2.FullName));
        DefaultObjectMappingItemCollection.LoadTypeMapping((EdmType) ((RefType) associationEndMember.TypeUsage.EdmType).ElementType, (EdmType) ((RefType) objectMember.TypeUsage.EdmType).ElementType, ocItemCollection, typeMappings);
        objectMapping.AddMemberMap((ObjectMemberMapping) new ObjectAssociationEndMapping(associationEndMember, objectMember));
      }
    }

    private static ObjectComplexPropertyMapping LoadComplexMemberMapping(
      EdmProperty containingEdmMember,
      EdmProperty containingClrMember,
      DefaultObjectMappingItemCollection ocItemCollection,
      Dictionary<string, ObjectTypeMapping> typeMappings)
    {
      DefaultObjectMappingItemCollection.LoadTypeMapping(containingEdmMember.TypeUsage.EdmType, containingClrMember.TypeUsage.EdmType, ocItemCollection, typeMappings);
      return new ObjectComplexPropertyMapping(containingEdmMember, containingClrMember);
    }

    private static ObjectTypeMapping LoadTypeMapping(
      EdmType edmType,
      EdmType objectType,
      DefaultObjectMappingItemCollection ocItemCollection,
      Dictionary<string, ObjectTypeMapping> typeMappings)
    {
      ObjectTypeMapping objectTypeMapping;
      if (typeMappings.TryGetValue(edmType.FullName, out objectTypeMapping))
        return objectTypeMapping;
      ObjectTypeMapping map;
      if (ocItemCollection != null && ocItemCollection.ContainsMap((GlobalItem) edmType, out map))
        return map;
      return DefaultObjectMappingItemCollection.LoadObjectMapping(edmType, objectType, ocItemCollection, typeMappings);
    }

    private bool ContainsMap(GlobalItem cspaceItem, out ObjectTypeMapping map)
    {
      int index;
      if (this._edmTypeIndexes.TryGetValue(cspaceItem.Identity, out index))
      {
        map = (ObjectTypeMapping) this[index];
        return true;
      }
      map = (ObjectTypeMapping) null;
      return false;
    }
  }
}
