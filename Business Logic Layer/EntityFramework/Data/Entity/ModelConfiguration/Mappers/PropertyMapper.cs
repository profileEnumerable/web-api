// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Mappers.PropertyMapper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Properties;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Mappers
{
  internal sealed class PropertyMapper
  {
    private readonly TypeMapper _typeMapper;

    public PropertyMapper(TypeMapper typeMapper)
    {
      this._typeMapper = typeMapper;
    }

    public void Map(
      PropertyInfo propertyInfo,
      ComplexType complexType,
      Func<ComplexTypeConfiguration> complexTypeConfiguration)
    {
      EdmProperty edmProperty = this.MapPrimitiveOrComplexOrEnumProperty(propertyInfo, (Func<StructuralTypeConfiguration>) complexTypeConfiguration, true);
      if (edmProperty == null)
        return;
      complexType.AddMember((EdmMember) edmProperty);
    }

    public void Map(
      PropertyInfo propertyInfo,
      EntityType entityType,
      Func<EntityTypeConfiguration> entityTypeConfiguration)
    {
      EdmProperty edmProperty = this.MapPrimitiveOrComplexOrEnumProperty(propertyInfo, (Func<StructuralTypeConfiguration>) entityTypeConfiguration, false);
      if (edmProperty != null)
        entityType.AddMember((EdmMember) edmProperty);
      else
        new NavigationPropertyMapper(this._typeMapper).Map(propertyInfo, entityType, entityTypeConfiguration);
    }

    internal bool MapIfNotNavigationProperty(
      PropertyInfo propertyInfo,
      EntityType entityType,
      Func<EntityTypeConfiguration> entityTypeConfiguration)
    {
      EdmProperty edmProperty = this.MapPrimitiveOrComplexOrEnumProperty(propertyInfo, (Func<StructuralTypeConfiguration>) entityTypeConfiguration, false);
      if (edmProperty == null)
        return false;
      entityType.AddMember((EdmMember) edmProperty);
      return true;
    }

    private EdmProperty MapPrimitiveOrComplexOrEnumProperty(
      PropertyInfo propertyInfo,
      Func<StructuralTypeConfiguration> structuralTypeConfiguration,
      bool discoverComplexTypes = false)
    {
      EdmProperty property = propertyInfo.AsEdmPrimitiveProperty();
      if (property == null)
      {
        Type underlyingType = propertyInfo.PropertyType;
        ComplexType complexType = this._typeMapper.MapComplexType(underlyingType, discoverComplexTypes);
        if (complexType != null)
        {
          property = EdmProperty.CreateComplex(propertyInfo.Name, complexType);
        }
        else
        {
          bool flag = underlyingType.TryUnwrapNullableType(out underlyingType);
          if (underlyingType.IsEnum())
          {
            EnumType enumType = this._typeMapper.MapEnumType(underlyingType);
            if (enumType != null)
            {
              property = EdmProperty.CreateEnum(propertyInfo.Name, enumType);
              property.Nullable = flag;
            }
          }
        }
      }
      if (property != null)
      {
        property.SetClrPropertyInfo(propertyInfo);
        new AttributeMapper(this._typeMapper.MappingContext.AttributeProvider).Map(propertyInfo, (ICollection<MetadataProperty>) property.GetMetadataProperties());
        if (!property.IsComplexType)
          this._typeMapper.MappingContext.ConventionsConfiguration.ApplyPropertyConfiguration(propertyInfo, (Func<PropertyConfiguration>) (() => (PropertyConfiguration) structuralTypeConfiguration().Property(new PropertyPath(propertyInfo), new OverridableConfigurationParts?())), this._typeMapper.MappingContext.ModelConfiguration);
      }
      return property;
    }
  }
}
