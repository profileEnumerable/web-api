// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.TypeMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents the Mapping metadata for a type map in CS space.
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
  /// This class represents the metadata for all the Type map elements in the
  /// above example namely EntityTypeMapping, AssociationTypeMapping and CompositionTypeMapping.
  /// The TypeMapping elements contain TableMappingFragments which in turn contain the property maps.
  /// </example>
  public abstract class TypeMapping : MappingItem
  {
    internal TypeMapping()
    {
    }

    internal abstract EntitySetBaseMapping SetMapping { get; }

    internal abstract ReadOnlyCollection<EntityTypeBase> Types { get; }

    internal abstract ReadOnlyCollection<EntityTypeBase> IsOfTypes { get; }

    internal abstract ReadOnlyCollection<MappingFragment> MappingFragments { get; }
  }
}
