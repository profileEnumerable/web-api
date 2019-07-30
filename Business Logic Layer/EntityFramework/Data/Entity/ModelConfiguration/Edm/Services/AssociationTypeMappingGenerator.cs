// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.Services.AssociationTypeMappingGenerator
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
  internal class AssociationTypeMappingGenerator : StructuralTypeMappingGenerator
  {
    public AssociationTypeMappingGenerator(DbProviderManifest providerManifest)
      : base(providerManifest)
    {
    }

    public void Generate(AssociationType associationType, DbDatabaseMapping databaseMapping)
    {
      if (associationType.Constraint != null)
        AssociationTypeMappingGenerator.GenerateForeignKeyAssociationType(associationType, databaseMapping);
      else if (associationType.IsManyToMany())
        this.GenerateManyToManyAssociation(associationType, databaseMapping);
      else
        this.GenerateIndependentAssociationType(associationType, databaseMapping);
    }

    private static void GenerateForeignKeyAssociationType(
      AssociationType associationType,
      DbDatabaseMapping databaseMapping)
    {
      AssociationEndMember dependentEnd = associationType.Constraint.DependentEnd;
      AssociationEndMember otherEnd = associationType.GetOtherEnd(dependentEnd);
      EntityTypeMapping mappingInHierarchy = StructuralTypeMappingGenerator.GetEntityTypeMappingInHierarchy(databaseMapping, otherEnd.GetEntityType());
      EntityTypeMapping dependentEntityTypeMapping = StructuralTypeMappingGenerator.GetEntityTypeMappingInHierarchy(databaseMapping, dependentEnd.GetEntityType());
      ForeignKeyBuilder foreignKeyBuilder = new ForeignKeyBuilder(databaseMapping.Database, associationType.Name)
      {
        PrincipalTable = mappingInHierarchy.MappingFragments.Single<MappingFragment>().Table,
        DeleteAction = otherEnd.DeleteBehavior != OperationAction.None ? otherEnd.DeleteBehavior : OperationAction.None
      };
      dependentEntityTypeMapping.MappingFragments.Single<MappingFragment>().Table.AddForeignKey(foreignKeyBuilder);
      foreignKeyBuilder.DependentColumns = associationType.Constraint.ToProperties.Select<EdmProperty, EdmProperty>((Func<EdmProperty, EdmProperty>) (dependentProperty => dependentEntityTypeMapping.GetPropertyMapping(dependentProperty).ColumnProperty));
      foreignKeyBuilder.SetAssociationType(associationType);
    }

    private void GenerateManyToManyAssociation(
      AssociationType associationType,
      DbDatabaseMapping databaseMapping)
    {
      EntityType entityType1 = associationType.SourceEnd.GetEntityType();
      EntityType entityType2 = associationType.TargetEnd.GetEntityType();
      EntityType dependentTable = databaseMapping.Database.AddTable(entityType1.Name + entityType2.Name);
      AssociationSetMapping associationSetMapping = AssociationTypeMappingGenerator.GenerateAssociationSetMapping(associationType, databaseMapping, associationType.SourceEnd, associationType.TargetEnd, dependentTable);
      this.GenerateIndependentForeignKeyConstraint(databaseMapping, entityType1, entityType2, dependentTable, associationSetMapping, associationSetMapping.SourceEndMapping, associationType.SourceEnd.Name, (AssociationEndMember) null, true);
      this.GenerateIndependentForeignKeyConstraint(databaseMapping, entityType2, entityType1, dependentTable, associationSetMapping, associationSetMapping.TargetEndMapping, associationType.TargetEnd.Name, (AssociationEndMember) null, true);
    }

    private void GenerateIndependentAssociationType(
      AssociationType associationType,
      DbDatabaseMapping databaseMapping)
    {
      AssociationEndMember principalEnd;
      AssociationEndMember dependentEnd;
      if (!associationType.TryGuessPrincipalAndDependentEnds(out principalEnd, out dependentEnd))
      {
        if (!associationType.IsPrincipalConfigured())
          throw Error.UnableToDeterminePrincipal((object) EntityTypeExtensions.GetClrType(associationType.SourceEnd.GetEntityType()), (object) EntityTypeExtensions.GetClrType(associationType.TargetEnd.GetEntityType()));
        principalEnd = associationType.SourceEnd;
        dependentEnd = associationType.TargetEnd;
      }
      EntityTypeMapping mappingInHierarchy = StructuralTypeMappingGenerator.GetEntityTypeMappingInHierarchy(databaseMapping, dependentEnd.GetEntityType());
      EntityType table = mappingInHierarchy.MappingFragments.First<MappingFragment>().Table;
      AssociationSetMapping associationSetMapping = AssociationTypeMappingGenerator.GenerateAssociationSetMapping(associationType, databaseMapping, principalEnd, dependentEnd, table);
      this.GenerateIndependentForeignKeyConstraint(databaseMapping, principalEnd.GetEntityType(), dependentEnd.GetEntityType(), table, associationSetMapping, associationSetMapping.SourceEndMapping, associationType.Name, principalEnd, false);
      foreach (EdmProperty keyProperty in dependentEnd.GetEntityType().KeyProperties())
        associationSetMapping.TargetEndMapping.AddPropertyMapping(new ScalarPropertyMapping(keyProperty, mappingInHierarchy.GetPropertyMapping(keyProperty).ColumnProperty));
    }

    private static AssociationSetMapping GenerateAssociationSetMapping(
      AssociationType associationType,
      DbDatabaseMapping databaseMapping,
      AssociationEndMember principalEnd,
      AssociationEndMember dependentEnd,
      EntityType dependentTable)
    {
      AssociationSetMapping associationSetMapping = databaseMapping.AddAssociationSetMapping(databaseMapping.Model.GetAssociationSet(associationType), databaseMapping.Database.GetEntitySet(dependentTable));
      associationSetMapping.StoreEntitySet = databaseMapping.Database.GetEntitySet(dependentTable);
      associationSetMapping.SourceEndMapping.AssociationEnd = principalEnd;
      associationSetMapping.TargetEndMapping.AssociationEnd = dependentEnd;
      return associationSetMapping;
    }

    private void GenerateIndependentForeignKeyConstraint(
      DbDatabaseMapping databaseMapping,
      EntityType principalEntityType,
      EntityType dependentEntityType,
      EntityType dependentTable,
      AssociationSetMapping associationSetMapping,
      EndPropertyMapping associationEndMapping,
      string name,
      AssociationEndMember principalEnd,
      bool isPrimaryKeyColumn = false)
    {
      EntityType table = StructuralTypeMappingGenerator.GetEntityTypeMappingInHierarchy(databaseMapping, principalEntityType).MappingFragments.Single<MappingFragment>().Table;
      ForeignKeyBuilder foreignKeyBuilder = new ForeignKeyBuilder(databaseMapping.Database, name)
      {
        PrincipalTable = table,
        DeleteAction = associationEndMapping.AssociationEnd.DeleteBehavior != OperationAction.None ? associationEndMapping.AssociationEnd.DeleteBehavior : OperationAction.None
      };
      NavigationProperty principalNavigationProperty = databaseMapping.Model.EntityTypes.SelectMany<EntityType, NavigationProperty>((Func<EntityType, IEnumerable<NavigationProperty>>) (e => (IEnumerable<NavigationProperty>) e.DeclaredNavigationProperties)).SingleOrDefault<NavigationProperty>((Func<NavigationProperty, bool>) (n => n.ResultEnd == principalEnd));
      dependentTable.AddForeignKey(foreignKeyBuilder);
      foreignKeyBuilder.DependentColumns = this.GenerateIndependentForeignKeyColumns(principalEntityType, dependentEntityType, associationSetMapping, associationEndMapping, dependentTable, isPrimaryKeyColumn, principalNavigationProperty);
    }

    private IEnumerable<EdmProperty> GenerateIndependentForeignKeyColumns(
      EntityType principalEntityType,
      EntityType dependentEntityType,
      AssociationSetMapping associationSetMapping,
      EndPropertyMapping associationEndMapping,
      EntityType dependentTable,
      bool isPrimaryKeyColumn,
      NavigationProperty principalNavigationProperty)
    {
      foreach (EdmProperty keyProperty in principalEntityType.KeyProperties())
      {
        string columnName = (principalNavigationProperty != null ? principalNavigationProperty.Name : principalEntityType.Name) + "_" + keyProperty.Name;
        EdmProperty foreignKeyColumn = this.MapTableColumn(keyProperty, columnName, false);
        dependentTable.AddColumn(foreignKeyColumn);
        if (isPrimaryKeyColumn)
          dependentTable.AddKeyMember((EdmMember) foreignKeyColumn);
        foreignKeyColumn.Nullable = associationEndMapping.AssociationEnd.IsOptional() || associationEndMapping.AssociationEnd.IsRequired() && dependentEntityType.BaseType != null;
        foreignKeyColumn.StoreGeneratedPattern = StoreGeneratedPattern.None;
        yield return foreignKeyColumn;
        associationEndMapping.AddPropertyMapping(new ScalarPropertyMapping(keyProperty, foreignKeyColumn));
        if (foreignKeyColumn.Nullable)
          associationSetMapping.AddCondition((ConditionPropertyMapping) new IsNullConditionMapping(foreignKeyColumn, false));
      }
    }
  }
}
