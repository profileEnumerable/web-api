// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.RelationshipEndMember
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Initializes a new instance of the RelationshipEndMember class
  /// </summary>
  public abstract class RelationshipEndMember : EdmMember
  {
    private OperationAction _deleteBehavior;
    private RelationshipMultiplicity _relationshipMultiplicity;

    internal RelationshipEndMember(
      string name,
      RefType endRefType,
      RelationshipMultiplicity multiplicity)
      : base(name, TypeUsage.Create((EdmType) endRefType, new FacetValues()
      {
        Nullable = (FacetValueContainer<bool?>) new bool?(false)
      }))
    {
      this._relationshipMultiplicity = multiplicity;
      this._deleteBehavior = OperationAction.None;
    }

    /// <summary>Gets the operational behavior of this relationship end member.</summary>
    /// <returns>
    /// One of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.OperationAction" /> values. The default is
    /// <see cref="P:System.Data.Entity.Core.Metadata.Edm.OperationAction.None" />
    /// .
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.OperationAction, true)]
    public OperationAction DeleteBehavior
    {
      get
      {
        return this._deleteBehavior;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._deleteBehavior = value;
      }
    }

    /// <summary>Gets the multiplicity of this relationship end member.</summary>
    /// <returns>
    /// One of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipMultiplicity" /> values.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.RelationshipMultiplicity, false)]
    public RelationshipMultiplicity RelationshipMultiplicity
    {
      get
      {
        return this._relationshipMultiplicity;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._relationshipMultiplicity = value;
      }
    }

    /// <summary>Access the EntityType of the EndMember in an association.</summary>
    /// <returns>The EntityType of the EndMember in an association.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public EntityType GetEntityType()
    {
      if (this.TypeUsage == null)
        return (EntityType) null;
      return (EntityType) ((RefType) this.TypeUsage.EdmType).ElementType;
    }
  }
}
