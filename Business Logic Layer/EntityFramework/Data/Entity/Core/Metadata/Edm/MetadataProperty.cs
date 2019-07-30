// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataProperty
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Class representing a metadata attribute for an item</summary>
  public class MetadataProperty : MetadataItem
  {
    private readonly string _name;
    private readonly PropertyKind _propertyKind;
    private object _value;
    private readonly TypeUsage _typeUsage;

    internal MetadataProperty()
    {
    }

    internal MetadataProperty(string name, TypeUsage typeUsage, object value)
    {
      Check.NotNull<TypeUsage>(typeUsage, nameof (typeUsage));
      this._name = name;
      this._value = value;
      this._typeUsage = typeUsage;
      this._propertyKind = PropertyKind.Extended;
    }

    internal MetadataProperty(string name, EdmType edmType, bool isCollectionType, object value)
    {
      this._name = name;
      this._value = value;
      this._typeUsage = !isCollectionType ? TypeUsage.Create(edmType) : TypeUsage.Create((EdmType) edmType.GetCollectionType());
      this._propertyKind = PropertyKind.System;
    }

    private MetadataProperty(string name, object value)
    {
      this._name = name;
      this._value = value;
      this._propertyKind = PropertyKind.Extended;
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataProperty" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataProperty" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.MetadataProperty;
      }
    }

    internal override string Identity
    {
      get
      {
        return this.Name;
      }
    }

    /// <summary>
    /// Gets the name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataProperty" />.
    /// </summary>
    /// <returns>
    /// The name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataProperty" />.
    /// </returns>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public virtual string Name
    {
      get
      {
        return this._name;
      }
    }

    /// <summary>
    /// Gets the value of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataProperty" />.
    /// </summary>
    /// <returns>
    /// The value of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataProperty" />.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the MetadataProperty instance is in readonly state</exception>
    [MetadataProperty(typeof (object), false)]
    public virtual object Value
    {
      get
      {
        MetadataPropertyValue metadataPropertyValue = this._value as MetadataPropertyValue;
        if (metadataPropertyValue != null)
          return metadataPropertyValue.GetValue();
        return this._value;
      }
      set
      {
        Check.NotNull<object>(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._value = value;
      }
    }

    /// <summary>
    /// Gets the instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> class that contains both the type of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataProperty" />
    /// and facets for the type.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object that contains both the type of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataProperty" />
    /// and facets for the type.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the MetadataProperty instance is in readonly state</exception>
    [MetadataProperty(BuiltInTypeKind.TypeUsage, false)]
    public TypeUsage TypeUsage
    {
      get
      {
        return this._typeUsage;
      }
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      base.SetReadOnly();
    }

    /// <summary>
    /// Gets the value of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.PropertyKind" />.
    /// </summary>
    /// <returns>
    /// The value of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.PropertyKind" />.
    /// </returns>
    public virtual PropertyKind PropertyKind
    {
      get
      {
        return this._propertyKind;
      }
    }

    /// <summary>
    /// Gets a boolean that indicates whether the metadata property is an annotation.
    /// </summary>
    public bool IsAnnotation
    {
      get
      {
        if (this.PropertyKind == PropertyKind.Extended)
          return this.TypeUsage == null;
        return false;
      }
    }

    /// <summary>
    /// The factory method for constructing the MetadataProperty object.
    /// </summary>
    /// <param name="name">The name of the metadata property.</param>
    /// <param name="typeUsage">The type usage of the metadata property.</param>
    /// <param name="value">The value of the metadata property.</param>
    /// <returns>The MetadataProperty object.</returns>
    /// <exception cref="T:System.NullReferenceException">
    /// Thrown <paramref name="typeUsage" /> is <c>null</c>.
    /// </exception>
    /// <remarks>The newly created MetadataProperty will be read only.</remarks>
    public static MetadataProperty Create(
      string name,
      TypeUsage typeUsage,
      object value)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<TypeUsage>(typeUsage, nameof (typeUsage));
      MetadataProperty metadataProperty = new MetadataProperty(name, typeUsage, value);
      metadataProperty.SetReadOnly();
      return metadataProperty;
    }

    /// <summary>
    /// Creates a metadata annotation having the specified name and value.
    /// </summary>
    /// <param name="name">The annotation name.</param>
    /// <param name="value">The annotation value.</param>
    /// <returns>A MetadataProperty instance representing the created annotation.</returns>
    public static MetadataProperty CreateAnnotation(string name, object value)
    {
      Check.NotEmpty(name, nameof (name));
      return new MetadataProperty(name, value);
    }
  }
}
