// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.CollectionType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Represents the Edm Collection Type</summary>
  public class CollectionType : EdmType
  {
    private readonly TypeUsage _typeUsage;

    internal CollectionType()
    {
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    internal CollectionType(EdmType elementType)
      : this(TypeUsage.Create(elementType))
    {
      this.DataSpace = elementType.DataSpace;
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    internal CollectionType(TypeUsage elementType)
      : base(CollectionType.GetIdentity(Check.NotNull<TypeUsage>(elementType, nameof (elementType))), "Transient", elementType.EdmType.DataSpace)
    {
      this._typeUsage = elementType;
      this.SetReadOnly();
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.CollectionType" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.CollectionType" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.CollectionType;
      }
    }

    /// <summary>
    /// Gets the instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> class that contains the type of the element that this current
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.CollectionType" />
    /// object includes and facets for that type.
    /// </summary>
    /// <returns>
    /// The instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> class that contains the type of the element that this current
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.CollectionType" />
    /// object includes and facets for that type.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.TypeUsage, false)]
    public virtual TypeUsage TypeUsage
    {
      get
      {
        return this._typeUsage;
      }
    }

    private static string GetIdentity(TypeUsage typeUsage)
    {
      StringBuilder builder = new StringBuilder(50);
      builder.Append("collection[");
      typeUsage.BuildIdentity(builder);
      builder.Append("]");
      return builder.ToString();
    }

    internal override bool EdmEquals(MetadataItem item)
    {
      if (object.ReferenceEquals((object) this, (object) item))
        return true;
      if (item == null || BuiltInTypeKind.CollectionType != item.BuiltInTypeKind)
        return false;
      return this.TypeUsage.EdmEquals((MetadataItem) ((CollectionType) item).TypeUsage);
    }
  }
}
