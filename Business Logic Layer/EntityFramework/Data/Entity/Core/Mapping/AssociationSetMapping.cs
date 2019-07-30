// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.AssociationSetMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents the Mapping metadata for an AssociationSet in CS space.
  /// </summary>
  /// <example>
  /// For Example if conceptually you could represent the CS MSL file as following
  /// --Mapping
  /// --EntityContainerMapping ( CNorthwind--&gt;SNorthwind )
  /// --EntitySetMapping
  /// --EntityTypeMapping
  /// --MappingFragment
  /// --EntityTypeMapping
  /// --MappingFragment
  /// --AssociationSetMapping
  /// --AssociationTypeMapping
  /// --MappingFragment
  /// This class represents the metadata for the AssociationSetMapping elements in the
  /// above example. And it is possible to access the AssociationTypeMap underneath it.
  /// There will be only one TypeMap under AssociationSetMap.
  /// </example>
  public class AssociationSetMapping : EntitySetBaseMapping
  {
    private readonly AssociationSet _associationSet;
    private AssociationTypeMapping _associationTypeMapping;
    private AssociationSetModificationFunctionMapping _modificationFunctionMapping;

    /// <summary>Initializes a new AssociationSetMapping instance.</summary>
    /// <param name="associationSet">The association set to be mapped.</param>
    /// <param name="storeEntitySet">The store entity set to be mapped.</param>
    /// <param name="containerMapping">The parent container mapping.</param>
    public AssociationSetMapping(
      AssociationSet associationSet,
      EntitySet storeEntitySet,
      EntityContainerMapping containerMapping)
      : base(containerMapping)
    {
      Check.NotNull<AssociationSet>(associationSet, nameof (associationSet));
      Check.NotNull<EntitySet>(storeEntitySet, nameof (storeEntitySet));
      this._associationSet = associationSet;
      this._associationTypeMapping = new AssociationTypeMapping(associationSet.ElementType, this);
      this._associationTypeMapping.MappingFragment = new MappingFragment(storeEntitySet, (TypeMapping) this._associationTypeMapping, false);
    }

    internal AssociationSetMapping(AssociationSet associationSet, EntitySet storeEntitySet)
      : this(associationSet, storeEntitySet, (EntityContainerMapping) null)
    {
    }

    internal AssociationSetMapping(
      AssociationSet associationSet,
      EntityContainerMapping containerMapping)
      : base(containerMapping)
    {
      this._associationSet = associationSet;
    }

    /// <summary>Gets the association set that is mapped.</summary>
    public AssociationSet AssociationSet
    {
      get
      {
        return this._associationSet;
      }
    }

    internal override EntitySetBase Set
    {
      get
      {
        return (EntitySetBase) this.AssociationSet;
      }
    }

    /// <summary>Gets the contained association type mapping.</summary>
    public AssociationTypeMapping AssociationTypeMapping
    {
      get
      {
        return this._associationTypeMapping;
      }
      internal set
      {
        this._associationTypeMapping = value;
      }
    }

    internal override IEnumerable<TypeMapping> TypeMappings
    {
      get
      {
        yield return (TypeMapping) this._associationTypeMapping;
      }
    }

    /// <summary>
    /// Gets or sets the corresponding function mapping. Can be null.
    /// </summary>
    public AssociationSetModificationFunctionMapping ModificationFunctionMapping
    {
      get
      {
        return this._modificationFunctionMapping;
      }
      set
      {
        this.ThrowIfReadOnly();
        this._modificationFunctionMapping = value;
      }
    }

    /// <summary>Gets the store entity set that is mapped.</summary>
    public EntitySet StoreEntitySet
    {
      get
      {
        if (this.SingleFragment == null)
          return (EntitySet) null;
        return this.SingleFragment.StoreEntitySet;
      }
      internal set
      {
        this.SingleFragment.StoreEntitySet = value;
      }
    }

    internal EntityType Table
    {
      get
      {
        if (this.StoreEntitySet == null)
          return (EntityType) null;
        return this.StoreEntitySet.ElementType;
      }
    }

    /// <summary>Gets or sets the source end property mapping.</summary>
    public EndPropertyMapping SourceEndMapping
    {
      get
      {
        if (this.SingleFragment == null)
          return (EndPropertyMapping) null;
        return this.SingleFragment.PropertyMappings.OfType<EndPropertyMapping>().FirstOrDefault<EndPropertyMapping>();
      }
      set
      {
        Check.NotNull<EndPropertyMapping>(value, nameof (value));
        this.ThrowIfReadOnly();
        this.SingleFragment.AddPropertyMapping((PropertyMapping) value);
      }
    }

    /// <summary>Gets or sets the target end property mapping.</summary>
    public EndPropertyMapping TargetEndMapping
    {
      get
      {
        if (this.SingleFragment == null)
          return (EndPropertyMapping) null;
        return this.SingleFragment.PropertyMappings.OfType<EndPropertyMapping>().ElementAtOrDefault<EndPropertyMapping>(1);
      }
      set
      {
        Check.NotNull<EndPropertyMapping>(value, nameof (value));
        this.ThrowIfReadOnly();
        this.SingleFragment.AddPropertyMapping((PropertyMapping) value);
      }
    }

    /// <summary>Gets the property mapping conditions.</summary>
    public ReadOnlyCollection<ConditionPropertyMapping> Conditions
    {
      get
      {
        if (this.SingleFragment == null)
          return new ReadOnlyCollection<ConditionPropertyMapping>((IList<ConditionPropertyMapping>) new List<ConditionPropertyMapping>());
        return this.SingleFragment.Conditions;
      }
    }

    private MappingFragment SingleFragment
    {
      get
      {
        if (this._associationTypeMapping == null)
          return (MappingFragment) null;
        return this._associationTypeMapping.MappingFragment;
      }
    }

    /// <summary>Adds a property mapping condition.</summary>
    /// <param name="condition">The condition to add.</param>
    public void AddCondition(ConditionPropertyMapping condition)
    {
      Check.NotNull<ConditionPropertyMapping>(condition, nameof (condition));
      this.ThrowIfReadOnly();
      if (this.SingleFragment == null)
        return;
      this.SingleFragment.AddCondition(condition);
    }

    /// <summary>Removes a property mapping condition.</summary>
    /// <param name="condition">The property mapping condition to remove.</param>
    public void RemoveCondition(ConditionPropertyMapping condition)
    {
      Check.NotNull<ConditionPropertyMapping>(condition, nameof (condition));
      this.ThrowIfReadOnly();
      if (this.SingleFragment == null)
        return;
      this.SingleFragment.RemoveCondition(condition);
    }

    internal override void SetReadOnly()
    {
      MappingItem.SetReadOnly((MappingItem) this._associationTypeMapping);
      MappingItem.SetReadOnly((MappingItem) this._modificationFunctionMapping);
      base.SetReadOnly();
    }
  }
}
