// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.FunctionParameter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Class representing a function parameter</summary>
  public sealed class FunctionParameter : MetadataItem, INamedDataModelItem
  {
    internal static Func<FunctionParameter, SafeLink<EdmFunction>> DeclaringFunctionLinker = (Func<FunctionParameter, SafeLink<EdmFunction>>) (fp => fp._declaringFunction);
    private readonly SafeLink<EdmFunction> _declaringFunction = new SafeLink<EdmFunction>();
    private readonly TypeUsage _typeUsage;
    private string _name;

    internal FunctionParameter()
    {
    }

    internal FunctionParameter(string name, TypeUsage typeUsage, ParameterMode parameterMode)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<TypeUsage>(typeUsage, nameof (typeUsage));
      this._name = name;
      this._typeUsage = typeUsage;
      this.SetParameterMode(parameterMode);
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.FunctionParameter" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.FunctionParameter" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.FunctionParameter;
      }
    }

    /// <summary>
    /// Gets the mode of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.FunctionParameter" />.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.ParameterMode" /> values.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the FunctionParameter instance is in ReadOnly state</exception>
    [MetadataProperty(BuiltInTypeKind.ParameterMode, false)]
    public ParameterMode Mode
    {
      get
      {
        return this.GetParameterMode();
      }
    }

    string INamedDataModelItem.Identity
    {
      get
      {
        return this.Identity;
      }
    }

    internal override string Identity
    {
      get
      {
        return this._name;
      }
    }

    /// <summary>
    /// Gets the name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.FunctionParameter" />.
    /// </summary>
    /// <returns>
    /// The name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.FunctionParameter" />.
    /// </returns>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        this.SetName(value);
      }
    }

    private void SetName(string name)
    {
      this._name = name;
      if (this.DeclaringFunction == null)
        return;
      (this.Mode == ParameterMode.ReturnValue ? this.DeclaringFunction.ReturnParameters.Source : this.DeclaringFunction.Parameters.Source).InvalidateCache();
    }

    /// <summary>
    /// Gets the instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> class that contains both the type of the parameter and facets for the type.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object that contains both the type of the parameter and facets for the type.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.TypeUsage, false)]
    public TypeUsage TypeUsage
    {
      get
      {
        return this._typeUsage;
      }
    }

    /// <summary>Gets the type name of this parameter.</summary>
    /// <returns>The type name of this parameter.</returns>
    public string TypeName
    {
      get
      {
        return this.TypeUsage.EdmType.Name;
      }
    }

    /// <summary>Gets whether the max length facet is constant for the database provider.</summary>
    /// <returns>true if the facet is constant; otherwise, false.</returns>
    public bool IsMaxLengthConstant
    {
      get
      {
        Facet facet;
        if (this.TypeUsage.Facets.TryGetValue("MaxLength", false, out facet))
          return facet.Description.IsConstant;
        return false;
      }
    }

    /// <summary>Gets the maximum length of the parameter.</summary>
    /// <returns>The maximum length of the parameter.</returns>
    public int? MaxLength
    {
      get
      {
        Facet facet;
        if (!this.TypeUsage.Facets.TryGetValue(nameof (MaxLength), false, out facet))
          return new int?();
        return facet.Value as int?;
      }
    }

    /// <summary>Gets whether the parameter uses the maximum length supported by the database provider.</summary>
    /// <returns>true if parameter uses the maximum length supported by the database provider; otherwise, false.</returns>
    public bool IsMaxLength
    {
      get
      {
        Facet facet;
        if (this.TypeUsage.Facets.TryGetValue("MaxLength", false, out facet))
          return facet.IsUnbounded;
        return false;
      }
    }

    /// <summary>Gets whether the precision facet is constant for the database provider.</summary>
    /// <returns>true if the facet is constant; otherwise, false.</returns>
    public bool IsPrecisionConstant
    {
      get
      {
        Facet facet;
        if (this.TypeUsage.Facets.TryGetValue("Precision", false, out facet))
          return facet.Description.IsConstant;
        return false;
      }
    }

    /// <summary>Gets the precision value of the parameter.</summary>
    /// <returns>The precision value of the parameter.</returns>
    public byte? Precision
    {
      get
      {
        Facet facet;
        if (!this.TypeUsage.Facets.TryGetValue(nameof (Precision), false, out facet))
          return new byte?();
        return facet.Value as byte?;
      }
    }

    /// <summary>Gets whether the scale facet is constant for the database provider.</summary>
    /// <returns>true if the facet is constant; otherwise, false.</returns>
    public bool IsScaleConstant
    {
      get
      {
        Facet facet;
        if (this.TypeUsage.Facets.TryGetValue("Scale", false, out facet))
          return facet.Description.IsConstant;
        return false;
      }
    }

    /// <summary>Gets the scale value of the parameter.</summary>
    /// <returns>The scale value of the parameter.</returns>
    public byte? Scale
    {
      get
      {
        Facet facet;
        if (!this.TypeUsage.Facets.TryGetValue(nameof (Scale), false, out facet))
          return new byte?();
        return facet.Value as byte?;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> on which this parameter is declared.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> object that represents the function on which this parameter is declared.
    /// </returns>
    public EdmFunction DeclaringFunction
    {
      get
      {
        return this._declaringFunction.Value;
      }
    }

    /// <summary>
    /// Returns the name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.FunctionParameter" />.
    /// </summary>
    /// <returns>
    /// The name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.FunctionParameter" />.
    /// </returns>
    public override string ToString()
    {
      return this.Name;
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      base.SetReadOnly();
    }

    /// <summary>
    /// The factory method for constructing the <see cref="T:System.Data.Entity.Core.Metadata.Edm.FunctionParameter" /> object.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="edmType">The EdmType of the parameter.</param>
    /// <param name="parameterMode">
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.ParameterMode" /> of the parameter.
    /// </param>
    /// <returns>
    /// A new, read-only instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> type.
    /// </returns>
    public static FunctionParameter Create(
      string name,
      EdmType edmType,
      ParameterMode parameterMode)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<EdmType>(edmType, nameof (edmType));
      FunctionParameter functionParameter = new FunctionParameter(name, TypeUsage.Create(edmType, FacetValues.NullFacetValues), parameterMode);
      functionParameter.SetReadOnly();
      return functionParameter;
    }
  }
}
