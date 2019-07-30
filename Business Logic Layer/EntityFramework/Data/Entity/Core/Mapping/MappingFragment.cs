// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.MappingFragment
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents the metadata for mapping fragment.
  /// A set of mapping fragments makes up the Set mappings( EntitySet, AssociationSet or CompositionSet )
  /// Each MappingFragment provides mapping for those properties of a type that map to a single table.
  /// </summary>
  /// <example>
  /// For Example if conceptually you could represent the CS MSL file as following
  /// --Mapping
  /// --EntityContainerMapping ( CNorthwind--&gt;SNorthwind )
  /// --EntitySetMapping
  /// --EntityTypeMapping
  /// --MappingFragment
  /// --EntityKey
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --EntityTypeMapping
  /// --MappingFragment
  /// --EntityKey
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --ComplexPropertyMap
  /// --ComplexTypeMapping
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --ScalarProperyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --DiscriminatorProperyMap ( constant value--&gt;SMemberMetadata )
  /// --ComplexTypeMapping
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --ScalarProperyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --DiscriminatorProperyMap ( constant value--&gt;SMemberMetadata )
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --AssociationSetMapping
  /// --AssociationTypeMapping
  /// --MappingFragment
  /// --EndPropertyMap
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --ScalarProperyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --EndPropertyMap
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// This class represents the metadata for all the mapping fragment elements in the
  /// above example. Users can access all the top level constructs of
  /// MappingFragment element like EntityKey map, Property Maps, Discriminator
  /// property through this mapping fragment class.
  /// </example>
  public class MappingFragment : StructuralTypeMapping
  {
    private readonly List<ColumnMappingBuilder> _columnMappings = new List<ColumnMappingBuilder>();
    private readonly Dictionary<EdmProperty, ConditionPropertyMapping> m_conditionProperties = new Dictionary<EdmProperty, ConditionPropertyMapping>((IEqualityComparer<EdmProperty>) EqualityComparer<EdmProperty>.Default);
    private readonly List<PropertyMapping> m_properties = new List<PropertyMapping>();
    private EntitySet m_tableExtent;
    private readonly TypeMapping m_typeMapping;
    private readonly bool m_isSQueryDistinct;

    /// <summary>Creates a MappingFragment instance.</summary>
    /// <param name="storeEntitySet">The EntitySet corresponding to the table of view being mapped.</param>
    /// <param name="typeMapping">The TypeMapping that contains this MappingFragment.</param>
    /// <param name="makeColumnsDistinct">Flag that indicates whether to include 'DISTINCT' when generating queries.</param>
    public MappingFragment(
      EntitySet storeEntitySet,
      TypeMapping typeMapping,
      bool makeColumnsDistinct)
    {
      Check.NotNull<EntitySet>(storeEntitySet, nameof (storeEntitySet));
      Check.NotNull<TypeMapping>(typeMapping, nameof (typeMapping));
      this.m_tableExtent = storeEntitySet;
      this.m_typeMapping = typeMapping;
      this.m_isSQueryDistinct = makeColumnsDistinct;
    }

    internal IEnumerable<ColumnMappingBuilder> ColumnMappings
    {
      get
      {
        return (IEnumerable<ColumnMappingBuilder>) this._columnMappings;
      }
    }

    internal void AddColumnMapping(ColumnMappingBuilder columnMappingBuilder)
    {
      Check.NotNull<ColumnMappingBuilder>(columnMappingBuilder, nameof (columnMappingBuilder));
      if (!columnMappingBuilder.PropertyPath.Any<EdmProperty>() || this._columnMappings.Contains(columnMappingBuilder))
        throw new ArgumentException(Strings.InvalidColumnBuilderArgument((object) "columnBuilderMapping"));
      this._columnMappings.Add(columnMappingBuilder);
      StructuralTypeMapping structuralTypeMapping = (StructuralTypeMapping) this;
      int index;
      EdmProperty property;
      for (index = 0; index < columnMappingBuilder.PropertyPath.Count - 1; ++index)
      {
        property = columnMappingBuilder.PropertyPath[index];
        ComplexPropertyMapping complexPropertyMapping = structuralTypeMapping.PropertyMappings.OfType<ComplexPropertyMapping>().SingleOrDefault<ComplexPropertyMapping>((Func<ComplexPropertyMapping, bool>) (pm => object.ReferenceEquals((object) pm.Property, (object) property)));
        ComplexTypeMapping typeMapping = (ComplexTypeMapping) null;
        if (complexPropertyMapping == null)
        {
          typeMapping = new ComplexTypeMapping(false);
          typeMapping.AddType(property.ComplexType);
          complexPropertyMapping = new ComplexPropertyMapping(property);
          complexPropertyMapping.AddTypeMapping(typeMapping);
          structuralTypeMapping.AddPropertyMapping((PropertyMapping) complexPropertyMapping);
        }
        structuralTypeMapping = (StructuralTypeMapping) (typeMapping ?? complexPropertyMapping.TypeMappings.Single<ComplexTypeMapping>());
      }
      property = columnMappingBuilder.PropertyPath[index];
      ScalarPropertyMapping scalarPropertyMapping1 = structuralTypeMapping.PropertyMappings.OfType<ScalarPropertyMapping>().SingleOrDefault<ScalarPropertyMapping>((Func<ScalarPropertyMapping, bool>) (pm => object.ReferenceEquals((object) pm.Property, (object) property)));
      if (scalarPropertyMapping1 == null)
      {
        ScalarPropertyMapping scalarPropertyMapping2 = new ScalarPropertyMapping(property, columnMappingBuilder.ColumnProperty);
        structuralTypeMapping.AddPropertyMapping((PropertyMapping) scalarPropertyMapping2);
        columnMappingBuilder.SetTarget(scalarPropertyMapping2);
      }
      else
        scalarPropertyMapping1.Column = columnMappingBuilder.ColumnProperty;
    }

    internal void RemoveColumnMapping(ColumnMappingBuilder columnMappingBuilder)
    {
      this._columnMappings.Remove(columnMappingBuilder);
      MappingFragment.RemoveColumnMapping((StructuralTypeMapping) this, (IEnumerable<EdmProperty>) columnMappingBuilder.PropertyPath);
    }

    private static void RemoveColumnMapping(
      StructuralTypeMapping structuralTypeMapping,
      IEnumerable<EdmProperty> propertyPath)
    {
      PropertyMapping propertyMapping = structuralTypeMapping.PropertyMappings.Single<PropertyMapping>((Func<PropertyMapping, bool>) (pm => object.ReferenceEquals((object) pm.Property, (object) propertyPath.First<EdmProperty>())));
      if (propertyMapping is ScalarPropertyMapping)
      {
        structuralTypeMapping.RemovePropertyMapping(propertyMapping);
      }
      else
      {
        ComplexPropertyMapping complexPropertyMapping = (ComplexPropertyMapping) propertyMapping;
        ComplexTypeMapping complexTypeMapping = complexPropertyMapping.TypeMappings.Single<ComplexTypeMapping>();
        MappingFragment.RemoveColumnMapping((StructuralTypeMapping) complexTypeMapping, propertyPath.Skip<EdmProperty>(1));
        if (complexTypeMapping.PropertyMappings.Any<PropertyMapping>())
          return;
        structuralTypeMapping.RemovePropertyMapping((PropertyMapping) complexPropertyMapping);
      }
    }

    /// <summary>
    /// Gets the EntitySet corresponding to the table or view being mapped.
    /// </summary>
    public EntitySet StoreEntitySet
    {
      get
      {
        return this.m_tableExtent;
      }
      internal set
      {
        this.m_tableExtent = value;
      }
    }

    internal EntitySet TableSet
    {
      get
      {
        return this.StoreEntitySet;
      }
      set
      {
        this.StoreEntitySet = value;
      }
    }

    internal EntityType Table
    {
      get
      {
        return this.m_tableExtent.ElementType;
      }
    }

    /// <summary>
    /// Gets the TypeMapping that contains this MappingFragment.
    /// </summary>
    public TypeMapping TypeMapping
    {
      get
      {
        return this.m_typeMapping;
      }
    }

    /// <summary>
    /// Gets a flag that indicates whether to include 'DISTINCT' when generating queries.
    /// </summary>
    public bool MakeColumnsDistinct
    {
      get
      {
        return this.m_isSQueryDistinct;
      }
    }

    internal bool IsSQueryDistinct
    {
      get
      {
        return this.MakeColumnsDistinct;
      }
    }

    internal ReadOnlyCollection<PropertyMapping> AllProperties
    {
      get
      {
        List<PropertyMapping> propertyMappingList = new List<PropertyMapping>();
        propertyMappingList.AddRange((IEnumerable<PropertyMapping>) this.m_properties);
        propertyMappingList.AddRange((IEnumerable<PropertyMapping>) this.m_conditionProperties.Values);
        return new ReadOnlyCollection<PropertyMapping>((IList<PropertyMapping>) propertyMappingList);
      }
    }

    /// <summary>Gets a read-only collection of property mappings.</summary>
    public override ReadOnlyCollection<PropertyMapping> PropertyMappings
    {
      get
      {
        return new ReadOnlyCollection<PropertyMapping>((IList<PropertyMapping>) this.m_properties);
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

    internal IEnumerable<ColumnMappingBuilder> FlattenedProperties
    {
      get
      {
        return MappingFragment.GetFlattenedProperties((IEnumerable<PropertyMapping>) this.m_properties, new List<EdmProperty>());
      }
    }

    private static IEnumerable<ColumnMappingBuilder> GetFlattenedProperties(
      IEnumerable<PropertyMapping> propertyMappings,
      List<EdmProperty> propertyPath)
    {
      foreach (PropertyMapping propertyMapping in propertyMappings)
      {
        propertyPath.Add(propertyMapping.Property);
        ComplexPropertyMapping storageComplexPropertyMapping = propertyMapping as ComplexPropertyMapping;
        if (storageComplexPropertyMapping != null)
        {
          foreach (ColumnMappingBuilder flattenedProperty in MappingFragment.GetFlattenedProperties((IEnumerable<PropertyMapping>) storageComplexPropertyMapping.TypeMappings.Single<ComplexTypeMapping>().PropertyMappings, propertyPath))
            yield return flattenedProperty;
        }
        else
        {
          ScalarPropertyMapping storageScalarPropertyMapping = propertyMapping as ScalarPropertyMapping;
          if (storageScalarPropertyMapping != null)
            yield return new ColumnMappingBuilder(storageScalarPropertyMapping.Column, (IList<EdmProperty>) propertyPath.ToList<EdmProperty>());
        }
        propertyPath.Remove(propertyMapping.Property);
      }
    }

    internal IEnumerable<ConditionPropertyMapping> ColumnConditions
    {
      get
      {
        return (IEnumerable<ConditionPropertyMapping>) this.m_conditionProperties.Values;
      }
    }

    internal int StartLineNumber { get; set; }

    internal int StartLinePosition { get; set; }

    internal string SourceLocation
    {
      get
      {
        return this.m_typeMapping.SetMapping.EntityContainerMapping.SourceLocation;
      }
    }

    /// <summary>Adds a property mapping.</summary>
    /// <param name="propertyMapping">The property mapping to be added.</param>
    public override void AddPropertyMapping(PropertyMapping propertyMapping)
    {
      Check.NotNull<PropertyMapping>(propertyMapping, nameof (propertyMapping));
      this.ThrowIfReadOnly();
      this.m_properties.Add(propertyMapping);
    }

    /// <summary>Removes a property mapping.</summary>
    /// <param name="propertyMapping">The property mapping to be removed.</param>
    public override void RemovePropertyMapping(PropertyMapping propertyMapping)
    {
      Check.NotNull<PropertyMapping>(propertyMapping, nameof (propertyMapping));
      this.ThrowIfReadOnly();
      this.m_properties.Remove(propertyMapping);
    }

    /// <summary>Adds a property mapping condition.</summary>
    /// <param name="condition">The property mapping condition to be added.</param>
    public override void AddCondition(ConditionPropertyMapping condition)
    {
      Check.NotNull<ConditionPropertyMapping>(condition, nameof (condition));
      this.ThrowIfReadOnly();
      this.AddConditionProperty(condition);
    }

    /// <summary>Removes a property mapping condition.</summary>
    /// <param name="condition">The property mapping condition to be removed.</param>
    public override void RemoveCondition(ConditionPropertyMapping condition)
    {
      Check.NotNull<ConditionPropertyMapping>(condition, nameof (condition));
      this.ThrowIfReadOnly();
      this.RemoveConditionProperty(condition);
    }

    internal void ClearConditions()
    {
      this.m_conditionProperties.Clear();
    }

    internal override void SetReadOnly()
    {
      this.m_properties.TrimExcess();
      MappingItem.SetReadOnly((IEnumerable<MappingItem>) this.m_properties);
      MappingItem.SetReadOnly((IEnumerable<MappingItem>) this.m_conditionProperties.Values);
      base.SetReadOnly();
    }

    internal void RemoveConditionProperty(ConditionPropertyMapping condition)
    {
      this.m_conditionProperties.Remove(condition.Property ?? condition.Column);
    }

    internal void AddConditionProperty(ConditionPropertyMapping conditionPropertyMap)
    {
      this.AddConditionProperty(conditionPropertyMap, (Action<EdmMember>) (_ => {}));
    }

    internal void AddConditionProperty(
      ConditionPropertyMapping conditionPropertyMap,
      Action<EdmMember> duplicateMemberConditionError)
    {
      EdmProperty key = conditionPropertyMap.Property ?? conditionPropertyMap.Column;
      if (!this.m_conditionProperties.ContainsKey(key))
        this.m_conditionProperties.Add(key, conditionPropertyMap);
      else
        duplicateMemberConditionError((EdmMember) key);
    }
  }
}
