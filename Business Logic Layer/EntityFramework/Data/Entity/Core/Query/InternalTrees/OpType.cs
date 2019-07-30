// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.OpType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal enum OpType
  {
    Constant = 0,
    InternalConstant = 1,
    NullSentinel = 2,
    Null = 3,
    ConstantPredicate = 4,
    VarRef = 5,
    GT = 6,
    GE = 7,
    LE = 8,
    LT = 9,
    EQ = 10, // 0x0000000A
    NE = 11, // 0x0000000B
    Like = 12, // 0x0000000C
    Plus = 13, // 0x0000000D
    Minus = 14, // 0x0000000E
    Multiply = 15, // 0x0000000F
    Divide = 16, // 0x00000010
    Modulo = 17, // 0x00000011
    UnaryMinus = 18, // 0x00000012
    And = 19, // 0x00000013
    Or = 20, // 0x00000014
    In = 21, // 0x00000015
    Not = 22, // 0x00000016
    IsNull = 23, // 0x00000017
    Case = 24, // 0x00000018
    Treat = 25, // 0x00000019
    IsOf = 26, // 0x0000001A
    Cast = 27, // 0x0000001B
    SoftCast = 28, // 0x0000001C
    Aggregate = 29, // 0x0000001D
    Function = 30, // 0x0000001E
    RelProperty = 31, // 0x0000001F
    Property = 32, // 0x00000020
    NewEntity = 33, // 0x00000021
    NewInstance = 34, // 0x00000022
    DiscriminatedNewEntity = 35, // 0x00000023
    NewMultiset = 36, // 0x00000024
    NewRecord = 37, // 0x00000025
    GetRefKey = 38, // 0x00000026
    GetEntityRef = 39, // 0x00000027
    Ref = 40, // 0x00000028
    Exists = 41, // 0x00000029
    Element = 42, // 0x0000002A
    Collect = 43, // 0x0000002B
    Deref = 44, // 0x0000002C
    Navigate = 45, // 0x0000002D
    ScanTable = 46, // 0x0000002E
    ScanView = 47, // 0x0000002F
    Filter = 48, // 0x00000030
    Project = 49, // 0x00000031
    InnerJoin = 50, // 0x00000032
    LeftOuterJoin = 51, // 0x00000033
    FullOuterJoin = 52, // 0x00000034
    CrossJoin = 53, // 0x00000035
    CrossApply = 54, // 0x00000036
    OuterApply = 55, // 0x00000037
    Unnest = 56, // 0x00000038
    Sort = 57, // 0x00000039
    ConstrainedSort = 58, // 0x0000003A
    GroupBy = 59, // 0x0000003B
    GroupByInto = 60, // 0x0000003C
    UnionAll = 61, // 0x0000003D
    Intersect = 62, // 0x0000003E
    Except = 63, // 0x0000003F
    Distinct = 64, // 0x00000040
    SingleRow = 65, // 0x00000041
    SingleRowTable = 66, // 0x00000042
    VarDef = 67, // 0x00000043
    VarDefList = 68, // 0x00000044
    Leaf = 69, // 0x00000045
    PhysicalProject = 70, // 0x00000046
    SingleStreamNest = 71, // 0x00000047
    MultiStreamNest = 72, // 0x00000048
    MaxMarker = 73, // 0x00000049
    NotValid = 73, // 0x00000049
  }
}
