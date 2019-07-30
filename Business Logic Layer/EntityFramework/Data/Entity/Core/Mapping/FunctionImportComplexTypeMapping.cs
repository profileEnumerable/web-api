// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportComplexTypeMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents a complex type mapping for a function import result.
  /// </summary>
  public sealed class FunctionImportComplexTypeMapping : FunctionImportStructuralTypeMapping
  {
    private readonly ComplexType _returnType;

    /// <summary>
    /// Initializes a new FunctionImportComplexTypeMapping instance.
    /// </summary>
    /// <param name="returnType">The return type.</param>
    /// <param name="properties">The property mappings for the result type of a function import.</param>
    public FunctionImportComplexTypeMapping(
      ComplexType returnType,
      Collection<FunctionImportReturnTypePropertyMapping> properties)
      : this(Check.NotNull<ComplexType>(returnType, nameof (returnType)), Check.NotNull<Collection<FunctionImportReturnTypePropertyMapping>>(properties, nameof (properties)), LineInfo.Empty)
    {
    }

    internal FunctionImportComplexTypeMapping(
      ComplexType returnType,
      Collection<FunctionImportReturnTypePropertyMapping> properties,
      LineInfo lineInfo)
      : base(properties, lineInfo)
    {
      this._returnType = returnType;
    }

    /// <summary>Ges the return type.</summary>
    public ComplexType ReturnType
    {
      get
      {
        return this._returnType;
      }
    }
  }
}
