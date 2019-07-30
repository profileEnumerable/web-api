// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.Services.TableMappingGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm.Services
{
  internal class TableMappingGenerator : StructuralTypeMappingGenerator
  {
    public TableMappingGenerator(DbProviderManifest providerManifest)
      : base(providerManifest)
    {
    }

    public void Generate(EntityType entityType, DbDatabaseMapping databaseMapping)
    {
      EntitySet entitySet = databaseMapping.Model.GetEntitySet(entityType);
      EntitySetMapping entitySetMapping = databaseMapping.GetEntitySetMapping(entitySet) ?? databaseMapping.AddEntitySetMapping(entitySet);
      EntityTypeMapping entityTypeMapping1 = entitySetMapping.EntityTypeMappings.FirstOrDefault<EntityTypeMapping>((Func<EntityTypeMapping, bool>) (m => m.EntityTypes.Contains((EntityTypeBase) entitySet.ElementType))) ?? entitySetMapping.EntityTypeMappings.FirstOrDefault<EntityTypeMapping>();
      EntityType entityType1 = entityTypeMapping1 != null ? entityTypeMapping1.MappingFragments.First<MappingFragment>().Table : databaseMapping.Database.AddTable(entityType.GetRootType().Name);
      EntityTypeMapping entityTypeMapping2 = new EntityTypeMapping((EntitySetMapping) null);
      MappingFragment mappingFragment = new MappingFragment(databaseMapping.Database.GetEntitySet(entityType1), (TypeMapping) entityTypeMapping2, false);
      entityTypeMapping2.AddType(entityType);
      entityTypeMapping2.AddFragment(mappingFragment);
      entityTypeMapping2.SetClrType(EntityTypeExtensions.GetClrType(entityType));
      entitySetMapping.AddTypeMapping(entityTypeMapping2);
      new PropertyMappingGenerator(this._providerManifest).Generate(entityType, (IEnumerable<EdmProperty>) entityType.Properties, entitySetMapping, mappingFragment, (IList<EdmProperty>) new List<EdmProperty>(), false);
    }
  }
}
