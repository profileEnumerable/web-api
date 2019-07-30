// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Resources.Strings
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.CodeDom.Compiler;

namespace System.Data.Entity.Resources
{
  [GeneratedCode("Resources.tt", "1.0.0.0")]
  internal static class Strings
  {
    internal static string AutomaticMigration
    {
      get
      {
        return EntityRes.GetString(nameof (AutomaticMigration));
      }
    }

    internal static string BootstrapMigration
    {
      get
      {
        return EntityRes.GetString(nameof (BootstrapMigration));
      }
    }

    internal static string InitialCreate
    {
      get
      {
        return EntityRes.GetString(nameof (InitialCreate));
      }
    }

    internal static string AutomaticDataLoss
    {
      get
      {
        return EntityRes.GetString(nameof (AutomaticDataLoss));
      }
    }

    internal static string LoggingAutoMigrate(object p0)
    {
      return EntityRes.GetString(nameof (LoggingAutoMigrate), p0);
    }

    internal static string LoggingRevertAutoMigrate(object p0)
    {
      return EntityRes.GetString(nameof (LoggingRevertAutoMigrate), p0);
    }

    internal static string LoggingApplyMigration(object p0)
    {
      return EntityRes.GetString(nameof (LoggingApplyMigration), p0);
    }

    internal static string LoggingRevertMigration(object p0)
    {
      return EntityRes.GetString(nameof (LoggingRevertMigration), p0);
    }

    internal static string LoggingSeedingDatabase
    {
      get
      {
        return EntityRes.GetString(nameof (LoggingSeedingDatabase));
      }
    }

    internal static string LoggingPendingMigrations(object p0, object p1)
    {
      return EntityRes.GetString(nameof (LoggingPendingMigrations), p0, p1);
    }

    internal static string LoggingPendingMigrationsDown(object p0, object p1)
    {
      return EntityRes.GetString(nameof (LoggingPendingMigrationsDown), p0, p1);
    }

    internal static string LoggingNoExplicitMigrations
    {
      get
      {
        return EntityRes.GetString(nameof (LoggingNoExplicitMigrations));
      }
    }

    internal static string LoggingAlreadyAtTarget(object p0)
    {
      return EntityRes.GetString(nameof (LoggingAlreadyAtTarget), p0);
    }

    internal static string LoggingTargetDatabase(object p0)
    {
      return EntityRes.GetString(nameof (LoggingTargetDatabase), p0);
    }

    internal static string LoggingTargetDatabaseFormat(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (LoggingTargetDatabaseFormat), p0, p1, p2, p3);
    }

    internal static string LoggingExplicit
    {
      get
      {
        return EntityRes.GetString(nameof (LoggingExplicit));
      }
    }

    internal static string UpgradingHistoryTable
    {
      get
      {
        return EntityRes.GetString(nameof (UpgradingHistoryTable));
      }
    }

    internal static string MetadataOutOfDate
    {
      get
      {
        return EntityRes.GetString(nameof (MetadataOutOfDate));
      }
    }

    internal static string MigrationNotFound(object p0)
    {
      return EntityRes.GetString(nameof (MigrationNotFound), p0);
    }

    internal static string PartialFkOperation(object p0, object p1)
    {
      return EntityRes.GetString(nameof (PartialFkOperation), p0, p1);
    }

    internal static string AutoNotValidTarget(object p0)
    {
      return EntityRes.GetString(nameof (AutoNotValidTarget), p0);
    }

    internal static string AutoNotValidForScriptWindows(object p0)
    {
      return EntityRes.GetString(nameof (AutoNotValidForScriptWindows), p0);
    }

    internal static string ContextNotConstructible(object p0)
    {
      return EntityRes.GetString(nameof (ContextNotConstructible), p0);
    }

    internal static string AmbiguousMigrationName(object p0)
    {
      return EntityRes.GetString(nameof (AmbiguousMigrationName), p0);
    }

    internal static string AutomaticDisabledException
    {
      get
      {
        return EntityRes.GetString(nameof (AutomaticDisabledException));
      }
    }

