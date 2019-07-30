// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.ColumnModel
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents information about a column.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class ColumnModel : PropertyModel
  {
    private static readonly Dictionary<PrimitiveTypeKind, int> _typeSize = new Dictionary<PrimitiveTypeKind, int>()
    {
      {
        PrimitiveTypeKind.Binary,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.Boolean,
        1
      },
      {
        PrimitiveTypeKind.Byte,
        1
      },
      {
        PrimitiveTypeKind.DateTime,
        8
      },
      {
        PrimitiveTypeKind.DateTimeOffset,
        10
      },
      {
        PrimitiveTypeKind.Decimal,
        17
      },
      {
        PrimitiveTypeKind.Double,
        53
      },
      {
        PrimitiveTypeKind.Guid,
        16
      },
      {
        PrimitiveTypeKind.Int16,
        2
      },
      {
        PrimitiveTypeKind.Int32,
        4
      },
      {
        PrimitiveTypeKind.Int64,
        8
      },
      {
        PrimitiveTypeKind.SByte,
        1
      },
      {
        PrimitiveTypeKind.Single,
        4
      },
      {
        PrimitiveTypeKind.String,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.Time,
        5
      },
      {
        PrimitiveTypeKind.Geometry,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.Geography,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeometryPoint,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeometryLineString,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeometryPolygon,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeometryMultiPoint,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeometryMultiLineString,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeometryMultiPolygon,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeometryCollection,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeographyPoint,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeographyLineString,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeographyPolygon,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeographyMultiPoint,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeographyMultiLineString,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeographyMultiPolygon,
        int.MaxValue
      },
      {
        PrimitiveTypeKind.GeographyCollection,
        int.MaxValue
      }
    };
    private IDictionary<string, AnnotationValues> _annotations = (IDictionary<string, AnnotationValues>) new Dictionary<string, AnnotationValues>();
    private readonly Type _clrType;
    private readonly object _clrDefaultValue;
    private PropertyInfo _apiPropertyInfo;

    /// <summary>
    /// Initializes a new instance of the ColumnModel class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="type"> The data type for this column. </param>
    public ColumnModel(PrimitiveTypeKind type)
      : this(type, (TypeUsage) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ColumnModel class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="type"> The data type for this column. </param>
    /// <param name="typeUsage"> Additional details about the data type. This includes details such as maximum length, nullability etc. </param>
    public ColumnModel(PrimitiveTypeKind type, TypeUsage typeUsage)
      : base(type, typeUsage)
    {
      this._clrType = PrimitiveType.GetEdmPrimitiveType(type).ClrEquivalentType;
      this._clrDefaultValue = this.CreateDefaultValue();
    }

    private object CreateDefaultValue()
    {
      if (this._clrType.IsValueType())
        return Activator.CreateInstance(this._clrType);
      if (this._clrType == typeof (string))
        return (object) string.Empty;
      if (this._clrType == typeof (DbGeography))
        return (object) DbGeography.FromText("POINT(0 0)");
      if (this._clrType == typeof (DbGeometry))
        return (object) DbGeometry.FromText("POINT(0 0)");
      return (object) new byte[0];
    }

    /// <summary>
    /// Gets the CLR type corresponding to the database type of this column.
    /// </summary>
    public virtual Type ClrType
    {
      get
      {
        return this._clrType;
      }
    }

    /// <summary>
    /// Gets the default value for the CLR type corresponding to the database type of this column.
    /// </summary>
    public virtual object ClrDefaultValue
    {
      get
      {
        return this._clrDefaultValue;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating if this column can store null values.
    /// </summary>
    public virtual bool? IsNullable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if values for this column will be generated by the database using the identity pattern.
    /// </summary>
    public virtual bool IsIdentity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if this property model should be configured as a timestamp.
    /// </summary>
    public virtual bool IsTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the custom annotations that have changed on the column.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public IDictionary<string, AnnotationValues> Annotations
    {
      get
      {
        return this._annotations;
      }
      set
      {
        this._annotations = value ?? (IDictionary<string, AnnotationValues>) new Dictionary<string, AnnotationValues>();
      }
    }

    internal PropertyInfo ApiPropertyInfo
    {
      get
      {
        return this._apiPropertyInfo;
      }
      set
      {
        this._apiPropertyInfo = value;
      }
    }

    /// <summary>
    /// Determines if this column is a narrower data type than another column.
    /// Used to determine if altering the supplied column definition to this definition will result in data loss.
    /// </summary>
    /// <param name="column"> The column to compare to. </param>
    /// <param name="providerManifest"> Details of the database provider being used. </param>
    /// <returns> True if this column is of a narrower data type. </returns>
    public bool IsNarrowerThan(ColumnModel column, DbProviderManifest providerManifest)
    {
      Check.NotNull<ColumnModel>(column, nameof (column));
      Check.NotNull<DbProviderManifest>(providerManifest, nameof (providerManifest));
      TypeUsage storeType1 = providerManifest.GetStoreType(this.TypeUsage);
      TypeUsage storeType2 = providerManifest.GetStoreType(column.TypeUsage);
      if (ColumnModel._typeSize[this.Type] >= ColumnModel._typeSize[column.Type])
      {
        bool? isUnicode1 = this.IsUnicode;
        if ((isUnicode1.HasValue ? (isUnicode1.GetValueOrDefault() ? 1 : 0) : 1) == 0)
        {
          bool? isUnicode2 = column.IsUnicode;
          if ((isUnicode2.HasValue ? (isUnicode2.GetValueOrDefault() ? 1 : 0) : 1) != 0)
            goto label_6;
        }
        bool? isNullable1 = this.IsNullable;
        if ((isNullable1.HasValue ? (isNullable1.GetValueOrDefault() ? 1 : 0) : 1) == 0)
        {
          bool? isNullable2 = column.IsNullable;
          if ((isNullable2.HasValue ? (isNullable2.GetValueOrDefault() ? 1 : 0) : 1) != 0)
            goto label_6;
        }
        return ColumnModel.IsNarrowerThan(storeType1, storeType2);
      }
label_6:
      return true;
    }

    private static bool IsNarrowerThan(TypeUsage typeUsage, TypeUsage other)
    {
      string[] strArray = new string[3]
      {
        "MaxLength",
        "Precision",
        "Scale"
      };
      foreach (string identity in strArray)
      {
        Facet facet1;
        Facet facet2;
        if (typeUsage.Facets.TryGetValue(identity, true, out facet1) && other.Facets.TryGetValue(facet1.Name, true, out facet2) && (facet1.Value != facet2.Value && Convert.ToInt32(facet1.Value, (IFormatProvider) CultureInfo.InvariantCulture) < Convert.ToInt32(facet2.Value, (IFormatProvider) CultureInfo.InvariantCulture)))
          return true;
      }
      return false;
    }

    internal override FacetValues ToFacetValues()
    {
      FacetValues facetValues = base.ToFacetValues();
      if (this.IsNullable.HasValue)
        facetValues.Nullable = (FacetValueContainer<bool?>) new bool?(this.IsNullable.Value);
      if (this.IsIdentity)
        facetValues.StoreGeneratedPattern = (FacetValueContainer<StoreGeneratedPattern?>) new StoreGeneratedPattern?(StoreGeneratedPattern.Identity);
      return facetValues;
    }
  }
}
