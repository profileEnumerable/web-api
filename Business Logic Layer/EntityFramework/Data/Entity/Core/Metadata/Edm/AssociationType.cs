// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.AssociationType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Describes an association/relationship between two entities in the conceptual model or a foreign key relationship
  /// between two tables in the store model. In the conceptual model the dependant class may or may not define a foreign key property.
  /// If a foreign key is defined the <see cref="P:System.Data.Entity.Core.Metadata.Edm.AssociationType.IsForeignKey" /> property will be true and the <see cref="P:System.Data.Entity.Core.Metadata.Edm.AssociationType.Constraint" /> property will contain details of the foreign keys
  /// </summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
  public class AssociationType : RelationshipType
  {
    internal volatile int Index = -1;
    private readonly ReadOnlyMetadataCollection<ReferentialConstraint> _referentialConstraints;
    private FilteredReadOnlyMetadataCollection<AssociationEndMember, EdmMember> _associationEndMembers;
    private bool _isForeignKey;

    internal AssociationType(
      string name,
      string namespaceName,
      bool foreignKey,
      DataSpace dataSpace)
      : base(name, namespaceName, dataSpace)
    {
      this._referentialConstraints = new ReadOnlyMetadataCollection<ReferentialConstraint>(new MetadataCollection<ReferentialConstraint>());
      this._isForeignKey = foreignKey;
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.AssociationType" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.AssociationType" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.AssociationType;
      }
    }

    /// <summary>
    /// Gets the list of ends for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.AssociationType" />.
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of ends for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.AssociationType" />
    /// .
    /// </returns>
    public ReadOnlyMetadataCollection<AssociationEndMember> AssociationEndMembers
    {
      get
      {
        if (this._associationEndMembers == null)
          Interlocked.CompareExchange<FilteredReadOnlyMetadataCollection<AssociationEndMember, EdmMember>>(ref this._associationEndMembers, new FilteredReadOnlyMetadataCollection<AssociationEndMember, EdmMember>(this.KeyMembers, new Predicate<EdmMember>(Helper.IsAssociationEndMember)), (FilteredReadOnlyMetadataCollection<AssociationEndMember, EdmMember>) null);
        return (ReadOnlyMetadataCollection<AssociationEndMember>) this._associationEndMembers;
      }
    }

    /// <summary>Gets or sets the referential constraint.</summary>
    /// <returns>The referential constraint.</returns>
    public ReferentialConstraint Constraint
    {
      get
      {
        return this.ReferentialConstraints.SingleOrDefault<ReferentialConstraint>();
      }
      set
      {
        Check.NotNull<ReferentialConstraint>(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        ReferentialConstraint constraint = this.Constraint;
        if (constraint != null)
          this.ReferentialConstraints.Source.Remove(constraint);
        this.AddReferentialConstraint(value);
        this._isForeignKey = true;
      }
    }

    internal AssociationEndMember SourceEnd
    {
      get
      {
        return this.KeyMembers.FirstOrDefault<EdmMember>() as AssociationEndMember;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        if (this.KeyMembers.Count == 0)
          this.AddKeyMember((EdmMember) value);
        else
          this.SetKeyMember(0, value);
      }
    }

    internal AssociationEndMember TargetEnd
    {
      get
      {
        return this.KeyMembers.ElementAtOrDefault<EdmMember>(1) as AssociationEndMember;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        if (this.KeyMembers.Count == 1)
          this.AddKeyMember((EdmMember) value);
        else
          this.SetKeyMember(1, value);
      }
    }

    private void SetKeyMember(int index, AssociationEndMember member)
    {
      int index1 = this.Members.IndexOf(this.KeyMembers.Source[index]);
      if (index1 >= 0)
        this.Members.Source[index1] = (EdmMember) member;
      this.KeyMembers.Source[index] = (EdmMember) member;
    }

    /// <summary>
    /// Gets the list of constraints for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.AssociationType" />.
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of constraints for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.AssociationType" />
    /// .
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.ReferentialConstraint, true)]
    public ReadOnlyMetadataCollection<ReferentialConstraint> ReferentialConstraints
    {
      get
      {
        return this._referentialConstraints;
      }
    }

    /// <summary>Gets the Boolean property value that specifies whether the column is a foreign key.</summary>
    /// <returns>A Boolean value that specifies whether the column is a foreign key. If true, the column is a foreign key. If false (default), the column is not a foreign key.</returns>
    [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
    public bool IsForeignKey
    {
      get
      {
        return this._isForeignKey;
      }
    }

    internal override void ValidateMemberForAdd(EdmMember member)
    {
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      base.SetReadOnly();
      this.ReferentialConstraints.Source.SetReadOnly();
    }

    internal void AddReferentialConstraint(ReferentialConstraint referentialConstraint)
    {
      this.ReferentialConstraints.Source.Add(referentialConstraint);
    }

    /// <summary>
    /// Creates a read-only AssociationType instance from the specified parameters.
    /// </summary>
    /// <param name="name">The name of the association type.</param>
    /// <param name="namespaceName">The namespace of the association type.</param>
    /// <param name="foreignKey">Flag that indicates a foreign key (FK) relationship.</param>
    /// <param name="dataSpace">The data space for the association type.</param>
    /// <param name="sourceEnd">The source association end member.</param>
    /// <param name="targetEnd">The target association end member.</param>
    /// <param name="constraint">A referential constraint.</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the instance.</param>
    /// <returns>The newly created AssociationType instance.</returns>
    /// <exception cref="T:System.ArgumentException">The specified name is null or empty.</exception>
    /// <exception cref="T:System.ArgumentException">The specified namespace is null or empty.</exception>
    public static AssociationType Create(
      string name,
      string namespaceName,
      bool foreignKey,
      DataSpace dataSpace,
      AssociationEndMember sourceEnd,
      AssociationEndMember targetEnd,
      ReferentialConstraint constraint,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotEmpty(namespaceName, nameof (namespaceName));
      AssociationType associationType = new AssociationType(name, namespaceName, foreignKey, dataSpace);
      if (sourceEnd != null)
        associationType.SourceEnd = sourceEnd;
      if (targetEnd != null)
        associationType.TargetEnd = targetEnd;
      if (constraint != null)
        associationType.AddReferentialConstraint(constraint);
      if (metadataProperties != null)
        associationType.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
      associationType.SetReadOnly();
      return associationType;
    }
  }
}
