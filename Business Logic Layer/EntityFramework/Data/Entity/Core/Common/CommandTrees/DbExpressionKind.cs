// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>
  /// Contains values that each expression class uses to denote the operation it represents. The
  /// <see cref="P:System.Data.Entity.Core.Common.CommandTrees.DbExpression.ExpressionKind" />
  /// property of an
  /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
  /// can be retrieved to determine which operation that expression represents.
  /// </summary>
  public enum DbExpressionKind
  {
    /// <summary>True for all.</summary>
    All,
    /// <summary>Logical And.</summary>
    And,
    /// <summary>True for any.</summary>
    Any,
    /// <summary>Conditional case statement.</summary>
    Case,
    /// <summary>Polymorphic type cast.</summary>
    Cast,
    /// <summary>A constant value.</summary>
    Constant,
    /// <summary>Cross apply</summary>
    CrossApply,
    /// <summary>Cross join</summary>
    CrossJoin,
    /// <summary>Dereference.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deref")] Deref,
    /// <summary>Duplicate removal.</summary>
    Distinct,
    /// <summary>Division.</summary>
    Divide,
    /// <summary>Set to singleton conversion.</summary>
    Element,
    /// <summary>Entity ref value retrieval.</summary>
    EntityRef,
    /// <summary>Equality</summary>
    Equals,
    /// <summary>Set subtraction</summary>
    Except,
    /// <summary>Restriction.</summary>
    Filter,
    /// <summary>Full outer join</summary>
    FullOuterJoin,
    /// <summary>Invocation of a stand-alone function</summary>
    Function,
    /// <summary>Greater than.</summary>
    GreaterThan,
    /// <summary>Greater than or equal.</summary>
    GreaterThanOrEquals,
    /// <summary>Grouping.</summary>
    GroupBy,
    /// <summary>Inner join</summary>
    InnerJoin,
    /// <summary>Set intersection.</summary>
    Intersect,
    /// <summary>Empty set determination.</summary>
    IsEmpty,
    /// <summary>Null determination.</summary>
    IsNull,
    /// <summary>Type comparison (specified Type or Subtype).</summary>
    IsOf,
    /// <summary>Type comparison (specified Type only).</summary>
    IsOfOnly,
    /// <summary>Left outer join</summary>
    LeftOuterJoin,
    /// <summary>Less than.</summary>
    LessThan,
    /// <summary>Less than or equal.</summary>
    LessThanOrEquals,
    /// <summary>String comparison.</summary>
    Like,
    /// <summary>Result count restriction (TOP n).</summary>
    Limit,
    /// <summary>Subtraction.</summary>
    Minus,
    /// <summary>Modulo.</summary>
    Modulo,
    /// <summary>Multiplication.</summary>
    Multiply,
    /// <summary>Instance, row, and set construction.</summary>
    NewInstance,
    /// <summary>Logical Not.</summary>
    Not,
    /// <summary>Inequality.</summary>
    NotEquals,
    /// <summary>Null.</summary>
    Null,
    /// <summary>Set members by type (or subtype).</summary>
    OfType,
    /// <summary>Set members by (exact) type.</summary>
    OfTypeOnly,
    /// <summary>Logical Or.</summary>
    Or,
    /// <summary>Outer apply.</summary>
    OuterApply,
    /// <summary>A reference to a parameter.</summary>
    ParameterReference,
    /// <summary>Addition.</summary>
    Plus,
    /// <summary>Projection.</summary>
    Project,
    /// <summary>Retrieval of a static or instance property.</summary>
    Property,
    /// <summary>Reference.</summary>
    Ref,
    /// <summary>Ref key value retrieval.</summary>
    RefKey,
    /// <summary>
    /// Navigation of a (composition or association) relationship.
    /// </summary>
    RelationshipNavigation,
    /// <summary>Entity or relationship set scan.</summary>
    Scan,
    /// <summary>Skip elements of an ordered collection.</summary>
    Skip,
    /// <summary>Sorting.</summary>
    Sort,
    /// <summary>Type conversion.</summary>
    Treat,
    /// <summary>Negation.</summary>
    UnaryMinus,
    /// <summary>Set union (with duplicates).</summary>
    UnionAll,
    /// <summary>A reference to a variable.</summary>
    VariableReference,
    /// <summary>Application of a lambda function</summary>
    Lambda,
    /// <summary>In.</summary>
    In,
  }
}
