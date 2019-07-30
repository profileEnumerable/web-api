// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.TypeUsage
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Class representing a type information for an item</summary>
  [DebuggerDisplay("EdmType={EdmType}, Facets.Count={Facets.Count}")]
  public class TypeUsage : MetadataItem
  {
    private static readonly string[] _identityFacets = new string[8]
    {
      "DefaultValue",
      "FixedLength",
      "MaxLength",
      "Nullable",
      "Precision",
      "Scale",
      "Unicode",
      "SRID"
    };
    internal static readonly EdmConstants.Unbounded DefaultMaxLengthFacetValue = EdmConstants.UnboundedValue;
    internal static readonly EdmConstants.Unbounded DefaultPrecisionFacetValue = EdmConstants.UnboundedValue;
    internal static readonly EdmConstants.Unbounded DefaultScaleFacetValue = EdmConstants.UnboundedValue;
    internal static readonly byte? DefaultDateTimePrecisionFacetValue = new byte?();
    internal const bool DefaultUnicodeFacetValue = true;
    internal const bool DefaultFixedLengthFacetValue = false;
    private TypeUsage _modelTypeUsage;
    private readonly EdmType _edmType;
    private ReadOnlyMetadataCollection<Facet> _facets;
    private string _identity;

    internal TypeUsage()
    {
    }

    private TypeUsage(EdmType edmType)
      : base(MetadataItem.MetadataFlags.Readonly)
    {
      Check.NotNull<EdmType>(edmType, nameof (edmType));
      this._edmType = edmType;
    }

    private TypeUsage(EdmType edmType, IEnumerable<Facet> facets)
      : this(edmType)
    {
      MetadataCollection<Facet> metadataCollection = MetadataCollection<Facet>.Wrap(facets.ToList<Facet>());
      metadataCollection.SetReadOnly();
      this._facets = metadataCollection.AsReadOnlyMetadataCollection();
    }

    internal static TypeUsage Create(EdmType edmType)
    {
      return new TypeUsage(edmType);
    }

    internal static TypeUsage Create(EdmType edmType, FacetValues values)
    {
      return new TypeUsage(edmType, TypeUsage.GetDefaultFacetDescriptionsAndOverrideFacetValues(edmType, values));
    }

    /// <summary>
    /// Factory method for creating a TypeUsage with specified EdmType and facets
    /// </summary>
    /// <param name="edmType"> EdmType for which to create a type usage </param>
    /// <param name="facets"> facets to be copied into the new TypeUsage </param>
    /// <returns> new TypeUsage instance </returns>
    public static TypeUsage Create(EdmType edmType, IEnumerable<Facet> facets)
    {
      return new TypeUsage(edmType, facets);
    }

    internal TypeUsage ShallowCopy(FacetValues facetValues)
    {
      return TypeUsage.Create(this._edmType, TypeUsage.OverrideFacetValues((IEnumerable<Facet>) this.Facets, facetValues));
    }

    internal TypeUsage ShallowCopy(params Facet[] facetValues)
    {
      return TypeUsage.Create(this._edmType, TypeUsage.OverrideFacetValues((IEnumerable<Facet>) this.Facets, (IEnumerable<Facet>) facetValues));
    }

    private static IEnumerable<Facet> OverrideFacetValues(
      IEnumerable<Facet> facets,
      IEnumerable<Facet> facetValues)
    {
      return facets.Except<Facet>(facetValues, (Func<Facet, Facet, bool>) ((f1, f2) => f1.EdmEquals((MetadataItem) f2))).Union<Facet>(facetValues);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object with the specified conceptual model type.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object with the default facet values for the specified
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// .
    /// </returns>
    /// <param name="edmType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> for which the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// object is created.
    /// </param>
    public static TypeUsage CreateDefaultTypeUsage(EdmType edmType)
    {
      Check.NotNull<EdmType>(edmType, nameof (edmType));
      return TypeUsage.Create(edmType);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object to describe a string type by using the specified facet values.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object describing a string type by using the specified facet values.
    /// </returns>
    /// <param name="primitiveType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" /> for which the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// object is created.
    /// </param>
    /// <param name="isUnicode">true to set the character-encoding standard of the string type to Unicode; otherwise, false.</param>
    /// <param name="isFixedLength">true to set the character-encoding standard of the string type to Unicode; otherwise, false.</param>
    /// <param name="maxLength">true to set the length of the string type to fixed; otherwise, false.</param>
    public static TypeUsage CreateStringTypeUsage(
      PrimitiveType primitiveType,
      bool isUnicode,
      bool isFixedLength,
      int maxLength)
    {
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.String)
        throw new ArgumentException(Strings.NotStringTypeForTypeUsage);
      TypeUsage.ValidateMaxLength(maxLength);
      return TypeUsage.Create((EdmType) primitiveType, new FacetValues()
      {
        MaxLength = (FacetValueContainer<int?>) new int?(maxLength),
        Unicode = (FacetValueContainer<bool?>) new bool?(isUnicode),
        FixedLength = (FacetValueContainer<bool?>) new bool?(isFixedLength)
      });
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object to describe a string type by using the specified facet values and unbounded MaxLength.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object describing a string type by using the specified facet values and unbounded MaxLength.
    /// </returns>
    /// <param name="primitiveType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" /> for which the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// object is created.
    /// </param>
    /// <param name="isUnicode">true to set the character-encoding standard of the string type to Unicode; otherwise, false.</param>
    /// <param name="isFixedLength">true to set the length of the string type to fixed; otherwise, false</param>
    public static TypeUsage CreateStringTypeUsage(
      PrimitiveType primitiveType,
      bool isUnicode,
      bool isFixedLength)
    {
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.String)
        throw new ArgumentException(Strings.NotStringTypeForTypeUsage);
      return TypeUsage.Create((EdmType) primitiveType, new FacetValues()
      {
        MaxLength = (FacetValueContainer<int?>) TypeUsage.DefaultMaxLengthFacetValue,
        Unicode = (FacetValueContainer<bool?>) new bool?(isUnicode),
        FixedLength = (FacetValueContainer<bool?>) new bool?(isFixedLength)
      });
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object to describe a binary type by using the specified facet values.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object describing a binary type by using the specified facet values.
    /// </returns>
    /// <param name="primitiveType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" /> for which the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// object is created.
    /// </param>
    /// <param name="isFixedLength">true to set the length of the binary type to fixed; otherwise, false.</param>
    /// <param name="maxLength">The maximum length of the binary type.</param>
    public static TypeUsage CreateBinaryTypeUsage(
      PrimitiveType primitiveType,
      bool isFixedLength,
      int maxLength)
    {
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Binary)
        throw new ArgumentException(Strings.NotBinaryTypeForTypeUsage);
      TypeUsage.ValidateMaxLength(maxLength);
      return TypeUsage.Create((EdmType) primitiveType, new FacetValues()
      {
        MaxLength = (FacetValueContainer<int?>) new int?(maxLength),
        FixedLength = (FacetValueContainer<bool?>) new bool?(isFixedLength)
      });
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object to describe a binary type by using the specified facet values.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object describing a binary type by using the specified facet values.
    /// </returns>
    /// <param name="primitiveType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" /> for which the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// object is created.
    /// </param>
    /// <param name="isFixedLength">true to set the length of the binary type to fixed; otherwise, false. </param>
    public static TypeUsage CreateBinaryTypeUsage(
      PrimitiveType primitiveType,
      bool isFixedLength)
    {
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Binary)
        throw new ArgumentException(Strings.NotBinaryTypeForTypeUsage);
      return TypeUsage.Create((EdmType) primitiveType, new FacetValues()
      {
        MaxLength = (FacetValueContainer<int?>) TypeUsage.DefaultMaxLengthFacetValue,
        FixedLength = (FacetValueContainer<bool?>) new bool?(isFixedLength)
      });
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Metadata.Edm.DateTimeTypeUsage" /> object of the type that the parameters describe.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Metadata.Edm.DateTimeTypeUsage" /> object.
    /// </returns>
    /// <param name="primitiveType">
    /// The simple type that defines the units of measurement of the <see cref="T:System." />DateTime object.
    /// </param>
    /// <param name="precision">
    /// The degree of granularity of the <see cref="T:System." />DateTimeOffset in fractions of a second, based on the number of decimal places supported. For example a precision of 3 means the granularity supported is milliseconds.
    /// </param>
    public static TypeUsage CreateDateTimeTypeUsage(
      PrimitiveType primitiveType,
      byte? precision)
    {
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.DateTime)
        throw new ArgumentException(Strings.NotDateTimeTypeForTypeUsage);
      return TypeUsage.Create((EdmType) primitiveType, new FacetValues()
      {
        Precision = (FacetValueContainer<byte?>) precision
      });
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Metadata.Edm.DateTimeOffsetTypeUsage" /> object of the type that the parameters describe.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Metadata.Edm.DateTimeOffsetTypeUsage" /> object.
    /// </returns>
    /// <param name="primitiveType">The simple type that defines the units of measurement of the offset.</param>
    /// <param name="precision">
    /// The degree of granularity of the <see cref="T:System." />DateTimeOffset in fractions of a second, based on the number of decimal places supported. For example a precision of 3 means the granularity supported is milliseconds.
    /// </param>
    public static TypeUsage CreateDateTimeOffsetTypeUsage(
      PrimitiveType primitiveType,
      byte? precision)
    {
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.DateTimeOffset)
        throw new ArgumentException(Strings.NotDateTimeOffsetTypeForTypeUsage);
      return TypeUsage.Create((EdmType) primitiveType, new FacetValues()
      {
        Precision = (FacetValueContainer<byte?>) precision
      });
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Metadata.Edm.TimeTypeUsage" /> object of the type that the parameters describe.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Metadata.Edm.TimeTypeUsage" /> object.
    /// </returns>
    /// <param name="primitiveType">
    /// The simple type that defines the units of measurement of the <see cref="T:System." />DateTime object.
    /// </param>
    /// <param name="precision">
    /// The degree of granularity of the <see cref="T:System." />DateTimeOffset in fractions of a second, based on the number of decimal places supported. For example a precision of 3 means the granularity supported is milliseconds.
    /// </param>
    public static TypeUsage CreateTimeTypeUsage(
      PrimitiveType primitiveType,
      byte? precision)
    {
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Time)
        throw new ArgumentException(Strings.NotTimeTypeForTypeUsage);
      return TypeUsage.Create((EdmType) primitiveType, new FacetValues()
      {
        Precision = (FacetValueContainer<byte?>) precision
      });
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object to describe a decimal type by using the specified facet values.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object describing a decimal type by using the specified facet values.
    /// </returns>
    /// <param name="primitiveType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" /> for which the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// object is created.
    /// </param>
    /// <param name="precision">
    /// The precision of the decimal type as type <see cref="T:System.Byte" />.
    /// </param>
    /// <param name="scale">
    /// The scale of the decimal type as type <see cref="T:System.Byte" />.
    /// </param>
    public static TypeUsage CreateDecimalTypeUsage(
      PrimitiveType primitiveType,
      byte precision,
      byte scale)
    {
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Decimal)
        throw new ArgumentException(Strings.NotDecimalTypeForTypeUsage);
      return TypeUsage.Create((EdmType) primitiveType, new FacetValues()
      {
        Precision = (FacetValueContainer<byte?>) new byte?(precision),
        Scale = (FacetValueContainer<byte?>) new byte?(scale)
      });
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object to describe a decimal type with unbounded precision and scale facet values.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object describing a decimal type with unbounded precision and scale facet values.
    /// </returns>
    /// <param name="primitiveType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.PrimitiveType" /> for which the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// object is created.
    /// </param>
    public static TypeUsage CreateDecimalTypeUsage(PrimitiveType primitiveType)
    {
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Decimal)
        throw new ArgumentException(Strings.NotDecimalTypeForTypeUsage);
      return TypeUsage.Create((EdmType) primitiveType, new FacetValues()
      {
        Precision = (FacetValueContainer<byte?>) TypeUsage.DefaultPrecisionFacetValue,
        Scale = (FacetValueContainer<byte?>) TypeUsage.DefaultScaleFacetValue
      });
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.TypeUsage;
      }
    }

    /// <summary>
    /// Gets the type information described by this <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object that represents the type information described by this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// .
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.EdmType, false)]
    public virtual EdmType EdmType
    {
      get
      {
        return this._edmType;
      }
    }

    /// <summary>
    /// Gets the list of facets for the type that is described by this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// .
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of facets for the type that is described by this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// .
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.Facet, true)]
    public virtual ReadOnlyMetadataCollection<Facet> Facets
    {
      get
      {
        if (this._facets == null)
        {
          MetadataCollection<Facet> metadataCollection = new MetadataCollection<Facet>(this.GetFacets());
          metadataCollection.SetReadOnly();
          Interlocked.CompareExchange<ReadOnlyMetadataCollection<Facet>>(ref this._facets, metadataCollection.AsReadOnlyMetadataCollection(), (ReadOnlyMetadataCollection<Facet>) null);
        }
        return this._facets;
      }
    }

    /// <summary>Returns a Model type usage for a provider type</summary>
    /// <value> Model (CSpace) type usage </value>
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public TypeUsage ModelTypeUsage
    {
      get
      {
        if (this._modelTypeUsage == null)
        {
          EdmType edmType = this.EdmType;
          if (edmType.DataSpace == DataSpace.CSpace || edmType.DataSpace == DataSpace.OSpace)
            return this;
          TypeUsage typeUsage;
          if (Helper.IsRowType((GlobalItem) edmType))
          {
            RowType rowType = (RowType) edmType;
            EdmProperty[] edmPropertyArray = new EdmProperty[rowType.Properties.Count];
            for (int index = 0; index < edmPropertyArray.Length; ++index)
            {
              EdmProperty property = rowType.Properties[index];
              TypeUsage modelTypeUsage = property.TypeUsage.ModelTypeUsage;
              edmPropertyArray[index] = new EdmProperty(property.Name, modelTypeUsage);
            }
            typeUsage = TypeUsage.Create((EdmType) new RowType((IEnumerable<EdmProperty>) edmPropertyArray, rowType.InitializerMetadata), (IEnumerable<Facet>) this.Facets);
          }
          else if (Helper.IsCollectionType((GlobalItem) edmType))
            typeUsage = TypeUsage.Create((EdmType) new CollectionType(((CollectionType) edmType).TypeUsage.ModelTypeUsage), (IEnumerable<Facet>) this.Facets);
          else if (Helper.IsPrimitiveType(edmType))
          {
            typeUsage = ((PrimitiveType) edmType).ProviderManifest.GetEdmType(this);
            if (typeUsage == null)
              throw new ProviderIncompatibleException(Strings.Mapping_ProviderReturnsNullType((object) this.ToString()));
            if (!TypeSemantics.IsNullable(this))
              typeUsage = TypeUsage.Create(typeUsage.EdmType, TypeUsage.OverrideFacetValues((IEnumerable<Facet>) typeUsage.Facets, new FacetValues()
              {
                Nullable = (FacetValueContainer<bool?>) new bool?(false)
              }));
          }
          else
          {
            if (!Helper.IsEntityTypeBase(edmType) && !Helper.IsComplexType(edmType))
              return (TypeUsage) null;
            typeUsage = this;
          }
          Interlocked.CompareExchange<TypeUsage>(ref this._modelTypeUsage, typeUsage, (TypeUsage) null);
        }
        return this._modelTypeUsage;
      }
    }

    /// <summary>
    /// Checks whether this <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> is a subtype of the specified
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// .
    /// </summary>
    /// <returns>
    /// true if this <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> is a subtype of the specified
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// ; otherwise, false.
    /// </returns>
    /// <param name="typeUsage">
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object to be checked.
    /// </param>
    public bool IsSubtypeOf(TypeUsage typeUsage)
    {
      if (this.EdmType == null || typeUsage == null)
        return false;
      return this.EdmType.IsSubtypeOf(typeUsage.EdmType);
    }

    private IEnumerable<Facet> GetFacets()
    {
      return this._edmType.GetAssociatedFacetDescriptions().Select<FacetDescription, Facet>((Func<FacetDescription, Facet>) (facetDescription => facetDescription.DefaultValueFacet));
    }

    internal override void SetReadOnly()
    {
      base.SetReadOnly();
    }

    internal override string Identity
    {
      get
      {
        if (this.Facets.Count == 0)
          return this.EdmType.Identity;
        if (this._identity == null)
        {
          StringBuilder builder = new StringBuilder(128);
          this.BuildIdentity(builder);
          Interlocked.CompareExchange<string>(ref this._identity, builder.ToString(), (string) null);
        }
        return this._identity;
      }
    }

    private static IEnumerable<Facet> GetDefaultFacetDescriptionsAndOverrideFacetValues(
      EdmType type,
      FacetValues values)
    {
      return TypeUsage.OverrideFacetValues<FacetDescription>(type.GetAssociatedFacetDescriptions(), (Func<FacetDescription, FacetDescription>) (fd => fd), (Func<FacetDescription, Facet>) (fd => fd.DefaultValueFacet), values);
    }

    private static IEnumerable<Facet> OverrideFacetValues(
      IEnumerable<Facet> facets,
      FacetValues values)
    {
      return TypeUsage.OverrideFacetValues<Facet>(facets, (Func<Facet, FacetDescription>) (f => f.Description), (Func<Facet, Facet>) (f => f), values);
    }

    private static IEnumerable<Facet> OverrideFacetValues<T>(
      IEnumerable<T> facetThings,
      Func<T, FacetDescription> getDescription,
      Func<T, Facet> getFacet,
      FacetValues values)
    {
      foreach (T facetThing in facetThings)
      {
        FacetDescription description = getDescription(facetThing);
        Facet facet;
        if (!description.IsConstant && values.TryGetFacet(description, out facet))
          yield return facet;
        else
          yield return getFacet(facetThing);
      }
    }

    internal override void BuildIdentity(StringBuilder builder)
    {
      if (this._identity != null)
      {
        builder.Append(this._identity);
      }
      else
      {
        builder.Append(this.EdmType.Identity);
        builder.Append("(");
        bool flag = true;
        for (int index = 0; index < this.Facets.Count; ++index)
        {
          Facet facet = this.Facets[index];
          if (0 <= Array.BinarySearch<string>(TypeUsage._identityFacets, facet.Name, (IComparer<string>) StringComparer.Ordinal))
          {
            if (flag)
              flag = false;
            else
              builder.Append(",");
            builder.Append(facet.Name);
            builder.Append("=");
            builder.Append(facet.Value ?? (object) string.Empty);
          }
        }
        builder.Append(")");
      }
    }

    /// <summary>
    /// Returns the full name of the type described by this <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />.
    /// </summary>
    /// <returns>
    /// The full name of the type described by this <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> as string.
    /// </returns>
    public override string ToString()
    {
      return this.EdmType.ToString();
    }

    internal override bool EdmEquals(MetadataItem item)
    {
      if (object.ReferenceEquals((object) this, (object) item))
        return true;
      if (item == null || BuiltInTypeKind.TypeUsage != item.BuiltInTypeKind)
        return false;
      TypeUsage typeUsage = (TypeUsage) item;
      if (!this.EdmType.EdmEquals((MetadataItem) typeUsage.EdmType))
        return false;
      if (this._facets == null && typeUsage._facets == null)
        return true;
      if (this.Facets.Count != typeUsage.Facets.Count)
        return false;
      foreach (Facet facet1 in this.Facets)
      {
        Facet facet2;
        if (!typeUsage.Facets.TryGetValue(facet1.Name, false, out facet2) || !object.Equals(facet1.Value, facet2.Value))
          return false;
      }
      return true;
    }

    private static void ValidateMaxLength(int maxLength)
    {
      if (maxLength <= 0)
        throw new ArgumentOutOfRangeException(nameof (maxLength), Strings.InvalidMaxLengthSize);
    }
  }
}
