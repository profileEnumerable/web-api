// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.EdmTypeAttribute
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>Base attribute for schematized types</summary>
  public abstract class EdmTypeAttribute : Attribute
  {
    internal EdmTypeAttribute()
    {
    }

    /// <summary>The name of the type in the conceptual schema that maps to the class to which this attribute is applied.</summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that is the name.
    /// </returns>
    public string Name { get; set; }

    /// <summary>The namespace name of the entity object type or complex type in the conceptual schema that maps to this type.</summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that is the namespace name.
    /// </returns>
    public string NamespaceName { get; set; }
  }
}
