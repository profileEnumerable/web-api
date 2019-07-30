// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Spatial.DbGeometryWellKnownValue
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Data.Entity.Spatial
{
  /// <summary>
  /// A data contract serializable representation of a <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> value.
  /// </summary>
  [DataContract]
  public sealed class DbGeometryWellKnownValue
  {
    /// <summary> Gets or sets the coordinate system identifier (SRID) of this value. </summary>
    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1)]
    public int CoordinateSystemId { get; set; }

    /// <summary> Gets or sets the well known text representation of this value. </summary>
    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2)]
    public string WellKnownText { get; set; }

    /// <summary> Gets or sets the well known binary representation of this value. </summary>
    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3)]
    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required for this feature")]
    public byte[] WellKnownBinary { get; set; }
  }
}
