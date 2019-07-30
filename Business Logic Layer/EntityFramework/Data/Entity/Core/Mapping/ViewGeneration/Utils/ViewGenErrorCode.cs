// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Utils.ViewGenErrorCode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Utils
{
  internal enum ViewGenErrorCode
  {
    Value = 3000, // 0x00000BB8
    InvalidCondition = 3001, // 0x00000BB9
    KeyConstraintViolation = 3002, // 0x00000BBA
    KeyConstraintUpdateViolation = 3003, // 0x00000BBB
    AttributesUnrecoverable = 3004, // 0x00000BBC
    AmbiguousMultiConstants = 3005, // 0x00000BBD
    NonKeyProjectedWithOverlappingPartitions = 3007, // 0x00000BBF
    ConcurrencyDerivedClass = 3008, // 0x00000BC0
    ConcurrencyTokenHasCondition = 3009, // 0x00000BC1
    DomainConstraintViolation = 3012, // 0x00000BC4
    ForeignKeyMissingTableMapping = 3013, // 0x00000BC5
    ForeignKeyNotGuaranteedInCSpace = 3014, // 0x00000BC6
    ForeignKeyMissingRelationshipMapping = 3015, // 0x00000BC7
    ForeignKeyUpperBoundMustBeOne = 3016, // 0x00000BC8
    ForeignKeyLowerBoundMustBeOne = 3017, // 0x00000BC9
    ForeignKeyParentTableNotMappedToEnd = 3018, // 0x00000BCA
    ForeignKeyColumnOrderIncorrect = 3019, // 0x00000BCB
    DisjointConstraintViolation = 3020, // 0x00000BCC
    DuplicateCPropertiesMapped = 3021, // 0x00000BCD
    NotNullNoProjectedSlot = 3022, // 0x00000BCE
    NoDefaultValue = 3023, // 0x00000BCF
    KeyNotMappedForCSideExtent = 3024, // 0x00000BD0
    KeyNotMappedForTable = 3025, // 0x00000BD1
    PartitionConstraintViolation = 3026, // 0x00000BD2
    MissingExtentMapping = 3027, // 0x00000BD3
    ImpopssibleCondition = 3030, // 0x00000BD6
    NullableMappingForNonNullableColumn = 3031, // 0x00000BD7
    ErrorPatternConditionError = 3032, // 0x00000BD8
    ErrorPatternSplittingError = 3033, // 0x00000BD9
    ErrorPatternInvalidPartitionError = 3034, // 0x00000BDA
    ErrorPatternMissingMappingError = 3035, // 0x00000BDB
    NoJoinKeyOrFKProvidedInMapping = 3036, // 0x00000BDC
    MultipleFragmentsBetweenCandSExtentWithDistinct = 3037, // 0x00000BDD
  }
}
