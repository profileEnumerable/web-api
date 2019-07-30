// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.EntityTypeMapping
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
  /// <summary>
  /// Mapping metadata for Entity type.
  /// If an EntitySet represents entities of more than one type, than we will have
  /// more than one EntityTypeMapping for an EntitySet( For ex : if
  /// PersonSet Entity extent represents entities of types Person and Customer,
  /// than we will have two EntityType Mappings under mapping for PersonSet).
  /// </summary>
  /// <example>
  /// For Example if conceptually you could represent the CS MSL file as following
  /// --Mapping
  /// --EntityContainerMapping ( CNorthwind--&gt;SNorthwind )
  /// --EntitySetMapping
  /// --EntityTypeMapping
  /// --MappingFragment
  /// --EntityKey
  /// --ScalarPropertyMap
  /// --ScalarPropertyMap
  /// --EntityTypeMapping
  /// --MappingFragment
  /// --EntityKey
  /// --ScalarPropertyMap
  /// --ComplexPropertyMap
  /// --ScalarPropertyMap
  /// --ScalarProperyMap
  /// --ScalarPropertyMap
  /// --AssociationSetMapping
  /// --AssociationTypeMapping
  /// --MappingFragment
  /// --EndPropertyMap
  /// --ScalarPropertyMap
  /// --ScalarProperyMap
  /// --EndPropertyMap
  /// --ScalarPropertyMap
  /// This class represents the metadata for all entity Type map elements in the
  /// above example. Users can access the table mapping fragments under the
  /// entity type mapping through this class.
  /// </example>
  public class EntityTypeMapping : TypeMapping
  {
    private readonly Dictionary<string, EntityType> m_entityTypes = new Dictionary<string, EntityType>((IEqualityComparer<string>) StringComparer.Ordinal);
    private readonly Dictionary<string, EntityType> m_isOfEntityTypes = new Dictionary<string, EntityType>((IEqualityComparer<string>) StringComparer.Ordinal);
    private readonly EntitySetMapping _entitySetMapping;
    private readonly List<MappingFragment> _fragments;
    private EntityType _entityType;

    /// <summary>Creates an EntityTypeMapping instance.</summary>
    /// <param name="entitySetMapping">The EntitySetMapping that contains this EntityTypeMapping.</param>
    public EntityTypeMapping(EntitySetMapping entitySetMapping)
    {
      this._entitySetMapping = entitySetMapping;
      this._fragments = new List<MappingFragment>();
    }

    /// <summary>
    /// Gets the EntitySetMapping that contains this EntityTypeMapping.
    /// </summary>
    public EntitySetMapping EntitySetMapping
    {
      get
      {
        return this._entitySetMapping;
      }
    }

    internal override EntitySetBaseMapping SetMapping
    {
      get
      {
        return (EntitySetBaseMapping) this.EntitySetMapping;
      }
    }

    /// <summary>
    /// Gets the single EntityType being mapped. Throws exception in case of hierarchy type mapping.
    /// </summary>
    public EntityType EntityType
    {
      get
      {
        return this._entityType ?? (this._entityType = this.m_entityTypes.Values.SingleOrDefault<EntityType>());
      }
    }

    /// <summary>
    /// Gets a flag that indicates whether this is a type hierarchy mapping.
    /// </summary>
    public bool IsHierarchyMapping
    {
      get
      {
        if (this.m_isOfEntityTypes.Count <= 0)
          return this.m_entityTypes.Count > 1;
        return true;
      }
    }

    /// <summary>Gets a read-only collection of mapping fragments.</summary>
    public ReadOnlyCollection<MappingFragment> Fragments
    {
      get
      {
        return new ReadOnlyCollection<MappingFragment>((IList<MappingFragment>) this._fragments);
      }
    }

    internal override ReadOnlyCollection<MappingFragment> MappingFragments
    {
      get
      {
        return this.Fragments;
      }
    }

    /// <summary>Gets the mapped entity types.</summary>
    public ReadOnlyCollection<EntityTypeBase> EntityTypes
    {
      get
      {
        return new ReadOnlyCollection<EntityTypeBase>((IList<EntityTypeBase>) new List<EntityTypeBase>((IEnumerable<EntityTypeBase>) this.m_entityTypes.Values));
      }
    }

    internal override ReadOnlyCollection<EntityTypeBase> Types
    {
      get
      {
        return this.EntityTypes;
      }
    }

    /// <summary>Gets the mapped base types for a hierarchy mapping.</summary>
    public ReadOnlyCollection<EntityTypeBase> IsOfEntityTypes
    {
      get
      {
        return new ReadOnlyCollection<EntityTypeBase>((IList<EntityTypeBase>) new List<EntityTypeBase>((IEnumerable<EntityTypeBase>) this.m_isOfEntityTypes.Values));
      }
    }

    internal override ReadOnlyCollection<EntityTypeBase> IsOfTypes
    {
      get
      {
        return this.IsOfEntityTypes;
      }
    }

    /// <summary>Adds an entity type to the mapping.</summary>
    /// <param name="type">The EntityType to be added.</param>
    public void AddType(EntityType type)
    {
      Check.NotNull<EntityType>(type, nameof (type));
      this.ThrowIfReadOnly();
      this.m_entityTypes.Add(type.FullName, type);
    }

    /// <summary>Removes an entity type from the mapping.</summary>
    /// <param name="type">The EntityType to be removed.</param>
    public void RemoveType(EntityType type)
    {
      Check.NotNull<EntityType>(type, nameof (type));
      this.ThrowIfReadOnly();
      this.m_entityTypes.Remove(type.FullName);
    }

    /// <summary>
    /// Adds an entity type hierarchy to the mapping.
    /// The hierarchy is represented by the specified root entity type.
    /// </summary>
    /// <param name="type">The root EntityType of the hierarchy to be added.</param>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AddIs")]
    public void AddIsOfType(EntityType type)
    {
      Check.NotNull<EntityType>(type, nameof (type));
      this.ThrowIfReadOnly();
      this.m_isOfEntityTypes.Add(type.FullName, type);
    }

    /// <summary>
    /// Removes an entity type hierarchy from the mapping.
    /// The hierarchy is represented by the specified root entity type.
    /// </summary>
    /// <param name="type">The root EntityType of the hierarchy to be removed.</param>
    public void RemoveIsOfType(EntityType type)
    {
      Check.NotNull<EntityType>(type, nameof (type));
      this.ThrowIfReadOnly();
      this.m_isOfEntityTypes.Remove(type.FullName);
    }

    /// <summary>Adds a mapping fragment.</summary>
    /// <param name="fragment">The mapping fragment to be added.</param>
    public void AddFragment(MappingFragment fragment)
    {
      Check.NotNull<MappingFragment>(fragment, nameof (fragment));
      this.ThrowIfReadOnly();
      this._fragments.Add(fragment);
    }

    /// <summary>Removes a mapping fragment.</summary>
    /// <param name="fragment">The mapping fragment to be removed.</param>
    public void RemoveFragment(MappingFragment fragment)
    {
      Check.NotNull<MappingFragment>(fragment, nameof (fragment));
      this.ThrowIfReadOnly();
      this._fragments.Remove(fragment);
    }

    internal override void SetReadOnly()
    {
      this._fragments.TrimExcess();
      MappingItem.SetReadOnly((IEnumerable<MappingItem>) this._fragments);
      base.SetReadOnly();
    }

    internal EntityType GetContainerType(string memberName)
    {
      foreach (EntityType entityType in this.m_entityTypes.Values)
      {
        if (entityType.Properties.Contains(memberName))
          return entityType;
      }
      foreach (EntityType entityType in this.m_isOfEntityTypes.Values)
      {
        if (entityType.Properties.Contains(memberName))
          return entityType;
      }
      return (EntityType) null;
    }
  }
}
