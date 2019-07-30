// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Mappers.TypeMapper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Mappers
{
  internal sealed class TypeMapper
  {
    private readonly List<Type> _knownTypes = new List<Type>();
    private readonly MappingContext _mappingContext;

    public TypeMapper(MappingContext mappingContext)
    {
      this._mappingContext = mappingContext;
      this._knownTypes.AddRange(mappingContext.ModelConfiguration.ConfiguredTypes.Select<Type, Assembly>((Func<Type, Assembly>) (t => t.Assembly())).Distinct<Assembly>().SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>) (a => a.GetAccessibleTypes().Where<Type>((Func<Type, bool>) (type => type.IsValidStructuralType())))));
    }

    public MappingContext MappingContext
    {
      get
      {
        return this._mappingContext;
      }
    }

    public EnumType MapEnumType(Type type)
    {
      EnumType enumType = TypeMapper.GetExistingEdmType<EnumType>(this._mappingContext.Model, type);
      if (enumType == null)
      {
        PrimitiveType primitiveType;
        if (!Enum.GetUnderlyingType(type).IsPrimitiveType(out primitiveType))
          return (EnumType) null;
        enumType = this._mappingContext.Model.AddEnumType(type.Name, this._mappingContext.ModelConfiguration.ModelNamespace);
        enumType.IsFlags = type.GetCustomAttributes<FlagsAttribute>(false).Any<FlagsAttribute>();
        enumType.SetClrType(type);
        enumType.UnderlyingType = primitiveType;
        foreach (string name in Enum.GetNames(type))
          enumType.AddMember(new EnumMember(name, Convert.ChangeType(Enum.Parse(type, name), type.GetEnumUnderlyingType(), (IFormatProvider) CultureInfo.InvariantCulture)));
      }
      return enumType;
    }

    public ComplexType MapComplexType(Type type, bool discoverNested = false)
    {
      if (!type.IsValidStructuralType())
        return (ComplexType) null;
      this._mappingContext.ConventionsConfiguration.ApplyModelConfiguration(type, this._mappingContext.ModelConfiguration);
      if (this._mappingContext.ModelConfiguration.IsIgnoredType(type) || !discoverNested && !this._mappingContext.ModelConfiguration.IsComplexType(type))
        return (ComplexType) null;
      ComplexType complexType = TypeMapper.GetExistingEdmType<ComplexType>(this._mappingContext.Model, type);
      if (complexType == null)
      {
        complexType = this._mappingContext.Model.AddComplexType(type.Name, this._mappingContext.ModelConfiguration.ModelNamespace);
        Func<ComplexTypeConfiguration> complexTypeConfiguration = (Func<ComplexTypeConfiguration>) (() => this._mappingContext.ModelConfiguration.ComplexType(type));
        this._mappingContext.ConventionsConfiguration.ApplyTypeConfiguration<ComplexTypeConfiguration>(type, complexTypeConfiguration, this._mappingContext.ModelConfiguration);
        this.MapStructuralElements<ComplexTypeConfiguration>(type, (ICollection<MetadataProperty>) complexType.GetMetadataProperties(), (Action<PropertyMapper, PropertyInfo>) ((m, p) => m.Map(p, complexType, complexTypeConfiguration)), complexTypeConfiguration);
      }
      return complexType;
    }

    public EntityType MapEntityType(Type type)
    {
      if (!type.IsValidStructuralType() || this._mappingContext.ModelConfiguration.IsIgnoredType(type) || this._mappingContext.ModelConfiguration.IsComplexType(type))
        return (EntityType) null;
      EntityType entityType = TypeMapper.GetExistingEdmType<EntityType>(this._mappingContext.Model, type);
      if (entityType == null)
      {
        this._mappingContext.ConventionsConfiguration.ApplyModelConfiguration(type, this._mappingContext.ModelConfiguration);
        if (this._mappingContext.ModelConfiguration.IsIgnoredType(type) || this._mappingContext.ModelConfiguration.IsComplexType(type))
          return (EntityType) null;
        entityType = this._mappingContext.Model.AddEntityType(type.Name, this._mappingContext.ModelConfiguration.ModelNamespace);
        entityType.Abstract = type.IsAbstract();
        EntityType entityType1 = this._mappingContext.Model.GetEntityType(type.BaseType().Name);
        if (entityType1 == null)
          this._mappingContext.Model.AddEntitySet(entityType.Name, entityType, (string) null);
        else if (object.ReferenceEquals((object) entityType1, (object) entityType))
          throw new NotSupportedException(Strings.SimpleNameCollision((object) type.FullName, (object) type.BaseType().FullName, (object) type.Name));
        entityType.BaseType = (EdmType) entityType1;
        Func<EntityTypeConfiguration> entityTypeConfiguration = (Func<EntityTypeConfiguration>) (() => this._mappingContext.ModelConfiguration.Entity(type));
        this._mappingContext.ConventionsConfiguration.ApplyTypeConfiguration<EntityTypeConfiguration>(type, entityTypeConfiguration, this._mappingContext.ModelConfiguration);
        List<PropertyInfo> navigationProperties = new List<PropertyInfo>();
        this.MapStructuralElements<EntityTypeConfiguration>(type, (ICollection<MetadataProperty>) entityType.GetMetadataProperties(), (Action<PropertyMapper, PropertyInfo>) ((m, p) =>
        {
          if (m.MapIfNotNavigationProperty(p, entityType, entityTypeConfiguration))
            return;
          navigationProperties.Add(p);
        }), entityTypeConfiguration);
        IEnumerable<PropertyInfo> source = (IEnumerable<PropertyInfo>) navigationProperties;
        if (this._mappingContext.ModelBuilderVersion.IsEF6OrHigher())
          source = (IEnumerable<PropertyInfo>) source.OrderBy<PropertyInfo, string>((Func<PropertyInfo, string>) (p => p.Name));
        foreach (PropertyInfo propertyInfo in source)
          new NavigationPropertyMapper(this).Map(propertyInfo, entityType, entityTypeConfiguration);
        if (entityType.BaseType != null)
          this.LiftInheritedProperties(type, entityType);
        this.MapDerivedTypes(type, entityType);
      }
      return entityType;
    }

    private static T GetExistingEdmType<T>(EdmModel model, Type type) where T : EdmType
    {
      EdmType structuralOrEnumType = model.GetStructuralOrEnumType(type.Name);
      if (structuralOrEnumType != null && type != structuralOrEnumType.GetClrType())
        throw new NotSupportedException(Strings.SimpleNameCollision((object) type.FullName, (object) structuralOrEnumType.GetClrType().FullName, (object) type.Name));
      return structuralOrEnumType as T;
    }

    private void MapStructuralElements<TStructuralTypeConfiguration>(
      Type type,
      ICollection<MetadataProperty> annotations,
      Action<PropertyMapper, PropertyInfo> propertyMappingAction,
      Func<TStructuralTypeConfiguration> structuralTypeConfiguration)
      where TStructuralTypeConfiguration : StructuralTypeConfiguration
    {
      annotations.SetClrType(type);
      new AttributeMapper(this._mappingContext.AttributeProvider).Map(type, annotations);
      PropertyMapper propertyMapper = new PropertyMapper(this);
      List<PropertyInfo> list = new PropertyFilter(this._mappingContext.ModelBuilderVersion).GetProperties(type, false, this._mappingContext.ModelConfiguration.GetConfiguredProperties(type), this._mappingContext.ModelConfiguration.StructuralTypes, false).ToList<PropertyInfo>();
      for (int index = 0; index < list.Count; ++index)
      {
        PropertyInfo propertyInfo = list[index];
        this._mappingContext.ConventionsConfiguration.ApplyPropertyConfiguration(propertyInfo, this._mappingContext.ModelConfiguration);
        this._mappingContext.ConventionsConfiguration.ApplyPropertyTypeConfiguration<TStructuralTypeConfiguration>(propertyInfo, structuralTypeConfiguration, this._mappingContext.ModelConfiguration);
        if (!this._mappingContext.ModelConfiguration.IsIgnoredProperty(type, propertyInfo))
          propertyMappingAction(propertyMapper, propertyInfo);
      }
    }

    private void MapDerivedTypes(Type type, EntityType entityType)
    {
      if (type.IsSealed())
        return;
      if (!this._knownTypes.Contains(type))
        this._knownTypes.AddRange(type.Assembly().GetAccessibleTypes().Where<Type>((Func<Type, bool>) (t => t.IsValidStructuralType())));
      IEnumerable<Type> source = this._knownTypes.Where<Type>((Func<Type, bool>) (t => t.BaseType() == type));
      if (this._mappingContext.ModelBuilderVersion.IsEF6OrHigher())
        source = (IEnumerable<Type>) source.OrderBy<Type, string>((Func<Type, string>) (t => t.FullName));
      List<Type> list = source.ToList<Type>();
      for (int index = 0; index < list.Count; ++index)
      {
        Type type1 = list[index];
        EntityType derivedEntityType = this.MapEntityType(type1);
        if (derivedEntityType != null)
        {
          derivedEntityType.BaseType = (EdmType) entityType;
          this.LiftDerivedType(type1, derivedEntityType, entityType);
        }
      }
    }

    private void LiftDerivedType(
      Type derivedType,
      EntityType derivedEntityType,
      EntityType entityType)
    {
      this._mappingContext.Model.ReplaceEntitySet(derivedEntityType, this._mappingContext.Model.GetEntitySet(entityType));
      this.LiftInheritedProperties(derivedType, derivedEntityType);
    }

    private void LiftInheritedProperties(Type type, EntityType entityType)
    {
      EntityTypeConfiguration typeConfiguration = this._mappingContext.ModelConfiguration.GetStructuralTypeConfiguration(type) as EntityTypeConfiguration;
      if (typeConfiguration != null)
      {
        typeConfiguration.ClearKey();
        foreach (PropertyInfo instanceProperty in type.BaseType().GetInstanceProperties())
        {
          PropertyInfo property = instanceProperty;
          if (!this._mappingContext.AttributeProvider.GetAttributes(property).OfType<NotMappedAttribute>().Any<NotMappedAttribute>() && typeConfiguration.IgnoredProperties.Any<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.IsSameAs(property))))
            throw Error.CannotIgnoreMappedBaseProperty((object) property.Name, (object) type, (object) property.DeclaringType);
        }
      }
      List<EdmMember> list = entityType.DeclaredMembers.ToList<EdmMember>();
      HashSet<PropertyInfo> propertyInfoSet = new HashSet<PropertyInfo>(new PropertyFilter(this._mappingContext.ModelBuilderVersion).GetProperties(type, true, this._mappingContext.ModelConfiguration.GetConfiguredProperties(type), this._mappingContext.ModelConfiguration.StructuralTypes, false));
      foreach (EdmMember edmMember in list)
      {
        PropertyInfo clrPropertyInfo = edmMember.GetClrPropertyInfo();
        if (!propertyInfoSet.Contains(clrPropertyInfo))
        {
          NavigationProperty navigationProperty = edmMember as NavigationProperty;
          if (navigationProperty != null)
            this._mappingContext.Model.RemoveAssociationType(navigationProperty.Association);
          entityType.RemoveMember(edmMember);
        }
      }
    }
  }
}
