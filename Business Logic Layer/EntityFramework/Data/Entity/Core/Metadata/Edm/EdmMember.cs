// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmMember
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Represents the edm member class</summary>
  public abstract class EdmMember : MetadataItem, INamedDataModelItem
  {
    private StructuralType _declaringType;
    private TypeUsage _typeUsage;
    private string _name;
    private string _identity;

    internal EdmMember()
    {
    }

    internal EdmMember(string name, TypeUsage memberTypeUsage)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<TypeUsage>(memberTypeUsage, nameof (memberTypeUsage));
      this._name = name;
      this._typeUsage = memberTypeUsage;
    }

    string INamedDataModelItem.Identity
    {
      get
      {
        return this.Identity;
      }
    }

    internal override string Identity
    {
      get
      {
        return this._identity ?? this.Name;
      }
    }

    /// <summary>
    /// Gets or sets the name of the property. Setting this from a store-space model-convention will change the name of the database
    /// column for this property. In the conceptual model, this should align with the corresponding property from the entity class
    /// and should not be changed.
    /// </summary>
    /// <returns>The name of this member.</returns>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public virtual string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        if (string.Equals(this._name, value, StringComparison.Ordinal))
          return;
        string identity = this.Identity;
        this._name = value;
        if (this._declaringType == null)
          return;
        if (this._declaringType.Members.Except<EdmMember>((IEnumerable<EdmMember>) new EdmMember[1]
        {
          this
        }).Any<EdmMember>((Func<EdmMember, bool>) (c => string.Equals(this.Identity, c.Identity, StringComparison.Ordinal))))
          this._identity = this._declaringType.Members.Select<EdmMember, string>((Func<EdmMember, string>) (i => i.Identity)).Uniquify(this.Identity);
        this._declaringType.NotifyItemIdentityChanged(this, identity);
      }
    }

    /// <summary>Gets the type on which this member is declared.</summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the type on which this member is declared.
    /// </returns>
    public virtual StructuralType DeclaringType
    {
      get
      {
        return this._declaringType;
      }
    }

    /// <summary>
    /// Gets the instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> class that contains both the type of the member and facets for the type.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> object that contains both the type of the member and facets for the type.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.TypeUsage, false)]
    public virtual TypeUsage TypeUsage
    {
      get
      {
        return this._typeUsage;
      }
      protected set
      {
        Check.NotNull<TypeUsage>(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._typeUsage = value;
      }
    }

    /// <summary>Returns the name of this member.</summary>
    /// <returns>The name of this member.</returns>
    public override string ToString()
    {
      return this.Name;
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      base.SetReadOnly();
      string identity = this._identity;
      this._identity = this.Name;
      if (this._declaringType == null || identity == null || string.Equals(identity, this._identity, StringComparison.Ordinal))
        return;
      this._declaringType.NotifyItemIdentityChanged(this, identity);
    }

    internal void ChangeDeclaringTypeWithoutCollectionFixup(StructuralType newDeclaringType)
    {
      this._declaringType = newDeclaringType;
    }

    /// <summary>
    /// Tells whether this member is marked as a Computed member in the EDM definition
    /// </summary>
    public bool IsStoreGeneratedComputed
    {
      get
      {
        Facet facet;
        if (this.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out facet))
          return (StoreGeneratedPattern) facet.Value == StoreGeneratedPattern.Computed;
        return false;
      }
    }

    /// <summary>
    /// Tells whether this member's Store generated pattern is marked as Identity in the EDM definition
    /// </summary>
    public bool IsStoreGeneratedIdentity
    {
      get
      {
        Facet facet;
        if (this.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out facet))
          return (StoreGeneratedPattern) facet.Value == StoreGeneratedPattern.Identity;
        return false;
      }
    }

    internal virtual bool IsPrimaryKeyColumn
    {
      get
      {
        EntityTypeBase declaringType = this._declaringType as EntityTypeBase;
        if (declaringType != null)
          return declaringType.KeyMembers.Contains(this);
        return false;
      }
    }
  }
}