    internal static string DownScriptWindowsNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (DownScriptWindowsNotSupported));
      }
    }

    internal static string AssemblyMigrator_NoConfigurationWithName(object p0, object p1)
    {
      return EntityRes.GetString(nameof (AssemblyMigrator_NoConfigurationWithName), p0, p1);
    }

    internal static string AssemblyMigrator_MultipleConfigurationsWithName(object p0, object p1)
    {
      return EntityRes.GetString(nameof (AssemblyMigrator_MultipleConfigurationsWithName), p0, p1);
    }

    internal static string AssemblyMigrator_NoConfiguration(object p0)
    {
      return EntityRes.GetString(nameof (AssemblyMigrator_NoConfiguration), p0);
    }

    internal static string AssemblyMigrator_MultipleConfigurations(object p0)
    {
      return EntityRes.GetString(nameof (AssemblyMigrator_MultipleConfigurations), p0);
    }

    internal static string MigrationsNamespaceNotUnderRootNamespace(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MigrationsNamespaceNotUnderRootNamespace), p0, p1);
    }

    internal static string UnableToDispatchAddOrUpdate(object p0)
    {
      return EntityRes.GetString(nameof (UnableToDispatchAddOrUpdate), p0);
    }

    internal static string NoSqlGeneratorForProvider(object p0)
    {
      return EntityRes.GetString(nameof (NoSqlGeneratorForProvider), p0);
    }

    internal static string ToolingFacade_AssemblyNotFound(object p0)
    {
      return EntityRes.GetString(nameof (ToolingFacade_AssemblyNotFound), p0);
    }

    internal static string ArgumentIsNullOrWhitespace(object p0)
    {
      return EntityRes.GetString(nameof (ArgumentIsNullOrWhitespace), p0);
    }

    internal static string EntityTypeConfigurationMismatch(object p0)
    {
      return EntityRes.GetString(nameof (EntityTypeConfigurationMismatch), p0);
    }

    internal static string ComplexTypeConfigurationMismatch(object p0)
    {
      return EntityRes.GetString(nameof (ComplexTypeConfigurationMismatch), p0);
    }

    internal static string KeyPropertyNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (KeyPropertyNotFound), p0, p1);
    }

    internal static string ForeignKeyPropertyNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ForeignKeyPropertyNotFound), p0, p1);
    }

    internal static string PropertyNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (PropertyNotFound), p0, p1);
    }

    internal static string NavigationPropertyNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NavigationPropertyNotFound), p0, p1);
    }

    internal static string InvalidPropertyExpression(object p0)
    {
      return EntityRes.GetString(nameof (InvalidPropertyExpression), p0);
    }

    internal static string InvalidComplexPropertyExpression(object p0)
    {
      return EntityRes.GetString(nameof (InvalidComplexPropertyExpression), p0);
    }

    internal static string InvalidPropertiesExpression(object p0)
    {
      return EntityRes.GetString(nameof (InvalidPropertiesExpression), p0);
    }

    internal static string InvalidComplexPropertiesExpression(object p0)
    {
      return EntityRes.GetString(nameof (InvalidComplexPropertiesExpression), p0);
    }

    internal static string DuplicateStructuralTypeConfiguration(object p0)
    {
      return EntityRes.GetString(nameof (DuplicateStructuralTypeConfiguration), p0);
    }

    internal static string ConflictingPropertyConfiguration(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ConflictingPropertyConfiguration), p0, p1, p2);
    }

    internal static string ConflictingTypeAnnotation(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (ConflictingTypeAnnotation), p0, p1, p2, p3);
    }

    internal static string ConflictingColumnConfiguration(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ConflictingColumnConfiguration), p0, p1, p2);
    }

    internal static string ConflictingConfigurationValue(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (ConflictingConfigurationValue), p0, p1, p2, p3);
    }

    internal static string ConflictingAnnotationValue(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ConflictingAnnotationValue), p0, p1, p2);
    }

    internal static string ConflictingIndexAttributeProperty(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ConflictingIndexAttributeProperty), p0, p1, p2);
    }

    internal static string ConflictingIndexAttribute(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConflictingIndexAttribute), p0, p1);
    }

    internal static string ConflictingIndexAttributesOnProperty(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (ConflictingIndexAttributesOnProperty), p0, p1, p2, p3);
    }

    internal static string IncompatibleTypes(object p0, object p1)
    {
      return EntityRes.GetString(nameof (IncompatibleTypes), p0, p1);
    }

    internal static string AnnotationSerializeWrongType(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (AnnotationSerializeWrongType), p0, p1, p2);
    }

    internal static string AnnotationSerializeBadFormat(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (AnnotationSerializeBadFormat), p0, p1, p2);
    }

    internal static string ConflictWhenConsolidating(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ConflictWhenConsolidating), p0, p1, p2);
    }

    internal static string OrderConflictWhenConsolidating(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (OrderConflictWhenConsolidating), p0, p1, p2, p3, p4);
    }

    internal static string CodeFirstInvalidComplexType(object p0)
    {
      return EntityRes.GetString(nameof (CodeFirstInvalidComplexType), p0);
    }

    internal static string InvalidEntityType(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEntityType), p0);
    }

    internal static string SimpleNameCollision(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (SimpleNameCollision), p0, p1, p2);
    }

    internal static string NavigationInverseItself(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NavigationInverseItself), p0, p1);
    }

    internal static string ConflictingConstraint(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConflictingConstraint), p0, p1);
    }

    internal static string ConflictingInferredColumnType(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ConflictingInferredColumnType), p0, p1, p2);
    }

    internal static string ConflictingMapping(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConflictingMapping), p0, p1);
    }

    internal static string ConflictingCascadeDeleteOperation(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConflictingCascadeDeleteOperation), p0, p1);
    }

    internal static string ConflictingMultiplicities(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConflictingMultiplicities), p0, p1);
    }

    internal static string MaxLengthAttributeConvention_InvalidMaxLength(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MaxLengthAttributeConvention_InvalidMaxLength), p0, p1);
    }

    internal static string StringLengthAttributeConvention_InvalidMaximumLength(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (StringLengthAttributeConvention_InvalidMaximumLength), p0, p1);
    }

    internal static string ModelGeneration_UnableToDetermineKeyOrder(object p0)
    {
      return EntityRes.GetString(nameof (ModelGeneration_UnableToDetermineKeyOrder), p0);
    }

    internal static string ForeignKeyAttributeConvention_EmptyKey(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ForeignKeyAttributeConvention_EmptyKey), p0, p1);
    }

    internal static string ForeignKeyAttributeConvention_InvalidKey(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (ForeignKeyAttributeConvention_InvalidKey), p0, p1, p2, p3);
    }

    internal static string ForeignKeyAttributeConvention_InvalidNavigationProperty(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ForeignKeyAttributeConvention_InvalidNavigationProperty), p0, p1, p2);
    }

    internal static string ForeignKeyAttributeConvention_OrderRequired(object p0)
    {
      return EntityRes.GetString(nameof (ForeignKeyAttributeConvention_OrderRequired), p0);
    }

    internal static string InversePropertyAttributeConvention_PropertyNotFound(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (InversePropertyAttributeConvention_PropertyNotFound), p0, p1, p2, p3);
    }

    internal static string InversePropertyAttributeConvention_SelfInverseDetected(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (InversePropertyAttributeConvention_SelfInverseDetected), p0, p1);
    }

    internal static string ValidationHeader
    {
      get
      {
        return EntityRes.GetString(nameof (ValidationHeader));
      }
    }

    internal static string ValidationItemFormat(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ValidationItemFormat), p0, p1, p2);
    }

    internal static string KeyRegisteredOnDerivedType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (KeyRegisteredOnDerivedType), p0, p1);
    }

    internal static string InvalidTableMapping(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidTableMapping), p0, p1);
    }

    internal static string InvalidTableMapping_NoTableName(object p0)
    {
      return EntityRes.GetString(nameof (InvalidTableMapping_NoTableName), p0);
    }

    internal static string InvalidChainedMappingSyntax(object p0)
    {
      return EntityRes.GetString(nameof (InvalidChainedMappingSyntax), p0);
    }

    internal static string InvalidNotNullCondition(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidNotNullCondition), p0, p1);
    }

    internal static string InvalidDiscriminatorType(object p0)
    {
      return EntityRes.GetString(nameof (InvalidDiscriminatorType), p0);
    }

    internal static string ConventionNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConventionNotFound), p0, p1);
    }

    internal static string InvalidEntitySplittingProperties(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEntitySplittingProperties), p0);
    }

    internal static string ProviderNameNotFound(object p0)
    {
      return EntityRes.GetString(nameof (ProviderNameNotFound), p0);
    }

    internal static string ProviderNotFound(object p0)
    {
      return EntityRes.GetString(nameof (ProviderNotFound), p0);
    }

    internal static string InvalidDatabaseName(object p0)
    {
      return EntityRes.GetString(nameof (InvalidDatabaseName), p0);
    }

    internal static string EntityMappingConfiguration_DuplicateMapInheritedProperties(object p0)
    {
      return EntityRes.GetString(nameof (EntityMappingConfiguration_DuplicateMapInheritedProperties), p0);
    }

    internal static string EntityMappingConfiguration_DuplicateMappedProperties(object p0)
    {
      return EntityRes.GetString(nameof (EntityMappingConfiguration_DuplicateMappedProperties), p0);
    }

    internal static string EntityMappingConfiguration_DuplicateMappedProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityMappingConfiguration_DuplicateMappedProperty), p0, p1);
    }

    internal static string EntityMappingConfiguration_CannotMapIgnoredProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityMappingConfiguration_CannotMapIgnoredProperty), p0, p1);
    }

    internal static string EntityMappingConfiguration_InvalidTableSharing(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EntityMappingConfiguration_InvalidTableSharing), p0, p1, p2);
    }

    internal static string EntityMappingConfiguration_TPCWithIAsOnNonLeafType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EntityMappingConfiguration_TPCWithIAsOnNonLeafType), p0, p1, p2);
    }

    internal static string CannotIgnoreMappedBaseProperty(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (CannotIgnoreMappedBaseProperty), p0, p1, p2);
    }

    internal static string ModelBuilder_KeyPropertiesMustBePrimitive(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ModelBuilder_KeyPropertiesMustBePrimitive), p0, p1);
    }

    internal static string TableNotFound(object p0)
    {
      return EntityRes.GetString(nameof (TableNotFound), p0);
    }

    internal static string IncorrectColumnCount(object p0)
    {
      return EntityRes.GetString(nameof (IncorrectColumnCount), p0);
    }

    internal static string BadKeyNameForAnnotation(object p0, object p1)
    {
      return EntityRes.GetString(nameof (BadKeyNameForAnnotation), p0, p1);
    }

    internal static string BadAnnotationName(object p0)
    {
      return EntityRes.GetString(nameof (BadAnnotationName), p0);
    }

    internal static string CircularComplexTypeHierarchy
    {
      get
      {
        return EntityRes.GetString(nameof (CircularComplexTypeHierarchy));
      }
    }

    internal static string UnableToDeterminePrincipal(object p0, object p1)
    {
      return EntityRes.GetString(nameof (UnableToDeterminePrincipal), p0, p1);
    }

    internal static string UnmappedAbstractType(object p0)
    {
      return EntityRes.GetString(nameof (UnmappedAbstractType), p0);
    }

    internal static string UnsupportedHybridInheritanceMapping(object p0)
    {
      return EntityRes.GetString(nameof (UnsupportedHybridInheritanceMapping), p0);
    }

    internal static string OrphanedConfiguredTableDetected(object p0)
    {
      return EntityRes.GetString(nameof (OrphanedConfiguredTableDetected), p0);
    }

    internal static string BadTphMappingToSharedColumn(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5,
      object p6)
    {
      return EntityRes.GetString(nameof (BadTphMappingToSharedColumn), p0, p1, p2, p3, p4, p5, p6);
    }

    internal static string DuplicateConfiguredColumnOrder(object p0)
    {
      return EntityRes.GetString(nameof (DuplicateConfiguredColumnOrder), p0);
    }

    internal static string UnsupportedUseOfV3Type(object p0, object p1)
    {
      return EntityRes.GetString(nameof (UnsupportedUseOfV3Type), p0, p1);
    }

    internal static string MultiplePropertiesMatchedAsKeys(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MultiplePropertiesMatchedAsKeys), p0, p1);
    }

    internal static string FailedToGetProviderInformation
    {
      get
      {
        return EntityRes.GetString(nameof (FailedToGetProviderInformation));
      }
    }

    internal static string DbPropertyEntry_CannotGetCurrentValue(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyEntry_CannotGetCurrentValue), p0, p1);
    }

    internal static string DbPropertyEntry_CannotSetCurrentValue(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyEntry_CannotSetCurrentValue), p0, p1);
    }

    internal static string DbPropertyEntry_NotSupportedForDetached(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (DbPropertyEntry_NotSupportedForDetached), p0, p1, p2);
    }

    internal static string DbPropertyEntry_SettingEntityRefNotSupported(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (DbPropertyEntry_SettingEntityRefNotSupported), p0, p1, p2);
    }

    internal static string DbPropertyEntry_NotSupportedForPropertiesNotInTheModel(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (DbPropertyEntry_NotSupportedForPropertiesNotInTheModel), p0, p1, p2);
    }

    internal static string DbEntityEntry_NotSupportedForDetached(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_NotSupportedForDetached), p0, p1);
    }

    internal static string DbSet_BadTypeForAddAttachRemove(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (DbSet_BadTypeForAddAttachRemove), p0, p1, p2);
    }

    internal static string DbSet_BadTypeForCreate(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbSet_BadTypeForCreate), p0, p1);
    }

    internal static string DbEntity_BadTypeForCast(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (DbEntity_BadTypeForCast), p0, p1, p2);
    }

    internal static string DbMember_BadTypeForCast(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (DbMember_BadTypeForCast), p0, p1, p2, p3, p4);
    }

    internal static string DbEntityEntry_UsedReferenceForCollectionProp(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_UsedReferenceForCollectionProp), p0, p1);
    }

    internal static string DbEntityEntry_UsedCollectionForReferenceProp(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_UsedCollectionForReferenceProp), p0, p1);
    }

    internal static string DbEntityEntry_NotANavigationProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_NotANavigationProperty), p0, p1);
    }

    internal static string DbEntityEntry_NotAScalarProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_NotAScalarProperty), p0, p1);
    }

    internal static string DbEntityEntry_NotAComplexProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_NotAComplexProperty), p0, p1);
    }

    internal static string DbEntityEntry_NotAProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_NotAProperty), p0, p1);
    }

    internal static string DbEntityEntry_DottedPartNotComplex(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_DottedPartNotComplex), p0, p1, p2);
    }

    internal static string DbEntityEntry_DottedPathMustBeProperty(object p0)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_DottedPathMustBeProperty), p0);
    }

    internal static string DbEntityEntry_WrongGenericForNavProp(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_WrongGenericForNavProp), p0, p1, p2, p3);
    }

    internal static string DbEntityEntry_WrongGenericForCollectionNavProp(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_WrongGenericForCollectionNavProp), p0, p1, p2, p3);
    }

    internal static string DbEntityEntry_WrongGenericForProp(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_WrongGenericForProp), p0, p1, p2, p3);
    }

    internal static string DbEntityEntry_BadPropertyExpression(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbEntityEntry_BadPropertyExpression), p0, p1);
    }

    internal static string DbContext_IndependentAssociationUpdateException
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_IndependentAssociationUpdateException));
      }
    }

    internal static string DbPropertyValues_CannotGetValuesForState(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_CannotGetValuesForState), p0, p1);
    }

    internal static string DbPropertyValues_CannotSetNullValue(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_CannotSetNullValue), p0, p1, p2);
    }

    internal static string DbPropertyValues_CannotGetStoreValuesWhenComplexPropertyIsNull(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_CannotGetStoreValuesWhenComplexPropertyIsNull), p0, p1);
    }

    internal static string DbPropertyValues_WrongTypeForAssignment(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_WrongTypeForAssignment), p0, p1, p2, p3);
    }

    internal static string DbPropertyValues_PropertyValueNamesAreReadonly
    {
      get
      {
        return EntityRes.GetString(nameof (DbPropertyValues_PropertyValueNamesAreReadonly));
      }
    }

    internal static string DbPropertyValues_PropertyDoesNotExist(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_PropertyDoesNotExist), p0, p1);
    }

    internal static string DbPropertyValues_AttemptToSetValuesFromWrongObject(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_AttemptToSetValuesFromWrongObject), p0, p1);
    }

    internal static string DbPropertyValues_AttemptToSetValuesFromWrongType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_AttemptToSetValuesFromWrongType), p0, p1);
    }

    internal static string DbPropertyValues_AttemptToSetNonValuesOnComplexProperty
    {
      get
      {
        return EntityRes.GetString(nameof (DbPropertyValues_AttemptToSetNonValuesOnComplexProperty));
      }
    }

    internal static string DbPropertyValues_ComplexObjectCannotBeNull(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_ComplexObjectCannotBeNull), p0, p1);
    }

    internal static string DbPropertyValues_NestedPropertyValuesNull(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_NestedPropertyValuesNull), p0, p1);
    }

    internal static string DbPropertyValues_CannotSetPropertyOnNullCurrentValue(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_CannotSetPropertyOnNullCurrentValue), p0, p1);
    }

    internal static string DbPropertyValues_CannotSetPropertyOnNullOriginalValue(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (DbPropertyValues_CannotSetPropertyOnNullOriginalValue), p0, p1);
    }

    internal static string DatabaseInitializationStrategy_ModelMismatch(object p0)
    {
      return EntityRes.GetString(nameof (DatabaseInitializationStrategy_ModelMismatch), p0);
    }

    internal static string Database_DatabaseAlreadyExists(object p0)
    {
      return EntityRes.GetString(nameof (Database_DatabaseAlreadyExists), p0);
    }

    internal static string Database_NonCodeFirstCompatibilityCheck
    {
      get
      {
        return EntityRes.GetString(nameof (Database_NonCodeFirstCompatibilityCheck));
      }
    }

    internal static string Database_NoDatabaseMetadata
    {
      get
      {
        return EntityRes.GetString(nameof (Database_NoDatabaseMetadata));
      }
    }

    internal static string Database_BadLegacyInitializerEntry(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Database_BadLegacyInitializerEntry), p0, p1);
    }

    internal static string Database_InitializeFromLegacyConfigFailed(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Database_InitializeFromLegacyConfigFailed), p0, p1);
    }

    internal static string Database_InitializeFromConfigFailed(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Database_InitializeFromConfigFailed), p0, p1);
    }

    internal static string ContextConfiguredMultipleTimes(object p0)
    {
      return EntityRes.GetString(nameof (ContextConfiguredMultipleTimes), p0);
    }

    internal static string SetConnectionFactoryFromConfigFailed(object p0)
    {
      return EntityRes.GetString(nameof (SetConnectionFactoryFromConfigFailed), p0);
    }

    internal static string DbContext_ContextUsedInModelCreating
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_ContextUsedInModelCreating));
      }
    }

    internal static string DbContext_MESTNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_MESTNotSupported));
      }
    }

    internal static string DbContext_Disposed
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_Disposed));
      }
    }

    internal static string DbContext_ProviderReturnedNullConnection
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_ProviderReturnedNullConnection));
      }
    }

    internal static string DbContext_ProviderNameMissing(object p0)
    {
      return EntityRes.GetString(nameof (DbContext_ProviderNameMissing), p0);
    }

    internal static string DbContext_ConnectionFactoryReturnedNullConnection
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_ConnectionFactoryReturnedNullConnection));
      }
    }

    internal static string DbSet_WrongNumberOfKeyValuesPassed
    {
      get
      {
        return EntityRes.GetString(nameof (DbSet_WrongNumberOfKeyValuesPassed));
      }
    }

    internal static string DbSet_WrongKeyValueType
    {
      get
      {
        return EntityRes.GetString(nameof (DbSet_WrongKeyValueType));
      }
    }

    internal static string DbSet_WrongEntityTypeFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbSet_WrongEntityTypeFound), p0, p1);
    }

    internal static string DbSet_MultipleAddedEntitiesFound
    {
      get
      {
        return EntityRes.GetString(nameof (DbSet_MultipleAddedEntitiesFound));
      }
    }

    internal static string DbSet_DbSetUsedWithComplexType(object p0)
    {
      return EntityRes.GetString(nameof (DbSet_DbSetUsedWithComplexType), p0);
    }

    internal static string DbSet_PocoAndNonPocoMixedInSameAssembly(object p0)
    {
      return EntityRes.GetString(nameof (DbSet_PocoAndNonPocoMixedInSameAssembly), p0);
    }

    internal static string DbSet_EntityTypeNotInModel(object p0)
    {
      return EntityRes.GetString(nameof (DbSet_EntityTypeNotInModel), p0);
    }

    internal static string DbQuery_BindingToDbQueryNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (DbQuery_BindingToDbQueryNotSupported));
      }
    }

    internal static string DbExtensions_InvalidIncludePathExpression
    {
      get
      {
        return EntityRes.GetString(nameof (DbExtensions_InvalidIncludePathExpression));
      }
    }

    internal static string DbContext_ConnectionStringNotFound(object p0)
    {
      return EntityRes.GetString(nameof (DbContext_ConnectionStringNotFound), p0);
    }

    internal static string DbContext_ConnectionHasModel
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_ConnectionHasModel));
      }
    }

    internal static string DbCollectionEntry_CannotSetCollectionProp(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbCollectionEntry_CannotSetCollectionProp), p0, p1);
    }

    internal static string CodeFirstCachedMetadataWorkspace_SameModelDifferentProvidersNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (CodeFirstCachedMetadataWorkspace_SameModelDifferentProvidersNotSupported));
      }
    }

    internal static string Mapping_MESTNotSupported(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (Mapping_MESTNotSupported), p0, p1, p2);
    }

    internal static string DbModelBuilder_MissingRequiredCtor(object p0)
    {
      return EntityRes.GetString(nameof (DbModelBuilder_MissingRequiredCtor), p0);
    }

    internal static string DbEntityValidationException_ValidationFailed
    {
      get
      {
        return EntityRes.GetString(nameof (DbEntityValidationException_ValidationFailed));
      }
    }

    internal static string DbUnexpectedValidationException_ValidationAttribute(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbUnexpectedValidationException_ValidationAttribute), p0, p1);
    }

    internal static string DbUnexpectedValidationException_IValidatableObject(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbUnexpectedValidationException_IValidatableObject), p0, p1);
    }

    internal static string SqlConnectionFactory_MdfNotSupported(object p0)
    {
      return EntityRes.GetString(nameof (SqlConnectionFactory_MdfNotSupported), p0);
    }

    internal static string Database_InitializationException
    {
      get
      {
        return EntityRes.GetString(nameof (Database_InitializationException));
      }
    }

    internal static string EdmxWriter_EdmxFromObjectContextNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (EdmxWriter_EdmxFromObjectContextNotSupported));
      }
    }

    internal static string EdmxWriter_EdmxFromModelFirstNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (EdmxWriter_EdmxFromModelFirstNotSupported));
      }
    }

    internal static string UnintentionalCodeFirstException_Message
    {
      get
      {
        return EntityRes.GetString(nameof (UnintentionalCodeFirstException_Message));
      }
    }

    internal static string DbContextServices_MissingDefaultCtor(object p0)
    {
      return EntityRes.GetString(nameof (DbContextServices_MissingDefaultCtor), p0);
    }

    internal static string CannotCallGenericSetWithProxyType
    {
      get
      {
        return EntityRes.GetString(nameof (CannotCallGenericSetWithProxyType));
      }
    }

    internal static string EdmModel_Validator_Semantic_SystemNamespaceEncountered(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_SystemNamespaceEncountered), p0);
    }

    internal static string EdmModel_Validator_Semantic_SimilarRelationshipEnd(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_SimilarRelationshipEnd), p0, p1, p2, p3, p4);
    }

    internal static string EdmModel_Validator_Semantic_InvalidEntitySetNameReference(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidEntitySetNameReference), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_ConcurrencyRedefinedOnSubTypeOfEntitySetType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ConcurrencyRedefinedOnSubTypeOfEntitySetType), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_EntitySetTypeHasNoKeys(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EntitySetTypeHasNoKeys), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_DuplicateEndName(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicateEndName), p0);
    }

    internal static string EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidCollectionKindNotCollection(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidCollectionKindNotCollection), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidCollectionKindNotV1_1(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidCollectionKindNotV1_1), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidComplexTypeAbstract(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidComplexTypeAbstract), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidKeyNullablePart(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidKeyNullablePart), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_EntityKeyMustBeScalar(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EntityKeyMustBeScalar), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_KeyMissingOnEntityType(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_KeyMissingOnEntityType), p0);
    }

    internal static string EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame));
      }
    }

    internal static string EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation));
      }
    }

    internal static string EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate), p0);
    }

    internal static string EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleUpperBoundMustBeOne(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleUpperBoundMustBeOne), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNullableV1(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNullableV1), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV1(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV1), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV2(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV2), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidToPropertyInRelationshipConstraint(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidToPropertyInRelationshipConstraint), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeOne(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeOne), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeMany(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeMany), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint));
      }
    }

    internal static string EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint), p0, p1, p2, p3, p4);
    }

    internal static string EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraint(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraint), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_NullableComplexType(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NullableComplexType), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidPropertyType(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidPropertyType), p0);
    }

    internal static string EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName), p0);
    }

    internal static string EdmModel_Validator_Semantic_TypeNameAlreadyDefinedDuplicate(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_TypeNameAlreadyDefinedDuplicate), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_PropertyNameAlreadyDefinedDuplicate(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_PropertyNameAlreadyDefinedDuplicate), p0);
    }

    internal static string EdmModel_Validator_Semantic_CycleInTypeHierarchy(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_CycleInTypeHierarchy), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidPropertyType_V1_1(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidPropertyType_V1_1), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidPropertyType_V3(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidPropertyType_V3), p0);
    }

    internal static string EdmModel_Validator_Semantic_ComposableFunctionImportsNotSupportedForSchemaVersion
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ComposableFunctionImportsNotSupportedForSchemaVersion));
      }
    }

    internal static string EdmModel_Validator_Syntactic_MissingName
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_MissingName));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong), p0);
    }

    internal static string EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed(object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed), p0);
    }

    internal static string EdmModel_Validator_Syntactic_EdmAssociationType_AssocationEndMustNotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmAssociationType_AssocationEndMustNotBeNull));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmAssociationConstraint_DependentEndMustNotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmAssociationConstraint_DependentEndMustNotBeNull));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmAssociationConstraint_DependentPropertiesMustNotBeEmpty
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmAssociationConstraint_DependentPropertiesMustNotBeEmpty));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmNavigationProperty_AssocationMustNotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmNavigationProperty_AssocationMustNotBeNull));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmNavigationProperty_ResultEndMustNotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmNavigationProperty_ResultEndMustNotBeNull));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmAssociationEnd_EntityTypeMustNotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmAssociationEnd_EntityTypeMustNotBeNull));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmEntitySet_ElementTypeMustNotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmEntitySet_ElementTypeMustNotBeNull));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmAssociationSet_ElementTypeMustNotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmAssociationSet_ElementTypeMustNotBeNull));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmAssociationSet_SourceSetMustNotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmAssociationSet_SourceSetMustNotBeNull));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmAssociationSet_TargetSetMustNotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmAssociationSet_TargetSetMustNotBeNull));
      }
    }

    internal static string EdmModel_Validator_Syntactic_EdmTypeReferenceNotValid
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmTypeReferenceNotValid));
      }
    }

    internal static string MetadataItem_InvalidDataSpace(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MetadataItem_InvalidDataSpace), p0, p1);
    }

    internal static string EdmModel_AddItem_NonMatchingNamespace
    {
      get
      {
        return EntityRes.GetString(nameof (EdmModel_AddItem_NonMatchingNamespace));
      }
    }

    internal static string Serializer_OneNamespaceAndOneContainer
    {
      get
      {
        return EntityRes.GetString(nameof (Serializer_OneNamespaceAndOneContainer));
      }
    }

    internal static string MaxLengthAttribute_ValidationError(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MaxLengthAttribute_ValidationError), p0, p1);
    }

    internal static string MaxLengthAttribute_InvalidMaxLength
    {
      get
      {
        return EntityRes.GetString(nameof (MaxLengthAttribute_InvalidMaxLength));
      }
    }

    internal static string MinLengthAttribute_ValidationError(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MinLengthAttribute_ValidationError), p0, p1);
    }

    internal static string MinLengthAttribute_InvalidMinLength
    {
      get
      {
        return EntityRes.GetString(nameof (MinLengthAttribute_InvalidMinLength));
      }
    }

    internal static string DbConnectionInfo_ConnectionStringNotFound(object p0)
    {
      return EntityRes.GetString(nameof (DbConnectionInfo_ConnectionStringNotFound), p0);
    }

    internal static string EagerInternalContext_CannotSetConnectionInfo
    {
      get
      {
        return EntityRes.GetString(nameof (EagerInternalContext_CannotSetConnectionInfo));
      }
    }

    internal static string LazyInternalContext_CannotReplaceEfConnectionWithDbConnection
    {
      get
      {
        return EntityRes.GetString(nameof (LazyInternalContext_CannotReplaceEfConnectionWithDbConnection));
      }
    }

    internal static string LazyInternalContext_CannotReplaceDbConnectionWithEfConnection
    {
      get
      {
        return EntityRes.GetString(nameof (LazyInternalContext_CannotReplaceDbConnectionWithEfConnection));
      }
    }

    internal static string EntityKey_EntitySetDoesNotMatch(object p0)
    {
      return EntityRes.GetString(nameof (EntityKey_EntitySetDoesNotMatch), p0);
    }

    internal static string EntityKey_IncorrectNumberOfKeyValuePairs(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EntityKey_IncorrectNumberOfKeyValuePairs), p0, p1, p2);
    }

    internal static string EntityKey_IncorrectValueType(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (EntityKey_IncorrectValueType), p0, p1, p2);
    }

    internal static string EntityKey_NoCorrespondingOSpaceTypeForEnumKeyMember(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityKey_NoCorrespondingOSpaceTypeForEnumKeyMember), p0, p1);
    }

    internal static string EntityKey_MissingKeyValue(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityKey_MissingKeyValue), p0, p1);
    }

    internal static string EntityKey_NoNullsAllowedInKeyValuePairs
    {
      get
      {
        return EntityRes.GetString(nameof (EntityKey_NoNullsAllowedInKeyValuePairs));
      }
    }

    internal static string EntityKey_UnexpectedNull
    {
      get
      {
        return EntityRes.GetString(nameof (EntityKey_UnexpectedNull));
      }
    }

    internal static string EntityKey_DoesntMatchKeyOnEntity(object p0)
    {
      return EntityRes.GetString(nameof (EntityKey_DoesntMatchKeyOnEntity), p0);
    }

    internal static string EntityKey_EntityKeyMustHaveValues
    {
      get
      {
        return EntityRes.GetString(nameof (EntityKey_EntityKeyMustHaveValues));
      }
    }

    internal static string EntityKey_InvalidQualifiedEntitySetName
    {
      get
      {
        return EntityRes.GetString(nameof (EntityKey_InvalidQualifiedEntitySetName));
      }
    }

    internal static string EntityKey_MissingEntitySetName
    {
      get
      {
        return EntityRes.GetString(nameof (EntityKey_MissingEntitySetName));
      }
    }

    internal static string EntityKey_InvalidName(object p0)
    {
      return EntityRes.GetString(nameof (EntityKey_InvalidName), p0);
    }

    internal static string EntityKey_CannotChangeKey
    {
      get
      {
        return EntityRes.GetString(nameof (EntityKey_CannotChangeKey));
      }
    }

    internal static string EntityTypesDoNotAgree
    {
      get
      {
        return EntityRes.GetString(nameof (EntityTypesDoNotAgree));
      }
    }

    internal static string EntityKey_NullKeyValue(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityKey_NullKeyValue), p0, p1);
    }

    internal static string EdmMembersDefiningTypeDoNotAgreeWithMetadataType
    {
      get
      {
        return EntityRes.GetString(nameof (EdmMembersDefiningTypeDoNotAgreeWithMetadataType));
      }
    }

    internal static string CannotCallNoncomposableFunction(object p0)
    {
      return EntityRes.GetString(nameof (CannotCallNoncomposableFunction), p0);
    }

    internal static string EntityClient_ConnectionStringMissingInfo(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_ConnectionStringMissingInfo), p0);
    }

    internal static string EntityClient_ValueNotString
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ValueNotString));
      }
    }

    internal static string EntityClient_KeywordNotSupported(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_KeywordNotSupported), p0);
    }

    internal static string EntityClient_NoCommandText
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_NoCommandText));
      }
    }

    internal static string EntityClient_ConnectionStringNeededBeforeOperation
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ConnectionStringNeededBeforeOperation));
      }
    }

    internal static string EntityClient_ConnectionNotOpen
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ConnectionNotOpen));
      }
    }

    internal static string EntityClient_DuplicateParameterNames(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_DuplicateParameterNames), p0);
    }

    internal static string EntityClient_NoConnectionForCommand
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_NoConnectionForCommand));
      }
    }

    internal static string EntityClient_NoConnectionForAdapter
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_NoConnectionForAdapter));
      }
    }

    internal static string EntityClient_ClosedConnectionForUpdate
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ClosedConnectionForUpdate));
      }
    }

    internal static string EntityClient_InvalidNamedConnection
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_InvalidNamedConnection));
      }
    }

    internal static string EntityClient_NestedNamedConnection(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_NestedNamedConnection), p0);
    }

    internal static string EntityClient_InvalidStoreProvider(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_InvalidStoreProvider), p0);
    }

    internal static string EntityClient_DataReaderIsStillOpen
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_DataReaderIsStillOpen));
      }
    }

    internal static string EntityClient_SettingsCannotBeChangedOnOpenConnection
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_SettingsCannotBeChangedOnOpenConnection));
      }
    }

    internal static string EntityClient_ExecutingOnClosedConnection(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_ExecutingOnClosedConnection), p0);
    }

    internal static string EntityClient_ConnectionStateClosed
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ConnectionStateClosed));
      }
    }

    internal static string EntityClient_ConnectionStateBroken
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ConnectionStateBroken));
      }
    }

    internal static string EntityClient_CannotCloneStoreProvider
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CannotCloneStoreProvider));
      }
    }

    internal static string EntityClient_UnsupportedCommandType
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_UnsupportedCommandType));
      }
    }

    internal static string EntityClient_ErrorInClosingConnection
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ErrorInClosingConnection));
      }
    }

    internal static string EntityClient_ErrorInBeginningTransaction
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ErrorInBeginningTransaction));
      }
    }

    internal static string EntityClient_ExtraParametersWithNamedConnection
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ExtraParametersWithNamedConnection));
      }
    }

    internal static string EntityClient_CommandDefinitionPreparationFailed
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CommandDefinitionPreparationFailed));
      }
    }

    internal static string EntityClient_CommandDefinitionExecutionFailed
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CommandDefinitionExecutionFailed));
      }
    }

    internal static string EntityClient_CommandExecutionFailed
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CommandExecutionFailed));
      }
    }

    internal static string EntityClient_StoreReaderFailed
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_StoreReaderFailed));
      }
    }

    internal static string EntityClient_FailedToGetInformation(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_FailedToGetInformation), p0);
    }

    internal static string EntityClient_TooFewColumns
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_TooFewColumns));
      }
    }

    internal static string EntityClient_InvalidParameterName(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_InvalidParameterName), p0);
    }

    internal static string EntityClient_EmptyParameterName
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_EmptyParameterName));
      }
    }

    internal static string EntityClient_ReturnedNullOnProviderMethod(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityClient_ReturnedNullOnProviderMethod), p0, p1);
    }

    internal static string EntityClient_CannotDeduceDbType
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CannotDeduceDbType));
      }
    }

    internal static string EntityClient_InvalidParameterDirection(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_InvalidParameterDirection), p0);
    }

    internal static string EntityClient_UnknownParameterType(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_UnknownParameterType), p0);
    }

    internal static string EntityClient_UnsupportedDbType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityClient_UnsupportedDbType), p0, p1);
    }

    internal static string EntityClient_IncompatibleNavigationPropertyResult(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityClient_IncompatibleNavigationPropertyResult), p0, p1);
    }

    internal static string EntityClient_TransactionAlreadyStarted
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_TransactionAlreadyStarted));
      }
    }

    internal static string EntityClient_InvalidTransactionForCommand
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_InvalidTransactionForCommand));
      }
    }

    internal static string EntityClient_NoStoreConnectionForUpdate
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_NoStoreConnectionForUpdate));
      }
    }

    internal static string EntityClient_CommandTreeMetadataIncompatible
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CommandTreeMetadataIncompatible));
      }
    }

    internal static string EntityClient_ProviderGeneralError
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_ProviderGeneralError));
      }
    }

    internal static string EntityClient_ProviderSpecificError(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_ProviderSpecificError), p0);
    }

    internal static string EntityClient_FunctionImportEmptyCommandText
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_FunctionImportEmptyCommandText));
      }
    }

    internal static string EntityClient_UnableToFindFunctionImportContainer(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_UnableToFindFunctionImportContainer), p0);
    }

    internal static string EntityClient_UnableToFindFunctionImport(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityClient_UnableToFindFunctionImport), p0, p1);
    }

    internal static string EntityClient_FunctionImportMustBeNonComposable(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_FunctionImportMustBeNonComposable), p0);
    }

    internal static string EntityClient_UnmappedFunctionImport(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_UnmappedFunctionImport), p0);
    }

    internal static string EntityClient_InvalidStoredProcedureCommandText
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_InvalidStoredProcedureCommandText));
      }
    }

    internal static string EntityClient_ItemCollectionsNotRegisteredInWorkspace(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_ItemCollectionsNotRegisteredInWorkspace), p0);
    }

    internal static string EntityClient_DbConnectionHasNoProvider(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_DbConnectionHasNoProvider), p0);
    }

    internal static string EntityClient_RequiresNonStoreCommandTree
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_RequiresNonStoreCommandTree));
      }
    }

    internal static string EntityClient_CannotReprepareCommandDefinitionBasedCommand
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CannotReprepareCommandDefinitionBasedCommand));
      }
    }

    internal static string EntityClient_EntityParameterEdmTypeNotScalar(object p0)
    {
      return EntityRes.GetString(nameof (EntityClient_EntityParameterEdmTypeNotScalar), p0);
    }

    internal static string EntityClient_EntityParameterInconsistentEdmType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityClient_EntityParameterInconsistentEdmType), p0, p1);
    }

    internal static string EntityClient_CannotGetCommandText
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CannotGetCommandText));
      }
    }

    internal static string EntityClient_CannotSetCommandText
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CannotSetCommandText));
      }
    }

    internal static string EntityClient_CannotGetCommandTree
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CannotGetCommandTree));
      }
    }

    internal static string EntityClient_CannotSetCommandTree
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CannotSetCommandTree));
      }
    }

    internal static string ELinq_ExpressionMustBeIQueryable
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_ExpressionMustBeIQueryable));
      }
    }

    internal static string ELinq_UnsupportedExpressionType(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedExpressionType), p0);
    }

    internal static string ELinq_UnsupportedUseOfContextParameter(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedUseOfContextParameter), p0);
    }

    internal static string ELinq_UnboundParameterExpression(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnboundParameterExpression), p0);
    }

    internal static string ELinq_UnsupportedConstructor
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedConstructor));
      }
    }

    internal static string ELinq_UnsupportedInitializers
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedInitializers));
      }
    }

    internal static string ELinq_UnsupportedBinding
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedBinding));
      }
    }

    internal static string ELinq_UnsupportedMethod(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedMethod), p0);
    }

    internal static string ELinq_UnsupportedMethodSuggestedAlternative(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedMethodSuggestedAlternative), p0, p1);
    }

    internal static string ELinq_ThenByDoesNotFollowOrderBy
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_ThenByDoesNotFollowOrderBy));
      }
    }

    internal static string ELinq_UnrecognizedMember(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnrecognizedMember), p0);
    }

    internal static string ELinq_UnresolvableFunctionForMethod(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnresolvableFunctionForMethod), p0, p1);
    }

    internal static string ELinq_UnresolvableFunctionForMethodAmbiguousMatch(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnresolvableFunctionForMethodAmbiguousMatch), p0, p1);
    }

    internal static string ELinq_UnresolvableFunctionForMethodNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnresolvableFunctionForMethodNotFound), p0, p1);
    }

    internal static string ELinq_UnresolvableFunctionForMember(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnresolvableFunctionForMember), p0, p1);
    }

    internal static string ELinq_UnresolvableStoreFunctionForMember(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnresolvableStoreFunctionForMember), p0, p1);
    }

    internal static string ELinq_UnresolvableFunctionForExpression(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnresolvableFunctionForExpression), p0);
    }

    internal static string ELinq_UnresolvableStoreFunctionForExpression(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnresolvableStoreFunctionForExpression), p0);
    }

    internal static string ELinq_UnsupportedType(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedType), p0);
    }

    internal static string ELinq_UnsupportedNullConstant(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedNullConstant), p0);
    }

    internal static string ELinq_UnsupportedConstant(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedConstant), p0);
    }

    internal static string ELinq_UnsupportedCast(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedCast), p0, p1);
    }

    internal static string ELinq_UnsupportedIsOrAs(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedIsOrAs), p0, p1, p2);
    }

    internal static string ELinq_UnsupportedQueryableMethod
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedQueryableMethod));
      }
    }

    internal static string ELinq_InvalidOfTypeResult(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_InvalidOfTypeResult), p0);
    }

    internal static string ELinq_UnsupportedNominalType(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedNominalType), p0);
    }

    internal static string ELinq_UnsupportedEnumerableType(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedEnumerableType), p0);
    }

    internal static string ELinq_UnsupportedHeterogeneousInitializers(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedHeterogeneousInitializers), p0);
    }

    internal static string ELinq_UnsupportedDifferentContexts
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedDifferentContexts));
      }
    }

    internal static string ELinq_UnsupportedCastToDecimal
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedCastToDecimal));
      }
    }

    internal static string ELinq_UnsupportedKeySelector(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedKeySelector), p0);
    }

    internal static string ELinq_CreateOrderedEnumerableNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_CreateOrderedEnumerableNotSupported));
      }
    }

    internal static string ELinq_UnsupportedPassthrough(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedPassthrough), p0, p1);
    }

    internal static string ELinq_UnexpectedTypeForNavigationProperty(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (ELinq_UnexpectedTypeForNavigationProperty), p0, p1, p2, p3);
    }

    internal static string ELinq_SkipWithoutOrder
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_SkipWithoutOrder));
      }
    }

    internal static string ELinq_PropertyIndexNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_PropertyIndexNotSupported));
      }
    }

    internal static string ELinq_NotPropertyOrField(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_NotPropertyOrField), p0);
    }

    internal static string ELinq_UnsupportedStringRemoveCase(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedStringRemoveCase), p0, p1);
    }

    internal static string ELinq_UnsupportedTrimStartTrimEndCase(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedTrimStartTrimEndCase), p0);
    }

    internal static string ELinq_UnsupportedVBDatePartNonConstantInterval(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedVBDatePartNonConstantInterval), p0, p1);
    }

    internal static string ELinq_UnsupportedVBDatePartInvalidInterval(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedVBDatePartInvalidInterval), p0, p1, p2);
    }

    internal static string ELinq_UnsupportedAsUnicodeAndAsNonUnicode(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedAsUnicodeAndAsNonUnicode), p0);
    }

    internal static string ELinq_UnsupportedComparison(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedComparison), p0);
    }

    internal static string ELinq_UnsupportedRefComparison(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedRefComparison), p0, p1);
    }

    internal static string ELinq_UnsupportedRowComparison(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedRowComparison), p0);
    }

    internal static string ELinq_UnsupportedRowMemberComparison(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedRowMemberComparison), p0);
    }

    internal static string ELinq_UnsupportedRowTypeComparison(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnsupportedRowTypeComparison), p0);
    }

    internal static string ELinq_AnonymousType
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_AnonymousType));
      }
    }

    internal static string ELinq_ClosureType
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_ClosureType));
      }
    }

    internal static string ELinq_UnhandledExpressionType(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnhandledExpressionType), p0);
    }

    internal static string ELinq_UnhandledBindingType(object p0)
    {
      return EntityRes.GetString(nameof (ELinq_UnhandledBindingType), p0);
    }

    internal static string ELinq_UnsupportedNestedFirst
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedNestedFirst));
      }
    }

    internal static string ELinq_UnsupportedNestedSingle
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedNestedSingle));
      }
    }

    internal static string ELinq_UnsupportedInclude
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedInclude));
      }
    }

    internal static string ELinq_UnsupportedMergeAs
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_UnsupportedMergeAs));
      }
    }

    internal static string ELinq_MethodNotDirectlyCallable
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_MethodNotDirectlyCallable));
      }
    }

    internal static string ELinq_CycleDetected
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_CycleDetected));
      }
    }

    internal static string ELinq_DbFunctionAttributedFunctionWithWrongReturnType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (ELinq_DbFunctionAttributedFunctionWithWrongReturnType), p0, p1);
    }

    internal static string ELinq_DbFunctionDirectCall
    {
      get
      {
        return EntityRes.GetString(nameof (ELinq_DbFunctionDirectCall));
      }
    }

    internal static string ELinq_HasFlagArgumentAndSourceTypeMismatch(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ELinq_HasFlagArgumentAndSourceTypeMismatch), p0, p1);
    }

    internal static string Elinq_ToStringNotSupportedForType(object p0)
    {
      return EntityRes.GetString(nameof (Elinq_ToStringNotSupportedForType), p0);
    }

    internal static string Elinq_ToStringNotSupportedForEnumsWithFlags
    {
      get
      {
        return EntityRes.GetString(nameof (Elinq_ToStringNotSupportedForEnumsWithFlags));
      }
    }

    internal static string CompiledELinq_UnsupportedParameterTypes(object p0)
    {
      return EntityRes.GetString(nameof (CompiledELinq_UnsupportedParameterTypes), p0);
    }

    internal static string CompiledELinq_UnsupportedNamedParameterType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CompiledELinq_UnsupportedNamedParameterType), p0, p1);
    }

    internal static string CompiledELinq_UnsupportedNamedParameterUseAsType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CompiledELinq_UnsupportedNamedParameterUseAsType), p0, p1);
    }

    internal static string Update_UnsupportedExpressionKind(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Update_UnsupportedExpressionKind), p0, p1);
    }

    internal static string Update_UnsupportedCastArgument(object p0)
    {
      return EntityRes.GetString(nameof (Update_UnsupportedCastArgument), p0);
    }

    internal static string Update_UnsupportedExtentType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Update_UnsupportedExtentType), p0, p1);
    }

    internal static string Update_ConstraintCycle
    {
      get
      {
        return EntityRes.GetString(nameof (Update_ConstraintCycle));
      }
    }

    internal static string Update_UnsupportedJoinType(object p0)
    {
      return EntityRes.GetString(nameof (Update_UnsupportedJoinType), p0);
    }

    internal static string Update_UnsupportedProjection(object p0)
    {
      return EntityRes.GetString(nameof (Update_UnsupportedProjection), p0);
    }

    internal static string Update_ConcurrencyError(object p0)
    {
      return EntityRes.GetString(nameof (Update_ConcurrencyError), p0);
    }

    internal static string Update_MissingEntity(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Update_MissingEntity), p0, p1);
    }

    internal static string Update_RelationshipCardinalityConstraintViolation(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (Update_RelationshipCardinalityConstraintViolation), p0, p1, p2, p3, p4, p5);
    }

    internal static string Update_GeneralExecutionException
    {
      get
      {
        return EntityRes.GetString(nameof (Update_GeneralExecutionException));
      }
    }

    internal static string Update_MissingRequiredEntity(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (Update_MissingRequiredEntity), p0, p1, p2);
    }

    internal static string Update_RelationshipCardinalityViolation(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (Update_RelationshipCardinalityViolation), p0, p1, p2, p3, p4, p5);
    }

    internal static string Update_NotSupportedComputedKeyColumn(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (Update_NotSupportedComputedKeyColumn), p0, p1, p2, p3, p4);
    }

    internal static string Update_AmbiguousServerGenIdentifier
    {
      get
      {
        return EntityRes.GetString(nameof (Update_AmbiguousServerGenIdentifier));
      }
    }

    internal static string Update_WorkspaceMismatch
    {
      get
      {
        return EntityRes.GetString(nameof (Update_WorkspaceMismatch));
      }
    }

    internal static string Update_MissingRequiredRelationshipValue(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Update_MissingRequiredRelationshipValue), p0, p1);
    }

    internal static string Update_MissingResultColumn(object p0)
    {
      return EntityRes.GetString(nameof (Update_MissingResultColumn), p0);
    }

    internal static string Update_NullReturnValueForNonNullableMember(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Update_NullReturnValueForNonNullableMember), p0, p1);
    }

    internal static string Update_ReturnValueHasUnexpectedType(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Update_ReturnValueHasUnexpectedType), p0, p1, p2, p3);
    }

    internal static string Update_UnableToConvertRowsAffectedParameter(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Update_UnableToConvertRowsAffectedParameter), p0, p1);
    }

    internal static string Update_MappingNotFound(object p0)
    {
      return EntityRes.GetString(nameof (Update_MappingNotFound), p0);
    }

    internal static string Update_ModifyingIdentityColumn(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (Update_ModifyingIdentityColumn), p0, p1, p2);
    }

    internal static string Update_GeneratedDependent(object p0)
    {
      return EntityRes.GetString(nameof (Update_GeneratedDependent), p0);
    }

    internal static string Update_ReferentialConstraintIntegrityViolation
    {
      get
      {
        return EntityRes.GetString(nameof (Update_ReferentialConstraintIntegrityViolation));
      }
    }

    internal static string Update_ErrorLoadingRecord
    {
      get
      {
        return EntityRes.GetString(nameof (Update_ErrorLoadingRecord));
      }
    }

    internal static string Update_NullValue(object p0)
    {
      return EntityRes.GetString(nameof (Update_NullValue), p0);
    }

    internal static string Update_CircularRelationships
    {
      get
      {
        return EntityRes.GetString(nameof (Update_CircularRelationships));
      }
    }

    internal static string Update_RelationshipCardinalityConstraintViolationSingleValue(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (Update_RelationshipCardinalityConstraintViolationSingleValue), p0, p1, p2, p3, p4);
    }

    internal static string Update_MissingFunctionMapping(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (Update_MissingFunctionMapping), p0, p1, p2);
    }

    internal static string Update_InvalidChanges
    {
      get
      {
        return EntityRes.GetString(nameof (Update_InvalidChanges));
      }
    }

    internal static string Update_DuplicateKeys
    {
      get
      {
        return EntityRes.GetString(nameof (Update_DuplicateKeys));
      }
    }

    internal static string Update_AmbiguousForeignKey(object p0)
    {
      return EntityRes.GetString(nameof (Update_AmbiguousForeignKey), p0);
    }

    internal static string Update_InsertingOrUpdatingReferenceToDeletedEntity(object p0)
    {
      return EntityRes.GetString(nameof (Update_InsertingOrUpdatingReferenceToDeletedEntity), p0);
    }

    internal static string ViewGen_Extent
    {
      get
      {
        return EntityRes.GetString(nameof (ViewGen_Extent));
      }
    }

    internal static string ViewGen_Null
    {
      get
      {
        return EntityRes.GetString(nameof (ViewGen_Null));
      }
    }

    internal static string ViewGen_CommaBlank
    {
      get
      {
        return EntityRes.GetString(nameof (ViewGen_CommaBlank));
      }
    }

    internal static string ViewGen_Entities
    {
      get
      {
        return EntityRes.GetString(nameof (ViewGen_Entities));
      }
    }

    internal static string ViewGen_Tuples
    {
      get
      {
        return EntityRes.GetString(nameof (ViewGen_Tuples));
      }
    }

    internal static string ViewGen_NotNull
    {
      get
      {
        return EntityRes.GetString(nameof (ViewGen_NotNull));
      }
    }

    internal static string ViewGen_NegatedCellConstant(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_NegatedCellConstant), p0);
    }

    internal static string ViewGen_Error
    {
      get
      {
        return EntityRes.GetString(nameof (ViewGen_Error));
      }
    }

    internal static string Viewgen_CannotGenerateQueryViewUnderNoValidation(object p0)
    {
      return EntityRes.GetString(nameof (Viewgen_CannotGenerateQueryViewUnderNoValidation), p0);
    }

    internal static string ViewGen_Missing_Sets_Mapping(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_Missing_Sets_Mapping), p0);
    }

    internal static string ViewGen_Missing_Type_Mapping(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_Missing_Type_Mapping), p0);
    }

    internal static string ViewGen_Missing_Set_Mapping(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_Missing_Set_Mapping), p0);
    }

    internal static string ViewGen_Concurrency_Derived_Class(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ViewGen_Concurrency_Derived_Class), p0, p1, p2);
    }

    internal static string ViewGen_Concurrency_Invalid_Condition(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_Concurrency_Invalid_Condition), p0, p1);
    }

    internal static string ViewGen_TableKey_Missing(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_TableKey_Missing), p0, p1);
    }

    internal static string ViewGen_EntitySetKey_Missing(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_EntitySetKey_Missing), p0, p1);
    }

    internal static string ViewGen_AssociationSetKey_Missing(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ViewGen_AssociationSetKey_Missing), p0, p1, p2);
    }

    internal static string ViewGen_Cannot_Recover_Attributes(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ViewGen_Cannot_Recover_Attributes), p0, p1, p2);
    }

    internal static string ViewGen_Cannot_Recover_Types(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_Cannot_Recover_Types), p0, p1);
    }

    internal static string ViewGen_Cannot_Disambiguate_MultiConstant(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_Cannot_Disambiguate_MultiConstant), p0, p1);
    }

    internal static string ViewGen_No_Default_Value(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_No_Default_Value), p0, p1);
    }

    internal static string ViewGen_No_Default_Value_For_Configuration(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_No_Default_Value_For_Configuration), p0);
    }

    internal static string ViewGen_KeyConstraint_Violation(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (ViewGen_KeyConstraint_Violation), p0, p1, p2, p3, p4, p5);
    }

    internal static string ViewGen_KeyConstraint_Update_Violation_EntitySet(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (ViewGen_KeyConstraint_Update_Violation_EntitySet), p0, p1, p2, p3);
    }

    internal static string ViewGen_KeyConstraint_Update_Violation_AssociationSet(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ViewGen_KeyConstraint_Update_Violation_AssociationSet), p0, p1, p2);
    }

    internal static string ViewGen_AssociationEndShouldBeMappedToKey(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_AssociationEndShouldBeMappedToKey), p0, p1);
    }

    internal static string ViewGen_Duplicate_CProperties(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_Duplicate_CProperties), p0);
    }

    internal static string ViewGen_Duplicate_CProperties_IsMapped(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_Duplicate_CProperties_IsMapped), p0, p1);
    }

    internal static string ViewGen_NotNull_No_Projected_Slot(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_NotNull_No_Projected_Slot), p0);
    }

    internal static string ViewGen_InvalidCondition(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_InvalidCondition), p0);
    }

    internal static string ViewGen_NonKeyProjectedWithOverlappingPartitions(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_NonKeyProjectedWithOverlappingPartitions), p0);
    }

    internal static string ViewGen_CQ_PartitionConstraint(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_CQ_PartitionConstraint), p0);
    }

    internal static string ViewGen_CQ_DomainConstraint(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_CQ_DomainConstraint), p0);
    }

    internal static string ViewGen_ErrorLog(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_ErrorLog), p0);
    }

    internal static string ViewGen_ErrorLog2(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_ErrorLog2), p0);
    }

    internal static string ViewGen_Foreign_Key_Missing_Table_Mapping(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_Foreign_Key_Missing_Table_Mapping), p0, p1);
    }

    internal static string ViewGen_Foreign_Key_ParentTable_NotMappedToEnd(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (ViewGen_Foreign_Key_ParentTable_NotMappedToEnd), p0, p1, p2, p3, p4, p5);
    }

    internal static string ViewGen_Foreign_Key(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (ViewGen_Foreign_Key), p0, p1, p2, p3, p4);
    }

    internal static string ViewGen_Foreign_Key_UpperBound_MustBeOne(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ViewGen_Foreign_Key_UpperBound_MustBeOne), p0, p1, p2);
    }

    internal static string ViewGen_Foreign_Key_LowerBound_MustBeOne(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ViewGen_Foreign_Key_LowerBound_MustBeOne), p0, p1, p2);
    }

    internal static string ViewGen_Foreign_Key_Missing_Relationship_Mapping(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_Foreign_Key_Missing_Relationship_Mapping), p0);
    }

    internal static string ViewGen_Foreign_Key_Not_Guaranteed_InCSpace(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_Foreign_Key_Not_Guaranteed_InCSpace), p0);
    }

    internal static string ViewGen_Foreign_Key_ColumnOrder_Incorrect(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5,
      object p6,
      object p7,
      object p8)
    {
      return EntityRes.GetString(nameof (ViewGen_Foreign_Key_ColumnOrder_Incorrect), p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    internal static string ViewGen_AssociationSet_AsUserString(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ViewGen_AssociationSet_AsUserString), p0, p1, p2);
    }

    internal static string ViewGen_AssociationSet_AsUserString_Negated(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ViewGen_AssociationSet_AsUserString_Negated), p0, p1, p2);
    }

    internal static string ViewGen_EntitySet_AsUserString(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_EntitySet_AsUserString), p0, p1);
    }

    internal static string ViewGen_EntitySet_AsUserString_Negated(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGen_EntitySet_AsUserString_Negated), p0, p1);
    }

    internal static string ViewGen_EntityInstanceToken
    {
      get
      {
        return EntityRes.GetString(nameof (ViewGen_EntityInstanceToken));
      }
    }

    internal static string Viewgen_ConfigurationErrorMsg(object p0)
    {
      return EntityRes.GetString(nameof (Viewgen_ConfigurationErrorMsg), p0);
    }

    internal static string ViewGen_HashOnMappingClosure_Not_Matching(object p0)
    {
      return EntityRes.GetString(nameof (ViewGen_HashOnMappingClosure_Not_Matching), p0);
    }

    internal static string Viewgen_RightSideNotDisjoint(object p0)
    {
      return EntityRes.GetString(nameof (Viewgen_RightSideNotDisjoint), p0);
    }

    internal static string Viewgen_QV_RewritingNotFound(object p0)
    {
      return EntityRes.GetString(nameof (Viewgen_QV_RewritingNotFound), p0);
    }

    internal static string Viewgen_NullableMappingForNonNullableColumn(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Viewgen_NullableMappingForNonNullableColumn), p0, p1);
    }

    internal static string Viewgen_ErrorPattern_ConditionMemberIsMapped(object p0)
    {
      return EntityRes.GetString(nameof (Viewgen_ErrorPattern_ConditionMemberIsMapped), p0);
    }

    internal static string Viewgen_ErrorPattern_DuplicateConditionValue(object p0)
    {
      return EntityRes.GetString(nameof (Viewgen_ErrorPattern_DuplicateConditionValue), p0);
    }

    internal static string Viewgen_ErrorPattern_TableMappedToMultipleES(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Viewgen_ErrorPattern_TableMappedToMultipleES), p0, p1, p2);
    }

    internal static string Viewgen_ErrorPattern_Partition_Disj_Eq
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Disj_Eq));
      }
    }

    internal static string Viewgen_ErrorPattern_NotNullConditionMappedToNullableMember(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Viewgen_ErrorPattern_NotNullConditionMappedToNullableMember), p0, p1);
    }

    internal static string Viewgen_ErrorPattern_Partition_MultipleTypesMappedToSameTable_WithoutCondition(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_MultipleTypesMappedToSameTable_WithoutCondition), p0, p1);
    }

    internal static string Viewgen_ErrorPattern_Partition_Disj_Subs_Ref
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Disj_Subs_Ref));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Disj_Subs
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Disj_Subs));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Disj_Unk
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Disj_Unk));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Eq_Disj
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Eq_Disj));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Eq_Subs_Ref
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Eq_Subs_Ref));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Eq_Subs
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Eq_Subs));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Eq_Unk
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Eq_Unk));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Eq_Unk_Association
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Eq_Unk_Association));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Sub_Disj
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Sub_Disj));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Sub_Eq
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Sub_Eq));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Sub_Eq_Ref
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Sub_Eq_Ref));
      }
    }

    internal static string Viewgen_ErrorPattern_Partition_Sub_Unk
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_ErrorPattern_Partition_Sub_Unk));
      }
    }

    internal static string Viewgen_NoJoinKeyOrFK
    {
      get
      {
        return EntityRes.GetString(nameof (Viewgen_NoJoinKeyOrFK));
      }
    }

    internal static string Viewgen_MultipleFragmentsBetweenCandSExtentWithDistinct(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Viewgen_MultipleFragmentsBetweenCandSExtentWithDistinct), p0, p1);
    }

    internal static string Validator_EmptyIdentity
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_EmptyIdentity));
      }
    }

    internal static string Validator_CollectionHasNoTypeUsage
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_CollectionHasNoTypeUsage));
      }
    }

    internal static string Validator_NoKeyMembers(object p0)
    {
      return EntityRes.GetString(nameof (Validator_NoKeyMembers), p0);
    }

    internal static string Validator_FacetTypeIsNull
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_FacetTypeIsNull));
      }
    }

    internal static string Validator_MemberHasNullDeclaringType
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_MemberHasNullDeclaringType));
      }
    }

    internal static string Validator_MemberHasNullTypeUsage
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_MemberHasNullTypeUsage));
      }
    }

    internal static string Validator_ItemAttributeHasNullTypeUsage
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_ItemAttributeHasNullTypeUsage));
      }
    }

    internal static string Validator_RefTypeHasNullEntityType
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_RefTypeHasNullEntityType));
      }
    }

    internal static string Validator_TypeUsageHasNullEdmType
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_TypeUsageHasNullEdmType));
      }
    }

    internal static string Validator_BaseTypeHasMemberOfSameName
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_BaseTypeHasMemberOfSameName));
      }
    }

    internal static string Validator_CollectionTypesCannotHaveBaseType
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_CollectionTypesCannotHaveBaseType));
      }
    }

    internal static string Validator_RefTypesCannotHaveBaseType
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_RefTypesCannotHaveBaseType));
      }
    }

    internal static string Validator_TypeHasNoName
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_TypeHasNoName));
      }
    }

    internal static string Validator_TypeHasNoNamespace
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_TypeHasNoNamespace));
      }
    }

    internal static string Validator_FacetHasNoName
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_FacetHasNoName));
      }
    }

    internal static string Validator_MemberHasNoName
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_MemberHasNoName));
      }
    }

    internal static string Validator_MetadataPropertyHasNoName
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_MetadataPropertyHasNoName));
      }
    }

    internal static string Validator_NullableEntityKeyProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Validator_NullableEntityKeyProperty), p0, p1);
    }

    internal static string Validator_OSpace_InvalidNavPropReturnType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_InvalidNavPropReturnType), p0, p1, p2);
    }

    internal static string Validator_OSpace_ScalarPropertyNotPrimitive(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_ScalarPropertyNotPrimitive), p0, p1, p2);
    }

    internal static string Validator_OSpace_ComplexPropertyNotComplex(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_ComplexPropertyNotComplex), p0, p1, p2);
    }

    internal static string Validator_OSpace_Convention_MultipleTypesWithSameName(object p0)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_MultipleTypesWithSameName), p0);
    }

    internal static string Validator_OSpace_Convention_NonPrimitiveTypeProperty(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_NonPrimitiveTypeProperty), p0, p1, p2);
    }

    internal static string Validator_OSpace_Convention_MissingRequiredProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_MissingRequiredProperty), p0, p1);
    }

    internal static string Validator_OSpace_Convention_BaseTypeIncompatible(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_BaseTypeIncompatible), p0, p1, p2);
    }

    internal static string Validator_OSpace_Convention_MissingOSpaceType(object p0)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_MissingOSpaceType), p0);
    }

    internal static string Validator_OSpace_Convention_RelationshipNotLoaded(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_RelationshipNotLoaded), p0, p1);
    }

    internal static string Validator_OSpace_Convention_AttributeAssemblyReferenced(object p0)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_AttributeAssemblyReferenced), p0);
    }

    internal static string Validator_OSpace_Convention_ScalarPropertyMissginGetterOrSetter(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_ScalarPropertyMissginGetterOrSetter), p0, p1, p2);
    }

    internal static string Validator_OSpace_Convention_AmbiguousClrType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_AmbiguousClrType), p0, p1, p2);
    }

    internal static string Validator_OSpace_Convention_Struct(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_Struct), p0, p1);
    }

    internal static string Validator_OSpace_Convention_BaseTypeNotLoaded(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_BaseTypeNotLoaded), p0, p1);
    }

    internal static string Validator_OSpace_Convention_SSpaceOSpaceTypeMismatch(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Validator_OSpace_Convention_SSpaceOSpaceTypeMismatch), p0, p1);
    }

    internal static string Validator_OSpace_Convention_NonMatchingUnderlyingTypes
    {
      get
      {
        return EntityRes.GetString(nameof (Validator_OSpace_Convention_NonMatchingUnderlyingTypes));
      }
    }

    internal static string Validator_UnsupportedEnumUnderlyingType(object p0)
    {
      return EntityRes.GetString(nameof (Validator_UnsupportedEnumUnderlyingType), p0);
    }

    internal static string ExtraInfo
    {
      get
      {
        return EntityRes.GetString(nameof (ExtraInfo));
      }
    }

    internal static string Metadata_General_Error
    {
      get
      {
        return EntityRes.GetString(nameof (Metadata_General_Error));
      }
    }

    internal static string InvalidNumberOfParametersForAggregateFunction(object p0)
    {
      return EntityRes.GetString(nameof (InvalidNumberOfParametersForAggregateFunction), p0);
    }

    internal static string InvalidParameterTypeForAggregateFunction(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidParameterTypeForAggregateFunction), p0, p1);
    }

    internal static string InvalidSchemaEncountered(object p0)
    {
      return EntityRes.GetString(nameof (InvalidSchemaEncountered), p0);
    }

    internal static string SystemNamespaceEncountered(object p0)
    {
      return EntityRes.GetString(nameof (SystemNamespaceEncountered), p0);
    }

    internal static string NoCollectionForSpace(object p0)
    {
      return EntityRes.GetString(nameof (NoCollectionForSpace), p0);
    }

    internal static string OperationOnReadOnlyCollection
    {
      get
      {
        return EntityRes.GetString(nameof (OperationOnReadOnlyCollection));
      }
    }

    internal static string OperationOnReadOnlyItem
    {
      get
      {
        return EntityRes.GetString(nameof (OperationOnReadOnlyItem));
      }
    }

    internal static string EntitySetInAnotherContainer
    {
      get
      {
        return EntityRes.GetString(nameof (EntitySetInAnotherContainer));
      }
    }

    internal static string InvalidKeyMember(object p0)
    {
      return EntityRes.GetString(nameof (InvalidKeyMember), p0);
    }

    internal static string InvalidFileExtension(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (InvalidFileExtension), p0, p1, p2);
    }

    internal static string NewTypeConflictsWithExistingType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NewTypeConflictsWithExistingType), p0, p1);
    }

    internal static string NotValidInputPath
    {
      get
      {
        return EntityRes.GetString(nameof (NotValidInputPath));
      }
    }

    internal static string UnableToDetermineApplicationContext
    {
      get
      {
        return EntityRes.GetString(nameof (UnableToDetermineApplicationContext));
      }
    }

    internal static string WildcardEnumeratorReturnedNull
    {
      get
      {
        return EntityRes.GetString(nameof (WildcardEnumeratorReturnedNull));
      }
    }

    internal static string InvalidUseOfWebPath(object p0)
    {
      return EntityRes.GetString(nameof (InvalidUseOfWebPath), p0);
    }

    internal static string UnableToFindReflectedType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (UnableToFindReflectedType), p0, p1);
    }

    internal static string AssemblyMissingFromAssembliesToConsider(object p0)
    {
      return EntityRes.GetString(nameof (AssemblyMissingFromAssembliesToConsider), p0);
    }

    internal static string UnableToLoadResource
    {
      get
      {
        return EntityRes.GetString(nameof (UnableToLoadResource));
      }
    }

    internal static string EdmVersionNotSupportedByRuntime(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EdmVersionNotSupportedByRuntime), p0, p1);
    }

    internal static string AtleastOneSSDLNeeded
    {
      get
      {
        return EntityRes.GetString(nameof (AtleastOneSSDLNeeded));
      }
    }

    internal static string InvalidMetadataPath
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidMetadataPath));
      }
    }

    internal static string UnableToResolveAssembly(object p0)
    {
      return EntityRes.GetString(nameof (UnableToResolveAssembly), p0);
    }

    internal static string DuplicatedFunctionoverloads(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DuplicatedFunctionoverloads), p0, p1);
    }

    internal static string EntitySetNotInCSPace(object p0)
    {
      return EntityRes.GetString(nameof (EntitySetNotInCSPace), p0);
    }

    internal static string TypeNotInEntitySet(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (TypeNotInEntitySet), p0, p1, p2);
    }

    internal static string TypeNotInAssociationSet(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (TypeNotInAssociationSet), p0, p1, p2);
    }

    internal static string DifferentSchemaVersionInCollection(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (DifferentSchemaVersionInCollection), p0, p1, p2);
    }

    internal static string InvalidCollectionForMapping(object p0)
    {
      return EntityRes.GetString(nameof (InvalidCollectionForMapping), p0);
    }

    internal static string OnlyStoreConnectionsSupported
    {
      get
      {
        return EntityRes.GetString(nameof (OnlyStoreConnectionsSupported));
      }
    }

    internal static string StoreItemCollectionMustHaveOneArtifact(object p0)
    {
      return EntityRes.GetString(nameof (StoreItemCollectionMustHaveOneArtifact), p0);
    }

    internal static string CheckArgumentContainsNullFailed(object p0)
    {
      return EntityRes.GetString(nameof (CheckArgumentContainsNullFailed), p0);
    }

    internal static string InvalidRelationshipSetName(object p0)
    {
      return EntityRes.GetString(nameof (InvalidRelationshipSetName), p0);
    }

    internal static string InvalidEntitySetName(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEntitySetName), p0);
    }

    internal static string OnlyFunctionImportsCanBeAddedToEntityContainer(object p0)
    {
      return EntityRes.GetString(nameof (OnlyFunctionImportsCanBeAddedToEntityContainer), p0);
    }

    internal static string ItemInvalidIdentity(object p0)
    {
      return EntityRes.GetString(nameof (ItemInvalidIdentity), p0);
    }

    internal static string ItemDuplicateIdentity(object p0)
    {
      return EntityRes.GetString(nameof (ItemDuplicateIdentity), p0);
    }

    internal static string NotStringTypeForTypeUsage
    {
      get
      {
        return EntityRes.GetString(nameof (NotStringTypeForTypeUsage));
      }
    }

    internal static string NotBinaryTypeForTypeUsage
    {
      get
      {
        return EntityRes.GetString(nameof (NotBinaryTypeForTypeUsage));
      }
    }

    internal static string NotDateTimeTypeForTypeUsage
    {
      get
      {
        return EntityRes.GetString(nameof (NotDateTimeTypeForTypeUsage));
      }
    }

    internal static string NotDateTimeOffsetTypeForTypeUsage
    {
      get
      {
        return EntityRes.GetString(nameof (NotDateTimeOffsetTypeForTypeUsage));
      }
    }

    internal static string NotTimeTypeForTypeUsage
    {
      get
      {
        return EntityRes.GetString(nameof (NotTimeTypeForTypeUsage));
      }
    }

    internal static string NotDecimalTypeForTypeUsage
    {
      get
      {
        return EntityRes.GetString(nameof (NotDecimalTypeForTypeUsage));
      }
    }

    internal static string ArrayTooSmall
    {
      get
      {
        return EntityRes.GetString(nameof (ArrayTooSmall));
      }
    }

    internal static string MoreThanOneItemMatchesIdentity(object p0)
    {
      return EntityRes.GetString(nameof (MoreThanOneItemMatchesIdentity), p0);
    }

    internal static string MissingDefaultValueForConstantFacet(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MissingDefaultValueForConstantFacet), p0, p1);
    }

    internal static string MinAndMaxValueMustBeSameForConstantFacet(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MinAndMaxValueMustBeSameForConstantFacet), p0, p1);
    }

    internal static string BothMinAndMaxValueMustBeSpecifiedForNonConstantFacet(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (BothMinAndMaxValueMustBeSpecifiedForNonConstantFacet), p0, p1);
    }

    internal static string MinAndMaxValueMustBeDifferentForNonConstantFacet(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MinAndMaxValueMustBeDifferentForNonConstantFacet), p0, p1);
    }

    internal static string MinAndMaxMustBePositive(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MinAndMaxMustBePositive), p0, p1);
    }

    internal static string MinMustBeLessThanMax(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (MinMustBeLessThanMax), p0, p1, p2);
    }

    internal static string SameRoleNameOnRelationshipAttribute(object p0, object p1)
    {
      return EntityRes.GetString(nameof (SameRoleNameOnRelationshipAttribute), p0, p1);
    }

    internal static string RoleTypeInEdmRelationshipAttributeIsInvalidType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (RoleTypeInEdmRelationshipAttributeIsInvalidType), p0, p1, p2);
    }

    internal static string TargetRoleNameInNavigationPropertyNotValid(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (TargetRoleNameInNavigationPropertyNotValid), p0, p1, p2, p3);
    }

    internal static string RelationshipNameInNavigationPropertyNotValid(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (RelationshipNameInNavigationPropertyNotValid), p0, p1, p2);
    }

    internal static string NestedClassNotSupported(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NestedClassNotSupported), p0, p1);
    }

    internal static string NullParameterForEdmRelationshipAttribute(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NullParameterForEdmRelationshipAttribute), p0, p1);
    }

    internal static string NullRelationshipNameforEdmRelationshipAttribute(object p0)
    {
      return EntityRes.GetString(nameof (NullRelationshipNameforEdmRelationshipAttribute), p0);
    }

    internal static string NavigationPropertyRelationshipEndTypeMismatch(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (NavigationPropertyRelationshipEndTypeMismatch), p0, p1, p2, p3, p4);
    }

    internal static string AllArtifactsMustTargetSameProvider_InvariantName(object p0, object p1)
    {
      return EntityRes.GetString(nameof (AllArtifactsMustTargetSameProvider_InvariantName), p0, p1);
    }

    internal static string AllArtifactsMustTargetSameProvider_ManifestToken(object p0, object p1)
    {
      return EntityRes.GetString(nameof (AllArtifactsMustTargetSameProvider_ManifestToken), p0, p1);
    }

    internal static string ProviderManifestTokenNotFound
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderManifestTokenNotFound));
      }
    }

    internal static string FailedToRetrieveProviderManifest
    {
      get
      {
        return EntityRes.GetString(nameof (FailedToRetrieveProviderManifest));
      }
    }

    internal static string InvalidMaxLengthSize
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidMaxLengthSize));
      }
    }

    internal static string ArgumentMustBeCSpaceType
    {
      get
      {
        return EntityRes.GetString(nameof (ArgumentMustBeCSpaceType));
      }
    }

    internal static string ArgumentMustBeOSpaceType
    {
      get
      {
        return EntityRes.GetString(nameof (ArgumentMustBeOSpaceType));
      }
    }

    internal static string FailedToFindOSpaceTypeMapping(object p0)
    {
      return EntityRes.GetString(nameof (FailedToFindOSpaceTypeMapping), p0);
    }

    internal static string FailedToFindCSpaceTypeMapping(object p0)
    {
      return EntityRes.GetString(nameof (FailedToFindCSpaceTypeMapping), p0);
    }

    internal static string FailedToFindClrTypeMapping(object p0)
    {
      return EntityRes.GetString(nameof (FailedToFindClrTypeMapping), p0);
    }

    internal static string GenericTypeNotSupported(object p0)
    {
      return EntityRes.GetString(nameof (GenericTypeNotSupported), p0);
    }

    internal static string InvalidEDMVersion(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEDMVersion), p0);
    }

    internal static string Mapping_General_Error
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_General_Error));
      }
    }

    internal static string Mapping_InvalidContent_General
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_General));
      }
    }

    internal static string Mapping_InvalidContent_EntityContainer(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_EntityContainer), p0);
    }

    internal static string Mapping_InvalidContent_StorageEntityContainer(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_StorageEntityContainer), p0);
    }

    internal static string Mapping_AlreadyMapped_StorageEntityContainer(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_AlreadyMapped_StorageEntityContainer), p0);
    }

    internal static string Mapping_InvalidContent_Entity_Set(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Entity_Set), p0);
    }

    internal static string Mapping_InvalidContent_Entity_Type(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Entity_Type), p0);
    }

    internal static string Mapping_InvalidContent_AbstractEntity_FunctionMapping(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_AbstractEntity_FunctionMapping), p0);
    }

    internal static string Mapping_InvalidContent_AbstractEntity_Type(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_AbstractEntity_Type), p0);
    }

    internal static string Mapping_InvalidContent_AbstractEntity_IsOfType(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_AbstractEntity_IsOfType), p0);
    }

    internal static string Mapping_InvalidContent_Entity_Type_For_Entity_Set(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Entity_Type_For_Entity_Set), p0, p1, p2);
    }

    internal static string Mapping_Invalid_Association_Type_For_Association_Set(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_Invalid_Association_Type_For_Association_Set), p0, p1, p2);
    }

    internal static string Mapping_InvalidContent_Table(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Table), p0);
    }

    internal static string Mapping_InvalidContent_Complex_Type(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Complex_Type), p0);
    }

    internal static string Mapping_InvalidContent_Association_Set(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Association_Set), p0);
    }

    internal static string Mapping_InvalidContent_AssociationSet_Condition(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_AssociationSet_Condition), p0);
    }

    internal static string Mapping_InvalidContent_ForeignKey_Association_Set(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_ForeignKey_Association_Set), p0);
    }

    internal static string Mapping_InvalidContent_ForeignKey_Association_Set_PKtoPK(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_ForeignKey_Association_Set_PKtoPK), p0);
    }

    internal static string Mapping_InvalidContent_Association_Type(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Association_Type), p0);
    }

    internal static string Mapping_InvalidContent_EndProperty(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_EndProperty), p0);
    }

    internal static string Mapping_InvalidContent_Association_Type_Empty
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_Association_Type_Empty));
      }
    }

    internal static string Mapping_InvalidContent_Table_Expected
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_Table_Expected));
      }
    }

    internal static string Mapping_InvalidContent_Cdm_Member(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Cdm_Member), p0);
    }

    internal static string Mapping_InvalidContent_Column(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Column), p0);
    }

    internal static string Mapping_InvalidContent_End(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_End), p0);
    }

    internal static string Mapping_InvalidContent_Container_SubElement
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_Container_SubElement));
      }
    }

    internal static string Mapping_InvalidContent_Duplicate_Cdm_Member(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Duplicate_Cdm_Member), p0);
    }

    internal static string Mapping_InvalidContent_Duplicate_Condition_Member(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Duplicate_Condition_Member), p0);
    }

    internal static string Mapping_InvalidContent_ConditionMapping_Both_Members
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_ConditionMapping_Both_Members));
      }
    }

    internal static string Mapping_InvalidContent_ConditionMapping_Either_Members
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_ConditionMapping_Either_Members));
      }
    }

    internal static string Mapping_InvalidContent_ConditionMapping_Both_Values
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_ConditionMapping_Both_Values));
      }
    }

    internal static string Mapping_InvalidContent_ConditionMapping_Either_Values
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_ConditionMapping_Either_Values));
      }
    }

    internal static string Mapping_InvalidContent_ConditionMapping_NonScalar
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_ConditionMapping_NonScalar));
      }
    }

    internal static string Mapping_InvalidContent_ConditionMapping_InvalidPrimitiveTypeKind(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_ConditionMapping_InvalidPrimitiveTypeKind), p0, p1);
    }

    internal static string Mapping_InvalidContent_ConditionMapping_InvalidMember(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_ConditionMapping_InvalidMember), p0);
    }

    internal static string Mapping_InvalidContent_ConditionMapping_Computed(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_ConditionMapping_Computed), p0);
    }

    internal static string Mapping_InvalidContent_Emtpty_SetMap(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidContent_Emtpty_SetMap), p0);
    }

    internal static string Mapping_InvalidContent_TypeMapping_QueryView
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_TypeMapping_QueryView));
      }
    }

    internal static string Mapping_Default_OCMapping_Clr_Member(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (Mapping_Default_OCMapping_Clr_Member), p0, p1, p2);
    }

    internal static string Mapping_Default_OCMapping_Clr_Member2(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (Mapping_Default_OCMapping_Clr_Member2), p0, p1, p2);
    }

    internal static string Mapping_Default_OCMapping_Invalid_MemberType(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (Mapping_Default_OCMapping_Invalid_MemberType), p0, p1, p2, p3, p4, p5);
    }

    internal static string Mapping_Default_OCMapping_MemberKind_Mismatch(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (Mapping_Default_OCMapping_MemberKind_Mismatch), p0, p1, p2, p3, p4, p5);
    }

    internal static string Mapping_Default_OCMapping_MultiplicityMismatch(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (Mapping_Default_OCMapping_MultiplicityMismatch), p0, p1, p2, p3, p4, p5);
    }

    internal static string Mapping_Default_OCMapping_Member_Count_Mismatch(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_Default_OCMapping_Member_Count_Mismatch), p0, p1);
    }

    internal static string Mapping_Default_OCMapping_Member_Type_Mismatch(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5,
      object p6,
      object p7)
    {
      return EntityRes.GetString(nameof (Mapping_Default_OCMapping_Member_Type_Mismatch), p0, p1, p2, p3, p4, p5, p6, p7);
    }

    internal static string Mapping_Enum_OCMapping_UnderlyingTypesMismatch(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Mapping_Enum_OCMapping_UnderlyingTypesMismatch), p0, p1, p2, p3);
    }

    internal static string Mapping_Enum_OCMapping_MemberMismatch(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Mapping_Enum_OCMapping_MemberMismatch), p0, p1, p2, p3);
    }

    internal static string Mapping_NotFound_EntityContainer(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_NotFound_EntityContainer), p0);
    }

    internal static string Mapping_Duplicate_CdmAssociationSet_StorageMap(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Duplicate_CdmAssociationSet_StorageMap), p0);
    }

    internal static string Mapping_Invalid_CSRootElementMissing(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (Mapping_Invalid_CSRootElementMissing), p0, p1, p2);
    }

    internal static string Mapping_ConditionValueTypeMismatch
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_ConditionValueTypeMismatch));
      }
    }

    internal static string Mapping_Storage_InvalidSpace(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Storage_InvalidSpace), p0);
    }

    internal static string Mapping_Invalid_Member_Mapping(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (Mapping_Invalid_Member_Mapping), p0, p1, p2, p3, p4, p5);
    }

    internal static string Mapping_Invalid_CSide_ScalarProperty(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Invalid_CSide_ScalarProperty), p0);
    }

    internal static string Mapping_Duplicate_Type(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Duplicate_Type), p0);
    }

    internal static string Mapping_Duplicate_PropertyMap_CaseInsensitive(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Duplicate_PropertyMap_CaseInsensitive), p0);
    }

    internal static string Mapping_Enum_EmptyValue(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Enum_EmptyValue), p0);
    }

    internal static string Mapping_Enum_InvalidValue(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Enum_InvalidValue), p0);
    }

    internal static string Mapping_InvalidMappingSchema_Parsing(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidMappingSchema_Parsing), p0);
    }

    internal static string Mapping_InvalidMappingSchema_validation(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_InvalidMappingSchema_validation), p0);
    }

    internal static string Mapping_Object_InvalidType(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Object_InvalidType), p0);
    }

    internal static string Mapping_Provider_WrongConnectionType(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Provider_WrongConnectionType), p0);
    }

    internal static string Mapping_Views_For_Extent_Not_Generated(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_Views_For_Extent_Not_Generated), p0, p1);
    }

    internal static string Mapping_TableName_QueryView(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_TableName_QueryView), p0);
    }

    internal static string Mapping_Empty_QueryView(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Empty_QueryView), p0);
    }

    internal static string Mapping_Empty_QueryView_OfType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_Empty_QueryView_OfType), p0, p1);
    }

    internal static string Mapping_Empty_QueryView_OfTypeOnly(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_Empty_QueryView_OfTypeOnly), p0, p1);
    }

    internal static string Mapping_QueryView_PropertyMaps(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_QueryView_PropertyMaps), p0);
    }

    internal static string Mapping_Invalid_QueryView(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_Invalid_QueryView), p0, p1);
    }

    internal static string Mapping_Invalid_QueryView2(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_Invalid_QueryView2), p0, p1);
    }

    internal static string Mapping_Invalid_QueryView_Type(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Invalid_QueryView_Type), p0);
    }

    internal static string Mapping_TypeName_For_First_QueryView
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_TypeName_For_First_QueryView));
      }
    }

    internal static string Mapping_AllQueryViewAtCompileTime(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_AllQueryViewAtCompileTime), p0);
    }

    internal static string Mapping_QueryViewMultipleTypeInTypeName(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_QueryViewMultipleTypeInTypeName), p0);
    }

    internal static string Mapping_QueryView_Duplicate_OfType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_QueryView_Duplicate_OfType), p0, p1);
    }

    internal static string Mapping_QueryView_Duplicate_OfTypeOnly(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_QueryView_Duplicate_OfTypeOnly), p0, p1);
    }

    internal static string Mapping_QueryView_TypeName_Not_Defined(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_QueryView_TypeName_Not_Defined), p0);
    }

    internal static string Mapping_QueryView_For_Base_Type(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_QueryView_For_Base_Type), p0, p1);
    }

    internal static string Mapping_UnsupportedExpressionKind_QueryView(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_UnsupportedExpressionKind_QueryView), p0, p1, p2);
    }

    internal static string Mapping_UnsupportedFunctionCall_QueryView(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_UnsupportedFunctionCall_QueryView), p0, p1);
    }

    internal static string Mapping_UnsupportedScanTarget_QueryView(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_UnsupportedScanTarget_QueryView), p0, p1);
    }

    internal static string Mapping_UnsupportedPropertyKind_QueryView(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_UnsupportedPropertyKind_QueryView), p0, p1, p2);
    }

    internal static string Mapping_UnsupportedInitialization_QueryView(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_UnsupportedInitialization_QueryView), p0, p1);
    }

    internal static string Mapping_EntitySetMismatchOnAssociationSetEnd_QueryView(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Mapping_EntitySetMismatchOnAssociationSetEnd_QueryView), p0, p1, p2, p3);
    }

    internal static string Mapping_Invalid_Query_Views_MissingSetClosure(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_Invalid_Query_Views_MissingSetClosure), p0);
    }

    internal static string DbMappingViewCacheTypeAttribute_InvalidContextType(object p0)
    {
      return EntityRes.GetString(nameof (DbMappingViewCacheTypeAttribute_InvalidContextType), p0);
    }

    internal static string DbMappingViewCacheTypeAttribute_CacheTypeNotFound(object p0)
    {
      return EntityRes.GetString(nameof (DbMappingViewCacheTypeAttribute_CacheTypeNotFound), p0);
    }

    internal static string DbMappingViewCacheTypeAttribute_MultipleInstancesWithSameContextType(
      object p0)
    {
      return EntityRes.GetString(nameof (DbMappingViewCacheTypeAttribute_MultipleInstancesWithSameContextType), p0);
    }

    internal static string DbMappingViewCacheFactory_CreateFailure
    {
      get
      {
        return EntityRes.GetString(nameof (DbMappingViewCacheFactory_CreateFailure));
      }
    }

    internal static string Generated_View_Type_Super_Class(object p0)
    {
      return EntityRes.GetString(nameof (Generated_View_Type_Super_Class), p0);
    }

    internal static string Generated_Views_Invalid_Extent(object p0)
    {
      return EntityRes.GetString(nameof (Generated_Views_Invalid_Extent), p0);
    }

    internal static string MappingViewCacheFactory_MustNotChange
    {
      get
      {
        return EntityRes.GetString(nameof (MappingViewCacheFactory_MustNotChange));
      }
    }

    internal static string Mapping_ItemWithSameNameExistsBothInCSpaceAndSSpace(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ItemWithSameNameExistsBothInCSpaceAndSSpace), p0);
    }

    internal static string Mapping_AbstractTypeMappingToNonAbstractType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_AbstractTypeMappingToNonAbstractType), p0, p1);
    }

    internal static string Mapping_EnumTypeMappingToNonEnumType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_EnumTypeMappingToNonEnumType), p0, p1);
    }

    internal static string StorageEntityContainerNameMismatchWhileSpecifyingPartialMapping(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (StorageEntityContainerNameMismatchWhileSpecifyingPartialMapping), p0, p1, p2);
    }

    internal static string Mapping_InvalidContent_IsTypeOfNotTerminated
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_InvalidContent_IsTypeOfNotTerminated));
      }
    }

    internal static string Mapping_CannotMapCLRTypeMultipleTimes(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_CannotMapCLRTypeMultipleTimes), p0);
    }

    internal static string Mapping_ModificationFunction_In_Table_Context
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_ModificationFunction_In_Table_Context));
      }
    }

    internal static string Mapping_ModificationFunction_Multiple_Types
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_ModificationFunction_Multiple_Types));
      }
    }

    internal static string Mapping_ModificationFunction_UnknownFunction(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_UnknownFunction), p0);
    }

    internal static string Mapping_ModificationFunction_AmbiguousFunction(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AmbiguousFunction), p0);
    }

    internal static string Mapping_ModificationFunction_NotValidFunction(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_NotValidFunction), p0);
    }

    internal static string Mapping_ModificationFunction_NotValidFunctionParameter(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_NotValidFunctionParameter), p0, p1, p2);
    }

    internal static string Mapping_ModificationFunction_MissingParameter(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_MissingParameter), p0, p1);
    }

    internal static string Mapping_ModificationFunction_AssociationSetDoesNotExist(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AssociationSetDoesNotExist), p0);
    }

    internal static string Mapping_ModificationFunction_AssociationSetRoleDoesNotExist(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AssociationSetRoleDoesNotExist), p0);
    }

    internal static string Mapping_ModificationFunction_AssociationSetFromRoleIsNotEntitySet(
      object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AssociationSetFromRoleIsNotEntitySet), p0);
    }

    internal static string Mapping_ModificationFunction_AssociationSetCardinality(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AssociationSetCardinality), p0);
    }

    internal static string Mapping_ModificationFunction_ComplexTypeNotFound(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_ComplexTypeNotFound), p0);
    }

    internal static string Mapping_ModificationFunction_WrongComplexType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_WrongComplexType), p0, p1);
    }

    internal static string Mapping_ModificationFunction_MissingVersion
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_ModificationFunction_MissingVersion));
      }
    }

    internal static string Mapping_ModificationFunction_VersionMustBeOriginal
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_ModificationFunction_VersionMustBeOriginal));
      }
    }

    internal static string Mapping_ModificationFunction_VersionMustBeCurrent
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_ModificationFunction_VersionMustBeCurrent));
      }
    }

    internal static string Mapping_ModificationFunction_ParameterNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_ParameterNotFound), p0, p1);
    }

    internal static string Mapping_ModificationFunction_PropertyNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_PropertyNotFound), p0, p1);
    }

    internal static string Mapping_ModificationFunction_PropertyNotKey(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_PropertyNotKey), p0, p1);
    }

    internal static string Mapping_ModificationFunction_ParameterBoundTwice(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_ParameterBoundTwice), p0);
    }

    internal static string Mapping_ModificationFunction_RedundantEntityTypeMapping(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_RedundantEntityTypeMapping), p0);
    }

    internal static string Mapping_ModificationFunction_MissingSetClosure(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_MissingSetClosure), p0);
    }

    internal static string Mapping_ModificationFunction_MissingEntityType(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_MissingEntityType), p0);
    }

    internal static string Mapping_ModificationFunction_PropertyParameterTypeMismatch(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_PropertyParameterTypeMismatch), p0, p1, p2, p3, p4, p5);
    }

    internal static string Mapping_ModificationFunction_AssociationSetAmbiguous(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AssociationSetAmbiguous), p0);
    }

    internal static string Mapping_ModificationFunction_MultipleEndsOfAssociationMapped(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_MultipleEndsOfAssociationMapped), p0, p1, p2);
    }

    internal static string Mapping_ModificationFunction_AmbiguousResultBinding(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AmbiguousResultBinding), p0, p1);
    }

    internal static string Mapping_ModificationFunction_AssociationSetNotMappedForOperation(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AssociationSetNotMappedForOperation), p0, p1, p2, p3);
    }

    internal static string Mapping_ModificationFunction_AssociationEndMappingInvalidForEntityType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AssociationEndMappingInvalidForEntityType), p0, p1, p2);
    }

    internal static string Mapping_ModificationFunction_AssociationEndMappingForeignKeyAssociation(
      object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ModificationFunction_AssociationEndMappingForeignKeyAssociation), p0);
    }

    internal static string Mapping_StoreTypeMismatch_ScalarPropertyMapping(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_StoreTypeMismatch_ScalarPropertyMapping), p0, p1);
    }

    internal static string Mapping_DistinctFlagInReadWriteContainer
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_DistinctFlagInReadWriteContainer));
      }
    }

    internal static string Mapping_ProviderReturnsNullType(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_ProviderReturnsNullType), p0);
    }

    internal static string Mapping_DifferentEdmStoreVersion
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_DifferentEdmStoreVersion));
      }
    }

    internal static string Mapping_DifferentMappingEdmStoreVersion
    {
      get
      {
        return EntityRes.GetString(nameof (Mapping_DifferentMappingEdmStoreVersion));
      }
    }

    internal static string Mapping_FunctionImport_StoreFunctionDoesNotExist(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_StoreFunctionDoesNotExist), p0);
    }

    internal static string Mapping_FunctionImport_FunctionImportDoesNotExist(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_FunctionImportDoesNotExist), p0, p1);
    }

    internal static string Mapping_FunctionImport_FunctionImportMappedMultipleTimes(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_FunctionImportMappedMultipleTimes), p0);
    }

    internal static string Mapping_FunctionImport_TargetFunctionMustBeNonComposable(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_TargetFunctionMustBeNonComposable), p0, p1);
    }

    internal static string Mapping_FunctionImport_TargetFunctionMustBeComposable(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_TargetFunctionMustBeComposable), p0, p1);
    }

    internal static string Mapping_FunctionImport_TargetParameterHasNoCorrespondingImportParameter(
      object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_TargetParameterHasNoCorrespondingImportParameter), p0);
    }

    internal static string Mapping_FunctionImport_ImportParameterHasNoCorrespondingTargetParameter(
      object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ImportParameterHasNoCorrespondingTargetParameter), p0);
    }

    internal static string Mapping_FunctionImport_IncompatibleParameterMode(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_IncompatibleParameterMode), p0, p1, p2);
    }

    internal static string Mapping_FunctionImport_IncompatibleParameterType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_IncompatibleParameterType), p0, p1, p2);
    }

    internal static string Mapping_FunctionImport_IncompatibleEnumParameterType(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_IncompatibleEnumParameterType), p0, p1, p2, p3);
    }

    internal static string Mapping_FunctionImport_RowsAffectedParameterDoesNotExist(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_RowsAffectedParameterDoesNotExist), p0, p1);
    }

    internal static string Mapping_FunctionImport_RowsAffectedParameterHasWrongType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_RowsAffectedParameterHasWrongType), p0, p1);
    }

    internal static string Mapping_FunctionImport_RowsAffectedParameterHasWrongMode(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_RowsAffectedParameterHasWrongMode), p0, p1, p2, p3);
    }

    internal static string Mapping_FunctionImport_EntityTypeMappingForFunctionNotReturningEntitySet(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_EntityTypeMappingForFunctionNotReturningEntitySet), p0, p1);
    }

    internal static string Mapping_FunctionImport_InvalidContentEntityTypeForEntitySet(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_InvalidContentEntityTypeForEntitySet), p0, p1, p2, p3);
    }

    internal static string Mapping_FunctionImport_ConditionValueTypeMismatch(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ConditionValueTypeMismatch), p0, p1, p2);
    }

    internal static string Mapping_FunctionImport_UnsupportedType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_UnsupportedType), p0, p1);
    }

    internal static string Mapping_FunctionImport_ResultMappingCountDoesNotMatchResultCount(
      object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ResultMappingCountDoesNotMatchResultCount), p0);
    }

    internal static string Mapping_FunctionImport_ResultMapping_MappedTypeDoesNotMatchReturnType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ResultMapping_MappedTypeDoesNotMatchReturnType), p0, p1);
    }

    internal static string Mapping_FunctionImport_ResultMapping_InvalidCTypeCTExpected(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ResultMapping_InvalidCTypeCTExpected), p0);
    }

    internal static string Mapping_FunctionImport_ResultMapping_InvalidCTypeETExpected(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ResultMapping_InvalidCTypeETExpected), p0);
    }

    internal static string Mapping_FunctionImport_ResultMapping_InvalidSType(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ResultMapping_InvalidSType), p0);
    }

    internal static string Mapping_FunctionImport_PropertyNotMapped(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_PropertyNotMapped), p0, p1, p2);
    }

    internal static string Mapping_FunctionImport_ImplicitMappingForAbstractReturnType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ImplicitMappingForAbstractReturnType), p0, p1);
    }

    internal static string Mapping_FunctionImport_ScalarMappingToMulticolumnTVF(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ScalarMappingToMulticolumnTVF), p0, p1);
    }

    internal static string Mapping_FunctionImport_ScalarMappingTypeMismatch(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_ScalarMappingTypeMismatch), p0, p1, p2, p3);
    }

    internal static string Mapping_FunctionImport_UnreachableType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_UnreachableType), p0, p1);
    }

    internal static string Mapping_FunctionImport_UnreachableIsTypeOf(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_UnreachableIsTypeOf), p0, p1);
    }

    internal static string Mapping_FunctionImport_FunctionAmbiguous(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_FunctionAmbiguous), p0);
    }

    internal static string Mapping_FunctionImport_CannotInferTargetFunctionKeys(object p0)
    {
      return EntityRes.GetString(nameof (Mapping_FunctionImport_CannotInferTargetFunctionKeys), p0);
    }

    internal static string Entity_EntityCantHaveMultipleChangeTrackers
    {
      get
      {
        return EntityRes.GetString(nameof (Entity_EntityCantHaveMultipleChangeTrackers));
      }
    }

    internal static string ComplexObject_NullableComplexTypesNotSupported(object p0)
    {
      return EntityRes.GetString(nameof (ComplexObject_NullableComplexTypesNotSupported), p0);
    }

    internal static string ComplexObject_ComplexObjectAlreadyAttachedToParent
    {
      get
      {
        return EntityRes.GetString(nameof (ComplexObject_ComplexObjectAlreadyAttachedToParent));
      }
    }

    internal static string ComplexObject_ComplexChangeRequestedOnScalarProperty(object p0)
    {
      return EntityRes.GetString(nameof (ComplexObject_ComplexChangeRequestedOnScalarProperty), p0);
    }

    internal static string ObjectStateEntry_SetModifiedOnInvalidProperty(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_SetModifiedOnInvalidProperty), p0);
    }

    internal static string ObjectStateEntry_OriginalValuesDoesNotExist
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_OriginalValuesDoesNotExist));
      }
    }

    internal static string ObjectStateEntry_CurrentValuesDoesNotExist
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_CurrentValuesDoesNotExist));
      }
    }

    internal static string ObjectStateEntry_InvalidState
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_InvalidState));
      }
    }

    internal static string ObjectStateEntry_CannotModifyKeyProperty(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_CannotModifyKeyProperty), p0);
    }

    internal static string ObjectStateEntry_CantModifyRelationValues
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_CantModifyRelationValues));
      }
    }

    internal static string ObjectStateEntry_CantModifyRelationState
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_CantModifyRelationState));
      }
    }

    internal static string ObjectStateEntry_CantModifyDetachedDeletedEntries
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_CantModifyDetachedDeletedEntries));
      }
    }

    internal static string ObjectStateEntry_SetModifiedStates(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_SetModifiedStates), p0);
    }

    internal static string ObjectStateEntry_CantSetEntityKey
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_CantSetEntityKey));
      }
    }

    internal static string ObjectStateEntry_CannotAccessKeyEntryValues
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_CannotAccessKeyEntryValues));
      }
    }

    internal static string ObjectStateEntry_CannotModifyKeyEntryState
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_CannotModifyKeyEntryState));
      }
    }

    internal static string ObjectStateEntry_CannotDeleteOnKeyEntry
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_CannotDeleteOnKeyEntry));
      }
    }

    internal static string ObjectStateEntry_EntityMemberChangedWithoutEntityMemberChanging
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_EntityMemberChangedWithoutEntityMemberChanging));
      }
    }

    internal static string ObjectStateEntry_ChangeOnUnmappedProperty(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_ChangeOnUnmappedProperty), p0);
    }

    internal static string ObjectStateEntry_ChangeOnUnmappedComplexProperty(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_ChangeOnUnmappedComplexProperty), p0);
    }

    internal static string ObjectStateEntry_ChangedInDifferentStateFromChanging(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_ChangedInDifferentStateFromChanging), p0, p1);
    }

    internal static string ObjectStateEntry_UnableToEnumerateCollection(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_UnableToEnumerateCollection), p0, p1);
    }

    internal static string ObjectStateEntry_RelationshipAndKeyEntriesDoNotHaveRelationshipManagers
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_RelationshipAndKeyEntriesDoNotHaveRelationshipManagers));
      }
    }

    internal static string ObjectStateEntry_InvalidTypeForComplexTypeProperty
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateEntry_InvalidTypeForComplexTypeProperty));
      }
    }

    internal static string ObjectStateEntry_ComplexObjectUsedMultipleTimes(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_ComplexObjectUsedMultipleTimes), p0, p1);
    }

    internal static string ObjectStateEntry_SetOriginalComplexProperties(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_SetOriginalComplexProperties), p0);
    }

    internal static string ObjectStateEntry_NullOriginalValueForNonNullableProperty(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_NullOriginalValueForNonNullableProperty), p0, p1, p2);
    }

    internal static string ObjectStateEntry_SetOriginalPrimaryKey(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateEntry_SetOriginalPrimaryKey), p0);
    }

    internal static string ObjectStateManager_NoEntryExistForEntityKey
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_NoEntryExistForEntityKey));
      }
    }

    internal static string ObjectStateManager_NoEntryExistsForObject(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateManager_NoEntryExistsForObject), p0);
    }

    internal static string ObjectStateManager_EntityNotTracked
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_EntityNotTracked));
      }
    }

    internal static string ObjectStateManager_DetachedObjectStateEntriesDoesNotExistInObjectStateManager
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_DetachedObjectStateEntriesDoesNotExistInObjectStateManager));
      }
    }

    internal static string ObjectStateManager_ObjectStateManagerContainsThisEntityKey(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateManager_ObjectStateManagerContainsThisEntityKey), p0);
    }

    internal static string ObjectStateManager_DoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity(
      object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateManager_DoesnotAllowToReAddUnchangedOrModifiedOrDeletedEntity), p0);
    }

    internal static string ObjectStateManager_CannotFixUpKeyToExistingValues(object p0)
    {
      return EntityRes.GetString(nameof (ObjectStateManager_CannotFixUpKeyToExistingValues), p0);
    }

    internal static string ObjectStateManager_KeyPropertyDoesntMatchValueInKey
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_KeyPropertyDoesntMatchValueInKey));
      }
    }

    internal static string ObjectStateManager_KeyPropertyDoesntMatchValueInKeyForAttach
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_KeyPropertyDoesntMatchValueInKeyForAttach));
      }
    }

    internal static string ObjectStateManager_InvalidKey
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_InvalidKey));
      }
    }

    internal static string ObjectStateManager_EntityTypeDoesnotMatchtoEntitySetType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (ObjectStateManager_EntityTypeDoesnotMatchtoEntitySetType), p0, p1);
    }

    internal static string ObjectStateManager_AcceptChangesEntityKeyIsNotValid
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_AcceptChangesEntityKeyIsNotValid));
      }
    }

    internal static string ObjectStateManager_EntityConflictsWithKeyEntry
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_EntityConflictsWithKeyEntry));
      }
    }

    internal static string ObjectStateManager_CannotGetRelationshipManagerForDetachedPocoEntity
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_CannotGetRelationshipManagerForDetachedPocoEntity));
      }
    }

    internal static string ObjectStateManager_CannotChangeRelationshipStateEntityDeleted
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_CannotChangeRelationshipStateEntityDeleted));
      }
    }

    internal static string ObjectStateManager_CannotChangeRelationshipStateEntityAdded
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_CannotChangeRelationshipStateEntityAdded));
      }
    }

    internal static string ObjectStateManager_CannotChangeRelationshipStateKeyEntry
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_CannotChangeRelationshipStateKeyEntry));
      }
    }

    internal static string ObjectStateManager_ConflictingChangesOfRelationshipDetected(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (ObjectStateManager_ConflictingChangesOfRelationshipDetected), p0, p1);
    }

    internal static string ObjectStateManager_ChangeRelationshipStateNotSupportedForForeignKeyAssociations
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_ChangeRelationshipStateNotSupportedForForeignKeyAssociations));
      }
    }

    internal static string ObjectStateManager_ChangeStateFromAddedWithNullKeyIsInvalid
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectStateManager_ChangeStateFromAddedWithNullKeyIsInvalid));
      }
    }

    internal static string ObjectContext_ClientEntityRemovedFromStore(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_ClientEntityRemovedFromStore), p0);
    }

    internal static string ObjectContext_StoreEntityNotPresentInClient
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_StoreEntityNotPresentInClient));
      }
    }

    internal static string ObjectContext_InvalidConnectionString
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_InvalidConnectionString));
      }
    }

    internal static string ObjectContext_InvalidConnection
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_InvalidConnection));
      }
    }

    internal static string ObjectContext_InvalidDefaultContainerName(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_InvalidDefaultContainerName), p0);
    }

    internal static string ObjectContext_NthElementInAddedState(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_NthElementInAddedState), p0);
    }

    internal static string ObjectContext_NthElementIsDuplicate(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_NthElementIsDuplicate), p0);
    }

    internal static string ObjectContext_NthElementIsNull(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_NthElementIsNull), p0);
    }

    internal static string ObjectContext_NthElementNotInObjectStateManager(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_NthElementNotInObjectStateManager), p0);
    }

    internal static string ObjectContext_ObjectNotFound
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_ObjectNotFound));
      }
    }

    internal static string ObjectContext_CannotDeleteEntityNotInObjectStateManager
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_CannotDeleteEntityNotInObjectStateManager));
      }
    }

    internal static string ObjectContext_CannotDetachEntityNotInObjectStateManager
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_CannotDetachEntityNotInObjectStateManager));
      }
    }

    internal static string ObjectContext_EntitySetNotFoundForName(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_EntitySetNotFoundForName), p0);
    }

    internal static string ObjectContext_EntityContainerNotFoundForName(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_EntityContainerNotFoundForName), p0);
    }

    internal static string ObjectContext_InvalidCommandTimeout
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_InvalidCommandTimeout));
      }
    }

    internal static string ObjectContext_NoMappingForEntityType(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_NoMappingForEntityType), p0);
    }

    internal static string ObjectContext_EntityAlreadyExistsInObjectStateManager
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_EntityAlreadyExistsInObjectStateManager));
      }
    }

    internal static string ObjectContext_InvalidEntitySetInKey(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (ObjectContext_InvalidEntitySetInKey), p0, p1, p2, p3);
    }

    internal static string ObjectContext_CannotAttachEntityWithoutKey
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_CannotAttachEntityWithoutKey));
      }
    }

    internal static string ObjectContext_CannotAttachEntityWithTemporaryKey
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_CannotAttachEntityWithTemporaryKey));
      }
    }

    internal static string ObjectContext_EntitySetNameOrEntityKeyRequired
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_EntitySetNameOrEntityKeyRequired));
      }
    }

    internal static string ObjectContext_ExecuteFunctionTypeMismatch(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ObjectContext_ExecuteFunctionTypeMismatch), p0, p1);
    }

    internal static string ObjectContext_ExecuteFunctionCalledWithScalarFunction(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (ObjectContext_ExecuteFunctionCalledWithScalarFunction), p0, p1);
    }

    internal static string ObjectContext_ExecuteFunctionCalledWithNonQueryFunction(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_ExecuteFunctionCalledWithNonQueryFunction), p0);
    }

    internal static string ObjectContext_ExecuteFunctionCalledWithNullParameter(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_ExecuteFunctionCalledWithNullParameter), p0);
    }

    internal static string ObjectContext_ContainerQualifiedEntitySetNameRequired
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_ContainerQualifiedEntitySetNameRequired));
      }
    }

    internal static string ObjectContext_CannotSetDefaultContainerName
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_CannotSetDefaultContainerName));
      }
    }

    internal static string ObjectContext_QualfiedEntitySetName
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_QualfiedEntitySetName));
      }
    }

    internal static string ObjectContext_EntitiesHaveDifferentType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ObjectContext_EntitiesHaveDifferentType), p0, p1);
    }

    internal static string ObjectContext_EntityMustBeUnchangedOrModified(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_EntityMustBeUnchangedOrModified), p0);
    }

    internal static string ObjectContext_EntityMustBeUnchangedOrModifiedOrDeleted(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_EntityMustBeUnchangedOrModifiedOrDeleted), p0);
    }

    internal static string ObjectContext_AcceptAllChangesFailure(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_AcceptAllChangesFailure), p0);
    }

    internal static string ObjectContext_CommitWithConceptualNull
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_CommitWithConceptualNull));
      }
    }

    internal static string ObjectContext_InvalidEntitySetOnEntity(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ObjectContext_InvalidEntitySetOnEntity), p0, p1);
    }

    internal static string ObjectContext_InvalidObjectSetTypeForEntitySet(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ObjectContext_InvalidObjectSetTypeForEntitySet), p0, p1, p2);
    }

    internal static string ObjectContext_InvalidEntitySetInKeyFromName(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (ObjectContext_InvalidEntitySetInKeyFromName), p0, p1, p2, p3, p4);
    }

    internal static string ObjectContext_ObjectDisposed
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_ObjectDisposed));
      }
    }

    internal static string ObjectContext_CannotExplicitlyLoadDetachedRelationships(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_CannotExplicitlyLoadDetachedRelationships), p0);
    }

    internal static string ObjectContext_CannotLoadReferencesUsingDifferentContext(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_CannotLoadReferencesUsingDifferentContext), p0);
    }

    internal static string ObjectContext_SelectorExpressionMustBeMemberAccess
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_SelectorExpressionMustBeMemberAccess));
      }
    }

    internal static string ObjectContext_MultipleEntitySetsFoundInSingleContainer(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (ObjectContext_MultipleEntitySetsFoundInSingleContainer), p0, p1);
    }

    internal static string ObjectContext_MultipleEntitySetsFoundInAllContainers(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_MultipleEntitySetsFoundInAllContainers), p0);
    }

    internal static string ObjectContext_NoEntitySetFoundForType(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_NoEntitySetFoundForType), p0);
    }

    internal static string ObjectContext_EntityNotInObjectSet_Delete(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (ObjectContext_EntityNotInObjectSet_Delete), p0, p1, p2, p3);
    }

    internal static string ObjectContext_EntityNotInObjectSet_Detach(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (ObjectContext_EntityNotInObjectSet_Detach), p0, p1, p2, p3);
    }

    internal static string ObjectContext_InvalidEntityState
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_InvalidEntityState));
      }
    }

    internal static string ObjectContext_InvalidRelationshipState
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_InvalidRelationshipState));
      }
    }

    internal static string ObjectContext_EntityNotTrackedOrHasTempKey
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_EntityNotTrackedOrHasTempKey));
      }
    }

    internal static string ObjectContext_ExecuteCommandWithMixOfDbParameterAndValues
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectContext_ExecuteCommandWithMixOfDbParameterAndValues));
      }
    }

    internal static string ObjectContext_InvalidEntitySetForStoreQuery(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ObjectContext_InvalidEntitySetForStoreQuery), p0, p1, p2);
    }

    internal static string ObjectContext_InvalidTypeForStoreQuery(object p0)
    {
      return EntityRes.GetString(nameof (ObjectContext_InvalidTypeForStoreQuery), p0);
    }

    internal static string ObjectContext_TwoPropertiesMappedToSameColumn(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ObjectContext_TwoPropertiesMappedToSameColumn), p0, p1);
    }

    internal static string RelatedEnd_InvalidOwnerStateForAttach
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_InvalidOwnerStateForAttach));
      }
    }

    internal static string RelatedEnd_InvalidNthElementNullForAttach(object p0)
    {
      return EntityRes.GetString(nameof (RelatedEnd_InvalidNthElementNullForAttach), p0);
    }

    internal static string RelatedEnd_InvalidNthElementContextForAttach(object p0)
    {
      return EntityRes.GetString(nameof (RelatedEnd_InvalidNthElementContextForAttach), p0);
    }

    internal static string RelatedEnd_InvalidNthElementStateForAttach(object p0)
    {
      return EntityRes.GetString(nameof (RelatedEnd_InvalidNthElementStateForAttach), p0);
    }

    internal static string RelatedEnd_InvalidEntityContextForAttach
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_InvalidEntityContextForAttach));
      }
    }

    internal static string RelatedEnd_InvalidEntityStateForAttach
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_InvalidEntityStateForAttach));
      }
    }

    internal static string RelatedEnd_UnableToAddEntity
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_UnableToAddEntity));
      }
    }

    internal static string RelatedEnd_UnableToRemoveEntity
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_UnableToRemoveEntity));
      }
    }

    internal static string RelatedEnd_UnableToAddRelationshipWithDeletedEntity
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_UnableToAddRelationshipWithDeletedEntity));
      }
    }

    internal static string RelatedEnd_CannotSerialize(object p0)
    {
      return EntityRes.GetString(nameof (RelatedEnd_CannotSerialize), p0);
    }

    internal static string RelatedEnd_CannotAddToFixedSizeArray(object p0)
    {
      return EntityRes.GetString(nameof (RelatedEnd_CannotAddToFixedSizeArray), p0);
    }

    internal static string RelatedEnd_CannotRemoveFromFixedSizeArray(object p0)
    {
      return EntityRes.GetString(nameof (RelatedEnd_CannotRemoveFromFixedSizeArray), p0);
    }

    internal static string Materializer_PropertyIsNotNullable
    {
      get
      {
        return EntityRes.GetString(nameof (Materializer_PropertyIsNotNullable));
      }
    }

    internal static string Materializer_PropertyIsNotNullableWithName(object p0)
    {
      return EntityRes.GetString(nameof (Materializer_PropertyIsNotNullableWithName), p0);
    }

    internal static string Materializer_SetInvalidValue(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Materializer_SetInvalidValue), p0, p1, p2, p3);
    }

    internal static string Materializer_InvalidCastReference(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Materializer_InvalidCastReference), p0, p1);
    }

    internal static string Materializer_InvalidCastNullable(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Materializer_InvalidCastNullable), p0, p1);
    }

    internal static string Materializer_NullReferenceCast(object p0)
    {
      return EntityRes.GetString(nameof (Materializer_NullReferenceCast), p0);
    }

    internal static string Materializer_RecyclingEntity(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (Materializer_RecyclingEntity), p0, p1, p2, p3);
    }

    internal static string Materializer_AddedEntityAlreadyExists(object p0)
    {
      return EntityRes.GetString(nameof (Materializer_AddedEntityAlreadyExists), p0);
    }

    internal static string Materializer_CannotReEnumerateQueryResults
    {
      get
      {
        return EntityRes.GetString(nameof (Materializer_CannotReEnumerateQueryResults));
      }
    }

    internal static string Materializer_UnsupportedType
    {
      get
      {
        return EntityRes.GetString(nameof (Materializer_UnsupportedType));
      }
    }

    internal static string Collections_NoRelationshipSetMatched(object p0)
    {
      return EntityRes.GetString(nameof (Collections_NoRelationshipSetMatched), p0);
    }

    internal static string Collections_ExpectedCollectionGotReference(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Collections_ExpectedCollectionGotReference), p0, p1, p2);
    }

    internal static string Collections_InvalidEntityStateSource
    {
      get
      {
        return EntityRes.GetString(nameof (Collections_InvalidEntityStateSource));
      }
    }

    internal static string Collections_InvalidEntityStateLoad(object p0)
    {
      return EntityRes.GetString(nameof (Collections_InvalidEntityStateLoad), p0);
    }

    internal static string Collections_CannotFillTryDifferentMergeOption(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Collections_CannotFillTryDifferentMergeOption), p0, p1);
    }

    internal static string Collections_UnableToMergeCollections
    {
      get
      {
        return EntityRes.GetString(nameof (Collections_UnableToMergeCollections));
      }
    }

    internal static string EntityReference_ExpectedReferenceGotCollection(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EntityReference_ExpectedReferenceGotCollection), p0, p1, p2);
    }

    internal static string EntityReference_CannotAddMoreThanOneEntityToEntityReference(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EntityReference_CannotAddMoreThanOneEntityToEntityReference), p0, p1);
    }

    internal static string EntityReference_LessThanExpectedRelatedEntitiesFound
    {
      get
      {
        return EntityRes.GetString(nameof (EntityReference_LessThanExpectedRelatedEntitiesFound));
      }
    }

    internal static string EntityReference_MoreThanExpectedRelatedEntitiesFound
    {
      get
      {
        return EntityRes.GetString(nameof (EntityReference_MoreThanExpectedRelatedEntitiesFound));
      }
    }

    internal static string EntityReference_CannotChangeReferentialConstraintProperty
    {
      get
      {
        return EntityRes.GetString(nameof (EntityReference_CannotChangeReferentialConstraintProperty));
      }
    }

    internal static string EntityReference_CannotSetSpecialKeys
    {
      get
      {
        return EntityRes.GetString(nameof (EntityReference_CannotSetSpecialKeys));
      }
    }

    internal static string EntityReference_EntityKeyValueMismatch
    {
      get
      {
        return EntityRes.GetString(nameof (EntityReference_EntityKeyValueMismatch));
      }
    }

    internal static string RelatedEnd_RelatedEndNotFound
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_RelatedEndNotFound));
      }
    }

    internal static string RelatedEnd_RelatedEndNotAttachedToContext(object p0)
    {
      return EntityRes.GetString(nameof (RelatedEnd_RelatedEndNotAttachedToContext), p0);
    }

    internal static string RelatedEnd_LoadCalledOnNonEmptyNoTrackedRelatedEnd
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_LoadCalledOnNonEmptyNoTrackedRelatedEnd));
      }
    }

    internal static string RelatedEnd_LoadCalledOnAlreadyLoadedNoTrackedRelatedEnd
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_LoadCalledOnAlreadyLoadedNoTrackedRelatedEnd));
      }
    }

    internal static string RelatedEnd_InvalidContainedType_Collection(object p0, object p1)
    {
      return EntityRes.GetString(nameof (RelatedEnd_InvalidContainedType_Collection), p0, p1);
    }

    internal static string RelatedEnd_InvalidContainedType_Reference(object p0, object p1)
    {
      return EntityRes.GetString(nameof (RelatedEnd_InvalidContainedType_Reference), p0, p1);
    }

    internal static string RelatedEnd_CannotCreateRelationshipBetweenTrackedAndNoTrackedEntities(
      object p0)
    {
      return EntityRes.GetString(nameof (RelatedEnd_CannotCreateRelationshipBetweenTrackedAndNoTrackedEntities), p0);
    }

    internal static string RelatedEnd_CannotCreateRelationshipEntitiesInDifferentContexts
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_CannotCreateRelationshipEntitiesInDifferentContexts));
      }
    }

    internal static string RelatedEnd_MismatchedMergeOptionOnLoad(object p0)
    {
      return EntityRes.GetString(nameof (RelatedEnd_MismatchedMergeOptionOnLoad), p0);
    }

    internal static string RelatedEnd_EntitySetIsNotValidForRelationship(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (RelatedEnd_EntitySetIsNotValidForRelationship), p0, p1, p2, p3, p4);
    }

    internal static string RelatedEnd_OwnerIsNull
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEnd_OwnerIsNull));
      }
    }

    internal static string RelationshipManager_UnableToRetrieveReferentialConstraintProperties
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipManager_UnableToRetrieveReferentialConstraintProperties));
      }
    }

    internal static string RelationshipManager_InconsistentReferentialConstraintProperties(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (RelationshipManager_InconsistentReferentialConstraintProperties), p0, p1);
    }

    internal static string RelationshipManager_CircularRelationshipsWithReferentialConstraints
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipManager_CircularRelationshipsWithReferentialConstraints));
      }
    }

    internal static string RelationshipManager_UnableToFindRelationshipTypeInMetadata(object p0)
    {
      return EntityRes.GetString(nameof (RelationshipManager_UnableToFindRelationshipTypeInMetadata), p0);
    }

    internal static string RelationshipManager_InvalidTargetRole(object p0, object p1)
    {
      return EntityRes.GetString(nameof (RelationshipManager_InvalidTargetRole), p0, p1);
    }

    internal static string RelationshipManager_UnexpectedNull
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipManager_UnexpectedNull));
      }
    }

    internal static string RelationshipManager_InvalidRelationshipManagerOwner
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipManager_InvalidRelationshipManagerOwner));
      }
    }

    internal static string RelationshipManager_OwnerIsNotSourceType(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (RelationshipManager_OwnerIsNotSourceType), p0, p1, p2, p3);
    }

    internal static string RelationshipManager_UnexpectedNullContext
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipManager_UnexpectedNullContext));
      }
    }

    internal static string RelationshipManager_ReferenceAlreadyInitialized(object p0)
    {
      return EntityRes.GetString(nameof (RelationshipManager_ReferenceAlreadyInitialized), p0);
    }

    internal static string RelationshipManager_RelationshipManagerAttached(object p0)
    {
      return EntityRes.GetString(nameof (RelationshipManager_RelationshipManagerAttached), p0);
    }

    internal static string RelationshipManager_InitializeIsForDeserialization
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipManager_InitializeIsForDeserialization));
      }
    }

    internal static string RelationshipManager_CollectionAlreadyInitialized(object p0)
    {
      return EntityRes.GetString(nameof (RelationshipManager_CollectionAlreadyInitialized), p0);
    }

    internal static string RelationshipManager_CollectionRelationshipManagerAttached(object p0)
    {
      return EntityRes.GetString(nameof (RelationshipManager_CollectionRelationshipManagerAttached), p0);
    }

    internal static string RelationshipManager_CollectionInitializeIsForDeserialization
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipManager_CollectionInitializeIsForDeserialization));
      }
    }

    internal static string RelationshipManager_NavigationPropertyNotFound(object p0)
    {
      return EntityRes.GetString(nameof (RelationshipManager_NavigationPropertyNotFound), p0);
    }

    internal static string RelationshipManager_CannotGetRelatEndForDetachedPocoEntity
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipManager_CannotGetRelatEndForDetachedPocoEntity));
      }
    }

    internal static string ObjectView_CannotReplacetheEntityorRow
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectView_CannotReplacetheEntityorRow));
      }
    }

    internal static string ObjectView_IndexBasedInsertIsNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectView_IndexBasedInsertIsNotSupported));
      }
    }

    internal static string ObjectView_WriteOperationNotAllowedOnReadOnlyBindingList
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectView_WriteOperationNotAllowedOnReadOnlyBindingList));
      }
    }

    internal static string ObjectView_AddNewOperationNotAllowedOnAbstractBindingList
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectView_AddNewOperationNotAllowedOnAbstractBindingList));
      }
    }

    internal static string ObjectView_IncompatibleArgument
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectView_IncompatibleArgument));
      }
    }

    internal static string ObjectView_CannotResolveTheEntitySet(object p0)
    {
      return EntityRes.GetString(nameof (ObjectView_CannotResolveTheEntitySet), p0);
    }

    internal static string CodeGen_ConstructorNoParameterless(object p0)
    {
      return EntityRes.GetString(nameof (CodeGen_ConstructorNoParameterless), p0);
    }

    internal static string CodeGen_PropertyDeclaringTypeIsValueType
    {
      get
      {
        return EntityRes.GetString(nameof (CodeGen_PropertyDeclaringTypeIsValueType));
      }
    }

    internal static string CodeGen_PropertyUnsupportedType
    {
      get
      {
        return EntityRes.GetString(nameof (CodeGen_PropertyUnsupportedType));
      }
    }

    internal static string CodeGen_PropertyIsIndexed
    {
      get
      {
        return EntityRes.GetString(nameof (CodeGen_PropertyIsIndexed));
      }
    }

    internal static string CodeGen_PropertyIsStatic
    {
      get
      {
        return EntityRes.GetString(nameof (CodeGen_PropertyIsStatic));
      }
    }

    internal static string CodeGen_PropertyNoGetter
    {
      get
      {
        return EntityRes.GetString(nameof (CodeGen_PropertyNoGetter));
      }
    }

    internal static string CodeGen_PropertyNoSetter
    {
      get
      {
        return EntityRes.GetString(nameof (CodeGen_PropertyNoSetter));
      }
    }

    internal static string PocoEntityWrapper_UnableToSetFieldOrProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (PocoEntityWrapper_UnableToSetFieldOrProperty), p0, p1);
    }

    internal static string PocoEntityWrapper_UnexpectedTypeForNavigationProperty(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (PocoEntityWrapper_UnexpectedTypeForNavigationProperty), p0, p1);
    }

    internal static string PocoEntityWrapper_UnableToMaterializeArbitaryNavPropType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (PocoEntityWrapper_UnableToMaterializeArbitaryNavPropType), p0, p1);
    }

    internal static string GeneralQueryError
    {
      get
      {
        return EntityRes.GetString(nameof (GeneralQueryError));
      }
    }

    internal static string CtxAlias
    {
      get
      {
        return EntityRes.GetString(nameof (CtxAlias));
      }
    }

    internal static string CtxAliasedNamespaceImport
    {
      get
      {
        return EntityRes.GetString(nameof (CtxAliasedNamespaceImport));
      }
    }

    internal static string CtxAnd
    {
      get
      {
        return EntityRes.GetString(nameof (CtxAnd));
      }
    }

    internal static string CtxAnyElement
    {
      get
      {
        return EntityRes.GetString(nameof (CtxAnyElement));
      }
    }

    internal static string CtxApplyClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxApplyClause));
      }
    }

    internal static string CtxBetween
    {
      get
      {
        return EntityRes.GetString(nameof (CtxBetween));
      }
    }

    internal static string CtxCase
    {
      get
      {
        return EntityRes.GetString(nameof (CtxCase));
      }
    }

    internal static string CtxCaseElse
    {
      get
      {
        return EntityRes.GetString(nameof (CtxCaseElse));
      }
    }

    internal static string CtxCaseWhenThen
    {
      get
      {
        return EntityRes.GetString(nameof (CtxCaseWhenThen));
      }
    }

    internal static string CtxCast
    {
      get
      {
        return EntityRes.GetString(nameof (CtxCast));
      }
    }

    internal static string CtxCollatedOrderByClauseItem
    {
      get
      {
        return EntityRes.GetString(nameof (CtxCollatedOrderByClauseItem));
      }
    }

    internal static string CtxCollectionTypeDefinition
    {
      get
      {
        return EntityRes.GetString(nameof (CtxCollectionTypeDefinition));
      }
    }

    internal static string CtxCommandExpression
    {
      get
      {
        return EntityRes.GetString(nameof (CtxCommandExpression));
      }
    }

    internal static string CtxCreateRef
    {
      get
      {
        return EntityRes.GetString(nameof (CtxCreateRef));
      }
    }

    internal static string CtxDeref
    {
      get
      {
        return EntityRes.GetString(nameof (CtxDeref));
      }
    }

    internal static string CtxDivide
    {
      get
      {
        return EntityRes.GetString(nameof (CtxDivide));
      }
    }

    internal static string CtxElement
    {
      get
      {
        return EntityRes.GetString(nameof (CtxElement));
      }
    }

    internal static string CtxEquals
    {
      get
      {
        return EntityRes.GetString(nameof (CtxEquals));
      }
    }

    internal static string CtxEscapedIdentifier
    {
      get
      {
        return EntityRes.GetString(nameof (CtxEscapedIdentifier));
      }
    }

    internal static string CtxExcept
    {
      get
      {
        return EntityRes.GetString(nameof (CtxExcept));
      }
    }

    internal static string CtxExists
    {
      get
      {
        return EntityRes.GetString(nameof (CtxExists));
      }
    }

    internal static string CtxExpressionList
    {
      get
      {
        return EntityRes.GetString(nameof (CtxExpressionList));
      }
    }

    internal static string CtxFlatten
    {
      get
      {
        return EntityRes.GetString(nameof (CtxFlatten));
      }
    }

    internal static string CtxFromApplyClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxFromApplyClause));
      }
    }

    internal static string CtxFromClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxFromClause));
      }
    }

    internal static string CtxFromClauseItem
    {
      get
      {
        return EntityRes.GetString(nameof (CtxFromClauseItem));
      }
    }

    internal static string CtxFromClauseList
    {
      get
      {
        return EntityRes.GetString(nameof (CtxFromClauseList));
      }
    }

    internal static string CtxFromJoinClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxFromJoinClause));
      }
    }

    internal static string CtxFunction(object p0)
    {
      return EntityRes.GetString(nameof (CtxFunction), p0);
    }

    internal static string CtxFunctionDefinition
    {
      get
      {
        return EntityRes.GetString(nameof (CtxFunctionDefinition));
      }
    }

    internal static string CtxGreaterThan
    {
      get
      {
        return EntityRes.GetString(nameof (CtxGreaterThan));
      }
    }

    internal static string CtxGreaterThanEqual
    {
      get
      {
        return EntityRes.GetString(nameof (CtxGreaterThanEqual));
      }
    }

    internal static string CtxGroupByClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxGroupByClause));
      }
    }

    internal static string CtxGroupPartition
    {
      get
      {
        return EntityRes.GetString(nameof (CtxGroupPartition));
      }
    }

    internal static string CtxHavingClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxHavingClause));
      }
    }

    internal static string CtxIdentifier
    {
      get
      {
        return EntityRes.GetString(nameof (CtxIdentifier));
      }
    }

    internal static string CtxIn
    {
      get
      {
        return EntityRes.GetString(nameof (CtxIn));
      }
    }

    internal static string CtxIntersect
    {
      get
      {
        return EntityRes.GetString(nameof (CtxIntersect));
      }
    }

    internal static string CtxIsNotNull
    {
      get
      {
        return EntityRes.GetString(nameof (CtxIsNotNull));
      }
    }

    internal static string CtxIsNotOf
    {
      get
      {
        return EntityRes.GetString(nameof (CtxIsNotOf));
      }
    }

    internal static string CtxIsNull
    {
      get
      {
        return EntityRes.GetString(nameof (CtxIsNull));
      }
    }

    internal static string CtxIsOf
    {
      get
      {
        return EntityRes.GetString(nameof (CtxIsOf));
      }
    }

    internal static string CtxJoinClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxJoinClause));
      }
    }

    internal static string CtxJoinOnClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxJoinOnClause));
      }
    }

    internal static string CtxKey
    {
      get
      {
        return EntityRes.GetString(nameof (CtxKey));
      }
    }

    internal static string CtxLessThan
    {
      get
      {
        return EntityRes.GetString(nameof (CtxLessThan));
      }
    }

    internal static string CtxLessThanEqual
    {
      get
      {
        return EntityRes.GetString(nameof (CtxLessThanEqual));
      }
    }

    internal static string CtxLike
    {
      get
      {
        return EntityRes.GetString(nameof (CtxLike));
      }
    }

    internal static string CtxLimitSubClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxLimitSubClause));
      }
    }

    internal static string CtxLiteral
    {
      get
      {
        return EntityRes.GetString(nameof (CtxLiteral));
      }
    }

    internal static string CtxMemberAccess
    {
      get
      {
        return EntityRes.GetString(nameof (CtxMemberAccess));
      }
    }

    internal static string CtxMethod
    {
      get
      {
        return EntityRes.GetString(nameof (CtxMethod));
      }
    }

    internal static string CtxMinus
    {
      get
      {
        return EntityRes.GetString(nameof (CtxMinus));
      }
    }

    internal static string CtxModulus
    {
      get
      {
        return EntityRes.GetString(nameof (CtxModulus));
      }
    }

    internal static string CtxMultiply
    {
      get
      {
        return EntityRes.GetString(nameof (CtxMultiply));
      }
    }

    internal static string CtxMultisetCtor
    {
      get
      {
        return EntityRes.GetString(nameof (CtxMultisetCtor));
      }
    }

    internal static string CtxNamespaceImport
    {
      get
      {
        return EntityRes.GetString(nameof (CtxNamespaceImport));
      }
    }

    internal static string CtxNamespaceImportList
    {
      get
      {
        return EntityRes.GetString(nameof (CtxNamespaceImportList));
      }
    }

    internal static string CtxNavigate
    {
      get
      {
        return EntityRes.GetString(nameof (CtxNavigate));
      }
    }

    internal static string CtxNot
    {
      get
      {
        return EntityRes.GetString(nameof (CtxNot));
      }
    }

    internal static string CtxNotBetween
    {
      get
      {
        return EntityRes.GetString(nameof (CtxNotBetween));
      }
    }

    internal static string CtxNotEqual
    {
      get
      {
        return EntityRes.GetString(nameof (CtxNotEqual));
      }
    }

    internal static string CtxNotIn
    {
      get
      {
        return EntityRes.GetString(nameof (CtxNotIn));
      }
    }

    internal static string CtxNotLike
    {
      get
      {
        return EntityRes.GetString(nameof (CtxNotLike));
      }
    }

    internal static string CtxNullLiteral
    {
      get
      {
        return EntityRes.GetString(nameof (CtxNullLiteral));
      }
    }

    internal static string CtxOfType
    {
      get
      {
        return EntityRes.GetString(nameof (CtxOfType));
      }
    }

    internal static string CtxOfTypeOnly
    {
      get
      {
        return EntityRes.GetString(nameof (CtxOfTypeOnly));
      }
    }

    internal static string CtxOr
    {
      get
      {
        return EntityRes.GetString(nameof (CtxOr));
      }
    }

    internal static string CtxOrderByClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxOrderByClause));
      }
    }

    internal static string CtxOrderByClauseItem
    {
      get
      {
        return EntityRes.GetString(nameof (CtxOrderByClauseItem));
      }
    }

    internal static string CtxOverlaps
    {
      get
      {
        return EntityRes.GetString(nameof (CtxOverlaps));
      }
    }

    internal static string CtxParen
    {
      get
      {
        return EntityRes.GetString(nameof (CtxParen));
      }
    }

    internal static string CtxPlus
    {
      get
      {
        return EntityRes.GetString(nameof (CtxPlus));
      }
    }

    internal static string CtxTypeNameWithTypeSpec
    {
      get
      {
        return EntityRes.GetString(nameof (CtxTypeNameWithTypeSpec));
      }
    }

    internal static string CtxQueryExpression
    {
      get
      {
        return EntityRes.GetString(nameof (CtxQueryExpression));
      }
    }

    internal static string CtxQueryStatement
    {
      get
      {
        return EntityRes.GetString(nameof (CtxQueryStatement));
      }
    }

    internal static string CtxRef
    {
      get
      {
        return EntityRes.GetString(nameof (CtxRef));
      }
    }

    internal static string CtxRefTypeDefinition
    {
      get
      {
        return EntityRes.GetString(nameof (CtxRefTypeDefinition));
      }
    }

    internal static string CtxRelationship
    {
      get
      {
        return EntityRes.GetString(nameof (CtxRelationship));
      }
    }

    internal static string CtxRelationshipList
    {
      get
      {
        return EntityRes.GetString(nameof (CtxRelationshipList));
      }
    }

    internal static string CtxRowCtor
    {
      get
      {
        return EntityRes.GetString(nameof (CtxRowCtor));
      }
    }

    internal static string CtxRowTypeDefinition
    {
      get
      {
        return EntityRes.GetString(nameof (CtxRowTypeDefinition));
      }
    }

    internal static string CtxSelectRowClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxSelectRowClause));
      }
    }

    internal static string CtxSelectValueClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxSelectValueClause));
      }
    }

    internal static string CtxSet
    {
      get
      {
        return EntityRes.GetString(nameof (CtxSet));
      }
    }

    internal static string CtxSimpleIdentifier
    {
      get
      {
        return EntityRes.GetString(nameof (CtxSimpleIdentifier));
      }
    }

    internal static string CtxSkipSubClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxSkipSubClause));
      }
    }

    internal static string CtxTopSubClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxTopSubClause));
      }
    }

    internal static string CtxTreat
    {
      get
      {
        return EntityRes.GetString(nameof (CtxTreat));
      }
    }

    internal static string CtxTypeCtor(object p0)
    {
      return EntityRes.GetString(nameof (CtxTypeCtor), p0);
    }

    internal static string CtxTypeName
    {
      get
      {
        return EntityRes.GetString(nameof (CtxTypeName));
      }
    }

    internal static string CtxUnaryMinus
    {
      get
      {
        return EntityRes.GetString(nameof (CtxUnaryMinus));
      }
    }

    internal static string CtxUnaryPlus
    {
      get
      {
        return EntityRes.GetString(nameof (CtxUnaryPlus));
      }
    }

    internal static string CtxUnion
    {
      get
      {
        return EntityRes.GetString(nameof (CtxUnion));
      }
    }

    internal static string CtxUnionAll
    {
      get
      {
        return EntityRes.GetString(nameof (CtxUnionAll));
      }
    }

    internal static string CtxWhereClause
    {
      get
      {
        return EntityRes.GetString(nameof (CtxWhereClause));
      }
    }

    internal static string CannotConvertNumericLiteral(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CannotConvertNumericLiteral), p0, p1);
    }

    internal static string GenericSyntaxError
    {
      get
      {
        return EntityRes.GetString(nameof (GenericSyntaxError));
      }
    }

    internal static string InFromClause
    {
      get
      {
        return EntityRes.GetString(nameof (InFromClause));
      }
    }

    internal static string InGroupClause
    {
      get
      {
        return EntityRes.GetString(nameof (InGroupClause));
      }
    }

    internal static string InRowCtor
    {
      get
      {
        return EntityRes.GetString(nameof (InRowCtor));
      }
    }

    internal static string InSelectProjectionList
    {
      get
      {
        return EntityRes.GetString(nameof (InSelectProjectionList));
      }
    }

    internal static string InvalidAliasName(object p0)
    {
      return EntityRes.GetString(nameof (InvalidAliasName), p0);
    }

    internal static string InvalidEmptyIdentifier
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidEmptyIdentifier));
      }
    }

    internal static string InvalidEmptyQuery
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidEmptyQuery));
      }
    }

    internal static string InvalidEscapedIdentifier(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEscapedIdentifier), p0);
    }

    internal static string InvalidEscapedIdentifierUnbalanced(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEscapedIdentifierUnbalanced), p0);
    }

    internal static string InvalidOperatorSymbol
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidOperatorSymbol));
      }
    }

    internal static string InvalidPunctuatorSymbol
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidPunctuatorSymbol));
      }
    }

    internal static string InvalidSimpleIdentifier(object p0)
    {
      return EntityRes.GetString(nameof (InvalidSimpleIdentifier), p0);
    }

    internal static string InvalidSimpleIdentifierNonASCII(object p0)
    {
      return EntityRes.GetString(nameof (InvalidSimpleIdentifierNonASCII), p0);
    }

    internal static string LocalizedCollection
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedCollection));
      }
    }

    internal static string LocalizedColumn
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedColumn));
      }
    }

    internal static string LocalizedComplex
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedComplex));
      }
    }

    internal static string LocalizedEntity
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedEntity));
      }
    }

    internal static string LocalizedEntityContainerExpression
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedEntityContainerExpression));
      }
    }

    internal static string LocalizedFunction
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedFunction));
      }
    }

    internal static string LocalizedInlineFunction
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedInlineFunction));
      }
    }

    internal static string LocalizedKeyword
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedKeyword));
      }
    }

    internal static string LocalizedLeft
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedLeft));
      }
    }

    internal static string LocalizedLine
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedLine));
      }
    }

    internal static string LocalizedMetadataMemberExpression
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedMetadataMemberExpression));
      }
    }

    internal static string LocalizedNamespace
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedNamespace));
      }
    }

    internal static string LocalizedNear
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedNear));
      }
    }

    internal static string LocalizedPrimitive
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedPrimitive));
      }
    }

    internal static string LocalizedReference
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedReference));
      }
    }

    internal static string LocalizedRight
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedRight));
      }
    }

    internal static string LocalizedRow
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedRow));
      }
    }

    internal static string LocalizedTerm
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedTerm));
      }
    }

    internal static string LocalizedType
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedType));
      }
    }

    internal static string LocalizedEnumMember
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedEnumMember));
      }
    }

    internal static string LocalizedValueExpression
    {
      get
      {
        return EntityRes.GetString(nameof (LocalizedValueExpression));
      }
    }

    internal static string AliasNameAlreadyUsed(object p0)
    {
      return EntityRes.GetString(nameof (AliasNameAlreadyUsed), p0);
    }

    internal static string AmbiguousFunctionArguments
    {
      get
      {
        return EntityRes.GetString(nameof (AmbiguousFunctionArguments));
      }
    }

    internal static string AmbiguousMetadataMemberName(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (AmbiguousMetadataMemberName), p0, p1, p2);
    }

    internal static string ArgumentTypesAreIncompatible(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ArgumentTypesAreIncompatible), p0, p1);
    }

    internal static string BetweenLimitsCannotBeUntypedNulls
    {
      get
      {
        return EntityRes.GetString(nameof (BetweenLimitsCannotBeUntypedNulls));
      }
    }

    internal static string BetweenLimitsTypesAreNotCompatible(object p0, object p1)
    {
      return EntityRes.GetString(nameof (BetweenLimitsTypesAreNotCompatible), p0, p1);
    }

    internal static string BetweenLimitsTypesAreNotOrderComparable(object p0, object p1)
    {
      return EntityRes.GetString(nameof (BetweenLimitsTypesAreNotOrderComparable), p0, p1);
    }

    internal static string BetweenValueIsNotOrderComparable(object p0, object p1)
    {
      return EntityRes.GetString(nameof (BetweenValueIsNotOrderComparable), p0, p1);
    }

    internal static string CannotCreateEmptyMultiset
    {
      get
      {
        return EntityRes.GetString(nameof (CannotCreateEmptyMultiset));
      }
    }

    internal static string CannotCreateMultisetofNulls
    {
      get
      {
        return EntityRes.GetString(nameof (CannotCreateMultisetofNulls));
      }
    }

    internal static string CannotInstantiateAbstractType(object p0)
    {
      return EntityRes.GetString(nameof (CannotInstantiateAbstractType), p0);
    }

    internal static string CannotResolveNameToTypeOrFunction(object p0)
    {
      return EntityRes.GetString(nameof (CannotResolveNameToTypeOrFunction), p0);
    }

    internal static string ConcatBuiltinNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (ConcatBuiltinNotSupported));
      }
    }

    internal static string CouldNotResolveIdentifier(object p0)
    {
      return EntityRes.GetString(nameof (CouldNotResolveIdentifier), p0);
    }

    internal static string CreateRefTypeIdentifierMustBeASubOrSuperType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CreateRefTypeIdentifierMustBeASubOrSuperType), p0, p1);
    }

    internal static string CreateRefTypeIdentifierMustSpecifyAnEntityType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CreateRefTypeIdentifierMustSpecifyAnEntityType), p0, p1);
    }

    internal static string DeRefArgIsNotOfRefType(object p0)
    {
      return EntityRes.GetString(nameof (DeRefArgIsNotOfRefType), p0);
    }

    internal static string DuplicatedInlineFunctionOverload(object p0)
    {
      return EntityRes.GetString(nameof (DuplicatedInlineFunctionOverload), p0);
    }

    internal static string ElementOperatorIsNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (ElementOperatorIsNotSupported));
      }
    }

    internal static string MemberDoesNotBelongToEntityContainer(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MemberDoesNotBelongToEntityContainer), p0, p1);
    }

    internal static string ExpressionCannotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (ExpressionCannotBeNull));
      }
    }

    internal static string OfTypeExpressionElementTypeMustBeEntityType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (OfTypeExpressionElementTypeMustBeEntityType), p0, p1);
    }

    internal static string OfTypeExpressionElementTypeMustBeNominalType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (OfTypeExpressionElementTypeMustBeNominalType), p0, p1);
    }

    internal static string ExpressionMustBeCollection
    {
      get
      {
        return EntityRes.GetString(nameof (ExpressionMustBeCollection));
      }
    }

    internal static string ExpressionMustBeNumericType
    {
      get
      {
        return EntityRes.GetString(nameof (ExpressionMustBeNumericType));
      }
    }

    internal static string ExpressionTypeMustBeBoolean
    {
      get
      {
        return EntityRes.GetString(nameof (ExpressionTypeMustBeBoolean));
      }
    }

    internal static string ExpressionTypeMustBeEqualComparable
    {
      get
      {
        return EntityRes.GetString(nameof (ExpressionTypeMustBeEqualComparable));
      }
    }

    internal static string ExpressionTypeMustBeEntityType(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ExpressionTypeMustBeEntityType), p0, p1, p2);
    }

    internal static string ExpressionTypeMustBeNominalType(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ExpressionTypeMustBeNominalType), p0, p1, p2);
    }

    internal static string ExpressionTypeMustNotBeCollection
    {
      get
      {
        return EntityRes.GetString(nameof (ExpressionTypeMustNotBeCollection));
      }
    }

    internal static string ExprIsNotValidEntitySetForCreateRef
    {
      get
      {
        return EntityRes.GetString(nameof (ExprIsNotValidEntitySetForCreateRef));
      }
    }

    internal static string FailedToResolveAggregateFunction(object p0)
    {
      return EntityRes.GetString(nameof (FailedToResolveAggregateFunction), p0);
    }

    internal static string GeneralExceptionAsQueryInnerException(object p0)
    {
      return EntityRes.GetString(nameof (GeneralExceptionAsQueryInnerException), p0);
    }

    internal static string GroupingKeysMustBeEqualComparable
    {
      get
      {
        return EntityRes.GetString(nameof (GroupingKeysMustBeEqualComparable));
      }
    }

    internal static string GroupPartitionOutOfContext
    {
      get
      {
        return EntityRes.GetString(nameof (GroupPartitionOutOfContext));
      }
    }

    internal static string HavingRequiresGroupClause
    {
      get
      {
        return EntityRes.GetString(nameof (HavingRequiresGroupClause));
      }
    }

    internal static string ImcompatibleCreateRefKeyElementType
    {
      get
      {
        return EntityRes.GetString(nameof (ImcompatibleCreateRefKeyElementType));
      }
    }

    internal static string ImcompatibleCreateRefKeyType
    {
      get
      {
        return EntityRes.GetString(nameof (ImcompatibleCreateRefKeyType));
      }
    }

    internal static string InnerJoinMustHaveOnPredicate
    {
      get
      {
        return EntityRes.GetString(nameof (InnerJoinMustHaveOnPredicate));
      }
    }

    internal static string InvalidAssociationTypeForUnion(object p0)
    {
      return EntityRes.GetString(nameof (InvalidAssociationTypeForUnion), p0);
    }

    internal static string InvalidCaseResultTypes
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidCaseResultTypes));
      }
    }

    internal static string InvalidCaseWhenThenNullType
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidCaseWhenThenNullType));
      }
    }

    internal static string InvalidCast(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidCast), p0, p1);
    }

    internal static string InvalidCastExpressionType
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidCastExpressionType));
      }
    }

    internal static string InvalidCastType
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidCastType));
      }
    }

    internal static string InvalidComplexType(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (InvalidComplexType), p0, p1, p2, p3);
    }

    internal static string InvalidCreateRefKeyType
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidCreateRefKeyType));
      }
    }

    internal static string InvalidCtorArgumentType(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (InvalidCtorArgumentType), p0, p1, p2);
    }

    internal static string InvalidCtorUseOnType(object p0)
    {
      return EntityRes.GetString(nameof (InvalidCtorUseOnType), p0);
    }

    internal static string InvalidDateTimeOffsetLiteral(object p0)
    {
      return EntityRes.GetString(nameof (InvalidDateTimeOffsetLiteral), p0);
    }

    internal static string InvalidDay(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidDay), p0, p1);
    }

    internal static string InvalidDayInMonth(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (InvalidDayInMonth), p0, p1, p2);
    }

    internal static string InvalidDeRefProperty(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (InvalidDeRefProperty), p0, p1, p2);
    }

    internal static string InvalidDistinctArgumentInCtor
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidDistinctArgumentInCtor));
      }
    }

    internal static string InvalidDistinctArgumentInNonAggFunction
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidDistinctArgumentInNonAggFunction));
      }
    }

    internal static string InvalidEntityRootTypeArgument(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidEntityRootTypeArgument), p0, p1);
    }

    internal static string InvalidEntityTypeArgument(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (InvalidEntityTypeArgument), p0, p1, p2, p3);
    }

    internal static string InvalidExpressionResolutionClass(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidExpressionResolutionClass), p0, p1);
    }

    internal static string InvalidFlattenArgument
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidFlattenArgument));
      }
    }

    internal static string InvalidGroupIdentifierReference(object p0)
    {
      return EntityRes.GetString(nameof (InvalidGroupIdentifierReference), p0);
    }

    internal static string InvalidHour(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidHour), p0, p1);
    }

    internal static string InvalidImplicitRelationshipFromEnd(object p0)
    {
      return EntityRes.GetString(nameof (InvalidImplicitRelationshipFromEnd), p0);
    }

    internal static string InvalidImplicitRelationshipToEnd(object p0)
    {
      return EntityRes.GetString(nameof (InvalidImplicitRelationshipToEnd), p0);
    }

    internal static string InvalidInExprArgs(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidInExprArgs), p0, p1);
    }

    internal static string InvalidJoinLeftCorrelation
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidJoinLeftCorrelation));
      }
    }

    internal static string InvalidKeyArgument(object p0)
    {
      return EntityRes.GetString(nameof (InvalidKeyArgument), p0);
    }

    internal static string InvalidKeyTypeForCollation(object p0)
    {
      return EntityRes.GetString(nameof (InvalidKeyTypeForCollation), p0);
    }

    internal static string InvalidLiteralFormat(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidLiteralFormat), p0, p1);
    }

    internal static string InvalidMetadataMemberName
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidMetadataMemberName));
      }
    }

    internal static string InvalidMinute(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMinute), p0, p1);
    }

    internal static string InvalidModeForWithRelationshipClause
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidModeForWithRelationshipClause));
      }
    }

    internal static string InvalidMonth(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMonth), p0, p1);
    }

    internal static string InvalidNamespaceAlias
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidNamespaceAlias));
      }
    }

    internal static string InvalidNullArithmetic
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidNullArithmetic));
      }
    }

    internal static string InvalidNullComparison
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidNullComparison));
      }
    }

    internal static string InvalidNullLiteralForNonNullableMember(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidNullLiteralForNonNullableMember), p0, p1);
    }

    internal static string InvalidParameterFormat(object p0)
    {
      return EntityRes.GetString(nameof (InvalidParameterFormat), p0);
    }

    internal static string InvalidPlaceholderRootTypeArgument(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (InvalidPlaceholderRootTypeArgument), p0, p1, p2, p3);
    }

    internal static string InvalidPlaceholderTypeArgument(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4,
      object p5)
    {
      return EntityRes.GetString(nameof (InvalidPlaceholderTypeArgument), p0, p1, p2, p3, p4, p5);
    }

    internal static string InvalidPredicateForCrossJoin
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidPredicateForCrossJoin));
      }
    }

    internal static string InvalidRelationshipMember(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidRelationshipMember), p0, p1);
    }

    internal static string InvalidMetadataMemberClassResolution(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (InvalidMetadataMemberClassResolution), p0, p1, p2);
    }

    internal static string InvalidRootComplexType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidRootComplexType), p0, p1);
    }

    internal static string InvalidRootRowType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidRootRowType), p0, p1);
    }

    internal static string InvalidRowType(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (InvalidRowType), p0, p1, p2, p3);
    }

    internal static string InvalidSecond(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidSecond), p0, p1);
    }

    internal static string InvalidSelectValueAliasedExpression
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidSelectValueAliasedExpression));
      }
    }

    internal static string InvalidSelectValueList
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidSelectValueList));
      }
    }

    internal static string InvalidTypeForWithRelationshipClause
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidTypeForWithRelationshipClause));
      }
    }

    internal static string InvalidUnarySetOpArgument(object p0)
    {
      return EntityRes.GetString(nameof (InvalidUnarySetOpArgument), p0);
    }

    internal static string InvalidUnsignedTypeForUnaryMinusOperation(object p0)
    {
      return EntityRes.GetString(nameof (InvalidUnsignedTypeForUnaryMinusOperation), p0);
    }

    internal static string InvalidYear(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidYear), p0, p1);
    }

    internal static string InvalidWithRelationshipTargetEndMultiplicity(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidWithRelationshipTargetEndMultiplicity), p0, p1);
    }

    internal static string InvalidQueryResultType(object p0)
    {
      return EntityRes.GetString(nameof (InvalidQueryResultType), p0);
    }

    internal static string IsNullInvalidType
    {
      get
      {
        return EntityRes.GetString(nameof (IsNullInvalidType));
      }
    }

    internal static string KeyMustBeCorrelated(object p0)
    {
      return EntityRes.GetString(nameof (KeyMustBeCorrelated), p0);
    }

    internal static string LeftSetExpressionArgsMustBeCollection
    {
      get
      {
        return EntityRes.GetString(nameof (LeftSetExpressionArgsMustBeCollection));
      }
    }

    internal static string LikeArgMustBeStringType
    {
      get
      {
        return EntityRes.GetString(nameof (LikeArgMustBeStringType));
      }
    }

    internal static string LiteralTypeNotFoundInMetadata(object p0)
    {
      return EntityRes.GetString(nameof (LiteralTypeNotFoundInMetadata), p0);
    }

    internal static string MalformedSingleQuotePayload
    {
      get
      {
        return EntityRes.GetString(nameof (MalformedSingleQuotePayload));
      }
    }

    internal static string MalformedStringLiteralPayload
    {
      get
      {
        return EntityRes.GetString(nameof (MalformedStringLiteralPayload));
      }
    }

    internal static string MethodInvocationNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (MethodInvocationNotSupported));
      }
    }

    internal static string MultipleDefinitionsOfParameter(object p0)
    {
      return EntityRes.GetString(nameof (MultipleDefinitionsOfParameter), p0);
    }

    internal static string MultipleDefinitionsOfVariable(object p0)
    {
      return EntityRes.GetString(nameof (MultipleDefinitionsOfVariable), p0);
    }

    internal static string MultisetElemsAreNotTypeCompatible
    {
      get
      {
        return EntityRes.GetString(nameof (MultisetElemsAreNotTypeCompatible));
      }
    }

    internal static string NamespaceAliasAlreadyUsed(object p0)
    {
      return EntityRes.GetString(nameof (NamespaceAliasAlreadyUsed), p0);
    }

    internal static string NamespaceAlreadyImported(object p0)
    {
      return EntityRes.GetString(nameof (NamespaceAlreadyImported), p0);
    }

    internal static string NestedAggregateCannotBeUsedInAggregate(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NestedAggregateCannotBeUsedInAggregate), p0, p1);
    }

    internal static string NoAggrFunctionOverloadMatch(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (NoAggrFunctionOverloadMatch), p0, p1, p2);
    }

    internal static string NoCanonicalAggrFunctionOverloadMatch(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (NoCanonicalAggrFunctionOverloadMatch), p0, p1, p2);
    }

    internal static string NoCanonicalFunctionOverloadMatch(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (NoCanonicalFunctionOverloadMatch), p0, p1, p2);
    }

    internal static string NoFunctionOverloadMatch(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (NoFunctionOverloadMatch), p0, p1, p2);
    }

    internal static string NotAMemberOfCollection(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NotAMemberOfCollection), p0, p1);
    }

    internal static string NotAMemberOfType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NotAMemberOfType), p0, p1);
    }

    internal static string NotASuperOrSubType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NotASuperOrSubType), p0, p1);
    }

    internal static string NullLiteralCannotBePromotedToCollectionOfNulls
    {
      get
      {
        return EntityRes.GetString(nameof (NullLiteralCannotBePromotedToCollectionOfNulls));
      }
    }

    internal static string NumberOfTypeCtorIsLessThenFormalSpec(object p0)
    {
      return EntityRes.GetString(nameof (NumberOfTypeCtorIsLessThenFormalSpec), p0);
    }

    internal static string NumberOfTypeCtorIsMoreThenFormalSpec(object p0)
    {
      return EntityRes.GetString(nameof (NumberOfTypeCtorIsMoreThenFormalSpec), p0);
    }

    internal static string OrderByKeyIsNotOrderComparable
    {
      get
      {
        return EntityRes.GetString(nameof (OrderByKeyIsNotOrderComparable));
      }
    }

    internal static string OfTypeOnlyTypeArgumentCannotBeAbstract(object p0)
    {
      return EntityRes.GetString(nameof (OfTypeOnlyTypeArgumentCannotBeAbstract), p0);
    }

    internal static string ParameterTypeNotSupported(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ParameterTypeNotSupported), p0, p1);
    }

    internal static string ParameterWasNotDefined(object p0)
    {
      return EntityRes.GetString(nameof (ParameterWasNotDefined), p0);
    }

    internal static string PlaceholderExpressionMustBeCompatibleWithEdm64(object p0, object p1)
    {
      return EntityRes.GetString(nameof (PlaceholderExpressionMustBeCompatibleWithEdm64), p0, p1);
    }

    internal static string PlaceholderExpressionMustBeConstant(object p0)
    {
      return EntityRes.GetString(nameof (PlaceholderExpressionMustBeConstant), p0);
    }

    internal static string PlaceholderExpressionMustBeGreaterThanOrEqualToZero(object p0)
    {
      return EntityRes.GetString(nameof (PlaceholderExpressionMustBeGreaterThanOrEqualToZero), p0);
    }

    internal static string PlaceholderSetArgTypeIsNotEqualComparable(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (PlaceholderSetArgTypeIsNotEqualComparable), p0, p1, p2);
    }

    internal static string PlusLeftExpressionInvalidType
    {
      get
      {
        return EntityRes.GetString(nameof (PlusLeftExpressionInvalidType));
      }
    }

    internal static string PlusRightExpressionInvalidType
    {
      get
      {
        return EntityRes.GetString(nameof (PlusRightExpressionInvalidType));
      }
    }

    internal static string PrecisionMustBeGreaterThanScale(object p0, object p1)
    {
      return EntityRes.GetString(nameof (PrecisionMustBeGreaterThanScale), p0, p1);
    }

    internal static string RefArgIsNotOfEntityType(object p0)
    {
      return EntityRes.GetString(nameof (RefArgIsNotOfEntityType), p0);
    }

    internal static string RefTypeIdentifierMustSpecifyAnEntityType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (RefTypeIdentifierMustSpecifyAnEntityType), p0, p1);
    }

    internal static string RelatedEndExprTypeMustBeReference
    {
      get
      {
        return EntityRes.GetString(nameof (RelatedEndExprTypeMustBeReference));
      }
    }

    internal static string RelatedEndExprTypeMustBePromotoableToToEnd(object p0, object p1)
    {
      return EntityRes.GetString(nameof (RelatedEndExprTypeMustBePromotoableToToEnd), p0, p1);
    }

    internal static string RelationshipFromEndIsAmbiguos
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipFromEndIsAmbiguos));
      }
    }

    internal static string RelationshipTypeExpected(object p0)
    {
      return EntityRes.GetString(nameof (RelationshipTypeExpected), p0);
    }

    internal static string RelationshipToEndIsAmbiguos
    {
      get
      {
        return EntityRes.GetString(nameof (RelationshipToEndIsAmbiguos));
      }
    }

    internal static string RelationshipTargetMustBeUnique(object p0)
    {
      return EntityRes.GetString(nameof (RelationshipTargetMustBeUnique), p0);
    }

    internal static string ResultingExpressionTypeCannotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (ResultingExpressionTypeCannotBeNull));
      }
    }

    internal static string RightSetExpressionArgsMustBeCollection
    {
      get
      {
        return EntityRes.GetString(nameof (RightSetExpressionArgsMustBeCollection));
      }
    }

    internal static string RowCtorElementCannotBeNull
    {
      get
      {
        return EntityRes.GetString(nameof (RowCtorElementCannotBeNull));
      }
    }

    internal static string SelectDistinctMustBeEqualComparable
    {
      get
      {
        return EntityRes.GetString(nameof (SelectDistinctMustBeEqualComparable));
      }
    }

    internal static string SourceTypeMustBePromotoableToFromEndRelationType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (SourceTypeMustBePromotoableToFromEndRelationType), p0, p1);
    }

    internal static string TopAndLimitCannotCoexist
    {
      get
      {
        return EntityRes.GetString(nameof (TopAndLimitCannotCoexist));
      }
    }

    internal static string TopAndSkipCannotCoexist
    {
      get
      {
        return EntityRes.GetString(nameof (TopAndSkipCannotCoexist));
      }
    }

    internal static string TypeDoesNotSupportSpec(object p0)
    {
      return EntityRes.GetString(nameof (TypeDoesNotSupportSpec), p0);
    }

    internal static string TypeDoesNotSupportFacet(object p0, object p1)
    {
      return EntityRes.GetString(nameof (TypeDoesNotSupportFacet), p0, p1);
    }

    internal static string TypeArgumentCountMismatch(object p0, object p1)
    {
      return EntityRes.GetString(nameof (TypeArgumentCountMismatch), p0, p1);
    }

    internal static string TypeArgumentMustBeLiteral
    {
      get
      {
        return EntityRes.GetString(nameof (TypeArgumentMustBeLiteral));
      }
    }

    internal static string TypeArgumentBelowMin(object p0)
    {
      return EntityRes.GetString(nameof (TypeArgumentBelowMin), p0);
    }

    internal static string TypeArgumentExceedsMax(object p0)
    {
      return EntityRes.GetString(nameof (TypeArgumentExceedsMax), p0);
    }

    internal static string TypeArgumentIsNotValid
    {
      get
      {
        return EntityRes.GetString(nameof (TypeArgumentIsNotValid));
      }
    }

    internal static string TypeKindMismatch(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (TypeKindMismatch), p0, p1, p2, p3);
    }

    internal static string TypeMustBeInheritableType
    {
      get
      {
        return EntityRes.GetString(nameof (TypeMustBeInheritableType));
      }
    }

    internal static string TypeMustBeEntityType(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (TypeMustBeEntityType), p0, p1, p2);
    }

    internal static string TypeMustBeNominalType(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (TypeMustBeNominalType), p0, p1, p2);
    }

    internal static string TypeNameNotFound(object p0)
    {
      return EntityRes.GetString(nameof (TypeNameNotFound), p0);
    }

    internal static string GroupVarNotFoundInScope
    {
      get
      {
        return EntityRes.GetString(nameof (GroupVarNotFoundInScope));
      }
    }

    internal static string InvalidArgumentTypeForAggregateFunction
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidArgumentTypeForAggregateFunction));
      }
    }

    internal static string InvalidSavePoint
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidSavePoint));
      }
    }

    internal static string InvalidScopeIndex
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidScopeIndex));
      }
    }

    internal static string LiteralTypeNotSupported(object p0)
    {
      return EntityRes.GetString(nameof (LiteralTypeNotSupported), p0);
    }

    internal static string ParserFatalError
    {
      get
      {
        return EntityRes.GetString(nameof (ParserFatalError));
      }
    }

    internal static string ParserInputError
    {
      get
      {
        return EntityRes.GetString(nameof (ParserInputError));
      }
    }

    internal static string StackOverflowInParser
    {
      get
      {
        return EntityRes.GetString(nameof (StackOverflowInParser));
      }
    }

    internal static string UnknownAstCommandExpression
    {
      get
      {
        return EntityRes.GetString(nameof (UnknownAstCommandExpression));
      }
    }

    internal static string UnknownAstExpressionType
    {
      get
      {
        return EntityRes.GetString(nameof (UnknownAstExpressionType));
      }
    }

    internal static string UnknownBuiltInAstExpressionType
    {
      get
      {
        return EntityRes.GetString(nameof (UnknownBuiltInAstExpressionType));
      }
    }

    internal static string UnknownExpressionResolutionClass(object p0)
    {
      return EntityRes.GetString(nameof (UnknownExpressionResolutionClass), p0);
    }

    internal static string Cqt_General_UnsupportedExpression(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_General_UnsupportedExpression), p0);
    }

    internal static string Cqt_General_PolymorphicTypeRequired(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_General_PolymorphicTypeRequired), p0);
    }

    internal static string Cqt_General_PolymorphicArgRequired(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_General_PolymorphicArgRequired), p0);
    }

    internal static string Cqt_General_MetadataNotReadOnly
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_General_MetadataNotReadOnly));
      }
    }

    internal static string Cqt_General_NoProviderBooleanType
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_General_NoProviderBooleanType));
      }
    }

    internal static string Cqt_General_NoProviderIntegerType
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_General_NoProviderIntegerType));
      }
    }

    internal static string Cqt_General_NoProviderStringType
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_General_NoProviderStringType));
      }
    }

    internal static string Cqt_Metadata_EdmMemberIncorrectSpace
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Metadata_EdmMemberIncorrectSpace));
      }
    }

    internal static string Cqt_Metadata_EntitySetEntityContainerNull
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Metadata_EntitySetEntityContainerNull));
      }
    }

    internal static string Cqt_Metadata_EntitySetIncorrectSpace
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Metadata_EntitySetIncorrectSpace));
      }
    }

    internal static string Cqt_Metadata_EntityTypeNullKeyMembersInvalid
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Metadata_EntityTypeNullKeyMembersInvalid));
      }
    }

    internal static string Cqt_Metadata_EntityTypeEmptyKeyMembersInvalid
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Metadata_EntityTypeEmptyKeyMembersInvalid));
      }
    }

    internal static string Cqt_Metadata_FunctionReturnParameterNull
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Metadata_FunctionReturnParameterNull));
      }
    }

    internal static string Cqt_Metadata_FunctionIncorrectSpace
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Metadata_FunctionIncorrectSpace));
      }
    }

    internal static string Cqt_Metadata_FunctionParameterIncorrectSpace
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Metadata_FunctionParameterIncorrectSpace));
      }
    }

    internal static string Cqt_Metadata_TypeUsageIncorrectSpace
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Metadata_TypeUsageIncorrectSpace));
      }
    }

    internal static string Cqt_Exceptions_InvalidCommandTree
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Exceptions_InvalidCommandTree));
      }
    }

    internal static string Cqt_Util_CheckListEmptyInvalid
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Util_CheckListEmptyInvalid));
      }
    }

    internal static string Cqt_Util_CheckListDuplicateName(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (Cqt_Util_CheckListDuplicateName), p0, p1, p2);
    }

    internal static string Cqt_ExpressionLink_TypeMismatch(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Cqt_ExpressionLink_TypeMismatch), p0, p1);
    }

    internal static string Cqt_ExpressionList_IncorrectElementCount
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_ExpressionList_IncorrectElementCount));
      }
    }

    internal static string Cqt_Copier_EntityContainerNotFound(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Copier_EntityContainerNotFound), p0);
    }

    internal static string Cqt_Copier_EntitySetNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Cqt_Copier_EntitySetNotFound), p0, p1);
    }

    internal static string Cqt_Copier_FunctionNotFound(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Copier_FunctionNotFound), p0);
    }

    internal static string Cqt_Copier_PropertyNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Cqt_Copier_PropertyNotFound), p0, p1);
    }

    internal static string Cqt_Copier_NavPropertyNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Cqt_Copier_NavPropertyNotFound), p0, p1);
    }

    internal static string Cqt_Copier_EndNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Cqt_Copier_EndNotFound), p0, p1);
    }

    internal static string Cqt_Copier_TypeNotFound(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Copier_TypeNotFound), p0);
    }

    internal static string Cqt_CommandTree_InvalidDataSpace
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_CommandTree_InvalidDataSpace));
      }
    }

    internal static string Cqt_CommandTree_InvalidParameterName(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_CommandTree_InvalidParameterName), p0);
    }

    internal static string Cqt_Validator_InvalidIncompatibleParameterReferences(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Validator_InvalidIncompatibleParameterReferences), p0);
    }

    internal static string Cqt_Validator_InvalidOtherWorkspaceMetadata(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Validator_InvalidOtherWorkspaceMetadata), p0);
    }

    internal static string Cqt_Validator_InvalidIncorrectDataSpaceMetadata(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Cqt_Validator_InvalidIncorrectDataSpaceMetadata), p0, p1);
    }

    internal static string Cqt_Factory_NewCollectionInvalidCommonType
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Factory_NewCollectionInvalidCommonType));
      }
    }

    internal static string NoSuchProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NoSuchProperty), p0, p1);
    }

    internal static string Cqt_Factory_NoSuchRelationEnd
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Factory_NoSuchRelationEnd));
      }
    }

    internal static string Cqt_Factory_IncompatibleRelationEnds
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Factory_IncompatibleRelationEnds));
      }
    }

    internal static string Cqt_Factory_MethodResultTypeNotSupported(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Factory_MethodResultTypeNotSupported), p0);
    }

    internal static string Cqt_Aggregate_InvalidFunction
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Aggregate_InvalidFunction));
      }
    }

    internal static string Cqt_Binding_CollectionRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Binding_CollectionRequired));
      }
    }

    internal static string Cqt_GroupBinding_CollectionRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_GroupBinding_CollectionRequired));
      }
    }

    internal static string Cqt_Binary_CollectionsRequired(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Binary_CollectionsRequired), p0);
    }

    internal static string Cqt_Unary_CollectionRequired(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Unary_CollectionRequired), p0);
    }

    internal static string Cqt_And_BooleanArgumentsRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_And_BooleanArgumentsRequired));
      }
    }

    internal static string Cqt_Apply_DuplicateVariableNames
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Apply_DuplicateVariableNames));
      }
    }

    internal static string Cqt_Arithmetic_NumericCommonType
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Arithmetic_NumericCommonType));
      }
    }

    internal static string Cqt_Arithmetic_InvalidUnsignedTypeForUnaryMinus(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Arithmetic_InvalidUnsignedTypeForUnaryMinus), p0);
    }

    internal static string Cqt_Case_WhensMustEqualThens
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Case_WhensMustEqualThens));
      }
    }

    internal static string Cqt_Case_InvalidResultType
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Case_InvalidResultType));
      }
    }

    internal static string Cqt_Cast_InvalidCast(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Cqt_Cast_InvalidCast), p0, p1);
    }

    internal static string Cqt_Comparison_ComparableRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Comparison_ComparableRequired));
      }
    }

    internal static string Cqt_Constant_InvalidType
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Constant_InvalidType));
      }
    }

    internal static string Cqt_Constant_InvalidValueForType(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Constant_InvalidValueForType), p0);
    }

    internal static string Cqt_Constant_InvalidConstantType(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Constant_InvalidConstantType), p0);
    }

    internal static string Cqt_Constant_ClrEnumTypeDoesNotMatchEdmEnumType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Cqt_Constant_ClrEnumTypeDoesNotMatchEdmEnumType), p0, p1, p2);
    }

    internal static string Cqt_Distinct_InvalidCollection
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Distinct_InvalidCollection));
      }
    }

    internal static string Cqt_DeRef_RefRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_DeRef_RefRequired));
      }
    }

    internal static string Cqt_Element_InvalidArgumentForUnwrapSingleProperty
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Element_InvalidArgumentForUnwrapSingleProperty));
      }
    }

    internal static string Cqt_Function_VoidResultInvalid
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Function_VoidResultInvalid));
      }
    }

    internal static string Cqt_Function_NonComposableInExpression
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Function_NonComposableInExpression));
      }
    }

    internal static string Cqt_Function_CommandTextInExpression
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Function_CommandTextInExpression));
      }
    }

    internal static string Cqt_Function_CanonicalFunction_NotFound(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Function_CanonicalFunction_NotFound), p0);
    }

    internal static string Cqt_Function_CanonicalFunction_AmbiguousMatch(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Function_CanonicalFunction_AmbiguousMatch), p0);
    }

    internal static string Cqt_GetEntityRef_EntityRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_GetEntityRef_EntityRequired));
      }
    }

    internal static string Cqt_GetRefKey_RefRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_GetRefKey_RefRequired));
      }
    }

    internal static string Cqt_GroupBy_AtLeastOneKeyOrAggregate
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_GroupBy_AtLeastOneKeyOrAggregate));
      }
    }

    internal static string Cqt_GroupBy_KeyNotEqualityComparable(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_GroupBy_KeyNotEqualityComparable), p0);
    }

    internal static string Cqt_GroupBy_AggregateColumnExistsAsGroupColumn(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_GroupBy_AggregateColumnExistsAsGroupColumn), p0);
    }

    internal static string Cqt_GroupBy_MoreThanOneGroupAggregate
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_GroupBy_MoreThanOneGroupAggregate));
      }
    }

    internal static string Cqt_CrossJoin_AtLeastTwoInputs
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_CrossJoin_AtLeastTwoInputs));
      }
    }

    internal static string Cqt_CrossJoin_DuplicateVariableNames(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (Cqt_CrossJoin_DuplicateVariableNames), p0, p1, p2);
    }

    internal static string Cqt_IsNull_CollectionNotAllowed
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_IsNull_CollectionNotAllowed));
      }
    }

    internal static string Cqt_IsNull_InvalidType
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_IsNull_InvalidType));
      }
    }

    internal static string Cqt_InvalidTypeForSetOperation(object p0, object p1)
    {
      return EntityRes.GetString(nameof (Cqt_InvalidTypeForSetOperation), p0, p1);
    }

    internal static string Cqt_Join_DuplicateVariableNames
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Join_DuplicateVariableNames));
      }
    }

    internal static string Cqt_Limit_ConstantOrParameterRefRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Limit_ConstantOrParameterRefRequired));
      }
    }

    internal static string Cqt_Limit_IntegerRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Limit_IntegerRequired));
      }
    }

    internal static string Cqt_Limit_NonNegativeLimitRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Limit_NonNegativeLimitRequired));
      }
    }

    internal static string Cqt_NewInstance_CollectionTypeRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_NewInstance_CollectionTypeRequired));
      }
    }

    internal static string Cqt_NewInstance_StructuralTypeRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_NewInstance_StructuralTypeRequired));
      }
    }

    internal static string Cqt_NewInstance_CannotInstantiateMemberlessType(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_NewInstance_CannotInstantiateMemberlessType), p0);
    }

    internal static string Cqt_NewInstance_CannotInstantiateAbstractType(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_NewInstance_CannotInstantiateAbstractType), p0);
    }

    internal static string Cqt_NewInstance_IncompatibleRelatedEntity_SourceTypeNotValid
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_NewInstance_IncompatibleRelatedEntity_SourceTypeNotValid));
      }
    }

    internal static string Cqt_Not_BooleanArgumentRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Not_BooleanArgumentRequired));
      }
    }

    internal static string Cqt_Or_BooleanArgumentsRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Or_BooleanArgumentsRequired));
      }
    }

    internal static string Cqt_In_SameResultTypeRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_In_SameResultTypeRequired));
      }
    }

    internal static string Cqt_Property_InstanceRequiredForInstance
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Property_InstanceRequiredForInstance));
      }
    }

    internal static string Cqt_Ref_PolymorphicArgRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Ref_PolymorphicArgRequired));
      }
    }

    internal static string Cqt_RelatedEntityRef_TargetEndFromDifferentRelationship
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_RelatedEntityRef_TargetEndFromDifferentRelationship));
      }
    }

    internal static string Cqt_RelatedEntityRef_TargetEndMustBeAtMostOne
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_RelatedEntityRef_TargetEndMustBeAtMostOne));
      }
    }

    internal static string Cqt_RelatedEntityRef_TargetEndSameAsSourceEnd
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_RelatedEntityRef_TargetEndSameAsSourceEnd));
      }
    }

    internal static string Cqt_RelatedEntityRef_TargetEntityNotRef
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_RelatedEntityRef_TargetEntityNotRef));
      }
    }

    internal static string Cqt_RelatedEntityRef_TargetEntityNotCompatible
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_RelatedEntityRef_TargetEntityNotCompatible));
      }
    }

    internal static string Cqt_RelNav_NoCompositions
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_RelNav_NoCompositions));
      }
    }

    internal static string Cqt_RelNav_WrongSourceType(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_RelNav_WrongSourceType), p0);
    }

    internal static string Cqt_Skip_ConstantOrParameterRefRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Skip_ConstantOrParameterRefRequired));
      }
    }

    internal static string Cqt_Skip_IntegerRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Skip_IntegerRequired));
      }
    }

    internal static string Cqt_Skip_NonNegativeCountRequired
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Skip_NonNegativeCountRequired));
      }
    }

    internal static string Cqt_Sort_NonStringCollationInvalid
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Sort_NonStringCollationInvalid));
      }
    }

    internal static string Cqt_Sort_OrderComparable
    {
      get
      {
        return EntityRes.GetString(nameof (Cqt_Sort_OrderComparable));
      }
    }

    internal static string Cqt_UDF_FunctionDefinitionGenerationFailed(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_UDF_FunctionDefinitionGenerationFailed), p0);
    }

    internal static string Cqt_UDF_FunctionDefinitionWithCircularReference(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_UDF_FunctionDefinitionWithCircularReference), p0);
    }

    internal static string Cqt_UDF_FunctionDefinitionResultTypeMismatch(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (Cqt_UDF_FunctionDefinitionResultTypeMismatch), p0, p1, p2);
    }

    internal static string Cqt_UDF_FunctionHasNoDefinition(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_UDF_FunctionHasNoDefinition), p0);
    }

    internal static string Cqt_Validator_VarRefInvalid(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Validator_VarRefInvalid), p0);
    }

    internal static string Cqt_Validator_VarRefTypeMismatch(object p0)
    {
      return EntityRes.GetString(nameof (Cqt_Validator_VarRefTypeMismatch), p0);
    }

    internal static string Iqt_General_UnsupportedOp(object p0)
    {
      return EntityRes.GetString(nameof (Iqt_General_UnsupportedOp), p0);
    }

    internal static string Iqt_CTGen_UnexpectedAggregate
    {
      get
      {
        return EntityRes.GetString(nameof (Iqt_CTGen_UnexpectedAggregate));
      }
    }

    internal static string Iqt_CTGen_UnexpectedVarDefList
    {
      get
      {
        return EntityRes.GetString(nameof (Iqt_CTGen_UnexpectedVarDefList));
      }
    }

    internal static string Iqt_CTGen_UnexpectedVarDef
    {
      get
      {
        return EntityRes.GetString(nameof (Iqt_CTGen_UnexpectedVarDef));
      }
    }

    internal static string ADP_MustUseSequentialAccess
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_MustUseSequentialAccess));
      }
    }

    internal static string ADP_ProviderDoesNotSupportCommandTrees
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_ProviderDoesNotSupportCommandTrees));
      }
    }

    internal static string ADP_ClosedDataReaderError
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_ClosedDataReaderError));
      }
    }

    internal static string ADP_DataReaderClosed(object p0)
    {
      return EntityRes.GetString(nameof (ADP_DataReaderClosed), p0);
    }

    internal static string ADP_ImplicitlyClosedDataReaderError
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_ImplicitlyClosedDataReaderError));
      }
    }

    internal static string ADP_NoData
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_NoData));
      }
    }

    internal static string ADP_GetSchemaTableIsNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_GetSchemaTableIsNotSupported));
      }
    }

    internal static string ADP_InvalidDataReaderFieldCountForScalarType
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_InvalidDataReaderFieldCountForScalarType));
      }
    }

    internal static string ADP_InvalidDataReaderMissingColumnForType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_InvalidDataReaderMissingColumnForType), p0, p1);
    }

    internal static string ADP_InvalidDataReaderMissingDiscriminatorColumn(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_InvalidDataReaderMissingDiscriminatorColumn), p0, p1);
    }

    internal static string ADP_InvalidDataReaderUnableToDetermineType
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_InvalidDataReaderUnableToDetermineType));
      }
    }

    internal static string ADP_InvalidDataReaderUnableToMaterializeNonScalarType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (ADP_InvalidDataReaderUnableToMaterializeNonScalarType), p0, p1);
    }

    internal static string ADP_KeysRequiredForJoinOverNest(object p0)
    {
      return EntityRes.GetString(nameof (ADP_KeysRequiredForJoinOverNest), p0);
    }

    internal static string ADP_KeysRequiredForNesting
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_KeysRequiredForNesting));
      }
    }

    internal static string ADP_NestingNotSupported(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_NestingNotSupported), p0, p1);
    }

    internal static string ADP_NoQueryMappingView(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_NoQueryMappingView), p0, p1);
    }

    internal static string ADP_InternalProviderError(object p0)
    {
      return EntityRes.GetString(nameof (ADP_InternalProviderError), p0);
    }

    internal static string ADP_InvalidEnumerationValue(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_InvalidEnumerationValue), p0, p1);
    }

    internal static string ADP_InvalidBufferSizeOrIndex(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_InvalidBufferSizeOrIndex), p0, p1);
    }

    internal static string ADP_InvalidDataLength(object p0)
    {
      return EntityRes.GetString(nameof (ADP_InvalidDataLength), p0);
    }

    internal static string ADP_InvalidDataType(object p0)
    {
      return EntityRes.GetString(nameof (ADP_InvalidDataType), p0);
    }

    internal static string ADP_InvalidDestinationBufferIndex(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_InvalidDestinationBufferIndex), p0, p1);
    }

    internal static string ADP_InvalidSourceBufferIndex(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_InvalidSourceBufferIndex), p0, p1);
    }

    internal static string ADP_NonSequentialChunkAccess(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ADP_NonSequentialChunkAccess), p0, p1, p2);
    }

    internal static string ADP_NonSequentialColumnAccess(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_NonSequentialColumnAccess), p0, p1);
    }

    internal static string ADP_UnknownDataTypeCode(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ADP_UnknownDataTypeCode), p0, p1);
    }

    internal static string DataCategory_Data
    {
      get
      {
        return EntityRes.GetString(nameof (DataCategory_Data));
      }
    }

    internal static string DbParameter_Direction
    {
      get
      {
        return EntityRes.GetString(nameof (DbParameter_Direction));
      }
    }

    internal static string DbParameter_Size
    {
      get
      {
        return EntityRes.GetString(nameof (DbParameter_Size));
      }
    }

    internal static string DataCategory_Update
    {
      get
      {
        return EntityRes.GetString(nameof (DataCategory_Update));
      }
    }

    internal static string DbParameter_SourceColumn
    {
      get
      {
        return EntityRes.GetString(nameof (DbParameter_SourceColumn));
      }
    }

    internal static string DbParameter_SourceVersion
    {
      get
      {
        return EntityRes.GetString(nameof (DbParameter_SourceVersion));
      }
    }

    internal static string ADP_CollectionParameterElementIsNull(object p0)
    {
      return EntityRes.GetString(nameof (ADP_CollectionParameterElementIsNull), p0);
    }

    internal static string ADP_CollectionParameterElementIsNullOrEmpty(object p0)
    {
      return EntityRes.GetString(nameof (ADP_CollectionParameterElementIsNullOrEmpty), p0);
    }

    internal static string NonReturnParameterInReturnParameterCollection
    {
      get
      {
        return EntityRes.GetString(nameof (NonReturnParameterInReturnParameterCollection));
      }
    }

    internal static string ReturnParameterInInputParameterCollection
    {
      get
      {
        return EntityRes.GetString(nameof (ReturnParameterInInputParameterCollection));
      }
    }

    internal static string NullEntitySetsForFunctionReturningMultipleResultSets
    {
      get
      {
        return EntityRes.GetString(nameof (NullEntitySetsForFunctionReturningMultipleResultSets));
      }
    }

    internal static string NumberOfEntitySetsDoesNotMatchNumberOfReturnParameters
    {
      get
      {
        return EntityRes.GetString(nameof (NumberOfEntitySetsDoesNotMatchNumberOfReturnParameters));
      }
    }

    internal static string EntityParameterCollectionInvalidParameterName(object p0)
    {
      return EntityRes.GetString(nameof (EntityParameterCollectionInvalidParameterName), p0);
    }

    internal static string EntityParameterCollectionInvalidIndex(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityParameterCollectionInvalidIndex), p0, p1);
    }

    internal static string InvalidEntityParameterType(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEntityParameterType), p0);
    }

    internal static string EntityParameterContainedByAnotherCollection
    {
      get
      {
        return EntityRes.GetString(nameof (EntityParameterContainedByAnotherCollection));
      }
    }

    internal static string EntityParameterCollectionRemoveInvalidObject
    {
      get
      {
        return EntityRes.GetString(nameof (EntityParameterCollectionRemoveInvalidObject));
      }
    }

    internal static string ADP_ConnectionStringSyntax(object p0)
    {
      return EntityRes.GetString(nameof (ADP_ConnectionStringSyntax), p0);
    }

    internal static string ExpandingDataDirectoryFailed
    {
      get
      {
        return EntityRes.GetString(nameof (ExpandingDataDirectoryFailed));
      }
    }

    internal static string ADP_InvalidDataDirectory
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_InvalidDataDirectory));
      }
    }

    internal static string ADP_InvalidMultipartNameDelimiterUsage
    {
      get
      {
        return EntityRes.GetString(nameof (ADP_InvalidMultipartNameDelimiterUsage));
      }
    }

    internal static string ADP_InvalidSizeValue(object p0)
    {
      return EntityRes.GetString(nameof (ADP_InvalidSizeValue), p0);
    }

    internal static string ADP_KeywordNotSupported(object p0)
    {
      return EntityRes.GetString(nameof (ADP_KeywordNotSupported), p0);
    }

    internal static string ConstantFacetSpecifiedInSchema(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConstantFacetSpecifiedInSchema), p0, p1);
    }

    internal static string DuplicateAnnotation(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DuplicateAnnotation), p0, p1);
    }

    internal static string EmptyFile(object p0)
    {
      return EntityRes.GetString(nameof (EmptyFile), p0);
    }

    internal static string EmptySchemaTextReader
    {
      get
      {
        return EntityRes.GetString(nameof (EmptySchemaTextReader));
      }
    }

    internal static string EmptyName(object p0)
    {
      return EntityRes.GetString(nameof (EmptyName), p0);
    }

    internal static string InvalidName(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidName), p0, p1);
    }

    internal static string MissingName
    {
      get
      {
        return EntityRes.GetString(nameof (MissingName));
      }
    }

    internal static string UnexpectedXmlAttribute(object p0)
    {
      return EntityRes.GetString(nameof (UnexpectedXmlAttribute), p0);
    }

    internal static string UnexpectedXmlElement(object p0)
    {
      return EntityRes.GetString(nameof (UnexpectedXmlElement), p0);
    }

    internal static string TextNotAllowed(object p0)
    {
      return EntityRes.GetString(nameof (TextNotAllowed), p0);
    }

    internal static string UnexpectedXmlNodeType(object p0)
    {
      return EntityRes.GetString(nameof (UnexpectedXmlNodeType), p0);
    }

    internal static string MalformedXml(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MalformedXml), p0, p1);
    }

    internal static string ValueNotUnderstood(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ValueNotUnderstood), p0, p1);
    }

    internal static string EntityContainerAlreadyExists(object p0)
    {
      return EntityRes.GetString(nameof (EntityContainerAlreadyExists), p0);
    }

    internal static string TypeNameAlreadyDefinedDuplicate(object p0)
    {
      return EntityRes.GetString(nameof (TypeNameAlreadyDefinedDuplicate), p0);
    }

    internal static string PropertyNameAlreadyDefinedDuplicate(object p0)
    {
      return EntityRes.GetString(nameof (PropertyNameAlreadyDefinedDuplicate), p0);
    }

    internal static string DuplicateMemberNameInExtendedEntityContainer(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (DuplicateMemberNameInExtendedEntityContainer), p0, p1, p2);
    }

    internal static string DuplicateEntityContainerMemberName(object p0)
    {
      return EntityRes.GetString(nameof (DuplicateEntityContainerMemberName), p0);
    }

    internal static string PropertyTypeAlreadyDefined(object p0)
    {
      return EntityRes.GetString(nameof (PropertyTypeAlreadyDefined), p0);
    }

    internal static string InvalidSize(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (InvalidSize), p0, p1, p2, p3);
    }

    internal static string InvalidSystemReferenceId(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (InvalidSystemReferenceId), p0, p1, p2, p3);
    }

    internal static string BadNamespaceOrAlias(object p0)
    {
      return EntityRes.GetString(nameof (BadNamespaceOrAlias), p0);
    }

    internal static string MissingNamespaceAttribute
    {
      get
      {
        return EntityRes.GetString(nameof (MissingNamespaceAttribute));
      }
    }

    internal static string InvalidBaseTypeForStructuredType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidBaseTypeForStructuredType), p0, p1);
    }

    internal static string InvalidPropertyType(object p0)
    {
      return EntityRes.GetString(nameof (InvalidPropertyType), p0);
    }

    internal static string InvalidBaseTypeForItemType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidBaseTypeForItemType), p0, p1);
    }

    internal static string InvalidBaseTypeForNestedType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidBaseTypeForNestedType), p0, p1);
    }

    internal static string DefaultNotAllowed
    {
      get
      {
        return EntityRes.GetString(nameof (DefaultNotAllowed));
      }
    }

    internal static string FacetNotAllowed(object p0, object p1)
    {
      return EntityRes.GetString(nameof (FacetNotAllowed), p0, p1);
    }

    internal static string RequiredFacetMissing(object p0, object p1)
    {
      return EntityRes.GetString(nameof (RequiredFacetMissing), p0, p1);
    }

    internal static string InvalidDefaultBinaryWithNoMaxLength(object p0)
    {
      return EntityRes.GetString(nameof (InvalidDefaultBinaryWithNoMaxLength), p0);
    }

    internal static string InvalidDefaultIntegral(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (InvalidDefaultIntegral), p0, p1, p2);
    }

    internal static string InvalidDefaultDateTime(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidDefaultDateTime), p0, p1);
    }

    internal static string InvalidDefaultTime(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidDefaultTime), p0, p1);
    }

    internal static string InvalidDefaultDateTimeOffset(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidDefaultDateTimeOffset), p0, p1);
    }

    internal static string InvalidDefaultDecimal(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (InvalidDefaultDecimal), p0, p1, p2);
    }

    internal static string InvalidDefaultFloatingPoint(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (InvalidDefaultFloatingPoint), p0, p1, p2);
    }

    internal static string InvalidDefaultGuid(object p0)
    {
      return EntityRes.GetString(nameof (InvalidDefaultGuid), p0);
    }

    internal static string InvalidDefaultBoolean(object p0)
    {
      return EntityRes.GetString(nameof (InvalidDefaultBoolean), p0);
    }

    internal static string DuplicateMemberName(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (DuplicateMemberName), p0, p1, p2);
    }

    internal static string GeneratorErrorSeverityError
    {
      get
      {
        return EntityRes.GetString(nameof (GeneratorErrorSeverityError));
      }
    }

    internal static string GeneratorErrorSeverityWarning
    {
      get
      {
        return EntityRes.GetString(nameof (GeneratorErrorSeverityWarning));
      }
    }

    internal static string GeneratorErrorSeverityUnknown
    {
      get
      {
        return EntityRes.GetString(nameof (GeneratorErrorSeverityUnknown));
      }
    }

    internal static string SourceUriUnknown
    {
      get
      {
        return EntityRes.GetString(nameof (SourceUriUnknown));
      }
    }

    internal static string BadPrecisionAndScale(object p0, object p1)
    {
      return EntityRes.GetString(nameof (BadPrecisionAndScale), p0, p1);
    }

    internal static string InvalidNamespaceInUsing(object p0)
    {
      return EntityRes.GetString(nameof (InvalidNamespaceInUsing), p0);
    }

    internal static string BadNavigationPropertyRelationshipNotRelationship(object p0)
    {
      return EntityRes.GetString(nameof (BadNavigationPropertyRelationshipNotRelationship), p0);
    }

    internal static string BadNavigationPropertyRolesCannotBeTheSame
    {
      get
      {
        return EntityRes.GetString(nameof (BadNavigationPropertyRolesCannotBeTheSame));
      }
    }

    internal static string BadNavigationPropertyUndefinedRole(object p0, object p1)
    {
      return EntityRes.GetString(nameof (BadNavigationPropertyUndefinedRole), p0, p1);
    }

    internal static string BadNavigationPropertyBadFromRoleType(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (BadNavigationPropertyBadFromRoleType), p0, p1, p2, p3, p4);
    }

    internal static string InvalidMemberNameMatchesTypeName(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMemberNameMatchesTypeName), p0, p1);
    }

    internal static string InvalidKeyKeyDefinedInBaseClass(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidKeyKeyDefinedInBaseClass), p0, p1);
    }

    internal static string InvalidKeyNullablePart(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidKeyNullablePart), p0, p1);
    }

    internal static string InvalidKeyNoProperty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidKeyNoProperty), p0, p1);
    }

    internal static string KeyMissingOnEntityType(object p0)
    {
      return EntityRes.GetString(nameof (KeyMissingOnEntityType), p0);
    }

    internal static string InvalidDocumentationBothTextAndStructure
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidDocumentationBothTextAndStructure));
      }
    }

    internal static string ArgumentOutOfRangeExpectedPostiveNumber(object p0)
    {
      return EntityRes.GetString(nameof (ArgumentOutOfRangeExpectedPostiveNumber), p0);
    }

    internal static string ArgumentOutOfRange(object p0)
    {
      return EntityRes.GetString(nameof (ArgumentOutOfRange), p0);
    }

    internal static string UnacceptableUri(object p0)
    {
      return EntityRes.GetString(nameof (UnacceptableUri), p0);
    }

    internal static string UnexpectedTypeInCollection(object p0, object p1)
    {
      return EntityRes.GetString(nameof (UnexpectedTypeInCollection), p0, p1);
    }

    internal static string AllElementsMustBeInSchema
    {
      get
      {
        return EntityRes.GetString(nameof (AllElementsMustBeInSchema));
      }
    }

    internal static string AliasNameIsAlreadyDefined(object p0)
    {
      return EntityRes.GetString(nameof (AliasNameIsAlreadyDefined), p0);
    }

    internal static string NeedNotUseSystemNamespaceInUsing(object p0)
    {
      return EntityRes.GetString(nameof (NeedNotUseSystemNamespaceInUsing), p0);
    }

    internal static string CannotUseSystemNamespaceAsAlias(object p0)
    {
      return EntityRes.GetString(nameof (CannotUseSystemNamespaceAsAlias), p0);
    }

    internal static string EntitySetTypeHasNoKeys(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntitySetTypeHasNoKeys), p0, p1);
    }

    internal static string TableAndSchemaAreMutuallyExclusiveWithDefiningQuery(object p0)
    {
      return EntityRes.GetString(nameof (TableAndSchemaAreMutuallyExclusiveWithDefiningQuery), p0);
    }

    internal static string UnexpectedRootElement(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (UnexpectedRootElement), p0, p1, p2);
    }

    internal static string UnexpectedRootElementNoNamespace(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (UnexpectedRootElementNoNamespace), p0, p1, p2);
    }

    internal static string ParameterNameAlreadyDefinedDuplicate(object p0)
    {
      return EntityRes.GetString(nameof (ParameterNameAlreadyDefinedDuplicate), p0);
    }

    internal static string FunctionWithNonPrimitiveTypeNotSupported(object p0, object p1)
    {
      return EntityRes.GetString(nameof (FunctionWithNonPrimitiveTypeNotSupported), p0, p1);
    }

    internal static string FunctionWithNonEdmPrimitiveTypeNotSupported(object p0, object p1)
    {
      return EntityRes.GetString(nameof (FunctionWithNonEdmPrimitiveTypeNotSupported), p0, p1);
    }

    internal static string FunctionImportWithUnsupportedReturnTypeV1(object p0)
    {
      return EntityRes.GetString(nameof (FunctionImportWithUnsupportedReturnTypeV1), p0);
    }

    internal static string FunctionImportWithUnsupportedReturnTypeV1_1(object p0)
    {
      return EntityRes.GetString(nameof (FunctionImportWithUnsupportedReturnTypeV1_1), p0);
    }

    internal static string FunctionImportWithUnsupportedReturnTypeV2(object p0)
    {
      return EntityRes.GetString(nameof (FunctionImportWithUnsupportedReturnTypeV2), p0);
    }

    internal static string FunctionImportUnknownEntitySet(object p0, object p1)
    {
      return EntityRes.GetString(nameof (FunctionImportUnknownEntitySet), p0, p1);
    }

    internal static string FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet(object p0)
    {
      return EntityRes.GetString(nameof (FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet), p0);
    }

    internal static string FunctionImportEntityTypeDoesNotMatchEntitySet(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (FunctionImportEntityTypeDoesNotMatchEntitySet), p0, p1, p2);
    }

    internal static string FunctionImportSpecifiesEntitySetButNotEntityType(object p0)
    {
      return EntityRes.GetString(nameof (FunctionImportSpecifiesEntitySetButNotEntityType), p0);
    }

    internal static string FunctionImportEntitySetAndEntitySetPathDeclared(object p0)
    {
      return EntityRes.GetString(nameof (FunctionImportEntitySetAndEntitySetPathDeclared), p0);
    }

    internal static string FunctionImportComposableAndSideEffectingNotAllowed(object p0)
    {
      return EntityRes.GetString(nameof (FunctionImportComposableAndSideEffectingNotAllowed), p0);
    }

    internal static string FunctionImportCollectionAndRefParametersNotAllowed(object p0)
    {
      return EntityRes.GetString(nameof (FunctionImportCollectionAndRefParametersNotAllowed), p0);
    }

    internal static string FunctionImportNonNullableParametersNotAllowed(object p0)
    {
      return EntityRes.GetString(nameof (FunctionImportNonNullableParametersNotAllowed), p0);
    }

    internal static string TVFReturnTypeRowHasNonScalarProperty
    {
      get
      {
        return EntityRes.GetString(nameof (TVFReturnTypeRowHasNonScalarProperty));
      }
    }

    internal static string DuplicateEntitySetTable(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (DuplicateEntitySetTable), p0, p1, p2);
    }

    internal static string ConcurrencyRedefinedOnSubTypeOfEntitySetType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ConcurrencyRedefinedOnSubTypeOfEntitySetType), p0, p1, p2);
    }

    internal static string SimilarRelationshipEnd(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (SimilarRelationshipEnd), p0, p1, p2, p3, p4);
    }

    internal static string InvalidRelationshipEndMultiplicity(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidRelationshipEndMultiplicity), p0, p1);
    }

    internal static string EndNameAlreadyDefinedDuplicate(object p0)
    {
      return EntityRes.GetString(nameof (EndNameAlreadyDefinedDuplicate), p0);
    }

    internal static string InvalidRelationshipEndType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidRelationshipEndType), p0, p1);
    }

    internal static string BadParameterDirection(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (BadParameterDirection), p0, p1, p2, p3);
    }

    internal static string BadParameterDirectionForComposableFunctions(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (BadParameterDirectionForComposableFunctions), p0, p1, p2, p3);
    }

    internal static string InvalidOperationMultipleEndsInAssociation
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidOperationMultipleEndsInAssociation));
      }
    }

    internal static string InvalidAction(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidAction), p0, p1);
    }

    internal static string DuplicationOperation(object p0)
    {
      return EntityRes.GetString(nameof (DuplicationOperation), p0);
    }

    internal static string NotInNamespaceAlias(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (NotInNamespaceAlias), p0, p1, p2);
    }

    internal static string NotNamespaceQualified(object p0)
    {
      return EntityRes.GetString(nameof (NotNamespaceQualified), p0);
    }

    internal static string NotInNamespaceNoAlias(object p0, object p1)
    {
      return EntityRes.GetString(nameof (NotInNamespaceNoAlias), p0, p1);
    }

    internal static string InvalidValueForParameterTypeSemanticsAttribute(object p0)
    {
      return EntityRes.GetString(nameof (InvalidValueForParameterTypeSemanticsAttribute), p0);
    }

    internal static string DuplicatePropertyNameSpecifiedInEntityKey(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DuplicatePropertyNameSpecifiedInEntityKey), p0, p1);
    }

    internal static string InvalidEntitySetType(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEntitySetType), p0);
    }

    internal static string InvalidRelationshipSetType(object p0)
    {
      return EntityRes.GetString(nameof (InvalidRelationshipSetType), p0);
    }

    internal static string InvalidEntityContainerNameInExtends(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEntityContainerNameInExtends), p0);
    }

    internal static string InvalidNamespaceOrAliasSpecified(object p0)
    {
      return EntityRes.GetString(nameof (InvalidNamespaceOrAliasSpecified), p0);
    }

    internal static string PrecisionOutOfRange(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (PrecisionOutOfRange), p0, p1, p2, p3);
    }

    internal static string ScaleOutOfRange(object p0, object p1, object p2, object p3)
    {
      return EntityRes.GetString(nameof (ScaleOutOfRange), p0, p1, p2, p3);
    }

    internal static string InvalidEntitySetNameReference(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidEntitySetNameReference), p0, p1);
    }

    internal static string InvalidEntityEndName(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidEntityEndName), p0, p1);
    }

    internal static string DuplicateEndName(object p0)
    {
      return EntityRes.GetString(nameof (DuplicateEndName), p0);
    }

    internal static string AmbiguousEntityContainerEnd(object p0, object p1)
    {
      return EntityRes.GetString(nameof (AmbiguousEntityContainerEnd), p0, p1);
    }

    internal static string MissingEntityContainerEnd(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MissingEntityContainerEnd), p0, p1);
    }

    internal static string InvalidEndEntitySetTypeMismatch(object p0)
    {
      return EntityRes.GetString(nameof (InvalidEndEntitySetTypeMismatch), p0);
    }

    internal static string InferRelationshipEndFailedNoEntitySetMatch(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (InferRelationshipEndFailedNoEntitySetMatch), p0, p1, p2, p3, p4);
    }

    internal static string InferRelationshipEndAmbiguous(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (InferRelationshipEndAmbiguous), p0, p1, p2, p3, p4);
    }

    internal static string InferRelationshipEndGivesAlreadyDefinedEnd(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InferRelationshipEndGivesAlreadyDefinedEnd), p0, p1);
    }

    internal static string TooManyAssociationEnds(object p0)
    {
      return EntityRes.GetString(nameof (TooManyAssociationEnds), p0);
    }

    internal static string InvalidEndRoleInRelationshipConstraint(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidEndRoleInRelationshipConstraint), p0, p1);
    }

    internal static string InvalidFromPropertyInRelationshipConstraint(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (InvalidFromPropertyInRelationshipConstraint), p0, p1, p2);
    }

    internal static string InvalidToPropertyInRelationshipConstraint(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (InvalidToPropertyInRelationshipConstraint), p0, p1, p2);
    }

    internal static string InvalidPropertyInRelationshipConstraint(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidPropertyInRelationshipConstraint), p0, p1);
    }

    internal static string TypeMismatchRelationshipConstraint(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (TypeMismatchRelationshipConstraint), p0, p1, p2, p3, p4);
    }

    internal static string InvalidMultiplicityFromRoleUpperBoundMustBeOne(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMultiplicityFromRoleUpperBoundMustBeOne), p0, p1);
    }

    internal static string InvalidMultiplicityFromRoleToPropertyNonNullableV1(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMultiplicityFromRoleToPropertyNonNullableV1), p0, p1);
    }

    internal static string InvalidMultiplicityFromRoleToPropertyNonNullableV2(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMultiplicityFromRoleToPropertyNonNullableV2), p0, p1);
    }

    internal static string InvalidMultiplicityFromRoleToPropertyNullableV1(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMultiplicityFromRoleToPropertyNullableV1), p0, p1);
    }

    internal static string InvalidMultiplicityToRoleLowerBoundMustBeZero(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMultiplicityToRoleLowerBoundMustBeZero), p0, p1);
    }

    internal static string InvalidMultiplicityToRoleUpperBoundMustBeOne(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMultiplicityToRoleUpperBoundMustBeOne), p0, p1);
    }

    internal static string InvalidMultiplicityToRoleUpperBoundMustBeMany(object p0, object p1)
    {
      return EntityRes.GetString(nameof (InvalidMultiplicityToRoleUpperBoundMustBeMany), p0, p1);
    }

    internal static string MismatchNumberOfPropertiesinRelationshipConstraint
    {
      get
      {
        return EntityRes.GetString(nameof (MismatchNumberOfPropertiesinRelationshipConstraint));
      }
    }

    internal static string MissingConstraintOnRelationshipType(object p0)
    {
      return EntityRes.GetString(nameof (MissingConstraintOnRelationshipType), p0);
    }

    internal static string SameRoleReferredInReferentialConstraint(object p0)
    {
      return EntityRes.GetString(nameof (SameRoleReferredInReferentialConstraint), p0);
    }

    internal static string InvalidPrimitiveTypeKind(object p0)
    {
      return EntityRes.GetString(nameof (InvalidPrimitiveTypeKind), p0);
    }

    internal static string EntityKeyMustBeScalar(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EntityKeyMustBeScalar), p0, p1);
    }

    internal static string EntityKeyTypeCurrentlyNotSupportedInSSDL(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (EntityKeyTypeCurrentlyNotSupportedInSSDL), p0, p1, p2, p3, p4);
    }

    internal static string EntityKeyTypeCurrentlyNotSupported(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (EntityKeyTypeCurrentlyNotSupported), p0, p1, p2);
    }

    internal static string MissingFacetDescription(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (MissingFacetDescription), p0, p1, p2);
    }

    internal static string EndWithManyMultiplicityCannotHaveOperationsSpecified(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EndWithManyMultiplicityCannotHaveOperationsSpecified), p0, p1);
    }

    internal static string EndWithoutMultiplicity(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EndWithoutMultiplicity), p0, p1);
    }

    internal static string EntityContainerCannotExtendItself(object p0)
    {
      return EntityRes.GetString(nameof (EntityContainerCannotExtendItself), p0);
    }

    internal static string ComposableFunctionOrFunctionImportMustDeclareReturnType
    {
      get
      {
        return EntityRes.GetString(nameof (ComposableFunctionOrFunctionImportMustDeclareReturnType));
      }
    }

    internal static string NonComposableFunctionCannotBeMappedAsComposable(object p0)
    {
      return EntityRes.GetString(nameof (NonComposableFunctionCannotBeMappedAsComposable), p0);
    }

    internal static string ComposableFunctionImportsReturningEntitiesNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (ComposableFunctionImportsReturningEntitiesNotSupported));
      }
    }

    internal static string StructuralTypeMappingsMustNotBeNullForFunctionImportsReturingNonScalarValues
    {
      get
      {
        return EntityRes.GetString(nameof (StructuralTypeMappingsMustNotBeNullForFunctionImportsReturingNonScalarValues));
      }
    }

    internal static string InvalidReturnTypeForComposableFunction
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidReturnTypeForComposableFunction));
      }
    }

    internal static string NonComposableFunctionMustNotDeclareReturnType
    {
      get
      {
        return EntityRes.GetString(nameof (NonComposableFunctionMustNotDeclareReturnType));
      }
    }

    internal static string CommandTextFunctionsNotComposable
    {
      get
      {
        return EntityRes.GetString(nameof (CommandTextFunctionsNotComposable));
      }
    }

    internal static string CommandTextFunctionsCannotDeclareStoreFunctionName
    {
      get
      {
        return EntityRes.GetString(nameof (CommandTextFunctionsCannotDeclareStoreFunctionName));
      }
    }

    internal static string NonComposableFunctionHasDisallowedAttribute
    {
      get
      {
        return EntityRes.GetString(nameof (NonComposableFunctionHasDisallowedAttribute));
      }
    }

    internal static string EmptyDefiningQuery
    {
      get
      {
        return EntityRes.GetString(nameof (EmptyDefiningQuery));
      }
    }

    internal static string EmptyCommandText
    {
      get
      {
        return EntityRes.GetString(nameof (EmptyCommandText));
      }
    }

    internal static string AmbiguousFunctionOverload(object p0, object p1)
    {
      return EntityRes.GetString(nameof (AmbiguousFunctionOverload), p0, p1);
    }

    internal static string AmbiguousFunctionAndType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (AmbiguousFunctionAndType), p0, p1);
    }

    internal static string CycleInTypeHierarchy(object p0)
    {
      return EntityRes.GetString(nameof (CycleInTypeHierarchy), p0);
    }

    internal static string IncorrectProviderManifest
    {
      get
      {
        return EntityRes.GetString(nameof (IncorrectProviderManifest));
      }
    }

    internal static string ComplexTypeAsReturnTypeAndDefinedEntitySet(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ComplexTypeAsReturnTypeAndDefinedEntitySet), p0, p1);
    }

    internal static string ComplexTypeAsReturnTypeAndNestedComplexProperty(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (ComplexTypeAsReturnTypeAndNestedComplexProperty), p0, p1, p2);
    }

    internal static string FacetsOnNonScalarType(object p0)
    {
      return EntityRes.GetString(nameof (FacetsOnNonScalarType), p0);
    }

    internal static string FacetDeclarationRequiresTypeAttribute
    {
      get
      {
        return EntityRes.GetString(nameof (FacetDeclarationRequiresTypeAttribute));
      }
    }

    internal static string TypeMustBeDeclared
    {
      get
      {
        return EntityRes.GetString(nameof (TypeMustBeDeclared));
      }
    }

    internal static string RowTypeWithoutProperty
    {
      get
      {
        return EntityRes.GetString(nameof (RowTypeWithoutProperty));
      }
    }

    internal static string TypeDeclaredAsAttributeAndElement
    {
      get
      {
        return EntityRes.GetString(nameof (TypeDeclaredAsAttributeAndElement));
      }
    }

    internal static string ReferenceToNonEntityType(object p0)
    {
      return EntityRes.GetString(nameof (ReferenceToNonEntityType), p0);
    }

    internal static string NoCodeGenNamespaceInStructuralAnnotation(object p0)
    {
      return EntityRes.GetString(nameof (NoCodeGenNamespaceInStructuralAnnotation), p0);
    }

    internal static string CannotLoadDifferentVersionOfSchemaInTheSameItemCollection
    {
      get
      {
        return EntityRes.GetString(nameof (CannotLoadDifferentVersionOfSchemaInTheSameItemCollection));
      }
    }

    internal static string InvalidEnumUnderlyingType
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidEnumUnderlyingType));
      }
    }

    internal static string DuplicateEnumMember
    {
      get
      {
        return EntityRes.GetString(nameof (DuplicateEnumMember));
      }
    }

    internal static string CalculatedEnumValueOutOfRange
    {
      get
      {
        return EntityRes.GetString(nameof (CalculatedEnumValueOutOfRange));
      }
    }

    internal static string EnumMemberValueOutOfItsUnderylingTypeRange(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EnumMemberValueOutOfItsUnderylingTypeRange), p0, p1, p2);
    }

    internal static string SpatialWithUseStrongSpatialTypesFalse
    {
      get
      {
        return EntityRes.GetString(nameof (SpatialWithUseStrongSpatialTypesFalse));
      }
    }

    internal static string ObjectQuery_QueryBuilder_InvalidResultType(object p0)
    {
      return EntityRes.GetString(nameof (ObjectQuery_QueryBuilder_InvalidResultType), p0);
    }

    internal static string ObjectQuery_QueryBuilder_InvalidQueryArgument
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectQuery_QueryBuilder_InvalidQueryArgument));
      }
    }

    internal static string ObjectQuery_QueryBuilder_NotSupportedLinqSource
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectQuery_QueryBuilder_NotSupportedLinqSource));
      }
    }

    internal static string ObjectQuery_InvalidConnection
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectQuery_InvalidConnection));
      }
    }

    internal static string ObjectQuery_InvalidQueryName(object p0)
    {
      return EntityRes.GetString(nameof (ObjectQuery_InvalidQueryName), p0);
    }

    internal static string ObjectQuery_UnableToMapResultType
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectQuery_UnableToMapResultType));
      }
    }

    internal static string ObjectQuery_UnableToMaterializeArray(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ObjectQuery_UnableToMaterializeArray), p0, p1);
    }

    internal static string ObjectQuery_UnableToMaterializeArbitaryProjectionType(object p0)
    {
      return EntityRes.GetString(nameof (ObjectQuery_UnableToMaterializeArbitaryProjectionType), p0);
    }

    internal static string ObjectParameter_InvalidParameterName(object p0)
    {
      return EntityRes.GetString(nameof (ObjectParameter_InvalidParameterName), p0);
    }

    internal static string ObjectParameter_InvalidParameterType(object p0)
    {
      return EntityRes.GetString(nameof (ObjectParameter_InvalidParameterType), p0);
    }

    internal static string ObjectParameterCollection_ParameterNameNotFound(object p0)
    {
      return EntityRes.GetString(nameof (ObjectParameterCollection_ParameterNameNotFound), p0);
    }

    internal static string ObjectParameterCollection_ParameterAlreadyExists(object p0)
    {
      return EntityRes.GetString(nameof (ObjectParameterCollection_ParameterAlreadyExists), p0);
    }

    internal static string ObjectParameterCollection_DuplicateParameterName(object p0)
    {
      return EntityRes.GetString(nameof (ObjectParameterCollection_DuplicateParameterName), p0);
    }

    internal static string ObjectParameterCollection_ParametersLocked
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectParameterCollection_ParametersLocked));
      }
    }

    internal static string ProviderReturnedNullForGetDbInformation(object p0)
    {
      return EntityRes.GetString(nameof (ProviderReturnedNullForGetDbInformation), p0);
    }

    internal static string ProviderReturnedNullForCreateCommandDefinition
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderReturnedNullForCreateCommandDefinition));
      }
    }

    internal static string ProviderDidNotReturnAProviderManifest
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderDidNotReturnAProviderManifest));
      }
    }

    internal static string ProviderDidNotReturnAProviderManifestToken
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderDidNotReturnAProviderManifestToken));
      }
    }

    internal static string ProviderDidNotReturnSpatialServices
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderDidNotReturnSpatialServices));
      }
    }

    internal static string SpatialProviderNotUsable
    {
      get
      {
        return EntityRes.GetString(nameof (SpatialProviderNotUsable));
      }
    }

    internal static string ProviderRequiresStoreCommandTree
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderRequiresStoreCommandTree));
      }
    }

    internal static string ProviderShouldOverrideEscapeLikeArgument
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderShouldOverrideEscapeLikeArgument));
      }
    }

    internal static string ProviderEscapeLikeArgumentReturnedNull
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderEscapeLikeArgumentReturnedNull));
      }
    }

    internal static string ProviderDidNotCreateACommandDefinition
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderDidNotCreateACommandDefinition));
      }
    }

    internal static string ProviderDoesNotSupportCreateDatabaseScript
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderDoesNotSupportCreateDatabaseScript));
      }
    }

    internal static string ProviderDoesNotSupportCreateDatabase
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderDoesNotSupportCreateDatabase));
      }
    }

    internal static string ProviderDoesNotSupportDatabaseExists
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderDoesNotSupportDatabaseExists));
      }
    }

    internal static string ProviderDoesNotSupportDeleteDatabase
    {
      get
      {
        return EntityRes.GetString(nameof (ProviderDoesNotSupportDeleteDatabase));
      }
    }

    internal static string Spatial_GeographyValueNotCompatibleWithSpatialServices
    {
      get
      {
        return EntityRes.GetString(nameof (Spatial_GeographyValueNotCompatibleWithSpatialServices));
      }
    }

    internal static string Spatial_GeometryValueNotCompatibleWithSpatialServices
    {
      get
      {
        return EntityRes.GetString(nameof (Spatial_GeometryValueNotCompatibleWithSpatialServices));
      }
    }

    internal static string Spatial_ProviderValueNotCompatibleWithSpatialServices
    {
      get
      {
        return EntityRes.GetString(nameof (Spatial_ProviderValueNotCompatibleWithSpatialServices));
      }
    }

    internal static string Spatial_WellKnownValueSerializationPropertyNotDirectlySettable
    {
      get
      {
        return EntityRes.GetString(nameof (Spatial_WellKnownValueSerializationPropertyNotDirectlySettable));
      }
    }

    internal static string EntityConnectionString_Name
    {
      get
      {
        return EntityRes.GetString(nameof (EntityConnectionString_Name));
      }
    }

    internal static string EntityConnectionString_Provider
    {
      get
      {
        return EntityRes.GetString(nameof (EntityConnectionString_Provider));
      }
    }

    internal static string EntityConnectionString_Metadata
    {
      get
      {
        return EntityRes.GetString(nameof (EntityConnectionString_Metadata));
      }
    }

    internal static string EntityConnectionString_ProviderConnectionString
    {
      get
      {
        return EntityRes.GetString(nameof (EntityConnectionString_ProviderConnectionString));
      }
    }

    internal static string EntityDataCategory_Context
    {
      get
      {
        return EntityRes.GetString(nameof (EntityDataCategory_Context));
      }
    }

    internal static string EntityDataCategory_NamedConnectionString
    {
      get
      {
        return EntityRes.GetString(nameof (EntityDataCategory_NamedConnectionString));
      }
    }

    internal static string EntityDataCategory_Source
    {
      get
      {
        return EntityRes.GetString(nameof (EntityDataCategory_Source));
      }
    }

    internal static string ObjectQuery_Span_IncludeRequiresEntityOrEntityCollection
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectQuery_Span_IncludeRequiresEntityOrEntityCollection));
      }
    }

    internal static string ObjectQuery_Span_NoNavProp(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ObjectQuery_Span_NoNavProp), p0, p1);
    }

    internal static string ObjectQuery_Span_SpanPathSyntaxError
    {
      get
      {
        return EntityRes.GetString(nameof (ObjectQuery_Span_SpanPathSyntaxError));
      }
    }

    internal static string EntityProxyTypeInfo_ProxyHasWrongWrapper
    {
      get
      {
        return EntityRes.GetString(nameof (EntityProxyTypeInfo_ProxyHasWrongWrapper));
      }
    }

    internal static string EntityProxyTypeInfo_CannotSetEntityCollectionProperty(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EntityProxyTypeInfo_CannotSetEntityCollectionProperty), p0, p1);
    }

    internal static string EntityProxyTypeInfo_ProxyMetadataIsUnavailable(object p0)
    {
      return EntityRes.GetString(nameof (EntityProxyTypeInfo_ProxyMetadataIsUnavailable), p0);
    }

    internal static string EntityProxyTypeInfo_DuplicateOSpaceType(object p0)
    {
      return EntityRes.GetString(nameof (EntityProxyTypeInfo_DuplicateOSpaceType), p0);
    }

    internal static string InvalidEdmMemberInstance
    {
      get
      {
        return EntityRes.GetString(nameof (InvalidEdmMemberInstance));
      }
    }

    internal static string EF6Providers_NoProviderFound(object p0)
    {
      return EntityRes.GetString(nameof (EF6Providers_NoProviderFound), p0);
    }

    internal static string EF6Providers_ProviderTypeMissing(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EF6Providers_ProviderTypeMissing), p0, p1);
    }

    internal static string EF6Providers_InstanceMissing(object p0)
    {
      return EntityRes.GetString(nameof (EF6Providers_InstanceMissing), p0);
    }

    internal static string EF6Providers_NotDbProviderServices(object p0)
    {
      return EntityRes.GetString(nameof (EF6Providers_NotDbProviderServices), p0);
    }

    internal static string ProviderInvariantRepeatedInConfig(object p0)
    {
      return EntityRes.GetString(nameof (ProviderInvariantRepeatedInConfig), p0);
    }

    internal static string DbDependencyResolver_NoProviderInvariantName(object p0)
    {
      return EntityRes.GetString(nameof (DbDependencyResolver_NoProviderInvariantName), p0);
    }

    internal static string DbDependencyResolver_InvalidKey(object p0, object p1)
    {
      return EntityRes.GetString(nameof (DbDependencyResolver_InvalidKey), p0, p1);
    }

    internal static string DefaultConfigurationUsedBeforeSet(object p0)
    {
      return EntityRes.GetString(nameof (DefaultConfigurationUsedBeforeSet), p0);
    }

    internal static string AddHandlerToInUseConfiguration
    {
      get
      {
        return EntityRes.GetString(nameof (AddHandlerToInUseConfiguration));
      }
    }

    internal static string ConfigurationSetTwice(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConfigurationSetTwice), p0, p1);
    }

    internal static string ConfigurationNotDiscovered(object p0)
    {
      return EntityRes.GetString(nameof (ConfigurationNotDiscovered), p0);
    }

    internal static string SetConfigurationNotDiscovered(object p0, object p1)
    {
      return EntityRes.GetString(nameof (SetConfigurationNotDiscovered), p0, p1);
    }

    internal static string MultipleConfigsInAssembly(object p0, object p1)
    {
      return EntityRes.GetString(nameof (MultipleConfigsInAssembly), p0, p1);
    }

    internal static string CreateInstance_BadMigrationsConfigurationType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CreateInstance_BadMigrationsConfigurationType), p0, p1);
    }

    internal static string CreateInstance_BadSqlGeneratorType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CreateInstance_BadSqlGeneratorType), p0, p1);
    }

    internal static string CreateInstance_BadDbConfigurationType(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CreateInstance_BadDbConfigurationType), p0, p1);
    }

    internal static string DbConfigurationTypeNotFound(object p0)
    {
      return EntityRes.GetString(nameof (DbConfigurationTypeNotFound), p0);
    }

    internal static string DbConfigurationTypeInAttributeNotFound(object p0)
    {
      return EntityRes.GetString(nameof (DbConfigurationTypeInAttributeNotFound), p0);
    }

    internal static string CreateInstance_NoParameterlessConstructor(object p0)
    {
      return EntityRes.GetString(nameof (CreateInstance_NoParameterlessConstructor), p0);
    }

    internal static string CreateInstance_AbstractType(object p0)
    {
      return EntityRes.GetString(nameof (CreateInstance_AbstractType), p0);
    }

    internal static string CreateInstance_GenericType(object p0)
    {
      return EntityRes.GetString(nameof (CreateInstance_GenericType), p0);
    }

    internal static string ConfigurationLocked(object p0)
    {
      return EntityRes.GetString(nameof (ConfigurationLocked), p0);
    }

    internal static string EnableMigrationsForContext(object p0)
    {
      return EntityRes.GetString(nameof (EnableMigrationsForContext), p0);
    }

    internal static string EnableMigrations_MultipleContexts(object p0)
    {
      return EntityRes.GetString(nameof (EnableMigrations_MultipleContexts), p0);
    }

    internal static string EnableMigrations_MultipleContextsWithName(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EnableMigrations_MultipleContextsWithName), p0, p1);
    }

    internal static string EnableMigrations_NoContext(object p0)
    {
      return EntityRes.GetString(nameof (EnableMigrations_NoContext), p0);
    }

    internal static string EnableMigrations_NoContextWithName(object p0, object p1)
    {
      return EntityRes.GetString(nameof (EnableMigrations_NoContextWithName), p0, p1);
    }

    internal static string MoreThanOneElement
    {
      get
      {
        return EntityRes.GetString(nameof (MoreThanOneElement));
      }
    }

    internal static string IQueryable_Not_Async(object p0)
    {
      return EntityRes.GetString(nameof (IQueryable_Not_Async), p0);
    }

    internal static string IQueryable_Provider_Not_Async
    {
      get
      {
        return EntityRes.GetString(nameof (IQueryable_Provider_Not_Async));
      }
    }

    internal static string EmptySequence
    {
      get
      {
        return EntityRes.GetString(nameof (EmptySequence));
      }
    }

    internal static string UnableToMoveHistoryTableWithAuto
    {
      get
      {
        return EntityRes.GetString(nameof (UnableToMoveHistoryTableWithAuto));
      }
    }

    internal static string NoMatch
    {
      get
      {
        return EntityRes.GetString(nameof (NoMatch));
      }
    }

    internal static string MoreThanOneMatch
    {
      get
      {
        return EntityRes.GetString(nameof (MoreThanOneMatch));
      }
    }

    internal static string CreateConfigurationType_NoParameterlessConstructor(object p0)
    {
      return EntityRes.GetString(nameof (CreateConfigurationType_NoParameterlessConstructor), p0);
    }

    internal static string CollectionEmpty(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CollectionEmpty), p0, p1);
    }

    internal static string DbMigrationsConfiguration_ContextType(object p0)
    {
      return EntityRes.GetString(nameof (DbMigrationsConfiguration_ContextType), p0);
    }

    internal static string ContextFactoryContextType(object p0)
    {
      return EntityRes.GetString(nameof (ContextFactoryContextType), p0);
    }

    internal static string DbMigrationsConfiguration_RootedPath(object p0)
    {
      return EntityRes.GetString(nameof (DbMigrationsConfiguration_RootedPath), p0);
    }

    internal static string ModelBuilder_PropertyFilterTypeMustBePrimitive(object p0)
    {
      return EntityRes.GetString(nameof (ModelBuilder_PropertyFilterTypeMustBePrimitive), p0);
    }

    internal static string LightweightEntityConfiguration_NonScalarProperty(object p0)
    {
      return EntityRes.GetString(nameof (LightweightEntityConfiguration_NonScalarProperty), p0);
    }

    internal static string MigrationsPendingException(object p0)
    {
      return EntityRes.GetString(nameof (MigrationsPendingException), p0);
    }

    internal static string ExecutionStrategy_ExistingTransaction(object p0)
    {
      return EntityRes.GetString(nameof (ExecutionStrategy_ExistingTransaction), p0);
    }

    internal static string ExecutionStrategy_MinimumMustBeLessThanMaximum(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ExecutionStrategy_MinimumMustBeLessThanMaximum), p0, p1);
    }

    internal static string ExecutionStrategy_NegativeDelay(object p0)
    {
      return EntityRes.GetString(nameof (ExecutionStrategy_NegativeDelay), p0);
    }

    internal static string ExecutionStrategy_RetryLimitExceeded(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ExecutionStrategy_RetryLimitExceeded), p0, p1);
    }

    internal static string BaseTypeNotMappedToFunctions(object p0, object p1)
    {
      return EntityRes.GetString(nameof (BaseTypeNotMappedToFunctions), p0, p1);
    }

    internal static string InvalidResourceName(object p0)
    {
      return EntityRes.GetString(nameof (InvalidResourceName), p0);
    }

    internal static string ModificationFunctionParameterNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ModificationFunctionParameterNotFound), p0, p1);
    }

    internal static string EntityClient_CannotOpenBrokenConnection
    {
      get
      {
        return EntityRes.GetString(nameof (EntityClient_CannotOpenBrokenConnection));
      }
    }

    internal static string ModificationFunctionParameterNotFoundOriginal(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ModificationFunctionParameterNotFoundOriginal), p0, p1);
    }

    internal static string ResultBindingNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ResultBindingNotFound), p0, p1);
    }

    internal static string ConflictingFunctionsMapping(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConflictingFunctionsMapping), p0, p1);
    }

    internal static string DbContext_InvalidTransactionForConnection
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_InvalidTransactionForConnection));
      }
    }

    internal static string DbContext_InvalidTransactionNoConnection
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_InvalidTransactionNoConnection));
      }
    }

    internal static string DbContext_TransactionAlreadyStarted
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_TransactionAlreadyStarted));
      }
    }

    internal static string DbContext_TransactionAlreadyEnlistedInUserTransaction
    {
      get
      {
        return EntityRes.GetString(nameof (DbContext_TransactionAlreadyEnlistedInUserTransaction));
      }
    }

    internal static string ExecutionStrategy_StreamingNotSupported(object p0)
    {
      return EntityRes.GetString(nameof (ExecutionStrategy_StreamingNotSupported), p0);
    }

    internal static string EdmProperty_InvalidPropertyType(object p0)
    {
      return EntityRes.GetString(nameof (EdmProperty_InvalidPropertyType), p0);
    }

    internal static string ConcurrentMethodInvocation
    {
      get
      {
        return EntityRes.GetString(nameof (ConcurrentMethodInvocation));
      }
    }

    internal static string AssociationSet_EndEntityTypeMismatch
    {
      get
      {
        return EntityRes.GetString(nameof (AssociationSet_EndEntityTypeMismatch));
      }
    }

    internal static string VisitDbInExpressionNotImplemented
    {
      get
      {
        return EntityRes.GetString(nameof (VisitDbInExpressionNotImplemented));
      }
    }

    internal static string InvalidColumnBuilderArgument(object p0)
    {
      return EntityRes.GetString(nameof (InvalidColumnBuilderArgument), p0);
    }

    internal static string StorageScalarPropertyMapping_OnlyScalarPropertiesAllowed
    {
      get
      {
        return EntityRes.GetString(nameof (StorageScalarPropertyMapping_OnlyScalarPropertiesAllowed));
      }
    }

    internal static string StorageComplexPropertyMapping_OnlyComplexPropertyAllowed
    {
      get
      {
        return EntityRes.GetString(nameof (StorageComplexPropertyMapping_OnlyComplexPropertyAllowed));
      }
    }

    internal static string MetadataItemErrorsFoundDuringGeneration
    {
      get
      {
        return EntityRes.GetString(nameof (MetadataItemErrorsFoundDuringGeneration));
      }
    }

    internal static string AutomaticStaleFunctions(object p0)
    {
      return EntityRes.GetString(nameof (AutomaticStaleFunctions), p0);
    }

    internal static string ScaffoldSprocInDownNotSupported
    {
      get
      {
        return EntityRes.GetString(nameof (ScaffoldSprocInDownNotSupported));
      }
    }

    internal static string LightweightEntityConfiguration_ConfigurationConflict_ComplexType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (LightweightEntityConfiguration_ConfigurationConflict_ComplexType), p0, p1);
    }

    internal static string LightweightEntityConfiguration_ConfigurationConflict_IgnoreType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (LightweightEntityConfiguration_ConfigurationConflict_IgnoreType), p0, p1);
    }

    internal static string AttemptToAddEdmMemberFromWrongDataSpace(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (AttemptToAddEdmMemberFromWrongDataSpace), p0, p1, p2, p3);
    }

    internal static string LightweightEntityConfiguration_InvalidNavigationProperty(object p0)
    {
      return EntityRes.GetString(nameof (LightweightEntityConfiguration_InvalidNavigationProperty), p0);
    }

    internal static string LightweightEntityConfiguration_InvalidInverseNavigationProperty(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (LightweightEntityConfiguration_InvalidInverseNavigationProperty), p0, p1, p2, p3);
    }

    internal static string LightweightEntityConfiguration_MismatchedInverseNavigationProperty(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (LightweightEntityConfiguration_MismatchedInverseNavigationProperty), p0, p1, p2, p3);
    }

    internal static string DuplicateParameterName(object p0)
    {
      return EntityRes.GetString(nameof (DuplicateParameterName), p0);
    }

    internal static string CommandLogFailed(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (CommandLogFailed), p0, p1, p2);
    }

    internal static string CommandLogCanceled(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CommandLogCanceled), p0, p1);
    }

    internal static string CommandLogComplete(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (CommandLogComplete), p0, p1, p2);
    }

    internal static string CommandLogAsync(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CommandLogAsync), p0, p1);
    }

    internal static string CommandLogNonAsync(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CommandLogNonAsync), p0, p1);
    }

    internal static string SuppressionAfterExecution
    {
      get
      {
        return EntityRes.GetString(nameof (SuppressionAfterExecution));
      }
    }

    internal static string BadContextTypeForDiscovery(object p0)
    {
      return EntityRes.GetString(nameof (BadContextTypeForDiscovery), p0);
    }

    internal static string ErrorGeneratingCommandTree(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ErrorGeneratingCommandTree), p0, p1);
    }

    internal static string LightweightNavigationPropertyConfiguration_IncompatibleMultiplicity(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (LightweightNavigationPropertyConfiguration_IncompatibleMultiplicity), p0, p1, p2);
    }

    internal static string LightweightNavigationPropertyConfiguration_InvalidMultiplicity(object p0)
    {
      return EntityRes.GetString(nameof (LightweightNavigationPropertyConfiguration_InvalidMultiplicity), p0);
    }

    internal static string LightweightPrimitivePropertyConfiguration_NonNullableProperty(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (LightweightPrimitivePropertyConfiguration_NonNullableProperty), p0, p1);
    }

    internal static string TestDoubleNotImplemented(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (TestDoubleNotImplemented), p0, p1, p2);
    }

    internal static string TestDoublesCannotBeConverted
    {
      get
      {
        return EntityRes.GetString(nameof (TestDoublesCannotBeConverted));
      }
    }

    internal static string InvalidNavigationPropertyComplexType(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (InvalidNavigationPropertyComplexType), p0, p1, p2);
    }

    internal static string ConventionsConfiguration_InvalidConventionType(object p0)
    {
      return EntityRes.GetString(nameof (ConventionsConfiguration_InvalidConventionType), p0);
    }

    internal static string ConventionsConfiguration_ConventionTypeMissmatch(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConventionsConfiguration_ConventionTypeMissmatch), p0, p1);
    }

    internal static string LightweightPrimitivePropertyConfiguration_DateTimeScale(object p0)
    {
      return EntityRes.GetString(nameof (LightweightPrimitivePropertyConfiguration_DateTimeScale), p0);
    }

    internal static string LightweightPrimitivePropertyConfiguration_DecimalNoScale(object p0)
    {
      return EntityRes.GetString(nameof (LightweightPrimitivePropertyConfiguration_DecimalNoScale), p0);
    }

    internal static string LightweightPrimitivePropertyConfiguration_HasPrecisionNonDateTime(
      object p0)
    {
      return EntityRes.GetString(nameof (LightweightPrimitivePropertyConfiguration_HasPrecisionNonDateTime), p0);
    }

    internal static string LightweightPrimitivePropertyConfiguration_HasPrecisionNonDecimal(
      object p0)
    {
      return EntityRes.GetString(nameof (LightweightPrimitivePropertyConfiguration_HasPrecisionNonDecimal), p0);
    }

    internal static string LightweightPrimitivePropertyConfiguration_IsRowVersionNonBinary(object p0)
    {
      return EntityRes.GetString(nameof (LightweightPrimitivePropertyConfiguration_IsRowVersionNonBinary), p0);
    }

    internal static string LightweightPrimitivePropertyConfiguration_IsUnicodeNonString(object p0)
    {
      return EntityRes.GetString(nameof (LightweightPrimitivePropertyConfiguration_IsUnicodeNonString), p0);
    }

    internal static string LightweightPrimitivePropertyConfiguration_NonLength(object p0)
    {
      return EntityRes.GetString(nameof (LightweightPrimitivePropertyConfiguration_NonLength), p0);
    }

    internal static string UnableToUpgradeHistoryWhenCustomFactory
    {
      get
      {
        return EntityRes.GetString(nameof (UnableToUpgradeHistoryWhenCustomFactory));
      }
    }

    internal static string CommitFailed
    {
      get
      {
        return EntityRes.GetString(nameof (CommitFailed));
      }
    }

    internal static string InterceptorTypeNotFound(object p0)
    {
      return EntityRes.GetString(nameof (InterceptorTypeNotFound), p0);
    }

    internal static string InterceptorTypeNotInterceptor(object p0)
    {
      return EntityRes.GetString(nameof (InterceptorTypeNotInterceptor), p0);
    }

    internal static string ViewGenContainersNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ViewGenContainersNotFound), p0, p1);
    }

    internal static string HashCalcContainersNotFound(object p0, object p1)
    {
      return EntityRes.GetString(nameof (HashCalcContainersNotFound), p0, p1);
    }

    internal static string ViewGenMultipleContainers
    {
      get
      {
        return EntityRes.GetString(nameof (ViewGenMultipleContainers));
      }
    }

    internal static string HashCalcMultipleContainers
    {
      get
      {
        return EntityRes.GetString(nameof (HashCalcMultipleContainers));
      }
    }

    internal static string BadConnectionWrapping
    {
      get
      {
        return EntityRes.GetString(nameof (BadConnectionWrapping));
      }
    }

    internal static string ConnectionClosedLog(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConnectionClosedLog), p0, p1);
    }

    internal static string ConnectionCloseErrorLog(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ConnectionCloseErrorLog), p0, p1, p2);
    }

    internal static string ConnectionOpenedLog(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConnectionOpenedLog), p0, p1);
    }

    internal static string ConnectionOpenErrorLog(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ConnectionOpenErrorLog), p0, p1, p2);
    }

    internal static string ConnectionOpenedLogAsync(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConnectionOpenedLogAsync), p0, p1);
    }

    internal static string ConnectionOpenErrorLogAsync(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (ConnectionOpenErrorLogAsync), p0, p1, p2);
    }

    internal static string TransactionStartedLog(object p0, object p1)
    {
      return EntityRes.GetString(nameof (TransactionStartedLog), p0, p1);
    }

    internal static string TransactionStartErrorLog(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (TransactionStartErrorLog), p0, p1, p2);
    }

    internal static string TransactionCommittedLog(object p0, object p1)
    {
      return EntityRes.GetString(nameof (TransactionCommittedLog), p0, p1);
    }

    internal static string TransactionCommitErrorLog(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (TransactionCommitErrorLog), p0, p1, p2);
    }

    internal static string TransactionRolledBackLog(object p0, object p1)
    {
      return EntityRes.GetString(nameof (TransactionRolledBackLog), p0, p1);
    }

    internal static string TransactionRollbackErrorLog(object p0, object p1, object p2)
    {
      return EntityRes.GetString(nameof (TransactionRollbackErrorLog), p0, p1, p2);
    }

    internal static string ConnectionOpenCanceledLog(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConnectionOpenCanceledLog), p0, p1);
    }

    internal static string TransactionHandler_AlreadyInitialized
    {
      get
      {
        return EntityRes.GetString(nameof (TransactionHandler_AlreadyInitialized));
      }
    }

    internal static string ConnectionDisposedLog(object p0, object p1)
    {
      return EntityRes.GetString(nameof (ConnectionDisposedLog), p0, p1);
    }

    internal static string TransactionDisposedLog(object p0, object p1)
    {
      return EntityRes.GetString(nameof (TransactionDisposedLog), p0, p1);
    }

    internal static string UnableToLoadEmbeddedResource(object p0, object p1)
    {
      return EntityRes.GetString(nameof (UnableToLoadEmbeddedResource), p0, p1);
    }

    internal static string CannotSetBaseTypeCyclicInheritance(object p0, object p1)
    {
      return EntityRes.GetString(nameof (CannotSetBaseTypeCyclicInheritance), p0, p1);
    }

    internal static string CannotDefineKeysOnBothBaseAndDerivedTypes
    {
      get
      {
        return EntityRes.GetString(nameof (CannotDefineKeysOnBothBaseAndDerivedTypes));
      }
    }
  }
}
