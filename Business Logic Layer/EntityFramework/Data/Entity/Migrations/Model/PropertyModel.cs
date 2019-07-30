// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.PropertyModel
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents information about a property of an entity.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public abstract class PropertyModel
  {
    private readonly PrimitiveTypeKind _type;
    private TypeUsage _typeUsage;

    /// <summary>
    /// Initializes a new instance of the PropertyModel class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="type"> The data type for this property model. </param>
    /// <param name="typeUsage"> Additional details about the data type. This includes details such as maximum length, nullability etc. </param>
    protected PropertyModel(PrimitiveTypeKind type, TypeUsage typeUsage)
    {
      this._type = type;
      this._typeUsage = typeUsage;
    }

    /// <summary>Gets the data type for this property model.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
    public virtual PrimitiveTypeKind Type
    {
      get
      {
        return this._type;
      }
    }

    /// <summary>
    /// Gets additional details about the data type of this property model.
    /// This includes details such as maximum length, nullability etc.
    /// </summary>
    public TypeUsage TypeUsage
    {
      get
      {
        return this._typeUsage ?? (this._typeUsage = this.BuildTypeUsage());
      }
    }

    /// <summary>
    /// Gets or sets the name of the property model.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets a provider specific data type to use for this property model.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public virtual string StoreType { get; set; }

    /// <summary>
    /// Gets or sets the maximum length for this property model.
    /// Only valid for array data types.
    /// </summary>
    public virtual int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the precision for this property model.
    /// Only valid for decimal data types.
    /// </summary>
    public virtual byte? Precision { get; set; }

    /// <summary>
    /// Gets or sets the scale for this property model.
    /// Only valid for decimal data types.
    /// </summary>
    public virtual byte? Scale { get; set; }

    /// <summary>
    /// Gets or sets a constant value to use as the default value for this property model.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public virtual object DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets a SQL expression used as the default value for this property model.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    public virtual string DefaultValueSql { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if this property model is fixed length.
    /// Only valid for array data types.
    /// </summary>
    public virtual bool? IsFixedLength { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if this property model supports Unicode characters.
    /// Only valid for textual data types.
    /// </summary>
    public virtual bool? IsUnicode { get; set; }

    private TypeUsage BuildTypeUsage()
    {
      PrimitiveType edmPrimitiveType = PrimitiveType.GetEdmPrimitiveType(this.Type);
      if (this.Type == PrimitiveTypeKind.Binary)
      {
        if (this.MaxLength.HasValue)
        {
          PrimitiveType primitiveType = edmPrimitiveType;
          bool? isFixedLength = this.IsFixedLength;
          int num = isFixedLength.HasValue ? (isFixedLength.GetValueOrDefault() ? 1 : 0) : 0;
          int maxLength = this.MaxLength.Value;
          return TypeUsage.CreateBinaryTypeUsage(primitiveType, num != 0, maxLength);
        }
        PrimitiveType primitiveType1 = edmPrimitiveType;
        bool? isFixedLength1 = this.IsFixedLength;
        int num1 = isFixedLength1.HasValue ? (isFixedLength1.GetValueOrDefault() ? 1 : 0) : 0;
        return TypeUsage.CreateBinaryTypeUsage(primitiveType1, num1 != 0);
      }
      if (this.Type == PrimitiveTypeKind.String)
      {
        if (this.MaxLength.HasValue)
        {
          PrimitiveType primitiveType = edmPrimitiveType;
          bool? isUnicode = this.IsUnicode;
          int num1 = isUnicode.HasValue ? (isUnicode.GetValueOrDefault() ? 1 : 0) : 1;
          bool? isFixedLength = this.IsFixedLength;
          int num2 = isFixedLength.HasValue ? (isFixedLength.GetValueOrDefault() ? 1 : 0) : 0;
          int maxLength = this.MaxLength.Value;
          return TypeUsage.CreateStringTypeUsage(primitiveType, num1 != 0, num2 != 0, maxLength);
        }
        PrimitiveType primitiveType1 = edmPrimitiveType;
        bool? isUnicode1 = this.IsUnicode;
        int num3 = isUnicode1.HasValue ? (isUnicode1.GetValueOrDefault() ? 1 : 0) : 1;
        bool? isFixedLength1 = this.IsFixedLength;
        int num4 = isFixedLength1.HasValue ? (isFixedLength1.GetValueOrDefault() ? 1 : 0) : 0;
        return TypeUsage.CreateStringTypeUsage(primitiveType1, num3 != 0, num4 != 0);
      }
      if (this.Type == PrimitiveTypeKind.DateTime)
        return TypeUsage.CreateDateTimeTypeUsage(edmPrimitiveType, this.Precision);
      if (this.Type == PrimitiveTypeKind.DateTimeOffset)
        return TypeUsage.CreateDateTimeOffsetTypeUsage(edmPrimitiveType, this.Precision);
      if (this.Type == PrimitiveTypeKind.Decimal)
      {
        byte? precision1 = this.Precision;
        if (!(precision1.HasValue ? new int?((int) precision1.GetValueOrDefault()) : new int?()).HasValue)
        {
          byte? scale = this.Scale;
          if (!(scale.HasValue ? new int?((int) scale.GetValueOrDefault()) : new int?()).HasValue)
            return TypeUsage.CreateDecimalTypeUsage(edmPrimitiveType);
        }
        PrimitiveType primitiveType = edmPrimitiveType;
        byte? precision2 = this.Precision;
        int num1 = precision2.HasValue ? (int) precision2.GetValueOrDefault() : 18;
        byte? scale1 = this.Scale;
        int num2 = scale1.HasValue ? (int) scale1.GetValueOrDefault() : 0;
        return TypeUsage.CreateDecimalTypeUsage(primitiveType, (byte) num1, (byte) num2);
      }
      if (this.Type != PrimitiveTypeKind.Time)
        return TypeUsage.CreateDefaultTypeUsage((EdmType) edmPrimitiveType);
      return TypeUsage.CreateTimeTypeUsage(edmPrimitiveType, this.Precision);
    }

    internal virtual FacetValues ToFacetValues()
    {
      FacetValues facetValues = new FacetValues();
      if (this.DefaultValue != null)
        facetValues.DefaultValue = this.DefaultValue;
      if (this.IsFixedLength.HasValue)
        facetValues.FixedLength = (FacetValueContainer<bool?>) new bool?(this.IsFixedLength.Value);
      if (this.IsUnicode.HasValue)
        facetValues.Unicode = (FacetValueContainer<bool?>) new bool?(this.IsUnicode.Value);
      if (this.MaxLength.HasValue)
        facetValues.MaxLength = (FacetValueContainer<int?>) new int?(this.MaxLength.Value);
      byte? precision = this.Precision;
      if ((precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        facetValues.Precision = (FacetValueContainer<byte?>) new byte?(this.Precision.Value);
      byte? scale = this.Scale;
      if ((scale.HasValue ? new int?((int) scale.GetValueOrDefault()) : new int?()).HasValue)
        facetValues.Scale = (FacetValueContainer<byte?>) new byte?(this.Scale.Value);
      return facetValues;
    }
  }
}
