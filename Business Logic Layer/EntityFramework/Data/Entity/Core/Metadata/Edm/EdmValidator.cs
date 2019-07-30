// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class EdmValidator
  {
    internal bool SkipReadOnlyItems { get; set; }

    public void Validate<T>(IEnumerable<T> items, List<EdmItemError> ospaceErrors) where T : EdmType
    {
      Check.NotNull<IEnumerable<T>>(items, nameof (items));
      Check.NotNull<IEnumerable<T>>(items, nameof (items));
      HashSet<MetadataItem> validatedItems = new HashSet<MetadataItem>();
      foreach (T obj in items)
        this.InternalValidate((MetadataItem) obj, ospaceErrors, validatedItems);
    }

    protected virtual void OnValidationError(ValidationErrorEventArgs e)
    {
    }

    private void AddError(List<EdmItemError> errors, EdmItemError newError)
    {
      ValidationErrorEventArgs e = new ValidationErrorEventArgs(newError);
      this.OnValidationError(e);
      errors.Add(e.ValidationError);
    }

    protected virtual IEnumerable<EdmItemError> CustomValidate(
      MetadataItem item)
    {
      return (IEnumerable<EdmItemError>) null;
    }

    private void InternalValidate(
      MetadataItem item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      if (item.IsReadOnly && this.SkipReadOnlyItems || validatedItems.Contains(item))
        return;
      validatedItems.Add(item);
      if (string.IsNullOrEmpty(item.Identity))
        this.AddError(errors, new EdmItemError(Strings.Validator_EmptyIdentity));
      switch (item.BuiltInTypeKind)
      {
        case BuiltInTypeKind.CollectionType:
          this.ValidateCollectionType((CollectionType) item, errors, validatedItems);
          break;
        case BuiltInTypeKind.ComplexType:
          this.ValidateComplexType((ComplexType) item, errors, validatedItems);
          break;
        case BuiltInTypeKind.EntityType:
          this.ValidateEntityType((EntityType) item, errors, validatedItems);
          break;
        case BuiltInTypeKind.Facet:
          this.ValidateFacet((Facet) item, errors, validatedItems);
          break;
        case BuiltInTypeKind.MetadataProperty:
          this.ValidateMetadataProperty((MetadataProperty) item, errors, validatedItems);
          break;
        case BuiltInTypeKind.NavigationProperty:
          this.ValidateNavigationProperty((NavigationProperty) item, errors, validatedItems);
          break;
        case BuiltInTypeKind.PrimitiveType:
          this.ValidatePrimitiveType((PrimitiveType) item, errors, validatedItems);
          break;
        case BuiltInTypeKind.EdmProperty:
          this.ValidateEdmProperty((EdmProperty) item, errors, validatedItems);
          break;
        case BuiltInTypeKind.RefType:
          this.ValidateRefType((RefType) item, errors, validatedItems);
          break;
        case BuiltInTypeKind.TypeUsage:
          this.ValidateTypeUsage((TypeUsage) item, errors, validatedItems);
          break;
      }
      IEnumerable<EdmItemError> collection = this.CustomValidate(item);
      if (collection == null)
        return;
      errors.AddRange(collection);
    }

    private void ValidateCollectionType(
      CollectionType item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateEdmType((EdmType) item, errors, validatedItems);
      if (item.BaseType != null)
        this.AddError(errors, new EdmItemError(Strings.Validator_CollectionTypesCannotHaveBaseType));
      if (item.TypeUsage == null)
        this.AddError(errors, new EdmItemError(Strings.Validator_CollectionHasNoTypeUsage));
      else
        this.InternalValidate((MetadataItem) item.TypeUsage, errors, validatedItems);
    }

    private void ValidateComplexType(
      ComplexType item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateStructuralType((StructuralType) item, errors, validatedItems);
    }

    [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
    private void ValidateEdmType(
      EdmType item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateItem((MetadataItem) item, errors, validatedItems);
      if (string.IsNullOrEmpty(item.Name))
        this.AddError(errors, new EdmItemError(Strings.Validator_TypeHasNoName));
      if (item.NamespaceName == null || item.DataSpace != DataSpace.OSpace && string.Empty == item.NamespaceName)
        this.AddError(errors, new EdmItemError(Strings.Validator_TypeHasNoNamespace));
      if (item.BaseType == null)
        return;
      this.InternalValidate((MetadataItem) item.BaseType, errors, validatedItems);
    }

    private void ValidateEntityType(
      EntityType item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      if (item.BaseType == null)
      {
        if (item.KeyMembers.Count < 1)
        {
          this.AddError(errors, new EdmItemError(Strings.Validator_NoKeyMembers((object) item.FullName)));
        }
        else
        {
          foreach (EdmProperty keyMember in item.KeyMembers)
          {
            if (keyMember.Nullable)
              this.AddError(errors, new EdmItemError(Strings.Validator_NullableEntityKeyProperty((object) keyMember.Name, (object) item.FullName)));
          }
        }
      }
      this.ValidateStructuralType((StructuralType) item, errors, validatedItems);
    }

    private void ValidateFacet(
      Facet item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateItem((MetadataItem) item, errors, validatedItems);
      if (string.IsNullOrEmpty(item.Name))
        this.AddError(errors, new EdmItemError(Strings.Validator_FacetHasNoName));
      if (item.FacetType == null)
        this.AddError(errors, new EdmItemError(Strings.Validator_FacetTypeIsNull));
      else
        this.InternalValidate((MetadataItem) item.FacetType, errors, validatedItems);
    }

    private void ValidateItem(
      MetadataItem item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      if (item.RawMetadataProperties == null)
        return;
      foreach (MetadataItem metadataProperty in item.MetadataProperties)
        this.InternalValidate(metadataProperty, errors, validatedItems);
    }

    private void ValidateEdmMember(
      EdmMember item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateItem((MetadataItem) item, errors, validatedItems);
      if (string.IsNullOrEmpty(item.Name))
        this.AddError(errors, new EdmItemError(Strings.Validator_MemberHasNoName));
      if (item.DeclaringType == null)
        this.AddError(errors, new EdmItemError(Strings.Validator_MemberHasNullDeclaringType));
      else
        this.InternalValidate((MetadataItem) item.DeclaringType, errors, validatedItems);
      if (item.TypeUsage == null)
        this.AddError(errors, new EdmItemError(Strings.Validator_MemberHasNullTypeUsage));
      else
        this.InternalValidate((MetadataItem) item.TypeUsage, errors, validatedItems);
    }

    private void ValidateMetadataProperty(
      MetadataProperty item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      if (item.PropertyKind != PropertyKind.Extended)
        return;
      this.ValidateItem((MetadataItem) item, errors, validatedItems);
      if (string.IsNullOrEmpty(item.Name))
        this.AddError(errors, new EdmItemError(Strings.Validator_MetadataPropertyHasNoName));
      if (item.TypeUsage == null)
        this.AddError(errors, new EdmItemError(Strings.Validator_ItemAttributeHasNullTypeUsage));
      else
        this.InternalValidate((MetadataItem) item.TypeUsage, errors, validatedItems);
    }

    private void ValidateNavigationProperty(
      NavigationProperty item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateEdmMember((EdmMember) item, errors, validatedItems);
    }

    private void ValidatePrimitiveType(
      PrimitiveType item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateSimpleType((SimpleType) item, errors, validatedItems);
    }

    private void ValidateEdmProperty(
      EdmProperty item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateEdmMember((EdmMember) item, errors, validatedItems);
    }

    private void ValidateRefType(
      RefType item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateEdmType((EdmType) item, errors, validatedItems);
      if (item.BaseType != null)
        this.AddError(errors, new EdmItemError(Strings.Validator_RefTypesCannotHaveBaseType));
      if (item.ElementType == null)
        this.AddError(errors, new EdmItemError(Strings.Validator_RefTypeHasNullEntityType));
      else
        this.InternalValidate((MetadataItem) item.ElementType, errors, validatedItems);
    }

    private void ValidateSimpleType(
      SimpleType item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateEdmType((EdmType) item, errors, validatedItems);
    }

    private void ValidateStructuralType(
      StructuralType item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateEdmType((EdmType) item, errors, validatedItems);
      Dictionary<string, EdmMember> dictionary = new Dictionary<string, EdmMember>();
      foreach (EdmMember member in item.Members)
      {
        EdmMember edmMember = (EdmMember) null;
        if (dictionary.TryGetValue(member.Name, out edmMember))
          this.AddError(errors, new EdmItemError(Strings.Validator_BaseTypeHasMemberOfSameName));
        else
          dictionary.Add(member.Name, member);
        this.InternalValidate((MetadataItem) member, errors, validatedItems);
      }
    }

    private void ValidateTypeUsage(
      TypeUsage item,
      List<EdmItemError> errors,
      HashSet<MetadataItem> validatedItems)
    {
      this.ValidateItem((MetadataItem) item, errors, validatedItems);
      if (item.EdmType == null)
        this.AddError(errors, new EdmItemError(Strings.Validator_TypeUsageHasNullEdmType));
      else
        this.InternalValidate((MetadataItem) item.EdmType, errors, validatedItems);
      foreach (MetadataItem facet in item.Facets)
        this.InternalValidate(facet, errors, validatedItems);
    }
  }
}
