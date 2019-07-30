// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ComplexPropertyMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>Mapping metadata for Complex properties.</summary>
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
  /// This class represents the metadata for all the complex property map elements in the
  /// above example. ComplexPropertyMaps contain ComplexTypeMaps which define mapping based
  /// on the type of the ComplexProperty in case of inheritance.
  /// </example>
  public class ComplexPropertyMapping : PropertyMapping
  {
    private readonly List<ComplexTypeMapping> _typeMappings;

    /// <summary>Construct a new Complex Property mapping object</summary>
    /// <param name="property"> The MemberMetadata object that represents this Complex member </param>
    public ComplexPropertyMapping(EdmProperty property)
      : base(property)
    {
      Check.NotNull<EdmProperty>(property, nameof (property));
      if (!TypeSemantics.IsComplexType(property.TypeUsage))
        throw new ArgumentException(Strings.StorageComplexPropertyMapping_OnlyComplexPropertyAllowed, nameof (property));
      this._typeMappings = new List<ComplexTypeMapping>();
    }

    /// <summary>
    /// Gets a read only collections of type mappings corresponding to the
    /// nested complex types.
    /// </summary>
    public ReadOnlyCollection<ComplexTypeMapping> TypeMappings
    {
      get
      {
        return new ReadOnlyCollection<ComplexTypeMapping>((IList<ComplexTypeMapping>) this._typeMappings);
      }
    }

    /// <summary>
    /// Adds a type mapping corresponding to a nested complex type.
    /// </summary>
    /// <param name="typeMapping">The complex type mapping to be added.</param>
    public void AddTypeMapping(ComplexTypeMapping typeMapping)
    {
      Check.NotNull<ComplexTypeMapping>(typeMapping, nameof (typeMapping));
      this.ThrowIfReadOnly();
      this._typeMappings.Add(typeMapping);
    }

    /// <summary>
    /// Removes a type mapping corresponding to a nested complex type.
    /// </summary>
    /// <param name="typeMapping">The complex type mapping to be removed.</param>
    public void RemoveTypeMapping(ComplexTypeMapping typeMapping)
    {
      Check.NotNull<ComplexTypeMapping>(typeMapping, nameof (typeMapping));
      this.ThrowIfReadOnly();
      this._typeMappings.Remove(typeMapping);
    }

    internal override void SetReadOnly()
    {
      this._typeMappings.TrimExcess();
      MappingItem.SetReadOnly((IEnumerable<MappingItem>) this._typeMappings);
      base.SetReadOnly();
    }
  }
}
