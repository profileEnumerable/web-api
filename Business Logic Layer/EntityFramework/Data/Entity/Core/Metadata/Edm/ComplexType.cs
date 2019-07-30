// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ComplexType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Represents the Edm Complex Type.  This can be used to configure complex types
  /// from a conceptual-space model-based convention. Complex types are not supported in the store model.
  /// </summary>
  public class ComplexType : StructuralType
  {
    internal ComplexType(string name, string namespaceName, DataSpace dataSpace)
      : base(name, namespaceName, dataSpace)
    {
    }

    internal ComplexType()
    {
    }

    internal ComplexType(string name)
      : this(name, "Transient", DataSpace.CSpace)
    {
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.ComplexType" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ComplexType" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.ComplexType;
      }
    }

    /// <summary>
    /// Gets the list of properties for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.ComplexType" />.
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of properties for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ComplexType" />
    /// .
    /// </returns>
    public virtual ReadOnlyMetadataCollection<EdmProperty> Properties
    {
      get
      {
        return (ReadOnlyMetadataCollection<EdmProperty>) new FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember>(this.Members, new Predicate<EdmMember>(Helper.IsEdmProperty));
      }
    }

    internal override void ValidateMemberForAdd(EdmMember member)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.ComplexType" /> type.
    /// </summary>
    /// <param name="name">The name of the complex type.</param>
    /// <param name="namespaceName">The namespace of the complex type.</param>
    /// <param name="dataSpace">The dataspace to which the complex type belongs to.</param>
    /// <param name="members">Members of the complex type.</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the instance.</param>
    /// <exception cref="T:System.ArgumentNullException">Thrown if either name, namespace or members argument is null.</exception>
    /// <returns>
    /// A new instance a the <see cref="T:System.Data.Entity.Core.Metadata.Edm.ComplexType" /> type.
    /// </returns>
    /// <notes>
    /// The newly created <see cref="T:System.Data.Entity.Core.Metadata.Edm.ComplexType" /> will be read only.
    /// </notes>
    public static ComplexType Create(
      string name,
      string namespaceName,
      DataSpace dataSpace,
      IEnumerable<EdmMember> members,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotEmpty(namespaceName, nameof (namespaceName));
      Check.NotNull<IEnumerable<EdmMember>>(members, nameof (members));
      ComplexType complexType = new ComplexType(name, namespaceName, dataSpace);
      foreach (EdmMember member in members)
        complexType.AddMember(member);
      if (metadataProperties != null)
        complexType.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
      complexType.SetReadOnly();
      return complexType;
    }
  }
}
