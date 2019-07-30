// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Used to configure a primitive property of an entity type or complex type.
  /// This configuration functionality is available via lightweight conventions.
  /// </summary>
  public class ConventionPrimitivePropertyConfiguration
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> _configuration;
    private readonly Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.BinaryPropertyConfiguration> _binaryConfiguration;
    private readonly Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DateTimePropertyConfiguration> _dateTimeConfiguration;
    private readonly Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DecimalPropertyConfiguration> _decimalConfiguration;
    private readonly Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.LengthPropertyConfiguration> _lengthConfiguration;
    private readonly Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration> _stringConfiguration;

    internal ConventionPrimitivePropertyConfiguration(
      PropertyInfo propertyInfo,
      Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> configuration)
    {
      this._propertyInfo = propertyInfo;
      this._configuration = configuration;
      this._binaryConfiguration = new Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.BinaryPropertyConfiguration>((Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.BinaryPropertyConfiguration>) (() => this._configuration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.BinaryPropertyConfiguration));
      this._dateTimeConfiguration = new Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DateTimePropertyConfiguration>((Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DateTimePropertyConfiguration>) (() => this._configuration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DateTimePropertyConfiguration));
      this._decimalConfiguration = new Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DecimalPropertyConfiguration>((Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DecimalPropertyConfiguration>) (() => this._configuration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DecimalPropertyConfiguration));
      this._lengthConfiguration = new Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.LengthPropertyConfiguration>((Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.LengthPropertyConfiguration>) (() => this._configuration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.LengthPropertyConfiguration));
      this._stringConfiguration = new Lazy<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration>((Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration>) (() => this._configuration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration));
    }

    /// <summary>
    /// Gets the <see cref="T:System.Reflection.PropertyInfo" /> for this property.
    /// </summary>
    public virtual PropertyInfo ClrPropertyInfo
    {
      get
      {
        return this._propertyInfo;
      }
    }

    internal Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> Configuration
    {
      get
      {
        return this._configuration;
      }
    }

    /// <summary>
    /// Configures the name of the database column used to store the property.
    /// </summary>
    /// <param name="columnName"> The name of the column. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration HasColumnName(
      string columnName)
    {
      Check.NotEmpty(columnName, nameof (columnName));
      if (this._configuration() != null && this._configuration().ColumnName == null)
        this._configuration().ColumnName = columnName;
      return this;
    }

    /// <summary>
    /// Sets an annotation in the model for the database column used to store the property. The annotation
    /// value can later be used when processing the column such as when creating migrations.
    /// </summary>
    /// <remarks>
    /// It will likely be necessary to register a <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /> if the type of
    /// the annotation value is anything other than a string. Calling this method will have no effect if the
    /// annotation with the given name has already been configured.
    /// </remarks>
    /// <param name="name">The annotation name, which must be a valid C#/EDM identifier.</param>
    /// <param name="value">The annotation value, which may be a string or some other type that
    /// can be serialized with an <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /></param>
    /// .
    ///             <returns>The same configuration instance so that multiple calls can be chained.</returns>
    public virtual ConventionPrimitivePropertyConfiguration HasColumnAnnotation(
      string name,
      object value)
    {
      Check.NotEmpty(name, nameof (name));
      if (this._configuration() != null && !this._configuration().Annotations.ContainsKey(name))
        this._configuration().SetAnnotation(name, value);
      return this;
    }

    /// <summary>
    /// Configures the name of the parameter used in stored procedures for this property.
    /// </summary>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    public virtual ConventionPrimitivePropertyConfiguration HasParameterName(
      string parameterName)
    {
      Check.NotEmpty(parameterName, nameof (parameterName));
      if (this._configuration() != null && this._configuration().ParameterName == null)
        this._configuration().ParameterName = parameterName;
      return this;
    }

    /// <summary>
    /// Configures the order of the database column used to store the property.
    /// This method is also used to specify key ordering when an entity type has a composite key.
    /// </summary>
    /// <param name="columnOrder"> The order that this column should appear in the database table. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration HasColumnOrder(
      int columnOrder)
    {
      if (columnOrder < 0)
        throw new ArgumentOutOfRangeException(nameof (columnOrder));
      if (this._configuration() != null && !this._configuration().ColumnOrder.HasValue)
        this._configuration().ColumnOrder = new int?(columnOrder);
      return this;
    }

    /// <summary>
    /// Configures the data type of the database column used to store the property.
    /// </summary>
    /// <param name="columnType"> Name of the database provider specific data type. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration HasColumnType(
      string columnType)
    {
      Check.NotEmpty(columnType, nameof (columnType));
      if (this._configuration() != null && this._configuration().ColumnType == null)
        this._configuration().ColumnType = columnType;
      return this;
    }

    /// <summary>
    /// Configures the property to be used as an optimistic concurrency token.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsConcurrencyToken()
    {
      return this.IsConcurrencyToken(true);
    }

    /// <summary>
    /// Configures whether or not the property is to be used as an optimistic concurrency token.
    /// </summary>
    /// <param name="concurrencyToken"> Value indicating if the property is a concurrency token or not. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsConcurrencyToken(
      bool concurrencyToken)
    {
      if (this._configuration() != null && !this._configuration().ConcurrencyMode.HasValue)
        this._configuration().ConcurrencyMode = new ConcurrencyMode?(concurrencyToken ? ConcurrencyMode.Fixed : ConcurrencyMode.None);
      return this;
    }

    /// <summary>
    /// Configures how values for the property are generated by the database.
    /// </summary>
    /// <param name="databaseGeneratedOption"> The pattern used to generate values for the property in the database. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration HasDatabaseGeneratedOption(
      DatabaseGeneratedOption databaseGeneratedOption)
    {
      if (!Enum.IsDefined(typeof (DatabaseGeneratedOption), (object) databaseGeneratedOption))
        throw new ArgumentOutOfRangeException(nameof (databaseGeneratedOption));
      if (this._configuration() != null && !this._configuration().DatabaseGeneratedOption.HasValue)
        this._configuration().DatabaseGeneratedOption = new DatabaseGeneratedOption?(databaseGeneratedOption);
      return this;
    }

    /// <summary>
    /// Configures the property to be optional.
    /// The database column used to store this property will be nullable.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsOptional()
    {
      if (this._configuration() != null && !this._configuration().IsNullable.HasValue)
      {
        if (!this._propertyInfo.PropertyType.IsNullable())
          throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_NonNullableProperty((object) (this._propertyInfo.DeclaringType.ToString() + "." + this._propertyInfo.Name), (object) this._propertyInfo.PropertyType.Name));
        this._configuration().IsNullable = new bool?(true);
      }
      return this;
    }

    /// <summary>
    /// Configures the property to be required.
    /// The database column used to store this property will be non-nullable.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsRequired()
    {
      if (this._configuration() != null && !this._configuration().IsNullable.HasValue)
        this._configuration().IsNullable = new bool?(false);
      return this;
    }

    /// <summary>
    /// Configures the property to support Unicode string content.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// This method throws if the property is not a <see cref="T:System.String" />.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsUnicode()
    {
      return this.IsUnicode(true);
    }

    /// <summary>
    /// Configures whether or not the property supports Unicode string content.
    /// </summary>
    /// <param name="unicode"> Value indicating if the property supports Unicode string content or not. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// This method throws if the property is not a <see cref="T:System.String" />.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsUnicode(
      bool unicode)
    {
      if (this._configuration() != null)
      {
        if (this._stringConfiguration.Value == null)
          throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_IsUnicodeNonString((object) this._propertyInfo.Name));
        if (!this._stringConfiguration.Value.IsUnicode.HasValue)
          this._stringConfiguration.Value.IsUnicode = new bool?(unicode);
      }
      return this;
    }

    /// <summary>
    /// Configures the property to be fixed length.
    /// Use HasMaxLength to set the length that the property is fixed to.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// This method throws if the property does not have length facets.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsFixedLength()
    {
      if (this._configuration() != null)
      {
        if (this._lengthConfiguration.Value == null)
          throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_NonLength((object) this._propertyInfo.Name));
        if (!this._lengthConfiguration.Value.IsFixedLength.HasValue)
          this._lengthConfiguration.Value.IsFixedLength = new bool?(true);
      }
      return this;
    }

    /// <summary>
    /// Configures the property to be variable length.
    /// Properties are variable length by default.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// This method throws if the property does not have length facets.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsVariableLength()
    {
      if (this._configuration() != null)
      {
        if (this._lengthConfiguration.Value == null)
          throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_NonLength((object) this._propertyInfo.Name));
        if (!this._lengthConfiguration.Value.IsFixedLength.HasValue)
          this._lengthConfiguration.Value.IsFixedLength = new bool?(false);
      }
      return this;
    }

    /// <summary>
    /// Configures the property to have the specified maximum length.
    /// </summary>
    /// <param name="maxLength"> The maximum length for the property. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// This method throws if the property does not have length facets.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration HasMaxLength(
      int maxLength)
    {
      if (maxLength < 1)
        throw new ArgumentOutOfRangeException(nameof (maxLength));
      if (this._configuration() != null)
      {
        if (this._lengthConfiguration.Value == null)
          throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_NonLength((object) this._propertyInfo.Name));
        if (!this._lengthConfiguration.Value.MaxLength.HasValue && !this._lengthConfiguration.Value.IsMaxLength.HasValue)
        {
          this._lengthConfiguration.Value.MaxLength = new int?(maxLength);
          if (!this._lengthConfiguration.Value.IsFixedLength.HasValue)
            this._lengthConfiguration.Value.IsFixedLength = new bool?(false);
        }
      }
      return this;
    }

    /// <summary>
    /// Configures the property to allow the maximum length supported by the database provider.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// This method throws if the property does not have length facets.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsMaxLength()
    {
      if (this._configuration() != null)
      {
        if (this._lengthConfiguration.Value == null)
          throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_NonLength((object) this._propertyInfo.Name));
        if (!this._lengthConfiguration.Value.IsMaxLength.HasValue && !this._lengthConfiguration.Value.MaxLength.HasValue)
          this._lengthConfiguration.Value.IsMaxLength = new bool?(true);
      }
      return this;
    }

    /// <summary>
    /// Configures the precision of the <see cref="T:System.DateTime" /> property.
    /// If the database provider does not support precision for the data type of the column then the value is ignored.
    /// </summary>
    /// <param name="value"> Precision of the property. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// This method will throw if the property is not a <see cref="T:System.DateTime" />.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration HasPrecision(
      byte value)
    {
      if (this._configuration() != null)
      {
        if (this._dateTimeConfiguration.Value == null)
        {
          if (this._decimalConfiguration.Value != null)
            throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_DecimalNoScale((object) this._propertyInfo.Name));
          throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_HasPrecisionNonDateTime((object) this._propertyInfo.Name));
        }
        byte? precision = this._dateTimeConfiguration.Value.Precision;
        if (!(precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
          this._dateTimeConfiguration.Value.Precision = new byte?(value);
      }
      return this;
    }

    /// <summary>
    /// Configures the precision and scale of the <see cref="T:System.Decimal" /> property.
    /// </summary>
    /// <param name="precision"> The precision of the property. </param>
    /// <param name="scale"> The scale of the property. </param>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// This method will throw if the property is not a <see cref="T:System.Decimal" />.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration HasPrecision(
      byte precision,
      byte scale)
    {
      if (this._configuration() != null)
      {
        if (this._decimalConfiguration.Value == null)
        {
          if (this._dateTimeConfiguration.Value != null)
            throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_DateTimeScale((object) this._propertyInfo.Name));
          throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_HasPrecisionNonDecimal((object) this._propertyInfo.Name));
        }
        byte? precision1 = this._decimalConfiguration.Value.Precision;
        if (!(precision1.HasValue ? new int?((int) precision1.GetValueOrDefault()) : new int?()).HasValue)
        {
          byte? scale1 = this._decimalConfiguration.Value.Scale;
          if (!(scale1.HasValue ? new int?((int) scale1.GetValueOrDefault()) : new int?()).HasValue)
          {
            this._decimalConfiguration.Value.Precision = new byte?(precision);
            this._decimalConfiguration.Value.Scale = new byte?(scale);
          }
        }
      }
      return this;
    }

    /// <summary>
    /// Configures the property to be a row version in the database.
    /// The actual data type will vary depending on the database provider being used.
    /// Setting the property to be a row version will automatically configure it to be an
    /// optimistic concurrency token.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// Calling this will have no effect once it has been configured.
    /// This method throws if the property is not a <see cref="T:Byte[]" />.
    /// </remarks>
    public virtual ConventionPrimitivePropertyConfiguration IsRowVersion()
    {
      if (this._configuration() != null)
      {
        if (this._binaryConfiguration.Value == null)
          throw new InvalidOperationException(Strings.LightweightPrimitivePropertyConfiguration_IsRowVersionNonBinary((object) this._propertyInfo.Name));
        if (!this._binaryConfiguration.Value.IsRowVersion.HasValue)
          this._binaryConfiguration.Value.IsRowVersion = new bool?(true);
      }
      return this;
    }

    /// <summary>
    /// Configures this property to be part of the entity type's primary key.
    /// </summary>
    /// <returns>
    /// The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" /> instance so that
    /// multiple calls can be chained.
    /// </returns>
    public virtual ConventionPrimitivePropertyConfiguration IsKey()
    {
      if (this._configuration() != null)
      {
        EntityTypeConfiguration typeConfiguration = this._configuration().TypeConfiguration as EntityTypeConfiguration;
        if (typeConfiguration != null && !typeConfiguration.IsKeyConfigured)
          typeConfiguration.Key(this.ClrPropertyInfo);
      }
      return this;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the current instance.
    /// </summary>
    /// <returns>The exact runtime type of the current instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
