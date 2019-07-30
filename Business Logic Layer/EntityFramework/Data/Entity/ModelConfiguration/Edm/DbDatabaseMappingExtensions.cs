// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.DbDatabaseMappingExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class DbDatabaseMappingExtensions
  {
    public static DbDatabaseMapping Initialize(
      this DbDatabaseMapping databaseMapping,
      EdmModel model,
      EdmModel database)
    {
      databaseMapping.Model = model;
      databaseMapping.Database = database;
      databaseMapping.AddEntityContainerMapping(new EntityContainerMapping(model.Containers.Single<EntityContainer>()));
      return databaseMapping;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    public static MetadataWorkspace ToMetadataWorkspace(
      this DbDatabaseMapping databaseMapping)
    {
      EdmItemCollection itemCollection = new EdmItemCollection(databaseMapping.Model);
      StoreItemCollection storeItemCollection = new StoreItemCollection(databaseMapping.Database);
      StorageMappingItemCollection storageMappingItemCollection = databaseMapping.ToStorageMappingItemCollection(itemCollection, storeItemCollection);
      MetadataWorkspace metadataWorkspace = new MetadataWorkspace((Func<EdmItemCollection>) (() => itemCollection), (Func<StoreItemCollection>) (() => storeItemCollection), (Func<StorageMappingItemCollection>) (() => storageMappingItemCollection));
      new CodeFirstOSpaceLoader((CodeFirstOSpaceTypeFactory) null).LoadTypes(itemCollection, (ObjectItemCollection) metadataWorkspace.GetItemCollection(DataSpace.OSpace));
      return metadataWorkspace;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public static StorageMappingItemCollection ToStorageMappingItemCollection(
      this DbDatabaseMapping databaseMapping,
      EdmItemCollection itemCollection,
      StoreItemCollection storeItemCollection)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StringBuilder output = stringBuilder;
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        Indent = true
      };
      using (XmlWriter xmlWriter = XmlWriter.Create(output, settings))
        new MslSerializer().Serialize(databaseMapping, xmlWriter);
      using (XmlReader xmlReader = XmlReader.Create((TextReader) new StringReader(stringBuilder.ToString())))
        return new StorageMappingItemCollection(itemCollection, storeItemCollection, (IEnumerable<XmlReader>) new XmlReader[1]
        {
          xmlReader
        });
    }

    public static EntityTypeMapping GetEntityTypeMapping(
      this DbDatabaseMapping databaseMapping,
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType)
    {
      IList<EntityTypeMapping> entityTypeMappings = databaseMapping.GetEntityTypeMappings(entityType);
      if (entityTypeMappings.Count <= 1)
        return entityTypeMappings.FirstOrDefault<EntityTypeMapping>();
      return entityTypeMappings.SingleOrDefault<EntityTypeMapping>((Func<EntityTypeMapping, bool>) (m => m.IsHierarchyMapping));
    }

    public static IList<EntityTypeMapping> GetEntityTypeMappings(
      this DbDatabaseMapping databaseMapping,
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType)
    {
      List<EntityTypeMapping> entityTypeMappingList = new List<EntityTypeMapping>();
      foreach (EntitySetMapping entitySetMapping in databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>().EntitySetMappings)
      {
        foreach (EntityTypeMapping entityTypeMapping in entitySetMapping.EntityTypeMappings)
        {
          if (entityTypeMapping.EntityType == entityType)
            entityTypeMappingList.Add(entityTypeMapping);
        }
      }
      return (IList<EntityTypeMapping>) entityTypeMappingList;
    }

    public static EntityTypeMapping GetEntityTypeMapping(
      this DbDatabaseMapping databaseMapping,
      Type clrType)
    {
      List<EntityTypeMapping> source = new List<EntityTypeMapping>();
      foreach (EntitySetMapping entitySetMapping in databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>().EntitySetMappings)
      {
        foreach (EntityTypeMapping entityTypeMapping in entitySetMapping.EntityTypeMappings)
        {
          if (entityTypeMapping.GetClrType() == clrType)
            source.Add(entityTypeMapping);
        }
      }
      if (source.Count <= 1)
        return source.FirstOrDefault<EntityTypeMapping>();
      return source.SingleOrDefault<EntityTypeMapping>((Func<EntityTypeMapping, bool>) (m => m.IsHierarchyMapping));
    }

    public static IEnumerable<Tuple<ColumnMappingBuilder, System.Data.Entity.Core.Metadata.Edm.EntityType>> GetComplexPropertyMappings(
      this DbDatabaseMapping databaseMapping,
      Type complexType)
    {
      return databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>().EntitySetMappings.SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeMapping>>) (esm => (IEnumerable<EntityTypeMapping>) esm.EntityTypeMappings), (esm, etm) => new
      {
        esm = esm,
        etm = etm
      }).SelectMany(_param0 => (IEnumerable<MappingFragment>) _param0.etm.MappingFragments, (_param0, etmf) => new
      {
        \u003C\u003Eh__TransparentIdentifiera = _param0,
        etmf = etmf
      }).SelectMany(_param0 => _param0.etmf.ColumnMappings, (_param0, epm) => new
      {
        \u003C\u003Eh__TransparentIdentifierb = _param0,
        epm = epm
      }).Where(_param1 => _param1.epm.PropertyPath.Any<EdmProperty>((Func<EdmProperty, bool>) (p =>
      {
        if (p.IsComplexType)
          return ComplexTypeExtensions.GetClrType(p.ComplexType) == complexType;
        return false;
      }))).Select(_param0 => Tuple.Create<ColumnMappingBuilder, System.Data.Entity.Core.Metadata.Edm.EntityType>(_param0.epm, _param0.\u003C\u003Eh__TransparentIdentifierb.etmf.Table));
    }

    public static IEnumerable<ModificationFunctionParameterBinding> GetComplexParameterBindings(
      this DbDatabaseMapping databaseMapping,
      Type complexType)
    {
      return databaseMapping.GetEntitySetMappings().SelectMany((Func<EntitySetMapping, IEnumerable<EntityTypeModificationFunctionMapping>>) (esm => (IEnumerable<EntityTypeModificationFunctionMapping>) esm.ModificationFunctionMappings), (esm, mfm) => new
      {
        esm = esm,
        mfm = mfm
      }).SelectMany(_param0 => _param0.mfm.PrimaryParameterBindings, (_param0, pb) => new
      {
        \u003C\u003Eh__TransparentIdentifier1f = _param0,
        pb = pb
      }).Where(_param1 => _param1.pb.MemberPath.Members.OfType<EdmProperty>().Any<EdmProperty>((Func<EdmProperty, bool>) (p =>
      {
        if (p.IsComplexType)
          return ComplexTypeExtensions.GetClrType(p.ComplexType) == complexType;
        return false;
      }))).Select(_param0 => _param0.pb);
    }

    public static EntitySetMapping GetEntitySetMapping(
      this DbDatabaseMapping databaseMapping,
      EntitySet entitySet)
    {
      return databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>().EntitySetMappings.SingleOrDefault<EntitySetMapping>((Func<EntitySetMapping, bool>) (e => e.EntitySet == entitySet));
    }

    public static IEnumerable<EntitySetMapping> GetEntitySetMappings(
      this DbDatabaseMapping databaseMapping)
    {
      return databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>().EntitySetMappings;
    }

    public static IEnumerable<AssociationSetMapping> GetAssociationSetMappings(
      this DbDatabaseMapping databaseMapping)
    {
      return databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>().AssociationSetMappings;
    }

    public static EntitySetMapping AddEntitySetMapping(
      this DbDatabaseMapping databaseMapping,
      EntitySet entitySet)
    {
      EntitySetMapping setMapping = new EntitySetMapping(entitySet, (EntityContainerMapping) null);
      databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>().AddSetMapping(setMapping);
      return setMapping;
    }

    public static AssociationSetMapping AddAssociationSetMapping(
      this DbDatabaseMapping databaseMapping,
      AssociationSet associationSet,
      EntitySet entitySet)
    {
      EntityContainerMapping containerMapping = databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>();
      AssociationSetMapping setMapping = new AssociationSetMapping(associationSet, entitySet, containerMapping).Initialize();
      containerMapping.AddSetMapping(setMapping);
      return setMapping;
    }
  }
}
