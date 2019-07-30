// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.FacetValues
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class FacetValues
  {
    private FacetValueContainer<bool?> _nullable;
    private FacetValueContainer<int?> _maxLength;
    private FacetValueContainer<bool?> _unicode;
    private FacetValueContainer<bool?> _fixedLength;
    private FacetValueContainer<byte?> _precision;
    private FacetValueContainer<byte?> _scale;
    private object _defaultValue;
    private FacetValueContainer<string> _collation;
    private FacetValueContainer<int?> _srid;
    private FacetValueContainer<bool?> _isStrict;
    private FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern?> _storeGeneratedPattern;
    private FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.ConcurrencyMode?> _concurrencyMode;
    private FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.CollectionKind?> _collectionKind;

    internal FacetValueContainer<bool?> Nullable
    {
      set
      {
        this._nullable = value;
      }
    }

    internal FacetValueContainer<int?> MaxLength
    {
      set
      {
        this._maxLength = value;
      }
    }

    internal FacetValueContainer<bool?> Unicode
    {
      set
      {
        this._unicode = value;
      }
    }

    internal FacetValueContainer<bool?> FixedLength
    {
      set
      {
        this._fixedLength = value;
      }
    }

    internal FacetValueContainer<byte?> Precision
    {
      set
      {
        this._precision = value;
      }
    }

    internal FacetValueContainer<byte?> Scale
    {
      set
      {
        this._scale = value;
      }
    }

    internal object DefaultValue
    {
      set
      {
        this._defaultValue = value;
      }
    }

    internal FacetValueContainer<string> Collation
    {
      set
      {
        this._collation = value;
      }
    }

    internal FacetValueContainer<int?> Srid
    {
      set
      {
        this._srid = value;
      }
    }

    internal FacetValueContainer<bool?> IsStrict
    {
      set
      {
        this._isStrict = value;
      }
    }

    internal FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern?> StoreGeneratedPattern
    {
      set
      {
        this._storeGeneratedPattern = value;
      }
    }

    internal FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.ConcurrencyMode?> ConcurrencyMode
    {
      set
      {
        this._concurrencyMode = value;
      }
    }

    internal FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.CollectionKind?> CollectionKind
    {
      set
      {
        this._collectionKind = value;
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal bool TryGetFacet(FacetDescription description, out Facet facet)
    {
      switch (description.FacetName)
      {
        case "Nullable":
          if (this._nullable.HasValue)
          {
            facet = Facet.Create(description, this._nullable.GetValueAsObject());
            return true;
          }
          break;
        case "MaxLength":
          if (this._maxLength.HasValue)
          {
            facet = Facet.Create(description, this._maxLength.GetValueAsObject());
            return true;
          }
          break;
        case "Unicode":
          if (this._unicode.HasValue)
          {
            facet = Facet.Create(description, this._unicode.GetValueAsObject());
            return true;
          }
          break;
        case "FixedLength":
          if (this._fixedLength.HasValue)
          {
            facet = Facet.Create(description, this._fixedLength.GetValueAsObject());
            return true;
          }
          break;
        case "Precision":
          if (this._precision.HasValue)
          {
            facet = Facet.Create(description, this._precision.GetValueAsObject());
            return true;
          }
          break;
        case "Scale":
          if (this._scale.HasValue)
          {
            facet = Facet.Create(description, this._scale.GetValueAsObject());
            return true;
          }
          break;
        case "DefaultValue":
          if (this._defaultValue != null)
          {
            facet = Facet.Create(description, this._defaultValue);
            return true;
          }
          break;
        case "Collation":
          if (this._collation.HasValue)
          {
            facet = Facet.Create(description, this._collation.GetValueAsObject());
            return true;
          }
          break;
        case "SRID":
          if (this._srid.HasValue)
          {
            facet = Facet.Create(description, this._srid.GetValueAsObject());
            return true;
          }
          break;
        case "IsStrict":
          if (this._isStrict.HasValue)
          {
            facet = Facet.Create(description, this._isStrict.GetValueAsObject());
            return true;
          }
          break;
        case "StoreGeneratedPattern":
          if (this._storeGeneratedPattern.HasValue)
          {
            facet = Facet.Create(description, this._storeGeneratedPattern.GetValueAsObject());
            return true;
          }
          break;
        case "ConcurrencyMode":
          if (this._concurrencyMode.HasValue)
          {
            facet = Facet.Create(description, this._concurrencyMode.GetValueAsObject());
            return true;
          }
          break;
        case "CollectionKind":
          if (this._collectionKind.HasValue)
          {
            facet = Facet.Create(description, this._collectionKind.GetValueAsObject());
            return true;
          }
          break;
      }
      facet = (Facet) null;
      return false;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    public static FacetValues Create(IEnumerable<Facet> facets)
    {
      FacetValues facetValues = new FacetValues();
      foreach (Facet facet in facets)
      {
        switch (facet.Description.FacetName)
        {
          case "Nullable":
            facetValues.Nullable = (FacetValueContainer<bool?>) ((bool?) facet.Value);
            continue;
          case "MaxLength":
            EdmConstants.Unbounded unbounded1 = facet.Value as EdmConstants.Unbounded;
            facetValues.MaxLength = unbounded1 == null ? (FacetValueContainer<int?>) ((int?) facet.Value) : (FacetValueContainer<int?>) unbounded1;
            continue;
          case "Unicode":
            facetValues.Unicode = (FacetValueContainer<bool?>) ((bool?) facet.Value);
            continue;
          case "FixedLength":
            facetValues.FixedLength = (FacetValueContainer<bool?>) ((bool?) facet.Value);
            continue;
          case "Precision":
            EdmConstants.Unbounded unbounded2 = facet.Value as EdmConstants.Unbounded;
            facetValues.Precision = unbounded2 == null ? (FacetValueContainer<byte?>) ((byte?) facet.Value) : (FacetValueContainer<byte?>) unbounded2;
            continue;
          case "Scale":
            EdmConstants.Unbounded unbounded3 = facet.Value as EdmConstants.Unbounded;
            facetValues.Scale = unbounded3 == null ? (FacetValueContainer<byte?>) ((byte?) facet.Value) : (FacetValueContainer<byte?>) unbounded3;
            continue;
          case "DefaultValue":
            facetValues.DefaultValue = facet.Value;
            continue;
          case "Collation":
            facetValues.Collation = (FacetValueContainer<string>) ((string) facet.Value);
            continue;
          case "SRID":
            facetValues.Srid = (FacetValueContainer<int?>) ((int?) facet.Value);
            continue;
          case "IsStrict":
            facetValues.IsStrict = (FacetValueContainer<bool?>) ((bool?) facet.Value);
            continue;
          case "StoreGeneratedPattern":
            facetValues.StoreGeneratedPattern = (FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern?>) ((System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern?) facet.Value);
            continue;
          case "ConcurrencyMode":
            facetValues.ConcurrencyMode = (FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.ConcurrencyMode?>) ((System.Data.Entity.Core.Metadata.Edm.ConcurrencyMode?) facet.Value);
            continue;
          case "CollectionKind":
            facetValues.CollectionKind = (FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.CollectionKind?>) ((System.Data.Entity.Core.Metadata.Edm.CollectionKind?) facet.Value);
            continue;
          default:
            continue;
        }
      }
      return facetValues;
    }

    internal static FacetValues NullFacetValues
    {
      get
      {
        return new FacetValues()
        {
          FixedLength = (FacetValueContainer<bool?>) new bool?(),
          MaxLength = (FacetValueContainer<int?>) new int?(),
          Precision = (FacetValueContainer<byte?>) new byte?(),
          Scale = (FacetValueContainer<byte?>) new byte?(),
          Unicode = (FacetValueContainer<bool?>) new bool?(),
          Collation = (FacetValueContainer<string>) ((string) null),
          Srid = (FacetValueContainer<int?>) new int?(),
          IsStrict = (FacetValueContainer<bool?>) new bool?(),
          ConcurrencyMode = (FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.ConcurrencyMode?>) new System.Data.Entity.Core.Metadata.Edm.ConcurrencyMode?(),
          StoreGeneratedPattern = (FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern?>) new System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern?(),
          CollectionKind = (FacetValueContainer<System.Data.Entity.Core.Metadata.Edm.CollectionKind?>) new System.Data.Entity.Core.Metadata.Edm.CollectionKind?()
        };
      }
    }
  }
}
