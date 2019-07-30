// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ParameterTypeSemantics
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// The enumeration defining the type semantics used to resolve function overloads.
  /// These flags are defined in the provider manifest per function definition.
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames")]
  public enum ParameterTypeSemantics
  {
    /// <summary>
    /// Allow Implicit Conversion between given and formal argument types (default).
    /// </summary>
    AllowImplicitConversion,
    /// <summary>
    /// Allow Type Promotion between given and formal argument types.
    /// </summary>
    AllowImplicitPromotion,
    /// <summary>Use strict Equivalence only.</summary>
    ExactMatchOnly,
  }
}
