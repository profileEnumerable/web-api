// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.StructuralTypeMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.ObjectModel;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>Specifies a structural type mapping.</summary>
  public abstract class StructuralTypeMapping : MappingItem
  {
    /// <summary>Gets a read-only collection of property mappings.</summary>
    public abstract ReadOnlyCollection<PropertyMapping> PropertyMappings { get; }

    /// <summary>
    /// Gets a read-only collection of property mapping conditions.
    /// </summary>
    public abstract ReadOnlyCollection<ConditionPropertyMapping> Conditions { get; }

    /// <summary>Adds a property mapping.</summary>
    /// <param name="propertyMapping">The property mapping to be added.</param>
    public abstract void AddPropertyMapping(PropertyMapping propertyMapping);

    /// <summary>Removes a property mapping.</summary>
    /// <param name="propertyMapping">The property mapping to be removed.</param>
    public abstract void RemovePropertyMapping(PropertyMapping propertyMapping);

    /// <summary>Adds a property mapping condition.</summary>
    /// <param name="condition">The property mapping condition to be added.</param>
    public abstract void AddCondition(ConditionPropertyMapping condition);

    /// <summary>Removes a property mapping condition.</summary>
    /// <param name="condition">The property mapping condition to be removed.</param>
    public abstract void RemoveCondition(ConditionPropertyMapping condition);
  }
}
