// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.IsNullConditionMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Specifies a mapping condition evaluated by checking whether the value
  /// of the a property/column is null or not null.
  /// </summary>
  public class IsNullConditionMapping : ConditionPropertyMapping
  {
    /// <summary>Creates an IsNullConditionMapping instance.</summary>
    /// <param name="propertyOrColumn">An EdmProperty that specifies a property or column.</param>
    /// <param name="isNull">A boolean that indicates whether to perform a null or a not-null check.</param>
    public IsNullConditionMapping(EdmProperty propertyOrColumn, bool isNull)
      : base(Check.NotNull<EdmProperty>(propertyOrColumn, nameof (propertyOrColumn)), (object) null, new bool?(isNull))
    {
    }

    /// <summary>
    /// Gets a bool that specifies whether the condition is evaluated by performing a null check
    /// or a not-null check.
    /// </summary>
    public bool IsNull
    {
      get
      {
        return base.IsNull.Value;
      }
    }
  }
}
