// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.MappingErrorCode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping
{
  internal enum MappingErrorCode
  {
    Value = 2000, // 0x000007D0
    InvalidContent = 2001, // 0x000007D1
    InvalidEntityContainer = 2002, // 0x000007D2
    InvalidEntitySet = 2003, // 0x000007D3
    InvalidEntityType = 2004, // 0x000007D4
    InvalidAssociationSet = 2005, // 0x000007D5
    InvalidAssociationType = 2006, // 0x000007D6
    InvalidTable = 2007, // 0x000007D7
    InvalidComplexType = 2008, // 0x000007D8
    InvalidEdmMember = 2009, // 0x000007D9
    InvalidStorageMember = 2010, // 0x000007DA
    TableMappingFragmentExpected = 2011, // 0x000007DB
    SetMappingExpected = 2012, // 0x000007DC
    DuplicateSetMapping = 2014, // 0x000007DE
    DuplicateTypeMapping = 2015, // 0x000007DF
    ConditionError = 2016, // 0x000007E0
    RootMappingElementMissing = 2018, // 0x000007E2
    IncompatibleMemberMapping = 2019, // 0x000007E3
    InvalidEnumValue = 2023, // 0x000007E7
    XmlSchemaParsingError = 2024, // 0x000007E8
    XmlSchemaValidationError = 2025, // 0x000007E9
    AmbiguousModificationFunctionMappingForAssociationSet = 2026, // 0x000007EA
    MissingSetClosureInModificationFunctionMapping = 2027, // 0x000007EB
    MissingModificationFunctionMappingForEntityType = 2028, // 0x000007EC
    InvalidTableNameAttributeWithModificationFunctionMapping = 2029, // 0x000007ED
    InvalidModificationFunctionMappingForMultipleTypes = 2030, // 0x000007EE
    AmbiguousResultBindingInModificationFunctionMapping = 2031, // 0x000007EF
    InvalidAssociationSetRoleInModificationFunctionMapping = 2032, // 0x000007F0
    InvalidAssociationSetCardinalityInModificationFunctionMapping = 2033, // 0x000007F1
    RedundantEntityTypeMappingInModificationFunctionMapping = 2034, // 0x000007F2
    MissingVersionInModificationFunctionMapping = 2035, // 0x000007F3
    InvalidVersionInModificationFunctionMapping = 2036, // 0x000007F4
    InvalidParameterInModificationFunctionMapping = 2037, // 0x000007F5
    ParameterBoundTwiceInModificationFunctionMapping = 2038, // 0x000007F6
    CSpaceMemberMappedToMultipleSSpaceMemberWithDifferentTypes = 2039, // 0x000007F7
    NoEquivalentStorePrimitiveTypeFound = 2040, // 0x000007F8
    NoEquivalentStorePrimitiveTypeWithFacetsFound = 2041, // 0x000007F9
    InvalidModificationFunctionMappingPropertyParameterTypeMismatch = 2042, // 0x000007FA
    InvalidModificationFunctionMappingMultipleEndsOfAssociationMapped = 2043, // 0x000007FB
    InvalidModificationFunctionMappingUnknownFunction = 2044, // 0x000007FC
    InvalidModificationFunctionMappingAmbiguousFunction = 2045, // 0x000007FD
    InvalidModificationFunctionMappingNotValidFunction = 2046, // 0x000007FE
    InvalidModificationFunctionMappingNotValidFunctionParameter = 2047, // 0x000007FF
    InvalidModificationFunctionMappingAssociationSetNotMappedForOperation = 2048, // 0x00000800
    InvalidModificationFunctionMappingAssociationEndMappingInvalidForEntityType = 2049, // 0x00000801
    MappingFunctionImportStoreFunctionDoesNotExist = 2050, // 0x00000802
    MappingFunctionImportStoreFunctionAmbiguous = 2051, // 0x00000803
    MappingFunctionImportFunctionImportDoesNotExist = 2052, // 0x00000804
    MappingFunctionImportFunctionImportMappedMultipleTimes = 2053, // 0x00000805
    MappingFunctionImportTargetFunctionMustBeNonComposable = 2054, // 0x00000806
    MappingFunctionImportTargetParameterHasNoCorrespondingImportParameter = 2055, // 0x00000807
    MappingFunctionImportImportParameterHasNoCorrespondingTargetParameter = 2056, // 0x00000808
    MappingFunctionImportIncompatibleParameterMode = 2057, // 0x00000809
    MappingFunctionImportIncompatibleParameterType = 2058, // 0x0000080A
    MappingFunctionImportRowsAffectedParameterDoesNotExist = 2059, // 0x0000080B
    MappingFunctionImportRowsAffectedParameterHasWrongType = 2060, // 0x0000080C
    MappingFunctionImportRowsAffectedParameterHasWrongMode = 2061, // 0x0000080D
    EmptyContainerMapping = 2062, // 0x0000080E
    EmptySetMapping = 2063, // 0x0000080F
    TableNameAttributeWithQueryView = 2064, // 0x00000810
    EmptyQueryView = 2065, // 0x00000811
    PropertyMapsWithQueryView = 2066, // 0x00000812
    MissingSetClosureInQueryViews = 2067, // 0x00000813
    InvalidQueryView = 2068, // 0x00000814
    InvalidQueryViewResultType = 2069, // 0x00000815
    ItemWithSameNameExistsBothInCSpaceAndSSpace = 2070, // 0x00000816
    MappingUnsupportedExpressionKindQueryView = 2071, // 0x00000817
    MappingUnsupportedScanTargetQueryView = 2072, // 0x00000818
    MappingUnsupportedPropertyKindQueryView = 2073, // 0x00000819
    MappingUnsupportedInitializationQueryView = 2074, // 0x0000081A
    MappingFunctionImportEntityTypeMappingForFunctionNotReturningEntitySet = 2075, // 0x0000081B
    MappingFunctionImportAmbiguousTypeConditions = 2076, // 0x0000081C
    MappingOfAbstractType = 2078, // 0x0000081E
    StorageEntityContainerNameMismatchWhileSpecifyingPartialMapping = 2079, // 0x0000081F
    TypeNameForFirstQueryView = 2080, // 0x00000820
    NoTypeNameForTypeSpecificQueryView = 2081, // 0x00000821
    QueryViewExistsForEntitySetAndType = 2082, // 0x00000822
    TypeNameContainsMultipleTypesForQueryView = 2083, // 0x00000823
    IsTypeOfQueryViewForBaseType = 2084, // 0x00000824
    InvalidTypeInScalarProperty = 2085, // 0x00000825
    AlreadyMappedStorageEntityContainer = 2086, // 0x00000826
    UnsupportedQueryViewInEntityContainerMapping = 2087, // 0x00000827
    MappingAllQueryViewAtCompileTime = 2088, // 0x00000828
    MappingNoViewsCanBeGenerated = 2089, // 0x00000829
    MappingStoreProviderReturnsNullEdmType = 2090, // 0x0000082A
    DuplicateMemberMapping = 2092, // 0x0000082C
    MappingFunctionImportUnexpectedEntityTypeMapping = 2093, // 0x0000082D
    MappingFunctionImportUnexpectedComplexTypeMapping = 2094, // 0x0000082E
    DistinctFragmentInReadWriteContainer = 2096, // 0x00000830
    EntitySetMismatchOnAssociationSetEnd = 2097, // 0x00000831
    InvalidModificationFunctionMappingAssociationEndForeignKey = 2098, // 0x00000832
    CannotLoadDifferentVersionOfSchemaInTheSameItemCollection = 2100, // 0x00000834
    MappingDifferentMappingEdmStoreVersion = 2101, // 0x00000835
    MappingDifferentEdmStoreVersion = 2102, // 0x00000836
    UnmappedFunctionImport = 2103, // 0x00000837
    MappingFunctionImportReturnTypePropertyNotMapped = 2104, // 0x00000838
    InvalidType = 2106, // 0x0000083A
    MappingFunctionImportTVFExpected = 2108, // 0x0000083C
    MappingFunctionImportScalarMappingTypeMismatch = 2109, // 0x0000083D
    MappingFunctionImportScalarMappingToMulticolumnTVF = 2110, // 0x0000083E
    MappingFunctionImportTargetFunctionMustBeComposable = 2111, // 0x0000083F
    UnsupportedFunctionCallInQueryView = 2112, // 0x00000840
    FunctionResultMappingCountMismatch = 2113, // 0x00000841
    MappingFunctionImportCannotInferTargetFunctionKeys = 2114, // 0x00000842
  }
}
