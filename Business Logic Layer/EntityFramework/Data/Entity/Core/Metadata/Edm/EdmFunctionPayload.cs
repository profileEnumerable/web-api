// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmFunctionPayload
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Contains additional attributes and properties of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />
  /// </summary>
  /// <remarks>
  /// Note that <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunctionPayload" /> objects are short lived and exist only to
  /// make <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> initialization easier. Instance of this type are not
  /// compared to each other and arrays returned by array properties are copied to internal
  /// collections in the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> ctor. Therefore it is fine to suppress the
  /// Code Analysis messages.
  /// </remarks>
  [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
  public class EdmFunctionPayload
  {
    /// <summary>Gets or sets the function schema.</summary>
    /// <returns>The function schema.</returns>
    public string Schema { get; set; }

    /// <summary>Gets or sets the store function name.</summary>
    /// <returns>The store function name.</returns>
    public string StoreFunctionName { get; set; }

    /// <summary>Gets or sets the command text associated with the function.</summary>
    /// <returns>The command text associated with the function.</returns>
    public string CommandText { get; set; }

    /// <summary>Gets or sets the entity sets for the function.</summary>
    /// <returns>The entity sets for the function.</returns>
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public IList<EntitySet> EntitySets { get; set; }

    /// <summary>Gets a value that indicates whether this is an aggregate function.</summary>
    /// <returns>true if this is an aggregate function; otherwise, false.</returns>
    public bool? IsAggregate { get; set; }

    /// <summary>Gets or sets whether this function is a built-in function.</summary>
    /// <returns>true if this function is a built-in function; otherwise, false.</returns>
    public bool? IsBuiltIn { get; set; }

    /// <summary>Gets or sets whether the function contains no arguments.</summary>
    /// <returns>true if the function contains no arguments; otherwise, false.</returns>
    public bool? IsNiladic { get; set; }

    /// <summary>Gets or sets whether this function can be composed.</summary>
    /// <returns>true if this function can be composed; otherwise, false.</returns>
    public bool? IsComposable { get; set; }

    /// <summary>Gets or sets whether this function is from a provider manifest.</summary>
    /// <returns>true if this function is from a provider manifest; otherwise, false.</returns>
    public bool? IsFromProviderManifest { get; set; }

    /// <summary>Gets or sets whether this function is a cached store function.</summary>
    /// <returns>true if this function is a cached store function; otherwise, false.</returns>
    public bool? IsCachedStoreFunction { get; set; }

    /// <summary>Gets or sets whether this function is a function import.</summary>
    /// <returns>true if this function is a function import; otherwise, false.</returns>
    public bool? IsFunctionImport { get; set; }

    /// <summary>Gets or sets the return parameters.</summary>
    /// <returns>The return parameters.</returns>
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public IList<FunctionParameter> ReturnParameters { get; set; }

    /// <summary>Gets or sets the parameter type semantics.</summary>
    /// <returns>The parameter type semantics.</returns>
    public System.Data.Entity.Core.Metadata.Edm.ParameterTypeSemantics? ParameterTypeSemantics { get; set; }

    /// <summary>Gets or sets the function parameters.</summary>
    /// <returns>The function parameters.</returns>
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public IList<FunctionParameter> Parameters { get; set; }
  }
}
