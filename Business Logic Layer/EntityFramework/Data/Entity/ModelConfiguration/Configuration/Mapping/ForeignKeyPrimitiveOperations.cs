// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Mapping.ForeignKeyPrimitiveOperations
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration.Mapping
{
  internal static class ForeignKeyPrimitiveOperations
  {
    public static void UpdatePrincipalTables(
      DbDatabaseMapping databaseMapping,
      EntityType entityType,
      EntityType fromTable,
      EntityType toTable,
      bool isMappingAnyInheritedProperty)
    {
      if (fromTable == toTable)
        return;
      ForeignKeyPrimitiveOperations.UpdatePrincipalTables(databaseMapping, toTable, entityType, false);
      if (!isMappingAnyInheritedProperty)
        return;
      ForeignKeyPrimitiveOperations.UpdatePrincipalTables(databaseMapping, toTable, (EntityType) entityType.BaseType, true);
    }

    private static void UpdatePrincipalTables(
      DbDatabaseMapping databaseMapping,
      EntityType toTable,
      EntityType entityType,
      bool removeFks)
    {
      foreach (AssociationType associationType in databaseMapping.Model.AssociationTypes.Where<AssociationType>((Func<AssociationType, bool>) (at =>
      {
        if (!at.SourceEnd.GetEntityType().Equals((object) entityType))
          return at.TargetEnd.GetEntityType().Equals((object) entityType);
        return true;
      })))
        ForeignKeyPrimitiveOperations.UpdatePrincipalTables(databaseMapping, toTable, removeFks, associationType, entityType);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static void UpdatePrincipalTables(
      DbDatabaseMapping databaseMapping,
      EntityType toTable,
      bool removeFks,
      AssociationType associationType,
      EntityType et)
    {
      List<AssociationEndMember> associationEndMemberList = new List<AssociationEndMember>();
      AssociationEndMember principalEnd;
      AssociationEndMember dependentEnd;
      if (associationType.TryGuessPrincipalAndDependentEnds(out principalEnd, out dependentEnd))
        associationEndMemberList.Add(principalEnd);
      else if (associationType.SourceEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many && associationType.TargetEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many)
      {
        associationEndMemberList.Add(associationType.SourceEnd);
        associationEndMemberList.Add(associationType.TargetEnd);
      }
      else
        associationEndMemberList.Add(associationType.SourceEnd);
      foreach (AssociationEndMember associationEnd in associationEndMemberList)
      {
        if (associationEnd.GetEntityType() == et)
        {
          IEnumerable<KeyValuePair<EntityType, IEnumerable<EdmProperty>>> keyValuePairs;
          if (associationType.Constraint != null)
          {
            EntityType entityType = associationType.GetOtherEnd(associationEnd).GetEntityType();
            keyValuePairs = databaseMapping.Model.GetSelfAndAllDerivedTypes(entityType).Select<EntityType, EntityTypeMapping>((Func<EntityType, EntityTypeMapping>) (t => databaseMapping.GetEntityTypeMapping(t))).Where<EntityTypeMapping>((Func<EntityTypeMapping, bool>) (dm => dm != null)).SelectMany<EntityTypeMapping, MappingFragment>((Func<EntityTypeMapping, IEnumerable<MappingFragment>>) (dm => dm.MappingFragments.Where<MappingFragment>((Func<MappingFragment, bool>) (tmf => associationType.Constraint.ToProperties.All<EdmProperty>((Func<EdmProperty, bool>) (p => tmf.ColumnMappings.Any<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => pm.PropertyPath.First<EdmProperty>() == p)))))))).Distinct<MappingFragment>((Func<MappingFragment, MappingFragment, bool>) ((f1, f2) => f1.Table == f2.Table)).Select<MappingFragment, KeyValuePair<EntityType, IEnumerable<EdmProperty>>>((Func<MappingFragment, KeyValuePair<EntityType, IEnumerable<EdmProperty>>>) (df => new KeyValuePair<EntityType, IEnumerable<EdmProperty>>(df.Table, df.ColumnMappings.Where<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (pm => associationType.Constraint.ToProperties.Contains(pm.PropertyPath.First<EdmProperty>()))).Select<ColumnMappingBuilder, EdmProperty>((Func<ColumnMappingBuilder, EdmProperty>) (pm => pm.ColumnProperty)))));
          }
          else
          {
            AssociationSetMapping associationSetMapping = databaseMapping.EntityContainerMappings.Single<EntityContainerMapping>().AssociationSetMappings.Single<AssociationSetMapping>((Func<AssociationSetMapping, bool>) (asm => asm.AssociationSet.ElementType == associationType));
            keyValuePairs = (IEnumerable<KeyValuePair<EntityType, IEnumerable<EdmProperty>>>) new KeyValuePair<EntityType, IEnumerable<EdmProperty>>[1]
            {
              new KeyValuePair<EntityType, IEnumerable<EdmProperty>>(associationSetMapping.Table, (associationSetMapping.SourceEndMapping.AssociationEnd == associationEnd ? (IEnumerable<ScalarPropertyMapping>) associationSetMapping.SourceEndMapping.PropertyMappings : (IEnumerable<ScalarPropertyMapping>) associationSetMapping.TargetEndMapping.PropertyMappings).Select<ScalarPropertyMapping, EdmProperty>((Func<ScalarPropertyMapping, EdmProperty>) (pm => pm.Column)))
            };
          }
          foreach (KeyValuePair<EntityType, IEnumerable<EdmProperty>> keyValuePair in keyValuePairs)
          {
            KeyValuePair<EntityType, IEnumerable<EdmProperty>> tableInfo = keyValuePair;
            foreach (ForeignKeyBuilder foreignKeyBuilder in tableInfo.Key.ForeignKeyBuilders.Where<ForeignKeyBuilder>((Func<ForeignKeyBuilder, bool>) (fk => fk.DependentColumns.SequenceEqual<EdmProperty>(tableInfo.Value))).ToArray<ForeignKeyBuilder>())
            {
              if (removeFks)
                tableInfo.Key.RemoveForeignKey(foreignKeyBuilder);
              else if (foreignKeyBuilder.GetAssociationType() == null || foreignKeyBuilder.GetAssociationType() == associationType)
                foreignKeyBuilder.PrincipalTable = toTable;
            }
          }
        }
      }
    }

    private static void MoveForeignKeyConstraint(
      EntityType fromTable,
      EntityType toTable,
      ForeignKeyBuilder fk)
    {
      fromTable.RemoveForeignKey(fk);
      if (fk.PrincipalTable == toTable && fk.DependentColumns.All<EdmProperty>((Func<EdmProperty, bool>) (c => c.IsPrimaryKeyColumn)))
        return;
      IList<EdmProperty> dependentColumns = ForeignKeyPrimitiveOperations.GetDependentColumns((IEnumerable<EdmProperty>) fk.DependentColumns.ToArray<EdmProperty>(), (IEnumerable<EdmProperty>) toTable.Properties);
      if (ForeignKeyPrimitiveOperations.ContainsEquivalentForeignKey(toTable, fk.PrincipalTable, (IEnumerable<EdmProperty>) dependentColumns))
        return;
      toTable.AddForeignKey(fk);
      fk.DependentColumns = (IEnumerable<EdmProperty>) dependentColumns;
    }

    private static void CopyForeignKeyConstraint(
      EdmModel database,
      EntityType toTable,
      ForeignKeyBuilder fk,
      Func<EdmProperty, EdmProperty> selector = null)
    {
      ForeignKeyBuilder foreignKeyBuilder = new ForeignKeyBuilder(database, ((IEnumerable<INamedDataModelItem>) database.EntityTypes.SelectMany<EntityType, ForeignKeyBuilder>((Func<EntityType, IEnumerable<ForeignKeyBuilder>>) (t => t.ForeignKeyBuilders))).UniquifyName(fk.Name))
      {
        PrincipalTable = fk.PrincipalTable,
        DeleteAction = fk.DeleteAction
      };
      foreignKeyBuilder.SetPreferredName(fk.Name);
      IList<EdmProperty> dependentColumns = ForeignKeyPrimitiveOperations.GetDependentColumns(selector != null ? fk.DependentColumns.Select<EdmProperty, EdmProperty>(selector) : fk.DependentColumns, (IEnumerable<EdmProperty>) toTable.Properties);
      if (ForeignKeyPrimitiveOperations.ContainsEquivalentForeignKey(toTable, foreignKeyBuilder.PrincipalTable, (IEnumerable<EdmProperty>) dependentColumns))
        return;
      toTable.AddForeignKey(foreignKeyBuilder);
      foreignKeyBuilder.DependentColumns = (IEnumerable<EdmProperty>) dependentColumns;
    }

    private static bool ContainsEquivalentForeignKey(
      EntityType dependentTable,
      EntityType principalTable,
      IEnumerable<EdmProperty> columns)
    {
      return dependentTable.ForeignKeyBuilders.Any<ForeignKeyBuilder>((Func<ForeignKeyBuilder, bool>) (fk =>
      {
        if (fk.PrincipalTable == principalTable)
          return fk.DependentColumns.SequenceEqual<EdmProperty>(columns);
        return false;
      }));
    }

    private static IList<EdmProperty> GetDependentColumns(
      IEnumerable<EdmProperty> sourceColumns,
      IEnumerable<EdmProperty> destinationColumns)
    {
      return (IList<EdmProperty>) sourceColumns.Select<EdmProperty, EdmProperty>((Func<EdmProperty, EdmProperty>) (sc => destinationColumns.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (dc => string.Equals(dc.Name, sc.Name, StringComparison.Ordinal))) ?? destinationColumns.Single<EdmProperty>((Func<EdmProperty, bool>) (dc => string.Equals(dc.GetUnpreferredUniqueName(), sc.Name, StringComparison.Ordinal))))).ToList<EdmProperty>();
    }

    private static IEnumerable<ForeignKeyBuilder> FindAllForeignKeyConstraintsForColumn(
      EntityType fromTable,
      EntityType toTable,
      EdmProperty column)
    {
      return fromTable.ForeignKeyBuilders.Where<ForeignKeyBuilder>((Func<ForeignKeyBuilder, bool>) (fk =>
      {
        if (fk.DependentColumns.Contains<EdmProperty>(column))
          return fk.DependentColumns.All<EdmProperty>((Func<EdmProperty, bool>) (c => toTable.Properties.Any<EdmProperty>((Func<EdmProperty, bool>) (nc =>
          {
            if (!string.Equals(nc.Name, c.Name, StringComparison.Ordinal))
              return string.Equals(nc.GetUnpreferredUniqueName(), c.Name, StringComparison.Ordinal);
            return true;
          }))));
        return false;
      }));
    }

    public static void CopyAllForeignKeyConstraintsForColumn(
      EdmModel database,
      EntityType fromTable,
      EntityType toTable,
      EdmProperty column,
      EdmProperty movedColumn)
    {
      ((IEnumerable<ForeignKeyBuilder>) ForeignKeyPrimitiveOperations.FindAllForeignKeyConstraintsForColumn(fromTable, toTable, column).ToArray<ForeignKeyBuilder>()).Each<ForeignKeyBuilder>((Action<ForeignKeyBuilder>) (fk => ForeignKeyPrimitiveOperations.CopyForeignKeyConstraint(database, toTable, fk, (Func<EdmProperty, EdmProperty>) (c =>
      {
        if (c != column)
          return c;
        return movedColumn;
      }))));
    }

    public static void MoveAllDeclaredForeignKeyConstraintsForPrimaryKeyColumns(
      EntityType entityType,
      EntityType fromTable,
      EntityType toTable)
    {
      foreach (EdmProperty keyProperty in fromTable.KeyProperties)
        ((IEnumerable<ForeignKeyBuilder>) ForeignKeyPrimitiveOperations.FindAllForeignKeyConstraintsForColumn(fromTable, toTable, keyProperty).ToArray<ForeignKeyBuilder>()).Each<ForeignKeyBuilder>((Action<ForeignKeyBuilder>) (fk =>
        {
          AssociationType associationType = fk.GetAssociationType();
          if (associationType == null || associationType.Constraint.ToRole.GetEntityType() != entityType || fk.GetIsTypeConstraint())
            return;
          ForeignKeyPrimitiveOperations.MoveForeignKeyConstraint(fromTable, toTable, fk);
        }));
    }

    public static void CopyAllForeignKeyConstraintsForPrimaryKeyColumns(
      EdmModel database,
      EntityType fromTable,
      EntityType toTable)
    {
      foreach (EdmProperty keyProperty in fromTable.KeyProperties)
        ((IEnumerable<ForeignKeyBuilder>) ForeignKeyPrimitiveOperations.FindAllForeignKeyConstraintsForColumn(fromTable, toTable, keyProperty).ToArray<ForeignKeyBuilder>()).Each<ForeignKeyBuilder>((Action<ForeignKeyBuilder>) (fk =>
        {
          if (fk.GetIsTypeConstraint())
            return;
          ForeignKeyPrimitiveOperations.CopyForeignKeyConstraint(database, toTable, fk, (Func<EdmProperty, EdmProperty>) null);
        }));
    }

    public static void MoveAllForeignKeyConstraintsForColumn(
      EntityType fromTable,
      EntityType toTable,
      EdmProperty column)
    {
      ((IEnumerable<ForeignKeyBuilder>) ForeignKeyPrimitiveOperations.FindAllForeignKeyConstraintsForColumn(fromTable, toTable, column).ToArray<ForeignKeyBuilder>()).Each<ForeignKeyBuilder>((Action<ForeignKeyBuilder>) (fk => ForeignKeyPrimitiveOperations.MoveForeignKeyConstraint(fromTable, toTable, fk)));
    }

    public static void RemoveAllForeignKeyConstraintsForColumn(
      EntityType table,
      EdmProperty column,
      DbDatabaseMapping databaseMapping)
    {
      ((IEnumerable<ForeignKeyBuilder>) table.ForeignKeyBuilders.Where<ForeignKeyBuilder>((Func<ForeignKeyBuilder, bool>) (fk => fk.DependentColumns.Contains<EdmProperty>(column))).ToArray<ForeignKeyBuilder>()).Each<ForeignKeyBuilder>((Action<ForeignKeyBuilder>) (fk =>
      {
        table.RemoveForeignKey(fk);
        ForeignKeyBuilder fk1 = databaseMapping.Database.EntityTypes.SelectMany<EntityType, ForeignKeyBuilder>((Func<EntityType, IEnumerable<ForeignKeyBuilder>>) (t => t.ForeignKeyBuilders)).SingleOrDefault<ForeignKeyBuilder>((Func<ForeignKeyBuilder, bool>) (fk2 => object.Equals((object) fk2.GetPreferredName(), (object) fk.Name)));
        if (fk1 == null)
          return;
        fk1.Name = fk1.GetPreferredName();
      }));
    }
  }
}
