// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Validation.EntityValidatorBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Internal.Validation
{
  internal class EntityValidatorBuilder
  {
    private readonly AttributeProvider _attributeProvider;

    public EntityValidatorBuilder(AttributeProvider attributeProvider)
    {
      this._attributeProvider = attributeProvider;
    }

    public virtual EntityValidator BuildEntityValidator(
      InternalEntityEntry entityEntry)
    {
      return this.BuildTypeValidator<EntityValidator>(entityEntry.EntityType, (IEnumerable<EdmProperty>) entityEntry.EdmEntityType.Properties, (IEnumerable<NavigationProperty>) entityEntry.EdmEntityType.NavigationProperties, (Func<IEnumerable<PropertyValidator>, IEnumerable<IValidator>, EntityValidator>) ((propertyValidators, typeLevelValidators) => new EntityValidator(propertyValidators, typeLevelValidators)));
    }

    protected virtual ComplexTypeValidator BuildComplexTypeValidator(
      Type clrType,
      ComplexType complexType)
    {
      return this.BuildTypeValidator<ComplexTypeValidator>(clrType, (IEnumerable<EdmProperty>) complexType.Properties, Enumerable.Empty<NavigationProperty>(), (Func<IEnumerable<PropertyValidator>, IEnumerable<IValidator>, ComplexTypeValidator>) ((propertyValidators, typeLevelValidators) => new ComplexTypeValidator(propertyValidators, typeLevelValidators)));
    }

    private T BuildTypeValidator<T>(
      Type clrType,
      IEnumerable<EdmProperty> edmProperties,
      IEnumerable<NavigationProperty> navigationProperties,
      Func<IEnumerable<PropertyValidator>, IEnumerable<IValidator>, T> validatorFactoryFunc)
      where T : TypeValidator
    {
      IList<PropertyValidator> source1 = this.BuildValidatorsForProperties(this.GetPublicInstanceProperties(clrType), edmProperties, navigationProperties);
      IEnumerable<Attribute> attributes = this._attributeProvider.GetAttributes(clrType);
      IList<IValidator> source2 = this.BuildValidationAttributeValidators(attributes);
      if (typeof (IValidatableObject).IsAssignableFrom(clrType))
        source2.Add((IValidator) new ValidatableObjectValidator(attributes.OfType<DisplayAttribute>().SingleOrDefault<DisplayAttribute>()));
      if (!source1.Any<PropertyValidator>() && !source2.Any<IValidator>())
        return default (T);
      return validatorFactoryFunc((IEnumerable<PropertyValidator>) source1, (IEnumerable<IValidator>) source2);
    }

    protected virtual IList<PropertyValidator> BuildValidatorsForProperties(
      IEnumerable<PropertyInfo> clrProperties,
      IEnumerable<EdmProperty> edmProperties,
      IEnumerable<NavigationProperty> navigationProperties)
    {
      List<PropertyValidator> propertyValidatorList = new List<PropertyValidator>();
      foreach (PropertyInfo clrProperty in clrProperties)
      {
        PropertyInfo property = clrProperty;
        EdmProperty edmProperty = edmProperties.Where<EdmProperty>((Func<EdmProperty, bool>) (p => p.Name == property.Name)).SingleOrDefault<EdmProperty>();
        PropertyValidator propertyValidator;
        if (edmProperty != null)
        {
          IEnumerable<ReferentialConstraint> source = navigationProperties.Select(navigationProperty => new
          {
            navigationProperty = navigationProperty,
            associationType = navigationProperty.RelationshipType as AssociationType
          }).Where(_param0 => _param0.associationType != null).SelectMany(_param0 => (IEnumerable<ReferentialConstraint>) _param0.associationType.ReferentialConstraints, (_param0, constraint) => new
          {
            \u003C\u003Eh__TransparentIdentifier4 = _param0,
            constraint = constraint
          }).Where(_param1 => _param1.constraint.ToProperties.Contains(edmProperty)).Select(_param0 => _param0.constraint);
          propertyValidator = this.BuildPropertyValidator(property, edmProperty, !source.Any<ReferentialConstraint>());
        }
        else
          propertyValidator = this.BuildPropertyValidator(property);
        if (propertyValidator != null)
          propertyValidatorList.Add(propertyValidator);
      }
      return (IList<PropertyValidator>) propertyValidatorList;
    }

    protected virtual PropertyValidator BuildPropertyValidator(
      PropertyInfo clrProperty,
      EdmProperty edmProperty,
      bool buildFacetValidators)
    {
      List<IValidator> source = new List<IValidator>();
      IEnumerable<Attribute> attributes = this._attributeProvider.GetAttributes(clrProperty);
      source.AddRange((IEnumerable<IValidator>) this.BuildValidationAttributeValidators(attributes));
      if (edmProperty.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType)
      {
        ComplexType edmType = (ComplexType) edmProperty.TypeUsage.EdmType;
        ComplexTypeValidator complexTypeValidator = this.BuildComplexTypeValidator(clrProperty.PropertyType, edmType);
        if (!source.Any<IValidator>() && complexTypeValidator == null)
          return (PropertyValidator) null;
        return (PropertyValidator) new ComplexPropertyValidator(clrProperty.Name, (IEnumerable<IValidator>) source, complexTypeValidator);
      }
      if (buildFacetValidators)
        source.AddRange(this.BuildFacetValidators(clrProperty, (EdmMember) edmProperty, attributes));
      if (!source.Any<IValidator>())
        return (PropertyValidator) null;
      return new PropertyValidator(clrProperty.Name, (IEnumerable<IValidator>) source);
    }

    protected virtual PropertyValidator BuildPropertyValidator(
      PropertyInfo clrProperty)
    {
      IList<IValidator> validatorList = this.BuildValidationAttributeValidators(this._attributeProvider.GetAttributes(clrProperty));
      if (validatorList.Count <= 0)
        return (PropertyValidator) null;
      return new PropertyValidator(clrProperty.Name, (IEnumerable<IValidator>) validatorList);
    }

    protected virtual IList<IValidator> BuildValidationAttributeValidators(
      IEnumerable<Attribute> attributes)
    {
      return (IList<IValidator>) ((IEnumerable<IValidator>) attributes.Where<Attribute>((Func<Attribute, bool>) (validationAttribute => validationAttribute is ValidationAttribute)).Select<Attribute, ValidationAttributeValidator>((Func<Attribute, ValidationAttributeValidator>) (validationAttribute => new ValidationAttributeValidator((ValidationAttribute) validationAttribute, attributes.OfType<DisplayAttribute>().SingleOrDefault<DisplayAttribute>())))).ToList<IValidator>();
    }

    protected virtual IEnumerable<PropertyInfo> GetPublicInstanceProperties(
      Type type)
    {
      return type.GetInstanceProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (p.IsPublic() && p.GetIndexParameters().Length == 0)
          return p.Getter() != (MethodInfo) null;
        return false;
      }));
    }

    protected virtual IEnumerable<IValidator> BuildFacetValidators(
      PropertyInfo clrProperty,
      EdmMember edmProperty,
      IEnumerable<Attribute> existingAttributes)
    {
      List<ValidationAttribute> source = new List<ValidationAttribute>();
      MetadataProperty metadataProperty;
      edmProperty.MetadataProperties.TryGetValue("http://schemas.microsoft.com/ado/2009/02/edm/annotation:StoreGeneratedPattern", false, out metadataProperty);
      bool flag = metadataProperty != null && metadataProperty.Value != null;
      Facet facet1;
      edmProperty.TypeUsage.Facets.TryGetValue("Nullable", false, out facet1);
      if (facet1 != null && facet1.Value != null && !(bool) facet1.Value && !flag && clrProperty.PropertyType.IsNullable() && !existingAttributes.Any<Attribute>((Func<Attribute, bool>) (a => a is RequiredAttribute)))
        source.Add((ValidationAttribute) new RequiredAttribute()
        {
          AllowEmptyStrings = true
        });
      Facet facet2;
      edmProperty.TypeUsage.Facets.TryGetValue("MaxLength", false, out facet2);
      if (facet2 != null && facet2.Value != null && facet2.Value is int && !existingAttributes.Any<Attribute>((Func<Attribute, bool>) (a => a is MaxLengthAttribute)) && !existingAttributes.Any<Attribute>((Func<Attribute, bool>) (a => a is StringLengthAttribute)))
        source.Add((ValidationAttribute) new MaxLengthAttribute((int) facet2.Value));
      return (IEnumerable<IValidator>) source.Select<ValidationAttribute, ValidationAttributeValidator>((Func<ValidationAttribute, ValidationAttributeValidator>) (attribute => new ValidationAttributeValidator(attribute, existingAttributes.OfType<DisplayAttribute>().SingleOrDefault<DisplayAttribute>())));
    }
  }
}
