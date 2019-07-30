// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.RefType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Class representing a ref type</summary>
  public class RefType : EdmType
  {
    private readonly EntityTypeBase _elementType;

    internal RefType()
    {
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    internal RefType(EntityType entityType)
      : base(RefType.GetIdentity((EntityTypeBase) Check.NotNull<EntityType>(entityType, nameof (entityType))), "Transient", entityType.DataSpace)
    {
      this._elementType = (EntityTypeBase) entityType;
      this.SetReadOnly();
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.RefType" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.RefType" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.RefType;
      }
    }

    /// <summary>
    /// Gets the entity type referenced by this <see cref="T:System.Data.Entity.Core.Metadata.Edm.RefType" />.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityTypeBase" /> object that represents the entity type referenced by this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.RefType" />
    /// .
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.EntityTypeBase, false)]
    public virtual EntityTypeBase ElementType
    {
      get
      {
        return this._elementType;
      }
    }

    private static string GetIdentity(EntityTypeBase entityTypeBase)
    {
      StringBuilder builder = new StringBuilder(50);
      builder.Append("reference[");
      entityTypeBase.BuildIdentity(builder);
      builder.Append("]");
      return builder.ToString();
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return this._elementType.GetHashCode() * 397 ^ typeof (RefType).GetHashCode();
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      RefType refType = obj as RefType;
      if (refType != null)
        return object.ReferenceEquals((object) refType._elementType, (object) this._elementType);
      return false;
    }
  }
}
