// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EnumMember
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Represents an enumeration member.</summary>
  public sealed class EnumMember : MetadataItem
  {
    private readonly string _name;
    private readonly object _value;

    internal EnumMember(string name, object value)
      : base(MetadataItem.MetadataFlags.Readonly)
    {
      Check.NotEmpty(name, nameof (name));
      this._name = name;
      this._value = value;
    }

    /// <summary> Gets the kind of this type. </summary>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.EnumMember;
      }
    }

    /// <summary> Gets the name of this enumeration member. </summary>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public string Name
    {
      get
      {
        return this._name;
      }
    }

    /// <summary> Gets the value of this enumeration member. </summary>
    [MetadataProperty(BuiltInTypeKind.PrimitiveType, false)]
    public object Value
    {
      get
      {
        return this._value;
      }
    }

    internal override string Identity
    {
      get
      {
        return this.Name;
      }
    }

    /// <summary> Overriding System.Object.ToString to provide better String representation for this type. </summary>
    /// <returns>The name of this enumeration member.</returns>
    public override string ToString()
    {
      return this.Name;
    }

    /// <summary>Creates a read-only EnumMember instance.</summary>
    /// <param name="name">The name of the enumeration member.</param>
    /// <param name="value">The value of the enumeration member.</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the enumeration member.</param>
    /// <returns>The newly created EnumMember instance.</returns>
    /// <exception cref="T:System.ArgumentException">name is null or empty.</exception>
    [CLSCompliant(false)]
    public static EnumMember Create(
      string name,
      sbyte value,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      return EnumMember.CreateInternal(name, (object) value, metadataProperties);
    }

    /// <summary>Creates a read-only EnumMember instance.</summary>
    /// <param name="name">The name of the enumeration member.</param>
    /// <param name="value">The value of the enumeration member.</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the enumeration member.</param>
    /// <returns>The newly created EnumMember instance.</returns>
    /// <exception cref="T:System.ArgumentException">name is null or empty.</exception>
    public static EnumMember Create(
      string name,
      byte value,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      return EnumMember.CreateInternal(name, (object) value, metadataProperties);
    }

    /// <summary>Creates a read-only EnumMember instance.</summary>
    /// <param name="name">The name of the enumeration member.</param>
    /// <param name="value">The value of the enumeration member.</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the enumeration member.</param>
    /// <returns>The newly created EnumMember instance.</returns>
    /// <exception cref="T:System.ArgumentException">name is null or empty.</exception>
    public static EnumMember Create(
      string name,
      short value,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      return EnumMember.CreateInternal(name, (object) value, metadataProperties);
    }

    /// <summary>Creates a read-only EnumMember instance.</summary>
    /// <param name="name">The name of the enumeration member.</param>
    /// <param name="value">The value of the enumeration member.</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the enumeration member.</param>
    /// <returns>The newly created EnumMember instance.</returns>
    /// <exception cref="T:System.ArgumentException">name is null or empty.</exception>
    public static EnumMember Create(
      string name,
      int value,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      return EnumMember.CreateInternal(name, (object) value, metadataProperties);
    }

    /// <summary>Creates a read-only EnumMember instance.</summary>
    /// <param name="name">The name of the enumeration member.</param>
    /// <param name="value">The value of the enumeration member.</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the enumeration member.</param>
    /// <returns>The newly created EnumMember instance.</returns>
    /// <exception cref="T:System.ArgumentException">name is null or empty.</exception>
    public static EnumMember Create(
      string name,
      long value,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      return EnumMember.CreateInternal(name, (object) value, metadataProperties);
    }

    private static EnumMember CreateInternal(
      string name,
      object value,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      EnumMember enumMember = new EnumMember(name, value);
      if (metadataProperties != null)
        enumMember.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
      enumMember.SetReadOnly();
      return enumMember;
    }
  }
}
