// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.PropertyEntryMetadata
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Internal
{
  internal class PropertyEntryMetadata : MemberEntryMetadata
  {
    private readonly bool _isMapped;
    private readonly bool _isComplex;

    public PropertyEntryMetadata(
      Type declaringType,
      Type propertyType,
      string propertyName,
      bool isMapped,
      bool isComplex)
      : base(declaringType, propertyType, propertyName)
    {
      this._isMapped = isMapped;
      this._isComplex = isComplex;
    }

    public static PropertyEntryMetadata ValidateNameAndGetMetadata(
      InternalContext internalContext,
      Type declaringType,
      Type requestedType,
      string propertyName)
    {
      Type type;
      DbHelpers.GetPropertyTypes(declaringType).TryGetValue(propertyName, out type);
      MetadataWorkspace metadataWorkspace = internalContext.ObjectContext.MetadataWorkspace;
      StructuralType structuralType = metadataWorkspace.GetItem<StructuralType>(declaringType.FullNameWithNesting(), DataSpace.OSpace);
      bool isMapped = false;
      bool isComplex = false;
      EdmMember edmMember;
      structuralType.Members.TryGetValue(propertyName, false, out edmMember);
      if (edmMember != null)
      {
        EdmProperty edmProperty = edmMember as EdmProperty;
        if (edmProperty == null)
          return (PropertyEntryMetadata) null;
        if (type == (Type) null)
        {
          PrimitiveType edmType = edmProperty.TypeUsage.EdmType as PrimitiveType;
          type = edmType == null ? ((ObjectItemCollection) metadataWorkspace.GetItemCollection(DataSpace.OSpace)).GetClrType((StructuralType) edmProperty.TypeUsage.EdmType) : edmType.ClrEquivalentType;
        }
        isMapped = true;
        isComplex = edmProperty.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType;
      }
      else
      {
        IDictionary<string, Func<object, object>> propertyGetters = DbHelpers.GetPropertyGetters(declaringType);
        IDictionary<string, Action<object, object>> propertySetters = DbHelpers.GetPropertySetters(declaringType);
        if (!propertyGetters.ContainsKey(propertyName) && !propertySetters.ContainsKey(propertyName))
          return (PropertyEntryMetadata) null;
      }
      if (!requestedType.IsAssignableFrom(type))
        throw Error.DbEntityEntry_WrongGenericForProp((object) propertyName, (object) declaringType.Name, (object) requestedType.Name, (object) type.Name);
      return new PropertyEntryMetadata(declaringType, type, propertyName, isMapped, isComplex);
    }

    public override InternalMemberEntry CreateMemberEntry(
      InternalEntityEntry internalEntityEntry,
      InternalPropertyEntry parentPropertyEntry)
    {
      if (parentPropertyEntry != null)
        return (InternalMemberEntry) new InternalNestedPropertyEntry(parentPropertyEntry, this);
      return (InternalMemberEntry) new InternalEntityPropertyEntry(internalEntityEntry, this);
    }

    public bool IsComplex
    {
      get
      {
        return this._isComplex;
      }
    }

    public override MemberEntryType MemberEntryType
    {
      get
      {
        return !this._isComplex ? MemberEntryType.ScalarProperty : MemberEntryType.ComplexProperty;
      }
    }

    public bool IsMapped
    {
      get
      {
        return this._isMapped;
      }
    }

    public override Type MemberType
    {
      get
      {
        return this.ElementType;
      }
    }
  }
}
