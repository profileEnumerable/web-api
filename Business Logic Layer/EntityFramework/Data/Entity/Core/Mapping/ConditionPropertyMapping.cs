// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ConditionPropertyMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Mapping metadata for Conditional property mapping on a type.
  /// Condition Property Mapping specifies a Condition either on the C side property or S side property.
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
  /// --ConditionProperyMap ( constant value--&gt;SMemberMetadata )
  /// --EntityTypeMapping
  /// --MappingFragment
  /// --EntityKey
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --ComplexPropertyMap
  /// --ComplexTypeMap
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --ScalarProperyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --ConditionProperyMap ( constant value--&gt;SMemberMetadata )
  /// --AssociationSetMapping
  /// --AssociationTypeMapping
  /// --MappingFragment
  /// --EndPropertyMap
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --ScalarProperyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// --EndPropertyMap
  /// --ScalarPropertyMap ( CMemberMetadata--&gt;SMemberMetadata )
  /// This class represents the metadata for all the condition property map elements in the
  /// above example.
  /// </example>
  public class ConditionPropertyMapping : PropertyMapping
  {
    private EdmProperty _column;
    private readonly object _value;
    private readonly bool? _isNull;

    internal ConditionPropertyMapping(EdmProperty propertyOrColumn, object value, bool? isNull)
    {
      DataSpace dataSpace = propertyOrColumn.TypeUsage.EdmType.DataSpace;
      switch (dataSpace)
      {
        case DataSpace.CSpace:
          base.Property = propertyOrColumn;
          break;
        case DataSpace.SSpace:
          this._column = propertyOrColumn;
          break;
        default:
          throw new ArgumentException(Strings.MetadataItem_InvalidDataSpace((object) dataSpace, (object) typeof (EdmProperty).Name), nameof (propertyOrColumn));
      }
      this._value = value;
      this._isNull = isNull;
    }

    internal ConditionPropertyMapping(
      EdmProperty property,
      EdmProperty column,
      object value,
      bool? isNull)
      : base(property)
    {
      this._column = column;
      this._value = value;
      this._isNull = isNull;
    }

    internal object Value
    {
      get
      {
        return this._value;
      }
    }

    internal bool? IsNull
    {
      get
      {
        return this._isNull;
      }
    }

    /// <summary>
    /// Gets an EdmProperty that specifies the mapped property.
    /// </summary>
    public override EdmProperty Property
    {
      get
      {
        return base.Property;
      }
      internal set
      {
        base.Property = value;
      }
    }

    /// <summary>Gets an EdmProperty that specifies the mapped column.</summary>
    public EdmProperty Column
    {
      get
      {
        return this._column;
      }
      internal set
      {
        this._column = value;
      }
    }
  }
}
