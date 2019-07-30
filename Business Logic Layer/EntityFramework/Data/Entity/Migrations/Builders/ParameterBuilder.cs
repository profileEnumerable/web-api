// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Builders.ParameterBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Spatial;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Builders
{
  /// <summary>
  /// Helper class that is used to configure a parameter.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class ParameterBuilder
  {
    /// <summary>
    /// Creates a new parameter definition to pass Binary data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="maxLength"> The maximum allowable length of the array data. </param>
    /// <param name="fixedLength"> Value indicating whether or not all data should be padded to the maximum length. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ParameterModel Binary(
      int? maxLength = null,
      bool? fixedLength = null,
      byte[] defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Binary, (object) defaultValue, defaultValueSql, maxLength, new byte?(), new byte?(), new bool?(), fixedLength, name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass Boolean data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ParameterModel Boolean(
      bool? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Boolean, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass Byte data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ParameterModel Byte(
      byte? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Byte, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass DateTime data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="precision"> The precision of the parameter. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ParameterModel DateTime(
      byte? precision = null,
      DateTime? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.DateTime, (object) defaultValue, defaultValueSql, new int?(), precision, new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass Decimal data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="precision"> The numeric precision of the parameter. </param>
    /// <param name="scale"> The numeric scale of the parameter. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ParameterModel Decimal(
      byte? precision = null,
      byte? scale = null,
      Decimal? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Decimal, (object) defaultValue, defaultValueSql, new int?(), precision, scale, new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass Double data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ParameterModel Double(
      double? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Double, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass GUID data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ParameterModel Guid(
      Guid? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Guid, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass Single data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ParameterModel Single(
      float? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Single, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass Short data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ParameterModel Short(
      short? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Int16, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass Integer data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ParameterModel Int(
      int? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Int32, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass Long data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ParameterModel Long(
      long? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Int64, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass String data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="maxLength"> The maximum allowable length of the string data. </param>
    /// <param name="fixedLength"> Value indicating whether or not all data should be padded to the maximum length. </param>
    /// <param name="unicode"> Value indicating whether or not the parameter supports Unicode content. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ParameterModel String(
      int? maxLength = null,
      bool? fixedLength = null,
      bool? unicode = null,
      string defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      string str = defaultValue;
      string defaultValueSql1 = defaultValueSql;
      int? maxLength1 = maxLength;
      bool? nullable = fixedLength;
      byte? precision = new byte?();
      byte? scale = new byte?();
      bool? unicode1 = unicode;
      bool? fixedLength1 = nullable;
      string name1 = name;
      string storeType1 = storeType;
      int num = outParameter ? 1 : 0;
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.String, (object) str, defaultValueSql1, maxLength1, precision, scale, unicode1, fixedLength1, name1, storeType1, num != 0);
    }

    /// <summary>
    /// Creates a new parameter definition to pass Time data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="precision"> The precision of the parameter. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ParameterModel Time(
      byte? precision = null,
      TimeSpan? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Time, (object) defaultValue, defaultValueSql, new int?(), precision, new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass DateTimeOffset data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="precision"> The precision of the parameter. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ParameterModel DateTimeOffset(
      byte? precision = null,
      DateTimeOffset? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.DateTimeOffset, (object) defaultValue, defaultValueSql, new int?(), precision, new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass geography data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ParameterModel Geography(
      DbGeography defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Geography, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    /// <summary>
    /// Creates a new parameter definition to pass geometry data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="defaultValue"> Constant value to use as the default value for this parameter. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this parameter. </param>
    /// <param name="name"> The name of the parameter. </param>
    /// <param name="storeType"> Provider specific data type to use for this parameter. </param>
    /// <param name="outParameter">A value indicating whether the parameter is an output parameter.</param>
    /// <returns> The newly constructed parameter definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ParameterModel Geometry(
      DbGeometry defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      return ParameterBuilder.BuildParameter(PrimitiveTypeKind.Geometry, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), name, storeType, outParameter);
    }

    private static ParameterModel BuildParameter(
      PrimitiveTypeKind primitiveTypeKind,
      object defaultValue,
      string defaultValueSql = null,
      int? maxLength = null,
      byte? precision = null,
      byte? scale = null,
      bool? unicode = null,
      bool? fixedLength = null,
      string name = null,
      string storeType = null,
      bool outParameter = false)
    {
      ParameterModel parameterModel = new ParameterModel(primitiveTypeKind);
      parameterModel.MaxLength = maxLength;
      parameterModel.Precision = precision;
      parameterModel.Scale = scale;
      parameterModel.IsUnicode = unicode;
      parameterModel.IsFixedLength = fixedLength;
      parameterModel.DefaultValue = defaultValue;
      parameterModel.DefaultValueSql = defaultValueSql;
      parameterModel.Name = name;
      parameterModel.StoreType = storeType;
      parameterModel.IsOutParameter = outParameter;
      return parameterModel;
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }

    /// <summary>
    /// Creates a shallow copy of the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>A shallow copy of the current <see cref="T:System.Object" />.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new object MemberwiseClone()
    {
      return base.MemberwiseClone();
    }
  }
}
