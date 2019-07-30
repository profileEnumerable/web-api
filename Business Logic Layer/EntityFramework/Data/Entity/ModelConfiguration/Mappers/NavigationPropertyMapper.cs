// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Mappers.NavigationPropertyMapper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Properties;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Mappers
{
  internal sealed class NavigationPropertyMapper
  {
    private readonly TypeMapper _typeMapper;

    public NavigationPropertyMapper(TypeMapper typeMapper)
    {
      this._typeMapper = typeMapper;
    }

    public void Map(
      PropertyInfo propertyInfo,
      EntityType entityType,
      Func<EntityTypeConfiguration> entityTypeConfiguration)
    {
      Type elementType = propertyInfo.PropertyType;
      RelationshipMultiplicity relationshipMultiplicity = RelationshipMultiplicity.ZeroOrOne;
      if (elementType.IsCollection(out elementType))
        relationshipMultiplicity = RelationshipMultiplicity.Many;
      EntityType targetEntityType = this._typeMapper.MapEntityType(elementType);
      if (targetEntityType == null)
        return;
      RelationshipMultiplicity sourceAssociationEndKind = relationshipMultiplicity.IsMany() ? RelationshipMultiplicity.ZeroOrOne : RelationshipMultiplicity.Many;
      AssociationType associationType = this._typeMapper.MappingContext.Model.AddAssociationType(entityType.Name + "_" + propertyInfo.Name, entityType, sourceAssociationEndKind, targetEntityType, relationshipMultiplicity, this._typeMapper.MappingContext.ModelConfiguration.ModelNamespace);
      associationType.SourceEnd.SetClrPropertyInfo(propertyInfo);
      this._typeMapper.MappingContext.Model.AddAssociationSet(associationType.Name, associationType);
      NavigationProperty property = entityType.AddNavigationProperty(propertyInfo.Name, associationType);
      property.SetClrPropertyInfo(propertyInfo);
      this._typeMapper.MappingContext.ConventionsConfiguration.ApplyPropertyConfiguration(propertyInfo, (Func<PropertyConfiguration>) (() => (PropertyConfiguration) entityTypeConfiguration().Navigation(propertyInfo)), this._typeMapper.MappingContext.ModelConfiguration);
      new AttributeMapper(this._typeMapper.MappingContext.AttributeProvider).Map(propertyInfo, (ICollection<MetadataProperty>) property.GetMetadataProperties());
    }
  }
}
