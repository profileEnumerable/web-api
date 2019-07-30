// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// This class represents a referential constraint between two entities specifying the "to" and "from" ends of the relationship.
  /// </summary>
  public sealed class ReferentialConstraint : MetadataItem
  {
    private RelationshipEndMember _fromRole;
    private RelationshipEndMember _toRole;
    private readonly ReadOnlyMetadataCollection<EdmProperty> _fromProperties;
    private readonly ReadOnlyMetadataCollection<EdmProperty> _toProperties;

    /// <summary>Constructs a new constraint on the relationship</summary>
    /// <param name="fromRole"> role from which the relationship originates </param>
    /// <param name="toRole"> role to which the relationship is linked/targeted to </param>
    /// <param name="fromProperties"> properties on entity type of to role which take part in the constraint </param>
    /// <param name="toProperties"> properties on entity type of from role which take part in the constraint </param>
    /// <exception cref="T:System.ArgumentNullException">Argument Null exception if any of the arguments is null</exception>
    public ReferentialConstraint(
      RelationshipEndMember fromRole,
      RelationshipEndMember toRole,
      IEnumerable<EdmProperty> fromProperties,
      IEnumerable<EdmProperty> toProperties)
    {
      Check.NotNull<RelationshipEndMember>(fromRole, nameof (fromRole));
      Check.NotNull<RelationshipEndMember>(toRole, nameof (toRole));
      Check.NotNull<IEnumerable<EdmProperty>>(fromProperties, nameof (fromProperties));
      Check.NotNull<IEnumerable<EdmProperty>>(toProperties, nameof (toProperties));
      this._fromRole = fromRole;
      this._toRole = toRole;
      this._fromProperties = new ReadOnlyMetadataCollection<EdmProperty>(new MetadataCollection<EdmProperty>(fromProperties));
      this._toProperties = new ReadOnlyMetadataCollection<EdmProperty>(new MetadataCollection<EdmProperty>(toProperties));
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.ReferentialConstraint;
      }
    }

    internal override string Identity
    {
      get
      {
        return this.FromRole.Name + "_" + this.ToRole.Name;
      }
    }

    /// <summary>
    /// Gets the "from role" that takes part in this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />
    /// .
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipEndMember" /> object that represents the "from role" that takes part in this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />
    /// .
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">Thrown if value passed into setter is null</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the ReferentialConstraint instance is in ReadOnly state</exception>
    [MetadataProperty(BuiltInTypeKind.RelationshipEndMember, false)]
    public RelationshipEndMember FromRole
    {
      get
      {
        return this._fromRole;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._fromRole = value;
      }
    }

    /// <summary>
    /// Gets the "to role" that takes part in this <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipEndMember" /> object that represents the "to role" that takes part in this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />
    /// .
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">Thrown if value passed into setter is null</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the ReferentialConstraint instance is in ReadOnly state</exception>
    [MetadataProperty(BuiltInTypeKind.RelationshipEndMember, false)]
    public RelationshipEndMember ToRole
    {
      get
      {
        return this._toRole;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._toRole = value;
      }
    }

    internal AssociationEndMember PrincipalEnd
    {
      get
      {
        return (AssociationEndMember) this.FromRole;
      }
    }

    internal AssociationEndMember DependentEnd
    {
      get
      {
        return (AssociationEndMember) this.ToRole;
      }
    }

    /// <summary>
    /// Gets the list of properties for the "from role" on which this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />
    /// is defined.
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of properties for "from role" on which this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />
    /// is defined.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.EdmProperty, true)]
    public ReadOnlyMetadataCollection<EdmProperty> FromProperties
    {
      get
      {
        if (!this.IsReadOnly && this._fromProperties.Count == 0)
          this._fromRole.GetEntityType().KeyMembers.Each<EdmMember>((Action<EdmMember>) (p => this._fromProperties.Source.Add((EdmProperty) p)));
        return this._fromProperties;
      }
    }

    /// <summary>
    /// Gets the list of properties for the "to role" on which this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />
    /// is defined.
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of properties for the "to role" on which this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint" />
    /// is defined.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.EdmProperty, true)]
    public ReadOnlyMetadataCollection<EdmProperty> ToProperties
    {
      get
      {
        return this._toProperties;
      }
    }

    /// <summary>
    /// Returns the combination of the names of the
    /// <see cref="P:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint.FromRole" />
    /// and the
    /// <see cref="P:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint.ToRole" />
    /// .
    /// </summary>
    /// <returns>
    /// The combination of the names of the
    /// <see cref="P:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint.FromRole" />
    /// and the
    /// <see cref="P:System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint.ToRole" />
    /// .
    /// </returns>
    public override string ToString()
    {
      return this.FromRole.Name + "_" + this.ToRole.Name;
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      this.FromProperties.Source.SetReadOnly();
      this.ToProperties.Source.SetReadOnly();
      base.SetReadOnly();
      this.FromRole?.SetReadOnly();
      this.ToRole?.SetReadOnly();
    }

    internal string BuildConstraintExceptionMessage()
    {
      string name1 = this.FromProperties.First<EdmProperty>().DeclaringType.Name;
      string name2 = this.ToProperties.First<EdmProperty>().DeclaringType.Name;
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      for (int index = 0; index < this.FromProperties.Count; ++index)
      {
        if (index > 0)
        {
          stringBuilder1.Append(", ");
          stringBuilder2.Append(", ");
        }
        stringBuilder1.Append(name1).Append('.').Append((object) this.FromProperties[index]);
        stringBuilder2.Append(name2).Append('.').Append((object) this.ToProperties[index]);
      }
      return Strings.RelationshipManager_InconsistentReferentialConstraintProperties((object) stringBuilder1.ToString(), (object) stringBuilder2.ToString());
    }
  }
}
