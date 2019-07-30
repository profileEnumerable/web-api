// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Mapping.ColumnMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration.Mapping
{
  [DebuggerDisplay("{Column.Name}")]
  internal class ColumnMapping
  {
    private readonly EdmProperty _column;
    private readonly List<PropertyMappingSpecification> _propertyMappings;

    public ColumnMapping(EdmProperty column)
    {
      this._column = column;
      this._propertyMappings = new List<PropertyMappingSpecification>();
    }

    public EdmProperty Column
    {
      get
      {
        return this._column;
      }
    }

    public IList<PropertyMappingSpecification> PropertyMappings
    {
      get
      {
        return (IList<PropertyMappingSpecification>) this._propertyMappings;
      }
    }

    public void AddMapping(
      EntityType entityType,
      IList<EdmProperty> propertyPath,
      IEnumerable<ConditionPropertyMapping> conditions,
      bool isDefaultDiscriminatorCondition)
    {
      this._propertyMappings.Add(new PropertyMappingSpecification(entityType, propertyPath, (IList<ConditionPropertyMapping>) conditions.ToList<ConditionPropertyMapping>(), isDefaultDiscriminatorCondition));
    }
  }
}
