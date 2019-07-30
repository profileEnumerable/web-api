// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.EndPropertyMapping
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
  /// <summary>Mapping metadata for End property of an association.</summary>
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
  /// This class represents the metadata for all the end property map elements in the
  /// above example. EndPropertyMaps provide mapping for each end of the association.
  /// </example>
  public class EndPropertyMapping : PropertyMapping
  {
    private readonly List<ScalarPropertyMapping> _properties = new List<ScalarPropertyMapping>();
    private AssociationEndMember _associationEnd;

    /// <summary>Creates an association end property mapping.</summary>
    /// <param name="associationEnd">An AssociationEndMember that specifies
    /// the association end to be mapped.</param>
    public EndPropertyMapping(AssociationEndMember associationEnd)
    {
      Check.NotNull<AssociationEndMember>(associationEnd, nameof (associationEnd));
      this._associationEnd = associationEnd;
    }

    internal EndPropertyMapping()
    {
    }

    /// <summary>
    /// Gets an AssociationEndMember that specifies the mapped association end.
    /// </summary>
    public AssociationEndMember AssociationEnd
    {
      get
      {
        return this._associationEnd;
      }
      internal set
      {
        this._associationEnd = value;
      }
    }

    /// <summary>
    /// Gets a ReadOnlyCollection of ScalarPropertyMapping that specifies the children
    /// of this association end property mapping.
    /// </summary>
    public ReadOnlyCollection<ScalarPropertyMapping> PropertyMappings
    {
      get
      {
        return new ReadOnlyCollection<ScalarPropertyMapping>((IList<ScalarPropertyMapping>) this._properties);
      }
    }

    internal IEnumerable<EdmMember> StoreProperties
    {
      get
      {
        return (IEnumerable<EdmMember>) this.PropertyMappings.Select<ScalarPropertyMapping, EdmProperty>((Func<ScalarPropertyMapping, EdmProperty>) (propertyMap => propertyMap.Column));
      }
    }

    /// <summary>Adds a child property-column mapping.</summary>
    /// <param name="propertyMapping">A ScalarPropertyMapping that specifies
    /// the property-column mapping to be added.</param>
    public void AddPropertyMapping(ScalarPropertyMapping propertyMapping)
    {
      Check.NotNull<ScalarPropertyMapping>(propertyMapping, nameof (propertyMapping));
      this.ThrowIfReadOnly();
      this._properties.Add(propertyMapping);
    }

    /// <summary>Removes a child property-column mapping.</summary>
    /// <param name="propertyMapping">A ScalarPropertyMapping that specifies
    /// the property-column mapping to be removed.</param>
    public void RemovePropertyMapping(ScalarPropertyMapping propertyMapping)
    {
      Check.NotNull<ScalarPropertyMapping>(propertyMapping, nameof (propertyMapping));
      this.ThrowIfReadOnly();
      this._properties.Remove(propertyMapping);
    }

    internal override void SetReadOnly()
    {
      this._properties.TrimExcess();
      MappingItem.SetReadOnly((IEnumerable<MappingItem>) this._properties);
      base.SetReadOnly();
    }
  }
}
