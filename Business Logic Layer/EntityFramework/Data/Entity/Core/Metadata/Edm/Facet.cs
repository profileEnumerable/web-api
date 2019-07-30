// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.Facet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Class for representing a Facet object
  /// This object is Immutable (not just set to readonly) and
  /// some parts of the system are depending on that behavior
  /// </summary>
  [DebuggerDisplay("{Name,nq}={Value}")]
  public class Facet : MetadataItem
  {
    private readonly FacetDescription _facetDescription;
    private readonly object _value;

    internal Facet()
    {
    }

    private Facet(FacetDescription facetDescription, object value)
      : base(MetadataItem.MetadataFlags.Readonly)
    {
      Check.NotNull<FacetDescription>(facetDescription, nameof (facetDescription));
      this._facetDescription = facetDescription;
      this._value = value;
    }

    internal static Facet Create(FacetDescription facetDescription, object value)
    {
      return Facet.Create(facetDescription, value, false);
    }

    internal static Facet Create(
      FacetDescription facetDescription,
      object value,
      bool bypassKnownValues)
    {
      if (!bypassKnownValues)
      {
        if (object.ReferenceEquals(value, (object) null))
          return facetDescription.NullValueFacet;
        if (object.Equals(facetDescription.DefaultValue, value))
          return facetDescription.DefaultValueFacet;
        if (facetDescription.FacetType.Identity == "Edm.Boolean")
        {
          bool flag = (bool) value;
          return facetDescription.GetBooleanFacet(flag);
        }
      }
      Facet facet = new Facet(facetDescription, value);
      if (value != null && !Helper.IsUnboundedFacetValue(facet) && (!Helper.IsVariableFacetValue(facet) && facet.FacetType.ClrType != (Type) null))
        value.GetType();
      return facet;
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.Facet;
      }
    }

    /// <summary>
    /// Gets the description of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.FacetDescription" /> object that represents the description of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />
    /// .
    /// </returns>
    public FacetDescription Description
    {
      get
      {
        return this._facetDescription;
      }
    }

    /// <summary>
    /// Gets the name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />.
    /// </summary>
    /// <returns>
    /// The name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />.
    /// </returns>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public virtual string Name
    {
      get
      {
        return this._facetDescription.FacetName;
      }
    }

    /// <summary>
    /// Gets the type of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object that represents the type of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />
    /// .
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.EdmType, false)]
    public EdmType FacetType
    {
      get
      {
        return this._facetDescription.FacetType;
      }
    }

    /// <summary>
    /// Gets the value of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />.
    /// </summary>
    /// <returns>
    /// The value of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the Facet instance is in ReadOnly state</exception>
    [MetadataProperty(typeof (object), false)]
    public virtual object Value
    {
      get
      {
        return this._value;
      }
    }

    internal override string Identity
    {
      get
      {
        return this._facetDescription.FacetName;
      }
    }

    /// <summary>Gets a value indicating whether the value of the facet is unbounded.</summary>
    /// <returns>true if the value of the facet is unbounded; otherwise, false.</returns>
    public bool IsUnbounded
    {
      get
      {
        return object.ReferenceEquals(this.Value, (object) EdmConstants.UnboundedValue);
      }
    }

    /// <summary>
    /// Returns the name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />.
    /// </summary>
    /// <returns>
    /// The name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.Facet" />.
    /// </returns>
    public override string ToString()
    {
      return this.Name;
    }
  }
}
