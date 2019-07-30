// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataItem
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Represents the base item class for all the metadata</summary>
  /// <summary>Represents the base item class for all the metadata</summary>
  public abstract class MetadataItem
  {
    private static readonly EdmType[] _builtInTypes = new EdmType[40];
    private int _flags;
    private MetadataPropertyCollection _itemAttributes;
    private static readonly ReadOnlyCollection<FacetDescription> _generalFacetDescriptions;
    private static readonly FacetDescription _nullableFacetDescription;
    private static readonly FacetDescription _defaultValueFacetDescription;
    private static readonly FacetDescription _collectionKindFacetDescription;

    internal MetadataItem()
    {
    }

    internal MetadataItem(MetadataItem.MetadataFlags flags)
    {
      this._flags = (int) flags;
    }

    internal virtual IEnumerable<MetadataProperty> Annotations
    {
      get
      {
        return this.GetMetadataProperties().Where<MetadataProperty>((Func<MetadataProperty, bool>) (p => p.IsAnnotation));
      }
    }

    /// <summary>Gets the built-in type kind for this type.</summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this type.
    /// </returns>
    public abstract BuiltInTypeKind BuiltInTypeKind { get; }

    /// <summary>Gets the list of properties of the current type.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of properties of the current type.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.MetadataProperty, true)]
    public virtual ReadOnlyMetadataCollection<MetadataProperty> MetadataProperties
    {
      get
      {
        return this.GetMetadataProperties().AsReadOnlyMetadataCollection();
      }
    }

    internal MetadataPropertyCollection GetMetadataProperties()
    {
      if (this._itemAttributes == null)
      {
        MetadataPropertyCollection propertyCollection = new MetadataPropertyCollection(this);
        if (this.IsReadOnly)
          propertyCollection.SetReadOnly();
        Interlocked.CompareExchange<MetadataPropertyCollection>(ref this._itemAttributes, propertyCollection, (MetadataPropertyCollection) null);
      }
      return this._itemAttributes;
    }

    /// <summary>
    /// Adds or updates an annotation with the specified name and value.
    /// </summary>
    /// <remarks>
    /// If an annotation with the given name already exists then the value of that annotation
    /// is updated to the given value. If the given value is null then the annotation will be
    /// removed.
    /// </remarks>
    /// <param name="name">The name of the annotation property.</param>
    /// <param name="value">The value of the annotation property.</param>
    public void AddAnnotation(string name, object value)
    {
      Check.NotEmpty(name, nameof (name));
      MetadataProperty metadataProperty = this.Annotations.FirstOrDefault<MetadataProperty>((Func<MetadataProperty, bool>) (a => a.Name == name));
      if (metadataProperty != null)
      {
        if (value == null)
          this.RemoveAnnotation(name);
        else
          metadataProperty.Value = value;
      }
      else
      {
        if (value == null)
          return;
        this.GetMetadataProperties().Add(MetadataProperty.CreateAnnotation(name, value));
      }
    }

    /// <summary>Removes an annotation with the specified name.</summary>
    /// <param name="name">The name of the annotation property.</param>
    /// <returns>true if an annotation was removed; otherwise, false.</returns>
    public bool RemoveAnnotation(string name)
    {
      Check.NotEmpty(name, nameof (name));
      MetadataPropertyCollection metadataProperties = this.GetMetadataProperties();
      MetadataProperty metadataProperty;
      if (metadataProperties.TryGetValue(name, false, out metadataProperty))
        return metadataProperties.Remove(metadataProperty);
      return false;
    }

    internal MetadataCollection<MetadataProperty> RawMetadataProperties
    {
      get
      {
        return (MetadataCollection<MetadataProperty>) this._itemAttributes;
      }
    }

    /// <summary>Gets or sets the documentation associated with this type.</summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.Documentation" /> object that represents the documentation on this type.
    /// </returns>
    public Documentation Documentation { get; set; }

    internal abstract string Identity { get; }

    internal virtual bool EdmEquals(MetadataItem item)
    {
      if (item == null)
        return false;
      if (this == item)
        return true;
      if (this.BuiltInTypeKind == item.BuiltInTypeKind)
        return this.Identity == item.Identity;
      return false;
    }

    internal bool IsReadOnly
    {
      get
      {
        return this.GetFlag(MetadataItem.MetadataFlags.Readonly);
      }
    }

    internal virtual void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      if (this._itemAttributes != null)
        this._itemAttributes.SetReadOnly();
      this.SetFlag(MetadataItem.MetadataFlags.Readonly, true);
    }

    internal virtual void BuildIdentity(StringBuilder builder)
    {
      builder.Append(this.Identity);
    }

    internal void AddMetadataProperties(List<MetadataProperty> metadataProperties)
    {
      this.GetMetadataProperties().AddRange(metadataProperties);
    }

    internal DataSpace GetDataSpace()
    {
      switch (this._flags & 7)
      {
        case 1:
          return DataSpace.CSpace;
        case 2:
          return DataSpace.OSpace;
        case 3:
          return DataSpace.OCSpace;
        case 4:
          return DataSpace.SSpace;
        case 5:
          return DataSpace.CSSpace;
        default:
          return ~DataSpace.OSpace;
      }
    }

    internal void SetDataSpace(DataSpace space)
    {
      this._flags = (int) ((MetadataItem.MetadataFlags) (this._flags & -8) | MetadataItem.MetadataFlags.DataSpace & MetadataItem.Convert(space));
    }

    private static MetadataItem.MetadataFlags Convert(DataSpace space)
    {
      switch (space)
      {
        case DataSpace.OSpace:
          return MetadataItem.MetadataFlags.OSpace;
        case DataSpace.CSpace:
          return MetadataItem.MetadataFlags.CSpace;
        case DataSpace.SSpace:
          return MetadataItem.MetadataFlags.SSpace;
        case DataSpace.OCSpace:
          return MetadataItem.MetadataFlags.OCSpace;
        case DataSpace.CSSpace:
          return MetadataItem.MetadataFlags.CSSpace;
        default:
          return MetadataItem.MetadataFlags.None;
      }
    }

    internal ParameterMode GetParameterMode()
    {
      switch ((MetadataItem.MetadataFlags) (this._flags & 3584))
      {
        case MetadataItem.MetadataFlags.In:
          return ParameterMode.In;
        case MetadataItem.MetadataFlags.Out:
          return ParameterMode.Out;
        case MetadataItem.MetadataFlags.InOut:
          return ParameterMode.InOut;
        case MetadataItem.MetadataFlags.ReturnValue:
          return ParameterMode.ReturnValue;
        default:
          return ~ParameterMode.In;
      }
    }

    internal void SetParameterMode(ParameterMode mode)
    {
      this._flags = (int) ((MetadataItem.MetadataFlags) (this._flags & -3585) | MetadataItem.MetadataFlags.ParameterMode & MetadataItem.Convert(mode));
    }

    private static MetadataItem.MetadataFlags Convert(ParameterMode mode)
    {
      switch (mode)
      {
        case ParameterMode.In:
          return MetadataItem.MetadataFlags.In;
        case ParameterMode.Out:
          return MetadataItem.MetadataFlags.Out;
        case ParameterMode.InOut:
          return MetadataItem.MetadataFlags.InOut;
        case ParameterMode.ReturnValue:
          return MetadataItem.MetadataFlags.ReturnValue;
        default:
          return MetadataItem.MetadataFlags.ParameterMode;
      }
    }

    internal bool GetFlag(MetadataItem.MetadataFlags flag)
    {
      return flag == ((MetadataItem.MetadataFlags) this._flags & flag);
    }

    internal void SetFlag(MetadataItem.MetadataFlags flag, bool value)
    {
      SpinWait spinWait = new SpinWait();
      while (true)
      {
        int flags = this._flags;
        int num = value ? (int) ((MetadataItem.MetadataFlags) flags | flag) : (int) ((MetadataItem.MetadataFlags) flags & ~flag);
        if ((flags & 8) != 8)
        {
          if (flags != Interlocked.CompareExchange(ref this._flags, num, flags))
            spinWait.SpinOnce();
          else
            goto label_3;
        }
        else
          break;
      }
      if ((flag & MetadataItem.MetadataFlags.Readonly) == MetadataItem.MetadataFlags.Readonly)
        return;
      throw new InvalidOperationException(Strings.OperationOnReadOnlyItem);
label_3:;
    }

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static MetadataItem()
    {
      MetadataItem._builtInTypes[0] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[2] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[1] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[3] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[3] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[7] = (EdmType) new EnumType();
      MetadataItem._builtInTypes[6] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[8] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[9] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[10] = (EdmType) new EnumType();
      MetadataItem._builtInTypes[11] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[12] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[13] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[14] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[4] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[5] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[15] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[16] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[17] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[18] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[19] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[20] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[21] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[22] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[23] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[24] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[25] = (EdmType) new EnumType();
      MetadataItem._builtInTypes[26] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[27] = (EdmType) new EnumType();
      MetadataItem._builtInTypes[28] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[29] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[30] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[31] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[32] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[33] = (EdmType) new EnumType();
      MetadataItem._builtInTypes[34] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[35] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[36] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[37] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[38] = (EdmType) new ComplexType();
      MetadataItem._builtInTypes[39] = (EdmType) new ComplexType();
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem), "ItemType", false, (ComplexType) null);
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataProperty), "MetadataProperty", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.GlobalItem), "GlobalItem", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.TypeUsage), "TypeUsage", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType), "EdmType", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.GlobalItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.SimpleType), "SimpleType", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EnumType), "EnumType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.SimpleType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.PrimitiveType), "PrimitiveType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.SimpleType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.CollectionType), "CollectionType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.RefType), "RefType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmMember), "EdmMember", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmProperty), "EdmProperty", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmMember));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.NavigationProperty), "NavigationProperty", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmMember));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.ProviderManifest), "ProviderManifest", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.RelationshipEndMember), "RelationshipEnd", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmMember));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.AssociationEndMember), "AssociationEnd", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.RelationshipEndMember));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EnumMember), "EnumMember", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.ReferentialConstraint), "ReferentialConstraint", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.StructuralType), "StructuralType", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.RowType), "RowType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.StructuralType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.ComplexType), "ComplexType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.StructuralType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EntityTypeBase), "ElementType", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.StructuralType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EntityType), "EntityType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EntityTypeBase));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.RelationshipType), "RelationshipType", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EntityTypeBase));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.AssociationType), "AssociationType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.RelationshipType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.Facet), "Facet", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EntityContainer), "EntityContainerType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.GlobalItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EntitySetBase), "BaseEntitySetType", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EntitySet), "EntitySetType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EntitySetBase));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.RelationshipSet), "RelationshipSet", true, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EntitySetBase));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.AssociationSet), "AssocationSetType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.RelationshipSet));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.AssociationSetEnd), "AssociationSetEndType", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.FunctionParameter), "FunctionParameter", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmFunction), "EdmFunction", false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType));
      MetadataItem.InitializeBuiltInTypes((ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.Documentation), nameof (Documentation), false, (ComplexType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataItem));
      MetadataItem.InitializeEnumType(BuiltInTypeKind.OperationAction, "DeleteAction", new string[2]
      {
        "None",
        "Cascade"
      });
      MetadataItem.InitializeEnumType(BuiltInTypeKind.RelationshipMultiplicity, "RelationshipMultiplicity", new string[3]
      {
        "One",
        "ZeroToOne",
        "Many"
      });
      MetadataItem.InitializeEnumType(BuiltInTypeKind.ParameterMode, "ParameterMode", new string[3]
      {
        "In",
        "Out",
        "InOut"
      });
      MetadataItem.InitializeEnumType(BuiltInTypeKind.CollectionKind, "CollectionKind", new string[3]
      {
        "None",
        "List",
        "Bag"
      });
      MetadataItem.InitializeEnumType(BuiltInTypeKind.PrimitiveTypeKind, "PrimitiveTypeKind", Enum.GetNames(typeof (PrimitiveTypeKind)));
      FacetDescription[] facetDescriptionArray = new FacetDescription[2];
      MetadataItem._nullableFacetDescription = new FacetDescription("Nullable", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), new int?(), new int?(), (object) true);
      facetDescriptionArray[0] = MetadataItem._nullableFacetDescription;
      MetadataItem._defaultValueFacetDescription = new FacetDescription("DefaultValue", MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType), new int?(), new int?(), (object) null);
      facetDescriptionArray[1] = MetadataItem._defaultValueFacetDescription;
      MetadataItem._generalFacetDescriptions = new ReadOnlyCollection<FacetDescription>((IList<FacetDescription>) facetDescriptionArray);
      MetadataItem._collectionKindFacetDescription = new FacetDescription("CollectionKind", MetadataItem.GetBuiltInType(BuiltInTypeKind.EnumType), new int?(), new int?(), (object) null);
      TypeUsage typeUsage1 = TypeUsage.Create((EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.String));
      TypeUsage typeUsage2 = TypeUsage.Create((EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean));
      TypeUsage typeUsage3 = TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType));
      TypeUsage typeUsage4 = TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.TypeUsage));
      TypeUsage typeUsage5 = TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.ComplexType));
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.MetadataProperty, new EdmProperty[3]
      {
        new EdmProperty("Name", typeUsage1),
        new EdmProperty("TypeUsage", typeUsage4),
        new EdmProperty("Value", typeUsage5)
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.MetadataItem, new EdmProperty[2]
      {
        new EdmProperty(nameof (MetadataProperties), TypeUsage.Create((EdmType) MetadataItem.GetBuiltInType(BuiltInTypeKind.MetadataProperty).GetCollectionType())),
        new EdmProperty(nameof (Documentation), TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.Documentation)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.TypeUsage, new EdmProperty[2]
      {
        new EdmProperty("EdmType", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType))),
        new EdmProperty("Facets", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.Facet)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.EdmType, new EdmProperty[5]
      {
        new EdmProperty("Name", typeUsage1),
        new EdmProperty("Namespace", typeUsage1),
        new EdmProperty("Abstract", typeUsage2),
        new EdmProperty("Sealed", typeUsage2),
        new EdmProperty("BaseType", typeUsage5)
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.EnumType, new EdmProperty[1]
      {
        new EdmProperty("EnumMembers", typeUsage1)
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.CollectionType, new EdmProperty[1]
      {
        new EdmProperty("TypeUsage", typeUsage4)
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.RefType, new EdmProperty[1]
      {
        new EdmProperty("EntityType", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EntityType)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.EdmMember, new EdmProperty[2]
      {
        new EdmProperty("Name", typeUsage1),
        new EdmProperty("TypeUsage", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.TypeUsage)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.EdmProperty, new EdmProperty[2]
      {
        new EdmProperty("Nullable", typeUsage1),
        new EdmProperty("DefaultValue", typeUsage5)
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.NavigationProperty, new EdmProperty[2]
      {
        new EdmProperty("RelationshipTypeName", typeUsage1),
        new EdmProperty("ToEndMemberName", typeUsage1)
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.RelationshipEndMember, new EdmProperty[2]
      {
        new EdmProperty("OperationBehaviors", typeUsage5),
        new EdmProperty("RelationshipMultiplicity", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EnumType)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.EnumMember, new EdmProperty[1]
      {
        new EdmProperty("Name", typeUsage1)
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.ReferentialConstraint, new EdmProperty[4]
      {
        new EdmProperty("ToRole", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.RelationshipEndMember))),
        new EdmProperty("FromRole", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.RelationshipEndMember))),
        new EdmProperty("ToProperties", TypeUsage.Create((EdmType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmProperty).GetCollectionType())),
        new EdmProperty("FromProperties", TypeUsage.Create((EdmType) MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmProperty).GetCollectionType()))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.StructuralType, new EdmProperty[1]
      {
        new EdmProperty("Members", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmMember)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.EntityTypeBase, new EdmProperty[1]
      {
        new EdmProperty("KeyMembers", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmMember)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.Facet, new EdmProperty[3]
      {
        new EdmProperty("Name", typeUsage1),
        new EdmProperty("EdmType", typeUsage3),
        new EdmProperty("Value", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EdmType)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.EntityContainer, new EdmProperty[2]
      {
        new EdmProperty("Name", typeUsage1),
        new EdmProperty("EntitySets", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EntitySet)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.EntitySetBase, new EdmProperty[4]
      {
        new EdmProperty("Name", typeUsage1),
        new EdmProperty("EntityType", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EntityType))),
        new EdmProperty("Schema", typeUsage1),
        new EdmProperty("Table", typeUsage1)
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.AssociationSet, new EdmProperty[1]
      {
        new EdmProperty("AssociationSetEnds", TypeUsage.Create((EdmType) MetadataItem.GetBuiltInType(BuiltInTypeKind.AssociationSetEnd).GetCollectionType()))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.AssociationSetEnd, new EdmProperty[2]
      {
        new EdmProperty("Role", typeUsage1),
        new EdmProperty("EntitySetType", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EntitySet)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.FunctionParameter, new EdmProperty[3]
      {
        new EdmProperty("Name", typeUsage1),
        new EdmProperty("Mode", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.EnumType))),
        new EdmProperty("TypeUsage", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.TypeUsage)))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.EdmFunction, new EdmProperty[4]
      {
        new EdmProperty("Name", typeUsage1),
        new EdmProperty("Namespace", typeUsage1),
        new EdmProperty("ReturnParameter", TypeUsage.Create(MetadataItem.GetBuiltInType(BuiltInTypeKind.FunctionParameter))),
        new EdmProperty("Parameters", TypeUsage.Create((EdmType) MetadataItem.GetBuiltInType(BuiltInTypeKind.FunctionParameter).GetCollectionType()))
      });
      MetadataItem.AddBuiltInTypeProperties(BuiltInTypeKind.Documentation, new EdmProperty[2]
      {
        new EdmProperty("Summary", typeUsage1),
        new EdmProperty("LongDescription", typeUsage1)
      });
      for (int index = 0; index < MetadataItem._builtInTypes.Length; ++index)
        MetadataItem._builtInTypes[index].SetReadOnly();
    }

    internal static FacetDescription DefaultValueFacetDescription
    {
      get
      {
        return MetadataItem._defaultValueFacetDescription;
      }
    }

    internal static FacetDescription CollectionKindFacetDescription
    {
      get
      {
        return MetadataItem._collectionKindFacetDescription;
      }
    }

    internal static FacetDescription NullableFacetDescription
    {
      get
      {
        return MetadataItem._nullableFacetDescription;
      }
    }

    internal static EdmProviderManifest EdmProviderManifest
    {
      get
      {
        return EdmProviderManifest.Instance;
      }
    }

    /// <summary>
    /// Returns a conceptual model built-in type that matches one of the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" />
    /// values.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object that represents the built-in type in the EDM.
    /// </returns>
    /// <param name="builtInTypeKind">
    /// One of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> values.
    /// </param>
    public static EdmType GetBuiltInType(BuiltInTypeKind builtInTypeKind)
    {
      return MetadataItem._builtInTypes[(int) builtInTypeKind];
    }

    /// <summary>Returns the list of the general facet descriptions for a specified type.</summary>
    /// <returns>
    /// A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> object that represents the list of the general facet descriptions for a specified type.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static ReadOnlyCollection<FacetDescription> GetGeneralFacetDescriptions()
    {
      return MetadataItem._generalFacetDescriptions;
    }

    private static void InitializeBuiltInTypes(
      ComplexType builtInType,
      string name,
      bool isAbstract,
      ComplexType baseType)
    {
      EdmType.Initialize((EdmType) builtInType, name, "Edm", DataSpace.CSpace, isAbstract, (EdmType) baseType);
    }

    private static void AddBuiltInTypeProperties(
      BuiltInTypeKind builtInTypeKind,
      EdmProperty[] properties)
    {
      ComplexType builtInType = (ComplexType) MetadataItem.GetBuiltInType(builtInTypeKind);
      if (properties == null)
        return;
      for (int index = 0; index < properties.Length; ++index)
        builtInType.AddMember((EdmMember) properties[index]);
    }

    private static void InitializeEnumType(
      BuiltInTypeKind builtInTypeKind,
      string name,
      string[] enumMemberNames)
    {
      EnumType builtInType = (EnumType) MetadataItem.GetBuiltInType(builtInTypeKind);
      EdmType.Initialize((EdmType) builtInType, name, "Edm", DataSpace.CSpace, false, (EdmType) null);
      for (int index = 0; index < enumMemberNames.Length; ++index)
        builtInType.AddMember(new EnumMember(enumMemberNames[index], (object) index));
    }

    [Flags]
    internal enum MetadataFlags
    {
      None = 0,
      CSpace = 1,
      OSpace = 2,
      OCSpace = OSpace | CSpace, // 0x00000003
      SSpace = 4,
      CSSpace = SSpace | CSpace, // 0x00000005
      DataSpace = CSSpace | OSpace, // 0x00000007
      Readonly = 8,
      IsAbstract = 16, // 0x00000010
      In = 512, // 0x00000200
      Out = 1024, // 0x00000400
      InOut = Out | In, // 0x00000600
      ReturnValue = 2048, // 0x00000800
      ParameterMode = ReturnValue | InOut, // 0x00000E00
    }
  }
}
