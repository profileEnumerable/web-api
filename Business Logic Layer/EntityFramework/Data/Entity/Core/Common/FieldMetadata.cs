// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.FieldMetadata
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common
{
  /// <summary>
  /// FieldMetadata class providing the correlation between the column ordinals and MemberMetadata.
  /// </summary>
  [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
  public struct FieldMetadata
  {
    private readonly EdmMember _fieldType;
    private readonly int _ordinal;

    /// <summary>
    /// Initializes a new <see cref="T:System.Data.Entity.Core.Common.FieldMetadata" /> object with the specified ordinal value and field type.
    /// </summary>
    /// <param name="ordinal">An integer specified the location of the metadata.</param>
    /// <param name="fieldType">The field type.</param>
    public FieldMetadata(int ordinal, EdmMember fieldType)
    {
      if (ordinal < 0)
        throw new ArgumentOutOfRangeException(nameof (ordinal));
      Check.NotNull<EdmMember>(fieldType, nameof (fieldType));
      this._fieldType = fieldType;
      this._ordinal = ordinal;
    }

    /// <summary>
    /// Gets the type of field for this <see cref="T:System.Data.Entity.Core.Common.FieldMetadata" /> object.
    /// </summary>
    /// <returns>
    /// The type of field for this <see cref="T:System.Data.Entity.Core.Common.FieldMetadata" /> object.
    /// </returns>
    public EdmMember FieldType
    {
      get
      {
        return this._fieldType;
      }
    }

    /// <summary>
    /// Gets the ordinal for this <see cref="T:System.Data.Entity.Core.Common.FieldMetadata" /> object.
    /// </summary>
    /// <returns>An integer representing the ordinal value.</returns>
    public int Ordinal
    {
      get
      {
        return this._ordinal;
      }
    }
  }
}
