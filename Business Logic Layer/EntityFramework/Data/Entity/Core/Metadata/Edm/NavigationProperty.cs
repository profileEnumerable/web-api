// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.NavigationProperty
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Represent the edm navigation property class</summary>
  public sealed class NavigationProperty : EdmMember
  {
    internal const string RelationshipTypeNamePropertyName = "RelationshipType";
    internal const string ToEndMemberNamePropertyName = "ToEndMember";
    private readonly NavigationPropertyAccessor _accessor;

    internal NavigationProperty(string name, TypeUsage typeUsage)
      : base(name, typeUsage)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<TypeUsage>(typeUsage, nameof (typeUsage));
      this._accessor = new NavigationPropertyAccessor(name);
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.NavigationProperty" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.NavigationProperty" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.NavigationProperty;
      }
    }

    /// <summary>Gets the relationship type that this navigation property operates on.</summary>
    /// <returns>The relationship type that this navigation property operates on.</returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the NavigationProperty instance is in ReadOnly state</exception>
    [MetadataProperty(BuiltInTypeKind.RelationshipType, false)]
    public RelationshipType RelationshipType { get; internal set; }

    /// <summary>Gets the "to" relationship end member of this navigation.</summary>
    /// <returns>The "to" relationship end member of this navigation.</returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the NavigationProperty instance is in ReadOnly state</exception>
    [MetadataProperty(BuiltInTypeKind.RelationshipEndMember, false)]
    public RelationshipEndMember ToEndMember { get; internal set; }

    /// <summary>Gets the "from" relationship end member in this navigation.</summary>
    /// <returns>The "from" relationship end member in this navigation.</returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the NavigationProperty instance is in ReadOnly state</exception>
    [MetadataProperty(BuiltInTypeKind.RelationshipEndMember, false)]
    public RelationshipEndMember FromEndMember { get; internal set; }

    internal AssociationType Association
    {
      get
      {
        return (AssociationType) this.RelationshipType;
      }
    }

    internal AssociationEndMember ResultEnd
    {
      get
      {
        return (AssociationEndMember) this.ToEndMember;
      }
    }

    internal NavigationPropertyAccessor Accessor
    {
      get
      {
        return this._accessor;
      }
    }

    /// <summary>
    /// Where the given navigation property is on the dependent end of a referential constraint,
    /// returns the foreign key properties. Otherwise, returns an empty set. We will return the members in the order
    /// of the principal end key properties.
    /// </summary>
    /// <returns>A collection of the foreign key properties.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public IEnumerable<EdmProperty> GetDependentProperties()
    {
      AssociationType relationshipType = (AssociationType) this.RelationshipType;
      if (relationshipType.ReferentialConstraints.Count > 0)
      {
        ReferentialConstraint referentialConstraint = relationshipType.ReferentialConstraints[0];
        if (referentialConstraint.ToRole.EdmEquals((MetadataItem) this.FromEndMember))
        {
          ReadOnlyMetadataCollection<EdmMember> keyMembers = referentialConstraint.FromRole.GetEntityType().KeyMembers;
          List<EdmProperty> edmPropertyList = new List<EdmProperty>(keyMembers.Count);
          for (int index = 0; index < keyMembers.Count; ++index)
            edmPropertyList.Add(referentialConstraint.ToProperties[referentialConstraint.FromProperties.IndexOf((EdmProperty) keyMembers[index])]);
          return (IEnumerable<EdmProperty>) new ReadOnlyCollection<EdmProperty>((IList<EdmProperty>) edmPropertyList);
        }
      }
      return Enumerable.Empty<EdmProperty>();
    }

    internal override void SetReadOnly()
    {
      if (!this.IsReadOnly && this.ToEndMember != null && this.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)
        this.TypeUsage = this.TypeUsage.ShallowCopy(Facet.Create(MetadataItem.NullableFacetDescription, (object) false));
      base.SetReadOnly();
    }

    /// <summary>
    /// Creates a NavigationProperty instance from the specified parameters.
    /// </summary>
    /// <param name="name">The name of the navigation property.</param>
    /// <param name="typeUsage">Specifies the navigation property type and its facets.</param>
    /// <param name="relationshipType">The relationship type for the navigation.</param>
    /// <param name="from">The source end member in the navigation.</param>
    /// <param name="to">The target end member in the navigation.</param>
    /// <param name="metadataProperties">The metadata properties of the navigation property.</param>
    /// <returns>The newly created NavigationProperty instance.</returns>
    public static NavigationProperty Create(
      string name,
      TypeUsage typeUsage,
      RelationshipType relationshipType,
      RelationshipEndMember from,
      RelationshipEndMember to,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<TypeUsage>(typeUsage, nameof (typeUsage));
      NavigationProperty navigationProperty = new NavigationProperty(name, typeUsage);
      navigationProperty.RelationshipType = relationshipType;
      navigationProperty.FromEndMember = from;
      navigationProperty.ToEndMember = to;
      if (metadataProperties != null)
        navigationProperty.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
      navigationProperty.SetReadOnly();
      return navigationProperty;
    }
  }
}
