// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ValueConditionMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Specifies a mapping condition evaluated by comparing the value of
  /// a property or column with a given value.
  /// </summary>
  public class ValueConditionMapping : ConditionPropertyMapping
  {
    /// <summary>Creates a ValueConditionMapping instance.</summary>
    /// <param name="propertyOrColumn">An EdmProperty that specifies a property or column.</param>
    /// <param name="value">An object that specifies the value to compare with.</param>
    public ValueConditionMapping(EdmProperty propertyOrColumn, object value)
      : base(Check.NotNull<EdmProperty>(propertyOrColumn, nameof (propertyOrColumn)), Check.NotNull<object>(value, nameof (value)), new bool?())
    {
    }

    /// <summary>
    /// Gets an object that specifies the value to check against.
    /// </summary>
    public new object Value
    {
      get
      {
        return base.Value;
      }
    }
  }
}
