// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Mappers.PropertyFilter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Mappers
{
  internal sealed class PropertyFilter
  {
    private readonly DbModelBuilderVersion _modelBuilderVersion;

    public PropertyFilter(DbModelBuilderVersion modelBuilderVersion = DbModelBuilderVersion.Latest)
    {
      this._modelBuilderVersion = modelBuilderVersion;
    }

    public IEnumerable<PropertyInfo> GetProperties(
      Type type,
      bool declaredOnly,
      IEnumerable<PropertyInfo> explicitlyMappedProperties = null,
      IEnumerable<Type> knownTypes = null,
      bool includePrivate = false)
    {
      explicitlyMappedProperties = explicitlyMappedProperties ?? Enumerable.Empty<PropertyInfo>();
      knownTypes = knownTypes ?? Enumerable.Empty<Type>();
      this.ValidatePropertiesForModelVersion(type, explicitlyMappedProperties);
      return (declaredOnly ? type.GetDeclaredProperties() : type.GetNonHiddenProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (!p.IsStatic())
          return p.IsValidStructuralProperty();
        return false;
      })).Select(p => new{ p = p, m = p.Getter() }).Where(_param1 =>
      {
        if (!includePrivate && !_param1.m.IsPublic && (!explicitlyMappedProperties.Contains<PropertyInfo>(_param1.p) && !knownTypes.Contains<Type>(_param1.p.PropertyType)) || declaredOnly && !type.BaseType().GetInstanceProperties().All<PropertyInfo>((Func<PropertyInfo, bool>) (bp => bp.Name != _param1.p.Name)) || !this.EdmV3FeaturesSupported && (PropertyFilter.IsEnumType(_param1.p.PropertyType) || PropertyFilter.IsSpatialType(_param1.p.PropertyType)))
          return false;
        if (!this.Ef6FeaturesSupported)
          return !_param1.p.PropertyType.IsNested;
        return true;
      }).Select(_param0 => _param0.p);
    }

    public void ValidatePropertiesForModelVersion(
      Type type,
      IEnumerable<PropertyInfo> explicitlyMappedProperties)
    {
      if (this._modelBuilderVersion == DbModelBuilderVersion.Latest || this.EdmV3FeaturesSupported)
        return;
      PropertyInfo propertyInfo = explicitlyMappedProperties.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (!PropertyFilter.IsEnumType(p.PropertyType))
          return PropertyFilter.IsSpatialType(p.PropertyType);
        return true;
      }));
      if (propertyInfo != (PropertyInfo) null)
        throw Error.UnsupportedUseOfV3Type((object) type.Name, (object) propertyInfo.Name);
    }

    public bool EdmV3FeaturesSupported
    {
      get
      {
        return this._modelBuilderVersion.GetEdmVersion() >= 3.0;
      }
    }

    public bool Ef6FeaturesSupported
    {
      get
      {
        if (this._modelBuilderVersion != DbModelBuilderVersion.Latest)
          return this._modelBuilderVersion >= DbModelBuilderVersion.V6_0;
        return true;
      }
    }

    private static bool IsEnumType(Type type)
    {
      type.TryUnwrapNullableType(out type);
      return type.IsEnum();
    }

    private static bool IsSpatialType(Type type)
    {
      type.TryUnwrapNullableType(out type);
      if (!(type == typeof (DbGeometry)))
        return type == typeof (DbGeography);
      return true;
    }
  }
}
