// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.Services.PropertyMappingGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm.Services
{
  internal class PropertyMappingGenerator : StructuralTypeMappingGenerator
  {
    public PropertyMappingGenerator(DbProviderManifest providerManifest)
      : base(providerManifest)
    {
    }

    public void Generate(
      EntityType entityType,
      IEnumerable<EdmProperty> properties,
      EntitySetMapping entitySetMapping,
      MappingFragment entityTypeMappingFragment,
      IList<EdmProperty> propertyPath,
      bool createNewColumn)
    {
      ReadOnlyMetadataCollection<EdmProperty> declaredProperties = entityType.GetRootType().DeclaredProperties;
      foreach (EdmProperty property1 in properties)
      {
        EdmProperty property = property1;
        if (property.IsComplexType && propertyPath.Any<EdmProperty>((Func<EdmProperty, bool>) (p =>
        {
          if (p.IsComplexType)
            return p.ComplexType == property.ComplexType;
          return false;
        })))
          throw Error.CircularComplexTypeHierarchy();
        propertyPath.Add(property);
        if (property.IsComplexType)
        {
          this.Generate(entityType, (IEnumerable<EdmProperty>) property.ComplexType.Properties, entitySetMapping, entityTypeMappingFragment, propertyPath, createNewColumn);
        }
        else
        {
          EdmProperty edmProperty = entitySetMapping.EntityTypeMappings.SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (etm => (IEnumerable<MappingFragment>) etm.MappingFragments)).SelectMany<MappingFragment, ColumnMappingBuilder>((Func<MappingFragment, IEnumerable<ColumnMappingBuilder>>) (etmf => etmf.ColumnMappings)).Where<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => pm.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) propertyPath))).Select<ColumnMappingBuilder, EdmProperty>((Func<ColumnMappingBuilder, EdmProperty>) (pm => pm.ColumnProperty)).FirstOrDefault<EdmProperty>();
          if (edmProperty == null || createNewColumn)
          {
            string columnName = string.Join("_", propertyPath.Select<EdmProperty, string>((Func<EdmProperty, string>) (p => p.Name)));
            edmProperty = this.MapTableColumn(property, columnName, !declaredProperties.Contains(propertyPath.First<EdmProperty>()));
            entityTypeMappingFragment.Table.AddColumn(edmProperty);
            if (entityType.KeyProperties().Contains<EdmProperty>(property))
              entityTypeMappingFragment.Table.AddKeyMember((EdmMember) edmProperty);
          }
          entityTypeMappingFragment.AddColumnMapping(new ColumnMappingBuilder(edmProperty, (IList<EdmProperty>) propertyPath.ToList<EdmProperty>()));
        }
        propertyPath.Remove(property);
      }
    }
  }
}
