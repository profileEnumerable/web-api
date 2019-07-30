// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ModificationFunctionResultBinding
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Globalization;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Defines a binding from a named result set column to a member taking the value.
  /// </summary>
  public sealed class ModificationFunctionResultBinding : MappingItem
  {
    private string _columnName;
    private readonly EdmProperty _property;

    /// <summary>
    /// Initializes a new ModificationFunctionResultBinding instance.
    /// </summary>
    /// <param name="columnName">The name of the column to bind from the function result set.</param>
    /// <param name="property">The property to be set on the entity.</param>
    public ModificationFunctionResultBinding(string columnName, EdmProperty property)
    {
      Check.NotNull<string>(columnName, nameof (columnName));
      Check.NotNull<EdmProperty>(property, nameof (property));
      this._columnName = columnName;
      this._property = property;
    }

    /// <summary>
    /// Gets the name of the column to bind from the function result set.
    /// </summary>
    public string ColumnName
    {
      get
      {
        return this._columnName;
      }
      internal set
      {
        this._columnName = value;
      }
    }

    /// <summary>Gets the property to be set on the entity.</summary>
    public EdmProperty Property
    {
      get
      {
        return this._property;
      }
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}->{1}", (object) this.ColumnName, (object) this.Property);
    }
  }
}
