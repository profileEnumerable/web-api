// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ComplexTypeMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>Mapping metadata for Complex Types.</summary>
  public class ComplexTypeMapping : StructuralTypeMapping
  {
    private readonly Dictionary<string, PropertyMapping> m_properties = new Dictionary<string, PropertyMapping>((IEqualityComparer<string>) StringComparer.Ordinal);
    private readonly Dictionary<EdmProperty, ConditionPropertyMapping> m_conditionProperties = new Dictionary<EdmProperty, ConditionPropertyMapping>((IEqualityComparer<EdmProperty>) EqualityComparer<EdmProperty>.Default);
    private readonly Dictionary<string, ComplexType> m_types = new Dictionary<string, ComplexType>((IEqualityComparer<string>) StringComparer.Ordinal);
    private readonly Dictionary<string, ComplexType> m_isOfTypes = new Dictionary<string, ComplexType>((IEqualityComparer<string>) StringComparer.Ordinal);

    /// <summary>Creates a ComplexTypeMapping instance.</summary>
    /// <param name="complexType">The ComplexType being mapped.</param>
    public ComplexTypeMapping(ComplexType complexType)
    {
      Check.NotNull<ComplexType>(complexType, nameof (complexType));
      this.AddType(complexType);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "isPartial")]
    internal ComplexTypeMapping(bool isPartial)
    {
    }

    /// <summary>Gets the ComplexType being mapped.</summary>
    public ComplexType ComplexType
    {
      get
      {
        return this.m_types.Values.SingleOrDefault<ComplexType>();
      }
    }

    internal ReadOnlyCollection<ComplexType> Types
    {
      get
      {
        return new ReadOnlyCollection<ComplexType>((IList<ComplexType>) new List<ComplexType>((IEnumerable<ComplexType>) this.m_types.Values));
      }
    }

    internal ReadOnlyCollection<ComplexType> IsOfTypes
    {
      get
      {
        return new ReadOnlyCollection<ComplexType>((IList<ComplexType>) new List<ComplexType>((IEnumerable<ComplexType>) this.m_isOfTypes.Values));
      }
    }

    /// <summary>Gets a read-only collection of property mappings.</summary>
    public override ReadOnlyCollection<PropertyMapping> PropertyMappings
    {
      get
      {
        return new ReadOnlyCollection<PropertyMapping>((IList<PropertyMapping>) new List<PropertyMapping>((IEnumerable<PropertyMapping>) this.m_properties.Values));
      }
    }

    /// <summary>
    /// Gets a read-only collection of property mapping conditions.
    /// </summary>
    public override ReadOnlyCollection<ConditionPropertyMapping> Conditions
    {
      get
      {
        return new ReadOnlyCollection<ConditionPropertyMapping>((IList<ConditionPropertyMapping>) new List<ConditionPropertyMapping>((IEnumerable<ConditionPropertyMapping>) this.m_conditionProperties.Values));
      }
    }

    internal ReadOnlyCollection<PropertyMapping> AllProperties
    {
      get
      {
        List<PropertyMapping> propertyMappingList = new List<PropertyMapping>();
        propertyMappingList.AddRange((IEnumerable<PropertyMapping>) this.m_properties.Values);
        propertyMappingList.AddRange((IEnumerable<PropertyMapping>) this.m_conditionProperties.Values);
        return new ReadOnlyCollection<PropertyMapping>((IList<PropertyMapping>) propertyMappingList);
      }
    }

    internal void AddType(ComplexType type)
    {
      this.m_types.Add(type.FullName, type);
    }

    internal void AddIsOfType(ComplexType type)
    {
      this.m_isOfTypes.Add(type.FullName, type);
    }

    /// <summary>Adds a property mapping.</summary>
    /// <param name="propertyMapping">The property mapping to be added.</param>
    public override void AddPropertyMapping(PropertyMapping propertyMapping)
    {
      Check.NotNull<PropertyMapping>(propertyMapping, nameof (propertyMapping));
      this.ThrowIfReadOnly();
      this.m_properties.Add(propertyMapping.Property.Name, propertyMapping);
    }

    /// <summary>Removes a property mapping.</summary>
    /// <param name="propertyMapping">The property mapping to be removed.</param>
    public override void RemovePropertyMapping(PropertyMapping propertyMapping)
    {
      Check.NotNull<PropertyMapping>(propertyMapping, nameof (propertyMapping));
      this.ThrowIfReadOnly();
      this.m_properties.Remove(propertyMapping.Property.Name);
    }

    /// <summary>Adds a property mapping condition.</summary>
    /// <param name="condition">The property mapping condition to be added.</param>
    public override void AddCondition(ConditionPropertyMapping condition)
    {
      Check.NotNull<ConditionPropertyMapping>(condition, nameof (condition));
      this.ThrowIfReadOnly();
      this.AddConditionProperty(condition, (Action<EdmMember>) (_ => {}));
    }

    /// <summary>Removes a property mapping condition.</summary>
    /// <param name="condition">The property mapping condition to be removed.</param>
    public override void RemoveCondition(ConditionPropertyMapping condition)
    {
      Check.NotNull<ConditionPropertyMapping>(condition, nameof (condition));
      this.ThrowIfReadOnly();
      this.m_conditionProperties.Remove(condition.Property ?? condition.Column);
    }

    internal override void SetReadOnly()
    {
      MappingItem.SetReadOnly((IEnumerable<MappingItem>) this.m_properties.Values);
      MappingItem.SetReadOnly((IEnumerable<MappingItem>) this.m_conditionProperties.Values);
      base.SetReadOnly();
    }

    internal void AddConditionProperty(
      ConditionPropertyMapping conditionPropertyMap,
      Action<EdmMember> duplicateMemberConditionError)
    {
      EdmProperty key = conditionPropertyMap.Property != null ? conditionPropertyMap.Property : conditionPropertyMap.Column;
      if (!this.m_conditionProperties.ContainsKey(key))
        this.m_conditionProperties.Add(key, conditionPropertyMap);
      else
        duplicateMemberConditionError((EdmMember) key);
    }

    internal ComplexType GetOwnerType(string memberName)
    {
      foreach (ComplexType complexType in this.m_types.Values)
      {
        EdmMember edmMember;
        if (complexType.Members.TryGetValue(memberName, false, out edmMember) && edmMember is EdmProperty)
          return complexType;
      }
      foreach (ComplexType complexType in this.m_isOfTypes.Values)
      {
        EdmMember edmMember;
        if (complexType.Members.TryGetValue(memberName, false, out edmMember) && edmMember is EdmProperty)
          return complexType;
      }
      return (ComplexType) null;
    }
  }
}
