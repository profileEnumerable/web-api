// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.Services.DatabaseMappingGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm.Services
{
  internal class DatabaseMappingGenerator
  {
    public static TypeUsage DiscriminatorTypeUsage = TypeUsage.CreateStringTypeUsage(PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.String), true, false, 128);
    private const string DiscriminatorColumnName = "Discriminator";
    public const int DiscriminatorMaxLength = 128;
    private readonly DbProviderInfo _providerInfo;
    private readonly DbProviderManifest _providerManifest;

    public DatabaseMappingGenerator(
      DbProviderInfo providerInfo,
      DbProviderManifest providerManifest)
    {
      this._providerInfo = providerInfo;
      this._providerManifest = providerManifest;
    }

    public DbDatabaseMapping Generate(EdmModel conceptualModel)
    {
      DbDatabaseMapping databaseMapping = this.InitializeDatabaseMapping(conceptualModel);
      DatabaseMappingGenerator.GenerateEntityTypes(databaseMapping);
      DatabaseMappingGenerator.GenerateDiscriminators(databaseMapping);
      DatabaseMappingGenerator.GenerateAssociationTypes(databaseMapping);
      return databaseMapping;
    }

    private DbDatabaseMapping InitializeDatabaseMapping(EdmModel conceptualModel)
    {
      EdmModel storeModel = EdmModel.CreateStoreModel(this._providerInfo, this._providerManifest, conceptualModel.SchemaVersion);
      return new DbDatabaseMapping().Initialize(conceptualModel, storeModel);
    }

    private static void GenerateEntityTypes(DbDatabaseMapping databaseMapping)
    {
      foreach (EntityType entityType1 in databaseMapping.Model.EntityTypes)
      {
        EntityType entityType = entityType1;
        if (entityType.Abstract && databaseMapping.Model.EntityTypes.All<EntityType>((Func<EntityType, bool>) (e => e.BaseType != entityType)))
          throw new InvalidOperationException(Strings.UnmappedAbstractType((object) EntityTypeExtensions.GetClrType(entityType)));
        new TableMappingGenerator(databaseMapping.ProviderManifest).Generate(entityType, databaseMapping);
      }
    }

    private static void GenerateDiscriminators(DbDatabaseMapping databaseMapping)
    {
      foreach (EntitySetMapping entitySetMapping in databaseMapping.GetEntitySetMappings())
      {
        if (entitySetMapping.EntityTypeMappings.Count<EntityTypeMapping>() > 1)
        {
          EdmProperty edmProperty = new EdmProperty("Discriminator", databaseMapping.ProviderManifest.GetStoreType(DatabaseMappingGenerator.DiscriminatorTypeUsage))
          {
            Nullable = false,
            DefaultValue = (object) "(Undefined)"
          };
          entitySetMapping.EntityTypeMappings.First<EntityTypeMapping>().MappingFragments.Single<MappingFragment>().Table.AddColumn(edmProperty);
          foreach (EntityTypeMapping entityTypeMapping in entitySetMapping.EntityTypeMappings)
          {
            if (!entityTypeMapping.EntityType.Abstract)
            {
              MappingFragment mappingFragment = entityTypeMapping.MappingFragments.Single<MappingFragment>();
              mappingFragment.SetDefaultDiscriminator(edmProperty);
              mappingFragment.AddDiscriminatorCondition(edmProperty, (object) entityTypeMapping.EntityType.Name);
            }
          }
        }
      }
    }

    private static void GenerateAssociationTypes(DbDatabaseMapping databaseMapping)
    {
      foreach (AssociationType associationType in databaseMapping.Model.AssociationTypes)
        new AssociationTypeMappingGenerator(databaseMapping.ProviderManifest).Generate(associationType, databaseMapping);
    }
  }
}
