// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Mapping.PropertyMappingSpecification
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.ModelConfiguration.Configuration.Mapping
{
  internal class PropertyMappingSpecification
  {
    private readonly EntityType _entityType;
    private readonly IList<EdmProperty> _propertyPath;
    private readonly IList<ConditionPropertyMapping> _conditions;
    private readonly bool _isDefaultDiscriminatorCondition;

    public PropertyMappingSpecification(
      EntityType entityType,
      IList<EdmProperty> propertyPath,
      IList<ConditionPropertyMapping> conditions,
      bool isDefaultDiscriminatorCondition)
    {
      this._entityType = entityType;
      this._propertyPath = propertyPath;
      this._conditions = conditions;
      this._isDefaultDiscriminatorCondition = isDefaultDiscriminatorCondition;
    }

    public EntityType EntityType
    {
      get
      {
        return this._entityType;
      }
    }

    public IList<EdmProperty> PropertyPath
    {
      get
      {
        return this._propertyPath;
      }
    }

    public IList<ConditionPropertyMapping> Conditions
    {
      get
      {
        return this._conditions;
      }
    }

    public bool IsDefaultDiscriminatorCondition
    {
      get
      {
        return this._isDefaultDiscriminatorCondition;
      }
    }
  }
}
