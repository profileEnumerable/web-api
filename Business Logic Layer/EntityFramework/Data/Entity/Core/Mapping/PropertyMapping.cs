// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.PropertyMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>Mapping metadata for all types of property mappings.</summary>
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
  /// This class represents the metadata for all property map elements in the
  /// above example. This includes the scalar property maps, complex property maps
  /// and end property maps.
  /// </example>
  public abstract class PropertyMapping : MappingItem
  {
    private EdmProperty _property;

    internal PropertyMapping(EdmProperty property)
    {
      this._property = property;
    }

    internal PropertyMapping()
    {
    }

    /// <summary>
    /// Gets an EdmProperty that specifies the mapped property.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property")]
    public virtual EdmProperty Property
    {
      get
      {
        return this._property;
      }
      internal set
      {
        this._property = value;
      }
    }
  }
}
