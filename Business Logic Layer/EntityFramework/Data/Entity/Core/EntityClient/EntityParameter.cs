// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.EntityParameter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Core.EntityClient
{
  /// <summary>Class representing a parameter used in EntityCommand</summary>
  public class EntityParameter : DbParameter, IDbDataParameter, IDataParameter
  {
    private string _parameterName;
    private DbType? _dbType;
    private EdmType _edmType;
    private byte? _precision;
    private byte? _scale;
    private bool _isDirty;
    private object _value;
    private object _parent;
    private ParameterDirection _direction;
    private int? _size;
    private string _sourceColumn;
    private DataRowVersion _sourceVersion;
    private bool _sourceColumnNullMapping;
    private bool? _isNullable;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> class using the default values.
    /// </summary>
    public EntityParameter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> class using the specified parameter name and data type.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">
    /// One of the <see cref="T:System.Data.DbType" /> values.
    /// </param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EntityParameter(string parameterName, DbType dbType)
    {
      this.SetParameterNameWithValidation(parameterName, nameof (parameterName));
      this.DbType = dbType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> class using the specified parameter name, data type and size.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">
    /// One of the <see cref="T:System.Data.DbType" /> values.
    /// </param>
    /// <param name="size">The size of the parameter.</param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EntityParameter(string parameterName, DbType dbType, int size)
    {
      this.SetParameterNameWithValidation(parameterName, nameof (parameterName));
      this.DbType = dbType;
      this.Size = size;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> class using the specified properties.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">
    /// One of the <see cref="T:System.Data.DbType" /> values.
    /// </param>
    /// <param name="size">The size of the parameter.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EntityParameter(string parameterName, DbType dbType, int size, string sourceColumn)
    {
      this.SetParameterNameWithValidation(parameterName, nameof (parameterName));
      this.DbType = dbType;
      this.Size = size;
      this.SourceColumn = sourceColumn;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" /> class using the specified properties.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">
    /// One of the <see cref="T:System.Data.DbType" /> values.
    /// </param>
    /// <param name="size">The size of the parameter.</param>
    /// <param name="direction">
    /// One of the <see cref="T:System.Data.ParameterDirection" /> values.
    /// </param>
    /// <param name="isNullable">true to indicate that the parameter accepts null values; otherwise, false.</param>
    /// <param name="precision">The number of digits used to represent the value.</param>
    /// <param name="scale">The number of decimal places to which value is resolved.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    /// <param name="sourceVersion">
    /// One of the <see cref="T:System.Data.DataRowVersion" /> values.
    /// </param>
    /// <param name="value">The value of the parameter.</param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EntityParameter(
      string parameterName,
      DbType dbType,
      int size,
      ParameterDirection direction,
      bool isNullable,
      byte precision,
      byte scale,
      string sourceColumn,
      DataRowVersion sourceVersion,
      object value)
    {
      this.SetParameterNameWithValidation(parameterName, nameof (parameterName));
      this.DbType = dbType;
      this.Size = size;
      this.Direction = direction;
      this.IsNullable = isNullable;
      this.Precision = precision;
      this.Scale = scale;
      this.SourceColumn = sourceColumn;
      this.SourceVersion = sourceVersion;
      this.Value = value;
    }

    private EntityParameter(EntityParameter source)
      : this()
    {
      source.CloneHelper(this);
      ICloneable cloneable = this._value as ICloneable;
      if (cloneable == null)
        return;
      this._value = cloneable.Clone();
    }

    /// <summary>Gets or sets the name of the entity parameter.</summary>
    /// <returns>The name of the entity parameter.</returns>
    public override string ParameterName
    {
      get
      {
        return this._parameterName ?? "";
      }
      set
      {
        this.SetParameterNameWithValidation(value, nameof (value));
      }
    }

    private void SetParameterNameWithValidation(string parameterName, string argumentName)
    {
      if (!string.IsNullOrEmpty(parameterName) && !DbCommandTree.IsValidParameterName(parameterName))
        throw new ArgumentException(Strings.EntityClient_InvalidParameterName((object) parameterName), argumentName);
      this.PropertyChanging();
      this._parameterName = parameterName;
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Data.DbType" /> of the parameter.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Data.DbType" /> values.
    /// </returns>
    public override DbType DbType
    {
      get
      {
        if (this._dbType.HasValue)
          return this._dbType.Value;
        if (this._edmType != null)
          return EntityParameter.GetDbTypeFromEdm(this._edmType);
        if (this._value == null)
          return DbType.String;
        try
        {
          return TypeHelpers.ConvertClrTypeToDbType(this._value.GetType());
        }
        catch (ArgumentException ex)
        {
          throw new InvalidOperationException(Strings.EntityClient_CannotDeduceDbType, (Exception) ex);
        }
      }
      set
      {
        this.PropertyChanging();
        this._dbType = new DbType?(value);
      }
    }

    /// <summary>Gets or sets the type of the parameter, expressed as an EdmType.</summary>
    /// <returns>The type of the parameter, expressed as an EdmType.</returns>
    public virtual EdmType EdmType
    {
      get
      {
        return this._edmType;
      }
      set
      {
        if (value != null && !Helper.IsScalarType(value))
          throw new InvalidOperationException(Strings.EntityClient_EntityParameterEdmTypeNotScalar((object) value.FullName));
        this.PropertyChanging();
        this._edmType = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of digits used to represent the
    /// <see cref="P:System.Data.Entity.Core.EntityClient.EntityParameter.Value" />
    /// property.
    /// </summary>
    /// <returns>The number of digits used to represent the value.</returns>
    public virtual byte Precision
    {
      get
      {
        return this._precision.HasValue ? this._precision.Value : (byte) 0;
      }
      set
      {
        this.PropertyChanging();
        this._precision = new byte?(value);
      }
    }

    /// <summary>
    /// Gets or sets the number of decimal places to which
    /// <see cref="P:System.Data.Entity.Core.EntityClient.EntityParameter.Value" />
    /// is resolved.
    /// </summary>
    /// <returns>The number of decimal places to which value is resolved.</returns>
    public virtual byte Scale
    {
      get
      {
        return this._scale.HasValue ? this._scale.Value : (byte) 0;
      }
      set
      {
        this.PropertyChanging();
        this._scale = new byte?(value);
      }
    }

    /// <summary>Gets or sets the value of the parameter.</summary>
    /// <returns>The value of the parameter.</returns>
    public override object Value
    {
      get
      {
        return this._value;
      }
      set
      {
        if (!this._dbType.HasValue && this._edmType == null)
        {
          DbType dbType1 = DbType.String;
          if (this._value != null)
            dbType1 = TypeHelpers.ConvertClrTypeToDbType(this._value.GetType());
          DbType dbType2 = DbType.String;
          if (value != null)
            dbType2 = TypeHelpers.ConvertClrTypeToDbType(value.GetType());
          if (dbType1 != dbType2)
            this.PropertyChanging();
        }
        this._value = value;
      }
    }

    internal virtual bool IsDirty
    {
      get
      {
        return this._isDirty;
      }
    }

    internal virtual bool IsDbTypeSpecified
    {
      get
      {
        return this._dbType.HasValue;
      }
    }

    internal virtual bool IsDirectionSpecified
    {
      get
      {
        return this._direction != (ParameterDirection) 0;
      }
    }

    internal virtual bool IsIsNullableSpecified
    {
      get
      {
        return this._isNullable.HasValue;
      }
    }

    internal virtual bool IsPrecisionSpecified
    {
      get
      {
        return this._precision.HasValue;
      }
    }

    internal virtual bool IsScaleSpecified
    {
      get
      {
        return this._scale.HasValue;
      }
    }

    internal virtual bool IsSizeSpecified
    {
      get
      {
        return this._size.HasValue;
      }
    }

    /// <summary>Gets or sets the direction of the parameter.</summary>
    /// <returns>
    /// One of the <see cref="T:System.Data.ParameterDirection" /> values.
    /// </returns>
    [EntityResCategory("DataCategory_Data")]
    [RefreshProperties(RefreshProperties.All)]
    [EntityResDescription("DbParameter_Direction")]
    public override ParameterDirection Direction
    {
      get
      {
        ParameterDirection direction = this._direction;
        if (direction == (ParameterDirection) 0)
          return ParameterDirection.Input;
        return direction;
      }
      set
      {
        if (this._direction == value)
          return;
        switch (value)
        {
          case ParameterDirection.Input:
          case ParameterDirection.Output:
          case ParameterDirection.InputOutput:
          case ParameterDirection.ReturnValue:
            this.PropertyChanging();
            this._direction = value;
            break;
          default:
            throw new ArgumentOutOfRangeException(typeof (ParameterDirection).Name, Strings.ADP_InvalidEnumerationValue((object) typeof (ParameterDirection).Name, (object) ((int) value).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        }
      }
    }

    /// <summary>Gets or sets a value that indicates whether the parameter accepts null values.</summary>
    /// <returns>true if null values are accepted; otherwise, false.</returns>
    public override bool IsNullable
    {
      get
      {
        return !this._isNullable.HasValue || this._isNullable.Value;
      }
      set
      {
        this._isNullable = new bool?(value);
      }
    }

    /// <summary>Gets or sets the maximum size of the data within the column.</summary>
    /// <returns>The maximum size of the data within the column.</returns>
    [EntityResDescription("DbParameter_Size")]
    [EntityResCategory("DataCategory_Data")]
    public override int Size
    {
      get
      {
        int num = this._size.HasValue ? this._size.Value : 0;
        if (num == 0)
          num = EntityParameter.ValueSize(this.Value);
        return num;
      }
      set
      {
        if (this._size.HasValue && this._size.Value == value)
          return;
        if (value < -1)
          throw new ArgumentException(Strings.ADP_InvalidSizeValue((object) value.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        this.PropertyChanging();
        if (value == 0)
          this._size = new int?();
        else
          this._size = new int?(value);
      }
    }

    /// <summary>
    /// Gets or sets the name of the source column mapped to the <see cref="T:System.Data.DataSet" /> and used for loading or returning the
    /// <see cref="P:System.Data.Entity.Core.EntityClient.EntityParameter.Value" />
    /// .
    /// </summary>
    /// <returns>The name of the source column mapped to the dataset and used for loading or returning the value.</returns>
    [EntityResCategory("DataCategory_Update")]
    [EntityResDescription("DbParameter_SourceColumn")]
    public override string SourceColumn
    {
      get
      {
        return this._sourceColumn ?? string.Empty;
      }
      set
      {
        this._sourceColumn = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether source column is nullable.</summary>
    /// <returns>true if source column is nullable; otherwise, false.</returns>
    public override bool SourceColumnNullMapping
    {
      get
      {
        return this._sourceColumnNullMapping;
      }
      set
      {
        this._sourceColumnNullMapping = value;
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Data.DataRowVersion" /> to use when loading the value.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Data.DataRowVersion" /> values.
    /// </returns>
    [EntityResDescription("DbParameter_SourceVersion")]
    [EntityResCategory("DataCategory_Update")]
    public override DataRowVersion SourceVersion
    {
      get
      {
        DataRowVersion sourceVersion = this._sourceVersion;
        if (sourceVersion == (DataRowVersion) 0)
          return DataRowVersion.Current;
        return sourceVersion;
      }
      set
      {
        switch (value)
        {
          case DataRowVersion.Original:
          case DataRowVersion.Current:
          case DataRowVersion.Proposed:
          case DataRowVersion.Default:
            this._sourceVersion = value;
            break;
          default:
            throw new ArgumentOutOfRangeException(typeof (DataRowVersion).Name, Strings.ADP_InvalidEnumerationValue((object) typeof (DataRowVersion).Name, (object) ((int) value).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        }
      }
    }

    /// <summary>
    /// Resets the type associated with the <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" />.
    /// </summary>
    public override void ResetDbType()
    {
      if (this._dbType.HasValue || this._edmType != null)
        this.PropertyChanging();
      this._edmType = (EdmType) null;
      this._dbType = new DbType?();
    }

    private void PropertyChanging()
    {
      this._isDirty = true;
    }

    private static int ValueSize(object value)
    {
      return EntityParameter.ValueSizeCore(value);
    }

    internal virtual EntityParameter Clone()
    {
      return new EntityParameter(this);
    }

    private void CloneHelper(EntityParameter destination)
    {
      destination._value = this._value;
      destination._direction = this._direction;
      destination._size = this._size;
      destination._sourceColumn = this._sourceColumn;
      destination._sourceVersion = this._sourceVersion;
      destination._sourceColumnNullMapping = this._sourceColumnNullMapping;
      destination._isNullable = this._isNullable;
      destination._parameterName = this._parameterName;
      destination._dbType = this._dbType;
      destination._edmType = this._edmType;
      destination._precision = this._precision;
      destination._scale = this._scale;
    }

    internal virtual TypeUsage GetTypeUsage()
    {
      if (!this.IsTypeConsistent)
        throw new InvalidOperationException(Strings.EntityClient_EntityParameterInconsistentEdmType((object) this._edmType.FullName, (object) this._parameterName));
      TypeUsage modelType;
      if (this._edmType != null)
        modelType = TypeUsage.Create(this._edmType);
      else if (!DbTypeMap.TryGetModelTypeUsage(this.DbType, out modelType))
      {
        PrimitiveType primitiveType;
        if (this.DbType != DbType.Object || this.Value == null || (!ClrProviderManifest.Instance.TryGetPrimitiveType(this.Value.GetType(), out primitiveType) || !Helper.IsSpatialType(primitiveType)))
          throw new InvalidOperationException(Strings.EntityClient_UnsupportedDbType((object) this.DbType.ToString(), (object) this.ParameterName));
        modelType = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(primitiveType.PrimitiveTypeKind);
      }
      return modelType;
    }

    internal virtual void ResetIsDirty()
    {
      this._isDirty = false;
    }

    private bool IsTypeConsistent
    {
      get
      {
        if (this._edmType == null || !this._dbType.HasValue)
          return true;
        DbType dbTypeFromEdm = EntityParameter.GetDbTypeFromEdm(this._edmType);
        if (dbTypeFromEdm == DbType.String)
        {
          DbType? dbType1 = this._dbType;
          if ((dbType1.GetValueOrDefault() != DbType.String ? 0 : (dbType1.HasValue ? 1 : 0)) == 0)
          {
            DbType? dbType2 = this._dbType;
            if ((dbType2.GetValueOrDefault() != DbType.AnsiString ? 0 : (dbType2.HasValue ? 1 : 0)) == 0 && dbTypeFromEdm != DbType.AnsiStringFixedLength)
              return dbTypeFromEdm == DbType.StringFixedLength;
          }
          return true;
        }
        DbType? dbType3 = this._dbType;
        DbType dbType4 = dbTypeFromEdm;
        if (dbType3.GetValueOrDefault() == dbType4)
          return dbType3.HasValue;
        return false;
      }
    }

    private static DbType GetDbTypeFromEdm(EdmType edmType)
    {
      PrimitiveType type = Helper.AsPrimitive(edmType);
      if (Helper.IsSpatialType(type))
        return DbType.Object;
      DbType dbType;
      if (DbCommandDefinition.TryGetDbTypeFromPrimitiveType(type, out dbType))
        return dbType;
      return DbType.AnsiString;
    }

    private void ResetSize()
    {
      if (!this._size.HasValue)
        return;
      this.PropertyChanging();
      this._size = new int?();
    }

    private bool ShouldSerializeSize()
    {
      if (this._size.HasValue)
        return this._size.Value != 0;
      return false;
    }

    internal virtual void CopyTo(DbParameter destination)
    {
      this.CloneHelper((EntityParameter) destination);
    }

    internal virtual object CompareExchangeParent(object value, object comparand)
    {
      object parent = this._parent;
      if (comparand == parent)
        this._parent = value;
      return parent;
    }

    internal virtual void ResetParent()
    {
      this._parent = (object) null;
    }

    /// <summary>Returns a string representation of the parameter.</summary>
    /// <returns>A string representation of the parameter.</returns>
    public override string ToString()
    {
      return this.ParameterName;
    }

    private static int ValueSizeCore(object value)
    {
      if (!EntityUtil.IsNull(value))
      {
        string str = value as string;
        if (str != null)
          return str.Length;
        byte[] numArray = value as byte[];
        if (numArray != null)
          return numArray.Length;
        char[] chArray = value as char[];
        if (chArray != null)
          return chArray.Length;
        if (value is byte || value is char)
          return 1;
      }
      return 0;
    }
  }
}
