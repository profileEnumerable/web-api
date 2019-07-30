// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmProperty
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// In conceptual-space, EdmProperty represents a property on an Entity.
  /// In store-space, EdmProperty represents a column in a table.
  /// </summary>
  public class EdmProperty : EdmMember
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly Type _entityDeclaringType;
    private Func<object, object> _memberGetter;
    private Action<object, object> _memberSetter;

    /// <summary> Creates a new primitive property. </summary>
    /// <returns> The newly created property. </returns>
    /// <param name="name"> The name of the property. </param>
    /// <param name="primitiveType"> The type of the property. </param>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static EdmProperty CreatePrimitive(string name, PrimitiveType primitiveType)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<PrimitiveType>(primitiveType, nameof (primitiveType));
      return EdmProperty.CreateProperty(name, (EdmType) primitiveType);
    }

    /// <summary> Creates a new enum property. </summary>
    /// <returns> The newly created property. </returns>
    /// <param name="name"> The name of the property. </param>
    /// <param name="enumType"> The type of the property. </param>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static EdmProperty CreateEnum(string name, EnumType enumType)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<EnumType>(enumType, nameof (enumType));
      return EdmProperty.CreateProperty(name, (EdmType) enumType);
    }

    /// <summary> Creates a new complex property. </summary>
    /// <returns> The newly created property. </returns>
    /// <param name="name"> The name of the property. </param>
    /// <param name="complexType"> The type of the property. </param>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static EdmProperty CreateComplex(string name, ComplexType complexType)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<ComplexType>(complexType, nameof (complexType));
      EdmProperty property = EdmProperty.CreateProperty(name, (EdmType) complexType);
      property.Nullable = false;
      return property;
    }

    /// <summary>Creates a new instance of EdmProperty type.</summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="typeUsage">
    /// Property <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" />
    /// </param>
    /// <returns>A new instance of EdmProperty type</returns>
    public static EdmProperty Create(string name, TypeUsage typeUsage)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<TypeUsage>(typeUsage, nameof (typeUsage));
      EdmType edmType = typeUsage.EdmType;
      if (!Helper.IsPrimitiveType(edmType) && !Helper.IsEnumType(edmType) && !Helper.IsComplexType(edmType))
        throw new ArgumentException(Strings.EdmProperty_InvalidPropertyType((object) edmType.FullName));
      return new EdmProperty(name, typeUsage);
    }

    private static EdmProperty CreateProperty(string name, EdmType edmType)
    {
      TypeUsage typeUsage = TypeUsage.Create(edmType, new FacetValues());
      return new EdmProperty(name, typeUsage);
    }

    internal EdmProperty(string name, TypeUsage typeUsage)
      : base(name, typeUsage)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<TypeUsage>(typeUsage, nameof (typeUsage));
    }

    internal EdmProperty(
      string name,
      TypeUsage typeUsage,
      PropertyInfo propertyInfo,
      Type entityDeclaringType)
      : this(name, typeUsage)
    {
      this._propertyInfo = propertyInfo;
      this._entityDeclaringType = entityDeclaringType;
    }

    internal EdmProperty(string name)
      : this(name, TypeUsage.Create((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.String)))
    {
    }

    internal PropertyInfo PropertyInfo
    {
      get
      {
        return this._propertyInfo;
      }
    }

    internal Type EntityDeclaringType
    {
      get
      {
        return this._entityDeclaringType;
      }
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmProperty" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmProperty" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.EdmProperty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmProperty" /> can have a null value.
    /// </summary>
    /// <remarks>
    /// Nullability in the conceptual model and store model is a simple indication of whether or not
    /// the property is considered nullable. Nullability in the object model is more complex.
    /// When using convention based mapping (as usually happens with POCO entities), a property in the
    /// object model is considered nullable if and only if the underlying CLR type is nullable and
    /// the property is not part of the primary key.
    /// When using attribute based mapping (usually used with entities that derive from the EntityObject
    /// base class), a property is considered nullable if the IsNullable flag is set to true in the
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EdmScalarPropertyAttribute" /> attribute. This flag can
    /// be set to true even if the underlying type is not nullable, and can be set to false even if the
    /// underlying type is nullable. The latter case happens as part of default code generation when
    /// a non-nullable property in the conceptual model is mapped to a nullable CLR type such as a string.
    /// In such a case, the Entity Framework treats the property as non-nullable even though the CLR would
    /// allow null to be set.
    /// There is no good reason to set a non-nullable CLR type as nullable in the object model and this
    /// should not be done even though the attribute allows it.
    /// </remarks>
    /// <returns>
    /// true if this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmProperty" /> can have a null value; otherwise, false.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the setter is called when the EdmProperty instance is in ReadOnly state</exception>
    public bool Nullable
    {
      get
      {
        return (bool) this.TypeUsage.Facets[nameof (Nullable)].Value;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this.TypeUsage = this.TypeUsage.ShallowCopy(new FacetValues()
        {
          Nullable = (FacetValueContainer<bool?>) new bool?(value)
        });
      }
    }

    /// <summary>Gets the type name of the property.</summary>
    /// <returns>The type name of the property.</returns>
    public string TypeName
    {
      get
      {
        return this.TypeUsage.EdmType.Name;
      }
    }

    /// <summary>
    /// Gets the default value for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmProperty" />.
    /// </summary>
    /// <returns>
    /// The default value for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmProperty" />.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the setter is called when the EdmProperty instance is in ReadOnly state</exception>
    public object DefaultValue
    {
      get
      {
        return this.TypeUsage.Facets[nameof (DefaultValue)].Value;
      }
      internal set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this.TypeUsage = this.TypeUsage.ShallowCopy(new FacetValues()
        {
          DefaultValue = value
        });
      }
    }

    internal Func<object, object> ValueGetter
    {
      get
      {
        return this._memberGetter;
      }
      set
      {
        Interlocked.CompareExchange<Func<object, object>>(ref this._memberGetter, value, (Func<object, object>) null);
      }
    }

    internal Action<object, object> ValueSetter
    {
      get
      {
        return this._memberSetter;
      }
      set
      {
        Interlocked.CompareExchange<Action<object, object>>(ref this._memberSetter, value, (Action<object, object>) null);
      }
    }

    internal bool IsKeyMember
    {
      get
      {
        EntityType declaringType = this.DeclaringType as EntityType;
        if (declaringType != null)
          return declaringType.KeyMembers.Contains((EdmMember) this);
        return false;
      }
    }

    /// <summary>Gets whether the property is a collection type property.</summary>
    /// <returns>true if the property is a collection type property; otherwise, false.</returns>
    public bool IsCollectionType
    {
      get
      {
        return this.TypeUsage.EdmType is CollectionType;
      }
    }

    /// <summary>Gets whether this property is a complex type property.</summary>
    /// <returns>true if this property is a complex type property; otherwise, false.</returns>
    public bool IsComplexType
    {
      get
      {
        return this.TypeUsage.EdmType is ComplexType;
      }
    }

    /// <summary>Gets whether this property is a primitive type.</summary>
    /// <returns>true if this property is a primitive type; otherwise, false.</returns>
    public bool IsPrimitiveType
    {
      get
      {
        return this.TypeUsage.EdmType is PrimitiveType;
      }
    }

    /// <summary>Gets whether this property is an enumeration type property.</summary>
    /// <returns>true if this property is an enumeration type property; otherwise, false.</returns>
    public bool IsEnumType
    {
      get
      {
        return this.TypeUsage.EdmType is EnumType;
      }
    }

    /// <summary>Gets whether this property is an underlying primitive type.</summary>
    /// <returns>true if this property is an underlying primitive type; otherwise, false.</returns>
    public bool IsUnderlyingPrimitiveType
    {
      get
      {
        if (!this.IsPrimitiveType)
          return this.IsEnumType;
        return true;
      }
    }

    /// <summary>Gets the complex type information for this property.</summary>
    /// <returns>The complex type information for this property.</returns>
    public ComplexType ComplexType
    {
      get
      {
        return this.TypeUsage.EdmType as ComplexType;
      }
    }

    /// <summary>Gets the primitive type information for this property.</summary>
    /// <returns>The primitive type information for this property.</returns>
    public PrimitiveType PrimitiveType
    {
      get
      {
        return this.TypeUsage.EdmType as PrimitiveType;
      }
      internal set
      {
        Check.NotNull<PrimitiveType>(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        StoreGeneratedPattern generatedPattern = this.StoreGeneratedPattern;
        ConcurrencyMode concurrencyMode = this.ConcurrencyMode;
        List<Facet> facetList = new List<Facet>();
        foreach (FacetDescription facetDescription in value.GetAssociatedFacetDescriptions())
        {
          Facet facet;
          if (this.TypeUsage.Facets.TryGetValue(facetDescription.FacetName, false, out facet) && (facet.Value == null && facet.Description.DefaultValue != null || facet.Value != null && !facet.Value.Equals(facet.Description.DefaultValue)))
            facetList.Add(facet);
        }
        this.TypeUsage = TypeUsage.Create((EdmType) value, FacetValues.Create((IEnumerable<Facet>) facetList));
        if (generatedPattern != StoreGeneratedPattern.None)
          this.StoreGeneratedPattern = generatedPattern;
        if (concurrencyMode == ConcurrencyMode.None)
          return;
        this.ConcurrencyMode = concurrencyMode;
      }
    }

    /// <summary>Gets the enumeration type information for this property.</summary>
    /// <returns>The enumeration type information for this property.</returns>
    public EnumType EnumType
    {
      get
      {
        return this.TypeUsage.EdmType as EnumType;
      }
    }

    /// <summary>Gets the underlying primitive type information for this property.</summary>
    /// <returns>The underlying primitive type information for this property.</returns>
    public PrimitiveType UnderlyingPrimitiveType
    {
      get
      {
        if (!this.IsUnderlyingPrimitiveType)
          return (PrimitiveType) null;
        if (!this.IsEnumType)
          return this.PrimitiveType;
        return this.EnumType.UnderlyingType;
      }
    }

    /// <summary>Gets or sets the concurrency mode for the property.</summary>
    /// <returns>The concurrency mode for the property.</returns>
    public ConcurrencyMode ConcurrencyMode
    {
      get
      {
        return MetadataHelper.GetConcurrencyMode((EdmMember) this);
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this.TypeUsage = this.TypeUsage.ShallowCopy(Facet.Create(Converter.ConcurrencyModeFacet, (object) value));
      }
    }

    /// <summary>Gets or sets the database generation method for the database column associated with this property</summary>
    /// <returns>The store generated pattern for the property.</returns>
    public StoreGeneratedPattern StoreGeneratedPattern
    {
      get
      {
        return MetadataHelper.GetStoreGeneratedPattern((EdmMember) this);
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this.TypeUsage = this.TypeUsage.ShallowCopy(Facet.Create(Converter.StoreGeneratedPatternFacet, (object) value));
      }
    }

    /// <summary>Gets or sets the kind of collection for this model.</summary>
    /// <returns>The kind of collection for this model.</returns>
    public CollectionKind CollectionKind
    {
      get
      {
        Facet facet;
        if (!this.TypeUsage.Facets.TryGetValue(nameof (CollectionKind), false, out facet))
          return CollectionKind.None;
        return (CollectionKind) facet.Value;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this.TypeUsage = this.TypeUsage.ShallowCopy(Facet.Create(MetadataItem.CollectionKindFacetDescription, (object) value));
      }
    }

    /// <summary>Gets whether the maximum length facet is constant for the database provider.</summary>
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

    /// <summary>Gets or sets the maximum length of the property.</summary>
    /// <returns>The maximum length of the property.</returns>
    public int? MaxLength
    {
      get
      {
        Facet facet;
        if (!this.TypeUsage.Facets.TryGetValue(nameof (MaxLength), false, out facet))
          return new int?();
        return facet.Value as int?;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        int? maxLength = this.MaxLength;
        int? nullable = value;
        if ((maxLength.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : (maxLength.HasValue != nullable.HasValue ? 1 : 0)) == 0)
          return;
        this.TypeUsage = this.TypeUsage.ShallowCopy(new FacetValues()
        {
          MaxLength = (FacetValueContainer<int?>) value
        });
      }
    }

    /// <summary>Gets or sets whether this property uses the maximum length supported by the provider.</summary>
    /// <returns>true if this property uses the maximum length supported by the provider; otherwise, false.</returns>
    public bool IsMaxLength
    {
      get
      {
        Facet facet;
        if (this.TypeUsage.Facets.TryGetValue("MaxLength", false, out facet))
          return facet.IsUnbounded;
        return false;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        if (!value)
          return;
        this.TypeUsage = this.TypeUsage.ShallowCopy(new FacetValues()
        {
          MaxLength = (FacetValueContainer<int?>) EdmConstants.UnboundedValue
        });
      }
    }

    /// <summary>Gets whether the fixed length facet is constant for the database provider.</summary>
    /// <returns>true if the facet is constant; otherwise, false.</returns>
    public bool IsFixedLengthConstant
    {
      get
      {
        Facet facet;
        if (this.TypeUsage.Facets.TryGetValue("FixedLength", false, out facet))
          return facet.Description.IsConstant;
        return false;
      }
    }

    /// <summary>Gets or sets whether the length of this property is fixed.</summary>
    /// <returns>true if the length of this property is fixed; otherwise, false.</returns>
    public bool? IsFixedLength
    {
      get
      {
        Facet facet;
        if (!this.TypeUsage.Facets.TryGetValue("FixedLength", false, out facet))
          return new bool?();
        return facet.Value as bool?;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        bool? isFixedLength = this.IsFixedLength;
        bool? nullable = value;
        if ((isFixedLength.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : (isFixedLength.HasValue != nullable.HasValue ? 1 : 0)) == 0)
          return;
        this.TypeUsage = this.TypeUsage.ShallowCopy(new FacetValues()
        {
          FixedLength = (FacetValueContainer<bool?>) value
        });
      }
    }

    /// <summary>Gets whether the Unicode facet is constant for the database provider.</summary>
    /// <returns>true if the facet is constant; otherwise, false.</returns>
    public bool IsUnicodeConstant
    {
      get
      {
        Facet facet;
        if (this.TypeUsage.Facets.TryGetValue("Unicode", false, out facet))
          return facet.Description.IsConstant;
        return false;
      }
    }

    /// <summary>Gets or sets whether this property is a Unicode property.</summary>
    /// <returns>true if this property is a Unicode property; otherwise, false.</returns>
    public bool? IsUnicode
    {
      get
      {
        Facet facet;
        if (!this.TypeUsage.Facets.TryGetValue("Unicode", false, out facet))
          return new bool?();
        return facet.Value as bool?;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        bool? isUnicode = this.IsUnicode;
        bool? nullable = value;
        if ((isUnicode.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : (isUnicode.HasValue != nullable.HasValue ? 1 : 0)) == 0)
          return;
        this.TypeUsage = this.TypeUsage.ShallowCopy(new FacetValues()
        {
          Unicode = (FacetValueContainer<bool?>) value
        });
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

    /// <summary>Gets or sets the precision of this property.</summary>
    /// <returns>The precision of this property.</returns>
    public byte? Precision
    {
      get
      {
        Facet facet;
        if (!this.TypeUsage.Facets.TryGetValue(nameof (Precision), false, out facet))
          return new byte?();
        return facet.Value as byte?;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        byte? precision = this.Precision;
        byte? nullable = value;
        if (((int) precision.GetValueOrDefault() != (int) nullable.GetValueOrDefault() ? 1 : (precision.HasValue != nullable.HasValue ? 1 : 0)) == 0)
          return;
        this.TypeUsage = this.TypeUsage.ShallowCopy(new FacetValues()
        {
          Precision = (FacetValueContainer<byte?>) value
        });
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

    /// <summary>Gets or sets the scale of this property.</summary>
    /// <returns>The scale of this property.</returns>
    public byte? Scale
    {
      get
      {
        Facet facet;
        if (!this.TypeUsage.Facets.TryGetValue(nameof (Scale), false, out facet))
          return new byte?();
        return facet.Value as byte?;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        byte? scale = this.Scale;
        byte? nullable = value;
        if (((int) scale.GetValueOrDefault() != (int) nullable.GetValueOrDefault() ? 1 : (scale.HasValue != nullable.HasValue ? 1 : 0)) == 0)
          return;
        this.TypeUsage = this.TypeUsage.ShallowCopy(new FacetValues()
        {
          Scale = (FacetValueContainer<byte?>) value
        });
      }
    }

    /// <summary>Sets the metadata properties.</summary>
    /// <param name="metadataProperties">The metadata properties to be set.</param>
    public void SetMetadataProperties(IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotNull<IEnumerable<MetadataProperty>>(metadataProperties, nameof (metadataProperties));
      Util.ThrowIfReadOnly((MetadataItem) this);
      this.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
    }
  }
}
