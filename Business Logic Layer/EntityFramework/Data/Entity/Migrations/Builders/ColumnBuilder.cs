// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Builders.ColumnBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Spatial;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Migrations.Builders
{
  /// <summary>
  /// Helper class that is used to configure a column.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class ColumnBuilder
  {
    /// <summary>
    /// Creates a new column definition to store Binary data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="maxLength"> The maximum allowable length of the array data. </param>
    /// <param name="fixedLength"> Value indicating whether or not all data should be padded to the maximum length. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="timestamp"> Value indicating whether or not this column should be configured as a timestamp. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ColumnModel Binary(
      bool? nullable = null,
      int? maxLength = null,
      bool? fixedLength = null,
      byte[] defaultValue = null,
      string defaultValueSql = null,
      bool timestamp = false,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Binary, nullable, (object) defaultValue, defaultValueSql, maxLength, new byte?(), new byte?(), new bool?(), fixedLength, false, timestamp, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store Boolean data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ColumnModel Boolean(
      bool? nullable = null,
      bool? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Boolean, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), false, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store Byte data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="identity"> Value indicating whether or not the database will generate values for this column during insert. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ColumnModel Byte(
      bool? nullable = null,
      bool identity = false,
      byte? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Byte, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), identity, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store DateTime data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="precision"> The precision of the column. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ColumnModel DateTime(
      bool? nullable = null,
      byte? precision = null,
      DateTime? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.DateTime, nullable, (object) defaultValue, defaultValueSql, new int?(), precision, new byte?(), new bool?(), new bool?(), false, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store Decimal data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="precision"> The numeric precision of the column. </param>
    /// <param name="scale"> The numeric scale of the column. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="identity"> Value indicating whether or not the database will generate values for this column during insert. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ColumnModel Decimal(
      bool? nullable = null,
      byte? precision = null,
      byte? scale = null,
      Decimal? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      bool identity = false,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      bool? nullable1 = nullable;
      // ISSUE: variable of a boxed type
      __Boxed<Decimal?> local = (System.ValueType) defaultValue;
      string defaultValueSql1 = defaultValueSql;
      int? maxLength = new int?();
      byte? precision1 = precision;
      string str1 = name;
      string str2 = storeType;
      byte? scale1 = scale;
      bool? unicode = new bool?();
      bool? fixedLength = new bool?();
      int num = identity ? 1 : 0;
      string name1 = str1;
      string storeType1 = str2;
      IDictionary<string, AnnotationValues> annotations1 = annotations;
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Decimal, nullable1, (object) local, defaultValueSql1, maxLength, precision1, scale1, unicode, fixedLength, num != 0, false, name1, storeType1, annotations1);
    }

    /// <summary>
    /// Creates a new column definition to store Double data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ColumnModel Double(
      bool? nullable = null,
      double? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Double, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), false, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store GUID data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="identity"> Value indicating whether or not the database will generate values for this column during insert. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ColumnModel Guid(
      bool? nullable = null,
      bool identity = false,
      Guid? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Guid, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), identity, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store Single data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ColumnModel Single(
      bool? nullable = null,
      float? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Single, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), false, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store Short data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="identity"> Value indicating whether or not the database will generate values for this column during insert. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ColumnModel Short(
      bool? nullable = null,
      bool identity = false,
      short? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Int16, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), identity, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store Integer data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="identity"> Value indicating whether or not the database will generate values for this column during insert. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ColumnModel Int(
      bool? nullable = null,
      bool identity = false,
      int? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Int32, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), identity, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store Long data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="identity"> Value indicating whether or not the database will generate values for this column during insert. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ColumnModel Long(
      bool? nullable = null,
      bool identity = false,
      long? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Int64, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), identity, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store String data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="maxLength"> The maximum allowable length of the string data. </param>
    /// <param name="fixedLength"> Value indicating whether or not all data should be padded to the maximum length. </param>
    /// <param name="unicode"> Value indicating whether or not the column supports Unicode content. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ColumnModel String(
      bool? nullable = null,
      int? maxLength = null,
      bool? fixedLength = null,
      bool? unicode = null,
      string defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      bool? nullable1 = nullable;
      string str = defaultValue;
      string defaultValueSql1 = defaultValueSql;
      int? maxLength1 = maxLength;
      bool? nullable2 = fixedLength;
      byte? precision = new byte?();
      byte? scale = new byte?();
      bool? unicode1 = unicode;
      bool? fixedLength1 = nullable2;
      string name1 = name;
      string storeType1 = storeType;
      IDictionary<string, AnnotationValues> annotations1 = annotations;
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.String, nullable1, (object) str, defaultValueSql1, maxLength1, precision, scale, unicode1, fixedLength1, false, false, name1, storeType1, annotations1);
    }

    /// <summary>
    /// Creates a new column definition to store Time data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="precision"> The precision of the column. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ColumnModel Time(
      bool? nullable = null,
      byte? precision = null,
      TimeSpan? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Time, nullable, (object) defaultValue, defaultValueSql, new int?(), precision, new byte?(), new bool?(), new bool?(), false, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store DateTimeOffset data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="precision"> The precision of the column. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ColumnModel DateTimeOffset(
      bool? nullable = null,
      byte? precision = null,
      DateTimeOffset? defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.DateTimeOffset, nullable, (object) defaultValue, defaultValueSql, new int?(), precision, new byte?(), new bool?(), new bool?(), false, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store geography data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ColumnModel Geography(
      bool? nullable = null,
      DbGeography defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Geography, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), false, false, name, storeType, annotations);
    }

    /// <summary>
    /// Creates a new column definition to store geometry data.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="nullable"> Value indicating whether or not the column allows null values. </param>
    /// <param name="defaultValue"> Constant value to use as the default value for this column. </param>
    /// <param name="defaultValueSql"> SQL expression used as the default value for this column. </param>
    /// <param name="name"> The name of the column. </param>
    /// <param name="storeType"> Provider specific data type to use for this column. </param>
    /// <param name="annotations"> Custom annotations usually from the Code First model. </param>
    /// <returns> The newly constructed column definition. </returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public ColumnModel Geometry(
      bool? nullable = null,
      DbGeometry defaultValue = null,
      string defaultValueSql = null,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      return ColumnBuilder.BuildColumn(PrimitiveTypeKind.Geometry, nullable, (object) defaultValue, defaultValueSql, new int?(), new byte?(), new byte?(), new bool?(), new bool?(), false, false, name, storeType, annotations);
    }

    private static ColumnModel BuildColumn(
      PrimitiveTypeKind primitiveTypeKind,
      bool? nullable,
      object defaultValue,
      string defaultValueSql = null,
      int? maxLength = null,
      byte? precision = null,
      byte? scale = null,
      bool? unicode = null,
      bool? fixedLength = null,
      bool identity = false,
      bool timestamp = false,
      string name = null,
      string storeType = null,
      IDictionary<string, AnnotationValues> annotations = null)
    {
      ColumnModel columnModel = new ColumnModel(primitiveTypeKind);
      columnModel.IsNullable = nullable;
      columnModel.MaxLength = maxLength;
      columnModel.Precision = precision;
      columnModel.Scale = scale;
      columnModel.IsUnicode = unicode;
      columnModel.IsFixedLength = fixedLength;
      columnModel.IsIdentity = identity;
      columnModel.DefaultValue = defaultValue;
      columnModel.DefaultValueSql = defaultValueSql;
      columnModel.IsTimestamp = timestamp;
      columnModel.Name = name;
      columnModel.StoreType = storeType;
      columnModel.Annotations = annotations;
      return columnModel;
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
