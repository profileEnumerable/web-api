// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbPropertyValues
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Internal;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// A collection of all the properties for an underlying entity or complex object.
  /// </summary>
  /// <remarks>
  /// An instance of this class can be converted to an instance of the generic class
  /// using the Cast method.
  /// Complex properties in the underlying entity or complex object are represented in
  /// the property values as nested instances of this class.
  /// </remarks>
  public class DbPropertyValues
  {
    private readonly InternalPropertyValues _internalValues;

    internal DbPropertyValues(InternalPropertyValues internalValues)
    {
      this._internalValues = internalValues;
    }

    /// <summary>
    /// Creates an object of the underlying type for this dictionary and hydrates it with property
    /// values from this dictionary.
    /// </summary>
    /// <returns> The properties of this dictionary copied into a new object. </returns>
    public object ToObject()
    {
      return this._internalValues.ToObject();
    }

    /// <summary>
    /// Sets the values of this dictionary by reading values out of the given object.
    /// The given object can be of any type.  Any property on the object with a name that
    /// matches a property name in the dictionary and can be read will be read.  Other
    /// properties will be ignored.  This allows, for example, copying of properties from
    /// simple Data Transfer Objects (DTOs).
    /// </summary>
    /// <param name="obj"> The object to read values from. </param>
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "Naming is intentional.", MessageId = "obj")]
    public void SetValues(object obj)
    {
      Check.NotNull<object>(obj, nameof (obj));
      this._internalValues.SetValues(obj);
    }

    /// <summary>
    /// Creates a new dictionary containing copies of all the properties in this dictionary.
    /// Changes made to the new dictionary will not be reflected in this dictionary and vice versa.
    /// </summary>
    /// <returns> A clone of this dictionary. </returns>
    public DbPropertyValues Clone()
    {
      return new DbPropertyValues(this._internalValues.Clone());
    }

    /// <summary>
    /// Sets the values of this dictionary by reading values from another dictionary.
    /// The other dictionary must be based on the same type as this dictionary, or a type derived
    /// from the type for this dictionary.
    /// </summary>
    /// <param name="propertyValues"> The dictionary to read values from. </param>
    public void SetValues(DbPropertyValues propertyValues)
    {
      Check.NotNull<DbPropertyValues>(propertyValues, nameof (propertyValues));
      this._internalValues.SetValues(propertyValues._internalValues);
    }

    /// <summary>
    /// Gets the set of names of all properties in this dictionary as a read-only set.
    /// </summary>
    /// <value> The property names. </value>
    public IEnumerable<string> PropertyNames
    {
      get
      {
        return (IEnumerable<string>) this._internalValues.PropertyNames;
      }
    }

    /// <summary>
    /// Gets or sets the value of the property with the specified property name.
    /// The value may be a nested instance of this class.
    /// </summary>
    /// <param name="propertyName"> The property name. </param>
    /// <returns> The value of the property. </returns>
    public object this[string propertyName]
    {
      get
      {
        Check.NotEmpty(propertyName, nameof (propertyName));
        object obj = this._internalValues[propertyName];
        InternalPropertyValues internalValues = obj as InternalPropertyValues;
        if (internalValues != null)
          obj = (object) new DbPropertyValues(internalValues);
        return obj;
      }
      set
      {
        Check.NotEmpty(propertyName, nameof (propertyName));
        this._internalValues[propertyName] = value;
      }
    }

    /// <summary>
    /// Gets the value of the property just like using the indexed property getter but
    /// typed to the type of the generic parameter.  This is useful especially with
    /// nested dictionaries to avoid writing expressions with lots of casts.
    /// </summary>
    /// <typeparam name="TValue"> The type of the property. </typeparam>
    /// <param name="propertyName"> Name of the property. </param>
    /// <returns> The value of the property. </returns>
    public TValue GetValue<TValue>(string propertyName)
    {
      return (TValue) this[propertyName];
    }

    internal InternalPropertyValues InternalPropertyValues
    {
      get
      {
        return this._internalValues;
      }
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the current instance.
    /// </summary>
    /// <returns>The exact runtime type of the current instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
